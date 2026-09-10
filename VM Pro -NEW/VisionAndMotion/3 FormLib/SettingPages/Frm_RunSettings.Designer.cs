using Controls;

namespace VMPro
{
    partial class Frm_RunSettings
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbx_jobsRunPouseTime = new Controls.CNumericUpDown();
            this.ckb_displayLine = new Controls.CCheckBox();
            this.cCheckBox1 = new Controls.CCheckBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "间隔时间：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(548, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "ms";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(548, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "mm/s";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(216, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "ms";
            // 
            // tbx_jobsRunPouseTime
            // 
            this.tbx_jobsRunPouseTime.BackColor = System.Drawing.Color.White;
            this.tbx_jobsRunPouseTime.DecimalPlaces = 0;
            this.tbx_jobsRunPouseTime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_jobsRunPouseTime.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbx_jobsRunPouseTime.Location = new System.Drawing.Point(79, 8);
            this.tbx_jobsRunPouseTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_jobsRunPouseTime.MaximumSize = new System.Drawing.Size(300, 25);
            this.tbx_jobsRunPouseTime.MaxValue = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.tbx_jobsRunPouseTime.MinimumSize = new System.Drawing.Size(50, 25);
            this.tbx_jobsRunPouseTime.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.tbx_jobsRunPouseTime.Name = "tbx_jobsRunPouseTime";
            this.tbx_jobsRunPouseTime.Size = new System.Drawing.Size(135, 25);
            this.tbx_jobsRunPouseTime.TabIndex = 13;
            this.tbx_jobsRunPouseTime.Value = 0D;
            // 
            // ckb_displayLine
            // 
            this.ckb_displayLine.BackColor = System.Drawing.Color.White;
            this.ckb_displayLine.Checked = false;
            this.ckb_displayLine.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_displayLine.Location = new System.Drawing.Point(16, 36);
            this.ckb_displayLine.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_displayLine.Name = "ckb_displayLine";
            this.ckb_displayLine.Size = new System.Drawing.Size(141, 20);
            this.ckb_displayLine.TabIndex = 21;
            this.ckb_displayLine.TextStr = "流程失败时停止循环";
            // 
            // cCheckBox1
            // 
            this.cCheckBox1.BackColor = System.Drawing.Color.White;
            this.cCheckBox1.Checked = false;
            this.cCheckBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cCheckBox1.Location = new System.Drawing.Point(16, 61);
            this.cCheckBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cCheckBox1.Name = "cCheckBox1";
            this.cCheckBox1.Size = new System.Drawing.Size(209, 20);
            this.cCheckBox1.TabIndex = 22;
            this.cCheckBox1.TextStr = "文件夹图像遍历结束后停止循环";
            // 
            // Frm_RunSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(545, 427);
            this.Controls.Add(this.cCheckBox1);
            this.Controls.Add(this.ckb_displayLine);
            this.Controls.Add(this.tbx_jobsRunPouseTime);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_RunSettings";
            this.Text = "Frm_runSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Label label6;
        public CNumericUpDown tbx_jobsRunPouseTime;
        public CCheckBox ckb_displayLine;
        public CCheckBox cCheckBox1;
    }
}