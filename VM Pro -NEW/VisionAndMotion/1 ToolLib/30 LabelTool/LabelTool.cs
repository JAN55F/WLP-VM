using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Windows.Forms;

namespace VMPro
{
    [Serializable]
    internal class LabelTool : ToolBase
    {
        public LabelTool()
        {
            Label label = new Label();
            label.Row = "200";
            label.Col = "100";
            label.ExpectValue = "OK";
            label.Incolor = "blue";
            label.OutColor = "blue";
            label.OutputItem = "InputItem1";
            label.PreAddStr = string.Empty;
            label.Size = "10";
            L_label.Add(label);
        }

        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 标签项集合
        /// </summary>
        internal List<Label> L_label = new List<Label>();
        /// <summary>
        /// 输入项和值
        /// </summary>
        internal Dictionary<string, string> D_inputItemAndVlaue = new Dictionary<string, string>();


        /// <summary>
        /// 保存数据
        /// </summary>
        internal void SaveData2()
        {
            try
            {
                if (Job.loadForm)
                    return;
                L_label.Clear();
                for (int i = 0; i < Frm_LabelTool.Instance.dgv_outputItem.Rows.Count - 1; i++)
                {
                    if (Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[2].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[3].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[4].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[5].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[6].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[7].Value != null)
                    {
                        if (Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[0].Value == null)
                            continue;
                        Label label = new Label();
                        label.OutputItem = Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[0].Value.ToString();
                        label.PreAddStr = Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[1].Value == null ? "" : Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[1].Value.ToString();
                        label.Row = Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[2].Value.ToString();
                        label.Col = Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[3].Value.ToString();
                        label.Incolor = Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[6].Value.ToString();
                        label.Size = Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[8].Value.ToString();
                        label.DownLimit = Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[4].Value.ToString();
                        label.UpLimit = Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[5].Value.ToString();
                        label.OutColor = Frm_LabelTool.Instance.dgv_outputItem.Rows[i].Cells[7].Value.ToString();
                        label.ValueType = "Value";      //表示可以比较大小的值类型
                        L_label.Add(label);
                    }
                }

                for (int i = 0; i < Frm_LabelTool.Instance.dgv_outputItem2.Rows.Count - 1; i++)
                {
                    if (Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[0].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[2].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[3].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[4].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[5].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[6].Value != null &&
                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[7].Value != null)
                    {
                        if (Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[0].Value == null)
                            continue;
                        Label label = new Label();
                        label.OutputItem = Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[0].Value.ToString();
                        label.PreAddStr = Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[1].Value == null ? "" : Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[1].Value.ToString();
                        label.Row = Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[2].Value.ToString();
                        label.Col = Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[3].Value.ToString();
                        label.Incolor = Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[5].Value.ToString();
                        label.Size = Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[7].Value.ToString();
                        label.OutColor = Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[6].Value.ToString();
                        label.ExpectValue = Frm_LabelTool.Instance.dgv_outputItem2.Rows[i].Cells[4].Value.ToString();
                        label.ValueType = "Str";      //表示字符串类型
                        L_label.Add(label);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    for (int i = 0; i < L_label.Count; i++)
                    {
                        GetImageWindowControl().set_display_font(Convert.ToInt16(L_label[i].Size), "nomo", "true", "false");

                        if (L_label[i].ValueType == "Value")        //数值类
                        {
                            double value = Convert.ToDouble(D_inputItemAndVlaue[L_label[i].OutputItem]);
                            double downLimit = Convert.ToDouble(L_label[i].DownLimit);
                            double upLimit = Convert.ToDouble(L_label[i].UpLimit);
                            if (value >= downLimit && value <= upLimit)
                            {
                                Frm_Main.Instance.disp_message(GetImageWindowControl().hwc_imageWindow.HWindowHalconID,
                                                             L_label[i].PreAddStr + D_inputItemAndVlaue[L_label[i].OutputItem],
                                                             new HTuple("image"),
                                                             new HTuple(Convert.ToInt32(L_label[i].Row)),
                                                             new HTuple(Convert.ToInt32(L_label[i].Col)),
                                                             new HTuple(L_label[i].Incolor),
                                                             new HTuple("false"));
                            }
                            else
                            {
                                Frm_Main.Instance.disp_message(GetImageWindowControl().hwc_imageWindow.HWindowHalconID,
                                                             L_label[i].PreAddStr + D_inputItemAndVlaue[L_label[i].OutputItem],
                                                             new HTuple("image"),
                                                             new HTuple(Convert.ToInt32(L_label[i].Row)),
                                                             new HTuple(Convert.ToInt32(L_label[i].Col)),
                                                             new HTuple(L_label[i].OutColor),
                                                             new HTuple("false"));
                            }
                        }
                        else
                        {
                            HTuple row, col, row1, col1;
                            HOperatorSet.GetPart(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, out row, out col, out row1, out col1);
                            if (L_label[i].ExpectValue == D_inputItemAndVlaue[L_label[i].OutputItem])
                            {
                                Frm_Main.Instance.disp_message(GetImageWindowControl().hwc_imageWindow.HWindowHalconID,
                                                             L_label[i].PreAddStr + D_inputItemAndVlaue[L_label[i].OutputItem],
                                                             new HTuple("image"),
                                                             new HTuple(Convert.ToInt32(L_label[i].Row)),
                                                             new HTuple(Convert.ToInt32(L_label[i].Col)),
                                                             new HTuple(L_label[i].Incolor),
                                                             new HTuple("false"));
                            }
                            else
                            {
                                Frm_Main.Instance.set_display_font(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, Convert.ToInt16(L_label[i].Size), "nomo", "true", "false");

                                Frm_Main.Instance.disp_message(GetImageWindowControl().hwc_imageWindow.HWindowHalconID,
                                                             L_label[i].PreAddStr + D_inputItemAndVlaue[L_label[i].OutputItem],
                                                             new HTuple("image"),
                                                             new HTuple((row + Convert.ToInt32(L_label[i].Row))),
                                                             new HTuple(col + Convert.ToInt32(L_label[i].Col)),
                                                             new HTuple(L_label[i].OutColor),
                                                             new HTuple("false"));
                                Application.DoEvents();
                            }
                        }
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
            private string _文本1 = string.Empty;

            public string 文本1
            {
                get { return _文本1; }
                set { _文本1 = value; }
            }

            private string _文本2 = string.Empty;

            public string 文本2
            {
                get { return _文本2; }
                set { _文本2 = value; }
            }

            private string _文本3 = string.Empty;

            public string 文本3
            {
                get { return _文本3; }
                set { _文本3 = value; }
            }

            private string _文本4 = string.Empty;

            public string 文本4
            {
                get { return _文本4; }
                set { _文本4 = value; }
            }

            private string _文本5 = string.Empty;

            public string 文本5
            {
                get { return _文本5; }
                set { _文本5 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {
            private HObject _输出图像;

            public HObject 输出图像
            {
                get { return _输出图像; }
                set { _输出图像 = value; }
            }
        }

    }
}
