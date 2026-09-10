using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HalconDotNet;
using Tool;

namespace VMPro
{
    internal partial class Frm_ComConfig : Frm_FormBase 
    {
        internal Frm_ComConfig()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_ComConfig _instance;
        public static Frm_ComConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ComConfig();
                return _instance;
            }
        }

       
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgv_comConfig.CurrentCell.ColumnIndex == 2)
                {
                    DataGridViewComboBoxCell jobNameCell = (DataGridViewComboBoxCell)(this.dgv_comConfig.Rows[dgv_comConfig.CurrentCell.RowIndex].Cells[dgv_comConfig.CurrentCell.ColumnIndex]);
                    string jobName = (jobNameCell.EditedFormattedValue.ToString());
                    DataGridViewComboBoxCell outputItemCell = (DataGridViewComboBoxCell)(this.dgv_comConfig.Rows[dgv_comConfig.CurrentCell.RowIndex].Cells[dgv_comConfig.CurrentCell.ColumnIndex + 1]);
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
        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                Project .Instance .configuration .L_communicationItemList.Clear();
                for (int i = 0; i < dgv_comConfig.Rows.Count - 1; i++)
                {
                    DataGridViewTextBoxCell receiveStrCell = (DataGridViewTextBoxCell)(this.dgv_comConfig.Rows[i].Cells[1]);
                    string receiveStr = (receiveStrCell.EditedFormattedValue.ToString());

                    DataGridViewComboBoxCell jobNameCell = (DataGridViewComboBoxCell)(this.dgv_comConfig.Rows[i].Cells[2]);
                    string jobName = (jobNameCell.EditedFormattedValue.ToString());

                    DataGridViewComboBoxCell outputItemCell = (DataGridViewComboBoxCell)(this.dgv_comConfig.Rows[i].Cells[3]);
                    string outputItem = (outputItemCell.EditedFormattedValue.ToString());

                    DataGridViewTextBoxCell ngRespondCell = (DataGridViewTextBoxCell)(this.dgv_comConfig.Rows[i].Cells[4]);
                    string ngRespond = (ngRespondCell.EditedFormattedValue.ToString());

                    DataGridViewTextBoxCell addValueCell = (DataGridViewTextBoxCell)(this.dgv_comConfig.Rows[i].Cells[5]);
                    string addValue = (addValueCell.EditedFormattedValue.ToString());

                    DataGridViewTextBoxCell suffixStrCell = (DataGridViewTextBoxCell)(this.dgv_comConfig.Rows[i].Cells[6]);
                    string suffixStrValue = (suffixStrCell.EditedFormattedValue.ToString());

                    CommConfigItem commConfig = new CommConfigItem();
                    commConfig.ReceivedCommand = receiveStr;
                    commConfig.JobName = jobName;
                    commConfig.OutputItem = outputItem;
                    commConfig.NGRespond = ngRespond;
                    commConfig.PrefixStr = addValue;
                    commConfig.suffixStr = suffixStrValue;
                    Project .Instance .configuration .L_communicationItemList.Add(commConfig);
                }
                switch (cbx_communcationType.SelectedIndex)
                {
                    case 0:
                        Project .Instance .configuration .communicationType = CommunicationType.None;
                        break;
                    case 1:
                        Project .Instance .configuration .communicationType = CommunicationType.Internet_Client;
                        break;
                    case 2:
                        Project .Instance .configuration .communicationType = CommunicationType.Internet_Sever;
                        break;
                    case 3:
                        Project .Instance .configuration .communicationType = CommunicationType.SerialPort;
                        break;
                    case 4:
                        Project .Instance .configuration .communicationType = CommunicationType.IO;
                        break;
                }
                Frm_MessageBox.Instance.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Save successfully" : "\r\n保存成功");
                this.Close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_deleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_comConfig.CurrentRow == null || dgv_comConfig.Rows.Count == 0)
                    return;
                string receivedStr = dgv_comConfig.CurrentRow.Cells[1].Value.ToString();
                for (int i = 0; i < Project .Instance .configuration .L_communicationItemList.Count; i++)
                {
                    if (Project .Instance .configuration .L_communicationItemList[i].ReceivedCommand == receivedStr)
                    {
                        Project .Instance .configuration .L_communicationItemList.RemoveAt(i);
                    }
                }
                dgv_comConfig.Rows.RemoveAt(dgv_comConfig.CurrentRow.Index);
                //序号重新排序
                for (int i = 0; i < dgv_comConfig.Rows.Count; i++)
                {
                    dgv_comConfig.Rows[i].Cells[0].Value = i + 1;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_ComConfig_Load(object sender, EventArgs e)
        {
            cbx_communcationType.SelectedIndex = Convert.ToInt16(Project.Instance.configuration.communicationType);
        }
        private void Frm_ComConfig_Shown(object sender, EventArgs e)
        {
            try
            {
                if (dgv_comConfig.Rows.Count == 1)
                {
                    dgv_comConfig.Rows[dgv_comConfig.Rows.Count - 1].Cells[0].Value = (dgv_comConfig.Rows.Count);
                    DataGridViewComboBoxCell jobCell = (DataGridViewComboBoxCell)(this.dgv_comConfig.Rows[dgv_comConfig.Rows.Count - 1].Cells[2]);
                    for (int i = 0; i < Frm_Job.Instance.tbc_jobs.TabPages.Count; i++)
                    {
                        if (!jobCell.Items.Contains(Frm_Job.Instance.tbc_jobs.TabPages[i].Text))
                            jobCell.Items.Add(Frm_Job.Instance.tbc_jobs.TabPages[i].Text);
                    }
                }
                if (Project.Instance.configuration.L_communicationItemList.Count <= 0)
                    return;
                dgv_comConfig.Rows.Clear();
                dgv_comConfig.Rows.Add(Project.Instance.configuration.L_communicationItemList.Count);
                for (int i = 0; i < Project.Instance.configuration.L_communicationItemList.Count; i++)
                {
                    string receiveStr = Project.Instance.configuration.L_communicationItemList[i].ReceivedCommand;
                    string jobName = Project.Instance.configuration.L_communicationItemList[i].JobName;
                    string outputItem = Project.Instance.configuration.L_communicationItemList[i].OutputItem;
                    string ngRespond = Project.Instance.configuration.L_communicationItemList[i].NGRespond ;
                    string addValue = Project.Instance.configuration.L_communicationItemList[i].PrefixStr;
                    string suffixValue = Project.Instance.configuration.L_communicationItemList[i].suffixStr;
                    this.dgv_comConfig.Rows[i].Cells[0].Value = i + 1;

                    DataGridViewTextBoxCell receiveStrCell = (DataGridViewTextBoxCell)(this.dgv_comConfig.Rows[i].Cells[1]);
                    receiveStrCell.Value = receiveStr;

                    DataGridViewComboBoxCell jobNameCell = (DataGridViewComboBoxCell)(this.dgv_comConfig.Rows[i].Cells[2]);
                    if (!jobNameCell.Items.Contains(jobName))
                    {
                        jobNameCell.Items.Add(jobName);
                    }
                    jobNameCell.Value = jobName;

                    DataGridViewComboBoxCell outputItemCell = (DataGridViewComboBoxCell)(this.dgv_comConfig.Rows[i].Cells[3]);
                    outputItemCell.Items.Add(outputItem);
                    outputItemCell.Value = outputItem;

                    DataGridViewTextBoxCell ngRespondCell = (DataGridViewTextBoxCell)(this.dgv_comConfig.Rows[i].Cells[4]);
                    ngRespondCell.Value = ngRespond;

                    DataGridViewTextBoxCell addStrCell = (DataGridViewTextBoxCell)(this.dgv_comConfig.Rows[i].Cells[5]);
                    addStrCell.Value = addValue.Substring(0, addValue.Length);

                    DataGridViewTextBoxCell suffixStrCell = (DataGridViewTextBoxCell)(this.dgv_comConfig.Rows[i].Cells[6]);
                    suffixStrCell.Value = suffixValue;
                }
                for (int i = 0; i < this.dgv_comConfig.Rows.Count; i++)
                {
                    DataGridViewComboBoxCell jobCell = (DataGridViewComboBoxCell)(this.dgv_comConfig.Rows[i].Cells[2]);
                    for (int j = 0; j < Frm_Job.Instance.tbc_jobs.TabPages.Count; j++)
                    {
                        if (!jobCell.Items.Contains(Frm_Job.Instance.tbc_jobs.TabPages[j].Text))
                            jobCell.Items.Add(Frm_Job.Instance.tbc_jobs.TabPages[j].Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void dgv_comConfig_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                dgv_comConfig.Rows[dgv_comConfig.Rows.Count - 1].Cells[0].Value = (dgv_comConfig.Rows.Count);
                DataGridViewComboBoxCell jobCell = (DataGridViewComboBoxCell)(this.dgv_comConfig.Rows[dgv_comConfig.Rows.Count - 1].Cells[2]);
                for (int i = 0; i < Frm_Job.Instance.tbc_jobs.TabPages.Count; i++)
                {
                    if (!jobCell.Items.Contains(Frm_Job.Instance.tbc_jobs.TabPages[i].Text))
                        jobCell.Items.Add(Frm_Job.Instance.tbc_jobs.TabPages[i].Text);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void dgv_comConfig_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                //判断相应的列
                if (dgv.CurrentCell.GetType().Name == "DataGridViewComboBoxCell" && dgv.CurrentCell.RowIndex != -1)
                {
                    //给这个DataGridViewComboBoxCell加上下拉事件
                    (e.Control as ComboBox).SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
       
    }
}
