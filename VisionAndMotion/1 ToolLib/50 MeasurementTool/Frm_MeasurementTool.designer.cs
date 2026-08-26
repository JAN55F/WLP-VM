using Controls;
namespace VMPro
{
    partial class Frm_MeasurementTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ShapeMatchTool));
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tbc_shapeMatch = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.button10 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.panel17 = new System.Windows.Forms.Panel();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.button9 = new System.Windows.Forms.Button();
            this.panel10 = new System.Windows.Forms.Panel();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_drawTemplateRegionRectangle2 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.btn_drawTemplateRegionCircle = new System.Windows.Forms.Button();
            this.btn_drawTemplateRegionEllipse = new System.Windows.Forms.Button();
            this.hwc_template = new HalconDotNet.HWindowControl();
            this.btn_drawTemplateRegionAny = new System.Windows.Forms.Button();
            this.btn_drawTemplateRegionRectangle1 = new System.Windows.Forms.Button();
            this.rdo_templateRegionSub = new System.Windows.Forms.RadioButton();
            this.rdo_templateRegionAdd = new System.Windows.Forms.RadioButton();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.ckb_autoContrast = new Controls.CCheckBox();
            this.numericUpDown2 = new Controls.CNumericUpDown();
            this.nud_angleRange = new Controls.CNumericUpDown();
            this.numericUpDown1 = new Controls.CNumericUpDown();
            this.nud_angleStart = new Controls.CNumericUpDown();
            this.comboBox1 = new Controls.CComboBox();
            this.cbx_polarity = new Controls.CComboBox();
            this.nud_minScore = new Controls.CNumericUpDown();
            this.numericUpDown4 = new Controls.CNumericUpDown();
            this.numericUpDown3 = new Controls.CNumericUpDown();
            this.nud_angleStep = new Controls.CNumericUpDown();
            this.nud_matchNum = new Controls.CNumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.panel14 = new System.Windows.Forms.Panel();
            this.panel15 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.tkb_contrast = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.lbl_contastValue = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label87 = new System.Windows.Forms.Label();
            this.label90 = new System.Windows.Forms.Label();
            this.label88 = new System.Windows.Forms.Label();
            this.ckb_autoStep = new System.Windows.Forms.CheckBox();
            this.label56 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label85 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.cbx_searchRegionType = new Controls.CComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.cbx_showTemplate = new System.Windows.Forms.CheckBox();
            this.ckb_showFeature = new System.Windows.Forms.CheckBox();
            this.ckb_showCross = new System.Windows.Forms.CheckBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dgv_matchResult = new System.Windows.Forms.DataGridView();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hWindow_Final1 = new ChoiceTech.Halcon.Control.HWindow_Final();
            this.cnt_rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.适应图像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全屏显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图像另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存窗口ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btn_confirm = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_runTool = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel5 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tbc_shapeMatch.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.panel17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tkb_contrast)).BeginInit();
            this.tabPage6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_matchResult)).BeginInit();
            this.cnt_rightClickMenu.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            // 
            // button100
            // 
            this.button100.FlatAppearance.BorderSize = 0;
            this.button100.Location = new System.Drawing.Point(803, 0);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Location = new System.Drawing.Point(2, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(903, 564);
            this.panel2.TabIndex = 116;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(903, 564);
            this.tableLayoutPanel1.TabIndex = 114;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripButton1,
            this.tsb_resetTool});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(903, 25);
            this.toolStrip1.TabIndex = 88;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.AutoSize = false;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(25, 22);
            this.toolStripButton2.Text = "toolStripButton4";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripButton2.ToolTipText = "训练图像";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click_1);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.AutoSize = false;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(25, 22);
            this.toolStripButton1.Text = "toolStripButton4";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripButton1.ToolTipText = "显示模板";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
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
            this.tsb_resetTool.ToolTipText = "重置";
            this.tsb_resetTool.Click += new System.EventHandler(this.tsb_resetTool_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.tableLayoutPanel2.Controls.Add(this.tbc_shapeMatch, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.hWindow_Final1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 28);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(897, 468);
            this.tableLayoutPanel2.TabIndex = 89;
            // 
            // tbc_shapeMatch
            // 
            this.tbc_shapeMatch.Controls.Add(this.tabPage5);
            this.tbc_shapeMatch.Controls.Add(this.tabPage4);
            this.tbc_shapeMatch.Controls.Add(this.tabPage6);
            this.tbc_shapeMatch.Controls.Add(this.tabPage2);
            this.tbc_shapeMatch.Controls.Add(this.tabPage1);
            this.tbc_shapeMatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbc_shapeMatch.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbc_shapeMatch.Location = new System.Drawing.Point(622, 4);
            this.tbc_shapeMatch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbc_shapeMatch.Name = "tbc_shapeMatch";
            this.tbc_shapeMatch.SelectedIndex = 0;
            this.tbc_shapeMatch.Size = new System.Drawing.Size(272, 460);
            this.tbc_shapeMatch.TabIndex = 5;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.button10);
            this.tabPage5.Controls.Add(this.button8);
            this.tabPage5.Controls.Add(this.button4);
            this.tabPage5.Controls.Add(this.button3);
            this.tabPage5.Controls.Add(this.button5);
            this.tabPage5.Controls.Add(this.panel17);
            this.tabPage5.Controls.Add(this.button9);
            this.tabPage5.Controls.Add(this.panel10);
            this.tabPage5.Controls.Add(this.pictureBox7);
            this.tabPage5.Controls.Add(this.pictureBox6);
            this.tabPage5.Controls.Add(this.panel9);
            this.tabPage5.Controls.Add(this.panel8);
            this.tabPage5.Controls.Add(this.panel7);
            this.tabPage5.Controls.Add(this.panel4);
            this.tabPage5.Controls.Add(this.panel3);
            this.tabPage5.Controls.Add(this.btn_drawTemplateRegionRectangle2);
            this.tabPage5.Controls.Add(this.button7);
            this.tabPage5.Controls.Add(this.button6);
            this.tabPage5.Controls.Add(this.btn_drawTemplateRegionCircle);
            this.tabPage5.Controls.Add(this.btn_drawTemplateRegionEllipse);
            this.tabPage5.Controls.Add(this.hwc_template);
            this.tabPage5.Controls.Add(this.btn_drawTemplateRegionAny);
            this.tabPage5.Controls.Add(this.btn_drawTemplateRegionRectangle1);
            this.tabPage5.Controls.Add(this.rdo_templateRegionSub);
            this.tabPage5.Controls.Add(this.rdo_templateRegionAdd);
            this.tabPage5.Location = new System.Drawing.Point(4, 26);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(264, 430);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "模板";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.button10.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button10.BackgroundImage")));
            this.button10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button10.FlatAppearance.BorderSize = 0;
            this.button10.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.button10.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.ForeColor = System.Drawing.Color.White;
            this.button10.Location = new System.Drawing.Point(74, 332);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(25, 25);
            this.button10.TabIndex = 140;
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.button8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button8.BackgroundImage")));
            this.button8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button8.FlatAppearance.BorderSize = 0;
            this.button8.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.button8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button8.ForeColor = System.Drawing.Color.White;
            this.button8.Location = new System.Drawing.Point(102, 332);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(25, 25);
            this.button8.TabIndex = 139;
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.button8_Click_1);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.button4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(46, 332);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(25, 25);
            this.button4.TabIndex = 138;
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click_4);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(18, 332);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(25, 25);
            this.button3.TabIndex = 137;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
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
            this.button5.Location = new System.Drawing.Point(156, 332);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(42, 25);
            this.button5.TabIndex = 136;
            this.button5.Text = "圆";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button4_Click_2);
            // 
            // panel17
            // 
            this.panel17.Controls.Add(this.radioButton4);
            this.panel17.Controls.Add(this.radioButton2);
            this.panel17.Controls.Add(this.radioButton1);
            this.panel17.Controls.Add(this.radioButton3);
            this.panel17.Location = new System.Drawing.Point(11, 373);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(240, 39);
            this.panel17.TabIndex = 135;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton4.Location = new System.Drawing.Point(68, 11);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(50, 21);
            this.radioButton4.TabIndex = 130;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "建模";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.Click += new System.EventHandler(this.radioButton4_Click);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton2.Location = new System.Drawing.Point(128, 11);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(50, 21);
            this.radioButton2.TabIndex = 128;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "涂抹";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            this.radioButton2.Click += new System.EventHandler(this.radioButton2_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.radioButton1.Location = new System.Drawing.Point(8, 11);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(50, 21);
            this.radioButton1.TabIndex = 127;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "正常";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            this.radioButton1.Click += new System.EventHandler(this.radioButton1_Click);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton3.Location = new System.Drawing.Point(188, 11);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(50, 21);
            this.radioButton3.TabIndex = 129;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "擦除";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            this.radioButton3.Click += new System.EventHandler(this.radioButton3_Click);
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.button9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button9.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button9.FlatAppearance.BorderSize = 0;
            this.button9.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.button9.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.ForeColor = System.Drawing.Color.White;
            this.button9.Location = new System.Drawing.Point(202, 332);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(42, 25);
            this.button9.TabIndex = 133;
            this.button9.Text = "矩形";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel10.Location = new System.Drawing.Point(26, 234);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(210, 2);
            this.panel10.TabIndex = 126;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.Location = new System.Drawing.Point(48, 212);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(20, 20);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 125;
            this.pictureBox7.TabStop = false;
            this.pictureBox7.Click += new System.EventHandler(this.pictureBox7_Click);
            // 
            // pictureBox6
            // 
            this.pictureBox6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox6.Image = global::VMPro.Properties.Resources.去勾选;
            this.pictureBox6.Location = new System.Drawing.Point(165, 212);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(20, 20);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 124;
            this.pictureBox6.TabStop = false;
            this.pictureBox6.Click += new System.EventHandler(this.pictureBox6_Click);
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel9.Location = new System.Drawing.Point(7, 422);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(248, 1);
            this.panel9.TabIndex = 122;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel8.Location = new System.Drawing.Point(254, 196);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(1, 227);
            this.panel8.TabIndex = 123;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel7.Location = new System.Drawing.Point(7, 196);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1, 227);
            this.panel7.TabIndex = 122;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel4.Controls.Add(this.label6);
            this.panel4.Location = new System.Drawing.Point(7, 294);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(248, 20);
            this.panel4.TabIndex = 121;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(3, 1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "涂抹与擦除";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(7, 176);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(248, 20);
            this.panel3.TabIndex = 120;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "模板绘制";
            // 
            // btn_drawTemplateRegionRectangle2
            // 
            this.btn_drawTemplateRegionRectangle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.btn_drawTemplateRegionRectangle2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_drawTemplateRegionRectangle2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_drawTemplateRegionRectangle2.FlatAppearance.BorderSize = 0;
            this.btn_drawTemplateRegionRectangle2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btn_drawTemplateRegionRectangle2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btn_drawTemplateRegionRectangle2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_drawTemplateRegionRectangle2.ForeColor = System.Drawing.Color.White;
            this.btn_drawTemplateRegionRectangle2.Location = new System.Drawing.Point(64, 249);
            this.btn_drawTemplateRegionRectangle2.Name = "btn_drawTemplateRegionRectangle2";
            this.btn_drawTemplateRegionRectangle2.Size = new System.Drawing.Size(42, 25);
            this.btn_drawTemplateRegionRectangle2.TabIndex = 11;
            this.btn_drawTemplateRegionRectangle2.TabStop = false;
            this.btn_drawTemplateRegionRectangle2.Text = "仿矩";
            this.btn_drawTemplateRegionRectangle2.UseVisualStyleBackColor = false;
            this.btn_drawTemplateRegionRectangle2.Click += new System.EventHandler(this.btn_drawTemplateRegionRectangle2_Click);
            // 
            // button7
            // 
            this.button7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button7.BackgroundImage")));
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button7.FlatAppearance.BorderSize = 0;
            this.button7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7.ForeColor = System.Drawing.Color.White;
            this.button7.Location = new System.Drawing.Point(186, 90);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(69, 30);
            this.button7.TabIndex = 119;
            this.button7.Text = "学习";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            this.button7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.button7.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.button7.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.button7.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // button6
            // 
            this.button6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button6.BackgroundImage")));
            this.button6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button6.FlatAppearance.BorderSize = 0;
            this.button6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.ForeColor = System.Drawing.Color.White;
            this.button6.Location = new System.Drawing.Point(186, 122);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(69, 30);
            this.button6.TabIndex = 118;
            this.button6.Text = "编辑";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            this.button6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.button6.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.button6.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.button6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // btn_drawTemplateRegionCircle
            // 
            this.btn_drawTemplateRegionCircle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.btn_drawTemplateRegionCircle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_drawTemplateRegionCircle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_drawTemplateRegionCircle.FlatAppearance.BorderSize = 0;
            this.btn_drawTemplateRegionCircle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btn_drawTemplateRegionCircle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btn_drawTemplateRegionCircle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_drawTemplateRegionCircle.ForeColor = System.Drawing.Color.White;
            this.btn_drawTemplateRegionCircle.Location = new System.Drawing.Point(110, 249);
            this.btn_drawTemplateRegionCircle.Name = "btn_drawTemplateRegionCircle";
            this.btn_drawTemplateRegionCircle.Size = new System.Drawing.Size(42, 25);
            this.btn_drawTemplateRegionCircle.TabIndex = 10;
            this.btn_drawTemplateRegionCircle.TabStop = false;
            this.btn_drawTemplateRegionCircle.Text = "圆";
            this.btn_drawTemplateRegionCircle.UseVisualStyleBackColor = false;
            this.btn_drawTemplateRegionCircle.Click += new System.EventHandler(this.btn_drawTemplateRegionCircle_Click);
            // 
            // btn_drawTemplateRegionEllipse
            // 
            this.btn_drawTemplateRegionEllipse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.btn_drawTemplateRegionEllipse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_drawTemplateRegionEllipse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_drawTemplateRegionEllipse.FlatAppearance.BorderSize = 0;
            this.btn_drawTemplateRegionEllipse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btn_drawTemplateRegionEllipse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btn_drawTemplateRegionEllipse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_drawTemplateRegionEllipse.ForeColor = System.Drawing.Color.White;
            this.btn_drawTemplateRegionEllipse.Location = new System.Drawing.Point(156, 249);
            this.btn_drawTemplateRegionEllipse.Name = "btn_drawTemplateRegionEllipse";
            this.btn_drawTemplateRegionEllipse.Size = new System.Drawing.Size(42, 25);
            this.btn_drawTemplateRegionEllipse.TabIndex = 8;
            this.btn_drawTemplateRegionEllipse.TabStop = false;
            this.btn_drawTemplateRegionEllipse.Text = "椭圆";
            this.btn_drawTemplateRegionEllipse.UseVisualStyleBackColor = false;
            this.btn_drawTemplateRegionEllipse.Click += new System.EventHandler(this.btn_drawTemplateRegionEllipse_Click);
            // 
            // hwc_template
            // 
            this.hwc_template.BackColor = System.Drawing.Color.Black;
            this.hwc_template.BorderColor = System.Drawing.Color.Black;
            this.hwc_template.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hwc_template.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hwc_template.Location = new System.Drawing.Point(7, 12);
            this.hwc_template.Margin = new System.Windows.Forms.Padding(2);
            this.hwc_template.Name = "hwc_template";
            this.hwc_template.Size = new System.Drawing.Size(175, 140);
            this.hwc_template.TabIndex = 70;
            this.hwc_template.Tag = "图像显示窗体";
            this.hwc_template.WindowSize = new System.Drawing.Size(175, 140);
            // 
            // btn_drawTemplateRegionAny
            // 
            this.btn_drawTemplateRegionAny.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.btn_drawTemplateRegionAny.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_drawTemplateRegionAny.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_drawTemplateRegionAny.FlatAppearance.BorderSize = 0;
            this.btn_drawTemplateRegionAny.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btn_drawTemplateRegionAny.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btn_drawTemplateRegionAny.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_drawTemplateRegionAny.ForeColor = System.Drawing.Color.White;
            this.btn_drawTemplateRegionAny.Location = new System.Drawing.Point(202, 249);
            this.btn_drawTemplateRegionAny.Name = "btn_drawTemplateRegionAny";
            this.btn_drawTemplateRegionAny.Size = new System.Drawing.Size(42, 25);
            this.btn_drawTemplateRegionAny.TabIndex = 7;
            this.btn_drawTemplateRegionAny.TabStop = false;
            this.btn_drawTemplateRegionAny.Text = "任意";
            this.btn_drawTemplateRegionAny.UseVisualStyleBackColor = false;
            this.btn_drawTemplateRegionAny.Click += new System.EventHandler(this.btn_drawTemplateRegionAny_Click);
            // 
            // btn_drawTemplateRegionRectangle1
            // 
            this.btn_drawTemplateRegionRectangle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.btn_drawTemplateRegionRectangle1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_drawTemplateRegionRectangle1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_drawTemplateRegionRectangle1.FlatAppearance.BorderSize = 0;
            this.btn_drawTemplateRegionRectangle1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btn_drawTemplateRegionRectangle1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btn_drawTemplateRegionRectangle1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_drawTemplateRegionRectangle1.ForeColor = System.Drawing.Color.White;
            this.btn_drawTemplateRegionRectangle1.Location = new System.Drawing.Point(18, 249);
            this.btn_drawTemplateRegionRectangle1.Name = "btn_drawTemplateRegionRectangle1";
            this.btn_drawTemplateRegionRectangle1.Size = new System.Drawing.Size(42, 25);
            this.btn_drawTemplateRegionRectangle1.TabIndex = 6;
            this.btn_drawTemplateRegionRectangle1.TabStop = false;
            this.btn_drawTemplateRegionRectangle1.Text = "矩形";
            this.btn_drawTemplateRegionRectangle1.UseVisualStyleBackColor = false;
            this.btn_drawTemplateRegionRectangle1.Click += new System.EventHandler(this.btn_drawTemplateRegionRectangle1_Click);
            // 
            // rdo_templateRegionSub
            // 
            this.rdo_templateRegionSub.AutoSize = true;
            this.rdo_templateRegionSub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdo_templateRegionSub.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdo_templateRegionSub.Location = new System.Drawing.Point(168, 210);
            this.rdo_templateRegionSub.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rdo_templateRegionSub.Name = "rdo_templateRegionSub";
            this.rdo_templateRegionSub.Size = new System.Drawing.Size(50, 25);
            this.rdo_templateRegionSub.TabIndex = 1;
            this.rdo_templateRegionSub.Text = "－";
            this.rdo_templateRegionSub.UseVisualStyleBackColor = true;
            this.rdo_templateRegionSub.CheckedChanged += new System.EventHandler(this.rdo_templateRegionSub_CheckedChanged);
            // 
            // rdo_templateRegionAdd
            // 
            this.rdo_templateRegionAdd.AutoSize = true;
            this.rdo_templateRegionAdd.Checked = true;
            this.rdo_templateRegionAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdo_templateRegionAdd.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdo_templateRegionAdd.Location = new System.Drawing.Point(51, 210);
            this.rdo_templateRegionAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rdo_templateRegionAdd.Name = "rdo_templateRegionAdd";
            this.rdo_templateRegionAdd.Size = new System.Drawing.Size(50, 25);
            this.rdo_templateRegionAdd.TabIndex = 0;
            this.rdo_templateRegionAdd.TabStop = true;
            this.rdo_templateRegionAdd.Text = "＋";
            this.rdo_templateRegionAdd.UseVisualStyleBackColor = true;
            this.rdo_templateRegionAdd.CheckedChanged += new System.EventHandler(this.rdo_templateRegionAdd_CheckedChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.ckb_autoContrast);
            this.tabPage4.Controls.Add(this.numericUpDown2);
            this.tabPage4.Controls.Add(this.nud_angleRange);
            this.tabPage4.Controls.Add(this.numericUpDown1);
            this.tabPage4.Controls.Add(this.nud_angleStart);
            this.tabPage4.Controls.Add(this.comboBox1);
            this.tabPage4.Controls.Add(this.cbx_polarity);
            this.tabPage4.Controls.Add(this.nud_minScore);
            this.tabPage4.Controls.Add(this.numericUpDown4);
            this.tabPage4.Controls.Add(this.numericUpDown3);
            this.tabPage4.Controls.Add(this.nud_angleStep);
            this.tabPage4.Controls.Add(this.nud_matchNum);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Controls.Add(this.panel11);
            this.tabPage4.Controls.Add(this.panel12);
            this.tabPage4.Controls.Add(this.panel13);
            this.tabPage4.Controls.Add(this.panel14);
            this.tabPage4.Controls.Add(this.panel15);
            this.tabPage4.Controls.Add(this.tkb_contrast);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.lbl_contastValue);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.label23);
            this.tabPage4.Controls.Add(this.label87);
            this.tabPage4.Controls.Add(this.label90);
            this.tabPage4.Controls.Add(this.label88);
            this.tabPage4.Controls.Add(this.ckb_autoStep);
            this.tabPage4.Controls.Add(this.label56);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.label15);
            this.tabPage4.Controls.Add(this.label85);
            this.tabPage4.Controls.Add(this.label26);
            this.tabPage4.Controls.Add(this.label55);
            this.tabPage4.Location = new System.Drawing.Point(4, 26);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage4.Size = new System.Drawing.Size(264, 430);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "参数";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // ckb_autoContrast
            // 
            this.ckb_autoContrast.BackColor = System.Drawing.Color.White;
            this.ckb_autoContrast.Checked = true;
            this.ckb_autoContrast.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoContrast.Location = new System.Drawing.Point(56, 276);
            this.ckb_autoContrast.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoContrast.Name = "ckb_autoContrast";
            this.ckb_autoContrast.Size = new System.Drawing.Size(97, 20);
            this.ckb_autoContrast.TabIndex = 166;
            this.ckb_autoContrast.TextStr = "自动";
            this.ckb_autoContrast.CheckChanged += new Controls.DCheckChanged(this.cCheckBox1_CheckChanged);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.BackColor = System.Drawing.Color.White;
            this.numericUpDown2.DecimalPlaces = 1;
            this.numericUpDown2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown2.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown2.Location = new System.Drawing.Point(146, 223);
            this.numericUpDown2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericUpDown2.MaximumSize = new System.Drawing.Size(300, 28);
            this.numericUpDown2.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown2.MinimumSize = new System.Drawing.Size(50, 28);
            this.numericUpDown2.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(80, 28);
            this.numericUpDown2.TabIndex = 165;
            this.numericUpDown2.Value = 0.1D;
            // 
            // nud_angleRange
            // 
            this.nud_angleRange.BackColor = System.Drawing.Color.White;
            this.nud_angleRange.DecimalPlaces = 0;
            this.nud_angleRange.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_angleRange.Incremeent = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_angleRange.Location = new System.Drawing.Point(146, 194);
            this.nud_angleRange.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nud_angleRange.MaximumSize = new System.Drawing.Size(300, 28);
            this.nud_angleRange.MaxValue = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nud_angleRange.MinimumSize = new System.Drawing.Size(50, 28);
            this.nud_angleRange.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nud_angleRange.Name = "nud_angleRange";
            this.nud_angleRange.Size = new System.Drawing.Size(80, 28);
            this.nud_angleRange.TabIndex = 163;
            this.nud_angleRange.Value = 0D;
            this.nud_angleRange.Leave += new System.EventHandler(this.numericUpDown5_Leave_1);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.White;
            this.numericUpDown1.DecimalPlaces = 1;
            this.numericUpDown1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown1.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown1.Location = new System.Drawing.Point(56, 223);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericUpDown1.MaximumSize = new System.Drawing.Size(300, 28);
            this.numericUpDown1.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown1.MinimumSize = new System.Drawing.Size(50, 28);
            this.numericUpDown1.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(80, 28);
            this.numericUpDown1.TabIndex = 164;
            this.numericUpDown1.Value = 0.1D;
            this.numericUpDown1.Leave += new System.EventHandler(this.numericUpDown6_Leave);
            // 
            // nud_angleStart
            // 
            this.nud_angleStart.BackColor = System.Drawing.Color.White;
            this.nud_angleStart.DecimalPlaces = 0;
            this.nud_angleStart.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_angleStart.Incremeent = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_angleStart.Location = new System.Drawing.Point(56, 194);
            this.nud_angleStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nud_angleStart.MaximumSize = new System.Drawing.Size(300, 28);
            this.nud_angleStart.MaxValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nud_angleStart.MinimumSize = new System.Drawing.Size(50, 28);
            this.nud_angleStart.MinValue = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.nud_angleStart.Name = "nud_angleStart";
            this.nud_angleStart.Size = new System.Drawing.Size(80, 28);
            this.nud_angleStart.TabIndex = 162;
            this.nud_angleStart.Value = 0D;
            this.nud_angleStart.Leave += new System.EventHandler(this.numericUpDown5_Leave);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.White;
            this.comboBox1.CanEdit = false;
            this.comboBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.Items = new string[] {
        "从上至下且从左至右"};
            this.comboBox1.Location = new System.Drawing.Point(54, 353);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndex = 0;
            this.comboBox1.Size = new System.Drawing.Size(172, 26);
            this.comboBox1.TabIndex = 119;
            this.comboBox1.TextStr = "从上至下且从左至右";
            // 
            // cbx_polarity
            // 
            this.cbx_polarity.BackColor = System.Drawing.Color.White;
            this.cbx_polarity.CanEdit = false;
            this.cbx_polarity.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_polarity.Items = new string[] {
        "使用极性",
        "忽略极性"};
            this.cbx_polarity.Location = new System.Drawing.Point(55, 169);
            this.cbx_polarity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_polarity.Name = "cbx_polarity";
            this.cbx_polarity.SelectedIndex = 1;
            this.cbx_polarity.Size = new System.Drawing.Size(170, 26);
            this.cbx_polarity.TabIndex = 118;
            this.cbx_polarity.TextStr = "忽略极性";
            // 
            // nud_minScore
            // 
            this.nud_minScore.BackColor = System.Drawing.Color.White;
            this.nud_minScore.DecimalPlaces = 1;
            this.nud_minScore.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_minScore.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_minScore.Location = new System.Drawing.Point(56, 49);
            this.nud_minScore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nud_minScore.MaximumSize = new System.Drawing.Size(300, 26);
            this.nud_minScore.MaxValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_minScore.MinimumSize = new System.Drawing.Size(50, 26);
            this.nud_minScore.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_minScore.Name = "nud_minScore";
            this.nud_minScore.Size = new System.Drawing.Size(170, 26);
            this.nud_minScore.TabIndex = 118;
            this.nud_minScore.Value = 0.1D;
            this.nud_minScore.ValueChanged += new Controls.DValueChanged(this.nud_minScore_ValueChanged);
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.BackColor = System.Drawing.Color.White;
            this.numericUpDown4.DecimalPlaces = 0;
            this.numericUpDown4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown4.Incremeent = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown4.Location = new System.Drawing.Point(54, 376);
            this.numericUpDown4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericUpDown4.MaximumSize = new System.Drawing.Size(300, 28);
            this.numericUpDown4.MaxValue = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown4.MinimumSize = new System.Drawing.Size(50, 28);
            this.numericUpDown4.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(170, 28);
            this.numericUpDown4.TabIndex = 163;
            this.numericUpDown4.Value = 100D;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.BackColor = System.Drawing.Color.White;
            this.numericUpDown3.DecimalPlaces = 0;
            this.numericUpDown3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown3.Incremeent = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown3.Location = new System.Drawing.Point(55, 136);
            this.numericUpDown3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericUpDown3.MaximumSize = new System.Drawing.Size(300, 28);
            this.numericUpDown3.MaxValue = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown3.MinimumSize = new System.Drawing.Size(50, 28);
            this.numericUpDown3.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(170, 28);
            this.numericUpDown3.TabIndex = 162;
            this.numericUpDown3.Value = 100D;
            // 
            // nud_angleStep
            // 
            this.nud_angleStep.BackColor = System.Drawing.Color.White;
            this.nud_angleStep.DecimalPlaces = 0;
            this.nud_angleStep.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_angleStep.Incremeent = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_angleStep.Location = new System.Drawing.Point(55, 107);
            this.nud_angleStep.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nud_angleStep.MaximumSize = new System.Drawing.Size(300, 28);
            this.nud_angleStep.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nud_angleStep.MinimumSize = new System.Drawing.Size(50, 28);
            this.nud_angleStep.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_angleStep.Name = "nud_angleStep";
            this.nud_angleStep.Size = new System.Drawing.Size(170, 28);
            this.nud_angleStep.TabIndex = 161;
            this.nud_angleStep.Value = 1D;
            // 
            // nud_matchNum
            // 
            this.nud_matchNum.BackColor = System.Drawing.Color.White;
            this.nud_matchNum.DecimalPlaces = 0;
            this.nud_matchNum.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_matchNum.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_matchNum.Location = new System.Drawing.Point(56, 78);
            this.nud_matchNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nud_matchNum.MaximumSize = new System.Drawing.Size(300, 28);
            this.nud_matchNum.MaxValue = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nud_matchNum.MinimumSize = new System.Drawing.Size(50, 28);
            this.nud_matchNum.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_matchNum.Name = "nud_matchNum";
            this.nud_matchNum.Size = new System.Drawing.Size(170, 28);
            this.nud_matchNum.TabIndex = 160;
            this.nud_matchNum.Value = 1D;
            this.nud_matchNum.ValueChanged += new Controls.DValueChanged(this.nud_matchNum_ValueChanged);
            this.nud_matchNum.Leave += new System.EventHandler(this.nud_matchNum_Leave_1);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(9, 227);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 17);
            this.label12.TabIndex = 149;
            this.label12.Text = "缩  放：";
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel11.Controls.Add(this.label11);
            this.panel11.Location = new System.Drawing.Point(4, 15);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(254, 20);
            this.panel11.TabIndex = 147;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(3, 1);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 17);
            this.label11.TabIndex = 11;
            this.label11.Text = "模板参数";
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel12.Location = new System.Drawing.Point(4, 413);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(254, 1);
            this.panel12.TabIndex = 145;
            // 
            // panel13
            // 
            this.panel13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel13.Location = new System.Drawing.Point(257, 29);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(1, 384);
            this.panel13.TabIndex = 146;
            // 
            // panel14
            // 
            this.panel14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel14.Location = new System.Drawing.Point(4, 30);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(1, 384);
            this.panel14.TabIndex = 144;
            // 
            // panel15
            // 
            this.panel15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel15.Controls.Add(this.label9);
            this.panel15.Location = new System.Drawing.Point(4, 319);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(254, 20);
            this.panel15.TabIndex = 143;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(3, 1);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 17);
            this.label9.TabIndex = 12;
            this.label9.Text = "排序";
            // 
            // tkb_contrast
            // 
            this.tkb_contrast.AutoSize = false;
            this.tkb_contrast.BackColor = System.Drawing.Color.White;
            this.tkb_contrast.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tkb_contrast.Enabled = false;
            this.tkb_contrast.Location = new System.Drawing.Point(51, 258);
            this.tkb_contrast.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tkb_contrast.Maximum = 244;
            this.tkb_contrast.Minimum = 1;
            this.tkb_contrast.Name = "tkb_contrast";
            this.tkb_contrast.Size = new System.Drawing.Size(182, 20);
            this.tkb_contrast.TabIndex = 54;
            this.tkb_contrast.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tkb_contrast.Value = 30;
            this.tkb_contrast.Scroll += new System.EventHandler(this.tkb_contrast_Scroll);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(223, 381);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 17);
            this.label8.TabIndex = 118;
            this.label8.Text = "pix";
            // 
            // lbl_contastValue
            // 
            this.lbl_contastValue.AutoSize = true;
            this.lbl_contastValue.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_contastValue.Location = new System.Drawing.Point(201, 279);
            this.lbl_contastValue.Name = "lbl_contastValue";
            this.lbl_contastValue.Size = new System.Drawing.Size(22, 17);
            this.lbl_contastValue.TabIndex = 63;
            this.lbl_contastValue.Text = "80";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 381);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 17);
            this.label7.TabIndex = 116;
            this.label7.Text = "间  隔：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 353);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "排  序：";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label23.Location = new System.Drawing.Point(9, 198);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(52, 17);
            this.label23.TabIndex = 113;
            this.label23.Text = "角  度：";
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label87.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label87.Location = new System.Drawing.Point(224, 141);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(25, 17);
            this.label87.TabIndex = 112;
            this.label87.Text = "ms";
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label90.Location = new System.Drawing.Point(9, 169);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(52, 17);
            this.label90.TabIndex = 109;
            this.label90.Text = "极  性：";
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label88.Location = new System.Drawing.Point(9, 140);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(52, 17);
            this.label88.TabIndex = 108;
            this.label88.Text = "超  时：";
            // 
            // ckb_autoStep
            // 
            this.ckb_autoStep.AutoSize = true;
            this.ckb_autoStep.Checked = true;
            this.ckb_autoStep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckb_autoStep.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoStep.Location = new System.Drawing.Point(276, 107);
            this.ckb_autoStep.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoStep.Name = "ckb_autoStep";
            this.ckb_autoStep.Size = new System.Drawing.Size(51, 21);
            this.ckb_autoStep.TabIndex = 107;
            this.ckb_autoStep.Text = "自动";
            this.ckb_autoStep.UseVisualStyleBackColor = true;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label56.Location = new System.Drawing.Point(9, 111);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(52, 17);
            this.label56.TabIndex = 104;
            this.label56.Text = "步  长：";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(9, 53);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 17);
            this.label14.TabIndex = 10;
            this.label14.Text = "分  数：";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(9, 82);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(52, 17);
            this.label15.TabIndex = 11;
            this.label15.Text = "个  数：";
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label85.Location = new System.Drawing.Point(9, 256);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(52, 17);
            this.label85.TabIndex = 53;
            this.label85.Text = "阈  值：";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label26.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label26.Location = new System.Drawing.Point(225, 199);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(31, 17);
            this.label26.TabIndex = 115;
            this.label26.Text = "deg";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label55.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label55.Location = new System.Drawing.Point(224, 112);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(31, 17);
            this.label55.TabIndex = 106;
            this.label55.Text = "deg";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.cbx_searchRegionType);
            this.tabPage6.Controls.Add(this.label5);
            this.tabPage6.Location = new System.Drawing.Point(4, 26);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(264, 430);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "搜索区域";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // cbx_searchRegionType
            // 
            this.cbx_searchRegionType.BackColor = System.Drawing.Color.White;
            this.cbx_searchRegionType.CanEdit = false;
            this.cbx_searchRegionType.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_searchRegionType.Items = new string[] {
        "整幅图像",
        "矩形",
        "仿射矩形",
        "圆",
        "多点"};
            this.cbx_searchRegionType.Location = new System.Drawing.Point(76, 13);
            this.cbx_searchRegionType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_searchRegionType.Name = "cbx_searchRegionType";
            this.cbx_searchRegionType.SelectedIndex = 0;
            this.cbx_searchRegionType.Size = new System.Drawing.Size(172, 26);
            this.cbx_searchRegionType.TabIndex = 120;
            this.cbx_searchRegionType.TextStr = "整幅图像";
            this.cbx_searchRegionType.SelectedIndexChanged += new Controls.DSelectedIndexChanged(this.cbx_searchRegionType_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(12, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "搜索区域：";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pictureBox9);
            this.tabPage2.Controls.Add(this.checkBox2);
            this.tabPage2.Controls.Add(this.pictureBox8);
            this.tabPage2.Controls.Add(this.checkBox1);
            this.tabPage2.Controls.Add(this.pictureBox5);
            this.tabPage2.Controls.Add(this.pictureBox4);
            this.tabPage2.Controls.Add(this.pictureBox3);
            this.tabPage2.Controls.Add(this.cbx_showTemplate);
            this.tabPage2.Controls.Add(this.ckb_showFeature);
            this.tabPage2.Controls.Add(this.ckb_showCross);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(264, 430);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "图形";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pictureBox9
            // 
            this.pictureBox9.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox9.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox9.Image")));
            this.pictureBox9.Location = new System.Drawing.Point(14, 66);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(20, 20);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox9.TabIndex = 12;
            this.pictureBox9.TabStop = false;
            this.pictureBox9.Click += new System.EventHandler(this.pictureBox9_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox2.Location = new System.Drawing.Point(19, 67);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(99, 21);
            this.checkBox2.TabIndex = 11;
            this.checkBox2.Text = "显示搜索区域";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // pictureBox8
            // 
            this.pictureBox8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox8.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox8.Image")));
            this.pictureBox8.Location = new System.Drawing.Point(14, 91);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(20, 20);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox8.TabIndex = 10;
            this.pictureBox8.TabStop = false;
            this.pictureBox8.Click += new System.EventHandler(this.pictureBox8_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox1.Location = new System.Drawing.Point(19, 92);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(75, 21);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "显示序号";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // pictureBox5
            // 
            this.pictureBox5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(14, 41);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(20, 20);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5.TabIndex = 8;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.Click += new System.EventHandler(this.pictureBox5_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(14, 116);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(20, 20);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 7;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(14, 16);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(20, 20);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 6;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // cbx_showTemplate
            // 
            this.cbx_showTemplate.AutoSize = true;
            this.cbx_showTemplate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.cbx_showTemplate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbx_showTemplate.Location = new System.Drawing.Point(19, 17);
            this.cbx_showTemplate.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_showTemplate.Name = "cbx_showTemplate";
            this.cbx_showTemplate.Size = new System.Drawing.Size(75, 21);
            this.cbx_showTemplate.TabIndex = 5;
            this.cbx_showTemplate.Text = "显示模板";
            this.cbx_showTemplate.UseVisualStyleBackColor = true;
            this.cbx_showTemplate.CheckedChanged += new System.EventHandler(this.cbx_showTemplate_CheckedChanged);
            // 
            // ckb_showFeature
            // 
            this.ckb_showFeature.AutoSize = true;
            this.ckb_showFeature.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ckb_showFeature.Location = new System.Drawing.Point(19, 42);
            this.ckb_showFeature.Margin = new System.Windows.Forms.Padding(2);
            this.ckb_showFeature.Name = "ckb_showFeature";
            this.ckb_showFeature.Size = new System.Drawing.Size(75, 21);
            this.ckb_showFeature.TabIndex = 4;
            this.ckb_showFeature.Text = "显示特征";
            this.ckb_showFeature.UseVisualStyleBackColor = true;
            this.ckb_showFeature.CheckedChanged += new System.EventHandler(this.ckb_showFeature_CheckedChanged);
            // 
            // ckb_showCross
            // 
            this.ckb_showCross.AutoSize = true;
            this.ckb_showCross.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ckb_showCross.Location = new System.Drawing.Point(19, 118);
            this.ckb_showCross.Margin = new System.Windows.Forms.Padding(2);
            this.ckb_showCross.Name = "ckb_showCross";
            this.ckb_showCross.Size = new System.Drawing.Size(87, 21);
            this.ckb_showCross.TabIndex = 3;
            this.ckb_showCross.Text = "显示参考点";
            this.ckb_showCross.UseVisualStyleBackColor = true;
            this.ckb_showCross.CheckedChanged += new System.EventHandler(this.ckb_showCross_CheckedChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgv_matchResult);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(264, 430);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "结果";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgv_matchResult
            // 
            this.dgv_matchResult.AllowDrop = true;
            this.dgv_matchResult.AllowUserToAddRows = false;
            this.dgv_matchResult.AllowUserToDeleteRows = false;
            this.dgv_matchResult.AllowUserToResizeRows = false;
            this.dgv_matchResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_matchResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column10,
            this.Column12});
            this.dgv_matchResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_matchResult.Location = new System.Drawing.Point(0, 0);
            this.dgv_matchResult.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv_matchResult.Name = "dgv_matchResult";
            this.dgv_matchResult.ReadOnly = true;
            this.dgv_matchResult.RowHeadersVisible = false;
            this.dgv_matchResult.RowTemplate.Height = 23;
            this.dgv_matchResult.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_matchResult.Size = new System.Drawing.Size(264, 430);
            this.dgv_matchResult.TabIndex = 12;
            this.dgv_matchResult.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_matchResult_CellClick);
            // 
            // Column7
            // 
            this.Column7.HeaderText = "编号";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Width = 55;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "分数";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 60;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "行";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.Width = 70;
            // 
            // Column10
            // 
            this.Column10.HeaderText = "列";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.Width = 70;
            // 
            // Column12
            // 
            this.Column12.HeaderText = "角度(°)";
            this.Column12.Name = "Column12";
            this.Column12.ReadOnly = true;
            this.Column12.Width = 80;
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
            this.hWindow_Final1.Location = new System.Drawing.Point(3, 3);
            this.hWindow_Final1.Name = "hWindow_Final1";
            this.hWindow_Final1.Size = new System.Drawing.Size(613, 462);
            this.hWindow_Final1.TabIndex = 6;
            // 
            // cnt_rightClickMenu
            // 
            this.cnt_rightClickMenu.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cnt_rightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.适应图像ToolStripMenuItem,
            this.显示信息ToolStripMenuItem,
            this.全屏显示ToolStripMenuItem,
            this.图像另存为ToolStripMenuItem,
            this.保存窗口ToolStripMenuItem});
            this.cnt_rightClickMenu.Name = "cnt_rightClickMenu";
            this.cnt_rightClickMenu.Size = new System.Drawing.Size(208, 114);
            // 
            // 适应图像ToolStripMenuItem
            // 
            this.适应图像ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.适应图像ToolStripMenuItem.Name = "适应图像ToolStripMenuItem";
            this.适应图像ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.适应图像ToolStripMenuItem.Text = "适应窗口";
            this.适应图像ToolStripMenuItem.Click += new System.EventHandler(this.适应图像ToolStripMenuItem_Click);
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
            this.全屏显示ToolStripMenuItem.Click += new System.EventHandler(this.全屏显示ToolStripMenuItem_Click);
            // 
            // 图像另存为ToolStripMenuItem
            // 
            this.图像另存为ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.图像另存为ToolStripMenuItem.Name = "图像另存为ToolStripMenuItem";
            this.图像另存为ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.图像另存为ToolStripMenuItem.Text = "图像另存";
            this.图像另存为ToolStripMenuItem.Click += new System.EventHandler(this.图像另存为ToolStripMenuItem_Click);
            // 
            // 保存窗口ToolStripMenuItem
            // 
            this.保存窗口ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.保存窗口ToolStripMenuItem.Name = "保存窗口ToolStripMenuItem";
            this.保存窗口ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.保存窗口ToolStripMenuItem.Text = "保存窗口";
            this.保存窗口ToolStripMenuItem.Click += new System.EventHandler(this.保存窗口ToolStripMenuItem_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btn_confirm);
            this.panel6.Controls.Add(this.btn_cancel);
            this.panel6.Controls.Add(this.btn_runTool);
            this.panel6.Controls.Add(this.label3);
            this.panel6.Controls.Add(this.label4);
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
            this.btn_confirm.Location = new System.Drawing.Point(721, 18);
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
            this.btn_cancel.Location = new System.Drawing.Point(820, 18);
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
            this.btn_runTool.Location = new System.Drawing.Point(652, 18);
            this.btn_runTool.Name = "btn_runTool";
            this.btn_runTool.Size = new System.Drawing.Size(65, 30);
            this.btn_runTool.TabIndex = 113;
            this.btn_runTool.Text = "运行工具";
            this.btn_runTool.UseVisualStyleBackColor = true;
            this.btn_runTool.Click += new System.EventHandler(this.tsb_runTool_Click);
            this.btn_runTool.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_runTool.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_runTool.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_runTool.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(94, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 117;
            this.label3.Text = "耗时：0ms";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(94, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 116;
            this.label4.Text = "状态：成功";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = global::VMPro.Properties.Resources.开;
            this.pictureBox2.Location = new System.Drawing.Point(18, 22);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(50, 25);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 109;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel5.Location = new System.Drawing.Point(7, 4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(882, 1);
            this.panel5.TabIndex = 112;
            // 
            // Frm_ShapeMatchTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(907, 592);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(380, 120);
            this.Name = "Frm_ShapeMatchTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模板匹配";
            this.Load += new System.EventHandler(this.Frm_ShapeMatchTool_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Frm_ShapeMatchTool_KeyUp);
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tbc_shapeMatch.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.panel17.ResumeLayout(false);
            this.panel17.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tkb_contrast)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_matchResult)).EndInit();
            this.cnt_rightClickMenu.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        public System.Windows.Forms.TabControl tbc_shapeMatch;
        public System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Panel panel10;
        public System.Windows.Forms.PictureBox pictureBox7;
        public System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel7;
        public System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Button btn_drawTemplateRegionRectangle2;
        internal System.Windows.Forms.Button button7;
        internal System.Windows.Forms.Button button6;
        internal System.Windows.Forms.Button btn_drawTemplateRegionCircle;
        internal System.Windows.Forms.Button btn_drawTemplateRegionEllipse;
        internal System.Windows.Forms.Button btn_drawTemplateRegionAny;
        internal System.Windows.Forms.Button btn_drawTemplateRegionRectangle1;
        public System.Windows.Forms.RadioButton rdo_templateRegionSub;
        public System.Windows.Forms.RadioButton rdo_templateRegionAdd;
        public System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panel11;
        public System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.Panel panel15;
        public System.Windows.Forms.Label label9;
        internal System.Windows.Forms.TrackBar tkb_contrast;
        public System.Windows.Forms.Label label8;
        internal System.Windows.Forms.Label lbl_contastValue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label26;
        public System.Windows.Forms.Label label23;
        public System.Windows.Forms.Label label87;
        public System.Windows.Forms.Label label90;
        public System.Windows.Forms.Label label88;
        public System.Windows.Forms.CheckBox ckb_autoStep;
        public System.Windows.Forms.Label label55;
        public System.Windows.Forms.Label label56;
        public System.Windows.Forms.Label label14;
        public System.Windows.Forms.Label label15;
        public System.Windows.Forms.Label label85;
        private System.Windows.Forms.TabPage tabPage6;
        public System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.PictureBox pictureBox8;
        public System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.PictureBox pictureBox5;
        public System.Windows.Forms.PictureBox pictureBox4;
        public System.Windows.Forms.PictureBox pictureBox3;
        public System.Windows.Forms.CheckBox cbx_showTemplate;
        public System.Windows.Forms.CheckBox ckb_showFeature;
        public System.Windows.Forms.CheckBox ckb_showCross;
        private System.Windows.Forms.TabPage tabPage1;
        internal System.Windows.Forms.DataGridView dgv_matchResult;
        private System.Windows.Forms.Panel panel6;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.PictureBox pictureBox2;
        internal System.Windows.Forms.Button btn_confirm;
        internal System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Panel panel5;
        internal System.Windows.Forms.Button btn_runTool;
        public System.Windows.Forms.ContextMenuStrip cnt_rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem 适应图像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全屏显示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图像另存为ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存窗口ToolStripMenuItem;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.RadioButton radioButton1;
        public System.Windows.Forms.Panel panel17;
        public System.Windows.Forms.Button button9;
        public System.Windows.Forms.Panel panel4;
        public System.Windows.Forms.Button button5;
        public System.Windows.Forms.ToolStrip toolStrip1;
        public ChoiceTech.Halcon.Control.HWindow_Final hWindow_Final1;
        public System.Windows.Forms.PictureBox pictureBox9;
        public System.Windows.Forms.CheckBox checkBox2;
        public System.Windows.Forms.Label label12;
        public CNumericUpDown nud_matchNum;
        public CNumericUpDown nud_angleStep;
        public CNumericUpDown numericUpDown3;
        public CNumericUpDown numericUpDown4;
        public CNumericUpDown nud_minScore;
        public CComboBox cbx_polarity;
        public CComboBox comboBox1;
        public CComboBox cbx_searchRegionType;
        public CNumericUpDown nud_angleStart;
        public CNumericUpDown nud_angleRange;
        public CNumericUpDown numericUpDown2;
        public CNumericUpDown numericUpDown1;
        private System.Windows.Forms.RadioButton radioButton4;
        internal System.Windows.Forms.RadioButton radioButton2;
        internal System.Windows.Forms.RadioButton radioButton3;
        public HalconDotNet.HWindowControl hwc_template;
        public System.Windows.Forms.Button button10;
        public System.Windows.Forms.Button button8;
        public System.Windows.Forms.Button button4;
        public System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        internal CCheckBox ckb_autoContrast;
    }
}