using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_FromDevice : Form
    {
        internal Frm_FromDevice()
        {
            InitializeComponent();

            btn_displayImage.Clicked += btn_realTime_Clicked;
            btn_saveImage.Clicked += btn_saveImage_Clicked;
            btn_browseSaveDirectory.Click += btn_browseSaveDirectory_Click;
            ckb_useTemplateImage.CheckedChanged += ckb_useTemplateImage_CheckedChanged;
            btn_browseTemplateImage.Click += btn_browseTemplateImage_Click;
            LoadExposureBrightness(imageAcqTool.exposure);
            InitializeManualIpControls();
        }

        /// <summary>
        /// 锁
        /// </summary>
        private object obj = new object();
        private bool updatingExposureUI = false;
        private TextBox tbx_manualIp;
        private Button btn_addManualIp;
        internal const double ExposureSliderScale = 10.0;
        internal const double ExposureMinValue = 0.1;
        internal const double ExposureMaxValue = 100.0;
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
        private static Frm_FromDevice _instance;
        internal static Frm_FromDevice Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_FromDevice();
                return _instance;
            }
        }
        internal static Frm_FromDevice CurrentInstance
        {
            get { return _instance; }
        }

        private AcqImageTool GetBoundTool()
        {
            try
            {
                if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(toolName))
                {
                    AcqImageTool boundTool = Job.FindToolByName(jobName, toolName) as AcqImageTool;
                    if (boundTool != null)
                    {
                        imageAcqTool = boundTool;
                        return boundTool;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }

            return imageAcqTool;
        }

        private void InitializeManualIpControls()
        {
            System.Windows.Forms.Label lblManualIp = new System.Windows.Forms.Label();
            lblManualIp.AutoSize = true;
            lblManualIp.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            lblManualIp.Location = new Point(5, 66);
            lblManualIp.Name = "lblManualIp";
            lblManualIp.Size = new Size(24, 17);
            lblManualIp.Text = "IP:";

            tbx_manualIp = new TextBox();
            tbx_manualIp.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            tbx_manualIp.Location = new Point(34, 63);
            tbx_manualIp.Name = "tbx_manualIp";
            tbx_manualIp.Size = new Size(150, 23);

            btn_addManualIp = new Button();
            btn_addManualIp.Cursor = Cursors.Hand;
            btn_addManualIp.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            btn_addManualIp.Location = new Point(190, 62);
            btn_addManualIp.Name = "btn_addManualIp";
            btn_addManualIp.Size = new Size(73, 25);
            btn_addManualIp.Text = "添加IP";
            btn_addManualIp.UseVisualStyleBackColor = true;
            btn_addManualIp.Click += btn_addManualIp_Click;

            Controls.Add(lblManualIp);
            Controls.Add(tbx_manualIp);
            Controls.Add(btn_addManualIp);
        }

        private void btn_addManualIp_Click(object sender, EventArgs e)
        {
            try
            {
                string manualIp = tbx_manualIp.Text.Trim();
                if (!Regex.IsMatch(manualIp, @"^\d{1,3}(\.\d{1,3}){3}$"))
                {
                    Frm_Main.Instance.OutputMsg("手动添加相机失败：IP地址必须是四段格式，例如 192.168.1.11", Color.Red);
                    return;
                }

                IPAddress ipAddress;
                if (!IPAddress.TryParse(manualIp, out ipAddress) || ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Frm_Main.Instance.OutputMsg("手动添加相机失败：IP地址格式不正确", Color.Red);
                    return;
                }

                if (!GetBoundTool().AddManualIpCamera(ipAddress.ToString()))
                    Frm_Main.Instance.OutputMsg("手动添加相机失败", Color.Red);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void cbx_deviceList_SelectedIndexChanged()
        {
            GetBoundTool().SwitchDevice(cbx_deviceList.TextStr);
        }
        private void tbx_exposure_ValueChanged(double value)
        {
            try
            {
                lock (obj)
                {
                    UpdateExposureBrightness(value, true);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tkb_exposure_Scroll(object sender, EventArgs e)
        {
            UpdateExposureBrightness(SliderValueToExposure(tkb_exposure.Value), false);
        }

        private void tkb_exposure_ValueChanged(object sender, EventArgs e)
        {
            UpdateExposureBrightness(SliderValueToExposure(tkb_exposure.Value), false);
        }

        internal void LoadExposureBrightness(double value)
        {
            try
            {
                updatingExposureUI = true;
                double exposure = NormalizeExposure(value);
                tbx_exposure.Value = exposure;
                tkb_exposure.Value = ExposureToSliderValue(exposure);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
            finally
            {
                updatingExposureUI = false;
            }
        }

        private void UpdateExposureBrightness(double value, bool fromInput)
        {
            if (updatingExposureUI)
                return;

            try
            {
                updatingExposureUI = true;

                double exposure = NormalizeExposure(value);
                int sliderValue = ExposureToSliderValue(exposure);

                if (tkb_exposure.Value != sliderValue)
                    tkb_exposure.Value = sliderValue;

                if (!fromInput && Math.Abs(tbx_exposure.Value - exposure) > 0.0001)
                    tbx_exposure.Value = exposure;

                AcqImageTool boundTool = GetBoundTool();
                boundTool.exposure = exposure;
                boundTool.ApplyExposureBrightness();
            }
            finally
            {
                updatingExposureUI = false;
            }
        }

        internal static int ExposureToSliderValue(double exposure)
        {
            return Math.Max(1, Math.Min((int)Math.Round(ExposureMaxValue * ExposureSliderScale), (int)Math.Round(exposure * ExposureSliderScale)));
        }

        internal static double SliderValueToExposure(int sliderValue)
        {
            return NormalizeExposure(sliderValue / ExposureSliderScale);
        }

        internal static double NormalizeExposure(double exposure)
        {
            return Math.Max(ExposureMinValue, Math.Min(ExposureMaxValue, exposure));
        }
        void btn_realTime_Clicked()
        {
            Frm_AcqImageTool form = Frm_AcqImageTool.CurrentInstance;
            if (form == null || !form.Visible)
            {
                Frm_Main.Instance.OutputMsg("相机实时失败：采集图像窗口未打开", Color.Red);
                return;
            }

            GetBoundTool().PlayImage(true, form.hWindow_Final1);
        }
        void btn_saveImage_Clicked()
        {
            GetBoundTool().SaveImage();
        }
        private void btn_browseSaveDirectory_Click(object sender, EventArgs e)
        {
            GetBoundTool().SelectCustomSaveDirectory();
        }
        private void ckb_useTemplateImage_CheckedChanged(object sender, EventArgs e)
        {
            AcqImageTool boundTool = GetBoundTool();
            boundTool.useTemplateImageInRun = ckb_useTemplateImage.Checked;
            if (ckb_useTemplateImage.Checked && !string.IsNullOrEmpty(boundTool.templateImagePath))
                boundTool.PrimeTemplateImageCache(false);
        }
        private void btn_browseTemplateImage_Click(object sender, EventArgs e)
        {
            GetBoundTool().SelectTemplateImagePath();
        }

    }
}
