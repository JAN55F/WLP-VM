using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_BatteryFirstAlignTool : Frm_FormBase
    {
        internal Frm_BatteryFirstAlignTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_BatteryFirstAlignTool _instance;
        internal static Frm_BatteryFirstAlignTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_BatteryFirstAlignTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static BatteryFirstAlignTool batteryFirstAlignTool;



        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            batteryFirstAlignTool.Run(true, true, toolName);
            if (batteryFirstAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(batteryFirstAlignTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(batteryFirstAlignTool.toolRunStatu.ToString(), Color.Black );
        }
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void tsb_help_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void ckb_findLineToolEnable_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable  = ckb_findLineToolEnable.Checked;
        }



        private void btn_runFindLineTool_Click(object sender, EventArgs e)
        {
            batteryFirstAlignTool.Run(true, true, toolName);
            if (batteryFirstAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(batteryFirstAlignTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(batteryFirstAlignTool.toolRunStatu.ToString(), Color.Green);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (batteryFirstAlignTool.regions.Count == 0)
            {
                GetImageWindowControl(jobName).hwc_imageWindow.viewWindow.genRect1(200.0, 200.0, 600.0, 800.0, ref batteryFirstAlignTool.regions);
                GetImageWindowControl(jobName).regions = batteryFirstAlignTool.regions;
            }
            else
            {
                GetImageWindowControl(jobName).hwc_imageWindow.viewWindow.displayROI(batteryFirstAlignTool.regions);
                GetImageWindowControl(jobName).regions = batteryFirstAlignTool.regions;
            }

        }

        private void nud_minThreshold_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                batteryFirstAlignTool.minThreshold = Convert.ToInt16(nud_minThreshold.Value);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void nud_maxThreshold_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                batteryFirstAlignTool.maxThreshold = Convert.ToInt16(nud_maxThreshold.Value);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                batteryFirstAlignTool.minArea = Convert.ToInt16(numericUpDown1.Value);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                batteryFirstAlignTool.maxArea = Convert.ToInt32(numericUpDown2.Value);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                batteryFirstAlignTool.dilationAndErosionSize = Convert.ToInt32(numericUpDown3.Value);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void cbx_edgeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            batteryFirstAlignTool.pointIndex =  Convert.ToInt16(cbx_edgeSelect.Text.Substring(1));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            batteryFirstAlignTool.lineIndex = Convert.ToInt16(comboBox1.Text.Substring(1));
        }

    }
}
