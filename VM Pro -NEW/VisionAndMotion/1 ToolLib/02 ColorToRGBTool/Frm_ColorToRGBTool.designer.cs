namespace VMPro
{
    partial class Frm_ColorToRGBTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ColorToRGBTool));
            this.btn_runColorToRGBTool = new System.Windows.Forms.Button();
            this.ckb_colorToRGBToolEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_help = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_runColorToRGBTool
            // 
            this.btn_runColorToRGBTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runColorToRGBTool.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runColorToRGBTool.Location = new System.Drawing.Point(459, 361);
            this.btn_runColorToRGBTool.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_runColorToRGBTool.Name = "btn_runColorToRGBTool";
            this.btn_runColorToRGBTool.Size = new System.Drawing.Size(98, 45);
            this.btn_runColorToRGBTool.TabIndex = 15;
            this.btn_runColorToRGBTool.Text = "运行";
            this.btn_runColorToRGBTool.UseVisualStyleBackColor = true;
            this.btn_runColorToRGBTool.Click += new System.EventHandler(this.btn_runColorToRGBTool_Click);
            // 
            // ckb_colorToRGBToolEnable
            // 
            this.ckb_colorToRGBToolEnable.AutoSize = true;
            this.ckb_colorToRGBToolEnable.BackColor = System.Drawing.Color.Transparent;
            this.ckb_colorToRGBToolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_colorToRGBToolEnable.Location = new System.Drawing.Point(545, 31);
            this.ckb_colorToRGBToolEnable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_colorToRGBToolEnable.Name = "ckb_colorToRGBToolEnable";
            this.ckb_colorToRGBToolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_colorToRGBToolEnable.TabIndex = 74;
            this.ckb_colorToRGBToolEnable.Text = "启用";
            this.ckb_colorToRGBToolEnable.UseVisualStyleBackColor = false;
            this.ckb_colorToRGBToolEnable.CheckedChanged += new System.EventHandler(this.ckb_colorToRGBToolEnable_CheckedChanged);
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
            this.toolStrip1.Size = new System.Drawing.Size(528, 25);
            this.toolStrip1.TabIndex = 88;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_runTool
            // 
            this.tsb_runTool.AutoSize = false;
            this.tsb_runTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_runTool.Image = ((System.Drawing.Image)(resources.GetObject("tsb_runTool.Image")));
            this.tsb_runTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_runTool.Name = "tsb_runTool";
            this.tsb_runTool.Size = new System.Drawing.Size(25, 22);
            this.tsb_runTool.Text = "toolStripButton5";
            this.tsb_runTool.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_runTool.ToolTipText = "运行一次";
            this.tsb_runTool.Click += new System.EventHandler(this.tsb_runTool_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.AutoSize = false;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(25, 22);
            this.toolStripButton1.Text = "toolStripButton5";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
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
            this.tsb_resetTool.Size = new System.Drawing.Size(25, 22);
            this.tsb_resetTool.Text = "toolStripButton4";
            this.tsb_resetTool.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
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
            this.tsb_help.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_help.ToolTipText = "帮助";
            // 
            // Frm_ColorToRGBTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(605, 430);
            this.Controls.Add(this.ckb_colorToRGBToolEnable);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btn_runColorToRGBTool);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(605, 430);
            this.MinimumSize = new System.Drawing.Size(605, 430);
            this.Name = "Frm_ColorToRGBTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "彩图转RGB图";
            this.Controls.SetChildIndex(this.btn_runColorToRGBTool, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.ckb_colorToRGBToolEnable, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox ckb_colorToRGBToolEnable;
        public System.Windows.Forms.Button btn_runColorToRGBTool;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_help;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}