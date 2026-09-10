using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using Tool;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using VMPro.Properties;

namespace VMPro
{
    /// <summary>
    /// 配置类
    /// </summary>
    [Serializable]
    public class Configuration
    {

        /// <summary>
        /// Ini读写对象
        /// </summary>
        private static Ini ini = new Ini(Application.StartupPath + "\\Config\\Configuration.ini");
        /// <summary>
        /// 数据输出路径
        /// </summary>
        public string dataPath = "D:\\WLP VM";
        private static bool _speedMode = false;

        public static bool SpeedMode
        {
            get { return Configuration._speedMode; }
            set
            {
                Configuration._speedMode = value;
                if (value)
                    Frm_Main.Instance.toolStripButton27.Image = Resources.急速退款;
                else
                    Frm_Main.Instance.toolStripButton27.Image = Resources.急速退款__1_;
            }
        }
        /// <summary>
        /// 图像窗体名称集合
        /// </summary>
        public List<string> imageWindowName = new List<string>();
        /// <summary>
        /// 程序退出时是否保存数据
        /// </summary>
        public bool saveWhenExit = true;
        /// <summary>
        /// 用户已主动删除默认服务端（服务端1）：为 true 时设备管理器不再自动重建它
        /// </summary>
        public bool defaultTcpServerDeleted = false;
        /// <summary>
        /// 程序开启后隐藏菜单栏
        /// </summary>
        public bool hideMenuAfterStart = false;
        /// <summary>
        /// 当前界面布局文件存储路径
        /// </summary>
        public string layoutFilePath = ("Config\\Resources\\Layout\\" + "经典布局1.config");
        /// <summary>
        /// 是否虚拟当前的运动控制卡，便于在非现场调试时使用
        /// </summary>
        public bool vitualCard = false;
        /// <summary>
        /// 是否锁定窗口布局
        /// </summary>
        public bool lockLayout = true;
        /// <summary>
        /// 是否启用主页面
        /// </summary>
        private bool enableMainForm = true;

        public bool EnableMainForm
        {
            get { return enableMainForm; }
            set
            {
                enableMainForm = value;
                Frm_Main.Instance.toolStripButton9.Visible = value;
            }
        }
        /// <summary>
        /// 是否启用视觉页面
        /// </summary>
        private bool enableVisionForm = true;

        public bool EnableVisionForm
        {
            get { return enableVisionForm; }
            set
            {
                enableVisionForm = value;
                Frm_Main.Instance.toolStripButton5.Visible = value ;
            }
        }
        /// <summary>
        /// 是否启用运动页面
        /// </summary>
        private bool enableMotionForm = true;

        public bool EnableMotionForm
        {
            get { return enableMotionForm; }
            set
            {
                enableMotionForm = value;
                Frm_Main.Instance.toolStripButton1.Visible = value;
            }
        }
        /// <summary>
        /// 默认页面
        /// </summary>
        public FormMode defaultForm = FormMode.VisionForm;
        /// <summary>
        /// 是否启用权限管控
        /// </summary>
        public bool enablePermissionControl = false;
        /// <summary>
        /// 是否允许改变主窗体大小
        /// </summary>
        public bool allowResizeForm = false ;
        /// <summary>
        /// 通讯方式
        /// </summary>
        public CommunicationType communicationType = CommunicationType.None;
        /// <summary>
        /// 主窗体高
        /// </summary>
        public Int32 mainFormHeight = 80;
        /// <summary>
        /// 程序启动后自动运行
        /// </summary>
        public bool autoRunAfterStart = false;

