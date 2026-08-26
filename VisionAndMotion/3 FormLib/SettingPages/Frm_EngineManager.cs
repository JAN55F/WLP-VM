using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    public partial class Frm_EngineManager : Form 
    {
        public Frm_EngineManager()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_EngineManager _instance;
        public static Frm_EngineManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_EngineManager();
                return _instance;
            }
        }
        /// <summary>
        /// 是否忽略执行
        /// </summary>
        internal static  bool ignore = false;


        private void Frm_EngineManager_Load(object sender, EventArgs e)
        {
            try
            {
                cbx_engineList.Clear();
                for (int i = 0; i < Project.Instance.L_engineList.Count; i++)
                {
                    cbx_engineList.Add(Project.Instance.L_engineList[i].schemeName);
                }
                cbx_engineList.TextStr = Project.Instance.curEngine.schemeName;
                Frm_EngineManager.Instance.TopMost = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void cbx_engineList_SelectedIndexChanged(object sender, EventArgs e)
        {
         
        }
        private void btn_upMove_Click(object sender, EventArgs e)
        {
           
        }
        private void btn_downMove_Click(object sender, EventArgs e)
        {
           
        }
        private void btn_createEngine_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.CreateScheme();
        }
        private void btn_cloneEngine_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.CloneScheme();
        }
        private void btn_inportEngine_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.InportScheme();
        }
        private void btn_exportEngine_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.ExportScheme();
        }
        private void btn_deleteEngine_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.DeleteScheme();
        }
        private void btn_save_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.CreateScheme();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.CloneScheme();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.InportScheme();
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.ExportScheme();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.DeleteScheme();
        }
        private void MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonDown;
            Application.DoEvents();
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
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

        private void button7_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.ExportScheme();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                ignore = true;
                Project.Instance.curEngine.schemeName = tbx_engineName.Text.Trim();
                if (cbx_engineList.TextStr != tbx_engineName.Text.Trim())
                {
                    cbx_engineList.Add(tbx_engineName.Text.Trim());
                    //////cbx_engineList.Remove(cbx_engineList.Text);
                    cbx_engineList.TextStr = tbx_engineName.Text.Trim();
                    //////Frm_Main.Instance.lbl_title.Text = string.Format("VM Pro - {0}    [ 当前方案：{1} ]", Project.Instance.configuration.ProgramTitle, tbx_engineName.Text.Trim());
                }
                ignore = false;
                this.Close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void cbx_engineList_SelectedIndexChanged()
        {
            try
            {
                if (ignore)
                    return;

                Scheme.SwitchScheme(cbx_engineList.TextStr );
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dgv_engineInfo.SelectedRows[0].Index;
                if (index == 0)
                    return;

                Job temp = Project.Instance.curEngine.L_jobList[index - 1];
                Project.Instance.curEngine.L_jobList[index - 1] = Project.Instance.curEngine.L_jobList[index];
                Project.Instance.curEngine.L_jobList[index] = temp;

                object obj = dgv_engineInfo.Rows[dgv_engineInfo.SelectedRows[0].Index - 1].Cells[1].Value.ToString();
                dgv_engineInfo.Rows[dgv_engineInfo.SelectedRows[0].Index - 1].Cells[1].Value = dgv_engineInfo.SelectedRows[0].Cells[1].Value;
                dgv_engineInfo.SelectedRows[0].Cells[1].Value = obj;

                TabPage tabPage = Frm_Job.Instance.tbc_jobs.TabPages[dgv_engineInfo.SelectedRows[0].Index - 1];
                Frm_Job.Instance.tbc_jobs.TabPages[dgv_engineInfo.SelectedRows[0].Index - 1] = Frm_Job.Instance.tbc_jobs.TabPages[dgv_engineInfo.SelectedRows[0].Index];
                Frm_Job.Instance.tbc_jobs.TabPages[dgv_engineInfo.SelectedRows[0].Index] = tabPage;

                dgv_engineInfo.Rows[index - 1].Selected = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dgv_engineInfo.SelectedRows[0].Index;
                if (index == Project.Instance.curEngine.L_jobList.Count - 1)
                    return;

                Job temp = Project.Instance.curEngine.L_jobList[index + 1];
                Project.Instance.curEngine.L_jobList[index + 1] = Project.Instance.curEngine.L_jobList[index];
                Project.Instance.curEngine.L_jobList[index] = temp;

                string obj = dgv_engineInfo.Rows[dgv_engineInfo.SelectedRows[0].Index + 1].Cells[1].Value.ToString();
                dgv_engineInfo.Rows[dgv_engineInfo.SelectedRows[0].Index + 1].Cells[1].Value = dgv_engineInfo.SelectedRows[0].Cells[1].Value;
                dgv_engineInfo.SelectedRows[0].Cells[1].Value = obj;

                TabPage tabPage = Frm_Job.Instance.tbc_jobs.TabPages[dgv_engineInfo.SelectedRows[0].Index + 1];
                Frm_Job.Instance.tbc_jobs.TabPages[dgv_engineInfo.SelectedRows[0].Index + 1] = Frm_Job.Instance.tbc_jobs.TabPages[dgv_engineInfo.SelectedRows[0].Index];
                Frm_Job.Instance.tbc_jobs.TabPages[dgv_engineInfo.SelectedRows[0].Index] = tabPage;

                dgv_engineInfo.Rows[index + 1].Selected = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.CreateScheme();
        }
    }
}
