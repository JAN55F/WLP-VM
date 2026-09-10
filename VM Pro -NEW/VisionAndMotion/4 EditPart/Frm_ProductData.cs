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
    public partial class Frm_ProductData : DockContent
    {
        public Frm_ProductData()
        {
            InitializeComponent();
            //////data.UpdataEvent += this.userChart1.UpdataData;
            //////Rs232 = new SeriralCom("螺丝机");
            //////ICom.SerialList.Add("螺丝机", Rs232);
            //////Rs232.PaintSplineEvent += data.TorqueByteToChartData;
            //////serialLib1.InitDevice();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_ProductData _instance;
        public static Frm_ProductData Instance
        {
            get
            {
               if (_instance == null)
                    _instance = new Frm_ProductData();
                return _instance;
            }
        }


        private void Frm_ProductData_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

    }
}
