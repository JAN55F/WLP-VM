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

namespace VMPro
{
    [Serializable]
    class DisplayEditTool : ToolBase
    {
        internal ToolPar toolPar = new ToolPar();

        internal string color = "green";
        internal string font = "mono";
        internal int lineWidth = 1;
        internal int size = 1;

    

        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage, bool runTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);

                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, color);
                    HOperatorSet.SetLineWidth(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, lineWidth);
                    //HOperatorSet.SetFont(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, font);
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
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

        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {
            private HObject _图像;

            public HObject 图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }
        }



    }

}
