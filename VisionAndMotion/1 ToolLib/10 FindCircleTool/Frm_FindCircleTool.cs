using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HalconDotNet;
using ViewWindow.Model;

namespace VMPro
{
    /// <summary>
    /// 圆查找编辑器。UI 只负责参数绑定、ROI 交互和结果呈现，
    /// 所有测量都由 FindCircleTool 完成。
    /// </summary>
    internal partial class Frm_FindCircleTool : Frm_FormBase
    {
        private static Frm_FindCircleTool _instance;
        private static FindCircleTool _cachedFindCircleTool = new FindCircleTool(false);
        private readonly Timer previewTimer;
        private bool bindingUi;
        private bool previewPending;

        public Frm_FindCircleTool()
        {
            InitializeComponent();
            previewTimer = new Timer();
            previewTimer.Interval = 180;
            previewTimer.Tick += PreviewTimer_Tick;

            hWindow_Final1.EnableImagePan = false;
            hWindow_Final1.hWindowControl.HMouseUp += HWindow_MouseUp;
            hWindow_Final1.viewWindow.AddRoiObserver(RoiControllerChanged);

            cbx_polarity.SelectedIndexChanged += PolarityChanged;
            comboBox1.SelectedIndexChanged += EdgeSelectChanged;
            tbx_threshold.ValueChanged += ThresholdChanged;
            tbx_cliperNum.ValueChanged += CaliperCountChanged;
            tbx_ringRadiusLength.ValueChanged += SearchLengthChanged;
            textBox1.ValueChanged += CaliperWidthChanged;
            textBox2.ValueChanged += IgnoreCountChanged;
            numericUpDown1.ValueChanged += MinScoreChanged;
            ckb_displayCaliper.CheckedChanged += DisplayChanged;
            ckb_displayFeature.CheckedChanged += DisplayChanged;
            ckb_displayCircle.CheckedChanged += DisplayChanged;
            checkBox1.CheckedChanged += DisplayChanged;
        }

