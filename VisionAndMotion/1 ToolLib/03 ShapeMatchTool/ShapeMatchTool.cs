using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using HalconDotNet;
using ViewWindow.Model;
using System.Reflection;
using System.ComponentModel;
using VMPro.Properties;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace VMPro
{
    [Serializable]
    internal class MatchTool : ToolBase
    {
        public MatchTool()
        {
            //单元大小分别为：6,16,26,36
            HOperatorSet.GenEmptyObj(out brush_region);
            HOperatorSet.GenEmptyObj(out  final_region);
            HOperatorSet.GenEmptyObj(out brush_region111);
            HOperatorSet.GenEmptyObj(out  final_region111);
        }
        public ToolPar toolPar = new ToolPar();
        internal void ShowStandardImage()
        {
            HObject displayImage = GetTemplateDisplayImage();
            if (displayImage == null)
            {
                Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                Frm_ShapeMatchTool.Instance.label4.Text = "状态：未指定输入图像";
                return;
            }
            Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(displayImage);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;

            HTuple row, col, row1, col1;
            HOperatorSet.GetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row, out col, out row1, out col1);
            DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "训练图像", 12, row + (row1 - row) / 30, col + (col1 - col) / 30, "blue", "false");
        }
        internal double minScale = 0.8;
        internal double maxScale = 1.2;
        internal MatchMode matchMode = MatchMode.BasedShape;
        /// <summary>
        /// 刷新输出
        /// </summary>
        /// <param name="toolName">工具名称</param>
        internal void UpdateOutput(string toolName)
        {
            try
            {
                List<ToolIO> L_toolIO = Job.FindJobByName(jobName).FindToolInfoByName(toolName).output;
                for (int i = 0; i < L_toolIO.Count; i++)
                {
                    string outputItem = L_toolIO[i].IOName;
                    string[] items = Regex.Split(outputItem, " . ");
                    object value = toolPar;
                    value = GetValue(value, "ResultPar");
                    for (int j = 0; j < items.Length; j++)
                    {
                        value = GetValue(value, items[j]);
                    }
                    Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value = value;
                    Job.FindJobByName(jobName).GetToolIONodeByNodeText(toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 是否显示匹配到的模板
        /// </summary>
        internal bool showTemplate = true;
        /// <summary>
        /// 是否显示中心十字架
        /// </summary>
        internal bool showCross = true;
        /// <summary>
        /// 是否显示特征
        /// </summary>
        internal bool showFeature = true;
        /// <summary>
        /// 是否显示匹配结果的外接框
        /// </summary>
        [OptionalField]
        internal bool showMatchBox = true;

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            // 模板模型句柄在软件重启后会失效，保存工程前将模型序列化为数据存入字段，
            // 加载工程时再反序列化重建句柄，实现模板跨会话保存。
            serializedModelData = null;
            try
            {
                if (modelID != null && modelID.Length > 0 && modelID.I != -1)
                {
                    HTuple data;
                    if (matchMode == MatchMode.BasedShape)
                        HOperatorSet.SerializeShapeModel(modelID, out data);
                    else
                        HOperatorSet.SerializeNccModel(modelID, out data);
                    serializedModelData = data;

                    // 同时保存模板显示图，重启后打开模板匹配窗口可直接显示模板区域与轮廓
                    templateDisplayImage = null;
                    try
                    {
                        if (toolPar != null && toolPar.InputPar != null && toolPar.InputPar.图像 != null)
                        {
                            if (totalRegion != null && totalRegion.IsInitialized())
                                HOperatorSet.ReduceDomain(toolPar.InputPar.图像, totalRegion, out templateDisplayImage);
                            else
                                templateDisplayImage = toolPar.InputPar.图像;
                        }
                    }
                    catch
                    {
                        templateDisplayImage = null;
                    }
                }
            }
            catch (Exception)
            {
                serializedModelData = null;
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            // 旧工程没有 showMatchBox 字段，加载时仍按新的默认显示逻辑处理。
            showMatchBox = true;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // 模板和搜索区参数需要保留，但上次运行的输入图不能作为新会话输入。
            if (toolPar != null && toolPar.InputPar != null)
                toolPar.InputPar.图像 = null;
            reducedImage = null;

            // 从序列化数据重建模板模型句柄（跨会话句柄失效）
            try
            {
                if (serializedModelData != null)
                {
                    if (matchMode == MatchMode.BasedShape)
                        HOperatorSet.DeserializeShapeModel(serializedModelData, out modelID);
                    else
                        HOperatorSet.DeserializeNccModel(serializedModelData, out modelID);
                }
                else
                {
                    modelID = -1;   // 旧工程未保存模型数据，标记为未创建模板
                }
            }
            catch (Exception)
            {
                modelID = -1;
            }
        }
        /// <summary>
        /// 显示结果序号
        /// </summary>
        internal bool showIndex = true;
        internal bool showSearchRegion = true;
        /// <summary>
        /// 模板句柄                                                   
        /// </summary>
        internal HTuple modelID = -1;
        /// <summary>
        /// 模板模型序列化数据（保存工程时写入，加载时重建句柄，实现模板跨会话保存）
        /// </summary>
        internal HTuple serializedModelData;
        /// <summary>
        /// 模板显示图（保存工程时写入，重启后用于模板匹配窗口显示模板区域与轮廓）
        /// </summary>
        internal HObject templateDisplayImage;
        /// <summary>
        /// 行列间隔像素数
        /// </summary>
        internal int spanPixelNum = 100;
        /// <summary>
        /// 排序模式
        /// </summary>
        internal SortMode sortMode = SortMode.从上至下且从左至右;
        /// <summary>
        /// 模板区域
        /// </summary>
        internal HObject templateRegion;
        internal HObject totalRegion;
        /// <summary>
        /// 搜索区域图像
        /// </summary>
        internal HObject reducedImage;
        /// <summary>
        /// 最小匹配分数
        /// </summary>
        internal double minScore = 0.5;
        /// <summary>
        /// 匹配个数
        /// </summary>
        internal int matchNum = 1;
        /// <summary>
        /// 起始角度
        /// </summary>
        internal int startAngle = -30;
        /// <summary>
        /// 结束角度（界面第二个角度输入框）
        /// </summary>
        internal int angleRange = 30;
        /// <summary>
        /// 角度步长
        /// </summary>
        internal int angleStep = 1;
        /// <summary>
        /// 对比度
        /// </summary>
        internal int contrast = 30;
        /// <summary>
        /// 极性
        /// </summary>
        internal string polarity = "use_polarity";

        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 模板匹配结果
        /// </summary>
        internal List<XYU> L_result = new List<XYU>();
        internal HObject brush_region;
        internal HObject final_region;





        internal HObject brush_region111;
        internal HObject final_region111;
        /// <summary>
        /// 搜索区域类型
        /// </summary>
        internal RegionType searchRegionType = RegionType.AllImage;
        /// <summary>
        /// 搜索区域
        /// </summary>
        internal List<ViewWindow.Model.ROI> L_regions = new List<ROI>();
        /// <summary>
        /// 可编辑模板区域。正区域用于添加模板，负区域用于扣除模板。
        /// </summary>
        internal List<ViewWindow.Model.ROI> templateROIs = new List<ROI>();
        /// <summary>
        /// 搜索区域点数据，如Rectangle1的左上点行列坐标和右下点行列坐标，需要存储下来，在重绘区域时用
        /// </summary>
        internal List<double> searchRegionPointData = new List<double>();
        /// <summary>
        /// 搜索区域
        /// </summary>
        private HObject _searchRegion;
        internal HObject SearchRegion
        {
            get
            {
                if (L_regions != null && L_regions.Count > 0)
                    _searchRegion = L_regions[0].getRegion();
                return _searchRegion;
            }
            set { _searchRegion = value; }
        }

        /// <summary>
        /// 明确切换为整幅图像搜索，同时清掉旧 ROI 和旧的缓存搜索区域。
        /// </summary>
        internal void UseAllImageSearchRegion(bool refreshDisplay)
        {
            L_regions.Clear();
            searchRegionPointData.Clear();
            _searchRegion = null;
            reducedImage = null;
            searchRegionType = RegionType.AllImage;
            Frm_ShapeMatchTool.Instance.regions = L_regions;

            if (refreshDisplay && toolPar.InputPar.图像 != null)
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
        }

        /// <summary>
        /// 界面保存的是起始角度和结束角度；HALCON 接收起始角度和角度跨度。
        /// </summary>
        private HTuple GetMatchAngleExtentRad()
        {
            double extent = angleRange - startAngle;
            if (extent < 0)
                extent += 360.0;
            if (extent > 360.0)
                extent = 360.0;
            return ((HTuple)extent).TupleRad();
        }

        /// <summary>
        /// 模型学习阶段固定生成完整旋转范围；运行阶段再按界面的起始/结束角度限制搜索。
        /// 这样扩大搜索角度时不依赖模型最初学习到的单侧角度范围。
        /// </summary>
        private HTuple GetModelAngleStartRad()
        {
            return ((HTuple)(-180.0)).TupleRad();
        }

        private HTuple GetModelAngleExtentRad()
        {
            return ((HTuple)360.0).TupleRad();
        }

        private HObject GetTemplateDisplayImage()
        {
            // 优先使用跨会话保存的模板显示图（重启后输入图尚未由上游传入时，也能显示模板）
            if (templateDisplayImage != null && templateDisplayImage.IsInitialized())
                return templateDisplayImage;
            return toolPar.InputPar.图像;
        }

        internal void RefreshEditableTemplateDisplay()
        {
            try
            {
                HObject displayImage = GetTemplateDisplayImage();
                if (displayImage == null)
                    return;

                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(displayImage);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                RebuildTemplateRoisInWindow();
                SyncTemplateRegionFromEditableRois();
                if (templateRegion != null)
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal void SyncTemplateRegionFromEditableRois()
        {
            try
            {
                if (templateROIs == null || templateROIs.Count == 0)
                {
                    templateRegion = null;
                    totalRegion = null;
                    return;
                }

                HRegion addRegion = new HRegion();
                HRegion subRegion = new HRegion();
                HRegion roiRegion;
                HTuple area, row, col;
                addRegion.GenEmptyRegion();
                subRegion.GenEmptyRegion();

                for (int i = 0; i < templateROIs.Count; i++)
                {
                    roiRegion = templateROIs[i].getRegion();
                    if (roiRegion == null)
                        continue;

                    if (templateROIs[i].getOperatorFlag() == ROI.NEGATIVE_FLAG)
                        subRegion = subRegion.Union2(roiRegion);
                    else
                        addRegion = addRegion.Union2(roiRegion);
                }

                HOperatorSet.AreaCenter(addRegion, out area, out row, out col);
                if (area <= 0)
                {
                    templateRegion = null;
                    totalRegion = null;
                    return;
                }

                HRegion modelRegion = addRegion.Difference(subRegion);
                HOperatorSet.AreaCenter(modelRegion, out area, out row, out col);
                if (area > 0)
                    templateRegion = modelRegion;
                else
                {
                    templateRegion = null;
                    totalRegion = null;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal bool HasValidTemplateRegion()
        {
            try
            {
                SyncTemplateRegionFromEditableRois();
                if (templateRegion == null)
                    return false;

                HTuple area, row, col;
                HOperatorSet.AreaCenter(templateRegion, out area, out row, out col);
                return area > 0;
            }
            catch
            {
                return false;
            }
        }

        internal bool UndoLastTemplateRegion()
        {
            try
            {
                if (templateROIs == null || templateROIs.Count == 0)
                    return false;

                templateROIs.RemoveAt(templateROIs.Count - 1);
                SyncTemplateRegionFromEditableRois();
                RefreshEditableTemplateDisplay();
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }

        private void RebuildTemplateRoisInWindow()
        {
            try
            {
                if (templateROIs == null || templateROIs.Count == 0)
                    return;

                List<ROI> oldRois = new List<ROI>(templateROIs);
                List<ROI> rebuiltRois = new List<ROI>();

                for (int i = 0; i < oldRois.Count; i++)
                {
                    int roiCountBeforeAdd = rebuiltRois.Count;
                    int operatorFlag = oldRois[i].getOperatorFlag();
                    switch (oldRois[i].Type)
                    {
                        case "ROIRectangle1":
                            HTuple rectangle1Data = oldRois[i].getModelData();
                            if (rectangle1Data == null)
                                continue;
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genRect1(rectangle1Data[0].D, rectangle1Data[1].D, rectangle1Data[2].D, rectangle1Data[3].D, ref rebuiltRois);
                            break;
                        case "ROIRectangle2":
                            HTuple rectangle2Data = oldRois[i].getModelData();
                            if (rectangle2Data == null)
                                continue;
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genRect2(rectangle2Data[0].D, rectangle2Data[1].D, rectangle2Data[2].D, rectangle2Data[3].D, rectangle2Data[4].D, ref rebuiltRois);
                            break;
                        case "ROICircle":
                            HTuple circleData = oldRois[i].getModelData();
                            if (circleData == null)
                                continue;
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genCircle(circleData[0].D, circleData[1].D, circleData[2].D, ref rebuiltRois);
                            break;
                        case "ROINurbs":
                            HTuple rows, cols;
                            oldRois[i].getModelData(out rows, out cols);
                            if (rows == null || cols == null || rows.Length == 0 || cols.Length == 0)
                                continue;
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genNurbs(rows, cols, ref rebuiltRois);
                            break;
                    }

                    if (rebuiltRois.Count > roiCountBeforeAdd)
                    {
                        rebuiltRois[rebuiltRois.Count - 1].setOperatorFlag(operatorFlag);
                        rebuiltRois[rebuiltRois.Count - 1].Color = operatorFlag == ROI.NEGATIVE_FLAG ? "red" : "green";
                    }
                }

                templateROIs = rebuiltRois;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void SetTemplateRoiOperator(ROI roi)
        {
            if (roi == null)
                return;

            if (Frm_ShapeMatchTool.Instance.rdo_templateRegionSub.Checked)
            {
                roi.setOperatorFlag(ROI.NEGATIVE_FLAG);
                roi.Color = "red";
            }
            else
            {
                roi.setOperatorFlag(ROI.POSITIVE_FLAG);
                roi.Color = "green";
            }
        }

        private void FinishTemplateRegionDraw(Button activeButton)
        {
            try
            {
                if (activeButton != null)
                    activeButton.BackColor = Color.FromArgb(46, 141, 230);
                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal void DrawPen(object sender)
        {
            try
            {
                string brushType = ((Button)sender).Name;//笔刷类型
                if (brushType == "button9")
                    brushType = "矩形笔刷";
                else
                    brushType = "圆形笔刷";

                HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null, hv_Column2 = null;

                HObject ho_temp_brush = new HObject();

                try
                {
                    //画图模式 开
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.Focus();
                    //锁住功能区
                    //////groupBox_tool.Enabled = false;
                    //显示提示
                    //////hWindow_Final1.ClearWindow();
                    //////hWindow_Final1.HobjectToHimage(image);
                    HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 15, "sans", new HTuple("true"), new HTuple("false"));
                    HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "在窗口画出" + brushType + ",点击右键结束", "window", 20, 20, "blue", "false");

                    //显示为黄色
                    HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "green");

                    //
                    switch (brushType)
                    {
                        case "矩形笔刷":
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = null;
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                            HOperatorSet.DrawRectangle1(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out hv_Row1, out hv_Column1, out hv_Row2,
                               out hv_Column2);
                            ho_temp_brush.Dispose();
                            HOperatorSet.GenRectangle1(out ho_temp_brush, hv_Row1, hv_Column1, hv_Row2,
                                hv_Column2);
                            //
                            if (hv_Row1.D != 0)
                            {
                                brush_region.Dispose();
                                brush_region = ho_temp_brush;
                            }
                            else
                            {

                                //////hWindow_Final1.HobjectToHimage(shapeMatchTool.inputImage);
                                HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 20, "sans", new HTuple("true"), new HTuple("false"));
                                HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "未画出有效区域", "window", 20, 20, "red", "false");
                                return;
                            }
                            break;
                        case "矩形2":
                            HTuple phi, lenght1, length2;
                            HOperatorSet.DrawRectangle2(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out hv_Row1, out hv_Column1, out phi, out lenght1, out length2);

                            ho_temp_brush.Dispose();
                            HOperatorSet.GenRectangle2(out ho_temp_brush, hv_Row1, hv_Column1, phi, lenght1, length2);
                            //
                            if (hv_Row1.D != 0)
                            {
                                brush_region.Dispose();
                                brush_region = ho_temp_brush;
                            }
                            else
                            {
                                //////hWindow_Final1.HobjectToHimage(shapeMatchTool.inputImage);
                                HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 20, "sans", new HTuple("true"), new HTuple("false"));
                                HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "未画出有效区域", "window", 20, 20, "red", "false");
                                return;
                            }
                            break;
                        case "圆形笔刷":
                            HTuple radius;
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = null;
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                            HOperatorSet.DrawCircle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out hv_Row1, out hv_Column1, out radius);

                            ho_temp_brush.Dispose();
                            HOperatorSet.GenCircle(out ho_temp_brush, hv_Row1, hv_Column1, radius);
                            //
                            if (hv_Row1.D != 0)
                            {
                                brush_region.Dispose();
                                brush_region = ho_temp_brush;
                            }
                            else
                            {
                                //////hWindow_Final1.HobjectToHimage(shapeMatchTool.inputImage);
                                HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 20, "sans", new HTuple("true"), new HTuple("false"));
                                HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "未画出有效区域", "window", 20, 20, "red", "false");
                                return;
                            }

                            break;
                        default:
                            MessageBox.Show("错误指令");
                            return;
                    }
                    //

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    HObject displayImage = GetTemplateDisplayImage();
                    if (displayImage != null)
                        Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(displayImage);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                    HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 15, "sans", new HTuple("true"), new HTuple("false"));
                    HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, brushType + " 笔刷创建成果", "window", 20, 20, "blue", "false");
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(ho_temp_brush, "yellow");
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
                    //解锁功能区
                    //////groupBox_tool.Enabled = true;
                    Thread.Sleep(1000);
                    if (displayImage != null)
                        Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(displayImage);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                }
                //////Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        public bool drawMode = false;
        internal void Work(object sender)
        {
            try
            {
                string actionType = ((RadioButton)sender).Name;// 画区域 或者 擦除
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = null;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.Focus();
                //////groupBox_tool.Enabled = false;

                HTuple hv_Button = null;
                HTuple hv_Row = null, hv_Column = null;
                HTuple areaBrush, rowBrush, columnBrush, homMat2D;


                HObject brush_region_affine = new HObject();
                HObject ho_Image = new HObject(toolPar.InputPar.图像);
                try
                {
                    if (!brush_region.IsInitialized())
                    {
                        MessageBox.Show("请先设置笔刷");
                        return;
                    }
                    else
                    {
                        HOperatorSet.AreaCenter(brush_region, out areaBrush, out rowBrush, out columnBrush);
                    }

                    //显示
                    //////hWindow_Final1.HobjectToHimage(shapeMatchTool.inputImage);


                    HTuple row1, col1, row2, col2;
                    HOperatorSet.SmallestRectangle1(templateRegion, out row1, out col1, out row2, out col2);
                    HObject outRectangle1;
                    HOperatorSet.GenRectangle1(out outRectangle1, row1 - 20, col1 - 20, row2 + 20, col2 + 20);

                    int size = ((row2 - row1) + (col2 - col1)) / 2;

                    HWindowControl temp = new HWindowControl();
                    HTuple w, h;
                    HOperatorSet.GetImageSize(toolPar.InputPar.图像, out w, out h);
                    HOperatorSet.SetWindowExtents(temp.HalconWindow, 0, 0, w, h);
                    HOperatorSet.SetPart(temp.HalconWindow, 0, 0, h - 1, w - 1);
                    HOperatorSet.ClearWindow(temp.HalconWindow);
                    HOperatorSet.DispObj(toolPar.InputPar.图像, temp.HalconWindow);
                    HOperatorSet.SetLineStyle(temp.HalconWindow, new HTuple());
                    HOperatorSet.SetColor(temp.HalconWindow, "green");
                    if (size < 200)
                        HOperatorSet.SetLineWidth(temp.HalconWindow, 1);
                    else
                        HOperatorSet.SetLineWidth(temp.HalconWindow, 2);
                    HOperatorSet.DispObj(contour, temp.HalconWindow);
                    HObject image;
                    HOperatorSet.DumpWindowImage(out image, temp.HalconWindow);
                    HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    HOperatorSet.DispObj(image, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(image);
                    //   return;

                    ho_Image = new HObject(image);





                    HObject imageReduced;
                    HOperatorSet.ReduceDomain(image, outRectangle1, out imageReduced);
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.ClearWindow();
                    HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, row1 - 20, col1 - 20, row2 + 20, col2 + 20);
                    HOperatorSet.DispObj(imageReduced, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    //////HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));
                    //////HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                    //////HOperatorSet.DispObj(templateRegion, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    //////HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("orange"));
                    //////HOperatorSet.DispObj(contour, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    Application.DoEvents();


                    try
                    {
                        HObject image1, image2, resultImage;
                        resultImage = new HObject();
                        HOperatorSet.PaintRegion(final_region, imageReduced, out  image1, 10, "fill");
                        HOperatorSet.Compose3(imageReduced, image1, image1, out resultImage);
                        HOperatorSet.DispObj(resultImage, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                        HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                        HOperatorSet.DispObj(final_region, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    }
                    catch { }



                    //Test();

                    Application.DoEvents();


                    //Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(final_region);

                    //画出笔刷
                    switch (actionType)
                    {
                        case "radioButton2":
                            HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "blue");

                            break;
                        case "radioButton3":
                            HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "red");

                            //检查final_region是否有效
                            if (!final_region.IsInitialized())
                            {
                                MessageBox.Show("请先使用画出合适区域,在使用擦除功能");
                                return;
                            }
                            break;
                        default:
                            MessageBox.Show("设置错误");
                            return;
                    }


                    #region "循环,等待涂抹"

                    //鼠标状态
                    hv_Button = 0;
                    // 4为鼠标右键
                    while (drawMode)
                    {
                        //一直在循环,需要让halcon控件也响应事件,不然到时候跳出循环,之前的事件会一起爆发触发,
                        Application.DoEvents();

                        hv_Row = -1;
                        hv_Column = -1;

                        //获取鼠标坐标
                        try
                        {
                            HOperatorSet.GetMposition(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out hv_Row, out hv_Column, out hv_Button);
                        }
                        catch (HalconException ex)
                        {
                            try
                            {
                                hv_Button = 0;
                                HOperatorSet.DispObj(image, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                HOperatorSet.DispObj(final_region, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                HObject image1, image2, resultImage;
                                resultImage = new HObject();
                                HObject iamgeR, iamgeG, iamgeB;
                                HOperatorSet.Decompose3(image, out iamgeR, out  iamgeG, out  iamgeB);
                                HOperatorSet.PaintRegion(final_region, iamgeR, out  image1, 20, "fill");
                                HOperatorSet.Compose3(imageReduced, iamgeG, image1, out resultImage);
                                HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, row1 - 20, col1 - 20, row2 + 20, col2 + 20);

                                HOperatorSet.DispObj(resultImage, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                                HOperatorSet.DispObj(final_region, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "涂抹结束后请重新学习", "window", 20, 20, "blue", "false");
                            }
                            catch
                            { }
                        }


                        HOperatorSet.SetSystem("flush_graphic", "false");
                        HOperatorSet.DispObj(ho_Image, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                        if (final_region.IsInitialized())
                        {
                            try
                            {
                                //////HOperatorSet.DispObj(final_region, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                HObject image1, image2, resultImage;
                                resultImage = new HObject();
                                HObject iamgeR, iamgeG, iamgeB;
                                HOperatorSet.Decompose3(image, out iamgeR, out  iamgeG, out  iamgeB);
                                HOperatorSet.PaintRegion(final_region, iamgeR, out  image1, 20, "fill");
                                HOperatorSet.Compose3(imageReduced, iamgeG, image1, out resultImage);
                                HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, row1 - 20, col1 - 20, row2 + 20, col2 + 20);

                                HOperatorSet.DispObj(resultImage, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                                HOperatorSet.DispObj(final_region, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                            }
                            catch { }
                        }



                        //check if mouse cursor is over window
                        if (hv_Row >= 0 && hv_Column >= 0)
                        {
                            //放射变换
                            HOperatorSet.VectorAngleToRigid(rowBrush, columnBrush, 0, hv_Row, hv_Column, 0, out  homMat2D);
                            brush_region_affine.Dispose();
                            HOperatorSet.AffineTransRegion(brush_region, out brush_region_affine, homMat2D, "nearest_neighbor");
                            HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                            HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                            HOperatorSet.DispObj(brush_region_affine, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);

                            HOperatorSet.SetSystem("flush_graphic", "true");
                            HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 15, "sans", new HTuple("true"), new HTuple("false"));
                            HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "按下鼠标左键开始涂抹", "window", 20, 20, "blue", "false");
                            // disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "按下鼠标左键涂画,右键结束", 20, 20, "red", "false");
                            //  Test();

                            //1为鼠标左键
                            if (hv_Button == 1)
                            {

                                //画出笔刷
                                switch (actionType)
                                {
                                    case "radioButton2":
                                        {
                                            if (final_region.IsInitialized())
                                            {
                                                HObject ExpTmpOutVar_0;
                                                HOperatorSet.Union2(final_region, brush_region_affine, out ExpTmpOutVar_0);
                                                final_region.Dispose();
                                                final_region = ExpTmpOutVar_0;
                                            }
                                            else
                                            {
                                                final_region = new HObject(brush_region_affine);
                                            }

                                        }
                                        break;
                                    case "radioButton3":
                                        {
                                            HObject ExpTmpOutVar_0;
                                            HOperatorSet.Difference(final_region, brush_region_affine, out ExpTmpOutVar_0);
                                            final_region.Dispose();
                                            final_region = ExpTmpOutVar_0;
                                        }
                                        break;
                                    default:
                                        MessageBox.Show("设置错误");
                                        return;
                                }//end switch

                            }//end if
                        }
                        else
                        {
                            HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 15, "sans", new HTuple("true"), new HTuple("false"));
                            HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "请将鼠标移动到窗口内部", "window", 20, 20, "blue", "false");
                        }


                    }//end while
                    #endregion
                }
                catch (HalconException HDevExpDefaultException)
                {
                    throw HDevExpDefaultException;
                }
                finally
                {
                    //////hWindow_Final1.HobjectToHimage(shapeMatchTool.inputImage);
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(final_region, "blue");
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;

                    //////groupBox_tool.Enabled = true;

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal void WorkCreateModel(object sender)
        {
            try
            {
                string actionType = ((RadioButton)sender).Name;// 画区域 或者 擦除
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = null;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.Focus();
                //////groupBox_tool.Enabled = false;

                HTuple hv_Button = null;
                HTuple hv_Row = null, hv_Column = null;
                HTuple areaBrush, rowBrush, columnBrush, homMat2D;


                HObject brush_region_affine = new HObject();
                HObject ho_Image = new HObject(toolPar.InputPar.图像);
                try
                {
                    if (!brush_region111.IsInitialized())
                    {
                        MessageBox.Show("请先设置笔刷");
                        return;
                    }
                    else
                    {
                        HOperatorSet.AreaCenter(brush_region111, out areaBrush, out rowBrush, out columnBrush);
                    }

                    //显示
                    //////hWindow_Final1.HobjectToHimage(shapeMatchTool.inputImage);


                    //HTuple row1, col1, row2, col2;
                    //HOperatorSet.SmallestRectangle1(templateRegion, out row1, out col1, out row2, out col2);
                    //HObject outRectangle1;
                    //HOperatorSet.GenRectangle1(out outRectangle1, row1 - 20, col1 - 20, row2 + 20, col2 + 20);

                    //int size = ((row2 - row1) + (col2 - col1)) / 2;

                    //HWindowControl temp = new HWindowControl();
                    //HTuple w, h;
                    //HOperatorSet.GetImageSize(standardImage, out w, out h);
                    //HOperatorSet.SetWindowExtents(temp.HalconWindow, 0, 0, w, h);
                    //HOperatorSet.SetPart(temp.HalconWindow, 0, 0, h - 1, w - 1);
                    //HOperatorSet.ClearWindow(temp.HalconWindow);
                    //HOperatorSet.DispObj(standardImage, temp.HalconWindow);
                    //HOperatorSet.SetLineStyle(temp.HalconWindow, new HTuple());
                    //HOperatorSet.SetColor(temp.HalconWindow, "green");
                    //if (size < 200)
                    //    HOperatorSet.SetLineWidth(temp.HalconWindow, 1);
                    //else
                    //    HOperatorSet.SetLineWidth(temp.HalconWindow, 2);
                    //HOperatorSet.DispObj(contour, temp.HalconWindow);
                    //HObject image;
                    //HOperatorSet.DumpWindowImage(out image, temp.HalconWindow);
                    //HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    //HOperatorSet.DispObj(image, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    //Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(image);
                    //   return;

                    // ho_Image = new HObject(image);





                    //HObject imageReduced;
                    //HOperatorSet.ReduceDomain(image, outRectangle1, out imageReduced);
                    //Frm_ShapeMatchTool.Instance.hWindow_Final1.ClearWindow();
                    //HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, row1 - 20, col1 - 20, row2 + 20, col2 + 20);
                    //HOperatorSet.DispObj(imageReduced, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    ////////HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));
                    ////////HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                    ////////HOperatorSet.DispObj(templateRegion, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    ////////HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("orange"));
                    ////////HOperatorSet.DispObj(contour, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    //Application.DoEvents();


                    try
                    {
                        //HObject image1, image2, resultImage;
                        //resultImage = new HObject();
                        //HOperatorSet.PaintRegion(final_region111, inputImage, out  image1, 10, "fill");
                        //HOperatorSet.Compose3(inputImage, image1, image1, out resultImage);
                        //HOperatorSet.DispObj(resultImage, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                        //HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                        //HOperatorSet.DispObj(final_region111, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    }
                    catch { }



                    //Test();

                    Application.DoEvents();


                    //Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(final_region);

                    //画出笔刷
                    switch (actionType)
                    {
                        case "radioButton4":
                            HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "blue");

                            break;
                        case "radioButton3":
                            HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "red");

                            //检查final_region是否有效
                            if (!final_region111.IsInitialized())
                            {
                                MessageBox.Show("请先使用画出合适区域,在使用擦除功能");
                                return;
                            }
                            break;
                        default:
                            MessageBox.Show("设置错误");
                            return;
                    }


                    #region "循环,等待涂抹"

                    //鼠标状态
                    hv_Button = 0;
                    // 4为鼠标右键
                    while (drawMode)
                    {
                        //一直在循环,需要让halcon控件也响应事件,不然到时候跳出循环,之前的事件会一起爆发触发,
                        Application.DoEvents();

                        hv_Row = -1;
                        hv_Column = -1;

                        //获取鼠标坐标
                        try
                        {
                            HOperatorSet.GetMposition(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out hv_Row, out hv_Column, out hv_Button);
                        }
                        catch (HalconException ex)
                        {
                            try
                            {
                                hv_Button = 0;
                                //HOperatorSet.DispObj(image, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                //HOperatorSet.DispObj(final_region111, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                //HObject image1, image2, resultImage;
                                //resultImage = new HObject();
                                //HObject iamgeR, iamgeG, iamgeB;
                                //HOperatorSet.Decompose3(image, out iamgeR, out  iamgeG, out  iamgeB);
                                //HOperatorSet.PaintRegion(final_region111, iamgeR, out  image1, 20, "fill");
                                //HOperatorSet.Compose3(imageReduced, iamgeG, image1, out resultImage);
                                //HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, row1 - 20, col1 - 20, row2 + 20, col2 + 20);

                                ////HOperatorSet.DispObj(resultImage, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                                HOperatorSet.DispObj(final_region111, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "涂抹结束后请重新学习", "window", 20, 20, "blue", "false");
                            }
                            catch
                            { }
                        }


                        HOperatorSet.SetSystem("flush_graphic", "false");
                        HOperatorSet.DispObj(ho_Image, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                        if (final_region111.IsInitialized())
                        {
                            try
                            {
                                //////HOperatorSet.DispObj(final_region, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                //HObject image1, image2, resultImage;
                                //resultImage = new HObject();
                                //HObject iamgeR, iamgeG, iamgeB;
                                //HOperatorSet.Decompose3(image, out iamgeR, out  iamgeG, out  iamgeB);
                                //HOperatorSet.PaintRegion(final_region111, iamgeR, out  image1, 20, "fill");
                                //HOperatorSet.Compose3(imageReduced, iamgeG, image1, out resultImage);
                                //HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, row1 - 20, col1 - 20, row2 + 20, col2 + 20);

                                // HOperatorSet.DispObj(resultImage, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                                HOperatorSet.DispObj(final_region111, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                            }
                            catch { }
                        }



                        //check if mouse cursor is over window
                        if (hv_Row >= 0 && hv_Column >= 0)
                        {
                            //放射变换
                            HOperatorSet.VectorAngleToRigid(rowBrush, columnBrush, 0, hv_Row, hv_Column, 0, out  homMat2D);
                            brush_region_affine.Dispose();
                            HOperatorSet.AffineTransRegion(brush_region111, out brush_region_affine, homMat2D, "nearest_neighbor");
                            HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                            HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                            HOperatorSet.DispObj(brush_region_affine, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);

                            HOperatorSet.SetSystem("flush_graphic", "true");
                            HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 15, "sans", new HTuple("true"), new HTuple("false"));
                            HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "按下鼠标左键开始涂抹", "window", 20, 20, "blue", "false");
                            // disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "按下鼠标左键涂画,右键结束", 20, 20, "red", "false");
                            //  Test();

                            //1为鼠标左键
                            if (hv_Button == 1)
                            {

                                //画出笔刷
                                switch (actionType)
                                {
                                    case "radioButton4":
                                        {
                                            if (final_region111.IsInitialized())
                                            {
                                                HObject ExpTmpOutVar_0;
                                                HOperatorSet.Union2(final_region111, brush_region_affine, out ExpTmpOutVar_0);
                                                final_region111.Dispose();
                                                final_region111 = ExpTmpOutVar_0;
                                            }
                                            else
                                            {
                                                final_region111 = new HObject(brush_region_affine);
                                            }

                                        }
                                        break;
                                    case "radioButton3":
                                        {
                                            HObject ExpTmpOutVar_0;
                                            HOperatorSet.Difference(final_region111, brush_region_affine, out ExpTmpOutVar_0);
                                            final_region111.Dispose();
                                            final_region111 = ExpTmpOutVar_0;
                                        }
                                        break;
                                    default:
                                        MessageBox.Show("设置错误");
                                        return;
                                }//end switch

                            }//end if
                        }
                        else
                        {
                            HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 15, "sans", new HTuple("true"), new HTuple("false"));
                            HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "请将鼠标移动到窗口内部", "window", 20, 20, "blue", "false");
                        }


                    }//end while
                    #endregion
                }
                catch (HalconException HDevExpDefaultException)
                {
                    throw HDevExpDefaultException;
                }
                finally
                {
                    //////hWindow_Final1.HobjectToHimage(shapeMatchTool.inputImage);
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(final_region111, "blue");
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;

                    //////groupBox_tool.Enabled = true;

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
                ResetRuntimeData();
                ResetToolUI();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 清理模板匹配运行时数据。不要在这里访问 WinForms/Halcon 窗口控件，按钮可放到后台线程执行。
        /// 注意：必须与 Run() 使用同一把锁(obj)，否则流程线程正在 FindShapeModel 时，
        /// 这里 ClearShapeModel(modelID) 会与流程线程并发操作同一个 HALCON 模型句柄，
        /// 导致原生层访问冲突(AccessViolation)，表现为程序直接闪退。
        /// </summary>
        internal void ResetRuntimeData()
        {
            try
            {
                lock (obj)
                {
                    drawMode = false;
                    HOperatorSet.GenEmptyObj(out final_region);
                    HOperatorSet.GenEmptyObj(out final_region111);
                    templateRegion = null;
                    totalRegion = null;
                    SearchRegion = null;
                    templateROIs = new List<ROI>();
                    reducedImage = null;
                    searchRegionType = RegionType.AllImage;
                    L_regions = new List<ROI>();
                    L_result.Clear();

                    if (modelID != -1)
                        HOperatorSet.ClearShapeModel(modelID);
                    modelID = -1;
                    serializedModelData = null;
                    templateDisplayImage = null;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 重置模板匹配界面。必须在 UI 线程调用。
        /// </summary>
        internal void ResetToolUI()
        {
            Frm_ShapeMatchTool.IsResettingUI = true;
            try
            {
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
                HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                if (toolPar.InputPar.图像 != null)
                {
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispImageFit();
                }

                Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Black;
                Frm_ShapeMatchTool.Instance.label4.Text = "状态：无";

                Frm_ShapeMatchTool.Instance.label3.Text = "耗时：0ms";
                // Frm_ShapeMatchTool.Instance.tbc_shapeMatch.SelectedIndex = 0;

                Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked = true;
                Frm_ShapeMatchTool.Instance.radioButton1.Checked = true;

                Frm_ShapeMatchTool.Instance.nud_minScore.Value = 0.5;
                Frm_ShapeMatchTool.Instance.nud_matchNum.Value = 1;
                Frm_ShapeMatchTool.Instance.ckb_autoContrast.Checked = true;
                Frm_ShapeMatchTool.Instance.nud_angleStart.Value = -30;
                Frm_ShapeMatchTool.Instance.nud_angleRange.Value = 30;

                Frm_ShapeMatchTool.Instance.cbx_searchRegionType.TextStr = string.Empty;
                Frm_ShapeMatchTool.Instance.cbx_showTemplate.Checked = true;
                Frm_ShapeMatchTool.Instance.ckb_showCross.Checked = true;
                Frm_ShapeMatchTool.Instance.ckb_showFeature.Checked = true;
                Frm_ShapeMatchTool.Instance.ckb_showMatchBox.Checked = true;
                Frm_ShapeMatchTool.Instance.checkBox1.Checked = false;

                Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows.Clear();

                Frm_ShapeMatchTool.Instance.radioButton2.Enabled = false;
                Frm_ShapeMatchTool.Instance.radioButton3.Enabled = false;
                //Frm_ShapeMatchTool.Instance.button5.Visible = false;
                //Frm_ShapeMatchTool.Instance.button9.Visible = false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
            finally
            {
                Frm_ShapeMatchTool.IsResettingUI = false;
            }
        }
        /// <summary>
        /// 清空上次运行的所有输入
        /// </summary>
        internal void ClearLastInput()
        {
            try
            {
                toolPar.InputPar.图像 = null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal void Test()
        {
            if (modelID == -1)
                return;
            HOperatorSet.GetShapeModelContours(out contour, modelID, new HTuple(1));
            HTuple area, row, col;
            HOperatorSet.AreaCenter(totalRegion, out area, out row, out col);
            HTuple homMat2D;
            HOperatorSet.HomMat2dIdentity(out homMat2D);
            HOperatorSet.HomMat2dTranslate(homMat2D, row, col, out homMat2D);
            HOperatorSet.AffineTransContourXld(contour, out contour, homMat2D);
            HOperatorSet.SetLineWidth(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 2);
            Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(contour, "green");
            //Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(final_region, "blue");
        }

        public HObject contour;
        /// <summary>
        /// 显示模板
        /// </summary>
        internal void ShowTemplate()
        {
            try
            {
                if (modelID == -1)
                {
                    Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未创建模板";
                    return;
                }
                if (GetTemplateDisplayImage() == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未指定输入图像";
                    return;
                }
                if (toolPar.InputPar.图像 == null || totalRegion == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未创建模板";
                    return;
                }
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(GetTemplateDisplayImage());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                if (matchMode == MatchMode.BasedShape)
                {
                    //Frm_ShapeMatchTool.Instance.hWindow_Final1.DispImageFit();
                    HOperatorSet.GetShapeModelContours(out contour, modelID, new HTuple(1));
                    HTuple area, row, col;
                    HOperatorSet.AreaCenter(totalRegion, out area, out row, out col);
                    HTuple homMat2D;
                    HOperatorSet.HomMat2dIdentity(out homMat2D);
                    HOperatorSet.HomMat2dTranslate(homMat2D, row, col, out homMat2D);
                    HOperatorSet.AffineTransContourXld(contour, out contour, homMat2D);
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(contour, "orange");
                }

                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                //////HOperatorSet.DispObj(outBoundary, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                //////HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple(4));
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(totalRegion, "green");
                ShowTemplatePreview();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal void ShowTemplatePreview()
        {
            try
            {
                HObject displayImage = GetTemplateDisplayImage();
                if (displayImage == null || totalRegion == null)
                {
                    HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                    return;
                }

                HTuple area, row, col;
                HOperatorSet.AreaCenter(totalRegion, out area, out row, out col);
                if (area <= 0)
                {
                    HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                    return;
                }

                HTuple row1, col1, row2, col2;
                HOperatorSet.SmallestRectangle1(totalRegion, out row1, out col1, out row2, out col2);
                HObject previewRegion;
                HOperatorSet.GenRectangle1(out previewRegion, row1 - 20, col1 - 20, row2 + 20, col2 + 20);
                HObject previewImage;
                HOperatorSet.ReduceDomain(displayImage, previewRegion, out previewImage);

                HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, row1 - 20, col1 - 20, row2 + 20, col2 + 20);
                HOperatorSet.DispObj(previewImage, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);

                if (matchMode == MatchMode.BasedShape && modelID != -1)
                {
                    HObject previewContour;
                    HOperatorSet.GetShapeModelContours(out previewContour, modelID, new HTuple(1));
                    HTuple homMat2D;
                    HOperatorSet.HomMat2dIdentity(out homMat2D);
                    HOperatorSet.HomMat2dTranslate(homMat2D, row, col, out homMat2D);
                    HOperatorSet.AffineTransContourXld(previewContour, out previewContour, homMat2D);
                    HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("orange"));
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple());
                    HOperatorSet.DispObj(previewContour, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 创建并显示模板
        /// </summary>
        internal void CreateAndShowTemplate()
        {
            try
            {
                if (toolPar.InputPar.图像 == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未指定输入图像";
                    return;
                }
                SyncTemplateRegionFromEditableRois();
                if (modelID != -1)
                {
                    HOperatorSet.ClearShapeModel(modelID);
                    modelID = -1;
                }
                if (CreateTemplate() == 0)
                {
                    if (matchMode == MatchMode.BasedShape)
                    {
                        HOperatorSet.GetShapeModelContours(out contour, modelID, (HTuple)1);


                        HTuple area, row, col;
                        HOperatorSet.AreaCenter(totalRegion, out area, out row, out col);
                        HTuple homMat2D;
                        HOperatorSet.HomMat2dIdentity(out homMat2D);
                        HOperatorSet.HomMat2dTranslate(homMat2D, row, col, out homMat2D);
                        HOperatorSet.AffineTransContourXld(contour, out contour, homMat2D);
                    }
                    //////GetImageWindowControl().hwc_imageWindow.DispObj(contour, "orange");
                    //ContrastChanged();

                    ShowTemplatePreview();
                    //standardImage = toolPar.InputPar.输入图像;
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绘制矩形模板
        /// </summary>
        internal void DrawTemplateRectangle1()
        {
            try
            {

                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = false;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = false;

                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle1.BackColor = Color.DarkGray;

                HObject displayImage = GetTemplateDisplayImage();
                // 进入绘制模式前先清空窗口（变黑），避免旧图像/残留影响绘制
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ClearWindow();
                if (displayImage == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未指定输入图像";
                    FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle1);
                    return;
                }
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(displayImage);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                RebuildTemplateRoisInWindow();
                Frm_ShapeMatchTool.Instance.hWindow_Final1.Focus();
                HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "margin");

                HTuple row111, col111, row1111, col1111;
                HOperatorSet.GetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row1111, out col1111);
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "请在图像窗口中绘制模板区域，右击绘制结束", 13, row111 + (row1111 - row111) / 30, col111 + (col1111 - col111) / 30, "blue", "false");

                HTuple row, col, row1, col1;
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "green");
                if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                }
                else
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 4);
                }
                //////GetImageWindowControl().SetDrawMode(true);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = null;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                HOperatorSet.DrawRectangle1(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row, out col, out row1, out col1);
                //////GetImageWindowControl().SetDrawMode(false);
                HObject rectangle1;
                HOperatorSet.GenRectangle1(out rectangle1, row, col, row1, col1);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genRect1(row.D, col.D, row1.D, col1.D, ref templateROIs);
                SetTemplateRoiOperator(templateROIs[templateROIs.Count - 1]);
                SyncTemplateRegionFromEditableRois();

                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                // disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 50, 100, "blue", "false");

                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle1.BackColor = Color.FromArgb(46, 141, 230);


                //disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 50, 100, "blue", "false");
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 13, row111 + (row1111 - row111) / 30, col111 + (col1111 - col111) / 30, "blue", "false");


                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;
            }
            catch (Exception ex)
            {
                FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle1);
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绘制仿射矩形模板
        /// </summary>
        internal void DrawTemplateRectangle2()
        {
            try
            {
                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = false;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = false;

                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle2.BackColor = Color.DarkGray;

                HObject displayImage = GetTemplateDisplayImage();
                // 进入绘制模式前先清空窗口（变黑），避免旧图像/残留影响绘制
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ClearWindow();
                if (displayImage == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未指定输入图像";
                    FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle2);
                    return;
                }
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(displayImage);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                RebuildTemplateRoisInWindow();
                Frm_ShapeMatchTool.Instance.hWindow_Final1.Focus();
                HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));
                set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 13, "sans", "true", "false");

                HTuple row111, col111, row1, col1;
                HOperatorSet.GetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row1, out col1);
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "请在图像窗口中绘制模板区域，右击绘制结束", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");

                HTuple row, col, angle, lenght1, length2;
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "green");
                if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                }
                else
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 4);
                }
                //////GetImageWindowControl().SetDrawMode(true);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = null;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                HOperatorSet.DrawRectangle2(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row, out col, out angle, out lenght1, out length2);
                //////GetImageWindowControl().SetDrawMode(false);
                HObject rectangle2;
                HOperatorSet.GenRectangle2(out rectangle2, row, col, angle, lenght1, length2);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genRect2(row.D, col.D, angle.D, lenght1.D, length2.D, ref templateROIs);
                SetTemplateRoiOperator(templateROIs[templateROIs.Count - 1]);
                SyncTemplateRegionFromEditableRois();

                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");


                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle2.BackColor = Color.FromArgb(46, 141, 230);
                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;
            }
            catch (Exception ex)
            {
                FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle2);
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绘制圆形模板
        /// </summary>
        internal void DrawTemplateCircle()
        {
            try
            {
                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = false;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = false;

                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionCircle.BackColor = Color.DarkGray;

                HObject displayImage = GetTemplateDisplayImage();
                // 进入绘制模式前先清空窗口（变黑），避免旧图像/残留影响绘制
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ClearWindow();
                if (displayImage == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未指定输入图像";
                    FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionCircle);
                    return;
                }
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(displayImage);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                RebuildTemplateRoisInWindow();
                Frm_ShapeMatchTool.Instance.hWindow_Final1.Focus();
                HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));

                HTuple row111, col111, row1, col1;
                HOperatorSet.GetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row1, out col1);
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "请在图像窗口中绘制模板区域，右击绘制结束", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");

                HTuple row, col, radius;
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "green");
                if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                }
                else
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 4);
                }
                //////GetImageWindowControl().SetDrawMode(true);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = null;

                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                HOperatorSet.DrawCircle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row, out col, out radius);
                //////GetImageWindowControl().SetDrawMode(false);
                HObject circle;
                HOperatorSet.GenCircle(out circle, row, col, radius);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genCircle(row.D, col.D, radius.D, ref templateROIs);
                SetTemplateRoiOperator(templateROIs[templateROIs.Count - 1]);
                SyncTemplateRegionFromEditableRois();

                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");
                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionCircle.BackColor = Color.FromArgb(46, 141, 230);
                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;

            }
            catch (Exception ex)
            {
                FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionCircle);
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绘制椭圆形模板
        /// </summary>
        internal void DrawTemplateEllipse()
        {
            try
            {
                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = false;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = false;

                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionEllipse.BackColor = Color.DarkGray;

                HObject displayImage = GetTemplateDisplayImage();
                // 进入绘制模式前先清空窗口（变黑），避免旧图像/残留影响绘制
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ClearWindow();
                if (displayImage == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未指定输入图像";
                    FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionEllipse);
                    return;
                }
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(displayImage);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                RebuildTemplateRoisInWindow();
                Frm_ShapeMatchTool.Instance.hWindow_Final1.Focus();
                HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));

                HTuple row111, col111, row1, col1;
                HOperatorSet.GetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row1, out col1);
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "请在图像窗口中绘制模板区域，右击绘制结束", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");
                HTuple row, col, angle, length1, length2;
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "green");
                if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                }
                else
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 4);
                }
                //////GetImageWindowControl().SetDrawMode(true);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = null;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                HOperatorSet.DrawEllipse(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row, out col, out angle, out length1, out length2);
                //////GetImageWindowControl().SetDrawMode(false);
                HObject ellipseContour;
                HTuple rows, cols;
                HOperatorSet.GenEllipseContourXld(out ellipseContour, row, col, angle, length1, length2, 0, ((HTuple)360).TupleRad(), "positive", 1.0);
                HOperatorSet.GetContourXld(ellipseContour, out rows, out cols);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genNurbs(rows, cols, ref templateROIs);
                SetTemplateRoiOperator(templateROIs[templateROIs.Count - 1]);
                SyncTemplateRegionFromEditableRois();

                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");
                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionEllipse.BackColor = Color.FromArgb(46, 141, 230);

                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;
            }
            catch (Exception ex)
            {
                FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionEllipse);
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绘制任意形状模板
        /// </summary>
        internal void DrawTemplateAny()
        {
            try
            {
                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = false;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = false;

                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionAny.BackColor = Color.DarkGray;

                HObject displayImage = GetTemplateDisplayImage();
                // 进入绘制模式前先清空窗口（变黑），避免旧图像/残留影响绘制
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ClearWindow();
                if (displayImage == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未指定输入图像";
                    FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionAny);
                    return;
                }
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(displayImage);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                RebuildTemplateRoisInWindow();
                Frm_ShapeMatchTool.Instance.hWindow_Final1.Focus();
                HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));

                HTuple row111, col111, row1, col1;
                HOperatorSet.GetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row1, out col1);
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "请在图像窗口中绘制模板区域，右击绘制结束", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");

                HObject region;
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "green");
                if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                }
                else
                {
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 4);
                }
                //////GetImageWindowControl().SetDrawMode(true);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = null;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;
                HTuple rows, cols, weights;
                HOperatorSet.DrawNurbs(out region, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "true", "true", "true", "true", 3, out rows, out cols, out weights);
                //////GetImageWindowControl().SetDrawMode(false);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genNurbs(rows, cols, ref templateROIs);
                SetTemplateRoiOperator(templateROIs[templateROIs.Count - 1]);
                SyncTemplateRegionFromEditableRois();

                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");
                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionAny.BackColor = Color.FromArgb(46, 141, 230);

                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;
            }
            catch (Exception ex)
            {
                FinishTemplateRegionDraw(Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionAny);
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绘制搜索区域
        /// </summary>
        internal void DrawSearchRegion()
        {
            try
            {
                if (Job.loadForm)
                    return;
                if (Frm_ShapeMatchTool.Instance.cbx_searchRegionType.TextStr == string.Empty)
                    return;
                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.Focus();
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("blue"));
                HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "margin");
                HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = true;

                switch (Frm_ShapeMatchTool.Instance.cbx_searchRegionType.TextStr)
                {
                    case "矩形":
                    case "Rectangle1":
                        if (searchRegionType == RegionType.Rectangle1)
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                        else
                        {
                            this.L_regions.Clear();
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genRect1(200.0, 200.0, 600.0, 800.0, ref this.L_regions);
                        }
                        searchRegionType = RegionType.Rectangle1;
                        break;
                    case "仿射矩形":
                    case "Rectangle2":
                        if (searchRegionType == RegionType.Rectangle2)
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                        else
                        {
                            this.L_regions.Clear();
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genRect2(400.0, 500.0, 0, 300.0, 200.0, ref this.L_regions);
                        }
                        searchRegionType = RegionType.Rectangle2;
                        break;
                    case "圆":
                    case "Circle":
                        if (searchRegionType == RegionType.Circle)
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                        else
                        {
                            this.L_regions.Clear();
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genCircle(400.0, 500.0, 200.0, ref this.L_regions);
                        }
                        searchRegionType = RegionType.Circle;
                        break;

                    case "多点":
                    case "Circxxle":
                        HTuple row, col, row1, col1;
                        HOperatorSet.GetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row, out col, out row1, out col1);
                        DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "提示：请在图像窗口中左击绘制多点区域，右击绘制结束", 12, row + (row1 - row) / 30, col + (col1 - col) / 30, "blue", "false");
                        if (searchRegionType == RegionType.MultPoint)
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                        else
                        {
                            this.L_regions.Clear();
                            HObject ho_ContOut1;
                            HTuple rows, cols, weights;
                            HOperatorSet.DrawNurbs(out ho_ContOut1, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "true", "true", "true", "true", 3, out rows, out cols, out weights);
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.genNurbs(rows, cols, ref this.L_regions);
                        }
                        searchRegionType = RegionType.MultPoint;
                        break;
                    //////case "椭圆":
                    //////case "Ellipse":
                    //////    HTuple row4, column4, angle4, length4, length5;
                    //////    if (searchRegionType == RegionType.Ellipse)
                    //////        HOperatorSet.DrawEllipseMod(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, searchRegionPointData[0], searchRegionPointData[1], searchRegionPointData[2], searchRegionPointData[3], searchRegionPointData[4], out row4, out column4, out angle4, out length4, out length5);
                    //////    else
                    //////        HOperatorSet.DrawEllipse(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, out row4, out column4, out angle4, out length4, out length5);
                    //////    searchRegionPointData.Clear();
                    //////    searchRegionPointData.Add((double)row4);
                    //////    searchRegionPointData.Add((double)column4);
                    //////    searchRegionPointData.Add((double)angle4);
                    //////    searchRegionPointData.Add((double)length4);
                    //////    searchRegionPointData.Add((double)length5);

                    //////    HObject ellipse;
                    //////    HOperatorSet.GenEllipse(out ellipse, row4, column4, angle4, length4, length5);
                    //////    HOperatorSet.DispObj(ellipse, GetImageWindowControl().hwc_imageWindow.HWindowHalconID);
                    //////    if (SearchRegion != null)
                    //////        HOperatorSet.Union2(SearchRegion, ellipse, out   _searchRegion);
                    //////    else
                    //////        SearchRegion = ellipse;
                    //////    searchRegionType = RegionType.Ellipse;
                    //////    break;
                    //////case "任意":
                    //////case "Any":
                    //////    HObject polygon;
                    //////    HOperatorSet.DrawRegion(out polygon, GetImageWindowControl().hwc_imageWindow.HWindowHalconID);
                    //////    HOperatorSet.DispObj(polygon, GetImageWindowControl().hwc_imageWindow.HWindowHalconID);
                    //////    if (SearchRegion == null)
                    //////    {
                    //////        SearchRegion = polygon;
                    //////    }
                    //////    else
                    //////    {
                    //////        HOperatorSet.Union2(SearchRegion, polygon, out   _searchRegion);
                    //////    }
                    //////    break;
                    default:
                        UseAllImageSearchRegion(false);
                        break;
                }
                Frm_ShapeMatchTool.Instance.regions = this.L_regions;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 改变对比度
        /// </summary>
        internal void ContrastChanged()
        {
            try
            {
                if (!Frm_ShapeMatchTool.Instance.ckb_autoContrast.Checked)
                    Frm_ShapeMatchTool.Instance.lbl_contastValue.Text = Frm_ShapeMatchTool.Instance.tkb_contrast.Value.ToString();
                contrast = Convert.ToInt16(Frm_ShapeMatchTool.Instance.tkb_contrast.Value);
                //standardImage = toolPar.InputPar.输入图像;
                if (CreateTemplate() != 0)
                    return;
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                HObject countour;
                HOperatorSet.GetShapeModelContours(out countour, modelID, (HTuple)1);
                HTuple area, row, column;
                HOperatorSet.AreaCenter(templateRegion, out area, out row, out column);
                HTuple homMat2D;
                HOperatorSet.HomMat2dIdentity(out homMat2D);
                HOperatorSet.HomMat2dTranslate(homMat2D, row, column, out homMat2D);
                HOperatorSet.AffineTransContourXld(countour, out countour, homMat2D);
                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("orange"));
                HOperatorSet.DispObj(countour, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                HOperatorSet.DispObj(templateRegion, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("blue"));
                if (SearchRegion != null)
                {
                    //////ShowObj(jobName, SearchRegion);
                }
                ////// CreateAndShowTemplate();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 单击结果dgv控件，查看每一个匹配结果
        /// </summary>
        internal void ClickResultDgv(DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1)
                {
                    return;
                }
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                int rowIndex = e.RowIndex;
                double row = Convert.ToDouble(Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[rowIndex].Cells[2].Value);
                double col = Convert.ToDouble(Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[rowIndex].Cells[3].Value);
                // 结果表格的标题明确为"角度(°)"，而 HALCON 的几何算子统一接收弧度。
                // 表格只负责以角度显示；点击结果重新绘制时必须转回弧度。
                double angleDegree = Convert.ToDouble(Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[rowIndex].Cells[4].Value);
                double angle = angleDegree * Math.PI / 180.0;

                MatchResult selectedResult = new MatchResult();
                selectedResult.Row = row;
                selectedResult.Col = col;
                selectedResult.Angle = angle;
                selectedResult.Socre = Convert.ToDouble(Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[rowIndex].Cells[1].Value);

                DisplaySearchRegion(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                DisplayMatchResult(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, selectedResult, rowIndex);
                ShowMatchedImagePreview(selectedResult);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 创建模板
        /// </summary>
        /// <returns>结果状态返回值：0表示成功 -1表示未知异常 1表示特征过少</returns>
        internal int CreateTemplate()
        {
            try
            {
                SyncTemplateRegionFromEditableRois();
                if (toolPar.InputPar.图像 == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未指定输入图像";
                    return -1;
                }
                HObject template;
                totalRegion = templateRegion;
                if (templateRegion == null)
                {
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未创建模板区域";
                    return -1;
                }

                HTuple templateArea, templateRow, templateColumn;
                HOperatorSet.AreaCenter(templateRegion, out templateArea, out templateRow, out templateColumn);
                if (templateArea <= 0)
                {
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：未创建模板区域";
                    return -1;
                }
                try
                {
                    if (final_region != null)
                        HOperatorSet.Difference(templateRegion, final_region, out totalRegion);
                }
                catch { }
                try
                {
                    if (final_region111 != null)
                        HOperatorSet.Union2(totalRegion, final_region111, out totalRegion);
                }
                catch { }

                HTuple totalAreaAfterEdit, totalRowAfterEdit, totalColumnAfterEdit;
                HOperatorSet.AreaCenter(totalRegion, out totalAreaAfterEdit, out totalRowAfterEdit, out totalColumnAfterEdit);
                if (totalAreaAfterEdit <= 0)
                {
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：模板区域无效，请重新绘制";
                    return -1;
                }

                HOperatorSet.ReduceDomain(toolPar.InputPar.图像, totalRegion, out template);
                try
                {
                    if (matchMode == MatchMode.BasedShape)
                        HOperatorSet.CreateScaledShapeModel(template,
                                                     (HTuple)"auto",
                                                     GetModelAngleStartRad(),
                                                     GetModelAngleExtentRad(),
                                                     (HTuple)("auto"),
                                                     minScale,
                                                     maxScale,
                                                     "auto",
                                                     (HTuple)"auto",
                                                     (HTuple)polarity,
                                                      Frm_ShapeMatchTool.Instance.ckb_autoContrast.Checked ? (HTuple)"auto" : (HTuple)contrast,
                                                     (HTuple)"auto",
                                                      out modelID);
                    else
                        HOperatorSet.CreateNccModel(template,
                                                     (HTuple)"auto",
                                                     GetModelAngleStartRad(),
                                                     GetModelAngleExtentRad(),
                                                     (HTuple)("auto"),
                                                     "use_polarity",
                                                      out modelID);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("#8510:"))      //特征过少，Halcon报错编号8510
                    {
                        Frm_ShapeMatchTool.Instance.label4.Text = "状态：" + "特征过少，无法完成训练（错误代码：0201）";
                        return 1;
                    }

                    Log.SaveError(ex);
                    Frm_ShapeMatchTool.Instance.label4.Text = "状态：学习失败，请检查模板区域或参数";
                    return -1;
                }

                // 学习成功后更新模板显示图，供窗口显示及跨会话保存
                try
                {
                    if (toolPar != null && toolPar.InputPar != null && toolPar.InputPar.图像 != null)
                    {
                        HObject dispImage;
                        HOperatorSet.ReduceDomain(toolPar.InputPar.图像, totalRegion, out dispImage);
                        templateDisplayImage = dispImage;
                    }
                }
                catch
                {
                    templateDisplayImage = null;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return -1;
            }
        }
        /// <summary>
        /// 清除搜索区域
        /// </summary>
        internal void ClearSearchRegion()
        {
            try
            {
                UseAllImageSearchRegion(false);
                Frm_ShapeMatchTool.Instance.cbx_searchRegionType.SelectedIndex = 0;
                Frm_ShapeMatchTool.Instance.cbx_searchRegionType.TextStr = Project.Instance.configuration.language == Language.English ? "AllImage" : "整幅图像";
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                if (templateRegion != null)
                {
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否刷新图像</param>
        private HTuple CreateMatchTransform(MatchResult result)
        {
            HTuple area, templateRow, templateCol;
            HOperatorSet.AreaCenter(totalRegion, out area, out templateRow, out templateCol);

            HTuple homMat2D;
            HOperatorSet.HomMat2dIdentity(out homMat2D);
            HOperatorSet.HomMat2dTranslate(homMat2D, -templateRow, -templateCol, out homMat2D);
            HOperatorSet.HomMat2dRotate(homMat2D, result.Angle, 0, 0, out homMat2D);
            HOperatorSet.HomMat2dTranslate(homMat2D, result.Row, result.Col, out homMat2D);
            return homMat2D;
        }

        private HObject CreateMatchedTemplateRegion(MatchResult result)
        {
            HObject matchedRegion;
            HOperatorSet.AffineTransRegion(totalRegion, out matchedRegion, CreateMatchTransform(result), "nearest_neighbor");
            return matchedRegion;
        }

        private void DisplaySearchRegion(HTuple windowHandle)
        {
            if (!showSearchRegion || toolPar.InputPar.图像 == null)
                return;

            HObject displayRegion = null;
            if (searchRegionType == RegionType.AllImage)
            {
                HTuple width, height;
                HOperatorSet.GetImageSize(toolPar.InputPar.图像, out width, out height);
                HOperatorSet.GenRectangle1(out displayRegion, 0, 0, height - 1, width - 1);
            }
            else if (SearchRegion != null)
            {
                displayRegion = SearchRegion;
            }

            if (displayRegion == null)
                return;

            HOperatorSet.SetDraw(windowHandle, "margin");
            HOperatorSet.SetLineWidth(windowHandle, 2);
            HOperatorSet.SetColor(windowHandle, "blue");
            HOperatorSet.DispObj(displayRegion, windowHandle);
        }

        private void DisplayMatchResult(HTuple windowHandle, MatchResult result, int resultIndex)
        {
            HObject matchedRegion = CreateMatchedTemplateRegion(result);

            HOperatorSet.SetDraw(windowHandle, "margin");
            HOperatorSet.SetLineStyle(windowHandle, new HTuple());

            // 绿色：匹配到的模板区域。
            if (showTemplate)
            {
                HOperatorSet.SetColor(windowHandle, "green");
                HOperatorSet.SetLineWidth(windowHandle, 2);
                HOperatorSet.DispObj(matchedRegion, windowHandle);
            }

            // 橙色：形状模型实际参与匹配的特征轮廓。
            if (showFeature && matchMode == MatchMode.BasedShape)
            {
                HObject modelContour, matchedContour;
                HTuple contourTransform;
                HOperatorSet.HomMat2dIdentity(out contourTransform);
                HOperatorSet.HomMat2dRotate(contourTransform, result.Angle, 0, 0, out contourTransform);
                HOperatorSet.HomMat2dTranslate(contourTransform, result.Row, result.Col, out contourTransform);
                HOperatorSet.GetShapeModelContours(out modelContour, modelID, new HTuple(1));
                HOperatorSet.AffineTransContourXld(modelContour, out matchedContour, contourTransform);
                HOperatorSet.SetColor(windowHandle, "orange");
                HOperatorSet.SetLineWidth(windowHandle, 1);
                HOperatorSet.DispObj(matchedContour, windowHandle);
            }

            // 黄色：匹配模板的最小外接框，不再用模板区域冒充"匹配框"。
            if (showMatchBox)
            {
                HTuple boxRow, boxCol, boxPhi, boxLength1, boxLength2;
                HObject matchBox;
                HOperatorSet.SmallestRectangle2(matchedRegion, out boxRow, out boxCol, out boxPhi, out boxLength1, out boxLength2);
                HOperatorSet.GenRectangle2(out matchBox, boxRow, boxCol, boxPhi, boxLength1, boxLength2);
                HOperatorSet.SetColor(windowHandle, "yellow");
                HOperatorSet.SetLineWidth(windowHandle, 2);
                HOperatorSet.DispObj(matchBox, windowHandle);
            }

            if (showCross || showIndex)
            {
                HTuple partRow1, partCol1, partRow2, partCol2;
                HOperatorSet.GetPart(windowHandle, out partRow1, out partCol1, out partRow2, out partCol2);
                double crossSize = Math.Max(6.0, ((double)(partRow2 - partRow1)) / 90.0 + 1.0);

                if (showCross)
                {
                    HObject cross;
                    HOperatorSet.GenCrossContourXld(out cross, result.Row, result.Col, crossSize, result.Angle);
                    HOperatorSet.SetColor(windowHandle, "cyan");
                    HOperatorSet.SetLineWidth(windowHandle, 2);
                    HOperatorSet.DispObj(cross, windowHandle);
                }

                if (showIndex)
                {
                    set_display_font(windowHandle, 10, "sans", "true", "false");
                    DispMessage(windowHandle, resultIndex + 1, 12, result.Row + crossSize, result.Col + crossSize, "cyan", "true");
                }
            }
        }

        /// <summary>
        /// 右上预览显示本次输入图中实际匹配到的目标，而不是训练时的旧模板。
        /// </summary>
        private void ShowMatchedImagePreview(MatchResult result)
        {
            if (!Frm_ShapeMatchTool.Instance.Visible || toolPar.InputPar.图像 == null)
                return;

            HObject matchedRegion = CreateMatchedTemplateRegion(result);
            HTuple row1, col1, row2, col2, width, height;
            HOperatorSet.SmallestRectangle1(matchedRegion, out row1, out col1, out row2, out col2);
            HOperatorSet.GetImageSize(toolPar.InputPar.图像, out width, out height);

            double boxHeight = Math.Max(1.0, (double)(row2 - row1));
            double boxWidth = Math.Max(1.0, (double)(col2 - col1));
            double padding = Math.Max(12.0, Math.Max(boxHeight, boxWidth) * 0.15);
            double previewRow1 = Math.Max(0.0, (double)row1 - padding);
            double previewCol1 = Math.Max(0.0, (double)col1 - padding);
            double previewRow2 = Math.Min((double)height - 1.0, (double)row2 + padding);
            double previewCol2 = Math.Min((double)width - 1.0, (double)col2 + padding);

            HTuple previewWindow = Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow;
            HOperatorSet.ClearWindow(previewWindow);
            HOperatorSet.SetPart(previewWindow, previewRow1, previewCol1, previewRow2, previewCol2);
            HOperatorSet.DispObj(toolPar.InputPar.图像, previewWindow);
            DisplayMatchResult(previewWindow, result, 0);
        }

        /// <summary>
        /// 运行时自动学习模板：当模型数据未加载（工程未保存模板或反序列化失败）时，
        /// 用已保存的模板区域 + 当前输入图创建模型，避免流程因未创建模板直接失败。
        /// 只使用字段数据与 HALCON 算子，不访问窗口控件，可在后台线程安全调用。
        /// </summary>
        private bool TryAutoCreateModel()
        {
            try
            {
                SyncTemplateRegionFromEditableRois();
                if (templateRegion == null || !templateRegion.IsInitialized())
                    return false;
                if (toolPar.InputPar.图像 == null)
                    return false;

                HTuple learnArea, learnRow, learnCol;
                HOperatorSet.AreaCenter(templateRegion, out learnArea, out learnRow, out learnCol);
                if (learnArea.I <= 0)
                    return false;

                HObject template;
                HOperatorSet.ReduceDomain(toolPar.InputPar.图像, templateRegion, out template);
                try
                {
                    if (matchMode == MatchMode.BasedShape)
                        HOperatorSet.CreateScaledShapeModel(template,
                                                         (HTuple)"auto",
                                                         GetModelAngleStartRad(),
                                                         GetModelAngleExtentRad(),
                                                         (HTuple)"auto",
                                                         minScale,
                                                         maxScale,
                                                         (HTuple)"auto",
                                                         (HTuple)"auto",
                                                         polarity,
                                                         (HTuple)contrast,
                                                         (HTuple)"auto",
                                                         out modelID);
                    else
                        HOperatorSet.CreateNccModel(template,
                                                     (HTuple)"auto",
                                                     GetModelAngleStartRad(),
                                                     GetModelAngleExtentRad(),
                                                     (HTuple)"auto",
                                                     "use_polarity",
                                                      out modelID);
                    totalRegion = templateRegion;
                }
                finally
                {
                    template.Dispose();
                }

                // 学习成功后同步更新模板显示图，供窗口显示及后续保存工程
                try
                {
                    if (toolPar != null && toolPar.InputPar != null && toolPar.InputPar.图像 != null)
                    {
                        HObject dispImage;
                        HOperatorSet.ReduceDomain(toolPar.InputPar.图像, totalRegion, out dispImage);
                        templateDisplayImage = dispImage;
                    }
                }
                catch
                {
                    templateDisplayImage = null;
                }

                return modelID != null && modelID.Length > 0 && modelID.I != -1;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                modelID = -1;
                return false;
            }
        }

        public override void Run(bool updateImage, bool runTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;
                    if (toolPar.InputPar.图像 == null)
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Create_Template : ToolRunStatu.未指定输入图像);
                        return;
                    }
                    if (modelID == -1)
                    {
                        // 模型数据未加载时自动学习，避免流程因未创建模板直接失败
                        if (!TryAutoCreateModel())
                        {
                            toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Create_Template : ToolRunStatu.未创建模板);
                            return;
                        }
                    }


                    HObject image;
                    if (searchRegionType == RegionType.AllImage)
                    {
                        image = toolPar.InputPar.图像;
                    }
                    else
                    {
                        HOperatorSet.ReduceDomain(toolPar.InputPar.图像, SearchRegion, out   reducedImage);
                        image = reducedImage;
                    }

                    List<MatchResult> L_resultList = new List<MatchResult>();
                    HTuple rows, cols, angles, scores;
                    //此处Catch中重新查找，是因为开启程序第一次查找时会报错，这时需要重新创建模板，然后重新查找即可
                    try
                    {
                        if (matchMode == MatchMode.BasedShape)
                        {
                            HTuple temp;

                            HOperatorSet.FindScaledShapeModel(image,
                                                       (HTuple)modelID,
                                                       ((HTuple)startAngle).TupleRad(),
                                                       GetMatchAngleExtentRad(),
                                                       minScale,
                                                       maxScale,
                                                       (HTuple)minScore,
                                                       (HTuple)matchNum,
                                                       (HTuple)0.5,
                                                       (HTuple)"least_squares",
                                                       (HTuple)0,
                                                       (HTuple)0.9,
                                                        out rows,
                                                        out cols,
                                                        out angles,
                                                        out temp,
                                                        out scores);
                        }
                        else
                        {
                            HOperatorSet.FindNccModel(image,
                                                        (HTuple)modelID,
                                                        ((HTuple)startAngle).TupleRad(),
                                                        GetMatchAngleExtentRad(),
                                                        (HTuple)minScore,
                                                        (HTuple)matchNum,
                                                        (HTuple)0.5,
                                                        (HTuple)"true",
                                                        (HTuple)0,
                                                         out rows,
                                                         out cols,
                                                         out angles,
                                                         out scores);
                        }
                    }
                    catch
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Create_Template : ToolRunStatu.未创建模板);
                        return;
                    }

                    //记录用户当前缩放视图，运行完成后恢复，避免每次运行图片都跳回自适应窗口
                    int savedRow1 = 0, savedCol1 = 0, savedRow2 = 0, savedCol2 = 0;
                    bool restoreView = false;
                    if (runTool && Frm_ShapeMatchTool.Instance != null && Frm_ShapeMatchTool.Instance.Visible)
                    {
                        Frm_ShapeMatchTool.Instance.hWindow_Final1.GetViewPart(out savedRow1, out savedCol1, out savedRow2, out savedCol2);
                        restoreView = savedRow2 > savedRow1 && savedCol2 > savedCol1;
                    }

                    //重新显示图像
                    if (updateImage)
                    {
                        if (runTool)
                        {
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                        }
                        else
                        {
                            ShowImage(toolPar.InputPar.图像);
                            //流程运行(后台线程)时调度到UI线程刷新窗口，避免直接操作HALCON窗口导致#5100等错误
                            SafeInvokeShapeMatchWindow(() =>
                            {
                                if (Frm_ShapeMatchTool.Instance.Visible)
                                {
                                    Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                                    Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                                }
                            });
                        }
                    }
                    SafeInvokeShapeMatchWindow(() =>
                    {
                        if (Frm_ShapeMatchTool.Instance.Visible)
                        {
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.EnableImagePan = false;
                        }
                    });

                    //恢复用户之前缩放视图（保留放大后的效果，方便查看匹配结果）
                    //若新图像尺寸比原视图小（视图越界），则不恢复，保持自适应显示
                    if (restoreView)
                    {
                        HTuple imgW, imgH;
                        HOperatorSet.GetImageSize(toolPar.InputPar.图像, out imgW, out imgH);
                        if (savedRow2 <= (int)imgH.I && savedCol2 <= (int)imgW.I)
                        {
                            SafeInvokeShapeMatchWindow(() =>
                                Frm_ShapeMatchTool.Instance.hWindow_Final1.SetViewPart(toolPar.InputPar.图像, savedRow1, savedCol1, savedRow2, savedCol2));
                        }
                    }

                    SafeInvokeShapeMatchWindow(() =>
                    {
                        if (Frm_ShapeMatchTool.Instance.Visible)
                            Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows.Clear();
                    });

                    if (rows.TupleLength() > 0)
                    {
                        for (int i = 0; i < rows.TupleLength(); i++)
                        {
                            MatchResult matchResult = new MatchResult();
                            matchResult.Row = Math.Round((double)rows[i], 3);
                            matchResult.Col = Math.Round((double)cols[i], 3);
                            // HALCON 返回弧度。内部结果和下游位姿跟随继续保留弧度，
                            // 避免破坏 VectorAngleToRigid 等几何计算；仅界面显示转换为度。
                            matchResult.Angle = (double)angles[i];
                            matchResult.Socre = Math.Round((double)scores[i], 3);
                            L_resultList.Add(matchResult);




                        }

                        //以下代码对结果依据分数进行排序
                        MatchResult temp;
                        if (sortMode == SortMode.从上至下且从左至右)
                        {
                            for (int i = 0; i < L_resultList.Count; i++)
                            {
                                for (int j = i + 1; j < L_resultList.Count; j++)
                                {
                                    if (L_resultList[i].Row - L_resultList[j].Row > spanPixelNum / 2)
                                    {
                                        temp = L_resultList[i];
                                        L_resultList[i] = L_resultList[j];
                                        L_resultList[j] = temp;
                                    }
                                }
                            }

                            for (int i = 0; i < L_resultList.Count; i++)
                            {
                                for (int j = i + 1; j < L_resultList.Count; j++)
                                {
                                    if ((L_resultList[i].Col - L_resultList[j].Col > spanPixelNum / 2) && (Math.Abs(L_resultList[i].Row - L_resultList[j].Row) < spanPixelNum / 2))
                                    {
                                        temp = L_resultList[i];
                                        L_resultList[i] = L_resultList[j];
                                        L_resultList[j] = temp;
                                    }
                                }
                            }


                        }
                        else
                        {
                            for (int i = 0; i < L_resultList.Count - 1; i++)
                            {
                                for (int j = i + 1; j < L_resultList.Count; j++)
                                {
                                    if (L_resultList[i].Socre < L_resultList[j].Socre)
                                    {
                                        temp = L_resultList[i];
                                        L_resultList[i] = L_resultList[j];
                                        L_resultList[j] = temp;
                                    }
                                }
                            }
                        }
                    }

                    L_result.Clear();
                    for (int i = 0; i < L_resultList.Count; i++)
                    {
                        XYU xyu = new XYU();
                        xyu.Point.X = Math.Round(L_resultList[i].Row, 3);
                        xyu.Point.Y = Math.Round(L_resultList[i].Col, 3);
                        xyu.U = Math.Round(L_resultList[i].Angle, 3);
                        L_result.Add(xyu);
                    }

                    if (runTool)
                        DisplaySearchRegion(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                    else
                    {
                        //调试窗口图像已由 ShowImage 显示，不再直接操作其HALCON窗口；
                        //模板匹配窗口的搜索区域绘制调度到UI线程执行
                        SafeInvokeShapeMatchWindow(() =>
                        {
                            if (Frm_ShapeMatchTool.Instance.Visible)
                                DisplaySearchRegion(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                        });
                    }

                    for (int i = 0; i < L_resultList.Count; i++)
                    {
                        if (runTool)
                            DisplayMatchResult(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, L_resultList[i], i);
                        else
                        {
                            int resultIndex = i;
                            SafeInvokeShapeMatchWindow(() =>
                            {
                                if (Frm_ShapeMatchTool.Instance.Visible)
                                    DisplayMatchResult(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, L_resultList[resultIndex], resultIndex);
                            });
                        }

                        //显示结果（调度到UI线程填充表格，避免后台线程跨线程操作DataGridView）
                        int rowIndex = i;
                        SafeInvokeShapeMatchWindow(() =>
                        {
                            if (Frm_ShapeMatchTool.Instance.Visible)
                            {
                                int index = Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows.Add();
                                Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[0].Value = rowIndex + 1;
                                Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[1].Value = Math.Round((double)L_resultList[rowIndex].Socre, 3);
                                Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[2].Value = Math.Round((double)L_resultList[rowIndex].Row, 3);
                                Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[3].Value = Math.Round((double)L_resultList[rowIndex].Col, 3);
                                Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[4].Value =
                                    Math.Round(L_resultList[rowIndex].Angle * 180.0 / Math.PI, 3);
                                Application.DoEvents();
                            }
                        });

                    }

                    if (L_resultList.Count > 0)
                    {
                        MatchResult previewResult = L_resultList[0];
                        SafeInvokeShapeMatchWindow(() => ShowMatchedImagePreview(previewResult));
                    }
                    else
                        SafeInvokeShapeMatchWindow(() =>
                        {
                            if (Frm_ShapeMatchTool.Instance.Visible)
                                HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                        });



                    if (L_result.Count == 0)
                    {
                        //如果没有匹配到模板，也添加一个值为0的结果
                        XYU matchResult = new XYU();
                        matchResult.Point.X = 0;
                        matchResult.Point.Y = 0;
                        matchResult.U = 0;
                        L_result.Add(matchResult);
                        toolRunStatu = ToolRunStatu.未匹配到模板;

                        return;
                    }
                    toolPar.ResultPar.位置 = L_result;
                    toolPar.ResultPar.结果数量 = L_result.Count;

                    if (runTool && !showCross && !showFeature && !showTemplate &&
                        !showMatchBox && !showSearchRegion && !showIndex)
                    {
                        toolRunStatu = ToolRunStatu.运行成功但是所有的特征都被设置不显示;
                        return;
                    }

                    if (rows.TupleLength() != matchNum)
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Create_Template : ToolRunStatu.匹配数量不足);
                        return;
                    }

                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 将模板匹配窗口的显示操作调度到 UI 线程执行。
        /// 流程运行时 Run 在后台线程执行，直接操作 HALCON 窗口/DataGridView
        /// 会抛 #5100 set_line_width 等异常，导致图像不刷新、流程中断。
        /// </summary>
        private void SafeInvokeShapeMatchWindow(Action action)
        {
            Frm_ShapeMatchTool form = Frm_ShapeMatchTool.CurrentInstance;
            if (form == null || form.IsDisposed)
                return;
            TryPostControlAction(form, action);
        }



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
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        public class ResultPar
        {
            private List<XYU> _位置;

            public List<XYU> 位置
            {
                get { return _位置; }
                set { _位置 = value; }
            }

            private int _结果数量;

            public int 结果数量
            {
                get { return _结果数量; }
                set { _结果数量 = value; }
            }
        }


    }

}
