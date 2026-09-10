namespace VMPro
{
    partial class Frm_OneKeyEyeHandCalibTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_OneKeyEyeHandCalibTool));
            this.btn_oneKeyCalibrate = new System.Windows.Forms.Button();
            this.cbx_outputItemList = new System.Windows.Forms.ComboBox();
            this.cbx_jobList = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cbo_calibType = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbx_scaleY = new System.Windows.Forms.TextBox();
            this.tbx_theta = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbx_translateX = new System.Windows.Forms.TextBox();
            this.tbx_rotation = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbx_translateY = new System.Windows.Forms.TextBox();
            this.tbx_scaleX = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_writeCalibData = new System.Windows.Forms.Button();
            this.btn_readCalibData = new System.Windows.Forms.Button();
            this.dgv_calibData = new System.Windows.Forms.DataGridView();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label54 = new System.Windows.Forms.Label();
            this.ckb_toolEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_runJob = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_calibrate = new System.Windows.Forms.Button();
            this.btn_connectPara = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_calibData)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_oneKeyCalibrate
            // 
            this.btn_oneKeyCalibrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_oneKeyCalibrate.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_oneKeyCalibrate.Location = new System.Drawing.Point(506, 377);
            this.btn_oneKeyCalibrate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_oneKeyCalibrate.Name = "btn_oneKeyCalibrate";
            this.btn_oneKeyCalibrate.Size = new System.Drawing.Size(98, 45);
            this.btn_oneKeyCalibrate.TabIndex = 22;
            this.btn_oneKeyCalibrate.Text = "一键标定";
            this.btn_oneKeyCalibrate.UseVisualStyleBackColor = true;
            this.btn_oneKeyCalibrate.Click += new System.EventHandler(this.btn_oneKeyCalibrate_Click);
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
            this.cbx_outputItemList.Location = new System.Drawing.Point(55, 54);
            this.cbx_outputItemList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_outputItemList.Name = "cbx_outputItemList";
            this.cbx_outputItemList.Size = new System.Drawing.Size(234, 25);
            this.cbx_outputItemList.TabIndex = 93;
            this.cbx_outputItemList.SelectedIndexChanged += new System.EventHandler(this.cbx_outputItemList_SelectedIndexChanged);
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
            this.cbx_jobList.Location = new System.Drawing.Point(55, 26);
            this.cbx_jobList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_jobList.Name = "cbx_jobList";
            this.cbx_jobList.Size = new System.Drawing.Size(234, 25);
            this.cbx_jobList.TabIndex = 91;
            this.cbx_jobList.SelectedIndexChanged += new System.EventHandler(this.cbx_jobList_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(6, 57);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 17);
            this.label10.TabIndex = 94;
            this.label10.Text = "输出项：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(6, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 17);
            this.label9.TabIndex = 92;
            this.label9.Text = "流程：";
            // 
            // cbo_calibType
            // 
            this.cbo_calibType.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.cbo_calibType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_calibType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbo_calibType.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbo_calibType.FormattingEnabled = true;
            this.cbo_calibType.Items.AddRange(new object[] {
            "四点标定",
            "九点标定"});
            this.cbo_calibType.Location = new System.Drawing.Point(73, 71);
            this.cbo_calibType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbo_calibType.Name = "cbo_calibType";
            this.cbo_calibType.Size = new System.Drawing.Size(251, 25);
            this.cbo_calibType.TabIndex = 20;
            this.cbo_calibType.SelectedIndexChanged += new System.EventHandler(this.cbo_calibType_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.tbx_scaleY);
            this.groupBox2.Controls.Add(this.tbx_theta);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tbx_translateX);
            this.groupBox2.Controls.Add(this.tbx_rotation);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tbx_translateY);
            this.groupBox2.Controls.Add(this.tbx_scaleX);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(400, 71);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(204, 216);
            this.groupBox2.TabIndex = 85;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "标定结果";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(165, 184);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 17);
            this.label7.TabIndex = 89;
            this.label7.Text = "度";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(165, 154);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(20, 17);
            this.label12.TabIndex = 88;
            this.label12.Text = "度";
            // 
            // tbx_scaleY
            // 
            this.tbx_scaleY.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_scaleY.Location = new System.Drawing.Point(74, 121);
            this.tbx_scaleY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_scaleY.Name = "tbx_scaleY";
            this.tbx_scaleY.ReadOnly = true;
            this.tbx_scaleY.Size = new System.Drawing.Size(88, 23);
            this.tbx_scaleY.TabIndex = 78;
            // 
            // tbx_theta
            // 
            this.tbx_theta.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_theta.Location = new System.Drawing.Point(74, 181);
            this.tbx_theta.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_theta.Name = "tbx_theta";
            this.tbx_theta.ReadOnly = true;
            this.tbx_theta.Size = new System.Drawing.Size(88, 23);
            this.tbx_theta.TabIndex = 84;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(19, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 17);
            this.label1.TabIndex = 73;
            this.label1.Text = "X平移：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(19, 184);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 83;
            this.label6.Text = "轴斜切：";
            // 
            // tbx_translateX
            // 
            this.tbx_translateX.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_translateX.Location = new System.Drawing.Point(74, 31);
            this.tbx_translateX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_translateX.Name = "tbx_translateX";
            this.tbx_translateX.ReadOnly = true;
            this.tbx_translateX.Size = new System.Drawing.Size(88, 23);
            this.tbx_translateX.TabIndex = 74;
            // 
            // tbx_rotation
            // 
            this.tbx_rotation.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_rotation.Location = new System.Drawing.Point(74, 151);
            this.tbx_rotation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_rotation.Name = "tbx_rotation";
            this.tbx_rotation.ReadOnly = true;
            this.tbx_rotation.Size = new System.Drawing.Size(88, 23);
            this.tbx_rotation.TabIndex = 82;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(19, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 17);
            this.label2.TabIndex = 75;
            this.label2.Text = "Y平移：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(19, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 17);
            this.label5.TabIndex = 81;
            this.label5.Text = "Y缩放：";
            // 
            // tbx_translateY
            // 
            this.tbx_translateY.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_translateY.Location = new System.Drawing.Point(74, 61);
            this.tbx_translateY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_translateY.Name = "tbx_translateY";
            this.tbx_translateY.ReadOnly = true;
            this.tbx_translateY.Size = new System.Drawing.Size(88, 23);
            this.tbx_translateY.TabIndex = 76;
            // 
            // tbx_scaleX
            // 
            this.tbx_scaleX.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_scaleX.Location = new System.Drawing.Point(74, 91);
            this.tbx_scaleX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_scaleX.Name = "tbx_scaleX";
            this.tbx_scaleX.ReadOnly = true;
            this.tbx_scaleX.Size = new System.Drawing.Size(88, 23);
            this.tbx_scaleX.TabIndex = 80;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(19, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 17);
            this.label3.TabIndex = 77;
            this.label3.Text = "X缩放：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(19, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 17);
            this.label4.TabIndex = 79;
            this.label4.Text = "旋转：";
            // 
            // btn_writeCalibData
            // 
            this.btn_writeCalibData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_writeCalibData.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_writeCalibData.Location = new System.Drawing.Point(481, 295);
            this.btn_writeCalibData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_writeCalibData.Name = "btn_writeCalibData";
            this.btn_writeCalibData.Size = new System.Drawing.Size(75, 26);
            this.btn_writeCalibData.TabIndex = 71;
            this.btn_writeCalibData.Text = "数据导出";
            this.btn_writeCalibData.UseVisualStyleBackColor = true;
            this.btn_writeCalibData.Click += new System.EventHandler(this.btn_writeCalibData_Click);
            // 
            // btn_readCalibData
            // 
            this.btn_readCalibData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_readCalibData.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_readCalibData.Location = new System.Drawing.Point(400, 295);
            this.btn_readCalibData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_readCalibData.Name = "btn_readCalibData";
            this.btn_readCalibData.Size = new System.Drawing.Size(75, 26);
            this.btn_readCalibData.TabIndex = 72;
            this.btn_readCalibData.Text = "数据导入";
            this.btn_readCalibData.UseVisualStyleBackColor = true;
            this.btn_readCalibData.Click += new System.EventHandler(this.btn_readCalibData_Click);
            // 
            // dgv_calibData
            // 
            this.dgv_calibData.AllowUserToAddRows = false;
            this.dgv_calibData.AllowUserToDeleteRows = false;
            this.dgv_calibData.AllowUserToResizeColumns = false;
            this.dgv_calibData.AllowUserToResizeRows = false;
            this.dgv_calibData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_calibData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column15,
            this.Column16,
            this.Column13,
            this.Column14});
            this.dgv_calibData.Location = new System.Drawing.Point(14, 104);
            this.dgv_calibData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv_calibData.Name = "dgv_calibData";
            this.dgv_calibData.RowHeadersVisible = false;
            this.dgv_calibData.RowTemplate.Height = 23;
            this.dgv_calibData.Size = new System.Drawing.Size(310, 234);
            this.dgv_calibData.TabIndex = 16;
            // 
            // Column15
            // 
            this.Column15.HeaderText = "像素X";
            this.Column15.Name = "Column15";
            this.Column15.Width = 77;
            // 
            // Column16
            // 
            this.Column16.HeaderText = "像素Y";
            this.Column16.Name = "Column16";
            this.Column16.Width = 77;
            // 
            // Column13
            // 
            this.Column13.HeaderText = "机械X";
            this.Column13.Name = "Column13";
            this.Column13.Width = 77;
            // 
            // Column14
            // 
            this.Column14.HeaderText = "机械Y";
            this.Column14.Name = "Column14";
            this.Column14.Width = 76;
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label54.Location = new System.Drawing.Point(9, 74);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(68, 17);
            this.label54.TabIndex = 19;
            this.label54.Text = "标定类型：";
            // 
            // ckb_toolEnable
            // 
            this.ckb_toolEnable.AutoSize = true;
            this.ckb_toolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_toolEnable.Location = new System.Drawing.Point(559, 31);
            this.ckb_toolEnable.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.ckb_toolEnable.Name = "ckb_toolEnable";
            this.ckb_toolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_toolEnable.TabIndex = 70;
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
            this.toolStrip1.Size = new System.Drawing.Size(547, 25);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cbx_outputItemList);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.cbx_jobList);
            this.groupBox1.Location = new System.Drawing.Point(15, 345);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 86);
            this.groupBox1.TabIndex = 95;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "特征点指定";
            // 
            // btn_calibrate
            // 
            this.btn_calibrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_calibrate.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_calibrate.Location = new System.Drawing.Point(481, 326);
            this.btn_calibrate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_calibrate.Name = "btn_calibrate";
            this.btn_calibrate.Size = new System.Drawing.Size(75, 28);
            this.btn_calibrate.TabIndex = 96;
            this.btn_calibrate.Text = "手动标定";
            this.btn_calibrate.UseVisualStyleBackColor = true;
            this.btn_calibrate.Click += new System.EventHandler(this.btn_calibrate_Click);
            // 
            // btn_connectPara
            // 
            this.btn_connectPara.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_connectPara.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_connectPara.Location = new System.Drawing.Point(400, 326);
            this.btn_connectPara.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_connectPara.Name = "btn_connectPara";
            this.btn_connectPara.Size = new System.Drawing.Size(75, 28);
            this.btn_connectPara.TabIndex = 97;
            this.btn_connectPara.Text = "连接设置";
            this.btn_connectPara.UseVisualStyleBackColor = true;
            this.btn_connectPara.Click += new System.EventHandler(this.btn_connectPara_Click);
            // 
            // Frm_OneKeyEyeHandCalibTool
            // 
            this.AcceptButton = this.btn_oneKeyCalibrate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(619, 448);
            this.Controls.Add(this.btn_connectPara);
            this.Controls.Add(this.btn_calibrate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbo_calibType);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btn_writeCalibData);
            this.Controls.Add(this.btn_readCalibData);
            this.Controls.Add(this.dgv_calibData);
            this.Controls.Add(this.label54);
            this.Controls.Add(this.ckb_toolEnable);
            this.Controls.Add(this.btn_oneKeyCalibrate);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(565, 400);
            this.Name = "Frm_OneKeyEyeHandCalibTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "手眼标定";
            this.Load += new System.EventHandler(this.Frm_OneKeyEyeHandCalibTool_Load);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.btn_oneKeyCalibrate, 0);
            this.Controls.SetChildIndex(this.ckb_toolEnable, 0);
            this.Controls.SetChildIndex(this.label54, 0);
            this.Controls.SetChildIndex(this.dgv_calibData, 0);
            this.Controls.SetChildIndex(this.btn_readCalibData, 0);
            this.Controls.SetChildIndex(this.btn_writeCalibData, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.cbo_calibType, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.btn_calibrate, 0);
            this.Controls.SetChildIndex(this.btn_connectPara, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_calibData)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label54;
        internal System.Windows.Forms.CheckBox ckb_toolEnable;
        private System.Windows.Forms.Button btn_readCalibData;
        private System.Windows.Forms.Button btn_writeCalibData;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.TextBox tbx_scaleY;
        internal System.Windows.Forms.TextBox tbx_theta;
        internal System.Windows.Forms.TextBox tbx_translateX;
        internal System.Windows.Forms.TextBox tbx_rotation;
        internal System.Windows.Forms.TextBox tbx_translateY;
        internal System.Windows.Forms.TextBox tbx_scaleX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        internal System.Windows.Forms.ComboBox cbo_calibType;
        internal System.Windows.Forms.DataGridView dgv_calibData;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.ToolStripButton tsb_runJob;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ComboBox cbx_jobList;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        internal System.Windows.Forms.ComboBox cbx_outputItemList;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button btn_oneKeyCalibrate;
        public System.Windows.Forms.Button btn_calibrate;
        public System.Windows.Forms.Button btn_connectPara;
    }
}