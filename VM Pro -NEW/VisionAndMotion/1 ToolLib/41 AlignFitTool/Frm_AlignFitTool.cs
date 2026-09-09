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
    internal partial class Frm_AlignFitTool : Frm_FormBase
    {
        internal Frm_AlignFitTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_AlignFitTool _instance;
        internal static Frm_AlignFitTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_AlignFitTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static AlignFitTool alignFitTool = new AlignFitTool();


    
        private void tbx_pickPosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateElementPlacePos.Point.X = Convert.ToDouble(tbx_pickPosX.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateElementPlacePos.Point.Y = Convert.ToDouble(tbx_pickPosY.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateElementPlacePos.U = Convert.ToDouble(tbx_pickPosU.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateElementPlacePosOffset.Point.X = Convert.ToDouble(tbx_pickPosOffsetX.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateElementPlacePosOffset.Point.Y = Convert.ToDouble(tbx_pickPosOffsetY.Text.Trim());
            }
            catch { }
        }
        private void tbx_pickPosOffsetU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateElementPlacePosOffset.U = Convert.ToDouble(tbx_pickPosOffsetU.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateBelowBoardPos.Point.X = Convert.ToDouble(tbx_featureX.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateBelowBoardPos.Point.Y = Convert.ToDouble(tbx_featureY.Text.Trim());
            }
            catch { }
        }
        private void tbx_featureU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateBelowBoardPos.U = Convert.ToDouble(tbx_featureU.Text.Trim());
            }
            catch { }
        }
        private void btn_autoGet_Click(object sender, EventArgs e)
        {
            try
            {
                Job.RunAndWait(jobName);
                tbx_featureX.Text = alignFitTool.inputPos.Point.X.ToString();
                tbx_featureY.Text = alignFitTool.inputPos.Point.Y.ToString();
                tbx_featureU.Text = alignFitTool.inputPos.U.ToString();
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
                alignFitTool.L_safetyRange.Point.X = Convert.ToDouble(tbx_saftyRangeX.Text.Trim());
            }
            catch { }
        }
        private void tbx_saftyRangeY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.L_safetyRange.Point.Y = Convert.ToDouble(tbx_saftyRangeY.Text.Trim());
            }
            catch { }
        }
        private void tbx_saftyRangeU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.L_safetyRange.U = Convert.ToDouble(tbx_saftyRangeU.Text.Trim());
            }
            catch { }
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

        
     

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Enabled = false;
            alignFitTool.Run(true, true, toolName);
            if (alignFitTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(alignFitTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(alignFitTool.toolRunStatu.ToString(), Color.Black);
            button5.Enabled = true;
        }

        private void cbx_toolEnable_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateElementPos.Point.X = Convert.ToDouble(textBox2.Text.Trim());
            }
            catch { }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateElementPos.Point.Y = Convert.ToDouble(textBox3.Text.Trim());
            }
            catch { }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alignFitTool.TemplateElementPos.Point.X = Convert.ToDouble(textBox2.Text.Trim());
            }
            catch { }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = !enable;
            if (!enable)
                pictureBox2.Image = Resources.开;
            else
                pictureBox2.Image = Resources.关;
        }

   
     

    

    

    }
}
