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
    internal partial class Frm_DistancePLTool : Frm_FormBase
    {
        internal Frm_DistancePLTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_DistancePLTool _instance;
        public static Frm_DistancePLTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_DistancePLTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static MatchTool shapeMatchTool = new MatchTool();


        internal void btn_drawShapeMatchSearchRegion_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawSearchRegion();
        }
        private void btn_deleteShapeMatchSearchRegion_Click(object sender, EventArgs e)
        {
            shapeMatchTool.ClearSearchRegion();
        }
        private void dgv_matchResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            shapeMatchTool.ClickResultDgv(e);
        }
        private void tkb_contrast_Scroll(object sender, EventArgs e)
        {
            shapeMatchTool.ContrastChanged();
        }
   
        private void cbo_shapeMatchSearchRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            shapeMatchTool.DrawSearchRegion();
        }
        private void btn_displayStandardImage_Click(object sender, EventArgs e)
        {
            shapeMatchTool.ShowStandardImage();
        }
        private void btn_displayTemplateContour_Click(object sender, EventArgs e)
        {
            shapeMatchTool.ShowTemplate();
        }
        private void ckb_shapeMatchToolNotRun_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_distancePLToolEnable.Checked;
        }
        private void btn_drawTemplateRegionRectangle1_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateRectangle1();
        }
        private void btn_drawTemplateRegionRectangle2_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateRectangle2();
        }
        private void btn_drawTemplateRegionCircle_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateCircle();
        }
        private void btn_drawTemplateRegionEllipse_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateEllipse();
        }
        private void btn_drawTemplateRegionAny_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateAny();
        }
    
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            shapeMatchTool.ResetTool();
        }
      
        private void btn_runShapeMatchTool_Click(object sender, EventArgs e)
        {
            btn_runDistancePLTool.Enabled = false;
            shapeMatchTool.Run(false, false, toolName);
            if (shapeMatchTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(shapeMatchTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(shapeMatchTool.toolRunStatu.ToString(), Color.Black);
            btn_runDistancePLTool.Enabled = true;
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            btn_runDistancePLTool.Enabled = false;
            shapeMatchTool.Run(false, false, toolName);
            if (shapeMatchTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(shapeMatchTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(shapeMatchTool.toolRunStatu.ToString(), Color.Black);
            btn_runDistancePLTool.Enabled = true;
        }

    }
}
