using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using Basler.Pylon;
using VMPro.Properties;
using Ookii.Dialogs.WinForms;
using VMPro;
using System.Text.RegularExpressions;
using ChoiceTech.Halcon.Control;
using System.Runtime.Serialization;

namespace VMPro
{
    [Serializable]
    class AcqImageTool : ToolBase
    {
        private const int AcquisitionTimeoutMs = 5000;
        private const int HardTriggerTimeoutMs = 5000;

        ~AcqImageTool()
        {
            displayImageMode = false;
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
        /// 硬触发模式
        /// </summary>
        internal bool hardTriggerMode = false;
        /// <summary>
        /// 曝光时间
        /// </summary>
        internal double exposure = 20;
        /// <summary>
        /// 图像源
        /// </summary>
        internal ImageSourceMode imageSourceMode = ImageSourceMode.FromDevice;
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
        internal bool RGBToGray = false;
        /// <summary>
        /// 是否显示图像的全部区域
        /// </summary>
        internal bool displayAllImageRegion = true;
        /// <summary>
        /// ROI
        /// </summary>
        internal List<ViewWindow.Model.ROI> L_regions = new List<ViewWindow.Model.ROI>();
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
        /// 自定义图片保存目录。运行采集后若该目录有效，会保存刚拍到的图像并输出完整文件路径。
        /// </summary>
        internal string customSaveDirectory = string.Empty;
        /// <summary>
        /// 流程运行时使用模板图片，不访问相机。
        /// </summary>
        internal bool useTemplateImageInRun = false;
        /// <summary>
        /// 流程运行使用的模板图片路径。
        /// </summary>
        internal string templateImagePath = string.Empty;
        /// <summary>
        /// 最后一次用于预览的图像路径。用于关闭重启后恢复采集工具预览。
        /// </summary>
        internal string lastPreviewImagePath = string.Empty;
        /// <summary>
        /// 手动添加成功过的相机 IP。刷新设备时会自动按这些 IP 再尝试检索。
        /// </summary>
        internal List<string> knownManualIpAddresses = new List<string>();
        [NonSerialized]
        private HObject templateImageCache;
        [NonSerialized]
        private string templateImageCachePath = string.Empty;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            try
            {
                templateImageCache = null;
                templateImageCachePath = string.Empty;
                if (knownManualIpAddresses == null)
                    knownManualIpAddresses = new List<string>();
                if (L_images == null)
                    L_images = new List<string>();
                RememberManualIpFromCameraInfo(SDK_Camera == null ? string.Empty : SDK_Camera.CameraInfoStr);
                ClearLegacyInternalTemplate();
                // 图像是运行时数据，不应随项目在新会话恢复。
                // 保留用户配置的文件/目录/模板路径，真正运行时再读取。
                lastPreviewImagePath = string.Empty;
                toolPar.ResultPar.图像 = null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private Frm_AcqImageTool TryGetAcqToolForm()
        {
            Frm_AcqImageTool form = Frm_AcqImageTool.CurrentInstance;
            if (form != null && !form.IsDisposed && form.IsHandleCreated)
                return form;

            return null;
        }

        private Frm_FromDevice TryGetFromDeviceForm()
        {
            Frm_AcqImageTool form = TryGetAcqToolForm();
            if (form == null)
                return null;

            Frm_FromDevice childForm = Frm_FromDevice.CurrentInstance;
            if (childForm != null && !childForm.IsDisposed && childForm.IsHandleCreated)
                return childForm;

            return null;
        }

        private Frm_FromLocal TryGetFromLocalForm()
        {
            Frm_AcqImageTool form = TryGetAcqToolForm();
            if (form == null)
                return null;

            Frm_FromLocal childForm = Frm_FromLocal.CurrentInstance;
            if (childForm != null && !childForm.IsDisposed && childForm.IsHandleCreated)
                return childForm;

            return null;
        }

        private void InvokeIfHandleCreated(Control control, MethodInvoker action)
        {
            if (control == null || control.IsDisposed || !control.IsHandleCreated || action == null)
                return;

            if (control.InvokeRequired)
                control.BeginInvoke(action);
            else
                action();
        }

        private Frm_Main TryGetMainForm()
        {
            return Application.OpenForms.OfType<Frm_Main>().FirstOrDefault();
        }

        private void PostMainOutputMessage(string message, Color color)
        {
            try
            {
                Frm_Main mainForm = TryGetMainForm();
                if (mainForm == null || mainForm.IsDisposed || !mainForm.IsHandleCreated)
                    return;

                if (mainForm.InvokeRequired)
                    mainForm.BeginInvoke(new MethodInvoker(delegate { mainForm.OutputMsg(message, color); }));
                else
                    mainForm.OutputMsg(message, color);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void UpdateAcqToolUi(MethodInvoker action)
        {
            InvokeIfHandleCreated(TryGetAcqToolForm(), action);
        }

        private void UpdateFromDeviceUi(MethodInvoker action)
        {
            InvokeIfHandleCreated(TryGetFromDeviceForm(), action);
        }

        private void UpdateFromLocalUi(MethodInvoker action)
        {
            InvokeIfHandleCreated(TryGetFromLocalForm(), action);
        }

        private void UpdateDeviceStatus(string message, Color color)
        {
            UpdateAcqToolUi(delegate
            {
                Frm_AcqImageTool form = TryGetAcqToolForm();
                if (form == null)
                    return;

                form.lbl_toolTip.ForeColor = color;
                form.lbl_toolTip.Text = message;
            });
        }

        private void ClearAcqToolPreview()
        {
            UpdateAcqToolUi(delegate
            {
                Frm_AcqImageTool form = TryGetAcqToolForm();
                if (form == null)
                    return;

                form.hWindow_Final1.ClearWindow();
            });
        }

        private void ApplyImageSourceUi(ImageSourceMode mode)
        {
            Frm_AcqImageTool form = TryGetAcqToolForm();
            if (form == null)
                return;

            switch (mode)
            {
                case ImageSourceMode.FromDevice:
                    form.rdo_fromDevice.Checked = true;
                    form.pic_fromDevice.Image = Resources.勾选;
                    form.pic_fromLocalFile.Image = Resources.去勾选;
                    form.pic_fromLocalDirectory.Image = Resources.去勾选;
                    form.rdo_fromDevice.ForeColor = Color.FromArgb(18, 150, 219);
                    form.radio_FromLocalFile.ForeColor = Color.Black;
                    form.rdo_fromLocalDirectory.ForeColor = Color.Black;
                    form.rdo_fromDevice.Font = new Font(form.rdo_fromDevice.Font.Name, form.rdo_fromDevice.Font.Size, FontStyle.Bold);
                    form.radio_FromLocalFile.Font = new Font(form.radio_FromLocalFile.Font.Name, form.radio_FromLocalFile.Font.Size, FontStyle.Regular);
                    form.rdo_fromLocalDirectory.Font = new Font(form.rdo_fromLocalDirectory.Font.Name, form.rdo_fromLocalDirectory.Font.Size, FontStyle.Regular);

                    form.pnl_formPanel.Controls.Clear();
                    Frm_FromDevice.Instance.TopLevel = false;
                    Frm_FromDevice.Instance.Parent = form.pnl_formPanel;
                    Frm_FromDevice.Instance.Dock = DockStyle.Fill;
                    Frm_FromDevice.Instance.Show();

                    form.cCheckBox1.Visible = true;
                    form.cCheckBox1.Location = new System.Drawing.Point(5, 395);
                    form.ckb_autoSwitch.Visible = false;
                    form.ckb_absPath.Visible = false;
                    form.ckb_RGBToGray.Visible = false;
                    form.ckb_displayAllImageRegion.Visible = false;
                    form.相机实时ToolStripMenuItem.Enabled = true;
                    form.相机实时ToolStripMenuItem.Visible = true;
                    break;
                case ImageSourceMode.FromFile:
                    form.radio_FromLocalFile.Checked = true;
                    form.pic_fromDevice.Image = Resources.去勾选;
                    form.pic_fromLocalFile.Image = Resources.勾选;
                    form.pic_fromLocalDirectory.Image = Resources.去勾选;
                    form.rdo_fromDevice.ForeColor = Color.Black;
                    form.radio_FromLocalFile.ForeColor = Color.FromArgb(18, 150, 219);
                    form.rdo_fromLocalDirectory.ForeColor = Color.Black;
                    form.rdo_fromDevice.Font = new Font(form.rdo_fromDevice.Font.Name, form.rdo_fromDevice.Font.Size, FontStyle.Regular);
                    form.radio_FromLocalFile.Font = new Font(form.radio_FromLocalFile.Font.Name, form.radio_FromLocalFile.Font.Size, FontStyle.Bold);
                    form.rdo_fromLocalDirectory.Font = new Font(form.rdo_fromLocalDirectory.Font.Name, form.rdo_fromLocalDirectory.Font.Size, FontStyle.Regular);

                    form.pnl_formPanel.Controls.Clear();
                    Frm_FromLocal.imageAcqTool = this;
                    Frm_FromLocal.Instance.TopLevel = false;
                    Frm_FromLocal.Instance.Parent = form.pnl_formPanel;
                    Frm_FromLocal.Instance.Dock = DockStyle.Top;
                    Frm_FromLocal.Instance.Show();
                    // 同步当前工具的固定路径，防止面板残留其他工具/上次会话的目录
                    Frm_FromLocal.Instance.tbx_imagePath.Text = imagePath ?? string.Empty;

                    form.cCheckBox1.Visible = false;
                    form.cCheckBox1.Location = new System.Drawing.Point(2, 316);
                    form.ckb_autoSwitch.Visible = false;
                    Frm_FromLocal.Instance.pnl_multImage.Visible = false;
                    Frm_FromLocal.Instance.btn_browseImage.Visible = false;
                    form.相机实时ToolStripMenuItem.Enabled = false;
                    form.相机实时ToolStripMenuItem.Visible = false;
                    Frm_FromLocal.Instance.pnl_multImage.Focus();
                    break;
                case ImageSourceMode.FromDirectory:
                    form.rdo_fromLocalDirectory.Checked = true;
                    form.pic_fromDevice.Image = Resources.去勾选;
                    form.pic_fromLocalFile.Image = Resources.去勾选;
                    form.pic_fromLocalDirectory.Image = Resources.勾选;
                    form.rdo_fromDevice.ForeColor = Color.Black;
                    form.radio_FromLocalFile.ForeColor = Color.Black;
                    form.rdo_fromLocalDirectory.ForeColor = Color.FromArgb(18, 150, 219);
                    form.rdo_fromDevice.Font = new Font(form.rdo_fromDevice.Font.Name, form.rdo_fromDevice.Font.Size, FontStyle.Regular);
                    form.radio_FromLocalFile.Font = new Font(form.radio_FromLocalFile.Font.Name, form.radio_FromLocalFile.Font.Size, FontStyle.Regular);
                    form.rdo_fromLocalDirectory.Font = new Font(form.rdo_fromLocalDirectory.Font.Name, form.rdo_fromLocalDirectory.Font.Size, FontStyle.Bold);

                    form.pnl_formPanel.Controls.Clear();
                    Frm_FromLocal.imageAcqTool = this;
                    Frm_FromLocal.Instance.TopLevel = false;
                    Frm_FromLocal.Instance.Parent = form.pnl_formPanel;
                    Frm_FromLocal.Instance.Dock = DockStyle.Top;
                    Frm_FromLocal.Instance.Show();
                    // 同步当前工具的固定目录，防止面板残留其他工具/上次会话的目录
                    Frm_FromLocal.Instance.tbx_imageDirectoryPath.Text = imageDirectoryPath ?? string.Empty;

                    form.cCheckBox1.Visible = false;
                    form.cCheckBox1.Location = new System.Drawing.Point(2, 316);
                    form.ckb_autoSwitch.Visible = true;
                    form.ckb_absPath.Visible = true;
                    Frm_FromLocal.Instance.pnl_multImage.Visible = true;
                    Frm_FromLocal.Instance.btn_browseImage.Visible = true;
                    form.相机实时ToolStripMenuItem.Enabled = false;
                    form.相机实时ToolStripMenuItem.Visible = false;
                    Frm_FromLocal.Instance.pnl_multImage.Focus();
                    break;
            }
        }


        /// <summary>
        /// 图像源模式切换：相机输出(FromDevice) / 固定路径单张图像(FromFile) / 固定路径文件夹(FromDirectory)
        /// </summary>
        internal void SwitchImageSource(ImageSourceMode mode)
        {
            try
            {
                imageSourceMode = mode;
                UpdateAcqToolUi(delegate { ApplyImageSourceUi(mode); });
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
                Frm_FromDevice fromDeviceForm = TryGetFromDeviceForm();
                if (fromDeviceForm != null && fromDeviceForm.cbx_deviceList.TextStr == "")
                {
                    this.SDK_Camera = null;
                }
                else
                {
                    for (int i = 0; i < L_devices.Count; i++)
                    {
                        if (L_devices[i].CameraInfoStr == cameraInfoStr)
                        {
                            UpdateDeviceStatus("当前相机：" + cameraInfoStr, Color.Black);
                            SDK_Camera = L_devices[i];
                            ApplyExposureBrightness();
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
        /// 立即应用曝光亮度，不额外触发采图。
        /// </summary>
        internal void ApplyExposureBrightness()
        {
            try
            {
                if (SDK_Camera != null && SDK_Camera.CheckCamExist())
                    SDK_Camera.SetExposure(exposure);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 重新枚举当前可用相机。用于程序启动后再接入相机时刷新设备列表。
        /// </summary>
        internal bool AddManualIpCamera(string ipAddress)
        {
            return AddManualIpCamera(ipAddress, true, true);
        }

        private bool AddManualIpCamera(string ipAddress, bool selectCamera, bool showResultMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                    return false;

                ipAddress = ipAddress.Trim();

                SDK_Base manualCamera;
                string cameraInfoStr;
                bool foundByHik = SDK_HIKVision.TryCreateManualIpCamera(ipAddress, out manualCamera, out cameraInfoStr);
                if (!foundByHik)
                {
                    cameraInfoStr = SDK_Halcon.BuildManualIpCameraInfo(ipAddress);
                    manualCamera = new SDK_Halcon(cameraInfoStr);
                }

                if (!manualCamera.TryOpenCamera())
                {
                    if (showResultMessage)
                    {
                        string message = "状态：已添加相机，但打开失败 " + cameraInfoStr;
                        UpdateDeviceStatus(message, Color.Red);
                        PostMainOutputMessage("手动添加相机后打开失败，请先关闭 MVS 中已打开的相机连接，再检查相机是否被占用、IP是否同网段、MVS/网卡驱动访问权限：" + cameraInfoStr, Color.Red);
                    }
                    return false;
                }

                RememberManualIp(ipAddress);
                bool cameraAlreadyExists = AcqImageTool.L_devices.Any(device => device.CameraInfoStr == cameraInfoStr);
                if (!cameraAlreadyExists)
                    AcqImageTool.L_devices.Add(manualCamera);

                UpdateFromDeviceUi(delegate
                {
                    Frm_FromDevice currentForm = TryGetFromDeviceForm();
                    if (currentForm != null)
                    {
                        if (!cameraAlreadyExists)
                            currentForm.cbx_deviceList.Add(cameraInfoStr);
                        if (selectCamera)
                            currentForm.cbx_deviceList.TextStr = cameraInfoStr;
                    }
                });

                Frm_FromDevice1 currentForm1 = Frm_FromDevice1.CurrentInstance;
                if (currentForm1 != null && !currentForm1.IsDisposed && currentForm1.IsHandleCreated)
                {
                    InvokeIfHandleCreated(currentForm1, delegate
                    {
                        if (!cameraAlreadyExists)
                            currentForm1.cbx_deviceList.Add(cameraInfoStr);
                        if (selectCamera)
                            currentForm1.cbx_deviceList.TextStr = cameraInfoStr;
                    });
                }

                if (selectCamera)
                {
                    SDK_Camera = manualCamera;
                    SwitchDevice(cameraInfoStr);
                }

                ApplyExposureBrightness();
                if (showResultMessage)
                    UpdateDeviceStatus("状态：已手动添加并打开相机 " + cameraInfoStr, Color.Black);
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }

        private void RememberManualIp(string ipAddress)
        {
            if (knownManualIpAddresses == null)
                knownManualIpAddresses = new List<string>();

            if (string.IsNullOrWhiteSpace(ipAddress))
                return;

            ipAddress = ipAddress.Trim();
            if (!Regex.IsMatch(ipAddress, @"^\d{1,3}(\.\d{1,3}){3}$"))
                return;

            if (!knownManualIpAddresses.Any(item => string.Equals(item, ipAddress, StringComparison.OrdinalIgnoreCase)))
                knownManualIpAddresses.Add(ipAddress);
        }

        private void RememberManualIpFromCameraInfo(string cameraInfoStr)
        {
            if (string.IsNullOrWhiteSpace(cameraInfoStr))
                return;

            Match match = Regex.Match(cameraInfoStr, @"(?<!\d)(\d{1,3}(?:\.\d{1,3}){3})(?!\d)");
            if (match.Success)
                RememberManualIp(match.Groups[1].Value);
        }

        private int AddKnownManualIpCameras()
        {
            if (knownManualIpAddresses == null || knownManualIpAddresses.Count == 0)
                return 0;

            int count = 0;
            foreach (string ipAddress in knownManualIpAddresses.ToArray())
            {
                if (AddManualIpCamera(ipAddress, false, false))
                    count++;
            }
            return count;
        }
        internal void RefreshDeviceList()
        {
            try
            {
                if (displayImageMode)
                    PlayImage(false, null);

                SDK_Camera = null;
                UpdateFromDeviceUi(delegate
                {
                    Frm_FromDevice currentForm = TryGetFromDeviceForm();
                    if (currentForm != null)
                        currentForm.cbx_deviceList.Clear();
                });
                Frm_FromDevice1 currentForm1 = Frm_FromDevice1.CurrentInstance;
                if (currentForm1 != null && !currentForm1.IsDisposed && currentForm1.IsHandleCreated)
                {
                    InvokeIfHandleCreated(currentForm1, delegate { currentForm1.cbx_deviceList.Clear(); });
                }
                AcqImageTool.L_devices.Clear();

                SDK_Halcon.CloseAllCamera();
                SDK_PointGrey.CloseAllCamera();
                try { SDK_Basler.CloseAllCamera(); }
                catch { }
                try { SDK_HIKVision.CloseAllCamera(); }
                catch { }
                try { SDK_MindVision.CloseAllCamera(); }
                catch { }

                SDK_Halcon.EnumCamera();
                try { Frm_SDKInfo.LoadState[1] = SDK_HIKVision.EnumCamera(); }
                catch { }
                try { Frm_SDKInfo.LoadState[5] = SDK_PointGrey.EnumCamera(); }
                catch { }
                int manualIpCount = AddKnownManualIpCameras();

                UpdateFromDeviceUi(delegate
                {
                    Frm_FromDevice currentForm = TryGetFromDeviceForm();
                    if (currentForm != null)
                        currentForm.cbx_deviceList.TextStr = string.Empty;
                });
                UpdateDeviceStatus(string.Format("状态：已重新检索设备，发现 {0} 个相机", AcqImageTool.L_devices.Count), Color.Black);
                if (manualIpCount > 0)
                    PostMainOutputMessage(string.Format("已通过保存的 IP 自动检索到 {0} 个相机", manualIpCount), Color.Black);
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
                customSaveDirectory = string.Empty;
                toolPar.ResultPar.自定义路径 = string.Empty;
                useTemplateImageInRun = false;
                templateImagePath = string.Empty;
                lastPreviewImagePath = string.Empty;
                templateImageCache = null;
                templateImageCachePath = string.Empty;
                if (imageSourceMode != ImageSourceMode.FromDevice)            //避免切换闪烁
                    SwitchImageSource(ImageSourceMode.FromDevice);

                RefreshDeviceList();
                UpdateFromDeviceUi(delegate
                {
                    Frm_FromDevice currentForm = TryGetFromDeviceForm();
                    if (currentForm == null)
                        return;

                    currentForm.LoadExposureBrightness(exposure);
                    currentForm.tbx_saveDirectory.Text = string.Empty;
                    currentForm.ckb_useTemplateImage.Checked = false;
                    currentForm.tbx_templateImagePath.Text = string.Empty;
                });
                UpdateFromLocalUi(delegate
                {
                    Frm_FromLocal currentForm = TryGetFromLocalForm();
                    if (currentForm == null)
                        return;

                    currentForm.tbx_imagePath.Text = string.Empty;
                    currentForm.tbx_imageDirectoryPath.Text = string.Empty;
                });

                UpdateAcqToolUi(delegate
                {
                    Frm_AcqImageTool form = TryGetAcqToolForm();
                    if (form == null)
                        return;

                    form.hWindow_Final1.ClearWindow();
                    form.lbl_toolTip.ForeColor = Color.Black;
                    form.lbl_toolTip.Text = "状态：无";
                    form.lbl_runTime.Text = "耗时：0ms";
                    form.ckb_autoSwitch.Checked = true;
                    form.ckb_RGBToGray.Checked = false;
                    form.ckb_displayAllImageRegion.Checked = true;
                });
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
                    Frm_FromLocal.Instance.tbx_imagePath.Text = imagePath;

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
                        Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Red;
                        Frm_AcqImageTool.Instance.lbl_toolTip.Text = Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）";
                        return;
                    }
                    toolPar.ResultPar.图像 = image;
                    lastPreviewImagePath = dig_openFileDialog.FileName;
                    Frm_AcqImageTool.Instance.hWindow_Final1.HobjectToHimage(image);
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
                    Frm_FromLocal.Instance.tbx_imageDirectoryPath.Text = imageDirectoryPath;

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
                            Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Red;
                            Frm_AcqImageTool.Instance.lbl_toolTip.Text = (Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）");
                            return;
                        }
                        currentImageIndex = 0;
                        currentImageName = Path.GetFileName(L_images[0]);
                        toolPar.ResultPar.图像 = image;
                        lastPreviewImagePath = L_images[0];
                        Frm_AcqImageTool.Instance.hWindow_Final1.HobjectToHimage(image);
                        Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Black;
                        Frm_AcqImageTool.Instance.lbl_toolTip.Text = string.Format("状态：当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + L_images.Count);
                    }
                    else
                    {
                        Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Red;
                        Frm_AcqImageTool.Instance.lbl_toolTip.Text = "状态：文件夹中无图像";
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
                    Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Red;
                    Frm_AcqImageTool.Instance.lbl_toolTip.Text = (Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）");
                    return;
                }
                currentImageName = Path.GetFileName(L_images[currentImageIndex]);
                lastPreviewImagePath = L_images[currentImageIndex];
                Frm_FromLocal.Instance.pnl_multImage.Focus();
                Frm_AcqImageTool.Instance.hWindow_Final1.HobjectToHimage(image);
                Frm_AcqImageTool.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + L_images.Count);
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
                    Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Red;
                    Frm_AcqImageTool.Instance.lbl_toolTip.Text = (Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1102)" : "图像文件异常或路径不合法（错误代码：0102）");
                    return;
                }
                currentImageName = Path.GetFileName(L_images[currentImageIndex]);
                lastPreviewImagePath = L_images[currentImageIndex];
                Frm_FromLocal.Instance.pnl_multImage.Focus();
                Frm_AcqImageTool.Instance.hWindow_Final1.HobjectToHimage(image);
                Frm_AcqImageTool.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", currentImageName, currentImageIndex + 1 + "/" + L_images.Count);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 相机实时
        /// </summary>
        internal void PlayImage(bool runTool, HWindow_Final window)
        {
            try
            {
                if (SDK_Camera == null)
                {
                    UpdateAcqToolUi(delegate
                    {
                        Frm_AcqImageTool form = TryGetAcqToolForm();
                        if (form == null)
                            return;

                        form.lbl_toolTip.ForeColor = Color.Red;
                        form.lbl_toolTip.Text = "状态：未指定采集设备";
                        form.相机实时ToolStripMenuItem.Checked = false;
                    });
                    if (!runTool)
                        Frm_Output.Instance.OutputMsg(string.Format("流程 [{0}] 相机实时失败，原因：未指定采集设备", jobName), Color.Red);
                    return;
                }

                if (!SDK_Camera.TryOpenCamera())
                {
                    UpdateAcqToolUi(delegate
                    {
                        Frm_AcqImageTool form = TryGetAcqToolForm();
                        if (form == null)
                            return;

                        form.lbl_toolTip.ForeColor = Color.Red;
                        form.lbl_toolTip.Text = "状态：相机打开失败 " + SDK_Camera.CameraInfoStr;
                        form.相机实时ToolStripMenuItem.Checked = false;
                    });
                    PostMainOutputMessage("相机实时失败：相机未能打开，请检查相机是否被占用、IP是否同网段、MVS/网卡驱动访问权限：" + SDK_Camera.CameraInfoStr, Color.Red);
                    return;
                }

                if (!displayImageMode)
                {
                    RGBToGray = false;
                    displayAllImageRegion = true;
                    displayImageMode = true;
                    UpdateAcqToolUi(delegate
                    {
                        Frm_AcqImageTool form = TryGetAcqToolForm();
                        if (form == null)
                            return;

                        form.相机实时ToolStripMenuItem.Checked = true;
                        form.btn_runTool.Enabled = false;
                    });
                    UpdateFromDeviceUi(delegate
                    {
                        Frm_FromDevice form = TryGetFromDeviceForm();
                        if (form == null)
                            return;

                        form.btn_displayImage.Text = "停止实时";
                        form.btn_saveImage.Enabled = false;
                        form.cbx_deviceList.Enabled = false;
                        form.tbx_exposure.Enabled = true;
                        form.tkb_exposure.Enabled = true;
                    });
                    ApplyExposureBrightness();
                    Frm_ImageWindow imageWindow = GetImageWindowControl(jobName);
                    if (imageWindow != null)
                        InvokeIfHandleCreated(imageWindow, delegate { imageWindow.实时显示ToolStripMenuItem.Checked = true; });

                    th_displayImage = new Thread(() =>
                    {
                        #region 相机实时
                        try
                        {
                            while (displayImageMode)
                            {
                                toolPar.ResultPar.图像 = SDK_Camera.GrabOneImage();
                                if (toolPar.ResultPar.图像 == null)
                                {
                                    displayImageMode = false;
                                    PostMainOutputMessage("相机实时失败：未采集到图像 " + SDK_Camera.CameraInfoStr, Color.Red);
                                    break;
                                }
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
                                    window.HobjectToHimage(toolPar.ResultPar.图像);
                                    DispRealtimeHint(window.HWindowHalconID);
                                }
                                else
                                {
                                    ShowImage(toolPar.ResultPar.图像);
                                    Show_Text("实时中...");
                                }

                                Thread.Sleep(10);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.SaveError(ex);
                        }
                        finally
                        {
                            if (!displayImageMode)
                            {
                                UpdateAcqToolUi(delegate
                                {
                                    Frm_AcqImageTool form = TryGetAcqToolForm();
                                    if (form == null)
                                        return;

                                    form.相机实时ToolStripMenuItem.Checked = false;
                                    form.btn_runTool.Enabled = true;
                                });
                                UpdateFromDeviceUi(delegate
                                {
                                    Frm_FromDevice form = TryGetFromDeviceForm();
                                    if (form == null)
                                        return;

                                    form.btn_displayImage.Text = "实时采集";
                                    form.btn_saveImage.Enabled = true;
                                    form.cbx_deviceList.Enabled = true;
                                    form.tbx_exposure.Enabled = true;
                                    form.tkb_exposure.Enabled = true;
                                });
                                Frm_ImageWindow realtimeImageWindow = GetImageWindowControl(jobName);
                                if (realtimeImageWindow != null)
                                    InvokeIfHandleCreated(realtimeImageWindow, delegate { realtimeImageWindow.实时显示ToolStripMenuItem.Checked = false; });
                            }
                        }
                        #endregion
                    });
                    th_displayImage.IsBackground = true;
                    th_displayImage.Start();
                }
                else
                {
                    displayImageMode = false;
                    Frm_AcqImageTool acqForm = TryGetAcqToolForm();
                    SaveLastPreviewImage(acqForm == null ? string.Empty : acqForm.toolName);
                    UpdateAcqToolUi(delegate
                    {
                        Frm_AcqImageTool form = TryGetAcqToolForm();
                        if (form == null)
                            return;

                        form.相机实时ToolStripMenuItem.Checked = false;
                        form.btn_runTool.Enabled = true;
                    });
                    UpdateFromDeviceUi(delegate
                    {
                        Frm_FromDevice form = TryGetFromDeviceForm();
                        if (form == null)
                            return;

                        form.btn_displayImage.Text = "实时采集";
                        form.btn_saveImage.Enabled = true;
                        form.cbx_deviceList.Enabled = true;
                        form.tbx_exposure.Enabled = true;
                        form.tkb_exposure.Enabled = true;
                    });
                    Frm_ImageWindow imageWindow = GetImageWindowControl(jobName);
                    if (imageWindow != null)
                        InvokeIfHandleCreated(imageWindow, delegate { imageWindow.实时显示ToolStripMenuItem.Checked = false; });
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void DispRealtimeHint(HTuple windowHandle)
        {
            try
            {
                HTuple row1, col1, row2, col2;
                HOperatorSet.GetPart(windowHandle, out row1, out col1, out row2, out col2);
                DispMessage(windowHandle, "实时中...", 12, row1 + (row2 - row1) / 30, col1 + (col2 - col1) / 30, "blue", "false");
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
                if (toolPar.ResultPar.图像 == null)
                {
                    Frm_Main.Instance.OutputMsg("当前没有可保存的图像", Color.Red);
                    return;
                }

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
                        toolPar.ResultPar.自定义路径 = dig_saveFileDialog.FileName;
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
        /// 选择运行采集后的自动保存目录。
        /// </summary>
        internal void SelectCustomSaveDirectory()
        {
            try
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "请选择运行采集后的图片保存目录";
                    dialog.SelectedPath = Directory.Exists(customSaveDirectory)
                        ? customSaveDirectory
                        : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        customSaveDirectory = dialog.SelectedPath;
                        Frm_FromDevice.Instance.tbx_saveDirectory.Text = customSaveDirectory;
                        toolPar.ResultPar.自定义路径 = customSaveDirectory;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 选择流程运行使用的模板图片。
        /// </summary>
        internal void SelectTemplateImagePath()
        {
            try
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Title = "请选择流程运行模板图片";
                    dialog.Filter = "图像文件(*.*)|*.*|图像文件(*.tif)|*.tif|图像文件(*.png)|*.png|图像文件(*.jpg)|*.jpg|图像文件(*.bmp)|*.bmp";
                    dialog.InitialDirectory = string.IsNullOrEmpty(templateImagePath) || !File.Exists(templateImagePath)
                        ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                        : Path.GetDirectoryName(templateImagePath);

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        templateImagePath = dialog.FileName;
                        lastPreviewImagePath = dialog.FileName;
                        Frm_FromDevice.Instance.tbx_templateImagePath.Text = templateImagePath;

                        if (!PrimeTemplateImageCache(true))
                            return;

                        Frm_AcqImageTool form = Frm_AcqImageTool.CurrentInstance;
                        if (form != null && !form.IsDisposed && form.IsHandleCreated)
                            form.hWindow_Final1.HobjectToHimage(toolPar.ResultPar.图像);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                PostMainOutputMessage("模板图片读取失败：" + ex.Message, Color.Red);
            }
        }
        internal bool PrimeTemplateImageCache(bool reportError)
        {
            try
            {
                if (string.IsNullOrEmpty(templateImagePath) || !File.Exists(templateImagePath))
                {
                    toolRunStatu = ToolRunStatu.未指定图像路径;
                    if (reportError)
                        PostMainOutputMessage("未指定流程运行模板图片", Color.Red);
                    return false;
                }

                if (templateImageCache != null && templateImageCachePath == templateImagePath)
                {
                    toolPar.ResultPar.图像 = templateImageCache.Clone();
                    UpdateOutput(string.Empty);
                    return true;
                }

                HObject templateImage;
                HOperatorSet.ReadImage(out templateImage, templateImagePath);
                templateImageCache = templateImage;
                templateImageCachePath = templateImagePath;
                toolPar.ResultPar.图像 = templateImage.Clone();
                UpdateOutput(string.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                toolRunStatu = ToolRunStatu.图像文件异常或路径不合法;
                if (reportError)
                    PostMainOutputMessage("模板图片读取失败：" + ex.Message, Color.Red);
                return false;
            }
        }
        private void SaveCapturedImageToCustomDirectory(string toolName)
        {
            try
            {
                if (string.IsNullOrEmpty(customSaveDirectory))
                {
                    toolPar.ResultPar.自定义路径 = string.Empty;
                    return;
                }

                if (!Directory.Exists(customSaveDirectory))
                    Directory.CreateDirectory(customSaveDirectory);

                string safeToolName = string.IsNullOrEmpty(toolName) ? "AcqImage" : toolName;
                foreach (char c in Path.GetInvalidFileNameChars())
                    safeToolName = safeToolName.Replace(c, '_');

                string filePath = Path.Combine(customSaveDirectory, string.Format("{0}_{1}.tif", safeToolName, DateTime.Now.ToString("yyyyMMdd_HHmmss_fff")));
                HOperatorSet.WriteImage(toolPar.ResultPar.图像, "tiff", 0, filePath);
                toolPar.ResultPar.自定义路径 = filePath;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                toolPar.ResultPar.自定义路径 = string.Empty;
                Frm_Main.Instance.OutputMsg(string.Format("采集图像保存失败：{0}", ex.Message), Color.Red);
            }
        }
        private void SaveLastPreviewImage(string toolName)
        {
            try
            {
                if (IsUsingInternalSampleImagePath())
                    return;

                if (toolPar.ResultPar.图像 == null)
                    return;

                string previewDir = Path.Combine(Application.StartupPath, "Config", "Runtime", "Preview");
                if (!Directory.Exists(previewDir))
                    Directory.CreateDirectory(previewDir);

                string safeJobName = string.IsNullOrEmpty(jobName) ? "Job" : jobName;
                string safeToolName = string.IsNullOrEmpty(toolName) ? "AcqImage" : toolName;
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    safeJobName = safeJobName.Replace(c, '_');
                    safeToolName = safeToolName.Replace(c, '_');
                }

                string filePath = Path.Combine(previewDir, safeJobName + "_" + safeToolName + ".tif");
                HOperatorSet.WriteImage(toolPar.ResultPar.图像, "tiff", 0, filePath);
                lastPreviewImagePath = filePath;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal bool TryLoadLastPreviewImage()
        {
            try
            {
                if (ClearInternalSamplePreview())
                    return false;

                string previewPath = lastPreviewImagePath;
                if (imageSourceMode == ImageSourceMode.FromDevice && string.IsNullOrWhiteSpace(previewPath))
                    return false;

                if (string.IsNullOrWhiteSpace(previewPath))
                    previewPath = imagePath;

                if (!string.IsNullOrWhiteSpace(previewPath))
                {
                    string fullPath = absPath ? previewPath : Application.StartupPath + previewPath;
                    if (File.Exists(fullPath))
                    {
                        HObject image;
                        HOperatorSet.ReadImage(out image, fullPath);
                        toolPar.ResultPar.图像 = image;
                        lastPreviewImagePath = fullPath;
                        return true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(imageDirectoryPath))
                {
                    string dir = absPath ? imageDirectoryPath : Application.StartupPath + imageDirectoryPath;
                    if (Directory.Exists(dir))
                    {
                        string[] files = Directory.GetFiles(dir)
                            .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                || file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
                                || file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                || file.EndsWith(".tif", StringComparison.OrdinalIgnoreCase))
                            .ToArray();
                        if (files.Length > 0)
                        {
                            Array.Sort(files);
                            int index = Math.Max(0, Math.Min(currentImageIndex, files.Length - 1));
                            HObject image;
                            HOperatorSet.ReadImage(out image, files[index]);
                            L_images = files.ToList();
                            currentImageIndex = index;
                            currentImageName = Path.GetFileName(files[index]);
                            toolPar.ResultPar.图像 = image;
                            lastPreviewImagePath = files[index];
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }

            return false;
        }

        /// <summary>
        /// 进入采集工具或选中流程节点时检查本次会话中的预览。
        /// 启动/加载项目后没有新输入时保持黑屏，不自动恢复上次预览文件。
        /// </summary>
        internal bool EnsurePreviewImage()
        {
            try
            {
                ClearLegacyInternalTemplate();
                ClearInternalSamplePreview();
                return toolPar.ResultPar.图像 != null && toolPar.ResultPar.图像.IsInitialized();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }

        /// <summary>
        /// 清理旧工程中指向程序内置样图的模板设置，防止它覆盖真实采集/选择的预览。
        /// </summary>
        private void ClearLegacyInternalTemplate()
        {
            if (!IsInternalSampleImagePath(templateImagePath))
                return;

            useTemplateImageInRun = false;
            templateImagePath = string.Empty;
            templateImageCache = null;
            templateImageCachePath = string.Empty;

            // 有真实预览缓存时由 EnsurePreviewImage 继续恢复；没有缓存才清掉旧样图对象。
            if (string.IsNullOrWhiteSpace(lastPreviewImagePath))
                toolPar.ResultPar.图像 = null;
        }
        internal bool ClearInternalSamplePreview()
        {
            try
            {
                if (!IsUsingInternalSampleImagePath())
                    return false;

                imagePath = string.Empty;
                imageDirectoryPath = string.Empty;
                lastPreviewImagePath = string.Empty;
                currentImageName = string.Empty;
                currentImageIndex = 0;
                L_images.Clear();
                toolPar.ResultPar.图像 = null;
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }

        private bool IsUsingInternalSampleImagePath()
        {
            return IsInternalSampleImagePath(imagePath)
                || IsInternalSampleImagePath(imageDirectoryPath)
                || IsInternalSampleImagePath(lastPreviewImagePath);
        }

        private bool IsInternalSampleImagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                string fullPath = path;
                if (!Path.IsPathRooted(fullPath))
                    fullPath = Path.Combine(Application.StartupPath, fullPath.TrimStart('\\', '/'));

                string sampleRoot = Path.Combine(Application.StartupPath, "Config", "Resources", "Image");
                fullPath = Path.GetFullPath(fullPath).TrimEnd('\\', '/');
                sampleRoot = Path.GetFullPath(sampleRoot).TrimEnd('\\', '/');
                return fullPath.Equals(sampleRoot, StringComparison.OrdinalIgnoreCase)
                    || fullPath.StartsWith(sampleRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                    || fullPath.StartsWith(sampleRoot + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return path.IndexOf(@"Config\Resources\Image", StringComparison.OrdinalIgnoreCase) >= 0
                    || path.IndexOf("Config/Resources/Image", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        private void BeginClearImageWindow()
        {
            try
            {
                Frm_ImageWindow ctrl = GetImageWindowControl();
                if (ctrl == null) return;
                if (ctrl.InvokeRequired)
                    ctrl.BeginInvoke(new MethodInvoker(delegate { ctrl.hwc_imageWindow.ClearWindow(); }));
                else
                    ctrl.hwc_imageWindow.ClearWindow();
            }
            catch (Exception ex) { Log.SaveError(ex); }
        }
        private bool TryRunCameraAction(Action action, int timeoutMs, out Exception actionException)
        {
            actionException = null;
            Exception workerException = null;
            bool finished = false;

            Thread worker = new Thread(new ThreadStart(delegate
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    workerException = ex;
                }
                finally
                {
                    finished = true;
                }
            }));
            worker.IsBackground = true;
            worker.Start();

            bool joined = worker.Join(timeoutMs);
            if (!joined || !finished)
                return false;

            actionException = workerException;
            return true;
        }
        /// <summary>
        /// 刷新输出
        /// </summary>
        /// <param name="toolName">工具名称</param>
        internal void UpdateOutput(string toolName)
        {
            try
            {
                Job job = Job.FindJobByName(jobName);
                ToolInfo toolInfo = ResolveToolInfo(job, toolName);
                if (toolInfo == null || toolInfo.output == null)
                    return;

                string resolvedToolName = toolInfo.toolName;
                List<ToolIO> L_toolIO = toolInfo.output;
                for (int i = 0; i < L_toolIO.Count; i++)
                {
                    string outputItem = L_toolIO[i].IOName;
                    object value = ResolveOutputValue(outputItem);
                    toolInfo.GetOutput(outputItem).value = value;

                    TreeNode outputNode = job.GetToolIONodeByNodeText(resolvedToolName, "-->" + outputItem);
                    if (outputNode != null)
                        outputNode.ToolTipText = FormatShowTip(value);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private ToolInfo ResolveToolInfo(Job job, string toolName)
        {
            if (job == null)
                return null;

            if (!string.IsNullOrEmpty(toolName))
            {
                ToolInfo namedToolInfo = job.FindToolInfoByName(toolName);
                if (namedToolInfo != null && namedToolInfo.tool == this)
                    return namedToolInfo;
            }

            for (int i = 0; i < job.L_toolList.Count; i++)
            {
                if (job.L_toolList[i].tool == this)
                    return job.L_toolList[i];
            }

            return null;
        }
        private object ResolveOutputValue(string outputItem)
        {
            if (outputItem == "\u56fe\u50cf" || outputItem == "\u8f93\u51fa\u56fe\u50cf" || outputItem == "OutputImage")
                return GetValue(GetValue(toolPar, "ResultPar"), "\u56fe\u50cf");

            if (outputItem == "\u81ea\u5b9a\u4e49\u8def\u5f84" || outputItem == "CustomPath")
                return GetValue(GetValue(toolPar, "ResultPar"), "\u81ea\u5b9a\u4e49\u8def\u5f84");

            string[] items = Regex.Split(outputItem, " . ");
            object value = GetValue(toolPar, "ResultPar");
            for (int j = 0; j < items.Length; j++)
                value = GetValue(value, items[j]);
            return value;
        }
        private Frm_AcqImageTool GetCurrentAcqWindow(string toolName)
        {
            Frm_AcqImageTool form = Frm_AcqImageTool.CurrentInstance;
            if (form != null
                && !form.IsDisposed
                && form.IsHandleCreated
                && form.Visible
                && form.jobName == jobName
                && form.toolName == toolName)
                return form;

            return null;
        }
        private bool CanDisplayInAcqWindow(string toolName)
        {
            return GetCurrentAcqWindow(toolName) != null;
        }
        private void InvokeAcqWindow(string toolName, MethodInvoker action)
        {
            Frm_AcqImageTool form = GetCurrentAcqWindow(toolName);
            if (form == null)
                return;

            if (form.InvokeRequired)
            {
                form.BeginInvoke(new MethodInvoker(delegate
                {
                    if (GetCurrentAcqWindow(toolName) != null)
                        action();
                }));
            }
            else
            {
                action();
            }
        }
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否更新图像</param>
        /// <param name="debugTool">调试工具模式</param>
        public override void Run(bool updateImage, bool runTool, string toolName)
        {
            try
            {
                bool usingTemplateImage = useTemplateImageInRun;
                RGBToGray = false;
                displayAllImageRegion = true;
                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);

                if (runTool)
                    ClearAcqToolPreview();

                if (usingTemplateImage)
                {
                    if (!PrimeTemplateImageCache(true))
                    {
                        if (runTool)
                            GetImageWindowControl().hwc_imageWindow.ClearWindow();
                        else if (updateImage)
                            BeginClearImageWindow();
                        return;
                    }
                }
                else
                    switch (imageSourceMode)
                    {
                        case ImageSourceMode.FromDevice:
                            #region 从设备采集
                            if (displayImageMode)
                            {
                                toolRunStatu = ToolRunStatu.相机实时状态下不可采集图像;
                                if (runTool)
                                {
                                    Frm_MessageBox.Instance.MessageBoxShow("\r\n相机实时状态下不可采集图像，请停止实时后重试", TipType.Error);
                                    GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                }
                                return;
                            }
                            if (SDK_Camera == null)
                            {
                                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Acq_Device : ToolRunStatu.未指定采集设备);
                                if (runTool)
                                    GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                else if (updateImage)
                                    BeginClearImageWindow();
                                if (CanDisplayInAcqWindow(toolName))
                                {
                                    InvokeAcqWindow(toolName, delegate
                                    {
                                        Frm_AcqImageTool.CurrentInstance.hWindow_Final1.ClearWindow();
                                        Frm_AcqImageTool.CurrentInstance.lbl_toolTip.ForeColor = Color.Red;
                                        Frm_AcqImageTool.CurrentInstance.lbl_toolTip.Text = "状态：未指定采集设备";
                                    });
                                }
                                return;
                            }

                            Exception openException;
                            bool cameraOpened = false;
                            if (!TryRunCameraAction(delegate { cameraOpened = SDK_Camera.TryOpenCamera(); }, AcquisitionTimeoutMs, out openException))
                            {
                                toolRunStatu = ToolRunStatu.运行超时;
                                Frm_Main.Instance.OutputMsg(string.Format("打开采集相机超时，超时阈值 {0}ms", AcquisitionTimeoutMs), Color.Red);
                                if (runTool)
                                    GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                else if (updateImage)
                                    BeginClearImageWindow();
                                return;
                            }
                            if (openException != null)
                            {
                                Log.SaveError(openException);
                                toolRunStatu = ToolRunStatu.采集图像时出错;
                                Frm_Main.Instance.OutputMsg("打开采集相机异常", Color.Red);
                                if (runTool)
                                    GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                else if (updateImage)
                                    BeginClearImageWindow();
                                return;
                            }
                            if (!cameraOpened)
                            {
                                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Acq_Device : ToolRunStatu.相机未连接);
                                if (runTool)
                                    GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                else if (updateImage)
                                    BeginClearImageWindow();
                                return;
                            }

                            if (hardTriggerMode)
                            {
                                SDK_Camera.waitingHardTriggerImage = true;
                                Stopwatch hardTriggerWatch = Stopwatch.StartNew();
                                while (SDK_Camera.waitingHardTriggerImage)
                                {
                                    if (hardTriggerWatch.ElapsedMilliseconds > HardTriggerTimeoutMs)
                                    {
                                        SDK_Camera.waitingHardTriggerImage = false;
                                        toolRunStatu = ToolRunStatu.采集图像时出错;
                                        Frm_Main.Instance.OutputMsg("等待硬触发图像超时", Color.Red);
                                        if (runTool)
                                            GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                        else if (updateImage)
                                            BeginClearImageWindow();
                                        return;
                                    }
                                    Thread.Sleep(20);
                                }
                            }
                            else
                            {
                                Exception exposureException;
                                if (!TryRunCameraAction(delegate { SDK_Camera.SetExposure(exposure); }, AcquisitionTimeoutMs, out exposureException))
                                {
                                    toolRunStatu = ToolRunStatu.运行超时;
                                    Frm_Main.Instance.OutputMsg("设置相机曝光超时", Color.Red);
                                    if (runTool)
                                        GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                    else if (updateImage)
                                        BeginClearImageWindow();
                                    return;
                                }
                                if (exposureException != null)
                                {
                                    Log.SaveError(exposureException);
                                    toolRunStatu = ToolRunStatu.采集图像时出错;
                                    Frm_Main.Instance.OutputMsg("设置相机曝光异常", Color.Red);
                                    return;
                                }

                                HObject grabbedImage = null;
                                Exception grabException;
                                if (!TryRunCameraAction(delegate { grabbedImage = SDK_Camera.GrabOneImage(); }, AcquisitionTimeoutMs, out grabException))
                                {
                                    toolRunStatu = ToolRunStatu.运行超时;
                                    Frm_Main.Instance.OutputMsg(string.Format("采集图像超时，超时阈值 {0}ms", AcquisitionTimeoutMs), Color.Red);
                                    if (runTool)
                                        GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                    else if (updateImage)
                                        BeginClearImageWindow();
                                    return;
                                }
                                if (grabException != null)
                                {
                                    Log.SaveError(grabException);
                                    toolRunStatu = ToolRunStatu.采集图像时出错;
                                    Frm_Main.Instance.OutputMsg("采集图像异常", Color.Red);
                                    if (runTool)
                                        GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                    else if (updateImage)
                                        BeginClearImageWindow();
                                    return;
                                }

                                if (grabbedImage == null)
                                {
                                    toolRunStatu = ToolRunStatu.采集图像时出错;
                                    if (runTool)
                                        GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                    else if (updateImage)
                                        BeginClearImageWindow();
                                    return;
                                }
                                toolPar.ResultPar.图像 = grabbedImage;
                            }
                            break;
                        #endregion
                        case ImageSourceMode.FromFile:
                            #region 从文件读取
                            if (imagePath == string.Empty)
                            {
                                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Path : ToolRunStatu.未指定图像路径);
                                if (runTool)
                                    GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                else if (updateImage)
                                    BeginClearImageWindow();
                                if (CanDisplayInAcqWindow(toolName))
                                {
                                    InvokeAcqWindow(toolName, delegate
                                    {
                                        Frm_AcqImageTool.CurrentInstance.hWindow_Final1.ClearWindow();
                                        Frm_AcqImageTool.CurrentInstance.lbl_toolTip.ForeColor = Color.Red;
                                        Frm_AcqImageTool.CurrentInstance.lbl_toolTip.Text = "状态：未指定图像路径";
                                    });
                                }
                                return;
                            }
                            HObject imageFromFile;
                            if (absPath)
                            {
                                HOperatorSet.ReadImage(out imageFromFile, imagePath);
                                lastPreviewImagePath = imagePath;
                            }
                            else
                            {
                                HOperatorSet.ReadImage(out imageFromFile, Application.StartupPath + imagePath);
                                lastPreviewImagePath = Application.StartupPath + imagePath;
                            }
                            toolPar.ResultPar.图像 = imageFromFile;
                            break;
                        #endregion
                        case ImageSourceMode.FromDirectory:
                            #region 从文件夹读取
                            if (imageDirectoryPath == string.Empty)
                            {
                                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Path : ToolRunStatu.未指定图像路径);
                                if (runTool)
                                    GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                else if (updateImage)
                                    BeginClearImageWindow();
                                if (CanDisplayInAcqWindow(toolName))
                                {
                                    InvokeAcqWindow(toolName, delegate
                                    {
                                        Frm_AcqImageTool.CurrentInstance.hWindow_Final1.ClearWindow();
                                        Frm_AcqImageTool.CurrentInstance.lbl_toolTip.ForeColor = Color.Red;
                                        Frm_AcqImageTool.CurrentInstance.lbl_toolTip.Text = "状态：未指定图像路径";
                                    });
                                }
                                return;
                            }

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
                                if (runTool)
                                    GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                else if (updateImage)
                                    BeginClearImageWindow();
                                return;
                            }

                            L_images.Clear();
                            for (int i = 0; i < files.Length; i++)
                            {
                                FileInfo fileInfo = new FileInfo(files[i]);
                                if (fileInfo.Extension == ".jpg" || fileInfo.Extension == ".bmp" || fileInfo.Extension == ".png" || fileInfo.Extension == ".tif")
                                    L_images.Add(files[i]);
                            }

                            if (L_images.Count == 0)
                            {
                                Frm_Main.Instance.OutputMsg("图像路径下无有效图像文件", Color.Black);
                                toolRunStatu = ToolRunStatu.文件夹内无图像;
                                if (runTool)
                                    GetImageWindowControl().hwc_imageWindow.ClearWindow();
                                else if (updateImage)
                                    BeginClearImageWindow();
                                return;
                            }

                            if (currentImageIndex > L_images.Count - 1)
                                currentImageIndex = 0;

                            HObject imageFromDir;
                            HOperatorSet.ReadImage(out imageFromDir, L_images[currentImageIndex]);
                            currentImageName = Path.GetFileName(L_images[currentImageIndex]);
                            lastPreviewImagePath = L_images[currentImageIndex];
                            toolPar.ResultPar.图像 = imageFromDir;

                            // 运行成功后自动切换下一张：单次运行始终切换；流程运行受“自动切换/停止切换”开关控制
                            if (!runTool || (autoSwitch && !Frm_Main.Instance.tsb_stopSwtich.Checked))
                                currentImageIndex = (currentImageIndex + 1) % L_images.Count;
                            break;
                            #endregion
                    }

                // 图像处理（纯计算，可在任意线程执行）
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

                if (rotateImage)
                {
                    HObject image2;
					HOperatorSet.RotateImage(toolPar.ResultPar.图像, out image2, rotateAngle, "constant");
                    toolPar.ResultPar.图像 = image2;
                }

                SaveCapturedImageToCustomDirectory(toolName);
                SaveLastPreviewImage(toolName);

                if (runTool)
                {
                    // 调试运行：在 UI 线程，直接操作控件
                    Frm_AcqImageTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.ResultPar.图像);
                    if (!displayAllImageRegion)
                    {
                        Frm_AcqImageTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                        Frm_AcqImageTool.Instance.L_regions = this.L_regions;
                    }
                }
                else
                {
                    // 模板图流程运行只负责给下游提供图像，避免刷新主图像窗口造成卡顿。
                    if (usingTemplateImage)
                    {
                        if (updateImage)
                            ShowImage(toolPar.ResultPar.图像);

                        if (CanDisplayInAcqWindow(toolName))
                        {
                            InvokeAcqWindow(toolName, delegate
                            {
                                Frm_AcqImageTool.CurrentInstance.hWindow_Final1.HobjectToHimage(toolPar.ResultPar.图像);
                                Frm_AcqImageTool.CurrentInstance.lbl_toolTip.ForeColor = Color.Black;
                                Frm_AcqImageTool.CurrentInstance.lbl_toolTip.Text = "状态：成功，已使用模板图";
                            });
                        }
                    }
                    else if (updateImage)
                    {
                        ShowImage(toolPar.ResultPar.图像);
                        if (CanDisplayInAcqWindow(toolName))
                        {
                            InvokeAcqWindow(toolName, delegate
                            {
                                Frm_AcqImageTool.CurrentInstance.hWindow_Final1.HobjectToHimage(toolPar.ResultPar.图像);
                            });
                        }
                    }
                }

                UpdateOutput(toolName);
                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        public override ToolRunResult Execute(ToolRunContext context)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Run(true, false, context == null ? string.Empty : context.ToolName);
            sw.Stop();

            bool success = toolRunStatu == (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
            return new ToolRunResult
            {
                Success = success,
                Timeout = toolRunStatu == ToolRunStatu.运行超时,
                Canceled = toolRunStatu == ToolRunStatu.用户取消,
                Status = toolRunStatu,
                Message = toolRunStatu.ToString(),
                ElapsedMs = sw.ElapsedMilliseconds
            };
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
            public HObject OutputImage
            {
                get { return _图像; }
                set { _图像 = value; }
            }
            public HObject 输出图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }
            private string _自定义路径 = string.Empty;
            public string 自定义路径
            {
                get { return _自定义路径; }
                set { _自定义路径 = value; }
            }
            public string CustomPath
            {
                get { return _自定义路径; }
                set { _自定义路径 = value; }
            }
        }
        #endregion

    }
    /// <summary>
    /// 图像源模式：从设备采集 | 读取图像文件 | 读取文件夹图像
    /// </summary>
    internal enum ImageSourceMode
    {
        FromDevice,
        FromFile,
        FromDirectory,
    }
}
