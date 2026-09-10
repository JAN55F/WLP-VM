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

namespace VMPro
{
    public partial class Frm_JobInfo : Frm_FormBase
    {
        public Frm_JobInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_JobInfo _instance;
        public static Frm_JobInfo Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_JobInfo();
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
                if (Project.Instance.configuration.language == Language.English)
                {
                    this.Text = "Job Info";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }



        private void btn_save_Click(object sender, EventArgs e)
        {

        }

        private void Frm_JobInfo_Load(object sender, EventArgs e)
        {
            try
            {

                //获取编辑界面所有的窗体控件
                cbx_imageWindowList.Clear();
                if (!cbx_imageWindowList.Items.Contains("不绑定"))
                    cbx_imageWindowList.Add("不绑定");
                if (!comboBox1.Items.Contains("不绑定"))
                    comboBox1.Add("不绑定");

                if (cbx_imageWindowList.Items.Length > 0)
                    cbx_imageWindowList.SelectedIndex = 0;

                foreach (Control item in Frm_UserForm.Instance.Controls)
                {
                    if (item.GetType().ToString() == "System.Windows.Forms.PictureBox")
                    {
                        cbx_imageWindowList.Add(item.Name.ToString());
                    }
                }
                cbx_imageWindowList.TextStr = Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).imageWindowName;
                if (cbx_imageWindowList.TextStr == "")
                {
                    cbx_imageWindowList.SelectedIndex = 0;
                }
                comboBox1.TextStr = Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).debugImageWindow;
                cComboBox1.SelectedIndex = (int)Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).jobRunMode;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void cButton1_Clicked()
        {

        }

        private void cComboBox1_SelectedIndexChanged()
        {
            Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).jobRunMode = (JobRunMode)cComboBox1.SelectedIndex;
        }

        private void btn_saveAndExit_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //修改流程名
                if (tbx_jobName.TextStr.Trim() != Frm_Job.Instance.tbc_jobs.SelectedTab.Text)
                {
                    //修改所有工具里面的流程名
                    for (int i = 0; i < Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).L_toolList.Count; i++)
                    {
                        Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).L_toolList[i].tool.jobName = tbx_jobName.TextStr.Trim();
                    }

                    Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).jobName = tbx_jobName.TextStr.Trim();
                    Frm_Job.Instance.tbc_jobs.SelectedTab.Text = tbx_jobName.TextStr.Trim();
                }

                Job.FindJobByName(tbx_jobName.TextStr.Trim()).imageWindowName = cbx_imageWindowList.TextStr;
                Job.FindJobByName(tbx_jobName.TextStr.Trim()).debugImageWindow = comboBox1.TextStr;
                Project.SaveProject();
                this.Close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }



    }
}
