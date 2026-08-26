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
    internal partial class Frm_PointAlignTool : Frm_FormBase
    {
        internal Frm_PointAlignTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_PointAlignTool _instance;
        internal static Frm_PointAlignTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_PointAlignTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static PointAlignTool pointAlignTool = new PointAlignTool();


        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void cbx_toolEnable_CheckedChanged(object sender, EventArgs e)
        {
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = cbx_toolEnable.Checked;
        }
        private void tbx_pickPosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.L_workPos[pointAlignTool.toolIdx].Point.X = Convert.ToDouble(tbx_pickPosX.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.L_workPos[pointAlignTool.toolIdx].Point.Y = Convert.ToDouble(tbx_pickPosY.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.L_workPosOffset[pointAlignTool.toolIdx].Point.X = Convert.ToDouble(tbx_pickPosOffsetX.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.L_workPosOffset[pointAlignTool.toolIdx].Point.Y = Convert.ToDouble(tbx_pickPosOffsetY.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.L_featurePos[pointAlignTool.toolIdx].Point.X = Convert.ToDouble(tbx_featureX.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.L_featurePos[pointAlignTool.toolIdx].Point.Y = Convert.ToDouble(tbx_featureY.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.L_featurePos[pointAlignTool.toolIdx].U = Convert.ToDouble(tbx_featureU.Text.Trim());
            }
            catch { }
        }
        private void btn_autoGet_Click(object sender, EventArgs e)
        {
            try
            {
                Job.RunAndWait(jobName);
                tbx_featureX.Text = pointAlignTool.inputPos.Point.X.ToString();
                tbx_featureY.Text = pointAlignTool.inputPos.Point.Y.ToString();
                tbx_featureU.Text = pointAlignTool.inputPos.U.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tbx_saftyRangeX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.L_safetyRange[pointAlignTool.toolIdx].Point.X = Convert.ToDouble(tbx_saftyRangeX.Text.Trim());
            }
            catch { }
        }
        private void tbx_saftyRangeY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.L_safetyRange[pointAlignTool.toolIdx].Point.Y = Convert.ToDouble(tbx_saftyRangeY.Text.Trim());
            }
            catch { }
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            tsb_runTool.Enabled = false;
            pointAlignTool.Run(true, true, toolName);
            if (pointAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(pointAlignTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(pointAlignTool.toolRunStatu.ToString(), Color.Green);
            tsb_runTool.Enabled = true;
        }
        private void btn_runTool_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            pointAlignTool.Run(true, true, toolName);
            if (pointAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(pointAlignTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(pointAlignTool.toolRunStatu.ToString(), Color.Black);
            btn_runTool.Enabled = true;
        }

        private void btn_setToPickPos_Click(object sender, EventArgs e)
        {
            try
            {
                tbx_pickPosX.Text = tbx_resultPosX.Text;
                tbx_pickPosY.Text = tbx_resultPosY.Text;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void btn_addTool_Click(object sender, EventArgs e)
        {
            try
            {
                Frm_InputMessage.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "Please input job's name" : "请输入工具名称");
                Frm_InputMessage.Instance.btn_confirm.Text = (Project.Instance.configuration.language == Language.English ? "Confirm" : "确定");
                Frm_InputMessage.Instance.passwordChar = false;
                Frm_InputMessage.Instance.txt_input.TextStr = string.Empty;
                Frm_InputMessage.Instance.ShowDialog();
                string input = Frm_InputMessage.input;
                if (input == string.Empty)
                    return;

                XYU featurePos = new XYU();
                pointAlignTool.L_featurePos.Add(featurePos);

                XYU pickPos = new XYU();
                pointAlignTool.L_workPos.Add(pickPos);

                XYU pickPosOffset = new XYU();
                pointAlignTool.L_workPosOffset.Add(pickPosOffset);

                XYU safetyRange = new XYU();
                pointAlignTool.L_safetyRange.Add(safetyRange);

                cbx_toolList.Items.Add(input);
                cbx_toolList.SelectedIndex = cbx_toolList.Items.Count - 1;
                pointAlignTool.L_toolName.Add(input);

           
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void cbx_toolList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pointAlignTool.toolIdx = cbx_toolList.SelectedIndex;

                Frm_PointAlignTool.Instance.tbx_pickPosX.Text = pointAlignTool.L_workPos[pointAlignTool.toolIdx].Point.X.ToString();
                Frm_PointAlignTool.Instance.tbx_pickPosY.Text = pointAlignTool.L_workPos[pointAlignTool.toolIdx].Point.Y.ToString();

                Frm_PointAlignTool.Instance.tbx_featureX.Text = pointAlignTool.L_featurePos[pointAlignTool.toolIdx].Point.X.ToString();
                Frm_PointAlignTool.Instance.tbx_featureY.Text = pointAlignTool.L_featurePos[pointAlignTool.toolIdx].Point.Y.ToString();
                Frm_PointAlignTool.Instance.tbx_featureU.Text = pointAlignTool.L_featurePos[pointAlignTool.toolIdx].U.ToString();

                Frm_PointAlignTool.Instance.tbx_pickPosOffsetX.Text = pointAlignTool.L_workPosOffset[pointAlignTool.toolIdx].Point.X.ToString();
                Frm_PointAlignTool.Instance.tbx_pickPosOffsetY.Text = pointAlignTool.L_workPosOffset[pointAlignTool.toolIdx].Point.Y.ToString();

                Frm_PointAlignTool.Instance.tbx_saftyRangeX.Text = pointAlignTool.L_safetyRange[pointAlignTool.toolIdx].Point.X.ToString();
                Frm_PointAlignTool.Instance.tbx_saftyRangeY.Text = pointAlignTool.L_safetyRange[pointAlignTool.toolIdx].Point.Y.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void btn_removeTool_Click(object sender, EventArgs e)
        {
            if (cbx_toolList.Text == string.Empty)
                return;
            pointAlignTool.L_toolName.Remove(cbx_toolList .Text );

            int idx = cbx_toolList.SelectedIndex;
            cbx_toolList.Items.RemoveAt(idx);
            if (cbx_toolList.Items.Count == idx)
                cbx_toolList.SelectedIndex = idx - 1;
            else
                cbx_toolList.SelectedIndex = idx;



            pointAlignTool.L_featurePos.RemoveAt(idx);


            pointAlignTool.L_workPos.RemoveAt(idx);


            pointAlignTool.L_workPosOffset.RemoveAt(idx);


            pointAlignTool.L_safetyRange.RemoveAt(idx);
        }

    }
}
