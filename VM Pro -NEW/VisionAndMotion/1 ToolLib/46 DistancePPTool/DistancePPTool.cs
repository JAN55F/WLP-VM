using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using Basler.Pylon;
using VMPro.Properties;
using Ookii.Dialogs.WinForms;
using VMPro;

namespace VMPro
{
    [Serializable]
    class DistancePPTool : ToolBase
    {
        ~DistancePPTool()
        {
            realTimeMode = false;
        }
        internal ToolPar toolPar = new ToolPar();
        internal bool AbsPath = true;
        /// <summary>
        /// 曝光时间
        /// </summary>
        internal double exposure = 20;
        /// <summary>
        /// 图像源模式
        /// </summary>
        internal ImageSourceMode imageSourceMode = ImageSourceMode.FromDevice;
        /// <summary>
        /// 是否处于实时采集模式
        /// </summary>
        internal bool realTimeMode = false;
        /// <summary>
        /// 相机对象
        /// </summary>
        internal SDK_HIKVision camera_HIKVision;
        /// <summary>
        /// 设备描述字符串
        /// </summary>
        internal string deviceInfoStr = string.Empty;
        /// <summary>
        /// 实时采集线程
        /// </summary>
        internal static Thread th_acq;
        internal int rotateAngle = 180;
        /// <summary>
        /// 读取文件夹图像模式时每次运行是否自动切换图像
        /// </summary>
        internal bool autoSwitch = true;
        internal bool rotateImage = false;
        /// <summary>
        /// 是否将彩色图像转化成灰度图像
        /// </summary>
        internal bool RGBToGray = true;
        internal bool showAll = true;
        internal List<ViewWindow.Model.ROI> L_regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 工作模式为读取文件夹图像时，当前图像的名称
        /// </summary>
        internal string currentImageName = string.Empty;
        /// <summary>
        /// 工作模式为读取文件夹图像时，当前显示的图片的索引
        /// </summary>
        internal int currentImageIndex = 0;
        /// <summary>
        /// 文件夹中的图像文件集合
        /// </summary>
        internal List<string> L_imageFiles = new List<string>();
        /// <summary>
        /// 单张图像文件路径
        /// </summary>
        internal string imagePath = string.Empty;
        /// <summary>
        /// 图像文件夹路径
        /// </summary>
        internal string imageDirectoryPath = string.Empty;
        /// <summary>
        /// 输出图像
        /// </summary>
        internal HObject outputImage;
        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();



