namespace SharpEdit
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.picMin = new System.Windows.Forms.Label();
            this.picMax = new System.Windows.Forms.Label();
            this.picClose = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timerCell = new System.Windows.Forms.Timer(this.components);
            this.timerBack = new System.Windows.Forms.Timer(this.components);
            this.fctb = new FastColoredTextBoxNS.FastColoredTextBox();
            this.contextMenuStripEx1 = new SharpEdit.ContextMenuStripEx();
            this.复制ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.剪切ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.粘贴ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.全选ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.清除ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleTextBox1 = new SharpEdit.ConsoleTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStripEx1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(17)))), ((int)(((byte)(18)))));
            this.splitContainer1.Panel1.Controls.Add(this.picMin);
            this.splitContainer1.Panel1.Controls.Add(this.picMax);
            this.splitContainer1.Panel1.Controls.Add(this.picClose);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel1_MouseMove);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(922, 635);
            this.splitContainer1.SplitterDistance = 31;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 1;
            // 
            // picMin
            // 
            this.picMin.AutoSize = true;
            this.picMin.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.picMin.ForeColor = System.Drawing.Color.White;
            this.picMin.Location = new System.Drawing.Point(838, 9);
            this.picMin.Name = "picMin";
            this.picMin.Size = new System.Drawing.Size(23, 19);
            this.picMin.TabIndex = 6;
            this.picMin.Text = "一";
            // 
            // picMax
            // 
            this.picMax.AutoSize = true;
            this.picMax.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.picMax.ForeColor = System.Drawing.Color.White;
            this.picMax.Location = new System.Drawing.Point(866, 7);
            this.picMax.Name = "picMax";
            this.picMax.Size = new System.Drawing.Size(23, 19);
            this.picMax.TabIndex = 5;
            this.picMax.Text = "口";
            // 
            // picClose
            // 
            this.picClose.AutoSize = true;
            this.picClose.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.picClose.ForeColor = System.Drawing.Color.White;
            this.picClose.Location = new System.Drawing.Point(894, 8);
            this.picClose.Name = "picClose";
            this.picClose.Size = new System.Drawing.Size(19, 19);
            this.picClose.TabIndex = 4;
            this.picClose.Text = "X";
            this.picClose.Click += new System.EventHandler(this.picClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(30, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "脚本编辑";
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label1_MouseMove);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-1, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(29, 28);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.fctb);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.DimGray;
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer2.Size = new System.Drawing.Size(922, 603);
            this.splitContainer2.SplitterDistance = 455;
            this.splitContainer2.SplitterWidth = 2;
            this.splitContainer2.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(17)))), ((int)(((byte)(18)))));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.consoleTextBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.9726F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 76.0274F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(922, 146);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 29);
            this.button1.TabIndex = 1;
            this.button1.Text = "编译";
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Black;
            this.imageList1.Images.SetKeyName(0, "s.png");
            this.imageList1.Images.SetKeyName(1, "d.png");
            this.imageList1.Images.SetKeyName(2, "m.png");
            this.imageList1.Images.SetKeyName(3, "k.png");
            this.imageList1.Images.SetKeyName(4, "c.png");
            // 
            // timerCell
            // 
            this.timerCell.Interval = 10;
            this.timerCell.Tick += new System.EventHandler(this.timerCell_Tick);
            // 
            // timerBack
            // 
            this.timerBack.Interval = 10;
            this.timerBack.Tick += new System.EventHandler(this.timerBack_Tick);
            // 
            // fctb
            // 
            this.fctb.AllowDrop = true;
            this.fctb.AutoScrollMinSize = new System.Drawing.Size(29, 16);
            this.fctb.BackBrush = null;
            this.fctb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(17)))), ((int)(((byte)(18)))));
            this.fctb.CaretColor = System.Drawing.Color.White;
            this.fctb.ContextMenuStrip = this.contextMenuStripEx1;
            this.fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctb.DelayedEventsInterval = 1000;
            this.fctb.DelayedTextChangedInterval = 1000;
            this.fctb.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(17)))), ((int)(((byte)(18)))));
            this.fctb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctb.FoldingIndicatorColor = System.Drawing.Color.Chartreuse;
            this.fctb.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fctb.ForeColor = System.Drawing.Color.White;
            this.fctb.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.VisibleRange;
            this.fctb.ImeMode = System.Windows.Forms.ImeMode.On;
            this.fctb.IndentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(17)))), ((int)(((byte)(18)))));
            this.fctb.IsReplaceMode = false;
            this.fctb.Language = FastColoredTextBoxNS.Language.CSharp;
            this.fctb.LineNumberColor = System.Drawing.Color.MintCream;
            this.fctb.Location = new System.Drawing.Point(0, 0);
            this.fctb.Name = "fctb";
            this.fctb.Paddings = new System.Windows.Forms.Padding(0);
            this.fctb.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.fctb.ServiceLinesColor = System.Drawing.Color.DarkGoldenrod;
            this.fctb.Size = new System.Drawing.Size(922, 455);
            this.fctb.TabIndex = 0;
            this.fctb.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fctb_TextChanged);
            this.fctb.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fctb_KeyDown);
            // 
            // contextMenuStripEx1
            // 
            this.contextMenuStripEx1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.复制ToolStripMenuItem1,
            this.剪切ToolStripMenuItem1,
            this.toolStripMenuItem4,
            this.粘贴ToolStripMenuItem1,
            this.toolStripMenuItem5,
            this.全选ToolStripMenuItem1,
            this.toolStripMenuItem6,
            this.清除ToolStripMenuItem1});
            this.contextMenuStripEx1.Name = "contextMenuStripEx1";
            this.contextMenuStripEx1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStripEx1.Size = new System.Drawing.Size(101, 132);
            // 
            // 复制ToolStripMenuItem1
            // 
            this.复制ToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.复制ToolStripMenuItem1.Name = "复制ToolStripMenuItem1";
            this.复制ToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.复制ToolStripMenuItem1.Text = "复制";
            this.复制ToolStripMenuItem1.Click += new System.EventHandler(this.复制ToolStripMenuItem_Click);
            // 
            // 剪切ToolStripMenuItem1
            // 
            this.剪切ToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.剪切ToolStripMenuItem1.Name = "剪切ToolStripMenuItem1";
            this.剪切ToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.剪切ToolStripMenuItem1.Text = "剪切";
            this.剪切ToolStripMenuItem1.Click += new System.EventHandler(this.剪切ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(97, 6);
            // 
            // 粘贴ToolStripMenuItem1
            // 
            this.粘贴ToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.粘贴ToolStripMenuItem1.Name = "粘贴ToolStripMenuItem1";
            this.粘贴ToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.粘贴ToolStripMenuItem1.Text = "粘贴";
            this.粘贴ToolStripMenuItem1.Click += new System.EventHandler(this.粘贴ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(97, 6);
            // 
            // 全选ToolStripMenuItem1
            // 
            this.全选ToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.全选ToolStripMenuItem1.Name = "全选ToolStripMenuItem1";
            this.全选ToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.全选ToolStripMenuItem1.Text = "全选";
            this.全选ToolStripMenuItem1.Click += new System.EventHandler(this.全选ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(97, 6);
            // 
            // 清除ToolStripMenuItem1
            // 
            this.清除ToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.清除ToolStripMenuItem1.Name = "清除ToolStripMenuItem1";
            this.清除ToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.清除ToolStripMenuItem1.Text = "清除";
            this.清除ToolStripMenuItem1.Click += new System.EventHandler(this.清除ToolStripMenuItem_Click);
            // 
            // consoleTextBox1
            // 
            this.consoleTextBox1.AllowDrop = true;
            this.consoleTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consoleTextBox1.AutoScrollMinSize = new System.Drawing.Size(2, 15);
            this.consoleTextBox1.BackBrush = null;
            this.consoleTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(17)))), ((int)(((byte)(18)))));
            this.consoleTextBox1.CaretColor = System.Drawing.Color.White;
            this.consoleTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.consoleTextBox1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.consoleTextBox1.Font = new System.Drawing.Font("Courier New", 10.5F);
            this.consoleTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.consoleTextBox1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.consoleTextBox1.IndentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(17)))), ((int)(((byte)(18)))));
            this.consoleTextBox1.IsReadLineMode = false;
            this.consoleTextBox1.IsReplaceMode = false;
            this.consoleTextBox1.Location = new System.Drawing.Point(3, 38);
            this.consoleTextBox1.Name = "consoleTextBox1";
            this.consoleTextBox1.Paddings = new System.Windows.Forms.Padding(0);
            this.consoleTextBox1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.consoleTextBox1.ServiceLinesColor = System.Drawing.Color.DarkGoldenrod;
            this.consoleTextBox1.ShowLineNumbers = false;
            this.consoleTextBox1.Size = new System.Drawing.Size(916, 105);
            this.consoleTextBox1.TabIndex = 0;
            this.consoleTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.consoleTextBox1_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(922, 635);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.contextMenuStripEx1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerCell;
        private System.Windows.Forms.Timer timerBack;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ConsoleTextBox consoleTextBox1;
        public FastColoredTextBoxNS.FastColoredTextBox fctb;
        private ContextMenuStripEx contextMenuStripEx1;
        private System.Windows.Forms.ToolStripMenuItem 复制ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 剪切ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem 粘贴ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem 全选ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem 清除ToolStripMenuItem1;
        private System.Windows.Forms.Label picMin;
        private System.Windows.Forms.Label picMax;
        private System.Windows.Forms.Label picClose;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    }
}

