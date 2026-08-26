namespace VMPro
{
    partial class Frm_LayoutManage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_LayoutManage));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.cbx_layoutList = new System.Windows.Forms.ComboBox();
            this.btn_runOnce = new System.Windows.Forms.Button();
            this.btn_deleteLayout = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Location = new System.Drawing.Point(10, 6);
            this.lbl_title.Size = new System.Drawing.Size(56, 17);
            this.lbl_title.Text = "布局管理";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 6);
            // 
            // button100
            // 
            this.button100.FlatAppearance.BorderSize = 0;
            this.button100.Location = new System.Drawing.Point(491, 0);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridView1.Location = new System.Drawing.Point(20, 50);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(304, 211);
            this.dataGridView1.TabIndex = 4;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "编号";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 60;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "名称";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 220;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "当前布局：";
            // 
            // cbx_layoutList
            // 
            this.cbx_layoutList.BackColor = System.Drawing.Color.DarkGray;
            this.cbx_layoutList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_layoutList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbx_layoutList.FormattingEnabled = true;
            this.cbx_layoutList.Location = new System.Drawing.Point(79, 22);
            this.cbx_layoutList.Name = "cbx_layoutList";
            this.cbx_layoutList.Size = new System.Drawing.Size(245, 25);
            this.cbx_layoutList.TabIndex = 6;
            this.cbx_layoutList.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btn_runOnce
            // 
            this.btn_runOnce.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_runOnce.BackgroundImage")));
            this.btn_runOnce.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_runOnce.FlatAppearance.BorderSize = 0;
            this.btn_runOnce.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_runOnce.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_runOnce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runOnce.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runOnce.ForeColor = System.Drawing.Color.White;
            this.btn_runOnce.Location = new System.Drawing.Point(335, 50);
            this.btn_runOnce.Margin = new System.Windows.Forms.Padding(5);
            this.btn_runOnce.Name = "btn_runOnce";
            this.btn_runOnce.Size = new System.Drawing.Size(90, 34);
            this.btn_runOnce.TabIndex = 106;
            this.btn_runOnce.Text = "新增当前布局";
            this.btn_runOnce.UseVisualStyleBackColor = true;
            this.btn_runOnce.Click += new System.EventHandler(this.btn_runOnce_Click);
            this.btn_runOnce.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_runOnce.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // btn_deleteLayout
            // 
            this.btn_deleteLayout.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_deleteLayout.BackgroundImage")));
            this.btn_deleteLayout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_deleteLayout.FlatAppearance.BorderSize = 0;
            this.btn_deleteLayout.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_deleteLayout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_deleteLayout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_deleteLayout.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_deleteLayout.ForeColor = System.Drawing.Color.White;
            this.btn_deleteLayout.Location = new System.Drawing.Point(335, 90);
            this.btn_deleteLayout.Margin = new System.Windows.Forms.Padding(5);
            this.btn_deleteLayout.Name = "btn_deleteLayout";
            this.btn_deleteLayout.Size = new System.Drawing.Size(90, 34);
            this.btn_deleteLayout.TabIndex = 107;
            this.btn_deleteLayout.Text = "删除选中布局";
            this.btn_deleteLayout.UseVisualStyleBackColor = true;
            this.btn_deleteLayout.Click += new System.EventHandler(this.button4_Click);
            this.btn_deleteLayout.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.btn_deleteLayout.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // button4
            // 
            this.button4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(334, 225);
            this.button4.Margin = new System.Windows.Forms.Padding(5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(90, 36);
            this.button4.TabIndex = 108;
            this.button4.Text = "确定";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            this.button4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.button4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.cbx_layoutList);
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.btn_deleteLayout);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btn_runOnce);
            this.panel2.Location = new System.Drawing.Point(2, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(443, 280);
            this.panel2.TabIndex = 109;
            // 
            // Frm_LayoutManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(447, 308);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(447, 308);
            this.MinimumSize = new System.Drawing.Size(447, 308);
            this.Name = "Frm_LayoutManage";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "布局管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_LayoutManage_FormClosing);
            this.Load += new System.EventHandler(this.Frm_LayoutManage_Load);
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbx_layoutList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        public System.Windows.Forms.Button btn_runOnce;
        public System.Windows.Forms.Button btn_deleteLayout;
        public System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel2;
    }
}