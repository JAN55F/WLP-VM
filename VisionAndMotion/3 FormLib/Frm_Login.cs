using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using Tool;

namespace VMPro
{
    internal partial class Frm_Login : Frm_FormBase
    {
        internal Frm_Login()
        {
            InitializeComponent();
            Init_Language();
        }

        #region 窗体拖动
        private static bool IsDrag = false;
        private int enterX;
        private int enterY;
        private void setForm_MouseDown(object sender, MouseEventArgs e)
        {
            IsDrag = true;
            enterX = e.Location.X;
            enterY = e.Location.Y;
        }
        private void setForm_MouseUp(object sender, MouseEventArgs e)
        {
            IsDrag = false;
            enterX = 0;
            enterY = 0;
        }
        private void setForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrag)
            {
                Left += e.Location.X - enterX;
                Top += e.Location.Y - enterY;
            }
        }
        #endregion
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_Login _instance;
        public static Frm_Login Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_Login();
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
                    this.Text = "Login";
                    cbx_user.Clear();
                    //cbx_user.AddRange(new string[] { "Developer", "Admin", "Operator", "Password-free Login" });
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 延迟界面不卡死函数，慎用，会大量消耗CPU
        /// </summary>
        /// <param name="pinterval">时长</param>
        private void Delay(double interval)
        {
            try
            {
                DateTime time = DateTime.Now;
                double span = interval * 10000;
                while ((DateTime.Now.Ticks - time.Ticks) < span)
                {
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void btn_login_Click(object sender, EventArgs e)
        {
            try
            {
                //去注释后可以免登录
                //////this.Hide();
                //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                //////Permission.CurrentPermission = PermissionLevel.Developer;
                //////Machine.SwitchToDebugForm();

                if (cbx_user.TextStr == (Project.Instance.configuration.language == Language.English ? "Developer" : "开发人员"))
                {
                    string temp = Method.GetMD5(tbx_password.TextStr.Trim());
                    if (Method.GetMD5(tbx_password.TextStr.Trim()) == Project.Instance.configuration.developerPassword)
                    {
                        this.Hide();
                        Permission.CurrentPermission = PermissionLevel.Developer;
                        Log.SaveLog(LogType.Operate, "用户登录成功，当前用户：Developer");
                        Machine.SwitchToDebugForm();
                    }
                    else
                    {
                        tbx_password.TextStr = string.Empty;
                        tbx_password.Focus();
                        tbx_password.TextStr = "密码错误，请重新输入";
                    }
                }
                else if (cbx_user.TextStr == (Project.Instance.configuration.language == Language.English ? "Admin" : "管理员"))
                {
                    string currentMD5 = Tool.Method.GetMD5(tbx_password.TextStr.Trim());
                    if (currentMD5 == Project.Instance.configuration.adminPassword)
                    {
                        this.Hide();
                        //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                        Log.SaveLog(LogType.Operate, "用户登录成功，当前用户：Admin");
                        Permission.CurrentPermission = PermissionLevel.Admin;
                        Machine.SwitchToDebugForm();
                    }
                    else
                    {
                        tbx_password.TextStr = string.Empty;
                        tbx_password.Focus();
                        tbx_password.TextStr = "密码错误，请重新输入";
                    }
                }
                else if (cbx_user.TextStr == (Project.Instance.configuration.language == Language.English ? "Admin" : "操作员"))
                {
                    string currentMD5 = Tool.Method.GetMD5(tbx_password.TextStr.Trim());
                    if (currentMD5 == Project.Instance.configuration.adminPassword)
                    {
                        this.Hide();
                        //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                        Permission.CurrentPermission = PermissionLevel.Operator;
                        Log.SaveLog(LogType.Operate, "用户登录成功，当前用户：Operator");
                        Machine.SwitchToDebugForm();
                    }
                    else
                    {
                        tbx_password.TextStr = string.Empty;
                        tbx_password.Focus();
                        tbx_password.TextStr = "密码错误，请重新输入";
                    }
                }
                else
                {
                    this.Hide();
                    //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                    Permission.CurrentPermission = PermissionLevel.NoPermission;
                    Machine.SwitchToDebugForm();
                }
                tbx_password.TextStr = string.Empty;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void cbx_user_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbx_password.Focus();
        }
        private void Frm_Login_Shown(object sender, EventArgs e)
        {
            label1.Text = Project.Instance.configuration.CompanyName;
            tbx_password.DefaultText = "请输入密码";
            tbx_password.TextStr = string.Empty;
            tbx_password.Focus();

            cbx_user.SelectedIndex = 0;
            tbx_password.Select();
        }
        private void btn_logout_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                Permission.CurrentPermission = PermissionLevel.NoPermission;
                Machine.SwitchToDebugForm();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                Permission.CurrentPermission = PermissionLevel.NoPermission;
                Machine.SwitchToDebugForm();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //去注释后可以免登录
                //////this.Hide();
                //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                //////Permission.CurrentPermission = PermissionLevel.Developer;
                //////Machine.SwitchToDebugForm();

                if (cbx_user.TextStr == (Project.Instance.configuration.language == Language.English ? "Developer" : "开发人员"))
                {
                    string temp = Method.GetMD5(tbx_password.TextStr.Trim());
                    if (Method.GetMD5(tbx_password.TextStr.Trim()) == Project.Instance.configuration.developerPassword)
                    {
                        this.Hide();
                        Permission.CurrentPermission = PermissionLevel.Developer;
                        Log.SaveLog(LogType.Operate, "用户登录成功，当前用户：Developer");
                        Machine.SwitchToDebugForm();
                    }
                    else
                    {
                        tbx_password.DefaultText = "密码错误，请重新输入";
                        tbx_password.TextStr = string.Empty;
                        //tbx_password.Focus();
                    }
                }
                else if (cbx_user.TextStr == (Project.Instance.configuration.language == Language.English ? "Admin" : "管理员"))
                {
                    string currentMD5 = Tool.Method.GetMD5(tbx_password.TextStr.Trim());
                    if (currentMD5 == Project.Instance.configuration.adminPassword)
                    {
                        this.Hide();
                        //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                        Log.SaveLog(LogType.Operate, "用户登录成功，当前用户：Admin");
                        Permission.CurrentPermission = PermissionLevel.Admin;
                        Machine.SwitchToDebugForm();
                    }
                    else
                    {
                        tbx_password.TextStr = "密码错误，请重新输入";
                        tbx_password.TextStr = string.Empty;
                        //////tbx_password.Focus();
                     
                    }
                }
                else if (cbx_user.TextStr == (Project.Instance.configuration.language == Language.English ? "Admin" : "操作员"))
                {
                    string currentMD5 = Tool.Method.GetMD5(tbx_password.TextStr.Trim());
                    if (currentMD5 == Project.Instance.configuration.adminPassword)
                    {
                        this.Hide();
                        //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                        Permission.CurrentPermission = PermissionLevel.Operator;
                        Log.SaveLog(LogType.Operate, "用户登录成功，当前用户：Operator");
                        Machine.SwitchToDebugForm();
                    }
                    else
                    {
                        tbx_password.TextStr = "密码错误，请重新输入";
                        tbx_password.TextStr = string.Empty;
                        //////tbx_password.Focus();
                      
                    }
                }
                else
                {
                    this.Hide();
                    //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                    Permission.CurrentPermission = PermissionLevel.NoPermission;
                    Machine.SwitchToDebugForm();
                }
                tbx_password.TextStr = string.Empty;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void Frm_Login_Load(object sender, EventArgs e)
        {
            label1.Text = Project.Instance.configuration.CompanyName;
            tbx_password.DefaultText = "请输入密码";
            tbx_password.TextStr = string.Empty;
            tbx_password.Focus();
     
            //tbx_password.Select();
           


            //tbx_password.TextStr = string.Empty;
            //tbx_password.Focus();
            //tbx_password.TextStr = "密码错误，请重新输入";
           
        }

    }
}
