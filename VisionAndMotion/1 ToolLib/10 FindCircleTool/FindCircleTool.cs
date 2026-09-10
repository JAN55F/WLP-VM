using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using HalconDotNet;
using ViewWindow.Model;

namespace VMPro
{
    /// <summary>
    /// HALCON Metrology 圆查找。预览和正式运行共用同一测量路径：
    /// 输入图像 -> 圆 ROI -> 可选位姿跟随 -> 卡尺找边 -> 离群点剔除 -> 圆拟合。
    /// 兼容约定：XY.X 是 HALCON Row，XY.Y 是 Column。
    /// </summary>
    [Serializable]
    internal class FindCircleTool : ToolBase
    {
        internal FindCircleTool() : this(true) { }

        internal FindCircleTool(bool useCurrentWorkflowImage)
        {
            double centerRow = 400.0;
            double centerCol = 500.0;
            double radius = 160.0;
            HObject image = null;
            if (useCurrentWorkflowImage)
            {
                try
                {
                    if (Frm_Job.Instance.tbc_jobs.SelectedTab != null)
                    {
                        Job job = Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text);
                        if (job != null)
                        {
                            for (int i = 0; i < job.L_toolList.Count; i++)
                            {
                                if (job.L_toolList[i].toolType == ToolType.ImageAcq)
                                {
                                    image = ((AcqImageTool)job.L_toolList[i].tool).toolPar.ResultPar.图像;
                                    break;
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

            HTuple width, height;
            if (TryGetHalconImageSize(image, out width, out height))
            {
                centerRow = height.D / 2.0;
                centerCol = width.D / 2.0;
                radius = Math.Max(30.0, Math.Min(width.D, height.D) / 6.0);
                ringRadiusLength = Math.Max(5, (int)(radius / 4.0));
            }
            L_regions.Add(new ROICircle(centerRow, centerCol, radius));
            expectCircleRow = centerRow;
            expectCircleCol = centerCol;
            expectCircleRadius = radius;
            templatePose.Add(new XYU());
            brush_region = new HObject();
            final_region = new HObject();
            contours = new HObject();
        }

        // 旧工程兼容字段。新 UI 不再进入阻塞式画笔循环。
        public bool drawMode = false;
        internal HObject brush_region;
        internal HObject final_region;
        internal List<ROI> L_regions = new List<ROI>();
        internal bool displayCaliper = true;
        internal bool displayFeature = true;
        internal bool displayCircle = true;
        internal bool displayCircleCenter = true;
        internal HTuple expectCircleRow = 300;
        internal HTuple expectCircleCol = 300;
        internal HTuple expectCircleRadius = 200;
        internal double startAngle = 10;
        internal double endAngle = 360;
        internal bool updateImage = false;
        internal int ringRadiusLength = 30;
        internal int caliperWidth = 5;
        internal int threshold = 30;
        internal List<XY> circleCenter = new List<XY>();
        internal int ignoreNum = 0;
        internal HObject contours;
        internal string polarity = "positive";
        internal string edgeSelect = "all";
        internal double minScore = 0.5;
        internal int cliperNum = 30;
        internal List<HTuple> newExpecCircleRow = new List<HTuple>();
        internal List<HTuple> newExpectCircleCol = new List<HTuple>();
        internal List<HTuple> newExpectCircleRadius = new List<HTuple>();
        internal List<XYU> templatePose = new List<XYU>();
        internal double LastExpectRow;
        internal double LastExpectCol;

        internal void NormalizeParameters()
        {
            cliperNum = Math.Max(4, Math.Min(720, cliperNum));
            ringRadiusLength = Math.Max(1, Math.Min(2000, ringRadiusLength));
            caliperWidth = Math.Max(1, Math.Min(1000, caliperWidth));
            threshold = Math.Max(1, Math.Min(255, threshold));
            ignoreNum = Math.Max(0, Math.Min(cliperNum - 3, ignoreNum));
            minScore = Math.Max(0.01, Math.Min(1.0, minScore));
            if (polarity != "positive" && polarity != "negative" && polarity != "all") polarity = "positive";
            if (edgeSelect != "first" && edgeSelect != "last" && edgeSelect != "all") edgeSelect = "all";
        }

        internal bool HasValidInput()
        {
            HTuple width, height;
            return TryGetHalconImageSize(toolPar.InputPar.图像, out width, out height);
        }

        internal string GetRoiSummary()
        {
            ROICircle roi = GetCircleRoi();
            return roi == null ? "未设置搜索圆" : string.Format(
                "圆心 Row {0:F1}  Col {1:F1}\r\n预期半径 {2:F1} px", roi.Row, roi.Column, roi.Radius);
        }

        internal string GetMaskSummary()
        {
            return HasMask() ? "已保留历史屏蔽区（可一键清除）" : "无屏蔽区";
        }

        internal void ClearMask()
        {
            drawMode = false;
            if (final_region != null) final_region.Dispose();
            final_region = new HObject();
        }

        internal void ResetRoiToImage()
        {
            double row = 400.0, col = 500.0, radius = 160.0;
            HTuple width, height;
            if (TryGetHalconImageSize(toolPar.InputPar.图像, out width, out height))
            {
                row = height.D / 2.0;
                col = width.D / 2.0;
                radius = Math.Max(30.0, Math.Min(width.D, height.D) / 6.0);
            }
            L_regions.Clear();
            L_regions.Add(new ROICircle(row, col, radius));
            expectCircleRow = row;
            expectCircleCol = col;
            expectCircleRadius = radius;
            CaptureTemplatePoseFromCurrentInput();
        }

        internal void CaptureTemplatePoseFromCurrentInput()
        {
            if (toolPar == null || toolPar.InputPar == null || toolPar.InputPar.跟随 == null || toolPar.InputPar.跟随.Count == 0) return;
            XYU input = toolPar.InputPar.跟随[0];
            XYU pose = new XYU();
            pose.Point.X = input.Point.X;
            pose.Point.Y = input.Point.Y;
            pose.U = input.U;
            templatePose.Clear();
            templatePose.Add(pose);
        }

        internal void EnsureTemplatePoseFromCurrentInput()
        {
            bool learned = templatePose != null && templatePose.Count > 0 &&
                (Math.Abs(templatePose[0].Point.X) > 0.000001 || Math.Abs(templatePose[0].Point.Y) > 0.000001 || Math.Abs(templatePose[0].U) > 0.000001);
            if (!learned) CaptureTemplatePoseFromCurrentInput();
        }

        internal void RebaseRoiToCurrentFollowPose()
        {
            ROICircle roi = GetCircleRoi();
            if (roi == null || templatePose == null || templatePose.Count == 0 || toolPar.InputPar.跟随 == null || toolPar.InputPar.跟随.Count == 0) return;
            XYU source = templatePose[0];
            XYU target = toolPar.InputPar.跟随[0];
            bool changed = Math.Abs(source.Point.X - target.Point.X) > 0.000001 || Math.Abs(source.Point.Y - target.Point.Y) > 0.000001 || Math.Abs(source.U - target.U) > 0.000001;
            if (!changed) return;
            HTuple homMat2D, row, col;
            HOperatorSet.VectorAngleToRigid(source.Point.X, source.Point.Y, source.U, target.Point.X, target.Point.Y, target.U, out homMat2D);
            HOperatorSet.AffineTransPixel(homMat2D, roi.Row, roi.Column, out row, out col);
            roi.createCircle(row.D, col.D, roi.Radius);
            CaptureTemplatePoseFromCurrentInput();
        }

        private void RepairROIHandlesFromOldProject()
        {
            if (L_regions?.Count > 0 && L_regions[0] is ROICircle circle)
            {
                circle.RepairHandles(2, 1);
            }
        }

        internal void UpdateImage(string ignoredJobName) { ShowImage(toolPar.InputPar.图像); }

        public void DrawExpectCircle(string ignoredJobName)
        {
            try
            {
                RepairROIHandlesFromOldProject();
                if (!Frm_FindCircleTool.IsOpen || !HasValidInput()) return;
                if (L_regions == null || L_regions.Count == 0) ResetRoiToImage();
                CaptureTemplatePoseFromCurrentInput();
                Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                Frm_FindCircleTool.Instance.regions = L_regions;
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Failed to draw expected circle" 
                    : "绘制期望圆失败", "DrawExpectCircle");
            }
        }

        internal bool SyncDisplayedRoi(bool refreshPreview)
        {
            if (!Frm_FindCircleTool.IsOpen || L_regions == null || L_regions.Count == 0) return false;
            int index;
            List<double> data;
            ROI roi = Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.smallestActiveROI(out data, out index);
            if (!(roi is ROICircle) || index < 0) return false;
            L_regions[0] = roi;
            ROICircle circle = (ROICircle)roi;
            expectCircleRow = circle.Row;
            expectCircleCol = circle.Column;
            expectCircleRadius = circle.Radius;
            CaptureTemplatePoseFromCurrentInput();
            Frm_FindCircleTool.Instance.regions = L_regions;
            if (refreshPreview) ShowContour(true, true, true);
            return true;
        }

        internal void ShowDraggingPreview()
        {
            if (!Frm_FindCircleTool.IsOpen || !HasValidInput() || GetCircleRoi() == null) return;
            try
            {
                BuildExpectedCircles(true);
                ChoiceTech.Halcon.Control.HWindow_Final window = Frm_FindCircleTool.Instance.hWindow_Final1;
                window.viewWindow._hWndControl.clearHObjectList();
                if (displayCircle && newExpecCircleRow.Count > 0)
                {
                    HObject expected = null;
                    try
                    {
                        HOperatorSet.GenCircleContourXld(out expected, newExpecCircleRow[0], newExpectCircleCol[0], newExpectCircleRadius[0], 0, Math.PI * 2.0, "positive", 1.5);
                        window.DispObj(expected, "green");
                    }
                    finally { if (expected != null) expected.Dispose(); }
                }
                window.viewWindow._hWndControl.repaint();
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Failed to show preview" 
                    : "预览显示失败", "ShowDraggingPreview");
            }
        }

        internal void ShowContour(bool showROI, bool trans = true) { ShowContour(showROI, trans, false); }

        internal void ShowContour(bool showROI, bool trans, bool preserveInteractiveRoi)
        {
            if (!Frm_FindCircleTool.IsOpen) return;
            lock (obj)
            {
                try
                {
                    NormalizeParameters();
                    HTuple width, height;
                    if (!TryGetHalconImageSize(toolPar.InputPar.图像, out width, out height))
                    {
                        toolRunStatu = FailureImageStatus();
                        Frm_FindCircleTool.Instance.UpdateRunStatus(toolRunStatu.ToString(), false, 0);
                        return;
                    }
                    if (GetCircleRoi() == null)
                    {
                        toolRunStatu = ToolRunStatu.缺少输入搜索区域;
                        Frm_FindCircleTool.Instance.UpdateRunStatus(toolRunStatu.ToString(), false, 0);
                        return;
                    }
                    BuildExpectedCircles(trans);
                    if (preserveInteractiveRoi) Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow._hWndControl.clearHObjectList();
                    else
                    {
                        Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                        if (showROI) Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                    }
                    HObject measurementImage = null;
                    bool ownsImage = false;
                    try
                    {
                        measurementImage = CreateMeasurementImage(toolPar.InputPar.图像, out ownsImage);
                        bool found = false;
                        for (int i = 0; i < newExpecCircleRow.Count; i++)
                        {
                            CircleMeasureResult result = MeasureCircle(measurementImage, width, height, newExpecCircleRow[i], newExpectCircleCol[i], newExpectCircleRadius[i]);
                            try
                            {
                                found = found || result.Found;
                                DisplayMeasureResult(result, true);
                            }
                            finally { result.Dispose(); }
                        }
                        Frm_FindCircleTool.Instance.UpdateRunStatus(found ? "预览完成：已找到圆" : "预览完成：未找到圆", found, 0);
                    }
                    finally { if (ownsImage && measurementImage != null) measurementImage.Dispose(); }
                    if (HasMask()) Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(final_region, "#8f62c7");
                    Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow._hWndControl.repaint();
                }
                catch (Exception ex)
                {
                    toolRunStatu = UnknownFailureStatus();
                    Log.SaveError(ex);
                    Frm_FindCircleTool.Instance.UpdateRunStatus("预览失败：" + ex.Message, false, 0);
                }
            }
        }

        public override void Run(bool refreshImage, bool runTool, string currentToolName)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            lock (obj)
            {
                try
                {
                    NormalizeParameters();
                    ResetResults();
                    toolRunStatu = UnknownFailureStatus();
                    HTuple width, height;
                    if (!TryGetHalconImageSize(toolPar.InputPar.图像, out width, out height)) { toolRunStatu = FailureImageStatus(); return; }
                    if (GetCircleRoi() == null) { toolRunStatu = ToolRunStatu.缺少输入搜索区域; return; }
                    if (refreshImage)
                    {
                        if (runTool && Frm_FindCircleTool.IsOpen)
                        {
                            Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                            Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                        }
                        else
                        {
                            ShowImage(toolPar.InputPar.图像);
                            SafeInvokeFindCircleWindow(delegate
                            {
                                Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                                Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                            });
                        }
                    }
                    BuildExpectedCircles(true);
                    HObject measurementImage = null;
                    bool ownsImage = false;
                    try
                    {
                        measurementImage = CreateMeasurementImage(toolPar.InputPar.图像, out ownsImage);
                        for (int i = 0; i < newExpecCircleRow.Count; i++)
                        {
                            CircleMeasureResult result = MeasureCircle(measurementImage, width, height, newExpecCircleRow[i], newExpectCircleCol[i], newExpectCircleRadius[i]);
                            try
                            {
                                if (!result.Found) continue;
                                XY center = new XY(result.Row, result.Column);
                                circleCenter.Add(center);
                                toolPar.ResultPar.圆心列表.Add(center);
                                if (!toolPar.ResultPar.是否找到圆)
                                {
                                    toolPar.ResultPar.是否找到圆 = true;
                                    toolPar.ResultPar.结果圆 = new Circle { X = result.Row, Y = result.Column, R = result.Radius };
                                    toolPar.ResultPar.圆半径 = result.Radius;
                                }
                                DisplayMeasureResult(result, runTool);
                            }
                            finally { result.Dispose(); }
                        }
                    }
                    finally { if (ownsImage && measurementImage != null) measurementImage.Dispose(); }
                    if (!toolPar.ResultPar.是否找到圆)
                    {
                        toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Circle_Found : ToolRunStatu.未找到圆;
                        return;
                    }
                    toolRunStatu = SuccessStatus();
                }
                catch (Exception ex)
                {
                    ResetResults();
                    toolRunStatu = UnknownFailureStatus();
                    Log.SaveError(ex);
                    SafeInvokeFindCircleWindow(delegate { Frm_FindCircleTool.Instance.UpdateRunStatus("运行异常：" + ex.Message, false, stopwatch.ElapsedMilliseconds); });
                }
                finally
                {
                    stopwatch.Stop();
                    UpdateResultDisplay(runTool);
                    SafeInvokeFindCircleWindow(delegate { Frm_FindCircleTool.Instance.UpdateRunStatus(toolRunStatu.ToString(), toolRunStatu == SuccessStatus(), stopwatch.ElapsedMilliseconds); });
                }
            }
        }

        public override ToolRunResult Execute(ToolRunContext context)
        {
            if (context != null && context.IsCancellationRequested != null && context.IsCancellationRequested())
            {
                toolRunStatu = ToolRunStatu.用户取消;
                return CreateRunResult(0);
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            Run(true, false, context == null ? string.Empty : context.ToolName);
            stopwatch.Stop();
            if (context != null && context.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > context.TimeoutMs && toolRunStatu != SuccessStatus()) toolRunStatu = ToolRunStatu.运行超时;
            return CreateRunResult(stopwatch.ElapsedMilliseconds);
        }

        private ToolRunResult CreateRunResult(long elapsedMs)
        {
            return new ToolRunResult { Success = toolRunStatu == SuccessStatus(), Canceled = toolRunStatu == ToolRunStatu.用户取消, Timeout = toolRunStatu == ToolRunStatu.运行超时, Status = toolRunStatu, Message = toolRunStatu.ToString(), ElapsedMs = elapsedMs };
        }

        private CircleMeasureResult MeasureCircle(HObject image, HTuple width, HTuple height, HTuple expectedRow, HTuple expectedCol, HTuple expectedRadius)
        {
            CircleMeasureResult result = new CircleMeasureResult();
            HTuple handle = null;
            try
            {
                HOperatorSet.CreateMetrologyModel(out handle);
                HOperatorSet.SetMetrologyModelImageSize(handle, width[0], height[0]);
                HTuple index;
                HOperatorSet.AddMetrologyObjectCircleMeasure(handle, expectedRow, expectedCol, expectedRadius, ringRadiusLength, caliperWidth, 1, threshold, new HTuple(), new HTuple(), out index);
                ApplyMetrologyParameters(handle);
                HOperatorSet.ApplyMetrologyModel(image, handle);
                HTuple rows, columns;
                HOperatorSet.GetMetrologyObjectMeasures(out result.MeasureContours, handle, "all", "all", out rows, out columns);
                result.MeasureRows = rows;
                result.MeasureColumns = columns;
                if (rows == null || columns == null || rows.TupleLength() < 3 || columns.TupleLength() < 3) return result;
                HTuple parameters;
                HOperatorSet.GetMetrologyObjectResult(handle, "all", "all", "result_type", "all_param", out parameters);
                if (parameters == null || parameters.TupleLength() < 3) return result;
                HOperatorSet.GetMetrologyObjectResultContour(out result.ResultContour, handle, "all", "all", 1.5);
                double row = parameters[0].D, column = parameters[1].D, radius = parameters[2].D;
                if (ignoreNum > 0 && rows.TupleLength() - ignoreNum >= 3)
                {
                    HTuple keptRows, keptColumns, rejectedRows, rejectedColumns;
                    SelectInlierPoints(result.ResultContour, rows, columns, ignoreNum, out keptRows, out keptColumns, out rejectedRows, out rejectedColumns);
                    double fittedRow, fittedColumn, fittedRadius;
                    if (TryFitCircle(keptRows, keptColumns, out fittedRow, out fittedColumn, out fittedRadius))
                    {
                        row = fittedRow; column = fittedColumn; radius = fittedRadius;
                        result.MeasureRows = keptRows; result.MeasureColumns = keptColumns;
                        result.RejectedRows = rejectedRows; result.RejectedColumns = rejectedColumns;
                        if (result.ResultContour != null) result.ResultContour.Dispose();
                        HOperatorSet.GenCircleContourXld(out result.ResultContour, row, column, radius, 0, Math.PI * 2.0, "positive", 1.5);
                    }
                }
                if (double.IsNaN(row) || double.IsInfinity(row) || double.IsNaN(column) || double.IsInfinity(column) || double.IsNaN(radius) || double.IsInfinity(radius) || radius <= 0) return result;
                result.Row = row; result.Column = column; result.Radius = radius; result.Found = true;
                return result;
            }
            finally
            {
                if (handle != null && handle.TupleLength() > 0)
                {
                    try { HOperatorSet.ClearMetrologyModel(handle); }
                    catch (Exception ex) { Log.SaveError(ex); }
                }
            }
        }

        private void ApplyMetrologyParameters(HTuple handle)
        {
            HOperatorSet.SetMetrologyObjectParam(handle, "all", "measure_transition", polarity);
            HOperatorSet.SetMetrologyObjectParam(handle, "all", "measure_select", edgeSelect);
            HOperatorSet.SetMetrologyObjectParam(handle, "all", "num_measures", cliperNum);
            HOperatorSet.SetMetrologyObjectParam(handle, "all", "measure_length1", ringRadiusLength);
            HOperatorSet.SetMetrologyObjectParam(handle, "all", "measure_length2", caliperWidth);
            HOperatorSet.SetMetrologyObjectParam(handle, "all", "measure_threshold", threshold);
            HOperatorSet.SetMetrologyObjectParam(handle, "all", "min_score", minScore);
        }

        private void DisplayMeasureResult(CircleMeasureResult result, bool runTool)
        {
            if (result == null) return;
            if (displayCaliper && IsValidObject(result.MeasureContours)) DisplayObject(result.MeasureContours, "#4c94d2", runTool);
            if (displayFeature && result.MeasureRows != null && result.MeasureRows.TupleLength() > 0)
            {
                HObject points = null;
                try { HOperatorSet.GenCrossContourXld(out points, result.MeasureRows, result.MeasureColumns, 12, 0); DisplayObject(points, "#f0a340", runTool); }
                finally { if (points != null) points.Dispose(); }
            }
            if (displayFeature && result.RejectedRows != null && result.RejectedRows.TupleLength() > 0)
            {
                HObject rejected = null;
                try { HOperatorSet.GenCrossContourXld(out rejected, result.RejectedRows, result.RejectedColumns, 14, 0); DisplayObject(rejected, "red", runTool); }
                finally { if (rejected != null) rejected.Dispose(); }
            }
            if (result.Found && displayCircle && IsValidObject(result.ResultContour)) DisplayObject(result.ResultContour, "green", runTool);
            if (result.Found && displayCircleCenter)
            {
                HObject center = null;
                try { HOperatorSet.GenCrossContourXld(out center, result.Row, result.Column, 20, 0); DisplayObject(center, "green", runTool); }
                finally { if (center != null) center.Dispose(); }
            }
        }

        private void DisplayObject(HObject value, string color, bool runTool)
        {
            if (!IsValidObject(value)) return;
            if (runTool && Frm_FindCircleTool.IsOpen) { Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(value, color); return; }
            ShowObj(value, color);
            // ShowObj 已经制作 HALCON 快照。窗体展示也必须在释放源对象前复制。
            if (Frm_FindCircleTool.IsOpen)
            {
                HObject snapshot = new HObject(value);
                bool queued = TryPostControlAction(Frm_FindCircleTool.Instance, delegate
                {
                    try { Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(snapshot, color); }
                    finally { snapshot.Dispose(); }
                });
                if (!queued) snapshot.Dispose();
            }
        }

        private void BuildExpectedCircles(bool transform)
        {
            newExpecCircleRow.Clear(); newExpectCircleCol.Clear(); newExpectCircleRadius.Clear();
            ROICircle roi = GetCircleRoi();
            if (roi == null) return;
            expectCircleRow = roi.Row; expectCircleCol = roi.Column; expectCircleRadius = roi.Radius;
            if (transform && toolPar.InputPar.跟随 != null && toolPar.InputPar.跟随.Count > 0 && templatePose != null && templatePose.Count > 0)
            {
                for (int i = 0; i < toolPar.InputPar.跟随.Count; i++)
                {
                    HTuple homMat2D, row, col;
                    XYU target = toolPar.InputPar.跟随[i];
                    HOperatorSet.VectorAngleToRigid(templatePose[0].Point.X, templatePose[0].Point.Y, templatePose[0].U, target.Point.X, target.Point.Y, target.U, out homMat2D);
                    HOperatorSet.AffineTransPixel(homMat2D, roi.Row, roi.Column, out row, out col);
                    newExpecCircleRow.Add(row); newExpectCircleCol.Add(col); newExpectCircleRadius.Add(roi.Radius);
                }
            }
            else { newExpecCircleRow.Add(roi.Row); newExpectCircleCol.Add(roi.Column); newExpectCircleRadius.Add(roi.Radius); }
            if (newExpecCircleRow.Count > 0) { LastExpectRow = newExpecCircleRow[0].D; LastExpectCol = newExpectCircleCol[0].D; }
        }

        private HObject CreateMeasurementImage(HObject image, out bool ownsImage)
        {
            ownsImage = false;
            if (!HasMask()) return image;
            HObject domain = null, allowed = null, reduced = null;
            try
            {
                HOperatorSet.GetDomain(image, out domain);
                HOperatorSet.Difference(domain, final_region, out allowed);
                HOperatorSet.ReduceDomain(image, allowed, out reduced);
                ownsImage = true;
                return reduced;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                if (reduced != null) reduced.Dispose();
                return image;
            }
            finally { if (domain != null) domain.Dispose(); if (allowed != null) allowed.Dispose(); }
        }

        private bool HasMask()
        {
            try
            {
                if (!IsValidObject(final_region)) return false;
                HTuple count;
                HOperatorSet.CountObj(final_region, out count);
                return count.TupleLength() > 0 && count.I > 0;
            }
            catch { return false; }
        }

        private static bool IsValidObject(HObject value)
        {
            try { return value != null && value.IsInitialized(); }
            catch { return false; }
        }

        private static void SelectInlierPoints(HObject initialCircle, HTuple rows, HTuple columns, int rejectCount, out HTuple keptRows, out HTuple keptColumns, out HTuple rejectedRows, out HTuple rejectedColumns)
        {
            List<PointDistance> distances = new List<PointDistance>();
            for (int i = 0; i < rows.TupleLength(); i++)
            {
                HTuple distance, nearest;
                HOperatorSet.DistancePc(initialCircle, rows[i], columns[i], out distance, out nearest);
                distances.Add(new PointDistance { Row = rows[i].D, Column = columns[i].D, Distance = distance.D });
            }
            distances.Sort(delegate(PointDistance left, PointDistance right) { return left.Distance.CompareTo(right.Distance); });
            int keepCount = Math.Max(3, distances.Count - rejectCount);
            keptRows = new HTuple(); keptColumns = new HTuple(); rejectedRows = new HTuple(); rejectedColumns = new HTuple();
            for (int i = 0; i < distances.Count; i++)
            {
                if (i < keepCount) { keptRows[i] = distances[i].Row; keptColumns[i] = distances[i].Column; }
                else { int index = i - keepCount; rejectedRows[index] = distances[i].Row; rejectedColumns[index] = distances[i].Column; }
            }
        }

        private static bool TryFitCircle(HTuple rows, HTuple columns, out double row, out double column, out double radius)
        {
            row = 0; column = 0; radius = 0;
            if (rows == null || columns == null || rows.TupleLength() < 3) return false;
            HObject contour = null;
            try
            {
                HOperatorSet.GenContourPolygonXld(out contour, rows, columns);
                HTuple fitRow, fitColumn, fitRadius, startPhi, endPhi, pointOrder;
                HOperatorSet.FitCircleContourXld(contour, "algebraic", -1, 0, 0, 3, 2, out fitRow, out fitColumn, out fitRadius, out startPhi, out endPhi, out pointOrder);
                if (fitRadius == null || fitRadius.TupleLength() == 0 || fitRadius.D <= 0) return false;
                row = fitRow.D; column = fitColumn.D; radius = fitRadius.D; return true;
            }
            catch { return false; }
            finally { if (contour != null) contour.Dispose(); }
        }

        public static PointF FitCenter(List<PointF> points, double epsilon = 0.1)
        {
            if (points == null || points.Count < 3) return PointF.Empty;
            double[] rows = new double[points.Count], columns = new double[points.Count];
            for (int i = 0; i < points.Count; i++) { rows[i] = points[i].X; columns[i] = points[i].Y; }
            Circle circle = FitCircleLeastSquares(rows, columns);
            return circle == null ? PointF.Empty : new PointF((float)circle.X, (float)circle.Y);
        }

        public bool FitCircle(double[] rows, double[] columns, out double resultColumn, out double resultRow, out double radius)
        {
            Circle circle = FitCircleLeastSquares(rows, columns);
            if (circle == null) { resultColumn = -1; resultRow = -1; radius = -1; return false; }
            resultRow = circle.X; resultColumn = circle.Y; radius = circle.R; return true;
        }

        public Circle LeastSquaresFit(double[] rows, double[] columns) { return FitCircleLeastSquares(rows, columns); }

        private static Circle FitCircleLeastSquares(double[] rows, double[] columns)
        {
            if (rows == null || columns == null || rows.Length != columns.Length || rows.Length < 3) return null;
            HTuple rowTuple = new HTuple(), columnTuple = new HTuple();
            for (int i = 0; i < rows.Length; i++) { rowTuple[i] = rows[i]; columnTuple[i] = columns[i]; }
            double row, column, radius;
            return TryFitCircle(rowTuple, columnTuple, out row, out column, out radius) ? new Circle { X = row, Y = column, R = radius } : null;
        }

        private ROICircle GetCircleRoi()
        {
            if (L_regions == null || L_regions.Count == 0) return null;
            ROICircle roi = L_regions[0] as ROICircle;
            return roi == null || roi.Radius <= 1 || double.IsNaN(roi.Radius) || double.IsInfinity(roi.Radius) ? null : roi;
        }

        private void ResetResults()
        {
            if (circleCenter == null) circleCenter = new List<XY>(); else circleCenter.Clear();
            toolPar.ResultPar.圆心列表.Clear();
            toolPar.ResultPar.是否找到圆 = false;
            toolPar.ResultPar.结果圆 = new Circle();
            toolPar.ResultPar.圆半径 = 0;
        }

        private void UpdateResultDisplay(bool runTool)
        {
            Circle result = toolPar.ResultPar.结果圆;
            bool found = toolPar.ResultPar.是否找到圆;
            string row = found ? result.X.ToString("F3") : "--";
            string col = found ? result.Y.ToString("F3") : "--";
            string radius = found ? result.R.ToString("F3") : "--";
            Action update = delegate { Frm_FindCircleTool.Instance.UpdateResultDisplay(row, col, radius, found); };
            if (runTool && Frm_FindCircleTool.IsOpen) update(); else SafeInvokeFindCircleWindow(update);
        }

        private void SafeInvokeFindCircleWindow(Action action)
        {
            if (Frm_FindCircleTool.IsOpen) TryPostControlAction(Frm_FindCircleTool.Instance, action);
        }

        private static ToolRunStatu SuccessStatus() { return Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功; }
        private static ToolRunStatu FailureImageStatus() { return Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像; }
        private static ToolRunStatu UnknownFailureStatus() { return Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            obj = new object();
            EnsureLoadedState();
            toolPar.InputPar.图像 = null;
        }

        internal void EnsureLoadedState()
        {
            if (obj == null) obj = new object();
            if (L_regions == null) L_regions = new List<ROI>();
            if (circleCenter == null) circleCenter = new List<XY>();
            if (newExpecCircleRow == null) newExpecCircleRow = new List<HTuple>();
            if (newExpectCircleCol == null) newExpectCircleCol = new List<HTuple>();
            if (newExpectCircleRadius == null) newExpectCircleRadius = new List<HTuple>();
            if (templatePose == null) templatePose = new List<XYU>();
            if (templatePose.Count == 0) templatePose.Add(new XYU());
            if (expectCircleRow == null || expectCircleRow.TupleLength() == 0) expectCircleRow = new HTuple(300);
            if (expectCircleCol == null || expectCircleCol.TupleLength() == 0) expectCircleCol = new HTuple(300);
            if (expectCircleRadius == null || expectCircleRadius.TupleLength() == 0) expectCircleRadius = new HTuple(200);
            if (L_regions.Count == 0) L_regions.Add(new ROICircle(expectCircleRow.D, expectCircleCol.D, expectCircleRadius.D));
            if (toolPar == null) toolPar = new ToolPar();
            if (toolPar.InputPar == null) toolPar.InputPar = new InputPar();
            if (toolPar.RunPar == null) toolPar.RunPar = new RunPar();
            if (toolPar.ResultPar == null) toolPar.ResultPar = new ResultPar();
            if (toolPar.InputPar.跟随 == null) toolPar.InputPar.跟随 = new List<XYU>();
            if (toolPar.ResultPar.圆心列表 == null) toolPar.ResultPar.圆心列表 = new List<XY>();
            if (toolPar.ResultPar.结果圆 == null) toolPar.ResultPar.结果圆 = new Circle();
            if (brush_region == null) brush_region = new HObject();
            if (final_region == null) final_region = new HObject();
            if (contours == null) contours = new HObject();
            drawMode = false;
            NormalizeParameters();
        }

        internal ToolPar toolPar = new ToolPar();

        [Serializable]
        public class ToolPar : ToolParBase
        {
            private InputPar _inputPar = new InputPar();
            public InputPar InputPar { get { return _inputPar; } set { _inputPar = value; } }
            private RunPar _runPar = new RunPar();
            public RunPar RunPar { get { return _runPar; } set { _runPar = value; } }
            private ResultPar _resultPar = new ResultPar();
            public ResultPar ResultPar { get { return _resultPar; } set { _resultPar = value; } }
        }

        [Serializable]
        public class InputPar
        {
            private HObject _图像;
            public HObject 图像 { get { return _图像; } set { _图像 = value; } }
            private List<XYU> _跟随 = new List<XYU>();
            public List<XYU> 跟随 { get { return _跟随; } set { _跟随 = value; } }
        }

        [Serializable]
        public class RunPar { }

        [Serializable]
        internal class ResultPar
        {
            // 保留旧字段名和 List<XY> 类型，确保旧 .pjt 可反序列化。
            private List<XY> _圆心 = new List<XY>();
            // 工具箱把“圆心”声明为 DataType.XY，所以流程输出必须是单点。
            public XY 圆心
            {
                get { return _圆心.Count > 0 ? _圆心[0] : new XY(); }
                set { _圆心.Clear(); if (value != null) _圆心.Add(value); }
            }
            public List<XY> 圆心列表 { get { return _圆心; } set { _圆心 = value ?? new List<XY>(); } }
            private bool _是否找到圆;
            public bool 是否找到圆 { get { return _是否找到圆; } set { _是否找到圆 = value; } }
            private Circle _结果圆 = new Circle();
            public Circle 结果圆 { get { return _结果圆; } set { _结果圆 = value; } }
            private double _圆半径;
            public double 圆半径 { get { return _圆半径; } set { _圆半径 = value; } }
        }

        private sealed class CircleMeasureResult : IDisposable
        {
            internal bool Found;
            internal double Row;
            internal double Column;
            internal double Radius;
            internal HObject MeasureContours;
            internal HObject ResultContour;
            internal HTuple MeasureRows = new HTuple();
            internal HTuple MeasureColumns = new HTuple();
            internal HTuple RejectedRows = new HTuple();
            internal HTuple RejectedColumns = new HTuple();
            public void Dispose() { if (MeasureContours != null) MeasureContours.Dispose(); if (ResultContour != null) ResultContour.Dispose(); }
        }

        private sealed class PointDistance
        {
            internal double Row;
            internal double Column;
            internal double Distance;
        }
    }

    public struct ttt
    {
        public double row;
        public double col;
        public double distance;
    }

    [Serializable]
    public class Circle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double R { get; set; }
    }
}
