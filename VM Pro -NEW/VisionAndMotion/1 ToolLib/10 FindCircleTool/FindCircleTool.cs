using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using VersionMethods;
using System.Threading;
using System.Windows.Forms;
using ViewWindow.Model;
using System.Drawing;
using System.Diagnostics;

namespace VMPro
{
    [Serializable]
    internal class FindCircleTool : ToolBase
    {
        /*
         * 圆查找工具学习入口
         * 
         * 这个类是“查找圆”工具的核心算法类，界面文件 Frm_FindCircleTool.cs 只负责把参数写进这里。
         * 建议阅读顺序：
         * 1. 构造函数：新建工具时如何生成默认预期圆 ROI，以及默认参数如何初始化。
         * 2. DrawExpectCircle()：用户点击“绘制/编辑预期圆”时，如何把 ROI 放到 Halcon 窗口。
         * 3. ShowContour()：只预览卡尺、边缘点和拟合圆，不写流程输出，适合调参时看效果。
         * 4. Run()：真正运行工具，读取输入图像/跟随位姿，调用 Halcon Metrology，写入 ResultPar 输出。
         * 5. Work()：用画笔生成 final_region，用来屏蔽不希望参与找圆的区域。
         * 
         * 坐标约定：
         * Halcon 的点坐标一般叫 row/column，即图像行/列。项目里的 XY 命名历史上不完全严格：
         * 这里 ResultPar.圆心 的 X 存 row，Y 存 column。修改输出含义前，要同步检查下游工具取值。
         */
        internal FindCircleTool()
        {
            HObject image = null;
            string s_jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
            // 新建圆查找工具时，优先取当前流程里第一个采集图像工具的结果图。
            // 目的只是为了让默认 ROI 落在真实图像中心附近，减少用户第一次拖动的工作量。
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
                double centerRow = h.D / 2.0;
                double centerCol = w.D / 2.0;
                double radius = Math.Max(30.0, Math.Min(w.D, h.D) / 6.0);
                // L_regions 保存界面上可拖动的圆 ROI。后续真正找圆时会读取
                // L_regions[0].getModelData()，得到预期圆的 row、column、radius。
                Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.genCircle(centerRow, centerCol, radius, ref this.L_regions);
                // ringRadiusLength 是卡尺沿半径方向的半长。默认按圆半径 1/4 给值，
                // 让新建工具时卡尺有足够搜索范围，但不至于覆盖太大的无关区域。
                this.ringRadiusLength = Math.Max(5, (int)(radius / 4.0));
            }
            else
            {
                // 没有输入图时仍创建一个默认圆，保证工具界面打开后有 ROI 可编辑。
                Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.genCircle(400.0, 500.0, 160.0, ref this.L_regions);
            }
            Frm_FindCircleTool.Instance.regions = this.L_regions;

            // templatePose 是“学习/绘制预期圆时”的基准位姿。
            // 如果运行时输入了“跟随”位姿，Run() 会用基准位姿 -> 当前位姿的刚性变换移动预期圆。
            XYU xyu = new XYU();
            xyu.Point.X = 0;
            xyu.Point.Y = 0;
            templatePose.Add(xyu);

            // brush_region/final_region 用于涂抹屏蔽区域：
            // brush_region 是画笔形状，final_region 是用户最终画出的屏蔽区域。
            HObject circle;
            HOperatorSet.GenCircle(out circle, 50, 50, 30);
            brush_region = circle;
            HOperatorSet.GenEmptyObj(out  final_region);



        }
        public bool drawMode = false;
        internal List<ViewWindow.Model.ROI> L_regions = new List<ViewWindow.Model.ROI>();
        internal void Work(object sender)
        {
            try
            {
                string actionType = ((RadioButton)sender).Name;// 画区域 或者 擦除
                Frm_FindCircleTool.Instance.hWindow_Final1.ContextMenuStrip = null;
                Frm_FindCircleTool.Instance.hWindow_Final1.DrawModel = true;
                Frm_FindCircleTool.Instance.hWindow_Final1.Focus();
                //////groupBox_tool.Enabled = false;

                HTuple hv_Button = null;
                HTuple hv_Row = null, hv_Column = null;
                HTuple areaBrush, rowBrush, columnBrush, homMat2D;


                HObject brush_region_affine = new HObject();
                HObject ho_Image = new HObject(toolPar.InputPar.图像);
                try
                {
                    // 进入涂抹模式前，必须先有画笔区域。画笔区域随后会被平移到鼠标位置。
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




                    HWindowControl temp = new HWindowControl();
                    HTuple row2, col2, w, h;
                    HTuple w1, h1;
                    HOperatorSet.GetImageSize(toolPar.InputPar.图像, out w1, out h1);
                    HOperatorSet.GetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, out row2, out col2, out h, out w);
                    HOperatorSet.SetWindowExtents(temp.HalconWindow, 0, 0, w1, h1);
                    HOperatorSet.SetPart(temp.HalconWindow, row2, col2, h, w);
                    HOperatorSet.ClearWindow(temp.HalconWindow);
                    HOperatorSet.DispObj(toolPar.InputPar.图像, temp.HalconWindow);
                    HOperatorSet.SetLineStyle(temp.HalconWindow, new HTuple());
                    HOperatorSet.SetColor(temp.HalconWindow, "green");
                    HOperatorSet.SetLineWidth(temp.HalconWindow, 3);




                    //////HTuple row2, col2, w, h;
                    //////HTuple w1, h1;
                    //////HOperatorSet.GetImageSize(inputImage, out w1, out h1);
                    //////HOperatorSet.GetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, out row2, out col2, out h, out w);
                    //////HOperatorSet.SetWindowExtents(temp.HalconWindow, 0, 0, w1, h1);
                    //////HOperatorSet.SetPart(temp.HalconWindow, row2, col2, h, w);
                    //////HOperatorSet.ClearWindow(temp.HalconWindow);
                    //////HOperatorSet.DispObj(inputImage, temp.HalconWindow);
                    //////HOperatorSet.SetLineStyle(temp.HalconWindow, new HTuple());
                    //////HOperatorSet.SetColor(temp.HalconWindow, "green");
                    //////HOperatorSet.SetLineWidth(temp.HalconWindow, 3);




                    HObject contours;
                    HTuple row, col;


                    // 涂抹前先按当前 ROI 和跟随位姿生成“运行时预期圆”，
                    // 再创建 Halcon Metrology 模型用于显示卡尺和边缘点，帮助用户决定哪里需要屏蔽。
                    newExpecCircleRow.Clear();
                    newExpectCircleCol.Clear();
                    newExpectCircleRadius.Clear();
                    if (toolPar.InputPar.跟随 != null)
                    {


                        HTuple _homMat2D;
                        HOperatorSet.VectorAngleToRigid(templatePose[0].Point.X, templatePose[0].Point.Y, templatePose[0].U, toolPar.InputPar.跟随[0].Point.X, toolPar.InputPar.跟随[0].Point.Y, toolPar.InputPar.跟随[0].U, out _homMat2D);
                        // 对预期圆心做刚性变换。半径不变，因为这里只支持平移+旋转跟随，不做缩放。
                        HTuple tempR, tempC;
                        HOperatorSet.AffineTransPixel(_homMat2D, (HTuple)L_regions[0].getModelData()[0], (HTuple)L_regions[0].getModelData()[1], out tempR, out tempC);
                        newExpecCircleRow.Add(tempR);
                        newExpectCircleCol.Add(tempC);
                        newExpectCircleRadius.Add(L_regions[0].getModelData()[2].D);

                    }
                    else
                    {
                        newExpecCircleRow.Add(L_regions[0].getModelData()[0]);
                        newExpectCircleCol.Add(L_regions[0].getModelData()[1]);
                        newExpectCircleRadius.Add(L_regions[0].getModelData()[2]);
                    }

                    HTuple handleID;
                    HOperatorSet.CreateMetrologyModel(out handleID);
                    HTuple width, height;
                    HOperatorSet.GetImageSize(toolPar.InputPar.图像, out width, out height);
                    HOperatorSet.SetMetrologyModelImageSize(handleID, width[0], height[0]);
                    HTuple index;
                    // AddMetrologyObjectCircleMeasure 定义“沿一个预期圆找边”的测量对象。
                    // ringRadiusLength 控制卡尺径向搜索长度，caliperWidth 后面用 measure_length2 控制卡尺切向宽度。
                    HOperatorSet.AddMetrologyObjectCircleMeasure(handleID, newExpecCircleRow[0], newExpectCircleCol[0], newExpectCircleRadius[0], new HTuple(ringRadiusLength), new HTuple(5), new HTuple(1), new HTuple(30), new HTuple(), new HTuple(), out index);
                    //////HTuple hom;
                    //////HTuple temp1 = (HTuple)L_regions[0].getModelData()[0];
                    //////HOperatorSet.VectorAngleToRigid(LastExpectRow, LastExpectCol, 0, (HTuple)L_regions[0].getModelData()[0], (HTuple)L_regions[0].getModelData()[1],0,out hom);
                    //////HObject contoursTransed;
                    //////HOperatorSet.AffineTransContourXld(contours ,out contoursTransed,hom );
                    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_transition"), new HTuple(polarity));
                    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("num_measures"), new HTuple(cliperNum));
                    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length1"), new HTuple(ringRadiusLength));
                    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length2"), new HTuple(caliperWidth));
                    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_threshold"), new HTuple(threshold));
                    //////HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_select"), new HTuple(edgeSelect));
                    //////HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("min_score"), new HTuple(minScore));
                    HOperatorSet.ApplyMetrologyModel(toolPar.InputPar.图像, handleID);
                    HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row, out col);

