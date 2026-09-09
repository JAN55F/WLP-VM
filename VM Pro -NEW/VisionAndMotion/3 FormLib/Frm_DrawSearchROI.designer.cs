namespace VMPro
{
    partial class Frm_DrawSearchROI
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
            this.btn_LoadImage = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button10 = new System.Windows.Forms.Button();
            this.CircleButton = new System.Windows.Forms.Button();
            this.DelActROIButton = new System.Windows.Forms.Button();
            this.Rect2Button = new System.Windows.Forms.Button();
            this.Rect1Button = new System.Windows.Forms.Button();
            this.dgv_ROI = new System.Windows.Forms.DataGridView();
            this.button11 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.hWindow_Fit1 = new ChoiceTech.Halcon.Control.HWindow_Final();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ROI)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_LoadImage
            // 
            this.btn_LoadImage.Location = new System.Drawing.Point(46, 6);
            this.btn_LoadImage.Name = "btn_LoadImage";
            this.btn_LoadImage.Size = new System.Drawing.Size(96, 35);
            this.btn_LoadImage.TabIndex = 5;
            this.btn_LoadImage.Text = "打开图片";
            this.btn_LoadImage.UseVisualStyleBackColor = true;
            this.btn_LoadImage.Click += new System.EventHandler(this.btn_LoadImage_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.button10);
            this.groupBox2.Controls.Add(this.CircleButton);
            this.groupBox2.Controls.Add(this.Rect2Button);
            this.groupBox2.Controls.Add(this.Rect1Button);
            this.groupBox2.Location = new System.Drawing.Point(176, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 179);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Create ROI";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(19, 141);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(93, 30);
            this.button10.TabIndex = 11;
            this.button10.Text = "CircleArc";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // CircleButton
            // 
            this.CircleButton.Location = new System.Drawing.Point(19, 101);
            this.CircleButton.Name = "CircleButton";
            this.CircleButton.Size = new System.Drawing.Size(96, 34);
            this.CircleButton.TabIndex = 8;
            this.CircleButton.Text = "Circle";
            this.CircleButton.Click += new System.EventHandler(this.CircleButton_Click);
            // 
            // DelActROIButton
            // 
            this.DelActROIButton.Location = new System.Drawing.Point(192, 191);
            this.DelActROIButton.Name = "DelActROIButton";
            this.DelActROIButton.Size = new System.Drawing.Size(96, 35);
            this.DelActROIButton.TabIndex = 7;
            this.DelActROIButton.Text = "Delete Active ROI";
            this.DelActROIButton.Click += new System.EventHandler(this.DelActROIButton_Click);
            // 
            // Rect2Button
            // 
            this.Rect2Button.Location = new System.Drawing.Point(19, 60);
            this.Rect2Button.Name = "Rect2Button";
            this.Rect2Button.Size = new System.Drawing.Size(96, 35);
            this.Rect2Button.TabIndex = 1;
            this.Rect2Button.Text = "Rectangle2";
            this.Rect2Button.Click += new System.EventHandler(this.Rect2Button_Click);
            // 
            // Rect1Button
            // 
            this.Rect1Button.Location = new System.Drawing.Point(19, 20);
            this.Rect1Button.Name = "Rect1Button";
            this.Rect1Button.Size = new System.Drawing.Size(96, 34);
            this.Rect1Button.TabIndex = 0;
            this.Rect1Button.Text = "Rectangle1";
            this.Rect1Button.Click += new System.EventHandler(this.Rect1Button_Click);
            // 
            // dgv_ROI
            // 
            this.dgv_ROI.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ROI.Location = new System.Drawing.Point(0, 305);
            this.dgv_ROI.Name = "dgv_ROI";
            this.dgv_ROI.RowTemplate.Height = 23;
            this.dgv_ROI.Size = new System.Drawing.Size(288, 206);
            this.dgv_ROI.TabIndex = 1;
            this.dgv_ROI.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_ROI_CellClick);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(252, 254);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(36, 32);
            this.button11.TabIndex = 14;
            this.button11.Text = "button11";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(46, 259);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(67, 32);
            this.button9.TabIndex = 13;
            this.button9.Text = "禁止编辑";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(46, 227);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(67, 32);
            this.button8.TabIndex = 12;
            this.button8.Text = "允许编辑";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(170, 249);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(76, 37);
            this.button7.TabIndex = 11;
            this.button7.Text = "button7";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(46, 131);
            this.button6.Margin = new System.Windows.Forms.Padding(2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(96, 27);
            this.button6.TabIndex = 10;
            this.button6.Text = "3显示截图";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(46, 88);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(96, 27);
            this.button5.TabIndex = 10;
            this.button5.Text = "2显示xld";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(46, 46);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(96, 27);
            this.button4.TabIndex = 9;
            this.button4.Text = "1显示region";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(46, 195);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(67, 32);
            this.button3.TabIndex = 8;
            this.button3.Text = "允许缩放";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(46, 163);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(67, 32);
            this.button2.TabIndex = 8;
            this.button2.Text = "禁止缩放";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // hWindow_Fit1
            // 
            this.hWindow_Fit1.BackColor = System.Drawing.Color.Transparent;
            this.hWindow_Fit1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hWindow_Fit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindow_Fit1.DrawModel = false;
            this.hWindow_Fit1.EditModel = true;
            this.hWindow_Fit1.Image = null;
            this.hWindow_Fit1.Location = new System.Drawing.Point(4, 4);
            this.hWindow_Fit1.Margin = new System.Windows.Forms.Padding(4);
            this.hWindow_Fit1.Name = "hWindow_Fit1";
            this.hWindow_Fit1.Size = new System.Drawing.Size(610, 512);
            this.hWindow_Fit1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.54099F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.45901F));
            this.tableLayoutPanel1.Controls.Add(this.hWindow_Fit1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(915, 520);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgv_ROI);
            this.panel1.Controls.Add(this.button11);
            this.panel1.Controls.Add(this.DelActROIButton);
            this.panel1.Controls.Add(this.btn_LoadImage);
            this.panel1.Controls.Add(this.button9);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.button8);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button7);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(621, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(291, 514);
            this.panel1.TabIndex = 12;
            // 
            // Frm_DrawSearchROI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 520);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximumSize = new System.Drawing.Size(956, 559);
            this.Name = "Frm_DrawSearchROI";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ROI)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_LoadImage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button CircleButton;
        public System.Windows.Forms.Button DelActROIButton;
        public System.Windows.Forms.Button Rect2Button;
        public System.Windows.Forms.Button Rect1Button;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private ChoiceTech.Halcon.Control.HWindow_Final hWindow_Fit1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.DataGridView dgv_ROI;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
    }
}

