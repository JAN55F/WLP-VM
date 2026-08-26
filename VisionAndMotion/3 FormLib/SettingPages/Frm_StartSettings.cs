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
    internal partial class Frm_StartSetting : Form
    {
        internal Frm_StartSetting()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_StartSetting _instance;
        public static Frm_StartSetting Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_StartSetting();
                return _instance;
            }
        }


        private void ckb_allowResizeFormSize_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_allowResizeFormSize.Checked)
                Frm_Main.Instance.MaximumSize = Frm_Main.Instance.MinimumSize = new Size(0, 0);
            else
                Frm_Main.Instance.MaximumSize = Frm_Main.Instance.MinimumSize = Frm_Main.Instance.Size;
        }

        private void ckb_autoBackup_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
