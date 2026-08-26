using Controls;
namespace VMPro
{
    partial class Frm_PointOffsetTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_PointOffsetTool));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbx_caputurePosY = new CNumeric();
            this.button5 = new System.Windows.Forms.Button();
            this.tbx_caputurePosX = new CNumeric();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbx_pickPosY = new CNumeric();
            this.tbx_pickPosOffsetY = new CNumericUpDown();
            this.tbx_pickPosX = new CNumeric();
            this.tbx_pickPosOffsetX = new CNumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tbx_inputPosX = new System.Windows.Forms.TextBox();
            this.tbx_inputPosY = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.tbx_resultPosX = new System.Windows.Forms.TextBox();
            this.tbx_resultPosY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label19 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.btn_confirm = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btn_runTool = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comboBox1 = new CComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            // 
            // button100
            // 
            this.button100.FlatAppearance.BorderSize = 0;
            this.button100.Location = new System.Drawing.Point(542, 0);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbx_caputurePosY);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.tbx_caputurePosX);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(22, 46);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(180, 136);
            this.groupBox2.TabIndex = 95;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "模板特征点机械坐标";
            // 
            // tbx_caputurePosY
            // 
            this.tbx_caputurePosY.BackColor = System.Drawing.Color.White;
            this.tbx_caputurePosY.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_caputurePosY.Location = new System.Drawing.Point(39, 67);
            this.tbx_caputurePosY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_caputurePosY.Name = "tbx_caputurePosY";
            this.tbx_caputurePosY.Size = new System.Drawing.Size(67, 22);
            this.tbx_caputurePosY.TabIndex = 110;
            this.tbx_caputurePosY.Value = "";
            this.tbx_caputurePosY.Leave += new System.EventHandler(this.tbx_caputurePosY_Leave);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button5.BackgroundImage")));
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Location = new System.Drawing.Point(109, 100);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(65, 30);
            this.button5.TabIndex = 114;
            this.button5.Text = "自动获取";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            this.button5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.button5.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.button5.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.button5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // tbx_caputurePosX
            // 
            this.tbx_caputurePosX.BackColor = System.Drawing.Color.White;
            this.tbx_caputurePosX.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_caputurePosX.Location = new System.Drawing.Point(39, 43);
            this.tbx_caputurePosX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_caputurePosX.Name = "tbx_caputurePosX";
            this.tbx_caputurePosX.Size = new System.Drawing.Size(67, 22);
            this.tbx_caputurePosX.TabIndex = 109;
            this.tbx_caputurePosX.Value = "";
            this.tbx_caputurePosX.Leave += new System.EventHandler(this.tbx_caputurePosX_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 17);
            this.label2.TabIndex = 77;
            this.label2.Text = "X：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 17);
            this.label3.TabIndex = 78;
            this.label3.Text = "Y：";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.White;
            this.groupBox3.Controls.Add(this.tbx_pickPosY);
            this.groupBox3.Controls.Add(this.tbx_pickPosOffsetY);
            this.groupBox3.Controls.Add(this.tbx_pickPosX);
            this.groupBox3.Controls.Add(this.tbx_pickPosOffsetX);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(219, 46);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(212, 136);
            this.groupBox3.TabIndex = 105;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "工作点位置坐标";
            // 
            // tbx_pickPosY
            // 
            this.tbx_pickPosY.BackColor = System.Drawing.Color.White;
            this.tbx_pickPosY.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_pickPosY.Location = new System.Drawing.Point(36, 70);
            this.tbx_pickPosY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_pickPosY.Name = "tbx_pickPosY";
            this.tbx_pickPosY.Size = new System.Drawing.Size(67, 22);
            this.tbx_pickPosY.TabIndex = 112;
            this.tbx_pickPosY.Value = "";
            this.tbx_pickPosY.Load += new System.EventHandler(this.tbx_pickPosY_Load);
            this.tbx_pickPosY.Leave += new System.EventHandler(this.tbx_pickPosY_Leave);
            // 
            // tbx_pickPosOffsetY
            // 
            this.tbx_pickPosOffsetY.BackColor = System.Drawing.Color.White;
            this.tbx_pickPosOffsetY.DecimalPlaces = 3;
            this.tbx_pickPosOffsetY.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_pickPosOffsetY.Incremeent = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.tbx_pickPosOffsetY.Location = new System.Drawing.Point(109, 66);
            this.tbx_pickPosOffsetY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_pickPosOffsetY.MaximumSize = new System.Drawing.Size(300, 25);
            this.tbx_pickPosOffsetY.MaxValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.tbx_pickPosOffsetY.MinimumSize = new System.Drawing.Size(50, 25);
            this.tbx_pickPosOffsetY.MinValue = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.tbx_pickPosOffsetY.Name = "tbx_pickPosOffsetY";
            this.tbx_pickPosOffsetY.Size = new System.Drawing.Size(91, 25);
            this.tbx_pickPosOffsetY.TabIndex = 110;
            this.tbx_pickPosOffsetY.Value = 0D;
            this.tbx_pickPosOffsetY.Leave += new System.EventHandler(this.tbx_pickPosOffsetY_Leave);
            // 
            // tbx_pickPosX
            // 
            this.tbx_pickPosX.BackColor = System.Drawing.Color.White;
            this.tbx_pickPosX.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_pickPosX.Location = new System.Drawing.Point(36, 43);
            this.tbx_pickPosX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_pickPosX.Name = "tbx_pickPosX";
            this.tbx_pickPosX.Size = new System.Drawing.Size(67, 22);
            this.tbx_pickPosX.TabIndex = 111;
            this.tbx_pickPosX.Value = "";
            this.tbx_pickPosX.Leave += new System.EventHandler(this.tbx_pickPosX_Leave);
            // 
            // tbx_pickPosOffsetX
            // 
            this.tbx_pickPosOffsetX.BackColor = System.Drawing.Color.White;
            this.tbx_pickPosOffsetX.DecimalPlaces = 3;
            this.tbx_pickPosOffsetX.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_pickPosOffsetX.Incremeent = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.tbx_pickPosOffsetX.Location = new System.Drawing.Point(109, 39);
            this.tbx_pickPosOffsetX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_pickPosOffsetX.MaximumSize = new System.Drawing.Size(300, 25);
            this.tbx_pickPosOffsetX.MaxValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.tbx_pickPosOffsetX.MinimumSize = new System.Drawing.Size(50, 25);
            this.tbx_pickPosOffsetX.MinValue = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.tbx_pickPosOffsetX.Name = "tbx_pickPosOffsetX";
            this.tbx_pickPosOffsetX.Size = new System.Drawing.Size(91, 25);
            this.tbx_pickPosOffsetX.TabIndex = 109;
            this.tbx_pickPosOffsetX.Value = 0D;
            this.tbx_pickPosOffsetX.Leave += new System.EventHandler(this.tbx_pickPosOffsetX_Leave);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(131, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 17);
            this.label10.TabIndex = 83;
            this.label10.Text = "补偿量";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 17);
            this.label6.TabIndex = 77;
            this.label6.Text = "X：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 17);
            this.label7.TabIndex = 78;
            this.label7.Text = "Y：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 88;
            this.label1.Text = "点编号：";
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.Color.White;
            this.groupBox6.Controls.Add(this.tbx_inputPosX);
            this.groupBox6.Controls.Add(this.tbx_inputPosY);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Location = new System.Drawing.Point(22, 208);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(180, 123);
            this.groupBox6.TabIndex = 107;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "本次定位特征点机械坐标";
            // 
            // tbx_inputPosX
            // 
            this.tbx_inputPosX.BackColor = System.Drawing.Color.White;
            this.tbx_inputPosX.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbx_inputPosX.Location = new System.Drawing.Point(35, 33);
            this.tbx_inputPosX.Name = "tbx_inputPosX";
            this.tbx_inputPosX.ReadOnly = true;
            this.tbx_inputPosX.Size = new System.Drawing.Size(67, 16);
            this.tbx_inputPosX.TabIndex = 74;
            // 
            // tbx_inputPosY
            // 
            this.tbx_inputPosY.BackColor = System.Drawing.Color.White;
            this.tbx_inputPosY.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbx_inputPosY.Location = new System.Drawing.Point(35, 57);
            this.tbx_inputPosY.Name = "tbx_inputPosY";
            this.tbx_inputPosY.ReadOnly = true;
            this.tbx_inputPosY.Size = new System.Drawing.Size(67, 16);
            this.tbx_inputPosY.TabIndex = 75;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 33);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(28, 17);
            this.label13.TabIndex = 80;
            this.label13.Text = "X：";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 57);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(27, 17);
            this.label14.TabIndex = 81;
            this.label14.Text = "Y：";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.White;
            this.groupBox4.Controls.Add(this.button6);
            this.groupBox4.Controls.Add(this.tbx_resultPosX);
            this.groupBox4.Controls.Add(this.tbx_resultPosY);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(219, 208);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(212, 123);
            this.groupBox4.TabIndex = 108;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "本次定位工作点机械坐标";
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button6.BackgroundImage")));
            this.button6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button6.FlatAppearance.BorderSize = 0;
            this.button6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.ForeColor = System.Drawing.Color.White;
            this.button6.Location = new System.Drawing.Point(130, 87);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(76, 30);
            this.button6.TabIndex = 115;
            this.button6.Text = "设为工作点";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // tbx_resultPosX
            // 
            this.tbx_resultPosX.BackColor = System.Drawing.Color.White;
            this.tbx_resultPosX.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbx_resultPosX.Location = new System.Drawing.Point(28, 33);
            this.tbx_resultPosX.Name = "tbx_resultPosX";
            this.tbx_resultPosX.ReadOnly = true;
            this.tbx_resultPosX.Size = new System.Drawing.Size(67, 16);
            this.tbx_resultPosX.TabIndex = 74;
            // 
            // tbx_resultPosY
            // 
            this.tbx_resultPosY.BackColor = System.Drawing.Color.White;
            this.tbx_resultPosY.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbx_resultPosY.Location = new System.Drawing.Point(28, 57);
            this.tbx_resultPosY.Name = "tbx_resultPosY";
            this.tbx_resultPosY.ReadOnly = true;
            this.tbx_resultPosY.Size = new System.Drawing.Size(67, 16);
            this.tbx_resultPosY.TabIndex = 75;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 17);
            this.label5.TabIndex = 77;
            this.label5.Text = "X：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 17);
            this.label4.TabIndex = 78;
            this.label4.Text = "Y：";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(642, 457);
            this.tableLayoutPanel1.TabIndex = 174;
            // 
            // toolStrip2
            // 
            this.toolStrip2.AutoSize = false;
            this.toolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton4});
            this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(642, 25);
            this.toolStrip2.TabIndex = 88;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.AutoSize = false;
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(25, 22);
            this.toolStripButton4.Text = "toolStripButton4";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripButton4.ToolTipText = "重置";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label19);
            this.panel6.Controls.Add(this.label24);
            this.panel6.Controls.Add(this.pictureBox8);
            this.panel6.Controls.Add(this.btn_confirm);
            this.panel6.Controls.Add(this.btn_cancel);
            this.panel6.Controls.Add(this.panel5);
            this.panel6.Controls.Add(this.btn_runTool);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 405);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(636, 49);
            this.panel6.TabIndex = 90;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(83, 13);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(68, 17);
            this.label19.TabIndex = 117;
            this.label19.Text = "耗时：0ms";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(83, 28);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(68, 17);
            this.label24.TabIndex = 116;
            this.label24.Text = "状态：成功";
            // 
            // pictureBox8
            // 
            this.pictureBox8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox8.Image = global::VMPro.Properties.Resources.开;
            this.pictureBox8.Location = new System.Drawing.Point(13, 18);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(50, 25);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox8.TabIndex = 109;
            this.pictureBox8.TabStop = false;
            this.pictureBox8.Click += new System.EventHandler(this.pictureBox8_Click);
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
            this.btn_confirm.Location = new System.Drawing.Point(458, 13);
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
            this.btn_cancel.Location = new System.Drawing.Point(557, 13);
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
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel5.Location = new System.Drawing.Point(7, 4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(621, 1);
            this.panel5.TabIndex = 112;
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
            this.btn_runTool.Location = new System.Drawing.Point(390, 13);
            this.btn_runTool.Name = "btn_runTool";
            this.btn_runTool.Size = new System.Drawing.Size(65, 30);
            this.btn_runTool.TabIndex = 113;
            this.btn_runTool.Text = "运行工具";
            this.btn_runTool.UseVisualStyleBackColor = true;
            this.btn_runTool.Click += new System.EventHandler(this.btn_runTool_Click);
            this.btn_runTool.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_runTool.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_runTool.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_runTool.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.comboBox1);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.groupBox6);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox4);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 371);
            this.panel2.TabIndex = 91;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 354);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(258, 17);
            this.label8.TabIndex = 109;
            this.label8.Text = "说明：以上坐标均为机械坐标，且单位均为mm";
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
            this.panel3.Size = new System.Drawing.Size(642, 457);
            this.panel3.TabIndex = 175;
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.White;
            this.comboBox1.CanEdit = false;
            this.comboBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.Items = new string[0];
            this.comboBox1.Location = new System.Drawing.Point(67, 11);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndex = -1;
            this.comboBox1.Size = new System.Drawing.Size(84, 26);
            this.comboBox1.TabIndex = 110;
            this.comboBox1.TextStr = "";
            // 
            // Frm_PointOffsetTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(646, 485);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(100, 60);
            this.Name = "Frm_PointOffsetTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "点线距离";
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox6;
        public System.Windows.Forms.TextBox tbx_inputPosX;
        public System.Windows.Forms.TextBox tbx_inputPosY;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.TextBox tbx_resultPosX;
        public System.Windows.Forms.TextBox tbx_resultPosY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label24;
        public System.Windows.Forms.PictureBox pictureBox8;
        internal System.Windows.Forms.Button btn_confirm;
        internal System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Panel panel5;
        internal System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        internal System.Windows.Forms.Button button5;
        internal System.Windows.Forms.Button button6;
        public CNumericUpDown tbx_pickPosOffsetY;
        public CNumericUpDown tbx_pickPosOffsetX;
        public CNumeric tbx_caputurePosX;
        public CNumeric tbx_pickPosY;
        public CNumeric tbx_pickPosX;
        public CNumeric tbx_caputurePosY;
        private System.Windows.Forms.Label label8;
        public CComboBox comboBox1;
    }
}