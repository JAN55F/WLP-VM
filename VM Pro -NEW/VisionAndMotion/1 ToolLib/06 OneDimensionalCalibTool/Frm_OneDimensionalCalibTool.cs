using HalconDotNet;
using Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_OneDimensionalCalibTool : Frm_FormBase
    {
        internal Frm_OneDimensionalCalibTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_OneDimensionalCalibTool _instance;
        internal static Frm_OneDimensionalCalibTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_OneDimensionalCalibTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static OneDimensionalCalibTool oneDimensionalCalibTool = new OneDimensionalCalibTool();

      
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            oneDimensionalCalibTool.ResetTool();
        }
        private void btn_calibrate_Click(object sender, EventArgs e)
        {
            oneDimensionalCalibTool.Calibrate();
        }
        private void ckb_toolEnable_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_toolEnable.Checked;
        }
        private void tsb_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            oneDimensionalCalibTool.Run(true, true, toolName);
            if (oneDimensionalCalibTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(oneDimensionalCalibTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(oneDimensionalCalibTool.toolRunStatu.ToString(), Color.Black );
        }
        private void Frm_OneDimensionalCalibTool_Load(object sender, EventArgs e)
        {
            dgv_calibrateData.Rows.Add(2);
        }
        private void btn_firstPoint_Click(object sender, EventArgs e)
        {
            try
            {
                dgv_calibrateData.Rows[0].Cells[0].Value = oneDimensionalCalibTool.inputValue;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_secondPoint_Click(object sender, EventArgs e)
        {
            try
            {
                dgv_calibrateData.Rows[1].Cells[0].Value = oneDimensionalCalibTool.inputValue;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
