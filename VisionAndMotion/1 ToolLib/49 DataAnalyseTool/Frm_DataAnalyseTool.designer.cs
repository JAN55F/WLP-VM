using Controls;
namespace VMPro
{
    partial class Frm_DataAnalyseTool
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_DataAnalyseTool));
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btn_runJob = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_runTool = new System.Windows.Forms.Button();
            this.lbl_runTime = new System.Windows.Forms.Label();
            this.lbl_toolTip = new System.Windows.Forms.Label();
            this.pic_onOff = new System.Windows.Forms.PictureBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_resetTool = new System.Windows.Forms.ToolStripButton();
            this.tsb_addInput = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_caption = new System.Windows.Forms.Label();
            this.dgv_outputItem = new System.Windows.Forms.DataGridView();
            this.colLinkDisplay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLink = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colDown = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colInResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOutResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cms_row = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuDeleteRow = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_onOff)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_outputItem)).BeginInit();
            this.cms_row.SuspendLayout();
            this.SuspendLayout();
            //
            // pictureBox1
            //
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            //
            // button100
            //
            this.button100.FlatAppearance.BorderSize = 0;
            this.button100.Location = new System.Drawing.Point(522, 0);
            //
            // panel3
            //
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.tableLayoutPanel1);
            this.panel3.Location = new System.Drawing.Point(2, 26);
            this.panel3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(622, 430);
            this.panel3.TabIndex = 117;
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbl_caption, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(622, 430);
            this.tableLayoutPanel1.TabIndex = 115;
            //
            // toolStrip1
            //
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 17);
            this.toolStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_resetTool,
            this.tsb_addInput});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(2, 0);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(620, 25);
            this.toolStrip1.TabIndex = 92;
            this.toolStrip1.Text = "toolStrip1";
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
            this.tsb_resetTool.ToolTipText = "重置";
            this.tsb_resetTool.Click += new System.EventHandler(this.tsb_resetTool_Click);
            //
            // tsb_addInput
            //
            this.tsb_addInput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsb_addInput.Name = "tsb_addInput";
            this.tsb_addInput.Size = new System.Drawing.Size(75, 22);
            this.tsb_addInput.Text = "增加输入项";
            this.tsb_addInput.Click += new System.EventHandler(this.tsb_addInput_Click);
            //
            // lbl_caption
            //
            this.lbl_caption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_caption.Padding = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.lbl_caption.Name = "lbl_caption";
            this.lbl_caption.TabIndex = 118;
            this.lbl_caption.Text = "分析项（每行链接一个数据源；右键行可删除）";
            //
            // panel6
            //
            this.panel6.Controls.Add(this.btn_runJob);
            this.panel6.Controls.Add(this.btn_close);
            this.panel6.Controls.Add(this.btn_runTool);
            this.panel6.Controls.Add(this.lbl_runTime);
            this.panel6.Controls.Add(this.lbl_toolTip);
            this.panel6.Controls.Add(this.pic_onOff);
            this.panel6.Controls.Add(this.panel5);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 368);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(616, 59);
            this.panel6.TabIndex = 90;
            //
            // btn_runJob
            //
            this.btn_runJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_runJob.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_runJob.BackgroundImage")));
            this.btn_runJob.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_runJob.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_runJob.FlatAppearance.BorderSize = 0;
            this.btn_runJob.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_runJob.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_runJob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runJob.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runJob.ForeColor = System.Drawing.Color.White;
            this.btn_runJob.Location = new System.Drawing.Point(435, 18);
            this.btn_runJob.Name = "btn_runJob";
            this.btn_runJob.Size = new System.Drawing.Size(65, 30);
            this.btn_runJob.TabIndex = 110;
            this.btn_runJob.Text = "运行流程";
            this.btn_runJob.UseVisualStyleBackColor = true;
            this.btn_runJob.Click += new System.EventHandler(this.btn_runJob_Click);
            //
            // btn_close
            //
            this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_close.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_close.BackgroundImage")));
            this.btn_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_close.FlatAppearance.BorderSize = 0;
            this.btn_close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_close.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_close.ForeColor = System.Drawing.Color.White;
            this.btn_close.Location = new System.Drawing.Point(540, 18);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(65, 30);
            this.btn_close.TabIndex = 111;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            //
            // btn_runTool
            //
            this.btn_runTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_runTool.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_runTool.BackgroundImage")));
            this.btn_runTool.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_runTool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_runTool.FlatAppearance.BorderSize = 0;
            this.btn_runTool.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_runTool.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_runTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_runTool.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_runTool.ForeColor = System.Drawing.Color.White;
            this.btn_runTool.Location = new System.Drawing.Point(367, 18);
            this.btn_runTool.Name = "btn_runTool";
            this.btn_runTool.Size = new System.Drawing.Size(65, 30);
            this.btn_runTool.TabIndex = 0;
            this.btn_runTool.TabStop = false;
            this.btn_runTool.Text = "运行";
            this.btn_runTool.UseVisualStyleBackColor = true;
            this.btn_runTool.Click += new System.EventHandler(this.tsb_runTool_Click);
            //
            // lbl_runTime
            //
            this.lbl_runTime.AutoSize = true;
            this.lbl_runTime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_runTime.Location = new System.Drawing.Point(94, 17);
            this.lbl_runTime.Name = "lbl_runTime";
            this.lbl_runTime.Size = new System.Drawing.Size(68, 17);
            this.lbl_runTime.TabIndex = 115;
            this.lbl_runTime.Text = "耗时：0ms";
            //
            // lbl_toolTip
            //
            this.lbl_toolTip.AutoSize = true;
            this.lbl_toolTip.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_toolTip.Location = new System.Drawing.Point(94, 34);
            this.lbl_toolTip.Name = "lbl_toolTip";
            this.lbl_toolTip.Size = new System.Drawing.Size(56, 17);
            this.lbl_toolTip.TabIndex = 114;
            this.lbl_toolTip.Text = "状态：无";
            //
            // pic_onOff
            //
            this.pic_onOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pic_onOff.Image = ((System.Drawing.Image)(resources.GetObject("pic_onOff.Image")));
            this.pic_onOff.Location = new System.Drawing.Point(18, 22);
            this.pic_onOff.Name = "pic_onOff";
            this.pic_onOff.Size = new System.Drawing.Size(50, 25);
            this.pic_onOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_onOff.TabIndex = 105;
            this.pic_onOff.TabStop = false;
            this.pic_onOff.Click += new System.EventHandler(this.pic_onOff_Click);
            //
            // panel5
            //
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel5.Location = new System.Drawing.Point(7, 4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(604, 1);
            this.panel5.TabIndex = 112;
            //
            // panel2
            //
            this.panel2.Controls.Add(this.dgv_outputItem);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(616, 334);
            this.panel2.TabIndex = 93;
            //
            // lbl_caption
            //
            this.lbl_caption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_caption.Padding = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.lbl_caption.Name = "lbl_caption";
            this.lbl_caption.TabIndex = 118;
            this.lbl_caption.Text = "分析项（每行链接一个数据源；右键行可删除）";
            //
            // dgv_outputItem
            //
            this.dgv_outputItem.AllowUserToDeleteRows = false;
            this.dgv_outputItem.AllowUserToAddRows = false;
            this.dgv_outputItem.AllowUserToOrderColumns = false;
            this.dgv_outputItem.AllowUserToResizeRows = false;
            this.dgv_outputItem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_outputItem.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLinkDisplay,
            this.colLink,
            this.colDown,
            this.colUp,
            this.colInResult,
            this.colOutResult});
            this.dgv_outputItem.ContextMenuStrip = this.cms_row;
            this.dgv_outputItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_outputItem.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dgv_outputItem.Location = new System.Drawing.Point(0, 0);
            this.dgv_outputItem.Margin = new System.Windows.Forms.Padding(2);
            this.dgv_outputItem.Name = "dgv_outputItem";
            this.dgv_outputItem.RowHeadersVisible = false;
            this.dgv_outputItem.RowTemplate.Height = 23;
            this.dgv_outputItem.Size = new System.Drawing.Size(616, 334);
            this.dgv_outputItem.TabIndex = 74;
            this.dgv_outputItem.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_outputItem_CellContentClick);
            this.dgv_outputItem.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgv_outputItem_CellMouseDown);
            this.dgv_outputItem.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_outputItem_CellValueChanged);
            this.dgv_outputItem.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgv_outputItem_CurrentCellDirtyStateChanged);
            //
            // colLinkDisplay
            //
            this.colLinkDisplay.HeaderText = "输入项";
            this.colLinkDisplay.Name = "colLinkDisplay";
            this.colLinkDisplay.ReadOnly = true;
            this.colLinkDisplay.Width = 220;
            //
            // colLink
            //
            this.colLink.HeaderText = "链接";
            this.colLink.Name = "colLink";
            this.colLink.Text = "链接…";
            this.colLink.UseColumnTextForButtonValue = true;
            this.colLink.Width = 60;
            //
            // colDown
            //
            this.colDown.HeaderText = "值下限";
            this.colDown.Name = "colDown";
            this.colDown.Width = 90;
            //
            // colUp
            //
            this.colUp.HeaderText = "值上限";
            this.colUp.Name = "colUp";
            this.colUp.Width = 90;
            //
            // colInResult
            //
            this.colInResult.HeaderText = "限内结果";
            this.colInResult.Name = "colInResult";
            this.colInResult.Width = 100;
            //
            // colOutResult
            //
            this.colOutResult.HeaderText = "限外结果";
            this.colOutResult.Name = "colOutResult";
            this.colOutResult.Width = 100;
            //
            // cms_row
            //
            this.cms_row.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDeleteRow});
            this.cms_row.Name = "cms_row";
            this.cms_row.Size = new System.Drawing.Size(125, 26);
            //
            // menuDeleteRow
            //
            this.menuDeleteRow.Name = "menuDeleteRow";
            this.menuDeleteRow.Size = new System.Drawing.Size(124, 22);
            this.menuDeleteRow.Text = "删除此行";
            this.menuDeleteRow.Click += new System.EventHandler(this.menuDeleteRow_Click);
            //
            // Frm_DataAnalyseTool
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(626, 458);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(380, 120);
            this.Name = "Frm_DataAnalyseTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据分析";
            this.Controls.SetChildIndex(this.panel3, 0);
            this.Controls.SetChildIndex(this.button100, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_onOff)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_outputItem)).EndInit();
            this.cms_row.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel6;
        public System.Windows.Forms.Label lbl_toolTip;
        internal System.Windows.Forms.Button btn_runJob;
        internal System.Windows.Forms.Button btn_close;
        internal System.Windows.Forms.Button btn_runTool;
        public System.Windows.Forms.Label lbl_runTime;
        public System.Windows.Forms.PictureBox pic_onOff;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_resetTool;
        private System.Windows.Forms.ToolStripButton tsb_addInput;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Label lbl_caption;
        public System.Windows.Forms.DataGridView dgv_outputItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLinkDisplay;
        private System.Windows.Forms.DataGridViewButtonColumn colLink;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDown;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOutResult;
        private System.Windows.Forms.ContextMenuStrip cms_row;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteRow;
    }
}
