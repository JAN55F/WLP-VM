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
    internal partial class Frm_DataAnalyseTool : Frm_FormBase
    {
        internal Frm_DataAnalyseTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 工具对象
        /// </summary>
        internal static DataAnalyseTool dataAnalyseTool = new DataAnalyseTool();
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_DataAnalyseTool _instance;
        public static Frm_DataAnalyseTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_DataAnalyseTool();
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
                dataAnalyseTool.Run(true, true, toolName);
                long time = sw.ElapsedMilliseconds;

                if (dataAnalyseTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                {
                    lbl_toolTip.ForeColor = Color.Red;
                    lbl_runTime.Text = string.Format("耗时：0ms");
                }
                else
                {
                    lbl_toolTip.ForeColor = Color.Black;
                    lbl_runTime.Text = string.Format("耗时：{0}ms", time.ToString());
                }
                lbl_toolTip.Text = "状态：" + dataAnalyseTool.toolRunStatu.ToString();
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

        private void dgv_outputItem_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (Job.loadForm)
                    return;

                dataAnalyseTool.L_items.Clear();
                for (int i = 0; i < dgv_outputItem.Rows.Count - 1; i++)
                {
                    if (dgv_outputItem.Rows[i].Cells[1].Value != null &&
                        dgv_outputItem.Rows[i].Cells[2].Value != null &&
                        dgv_outputItem.Rows[i].Cells[3].Value != null &&
                        dgv_outputItem.Rows[i].Cells[4].Value != null)
                    {
                        if (dgv_outputItem.Rows[i].Cells[0].Value == null)
                            continue;
                        sDataType dataType = new sDataType();
                        dataType.inputItem = dgv_outputItem.Rows[i].Cells[0].Value.ToString();
                        dataType.downLimit = Convert.ToDouble(dgv_outputItem.Rows[i].Cells[1].Value);
                        dataType.upLimit = Convert.ToDouble(dgv_outputItem.Rows[i].Cells[2].Value);
                        dataType.inResult = dgv_outputItem.Rows[i].Cells[3].Value.ToString();
                        dataType.outResult = dgv_outputItem.Rows[i].Cells[4].Value.ToString();
                        dataAnalyseTool.L_items.Add(dataType);
                    }
                }


            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }





    }
}
