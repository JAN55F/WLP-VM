using Controls;
namespace VMPro
{
    partial class Frm_JobInfo
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
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cComboBox1 = new Controls.CComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbx_imageWindowList = new Controls.CComboBox();
            this.comboBox1 = new Controls.CComboBox();
            this.tbx_jobName = new Controls.CTextBox();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Location = new System.Drawing.Point(6, 6);
            this.lbl_title.Size = new System.Drawing.Size(56, 17);
            this.lbl_title.Text = "流程属性";
            // 
            // button100
            // 
            this.button100.FlatAppearance.BorderSize = 0;
            this.button100.Location = new System.Drawing.Point(570, 0);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(19, 21);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "流程名：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(19, 49);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 67;
            this.label1.Text = "调试窗口：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(19, 76);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 70;
            this.label4.Text = "生产窗口：";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.cComboBox1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.cbx_imageWindowList);
            this.panel2.Controls.Add(this.comboBox1);
            this.panel2.Controls.Add(this.tbx_jobName);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(2, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(469, 332);
            this.panel2.TabIndex = 72;
            // 
            // cComboBox1
            // 
            this.cComboBox1.BackColor = System.Drawing.Color.White;
            this.cComboBox1.CanEdit = false;
            this.cComboBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cComboBox1.Items = new string[] {
        "调用时执行",
        "启动后连续执行"};
            this.cComboBox1.Location = new System.Drawing.Point(79, 100);
            this.cComboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cComboBox1.Name = "cComboBox1";
            this.cComboBox1.SelectedIndex = -1;
            this.cComboBox1.Size = new System.Drawing.Size(137, 26);
            this.cComboBox1.TabIndex = 77;
            this.cComboBox1.TextStr = "";
            this.cComboBox1.SelectedIndexChanged += new Controls.DSelectedIndexChanged(this.cComboBox1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(19, 103);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 76;
            this.label3.Text = "执行方式：";
            // 
            // cbx_imageWindowList
            // 
            this.cbx_imageWindowList.BackColor = System.Drawing.Color.White;
            this.cbx_imageWindowList.CanEdit = false;
            this.cbx_imageWindowList.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_imageWindowList.Items = new string[0];
            this.cbx_imageWindowList.Location = new System.Drawing.Point(79, 73);
            this.cbx_imageWindowList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbx_imageWindowList.Name = "cbx_imageWindowList";
            this.cbx_imageWindowList.SelectedIndex = -1;
            this.cbx_imageWindowList.Size = new System.Drawing.Size(137, 26);
            this.cbx_imageWindowList.TabIndex = 74;
            this.cbx_imageWindowList.TextStr = "";
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.White;
            this.comboBox1.CanEdit = false;
            this.comboBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.Items = new string[0];
            this.comboBox1.Location = new System.Drawing.Point(79, 46);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndex = -1;
            this.comboBox1.Size = new System.Drawing.Size(137, 26);
            this.comboBox1.TabIndex = 73;
            this.comboBox1.TextStr = "";
            // 
            // tbx_jobName
            // 
            this.tbx_jobName.BackColor = System.Drawing.Color.White;
            this.tbx_jobName.DefaultText = "请输入流程名";
            this.tbx_jobName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_jobName.Location = new System.Drawing.Point(79, 19);
            this.tbx_jobName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_jobName.MaximumSize = new System.Drawing.Size(400, 22);
            this.tbx_jobName.MinimumSize = new System.Drawing.Size(20, 22);
            this.tbx_jobName.Name = "tbx_jobName";
            this.tbx_jobName.PasswordChar = false;
            this.tbx_jobName.Size = new System.Drawing.Size(202, 22);
            this.tbx_jobName.TabIndex = 72;
            this.tbx_jobName.TextStr = "";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(150)))), ((int)(((byte)(219)))));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(389, 292);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(70, 30);
            this.button3.TabIndex = 111;
            this.button3.TabStop = false;
            this.button3.Text = "保存";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Frm_JobInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(473, 360);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(473, 360);
            this.MinimumSize = new System.Drawing.Size(473, 360);
            this.Name = "Frm_JobInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "当前流程属性";
            this.Load += new System.EventHandler(this.Frm_JobInfo_Load);
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        public CTextBox tbx_jobName;
        internal CComboBox cbx_imageWindowList;
        internal CComboBox comboBox1;
        internal CComboBox cComboBox1;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Button button3;
    }
}