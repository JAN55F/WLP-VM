namespace VMPro
{
    partial class Frm_AxisSetting
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
            this.ppg_axisProperty = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // ppg_axisProperty
            // 
            this.ppg_axisProperty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ppg_axisProperty.Location = new System.Drawing.Point(0, 0);
            this.ppg_axisProperty.Name = "ppg_axisProperty";
            this.ppg_axisProperty.Size = new System.Drawing.Size(308, 466);
            this.ppg_axisProperty.TabIndex = 0;
            // 
            // Frm_AxisSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 466);
            this.Controls.Add(this.ppg_axisProperty);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Frm_AxisSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "轴参数";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_AxisSetting_FormClosing);
            this.Load += new System.EventHandler(this.Frm_AxisSetting_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PropertyGrid ppg_axisProperty;


    }
}