using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMPro
{

    /// <summary>
    /// 工具类型
    /// </summary>
    public enum ToolType
    {
        None,
        SDK_Halcon,
        ImageAcq,
        Match,
        EyeHandCalib,
        OneKeyEyeHandCalib,
        OneDimensionalCalib,
        CircleCalibration,
        SubImage,
        BlobAnalyse,
        FindLine,
        FindCircle,
        CreateROI,
        ArrayRegion,
        Mark,
        CreatePosition,
        OCR,
        Barcode,
        RegionFeature,
        RegionOperation,
        QRCode,
        Scaner_Kenyence,
        KeyenceScanner1,
        KeyenceScanner2,
        Light_OPT,
        OPTLightControl,
        UpCamAlign,
        DownCamAlign,
        RotatePlatform,
        ColorToRGB,
        DistancePL,
        DistanceSS,
        LLIntersect,
        CodeEdit,
        Label,
        Output,
        CreateLine,
        CenterOfPP,
        BuChang,
        batteryFirstAlign,
        XYPlatform,
        SaveImage,
        ToStr,
        PointAlign,
        PointOffset,
        AlignFit,
        QuoteTrans,
        ImagePreprocessing,
        AlignWithoutCalibRotateCenter,
        DisplayEdit,
        DistancePP,
        EthernetReceive,
        EthernetSend,
        DataAnalyse,
        Measurement,
        AngleLL,
        PLCComm,
    }
    [Serializable]
    internal class GlobelVariable
    {
        internal List<Variable> L_variable = new List<Variable>();
        internal object GetGlobalVariableValue(string name)
        {
            for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
            {
                if (Project.Instance.curEngine.globelVariable.L_variable[i].name == name)
                    return Project.Instance.curEngine.globelVariable.L_variable[i].value;
            }
            return null;
        }
        internal void SetGlobalVariableValue(string name, object value)
        {
            for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
            {
                if (Project.Instance.curEngine.globelVariable.L_variable[i].name == name)
                    Project.Instance.curEngine.globelVariable.L_variable[i].value = value;
            }
        }


    }
    [Serializable]
    public class ToolParBase
    {

    }
    internal enum  MatchMode
{
        BasedShape,
        BasedGray,
}
    [Serializable]
    internal class Variable
    {
        internal Variable(int index, string type, string name)
        {
            this.index = index;
            this.type = type;
            this.name = name;
        }
        internal int index = 0;
        internal string type = "Int";
        internal string name = "Value";
        internal object value = 0;
        internal string info = string.Empty;
        internal int variableType = 1;        //0表示系统变量     1表示自定义变量
    }
    /// <summary>
    /// 工具的输入输出类
    /// </summary>
    [Serializable]
    public class ToolIO
    {
        public ToolIO() { }
        public ToolIO(string IOName1, object value1, DataType ioType1)
        {
            this.IOName = IOName1;
            this.value = value1;
            this.ioType = ioType1;
        }

        public string IOName;
        public object value;
        public DataType ioType;
    }

    /// <summary>
    /// 字符类型   白纸黑字|黑纸白字
    /// </summary>
    internal enum CharType
    {
        BlackChar,
        WhiteChar,
    }

    [Serializable]
    internal struct Label
    {
        internal string ValueType;
        internal string ExpectValue;
        internal string OutputItem;
        internal string PreAddStr;
        internal string Row;
        internal string Col;
        internal string Incolor;
        internal string Size;
        internal string DownLimit;
        internal string UpLimit;
        internal string OutColor;
    }
    public enum JobRunMode
    {
        RunAfterCall,
        LoopRunAfterStart,
    }
    public enum JobRunStatu
    {
        Succeed,
        Fail,
    }

    public enum SortMode
    {
        从上至下且从左至右,
        从左至右且从上至下,
        从上至下且从右至左,
        从左至右且从下至上,
    }

    public enum DataType
    {
        String,
        Region,
        Image,
        XY,
        Line,
        Circle,
        Pose,
    }
    //public enum DeviceType
    //{
    //    LightController_OPT,
    //    TCPIPSever,
    //    TCPIPClient,
    //}
    public enum TipType
    {
        Tip,
        Warn,
        Error,
    }
    public enum PLCBrand
    {
        Omron,
        Panasonic,
        Mitsubishi,
        Siemens,
        AB,
        Inovance,
    }

    /// <summary>
    /// 运动控制卡类型 固高GTS系列|雷赛IOC0640系列
    /// </summary>
    public enum CardType
    {
        无,
        固高_GTS,
        雷赛_IOC0640,
        雷塞_DMC2210,
        雷塞_DMC2410,
        凌华_AMP204C,
        联赢_WMX,
        安川_MP3100,
    }

    /// <summary>
    /// 填充模式 填充|轮廓
    /// </summary>
    internal enum FillMode
    {
        Fill,
        Margin,
    }

    /// <summary>
    /// 区域类型
    /// </summary>
    internal enum RegionType
    {
        AllImage,
        Rectangle1,
        Rectangle2,
        Circle,
        Ellipse,
        MultPoint,
        Ring,
        Any,
        整幅图像,
        矩形,
        仿射矩形,
        圆,
        多点,
        椭圆,
        圆环,
        任意,
        InputRegion,        //此区域类型指来自于输入的区域
    }

    /// <summary>
    /// 语言 中文|英文
    /// </summary>
    public enum Language
    {
        Chinese,
        English,
    }

    /// <summary>
    /// 标定类型 四点|九点
    /// </summary>
    internal enum CalibType
    {
        Four_Point,
        Nine_Point,
    }

    /// <summary>
    /// 相机安装类型 眼在手外|眼在手上
    /// </summary>
    internal enum FixedType
    {
        OutsideHand,
        OnHand,
    }

    public enum CommunicationType
    {
        None,
        Internet_Client,
        Internet_Sever,
        SerialPort,
        IO,
    }

    [Serializable]
    public class Line
    {
        public Line()
        {
            _起点 = new XY();
            _终点 = new XY();
        }

        private XY _起点;

        public XY 起点
        {
            get { return _起点; }
            set { _起点 = value; }
        }
        private XY _终点;

        public XY 终点
        {
            get { return _终点; }
            set { _终点 = value; }
        }

        internal string ToShowTip()
        {
            return 起点.X.ToString() + " | " + 起点.Y.ToString() + " | " + 终点.X.ToString() + " | " + 终点.Y.ToString();
        }
        private HTuple _方向;
        public double 方向
        {
            get
            {
                HOperatorSet.AngleLx(起点.X, 起点.Y, 终点.X, 终点.Y, out _方向);
                return Math.Round(_方向.D , 3);
            }
        }
        public double GetAngle()
        {
            HTuple angle;
            HOperatorSet.AngleLx(起点.X, 起点.Y, 终点.X, 终点.Y, out angle);
            return angle;
        }
    }

    /// <summary>
    /// 采集设备
    /// </summary>
    [Serializable]
    internal class AcquistionDevice
    {
        internal HTuple Handle;
        internal string DeviceStr;
        internal string InterfaceType;
        internal string DeviceDescriptionStr;
        internal double Exposure;
        internal int MinExposure;
        internal int MaxExposure;
    }

    /// <summary>
    /// 条码识别工具结果结构
    /// </summary>
    [Serializable]
    internal struct RunResult
    {
        internal string ResultString;
        internal string BarcodeType;
        internal HObject Region;
        internal double Row;
        internal double Col;
        internal double Angle;
    }

    /// <summary>
    /// 通讯配置项
    /// </summary>
    [Serializable]
    internal struct CommConfigItem
    {
        internal string ReceivedCommand;
        internal string JobName;
        internal string OutputItem;
        internal string NGRespond;
        internal string PrefixStr;
        internal string suffixStr;
    }

    /// <summary>
    /// 模板匹配结果
    /// </summary>
    [Serializable]
    internal struct MatchResult
    {
        internal double Row;
        internal double Col;
        internal double Angle;
        internal double Socre;
    }

    /// <summary>
    /// 斑点分析工具结果
    /// </summary>
    [Serializable]
    internal struct BlobResult
    {
        internal double Row;
        internal double Col;
        internal double Area;
        internal double CircumcircleRadius;
        internal HObject region;
    }

    /// <summary>
    /// 预处理项
    /// </summary>
    [Serializable]
    internal class PreProcessing
    {
        internal string PreProcessingType;
        internal string ElementType;
        internal Int32 ElementSize;
        internal Int32 MinArea;
        internal Int32 MaxArea;
        internal bool Enable;
    }

    /// <summary>
    /// XYU结果
    /// </summary>
    [Serializable]
    public class XYU
    {
        internal XYU()
        {
            _point = new XY();
        }
        internal XYU(double x, double y, double u)
        {
            _point = new XY();
            _point.X = x;
            _point.Y = y;
            _u = u;
        }
        private XY _point;

        public XY Point
        {
            get { return _point; }
            set { _point = value; }
        }
        private double _u;
        public double U
        {
            get
            {
                return Math.Round(_u, 3);
            }
            set { _u = value; }
        }
        /// <summary>
        /// 将XYU类型转化成格式化字符串
        /// </summary>
        /// <returns></returns>
        internal string ToFormatStr()
        {
            return (Point.X >= 0 ? "+" + Point.X.ToString("000.000") : Point.X.ToString("000.000")) + (Point.Y >= 0 ? "+" + Point.Y.ToString("000.000") : Point.Y.ToString("000.000")) + (U >= 0 ? "+" + U.ToString("000.000") : U.ToString("000.000"));

            //  return (X >= 0 ? "+" + X.ToString("000.000") : X.ToString("000.000")) + ";" + (Y >= 0 ? "+" + Y.ToString("000.000") : Y.ToString("000.000")) + ";" + (U >= 0 ? "+" + U.ToString("000.000") : U.ToString("000.000"));
        }
        /// <summary>
        /// 将XYU类型转化成格式化字符串
        /// </summary>
        /// <returns></returns>
        internal string ToFormatStrTwoSpace()
        {
            return (Point.X >= 0 ? "+" + Point.X.ToString("000.000") : Point.X.ToString("000.000")) + (Point.Y >= 0 ? "  +" + Point.Y.ToString("000.000") : Point.Y.ToString("000.000")) + (U >= 0 ? "  +" + U.ToString("000.000") : U.ToString("000.000"));

            //  return (X >= 0 ? "+" + X.ToString("000.000") : X.ToString("000.000")) + ";" + (Y >= 0 ? "+" + Y.ToString("000.000") : Y.ToString("000.000")) + ";" + (U >= 0 ? "+" + U.ToString("000.000") : U.ToString("000.000"));
        }
        internal string ToShowTip()
        {
            return Point.X.ToString() + " | " + Point.Y.ToString() + " | " + U.ToString();
        }
        /// <summary>
        /// 重写 -
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <returns></returns>
        public static XYU operator -(XYU p1, XYU p2)
        {
            return new XYU(p1.Point.X - p2.Point.X, p1.Point.Y - p2.Point.Y, p1.U - p2.U);
        }
        /// <summary>
        /// 重写 +
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <returns></returns>
        public static XYU operator +(XYU p1, XYU p2)
        {
            return new XYU(p1.Point.X + p2.Point.X, p1.Point.Y + p2.Point.Y, p1.U + p2.U);
        }
    }

    /// <summary>
    /// 阈值分割后的筛选项
    /// </summary>
    [Serializable]
    internal struct SelectItem
    {
        internal string SelectType;
        internal Int64 AreaUpLimit;
        internal Int64 AreaDownLimit;
    }

    [Serializable]
    public class XY
    {
        internal XY() { }
        internal XY(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        private double _x;

        public double X
        {
            get
            {
                return Math.Round(_x, 3);
            }
            set { _x = value; }
        }
        private double _y;

        public double Y
        {
            get
            {
                return Math.Round(_y, 3);
            }
            set { _y = value; }
        }
        /// <summary>
        /// 重写 -
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <returns></returns>
        public static XY operator -(XY p1, XY p2)
        {
            return new XY(p1.X - p2.X, p1.Y - p2.Y);
        }
        /// <summary>
        /// 重写 +
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <returns></returns>
        public static XY operator +(XY p1, XY p2)
        {
            return new XY(p1.X + p2.X, p1.Y + p2.Y);
        }
        /// <summary>
        /// 获得点矢量长度
        /// </summary>
        internal double GetDistance
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }
        internal string ToShowTip()
        {
            return X.ToString() + " | " + Y.ToString();
        }
    }

}
