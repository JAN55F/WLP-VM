namespace VMPro
{
    partial class Frm_Job
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Job));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_runOnce = new System.Windows.Forms.Button();
            this.btn_runLoop = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_createJob = new System.Windows.Forms.ToolStripButton();
            this.tsb_expandJob = new System.Windows.Forms.ToolStripButton();
            this.tsb_foldJob = new System.Windows.Forms.ToolStripButton();
            this.tsb_deleteJob = new System.Windows.Forms.ToolStripButton();
            this.tsb_jobInfo = new System.Windows.Forms.ToolStripButton();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbc_jobs = new System.Windows.Forms.TabControl();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btn_runOnce);
            this.panel1.Controls.Add(this.btn_runLoop);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(0, 502);
            this.panel1.Margin = new System.Windows.Forms.Padding(5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(406, 47);
            this.panel1.TabIndex = 100;
            this.panel1.TabStop = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(401, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 26);
            this.label2.TabIndex = 57;
            this.label2.Text = "label2";
            // 
            // btn_runOnce
            // 
            this.btn_runOnce.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_runOnce.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_runOnce.BackgroundImage")));
            this.btn_runOnce.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_runOnce.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_runOnce.FlatAppearance.BorderSize = 0;
            this.btn_runOnce.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_runOnce.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_runOnce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runOnce.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runOnce.ForeColor = System.Drawing.Color.White;
            this.btn_runOnce.Location = new System.Drawing.Point(289, 3);
            this.btn_runOnce.Margin = new System.Windows.Forms.Padding(5);
            this.btn_runOnce.Name = "btn_runOnce";
            this.btn_runOnce.Size = new System.Drawing.Size(110, 41);
            this.btn_runOnce.TabIndex = 10;
            this.btn_runOnce.Text = "单次运行";
            this.btn_runOnce.UseVisualStyleBackColor = true;
            this.btn_runOnce.Click += new System.EventHandler(this.btn_runOnce_Click);
            this.btn_runOnce.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_runOnce_MouseDown);
            this.btn_runOnce.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_runOnce.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_runOnce.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_runOnce_MouseUp);
            // 
            // btn_runLoop
            // 
            this.btn_runLoop.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_runLoop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_runLoop.BackgroundImage")));
            this.btn_runLoop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_runLoop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_runLoop.FlatAppearance.BorderSize = 0;
            this.btn_runLoop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_runLoop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_runLoop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runLoop.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runLoop.ForeColor = System.Drawing.Color.White;
            this.btn_runLoop.Location = new System.Drawing.Point(176, 3);
            this.btn_runLoop.Margin = new System.Windows.Forms.Padding(5);
            this.btn_runLoop.Name = "btn_runLoop";
            this.btn_runLoop.Size = new System.Drawing.Size(110, 41);
            this.btn_runLoop.TabIndex = 55;
            this.btn_runLoop.TabStop = false;
            this.btn_runLoop.Text = "连续运行";
            this.btn_runLoop.UseVisualStyleBackColor = true;
            this.btn_runLoop.Click += new System.EventHandler(this.btn_jobLoopRun_Click);
            this.btn_runLoop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_runLoop_MouseDown);
            this.btn_runLoop.MouseEnter += new System.EventHandler(this.Btn_MouseEnter);
            this.btn_runLoop.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.btn_runLoop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_runLoop_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 30);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_createJob,
            this.tsb_expandJob,
            this.tsb_foldJob,
            this.tsb_deleteJob,
            this.tsb_jobInfo});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 10);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStrip1.Size = new System.Drawing.Size(406, 37);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_createJob
            // 
            this.tsb_createJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_createJob.Image = ((System.Drawing.Image)(resources.GetObject("tsb_createJob.Image")));
            this.tsb_createJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_createJob.Name = "tsb_createJob";
            this.tsb_createJob.Size = new System.Drawing.Size(24, 34);
            this.tsb_createJob.Text = "新建流程";
            this.tsb_createJob.Click += new System.EventHandler(this.tsb_createJob_Click);
            // 
            // tsb_expandJob
            // 
            this.tsb_expandJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_expandJob.Image = ((System.Drawing.Image)(resources.GetObject("tsb_expandJob.Image")));
            this.tsb_expandJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_expandJob.Name = "tsb_expandJob";
            this.tsb_expandJob.Size = new System.Drawing.Size(24, 34);
            this.tsb_expandJob.Text = "展开流程";
            this.tsb_expandJob.Click += new System.EventHandler(this.tsb_expandJob_Click);
            // 
            // tsb_foldJob
            // 
            this.tsb_foldJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_foldJob.Image = ((System.Drawing.Image)(resources.GetObject("tsb_foldJob.Image")));
            this.tsb_foldJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_foldJob.Name = "tsb_foldJob";
            this.tsb_foldJob.Size = new System.Drawing.Size(24, 34);
            this.tsb_foldJob.Text = "折叠流程";
            this.tsb_foldJob.Click += new System.EventHandler(this.tsb_foldJob_Click);
            // 
            // tsb_deleteJob
            // 
            this.tsb_deleteJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_deleteJob.Image = ((System.Drawing.Image)(resources.GetObject("tsb_deleteJob.Image")));
            this.tsb_deleteJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_deleteJob.Name = "tsb_deleteJob";
            this.tsb_deleteJob.Size = new System.Drawing.Size(24, 34);
            this.tsb_deleteJob.Text = "删除流程";
            this.tsb_deleteJob.Click += new System.EventHandler(this.tsb_deleteJob_Click);
            // 
            // tsb_jobInfo
            // 
            this.tsb_jobInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_jobInfo.Image = ((System.Drawing.Image)(resources.GetObject("tsb_jobInfo.Image")));
            this.tsb_jobInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_jobInfo.Name = "tsb_jobInfo";
            this.tsb_jobInfo.Size = new System.Drawing.Size(24, 34);
            this.tsb_jobInfo.Text = "流程属性";
            this.tsb_jobInfo.Click += new System.EventHandler(this.tsb_jobInfo_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button1.Location = new System.Drawing.Point(330, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 23);
            this.button1.TabIndex = 56;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btn_runOnce_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(406, 2);
            this.label1.TabIndex = 11;
            // 
            // tbc_jobs
            // 
            this.tbc_jobs.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbc_jobs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbc_jobs.ItemSize = new System.Drawing.Size(0, 20);
            this.tbc_jobs.Location = new System.Drawing.Point(0, 2);
            this.tbc_jobs.Margin = new System.Windows.Forms.Padding(0);
            this.tbc_jobs.Name = "tbc_jobs";
            this.tbc_jobs.SelectedIndex = 0;
            this.tbc_jobs.Size = new System.Drawing.Size(406, 500);
            this.tbc_jobs.TabIndex = 0;
            this.tbc_jobs.TabStop = false;
            this.tbc_jobs.SelectedIndexChanged += new System.EventHandler(this.tbc_jobs_SelectedIndexChanged);
            // 
            // Frm_Job
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(406, 549);
            this.Controls.Add(this.tbc_jobs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Frm_Job";
            this.Text = "流程编辑器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_Job_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Frm_Job_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.Button btn_runLoop;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TabControl tbc_jobs;
        private System.Windows.Forms.ToolStripButton tsb_createJob;
        private System.Windows.Forms.ToolStripButton tsb_expandJob;
        private System.Windows.Forms.ToolStripButton tsb_foldJob;
        private System.Windows.Forms.ToolStripButton tsb_jobInfo;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button btn_runOnce;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.ToolStripButton tsb_deleteJob;
    }
}