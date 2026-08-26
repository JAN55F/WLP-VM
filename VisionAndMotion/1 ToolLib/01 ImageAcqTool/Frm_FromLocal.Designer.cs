namespace VMPro
{
    partial class Frm_FromLocal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FromLocal));
            this.tbx_imagePath = new System.Windows.Forms.TextBox();
            this.btn_lastImage = new System.Windows.Forms.Button();
            this.btn_nextImage = new System.Windows.Forms.Button();
            this.tbx_imageDirectoryPath = new System.Windows.Forms.TextBox();
            this.pnl_multImage = new System.Windows.Forms.Panel();
            this.btn_browseImage = new Controls.CButton();
            this.btn_selectImageDirectoryPath = new Controls.CButton();
            this.btn_selectImagePath = new Controls.CButton();
            this.pnl_multImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbx_imagePath
            // 
            this.tbx_imagePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_imagePath.Location = new System.Drawing.Point(6, 18);
            this.tbx_imagePath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_imagePath.Multiline = true;
            this.tbx_imagePath.Name = "tbx_imagePath";
            this.tbx_imagePath.Size = new System.Drawing.Size(258, 88);
            this.tbx_imagePath.TabIndex = 1;
            this.tbx_imagePath.TextChanged += new System.EventHandler(this.tbx_imagePath_TextChanged);
            // 
            // btn_lastImage
            // 
            this.btn_lastImage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_lastImage.BackgroundImage")));
            this.btn_lastImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_lastImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_lastImage.FlatAppearance.BorderSize = 0;
            this.btn_lastImage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_lastImage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_lastImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_lastImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_lastImage.Location = new System.Drawing.Point(5, 104);
            this.btn_lastImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_lastImage.Name = "btn_lastImage";
            this.btn_lastImage.Size = new System.Drawing.Size(33, 28);
            this.btn_lastImage.TabIndex = 44;
            this.btn_lastImage.TabStop = false;
            this.btn_lastImage.UseVisualStyleBackColor = true;
            this.btn_lastImage.Click += new System.EventHandler(this.btn_lastImage_Click);
            this.btn_lastImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_lastImage_MouseDown);
            this.btn_lastImage.MouseEnter += new System.EventHandler(this.btn_lastImage_MouseEnter);
            this.btn_lastImage.MouseLeave += new System.EventHandler(this.btn_lastImage_MouseLeave);
            this.btn_lastImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_lastImage_MouseUp);
            // 
            // btn_nextImage
            // 
            this.btn_nextImage.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btn_nextImage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_nextImage.BackgroundImage")));
            this.btn_nextImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_nextImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_nextImage.FlatAppearance.BorderSize = 0;
            this.btn_nextImage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_nextImage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_nextImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_nextImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_nextImage.Location = new System.Drawing.Point(41, 104);
            this.btn_nextImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_nextImage.Name = "btn_nextImage";
            this.btn_nextImage.Size = new System.Drawing.Size(33, 28);
            this.btn_nextImage.TabIndex = 0;
            this.btn_nextImage.TabStop = false;
            this.btn_nextImage.UseVisualStyleBackColor = true;
            this.btn_nextImage.Click += new System.EventHandler(this.btn_nextImage_Click);
            this.btn_nextImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_nextImage_MouseDown);
            this.btn_nextImage.MouseEnter += new System.EventHandler(this.btn_nextImage_MouseEnter);
            this.btn_nextImage.MouseLeave += new System.EventHandler(this.btn_nextImage_MouseLeave);
            this.btn_nextImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_nextImage_MouseUp);
            // 
            // tbx_imageDirectoryPath
            // 
            this.tbx_imageDirectoryPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_imageDirectoryPath.Location = new System.Drawing.Point(5, 10);
            this.tbx_imageDirectoryPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_imageDirectoryPath.Multiline = true;
            this.tbx_imageDirectoryPath.Name = "tbx_imageDirectoryPath";
            this.tbx_imageDirectoryPath.Size = new System.Drawing.Size(258, 88);
            this.tbx_imageDirectoryPath.TabIndex = 5;
            this.tbx_imageDirectoryPath.TabStop = false;
            this.tbx_imageDirectoryPath.TextChanged += new System.EventHandler(this.tbx_imageDirectoryPath_TextChanged);
            // 
            // pnl_multImage
            // 
            this.pnl_multImage.Controls.Add(this.btn_browseImage);
            this.pnl_multImage.Controls.Add(this.btn_selectImageDirectoryPath);
            this.pnl_multImage.Controls.Add(this.tbx_imageDirectoryPath);
            this.pnl_multImage.Controls.Add(this.btn_nextImage);
            this.pnl_multImage.Controls.Add(this.btn_lastImage);
            this.pnl_multImage.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pnl_multImage.Location = new System.Drawing.Point(1, 8);
            this.pnl_multImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnl_multImage.Name = "pnl_multImage";
            this.pnl_multImage.Size = new System.Drawing.Size(282, 148);
            this.pnl_multImage.TabIndex = 87;
            this.pnl_multImage.Visible = false;
            // 
            // btn_browseImage
            // 
            this.btn_browseImage.BackColor = System.Drawing.Color.White;
            this.btn_browseImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_browseImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_browseImage.Location = new System.Drawing.Point(80, 106);
            this.btn_browseImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_browseImage.Name = "btn_browseImage";
            this.btn_browseImage.Size = new System.Drawing.Size(68, 26);
            this.btn_browseImage.TabIndex = 116;
            this.btn_browseImage.TextStr = "浏览图像";
            // 
            // btn_selectImageDirectoryPath
            // 
            this.btn_selectImageDirectoryPath.BackColor = System.Drawing.Color.White;
            this.btn_selectImageDirectoryPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_selectImageDirectoryPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_selectImageDirectoryPath.Location = new System.Drawing.Point(195, 106);
            this.btn_selectImageDirectoryPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_selectImageDirectoryPath.Name = "btn_selectImageDirectoryPath";
            this.btn_selectImageDirectoryPath.Size = new System.Drawing.Size(68, 26);
            this.btn_selectImageDirectoryPath.TabIndex = 117;
            this.btn_selectImageDirectoryPath.TextStr = "指定路径";
            // 
            // btn_selectImagePath
            // 
            this.btn_selectImagePath.BackColor = System.Drawing.Color.White;
            this.btn_selectImagePath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_selectImagePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_selectImagePath.Location = new System.Drawing.Point(196, 114);
            this.btn_selectImagePath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_selectImagePath.Name = "btn_selectImagePath";
            this.btn_selectImagePath.Size = new System.Drawing.Size(68, 26);
            this.btn_selectImagePath.TabIndex = 118;
            this.btn_selectImagePath.TextStr = "指定路径";
            // 
            // Frm_FromLocal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(270, 294);
            this.Controls.Add(this.pnl_multImage);
            this.Controls.Add(this.tbx_imagePath);
            this.Controls.Add(this.btn_selectImagePath);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Frm_FromLocal";
            this.pnl_multImage.ResumeLayout(false);
            this.pnl_multImage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox tbx_imagePath;
        internal System.Windows.Forms.Button btn_lastImage;
        internal System.Windows.Forms.Button btn_nextImage;
        internal System.Windows.Forms.TextBox tbx_imageDirectoryPath;
        internal System.Windows.Forms.Panel pnl_multImage;
        internal Controls.CButton btn_selectImagePath;
        internal Controls.CButton btn_browseImage;
        internal Controls.CButton btn_selectImageDirectoryPath;

    }
}