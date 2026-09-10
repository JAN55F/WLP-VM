using Controls;
namespace VMPro
{
    partial class Frm_GeneralSettings
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_drawTemplateRegionRectangle1 = new System.Windows.Forms.Button();
            this.tbx_companyName = new Controls.CTextBox();
            this.tbx_dataPath = new Controls.CTextBox();
            this.cbo_lanuage = new Controls.CComboBox();
            this.ckb_dataSaveDays = new Controls.CNumericUpDown();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "语      言：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "公司名称：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "数据路径：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 16;
            this.label4.Text = "存储天数：";
            // 
            // btn_drawTemplateRegionRectangle1
            // 
            this.btn_drawTemplateRegionRectangle1.BackColor = System.Drawing.Color.White;
            this.btn_drawTemplateRegionRectangle1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_drawTemplateRegionRectangle1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_drawTemplateRegionRectangle1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btn_drawTemplateRegionRectangle1.FlatAppearance.BorderSize = 0;
            this.btn_drawTemplateRegionRectangle1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btn_drawTemplateRegionRectangle1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btn_drawTemplateRegionRectangle1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_drawTemplateRegionRectangle1.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_drawTemplateRegionRectangle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_drawTemplateRegionRectangle1.Location = new System.Drawing.Point(356, 62);
            this.btn_drawTemplateRegionRectangle1.Name = "btn_drawTemplateRegionRectangle1";
            this.btn_drawTemplateRegionRectangle1.Size = new System.Drawing.Size(28, 23);
            this.btn_drawTemplateRegionRectangle1.TabIndex = 19;
            this.btn_drawTemplateRegionRectangle1.TabStop = false;
            this.btn_drawTemplateRegionRectangle1.Text = "...";
            this.btn_drawTemplateRegionRectangle1.UseVisualStyleBackColor = false;
            this.btn_drawTemplateRegionRectangle1.Click += new System.EventHandler(this.btn_drawTemplateRegionRectangle1_Click);
            // 
            // tbx_companyName
            // 
            this.tbx_companyName.BackColor = System.Drawing.Color.White;
            this.tbx_companyName.DefaultText = "请输入公司名称";
            this.tbx_companyName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_companyName.Location = new System.Drawing.Point(80, 38);
            this.tbx_companyName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_companyName.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_companyName.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_companyName.Name = "tbx_companyName";
            this.tbx_companyName.PasswordChar = false;
            this.tbx_companyName.Size = new System.Drawing.Size(274, 22);
            this.tbx_companyName.TabIndex = 21;
            this.tbx_companyName.TextStr = "";
            // 
            // tbx_dataPath
            // 
            this.tbx_dataPath.BackColor = System.Drawing.Color.White;
            this.tbx_dataPath.DefaultText = "请点击右侧按钮指定数据存储路径";
            this.tbx_dataPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_dataPath.Location = new System.Drawing.Point(80, 65);
            this.tbx_dataPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_dataPath.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_dataPath.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_dataPath.Name = "tbx_dataPath";
            this.tbx_dataPath.PasswordChar = false;
            this.tbx_dataPath.Size = new System.Drawing.Size(274, 22);
            this.tbx_dataPath.TabIndex = 22;
            this.tbx_dataPath.TextStr = "";
            // 
            // cbo_lanuage
            // 
            this.cbo_lanuage.BackColor = System.Drawing.Color.White;
            this.cbo_lanuage.CanEdit = false;
            this.cbo_lanuage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbo_lanuage.Items = new string[] {
        "简体中文",
        "英文"};
            this.cbo_lanuage.Location = new System.Drawing.Point(80, 10);
            this.cbo_lanuage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbo_lanuage.Name = "cbo_lanuage";
            this.cbo_lanuage.SelectedIndex = 0;
            this.cbo_lanuage.Size = new System.Drawing.Size(120, 26);
            this.cbo_lanuage.TabIndex = 23;
            this.cbo_lanuage.TextStr = "简体中文";
            // 
            // ckb_dataSaveDays
            // 
            this.ckb_dataSaveDays.BackColor = System.Drawing.Color.White;
            this.ckb_dataSaveDays.DecimalPlaces = 0;
            this.ckb_dataSaveDays.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_dataSaveDays.Incremeent = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ckb_dataSaveDays.Location = new System.Drawing.Point(80, 89);
            this.ckb_dataSaveDays.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_dataSaveDays.MaximumSize = new System.Drawing.Size(300, 26);
            this.ckb_dataSaveDays.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.ckb_dataSaveDays.MinimumSize = new System.Drawing.Size(50, 26);
            this.ckb_dataSaveDays.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ckb_dataSaveDays.Name = "ckb_dataSaveDays";
            this.ckb_dataSaveDays.Size = new System.Drawing.Size(120, 26);
            this.ckb_dataSaveDays.TabIndex = 24;
            this.ckb_dataSaveDays.Value = 0D;
            // 
            // Frm_GeneralSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(545, 427);
            this.Controls.Add(this.ckb_dataSaveDays);
            this.Controls.Add(this.cbo_lanuage);
            this.Controls.Add(this.tbx_dataPath);
            this.Controls.Add(this.tbx_companyName);
            this.Controls.Add(this.btn_drawTemplateRegionRectangle1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_GeneralSettings";
            this.Text = "Frm_GeneralSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Button btn_drawTemplateRegionRectangle1;
        public CTextBox tbx_companyName;
        public CTextBox tbx_dataPath;
        public CComboBox cbo_lanuage;
        public CNumericUpDown ckb_dataSaveDays;
    }
}