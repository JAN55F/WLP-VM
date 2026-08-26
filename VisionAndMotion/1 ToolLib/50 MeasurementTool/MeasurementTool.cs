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

namespace VMPro
{
    [Serializable]
    internal class MeasurementTool : ToolBase
    {
        public MeasurementTool()
        {
            //单元大小分别为：6,16,26,36
            HObject circle;
            HOperatorSet.GenCircle(out circle, 20, 20, 12);
            brush_region = circle;
            HOperatorSet.GenEmptyObj(out  final_region);
            brush_region111 = circle;
            HOperatorSet.GenEmptyObj(out  final_region111);
        }
        public ToolPar toolPar = new ToolPar();
        internal void ShowStandardImage()
        {
            Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);

            HTuple row, col, row1, col1;
            HOperatorSet.GetPart(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, out row, out col, out row1, out col1);
            DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "训练图像", 12, row + (row1 - row) / 30, col + (col1 - col) / 30, "blue", "false");
        }
        internal double minScale = 0.8;
        internal double maxScale = 1.2;
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
        /// 显示结果序号
        /// </summary>
        internal bool showIndex = true ;
        internal bool showSearchRegion = true;
        /// <summary>
        /// 模板句柄                                                   
        /// </summary>
        internal HTuple modelID = -1;
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
        /// 角度范围
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
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                    HalconPaint.HalconTool.set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 15, "sans", new HTuple("true"), new HTuple("false"));
                    HalconPaint.HalconTool.disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, brushType + " 笔刷创建成果", "window", 20, 20, "blue", "false");
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(ho_temp_brush, "yellow");
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
                    //解锁功能区
                    //////groupBox_tool.Enabled = true;
                    Thread.Sleep(1000);
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
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
                HOperatorSet.GenEmptyObj(out final_region);
                HOperatorSet.GenEmptyObj(out final_region111);
                templateRegion = null;
                SearchRegion = null;
                templateRegion = null;
                reducedImage = null;
                searchRegionType = RegionType.AllImage;
                L_regions = new List<ROI>();

                HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispImageFit();
                if (modelID != -1)
                    HOperatorSet.ClearShapeModel(modelID);
                modelID = -1;

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
                Frm_ShapeMatchTool.Instance.nud_angleRange.Value = 60;

                Frm_ShapeMatchTool.Instance.cbx_searchRegionType.TextStr = string.Empty;
                Frm_ShapeMatchTool.Instance.cbx_showTemplate.Checked = true;
                Frm_ShapeMatchTool.Instance.ckb_showCross.Checked = true;
                Frm_ShapeMatchTool.Instance.ckb_showFeature.Checked = true;
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
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
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


                HObject outBoundary, inBoundary;
                HOperatorSet.Boundary(templateRegion, out outBoundary, "inner_filled");
                HOperatorSet.Boundary(templateRegion, out inBoundary, "outer");


                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                //////HOperatorSet.DispObj(outBoundary, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                //////HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple(4));
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(totalRegion, "green");
                HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, "green");
                HOperatorSet.DispObj(totalRegion, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
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
                if (CreateTemplate() == 0)
                {
                    HOperatorSet.GetShapeModelContours(out contour, modelID, (HTuple)1);
                    HTuple area, row, col;
                    HOperatorSet.AreaCenter(totalRegion, out area, out row, out col);
                    HTuple homMat2D;
                    HOperatorSet.HomMat2dIdentity(out homMat2D);
                    HOperatorSet.HomMat2dTranslate(homMat2D, row, col, out homMat2D);
                    HOperatorSet.AffineTransContourXld(contour, out contour, homMat2D);
                    //////GetImageWindowControl().hwc_imageWindow.DispObj(contour, "orange");
                    //ContrastChanged();

                    //在模板窗口显示模板
                    HTuple row1, col1, row2, col2;
                    HOperatorSet.SmallestRectangle1(totalRegion, out row1, out col1, out row2, out col2);
                    HObject outRectangle1;
                    HOperatorSet.GenRectangle1(out outRectangle1, row1 - 20, col1 - 20, row2 + 20, col2 + 20);
                    HObject imageReduced;
                    HOperatorSet.ReduceDomain(toolPar.InputPar.图像, outRectangle1, out imageReduced);
                    //  HOperatorSet.CropPart (toolPar.InputPar.图像,out imageReduced, row1 - 20, col1 - 20, row2 + 20, col2 + 20);

                    HObject outBoundary, inBoundary;
                    HOperatorSet.Boundary(templateRegion, out outBoundary, "inner_filled");
                    HOperatorSet.Boundary(templateRegion, out inBoundary, "outer");

                    HOperatorSet.SetSystem("flush_graphic", "true");
                    HOperatorSet.ClearWindow(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                    Application.DoEvents();
                    HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, row1 - 20, col1 - 20, row2 + 20, col2 + 20);
                    HOperatorSet.DispObj(imageReduced, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                    HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("margin"));

                    HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("green"));
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple());
                    HOperatorSet.DispObj(outBoundary, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple(4));
                    HOperatorSet.DispObj(templateRegion, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);


                    HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple());
                    HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("orange"));
                    HOperatorSet.DispObj(contour, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                    HOperatorSet.SetSystem("flush_graphic", "false");
                    //standardImage = toolPar.InputPar.输入图像;
                    Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
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
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
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
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(rectangle1, "green");

                if (templateRegion == null)
                {
                    templateRegion = rectangle1;
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.Union2(templateRegion, rectangle1, out templateRegion);
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionSub.Checked)
                {
                    HOperatorSet.Difference(templateRegion, rectangle1, out templateRegion);
                }
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
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
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
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
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(rectangle2, "green");

                if (templateRegion == null)
                {
                    templateRegion = rectangle2;
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.Union2(templateRegion, rectangle2, out    templateRegion);
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionSub.Checked)
                {
                    HOperatorSet.Difference(templateRegion, rectangle2, out    templateRegion);
                }
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");

                
                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle2.BackColor = Color.FromArgb(46, 141, 230);
                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;
            }
            catch (Exception ex)
            {
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
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
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
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(circle, "green");

                if (templateRegion == null)
                {
                    templateRegion = circle;
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.Union2(templateRegion, circle, out   templateRegion);
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionSub.Checked)
                {
                    HOperatorSet.Difference(templateRegion, circle, out   templateRegion);
                }

                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");
                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionCircle.BackColor = Color.FromArgb(46, 141, 230);
                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;

            }
            catch (Exception ex)
            {
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
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
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
                HObject ellipse;
                HOperatorSet.GenEllipse(out ellipse, row, col, angle, length1, length2);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(ellipse, "green");

                if (templateRegion == null)
                {
                    templateRegion = ellipse;
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.Union2(templateRegion, ellipse, out     templateRegion);
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionSub.Checked)
                {
                    HOperatorSet.Difference(templateRegion, ellipse, out   templateRegion);
                }

                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");
                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionEllipse.BackColor = Color.FromArgb(46, 141, 230);

                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;
            }
            catch (Exception ex)
            {
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
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
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
                HOperatorSet.DrawRegion(out region, Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID);
                //////GetImageWindowControl().SetDrawMode(false);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(region, "green");

                if (templateRegion == null)
                {
                    templateRegion = region;
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionAdd.Checked)
                {
                    HOperatorSet.Union2(templateRegion, region, out    templateRegion);
                }
                else if (Frm_ShapeMatchTool.Instance.rdo_templateRegionSub.Checked)
                {
                    HOperatorSet.Difference(templateRegion, region, out     templateRegion);
                }

                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(templateRegion, "green");
                DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "若绘制已完成，请点击学习按钮进行学习", 13, row111 + (row1 - row111) / 30, col111 + (col1 - col111) / 30, "blue", "false");
                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionAny.BackColor = Color.FromArgb(46, 141, 230);

                Frm_ShapeMatchTool.Instance.tbc_shapeMatch.Enabled = true;
                Frm_ShapeMatchTool.Instance.toolStrip1.Enabled = true;
            }
            catch (Exception ex)
            {
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
                    default :
                        L_regions.Clear();
                        searchRegionType = RegionType.AllImage ;
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
                int rowIndex = e.RowIndex;
                string index = Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[rowIndex].Cells[0].Value.ToString();
                double row = Convert.ToDouble(Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[rowIndex].Cells[2].Value);
                double col = Convert.ToDouble(Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[rowIndex].Cells[3].Value);
                double angle = Convert.ToDouble(Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[rowIndex].Cells[4].Value);


                //显示区域
                HTuple homMat2D;
                HOperatorSet.HomMat2dIdentity(out homMat2D);
                HTuple area1, row1, column1;
                HOperatorSet.AreaCenter(templateRegion, out area1, out row1, out column1);
                HOperatorSet.HomMat2dTranslate(homMat2D, (HTuple)(-row1), (HTuple)(-column1), out homMat2D);
                HOperatorSet.HomMat2dRotate(homMat2D, angle, (HTuple)0, (HTuple)0, out homMat2D);
                HObject rectangle1AfterTrans;
                HOperatorSet.HomMat2dTranslate(homMat2D, row, col, out homMat2D);
                HOperatorSet.AffineTransRegion(templateRegion, out rectangle1AfterTrans, homMat2D, "nearest_neighbor");
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(rectangle1AfterTrans, "green");

                //显示中心十字架
                HOperatorSet.SetLineWidth(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple(1));
                HObject cross;
                HOperatorSet.GenCrossContourXld(out cross, row, col, 20, angle);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(cross, "blue");

                //显示特征
                HObject countor;
                HOperatorSet.GetShapeModelContours(out countor, modelID, new HTuple(1));
                HOperatorSet.HomMat2dIdentity(out homMat2D);
                HOperatorSet.VectorAngleToRigid(0, 0, 0, row, col, angle, out homMat2D);
                HObject countorAfterTrans;
                HOperatorSet.AffineTransContourXld(countor, out countorAfterTrans, homMat2D);
                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(countorAfterTrans, "orange");
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
                HObject template;
                totalRegion = templateRegion;
                if (templateRegion == null)
                {
                    HOperatorSet.GenEmptyObj(out templateRegion);
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
                HOperatorSet.ReduceDomain(toolPar.InputPar.图像, totalRegion, out template);
                try
                {
                    HOperatorSet.CreateScaledShapeModel(template,
                                                 (HTuple)"auto",
                                                ((HTuple)startAngle).TupleRad(),
                                                 ((HTuple)angleRange).TupleRad(),
                                                 (HTuple)("auto"),
                                                 minScale,
                                                 maxScale,
                                                 "auto",
                                                 (HTuple)"auto",
                                                 (HTuple)polarity,
                                                  Frm_ShapeMatchTool.Instance.ckb_autoContrast.Checked ? (HTuple)"auto" : (HTuple)contrast,
                                                 (HTuple)"auto",
                                                  out modelID);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("#8510:"))      //特征过少，Halcon报错编号8510
                    {
                        Frm_ShapeMatchTool.Instance.label4.Text = "状态：" + "特征过少，无法完成训练（错误代码：0201）";
                        return 1;
                    }
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
                SearchRegion = null;
                searchRegionType = RegionType.AllImage;
                Frm_ShapeMatchTool.Instance.cbx_searchRegionType.TextStr = "";
                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
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
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Create_Template : ToolRunStatu.未创建模板);
                        return;
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
                        HTuple temp;
                        HOperatorSet.FindScaledShapeModel(image,
                                                   (HTuple)modelID,
                                                   ((HTuple)startAngle).TupleRad(),
                                                   ((HTuple)angleRange - startAngle).TupleRad(),
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
                    catch
                    {
                        CreateTemplate();
                        HTuple temp;
                        HOperatorSet.FindScaledShapeModel(image,
                                                   (HTuple)modelID,
                                                   ((HTuple)startAngle).TupleRad(),
                                                   ((HTuple)angleRange - startAngle).TupleRad(),
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

                    //重新显示图像
                    if (updateImage)
                    {
                        if (runTool)
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                        else
                        {
                            ShowImage(toolPar.InputPar.图像);
                            if (Frm_ShapeMatchTool.Instance.Visible)
                            {
                                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                            }
                        }
                    }
                    if (Frm_ShapeMatchTool.Instance.Visible)
                        Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);

                    if (Frm_ShapeMatchTool.Instance.Visible)
                        Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows.Clear();

                    if (rows.TupleLength() > 0)
                    {
                        for (int i = 0; i < rows.TupleLength(); i++)
                        {
                            MatchResult matchResult = new MatchResult();
                            matchResult.Row = Math.Round((double)rows[i], 3);
                            matchResult.Col = Math.Round((double)cols[i], 3);
                            matchResult.Angle = Math.Round((double)angles[i], 3);
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

                    if (SearchRegion != null && showSearchRegion)
                    {
                        if (runTool)
                        {
                            HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "blue");
                            Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                        }
                        else
                        {
                            GetImageWindowControl().hwc_imageWindow.DispObj(L_regions[0].getRegion(), "blue");
                            HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("blue"));
                        }
                    }

                    for (int i = 0; i < L_resultList.Count; i++)
                    {
                        //显示匹配特征
                        HOperatorSet.SetLineStyle(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                        if (showFeature)
                        {
                            HObject countor;
                            HOperatorSet.GetShapeModelContours(out countor, modelID, new HTuple(1));
                            HTuple homMat2D;
                            HOperatorSet.HomMat2dIdentity(out homMat2D);
                            HOperatorSet.HomMat2dTranslate(homMat2D, L_resultList[i].Row, L_resultList[i].Col, out homMat2D);
                            HOperatorSet.HomMat2dRotate(homMat2D, (HTuple)L_resultList[i].Angle, (HTuple)L_resultList[i].Row, (HTuple)L_resultList[i].Col, out homMat2D);
                            HObject countorAfterTrans;
                            HOperatorSet.AffineTransContourXld(countor, out countorAfterTrans, homMat2D);
                            if (runTool)
                                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(countorAfterTrans, "orange");
                            else
                            {
                                GetImageWindowControl().hwc_imageWindow.DispObj(countorAfterTrans, "orange");

                            }
                            if (Frm_ShapeMatchTool.Instance.Visible)
                                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(countorAfterTrans, "orange");

                        }

                        //显示结果
                        if (Frm_ShapeMatchTool.Instance.Visible)
                        {
                            int index = Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows.Add();
                            Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[0].Value = i + 1;
                            Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[1].Value = Math.Round((double)L_resultList[i].Socre, 3);
                            Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[2].Value = Math.Round((double)L_resultList[i].Row, 3);
                            Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[3].Value = Math.Round((double)L_resultList[i].Col, 3);
                            Frm_ShapeMatchTool.Instance.dgv_matchResult.Rows[index].Cells[4].Value = Math.Round((double)L_resultList[i].Angle, 3);
                            Application.DoEvents();
                        }

                        if (showTemplate)
                        {
                            HTuple area, row, col;
                            HOperatorSet.AreaCenter(totalRegion, out area, out row, out col);
                            HTuple homMat2D1;
                            HOperatorSet.HomMat2dIdentity(out homMat2D1);
                            HOperatorSet.HomMat2dTranslate(homMat2D1, -row, -col, out homMat2D1);
                            double roation = Math.Round(L_resultList[i].Angle, 3);
                            HOperatorSet.HomMat2dRotate(homMat2D1, roation, 0, 0, out homMat2D1);
                            HObject rectangle1AfterTrans;
                            HOperatorSet.HomMat2dTranslate(homMat2D1, L_resultList[i].Row, L_resultList[i].Col, out homMat2D1);
                            HOperatorSet.AffineTransRegion(totalRegion, out rectangle1AfterTrans, homMat2D1, "nearest_neighbor");
                            if (runTool)
                            {
                                HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "margin");
                                Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayHobject(rectangle1AfterTrans, "green");
                            }
                            else
                                GetImageWindowControl().hwc_imageWindow.viewWindow.displayHobject(rectangle1AfterTrans, "green");

                            if (Frm_ShapeMatchTool.Instance.Visible)
                            {
                                HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, "margin");
                                Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayHobject(rectangle1AfterTrans, "green");
                            }
                        }

                        //显示中心十字架和序号
                        if (showCross || showIndex)
                        {
                            HTuple area, row, col;
                            HOperatorSet.AreaCenter(totalRegion, out area, out row, out col);
                            HTuple homMat2D1;
                            HOperatorSet.HomMat2dIdentity(out homMat2D1);
                            HOperatorSet.HomMat2dTranslate(homMat2D1, -row, -col, out homMat2D1);
                            double roation = Math.Round(L_resultList[i].Angle, 3);
                            HOperatorSet.HomMat2dRotate(homMat2D1, roation, 0, 0, out homMat2D1);
                            HOperatorSet.HomMat2dTranslate(homMat2D1, L_resultList[i].Row, L_resultList[i].Col, out homMat2D1);
                            SetColor(jobName, "blue");
                            HOperatorSet.AffineTransPoint2d(homMat2D1, row, col, out row, out col);
                            HOperatorSet.SetLineWidth(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple(1));


                            //HObject cross;
                            //HOperatorSet.GenCrossContourXld(out cross, row, col, new HTuple(20), roation);


                            HObject cross;
                            HTuple row111, col111, row11, col11;
                            HOperatorSet.GetPart(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, out row111, out col111, out row11, out col11);
                            HOperatorSet.GenCrossContourXld(out cross, row, col, new HTuple((row11 - row111) / 90.0 + 1), new HTuple(0));


                            if (runTool)
                            {
                                if (showCross)
                                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(cross, "blue");
                                if (showIndex)
                                {
                                    set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 10, "sans", "true", "false");
                                    disp_message(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, i + 1, row + 20, col + 20, "blue", "true");
                                }
                            }
                            else
                            {
                                if (showCross)
                                    GetImageWindowControl().hwc_imageWindow.DispObj(cross, "blue");
                                if (Frm_ShapeMatchTool.Instance.Visible)
                                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(cross, "blue");

                                if (showIndex)
                                {
                                    GetImageWindowControl().set_display_font(10, "sans", "true", "false");
                                    DispMessage(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, i + 1, 12, row + 20, col + 20, "blue", "true");
                                }
                                if (Frm_ShapeMatchTool.Instance.Visible)
                                {
                                    set_display_font(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, 10, "sans", "true", "false");
                                    DispMessage(Frm_ShapeMatchTool.Instance.hWindow_Final1.HWindowHalconID, i + 1, 12, row + 20, col + 20, "blue", "true");
                                }
                            }
                        }
                    }



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

                    if (runTool && !showCross && !showFeature && !showTemplate)
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
