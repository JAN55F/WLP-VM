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
    internal partial class Frm_SubImageTool : Frm_FormBase
    {
        internal Frm_SubImageTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_SubImageTool _instance;
        public static Frm_SubImageTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_SubImageTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static SubImageTool subImageTool = new SubImageTool();


        private void btn_runImageSubTool_Click(object sender, EventArgs e)
        {
            btn_runImageSubTool.Enabled = false;
            subImageTool.Run( true ,true,toolName  );
            if (subImageTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(subImageTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(subImageTool.toolRunStatu.ToString(), Color.Black );
            btn_runImageSubTool.Enabled = true;
        }
        private void cbo_templateImageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            subImageTool.standardImageName = cbx_standardImage.Text;
        }
        private void ckb_subImageToolEnable_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable  = ckb_subImageToolEnable.Checked;
        }
      
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            btn_runImageSubTool.Enabled = false;
            subImageTool.Run(true, true, toolName);
            if (subImageTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(subImageTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(subImageTool.toolRunStatu.ToString(), Color.Black);
            btn_runImageSubTool.Enabled = true;
        }
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void btn_subResultImage_Click(object sender, EventArgs e)
        {
            subImageTool.ShowImage(subImageTool .inputImage   );
        }

    }
}
