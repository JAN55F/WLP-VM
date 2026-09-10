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
    internal partial class Frm_BuChangTool : Frm_FormBase
    {
        internal Frm_BuChangTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_BuChangTool _instance;
        public static Frm_BuChangTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_BuChangTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static BuChangTool buChangTool = new BuChangTool();



      
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.ResetTool();
        }

        private void btn_runShapeMatchTool_Click(object sender, EventArgs e)
        {
            //////btn_runDistancePLTool.Enabled = false;
            //////shapeMatchTool.Run(true, jobName);
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runDistancePLTool.Enabled = true;
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            //////btn_runDistancePLTool.Enabled = false;
            //////shapeMatchTool.Run(true, jobName);
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runDistancePLTool.Enabled = true;
        }

        private void tbx_caputurePosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                buChangTool.templatePos.Point.X = Convert.ToDouble(tbx_caputurePosX.Text.Trim());
            }
            catch { }
        }

        private void tbx_caputurePosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                buChangTool.templatePos.Point.Y = Convert.ToDouble(tbx_caputurePosY.Text.Trim());
            }
            catch { }
        }

        private void tbx_caputurePosU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                buChangTool.templatePos.U = Convert.ToDouble(tbx_caputurePosU.Text.Trim());
            }
            catch { }
        }

      


        private void btn_autoGet_Click(object sender, EventArgs e)
        {
            Job.RunAndWait(jobName);
            tbx_caputurePosX.Text = buChangTool.inputPos.Point.X.ToString();
            tbx_caputurePosY.Text = buChangTool.inputPos.Point.Y.ToString();
            tbx_caputurePosU.Text = buChangTool.inputPos.U.ToString();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void tbx_pickPosOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                buChangTool.buchang.Point.X = Convert.ToDouble(tbx_pickPosOffsetX.Text.Trim());
            }
            catch { }
        }

        private void tbx_pickPosOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                buChangTool.buchang.Point.Y = Convert.ToDouble(tbx_pickPosOffsetY.Text.Trim());
            }
            catch { }
        }

        private void tbx_pickPosOffsetU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                buChangTool.buchang.U  = Convert.ToDouble(tbx_pickPosOffsetU.Text.Trim());
            }
            catch { }
        }

        private void tbx_pickPosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                buChangTool.workPos.Point.X = Convert.ToDouble(tbx_pickPosX.Text.Trim());
            }
            catch 
            {
                
            }
        }

        private void tbx_pickPosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                buChangTool.workPos.Point.Y = Convert.ToDouble(tbx_pickPosY.Text.Trim());
            }
            catch
            {

            }
        }

        private void tbx_pickPosU_TextChanged(object sender, EventArgs e)
        {
            try
            {
                buChangTool.workPos.U = Convert.ToDouble(tbx_pickPosU.Text.Trim());
            }
            catch
            {

            }
        }
    }
}
