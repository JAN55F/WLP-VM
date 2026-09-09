namespace VMPro
{
    partial class Frm_ProcessingItem
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
            this.btn_applyAndExit = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbx_elementSize = new System.Windows.Forms.TextBox();
            this.tbx_minArea = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbx_maxArea = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_applyAndExit
            // 
            this.btn_applyAndExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_applyAndExit.Location = new System.Drawing.Point(191, 113);
            this.btn_applyAndExit.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btn_applyAndExit.Name = "btn_applyAndExit";
            this.btn_applyAndExit.Size = new System.Drawing.Size(82, 28);
            this.btn_applyAndExit.TabIndex = 16;
            this.btn_applyAndExit.Tag = "";
            this.btn_applyAndExit.Text = "应用并退出";
            this.btn_applyAndExit.UseVisualStyleBackColor = true;
            this.btn_applyAndExit.Click += new System.EventHandler(this.btn_applyAndExit_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(76, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 17);
            this.label6.TabIndex = 15;
            this.label6.Text = "面积下限：";
            // 
            // tbx_elementSize
            // 
            this.tbx_elementSize.Location = new System.Drawing.Point(139, 79);
            this.tbx_elementSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_elementSize.Name = "tbx_elementSize";
            this.tbx_elementSize.Size = new System.Drawing.Size(105, 23);
            this.tbx_elementSize.TabIndex = 12;
            this.tbx_elementSize.Text = "1";
            // 
            // tbx_minArea
            // 
            this.tbx_minArea.Location = new System.Drawing.Point(139, 19);
            this.tbx_minArea.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_minArea.Name = "tbx_minArea";
            this.tbx_minArea.Size = new System.Drawing.Size(105, 23);
            this.tbx_minArea.TabIndex = 14;
            this.tbx_minArea.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(76, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 17);
            this.label7.TabIndex = 11;
            this.label7.Text = "半径：";
            // 
            // tbx_maxArea
            // 
            this.tbx_maxArea.Location = new System.Drawing.Point(139, 49);
            this.tbx_maxArea.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_maxArea.Name = "tbx_maxArea";
            this.tbx_maxArea.Size = new System.Drawing.Size(105, 23);
            this.tbx_maxArea.TabIndex = 18;
            this.tbx_maxArea.Text = "10000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "面积上限：";
            // 
            // Frm_ProcessingItem
            // 
            this.AcceptButton = this.btn_applyAndExit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(320, 161);
            this.Controls.Add(this.tbx_minArea);
            this.Controls.Add(this.tbx_maxArea);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_applyAndExit);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbx_elementSize);
            this.Controls.Add(this.label7);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_ProcessingItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "配置";
            this.Load += new System.EventHandler(this.Frm_ProcessingItemConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_applyAndExit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tbx_elementSize;
        public System.Windows.Forms.TextBox tbx_minArea;
        public System.Windows.Forms.TextBox tbx_maxArea;
    }
}