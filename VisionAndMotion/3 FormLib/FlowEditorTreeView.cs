using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace VMPro
{
    /// <summary>
    /// 流程编辑器专用树。节点和连线都在 TreeView 的 UI 重绘周期内完成，
    /// 不创建后台画线线程，也不持有临时 Graphics 对象。
    /// </summary>
    internal sealed class FlowEditorTreeView : TreeView
    {
        private const int WmHScroll = 0x0114;
        private const int WmVScroll = 0x0115;
        private const int TvmSetExtendedStyle = 0x1100 + 44;
        private const int TvsExDoubleBuffer = 0x0004;

        private static readonly Color InputColor = Color.FromArgb(139, 92, 170);
        private static readonly Color OutputColor = Color.FromArgb(38, 132, 196);
        private static readonly Color SuccessColor = Color.FromArgb(48, 151, 104);
        private static readonly Color WarningColor = Color.FromArgb(205, 139, 43);
        private static readonly Color DisabledColor = Color.FromArgb(150, 162, 172);
        private static readonly Color[] ConnectionPalette =
        {
            Color.FromArgb(49, 132, 196),
            Color.FromArgb(42, 150, 137),
            Color.FromArgb(132, 102, 190),
            Color.FromArgb(214, 139, 45),
            Color.FromArgb(203, 91, 91),
            Color.FromArgb(67, 155, 104)
        };

        private Func<KeyValuePair<TreeNode, TreeNode>[]> connectionProvider;
        private Func<bool> connectionVisibilityProvider;
        private readonly ConnectionLayer connectionLayer;
        private int refreshPending;
        private int lastDoubleClickTick = int.MinValue;

        /// <summary>
        /// 抑制双击引发的被动收起：双击已展开节点的第二击会让 TreeView 收起分支，
        /// 但流程编辑器里双击有专门语义（打开工具编辑页），被动收起会让后续命中坐标漂移、
        /// 且表现为“展开状态被意外收起”。仅拦截紧跟双击的收起，用户点 +/- 折叠不受影响。
        /// </summary>
        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            int delta = unchecked(Environment.TickCount - lastDoubleClickTick);
            if (delta >= 0 && delta < 500)
                e.Cancel = true;
            base.OnBeforeCollapse(e);
        }

        internal FlowEditorTreeView()
            : this(null, null)
        {
        }

        internal FlowEditorTreeView(
            Func<KeyValuePair<TreeNode, TreeNode>[]> connectionProvider,
            Func<bool> connectionVisibilityProvider)
        {
            this.connectionProvider = connectionProvider;
            this.connectionVisibilityProvider = connectionVisibilityProvider;

            BackColor = ModernUiTheme.Surface;
            ForeColor = ModernUiTheme.PrimaryText;
            BorderStyle = BorderStyle.None;
            DrawMode = TreeViewDrawMode.OwnerDrawAll;
            FullRowSelect = true;
            HideSelection = false;
            // TreeView 的原生 HotTracking 会在每次鼠标移动时局部擦除行背景；
            // 连线若直接补画在同一 HWND 上就会被反复擦除，表现为闪烁。
            HotTracking = false;
            Indent = 24;
            ItemHeight = 32;
            ShowLines = false;
            ShowPlusMinus = false;
            ShowRootLines = false;
            ShowNodeToolTips = true;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            connectionLayer = new ConnectionLayer(this);
            Controls.Add(connectionLayer);
            UpdateConnectionLayerBounds();
        }

        internal int LastRenderedConnectionCount { get; private set; }

        internal int ConnectionGutterWidth
        {
            get { return Math.Max(64, Math.Min(112, ClientSize.Width / 3)); }
        }

        /// <summary>
        /// 连接点必须完整位于连线覆盖层左侧，避免覆盖层清理背景时裁掉右半边。
        /// </summary>
        internal int ConnectionPortCenterX
        {
            get { return Math.Max(6, GetContentRight() - 9); }
        }

        internal bool ShowAllConnections
        {
            get { return ResolveShowAllConnections(); }
        }

        internal void SetConnectionProvider(
            Func<KeyValuePair<TreeNode, TreeNode>[]> provider,
            Func<bool> visibilityProvider)
        {
            connectionProvider = provider;
            connectionVisibilityProvider = visibilityProvider;
            InvalidateConnectionLayer();
        }

        /// <summary>
        /// 合并运行线程短时间内产生的重复刷新，防止 BeginInvoke 队列反过来拖慢流程。
        /// </summary>
        internal void RequestConnectionRefresh()
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
                return;

            if (!InvokeRequired)
            {
                InvalidateConnectionLayer();
                return;
            }

            if (Interlocked.Exchange(ref refreshPending, 1) != 0)
                return;

            try
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    Interlocked.Exchange(ref refreshPending, 0);
                    if (!IsDisposed && !Disposing)
                        InvalidateConnectionLayer();
                }));
            }
            catch
            {
                Interlocked.Exchange(ref refreshPending, 0);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                SendMessage(Handle, TvmSetExtendedStyle,
                    new IntPtr(TvsExDoubleBuffer), new IntPtr(TvsExDoubleBuffer));
                UpdateConnectionLayerBounds();
                connectionLayer.BringToFront();
            }
            catch
            {
                // 双缓冲不可用时仍可使用系统 TreeView 的正常绘制。
            }
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (e == null || e.Node == null || e.Bounds.Height <= 0)
                return;

            Graphics graphics = e.Graphics;
            SmoothingMode oldSmoothing = graphics.SmoothingMode;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int contentRight = GetContentRight();
            bool selected = e.Node == SelectedNode;
            bool toolNode = e.Node.Level == 0;
            Rectangle visualBounds = GetNodeVisualBounds(e.Node, e.Bounds, contentRight);

            if (visualBounds.Width > 2 && visualBounds.Height > 2)
            {
                if (toolNode)
                    DrawToolNode(graphics, e.Node, visualBounds, selected);
                else
                    DrawPortNode(graphics, e.Node, visualBounds, selected);
            }

            graphics.SmoothingMode = oldSmoothing;
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);
            UpdateConnectionLayerBounds();
            InvalidateConnectionLayer();
        }

        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            base.OnAfterCollapse(e);
            UpdateConnectionLayerBounds();
            InvalidateConnectionLayer();
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            // 聚焦模式下只有选中具体输入/输出端口时才绘制对应连线。
            InvalidateConnectionLayer();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            InvalidateConnectionLayer();
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Left && e.Node != null &&
                e.Node.Level == 0 && e.Node.Nodes.Count > 0)
            {
                Rectangle bounds = GetNodeVisualBounds(e.Node, e.Node.Bounds, GetContentRight());
                Rectangle chevronBounds = new Rectangle(bounds.Left + 8,
                    bounds.Top + (bounds.Height - 14) / 2, 14, 14);
                if (chevronBounds.Contains(e.Location))
                {
                    if (e.Node.IsExpanded)
                        e.Node.Collapse();
                    else
                        e.Node.Expand();
                    SelectedNode = e.Node;
                }
            }
            base.OnNodeMouseClick(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateConnectionLayerBounds();
            InvalidateConnectionLayer();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_LBUTTONDBLCLK = 0x0203;
            if (m.Msg == WM_LBUTTONDBLCLK)
                lastDoubleClickTick = Environment.TickCount;
            base.WndProc(ref m);
            if (m.Msg == WmHScroll || m.Msg == WmVScroll)
                InvalidateConnectionLayer();
        }

        /// <summary>
        /// 供重绘周期和无硬件 UI 回归共同调用的确定性连线渲染入口。
        /// </summary>
        internal void RenderConnections(Graphics graphics)
        {
            RenderConnections(graphics, 0, ClientRectangle);
        }

        private void RenderConnections(Graphics graphics, int horizontalOffset, Rectangle clipBounds)
        {
            LastRenderedConnectionCount = 0;
            if (graphics == null || connectionProvider == null || Configuration.SpeedMode)
                return;

            bool showAllConnections = ResolveShowAllConnections();
            TreeNode selectedConnectionNode = showAllConnections ? null : SelectedNode;
            if (!showAllConnections)
            {
                // 模块标题只负责选择/展开，不触发整组连线。必须选中一个当前可见的
                // 输入或输出端口，才能查看这一个端口的来源或去向。
                if (selectedConnectionNode == null || selectedConnectionNode.Level == 0 ||
                    selectedConnectionNode.Parent == null || !selectedConnectionNode.Parent.IsExpanded ||
                    (!IsInputNode(selectedConnectionNode) && !IsOutputNode(selectedConnectionNode)))
                    return;
            }

            KeyValuePair<TreeNode, TreeNode>[] snapshot;
            try
            {
                snapshot = connectionProvider() ?? new KeyValuePair<TreeNode, TreeNode>[0];
            }
            catch
            {
                return;
            }

            List<ConnectionVisual> connections = new List<ConnectionVisual>();
            Dictionary<TreeNode, HashSet<TreeNode>> visibleRelations =
                new Dictionary<TreeNode, HashSet<TreeNode>>();
            foreach (KeyValuePair<TreeNode, TreeNode> pair in snapshot)
            {
                if (pair.Key == null || pair.Value == null ||
                    pair.Key.TreeView != this || pair.Value.TreeView != this)
                    continue;

                TreeNode inputToolNode = GetToolNode(pair.Key);
                TreeNode outputToolNode = GetToolNode(pair.Value);
                if (!showAllConnections &&
                    pair.Key != selectedConnectionNode && pair.Value != selectedConnectionNode)
                    continue;

                if (inputToolNode == null || outputToolNode == null)
                    continue;

                // 两端分别按自己的展开状态决定连接位置：展开的一端连到
                // 具体输入/输出端口，折叠的一端才收口到模块。
                TreeNode inputNode = ResolveVisibleConnectionEndpoint(pair.Key);
                TreeNode outputNode = ResolveVisibleConnectionEndpoint(pair.Value);
                if (inputNode == null || outputNode == null || inputNode == outputNode)
                    continue;

                // 仅对当前画面上实际可见的端点对去重。因此两端都折叠时
                // 同一模块对只有一条线；只展开一端时，该端的不同端口仍分别显示。
                HashSet<TreeNode> visibleTargets;
                if (!visibleRelations.TryGetValue(outputNode, out visibleTargets))
                {
                    visibleTargets = new HashSet<TreeNode>();
                    visibleRelations.Add(outputNode, visibleTargets);
                }
                if (!visibleTargets.Add(inputNode))
                    continue;

                Rectangle inputBounds = GetNodeVisualBounds(inputNode, inputNode.Bounds, GetContentRight());
                Rectangle outputBounds = GetNodeVisualBounds(outputNode, outputNode.Bounds, GetContentRight());
                if (!IsOnScreen(inputBounds) || !IsOnScreen(outputBounds))
                    continue;

                connections.Add(new ConnectionVisual(inputNode, outputNode, inputBounds, outputBounds));
            }

            connections = connections
                .OrderBy(item => GetStableNodeOrder(item.OutputNode))
                .ThenBy(item => GetStableNodeOrder(item.InputNode))
                .ToList();

            int contentRight = GetContentRight() - horizontalOffset;
            int endpointX = horizontalOffset == 0 ? contentRight - 1 : Math.Max(3, contentRight);
            int firstLaneX = Math.Max(endpointX + 16, contentRight + 16);
            int lastLaneX = Math.Max(firstLaneX, ClientSize.Width - horizontalOffset - 16);
            int laneCount = Math.Max(1, ((lastLaneX - firstLaneX) / 8) + 1);

            Region oldClip = graphics.Clip == null ? null : graphics.Clip.Clone();
            graphics.SetClip(clipBounds);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            for (int index = 0; index < connections.Count; index++)
            {
                ConnectionVisual connection = connections[index];
                Point start = new Point(endpointX,
                    connection.OutputBounds.Top + connection.OutputBounds.Height / 2);
                Point end = new Point(endpointX,
                    connection.InputBounds.Top + connection.InputBounds.Height / 2);
                int laneX = Math.Min(lastLaneX, firstLaneX + (index % laneCount) * 8);
                Color lineColor = ResolveConnectionColor(connection.OutputNode);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLine(start.X, start.Y, laneX, start.Y);
                    path.AddLine(laneX, start.Y, laneX, end.Y);
                    path.AddLine(laneX, end.Y, end.X, end.Y);

                    using (Pen halo = new Pen(Color.FromArgb(225, ModernUiTheme.Surface), 5F))
                    {
                        halo.LineJoin = LineJoin.Round;
                        halo.StartCap = LineCap.Round;
                        halo.EndCap = LineCap.Round;
                        graphics.DrawPath(halo, path);
                    }

                    using (Pen pen = new Pen(lineColor, 2F))
                    using (AdjustableArrowCap arrow = new AdjustableArrowCap(3.2F, 4.6F, true))
                    {
                        pen.LineJoin = LineJoin.Round;
                        pen.StartCap = LineCap.Round;
                        pen.CustomEndCap = arrow;
                        graphics.DrawPath(pen, path);
                    }
                }

                LastRenderedConnectionCount++;
            }

            if (oldClip != null)
            {
                graphics.Clip = oldClip;
                oldClip.Dispose();
            }
            else
            {
                graphics.ResetClip();
            }
        }

        private void UpdateConnectionLayerBounds()
        {
            if (connectionLayer == null || connectionLayer.IsDisposed)
                return;

            int width = Math.Min(ClientSize.Width, ConnectionGutterWidth);
            connectionLayer.SetBounds(Math.Max(0, ClientSize.Width - width), 0,
                Math.Max(0, width), Math.Max(0, ClientSize.Height));
            connectionLayer.BringToFront();
        }

        private void InvalidateConnectionLayer()
        {
            if (connectionLayer == null || connectionLayer.IsDisposed)
                return;
            connectionLayer.Invalidate();
        }

        private bool ResolveShowAllConnections()
        {
            if (connectionVisibilityProvider == null)
                return true;

            try
            {
                return connectionVisibilityProvider();
            }
            catch
            {
                // 配置读取与窗口关闭重叠时退回聚焦模式，不让绘制异常影响编辑器。
                return false;
            }
        }

        private sealed class ConnectionLayer : Control
        {
            private const int WmNcHitTest = 0x0084;
            private static readonly IntPtr HtTransparent = new IntPtr(-1);
            private readonly FlowEditorTreeView owner;

            internal ConnectionLayer(FlowEditorTreeView owner)
            {
                this.owner = owner;
                Name = "flowConnectionLayer";
                BackColor = ModernUiTheme.Surface;
                TabStop = false;
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.Clear(BackColor);
                try
                {
                    owner.RenderConnections(e.Graphics, Left, ClientRectangle);
                }
                catch
                {
                    owner.LastRenderedConnectionCount = 0;
                }
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WmNcHitTest)
                {
                    m.Result = HtTransparent;
                    return;
                }
                base.WndProc(ref m);
            }
        }

        private void DrawToolNode(Graphics graphics, TreeNode node, Rectangle bounds, bool selected)
        {
            Color fill = selected ? ModernUiTheme.Selection : ModernUiTheme.SurfaceRaised;
            Color border = selected ? Color.FromArgb(162, 204, 239) : ModernUiTheme.Border;
            Color categoryColor = ResolveCategoryColor(node.ImageIndex);

            using (GraphicsPath card = RoundedRectangle(bounds, 7))
            using (SolidBrush fillBrush = new SolidBrush(fill))
            using (Pen borderPen = new Pen(border, selected ? 1.5F : 1F))
            {
                graphics.FillPath(fillBrush, card);
                graphics.DrawPath(borderPen, card);
            }

            Rectangle accentBar = new Rectangle(bounds.Left, bounds.Top + 5, 3, bounds.Height - 10);
            using (SolidBrush accentBrush = new SolidBrush(categoryColor))
                graphics.FillRectangle(accentBrush, accentBar);

            DrawChevron(graphics, node, new Rectangle(bounds.Left + 8,
                bounds.Top + (bounds.Height - 14) / 2, 14, 14), categoryColor);

            Rectangle iconBounds = new Rectangle(bounds.Left + 28, bounds.Top + (bounds.Height - 22) / 2, 22, 22);
            DrawToolGlyph(graphics, node.ImageIndex, iconBounds, categoryColor);

            Rectangle statusBounds = new Rectangle(bounds.Right - 17, bounds.Top + (bounds.Height - 8) / 2, 8, 8);
            Color statusColor = ResolveStatusColor(node.ForeColor);
            using (SolidBrush statusBrush = new SolidBrush(statusColor))
                graphics.FillEllipse(statusBrush, statusBounds);

            if (!node.IsEditing)
            {
                Rectangle textBounds = new Rectangle(iconBounds.Right + 9, bounds.Top,
                    Math.Max(4, statusBounds.Left - iconBounds.Right - 14), bounds.Height);
                Color textColor = node.ForeColor == Color.DarkGray
                    ? ModernUiTheme.SecondaryText
                    : (node.ForeColor == Color.Red ? ModernUiTheme.Danger : ModernUiTheme.PrimaryText);
                TextRenderer.DrawText(graphics, node.Text, ModernUiTheme.UiFontBold, textBounds, textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        private void DrawPortNode(Graphics graphics, TreeNode node, Rectangle bounds, bool selected)
        {
            bool input = IsInputNode(node);
            bool output = IsOutputNode(node);
            Color directionColor = input ? InputColor : (output ? OutputColor : ModernUiTheme.SecondaryText);
            Color fill = selected ? ModernUiTheme.AccentSoft : Color.FromArgb(250, 251, 251);

            using (SolidBrush fillBrush = new SolidBrush(fill))
                graphics.FillRectangle(fillBrush, bounds);
            if (selected)
            {
                using (Pen selectedPen = new Pen(Color.FromArgb(190, 218, 240)))
                    graphics.DrawRectangle(selectedPen, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);
            }

            Rectangle glyphBounds = new Rectangle(bounds.Left + 8, bounds.Top + (bounds.Height - 15) / 2, 15, 15);
            DrawPortGlyph(graphics, glyphBounds, directionColor, input);

            if (!node.IsEditing)
            {
                Rectangle textBounds = new Rectangle(glyphBounds.Right + 7, bounds.Top,
                    Math.Max(4, bounds.Width - 44), bounds.Height);
                TextRenderer.DrawText(graphics, GetPortDisplayText(node.Text), ModernUiTheme.UiFont,
                    textBounds, ModernUiTheme.PrimaryText,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            Point port = new Point(ConnectionPortCenterX, bounds.Top + bounds.Height / 2);
            using (Pen stubPen = new Pen(directionColor, 1.5F))
            {
                stubPen.StartCap = LineCap.Round;
                stubPen.EndCap = LineCap.Round;
                graphics.DrawLine(stubPen, port.X + 4, port.Y, GetContentRight() - 1, port.Y);
            }
            using (SolidBrush portBrush = new SolidBrush(directionColor))
            using (Pen portBorder = new Pen(Color.White, 1F))
            {
                graphics.FillEllipse(portBrush, port.X - 4, port.Y - 4, 8, 8);
                graphics.DrawEllipse(portBorder, port.X - 3, port.Y - 3, 6, 6);
            }
        }

        private Rectangle GetNodeVisualBounds(TreeNode node, Rectangle nativeBounds, int contentRight)
        {
            int left = node != null && node.Level == 0
                ? 8
                : Math.Max(31, nativeBounds.Left - 18);
            int top = nativeBounds.Top + 2;
            int height = Math.Max(4, nativeBounds.Height - 4);
            return new Rectangle(left, top, Math.Max(4, contentRight - left - 4), height);
        }

        private int GetContentRight()
        {
            return Math.Max(104, ClientSize.Width - ConnectionGutterWidth);
        }

        private bool IsOnScreen(Rectangle bounds)
        {
            return bounds.Height > 0 && bounds.Bottom >= 0 && bounds.Top <= ClientSize.Height;
        }

        private static TreeNode GetToolNode(TreeNode node)
        {
            TreeNode current = node;
            while (current != null && current.Parent != null)
                current = current.Parent;
            return current;
        }

        /// <summary>
        /// 展开模块保留具体端口；折叠模块收口到模块标题。
        /// 输入端和输出端必须各自判断，不能由另一端的状态一起降级。
        /// </summary>
        private static TreeNode ResolveVisibleConnectionEndpoint(TreeNode endpointNode)
        {
            TreeNode toolNode = GetToolNode(endpointNode);
            if (toolNode == null)
                return null;

            return toolNode.IsExpanded ? endpointNode : toolNode;
        }

        private static int GetStableNodeOrder(TreeNode node)
        {
            if (node == null)
                return Int32.MaxValue;
            int parentIndex = node.Parent == null ? node.Index : node.Parent.Index;
            return parentIndex * 4096 + (node.Parent == null ? 0 : node.Index + 1);
        }

        private static bool IsInputNode(TreeNode node)
        {
            return node != null && node.Text != null && node.Text.StartsWith("<--", StringComparison.Ordinal);
        }

        private static bool IsOutputNode(TreeNode node)
        {
            return node != null && node.Text != null && node.Text.StartsWith("-->", StringComparison.Ordinal);
        }

        private static string GetPortDisplayText(string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            string text = value;
            if (text.StartsWith("<--", StringComparison.Ordinal) || text.StartsWith("-->", StringComparison.Ordinal))
                text = text.Substring(3);

            int sourceIndex = text.IndexOf("《- ", StringComparison.Ordinal);
            if (sourceIndex >= 0)
            {
                string source = text.Substring(sourceIndex + 3).Replace("->", " / ");
                text = text.Substring(0, sourceIndex) + "  ←  " + source;
            }
            return text;
        }

        private static Color ResolveStatusColor(Color value)
        {
            if (value == Color.Red)
                return ModernUiTheme.Danger;
            if (value == Color.Green)
                return SuccessColor;
            if (value == Color.Goldenrod || value == Color.Orange)
                return WarningColor;
            if (value == Color.DarkGray || value == Color.Gray)
                return DisabledColor;
            return Color.FromArgb(184, 197, 207);
        }

        private static Color ResolveConnectionColor(TreeNode sourceNode)
        {
            int value = GetStableNodeOrder(sourceNode);
            if (value == Int32.MaxValue)
                value = 0;
            return ConnectionPalette[Math.Abs(value) % ConnectionPalette.Length];
        }

        private static Color ResolveCategoryColor(int imageIndex)
        {
            switch (ResolveCategory(imageIndex))
            {
                case ToolGlyphCategory.Image:
                    return Color.FromArgb(47, 139, 201);
                case ToolGlyphCategory.Detection:
                    return Color.FromArgb(39, 151, 137);
                case ToolGlyphCategory.Geometry:
                    return Color.FromArgb(128, 103, 188);
                case ToolGlyphCategory.Logic:
                    return Color.FromArgb(213, 139, 45);
                case ToolGlyphCategory.Device:
                    return Color.FromArgb(78, 125, 158);
                case ToolGlyphCategory.Output:
                    return Color.FromArgb(55, 151, 104);
                default:
                    return ModernUiTheme.Accent;
            }
        }

        private static void DrawToolGlyph(Graphics graphics, int imageIndex, Rectangle bounds, Color color)
        {
            using (SolidBrush softBrush = new SolidBrush(Color.FromArgb(28, color)))
            using (Pen pen = new Pen(color, 1.55F))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                graphics.FillEllipse(softBrush, bounds);

                float x = bounds.Left;
                float y = bounds.Top;
                ToolGlyphCategory category = ResolveCategory(imageIndex);
                if (category == ToolGlyphCategory.Image)
                {
                    graphics.DrawRectangle(pen, x + 5, y + 6, 12, 10);
                    graphics.DrawEllipse(pen, x + 12, y + 8, 2.5F, 2.5F);
                    graphics.DrawLines(pen, new[]
                    {
                        new PointF(x + 6.5F, y + 14.5F), new PointF(x + 10F, y + 11F),
                        new PointF(x + 13F, y + 14F), new PointF(x + 16F, y + 12F)
                    });
                }
                else if (category == ToolGlyphCategory.Detection)
                {
                    graphics.DrawEllipse(pen, x + 6, y + 6, 10, 10);
                    graphics.DrawLine(pen, x + 11, y + 3.5F, x + 11, y + 8);
                    graphics.DrawLine(pen, x + 11, y + 14, x + 11, y + 18.5F);
                    graphics.DrawLine(pen, x + 3.5F, y + 11, x + 8, y + 11);
                    graphics.DrawLine(pen, x + 14, y + 11, x + 18.5F, y + 11);
                }
                else if (category == ToolGlyphCategory.Geometry)
                {
                    graphics.DrawEllipse(pen, x + 5, y + 7, 8, 8);
                    graphics.DrawPolygon(pen, new[]
                    {
                        new PointF(x + 14F, y + 5F), new PointF(x + 18F, y + 16F),
                        new PointF(x + 10F, y + 16F)
                    });
                }
                else if (category == ToolGlyphCategory.Logic)
                {
                    graphics.DrawLine(pen, x + 7, y + 6, x + 7, y + 16);
                    graphics.DrawLine(pen, x + 7, y + 8, x + 15, y + 8);
                    graphics.DrawLine(pen, x + 7, y + 15, x + 15, y + 15);
                    graphics.DrawEllipse(pen, x + 4.5F, y + 3.5F, 5, 5);
                    graphics.DrawEllipse(pen, x + 13, y + 5.5F, 5, 5);
                    graphics.DrawEllipse(pen, x + 13, y + 12.5F, 5, 5);
                }
                else if (category == ToolGlyphCategory.Device)
                {
                    graphics.DrawRectangle(pen, x + 6, y + 6, 10, 10);
                    for (int offset = 7; offset <= 15; offset += 4)
                    {
                        graphics.DrawLine(pen, x + offset, y + 3.5F, x + offset, y + 6);
                        graphics.DrawLine(pen, x + offset, y + 16, x + offset, y + 18.5F);
                    }
                }
                else if (category == ToolGlyphCategory.Output)
                {
                    graphics.DrawLine(pen, x + 5, y + 16, x + 17, y + 16);
                    graphics.DrawLine(pen, x + 11, y + 5, x + 11, y + 13);
                    graphics.DrawLines(pen, new[]
                    {
                        new PointF(x + 7.5F, y + 10F), new PointF(x + 11F, y + 13.5F),
                        new PointF(x + 14.5F, y + 10F)
                    });
                }
                else
                {
                    graphics.DrawRectangle(pen, x + 5, y + 5, 5, 5);
                    graphics.DrawRectangle(pen, x + 12, y + 12, 5, 5);
                    graphics.DrawLine(pen, x + 9.5F, y + 9.5F, x + 12.5F, y + 12.5F);
                }
            }
        }

        private static ToolGlyphCategory ResolveCategory(int imageIndex)
        {
            switch (imageIndex)
            {
                case 1:
                case 2:
                case 11:
                case 31:
                case 32:
                case 39:
                case 40:
                case 55:
                    return ToolGlyphCategory.Image;
                case 6:
                case 7:
                case 10:
                case 23:
                case 24:
                case 25:
                case 33:
                case 48:
                case 49:
                case 50:
                    return ToolGlyphCategory.Detection;
                case 4:
                case 5:
                case 8:
                case 9:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 38:
                case 41:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                    return ToolGlyphCategory.Geometry;
                case 26:
                case 28:
                case 42:
                case 51:
                case 52:
                case 53:
                case 58:
                    return ToolGlyphCategory.Logic;
                case 27:
                case 37:
                case 54:
                case 56:
                case 57:
                    return ToolGlyphCategory.Device;
                case 29:
                case 30:
                case 59:
                case 60:
                    return ToolGlyphCategory.Output;
                default:
                    return ToolGlyphCategory.Generic;
            }
        }

        private static void DrawPortGlyph(Graphics graphics, Rectangle bounds, Color color, bool input)
        {
            using (Pen pen = new Pen(color, 1.55F))
            using (SolidBrush brush = new SolidBrush(color))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                float middleY = bounds.Top + bounds.Height / 2F;
                float startX = input ? bounds.Right - 2 : bounds.Left + 2;
                float endX = input ? bounds.Left + 3 : bounds.Right - 3;
                graphics.DrawLine(pen, startX, middleY, endX, middleY);
                PointF[] arrow = input
                    ? new[] { new PointF(endX, middleY), new PointF(endX + 4, middleY - 3.5F), new PointF(endX + 4, middleY + 3.5F) }
                    : new[] { new PointF(endX, middleY), new PointF(endX - 4, middleY - 3.5F), new PointF(endX - 4, middleY + 3.5F) };
                graphics.FillPolygon(brush, arrow);
            }
        }

        private static void DrawChevron(Graphics graphics, TreeNode node, Rectangle bounds, Color color)
        {
            if (node == null || node.Nodes.Count == 0)
                return;

            using (Pen pen = new Pen(color, 1.6F))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                float middleX = bounds.Left + bounds.Width / 2F;
                float middleY = bounds.Top + bounds.Height / 2F;
                if (node.IsExpanded)
                {
                    graphics.DrawLines(pen, new[]
                    {
                        new PointF(middleX - 3.5F, middleY - 1.5F),
                        new PointF(middleX, middleY + 2F),
                        new PointF(middleX + 3.5F, middleY - 1.5F)
                    });
                }
                else
                {
                    graphics.DrawLines(pen, new[]
                    {
                        new PointF(middleX - 1.5F, middleY - 3.5F),
                        new PointF(middleX + 2F, middleY),
                        new PointF(middleX - 1.5F, middleY + 3.5F)
                    });
                }
            }
        }

        private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Max(2, radius * 2);
            Rectangle arc = new Rectangle(bounds.Left, bounds.Top, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr handle, int message, IntPtr wParam, IntPtr lParam);

        private sealed class ConnectionVisual
        {
            internal readonly TreeNode InputNode;
            internal readonly TreeNode OutputNode;
            internal readonly Rectangle InputBounds;
            internal readonly Rectangle OutputBounds;

            internal ConnectionVisual(
                TreeNode inputNode,
                TreeNode outputNode,
                Rectangle inputBounds,
                Rectangle outputBounds)
            {
                InputNode = inputNode;
                OutputNode = outputNode;
                InputBounds = inputBounds;
                OutputBounds = outputBounds;
            }
        }

        private enum ToolGlyphCategory
        {
            Generic,
            Image,
            Detection,
            Geometry,
            Logic,
            Device,
            Output
        }
    }
}
