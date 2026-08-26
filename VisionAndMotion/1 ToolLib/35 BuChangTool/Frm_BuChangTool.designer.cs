namespace VMPro
{
    partial class Frm_BuChangTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_BuChangTool));
            this.btn_runDistancePLTool = new System.Windows.Forms.Button();
            this.ckb_distancePLToolEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_help = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_autoGet = new System.Windows.Forms.Button();
            this.tbx_caputurePosU = new System.Windows.Forms.TextBox();
            this.tbx_caputurePosX = new System.Windows.Forms.TextBox();
            this.tbx_caputurePosY = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tbx_pickPosOffsetU = new System.Windows.Forms.TextBox();
            this.tbx_pickPosOffsetX = new System.Windows.Forms.TextBox();
            this.tbx_pickPosOffsetY = new System.Windows.Forms.TextBox();
            this.tbx_pickPosU = new System.Windows.Forms.TextBox();
            this.tbx_pickPosX = new System.Windows.Forms.TextBox();
            this.tbx_pickPosY = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_runDistancePLTool
            // 
            this.btn_runDistancePLTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runDistancePLTool.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runDistancePLTool.Location = new System.Drawing.Point(461, 358);
            this.btn_runDistancePLTool.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_runDistancePLTool.Name = "btn_runDistancePLTool";
            this.btn_runDistancePLTool.Size = new System.Drawing.Size(98, 45);
            this.btn_runDistancePLTool.TabIndex = 15;
            this.btn_runDistancePLTool.Text = "运行";
            this.btn_runDistancePLTool.UseVisualStyleBackColor = true;
            this.btn_runDistancePLTool.Click += new System.EventHandler(this.btn_runShapeMatchTool_Click);
            // 
            // ckb_distancePLToolEnable
            // 
            this.ckb_distancePLToolEnable.AutoSize = true;
            this.ckb_distancePLToolEnable.BackColor = System.Drawing.Color.Transparent;
            this.ckb_distancePLToolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_distancePLToolEnable.Location = new System.Drawing.Point(545, 34);
            this.ckb_distancePLToolEnable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_distancePLToolEnable.Name = "ckb_distancePLToolEnable";
            this.ckb_distancePLToolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_distancePLToolEnable.TabIndex = 74;
            this.ckb_distancePLToolEnable.Text = "启用";
            this.ckb_distancePLToolEnable.UseVisualStyleBackColor = false;
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
            this.toolStrip1.Size = new System.Drawing.Size(525, 25);
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
            this.toolStripButton1.Size = new System.Drawing.Size(25, 22);
            this.toolStripButton1.Text = "toolStripButton1";
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_autoGet);
            this.groupBox2.Controls.Add(this.tbx_caputurePosU);
            this.groupBox2.Controls.Add(this.tbx_caputurePosX);
            this.groupBox2.Controls.Add(this.tbx_caputurePosY);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(24, 71);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(180, 157);
            this.groupBox2.TabIndex = 95;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "模板特征点机械坐标";
            // 
            // btn_autoGet
            // 
            this.btn_autoGet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_autoGet.Location = new System.Drawing.Point(77, 117);
            this.btn_autoGet.Name = "btn_autoGet";
            this.btn_autoGet.Size = new System.Drawing.Size(75, 31);
            this.btn_autoGet.TabIndex = 80;
            this.btn_autoGet.Text = "自动获取";
            this.btn_autoGet.UseVisualStyleBackColor = true;
            this.btn_autoGet.Click += new System.EventHandler(this.btn_autoGet_Click);
            // 
            // tbx_caputurePosU
            // 
            this.tbx_caputurePosU.Location = new System.Drawing.Point(52, 88);
            this.tbx_caputurePosU.Name = "tbx_caputurePosU";
            this.tbx_caputurePosU.Size = new System.Drawing.Size(100, 23);
            this.tbx_caputurePosU.TabIndex = 76;
            this.tbx_caputurePosU.TextChanged += new System.EventHandler(this.tbx_caputurePosU_TextChanged);
            // 
            // tbx_caputurePosX
            // 
            this.tbx_caputurePosX.Location = new System.Drawing.Point(52, 28);
            this.tbx_caputurePosX.Name = "tbx_caputurePosX";
            this.tbx_caputurePosX.Size = new System.Drawing.Size(100, 23);
            this.tbx_caputurePosX.TabIndex = 74;
            this.tbx_caputurePosX.TextChanged += new System.EventHandler(this.tbx_caputurePosX_TextChanged);
            // 
            // tbx_caputurePosY
            // 
            this.tbx_caputurePosY.Location = new System.Drawing.Point(52, 58);
            this.tbx_caputurePosY.Name = "tbx_caputurePosY";
            this.tbx_caputurePosY.Size = new System.Drawing.Size(100, 23);
            this.tbx_caputurePosY.TabIndex = 75;
            this.tbx_caputurePosY.TextChanged += new System.EventHandler(this.tbx_caputurePosY_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 17);
            this.label2.TabIndex = 77;
            this.label2.Text = "X：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 17);
            this.label3.TabIndex = 78;
            this.label3.Text = "Y：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 17);
            this.label4.TabIndex = 79;
            this.label4.Text = "U：";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.White;
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.tbx_pickPosOffsetU);
            this.groupBox3.Controls.Add(this.tbx_pickPosOffsetX);
            this.groupBox3.Controls.Add(this.tbx_pickPosOffsetY);
            this.groupBox3.Controls.Add(this.tbx_pickPosU);
            this.groupBox3.Controls.Add(this.tbx_pickPosX);
            this.groupBox3.Controls.Add(this.tbx_pickPosY);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(221, 78);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(180, 150);
            this.groupBox3.TabIndex = 105;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "工作点位置坐标";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(144, 48);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(30, 17);
            this.label20.TabIndex = 86;
            this.label20.Text = "mm";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(144, 75);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(30, 17);
            this.label21.TabIndex = 87;
            this.label21.Text = "mm";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(144, 98);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(20, 17);
            this.label22.TabIndex = 88;
            this.label22.Text = "度";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(92, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 17);
            this.label10.TabIndex = 83;
            this.label10.Text = "补偿量";
            // 
            // tbx_pickPosOffsetU
            // 
            this.tbx_pickPosOffsetU.Location = new System.Drawing.Point(95, 93);
            this.tbx_pickPosOffsetU.Name = "tbx_pickPosOffsetU";
            this.tbx_pickPosOffsetU.Size = new System.Drawing.Size(46, 23);
            this.tbx_pickPosOffsetU.TabIndex = 82;
            this.tbx_pickPosOffsetU.TextChanged += new System.EventHandler(this.tbx_pickPosOffsetU_TextChanged);
            // 
            // tbx_pickPosOffsetX
            // 
            this.tbx_pickPosOffsetX.Location = new System.Drawing.Point(95, 45);
            this.tbx_pickPosOffsetX.Name = "tbx_pickPosOffsetX";
            this.tbx_pickPosOffsetX.Size = new System.Drawing.Size(46, 23);
            this.tbx_pickPosOffsetX.TabIndex = 80;
            this.tbx_pickPosOffsetX.TextChanged += new System.EventHandler(this.tbx_pickPosOffsetX_TextChanged);
            // 
            // tbx_pickPosOffsetY
            // 
            this.tbx_pickPosOffsetY.Location = new System.Drawing.Point(95, 69);
            this.tbx_pickPosOffsetY.Name = "tbx_pickPosOffsetY";
            this.tbx_pickPosOffsetY.Size = new System.Drawing.Size(46, 23);
            this.tbx_pickPosOffsetY.TabIndex = 81;
            this.tbx_pickPosOffsetY.TextChanged += new System.EventHandler(this.tbx_pickPosOffsetY_TextChanged);
            // 
            // tbx_pickPosU
            // 
            this.tbx_pickPosU.Location = new System.Drawing.Point(27, 93);
            this.tbx_pickPosU.Name = "tbx_pickPosU";
            this.tbx_pickPosU.Size = new System.Drawing.Size(67, 23);
            this.tbx_pickPosU.TabIndex = 76;
            this.tbx_pickPosU.TextChanged += new System.EventHandler(this.tbx_pickPosU_TextChanged);
            // 
            // tbx_pickPosX
            // 
            this.tbx_pickPosX.Location = new System.Drawing.Point(27, 45);
            this.tbx_pickPosX.Name = "tbx_pickPosX";
            this.tbx_pickPosX.Size = new System.Drawing.Size(67, 23);
            this.tbx_pickPosX.TabIndex = 74;
            this.tbx_pickPosX.TextChanged += new System.EventHandler(this.tbx_pickPosX_TextChanged);
            // 
            // tbx_pickPosY
            // 
            this.tbx_pickPosY.Location = new System.Drawing.Point(27, 69);
            this.tbx_pickPosY.Name = "tbx_pickPosY";
            this.tbx_pickPosY.Size = new System.Drawing.Size(67, 23);
            this.tbx_pickPosY.TabIndex = 75;
            this.tbx_pickPosY.TextChanged += new System.EventHandler(this.tbx_pickPosY_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 17);
            this.label6.TabIndex = 77;
            this.label6.Text = "X：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 17);
            this.label7.TabIndex = 78;
            this.label7.Text = "Y：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 96);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 17);
            this.label8.TabIndex = 79;
            this.label8.Text = "U：";
            // 
            // Frm_BuChangTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(605, 430);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.ckb_distancePLToolEnable);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btn_runDistancePLTool);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(605, 430);
            this.MinimumSize = new System.Drawing.Size(605, 430);
            this.Name = "Frm_BuChangTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "点线距离";
            this.Controls.SetChildIndex(this.btn_runDistancePLTool, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.ckb_distancePLToolEnable, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox ckb_distancePLToolEnable;
        public System.Windows.Forms.Button btn_runDistancePLTool;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_help;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.TextBox tbx_caputurePosU;
        public System.Windows.Forms.TextBox tbx_caputurePosX;
        public System.Windows.Forms.TextBox tbx_caputurePosY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_autoGet;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox tbx_pickPosOffsetU;
        public System.Windows.Forms.TextBox tbx_pickPosOffsetX;
        public System.Windows.Forms.TextBox tbx_pickPosOffsetY;
        public System.Windows.Forms.TextBox tbx_pickPosU;
        public System.Windows.Forms.TextBox tbx_pickPosX;
        public System.Windows.Forms.TextBox tbx_pickPosY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}