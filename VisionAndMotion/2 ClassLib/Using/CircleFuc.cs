using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;

namespace VersionMethods
{
    public class CircleParam
    {
        private int threshold = 50;
        private double sigma = 3.0;
        private int roiWidth = 5;
        private string transition = "all";
        private string select = "all";
        private string direction = "正";
        private int pointsCount = 30;
        private int ringWidth = 20;
        private double angleStart = 0;
        private double angleEnd = 360;
        private string circleParamPath = "";

        private bool followModel = false;
        private string modelParamPath = "";
        private string modelPath = "";

        private int level = 0;



        [Description("边缘幅度，对应halcon算子里的Threshold。(范围[0, 255])"), Category("找圆参数"), DefaultValue(50)]
        public int Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        [Description("平滑度，对应halcon算子里的Sigma。(范围[0, 25])"), Category("找圆参数"), DefaultValue(3.0)]
        public double Sigma
        {
            get { return sigma; }
            set { sigma = value; }
        }

        [Description("Roi宽度。"), Category("找圆参数"), DefaultValue(5)]
        public int RoiWidth
        {
            get { return roiWidth; }
            set { roiWidth = value; }
        }

        [Description("选择找点方式，对应halcon算子里的Transition。(all表示所有方式，positive表示'从暗到明'查找，nagetive表示'从明到暗'查找)")
        , Category("找点调节"), TypeConverter(typeof(TransitionString))]
        public string Transition
        {
            get { return transition; }
            set { transition = value; }
        }

        [Description("选择找点位置，对应halcon算子里的Select。(all表示所有方向，first表示第一个点，last表示最后一个点)"),
        Category("找点调节"), TypeConverter(typeof(SelectString))]
        public string Select
        {
            get { return select; }
            set { select = value; }
        }

        [Description("选择找点方向。('正'表示正常方向，'反'表示正常方向的相反方向)"), Category("找点调节"), TypeConverter(typeof(DirectionString))]
        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        [Description("拟合一个圆的点的个数。(点数越多，拟合的圆越准确，但所用的时间也越长)"), Category("找点调节"), DefaultValue(30)]
        public int PointsCount
        {
            get { return pointsCount; }
            set { pointsCount = value; }
        }

        [Description("圆环宽度。"), Category("圆环设置"), DefaultValue(20)]
        public int RingWidth
        {
            get { return ringWidth; }
            set { ringWidth = value; }
        }

        [Description("圆环的起始角度。(范围[-360, 360])"), Category("圆环设置"), DefaultValue(-45.0)]
        public double AngleStart
        {
            get { return angleStart; }
            set { angleStart = value; }
        }

        [Description("圆环的终止角度。(范围[0, 360])"), Category("圆环设置"), DefaultValue(90.0)]
        public double AngleEnd
        {
            get { return angleEnd; }
            set { angleEnd = value; }
        }

        //[Description("找圆参数保存路径。"), Category("参数保存"), Editor(typeof(PropertyGridSaveFileDlg), typeof(UITypeEditor))]
        public string CircleParamPath
        {
            get { return circleParamPath; }
            set { circleParamPath = value; }
        }

        /**************************************************************/
        [Description("是否跟随模板"), Category("模板相关"), DefaultValue(false)]
        public bool FollowModel
        {
            get { return followModel; }
            set { followModel = value; }
        }

        [Description("模板参数路径。"), Category("模板相关"), Editor(typeof(PropertyGridOpenFileDlg), typeof(UITypeEditor))]
        public string ModelParamPath
        {
            get { return modelParamPath; }
            set { modelParamPath = value; }
        }

        [Description("模板路径。"), Category("模板相关"), Editor(typeof(PropertyGridOpenFileDlg), typeof(UITypeEditor))]
        public string ModelPath
        {
            get { return modelPath; }
            set { modelPath = value; }
        }

        [Description("显示级别。(0级只显示圆，1级显示找圆的圆环，2级显示找点的测量小矩形，3级显示拟合圆的点)"), Category("显示级别")
        , DefaultValue(0), TypeConverter(typeof(LevelInt32))]
        public int Level
        {
            get { return level; }
            set { level = value; }
        }



        #region PropertyGrid属性调节
        public class TransitionString : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new string[] { "all", "positive", "negative" });
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        public class SelectString : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new string[] { "all", "first", "last" });
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        public class DirectionString : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new string[] { "正", "反" });
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        public class PropertyGridOpenFileDlg : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    OpenFileDialog fileDlg = new OpenFileDialog();
                    fileDlg.AddExtension = false;
                    if (fileDlg.ShowDialog().Equals(DialogResult.OK))
                    {
                        return fileDlg.FileName;
                    }
                }
                return value;
            }
        }

        public class LevelInt32 : Int32Converter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new Int32[] { 0, 1, 2, 3 });
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        #endregion
    }

