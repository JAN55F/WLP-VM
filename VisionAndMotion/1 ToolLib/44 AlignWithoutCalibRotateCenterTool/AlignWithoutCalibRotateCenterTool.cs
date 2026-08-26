using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VMPro
{
    [Serializable]
    internal class AlignWithoutCalibRotateCenterTool : ToolBase
    {
        internal AlignWithoutCalibRotateCenterTool()
        {
        }

        internal XY featurePosBeforeRotate = new XY();
        internal XY featurePosAfterRotate = new XY();
        internal double RotateAngle = 0;
        internal double StDx = 0;
        internal double StDy = 0;
        internal double CDx = 0;
        internal double CDy = 0;
        internal XYU templateFeaturePos = new XYU();
        internal XYU templatePickPos = new XYU();
        /// <summary>
        /// 本次定位输入坐标
        /// </summary>
        internal List<XYU> inputPos = new List<XYU>();
        /// <summary>
        /// 计算出来的本次定位最终机械手取料坐标
        /// </summary>
        internal XYU resultPos = new XYU();
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
                var res = new XYU();
                res.Point.X = rotateCenter.Point.X + (curPos.Point.X - rotateCenter.Point.X) * Math.Cos(rad) - (curPos.Point.Y - rotateCenter.Point.Y) * Math.Sin(rad);
                res.Point.Y = rotateCenter.Point.Y + (curPos.Point.X - rotateCenter.Point.X) * Math.Sin(rad) + (curPos.Point.Y - rotateCenter.Point.Y) * Math.Cos(rad);
                res.U = curPos.U + rotateAngle;

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


                    XYU temp11 = inputPos[0] - templateFeaturePos;
                    double a = (temp11.U);


                    double temp111 = (Math.Cos(a) - 1) * StDx;
                    double temp222 = Math.Sin(a) * StDy;
                    double temp333 = temp11.Point.X;

                    CDx = (Math.Cos(a) - 1) * StDx - Math.Sin(a) * StDy + temp11.Point.X;
                    CDy = (Math.Cos(a) - 1) * StDy + Math.Sin(a) * StDx + temp11.Point.Y;
                    double CDu = a * 180 / Math.PI;

                    resultPos.Point.X = CDx + templatePickPos.Point.X;
                    resultPos.Point.Y = CDy + templatePickPos.Point.Y;
                    resultPos.U = CDu + templatePickPos.U;



                    Frm_AlignWithoutCalibRotateCenterTool.Instance.tbx_pickPosX.Text = inputPos[0].Point.X.ToString();
                    Frm_AlignWithoutCalibRotateCenterTool.Instance.tbx_pickPosY.Text = inputPos[0].Point.Y.ToString();
                    Frm_AlignWithoutCalibRotateCenterTool.Instance.tbx_pickPosU.Text = inputPos[0].U.ToString();

                    Frm_AlignWithoutCalibRotateCenterTool.Instance.textBox6.Text = CDx.ToString();
                    Frm_AlignWithoutCalibRotateCenterTool.Instance.textBox7.Text = CDy.ToString();
                    Frm_AlignWithoutCalibRotateCenterTool.Instance.textBox20.Text = CDu.ToString();



                    Frm_AlignWithoutCalibRotateCenterTool.Instance.textBox8.Text = resultPos.Point.X.ToString();
                    Frm_AlignWithoutCalibRotateCenterTool.Instance.textBox11.Text = resultPos.Point.Y.ToString();
                    Frm_AlignWithoutCalibRotateCenterTool.Instance.textBox21.Text = resultPos.U.ToString();

                    //安全管控
                    //////XYU offset = inputPos - L_featurePos[toolIdx];
                    //////if (Math.Abs(offset.Point.X) > L_safetyRange[toolIdx].Point.X)
                    //////{
                    //////    toolRunStatu = ToolRunStatu.定位结果X值超限;
                    //////    return;
                    //////}
                    //////else if (Math.Abs(offset.Point.Y) > L_safetyRange[toolIdx].Point.Y)
                    //////{
                    //////    toolRunStatu = ToolRunStatu.定位结果Y值超限;
                    //////    return;
                    //////}
                    //////else if (Math.Abs(offset.U) * 180 / Math.PI > L_safetyRange[toolIdx].U)
                    //////{
                    //////    toolRunStatu = ToolRunStatu.定位结果U值超限;
                    //////    return;
                    //////}

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
