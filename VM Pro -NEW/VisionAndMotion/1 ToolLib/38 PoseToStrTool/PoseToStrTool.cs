using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using HalconDotNet;
using WeifenLuo.WinFormsUI.Docking;

namespace VMPro
{
    [Serializable]
    internal class ToStrTool : ToolBase
    {
        internal XYU templatePos = new XYU();
        internal XYU inputPos = new XYU();
        internal string splitChar = " ";
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
                    if (inputPos != null)
                        toolPar.ResultPar.文本 = (inputPos.Point.X >= 0 ? "+" + inputPos.Point.X.ToString("000.000") : inputPos.Point.X.ToString("000.000")) + splitChar + (inputPos.Point.Y >= 0 ? "+" + inputPos.Point.Y.ToString("000.000") : inputPos.Point.Y.ToString("000.000")) + splitChar + (inputPos.U >= 0 ? "+" + inputPos.U.ToString("000.000") : inputPos.U.ToString("000.000"));
                    else if (toolPar.InputPar.点 != null)
                        toolPar.ResultPar.文本 = (toolPar.InputPar.点.X >= 0 ? "+" + toolPar.InputPar.点.X.ToString("000.000") : toolPar.InputPar.点.X.ToString("000.000")) + splitChar + (toolPar.InputPar.点.Y >= 0 ? "+" + toolPar.InputPar.点.Y.ToString("000.000") : toolPar.InputPar.点.Y.ToString("000.000"));


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
            private XY _点 = new XY();

            public XY 点
            {
                get { return _点; }
                set { _点 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {
            private string _文本 = string.Empty;

            public string 文本
            {
                get { return _文本; }
                set { _文本 = value; }
            }
        }

    }
}
