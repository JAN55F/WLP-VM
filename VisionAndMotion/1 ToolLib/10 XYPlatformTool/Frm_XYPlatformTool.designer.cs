namespace VMPro
{
    partial class Frm_XYPlatformTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_XYPlatformTool));
            this.btn_runTool = new System.Windows.Forms.Button();
            this.ckb_toolEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_runJob = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbx_pickPosOffsetU = new System.Windows.Forms.TextBox();
            this.tbx_pickPosOffsetX = new System.Windows.Forms.TextBox();
            this.tbx_pickPosOffsetY = new System.Windows.Forms.TextBox();
            this.tbx_pickPosU = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbx_pickPosX = new System.Windows.Forms.TextBox();
            this.tbx_pickPosY = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_autoGet = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label24 = new System.Windows.Forms.Label();
            this.tbx_featureU = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tbx_featureX = new System.Windows.Forms.TextBox();
            this.tbx_featureY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.tbx_outputPointX = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.tbx_outputPointY = new System.Windows.Forms.TextBox();
            this.tbx_outputPointU = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tbx_inputPointX = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbx_inputPointY = new System.Windows.Forms.TextBox();
            this.tbx_inputPointU = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            // 
            // btn_runTool
            // 
            this.btn_runTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runTool.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runTool.Location = new System.Drawing.Point(461, 360);
            this.btn_runTool.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_runTool.Name = "btn_runTool";
            this.btn_runTool.Size = new System.Drawing.Size(98, 45);
            this.btn_runTool.TabIndex = 15;
            this.btn_runTool.Text = "运行";
            this.btn_runTool.UseVisualStyleBackColor = true;
            this.btn_runTool.Click += new System.EventHandler(this.btn_runShapeMatchTool_Click);
            // 
            // ckb_toolEnable
            // 
            this.ckb_toolEnable.AutoSize = true;
            this.ckb_toolEnable.BackColor = System.Drawing.Color.Transparent;
            this.ckb_toolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_toolEnable.Location = new System.Drawing.Point(545, 34);
            this.ckb_toolEnable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_toolEnable.Name = "ckb_toolEnable";
            this.ckb_toolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_toolEnable.TabIndex = 74;
            this.ckb_toolEnable.Text = "启用";
            this.ckb_toolEnable.UseVisualStyleBackColor = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_runTool,
            this.tsb_runJob,
            this.toolStripSeparator1,
            this.tsb_resetTool});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(4, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.toolStrip1.Size = new System.Drawing.Size(530, 25);
            this.toolStrip1.TabIndex = 88;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_runTool
            // 
            this.tsb_runTool.AutoSize = false;
            this.tsb_runTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_runTool.Image = ((System.Drawing.Image)(resources.GetObject("tsb_runTool.Image")));
            this.tsb_runTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_runTool.Name = "tsb_runTool";
            this.tsb_runTool.Size = new System.Drawing.Size(25, 22);
            this.tsb_runTool.Text = "toolStripButton5";
            this.tsb_runTool.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_runTool.ToolTipText = "运行工具";
            this.tsb_runTool.Click += new System.EventHandler(this.tsb_runOnce_Click);
            // 
            // tsb_runJob
            // 
            this.tsb_runJob.AutoSize = false;
            this.tsb_runJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_runJob.Image = ((System.Drawing.Image)(resources.GetObject("tsb_runJob.Image")));
            this.tsb_runJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_runJob.Name = "tsb_runJob";
            this.tsb_runJob.Size = new System.Drawing.Size(25, 22);
            this.tsb_runJob.Text = "toolStripButton1";
            this.tsb_runJob.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_runJob.ToolTipText = "运行流程";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // tsb_resetTool
            // 
            this.tsb_resetTool.AutoSize = false;
            this.tsb_resetTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_resetTool.Image = ((System.Drawing.Image)(resources.GetObject("tsb_resetTool.Image")));
            this.tsb_resetTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_resetTool.Name = "tsb_resetTool";
            this.tsb_resetTool.Size = new System.Drawing.Size(25, 22);
            this.tsb_resetTool.Text = "toolStripButton4";
            this.tsb_resetTool.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_resetTool.ToolTipText = "复位工具";
            this.tsb_resetTool.Click += new System.EventHandler(this.tsb_resetTool_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.tbx_pickPosOffsetU);
            this.groupBox2.Controls.Add(this.tbx_pickPosOffsetX);
            this.groupBox2.Controls.Add(this.tbx_pickPosOffsetY);
            this.groupBox2.Controls.Add(this.tbx_pickPosU);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbx_pickPosX);
            this.groupBox2.Controls.Add(this.tbx_pickPosY);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(24, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 142);
            this.groupBox2.TabIndex = 95;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "模板取料坐标";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(154, 35);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(30, 17);
            this.label20.TabIndex = 114;
            this.label20.Text = "mm";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(154, 62);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(30, 17);
            this.label21.TabIndex = 115;
            this.label21.Text = "mm";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(154, 85);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(20, 17);
            this.label22.TabIndex = 116;
            this.label22.Text = "度";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(103, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(44, 17);
            this.label12.TabIndex = 113;
            this.label12.Text = "补偿量";
            // 
            // tbx_pickPosOffsetU
            // 
            this.tbx_pickPosOffsetU.Location = new System.Drawing.Point(106, 81);
            this.tbx_pickPosOffsetU.Name = "tbx_pickPosOffsetU";
            this.tbx_pickPosOffsetU.Size = new System.Drawing.Size(46, 23);
            this.tbx_pickPosOffsetU.TabIndex = 112;
            this.tbx_pickPosOffsetU.TextChanged += new System.EventHandler(this.tbx_pickPosOffsetU_TextChanged);
            // 
            // tbx_pickPosOffsetX
            // 
            this.tbx_pickPosOffsetX.Location = new System.Drawing.Point(106, 31);
            this.tbx_pickPosOffsetX.Name = "tbx_pickPosOffsetX";
            this.tbx_pickPosOffsetX.Size = new System.Drawing.Size(46, 23);
            this.tbx_pickPosOffsetX.TabIndex = 110;
            this.tbx_pickPosOffsetX.TextChanged += new System.EventHandler(this.tbx_pickPosOffsetX_TextChanged);
            // 
            // tbx_pickPosOffsetY
            // 
            this.tbx_pickPosOffsetY.Location = new System.Drawing.Point(106, 56);
            this.tbx_pickPosOffsetY.Name = "tbx_pickPosOffsetY";
            this.tbx_pickPosOffsetY.Size = new System.Drawing.Size(46, 23);
            this.tbx_pickPosOffsetY.TabIndex = 111;
            this.tbx_pickPosOffsetY.TextChanged += new System.EventHandler(this.tbx_pickPosOffsetY_TextChanged);
            // 
            // tbx_pickPosU
            // 
            this.tbx_pickPosU.Location = new System.Drawing.Point(33, 81);
            this.tbx_pickPosU.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_pickPosU.Name = "tbx_pickPosU";
            this.tbx_pickPosU.Size = new System.Drawing.Size(71, 23);
            this.tbx_pickPosU.TabIndex = 108;
            this.tbx_pickPosU.TextChanged += new System.EventHandler(this.tbx_pickPosU_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 84);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 17);
            this.label4.TabIndex = 109;
            this.label4.Text = "U:";
            // 
            // tbx_pickPosX
            // 
            this.tbx_pickPosX.Location = new System.Drawing.Point(33, 31);
            this.tbx_pickPosX.Name = "tbx_pickPosX";
            this.tbx_pickPosX.Size = new System.Drawing.Size(71, 23);
            this.tbx_pickPosX.TabIndex = 74;
            this.tbx_pickPosX.TextChanged += new System.EventHandler(this.tbx_pickPosX_TextChanged);
            // 
            // tbx_pickPosY
            // 
            this.tbx_pickPosY.Location = new System.Drawing.Point(33, 56);
            this.tbx_pickPosY.Name = "tbx_pickPosY";
            this.tbx_pickPosY.Size = new System.Drawing.Size(71, 23);
            this.tbx_pickPosY.TabIndex = 75;
            this.tbx_pickPosY.TextChanged += new System.EventHandler(this.tbx_pickPosY_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 17);
            this.label2.TabIndex = 77;
            this.label2.Text = "X：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 17);
            this.label3.TabIndex = 78;
            this.label3.Text = "Y：";
            // 
            // btn_autoGet
            // 
            this.btn_autoGet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_autoGet.Location = new System.Drawing.Point(63, 111);
            this.btn_autoGet.Name = "btn_autoGet";
            this.btn_autoGet.Size = new System.Drawing.Size(71, 24);
            this.btn_autoGet.TabIndex = 80;
            this.btn_autoGet.Text = "自动获取";
            this.btn_autoGet.UseVisualStyleBackColor = true;
            this.btn_autoGet.Click += new System.EventHandler(this.btn_autoGet_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.tbx_featureU);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.btn_autoGet);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.tbx_featureX);
            this.groupBox1.Controls.Add(this.tbx_featureY);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(228, 73);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(170, 142);
            this.groupBox1.TabIndex = 96;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "模板特征点机械坐标";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(134, 84);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(32, 17);
            this.label24.TabIndex = 121;
            this.label24.Text = "弧度";
            // 
            // tbx_featureU
            // 
            this.tbx_featureU.Location = new System.Drawing.Point(63, 81);
            this.tbx_featureU.Name = "tbx_featureU";
            this.tbx_featureU.Size = new System.Drawing.Size(71, 23);
            this.tbx_featureU.TabIndex = 119;
            this.tbx_featureU.TextChanged += new System.EventHandler(this.tbx_featureU_TextChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(38, 84);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(29, 17);
            this.label25.TabIndex = 120;
            this.label25.Text = "U：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(134, 32);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(30, 17);
            this.label13.TabIndex = 117;
            this.label13.Text = "mm";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(134, 59);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(30, 17);
            this.label14.TabIndex = 118;
            this.label14.Text = "mm";
            // 
            // tbx_featureX
            // 
            this.tbx_featureX.Location = new System.Drawing.Point(63, 31);
            this.tbx_featureX.Name = "tbx_featureX";
            this.tbx_featureX.Size = new System.Drawing.Size(71, 23);
            this.tbx_featureX.TabIndex = 74;
            this.tbx_featureX.TextChanged += new System.EventHandler(this.tbx_featureX_TextChanged);
            // 
            // tbx_featureY
            // 
            this.tbx_featureY.Location = new System.Drawing.Point(63, 56);
            this.tbx_featureY.Name = "tbx_featureY";
            this.tbx_featureY.Size = new System.Drawing.Size(71, 23);
            this.tbx_featureY.TabIndex = 75;
            this.tbx_featureY.TextChanged += new System.EventHandler(this.tbx_featureY_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 17);
            this.label1.TabIndex = 77;
            this.label1.Text = "X：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 17);
            this.label5.TabIndex = 78;
            this.label5.Text = "Y：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.tbx_outputPointX);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.tbx_outputPointY);
            this.groupBox3.Controls.Add(this.tbx_outputPointU);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(188, 230);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(150, 114);
            this.groupBox3.TabIndex = 176;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "输出点";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(118, 32);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(30, 17);
            this.label18.TabIndex = 178;
            this.label18.Text = "mm";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(118, 59);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(30, 17);
            this.label19.TabIndex = 179;
            this.label19.Text = "mm";
            // 
            // tbx_outputPointX
            // 
            this.tbx_outputPointX.Location = new System.Drawing.Point(45, 29);
            this.tbx_outputPointX.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_outputPointX.Name = "tbx_outputPointX";
            this.tbx_outputPointX.ReadOnly = true;
            this.tbx_outputPointX.Size = new System.Drawing.Size(71, 23);
            this.tbx_outputPointX.TabIndex = 102;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(118, 82);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(20, 17);
            this.label23.TabIndex = 180;
            this.label23.Text = "度";
            // 
            // tbx_outputPointY
            // 
            this.tbx_outputPointY.Location = new System.Drawing.Point(45, 54);
            this.tbx_outputPointY.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_outputPointY.Name = "tbx_outputPointY";
            this.tbx_outputPointY.ReadOnly = true;
            this.tbx_outputPointY.Size = new System.Drawing.Size(71, 23);
            this.tbx_outputPointY.TabIndex = 104;
            // 
            // tbx_outputPointU
            // 
            this.tbx_outputPointU.Location = new System.Drawing.Point(45, 79);
            this.tbx_outputPointU.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_outputPointU.Name = "tbx_outputPointU";
            this.tbx_outputPointU.ReadOnly = true;
            this.tbx_outputPointU.Size = new System.Drawing.Size(71, 23);
            this.tbx_outputPointU.TabIndex = 106;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 32);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 17);
            this.label6.TabIndex = 103;
            this.label6.Text = "X：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 82);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 17);
            this.label7.TabIndex = 107;
            this.label7.Text = "U:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 57);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 17);
            this.label8.TabIndex = 105;
            this.label8.Text = "Y:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.tbx_inputPointX);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.tbx_inputPointY);
            this.groupBox4.Controls.Add(this.tbx_inputPointU);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Location = new System.Drawing.Point(24, 230);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(150, 114);
            this.groupBox4.TabIndex = 177;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "输入点";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(118, 32);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(30, 17);
            this.label15.TabIndex = 117;
            this.label15.Text = "mm";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(118, 59);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 17);
            this.label16.TabIndex = 118;
            this.label16.Text = "mm";
            // 
            // tbx_inputPointX
            // 
            this.tbx_inputPointX.Location = new System.Drawing.Point(45, 29);
            this.tbx_inputPointX.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_inputPointX.Name = "tbx_inputPointX";
            this.tbx_inputPointX.ReadOnly = true;
            this.tbx_inputPointX.Size = new System.Drawing.Size(71, 23);
            this.tbx_inputPointX.TabIndex = 102;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(118, 82);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(20, 17);
            this.label17.TabIndex = 119;
            this.label17.Text = "度";
            // 
            // tbx_inputPointY
            // 
            this.tbx_inputPointY.Location = new System.Drawing.Point(45, 54);
            this.tbx_inputPointY.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_inputPointY.Name = "tbx_inputPointY";
            this.tbx_inputPointY.ReadOnly = true;
            this.tbx_inputPointY.Size = new System.Drawing.Size(71, 23);
            this.tbx_inputPointY.TabIndex = 104;
            // 
            // tbx_inputPointU
            // 
            this.tbx_inputPointU.Location = new System.Drawing.Point(45, 79);
            this.tbx_inputPointU.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_inputPointU.Name = "tbx_inputPointU";
            this.tbx_inputPointU.ReadOnly = true;
            this.tbx_inputPointU.Size = new System.Drawing.Size(71, 23);
            this.tbx_inputPointU.TabIndex = 106;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 32);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 17);
            this.label9.TabIndex = 103;
            this.label9.Text = "X：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 82);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(20, 17);
            this.label10.TabIndex = 107;
            this.label10.Text = "U:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(20, 57);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(18, 17);
            this.label11.TabIndex = 105;
            this.label11.Text = "Y:";
            // 
            // Frm_XYPlatformTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(605, 430);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.ckb_toolEnable);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btn_runTool);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(605, 430);
            this.MinimumSize = new System.Drawing.Size(605, 430);
            this.Name = "Frm_XYPlatformTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XY平台";
            this.Controls.SetChildIndex(this.btn_runTool, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.ckb_toolEnable, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox ckb_toolEnable;
        public System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_runJob;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.TextBox tbx_pickPosX;
        public System.Windows.Forms.TextBox tbx_pickPosY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.TextBox tbx_featureX;
        public System.Windows.Forms.TextBox tbx_featureY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_autoGet;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.TextBox tbx_outputPointX;
        public System.Windows.Forms.TextBox tbx_outputPointY;
        public System.Windows.Forms.TextBox tbx_outputPointU;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.TextBox tbx_inputPointX;
        public System.Windows.Forms.TextBox tbx_inputPointY;
        public System.Windows.Forms.TextBox tbx_inputPointU;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.TextBox tbx_pickPosU;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.TextBox tbx_pickPosOffsetU;
        public System.Windows.Forms.TextBox tbx_pickPosOffsetX;
        public System.Windows.Forms.TextBox tbx_pickPosOffsetY;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label24;
        public System.Windows.Forms.TextBox tbx_featureU;
        private System.Windows.Forms.Label label25;
    }
}