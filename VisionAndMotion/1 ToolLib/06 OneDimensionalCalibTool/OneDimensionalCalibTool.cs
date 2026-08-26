using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;

namespace VMPro
{
    [Serializable]
    internal class OneDimensionalCalibTool : ToolBase
    {

        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 标定数据
        /// </summary>
        internal List<List<double>> L_calibData = new List<List<double>>();
        /// <summary>
        /// 输入值
        /// </summary>
        internal double inputValue = 0;
        /// <summary>
        /// 输出值
        /// </summary>
        private double _outputValue = 0;
        internal double OutputValue
        {
            get
            {
                _outputValue = Math.Round((double)_outputValue, 3);
                return _outputValue;
            }
            set { _outputValue = value; }
        }
        /// <summary>
        /// 平移
        /// </summary>
        private HTuple _translate = 0;
        internal HTuple Translate
        {
            get
            {
                _translate = Math.Round((double)_translate, 5);
                return _translate;
            }
            set
            {
                _translate = value;
            }
        }
        /// <summary>
        /// 缩放
        /// </summary>
        private HTuple _scan = 1;
        internal HTuple Scan
        {
            get
            {
                _scan = Math.Round((double)_scan, 5);
                return _scan;
            }
            set
            {
                _scan = value;
            }
        }


        /// <summary>
        /// 复位工具
        /// </summary>
        internal void ResetTool()
        {
            try
            {
                Translate = 0;
                Scan = 1;
                Frm_OneDimensionalCalibTool.Instance.dgv_calibrateData.Rows.Clear();
                Frm_OneDimensionalCalibTool.Instance.dgv_calibrateData.Rows.Add(2);
                Frm_OneDimensionalCalibTool.Instance.tbx_translate.Text = "0";
                Frm_OneDimensionalCalibTool.Instance.tbx_scale.Text = "1";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 标定
        /// </summary>
        internal void Calibrate()
        {
            try
            {
                List<double> L_pixelPos = new List<double>();
                List<double> L_MechanicalPos = new List<double>();
                for (int i = 0; i < 2; i++)
                {
                    L_pixelPos.Add(Convert.ToDouble(Frm_OneDimensionalCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[0].Value));
                    L_MechanicalPos.Add(Convert.ToDouble(Frm_OneDimensionalCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[1].Value));
                }
                double spanPixel = L_pixelPos[1] - L_pixelPos[0];
                double spanMachanical = L_MechanicalPos[1] - L_MechanicalPos[0];
                Scan = spanMachanical / spanPixel;
                double valueAfterTrans = L_MechanicalPos[0] / Scan;
                Translate = valueAfterTrans - L_pixelPos[0];
                Frm_OneDimensionalCalibTool.Instance.tbx_translate.Text = Translate.ToString();
                Frm_OneDimensionalCalibTool.Instance.tbx_scale.Text = Scan.ToString();

                //保存标定数据
                L_calibData.Clear();
                for (int i = 0; i < 2; i++)
                {
                    List<double> list = new List<double>();
                    for (int j = 0; j < 2; j++)
                    {
                        list.Add(Math.Round(Convert.ToDouble(Frm_OneDimensionalCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[j].Value), 3));
                    }
                    L_calibData.Add(list);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 工具运行
        /// </summary>
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    _outputValue = (inputValue + Translate) * Scan;
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
