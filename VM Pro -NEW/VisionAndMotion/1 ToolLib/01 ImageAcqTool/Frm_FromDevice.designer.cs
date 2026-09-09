using Controls;
namespace VMPro
{
    partial class Frm_FromDevice
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
            this.label_saveDirectory = new System.Windows.Forms.Label();
            this.tbx_saveDirectory = new System.Windows.Forms.TextBox();
            this.btn_browseSaveDirectory = new System.Windows.Forms.Button();
            this.ckb_useTemplateImage = new System.Windows.Forms.CheckBox();
            this.tbx_templateImagePath = new System.Windows.Forms.TextBox();
            this.btn_browseTemplateImage = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tkb_exposure)).BeginInit();
            this.SuspendLayout();
            // 
            // label147
            // 
            this.label147.AutoSize = true;
            this.label147.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label147.Location = new System.Drawing.Point(2, 98);
            this.label147.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label147.Name = "label147";
            this.label147.Size = new System.Drawing.Size(68, 17);
            this.label147.TabIndex = 94;
            this.label147.Text = "曝光亮度：";
            // 
            // tkb_exposure
            // 
            this.tkb_exposure.AutoSize = false;
            this.tkb_exposure.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tkb_exposure.Location = new System.Drawing.Point(1, 124);
            this.tkb_exposure.Margin = new System.Windows.Forms.Padding(2);
            this.tkb_exposure.Maximum = 1000;
            this.tkb_exposure.Minimum = 1;
            this.tkb_exposure.Name = "tkb_exposure";
            this.tkb_exposure.Size = new System.Drawing.Size(268, 19);
            this.tkb_exposure.TabIndex = 95;
            this.tkb_exposure.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tkb_exposure.Value = 200;
            this.tkb_exposure.ValueChanged += new System.EventHandler(this.tkb_exposure_ValueChanged);
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
            this.label148.Location = new System.Drawing.Point(185, 98);
            this.label148.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label148.Name = "label148";
            this.label148.Size = new System.Drawing.Size(61, 17);
            this.label148.TabIndex = 97;
            this.label148.Text = "0.1 - 100";
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
            1,
            0,
            0,
            65536});
            this.tbx_exposure.Location = new System.Drawing.Point(65, 95);
            this.tbx_exposure.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_exposure.MaximumSize = new System.Drawing.Size(300, 26);
            this.tbx_exposure.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.tbx_exposure.MinimumSize = new System.Drawing.Size(50, 26);
            this.tbx_exposure.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            this.btn_displayImage.Location = new System.Drawing.Point(5, 154);
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
            this.btn_saveImage.Location = new System.Drawing.Point(78, 154);
            this.btn_saveImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_saveImage.Name = "btn_saveImage";
            this.btn_saveImage.Size = new System.Drawing.Size(67, 28);
            this.btn_saveImage.TabIndex = 106;
            this.btn_saveImage.TextStr = "图像另存";
            // 
            // label_saveDirectory
            // 
            this.label_saveDirectory.AutoSize = true;
            this.label_saveDirectory.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_saveDirectory.Location = new System.Drawing.Point(2, 268);
            this.label_saveDirectory.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_saveDirectory.Name = "label_saveDirectory";
            this.label_saveDirectory.Size = new System.Drawing.Size(104, 17);
            this.label_saveDirectory.TabIndex = 107;
            this.label_saveDirectory.Text = "自动保存目录：";
            // 
            // tbx_saveDirectory
            // 
            this.tbx_saveDirectory.BackColor = System.Drawing.Color.White;
            this.tbx_saveDirectory.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_saveDirectory.Location = new System.Drawing.Point(5, 288);
            this.tbx_saveDirectory.Name = "tbx_saveDirectory";
            this.tbx_saveDirectory.ReadOnly = true;
            this.tbx_saveDirectory.Size = new System.Drawing.Size(185, 23);
            this.tbx_saveDirectory.TabIndex = 108;
            // 
            // btn_browseSaveDirectory
            // 
            this.btn_browseSaveDirectory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_browseSaveDirectory.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_browseSaveDirectory.Location = new System.Drawing.Point(196, 287);
            this.btn_browseSaveDirectory.Name = "btn_browseSaveDirectory";
            this.btn_browseSaveDirectory.Size = new System.Drawing.Size(67, 25);
            this.btn_browseSaveDirectory.TabIndex = 109;
            this.btn_browseSaveDirectory.Text = "选择";
            this.btn_browseSaveDirectory.UseVisualStyleBackColor = true;
            // 
            // ckb_useTemplateImage
            // 
            this.ckb_useTemplateImage.AutoSize = true;
            this.ckb_useTemplateImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_useTemplateImage.Location = new System.Drawing.Point(5, 194);
            this.ckb_useTemplateImage.Name = "ckb_useTemplateImage";
            this.ckb_useTemplateImage.Size = new System.Drawing.Size(159, 21);
            this.ckb_useTemplateImage.TabIndex = 110;
            this.ckb_useTemplateImage.Text = "流程运行时使用模板图";
            this.ckb_useTemplateImage.UseVisualStyleBackColor = true;
            // 
            // tbx_templateImagePath
            // 
            this.tbx_templateImagePath.BackColor = System.Drawing.Color.White;
            this.tbx_templateImagePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_templateImagePath.Location = new System.Drawing.Point(5, 224);
            this.tbx_templateImagePath.Name = "tbx_templateImagePath";
            this.tbx_templateImagePath.ReadOnly = true;
            this.tbx_templateImagePath.Size = new System.Drawing.Size(185, 23);
            this.tbx_templateImagePath.TabIndex = 111;
            // 
            // btn_browseTemplateImage
            // 
            this.btn_browseTemplateImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_browseTemplateImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_browseTemplateImage.Location = new System.Drawing.Point(196, 223);
            this.btn_browseTemplateImage.Name = "btn_browseTemplateImage";
            this.btn_browseTemplateImage.Size = new System.Drawing.Size(67, 25);
            this.btn_browseTemplateImage.TabIndex = 112;
            this.btn_browseTemplateImage.Text = "选择";
            this.btn_browseTemplateImage.UseVisualStyleBackColor = true;
            // 
            // Frm_FromDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(270, 340);
            this.Controls.Add(this.btn_browseTemplateImage);
            this.Controls.Add(this.tbx_templateImagePath);
            this.Controls.Add(this.ckb_useTemplateImage);
            this.Controls.Add(this.btn_browseSaveDirectory);
            this.Controls.Add(this.tbx_saveDirectory);
            this.Controls.Add(this.label_saveDirectory);
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
            this.Name = "Frm_FromDevice";
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
        private System.Windows.Forms.Label label_saveDirectory;
        internal System.Windows.Forms.TextBox tbx_saveDirectory;
        internal System.Windows.Forms.Button btn_browseSaveDirectory;
        internal System.Windows.Forms.CheckBox ckb_useTemplateImage;
        internal System.Windows.Forms.TextBox tbx_templateImagePath;
        internal System.Windows.Forms.Button btn_browseTemplateImage;
    }
}
