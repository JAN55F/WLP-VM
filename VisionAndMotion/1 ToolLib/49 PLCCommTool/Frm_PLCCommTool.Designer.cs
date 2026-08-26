namespace VMPro
{
    partial class Frm_PLCCommTool
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_PLCCommTool));
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_device = new System.Windows.Forms.Label();
            this.cbo_device = new System.Windows.Forms.ComboBox();
            this.lbl_address = new System.Windows.Forms.Label();
            this.txb_address = new System.Windows.Forms.TextBox();
            this.lbl_dataType = new System.Windows.Forms.Label();
            this.cbo_dataType = new System.Windows.Forms.ComboBox();
            this.lbl_opMode = new System.Windows.Forms.Label();
            this.cbo_opMode = new System.Windows.Forms.ComboBox();
            this.lbl_writeValue = new System.Windows.Forms.Label();
            this.txb_writeValue = new System.Windows.Forms.TextBox();
            this.lbl_readResult = new System.Windows.Forms.Label();
            this.txb_readResult = new System.Windows.Forms.TextBox();
            this.lbl_expectValue = new System.Windows.Forms.Label();
            this.txb_expectValue = new System.Windows.Forms.TextBox();
            this.btn_stopWait = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.pic_onOff = new System.Windows.Forms.PictureBox();
            this.lbl_toolTip = new System.Windows.Forms.Label();
            this.lbl_runTime = new System.Windows.Forms.Label();
            this.btn_runTool = new System.Windows.Forms.Button();
            this.btn_runJob = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_onOff)).BeginInit();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1 (from base)
            // 
            this.pictureBox1.Image = global::VMPro.Properties.Resources.发送区;
            this.pictureBox1.Location = new System.Drawing.Point(5, 4);
            this.pictureBox1.Size = new System.Drawing.Size(18, 16);
            // 
            // button100 (from base)
            // 
            this.button100.Location = new System.Drawing.Point(496, 0);
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
            this.panel3.Size = new System.Drawing.Size(546, 370);
            this.panel3.TabIndex = 117;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(546, 370);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_resetTool});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(546, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // tsb_resetTool
            // 
            this.tsb_resetTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_resetTool.Image = global::VMPro.Properties.Resources.开;
            this.tsb_resetTool.Name = "tsb_resetTool";
            this.tsb_resetTool.Size = new System.Drawing.Size(23, 22);
            this.tsb_resetTool.Text = "启用工具";
            this.tsb_resetTool.ToolTipText = "启用工具";
            this.tsb_resetTool.Click += new System.EventHandler(this.tsb_resetTool_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbl_device);
            this.panel2.Controls.Add(this.cbo_device);
            this.panel2.Controls.Add(this.lbl_address);
            this.panel2.Controls.Add(this.txb_address);
            this.panel2.Controls.Add(this.lbl_dataType);
            this.panel2.Controls.Add(this.cbo_dataType);
            this.panel2.Controls.Add(this.lbl_opMode);
            this.panel2.Controls.Add(this.cbo_opMode);
            this.panel2.Controls.Add(this.lbl_writeValue);
            this.panel2.Controls.Add(this.txb_writeValue);
            this.panel2.Controls.Add(this.lbl_readResult);
            this.panel2.Controls.Add(this.txb_readResult);
            this.panel2.Controls.Add(this.lbl_expectValue);
            this.panel2.Controls.Add(this.txb_expectValue);
            this.panel2.Controls.Add(this.btn_stopWait);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(540, 274);
            this.panel2.TabIndex = 1;
            // 
            // lbl_device
            // 
            this.lbl_device.AutoSize = true;
            this.lbl_device.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbl_device.Location = new System.Drawing.Point(10, 20);
            this.lbl_device.Name = "lbl_device";
            this.lbl_device.Size = new System.Drawing.Size(65, 17);
            this.lbl_device.Text = "PLC设备：";
            // 
            // cbo_device
            // 
            this.cbo_device.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_device.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.cbo_device.Location = new System.Drawing.Point(90, 17);
            this.cbo_device.Name = "cbo_device";
            this.cbo_device.Size = new System.Drawing.Size(200, 25);
            this.cbo_device.TabIndex = 1;
            this.cbo_device.SelectedIndexChanged += new System.EventHandler(this.cbo_device_SelectedIndexChanged);
            // 
            // lbl_address
            // 
            this.lbl_address.AutoSize = true;
            this.lbl_address.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbl_address.Location = new System.Drawing.Point(10, 57);
            this.lbl_address.Name = "lbl_address";
            this.lbl_address.Size = new System.Drawing.Size(53, 17);
            this.lbl_address.Text = "操作地址：";
            // 
            // txb_address
            // 
            this.txb_address.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txb_address.Location = new System.Drawing.Point(90, 54);
            this.txb_address.Name = "txb_address";
            this.txb_address.Size = new System.Drawing.Size(200, 23);
            this.txb_address.TabIndex = 2;
            this.txb_address.TextChanged += new System.EventHandler(this.txb_address_TextChanged);
            // 
            // lbl_dataType
            // 
            this.lbl_dataType.AutoSize = true;
            this.lbl_dataType.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbl_dataType.Location = new System.Drawing.Point(10, 95);
            this.lbl_dataType.Name = "lbl_dataType";
            this.lbl_dataType.Size = new System.Drawing.Size(53, 17);
            this.lbl_dataType.Text = "数据类型：";
            // 
            // cbo_dataType
            // 
            this.cbo_dataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_dataType.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.cbo_dataType.Location = new System.Drawing.Point(90, 92);
            this.cbo_dataType.Name = "cbo_dataType";
            this.cbo_dataType.Size = new System.Drawing.Size(120, 25);
            this.cbo_dataType.TabIndex = 3;
            this.cbo_dataType.SelectedIndexChanged += new System.EventHandler(this.cbo_dataType_SelectedIndexChanged);
            // 
            // lbl_opMode
            // 
            this.lbl_opMode.AutoSize = true;
            this.lbl_opMode.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbl_opMode.Location = new System.Drawing.Point(230, 95);
            this.lbl_opMode.Name = "lbl_opMode";
            this.lbl_opMode.Size = new System.Drawing.Size(53, 17);
            this.lbl_opMode.Text = "操作模式：";
            // 
            // cbo_opMode
            // 
            this.cbo_opMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_opMode.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.cbo_opMode.Location = new System.Drawing.Point(310, 92);
            this.cbo_opMode.Name = "cbo_opMode";
            this.cbo_opMode.Size = new System.Drawing.Size(110, 25);
            this.cbo_opMode.TabIndex = 4;
            this.cbo_opMode.SelectedIndexChanged += new System.EventHandler(this.cbo_opMode_SelectedIndexChanged);
            // 
            // lbl_writeValue
            // 
            this.lbl_writeValue.AutoSize = true;
            this.lbl_writeValue.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbl_writeValue.ForeColor = System.Drawing.Color.DarkMagenta;
            this.lbl_writeValue.Location = new System.Drawing.Point(10, 133);
            this.lbl_writeValue.Name = "lbl_writeValue";
            this.lbl_writeValue.Size = new System.Drawing.Size(53, 17);
            this.lbl_writeValue.Text = "<--写入值：";
            this.lbl_writeValue.Visible = false;
            // 
            // txb_writeValue
            // 
            this.txb_writeValue.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txb_writeValue.Location = new System.Drawing.Point(90, 130);
            this.txb_writeValue.Name = "txb_writeValue";
            this.txb_writeValue.Size = new System.Drawing.Size(200, 23);
            this.txb_writeValue.TabIndex = 5;
            this.txb_writeValue.Visible = false;
            this.txb_writeValue.TextChanged += new System.EventHandler(this.txb_writeValue_TextChanged);
            // 
            // lbl_readResult
            // 
            this.lbl_readResult.AutoSize = true;
            this.lbl_readResult.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbl_readResult.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbl_readResult.Location = new System.Drawing.Point(10, 170);
            this.lbl_readResult.Name = "lbl_readResult";
            this.lbl_readResult.Size = new System.Drawing.Size(53, 17);
            this.lbl_readResult.Text = "-->读取值：";
            this.lbl_readResult.Visible = true;
            // 
            // txb_readResult
            // 
            this.txb_readResult.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txb_readResult.Location = new System.Drawing.Point(90, 167);
            this.txb_readResult.Name = "txb_readResult";
            this.txb_readResult.ReadOnly = true;
            this.txb_readResult.Size = new System.Drawing.Size(200, 23);
            this.txb_readResult.TabIndex = 6;
            this.txb_readResult.Visible = true;
            // 
            // lbl_expectValue
            // 
            this.lbl_expectValue.AutoSize = true;
            this.lbl_expectValue.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbl_expectValue.ForeColor = System.Drawing.Color.DarkBlue;
            this.lbl_expectValue.Location = new System.Drawing.Point(10, 207);
            this.lbl_expectValue.Name = "lbl_expectValue";
            this.lbl_expectValue.Size = new System.Drawing.Size(53, 17);
            this.lbl_expectValue.Text = "期望值：";
            // 
            // txb_expectValue
            // 
            this.txb_expectValue.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txb_expectValue.Location = new System.Drawing.Point(90, 204);
            this.txb_expectValue.Name = "txb_expectValue";
            this.txb_expectValue.Size = new System.Drawing.Size(200, 23);
            this.txb_expectValue.TabIndex = 7;
            this.txb_expectValue.TextChanged += new System.EventHandler(this.txb_expectValue_TextChanged);
            // 
            // btn_stopWait
            // 
            this.btn_stopWait.FlatAppearance.BorderSize = 0;
            this.btn_stopWait.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_stopWait.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_stopWait.ForeColor = System.Drawing.Color.White;
            this.btn_stopWait.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_stopWait.Location = new System.Drawing.Point(300, 202);
            this.btn_stopWait.Name = "btn_stopWait";
            this.btn_stopWait.Size = new System.Drawing.Size(75, 27);
            this.btn_stopWait.TabIndex = 8;
            this.btn_stopWait.Text = "放弃等待";
            this.btn_stopWait.UseVisualStyleBackColor = false;
            this.btn_stopWait.Click += new System.EventHandler(this.btn_stopWait_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.pic_onOff);
            this.panel6.Controls.Add(this.lbl_toolTip);
            this.panel6.Controls.Add(this.lbl_runTime);
            this.panel6.Controls.Add(this.btn_runTool);
            this.panel6.Controls.Add(this.btn_runJob);
            this.panel6.Controls.Add(this.btn_close);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 305);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(540, 59);
            this.panel6.TabIndex = 90;
            // 
            // pic_onOff
            // 
            this.pic_onOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pic_onOff.Image = global::VMPro.Properties.Resources.开;
            this.pic_onOff.Location = new System.Drawing.Point(8, 18);
            this.pic_onOff.Name = "pic_onOff";
            this.pic_onOff.Size = new System.Drawing.Size(36, 30);
            this.pic_onOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_onOff.TabIndex = 100;
            this.pic_onOff.TabStop = false;
            this.pic_onOff.Click += new System.EventHandler(this.pic_onOff_Click);
            // 
            // lbl_toolTip
            // 
            this.lbl_toolTip.AutoSize = true;
            this.lbl_toolTip.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbl_toolTip.Location = new System.Drawing.Point(54, 5);
            this.lbl_toolTip.Name = "lbl_toolTip";
            this.lbl_toolTip.Size = new System.Drawing.Size(120, 17);
            this.lbl_toolTip.Text = "状态：";
            // 
            // lbl_runTime
            // 
            this.lbl_runTime.AutoSize = true;
            this.lbl_runTime.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbl_runTime.Location = new System.Drawing.Point(54, 26);
            this.lbl_runTime.Name = "lbl_runTime";
            this.lbl_runTime.Size = new System.Drawing.Size(60, 17);
            this.lbl_runTime.Text = "耗时：";
            // 
            // btn_runTool
            // 
            this.btn_runTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_runTool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_runTool.FlatAppearance.BorderSize = 0;
            this.btn_runTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runTool.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_runTool.ForeColor = System.Drawing.Color.White;
            this.btn_runTool.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_runTool.Location = new System.Drawing.Point(295, 18);
            this.btn_runTool.Name = "btn_runTool";
            this.btn_runTool.Size = new System.Drawing.Size(65, 30);
            this.btn_runTool.TabIndex = 0;
            this.btn_runTool.Text = "运行工具";
            this.btn_runTool.UseVisualStyleBackColor = false;
            this.btn_runTool.Click += new System.EventHandler(this.btn_runTool_Click);
            // 
            // btn_runJob
            // 
            this.btn_runJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_runJob.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_runJob.FlatAppearance.BorderSize = 0;
            this.btn_runJob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runJob.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_runJob.ForeColor = System.Drawing.Color.White;
            this.btn_runJob.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_runJob.Location = new System.Drawing.Point(365, 18);
            this.btn_runJob.Name = "btn_runJob";
            this.btn_runJob.Size = new System.Drawing.Size(65, 30);
            this.btn_runJob.TabIndex = 110;
            this.btn_runJob.Text = "运行流程";
            this.btn_runJob.UseVisualStyleBackColor = false;
            this.btn_runJob.Click += new System.EventHandler(this.btn_runJob_Click);
            // 
            // btn_close
            // 
            this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_close.FlatAppearance.BorderSize = 0;
            this.btn_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_close.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_close.ForeColor = System.Drawing.Color.White;
            this.btn_close.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_close.Location = new System.Drawing.Point(435, 18);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(65, 30);
            this.btn_close.TabIndex = 111;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = false;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // Frm_PLCCommTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(550, 400);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(550, 400);
            this.Name = "Frm_PLCCommTool";
            this.Text = "PLC通讯";
            this.Load += new System.EventHandler(this.Frm_PLCCommTool_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_onOff)).EndInit();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbl_device;
        private System.Windows.Forms.ComboBox cbo_device;
        private System.Windows.Forms.Label lbl_address;
        private System.Windows.Forms.TextBox txb_address;
        private System.Windows.Forms.Label lbl_dataType;
        private System.Windows.Forms.ComboBox cbo_dataType;
        private System.Windows.Forms.Label lbl_opMode;
        private System.Windows.Forms.ComboBox cbo_opMode;
        private System.Windows.Forms.Label lbl_writeValue;
        private System.Windows.Forms.TextBox txb_writeValue;
        private System.Windows.Forms.Label lbl_readResult;
        private System.Windows.Forms.TextBox txb_readResult;
        private System.Windows.Forms.Label lbl_expectValue;
        private System.Windows.Forms.TextBox txb_expectValue;
        private System.Windows.Forms.Button btn_stopWait;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.PictureBox pic_onOff;
        private System.Windows.Forms.Label lbl_toolTip;
        private System.Windows.Forms.Label lbl_runTime;
        private System.Windows.Forms.Button btn_runTool;
        private System.Windows.Forms.Button btn_runJob;
        private System.Windows.Forms.Button btn_close;
    }
}
