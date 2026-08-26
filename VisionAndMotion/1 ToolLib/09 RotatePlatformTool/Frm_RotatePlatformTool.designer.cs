namespace VMPro
{
    partial class Frm_RotatePlatformTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_RotatePlatformTool));
            this.ckb_toolEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_runJob = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.label1 = new System.Windows.Forms.Label();
            this.dgv_data = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_runTool = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbx_rotateCenterX = new System.Windows.Forms.TextBox();
            this.tbx_rotateCenterY = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_thirdPoint = new System.Windows.Forms.Button();
            this.btn_secondPoint = new System.Windows.Forms.Button();
            this.btn_firstPoint = new System.Windows.Forms.Button();
            this.btn_calculate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbx_inputPointX = new System.Windows.Forms.TextBox();
            this.tbx_inputPointY = new System.Windows.Forms.TextBox();
            this.tbx_inputPointU = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbx_outputPointX = new System.Windows.Forms.TextBox();
            this.tbx_outputPointY = new System.Windows.Forms.TextBox();
            this.tbx_outputPointU = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cbx_outputItemList = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbx_jobList = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_data)).BeginInit();
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
            // ckb_toolEnable
            // 
            this.ckb_toolEnable.AutoSize = true;
            this.ckb_toolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_toolEnable.Location = new System.Drawing.Point(506, 34);
            this.ckb_toolEnable.Name = "ckb_toolEnable";
            this.ckb_toolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_toolEnable.TabIndex = 79;
            this.ckb_toolEnable.Text = "启用";
            this.ckb_toolEnable.UseVisualStyleBackColor = true;
            this.ckb_toolEnable.CheckedChanged += new System.EventHandler(this.ckb_toolEnable_CheckedChanged);
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
            this.toolStrip1.Size = new System.Drawing.Size(490, 25);
            this.toolStrip1.TabIndex = 166;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_runTool
            // 
            this.tsb_runTool.AutoSize = false;
            this.tsb_runTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_runTool.Image = ((System.Drawing.Image)(resources.GetObject("tsb_runTool.Image")));
            this.tsb_runTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_runTool.Name = "tsb_runTool";
            this.tsb_runTool.RightToLeftAutoMirrorImage = true;
            this.tsb_runTool.Size = new System.Drawing.Size(25, 22);
            this.tsb_runTool.Text = "toolStripButton5";
            this.tsb_runTool.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsb_runTool.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_runTool.ToolTipText = "运行工具";
            this.tsb_runTool.Click += new System.EventHandler(this.tsb_runTool_Click);
            // 
            // tsb_runJob
            // 
            this.tsb_runJob.AutoSize = false;
            this.tsb_runJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_runJob.Image = ((System.Drawing.Image)(resources.GetObject("tsb_runJob.Image")));
            this.tsb_runJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_runJob.Name = "tsb_runJob";
            this.tsb_runJob.RightToLeftAutoMirrorImage = true;
            this.tsb_runJob.Size = new System.Drawing.Size(25, 22);
            this.tsb_runJob.Text = "toolStripButton5";
            this.tsb_runJob.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsb_runJob.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_runJob.ToolTipText = "运行流程";
            this.tsb_runJob.Click += new System.EventHandler(this.tsb_runJob_Click);
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
            this.tsb_resetTool.RightToLeftAutoMirrorImage = true;
            this.tsb_resetTool.Size = new System.Drawing.Size(25, 22);
            this.tsb_resetTool.Text = "toolStripButton4";
            this.tsb_resetTool.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsb_resetTool.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_resetTool.ToolTipText = "复位工具";
            this.tsb_resetTool.Click += new System.EventHandler(this.tsb_resetTool_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 67);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 17);
            this.label1.TabIndex = 176;
            this.label1.Text = "三点定圆求旋转中心";
            // 
            // dgv_data
            // 
            this.dgv_data.AllowUserToAddRows = false;
            this.dgv_data.AllowUserToDeleteRows = false;
            this.dgv_data.AllowUserToResizeColumns = false;
            this.dgv_data.AllowUserToResizeRows = false;
            this.dgv_data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_data.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column15,
            this.Column16});
            this.dgv_data.Location = new System.Drawing.Point(23, 88);
            this.dgv_data.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv_data.Name = "dgv_data";
            this.dgv_data.RowHeadersVisible = false;
            this.dgv_data.RowTemplate.Height = 23;
            this.dgv_data.Size = new System.Drawing.Size(230, 97);
            this.dgv_data.TabIndex = 175;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "编号";
            this.Column1.Name = "Column1";
            this.Column1.Width = 65;
            // 
            // Column15
            // 
            this.Column15.HeaderText = "像素X";
            this.Column15.Name = "Column15";
            this.Column15.Width = 80;
            // 
            // Column16
            // 
            this.Column16.HeaderText = "像素Y";
            this.Column16.Name = "Column16";
            this.Column16.Width = 80;
            // 
            // btn_runTool
            // 
            this.btn_runTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runTool.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runTool.Location = new System.Drawing.Point(452, 346);
            this.btn_runTool.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_runTool.Name = "btn_runTool";
            this.btn_runTool.Size = new System.Drawing.Size(98, 45);
            this.btn_runTool.TabIndex = 173;
            this.btn_runTool.Text = "运行";
            this.btn_runTool.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbx_rotateCenterX);
            this.groupBox2.Controls.Add(this.tbx_rotateCenterY);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(390, 124);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(159, 95);
            this.groupBox2.TabIndex = 174;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "旋转中心";
            // 
            // tbx_rotateCenterX
            // 
            this.tbx_rotateCenterX.Location = new System.Drawing.Point(52, 34);
            this.tbx_rotateCenterX.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_rotateCenterX.Name = "tbx_rotateCenterX";
            this.tbx_rotateCenterX.Size = new System.Drawing.Size(71, 23);
            this.tbx_rotateCenterX.TabIndex = 102;
            // 
            // tbx_rotateCenterY
            // 
            this.tbx_rotateCenterY.Location = new System.Drawing.Point(52, 62);
            this.tbx_rotateCenterY.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_rotateCenterY.Name = "tbx_rotateCenterY";
            this.tbx_rotateCenterY.Size = new System.Drawing.Size(71, 23);
            this.tbx_rotateCenterY.TabIndex = 104;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 37);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 17);
            this.label7.TabIndex = 103;
            this.label7.Text = "X：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(27, 65);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(18, 17);
            this.label9.TabIndex = 105;
            this.label9.Text = "Y:";
            // 
            // btn_thirdPoint
            // 
            this.btn_thirdPoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_thirdPoint.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_thirdPoint.Location = new System.Drawing.Point(260, 161);
            this.btn_thirdPoint.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_thirdPoint.Name = "btn_thirdPoint";
            this.btn_thirdPoint.Size = new System.Drawing.Size(49, 24);
            this.btn_thirdPoint.TabIndex = 179;
            this.btn_thirdPoint.Text = "获取";
            this.btn_thirdPoint.UseVisualStyleBackColor = true;
            this.btn_thirdPoint.Click += new System.EventHandler(this.btn_thirdPoint_Click);
            // 
            // btn_secondPoint
            // 
            this.btn_secondPoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_secondPoint.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_secondPoint.Location = new System.Drawing.Point(260, 138);
            this.btn_secondPoint.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_secondPoint.Name = "btn_secondPoint";
            this.btn_secondPoint.Size = new System.Drawing.Size(49, 24);
            this.btn_secondPoint.TabIndex = 178;
            this.btn_secondPoint.Text = "获取";
            this.btn_secondPoint.UseVisualStyleBackColor = true;
            this.btn_secondPoint.Click += new System.EventHandler(this.btn_secondPoint_Click);
            // 
            // btn_firstPoint
            // 
            this.btn_firstPoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_firstPoint.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_firstPoint.Location = new System.Drawing.Point(260, 115);
            this.btn_firstPoint.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_firstPoint.Name = "btn_firstPoint";
            this.btn_firstPoint.Size = new System.Drawing.Size(49, 24);
            this.btn_firstPoint.TabIndex = 177;
            this.btn_firstPoint.Text = "获取";
            this.btn_firstPoint.UseVisualStyleBackColor = true;
            this.btn_firstPoint.Click += new System.EventHandler(this.btn_firstPoint_Click);
            // 
            // btn_calculate
            // 
            this.btn_calculate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_calculate.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_calculate.Location = new System.Drawing.Point(390, 88);
            this.btn_calculate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_calculate.Name = "btn_calculate";
            this.btn_calculate.Size = new System.Drawing.Size(90, 30);
            this.btn_calculate.TabIndex = 180;
            this.btn_calculate.Text = "计算旋转中心";
            this.btn_calculate.UseVisualStyleBackColor = true;
            this.btn_calculate.Click += new System.EventHandler(this.btn_calculate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbx_inputPointX);
            this.groupBox1.Controls.Add(this.tbx_inputPointY);
            this.groupBox1.Controls.Add(this.tbx_inputPointU);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(23, 282);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(136, 114);
            this.groupBox1.TabIndex = 175;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "输入点";
            // 
            // tbx_inputPointX
            // 
            this.tbx_inputPointX.Location = new System.Drawing.Point(42, 30);
            this.tbx_inputPointX.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_inputPointX.Name = "tbx_inputPointX";
            this.tbx_inputPointX.ReadOnly = true;
            this.tbx_inputPointX.Size = new System.Drawing.Size(71, 23);
            this.tbx_inputPointX.TabIndex = 102;
            // 
            // tbx_inputPointY
            // 
            this.tbx_inputPointY.Location = new System.Drawing.Point(42, 55);
            this.tbx_inputPointY.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_inputPointY.Name = "tbx_inputPointY";
            this.tbx_inputPointY.ReadOnly = true;
            this.tbx_inputPointY.Size = new System.Drawing.Size(71, 23);
            this.tbx_inputPointY.TabIndex = 104;
            // 
            // tbx_inputPointU
            // 
            this.tbx_inputPointU.Location = new System.Drawing.Point(42, 80);
            this.tbx_inputPointU.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_inputPointU.Name = "tbx_inputPointU";
            this.tbx_inputPointU.ReadOnly = true;
            this.tbx_inputPointU.Size = new System.Drawing.Size(71, 23);
            this.tbx_inputPointU.TabIndex = 106;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 33);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 17);
            this.label2.TabIndex = 103;
            this.label2.Text = "X：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 83);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 17);
            this.label3.TabIndex = 107;
            this.label3.Text = "U:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 58);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 17);
            this.label4.TabIndex = 105;
            this.label4.Text = "Y:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbx_outputPointX);
            this.groupBox3.Controls.Add(this.tbx_outputPointY);
            this.groupBox3.Controls.Add(this.tbx_outputPointU);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(173, 282);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(136, 114);
            this.groupBox3.TabIndex = 175;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "输出点";
            // 
            // tbx_outputPointX
            // 
            this.tbx_outputPointX.Location = new System.Drawing.Point(45, 30);
            this.tbx_outputPointX.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_outputPointX.Name = "tbx_outputPointX";
            this.tbx_outputPointX.ReadOnly = true;
            this.tbx_outputPointX.Size = new System.Drawing.Size(71, 23);
            this.tbx_outputPointX.TabIndex = 102;
            // 
            // tbx_outputPointY
            // 
            this.tbx_outputPointY.Location = new System.Drawing.Point(45, 55);
            this.tbx_outputPointY.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_outputPointY.Name = "tbx_outputPointY";
            this.tbx_outputPointY.ReadOnly = true;
            this.tbx_outputPointY.Size = new System.Drawing.Size(71, 23);
            this.tbx_outputPointY.TabIndex = 104;
            // 
            // tbx_outputPointU
            // 
            this.tbx_outputPointU.Location = new System.Drawing.Point(45, 80);
            this.tbx_outputPointU.Margin = new System.Windows.Forms.Padding(2);
            this.tbx_outputPointU.Name = "tbx_outputPointU";
            this.tbx_outputPointU.ReadOnly = true;
            this.tbx_outputPointU.Size = new System.Drawing.Size(71, 23);
            this.tbx_outputPointU.TabIndex = 106;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 33);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 17);
            this.label5.TabIndex = 103;
            this.label5.Text = "X：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 83);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 17);
            this.label6.TabIndex = 107;
            this.label6.Text = "U:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 58);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 17);
            this.label8.TabIndex = 105;
            this.label8.Text = "Y:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.cbx_outputItemList);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.cbx_jobList);
            this.groupBox4.Location = new System.Drawing.Point(23, 191);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(286, 86);
            this.groupBox4.TabIndex = 181;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "特征点指定";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(6, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 17);
            this.label10.TabIndex = 92;
            this.label10.Text = "流程：";
            // 
            // cbx_outputItemList
            // 
            this.cbx_outputItemList.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.cbx_outputItemList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_outputItemList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbx_outputItemList.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_outputItemList.FormattingEnabled = true;
            this.cbx_outputItemList.Items.AddRange(new object[] {
            "四点标定",
            "九点标定"});
            this.cbx_outputItemList.Location = new System.Drawing.Point(55, 53);
            this.cbx_outputItemList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_outputItemList.Name = "cbx_outputItemList";
            this.cbx_outputItemList.Size = new System.Drawing.Size(215, 25);
            this.cbx_outputItemList.TabIndex = 93;
            this.cbx_outputItemList.SelectedIndexChanged += new System.EventHandler(this.cbx_outputItemList_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(6, 56);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 17);
            this.label11.TabIndex = 94;
            this.label11.Text = "输出项：";
            // 
            // cbx_jobList
            // 
            this.cbx_jobList.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.cbx_jobList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_jobList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbx_jobList.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_jobList.FormattingEnabled = true;
            this.cbx_jobList.Items.AddRange(new object[] {
            "四点标定",
            "九点标定"});
            this.cbx_jobList.Location = new System.Drawing.Point(55, 25);
            this.cbx_jobList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_jobList.Name = "cbx_jobList";
            this.cbx_jobList.Size = new System.Drawing.Size(215, 25);
            this.cbx_jobList.TabIndex = 91;
            this.cbx_jobList.SelectedIndexChanged += new System.EventHandler(this.cbx_jobList_SelectedIndexChanged);
            // 
            // Frm_RotatePlatformTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(566, 420);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_calculate);
            this.Controls.Add(this.btn_thirdPoint);
            this.Controls.Add(this.btn_secondPoint);
            this.Controls.Add(this.btn_firstPoint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgv_data);
            this.Controls.Add(this.btn_runTool);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.ckb_toolEnable);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(582, 449);
            this.Name = "Frm_RotatePlatformTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查找线";
            this.Load += new System.EventHandler(this.Frm_RotatePlatformTool_Load);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.ckb_toolEnable, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.btn_runTool, 0);
            this.Controls.SetChildIndex(this.dgv_data, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.btn_firstPoint, 0);
            this.Controls.SetChildIndex(this.btn_secondPoint, 0);
            this.Controls.SetChildIndex(this.btn_thirdPoint, 0);
            this.Controls.SetChildIndex(this.btn_calculate, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_data)).EndInit();
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
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_runJob;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.DataGridView dgv_data;
        public System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.TextBox tbx_rotateCenterX;
        public System.Windows.Forms.TextBox tbx_rotateCenterY;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_thirdPoint;
        private System.Windows.Forms.Button btn_secondPoint;
        private System.Windows.Forms.Button btn_firstPoint;
        public System.Windows.Forms.Button btn_calculate;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.TextBox tbx_inputPointX;
        public System.Windows.Forms.TextBox tbx_inputPointY;
        public System.Windows.Forms.TextBox tbx_inputPointU;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.TextBox tbx_outputPointX;
        public System.Windows.Forms.TextBox tbx_outputPointY;
        public System.Windows.Forms.TextBox tbx_outputPointU;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label10;
        internal System.Windows.Forms.ComboBox cbx_outputItemList;
        private System.Windows.Forms.Label label11;
        internal System.Windows.Forms.ComboBox cbx_jobList;
    }
}