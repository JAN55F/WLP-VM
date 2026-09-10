using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VMPro
{
    [Serializable]
    internal class UpCamAlignTool : ToolBase
    {
        internal UpCamAlignTool()
        {
            //新建工具后，默认添加一个工具
            L_toolName.Add("工具1");

            XYU featurePos = new XYU();
            L_featurePos.Add(featurePos);

            XYU pickPos = new XYU();
            L_pickPos.Add(pickPos);

            XYU pickPosOffset = new XYU();
            L_pickPosOffset.Add(pickPosOffset);

            XYU safetyRange = new XYU();
            safetyRange.Point.X = 3;
            safetyRange.Point.Y = 3;
            safetyRange.U = 3;
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
        /// 制作模板时示教的取料位置坐标
        /// </summary>
        internal List<XYU> L_pickPos = new List<XYU>();
        /// <summary>
        /// 模板取料位置补偿值
        /// </summary>
        internal List<XYU> L_pickPosOffset = new List<XYU>();
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
                toolPar.InputPar.位置 = null;
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
                    if (toolPar.InputPar.位置 == null)
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Pos : ToolRunStatu.未指定输入坐标点);
                        return;
                    }

                    //首先旋转使角度重合,然后将机械手平移，计算出使本次定位产品和创建模板时的产品重合时机械手的坐标，即取料坐标
                    double offsetU = toolPar.InputPar.位置.U - L_featurePos[toolIdx].U;
                    offsetU = offsetU * 180 / Math.PI;
                    double robotPosAfterRotateU = ((L_pickPos[toolIdx] + L_pickPosOffset[toolIdx]).U) + offsetU;

                    //计算旋转之后模板特征点的XY坐标
                    XYU featurePosAfterRotate = RotateAt(L_featurePos[toolIdx], L_pickPos[toolIdx] + L_pickPosOffset[toolIdx], offsetU);

                    //计算经过旋转的特征点坐标和创建模板时的机械坐标的平移量
                    double offsetX = toolPar.InputPar.位置.Point.X - featurePosAfterRotate.Point.X;
                    double offsetY = toolPar.InputPar.位置.Point.Y - featurePosAfterRotate.Point.Y;

                    //机械手再平移这些量
                    XYU robotPosAfterRotateUAndMoveXY = new XYU();
                    robotPosAfterRotateUAndMoveXY.Point.X = (L_pickPos[toolIdx] + L_pickPosOffset[toolIdx]).Point.X + offsetX;
                    robotPosAfterRotateUAndMoveXY.Point.Y = (L_pickPos[toolIdx] + L_pickPosOffset[toolIdx]).Point.Y + offsetY;
                    robotPosAfterRotateUAndMoveXY.U = robotPosAfterRotateU;

                    toolPar.ResultPar.位置 = robotPosAfterRotateUAndMoveXY;

                    Frm_UpCamAlignTool.Instance.tbx_inputPosX.Text = toolPar.InputPar.位置.Point.X.ToString();
                    Frm_UpCamAlignTool.Instance.tbx_inputPosY.Text = toolPar.InputPar.位置.Point.Y.ToString();
                    Frm_UpCamAlignTool.Instance.tbx_inputPosU.Text = toolPar.InputPar.位置.U.ToString();

                    Frm_UpCamAlignTool.Instance.tbx_resultPosX.Text = toolPar.ResultPar.位置.Point.X.ToString();
                    Frm_UpCamAlignTool.Instance.tbx_resultPosY.Text = toolPar.ResultPar.位置.Point.Y.ToString();
                    Frm_UpCamAlignTool.Instance.tbx_resultPosU.Text = toolPar.ResultPar.位置.U.ToString();

                    //安全管控
                    XYU offset = toolPar.InputPar.位置 - L_featurePos[toolIdx];
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
        internal ToolPar toolPar = new ToolPar();


        [Serializable]
        public class ToolPar : ToolParBase
        {
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

            private XYU _位置 = new XYU();

            public XYU 位置
            {
                get { return _位置; }
                set { _位置 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {

            private XYU _位置 = new XYU();

            public XYU 位置
            {
                get { return _位置; }
                set { _位置 = value; }
            }
        }

    }
}
