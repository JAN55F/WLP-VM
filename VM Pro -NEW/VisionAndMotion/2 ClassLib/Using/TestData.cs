using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HalconDotNet;
namespace HalconTool
{
   public class TestData//序列化类
    {
        public string ModelPath = "Parameter\\model.shm";//记录模板路径
        public string ModelImagePath = "Parameter\\model.bmp";//记录模板图路径

        [XmlIgnore] public  HObject modelImage = null;//记录模板图
        public  double[] modelCenter = new double[2] { 0, 0 };//记录模板中心值[row,col]顺序
        [XmlIgnore]        public  HTuple mModelID = null;//模板句柄

        [XmlIgnore] public  HTuple mHomMat = null;//放射变化
         public List<Rectangle2> HobjCheck = new List<Rectangle2>();//要检测区域
        public class Rectangle2//如果id=0表示没有实例化
        {
            public int id;
            public double row;
            public double col;
            public double phi;
            public double length1;
            public double length2;
        }
        public struct mLineCirCle//定义找直线圆结构体
        {
            public int serial;//序号
            public string TypeName;//类型
            public double row1;
            public double col1;
            public double row2;
            public double col2;
            public double angleStart;
            public double angleExt;//这里不是结束角，实际是扩展角度
            public int Num;//数量
            public int Length;//高
            public int Width;//宽
            public int YuZhi;//阈值大小
            public int PinHua;//平滑系数
            public string JiXing;//极性
            public string PointSelect;//点选择，是first,end,all等
            
        }
        public List<mLineCirCle> mFindLineCircle = new List<mLineCirCle>();//记录要测试的线
    }
}
