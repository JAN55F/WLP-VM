using Controls;
namespace VMPro
{
    partial class Frm_EthernetReceiveTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_EthernetReceiveTool));
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btn_runJob = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_runTool = new System.Windows.Forms.Button();
            this.lbl_runTime = new System.Windows.Forms.Label();
            this.lbl_toolTip = new System.Windows.Forms.Label();
            this.pic_onOff = new System.Windows.Forms.PictureBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_endCharEnter = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_endCharNone = new System.Windows.Forms.Button();
            this.cButton1 = new Controls.CButton();
            this.cNumericUpDown1 = new Controls.CNumericUpDown();
            this.comboBox1222 = new Controls.CComboBox();
            this.tbx_imageSavePath = new Controls.CTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cCheckBox1 = new Controls.CCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_onOff)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VMPro.Properties.Resources.接收区;
            this.pictureBox1.Location = new System.Drawing.Point(5, 6);
            this.pictureBox1.Size = new System.Drawing.Size(18, 16);
            // 
            // button100
            // 
            this.button100.FlatAppearance.BorderSize = 0;
            this.button100.Location = new System.Drawing.Point(522, 0);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.tableLayoutPanel1);
            this.panel3.Location = new System.Drawing.Point(2, 26);
            this.panel3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(622, 430);
            this.panel3.TabIndex = 117;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(622, 430);
            this.tableLayoutPanel1.TabIndex = 115;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btn_runJob);
            this.panel6.Controls.Add(this.btn_close);
            this.panel6.Controls.Add(this.btn_runTool);
            this.panel6.Controls.Add(this.lbl_runTime);
            this.panel6.Controls.Add(this.lbl_toolTip);
            this.panel6.Controls.Add(this.pic_onOff);
            this.panel6.Controls.Add(this.panel5);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 368);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(616, 59);
            this.panel6.TabIndex = 90;
            // 
            // btn_runJob
            // 
            this.btn_runJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_runJob.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_runJob.BackgroundImage")));
            this.btn_runJob.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_runJob.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_runJob.FlatAppearance.BorderSize = 0;
            this.btn_runJob.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_runJob.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_runJob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runJob.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runJob.ForeColor = System.Drawing.Color.White;
            this.btn_runJob.Location = new System.Drawing.Point(435, 18);
            this.btn_runJob.Name = "btn_runJob";
            this.btn_runJob.Size = new System.Drawing.Size(65, 30);
            this.btn_runJob.TabIndex = 110;
            this.btn_runJob.Text = "运行流程";
            this.btn_runJob.UseVisualStyleBackColor = true;
            this.btn_runJob.Click += new System.EventHandler(this.btn_runJob_Click);
            // 
            // btn_close
            // 
            this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_close.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_close.BackgroundImage")));
            this.btn_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_close.FlatAppearance.BorderSize = 0;
            this.btn_close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_close.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_close.ForeColor = System.Drawing.Color.White;
            this.btn_close.Location = new System.Drawing.Point(540, 18);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(65, 30);
            this.btn_close.TabIndex = 111;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_runTool
            // 
            this.btn_runTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_runTool.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_runTool.BackgroundImage")));
            this.btn_runTool.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_runTool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_runTool.FlatAppearance.BorderSize = 0;
            this.btn_runTool.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_runTool.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_runTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runTool.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runTool.ForeColor = System.Drawing.Color.White;
            this.btn_runTool.Location = new System.Drawing.Point(367, 18);
            this.btn_runTool.Name = "btn_runTool";
            this.btn_runTool.Size = new System.Drawing.Size(65, 30);
            this.btn_runTool.TabIndex = 0;
            this.btn_runTool.TabStop = false;
            this.btn_runTool.Text = "运行工具";
            this.btn_runTool.UseVisualStyleBackColor = true;
            this.btn_runTool.Click += new System.EventHandler(this.tsb_runTool_Click);
            // 
            // lbl_runTime
            // 
            this.lbl_runTime.AutoSize = true;
            this.lbl_runTime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_runTime.Location = new System.Drawing.Point(94, 17);
            this.lbl_runTime.Name = "lbl_runTime";
            this.lbl_runTime.Size = new System.Drawing.Size(68, 17);
            this.lbl_runTime.TabIndex = 115;
            this.lbl_runTime.Text = "耗时：0ms";
            // 
            // lbl_toolTip
            // 
            this.lbl_toolTip.AutoSize = true;
            this.lbl_toolTip.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_toolTip.Location = new System.Drawing.Point(94, 34);
            this.lbl_toolTip.Name = "lbl_toolTip";
            this.lbl_toolTip.Size = new System.Drawing.Size(56, 17);
            this.lbl_toolTip.TabIndex = 114;
            this.lbl_toolTip.Text = "状态：无";
            // 
            // pic_onOff
            // 
            this.pic_onOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pic_onOff.Image = ((System.Drawing.Image)(resources.GetObject("pic_onOff.Image")));
            this.pic_onOff.Location = new System.Drawing.Point(18, 22);
            this.pic_onOff.Name = "pic_onOff";
            this.pic_onOff.Size = new System.Drawing.Size(50, 25);
            this.pic_onOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_onOff.TabIndex = 105;
            this.pic_onOff.TabStop = false;
            this.pic_onOff.Click += new System.EventHandler(this.pic_onOff_Click);
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel5.Location = new System.Drawing.Point(7, 4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(604, 1);
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
            this.tsb_resetTool});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(2, 0);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(620, 25);
            this.toolStrip1.TabIndex = 92;
            this.toolStrip1.Text = "toolStrip1";
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
            this.tsb_resetTool.Click += new System.EventHandler(this.tsb_resetTool_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btn_endCharEnter);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btn_endCharNone);
            this.panel2.Controls.Add(this.cButton1);
            this.panel2.Controls.Add(this.cNumericUpDown1);
            this.panel2.Controls.Add(this.comboBox1222);
            this.panel2.Controls.Add(this.tbx_imageSavePath);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.cCheckBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(616, 334);
            this.panel2.TabIndex = 93;
            // 
            // btn_endCharEnter
            // 
            this.btn_endCharEnter.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_endCharEnter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_endCharEnter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_endCharEnter.FlatAppearance.BorderSize = 0;
            this.btn_endCharEnter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btn_endCharEnter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btn_endCharEnter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_endCharEnter.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_endCharEnter.ForeColor = System.Drawing.Color.White;
            this.btn_endCharEnter.Location = new System.Drawing.Point(121, 79);
            this.btn_endCharEnter.Name = "btn_endCharEnter";
            this.btn_endCharEnter.Size = new System.Drawing.Size(42, 25);
            this.btn_endCharEnter.TabIndex = 163;
            this.btn_endCharEnter.TabStop = false;
            this.btn_endCharEnter.Text = "\\r\\n";
            this.btn_endCharEnter.UseVisualStyleBackColor = false;
            this.btn_endCharEnter.Click += new System.EventHandler(this.btn_endCharEnter_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(15, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 162;
            this.label1.Text = "结 束 符：";
            // 
            // btn_endCharNone
            // 
            this.btn_endCharNone.BackColor = System.Drawing.Color.Gray;
            this.btn_endCharNone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_endCharNone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_endCharNone.FlatAppearance.BorderSize = 0;
            this.btn_endCharNone.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btn_endCharNone.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btn_endCharNone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_endCharNone.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_endCharNone.ForeColor = System.Drawing.Color.White;
            this.btn_endCharNone.Location = new System.Drawing.Point(79, 79);
            this.btn_endCharNone.Name = "btn_endCharNone";
            this.btn_endCharNone.Size = new System.Drawing.Size(42, 25);
            this.btn_endCharNone.TabIndex = 161;
            this.btn_endCharNone.TabStop = false;
            this.btn_endCharNone.Text = "无";
            this.btn_endCharNone.UseVisualStyleBackColor = false;
            this.btn_endCharNone.Click += new System.EventHandler(this.btn_endCharNone_Click);
            // 
            // cButton1
            // 
            this.cButton1.BackColor = System.Drawing.Color.White;
            this.cButton1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cButton1.Location = new System.Drawing.Point(16, 152);
            this.cButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cButton1.Name = "cButton1";
            this.cButton1.Size = new System.Drawing.Size(67, 28);
            this.cButton1.TabIndex = 160;
            this.cButton1.TextStr = "放弃接收";
            this.cButton1.Clicked += new Controls.DClicked(this.cButton1_Clicked);
            // 
            // cNumericUpDown1
            // 
            this.cNumericUpDown1.BackColor = System.Drawing.Color.White;
            this.cNumericUpDown1.DecimalPlaces = 0;
            this.cNumericUpDown1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cNumericUpDown1.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.cNumericUpDown1.Location = new System.Drawing.Point(158, 112);
            this.cNumericUpDown1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cNumericUpDown1.MaximumSize = new System.Drawing.Size(300, 26);
            this.cNumericUpDown1.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.cNumericUpDown1.MinimumSize = new System.Drawing.Size(50, 26);
            this.cNumericUpDown1.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.cNumericUpDown1.Name = "cNumericUpDown1";
            this.cNumericUpDown1.Size = new System.Drawing.Size(117, 26);
            this.cNumericUpDown1.TabIndex = 159;
            this.cNumericUpDown1.Value = 0D;
            // 
            // comboBox1222
            // 
            this.comboBox1222.BackColor = System.Drawing.Color.White;
            this.comboBox1222.CanEdit = false;
            this.comboBox1222.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1222.Items = new string[0];
            this.comboBox1222.Location = new System.Drawing.Point(77, 14);
            this.comboBox1222.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox1222.Name = "comboBox1222";
            this.comboBox1222.SelectedIndex = -1;
            this.comboBox1222.Size = new System.Drawing.Size(198, 26);
            this.comboBox1222.TabIndex = 157;
            this.comboBox1222.TextStr = "tif";
            this.comboBox1222.SelectedIndexChanged += new Controls.DSelectedIndexChanged(this.comboBox1_SelectedIndexChanged);
            // 
            // tbx_imageSavePath
            // 
            this.tbx_imageSavePath.BackColor = System.Drawing.Color.White;
            this.tbx_imageSavePath.DefaultText = "请输入一个等待接收的命令";
            this.tbx_imageSavePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_imageSavePath.Location = new System.Drawing.Point(76, 48);
            this.tbx_imageSavePath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_imageSavePath.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_imageSavePath.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_imageSavePath.Name = "tbx_imageSavePath";
            this.tbx_imageSavePath.PasswordChar = false;
            this.tbx_imageSavePath.Size = new System.Drawing.Size(199, 22);
            this.tbx_imageSavePath.TabIndex = 154;
            this.tbx_imageSavePath.TextStr = "";
            this.tbx_imageSavePath.TextStrChanged += new Controls.DTextStrChanged(this.tbx_imageSavePath_TextStrChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(15, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 144;
            this.label4.Text = "通讯选择：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(15, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 143;
            this.label3.Text = "触发命令：";
            // 
            // cCheckBox1
            // 
            this.cCheckBox1.BackColor = System.Drawing.Color.White;
            this.cCheckBox1.Checked = false;
            this.cCheckBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cCheckBox1.Location = new System.Drawing.Point(76, 118);
            this.cCheckBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cCheckBox1.Name = "cCheckBox1";
            this.cCheckBox1.Size = new System.Drawing.Size(84, 20);
            this.cCheckBox1.TabIndex = 158;
            this.cCheckBox1.TextStr = "启用超时";
            // 
            // Frm_EthernetReceiveTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(626, 458);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(626, 458);
            this.MinimumSize = new System.Drawing.Size(626, 458);
            this.Name = "Frm_EthernetReceiveTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDK_海康威视";
            this.Controls.SetChildIndex(this.panel3, 0);
            this.Controls.SetChildIndex(this.button100, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_onOff)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel6;
        public System.Windows.Forms.Label lbl_toolTip;
        internal System.Windows.Forms.Button btn_runJob;
        internal System.Windows.Forms.Button btn_close;
        public System.Windows.Forms.PictureBox pic_onOff;
        private System.Windows.Forms.Panel panel5;
        internal System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        public System.Windows.Forms.Label lbl_runTime;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label3;
        private CCheckBox cCheckBox1;
        internal CNumericUpDown cNumericUpDown1;
        internal CComboBox comboBox1222;
        internal CTextBox tbx_imageSavePath;
        private CButton cButton1;
        internal System.Windows.Forms.Button btn_endCharEnter;
        public System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Button btn_endCharNone;
    }
}