        public bool failStop = false  ;
        public bool endStop = false   ;
        /// <summary>
        /// 主窗体宽
        /// </summary>
        public Int32 mainFormWidth = 100;
        /// <summary>
        /// 显示连线
        /// </summary>
        public bool displayLine = true;
        /// <summary>
        /// 程序启动后是否处于生产界面
        /// </summary>
        public bool showProductionFormAfterStart = false;
        /// <summary>
        /// 程序开启后是否自动最大化
        /// </summary>
        public bool maxSizeAfterStart = false;
        /// <summary>
        /// 是否每次启动程序和关闭程序时自动备份程序
        /// </summary>
        public bool autoBackupProgram = false;
        /// <summary>
        /// 程序启动后自动锁定
        /// </summary>
        public bool autoLockAfterStart = false;
        /// <summary>
        /// 数据存储天数
        /// </summary>
        public int dataSaveDays = 7;
        /// <summary>
        /// 管理员密码
        /// </summary>
        public string adminPassword = "21232f297a57a5a743894a0e4a801fc3";        //默认密码为admin
        /// <summary>
        /// 开发者密码
        /// </summary>
        public string developerPassword = "5e8edd851d2fdfbd7415232c67367cc3";        //密码默认为developer
        /// <summary>
        /// 当前使用的板卡类型
        /// </summary>
        public CardType cardType = CardType.无;
        /// <summary>
        /// 语言
        /// </summary>
        public Language language = Language.Chinese;
        /// <summary>
        /// 程序启动以后自动进行通讯连接
        /// </summary>
        public bool autoConnectAfterStart = false;
        /// <summary>
        /// 充当客户端时对方IP
        /// </summary>
        public string remoteIPAsClient = "192.168.0.1";
        /// <summary>
        /// 充当客户端时对方端口号
        /// </summary>
        public Int32 remotePortAsClient = 10004;
        /// <summary>
        /// 充当服务端时本地IP
        /// </summary>
        public string localIPAsSever = "192.168.0.1";
        /// <summary>
        /// 充当服务端时本地端口号
        /// </summary>
        public Int32 localPortAsSever = 10004;
        /// <summary>
        /// 自动运行速度
        /// </summary>
        public short autoRunVel = 20;
        /// <summary>
        /// 自动运行时速度百分比
        /// </summary>
        public double autoRunVelRoute = 100;
        /// <summary>
        /// 作业自动运行间隔时间
        /// </summary>
        public int timeBetweenJobRun = 200;
        /// <summary>
        /// 开机自动运行状态
        /// </summary>
        public bool switchedToAutoMode = true;
        /// <summary>
        /// 开机后程序自启动
        /// </summary>
        public bool autoStartAfterStartup = false;
        /// <summary>
        /// 通讯配置项
        /// </summary>
        internal List<CommConfigItem> L_communicationItemList = new List<CommConfigItem>();
        /// <summary>
        /// 近期打开过的文件
        /// </summary>
        public List<string> L_recentlyOpendFile = new List<string>();
        /// <summary>
        /// 公司名称
        /// </summary>
        internal const string DefaultCompanyName = "威乐普电子科技有限公司";
        internal const string ProductName = "WLP VM";
        internal const string ProductVersion = "1.5.0";
        internal const string ProductUpdateTime = "2026-09-10";
        internal const string ProductDisplayName = ProductName + " v" + ProductVersion;
        internal const string DefaultProgramTitle = ProductName;
        private string _companyName = DefaultCompanyName;
        public string CompanyName
        {
            get { return Project.Instance.configuration._companyName; }
            set
            {
                // 当前发行版使用固定公司品牌；旧项目或设置页输入不能覆盖。
                Project.Instance.configuration._companyName = DefaultCompanyName;
                Frm_Main.Instance.lbl_title.Text = BuildApplicationTitle(
                    Project.Instance.configuration.ProgramTitle);
            }
        }
        /// <summary>
        /// 项目名称
        /// </summary>
        private string programTitle = DefaultProgramTitle;
        public string ProgramTitle
        {
            get { return NormalizeProgramTitle(Project.Instance.configuration.programTitle); }
            set
            {
                Project.Instance.configuration.programTitle = NormalizeProgramTitle(value);
                Frm_Main.Instance.lbl_title.Text = BuildApplicationTitle(
                    Project.Instance.configuration.ProgramTitle);

            }
        }

