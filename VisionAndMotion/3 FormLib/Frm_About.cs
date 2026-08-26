using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_About : Frm_FormBase   
    {
        internal Frm_About()
        {
            InitializeComponent();
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
                    this.Text = "About";
                }
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 配置
        /// </summary>
        public string AssemblyConfiguration
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyConfigurationAttribute)attributes[0]).Configuration;
            }
        }

        private void Frm_Version_Load(object sender, EventArgs e)
        {
            lbl_version.Text = this.AssemblyConfiguration+" 开发版";
        }

    }
}
