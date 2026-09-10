using Controls;
namespace VMPro
{
    partial class Frm_Lock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Lock));
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbx_password = new Controls.CTextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(38, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "锁定中......";
            this.label1.Click += new System.EventHandler(this.Frm_Lock_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(1, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(36, 33);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // tbx_password
            // 
            this.tbx_password.BackColor = System.Drawing.Color.White;
            this.tbx_password.DefaultText = "请输入解锁密码";
            this.tbx_password.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_password.Location = new System.Drawing.Point(21, 64);
            this.tbx_password.Margin = new System.Windows.Forms.Padding(0);
            this.tbx_password.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_password.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_password.Name = "tbx_password";
            this.tbx_password.PasswordChar = true;
            this.tbx_password.Size = new System.Drawing.Size(160, 22);
            this.tbx_password.TabIndex = 4;
            this.tbx_password.TextStr = "";
            this.tbx_password.TextStrChanged += new Controls.DTextStrChanged(this.tbx_password_TextStrChanged);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(184, 62);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 26);
            this.button1.TabIndex = 5;
            this.button1.Text = "解锁";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Frm_Lock
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(254, 106);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbx_password);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Frm_Lock";
            this.Opacity = 0.6D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frm_Lock";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_Lock_FormClosing);
            this.Load += new System.EventHandler(this.Frm_Lock_Load);
            this.Shown += new System.EventHandler(this.Frm_Lock_Shown);
            this.Click += new System.EventHandler(this.Frm_Lock_Click);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.setForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.setForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.setForm_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private CTextBox tbx_password;
        private System.Windows.Forms.Button button1;
    }
}