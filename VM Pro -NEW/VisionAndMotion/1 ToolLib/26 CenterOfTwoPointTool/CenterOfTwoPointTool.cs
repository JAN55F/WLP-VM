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
    internal class CenterOfPP : ToolBase
    {

        /// <summary>
        /// 输入图像
        /// </summary>
        internal HObject inputImage;
        /// <summary>
        /// 流程名
        /// </summary>
        internal string jobName = string.Empty;
        /// <summary>
        /// 输入点
        /// </summary>
        internal XY inputPoint;
        /// <summary>
        /// 输入线
        /// </summary>
        internal Line inputLine;
        /// <summary>
        /// 点线距离
        /// </summary>
        internal double outputDistance = 0;


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
                inputImage = null;
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


                    toolPar.ResultPar.中点.X = (toolPar.InputPar.点1.X + toolPar.InputPar.点2.X) / 2;
                    toolPar.ResultPar.中点.Y = (toolPar.InputPar.点1.Y + toolPar.InputPar.点2.Y) / 2;

                    HTuple temp;
                    HOperatorSet.AngleLx(toolPar.InputPar.点1.X, toolPar.InputPar.点1.Y, toolPar.InputPar.点2.X, toolPar.InputPar.点2.Y, out temp);
                    toolPar.ResultPar.方向 = temp;

                    HObject cross;
                    HOperatorSet.GenCrossContourXld(out cross, toolPar.ResultPar.中点.X, toolPar.ResultPar.中点.Y, new HTuple(30), new HTuple(0));
                    // ShowObj(jobName  ,cross);
                    GetImageWindowControl().hwc_imageWindow.DispObj(cross, "green");
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
            private XY _点1;

            public XY 点1
            {
                get { return _点1; }
                set { _点1 = value; }
            }

            private XY _点2;

            public XY 点2
            {
                get { return _点2; }
                set { _点2 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {
            private XY _中点 = new XY();

            public XY 中点
            {
                get { return _中点; }
                set { _中点 = value; }
            }
            private double _方向;
            public double 方向
            {
                get { return _方向; }
                set { _方向 = value; }
            }
        }



    }
}
