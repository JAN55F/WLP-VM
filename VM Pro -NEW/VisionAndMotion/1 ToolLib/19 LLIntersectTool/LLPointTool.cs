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
    internal class LLIntersectTool : ToolBase
    {

        /// <summary>
        /// 输入的第一条线段
        /// </summary>
        internal Line inputLine1;
        /// <summary>
        /// 输入的第二条线段
        /// </summary>
        internal Line inputLine2;

        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();


        /// <summary>
        /// 复位工具
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
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="updateImage"></param>
        /// <param name="b"></param>
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    HTuple row, col, temp1;
                    HOperatorSet.IntersectionLines(inputLine1.起点.X,
                                                   inputLine1.起点.Y,
                                                   inputLine1.终点.X,
                                                   inputLine1.终点.Y,
                                                   inputLine2.起点.X,
                                                   inputLine2.起点.Y,
                                                   inputLine2.终点.X,
                                                   inputLine2.终点.Y,
                                                   out row,
                                                   out col,
                                                   out temp1);
                    HObject cross;
                    HOperatorSet.GenCrossContourXld(out cross, row, col, new HTuple(50), new HTuple(0));
                    HObject circle;
                    HOperatorSet.GenCircle(out circle, row, col, 15);
                    ShowObj(cross, "green");
                    ShowObj(circle, "green");
                    XY p = new XY();
                    p.X = row;
                    p.Y = col;
                    toolPar.ResultPar.点.Clear();
                    toolPar.ResultPar.点.Add(p);
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
        public class ResultPar
        {
            private List<XY> _点 = new List<XY>();

            public List<XY> 点
            {
                get { return _点; }
                set { _点 = value; }
            }
        }



    }
}
