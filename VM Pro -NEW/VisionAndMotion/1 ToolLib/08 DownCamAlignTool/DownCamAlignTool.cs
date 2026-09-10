using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VMPro
{
    [Serializable]
    internal class DownCamAlignTool : ToolBase
    {

        /// <summary>
        /// 机械手拍照位置坐标
        /// </summary>
        internal XYU photoPos = new XYU();
        /// <summary>
        /// 制作模板时特征点机械坐标
        /// </summary>
        internal XYU featurePos = new XYU();
        /// <summary>
        /// 示教的机械手放料坐标
        /// </summary>
        internal XYU placePos = new XYU();
        /// <summary>
        /// 本次定位输入坐标
        /// </summary>
        internal XYU inputPos = new XYU();
        /// <summary>
        /// 计算出来的最终机械手放料坐标
        /// </summary>
        internal XYU resultPos = new XYU();
        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();


        internal void ResetTool()
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
        /// <param name="rotateAngle">旋转角度，单位为度</param>
        /// <returns></returns>
        internal XYU Rotate_At(XYU curPos, XYU rotateCenter, double rotateAngle)
        {
            try
            {
                double rad = rotateAngle * Math.PI / 180;
                var res = new XYU();
                 res.   Point.X = rotateCenter.Point.X + (curPos.Point.X - rotateCenter.Point.X) * Math.Cos(rad) - (curPos.Point.Y - rotateCenter.Point.Y) * Math.Sin(rad);
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
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);



                    ////////动态建立工具坐标方式
                    //////double templateOffsetU = templateFeaturePos.U - inputPos.U;
                    //////templateOffsetU = templateOffsetU * 180 / Math.PI;
                    //////XYU posAfterRotate = Rotate_At(templateFeaturePos, caputurePos, touchPos.U - caputurePos.U);

                    //////targetPos.X = posAfterRotate.X + (touchPos.X - caputurePos.X);
                    //////targetPos.Y = posAfterRotate.Y + (touchPos.Y - caputurePos.Y);
                    //////targetPos.U = touchPos.U - templateOffsetU;




                    //不建立工具坐标方式
                    if (inputPos==null )
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Pos : ToolRunStatu.未指定输入坐标点);
                        return;
                    }

                    //然后将机械手平移和旋转，使本次定位特征点和模板时的特征点重合
                    double offsetU = featurePos.U - inputPos.U;
                    offsetU = offsetU * 180 / Math.PI;
                    double robotPosAfterRotateU = photoPos.U - offsetU;

                    //首先机械手旋转使产品角度重合，计算旋转之后特征点的坐标
                    XYU curFeaturePosAfterRotate = Rotate_At(inputPos, photoPos, -offsetU);

                    //计算本次定位特征点经过旋转后的机械坐标和模板时的机械坐标的平移量
                    double offsetX = featurePos.Point.X - curFeaturePosAfterRotate.Point.X;
                    double offsetY = featurePos.Point.Y - curFeaturePosAfterRotate.Point.Y;

                    //机械手再平移这些量，然后当机械手移动到计算出来的位置时，产品和模板时的产品重合
                    XYU robotPosAfterRotateUAndMoveXY = new XYU();
                    robotPosAfterRotateUAndMoveXY.Point.X = photoPos.Point.X + offsetX;
                    robotPosAfterRotateUAndMoveXY.Point.Y = photoPos.Point.Y + offsetY;
                    robotPosAfterRotateUAndMoveXY.U = robotPosAfterRotateU;

                    //如果机械手移动到上述点，则本次的产品与模板产品重合

                    //计算做模板时，从拍照位置到放料位置旋转的角度
                    double anglePhotoToPlace = placePos.U - photoPos.U;

                    //计算模板产品绕模板特征点旋转这么多角度时机械手应该所处的位置
                    XYU robotTemplatePosAfterRotate = Rotate_At(photoPos, featurePos, anglePhotoToPlace);
                    robotTemplatePosAfterRotate.U = photoPos.U + anglePhotoToPlace;

                    //计算模板产品旋转后的位置到放料位置的平移量             //此处不应该取绝对值，后期应更正
                    double productPlaceMoveX = Math.Abs(placePos.Point.X - robotTemplatePosAfterRotate.Point.X);
                    double productPlaceMoveY = Math.Abs(placePos.Point.Y - robotTemplatePosAfterRotate.Point.Y);

                    //计算当前产品绕模板特征点旋转这么多角度时机械手应该所处的位置
                    XYU curRobotPosAfterRotate = Rotate_At(robotPosAfterRotateUAndMoveXY, featurePos, anglePhotoToPlace);
                    curRobotPosAfterRotate.U = robotPosAfterRotateUAndMoveXY.U + anglePhotoToPlace;

                    //当前产品旋转后的位置加上放料平移量，就是本次定位最终的放料坐标
                    resultPos.Point.X = Math.Round(curRobotPosAfterRotate.Point.X + productPlaceMoveX, 3);
                    resultPos.Point.Y = Math.Round(curRobotPosAfterRotate.Point.Y + productPlaceMoveY, 3);
                    resultPos.U = Math.Round(curRobotPosAfterRotate.U, 3);

                    Frm_DownCamAlignTool.Instance.tbx_inputPosX.Text = inputPos.Point.X.ToString();
                    Frm_DownCamAlignTool.Instance.tbx_inputPosY.Text = inputPos.Point.Y.ToString();
                    Frm_DownCamAlignTool.Instance.tbx_inputPosU.Text = inputPos.U.ToString();

                    Frm_DownCamAlignTool.Instance.tbx_resultPosX.Text = resultPos.Point.X.ToString();
                    Frm_DownCamAlignTool.Instance.tbx_resultPosY.Text = resultPos.Point.Y.ToString();
                    Frm_DownCamAlignTool.Instance.tbx_resultPosU.Text = resultPos.U.ToString();

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
