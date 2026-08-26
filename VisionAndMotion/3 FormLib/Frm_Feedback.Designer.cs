using Controls;
namespace VMPro
{
    partial class Frm_Feedback
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Feedback));
            this.label1 = new System.Windows.Forms.Label();
            this.tbx_feedBackMessage = new System.Windows.Forms.TextBox();
            this.btn_submit = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbx_emailAddress = new Controls.CTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Location = new System.Drawing.Point(10, 6);
            this.lbl_title.Size = new System.Drawing.Size(68, 17);
            this.lbl_title.Text = "建议与反馈";
            // 
            // button100
            // 
            this.button100.FlatAppearance.BorderSize = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(375, 36);
            this.label1.TabIndex = 0;
            this.label1.Text = "      感谢您对此产品提出宝贵意见，我们将认真汲取并改进，从而更好的服务用户，请在下面的文本框内填写您的反馈信息后提交。";
            // 
            // tbx_feedBackMessage
            // 
            this.tbx_feedBackMessage.BackColor = System.Drawing.Color.White;
            this.tbx_feedBackMessage.Location = new System.Drawing.Point(19, 59);
            this.tbx_feedBackMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_feedBackMessage.Multiline = true;
            this.tbx_feedBackMessage.Name = "tbx_feedBackMessage";
            this.tbx_feedBackMessage.Size = new System.Drawing.Size(372, 104);
            this.tbx_feedBackMessage.TabIndex = 1;
            // 
            // btn_submit
            // 
            this.btn_submit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_submit.BackgroundImage")));
            this.btn_submit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_submit.FlatAppearance.BorderSize = 0;
            this.btn_submit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_submit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_submit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_submit.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_submit.ForeColor = System.Drawing.Color.White;
            this.btn_submit.Location = new System.Drawing.Point(321, 208);
            this.btn_submit.Margin = new System.Windows.Forms.Padding(5);
            this.btn_submit.Name = "btn_submit";
            this.btn_submit.Size = new System.Drawing.Size(70, 30);
            this.btn_submit.TabIndex = 105;
            this.btn_submit.Text = "提交";
            this.btn_submit.UseVisualStyleBackColor = true;
            this.btn_submit.Click += new System.EventHandler(this.btn_runOnce_Click);
            this.btn_submit.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_submit.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_submit.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_submit.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.tbx_emailAddress);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.tbx_feedBackMessage);
            this.panel2.Controls.Add(this.btn_submit);
            this.panel2.Location = new System.Drawing.Point(2, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(410, 246);
            this.panel2.TabIndex = 106;
            // 
            // tbx_emailAddress
            // 
            this.tbx_emailAddress.BackColor = System.Drawing.Color.White;
            this.tbx_emailAddress.DefaultText = "请输入您的邮箱地址，便于我们回复（可不填）";
            this.tbx_emailAddress.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_emailAddress.Location = new System.Drawing.Point(19, 174);
            this.tbx_emailAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_emailAddress.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_emailAddress.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_emailAddress.Name = "tbx_emailAddress";
            this.tbx_emailAddress.PasswordChar = false;
            this.tbx_emailAddress.Size = new System.Drawing.Size(372, 22);
            this.tbx_emailAddress.TabIndex = 106;
            this.tbx_emailAddress.TextStr = "";
            // 
            // Frm_Feedback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(414, 274);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(414, 274);
            this.MinimumSize = new System.Drawing.Size(414, 274);
            this.Name = "Frm_Feedback";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "建议和反馈";
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbx_feedBackMessage;
        public System.Windows.Forms.Button btn_submit;
        private System.Windows.Forms.Panel panel2;
        private CTextBox tbx_emailAddress;
    }
}