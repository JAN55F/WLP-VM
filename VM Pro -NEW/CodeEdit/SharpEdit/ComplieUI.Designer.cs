namespace SharpEdit
{
    partial class ComplieUI
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCsc = new System.Windows.Forms.Button();
            this.chkCmd = new System.Windows.Forms.CheckBox();
            this.radDll = new System.Windows.Forms.RadioButton();
            this.radWinexe = new System.Windows.Forms.RadioButton();
            this.radExe = new System.Windows.Forms.RadioButton();
            this.txtArgs = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.contextMenuStripEx1 = new SharpEdit.ContextMenuStripEx();
            this.复制ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.清除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripEx1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCsc
            // 
            this.btnCsc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCsc.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCsc.ForeColor = System.Drawing.Color.White;
            this.btnCsc.Location = new System.Drawing.Point(356, 3);
            this.btnCsc.Name = "btnCsc";
            this.btnCsc.Size = new System.Drawing.Size(44, 23);
            this.btnCsc.TabIndex = 16;
            this.btnCsc.Text = "CSC";
            this.btnCsc.UseVisualStyleBackColor = true;
            this.btnCsc.Click += new System.EventHandler(this.btnCsc_Click);
            // 
            // chkCmd
            // 
            this.chkCmd.AutoSize = true;
            this.chkCmd.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkCmd.ForeColor = System.Drawing.Color.White;
            this.chkCmd.Location = new System.Drawing.Point(304, 6);
            this.chkCmd.Name = "chkCmd";
            this.chkCmd.Size = new System.Drawing.Size(57, 21);
            this.chkCmd.TabIndex = 13;
            this.chkCmd.Text = "CMD";
            this.chkCmd.UseVisualStyleBackColor = true;
            // 
            // radDll
            // 
            this.radDll.AutoSize = true;
            this.radDll.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radDll.ForeColor = System.Drawing.Color.White;
            this.radDll.Location = new System.Drawing.Point(256, 5);
            this.radDll.Name = "radDll";
            this.radDll.Size = new System.Drawing.Size(42, 21);
            this.radDll.TabIndex = 12;
            this.radDll.Text = "dll";
            this.radDll.UseVisualStyleBackColor = true;
            // 
            // radWinexe
            // 
            this.radWinexe.AutoSize = true;
            this.radWinexe.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radWinexe.ForeColor = System.Drawing.Color.White;
            this.radWinexe.Location = new System.Drawing.Point(185, 5);
            this.radWinexe.Name = "radWinexe";
            this.radWinexe.Size = new System.Drawing.Size(69, 21);
            this.radWinexe.TabIndex = 10;
            this.radWinexe.Text = "winexe";
            this.radWinexe.UseVisualStyleBackColor = true;
            // 
            // radExe
            // 
            this.radExe.AutoSize = true;
            this.radExe.BackColor = System.Drawing.Color.Transparent;
            this.radExe.Checked = true;
            this.radExe.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radExe.ForeColor = System.Drawing.Color.White;
            this.radExe.Location = new System.Drawing.Point(137, 5);
            this.radExe.Name = "radExe";
            this.radExe.Size = new System.Drawing.Size(47, 21);
            this.radExe.TabIndex = 11;
            this.radExe.TabStop = true;
            this.radExe.Text = "exe";
            this.radExe.UseVisualStyleBackColor = false;
            // 
            // txtArgs
            // 
            this.txtArgs.BackColor = System.Drawing.Color.White;
            this.txtArgs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtArgs.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtArgs.Location = new System.Drawing.Point(45, 3);
            this.txtArgs.Name = "txtArgs";
            this.txtArgs.Size = new System.Drawing.Size(86, 23);
            this.txtArgs.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "参数：";
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(17)))), ((int)(((byte)(18)))));
            this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOutput.ContextMenuStrip = this.contextMenuStripEx1;
            this.txtOutput.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtOutput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.txtOutput.Location = new System.Drawing.Point(0, 29);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(492, 162);
            this.txtOutput.TabIndex = 7;
            // 
            // contextMenuStripEx1
            // 
            this.contextMenuStripEx1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.复制ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.清除ToolStripMenuItem});
            this.contextMenuStripEx1.Name = "contextMenuStripEx1";
            this.contextMenuStripEx1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStripEx1.Size = new System.Drawing.Size(101, 54);
            // 
            // 复制ToolStripMenuItem
            // 
            this.复制ToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.复制ToolStripMenuItem.Name = "复制ToolStripMenuItem";
            this.复制ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.复制ToolStripMenuItem.Text = "复制";
            this.复制ToolStripMenuItem.Click += new System.EventHandler(this.texstToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(97, 6);
            // 
            // 清除ToolStripMenuItem
            // 
            this.清除ToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.清除ToolStripMenuItem.Name = "清除ToolStripMenuItem";
            this.清除ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.清除ToolStripMenuItem.Text = "清除";
            this.清除ToolStripMenuItem.Click += new System.EventHandler(this.清楚ToolStripMenuItem_Click);
            // 
            // ComplieUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.radDll);
            this.Controls.Add(this.btnCsc);
            this.Controls.Add(this.chkCmd);
            this.Controls.Add(this.radWinexe);
            this.Controls.Add(this.radExe);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.txtArgs);
            this.Controls.Add(this.label2);
            this.Name = "ComplieUI";
            this.Size = new System.Drawing.Size(492, 192);
            this.contextMenuStripEx1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCsc;
        private System.Windows.Forms.CheckBox chkCmd;
        private System.Windows.Forms.RadioButton radDll;
        private System.Windows.Forms.RadioButton radWinexe;
        private System.Windows.Forms.RadioButton radExe;
        private System.Windows.Forms.TextBox txtArgs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOutput;
        private ContextMenuStripEx contextMenuStripEx1;
        private System.Windows.Forms.ToolStripMenuItem 复制ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 清除ToolStripMenuItem;
    }
}
