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
    internal partial class Frm_LLIntersectTool : Frm_FormBase
    {
        internal Frm_LLIntersectTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_LLIntersectTool _instance;
        public static Frm_LLIntersectTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_LLIntersectTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static LLIntersectTool llPointTool = new LLIntersectTool();


        internal void btn_drawShapeMatchSearchRegion_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Search_Region();
        }
        private void btn_deleteShapeMatchSearchRegion_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Clear_Search_Region();
        }
        private void dgv_matchResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //////shapeMatchTool.Click_Result_Dgv(e);
        }
        private void tkb_contrast_Scroll(object sender, EventArgs e)
        {
            //////shapeMatchTool.Contrast_Changed();
        }
   
        private void cbo_shapeMatchSearchRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Search_Region();
        }
        private void btn_displayStandardImage_Click(object sender, EventArgs e)
        {
            //////HOperatorSet.DispObj(shapeMatchTool.standardImage, Frm_ImageWindow.Instance.WindowHandle);
        }
        private void btn_displayTemplateContour_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.ShowTemplate();
        }
        private void ckb_shapeMatchToolNotRun_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_distancePLToolEnable.Checked;
        }
        private void btn_drawTemplateRegionRectangle1_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Template_Rectangle1();
        }
        private void btn_drawTemplateRegionRectangle2_Click(object sender, EventArgs e)
        {
            //////////shapeMatchTool.Draw_Template_Rectangle2();
        }
        private void btn_drawTemplateRegionCircle_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Template_Circle();
        }
        private void btn_drawTemplateRegionEllipse_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Template_Ellipse();
        }
        private void btn_drawTemplateRegionAny_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Template_Any();
        }
     
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

    }
}
