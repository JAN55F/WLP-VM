using ChoiceTech.Halcon.Control;
using HalconDotNet;
using Tool;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace VMPro
{
    /// <summary>
    /// 彩图转 RGB 工具的执行与预览窗口。
    /// 输入、输出和工具连接由流程编辑器负责，本窗口只负责运行当前工具并查看结果。
    /// </summary>
    internal partial class Frm_ColorToRGBTool : Frm_FormBase
    {
        private readonly Color previewSurfaceColor = Color.FromArgb(247, 250, 253);
        private TableLayoutPanel previewGrid;
        private Panel previewStatusPanel;
        // VMPro 已有同名的业务结构 Label；这里必须明确使用 WinForms 标签控件。
        private System.Windows.Forms.Label lbl_previewStatus;
        private System.Windows.Forms.Label lbl_previewHint;
        private HWindow_Final hwc_input;
        private HWindow_Final hwc_red;
        private HWindow_Final hwc_green;
        private HWindow_Final hwc_blue;

        internal Frm_ColorToRGBTool()
        {
            InitializeComponent();
            InitializePreviewWorkspace();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_ColorToRGBTool _instance;
        public static Frm_ColorToRGBTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ColorToRGBTool();
                return _instance;
            }
        }

        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static ColorToRGBTool colorToRGBTool = new ColorToRGBTool();

        /// <summary>
        /// 仅绑定当前工具并刷新预览；流程的输入、输出关系仍由流程编辑器维护。
        /// </summary>
        internal void BindTool(ColorToRGBTool tool, bool enabled)
        {
            colorToRGBTool = tool ?? new ColorToRGBTool();

            ckb_colorToRGBToolEnable.CheckedChanged -= ckb_colorToRGBToolEnable_CheckedChanged;
            ckb_colorToRGBToolEnable.Checked = enabled;
            ckb_colorToRGBToolEnable.CheckedChanged += ckb_colorToRGBToolEnable_CheckedChanged;

            RefreshPreview();
            UpdateRunAvailability();
        }

        private void InitializePreviewWorkspace()
        {
            SuspendLayout();

            // 运行入口放在紧凑工具栏中，避免大面积的单一按钮占用预览空间。
            btn_runColorToRGBTool.Visible = false;
            btn_runColorToRGBTool.TabStop = false;

            toolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip1.ImageScalingSize = new Size(18, 18);
            toolStrip1.Location = new Point(8, 28);
            toolStrip1.Size = new Size(430, 27);
            toolStrip1.Padding = new Padding(0, 1, 0, 1);
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;

            ConfigureToolbarButton(tsb_runTool, "运行当前", "运行当前工具并更新 RGB 预览", 88);
            ConfigureToolbarButton(tsb_resetTool, "清除预览", "清除窗口中的预览，不会删除流程输入或输出", 88);
            ConfigureToolbarButton(tsb_help, "说明", "输入输出关系请在流程编辑器中查看", 56);
            tsb_help.Click += tsb_help_Click;
            toolStripButton1.Visible = false;
            toolStripSeparator1.Visible = false;

            ckb_colorToRGBToolEnable.Location = new Point(535, 32);
            ckb_colorToRGBToolEnable.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            previewStatusPanel = new Panel();
            previewStatusPanel.BackColor = previewSurfaceColor;
            previewStatusPanel.BorderStyle = BorderStyle.FixedSingle;
            previewStatusPanel.Location = new Point(10, 61);
            previewStatusPanel.Size = new Size(ClientSize.Width - 20, 27);
            previewStatusPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lbl_previewStatus = new System.Windows.Forms.Label();
            lbl_previewStatus.AutoEllipsis = true;
            lbl_previewStatus.Dock = DockStyle.Fill;
            lbl_previewStatus.Font = new Font("微软雅黑", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)134));
            lbl_previewStatus.ForeColor = Color.FromArgb(80, 96, 112);
            lbl_previewStatus.Padding = new Padding(9, 4, 9, 2);
            lbl_previewStatus.Text = "等待运行：输入、输出和连接关系请在流程编辑器中查看。";
            previewStatusPanel.Controls.Add(lbl_previewStatus);

            previewGrid = new TableLayoutPanel();
            previewGrid.BackColor = BackColor;
            previewGrid.ColumnCount = 2;
            previewGrid.RowCount = 2;
            previewGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            previewGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            previewGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            previewGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            previewGrid.Location = new Point(6, 92);
            previewGrid.Size = new Size(ClientSize.Width - 12, ClientSize.Height - 128);
            previewGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            previewGrid.Padding = new Padding(2);

            previewGrid.Controls.Add(CreatePreviewCard("输入彩图", Color.FromArgb(67, 145, 211), out hwc_input), 0, 0);
            previewGrid.Controls.Add(CreatePreviewCard("R 通道", Color.FromArgb(211, 82, 82), out hwc_red), 1, 0);
            previewGrid.Controls.Add(CreatePreviewCard("G 通道", Color.FromArgb(67, 166, 107), out hwc_green), 0, 1);
            previewGrid.Controls.Add(CreatePreviewCard("B 通道", Color.FromArgb(74, 132, 220), out hwc_blue), 1, 1);

            lbl_previewHint = new System.Windows.Forms.Label();
            lbl_previewHint.AutoEllipsis = true;
            lbl_previewHint.Font = new Font("微软雅黑", 8.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)134));
            lbl_previewHint.ForeColor = Color.FromArgb(120, 132, 145);
            lbl_previewHint.Location = new Point(12, ClientSize.Height - 31);
            lbl_previewHint.Size = new Size(ClientSize.Width - 24, 19);
            lbl_previewHint.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbl_previewHint.TextAlign = ContentAlignment.MiddleLeft;
            lbl_previewHint.Text = "预览只展示当前工具的输入和本次（或上次）运行生成的 RGB 三通道。";

            Controls.Add(previewGrid);
            Controls.Add(previewStatusPanel);
            Controls.Add(lbl_previewHint);
            previewGrid.BringToFront();
            previewStatusPanel.BringToFront();
            lbl_previewHint.BringToFront();
            toolStrip1.BringToFront();
            ckb_colorToRGBToolEnable.BringToFront();

            ResumeLayout(false);
        }

        private static void ConfigureToolbarButton(ToolStripButton button, string text, string toolTip, int width)
        {
            button.AutoSize = false;
            button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            button.Text = text;
            button.ToolTipText = toolTip;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.Size = new Size(width, 24);
        }

        private Panel CreatePreviewCard(string title, Color accentColor, out HWindow_Final imageWindow)
        {
            Panel card = new Panel();
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Margin = new Padding(4);

            Panel header = new Panel();
            header.BackColor = Color.FromArgb(246, 249, 252);
            header.Dock = DockStyle.Top;
            header.Height = 27;

            Panel accent = new Panel();
            accent.BackColor = accentColor;
            accent.Dock = DockStyle.Left;
            accent.Width = 4;

            System.Windows.Forms.Label caption = new System.Windows.Forms.Label();
            caption.AutoEllipsis = true;
            caption.Dock = DockStyle.Fill;
            caption.Font = new Font("微软雅黑", 8.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)134));
            caption.ForeColor = Color.FromArgb(59, 73, 87);
            caption.Padding = new Padding(8, 5, 6, 2);
            caption.Text = title;
            caption.TextAlign = ContentAlignment.MiddleLeft;

            imageWindow = new HWindow_Final();
            imageWindow.BackColor = Color.Black;
            imageWindow.BorderStyle = BorderStyle.None;
            imageWindow.Dock = DockStyle.Fill;
            imageWindow.DrawModel = false;
            imageWindow.EditModel = false;
            imageWindow.EnableImagePan = true;
            imageWindow.m_CtrlHStatusLabelCtrl.Visible = false;

            header.Controls.Add(caption);
            header.Controls.Add(accent);
            card.Controls.Add(imageWindow);
            card.Controls.Add(header);
            return card;
        }

        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            colorToRGBTool.ResetTool();
            ClearPreview();
            SetPreviewStatus("预览已清除：流程中的输入和输出数据未改动。", Color.FromArgb(80, 96, 112));
        }

        private void ckb_colorToRGBToolEnable_CheckedChanged(object sender, EventArgs e)
        {
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = ckb_colorToRGBToolEnable.Checked;
            UpdateRunAvailability();

            if (ckb_colorToRGBToolEnable.Checked)
                RefreshPreview();
        }

        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            RunCurrentTool();
        }

        private void btn_runColorToRGBTool_Click(object sender, EventArgs e)
        {
            RunCurrentTool();
        }

        private void tsb_help_Click(object sender, EventArgs e)
        {
            SetPreviewStatus("说明：本窗口用于运行和查看 RGB 预览；输入、输出和工具连接请在流程编辑器中配置。",
                Color.FromArgb(80, 96, 112));
        }

        private void RunCurrentTool()
        {
            // “禁用”必须同时约束流程执行和本窗口的手动运行，不能只改变流程树的显示状态。
            if (!ckb_colorToRGBToolEnable.Checked)
            {
                colorToRGBTool.toolRunStatu = Project.Instance.configuration.language == Language.English
                    ? ToolRunStatu.Not_Enabled
                    : ToolRunStatu.未启用;
                SetPreviewStatus("工具已禁用：当前工具不会运行。", Color.DarkGray);
                Frm_Main.Instance.OutputMsg(colorToRGBTool.toolRunStatu.ToString(), Color.DarkGray);
                return;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            colorToRGBTool.Run(true, true, toolName);
            stopwatch.Stop();

            bool succeeded = colorToRGBTool.toolRunStatu ==
                (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
            RefreshPreview();

            string message = succeeded
                ? string.Format("运行完成：已生成 R / G / B 通道，耗时 {0} ms。", stopwatch.ElapsedMilliseconds)
                : string.Format("运行失败：{0}", colorToRGBTool.toolRunStatu);
            SetPreviewStatus(message, succeeded ? Color.FromArgb(44, 126, 81) : Color.FromArgb(193, 67, 67));
            Frm_Main.Instance.OutputMsg(colorToRGBTool.toolRunStatu.ToString(), succeeded ? Color.Black : Color.Red);
        }

        private void UpdateRunAvailability()
        {
            bool enabled = ckb_colorToRGBToolEnable.Checked;
            tsb_runTool.Enabled = enabled;
            btn_runColorToRGBTool.Enabled = enabled;

            try
            {
                TreeView tree = Job.GetJobTree(jobName);
                if (tree != null)
                {
                    foreach (TreeNode node in tree.Nodes)
                    {
                        if (node.Text == toolName)
                        {
                            node.ForeColor = enabled ? Color.Black : Color.DarkGray;
                            node.ToolTipText = enabled ? string.Empty : "工具已禁用，流程运行时将跳过。";
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }

            if (!enabled)
                SetPreviewStatus("工具已禁用：流程运行和“运行当前”都会跳过该工具。", Color.DarkGray);
        }

        /// <summary>
        /// 切换到该工具或运行完成后调用，显示已有的输入和输出对象。
        /// </summary>
        internal void RefreshPreview()
        {
            if (hwc_input == null || hwc_input.IsDisposed || !IsHandleCreated)
                return;

            ShowPreviewImage(hwc_input, colorToRGBTool.inputImage);
            ShowPreviewImage(hwc_red, colorToRGBTool.outputRed);
            ShowPreviewImage(hwc_green, colorToRGBTool.outputGreen);
            ShowPreviewImage(hwc_blue, colorToRGBTool.outputBlue);

            if (colorToRGBTool.inputImage == null)
            {
                SetPreviewStatus("等待输入图像：请在流程编辑器中连接上游图像后运行。", Color.FromArgb(80, 96, 112));
            }
            else if (colorToRGBTool.outputRed != null && colorToRGBTool.outputGreen != null && colorToRGBTool.outputBlue != null)
            {
                SetPreviewStatus("已显示当前工具保存的 RGB 通道结果。", Color.FromArgb(44, 126, 81));
            }
            else
            {
                SetPreviewStatus("输入图像已就绪：点击“运行当前”生成 RGB 三通道。", Color.FromArgb(80, 96, 112));
            }
        }

        private static void ShowPreviewImage(HWindow_Final imageWindow, HObject image)
        {
            try
            {
                if (image == null || !image.IsInitialized())
                    imageWindow.ClearWindow();
                else
                    imageWindow.HobjectToHimage(image);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void ClearPreview()
        {
            ClearPreviewWindow(hwc_input);
            ClearPreviewWindow(hwc_red);
            ClearPreviewWindow(hwc_green);
            ClearPreviewWindow(hwc_blue);
        }

        private static void ClearPreviewWindow(HWindow_Final imageWindow)
        {
            try
            {
                if (imageWindow != null && !imageWindow.IsDisposed)
                    imageWindow.ClearWindow();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void SetPreviewStatus(string message, Color color)
        {
            if (lbl_previewStatus == null || lbl_previewStatus.IsDisposed)
                return;

            lbl_previewStatus.ForeColor = color;
            lbl_previewStatus.Text = message;
        }
    }
}
