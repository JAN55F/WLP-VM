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
    internal partial class Frm_OptLightControlTool : Frm_FormBase
    {
        internal Frm_OptLightControlTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_OptLightControlTool _instance;
        public static Frm_OptLightControlTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_OptLightControlTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static OptLightControlTool optLightControlTool = new OptLightControlTool();


        private void ckb_shapeMatchToolNotRun_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_shapeMatchToolEnable.Checked;
        }

      
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }
        private void btn_runShapeMatchTool_Click(object sender, EventArgs e)
        {
            optLightControlTool.Run(true, true, toolName);
            if (optLightControlTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(optLightControlTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(optLightControlTool.toolRunStatu.ToString(), Color.Green);
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            optLightControlTool.Run(true, true, toolName);
            if (optLightControlTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(optLightControlTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(optLightControlTool.toolRunStatu.ToString(), Color.Black );
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            optLightControlTool.controlMode = (comboBox1.SelectedIndex == 0 ? true : false);
        }

    }
}
