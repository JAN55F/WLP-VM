namespace VMPro
{
    partial class Frm_BarcodeTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_BarcodeTool));
            this.lbl_resultCount = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbx_minContrast = new System.Windows.Forms.TextBox();
            this.nud_findCount = new System.Windows.Forms.NumericUpDown();
            this.label44 = new System.Windows.Forms.Label();
            this.dgv_barcordFindResult = new System.Windows.Forms.DataGridView();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_runFindBarcodeTool = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.btn_barcodeSearchRegionRectangle1 = new System.Windows.Forms.Button();
            this.rdo_barcodeSearchRegionEllipse = new System.Windows.Forms.RadioButton();
            this.rdo_barcodeSearchRegionCircle = new System.Windows.Forms.RadioButton();
            this.rdo_barcodeSearchRegionRectangle2 = new System.Windows.Forms.RadioButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel13 = new System.Windows.Forms.Panel();
            this.rdo_notDisplayArrow = new System.Windows.Forms.RadioButton();
            this.rdo_showArrow = new System.Windows.Forms.RadioButton();
            this.label17 = new System.Windows.Forms.Label();
            this.panel14 = new System.Windows.Forms.Panel();
            this.rdo_notDisplayResultStr = new System.Windows.Forms.RadioButton();
            this.rdo_showResultStr = new System.Windows.Forms.RadioButton();
            this.label16 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_help = new System.Windows.Forms.ToolStripButton();
            this.ckb_barcodeToolEnable = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_findCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_barcordFindResult)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel14.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_resultCount
            // 
            this.lbl_resultCount.AutoSize = true;
            this.lbl_resultCount.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_resultCount.Location = new System.Drawing.Point(64, 32);
            this.lbl_resultCount.Name = "lbl_resultCount";
            this.lbl_resultCount.Size = new System.Drawing.Size(25, 28);
            this.lbl_resultCount.TabIndex = 84;
            this.lbl_resultCount.Text = "0";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(7, 39);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(68, 17);
            this.label37.TabIndex = 83;
            this.label37.Text = "结果数量：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 188);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 17);
            this.label7.TabIndex = 66;
            this.label7.Text = "最小对比度：";
            // 
            // tbx_minContrast
            // 
            this.tbx_minContrast.Location = new System.Drawing.Point(102, 186);
            this.tbx_minContrast.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_minContrast.Name = "tbx_minContrast";
            this.tbx_minContrast.Size = new System.Drawing.Size(96, 23);
            this.tbx_minContrast.TabIndex = 67;
            this.tbx_minContrast.Text = "120";
            this.tbx_minContrast.TextChanged += new System.EventHandler(this.tbx_minContrast_TextChanged);
            // 
            // nud_findCount
            // 
            this.nud_findCount.Location = new System.Drawing.Point(102, 158);
            this.nud_findCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nud_findCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nud_findCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_findCount.Name = "nud_findCount";
            this.nud_findCount.Size = new System.Drawing.Size(95, 23);
            this.nud_findCount.TabIndex = 25;
            this.nud_findCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_findCount.ValueChanged += new System.EventHandler(this.nud_findCount_ValueChanged);
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(25, 160);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(68, 17);
            this.label44.TabIndex = 24;
            this.label44.Text = "查找数量：";
            // 
            // dgv_barcordFindResult
            // 
            this.dgv_barcordFindResult.AllowUserToAddRows = false;
            this.dgv_barcordFindResult.AllowUserToDeleteRows = false;
            this.dgv_barcordFindResult.AllowUserToResizeRows = false;
            this.dgv_barcordFindResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_barcordFindResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column4,
            this.Column11,
            this.Column5,
            this.Column2,
            this.Column3,
            this.Column6});
            this.dgv_barcordFindResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgv_barcordFindResult.Location = new System.Drawing.Point(3, 37);
            this.dgv_barcordFindResult.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv_barcordFindResult.Name = "dgv_barcordFindResult";
            this.dgv_barcordFindResult.ReadOnly = true;
            this.dgv_barcordFindResult.RowHeadersVisible = false;
            this.dgv_barcordFindResult.RowTemplate.Height = 23;
            this.dgv_barcordFindResult.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_barcordFindResult.Size = new System.Drawing.Size(542, 301);
            this.dgv_barcordFindResult.TabIndex = 81;
            this.dgv_barcordFindResult.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_barcordFindResult_CellClick);
            // 
            // Column4
            // 
            this.Column4.HeaderText = "解码字符串";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 250;
            // 
            // Column11
            // 
            this.Column11.HeaderText = "长度";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            this.Column11.Width = 65;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "条码类型";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 140;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "行";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 65;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "列";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 65;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "角度";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 65;
            // 
            // btn_runFindBarcodeTool
            // 
            this.btn_runFindBarcodeTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runFindBarcodeTool.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runFindBarcodeTool.Location = new System.Drawing.Point(418, 287);
            this.btn_runFindBarcodeTool.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_runFindBarcodeTool.Name = "btn_runFindBarcodeTool";
            this.btn_runFindBarcodeTool.Size = new System.Drawing.Size(98, 45);
            this.btn_runFindBarcodeTool.TabIndex = 80;
            this.btn_runFindBarcodeTool.Text = "运行";
            this.btn_runFindBarcodeTool.UseVisualStyleBackColor = true;
            this.btn_runFindBarcodeTool.Click += new System.EventHandler(this.btn_runFindBarcodeTool_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(3, 66);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(556, 371);
            this.tabControl1.TabIndex = 90;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbx_minContrast);
            this.tabPage1.Controls.Add(this.groupBox10);
            this.tabPage1.Controls.Add(this.label44);
            this.tabPage1.Controls.Add(this.nud_findCount);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.btn_runFindBarcodeTool);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(548, 341);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "参数与结果";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.radioButton1);
            this.groupBox10.Controls.Add(this.btn_barcodeSearchRegionRectangle1);
            this.groupBox10.Controls.Add(this.rdo_barcodeSearchRegionEllipse);
            this.groupBox10.Controls.Add(this.rdo_barcodeSearchRegionCircle);
            this.groupBox10.Controls.Add(this.rdo_barcodeSearchRegionRectangle2);
            this.groupBox10.Location = new System.Drawing.Point(28, 45);
            this.groupBox10.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox10.Size = new System.Drawing.Size(478, 83);
            this.groupBox10.TabIndex = 88;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "搜索范围";
            // 
            // radioButton1
            // 
            this.radioButton1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton1.Location = new System.Drawing.Point(365, 33);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(80, 36);
            this.radioButton1.TabIndex = 74;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "任意";
            this.radioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // btn_barcodeSearchRegionRectangle1
            // 
            this.btn_barcodeSearchRegionRectangle1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_barcodeSearchRegionRectangle1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_barcodeSearchRegionRectangle1.Location = new System.Drawing.Point(33, 33);
            this.btn_barcodeSearchRegionRectangle1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_barcodeSearchRegionRectangle1.Name = "btn_barcodeSearchRegionRectangle1";
            this.btn_barcodeSearchRegionRectangle1.Size = new System.Drawing.Size(80, 36);
            this.btn_barcodeSearchRegionRectangle1.TabIndex = 73;
            this.btn_barcodeSearchRegionRectangle1.Text = "矩形";
            this.btn_barcodeSearchRegionRectangle1.UseVisualStyleBackColor = true;
            this.btn_barcodeSearchRegionRectangle1.Click += new System.EventHandler(this.btn_barcodeSearchRegionRectangle1_Click);
            // 
            // rdo_barcodeSearchRegionEllipse
            // 
            this.rdo_barcodeSearchRegionEllipse.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdo_barcodeSearchRegionEllipse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdo_barcodeSearchRegionEllipse.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdo_barcodeSearchRegionEllipse.Location = new System.Drawing.Point(282, 33);
            this.rdo_barcodeSearchRegionEllipse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rdo_barcodeSearchRegionEllipse.Name = "rdo_barcodeSearchRegionEllipse";
            this.rdo_barcodeSearchRegionEllipse.Size = new System.Drawing.Size(80, 36);
            this.rdo_barcodeSearchRegionEllipse.TabIndex = 70;
            this.rdo_barcodeSearchRegionEllipse.TabStop = true;
            this.rdo_barcodeSearchRegionEllipse.Text = "椭圆";
            this.rdo_barcodeSearchRegionEllipse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdo_barcodeSearchRegionEllipse.UseVisualStyleBackColor = true;
            this.rdo_barcodeSearchRegionEllipse.CheckedChanged += new System.EventHandler(this.rdo_barcodeSearchRegionEllipse_CheckedChanged);
            // 
            // rdo_barcodeSearchRegionCircle
            // 
            this.rdo_barcodeSearchRegionCircle.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdo_barcodeSearchRegionCircle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdo_barcodeSearchRegionCircle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdo_barcodeSearchRegionCircle.Location = new System.Drawing.Point(199, 33);
            this.rdo_barcodeSearchRegionCircle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rdo_barcodeSearchRegionCircle.Name = "rdo_barcodeSearchRegionCircle";
            this.rdo_barcodeSearchRegionCircle.Size = new System.Drawing.Size(80, 36);
            this.rdo_barcodeSearchRegionCircle.TabIndex = 68;
            this.rdo_barcodeSearchRegionCircle.TabStop = true;
            this.rdo_barcodeSearchRegionCircle.Text = "圆";
            this.rdo_barcodeSearchRegionCircle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdo_barcodeSearchRegionCircle.UseVisualStyleBackColor = true;
            this.rdo_barcodeSearchRegionCircle.CheckedChanged += new System.EventHandler(this.rdo_barcodeSearchRegionCircle_CheckedChanged);
            // 
            // rdo_barcodeSearchRegionRectangle2
            // 
            this.rdo_barcodeSearchRegionRectangle2.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdo_barcodeSearchRegionRectangle2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdo_barcodeSearchRegionRectangle2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdo_barcodeSearchRegionRectangle2.Location = new System.Drawing.Point(116, 33);
            this.rdo_barcodeSearchRegionRectangle2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rdo_barcodeSearchRegionRectangle2.Name = "rdo_barcodeSearchRegionRectangle2";
            this.rdo_barcodeSearchRegionRectangle2.Size = new System.Drawing.Size(80, 36);
            this.rdo_barcodeSearchRegionRectangle2.TabIndex = 69;
            this.rdo_barcodeSearchRegionRectangle2.TabStop = true;
            this.rdo_barcodeSearchRegionRectangle2.Text = "仿射矩形";
            this.rdo_barcodeSearchRegionRectangle2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdo_barcodeSearchRegionRectangle2.UseVisualStyleBackColor = true;
            this.rdo_barcodeSearchRegionRectangle2.CheckedChanged += new System.EventHandler(this.rdo_barcodeSearchRegionRectangle2_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel13);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.panel14);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(548, 341);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "显示设置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.rdo_notDisplayArrow);
            this.panel13.Controls.Add(this.rdo_showArrow);
            this.panel13.Location = new System.Drawing.Point(129, 48);
            this.panel13.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(157, 26);
            this.panel13.TabIndex = 127;
            // 
            // rdo_notDisplayArrow
            // 
            this.rdo_notDisplayArrow.AutoSize = true;
            this.rdo_notDisplayArrow.Location = new System.Drawing.Point(99, 29);
            this.rdo_notDisplayArrow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rdo_notDisplayArrow.Name = "rdo_notDisplayArrow";
            this.rdo_notDisplayArrow.Size = new System.Drawing.Size(38, 21);
            this.rdo_notDisplayArrow.TabIndex = 15;
            this.rdo_notDisplayArrow.Text = "否";
            this.rdo_notDisplayArrow.UseVisualStyleBackColor = true;
            // 
            // rdo_showArrow
            // 
            this.rdo_showArrow.AutoSize = true;
            this.rdo_showArrow.Checked = true;
            this.rdo_showArrow.Location = new System.Drawing.Point(27, 29);
            this.rdo_showArrow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rdo_showArrow.Name = "rdo_showArrow";
            this.rdo_showArrow.Size = new System.Drawing.Size(38, 21);
            this.rdo_showArrow.TabIndex = 14;
            this.rdo_showArrow.TabStop = true;
            this.rdo_showArrow.Text = "是";
            this.rdo_showArrow.UseVisualStyleBackColor = true;
            this.rdo_showArrow.CheckedChanged += new System.EventHandler(this.rdo_displayArrow_CheckedChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(25, 51);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(59, 17);
            this.label17.TabIndex = 126;
            this.label17.Text = "显示箭头:";
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.rdo_notDisplayResultStr);
            this.panel14.Controls.Add(this.rdo_showResultStr);
            this.panel14.Location = new System.Drawing.Point(129, 20);
            this.panel14.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(157, 24);
            this.panel14.TabIndex = 125;
            // 
            // rdo_notDisplayResultStr
            // 
            this.rdo_notDisplayResultStr.AutoSize = true;
            this.rdo_notDisplayResultStr.Location = new System.Drawing.Point(99, 29);
            this.rdo_notDisplayResultStr.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rdo_notDisplayResultStr.Name = "rdo_notDisplayResultStr";
            this.rdo_notDisplayResultStr.Size = new System.Drawing.Size(38, 21);
            this.rdo_notDisplayResultStr.TabIndex = 15;
            this.rdo_notDisplayResultStr.Text = "否";
            this.rdo_notDisplayResultStr.UseVisualStyleBackColor = true;
            // 
            // rdo_showResultStr
            // 
            this.rdo_showResultStr.AutoSize = true;
            this.rdo_showResultStr.Checked = true;
            this.rdo_showResultStr.Location = new System.Drawing.Point(27, 29);
            this.rdo_showResultStr.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rdo_showResultStr.Name = "rdo_showResultStr";
            this.rdo_showResultStr.Size = new System.Drawing.Size(38, 21);
            this.rdo_showResultStr.TabIndex = 14;
            this.rdo_showResultStr.TabStop = true;
            this.rdo_showResultStr.Text = "是";
            this.rdo_showResultStr.UseVisualStyleBackColor = true;
            this.rdo_showResultStr.CheckedChanged += new System.EventHandler(this.rdo_diplayResultStr_CheckedChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(25, 24);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(95, 17);
            this.label16.TabIndex = 124;
            this.label16.Text = "显示结果字符串:";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgv_barcordFindResult);
            this.tabPage3.Controls.Add(this.lbl_resultCount);
            this.tabPage3.Controls.Add(this.label37);
            this.tabPage3.Location = new System.Drawing.Point(4, 26);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(548, 341);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "结果";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_runTool,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.tsb_resetTool,
            this.tsb_help});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(4, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.toolStrip1.Size = new System.Drawing.Size(484, 25);
            this.toolStrip1.TabIndex = 91;
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
            // toolStripButton1
            // 
            this.toolStripButton1.AutoSize = false;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(25, 22);
            this.toolStripButton1.Text = "toolStripButton5";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripButton1.ToolTipText = "运行流程";
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
            // tsb_help
            // 
            this.tsb_help.AutoSize = false;
            this.tsb_help.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_help.Image = ((System.Drawing.Image)(resources.GetObject("tsb_help.Image")));
            this.tsb_help.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_help.Name = "tsb_help";
            this.tsb_help.Size = new System.Drawing.Size(25, 22);
            this.tsb_help.Text = "toolStripButton7";
            this.tsb_help.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_help.ToolTipText = "帮助";
            this.tsb_help.Click += new System.EventHandler(this.tsb_help_Click);
            // 
            // ckb_barcodeToolEnable
            // 
            this.ckb_barcodeToolEnable.AutoSize = true;
            this.ckb_barcodeToolEnable.BackColor = System.Drawing.Color.Transparent;
            this.ckb_barcodeToolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_barcodeToolEnable.Location = new System.Drawing.Point(503, 34);
            this.ckb_barcodeToolEnable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_barcodeToolEnable.Name = "ckb_barcodeToolEnable";
            this.ckb_barcodeToolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_barcodeToolEnable.TabIndex = 92;
            this.ckb_barcodeToolEnable.Text = "启用";
            this.ckb_barcodeToolEnable.UseVisualStyleBackColor = false;
            this.ckb_barcodeToolEnable.CheckedChanged += new System.EventHandler(this.ckb_barcodeToolEnable_CheckedChanged);
            // 
            // Frm_BarcodeTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(562, 450);
            this.Controls.Add(this.ckb_barcodeToolEnable);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(562, 450);
            this.MinimumSize = new System.Drawing.Size(562, 450);
            this.Name = "Frm_BarcodeTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "条码";
            this.Controls.SetChildIndex(this.tabControl1, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.ckb_barcodeToolEnable, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_findCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_barcordFindResult)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel13.ResumeLayout(false);
            this.panel13.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label lbl_resultCount;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.TextBox tbx_minContrast;
        internal System.Windows.Forms.NumericUpDown nud_findCount;
        private System.Windows.Forms.Label label44;
        internal System.Windows.Forms.DataGridView dgv_barcordFindResult;
        internal System.Windows.Forms.Button btn_runFindBarcodeTool;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Button btn_barcodeSearchRegionRectangle1;
        private System.Windows.Forms.RadioButton rdo_barcodeSearchRegionEllipse;
        private System.Windows.Forms.RadioButton rdo_barcodeSearchRegionCircle;
        private System.Windows.Forms.RadioButton rdo_barcodeSearchRegionRectangle2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.RadioButton rdo_notDisplayArrow;
        internal System.Windows.Forms.RadioButton rdo_showArrow;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.RadioButton rdo_notDisplayResultStr;
        internal System.Windows.Forms.RadioButton rdo_showResultStr;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_help;
        private System.Windows.Forms.TabPage tabPage3;
        public System.Windows.Forms.CheckBox ckb_barcodeToolEnable;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}