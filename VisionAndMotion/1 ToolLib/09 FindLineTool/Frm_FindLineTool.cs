using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HalconDotNet;
using ViewWindow.Model;

namespace VMPro
{
    /// <summary>查找边（直线）编辑器：只负责参数、ROI 和结果呈现。</summary>
    internal partial class Frm_FindLineTool : Frm_FormBase
    {
        private static Frm_FindLineTool _instance;
        internal static FindLineTool findLineTool;
        private readonly Timer previewTimer;
        private bool bindingUi;
        private bool previewPending;

        internal Frm_FindLineTool()
        {
            InitializeComponent();
            previewTimer = new Timer();
            previewTimer.Interval = 180;
            previewTimer.Tick += PreviewTimer_Tick;

            hWindow_Final1.EnableImagePan = false;
            hWindow_Final1.hWindowControl.HMouseUp += HWindow_MouseUp;
            hWindow_Final1.viewWindow.AddRoiObserver(RoiControllerChanged);
            cbx_polarity.SelectedIndexChanged += PolarityChanged;
            cbx_edgeSelect.SelectedIndexChanged += EdgeSelectChanged;
            tbx_threshold.ValueChanged += ThresholdChanged;
            tbx_caliperNum.ValueChanged += CaliperCountChanged;
            numericUpDown3.ValueChanged += SearchLengthChanged;
            numericUpDown2.ValueChanged += CaliperWidthChanged;
            textBox2.ValueChanged += IgnoreCountChanged;
            numericUpDown1.ValueChanged += MinScoreChanged;
            ckb_displayCaliper.CheckedChanged += DisplayChanged;
            ckb_displayFeature.CheckedChanged += DisplayChanged;
            cCheckBox3.CheckedChanged += DisplayChanged;
        }

