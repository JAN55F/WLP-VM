using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Tool;

namespace VMPro
{
    internal partial class Frm_UserManager : Form
    {
        internal Frm_UserManager()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        private Ini ini = new Ini(Application.StartupPath + "\\Config.ini");
        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_UserManager _instance;
        public static Frm_UserManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_UserManager();
                return _instance;
            }
        }


        /// <summary>
        /// 延迟界面不卡死函数
        /// </summary>
        /// <param name="pinterval"></param>
        private void Delay(double pinterval)
        {
            try
            {
                DateTime time = DateTime.Now;
                double span = pinterval * 10000;
                while ((DateTime.Now.Ticks - time.Ticks) < span)
                {
                    Application.DoEvents();
                }
            }
            catch { }
        }
        /// <summary>
        /// 输入检查
        /// </summary>
        /// <returns></returns>
        private bool InputCheck()
        {
            try
            {
                if (tbx_newPassword.TextStr  == string.Empty)
                {
                    tbx_newPassword.TextStr = string.Empty;
                    tbx_newPasswordAgain.TextStr = string.Empty;
                    tbx_newPassword.Focus();
                    tbx_newPassword.TextStr = "密码不能为空 ，请重新输入";
                    return false;
                }
                if (tbx_newPassword.TextStr.Length < 1)
                {
                    tbx_newPassword.TextStr = string.Empty;
                    tbx_newPasswordAgain.TextStr = string.Empty;
                    tbx_newPassword.Focus();
                    tbx_newPassword.TextStr = "密码长度不能小于1，请重新输入";
                    return false;
                }
                if (tbx_newPassword.TextStr != tbx_newPasswordAgain.TextStr)
                {
                    //////lal_tip.Text = Project.Instance.configuration.language == Language.English ? "The two password entries do not match, please enter again!" : "两次输入的密码不一致，请重新输入";
                    //////lal_tip.ForeColor = Color.Red;
                    tbx_newPassword.TextStr = string.Empty;
                    tbx_newPasswordAgain.TextStr = string.Empty;
                    tbx_newPassword.Focus();
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void btn_confirm_Click(object sender, EventArgs e)
        {
           
        }
        private void Frm_UserManager_Load(object sender, EventArgs e)
        {
            try
            {
                cbx_level.SelectedIndex = 0;
                tbx_originPassword.Select();

                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = index + 1;
                dataGridView1.Rows[index].Cells[1].Value = "开发人员";
                dataGridView1.Rows[index].Cells[2].Value = "开发人员";

                index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = index + 1;
                dataGridView1.Rows[index].Cells[1].Value = "管理员";
                dataGridView1.Rows[index].Cells[2].Value = "管理员";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }

        private void tbx_newPassword_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string currentMD5 = Method.GetMD5(tbx_originPassword.TextStr.Trim());
                if (cbx_level.SelectedIndex == 0)
                {
                    string localMD5 = Project.Instance.configuration.developerPassword;
                    if (currentMD5 == localMD5)
                    {
                        if (InputCheck())
                        {
                            string passwordMD5 = Method.GetMD5(tbx_newPassword.TextStr);
                            Project.Instance.configuration.developerPassword = passwordMD5;
                            MessageBox.Show(Project.Instance.configuration.language == Language.English ? "Password modification successful!" : "密码修改成功", "Tip:");
                            tbx_newPassword.TextStr = string.Empty;
                            tbx_newPasswordAgain.TextStr = string.Empty;
                            tbx_originPassword.TextStr = string.Empty;
                        }
                    }
                    else
                    {
                        tbx_originPassword.TextStr = string.Empty;
                        tbx_originPassword.Focus();
                        tbx_originPassword.TextStr = "初始密码错误，请重新输入";
                    }
                }
                else if (cbx_level.SelectedIndex == 1)
                {
                    string localMD5 = Project.Instance.configuration.adminPassword;
                    if (currentMD5 == localMD5)
                    {
                        if (InputCheck())
                        {
                            string passwordMD5 = Method.GetMD5(tbx_newPassword.TextStr);
                            Project.Instance.configuration.adminPassword = passwordMD5;
                            MessageBox.Show(Project.Instance.configuration.language == Language.English ? "Password modification successful!" : "密码修改成功", "Tip:");
                            tbx_newPassword.TextStr = string.Empty;
                            tbx_newPasswordAgain.TextStr = string.Empty;
                            tbx_originPassword.TextStr = string.Empty;
                        }
                    }
                    else
                    {
                        tbx_originPassword.TextStr = string.Empty;
                        tbx_originPassword.Focus();
                        tbx_originPassword.TextStr = "初始密码错误，请重新输入";
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
