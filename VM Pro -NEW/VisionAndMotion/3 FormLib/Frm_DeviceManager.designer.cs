namespace VMPro
{
    partial class Frm_DeviceManager
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbl_tip = new System.Windows.Forms.Label();
            this.btn_close = new System.Windows.Forms.Button();
            this.pnl_formPnl = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbo_deviceType = new System.Windows.Forms.ComboBox();
            this.btn_addDevice = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dgv_deviceList = new System.Windows.Forms.DataGridView();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewImageColumn();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pLC通讯ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TCPIP服务端ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TCPIP客户端ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全屏显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.扫码枪ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.奥普特光源控制器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.位移传感器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.压力控制器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_deviceList)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Location = new System.Drawing.Point(10, 6);
            this.lbl_title.Size = new System.Drawing.Size(68, 17);
            this.lbl_title.Text = "通讯及设备";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.lbl_tip);
            this.panel3.Controls.Add(this.btn_close);
            this.panel3.Controls.Add(this.pnl_formPnl);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Location = new System.Drawing.Point(2, 26);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(753, 478);
            this.panel3.TabIndex = 117;
            // 
            // lbl_tip
            // 
            this.lbl_tip.AutoSize = true;
            this.lbl_tip.ForeColor = System.Drawing.Color.Black;
            this.lbl_tip.Location = new System.Drawing.Point(220, 445);
            this.lbl_tip.Name = "lbl_tip";
            this.lbl_tip.Size = new System.Drawing.Size(80, 17);
            this.lbl_tip.TabIndex = 3;
            this.lbl_tip.Text = "提示：请选择设备";
            // 
            // btn_close
            // 
            this.btn_close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.btn_close.FlatAppearance.BorderSize = 0;
            this.btn_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_close.ForeColor = System.Drawing.Color.White;
            this.btn_close.Location = new System.Drawing.Point(678, 438);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(67, 31);
            this.btn_close.TabIndex = 2;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = false;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // pnl_formPnl
            // 
            this.pnl_formPnl.BackColor = System.Drawing.Color.White;
            this.pnl_formPnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_formPnl.Location = new System.Drawing.Point(220, 14);
            this.pnl_formPnl.Name = "pnl_formPnl";
            this.pnl_formPnl.Size = new System.Drawing.Size(525, 415);
            this.pnl_formPnl.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.cbo_deviceType);
            this.panel2.Controls.Add(this.btn_addDevice);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.dgv_deviceList);
            this.panel2.Location = new System.Drawing.Point(10, 14);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(198, 455);
            this.panel2.TabIndex = 0;
            // 
            // cbo_deviceType
            // 
            this.cbo_deviceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_deviceType.FormattingEnabled = true;
            this.cbo_deviceType.Location = new System.Drawing.Point(8, 418);
            this.cbo_deviceType.Name = "cbo_deviceType";
            this.cbo_deviceType.Size = new System.Drawing.Size(102, 25);
            this.cbo_deviceType.TabIndex = 3;
            // 
            // btn_addDevice
            // 
            this.btn_addDevice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.btn_addDevice.FlatAppearance.BorderSize = 0;
            this.btn_addDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_addDevice.ForeColor = System.Drawing.Color.White;
            this.btn_addDevice.Location = new System.Drawing.Point(116, 417);
            this.btn_addDevice.Name = "btn_addDevice";
            this.btn_addDevice.Size = new System.Drawing.Size(68, 28);
            this.btn_addDevice.TabIndex = 2;
            this.btn_addDevice.Text = "添加";
            this.btn_addDevice.UseVisualStyleBackColor = false;
            this.btn_addDevice.Click += new System.EventHandler(this.btn_addDevice_Click);
            this.btn_addDevice.MouseEnter += new System.EventHandler(this.btn_addDevice_MouseEnter);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 28);
            this.label1.TabIndex = 1;
            this.label1.Text = "通讯及设备列表";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgv_deviceList
            // 
            this.dgv_deviceList.AllowUserToAddRows = false;
            this.dgv_deviceList.AllowUserToDeleteRows = false;
            this.dgv_deviceList.AllowUserToResizeColumns = false;
            this.dgv_deviceList.AllowUserToResizeRows = false;
            this.dgv_deviceList.BackgroundColor = System.Drawing.Color.White;
            this.dgv_deviceList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_deviceList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_deviceList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.Column1,
            this.Column3});
            this.dgv_deviceList.ContextMenuStrip = this.contextMenuStrip2;
            this.dgv_deviceList.Location = new System.Drawing.Point(8, 36);
            this.dgv_deviceList.MultiSelect = false;
            this.dgv_deviceList.Name = "dgv_deviceList";
            this.dgv_deviceList.ReadOnly = true;
            this.dgv_deviceList.RowHeadersVisible = false;
            this.dgv_deviceList.RowTemplate.Height = 28;
            this.dgv_deviceList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_deviceList.Size = new System.Drawing.Size(178, 372);
            this.dgv_deviceList.TabIndex = 0;
            this.dgv_deviceList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_deviceList_CellClick);
            this.dgv_deviceList.SelectionChanged += new System.EventHandler(this.dgv_deviceList_SelectionChanged);
            // 
            // Column2
            // 
            this.Column2.HeaderText = "名称";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 92;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "类型";
            this.Column1.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 38;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "状态";
            this.Column3.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 38;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除toolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(101, 26);
            // 
            // 删除toolStripMenuItem
            // 
            this.删除toolStripMenuItem.Name = "删除toolStripMenuItem";
            this.删除toolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.删除toolStripMenuItem.Text = "删除";
            this.删除toolStripMenuItem.Click += new System.EventHandler(this.删除toolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pLC通讯ToolStripMenuItem,
            this.TCPIP服务端ToolStripMenuItem,
            this.TCPIP客户端ToolStripMenuItem,
            this.全屏显示ToolStripMenuItem,
            this.扫码枪ToolStripMenuItem,
            this.奥普特光源控制器ToolStripMenuItem,
            this.位移传感器ToolStripMenuItem,
            this.压力控制器ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(151, 180);
            // 
            // pLC通讯ToolStripMenuItem
            // 
            this.pLC通讯ToolStripMenuItem.Name = "pLC通讯ToolStripMenuItem";
            this.pLC通讯ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.pLC通讯ToolStripMenuItem.Text = "PLC 通讯";
            this.pLC通讯ToolStripMenuItem.Click += new System.EventHandler(this.pLC通讯ToolStripMenuItem_Click);
            // 
            // TCPIP服务端ToolStripMenuItem
            // 
            this.TCPIP服务端ToolStripMenuItem.Name = "TCPIP服务端ToolStripMenuItem";
            this.TCPIP服务端ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.TCPIP服务端ToolStripMenuItem.Text = "TCP/IP 服务端";
            this.TCPIP服务端ToolStripMenuItem.Click += new System.EventHandler(this.TCPIP服务端ToolStripMenuItem_Click);
            // 
            // TCPIP客户端ToolStripMenuItem
            // 
            this.TCPIP客户端ToolStripMenuItem.Name = "TCPIP客户端ToolStripMenuItem";
            this.TCPIP客户端ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.TCPIP客户端ToolStripMenuItem.Text = "TCP/IP 客户端";
            this.TCPIP客户端ToolStripMenuItem.Click += new System.EventHandler(this.TCPIP客户端ToolStripMenuItem_Click);
            // 
            // 全屏显示ToolStripMenuItem
            // 
            this.全屏显示ToolStripMenuItem.Name = "全屏显示ToolStripMenuItem";
            this.全屏显示ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.全屏显示ToolStripMenuItem.Text = "串口通讯";
            this.全屏显示ToolStripMenuItem.Click += new System.EventHandler(this.全屏显示ToolStripMenuItem_Click);
            // 
            // 扫码枪ToolStripMenuItem
            // 
            this.扫码枪ToolStripMenuItem.Name = "扫码枪ToolStripMenuItem";
            this.扫码枪ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.扫码枪ToolStripMenuItem.Text = "扫码枪";
            this.扫码枪ToolStripMenuItem.Click += new System.EventHandler(this.扫码枪ToolStripMenuItem_Click);
            // 
            // 奥普特光源控制器ToolStripMenuItem
            // 
            this.奥普特光源控制器ToolStripMenuItem.Name = "奥普特光源控制器ToolStripMenuItem";
            this.奥普特光源控制器ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.奥普特光源控制器ToolStripMenuItem.Text = "光源控制器";
            this.奥普特光源控制器ToolStripMenuItem.Click += new System.EventHandler(this.奥普特光源控制器ToolStripMenuItem_Click);
            // 
            // 位移传感器ToolStripMenuItem
            // 
            this.位移传感器ToolStripMenuItem.Name = "位移传感器ToolStripMenuItem";
            this.位移传感器ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.位移传感器ToolStripMenuItem.Text = "位移传感器";
            // 
            // 压力控制器ToolStripMenuItem
            // 
            this.压力控制器ToolStripMenuItem.Name = "压力控制器ToolStripMenuItem";
            this.压力控制器ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.压力控制器ToolStripMenuItem.Text = "压力传感器";
            // 
            // Frm_DeviceManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(757, 506);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MaximumSize = new System.Drawing.Size(757, 506);
            this.MinimumSize = new System.Drawing.Size(757, 506);
            this.Name = "Frm_DeviceManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "通讯及设备";
            this.Controls.SetChildIndex(this.panel3, 0);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_deviceList)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 奥普特光源控制器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TCPIP服务端ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全屏显示ToolStripMenuItem;
        internal System.Windows.Forms.Button btn_addDevice;
        internal System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Panel pnl_formPnl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cbo_deviceType;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.DataGridView dgv_deviceList;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 删除toolStripMenuItem;
        public System.Windows.Forms.Label lbl_tip;
        private System.Windows.Forms.ToolStripMenuItem TCPIP客户端ToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewImageColumn Column1;
        private System.Windows.Forms.DataGridViewImageColumn Column3;
        private System.Windows.Forms.ToolStripMenuItem pLC通讯ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 扫码枪ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 位移传感器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 压力控制器ToolStripMenuItem;
    }
}
