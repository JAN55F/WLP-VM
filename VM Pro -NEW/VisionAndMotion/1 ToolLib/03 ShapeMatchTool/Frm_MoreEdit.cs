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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMPro.Properties;
using HalconDotNet;

namespace VMPro
{
    internal partial class Frm_MoreEdit : Frm_FormBase
    {
        internal Frm_MoreEdit()
        {
            InitializeComponent();
            hWindow_Final1.hWindowControl.MouseUp += Hwindow_MouseUp;
            btn_runTool.Click += btn_runTool_Click;
            button3.Click += btn_relearn_Click;
        }

        internal MatchTool shapeMatchTool = new MatchTool();
        /// <summary>
        /// ROI区域
        /// </summary>
        internal List<ViewWindow.Model.ROI> L_regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_MoreEdit _instance;
        public static Frm_MoreEdit Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_MoreEdit();
                return _instance;
            }
        }

        internal void BindTool(MatchTool tool)
        {
            try
            {
                if (tool != null)
                    shapeMatchTool = tool;

                this.TopMost = true;
                this.Text = "模板编辑";
                this.lbl_title.Text = "模板编辑";
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void RefreshDisplay()
        {
            try
            {
                HObject image = null;
                if (shapeMatchTool.toolPar.InputPar.图像 != null)
                    image = shapeMatchTool.toolPar.InputPar.图像;

                if (image == null)
                {
                    hWindow_Final1.ClearWindow();
                    return;
                }

                hWindow_Final1.HobjectToHimage(image);
                if (shapeMatchTool.SearchRegion != null)
                    hWindow_Final1.viewWindow.displayROI(shapeMatchTool.L_regions);
                if (shapeMatchTool.templateRegion != null)
                    hWindow_Final1.DispObj(shapeMatchTool.templateRegion, "green");
                hWindow_Final1.DispImageFit();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
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
        private void tsb_displayImage_Click(object sender, EventArgs e)
        {
            RefreshDisplay();
        }
        private void tsb_saveImage_Click(object sender, EventArgs e)
        {
            SaveCurrentImage();
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
            相机实时ToolStripMenuItem.Checked = false;
            Frm_MessageBox.Instance.MessageBoxShow("\r\n模板编辑使用当前流程输入图像，请在采集图像工具中控制相机实时。");
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
            SaveCurrentImage();
        }
        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
   
        private void btn_runJob_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            button100.Image = Resources.钉;
            //"运行流程"：从流程第一个工具开始依次运行（图像从上往下传递），直到运行到当前工具为止
            Job job = Job.FindJobByName(jobName);
            if (job != null)
                job.RunToTool(toolName, true);
            if (Frm_ShapeMatchTool.Instance.Visible)
                Frm_ShapeMatchTool.Instance.RefreshInputImageFromFlow(false);
            RefreshDisplay();
            this.Activate();
        }
        private void btn_runTool_Click(object sender, EventArgs e)
        {
            try
            {
                if (Frm_ShapeMatchTool.Instance.Visible)
                    Frm_ShapeMatchTool.Instance.RefreshInputImageFromFlow(false);
                shapeMatchTool.Run(true, true, toolName);
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_relearn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Frm_ShapeMatchTool.Instance.Visible)
                    Frm_ShapeMatchTool.Instance.RefreshInputImageFromFlow(false);
                shapeMatchTool.CreateAndShowTemplate();
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void SaveCurrentImage()
        {
            try
            {
                HObject image = shapeMatchTool.toolPar.InputPar.图像;
                if (image == null)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n当前没有可保存的图像。");
                    return;
                }

                SaveFileDialog dig_saveImage = new SaveFileDialog();
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd");
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image path" : "请选择图像保存路径";
                dig_saveImage.Filter = Project.Instance.configuration.language == Language.English ? "Image File(*.tif)|*.tif|Image File(*.png)|*.png|Image File(*.jpg)|*.jpg|Image File(*.*)|*.*" : "图像文件(*.tif)|*.tif|图像文件(*.png)|*.png|图像文件(*.jpg)|*.jpg|图像文件(*.*)|*.*";
                dig_saveImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                    HOperatorSet.WriteImage(image, "tiff", 0, dig_saveImage.FileName);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void ckb_autoSwitch_CheckChanged(bool Checked)
        {
        }
        private void ckb_absPath_CheckChanged(bool Checked)
        {
        }
        private void ckb_RGBToGray_CheckChanged(bool Checked)
        {
        }
        private void ckb_displayAllImageRegion_CheckChanged(bool Checked)
        {
            try
            {
                if (Job.loadForm)
                    return;
                RefreshDisplay();
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
