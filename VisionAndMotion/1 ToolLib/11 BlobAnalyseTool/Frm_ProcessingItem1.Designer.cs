namespace VMPro
{
    partial class Frm_ProcessingItem1
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
            this.label5 = new System.Windows.Forms.Label();
            this.cbx_elementType = new System.Windows.Forms.ComboBox();
            this.tbx_elementSize = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_applyAndExit
            // 
            this.btn_applyAndExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_applyAndExit.Location = new System.Drawing.Point(171, 100);
            this.btn_applyAndExit.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btn_applyAndExit.Name = "btn_applyAndExit";
            this.btn_applyAndExit.Size = new System.Drawing.Size(82, 28);
            this.btn_applyAndExit.TabIndex = 21;
            this.btn_applyAndExit.Tag = "";
            this.btn_applyAndExit.Text = "应用并退出";
            this.btn_applyAndExit.UseVisualStyleBackColor = true;
            this.btn_applyAndExit.Click += new System.EventHandler(this.btn_saveAndExit_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 17);
            this.label5.TabIndex = 18;
            this.label5.Text = "结构单元：";
            // 
            // cbx_elementType
            // 
            this.cbx_elementType.BackColor = System.Drawing.Color.LightGray;
            this.cbx_elementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_elementType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbx_elementType.FormattingEnabled = true;
            this.cbx_elementType.Items.AddRange(new object[] {
            "circle"});
            this.cbx_elementType.Location = new System.Drawing.Point(113, 21);
            this.cbx_elementType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_elementType.Name = "cbx_elementType";
            this.cbx_elementType.Size = new System.Drawing.Size(115, 25);
            this.cbx_elementType.TabIndex = 17;
            // 
            // tbx_elementSize
            // 
            this.tbx_elementSize.Location = new System.Drawing.Point(113, 56);
            this.tbx_elementSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_elementSize.Name = "tbx_elementSize";
            this.tbx_elementSize.Size = new System.Drawing.Size(115, 23);
            this.tbx_elementSize.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(51, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 17);
            this.label7.TabIndex = 22;
            this.label7.Text = "大小：";
            // 
            // Frm_ProcessingItem1
            // 
            this.AcceptButton = this.btn_applyAndExit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(278, 149);
            this.Controls.Add(this.cbx_elementType);
            this.Controls.Add(this.tbx_elementSize);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btn_applyAndExit);
            this.Controls.Add(this.label5);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_ProcessingItem1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "配置";
            this.Load += new System.EventHandler(this.Frm_ProcessingItemConfig1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button btn_applyAndExit;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.ComboBox cbx_elementType;
        public System.Windows.Forms.TextBox tbx_elementSize;
        public System.Windows.Forms.Label label7;

    }
}