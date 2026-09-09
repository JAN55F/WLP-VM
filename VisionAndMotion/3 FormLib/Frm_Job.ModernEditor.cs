using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_Job
    {
        private TableLayoutPanel modernEditorCommandBar;
        private TableLayoutPanel modernWorkflowSelectorHost;
        private System.Windows.Forms.Label modernWorkflowSelectorLabel;
        private ComboBox modernWorkflowSelector;
        private ToolStripButton modernShowAllConnectionsButton;
        private bool syncingWorkflowSelector;
        private bool workflowUiRefreshPending;

        private void InitializeModernEditor()
        {
            SuspendLayout();
            panel1.SuspendLayout();
            try
            {
                BackColor = ModernUiTheme.Page;
                Font = ModernUiTheme.UiFont;
                HideOnClose = true;

                label1.BackColor = ModernUiTheme.Border;
                label1.Height = 1;

                ConfigureJobTabs();
                ConfigureWorkflowSelector();
                ConfigureEditorCommandBar();
                ConfigureEditorLanguage();
                VisibleChanged -= FrmJob_VisibleChanged;
                VisibleChanged += FrmJob_VisibleChanged;
                ModernUiTheme.Apply(this);
            }
            finally
            {
                panel1.ResumeLayout(true);
                ResumeLayout(true);
            }
        }

        private void ConfigureJobTabs()
        {
            tbc_jobs.BackColor = ModernUiTheme.Surface;
            tbc_jobs.ForeColor = ModernUiTheme.PrimaryText;
            tbc_jobs.Appearance = TabAppearance.FlatButtons;
            tbc_jobs.DrawMode = TabDrawMode.Normal;
            tbc_jobs.ItemSize = new Size(1, 1);
            tbc_jobs.Padding = Point.Empty;
            tbc_jobs.SizeMode = TabSizeMode.Fixed;
            tbc_jobs.Multiline = false;
            tbc_jobs.TabStop = false;
            tbc_jobs.DrawItem -= TbcJobs_DrawItem;
            tbc_jobs.DrawItem += TbcJobs_DrawItem;
            tbc_jobs.SelectedIndexChanged -= TbcJobs_RefreshVisuals;
            tbc_jobs.SelectedIndexChanged += TbcJobs_RefreshVisuals;
            tbc_jobs.ControlAdded -= TbcJobs_ControlAdded;
            tbc_jobs.ControlAdded += TbcJobs_ControlAdded;
            tbc_jobs.ControlRemoved -= TbcJobs_ControlRemoved;
            tbc_jobs.ControlRemoved += TbcJobs_ControlRemoved;

            foreach (TabPage page in tbc_jobs.TabPages)
                SubscribeWorkflowPage(page);
        }

        private void ConfigureWorkflowSelector()
        {
            bool english = Project.Instance.configuration.language == Language.English;

            modernWorkflowSelectorHost = new TableLayoutPanel();
            modernWorkflowSelectorHost.Name = "modernWorkflowSelectorHost";
            modernWorkflowSelectorHost.Dock = DockStyle.Top;
            modernWorkflowSelectorHost.Height = 44;
            modernWorkflowSelectorHost.Margin = Padding.Empty;
            modernWorkflowSelectorHost.Padding = new Padding(8, 6, 8, 5);
            modernWorkflowSelectorHost.ColumnCount = 2;
            modernWorkflowSelectorHost.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 76F));
            modernWorkflowSelectorHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            modernWorkflowSelectorHost.RowCount = 1;
            modernWorkflowSelectorHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            modernWorkflowSelectorHost.BackColor = ModernUiTheme.Surface;

            modernWorkflowSelectorLabel = new System.Windows.Forms.Label();
            modernWorkflowSelectorLabel.Name = "modernWorkflowSelectorLabel";
            modernWorkflowSelectorLabel.Dock = DockStyle.Fill;
            modernWorkflowSelectorLabel.Margin = Padding.Empty;
            modernWorkflowSelectorLabel.TextAlign = ContentAlignment.MiddleLeft;
            modernWorkflowSelectorLabel.Font = ModernUiTheme.UiFontBold;
            modernWorkflowSelectorLabel.ForeColor = ModernUiTheme.SecondaryText;
            modernWorkflowSelectorLabel.Text = english ? "Workflow" : "当前流程";
            modernWorkflowSelectorHost.Controls.Add(modernWorkflowSelectorLabel, 0, 0);

            modernWorkflowSelector = new ComboBox();
            modernWorkflowSelector.Name = "modernWorkflowSelector";
            modernWorkflowSelector.Dock = DockStyle.Fill;
            modernWorkflowSelector.Margin = new Padding(0);
            modernWorkflowSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            modernWorkflowSelector.FlatStyle = FlatStyle.Flat;
            modernWorkflowSelector.DrawMode = DrawMode.OwnerDrawFixed;
            modernWorkflowSelector.ItemHeight = 26;
            modernWorkflowSelector.IntegralHeight = false;
            modernWorkflowSelector.DropDownHeight = 286;
            modernWorkflowSelector.MaxDropDownItems = 10;
            modernWorkflowSelector.Font = ModernUiTheme.UiFont;
            modernWorkflowSelector.BackColor = ModernUiTheme.SurfaceRaised;
            modernWorkflowSelector.ForeColor = ModernUiTheme.PrimaryText;
            modernWorkflowSelector.AccessibleName = english ? "Current workflow" : "当前流程";
            modernWorkflowSelector.DrawItem += ModernWorkflowSelector_DrawItem;
            modernWorkflowSelector.SelectedIndexChanged += ModernWorkflowSelector_SelectedIndexChanged;
            modernWorkflowSelector.DropDown += delegate { SyncWorkflowSelector(); };
            modernWorkflowSelectorHost.Controls.Add(modernWorkflowSelector, 1, 0);

            Controls.Add(modernWorkflowSelectorHost);
            modernWorkflowSelectorHost.SendToBack();
            SyncWorkflowSelector();
        }

        private void TbcJobs_ControlAdded(object sender, ControlEventArgs e)
        {
            SubscribeWorkflowPage(e.Control as TabPage);
            // 构造阶段工具栏因没有流程而被禁用。第一个默认流程加入时不会保证
            // SelectedIndexChanged 再触发一次，因此必须在集合变化处主动完成激活。
            ActivateCurrentWorkflowEditor();
            ScheduleWorkflowUiRefresh();
        }

        private void TbcJobs_ControlRemoved(object sender, ControlEventArgs e)
        {
            TabPage page = e.Control as TabPage;
            if (page != null)
                page.TextChanged -= WorkflowPage_TextChanged;

            // Some TabControl mutation paths raise ControlRemoved before TabPages has
            // completed its internal collection update. Defer the selector rebuild by
            // one UI turn; opening the selector also performs an unconditional sync for
            // legacy paths that mutate TabPages without raising this notification.
            if (IsHandleCreated && !IsDisposed)
            {
                ScheduleWorkflowUiRefresh();
            }
            else
            {
                ActivateCurrentWorkflowEditor();
            }
        }

        private void FrmJob_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                ActivateCurrentWorkflowEditor();
        }

        /// <summary>
        /// 让首次进入视觉页面与手动切换流程走同一套激活逻辑。
        /// </summary>
        internal void ActivateCurrentWorkflowEditor()
        {
            if (tbc_jobs.TabPages.Count > 0 &&
                (tbc_jobs.SelectedIndex < 0 || tbc_jobs.SelectedIndex >= tbc_jobs.TabPages.Count))
                tbc_jobs.SelectedIndex = 0;

            TbcJobs_RefreshVisuals(tbc_jobs, EventArgs.Empty);

            TabPage selectedPage = tbc_jobs.SelectedTab;
            if (selectedPage == null || selectedPage.Controls.Count == 0)
                return;

            FlowEditorTreeView editor = selectedPage.Controls[0] as FlowEditorTreeView;
            if (editor != null)
            {
                editor.BringToFront();
                editor.Invalidate();
                editor.RequestConnectionRefresh();
            }
        }

        private void ScheduleWorkflowUiRefresh()
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
                return;
            if (workflowUiRefreshPending)
                return;

            workflowUiRefreshPending = true;
            try
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    workflowUiRefreshPending = false;
                    if (!IsDisposed && !Disposing)
                        ActivateCurrentWorkflowEditor();
                });
            }
            catch
            {
                workflowUiRefreshPending = false;
            }
        }

        private void SubscribeWorkflowPage(TabPage page)
        {
            if (page == null)
                return;
            page.TextChanged -= WorkflowPage_TextChanged;
            page.TextChanged += WorkflowPage_TextChanged;
        }

        private void WorkflowPage_TextChanged(object sender, EventArgs e)
        {
            SyncWorkflowSelector();
        }

        private void SyncWorkflowSelector()
        {
            if (modernWorkflowSelector == null || modernWorkflowSelector.IsDisposed)
                return;

            bool english = Project.Instance.configuration.language == Language.English;
            bool hasWorkflows = tbc_jobs.TabPages.Count > 0;
            int desiredCount = hasWorkflows ? tbc_jobs.TabPages.Count : 1;
            bool rebuildItems = modernWorkflowSelector.Items.Count != desiredCount;
            if (!rebuildItems)
            {
                if (!hasWorkflows)
                {
                    rebuildItems = !string.Equals(
                        modernWorkflowSelector.Items[0].ToString(),
                        english ? "No workflows" : "暂无流程",
                        StringComparison.Ordinal);
                }
                else
                {
                    for (int index = 0; index < tbc_jobs.TabPages.Count; index++)
                    {
                        if (!string.Equals(modernWorkflowSelector.Items[index].ToString(),
                            tbc_jobs.TabPages[index].Text, StringComparison.Ordinal))
                        {
                            rebuildItems = true;
                            break;
                        }
                    }
                }
            }

            syncingWorkflowSelector = true;
            if (rebuildItems)
                modernWorkflowSelector.BeginUpdate();
            try
            {
                if (rebuildItems)
                    modernWorkflowSelector.Items.Clear();
                if (tbc_jobs.TabPages.Count == 0)
                {
                    if (rebuildItems)
                        modernWorkflowSelector.Items.Add(english ? "No workflows" : "暂无流程");
                    modernWorkflowSelector.SelectedIndex = 0;
                    modernWorkflowSelector.Enabled = false;
                }
                else
                {
                    if (rebuildItems)
                    {
                        foreach (TabPage page in tbc_jobs.TabPages)
                            modernWorkflowSelector.Items.Add(page.Text);
                    }
                    modernWorkflowSelector.Enabled = true;
                    int selectedIndex = tbc_jobs.SelectedIndex;
                    if (selectedIndex < 0 || selectedIndex >= tbc_jobs.TabPages.Count)
                        selectedIndex = 0;
                    modernWorkflowSelector.SelectedIndex = selectedIndex;
                }
            }
            finally
            {
                if (rebuildItems)
                    modernWorkflowSelector.EndUpdate();
                syncingWorkflowSelector = false;
            }
            if (rebuildItems)
                modernWorkflowSelector.Invalidate();
        }

        private void ModernWorkflowSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (syncingWorkflowSelector || modernWorkflowSelector == null)
                return;

            int selectedIndex = modernWorkflowSelector.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < tbc_jobs.TabPages.Count &&
                tbc_jobs.SelectedIndex != selectedIndex)
                tbc_jobs.SelectedIndex = selectedIndex;
        }

        private void ModernWorkflowSelector_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || modernWorkflowSelector == null)
                return;

            bool highlighted = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color background = highlighted ? ModernUiTheme.Selection : ModernUiTheme.SurfaceRaised;
            using (SolidBrush backgroundBrush = new SolidBrush(background))
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

            bool currentWorkflow = modernWorkflowSelector.Enabled && e.Index == tbc_jobs.SelectedIndex;
            Rectangle dot = new Rectangle(e.Bounds.Left + 10,
                e.Bounds.Top + (e.Bounds.Height - 7) / 2, 7, 7);
            using (SolidBrush dotBrush = new SolidBrush(currentWorkflow
                ? ModernUiTheme.Accent
                : Color.FromArgb(177, 190, 200)))
                e.Graphics.FillEllipse(dotBrush, dot);

            Rectangle textBounds = new Rectangle(dot.Right + 8, e.Bounds.Top,
                Math.Max(10, e.Bounds.Width - 30), e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, modernWorkflowSelector.Items[e.Index].ToString(),
                currentWorkflow ? ModernUiTheme.UiFontBold : ModernUiTheme.UiFont,
                textBounds, ModernUiTheme.PrimaryText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private void ConfigureEditorCommandBar()
        {
            panel1.BackColor = ModernUiTheme.Surface;
            panel1.Height = 54;
            panel1.Padding = new Padding(8, 6, 8, 6);

            button1.Visible = false;
            label2.Visible = false;
            // 旧运行状态链仍会更新这两个控件的 Enabled/Text，因此保留对象，
            // 但不再把重复的运行入口放进流程编辑器界面。
            btn_runLoop.Visible = false;
            btn_runOnce.Visible = false;

            modernEditorCommandBar = new TableLayoutPanel();
            modernEditorCommandBar.Name = "modernEditorCommandBar";
            modernEditorCommandBar.BackColor = ModernUiTheme.Surface;
            modernEditorCommandBar.Dock = DockStyle.Fill;
            modernEditorCommandBar.Margin = Padding.Empty;
            modernEditorCommandBar.Padding = Padding.Empty;
            modernEditorCommandBar.RowCount = 1;
            modernEditorCommandBar.ColumnCount = 1;
            modernEditorCommandBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            modernEditorCommandBar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            toolStrip1.Dock = DockStyle.Fill;
            toolStrip1.AutoSize = false;
            toolStrip1.BackColor = ModernUiTheme.Surface;
            toolStrip1.Renderer = ModernUiTheme.ToolStripRenderer;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip1.Padding = new Padding(2, 0, 2, 0);
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.ShowItemToolTips = true;

            modernShowAllConnectionsButton = new ToolStripButton();
            modernShowAllConnectionsButton.Name = "modernShowAllConnectionsButton";
            modernShowAllConnectionsButton.AutoSize = false;
            modernShowAllConnectionsButton.CheckOnClick = true;
            modernShowAllConnectionsButton.Checked = Project.Instance.configuration.displayLine;
            modernShowAllConnectionsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            modernShowAllConnectionsButton.Click += ModernShowAllConnectionsButton_Click;
            toolStrip1.Items.Add(modernShowAllConnectionsButton);

            foreach (ToolStripItem item in toolStrip1.Items)
            {
                item.AutoSize = false;
                item.Size = new Size(38, 38);
                item.Margin = new Padding(1, 2, 1, 2);
                item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            }

            modernEditorCommandBar.Controls.Add(toolStrip1, 0, 0);
            panel1.Controls.Add(modernEditorCommandBar);
            modernEditorCommandBar.BringToFront();

            ConfigureEditorIcons();
            TbcJobs_RefreshVisuals(tbc_jobs, EventArgs.Empty);
        }

        private void ConfigureEditorIcons()
        {
            int commandSize = ModernVectorIconFactory.GetPixelSize(toolStrip1, 20);
            tsb_createJob.Image = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Add, commandSize, ModernUiTheme.Accent);
            tsb_expandJob.Image = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Expand, commandSize, ModernUiTheme.Accent);
            tsb_foldJob.Image = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Collapse, commandSize, ModernUiTheme.Accent);
            tsb_deleteJob.Image = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Delete, commandSize, ModernUiTheme.Danger);
            tsb_jobInfo.Image = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Info, commandSize, ModernUiTheme.Accent);
            modernShowAllConnectionsButton.Image = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Connection, commandSize, ModernUiTheme.Accent);

        }

        private void ConfigureEditorLanguage()
        {
            bool english = Project.Instance.configuration.language == Language.English;
            Text = english ? "Workflow Editor" : "流程编辑器";
            tsb_createJob.ToolTipText = english ? "Create workflow" : "新建流程";
            tsb_expandJob.ToolTipText = english ? "Expand all nodes" : "展开全部节点";
            tsb_foldJob.ToolTipText = english ? "Collapse all nodes" : "折叠全部节点";
            tsb_deleteJob.ToolTipText = english ? "Delete workflow" : "删除当前流程";
            tsb_jobInfo.ToolTipText = english ? "Workflow properties" : "流程属性";
            modernShowAllConnectionsButton.ToolTipText = english
                ? "Show all connections (off: select an input/output port)"
                : "显示全部连线（关闭时选择具体输入/输出端口查看连线）";
            modernShowAllConnectionsButton.AccessibleName = modernShowAllConnectionsButton.ToolTipText;
        }

        private void ModernShowAllConnectionsButton_Click(object sender, EventArgs e)
        {
            Project.Instance.configuration.displayLine = modernShowAllConnectionsButton.Checked;
            RefreshConnectionDisplayMode();
        }

        internal void RefreshConnectionDisplayMode()
        {
            if (modernShowAllConnectionsButton != null)
                modernShowAllConnectionsButton.Checked = Project.Instance.configuration.displayLine;

            foreach (TabPage page in tbc_jobs.TabPages)
            {
                foreach (Control control in page.Controls)
                {
                    FlowEditorTreeView editor = control as FlowEditorTreeView;
                    if (editor != null)
                        editor.RequestConnectionRefresh();
                }
            }
        }

        private void TbcJobs_RefreshVisuals(object sender, EventArgs e)
        {
            tbc_jobs.Invalidate();
            SyncWorkflowSelector();
            bool hasWorkflow = tbc_jobs.TabPages.Count > 0 && tbc_jobs.SelectedIndex >= 0;
            if (modernShowAllConnectionsButton != null)
            {
                modernShowAllConnectionsButton.Checked = Project.Instance.configuration.displayLine;
                modernShowAllConnectionsButton.Enabled = hasWorkflow;
            }
            tsb_expandJob.Enabled = hasWorkflow;
            tsb_foldJob.Enabled = hasWorkflow;
            tsb_deleteJob.Enabled = hasWorkflow;
            tsb_jobInfo.Enabled = hasWorkflow;
            if (!hasWorkflow)
            {
                btn_runLoop.Enabled = false;
                btn_runOnce.Enabled = false;
                return;
            }

            TabPage selectedPage = tbc_jobs.TabPages[tbc_jobs.SelectedIndex];
            if (selectedPage.Controls.Count == 0)
                return;

            FlowEditorTreeView editor = selectedPage.Controls[0] as FlowEditorTreeView;
            if (editor != null)
                editor.Invalidate();
        }

        private void TbcJobs_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= tbc_jobs.TabPages.Count)
                return;

            bool selected = e.Index == tbc_jobs.SelectedIndex;
            Rectangle bounds = e.Bounds;
            Color fill = selected ? ModernUiTheme.SurfaceRaised : Color.FromArgb(241, 246, 249);
            Color text = selected ? ModernUiTheme.PrimaryText : ModernUiTheme.SecondaryText;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush fillBrush = new SolidBrush(fill))
                e.Graphics.FillRectangle(fillBrush, bounds);
            using (Pen divider = new Pen(ModernUiTheme.Border))
                e.Graphics.DrawLine(divider, bounds.Right - 1, bounds.Top + 6, bounds.Right - 1, bounds.Bottom - 6);

            Rectangle dot = new Rectangle(bounds.Left + 12, bounds.Top + (bounds.Height - 7) / 2, 7, 7);
            using (SolidBrush dotBrush = new SolidBrush(selected ? ModernUiTheme.Accent : Color.FromArgb(177, 190, 200)))
                e.Graphics.FillEllipse(dotBrush, dot);

            Rectangle textBounds = new Rectangle(dot.Right + 8, bounds.Top, bounds.Width - 38, bounds.Height);
            TextRenderer.DrawText(e.Graphics, tbc_jobs.TabPages[e.Index].Text,
                selected ? ModernUiTheme.UiFontBold : ModernUiTheme.UiFont,
                textBounds, text,
                TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            if (selected)
            {
                using (Pen accent = new Pen(ModernUiTheme.Accent, 3F))
                    e.Graphics.DrawLine(accent, bounds.Left + 6, bounds.Bottom - 2, bounds.Right - 6, bounds.Bottom - 2);
            }
        }
    }
}
