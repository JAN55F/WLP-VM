using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_FromLocal1 : Form
    {
        internal Frm_FromLocal1()
        {
            InitializeComponent();

            btn_selectImagePath.Clicked += btn_selectImagePath_Clicked;
            btn_browseImage.Clicked += btn_browseImage_Clicked;
            btn_selectImageDirectoryPath.Clicked += btn_selectImageDirectoryPath_Clicked;


            ToolTip toolTip = new ToolTip();
            toolTip.ShowAlways = true;
            toolTip.SetToolTip(this.btn_lastImage, "上一张图像");
            toolTip.SetToolTip(this.btn_nextImage, "下一张图像");
        }

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
        private static Frm_FromLocal1 _instance;
        public static Frm_FromLocal1 Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_FromLocal1();
                return _instance;
            }
        }


        private void tbx_imageDirectoryPath_TextChanged(object sender, EventArgs e)
        {
            imageAcqTool.imageDirectoryPath = tbx_imageDirectoryPath.Text.Trim();
        }
        private void btn_lastImage_Click(object sender, EventArgs e)
        {
            imageAcqTool.ReadLastImage();
        }
        private void btn_nextImage_Click(object sender, EventArgs e)
        {
            imageAcqTool.ReadNextImage();
        }
        void btn_browseImage_Clicked()
        {
            try
            {
                if (Directory.Exists(imageAcqTool.imageDirectoryPath))
                {
                     Frm_AcqImageTool.Instance.TopMost = false;
                     Frm_AcqImageTool.Instance.RefreshTitleButtonVisuals();
                    Process.Start(imageAcqTool.imageDirectoryPath);
                    this.pnl_multImage.Focus();
                }
                else
                {
                     Frm_AcqImageTool.Instance.lbl_toolTip.Text = "状态：请先指定图像目录路径";
                     Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        void btn_selectImageDirectoryPath_Clicked()
        {
            imageAcqTool.SelectImageDirectoryPath();
        }
        private void tbx_imagePath_TextChanged(object sender, EventArgs e)
        {
            imageAcqTool.imagePath = this.tbx_imagePath.Text.Trim();
        }
        void btn_selectImagePath_Clicked()
        {
            imageAcqTool.SelectImagePath();
        }

        private void btn_lastImage_MouseDown(object sender, MouseEventArgs e)
        {
            btn_lastImage.BackgroundImage = Resources.LastOneDown;
            Application.DoEvents();
        }
        private void btn_lastImage_MouseUp(object sender, MouseEventArgs e)
        {
            btn_lastImage.BackgroundImage = Resources.LastOneUp;
            Application.DoEvents();
        }
        private void btn_lastImage_MouseEnter(object sender, EventArgs e)
        {
            btn_lastImage.BackgroundImage = Resources.LastOneEnter;
            Application.DoEvents();
        }
        private void btn_lastImage_MouseLeave(object sender, EventArgs e)
        {
            btn_lastImage.BackgroundImage = Resources.LastOneUp;
            Application.DoEvents();
        }
        private void btn_nextImage_MouseDown(object sender, MouseEventArgs e)
        {
            btn_nextImage.BackgroundImage = Resources.NextOneDown;
            Application.DoEvents();
        }
        private void btn_nextImage_MouseUp(object sender, MouseEventArgs e)
        {
            btn_nextImage.BackgroundImage = Resources.NextOneUp;
            Application.DoEvents();
        }
        private void btn_nextImage_MouseEnter(object sender, EventArgs e)
        {
            btn_nextImage.BackgroundImage = Resources.NextOneEnter;
            Application.DoEvents();
        }
        private void btn_nextImage_MouseLeave(object sender, EventArgs e)
        {
            btn_nextImage.BackgroundImage = Resources.NextOneUp;
            Application.DoEvents();
        }

    }
}
