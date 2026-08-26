namespace VMPro
{
    partial class Frm_CreatePositionTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_CreateROITool));
            this.btn_runDistancePLTool = new System.Windows.Forms.Button();
            this.ckb_createROIToolEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_help = new System.Windows.Forms.ToolStripButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tbx_leftTopRow = new System.Windows.Forms.TextBox();
            this.ckb_leftTopRow = new System.Windows.Forms.CheckBox();
            this.ckb_leftTopCol = new System.Windows.Forms.CheckBox();
            this.tbx_leftTopCol = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ckb_rightDownRow = new System.Windows.Forms.CheckBox();
            this.tbx_rightDownRow = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ckb_rightDownCol = new System.Windows.Forms.CheckBox();
            this.tbx_rightDownCol = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_runDistancePLTool
            // 
            this.btn_runDistancePLTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runDistancePLTool.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runDistancePLTool.Location = new System.Drawing.Point(461, 333);
            this.btn_runDistancePLTool.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_runDistancePLTool.Name = "btn_runDistancePLTool";
            this.btn_runDistancePLTool.Size = new System.Drawing.Size(98, 45);
            this.btn_runDistancePLTool.TabIndex = 15;
            this.btn_runDistancePLTool.Text = "运行";
            this.btn_runDistancePLTool.UseVisualStyleBackColor = true;
            this.btn_runDistancePLTool.Click += new System.EventHandler(this.btn_runShapeMatchTool_Click);
            // 
            // ckb_createROIToolEnable
            // 
            this.ckb_createROIToolEnable.AutoSize = true;
            this.ckb_createROIToolEnable.BackColor = System.Drawing.Color.Transparent;
            this.ckb_createROIToolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_createROIToolEnable.Location = new System.Drawing.Point(522, 3);
            this.ckb_createROIToolEnable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_createROIToolEnable.Name = "ckb_createROIToolEnable";
            this.ckb_createROIToolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_createROIToolEnable.TabIndex = 74;
            this.ckb_createROIToolEnable.Text = "启用";
            this.ckb_createROIToolEnable.UseVisualStyleBackColor = false;
            this.ckb_createROIToolEnable.CheckedChanged += new System.EventHandler(this.ckb_shapeMatchToolNotRun_CheckedChanged);
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
            this.toolStrip1.Location = new System.Drawing.Point(-5, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(596, 25);
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
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 89;
            this.label1.Text = "左上点行：";
            // 
            // tbx_leftTopRow
            // 
            this.tbx_leftTopRow.Location = new System.Drawing.Point(80, 45);
            this.tbx_leftTopRow.Name = "tbx_leftTopRow";
            this.tbx_leftTopRow.Size = new System.Drawing.Size(100, 23);
            this.tbx_leftTopRow.TabIndex = 90;
            this.tbx_leftTopRow.TextChanged += new System.EventHandler(this.tbx_leftTopRow_TextChanged);
            // 
            // ckb_leftTopRow
            // 
            this.ckb_leftTopRow.AutoSize = true;
            this.ckb_leftTopRow.Location = new System.Drawing.Point(193, 48);
            this.ckb_leftTopRow.Name = "ckb_leftTopRow";
            this.ckb_leftTopRow.Size = new System.Drawing.Size(63, 21);
            this.ckb_leftTopRow.TabIndex = 91;
            this.ckb_leftTopRow.Text = "固定值";
            this.ckb_leftTopRow.UseVisualStyleBackColor = true;
            this.ckb_leftTopRow.CheckedChanged += new System.EventHandler(this.ckb_leftTopRow_CheckedChanged);
            // 
            // ckb_leftTopCol
            // 
            this.ckb_leftTopCol.AutoSize = true;
            this.ckb_leftTopCol.Location = new System.Drawing.Point(193, 85);
            this.ckb_leftTopCol.Name = "ckb_leftTopCol";
            this.ckb_leftTopCol.Size = new System.Drawing.Size(63, 21);
            this.ckb_leftTopCol.TabIndex = 94;
            this.ckb_leftTopCol.Text = "固定值";
            this.ckb_leftTopCol.UseVisualStyleBackColor = true;
            this.ckb_leftTopCol.CheckedChanged += new System.EventHandler(this.ckb_leftTopCol_CheckedChanged);
            // 
            // tbx_leftTopCol
            // 
            this.tbx_leftTopCol.Location = new System.Drawing.Point(80, 82);
            this.tbx_leftTopCol.Name = "tbx_leftTopCol";
            this.tbx_leftTopCol.Size = new System.Drawing.Size(100, 23);
            this.tbx_leftTopCol.TabIndex = 93;
            this.tbx_leftTopCol.TextChanged += new System.EventHandler(this.tbx_leftTopCol_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 92;
            this.label2.Text = "左上点列：";
            // 
            // ckb_rightDownRow
            // 
            this.ckb_rightDownRow.AutoSize = true;
            this.ckb_rightDownRow.Location = new System.Drawing.Point(193, 122);
            this.ckb_rightDownRow.Name = "ckb_rightDownRow";
            this.ckb_rightDownRow.Size = new System.Drawing.Size(63, 21);
            this.ckb_rightDownRow.TabIndex = 97;
            this.ckb_rightDownRow.Text = "固定值";
            this.ckb_rightDownRow.UseVisualStyleBackColor = true;
            this.ckb_rightDownRow.CheckedChanged += new System.EventHandler(this.ckb_rightDownRow_CheckedChanged);
            // 
            // tbx_rightDownRow
            // 
            this.tbx_rightDownRow.Location = new System.Drawing.Point(80, 119);
            this.tbx_rightDownRow.Name = "tbx_rightDownRow";
            this.tbx_rightDownRow.Size = new System.Drawing.Size(100, 23);
            this.tbx_rightDownRow.TabIndex = 96;
            this.tbx_rightDownRow.TextChanged += new System.EventHandler(this.tbx_rightDownRow_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 95;
            this.label3.Text = "右下点行：";
            // 
            // ckb_rightDownCol
            // 
            this.ckb_rightDownCol.AutoSize = true;
            this.ckb_rightDownCol.Location = new System.Drawing.Point(193, 159);
            this.ckb_rightDownCol.Name = "ckb_rightDownCol";
            this.ckb_rightDownCol.Size = new System.Drawing.Size(63, 21);
            this.ckb_rightDownCol.TabIndex = 100;
            this.ckb_rightDownCol.Text = "固定值";
            this.ckb_rightDownCol.UseVisualStyleBackColor = true;
            this.ckb_rightDownCol.CheckedChanged += new System.EventHandler(this.ckb_rightDownCol_CheckedChanged);
            // 
            // tbx_rightDownCol
            // 
            this.tbx_rightDownCol.Location = new System.Drawing.Point(80, 156);
            this.tbx_rightDownCol.Name = "tbx_rightDownCol";
            this.tbx_rightDownCol.Size = new System.Drawing.Size(100, 23);
            this.tbx_rightDownCol.TabIndex = 99;
            this.tbx_rightDownCol.TextChanged += new System.EventHandler(this.tbx_rightDownCol_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 98;
            this.label4.Text = "右下点列：";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(420, 103);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 101;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Frm_CreateROITool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(589, 391);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ckb_rightDownCol);
            this.Controls.Add(this.tbx_rightDownCol);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ckb_rightDownRow);
            this.Controls.Add(this.tbx_rightDownRow);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ckb_leftTopCol);
            this.Controls.Add(this.tbx_leftTopCol);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ckb_leftTopRow);
            this.Controls.Add(this.tbx_leftTopRow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ckb_createROIToolEnable);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btn_runDistancePLTool);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(605, 430);
            this.MinimumSize = new System.Drawing.Size(605, 430);
            this.Name = "Frm_CreateROITool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "创建ROI";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox ckb_createROIToolEnable;
        public System.Windows.Forms.Button btn_runDistancePLTool;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_help;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox tbx_leftTopRow;
        public System.Windows.Forms.CheckBox ckb_leftTopRow;
        public System.Windows.Forms.CheckBox ckb_leftTopCol;
        public System.Windows.Forms.TextBox tbx_leftTopCol;
        public System.Windows.Forms.CheckBox ckb_rightDownRow;
        public System.Windows.Forms.TextBox tbx_rightDownRow;
        public System.Windows.Forms.CheckBox ckb_rightDownCol;
        public System.Windows.Forms.TextBox tbx_rightDownCol;
        private System.Windows.Forms.Button button1;
    }
}