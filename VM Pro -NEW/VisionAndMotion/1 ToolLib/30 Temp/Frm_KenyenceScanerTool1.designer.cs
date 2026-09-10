namespace VMPro
{
    partial class Frm_KenyenceScanerTool1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_KenyenceScanerTool1));
            this.ckb_shapeMatchToolEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_help = new System.Windows.Forms.ToolStripButton();
            this.btn_closePort = new System.Windows.Forms.Button();
            this.lnk_clear = new System.Windows.Forms.LinkLabel();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_send = new System.Windows.Forms.Button();
            this.tbx_sendMsg = new System.Windows.Forms.TextBox();
            this.tbx_output = new System.Windows.Forms.TextBox();
            this.lbl_statu = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_openPort = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbx_parityBit = new System.Windows.Forms.ComboBox();
            this.cbx_stopBit = new System.Windows.Forms.ComboBox();
            this.cbx_dataBit = new System.Windows.Forms.ComboBox();
            this.cbx_baudRate = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbx_portName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_runKenyenceScannerTool = new System.Windows.Forms.Button();
            this.btn_endScan = new System.Windows.Forms.Button();
            this.btn_startScan = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 1);
            // 
            // ckb_shapeMatchToolEnable
            // 
            this.ckb_shapeMatchToolEnable.AutoSize = true;
            this.ckb_shapeMatchToolEnable.BackColor = System.Drawing.Color.Transparent;
            this.ckb_shapeMatchToolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_shapeMatchToolEnable.Location = new System.Drawing.Point(474, 34);
            this.ckb_shapeMatchToolEnable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_shapeMatchToolEnable.Name = "ckb_shapeMatchToolEnable";
            this.ckb_shapeMatchToolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_shapeMatchToolEnable.TabIndex = 74;
            this.ckb_shapeMatchToolEnable.Text = "启用";
            this.ckb_shapeMatchToolEnable.UseVisualStyleBackColor = false;
            this.ckb_shapeMatchToolEnable.CheckedChanged += new System.EventHandler(this.ckb_shapeMatchToolNotRun_CheckedChanged);
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
            this.toolStrip1.Size = new System.Drawing.Size(458, 25);
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
            // toolStripButton1
            // 
            this.toolStripButton1.AutoSize = false;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(25, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripButton1.ToolTipText = "运行流程";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
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
            // 
            // btn_closePort
            // 
            this.btn_closePort.Location = new System.Drawing.Point(117, 277);
            this.btn_closePort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_closePort.Name = "btn_closePort";
            this.btn_closePort.Size = new System.Drawing.Size(87, 30);
            this.btn_closePort.TabIndex = 98;
            this.btn_closePort.Text = "关闭串口";
            this.btn_closePort.UseVisualStyleBackColor = true;
            this.btn_closePort.Click += new System.EventHandler(this.btn_closePort_Click);
            // 
            // lnk_clear
            // 
            this.lnk_clear.AutoSize = true;
            this.lnk_clear.Location = new System.Drawing.Point(172, 342);
            this.lnk_clear.Name = "lnk_clear";
            this.lnk_clear.Size = new System.Drawing.Size(32, 17);
            this.lnk_clear.TabIndex = 97;
            this.lnk_clear.TabStop = true;
            this.lnk_clear.Text = "清空";
            this.lnk_clear.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_clear_LinkClicked);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(216, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 17);
            this.label9.TabIndex = 96;
            this.label9.Text = "通讯记录";
            // 
            // btn_send
            // 
            this.btn_send.Location = new System.Drawing.Point(454, 257);
            this.btn_send.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(68, 43);
            this.btn_send.TabIndex = 95;
            this.btn_send.Text = "发送";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.btn_send_Click);
            // 
            // tbx_sendMsg
            // 
            this.tbx_sendMsg.Location = new System.Drawing.Point(219, 257);
            this.tbx_sendMsg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_sendMsg.Multiline = true;
            this.tbx_sendMsg.Name = "tbx_sendMsg";
            this.tbx_sendMsg.Size = new System.Drawing.Size(229, 43);
            this.tbx_sendMsg.TabIndex = 94;
            // 
            // tbx_output
            // 
            this.tbx_output.Location = new System.Drawing.Point(219, 94);
            this.tbx_output.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_output.Multiline = true;
            this.tbx_output.Name = "tbx_output";
            this.tbx_output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbx_output.Size = new System.Drawing.Size(303, 157);
            this.tbx_output.TabIndex = 93;
            // 
            // lbl_statu
            // 
            this.lbl_statu.AutoSize = true;
            this.lbl_statu.ForeColor = System.Drawing.Color.Red;
            this.lbl_statu.Location = new System.Drawing.Point(47, 342);
            this.lbl_statu.Name = "lbl_statu";
            this.lbl_statu.Size = new System.Drawing.Size(44, 17);
            this.lbl_statu.TabIndex = 92;
            this.lbl_statu.Text = "未打开";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 342);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 17);
            this.label6.TabIndex = 91;
            this.label6.Text = "状态：";
            // 
            // btn_openPort
            // 
            this.btn_openPort.Location = new System.Drawing.Point(18, 277);
            this.btn_openPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_openPort.Name = "btn_openPort";
            this.btn_openPort.Size = new System.Drawing.Size(87, 30);
            this.btn_openPort.TabIndex = 90;
            this.btn_openPort.Text = "打开串口";
            this.btn_openPort.UseVisualStyleBackColor = true;
            this.btn_openPort.Click += new System.EventHandler(this.btn_openPort_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbx_parityBit);
            this.groupBox1.Controls.Add(this.cbx_stopBit);
            this.groupBox1.Controls.Add(this.cbx_dataBit);
            this.groupBox1.Controls.Add(this.cbx_baudRate);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbx_portName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(18, 67);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(186, 202);
            this.groupBox1.TabIndex = 89;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "串口设置";
            // 
            // cbx_parityBit
            // 
            this.cbx_parityBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_parityBit.FormattingEnabled = true;
            this.cbx_parityBit.Location = new System.Drawing.Point(67, 185);
            this.cbx_parityBit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_parityBit.Name = "cbx_parityBit";
            this.cbx_parityBit.Size = new System.Drawing.Size(99, 25);
            this.cbx_parityBit.TabIndex = 13;
            this.cbx_parityBit.SelectedIndexChanged += new System.EventHandler(this.cbx_parityBit_SelectedIndexChanged);
            // 
            // cbx_stopBit
            // 
            this.cbx_stopBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_stopBit.FormattingEnabled = true;
            this.cbx_stopBit.Location = new System.Drawing.Point(67, 153);
            this.cbx_stopBit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_stopBit.Name = "cbx_stopBit";
            this.cbx_stopBit.Size = new System.Drawing.Size(99, 25);
            this.cbx_stopBit.TabIndex = 12;
            this.cbx_stopBit.SelectedIndexChanged += new System.EventHandler(this.cbx_stopBit_SelectedIndexChanged);
            // 
            // cbx_dataBit
            // 
            this.cbx_dataBit.FormattingEnabled = true;
            this.cbx_dataBit.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8"});
            this.cbx_dataBit.Location = new System.Drawing.Point(67, 121);
            this.cbx_dataBit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_dataBit.Name = "cbx_dataBit";
            this.cbx_dataBit.Size = new System.Drawing.Size(99, 25);
            this.cbx_dataBit.TabIndex = 11;
            this.cbx_dataBit.Text = "8";
            this.cbx_dataBit.SelectedIndexChanged += new System.EventHandler(this.tbx_dataBit_SelectedIndexChanged);
            // 
            // cbx_baudRate
            // 
            this.cbx_baudRate.FormattingEnabled = true;
            this.cbx_baudRate.Items.AddRange(new object[] {
            "4800",
            "9600",
            "10004",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cbx_baudRate.Location = new System.Drawing.Point(67, 89);
            this.cbx_baudRate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_baudRate.Name = "cbx_baudRate";
            this.cbx_baudRate.Size = new System.Drawing.Size(99, 25);
            this.cbx_baudRate.TabIndex = 10;
            this.cbx_baudRate.Text = "9600";
            this.cbx_baudRate.SelectedIndexChanged += new System.EventHandler(this.cbx_baudRate_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "停止位：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "效验位：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "数据位：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "波特率：";
            // 
            // cbx_portName
            // 
            this.cbx_portName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_portName.FormattingEnabled = true;
            this.cbx_portName.Location = new System.Drawing.Point(67, 57);
            this.cbx_portName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_portName.Name = "cbx_portName";
            this.cbx_portName.Size = new System.Drawing.Size(99, 25);
            this.cbx_portName.TabIndex = 1;
            this.cbx_portName.SelectedIndexChanged += new System.EventHandler(this.cbx_portName_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "端口：";
            // 
            // btn_runKenyenceScannerTool
            // 
            this.btn_runKenyenceScannerTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runKenyenceScannerTool.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runKenyenceScannerTool.Location = new System.Drawing.Point(424, 314);
            this.btn_runKenyenceScannerTool.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_runKenyenceScannerTool.Name = "btn_runKenyenceScannerTool";
            this.btn_runKenyenceScannerTool.Size = new System.Drawing.Size(98, 45);
            this.btn_runKenyenceScannerTool.TabIndex = 99;
            this.btn_runKenyenceScannerTool.Text = "运行";
            this.btn_runKenyenceScannerTool.UseVisualStyleBackColor = true;
            this.btn_runKenyenceScannerTool.Click += new System.EventHandler(this.btn_runKenyenceScannerTool_Click);
            // 
            // btn_endScan
            // 
            this.btn_endScan.Location = new System.Drawing.Point(117, 308);
            this.btn_endScan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_endScan.Name = "btn_endScan";
            this.btn_endScan.Size = new System.Drawing.Size(87, 30);
            this.btn_endScan.TabIndex = 101;
            this.btn_endScan.Text = "结束扫码";
            this.btn_endScan.UseVisualStyleBackColor = true;
            this.btn_endScan.Click += new System.EventHandler(this.btn_endScan_Click);
            // 
            // btn_startScan
            // 
            this.btn_startScan.Location = new System.Drawing.Point(18, 308);
            this.btn_startScan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_startScan.Name = "btn_startScan";
            this.btn_startScan.Size = new System.Drawing.Size(87, 30);
            this.btn_startScan.TabIndex = 100;
            this.btn_startScan.Text = "开始扫码";
            this.btn_startScan.UseVisualStyleBackColor = true;
            this.btn_startScan.Click += new System.EventHandler(this.btn_startScan_Click);
            // 
            // Frm_KenyenceScanerTool1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(539, 371);
            this.Controls.Add(this.btn_endScan);
            this.Controls.Add(this.btn_startScan);
            this.Controls.Add(this.btn_runKenyenceScannerTool);
            this.Controls.Add(this.btn_closePort);
            this.Controls.Add(this.lnk_clear);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btn_send);
            this.Controls.Add(this.tbx_sendMsg);
            this.Controls.Add(this.tbx_output);
            this.Controls.Add(this.lbl_statu);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btn_openPort);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ckb_shapeMatchToolEnable);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(605, 430);
            this.Name = "Frm_KenyenceScanerTool1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模板匹配工具";
            this.Load += new System.EventHandler(this.Frm_KenyenceScanerTool_Load);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.ckb_shapeMatchToolEnable, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.btn_openPort, 0);
            this.Controls.SetChildIndex(this.label6, 0);
            this.Controls.SetChildIndex(this.lbl_statu, 0);
            this.Controls.SetChildIndex(this.tbx_output, 0);
            this.Controls.SetChildIndex(this.tbx_sendMsg, 0);
            this.Controls.SetChildIndex(this.btn_send, 0);
            this.Controls.SetChildIndex(this.label9, 0);
            this.Controls.SetChildIndex(this.lnk_clear, 0);
            this.Controls.SetChildIndex(this.btn_closePort, 0);
            this.Controls.SetChildIndex(this.btn_runKenyenceScannerTool, 0);
            this.Controls.SetChildIndex(this.btn_startScan, 0);
            this.Controls.SetChildIndex(this.btn_endScan, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox ckb_shapeMatchToolEnable;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_help;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button btn_closePort;
        private System.Windows.Forms.LinkLabel lnk_clear;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.TextBox tbx_sendMsg;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_openPort;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tbx_output;
        public System.Windows.Forms.Label lbl_statu;
        public System.Windows.Forms.Button btn_runKenyenceScannerTool;
        public System.Windows.Forms.ComboBox cbx_portName;
        public System.Windows.Forms.ComboBox cbx_parityBit;
        public System.Windows.Forms.ComboBox cbx_stopBit;
        public System.Windows.Forms.ComboBox cbx_dataBit;
        public System.Windows.Forms.ComboBox cbx_baudRate;
        private System.Windows.Forms.Button btn_endScan;
        private System.Windows.Forms.Button btn_startScan;
    }
}