using Controls;

namespace VMPro
{
    partial class Frm_ProjetSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbx_programTitle = new Controls.CTextBox();
            this.checkBox2 = new Controls.CCheckBox();
            this.checkBox1 = new Controls.CCheckBox();
            this.checkBox3 = new Controls.CCheckBox();
            this.ckb_vitualCard = new Controls.CCheckBox();
            this.radioButton1 = new Controls.CRadioBox();
            this.radioButton2 = new Controls.CRadioBox();
            this.radioButton3 = new Controls.CRadioBox();
            this.cbx_cardType = new Controls.CComboBox();
            this.btn_drawTemplateRegionRectangle2 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(15, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "板卡型号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "项目名称：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(15, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 15;
            this.label3.Text = "页面选择：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(15, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 20;
            this.label4.Text = "默认页面：";
            // 
            // tbx_programTitle
            // 
            this.tbx_programTitle.BackColor = System.Drawing.Color.White;
            this.tbx_programTitle.DefaultText = "请输入项目名称";
            this.tbx_programTitle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_programTitle.Location = new System.Drawing.Point(78, 11);
            this.tbx_programTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_programTitle.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_programTitle.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_programTitle.Name = "tbx_programTitle";
            this.tbx_programTitle.PasswordChar = false;
            this.tbx_programTitle.Size = new System.Drawing.Size(274, 22);
            this.tbx_programTitle.TabIndex = 24;
            this.tbx_programTitle.TextStr = "";
            // 
            // checkBox2
            // 
            this.checkBox2.BackColor = System.Drawing.Color.White;
            this.checkBox2.Checked = false;
            this.checkBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox2.Location = new System.Drawing.Point(164, 65);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(97, 27);
            this.checkBox2.TabIndex = 26;
            this.checkBox2.TextStr = "视觉";
            // 
            // checkBox1
            // 
            this.checkBox1.BackColor = System.Drawing.Color.White;
            this.checkBox1.Checked = false;
            this.checkBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox1.Location = new System.Drawing.Point(77, 65);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(97, 27);
            this.checkBox1.TabIndex = 25;
            this.checkBox1.TextStr = "首页";
            // 
            // checkBox3
            // 
            this.checkBox3.BackColor = System.Drawing.Color.White;
            this.checkBox3.Checked = false;
            this.checkBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox3.Location = new System.Drawing.Point(251, 65);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(97, 27);
            this.checkBox3.TabIndex = 27;
            this.checkBox3.TextStr = "运动";
            // 
            // ckb_vitualCard
            // 
            this.ckb_vitualCard.BackColor = System.Drawing.Color.White;
            this.ckb_vitualCard.Checked = false;
            this.ckb_vitualCard.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_vitualCard.Location = new System.Drawing.Point(354, 43);
            this.ckb_vitualCard.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_vitualCard.Name = "ckb_vitualCard";
            this.ckb_vitualCard.Size = new System.Drawing.Size(97, 27);
            this.ckb_vitualCard.TabIndex = 28;
            this.ckb_vitualCard.TextStr = "虚拟";
            this.ckb_vitualCard.Visible = false;
            // 
            // radioButton1
            // 
            this.radioButton1.BackColor = System.Drawing.Color.White;
            this.radioButton1.Checked = false;
            this.radioButton1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton1.Location = new System.Drawing.Point(75, 89);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(97, 24);
            this.radioButton1.TabIndex = 29;
            this.radioButton1.TextStr = "首页";
            // 
            // radioButton2
            // 
            this.radioButton2.BackColor = System.Drawing.Color.White;
            this.radioButton2.Checked = false;
            this.radioButton2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton2.Location = new System.Drawing.Point(162, 89);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(97, 24);
            this.radioButton2.TabIndex = 30;
            this.radioButton2.TextStr = "视觉";
            // 
            // radioButton3
            // 
            this.radioButton3.BackColor = System.Drawing.Color.White;
            this.radioButton3.Checked = false;
            this.radioButton3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton3.Location = new System.Drawing.Point(249, 89);
            this.radioButton3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(97, 24);
            this.radioButton3.TabIndex = 31;
            this.radioButton3.TextStr = "运动";
            // 
            // cbx_cardType
            // 
            this.cbx_cardType.BackColor = System.Drawing.Color.White;
            this.cbx_cardType.CanEdit = false;
            this.cbx_cardType.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_cardType.Items = new string[] {
        "无",
        "固高_GTS",
        "雷塞_DMC2210",
        "雷塞_DMC2410",
        "雷赛_IOC0640",
        "凌华_AMP204C",
        "软赢_WMX",
        "安川_MP3100"};
            this.cbx_cardType.Location = new System.Drawing.Point(78, 38);
            this.cbx_cardType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_cardType.Name = "cbx_cardType";
            this.cbx_cardType.SelectedIndex = -1;
            this.cbx_cardType.Size = new System.Drawing.Size(274, 26);
            this.cbx_cardType.TabIndex = 32;
            this.cbx_cardType.TextStr = "";
            // 
            // btn_drawTemplateRegionRectangle2
            // 
            this.btn_drawTemplateRegionRectangle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.btn_drawTemplateRegionRectangle2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_drawTemplateRegionRectangle2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_drawTemplateRegionRectangle2.FlatAppearance.BorderSize = 0;
            this.btn_drawTemplateRegionRectangle2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btn_drawTemplateRegionRectangle2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btn_drawTemplateRegionRectangle2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_drawTemplateRegionRectangle2.ForeColor = System.Drawing.Color.White;
            this.btn_drawTemplateRegionRectangle2.Location = new System.Drawing.Point(77, 136);
            this.btn_drawTemplateRegionRectangle2.Name = "btn_drawTemplateRegionRectangle2";
            this.btn_drawTemplateRegionRectangle2.Size = new System.Drawing.Size(70, 30);
            this.btn_drawTemplateRegionRectangle2.TabIndex = 108;
            this.btn_drawTemplateRegionRectangle2.TabStop = false;
            this.btn_drawTemplateRegionRectangle2.Text = "导入项目";
            this.btn_drawTemplateRegionRectangle2.UseVisualStyleBackColor = false;
            this.btn_drawTemplateRegionRectangle2.Click += new System.EventHandler(this.btn_drawTemplateRegionRectangle2_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(153, 136);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 30);
            this.button2.TabIndex = 109;
            this.button2.TabStop = false;
            this.button2.Text = "导出项目";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Frm_ProjetSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(545, 427);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btn_drawTemplateRegionRectangle2);
            this.Controls.Add(this.cbx_cardType);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.ckb_vitualCard);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.tbx_programTitle);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_ProjetSettings";
            this.Text = "Frm_ProjetSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Label label4;
        public CTextBox tbx_programTitle;
        public CCheckBox checkBox2;
        public CCheckBox checkBox3;
        public CCheckBox checkBox1;
        public CCheckBox ckb_vitualCard;
        public CRadioBox radioButton1;
        public CRadioBox radioButton2;
        public CRadioBox radioButton3;
        public CComboBox cbx_cardType;
        internal System.Windows.Forms.Button btn_drawTemplateRegionRectangle2;
        internal System.Windows.Forms.Button button2;
    }
}