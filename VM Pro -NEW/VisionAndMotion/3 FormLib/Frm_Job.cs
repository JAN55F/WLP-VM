using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_Job : DockContent
    {
        internal Frm_Job()
        {
            InitializeComponent();
            Init_Language();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_Job _instance;
        public static Frm_Job Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_Job();
                return _instance;
            }
        }
        /// <summary>
        /// 运行流程线程
        /// </summary>
        internal Thread th_runJob;


        /// <summary>
        /// 初始化语言
        /// </summary>
        private void Init_Language()
        {
            try
            {
                if (Project.Instance.configuration.language == Language.English)
                {
                    this.Text = "Job Editor";
                    btn_runOnce.Text = "Run Once";
                    btn_runLoop.Text = "Run Loop";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 作业实时运行
        /// </summary>
        internal void RealTimeRun(object jobName)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void btn_jobLoopRun_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                if (btn_runLoop.Text == "连续运行")
                    Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).LoopRun(true);
                else
                    Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).LoopRun(false);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_Job_FormClosed(object sender, FormClosedEventArgs e)
        {
            _instance = null;
        }
        private void Frm_Job_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
        private void tbc_jobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                Frm_Monitor.Instance.dgv_monitor.Rows.Clear();
                if (Frm_Job.Instance.tbc_jobs.RowCount > 0)
                {
                    if (Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).isRunLoop)
                        Frm_Job.Instance.Text = "流程编辑器    Runing...";
                    else
                        Frm_Job.Instance.Text = "流程编辑器";

                    if (Machine.machineRunStatu == MachineRunStatu.Running && Job.FindJobByName(tbc_jobs.SelectedTab.Text).jobRunMode == JobRunMode.LoopRunAfterStart)
                    {
                        btn_runLoop.Text = "连续运行";
                        btn_runLoop.Enabled = false;
                        btn_runOnce.Enabled = false;
                        Frm_Main.Instance.toolStripButton11.Enabled = false;
                        Frm_Main.Instance.toolStripButton12.Enabled = false;
                        Frm_Main.Instance.toolStripButton35.Enabled = false;
                        Frm_Main.Instance.toolStripButton16.Enabled = false;
                    }
                    else
                    {
                        btn_runLoop.Enabled = true;
                        btn_runOnce.Enabled = true;
                        Frm_Main.Instance.toolStripButton11.Enabled = true;
                        Frm_Main.Instance.toolStripButton12.Enabled = true;
                        Frm_Main.Instance.toolStripButton35.Enabled = true;
                        Frm_Main.Instance.toolStripButton16.Enabled = true;
                        if (Job.FindJobByName(tbc_jobs.SelectedTab.Text).isRunLoop)
                            btn_runLoop.Text = "停止运行";
                        else
                            btn_runLoop.Text = "连续运行";
                    }
                }
                else
                {
                    btn_runLoop.Text = "连续运行";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tsb_createJob_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Job.CreateJob();
        }
        private void tsb_expandJob_Click(object sender, EventArgs e)
        {
            if (tbc_jobs.TabPages.Count < 1)
                return;
            string jobName = tbc_jobs.SelectedTab.Text;
            Job job = Job.FindJobByName(jobName);
            Job.GetJobTree(jobName).ExpandAll();
            job.DrawLine();

        }
        private void tsb_foldJob_Click(object sender, EventArgs e)
        {
            try
            {
                if (Frm_Job.Instance.tbc_jobs.TabPages.Count < 1)
                    return;
                string jobName = tbc_jobs.SelectedTab.Text;
                Job job = Job.FindJobByName(jobName);
                Job.GetJobTree(jobName).CollapseAll();
                job.DrawLine();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal void tsb_deleteJob_Click(object sender, EventArgs e)
        {
            if (Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).isRunLoop)
            {
                Frm_MessageBox.Instance.MessageBoxShow("当前流程正在运行，请先停止运行", TipType.Error);
                return;
            }
            Job.DeleteJob();
        }
        public void tsb_jobInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                if (Frm_Job.Instance.tbc_jobs.TabPages.Count == 0)
                {
                    Frm_Main.Instance.OutputMsg("当前无可用流程，不可打开流程属性页面", Color.Black);
                    return;
                }
                Frm_JobInfo.Instance.tbx_jobName.TextStr = tbc_jobs.SelectedTab.Text;
                Frm_JobInfo.Instance.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void btn_runOnce_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            if (Frm_Job.Instance.tbc_jobs.TabPages.Count == 0)
            {
                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "No jobs to run" : "没有可运行的流程", Color.Green);
                return;
            }

            Job.RunAsync(Frm_Job.Instance.tbc_jobs.SelectedTab.Text, true);
        }

        private void btn_runOnce_MouseDown(object sender, MouseEventArgs e)
        {
            btn_runOnce.BackgroundImage = Resources.ButtonDown;
            Application.DoEvents();
        }

        private void btn_runOnce_MouseUp(object sender, MouseEventArgs e)
        {
            btn_runOnce.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }

        private void newButton2_Load(object sender, EventArgs e)
        {

        }

        private void newButton2_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Job.RunCurJob();

            if (Project.Instance.configuration.displayLine && Frm_Job.Instance.tbc_jobs.TabPages.Count > 0)
            {
                Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).DrawLine();
            }
        }

        //private void btn_runLoop_MouseDown(object sender, MouseEventArgs e)
        //{
        //    btn_runLoop.BackgroundImage = Resources.ButtonDown;
        //    Application.DoEvents();
        //}

        //private void btn_runLoop_MouseUp(object sender, MouseEventArgs e)
        //{
        //    btn_runLoop.BackgroundImage = Resources.ButtonUp;
        //    Application.DoEvents();
        //}
        private void btn_runLoop_MouseDown(object sender, MouseEventArgs e)
        {
            btn_runLoop.BackgroundImage = Resources.按钮__1_;
            Application.DoEvents();
        }

        private void btn_runLoop_MouseUp(object sender, MouseEventArgs e)
        {
            btn_runLoop.BackgroundImage = Resources.MouseEnter;
            Application.DoEvents();
        }

        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.按钮__2_;
            Application.DoEvents();
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }

    }
}
