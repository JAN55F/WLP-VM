using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_About : Frm_FormBase   
    {
        internal Frm_About()
        {
            InitializeComponent();
            Text = "关于 WLP VM";
            lbl_title.Text = "关于 WLP VM";
            lbl_legalStatement.Text = "WLP VM 是威乐普电子科技有限公司的工业视觉软件，用于视觉流程编排、检测识别、标定定位与设备协同。";
            label3.Text = Configuration.DefaultCompanyName;
            label3.AutoSize = false;
            label3.Location = new Point(20, 150);
            label3.Size = new Size(250, 20);
            lbl_version.Location = new Point(20, 172);
            label5.Text = "Copyright © " + Configuration.DefaultCompanyName;
            pictureBox2.Visible = false;
            System.Windows.Forms.Label productName = new System.Windows.Forms.Label();
            productName.Name = "aboutProductName";
            productName.Location = new Point(270, 42);
            productName.Size = new Size(124, 48);
            productName.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            productName.ForeColor = ModernUiTheme.Accent;
            productName.TextAlign = ContentAlignment.MiddleCenter;
            productName.Text = Configuration.ProductDisplayName;
            panel2.Controls.Add(productName);
            Init_Language();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_About _instance;
        internal static Frm_About Instance
        {
            get
            {
                if (_instance == null||_instance.IsDisposed )
                    _instance = new Frm_About();
                return _instance;
            }
        }


        /// <summary>
        /// 初始化语言
        /// </summary>
        private void Init_Language()
        {
            try
            {
                if (Project .Instance .configuration .language == Language.English)
                {
                    this.Text = "About WLP VM";
                    lbl_title.Text = "About WLP VM";
                }
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        private void Frm_Version_Load(object sender, EventArgs e)
        {
            lbl_version.Text = Project.Instance.configuration.language == Language.English
                ? "Version " + Configuration.ProductVersion
                : "版本 " + Configuration.ProductVersion;
        }

    }
}
