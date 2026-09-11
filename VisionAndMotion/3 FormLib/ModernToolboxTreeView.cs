using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VMPro
{
    /// <summary>
    /// 工具箱专用树：分类、工具和添加入口均由同一绘制周期完成，避免继续依赖
    /// TreeView 的旧式文件夹外观，同时不改变 TreeNode 及拖拽业务语义。
    /// </summary>
    internal sealed class ModernToolboxTreeView : TreeView
    {
        private const int TvmSetExtendedStyle = 0x1100 + 44;
        private const int TvsExDoubleBuffer = 0x0004;

        private static readonly Color[] CategoryColors =
        {
            Color.FromArgb(76, 148, 210),
            Color.FromArgb(45, 166, 154),
            Color.FromArgb(125, 108, 190),
            Color.FromArgb(221, 145, 56),
            Color.FromArgb(72, 132, 190),
            Color.FromArgb(83, 150, 113),
            Color.FromArgb(194, 104, 116)
        };

        private TreeNode hotNode;

        internal ModernToolboxTreeView()
        {
            BorderStyle = BorderStyle.None;
            DrawMode = TreeViewDrawMode.OwnerDrawAll;
            FullRowSelect = true;
            HideSelection = false;
            HotTracking = false;
            Indent = 20;
            ItemHeight = 38;
            ShowLines = false;
            ShowPlusMinus = false;
            ShowRootLines = false;
            BackColor = ModernUiTheme.Surface;
            ForeColor = ModernUiTheme.PrimaryText;
            Font = ModernUiTheme.UiFont;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
        }

        internal event EventHandler<ToolboxAddRequestedEventArgs> AddRequested;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                SendMessage(Handle, TvmSetExtendedStyle,
                    new IntPtr(TvsExDoubleBuffer), new IntPtr(TvsExDoubleBuffer));
            }
            catch
            {
                // 双缓冲只是视觉增强，失败时保留 TreeView 的正常行为。
            }
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (e.Node == null)
                return;

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            using (SolidBrush background = new SolidBrush(ModernUiTheme.Surface))
                graphics.FillRectangle(background, new Rectangle(0, e.Bounds.Top, ClientSize.Width, e.Bounds.Height));

            if (e.Node.Level == 0)
                DrawCategory(graphics, e.Node, e.Bounds);
            else
                DrawTool(graphics, e.Node, e.Bounds);
        }

        private void DrawCategory(Graphics graphics, TreeNode node, Rectangle rowBounds)
        {
            Rectangle card = new Rectangle(8, rowBounds.Top + 3,
                Math.Max(20, ClientSize.Width - 16), Math.Max(24, rowBounds.Height - 6));
            bool selected = node == SelectedNode;
            Color accent = GetCategoryColor(node.Index);
            Color fill = selected ? ModernUiTheme.Selection : Color.FromArgb(244, 248, 251);

            using (GraphicsPath path = CreateRoundedPath(card, 7))
            using (SolidBrush fillBrush = new SolidBrush(fill))
            using (Pen borderPen = new Pen(selected ? Color.FromArgb(166, 207, 239) : ModernUiTheme.Border))
            {
                graphics.FillPath(fillBrush, path);
                graphics.DrawPath(borderPen, path);
            }

            using (SolidBrush accentBrush = new SolidBrush(accent))
            {
                Rectangle accentBar = new Rectangle(card.Left, card.Top + 5, 3, card.Height - 10);
                graphics.FillRectangle(accentBrush, accentBar);
            }

            Rectangle chevron = GetCategoryChevronBounds(rowBounds);
            using (Pen chevronPen = new Pen(accent, 1.6F))
            {
                chevronPen.StartCap = LineCap.Round;
                chevronPen.EndCap = LineCap.Round;
                chevronPen.LineJoin = LineJoin.Round;
                if (node.IsExpanded)
                {
                    graphics.DrawLines(chevronPen, new[]
                    {
                        new Point(chevron.Left, chevron.Top + 2),
                        new Point(chevron.Left + 4, chevron.Top + 6),
                        new Point(chevron.Left + 8, chevron.Top + 2)
                    });
                }
                else
                {
                    graphics.DrawLines(chevronPen, new[]
                    {
                        new Point(chevron.Left + 2, chevron.Top),
                        new Point(chevron.Left + 6, chevron.Top + 4),
                        new Point(chevron.Left + 2, chevron.Top + 8)
                    });
                }
            }

            Rectangle categoryGlyph = new Rectangle(card.Left + 29, card.Top + 8, 16, 16);
            using (SolidBrush softBrush = new SolidBrush(Color.FromArgb(35, accent)))
            using (Pen glyphPen = new Pen(accent, 1.4F))
            {
                graphics.FillEllipse(softBrush, categoryGlyph);
                graphics.DrawEllipse(glyphPen, categoryGlyph);
                using (SolidBrush dotBrush = new SolidBrush(accent))
                {
                    graphics.FillEllipse(dotBrush, categoryGlyph.Left + 4, categoryGlyph.Top + 4, 2.5F, 2.5F);
                    graphics.FillEllipse(dotBrush, categoryGlyph.Right - 6.5F, categoryGlyph.Top + 4, 2.5F, 2.5F);
                    graphics.FillEllipse(dotBrush, categoryGlyph.Left + 4, categoryGlyph.Bottom - 6.5F, 2.5F, 2.5F);
                    graphics.FillEllipse(dotBrush, categoryGlyph.Right - 6.5F, categoryGlyph.Bottom - 6.5F, 2.5F, 2.5F);
                }
            }

            string badgeText = node.Nodes.Count.ToString();
            Size badgeSize = TextRenderer.MeasureText(badgeText, ModernUiTheme.UiFont,
                Size.Empty, TextFormatFlags.NoPadding);
            Rectangle badge = new Rectangle(card.Right - Math.Max(27, badgeSize.Width + 13),
                card.Top + 7, Math.Max(22, badgeSize.Width + 9), card.Height - 14);
            using (GraphicsPath badgePath = CreateRoundedPath(badge, badge.Height / 2))
            using (SolidBrush badgeBrush = new SolidBrush(Color.FromArgb(232, 239, 244)))
                graphics.FillPath(badgeBrush, badgePath);

            TextRenderer.DrawText(graphics, badgeText, ModernUiTheme.UiFont, badge,
                ModernUiTheme.SecondaryText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            Rectangle textBounds = new Rectangle(categoryGlyph.Right + 9, card.Top,
                Math.Max(10, badge.Left - categoryGlyph.Right - 14), card.Height);
            TextRenderer.DrawText(graphics, node.Text, ModernUiTheme.UiFontBold, textBounds,
                ModernUiTheme.PrimaryText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private void DrawTool(Graphics graphics, TreeNode node, Rectangle rowBounds)
        {
            bool selected = node == SelectedNode;
            bool hot = node == hotNode;
            Rectangle item = new Rectangle(28, rowBounds.Top + 2,
                Math.Max(20, ClientSize.Width - 36), Math.Max(26, rowBounds.Height - 4));

            if (selected || hot)
            {
                using (GraphicsPath path = CreateRoundedPath(item, 7))
                using (SolidBrush brush = new SolidBrush(selected
                    ? ModernUiTheme.Selection
                    : Color.FromArgb(247, 250, 252)))
                {
                    graphics.FillPath(brush, path);
                }
            }

            Rectangle iconBounds = new Rectangle(item.Left + 9, item.Top + (item.Height - 20) / 2, 20, 20);
            DrawToolImage(graphics, node, iconBounds);

            int rightReserve = selected || hot ? 38 : 10;
            Rectangle textBounds = new Rectangle(iconBounds.Right + 9, item.Top,
                Math.Max(10, item.Right - iconBounds.Right - rightReserve), item.Height);
            TextRenderer.DrawText(graphics, node.Text, ModernUiTheme.UiFont, textBounds,
                ModernUiTheme.PrimaryText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            if (selected || hot)
                DrawAddButton(graphics, GetAddButtonBounds(rowBounds), selected);
        }

        private void DrawToolImage(Graphics graphics, TreeNode node, Rectangle bounds)
        {
            Image image = null;
            if (ImageList != null && node.ImageIndex >= 0 && node.ImageIndex < ImageList.Images.Count)
                image = ImageList.Images[node.ImageIndex];

            if (image != null)
            {
                InterpolationMode oldMode = graphics.InterpolationMode;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, bounds);
                graphics.InterpolationMode = oldMode;
                return;
            }

            using (SolidBrush brush = new SolidBrush(ModernUiTheme.AccentSoft))
            using (Pen pen = new Pen(ModernUiTheme.Accent, 1.3F))
            {
                graphics.FillEllipse(brush, bounds);
                graphics.DrawEllipse(pen, bounds);
                graphics.DrawLine(pen, bounds.Left + 6, bounds.Top + 10, bounds.Right - 6, bounds.Top + 10);
            }
        }

        private static void DrawAddButton(Graphics graphics, Rectangle bounds, bool selected)
        {
            Color fill = selected ? Color.FromArgb(255, 255, 255) : ModernUiTheme.AccentSoft;
            using (SolidBrush brush = new SolidBrush(fill))
            using (Pen pen = new Pen(ModernUiTheme.Accent, 1.35F))
            {
                graphics.FillEllipse(brush, bounds);
                graphics.DrawEllipse(pen, bounds);
                int centerX = bounds.Left + bounds.Width / 2;
                int centerY = bounds.Top + bounds.Height / 2;
                graphics.DrawLine(pen, centerX - 4, centerY, centerX + 4, centerY);
                graphics.DrawLine(pen, centerX, centerY - 4, centerX, centerY + 4);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            TreeNode node = GetNodeAt(e.Location);
            if (node != hotNode)
            {
                TreeNode previous = hotNode;
                hotNode = node;
                InvalidateNode(previous);
                InvalidateNode(hotNode);
            }

            bool actionable = node != null &&
                ((node.Level == 0 && GetCategoryChevronBounds(node.Bounds).Contains(e.Location)) ||
                 (node.Level > 0 && GetAddButtonBounds(node.Bounds).Contains(e.Location)));
            Cursor = actionable ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            TreeNode previous = hotNode;
            hotNode = null;
            Cursor = Cursors.Default;
            InvalidateNode(previous);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            TreeNode node = GetNodeAt(e.Location);
            if (e.Button == MouseButtons.Left && node != null)
            {
                if (node.Level == 0 && GetCategoryChevronBounds(node.Bounds).Contains(e.Location))
                {
                    SelectedNode = node;
                    if (node.IsExpanded)
                        node.Collapse();
                    else
                        node.Expand();
                    Invalidate();
                    return;
                }

                if (node.Level > 0 && GetAddButtonBounds(node.Bounds).Contains(e.Location))
                {
                    SelectedNode = node;
                    Focus();
                    EventHandler<ToolboxAddRequestedEventArgs> handler = AddRequested;
                    if (handler != null)
                        handler(this, new ToolboxAddRequestedEventArgs(node));
                    return;
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            Invalidate();
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);
            Invalidate();
        }

        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            base.OnAfterCollapse(e);
            Invalidate();
        }

        private void InvalidateNode(TreeNode node)
        {
            if (node == null || node.TreeView != this)
                return;
            Rectangle bounds = node.Bounds;
            bounds.X = 0;
            bounds.Width = ClientSize.Width;
            Invalidate(bounds);
        }

        private Rectangle GetCategoryChevronBounds(Rectangle rowBounds)
        {
            return new Rectangle(19, rowBounds.Top + Math.Max(0, (rowBounds.Height - 10) / 2), 12, 10);
        }

        private Rectangle GetAddButtonBounds(Rectangle rowBounds)
        {
            return new Rectangle(Math.Max(30, ClientSize.Width - 38),
                rowBounds.Top + Math.Max(2, (rowBounds.Height - 22) / 2), 22, 22);
        }

        private static Color GetCategoryColor(int index)
        {
            return CategoryColors[Math.Abs(index) % CategoryColors.Length];
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Min(Math.Min(radius * 2, bounds.Width), bounds.Height);
            if (diameter <= 2)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

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
    }

    internal sealed class ToolboxAddRequestedEventArgs : EventArgs
    {
        internal ToolboxAddRequestedEventArgs(TreeNode node)
        {
            Node = node;
        }

        internal TreeNode Node { get; private set; }
    }

    internal sealed class ToolboxCategoryExpandedEventArgs : EventArgs
    {
        internal ToolboxCategoryExpandedEventArgs(TreeNode category, bool expanded)
        {
            Category = category;
            Expanded = expanded;
        }

        internal TreeNode Category { get; private set; }
        internal bool Expanded { get; private set; }
    }

    /// <summary>
    /// 工具箱的快捷方式视图。它有意不承载工具创建业务，所有项目仍对应原 TreeView
    /// 中的 TreeNode，因此搜索、权限校验和 AddTool 的既有分派保持不变。
    /// </summary>
    internal sealed class ModernToolboxGrid : Panel
    {
        private const int ColumnCount = 3;
        private const int HeaderHeight = 30;
        // Keep cards compact even if the toolbox itself is widened.  Their
        // dimensions are reduced together from the previous 95 x 87 layout,
        // so the shortcut keeps its visual proportion at the narrowest width.
        private const int CardWidth = 78;
        private const int CardHeight = 76;
        private const int CardGap = 4;
        private const int ContentLeft = 8;
        private readonly List<TreeNode> categories = new List<TreeNode>();
        private readonly List<GridItem> hitItems = new List<GridItem>();
        private TreeNode hotTool;
        private TreeNode dragCandidate;
        private Point dragStart;
        private bool dragStarted;

        internal ModernToolboxGrid()
        {
            AutoScroll = true;
            BackColor = ModernUiTheme.Surface;
            Cursor = Cursors.Default;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
        }

        internal ImageList ImageList { get; set; }
        internal event EventHandler<ToolboxAddRequestedEventArgs> ToolClicked;
        internal event EventHandler<ToolboxAddRequestedEventArgs> ToolDragStarted;
        internal event EventHandler<ToolboxCategoryExpandedEventArgs> CategoryExpandedChanged;

        internal void SetCategories(IEnumerable<TreeNode> source)
        {
            categories.Clear();
            if (source != null)
                categories.AddRange(source);
            RebuildLayout();
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RebuildLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (GridItem item in hitItems)
            {
                // TextRenderer does not apply Graphics.Transform, while DrawImage does.
                // Translate the whole card before drawing so icons and labels use the same
                // coordinate space after scrolling.
                GridItem visibleItem = new GridItem(item.Node, OffsetForScroll(item.Bounds), item.IsCategory);
                if (visibleItem.IsCategory)
                    DrawCategory(e.Graphics, visibleItem);
                else
                    DrawTool(e.Graphics, visibleItem);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dragCandidate != null && e.Button == MouseButtons.Left && HasMovedBeyondDragThreshold(e.Location))
            {
                TreeNode node = dragCandidate;
                dragCandidate = null;
                dragStarted = true;
                EventHandler<ToolboxAddRequestedEventArgs> dragHandler = ToolDragStarted;
                if (dragHandler != null)
                    dragHandler(this, new ToolboxAddRequestedEventArgs(node));
                return;
            }

            GridItem item = FindItem(e.Location);
            TreeNode newHot = item == null || item.IsCategory ? null : item.Node;
            if (newHot != hotTool)
            {
                hotTool = newHot;
                Invalidate();
            }
            Cursor = item == null ? Cursors.Default : Cursors.Hand;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hotTool = null;
            Cursor = Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            dragCandidate = null;
            dragStarted = false;
            if (e.Button == MouseButtons.Left)
            {
                GridItem item = FindItem(e.Location);
                if (item != null && !item.IsCategory)
                {
                    dragCandidate = item.Node;
                    dragStart = e.Location;
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            dragCandidate = null;
            base.OnMouseUp(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (dragStarted)
            {
                dragStarted = false;
                return;
            }
            if (e.Button != MouseButtons.Left)
                return;
            GridItem item = FindItem(e.Location);
            if (item == null)
                return;
            if (item.IsCategory)
            {
                EventHandler<ToolboxCategoryExpandedEventArgs> categoryHandler = CategoryExpandedChanged;
                if (categoryHandler != null)
                    categoryHandler(this, new ToolboxCategoryExpandedEventArgs(item.Node, !item.Node.IsExpanded));
                return;
            }
            EventHandler<ToolboxAddRequestedEventArgs> toolHandler = ToolClicked;
            if (toolHandler != null)
                toolHandler(this, new ToolboxAddRequestedEventArgs(item.Node));
        }

        private bool HasMovedBeyondDragThreshold(Point location)
        {
            Size dragSize = SystemInformation.DragSize;
            Rectangle threshold = new Rectangle(
                dragStart.X - dragSize.Width / 2,
                dragStart.Y - dragSize.Height / 2,
                dragSize.Width,
                dragSize.Height);
            return !threshold.Contains(location);
        }

        private void RebuildLayout()
        {
            hitItems.Clear();
            int contentWidth = ColumnCount * CardWidth + (ColumnCount - 1) * CardGap;
            int y = 6;
            foreach (TreeNode category in categories)
            {
                // Category bars are exactly as wide as a row of three shortcut
                // cards instead of stretching across unused toolbox space.
                hitItems.Add(new GridItem(category, new Rectangle(ContentLeft, y, contentWidth, HeaderHeight), true));
                y += HeaderHeight + 4;
                if (category.IsExpanded)
                {
                    for (int index = 0; index < category.Nodes.Count; index++)
                    {
                        int column = index % ColumnCount;
                        int row = index / ColumnCount;
                        Rectangle card = new Rectangle(ContentLeft + column * (CardWidth + CardGap),
                            y + row * CardHeight, CardWidth, CardHeight - 5);
                        hitItems.Add(new GridItem(category.Nodes[index], card, false));
                    }
                    y += ((category.Nodes.Count + ColumnCount - 1) / ColumnCount) * CardHeight;
                }
                y += 4;
            }
            AutoScrollMinSize = new Size(contentWidth + ContentLeft * 2, Math.Max(0, y));
        }

        private GridItem FindItem(Point location)
        {
            Point documentPoint = new Point(location.X - AutoScrollPosition.X, location.Y - AutoScrollPosition.Y);
            return hitItems.FirstOrDefault(item => item.Bounds.Contains(documentPoint));
        }

        private Rectangle OffsetForScroll(Rectangle bounds)
        {
            return new Rectangle(
                bounds.X + AutoScrollPosition.X,
                bounds.Y + AutoScrollPosition.Y,
                bounds.Width,
                bounds.Height);
        }

        private static void DrawCategory(Graphics graphics, GridItem item)
        {
            Color accent = GetCategoryColor(item.Node.Index);
            using (SolidBrush fill = new SolidBrush(Color.FromArgb(244, 248, 251)))
            using (Pen border = new Pen(ModernUiTheme.Border))
            using (GraphicsPath path = CreateRoundedPath(item.Bounds, 7))
            {
                graphics.FillPath(fill, path);
                graphics.DrawPath(border, path);
            }
            using (SolidBrush bar = new SolidBrush(accent))
                graphics.FillRectangle(bar, item.Bounds.Left, item.Bounds.Top + 5, 3, item.Bounds.Height - 10);
            string chevron = item.Node.IsExpanded ? "⌄" : "›";
            TextRenderer.DrawText(graphics, chevron, ModernUiTheme.UiFontBold,
                new Rectangle(item.Bounds.Left + 13, item.Bounds.Top, 16, item.Bounds.Height), accent,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
            TextRenderer.DrawText(graphics, item.Node.Text, ModernUiTheme.UiFontBold,
                new Rectangle(item.Bounds.Left + 34, item.Bounds.Top, item.Bounds.Width - 78, item.Bounds.Height),
                ModernUiTheme.PrimaryText, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            TextRenderer.DrawText(graphics, item.Node.Nodes.Count.ToString(), ModernUiTheme.UiFont,
                new Rectangle(item.Bounds.Right - 32, item.Bounds.Top, 25, item.Bounds.Height),
                ModernUiTheme.SecondaryText, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
        }

        private void DrawTool(Graphics graphics, GridItem item)
        {
            bool hot = item.Node == hotTool;
            Rectangle card = item.Bounds;
            using (GraphicsPath path = CreateRoundedPath(card, 6))
            using (SolidBrush fill = new SolidBrush(hot ? ModernUiTheme.Selection : ModernUiTheme.Surface))
            using (Pen border = new Pen(hot ? Color.FromArgb(166, 207, 239) : Color.FromArgb(232, 238, 243)))
            {
                graphics.FillPath(fill, path);
                graphics.DrawPath(border, path);
            }
            Rectangle icon = new Rectangle(card.Left + (card.Width - 24) / 2, card.Top + 7, 24, 24);
            Image image = ImageList != null && item.Node.ImageIndex >= 0 && item.Node.ImageIndex < ImageList.Images.Count
                ? ImageList.Images[item.Node.ImageIndex] : null;
            if (image != null)
                graphics.DrawImage(image, icon);
            else
            {
                using (SolidBrush brush = new SolidBrush(ModernUiTheme.AccentSoft))
                using (Pen pen = new Pen(ModernUiTheme.Accent, 1.2F))
                {
                    graphics.FillEllipse(brush, icon);
                    graphics.DrawEllipse(pen, icon);
                }
            }
            TextRenderer.DrawText(graphics, item.Node.Text, ModernUiTheme.UiFont,
                new Rectangle(card.Left + 3, icon.Bottom + 3, card.Width - 6, card.Bottom - icon.Bottom - 6),
                ModernUiTheme.PrimaryText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis |
                TextFormatFlags.TextBoxControl | TextFormatFlags.NoPrefix);
        }

        private static Color GetCategoryColor(int index)
        {
            Color[] colors = { Color.FromArgb(76, 148, 210), Color.FromArgb(45, 166, 154), Color.FromArgb(125, 108, 190), Color.FromArgb(221, 145, 56), Color.FromArgb(72, 132, 190), Color.FromArgb(83, 150, 113), Color.FromArgb(194, 104, 116) };
            return colors[Math.Abs(index) % colors.Length];
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Min(Math.Min(radius * 2, bounds.Width), bounds.Height);
            Rectangle arc = new Rectangle(bounds.Left, bounds.Top, diameter, diameter);
            path.AddArc(arc, 180, 90); arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90); arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90); arc.X = bounds.Left;
            path.AddArc(arc, 90, 90); path.CloseFigure();
            return path;
        }

        private sealed class GridItem
        {
            internal GridItem(TreeNode node, Rectangle bounds, bool isCategory)
            { Node = node; Bounds = bounds; IsCategory = isCategory; }
            internal TreeNode Node;
            internal Rectangle Bounds;
            internal bool IsCategory;
        }
    }
}