        internal static Frm_FindCircleTool Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed) _instance = new Frm_FindCircleTool();
                return _instance;
            }
        }

        internal static bool IsOpen
        {
            get { return _instance != null && !_instance.IsDisposed && _instance.Visible; }
        }

        internal static FindCircleTool findCircleTool
        {
            get
            {
                FindCircleTool bound = _instance == null || _instance.IsDisposed ? null : _instance.GetBoundTool();
                return bound ?? _cachedFindCircleTool;
            }
            set { _cachedFindCircleTool = value; }
        }

        internal List<ROI> regions = new List<ROI>();

        private FindCircleTool GetBoundTool()
        {
            try
            {
                if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(toolName))
                    return Job.FindToolByName(jobName, toolName) as FindCircleTool;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
            return _cachedFindCircleTool;
        }

        internal void BindTool(FindCircleTool tool, bool enabled)
        {
            if (tool == null) return;
            _cachedFindCircleTool = tool;
            tool.NormalizeParameters();
            regions = tool.L_regions;

            bindingUi = true;
            try
            {
                cbx_polarity.SelectedIndex = tool.polarity == "negative" ? 1 : tool.polarity == "all" ? 2 : 0;
                comboBox1.SelectedIndex = tool.edgeSelect == "first" ? 0 : tool.edgeSelect == "last" ? 1 : 2;
                tbx_threshold.Value = tool.threshold;
                tbx_cliperNum.Value = tool.cliperNum;
                tbx_ringRadiusLength.Value = tool.ringRadiusLength;
                textBox1.Value = tool.caliperWidth;
                textBox2.MaxValue = Math.Max(0, tool.cliperNum - 3);
                textBox2.Value = Math.Min(tool.ignoreNum, tool.cliperNum - 3);
                numericUpDown1.Value = tool.minScore;
                ckb_displayCaliper.Checked = tool.displayCaliper;
                ckb_displayFeature.Checked = tool.displayFeature;
                ckb_displayCircle.Checked = tool.displayCircle;
                checkBox1.Checked = tool.displayCircleCenter;
                lbl_inputStatus.Text = tool.HasValidInput() ? "图像：已连接，可预览" : "图像：未连接或上游未运行";
                lbl_inputStatus.ForeColor = tool.HasValidInput() ? ModernUiTheme.HeaderBlue : ModernUiTheme.Danger;
                lbl_roiSummary.Text = tool.GetRoiSummary();
                lbl_maskSummary.Text = tool.GetMaskSummary();
                btn_runTool.Enabled = enabled;
                btn_preview.Enabled = enabled;
                UpdateResultFromTool(tool);
            }
            finally
            {
                bindingUi = false;
            }

            if (tool.HasValidInput())
            {
                hWindow_Final1.HobjectToHimage(tool.toolPar.InputPar.图像);
                hWindow_Final1.viewWindow.displayInteractiveROI(tool.L_regions);
                QueuePreview();
            }
            else
            {
                hWindow_Final1.ClearWindow();
                UpdateRunStatus("等待有效输入图像", false, 0);
            }
        }

        private void UpdateResultFromTool(FindCircleTool tool)
        {
            bool found = tool.toolPar.ResultPar.是否找到圆;
            Circle circle = tool.toolPar.ResultPar.结果圆;
            UpdateResultDisplay(found ? circle.X.ToString("F3") : "--",
                found ? circle.Y.ToString("F3") : "--",
                found ? circle.R.ToString("F3") : "--", found);
        }

        internal void UpdateResultDisplay(string row, string col, string radius, bool found)
        {
            tbx_resultCircleRow.TextStr = row;
            tbx_resultCircleCol.TextStr = col;
            tbx_resultCircleRadius.TextStr = radius;
            lbl_found.Text = found ? "是否找到：是" : "是否找到：否";
            lbl_found.ForeColor = found ? Color.FromArgb(43, 132, 86) : ModernUiTheme.SecondaryText;
        }

        internal void UpdateRunStatus(string message, bool success, long elapsedMs)
        {
            lbl_toolTip.Text = "状态：" + (message ?? string.Empty);
            lbl_toolTip.ForeColor = success ? Color.FromArgb(43, 132, 86) : ModernUiTheme.Danger;
            lbl_time.Text = elapsedMs > 0 ? string.Format("耗时：{0} ms", elapsedMs) : "耗时：-- ms";
        }

        private void QueuePreview()
        {
            if (bindingUi || Job.loadForm || IsBoundJobExecuting()) return;
            previewPending = true;
            previewTimer.Stop();
            previewTimer.Start();
        }

        private void PreviewTimer_Tick(object sender, EventArgs e)
        {
            if (!previewPending || IsBoundJobExecuting()) return;
            previewPending = false;
            previewTimer.Stop();
            RefreshPreviewNow();
        }

        private void RefreshPreviewNow()
        {
            FindCircleTool tool = GetBoundTool();
            if (tool == null) return;
            bool synced = tool.SyncDisplayedRoi(false);
            tool.ShowContour(true, true, synced);
            lbl_roiSummary.Text = tool.GetRoiSummary();
        }

        private bool IsBoundJobExecuting()
        {
            try
            {
                if (string.IsNullOrEmpty(jobName) || Project.Instance.curEngine == null) return false;
                Job job = Project.Instance.curEngine.FindJobByName(jobName);
                return job != null && job.IsExecutionActive;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }

        private void HWindow_MouseUp(object sender, HMouseEventArgs e)
        {
            FindCircleTool tool = GetBoundTool();
            if (tool == null) return;
            tool.SyncDisplayedRoi(false);
            lbl_roiSummary.Text = tool.GetRoiSummary();
            QueuePreview();
        }

        private void RoiControllerChanged(int eventType)
        {
            if (eventType != ROIController.EVENT_MOVING_ROI || IsBoundJobExecuting()) return;
            FindCircleTool tool = GetBoundTool();
            if (tool != null && tool.SyncDisplayedRoi(false))
            {
                lbl_roiSummary.Text = tool.GetRoiSummary();
                tool.ShowDraggingPreview();
            }
        }

        private void PolarityChanged()
        {
            if (bindingUi) return;
            FindCircleTool tool = GetBoundTool();
            if (tool == null) return;
            tool.polarity = cbx_polarity.SelectedIndex == 1 ? "negative" : cbx_polarity.SelectedIndex == 2 ? "all" : "positive";
            QueuePreview();
        }

        private void EdgeSelectChanged()
        {
            if (bindingUi) return;
            FindCircleTool tool = GetBoundTool();
            if (tool == null) return;
            tool.edgeSelect = comboBox1.SelectedIndex == 0 ? "first" : comboBox1.SelectedIndex == 1 ? "last" : "all";
            QueuePreview();
        }

        private void ThresholdChanged(double value) { SetNumeric(delegate(FindCircleTool t) { t.threshold = (int)value; }); }
        private void SearchLengthChanged(double value) { SetNumeric(delegate(FindCircleTool t) { t.ringRadiusLength = (int)value; }); }
        private void CaliperWidthChanged(double value) { SetNumeric(delegate(FindCircleTool t) { t.caliperWidth = (int)value; }); }
        private void MinScoreChanged(double value) { SetNumeric(delegate(FindCircleTool t) { t.minScore = value; }); }

        private void CaliperCountChanged(double value)
        {
            SetNumeric(delegate(FindCircleTool t)
            {
                t.cliperNum = (int)value;
                textBox2.MaxValue = Math.Max(0, t.cliperNum - 3);
                if (t.ignoreNum > t.cliperNum - 3)
                {
                    t.ignoreNum = Math.Max(0, t.cliperNum - 3);
                    textBox2.Value = t.ignoreNum;
                }
            });
        }

        private void IgnoreCountChanged(double value) { SetNumeric(delegate(FindCircleTool t) { t.ignoreNum = (int)value; }); }

        private void SetNumeric(Action<FindCircleTool> setter)
        {
            if (bindingUi) return;
            FindCircleTool tool = GetBoundTool();
            if (tool == null) return;
            setter(tool);
            tool.NormalizeParameters();
            QueuePreview();
        }

        private void DisplayChanged(object sender, EventArgs e)
        {
            if (bindingUi) return;
            FindCircleTool tool = GetBoundTool();
            if (tool == null) return;
            tool.displayCaliper = ckb_displayCaliper.Checked;
            tool.displayFeature = ckb_displayFeature.Checked;
            tool.displayCircle = ckb_displayCircle.Checked;
            tool.displayCircleCenter = checkBox1.Checked;
            QueuePreview();
        }

        private void btn_preview_Click(object sender, EventArgs e)
        {
            previewTimer.Stop();
            previewPending = false;
            RefreshPreviewNow();
        }

        internal void btn_runFindCircleTool_Click(object sender, EventArgs e) { btn_runTool_Click(sender, e); }

        private void btn_runTool_Click(object sender, EventArgs e)
        {
            FindCircleTool tool = GetBoundTool();
            if (tool == null || IsBoundJobExecuting()) return;
            btn_runTool.Enabled = false;
            try
            {
                tool.SyncDisplayedRoi(false);
                tool.Run(true, true, toolName);
                UpdateResultFromTool(tool);
                bool success = tool.toolRunStatu == (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                Frm_Main.Instance.OutputMsg(tool.toolRunStatu.ToString(), success ? Color.Green : Color.Red);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                UpdateRunStatus("运行异常：" + ex.Message, false, 0);
            }
            finally { btn_runTool.Enabled = true; }
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(toolName) || IsBoundJobExecuting()) return;
            try
            {
                Job.RunAndWaitToCurrentTool(jobName, toolName);
                FindCircleTool tool = GetBoundTool();
                if (tool != null) UpdateResultFromTool(tool);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                UpdateRunStatus("流程运行异常：" + ex.Message, false, 0);
            }
        }

        private void btn_editRoi_Click(object sender, EventArgs e)
        {
            FindCircleTool tool = GetBoundTool();
            if (tool == null || !tool.HasValidInput())
            {
                UpdateRunStatus("请先运行上游图像工具", false, 0);
                return;
            }
            tool.DrawExpectCircle(jobName);
            UpdateRunStatus("已进入 ROI 编辑", true, 0);
        }

        private void btn_resetRoi_Click(object sender, EventArgs e)
        {
            FindCircleTool tool = GetBoundTool();
            if (tool == null) return;
            tool.ResetRoiToImage();
            regions = tool.L_regions;
            lbl_roiSummary.Text = tool.GetRoiSummary();
            if (tool.HasValidInput())
            {
                hWindow_Final1.HobjectToHimage(tool.toolPar.InputPar.图像);
                hWindow_Final1.viewWindow.displayInteractiveROI(tool.L_regions);
                QueuePreview();
            }
        }

        private void btn_clearMask_Click(object sender, EventArgs e)
        {
            FindCircleTool tool = GetBoundTool();
            if (tool == null) return;
            tool.ClearMask();
            lbl_maskSummary.Text = tool.GetMaskSummary();
            QueuePreview();
        }

        private void btn_cancel_Click(object sender, EventArgs e) { Close(); }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            previewTimer.Stop();
            base.OnFormClosed(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!Visible)
            {
                previewTimer.Stop();
                previewPending = false;
            }
        }
    }
}