        /// <summary>
        /// 图像源模式切换
        /// </summary>
        internal void ImageSourceModeChanged(ImageSourceMode mode)
        {
            if (mode == ImageSourceMode.FromDevice)
            {
                Frm_AcqImageTool.Instance.ckb_absPath.Visible = false;
                Frm_AcqImageTool.Instance.相机实时ToolStripMenuItem.Visible = true;
                Frm_AcqImageTool.Instance.ckb_autoSwitch.Visible = false;
                Frm_AcqImageTool.Instance.rdo_fromDevice.Checked = true;
                Frm_AcqImageTool.Instance.rdo_fromDevice.ForeColor = Color.FromArgb(18, 150, 219);
                Frm_AcqImageTool.Instance.radio_FromLocalFile.ForeColor = Color.Black;
                Frm_AcqImageTool.Instance.rdo_fromLocalDirectory.ForeColor = Color.Black;
                Frm_AcqImageTool.Instance.rdo_fromDevice.Font = new Font(Frm_AcqImageTool.Instance.rdo_fromDevice.Font.Name, Frm_AcqImageTool.Instance.rdo_fromDevice.Font.Size, FontStyle.Bold);
                Frm_AcqImageTool.Instance.radio_FromLocalFile.Font = new Font(Frm_AcqImageTool.Instance.radio_FromLocalFile.Font.Name, Frm_AcqImageTool.Instance.radio_FromLocalFile.Font.Size, FontStyle.Regular);
                Frm_AcqImageTool.Instance.rdo_fromLocalDirectory.Font = new Font(Frm_AcqImageTool.Instance.rdo_fromLocalDirectory.Font.Name, Frm_AcqImageTool.Instance.rdo_fromLocalDirectory.Font.Size, FontStyle.Regular);
                Frm_AcqImageTool.Instance.相机实时ToolStripMenuItem.Enabled = true;
                Frm_AcqImageTool.Instance.pic_fromDevice.Image = Resources.勾选;
                Frm_AcqImageTool.Instance.pic_fromLocalFile.Image = Resources.去勾选;
                Frm_AcqImageTool.Instance.pic_fromLocalDirectory.Image = Resources.去勾选;
                Frm_AcqImageTool.Instance.pnl_formPanel.Controls.Clear();
                Frm_FromDevice.Instance.TopLevel = false;
                Frm_FromDevice.Instance.Parent = Frm_AcqImageTool.Instance.pnl_formPanel;
                Frm_FromDevice.Instance.Dock = DockStyle.Top;
                Frm_FromDevice.Instance.Show();
                imageSourceMode = ImageSourceMode.FromDevice;
            }
            else
            {
                Frm_AcqImageTool.Instance.ckb_absPath.Visible = true;
                Frm_AcqImageTool.Instance.相机实时ToolStripMenuItem.Visible = false;
                Frm_AcqImageTool.Instance.相机实时ToolStripMenuItem.Enabled = false;
                Frm_AcqImageTool.Instance.pic_fromDevice.Image = Resources.去勾选;
                Frm_AcqImageTool.Instance.pic_fromLocalFile.Image = Resources.勾选;
                Frm_AcqImageTool.Instance.pnl_formPanel.Controls.Clear();
                Frm_FromLocal.Instance.TopLevel = false;
                Frm_FromLocal.Instance.Parent = Frm_AcqImageTool.Instance.pnl_formPanel;
                Frm_FromLocal.Instance.Dock = DockStyle.Top;
                Frm_FromLocal.Instance.Show();
                imageSourceMode = ImageSourceMode.FromFile;
            }
        }
        /// <summary>
        /// 复位工具
        /// </summary>
        internal void ResetTool()
        {
            try
            {
                imagePath = string.Empty;
                imageDirectoryPath = string.Empty;

                Frm_FromDevice.Instance.cbx_deviceList.TextStr = string.Empty;
                Frm_FromDevice.Instance.tbx_exposure.Text = "0";
                Frm_FromLocal.Instance.tbx_imageDirectoryPath.Text = string.Empty;
                Frm_FromLocal.Instance.tbx_imagePath.Text = string.Empty;
                Frm_AcqImageTool.Instance.hWindow_Final1.ClearWindow();

                Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Black;
                Frm_AcqImageTool.Instance.lbl_toolTip.Text = "状态：无";

                Frm_AcqImageTool.Instance.lbl_runTime.Text = "耗时：0ms";

                if (imageSourceMode != ImageSourceMode.FromDevice)            //防止切换闪烁
                    ImageSourceModeChanged(ImageSourceMode.FromDevice);

                Frm_AcqImageTool.Instance.ckb_autoSwitch.Checked = true;

                Frm_AcqImageTool.Instance.ckb_RGBToGray.Checked = true;

                Frm_AcqImageTool.Instance.ckb_displayAllImageRegion.Checked = true;



            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        /// <summary>
        /// 读取文件夹中上一张图像
        /// </summary>
        internal void ReadLastImage()
        {
            try
            {
                HObject image;
                HOperatorSet.GenEmptyObj(out image);
                currentImageIndex = currentImageIndex - 1;
                if (currentImageIndex < 0)
                    currentImageIndex = L_imageFiles.Count - 1;
                try
                {
                    HOperatorSet.ReadImage(out image, L_imageFiles[currentImageIndex]);
                }
                catch
                {
                    Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Red;
                    Frm_AcqImageTool.Instance.lbl_toolTip.Text = Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）";
                    return;
                }
                currentImageName = Path.GetFileName(L_imageFiles[currentImageIndex]);
                Frm_AcqImageTool.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + L_imageFiles.Count);

                Frm_AcqImageTool.Instance.hWindow_Final1.HobjectToHimage(image);
                Frm_FromLocal.Instance.pnl_multImage.Focus();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 读取文件夹中下一张图像
        /// </summary>
        internal void ReadNextImage()
        {
            try
            {
                HObject image;
                HOperatorSet.GenEmptyObj(out image);
                currentImageIndex = currentImageIndex + 1;
                if (currentImageIndex > L_imageFiles.Count - 1)
                {
                    currentImageIndex = 0;
                }
                try
                {
                    HOperatorSet.ReadImage(out image, L_imageFiles[currentImageIndex]);
                }
                catch
                {
                    Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Red;
                    Frm_AcqImageTool.Instance.lbl_toolTip.Text = Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）";
                    return;
                }
                currentImageName = Path.GetFileName(L_imageFiles[currentImageIndex]);
                Frm_AcqImageTool.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + L_imageFiles.Count);
                Frm_AcqImageTool.Instance.hWindow_Final1.HobjectToHimage(image);
                Frm_FromLocal.Instance.pnl_multImage.Focus();

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        /// <summary>
        /// 图像另存为
        /// </summary>
        internal void SaveImage()
        {
            try
            {
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd");
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image path" : "请选择图像保存路径";
                dig_saveImage.Filter = Project.Instance.configuration.language == Language.English ? "Image File(*.tif)|*.tif|Image File(*.png)|*.png|Image File(*.jpg)|*.jpg|Image File(*.*)|*.*" : "图像文件(*.tif)|*.tif|图像文件(*.png)|*.png|图像文件(*.jpg)|*.jpg|图像文件(*.*)|*.*";
                dig_saveImage.InitialDirectory = path;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dig_saveImage.FileName;
                    try
                    {
                        HOperatorSet.WriteImage(outputImage, "tiff", 0, dig_saveImage.FileName);
                    }
                    catch
                    {
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1201)" : "图像文件异常或路径不合法（错误代码：0102）", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage, bool runTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);

                    string ttt = toolPar.InputPar.起点.ToString();
                    if (toolPar.InputPar.起点.ToString() == "System.Collections.Generic.List`1[VMPro.XY]")
                    {
                        HTuple temp;
                        HOperatorSet.DistancePp(((List<XY>)toolPar.InputPar.起点)[0].X, ((List<XY>)toolPar.InputPar.起点)[0].Y, ((List<XY>)toolPar.InputPar.终点)[0].X, ((List<XY>)toolPar.InputPar.终点)[0].Y, out temp);
                        toolPar.ResultPar.距离 = temp.D;
                    }
                    else
                    {

                    }

                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }





        [Serializable]
        public class ToolPar : ToolParBase
        {
            private InputPar _inputPar = new InputPar();

            public InputPar InputPar
            {
                get { return _inputPar; }
                set { _inputPar = value; }
            }
            private RunPar _runPar = new RunPar();

            public RunPar RunPar
            {
                get { return _runPar; }
                set { _runPar = value; }
            }
            private ResultPar _resultPar = new ResultPar();

            public ResultPar ResultPar
            {
                get { return _resultPar; }
                set { _resultPar = value; }
            }
        }
        [Serializable]
        public class InputPar
        {
            private object _起点;

            public object 起点
            {
                get { return _起点; }
                set { _起点 = value; }
            }

            private object _终点;

            public object 终点
            {
                get { return _终点; }
                set { _终点 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {
            private double _距离;

            public double 距离
            {
                get
                {
                    _距离 = Math.Round(_距离, 3);
                    return _距离;
                }
                set { _距离 = value; }
            }
        }



    }

}