        internal static Frm_FindLineTool Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed) _instance = new Frm_FindLineTool();
                return _instance;
            }
        }

        internal static bool IsOpen
        {
            get { return _instance != null && !_instance.IsDisposed && _instance.Visible; }
        }

        internal List<ROI> regions = new List<ROI>();

        internal void BindTool(FindLineTool tool, bool enabled)
        {
            if (tool == null) return;
            findLineTool = tool;
            tool.NormalizeParameters();
            regions = tool.L_regions;

            bindingUi = true;
            try
            {
                cbx_polarity.SelectedIndex = tool.polarity == "negative" ? 1 : tool.polarity == "all" ? 2 : 0;
                cbx_edgeSelect.SelectedIndex = tool.edgeSelect == "first" ? 0 : tool.edgeSelect == "last" ? 1 : 2;
                tbx_threshold.Value = tool.threshold;
                tbx_caliperNum.Value = tool.cliperNum;
                numericUpDown3.Value = tool.Length;
                numericUpDown2.Value = tool.caliperWidth;
                textBox2.MaxValue = Math.Max(0, tool.cliperNum - 2);
                textBox2.Value = Math.Min(tool.ignoreNum, tool.cliperNum - 2);
                numericUpDown1.Value = tool.minScore;
                ckb_displayCaliper.Checked = tool.displayCaliper;
                ckb_displayFeature.Checked = tool.displayFeature;
                cCheckBox3.Checked = tool.displayLine;
                lbl_inputStatus.Text = tool.HasValidInput() ? "图像：已连接，可预览" : "图像：未连接或上游未运行";
                lbl_inputStatus.ForeColor = tool.HasValidInput() ? ModernUiTheme.HeaderBlue : ModernUiTheme.Danger;
                lbl_roiSummary.Text = tool.GetRoiSummary();
                btn_runTool.Enabled = enabled;
                btn_preview.Enabled = enabled;
                UpdateResultFromTool(tool);
            }
            finally { bindingUi = false; }

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

        private void UpdateResultFromTool(FindLineTool tool)
        {
            bool found = tool.toolPar != null && tool.toolPar.ResultPar != null && tool.toolPar.ResultPar.线 != null &&
                tool.toolRunStatu == (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
            UpdateResultDisplay(found ? tool.ResultLineStartRow.ToString() : "--",
                found ? tool.ResultLineStartCol.ToString() : "--",
                found ? tool.ResultLineEndRow.ToString() : "--",
                found ? tool.ResultLineEndCol.ToString() : "--",
                found ? tool.Angle.ToString() : "--", found);
        }

        internal void UpdateResultDisplay(string startRow, string startCol, string endRow, string endCol, string angle, bool found)
        {
            tbx_resultStartRow.Text = startRow;
            tbx_resultStartCol.Text = startCol;
            tbx_resultEndRow.Text = endRow;
            tbx_resultEndCol.Text = endCol;
            lbl_angle.Text = "方向角：" + angle + (angle == "--" ? string.Empty : " rad");
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
            if (findLineTool == null) return;
            bool synced = findLineTool.SyncDisplayedRoi(false);
            findLineTool.ShowContour(true, true, synced);
            lbl_roiSummary.Text = findLineTool.GetRoiSummary();
        }

        private bool IsBoundJobExecuting()
        {
            try
            {
                if (string.IsNullOrEmpty(jobName) || Project.Instance.curEngine == null) return false;
                Job job = Project.Instance.curEngine.FindJobByName(jobName);
                return job != null && job.IsExecutionActive;
            }
            catch (Exception ex) { Log.SaveError(ex); return false; }
        }

        private void HWindow_MouseUp(object sender, HMouseEventArgs e)
        {
            if (findLineTool == null) return;
            findLineTool.SyncDisplayedRoi(false);
            lbl_roiSummary.Text = findLineTool.GetRoiSummary();
            QueuePreview();
        }

        private void RoiControllerChanged(int eventType)
        {
            try
            {
                if (eventType != ROIController.EVENT_MOVING_ROI || findLineTool == null || IsBoundJobExecuting()) return;
                if (findLineTool.SyncDisplayedRoi(false))
                {
                    lbl_roiSummary.Text = findLineTool.GetRoiSummary();
                    findLineTool.ShowDraggingPreview();
                }
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Error updating ROI" 
                    : "ROI 更新出错", "RoiControllerChanged");
            }
        }

        private void PolarityChanged()
        {
            try
            {
                if (bindingUi || findLineTool == null) return;
                findLineTool.polarity = cbx_polarity.SelectedIndex == 1 ? "negative" : cbx_polarity.SelectedIndex == 2 ? "all" : "positive";
                QueuePreview();
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Error changing polarity" 
                    : "极性修改出错", "PolarityChanged");
            }
        }

        private void EdgeSelectChanged()
        {
            try
            {
                if (bindingUi || findLineTool == null) return;
                findLineTool.edgeSelect = cbx_edgeSelect.SelectedIndex == 0 ? "first" : cbx_edgeSelect.SelectedIndex == 1 ? "last" : "all";
                QueuePreview();
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Error changing edge selection" 
                    : "边缘选择出错", "EdgeSelectChanged");
            }
        }

        private void ThresholdChanged(double value) { SetNumeric(delegate { findLineTool.threshold = (int)value; }); }
        private void CaliperWidthChanged(double value) { SetNumeric(delegate { findLineTool.caliperWidth = (int)value; }); }
        private void MinScoreChanged(double value) { SetNumeric(delegate { findLineTool.minScore = value; }); }

        private void CaliperCountChanged(double value)
        {
            SetNumeric(delegate
            {
                findLineTool.cliperNum = (int)value;
                textBox2.MaxValue = Math.Max(0, findLineTool.cliperNum - 2);
                if (findLineTool.ignoreNum > findLineTool.cliperNum - 2)
                {
                    findLineTool.ignoreNum = Math.Max(0, findLineTool.cliperNum - 2);
                    textBox2.Value = findLineTool.ignoreNum;
                }
            });
        }

        private void SearchLengthChanged(double value)
        {
            SetNumeric(delegate
            {
                findLineTool.SyncDisplayedRoi(false);
                findLineTool.Length = (int)value;
                ROIRectangle2 roi = findLineTool.L_regions.Count > 0 ? findLineTool.L_regions[0] as ROIRectangle2 : null;
                if (roi != null) roi.SetLength1(value);
                findLineTool.EnableLineRoiEditing();
            });
        }

        private void IgnoreCountChanged(double value) { SetNumeric(delegate { findLineTool.ignoreNum = (int)value; }); }

        private void SetNumeric(Action setter)
        {
            if (bindingUi || findLineTool == null) return;
            setter();
            findLineTool.NormalizeParameters();
            QueuePreview();
        }

        private void DisplayChanged(object sender, EventArgs e)
        {
            try
            {
                if (bindingUi || findLineTool == null) return;
                findLineTool.displayCaliper = ckb_displayCaliper.Checked;
                findLineTool.displayFeature = ckb_displayFeature.Checked;
                findLineTool.displayLine = cCheckBox3.Checked;
                QueuePreview();
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Error updating display settings" 
                    : "显示设置更新出错", "DisplayChanged");
            }
        }

        private void btn_preview_Click(object sender, EventArgs e)
        {
            try
            {
                previewTimer.Stop();
                previewPending = false;
                RefreshPreviewNow();
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Preview failed" 
                    : "预览失败", "btn_preview_Click");
            }
        }

        private void btn_runTool_Click(object sender, EventArgs e)
        {
            if (findLineTool == null || IsBoundJobExecuting()) return;
            btn_runTool.Enabled = false;
            try
            {
                findLineTool.SyncDisplayedRoi(false);
                findLineTool.Run(true, true, toolName);
                UpdateResultFromTool(findLineTool);
                bool success = findLineTool.toolRunStatu == (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                Frm_Main.Instance.OutputMsg(findLineTool.toolRunStatu.ToString(), success ? Color.Green : Color.Red);
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Tool execution failed: " + ex.Message 
                    : "工具运行异常：" + ex.Message, "btn_runTool_Click");
                UpdateRunStatus(Project.Instance.configuration.language == Language.English 
                    ? "Execution error: " + ex.Message 
                    : "运行异常：" + ex.Message, false, 0);
            }
            finally { btn_runTool.Enabled = true; }
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(toolName) || IsBoundJobExecuting()) return;
            try
            {
                Job.RunAndWaitToCurrentTool(jobName, toolName);
                if (findLineTool != null) UpdateResultFromTool(findLineTool);
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Job execution failed: " + ex.Message 
                    : "流程运行异常：" + ex.Message, "btn_confirm_Click");
                UpdateRunStatus(Project.Instance.configuration.language == Language.English 
                    ? "Job error: " + ex.Message 
                    : "流程运行异常：" + ex.Message, false, 0);
            }
        }

        private void btn_editRoi_Click(object sender, EventArgs e)
        {
            try
            {
                if (findLineTool == null || !findLineTool.HasValidInput())
                {
                    UpdateRunStatus(Project.Instance.configuration.language == Language.English 
                        ? "Please run upstream image tool first" 
                        : "请先运行上游图像工具", false, 0);
                    return;
                }
                findLineTool.EditCaliper();
                UpdateRunStatus(Project.Instance.configuration.language == Language.English 
                    ? "Entered ROI editing mode" 
                    : "已进入 ROI 编辑", true, 0);
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Failed to edit caliper" 
                    : "卡尺编辑失败", "btn_editRoi_Click");
            }
        }

        private void btn_resetRoi_Click(object sender, EventArgs e)
        {
            try
            {
                if (findLineTool == null) return;
                findLineTool.ResetRoiToImage();
                regions = findLineTool.L_regions;
                lbl_roiSummary.Text = findLineTool.GetRoiSummary();
                if (findLineTool.HasValidInput())
                {
                    hWindow_Final1.HobjectToHimage(findLineTool.toolPar.InputPar.图像);
                    hWindow_Final1.viewWindow.displayInteractiveROI(findLineTool.L_regions);
                    QueuePreview();
                }
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Failed to reset ROI" 
                    : "重置 ROI 失败", "btn_resetRoi_Click");
            }
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
