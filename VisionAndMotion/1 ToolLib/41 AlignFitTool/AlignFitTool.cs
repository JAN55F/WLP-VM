using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VMPro
{
    [Serializable]
    internal class AlignFitTool : ToolBase
    {
        internal AlignFitTool()
        {
          
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
        internal XYU TemplateBelowBoardPos = new XYU();
        internal XYU TemplateElementPos = new XYU();
        /// <summary>
        /// 制作模板时示教的取料位置坐标
        /// </summary>
        internal XYU TemplateElementPlacePos = new XYU();



        internal XYU curBelowBoardPos = new XYU();
        internal XYU curElementPos = new XYU();
        /// <summary>
        /// 制作模板时示教的取料位置坐标
        /// </summary>
        internal XYU curElementPlacePos = new XYU();
        /// <summary>
        /// 本次定位输入坐标
        /// </summary>
        internal XYU inputPos = new XYU();
        /// <summary>
        /// 模板取料位置补偿值
        /// </summary>
        internal XYU TemplateElementPlacePosOffset = new XYU();
        /// <summary>
        /// 安全范围
        /// </summary>
        internal XYU L_safetyRange = new XYU();
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
                var res = new XYU()        ;
                   res. Point.X = rotateCenter.Point.X + (curPos.Point.X - rotateCenter.Point.X) * Math.Cos(rad) - (curPos.Point.Y - rotateCenter.Point.Y) * Math.Sin(rad);
                   res.Point.Y = rotateCenter.Point.Y + (curPos.Point.X - rotateCenter.Point.X) * Math.Sin(rad) + (curPos.Point.Y - rotateCenter.Point.Y) * Math.Cos(rad);
                 res.   U = curPos.U + rotateAngle  ;
               
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
        public override void Run(  bool updateImage, bool temp,string toolName)
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

                    ////////首先旋转使角度重合,然后将机械手平移，计算出使本次定位产品和创建模板时的产品重合时机械手的坐标，即取料坐标
                    //////double offsetU = inputPos.U - BelowBoardPos[toolIdx].U;
                    //////offsetU = offsetU * 180 / Math.PI;
                    //////double robotPosAfterRotateU = ((TemplateElementPlacePos[toolIdx] + L_pickPosOffset[toolIdx]).U) + offsetU;

                    ////////计算旋转之后模板特征点的XY坐标
                    //////XYU featurePosAfterRotate = RotateAt(BelowBoardPos[toolIdx], TemplateElementPlacePos[toolIdx] + L_pickPosOffset[toolIdx], offsetU);

                    ////////计算经过旋转的特征点坐标和创建模板时的机械坐标的平移量
                    //////double offsetX = inputPos.Point.X - featurePosAfterRotate.Point.X;
                    //////double offsetY = inputPos.Point.Y - featurePosAfterRotate.Point.Y;

                    ////////机械手再平移这些量
                    //////XYU robotPosAfterRotateUAndMoveXY = new XYU();
                    //////robotPosAfterRotateUAndMoveXY.Point.X = (TemplateElementPlacePos[toolIdx] + L_pickPosOffset[toolIdx]).Point.X + offsetX;
                    //////robotPosAfterRotateUAndMoveXY.Point.Y = (TemplateElementPlacePos[toolIdx] + L_pickPosOffset[toolIdx]).Point.Y + offsetY;
                    //////robotPosAfterRotateUAndMoveXY.U = robotPosAfterRotateU;

                    //////resultPos = robotPosAfterRotateUAndMoveXY;

                    //////Frm_UpCamAlignTool.Instance.tbx_inputPosX.Text = inputPos.Point.X.ToString();
                    //////Frm_UpCamAlignTool.Instance.tbx_inputPosY.Text = inputPos.Point.Y.ToString();
                    //////Frm_UpCamAlignTool.Instance.tbx_inputPosU.Text = inputPos.U.ToString();

                    //////Frm_UpCamAlignTool.Instance.tbx_resultPosX.Text = resultPos.Point.X.ToString();
                    //////Frm_UpCamAlignTool.Instance.tbx_resultPosY.Text = resultPos.Point.Y.ToString();
                    //////Frm_UpCamAlignTool.Instance.tbx_resultPosU.Text = resultPos.U.ToString();

                    ////////安全管控
                    //////XYU offset = inputPos - BelowBoardPos[toolIdx];
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
                    toolPar.ResultPar.组装位置 = inputPos;
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
           
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        public class ResultPar
        {
            private XYU _组装位置;

            public XYU 组装位置
            {
                get { return _组装位置; }
                set { _组装位置 = value; }
            }

            
        }



    }
}
