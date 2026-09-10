namespace VMPro
{
    partial class Frm_ToolBox
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ToolBox));
            this.lbl_toolInfo = new System.Windows.Forms.Label();
            this.tvw_tools = new VMPro.ModernToolboxTreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.折叠所有ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.展开所有ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_toolInfo
            // 
            this.lbl_toolInfo.BackColor = System.Drawing.Color.White;
            this.lbl_toolInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbl_toolInfo.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_toolInfo.Location = new System.Drawing.Point(0, 403);
            this.lbl_toolInfo.MaximumSize = new System.Drawing.Size(0, 40);
            this.lbl_toolInfo.Name = "lbl_toolInfo";
            this.lbl_toolInfo.Size = new System.Drawing.Size(175, 40);
            this.lbl_toolInfo.TabIndex = 21;
            this.lbl_toolInfo.Text = "说明：无";
            // 
            // tvw_tools
            // 
            this.tvw_tools.AllowDrop = true;
            this.tvw_tools.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvw_tools.ContextMenuStrip = this.contextMenuStrip1;
            this.tvw_tools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvw_tools.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tvw_tools.FullRowSelect = true;
            this.tvw_tools.Indent = 30;
            this.tvw_tools.ItemHeight = 30;
            this.tvw_tools.LineColor = System.Drawing.Color.Green;
            this.tvw_tools.Location = new System.Drawing.Point(0, 0);
            this.tvw_tools.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.tvw_tools.Name = "tvw_tools";
            this.tvw_tools.ShowLines = false;
            this.tvw_tools.ShowRootLines = false;
            this.tvw_tools.Size = new System.Drawing.Size(175, 403);
            this.tvw_tools.TabIndex = 23;
            this.tvw_tools.Tag = "100";
            this.tvw_tools.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvw_tools_ItemDrag);
            this.tvw_tools.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvw_job_AfterSelect);
            this.tvw_tools.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvw_tools_DragDrop);
            this.tvw_tools.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvw_tools_DragEnter);
            this.tvw_tools.DoubleClick += new System.EventHandler(this.tvw_job_DoubleClick);
            this.tvw_tools.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvw_tools_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.折叠所有ToolStripMenuItem,
            this.展开所有ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(128, 70);
            // 
            // 折叠所有ToolStripMenuItem
            // 
            this.折叠所有ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.折叠所有ToolStripMenuItem.Name = "折叠所有ToolStripMenuItem";
            this.折叠所有ToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.折叠所有ToolStripMenuItem.Text = "折叠所有";
            this.折叠所有ToolStripMenuItem.Click += new System.EventHandler(this.折叠所有ToolStripMenuItem_Click);
            // 
            // 展开所有ToolStripMenuItem
            // 
            this.展开所有ToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.展开所有ToolStripMenuItem.Name = "展开所有ToolStripMenuItem";
            this.展开所有ToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.展开所有ToolStripMenuItem.Text = "展开所有";
            this.展开所有ToolStripMenuItem.Click += new System.EventHandler(this.展开所有ToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            // 
            // Frm_ToolBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(175, 443);
            this.Controls.Add(this.tvw_tools);
            this.Controls.Add(this.lbl_toolInfo);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_ToolBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "工具箱";
            this.DockStateChanged += new System.EventHandler(this.Frm_Tools_DockStateChanged);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Frm_Tools_FormClosed);
            this.Load += new System.EventHandler(this.Frm_Tools_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl_toolInfo;
        private VMPro.ModernToolboxTreeView tvw_tools;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 折叠所有ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 展开所有ToolStripMenuItem;
    }
}
