using Controls;

namespace VMPro
{
    partial class Frm_FindCircleTool
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootPanel = new System.Windows.Forms.Panel();
            this.bodyLayout = new System.Windows.Forms.TableLayoutPanel();
            this.imageCard = new System.Windows.Forms.Panel();
            this.hWindow_Final1 = new ChoiceTech.Halcon.Control.HWindow_Final();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabBasic = new System.Windows.Forms.TabPage();
            this.tabRun = new System.Windows.Forms.TabPage();
            this.tabResult = new System.Windows.Forms.TabPage();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.footerButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_cancel = CreateFooterButton("关闭", false);
            this.btn_confirm = CreateFooterButton("运行流程", false);
            this.btn_runTool = CreateFooterButton("运行工具", true);
            this.btn_preview = CreateFooterButton("预览", false);
            this.lbl_toolTip = new System.Windows.Forms.Label();
            this.lbl_time = new System.Windows.Forms.Label();
            this.cbx_polarity = CreateCombo(new string[] { "从暗到明", "从明到暗", "任意极性" });
            this.comboBox1 = CreateCombo(new string[] { "第一条边", "最后一条边", "全部边" });
            this.tbx_threshold = CreateNumeric(1, 255, 1, 0);
            this.tbx_cliperNum = CreateNumeric(4, 720, 1, 0);
            this.tbx_ringRadiusLength = CreateNumeric(1, 2000, 1, 0);
            this.textBox1 = CreateNumeric(1, 1000, 1, 0);
            this.textBox2 = CreateNumeric(0, 717, 1, 0);
            this.numericUpDown1 = CreateNumeric(0.01M, 1, 0.01M, 2);
            this.ckb_displayCaliper = CreateCheckBox("显示卡尺");
            this.ckb_displayFeature = CreateCheckBox("显示边缘点");
            this.ckb_displayCircle = CreateCheckBox("显示结果圆");
            this.checkBox1 = CreateCheckBox("显示圆心");
            this.tbx_resultCircleRow = CreateResultBox();
            this.tbx_resultCircleCol = CreateResultBox();
            this.tbx_resultCircleRadius = CreateResultBox();
            this.lbl_inputStatus = new System.Windows.Forms.Label();
            this.lbl_roiSummary = new System.Windows.Forms.Label();
            this.lbl_maskSummary = new System.Windows.Forms.Label();
            this.lbl_found = new System.Windows.Forms.Label();
            this.btn_editRoi = CreateActionButton("编辑搜索圆");
            this.btn_resetRoi = CreateActionButton("重置 ROI");
            this.btn_clearMask = CreateActionButton("清除屏蔽区");

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();

            this.lbl_title.Text = "圆查找";
            this.Text = "圆查找";
            this.ClientSize = new System.Drawing.Size(1060, 700);
            this.MinimumSize = new System.Drawing.Size(920, 620);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.rootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootPanel.Padding = new System.Windows.Forms.Padding(12, 37, 12, 12);
            this.rootPanel.BackColor = System.Drawing.Color.FromArgb(248, 247, 243);

