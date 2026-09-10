using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using VMPro.Properties;
using System.IO;
using HalconDotNet;
using System.Xml;
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Net.Sockets;

namespace VMPro
{
    internal class Machine
    {

        /// <summary>
        /// 表示程序启动时是否初始化成功
        /// </summary>
        internal static bool initSucceed = true;
        /// <summary>
        /// 是否处于生产模式，分前生产模式和调试模式
        /// </summary>
        internal static bool productionMode = true;
        /// <summary>
        /// 记录旧的时间
        /// </summary>
        internal static DateTime lastTime = DateTime.Now;
        /// <summary>
        /// 当前窗体模式
        /// </summary>
        internal static FormMode curFormMode = FormMode.MainForm;
        /// <summary>
        /// 总生产时间
        /// </summary>
        internal static TimeSpan runTime;
        /// <summary>
        /// 总待机时间
        /// </summary>
        internal static TimeSpan waitTime;
        /// <summary>
        /// 总报警时间
        /// </summary>
        internal static TimeSpan alarmTime;
        /// <summary>
        /// 资源锁
        /// </summary>
        internal static object lock_resources = new object();
        /// <summary>
        /// 是否正在启动
        /// </summary>
        internal static volatile bool loading = true;
        /// <summary>
        /// 资源锁
        /// </summary>
        private static object obj = new object();
        /// <summary>
        /// 程序即将退出
        /// </summary>
        internal static bool willExit = false;
        /// <summary>
        /// 设备运行状态
        /// </summary>
        internal static MachineRunStatu machineRunStatu = MachineRunStatu.WaitReset;

        [DllImport("Kernel32.DLL ", SetLastError = true)]
        public static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        /// <summary>
        /// 在主界面线程同步执行启动阶段的控件操作。
        /// VM.Init 会在启动工作线程之前创建主窗体及其句柄，因此这里不会
        /// 在后台线程意外创建 WinForms 句柄。
        /// </summary>
        private static void RunOnMainUiThread(Action action)
        {
            if (action == null)
                return;

            Frm_Main mainForm = Frm_Main.Instance;
            if (mainForm.IsDisposed || mainForm.Disposing)
                return;
            if (!mainForm.IsHandleCreated)
                throw new InvalidOperationException("主窗体句柄尚未创建，不能从启动线程更新界面。");

            if (mainForm.InvokeRequired)
                mainForm.Invoke(action);
            else
                action();
        }

        /// <summary>
        /// 在主 UI 线程显示启动阶段的提示窗体。硬件初始化仍由调用线程执行，
        /// 这里只切换 WinForms 窗体的创建和显示线程。
        /// </summary>
        internal static void ShowMessageOnMainUiThread(string message, TipType tipType = TipType.Tip)
        {
            RunOnMainUiThread(delegate
            {
                using (Frm_MessageBox messageBox = new Frm_MessageBox())
                    messageBox.MessageBoxShow(message, tipType);
            });
        }

