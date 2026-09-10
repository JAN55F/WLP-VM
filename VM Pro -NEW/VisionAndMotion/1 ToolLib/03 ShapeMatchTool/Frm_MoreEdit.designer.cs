using Controls;
namespace VMPro
{
    partial class Frm_MoreEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_MoreEdit));
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btn_runJob = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_runTool = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.hWindow_Final1 = new ChoiceTech.Halcon.Control.HWindow_Final();
            this.cnt_rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.适应图像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.相机实时ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全屏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图像另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.tsb_displayImage = new System.Windows.Forms.ToolStripButton();
            this.tsb_saveImage = new System.Windows.Forms.ToolStripButton();
            this.tsb_SDKInfo = new System.Windows.Forms.ToolStripButton();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.cnt_rightClickMenu.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Location = new System.Drawing.Point(10, 6);
            this.lbl_title.Size = new System.Drawing.Size(56, 17);
            this.lbl_title.Text = "更多参数";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Size = new System.Drawing.Size(20, 18);
            this.pictureBox1.Visible = false;
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
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(903, 564);
            this.tableLayoutPanel1.TabIndex = 115;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.button3);
            this.panel6.Controls.Add(this.btn_runJob);
            this.panel6.Controls.Add(this.btn_close);
            this.panel6.Controls.Add(this.btn_runTool);
            this.panel6.Controls.Add(this.panel5);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 502);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(897, 59);
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
            this.btn_runJob.Location = new System.Drawing.Point(716, 18);
            this.btn_runJob.Name = "btn_runJob";
            this.btn_runJob.Size = new System.Drawing.Size(65, 30);
            this.btn_runJob.TabIndex = 116;
            this.btn_runJob.TabStop = false;
            this.btn_runJob.Text = "运行流程";
            this.btn_runJob.UseVisualStyleBackColor = true;
            this.btn_runJob.Click += new System.EventHandler(this.btn_runJob_Click);
            this.btn_runJob.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_runJob.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_runJob.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_runJob.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
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
            this.btn_close.Location = new System.Drawing.Point(821, 18);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(65, 30);
            this.btn_close.TabIndex = 111;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            this.btn_close.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_close.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_close.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_close.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
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
            this.btn_runTool.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_runTool.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_runTool.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_runTool.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
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
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.tableLayoutPanel2.Controls.Add(this.hWindow_Final1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 493F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(897, 493);
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
            this.hWindow_Final1.Size = new System.Drawing.Size(613, 485);
            this.hWindow_Final1.TabIndex = 90;
            // 
            // cnt_rightClickMenu
            // 
            this.cnt_rightClickMenu.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cnt_rightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.适应图像ToolStripMenuItem,
            this.相机实时ToolStripMenuItem,
            this.显示信息ToolStripMenuItem,
            this.全屏ToolStripMenuItem,
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
            this.适应图像ToolStripMenuItem.Click += new System.EventHandler(this.适应图像ToolStripMenuItem_Click);
            // 
            // 相机实时ToolStripMenuItem
            // 
            this.相机实时ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.相机实时ToolStripMenuItem.CheckOnClick = true;
            this.相机实时ToolStripMenuItem.Name = "相机实时ToolStripMenuItem";
            this.相机实时ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.相机实时ToolStripMenuItem.Text = "相机实时";
            this.相机实时ToolStripMenuItem.Click += new System.EventHandler(this.相机实时ToolStripMenuItem_Click);
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
            // 全屏ToolStripMenuItem
            // 
            this.全屏ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.全屏ToolStripMenuItem.CheckOnClick = true;
            this.全屏ToolStripMenuItem.Name = "全屏ToolStripMenuItem";
            this.全屏ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.全屏ToolStripMenuItem.Text = "全屏（ESC退出全屏）";
            this.全屏ToolStripMenuItem.Click += new System.EventHandler(this.全屏ToolStripMenuItem_Click);
            // 
            // 图像另存为ToolStripMenuItem
            // 
            this.图像另存为ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.图像另存为ToolStripMenuItem.Name = "图像另存为ToolStripMenuItem";
            this.图像另存为ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.图像另存为ToolStripMenuItem.Text = "图像另存";
            this.图像另存为ToolStripMenuItem.Click += new System.EventHandler(this.图像另存为ToolStripMenuItem_Click);
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.AutoSize = false;
            this.miniToolStrip.BackColor = System.Drawing.Color.White;
            this.miniToolStrip.CanOverflow = false;
            this.miniToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.miniToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.miniToolStrip.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.miniToolStrip.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.miniToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.miniToolStrip.Location = new System.Drawing.Point(0, 0);
            this.miniToolStrip.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.miniToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.miniToolStrip.Size = new System.Drawing.Size(723, 25);
            this.miniToolStrip.TabIndex = 92;
            // 
            // tsb_displayImage
            // 
            this.tsb_displayImage.AutoSize = false;
            this.tsb_displayImage.CheckOnClick = true;
            this.tsb_displayImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_displayImage.Image = ((System.Drawing.Image)(resources.GetObject("tsb_displayImage.Image")));
            this.tsb_displayImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_displayImage.Name = "tsb_displayImage";
            this.tsb_displayImage.RightToLeftAutoMirrorImage = true;
            this.tsb_displayImage.Size = new System.Drawing.Size(25, 22);
            this.tsb_displayImage.Text = "toolStripButton6";
            this.tsb_displayImage.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsb_displayImage.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_displayImage.ToolTipText = "相机实时";
            this.tsb_displayImage.Click += new System.EventHandler(this.tsb_displayImage_Click);
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
            this.tsb_saveImage.Click += new System.EventHandler(this.tsb_saveImage_Click);
            // 
            // tsb_SDKInfo
            // 
            this.tsb_SDKInfo.AutoSize = false;
            this.tsb_SDKInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_SDKInfo.Image = ((System.Drawing.Image)(resources.GetObject("tsb_SDKInfo.Image")));
            this.tsb_SDKInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_SDKInfo.Name = "tsb_SDKInfo";
            this.tsb_SDKInfo.RightToLeftAutoMirrorImage = true;
            this.tsb_SDKInfo.Size = new System.Drawing.Size(25, 22);
            this.tsb_SDKInfo.Text = "toolStripButton4";
            this.tsb_SDKInfo.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.tsb_SDKInfo.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_SDKInfo.ToolTipText = "相机SDK信息";
            this.tsb_SDKInfo.Click += new System.EventHandler(this.tsb_SDKInfo_Click);
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
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.toolStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_displayImage,
            this.tsb_saveImage,
            this.tsb_SDKInfo,
            this.tsb_resetTool});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(2, 0);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(723, 25);
            this.toolStrip1.TabIndex = 92;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(580, 18);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(65, 30);
            this.button3.TabIndex = 117;
            this.button3.TabStop = false;
            this.button3.Text = "重新学习";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // Frm_MoreEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(907, 592);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(380, 120);
            this.Name = "Frm_MoreEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDK_海康威视";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Frm_AcqImageTool_KeyUp);
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.cnt_rightClickMenu.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel3;
        public ChoiceTech.Halcon.Control.HWindow_Final hWindow_Final1;
        public System.Windows.Forms.ContextMenuStrip cnt_rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem 适应图像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全屏ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图像另存为ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 相机实时ToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel6;
        internal System.Windows.Forms.Button btn_runJob;
        internal System.Windows.Forms.Button btn_close;
        internal System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStrip miniToolStrip;
        public System.Windows.Forms.ToolStripButton tsb_displayImage;
        private System.Windows.Forms.ToolStripButton tsb_saveImage;
        private System.Windows.Forms.ToolStripButton tsb_SDKInfo;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStrip toolStrip1;
        internal System.Windows.Forms.Button button3;
    }
}