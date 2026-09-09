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
    internal partial class Frm_RunSettings : Form
    {
        internal Frm_RunSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_RunSettings _instance;
        public static Frm_RunSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_RunSettings();
                return _instance;
            }
        }

    }
}
