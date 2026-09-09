namespace VMPro
{
    partial class Frm_MessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_MessageBox));
            this.btn_confim = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbl_info = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Location = new System.Drawing.Point(28, 6);
            this.lbl_title.Size = new System.Drawing.Size(32, 17);
            this.lbl_title.Text = "提示";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 6);
            this.pictureBox1.Size = new System.Drawing.Size(18, 15);
            // 
            // button100
            // 
            this.button100.FlatAppearance.BorderSize = 0;
            // 
            // btn_confim
            // 
            this.btn_confim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_confim.BackgroundImage = global::VMPro.Properties.Resources.ButtonUp;
            this.btn_confim.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_confim.FlatAppearance.BorderSize = 0;
            this.btn_confim.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_confim.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_confim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_confim.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_confim.ForeColor = System.Drawing.Color.White;
            this.btn_confim.Location = new System.Drawing.Point(298, 99);
            this.btn_confim.Name = "btn_confim";
            this.btn_confim.Size = new System.Drawing.Size(69, 30);
            this.btn_confim.TabIndex = 0;
            this.btn_confim.Text = "确定";
            this.btn_confim.UseVisualStyleBackColor = true;
            this.btn_confim.Click += new System.EventHandler(this.btn_confim_Click);
            this.btn_confim.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_confim_MouseDown);
            this.btn_confim.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_confim.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_confim.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_confim_MouseUp);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.lbl_info);
            this.panel2.Controls.Add(this.btn_confim);
            this.panel2.Location = new System.Drawing.Point(2, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(389, 136);
            this.panel2.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.ForeColor = System.Drawing.Color.White;
            this.panel3.Location = new System.Drawing.Point(308, 124);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(50, 1);
            this.panel3.TabIndex = 108;
            // 
            // lbl_info
            // 
            this.lbl_info.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_info.Location = new System.Drawing.Point(16, 8);
            this.lbl_info.Name = "lbl_info";
            this.lbl_info.Size = new System.Drawing.Size(356, 82);
            this.lbl_info.TabIndex = 2;
            this.lbl_info.Text = "lal_info";
            // 
            // Frm_MessageBox
            // 
            this.AcceptButton = this.btn_confim;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(393, 164);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(393, 164);
            this.MinimumSize = new System.Drawing.Size(393, 164);
            this.Name = "Frm_MessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "提示";
            this.Load += new System.EventHandler(this.Frm_MessageBox_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.setForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.setForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.setForm_MouseUp);
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btn_confim;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        internal System.Windows.Forms.Label lbl_info;
    }
}