        /// <summary>
        /// 旧版曾把产品名或演示产线名存入项目标题。品牌升级后只归一化
        /// 这些已知占位值；用户自行命名的项目仍原样保留。
        /// </summary>
        internal static string NormalizeProgramTitle(string title)
        {
            string normalized = (title ?? string.Empty).Trim();
            if (normalized.Length == 0 ||
                string.Equals(normalized, "未命名", StringComparison.Ordinal) ||
                string.Equals(normalized, "手机组装", StringComparison.Ordinal) ||
                string.Equals(normalized, "通用视觉软件", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "VM Pro", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "VM Pro 通用视觉软件", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, ProductDisplayName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, ProductName, StringComparison.OrdinalIgnoreCase))
            {
                return DefaultProgramTitle;
            }

            return normalized;
        }

        /// <summary>
        /// 主窗口始终保留软件品牌；当用户设置了独立项目名时，将项目名作为
        /// 次级上下文展示，避免加载项目后”WLP VM”从标题中消失。
        /// </summary>
        internal static string BuildApplicationTitle(string projectTitle)
        {
            string normalized = NormalizeProgramTitle(projectTitle);
            string productTitle = string.Format(“{0} - {1} [Debug: {2}]”, DefaultCompanyName, ProductDisplayName, ProductUpdateTime);
            if (string.Equals(normalized, DefaultProgramTitle, StringComparison.OrdinalIgnoreCase))
                return productTitle;

            return string.Format(“{0} · {1}”, productTitle, normalized);
        }


