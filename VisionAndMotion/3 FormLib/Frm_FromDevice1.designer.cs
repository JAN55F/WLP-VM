using Controls;
namespace VMPro
{
    partial class Frm_FromDevice1
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
            this.label147 = new System.Windows.Forms.Label();
            this.tkb_exposure = new System.Windows.Forms.TrackBar();
            this.label123 = new System.Windows.Forms.Label();
            this.label148 = new System.Windows.Forms.Label();
            this.cbx_deviceList = new Controls.CComboBox();
            this.tbx_exposure = new Controls.CNumericUpDown();
            this.btn_displayImage = new Controls.CButton();
            this.btn_saveImage = new Controls.CButton();
            ((System.ComponentModel.ISupportInitialize)(this.tkb_exposure)).BeginInit();
            this.SuspendLayout();
            // 
            // label147
            // 
            this.label147.AutoSize = true;
            this.label147.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label147.Location = new System.Drawing.Point(2, 83);
            this.label147.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label147.Name = "label147";
            this.label147.Size = new System.Drawing.Size(68, 17);
            this.label147.TabIndex = 94;
            this.label147.Text = "曝光时间：";
            // 
            // tkb_exposure
            // 
            this.tkb_exposure.AutoSize = false;
            this.tkb_exposure.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tkb_exposure.Location = new System.Drawing.Point(1, 105);
            this.tkb_exposure.Margin = new System.Windows.Forms.Padding(2);
            this.tkb_exposure.Maximum = 2000;
            this.tkb_exposure.Minimum = 1;
            this.tkb_exposure.Name = "tkb_exposure";
            this.tkb_exposure.Size = new System.Drawing.Size(268, 19);
            this.tkb_exposure.TabIndex = 95;
            this.tkb_exposure.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tkb_exposure.Value = 1;
            this.tkb_exposure.Scroll += new System.EventHandler(this.tkb_exposure_Scroll);
            // 
            // label123
            // 
            this.label123.AutoSize = true;
            this.label123.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label123.Location = new System.Drawing.Point(2, 18);
            this.label123.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label123.Name = "label123";
            this.label123.Size = new System.Drawing.Size(56, 17);
            this.label123.TabIndex = 98;
            this.label123.Text = "设备列表";
            // 
            // label148
            // 
            this.label148.AutoSize = true;
            this.label148.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label148.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label148.Location = new System.Drawing.Point(185, 83);
            this.label148.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label148.Name = "label148";
            this.label148.Size = new System.Drawing.Size(25, 17);
            this.label148.TabIndex = 97;
            this.label148.Text = "ms";
            // 
            // cbx_deviceList
            // 
            this.cbx_deviceList.BackColor = System.Drawing.Color.White;
            this.cbx_deviceList.CanEdit = false;
            this.cbx_deviceList.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_deviceList.Items = new string[0];
            this.cbx_deviceList.Location = new System.Drawing.Point(5, 32);
            this.cbx_deviceList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_deviceList.Name = "cbx_deviceList";
            this.cbx_deviceList.SelectedIndex = -1;
            this.cbx_deviceList.Size = new System.Drawing.Size(258, 26);
            this.cbx_deviceList.TabIndex = 104;
            this.cbx_deviceList.TextStr = "";
            this.cbx_deviceList.SelectedIndexChanged += new Controls.DSelectedIndexChanged(this.cbx_deviceList_SelectedIndexChanged);
            // 
            // tbx_exposure
            // 
            this.tbx_exposure.BackColor = System.Drawing.Color.White;
            this.tbx_exposure.DecimalPlaces = 3;
            this.tbx_exposure.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_exposure.Incremeent = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.tbx_exposure.Location = new System.Drawing.Point(65, 78);
            this.tbx_exposure.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_exposure.MaximumSize = new System.Drawing.Size(300, 26);
            this.tbx_exposure.MaxValue = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.tbx_exposure.MinimumSize = new System.Drawing.Size(50, 26);
            this.tbx_exposure.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbx_exposure.Name = "tbx_exposure";
            this.tbx_exposure.Size = new System.Drawing.Size(120, 26);
            this.tbx_exposure.TabIndex = 103;
            this.tbx_exposure.Value = 20D;
            this.tbx_exposure.ValueChanged += new Controls.DValueChanged(this.tbx_exposure_ValueChanged);
            // 
            // btn_displayImage
            // 
            this.btn_displayImage.BackColor = System.Drawing.Color.White;
            this.btn_displayImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_displayImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_displayImage.Location = new System.Drawing.Point(5, 146);
            this.btn_displayImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_displayImage.Name = "btn_displayImage";
            this.btn_displayImage.Size = new System.Drawing.Size(67, 28);
            this.btn_displayImage.TabIndex = 105;
            this.btn_displayImage.TextStr = "相机实时";
            // 
            // btn_saveImage
            // 
            this.btn_saveImage.BackColor = System.Drawing.Color.White;
            this.btn_saveImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_saveImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_saveImage.Location = new System.Drawing.Point(78, 146);
            this.btn_saveImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_saveImage.Name = "btn_saveImage";
            this.btn_saveImage.Size = new System.Drawing.Size(67, 28);
            this.btn_saveImage.TabIndex = 106;
            this.btn_saveImage.TextStr = "图像另存";
            // 
            // Frm_FromDevice1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(270, 294);
            this.Controls.Add(this.btn_saveImage);
            this.Controls.Add(this.btn_displayImage);
            this.Controls.Add(this.cbx_deviceList);
            this.Controls.Add(this.tbx_exposure);
            this.Controls.Add(this.label148);
            this.Controls.Add(this.label147);
            this.Controls.Add(this.tkb_exposure);
            this.Controls.Add(this.label123);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_FromDevice1";
            this.Text = "Frm_AcquistionFromDevice";
            ((System.ComponentModel.ISupportInitialize)(this.tkb_exposure)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label147;
        internal System.Windows.Forms.TrackBar tkb_exposure;
        private System.Windows.Forms.Label label123;
        private System.Windows.Forms.Label label148;
        internal CNumericUpDown tbx_exposure;
        internal CComboBox cbx_deviceList;
        internal CButton btn_displayImage;
        internal CButton btn_saveImage;
    }
}