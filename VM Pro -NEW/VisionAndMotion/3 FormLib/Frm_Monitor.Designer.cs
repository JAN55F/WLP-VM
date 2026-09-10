namespace VMPro
{
    partial class Frm_Monitor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Monitor));
            this.dgv_monitor = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_monitor)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_monitor
            // 
            this.dgv_monitor.AllowUserToDeleteRows = false;
            this.dgv_monitor.AllowUserToResizeRows = false;
            this.dgv_monitor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_monitor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dgv_monitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_monitor.Location = new System.Drawing.Point(0, 0);
            this.dgv_monitor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv_monitor.Name = "dgv_monitor";
            this.dgv_monitor.RowHeadersVisible = false;
            this.dgv_monitor.RowTemplate.Height = 23;
            this.dgv_monitor.Size = new System.Drawing.Size(581, 165);
            this.dgv_monitor.TabIndex = 0;
            this.dgv_monitor.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgv_monitor_RowsAdded);
            // 
            // Column1
            // 
            this.Column1.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Column1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Column1.HeaderText = "项名";
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column1.Width = 300;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "项值";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 255;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Frm_Monitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 165);
            this.Controls.Add(this.dgv_monitor);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_Monitor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "值监控器";
            this.Load += new System.EventHandler(this.Frm_Monitor_Load);
            this.Resize += new System.EventHandler(this.Frm_Monitor_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_monitor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        public System.Windows.Forms.DataGridView dgv_monitor;
    }
}