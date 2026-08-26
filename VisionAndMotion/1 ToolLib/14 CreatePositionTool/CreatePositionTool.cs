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
    internal class CreatePositionTool : ToolBase
    {
        internal CreatePositionTool()
        {
            XYU xyu = new XYU();
        }

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
        /// <param name="updateImage">是否刷新图像</param>
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    toolPar.ResultPar.位置.Clear();
                    if (toolPar.InputPar.点 == null || toolPar.InputPar.点.Count == 0 || toolPar.InputPar.点[0] == null)
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Pos : ToolRunStatu.未指定输入坐标点);
                        return;
                    }
                    XYU xyu = new XYU();
                    xyu.Point.X = toolPar.InputPar.点[0].X;
                    xyu.Point.Y = toolPar.InputPar.点[0].Y;
                    xyu.U = toolPar.InputPar.方向;
                    toolPar.ResultPar.位置.Add(xyu);

                 
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
            private List<XY> _点 = new List<XY>();

            public List<XY> 点
            {
                get { return _点; }
                set { _点 = value; }
            }

            private double _方向 = 0;

            public double 方向
            {
                get { return _方向; }
                set { _方向 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        public class ResultPar
        {
            private List<XYU> _位置 = new List<XYU>();

            public List<XYU> 位置
            {
                get { return _位置; }
                set { _位置 = value; }
            }
        }

    }
}
