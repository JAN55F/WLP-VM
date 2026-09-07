using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using ViewWindow.Model;
using HalconTool;
using System.Drawing;

namespace VMPro
{
    /// <summary>
    /// 二维直线查找工具。
    ///
    /// 完整数据流：输入图像 -> 从 Rect2 ROI 提取期望线 -> 根据模板匹配位姿做刚性跟随
    /// -> HALCON Metrology 沿线布置卡尺 -> 提取边缘点 -> 可选剔除离群点
    /// -> 输出最终直线端点。
    ///
    /// 坐标约定：为兼容旧工程，XY/Line 中的 X 实际表示 HALCON Row，Y 实际表示
    /// HALCON Column。修改坐标含义时必须同步检查显示、跟随和所有下游工具。
    /// </summary>
    [Serializable]
    internal class FindLineTool : ToolBase
    {
        /// <summary>
        /// 新建工具时初始化默认搜索 ROI 和空的模板基准位姿。
        /// 有采集图像时按其分辨率生成 ROI；没有图像时使用固定兜底尺寸。
        /// 构造阶段只做初始化，不执行实际找线。
        /// </summary>
        internal FindLineTool()
        {




            // 尝试取得当前流程的采集图像，使新建 ROI 能适配实际相机分辨率。
            HObject image = null;
            string s_jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
            for (int i = 0; i < Job.FindJobByName(s_jobName).L_toolList.Count; i++)
            {
                if (Job.FindJobByName(s_jobName).L_toolList[i].toolType == ToolType.ImageAcq)
                {
                    image = ((AcqImageTool)Job.FindJobByName(s_jobName).L_toolList[i].tool).toolPar.ResultPar.图像;
                    break;
                }
            }

            if (image != null)
            {
                HTuple w, h;
                HOperatorSet.GetImageSize(image, out w, out h);
                Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.genRect2(h.D / 8.0, w.D / 8.0, 0, h.D / 40, w.D / 12, ref this.L_regions);
                //this.ringRadiusLength = w.I / 60;
            }
            else
            {
                // 参数依次为中心 Row、中心 Column、角度、两个方向的半长度。
                Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.genRect2(400.0, 500.0, 0, 160, 120, ref this.L_regions);
            }

            EnableLineRoiEditing();




            //GetImageWindowControl(Frm_Job .Instance .tbc_jobs .SelectedTab .Text ).hwc_imageWindow.viewWindow.genRect2(400.0, 500.0, 0, 300.0, 200.0, ref  L_regions);
            //GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).regions = this.L_regions;
            //ROIRectangle2 rect = new ROIRectangle2(400.0, 500.0, 0, 300.0, 200.0);
            //this.L_regions.Add(rect);
            
            Frm_FindLineTool.Instance.regions = this.L_regions;


            // (0,0,0) 是“尚未学习跟随基准”的占位值，拿到有效跟随输入后会被替换。
            XYU xyu = new XYU();
            xyu.Point.X = 0;
            xyu.Point.Y = 0;
            templatePose.Add(xyu);
        }




        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 唯一搜索区域，实际类型应为 ROIRectangle2。
        /// 长轴定义期望线方向，区域宽度定义卡尺搜索范围。
        /// </summary>
        internal List<ViewWindow.Model.ROI> L_regions = new List<ViewWindow.Model.ROI>();

