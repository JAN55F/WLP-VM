using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using HalconDotNet;

namespace VMPro
{
    [Serializable]
    internal class CreateROITool : ToolBase
    {

        /// <summary>
        /// 流程名
        /// </summary>
        internal string jobName = string.Empty;
        internal HObject localRegion;
        internal bool fromLocal = false;

        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();
        internal HObject inputImage;

        /// <summary>
        /// 输入的第一条线段
        /// </summary>
        internal Line line1;
        /// <summary>
        /// 输入的第二条线段
        /// </summary>
        internal Line line2;
        internal int leftTopRow;
        internal int leftTopCol;
        internal int rightDownRow;
        internal int rightDownCol;
        internal bool LeftTopRowUseConst = true;
        internal bool LeftTopColUseConst = true;
        internal bool RightDownRowUseConst = true;
        internal bool RightDownColUseConst = true;
        internal int leftTopRowConstValue;
        internal int leftTopColConstValue;
        internal int rightDownRowConstValue;
        internal int rightDownColConstValue;
        internal XYU inputPose = new XYU();
        internal XYU templatePose = new XYU();
        public ToolPar toolPar = new ToolPar();
        /// <summary>
        /// 计算出来的线与线之间的距离值
        /// </summary>
        private XY _resultDistance;
        internal XY ResultDistance
        {
            get
            {
                if (_resultDistance == null)
                    _resultDistance = new XY();

                _resultDistance.X = Math.Round(_resultDistance.X, 3);
                _resultDistance.Y = Math.Round(_resultDistance.Y, 3);



                return _resultDistance;
            }
            set { _resultDistance = value; }
        }


        /// <summary>
        /// 工具恢复到初始状态
        /// </summary>
        internal void ResetTool()
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
        /// 清空上次运行的所有输入
        /// </summary>
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
        private object obj = new object();
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否刷新图像</param>
        public override void Run(bool updateImage, bool b, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;
                    if (fromLocal)
                    {
                        HObject temp = localRegion;
                        if (inputPose != null)
                        {
                            HTuple Row = inputPose.Point.X - templatePose.Point.X;
                            HTuple Col = inputPose.Point.Y - templatePose.Point.Y;
                            HTuple angle = inputPose.U - templatePose.U;

                            HTuple _homMat2D;
                            HOperatorSet.HomMat2dIdentity(out _homMat2D);
                            HOperatorSet.HomMat2dRotate(_homMat2D, ((HTuple)(angle)).TupleRad(), (HTuple)templatePose.Point.X, (HTuple)templatePose.Point.Y, out _homMat2D);
                            HOperatorSet.HomMat2dTranslate(_homMat2D, (HTuple)(Row), (HTuple)(Col), out _homMat2D);

                            //对预期线的起始点做放射变换

                            HOperatorSet.AffineTransRegion(temp, out temp, _homMat2D, new HTuple("nearest_neighbor"));
                        }

                        //////ShowObj(jobName, temp);
                        toolPar.ResultPar.ROI = temp;
                    }
                    else
                    {
                        //HOperatorSet.GenRectangle1(out outputROI, new HTuple(LeftTopRowUseConst ? leftTopRowConstValue : leftTopRow), new HTuple(LeftTopColUseConst ? leftTopColConstValue : leftTopCol - 80), new HTuple(RightDownRowUseConst ? rightDownRowConstValue : rightDownRow), new HTuple(RightDownColUseConst ? rightDownColConstValue : rightDownCol + 80));
                        //////ShowObj(jobName, outputROI);
                        HObject temp111;
                        HOperatorSet.GenEmptyObj(out temp111);
                        toolPar.ResultPar.ROI = temp111;
                        for (int i = 0; i < regions.Count; i++)
                        {
                            HObject temp222;
                            HOperatorSet.Union2(toolPar.ResultPar.ROI, regions[i].getRegion(), out temp222);
                            toolPar.ResultPar.ROI = temp222;
                        }

                    }
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }





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
            private HObject _图像;

            public HObject 图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {
            private HObject _ROI;

            public HObject ROI
            {
                get { return _ROI; }
                set { _ROI = value; }
            }
        }

    }
}
