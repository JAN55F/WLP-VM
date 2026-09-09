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
    internal partial class Frm_MarkTool : Frm_FormBase
    {
        internal Frm_MarkTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_MarkTool _instance;
        public static Frm_MarkTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_MarkTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static MarkTool markTool = new MarkTool();


        private void ckb_shapeMatchToolNotRun_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_shapeMatchToolEnable.Checked;
        }

     
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            //////markTool.ResetTool();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }
        private void btn_runShapeMatchTool_Click(object sender, EventArgs e)
        {
            //////btn_runShapeMatchTool.Enabled = false;
            //////shapeMatchTool.Run( jobName,true ,true );
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runShapeMatchTool.Enabled = true;
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            //////btn_runShapeMatchTool.Enabled = false;
            //////shapeMatchTool.Run( jobName,true ,true );
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runShapeMatchTool.Enabled = true;
        }

        private void tbx_caputurePosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                markTool.inputPoint.X = Convert.ToDouble(tbx_caputurePosX.Text.Trim());
            }
            catch { }
        }

        private void tbx_caputurePosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                markTool.inputPoint.Y  = Convert.ToDouble(tbx_caputurePosY.Text.Trim());
            }
            catch { }
        }

    }
}
