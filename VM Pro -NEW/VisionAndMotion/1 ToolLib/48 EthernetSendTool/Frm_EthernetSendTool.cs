using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_EthernetSendTool : Frm_FormBase
    {
        internal Frm_EthernetSendTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 工具对象
        /// </summary>
        internal static EthernetSendTool ethernetReceiveTool = new EthernetSendTool();
        /// <summary>
        /// ROI区域
        /// </summary>
        internal List<ViewWindow.Model.ROI> L_regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_EthernetSendTool _instance;
        public static Frm_EthernetSendTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_EthernetSendTool();
                return _instance;
            }
        }



        internal override void btn_baseClose_Click(object sender, EventArgs e)
        {
            try
            {
                base.btn_baseClose_Click(sender, e);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = true;
            this.pic_onOff.Image = Resources.开;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pic_onOff_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = !enable;
            if (!enable)
                pic_onOff.Image = Resources.开;
            else
                pic_onOff.Image = Resources.关;
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                ethernetReceiveTool.Run(true, true, toolName);
                long time = sw.ElapsedMilliseconds;

                if (ethernetReceiveTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                {
                    lbl_toolTip.ForeColor = Color.Red;
                    lbl_runTime.Text = string.Format("耗时：0ms");
                }
                else
                {
                    lbl_toolTip.ForeColor = Color.Black;
                    lbl_runTime.Text = string.Format("耗时：{0}ms", time.ToString());
                }
                lbl_toolTip.Text = "状态：" + ethernetReceiveTool.toolRunStatu.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void comboBox1_SelectedIndexChanged()
        {
            ethernetReceiveTool.EthernetName = comboBox1222.TextStr;
        }

        private void tbx_imageSavePath_TextStrChanged(string textStr)
        {
            ethernetReceiveTool.toolPar.InputPar.消息 = tbx_imageSavePath.TextStr;
        }

        private void btn_endCharNone_Click(object sender, EventArgs e)
        {
            ethernetReceiveTool.endChar = string.Empty;
            btn_endCharNone.BackColor = Color.Gray;
            btn_endCharEnter.BackColor = Color.Gainsboro;
        }

        private void btn_endCharEnter_Click(object sender, EventArgs e)
        {
            ethernetReceiveTool.endChar = "\r\n";
            btn_endCharNone.BackColor = Color.Gainsboro;
            btn_endCharEnter.BackColor = Color.Gray;
        }


    }
}
