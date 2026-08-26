using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_AcqImageTool : Frm_FormBase
    {
        internal Frm_AcqImageTool()
        {
            InitializeComponent();
            hWindow_Final1.hWindowControl.MouseUp += Hwindow_MouseUp;
        }

        /// <summary>
        /// 工具对象
        /// </summary>
        internal AcqImageTool acqImageTool = new AcqImageTool();
        /// <summary>
        /// ROI区域
        /// </summary>
        internal List<ViewWindow.Model.ROI> L_regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_AcqImageTool _instance;
        public static Frm_AcqImageTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_AcqImageTool();
                return _instance;
            }
        }
        internal static Frm_AcqImageTool CurrentInstance
        {
            get { return _instance; }
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
                //退出窗体时自动停止相机实时
                if (acqImageTool.displayImageMode)
                    acqImageTool.PlayImage(false, hWindow_Final1);

                acqImageTool.UpdateOutput(toolName);
                base.btn_baseClose_Click(sender, e);
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
            acqImageTool.PlayImage(相机实时ToolStripMenuItem.Checked, hWindow_Final1);
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
            acqImageTool.SaveImage();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            if (acqImageTool.displayImageMode)
                acqImageTool.PlayImage(false, hWindow_Final1);

            acqImageTool.UpdateOutput(toolName);
            this.Close();
        }
        private void rdo_fromDevice_CheckedChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            if (rdo_fromDevice.Checked)
                acqImageTool.SwitchImageSource(ImageSourceMode.FromDevice);
        }
        private void radio_FromLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            if (radio_FromLocalFile.Checked)
                acqImageTool.SwitchImageSource(ImageSourceMode.FromFile);
        }
        private void rdo_fromLocalDirectory_CheckedChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            if (rdo_fromLocalDirectory.Checked)
                acqImageTool.SwitchImageSource(ImageSourceMode.FromDirectory);
        }
        private void pic_fromDevice_Click(object sender, EventArgs e)
        {
            acqImageTool.SwitchImageSource(ImageSourceMode.FromDevice);
        }
        private void pic_fromLocalFile_Click(object sender, EventArgs e)
        {
            acqImageTool.SwitchImageSource(ImageSourceMode.FromFile);
        }
        private void pic_fromLocalDirectory_Click(object sender, EventArgs e)
        {
            acqImageTool.SwitchImageSource(ImageSourceMode.FromDirectory);
        }
        private void pic_onOff_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = !enable;
            if (!enable)
                pic_onOff.Image = Resources.开;
            else
                pic_onOff.Image = Resources.关;
        }
        private async void tsb_runTool_Click(object sender, EventArgs e)
        {
            try
            {
                btn_runTool.Enabled = false;
                ToolRunResult result = await Task.Run(() => acqImageTool.Execute(new ToolRunContext
                {
                    JobName = jobName,
                    ToolName = toolName,
                    TimeoutMs = 5000,
                    IsCancellationRequested = null
                }));
                long time = result.ElapsedMs;

                if (acqImageTool.toolPar.ResultPar.图像 != null)
                    hWindow_Final1.HobjectToHimage(acqImageTool.toolPar.ResultPar.图像);

                if (!result.Success)
                {
                    lbl_toolTip.ForeColor = Color.Red;
                    lbl_runTime.Text = string.Format("耗时：0ms");
                    lbl_toolTip.Text = "状态：" + result.Message;
                }
                else
                {
                    acqImageTool.UpdateOutput(toolName);
                    lbl_toolTip.ForeColor = Color.Black;
                    lbl_runTime.Text = string.Format("耗时：{0}ms", time.ToString());
                    lbl_toolTip.Text = string.IsNullOrEmpty(acqImageTool.toolPar.ResultPar.自定义路径)
                        ? "状态：" + result.Message
                        : "状态：成功，已保存：" + acqImageTool.toolPar.ResultPar.自定义路径;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
            finally
            {
                btn_runTool.Enabled = true;
            }
        }
        private void ckb_autoSwitch_CheckChanged(bool Checked)
        {
            acqImageTool.autoSwitch = Checked;
        }
        private void ckb_absPath_CheckChanged(bool Checked)
        {
            acqImageTool.absPath = Checked;
        }
        private void ckb_RGBToGray_CheckChanged(bool Checked)
        {
            acqImageTool.RGBToGray = Checked;
        }
        private void ckb_displayAllImageRegion_CheckChanged(bool Checked)
        {
            try
            {
                if (Job.loadForm)
                    return;

                if (Checked)
                {
                    acqImageTool.displayAllImageRegion = true;
                    hWindow_Final1.HobjectToHimage(acqImageTool.toolPar.ResultPar.图像);
                }
                else
                {
                    acqImageTool.displayAllImageRegion = false;
                    if (acqImageTool.L_regions.Count == 0)
                        hWindow_Final1.viewWindow.genRect1(200, 200, 400, 400, ref acqImageTool.L_regions);
                    else
                        Frm_AcqImageTool.Instance.hWindow_Final1.viewWindow.displayROI(acqImageTool.L_regions);
                    this.L_regions = acqImageTool.L_regions;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void ckb_rotateImage_CheckChanged(bool Checked)
        {
            try
            {
                acqImageTool.rotateImage = Checked;
                nud_rotateAngle.Visible = Checked;
                lbl_deg.Visible = Checked;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void nud_rotateAngle_ValueChanged(double value)
        {
            try
            {
                acqImageTool.rotateAngle = Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_AcqImageTool_KeyUp(object sender, KeyEventArgs e)
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
        private void cCheckBox1_CheckChanged(bool Checked)
        {
            acqImageTool.hardTriggerMode = Checked;
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

    }
}
