using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace VMPro
{
    [Serializable]
    internal class BlobAnalyseTool : ToolBase
    {
        internal BlobAnalyseTool()
        {
            //默认添加一个面积筛选
            SelectItem select = new SelectItem();
            select.SelectType = "area";
            select.AreaDownLimit = 100;
            select.AreaUpLimit = 10000000;
            L_select.Add(select);
        }

        /// <summary>
        /// 轮廓线宽
        /// </summary>
        internal int lineWidth = 1;
        /// <summary>
        /// 排序模式
        /// </summary>
        internal SortMode sortMode = SortMode.从上至下且从左至右;
        /// <summary>
        /// 是否显示搜索区域
        /// </summary>
        internal bool displaySearchRegion = true;
        /// <summary>
        /// 是否显示中心十字架
        /// </summary>
        internal bool displayCross = true;
        /// <summary>
        /// 是否显示区域外接圆
        /// </summary>
        internal bool displayOutCircle = false;
        /// <summary>
        /// 结果区域
        /// </summary>
        internal HObject outputRegion;
        /// <summary>
        /// 搜索区域对应的图像
        /// </summary>
        private HObject searchRegionImage;
        /// <summary>
        /// 是否显示结果区域
        /// </summary>
        internal bool displayRegion = true;
        /// <summary>
        /// 阈值下限
        /// </summary>
        internal int minThreshold = 128;
        /// <summary>
        /// 阈值上限
        /// </summary>
        internal int maxThreshold = 255;
        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 搜索区域类型
        /// </summary>
        internal RegionType searchRegionType = (Project.Instance.configuration.language == Language.English ? RegionType.AllImage : RegionType.整幅图像);
        /// <summary>
        /// 结果区域填充模式
        /// </summary>
        internal FillMode regionDrawMode = FillMode.Margin;
        /// <summary>
        /// 流程运行时是否在本工具绘制前重绘主图像。
        /// 同一流程中连续的斑点工具只允许第一个重绘，后续工具需要在同一图层上叠加显示。
        /// </summary>
        internal bool clearMainImageBeforeDraw = true;
        /// <summary>
        /// 显示外接圆的填充模式
        /// </summary>
        internal FillMode outCircleDrawMode = FillMode.Margin;
        /// <summary>
        /// 筛选项集合
        /// </summary>
        internal List<SelectItem> L_select = new List<SelectItem>();
        /// <summary>
        /// 搜索区域
        /// </summary>
        internal List<ViewWindow.Model.ROI> L_regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 绘制搜索区域时的跟随基准位姿。
        /// </summary>
        internal List<XYU> templatePose = new List<XYU>();
        /// <summary>
        /// 区域处理操作集合
        /// </summary>
        internal List<PreProcessing> L_prePorcessing = new List<PreProcessing>();
        /// <summary>
        /// 结果区域集合
        /// </summary>
        internal List<BlobResult> L_resultBlob = new List<BlobResult>();
        /// <summary>
        /// 搜索区域
        /// </summary>
        private HObject _searchRegion;
        internal HObject SearchRegion
        {
            get
            {
                if (L_regions.Count > 0)
                    _searchRegion = L_regions[0].getRegion();
                return _searchRegion;
            }
            set { _searchRegion = value; }
        }

        /// <summary>
        /// 将当前跟随输入保存为搜索区域的学习基准位姿。
        /// </summary>
        internal void CaptureTemplatePoseFromCurrentInput()
        {
            if (toolPar == null || toolPar.InputPar == null ||
                toolPar.InputPar.跟随 == null || toolPar.InputPar.跟随.Count == 0 ||
                toolPar.InputPar.跟随[0] == null || toolPar.InputPar.跟随[0].Point == null)
                return;

            XYU currentPose = toolPar.InputPar.跟随[0];
            XYU pose = new XYU();
            pose.Point.X = currentPose.Point.X;
            pose.Point.Y = currentPose.Point.Y;
            pose.U = currentPose.U;

            if (templatePose == null)
                templatePose = new List<XYU>();
            templatePose.Clear();
            templatePose.Add(pose);
        }

        /// <summary>
        /// 旧流程没有保存跟随基准时，以第一次拿到的模板匹配位姿作为基准。
        /// 后续每次运行均相对此基准变换搜索区域。
        /// </summary>
        internal void EnsureTemplatePoseFromCurrentInput()
        {
            if (templatePose != null && templatePose.Count > 0)
                return;

            CaptureTemplatePoseFromCurrentInput();
        }

        /// <summary>
        /// 获取本次运行使用的搜索区域。内部 ROI 可以随模板位姿平移和旋转，
        /// 外部输入区域及整幅图像保持原有行为。
        /// </summary>
        internal HObject GetRuntimeSearchRegion()
        {
            HObject baseRegion = SearchRegion;
            if (searchRegionType == RegionType.AllImage ||
                searchRegionType == RegionType.整幅图像 ||
                searchRegionType == RegionType.InputRegion ||
                L_regions == null || L_regions.Count == 0 ||
                baseRegion == null ||
                templatePose == null || templatePose.Count == 0 ||
                templatePose[0] == null || templatePose[0].Point == null ||
                toolPar.InputPar.跟随 == null || toolPar.InputPar.跟随.Count == 0)
                return baseRegion;

            XYU basePose = templatePose[0];
            XYU currentPose = toolPar.InputPar.跟随[0];
            if (currentPose == null || currentPose.Point == null)
                return baseRegion;
            HTuple homMat2D;
            HOperatorSet.VectorAngleToRigid(basePose.Point.X, basePose.Point.Y, basePose.U,
                                            currentPose.Point.X, currentPose.Point.Y, currentPose.U,
                                            out homMat2D);

            HObject runtimeRegion;
            HOperatorSet.AffineTransRegion(baseRegion, out runtimeRegion, homMat2D, "nearest_neighbor");
            return runtimeRegion;
        }


        /// <summary>
        /// 清除搜索区域
        /// </summary>
        internal void ClearSearchRegion()
        {
            try
            {

                SearchRegion = null;
                searchRegionType = RegionType.AllImage;
                Frm_BlobAnalyseTool.Instance.cbx_searchRegionType.TextStr = "整幅图像";
                Frm_BlobAnalyseTool editor = Frm_BlobAnalyseTool.CurrentInstance;
                if (editor != null && !editor.IsDisposed)
                {
                    HTuple width;
                    HTuple height;
                    if (toolPar != null && toolPar.InputPar != null &&
                        TryGetHalconImageSize(toolPar.InputPar.图像, out width, out height))
                        editor.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                    else
                        editor.hWindow_Final1.ClearWindow();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 复位工具
        /// </summary>
        internal void ResetTool()
        {
            try
            {
                L_regions = new List<ViewWindow.Model.ROI>();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 保存筛选项
        /// </summary>
        internal void SaveSelectItem()
        {
            try
            {
                if (Job.loadForm)
                    return;
                L_select.Clear();
                for (int i = 0; i < Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows.Count; i++)
                {
                    if (Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[i].Cells[0].Value != null)
                    {
                        SelectItem selectItem = new SelectItem();
                        selectItem.SelectType = Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[i].Cells[0].Value.ToString();
                        try
                        {
                            selectItem.AreaDownLimit = Convert.ToInt32(Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[i].Cells[1].Value);
                            selectItem.AreaUpLimit = Convert.ToInt32(Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[i].Cells[2].Value);
                        }
                        catch
                        {
                            Frm_Output.Instance.OutputMsg("保存失败，输入数据不合法（错误代码：10501）", Color.Red);
                        }
                        L_select.Add(selectItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 添加处理项
        /// </summary>
        internal void AddProcessingItem()
        {
            try
            {
                //////switch (Frm_BlobAnalyseTool.Instance.tvw_preProcessingItem.SelectedNode.Text)
                //////{
                //////    //////if (preProcessingItem == "开运算")
                //////    //////{
                //////    //////    PreProcessing prePorcessing = new PreProcessing();
                //////    //////    prePorcessing.PreProcessingType = "开运算";
                //////    //////    prePorcessing.ElementType = "circle";
                //////    //////    prePorcessing.ElementSize = 3;
                //////    //////    prePorcessing.Enable = true;
                //////    //////    blobAnalyseTool.L_prePorcessing.Add(prePorcessing);
                //////    //////    int index = dgv_processingItem.Rows.Add();
                //////    //////    dgv_processingItem.Rows[index].Cells[0].Value = "开运算";
                //////    //////    ((DataGridViewCheckBoxCell)this.dgv_processingItem.Rows[index].Cells[1]).Value = true;
                //////    //////}
                //////    //////else if (preProcessingItem == "闭运算")
                //////    //////{
                //////    //////    PreProcessing prePorcessing = new PreProcessing();
                //////    //////    prePorcessing.PreProcessingType = "闭运算";
                //////    //////    prePorcessing.ElementType = "circle";
                //////    //////    prePorcessing.ElementSize = 3;
                //////    //////    prePorcessing.Enable = true;
                //////    //////    blobAnalyseTool.L_prePorcessing.Add(prePorcessing);
                //////    //////    int index = dgv_processingItem.Rows.Add();
                //////    //////    ((DataGridViewCheckBoxCell)this.dgv_processingItem.Rows[index].Cells[1]).Value = true;
                //////    //////    dgv_processingItem.Rows[index].Cells[0].Value = "闭运算";
                //////    //////}
                //////    case "填充":
                //////        PreProcessing prePorcessing = new PreProcessing();
                //////        prePorcessing.PreProcessingType = "填充";
                //////        prePorcessing.ElementType = "";
                //////        prePorcessing.ElementSize = 0;
                //////        prePorcessing.Enable = true;
                //////        L_prePorcessing.Add(prePorcessing);
                //////        int index = Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Add();
                //////        Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[0].Value = "填充";
                //////        ((DataGridViewCheckBoxCell)Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[1]).Value = true;
                //////        break;
                //////    case "腐蚀":
                //////        prePorcessing = new PreProcessing();
                //////        prePorcessing.PreProcessingType = "腐蚀";
                //////        prePorcessing.ElementType = "";
                //////        prePorcessing.ElementSize = 3;
                //////        prePorcessing.Enable = true;
                //////        prePorcessing.MinArea = 100;
                //////        prePorcessing.MaxArea = 10000;
                //////        L_prePorcessing.Add(prePorcessing);
                //////        index = Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Add();
                //////        Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[0].Value = "腐蚀";
                //////        ((DataGridViewCheckBoxCell)Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[1]).Value = true;
                //////        break;
                //////    case "膨胀":
                //////        prePorcessing = new PreProcessing();
                //////        prePorcessing.PreProcessingType = "膨胀";
                //////        prePorcessing.ElementType = "";
                //////        prePorcessing.Enable = true;
                //////        prePorcessing.ElementSize = 3;
                //////        prePorcessing.MinArea = 100;
                //////        prePorcessing.MaxArea = 10000;
                //////        L_prePorcessing.Add(prePorcessing);
                //////        index = Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Add();
                //////        Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[0].Value = "膨胀";
                //////        ((DataGridViewCheckBoxCell)Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[1]).Value = true;
                //////        break;
                //////    default:
                //////        Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
                //////        break;
                //////}
                Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Count - 1].Selected = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 点击结果列表
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="e"></param>
        internal void Click_Result_List(DataGridView dgv, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                for (int i = 0; i < L_resultBlob.Count; i++)
                {
                    if (L_resultBlob[i].Area.ToString() == dgv.Rows[e.RowIndex].Cells[1].Value.ToString() &&
                        L_resultBlob[i].Row.ToString() == dgv.Rows[e.RowIndex].Cells[2].Value.ToString() &&
                        L_resultBlob[i].Col.ToString() == dgv.Rows[e.RowIndex].Cells[3].Value.ToString())
                    {

                        Frm_BlobAnalyseTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                        //显示搜索区域
                        if (searchRegionType != RegionType.AllImage)
                        {
                            HOperatorSet.SetDraw(Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(SearchRegion, "blue");
                        }

                        //显示结果区域
                        if (displayRegion)
                        {
                            if (regionDrawMode == FillMode.Fill)
                                HOperatorSet.SetDraw(Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("fill"));
                            else
                                HOperatorSet.SetDraw(Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));
                            Frm_Main.Instance.Display_Obj(Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID, L_resultBlob[i].region);
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(L_resultBlob[i].region, "green");
                        }

                        //显示外接圆
                        if (displayOutCircle)
                        {
                            if (outCircleDrawMode == FillMode.Fill)
                                HOperatorSet.SetDraw(Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("fill"));
                            else
                                HOperatorSet.SetDraw(Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));
                            HOperatorSet.SetColor(Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                            HOperatorSet.DispCircle(Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID, L_resultBlob[i].Row, L_resultBlob[i].Col, L_resultBlob[i].CircumcircleRadius);
                        }

                        //显示中心十字架
                        if (displayCross)
                        {
                            HObject cross;
                            HOperatorSet.GenCrossContourXld(out cross, L_resultBlob[i].Row, L_resultBlob[i].Col, 20, 0);
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(cross, "blue");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绘制搜索区域
        /// </summary>
        internal void Draw_Search_Region(string jobName)
        {
            try
            {
                bool roiCreated = false;
                if (Frm_BlobAnalyseTool.Instance.cbx_searchRegionType.TextStr == "整幅图像")
                    return;
                HOperatorSet.SetLineStyle(Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_BlobAnalyseTool.Instance.hWindow_Final1.Select();
                if (toolPar.InputPar.图像 != null)
                {
                    Frm_BlobAnalyseTool.Instance.hWindow_Final1.ClearWindow();
                    Frm_BlobAnalyseTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                }
                switch (Frm_BlobAnalyseTool.Instance.cbx_searchRegionType.TextStr)
                {
                    case "矩形":
                    case "Rectangle1":
                        if (searchRegionType == RegionType.Rectangle1)
                        {
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                        }
                        else
                        {
                            this.L_regions.Clear();
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.viewWindow.genRect1(200.0, 200.0, 600.0, 800.0, ref this.L_regions);
                            roiCreated = true;
                        }
                        searchRegionType = RegionType.Rectangle1;
                        break;
                    case "仿射矩形":
                    case "Rectangle2":
                        if (searchRegionType == RegionType.Rectangle2)
                        {
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                        }
                        else
                        {
                            this.L_regions.Clear();
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.viewWindow.genRect2(400.0, 500.0, 0, 300.0, 200.0, ref this.L_regions);
                            roiCreated = true;
                        }
                        searchRegionType = RegionType.Rectangle2;
                        break;
                    case "圆":
                    case "Circle":
                        if (searchRegionType == RegionType.Circle)
                        {
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                        }
                        else
                        {
                            this.L_regions.Clear();
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.viewWindow.genCircle(400.0, 500.0, 200.0, ref this.L_regions);
                            roiCreated = true;
                        }
                        searchRegionType = RegionType.Circle;
                        break;
                    //////case "Ellipse":
                    //////    HTuple row4, column4, angle4, length4, length5;
                    //////    HOperatorSet.DrawEllipse(GetImageWindowControl().WindowHandle, out row4, out column4, out angle4, out length4, out length5);
                    //////    HObject ellipse;
                    //////    HOperatorSet.GenEllipse(out ellipse, row4, column4, angle4, length4, length5);
                    //////    HOperatorSet.DispObj(ellipse, GetImageWindowControl().WindowHandle);
                    //////    if (SearchRegion == null)
                    //////    {
                    //////        SearchRegion = ellipse;
                    //////    }
                    //////    else
                    //////    {
                    //////        HObject temp;
                    //////        HOperatorSet.Union2((HObject)SearchRegion, ellipse, out  temp);
                    //////        SearchRegion = temp;
                    //////    }
                    //////    searchRegionType = RegionType.Ellipse;
                    //////    break;
                    //////case "Any":
                    //////    HObject polygon;
                    //////    HOperatorSet.DrawRegion(out polygon, GetImageWindowControl().WindowHandle);
                    //////    HOperatorSet.DispObj(polygon, GetImageWindowControl().WindowHandle);
                    //////    if (SearchRegion == null)
                    //////    {
                    //////        SearchRegion = polygon;
                    //////    }
                    //////    else
                    //////    {
                    //////        HObject temp;
                    //////        HOperatorSet.Union2((HObject)SearchRegion, polygon, out  temp);
                    //////        SearchRegion = temp;
                    //////    }
                    //////    searchRegionType = RegionType.Any;
                    //////    break;
                    default:
                        Frm_BlobAnalyseTool editor = Frm_BlobAnalyseTool.CurrentInstance;
                        if (editor != null && !editor.IsDisposed)
                        {
                            HTuple width;
                            HTuple height;
                            if (toolPar != null && toolPar.InputPar != null &&
                                TryGetHalconImageSize(toolPar.InputPar.图像, out width, out height))
                                editor.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                            else
                                editor.hWindow_Final1.ClearWindow();
                        }
                        SearchRegion = null;
                        searchRegionType = RegionType.InputRegion;
                        break;
                }
                Frm_BlobAnalyseTool.Instance.regions = this.L_regions;
                if (roiCreated)
                    CaptureTemplatePoseFromCurrentInput();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 行列间隔像素数
        /// </summary>
        internal int spanPixelNum = 100;

        private static HObject CloneDisplayObject(HObject source)
        {
            if (source == null || !source.IsInitialized())
                return null;

            try
            {
                return new HObject(source);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }

        private static void DisposeDisplayObject(HObject value)
        {
            if (value == null)
                return;

            try
            {
                value.Dispose();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private static void DisplayRuntimeLayer(
            HTuple windowHandle,
            HObject layer,
            string color,
            string drawMode,
            int width)
        {
            if (layer == null || !layer.IsInitialized())
                return;

            HOperatorSet.SetDraw(windowHandle, drawMode);
            HOperatorSet.SetLineWidth(windowHandle, Math.Max(1, width));
            HOperatorSet.SetColor(windowHandle, color);
            HOperatorSet.DispObj(layer, windowHandle);
        }

        /// <summary>
        /// 流程运行不再直接读取图像窗口。先在工作线程复制本轮图层，
        /// 再在目标窗口的 UI 线程中按“背景 -> 搜索区 -> 结果 -> 外接圆 -> 十字”一次绘制。
        /// </summary>
        private void QueueRuntimeDisplay(
            HObject background,
            HObject searchRegion,
            HObject resultRegion,
            HObject outCircles,
            HObject crosses)
        {
            HObject backgroundSnapshot = CloneDisplayObject(background);
            HObject searchSnapshot = CloneDisplayObject(searchRegion);
            HObject resultSnapshot = CloneDisplayObject(resultRegion);
            HObject circleSnapshot = CloneDisplayObject(outCircles);
            HObject crossSnapshot = CloneDisplayObject(crosses);
            int lineWidthSnapshot = lineWidth;
            FillMode regionDrawModeSnapshot = regionDrawMode;
            FillMode outCircleDrawModeSnapshot = outCircleDrawMode;

            if (backgroundSnapshot == null && searchSnapshot == null &&
                resultSnapshot == null && circleSnapshot == null && crossSnapshot == null)
                return;

            Action releaseSnapshots = delegate
            {
                HObject value = backgroundSnapshot;
                backgroundSnapshot = null;
                DisposeDisplayObject(value);
                value = searchSnapshot;
                searchSnapshot = null;
                DisposeDisplayObject(value);
                value = resultSnapshot;
                resultSnapshot = null;
                DisposeDisplayObject(value);
                value = circleSnapshot;
                circleSnapshot = null;
                DisposeDisplayObject(value);
                value = crossSnapshot;
                crossSnapshot = null;
                DisposeDisplayObject(value);
            };

            PostToImageWindow(delegate(Job job, Frm_ImageWindow imageWindow)
            {
                try
                {
                    if (imageWindow.hwc_imageWindow == null)
                        return;

                    if (backgroundSnapshot != null)
                        imageWindow.hwc_imageWindow.HobjectToHimage(backgroundSnapshot);

                    HTuple windowHandle = imageWindow.hwc_imageWindow.HWindowHalconID;
                    DisplayRuntimeLayer(windowHandle, searchSnapshot, "blue", "margin", lineWidthSnapshot);
                    DisplayRuntimeLayer(windowHandle, resultSnapshot, "green",
                        regionDrawModeSnapshot == FillMode.Fill ? "fill" : "margin", 1);
                    DisplayRuntimeLayer(windowHandle, circleSnapshot, "green",
                        outCircleDrawModeSnapshot == FillMode.Fill ? "fill" : "margin", 1);
                    DisplayRuntimeLayer(windowHandle, crossSnapshot, "blue", "margin", 1);

                    // 不把本工具的 fill/线宽状态泄漏给后续工具。
                    HOperatorSet.SetDraw(windowHandle, "margin");
                    HOperatorSet.SetLineWidth(windowHandle, 1);
                }
                finally
                {
                    releaseSnapshots();
                }
            }, releaseSnapshots);
        }

        private void QueueResultGridUpdate()
        {
            Frm_BlobAnalyseTool editor = Frm_BlobAnalyseTool.CurrentInstance;
            if (editor == null || !ReferenceEquals(Frm_BlobAnalyseTool.blobAnalyseTool, this))
                return;

            BlobResult[] resultSnapshot = L_resultBlob == null
                ? new BlobResult[0]
                : L_resultBlob.ToArray();
            TryPostControlAction(editor, delegate
            {
                if (!editor.Visible || !ReferenceEquals(Frm_BlobAnalyseTool.blobAnalyseTool, this))
                    return;

                editor.dgv_result.Rows.Clear();
                for (int i = 0; i < resultSnapshot.Length; i++)
                {
                    int index = editor.dgv_result.Rows.Add();
                    editor.dgv_result.Rows[index].Cells[0].Value = i + 1;
                    editor.dgv_result.Rows[index].Cells[1].Value = resultSnapshot[i].Area;
                    editor.dgv_result.Rows[index].Cells[2].Value = resultSnapshot[i].Row;
                    editor.dgv_result.Rows[index].Cells[3].Value = resultSnapshot[i].Col;
                    editor.dgv_result.Rows[index].Cells[4].Value = resultSnapshot[i].CircumcircleRadius;
                }
            });
        }

        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage, bool runTool  , string toolName)
        {
            HObject runtimeCircleOverlay = null;
            HObject runtimeCrossOverlay = null;
            try
            {
                lock (obj)
                {
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;

                    // HObject 非 null 仍可能是 object ID 0，不能送入后续 HALCON 算子。
                    HTuple inputWidth;
                    HTuple inputHeight;
                    if (toolPar == null || toolPar.InputPar == null ||
                        !TryGetHalconImageSize(toolPar.InputPar.图像, out inputWidth, out inputHeight))
                    {
                        toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Lack_Of_Input_Image : ToolRunStatu.缺少输入图像;
                        return;
                    }
                    //如果搜索区域是外部输入，判断区域是否有为空
                    if (searchRegionType == RegionType.InputRegion && toolPar.InputPar.搜索区域 == null)
                    {
                        toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Lack_Of_Input_Search_Region : ToolRunStatu.缺少输入搜索区域;
                        return;
                    }
                    else
                    {
                        SearchRegion = toolPar.InputPar.搜索区域;
                    }
                    if (updateImage)
                    {
                        Frm_BlobAnalyseTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                    }

                    // 新建或旧版流程在首次取得跟随输入时记录 ROI 的学习基准。
                    // 不能每次覆盖，否则查找框始终以当前位姿为基准而看起来没有跟随。
                    EnsureTemplatePoseFromCurrentInput();
                    HObject runtimeSearchRegion = GetRuntimeSearchRegion();

                    //截取出搜索区域图像
                    if (searchRegionType != RegionType.AllImage)
                    {
                        HOperatorSet.ReduceDomain(toolPar.InputPar.图像, runtimeSearchRegion, out   searchRegionImage);
                    }

                    //开始阈值分割
                    HObject resultRegion; HTuple temp1;
                    if (searchRegionType != RegionType.AllImage)
                    {
                        //////if (Frm_BlobAnalyseTool.Instance.checkBox1.Checked)
                        //////    HOperatorSet.BinaryThreshold(searchRegionImage, out resultRegion, "max_separability", "dark", out  temp1);
                        //////else
                        HOperatorSet.Threshold(searchRegionImage, out resultRegion, minThreshold, maxThreshold);
                    }
                    else
                        HOperatorSet.Threshold(toolPar.InputPar.图像, out resultRegion, minThreshold, maxThreshold);

                    HTuple count = 0;
                    HOperatorSet.Connection(resultRegion, out resultRegion);
                    HOperatorSet.CountObj(resultRegion, out count);
                    if (count > 10000)
                    {
                        Frm_Output.Instance.OutputMsg("斑点分析工具分割出的斑点数大于10000，可能会造成电脑卡顿，已放弃执行", Color.Red);
                        toolRunStatu = ToolRunStatu.未知原因;
                        return;
                    }
                    //此处进行预处理
                    for (int i = 0; i < L_prePorcessing.Count; i++)
                    {
                        if (L_prePorcessing[i].Enable == false)
                            continue;
                        switch (L_prePorcessing[i].PreProcessingType)
                        {
                            case "开运算":
                                HOperatorSet.OpeningCircle(resultRegion, out resultRegion, L_prePorcessing[i].ElementSize);
                                break;
                            case "闭运算":
                                HOperatorSet.ClosingCircle(resultRegion, out resultRegion, L_prePorcessing[i].ElementSize);
                                break;
                            case "填充":
                                HOperatorSet.FillUp(resultRegion, out resultRegion);
                                break;
                            case "腐蚀":      //此处对面积在一定范围内的斑点进行腐蚀

                                HOperatorSet.CountObj(resultRegion, out count);
                                HObject tempRegion;
                                HOperatorSet.GenEmptyRegion(out tempRegion);
                                for (int j = 0; j < count; j++)
                                {
                                    HObject region;
                                    HOperatorSet.SelectObj(resultRegion, out region, new HTuple(j + 1));
                                    HTuple area, row, col;
                                    HOperatorSet.AreaCenter(region, out area, out row, out col);
                                    if ((int)area > L_prePorcessing[i].MinArea)
                                    {
                                        HOperatorSet.ErosionCircle(region, out region, new HTuple(L_prePorcessing[i].ElementSize));
                                    }
                                    HOperatorSet.Union2(tempRegion, region, out tempRegion);
                                }
                                resultRegion = tempRegion;
                                HOperatorSet.Connection(resultRegion, out resultRegion);
                                break;
                            case "膨胀":
                                HOperatorSet.Connection(resultRegion, out resultRegion);
                                HOperatorSet.CountObj(resultRegion, out count);
                                HOperatorSet.GenEmptyRegion(out tempRegion);
                                for (int j = 0; j < count; j++)
                                {
                                    HObject region;
                                    HOperatorSet.SelectObj(resultRegion, out region, new HTuple(j + 1));
                                    HTuple area1, row1, col1;
                                    HOperatorSet.AreaCenter(region, out area1, out row1, out col1);
                                    if ((int)area1 < L_prePorcessing[i].MaxArea && (int)area1 > L_prePorcessing[i].MinArea)
                                    {
                                        HOperatorSet.DilationCircle(region, out region, new HTuple(L_prePorcessing[i].ElementSize));
                                    }
                                    HOperatorSet.Union2(tempRegion, region, out tempRegion);
                                }
                                resultRegion = tempRegion;
                                HOperatorSet.Connection(resultRegion, out resultRegion);
                                break;
                        }
                    }

                    //特征筛选
                    for (int i = 0; i < L_select.Count; i++)
                    {
                        HOperatorSet.SelectShape(resultRegion, out resultRegion, L_select[i].SelectType, "and", L_select[i].AreaDownLimit, L_select[i].AreaUpLimit);
                    }

                    //显示搜索区域
                    if (runTool && searchRegionType != RegionType.AllImage && displaySearchRegion)
                    {
                        HTuple displayWindow = Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID;
                        // 搜索区域始终按轮廓画，不能继承上一轮结果区域的 fill 状态。
                        HOperatorSet.SetDraw(displayWindow, "margin");
                        HOperatorSet.SetLineWidth(displayWindow, lineWidth);
                        if (searchRegionType == RegionType.InputRegion)
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(toolPar.InputPar.搜索区域, "blue");
                        else
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(runtimeSearchRegion, "blue");
                    }

                    if (runTool && displayRegion)
                    {
                        HTuple displayWindow = Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID;
                        if (regionDrawMode == FillMode.Fill)
                            HOperatorSet.SetDraw(displayWindow, "fill");
                        else
                            HOperatorSet.SetDraw(displayWindow, "margin");
                        HOperatorSet.SetLineWidth(displayWindow, 1);
                        Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(resultRegion, "green");
                    }
                    outputRegion = resultRegion;
                    HOperatorSet.Union1(outputRegion, out outputRegion);
                    HOperatorSet.Connection(outputRegion, out outputRegion);

                    HTuple resultCount = 0;
                    HOperatorSet.CountObj(resultRegion, out resultCount);
                    L_resultBlob.Clear();
                    for (int i = 0; i < resultCount; i++)
                    {
                        HObject region;
                        HOperatorSet.SelectObj(resultRegion, out region, new HTuple(i + 1));

                        //显示外接圆
                        if (displayOutCircle && runTool)
                        {
                            HTuple row2, col2, radius2;
                            HOperatorSet.SmallestCircle(region, out row2, out col2, out radius2);
                            HTuple displayWindow = Frm_BlobAnalyseTool.Instance.hWindow_Final1.HWindowHalconID;
                            HOperatorSet.SetDraw(displayWindow,
                                outCircleDrawMode == FillMode.Fill ? "fill" : "margin");
                            HObject circle;
                            HOperatorSet.GenCircle(out circle, row2, col2, radius2);
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(circle, "green");
                            circle.Dispose();
                        }

                        HTuple area3, row3, col3;
                        HOperatorSet.AreaCenter(region, out area3, out row3, out col3);
                        HTuple row4, col4, radius4;
                        HOperatorSet.SmallestCircle(region, out row4, out col4, out radius4);
                        BlobResult blobResult = new BlobResult();

                        blobResult.Row = Math.Round(Convert.ToDouble(row3.ToString()), 3);
                        blobResult.Col = Math.Round(Convert.ToDouble(col3.ToString()), 3);
                        blobResult.Area = Math.Round(Convert.ToDouble(area3.ToString()), 3);
                        blobResult.CircumcircleRadius = Math.Round(Convert.ToDouble(radius4.ToString()), 3);
                        blobResult.region = region;
                        L_resultBlob.Add(blobResult);

                        //显示十字架
                        if (displayCross && runTool)
                        {
                            HObject cross;
                            HOperatorSet.GenCrossContourXld(out cross, row3, col3, 20, 0);
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(cross, "blue");
                            cross.Dispose();
                        }

                    }

                    // 主图像窗口的外接圆和十字各合并为一个 HALCON 对象组，
                    // 一轮只投递一次批量显示，避免按斑点数堆积 UI 回调。
                    if (!runTool && resultCount > 0)
                    {
                        if (displayOutCircle)
                        {
                            HTuple circleRows;
                            HTuple circleColumns;
                            HTuple circleRadii;
                            HOperatorSet.SmallestCircle(resultRegion,
                                out circleRows, out circleColumns, out circleRadii);
                            HOperatorSet.GenCircle(out runtimeCircleOverlay,
                                circleRows, circleColumns, circleRadii);
                        }

                        if (displayCross)
                        {
                            HTuple areas;
                            HTuple crossRows;
                            HTuple crossColumns;
                            HOperatorSet.AreaCenter(resultRegion,
                                out areas, out crossRows, out crossColumns);
                            HOperatorSet.GenCrossContourXld(out runtimeCrossOverlay,
                                crossRows, crossColumns, 20, 0);
                        }
                    }
                    //排序
                    BlobResult temp;
                    for (int i = 0; i < L_resultBlob.Count - 1; i++)
                    {
                        for (int j = i + 1; j < L_resultBlob.Count; j++)
                        {
                            if (L_resultBlob[i].Area < L_resultBlob[j].Area)
                            {
                                temp = L_resultBlob[i];
                                L_resultBlob[i] = L_resultBlob[j];
                                L_resultBlob[j] = temp;
                            }
                        }
                    }



                    //以下代码对结果依据分数进行排序
                    BlobResult temp111;
                    if (sortMode == SortMode.从上至下且从左至右)
                    {
                        for (int i = 0; i < L_resultBlob.Count; i++)
                        {
                            for (int j = i + 1; j < L_resultBlob.Count; j++)
                            {
                                if (L_resultBlob[i].Row - L_resultBlob[j].Row > spanPixelNum / 2)
                                {
                                    temp111 = L_resultBlob[i];
                                    L_resultBlob[i] = L_resultBlob[j];
                                    L_resultBlob[j] = temp111;
                                }
                            }
                        }

                        for (int i = 0; i < L_resultBlob.Count; i++)
                        {
                            for (int j = i + 1; j < L_resultBlob.Count; j++)
                            {
                                if ((L_resultBlob[i].Col - L_resultBlob[j].Col > spanPixelNum / 2) && (Math.Abs(L_resultBlob[i].Row - L_resultBlob[j].Row) < spanPixelNum / 2))
                                {
                                    temp111 = L_resultBlob[i];
                                    L_resultBlob[i] = L_resultBlob[j];
                                    L_resultBlob[j] = temp111;
                                }
                            }
                        }


                    }
                    else if (sortMode == SortMode.从左至右且从上至下)
                    {
                        for (int i = 0; i < L_resultBlob.Count; i++)
                        {
                            for (int j = i + 1; j < L_resultBlob.Count; j++)
                            {
                                if (L_resultBlob[i].Col - L_resultBlob[j].Col > spanPixelNum / 2)
                                {
                                    temp111 = L_resultBlob[i];
                                    L_resultBlob[i] = L_resultBlob[j];
                                    L_resultBlob[j] = temp111;
                                }
                            }
                        }

                        for (int i = 0; i < L_resultBlob.Count; i++)
                        {
                            for (int j = i + 1; j < L_resultBlob.Count; j++)
                            {
                                if ((L_resultBlob[i].Row - L_resultBlob[j].Row > spanPixelNum / 2) && (Math.Abs(L_resultBlob[i].Col - L_resultBlob[j].Col) < spanPixelNum / 2))
                                {
                                    temp111 = L_resultBlob[i];
                                    L_resultBlob[i] = L_resultBlob[j];
                                    L_resultBlob[j] = temp111;
                                }
                            }
                        }
                    }
                    else if (sortMode == SortMode.从上至下且从右至左)
                    {
                        for (int i = 0; i < L_resultBlob.Count; i++)
                        {
                            for (int j = i + 1; j < L_resultBlob.Count; j++)
                            {
                                if (L_resultBlob[i].Row - L_resultBlob[j].Row > spanPixelNum / 2)
                                {
                                    temp111 = L_resultBlob[i];
                                    L_resultBlob[i] = L_resultBlob[j];
                                    L_resultBlob[j] = temp111;
                                }
                            }
                        }

                        for (int i = 0; i < L_resultBlob.Count; i++)
                        {
                            for (int j = i + 1; j < L_resultBlob.Count; j++)
                            {
                                if ((L_resultBlob[i].Col - L_resultBlob[j].Col < spanPixelNum / 2) && (Math.Abs(L_resultBlob[i].Row - L_resultBlob[j].Row) < spanPixelNum / 2))
                                {
                                    temp111 = L_resultBlob[i];
                                    L_resultBlob[i] = L_resultBlob[j];
                                    L_resultBlob[j] = temp111;
                                }
                            }
                        }


                    }
                    else if (sortMode == SortMode.从左至右且从下至上)
                    {
                        for (int i = 0; i < L_resultBlob.Count; i++)
                        {
                            for (int j = i + 1; j < L_resultBlob.Count; j++)
                            {
                                if (L_resultBlob[i].Col - L_resultBlob[j].Col < spanPixelNum / 2)
                                {
                                    temp111 = L_resultBlob[i];
                                    L_resultBlob[i] = L_resultBlob[j];
                                    L_resultBlob[j] = temp111;
                                }
                            }
                        }

                        for (int i = 0; i < L_resultBlob.Count; i++)
                        {
                            for (int j = i + 1; j < L_resultBlob.Count; j++)
                            {
                                if ((L_resultBlob[i].Row - L_resultBlob[j].Row > spanPixelNum / 2) && (Math.Abs(L_resultBlob[i].Col - L_resultBlob[j].Col) < spanPixelNum / 2))
                                {
                                    temp111 = L_resultBlob[i];
                                    L_resultBlob[i] = L_resultBlob[j];
                                    L_resultBlob[j] = temp111;
                                }
                            }
                        }
                    }


                    QueueResultGridUpdate();


                    toolPar.ResultPar.位置.Clear();
                    for (int i = 0; i < L_resultBlob.Count; i++)
                    {
                        XYU xyu = new XYU();
                        xyu.Point.X = L_resultBlob[i].Row;
                        xyu.Point.Y = L_resultBlob[i].Col;
                        xyu.U = 0;
                        toolPar.ResultPar.位置.Add(xyu);
                    }
                    toolPar.ResultPar.斑点数 = toolPar.ResultPar.位置.Count;
                    if (toolPar.ResultPar.斑点数 == 1)
                        toolPar.ResultPar.结果 = "OK";
                    else
                        toolPar.ResultPar.结果 = "NG";

                    if (!runTool)
                    {
                        HObject runtimeDisplaySearchRegion = null;
                        if (searchRegionType != RegionType.AllImage && displaySearchRegion)
                        {
                            runtimeDisplaySearchRegion = searchRegionType == RegionType.InputRegion
                                ? toolPar.InputPar.搜索区域
                                : runtimeSearchRegion;
                        }

                        // 每轮流程仅由第一个成功的斑点工具重绘背景。
                        // 后续斑点工具只投递叠加层，不再清屏。
                        QueueRuntimeDisplay(
                            clearMainImageBeforeDraw ? toolPar.InputPar.图像 : null,
                            runtimeDisplaySearchRegion,
                            displayRegion ? resultRegion : null,
                            runtimeCircleOverlay,
                            runtimeCrossOverlay);
                    }

                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
            finally
            {
                DisposeDisplayObject(runtimeCircleOverlay);
                DisposeDisplayObject(runtimeCrossOverlay);
            }
        }
        public ToolPar toolPar = new ToolPar();




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

            private HObject _搜索区域;

            public HObject 搜索区域
            {
                get { return _搜索区域; }
                set { _搜索区域 = value; }
            }

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
            private List<XYU> _位置 = new List<XYU>();

            public List<XYU> 位置
            {
                get { return _位置; }
                set { _位置 = value; }
            }

            private int _斑点数;

            public int 斑点数
            {
                get { return _斑点数; }
                set { _斑点数 = value; }
            }
            private string _结果 = string.Empty;

            public string 结果
            {
                get { return _结果; }
                set { _结果 = value; }
            }
        }

    }





}
