using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_FromDevice1 : Form
    {
        internal Frm_FromDevice1()
        {
            InitializeComponent();

            btn_displayImage.Clicked += btn_realTime_Clicked;
            btn_saveImage.Clicked += btn_saveImage_Clicked;
        }

        /// <summary>
        /// 锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 当前工具所属的流程
        /// </summary>
        internal string jobName = string.Empty;
        /// <summary>
        /// 当前工具名
        /// </summary>
        internal string toolName = string.Empty;
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static AcqImageTool imageAcqTool = new AcqImageTool();
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_FromDevice1 _instance;
        internal static Frm_FromDevice1 Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_FromDevice1();
                return _instance;
            }
        }
        internal static Frm_FromDevice1 CurrentInstance
        {
            get { return _instance; }
        }


        private void cbx_deviceList_SelectedIndexChanged()
        {
            imageAcqTool.SwitchDevice(cbx_deviceList.TextStr);
        }
        private void tbx_exposure_ValueChanged(double value)
        {
            try
            {
                lock (obj)
                {
                    tkb_exposure.Value = (int)value;
                    imageAcqTool.exposure = value;
                    imageAcqTool.Run(true, true, toolName);
                    tbx_exposure.Focus();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tkb_exposure_Scroll(object sender, EventArgs e)
        {
            tbx_exposure.Value = tkb_exposure.Value;
        }
        void btn_realTime_Clicked()
        {
            if (!Frm_AcqDevice.Instance.Visible)
                return;

            imageAcqTool.PlayImage(true, Frm_AcqDevice.Instance.hWindow_Final1);
        }
        void btn_saveImage_Clicked()
        {
            imageAcqTool.SaveImage();
        }

    }
}
