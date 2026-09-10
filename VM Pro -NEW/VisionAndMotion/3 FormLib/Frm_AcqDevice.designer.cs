using Controls;
namespace VMPro
{
    partial class Frm_AcqDevice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_AcqDevice));
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.hWindow_Final1 = new ChoiceTech.Halcon.Control.HWindow_Final();
            this.cnt_rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.适应图像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.相机实时ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全屏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图像另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ckb_rotateImage = new Controls.CCheckBox();
            this.ckb_RGBToGray = new Controls.CCheckBox();
            this.ckb_autoSwitch = new Controls.CCheckBox();
            this.nud_rotateAngle = new Controls.CNumericUpDown();
            this.lbl_deg = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pic_fromLocalDirectory = new System.Windows.Forms.PictureBox();
            this.rdo_fromLocalDirectory = new System.Windows.Forms.RadioButton();
            this.pic_fromLocalFile = new System.Windows.Forms.PictureBox();
            this.pic_fromDevice = new System.Windows.Forms.PictureBox();
            this.radio_FromLocalFile = new System.Windows.Forms.RadioButton();
            this.rdo_fromDevice = new System.Windows.Forms.RadioButton();
            this.pnl_formPanel = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_runTool = new System.Windows.Forms.Button();
            this.lbl_runTime = new System.Windows.Forms.Label();
            this.lbl_toolTip = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_SDKInfo = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.cnt_rightClickMenu.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_fromLocalDirectory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_fromLocalFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_fromDevice)).BeginInit();
            this.panel6.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Size = new System.Drawing.Size(56, 17);
            this.lbl_title.Text = "采集设备";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Size = new System.Drawing.Size(20, 18);
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
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(903, 564);
            this.tableLayoutPanel1.TabIndex = 115;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.tableLayoutPanel2.Controls.Add(this.hWindow_Final1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 0);
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
            // panel2
            // 
            this.panel2.Controls.Add(this.ckb_rotateImage);
            this.panel2.Controls.Add(this.ckb_RGBToGray);
            this.panel2.Controls.Add(this.ckb_autoSwitch);
            this.panel2.Controls.Add(this.nud_rotateAngle);
            this.panel2.Controls.Add(this.lbl_deg);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.pic_fromLocalDirectory);
            this.panel2.Controls.Add(this.rdo_fromLocalDirectory);
            this.panel2.Controls.Add(this.pic_fromLocalFile);
            this.panel2.Controls.Add(this.pic_fromDevice);
            this.panel2.Controls.Add(this.radio_FromLocalFile);
            this.panel2.Controls.Add(this.rdo_fromDevice);
            this.panel2.Controls.Add(this.pnl_formPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(621, 3);
            this.panel2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(274, 462);
            this.panel2.TabIndex = 89;
            // 
            // ckb_rotateImage
            // 
            this.ckb_rotateImage.BackColor = System.Drawing.Color.White;
            this.ckb_rotateImage.Checked = false;
            this.ckb_rotateImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_rotateImage.Location = new System.Drawing.Point(2, 440);
            this.ckb_rotateImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_rotateImage.Name = "ckb_rotateImage";
            this.ckb_rotateImage.Size = new System.Drawing.Size(59, 20);
            this.ckb_rotateImage.TabIndex = 127;
            this.ckb_rotateImage.TextStr = "旋转";
            this.ckb_rotateImage.CheckChanged += new Controls.DCheckChanged(this.ckb_rotateImage_CheckChanged);
            // 
            // ckb_RGBToGray
            // 
            this.ckb_RGBToGray.BackColor = System.Drawing.Color.White;
            this.ckb_RGBToGray.Checked = true;
            this.ckb_RGBToGray.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_RGBToGray.Location = new System.Drawing.Point(2, 416);
            this.ckb_RGBToGray.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_RGBToGray.Name = "ckb_RGBToGray";
            this.ckb_RGBToGray.Size = new System.Drawing.Size(97, 20);
            this.ckb_RGBToGray.TabIndex = 126;
            this.ckb_RGBToGray.TextStr = "彩图转灰度图";
            this.ckb_RGBToGray.CheckChanged += new Controls.DCheckChanged(this.ckb_RGBToGray_CheckChanged);
            // 
            // ckb_autoSwitch
            // 
            this.ckb_autoSwitch.BackColor = System.Drawing.Color.White;
            this.ckb_autoSwitch.Checked = true;
            this.ckb_autoSwitch.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoSwitch.Location = new System.Drawing.Point(2, 390);
            this.ckb_autoSwitch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoSwitch.Name = "ckb_autoSwitch";
            this.ckb_autoSwitch.Size = new System.Drawing.Size(97, 20);
            this.ckb_autoSwitch.TabIndex = 124;
            this.ckb_autoSwitch.TextStr = "自动切换";
            this.ckb_autoSwitch.CheckChanged += new Controls.DCheckChanged(this.ckb_autoSwitch_CheckChanged);
            // 
            // nud_rotateAngle
            // 
            this.nud_rotateAngle.BackColor = System.Drawing.Color.White;
            this.nud_rotateAngle.DecimalPlaces = 0;
            this.nud_rotateAngle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_rotateAngle.Incremeent = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nud_rotateAngle.Location = new System.Drawing.Point(59, 435);
            this.nud_rotateAngle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nud_rotateAngle.MaximumSize = new System.Drawing.Size(300, 28);
            this.nud_rotateAngle.MaxValue = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nud_rotateAngle.MinimumSize = new System.Drawing.Size(100, 28);
            this.nud_rotateAngle.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nud_rotateAngle.Name = "nud_rotateAngle";
            this.nud_rotateAngle.Size = new System.Drawing.Size(100, 28);
            this.nud_rotateAngle.TabIndex = 0;
            this.nud_rotateAngle.Value = 0D;
            this.nud_rotateAngle.Visible = false;
            // 
            // lbl_deg
            // 
            this.lbl_deg.AutoSize = true;
            this.lbl_deg.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_deg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbl_deg.Location = new System.Drawing.Point(157, 441);
            this.lbl_deg.Name = "lbl_deg";
            this.lbl_deg.Size = new System.Drawing.Size(31, 17);
            this.lbl_deg.TabIndex = 122;
            this.lbl_deg.Text = "deg";
            this.lbl_deg.Visible = false;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel4.Location = new System.Drawing.Point(7, 28);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(260, 2);
            this.panel4.TabIndex = 106;
            // 
            // pic_fromLocalDirectory
            // 
            this.pic_fromLocalDirectory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pic_fromLocalDirectory.Image = ((System.Drawing.Image)(resources.GetObject("pic_fromLocalDirectory.Image")));
            this.pic_fromLocalDirectory.Location = new System.Drawing.Point(180, 4);
            this.pic_fromLocalDirectory.Name = "pic_fromLocalDirectory";
            this.pic_fromLocalDirectory.Size = new System.Drawing.Size(20, 20);
            this.pic_fromLocalDirectory.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_fromLocalDirectory.TabIndex = 97;
            this.pic_fromLocalDirectory.TabStop = false;
            this.pic_fromLocalDirectory.Click += new System.EventHandler(this.pic_fromLocalDirectory_Click);
            // 
            // rdo_fromLocalDirectory
            // 
            this.rdo_fromLocalDirectory.AutoSize = true;
            this.rdo_fromLocalDirectory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdo_fromLocalDirectory.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdo_fromLocalDirectory.Location = new System.Drawing.Point(182, 2);
            this.rdo_fromLocalDirectory.Name = "rdo_fromLocalDirectory";
            this.rdo_fromLocalDirectory.Size = new System.Drawing.Size(55, 24);
            this.rdo_fromLocalDirectory.TabIndex = 96;
            this.rdo_fromLocalDirectory.TabStop = true;
            this.rdo_fromLocalDirectory.Text = "目录";
            this.rdo_fromLocalDirectory.UseVisualStyleBackColor = true;
            this.rdo_fromLocalDirectory.CheckedChanged += new System.EventHandler(this.rdo_fromLocalDirectory_CheckedChanged);
            // 
            // pic_fromLocalFile
            // 
            this.pic_fromLocalFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pic_fromLocalFile.Image = ((System.Drawing.Image)(resources.GetObject("pic_fromLocalFile.Image")));
            this.pic_fromLocalFile.Location = new System.Drawing.Point(112, 4);
            this.pic_fromLocalFile.Name = "pic_fromLocalFile";
            this.pic_fromLocalFile.Size = new System.Drawing.Size(20, 20);
            this.pic_fromLocalFile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_fromLocalFile.TabIndex = 95;
            this.pic_fromLocalFile.TabStop = false;
            this.pic_fromLocalFile.Click += new System.EventHandler(this.pic_fromLocalFile_Click);
            // 
            // pic_fromDevice
            // 
            this.pic_fromDevice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pic_fromDevice.Image = ((System.Drawing.Image)(resources.GetObject("pic_fromDevice.Image")));
            this.pic_fromDevice.Location = new System.Drawing.Point(41, 4);
            this.pic_fromDevice.Name = "pic_fromDevice";
            this.pic_fromDevice.Size = new System.Drawing.Size(20, 20);
            this.pic_fromDevice.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_fromDevice.TabIndex = 94;
            this.pic_fromDevice.TabStop = false;
            this.pic_fromDevice.Click += new System.EventHandler(this.pic_fromDevice_Click);
            // 
            // radio_FromLocalFile
            // 
            this.radio_FromLocalFile.AutoSize = true;
            this.radio_FromLocalFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radio_FromLocalFile.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radio_FromLocalFile.Location = new System.Drawing.Point(115, 2);
            this.radio_FromLocalFile.Name = "radio_FromLocalFile";
            this.radio_FromLocalFile.Size = new System.Drawing.Size(55, 24);
            this.radio_FromLocalFile.TabIndex = 93;
            this.radio_FromLocalFile.TabStop = true;
            this.radio_FromLocalFile.Text = "文件";
            this.radio_FromLocalFile.UseVisualStyleBackColor = true;
            this.radio_FromLocalFile.CheckedChanged += new System.EventHandler(this.radio_FromLocalFile_CheckedChanged);
            // 
            // rdo_fromDevice
            // 
            this.rdo_fromDevice.AutoSize = true;
            this.rdo_fromDevice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdo_fromDevice.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdo_fromDevice.Location = new System.Drawing.Point(45, 2);
            this.rdo_fromDevice.Name = "rdo_fromDevice";
            this.rdo_fromDevice.Size = new System.Drawing.Size(55, 24);
            this.rdo_fromDevice.TabIndex = 92;
            this.rdo_fromDevice.TabStop = true;
            this.rdo_fromDevice.Text = "相机";
            this.rdo_fromDevice.UseVisualStyleBackColor = true;
            this.rdo_fromDevice.CheckedChanged += new System.EventHandler(this.rdo_fromDevice_CheckedChanged);
            // 
            // pnl_formPanel
            // 
            this.pnl_formPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl_formPanel.BackColor = System.Drawing.Color.White;
            this.pnl_formPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnl_formPanel.Location = new System.Drawing.Point(2, 30);
            this.pnl_formPanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pnl_formPanel.Name = "pnl_formPanel";
            this.pnl_formPanel.Size = new System.Drawing.Size(270, 450);
            this.pnl_formPanel.TabIndex = 88;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btn_close);
            this.panel6.Controls.Add(this.btn_runTool);
            this.panel6.Controls.Add(this.lbl_runTime);
            this.panel6.Controls.Add(this.lbl_toolTip);
            this.panel6.Controls.Add(this.panel5);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 502);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(897, 59);
            this.panel6.TabIndex = 90;
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
            this.btn_runTool.Location = new System.Drawing.Point(716, 18);
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
            // lbl_runTime
            // 
            this.lbl_runTime.AutoSize = true;
            this.lbl_runTime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_runTime.Location = new System.Drawing.Point(12, 17);
            this.lbl_runTime.Name = "lbl_runTime";
            this.lbl_runTime.Size = new System.Drawing.Size(68, 17);
            this.lbl_runTime.TabIndex = 115;
            this.lbl_runTime.Text = "耗时：0ms";
            // 
            // lbl_toolTip
            // 
            this.lbl_toolTip.AutoSize = true;
            this.lbl_toolTip.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_toolTip.Location = new System.Drawing.Point(12, 34);
            this.lbl_toolTip.Name = "lbl_toolTip";
            this.lbl_toolTip.Size = new System.Drawing.Size(56, 17);
            this.lbl_toolTip.TabIndex = 114;
            this.lbl_toolTip.Text = "状态：无";
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
            this.tsb_SDKInfo});
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
            // Frm_AcqDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(907, 592);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(380, 120);
            this.Name = "Frm_AcqDevice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDK_海康威视";
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.cnt_rightClickMenu.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_fromLocalDirectory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_fromLocalFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_fromDevice)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        public ChoiceTech.Halcon.Control.HWindow_Final hWindow_Final1;
        public System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        public System.Windows.Forms.PictureBox pic_fromLocalDirectory;
        public System.Windows.Forms.RadioButton rdo_fromLocalDirectory;
        public System.Windows.Forms.PictureBox pic_fromLocalFile;
        public System.Windows.Forms.PictureBox pic_fromDevice;
        public System.Windows.Forms.RadioButton radio_FromLocalFile;
        public System.Windows.Forms.RadioButton rdo_fromDevice;
        public System.Windows.Forms.Panel pnl_formPanel;
        private System.Windows.Forms.Panel panel6;
        public System.Windows.Forms.Label lbl_toolTip;
        internal System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Panel panel5;
        internal System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ContextMenuStrip cnt_rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem 适应图像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全屏ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图像另存为ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 相机实时ToolStripMenuItem;
        public System.Windows.Forms.Label lbl_runTime;
        private System.Windows.Forms.ToolStripButton tsb_SDKInfo;
        private System.Windows.Forms.Label lbl_deg;
        public CNumericUpDown nud_rotateAngle;
        public CCheckBox ckb_autoSwitch;
        public CCheckBox ckb_RGBToGray;
        public CCheckBox ckb_rotateImage;
    }
}
