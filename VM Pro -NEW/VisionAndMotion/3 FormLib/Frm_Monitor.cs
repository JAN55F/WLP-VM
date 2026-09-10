using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace VMPro
{
    internal partial class Frm_Monitor : DockContent
    {
        internal Frm_Monitor()
        {
            InitializeComponent();
            Init_Language();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_Monitor _instance;
        public static Frm_Monitor Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_Monitor();
                return _instance;
            }
        }


        /// <summary>
        /// 初始化语言
        /// </summary>
        private void Init_Language()
        {
            try
            {
                if (Project .Instance .configuration .language == Language.English)
                {
                    this.Text = "Value Monitor";
                    Column1.HeaderText = "ItemName";
                    Column2.HeaderText = "ItemValue";
                }
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        private void Frm_Monitor_Load(object sender, EventArgs e)
        {
            try
            {
                if (Frm_Job.Instance.tbc_jobs.TabCount == 0)
                    return;
                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                DataGridViewComboBoxCell outputItemCell = (DataGridViewComboBoxCell)(this.dgv_monitor.Rows[0].Cells[0]);
                for (int i = 0; i < Job.FindJobByName(jobName).L_toolList.Count; i++)
                {
                    if (Job.FindJobByName(jobName).L_toolList[i].toolType == ToolType.Output)
                    {
                        int resultCount = Job.FindJobByName(jobName).L_toolList[i].input.Count;
                        outputItemCell.Items.Clear();
                        for (int j = 0; j < resultCount; j++)
                        {
                            outputItemCell.Items.Add(Job.FindJobByName(jobName).L_toolList[i].input [j].IOName .Substring(3));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < this.dgv_monitor.Rows.Count - 1; i++)
                {
                    DataGridViewComboBoxCell outputItemCell = (DataGridViewComboBoxCell)(this.dgv_monitor.Rows[i].Cells[0]);
                    string toolNameAndOutputItem = (outputItemCell.EditedFormattedValue.ToString());
                    if (toolNameAndOutputItem == "")
                        return;
                    string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                    Job job = Job.FindJobByName(jobName);
                    ToolInfo info = new ToolInfo();
                    for (int j = 0; j < job.L_toolList.Count; j++)
                    {
                        if (job.L_toolList[j].toolType == ToolType.Output)
                        {
                            info = job.L_toolList[j];
                        }
                    }
                    string result = info.GetInput ("<--" + toolNameAndOutputItem).value .ToString();
                    this.dgv_monitor.Rows[i].Cells[1].Value = result;
                }
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        private void Frm_Monitor_Resize(object sender, EventArgs e)
        {
            this.dgv_monitor.Columns[1].Width = Convert.ToInt16(this.Size.Width - 319);
        }
        private void dgv_monitor_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                if (Frm_Job.Instance.tbc_jobs.TabCount > 0)
                {
                    string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                    DataGridViewComboBoxCell outputItemCell = (DataGridViewComboBoxCell)(this.dgv_monitor.Rows[this.dgv_monitor.Rows.Count - 1].Cells[0]);
                    for (int i = 0; i < Job.FindJobByName(jobName).L_toolList.Count; i++)
                    {
                        if (Job.FindJobByName(jobName).L_toolList[i].toolType == ToolType.Output)
                        {
                            int resultCount = Job.FindJobByName(jobName).L_toolList[i].input.Count;
                            outputItemCell.Items.Clear();
                            for (int j = 0; j < resultCount; j++)
                            {
                                outputItemCell.Items.Add(Job.FindJobByName(jobName).L_toolList[i].input [j].IOName .Substring(3));
                            }
                        }
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
