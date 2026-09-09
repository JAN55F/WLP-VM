using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using System.Diagnostics;
using System.Threading;//调试，精确计时
//公共类,不能实例子化
namespace HalconTool
{
   public static class Common
    {
       public static TestData WorkRun=new TestData();
       public static HObject ho_Image = null;//相机采集到的图像
       public static HTuple hv_AcqHandle = null;//相机句柄
       public static bool isOpen = false;//相机是否开启
       public static Mutex CaptureMutex = new Mutex();
       public static bool OpenCamera()//开启相机
       {
           try
           {
               if (isOpen == true)
               {
                   return true;
               }
               //1E1000F8D0AA_PointGreyResearch_Chameleon3CM3U313Y3M
               HOperatorSet.OpenFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive",
    -1, "default", -1, "false", "default", "c1",
    0, -1, out hv_AcqHandle);
              // HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "PixelFormat", "Mono8");//设置采集颜色
              HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "PixelFormat", "BayerRG8");//设置采集颜色
               HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");
               HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "GainAuto", "Off");
             //  HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "GainRaw", 1);//Gain设置
            
               HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSelector", "FrameStart");
               HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "ExposureAuto", "Off");
             //  HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "ExposureTimeRaw", 5000);//设置曝光时间
               HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "AcquisitionMode", "Continuous");

            //  HOperatorSet.GrabImageStart(hv_AcqHandle, -1);//异步采集开始
               isOpen = true;//设置标志
               return true;
           }
           catch (Exception)
           {
               try
               {
                   if (hv_AcqHandle != null)
                   {
                       HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                       hv_AcqHandle = null;
                       isOpen = false;
                   }
               }
               catch (Exception)
               {
                   
                  
               }
              
              
               return false;
           }
       
       }
       
       public static HObject Capture()
       {
           try
           {
              CaptureMutex.WaitOne();
               HOperatorSet.GenEmptyObj(out ho_Image);
               ho_Image.Dispose();
               HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle);//同步采集
             //  HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);//异步采集
               CaptureMutex.ReleaseMutex();
               return ho_Image.Clone();
           }
           catch (Exception ex)
           {
               ex.Message.ToString();
               MessageBox.Show("采集出错");
               CaptureMutex.ReleaseMutex();
               return null;
           }
       }
       public static bool CloseCamera()//关闭相机
       {
           try
           {
               if (isOpen == false)
               {
                   return true;
               }
               HOperatorSet.CloseFramegrabber(hv_AcqHandle);
               return true;
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message);
               return false;
           }
       }

       //精确延时
       public static void Delay(double t)
       {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Restart();
            while (stopWatch.Elapsed.TotalMilliseconds < t)
            {
                Application.DoEvents();
            }
       }
       #region "找线找圆工具"
       public static void draw_rake(out HObject ho_Regions, HTuple hv_WindowHandle, HTuple hv_Elements,
        HTuple hv_DetectHeight, HTuple hv_DetectWidth, out HTuple hv_Row1, out HTuple hv_Column1,
        out HTuple hv_Row2, out HTuple hv_Column2)
       {



           // Stack for temporary objects 
           HObject[] OTemp = new HObject[20];

           // Local iconic variables 

           HObject ho_RegionLines, ho_Rectangle = null;
           HObject ho_Arrow1 = null;

           // Local control variables 

           HTuple hv_ATan = null, hv_i = null, hv_RowC = new HTuple();
           HTuple hv_ColC = new HTuple(), hv_Distance = new HTuple();
           HTuple hv_RowL2 = new HTuple(), hv_RowL1 = new HTuple();
           HTuple hv_ColL2 = new HTuple(), hv_ColL1 = new HTuple();
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_Regions);
           HOperatorSet.GenEmptyObj(out ho_RegionLines);
           HOperatorSet.GenEmptyObj(out ho_Rectangle);
           HOperatorSet.GenEmptyObj(out ho_Arrow1);
           //提示
           Common.disp_message(hv_WindowHandle, new HTuple("点击鼠标左键画一条直线,点击右键确认"),
               "window", 12, 12, "red", "false");
           //产生一个空显示对象，用于显示
           ho_Regions.Dispose();
           HOperatorSet.GenEmptyObj(out ho_Regions);
           HOperatorSet.SetColor(hv_WindowHandle, "red");
           //画矢量检测直线
           HOperatorSet.DrawLine(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
               out hv_Column2);
           //产生直线xld
           ho_RegionLines.Dispose();
           HOperatorSet.GenContourPolygonXld(out ho_RegionLines, hv_Row1.TupleConcat(hv_Row2),
               hv_Column1.TupleConcat(hv_Column2));
           //存储到显示对象
           {
               HObject ExpTmpOutVar_0;
               HOperatorSet.ConcatObj(ho_Regions, ho_RegionLines, out ExpTmpOutVar_0);
               ho_Regions.Dispose();
               ho_Regions = ExpTmpOutVar_0;
           }
           //计算直线与x轴的夹角，逆时针方向为正向。
           HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_ATan);

           //边缘检测方向垂直于检测直线：直线方向正向旋转90°为边缘检测方向
           hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());

           //根据检测直线按顺序产生测量区域矩形，并存储到显示对象
           HTuple end_val17 = hv_Elements;
           HTuple step_val17 = 1;
           for (hv_i = 1; hv_i.Continue(end_val17, step_val17); hv_i = hv_i.TupleAdd(step_val17))
           {
               //如果只有一个测量矩形，作为卡尺工具，宽度为检测直线的长度
               if ((int)(new HTuple(hv_Elements.TupleEqual(1))) != 0)
               {
                   hv_RowC = (hv_Row1 + hv_Row2) * 0.5;
                   hv_ColC = (hv_Column1 + hv_Column2) * 0.5;
                   HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
                   ho_Rectangle.Dispose();
                   HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                       hv_ATan, hv_DetectHeight / 2, hv_Distance / 2);
               }
               else
               {
                   //如果有多个测量矩形，产生该测量矩形xld
                   hv_RowC = hv_Row1 + (((hv_Row2 - hv_Row1) * (hv_i - 1)) / (hv_Elements - 1));
                   hv_ColC = hv_Column1 + (((hv_Column2 - hv_Column1) * (hv_i - 1)) / (hv_Elements - 1));
                   ho_Rectangle.Dispose();
                   HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                       hv_ATan, hv_DetectHeight / 2, hv_DetectWidth / 2);
               }
               //把测量矩形xld存储到显示对象
               {
                   HObject ExpTmpOutVar_0;
                   HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle, out ExpTmpOutVar_0);
                   ho_Regions.Dispose();
                   ho_Regions = ExpTmpOutVar_0;
               }
               if ((int)(new HTuple(hv_i.TupleEqual(1))) != 0)
               {
                   //在第一个测量矩形绘制一个箭头xld，用于只是边缘检测方向
                   hv_RowL2 = hv_RowC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                   hv_RowL1 = hv_RowC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                   hv_ColL2 = hv_ColC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                   hv_ColL1 = hv_ColC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                   ho_Arrow1.Dispose();
                   Common.gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                       25, 25);
                   //把xld存储到显示对象
                   {
                       HObject ExpTmpOutVar_0;
                       HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                       ho_Regions.Dispose();
                       ho_Regions = ExpTmpOutVar_0;
                   }
               }
           }

           ho_RegionLines.Dispose();
           ho_Rectangle.Dispose();
           ho_Arrow1.Dispose();

           return;
       }
       public static void draw_spoke( HObject ho_Image, out HObject ho_Regions, HTuple hv_WindowHandle,
    HTuple hv_Elements, HTuple hv_DetectHeight, HTuple hv_DetectWidth, out HTuple hv_ROIRows,
    out HTuple hv_ROICols, out HTuple hv_Direct)
       {




           // Stack for temporary objects 
           HObject[] OTemp = new HObject[20];

           // Local iconic variables 

           HObject ho_ContOut1, ho_Contour, ho_ContCircle;
           HObject ho_Cross, ho_Rectangle1 = null, ho_Arrow1 = null;

           // Local control variables 

           HTuple hv_Rows = null, hv_Cols = null, hv_Weights = null;
           HTuple hv_Length1 = null, hv_RowC = null, hv_ColumnC = null;
           HTuple hv_Radius = null, hv_StartPhi = null, hv_EndPhi = null;
           HTuple hv_PointOrder = null, hv_RowXLD = null, hv_ColXLD = null;
           HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
           HTuple hv_Column2 = null, hv_DistanceStart = null, hv_DistanceEnd = null;
           HTuple hv_Length2 = null, hv_i = null, hv_j = new HTuple();
           HTuple hv_RowE = new HTuple(), hv_ColE = new HTuple();
           HTuple hv_ATan = new HTuple(), hv_RowL2 = new HTuple();
           HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
           HTuple hv_ColL1 = new HTuple();
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_Regions);
           HOperatorSet.GenEmptyObj(out ho_ContOut1);
           HOperatorSet.GenEmptyObj(out ho_Contour);
           HOperatorSet.GenEmptyObj(out ho_ContCircle);
           HOperatorSet.GenEmptyObj(out ho_Cross);
           HOperatorSet.GenEmptyObj(out ho_Rectangle1);
           HOperatorSet.GenEmptyObj(out ho_Arrow1);
           hv_ROIRows = new HTuple();
           hv_ROICols = new HTuple();
           hv_Direct = new HTuple();
           //提示
           Common.disp_message(hv_WindowHandle, new HTuple("1、画4个以上点确定一个圆弧,点击右键确认"),
               "window", 12, 12, "red", "false");
           //产生一个空显示对象，用于显示
           ho_Regions.Dispose();
           HOperatorSet.GenEmptyObj(out ho_Regions);
           HOperatorSet.SetColor(hv_WindowHandle, "red");
           //沿着圆弧或圆的边缘画点
           ho_ContOut1.Dispose();
           HOperatorSet.DrawNurbs(out ho_ContOut1, hv_WindowHandle, "true", "true",
               "true", "true", 3, out hv_Rows, out hv_Cols, out hv_Weights);
           //至少要4个点
           HOperatorSet.TupleLength(hv_Weights, out hv_Length1);
           if ((int)(new HTuple(hv_Length1.TupleLess(4))) != 0)
           {
               Common.disp_message(hv_WindowHandle, "提示：点数太少，请重画", "window",
                   32, 12, "red", "false");
               hv_ROIRows = new HTuple();
               hv_ROICols = new HTuple();
               ho_ContOut1.Dispose();
               ho_Contour.Dispose();
               ho_ContCircle.Dispose();
               ho_Cross.Dispose();
               ho_Rectangle1.Dispose();
               ho_Arrow1.Dispose();

               return;
           }
           //获取点
           hv_ROIRows = hv_Rows.Clone();
           hv_ROICols = hv_Cols.Clone();
           //产生xld
           ho_Contour.Dispose();
           HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);
           //用回归线法（不抛出异常点，所有点权重一样）拟合圆
           //HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 3, 2, out hv_RowC,
           //    out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
           //根据拟合结果产生xld，并保持到显示对象
         //  ho_ContCircle.Dispose();
           ////HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
           ////    hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
           ////{
           ////    HObject ExpTmpOutVar_0;
           ////    HOperatorSet.ConcatObj(ho_Regions, ho_ContCircle, out ExpTmpOutVar_0);
           ////    ho_Regions.Dispose();
           ////    ho_Regions = ExpTmpOutVar_0;
           ////}

           //获取圆或圆弧xld上的点坐标
           HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);
           //显示图像和圆弧
           HOperatorSet.DispObj(ho_Image, hv_WindowHandle);
           HOperatorSet.DispObj(ho_Contour, hv_WindowHandle);
           //产生并显示圆心
           ho_Cross.Dispose();
           HOperatorSet.GenCrossContourXld(out ho_Cross, hv_RowC, hv_ColumnC, 60, 0.785398);
           HOperatorSet.DispObj(ho_Cross, hv_WindowHandle);
           //提示
           Common.disp_message(hv_WindowHandle, "2、远离圆心，画箭头确定边缘检测方向，点击右键确认",
               "window", 12, 12, "red", "false");
           //画线，确定检测方向
           //////HOperatorSet.DrawLine(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
           //////    out hv_Column2);
           ////////求圆心到检测方向直线起点的距离
           //////HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row1, hv_Column1, out hv_DistanceStart);
           ////////求圆心到检测方向直线终点的距离
           //////HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row2, hv_Column2, out hv_DistanceEnd);

           //求圆或圆弧xld上的点的数量
           //////HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
           ////////判断检测的边缘数量是否过少
           //////if ((int)(new HTuple(hv_Elements.TupleLess(3))) != 0)
           //////{
           //////    hv_ROIRows = new HTuple();
           //////    hv_ROICols = new HTuple();
           //////    Common.disp_message(hv_WindowHandle, "检测的边缘数量太少，请重新设置!",
           //////        "window", 52, 12, "red", "false");
           //////    ho_ContOut1.Dispose();
           //////    ho_Contour.Dispose();
           //////    ho_ContCircle.Dispose();
           //////    ho_Cross.Dispose();
           //////    ho_Rectangle1.Dispose();
           //////    ho_Arrow1.Dispose();

           //////    return;
           //////}
           //如果xld是圆弧，有Length2个点，从起点开始，等间距（间距为Length2/(Elements-1)）取Elements个点，作为卡尺工具的中点
           //如果xld是圆，有Length2个点，以0°为起点，从起点开始，等间距（间距为Length2/(Elements)）取Elements个点，作为卡尺工具的中点
           HTuple end_val53 = hv_Elements - 1;
           HTuple step_val53 = 1;
           for (hv_i = 0; hv_i.Continue(end_val53, step_val53); hv_i = hv_i.TupleAdd(step_val53))
           {

               if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                   hv_Length2 - 1)))) != 0)
               {
                   //xld的起点和终点坐标相对，为圆
                   HOperatorSet.TupleInt(((1.0 * hv_Length2) / hv_Elements) * hv_i, out hv_j);

               }
               else
               {
                   //否则为圆弧
                   HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
               }
               //索引越界，强制赋值为最后一个索引
               if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
               {
                   hv_j = hv_Length2 - 1;
                   //continue
               }
               //获取卡尺工具中心
               hv_RowE = hv_RowXLD.TupleSelect(hv_j);
               hv_ColE = hv_ColXLD.TupleSelect(hv_j);

               //如果圆心到检测方向直线的起点的距离大于圆心到检测方向直线的终点的距离，搜索方向由圆外指向圆心
               //如果圆心到检测方向直线的起点的距离不大于圆心到检测方向直线的终点的距离，搜索方向由圆心指向圆外
               if ((int)(new HTuple(hv_DistanceStart.TupleGreater(hv_DistanceEnd))) != 0)
               {
                   //求卡尺工具的边缘搜索方向
                   //求圆心指向边缘的矢量的角度
                   HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                   //角度反向
                   hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;
                   //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                   hv_Direct = "inner";
               }
               else
               {
                   //求卡尺工具的边缘搜索方向
                   //求圆心指向边缘的矢量的角度
                   HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                   //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                   hv_Direct = "outer";
               }

               //产生卡尺xld，并保持到显示对象
               ho_Rectangle1.Dispose();
               HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle1, hv_RowE, hv_ColE, hv_ATan,
                   hv_DetectHeight / 2, hv_DetectWidth / 2);
               {
                   HObject ExpTmpOutVar_0;
                   HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle1, out ExpTmpOutVar_0);
                   ho_Regions.Dispose();
                   ho_Regions = ExpTmpOutVar_0;
               }

               //用箭头xld指示边缘搜索方向，并保持到显示对象
               if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
               {
                   hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                   hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                   hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                   hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                   ho_Arrow1.Dispose();
                   Common.gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                       25, 25);
                   {
                       HObject ExpTmpOutVar_0;
                       HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                       ho_Regions.Dispose();
                       ho_Regions = ExpTmpOutVar_0;
                   }
               }
           }

           ho_ContOut1.Dispose();
           ho_Contour.Dispose();
           ho_ContCircle.Dispose();
           ho_Cross.Dispose();
           ho_Rectangle1.Dispose();
           ho_Arrow1.Dispose();

           return;
       }
       /// <summary>
       /// 拟合圆
       /// </summary>
       /// <param name="ho_Circle">输出拟合圆的xld</param>
       /// <param name="hv_Rows">拟合圆的边缘y数组</param>
       /// <param name="hv_Cols">拟合圆的边缘x数组</param>
       /// <param name="hv_ActiveNum">最小有效点数</param>
       /// <param name="hv_ArcType">拟合圆弧类型：'arc'圆弧；'circle'圆</param>
       /// <param name="hv_RowCenter">拟合的圆中心y</param>
       /// <param name="hv_ColCenter">拟合的圆中心x</param>
       /// <param name="hv_Radius">拟合的圆半径</param>
       /// <param name="hv_StartPhi">圆弧起点角度(单位：弧度)</param>
       /// <param name="hv_EndPhi">圆弧终点角度(单位：弧度)</param>
       /// <param name="hv_PointOrder">轮廓点方向</param>
       /// <param name="hv_ArcAngle"></param>
       public static void pts_to_best_circle(out HObject ho_Circle, HTuple hv_Rows, HTuple hv_Cols,
   HTuple hv_ActiveNum, HTuple hv_ArcType, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
   out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder,
   out HTuple hv_ArcAngle)
       {



           // Local iconic variables 

           HObject ho_Contour = null;

           // Local control variables 

           HTuple hv_Length = null, hv_Length1 = new HTuple();
           HTuple hv_CircleLength = new HTuple();
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_Circle);
           HOperatorSet.GenEmptyObj(out ho_Contour);
           hv_StartPhi = new HTuple();
           hv_EndPhi = new HTuple();
           hv_PointOrder = new HTuple();
           hv_ArcAngle = new HTuple();
           //初始化
           hv_RowCenter = 0;
           hv_ColCenter = 0;
           hv_Radius = 0;
           //产生一个空的直线对象，用于保存拟合后的圆
           ho_Circle.Dispose();
           HOperatorSet.GenEmptyObj(out ho_Circle);
           //计算边缘数量
           HOperatorSet.TupleLength(hv_Cols, out hv_Length);
           //当边缘数量不小于有效点数时进行拟合
           if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(new HTuple(hv_ActiveNum.TupleGreater(
               2)))) != 0)
           {
               //halcon的拟合是基于xld的，需要把边缘连接成xld
               if ((int)(new HTuple(hv_ArcType.TupleEqual("circle"))) != 0)
               {
                   //如果是闭合的圆，轮廓需要首尾相连
                   ho_Contour.Dispose();
                   HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows.TupleConcat(hv_Rows.TupleSelect(
                       0)), hv_Cols.TupleConcat(hv_Cols.TupleSelect(0)));
               }
               else
               {
                   ho_Contour.Dispose();
                   HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);
               }
               //拟合圆。使用的算法是''geotukey''，其他算法请参考fit_circle_contour_xld的描述部分。
               HOperatorSet.FitCircleContourXld(ho_Contour, "geotukey", -1, 0, 0, 3, 2, out hv_RowCenter,
                   out hv_ColCenter, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
               //判断拟合结果是否有效：如果拟合成功，数组中元素的数量大于0
               HOperatorSet.TupleLength(hv_StartPhi, out hv_Length1);
               if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
               {
                   ho_Contour.Dispose();

                   return;
               }
               //根据拟合结果，产生直线xld
               if ((int)(new HTuple(hv_ArcType.TupleEqual("arc"))) != 0)
               {
                   //判断圆弧的方向：顺时针还是逆时针
                   //halcon求圆弧会出现方向混乱的问题
                   //tuple_mean (Rows, RowsMean)
                   //tuple_mean (Cols, ColsMean)
                   //gen_cross_contour_xld (Cross, RowsMean, ColsMean, 6, 0.785398)
                   //gen_circle_contour_xld (Circle1, RowCenter, ColCenter, Radius, StartPhi, EndPhi, 'positive', 1)
                   //求轮廓1中心
                   //area_center_points_xld (Circle1, Area, Row1, Column1)
                   //gen_circle_contour_xld (Circle2, RowCenter, ColCenter, Radius, StartPhi, EndPhi, 'negative', 1)
                   //求轮廓2中心
                   //area_center_points_xld (Circle2, Area, Row2, Column2)
                   //distance_pp (RowsMean, ColsMean, Row1, Column1, Distance1)
                   //distance_pp (RowsMean, ColsMean, Row2, Column2, Distance2)
                   //ArcAngle := EndPhi-StartPhi
                   //if (Distance1<Distance2)

                   //PointOrder := 'positive'
                   //copy_obj (Circle1, Circle, 1, 1)
                   //else

                   //PointOrder := 'negative'
                   //if (abs(ArcAngle)>3.1415926)
                   //ArcAngle := ArcAngle-2.0*3.1415926
                   //endif
                   //copy_obj (Circle2, Circle, 1, 1)
                   //endif
                   ho_Circle.Dispose();
                   HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                       hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);

                   HOperatorSet.LengthXld(ho_Circle, out hv_CircleLength);
                   hv_ArcAngle = hv_EndPhi - hv_StartPhi;
                   if ((int)(new HTuple(hv_CircleLength.TupleGreater(((new HTuple(180)).TupleRad()
                       ) * hv_Radius))) != 0)
                   {
                       if ((int)(new HTuple(((hv_ArcAngle.TupleAbs())).TupleLess((new HTuple(180)).TupleRad()
                           ))) != 0)
                       {
                           if ((int)(new HTuple(hv_ArcAngle.TupleGreater(0))) != 0)
                           {
                               hv_ArcAngle = ((new HTuple(360)).TupleRad()) - hv_ArcAngle;
                           }
                           else
                           {

                               hv_ArcAngle = ((new HTuple(360)).TupleRad()) + hv_ArcAngle;
                           }
                       }
                   }
                   else
                   {
                       if ((int)(new HTuple(hv_CircleLength.TupleLess(((new HTuple(180)).TupleRad()
                           ) * hv_Radius))) != 0)
                       {
                           if ((int)(new HTuple(((hv_ArcAngle.TupleAbs())).TupleGreater((new HTuple(180)).TupleRad()
                               ))) != 0)
                           {
                               if ((int)(new HTuple(hv_ArcAngle.TupleGreater(0))) != 0)
                               {
                                   hv_ArcAngle = hv_ArcAngle - ((new HTuple(360)).TupleRad());

                               }
                               else
                               {
                                   hv_ArcAngle = ((new HTuple(360)).TupleRad()) + hv_ArcAngle;
                               }
                           }
                       }

                   }

               }
               else
               {
                   hv_StartPhi = 0;
                   hv_EndPhi = (new HTuple(360)).TupleRad();
                   hv_ArcAngle = (new HTuple(360)).TupleRad();
                   ho_Circle.Dispose();
                   HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                       hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
               }
           }

           ho_Contour.Dispose();

           return;
       }
       /// <summary>
       /// 找圆
       /// </summary>
       /// <param name="ho_Image">输入图像</param>
       /// <param name="ho_Regions">输出显示对象合集</param>
       /// <param name="hv_WindowHandle">输入显示窗体句柄</param>
       /// <param name="hv_Elements">卡尺数</param>
       /// <param name="hv_DetectHeight">卡尺高</param>
       /// <param name="hv_DetectWidth">卡尺宽</param>
       /// <param name="hv_Sigma">平滑系数</param>
       /// <param name="hv_Threshold">阈值大小</param>
       /// <param name="hv_Transition">极性</param>
       /// <param name="hv_Select">选择点，第一，最后还是最大</param>
       /// <param name="hv_ROIRows">检测区域起点的y值</param>
       /// <param name="hv_ROICols">检测区域起点的x值</param>
       /// <param name="hv_Direct">检测方向-'inner'表示检测方向由边缘点指向圆心;'outer'表示检测方向由圆心指向边缘点</param>
       /// <param name="hv_ResultRow">检测到的边缘点的y坐标数组</param>
       /// <param name="hv_ResultColumn">检测到的边缘点的x坐标数组</param>
       /// <param name="hv_ArcType">拟合圆弧类型：'arc'圆弧；'circle'圆</param>
       public static void spoke(HObject ho_Image, out HObject ho_Regions, HTuple hv_WindowHandle,
           HTuple hv_Elements, HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Sigma,
           HTuple hv_Threshold, HTuple hv_Transition, HTuple hv_Select, HTuple hv_ROIRows,
           HTuple hv_ROICols, HTuple hv_Direct, out HTuple hv_ResultRow, out HTuple hv_ResultColumn,
           out HTuple hv_ArcType)
       {




           // Stack for temporary objects 
           HObject[] OTemp = new HObject[20];

           // Local iconic variables 

           HObject ho_Contour, ho_ContCircle, ho_Rectangle1 = null;
           HObject ho_Arrow1 = null;

           // Local control variables 

           HTuple hv_Width = null, hv_Height = null, hv_RowC = null;
           HTuple hv_ColumnC = null, hv_Radius = null, hv_StartPhi = null;
           HTuple hv_EndPhi = null, hv_PointOrder = null, hv_RowXLD = null;
           HTuple hv_ColXLD = null, hv_Length2 = null, hv_i = null;
           HTuple hv_j = new HTuple(), hv_RowE = new HTuple(), hv_ColE = new HTuple();
           HTuple hv_ATan = new HTuple(), hv_RowL2 = new HTuple();
           HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
           HTuple hv_ColL1 = new HTuple(), hv_MsrHandle_Measure = new HTuple();
           HTuple hv_RowEdge = new HTuple(), hv_ColEdge = new HTuple();
           HTuple hv_Amplitude = new HTuple(), hv_Distance = new HTuple();
           HTuple hv_tRow = new HTuple(), hv_tCol = new HTuple();
           HTuple hv_t = new HTuple(), hv_Number = new HTuple(), hv_k = new HTuple();
           HTuple hv_Select_COPY_INP_TMP = hv_Select.Clone();
           HTuple hv_Transition_COPY_INP_TMP = hv_Transition.Clone();

           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_Regions);
           HOperatorSet.GenEmptyObj(out ho_Contour);
           HOperatorSet.GenEmptyObj(out ho_ContCircle);
           HOperatorSet.GenEmptyObj(out ho_Rectangle1);
           HOperatorSet.GenEmptyObj(out ho_Arrow1);
           hv_ArcType = new HTuple();
           //获取图像尺寸
           HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
           //产生一个空显示对象，用于显示
           ho_Regions.Dispose();
           HOperatorSet.GenEmptyObj(out ho_Regions);
           //初始化边缘坐标数组
           hv_ResultRow = new HTuple();
           hv_ResultColumn = new HTuple();

           //产生xld
           ho_Contour.Dispose();
           HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);
           //用回归线法（不抛出异常点，所有点权重一样）拟合圆
           HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 1, 2, out hv_RowC,
               out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
           //根据拟合结果产生xld，并保持到显示对象
           ho_ContCircle.Dispose();
           HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
               hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
           {
               HObject ExpTmpOutVar_0;
               HOperatorSet.ConcatObj(ho_Regions, ho_ContCircle, out ExpTmpOutVar_0);
               ho_Regions.Dispose();
               ho_Regions = ExpTmpOutVar_0;
           }

           //获取圆或圆弧xld上的点坐标
           HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);

           //求圆或圆弧xld上的点的数量
           HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
           if ((int)(new HTuple(hv_Elements.TupleLess(3))) != 0)
           {
               Common.disp_message(hv_WindowHandle, "检测的边缘数量太少，请重新设置!",
                   "window", 52, 12, "red", "false");
               ho_Contour.Dispose();
               ho_ContCircle.Dispose();
               ho_Rectangle1.Dispose();
               ho_Arrow1.Dispose();

               return;
           }
           //如果xld是圆弧，有Length2个点，从起点开始，等间距（间距为Length2/(Elements-1)）取Elements个点，作为卡尺工具的中点
           //如果xld是圆，有Length2个点，以0°为起点，从起点开始，等间距（间距为Length2/(Elements)）取Elements个点，作为卡尺工具的中点
           HTuple end_val27 = hv_Elements - 1;
           HTuple step_val27 = 1;
           for (hv_i = 0; hv_i.Continue(end_val27, step_val27); hv_i = hv_i.TupleAdd(step_val27))
           {

               if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                   hv_Length2 - 1)))) != 0)
               {
                   //xld的起点和终点坐标相对，为圆
                   HOperatorSet.TupleInt(((1.0 * hv_Length2) / hv_Elements) * hv_i, out hv_j);
                   hv_ArcType = "circle";
               }
               else
               {
                   //否则为圆弧
                   HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                   hv_ArcType = "arc";
               }
               //索引越界，强制赋值为最后一个索引
               if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
               {
                   hv_j = hv_Length2 - 1;
                   //continue
               }
               //获取卡尺工具中心
               hv_RowE = hv_RowXLD.TupleSelect(hv_j);
               hv_ColE = hv_ColXLD.TupleSelect(hv_j);

               //超出图像区域，不检测，否则容易报异常
               if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(hv_Height - 1))).TupleOr(
                   new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
                   hv_Width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
               {
                   continue;
               }
               //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
               if ((int)(new HTuple(hv_Direct.TupleEqual("inner"))) != 0)
               {
                   //求卡尺工具的边缘搜索方向
                   //求圆心指向边缘的矢量的角度
                   HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                   //角度反向
                   hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;
               }
               else
               {
                   //求卡尺工具的边缘搜索方向
                   //求圆心指向边缘的矢量的角度
                   HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
               }


               //产生卡尺xld，并保持到显示对象
               ho_Rectangle1.Dispose();
               HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle1, hv_RowE, hv_ColE, hv_ATan,
                   hv_DetectHeight / 2, hv_DetectWidth / 2);
               {
                   HObject ExpTmpOutVar_0;
                   HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle1, out ExpTmpOutVar_0);
                   ho_Regions.Dispose();
                   ho_Regions = ExpTmpOutVar_0;
               }
               //用箭头xld指示边缘搜索方向，并保持到显示对象
               if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
               {
                   hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                   hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                   hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                   hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                   ho_Arrow1.Dispose();
                   Common.gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                       25, 25);
                   {
                       HObject ExpTmpOutVar_0;
                       HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                       ho_Regions.Dispose();
                       ho_Regions = ExpTmpOutVar_0;
                   }
               }


               //产生测量对象句柄
               HOperatorSet.GenMeasureRectangle2(hv_RowE, hv_ColE, hv_ATan, hv_DetectHeight / 2,
                   hv_DetectWidth / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);

               //设置极性
               if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
               {
                   hv_Transition_COPY_INP_TMP = "negative";
               }
               else
               {
                   if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
                   {

                       hv_Transition_COPY_INP_TMP = "positive";
                   }
                   else
                   {
                       hv_Transition_COPY_INP_TMP = "all";
                   }
               }
               //设置边缘位置。最强点是从所有边缘中选择幅度绝对值最大点，需要设置为'all'
               if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("first"))) != 0)
               {
                   hv_Select_COPY_INP_TMP = "first";
               }
               else
               {
                   if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("last"))) != 0)
                   {

                       hv_Select_COPY_INP_TMP = "last";
                   }
                   else
                   {
                       hv_Select_COPY_INP_TMP = "all";
                   }
               }
               //检测边缘
               HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                   hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdge, out hv_ColEdge,
                   out hv_Amplitude, out hv_Distance);
               //清除测量对象句柄
               HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);
               //临时变量初始化
               //tRow，tCol保存找到指定边缘的坐标
               hv_tRow = 0;
               hv_tCol = 0;
               //t保存边缘的幅度绝对值
               hv_t = 0;
               HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
               //找到的边缘必须至少为1个
               if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
               {
                   continue;
               }
               //有多个边缘时，选择幅度绝对值最大的边缘
               HTuple end_val120 = hv_Number - 1;
               HTuple step_val120 = 1;
               for (hv_k = 0; hv_k.Continue(end_val120, step_val120); hv_k = hv_k.TupleAdd(step_val120))
               {
                   if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_k))).TupleAbs())).TupleGreater(
                       hv_t))) != 0)
                   {

                       hv_tRow = hv_RowEdge.TupleSelect(hv_k);
                       hv_tCol = hv_ColEdge.TupleSelect(hv_k);
                       hv_t = ((hv_Amplitude.TupleSelect(hv_k))).TupleAbs();
                   }
               }
               //把找到的边缘保存在输出数组
               if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
               {

                   hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                   hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
               }
           }


           ho_Contour.Dispose();
           ho_ContCircle.Dispose();
           ho_Rectangle1.Dispose();
           ho_Arrow1.Dispose();

           return;
       }




       public static void pts_to_best_line(out HObject ho_Line, HTuple hv_Rows, HTuple hv_Cols,
           HTuple hv_ActiveNum, out HTuple hv_Row1, out HTuple hv_Column1, out HTuple hv_Row2,
           out HTuple hv_Column2)
       {



           // Local iconic variables 

           HObject ho_Contour = null;

           // Local control variables 

           HTuple hv_Length = null, hv_Nr = new HTuple();
           HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_Length1 = new HTuple();
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_Line);
           HOperatorSet.GenEmptyObj(out ho_Contour);
           //初始化
           hv_Row1 = 0;
           hv_Column1 = 0;
           hv_Row2 = 0;
           hv_Column2 = 0;
           //产生一个空的直线对象，用于保存拟合后的直线
           ho_Line.Dispose();
           HOperatorSet.GenEmptyObj(out ho_Line);
           //计算边缘数量
           HOperatorSet.TupleLength(hv_Cols, out hv_Length);
           //当边缘数量不小于有效点数时进行拟合
           if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(new HTuple(hv_ActiveNum.TupleGreater(
               1)))) != 0)
           {
               //halcon的拟合是基于xld的，需要把边缘连接成xld
               ho_Contour.Dispose();
               HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);
               //拟合直线。使用的算法是'tukey'，其他算法请参考fit_line_contour_xld的描述部分。
               HOperatorSet.FitLineContourXld(ho_Contour, "tukey", -1, 0, 5, 2, out hv_Row1,
                   out hv_Column1, out hv_Row2, out hv_Column2, out hv_Nr, out hv_Nc, out hv_Dist);
               //判断拟合结果是否有效：如果拟合成功，数组中元素的数量大于0
               HOperatorSet.TupleLength(hv_Dist, out hv_Length1);
               if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
               {
                   ho_Contour.Dispose();

                   return;
               }
               //根据拟合结果，产生直线xld
               ho_Line.Dispose();
               HOperatorSet.GenContourPolygonXld(out ho_Line, hv_Row1.TupleConcat(hv_Row2),
                   hv_Column1.TupleConcat(hv_Column2));
           }

           ho_Contour.Dispose();

           return;
       }

       public static void rake(HObject ho_Image, out HObject ho_Regions, HTuple hv_Elements,
           HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Sigma, HTuple hv_Threshold,
           HTuple hv_Transition, HTuple hv_Select, HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2,
           HTuple hv_Column2, out HTuple hv_ResultRow, out HTuple hv_ResultColumn)
       {




           // Stack for temporary objects 
           HObject[] OTemp = new HObject[20];

           // Local iconic variables 

           HObject ho_RegionLines, ho_Rectangle = null;
           HObject ho_Arrow1 = null;

           // Local control variables 

           HTuple hv_Width = null, hv_Height = null, hv_ATan = null;
           HTuple hv_i = null, hv_RowC = new HTuple(), hv_ColC = new HTuple();
           HTuple hv_Distance = new HTuple(), hv_RowL2 = new HTuple();
           HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
           HTuple hv_ColL1 = new HTuple(), hv_MsrHandle_Measure = new HTuple();
           HTuple hv_RowEdge = new HTuple(), hv_ColEdge = new HTuple();
           HTuple hv_Amplitude = new HTuple(), hv_tRow = new HTuple();
           HTuple hv_tCol = new HTuple(), hv_t = new HTuple(), hv_Number = new HTuple();
           HTuple hv_j = new HTuple();
           HTuple hv_DetectWidth_COPY_INP_TMP = hv_DetectWidth.Clone();
           HTuple hv_Select_COPY_INP_TMP = hv_Select.Clone();
           HTuple hv_Transition_COPY_INP_TMP = hv_Transition.Clone();

           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_Regions);
           HOperatorSet.GenEmptyObj(out ho_RegionLines);
           HOperatorSet.GenEmptyObj(out ho_Rectangle);
           HOperatorSet.GenEmptyObj(out ho_Arrow1);
           //获取图像尺寸
           HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
           //产生一个空显示对象，用于显示
           ho_Regions.Dispose();
           HOperatorSet.GenEmptyObj(out ho_Regions);
           //初始化边缘坐标数组
           hv_ResultRow = new HTuple();
           hv_ResultColumn = new HTuple();
           //产生直线xld
           ho_RegionLines.Dispose();
           HOperatorSet.GenContourPolygonXld(out ho_RegionLines, hv_Row1.TupleConcat(hv_Row2),
               hv_Column1.TupleConcat(hv_Column2));
           //存储到显示对象
           {
               HObject ExpTmpOutVar_0;
               HOperatorSet.ConcatObj(ho_Regions, ho_RegionLines, out ExpTmpOutVar_0);
               ho_Regions.Dispose();
               ho_Regions = ExpTmpOutVar_0;
           }
           //计算直线与x轴的夹角，逆时针方向为正向。
           HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_ATan);

           //边缘检测方向垂直于检测直线：直线方向正向旋转90°为边缘检测方向
           hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());

           //根据检测直线按顺序产生测量区域矩形，并存储到显示对象
           HTuple end_val18 = hv_Elements;
           HTuple step_val18 = 1;
           for (hv_i = 1; hv_i.Continue(end_val18, step_val18); hv_i = hv_i.TupleAdd(step_val18))
           {
               //RowC := Row1+(((Row2-Row1)*i)/(Elements+1))
               //ColC := Column1+(Column2-Column1)*i/(Elements+1)
               //if (RowC>Height-1 or RowC<0 or ColC>Width-1 or ColC<0)
               //continue
               //endif
               //如果只有一个测量矩形，作为卡尺工具，宽度为检测直线的长度
               if ((int)(new HTuple(hv_Elements.TupleEqual(1))) != 0)
               {
                   hv_RowC = (hv_Row1 + hv_Row2) * 0.5;
                   hv_ColC = (hv_Column1 + hv_Column2) * 0.5;
                   //判断是否超出图像,超出不检测边缘
                   if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                       new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                       hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
                   {
                       continue;
                   }
                   HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
                   hv_DetectWidth_COPY_INP_TMP = hv_Distance.Clone();
                   ho_Rectangle.Dispose();
                   HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                       hv_ATan, hv_DetectHeight / 2, hv_Distance / 2);
               }
               else
               {
                   //如果有多个测量矩形，产生该测量矩形xld
                   hv_RowC = hv_Row1 + (((hv_Row2 - hv_Row1) * (hv_i - 1)) / (hv_Elements - 1));
                   hv_ColC = hv_Column1 + (((hv_Column2 - hv_Column1) * (hv_i - 1)) / (hv_Elements - 1));
                   //判断是否超出图像,超出不检测边缘
                   if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                       new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                       hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
                   {
                       continue;
                   }
                   ho_Rectangle.Dispose();
                   HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                       hv_ATan, hv_DetectHeight / 2, hv_DetectWidth_COPY_INP_TMP / 2);
               }

               //把测量矩形xld存储到显示对象
               {
                   HObject ExpTmpOutVar_0;
                   HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle, out ExpTmpOutVar_0);
                   ho_Regions.Dispose();
                   ho_Regions = ExpTmpOutVar_0;
               }
               if ((int)(new HTuple(hv_i.TupleEqual(1))) != 0)
               {
                   //在第一个测量矩形绘制一个箭头xld，用于只是边缘检测方向
                   hv_RowL2 = hv_RowC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                   hv_RowL1 = hv_RowC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                   hv_ColL2 = hv_ColC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                   hv_ColL1 = hv_ColC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                   ho_Arrow1.Dispose();
                   Common.gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                       25, 25);
                   //把xld存储到显示对象
                   {
                       HObject ExpTmpOutVar_0;
                       HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                       ho_Regions.Dispose();
                       ho_Regions = ExpTmpOutVar_0;
                   }
               }
               //产生测量对象句柄
               HOperatorSet.GenMeasureRectangle2(hv_RowC, hv_ColC, hv_ATan, hv_DetectHeight / 2,
                   hv_DetectWidth_COPY_INP_TMP / 2, hv_Width, hv_Height, "nearest_neighbor",
                   out hv_MsrHandle_Measure);

               //设置极性
               if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
               {
                   hv_Transition_COPY_INP_TMP = "negative";
               }
               else
               {
                   if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
                   {

                       hv_Transition_COPY_INP_TMP = "positive";
                   }
                   else
                   {
                       hv_Transition_COPY_INP_TMP = "all";
                   }
               }
               //设置边缘位置。最强点是从所有边缘中选择幅度绝对值最大点，需要设置为'all'
               if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("first"))) != 0)
               {
                   hv_Select_COPY_INP_TMP = "first";
               }
               else
               {
                   if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("last"))) != 0)
                   {

                       hv_Select_COPY_INP_TMP = "last";
                   }
                   else
                   {
                       hv_Select_COPY_INP_TMP = "all";
                   }
               }
               //检测边缘
               HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                   hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdge, out hv_ColEdge,
                   out hv_Amplitude, out hv_Distance);
               //清除测量对象句柄
               HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);

               //临时变量初始化
               //tRow，tCol保存找到指定边缘的坐标
               hv_tRow = 0;
               hv_tCol = 0;
               //t保存边缘的幅度绝对值
               hv_t = 0;
               //找到的边缘必须至少为1个
               HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
               if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
               {
                   continue;
               }
               //有多个边缘时，选择幅度绝对值最大的边缘
               HTuple end_val100 = hv_Number - 1;
               HTuple step_val100 = 1;
               for (hv_j = 0; hv_j.Continue(end_val100, step_val100); hv_j = hv_j.TupleAdd(step_val100))
               {
                   if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_j))).TupleAbs())).TupleGreater(
                       hv_t))) != 0)
                   {

                       hv_tRow = hv_RowEdge.TupleSelect(hv_j);
                       hv_tCol = hv_ColEdge.TupleSelect(hv_j);
                       hv_t = ((hv_Amplitude.TupleSelect(hv_j))).TupleAbs();
                   }
               }
               //把找到的边缘保存在输出数组
               if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
               {
                   hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                   hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
               }
           }

           ho_RegionLines.Dispose();
           ho_Rectangle.Dispose();
           ho_Arrow1.Dispose();

           return;
       }
       #endregion


       #region  "halcon显示消息，设置字体,生成十字叉"
       public static void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
    HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
       {



           // Local iconic variables 

           // Local control variables 

           HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
           HTuple hv_Row1Part = null, hv_Column1Part = null, hv_Row2Part = null;
           HTuple hv_Column2Part = null, hv_RowWin = null, hv_ColumnWin = null;
           HTuple hv_WidthWin = null, hv_HeightWin = null, hv_MaxAscent = null;
           HTuple hv_MaxDescent = null, hv_MaxWidth = null, hv_MaxHeight = null;
           HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
           HTuple hv_FactorColumn = new HTuple(), hv_UseShadow = null;
           HTuple hv_ShadowColor = null, hv_Exception = new HTuple();
           HTuple hv_Width = new HTuple(), hv_Index = new HTuple();
           HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
           HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
           HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
           HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
           HTuple hv_CurrentColor = new HTuple();
           HTuple hv_Box_COPY_INP_TMP = hv_Box.Clone();
           HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
           HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
           HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
           HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

           // Initialize local and output iconic variables 
           //This procedure displays text in a graphics window.
           //
           //Input parameters:
           //WindowHandle: The WindowHandle of the graphics window, where
           //   the message should be displayed
           //String: A tuple of strings containing the text message to be displayed
           //CoordSystem: If set to 'window', the text position is given
           //   with respect to the window coordinate system.
           //   If set to 'image', image coordinates are used.
           //   (This may be useful in zoomed images.)
           //Row: The row coordinate of the desired text position
           //   If set to -1, a default value of 12 is used.
           //Column: The column coordinate of the desired text position
           //   If set to -1, a default value of 12 is used.
           //Color: defines the color of the text as string.
           //   If set to [], '' or 'auto' the currently set color is used.
           //   If a tuple of strings is passed, the colors are used cyclically
           //   for each new textline.
           //Box: If Box[0] is set to 'true', the text is written within an orange box.
           //     If set to' false', no box is displayed.
           //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
           //       the text is written in a box of that color.
           //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
           //       'true' -> display a shadow in a default color
           //       'false' -> display no shadow (same as if no second value is given)
           //       otherwise -> use given string as color string for the shadow color
           //
           //Prepare window
           HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
           HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
               out hv_Column2Part);
           HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
               out hv_WidthWin, out hv_HeightWin);
           HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
           //
           //default settings
           if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
           {
               hv_Row_COPY_INP_TMP = 12;
           }
           if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
           {
               hv_Column_COPY_INP_TMP = 12;
           }
           if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
           {
               hv_Color_COPY_INP_TMP = "";
           }
           //
           hv_String_COPY_INP_TMP = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit("\n");
           //
           //Estimate extentions of text depending on font size.
           HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
               out hv_MaxWidth, out hv_MaxHeight);
           if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
           {
               hv_R1 = hv_Row_COPY_INP_TMP.Clone();
               hv_C1 = hv_Column_COPY_INP_TMP.Clone();
           }
           else
           {
               //Transform image to window coordinates
               hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
               hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
               hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
               hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
           }
           //
           //Display text box depending on text size
           hv_UseShadow = 1;
           hv_ShadowColor = "gray";
           if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleEqual("true"))) != 0)
           {
               if (hv_Box_COPY_INP_TMP == null)
                   hv_Box_COPY_INP_TMP = new HTuple();
               hv_Box_COPY_INP_TMP[0] = "#fce9d4";
               hv_ShadowColor = "#f28d26";
           }
           if ((int)(new HTuple((new HTuple(hv_Box_COPY_INP_TMP.TupleLength())).TupleGreater(
               1))) != 0)
           {
               if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual("true"))) != 0)
               {
                   //Use default ShadowColor set above
               }
               else if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual(
                   "false"))) != 0)
               {
                   hv_UseShadow = 0;
               }
               else
               {
                   hv_ShadowColor = hv_Box_COPY_INP_TMP[1];
                   //Valid color?
                   try
                   {
                       HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                           1));
                   }
                   // catch (Exception) 
                   catch (HalconException HDevExpDefaultException1)
                   {
                       HDevExpDefaultException1.ToHTuple(out hv_Exception);
                       hv_Exception = "Wrong value of control parameter Box[1] (must be a 'true', 'false', or a valid color string)";
                       throw new HalconException(hv_Exception);
                   }
               }
           }
           if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleNotEqual("false"))) != 0)
           {
               //Valid color?
               try
               {
                   HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
               }
               // catch (Exception) 
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   hv_Exception = "Wrong value of control parameter Box[0] (must be a 'true', 'false', or a valid color string)";
                   throw new HalconException(hv_Exception);
               }
               //Calculate box extents
               hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
               hv_Width = new HTuple();
               for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                   )) - 1); hv_Index = (int)hv_Index + 1)
               {
                   HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                       hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                   hv_Width = hv_Width.TupleConcat(hv_W);
               }
               hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                   ));
               hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
               hv_R2 = hv_R1 + hv_FrameHeight;
               hv_C2 = hv_C1 + hv_FrameWidth;
               //Display rectangles
               HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
               HOperatorSet.SetDraw(hv_WindowHandle, "fill");
               //Set shadow color
               HOperatorSet.SetColor(hv_WindowHandle, hv_ShadowColor);
               if ((int)(hv_UseShadow) != 0)
               {
                   HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 1, hv_C1 + 1, hv_R2 + 1, hv_C2 + 1);
               }
               //Set box color
               HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
               HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
               HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
           }
           //Write text.
           for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
               )) - 1); hv_Index = (int)hv_Index + 1)
           {
               hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                   )));
               if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                   "auto")))) != 0)
               {
                   HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
               }
               else
               {
                   HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
               }
               hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
               HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
               HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                   hv_Index));
           }
           //Reset changed window settings
           HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
           HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
               hv_Column2Part);

           return;
       }

       // Chapter: Graphics / Text
       // Short Description: Set font independent of OS 
       public static void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
           HTuple hv_Bold, HTuple hv_Slant)
       {



           // Local iconic variables 

           // Local control variables 

           HTuple hv_OS = null, hv_BufferWindowHandle = new HTuple();
           HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
           HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
           HTuple hv_Scale = new HTuple(), hv_Exception = new HTuple();
           HTuple hv_SubFamily = new HTuple(), hv_Fonts = new HTuple();
           HTuple hv_SystemFonts = new HTuple(), hv_Guess = new HTuple();
           HTuple hv_I = new HTuple(), hv_Index = new HTuple(), hv_AllowedFontSizes = new HTuple();
           HTuple hv_Distances = new HTuple(), hv_Indices = new HTuple();
           HTuple hv_FontSelRegexp = new HTuple(), hv_FontsCourier = new HTuple();
           HTuple hv_Bold_COPY_INP_TMP = hv_Bold.Clone();
           HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
           HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();
           HTuple hv_Slant_COPY_INP_TMP = hv_Slant.Clone();

           // Initialize local and output iconic variables 
           //This procedure sets the text font of the current window with
           //the specified attributes.
           //It is assumed that following fonts are installed on the system:
           //Windows: Courier New, Arial Times New Roman
           //Mac OS X: CourierNewPS, Arial, TimesNewRomanPS
           //Linux: courier, helvetica, times
           //Because fonts are displayed smaller on Linux than on Windows,
           //a scaling factor of 1.25 is used the get comparable results.
           //For Linux, only a limited number of font sizes is supported,
           //to get comparable results, it is recommended to use one of the
           //following sizes: 9, 11, 14, 16, 20, 27
           //(which will be mapped internally on Linux systems to 11, 14, 17, 20, 25, 34)
           //
           //Input parameters:
           //WindowHandle: The graphics window for which the font will be set
           //Size: The font size. If Size=-1, the default of 16 is used.
           //Bold: If set to 'true', a bold font is used
           //Slant: If set to 'true', a slanted font is used
           //
           HOperatorSet.GetSystem("operating_system", out hv_OS);
           // dev_get_preferences(...); only in hdevelop
           // dev_set_preferences(...); only in hdevelop
           if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
               new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
           {
               hv_Size_COPY_INP_TMP = 16;
           }
           if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
           {
               //Set font on Windows systems
               try
               {
                   //Check, if font scaling is switched on
                   HOperatorSet.OpenWindow(0, 0, 256, 256, 0, "buffer", "", out hv_BufferWindowHandle);
                   HOperatorSet.SetFont(hv_BufferWindowHandle, "-Consolas-16-*-0-*-*-1-");
                   HOperatorSet.GetStringExtents(hv_BufferWindowHandle, "test_string", out hv_Ascent,
                       out hv_Descent, out hv_Width, out hv_Height);
                   //Expected width is 110
                   hv_Scale = 110.0 / hv_Width;
                   hv_Size_COPY_INP_TMP = ((hv_Size_COPY_INP_TMP * hv_Scale)).TupleInt();
                   HOperatorSet.CloseWindow(hv_BufferWindowHandle);
               }
               // catch (Exception) 
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   //throw (Exception)
               }
               if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                   new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
               {
                   hv_Font_COPY_INP_TMP = "Courier New";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
               {
                   hv_Font_COPY_INP_TMP = "Consolas";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
               {
                   hv_Font_COPY_INP_TMP = "Arial";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
               {
                   hv_Font_COPY_INP_TMP = "Times New Roman";
               }
               if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
               {
                   hv_Bold_COPY_INP_TMP = 1;
               }
               else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
               {
                   hv_Bold_COPY_INP_TMP = 0;
               }
               else
               {
                   hv_Exception = "Wrong value of control parameter Bold";
                   throw new HalconException(hv_Exception);
               }
               if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
               {
                   hv_Slant_COPY_INP_TMP = 1;
               }
               else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
               {
                   hv_Slant_COPY_INP_TMP = 0;
               }
               else
               {
                   hv_Exception = "Wrong value of control parameter Slant";
                   throw new HalconException(hv_Exception);
               }
               try
               {
                   HOperatorSet.SetFont(hv_WindowHandle, ((((((("-" + hv_Font_COPY_INP_TMP) + "-") + hv_Size_COPY_INP_TMP) + "-*-") + hv_Slant_COPY_INP_TMP) + "-*-*-") + hv_Bold_COPY_INP_TMP) + "-");
               }
               // catch (Exception) 
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   //throw (Exception)
               }
           }
           else if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Dar"))) != 0)
           {
               //Set font on Mac OS X systems. Since OS X does not have a strict naming
               //scheme for font attributes, we use tables to determine the correct font
               //name.
               hv_SubFamily = 0;
               if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
               {
                   hv_SubFamily = hv_SubFamily.TupleBor(1);
               }
               else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
               {
                   hv_Exception = "Wrong value of control parameter Slant";
                   throw new HalconException(hv_Exception);
               }
               if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
               {
                   hv_SubFamily = hv_SubFamily.TupleBor(2);
               }
               else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
               {
                   hv_Exception = "Wrong value of control parameter Bold";
                   throw new HalconException(hv_Exception);
               }
               if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
               {
                   hv_Fonts = new HTuple();
                   hv_Fonts[0] = "Menlo-Regular";
                   hv_Fonts[1] = "Menlo-Italic";
                   hv_Fonts[2] = "Menlo-Bold";
                   hv_Fonts[3] = "Menlo-BoldItalic";
               }
               else if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                   new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
               {
                   hv_Fonts = new HTuple();
                   hv_Fonts[0] = "CourierNewPSMT";
                   hv_Fonts[1] = "CourierNewPS-ItalicMT";
                   hv_Fonts[2] = "CourierNewPS-BoldMT";
                   hv_Fonts[3] = "CourierNewPS-BoldItalicMT";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
               {
                   hv_Fonts = new HTuple();
                   hv_Fonts[0] = "ArialMT";
                   hv_Fonts[1] = "Arial-ItalicMT";
                   hv_Fonts[2] = "Arial-BoldMT";
                   hv_Fonts[3] = "Arial-BoldItalicMT";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
               {
                   hv_Fonts = new HTuple();
                   hv_Fonts[0] = "TimesNewRomanPSMT";
                   hv_Fonts[1] = "TimesNewRomanPS-ItalicMT";
                   hv_Fonts[2] = "TimesNewRomanPS-BoldMT";
                   hv_Fonts[3] = "TimesNewRomanPS-BoldItalicMT";
               }
               else
               {
                   //Attempt to figure out which of the fonts installed on the system
                   //the user could have meant.
                   HOperatorSet.QueryFont(hv_WindowHandle, out hv_SystemFonts);
                   hv_Fonts = new HTuple();
                   hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Guess = new HTuple();
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Regular");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "MT");
                   for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                   {
                       HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                       if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                       {
                           if (hv_Fonts == null)
                               hv_Fonts = new HTuple();
                           hv_Fonts[0] = hv_Guess.TupleSelect(hv_I);
                           break;
                       }
                   }
                   //Guess name of slanted font
                   hv_Guess = new HTuple();
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Italic");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-ItalicMT");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Oblique");
                   for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                   {
                       HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                       if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                       {
                           if (hv_Fonts == null)
                               hv_Fonts = new HTuple();
                           hv_Fonts[1] = hv_Guess.TupleSelect(hv_I);
                           break;
                       }
                   }
                   //Guess name of bold font
                   hv_Guess = new HTuple();
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Bold");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldMT");
                   for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                   {
                       HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                       if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                       {
                           if (hv_Fonts == null)
                               hv_Fonts = new HTuple();
                           hv_Fonts[2] = hv_Guess.TupleSelect(hv_I);
                           break;
                       }
                   }
                   //Guess name of bold slanted font
                   hv_Guess = new HTuple();
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalic");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalicMT");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldOblique");
                   for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                   {
                       HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                       if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                       {
                           if (hv_Fonts == null)
                               hv_Fonts = new HTuple();
                           hv_Fonts[3] = hv_Guess.TupleSelect(hv_I);
                           break;
                       }
                   }
               }
               hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(hv_SubFamily);
               try
               {
                   HOperatorSet.SetFont(hv_WindowHandle, (hv_Font_COPY_INP_TMP + "-") + hv_Size_COPY_INP_TMP);
               }
               // catch (Exception) 
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   //throw (Exception)
               }
           }
           else
           {
               //Set font for UNIX systems
               hv_Size_COPY_INP_TMP = hv_Size_COPY_INP_TMP * 1.25;
               hv_AllowedFontSizes = new HTuple();
               hv_AllowedFontSizes[0] = 11;
               hv_AllowedFontSizes[1] = 14;
               hv_AllowedFontSizes[2] = 17;
               hv_AllowedFontSizes[3] = 20;
               hv_AllowedFontSizes[4] = 25;
               hv_AllowedFontSizes[5] = 34;
               if ((int)(new HTuple(((hv_AllowedFontSizes.TupleFind(hv_Size_COPY_INP_TMP))).TupleEqual(
                   -1))) != 0)
               {
                   hv_Distances = ((hv_AllowedFontSizes - hv_Size_COPY_INP_TMP)).TupleAbs();
                   HOperatorSet.TupleSortIndex(hv_Distances, out hv_Indices);
                   hv_Size_COPY_INP_TMP = hv_AllowedFontSizes.TupleSelect(hv_Indices.TupleSelect(
                       0));
               }
               if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))).TupleOr(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(
                   "Courier")))) != 0)
               {
                   hv_Font_COPY_INP_TMP = "courier";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
               {
                   hv_Font_COPY_INP_TMP = "helvetica";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
               {
                   hv_Font_COPY_INP_TMP = "times";
               }
               if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
               {
                   hv_Bold_COPY_INP_TMP = "bold";
               }
               else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
               {
                   hv_Bold_COPY_INP_TMP = "medium";
               }
               else
               {
                   hv_Exception = "Wrong value of control parameter Bold";
                   throw new HalconException(hv_Exception);
               }
               if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
               {
                   if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("times"))) != 0)
                   {
                       hv_Slant_COPY_INP_TMP = "i";
                   }
                   else
                   {
                       hv_Slant_COPY_INP_TMP = "o";
                   }
               }
               else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
               {
                   hv_Slant_COPY_INP_TMP = "r";
               }
               else
               {
                   hv_Exception = "Wrong value of control parameter Slant";
                   throw new HalconException(hv_Exception);
               }
               try
               {
                   HOperatorSet.SetFont(hv_WindowHandle, ((((((("-adobe-" + hv_Font_COPY_INP_TMP) + "-") + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
               }
               // catch (Exception) 
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   if ((int)((new HTuple(((hv_OS.TupleSubstr(0, 4))).TupleEqual("Linux"))).TupleAnd(
                       new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                   {
                       HOperatorSet.QueryFont(hv_WindowHandle, out hv_Fonts);
                       hv_FontSelRegexp = (("^-[^-]*-[^-]*[Cc]ourier[^-]*-" + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP;
                       hv_FontsCourier = ((hv_Fonts.TupleRegexpSelect(hv_FontSelRegexp))).TupleRegexpMatch(
                           hv_FontSelRegexp);
                       if ((int)(new HTuple((new HTuple(hv_FontsCourier.TupleLength())).TupleEqual(
                           0))) != 0)
                       {
                           hv_Exception = "Wrong font name";
                           //throw (Exception)
                       }
                       else
                       {
                           try
                           {
                               HOperatorSet.SetFont(hv_WindowHandle, (((hv_FontsCourier.TupleSelect(
                                   0)) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
                           }
                           // catch (Exception) 
                           catch (HalconException HDevExpDefaultException2)
                           {
                               HDevExpDefaultException2.ToHTuple(out hv_Exception);
                               //throw (Exception)
                           }
                       }
                   }
                   //throw (Exception)
               }
           }
           // dev_set_preferences(...); only in hdevelop

           return;
       }
       // Chapter: XLD / Creation
       // Short Description: Creates an arrow shaped XLD contour. 
       public static void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
           HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
       {



           // Stack for temporary objects 
           HObject[] OTemp = new HObject[20];

           // Local iconic variables 

           HObject ho_TempArrow = null;

           // Local control variables 

           HTuple hv_Length = null, hv_ZeroLengthIndices = null;
           HTuple hv_DR = null, hv_DC = null, hv_HalfHeadWidth = null;
           HTuple hv_RowP1 = null, hv_ColP1 = null, hv_RowP2 = null;
           HTuple hv_ColP2 = null, hv_Index = null;
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_Arrow);
           HOperatorSet.GenEmptyObj(out ho_TempArrow);
           //This procedure generates arrow shaped XLD contours,
           //pointing from (Row1, Column1) to (Row2, Column2).
           //If starting and end point are identical, a contour consisting
           //of a single point is returned.
           //
           //input parameteres:
           //Row1, Column1: Coordinates of the arrows' starting points
           //Row2, Column2: Coordinates of the arrows' end points
           //HeadLength, HeadWidth: Size of the arrow heads in pixels
           //
           //output parameter:
           //Arrow: The resulting XLD contour
           //
           //The input tuples Row1, Column1, Row2, and Column2 have to be of
           //the same length.
           //HeadLength and HeadWidth either have to be of the same length as
           //Row1, Column1, Row2, and Column2 or have to be a single element.
           //If one of the above restrictions is violated, an error will occur.
           //
           //
           //Init
           ho_Arrow.Dispose();
           HOperatorSet.GenEmptyObj(out ho_Arrow);
           //
           //Calculate the arrow length
           HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
           //
           //Mark arrows with identical start and end point
           //(set Length to -1 to avoid division-by-zero exception)
           hv_ZeroLengthIndices = hv_Length.TupleFind(0);
           if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
           {
               if (hv_Length == null)
                   hv_Length = new HTuple();
               hv_Length[hv_ZeroLengthIndices] = -1;
           }
           //
           //Calculate auxiliary variables.
           hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
           hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
           hv_HalfHeadWidth = hv_HeadWidth / 2.0;
           //
           //Calculate end points of the arrow head.
           hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
           hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
           hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
           hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
           //
           //Finally create output XLD contour for each input point pair
           for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
           {
               if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
               {
                   //Create_ single points for arrows with identical start and end point
                   ho_TempArrow.Dispose();
                   HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(hv_Index),
                       hv_Column1.TupleSelect(hv_Index));
               }
               else
               {
                   //Create arrow contour
                   ho_TempArrow.Dispose();
                   HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                       hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                       hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                       hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                       ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                       hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                       hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                       hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
               }
               {
                   HObject ExpTmpOutVar_0;
                   HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                   ho_Arrow.Dispose();
                   ho_Arrow = ExpTmpOutVar_0;
               }
           }
           ho_TempArrow.Dispose();

           return;
       }
        #endregion
    }
}
