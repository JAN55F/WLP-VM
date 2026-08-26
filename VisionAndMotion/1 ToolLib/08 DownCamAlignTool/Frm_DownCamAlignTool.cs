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
    internal partial class Frm_DownCamAlignTool : Frm_FormBase
    {
        internal Frm_DownCamAlignTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_DownCamAlignTool _instance;
        internal static Frm_DownCamAlignTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_DownCamAlignTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static DownCamAlignTool robotDownCamAlignTool = new DownCamAlignTool();


        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            robotDownCamAlignTool.ResetTool();
        }
        private void cbx_toolEnable_CheckedChanged(object sender, EventArgs e)
        {
      Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable    = cbx_toolEnable.Checked;
        }
        private void tbs_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }
        private void tbx_photoPosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                robotDownCamAlignTool.photoPos.Point.X = Convert.ToDouble(tbx_photoPosX.Text.Trim());
            }
            catch { }
        }
        private void tbx_photoPosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                robotDownCamAlignTool.photoPos.Point.Y = Convert.ToDouble(tbx_photoPosY.Text.Trim());
            }
            catch { }
        }
        private void tbx_photoPosU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                robotDownCamAlignTool.photoPos.U = Convert.ToDouble(tbx_photoPosU.Text.Trim());
            }
            catch { }
        }
        private void tbx_placePosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                robotDownCamAlignTool.placePos.Point.X = Convert.ToDouble(tbx_placePosX.Text.Trim());
            }
            catch { }
        }
        private void tbx_placePosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                robotDownCamAlignTool.placePos.Point.Y = Convert.ToDouble(tbx_placePosY.Text.Trim());
            }
            catch { }
        }
        private void tbx_placePosU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                robotDownCamAlignTool.placePos.U = Convert.ToDouble(tbx_placePosU.Text.Trim());
            }
            catch { }
        }
        private void tbx_featurePosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                robotDownCamAlignTool.featurePos.Point.X = Convert.ToDouble(tbx_featurePosX.Text.Trim());
            }
            catch { }
        }
        private void tbx_featurePosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                robotDownCamAlignTool.featurePos.Point.Y = Convert.ToDouble(tbx_featurePosY.Text.Trim());
            }
            catch { }
        }
        private void tbx_featurePosU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                robotDownCamAlignTool.featurePos.U = Convert.ToDouble(tbx_featurePosU.Text.Trim());
            }
            catch { }
        }
        private void btn_autoGet_Click(object sender, EventArgs e)
        {
            try
            {
                Job.RunAndWait(jobName);
                tbx_featurePosX.Text = robotDownCamAlignTool.inputPos.Point.X.ToString();
                tbx_featurePosY.Text = robotDownCamAlignTool.inputPos.Point.Y.ToString();
                tbx_featurePosU.Text = robotDownCamAlignTool.inputPos.U.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            tsb_runTool.Enabled = false;
            robotDownCamAlignTool.Run(true, true, toolName);
            if (robotDownCamAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(robotDownCamAlignTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(robotDownCamAlignTool.toolRunStatu.ToString(), Color.Green);
            tsb_runTool.Enabled = true;
        }
        private void btn_runTool_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            robotDownCamAlignTool.Run(true, true, toolName);
            if (robotDownCamAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(robotDownCamAlignTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(robotDownCamAlignTool.toolRunStatu.ToString(), Color.Black);
            btn_runTool.Enabled = true;
        }

    }
}
