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
    internal class CreateLineTool : ToolBase
    {
        internal HObject localRegion;
        internal bool fromLocal = false;


        internal Line outputLine = new Line();
        internal XY inputPoint;
        internal double inputAngle;
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
        internal HObject outputROI;
        internal XYU outputPos = new XYU();
        internal XYU inputPose = new XYU();
        internal XYU templatePose = new XYU();
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
                    outputLine = new Line();
                    outputLine.起点 = toolPar.InputPar.起点;
                    outputLine.终点 = toolPar.InputPar.终点;
                    toolPar.ResultPar.线.起点 = outputLine.起点;
                    toolPar.ResultPar.线.终点 = outputLine.终点;
                    HOperatorSet.DispLine(Frm_ImageWindow.Instance.hwc_imageWindow.HWindowHalconID, toolPar.InputPar.起点.X, toolPar.InputPar.起点.Y, toolPar.InputPar.终点.X, toolPar.InputPar.终点.Y);
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
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
            private XY _起点 = new XY();

            public XY 起点
            {
                get { return _起点; }
                set { _起点 = value; }
            }

            private XY _终点 = new XY();

            public XY 终点
            {
                get { return _终点; }
                set { _终点 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        public class ResultPar
        {
            private Line _线 = new Line();

            public Line 线
            {
                get { return _线; }
                set { _线 = value; }
            }
        }

    }
    [Serializable]
    public class Result2
    {
        public Result2()
        {
            _线 = new Line();
        }
        private Line _线;

        public Line 线
        {
            get { return _线; }
            set { _线 = value; }
        }
    }
}
