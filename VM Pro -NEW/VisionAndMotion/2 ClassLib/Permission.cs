using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using VMPro.Properties;

namespace VMPro
{
    /// <summary>
    /// 权限类
    /// </summary>
    class Permission
    {

        /// <summary>
        /// 当前权限等级
        /// </summary>
        private static PermissionLevel currentPermission = PermissionLevel.NoPermission;
        internal static PermissionLevel CurrentPermission
        {
            get { return Permission.currentPermission; }
            set
            {
                try
                {
                    Permission.currentPermission = value;
                    string loginInfo = string.Empty;
                    switch (value)
                    {
                        case PermissionLevel.NoPermission:
                            if (Project.Instance.configuration.enablePermissionControl)
                                loginInfo = (Project.Instance.configuration.language == Language.English ? "Not logged in, default to minimum permissions" : "未登录，最低权限");
                            else
                                loginInfo = (Project.Instance.configuration.language == Language.English ? "Not logged in, default to minimum permissions" : "未登录");
                            Frm_Main.Instance.toolStripButton2.Image = Resources.Login1;
                            break;
                        case PermissionLevel.Operator:
                            loginInfo = (Project.Instance.configuration.language == Language.English ? "Operator" : "操作员");
                            Frm_Main.Instance.toolStripButton2.Image = Resources.Login;
                            break;
                        case PermissionLevel.Admin:
                            loginInfo = (Project.Instance.configuration.language == Language.English ? "Admin" : "管理员");
                            Frm_Main.Instance.toolStripButton2.Image = Resources.Login;
                            break;
                        case PermissionLevel.Developer:
                            loginInfo = (Project.Instance.configuration.language == Language.English ? "Developer" : "开发人员");
                            Frm_Main.Instance.toolStripButton2.Image = Resources.Login;
                            break;
                    }

                    Frm_Main.Instance.tss_permissionInfo.Text = (Project.Instance.configuration.language == Language.English ? "当前用户：" : "当前用户：") + loginInfo;
                }
                catch (Exception ex)
                {
                    Log.SaveError(ex);
                }
            }
        }


        /// <summary>
        /// 检查权限等级
        /// </summary>
        /// <param name="permission">能进行此操作的最低权限等级</param>
        /// <returns></returns>
        internal static bool CheckPermission(PermissionLevel permission)
        {
            if (!Project.Instance.configuration.enablePermissionControl)
                return true;
            if ((int)currentPermission < (int)permission)
            {
                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Insufficient permissions, please login to a higher level of permissions and try again" : "权限不足，请登录更高一级权限后重试", Color.Red);
                return false;
            }
            return true;
        }

    }
    internal enum PermissionLevel
    {
        NoPermission,
        Operator,
        Admin,
        Developer,
    }
}