            System.Windows.Forms.Panel header = new System.Windows.Forms.Panel();
            header.Dock = System.Windows.Forms.DockStyle.Top;
            header.Height = 48;
            header.BackColor = System.Drawing.Color.FromArgb(255, 254, 250);
            System.Windows.Forms.Label headerTitle = new System.Windows.Forms.Label();
            headerTitle.AutoSize = true;
            headerTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold);
            headerTitle.ForeColor = System.Drawing.Color.FromArgb(39, 56, 72);
            headerTitle.Location = new System.Drawing.Point(14, 8);
            headerTitle.Text = "圆形边缘定位";
            System.Windows.Forms.Label headerHint = new System.Windows.Forms.Label();
            headerHint.AutoSize = true;
            headerHint.ForeColor = System.Drawing.Color.FromArgb(99, 116, 130);
            headerHint.Location = new System.Drawing.Point(14, 29);
            headerHint.Text = "拖动圆心移动 ROI，拖动圆周手柄调整半径";
            header.Controls.Add(headerTitle);
            header.Controls.Add(headerHint);

            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Height = 62;
            this.footerPanel.Padding = new System.Windows.Forms.Padding(12, 12, 12, 8);
            this.footerPanel.BackColor = System.Drawing.Color.FromArgb(255, 254, 250);
            this.footerButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.footerButtons.AutoSize = true;
            this.footerButtons.WrapContents = false;
            this.footerButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.footerButtons.Controls.Add(this.btn_cancel);
            this.footerButtons.Controls.Add(this.btn_confirm);
            this.footerButtons.Controls.Add(this.btn_runTool);
            this.footerButtons.Controls.Add(this.btn_preview);
            this.lbl_toolTip.AutoSize = true;
            this.lbl_toolTip.Location = new System.Drawing.Point(14, 12);
            this.lbl_toolTip.ForeColor = System.Drawing.Color.FromArgb(99, 116, 130);
            this.lbl_toolTip.Text = "状态：等待运行";
            this.lbl_time.AutoSize = true;
            this.lbl_time.Location = new System.Drawing.Point(14, 34);
            this.lbl_time.ForeColor = System.Drawing.Color.FromArgb(99, 116, 130);
            this.lbl_time.Text = "耗时：-- ms";
            this.footerPanel.Controls.Add(this.footerButtons);
            this.footerPanel.Controls.Add(this.lbl_toolTip);
            this.footerPanel.Controls.Add(this.lbl_time);

            this.bodyLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyLayout.ColumnCount = 2;
            this.bodyLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bodyLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.bodyLayout.RowCount = 1;
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bodyLayout.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);

            this.imageCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageCard.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.imageCard.Padding = new System.Windows.Forms.Padding(1);
            this.imageCard.BackColor = System.Drawing.Color.FromArgb(217, 227, 234);
            this.hWindow_Final1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindow_Final1.BackColor = System.Drawing.Color.FromArgb(32, 38, 44);
            this.imageCard.Controls.Add(this.hWindow_Final1);

            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Controls.Add(this.tabBasic);
            this.tabControl1.Controls.Add(this.tabRun);
            this.tabControl1.Controls.Add(this.tabResult);
            this.tabBasic.Text = "基本参数";
            this.tabRun.Text = "运行参数";
            this.tabResult.Text = "结果显示";
            this.tabBasic.BackColor = System.Drawing.Color.FromArgb(255, 254, 250);
            this.tabRun.BackColor = System.Drawing.Color.FromArgb(255, 254, 250);
            this.tabResult.BackColor = System.Drawing.Color.FromArgb(255, 254, 250);
            BuildBasicTab();
            BuildRunTab();
            BuildResultTab();

            this.bodyLayout.Controls.Add(this.imageCard, 0, 0);
            this.bodyLayout.Controls.Add(this.tabControl1, 1, 0);
            this.rootPanel.Controls.Add(this.bodyLayout);
            this.rootPanel.Controls.Add(this.footerPanel);
            this.rootPanel.Controls.Add(header);
            this.Controls.Add(this.rootPanel);
            this.Controls.SetChildIndex(this.rootPanel, this.Controls.Count - 1);

            this.btn_preview.Click += new System.EventHandler(this.btn_preview_Click);
            this.btn_runTool.Click += new System.EventHandler(this.btn_runTool_Click);
            this.btn_confirm.Click += new System.EventHandler(this.btn_confirm_Click);
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            this.btn_editRoi.Click += new System.EventHandler(this.btn_editRoi_Click);
            this.btn_resetRoi.Click += new System.EventHandler(this.btn_resetRoi_Click);
            this.btn_clearMask.Click += new System.EventHandler(this.btn_clearMask_Click);

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
        }

        private void BuildBasicTab()
        {
            System.Windows.Forms.FlowLayoutPanel stack = CreateStack();
            stack.Controls.Add(CreateSectionTitle("输入"));
            this.lbl_inputStatus.AutoSize = false;
            this.lbl_inputStatus.Size = new System.Drawing.Size(304, 42);
            this.lbl_inputStatus.Padding = new System.Windows.Forms.Padding(10);
            this.lbl_inputStatus.BackColor = System.Drawing.Color.FromArgb(232, 243, 252);
            this.lbl_inputStatus.ForeColor = System.Drawing.Color.FromArgb(52, 126, 184);
            this.lbl_inputStatus.Text = "图像：未连接";
            stack.Controls.Add(this.lbl_inputStatus);
            stack.Controls.Add(CreateSectionTitle("搜索圆 ROI"));
            this.lbl_roiSummary.AutoSize = false;
            this.lbl_roiSummary.Size = new System.Drawing.Size(304, 52);
            this.lbl_roiSummary.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.lbl_roiSummary.BackColor = System.Drawing.Color.FromArgb(248, 247, 243);
            this.lbl_roiSummary.Text = "未设置搜索圆";
            stack.Controls.Add(this.lbl_roiSummary);
            System.Windows.Forms.FlowLayoutPanel actions = new System.Windows.Forms.FlowLayoutPanel();
            actions.AutoSize = true;
            actions.WrapContents = false;
            actions.Controls.Add(this.btn_editRoi);
            actions.Controls.Add(this.btn_resetRoi);
            stack.Controls.Add(actions);
            stack.Controls.Add(CreateSectionTitle("屏蔽区兼容"));
            this.lbl_maskSummary.AutoSize = false;
            this.lbl_maskSummary.Size = new System.Drawing.Size(304, 42);
            this.lbl_maskSummary.Padding = new System.Windows.Forms.Padding(10);
            this.lbl_maskSummary.BackColor = System.Drawing.Color.FromArgb(248, 247, 243);
            stack.Controls.Add(this.lbl_maskSummary);
            stack.Controls.Add(this.btn_clearMask);
            System.Windows.Forms.Label note = new System.Windows.Forms.Label();
            note.AutoSize = false;
            note.Size = new System.Drawing.Size(304, 70);
            note.ForeColor = System.Drawing.Color.FromArgb(99, 116, 130);
            note.Text = "新版取消了会卡住界面的画笔循环。\r\n旧项目已保存的屏蔽区仍会参与测量。";
            stack.Controls.Add(note);
            this.tabBasic.Controls.Add(stack);
        }

        private void BuildRunTab()
        {
            System.Windows.Forms.TableLayoutPanel grid = CreateParameterGrid(8);
            AddParameterRow(grid, 0, "边缘极性", this.cbx_polarity);
            AddParameterRow(grid, 1, "边缘选择", this.comboBox1);
            AddParameterRow(grid, 2, "边缘阈值", this.tbx_threshold);
            AddParameterRow(grid, 3, "卡尺数量", this.tbx_cliperNum);
            AddParameterRow(grid, 4, "搜索半长", this.tbx_ringRadiusLength);
            AddParameterRow(grid, 5, "卡尺半宽", this.textBox1);
            AddParameterRow(grid, 6, "剔除点数", this.textBox2);
            AddParameterRow(grid, 7, "最小得分", this.numericUpDown1);
            System.Windows.Forms.Label hint = new System.Windows.Forms.Label();
            hint.Dock = System.Windows.Forms.DockStyle.Bottom;
            hint.Height = 76;
            hint.Padding = new System.Windows.Forms.Padding(12);
            hint.ForeColor = System.Drawing.Color.FromArgb(99, 116, 130);
            hint.Text = "调参顺序：先确定极性和搜索范围，\r\n再调阈值，最后使用剔除点处理局部毛刺。";
            this.tabRun.Controls.Add(grid);
            this.tabRun.Controls.Add(hint);
        }

        private void BuildResultTab()
        {
            System.Windows.Forms.FlowLayoutPanel stack = CreateStack();
            stack.Controls.Add(CreateSectionTitle("叠加显示"));
            stack.Controls.Add(this.ckb_displayCaliper);
            stack.Controls.Add(this.ckb_displayFeature);
            stack.Controls.Add(this.ckb_displayCircle);
            stack.Controls.Add(this.checkBox1);
            stack.Controls.Add(CreateSectionTitle("运行结果"));
            this.lbl_found.AutoSize = false;
            this.lbl_found.Size = new System.Drawing.Size(304, 38);
            this.lbl_found.Padding = new System.Windows.Forms.Padding(10);
            this.lbl_found.BackColor = System.Drawing.Color.FromArgb(248, 247, 243);
            this.lbl_found.Text = "是否找到：--";
            stack.Controls.Add(this.lbl_found);
            stack.Controls.Add(CreateResultRow("圆心 Row", this.tbx_resultCircleRow));
            stack.Controls.Add(CreateResultRow("圆心 Col", this.tbx_resultCircleCol));
            stack.Controls.Add(CreateResultRow("圆半径", this.tbx_resultCircleRadius));
            this.tabResult.Controls.Add(stack);
        }

        private static System.Windows.Forms.FlowLayoutPanel CreateStack()
        {
            System.Windows.Forms.FlowLayoutPanel panel = new System.Windows.Forms.FlowLayoutPanel();
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            panel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            panel.WrapContents = false;
            panel.AutoScroll = true;
            panel.Padding = new System.Windows.Forms.Padding(14);
            return panel;
        }

        private static System.Windows.Forms.Label CreateSectionTitle(string text)
        {
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.AutoSize = false;
            label.Size = new System.Drawing.Size(304, 30);
            label.Padding = new System.Windows.Forms.Padding(0, 7, 0, 0);
            label.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            label.ForeColor = System.Drawing.Color.FromArgb(39, 56, 72);
            label.Text = text;
            return label;
        }

        private static System.Windows.Forms.TableLayoutPanel CreateParameterGrid(int rows)
        {
            System.Windows.Forms.TableLayoutPanel grid = new System.Windows.Forms.TableLayoutPanel();
            grid.Dock = System.Windows.Forms.DockStyle.Top;
            grid.Padding = new System.Windows.Forms.Padding(14, 18, 14, 0);
            grid.ColumnCount = 2;
            grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58F));
            grid.RowCount = rows;
            grid.Height = 18 + rows * 48;
            for (int i = 0; i < rows; i++) grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            return grid;
        }

        private static void AddParameterRow(System.Windows.Forms.TableLayoutPanel grid, int row, string text, System.Windows.Forms.Control input)
        {
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Dock = System.Windows.Forms.DockStyle.Fill;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.ForeColor = System.Drawing.Color.FromArgb(39, 56, 72);
            label.Text = text;
            input.Dock = System.Windows.Forms.DockStyle.Fill;
            input.Margin = new System.Windows.Forms.Padding(4, 9, 0, 9);
            grid.Controls.Add(label, 0, row);
            grid.Controls.Add(input, 1, row);
        }

        private static System.Windows.Forms.Panel CreateResultRow(string text, System.Windows.Forms.Control input)
        {
            System.Windows.Forms.Panel row = new System.Windows.Forms.Panel();
            row.Size = new System.Drawing.Size(304, 42);
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Location = new System.Drawing.Point(0, 10);
            label.Size = new System.Drawing.Size(102, 24);
            label.Text = text;
            input.Location = new System.Drawing.Point(108, 6);
            input.Size = new System.Drawing.Size(196, 30);
            row.Controls.Add(label);
            row.Controls.Add(input);
            return row;
        }

        private static CComboBox CreateCombo(string[] items)
        {
            CComboBox combo = new CComboBox();
            combo.Items = items;
            combo.CanEdit = false;
            combo.Height = 30;
            return combo;
        }

        private static CNumericUpDown CreateNumeric(decimal min, decimal max, decimal increment, int decimals)
        {
            CNumericUpDown numeric = new CNumericUpDown();
            numeric.MinValue = min;
            numeric.MaxValue = max;
            numeric.Incremeent = increment;
            numeric.DecimalPlaces = decimals;
            numeric.Height = 30;
            return numeric;
        }

        private static CTextBox CreateResultBox()
        {
            CTextBox box = new CTextBox();
            box.Enabled = false;
            box.DefaultText = "--";
            return box;
        }

        private static System.Windows.Forms.CheckBox CreateCheckBox(string text)
        {
            System.Windows.Forms.CheckBox box = new System.Windows.Forms.CheckBox();
            box.AutoSize = false;
            box.Size = new System.Drawing.Size(304, 34);
            box.Text = text;
            return box;
        }

        private static System.Windows.Forms.Button CreateActionButton(string text)
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.AutoSize = false;
            button.Size = new System.Drawing.Size(145, 34);
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.Text = text;
            return button;
        }

        private static System.Windows.Forms.Button CreateFooterButton(string text, bool primary)
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.Size = new System.Drawing.Size(96, 36);
            button.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.Text = text;
            if (primary)
            {
                button.BackColor = System.Drawing.Color.FromArgb(76, 148, 210);
                button.ForeColor = System.Drawing.Color.White;
            }
            return button;
        }

        private System.Windows.Forms.Panel rootPanel;
        private System.Windows.Forms.TableLayoutPanel bodyLayout;
        private System.Windows.Forms.Panel imageCard;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabBasic;
        private System.Windows.Forms.TabPage tabRun;
        private System.Windows.Forms.TabPage tabResult;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.FlowLayoutPanel footerButtons;
        internal ChoiceTech.Halcon.Control.HWindow_Final hWindow_Final1;
        internal System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.Button btn_preview;
        private System.Windows.Forms.Button btn_confirm;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_editRoi;
        private System.Windows.Forms.Button btn_resetRoi;
        private System.Windows.Forms.Button btn_clearMask;
        private System.Windows.Forms.Label lbl_inputStatus;
        private System.Windows.Forms.Label lbl_roiSummary;
        private System.Windows.Forms.Label lbl_maskSummary;
        private System.Windows.Forms.Label lbl_found;
        internal System.Windows.Forms.Label lbl_toolTip;
        private System.Windows.Forms.Label lbl_time;
        internal CComboBox cbx_polarity;
        internal CComboBox comboBox1;
        internal CNumericUpDown tbx_threshold;
        internal CNumericUpDown tbx_cliperNum;
        internal CNumericUpDown tbx_ringRadiusLength;
        internal CNumericUpDown textBox1;
        internal CNumericUpDown textBox2;
        internal CNumericUpDown numericUpDown1;
        internal System.Windows.Forms.CheckBox ckb_displayCaliper;
        internal System.Windows.Forms.CheckBox ckb_displayFeature;
        internal System.Windows.Forms.CheckBox ckb_displayCircle;
        internal System.Windows.Forms.CheckBox checkBox1;
        internal CTextBox tbx_resultCircleRow;
        internal CTextBox tbx_resultCircleCol;
        internal CTextBox tbx_resultCircleRadius;
    }
}
