namespace VMPro
{
    partial class Frm_SubImageTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_SubImageTool));
            this.cbx_standardImage = new System.Windows.Forms.ComboBox();
            this.btn_runImageSubTool = new System.Windows.Forms.Button();
            this.label107 = new System.Windows.Forms.Label();
            this.ckb_subImageToolEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_help = new System.Windows.Forms.ToolStripButton();
            this.btn_subResultImage = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbx_standardImage
            // 
            this.cbx_standardImage.BackColor = System.Drawing.Color.LightGray;
            this.cbx_standardImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_standardImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbx_standardImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_standardImage.FormattingEnabled = true;
            this.cbx_standardImage.Items.AddRange(new object[] {
            ""});
            this.cbx_standardImage.Location = new System.Drawing.Point(75, 81);
            this.cbx_standardImage.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_standardImage.Name = "cbx_standardImage";
            this.cbx_standardImage.Size = new System.Drawing.Size(283, 25);
            this.cbx_standardImage.TabIndex = 78;
            this.cbx_standardImage.SelectedIndexChanged += new System.EventHandler(this.cbo_templateImageSelect_SelectedIndexChanged);
            // 
            // btn_runImageSubTool
            // 
            this.btn_runImageSubTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runImageSubTool.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runImageSubTool.Location = new System.Drawing.Point(260, 128);
            this.btn_runImageSubTool.Margin = new System.Windows.Forms.Padding(2);
            this.btn_runImageSubTool.Name = "btn_runImageSubTool";
            this.btn_runImageSubTool.Size = new System.Drawing.Size(98, 45);
            this.btn_runImageSubTool.TabIndex = 86;
            this.btn_runImageSubTool.Text = "运行";
            this.btn_runImageSubTool.UseVisualStyleBackColor = true;
            this.btn_runImageSubTool.Click += new System.EventHandler(this.btn_runImageSubTool_Click);
            // 
            // label107
            // 
            this.label107.AutoSize = true;
            this.label107.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label107.Location = new System.Drawing.Point(14, 84);
            this.label107.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label107.Name = "label107";
            this.label107.Size = new System.Drawing.Size(68, 17);
            this.label107.TabIndex = 77;
            this.label107.Text = "模板图像：";
            // 
            // ckb_subImageToolEnable
            // 
            this.ckb_subImageToolEnable.AutoSize = true;
            this.ckb_subImageToolEnable.BackColor = System.Drawing.Color.Transparent;
            this.ckb_subImageToolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_subImageToolEnable.Location = new System.Drawing.Point(331, 34);
            this.ckb_subImageToolEnable.Margin = new System.Windows.Forms.Padding(2);
            this.ckb_subImageToolEnable.Name = "ckb_subImageToolEnable";
            this.ckb_subImageToolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_subImageToolEnable.TabIndex = 90;
            this.ckb_subImageToolEnable.Text = "启用";
            this.ckb_subImageToolEnable.UseVisualStyleBackColor = false;
            this.ckb_subImageToolEnable.CheckedChanged += new System.EventHandler(this.ckb_subImageToolEnable_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_runTool,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.tsb_resetTool,
            this.tsb_help});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(4, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.toolStrip1.Size = new System.Drawing.Size(311, 25);
            this.toolStrip1.TabIndex = 93;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_runTool
            // 
            this.tsb_runTool.AutoSize = false;
            this.tsb_runTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_runTool.Image = ((System.Drawing.Image)(resources.GetObject("tsb_runTool.Image")));
            this.tsb_runTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_runTool.Name = "tsb_runTool";
            this.tsb_runTool.RightToLeftAutoMirrorImage = true;
            this.tsb_runTool.Size = new System.Drawing.Size(25, 22);
            this.tsb_runTool.Text = "toolStripButton5";
            this.tsb_runTool.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.tsb_runTool.ToolTipText = "运行工具";
            this.tsb_runTool.Click += new System.EventHandler(this.tsb_runOnce_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.AutoSize = false;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.RightToLeftAutoMirrorImage = true;
            this.toolStripButton1.Size = new System.Drawing.Size(25, 22);
            this.toolStripButton1.Text = "toolStripButton5";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.toolStripButton1.ToolTipText = "运行流程";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // tsb_resetTool
            // 
            this.tsb_resetTool.AutoSize = false;
            this.tsb_resetTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_resetTool.Image = ((System.Drawing.Image)(resources.GetObject("tsb_resetTool.Image")));
            this.tsb_resetTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_resetTool.Name = "tsb_resetTool";
            this.tsb_resetTool.RightToLeftAutoMirrorImage = true;
            this.tsb_resetTool.Size = new System.Drawing.Size(25, 22);
            this.tsb_resetTool.Text = "toolStripButton4";
            this.tsb_resetTool.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.tsb_resetTool.ToolTipText = "复位工具";
            this.tsb_resetTool.Click += new System.EventHandler(this.tsb_resetTool_Click);
            // 
            // tsb_help
            // 
            this.tsb_help.AutoSize = false;
            this.tsb_help.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_help.Image = ((System.Drawing.Image)(resources.GetObject("tsb_help.Image")));
            this.tsb_help.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_help.Name = "tsb_help";
            this.tsb_help.Size = new System.Drawing.Size(25, 22);
            this.tsb_help.Text = "toolStripButton7";
            this.tsb_help.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.tsb_help.ToolTipText = "帮助";
            // 
            // btn_subResultImage
            // 
            this.btn_subResultImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_subResultImage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_subResultImage.Location = new System.Drawing.Point(17, 128);
            this.btn_subResultImage.Margin = new System.Windows.Forms.Padding(2);
            this.btn_subResultImage.Name = "btn_subResultImage";
            this.btn_subResultImage.Size = new System.Drawing.Size(76, 26);
            this.btn_subResultImage.TabIndex = 94;
            this.btn_subResultImage.Text = "结果图像";
            this.btn_subResultImage.UseVisualStyleBackColor = true;
            this.btn_subResultImage.Click += new System.EventHandler(this.btn_subResultImage_Click);
            // 
            // Frm_SubImageTool
            // 
            this.AcceptButton = this.btn_runImageSubTool;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(391, 195);
            this.Controls.Add(this.btn_subResultImage);
            this.Controls.Add(this.cbx_standardImage);
            this.Controls.Add(this.ckb_subImageToolEnable);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btn_runImageSubTool);
            this.Controls.Add(this.label107);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(391, 195);
            this.MinimumSize = new System.Drawing.Size(391, 195);
            this.Name = "Frm_SubImageTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "减图像";
            this.Controls.SetChildIndex(this.label107, 0);
            this.Controls.SetChildIndex(this.btn_runImageSubTool, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.ckb_subImageToolEnable, 0);
            this.Controls.SetChildIndex(this.cbx_standardImage, 0);
            this.Controls.SetChildIndex(this.btn_subResultImage, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        internal System.Windows.Forms.CheckBox ckb_subImageToolEnable;
        public System.Windows.Forms.ComboBox cbx_standardImage;
        public System.Windows.Forms.Button btn_runImageSubTool;
        public System.Windows.Forms.Label label107;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_help;
        public System.Windows.Forms.Button btn_subResultImage;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}