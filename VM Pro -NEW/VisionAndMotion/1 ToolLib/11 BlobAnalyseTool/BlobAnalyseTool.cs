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
        /// 清除搜索区域
        /// </summary>
        internal void ClearSearchRegion()
        {
            try
            {

                SearchRegion = null;
                searchRegionType = RegionType.AllImage;
                Frm_BlobAnalyseTool.Instance.cbx_searchRegionType.TextStr = "整幅图像";
                GetImageWindowControl().hwc_imageWindow.HobjectToHimage(toolPar.InputPar.图像);
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
                        HOperatorSet.ClearWindow(GetImageWindowControl().WindowHandle);
                        HOperatorSet.DispObj(GetImageWindowControl().currentImage, GetImageWindowControl().WindowHandle);
                        SearchRegion = null;
                        searchRegionType = RegionType.InputRegion;
                        break;
                }
                Frm_BlobAnalyseTool.Instance.regions = this.L_regions;
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
        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage, bool runTool  , string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;

                    //判断是否有图像输入
                    if (toolPar.InputPar.图像 == null)
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

                    //截取出搜索区域图像
                    if (searchRegionType != RegionType.AllImage)
                    {
                        HOperatorSet.ReduceDomain(toolPar.InputPar.图像, SearchRegion, out   searchRegionImage);
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
                    if (searchRegionType != RegionType.AllImage && displaySearchRegion)
                    {
                        if (searchRegionType == RegionType.InputRegion)
                        {
                            GetImageWindowControl().hwc_imageWindow.DispObj(toolPar.InputPar.搜索区域, "blue");
                        }
                        else
                        {
                            if (Frm_BlobAnalyseTool.Instance.Visible)
                                Frm_BlobAnalyseTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                            else
                                GetImageWindowControl().hwc_imageWindow.DispObj(SearchRegion, "blue");
                        }
                    }

                    if (displayRegion)
                    {
                        if (regionDrawMode == FillMode.Fill)
                        {
                            SetDraw("fill");
                        }
                        else
                        {
                            SetDraw("margin");
                        }
                        if (runTool)
                            Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(resultRegion, "green");
                        else
                        {
                            SetLineWidth(1);
                            ShowObj(resultRegion, "green");
                        }
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
                        if (displayOutCircle)
                        {
                            if (outCircleDrawMode == FillMode.Fill)
                                HOperatorSet.SetDraw(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, "fill");
                            else
                                HOperatorSet.SetDraw(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, "margin");
                            HTuple row2, col2, radius2;
                            HOperatorSet.SmallestCircle(region, out row2, out col2, out radius2);
                            HObject circle;
                            HOperatorSet.GenCircle(out circle, row2, col2, radius2);
                            if (runTool)
                                Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(circle, "green");
                            else
                                GetImageWindowControl().hwc_imageWindow.DispObj(circle, "green");
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
                        if (displayCross)
                        {
                            HObject cross;
                            HOperatorSet.GenCrossContourXld(out cross, row3, col3, 20, 0);
                            if (runTool)
                                Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(cross, "blue");
                            else
                                GetImageWindowControl().hwc_imageWindow.DispObj(cross, "blue");
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


                    if (Frm_BlobAnalyseTool.Instance.Visible)
                    {
                        Frm_BlobAnalyseTool.Instance.dgv_result.Rows.Clear();
                        for (int i = 0; i < L_resultBlob.Count; i++)
                        {
                            int index = Frm_BlobAnalyseTool.Instance.dgv_result.Rows.Add();
                            Frm_BlobAnalyseTool.Instance.dgv_result.Rows[index].Cells[0].Value = i + 1;
                            Frm_BlobAnalyseTool.Instance.dgv_result.Rows[index].Cells[1].Value = L_resultBlob[i].Area;
                            Frm_BlobAnalyseTool.Instance.dgv_result.Rows[index].Cells[2].Value = L_resultBlob[i].Row;
                            Frm_BlobAnalyseTool.Instance.dgv_result.Rows[index].Cells[3].Value = L_resultBlob[i].Col;
                            Frm_BlobAnalyseTool.Instance.dgv_result.Rows[index].Cells[4].Value = L_resultBlob[i].CircumcircleRadius;
                        }
                    }


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

                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
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