        /// <summary>
        /// 从本地读取配置项
        /// </summary>
        public void Read(bool tip = true)
        {
            try
            {
                if (File.Exists(Application.StartupPath + "\\Config\\Configuration.ini"))
                {
                    autoBackupProgram = Convert.ToBoolean(ini.IniReadConfig("autoBackup"));
                    autoConnectAfterStart = Convert.ToBoolean(ini.IniReadConfig("AutoConnectAfterStart"));
                    autoRunVel = Convert.ToInt16(ini.IniReadConfig("AutoRunVel"));
                    programTitle = NormalizeProgramTitle(ini.IniReadConfig("ProgramTitle"));
                    timeBetweenJobRun = Convert.ToInt16(ini.IniReadConfig("TimeBetweenJobRun"));
                    switchedToAutoMode = Convert.ToBoolean(ini.IniReadConfig("SwitchedToAuto"));
                    language = (Language)Enum.Parse(typeof(Language), ini.IniReadConfig("Language"));
                    cardType = (CardType)Enum.Parse(typeof(CardType), ini.IniReadConfig("CardType"));
                    autoStartAfterStartup = Convert.ToBoolean(ini.IniReadConfig("AutoStartAfterStartup"));
                    // 配置会在壳层构造前读取；这里只更新模型，不能经属性 setter
                    // 隐式创建或直接触碰主窗体。Machine.InitAll 随后统一刷新标题。
                    _companyName = DefaultCompanyName;
                    localIPAsSever = ini.IniReadConfig("LocalIPAsSever");
                    displayLine = Convert.ToBoolean(ini.IniReadConfig("DisplayLine"));
                    localPortAsSever = Convert.ToInt32(ini.IniReadConfig("LocalPortAsSever"));
                    remoteIPAsClient = ini.IniReadConfig("RemoteIPAsClient");
                    remotePortAsClient = Convert.ToInt32(ini.IniReadConfig("RemotePortAsClient"));
                    showProductionFormAfterStart = Convert.ToBoolean(ini.IniReadConfig("ShowMainForm"));
                    enablePermissionControl = Convert.ToBoolean(ini.IniReadConfig("LoginFree"));
                    communicationType = (CommunicationType)Enum.Parse(typeof(CommunicationType), ini.IniReadConfig("CommuniationMode"));
                    adminPassword = ini.IniReadConfig("AdminPassword");
                    developerPassword = ini.IniReadConfig("DeveloperPassword");
                    autoLockAfterStart = Convert.ToBoolean(ini.IniReadConfig("AutoLock"));
                    mainFormWidth = Convert.ToInt32(ini.IniReadConfig("FormWidth"));
                    mainFormHeight = Convert.ToInt32(ini.IniReadConfig("FormHeight"));
                    maxSizeAfterStart = Convert.ToBoolean(ini.IniReadConfig("MaxSizeAfterStart"));
                    allowResizeForm = Convert.ToBoolean(ini.IniReadConfig("AllowResizeForm"));
                    enablePermissionControl = Convert.ToBoolean(ini.IniReadConfig("EnablePermissionControl"));
                    layoutFilePath = ini.IniReadConfig("CurrentLayout");
                    lockLayout = Convert.ToBoolean(ini.IniReadConfig("LockLayout"));
                    vitualCard = Convert.ToBoolean(ini.IniReadConfig("IsVitualCard"));
                    saveWhenExit = Convert.ToBoolean(ini.IniReadConfig("SaveWhenExit"));
                    hideMenuAfterStart = Convert.ToBoolean(ini.IniReadConfig("HideMenuAfterStart"));
                    autoRunVelRoute = Convert.ToDouble(ini.IniReadConfig("AutoRunVelRoute"));
                    dataSaveDays = Convert.ToInt16(ini.IniReadConfig("DataSaveDays"));

                    enableMainForm = Convert.ToBoolean(ini.IniReadConfig("EnableMainForm"));
                    enableVisionForm = Convert.ToBoolean(ini.IniReadConfig("EnableVisionForm"));
                    enableMotionForm = Convert.ToBoolean(ini.IniReadConfig("EnableMotionForm"));

                    failStop = Convert.ToBoolean(ini.IniReadConfig("FailStop"));
                    endStop = Convert.ToBoolean(ini .IniReadConfig ("EndStop"));
                    autoRunAfterStart = Convert.ToBoolean(ini.IniReadConfig("AutoRunAfterStart"));

                    L_recentlyOpendFile.Clear();
                    for (int i = 1; i < 6; i++)
                    {
                        L_recentlyOpendFile.Add(ini.IniReadConfig("RecentlyOpendFile" + i));
                    }

                    string data = ini.IniReadConfig("CommConfigList");
                    L_communicationItemList.Clear();
                    if (data != "")
                    {
                        string[] item = Regex.Split(data, "@");
                        for (int i = 0; i < item.Length; i++)
                        {
                            string[] temp = Regex.Split(item[i], ",");
                            CommConfigItem commConfigItem = new CommConfigItem();
                            commConfigItem.ReceivedCommand = temp[0];
                            commConfigItem.JobName = temp[1];
                            commConfigItem.OutputItem = temp[2];
                            commConfigItem.NGRespond = temp[3];
                            commConfigItem.PrefixStr = temp[4];
                            commConfigItem.suffixStr = temp[5];
                            L_communicationItemList.Add(commConfigItem);
                        }
                    }

                    data = ini.IniReadConfig("ImageWindowName");
                    if (data != string.Empty)
                    {
                        string[] strs = Regex.Split(data, ";");
                        imageWindowName.AddRange(strs);
                    }
                }
                else
                {
                    if (tip)
                    {
                        //Frm_MessageBox.Instance.MessageBoxShow("\r\n配置文件不存在，将以初始默认参数启动");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 保存所有配置项
        /// </summary>
        internal void Save()
        {
            try
            {
                ini.IniWriteConfig("autoBackup", autoBackupProgram.ToString());
                ini.IniWriteConfig("AutoConnectAfterStart", autoConnectAfterStart.ToString());
                ini.IniWriteConfig("AutoRunVel", autoRunVel.ToString());
                ini.IniWriteConfig("ProgramTitle", NormalizeProgramTitle(programTitle));
                ini.IniWriteConfig("TimeBetweenJobRun", timeBetweenJobRun.ToString());
                ini.IniWriteConfig("SwitchedToAuto", switchedToAutoMode.ToString());
                ini.IniWriteConfig("Language", language.ToString());
                ini.IniWriteConfig("CardType", cardType.ToString());
                ini.IniWriteConfig("AutoStartAfterStartup", autoStartAfterStartup.ToString());
                ini.IniWriteConfig("CompanyName", CompanyName);
                ini.IniWriteConfig("DisplayLine", displayLine.ToString());
                ini.IniWriteConfig("LocalIPAsSever", localIPAsSever);
                ini.IniWriteConfig("LocalPortAsSever", localPortAsSever.ToString());
                ini.IniWriteConfig("RemoteIPAsClient", remoteIPAsClient);
                ini.IniWriteConfig("RemotePortAsClient", remotePortAsClient.ToString());
                ini.IniWriteConfig("ShowMainForm", showProductionFormAfterStart.ToString());
                ini.IniWriteConfig("LoginFree", enablePermissionControl.ToString());
                ini.IniWriteConfig("CommuniationMode", communicationType.ToString());
                ini.IniWriteConfig("AdminPassword", adminPassword);
                ini.IniWriteConfig("DeveloperPassword", developerPassword);
                ini.IniWriteConfig("AutoLock", autoLockAfterStart.ToString());
                ini.IniWriteConfig("FormWidth", mainFormWidth.ToString());
                ini.IniWriteConfig("FormHeight", mainFormHeight.ToString());
                ini.IniWriteConfig("MaxSizeAfterStart", maxSizeAfterStart.ToString());
                ini.IniWriteConfig("AllowResizeForm", allowResizeForm.ToString());
                ini.IniWriteConfig("CurrentLayout", layoutFilePath);
                ini.IniWriteConfig("LockLayout", lockLayout.ToString());
                ini.IniWriteConfig("IsVitualCard", vitualCard.ToString());
                ini.IniWriteConfig("EnablePermissionControl", enablePermissionControl.ToString());
                ini.IniWriteConfig("SaveWhenExit", saveWhenExit.ToString());
                ini.IniWriteConfig("HideMenuAfterStart", hideMenuAfterStart.ToString());
                ini.IniWriteConfig("AutoRunVelRoute", autoRunVelRoute.ToString());
                ini.IniWriteConfig("DataSaveDays", dataSaveDays.ToString());

                ini.IniWriteConfig("EnableMainForm", enableMainForm.ToString());
                ini.IniWriteConfig("EnableVisionForm", enableVisionForm.ToString());
                ini.IniWriteConfig("EnableMotionForm", enableMotionForm.ToString());

                ini.IniWriteConfig("FailStop", failStop.ToString());
                ini.IniWriteConfig("EndStop",endStop .ToString ());
                ini.IniWriteConfig("AutoRunAfterStart", autoRunAfterStart.ToString ());

                for (int i = 0; i < L_recentlyOpendFile.Count; i++)
                {
                    ini.IniWriteConfig("RecentlyOpendFile" + (i + 1), L_recentlyOpendFile[i]);
                }

                string data = string.Empty;
                for (int i = 0; i < imageWindowName.Count; i++)
                {
                    data += imageWindowName[i];
                    if (i != imageWindowName.Count - 1)
                        data += ";";
                }
                ini.IniWriteConfig("ImageWindowName", data);

                //保存通讯配置项
                string data1 = string.Empty;
                for (int i = 0; i < L_communicationItemList.Count; i++)
                {
                    data1 += L_communicationItemList[i].ReceivedCommand + "," + L_communicationItemList[i].JobName + "," + L_communicationItemList[i].OutputItem + "," + L_communicationItemList[i].NGRespond + "," + L_communicationItemList[i].PrefixStr + "," + L_communicationItemList[i].suffixStr;
                    if (i != L_communicationItemList.Count - 1)
                        data1 += "@";
                }
                ini.IniWriteConfig("CommConfigList", data1);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
