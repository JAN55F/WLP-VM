using Controls;
namespace VMPro
{
    partial class Frm_FindLineTool
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FindLineTool));
            this.btn_runTool = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbx_resultEndCol = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbx_resultStartRow = new System.Windows.Forms.TextBox();
            this.tbx_resultStartCol = new System.Windows.Forms.TextBox();
            this.tbx_resultEndRow = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.numericUpDown2 = new Controls.CNumericUpDown();
            this.numericUpDown3 = new Controls.CNumericUpDown();
            this.tbx_caliperNum = new Controls.CNumericUpDown();
            this.tbx_threshold = new Controls.CNumericUpDown();
            this.cbx_edgeSelect = new Controls.CComboBox();
            this.cbx_polarity = new Controls.CComboBox();
            this.numericUpDown1 = new Controls.CNumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new Controls.CNumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.label15 = new System.Windows.Forms.Label();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.label16 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label17 = new System.Windows.Forms.Label();
            this.tkb_exposure = new System.Windows.Forms.TrackBar();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cCheckBox3 = new Controls.CCheckBox();
            this.ckb_displayFeature = new Controls.CCheckBox();
            this.ckb_displayCaliper = new Controls.CCheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tbx_lineStartRow = new Controls.CTextBox();
            this.tbx_lineStartCol = new Controls.CTextBox();
            this.tbx_lineEndRow = new Controls.CTextBox();
            this.tbx_lineEndCol = new Controls.CTextBox();
            this.tbx_lineAngle = new Controls.CTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.hWindow_Final1 = new ChoiceTech.Halcon.Control.HWindow_Final();
            this.cnt_rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.适应图像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.实时ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全屏显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图像另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btn_confirm = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_realTimeDisplay = new System.Windows.Forms.ToolStripButton();
            this.tsb_saveImage = new System.Windows.Forms.ToolStripButton();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tkb_exposure)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.cnt_rightClickMenu.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Location = new System.Drawing.Point(26, 5);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 5);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            // 
            // button100
            // 
            this.button100.FlatAppearance.BorderSize = 0;
            this.button100.Location = new System.Drawing.Point(803, 0);
            this.button100.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            // 
            // btn_runTool
            // 
            this.btn_runTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runTool.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runTool.Location = new System.Drawing.Point(375, 212);
            this.btn_runTool.Name = "btn_runTool";
            this.btn_runTool.Size = new System.Drawing.Size(84, 32);
            this.btn_runTool.TabIndex = 1;
            this.btn_runTool.Text = "运行";
            this.btn_runTool.UseVisualStyleBackColor = true;
            this.btn_runTool.Click += new System.EventHandler(this.btn_runTool_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbx_resultEndCol);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.tbx_resultStartRow);
            this.groupBox2.Controls.Add(this.tbx_resultStartCol);
            this.groupBox2.Controls.Add(this.tbx_resultEndRow);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(287, 27);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox2.Size = new System.Drawing.Size(172, 107);
            this.groupBox2.TabIndex = 144;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "结果线段";
            // 
            // tbx_resultEndCol
            // 
            this.tbx_resultEndCol.Location = new System.Drawing.Point(88, 80);
            this.tbx_resultEndCol.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.tbx_resultEndCol.Name = "tbx_resultEndCol";
            this.tbx_resultEndCol.Size = new System.Drawing.Size(61, 23);
            this.tbx_resultEndCol.TabIndex = 108;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(23, 83);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 17);
            this.label8.TabIndex = 109;
            this.label8.Text = "终点列坐标：";
            // 
            // tbx_resultStartRow
            // 
            this.tbx_resultStartRow.Location = new System.Drawing.Point(88, 21);
            this.tbx_resultStartRow.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.tbx_resultStartRow.Name = "tbx_resultStartRow";
            this.tbx_resultStartRow.Size = new System.Drawing.Size(61, 23);
            this.tbx_resultStartRow.TabIndex = 102;
            // 
            // tbx_resultStartCol
            // 
            this.tbx_resultStartCol.Location = new System.Drawing.Point(88, 41);
            this.tbx_resultStartCol.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.tbx_resultStartCol.Name = "tbx_resultStartCol";
            this.tbx_resultStartCol.Size = new System.Drawing.Size(61, 23);
            this.tbx_resultStartCol.TabIndex = 104;
            // 
            // tbx_resultEndRow
            // 
            this.tbx_resultEndRow.Location = new System.Drawing.Point(88, 61);
            this.tbx_resultEndRow.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.tbx_resultEndRow.Name = "tbx_resultEndRow";
            this.tbx_resultEndRow.Size = new System.Drawing.Size(61, 23);
            this.tbx_resultEndRow.TabIndex = 106;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 23);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 17);
            this.label7.TabIndex = 103;
            this.label7.Text = "起点行坐标：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(23, 63);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 17);
            this.label10.TabIndex = 107;
            this.label10.Text = "终点行坐标：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(23, 43);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 17);
            this.label9.TabIndex = 105;
            this.label9.Text = "起点列坐标：";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(622, 2);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(272, 464);
            this.tabControl1.TabIndex = 170;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.numericUpDown2);
            this.tabPage1.Controls.Add(this.numericUpDown3);
            this.tabPage1.Controls.Add(this.tbx_caliperNum);
            this.tabPage1.Controls.Add(this.tbx_threshold);
            this.tabPage1.Controls.Add(this.cbx_edgeSelect);
            this.tabPage1.Controls.Add(this.cbx_polarity);
            this.tabPage1.Controls.Add(this.numericUpDown1);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.button7);
            this.tabPage1.Controls.Add(this.trackBar3);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.trackBar2);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.trackBar1);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.tkb_exposure);
            this.tabPage1.Controls.Add(this.label18);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.btn_runTool);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Size = new System.Drawing.Size(264, 434);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "参数";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.BackColor = System.Drawing.Color.White;
            this.numericUpDown2.DecimalPlaces = 0;
            this.numericUpDown2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown2.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Location = new System.Drawing.Point(187, 91);
            this.numericUpDown2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericUpDown2.MaximumSize = new System.Drawing.Size(300, 26);
            this.numericUpDown2.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown2.MinimumSize = new System.Drawing.Size(50, 26);
            this.numericUpDown2.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(70, 26);
            this.numericUpDown2.TabIndex = 237;
            this.numericUpDown2.Value = 100D;
            this.numericUpDown2.ValueChanged += new Controls.DValueChanged(this.numericUpDown2_ValueChanged);
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.BackColor = System.Drawing.Color.White;
            this.numericUpDown3.DecimalPlaces = 0;
            this.numericUpDown3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown3.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown3.Location = new System.Drawing.Point(187, 63);
            this.numericUpDown3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericUpDown3.MaximumSize = new System.Drawing.Size(300, 26);
            this.numericUpDown3.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown3.MinimumSize = new System.Drawing.Size(50, 26);
            this.numericUpDown3.MinValue = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(70, 26);
            this.numericUpDown3.TabIndex = 236;
            this.numericUpDown3.Value = 100D;
            this.numericUpDown3.ValueChanged += new Controls.DValueChanged(this.numericUpDown3_ValueChanged);
            // 
            // tbx_caliperNum
            // 
            this.tbx_caliperNum.BackColor = System.Drawing.Color.White;
            this.tbx_caliperNum.DecimalPlaces = 0;
            this.tbx_caliperNum.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_caliperNum.Incremeent = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.tbx_caliperNum.Location = new System.Drawing.Point(187, 35);
            this.tbx_caliperNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_caliperNum.MaximumSize = new System.Drawing.Size(300, 26);
            this.tbx_caliperNum.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.tbx_caliperNum.MinimumSize = new System.Drawing.Size(50, 26);
            this.tbx_caliperNum.MinValue = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.tbx_caliperNum.Name = "tbx_caliperNum";
            this.tbx_caliperNum.Size = new System.Drawing.Size(70, 26);
            this.tbx_caliperNum.TabIndex = 235;
            this.tbx_caliperNum.Value = 100D;
            this.tbx_caliperNum.ValueChanged += new Controls.DValueChanged(this.tbx_caliperNum_ValueChanged);
            // 
            // tbx_threshold
            // 
            this.tbx_threshold.BackColor = System.Drawing.Color.White;
            this.tbx_threshold.DecimalPlaces = 0;
            this.tbx_threshold.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_threshold.Incremeent = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.tbx_threshold.Location = new System.Drawing.Point(187, 7);
            this.tbx_threshold.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_threshold.MaximumSize = new System.Drawing.Size(300, 26);
            this.tbx_threshold.MaxValue = new decimal(new int[] {
            254,
            0,
            0,
            0});
            this.tbx_threshold.MinimumSize = new System.Drawing.Size(50, 26);
            this.tbx_threshold.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbx_threshold.Name = "tbx_threshold";
            this.tbx_threshold.Size = new System.Drawing.Size(70, 26);
            this.tbx_threshold.TabIndex = 234;
            this.tbx_threshold.Value = 100D;
            this.tbx_threshold.ValueChanged += new Controls.DValueChanged(this.tbx_threshold_ValueChanged);
            // 
            // cbx_edgeSelect
            // 
            this.cbx_edgeSelect.BackColor = System.Drawing.Color.White;
            this.cbx_edgeSelect.CanEdit = false;
            this.cbx_edgeSelect.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_edgeSelect.Items = new string[] {
        "首点",
        "末点",
        "所有点"};
            this.cbx_edgeSelect.Location = new System.Drawing.Point(53, 158);
            this.cbx_edgeSelect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_edgeSelect.Name = "cbx_edgeSelect";
            this.cbx_edgeSelect.SelectedIndex = 2;
            this.cbx_edgeSelect.Size = new System.Drawing.Size(131, 26);
            this.cbx_edgeSelect.TabIndex = 214;
            this.cbx_edgeSelect.TextStr = "所有点";
            this.cbx_edgeSelect.SelectedIndexChanged += new Controls.DSelectedIndexChanged(this.cbx_edgeSelect_SelectedIndexChanged);
            // 
            // cbx_polarity
            // 
            this.cbx_polarity.BackColor = System.Drawing.Color.White;
            this.cbx_polarity.CanEdit = false;
            this.cbx_polarity.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_polarity.Items = new string[] {
        "从明到暗",
        "从暗到明"};
            this.cbx_polarity.Location = new System.Drawing.Point(53, 130);
            this.cbx_polarity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_polarity.Name = "cbx_polarity";
            this.cbx_polarity.SelectedIndex = 0;
            this.cbx_polarity.Size = new System.Drawing.Size(131, 26);
            this.cbx_polarity.TabIndex = 213;
            this.cbx_polarity.TextStr = "从明到暗";
            this.cbx_polarity.SelectedIndexChanged += new Controls.DSelectedIndexChanged(this.cbx_polarity_SelectedIndexChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.White;
            this.numericUpDown1.DecimalPlaces = 2;
            this.numericUpDown1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown1.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown1.Location = new System.Drawing.Point(53, 213);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericUpDown1.MaximumSize = new System.Drawing.Size(300, 26);
            this.numericUpDown1.MaxValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.MinimumSize = new System.Drawing.Size(50, 26);
            this.numericUpDown1.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(133, 26);
            this.numericUpDown1.TabIndex = 233;
            this.numericUpDown1.Value = 0.5D;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(5, 216);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(56, 17);
            this.label14.TabIndex = 232;
            this.label14.Text = "分   数：";
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Image = ((System.Drawing.Image)(resources.GetObject("button5.Image")));
            this.button5.Location = new System.Drawing.Point(190, 154);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(68, 25);
            this.button5.TabIndex = 231;
            this.button5.Text = "切换";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 160);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 230;
            this.label3.Text = "点选择：";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.DecimalPlaces = 0;
            this.textBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox2.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textBox2.Location = new System.Drawing.Point(53, 183);
            this.textBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox2.MaximumSize = new System.Drawing.Size(300, 26);
            this.textBox2.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.textBox2.MinimumSize = new System.Drawing.Size(50, 26);
            this.textBox2.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(133, 26);
            this.textBox2.TabIndex = 229;
            this.textBox2.Value = 0D;
            this.textBox2.ValueChanged += new Controls.DValueChanged(this.textBox2_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 188);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 228;
            this.label4.Text = "忽略点：";
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button7.FlatAppearance.BorderSize = 0;
            this.button7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.button7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.ForeColor = System.Drawing.Color.White;
            this.button7.Image = ((System.Drawing.Image)(resources.GetObject("button7.Image")));
            this.button7.Location = new System.Drawing.Point(190, 126);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(68, 25);
            this.button7.TabIndex = 227;
            this.button7.Text = "切换";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // trackBar3
            // 
            this.trackBar3.AutoSize = false;
            this.trackBar3.BackColor = System.Drawing.Color.White;
            this.trackBar3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trackBar3.Location = new System.Drawing.Point(48, 98);
            this.trackBar3.Margin = new System.Windows.Forms.Padding(2);
            this.trackBar3.Maximum = 50;
            this.trackBar3.Minimum = 2;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(141, 19);
            this.trackBar3.TabIndex = 226;
            this.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar3.Value = 2;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(5, 98);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 17);
            this.label15.TabIndex = 225;
            this.label15.Text = "卡尺宽：";
            // 
            // trackBar2
            // 
            this.trackBar2.AutoSize = false;
            this.trackBar2.BackColor = System.Drawing.Color.White;
            this.trackBar2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trackBar2.Location = new System.Drawing.Point(48, 70);
            this.trackBar2.Margin = new System.Windows.Forms.Padding(2);
            this.trackBar2.Maximum = 100;
            this.trackBar2.Minimum = 2;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(141, 19);
            this.trackBar2.TabIndex = 224;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar2.Value = 2;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 70);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(56, 17);
            this.label16.TabIndex = 223;
            this.label16.Text = "卡尺长：";
            // 
            // trackBar1
            // 
            this.trackBar1.AutoSize = false;
            this.trackBar1.BackColor = System.Drawing.Color.White;
            this.trackBar1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trackBar1.Location = new System.Drawing.Point(48, 42);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(2);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Minimum = 2;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(141, 19);
            this.trackBar1.TabIndex = 222;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Value = 2;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(5, 42);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(56, 17);
            this.label17.TabIndex = 221;
            this.label17.Text = "卡尺数：";
            // 
            // tkb_exposure
            // 
            this.tkb_exposure.AutoSize = false;
            this.tkb_exposure.BackColor = System.Drawing.Color.White;
            this.tkb_exposure.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tkb_exposure.Location = new System.Drawing.Point(48, 14);
            this.tkb_exposure.Margin = new System.Windows.Forms.Padding(2);
            this.tkb_exposure.Maximum = 254;
            this.tkb_exposure.Minimum = 1;
            this.tkb_exposure.Name = "tkb_exposure";
            this.tkb_exposure.Size = new System.Drawing.Size(141, 19);
            this.tkb_exposure.TabIndex = 219;
            this.tkb_exposure.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tkb_exposure.Value = 1;
            this.tkb_exposure.Scroll += new System.EventHandler(this.tkb_exposure_Scroll);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(5, 14);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(56, 17);
            this.label18.TabIndex = 217;
            this.label18.Text = "阈   值：";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(5, 132);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(56, 17);
            this.label19.TabIndex = 215;
            this.label19.Text = "极   性：";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cCheckBox3);
            this.tabPage2.Controls.Add(this.ckb_displayFeature);
            this.tabPage2.Controls.Add(this.ckb_displayCaliper);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage2.Size = new System.Drawing.Size(264, 434);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "图形";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cCheckBox3
            // 
            this.cCheckBox3.BackColor = System.Drawing.Color.White;
            this.cCheckBox3.Checked = false;
            this.cCheckBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cCheckBox3.Location = new System.Drawing.Point(11, 71);
            this.cCheckBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cCheckBox3.Name = "cCheckBox3";
            this.cCheckBox3.Size = new System.Drawing.Size(97, 20);
            this.cCheckBox3.TabIndex = 4;
            this.cCheckBox3.TextStr = "显示线";
            this.cCheckBox3.CheckChanged += new Controls.DCheckChanged(this.cCheckBox3_CheckChanged);
            // 
            // ckb_displayFeature
            // 
            this.ckb_displayFeature.BackColor = System.Drawing.Color.White;
            this.ckb_displayFeature.Checked = false;
            this.ckb_displayFeature.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_displayFeature.Location = new System.Drawing.Point(11, 43);
            this.ckb_displayFeature.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_displayFeature.Name = "ckb_displayFeature";
            this.ckb_displayFeature.Size = new System.Drawing.Size(97, 20);
            this.ckb_displayFeature.TabIndex = 3;
            this.ckb_displayFeature.TextStr = "显示特征点";
            this.ckb_displayFeature.CheckChanged += new Controls.DCheckChanged(this.cCheckBox2_CheckChanged);
            // 
            // ckb_displayCaliper
            // 
            this.ckb_displayCaliper.BackColor = System.Drawing.Color.White;
            this.ckb_displayCaliper.Checked = false;
            this.ckb_displayCaliper.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_displayCaliper.Location = new System.Drawing.Point(11, 15);
            this.ckb_displayCaliper.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_displayCaliper.Name = "ckb_displayCaliper";
            this.ckb_displayCaliper.Size = new System.Drawing.Size(97, 20);
            this.ckb_displayCaliper.TabIndex = 2;
            this.ckb_displayCaliper.TextStr = "显示卡尺";
            this.ckb_displayCaliper.CheckChanged += new Controls.DCheckChanged(this.cCheckBox1_CheckChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tbx_lineStartRow);
            this.tabPage3.Controls.Add(this.tbx_lineStartCol);
            this.tabPage3.Controls.Add(this.tbx_lineEndRow);
            this.tabPage3.Controls.Add(this.tbx_lineEndCol);
            this.tabPage3.Controls.Add(this.tbx_lineAngle);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.label20);
            this.tabPage3.Controls.Add(this.label21);
            this.tabPage3.Location = new System.Drawing.Point(4, 26);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(264, 434);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "结果";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tbx_lineEndRow
            // 
            this.tbx_lineEndRow.BackColor = System.Drawing.Color.White;
            this.tbx_lineEndRow.DefaultText = "";
            this.tbx_lineEndRow.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_lineEndRow.Location = new System.Drawing.Point(60, 67);
            this.tbx_lineEndRow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_lineEndRow.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_lineEndRow.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_lineEndRow.Name = "tbx_lineEndRow";
            this.tbx_lineEndRow.PasswordChar = false;
            this.tbx_lineEndRow.Size = new System.Drawing.Size(137, 22);
            this.tbx_lineEndRow.TabIndex = 204;
            this.tbx_lineEndRow.TextStr = "";
            // 
            // tbx_lineEndCol
            // 
            this.tbx_lineEndCol.BackColor = System.Drawing.Color.White;
            this.tbx_lineEndCol.DefaultText = "";
            this.tbx_lineEndCol.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_lineEndCol.Location = new System.Drawing.Point(60, 94);
            this.tbx_lineEndCol.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_lineEndCol.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_lineEndCol.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_lineEndCol.Name = "tbx_lineEndCol";
            this.tbx_lineEndCol.PasswordChar = false;
            this.tbx_lineEndCol.Size = new System.Drawing.Size(137, 22);
            this.tbx_lineEndCol.TabIndex = 206;
            this.tbx_lineEndCol.TextStr = "";
            // 
            // tbx_lineAngle
            // 
            this.tbx_lineAngle.BackColor = System.Drawing.Color.White;
            this.tbx_lineAngle.DefaultText = "";
            this.tbx_lineAngle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_lineAngle.Location = new System.Drawing.Point(60, 121);
            this.tbx_lineAngle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_lineAngle.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_lineAngle.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_lineAngle.Name = "tbx_lineAngle";
            this.tbx_lineAngle.PasswordChar = false;
            this.tbx_lineAngle.Size = new System.Drawing.Size(137, 22);
            this.tbx_lineAngle.TabIndex = 208;
            this.tbx_lineAngle.TextStr = "";
            // 
            // tbx_lineStartCol
            // 
            this.tbx_lineStartCol.BackColor = System.Drawing.Color.White;
            this.tbx_lineStartCol.DefaultText = "";
            this.tbx_lineStartCol.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_lineStartCol.Location = new System.Drawing.Point(60, 40);
            this.tbx_lineStartCol.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_lineStartCol.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_lineStartCol.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_lineStartCol.Name = "tbx_lineStartCol";
            this.tbx_lineStartCol.PasswordChar = false;
            this.tbx_lineStartCol.Size = new System.Drawing.Size(137, 22);
            this.tbx_lineStartCol.TabIndex = 203;
            this.tbx_lineStartCol.TextStr = "";
            // 
            // tbx_lineStartRow
            // 
            this.tbx_lineStartRow.BackColor = System.Drawing.Color.White;
            this.tbx_lineStartRow.DefaultText = "";
            this.tbx_lineStartRow.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_lineStartRow.Location = new System.Drawing.Point(60, 13);
            this.tbx_lineStartRow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_lineStartRow.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_lineStartRow.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_lineStartRow.Name = "tbx_lineStartRow";
            this.tbx_lineStartRow.PasswordChar = false;
            this.tbx_lineStartRow.Size = new System.Drawing.Size(137, 22);
            this.tbx_lineStartRow.TabIndex = 202;
            this.tbx_lineStartRow.TextStr = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 15);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 17);
            this.label5.TabIndex = 199;
            this.label5.Text = "起点行坐标：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 69);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 17);
            this.label6.TabIndex = 201;
            this.label6.Text = "终点行坐标：";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(11, 96);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(92, 17);
            this.label20.TabIndex = 205;
            this.label20.Text = "终点列坐标：";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(11, 123);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(80, 17);
            this.label21.TabIndex = 207;
            this.label21.Text = "方向(rad)：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(11, 42);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 17);
            this.label11.TabIndex = 200;
            this.label11.Text = "起点列坐标：";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(903, 564);
            this.tableLayoutPanel1.TabIndex = 171;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.tableLayoutPanel2.Controls.Add(this.hWindow_Final1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tabControl1, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 28);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 468F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(897, 468);
            this.tableLayoutPanel2.TabIndex = 89;
            // 
            // hWindow_Final1
            // 
            this.hWindow_Final1.BackColor = System.Drawing.Color.Transparent;
            this.hWindow_Final1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hWindow_Final1.ContextMenuStrip = this.cnt_rightClickMenu;
            this.hWindow_Final1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindow_Final1.DrawModel = false;
            this.hWindow_Final1.EditModel = true;
            this.hWindow_Final1.Image = null;
            this.hWindow_Final1.Location = new System.Drawing.Point(3, 4);
            this.hWindow_Final1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.hWindow_Final1.Name = "hWindow_Final1";
            this.hWindow_Final1.Size = new System.Drawing.Size(613, 460);
            this.hWindow_Final1.TabIndex = 90;
            // 
            // cnt_rightClickMenu
            // 
            this.cnt_rightClickMenu.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cnt_rightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.适应图像ToolStripMenuItem,
            this.实时ToolStripMenuItem,
            this.显示信息ToolStripMenuItem,
            this.全屏显示ToolStripMenuItem,
            this.图像另存为ToolStripMenuItem});
            this.cnt_rightClickMenu.Name = "cnt_rightClickMenu";
            this.cnt_rightClickMenu.Size = new System.Drawing.Size(208, 114);
            // 
            // 适应图像ToolStripMenuItem
            // 
            this.适应图像ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.适应图像ToolStripMenuItem.Name = "适应图像ToolStripMenuItem";
            this.适应图像ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.适应图像ToolStripMenuItem.Text = "适应窗口";
            // 
            // 实时ToolStripMenuItem
            // 
            this.实时ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.实时ToolStripMenuItem.CheckOnClick = true;
            this.实时ToolStripMenuItem.Name = "实时ToolStripMenuItem";
            this.实时ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.实时ToolStripMenuItem.Text = "相机实时";
            // 
            // 显示信息ToolStripMenuItem
            // 
            this.显示信息ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.显示信息ToolStripMenuItem.CheckOnClick = true;
            this.显示信息ToolStripMenuItem.Name = "显示信息ToolStripMenuItem";
            this.显示信息ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.显示信息ToolStripMenuItem.Text = "显示信息";
            this.显示信息ToolStripMenuItem.Click += new System.EventHandler(this.显示信息ToolStripMenuItem_Click);
            // 
            // 全屏显示ToolStripMenuItem
            // 
            this.全屏显示ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.全屏显示ToolStripMenuItem.CheckOnClick = true;
            this.全屏显示ToolStripMenuItem.Name = "全屏显示ToolStripMenuItem";
            this.全屏显示ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.全屏显示ToolStripMenuItem.Text = "全屏（ESC退出全屏）";
            // 
            // 图像另存为ToolStripMenuItem
            // 
            this.图像另存为ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.图像另存为ToolStripMenuItem.Name = "图像另存为ToolStripMenuItem";
            this.图像另存为ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.图像另存为ToolStripMenuItem.Text = "图像另存";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btn_confirm);
            this.panel6.Controls.Add(this.btn_cancel);
            this.panel6.Controls.Add(this.button4);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Controls.Add(this.label1);
            this.panel6.Controls.Add(this.pictureBox2);
            this.panel6.Controls.Add(this.panel5);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 502);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(897, 59);
            this.panel6.TabIndex = 90;
            // 
            // btn_confirm
            // 
            this.btn_confirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_confirm.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_confirm.BackgroundImage")));
            this.btn_confirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_confirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_confirm.FlatAppearance.BorderSize = 0;
            this.btn_confirm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_confirm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_confirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_confirm.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_confirm.ForeColor = System.Drawing.Color.White;
            this.btn_confirm.Location = new System.Drawing.Point(716, 18);
            this.btn_confirm.Name = "btn_confirm";
            this.btn_confirm.Size = new System.Drawing.Size(65, 30);
            this.btn_confirm.TabIndex = 110;
            this.btn_confirm.Text = "运行流程";
            this.btn_confirm.UseVisualStyleBackColor = true;
            this.btn_confirm.Click += new System.EventHandler(this.btn_confirm_Click);
            this.btn_confirm.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_confirm.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_confirm.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_confirm.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_cancel.BackgroundImage")));
            this.btn_cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_cancel.FlatAppearance.BorderSize = 0;
            this.btn_cancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_cancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_cancel.ForeColor = System.Drawing.Color.White;
            this.btn_cancel.Location = new System.Drawing.Point(821, 18);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(65, 30);
            this.btn_cancel.TabIndex = 111;
            this.btn_cancel.Text = "关闭";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            this.btn_cancel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_cancel.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_cancel.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_cancel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(648, 18);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(65, 30);
            this.button4.TabIndex = 0;
            this.button4.TabStop = false;
            this.button4.Text = "运行工具";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            this.button4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.button4.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.button4.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.button4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(94, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 115;
            this.label2.Text = "耗时：0ms";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(94, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 114;
            this.label1.Text = "状态：无";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(18, 22);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(50, 25);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 105;
            this.pictureBox2.TabStop = false;
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel5.Location = new System.Drawing.Point(7, 4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(885, 1);
            this.panel5.TabIndex = 112;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.toolStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_realTimeDisplay,
            this.tsb_saveImage,
            this.tsb_resetTool});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(2, 0);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(901, 25);
            this.toolStrip1.TabIndex = 92;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_realTimeDisplay
            // 
            this.tsb_realTimeDisplay.AutoSize = false;
            this.tsb_realTimeDisplay.CheckOnClick = true;
            this.tsb_realTimeDisplay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_realTimeDisplay.Image = ((System.Drawing.Image)(resources.GetObject("tsb_realTimeDisplay.Image")));
            this.tsb_realTimeDisplay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_realTimeDisplay.Name = "tsb_realTimeDisplay";
            this.tsb_realTimeDisplay.RightToLeftAutoMirrorImage = true;
            this.tsb_realTimeDisplay.Size = new System.Drawing.Size(25, 22);
            this.tsb_realTimeDisplay.Text = "toolStripButton6";
            this.tsb_realTimeDisplay.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsb_realTimeDisplay.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_realTimeDisplay.ToolTipText = "相机实时";
            // 
            // tsb_saveImage
            // 
            this.tsb_saveImage.AutoSize = false;
            this.tsb_saveImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_saveImage.Image = ((System.Drawing.Image)(resources.GetObject("tsb_saveImage.Image")));
            this.tsb_saveImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_saveImage.Name = "tsb_saveImage";
            this.tsb_saveImage.RightToLeftAutoMirrorImage = true;
            this.tsb_saveImage.Size = new System.Drawing.Size(25, 22);
            this.tsb_saveImage.Text = "toolStripButton2";
            this.tsb_saveImage.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsb_saveImage.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_saveImage.ToolTipText = "图像另存";
            // 
            // tsb_resetTool
            // 
            this.tsb_resetTool.AutoSize = false;
            this.tsb_resetTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_resetTool.Image = ((System.Drawing.Image)(resources.GetObject("tsb_resetTool.Image")));
            this.tsb_resetTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_resetTool.Name = "tsb_resetTool";
            this.tsb_resetTool.RightToLeftAutoMirrorImage = true;
            this.tsb_resetTool.Size = new System.Drawing.Size(25, 22);
            this.tsb_resetTool.Text = "toolStripButton4";
            this.tsb_resetTool.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsb_resetTool.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_resetTool.ToolTipText = "重置";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.tableLayoutPanel1);
            this.panel3.Location = new System.Drawing.Point(2, 26);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(903, 564);
            this.panel3.TabIndex = 172;
            // 
            // Frm_FindLineTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(907, 592);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(499, 317);
            this.Name = "Frm_FindLineTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查找线";
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tkb_exposure)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.cnt_rightClickMenu.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.TextBox tbx_resultEndCol;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox tbx_resultStartRow;
        public System.Windows.Forms.TextBox tbx_resultStartCol;
        public System.Windows.Forms.TextBox tbx_resultEndRow;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        public ChoiceTech.Halcon.Control.HWindow_Final hWindow_Final1;
        private System.Windows.Forms.Panel panel6;
        internal System.Windows.Forms.Button btn_confirm;
        internal System.Windows.Forms.Button btn_cancel;
        internal System.Windows.Forms.Button button4;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ToolStripButton tsb_realTimeDisplay;
        private System.Windows.Forms.ToolStripButton tsb_saveImage;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.ContextMenuStrip cnt_rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem 适应图像ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 实时ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全屏显示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图像另存为ToolStripMenuItem;
        public CComboBox cbx_edgeSelect;
        public CComboBox cbx_polarity;
        public CNumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label3;
        public CNumericUpDown textBox2;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Button button7;
        internal System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.Label label15;
        internal System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.Label label16;
        internal System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label17;
        internal System.Windows.Forms.TrackBar tkb_exposure;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        public CNumericUpDown numericUpDown2;
        public CNumericUpDown numericUpDown3;
        public CNumericUpDown tbx_caliperNum;
        public CNumericUpDown tbx_threshold;
        private System.Windows.Forms.TabPage tabPage3;
        internal CTextBox tbx_lineStartRow;
        internal CTextBox tbx_lineStartCol;
        internal CTextBox tbx_lineEndRow;
        internal CTextBox tbx_lineEndCol;
        internal CTextBox tbx_lineAngle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        internal CCheckBox cCheckBox3;
        internal CCheckBox ckb_displayFeature;
        internal CCheckBox ckb_displayCaliper;
    }
}