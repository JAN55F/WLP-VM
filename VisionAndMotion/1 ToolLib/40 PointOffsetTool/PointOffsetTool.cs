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
    internal class PointOffsetTool : ToolBase
    {
        internal XY templatePos = new XY();
        internal XY buchang = new XY();
        internal XY workPos = new XY();
        internal int pointIdx = 1;



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

                    if (toolPar.InputPar.点.ToString() == "System.Collections.Generic.List`1[VMPro.Point]")
                    {
                        if (((List<XY>)toolPar.InputPar.点).Count == 0)
                        {
                            toolRunStatu = ToolRunStatu.未知原因;
                            return;
                        }

                        if (((List<XY>)toolPar.InputPar.点).Count <= pointIdx - 1)
                        {
                            toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;
                            return;
                        }

                        XY offset = (((List<XY>)toolPar.InputPar.点)[pointIdx - 1] - templatePos) + buchang;
                        toolPar.ResultPar.点 = workPos + offset;
                    }
                    else
                    {
                        XY offset = ((XY)toolPar.InputPar.点[0] - templatePos) + buchang;
                        toolPar.ResultPar.点 = workPos + offset;
                    }
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
            private List <XY > _点;

            public List<XY> 点
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
            private XY _点;

            public XY 点
            {
                get { return _点; }
                set { _点 = value; }
            }
        }


    }
}
