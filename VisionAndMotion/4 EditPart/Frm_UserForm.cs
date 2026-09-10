using HalconDotNet;
using MotionAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tool;
using WeifenLuo.WinFormsUI.Docking;
using WinFormsLabel = System.Windows.Forms.Label;

namespace VMPro
{
    public partial class Frm_UserForm : Form
    {
        public Frm_UserForm()
        {
            InitializeComponent();
            BuildModernDashboard();
        }

        private WinFormsLabel dashboardTitle;
        private WinFormsLabel dashboardSubtitle;
        private WinFormsLabel machineStatusValue;
        private WinFormsLabel schemeValue;
        private WinFormsLabel workflowValue;
        private WinFormsLabel runtimeValue;

        private void BuildModernDashboard()
        {
            bool english = Project.Instance.configuration.language == Language.English;

            label1.Visible = false;
            BackColor = ModernUiTheme.Page;

            TableLayoutPanel dashboard = new TableLayoutPanel();
            dashboard.Name = "modernDashboard";
            dashboard.Dock = DockStyle.Fill;
            dashboard.BackColor = ModernUiTheme.Page;
            dashboard.Padding = new Padding(24, 20, 24, 20);
            dashboard.ColumnCount = 1;
            dashboard.RowCount = 4;
            dashboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            dashboard.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            dashboard.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            dashboard.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
            dashboard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Panel heading = new Panel();
            heading.Dock = DockStyle.Fill;
            heading.Margin = Padding.Empty;
            heading.BackColor = ModernUiTheme.Page;

            dashboardTitle = new WinFormsLabel();
            dashboardTitle.AutoSize = true;
            dashboardTitle.Font = ModernUiTheme.SectionTitleFont;
            dashboardTitle.ForeColor = ModernUiTheme.PrimaryText;
            dashboardTitle.Location = new Point(0, 0);
            dashboardTitle.Text = english ? "WLP VM workspace" : "WLP VM 工作台";

            dashboardSubtitle = new WinFormsLabel();
            dashboardSubtitle.AutoEllipsis = true;
            dashboardSubtitle.Font = ModernUiTheme.UiFont;
            dashboardSubtitle.ForeColor = ModernUiTheme.SecondaryText;
            dashboardSubtitle.Location = new Point(2, 40);
            dashboardSubtitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dashboardSubtitle.Width = 760;

            heading.Controls.Add(dashboardTitle);
            heading.Controls.Add(dashboardSubtitle);
            dashboard.Controls.Add(heading, 0, 0);

            TableLayoutPanel cards = new TableLayoutPanel();
            cards.Dock = DockStyle.Fill;
            cards.Margin = Padding.Empty;
            cards.Padding = Padding.Empty;
            cards.ColumnCount = 4;
            cards.RowCount = 1;
            for (int index = 0; index < 4; index++)
                cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cards.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            machineStatusValue = AddDashboardCard(cards, 0, english ? "Machine status" : "机器状态");
            schemeValue = AddDashboardCard(cards, 1, english ? "Current solution" : "当前方案");
            workflowValue = AddDashboardCard(cards, 2, english ? "Workflows" : "流程数量");
            runtimeValue = AddDashboardCard(cards, 3, english ? "Run time" : "运行时长");
            dashboard.Controls.Add(cards, 0, 1);

            FlowLayoutPanel actions = new FlowLayoutPanel();
            actions.Dock = DockStyle.Fill;
            actions.Margin = Padding.Empty;
            actions.Padding = new Padding(0, 18, 0, 8);
            actions.WrapContents = true;
            actions.BackColor = ModernUiTheme.Page;

            actions.Controls.Add(CreateDashboardAction(
                english ? "Open vision workspace" : "进入视觉工作区",
                delegate { Machine.SwitchFrom(FormMode.VisionForm); }));
            actions.Controls.Add(CreateDashboardAction(
                english ? "Open motion control" : "进入运动控制",
                delegate { Machine.SwitchFrom(FormMode.MotionForm); }));
            actions.Controls.Add(CreateDashboardAction(
                english ? "System options" : "系统设置",
                delegate
                {
                    if (Frm_Main.Instance.toolStripButton10.Enabled)
                        Frm_Main.Instance.toolStripButton10.PerformClick();
                }));
            dashboard.Controls.Add(actions, 0, 2);

            WinFormsLabel guidance = new WinFormsLabel();
            guidance.Dock = DockStyle.Top;
            guidance.AutoSize = false;
            guidance.Height = 58;
            guidance.Padding = new Padding(14, 11, 14, 8);
            guidance.Margin = new Padding(0, 12, 0, 0);
            guidance.BackColor = Color.FromArgb(234, 244, 255);
            guidance.ForeColor = ModernUiTheme.SecondaryText;
            guidance.Font = ModernUiTheme.UiFont;
            guidance.Text = english
                ? "Read-only overview. Use the status bar and Device menu for connection details; safety and motion commands keep their existing permission checks."
                : "此页仅提供只读概览。设备连接详情请查看状态栏或“设备”菜单；安全与运动操作继续沿用现有权限和互锁逻辑。";
            ModernUiTheme.ApplyRoundedRegion(guidance, 10);
            dashboard.Controls.Add(guidance, 0, 3);

            Controls.Add(dashboard);
            dashboard.BringToFront();
            VisibleChanged += delegate
            {
                if (Visible)
                    RefreshDashboard();
            };

            ModernUiTheme.Apply(this);
            RefreshDashboard();
        }