        /// <summary>
        /// 被跟随的位置
        /// </summary>
        internal XYU followedPose = new XYU();
        /// <summary>
        /// 找边极性，从明到暗或从暗到明
        /// </summary>
        internal string polarity = "positive";
        /// <summary>
        /// 沿期望线均匀布置的卡尺数量，对应 HALCON num_measures。
        /// </summary>
        internal int cliperNum = 20;
        /// <summary>
        /// 卡尺宽度，传给 HALCON measure_length2。
        /// </summary>
        internal int caliperWidth = 20;
        /// <summary>
        /// 显示卡尺
        /// </summary>
        internal bool displayCaliper = true;
        /// <summary>
        /// 显示特征点
        /// </summary>
        internal bool displayFeature = true;
        /// <summary>
        /// 是否显示最终拟合线：调试运行显示到工具窗口，流程运行显示到主预览窗口。
        /// </summary>
        internal bool displayLine = true;
        /// <summary>
        /// 卡尺边缘幅值阈值，对应 HALCON measure_threshold。
        /// </summary>
        internal int threshold = 30;
        /// <summary>
        /// 同一卡尺检测到多条边时的选择方式，对应 HALCON measure_select。
        /// </summary>
        internal string edgeSelect = "all";
        /// <summary>
        /// Metrology 对象最低接受分数，对应 HALCON min_score。
        /// </summary>
        internal double minScore = 0.5;
        /// <summary>
        /// 找到的线段
        /// </summary>
        internal Line resultLine = new Line();
        /// <summary>
        /// 本轮实际测量的期望线端点集合。一个跟随输入位姿对应一条期望线。
        /// </summary>
        internal List<HTuple> newExpectLineStartRow = new List<HTuple>(), newExpectLineStartCol = new List<HTuple>(), newExpectLineEndRow = new List<HTuple>(), newExpectLineEndCol = new List<HTuple>();
        /// <summary>
        /// 卡尺沿搜索方向的半长度，对应 HALCON measure_length1。
        /// 当前值直接读取 Rect2 ROI，保证界面区域和算法参数一致。
        /// </summary>
        private int _length = 80;
        internal int Length
        {
            get
            {
                _length = (int)((ROIRectangle2)L_regions[0]).Lenth1;
                return _length;
            }
            set { _length = value; }
        }
        /// <summary>
        /// 查找到的线的起点行坐标
        /// </summary>
        private HTuple _resultLineStartRow = 0;
        internal HTuple ResultLineStartRow
        {
            get
            {
                _resultLineStartRow = Math.Round((double)_resultLineStartRow, 3);
                return _resultLineStartRow;
            }
            set { _resultLineStartRow = value; }
        }
        /// <summary>
        /// 查找到的线的起点列坐标
        /// </summary>
        private HTuple _resultLineStartCol = 0;
        internal HTuple ResultLineStartCol
        {
            get
            {
                _resultLineStartCol = Math.Round((double)_resultLineStartCol, 3);
                return _resultLineStartCol;
            }
            set { _resultLineStartCol = value; }
        }
        /// <summary>
        /// 查找到的线的终点行坐标
        /// </summary>
        private HTuple _resultLineEndRow = 0;
        internal HTuple ResultLineEndRow
        {
            get
            {
                _resultLineEndRow = Math.Round((double)_resultLineEndRow, 3);
                return _resultLineEndRow;
            }
            set { _resultLineEndRow = value; }
        }
        /// <summary>
        /// 查找到的线的终点列坐标
        /// </summary>
        private HTuple _resultLineEndCol = 0;
        internal HTuple ResultLineEndCol
        {
            get
            {
                _resultLineEndCol = Math.Round((double)_resultLineEndCol, 3);
                return _resultLineEndCol;
            }
            set { _resultLineEndCol = value; }
        }
        /// <summary>
        /// 查找到线的方向
        /// </summary>
        private HTuple _angle = 0;
        internal HTuple Angle
        {
            get
            {
                _angle = Math.Round((double)_angle, 3);
                return _angle;
            }
            set { _angle = value; }
        }


