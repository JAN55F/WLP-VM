using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using VersionMethods;
using System.Threading;
using System.Windows.Forms;
using ViewWindow.Model;
using System.Text.RegularExpressions;

namespace VMPro
{
    [Serializable]
    internal class QuoteTransTool : ToolBase
    {

        /// <summary>
        /// 卡尺数量
        /// </summary>
        internal string cliperNum = string.Empty;
        internal int photoPos = 0;

        private object obj = new object();
        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage, bool runTool, string toolName1)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);

                    if (cliperNum == string.Empty)
                    {
                        toolRunStatu = ToolRunStatu.未指定被引用标定工具;
                        return;
                    }

                    string jobbName = Regex.Split(cliperNum, " . ")[0];
                    string toolName = Regex.Split(cliperNum, " . ")[1];

                    EyeHandCalibTool eyeHandCalibTool = Job.FindJobByName(jobbName).FindToolByName(toolName) as EyeHandCalibTool;
                    HTuple homMat2D = eyeHandCalibTool.homMat2D;

                    Dictionary<int, XY> D_photoPos = eyeHandCalibTool.D_photoPos;

                    //对点进行放射变换
                    if (toolPar.InputPar.点.Count != 0)
                    {
                        toolPar.ResultPar.点.Clear();
                        for (int i = 0; i < toolPar.InputPar.点.Count; i++)
                        {
                            HTuple rowAfterTrans;
                            HTuple colAfterTrans;
                            HOperatorSet.AffineTransPoint2d(homMat2D, (HTuple)toolPar.InputPar.点[i].X, (HTuple)toolPar.InputPar.点[i].Y, out rowAfterTrans, out colAfterTrans);


                            //标定一次，可能会有多个拍照位置，拍照位不同，此处需要做一下变换
                            double spanX = D_photoPos[photoPos].X - D_photoPos[0].X;
                            double spanY = D_photoPos[photoPos].Y - D_photoPos[0].Y;
                            rowAfterTrans = rowAfterTrans + spanX;
                            colAfterTrans = colAfterTrans + spanY;

                            XY temp1 = new XY();
                            temp1.X = rowAfterTrans;
                            temp1.Y = colAfterTrans;

                            toolPar.ResultPar.点.Add(temp1);
                        }

                    }
                    else if (toolPar.InputPar.位置.Count != 0)
                    {
                        HTuple rowAfterTrans;
                        HTuple colAfterTrans;
                        HOperatorSet.AffineTransPoint2d(homMat2D, (HTuple)toolPar.InputPar.位置[0].Point.X, (HTuple)toolPar.InputPar.位置[0].Point.Y, out rowAfterTrans, out colAfterTrans);
                        toolPar.ResultPar.位置.Clear();
                        XYU xyu = new XYU();

                        xyu.Point.X = rowAfterTrans;
                        xyu.Point.Y = colAfterTrans;
                        xyu.U = toolPar.InputPar.位置[0].U;
                        toolPar.ResultPar.位置.Add(xyu);
                    }


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

            private List<XY> _点 = new List<XY>();

            public List<XY> 点
            {
                get { return _点; }
                set { _点 = value; }
            }

            private List<XYU> _位置 = new List<XYU>();

            public List<XYU> 位置
            {
                get { return _位置; }
                set { _位置 = value; }
            }
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

            private List<XY> _点 = new List<XY>();

            public List<XY> 点
            {
                get { return _点; }
                set { _点 = value; }
            }

            private List<XYU> _位置 = new List<XYU>();

            public List<XYU> 位置
            {
                get { return _位置; }
                set { _位置 = value; }
            }
        }


    }
}
