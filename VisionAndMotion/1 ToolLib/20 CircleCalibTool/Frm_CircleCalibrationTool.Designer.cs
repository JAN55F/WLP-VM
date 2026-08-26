namespace VMPro
{
    partial class Frm_CircleCalibrationTool
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
            this.label13 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbx_mmPixelRoute = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbx_polarity
            // 
            this.cbx_polarity.Size = new System.Drawing.Size(71, 25);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(304, 188);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(92, 17);
            this.label13.TabIndex = 172;
            this.label13.Text = "小圆实际半径：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(389, 185);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(85, 23);
            this.textBox1.TabIndex = 173;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(478, 188);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(30, 17);
            this.label14.TabIndex = 174;
            this.label14.Text = "mm";
            // 
            // tbx_mmPixelRoute
            // 
            this.tbx_mmPixelRoute.Location = new System.Drawing.Point(389, 215);
            this.tbx_mmPixelRoute.Name = "tbx_mmPixelRoute";
            this.tbx_mmPixelRoute.ReadOnly = true;
            this.tbx_mmPixelRoute.Size = new System.Drawing.Size(85, 23);
            this.tbx_mmPixelRoute.TabIndex = 176;
            this.tbx_mmPixelRoute.TextChanged += new System.EventHandler(this.tbx_mmPixelRoute_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(304, 218);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 17);
            this.label15.TabIndex = 175;
            this.label15.Text = "毫米像素比：";
            // 
            // Frm_CircleCalibrationTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 392);
            this.Controls.Add(this.tbx_mmPixelRoute);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label13);
            this.Name = "Frm_CircleCalibrationTool";
            this.Text = "圆标定";
            this.Controls.SetChildIndex(this.tbx_ringRadiusLength, 0);
            this.Controls.SetChildIndex(this.tbx_cliperNum, 0);
            this.Controls.SetChildIndex(this.cbx_polarity, 0);
            this.Controls.SetChildIndex(this.tbx_threshold, 0);
            this.Controls.SetChildIndex(this.label13, 0);
            this.Controls.SetChildIndex(this.textBox1, 0);
            this.Controls.SetChildIndex(this.label14, 0);
            this.Controls.SetChildIndex(this.label15, 0);
            this.Controls.SetChildIndex(this.tbx_mmPixelRoute, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbx_mmPixelRoute;
        private System.Windows.Forms.Label label15;
    }
}