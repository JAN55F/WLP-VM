using HalconDotNet;
using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_AcqDevice : Frm_FormBase
    {
        internal Frm_AcqDevice()
        {
            InitializeComponent();
            hWindow_Final1.hWindowControl.MouseUp += Hwindow_MouseUp;
        }

      
        /// <summary>
        /// ROI区域
        /// </summary>
        internal List<ViewWindow.Model.ROI> L_regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_AcqDevice _instance;
        public static Frm_AcqDevice Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_AcqDevice();
                return _instance;
            }
        }


        /// <summary>
        /// 注册haclon窗体的鼠标弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hwindow_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int index;
                List<double> data;
                ViewWindow.Model.ROI roi = hWindow_Final1.viewWindow.smallestActiveROI(out data, out index);
                if (index > -1)
                {
                    string name = roi.GetType().Name;
                    this.L_regions[index] = roi;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal override void btn_baseClose_Click(object sender, EventArgs e)
        {
            try
            {
                base.btn_baseClose_Click(sender, e);

                //退出窗体时自动停止相机实时
                if (displayImageMode)
                    PlayImage(false);

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tsb_SDKInfo_Click(object sender, EventArgs e)
        {
            Frm_SDKInfo.Instance.ShowDialog();
        }
        private void 适应图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.DispImageFit();
        }
        private void 相机实时ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayImage(相机实时ToolStripMenuItem.Checked);
        }
        private void 显示信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.barVisible_strip_CheckedChanged(sender, e);
        }
        private void 全屏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (全屏ToolStripMenuItem.Checked)
                {
                    全屏ToolStripMenuItem.Text = "退出全屏";
                    panel3.Visible = false;
                    hWindow_Final1.Parent = this;
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    全屏ToolStripMenuItem.Text = "全屏（ESC退出全屏）";
                    panel3.Visible = true;
                    hWindow_Final1.Parent = tableLayoutPanel2;
                    this.WindowState = FormWindowState.Normal;
                    hWindow_Final1.m_CtrlHStatusLabelCtrl.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void 图像另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage();
        }
        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();

            if (displayImageMode)
                PlayImage(false);
        }
        private void rdo_fromDevice_CheckedChanged(object sender, EventArgs e)
        {
            if (rdo_fromDevice.Checked)
                SwitchImageSource(ImageSourceMode.FromDevice);
        }
        private void radio_FromLocalFile_CheckedChanged(object sender, EventArgs e)
        {

            if (radio_FromLocalFile.Checked)
                SwitchImageSource(ImageSourceMode.FromFile);
        }
        private void rdo_fromLocalDirectory_CheckedChanged(object sender, EventArgs e)
        {

            if (rdo_fromLocalDirectory.Checked)
                SwitchImageSource(ImageSourceMode.FromDirectory);
        }
        private void pic_fromDevice_Click(object sender, EventArgs e)
        {
            SwitchImageSource(ImageSourceMode.FromDevice);
        }
        private void pic_fromLocalFile_Click(object sender, EventArgs e)
        {
            SwitchImageSource(ImageSourceMode.FromFile);
        }
        private void pic_fromLocalDirectory_Click(object sender, EventArgs e)
        {
            SwitchImageSource(ImageSourceMode.FromDirectory);
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Run(true, true);
                long time = sw.ElapsedMilliseconds;

                //////if (toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                //////{
                //////    lbl_toolTip.ForeColor = Color.Red;
                //////    lbl_runTime.Text = string.Format("耗时：0ms");
                //////    lbl_toolTip.Text = "状态：" + toolRunStatu.ToString();
                //////}
                //////else
                //////{
                //////    lbl_toolTip.ForeColor = Color.Black;
                //////    lbl_runTime.Text = string.Format("耗时：{0}ms", time.ToString());
                //////    if (imageSourceMode == ImageSourceMode.FromDirectory)
                //////        lbl_toolTip.Text = string.Format("状态：当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + acqImageTool.L_images.Count);
                //////    else
                //////        lbl_toolTip.Text = "状态：" + toolRunStatu.ToString();
                //////}
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void ckb_autoSwitch_CheckChanged(bool Checked)
        {
            autoSwitch = Checked;
        }
        private void ckb_absPath_CheckChanged(bool Checked)
        {
            absPath = Checked;
        }
        private void ckb_RGBToGray_CheckChanged(bool Checked)
        {
            RGBToGray = Checked;
        }
      
        private void ckb_rotateImage_CheckChanged(bool Checked)
        {
            try
            {
                rotateImage = Checked;
                nud_rotateAngle.Visible = Checked;
                lbl_deg.Visible = Checked;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_AcqDevice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (全屏ToolStripMenuItem.Checked)
                    {
                        if (全屏ToolStripMenuItem.Checked)
                        {
                            全屏ToolStripMenuItem.Checked = false;
                            全屏ToolStripMenuItem.Text = "全屏（ESC退出全屏）";
                            panel3.Visible = true;
                            hWindow_Final1.Parent = tableLayoutPanel2;
                            this.WindowState = FormWindowState.Normal;
                            hWindow_Final1.m_CtrlHStatusLabelCtrl.BackColor = Color.White;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonDown;
            Application.DoEvents();
        }
        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }
        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.按钮__2_;
            Application.DoEvents();
        }
        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }









        /// <summary>
        /// 相机集合
        /// </summary>
        internal static List<SDK_Base> L_devices = new List<SDK_Base>();
        /// <summary>
        /// 工具参数
        /// </summary>
        internal ToolPar toolPar = new ToolPar();
        /// <summary>
        /// 绝对路径
        /// </summary>
        internal bool absPath = true;
        /// <summary>
        /// 曝光时间
        /// </summary>
        internal double exposure = 20;
        /// <summary>
        /// 图像源
        /// </summary>
        internal ImageSourceMode imageSourceMode = ImageSourceMode.FromDirectory;
        /// <summary>
        /// 是否处于实时采集模式
        /// </summary>
        internal bool displayImageMode = false;
        /// <summary>
        /// 相机对象
        /// </summary>
        internal SDK_Base SDK_Camera;
        /// <summary>
        /// 相机实时线程
        /// </summary>
        internal static Thread th_displayImage;
        /// <summary>
        /// 图像旋转角度
        /// </summary>
        internal int rotateAngle = 0;
        /// <summary>
        /// 读取文件夹图像模式时每次运行是否自动切换图像
        /// </summary>
        internal bool autoSwitch = true;
        /// <summary>
        /// 是否旋转图像
        /// </summary>
        internal bool rotateImage = false;
        /// <summary>
        /// 是否将彩色图像转化成灰度图像
        /// </summary>
        internal bool RGBToGray = true;
        /// <summary>
        /// 是否显示图像的全部区域
        /// </summary>
        internal bool displayAllImageRegion = true;
       
        /// <summary>
        /// 工作模式为读取文件夹图像时，当前图像的名称
        /// </summary>
        internal string currentImageName = string.Empty;
        /// <summary>
        /// 工作模式为读取文件夹图像时，当前图片的索引
        /// </summary>
        internal int currentImageIndex = 0;
        /// <summary>
        /// 文件夹中的图像文件集合
        /// </summary>
        internal List<string> L_images = new List<string>();
        /// <summary>
        /// 单张图像文件路径
        /// </summary>
        internal string imagePath = string.Empty;
        /// <summary>
        /// 图像文件夹路径
        /// </summary>
        internal string imageDirectoryPath = string.Empty;
         

        /// <summary>
        /// 图像源模式切换
        /// </summary>
        internal void SwitchImageSource(ImageSourceMode mode)
        {
            try
            {
                imageSourceMode = mode;
                switch (mode)
                {
                    case ImageSourceMode.FromDevice:
                        Frm_AcqDevice.Instance.rdo_fromDevice.Checked = true;
                        Frm_AcqDevice.Instance.pic_fromDevice.Image = Resources.勾选;
                        Frm_AcqDevice.Instance.pic_fromLocalFile.Image = Resources.去勾选;
                        Frm_AcqDevice.Instance.pic_fromLocalDirectory.Image = Resources.去勾选;
                        Frm_AcqDevice.Instance.rdo_fromDevice.ForeColor = Color.FromArgb(18, 150, 219);
                        Frm_AcqDevice.Instance.radio_FromLocalFile.ForeColor = Color.Black;
                        Frm_AcqDevice.Instance.rdo_fromLocalDirectory.ForeColor = Color.Black;
                        Frm_AcqDevice.Instance.rdo_fromDevice.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Bold);
                        Frm_AcqDevice.Instance.radio_FromLocalFile.Font = new Font(Frm_AcqDevice.Instance.radio_FromLocalFile.Font.Name, Frm_AcqDevice.Instance.radio_FromLocalFile.Font.Size, FontStyle.Regular);
                        Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font = new Font(Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font.Name, Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font.Size, FontStyle.Regular);

                        Frm_AcqDevice.Instance.pnl_formPanel.Controls.Clear();
                        Frm_FromDevice1.Instance.TopLevel = false;
                        Frm_FromDevice1.Instance.Parent = Frm_AcqDevice.Instance.pnl_formPanel;
                        Frm_FromDevice1.Instance.Dock = DockStyle.Top;
                        Frm_FromDevice1.Instance.Show();

                        Frm_AcqDevice.Instance.ckb_autoSwitch.Visible = false;
                        Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Enabled = true;
                        Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Visible = true;
                        break;
                    case ImageSourceMode.FromFile:
                        Frm_AcqDevice.Instance.radio_FromLocalFile.Checked = true;
                        Frm_AcqDevice.Instance.pic_fromDevice.Image = Resources.去勾选;
                        Frm_AcqDevice.Instance.pic_fromLocalFile.Image = Resources.勾选;
                        Frm_AcqDevice.Instance.pic_fromLocalDirectory.Image = Resources.去勾选;
                        Frm_AcqDevice.Instance.rdo_fromDevice.ForeColor = Color.Black;
                        Frm_AcqDevice.Instance.radio_FromLocalFile.ForeColor = Color.FromArgb(18, 150, 219);
                        Frm_AcqDevice.Instance.rdo_fromLocalDirectory.ForeColor = Color.Black;
                        Frm_AcqDevice.Instance.rdo_fromDevice.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Regular);
                        Frm_AcqDevice.Instance.radio_FromLocalFile.Font = new Font(Frm_AcqDevice.Instance.radio_FromLocalFile.Font.Name, Frm_AcqDevice.Instance.radio_FromLocalFile.Font.Size, FontStyle.Bold);
                        Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font = new Font(Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font.Name, Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font.Size, FontStyle.Regular);

                        Frm_AcqDevice.Instance.pnl_formPanel.Controls.Clear();
                        Frm_FromLocal1.Instance.TopLevel = false;
                        Frm_FromLocal1.Instance.Parent = Frm_AcqDevice.Instance.pnl_formPanel;
                        Frm_FromLocal1.Instance.Dock = DockStyle.Top;
                        Frm_FromLocal1.Instance.Show();

                        Frm_AcqDevice.Instance.ckb_autoSwitch.Visible = false;
                        Frm_FromLocal1.Instance.pnl_multImage.Visible = false;
                        Frm_FromLocal1.Instance.btn_browseImage.Visible = false;
                        Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Enabled = false;
                        Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Visible = false;
                        Frm_FromLocal1.Instance.pnl_multImage.Focus();
                        break;
                    case ImageSourceMode.FromDirectory:
                        Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Checked = true;
                        Frm_AcqDevice.Instance.pic_fromDevice.Image = Resources.去勾选;
                        Frm_AcqDevice.Instance.pic_fromLocalFile.Image = Resources.去勾选;
                        Frm_AcqDevice.Instance.pic_fromLocalDirectory.Image = Resources.勾选;
                        Frm_AcqDevice.Instance.rdo_fromDevice.ForeColor = Color.Black;
                        Frm_AcqDevice.Instance.radio_FromLocalFile.ForeColor = Color.Black;
                        Frm_AcqDevice.Instance.rdo_fromLocalDirectory.ForeColor = Color.FromArgb(18, 150, 219);
                        Frm_AcqDevice.Instance.rdo_fromDevice.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Regular);
                        Frm_AcqDevice.Instance.radio_FromLocalFile.Font = new Font(Frm_AcqDevice.Instance.radio_FromLocalFile.Font.Name, Frm_AcqDevice.Instance.radio_FromLocalFile.Font.Size, FontStyle.Regular);
                        Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font = new Font(Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font.Name, Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font.Size, FontStyle.Bold);

                        Frm_AcqDevice.Instance.pnl_formPanel.Controls.Clear();
                        Frm_FromLocal1.Instance.TopLevel = false;
                        Frm_FromLocal1.Instance.Parent = Frm_AcqDevice.Instance.pnl_formPanel;
                        Frm_FromLocal1.Instance.Dock = DockStyle.Top;
                        Frm_FromLocal1.Instance.Show();

                        Frm_AcqDevice.Instance.ckb_autoSwitch.Visible = true;
                        Frm_FromLocal1.Instance.pnl_multImage.Visible = true;
                        Frm_FromLocal1.Instance.btn_browseImage.Visible = true;
                        Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Enabled = false;
                        Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Visible = false;
                        Frm_FromLocal1.Instance.pnl_multImage.Focus();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 切换相机
        /// </summary>
        /// <param name="deviceDescriptionStr">设备信息字符串</param>
        internal void SwitchDevice(string cameraInfoStr)
        {
            try
            {
                if (Frm_FromDevice1.Instance.cbx_deviceList.TextStr == "")
                {
                    this.SDK_Camera = null;
                }
                else
                {
                    for (int i = 0; i < L_devices.Count; i++)
                    {
                        if (L_devices[i].CameraInfoStr == cameraInfoStr)
                        {
                            Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Black;
                            Frm_AcqDevice.Instance.lbl_toolTip.Text = "当前相机：" + cameraInfoStr;
                            SDK_Camera = L_devices[i];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
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
                if (imageSourceMode != ImageSourceMode.FromDevice)            //避免切换闪烁
                    SwitchImageSource(ImageSourceMode.FromDevice);

                Frm_FromDevice1.Instance.cbx_deviceList.TextStr = string.Empty;
                Frm_FromDevice1.Instance.tbx_exposure.Text = "0";
                Frm_FromLocal1.Instance.tbx_imagePath.Text = string.Empty;
                Frm_FromLocal1.Instance.tbx_imageDirectoryPath.Text = string.Empty;

                Frm_AcqDevice.Instance.hWindow_Final1.ClearWindow();
                Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Black;
                Frm_AcqDevice.Instance.lbl_toolTip.Text = "状态：无";
                Frm_AcqDevice.Instance.lbl_runTime.Text = "耗时：0ms";
                Frm_AcqDevice.Instance.ckb_autoSwitch.Checked = true;
                Frm_AcqDevice.Instance.ckb_RGBToGray.Checked = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 选择图像文件路径
        /// </summary>
        internal void SelectImagePath()
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dig_openFileDialog = new System.Windows.Forms.OpenFileDialog();
                dig_openFileDialog.Title = (Project.Instance.configuration.language == Language.English ? "Please select image path" : "请选择图像文件路径");
                if (imagePath == string.Empty)
                {
                    if (absPath)
                        dig_openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    else
                        dig_openFileDialog.InitialDirectory = Application.StartupPath;
                }
                else
                {
                    if (absPath)
                        dig_openFileDialog.InitialDirectory = Path.GetDirectoryName(imagePath);
                    else
                        dig_openFileDialog.InitialDirectory = Application.StartupPath + imagePath;
                }
                dig_openFileDialog.Filter = (Project.Instance.configuration.language == Language.English ? "Image File(*.*)|*.*|Image File(*.png)|*.png|Image File(*.jpg)|*.jpg|Image File(*.tif)|*.tif" : "图像文件(*.*)|*.*|图像文件(*.tif)|*.tif|图像文件(*.png)|*.png|图像文件(*.jpg)|*.jpg|图像文件(*.bmp)|*.bmp");
                if (dig_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!absPath)         //相对路径模式下，只允许选择程序输出目录下的路径
                    {
                        if (!dig_openFileDialog.FileName.StartsWith(Application.StartupPath))
                        {
                            Frm_MessageBox.Instance.MessageBoxShow("\r\n相对路径模式下只能指定程序输出目录下的路径，路径指定失败");
                            return;
                        }
                        else
                        {
                            imagePath = dig_openFileDialog.FileName.Substring(Application.StartupPath.Length, dig_openFileDialog.FileName.Length - Application.StartupPath.Length);
                        }
                    }
                    else
                    {
                        imagePath = dig_openFileDialog.FileName;
                    }
                    Frm_FromLocal1.Instance.tbx_imagePath.Text = imagePath;

                    HObject image;
                    try
                    {
                        HOperatorSet.ReadImage(out image, dig_openFileDialog.FileName);
                        if (RGBToGray)
                        {
                            HTuple channel;
                            HOperatorSet.CountChannels(image, out channel);
                            if (channel == 3)
                                HOperatorSet.Rgb1ToGray(image, out image);
                        }
                    }
                    catch
                    {
                        Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Red;
                        Frm_AcqDevice.Instance.lbl_toolTip.Text = Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）";
                        return;
                    }
                    toolPar.ResultPar.图像 = image;
                    Frm_AcqDevice.Instance.hWindow_Final1.HobjectToHimage(image);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 选择图像文件夹路径
        /// </summary>
        internal void SelectImageDirectoryPath()
        {
            try
            {
                VistaFolderBrowserDialog _sampleVistaFolderBrowserDialog = new VistaFolderBrowserDialog();
                if (imageDirectoryPath == string.Empty)
                {
                    if (absPath)
                        _sampleVistaFolderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    else
                        _sampleVistaFolderBrowserDialog.SelectedPath = Application.StartupPath;
                }
                else
                {
                    if (absPath)
                        _sampleVistaFolderBrowserDialog.SelectedPath = imageDirectoryPath;
                    else
                        _sampleVistaFolderBrowserDialog.SelectedPath = Application.StartupPath + imageDirectoryPath;
                }
                _sampleVistaFolderBrowserDialog.Description = (Project.Instance.configuration.language == Language.English ? "Please select image folder" : "请选择图像文件夹路径");
                if (_sampleVistaFolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!absPath)
                    {
                        if (!_sampleVistaFolderBrowserDialog.SelectedPath.StartsWith(Application.StartupPath))
                        {
                            Frm_MessageBox.Instance.MessageBoxShow("\r\n相对路径模式下只能指定程序输出目录下的路径，路径指定失败");
                            return;
                        }
                        else
                        {
                            imageDirectoryPath = _sampleVistaFolderBrowserDialog.SelectedPath.Substring(Application.StartupPath.Length, _sampleVistaFolderBrowserDialog.SelectedPath.Length - Application.StartupPath.Length);
                        }
                    }
                    else
                    {
                        imageDirectoryPath = _sampleVistaFolderBrowserDialog.SelectedPath;
                    }
                    Frm_FromLocal1.Instance.tbx_imageDirectoryPath.Text = imageDirectoryPath;

                    L_images.Clear();
                    string[] files = Directory.GetFiles(_sampleVistaFolderBrowserDialog.SelectedPath);
                    for (int i = 0; i < files.Length; i++)
                    {
                        FileInfo fileInfo = new FileInfo(files[i]);
                        if (fileInfo.Extension == ".jpg" || fileInfo.Extension == ".bmp" || fileInfo.Extension == ".png" || fileInfo.Extension == ".tif")
                            L_images.Add(files[i]);
                    }
                    if (L_images.Count > 0)
                    {
                        HObject image;
                        try
                        {
                            HOperatorSet.ReadImage(out image, L_images[0]);
                            if (RGBToGray)
                            {
                                HTuple channel;
                                HOperatorSet.CountChannels(image, out channel);
                                if (channel == 3)
                                    HOperatorSet.Rgb1ToGray(image, out image);
                            }
                        }
                        catch
                        {
                            Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Red;
                            Frm_AcqDevice.Instance.lbl_toolTip.Text = (Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）");
                            return;
                        }
                        currentImageIndex = 0;
                        currentImageName = Path.GetFileName(L_images[0]);
                        toolPar.ResultPar.图像 = image;
                        Frm_AcqDevice.Instance.hWindow_Final1.HobjectToHimage(image);
                        Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Black;
                        Frm_AcqDevice.Instance.lbl_toolTip.Text = string.Format("状态：当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + L_images.Count);
                    }
                    else
                    {
                        Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Red;
                        Frm_AcqDevice.Instance.lbl_toolTip.Text = "状态：文件夹中无图像";
                    }
                }
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
                    currentImageIndex = L_images.Count - 1;
                try
                {
                    HOperatorSet.ReadImage(out image, L_images[currentImageIndex]);
                }
                catch
                {
                    Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Red;
                    Frm_AcqDevice.Instance.lbl_toolTip.Text = (Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）");
                    return;
                }
                currentImageName = Path.GetFileName(L_images[currentImageIndex]);
                Frm_FromLocal1.Instance.pnl_multImage.Focus();
                Frm_AcqDevice.Instance.hWindow_Final1.HobjectToHimage(image);
                Frm_AcqDevice.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + L_images.Count);
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
                if (currentImageIndex > L_images.Count - 1)
                    currentImageIndex = 0;
                try
                {
                    HOperatorSet.ReadImage(out image, L_images[currentImageIndex]);
                }
                catch
                {
                    Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Red;
                    Frm_AcqDevice.Instance.lbl_toolTip.Text = (Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）");
                    return;
                }
                currentImageName = Path.GetFileName(L_images[currentImageIndex]);
                Frm_FromLocal1.Instance.pnl_multImage.Focus();
                Frm_AcqDevice.Instance.hWindow_Final1.HobjectToHimage(image);
                Frm_AcqDevice.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + L_images.Count);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 相机实时
        /// </summary>
        internal void PlayImage(bool runTool )
        {
            try
            {
                if (SDK_Camera == null)
                {
                    Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Red;
                    Frm_AcqDevice.Instance.lbl_toolTip.Text = "状态：未指定采集设备";
                    Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Checked = false;
                    if (!runTool)
                        Frm_Output.Instance.OutputMsg(string.Format("流程 [{0}] 相机实时失败，原因：未指定采集设备", jobName), Color.Red);
                    return;
                }

                if (!displayImageMode)
                {
                    displayImageMode = true;
                    Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Checked = true;
                    Frm_FromDevice1.Instance.btn_displayImage.Text = "停止实时";

                    Frm_AcqDevice.Instance.btn_runTool.Enabled = false;
                    Frm_FromDevice1.Instance.btn_saveImage.Enabled = false;
                    Frm_FromDevice1.Instance.cbx_deviceList.Enabled = false;
                    Frm_FromDevice1.Instance.tbx_exposure.Enabled = false;
                    GetImageWindowControl(jobName).实时显示ToolStripMenuItem.Checked = true;

                    th_displayImage = new Thread(() =>
                    {
                        #region 相机实时
                        try
                        {
                            while (displayImageMode)
                            {
                                if (runTool)
                                {
                                    HTuple row1, col1, row2, col2;
                                    HOperatorSet.GetPart(Frm_AcqDevice .Instance .hWindow_Final1 .HWindowHalconID , out row1, out col1, out row2, out col2);
                                    //////DispMessage(Frm_AcqDevice.Instance.hWindow_Final1.HWindowHalconID, "实时中...", 12, row1 + (row2 - row1) / 30, col1 + (col2 - col1) / 30, "blue", "false");
                                }
                                else
                                {
                                    //////Show_Text("实时中...");
                                }
                                toolPar.ResultPar.图像 = SDK_Camera.GrabOneImage();
                                if (rotateImage)
                                {
                                    HObject image;
                                    HOperatorSet.RotateImage(toolPar.ResultPar.图像, out image, rotateAngle, "constant");
                                    toolPar.ResultPar.图像 = image;
                                }

                                //彩色图像转灰度图像
                                if (RGBToGray)
                                {
                                    HTuple channel;
                                    HOperatorSet.CountChannels(toolPar.ResultPar.图像, out channel);
                                    if (channel == 3)
                                    {
                                        HObject image;
                                        HOperatorSet.Rgb1ToGray(toolPar.ResultPar.图像, out image);
                                        toolPar.ResultPar.图像 = image;
                                    }
                                }
                                if (runTool)
                                {
                                 Frm_AcqDevice .Instance .hWindow_Final1   .HobjectToHimage(toolPar.ResultPar.图像);
                                }
                                else
                                    //////ShowImage(toolPar.ResultPar.图像);

                                Thread.Sleep(10);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.SaveError(ex);
                        }
                        #endregion
                    });
                    th_displayImage.IsBackground = true;
                    th_displayImage.Start();
                }
                else
                {
                    displayImageMode = false;
                    Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Checked = false;
                    Frm_FromDevice1.Instance.btn_displayImage.Text = "实时采集";

                    Frm_AcqDevice.Instance.btn_runTool.Enabled = true;
                    Frm_FromDevice1.Instance.btn_saveImage.Enabled = true;
                    Frm_FromDevice1.Instance.cbx_deviceList.Enabled = true;
                    Frm_FromDevice1.Instance.tbx_exposure.Enabled = true;
                    GetImageWindowControl(jobName).实时显示ToolStripMenuItem.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 图像另存
        /// </summary>
        internal void SaveImage()
        {
            try
            {
                System.Windows.Forms.SaveFileDialog dig_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_saveFileDialog.FileName = DateTime.Now.ToString("yyyy_MM_dd");
                dig_saveFileDialog.Title = (Project.Instance.configuration.language == Language.English ? "Please select the image path" : "请选择图像保存路径");
                dig_saveFileDialog.Filter = (Project.Instance.configuration.language == Language.English ? "Image File(*.tif)|*.tif|Image File(*.png)|*.png|Image File(*.jpg)|*.jpg|Image File(*.*)|*.*" : "图像文件(*.tif)|*.tif|图像文件(*.png)|*.png|图像文件(*.jpg)|*.jpg|图像文件(*.*)|*.*");
                dig_saveFileDialog.InitialDirectory = path;
                if (dig_saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        HOperatorSet.WriteImage(toolPar.ResultPar.图像, "tiff", 0, dig_saveFileDialog.FileName);
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
        /// 工具锁
        /// </summary>
        internal object obj = new object();
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否更新图像</param>
        /// <param name="debugTool">调试工具模式</param>
        public  void Run(bool updateImage, bool runTool)
        {
            try
            {
                lock (obj)
                {
                    if (runTool)
                        Frm_AcqDevice.Instance.hWindow_Final1.ClearWindow();

                    HObject image;
                    switch (imageSourceMode)
                    {
                        case ImageSourceMode.FromDevice:
                            #region 从设备采集
                            if (displayImageMode)
                            {
                                Frm_MessageBox.Instance.MessageBoxShow("\r\n相机实时状态下不可采集图像，请停止实时后重试", TipType.Error);
                                return;
                            }
                            if (SDK_Camera == null)
                            {
                                if (Frm_AcqDevice.Instance.Visible)
                                {
                                    Frm_AcqDevice.Instance.hWindow_Final1.ClearWindow();
                                    Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Red;
                                    Frm_AcqDevice.Instance.lbl_toolTip.Text = "状态：未指定采集设备";
                                }
                                return;
                            }
                            if (!SDK_Camera.CheckCamExist())
                            {
                                return;
                            }

                            SDK_Camera.SetExposure(exposure);
                            toolPar.ResultPar.图像 = SDK_Camera.GrabOneImage();
                            if (toolPar.ResultPar.图像 == null)
                            {
                             
                                return;
                            }
                            break;
                            #endregion
                        case ImageSourceMode.FromFile:
                            #region 从文件读取
                            if (imagePath == string.Empty)
                            {
                                if (Frm_AcqDevice.Instance.Visible)
                                {
                                    Frm_AcqDevice.Instance.hWindow_Final1.ClearWindow();
                                    Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Red;
                                    Frm_AcqDevice.Instance.lbl_toolTip.Text = "状态：未指定图像路径";
                                }
                                return;
                            }
                            if (absPath)
                                HOperatorSet.ReadImage(out image, imagePath);
                            else
                                HOperatorSet.ReadImage(out image, Application.StartupPath + imagePath);
                            toolPar.ResultPar.图像 = image;
                            break;
                            #endregion
                        case ImageSourceMode.FromDirectory:
                            #region 从文件夹读取
                            if (imageDirectoryPath == string.Empty)
                            {
                                if (Frm_AcqDevice.Instance.Visible)
                                {
                                    Frm_AcqDevice.Instance.hWindow_Final1.ClearWindow();
                                    Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Red;
                                    Frm_AcqDevice.Instance.lbl_toolTip.Text = "状态：未指定图像路径";
                                }
                                return;
                            }

                            //更新一下文件夹下面的图像
                            string[] files = new string[] { };
                            try
                            {
                                if (absPath)
                                    files = Directory.GetFiles(imageDirectoryPath);
                                else
                                    files = Directory.GetFiles(Application.StartupPath + imageDirectoryPath);
                            }
                            catch
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The path is invalid(ErrorCode:0106)" : "路径不合法（错误代码：10106）", Color.Red);
                                return;
                            }

                            L_images.Clear();
                            for (int i = 0; i < files.Length; i++)
                            {
                                FileInfo fileInfo = new FileInfo(files[i]);
                                if (fileInfo.Extension == ".jpg" || fileInfo.Extension == ".bmp" || fileInfo.Extension == ".png" || fileInfo.Extension == ".tif")
                                    L_images.Add(files[i]);
                            }

                            if (autoSwitch && !Frm_Main.Instance.tsb_stopSwtich.Checked)
                                currentImageIndex++;

                            if (currentImageIndex > L_images.Count - 1)
                                currentImageIndex = 0;

                            if (L_images.Count == 0)
                            {
                                Frm_Main.Instance.OutputMsg("图像路径下无有效图像文件", Color.Black);
                                return;
                            }
                            HOperatorSet.ReadImage(out image, L_images[currentImageIndex]);
                            currentImageName = Path.GetFileName(L_images[currentImageIndex]);
                            toolPar.ResultPar.图像 = image;
                            break;
                            #endregion
                    }


                    //彩色图像转灰度图像
                    if (RGBToGray)
                    {
                        HTuple channel;
                        HOperatorSet.CountChannels(toolPar.ResultPar.图像, out channel);
                        if (channel == 3)
                        {
                            HObject image1;
                            HOperatorSet.Rgb1ToGray(toolPar.ResultPar.图像, out image1);
                            toolPar.ResultPar.图像 = image1;
                        }
                    }

                    //旋转图像
                    if (rotateImage)
                    {
                        HObject image2;
                        HOperatorSet.RotateImage(toolPar.ResultPar.图像, out image2, rotateAngle, "constant");
                        toolPar.ResultPar.图像 = image2;
                    }

                    if (runTool)
                    {
                        Frm_AcqDevice.Instance.hWindow_Final1.HobjectToHimage(toolPar.ResultPar.图像);
                        if (!displayAllImageRegion)
                        {
                            Frm_AcqDevice.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                            Frm_AcqDevice.Instance.L_regions = this.L_regions;
                        }
                    }
                    else
                    {
                        //自动运行时显示局部图像
                        
                        //////ShowImage(toolPar.ResultPar.图像);
                        if (Frm_AcqDevice.Instance.Visible)
                            Frm_AcqDevice.Instance.hWindow_Final1.HobjectToHimage(toolPar.ResultPar.图像);

                        if (imageSourceMode != ImageSourceMode.FromDevice)
                        {
                            HTuple row1, col1, row2, col2;
                            if (imageSourceMode == ImageSourceMode.FromFile)
                            {
                            }
                            else
                            {
                                Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Black;
                                Frm_AcqDevice.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + L_images.Count);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        #region 参数
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
        public class InputPar { }
        [Serializable]
        public class RunPar { }
        [Serializable]
        internal class ResultPar
        {
            private HObject _图像;
            public HObject 图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }
        }
        #endregion




      
    }
}
