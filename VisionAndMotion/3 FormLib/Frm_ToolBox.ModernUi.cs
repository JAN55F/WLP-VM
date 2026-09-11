using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_ToolBox
    {
        // Three compact shortcut cards need 242 px; this leaves only the host
        // padding and a vertical scrollbar margin, so the outer toolbox is just
        // slightly wider than one full row of three cards.
        private const int ModernToolboxMinimumWidth = 286;
        private const int ModernToolboxMinimumHeight = 180;
        private TableLayoutPanel modernToolboxLayout;
        private ModernToolboxSearchBox modernToolSearch;
        private System.Windows.Forms.Label modernToolCount;
        private System.Windows.Forms.Label modernEmptyState;
        private ModernToolboxGrid modernToolGrid;
        private readonly List<TreeNode> modernToolCatalog = new List<TreeNode>();
        private readonly HashSet<string> modernExpandedToolCategories = new HashSet<string>(StringComparer.Ordinal);
        private int modernToolTotal;
        private bool rebuildingModernToolTree;

        private void InitializeModernToolboxUi()
        {
            bool english = Project.Instance.configuration.language == Language.English;

            SuspendLayout();
            try
            {
                tvw_tools.DrawMode = TreeViewDrawMode.OwnerDrawAll;
                tvw_tools.BorderStyle = BorderStyle.None;
                tvw_tools.FullRowSelect = true;
                tvw_tools.HideSelection = false;
                tvw_tools.ShowLines = false;
                tvw_tools.ShowPlusMinus = false;
                tvw_tools.ShowRootLines = false;
                tvw_tools.ItemHeight = 38;
                tvw_tools.BackColor = ModernUiTheme.Surface;
                tvw_tools.ForeColor = ModernUiTheme.PrimaryText;
                tvw_tools.Font = ModernUiTheme.UiFont;
                tvw_tools.AddRequested += ModernToolbox_AddRequested;
                tvw_tools.AfterSelect -= tvw_job_AfterSelect;

                modernToolboxLayout = new TableLayoutPanel();
                modernToolboxLayout.Name = "modernToolboxLayout";
                modernToolboxLayout.Dock = DockStyle.Fill;
                modernToolboxLayout.Margin = Padding.Empty;
                modernToolboxLayout.Padding = Padding.Empty;
                modernToolboxLayout.ColumnCount = 1;
                modernToolboxLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                modernToolboxLayout.RowCount = 2;
                modernToolboxLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
                modernToolboxLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                modernToolboxLayout.BackColor = ModernUiTheme.Page;

                Control header = CreateModernToolboxHeader(english);
                Control treeHost = CreateModernToolboxTreeHost(english);
                modernToolboxLayout.Controls.Add(header, 0, 0);
                modernToolboxLayout.Controls.Add(treeHost, 0, 1);

                Controls.Clear();
                Controls.Add(modernToolboxLayout);
                BackColor = ModernUiTheme.Page;
                Font = ModernUiTheme.UiFont;
                Text = english ? "Toolbox" : "工具箱";
                MinimumSize = new Size(ModernToolboxMinimumWidth, ModernToolboxMinimumHeight);
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        private Control CreateModernToolboxHeader(bool english)
        {
            TableLayoutPanel header = new TableLayoutPanel();
            header.Name = "modernToolboxHeader";
            header.Dock = DockStyle.Fill;
            header.Margin = Padding.Empty;
            header.Padding = new Padding(6, 4, 6, 2);
            header.ColumnCount = 1;
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            header.RowCount = 2;
            header.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            header.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            header.BackColor = ModernUiTheme.Surface;

            modernToolSearch = new ModernToolboxSearchBox(english);
            modernToolSearch.Name = "modernToolSearch";
            modernToolSearch.Dock = DockStyle.Fill;
            modernToolSearch.Margin = Padding.Empty;
            modernToolSearch.QueryChanged += ModernToolSearch_QueryChanged;
            modernToolSearch.SearchSubmitted += ModernToolSearch_SearchSubmitted;
            modernToolSearch.MoveToResultsRequested += ModernToolSearch_MoveToResultsRequested;
            header.Controls.Add(modernToolSearch, 0, 0);

            Panel commands = new Panel();
            commands.Name = "modernToolboxCommands";
            commands.Dock = DockStyle.Fill;
            commands.Margin = Padding.Empty;
            commands.BackColor = ModernUiTheme.Surface;

            modernToolCount = new System.Windows.Forms.Label();
            modernToolCount.Name = "modernToolCount";
            modernToolCount.Dock = DockStyle.Fill;
            modernToolCount.TextAlign = ContentAlignment.MiddleLeft;
            modernToolCount.ForeColor = ModernUiTheme.SecondaryText;
            modernToolCount.Font = ModernUiTheme.UiFont;
            modernToolCount.Text = english ? "All tools" : "全部工具";
            commands.Controls.Add(modernToolCount);

            Button collapse = CreateToolboxCommandButton(
                "modernCollapseAll", ModernVectorIconFactory.Glyph.Collapse,
                english ? "Collapse all" : "全部折叠");
            collapse.Dock = DockStyle.Right;
            collapse.Click += delegate { SetAllToolCategoriesExpanded(false); };
            commands.Controls.Add(collapse);

            Button expand = CreateToolboxCommandButton(
                "modernExpandAll", ModernVectorIconFactory.Glyph.Expand,
                english ? "Expand all" : "全部展开");
            expand.Dock = DockStyle.Right;
            expand.Click += delegate { SetAllToolCategoriesExpanded(true); };
            commands.Controls.Add(expand);

            header.Controls.Add(commands, 0, 1);
            return header;
        }

        private Control CreateModernToolboxTreeHost(bool english)
        {
            Panel host = new Panel();
            host.Name = "modernToolboxTreeHost";
            host.Dock = DockStyle.Fill;
            host.Margin = Padding.Empty;
            host.Padding = new Padding(4, 2, 4, 2);
            host.BackColor = ModernUiTheme.Surface;

            // TreeView remains in the visual tree as the interaction source.  The shortcut
            // grid sits above it, but forwards drag operations back to this control so its
            // established drag-and-drop contract remains intact.
            tvw_tools.Visible = false;
            tvw_tools.Dock = DockStyle.Fill;
            host.Controls.Add(tvw_tools);
            modernToolGrid = new ModernToolboxGrid();
            modernToolGrid.Name = "modernToolGrid";
            modernToolGrid.Dock = DockStyle.Fill;
            modernToolGrid.MinimumSize = new Size(262, 0);
            modernToolGrid.ImageList = tvw_tools.ImageList;
            modernToolGrid.ToolClicked += ModernToolGrid_ToolClicked;
            modernToolGrid.ToolDragStarted += ModernToolGrid_ToolDragStarted;
            modernToolGrid.CategoryExpandedChanged += ModernToolGrid_CategoryExpandedChanged;
            host.Controls.Add(modernToolGrid);

            modernEmptyState = new System.Windows.Forms.Label();
            modernEmptyState.Name = "modernToolboxEmptyState";
            modernEmptyState.Dock = DockStyle.Fill;
            modernEmptyState.BackColor = ModernUiTheme.Surface;
            modernEmptyState.ForeColor = ModernUiTheme.SecondaryText;
            modernEmptyState.Font = ModernUiTheme.UiFont;
            modernEmptyState.TextAlign = ContentAlignment.MiddleCenter;
            modernEmptyState.Text = english ? "No matching tools\r\nTry another keyword" : "没有找到匹配的工具\r\n请更换关键词";
            modernEmptyState.Visible = false;
            host.Controls.Add(modernEmptyState);
            return host;
        }

        private Button CreateToolboxCommandButton(string name, ModernVectorIconFactory.Glyph glyph, string tooltip)
        {
            Button button = new Button();
            button.Name = name;
            button.Width = 28;
            button.Height = 24;
            button.Margin = Padding.Empty;
            button.Padding = Padding.Empty;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ModernUiTheme.Selection;
            button.FlatAppearance.MouseDownBackColor = ModernUiTheme.AccentSoft;
            button.BackColor = ModernUiTheme.Surface;
            button.Cursor = Cursors.Hand;
            button.Image = ModernVectorIconFactory.Get(
                glyph,
                ModernVectorIconFactory.GetPixelSize(this, 18),
                ModernUiTheme.Accent);
            ToolTip toolTip = new ToolTip(components);
            toolTip.SetToolTip(button, tooltip);
            return button;
        }

        /// <summary>
        /// 在旧节点清单完成构建及七类归并后建立只读目录快照。筛选时只替换显示
        /// 克隆，不改动工具名称、图标索引、Tag 或 AddTool 的分派逻辑。
        /// </summary>
        private void FinalizeModernToolboxUi()
        {
            modernToolCatalog.Clear();
            modernExpandedToolCategories.Clear();
            foreach (TreeNode root in tvw_tools.Nodes)
            {
                modernToolCatalog.Add((TreeNode)root.Clone());
                if (root.IsExpanded)
                    modernExpandedToolCategories.Add(root.Text);
            }

            modernToolTotal = modernToolCatalog.Sum(CountLeafTools);
            ApplyModernToolFilter();
        }

        private void ModernToolSearch_QueryChanged(object sender, EventArgs e)
        {
            ApplyModernToolFilter();
        }

        private void ApplyModernToolFilter()
        {
            if (rebuildingModernToolTree || modernToolCatalog.Count == 0)
                return;

            rebuildingModernToolTree = true;
            try
            {
                string query = modernToolSearch == null ? string.Empty : modernToolSearch.Query.Trim();
                tvw_tools.BeginUpdate();
                try
                {
                    tvw_tools.Nodes.Clear();
                    foreach (TreeNode sourceRoot in modernToolCatalog)
                    {
                        bool categoryMatches = MatchesToolQuery(sourceRoot, query);
                        TreeNode targetRoot = (TreeNode)sourceRoot.Clone();
                        targetRoot.Nodes.Clear();

                        foreach (TreeNode sourceTool in sourceRoot.Nodes)
                        {
                            if (categoryMatches || MatchesToolQuery(sourceTool, query))
                                targetRoot.Nodes.Add((TreeNode)sourceTool.Clone());
                        }

                        if (targetRoot.Nodes.Count > 0)
                        {
                            tvw_tools.Nodes.Add(targetRoot);
                            if (query.Length > 0 || modernExpandedToolCategories.Contains(sourceRoot.Text))
                                targetRoot.Expand();
                        }
                    }

                }
                finally
                {
                    tvw_tools.EndUpdate();
                }

                int visibleCount = tvw_tools.Nodes.Cast<TreeNode>().Sum(CountLeafTools);
                if (modernToolGrid != null)
                    modernToolGrid.SetCategories(tvw_tools.Nodes.Cast<TreeNode>());
                bool english = Project.Instance.configuration.language == Language.English;
                modernToolCount.Text = query.Length == 0
                    ? (english ? string.Format("All tools  {0}", modernToolTotal) : string.Format("全部工具  {0}", modernToolTotal))
                    : (english
                        ? string.Format("Found {0} / {1}", visibleCount, modernToolTotal)
                        : string.Format("找到 {0} / {1}", visibleCount, modernToolTotal));

                modernEmptyState.Visible = visibleCount == 0;
                if (modernEmptyState.Visible)
                    modernEmptyState.BringToFront();
                else
                    modernToolGrid.BringToFront();

                if (query.Length > 0)
                {
                    TreeNode firstTool = FindFirstVisibleTool();
                    if (firstTool != null)
                        tvw_tools.SelectedNode = firstTool;
                }
                if (modernToolGrid != null)
                    modernToolGrid.Invalidate();
            }
            finally
            {
                rebuildingModernToolTree = false;
            }
        }

        private static bool MatchesToolQuery(TreeNode node, string query)
        {
            if (query.Length == 0)
                return true;

            string[] terms = query.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string haystack = node.Text + " " + (node.Tag as string ?? string.Empty);
            foreach (string term in terms)
            {
                if (haystack.IndexOf(term, StringComparison.CurrentCultureIgnoreCase) < 0)
                    return false;
            }
            return true;
        }

        private static int CountLeafTools(TreeNode root)
        {
            if (root == null)
                return 0;
            if (root.Level > 0 || root.Nodes.Count == 0)
                return root.Level > 0 ? 1 : 0;

            int count = 0;
            foreach (TreeNode child in root.Nodes)
                count += child.Nodes.Count == 0 ? 1 : CountLeafTools(child);
            return count;
        }

        private TreeNode FindFirstVisibleTool()
        {
            foreach (TreeNode root in tvw_tools.Nodes)
            {
                if (root.Nodes.Count > 0)
                    return root.Nodes[0];
            }
            return null;
        }

        private void ModernToolSearch_SearchSubmitted(object sender, EventArgs e)
        {
            TreeNode selected = tvw_tools.SelectedNode;
            if (selected != null && selected.Level > 0)
                tvw_job_DoubleClick(tvw_tools, EventArgs.Empty);
        }

        private void ModernToolSearch_MoveToResultsRequested(object sender, EventArgs e)
        {
            TreeNode first = FindFirstVisibleTool();
            if (first == null)
                return;
            tvw_tools.SelectedNode = first;
            tvw_tools.Focus();
        }

        private void ModernToolbox_AddRequested(object sender, ToolboxAddRequestedEventArgs e)
        {
            if (e.Node == null || e.Node.Level == 0)
                return;
            tvw_tools.SelectedNode = e.Node;
            tvw_job_DoubleClick(tvw_tools, EventArgs.Empty);
        }

        private void ModernToolGrid_ToolClicked(object sender, ToolboxAddRequestedEventArgs e)
        {
            if (e.Node == null)
                return;
            tvw_tools.SelectedNode = e.Node;
            tvw_job_DoubleClick(tvw_tools, EventArgs.Empty);
        }

        private void ModernToolGrid_ToolDragStarted(object sender, ToolboxAddRequestedEventArgs e)
        {
            if (e.Node == null || e.Node.Level == 0)
                return;

            tvw_tools.SelectedNode = e.Node;
            tvw_tools_ItemDrag(tvw_tools, new ItemDragEventArgs(MouseButtons.Left, e.Node));
        }

        private void ModernToolGrid_CategoryExpandedChanged(object sender, ToolboxCategoryExpandedEventArgs e)
        {
            if (e.Category == null)
                return;
            SetCategoryExpanded(e.Category, e.Expanded);
            ApplyModernToolFilter();
        }

        private void SetAllToolCategoriesExpanded(bool expanded)
        {
            modernExpandedToolCategories.Clear();
            if (expanded)
            {
                foreach (TreeNode category in modernToolCatalog)
                    modernExpandedToolCategories.Add(category.Text);
            }
            foreach (TreeNode category in tvw_tools.Nodes)
            {
                if (expanded)
                    category.Expand();
                else
                    category.Collapse();
            }
            ApplyModernToolFilter();
        }

        private void SetCategoryExpanded(TreeNode visibleCategory, bool expanded)
        {
            if (expanded)
                modernExpandedToolCategories.Add(visibleCategory.Text);
            else
                modernExpandedToolCategories.Remove(visibleCategory.Text);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F) && modernToolSearch != null)
            {
                modernToolSearch.FocusInput();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    /// <summary>
    /// 无系统边框的搜索输入，负责占位文字和清除动作；外框、搜索及清除图标
    /// 都使用与主界面一致的运行时矢量绘制。
    /// </summary>
    internal sealed class ModernToolboxSearchBox : UserControl
    {
        private readonly TextBox input;
        private readonly Button clearButton;
        private readonly string placeholder;
        private bool placeholderVisible;
        private bool updatingText;
        private bool inputFocused;

        internal ModernToolboxSearchBox(bool english)
        {
            placeholder = english ? "Search tools...  Ctrl+F" : "搜索工具…  Ctrl+F";
            Height = 26;
            BackColor = ModernUiTheme.SurfaceRaised;
            Padding = Padding.Empty;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            input = new TextBox();
            input.Name = "modernToolSearchInput";
            input.BorderStyle = BorderStyle.None;
            input.BackColor = ModernUiTheme.SurfaceRaised;
            input.ForeColor = ModernUiTheme.PrimaryText;
            input.Font = ModernUiTheme.UiFont;
            input.Location = new Point(30, 6);
            input.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            input.TextChanged += Input_TextChanged;
            input.GotFocus += Input_GotFocus;
            input.LostFocus += Input_LostFocus;
            input.KeyDown += Input_KeyDown;
            Controls.Add(input);

            clearButton = new Button();
            clearButton.Name = "modernToolSearchClear";
            clearButton.Size = new Size(22, 22);
            clearButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            clearButton.FlatStyle = FlatStyle.Flat;
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.FlatAppearance.MouseOverBackColor = ModernUiTheme.Selection;
            clearButton.FlatAppearance.MouseDownBackColor = ModernUiTheme.AccentSoft;
            clearButton.BackColor = Color.Transparent;
            clearButton.Cursor = Cursors.Hand;
            clearButton.Image = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Close,
                14,
                ModernUiTheme.SecondaryText);
            clearButton.Click += delegate { ClearQuery(); };
            Controls.Add(clearButton);

            ShowPlaceholder();
            PerformSearchLayout();
        }

        internal event EventHandler QueryChanged;
        internal event EventHandler SearchSubmitted;
        internal event EventHandler MoveToResultsRequested;

        internal string Query
        {
            get { return placeholderVisible ? string.Empty : input.Text; }
        }

        internal void FocusInput()
        {
            input.Focus();
            input.SelectAll();
        }

        internal void ApplyInputPalette()
        {
            input.BackColor = ModernUiTheme.SurfaceRaised;
            input.BorderStyle = BorderStyle.None;
            input.ForeColor = placeholderVisible
                ? Color.FromArgb(139, 153, 165)
                : ModernUiTheme.PrimaryText;
        }

        internal void ClearQuery()
        {
            updatingText = true;
            placeholderVisible = false;
            input.Text = string.Empty;
            input.ForeColor = ModernUiTheme.PrimaryText;
            clearButton.Visible = false;
            updatingText = false;
            OnQueryChanged();
            input.Focus();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PerformSearchLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle border = new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1));
            using (GraphicsPath path = CreateRoundedPath(border, 6))
            using (Pen pen = new Pen(inputFocused ? ModernUiTheme.Accent : ModernUiTheme.Border,
                inputFocused ? 1.4F : 1F))
            {
                e.Graphics.DrawPath(pen, path);
            }

            Image searchIcon = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Search,
                15,
                inputFocused ? ModernUiTheme.Accent : ModernUiTheme.SecondaryText);
            e.Graphics.DrawImage(searchIcon, new Rectangle(9, (Height - 15) / 2, 15, 15));
        }

        private void PerformSearchLayout()
        {
            if (input == null || clearButton == null)
                return;
            clearButton.Location = new Point(Math.Max(35, Width - clearButton.Width - 5),
                Math.Max(3, (Height - clearButton.Height) / 2));
            input.Location = new Point(30, Math.Max(3, (Height - input.PreferredHeight) / 2));
            input.Width = Math.Max(20, clearButton.Left - input.Left - 3);
        }

        private void Input_TextChanged(object sender, EventArgs e)
        {
            if (updatingText)
                return;
            clearButton.Visible = input.TextLength > 0;
            OnQueryChanged();
        }

        private void Input_GotFocus(object sender, EventArgs e)
        {
            inputFocused = true;
            if (placeholderVisible)
            {
                updatingText = true;
                placeholderVisible = false;
                input.Text = string.Empty;
                input.ForeColor = ModernUiTheme.PrimaryText;
                updatingText = false;
            }
            Invalidate();
        }

        private void Input_LostFocus(object sender, EventArgs e)
        {
            inputFocused = false;
            if (input.TextLength == 0)
                ShowPlaceholder();
            Invalidate();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ClearQuery();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                EventHandler handler = SearchSubmitted;
                if (handler != null)
                    handler(this, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                EventHandler handler = MoveToResultsRequested;
                if (handler != null)
                    handler(this, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
        }

        private void ShowPlaceholder()
        {
            updatingText = true;
            placeholderVisible = true;
            input.ForeColor = Color.FromArgb(139, 153, 165);
            input.Text = placeholder;
            clearButton.Visible = false;
            updatingText = false;
        }

        private void OnQueryChanged()
        {
            EventHandler handler = QueryChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
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
    }
}
