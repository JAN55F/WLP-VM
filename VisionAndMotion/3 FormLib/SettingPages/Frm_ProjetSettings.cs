using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace VMPro
{
    internal partial class Frm_ProjetSettings : DockContent
    {
        internal Frm_ProjetSettings()
        {
            InitializeComponent();
            cbx_cardType.SelectedIndexChanged += cbx_cardType_SelectedIndexChanged;
        }

        void cbx_cardType_SelectedIndexChanged()
        {
            if (cbx_cardType.SelectedIndex == 0)
                ckb_vitualCard.Visible = false;
            else
                ckb_vitualCard.Visible = true;
        }



        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_ProjetSettings _instance;
        public static Frm_ProjetSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ProjetSettings();
                return _instance;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ckb_vitualCard_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_drawTemplateRegionRectangle2_Click(object sender, EventArgs e)
        {
            Project.InportProject();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Project.ExportProject ();
        }


    }
}
