using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using ViewWindow.Model;
using HalconTool;
using System.Windows.Forms;

namespace VMPro
{
    [Serializable]
    internal class BatteryFirstAlignTool : ToolBase
    {
        internal BatteryFirstAlignTool()
        {
            //如果没有指定卡尺，就创建一个              
            GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.viewWindow.genRect1(200.0, 200.0, 600.0, 800.0, ref regions);
            GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).regions = this.regions;
        }
        /// <summary>
        /// 输入图像
        /// </summary>
        internal HObject inputImage;
        internal int pointIndex = 1;
        internal int lineIndex = 1;
        internal HObject SearchRegion;
        public HObject imageWithoutEar;
        internal XYU outputPos = new XYU();
        internal List<ViewWindow.Model.ROI> regions = new List<ROI>();
        public int minThreshold = 0;
        public int maxThreshold = 128;
        public Int32 minArea = 100;
        public Int32 maxArea = 10000000;
        public int dilationAndErosionSize = 5;
        private object obj = new object();
        // Procedures 
        // Local procedures 
        public int BatteryAlign(HObject ho_inputImage, HObject ho_searchRegion, HTuple hv_windowHandle,
            HTuple hv_pointIndex, HTuple hv_minThreshold, HTuple hv_maxThreshold, HTuple hv_minArea,
            HTuple hv_maxArea, HTuple hv_angleIndex, out HTuple hv_X, out HTuple hv_Y, out HTuple hv_U)
        {


            try
            {

                // Stack for temporary objects 
                HObject[] OTemp = new HObject[20];

                // Local iconic variables 

                HObject ho_Image = null, ho_Region, ho_ConnectedRegions;
                HObject ho_SelectedRegions, ho_RegionDilation, ho_RegionFillUp;
                HObject ho_Rectangle, ho_RegionDifference, ho_Rectangle2;
                HObject ho_RegionDifference1, ho_RegionErosion, ho_Cross;
                HObject ho_Cross1, ho_Cross2, ho_Cross3, ho_Contours = null;
                HObject ho_Contour = null;

                // Local control variables 

                HTuple hv_Row3 = null, hv_Column3 = null, hv_Phi = null;
                HTuple hv_Length1 = null, hv_Length2 = null, hv_Area1 = null;
                HTuple hv_Row1 = null, hv_Column1 = null, hv_Row4 = null;
                HTuple hv_Column4 = null, hv_Phi1 = null, hv_Length11 = null;
                HTuple hv_Length21 = null, hv_Area = null, hv_Row = null;
                HTuple hv_Column = null, hv_Row5 = null, hv_Column5 = null;
                HTuple hv_Phi2 = null, hv_Length12 = null, hv_Length22 = null;
                HTuple hv_Cos = null, hv_Sin = null, hv_RT_X = null, hv_RT_Y = null;
                HTuple hv_RB_X = null, hv_RB_Y = null, hv_LB_X = null;
                HTuple hv_LB_Y = null, hv_LT_X = null, hv_LT_Y = null;
                HTuple hv_MetrologyHandle = new HTuple(), hv_Width = new HTuple();
                HTuple hv_Height = new HTuple(), hv_Index = new HTuple();
                HTuple hv_Row6 = new HTuple(), hv_Column6 = new HTuple();
                HTuple hv_Parameter = new HTuple(), hv_startR1 = new HTuple();
                HTuple hv_startC1 = new HTuple(), hv_endR1 = new HTuple();
                HTuple hv_endC1 = new HTuple(), hv_startR2 = new HTuple();
                HTuple hv_startC2 = new HTuple(), hv_endR2 = new HTuple();
                HTuple hv_endC2 = new HTuple(), hv_startR3 = new HTuple();
                HTuple hv_startC3 = new HTuple(), hv_endR3 = new HTuple();
                HTuple hv_endC3 = new HTuple(), hv_startR4 = new HTuple();
                HTuple hv_startC4 = new HTuple(), hv_endR4 = new HTuple();
                HTuple hv_endC4 = new HTuple(), hv_IsOverlapping = new HTuple();
                // Initialize local and output iconic variables 
                HOperatorSet.GenEmptyObj(out ho_Image);
                HOperatorSet.GenEmptyObj(out ho_Region);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
                HOperatorSet.GenEmptyObj(out ho_RegionDilation);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
                HOperatorSet.GenEmptyObj(out ho_Rectangle);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference);
                HOperatorSet.GenEmptyObj(out ho_Rectangle2);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
                HOperatorSet.GenEmptyObj(out ho_RegionErosion);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_Cross1);
                HOperatorSet.GenEmptyObj(out ho_Cross2);
                HOperatorSet.GenEmptyObj(out ho_Cross3);
                HOperatorSet.GenEmptyObj(out ho_Contours);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                hv_X = new HTuple();
                hv_Y = new HTuple();
                hv_U = new HTuple();
                hv_X =0;
                hv_Y = 0;
                hv_U = 0;
                HOperatorSet.SetDraw(hv_windowHandle, "margin");
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_inputImage, ho_searchRegion, out ExpTmpOutVar_0);
                    ho_Image.Dispose();
                    ho_Image = ExpTmpOutVar_0;
                }
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_Image, out ho_Region, hv_minThreshold, hv_maxThreshold);


                //GetImageWindowControl().hwc_imageWindow.DispObj(ho_Region,"red" );
                //Application.DoEvents();

                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", hv_minArea, hv_maxArea);
                ho_RegionDilation.Dispose();
                HOperatorSet.DilationCircle(ho_SelectedRegions, out ho_RegionDilation, 5);
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionDilation, out ho_RegionFillUp);
                HTuple area, row, col;
                HOperatorSet.AreaCenter(ho_RegionFillUp, out area, out row, out col);
                if (area.Length == 0)
                {
                    return 1;
                }


                //求电池整个外接区域
                HOperatorSet.SmallestRectangle2(ho_RegionFillUp, out hv_Row3, out hv_Column3,
                    out hv_Phi, out hv_Length1, out hv_Length2);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row3, hv_Column3, hv_Phi, hv_Length1,
                    hv_Length2);
                HOperatorSet.AreaCenter(ho_Rectangle, out hv_Area1, out hv_Row1, out hv_Column1);
                GetImageWindowControl().hwc_imageWindow.DispObj(ho_Rectangle, "red");
                Application.DoEvents();



                //求极耳外接区域
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Rectangle, ho_RegionFillUp, out ho_RegionDifference
                    );
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ErosionCircle(ho_RegionDifference, out ExpTmpOutVar_0, 20);
                    ho_RegionDifference.Dispose();
                    ho_RegionDifference = ExpTmpOutVar_0;
                }
                HOperatorSet.SmallestRectangle2(ho_RegionDifference, out hv_Row4, out hv_Column4,
                    out hv_Phi1, out hv_Length11, out hv_Length21);
                ho_Rectangle2.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row4, hv_Column4, hv_Phi1, hv_Length11,
                    hv_Length21);
                HOperatorSet.AreaCenter(ho_Rectangle2, out hv_Area, out hv_Row, out hv_Column);
                GetImageWindowControl().hwc_imageWindow.DispObj(ho_Rectangle2, "orange");
                Application.DoEvents();


                //求电池本体外接区域
                ho_RegionDifference1.Dispose();
                HOperatorSet.Difference(ho_Rectangle, ho_Rectangle2, out ho_RegionDifference1
                    );
                ho_RegionErosion.Dispose();
                HOperatorSet.ErosionCircle(ho_RegionDifference1, out ho_RegionErosion, 20);
                HOperatorSet.SmallestRectangle2(ho_RegionErosion, out hv_Row5, out hv_Column5,
                    out hv_Phi2, out hv_Length12, out hv_Length22);
                if (hv_Row5.D == 0||hv_Row5 .Length !=1)
                {
                    return 2;
                }
                GetImageWindowControl().hwc_imageWindow.DispObj(ho_RegionErosion, "green");
                Application.DoEvents();



                HOperatorSet.TupleCos(hv_Phi2, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi2, out hv_Sin);

                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetColor(HDevWindowStack.GetActive(), "green");
                }
                hv_RT_X = ((-hv_Length12) * hv_Cos) - (hv_Length22 * hv_Sin);
                hv_RT_Y = ((-hv_Length12) * hv_Sin) + (hv_Length22 * hv_Cos);
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row5 - hv_RT_Y, hv_Column5 + hv_RT_X,
                    60, hv_Phi2);

                hv_RB_X = (hv_Length12 * hv_Cos) - (hv_Length22 * hv_Sin);
                hv_RB_Y = (hv_Length12 * hv_Sin) + (hv_Length22 * hv_Cos);
                ho_Cross1.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_Row5 - hv_RB_Y, hv_Column5 + hv_RB_X,
                    60, hv_Phi2);

                hv_LB_X = (hv_Length12 * hv_Cos) + (hv_Length22 * hv_Sin);
                hv_LB_Y = (hv_Length12 * hv_Sin) - (hv_Length22 * hv_Cos);
                ho_Cross2.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_Row5 - hv_LB_Y, hv_Column5 + hv_LB_X,
                    60, hv_Phi2);

                hv_LT_X = ((-hv_Length12) * hv_Cos) + (hv_Length22 * hv_Sin);
                hv_LT_Y = ((-hv_Length12) * hv_Sin) - (hv_Length22 * hv_Cos);
                ho_Cross3.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_Row5 - hv_LT_Y, hv_Column5 + hv_LT_X,
                    60, hv_Phi2);

                HObject temp;
                HOperatorSet.Threshold(ho_Image, out temp, 0, 200);
                HObject binImage;
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                HOperatorSet.RegionToBin(temp, out binImage, 0, 255, hv_Width, hv_Height);

                //查找第一条线
                if ((int)((new HTuple(hv_pointIndex.TupleEqual(1))).TupleOr(new HTuple(hv_pointIndex.TupleEqual(
                    2)))) != 0)
                {
                    HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                    HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                    HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_Row5 - hv_RT_Y,
                        hv_Column5 + hv_RT_X, hv_Row5 - hv_RB_Y, hv_Column5 + hv_RB_X, 30, 5, 1, 30,
                        new HTuple(), new HTuple(), out hv_Index);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",0.3);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 100);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "negative");
                    HOperatorSet.ApplyMetrologyModel(binImage, hv_MetrologyHandle);
                    HOperatorSet.SetColor(hv_windowHandle, "blue");
                    ho_Contours.Dispose();
                    HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                        "all", "all", out hv_Row6, out hv_Column6);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                        "all_param", out hv_Parameter);
                    HOperatorSet.SetColor(hv_windowHandle, "green");
                    ho_Contour.Dispose();
                    HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                        "all", "all", 1.5);
                    GetImageWindowControl().hwc_imageWindow.DispObj(ho_Contours, "blue");
                    GetImageWindowControl().hwc_imageWindow.DispObj(ho_Contour, "green");
                    HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                    if ((int)(new HTuple((new HTuple(hv_Parameter.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_startR1 = hv_Parameter.TupleSelect(0);
                        hv_startC1 = hv_Parameter.TupleSelect(1);
                        hv_endR1 = hv_Parameter.TupleSelect(2);
                        hv_endC1 = hv_Parameter.TupleSelect(3);
                    }
                    else
                    {
                        return 3;
                    }
                }

                //查找第二条线
                if ((int)((new HTuple(hv_pointIndex.TupleEqual(2))).TupleOr(new HTuple(hv_pointIndex.TupleEqual(
                    3)))) != 0)
                {
                    HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                    HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                    HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_Row5 - hv_RB_Y,
                        hv_Column5 + hv_RB_X, hv_Row5 - hv_LB_Y, hv_Column5 + hv_LB_X, 30, 5, 1, 30,
                        new HTuple(), new HTuple(), out hv_Index);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.3);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 100);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "negative");
                    HOperatorSet.ApplyMetrologyModel(binImage, hv_MetrologyHandle);
                    HOperatorSet.SetColor(hv_windowHandle, "blue");
                    ho_Contours.Dispose();
                    HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                        "all", "all", out hv_Row6, out hv_Column6);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                        "all_param", out hv_Parameter);
                    HOperatorSet.SetColor(hv_windowHandle, "green");
                    ho_Contour.Dispose();
                    HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                        "all", "all", 1.5);
                    GetImageWindowControl().hwc_imageWindow.DispObj(ho_Contours, "blue");
                    GetImageWindowControl().hwc_imageWindow.DispObj(ho_Contour, "green");
                    HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                    if ((int)(new HTuple((new HTuple(hv_Parameter.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_startR2 = hv_Parameter.TupleSelect(0);
                        hv_startC2 = hv_Parameter.TupleSelect(1);
                        hv_endR2 = hv_Parameter.TupleSelect(2);
                        hv_endC2 = hv_Parameter.TupleSelect(3);
                    }
                    else
                    {
                        return 3;
                    }
                }


                //查找第三条线
                if ((int)((new HTuple(hv_pointIndex.TupleEqual(3))).TupleOr(new HTuple(hv_pointIndex.TupleEqual(
                    4)))) != 0)
                {
                    HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                    HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                    HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_Row5 - hv_LB_Y,
                        hv_Column5 + hv_LB_X, hv_Row5 - hv_LT_Y, hv_Column5 + hv_LT_X, 30, 5, 1, 30,
                        new HTuple(), new HTuple(), out hv_Index);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.3);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 100);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "negative");
                    HOperatorSet.ApplyMetrologyModel(binImage, hv_MetrologyHandle);
                    HOperatorSet.SetColor(hv_windowHandle, "blue");
                    ho_Contours.Dispose();
                    HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                        "all", "all", out hv_Row6, out hv_Column6);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                        "all_param", out hv_Parameter);
                    HOperatorSet.SetColor(hv_windowHandle, "green");
                    ho_Contour.Dispose();
                    HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                        "all", "all", 1.5);
                    GetImageWindowControl().hwc_imageWindow.DispObj(ho_Contours, "blue");
                    GetImageWindowControl().hwc_imageWindow.DispObj(ho_Contour, "green");
                    HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                    if ((int)(new HTuple((new HTuple(hv_Parameter.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_startR3 = hv_Parameter.TupleSelect(0);
                        hv_startC3 = hv_Parameter.TupleSelect(1);
                        hv_endR3 = hv_Parameter.TupleSelect(2);
                        hv_endC3 = hv_Parameter.TupleSelect(3);
                    }
                    else
                    {
                        return 3;
                    }
                }


                //查找第四条线
                if ((int)((new HTuple(hv_pointIndex.TupleEqual(4))).TupleOr(new HTuple(hv_pointIndex.TupleEqual(
                    1)))) != 0)
                {
                    HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                    HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                    HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_Row5 - hv_LT_Y,
                        hv_Column5 + hv_LT_X, hv_Row5 - hv_RT_Y, hv_Column5 + hv_RT_X, 30, 5, 1, 30,
                        new HTuple(), new HTuple(), out hv_Index);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.3);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 100);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "negative");
                    HOperatorSet.ApplyMetrologyModel(binImage, hv_MetrologyHandle);
                    HOperatorSet.SetColor(hv_windowHandle, "blue");
                    ho_Contours.Dispose();
                    HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                        "all", "all", out hv_Row6, out hv_Column6);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                        "all_param", out hv_Parameter);
                    HOperatorSet.SetColor(hv_windowHandle, "green");
                    ho_Contour.Dispose();
                    HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                        "all", "all", 1.5);
                    GetImageWindowControl().hwc_imageWindow.DispObj(ho_Contours, "blue");
                    GetImageWindowControl().hwc_imageWindow.DispObj(ho_Contour, "green");
                    HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                    if ((int)(new HTuple((new HTuple(hv_Parameter.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_startR4 = hv_Parameter.TupleSelect(0);
                        hv_startC4 = hv_Parameter.TupleSelect(1);
                        hv_endR4 = hv_Parameter.TupleSelect(2);
                        hv_endC4 = hv_Parameter.TupleSelect(3);
                    }
                    else
                    {
                        return 3;
                    }
                }


                //得到四个交点
                HOperatorSet.SetColor(hv_windowHandle, "green");
                HOperatorSet.SetDraw(hv_windowHandle, "fill");
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetDraw(HDevWindowStack.GetActive(), "fill");
                }
                if ((int)(new HTuple(hv_pointIndex.TupleEqual(1))) != 0)
                {
                    HOperatorSet.IntersectionLines(hv_startR4, hv_startC4, hv_endR4, hv_endC4,
                        hv_startR1, hv_startC1, hv_endR1, hv_endC1, out hv_X, out hv_Y, out hv_IsOverlapping);
                    HOperatorSet.DispCircle(hv_windowHandle, hv_X, hv_Y, 15);
                }
                else if ((int)(new HTuple(hv_pointIndex.TupleEqual(2))) != 0)
                {
                    HOperatorSet.IntersectionLines(hv_startR1, hv_startC1, hv_endR1, hv_endC1,
                        hv_startR2, hv_startC2, hv_endR2, hv_endC2, out hv_X, out hv_Y, out hv_IsOverlapping);
                    HOperatorSet.DispCircle(hv_windowHandle, hv_X, hv_Y, 15);
                }
                else if ((int)(new HTuple(hv_pointIndex.TupleEqual(3))) != 0)
                {
                    HOperatorSet.IntersectionLines(hv_startR2, hv_startC2, hv_endR2, hv_endC2,
                        hv_startR3, hv_startC3, hv_endR3, hv_endC3, out hv_X, out hv_Y, out hv_IsOverlapping);
                    HOperatorSet.DispCircle(hv_windowHandle, hv_X, hv_Y, 15);
                }
                else if ((int)(new HTuple(hv_pointIndex.TupleEqual(4))) != 0)
                {
                    HOperatorSet.IntersectionLines(hv_startR3, hv_startC3, hv_endR3, hv_endC3,
                        hv_startR4, hv_startC4, hv_endR4, hv_endC4, out hv_X, out hv_Y, out hv_IsOverlapping);
                    HOperatorSet.DispCircle(hv_windowHandle, hv_X, hv_Y, 15);
                }


                //取线一的方向作为产品方向
                if ((int)(new HTuple(hv_angleIndex.TupleEqual(1))) != 0)
                {
                    HOperatorSet.AngleLx(hv_startR1, hv_startC1, hv_endR1, hv_endC1, out hv_U);
                }
                else if ((int)(new HTuple(hv_angleIndex.TupleEqual(2))) != 0)
                {
                    HOperatorSet.AngleLx(hv_endR2, hv_endC2, hv_startR2, hv_startC2, out hv_U);
                }
                else if ((int)(new HTuple(hv_angleIndex.TupleEqual(3))) != 0)
                {
                    HOperatorSet.AngleLx(hv_endR3, hv_endC3, hv_startR3, hv_startC3, out hv_U);
                }
                else if ((int)(new HTuple(hv_angleIndex.TupleEqual(4))) != 0)
                {
                    HOperatorSet.AngleLx(hv_endR4, hv_endC4, hv_startR4, hv_startC4, out hv_U);
                }

                HOperatorSet.DispCircle(hv_windowHandle, hv_Row5, hv_Column5, 15);
                HOperatorSet.DispArrow(hv_windowHandle, hv_Row5, hv_Column5, hv_Row, hv_Column,
                    5);

                ho_Image.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionFillUp.Dispose();
                ho_Rectangle.Dispose();
                ho_RegionDifference.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionDifference1.Dispose();
                ho_RegionErosion.Dispose();
                ho_Cross.Dispose();
                ho_Cross1.Dispose();
                ho_Cross2.Dispose();
                ho_Cross3.Dispose();
                ho_Contours.Dispose();
                ho_Contour.Dispose();

                return 0;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex );
                hv_X = 0;
                hv_Y = 0;
                hv_U = 0;
                return -1;
            }
        }
        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage1, bool b, string toolName)
        {
            try
            {
                lock (obj)
                {
                    GetImageWindowControl().hwc_imageWindow.DispObj(regions[0].getRegion(), "blue");
                    HTuple X, Y, U;
                    int result = BatteryAlign(inputImage, new HObject(regions[0].getRegion()), GetImageWindowControl(jobName).hwc_imageWindow.HWindowHalconID, pointIndex, minThreshold, maxThreshold, minArea, maxArea, lineIndex, out X, out Y, out U);
                    if (result == 1)
                    {
                        toolRunStatu = ToolRunStatu.未找到电池;
                        return;
                    }
                    else if (result == 2)
                    {
                        toolRunStatu = ToolRunStatu.电池形状异常或视野中有干扰物  ;
                        return;
                    }
                    else if (result == 3)
                    {
                        toolRunStatu = ToolRunStatu.未找到线;
                        return;
                    }
                    else if (result == -1)
                    {
                        toolRunStatu = ToolRunStatu.未知原因 ;
                        return;
                    }
                    outputPos.Point.X = X;
                    outputPos.Point.Y = Y;
                    outputPos.U = U;

                    //////HObject reducedImage;
                    //////HOperatorSet.ReduceDomain(inputImage, regions[0].getRegion(), out reducedImage);
                    //////HObject batteryRegion;
                    //////HOperatorSet.Threshold(reducedImage, out batteryRegion, minThreshold, maxThreshold);

                    //////HOperatorSet.Connection(batteryRegion, out batteryRegion);
                    //////HOperatorSet.SelectShape(batteryRegion, out batteryRegion, "area", "and", minArea, maxArea);
                    //////HOperatorSet.DilationCircle(batteryRegion, out batteryRegion, dilationAndErosionSize);
                    //////HOperatorSet.FillUp(batteryRegion, out batteryRegion);
                    //////HOperatorSet.ErosionCircle(batteryRegion, out batteryRegion, dilationAndErosionSize);
                    //////GetImageWindowControl().hwc_imageWindow.DispObj(batteryRegion, "orange");

                    //////HObject smallestRectangle2, differenceRegion;
                    //////HTuple row, col, phi, length1, length2;
                    //////HOperatorSet.SmallestRectangle2(batteryRegion, out row, out col, out phi, out length1, out length2);

                    //////if (row.Length != 1)
                    //////{
                    //////    toolRunStatu = ToolRunStatu.未找到电池;
                    //////    return;
                    //////}

                    //////HOperatorSet.GenRectangle2(out smallestRectangle2, row, col, phi, length1, length2);
                    //////HOperatorSet.Difference(smallestRectangle2, batteryRegion, out differenceRegion);
                    //////HOperatorSet.ErosionCircle(differenceRegion, out differenceRegion, 10);

                    //////HObject smallestRectangle21;
                    //////HTuple row2, col2, phi2, length12, length22;
                    //////HOperatorSet.SmallestRectangle2(differenceRegion, out row2, out col2, out phi2, out length12, out length22);
                    //////HOperatorSet.GenRectangle2(out smallestRectangle21, row2, col2, phi2, length12, length22);
                    //////HTuple area1, row1, col1;
                    //////HOperatorSet.AreaCenter(smallestRectangle21, out area1, out row1, out col1);
                    //////HTuple row3, col3, area3;
                    //////HOperatorSet.AreaCenter(batteryRegion, out area3, out row3, out col3);

                    //////HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, "green");
                    //////HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, row3, col3, 0, 10);
                    //////HObject circle;
                    //////if (row3.Length != 1)
                    //////{
                    //////    toolRunStatu = ToolRunStatu.未知原因;
                    //////    return;
                    //////}

                    //////HOperatorSet.GenCircle(out circle, row3, col3, 5);
                    //////GetImageWindowControl().hwc_imageWindow.DispObj(circle, "green");

                    ////////为避免极耳干扰找线，此处找到极耳区域，然后挖掉这个区域再找线
                    //////HObject imageRegionWithoutEar;
                    //////HOperatorSet.DilationCircle(differenceRegion, out differenceRegion, 10);
                    //////HOperatorSet.Difference(smallestRectangle21, differenceRegion, out imageRegionWithoutEar);
                    //////HOperatorSet.Connection(imageRegionWithoutEar, out imageRegionWithoutEar);
                    //////HOperatorSet.SelectShape(imageRegionWithoutEar, out imageRegionWithoutEar, "area", "and", 100, 1000000);

                    //////HOperatorSet.DilationCircle(imageRegionWithoutEar, out imageRegionWithoutEar, 15);
                    //////GetImageWindowControl().hwc_imageWindow.DispObj(imageRegionWithoutEar, "yellow");
                    //////HTuple width, height;
                    //////HOperatorSet.GetImageSize(inputImage, out width, out height);
                    //////HObject rectangle1;
                    //////HOperatorSet.GenRectangle1(out rectangle1, 0, 0, height, width);
                    //////HOperatorSet.Difference(rectangle1, imageRegionWithoutEar, out imageRegionWithoutEar);
                    //////HOperatorSet.ReduceDomain(inputImage, imageRegionWithoutEar, out imageWithoutEar);



                    //////Line temp = new Line();
                    //////temp.StartPoint.X  = row3;
                    //////temp.StartPoint.Y  = col3;
                    //////temp.EndPoint.X  = row1;
                    //////temp.EndPoint.Y  = col1;
                    //////HOperatorSet.DispArrow(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, row3, col3, row1, col1, 10);

                    //////outputPos.X = row3;
                    //////outputPos.Y = col3;
                    //////outputPos.U = temp.GetAngle();
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
