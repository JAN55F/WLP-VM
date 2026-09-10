using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using ViewWindow.Model;
using HalconTool;
using System.Drawing;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace VMPro
{
    /// <summary>
    /// 直线边缘检测工具 - 完全重写版本
    /// 使用HALCON Metrology沿指定直线方向布置卡尺，提取边缘点，拟合输出直线。
    /// 支持模板匹配跟随、离群点剔除、多工件位姿处理。
    /// 坐标约定：X = Row, Y = Column
    /// </summary>
    [Serializable]
    internal class FindLineTool : ToolBase
    {
        private object syncLock = new object();

        internal FindLineTool()
        {
            InitializeROI();
            InitializeParameters();
            InitializeTemplatePose();
        }

        private void InitializeROI()
        {
            HObject image = TryGetInputImage();
            HTuple width, height;
            double centerRow, centerCol, halfLength1, halfLength2;

            if (TryGetHalconImageSize(image, out width, out height))
            {
                centerRow = height.D / 2.0;
                centerCol = width.D / 2.0;
                halfLength1 = Math.Max(20.0, height.D / 16.0);
                halfLength2 = Math.Max(40.0, width.D / 8.0);
            }
            else
            {
                centerRow = 240.0;
                centerCol = 320.0;
                halfLength1 = 40.0;
                halfLength2 = 80.0;
            }

            L_regions = new List<ROI> { new ROIRectangle2(centerRow, centerCol, 0, halfLength1, halfLength2) };
            EnableLineRoiEditing();
        }

        private void InitializeParameters()
        {
            polarity = "positive";
            cliperNum = 24;
            caliperWidth = 25;
            threshold = 40;
            edgeSelect = "all";
            minScore = 0.5;
            ignoreNum = 0;
            displayCaliper = true;
            displayFeature = true;
            displayLine = true;
        }

        private void InitializeTemplatePose()
        {
            templatePose = new List<XYU> { new XYU() };
        }

        private HObject TryGetInputImage()
        {
            try
            {
                Job currentJob = Frm_Job.Instance?.tbc_jobs?.SelectedTab == null
                    ? null
                    : Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text);

                if (currentJob == null) return null;

                for (int i = 0; i < currentJob.L_toolList.Count; i++)
                {
                    if (currentJob.L_toolList[i].toolType == ToolType.ImageAcq)
                    {
                        return ((AcqImageTool)currentJob.L_toolList[i].tool).toolPar.ResultPar.图像;
                    }
                }
            }
            catch { }
            return null;
        }

        internal List<ROI> L_regions = new List<ROI>();
        internal XYU followedPose = new XYU();

        internal string polarity = "positive";
        internal int cliperNum = 24;
        internal int caliperWidth = 25;
        internal bool displayCaliper = true;
        internal bool displayFeature = true;
        internal bool displayLine = true;
        internal int threshold = 40;
        internal string edgeSelect = "all";
        internal double minScore = 0.5;
        internal int ignoreNum = 0;
        internal Line resultLine = new Line();

        internal List<HTuple> newExpectLineStartRow = new List<HTuple>();
        internal List<HTuple> newExpectLineStartCol = new List<HTuple>();
        internal List<HTuple> newExpectLineEndRow = new List<HTuple>();
        internal List<HTuple> newExpectLineEndCol = new List<HTuple>();

        private int _length = 80;
        internal int Length
        {
            get
            {
                ROIRectangle2 rect = L_regions?.Count > 0 ? L_regions[0] as ROIRectangle2 : null;
                return rect != null ? (int)rect.Lenth1 : _length;
            }
            set { _length = value; }
        }

        private HTuple _resultLineStartRow = 0;
        internal HTuple ResultLineStartRow
        {
            get { return Math.Round((double)_resultLineStartRow, 3); }
            set { _resultLineStartRow = value; }
        }

        private HTuple _resultLineStartCol = 0;
        internal HTuple ResultLineStartCol
        {
            get { return Math.Round((double)_resultLineStartCol, 3); }
            set { _resultLineStartCol = value; }
        }

        private HTuple _resultLineEndRow = 0;
        internal HTuple ResultLineEndRow
        {
            get { return Math.Round((double)_resultLineEndRow, 3); }
            set { _resultLineEndRow = value; }
        }

        private HTuple _resultLineEndCol = 0;
        internal HTuple ResultLineEndCol
        {
            get { return Math.Round((double)_resultLineEndCol, 3); }
            set { _resultLineEndCol = value; }
        }

        private HTuple _angle = 0;
        internal HTuple Angle
        {
            get { return Math.Round((double)_angle, 3); }
            set { _angle = value; }
        }

        internal List<XYU> templatePose = new List<XYU>();

        internal void ClearLastInput()
        {
            try { toolPar.InputPar.跟随 = new List<XYU>(); }
            catch (Exception ex) { Log.SaveError(ex); }
        }

        internal void CaptureTemplatePoseFromCurrentInput()
        {
            if (toolPar.InputPar.跟随 == null || toolPar.InputPar.跟随.Count == 0) return;

            XYU current = toolPar.InputPar.跟随[0];
            XYU pose = new XYU
            {
                Point = new XY { X = current.Point.X, Y = current.Point.Y },
                U = current.U
            };

            templatePose = new List<XYU> { pose };
        }

        internal void EnsureTemplatePoseFromCurrentInput()
        {
            bool hasValidPose = templatePose?.Count > 0 &&
                (Math.Abs(templatePose[0].Point.X) > 0.000001 ||
                 Math.Abs(templatePose[0].Point.Y) > 0.000001 ||
                 Math.Abs(templatePose[0].U) > 0.000001);

            if (!hasValidPose)
                CaptureTemplatePoseFromCurrentInput();
        }

        internal void RebaseRoiToCurrentFollowPose()
        {
            if (L_regions?.Count == 0 || templatePose?.Count == 0 || toolPar.InputPar.跟随?.Count == 0)
                return;

            ROIRectangle2 rect = L_regions[0] as ROIRectangle2;
            if (rect == null) return;

            XYU basePose = templatePose[0];
            XYU currentPose = toolPar.InputPar.跟随[0];

            double deltaAngle = currentPose.U - basePose.U;
            bool poseChanged = Math.Abs(currentPose.Point.X - basePose.Point.X) > 0.000001 ||
                               Math.Abs(currentPose.Point.Y - basePose.Point.Y) > 0.000001 ||
                               Math.Abs(deltaAngle) > 0.000001;
            if (!poseChanged) return;

            HTuple homMat2D;
            HOperatorSet.VectorAngleToRigid(basePose.Point.X, basePose.Point.Y, basePose.U,
                                            currentPose.Point.X, currentPose.Point.Y, currentPose.U,
                                            out homMat2D);

            HTuple newRow, newCol;
            HOperatorSet.AffineTransPixel(homMat2D, rect.Row, rect.Column, out newRow, out newCol);

            rect.createRectangle2(newRow.D, newCol.D, rect.Phi + deltaAngle, rect.Lenth1, rect.Lenth2);
            EnableLineRoiEditing();
            CaptureTemplatePoseFromCurrentInput();
        }

        internal void NormalizeParameters()
        {
            cliperNum = Math.Max(2, Math.Min(720, cliperNum));
            caliperWidth = Math.Max(1, Math.Min(1000, caliperWidth));
            threshold = Math.Max(1, Math.Min(255, threshold));
            ignoreNum = Math.Max(0, Math.Min(cliperNum - 2, ignoreNum));
            minScore = Math.Max(0.01, Math.Min(1.0, minScore));

            ROIRectangle2 rect = L_regions?.Count > 0 ? L_regions[0] as ROIRectangle2 : null;
            if (rect != null)
            {
                double len1 = Math.Max(1, Math.Min(2000, rect.Lenth1));
                if (Math.Abs(len1 - rect.Lenth1) > 0.000001)
                    rect.SetLength1(len1);
                _length = (int)len1;
            }

            if (polarity != "positive" && polarity != "negative" && polarity != "all")
                polarity = "positive";
            if (edgeSelect != "first" && edgeSelect != "last" && edgeSelect != "all")
                edgeSelect = "all";
        }

        internal bool HasValidInput()
        {
            HTuple width, height;
            return TryGetHalconImageSize(toolPar.InputPar.图像, out width, out height);
        }

        internal string GetRoiSummary()
        {
            ROIRectangle2 rect = L_regions?.Count > 0 ? L_regions[0] as ROIRectangle2 : null;
            if (rect == null) return "未设置搜索区";
            return $"中心 Row {rect.Row:F1}  Col {rect.Column:F1}\r\n方向 {rect.Phi:F3} rad  搜索半长 {rect.Lenth1:F1} px";
        }

        internal void ResetRoiToImage()
        {
            HTuple width, height;
            double centerRow = 240, centerCol = 320, len1 = 40, len2 = 80;

            if (TryGetHalconImageSize(toolPar.InputPar.图像, out width, out height))
            {
                centerRow = height.D / 2.0;
                centerCol = width.D / 2.0;
                len1 = Math.Max(20, height.D / 16.0);
                len2 = Math.Max(40, width.D / 8.0);
            }

            L_regions.Clear();
            L_regions.Add(new ROIRectangle2(centerRow, centerCol, 0, len1, len2));
            _length = (int)len1;
            EnableLineRoiEditing();
            CaptureTemplatePoseFromCurrentInput();
        }

        internal void ShowContour(bool showROI, bool trans = true)
        {
            ShowContour(showROI, trans, false);
        }

        internal void ShowContour(bool showROI, bool trans, bool preserveInteractiveRoi)
        {
            lock (syncLock)
            {
                try
                {
                    NormalizeParameters();

                    HTuple width, height;
                    if (!TryGetHalconImageSize(toolPar.InputPar.图像, out width, out height))
                    {
                        UpdateWindowStatus("未指定输入图像", false);
                        return;
                    }
                    if (L_regions?.Count == 0)
                    {
                        UpdateWindowStatus("缺少搜索区域", false);
                        return;
                    }

                    EnableLineRoiEditing();
                    BuildExpectedLines(trans);

                    if (!preserveInteractiveRoi)
                    {
                        Frm_FindLineTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                        if (showROI)
                            Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                    }
                    else
                    {
                        Frm_FindLineTool.Instance.hWindow_Final1.viewWindow._hWndControl.clearHObjectList();
                    }

                    bool foundLine = MeasureAndDisplayLines(width, height);
                    Frm_FindLineTool.Instance.hWindow_Final1.viewWindow._hWndControl.repaint();
                    UpdateWindowStatus(foundLine ? "预览完成：已找到直线" : "预览完成：未找到直线", foundLine);
                }
                catch (Exception ex)
                {
                    Log.SaveError(ex);
                    UpdateWindowStatus("预览失败：" + ex.Message, false);
                }
            }
        }

        private bool MeasureAndDisplayLines(HTuple width, HTuple height)
        {
            bool foundLine = false;

            for (int i = 0; i < newExpectLineStartRow.Count; i++)
            {
                HTuple handleID = null;
                HObject measureContours = null, lineContour = null;

                try
                {
                    HOperatorSet.CreateMetrologyModel(out handleID);
                    HOperatorSet.SetMetrologyModelImageSize(handleID, width[0], height[0]);

                    HTuple index;
                    HOperatorSet.AddMetrologyObjectLineMeasure(handleID,
                        newExpectLineStartRow[i], newExpectLineStartCol[i],
                        newExpectLineEndRow[i], newExpectLineEndCol[i],
                        new HTuple(Length), new HTuple(caliperWidth), new HTuple(1), new HTuple(30),
                        new HTuple(), new HTuple(), out index);

                    ApplyMetrologyParams(handleID);
                    HOperatorSet.ApplyMetrologyModel(toolPar.InputPar.图像, handleID);

                    HTuple rows, cols;
                    HOperatorSet.GetMetrologyObjectMeasures(out measureContours, handleID,
                        new HTuple("all"), new HTuple("all"), out rows, out cols);

                    if (rows == null || rows.TupleLength() < 2) continue;

                    if (displayCaliper)
                        DisplayObject(measureContours, "#4c94d2", true);

                    HOperatorSet.GetMetrologyObjectResultContour(out lineContour, handleID,
                        new HTuple("all"), new HTuple("all"), new HTuple(1.5));

                    Line resultLine;
                    if (ignoreNum > 0 && rows.TupleLength() > ignoreNum + 1)
                        resultLine = FitLineAfterRejectOutliers(lineContour, rows, cols, ignoreNum, true);
                    else
                    {
                        if (!TryGetLineResult(handleID, out resultLine)) continue;
                        if (displayFeature)
                            DisplayFeaturePoints(rows, cols, true, false);
                    }

                    foundLine = true;
                    if (displayLine)
                        DisplayLineObject(resultLine.起点.X, resultLine.起点.Y, resultLine.终点.X, resultLine.终点.Y, "green", true);
                }
                catch (Exception ex)
                {
                    Log.SaveError(ex);
                }
                finally
                {
                    if (measureContours != null) measureContours.Dispose();
                    if (lineContour != null) lineContour.Dispose();
                    if (handleID != null && handleID.TupleLength() > 0)
                    {
                        try { HOperatorSet.ClearMetrologyModel(handleID); }
                        catch { }
                    }
                }
            }

            return foundLine;
        }

        internal bool SyncDisplayedRoi(bool refreshPreview)
        {
            if (L_regions?.Count == 0) return false;

            int index;
            List<double> data;
            ROI roi = Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.smallestActiveROI(out data, out index);
            if (roi == null || index < 0) return false;

            L_regions[0] = roi;
            EnableLineRoiEditing();
            Frm_FindLineTool.Instance.regions = L_regions;
            CaptureTemplatePoseFromCurrentInput();

            if (refreshPreview)
                ShowContour(true, true, true);

            return true;
        }

        internal void ShowDraggingPreview()
        {
            try
            {
                if (toolPar.InputPar.图像 == null || L_regions?.Count == 0) return;

                EnableLineRoiEditing();
                BuildExpectedLines(true);

                var window = Frm_FindLineTool.Instance.hWindow_Final1;
                window.viewWindow._hWndControl.clearHObjectList();

                if (displayLine)
                {
                    for (int i = 0; i < newExpectLineStartRow.Count; i++)
                    {
                        HObject expectedLine = null;
                        try
                        {
                            HTuple rows = newExpectLineStartRow[i].TupleConcat(newExpectLineEndRow[i]);
                            HTuple cols = newExpectLineStartCol[i].TupleConcat(newExpectLineEndCol[i]);
                            HOperatorSet.GenContourPolygonXld(out expectedLine, rows, cols);
                            window.DispObj(expectedLine, "green");
                        }
                        finally { if (expectedLine != null) expectedLine.Dispose(); }
                    }
                }

                window.viewWindow._hWndControl.repaint();
            }
            catch (Exception ex) { Log.SaveError(ex); }
        }

        internal void EditCaliper()
        {
            try
            {
                EnableLineRoiEditing();
                Frm_FindLineTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);

                if (L_regions.Count == 0)
                    Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.genRect2(240.0, 320.0, 0, 80.0, 160.0, ref L_regions);
                else
                    Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);

                Frm_FindLineTool.Instance.regions = L_regions;

                if (toolPar.InputPar.跟随?.Count > 0)
                {
                    followedPose = toolPar.InputPar.跟随[0];
                    CaptureTemplatePoseFromCurrentInput();
                }
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Failed to edit caliper/ROI" 
                    : "卡尺编辑失败", "EditCaliper");
            }
        }

        internal void EnableLineRoiEditing()
        {
            if (L_regions?.Count == 0) return;
            ROIRectangle2 rect = L_regions[0] as ROIRectangle2;
            if (rect != null) rect.EndpointRotationEnabled = true;
        }

        private void BuildExpectedLines(bool trans)
        {
            newExpectLineStartRow.Clear();
            newExpectLineStartCol.Clear();
            newExpectLineEndRow.Clear();
            newExpectLineEndCol.Clear();

            HTuple baseStartRow, baseStartCol, baseEndRow, baseEndCol;
            GetBaseLine(out baseStartRow, out baseStartCol, out baseEndRow, out baseEndCol);

            if (trans && toolPar.InputPar.跟随?.Count > 0 && templatePose?.Count > 0)
            {
                for (int i = 0; i < toolPar.InputPar.跟随.Count; i++)
                {
                    HTuple homMat2D;
                    HOperatorSet.VectorAngleToRigid(templatePose[0].Point.X, templatePose[0].Point.Y, templatePose[0].U,
                                                    toolPar.InputPar.跟随[i].Point.X, toolPar.InputPar.跟随[i].Point.Y, toolPar.InputPar.跟随[i].U,
                                                    out homMat2D);

                    HTuple startRow, startCol, endRow, endCol;
                    HOperatorSet.AffineTransPixel(homMat2D, baseStartRow, baseStartCol, out startRow, out startCol);
                    HOperatorSet.AffineTransPixel(homMat2D, baseEndRow, baseEndCol, out endRow, out endCol);

                    newExpectLineStartRow.Add(startRow);
                    newExpectLineStartCol.Add(startCol);
                    newExpectLineEndRow.Add(endRow);
                    newExpectLineEndCol.Add(endCol);
                }
            }
            else
            {
                newExpectLineStartRow.Add(baseStartRow);
                newExpectLineStartCol.Add(baseStartCol);
                newExpectLineEndRow.Add(baseEndRow);
                newExpectLineEndCol.Add(baseEndCol);
            }
        }

        private void GetBaseLine(out HTuple startRow, out HTuple startCol, out HTuple endRow, out HTuple endCol)
        {
            if (L_regions?.Count == 0 || L_regions[0] == null)
            {
                startRow = 0; startCol = 0; endRow = 100; endCol = 0;
                return;
            }

            double[] rows = L_regions[0].getRowsData();
            double[] cols = L_regions[0].getColsData();

            startRow = rows.Length > 7 ? rows[7] : 0;
            startCol = cols.Length > 7 ? cols[7] : 0;
            endRow = rows.Length > 9 ? rows[9] : 100;
            endCol = cols.Length > 9 ? cols[9] : 0;
        }

        private void ApplyMetrologyParams(HTuple handleID)
        {
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_transition"), new HTuple(polarity));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("num_measures"), new HTuple(cliperNum));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length1"), new HTuple(Length));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length2"), new HTuple(caliperWidth));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_threshold"), new HTuple(threshold));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_select"), new HTuple(edgeSelect));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("min_score"), new HTuple(minScore));
        }

        private void DisplayFeaturePoints(HTuple rows, HTuple cols, bool runTool, bool rejected)
        {
            if (rows == null || rows.TupleLength() == 0) return;

            HObject cross = null;
            try
            {
                HOperatorSet.GenCrossContourXld(out cross, rows, cols, new HTuple(12), new HTuple(0));
                DisplayObject(cross, rejected ? "red" : "#f0a340", runTool);
            }
            finally { if (cross != null) cross.Dispose(); }
        }

        private bool TryGetLineResult(HTuple handleID, out Line line)
        {
            line = new Line();
            HTuple parameter;
            HOperatorSet.GetMetrologyObjectResult(handleID, new HTuple("all"), new HTuple("all"),
                                                   new HTuple("result_type"), new HTuple("all_param"), out parameter);

            if (parameter == null || parameter.Length < 4) return false;

            line.起点.X = parameter[0];
            line.起点.Y = parameter[1];
            line.终点.X = parameter[2];
            line.终点.Y = parameter[3];
            return true;
        }

        private Line FitLineAfterRejectOutliers(HObject lineContour, HTuple rows, HTuple cols, int rejectCount, bool runTool)
        {
            List<PointDist> points = new List<PointDist>();

            for (int i = 0; i < rows.TupleLength(); i++)
            {
                HTuple dist, temp;
                HOperatorSet.DistancePc(lineContour, rows[i], cols[i], out dist, out temp);
                points.Add(new PointDist { Row = rows[i].D, Col = cols[i].D, Distance = dist.D });
            }

            points.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            int keepCount = Math.Max(2, points.Count - rejectCount);
            double[] keptRows = new double[keepCount];
            double[] keptCols = new double[keepCount];

            for (int i = 0; i < keepCount; i++)
            {
                keptRows[i] = points[i].Row;
                keptCols[i] = points[i].Col;
            }

            if (displayFeature)
            {
                HTuple keptRowsTuple = new HTuple(keptRows);
                HTuple keptColsTuple = new HTuple(keptCols);
                DisplayFeaturePoints(keptRowsTuple, keptColsTuple, runTool, false);

                if (keepCount < points.Count)
                {
                    HTuple rejectedRows = new HTuple();
                    HTuple rejectedCols = new HTuple();
                    for (int i = keepCount; i < points.Count; i++)
                    {
                        rejectedRows[i - keepCount] = points[i].Row;
                        rejectedCols[i - keepCount] = points[i].Col;
                    }
                    DisplayFeaturePoints(rejectedRows, rejectedCols, runTool, true);
                }
            }

            return FitLineWithTukey(keptRows, keptCols);
        }

        private Line FitLineWithTukey(double[] rows, double[] cols)
        {
            if (rows == null || cols == null || rows.Length < 2) return new Line();

            HTuple rowTuple = new HTuple(rows);
            HTuple colTuple = new HTuple(cols);
            HObject contour = null;

            try
            {
                HOperatorSet.GenContourPolygonXld(out contour, rowTuple, colTuple);
                HTuple fitRow, fitCol, fitRowEnd, fitColEnd, nr, nc, dist;
                HOperatorSet.FitLineContourXld(contour, "tukey", -1, 0, 5, 2,
                    out fitRow, out fitCol, out fitRowEnd, out fitColEnd, out nr, out nc, out dist);

                return new Line
                {
                    起点 = new XY { X = fitRow, Y = fitCol },
                    终点 = new XY { X = fitRowEnd, Y = fitColEnd }
                };
            }
            finally { if (contour != null) contour.Dispose(); }
        }

        private void DisplayObject(HObject obj, string color, bool runTool)
        {
            if (obj == null || !obj.IsInitialized()) return;

            if (runTool && Frm_FindLineTool.IsOpen)
            {
                Frm_FindLineTool.Instance.hWindow_Final1.DispObj(obj, color);
                return;
            }

            ShowObj(obj, color);
            if (Frm_FindLineTool.IsOpen)
            {
                HObject snapshot = new HObject(obj);
                bool queued = TryPostControlAction(Frm_FindLineTool.Instance, delegate
                {
                    try { Frm_FindLineTool.Instance.hWindow_Final1.DispObj(snapshot, color); }
                    finally { snapshot.Dispose(); }
                });
                if (!queued) snapshot.Dispose();
            }
        }

        private void DisplayLineObject(HTuple row1, HTuple col1, HTuple row2, HTuple col2, string color, bool runTool)
        {
            HObject line = null;
            try
            {
                HOperatorSet.GenRegionLine(out line, row1, col1, row2, col2);
                DisplayObject(line, color, runTool);
            }
            finally { if (line != null) line.Dispose(); }
        }

        private void UpdateWindowStatus(string message, bool success)
        {
            if (!Frm_FindLineTool.IsOpen) return;
            TryPostControlAction(Frm_FindLineTool.Instance, () =>
            {
                Frm_FindLineTool.Instance.UpdateRunStatus(message, success, 0);
            });
        }

        public override void Run(bool updateImage, bool runTool, string toolName)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            lock (syncLock)
            {
                try
                {
                    NormalizeParameters();
                    InitializeRunState();

                    HTuple inputWidth, inputHeight;
                    if (!TryGetHalconImageSize(toolPar.InputPar.图像, out inputWidth, out inputHeight))
                    {
                        toolRunStatu = ToolRunStatu.未指定输入图像;
                        return;
                    }

                    if (L_regions?.Count == 0)
                    {
                        toolRunStatu = ToolRunStatu.缺少输入搜索区域;
                        return;
                    }

                    EnableLineRoiEditing();
                    UpdateBackgroundImage(updateImage, runTool);
                    BuildExpectedLines(true);

                    bool foundLine = MeasureAndDisplayLines(inputWidth, inputHeight);

                    if (!foundLine)
                    {
                        SafeUpdateResultDisplay("--", "--", "--", "--", "--", false);
                        toolRunStatu = ToolRunStatu.未找到线;
                        return;
                    }

                    SafeUpdateResultDisplay(
                        ResultLineStartRow.ToString(),
                        ResultLineStartCol.ToString(),
                        ResultLineEndRow.ToString(),
                        ResultLineEndCol.ToString(),
                        Angle.ToString(),
                        true);

                    toolPar.ResultPar.线.起点.X = ResultLineStartRow;
                    toolPar.ResultPar.线.起点.Y = ResultLineStartCol;
                    toolPar.ResultPar.线.终点.X = ResultLineEndRow;
                    toolPar.ResultPar.线.终点.Y = ResultLineEndCol;

                    toolRunStatu = ToolRunStatu.成功;
                }
                catch (Exception ex)
                {
                    toolPar.ResultPar.线 = new Line();
                    toolRunStatu = ToolRunStatu.未知原因;
                    Log.SaveError(ex);
                }
                finally
                {
                    stopwatch.Stop();
                    bool success = toolRunStatu == ToolRunStatu.成功;
                    Action updateUI = () =>
                    {
                        Frm_FindLineTool.Instance.UpdateResultDisplay(
                            success ? ResultLineStartRow.ToString() : "--",
                            success ? ResultLineStartCol.ToString() : "--",
                            success ? ResultLineEndRow.ToString() : "--",
                            success ? ResultLineEndCol.ToString() : "--",
                            success ? Angle.ToString() : "--",
                            success);
                        Frm_FindLineTool.Instance.UpdateRunStatus(toolRunStatu.ToString(), success, stopwatch.ElapsedMilliseconds);
                    };

                    if (runTool && Frm_FindLineTool.IsOpen)
                        updateUI();
                    else
                        SafeInvokeWindow(updateUI);
                }
            }
        }

        private void InitializeRunState()
        {
            toolRunStatu = ToolRunStatu.未知原因;
            toolPar.ResultPar.圆心.Clear();
            toolPar.ResultPar.线 = new Line();
        }

        private void UpdateBackgroundImage(bool updateImage, bool runTool)
        {
            if (!updateImage) return;

            if (runTool)
            {
                Frm_FindLineTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
            }
            else
            {
                ShowImage(toolPar.InputPar.图像);
                SafeInvokeWindow(() =>
                {
                    Frm_FindLineTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                    Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                });
            }
        }

        private void SafeUpdateResultDisplay(string startRow, string startCol, string endRow, string endCol, string angle, bool found)
        {
            SafeInvokeWindow(() =>
            {
                Frm_FindLineTool.Instance.UpdateResultDisplay(startRow, startCol, endRow, endCol, angle, found);
            });
        }

        private void SafeInvokeWindow(Action action)
        {
            if (!Frm_FindLineTool.IsOpen) return;
            TryPostControlAction(Frm_FindLineTool.Instance, action);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            syncLock = new object();
            EnsureLoadedState();
            toolPar.InputPar.图像 = null;
        }

        internal void EnsureLoadedState()
        {
            if (syncLock == null) syncLock = new object();
            if (L_regions == null) L_regions = new List<ROI>();
            if (L_regions.Count == 0) L_regions.Add(new ROIRectangle2(240.0, 320.0, 0, 80, 160));
            EnableLineRoiEditing();

            if (followedPose == null) followedPose = new XYU();
            if (resultLine == null) resultLine = new Line();
            if (newExpectLineStartRow == null) newExpectLineStartRow = new List<HTuple>();
            if (newExpectLineStartCol == null) newExpectLineStartCol = new List<HTuple>();
            if (newExpectLineEndRow == null) newExpectLineEndRow = new List<HTuple>();
            if (newExpectLineEndCol == null) newExpectLineEndCol = new List<HTuple>();
            if (templatePose == null) templatePose = new List<XYU>();
            if (templatePose.Count == 0) templatePose.Add(new XYU());

            if (toolPar == null) toolPar = new ToolPar();
            if (toolPar.InputPar == null) toolPar.InputPar = new InputPar();
            if (toolPar.RunPar == null) toolPar.RunPar = new RunPar();
            if (toolPar.ResultPar == null) toolPar.ResultPar = new ResultPar();
            if (toolPar.InputPar.跟随 == null) toolPar.InputPar.跟随 = new List<XYU>();
            if (toolPar.ResultPar.圆心 == null) toolPar.ResultPar.圆心 = new List<XY>();
            if (toolPar.ResultPar.线 == null) toolPar.ResultPar.线 = new Line();

            NormalizeParameters();
        }

        internal ToolPar toolPar = new ToolPar();

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
        public class InputPar
        {
            private HObject _图像;
            public HObject 图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }

            private List<XYU> _跟随 = new List<XYU>();
            public List<XYU> 跟随
            {
                get { return _跟随; }
                set { _跟随 = value; }
            }
        }

        [Serializable]
        public class RunPar { }

        [Serializable]
        internal class ResultPar
        {
            private List<XY> _圆心 = new List<XY>();
            public List<XY> 圆心
            {
                get { return _圆心; }
                set { _圆心 = value; }
            }

            private Line _线 = new Line();
            public Line 线
            {
                get { return _线; }
                set { _线 = value; }
            }
        }

        private sealed class PointDist
        {
            internal double Row;
            internal double Col;
            internal double Distance;
        }
    }
}