        //更新进度
        internal static void UpdateStep(int percentValue, string stepMsg, bool succeed)
        {
            try
            {
                RunOnMainUiThread(delegate
                {
                    Frm_Welcome welcome = Frm_Welcome.Instance;
                    welcome.bar_step.Value = percentValue;
                    welcome.lbl_step.Text = stepMsg + "......";
                });
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        internal static void InitAll()
        {
            try
            {
                UpdateStep(2, Project.Instance.configuration.language == Language.English ? "Init" : "初始化", true);
                // VM.Init 已在创建欢迎页和主窗体前读取配置；这里不再重复读取，
                // 避免集合项重复，并保证壳层首次构造即使用已保存语言。

                RunOnMainUiThread(delegate
                {
                    Frm_Welcome.Instance.lbl_companyName.Text = Project.Instance.configuration.CompanyName;
                    Frm_Main.Instance.lbl_title.Text = Configuration.BuildApplicationTitle(
                        Project.Instance.configuration.ProgramTitle);
                    Frm_Main.Instance.Init_Tool_Tips();
                });

                //设置环境变量，防止因Halcon版本问题弹出关于HalconRoot的报错  
                //SetEnvironmentVariable("HALCONROOT", "TEST");

                // Install the bundled font silently when it is available. Starting the font file
                // through the shell opens a Windows window while the application is initializing.
                string fontPath = Application.StartupPath + "\\STXINWEI.TTF";
                if (!File.Exists(@"C:\Windows\Fonts\STXINWEI.TTF") && File.Exists(fontPath))
                {
                    try
                    {
                        File.Copy(fontPath, @"C:\Windows\Fonts\STXINWEI.TTF", false);
                    }
                    catch
                    {
                        // Font installation requires system permissions; application startup continues with fallback fonts.
                    }
                }


                //初始化配置文件
                UpdateStep(8, Project.Instance.configuration.language == Language.English ? "Initialization profile" : "初始化配置文件", true);
                InitConfigDirctory();

                //反序列化轴配置对象类
                //////if (File.Exists(Application.StartupPath + "\\Config\\Project\\Motion\\AxisPar.cfg"))
                //////{
                //////    IFormatter formatter = new BinaryFormatter();
                //////    Stream stream = new FileStream(Application.StartupPath + "\\Config\\Project\\Motion\\AxisPar.cfg", FileMode.Open, FileAccess.Read, FileShare.None);
                //////    Axis_Config.Instance = (Axis_Config)formatter.Deserialize(stream);
                //////    stream.Close();
                //////}



                //加载点表信息
                UpdateStep(12, Project.Instance.configuration.language == Language.English ? "Loading point table information" : "加载点表信息", true);
                //if (File.Exists(Application.StartupPath + "\\Config\\Project\\Motion\\Point.xml"))
                //{
                //    XmlDocument xmlDoc = new XmlDocument();
                //    xmlDoc.Load(Application.StartupPath + "\\Config\\Project\\Motion\\Point.xml");
                //    XmlNode rootNode = xmlDoc.SelectSingleNode("PointList");
                //    XmlNodeList Pointnodes = rootNode.ChildNodes;
                //    for (int i = 0; i < Pointnodes.Count; i++)
                //    {
                //        int index = Frm_MotionControl.Instance.dgv_pointList.Rows.Add();
                //        Frm_MotionControl.Instance.dgv_pointList.Rows[index].Cells[0].Value = Frm_MotionControl.Instance.dgv_pointList.Rows.Count - 1;
                //        Frm_MotionControl.Instance.dgv_pointList.Rows[index].Cells[1].Value = Pointnodes[i].Name;
                //        XmlNodeList AxisNodes = Pointnodes[i].ChildNodes;
                //        Application.DoEvents();
                //        for (int j = 0; j < AxisNodes.Count; j++)
                //        {
                //            string axisName = AxisNodes[j].Name;
                //            if (!Frm_MotionControl.Instance.dgv_pointList.Columns.Contains(axisName) && axisName != "速度")
                //                Frm_MotionControl.Instance.dgv_pointList.Columns.Add(axisName, axisName);
                //            string pos = AxisNodes[j].InnerText;
                //            Frm_MotionControl.Instance.dgv_pointList.Rows[index].Cells[j + 2].Value = pos;
                //        }
                //    }
                //}




                ////对RibbonControl进行强签名
                //try
                //{
                //    if (Directory.Exists(@"C:\Windows\assembly"))
                //    {
                //        if (!File.Exists(@"C:\Windows\assembly\DevComponents.DotNetBar2.dll"))
                //            File.Copy(Application.StartupPath + "\\DevComponents.DotNetBar2.dll", @"C:\Windows\assembly\DevComponents.DotNetBar2.dll");
                //    }
                //}
                //catch { }


                //通讯连接
                UpdateStep(16, Project.Instance.configuration.language == Language.English ? "Try Tencent connection" : "尝试通讯连接", true);
                if (Project.Instance.configuration.autoConnectAfterStart)
                {
                    if (Project.Instance.configuration.communicationType == CommunicationType.Internet_Client)
                        ;//////Frm_TCPClient.Instance.Connect();
                    else if (Project.Instance.configuration.communicationType == CommunicationType.Internet_Sever)
                    { }
                    //////Frm_TCPServer.Instance.Listen();
                    //////else if (Project.Instance.configuration.communicationType == CommunicationType.SerialPort)
                    //////    Frm_SerialPort.Instance.btn_openPort_Click(null, null);
                }

                //枚举网络中的相机 
                UpdateStep(20, Project.Instance.configuration.language == Language.English ? "Enumerate cameras in the network" : "枚举网络中的相机", true);

                // 启动阶段不枚举相机。部分相机 SDK 在驱动/运行库不匹配时会触发 native 崩溃，
                // 无法被 C# try/catch 捕获；进入界面后由用户手动刷新设备列表。
                //////SDK_Halcon.EnumCamera();       //枚举网络中的所有的相机
                try
                {
                    //////Frm_SDKInfo.LoadState[0] = SDK_Basler.EnumCamrea();
                }
                catch { }
                try
                {
                    //////SDK_HIKVision instance = new SDK_HIKVision(string.Empty);
                    //////Frm_SDKInfo.LoadState[1] = instance.EnumCamrea();
                }
                catch { }
                try
                {
                    //////Frm_SDKInfo.LoadState[2] = SDK_MindVision.EnumCamera();
                }
                catch { }
                try
                {
                    //////Frm_SDKInfo.LoadState[5] = SDK_PointGrey.EnumCamera();
                }
                catch { }

                //加载标准图像
                UpdateStep(70, Project.Instance.configuration.language == Language.English ? "Load standard image" : "加载标准图像", true);


                //初始化板卡
                UpdateStep(75, Project.Instance.configuration.language == Language.English ? "Initialize board" : "初始化板卡", true);
                switch (Project.Instance.configuration.cardType)
                {
                    case CardType.固高_GTS:
                        Card_Googol.Init();
                        break;
                    case CardType.雷赛_IOC0640:
                        Card_IOC0640.Init();
                        break;
                    case CardType.雷塞_DMC2210:
                        Card_LeadShineDMC2210.Init();
                        break;
                    case CardType.雷塞_DMC2410:
                        Card_LeadShine_DMC2410.Init();
                        break;
                    case CardType.联赢_WMX:
                        //Card_WMX.Init();
                        break;
                }



                //////try
                //////{
                //////    HOperatorSet.SetDraw(Frm_ImageWindow.Instance.WindowHandle, new HTuple("margin"));
                //////}
                //////catch
                //////{
                //////    Frm_MessageBox.Instance.MessageBoxShow("启动异常，启动后程序将不能正常运行(错误代码：002)\r\n可能原因：\r\n1、本机未安装Halcon\r\n2、所安装的Halcon不是17.12版");
                //////}

                //初始化并运行自动流程
                //////Task_SmartLineB.Init();
                //////Task_SmartLineA.AutoRun();

                //显示生产界面
                UpdateStep(78, Project.Instance.configuration.language == Language.English ? "Initialize form" : "初始化窗体", true);
                if (Project.Instance.configuration.showProductionFormAfterStart)
                    RunOnMainUiThread(Machine.SwitchToProductForm);

                UpdateStep(80, Project.Instance.configuration.language == Language.English ? "Check registration status" : "检查注册状态", true);
                string registrationCode = Regiest.Get_RNum(Regiest.Get_MNum());
                RunOnMainUiThread(delegate
                {
                    Frm_Main.Instance.regiestCode = registrationCode;
                    if (Project.Instance.configuration.maxSizeAfterStart)
                        Frm_Main.Instance.WindowState = FormWindowState.Maximized;
                });

                ////// //获取开机后运行模式
                ////// if (Project .Instance .configuration .SwitchedToAuto)
                //////Machine.runStatu = MachineRunStatu.Running;
                ////// else
                //////     Machine.runStatu = MachineRunStatu.Stop ;

                //////if (Project .Instance .configuration .hideFuncPart)
                //////{
                //////    foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_Main.Instance.D_imageWindow)
                //////    {
                //////        item.Value.cbx_toolRunResultImageList.Dock = DockStyle.None;
                //////        item.Value.cbx_toolRunResultImageList.Size = new Size(0, 0);
                //////    }
                //////}

                //初始化各设备
                for (int i = 0; i < Project.Instance.L_lightController.Count; i++)
                {
                    Project.Instance.L_lightController[i].OpenController();
                }


                //加载流程

                RunOnMainUiThread(Job.InitImageList);
                UpdateStep(90, Project.Instance.configuration.language == Language.English ? "Load process file" : "加载流程文件", true);
                // 优先加载上次成功保存/打开的项目；没有记录时再加载最近修改的 .pjt。
                // LoadProject 同时重建流程页、树和菜单；在不能拆分模型/UI 阶段前，
                // 整体回到主线程执行，避免反序列化后的控件更新跨线程。
                RunOnMainUiThread(delegate { Project.LoadStartupProject(); });

                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    //////Project.Instance.curEngine.L_jobList[i].Run();
                }

                UpdateStep(100, Project.Instance.configuration.language == Language.English ? "Load process file" : "启动成功", true);

                //加载设备

                UpdateStep(90, Project.Instance.configuration.language == Language.English ? "Check registration status" : "正在尝试通讯连接", true);

                for (int i = 0; i < Project.Instance.L_TCPClient.Count; i++)
                {
                    TCPClient tcpClient = Project.Instance.L_TCPClient[i];
                    if (tcpClient.AutoConnectAfterStart)
                        tcpClient.Connect();

                    // 网络连接保留在后台；仅 ToolStripItem 的创建和挂接回到 UI 线程。
                    bool connected = tcpClient.FindSocketByName().Connected;
                    string toolTipText = string.Format("名称：{0}\r\n状态：{1}\r\nIP    : {2}\r\nPort : {3}", tcpClient.Name, connected ? "已连接" : "未连接", tcpClient.severIP, tcpClient.severPort);
                    RunOnMainUiThread(delegate
                    {
                        ToolStripItem tsb = new ToolStripStatusLabel("", Resources.客户端1);
                        tsb.AutoSize = false;
                        tsb.Width = 20;
                        tsb.Name = tcpClient.Name;
                        //tsb.BorderSides = ToolStripStatusLabelBorderSides.Right;
                        tsb.ToolTipText = toolTipText;
                        tsb.Tag = tcpClient;
                        tsb.MouseEnter += tsb_MouseEnter;
                        Frm_Main.Instance.statusStrip1.Items.Insert(1, tsb);
                        Frm_Main.Instance.tss_curTime.BorderSides = ToolStripStatusLabelBorderSides.Left;
                    });

                }
                for (int i = 0; i < Project.Instance.L_TCPSever.Count; i++)
                {
                    if (Project.Instance.L_TCPSever[i].AutoListenAfterStart)
                        Project.Instance.L_TCPSever[i].Listen(false);

                    ////////状态显示图标
                    //////ToolStripItem tsb = new ToolStripStatusLabel("", Resources.Client);
                    //////tsb.AutoSize = false;
                    //////tsb.Width = 20;
                    //////tsb.Name = Project.Instance.L_TCPClient[i].ClientName;
                    ////////tsb.BorderSides = ToolStripStatusLabelBorderSides.Right;
                    //////tsb.ToolTipText = string.Format("名称：{0}\r\n状态：{1}\r\nIP    : {2}\r\nPort : {3}", Project.Instance.L_TCPClient[i].ClientName, Project.Instance.L_TCPClient[i].FindSocketByName().Connected ? "已连接" : "未连接", Project.Instance.L_TCPClient[i].severIP, Project.Instance.L_TCPClient[i].severPort);
                    //////Frm_Main.Instance.statusStrip1.Items.Insert(1, tsb);
                    //////Frm_Main.Instance.tss_curTime.BorderSides = ToolStripStatusLabelBorderSides.Left;

                }

                for (int i = 0; i < Project.Instance.L_lightController.Count; i++)
                {
                    if (Project.Instance.L_lightController[i].OpenAllChAfterStart)
                        Project.Instance.L_lightController[i].OpenAllChannel();
                }

                for (int i = 0; i < Project.Instance.L_Scaner.Count; i++)
                {
                    Project.Instance.L_Scaner[i].Init();
                }

                for (int i = 0; i < Project.Instance.L_Serial.Count; i++)
                {
                    Project.Instance.L_Serial[i].Init();
                }

                for (int i = 0; i < Project.Instance.L_PLCDevice.Count; i++)
                {
                    if (Project.Instance.L_PLCDevice[i].AutoConnectAfterStart)
                    {
                        string errMsg;
                        Project.Instance.L_PLCDevice[i].Connect(out errMsg);
                    }
                }
                //添加最近打开过的文件列表
                int count = Project.Instance.configuration.L_recentlyOpendFile.Count;
                for (int i = 0; i < 5 - count; i++)
                {
                    Project.Instance.configuration.L_recentlyOpendFile.Add(string.Empty);
                }
                RunOnMainUiThread(delegate
                {
                    for (int i = 0; i < Project.Instance.configuration.L_recentlyOpendFile.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (Project.Instance.configuration.L_recentlyOpendFile[0] != string.Empty)
                                {
                                    string recentPath = Project.Instance.configuration.L_recentlyOpendFile[i];
                                    string recentFileName = Path.GetFileName(recentPath);
                                    // 历史演示工程保留原文件路径以确保仍可加载，但菜单不再把
                                    // 具体产线名误当成当前产品名称展示。
                                    string recentTitle = Path.GetFileNameWithoutExtension(recentFileName);
                                    if (string.Equals(recentTitle, "手机组装", StringComparison.Ordinal) ||
                                        string.Equals(recentTitle, "通用视觉软件", StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(recentTitle, "VM Pro", StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(recentTitle, "VM Pro 通用视觉软件", StringComparison.OrdinalIgnoreCase))
                                        recentFileName = Configuration.DefaultProgramTitle + Path.GetExtension(recentFileName);
                                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem((i + 1) + ". " + recentFileName);
                                    toolStripMenuItem.Tag = Project.Instance.configuration.L_recentlyOpendFile[i];
                                    toolStripMenuItem.Click += toolStripMenuItem_Click;
                                    toolStripMenuItem.BackColor = Color.White;
                                    Frm_Main.Instance.最近的项目ToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
                                }
                                break;
                        }
                    }
                });

                //更新最后一次结果图像列表
                //////Frm_Main.Instance.tbx_percentageOfMovementSpeed.Text = Project.Instance.configuration.autoRunVelRoute.ToString();



                UpdateStep(100, Project.Instance.configuration.language == Language.English ? "Check registration status" : "启动成功", true);
                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Startup successful" : "程序启动");
                RunOnMainUiThread(delegate
                {
                    Frm_Main mainForm = Frm_Main.Instance;
                    Frm_Welcome welcome = Frm_Welcome.Instance;

                    mainForm.tss_permissionInfo.Text = "当前用户：未登录";
                    if (!Project.Instance.configuration.allowResizeForm)
                    {
                        mainForm.MinimumSize = mainForm.Size;
                        mainForm.MaximumSize = mainForm.Size;
                    }

                    if (!Project.Instance.configuration.EnableMainForm)
                        mainForm.toolStripButton9.Visible = false;
                    if (!Project.Instance.configuration.EnableVisionForm)
                        mainForm.toolStripButton5.Visible = false;
                    if (!Project.Instance.configuration.EnableMotionForm)
                        mainForm.toolStripButton1.Visible = false;

                    //////Frm_Main.Instance.ribbonTabItem1.Select();
                    if (Machine.initSucceed)
                    {
                        mainForm.OutputMsg(Project.Instance.configuration.language == Language.English ? "Startup successful" : "启动成功", Color.Black);
                        welcome.lbl_step.Text = Project.Instance.configuration.language == Language.English ? "Startup successful" : "启动成功";
                    }
                    else
                    {
                        mainForm.OutputMsg("启动出错", Color.Red);
                        welcome.lbl_step.Text = "                  启动出错";
                        welcome.lbl_step.ForeColor = Color.Red;
                        welcome.Height = 356;
                    }
                });

                // 启动界面到这里已经完成。不要在启动阶段预跑流程：
                // 流程可能访问相机、模板、Halcon窗口或外设，容易把欢迎页和主窗体切换卡死。
                loading = false;
            }
            catch (Exception ex)
            {
                loading = false;
                initSucceed = false;
                Log.SaveError(ex);
            }
        }

        static void tsb_MouseEnter(object sender, EventArgs e)
        {
            TCPClient tcpClient = (TCPClient)(((ToolStripItem)sender).Tag);

            Frm_Main.Instance.toolTip1.RemoveAll();
            Frm_Main.Instance.toolTip1.ToolTipTitle = tcpClient.Name;
            string info = string.Format("状态 ：{0}\r\nIP    : {1}\r\nPort : {2}", tcpClient.FindSocketByName().Connected ? "已连接" : "未连接", tcpClient.severIP, tcpClient.severPort);
            Frm_Main.Instance.toolTip1.Show(info, Frm_Main.Instance.statusStrip1);
        }

        static void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = ((ToolStripMenuItem)sender).Tag.ToString();
            string extension = Path.GetExtension(path).ToLowerInvariant();
            if (extension == ".eng")
                Scheme.LoadScheme(path);
            else if (extension == ".job")
                Job.LoadJob(path);
            else
                Project.LoadProject(path);
        }
        internal static void UpdateIO()
        {
            try
            {
                lock (lock_resources)
                {
                    //////while (true)
                    //////{
                    //////if (willExit)
                    //////{
                    //////    break;
                    //////}


                    //实时更新运动控制卡轴与IO状态
                    if (Project.Instance.configuration.cardType == CardType.固高_GTS)
                    {
                        if (Card_Googol.initSucceed || Project.Instance.configuration.vitualCard)
                        {
                            //更新轴位置
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_axisInfo.Rows.Count; i++)
                            {
                                double curPos = Card_Googol.GetCurPosition(Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[1].Value);
                                Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[2].Value = curPos.ToString("0.000");
                            }

                            //更新各IO状态
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_diList.Rows.Count; i++)
                            {
                                string diName = Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[3].Value.ToString();
                                Level level = Card_Googol.GetDiSts(diName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "On")             //此处加一个条件判断，意在防止因重复显示图像导致的闪图问题
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "Off";
                                    }
                                }
                            }
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_doList.Rows.Count; i++)
                            {
                                string doName = Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[4].Value.ToString();
                                Level level = Card_Googol.GetDoSts(doName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "Off";
                                    }
                                }
                            }
                        }
                    }
                    else if (Project.Instance.configuration.cardType == CardType.雷赛_IOC0640)
                    {
                        if (Card_Googol.initSucceed || Project.Instance.configuration.vitualCard)
                        {
                            //雷赛IOC0640为IO卡，不存在轴，故只更新各IO状态
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_diList.Rows.Count; i++)
                            {
                                string diName = Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[3].Value.ToString();
                                Level level = Card_IOC0640.GetDiSts(diName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "Off";
                                    }
                                }
                            }
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_doList.Rows.Count; i++)
                            {
                                string doName = Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[4].Value.ToString();
                                Level level = Card_IOC0640.GetDoSts(doName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "Off";
                                    }
                                }
                            }
                        }
                    }
                    else if (Project.Instance.configuration.cardType == CardType.雷塞_DMC2210)
                    {
                        if (Card_Googol.initSucceed || Project.Instance.configuration.vitualCard)
                        {
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_diList.Rows.Count; i++)
                            {
                                string diName = Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[3].Value.ToString();
                                Level level = Card_LeadShineDMC2210.GetDiSts(diName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "Off";
                                    }
                                }
                            }
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_doList.Rows.Count; i++)
                            {
                                string doName = Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[4].Value.ToString();
                                Level level = Card_LeadShineDMC2210.GetDoSts(doName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "Off";
                                    }
                                }
                            }
                        }
                    }
                    else if (Project.Instance.configuration.cardType == CardType.雷塞_DMC2410)
                    {
                        if (Card_Googol.initSucceed || Project.Instance.configuration.vitualCard)
                        {
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_diList.Rows.Count; i++)
                            {
                                string diName = Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[3].Value.ToString();
                                Level level = Card_LeadShine_DMC2410.GetDiSts(diName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "Off";
                                    }
                                }
                            }
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_doList.Rows.Count; i++)
                            {
                                string doName = Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[4].Value.ToString();
                                Level level = Card_LeadShine_DMC2410.GetDoSts(doName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "Off";
                                    }
                                }
                            }
                        }
                    }
                    else if (Project.Instance.configuration.cardType == CardType.凌华_AMP204C)
                    {
                        if (Card_Googol.initSucceed || Project.Instance.configuration.vitualCard)
                        {
                            //更新轴位置
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_axisInfo.Rows.Count; i++)
                            {
                                double curPos = Card_ADLink.GetCurPosition(Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[1].Value);
                                Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[2].Value = curPos.ToString("0.000");
                                Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[4].Value = Math.Round(curPos * Axis_Config.Instance.MMPixelRoute[Card_ADLink.FindAxisByName(Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[1].Value).actNo], 3);
                            }
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_axisInfo.Rows.Count; i++)
                            {
                                double curPos = Card_ADLink.GetCurEncoder(Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[1].Value);
                                Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[3].Value = curPos.ToString("0.000");
                                Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[5].Value = Math.Round(curPos * Axis_Config.Instance.MMPixelRoute[Card_ADLink.FindAxisByName(Frm_MotionControl.Instance.dgv_axisInfo.Rows[i].Cells[1].Value).actNo], 3);

                            }

                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_diList.Rows.Count; i++)
                            {
                                string diName = Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[3].Value.ToString();
                                Level level = Card_ADLink.GetDiSts(diName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "Off";
                                    }
                                }
                            }
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_doList.Rows.Count; i++)
                            {
                                string doName = Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[4].Value.ToString();
                                Level level = Card_ADLink.GetDoSts(doName);
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "Off";
                                    }
                                }
                            }
                        }
                    }
                    else if (Project.Instance.configuration.cardType == CardType.安川_MP3100)
                    {
                        if (Card_Ymc3100.initSucceed || Project.Instance.configuration.vitualCard)
                        {
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_diList.Rows.Count; i++)
                            {
                                string diName = Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[3].Value.ToString();
                                Level level = Card_Ymc3100.GetDi((Di)Enum.Parse(typeof(Di), diName));
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_diList.Rows[i].Cells[2].Tag = "Off";
                                    }
                                }
                            }
                            for (int i = 0; i < Frm_MotionControl.Instance.dgv_doList.Rows.Count; i++)
                            {
                                string doName = Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[4].Value.ToString();
                                Level level = Card_Ymc3100.GetDo((Do)Enum.Parse(typeof(Do), doName));
                                if (level == Level.High)
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "On")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.On;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "On";
                                    }
                                }
                                else
                                {
                                    if (Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag.ToString() != "Off")
                                    {
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Value = Resources.Off;
                                        Frm_MotionControl.Instance.dgv_doList.Rows[i].Cells[3].Tag = "Off";
                                    }
                                }
                            }
                        }
                    }

                    //检测启动按钮
                    //////if (Card_Googol.initSucceed)
                    //////{
                    //////    Level statu = Card_Googol.GetDiSts(Di.启动信号);
                    //////    if (statu == Level.High)
                    //////    {
                    //////        StartRun();
                    //////    }
                    //////}

                    ////////监控停止按钮
                    //////if (Card_Googol.initSucceed)
                    //////{
                    //////    Level statu = Card_Googol.GetDiSts(Di.停止信号);
                    //////    if (statu == Level.High)
                    //////    {
                    //////        StopRun();
                    //////    }
                    //////}

                    ////////监控复位按钮
                    //////if (Card_Googol.initSucceed)
                    //////{
                    //////    Level statu = Card_Googol.GetDiSts(Di.停止信号);
                    //////    if (statu == Level.High)
                    //////    {
                    //////        Home();
                    //////    }
                    //////}

                    ////////监控急停按钮
                    //////if (Card_Googol.initSucceed)
                    //////{
                    //////    Level statu = Card_Googol.GetDiSts(Di.急停信号);
                    //////    if (statu == Level.High)
                    //////    {
                    //////        //急停
                    //////    }
                    //////}
                }
                //////}
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 实时刷新线程
        /// </summary>
        internal static void UpdateAll()
        {
            try
            {
                // 该入口由主窗体调度器低频调用。不要在 UI 线程内等待，也不要
                // 在全局资源锁中同步 Invoke；具体设备状态由各自可见工作区刷新。
                UpdateRunStatu();

                    ////////检测启动按钮
                    //////if (Card_Googol.initSucceed)
                    //////{
                    //////    Level statu = Card_Googol.GetDiSts(Di.启动信号);
                    //////    if (statu == Level.High)
                    //////    {
                    //////        StartRun();
                    //////    }
                    //////}

                    ////////监控停止按钮
                    //////if (Card_Googol.initSucceed)
                    //////{
                    //////    Level statu = Card_Googol.GetDiSts(Di.停止信号);
                    //////    if (statu == Level.High)
                    //////    {
                    //////        StopRun();
                    //////    }
                    //////}

                    ////////监控复位按钮
                    //////if (Card_Googol.initSucceed)
                    //////{
                    //////    Level statu = Card_Googol.GetDiSts(Di.停止信号);
                    //////    if (statu == Level.High)
                    //////    {
                    //////        Home();
                    //////    }
                    //////}

                    ////////监控急停按钮
                    //////if (Card_Googol.initSucceed)
                    //////{
                    //////    Level statu = Card_Googol.GetDiSts(Di.急停信号);
                    //////    if (statu == Level.High)
                    //////    {
                    //////        //急停
                    //////    }
                    //////}
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 设备整体复位
        /// </summary>
        public static void Home()
        {
            try
            {
                if (machineRunStatu == MachineRunStatu.Running)
                {
                    Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : "设备运行中，确定要整体复位吗？");
                    Frm_ConfirmBox.Instance.ShowDialog();
                    if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Yes)
                    {
                        Frm_Main.Instance.OutputMsg("程序整体复位成功", Color.Black);
                        Log.SaveLog(LogType.Operate, "程序复位");
                        machineRunStatu = MachineRunStatu.WaitRun;
                    }
                }
                else
                {
                    Frm_Main.Instance.OutputMsg("程序整体复位成功", Color.Black);
                    machineRunStatu = MachineRunStatu.WaitRun;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 开始自动运行
        /// </summary>
        internal static void StartRun()
        {
            try
            {
                if (machineRunStatu == MachineRunStatu.Homing)
                {
                    Frm_Main.Instance.OutputMsg("复位中，请复位完成后开始", System.Drawing.Color.Red);
                    return;
                }
                else if (machineRunStatu == MachineRunStatu.WaitReset)
                {
                    Frm_Main.Instance.OutputMsg("程序未复位，请复位成后开始", System.Drawing.Color.Red);
                    return;
                }
                else
                {


                    Frm_Main.Instance.OutputMsg("开始运行", System.Drawing.Color.Black);
                    Log.SaveLog(LogType.Operate, "开始运行");
                    machineRunStatu = MachineRunStatu.Running;


                    for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                    {
                        if (Project.Instance.curEngine.L_jobList[i].jobRunMode == JobRunMode.LoopRunAfterStart)
                            Project.Instance.curEngine.L_jobList[i].LoopRun(true);
                    }

                    if (Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).jobRunMode == JobRunMode.LoopRunAfterStart)
                    {
                        Frm_Job.Instance.btn_runLoop.Enabled = false;
                        Frm_Job.Instance.btn_runOnce.Enabled = false;
                    }
                    //Frm_UserForm.Instance.Start();
                }
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Failed to start run operation" 
                    : "启动运行失败", "StartRun");
            }
        }
        internal static void SwitchFrom(FormMode formMode)
        {
            Frm_Main.Instance.ApplyWorkspaceMode(formMode);
        }
        /// <summary>
        /// 停止自动运行
        /// </summary>
        internal static void StopRun()
        {
            try
            {
                if (machineRunStatu == MachineRunStatu.Running)
                {

                    Frm_Main.Instance.OutputMsg("停止运行", System.Drawing.Color.Black);
                    Log.SaveLog(LogType.Operate, "停止自动运行");
                    machineRunStatu = MachineRunStatu.Stop;

                    //Frm_UserForm.Instance.Stop();
                    for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                    {
                        Project.Instance.curEngine.L_jobList[i].LoopRun(false);
                    }

                    if (Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).jobRunMode == JobRunMode.LoopRunAfterStart)
                    {
                        Frm_Job.Instance.btn_runLoop.Enabled = true ;
                        Frm_Job.Instance.btn_runOnce.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Failed to stop run operation" 
                    : "停止运行失败", "StopRun");
            }
        }
        /// <summary>
        /// 检查各配置文件
        /// </summary>
        private static void InitConfigDirctory()
        {
            try
            {
                //主配置文件文件夹
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Config\\Project"))
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Config\\Project");

                //标准图像文件夹
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Config\\Project\\Vision\\StandardImage"))
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Config\\Project\\Vision\\StandardImage");

                //运动控制文件夹
                if (!Directory.Exists(Application.StartupPath + "\\Config\\Project\\Motion"))
                    Directory.CreateDirectory(Application.StartupPath + "\\Config\\Project\\Motion");

                //作业文件夹
                if (!Directory.Exists(Application.StartupPath + "\\Config\\Project\\Vision\\Job"))
                    Directory.CreateDirectory(Application.StartupPath + "\\Config\\Project\\Vision\\Job");

                //资源文件夹
                if (!Directory.Exists(Application.StartupPath + "\\Config\\Resources"))
                    Directory.CreateDirectory(Application.StartupPath + "\\Config\\Resources");

                //通讯记录文件夹
                if (!Directory.Exists(Application.StartupPath + "\\Config\\Log\\Comm"))
                    Directory.CreateDirectory(Application.StartupPath + "\\Config\\Log\\Comm");

                //操作记录文件夹
                if (!Directory.Exists(Application.StartupPath + "\\Config\\Log\\Comm"))
                    Directory.CreateDirectory(Application.StartupPath + "\\Config\\Log\\Operate");

                //错误信息保存文件夹
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Config\\Log\\Error"))
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Config\\Log\\Error");

                //主配置文件ini
                if (!File.Exists(System.Windows.Forms.Application.StartupPath + "\\Config\\Config.ini"))
                    File.Create(System.Windows.Forms.Application.StartupPath + "\\Config\\Config.ini").Close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        //////public static void SetPath( string key, string pathValue)
        //////{
        //////    string pathlist;
        //////    pathlist = SysEnvironment.GetSysEnvironmentByName("PATH");
        //////    string[] list = pathlist.Split(';');
        //////    bool isPathExist = false;

        //////    foreach (string item in list)
        //////    {
        //////        if (item == pathValue)
        //////            isPathExist = true;
        //////    }
        //////    if (!isPathExist)
        //////    {
        //////        SetEnvironmentVariable("PATH", pathlist + pathValue + ";");

        //////    }
        //////}
        /// <summary>
        /// 更新运行状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void UpdateRunStatu()
        {
            try
            {
                //////lock (obj)
                //////{
                //////    Frm_Main.Instance.lbl_title.Invoke(new Action(() =>
                //////    {
                //////        if (machineRunStatu == MachineRunStatu.WaitReset)
                //////        {
                //////            Frm_Main.Instance.lbl_runStatu.Text = Project.Instance.configuration.language == Language.English ? "WaitHome......" : "等待复位";
                //////            Frm_Main.Instance.pictureBox1.Image = Resources.Yellow;
                //////            ThreeColorLamp.SetYellow();
                //////        }
                //////        else if (machineRunStatu == MachineRunStatu.Homing)
                //////        {
                //////            Frm_Main.Instance.lbl_runStatu.Text = Project.Instance.configuration.language == Language.English ? "Homing......" : "复位中";
                //////            Frm_Main.Instance.pictureBox1.Image = Resources.Yellow;
                //////            ThreeColorLamp.SetYellow();
                //////        }
                //////        else if (machineRunStatu == MachineRunStatu.WaitRun)
                //////        {
                //////            Frm_Main.Instance.lbl_runStatu.Text = Project.Instance.configuration.language == Language.English ? "WaitRun......" : "等待运行";
                //////            Frm_Main.Instance.pictureBox1.Image = Resources.Yellow;
                //////            ThreeColorLamp.SetYellow();
                //////        }
                //////        else if (machineRunStatu == MachineRunStatu.Running)
                //////        {
                //////            Frm_Main.Instance.lbl_runStatu.Text = Project.Instance.configuration.language == Language.English ? "Runing......" : "运行中";
                //////            Frm_Main.Instance.pictureBox1.Image = Resources.Green;
                //////            ThreeColorLamp.SetGreen();
                //////        }
                //////        else if (machineRunStatu == MachineRunStatu.Stop)
                //////        {
                //////            Frm_Main.Instance.lbl_runStatu.Text = Project.Instance.configuration.language == Language.English ? "Stop......" : "暂停中";
                //////            Frm_Main.Instance.pictureBox1.Image = Resources.Yellow;
                //////            ThreeColorLamp.SetYellow();
                //////        }
                //////        else if (machineRunStatu == MachineRunStatu.Alarm)
                //////        {
                //////            Frm_Main.Instance.lbl_runStatu.Text = Project.Instance.configuration.language == Language.English ? "Alarm......" : "报警中";
                //////            Thread.Sleep(400);
                //////            Frm_Main.Instance.pictureBox1.Image = Resources.Red;
                //////            ThreeColorLamp.SetRed();
                //////        }
                //////    }));
                //////}
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 切换到调试模式
        /// </summary>
        internal static void SwitchToDebugForm()
        {
            try
            {
                //////Frm_Main.Instance.tableLayoutPanel1.Height = 145;
                //////Frm_Main.Instance.statusStrip1.Height = 26;
                //////Frm_Main.Instance.pnl_productFormBox.Visible = false;
                //////Frm_Main.Instance.buttonItem776.Image = Properties.Resources.Product1;
                //////Frm_Main.Instance.buttonItem776.Text = "生产页面";
                //////productionMode = false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 切换到生产状态
        /// </summary>
        internal static void SwitchToProductForm()
        {
            try
            {
                //////Frm_Main.Instance.statusStrip1.AutoSize = false;
                //////Frm_Main.Instance.statusStrip1.Height = 0;
                //////Frm_Main.Instance.pnl_productFormBox.Visible = true;
                //////Frm_UserForm.Instance.TopLevel = false;
                //////Frm_Main.Instance.pnl_productFormBox.Controls.Add(Frm_UserForm.Instance);
                //////Frm_UserForm.Instance.Parent = Frm_Main.Instance.pnl_productFormBox;
                //////Frm_UserForm.Instance.Show();
                //////Frm_UserForm.Instance.Dock = DockStyle.Fill;
                //////productionMode = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 定期清理
        /// </summary>
        private void RegularClear(int dayNum)
        {
            try
            {
                UpdateStep(5, Project.Instance.configuration.language == Language.English ? "Program backup" : "程序备份", true);
                if (Project.Instance.configuration.autoBackupProgram)
                {
                    try
                    {
                        if (Application.StartupPath.Substring(0, 6) == @"C:\Kim")       //我们不允许在备份的文件夹下启动
                        {
                            MessageBox.Show("Do not run this backup program directly, please copy to another path and run again");
                            Process.GetCurrentProcess().Kill();
                        }
                        string date = DateTime.Now.ToString("yyyy-MM-dd");
                        if (Directory.Exists(@"C:\Kim\" + date))
                        {
                            Directory.Delete(@"C:\Kim\" + date, true);
                            Directory.CreateDirectory(@"C:\Kim\" + date);
                        }
                        Frm_Main.CopyFiles(Application.StartupPath, @"C:\Kim\" + date);

                        //清理30天以前的备份
                        DateTime now = DateTime.Now;
                        string[] fileList = Directory.GetDirectories(@"C:\Kim");
                        for (int i = 0; i < fileList.Length; i++)
                        {
                            DirectoryInfo dir = new System.IO.DirectoryInfo(fileList[i]);
                            DateTime dt = dir.CreationTime;
                            if ((now - dt).Days > 30)
                            {
                                File.Delete(fileList[i]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.SaveError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
    /// <summary>
    /// 设备运行状态
    /// </summary>
    internal enum MachineRunStatu
    {
        WaitReset,
        Stop,
        Pause,
        Homing,
        WaitRun,
        Running,
        Alarm,
    }
    public enum FormMode
    {
        None,
        MainForm,
        VisionForm,
        MotionForm,
    }
}