public 	class CircleFuc
	{
        /// <summary>
        /// 跟新找圆的坐标
        /// </summary>
        /// <param name="window"></param>
        /// <param name="Row"></param>
        /// <param name="Col"></param>
        /// <param name="Radius"></param>
        public void getNewRoi(HWindow window, ref HTuple Row, ref HTuple Col, ref HTuple Radius)
        {
            try
            {
                HTuple row, col, radius;
                row = Row; col = Col; radius = Radius;

                HOperatorSet.DrawCircleMod(window, Row, Col, Radius, out Row, out Col, out Radius);
                if (Row.Length == 0)
                {
                    Row = row;
                    Col = col;
                    Radius = radius;
                }
            }
            catch { }
        }

        public void getRoiRingContours(HWindow window, out HObject m_Object, HTuple hv_Row,
      HTuple hv_Col, HTuple hv_Radius, HTuple hv_RingWidth, HTuple hv_angleStart,
      HTuple hv_angleEnd, HTuple hv_direction, HTuple hv_roiWidth)           //得到圆环的轮廓，包括箭头方向
        {
            HTuple angleRange, angleOffset, hv_i, PhiTemp, RowTemp, ColTemp;
            HObject RingContours, Arrow, Arrow1, Arrow2, Arrow3, Arrow4;
            HOperatorSet.GenEmptyObj(out m_Object);

            angleRange = hv_angleEnd - hv_angleStart;
            m_Object.Dispose();
            if (hv_RingWidth > 2 * hv_Radius)
            {
                disp_message(window, "圆环宽度不能大于圆的直径", "true", 1, 1, "red", "true");
                HOperatorSet.GenEmptyObj(out m_Object);
                return;
            }
            get_circle_ring_contours(out RingContours, hv_Row, hv_Col, hv_Radius,
                hv_RingWidth, hv_angleStart, hv_angleEnd);
            HOperatorSet.ConcatObj(RingContours, RingContours, out m_Object);

            if ((int)(new HTuple(angleRange.TupleEqual(360))) != 0)
            {
                gen_rectangle_arrow_contour(out Arrow, hv_Row, hv_Col + hv_Radius,
                    0 + (hv_direction * ((new HTuple(180)).TupleRad())), hv_RingWidth / 2.0,
                    hv_roiWidth / 2.0);

                HOperatorSet.ConcatObj(m_Object, Arrow, out m_Object);

                gen_rectangle_arrow_contour(out Arrow1, hv_Row - hv_Radius, hv_Col,
                    ((new HTuple(90)).TupleRad()) + (hv_direction * ((new HTuple(180)).TupleRad()
                    )), hv_RingWidth / 2.0, hv_roiWidth / 2.0);

                HOperatorSet.ConcatObj(m_Object, Arrow1, out m_Object);

                gen_rectangle_arrow_contour(out Arrow2, hv_Row, hv_Col - hv_Radius,
                    ((new HTuple(180)).TupleRad()) + (hv_direction * ((new HTuple(180)).TupleRad()
                    )), hv_RingWidth / 2.0, hv_roiWidth / 2.0);

                HOperatorSet.ConcatObj(m_Object, Arrow2, out m_Object);

                gen_rectangle_arrow_contour(out Arrow3, hv_Row + hv_Radius, hv_Col,
                    ((new HTuple(270)).TupleRad()) + (hv_direction * ((new HTuple(180)).TupleRad()
                    )), hv_RingWidth / 2.0, hv_roiWidth / 2.0);

                HOperatorSet.ConcatObj(m_Object, Arrow3, out m_Object);
            }
            if ((int)((new HTuple(angleRange.TupleLess(360))).TupleAnd(new HTuple(angleRange.TupleGreater(
                180)))) != 0)
            {
                angleOffset = angleRange / 4.0;
                for (hv_i = 1; (int)hv_i <= 3; hv_i = (int)hv_i + 1)
                {
                    PhiTemp = (hv_angleStart.TupleRad()) + (hv_i * (angleOffset.TupleRad()
                        ));
                    RowTemp = hv_Row - (hv_Radius * (PhiTemp.TupleSin()));
                    ColTemp = hv_Col + (hv_Radius * (PhiTemp.TupleCos()));

                    gen_rectangle_arrow_contour(out Arrow4, RowTemp, ColTemp,
                        PhiTemp + (hv_direction * ((new HTuple(180)).TupleRad())), hv_RingWidth / 2.0,
                        hv_roiWidth / 2.0);

                    HOperatorSet.ConcatObj(m_Object, Arrow4, out m_Object);
                }
            }
            if ((int)(new HTuple(angleRange.TupleLessEqual(180))) != 0)
            {
                angleOffset = angleRange / 3.0;
                for (hv_i = 1; (int)hv_i <= 2; hv_i = (int)hv_i + 1)
                {
                    PhiTemp = (hv_angleStart.TupleRad()) + (hv_i * (angleOffset.TupleRad()
                        ));
                    RowTemp = hv_Row - (hv_Radius * (PhiTemp.TupleSin()));
                    ColTemp = hv_Col + (hv_Radius * (PhiTemp.TupleCos()));

                    gen_rectangle_arrow_contour(out Arrow4, RowTemp, ColTemp,
                        PhiTemp + (hv_direction * ((new HTuple(180)).TupleRad())), hv_RingWidth / 2.0,
                        hv_roiWidth / 2.0);

                    HOperatorSet.ConcatObj(m_Object, Arrow4, out m_Object);
                }
            }
        }

        public void get_circle_ring_contours(out HObject m_Object, HTuple hv_Row,
      HTuple hv_Col, HTuple hv_Radius, HTuple hv_RingWidth, HTuple hv_angleStart,
      HTuple hv_angleEnd)           //获取圆环区域轮廓
        {


            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_Cross, ho_ContCircle1, ho_ContCircle2;
            HObject ho_Contour, ho_Contour1;


            // Local control variables 

            HTuple hv_Radius1, hv_Radius2, hv_anglestart;
            HTuple hv_angleend, hv_row1, hv_col1, hv_row2, hv_col2;
            HTuple hv_row3, hv_col3, hv_row4, hv_col4;

            HTuple hv_angleEnd_COPY_INP_TMP = hv_angleEnd.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out m_Object);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_ContCircle1);
            HOperatorSet.GenEmptyObj(out ho_ContCircle2);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Contour1);

            try
            {

                m_Object.Dispose();
                HOperatorSet.GenEmptyObj(out m_Object);

                if ((int)(new HTuple(hv_angleEnd_COPY_INP_TMP.TupleLess(hv_angleStart))) != 0)
                {
                    hv_angleEnd_COPY_INP_TMP = hv_angleEnd_COPY_INP_TMP + 360;
                }

                //gen_circle_contour_xld (ContCircle, Row, Col, Radius, rad(angleStart), rad(angleEnd), 'positive', 1)
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Col, 45, 0.785398);
                hv_Radius1 = hv_Radius + (hv_RingWidth / 2.0);
                hv_Radius2 = hv_Radius - (hv_RingWidth / 2.0);
                ho_ContCircle1.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircle1, hv_Row, hv_Col, hv_Radius1,
                    hv_angleStart.TupleRad(), hv_angleEnd_COPY_INP_TMP.TupleRad(), "positive",
                    1);
                ho_ContCircle2.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircle2, hv_Row, hv_Col, hv_Radius2,
                    hv_angleStart.TupleRad(), hv_angleEnd_COPY_INP_TMP.TupleRad(), "positive",
                    1);
                hv_anglestart = hv_angleStart.TupleRad();
                hv_angleend = hv_angleEnd_COPY_INP_TMP.TupleRad();

                hv_row1 = hv_Row - (hv_Radius1 * (hv_anglestart.TupleSin()));
                hv_col1 = hv_Col + (hv_Radius1 * (hv_anglestart.TupleCos()));
                //gen_cross_contour_xld (Cross1, row1, col1, 45, 0.785398)
                hv_row2 = hv_Row - (hv_Radius2 * (hv_anglestart.TupleSin()));
                hv_col2 = hv_Col + (hv_Radius2 * (hv_anglestart.TupleCos()));
                //gen_cross_contour_xld (Cross2, row2, col2, 45, 0.785398)
                //gen_contour_polygon_xld (Contour, [Row, row2], [Col, col2])
                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_row2.TupleConcat(hv_row1),
                    hv_col2.TupleConcat(hv_col1));

                hv_row3 = hv_Row - (hv_Radius1 * (hv_angleend.TupleSin()));
                hv_col3 = hv_Col + (hv_Radius1 * (hv_angleend.TupleCos()));
                //gen_cross_contour_xld (Cross3, row3, col3, 45, 0.785398)
                hv_row4 = hv_Row - (hv_Radius2 * (hv_angleend.TupleSin()));
                hv_col4 = hv_Col + (hv_Radius2 * (hv_angleend.TupleCos()));
                //gen_cross_contour_xld (Cross4, row4, col4, 45, 0.785398)
                //gen_contour_polygon_xld (Contour2, [Row, row4], [Col, col4])
                ho_Contour1.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour1, hv_row4.TupleConcat(hv_row3),
                    hv_col4.TupleConcat(hv_col3));

                OTemp[SP_O] = m_Object.CopyObj(1, -1);
                SP_O++;
                m_Object.Dispose();
                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Contour, out m_Object);
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                OTemp[SP_O] = m_Object.CopyObj(1, -1);
                SP_O++;
                m_Object.Dispose();
                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Contour1, out m_Object);
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                OTemp[SP_O] = m_Object.CopyObj(1, -1);
                SP_O++;
                m_Object.Dispose();
                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_ContCircle1, out m_Object);
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                OTemp[SP_O] = m_Object.CopyObj(1, -1);
                SP_O++;
                m_Object.Dispose();
                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_ContCircle2, out m_Object);
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                ho_Cross.Dispose();
                ho_ContCircle1.Dispose();
                ho_ContCircle2.Dispose();
                ho_Contour.Dispose();
                ho_Contour1.Dispose();

                return;

            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Cross.Dispose();
                ho_ContCircle1.Dispose();
                ho_ContCircle2.Dispose();
                ho_Contour.Dispose();
                ho_Contour1.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void gen_rectangle_arrow_contour(out HObject ho_Arrow, HTuple hv_Row, HTuple hv_Column,
      HTuple hv_Phi, HTuple hv_Length1, HTuple hv_Length2)      //根据矩形框画箭头
        {


            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_Contour, ho_Contour1;


            // Local control variables 

            HTuple hv_RowBegin, hv_ColBegin, hv_RowEnd;
            HTuple hv_ColEnd, hv_row, hv_col, hv_L, hv_RowArrow1, hv_ColArrow1;
            HTuple hv_RowArrow2, hv_ColArrow2;

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Contour1);

            try
            {
                ho_Arrow.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Arrow);

                hv_RowBegin = hv_Row + (hv_Length1 * (hv_Phi.TupleSin()));
                hv_ColBegin = hv_Column - (hv_Length1 * (hv_Phi.TupleCos()));
                hv_RowEnd = hv_Row - (hv_Length1 * (hv_Phi.TupleSin()));
                hv_ColEnd = hv_Column + (hv_Length1 * (hv_Phi.TupleCos()));

                hv_row = (0.3 * hv_Row) + (0.7 * hv_RowEnd);
                hv_col = (0.3 * hv_Column) + (0.7 * hv_ColEnd);
                hv_L = 0.3 * hv_Length1;

                hv_RowArrow1 = hv_row - (hv_L * (hv_Phi.TupleCos()));
                hv_ColArrow1 = hv_col - (hv_L * (hv_Phi.TupleSin()));
                hv_RowArrow2 = hv_row + (hv_L * (hv_Phi.TupleCos()));
                hv_ColArrow2 = hv_col + (hv_L * (hv_Phi.TupleSin()));

                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, ((hv_RowBegin.TupleConcat(
                    hv_RowEnd))).TupleConcat(hv_RowArrow1), ((hv_ColBegin.TupleConcat(hv_ColEnd))).TupleConcat(
                    hv_ColArrow1));
                ho_Contour1.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour1, ((hv_RowBegin.TupleConcat(
                    hv_RowEnd))).TupleConcat(hv_RowArrow2), ((hv_ColBegin.TupleConcat(hv_ColEnd))).TupleConcat(
                    hv_ColArrow2));
                OTemp[SP_O] = ho_Arrow.CopyObj(1, -1);
                SP_O++;
                ho_Arrow.Dispose();
                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Contour, out ho_Arrow);
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                OTemp[SP_O] = ho_Arrow.CopyObj(1, -1);
                SP_O++;
                ho_Arrow.Dispose();
                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Contour1, out ho_Arrow);
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;

                ho_Contour.Dispose();
                ho_Contour1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Contour.Dispose();
                ho_Contour1.Dispose();

                throw HDevExpDefaultException;
            }
        }

        /// <summary>
        /// 找圆
        /// </summary>
        /// <param name="ho_Image">图片输入</param>
        /// <param name="ho_object">object输出</param>
        /// <param name="hv_value">参数输入</param>
        /// <param name="hv_outValue">输出信息</param>
        /// <param name="hv_message">显示信息</param>
        /// <param name="hv_error">成功或失败</param>
        public void find_circle(HObject ho_Image, out HObject ho_object, HTuple hv_value,
      out HTuple hv_outValue, out HTuple hv_message, out HTuple hv_error)       //找圆函数
        {

            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_RingContour = null, ho_Arrow = null;
            HObject ho_Arrow1 = null, ho_Arrow2 = null, ho_Arrow3 = null;
            HObject ho_Arrow4 = null, ho_Rectangle = null, ho_Cross1 = null;
            HObject ho_Contour = null, ho_ContCircle = null, ho_Cross = null;


            // Local control variables 

            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Radius = new HTuple(), hv_iThreshold = new HTuple();
            HTuple hv_iSigma = new HTuple(), hv_RoiWidth = new HTuple();
            HTuple hv_iTransition = new HTuple(), hv_iSelect = new HTuple();
            HTuple hv_iDirection = new HTuple(), hv_RingWidth = new HTuple();
            HTuple hv_PointCounts = new HTuple(), hv_angleStart = new HTuple();
            HTuple hv_angleEnd = new HTuple(), hv_level = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_RowCenter = new HTuple();
            HTuple hv_ColCenter = new HTuple(), hv_Pointer = new HTuple();
            HTuple hv_Type = new HTuple(), hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_RowEdges = new HTuple(), hv_ColEdges = new HTuple();
            HTuple hv_i = new HTuple(), hv_MeasureHandle = new HTuple();
            HTuple hv_RowEdge = new HTuple(), hv_ColumnEdge = new HTuple();
            HTuple hv_Amplitude = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_angleRange = new HTuple(), hv_angleOffset = new HTuple();
            HTuple hv_PhiTemp = new HTuple(), hv_RowTemp = new HTuple();
            HTuple hv_ColTemp = new HTuple(), hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Radius1 = new HTuple(), hv_StartPhi = new HTuple();
            HTuple hv_EndPhi = new HTuple(), hv_PointOrder = new HTuple();
            HTuple hv_Exception;

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_object);
            HOperatorSet.GenEmptyObj(out ho_RingContour);
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            HOperatorSet.GenEmptyObj(out ho_Arrow2);
            HOperatorSet.GenEmptyObj(out ho_Arrow3);
            HOperatorSet.GenEmptyObj(out ho_Arrow4);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Cross1);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Cross);

            hv_outValue = new HTuple();
            try
            {
                //Image,输入图片
                //value，输入找圆参数
                //object, 输出显示的图形参数
                //outValue, 输出的控制参数
                //message, 输出显示的消息
                //error, 找圆成功或失败的标志
                //**********************************
                //iTransition, 3个值，0表示“all”， 1表示“positive”， 2表示“negative”
                //iSelect， 3个值， 0表示“all”， 1表示“first”, 2表示“last”
                //iDirection, 2个值， 0表示“正”， 1表示“负”
                //level, 0只显示拟合的圆， 1显示找圆的环形区域， 2显示找点的测量小矩形， 3显示拟合圆所找到的点

                ho_object.Dispose();
                HOperatorSet.GenEmptyObj(out ho_object);
                hv_error = 0;
                hv_message = "";
                try
                {
                    hv_Row = hv_value[0];
                    hv_Column = hv_value[1];
                    hv_Radius = hv_value[2];
                    hv_iThreshold = hv_value[3];
                    hv_iSigma = hv_value[4];
                    hv_RoiWidth = hv_value[5];
                    hv_iTransition = hv_value[6];
                    hv_iSelect = hv_value[7];
                    hv_iDirection = hv_value[8];
                    hv_RingWidth = hv_value[9];
                    hv_PointCounts = hv_value[10];
                    hv_angleStart = hv_value[11];
                    hv_angleEnd = hv_value[12];
                    hv_level = hv_value[13];


                    if ((int)(new HTuple(hv_angleEnd.TupleLess(hv_angleStart))) != 0)
                    {
                        hv_angleEnd = hv_angleEnd + 360;
                    }

                    hv_Phi = new HTuple();
                    hv_RowCenter = new HTuple();
                    hv_ColCenter = new HTuple();

                    HOperatorSet.GetImagePointer1(ho_Image, out hv_Pointer, out hv_Type, out hv_Width,
                        out hv_Height);

                    hv_RowEdges = new HTuple();
                    hv_ColEdges = new HTuple();
                    for (hv_i = 0; hv_i.Continue(hv_PointCounts - 1, 1); hv_i = hv_i.TupleAdd(1))
                    {
                        hv_Phi[hv_i] = (hv_angleStart.TupleRad()) + (hv_i * (((((hv_angleEnd - hv_angleStart) * 1.0) / hv_PointCounts)).TupleRad()
                            ));
                        hv_RowCenter[hv_i] = hv_Row - (hv_Radius * (((hv_Phi.TupleSelect(hv_i))).TupleSin()
                            ));
                        hv_ColCenter[hv_i] = hv_Column + (hv_Radius * (((hv_Phi.TupleSelect(hv_i))).TupleCos()
                            ));
                        if ((int)(new HTuple(hv_iDirection.TupleEqual(1))) != 0)
                        {
                            hv_Phi[hv_i] = (hv_Phi.TupleSelect(hv_i)) + ((new HTuple(180)).TupleRad()
                                );
                        }
                        HOperatorSet.GenMeasureRectangle2(hv_RowCenter.TupleSelect(hv_i), hv_ColCenter.TupleSelect(
                            hv_i), hv_Phi.TupleSelect(hv_i), hv_RingWidth / 2.0, hv_RoiWidth / 2.0,
                            hv_Width, hv_Height, "nearest_neighbor", out hv_MeasureHandle);
                        HOperatorSet.MeasurePos(ho_Image, hv_MeasureHandle, hv_iSigma, hv_iThreshold,
                            hv_iTransition, hv_iSelect, out hv_RowEdge, out hv_ColumnEdge, out hv_Amplitude,
                            out hv_Distance);
                        hv_RowEdges = hv_RowEdges.TupleConcat(hv_RowEdge);
                        hv_ColEdges = hv_ColEdges.TupleConcat(hv_ColumnEdge);
                        HOperatorSet.CloseMeasure(hv_MeasureHandle);
                    }

                    if ((int)(new HTuple(hv_level.TupleEqual(1))) != 0)
                    {
                        hv_angleRange = hv_angleEnd - hv_angleStart;
                        ho_RingContour.Dispose();
                        get_circle_ring_contours(out ho_RingContour, hv_Row, hv_Column, hv_Radius,
                            hv_RingWidth, hv_angleStart, hv_angleEnd);
                        OTemp[SP_O] = ho_object.CopyObj(1, -1);
                        SP_O++;
                        ho_object.Dispose();
                        HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_RingContour, out ho_object);
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;
                        if ((int)(new HTuple(hv_angleRange.TupleEqual(360))) != 0)
                        {
                            ho_Arrow.Dispose();
                            gen_rectangle_arrow_contour(out ho_Arrow, hv_Row, hv_Column + hv_Radius,
                                0 + (hv_iDirection * ((new HTuple(180)).TupleRad())), hv_RingWidth / 2.0,
                                hv_RoiWidth / 2.0);
                            OTemp[SP_O] = ho_object.CopyObj(1, -1);
                            SP_O++;
                            ho_object.Dispose();
                            HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow, out ho_object);
                            OTemp[SP_O - 1].Dispose();
                            SP_O = 0;
                            ho_Arrow1.Dispose();
                            gen_rectangle_arrow_contour(out ho_Arrow1, hv_Row - hv_Radius, hv_Column,
                                ((new HTuple(90)).TupleRad()) + (hv_iDirection * ((new HTuple(180)).TupleRad()
                                )), hv_RingWidth / 2.0, hv_RoiWidth / 2.0);
                            OTemp[SP_O] = ho_object.CopyObj(1, -1);
                            SP_O++;
                            ho_object.Dispose();
                            HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow1, out ho_object);
                            OTemp[SP_O - 1].Dispose();
                            SP_O = 0;
                            ho_Arrow2.Dispose();
                            gen_rectangle_arrow_contour(out ho_Arrow2, hv_Row, hv_Column - hv_Radius,
                                ((new HTuple(180)).TupleRad()) + (hv_iDirection * ((new HTuple(180)).TupleRad()
                                )), hv_RingWidth / 2.0, hv_RoiWidth / 2.0);
                            OTemp[SP_O] = ho_object.CopyObj(1, -1);
                            SP_O++;
                            ho_object.Dispose();
                            HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow2, out ho_object);
                            OTemp[SP_O - 1].Dispose();
                            SP_O = 0;
                            ho_Arrow3.Dispose();
                            gen_rectangle_arrow_contour(out ho_Arrow3, hv_Row + hv_Radius, hv_Column,
                                ((new HTuple(270)).TupleRad()) + (hv_iDirection * ((new HTuple(180)).TupleRad()
                                )), hv_RingWidth / 2.0, hv_RoiWidth / 2.0);
                            OTemp[SP_O] = ho_object.CopyObj(1, -1);
                            SP_O++;
                            ho_object.Dispose();
                            HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow3, out ho_object);
                            OTemp[SP_O - 1].Dispose();
                            SP_O = 0;
                        }
                        if ((int)((new HTuple(hv_angleRange.TupleLess(360))).TupleAnd(new HTuple(hv_angleRange.TupleGreater(
                            180)))) != 0)
                        {
                            hv_angleOffset = hv_angleRange / 4.0;
                            for (hv_i = 1; (int)hv_i <= 3; hv_i = (int)hv_i + 1)
                            {
                                hv_PhiTemp = (hv_angleStart.TupleRad()) + (hv_i * (hv_angleOffset.TupleRad()
                                    ));
                                hv_RowTemp = hv_Row - (hv_Radius * (hv_PhiTemp.TupleSin()));
                                hv_ColTemp = hv_Column + (hv_Radius * (hv_PhiTemp.TupleCos()));
                                ho_Arrow4.Dispose();
                                gen_rectangle_arrow_contour(out ho_Arrow4, hv_RowTemp, hv_ColTemp,
                                    hv_PhiTemp + (hv_iDirection * ((new HTuple(180)).TupleRad())), hv_RingWidth / 2.0,
                                    hv_RoiWidth / 2.0);
                                OTemp[SP_O] = ho_object.CopyObj(1, -1);
                                SP_O++;
                                ho_object.Dispose();
                                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow4, out ho_object);
                                OTemp[SP_O - 1].Dispose();
                                SP_O = 0;
                            }
                        }
                        if ((int)(new HTuple(hv_angleRange.TupleLessEqual(180))) != 0)
                        {
                            hv_angleOffset = hv_angleRange / 3.0;
                            for (hv_i = 1; (int)hv_i <= 2; hv_i = (int)hv_i + 1)
                            {
                                hv_PhiTemp = (hv_angleStart.TupleRad()) + (hv_i * (hv_angleOffset.TupleRad()
                                    ));
                                hv_RowTemp = hv_Row - (hv_Radius * (hv_PhiTemp.TupleSin()));
                                hv_ColTemp = hv_Column + (hv_Radius * (hv_PhiTemp.TupleCos()));
                                ho_Arrow4.Dispose();
                                gen_rectangle_arrow_contour(out ho_Arrow4, hv_RowTemp, hv_ColTemp,
                                    hv_PhiTemp + (hv_iDirection * ((new HTuple(180)).TupleRad())), hv_RingWidth / 2.0,
                                    hv_RoiWidth / 2.0);
                                OTemp[SP_O] = ho_object.CopyObj(1, -1);
                                SP_O++;
                                ho_object.Dispose();
                                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow4, out ho_object);
                                OTemp[SP_O - 1].Dispose();
                                SP_O = 0;
                            }
                        }
                    }

                    if ((int)(new HTuple(hv_level.TupleEqual(2))) != 0)
                    {
                        for (hv_i = 0; hv_i.Continue(hv_PointCounts - 1, 1); hv_i = hv_i.TupleAdd(1))
                        {
                            if ((int)(new HTuple(hv_iDirection.TupleEqual(1))) != 0)
                            {
                                hv_Phi[hv_i] = (hv_Phi.TupleSelect(hv_i)) + ((new HTuple(180)).TupleRad()
                                    );
                            }
                            ho_Rectangle.Dispose();
                            HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowCenter.TupleSelect(
                                hv_i), hv_ColCenter.TupleSelect(hv_i), hv_Phi.TupleSelect(hv_i),
                                hv_RingWidth / 2.0, hv_RoiWidth / 2.0);
                            OTemp[SP_O] = ho_object.CopyObj(1, -1);
                            SP_O++;
                            ho_object.Dispose();
                            HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Rectangle, out ho_object);
                            OTemp[SP_O - 1].Dispose();
                            SP_O = 0;
                        }
                    }

                    if ((int)(new HTuple(hv_level.TupleEqual(3))) != 0)
                    {
                        for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_RowEdges.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                        {
                            ho_Cross1.Dispose();
                            HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_RowEdges.TupleSelect(
                                hv_i), hv_ColEdges.TupleSelect(hv_i), 20, 0.785398);
                            OTemp[SP_O] = ho_object.CopyObj(1, -1);
                            SP_O++;
                            ho_object.Dispose();
                            HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Cross1, out ho_object);
                            OTemp[SP_O - 1].Dispose();
                            SP_O = 0;
                        }
                    }

                    if ((int)(new HTuple((new HTuple(hv_RowEdges.TupleLength())).TupleGreaterEqual(
                        10))) != 0)
                    {
                        ho_Contour.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_RowEdges, hv_ColEdges);
                        HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 3,
                            2, out hv_Row1, out hv_Column1, out hv_Radius1, out hv_StartPhi, out hv_EndPhi,
                            out hv_PointOrder);
                        ho_ContCircle.Dispose();
                        HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_Row1, hv_Column1,
                            hv_Radius1, 0, 6.28318, "positive", 1);
                        OTemp[SP_O] = ho_object.CopyObj(1, -1);
                        SP_O++;
                        ho_object.Dispose();
                        HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_ContCircle, out ho_object);
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row1, hv_Column1, 25,
                            0.785398);
                        OTemp[SP_O] = ho_object.CopyObj(1, -1);
                        SP_O++;
                        ho_object.Dispose();
                        HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Cross, out ho_object);
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;
                        hv_outValue = new HTuple();
                        hv_outValue = hv_outValue.TupleConcat(hv_Row1);
                        hv_outValue = hv_outValue.TupleConcat(hv_Column1);
                        hv_outValue = hv_outValue.TupleConcat(hv_Radius1);
                        hv_message = new HTuple();
                        hv_message = hv_message.TupleConcat("Row = " + hv_Row1);
                        hv_message = hv_message.TupleConcat("Col = " + hv_Column1);
                        hv_message = hv_message.TupleConcat("Radius = " + hv_Radius1);
                    }
                    else
                    {
                        hv_message = "找圆失败，点数不足";
                        hv_error = hv_error + 1;
                        ho_RingContour.Dispose();
                        ho_Arrow.Dispose();
                        ho_Arrow1.Dispose();
                        ho_Arrow2.Dispose();
                        ho_Arrow3.Dispose();
                        ho_Arrow4.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Cross1.Dispose();
                        ho_Contour.Dispose();
                        ho_ContCircle.Dispose();
                        ho_Cross.Dispose();

                        return;
                    }

                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_message = "找圆失败:" + (hv_Exception.TupleSelect(4));
                    hv_error = hv_error + 1;
                    ho_RingContour.Dispose();
                    ho_Arrow.Dispose();
                    ho_Arrow1.Dispose();
                    ho_Arrow2.Dispose();
                    ho_Arrow3.Dispose();
                    ho_Arrow4.Dispose();
                    ho_Rectangle.Dispose();
                    ho_Cross1.Dispose();
                    ho_Contour.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Cross.Dispose();

                    return;
                }


            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_RingContour.Dispose();
                ho_Arrow.Dispose();
                ho_Arrow1.Dispose();
                ho_Arrow2.Dispose();
                ho_Arrow3.Dispose();
                ho_Arrow4.Dispose();
                ho_Rectangle.Dispose();
                ho_Cross1.Dispose();
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Cross.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
      HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {


            // Local control variables 

            HTuple hv_Red, hv_Green, hv_Blue, hv_Row1Part;
            HTuple hv_Column1Part, hv_Row2Part, hv_Column2Part, hv_RowWin;
            HTuple hv_ColumnWin, hv_WidthWin, hv_HeightWin, hv_MaxAscent;
            HTuple hv_MaxDescent, hv_MaxWidth, hv_MaxHeight, hv_R1 = new HTuple();
            HTuple hv_C1 = new HTuple(), hv_FactorRow = new HTuple(), hv_FactorColumn = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Index = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_W = new HTuple(), hv_H = new HTuple();
            HTuple hv_FrameHeight = new HTuple(), hv_FrameWidth = new HTuple();
            HTuple hv_R2 = new HTuple(), hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_CurrentColor = new HTuple();

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
            //Box: If set to 'true', the text is written within a white box.
            //
            //prepare window
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
                //transform image to window coordinates
                hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
            }
            //
            //display text box depending on text size
            if ((int)(new HTuple(hv_Box.TupleEqual("true"))) != 0)
            {
                //calculate box extents
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
                //display rectangles
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                HOperatorSet.SetColor(hv_WindowHandle, "light gray");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 3, hv_C1 + 3, hv_R2 + 3, hv_C2 + 3);
                HOperatorSet.SetColor(hv_WindowHandle, "white");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            else if ((int)(new HTuple(hv_Box.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Box";
                throw new HalconException(hv_Exception);
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
            //reset changed window settings
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);

            return;
        }
	}
}
