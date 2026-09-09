using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using ViewWindow.Model;
using HalconTool;
using System.Drawing;
using System.Windows;

namespace VMPro
{
    [Serializable]
    internal class RotatePlatformTool : ToolBase
    {

        /// <summary>
        /// 求旋转中心用的流程名称
        /// </summary>
        internal string calibJobName = string.Empty;
        /// <summary>
        /// 求旋转中心用的输出项的名称
        /// </summary>
        internal string calibItemName = string.Empty;
        /// <summary>
        /// 标定数据
        /// </summary>
        internal List<List<double>> L_calibData = new List<List<double>>();
        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 旋转中心
        /// </summary>
        internal XYU rotateCenter = new XYU();
        /// <summary>
        /// 输入位置
        /// </summary>
        internal XYU inputPos = new XYU();
        /// <summary>
        /// 输出位置
        /// </summary>
        internal XYU outputPos = new XYU();


        internal void ClearLastInput()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 多点拟合圆
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
        /// <summary>
        /// 计算旋转中心
        /// </summary>
        internal void CalculateRotateCenter()
        {
            try
            {
                List<PointF> L_points = new List<PointF>();
                for (int i = 0; i < Frm_RotatePlatformTool.Instance.dgv_data.Rows.Count; i++)
                {
                    PointF pointf = new PointF((float)Convert.ToDouble(Frm_RotatePlatformTool.Instance.dgv_data.Rows[i].Cells[1].Value), (float)Convert.ToDouble(Frm_RotatePlatformTool.Instance.dgv_data.Rows[i].Cells[2].Value));
                    L_points.Add(pointf);
                }
                PointF result = FitCenter(L_points);
                rotateCenter.Point.X = result.X;
                rotateCenter.Point.Y = result.Y;
                Frm_RotatePlatformTool.Instance.tbx_rotateCenterX.Text = result.X.ToString();
                Frm_RotatePlatformTool.Instance.tbx_rotateCenterY.Text = result.Y.ToString();

                L_calibData.Clear();
                for (int i = 0; i < Frm_RotatePlatformTool.Instance.dgv_data.Rows.Count; i++)
                {
                    List<double> list = new List<double>();
                    for (int j = 0; j < 3; j++)
                    {
                        list.Add(Math.Round(Convert.ToDouble(Frm_RotatePlatformTool.Instance.dgv_data.Rows[i].Cells[j].Value), 3));
                    }
                    L_calibData.Add(list);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绕点旋转
        /// </summary>
        /// <param name="curPos">当前被旋转的点</param>
        /// <param name="rotateCenter">旋转中心</param>
        /// <param name="rotateAngle">旋转角度</param>
        /// <returns></returns>
        internal XYU RotateAt(XYU curPos, XYU rotateCenter, double rotateAngle)
        {
            try
            {
                double rad = rotateAngle * Math.PI / 180;
                var res = new XYU();
                  res.  Point.X = rotateCenter.Point.X + (curPos.Point.X - rotateCenter.Point.X) * Math.Cos(rad) - (curPos .Point .Y - rotateCenter.Point.Y) * Math.Sin(rad);
                  res . Point.Y = rotateCenter.Point.Y + (curPos.Point.X - rotateCenter.Point.X) * Math.Sin(rad) + (curPos.Point.Y - rotateCenter.Point.Y) * Math.Cos(rad);
                  res . U = curPos.U + rotateAngle ;
              
                if (res.U < -180)
                {
                    res.U += 360;
                }
                else if (res.U >= 180)
                {
                    res.U -= 360;
                }
                return res;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new XYU();
            }
        }
        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    double angle = inputPos.U * 180 / Math.PI;
                    outputPos = RotateAt(inputPos, rotateCenter, -angle);
                    outputPos.U = angle ;
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
