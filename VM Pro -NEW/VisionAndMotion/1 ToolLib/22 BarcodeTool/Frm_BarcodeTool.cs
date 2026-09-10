using HalconDotNet;
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
    public partial class Frm_BarcodeTool : Frm_FormBase
    {
        public Frm_BarcodeTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_BarcodeTool _instance;
        public static Frm_BarcodeTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_BarcodeTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static BarcodeTool barcodeTool = new BarcodeTool();


        private void dgv_barcordFindResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            barcodeTool.Click_Result_Dgv(e);
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            tsb_runTool.Enabled = false;
            barcodeTool.Run(true, true, toolName);
            if (barcodeTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(barcodeTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(barcodeTool.toolRunStatu.ToString(), Color.Green);
            tsb_runTool.Enabled = true;
        }
        private void btn_runFindBarcodeTool_Click(object sender, EventArgs e)
        {
            btn_runFindBarcodeTool.Enabled = false;
            barcodeTool.Run(true, true, toolName);
            if (barcodeTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(barcodeTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(barcodeTool.toolRunStatu.ToString(), Color.Black );
            btn_runFindBarcodeTool.Enabled = true;
        }
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void tsb_help_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void ckb_barcodeToolEnable_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable  = ckb_barcodeToolEnable.Checked;
        }
        private void btn_barcodeSearchRegionRectangle1_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void rdo_barcodeSearchRegionRectangle2_CheckedChanged(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void rdo_barcodeSearchRegionCircle_CheckedChanged(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void rdo_barcodeSearchRegionEllipse_CheckedChanged(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void nud_findCount_ValueChanged(object sender, EventArgs e)
        {
            barcodeTool.findNum = Convert.ToInt16(nud_findCount .Value );
        }
        private void tbx_minContrast_TextChanged(object sender, EventArgs e)
        {
            barcodeTool.minContrast = Convert.ToDouble(tbx_minContrast .Text .Trim ());
        }
        private void rdo_diplayResultStr_CheckedChanged(object sender, EventArgs e)
        {
            barcodeTool.showResultStr = rdo_showResultStr.Checked;
        }
        private void rdo_displayArrow_CheckedChanged(object sender, EventArgs e)
        {
            barcodeTool.showArrow = rdo_showArrow.Checked;
        }

    }
}
