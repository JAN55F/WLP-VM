using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ViewWindow.Model;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_FindLineTool : Frm_FormBase
    {
        internal Frm_FindLineTool()
        {
            InitializeComponent();
            hWindow_Final1.hWindowControl.HMouseUp += Hwindow_MouseUp;
            hWindow_Final1.viewWindow.AddRoiObserver(RoiControllerChanged);
            hWindow_Final1.EnableImagePan = false;
            numericUpDown1.ValueChanged += numericUpDown1_valueChanged;
            previewRefreshTimer = new Timer();
            previewRefreshTimer.Interval = 150;
            previewRefreshTimer.Tick += PreviewRefreshTimer_Tick;
        }

        void numericUpDown1_valueChanged(double value)
        {
            findLineTool.minScore = numericUpDown1.Value;
            RefreshInteractivePreview();
        }

        /// <summary>
        /// 参数变化时保留窗口中正在编辑的同一个 ROI，避免重载图像后交互状态被重置。
        /// </summary>
        private Timer previewRefreshTimer;
        private bool previewRefreshPending;

        private void RefreshInteractivePreview()
        {
            previewRefreshPending = true;
            previewRefreshTimer.Stop();
            previewRefreshTimer.Start();
        }

        private void PreviewRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (!previewRefreshPending)
            {
                previewRefreshTimer.Stop();
                return;
            }

            // 流程线程尚未真正退出时只保存参数，不与 Run() 并发执行 HALCON。
            if (IsBoundJobExecuting())
                return;

            previewRefreshPending = false;
            previewRefreshTimer.Stop();
            RefreshInteractivePreviewNow();
        }

        private bool IsBoundJobExecuting()
        {
            try
            {
                if (string.IsNullOrEmpty(jobName) || Project.Instance.curEngine == null)
                    return false;

                Job job = Project.Instance.curEngine.FindJobByName(jobName);
                return job != null && job.IsExecutionActive;
            }
            catch
            {
                return false;
            }
        }

        private void RefreshInteractivePreviewNow()
        {
            if (findLineTool == null)
                return;

            if (!findLineTool.SyncDisplayedRoi(false))
                findLineTool.ShowContour(true, true);
            else
                findLineTool.ShowContour(true, true, true);
        }


        /// <summary>
        /// 注册haclon窗体的鼠标弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hwindow_MouseUp(object sender, HMouseEventArgs e)
        {
            try
            {
                if (findLineTool != null)
                    RefreshInteractivePreview();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// ROIController 在真正修改完 ROI 坐标后发出移动事件。
        /// 拖动期间只做轻量预览，避免 Metrology 阻塞后续鼠标移动事件。
        /// </summary>
        private void RoiControllerChanged(int eventType)
        {
            try
            {
                if (eventType != ROIController.EVENT_MOVING_ROI || findLineTool == null || IsBoundJobExecuting())
                    return;

                if (findLineTool.SyncDisplayedRoi(false))
                    findLineTool.ShowDraggingPreview();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_FindLineTool _instance;
        internal static Frm_FindLineTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_FindLineTool();
                return _instance;
            }
        }
        /// <summary>
        /// 窗体是否已打开（避免在流程运行线程访问 Instance 时误创建隐藏窗体）。
        /// </summary>
        internal static bool IsOpen
        {
            get { return _instance != null && !_instance.IsDisposed && _instance.Visible; }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static FindLineTool findLineTool;


        private void cbx_polarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            findLineTool.polarity = (cbx_polarity.SelectedIndex == 0 ? "positive" : "negative");
        }
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void ckb_findLineToolEnable_CheckedChanged(object sender, EventArgs e)
        {
            //Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable  = ckb_toolEnable.Checked;
        }
        private void tbx_threshold_TextChanged(object sender, EventArgs e)
        {

        }
        private void tbx_caliperNum_TextChanged(object sender, EventArgs e)
        {
            try
            {
                findLineTool.cliperNum = Convert.ToInt16(tbx_caliperNum.Text.Trim());
            }
            catch { }
        }

        private void cbx_edgeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            findLineTool.edgeSelect = cbx_edgeSelect.Text;
        }
        private void ckb_displayCaliper_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void ckb_displayFeature_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void btn_editCaliper_Click(object sender, EventArgs e)
        {
            findLineTool.EditCaliper();
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            findLineTool.Run(true, true, toolName);
            if (findLineTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(findLineTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(findLineTool.toolRunStatu.ToString(), Color.Black);
        }
        private void btn_runTool_Click(object sender, EventArgs e)
        {
            findLineTool.Run(true, true, toolName);
            if (findLineTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(findLineTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(findLineTool.toolRunStatu.ToString(), Color.Black);
        }

        private void 显示信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.barVisible_strip_CheckedChanged(sender, e);
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

        private void button4_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            findLineTool.Run(true, true,toolName );
            if (findLineTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(findLineTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(findLineTool.toolRunStatu.ToString(), Color.Green);
            btn_runTool.Enabled = true;
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                findLineTool.threshold = Convert.ToInt16(tbx_threshold.Text.Trim());
            }
            catch { }
        }

        private void cbx_polarity_SelectedIndexChanged()
        {
            findLineTool.polarity = (cbx_polarity.SelectedIndex == 0 ? "positive" : "negative");
            RefreshInteractivePreview();
        }

        private void tkb_exposure_Scroll(object sender, EventArgs e)
        {
            tbx_threshold.Value = tkb_exposure.Value;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            tbx_caliperNum.Value = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            numericUpDown3.Value = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            numericUpDown2.Value = trackBar3.Value;
        }

        private void tbx_threshold_ValueChanged(double value)
        {
            findLineTool.threshold = (int)value;
            tkb_exposure.Value = (int)value;
            RefreshInteractivePreview();
        }

        private void tbx_caliperNum_ValueChanged(double value)
        {
            findLineTool.cliperNum = (int)value;
            trackBar1.Value = (int)value;
            RefreshInteractivePreview();
        }

        private void numericUpDown3_ValueChanged(double value)
        {
            findLineTool.Length = (int)value;
            trackBar2.Value = (int)value;

            // 先接管窗口中最新的 ROI；它也是控制器正在拖动的对象。
            findLineTool.SyncDisplayedRoi(false);
            // 直接修改现有 ROI，避免清空重建时发生闪烁并丢失端点编辑模式。
            ROIRectangle2 rectangle = findLineTool.L_regions[0] as ROIRectangle2;
            if (rectangle != null)
                rectangle.SetLength1(value);

            findLineTool.EnableLineRoiEditing();
            Frm_FindLineTool.Instance.regions = findLineTool.L_regions;
            if (!IsBoundJobExecuting())
                findLineTool.ShowDraggingPreview();
            RefreshInteractivePreview();

        }

        private void numericUpDown2_ValueChanged(double value)
        {
            int caliperWidth = (int)value;
            findLineTool.caliperWidth = caliperWidth;
            if (caliperWidth < trackBar3.Minimum)
                trackBar3.Value = trackBar3.Minimum;
            else if (caliperWidth > trackBar3.Maximum)
                trackBar3.Value = trackBar3.Maximum;
            else
                trackBar3.Value = caliperWidth;
            RefreshInteractivePreview();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (cbx_polarity.SelectedIndex == 0)
                cbx_polarity.SelectedIndex = 1;
            else
                cbx_polarity.SelectedIndex = 0;


        }

        private void button5_Click(object sender, EventArgs e)
        {
            switch (cbx_edgeSelect.SelectedIndex)
            {
                case 0:
                    cbx_edgeSelect.SelectedIndex = 1;
                    break;
                case 1:
                    cbx_edgeSelect.SelectedIndex = 2;
                    break;
                case 2:
                    cbx_edgeSelect.SelectedIndex = 0;
                    break;
            }
        }

        private void cbx_edgeSelect_SelectedIndexChanged()
        {
            switch (cbx_edgeSelect.SelectedIndex)
            {
                case 0:
                    findLineTool.edgeSelect = "first";
                    break;
                case 1:
                    findLineTool.edgeSelect = "last";
                    break;
                case 2:
                    findLineTool.edgeSelect = "all";
                    break;
            }
            RefreshInteractivePreview();
        }

        private void cCheckBox1_CheckChanged(bool Checked)
        {
            findLineTool.displayCaliper = ckb_displayCaliper.Checked;
            RefreshInteractivePreview();
        }

        private void cCheckBox2_CheckChanged(bool Checked)
        {
            findLineTool.displayFeature = ckb_displayFeature.Checked;
            RefreshInteractivePreview();
        }

        private void cCheckBox3_CheckChanged(bool Checked)
        {
            findLineTool.displayLine = Checked;
            RefreshInteractivePreview();
        }

        private void textBox2_ValueChanged(double value)
        {
            findLineTool.ignoreNum = (int)value;
            RefreshInteractivePreview();
        }

    }
}
