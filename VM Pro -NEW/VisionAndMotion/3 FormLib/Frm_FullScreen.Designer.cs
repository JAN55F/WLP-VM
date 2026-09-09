namespace VMPro
{
    partial class Frm_FullScreen
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
            this.pic_showImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pic_showImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pic_showImage
            // 
            this.pic_showImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic_showImage.Location = new System.Drawing.Point(0, 0);
            this.pic_showImage.Name = "pic_showImage";
            this.pic_showImage.Size = new System.Drawing.Size(604, 418);
            this.pic_showImage.TabIndex = 0;
            this.pic_showImage.TabStop = false;
            // 
            // Frm_FullScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 418);
            this.Controls.Add(this.pic_showImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Frm_FullScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frm_FullScreen";
            this.Load += new System.EventHandler(this.Frm_FullScreen_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Frm_FullScreen_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pic_showImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.PictureBox pic_showImage;


    }
}