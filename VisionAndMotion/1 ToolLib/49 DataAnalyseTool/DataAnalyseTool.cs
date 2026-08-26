using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using Basler.Pylon;
using VMPro.Properties;
using Ookii.Dialogs.WinForms;
using VMPro;
using System.Text.RegularExpressions;

namespace VMPro
{
    [Serializable]
    class DataAnalyseTool : ToolBase
    {
        internal List<sDataType> L_items = new List<sDataType>();
        internal string endChar = string.Empty;
        /// <summary>
        /// 通讯端口名称
        /// </summary>
        internal string EthernetName = string.Empty;
        /// <summary>
        /// 工具参数
        /// </summary>
        internal ToolPar toolPar = new ToolPar();


        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否更新图像</param>
        /// <param name="debugTool">调试工具模式</param>
        public override void Run(bool updateImage, bool debugTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);

                    if (L_items[0].downLimit <= Convert.ToDouble(toolPar.InputPar.输入项1) && Convert.ToDouble(toolPar.InputPar.输入项1) <= L_items[0].upLimit)
                        toolPar.ResultPar.输出项1 = L_items[0].inResult;
                    else
                        toolPar.ResultPar.输出项1 = L_items[0].outResult;

                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        #region 参数
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
            private string _输入项1 = string.Empty;
            public string 输入项1
            {
                get { return _输入项1; }
                set { _输入项1 = value; }
            }
        }
        [Serializable]
        public class RunPar { }
        [Serializable]
        internal class ResultPar
        {
            private string _输出项1 = string.Empty;
            public string 输出项1
            {
                get { return _输出项1; }
                set { _输出项1 = value; }
            }
        }
        #endregion

    }
    [Serializable]
    internal struct sDataType
    {
        internal string inputItem;
        internal double downLimit;
        internal double upLimit;
        internal string inResult;
        internal string outResult;
    }
}
