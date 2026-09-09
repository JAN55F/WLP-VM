namespace VMPro
{
    partial class Frm_TCPServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_TCPServer));
            this.lbx_connectedList = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.断开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.tbx_log = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lnk_clearLog = new System.Windows.Forms.LinkLabel();
            this.tbx_severIP = new Controls.CTextBox();
            this.tbx_severPort = new Controls.CTextBox();
            this.cbx_connectedList = new Controls.CComboBox();
            this.tbx_sendMessage = new Controls.CTextBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.tbx_severName = new Controls.CTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_listen = new Controls.CButton();
            this.ckb_autoDisconnectBeforeClose = new Controls.CCheckBox();
            this.ckb_autoConnectAfterStart = new Controls.CCheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_send = new Controls.CButton();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbx_connectedList
            // 
            this.lbx_connectedList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbx_connectedList.ContextMenuStrip = this.contextMenuStrip1;
            this.lbx_connectedList.ForeColor = System.Drawing.Color.Green;
            this.lbx_connectedList.FormattingEnabled = true;
            this.lbx_connectedList.ItemHeight = 17;
            this.lbx_connectedList.Location = new System.Drawing.Point(6, 240);
            this.lbx_connectedList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lbx_connectedList.Name = "lbx_connectedList";
            this.lbx_connectedList.Size = new System.Drawing.Size(195, 138);
            this.lbx_connectedList.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.断开ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // 断开ToolStripMenuItem
            // 
            this.断开ToolStripMenuItem.Name = "断开ToolStripMenuItem";
            this.断开ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.断开ToolStripMenuItem.Text = "断开";
            this.断开ToolStripMenuItem.Click += new System.EventHandler(this.断开ToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 222);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "已连接列表";
            // 
            // tbx_log
            // 
            this.tbx_log.BackColor = System.Drawing.Color.White;
            this.tbx_log.Location = new System.Drawing.Point(233, 44);
            this.tbx_log.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_log.Multiline = true;
            this.tbx_log.Name = "tbx_log";
            this.tbx_log.ReadOnly = true;
            this.tbx_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbx_log.Size = new System.Drawing.Size(293, 300);
            this.tbx_log.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(3, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "端口号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(3, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "IP地址：";
            // 
            // lnk_clearLog
            // 
            this.lnk_clearLog.AutoSize = true;
            this.lnk_clearLog.Location = new System.Drawing.Point(230, 23);
            this.lnk_clearLog.Name = "lnk_clearLog";
            this.lnk_clearLog.Size = new System.Drawing.Size(32, 17);
            this.lnk_clearLog.TabIndex = 25;
            this.lnk_clearLog.TabStop = true;
            this.lnk_clearLog.Text = "清空";
            this.lnk_clearLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_clearLog_LinkClicked);
            // 
            // tbx_severIP
            // 
            this.tbx_severIP.BackColor = System.Drawing.Color.White;
            this.tbx_severIP.DefaultText = "请输入IP地址";
            this.tbx_severIP.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_severIP.Location = new System.Drawing.Point(52, 62);
            this.tbx_severIP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_severIP.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_severIP.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_severIP.Name = "tbx_severIP";
            this.tbx_severIP.PasswordChar = false;
            this.tbx_severIP.Size = new System.Drawing.Size(151, 22);
            this.tbx_severIP.TabIndex = 26;
            this.tbx_severIP.TextStr = "";
            this.tbx_severIP.TextStrChanged += new Controls.DTextStrChanged(this.tbx_severIP_TextStrChanged);
            // 
            // tbx_severPort
            // 
            this.tbx_severPort.BackColor = System.Drawing.Color.White;
            this.tbx_severPort.DefaultText = "请输入端口号";
            this.tbx_severPort.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_severPort.Location = new System.Drawing.Point(52, 87);
            this.tbx_severPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_severPort.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_severPort.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_severPort.Name = "tbx_severPort";
            this.tbx_severPort.PasswordChar = false;
            this.tbx_severPort.Size = new System.Drawing.Size(151, 22);
            this.tbx_severPort.TabIndex = 27;
            this.tbx_severPort.TextStr = "";
            this.tbx_severPort.TextStrChanged += new Controls.DTextStrChanged(this.tbx_severPort_TextStrChanged);
            // 
            // cbx_connectedList
            // 
            this.cbx_connectedList.BackColor = System.Drawing.Color.White;
            this.cbx_connectedList.CanEdit = false;
            this.cbx_connectedList.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_connectedList.Items = new string[0];
            this.cbx_connectedList.Location = new System.Drawing.Point(365, 17);
            this.cbx_connectedList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_connectedList.Name = "cbx_connectedList";
            this.cbx_connectedList.SelectedIndex = -1;
            this.cbx_connectedList.Size = new System.Drawing.Size(161, 26);
            this.cbx_connectedList.TabIndex = 28;
            this.cbx_connectedList.TextStr = "";
            // 
            // tbx_sendMessage
            // 
            this.tbx_sendMessage.BackColor = System.Drawing.Color.White;
            this.tbx_sendMessage.DefaultText = "请输入要发送的信息";
            this.tbx_sendMessage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_sendMessage.Location = new System.Drawing.Point(233, 359);
            this.tbx_sendMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_sendMessage.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_sendMessage.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_sendMessage.Name = "tbx_sendMessage";
            this.tbx_sendMessage.PasswordChar = false;
            this.tbx_sendMessage.Size = new System.Drawing.Size(230, 22);
            this.tbx_sendMessage.TabIndex = 29;
            this.tbx_sendMessage.TextStr = "";
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel8.Location = new System.Drawing.Point(217, 7);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(1, 390);
            this.panel8.TabIndex = 154;
            // 
            // tbx_severName
            // 
            this.tbx_severName.BackColor = System.Drawing.Color.White;
            this.tbx_severName.DefaultText = "请输入客户端名称";
            this.tbx_severName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_severName.Location = new System.Drawing.Point(52, 18);
            this.tbx_severName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_severName.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_severName.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_severName.Name = "tbx_severName";
            this.tbx_severName.PasswordChar = false;
            this.tbx_severName.Size = new System.Drawing.Size(151, 22);
            this.tbx_severName.TabIndex = 156;
            this.tbx_severName.TextStr = "";
            this.tbx_severName.TextStrChanged += new Controls.DTextStrChanged(this.tbx_severName_TextStrChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(3, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 155;
            this.label4.Text = "名   称：";
            // 
            // btn_listen
            // 
            this.btn_listen.BackColor = System.Drawing.Color.White;
            this.btn_listen.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_listen.Location = new System.Drawing.Point(136, 115);
            this.btn_listen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_listen.Name = "btn_listen";
            this.btn_listen.Size = new System.Drawing.Size(67, 28);
            this.btn_listen.TabIndex = 157;
            this.btn_listen.TextStr = "开始监听";
            this.btn_listen.Clicked += new Controls.DClicked(this.btn_listen_Clicked);
            // 
            // ckb_autoDisconnectBeforeClose
            // 
            this.ckb_autoDisconnectBeforeClose.BackColor = System.Drawing.Color.White;
            this.ckb_autoDisconnectBeforeClose.Checked = false;
            this.ckb_autoDisconnectBeforeClose.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoDisconnectBeforeClose.Location = new System.Drawing.Point(70, 182);
            this.ckb_autoDisconnectBeforeClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoDisconnectBeforeClose.Name = "ckb_autoDisconnectBeforeClose";
            this.ckb_autoDisconnectBeforeClose.Size = new System.Drawing.Size(170, 20);
            this.ckb_autoDisconnectBeforeClose.TabIndex = 159;
            this.ckb_autoDisconnectBeforeClose.TextStr = "程序关闭前自动断开";
            this.ckb_autoDisconnectBeforeClose.CheckChanged += new Controls.DCheckChanged(this.ckb_autoDisconnectBeforeClose_CheckChanged);
            // 
            // ckb_autoConnectAfterStart
            // 
            this.ckb_autoConnectAfterStart.BackColor = System.Drawing.Color.White;
            this.ckb_autoConnectAfterStart.Checked = false;
            this.ckb_autoConnectAfterStart.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoConnectAfterStart.Location = new System.Drawing.Point(70, 160);
            this.ckb_autoConnectAfterStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoConnectAfterStart.Name = "ckb_autoConnectAfterStart";
            this.ckb_autoConnectAfterStart.Size = new System.Drawing.Size(170, 20);
            this.ckb_autoConnectAfterStart.TabIndex = 158;
            this.ckb_autoConnectAfterStart.TextStr = "程序启动后自动监听";
            this.ckb_autoConnectAfterStart.CheckChanged += new Controls.DCheckChanged(this.ckb_autoConnectAfterStart_CheckChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(319, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 160;
            this.label5.Text = "客户端：";
            // 
            // btn_send
            // 
            this.btn_send.BackColor = System.Drawing.Color.White;
            this.btn_send.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_send.Location = new System.Drawing.Point(471, 357);
            this.btn_send.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(55, 24);
            this.btn_send.TabIndex = 161;
            this.btn_send.TextStr = "发送";
            this.btn_send.Clicked += new Controls.DClicked(this.btn_send_Clicked);
            // 
            // Frm_TCPServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(542, 415);
            this.Controls.Add(this.btn_send);
            this.Controls.Add(this.cbx_connectedList);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btn_listen);
            this.Controls.Add(this.tbx_severName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.tbx_sendMessage);
            this.Controls.Add(this.tbx_severPort);
            this.Controls.Add(this.tbx_severIP);
            this.Controls.Add(this.lnk_clearLog);
            this.Controls.Add(this.tbx_log);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbx_connectedList);
            this.Controls.Add(this.ckb_autoDisconnectBeforeClose);
            this.Controls.Add(this.ckb_autoConnectAfterStart);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_TCPServer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TCP服务器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_TCPServer_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox tbx_log;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel lnk_clearLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 断开ToolStripMenuItem;
        public System.Windows.Forms.ListBox lbx_connectedList;
        private Controls.CTextBox tbx_severIP;
        private Controls.CTextBox tbx_severPort;
        internal Controls.CComboBox cbx_connectedList;
        private Controls.CTextBox tbx_sendMessage;
        private System.Windows.Forms.Panel panel8;
        private Controls.CTextBox tbx_severName;
        internal System.Windows.Forms.Label label4;
        internal Controls.CButton btn_listen;
        private Controls.CCheckBox ckb_autoDisconnectBeforeClose;
        private Controls.CCheckBox ckb_autoConnectAfterStart;
        internal System.Windows.Forms.Label label5;
        private Controls.CButton btn_send;
    }
}