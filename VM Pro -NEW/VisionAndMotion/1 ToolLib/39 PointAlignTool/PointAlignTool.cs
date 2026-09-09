using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using VersionMethods;
using System.Threading;

namespace VMPro
{
    [Serializable]
    internal class PointAlignTool : ToolBase
    {
        internal PointAlignTool()
        {
            //新建工具后，默认添加一个工具
            L_toolName.Add("工具1");

            XYU featurePos = new XYU();
            L_featurePos.Add(featurePos);

            XYU pickPos = new XYU();
            L_workPos.Add(pickPos);

            XYU pickPosOffset = new XYU();
            L_workPosOffset.Add(pickPosOffset);

            XYU safetyRange = new XYU();
            L_safetyRange.Add(safetyRange);
        }

        /// <summary>
        /// 工具名称集合
        /// </summary>
        internal List<string> L_toolName = new List<string>();
        /// <summary>
        /// 机械手上所安装的工具的编号
        /// </summary>
        internal int toolIdx = 0;
        /// <summary>
        /// 制作模板时特征点坐标
        /// </summary>
        internal List<XYU> L_featurePos = new List<XYU>();
        /// <summary>
        /// 制作模板时的工作点坐标
        /// </summary>
        internal List<XYU> L_workPos = new List<XYU>();
        /// <summary>
        /// 本次定位输入坐标
        /// </summary>
        internal XYU inputPos = new XYU();
        /// <summary>
        /// 计算出来的本次定位最终机械手取料坐标
        /// </summary>
        internal XYU resultPos = new XYU();
        /// <summary>
        /// 模板取料位置补偿值
        /// </summary>
        internal List<XYU> L_workPosOffset = new List<XYU>();
        /// <summary>
        /// 安全范围
        /// </summary>
        internal List<XYU> L_safetyRange = new List<XYU>();
        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();


        internal void ClearLastInput()
        {
            try
            {
                inputPos = null;
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
                var res = new XYU()   ;
                  res.  Point.X = rotateCenter.Point.X + (curPos.Point.X - rotateCenter.Point.X) * Math.Cos(rad) - (curPos.Point.Y - rotateCenter.Point.Y) * Math.Sin(rad);
               res.     Point.Y = rotateCenter.Point.Y + (curPos.Point.X - rotateCenter.Point.X) * Math.Sin(rad) + (curPos.Point.Y - rotateCenter.Point.Y) * Math.Cos(rad);
               res .U = curPos.U + rotateAngle;
               
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
                    if (inputPos == null)
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Pos : ToolRunStatu.未指定输入坐标点);
                        return;
                    }

                    //首先把所示教的模板工作点绕被模板特征点旋转一个角度，使角度与本次定位的角度相同
                    double offsetU = inputPos.U - L_featurePos[toolIdx].U;
                    offsetU = offsetU * 180 / Math.PI;
                    XYU templateWorkPosAfterRotate = RotateAt(L_workPos[toolIdx]+L_workPosOffset[toolIdx ], L_featurePos [toolIdx ], -offsetU);

                    //计算本次定位特征点和模板特征点的偏差
                    double offsetX = templateWorkPosAfterRotate.Point.X - L_featurePos[toolIdx].Point.X;
                    double offsetY = templateWorkPosAfterRotate.Point.Y - L_featurePos[toolIdx].Point.Y; 

                    //机械手再平移这些量
                    XYU templateWorkPosAfterRotateUAndMoveXY = new XYU();
                    templateWorkPosAfterRotateUAndMoveXY.Point.X = inputPos.Point.X + offsetX;
                    templateWorkPosAfterRotateUAndMoveXY.Point.Y = inputPos.Point.Y + offsetY;
                    templateWorkPosAfterRotateUAndMoveXY.U = inputPos .U ; 

                    resultPos = templateWorkPosAfterRotateUAndMoveXY;

                    Frm_PointAlignTool.Instance.tbx_inputPosX.Text = inputPos.Point.X.ToString();
                    Frm_PointAlignTool.Instance.tbx_inputPosY.Text = inputPos.Point.Y.ToString();
                    Frm_PointAlignTool.Instance.tbx_inputPosU.Text = inputPos.U.ToString();

                    Frm_PointAlignTool.Instance.tbx_resultPosX.Text = resultPos.Point.X.ToString();
                    Frm_PointAlignTool.Instance.tbx_resultPosY.Text = resultPos.Point.Y.ToString();

                    //安全管控
                    XYU offset = inputPos - L_featurePos[toolIdx];
                    if (Math.Abs(offset.Point.X) > L_safetyRange[toolIdx].Point.X)
                    {
                        toolRunStatu = ToolRunStatu.定位结果X值超限;
                        return;
                    }
                    else if (Math.Abs(offset.Point.Y) > L_safetyRange[toolIdx].Point.Y)
                    {
                        toolRunStatu = ToolRunStatu.定位结果Y值超限;
                        return;
                    }
                    else if (Math.Abs(offset.U) * 180 / Math.PI > L_safetyRange[toolIdx].U)
                    {
                        toolRunStatu = ToolRunStatu.定位结果U值超限;
                        return;
                    }

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
