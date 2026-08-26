using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;

namespace VMPro
{
    internal partial class Frm_FullScreen : Form
    {
        internal Frm_FullScreen()
        {
            InitializeComponent();
            Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
            this.Width = rect.Width;
            this.Height = rect.Height;
        }
        internal Frm_ImageWindow frm_imageWindow;
        /// <summary>
        /// 图形窗体句柄
        /// </summary>
        internal HTuple windowHandle;
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_FullScreen _instance;
        public static Frm_FullScreen Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_FullScreen();
                return _instance;
            }
        }


        private void Frm_FullScreen_Load(object sender, EventArgs e)
        {
            try
            {
                Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
                this.Width = rect.Width;
                this.Height = rect.Height;
                this.Location = new System.Drawing.Point(0, 0);
                HOperatorSet.OpenWindow(0, 0, rect.Width, rect.Height, this.pic_showImage.Handle, new HTuple("visible"), new HTuple(""), out windowHandle);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_FullScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Frm_Main.fullScreen = false;
                frm_imageWindow.isFullScreenMode = false;
                this.Hide();
            }
        }

    }
}
