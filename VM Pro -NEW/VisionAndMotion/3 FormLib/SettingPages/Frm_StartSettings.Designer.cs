using Controls;

namespace VMPro
{
    partial class Frm_StartSetting
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
            this.ckb_autoConnect = new Controls.CCheckBox();
            this.ckb_switchedToAutoRunMode = new Controls.CCheckBox();
            this.ckb_autoStartAfterStartup = new Controls.CCheckBox();
            this.ckb_allowResizeFormSize = new Controls.CCheckBox();
            this.ckb_maxSizeAfterStart = new Controls.CCheckBox();
            this.ckb_autoLock = new Controls.CCheckBox();
            this.ckb_displayLine = new Controls.CCheckBox();
            this.ckb_saveWhileExit = new Controls.CCheckBox();
            this.cCheckBox1 = new Controls.CCheckBox();
            this.ckb_enablePermissionControl = new Controls.CCheckBox();
            this.SuspendLayout();
            // 
            // ckb_autoConnect
            // 
            this.ckb_autoConnect.BackColor = System.Drawing.Color.White;
            this.ckb_autoConnect.Checked = false;
            this.ckb_autoConnect.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoConnect.Location = new System.Drawing.Point(14, 9);
            this.ckb_autoConnect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoConnect.Name = "ckb_autoConnect";
            this.ckb_autoConnect.Size = new System.Drawing.Size(232, 20);
            this.ckb_autoConnect.TabIndex = 20;
            this.ckb_autoConnect.TextStr = "程序开启后自动连接第三方通讯设备";
            // 
            // ckb_switchedToAutoRunMode
            // 
            this.ckb_switchedToAutoRunMode.BackColor = System.Drawing.Color.White;
            this.ckb_switchedToAutoRunMode.Checked = false;
            this.ckb_switchedToAutoRunMode.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_switchedToAutoRunMode.Location = new System.Drawing.Point(14, 36);
            this.ckb_switchedToAutoRunMode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_switchedToAutoRunMode.Name = "ckb_switchedToAutoRunMode";
            this.ckb_switchedToAutoRunMode.Size = new System.Drawing.Size(232, 20);
            this.ckb_switchedToAutoRunMode.TabIndex = 21;
            this.ckb_switchedToAutoRunMode.TextStr = "程序开启后自动切换到自动运行模式";
            // 
            // ckb_autoStartAfterStartup
            // 
            this.ckb_autoStartAfterStartup.BackColor = System.Drawing.Color.White;
            this.ckb_autoStartAfterStartup.Checked = false;
            this.ckb_autoStartAfterStartup.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoStartAfterStartup.Location = new System.Drawing.Point(14, 63);
            this.ckb_autoStartAfterStartup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoStartAfterStartup.Name = "ckb_autoStartAfterStartup";
            this.ckb_autoStartAfterStartup.Size = new System.Drawing.Size(181, 20);
            this.ckb_autoStartAfterStartup.TabIndex = 22;
            this.ckb_autoStartAfterStartup.TextStr = "开机后程序自启动";
            // 
            // ckb_allowResizeFormSize
            // 
            this.ckb_allowResizeFormSize.BackColor = System.Drawing.Color.White;
            this.ckb_allowResizeFormSize.Checked = false;
            this.ckb_allowResizeFormSize.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_allowResizeFormSize.Location = new System.Drawing.Point(14, 144);
            this.ckb_allowResizeFormSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_allowResizeFormSize.Name = "ckb_allowResizeFormSize";
            this.ckb_allowResizeFormSize.Size = new System.Drawing.Size(181, 20);
            this.ckb_allowResizeFormSize.TabIndex = 27;
            this.ckb_allowResizeFormSize.TextStr = "允许改变窗体大小";
            // 
            // ckb_maxSizeAfterStart
            // 
            this.ckb_maxSizeAfterStart.BackColor = System.Drawing.Color.White;
            this.ckb_maxSizeAfterStart.Checked = false;
            this.ckb_maxSizeAfterStart.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_maxSizeAfterStart.Location = new System.Drawing.Point(14, 117);
            this.ckb_maxSizeAfterStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_maxSizeAfterStart.Name = "ckb_maxSizeAfterStart";
            this.ckb_maxSizeAfterStart.Size = new System.Drawing.Size(181, 20);
            this.ckb_maxSizeAfterStart.TabIndex = 25;
            this.ckb_maxSizeAfterStart.TextStr = "程序开启后自动最大化";
            // 
            // ckb_autoLock
            // 
            this.ckb_autoLock.BackColor = System.Drawing.Color.White;
            this.ckb_autoLock.Checked = false;
            this.ckb_autoLock.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_autoLock.Location = new System.Drawing.Point(14, 90);
            this.ckb_autoLock.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_autoLock.Name = "ckb_autoLock";
            this.ckb_autoLock.Size = new System.Drawing.Size(181, 20);
            this.ckb_autoLock.TabIndex = 24;
            this.ckb_autoLock.TextStr = "程序开启后自动锁定";
            // 
            // ckb_displayLine
            // 
            this.ckb_displayLine.BackColor = System.Drawing.Color.White;
            this.ckb_displayLine.Checked = false;
            this.ckb_displayLine.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_displayLine.Location = new System.Drawing.Point(14, 198);
            this.ckb_displayLine.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_displayLine.Name = "ckb_displayLine";
            this.ckb_displayLine.Size = new System.Drawing.Size(181, 20);
            this.ckb_displayLine.TabIndex = 30;
            this.ckb_displayLine.TextStr = "启用流程连线";
            // 
            // ckb_saveWhileExit
            // 
            this.ckb_saveWhileExit.BackColor = System.Drawing.Color.White;
            this.ckb_saveWhileExit.Checked = false;
            this.ckb_saveWhileExit.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_saveWhileExit.Location = new System.Drawing.Point(14, 171);
            this.ckb_saveWhileExit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_saveWhileExit.Name = "ckb_saveWhileExit";
            this.ckb_saveWhileExit.Size = new System.Drawing.Size(181, 20);
            this.ckb_saveWhileExit.TabIndex = 28;
            this.ckb_saveWhileExit.TextStr = "退出程序时保存数据";
            // 
            // cCheckBox1
            // 
            this.cCheckBox1.BackColor = System.Drawing.Color.White;
            this.cCheckBox1.Checked = false;
            this.cCheckBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cCheckBox1.Location = new System.Drawing.Point(14, 225);
            this.cCheckBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cCheckBox1.Name = "cCheckBox1";
            this.cCheckBox1.Size = new System.Drawing.Size(181, 20);
            this.cCheckBox1.TabIndex = 31;
            this.cCheckBox1.TextStr = "程序打开后自动启动";
            // 
            // ckb_enablePermissionControl
            // 
            this.ckb_enablePermissionControl.BackColor = System.Drawing.Color.White;
            this.ckb_enablePermissionControl.Checked = false;
            this.ckb_enablePermissionControl.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_enablePermissionControl.Location = new System.Drawing.Point(14, 252);
            this.ckb_enablePermissionControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_enablePermissionControl.Name = "ckb_enablePermissionControl";
            this.ckb_enablePermissionControl.Size = new System.Drawing.Size(106, 20);
            this.ckb_enablePermissionControl.TabIndex = 32;
            this.ckb_enablePermissionControl.TextStr = "开启权限管控";
            // 
            // Frm_StartSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(545, 427);
            this.Controls.Add(this.ckb_enablePermissionControl);
            this.Controls.Add(this.cCheckBox1);
            this.Controls.Add(this.ckb_displayLine);
            this.Controls.Add(this.ckb_saveWhileExit);
            this.Controls.Add(this.ckb_allowResizeFormSize);
            this.Controls.Add(this.ckb_maxSizeAfterStart);
            this.Controls.Add(this.ckb_autoLock);
            this.Controls.Add(this.ckb_autoStartAfterStartup);
            this.Controls.Add(this.ckb_switchedToAutoRunMode);
            this.Controls.Add(this.ckb_autoConnect);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_StartSetting";
            this.Text = "Frm_startSetting";
            this.ResumeLayout(false);

        }

        #endregion

        public CCheckBox ckb_autoConnect;
        public CCheckBox ckb_switchedToAutoRunMode;
        public CCheckBox ckb_autoStartAfterStartup;
        public CCheckBox ckb_allowResizeFormSize;
        public CCheckBox ckb_maxSizeAfterStart;
        public CCheckBox ckb_autoLock;
        public CCheckBox ckb_displayLine;
        public CCheckBox ckb_saveWhileExit;
        public CCheckBox cCheckBox1;
        public CCheckBox ckb_enablePermissionControl;


    }
}