                    HOperatorSet.DispObj(contours, temp.HalconWindow);

                    //HOperatorSet.ApplyMetrologyModel(inputImage, handleID);
                    //HTuple row1, col1;
                    //HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row1, out col1);
                    //HObject cross;
                    //HOperatorSet.GenCrossContourXld(out cross, row1, col1, new HTuple(20), new HTuple(0));
                    //HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("orange"));
                    //HOperatorSet.DispObj(cross, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);

                    //把点显示出来

                    HTuple row1, col1;
                    HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row1, out col1);


                    HObject cross;
                    HTuple row111, col111, row11, col11;
                    HOperatorSet.GetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row11, out col11);
                    HOperatorSet.GenCrossContourXld(out cross, row, col, new HTuple((row11 - row111) / 90.0 + 1), new HTuple(0));        //小细节：我们要想使无论图像像素多大，显示的十字大小都是一样的，就需要得出y=kx+b中的k和b



                    HOperatorSet.SetColor(temp.HalconWindow, new HTuple("orange"));
                    HOperatorSet.DispObj(cross, temp.HalconWindow);

                    HOperatorSet.ClearMetrologyModel(handleID);








                    //////HOperatorSet.DispObj(contour, temp.HalconWindow);
                    HObject image;
                    HOperatorSet.DumpWindowImage(out image, temp.HalconWindow);
                    HOperatorSet.ClearWindow(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                    Frm_FindCircleTool.Instance.hWindow_Final1.DispImageFit();
                    HOperatorSet.DispObj(image, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                    //Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(image);



                    // return;

                    ho_Image = new HObject(image);




                    //HTuple row1, col1, row2, col2;
                    //HOperatorSet.SmallestRectangle1(templateRegion, out row1, out col1, out row2, out col2);
                    //HObject outRectangle1;
                    //HOperatorSet.GenRectangle1(out outRectangle1, row1 - 20, col1 - 20, row2 + 20, col2 + 20);
                    //HObject imageReduced;
                    //HOperatorSet.ReduceDomain(image, outRectangle1, out imageReduced);
                    //Frm_FindCircleTool.Instance.hWindow_Final1.ClearWindow();
                    //HOperatorSet.SetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, row1 - 20, col1 - 20, row2 + 20, col2 + 20);
                    //HOperatorSet.DispObj(imageReduced, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                    //////HOperatorSet.SetDraw(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("margin"));
                    //////HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                    //////HOperatorSet.DispObj(templateRegion, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                    //////HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("orange"));
                    //////HOperatorSet.DispObj(contour, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                    Application.DoEvents();


                    try
                    {
                        HObject image1, image2, resultImage;
                        resultImage = new HObject();
                        HOperatorSet.PaintRegion(final_region, toolPar.InputPar.图像, out  image1, 10, "fill");
                        HOperatorSet.Compose3(image, image1, image1, out resultImage);
                        HOperatorSet.DispObj(resultImage, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                        HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                        HOperatorSet.DispObj(final_region, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                    }
                    catch { }



                    //Test();

                    Application.DoEvents();


                    //Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(final_region);

                    //画出笔刷
                    switch (actionType)
                    {
                        case "radioButton2":
                            // radioButton2：添加屏蔽区域。屏蔽区域会在 Run() 中从图像域里扣掉。
                            HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "blue");

                            break;
                        case "radioButton3":
                            // radioButton3：擦除已有屏蔽区域，相当于 Difference(final_region, brush)。
                            HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "red");

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
                            HOperatorSet.GetMposition(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, out hv_Row, out hv_Column, out hv_Button);
                        }
                        catch (HalconException ex)
                        {
                            try
                            {
                                hv_Button = 0;
                                HOperatorSet.DispObj(image, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                HOperatorSet.DispObj(final_region, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                HObject image1, image2, resultImage;
                                resultImage = new HObject();
                                HObject iamgeR, iamgeG, iamgeB;
                                HOperatorSet.Decompose3(image, out iamgeR, out  iamgeG, out  iamgeB);
                                HOperatorSet.PaintRegion(final_region, iamgeR, out  image1, 20, "fill");
                                HOperatorSet.Compose3(image, iamgeG, image1, out resultImage);
                                //HOperatorSet.SetPart(FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, row1 - 20, col1 - 20, row2 + 20, col2 + 20);

                                HOperatorSet.DispObj(resultImage, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                                HOperatorSet.DispObj(final_region, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);


                                HalconPaint.HalconTool.disp_message(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "涂抹结束后请重新学习", "window", 20, 20, "blue", "false");
                            }
                            catch
                            { }
                        }


                        HOperatorSet.SetSystem("flush_graphic", "false");
                        HOperatorSet.DispObj(ho_Image, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                        if (final_region.IsInitialized())
                        {
                            try
                            {
                                //////HOperatorSet.DispObj(final_region, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                HObject image1, image2, resultImage;
                                resultImage = new HObject();
                                HObject iamgeR, iamgeG, iamgeB;
                                HOperatorSet.Decompose3(image, out iamgeR, out  iamgeG, out  iamgeB);
                                HOperatorSet.PaintRegion(final_region, iamgeR, out  image1, 20, "fill");
                                HOperatorSet.Compose3(image, iamgeG, image1, out resultImage);
                                //HOperatorSet.SetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, row1 - 20, col1 - 20, row2 + 20, col2 + 20);

                                HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "blue");
                                HOperatorSet.DispObj(resultImage, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                                HOperatorSet.DispObj(final_region, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);








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
                            HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "yellow");
                            HOperatorSet.SetLineStyle(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple());
                            HOperatorSet.DispObj(brush_region_affine, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);

                            HOperatorSet.SetSystem("flush_graphic", "true");
                            HalconPaint.HalconTool.set_display_font(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, 15, "sans", new HTuple("true"), new HTuple("false"));
                            HalconPaint.HalconTool.disp_message(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "按下鼠标左键开始涂抹", "window", 20, 20, "blue", "false");
                            // disp_message(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "按下鼠标左键涂画,右键结束", 20, 20, "red", "false");
                            //  Test();

                            //1为鼠标左键
                            if (hv_Button == 1)
                            {

                                //画出笔刷
                                switch (actionType)
                                {
                                    case "radioButton2":
                                        {
                                            // 左键按下时，把当前鼠标位置的画笔区域合并到 final_region。
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
                                            // 擦除模式下，从 final_region 中减去当前画笔区域。
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
                            HalconPaint.HalconTool.set_display_font(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, 15, "sans", new HTuple("true"), new HTuple("false"));
                            HalconPaint.HalconTool.disp_message(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "请将鼠标移动到窗口内部", "window", 20, 20, "blue", "false");
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
                    Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(final_region, "blue");
                    Frm_FindCircleTool.Instance.hWindow_Final1.DrawModel = false;

                    //////groupBox_tool.Enabled = true;

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal HObject brush_region;
        internal HObject final_region;
        /// <summary>
        /// 显示卡尺
        /// </summary>
        internal bool displayCaliper = true;
        /// <summary>
        /// 显示特征点
        /// </summary>
        internal bool displayFeature = true;
        /// <summary>
        /// 显示结果圆
        /// </summary>
        internal bool displayCircle = true;
        /// <summary>
        /// 显示结果圆圆心
        /// </summary>
        internal bool displayCircleCenter = true;
        /// <summary>
        /// 期望圆圆心行坐标
        /// </summary>
        internal HTuple expectCircleRow = 300;
        /// <summary>
        /// 期望圆圆心列坐标
        /// </summary>
        internal HTuple expectCircleCol = 300;
        /// <summary>
        /// 期望圆半径
        /// </summary>
        internal HTuple expectCircleRadius = 200;

        //internal List<double> ResultCircleRow = new List<double>();

        //internal List<double> ResultCircleCol = new List<double>();

        //internal List<double> ResultCircleRadius = new List<double>();

        /// <summary>
        /// 起始角度
        /// </summary>
        internal double startAngle = 10;
        /// <summary>
        /// 结束角度
        /// </summary>
        internal double endAngle = 360;
        /// <summary>
        /// 运行工具时是否刷新输入图像
        /// </summary>
        internal bool updateImage = false;
        /// <summary>
        /// 圆环径向长度
        /// </summary>
        internal int ringRadiusLength = 30;
        internal int caliperWidth = 5;
        /// <summary>
        /// 边阈值
        /// </summary>
        internal int threshold = 30;
        internal List<XY> circleCenter = new List<XY>();
        internal int ignoreNum = 0;
        /// <summary>
        /// 卡尺
        /// </summary>
        internal HObject contours;
        /// <summary>
        /// 找边极性，从明到暗或从暗到明
        /// </summary>
        internal string polarity = "positive";
        internal string edgeSelect = "all";
        internal double minScore = 0.5;
        /// <summary>
        /// 卡尺数量
        /// </summary>
        internal int cliperNum = 30;
        /// <summary>
        /// 新的跟随姿态变化后的预期圆信息
        /// </summary>
        internal List<HTuple> newExpecCircleRow = new List<HTuple>(), newExpectCircleCol = new List<HTuple>(), newExpectCircleRadius = new List<HTuple>();
        /// <summary>
        /// 制作模板时的输入位姿
        /// </summary>
        internal List<XYU> templatePose = new List<XYU>();


        /// <summary>
        /// 刷新图像
        /// </summary>
        /// <param name="jobName">流程名</param>
        internal void UpdateImage(string jobName)
        {
            try
            {
                ShowImage(toolPar.InputPar.图像);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 多点拟合圆。
        /// 这是一个备用的三点组合平均算法：从所有非共线三点计算圆心，再求平均。
        /// 当前主流程 Run() 中使用的是 LeastSquaresFit()，这里保留给后续改进或对比算法使用。
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static PointF FitCenter(List<PointF> pts, double epsilon = 0.1)
        {
            try
            {
                double totalX = 0, totalY = 0;
                int setCount = 0;

                for (int i = 0; i < pts.Count; i++)
                {
                    for (int j = 1; j < pts.Count; j++)
                    {
                        for (int k = 2; k < pts.Count; k++)
                        {
                            double delta = (pts[k].X - pts[j].X) * (pts[j].Y - pts[i].Y) - (pts[j].X - pts[i].X) * (pts[k].Y - pts[j].Y);

                            if (Math.Abs(delta) > epsilon)
                            {
                                double ii = Math.Pow(pts[i].X, 2) + Math.Pow(pts[i].Y, 2);
                                double jj = Math.Pow(pts[j].X, 2) + Math.Pow(pts[j].Y, 2);
                                double kk = Math.Pow(pts[k].X, 2) + Math.Pow(pts[k].Y, 2);

                                double cx = ((pts[k].Y - pts[j].Y) * ii + (pts[i].Y - pts[k].Y) * jj + (pts[j].Y - pts[i].Y) * kk) / (2 * delta);
                                double cy = -((pts[k].X - pts[j].X) * ii + (pts[i].X - pts[k].X) * jj + (pts[j].X - pts[i].X) * kk) / (2 * delta);

                                totalX += cx;
                                totalY += cy;

                                setCount++;
                            }
                        }
                    }
                }

                if (setCount == 0)
                {
                    return PointF.Empty;
                }

                return new PointF((float)totalX / setCount, (float)totalY / setCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return PointF.Empty;
            }
        }
        public bool FitCircle(double[] X, double[] Y, out double RcX, out double RcY, out double R)
        {
            try
            {
                HTuple hTuple = new HTuple();
                HTuple hTuple2 = new HTuple();
                int num = 0;
                for (num = 0; num < X.Length; num++)
                {
                    // 过滤无效点，然后把点转成 Halcon 轮廓拟合需要的 HTuple。
                    // 注意这里 X/Y 的实际含义来自调用方，圆查找内部通常 X=row、Y=column。
                    if ((X[num] > 0.0) & (Y[num] > 0.0))//获得寻找到的模板中心装入hTuple2与hTuple
                    {
                        hTuple2.TupleConcat(X[num]);
                        hTuple.TupleConcat(Y[num]);
                    }
                }
                HObject contour;
                HOperatorSet.GenContourPolygonXld(out contour, hTuple, hTuple2);//使用模板中心生成多边形XLD轮廓
                HTuple row, column, radius, StartPhi, EndPhi, pointOrder;
                HOperatorSet.FitCircleContourXld(contour, "algebraic", -1, 0, 0, 3, 2, out  row, out  column, out  radius, out StartPhi, out EndPhi, out pointOrder);//拟合圆形
                //得出结果
                RcY = row;
                RcX = column;
                R = radius;

                contour.Dispose();
                return true;
            }
            catch
            {
                RcY = -1.0;
                RcX = -1.0;
                R = -1.0;
                return false;
            }
        }
        /// <summary>
        /// 绘制或重新显示预期圆。
        /// 用户点击界面上的“圆”按钮时进入这里：先显示输入图像，再把 L_regions 中的圆 ROI 放到窗口。
        /// 用户拖动这个 ROI 后，Hwindow_MouseUp 会把最新 ROI 写回 L_regions，Run() 就会按新位置找圆。
        /// </summary>
        /// <param name="jobName">流程名</param>
        public void DrawExpectCircle(string jobName)
        {
            try
            {



                Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);



                if (L_regions.Count == 0)
                    // 历史工程可能没有保存 ROI，兜底生成一个默认预期圆。
                    Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.genCircle(400.0, 500.0, 200.0, ref this.L_regions);

                else
                    Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                Frm_FindCircleTool.Instance.regions = L_regions;

                // 记录绘制预期圆时的“跟随”位姿。运行时如果输入了新的跟随位姿，
                // 会以这里记录的位姿为基准，把预期圆移动到当前工件位置。
                templatePose.Clear();
                if (toolPar.InputPar.跟随 != null && toolPar.InputPar.跟随.Count > 0)
                {
                    XYU temp = new XYU();
                    temp.Point.X = toolPar.InputPar.跟随[0].Point.X;
                    temp.Point.Y = toolPar.InputPar.跟随[0].Point.Y;
                    temp.U = toolPar.InputPar.跟随[0].U;
                    templatePose.Add(temp);
                }

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private object obj = new object();

        internal void ShowContour(bool showROI, bool trans = true)
        {
            HObject contours;
            HTuple row, col;



            // ShowContour 是调参预览：只显示卡尺、特征点和拟合轮廓，不写 ResultPar。
            // trans=true 表示应用“跟随”位姿，把预期圆从学习位置变换到当前位置；
            // trans=false 常用于打开工具时显示原始 ROI，避免预览时又改动用户正在编辑的 ROI。
            if (toolPar.InputPar.跟随.Count != 0)
            {
                if (trans)
                {
                    newExpecCircleRow.Clear();
                    newExpectCircleCol.Clear();
                    newExpectCircleRadius.Clear();
                    HTuple _homMat2D;
                    HOperatorSet.VectorAngleToRigid(templatePose[0].Point.X, templatePose[0].Point.Y, templatePose[0].U, toolPar.InputPar.跟随[0].Point.X, toolPar.InputPar.跟随[0].Point.Y, toolPar.InputPar.跟随[0].U, out _homMat2D);
                    // 对预期圆心做刚性变换；圆半径不变。
                    HTuple tempR, tempC;
                    HOperatorSet.AffineTransPixel(_homMat2D, (HTuple)L_regions[0].getModelData()[0], (HTuple)L_regions[0].getModelData()[1], out tempR, out tempC);
                    newExpecCircleRow.Add(tempR);
                    newExpectCircleCol.Add(tempC);
                    newExpectCircleRadius.Add(L_regions[0].getModelData()[2].D);
                }
                else
                {
                    newExpecCircleRow.Clear();
                    newExpectCircleCol.Clear();
                    newExpectCircleRadius.Clear();
                    newExpecCircleRow.Add(L_regions[0].getModelData()[0]);
                    newExpectCircleCol.Add(L_regions[0].getModelData()[1]);
                    newExpectCircleRadius.Add(L_regions[0].getModelData()[2]);
                }
            }
            else
            {
                newExpecCircleRow.Clear();
                newExpectCircleCol.Clear();
                newExpectCircleRadius.Clear();
                newExpecCircleRow.Add(L_regions[0].getModelData()[0]);
                newExpectCircleCol.Add(L_regions[0].getModelData()[1]);
                newExpectCircleRadius.Add(L_regions[0].getModelData()[2]);
            }

            HTuple handleID;
            HOperatorSet.CreateMetrologyModel(out handleID);
            HTuple width, height;
            HOperatorSet.GetImageSize(toolPar.InputPar.图像, out width, out height);
            HOperatorSet.SetMetrologyModelImageSize(handleID, width[0], height[0]);
            HTuple index;
            // 这里先创建一个圆测量对象，后续 SetMetrologyObjectParam 再覆盖核心调参项。
            // 修改查找稳定性时，优先看下面这些参数，而不是 AddMetrologyObjectCircleMeasure 的常量。
            HOperatorSet.AddMetrologyObjectCircleMeasure(handleID, newExpecCircleRow[0], newExpectCircleCol[0], newExpectCircleRadius[0], new HTuple(ringRadiusLength), new HTuple(5), new HTuple(1), new HTuple(30), new HTuple(), new HTuple(), out index);
            //////HTuple hom;
            //////HTuple temp1 = (HTuple)L_regions[0].getModelData()[0];
            //////HOperatorSet.VectorAngleToRigid(LastExpectRow, LastExpectCol, 0, (HTuple)L_regions[0].getModelData()[0], (HTuple)L_regions[0].getModelData()[1],0,out hom);
            //////HObject contoursTransed;
            //////HOperatorSet.AffineTransContourXld(contours ,out contoursTransed,hom );
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_transition"), new HTuple(polarity));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("num_measures"), new HTuple(cliperNum));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length1"), new HTuple(ringRadiusLength));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length2"), new HTuple(caliperWidth));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_threshold"), new HTuple(threshold));
            HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_select"), new HTuple(edgeSelect));
            // 参数含义：
            // measure_transition: 边缘极性，positive=暗到亮，negative=亮到暗。
            // num_measures: 沿圆周均匀布置的卡尺数量，越多越抗局部缺口，但耗时更高。
            // measure_length1: 卡尺沿半径方向搜索半长，预期圆不准时需要加大。
            // measure_length2: 卡尺沿圆切线方向宽度，边缘有噪声时可适当加大。
            // measure_threshold: 边缘强度阈值，过高会漏边，过低会被噪声干扰。
            // measure_select: 同一卡尺内多条边的选择策略，first/last/all。
            //////HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("min_score"), new HTuple(minScore));
            HOperatorSet.ApplyMetrologyModel(toolPar.InputPar.图像, handleID);
            HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row, out col);
            Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
            if (showROI)
                Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
            Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(contours, "blue");

            HOperatorSet.ApplyMetrologyModel(toolPar.InputPar.图像, handleID);

            if (displayFeature)
            {
                HTuple row1, col1;
                HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row1, out col1);
                //HObject cross;
                //HOperatorSet.GenCrossContourXld(out cross, row1, col1, new HTuple(20), new HTuple(0));



                HObject cross;
                HTuple row111, col111, row11, col11;
                HOperatorSet.GetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row11, out col11);
                HOperatorSet.GenCrossContourXld(out cross, row, col, new HTuple((row11 - row111) / 90.0 + 1), new HTuple(0));        //小细节：我们要想使无论图像像素多大，显示的十字大小都是一样的，就需要得出y=kx+b中的k和b
                Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(cross, "orange");
            }

            HObject circle;
            HOperatorSet.GetMetrologyObjectResultContour(out circle, handleID, new HTuple("all"), new HTuple("all"), new HTuple(1.5));
            Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(circle, "green");

            HOperatorSet.ClearMetrologyModel(handleID);

        }
        internal double LastExpectRow = 0;
        internal double LastExpectCol = 0;
        public Circle LeastSquaresFit(double[] X, double[] Y)
        {
            // 最小二乘拟合圆，用于 ignoreNum > 0 时剔除离群边缘点后重新拟合。
            // 传入点数少于 3 个无法唯一确定圆，直接返回 null。
            if (X.Length < 3)
            {
                return null;
            }
            double cent_x = 0.0,
                cent_y = 0.0,
                radius = 0.0;
            double sum_x = 0.0f, sum_y = 0.0f;
            double sum_x2 = 0.0f, sum_y2 = 0.0f;
            double sum_x3 = 0.0f, sum_y3 = 0.0f;
            double sum_xy = 0.0f, sum_x1y2 = 0.0f, sum_x2y1 = 0.0f;
            int N = X.Length;
            double x, y, x2, y2;
            for (int i = 0; i < N; i++)
            {
                x = X[i];
                y = Y[i];
                x2 = x * x;
                y2 = y * y;
                sum_x += x;
                sum_y += y;
                sum_x2 += x2;
                sum_y2 += y2;
                sum_x3 += x2 * x;
                sum_y3 += y2 * y;
                sum_xy += x * y;
                sum_x1y2 += x * y2;
                sum_x2y1 += x2 * y;
            }
            double C, D, E, G, H;
            double a, b, c;
            C = N * sum_x2 - sum_x * sum_x;
            D = N * sum_xy - sum_x * sum_y;
            E = N * sum_x3 + N * sum_x1y2 - (sum_x2 + sum_y2) * sum_x;
            G = N * sum_y2 - sum_y * sum_y;
            H = N * sum_x2y1 + N * sum_y3 - (sum_x2 + sum_y2) * sum_y;
            a = (H * D - E * G) / (C * G - D * D);
            b = (H * C - E * D) / (D * D - G * C);
            c = -(a * sum_x + b * sum_y + sum_x2 + sum_y2) / N;
            cent_x = a / (-2);
            cent_y = b / (-2);
            radius = Math.Sqrt(a * a + b * b - 4 * c) / 2;
            var result = new Circle();
            result.X = cent_x;
            result.Y = cent_y;
            result.R = radius;
            return result;
        }
        /// <summary>
        /// 运行工具。
        /// 这是流程执行时真正调用的圆查找入口：
        /// 1. 检查输入图像；
        /// 2. 根据“跟随”位姿计算运行时预期圆；
        /// 3. 创建 Halcon Metrology 圆测量对象；
        /// 4. 可选扣除 final_region 屏蔽区域；
        /// 5. 读取拟合结果，写入 toolPar.ResultPar，供下游工具连接使用。
        /// </summary>
        public override void Run(bool updateImage, bool runTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    if (toolPar.InputPar.图像 == null)
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Input_Image : ToolRunStatu.无输入图像);
                        return;
                    }
                    if (updateImage)
                    {
                        if (runTool)
                        {
                            Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                        }
                        else
                        {
                            //////ClearWindow(jobName);
                            //////ShowImage(jobName, inputImage);
                            // 流程运行（runTool=false）时，如果找圆窗体已打开，也同步更新窗体图像窗口。
                            SafeInvokeFindCircleWindow(() =>
                            {
                                Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(toolPar.InputPar.图像);
                                if (L_regions != null && L_regions.Count > 0)
                                    Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.displayROI(L_regions);
                            });
                        }
                    }

                    // 运行前根据当前工件位姿生成一组预期圆。
                    // 如果上游模板匹配输出多个 XYU，这里会为每个位姿各找一个圆。
                    newExpecCircleRow.Clear();
                    newExpectCircleCol.Clear();
                    newExpectCircleRadius.Clear();
                    if (toolPar.InputPar.跟随 != null)
                    {

                        for (int i = 0; i < toolPar.InputPar.跟随.Count; i++)
                        {
                            HTuple _homMat2D;
                            HOperatorSet.VectorAngleToRigid(templatePose[0].Point.X, templatePose[0].Point.Y, templatePose[0].U, toolPar.InputPar.跟随[i].Point.X, toolPar.InputPar.跟随[i].Point.Y, toolPar.InputPar.跟随[i].U, out _homMat2D);
                            // 对预期圆心做刚性变换，得到当前工件位置下的搜索圆。
                            HTuple tempR, tempC;
                            HOperatorSet.AffineTransPixel(_homMat2D, (HTuple)L_regions[0].getModelData()[0], (HTuple)L_regions[0].getModelData()[1], out tempR, out tempC);
                            newExpecCircleRow.Add(tempR);
                            newExpectCircleCol.Add(tempC);
                            newExpectCircleRadius.Add(L_regions[0].getModelData()[2].D);
                        }
                    }
                    else
                    {
                        newExpecCircleRow.Add(L_regions[0].getModelData()[0]);
                        newExpectCircleCol.Add(L_regions[0].getModelData()[1]);
                        newExpectCircleRadius.Add(L_regions[0].getModelData()[2]);
                    }

                    //ResultCircleCol.Clear();
                    //ResultCircleRow.Clear();
                    //ResultCircleRadius.Clear();
                    // 每次运行先清空上次结果，避免本次失败时下游仍读到旧圆。
                    toolPar.ResultPar.圆心.Clear();
                    toolPar.ResultPar.是否找到圆 = false;
                    toolPar.ResultPar.结果圆 = null;
                    toolPar.ResultPar.圆半径 = 0;
                    circleCenter.Clear();
                    List<Circle> foundCircles = new List<Circle>();

                    Circle cc = new Circle();
                    for (int i = 0; i < newExpectCircleCol.Count; i++)
                    {
                        HTuple handleID;
                        //  HOperatorSet.ClearAllMetrologyModels();
                        HOperatorSet.CreateMetrologyModel(out handleID);

                        HTuple width, height;
                        HOperatorSet.GetImageSize(toolPar.InputPar.图像, out width, out height);
                        HOperatorSet.SetMetrologyModelImageSize(handleID, width[0], height[0]);
                        HTuple index;
                        // 为当前预期圆创建测量模型。Metrology 会在圆周上布置多个矩形卡尺，
                        // 每个卡尺沿径向搜索边缘点，最后用这些边缘点拟合圆。
                        HOperatorSet.AddMetrologyObjectCircleMeasure(handleID, newExpecCircleRow[i], newExpectCircleCol[i], newExpectCircleRadius[i], new HTuple(ringRadiusLength), new HTuple(5), new HTuple(1), new HTuple(30), new HTuple(), new HTuple(), out index);
                        LastExpectRow = newExpecCircleRow[i];
                        LastExpectCol = newExpectCircleCol[i];
                        // 核心调参入口。界面上的阈值、卡尺数量、卡尺宽度、极性最终都写到这里。
                        HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_transition"), new HTuple(polarity));
                        HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("num_measures"), new HTuple(cliperNum));
                        HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length1"), new HTuple(ringRadiusLength));
                        HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length2"), new HTuple(caliperWidth));
                        HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_threshold"), new HTuple(threshold));
                        HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_select"), new HTuple(edgeSelect));
                        HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("min_score"), new HTuple(minScore));


                        if (final_region != null)
                        {
                            // final_region 是用户涂抹出来的屏蔽区域。
                            // 这里先生成整幅图区域 rec1，再 rec1 - final_region，只在剩余区域里找边。
                            HObject image1;
                            HTuple w, h;
                            HOperatorSet.GetImageSize(toolPar.InputPar.图像, out w, out h);
                            HObject rec1;
                            HOperatorSet.GenRectangle1(out rec1, 0, 0, h, w);
                            HOperatorSet.Difference(rec1, final_region, out rec1);
                            HOperatorSet.ReduceDomain(toolPar.InputPar.图像, rec1, out image1);
                            HOperatorSet.CropDomain(image1, out image1);


                            HOperatorSet.ApplyMetrologyModel(image1, handleID);
                        }
                        else
                        {
                            HOperatorSet.ApplyMetrologyModel(toolPar.InputPar.图像, handleID);
                        }



                        //显示所有卡尺
                        if (displayCaliper)
                        {
                            HTuple row, col;
                            HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row, out col);

                            //显示指示找线方向的箭头
                            if (runTool)
                            {
                                //////HOperatorSet.DispArrow(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, newExpecCircleRow[i], newExpectCircleCol[i], newExpecCircleRow[i], newExpectCircleCol[i] + newExpectCircleRadius[i] * 1.5, 1);
                                HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("blue"));
                                HOperatorSet.DispObj(contours, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                            }
                            else
                            {
                                HOperatorSet.SetColor(GetImageWindowControl().WindowHandle, new HTuple("blue"));
                                //////HOperatorSet.DispArrow(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, newExpecCircleRow[i], newExpectCircleCol[i], newExpecCircleRow[i], newExpectCircleCol[i] + newExpectCircleRadius[i] * 1.5, 1);
                                HOperatorSet.DispObj(contours, GetImageWindowControl().WindowHandle);
                                SafeInvokeFindCircleWindow(() =>
                                {
                                    HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("blue"));
                                    HOperatorSet.DispObj(contours, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                });
                            }
                        }





                        HTuple parameter;
                        HObject circle;
                        HOperatorSet.GetMetrologyObjectResult(handleID, new HTuple("all"), new HTuple("all"), new HTuple("result_type"), new HTuple("all_param"), out parameter);
                        HOperatorSet.GetMetrologyObjectResultContour(out circle, handleID, new HTuple("all"), new HTuple("all"), new HTuple(1.5));


                        if (ignoreNum == 0)
                        {

                            if (parameter.Length >= 3)
                            {
                                // Halcon 返回 all_param 时，圆结果依次为 row、column、radius。
                                // 项目输出中 XY(parameter[0], parameter[1]) 即 XY(row, column)。
                                //ResultCircleRow.Add(parameter[0]);
                                //ResultCircleCol.Add(parameter[1]);
                                //ResultCircleRadius.Add(parameter[2]);
                                toolPar.ResultPar.圆心.Add(new XY(parameter[0], parameter[1]));
                                foundCircles.Add(new Circle
                                {
                                    X = parameter[0],
                                    Y = parameter[1],
                                    R = parameter[2]
                                });

                                XY p = new XY();
                                p.X = parameter[0];
                                p.Y = parameter[1];
                                circleCenter.Add(p);

                                if (displayCircleCenter)
                                {


                                    HObject cross;
                                    HTuple row111, col111, row11, col11;
                                    HOperatorSet.GetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row11, out col11);
                                    HOperatorSet.GenCrossContourXld(out cross, p.X, p.Y, new HTuple((row11 - row111) / 90.0 + 1), new HTuple(0));        //小细节：我们要想使无论图像像素多大，显示的十字大小都是一样的，就需要得出y=kx+b中的k和b


                                    if (runTool)
                                    {
                                        Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(cross, "blue");
                                    }
                                    else
                                    {
                                        GetImageWindowControl().hwc_imageWindow.DispObj(cross, "blue");
                                        SafeInvokeFindCircleWindow(() => Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(cross, "blue"));
                                    }
                                }




                            }


                            //把点显示出来
                            if (displayFeature)
                            {

                                HTuple row, col;
                                HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row, out col);
                                HObject cross;
                                HTuple row111, col111, row1, col1;
                                HOperatorSet.GetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row1, out col1);
                                HOperatorSet.GenCrossContourXld(out cross, row, col, new HTuple((row1 - row111) / 90.0 + 1), new HTuple(0));        //小细节：我们要想使无论图像像素多大，显示的十字大小都是一样的，就需要得出y=kx+b中的k和b





                                if (runTool)
                                {
                                    HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("orange"));
                                    HOperatorSet.DispObj(cross, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                }
                                else
                                {
                                    HOperatorSet.SetColor(GetImageWindowControl().WindowHandle, new HTuple("orange"));
                                    HOperatorSet.DispObj(cross, GetImageWindowControl().WindowHandle);
                                    SafeInvokeFindCircleWindow(() =>
                                    {
                                        HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("orange"));
                                        HOperatorSet.DispObj(cross, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                    });
                                }
                            }

                            //显示找到的圆
                            if (displayCircle)
                            {
                                if (runTool)
                                {
                                    HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                                    HOperatorSet.DispObj(circle, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                }
                                else
                                {
                                    HOperatorSet.SetColor(GetImageWindowControl().WindowHandle, new HTuple("green"));
                                    ShowObj(circle, "green");
                                    SafeInvokeFindCircleWindow(() =>
                                    {
                                        HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                                        HOperatorSet.DispObj(circle, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                    });
                                }
                            }

                            HOperatorSet.ClearMetrologyModel(handleID);
                        }
                        else
                        {
                            if (parameter.Length >= 3)
                            {
                                //ResultCircleRow.Add(parameter[0]);
                                //ResultCircleCol.Add(parameter[1]);
                                //ResultCircleRadius.Add(parameter[2]);

                                XY p = new XY();
                                p.X = parameter[0];
                                p.Y = parameter[1];
                                circleCenter.Add(p);

                                HObject cross;
                                HTuple row111, col111, row11, col11;
                                HOperatorSet.GetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row11, out col11);
                                HOperatorSet.GenCrossContourXld(out cross, p.X, p.Y, new HTuple((row11 - row111) / 90.0 + 1), new HTuple(0));        //小细节：我们要想使无论图像像素多大，显示的十字大小都是一样的，就需要得出y=kx+b中的k和b


                                if (runTool)
                                {
                                    Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(cross, "blue");
                                }
                                else
                                {
                                    GetImageWindowControl().hwc_imageWindow.DispObj(cross, "blue");
                                    SafeInvokeFindCircleWindow(() => Frm_FindCircleTool.Instance.hWindow_Final1.DispObj(cross, "blue"));
                                }

                            }






                            //显示找到的圆
                            if (displayCircle)
                            {
                                if (runTool)
                                {
                                    //////HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("green"));
                                    //////HOperatorSet.DispObj(circle, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                }
                                else
                                {
                                    //////HOperatorSet.SetColor(GetImageWindowControl().WindowHandle, new HTuple("green"));
                                    //////ShowObj(circle, "green");
                                }
                            }
                            //////}




                            HTuple row, col;
                            HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row, out col);


                            List<ttt> distance = new List<ttt>();
                            for (int j = 0; j < row.TupleLength(); j++)
                            {
                                // ignoreNum 模式：先算每个边缘点到 Halcon 初拟合圆的距离。
                                // 距离越大越像离群点，后面排序后剔除最大的 ignoreNum 个点。
                                HTuple distance1, temp;
                                HOperatorSet.DistancePc(circle, row[j], col[j], out distance1, out temp);
                                ttt tt = new ttt();
                                tt.row = row[j];
                                tt.col = col[j];
                                tt.distance = distance1.D;
                                distance.Add(tt);
                            }

                            //排序
                            // 从小到大排序，保留距离圆轮廓最近的点重新拟合。
                            ttt temp1;
                            for (int j = 0; j < distance.Count - 1; j++)
                            {
                                for (int k = j + 1; k < distance.Count; k++)
                                {
                                    if (distance[j].distance > distance[k].distance)
                                    {
                                        temp1 = distance[j];
                                        distance[j] = distance[k];
                                        distance[k] = temp1;
                                    }
                                }
                            }
                            //distance.RemoveRange(distance.Count - ignoreNum, ignoreNum);
                            double[] rowss = new double[distance.Count - ignoreNum];
                            double[] colss = new double[distance.Count - ignoreNum];
                            List<PointF> list = new List<PointF>();
                            for (int k = 0; k < distance.Count - ignoreNum; k++)
                            {
                                PointF p = new PointF();
                                p.X = (float)distance[k].row;
                                p.Y = (float)distance[k].col;
                                rowss[k] = (float)distance[k].row;
                                colss[k] = (float)distance[k].col;
                                list.Add(p);
                            }
                            // 使用剔除离群点后的点集重新最小二乘拟合。
                            // 如果你要提升抗干扰能力，可以重点改这一段：例如改成 RANSAC、按边缘强度加权等。
                            cc = LeastSquaresFit(rowss, colss);
                            if (displayCircle)
                            {
                                if (runTool)
                                {
                                    HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "green");
                                    HOperatorSet.DispCircle(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, cc.X, cc.Y, cc.R);
                                }
                                else
                                {
                                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, "green");
                                    HOperatorSet.DispCircle(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, cc.X, cc.Y, cc.R);
                                    SafeInvokeFindCircleWindow(() =>
                                    {
                                        HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, "green");
                                        HOperatorSet.DispCircle(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, cc.X, cc.Y, cc.R);
                                    });
                                }
                            }
                            //ResultCircleRow.Add(cc.X);
                            //ResultCircleCol.Add(cc.Y);
                            //ResultCircleRadius.Add(cc.R);
                            toolPar.ResultPar.圆心.Add(new XY(cc.X, cc.Y));
                            foundCircles.Add(new Circle
                            {
                                X = cc.X,
                                Y = cc.Y,
                                R = cc.R
                            });

                            //显示合格点
                            //把点显示出来
                            if (displayFeature)
                            {
                                HTuple rowssssss = new HTuple();
                                HTuple colsssss = new HTuple();
                                for (int j = 0; j < distance.Count - ignoreNum; j++)
                                {
                                    rowssssss[j] = (distance[j].row);
                                    colsssss[j] = (distance[j].col);
                                }

                                HObject cross;
                                HTuple row111, col111, row1, col1;
                                HOperatorSet.GetPart(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, out row111, out col111, out row1, out col1);
                                HOperatorSet.GenCrossContourXld(out cross, rowssssss, colsssss, new HTuple((row1 - row111) / 90.0 + 1), new HTuple(0));        //小细节：我们要想使无论图像像素多大，显示的十字大小都是一样的，就需要得出y=kx+b中的k和b



                                if (runTool)
                                {
                                    HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("#ffb529"));
                                    HOperatorSet.DispObj(cross, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                }
                                else
                                {
                                    HOperatorSet.SetColor(GetImageWindowControl().WindowHandle, new HTuple("#ffb529"));
                                    HOperatorSet.DispObj(cross, GetImageWindowControl().WindowHandle);
                                    SafeInvokeFindCircleWindow(() =>
                                    {
                                        HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("#ffb529"));
                                        HOperatorSet.DispObj(cross, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                    });
                                }


                                //显示被忽略的点
                                HTuple rowsssssslll = new HTuple();
                                HTuple colssssslll = new HTuple();
                                for (int j = distance.Count - ignoreNum; j < distance.Count; j++)
                                {
                                    rowsssssslll[j] = (distance[j].row);
                                    colssssslll[j] = (distance[j].col);
                                }



                                HOperatorSet.GenCrossContourXld(out cross, rowsssssslll, colssssslll, new HTuple((row1 - row111) / 90.0 + 1), new HTuple(0));        //小细节：我们要想使无论图像像素多大，显示的十字大小都是一样的，就需要得出y=kx+b中的k和b







                                if (runTool)
                                {
                                    HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("red"));
                                    HOperatorSet.DispObj(cross, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                }
                                else
                                {
                                    HOperatorSet.SetColor(GetImageWindowControl().WindowHandle, new HTuple("orange"));
                                    HOperatorSet.DispObj(cross, GetImageWindowControl().WindowHandle);
                                    SafeInvokeFindCircleWindow(() =>
                                    {
                                        HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("red"));
                                        HOperatorSet.DispObj(cross, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);
                                    });
                                }
                            }

                            HOperatorSet.ClearMetrologyModel(handleID);

                        }


                    }

                    if (toolPar.ResultPar.圆心.Count > 0 && foundCircles.Count > 0)
                    {
                        // 对外输出只取 foundCircles[0] 作为“结果圆”，同时保留圆心列表。
                        // 如果需要支持多目标圆输出，需要扩展 ResultPar，而不是只改 foundCircles。
                        toolPar.ResultPar.是否找到圆 = true;
                        toolPar.ResultPar.结果圆 = foundCircles[0];
                        toolPar.ResultPar.圆半径 = foundCircles[0].R;
                        Frm_FindCircleTool.Instance.tbx_resultCircleRow.TextStr = toolPar.ResultPar.圆心[0].X.ToString("0.000");
                        Frm_FindCircleTool.Instance.tbx_resultCircleCol.TextStr = toolPar.ResultPar.圆心[0].Y.ToString("0.000");
                        Frm_FindCircleTool.Instance.tbx_resultCircleRadius.TextStr = foundCircles[0].R.ToString("0.000");
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                    }
                    else
                    {
                        Frm_FindCircleTool.Instance.tbx_resultCircleRow.TextStr = "0";
                        Frm_FindCircleTool.Instance.tbx_resultCircleCol.TextStr = "0";
                        Frm_FindCircleTool.Instance.tbx_resultCircleRadius.TextStr = "0";
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Circle_Found : ToolRunStatu.未找到圆);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        public override ToolRunResult Execute(ToolRunContext context)
        {
            // Job.Run() 当前通过 Execute() 包装工具运行，统一返回成功/失败/耗时。
            // 注意 TimeoutMs 目前没有在本工具内部强制中断；如果要加超时，需要把 Run() 拆成可取消逻辑。
            if (context != null && context.IsCancellationRequested != null && context.IsCancellationRequested())
            {
                toolRunStatu = ToolRunStatu.用户取消;
                return new ToolRunResult
                {
                    Success = false,
                    Canceled = true,
                    Timeout = false,
                    Status = toolRunStatu,
                    Message = toolRunStatu.ToString(),
                    ElapsedMs = 0
                };
            }

            Stopwatch sw = Stopwatch.StartNew();
            Run(true, false, context == null ? string.Empty : context.ToolName);
            sw.Stop();

            bool success = toolRunStatu == (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
            return new ToolRunResult
            {
                Success = success,
                Canceled = toolRunStatu == ToolRunStatu.用户取消,
                Timeout = toolRunStatu == ToolRunStatu.运行超时,
                Status = toolRunStatu,
                Message = toolRunStatu.ToString(),
                ElapsedMs = sw.ElapsedMilliseconds
            };
        }

        /// <summary>
        /// 在 UI 线程安全地更新找圆窗体；窗体未打开时直接跳过，不创建隐藏窗体。
        /// </summary>
        private void SafeInvokeFindCircleWindow(Action action)
        {
            try
            {
                if (!Frm_FindCircleTool.IsOpen)
                    return;
                Frm_FindCircleTool frm = Frm_FindCircleTool.Instance;
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
            // 工具参数分三层：
            // InputPar 由 Job.Run() 从上游工具输出填充；
            // RunPar 预留运行参数，目前圆查找的运行参数直接放在 FindCircleTool 字段上；
            // ResultPar 是本工具对下游暴露的输出。
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
            // 输入图像：通常来自“采集图像”或图像处理工具输出。
            private HObject _图像;

            public HObject 图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }

            // 跟随位姿：通常来自模板匹配等定位工具。
            // 为空时，按 L_regions 中的预期圆原位置找圆；
            // 非空时，用 templatePose -> 当前 XYU 的变换移动预期圆。
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
            // 圆心列表：每个输入跟随位姿最多对应一个圆心。
            // 历史命名为 XY，但这里 X=row，Y=column。
            private List<XY> _圆心 = new List<XY>();

            public List<XY> 圆心
            {
                get { return _圆心; }
                set { _圆心 = value; }
            }

            // 是否找到圆：流程判断成功与否时可连接这个布尔输出。
            private bool _是否找到圆 = false;
            public bool 是否找到圆
            {
                get { return _是否找到圆; }
                set { _是否找到圆 = value; }
            }

            // 结果圆：当前只保存第一个找到的圆，包含 row/column/radius。
            private Circle _结果圆;
            public Circle 结果圆
            {
                get { return _结果圆; }
                set { _结果圆 = value; }
            }

            // 圆半径：为了方便下游直接连接数值，单独复制一份第一个结果圆的半径。
            private double _圆半径 = 0;
            public double 圆半径
            {
                get { return _圆半径; }
                set { _圆半径 = value; }
            }


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
        /// <summary>
        /// 圆心横坐标
        /// </summary>
        /// <value></value>
        public double X { get; set; }
        /// <summary>
        /// 圆心纵坐标
        /// </summary>
        /// <value></value>
        public double Y { get; set; }
        /// <summary>
        /// 圆半径
        /// </summary>
        /// <value></value>
        public double R { get; set; }
    }
}
