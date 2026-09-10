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
    internal partial class Frm_ColorToRGBTool : Frm_FormBase 
    {
        internal Frm_ColorToRGBTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_ColorToRGBTool _instance;
        public static Frm_ColorToRGBTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ColorToRGBTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static ColorToRGBTool colorToRGBTool = new ColorToRGBTool();


        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            colorToRGBTool.ResetTool();
        }
        private void ckb_colorToRGBToolEnable_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_colorToRGBToolEnable.Checked;
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            colorToRGBTool.Run(true, true, toolName);
            if (colorToRGBTool.toolRunStatu  != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(colorToRGBTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(colorToRGBTool.toolRunStatu.ToString(), Color.Black);
        }
        private void btn_runColorToRGBTool_Click(object sender, EventArgs e)
        {
            colorToRGBTool.Run(true, true, toolName);
            if (colorToRGBTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(colorToRGBTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(colorToRGBTool.toolRunStatu.ToString(), Color.Green);
        }
       
    }
}
