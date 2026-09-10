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
    internal partial class Frm_XYPlatformTool : Frm_FormBase
    {
        internal Frm_XYPlatformTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_XYPlatformTool _instance;
        public static Frm_XYPlatformTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_XYPlatformTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static XYPlatformTool xyPlatformTool = new XYPlatformTool();


        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.ResetTool();
        }
        private void btn_runShapeMatchTool_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            xyPlatformTool.Run(true, true, toolName);
            if (xyPlatformTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(xyPlatformTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(xyPlatformTool.toolRunStatu.ToString(), Color.Green);
            btn_runTool.Enabled = true;
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            xyPlatformTool.Run(true, true, toolName);
            if (xyPlatformTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(xyPlatformTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(xyPlatformTool.toolRunStatu.ToString(), Color.Green);
            btn_runTool.Enabled = true;
        }


        private void btn_autoGet_Click(object sender, EventArgs e)
        {
            try
            {
                Job.RunAndWait(jobName);
                tbx_featureX.Text = xyPlatformTool.inputPos.Point.X.ToString();
                tbx_featureY.Text = xyPlatformTool.inputPos.Point.Y.ToString();
                tbx_featureU.Text = xyPlatformTool.inputPos.U.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tbx_pickPosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                xyPlatformTool.pickPos.Point.X = Convert.ToDouble(tbx_pickPosX.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                xyPlatformTool.pickPos.Point.Y = Convert.ToDouble(tbx_pickPosY.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                xyPlatformTool.featurePos.Point.X = Convert.ToDouble(tbx_featureX.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                xyPlatformTool.featurePos.Point.Y = Convert.ToDouble(tbx_featureY.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                xyPlatformTool.pickPos.U = Convert.ToDouble(tbx_pickPosU.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                xyPlatformTool.pickPosOffset.Point.X = Convert.ToDouble(tbx_pickPosOffsetX.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                xyPlatformTool.pickPosOffset.Point.Y = Convert.ToDouble(tbx_pickPosOffsetY.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                xyPlatformTool.pickPosOffset.U = Convert.ToDouble(tbx_pickPosOffsetU.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                xyPlatformTool.featurePos.U = Convert.ToDouble(tbx_featureU.Text.Trim());
            }
            catch { }
        }

    }
}
