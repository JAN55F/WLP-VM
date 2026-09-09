using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_UpCamAlignTool : Frm_FormBase
    {
        internal Frm_UpCamAlignTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_UpCamAlignTool _instance;
        internal static Frm_UpCamAlignTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_UpCamAlignTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static UpCamAlignTool upCamAlignTool = new UpCamAlignTool();


        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        //private void cbx_toolEnable_CheckedChanged(object sender, EventArgs e)
        //{
        //    Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = cbx_toolEnable.Checked;
        //}
        private void tbx_pickPosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.X = Convert.ToDouble(tbx_pickPosX.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.Y = Convert.ToDouble(tbx_pickPosY.Text.Trim());
            }
            catch { }
        }


        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonDown;
            Application.DoEvents();
        }
        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }
        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.按钮__2_;
            Application.DoEvents();
        }
        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }
        private void tbx_pickPosU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].U = Convert.ToDouble(tbx_pickPosU.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.X = Convert.ToDouble(tbx_pickPosOffsetX.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.Y = Convert.ToDouble(tbx_pickPosOffsetY.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].U = Convert.ToDouble(tbx_pickPosOffsetU.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.X = Convert.ToDouble(tbx_featureX.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.Y = Convert.ToDouble(tbx_featureY.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].U = Convert.ToDouble(tbx_featureU.Text.Trim());
            }
            catch { }
        }
        private void btn_autoGet_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = true;
                button100.Image = Resources.钉;

                Job.RunAndWait(jobName);
                tbx_featureX.Value = upCamAlignTool.toolPar.InputPar.位置.Point.X.ToString();
                tbx_featureY.Value = upCamAlignTool.toolPar.InputPar.位置.Point.Y.ToString();
                tbx_featureU.Value = upCamAlignTool.toolPar.InputPar.位置.U.ToString();
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
                upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.X = Convert.ToDouble(tbx_saftyRangeX.Text.Trim());
            }
            catch { }
        }
        private void tbx_saftyRangeY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.Y = Convert.ToDouble(tbx_saftyRangeY.Text.Trim());
            }
            catch { }
        }
        private void tbx_saftyRangeU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].U = Convert.ToDouble(tbx_saftyRangeU.Text.Trim());
            }
            catch { }
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            upCamAlignTool.Run(true, true, toolName);
            if (upCamAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(upCamAlignTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(upCamAlignTool.toolRunStatu.ToString(), Color.Black);
            btn_runTool.Enabled = true;
        }
        private void btn_runTool_Click(object sender, EventArgs e)
        {

        }

        private void btn_setToPickPos_Click(object sender, EventArgs e)
        {
            try
            {
                tbx_pickPosX.Text = tbx_resultPosX.Text;
                tbx_pickPosY.Text = tbx_resultPosY.Text;
                tbx_pickPosU.Text = tbx_resultPosU.Text;
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

                cbx_toolList.Items.Add(input);
                cbx_toolList.Text = input;
                upCamAlignTool.L_toolName.Add(input);

                XYU featurePos = new XYU();
                upCamAlignTool.L_featurePos.Add(featurePos);

                XYU pickPos = new XYU();
                upCamAlignTool.L_pickPos.Add(pickPos);

                XYU pickPosOffset = new XYU();
                upCamAlignTool.L_pickPosOffset.Add(pickPosOffset);

                XYU safetyRange = new XYU();
                upCamAlignTool.L_safetyRange.Add(safetyRange);
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
                Frm_UpCamAlignTool.Instance.tbx_pickPosX.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.X.ToString();
                Frm_UpCamAlignTool.Instance.tbx_pickPosY.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.Y.ToString();
                Frm_UpCamAlignTool.Instance.tbx_pickPosU.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].U.ToString();

                Frm_UpCamAlignTool.Instance.tbx_featureX.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.X.ToString();
                Frm_UpCamAlignTool.Instance.tbx_featureY.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.Y.ToString();
                Frm_UpCamAlignTool.Instance.tbx_featureU.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].U.ToString();

                Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetX.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.X.ToString();
                Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetY.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.Y.ToString();
                Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetU.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].U.ToString();

                Frm_UpCamAlignTool.Instance.tbx_saftyRangeX.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.X.ToString();
                Frm_UpCamAlignTool.Instance.tbx_saftyRangeY.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.Y.ToString();
                Frm_UpCamAlignTool.Instance.tbx_saftyRangeU.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].U.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void cbx_toolEnable_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void tsb_runTool_Click_1(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            upCamAlignTool.Run(true, true, toolName);
            if (upCamAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(upCamAlignTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(upCamAlignTool.toolRunStatu.ToString(), Color.Black);
            btn_runTool.Enabled = true;
        }

        private void btn_runJob_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            button100.Image = Resources.钉;
            Job.RunAndWaitToCurrentTool(jobName, toolName);
            this.Focus();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cNumeric6_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.X = value;
            }
            catch { }
        }

        private void cNumeric5_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.Y = value;
            }
            catch { }
        }

        private void cNumeric4_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].U = value;
            }
            catch { }
        }

        private void cNumeric2_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.X =value ;
            }
            catch { }
        }

        private void cNumeric1_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.Y =value ;
            }
            catch { }
        }

        private void cNumeric3_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].U =value ;
            }
            catch { }
        }

        private void cNumericUpDown2_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.X = value ;
            }
            catch { }
        }

        private void cNumericUpDown1_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.Y = value ;
            }
            catch { }
        }

        private void cNumericUpDown3_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].U = value ;
            }
            catch { }
        }

        private void cNumeric9_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.X = value ;
            }
            catch { }
        }

        private void cNumeric8_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.Y = value;
            }
            catch { }
        }

        private void cNumeric7_ValueChanged(double value)
        {
            try
            {
                upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].U = value;
            }
            catch { }
        }

    }
}
