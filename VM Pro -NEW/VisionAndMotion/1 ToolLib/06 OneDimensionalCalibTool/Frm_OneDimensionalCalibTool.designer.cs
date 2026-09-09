namespace VMPro
{
    partial class Frm_OneDimensionalCalibTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_OneDimensionalCalibTool));
            this.ckb_toolEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_runTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_runJob = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.dgv_calibrateData = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbx_translate = new System.Windows.Forms.TextBox();
            this.tbx_scale = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_calibrate = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_secondPoint = new System.Windows.Forms.Button();
            this.btn_firstPoint = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_calibrateData)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckb_toolEnable
            // 
            this.ckb_toolEnable.AutoSize = true;
            this.ckb_toolEnable.BackColor = System.Drawing.Color.Transparent;
            this.ckb_toolEnable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckb_toolEnable.Location = new System.Drawing.Point(436, 34);
            this.ckb_toolEnable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckb_toolEnable.Name = "ckb_toolEnable";
            this.ckb_toolEnable.Size = new System.Drawing.Size(51, 21);
            this.ckb_toolEnable.TabIndex = 74;
            this.ckb_toolEnable.Text = "启用";
            this.ckb_toolEnable.UseVisualStyleBackColor = false;
            this.ckb_toolEnable.CheckedChanged += new System.EventHandler(this.ckb_toolEnable_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_runTool,
            this.tsb_runJob,
            this.toolStripSeparator1,
            this.tsb_resetTool});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(4, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.toolStrip1.Size = new System.Drawing.Size(420, 25);
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
            this.tsb_runTool.Click += new System.EventHandler(this.tsb_runTool_Click);
            // 
            // tsb_runJob
            // 
            this.tsb_runJob.AutoSize = false;
            this.tsb_runJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_runJob.Image = ((System.Drawing.Image)(resources.GetObject("tsb_runJob.Image")));
            this.tsb_runJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_runJob.Name = "tsb_runJob";
            this.tsb_runJob.Size = new System.Drawing.Size(25, 22);
            this.tsb_runJob.Text = "toolStripButton1";
            this.tsb_runJob.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsb_runJob.ToolTipText = "运行流程";
            this.tsb_runJob.Click += new System.EventHandler(this.tsb_runJob_Click);
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
            // dgv_calibrateData
            // 
            this.dgv_calibrateData.AllowUserToAddRows = false;
            this.dgv_calibrateData.AllowUserToDeleteRows = false;
            this.dgv_calibrateData.AllowUserToResizeColumns = false;
            this.dgv_calibrateData.AllowUserToResizeRows = false;
            this.dgv_calibrateData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_calibrateData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dgv_calibrateData.Location = new System.Drawing.Point(15, 89);
            this.dgv_calibrateData.Name = "dgv_calibrateData";
            this.dgv_calibrateData.RowHeadersVisible = false;
            this.dgv_calibrateData.RowTemplate.Height = 23;
            this.dgv_calibrateData.Size = new System.Drawing.Size(206, 118);
            this.dgv_calibrateData.TabIndex = 89;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "像素";
            this.Column1.Name = "Column1";
            this.Column1.Width = 101;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "机械";
            this.Column2.Name = "Column2";
            this.Column2.Width = 101;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbx_translate);
            this.groupBox1.Controls.Add(this.tbx_scale);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(15, 213);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 111);
            this.groupBox1.TabIndex = 90;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "标定结果";
            // 
            // tbx_translate
            // 
            this.tbx_translate.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_translate.Location = new System.Drawing.Point(78, 42);
            this.tbx_translate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_translate.Name = "tbx_translate";
            this.tbx_translate.ReadOnly = true;
            this.tbx_translate.Size = new System.Drawing.Size(88, 23);
            this.tbx_translate.TabIndex = 82;
            // 
            // tbx_scale
            // 
            this.tbx_scale.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbx_scale.Location = new System.Drawing.Point(78, 74);
            this.tbx_scale.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbx_scale.Name = "tbx_scale";
            this.tbx_scale.ReadOnly = true;
            this.tbx_scale.Size = new System.Drawing.Size(88, 23);
            this.tbx_scale.TabIndex = 86;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(40, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 81;
            this.label1.Text = "平移：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(40, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 17);
            this.label3.TabIndex = 85;
            this.label3.Text = "缩放：";
            // 
            // btn_calibrate
            // 
            this.btn_calibrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_calibrate.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_calibrate.Location = new System.Drawing.Point(372, 279);
            this.btn_calibrate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_calibrate.Name = "btn_calibrate";
            this.btn_calibrate.Size = new System.Drawing.Size(98, 45);
            this.btn_calibrate.TabIndex = 91;
            this.btn_calibrate.Text = "标定";
            this.btn_calibrate.UseVisualStyleBackColor = true;
            this.btn_calibrate.Click += new System.EventHandler(this.btn_calibrate_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 87;
            this.label2.Text = "标定数据";
            // 
            // btn_secondPoint
            // 
            this.btn_secondPoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_secondPoint.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_secondPoint.Location = new System.Drawing.Point(227, 137);
            this.btn_secondPoint.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_secondPoint.Name = "btn_secondPoint";
            this.btn_secondPoint.Size = new System.Drawing.Size(49, 24);
            this.btn_secondPoint.TabIndex = 98;
            this.btn_secondPoint.Text = "获取";
            this.btn_secondPoint.UseVisualStyleBackColor = true;
            this.btn_secondPoint.Click += new System.EventHandler(this.btn_secondPoint_Click);
            // 
            // btn_firstPoint
            // 
            this.btn_firstPoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_firstPoint.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_firstPoint.Location = new System.Drawing.Point(227, 114);
            this.btn_firstPoint.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_firstPoint.Name = "btn_firstPoint";
            this.btn_firstPoint.Size = new System.Drawing.Size(49, 24);
            this.btn_firstPoint.TabIndex = 97;
            this.btn_firstPoint.Text = "获取";
            this.btn_firstPoint.UseVisualStyleBackColor = true;
            this.btn_firstPoint.Click += new System.EventHandler(this.btn_firstPoint_Click);
            // 
            // Frm_OneDimensionalCalibTool
            // 
            this.AcceptButton = this.btn_calibrate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(496, 337);
            this.Controls.Add(this.btn_secondPoint);
            this.Controls.Add(this.btn_firstPoint);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_calibrate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgv_calibrateData);
            this.Controls.Add(this.ckb_toolEnable);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(605, 430);
            this.Name = "Frm_OneDimensionalCalibTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "一维标定";
            this.Load += new System.EventHandler(this.Frm_OneDimensionalCalibTool_Load);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.ckb_toolEnable, 0);
            this.Controls.SetChildIndex(this.dgv_calibrateData, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.btn_calibrate, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.btn_firstPoint, 0);
            this.Controls.SetChildIndex(this.btn_secondPoint, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_calibrateData)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox ckb_toolEnable;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_runTool;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_runJob;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox tbx_translate;
        internal System.Windows.Forms.TextBox tbx_scale;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_calibrate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        public System.Windows.Forms.DataGridView dgv_calibrateData;
        private System.Windows.Forms.Button btn_secondPoint;
        private System.Windows.Forms.Button btn_firstPoint;
    }
}