        private static WinFormsLabel AddDashboardCard(TableLayoutPanel host, int column, string caption)
        {
            Panel card = new Panel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(column == 0 ? 0 : 8, 6, column == 3 ? 0 : 8, 12);
            card.Padding = new Padding(16, 14, 16, 12);
            ModernUiTheme.StyleCard(card);

            WinFormsLabel captionLabel = new WinFormsLabel();
            captionLabel.Dock = DockStyle.Top;
            captionLabel.Height = 28;
            captionLabel.Font = ModernUiTheme.UiFont;
            captionLabel.ForeColor = ModernUiTheme.SecondaryText;
            captionLabel.Text = caption;

            WinFormsLabel valueLabel = new WinFormsLabel();
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.AutoEllipsis = true;
            valueLabel.Font = ModernUiTheme.MetricFont;
            valueLabel.ForeColor = ModernUiTheme.PrimaryText;
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;

            card.Controls.Add(valueLabel);
            card.Controls.Add(captionLabel);
            host.Controls.Add(card, column, 0);
            return valueLabel;
        }

        private static Button CreateDashboardAction(string text, Action action)
        {
            Button button = new Button();
            button.AutoSize = false;
            button.Size = new Size(168, 44);
            button.Margin = new Padding(0, 0, 12, 0);
            button.Text = text;
            button.BackColor = ModernUiTheme.Surface;
            button.ForeColor = ModernUiTheme.PrimaryText;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = ModernUiTheme.Border;
            button.FlatAppearance.MouseOverBackColor = ModernUiTheme.Selection;
            button.Click += delegate { action(); };
            return button;
        }

        internal void RefreshDashboard()
        {
            if (machineStatusValue == null)
                return;

            bool english = Project.Instance.configuration.language == Language.English;
            Scheme scheme = Project.Instance.curEngine;
            int workflowCount = scheme != null && scheme.L_jobList != null ? scheme.L_jobList.Count : 0;

            string projectTitle = Configuration.NormalizeProgramTitle(
                Project.Instance.configuration.ProgramTitle);
            string projectContext = string.Equals(
                projectTitle,
                Configuration.DefaultProgramTitle,
                StringComparison.OrdinalIgnoreCase)
                ? (english ? "No custom project name" : "未设置自定义项目名")
                : (english ? "Current project: " : "当前项目：") + projectTitle;
            dashboardSubtitle.Text = string.Format("{0} · {1} · {2}",
                Project.Instance.configuration.CompanyName,
                Configuration.ProductName,
                projectContext);
            machineStatusValue.Text = GetMachineStatusText(Machine.machineRunStatu, english);
            machineStatusValue.ForeColor = Machine.machineRunStatu == MachineRunStatu.Running
                ? Color.FromArgb(31, 143, 84)
                : ModernUiTheme.PrimaryText;
            schemeValue.Text = scheme == null || string.IsNullOrWhiteSpace(scheme.schemeName)
                ? (english ? "Not created" : "未创建")
                : scheme.schemeName;
            workflowValue.Text = workflowCount.ToString();
            runtimeValue.Text = string.Format("{0:0.00} h", Machine.runTime.TotalHours);
        }

        private static string GetMachineStatusText(MachineRunStatu status, bool english)
        {
            if (english)
                return status.ToString();

            switch (status)
            {
                case MachineRunStatu.Running:
                    return "运行中";
                case MachineRunStatu.Homing:
                    return "复位中";
                case MachineRunStatu.WaitRun:
                    return "待运行";
                case MachineRunStatu.Stop:
                    return "已停止";
                case MachineRunStatu.Pause:
                    return "已暂停";
                case MachineRunStatu.Alarm:
                    return "报警";
                default:
                    return "待复位";
            }
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_UserForm _instance;
        public static Frm_UserForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_UserForm();
                return _instance;

            }
        }


    }
    /// <summary>
    /// 轴
    /// </summary>
    public enum Axis
    {
        X,
        Y,
        YL,
        YR,
        Z,
        R,
        右侧轨道,
        中间轨道,
        左侧轨道,
        左侧调宽,
        右侧调宽,
        TR,
    }
}