        internal void ClearLastInput()
        {
            try
            {
                toolPar.InputPar.跟随 = null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 将当前跟随输入保存为 ROI 的学习基准位姿。
        /// 只有首次配置，或用户确实修改了 ROI 时才调用，避免仅仅重新打开工具就改变偏移基准。
        /// </summary>
        internal void CaptureTemplatePoseFromCurrentInput()
        {
            if (toolPar.InputPar.跟随 == null || toolPar.InputPar.跟随.Count == 0)
                return;

            XYU currentPose = toolPar.InputPar.跟随[0];
            XYU pose = new XYU();
            pose.Point.X = currentPose.Point.X;
            pose.Point.Y = currentPose.Point.Y;
            pose.U = currentPose.U;

            if (templatePose == null)
                templatePose = new List<XYU>();
            else
                templatePose.Clear();
            templatePose.Add(pose);
        }

        /// <summary>
        /// 兼容新建工具的 (0,0,0) 占位位姿；已保存过的学习基准不得在打开窗口时覆盖。
        /// </summary>
        internal void EnsureTemplatePoseFromCurrentInput()
        {
            bool hasLearnedPose = templatePose != null && templatePose.Count > 0 &&
                                  (Math.Abs(templatePose[0].Point.X) > 0.000001 ||
                                   Math.Abs(templatePose[0].Point.Y) > 0.000001 ||
                                   Math.Abs(templatePose[0].U) > 0.000001);
            if (!hasLearnedPose)
                CaptureTemplatePoseFromCurrentInput();
        }

        /// <summary>
        /// 把保存的基础 ROI 变换到当前模板匹配位姿，并将当前位姿设为新的显示基准。
        /// 用于重新打开配置窗口，保证可编辑 ROI 与正式运行使用的期望线处于同一位置。
        /// </summary>
        internal void RebaseRoiToCurrentFollowPose()
        {
            if (L_regions == null || L_regions.Count == 0 ||
                templatePose == null || templatePose.Count == 0 ||
                toolPar.InputPar.跟随 == null || toolPar.InputPar.跟随.Count == 0)
                return;

            ROIRectangle2 rectangle = L_regions[0] as ROIRectangle2;
            if (rectangle == null)
                return;

            XYU basePose = templatePose[0];
            XYU currentPose = toolPar.InputPar.跟随[0];
            double deltaAngle = currentPose.U - basePose.U;
            bool poseChanged = Math.Abs(currentPose.Point.X - basePose.Point.X) > 0.000001 ||
                               Math.Abs(currentPose.Point.Y - basePose.Point.Y) > 0.000001 ||
                               Math.Abs(deltaAngle) > 0.000001;
            if (!poseChanged)
                return;

            HTuple homMat2D;
            HOperatorSet.VectorAngleToRigid(basePose.Point.X, basePose.Point.Y, basePose.U,
                                            currentPose.Point.X, currentPose.Point.Y, currentPose.U,
                                            out homMat2D);
            HTuple transformedRow, transformedCol;
            HOperatorSet.AffineTransPixel(homMat2D, rectangle.Row, rectangle.Column,
                                          out transformedRow, out transformedCol);

            rectangle.createRectangle2(transformedRow.D, transformedCol.D,
                                       rectangle.Phi + deltaAngle,
                                       rectangle.Lenth1, rectangle.Lenth2);
            EnableLineRoiEditing();
            CaptureTemplatePoseFromCurrentInput();
        }
        /// <summary>
        /// 制作模板时的输入位姿
        /// </summary>
        internal List<XYU> templatePose = new List<XYU>();

        /// <summary>
        /// 在工具配置窗口中预览查线效果。
        /// showROI 控制是否显示用户可编辑的 ROI，trans 控制是否按当前跟随位姿显示预期线。
        /// </summary>
        internal void ShowContour(bool showROI, bool trans = true)
        {
            ShowContour(showROI, trans, false);
        }

        internal void ShowContour(bool showROI, bool trans, bool preserveInteractiveRoi)
        {
            try
            {
                if (toolPar.InputPar.图像 == null || L_regions == null || L_regions.Count == 0)
                    return;

                EnableLineRoiEditing();
                BuildExpectedLines(trans);

                if (preserveInteractiveRoi)
                {
                    Frm_FindLineTool.Instance.hWindow_Final1.viewWindow._hWndControl.clearHObjectList();
                    Frm_FindLineTool.Instance.hWindow_Final1.viewWindow._hWndControl.repaint();
                }
                else
                {
                    Frm_FindLineTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                    if (showROI)
                        Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                }

                HTuple handleID;
                HOperatorSet.CreateMetrologyModel(out handleID);
                try
                {
                    HTuple width, height;
                    HOperatorSet.GetImageSize(toolPar.InputPar.图像, out width, out height);
                    HOperatorSet.SetMetrologyModelImageSize(handleID, width[0], height[0]);
                    HTuple index;
                    HOperatorSet.AddMetrologyObjectLineMeasure(handleID, newExpectLineStartRow[0], newExpectLineStartCol[0], newExpectLineEndRow[0], newExpectLineEndCol[0], new HTuple(Length), new HTuple(caliperWidth), new HTuple(1), new HTuple(30), new HTuple(), new HTuple(), out index);
                    ApplyMetrologyParams(handleID);
                    HOperatorSet.ApplyMetrologyModel(toolPar.InputPar.图像, handleID);

                    HObject contours;
                    HTuple row, col;
                    HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row, out col);
                    if (displayCaliper)
                        Frm_FindLineTool.Instance.hWindow_Final1.DispObj(contours, "blue");

                    if (displayFeature)
                        DisplayFeaturePoints(row, col, true, false);

                    HObject line;
                    HOperatorSet.GetMetrologyObjectResultContour(out line, handleID, new HTuple("all"), new HTuple("all"), new HTuple(1.5));
                    if (displayLine)
                        Frm_FindLineTool.Instance.hWindow_Final1.DispObj(line, "green");

                    // DispObj 会立即画在当前窗口最上层。再按统一顺序完整重绘一次：
                    // 背景图 -> 卡尺/结果层 -> ROI，确保正在拖动的 ROI 始终可见且位于最上层。
                    Frm_FindLineTool.Instance.hWindow_Final1.viewWindow._hWndControl.repaint();
                }
                finally
                {
                    HOperatorSet.ClearMetrologyModel(handleID);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal bool SyncDisplayedRoi(bool refreshPreview)
        {
            if (L_regions == null || L_regions.Count == 0)
                return false;

            int index;
            List<double> data;
            ROI roi = Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.smallestActiveROI(out data, out index);
            if (roi == null || index < 0)
                return false;

            // 找线工具只有一个搜索 ROI。即使旧窗口状态里残留了多个显示项，
            // 也始终以当前激活项覆盖唯一的业务 ROI，避免索引不一致导致拖动失效。
            L_regions[0] = roi;
            EnableLineRoiEditing();
            Frm_FindLineTool.Instance.regions = L_regions;

            // ROI 此刻是按当前图像/当前工件位置重新编辑的，后续偏移应以当前匹配位姿为基准。
            CaptureTemplatePoseFromCurrentInput();

            if (refreshPreview)
                ShowContour(true, true, true);

            return true;
        }

        /// <summary>
        /// 拖动期间的轻量预览。只更新期望线和 ROI，不执行耗时的 Metrology。
        /// 完整卡尺、特征点和拟合结果在鼠标松开后由 ShowContour() 刷新。
        /// </summary>
        internal void ShowDraggingPreview()
        {
            try
            {
                if (toolPar.InputPar.图像 == null || L_regions == null || L_regions.Count == 0)
                    return;

                EnableLineRoiEditing();
                BuildExpectedLines(true);

                var window = Frm_FindLineTool.Instance.hWindow_Final1;
                window.viewWindow._hWndControl.clearHObjectList();

                if (displayLine)
                {
                    for (int i = 0; i < newExpectLineStartRow.Count; i++)
                    {
                        HTuple rows = newExpectLineStartRow[i].TupleConcat(newExpectLineEndRow[i]);
                        HTuple cols = newExpectLineStartCol[i].TupleConcat(newExpectLineEndCol[i]);
                        HObject expectedLine;
                        HOperatorSet.GenContourPolygonXld(out expectedLine, rows, cols);
                        window.DispObj(expectedLine, "green");
                    }
                }

                // 统一重绘保证背景、期望线、ROI 的层级稳定，ROI 始终在最上层。
                window.viewWindow._hWndControl.repaint();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        /// <summary>
        /// 编辑找边卡尺
        /// </summary>
        internal void EditCaliper()
        {
            try
            {
                EnableLineRoiEditing();
                Frm_FindLineTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                if (L_regions.Count == 0)
                    Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.genRect2(400.0, 500.0, 0, 300.0, 200.0, ref  L_regions);
                else
                    Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                Frm_FindLineTool.Instance.regions = L_regions;

                followedPose.Point.X = toolPar.InputPar.跟随[0].Point.X;
                followedPose.Point.Y = toolPar.InputPar.跟随[0].Point.Y;
                followedPose.U = toolPar.InputPar.跟随[0].U;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 要剔除的最远离群点数量。0 表示直接采用 HALCON Metrology 的拟合结果。
        /// </summary>
        internal int ignoreNum = 0;

        /// <summary>
        /// 对保留的卡尺边缘点执行 Tukey 鲁棒直线拟合。
        /// X/Y 参数分别表示 Row/Column，返回 Line 的 X/Y 也遵循相同约定。
        /// </summary>
        private Line FitLine(double[] X, double[] Y)
        {
            HTuple rows = X[0];
            HTuple cols = Y[0];
            for (int i = 1; i < X.Length; i++)
            {
                rows = rows.TupleConcat(X[i]);
                cols = cols.TupleConcat(Y[i]);
            }
            HObject Contour;
            HOperatorSet.GenContourPolygonXld(out Contour, rows, cols);
            HTuple RowBegin, ColBegin, RowEnd, ColEnd, Nr, Nc, Dist;
            HOperatorSet.FitLineContourXld(Contour, "tukey", -1, 0, 5, 2, out RowBegin, out ColBegin, out RowEnd, out ColEnd, out Nr, out Nc, out Dist);
            Line line = new Line();
            line.起点.X = RowBegin;
            line.起点.Y = ColBegin;
            line.终点.X = RowEnd;
            line.终点.Y = ColEnd;
            return line;
        }

        /// <summary>
        /// 从矩形 ROI 中取出代表“期望直线”的两个端点。
        /// 这里沿用原 ROI 数据结构的端点索引，避免破坏已有保存格式。
        /// </summary>
        private void GetBaseLine(out HTuple startRow, out HTuple startCol, out HTuple endRow, out HTuple endCol)
        {
            startRow = L_regions[0].getRowsData()[7];
            startCol = L_regions[0].getColsData()[7];
            endRow = L_regions[0].getRowsData()[9];
            endCol = L_regions[0].getColsData()[9];
        }

        /// <summary>
        /// 开启找线专用编辑方式：内部拖动整体，两端拖动长度和角度。
        /// 老工程中的普通旋转矩形也会在加载后自动启用，不改变原有 ROI 保存结构。
        /// </summary>
        internal void EnableLineRoiEditing()
        {
            if (L_regions == null || L_regions.Count == 0)
                return;

            ROIRectangle2 rectangle = L_regions[0] as ROIRectangle2;
            if (rectangle != null)
                rectangle.EndpointRotationEnabled = true;
        }

        /// <summary>
        /// 生成本次要搜索的预期直线。
        /// 有跟随输入时，把学习时 ROI 通过刚性变换移动到当前工件位姿；没有跟随时直接使用原 ROI。
        /// </summary>
        private void BuildExpectedLines(bool trans)
        {
            // 每帧都重新生成，防止沿用上一帧或上一组跟随结果。
            newExpectLineStartRow.Clear();
            newExpectLineStartCol.Clear();
            newExpectLineEndRow.Clear();
            newExpectLineEndCol.Clear();

            HTuple baseStartRow, baseStartCol, baseEndRow, baseEndCol;
            GetBaseLine(out baseStartRow, out baseStartCol, out baseEndRow, out baseEndCol);

            if (trans && toolPar.InputPar.跟随 != null && toolPar.InputPar.跟随.Count > 0 && templatePose != null && templatePose.Count > 0)
            {
                for (int i = 0; i < toolPar.InputPar.跟随.Count; i++)
                {
                    // 这里只使用刚性变换（平移+旋转），不会向 ROI 引入缩放或错切。
                    HTuple homMat2D;
                    HOperatorSet.VectorAngleToRigid(templatePose[0].Point.X, templatePose[0].Point.Y, templatePose[0].U, toolPar.InputPar.跟随[i].Point.X, toolPar.InputPar.跟随[i].Point.Y, toolPar.InputPar.跟随[i].U, out homMat2D);
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

        /// <summary>
        /// 统一设置 HALCON Metrology 找线参数，保证预览和正式运行使用同一套参数。
        /// </summary>
        private void ApplyMetrologyParams(HTuple handleID)
        {
            // 极性决定搜索由暗到明、由明到暗或两种灰度跃迁。
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_transition"), new HTuple(polarity));
            // num_measures 是卡尺数；length1/length2 是单个卡尺两个方向的半尺寸。
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("num_measures"), new HTuple(cliperNum));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length1"), new HTuple(Length));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length2"), new HTuple(caliperWidth));
            // threshold 过滤弱边；select 决定存在多条候选边时取 first、last 或 all。
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_threshold"), new HTuple(threshold));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_select"), new HTuple(edgeSelect));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("min_score"), new HTuple(minScore));
        }

        /// <summary>
        /// 显示卡尺提取到的边缘点；被剔除的异常点用红色，参与拟合的点用橙色。
        /// </summary>
        private void DisplayFeaturePoints(HTuple rows, HTuple cols, bool runTool, bool rejected)
        {
            if (rows == null || rows.TupleLength() == 0)
                return;

            HObject cross;
            HOperatorSet.GenCrossContourXld(out cross, rows, cols, new HTuple(12), new HTuple(0));
            string color = rejected ? "red" : "orange";
            // 流程线程只负责计算，不能直接操作 HALCON/WinForms 窗口。
            if (runTool)
                Frm_FindLineTool.Instance.hWindow_Final1.DispObj(cross, color);
        }

        /// <summary>
        /// 读取 HALCON Metrology 拟合出的直线端点。
        /// all_param 对直线对象返回起点 row/col、终点 row/col 等参数。
        /// </summary>
        private bool TryGetLineResult(HTuple handleID, out Line line)
        {
            line = new Line();
            HTuple parameter;
            HOperatorSet.GetMetrologyObjectResult(handleID, new HTuple("all"), new HTuple("all"), new HTuple("result_type"), new HTuple("all_param"), out parameter);
            if (parameter.Length < 4)
                return false;

            line.起点.X = parameter[0];
            line.起点.Y = parameter[1];
            line.终点.X = parameter[2];
            line.终点.Y = parameter[3];
            return true;
        }

        /// <summary>
        /// 按“点到直线距离”剔除最远的若干边缘点，再用剩余点重新拟合直线。
        /// 这比直接取第一条边更适合现场有毛刺、污点、局部缺口的工件边缘。
        /// </summary>
        private Line FitLineAfterReject(HObject lineContour, HTuple rows, HTuple cols, int rejectCount, bool runTool)
        {
            // 计算每个边缘点到初始拟合线的距离，距离大的点更可能来自毛刺、污点或缺口。
            List<ttt> distance = new List<ttt>();
            for (int j = 0; j < rows.TupleLength(); j++)
            {
                HTuple distanceToLine, temp;
                HOperatorSet.DistancePc(lineContour, rows[j], cols[j], out distanceToLine, out temp);
                ttt item = new ttt();
                item.row = rows[j];
                item.col = cols[j];
                item.distance = distanceToLine.D;
                distance.Add(item);
            }

            distance.Sort(delegate(ttt a, ttt b)
            {
                return a.distance.CompareTo(b.distance);
            });

            // 直线至少需要两个点；配置超限时退回全部点，避免得到无效拟合输入。
            int validCount = distance.Count - rejectCount;
            if (validCount < 2)
                validCount = distance.Count;

            double[] keptRows = new double[validCount];
            double[] keptCols = new double[validCount];
            HTuple keptRowsTuple = new HTuple();
            HTuple keptColsTuple = new HTuple();
            for (int k = 0; k < validCount; k++)
            {
                keptRows[k] = distance[k].row;
                keptCols[k] = distance[k].col;
                keptRowsTuple[k] = distance[k].row;
                keptColsTuple[k] = distance[k].col;
            }

            if (displayFeature)
                DisplayFeaturePoints(keptRowsTuple, keptColsTuple, runTool, false);

            if (displayFeature && validCount < distance.Count)
            {
                HTuple rejectedRows = new HTuple();
                HTuple rejectedCols = new HTuple();
                for (int k = validCount; k < distance.Count; k++)
                {
                    rejectedRows[k - validCount] = distance[k].row;
                    rejectedCols[k - validCount] = distance[k].col;
                }
                DisplayFeaturePoints(rejectedRows, rejectedCols, runTool, true);
            }

            // 不再沿用初始 Metrology 端点，而是仅使用保留点进行一次鲁棒重拟合。
            return FitLine(keptRows, keptCols);
        }
        /// <summary>
        /// 正式运行直线查找。
        /// </summary>
        /// <param name="updateImage">是否刷新当前运行模式对应的背景图。</param>
        /// <param name="runTool">
        /// true：在找线模块内调试运行，可以更新工具窗口；
        /// false：作为流程节点运行，结果叠加通过 ShowObj() 送往主预览窗口。
        /// </param>
        /// <param name="toolName">流程框架传入的当前工具名称。</param>
        public override void Run(bool updateImage, bool runTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    if (toolPar.InputPar.图像 == null)
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                        return;
                    }
                    if (L_regions == null || L_regions.Count == 0)
                    {
                        toolRunStatu = ToolRunStatu.缺少输入搜索区域;
                        return;
                    }

                    // 兼容从老工程反序列化出来的普通 Rect2，确保端点拖动仍使用找线专用行为。
                    EnableLineRoiEditing();

                    // 这里只决定背景图画到哪个窗口；算法输入始终是 InputPar.图像，而不是窗口截图。
                    if (updateImage)
                    {
                        if (runTool)
                        {
                            Frm_FindLineTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                            Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                        }
                        else
                        {
                            GetImageWindowControl().hwc_imageWindow.HobjectToHimage(toolPar.InputPar.图像);
                            // 流程运行（runTool=false）时，如果找线窗体已打开，也同步更新窗体图像窗口。
                            SafeInvokeFindLineWindow(() =>
                            {
                                Frm_FindLineTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                                Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.displayInteractiveROI(L_regions);
                            });
                        }
                    }

                    // 正式运行始终尝试应用跟随变换；无跟随输入时自动退回原始 ROI。
                    BuildExpectedLines(true);
                    // 先清空上一轮结果，避免本轮失败时下游继续读取旧直线。
                    toolPar.ResultPar.圆心.Clear();
                    toolPar.ResultPar.线 = new Line();
                    bool foundLine = false;

                    // 一个跟随结果可能对应多个工件位姿，因此每条变换后的预期线都尝试测量一次。
                    for (int i = 0; i < newExpectLineStartRow.Count; i++)
                    {
                        HTuple handleID;
                        HOperatorSet.CreateMetrologyModel(out handleID);
                        try
                        {
                            // 每条期望线使用独立 Metrology Model，句柄必须在 finally 中释放。
                            HTuple width, height;
                            HOperatorSet.GetImageSize(toolPar.InputPar.图像, out width, out height);
                            HOperatorSet.SetMetrologyModelImageSize(handleID, width[0], height[0]);
                            HTuple index;
                            // 前四项是期望线端点，Length/caliperWidth 是卡尺两个方向的半尺寸。
                            HOperatorSet.AddMetrologyObjectLineMeasure(handleID, newExpectLineStartRow[i], newExpectLineStartCol[i], newExpectLineEndRow[i], newExpectLineEndCol[i], new HTuple(Length), new HTuple(caliperWidth), new HTuple(1), new HTuple(30), new HTuple(), new HTuple(), out index);
                            ApplyMetrologyParams(handleID);
                            HOperatorSet.ApplyMetrologyModel(toolPar.InputPar.图像, handleID);

                            // rows/cols 是每个卡尺实际找到的边缘点，后续用于显示和异常点剔除。
                            HObject contours;
                            HTuple rows, cols;
                            HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out rows, out cols);
                            if (rows == null || rows.TupleLength() < 2)
                                continue;

                            if (displayCaliper)
                            {
                                if (runTool)
                                    Frm_FindLineTool.Instance.hWindow_Final1.DispObj(contours, "blue");
                                else
                                    SafeInvokeFindLineWindow(() => Frm_FindLineTool.Instance.hWindow_Final1.DispObj(contours, "blue"));
                            }

                            HObject lineContour;
                            HOperatorSet.GetMetrologyObjectResultContour(out lineContour, handleID, new HTuple("all"), new HTuple("all"), new HTuple(1.5));

                            // ignoreNum=0 使用 HALCON 原结果；大于0时剔除最远点后重新拟合。
                            Line lineResult;
                            if (ignoreNum > 0 && rows.TupleLength() > ignoreNum + 1)
                                lineResult = FitLineAfterReject(lineContour, rows, cols, ignoreNum, runTool);
                            else
                            {
                                if (!TryGetLineResult(handleID, out lineResult))
                                    continue;
                                if (displayFeature)
                                    DisplayFeaturePoints(rows, cols, runTool, false);
                            }

                            // 同步内部缓存、流程输出和角度。X/Y 在此仍分别表示 Row/Column。
                            ResultLineStartRow = lineResult.起点.X;
                            ResultLineStartCol = lineResult.起点.Y;
                            ResultLineEndRow = lineResult.终点.X;
                            ResultLineEndCol = lineResult.终点.Y;
                            resultLine = lineResult;
                            toolPar.ResultPar.线 = lineResult;
                            HOperatorSet.AngleLx(ResultLineStartRow, ResultLineStartCol, ResultLineEndRow, ResultLineEndCol, out _angle);
                            foundLine = true;

							// 调试运行画到工具窗口，流程运行画到主预览窗口。
							if (displayLine)
							{
								HObject finalLine;
								HOperatorSet.GenRegionLine(out finalLine, ResultLineStartRow, ResultLineStartCol, ResultLineEndRow, ResultLineEndCol);
								if (runTool)
									Frm_FindLineTool.Instance.hWindow_Final1.DispObj(finalLine, "green");
								else
								{
									ShowObj(finalLine, "green");
									SafeInvokeFindLineWindow(() => Frm_FindLineTool.Instance.hWindow_Final1.DispObj(finalLine, "green"));
								}
							}
                        }
                        finally
                        {
                            HOperatorSet.ClearMetrologyModel(handleID);
                        }
                    }

                    if (!foundLine)
                    {
                        // 未找到线时清空结果页签，避免上一轮的旧值误导。
                        SafeInvokeFindLineWindow(() =>
                        {
                            Frm_FindLineTool.Instance.tbx_lineStartRow.TextStr = "";
                            Frm_FindLineTool.Instance.tbx_lineStartCol.TextStr = "";
                            Frm_FindLineTool.Instance.tbx_lineEndRow.TextStr = "";
                            Frm_FindLineTool.Instance.tbx_lineEndCol.TextStr = "";
                            Frm_FindLineTool.Instance.tbx_lineAngle.TextStr = "";
                        });
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未找到线);
                        return;
                    }

                    // 结果刷新统一走 SafeInvoke：窗口未打开直接跳过、跨线程 BeginInvoke，
                    // 模块调试与流程运行（页面打开时）都实时刷新，避免陈旧值误导。
                    SafeInvokeFindLineWindow(() =>
                    {
                        Frm_FindLineTool.Instance.tbx_resultStartRow.Text = ResultLineStartRow.ToString();
                        Frm_FindLineTool.Instance.tbx_resultStartCol.Text = ResultLineStartCol.ToString();
                        Frm_FindLineTool.Instance.tbx_resultEndRow.Text = ResultLineEndRow.ToString();
                        Frm_FindLineTool.Instance.tbx_resultEndCol.Text = ResultLineEndCol.ToString();
                        Frm_FindLineTool.Instance.tbx_lineStartRow.TextStr = ResultLineStartRow.ToString();
                        Frm_FindLineTool.Instance.tbx_lineStartCol.TextStr = ResultLineStartCol.ToString();
                        Frm_FindLineTool.Instance.tbx_lineEndRow.TextStr = ResultLineEndRow.ToString();
                        Frm_FindLineTool.Instance.tbx_lineEndCol.TextStr = ResultLineEndCol.ToString();
                        Frm_FindLineTool.Instance.tbx_lineAngle.TextStr = Math.Round(_angle.D, 3).ToString();
                    });

                    // 用经过三位小数归一化的属性值覆盖输出，保持界面和下游读取一致。
                    toolPar.ResultPar.线.起点.X = ResultLineStartRow;
                    toolPar.ResultPar.线.起点.Y = ResultLineStartCol;
                    toolPar.ResultPar.线.终点.X = ResultLineEndRow;
                    toolPar.ResultPar.线.终点.Y = ResultLineEndCol;

                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 在 UI 线程安全地更新找线窗体；窗体未打开时直接跳过，不创建隐藏窗体。
        /// </summary>
        private void SafeInvokeFindLineWindow(Action action)
        {
            try
            {
                if (!Frm_FindLineTool.IsOpen)
                    return;
                Frm_FindLineTool frm = Frm_FindLineTool.Instance;
                if (frm.InvokeRequired)
                    frm.BeginInvoke(action);
                else
                    action();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
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
            /// <summary>本轮测量使用的 HALCON 图像，由流程输入连接赋值。</summary>
            private HObject _图像;

            public HObject 图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }

            /// <summary>
            /// 可选模板匹配位姿列表；每个 XYU 表示 Row、Column、Angle。
            /// 多个位姿会生成多条期望线并逐一测量。
            /// </summary>
            private List<XYU> _跟随 = new List<XYU>();

            public List<XYU> 跟随
            {
                get { return _跟随; }
                set { _跟随 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {
            /// <summary>
            /// 历史兼容字段，当前直线结果不使用圆心；运行前仍需清空以免旧工程读到残留值。
            /// </summary>
            private List<XY> _圆心 = new List<XY>();

            public List<XY> 圆心
            {
                get { return _圆心; }
                set { _圆心 = value; }
            }

            /// <summary>最终拟合线，端点 X/Y 分别表示 HALCON Row/Column。</summary>
            private Line _线 = new Line();
            public Line 线
            {
                get { return _线; }
                set { _线 = value; }
            }

        }



    }
}
