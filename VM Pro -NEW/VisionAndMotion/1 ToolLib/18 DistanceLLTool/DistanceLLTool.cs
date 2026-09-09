using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMPro
{
    [Serializable]
    internal class DistanceLLTool : ToolBase
    {

        /// <summary>
        /// 输入的第一条线段
        /// </summary>
        internal Line line1;
        /// <summary>
        /// 输入的第二条线段
        /// </summary>
        internal Line line2;
        /// <summary>
        /// 计算出来的线与线之间的距离值
        /// </summary>
        private double _resultDistance;
        internal double ResultDistance
        {
            get
            {
                _resultDistance = Math.Round(_resultDistance, 3);
                return _resultDistance;
            }
            set { _resultDistance = value; }
        }


        /// <summary>
        /// 获取线段与线段之间的距离
        /// </summary>
        /// <param name="line1">线段1</param>
        /// <param name="line2">线段2</param>
        /// <returns>距离值</returns>
        public static double DistanceLineToLine(Line line1, Line line2)
        {
            try
            {
                double distance;
                double x1 = line1.起点.X ;
                double y1 = line1.起点.Y ;
                double x2 = line1.终点.X ; //B点坐标（x2,y2,z2）   
                double y2 = line1.终点.Y ;
                double x3 = line2.起点.X ; //C点坐标（x3,y3,z3）   
                double y3 = line2.起点.Y ;
                double x4 = line2.终点.X ; //D点坐标（x4,y4,z4）   
                double y4 = line2.终点.Y ;
                XY p1 = new XY() { X  = x1, Y = y1 };
                XY p2 = new XY() { X = x2, Y = y2 };
                XY p3 = new XY() { X = x3, Y = y3 };
                XY p4 = new XY() { X = x4, Y = y4 };
                double a = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
                double b = -((x2 - x1) * (x4 - x3) + (y2 - y1) * (y4 - y3));
                double c = -((x1 - x2) * (x1 - x3) + (y1 - y2) * (y1 - y3));

                double d = -((x2 - x1) * (x4 - x3) + (y2 - y1) * (y4 - y3));
                double e = (x4 - x3) * (x4 - x3) + (y4 - y3) * (y4 - y3);
                double f = -((x1 - x3) * (x4 - x3) + (y1 - y3) * (y4 - y3));

                if ((a * e - b * d) == 0 && (b * d - a * e) == 0) //平行   
                {
                    double d1 = (p1 - p3).GetDistance;
                    double d2 = (p1 - p4).GetDistance;
                    distance = (d1 < d2) ? d1 : d2;
                    return distance;
                }

                double s = (b * f - e * c) / (a * e - b * d);
                double t = (a * f - d * c) / (b * d - a * e);

                if (0 <= s && s <= 1 && 0 <= t && t <= 1) //说明P点落在线段AB上,Q点落在线段CD上   
                {
                    //2条线段的公垂线段PQ;   
                    //P点坐标   
                    double X = x1 + s * (x2 - x1);
                    double Y = y1 + s * (y2 - y1);
                    //Q点坐标   
                    double U = x3 + t * (x4 - x3);
                    double V = y3 + t * (y4 - y3);
                    XY P = new XY(X, Y);
                    XY Q = new XY(U, V);
                    distance = (P - Q).GetDistance;
                }
                else
                {
                    double d1 = DistancePointToSegment(p1, p3, p4);
                    double d2 = DistancePointToSegment(p2, p3, p4);
                    double d3 = DistancePointToSegment(p3, p1, p2);
                    double d4 = DistancePointToSegment(p4, p1, p2);
                    distance = (d1 < d2) ? d1 : d2;
                    distance = (distance < d3) ? distance : d3;
                    distance = (distance < d4) ? distance : d4;
                }
                return distance;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return -1;
            }
        }

        /// <summary>
        /// 点到线段的距离
        /// </summary>
        /// <param name="p">点</param>
        /// <param name="pStart">线段的起点</param>
        /// <param name="pEnd">线段的终点</param>
        /// <returns>返回的距离</returns>
        public static double DistancePointToSegment(XY p, XY pStart, XY pEnd)
        {
            try
            {
                double px = (pEnd - pStart).X;
                double py = (pEnd - pStart).Y;
                double som = px * px + py * py;
                double u = ((p.X - pStart.X) * px + (p.Y - pStart.Y) * py) / som;
                if (u > 1)
                {
                    u = 1;
                }
                if (u < 0)
                {
                    u = 0;
                }
                //the closest point
                double x = pStart.X + u * px;
                double y = pStart.Y + u * py;
                double dx = x - p.X;
                double dy = y - p.Y;
                double dist = Math.Sqrt(dx * dx + dy * dy);

                return dist;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return -1;
            }
        }
        private object obj = new object();
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="jobName">流程名</param>
        public override void Run(bool updateImage, bool b, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    ResultDistance = DistanceLineToLine(line1, line2);
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
