namespace VMPro
{
    partial class Frm_Setting
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("常规");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("项目");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("方案");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("功能");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("用户管理");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("安全", new System.Windows.Forms.TreeNode[] {
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("运行");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Setting));
            this.tvw_setting = new System.Windows.Forms.TreeView();
            this.pnl_window = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Location = new System.Drawing.Point(10, 6);
            this.lbl_title.Size = new System.Drawing.Size(56, 17);
            this.lbl_title.Text = "系统设置";
            // 
            // button100
            // 
            this.button100.FlatAppearance.BorderSize = 0;
            this.button100.Location = new System.Drawing.Point(613, 0);
            // 
            // tvw_setting
            // 
            this.tvw_setting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvw_setting.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tvw_setting.FullRowSelect = true;
            this.tvw_setting.Indent = 25;
            this.tvw_setting.ItemHeight = 28;
            this.tvw_setting.Location = new System.Drawing.Point(10, 11);
            this.tvw_setting.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tvw_setting.Name = "tvw_setting";
            treeNode1.Name = "节点0";
            treeNode1.Text = "常规";
            treeNode2.Name = "节点0";
            treeNode2.Text = "项目";
            treeNode3.Name = "节点0";
            treeNode3.Text = "方案";
            treeNode4.Name = "节点4";
            treeNode4.Text = "功能";
            treeNode5.Name = "节点1";
            treeNode5.Text = "用户管理";
            treeNode6.Name = "节点0";
            treeNode6.Text = "安全";
            treeNode7.Name = "节点0";
            treeNode7.Text = "运行";
            this.tvw_setting.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode6,
            treeNode7});
            this.tvw_setting.ShowLines = false;
            this.tvw_setting.Size = new System.Drawing.Size(143, 475);
            this.tvw_setting.TabIndex = 19;
            this.tvw_setting.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvw_setting_AfterSelect);
            // 
            // pnl_window
            // 
            this.pnl_window.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pnl_window.Location = new System.Drawing.Point(159, 11);
            this.pnl_window.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnl_window.Name = "pnl_window";
            this.pnl_window.Size = new System.Drawing.Size(545, 427);
            this.pnl_window.TabIndex = 20;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.tvw_setting);
            this.panel2.Controls.Add(this.pnl_window);
            this.panel2.Location = new System.Drawing.Point(2, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(713, 497);
            this.panel2.TabIndex = 106;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel5.Location = new System.Drawing.Point(158, 443);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(546, 2);
            this.panel5.TabIndex = 151;
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
            this.button3.Location = new System.Drawing.Point(634, 456);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(70, 30);
            this.button3.TabIndex = 110;
            this.button3.TabStop = false;
            this.button3.Text = "保存";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Frm_Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(717, 525);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(717, 525);
            this.MinimumSize = new System.Drawing.Size(717, 525);
            this.Name = "Frm_Setting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置";
            this.Load += new System.EventHandler(this.Frm_Setting_Load);
            this.Controls.SetChildIndex(this.button100, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TreeView tvw_setting;
        private System.Windows.Forms.Panel pnl_window;
        private System.Windows.Forms.Panel panel2;
        internal System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panel5;


    }
}