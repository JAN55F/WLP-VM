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
        /// <summary>
        /// 曝光改动防抖定时器：拖动滑条/输入数值时每个刻度都会触发 ValueChanged，
        /// 若每次刻度都全量执行一次采集 Run，高频重入会让相机 SDK 在原生层并发崩溃
        /// （进程直接闪退、无托管异常日志）；改为静默期（300ms 无新改动）后只执行一次。
        /// </summary>
        private Timer exposureRunTimer;

        private void tbx_exposure_ValueChanged(double value)
        {
            try
            {
                lock (obj)
                {
                    tkb_exposure.Value = (int)value;
                    imageAcqTool.exposure = value;
                    tbx_exposure.Focus();
                }
                ScheduleAcquisitionRun();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 重置防抖计时；静默期满后把一次完整采集投回 UI 线程执行。
        /// </summary>
        private void ScheduleAcquisitionRun()
        {
            if (IsDisposed || Disposing)
                return;
            if (exposureRunTimer == null)
            {
                exposureRunTimer = new Timer();
                exposureRunTimer.Interval = 300;
                exposureRunTimer.Tick += delegate
                {
                    try
                    {
                        exposureRunTimer.Stop();
                        if (IsDisposed || Disposing)
                            return;
                        lock (obj)
                        {
                            if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(toolName))
                            {
                                // 设备窗口从视觉界面菜单独立打开（无工具上下文）：
                                // 只下发曝光并刷新设备窗口预览；若走工具 Run，会以空流程名查流程，
                                // 弹“未找到名为的流程（错误代码：00001）”（ToolBase.GetImageWindowControlCore→Job.FindJobByName）。
                                Frm_AcqDevice.Instance.Run(true, false);
                            }
                            else
                            {
                                imageAcqTool.Run(true, true, toolName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 文件日志留全栈；日志区同步给一条摘要，避免“看不到异常信息”
                        Log.SaveError(ex, "图像采集设备窗口应用曝光");
                        try
                        {
                            Frm_Main.Instance.OutputMsg("图像采集设备应用曝光异常：" + ex.Message, Color.Red);
                        }
                        catch { }
                    }
                };
            }
            exposureRunTimer.Stop();
            exposureRunTimer.Start();
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
