using Controls;
namespace VMPro
{
    partial class Frm_DistancePPTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_DistancePPTool));
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cnt_rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.适应图像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.实时ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全屏显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图像另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btn_confirm = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_runTool = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_realTimeDisplay = new System.Windows.Forms.ToolStripButton();
            this.tsb_saveImage = new System.Windows.Forms.ToolStripButton();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbx_caputurePosY = new CNumeric();
            this.tbx_caputurePosX = new CNumeric();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numeric1 = new CNumeric();
            this.numeric2 = new CNumeric();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numeric3 = new CNumeric();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.cnt_rightClickMenu.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.panel3.Size = new System.Drawing.Size(903, 564);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(903, 564);
            this.tableLayoutPanel1.TabIndex = 115;
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
            this.panel6.Controls.Add(this.btn_runTool);
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
            this.btn_runTool.Location = new System.Drawing.Point(648, 18);
            this.btn_runTool.Name = "btn_runTool";
            this.btn_runTool.Size = new System.Drawing.Size(65, 30);
            this.btn_runTool.TabIndex = 0;
            this.btn_runTool.TabStop = false;
            this.btn_runTool.Text = "运行工具";
            this.btn_runTool.UseVisualStyleBackColor = true;
            this.btn_runTool.Click += new System.EventHandler(this.tsb_runTool_Click);
            this.btn_runTool.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_runTool.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_runTool.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_runTool.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
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
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
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
            this.tsb_resetTool,
            this.toolStripButton2});
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
            // toolStripButton2
            // 
            this.toolStripButton2.AutoSize = false;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.RightToLeftAutoMirrorImage = true;
            this.toolStripButton2.Size = new System.Drawing.Size(25, 22);
            this.toolStripButton2.Text = "toolStripButton4";
            this.toolStripButton2.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripButton2.ToolTipText = "相机SDK信息";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.numeric3);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.numeric1);
            this.panel2.Controls.Add(this.numeric2);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.tbx_caputurePosY);
            this.panel2.Controls.Add(this.tbx_caputurePosX);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(897, 468);
            this.panel2.TabIndex = 93;
            // 
            // tbx_caputurePosY
            // 
            this.tbx_caputurePosY.BackColor = System.Drawing.Color.White;
            this.tbx_caputurePosY.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_caputurePosY.Location = new System.Drawing.Point(83, 54);
            this.tbx_caputurePosY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_caputurePosY.Name = "tbx_caputurePosY";
            this.tbx_caputurePosY.Size = new System.Drawing.Size(67, 22);
            this.tbx_caputurePosY.TabIndex = 114;
            this.tbx_caputurePosY.Value = "";
            // 
            // tbx_caputurePosX
            // 
            this.tbx_caputurePosX.BackColor = System.Drawing.Color.White;
            this.tbx_caputurePosX.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_caputurePosX.Location = new System.Drawing.Point(83, 30);
            this.tbx_caputurePosX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_caputurePosX.Name = "tbx_caputurePosX";
            this.tbx_caputurePosX.Size = new System.Drawing.Size(67, 22);
            this.tbx_caputurePosX.TabIndex = 113;
            this.tbx_caputurePosX.Value = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 111;
            this.label3.Text = "X：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 12);
            this.label4.TabIndex = 112;
            this.label4.Text = "Y：";
            // 
            // numeric1
            // 
            this.numeric1.BackColor = System.Drawing.Color.White;
            this.numeric1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numeric1.Location = new System.Drawing.Point(83, 135);
            this.numeric1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numeric1.Name = "numeric1";
            this.numeric1.Size = new System.Drawing.Size(67, 22);
            this.numeric1.TabIndex = 118;
            this.numeric1.Value = "";
            // 
            // numeric2
            // 
            this.numeric2.BackColor = System.Drawing.Color.White;
            this.numeric2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numeric2.Location = new System.Drawing.Point(83, 111);
            this.numeric2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numeric2.Name = "numeric2";
            this.numeric2.Size = new System.Drawing.Size(67, 22);
            this.numeric2.TabIndex = 117;
            this.numeric2.Value = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(59, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 115;
            this.label5.Text = "X：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(59, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 12);
            this.label6.TabIndex = 116;
            this.label6.Text = "Y：";
            // 
            // numeric3
            // 
            this.numeric3.BackColor = System.Drawing.Color.White;
            this.numeric3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numeric3.Location = new System.Drawing.Point(83, 232);
            this.numeric3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numeric3.Name = "numeric3";
            this.numeric3.Size = new System.Drawing.Size(67, 22);
            this.numeric3.TabIndex = 122;
            this.numeric3.Value = "";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(59, 234);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 12);
            this.label8.TabIndex = 120;
            this.label8.Text = "Y：";
            // 
            // Frm_DistancePPTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(907, 592);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(380, 120);
            this.Name = "Frm_DistancePPTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDK_海康威视";
            this.Load += new System.EventHandler(this.Frm_SDK_HIKVisionTool_Load_1);
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.cnt_rightClickMenu.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
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
        public System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Button btn_confirm;
        internal System.Windows.Forms.Button btn_cancel;
        public System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel5;
        internal System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ToolStripButton tsb_realTimeDisplay;
        private System.Windows.Forms.ToolStripButton tsb_saveImage;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        public System.Windows.Forms.ContextMenuStrip cnt_rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem 适应图像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全屏显示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图像另存为ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 实时ToolStripMenuItem;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.Panel panel2;
        public CNumeric numeric3;
        private System.Windows.Forms.Label label8;
        public CNumeric numeric1;
        public CNumeric numeric2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        public CNumeric tbx_caputurePosY;
        public CNumeric tbx_caputurePosX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}