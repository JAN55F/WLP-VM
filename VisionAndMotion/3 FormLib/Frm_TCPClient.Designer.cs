namespace VMPro
{
    partial class Frm_TCPClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_TCPClient));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbx_log = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lnk_clearLog = new System.Windows.Forms.LinkLabel();
            this.tbx_severIP = new Controls.CTextBox();
            this.tbx_severPort = new Controls.CTextBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.tbx_sendMessage = new Controls.CTextBox();
            this.tbx_clientName = new Controls.CTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_connect = new Controls.CButton();
            this.ckb_autoDisconnectBeforeClose = new Controls.CCheckBox();
            this.ckb_autoConnectAfterStart = new Controls.CCheckBox();
            this.btn_send = new Controls.CButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(3, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP地址：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(3, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "端口号：";
            // 
            // tbx_log
            // 
            this.tbx_log.BackColor = System.Drawing.Color.White;
            this.tbx_log.Location = new System.Drawing.Point(231, 42);
            this.tbx_log.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_log.Multiline = true;
            this.tbx_log.Name = "tbx_log";
            this.tbx_log.ReadOnly = true;
            this.tbx_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbx_log.Size = new System.Drawing.Size(295, 288);
            this.tbx_log.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(228, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "通讯记录";
            // 
            // lnk_clearLog
            // 
            this.lnk_clearLog.AutoSize = true;
            this.lnk_clearLog.Location = new System.Drawing.Point(494, 21);
            this.lnk_clearLog.Name = "lnk_clearLog";
            this.lnk_clearLog.Size = new System.Drawing.Size(32, 17);
            this.lnk_clearLog.TabIndex = 12;
            this.lnk_clearLog.TabStop = true;
            this.lnk_clearLog.Text = "清空";
            this.lnk_clearLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_clearLog_LinkClicked);
            // 
            // tbx_severIP
            // 
            this.tbx_severIP.BackColor = System.Drawing.Color.White;
            this.tbx_severIP.DefaultText = "请输入服务器IP地址";
            this.tbx_severIP.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_severIP.Location = new System.Drawing.Point(52, 60);
            this.tbx_severIP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_severIP.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_severIP.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_severIP.Name = "tbx_severIP";
            this.tbx_severIP.PasswordChar = false;
            this.tbx_severIP.Size = new System.Drawing.Size(151, 22);
            this.tbx_severIP.TabIndex = 13;
            this.tbx_severIP.TextStr = "";
            this.tbx_severIP.TextStrChanged += new Controls.DTextStrChanged(this.tbx_severIP_TextStrChanged);
            // 
            // tbx_severPort
            // 
            this.tbx_severPort.BackColor = System.Drawing.Color.White;
            this.tbx_severPort.DefaultText = "请输入服务器端口号";
            this.tbx_severPort.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_severPort.Location = new System.Drawing.Point(52, 85);
            this.tbx_severPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_severPort.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_severPort.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_severPort.Name = "tbx_severPort";
            this.tbx_severPort.PasswordChar = false;
            this.tbx_severPort.Size = new System.Drawing.Size(151, 22);
            this.tbx_severPort.TabIndex = 14;
            this.tbx_severPort.TextStr = "";
            this.tbx_severPort.TextStrChanged += new Controls.DTextStrChanged(this.tbx_port_TextStrChanged);
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel8.Location = new System.Drawing.Point(217, 7);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(1, 390);
            this.panel8.TabIndex = 150;
            // 
            // tbx_sendMessage
            // 
            this.tbx_sendMessage.BackColor = System.Drawing.Color.White;
            this.tbx_sendMessage.DefaultText = "请输入需要发送的信息";
            this.tbx_sendMessage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_sendMessage.Location = new System.Drawing.Point(231, 356);
            this.tbx_sendMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_sendMessage.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_sendMessage.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_sendMessage.Name = "tbx_sendMessage";
            this.tbx_sendMessage.PasswordChar = false;
            this.tbx_sendMessage.Size = new System.Drawing.Size(234, 22);
            this.tbx_sendMessage.TabIndex = 151;
            this.tbx_sendMessage.TextStr = "Test";
            // 
            // tbx_clientName
            // 
            this.tbx_clientName.BackColor = System.Drawing.Color.White;
            this.tbx_clientName.DefaultText = "请输入客户端名称";
            this.tbx_clientName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_clientName.Location = new System.Drawing.Point(52, 18);
            this.tbx_clientName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_clientName.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_clientName.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_clientName.Name = "tbx_clientName";
            this.tbx_clientName.PasswordChar = false;
            this.tbx_clientName.Size = new System.Drawing.Size(151, 22);
            this.tbx_clientName.TabIndex = 153;
            this.tbx_clientName.TextStr = "";
            this.tbx_clientName.TextStrChanged += new Controls.DTextStrChanged(this.tbx_clientName_TextStrChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(3, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 152;
            this.label3.Text = "名   称：";
            // 
            // btn_connect
            // 
            this.btn_connect.BackColor = System.Drawing.Color.White;
            this.btn_connect.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_connect.Location = new System.Drawing.Point(136, 115);
            this.btn_connect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(67, 28);
            this.btn_connect.TabIndex = 154;
            this.btn_connect.TextStr = "连接";
            this.btn_connect.Clicked += new Controls.DClicked(this.btn_connect_Clicked);
            // 
            // ckb_autoDisconnectBeforeClose
            // 
            this.ckb_autoDisconnectBeforeClose.BackColor = System.Drawing.Color.White;
            this.ckb_autoDisconnectBeforeClose.Checked = false;
            this.ckb_autoDisconnectBeforeClose.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoDisconnectBeforeClose.Location = new System.Drawing.Point(57, 182);
            this.ckb_autoDisconnectBeforeClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoDisconnectBeforeClose.Name = "ckb_autoDisconnectBeforeClose";
            this.ckb_autoDisconnectBeforeClose.Size = new System.Drawing.Size(170, 20);
            this.ckb_autoDisconnectBeforeClose.TabIndex = 156;
            this.ckb_autoDisconnectBeforeClose.TextStr = "程序关闭前断开服务器";
            this.ckb_autoDisconnectBeforeClose.CheckChanged += new Controls.DCheckChanged(this.ckb_autoDisconnectBeforeClose_CheckChanged);
            // 
            // ckb_autoConnectAfterStart
            // 
            this.ckb_autoConnectAfterStart.BackColor = System.Drawing.Color.White;
            this.ckb_autoConnectAfterStart.Checked = false;
            this.ckb_autoConnectAfterStart.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoConnectAfterStart.Location = new System.Drawing.Point(57, 160);
            this.ckb_autoConnectAfterStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoConnectAfterStart.Name = "ckb_autoConnectAfterStart";
            this.ckb_autoConnectAfterStart.Size = new System.Drawing.Size(170, 20);
            this.ckb_autoConnectAfterStart.TabIndex = 155;
            this.ckb_autoConnectAfterStart.TextStr = "程序启动后连接服务器";
            this.ckb_autoConnectAfterStart.CheckChanged += new Controls.DCheckChanged(this.ckb_autoConnectAfterStart_CheckChanged);
            // 
            // btn_send
            // 
            this.btn_send.BackColor = System.Drawing.Color.White;
            this.btn_send.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_send.Location = new System.Drawing.Point(471, 354);
            this.btn_send.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(55, 24);
            this.btn_send.TabIndex = 157;
            this.btn_send.TextStr = "发送";
            this.btn_send.Clicked += new Controls.DClicked(this.btn_send_Clicked);
            // 
            // Frm_TCPClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(542, 415);
            this.Controls.Add(this.btn_send);
            this.Controls.Add(this.btn_connect);
            this.Controls.Add(this.tbx_clientName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbx_sendMessage);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.tbx_severPort);
            this.Controls.Add(this.tbx_severIP);
            this.Controls.Add(this.lnk_clearLog);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbx_log);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ckb_autoDisconnectBeforeClose);
            this.Controls.Add(this.ckb_autoConnectAfterStart);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_TCPClient";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TCP客户端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_TCPClient_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox tbx_log;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel lnk_clearLog;
        private Controls.CTextBox tbx_severIP;
        private Controls.CTextBox tbx_severPort;
        private System.Windows.Forms.Panel panel8;
        private Controls.CTextBox tbx_sendMessage;
        private Controls.CTextBox tbx_clientName;
        internal System.Windows.Forms.Label label3;
        private Controls.CCheckBox ckb_autoDisconnectBeforeClose;
        private Controls.CCheckBox ckb_autoConnectAfterStart;
        private Controls.CButton btn_send;
        internal Controls.CButton btn_connect;
    }
}