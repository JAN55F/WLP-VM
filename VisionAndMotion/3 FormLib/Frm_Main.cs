using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Management;
using System.Data.Common;
using System.Data.OleDb;
using System.Net.Sockets;
using System.Net;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using WeifenLuo.WinFormsUI.Docking;
using CameraHandle = System.Int32;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Resources;
using Microsoft.Win32;
using VMPro.Properties;
using SACTools;
using gts;
using Tool;
using System.Xml;
using HalconDotNet;
using Newtonsoft.Json;
using ShareMemNet;


namespace VMPro
{
    internal partial class Frm_Main : Form
    {
        internal Frm_Main()
        {
            deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            //初始化窗体控件
            try
            {
                InitializeComponent();
                ApplyModernMainLayout();
            }
            catch
            {
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Failed to start!, suspected that Halcon is missing a usable License file 或者系统平台不正确" : "启动失败，Halcon已过期或者系统平台不正确");
                Process.GetCurrentProcess().Kill();
            }
            Init_Language();
        }

        private void ClearSampleAcqImageDirectory(Job job)
        {
            if (job == null)
                return;

            try
            {
                AcqImageTool acqTool = Job.FindToolByName(job.jobName, "SDK_Halcon") as AcqImageTool;
                if (acqTool == null)
                    return;

                acqTool.imageDirectoryPath = string.Empty;
                acqTool.imagePath = string.Empty;
                acqTool.lastPreviewImagePath = string.Empty;
                acqTool.L_images.Clear();
                acqTool.currentImageName = string.Empty;
                acqTool.toolPar.ResultPar.图像 = null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        #region 作者相关

        //************************************************************************************************************************************************************************ 
        //*  Author:                              Kim Li    (QQ:1070645289)                                                                                                                        *
        //*  Pen name:                            Bear in the forest                                                                                                                         *
        //*  Guidance Teacher:                    NoBody                                                                                                                         *
        //*  Write Date:                          2018-05-02                                                                                                                     *
        //*  Apartment:                           TianJi Third Apartment                                                                                                         *
        //*  Welcome Sentence:                    Welcome to browse this source code                                                                                             *
        //*  Notes:                               Please do not modify without author's permission                                                                               *
        //*  Record items:                        ①Nothing                                                                                                                      *
        //*  Version Info and Update Details:     ①Vision Number:20180502 1.0.0.0（Initial Vision）                                                                             *  
        //*                                       Update Details:Nothing                                                                                                         *
        //*  Description:                         This software is based on Halcon as a visual processing software, only for personal learning. Due to personal time relations,  *
        //*                                       there are still many bugs and deficiencies in the software. Later, new versions will be continuously updated to optimize and   *
        //*                                       improve the deficiencies. At the same time, users who have better suggestions or corrections are welcome to inform me. Thank   *
        //*                                       you for using                                                                                                                  *
        //************************************************************************************************************************************************************************

        #endregion

        #region 变量定义

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_Main _instance;
        public static Frm_Main Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_Main();
                return _instance;
            }
        }

        /// <summary>
        /// 指示是否允许拖动和缩放
        /// </summary>
        public static bool allowScaleAndZoom = false;

        /// <summary>
        /// 指示是否已按下Ctrl+E组合键
        /// </summary>
        private bool controlE = false;
        /// <summary>
        /// 图像窗口编号
        /// </summary>
        private int imageWindowIndex = 0;
        /// <summary>
        /// 反序列化Dock控件对象
        /// </summary>
        internal DeserializeDockContent deserializeDockContent;
        /// <summary>
        /// 监控线程
        /// </summary>
        public static Thread th_update;
        /// <summary>
        /// 指示是否启用了全屏模式
        /// </summary>
        internal static bool fullScreen = false;
        /// <summary>
        /// 注册码
        /// </summary>
        public string regiestCode;
        /// <summary>
        /// 累计时间
        /// </summary>
        public int elapsedTime = 0;
        /// <summary>
        /// 贪吃蛇游戏进程
        /// </summary>
        internal Process processGreedSnake;
        /// <summary>
        /// 虚拟键盘进程
        /// </summary>
        internal Process processKeyBoard;
        /// <summary>
        /// 配置文件读写对象
        /// </summary>
        private static Ini iniConfig = new Ini(Application.StartupPath + "\\Config.ini");
        /// <summary>
        /// 轴配置窗体对象
        /// </summary>
        private static Frm_AxisSetting frm_axisSetting = new Frm_AxisSetting();

        #endregion

        #region 函数定义

        private void Init_Language()
        {
            try
            {
                if (Project.Instance.configuration.language == Language.English)
                {



                    ////btn_runOnce.ToolTipText = "Run once";
                    ////tsb_runLoop.ToolTipText = "Run loop";

                    tss_permissionInfo.Text = "Current login: not logged in, default is minimum permission";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 整体保存
        /// </summary>
        internal void SaveAll()
        {
            try
            {
                //foreach (TabPage item in Frm_Job.Instance.tbc_jobs.TabPages)
                //{
                //    //如果本地没有此流程，则可能是临时读取的流程，返回，不保存
                //    if (Frm_Job.Instance.tbc_jobs.TabCount > 0)
                //        Save(item.Text);
                //}
                Project.SaveProject();
                //Project .Instance .configuration .Save();
                //////Frm_Job.Instance.Dock = DockStyle.Fill;
                //////Frm_Job.Instance.Show();
                //////Frm_Job.Instance.Dock = DockStyle.Fill;
                //  Frm_Main.Instance.OutputMsg("保存项目成功", Color.Green);
                SaveDockLayout(false);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="persistString"></param>
        /// <returns></returns>
        private IDockContent GetContentFromPersistString(string persistString)
        {
            try
            {
                if (persistString == typeof(Frm_Job).ToString())
                    return Frm_Job.Instance;
                else if (persistString == typeof(Frm_ToolBox).ToString())
                    return Frm_ToolBox.Instance;
                //else if (persistString == typeof(Frm_ImageWindow).ToString() && imageWindowIndex == 0&&Project .Instance .configuration .imageWindowName.Count == 0)
                //{
                //    Project .Instance .configuration .imageWindowName.Add("图像");
                //    Frm_ImageWindow.D_imageWindow.Add(Project .Instance .configuration .language == Language.English ? "Image" : "图像", Frm_ImageWindow.Instance);
                //    imageWindowIndex++;
                //    return Frm_ImageWindow.Instance;
                //}
                else if (persistString == typeof(Frm_Output).ToString())
                    return Frm_Output.Instance;
                else if (persistString == typeof(Frm_ProductData).ToString())
                    return Frm_ProductData.Instance;
                else if (persistString == typeof(Frm_Monitor).ToString())
                    return Frm_Monitor.Instance;
                else if (persistString == typeof(Frm_MotionControl).ToString())
                    // 运动控制现为主窗体内嵌工作区，不再是 DockContent。
                    // 旧代码错误地把它恢复成数值监控页，会造成监控页重复。
                    return null;
                else
                {
                    string[] parsedStrings = persistString.Split(new char[] { ',' });

                    if (parsedStrings[0] != typeof(Frm_ImageWindow).ToString())
                        return null;

                    Frm_ImageWindow dummyDoc = new Frm_ImageWindow();
                    if (Project.Instance.configuration.imageWindowName.Count == 0)
                    {
                        Project.Instance.configuration.imageWindowName.Add("图像");
                    }
                    try
                    {
                        dummyDoc.Text = Project.Instance.configuration.language == Language.English ? "Image" : Project.Instance.configuration.imageWindowName[imageWindowIndex];
                        Frm_ImageWindow.D_imageWindow.Add(Project.Instance.configuration.language == Language.English ? "Image" : Project.Instance.configuration.imageWindowName[imageWindowIndex], dummyDoc);

                        imageWindowIndex++;


                    }
                    catch { }
                    return dummyDoc;
                }
                return Frm_ToolBox.Instance;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }



        /// <summary>
        /// 显示对象
        /// </summary>
        /// <param name="obj">要显示的对象</param>
        internal void Display_Obj(HTuple hw, HObject obj)
        {
            try
            {
                //有开启全屏
                if (fullScreen)
                {
                    HOperatorSet.DispObj(obj, Frm_FullScreen.Instance.windowHandle);
                }
                else
                {
                    HOperatorSet.DispObj(obj, hw);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 开机自启动
        /// </summary>
        /// <param name="isAuto">是否启用</param>
        internal static void Auto_Start(bool isAuto)
        {
            try
            {
                if (isAuto == true)
                {
                    RegistryKey R_local = Registry.LocalMachine;        //RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.SetValue("应用名称", Application.ExecutablePath);
                    R_run.Close();
                    R_local.Close();
                }

                else
                {
                    RegistryKey R_local = Registry.LocalMachine;        //RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.DeleteValue("应用名称", false);
                    R_run.Close();
                    R_local.Close();
                }
            }
            catch (Exception)
            {
                Frm_MessageBox.Instance.MessageBoxShow("\r\n开机程序自启动设置失败，请以管理员权限运行此程序后重新尝试");
            }
        }
        private object obj = new object();
        public delegate void ShowTestData();
        /// <summary>
        /// 信息输出
        /// </summary>
        /// <param name="msg">要输出的信息</param>
        /// <param name="color">背景颜色</param>
        public void OutputMsg(string msg, Color color)
        {
            try
            {
                Frm_Output existingOutput;
                if (Frm_Output.TryGetExistingInstance(out existingOutput))
                {
                    // Frm_Output 只在此处更新线程安全模型；控件绘制由其 UI Timer 合并提交。
                    existingOutput.OutputMsg(msg, color);
                    Interlocked.Exchange(ref elapsedTime, 0);
                    Log.SaveLog(LogType.Operate, msg);
                    return;
                }

                ShowTestData showTestData = delegate()
                {
                    Frm_Output.Instance.OutputMsg(msg, color);
                    Interlocked.Exchange(ref elapsedTime, 0);
                    //lbl_output.Text = DateTime.Now.ToString("HH:mm:ss") + "    " + msg;
                    //if (color == Color.Red)
                    //{
                    //    lbl_output.ForeColor = color;
                    //}
                    //else
                    //{
                    //    lbl_output.ForeColor = color;
                    //}
                };
                if (statusStrip1.IsHandleCreated && statusStrip1.InvokeRequired)
                    statusStrip1.BeginInvoke(showTestData);
                else
                    showTestData();
                Log.SaveLog(LogType.Operate, msg);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        /// <summary>
        /// 整体保存
        /// </summary>
        internal static string Save()
        {
            try
            {
                if (Frm_Job.Instance.tbc_jobs.TabPages.Count == 0)
                {
                    return "";
                }

                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                Job job = Job.FindJobByName(jobName);

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + job.jobName + ".job", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, job);
                stream.Close();

                //更新结果下拉框
                ////// GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).Update_Last_Run_Result_Image_List();
                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Program saved successfully" : "程序保存成功");
                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Program saved successfully" : "流程保存成功", Color.Green);
                return jobName;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return "";
            }
        }

        /// <summary>
        /// 初始化工具提示
        /// </summary>
        public void Init_Tool_Tips()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 10;
            toolTip.ReshowDelay = 10;
            toolTip.ShowAlways = true;
        }



        /// <summary>
        /// 旋转图像
        /// </summary>
        /// <param name="angle"></param>
        private void Rotate_Image(double angle)
        {
            try
            {
                HObject imageAfterRotate;
                HOperatorSet.RotateImage(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).currentImage, out imageAfterRotate, (HTuple)(angle), "constant");
                Frm_ImageWindow.Instance.Display_Image(imageAfterRotate);
                Frm_Main.Instance.OutputMsg("Image rotatate" + angle + "degree Successly", Color.Green);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        /// <summary>
        /// 创建新的图像窗体
        /// </summary>
        /// <returns></returns>
        internal void CreateNewImageWindowWithoutInput()
        {
            try
            {
                string imageWindowName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;


                if (FindImageWindow(imageWindowName) != null)
                {
                    return;
                }
                Frm_ImageWindow dummyDoc = new Frm_ImageWindow();
                dummyDoc.Text = imageWindowName;
                Frm_ImageWindow.D_imageWindow.Add(imageWindowName, dummyDoc);
                Project.Instance.configuration.imageWindowName = Frm_ImageWindow.D_imageWindow.Keys.ToList();
                Frm_JobInfo.Instance.comboBox1.Add(imageWindowName);

                //自动绑定相同名称的流程和窗口
                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    if (Project.Instance.curEngine.L_jobList[i].jobName == imageWindowName)
                    {
                        Project.Instance.curEngine.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).debugImageWindow = imageWindowName;
                        Frm_Main.Instance.OutputMsg(string.Format("图像窗口 [{0}] 已添加，并已自动和流程 [{0}] 进行绑定", imageWindowName, imageWindowName), Color.Black);
                    }
                }

                if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
                {
                    dummyDoc.MdiParent = this;
                    dummyDoc.Show();
                }
                else
                    dummyDoc.Show(dockPanel);


            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return;
            }
        }

        /// <summary>
        /// 创建新的图像窗体
        /// </summary>
        /// <returns></returns>
        internal void CreateNewImageWindow()
        {
            try
            {
                string imageWindowName;
                while (true)
                {
                    using (Frm_InputMessage inputMessage = new Frm_InputMessage())
                    {
                        Frm_InputMessage.input = string.Empty;
                        inputMessage.txt_input.DefaultText = "请输入新流程名";
                        inputMessage.txt_input.TextStr = Frm_Job.Instance.tbc_jobs.SelectedTab == null ? "图像" : Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                        inputMessage.lbl_title.Text = Project.Instance.configuration.language == Language.English ? "Please input name of standard image" : "请输入图像窗体名称";
                        inputMessage.btn_confirm.Text = Project.Instance.configuration.language == Language.English ? "OK" : "确定";
                        inputMessage.TopMost = true;
                        inputMessage.ShowDialog();
                        imageWindowName = Frm_InputMessage.input;
                    }

                    if (string.IsNullOrEmpty(imageWindowName))
                        return;
                    if (FindImageWindow(imageWindowName) == null)
                        break;

                    Frm_MessageBox.Instance.MessageBoxShow("\r\n已经存在此名称的图像窗体，名称不可重复，请重新输入！");
                }

                Frm_ImageWindow dummyDoc = new Frm_ImageWindow();
                dummyDoc.Text = imageWindowName;
                Frm_ImageWindow.D_imageWindow.Add(imageWindowName, dummyDoc);
                Project.Instance.configuration.imageWindowName = Frm_ImageWindow.D_imageWindow.Keys.ToList();
                Frm_JobInfo.Instance.comboBox1.Add(imageWindowName);

                //自动绑定相同名称的流程和窗口
                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    if (Project.Instance.curEngine.L_jobList[i].jobName == imageWindowName)
                    {
                        Project.Instance.curEngine.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).debugImageWindow = imageWindowName;
                        Frm_Main.Instance.OutputMsg(string.Format("图像窗口 [{0}] 已添加，并已自动和流程 [{0}] 进行绑定", imageWindowName, imageWindowName), Color.Black);
                    }
                }

                if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
                {
                    dummyDoc.MdiParent = this;
                    dummyDoc.Show();
                }
                else
                    dummyDoc.Show(dockPanel);


            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return;
            }
        }

        /// <summary>
        /// 在图像中显示字符串
        /// </summary>
        internal void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem, HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            try
            {
                HTuple hv_M = null, hv_N = null, hv_Red = null;
                HTuple hv_Green = null, hv_Blue = null, hv_RowI1Part = null;
                HTuple hv_ColumnI1Part = null, hv_RowI2Part = null, hv_ColumnI2Part = null;
                HTuple hv_RowIWin = null, hv_ColumnIWin = null, hv_WidthWin = null;
                HTuple hv_HeightWin = null, hv_I = null, hv_RowI = new HTuple();
                HTuple hv_ColumnI = new HTuple(), hv_StringI = new HTuple();
                HTuple hv_MaxAscent = new HTuple(), hv_MaxDescent = new HTuple();
                HTuple hv_MaxWidth = new HTuple(), hv_MaxHeight = new HTuple();
                HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRowI = new HTuple();
                HTuple hv_FactorColumnI = new HTuple(), hv_UseShadow = new HTuple();
                HTuple hv_ShadowColor = new HTuple(), hv_Exception = new HTuple();
                HTuple hv_Width = new HTuple(), hv_Index = new HTuple();
                HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
                HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
                HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
                HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
                HTuple hv_CurrentColor = new HTuple();
                HTuple hv_Box_COPY_INP_TMP = hv_Box.Clone();
                HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
                HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
                HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
                HTuple hv_String_COPY_INP_TMP = hv_String.Clone();
                if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
                {
                    hv_Color_COPY_INP_TMP = "";
                }
                if ((int)(new HTuple(hv_Box_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
                {
                    hv_Box_COPY_INP_TMP = "false";
                }
                hv_M = (new HTuple(hv_Row_COPY_INP_TMP.TupleLength())) * (new HTuple(hv_Column_COPY_INP_TMP.TupleLength()
                    ));
                hv_N = new HTuple(hv_Row_COPY_INP_TMP.TupleLength());
                if ((int)((new HTuple(hv_M.TupleEqual(0))).TupleOr(new HTuple(hv_String_COPY_INP_TMP.TupleEqual(
                    new HTuple())))) != 0)
                {
                    return;
                }
                if ((int)(new HTuple(hv_M.TupleNotEqual(1))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_Row_COPY_INP_TMP.TupleLength())).TupleEqual(
                        1))) != 0)
                    {
                        hv_N = new HTuple(hv_Column_COPY_INP_TMP.TupleLength());
                        HOperatorSet.TupleGenConst(hv_N, hv_Row_COPY_INP_TMP, out hv_Row_COPY_INP_TMP);
                    }
                    else if ((int)(new HTuple((new HTuple(hv_Column_COPY_INP_TMP.TupleLength()
                        )).TupleEqual(1))) != 0)
                    {
                        HOperatorSet.TupleGenConst(hv_N, hv_Column_COPY_INP_TMP, out hv_Column_COPY_INP_TMP);
                    }
                    else if ((int)(new HTuple((new HTuple(hv_Column_COPY_INP_TMP.TupleLength()
                        )).TupleNotEqual(new HTuple(hv_Row_COPY_INP_TMP.TupleLength())))) != 0)
                    {
                        throw new HalconException("Number of elements in Row and Column does not match.");
                    }
                    if ((int)(new HTuple((new HTuple(hv_String_COPY_INP_TMP.TupleLength())).TupleEqual(
                        1))) != 0)
                    {
                        HOperatorSet.TupleGenConst(hv_N, hv_String_COPY_INP_TMP, out hv_String_COPY_INP_TMP);
                    }
                    else if ((int)(new HTuple((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                        )).TupleNotEqual(hv_N))) != 0)
                    {
                        throw new HalconException("Number of elements in Strings does not match number of positions.");
                    }
                }
                HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
                HOperatorSet.GetPart(hv_WindowHandle, out hv_RowI1Part, out hv_ColumnI1Part,
                    out hv_RowI2Part, out hv_ColumnI2Part);
                HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowIWin, out hv_ColumnIWin,
                    out hv_WidthWin, out hv_HeightWin);
                HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
                HTuple end_val89 = hv_N - 1;
                HTuple step_val89 = 1;
                for (hv_I = 0; hv_I.Continue(end_val89, step_val89); hv_I = hv_I.TupleAdd(step_val89))
                {
                    hv_RowI = hv_Row_COPY_INP_TMP.TupleSelect(hv_I);
                    hv_ColumnI = hv_Column_COPY_INP_TMP.TupleSelect(hv_I);
                    if ((int)(new HTuple(hv_N.TupleEqual(1))) != 0)
                    {
                        hv_StringI = hv_String_COPY_INP_TMP.Clone();
                    }
                    else
                    {
                        hv_StringI = hv_String_COPY_INP_TMP.TupleSelect(hv_I);
                    }
                    if ((int)(new HTuple(hv_RowI.TupleEqual(-1))) != 0)
                    {
                        hv_RowI = 12;
                    }
                    if ((int)(new HTuple(hv_ColumnI.TupleEqual(-1))) != 0)
                    {
                        hv_ColumnI = 12;
                    }
                    hv_StringI = ((("" + hv_StringI) + "")).TupleSplit("\n");
                    HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                        out hv_MaxWidth, out hv_MaxHeight);
                    if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
                    {
                        hv_R1 = hv_RowI.Clone();
                        hv_C1 = hv_ColumnI.Clone();
                    }
                    else
                    {
                        hv_FactorRowI = (1.0 * hv_HeightWin) / ((hv_RowI2Part - hv_RowI1Part) + 1);
                        hv_FactorColumnI = (1.0 * hv_WidthWin) / ((hv_ColumnI2Part - hv_ColumnI1Part) + 1);
                        hv_R1 = (((hv_RowI - hv_RowI1Part) + 0.5) * hv_FactorRowI) - 0.5;
                        hv_C1 = (((hv_ColumnI - hv_ColumnI1Part) + 0.5) * hv_FactorColumnI) - 0.5;
                    }
                    hv_UseShadow = 1;
                    hv_ShadowColor = "gray";
                    if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleEqual("true"))) != 0)
                    {
                        if (hv_Box_COPY_INP_TMP == null)
                            hv_Box_COPY_INP_TMP = new HTuple();
                        hv_Box_COPY_INP_TMP[0] = "#fce9d4";
                        hv_ShadowColor = "#f28d26";
                    }
                    if ((int)(new HTuple((new HTuple(hv_Box_COPY_INP_TMP.TupleLength())).TupleGreater(
                        1))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual("true"))) != 0)
                        {
                        }
                        else if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual(
                            "false"))) != 0)
                        {
                            hv_UseShadow = 0;
                        }
                        else
                        {
                            hv_ShadowColor = hv_Box_COPY_INP_TMP.TupleSelect(1);
                            try
                            {
                                HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                                    1));
                            }
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                                hv_Exception = new HTuple("Wrong value of control parameter Box[1] (must be a 'true', 'false', or a valid color string)");
                                throw new HalconException(hv_Exception);
                            }
                        }
                    }
                    if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleNotEqual("false"))) != 0)
                    {
                        try
                        {
                            HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                                0));
                        }
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            hv_Exception = new HTuple("Wrong value of control parameter Box[0] (must be a 'true', 'false', or a valid color string)");
                            throw new HalconException(hv_Exception);
                        }
                        hv_StringI = (" " + hv_StringI) + " ";
                        hv_Width = new HTuple();
                        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_StringI.TupleLength()
                            )) - 1); hv_Index = (int)hv_Index + 1)
                        {
                            HOperatorSet.GetStringExtents(hv_WindowHandle, hv_StringI.TupleSelect(hv_Index),
                                out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                            hv_Width = hv_Width.TupleConcat(hv_W);
                        }
                        hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_StringI.TupleLength()));
                        hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
                        hv_R2 = hv_R1 + hv_FrameHeight;
                        hv_C2 = hv_C1 + hv_FrameWidth;
                        HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                        HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                        HOperatorSet.SetColor(hv_WindowHandle, hv_ShadowColor);
                        if ((int)(hv_UseShadow) != 0)
                        {
                            HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 1, hv_C1 + 1, hv_R2 + 1,
                                hv_C2 + 1);
                        }
                        HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                        HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                        HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
                    }
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_StringI.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        if ((int)(new HTuple(hv_N.TupleEqual(1))) != 0)
                        {
                            hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                                )));
                        }
                        else
                        {
                            hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_I % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                                )));
                        }
                        if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                            "auto")))) != 0)
                        {
                            try
                            {
                                HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                            }
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                                hv_Exception = ((("Wrong value of control parameter Color[" + (hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                                    )))) + "] == '") + hv_CurrentColor) + "' (must be a valid color string)";
                                throw new HalconException(hv_Exception);
                            }
                        }
                        else
                        {
                            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                        }
                        hv_RowI = hv_R1 + (hv_MaxHeight * hv_Index);
                        HOperatorSet.SetTposition(hv_WindowHandle, hv_RowI, hv_C1);
                        HOperatorSet.WriteString(hv_WindowHandle, hv_StringI.TupleSelect(hv_Index));
                    }
                }
                HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                HOperatorSet.SetPart(hv_WindowHandle, hv_RowI1Part, hv_ColumnI1Part, hv_RowI2Part,
                    hv_ColumnI2Part);
                return;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 显示模板匹配结果
        /// </summary>
        internal void dev_display_shape_Match_results(HTuple hv_ModelID, HTuple hv_Color, HTuple hv_Row, HTuple hv_Column, HTuple hv_Angle, HTuple hv_ScaleR, HTuple hv_ScaleC, HTuple hv_Model)
        {
            try
            {
                HObject ho_ModelContours = null, ho_ContoursAffinTrans = null;
                HTuple hv_NumMatches = null, hv_Index = new HTuple();
                HTuple hv_Match = new HTuple(), hv_HomMat2DIdentity = new HTuple();
                HTuple hv_HomMat2DScale = new HTuple(), hv_HomMat2DRotate = new HTuple();
                HTuple hv_HomMat2DTranslate = new HTuple();
                HTuple hv_Model_COPY_INP_TMP = hv_Model.Clone();
                HTuple hv_ScaleC_COPY_INP_TMP = hv_ScaleC.Clone();
                HTuple hv_ScaleR_COPY_INP_TMP = hv_ScaleR.Clone();
                HOperatorSet.GenEmptyObj(out ho_ModelContours);
                HOperatorSet.GenEmptyObj(out ho_ContoursAffinTrans);
                hv_NumMatches = new HTuple(hv_Row.TupleLength());
                if ((int)(new HTuple(hv_NumMatches.TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_ScaleR_COPY_INP_TMP.TupleLength())).TupleEqual(
                        1))) != 0)
                    {
                        HOperatorSet.TupleGenConst(hv_NumMatches, hv_ScaleR_COPY_INP_TMP, out hv_ScaleR_COPY_INP_TMP);
                    }
                    if ((int)(new HTuple((new HTuple(hv_ScaleC_COPY_INP_TMP.TupleLength())).TupleEqual(
                        1))) != 0)
                    {
                        HOperatorSet.TupleGenConst(hv_NumMatches, hv_ScaleC_COPY_INP_TMP, out hv_ScaleC_COPY_INP_TMP);
                    }
                    if ((int)(new HTuple((new HTuple(hv_Model_COPY_INP_TMP.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        HOperatorSet.TupleGenConst(hv_NumMatches, 0, out hv_Model_COPY_INP_TMP);
                    }
                    else if ((int)(new HTuple((new HTuple(hv_Model_COPY_INP_TMP.TupleLength()
                        )).TupleEqual(1))) != 0)
                    {
                        HOperatorSet.TupleGenConst(hv_NumMatches, hv_Model_COPY_INP_TMP, out hv_Model_COPY_INP_TMP);
                    }
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ModelID.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        ho_ModelContours.Dispose();
                        HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID.TupleSelect(
                            hv_Index), 1);
                        HOperatorSet.SetColor(Frm_Main.fullScreen ? Frm_FullScreen.Instance.windowHandle : Frm_ImageWindow.Instance.WindowHandle, hv_Color.TupleSelect(
                            hv_Index % (new HTuple(hv_Color.TupleLength()))));
                        HTuple end_val18 = hv_NumMatches - 1;
                        HTuple step_val18 = 1;
                        for (hv_Match = 0; hv_Match.Continue(end_val18, step_val18); hv_Match = hv_Match.TupleAdd(step_val18))
                        {
                            if ((int)(new HTuple(hv_Index.TupleEqual(hv_Model_COPY_INP_TMP.TupleSelect(
                                hv_Match)))) != 0)
                            {
                                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                                HOperatorSet.HomMat2dScale(hv_HomMat2DIdentity, hv_ScaleR_COPY_INP_TMP.TupleSelect(
                                    hv_Match), hv_ScaleC_COPY_INP_TMP.TupleSelect(hv_Match), 0, 0, out hv_HomMat2DScale);
                                HOperatorSet.HomMat2dRotate(hv_HomMat2DScale, hv_Angle.TupleSelect(hv_Match),
                                    0, 0, out hv_HomMat2DRotate);
                                HOperatorSet.HomMat2dTranslate(hv_HomMat2DRotate, hv_Row.TupleSelect(
                                    hv_Match), hv_Column.TupleSelect(hv_Match), out hv_HomMat2DTranslate);
                                ho_ContoursAffinTrans.Dispose();
                                HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ContoursAffinTrans,
                                    hv_HomMat2DTranslate);
                                //HOperatorSet.DispObj(ho_ContoursAffinTrans, Frm_ImageWindow.Instance.dip_displayImage.HalconWindow);
                                Display_Obj(Frm_ImageWindow.Instance.WindowHandle, ho_ContoursAffinTrans);
                            }
                        }
                    }
                }
                ho_ModelContours.Dispose();
                ho_ContoursAffinTrans.Dispose();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 把一个目录下的文件拷贝的目标目录下
        /// </summary>
        /// <param name="sourceFolder">源目录</param>
        /// <param name="targerFolder">目标目录</param>
        /// <param name="removePrefix">移除文件名部分路径</param>
        internal static void CopyFiles(string sourceFolder, string targerFolder, string removePrefix = "")
        {
            try
            {
                if (string.IsNullOrEmpty(removePrefix))
                {
                    removePrefix = sourceFolder;
                }
                if (!Directory.Exists(targerFolder))
                {
                    Directory.CreateDirectory(targerFolder);
                }
                DirectoryInfo directory = new DirectoryInfo(sourceFolder);
                //获取目录下的文件
                FileInfo[] files = directory.GetFiles();
                foreach (FileInfo item in files)
                {
                    if (item.Name == "Thumbs.db")
                    {
                        continue;
                    }
                    string tempPath = item.FullName.Replace(removePrefix, string.Empty);
                    tempPath = targerFolder + tempPath;
                    FileInfo fileInfo = new FileInfo(tempPath);
                    if (!fileInfo.Directory.Exists)
                    {
                        fileInfo.Directory.Create();
                    }
                    File.Delete(tempPath);
                    item.CopyTo(tempPath, true);
                }
                //获取目录下的子目录
                DirectoryInfo[] directors = directory.GetDirectories();
                foreach (var item in directors)
                {
                    CopyFiles(item.FullName, targerFolder, removePrefix);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 获取图像窗体
        /// </summary>
        /// <param name="text">窗体名</param>
        /// <returns></returns>
        private IDockContent FindImageWindow(string text)
        {
            try
            {
                if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
                {
                    foreach (Form form in MdiChildren)
                        if (form.Text == text)
                            return form as IDockContent;
                    return null;
                }
                else
                {
                    foreach (IDockContent content in dockPanel.Documents)
                        if (content.DockHandler.TabText == text)
                            return content;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }

        #endregion

        #region 相关事件
        private void tss_permissionInfo_Click(object sender, EventArgs e)
        {
            Frm_Login.Instance.ShowDialog();
        }
        private void btn_startRun_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Operator))
                    return;
                Machine.StartRun();
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Start run button click failed" 
                    : "启动按钮点击失败");
            }
        }
        private void Frm_Main_Resize(object sender, EventArgs e)
        {
            int length = 0;
            //foreach (ToolStripItem item in statusStrip1.Items)
            //{
            //    if (item.Name != "lbl_output")
            //        length += item.Width;
            //}
            ////lbl_output.Size = new Size(length  - 15, lbl_output.Size.Height);
            //Frm_Main.Instance.lbl_output.Size = new Size(Frm_Main.Instance.statusStrip1.Size.Width - length - 15, Frm_Main.Instance.lbl_output.Size.Height);

            //lbl_output.Size = new Size(statusStrip1.Size.Width - lbl_runStatu.Size.Width - tss_curTime.Size.Width - tss_permissionInfo.Size.Width - 15, lbl_output.Size.Height);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if ((keyData & Keys.Control) == Keys.Control)
                {
                    if ((keyData & Keys.M) == Keys.M)
                    {
                        btn_changeMode_Click(null, null);
                    }
                    if ((keyData & Keys.W) == Keys.W)
                    {
                        controlE = true;
                        return true;
                    }
                    else if ((keyData & Keys.E) == Keys.E && controlE)
                    {
                        Frm_ImageWindow.Instance.Show(dockPanel, DockState.DockTop);
                    }
                }
                //Delete删除工具
                else if (keyData == Keys.Delete)
                {
                    if (Frm_Job.Instance.tbc_jobs.Focused)
                    {
                        Job.DeleteJob();
                    }
                    else
                    {
                        IDockContent temp = dockPanel.ActiveContent;
                        Frm_Job frm_job = temp as Frm_Job;
                        if (frm_job != null)
                        {
                            TreeNode selectedNode = Job.GetJobTree(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).SelectedNode;
                            Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).DeleteItem(null, null);
                        }
                    }
                }
                controlE = false;
                return base.ProcessCmdKey(ref msg, keyData);
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Keyboard shortcut processing failed" 
                    : "快捷键处理失败", "ProcessCmdKey");
                return false;
            }
        }
        /// <summary>
        /// 通过流程名获取窗体句柄
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        internal Frm_ImageWindow GetImageWindowControl(string jobName)
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        if (Machine.machineRunStatu != MachineRunStatu.Running)
                        {
                            item.Value.Show();              //切换到当前图像窗体
                        }
                        return item.Value;
                    }
                }
                //////Frm_Main.Instance.OutputMsg(Project .Instance .configuration .language == Language.English ? "The process was successfully run,Elapsed：" : "此流程所绑定的窗体不存在，已自动更换为默认图像窗体", Color.Red);
                //////Job.GetJobByName(jobName).debugImageWindow = Frm_ImageWindow.Instance.Text;
                return Frm_ImageWindow.Instance;
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Failed to get image window" 
                    : "获取图像窗体失败", "GetImageWindowControl");
                return Frm_ImageWindow.Instance;
            }
        }
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.Opacity = 0;

                try
                {
                    switch (Project.Instance.configuration.cardType)
                    {
                        case CardType.雷塞_DMC2210:
                            csDmc2210.Dmc2210.d2210_board_close();
                            break;
                        case CardType.雷塞_DMC2410:
                            csDmc2410.Dmc2410.d2410_board_close();
                            break;
                        case CardType.凌华_AMP204C:
                            Card_ADLink.CloseBoard();
                            break;
                        case CardType.安川_MP3100:
                            Card_Ymc3100.CloseBoard();
                            break;
                    }
                }
                catch { }
                try
                {
                    SDK_HIKVision.CloseAllCamera();
                }
                catch { }
                try
                {
                    SDK_Basler.CloseAllCamera();
                }
                catch { }
                try
                {
                    SDK_MindVision.CloseAllCamera();
                }
                catch { }
                try
                {
                    SDK_PointGrey.CloseAllCamera();
                }
                catch { }
                Frm_CloseTip frm_closeTip = new Frm_CloseTip();
                frm_closeTip.Show();
                try
                {
                    ShareMemProH.PH_Exit();
                }
                catch { }

                //关闭设备
                for (int i = 0; i < Project.Instance.L_lightController.Count; i++)
                {

                    if (Project.Instance.L_lightController[i].CloseAllChBeforeClose)
                        Project.Instance.L_lightController[i].CloseAllChannel();
                }
                for (int i = 0; i < Project.Instance.L_TCPClient.Count; i++)
                {

                    if (Project.Instance.L_TCPClient[i].AutoDisconnectBeforeClose)
                        Project.Instance.L_TCPClient[i].Close();
                }
                for (int i = 0; i < Project.Instance.L_TCPSever.Count; i++)
                {
                    //服务端同样支持“程序关闭前自动断开”：停止监听并断开所有已接入客户端
                    if (Project.Instance.L_TCPSever[i].AutoDisconnectBeforeClose)
                        Project.Instance.L_TCPSever[i].Close();
                }
                for (int i = 0; i < Project.Instance.L_PLCDevice.Count; i++)
                {
                    if (Project.Instance.L_PLCDevice[i].AutoDisconnectBeforeClose)
                        Project.Instance.L_PLCDevice[i].Disconnect();
                }
                for (int i = 0; i < Project.Instance.L_Scaner.Count; i++)
                {
                    Project.Instance.L_Scaner[i].Close();
                }

                //清除图像窗体里面的ROI
                ////// GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.viewWindow.resetWindowImage();
                ////// GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.ClearWindow();
                ////// GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.viewWindow._hWndControl.roiManager.reset();
                ////// GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.viewWindow._hWndControl.roiManager.ROIList.Clear();

                Application.DoEvents();
                this.ShowInTaskbar = false;
                //关闭已连接的Socket
                //////if (Frm_TCPClient.Instance.socket != null && Frm_TCPClient.Instance.socket.Connected)
                //////{
                //////    Frm_TCPClient.Instance.socket.Disconnect(false);
                //////}
                //////if (Frm_TCPServer.Instance.commSkt != null && Frm_TCPServer.Instance.commSkt.Connected)
                //////{
                //////    Frm_TCPServer.Instance.commSkt.Disconnect(false);
                //////}
                //保存配置信息
                //Project .Instance .configuration .Save();

                //要返回到作业编辑界面以下，否则会有一些修改过得参数不能保存


                //////if (Frm_TCPClient.Instance.socket != null && Frm_TCPClient.Instance.socket.Connected)
                //////{
                //////    Frm_TCPClient.Instance.socket.Disconnect(false);
                //////    Frm_TCPClient.Instance.socket.Close();
                //////}
                try
                {
                    //ImageAcqTool.Close_All_Camera();
                }
                catch { }
                Log.SaveLog(LogType.Operate, "程序关闭\r\n");
                frm_closeTip.Close();
                Application.DoEvents();
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_Main_Shown(object sender, EventArgs e)
        {
            try
            {
                Frm_Welcome.Instance.Hide();
                string localRegiestCode = iniConfig.IniReadValue("Regiest", "RegiestCode");

                //////string needLogin = iniConfig.IniReadValue("Login", "NeedPassword");
                //////if (localRegiestCode != regiestCode)
                //////{
                //////    Frm_Regiest.Instance.ShowDialog();
                //////}
                Application.DoEvents();
                //  Thread.Sleep(1000);
                this.Opacity = 1;



                if (Project.Instance.configuration.showProductionFormAfterStart)
                    Machine.SwitchToProductForm();
                else
                    Machine.SwitchToDebugForm();
                //////if (Frm_Job.Instance.tbc_jobs.TabCount > 0)
                //////    Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).DrawLine();

                if (Project.Instance.configuration.hideMenuAfterStart)
                    buttonItem2_Click(null, null);

                //int length = 0;
                //foreach (ToolStripItem item in statusStrip1.Items)
                //{
                //    if (item.Name != "lbl_output")
                //        length += item.Width;
                //}
                ////lbl_output.Size = new Size(length - 15, lbl_output.Size.Height);
                //Frm_Main.Instance.lbl_output.Size = new Size(Frm_Main.Instance.statusStrip1.Size.Width - length - 15, Frm_Main.Instance.lbl_output.Size.Height);

                //////lbl_output.Size = new Size(statusStrip1.Size.Width - lbl_runStatu.Size.Width - tss_curTime.Size.Width - tss_permissionInfo.Size.Width - 15, lbl_output.Size.Height);
                //////Frm_Login frm_login = new Frm_Login();
                //////frm_login.ShowDialog();


                //////for (int i = 0; i < Frm_Job.Instance.tbc_jobs.TabCount; i++)
                //////{
                //////    Job.FindJobByName(Frm_Job.Instance.tbc_jobs.TabPages[i].Text).Run();
                //////}
                //////Frm_OPTLightTool.Instance.ctl_opt.ReFlush();



                //HOperatorSet.ReadImage(out image, Application.StartupPath + "\\EmptyImage.jpg");
                //foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                //{
                //    item.Value.hwc_imageWindow.HobjectToHimage(image);
                //}
                Frm_Output.Instance.ClearLog();
                Thread.Sleep(200);
                if (Project.Instance.configuration.autoRunAfterStart)
                    toolStripButton4_Click(null, null);

                if (Project.Instance.configuration.autoLockAfterStart)
                {
                    Frm_Lock frm_lock = new Frm_Lock();
                    frm_lock.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }

        }
        HObject image;
        private void Frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Project.Instance.configuration.saveWhenExit)
                {
                    Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : "\r\n退出前是否需要保存项目？");
                    Frm_ConfirmBox.Instance.ShowDialog();
                    if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Yes)
                    {
                        Project.SaveProject();
                        // SaveAll();
                        Project.Instance.configuration.Save();

                        SaveDockLayout(false);
                    }
                    else if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : "\r\n确定要退出吗？");
                    Frm_ConfirmBox.Instance.ShowDialog();
                    if (Frm_ConfirmBox.Instance.Result != ConfirmBoxResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                Machine.willExit = true;
                if (processGreedSnake != null && !processGreedSnake.HasExited)
                    processGreedSnake.Kill();
                if (processKeyBoard != null && !processKeyBoard.HasExited)
                    processKeyBoard.Kill();

                string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
                dockPanel.SaveAsXml(configFile);



                //////if (经典布局1ToolStripMenuItem.Checked)
                //////    Project .Instance .configuration .layoutFilePath = Project .Instance .configuration .layoutFilePath = Application.StartupPath + "\\Resources\\Layout\\" + "ClassicalLayout1.config";
                //////else if (经典布局2ToolStripMenuItem.Checked)
                //////    Project .Instance .configuration .layoutFilePath = Project .Instance .configuration .layoutFilePath = Application.StartupPath + "\\Resources\\Layout\\" + "ClassicalLayout2.config";
                //////else if (经典布局3ToolStripMenuItem.Checked)
                //////    Project .Instance .configuration .layoutFilePath = Project .Instance .configuration .layoutFilePath = Application.StartupPath + "\\Resources\\Layout\\" + "ClassicalLayout3.config";
                //////else
                //////    Project .Instance .configuration .layoutFilePath = configFile;
                Project.Instance.configuration.mainFormWidth = this.Size.Width;
                Project.Instance.configuration.mainFormHeight = this.Size.Height;


                Frm_Job.Instance.Hide();

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private static List<HWindowControl> tt = new List<HWindowControl>();
        public static List<HWindowControl> d = new List<HWindowControl>();
        private void Frm_Main_Load(object sender, EventArgs e)
        {
            try
            {
                // 添加左中右布局菜单项
                InitializeLayoutMenu();

                Machine.curFormMode = FormMode.None;
                switch (Project.Instance.configuration.defaultForm)
                {
                    case FormMode.MainForm:
                        Machine.SwitchFrom(FormMode.MainForm);
                        break;
                    case FormMode.VisionForm:
                        Machine.SwitchFrom(FormMode.VisionForm);
                        break;
                    case FormMode.MotionForm:
                        Machine.SwitchFrom(FormMode.MotionForm);
                        break;
                    default:
                        Machine.SwitchFrom(FormMode.VisionForm);
                        break;
                }

                try
                {
                    MigrateLegacySampleLayoutIfNeeded();
                    string selectedLayoutPath = ResolveDockLayoutPath(Project.Instance.configuration.layoutFilePath);
                    if (File.Exists(selectedLayoutPath))
                        Frm_Main.Instance.dockPanel.LoadFromXml(selectedLayoutPath, Frm_Main.Instance.deserializeDockContent);
                    else
                        Frm_Main.Instance.dockPanel.LoadFromXml(ResolveDockLayoutPath("Config\\Resources\\Layout\\经典布局1.config"), Frm_Main.Instance.deserializeDockContent);
                }
                catch { }

                // 布局创建完图像窗口后统一清空。没有本次会话的新输入时，
                // 主界面和所有小图像窗口均显示 Halcon 默认黑色背景。
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    item.Value.currentImage = null;
                    item.Value.hwc_imageWindow.ClearWindow();
                }



                // 不在主窗体显示前预跑采集工具。采集工具可能访问相机 SDK/外设，
                // 驱动或运行库异常会导致进程直接退出，影响软件启动。


                ToolTip toolTip = new ToolTip();
                toolTip.AutoPopDelay = 5000;
                toolTip.InitialDelay = 10;
                toolTip.ReshowDelay = 10;
                toolTip.ShowAlways = true;
                //////toolTip.SetToolTip(btn_startRun, Project .Instance .configuration .language == Language.English ? "Start" : "开始运行");
                //////toolTip.SetToolTip(btn_stopRun, Project .Instance .configuration .language == Language.English ? "Stop" : "停止运行");
                //////toolTip.SetToolTip(btn_allHome, Project .Instance .configuration .language == Language.English ? "Home" : "整体复位");
                //////toolTip.SetToolTip(btn_changeMode, Project .Instance .configuration .language == Language.English ? "Product/debug" : "生产/调试切换");
                //////toolTip.SetToolTip(btn_changeUser, Project .Instance .configuration .language == Language.English ? "Switch user" : "切换用户");
                //////toolTip.SetToolTip(btn_exit, Project .Instance .configuration .language == Language.English ? "Exit" : "退出");

                //////tsm_lockLayout.Checked = Project .Instance .configuration .lockLayout;
                dockPanel.AllowEndUserDocking = !Project.Instance.configuration.lockLayout;

                // 首页和运动页按当前工作区延迟创建，避免启动时加载隐藏页面及其轮询资源。
                EnsureEmbeddedWorkspace(Machine.curFormMode);

                //////if (Project.Instance.configuration.layoutFilePath.Contains("经典布局1"))
                //////    checkBoxItem1.Checked = true;
                //////else if (Project.Instance.configuration.layoutFilePath.Contains("经典布局2"))
                //////    checkBoxItem2.Checked = true;

                for (int i = 0; i < Frm_ImageWindow.D_imageWindow.Keys.Count; i++)
                {
                    Frm_JobInfo.Instance.comboBox1.Add(Frm_ImageWindow.D_imageWindow.Keys.ToArray()[i]);
                }

                //  if (Frm_ImageWindow.Instance.DockState == DockState.Hidden)
                if (Frm_ImageWindow.D_imageWindow.Count == 0)
                {
                    string defaultImageWindowName;
                    if (Project.Instance.configuration.imageWindowName.Count > 0 &&
                        !string.IsNullOrWhiteSpace(Project.Instance.configuration.imageWindowName[0]))
                    {
                        defaultImageWindowName = Project.Instance.configuration.imageWindowName[0];
                    }
                    else
                    {
                        defaultImageWindowName = Project.Instance.configuration.language == Language.English ? "Image" : "图像";
                        Project.Instance.configuration.imageWindowName.Clear();
                        Project.Instance.configuration.imageWindowName.Add(defaultImageWindowName);
                    }

                    Frm_ImageWindow defaultImageWindow = Frm_ImageWindow.Instance;
                    defaultImageWindow.Text = defaultImageWindowName;
                    Frm_ImageWindow.D_imageWindow.Add(defaultImageWindowName, defaultImageWindow);
                    defaultImageWindow.Show(dockPanel);
                }

                Frm_ImageWindow firstImageWindow = Frm_ImageWindow.D_imageWindow.Values.FirstOrDefault(
                    imageWindow => imageWindow != null && !imageWindow.IsDisposed);
                if (firstImageWindow != null)
                    firstImageWindow.Activate();

                //Frm_UserForm.Init();
                //Frm_UserForm.Instance.timer_lowSpeed.Enabled = true;
                //ToolStripButton tsb = new ToolStripButton("",Resources.BlobAnalyseTool);
                //statusStrip1.Items.Add(tsb );

                //开启实时刷新线程
                //////Frm_Main.th_update = new Thread(Machine.UpdateAll);
                //////Frm_Main.th_update.IsBackground = true;
                //////Frm_Main.th_update.Start();
                ModernUiTheme.RefreshToolStripItems(this);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_changeMode_Click(object sender, EventArgs e)
        {
            try
            {
                if (Machine.productionMode)
                {
                    Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Switch to debug page" : "切换到调试页面");
                    Frm_Login.Instance.ShowDialog();
                }
                else
                {
                    Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Switch to production page" : "切换到生产页面");
                    Machine.SwitchToProductForm();
                }
            }
            catch (Exception ex)
            {
                Log.SaveErrorAndShow(ex, Project.Instance.configuration.language == Language.English 
                    ? "Failed to change mode" 
                    : "切换模式失败");
            }
        }
        private void buttonItem14_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void buttonItem2_Click(object sender, EventArgs e)
        {
            try
            {
                //////if (Machine.productionMode)             //生产模式不让收缩菜单栏
                //////    return;
                //////if (buttonItem2.Text == "^")
                //////{
                //////    buttonItem2.Text = "+";
                //////    ribbonControl1.Height = 32;
                //////    tableLayoutPanel1.Height = 32;
                //////    buttonItem2.Tooltip = "显示菜单栏";
                //////}
                //////else
                //////{
                //////    buttonItem2.Text = "^";
                //////    ribbonControl1.Height = 145;
                //////    tableLayoutPanel1.Height = 145;
                //////    buttonItem2.Tooltip = "隐藏菜单栏";
                //////}
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void buttonItem53_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Frm_DeviceManager.Instance.Show();
        }
        private void buttonItem54_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void buttonItem55_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;

                Frm_ComConfig.Instance.WindowState = FormWindowState.Normal;
                Frm_ComConfig.Instance.ShowDialog();
                Frm_ComConfig.Instance.TopMost = true;
                Frm_ComConfig.Instance.TopMost = false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void buttonItem56_Click(object sender, EventArgs e)
        {

        }
        private void buttonItem59_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\Config\\Resources\\Demo");
        }
        private void buttonItem57_Click_2(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\Config\\Resources\\Help.html");
        }
        private void buttonItem58_Click_2(object sender, EventArgs e)
        {
            Frm_Feedback.Instance.ShowDialog();
        }
        private void buttonItem62_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void buttonItem63_Click(object sender, EventArgs e)
        {
            Frm_About.Instance.ShowDialog();
        }
        private void buttonItem17_Click(object sender, EventArgs e)
        {

        }
        private void buttonItem26_Click(object sender, EventArgs e)
        {

        }
        private static string imageSavePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private void buttonItem60_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(imageSavePath))
                {
                    Directory.CreateDirectory(imageSavePath);
                }
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                int index;
                for (index = 1; index < 100; index++)
                {
                    if (!File.Exists(imageSavePath + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "_" + index + ".bmp"))
                        break;
                }
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd") + "_" + index;
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择图像保存路径";
                dig_saveImage.Filter = "图像文件(*.bmp)|*.bmp|图像文件(*.jpg)|*.jpg|Image File|*.tif|Image File(*.png)|*.txt|Image File(*.*)|*.*";
                dig_saveImage.InitialDirectory = imageSavePath;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {





                    //HOperatorSet.DumpWindowImage(out image, ff.WindowHandle);
                    //HOperatorSet.WriteImage(image, "tiff", 0, dig_saveImage.FileName);
                    //Frm_Main.Instance.OutputMsg("Image saved successfully", Color.Green);







                    string fileName = dig_saveImage.FileName;
                    imageSavePath = Path.GetDirectoryName(dig_saveImage.FileName);


                    IDockContent temp = dockPanel.ActiveContent;
                    Frm_ImageWindow ff = temp as Frm_ImageWindow;
                    if (ff != null)
                    {

                        HOperatorSet.WriteImage(ff.currentImage, "jpg", 0, dig_saveImage.FileName);
                        Frm_Main.Instance.OutputMsg("Image saved successfully", Color.Green);
                    }
                    else
                    {
                        Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //////if (Regex.IsMatch(tbx_percentageOfMovementSpeed.Text.Trim(), "^-?\\d+$"))
            //////    Project.Instance.configuration.autoRunVelRoute = Convert.ToInt16(tbx_percentageOfMovementSpeed.Text.Trim());
            //////else if (tbx_percentageOfMovementSpeed.Text.Trim() == string.Empty || tbx_percentageOfMovementSpeed.Text.Trim() == "-")
            //////{
            //////    //不做事
            //////}
            //////else
            //////{
            //////    Frm_Main.Instance.OutputMsg("曝光值不合法，请输入整型值（错误代码：0101）", Color.Red);
            //////}
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;

            //////// if (homing)
            //////// {
            //////     Frm_Main.Instance.OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////     return;
            ////// }

            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void buttonItem38_Click(object sender, EventArgs e)
        {

        }
        private void buttonItem36_Click(object sender, EventArgs e)
        {

        }
        private void buttonItem64_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = string.Empty;
                dig_openImage.Title = Project.Instance.configuration.language == Language.English ? "Please select image path" : "请选择图像文件";
                dig_openImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openImage.Filter = Project.Instance.configuration.language == Language.English ? "Image File(*.*)|*.*|Image File(*.png)|*.txt|Image File(*.jpg)|*.jpg|Image File(*.bmp)|*.bmp|Image File(*.tif)|*.tif" : "图像文件(*.jpg)|*.jpg|图像文件(*.tif)|*.tif|图像文件(*.png)|*.txt|图像文件(*.bmp)|*.bmp|图像文件(*.*)|*.*";
                if (dig_openImage.ShowDialog() == DialogResult.OK)
                {
                    HObject image;
                    try
                    {
                        HOperatorSet.ReadImage(out image, dig_openImage.FileName);
                    }
                    catch
                    {
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Unable to read specified file" : "图像文件异常，无法读取", Color.Red);
                        return;
                    }


                    IDockContent temp = dockPanel.ActiveContent;
                    Frm_ImageWindow ff = temp as Frm_ImageWindow;
                    if (ff != null)
                    {


                        ff.hwc_imageWindow.HobjectToHimage(image);
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Loading Image successfully" : "读取图像成功", Color.Green);
                    }
                    else
                    {
                        Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void buttonItem37_Click_1(object sender, EventArgs e)
        {

        }
        private void buttonItem69_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Project.ExportProject();
        }
        private void buttonItem71_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Project.InportProject();
        }

        private void buttonItem73_Click(object sender, EventArgs e)
        {
            if (Machine.productionMode)
            {
                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Switch to debug page" : "切换到调试页面");
                Frm_Login.Instance.ShowDialog();
            }
            else
            {
                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Switch to production page" : "切换到生产页面");
                //////Frm_Main.Instance.buttonItem776.Image = Properties.Resources.Debug;
                //////Frm_Main.Instance.buttonItem776.Text = "调试";
                //////Machine.SwitchToProductForm();
            }
        }

        private void buttonItem76_Click(object sender, EventArgs e)
        {
            if (Machine.productionMode)
            {
                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Switch to debug page" : "切换到调试页面");
                Frm_Login.Instance.ShowDialog();
            }
        }

        private void buttonItem46_Click_4(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;

                if (Frm_Job.Instance.tbc_jobs.TabPages.Count == 0)
                {
                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "No jobs to run" : "没有可运行的流程", Color.Green);
                    return;
                }
                //////Frm_Main.Instance.btn_runOnce.Enabled = false;
                //////Frm_Job.Instance.btn_runLoop.Enabled = false;
                //////Frm_Job.Instance.btn_runOnce.Enabled = false;
                //////Application.DoEvents();
                //////Thread.Sleep(50);
                //////if (Frm_Job.Instance.btn_runLoop.Text == (Project.Instance.configuration.language == Language.English ? "Run Loop" : "连续运行"))
                //////{
                //////    Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).isRunLoop = true;
                //////    Frm_Job.Instance.th_runJob = new Thread(Frm_Job.Instance.RealTimeRun);
                //////    Frm_Job.Instance.th_runJob.IsBackground = true;
                //////    Frm_Job.Instance.th_runJob.Start(Frm_Job.Instance.tbc_jobs.SelectedTab.Text);
                //////    Frm_Main.Instance.btn_runLoop.Text = "停止运行";
                //////    Frm_Job.Instance.btn_runLoop.Text = Project.Instance.configuration.language == Language.English ? "Run Loop" : "停止运行";
                //////    Frm_Main.Instance.btn_runOnce.Enabled = false;
                //////}
                //////else
                //////{
                //////    Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).isRunLoop = false;
                //////    Thread.Sleep(20);
                //////    Frm_Main.Instance.btn_runLoop.Text = "连续运行";
                //////    Frm_Job.Instance.btn_runLoop.Text = Project.Instance.configuration.language == Language.English ? "Run Loop" : "连续运行";
                //////    Frm_Main.Instance.btn_runOnce.Enabled = true;
                //////    Frm_Job.Instance.btn_runOnce.Enabled = true;
                //////}
                Frm_Job.Instance.btn_runLoop.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void buttonItem46_Click_5(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;

            Frm_TCPServer.Instance.WindowState = FormWindowState.Normal;
            Frm_TCPServer.Instance.Show();
            Frm_TCPServer.Instance.TopMost = true;
        }
        private void buttonItem50_Click_1(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;

            Frm_TCPClient.Instance.WindowState = FormWindowState.Normal;
            Frm_TCPClient.Instance.Show();
            Frm_TCPClient.Instance.TopMost = true;
        }
        private void buttonItem77_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Frm_DeviceManager.Instance.Show();
        }
        private void buttonItem13_Click_1(object sender, EventArgs e)
        {
            SaveAll();
            this.Close();
        }
        private void buttonItem5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void buttonItem3_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Job.CreateJob();
        }
        private void buttonItem4_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = "";
                dig_openImage.Title = (Project.Instance.configuration.language == Language.English ? "Please select a job file" : "请选择流程文件");
                dig_openImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openImage.Filter = (Project.Instance.configuration.language == Language.English ? "job file(*.job)|*.job" : "流程文件(*.job)|*.job");
                if (dig_openImage.ShowDialog() == DialogResult.OK)
                {
                    Project.Instance.configuration.L_recentlyOpendFile.Insert(0, dig_openImage.FileName);
                    if (Project.Instance.configuration.L_recentlyOpendFile.Count >= 5)
                        Project.Instance.configuration.L_recentlyOpendFile.RemoveRange(5, Project.Instance.configuration.L_recentlyOpendFile.Count - 5);
                    Job.LoadJob(dig_openImage.FileName);
                }
                Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void buttonItem6_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void buttonItem7_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void buttonItem8_Click(object sender, EventArgs e)
        {
            Project.SaveProject();
            // SaveAll();
            Project.Instance.configuration.Save();
        }

        private void buttonItem28_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                if (Frm_Job.Instance.tbc_jobs.TabPages.Count == 0)
                {
                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "No jobs to run" : "没有可运行的流程", Color.Green);
                    return;
                }
                Frm_Job.Instance.btn_runOnce.Enabled = false;
                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                Job job = Job.FindJobByName(jobName);
                Job.RunAndWait(job.jobName);
                Frm_Job.Instance.btn_runOnce.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void buttonItem19_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
            return;
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Machine.SwitchToProductForm();
        }
        private void buttonItem20_Click(object sender, EventArgs e)
        {
            if (Frm_Job.Instance.DockState == DockState.Hidden || Frm_Job.Instance.DockState == DockState.Unknown)
                Frm_Job.Instance.Show(Frm_Main.Instance.dockPanel, DockState.DockRight);
        }
        private void buttonItem21_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                CreateNewImageWindow();

                //需要重新保存一下布局
                SaveDockLayout(true);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void buttonItem23_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Frm_Setting.Instance.ShowDialog();
        }
        private void buttonItem24_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;

            Frm_MotionControl.Instance.Show();
        }
        private void buttonItem25_Click(object sender, EventArgs e)
        {
            ShowToolboxInVisionSidebar();
        }
        private void buttonItem27_Click(object sender, EventArgs e)
        {
            ShowOutputInVisionBottomPanel();
        }
        private void buttonItem31_Click(object sender, EventArgs e)
        {
        }
        private void buttonItem32_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
            return;
            if (Frm_Monitor.Instance.DockState == DockState.Hidden || Frm_Monitor.Instance.DockState == DockState.Unknown)

                Frm_Monitor.Instance.Show(dockPanel, DockState.DockBottomAutoHide);

            else
                Frm_Monitor.Instance.Activate();
        }
        private void buttonItem33_Click(object sender, EventArgs e)
        {

            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void buttonItem35_Click(object sender, EventArgs e)
        {

        }
        private void buttonItem34_Click(object sender, EventArgs e)
        {

        }
        private void buttonItem39_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Frm_LayoutManage.Instance.Show();
        }
        private void buttonItem40_Click(object sender, EventArgs e)
        {
            //////if (!Permission.CheckPermission(PermissionLevel.Admin))
            //////    return;

            //////dockPanel.AllowEndUserDocking = !dockPanel.AllowEndUserDocking;
            //////if (dockPanel.AllowEndUserDocking)
            //////{
            //////    buttonItem40.Text = "已解锁";
            //////    buttonItem40.Image = Properties.Resources.UnLock;
            //////}
            //////else
            //////{
            //////    buttonItem40.Text = "已锁定";
            //////    buttonItem40.Image = Properties.Resources.Lock;

            //////}
            Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Interface lock enabled" : "界面锁定启用");
        }
        private void buttonItem41_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.ContextMenuStrip = null;
                HOperatorSet.SetColor(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, new HTuple("green"));
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.Focus();
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.Select();

                HTuple row, column;
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.DrawModel = true;
                HOperatorSet.DrawPoint(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, out row, out column);
                HOperatorSet.SetLineWidth(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, new HTuple(2));
                HOperatorSet.DispCross(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, row, column, 40, 0);
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;

                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.ContextMenuStrip = null;
                HOperatorSet.SetColor(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, new HTuple("green"));
                HTuple row1, column1;
                HOperatorSet.DrawPoint(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, out row1, out column1);
                HOperatorSet.SetLineWidth(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, new HTuple(2));
                HOperatorSet.DispCross(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, row1, column1, 40, 0);
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.DrawModel = false;

                HOperatorSet.DispArrow(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, row, column, row1, column1, new HTuple(10));
                HTuple distance;
                HOperatorSet.DistancePp(row, column, row1, column1, out distance);
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "\r\nPixel distance" : "\r\n像素距离：" + ((double)distance).ToString("0.000") + "个像素");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void buttonItem42_Click(object sender, EventArgs e)
        {
            processKeyBoard = System.Diagnostics.Process.Start("osk.exe");
        }
        private void buttonItem43_Click(object sender, EventArgs e)
        {
            try
            {
                Rectangle bounds = Screen.FromControl(this).Bounds;
                using (Bitmap image = new Bitmap(bounds.Width, bounds.Height))
                using (Graphics graphics = Graphics.FromImage(image))
                using (System.Windows.Forms.SaveFileDialog saveImageDialog = new System.Windows.Forms.SaveFileDialog())
                {
                    graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    saveImageDialog.Title = Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择图像保存路径";
                    saveImageDialog.Filter = "图像文件(*.jpg)|*.jpg|Image File|*.tif|Image File(*.png)|*.png|Image File(*.bmp)|*.bmp";
                    saveImageDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    saveImageDialog.FileName = DateTime.Now.ToString("yyyy_MM_dd");
                    if (saveImageDialog.ShowDialog() == DialogResult.OK)
                        image.Save(saveImageDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void buttonItem44_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Frm_inputSingalVitual.Instance.Show();
        }
        private void buttonItem29_Click_1(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Open the Settings page" : "打开设置页面");
            Frm_Setting.Instance.ShowDialog();
        }
        private void buttonItem47_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "Are you sure you want to reset all the setting and config?" : "      确定要将程序恢复到初始状态吗？这将丢失所有配置信息及\r\n相关设置，使程序恢复到安装完毕时的初始状态！");
                Frm_ConfirmBox.Instance.ShowDialog();
                if (Frm_ConfirmBox.Instance.Result != ConfirmBoxResult.Yes)
                {
                    return;
                }

                for (int i = Frm_ImageWindow.D_imageWindow.Count - 1; i >= 0; i--)
                {
                    Frm_ImageWindow.D_imageWindow.Values.ToArray()[i].Close();
                }

                Project.Instance.L_engineList.Clear();
                Project.Instance.configuration = new Configuration();

                //删除所有配置文件
                if (Directory.Exists(Application.StartupPath + "\\Config\\Project"))
                    Directory.Delete(Application.StartupPath + "\\Config\\Project", true);
                if (File.Exists(Application.StartupPath + "\\Config\\Configuration.ini"))
                    File.Delete(Application.StartupPath + "\\Config\\Configuration.ini");
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "\r\nReset succeeded! (Effective after restart, the program will shut down automatically)" : "\r\n重置成功!（重启后生效，程序将自动关闭）");
                this.Close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void buttonItem48_Click(object sender, EventArgs e)
        {
            Process.Start(Project.Instance.configuration.dataPath + "\\Log");
        }
        private void buttonItem49_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow("\r\n已停用");
            return;

            processGreedSnake = Process.Start(Application.StartupPath + "\\我的贪吃蛇.exe");
        }

        #endregion

        #region 通讯协议

        /// <summary>
        /// 协议
        /// </summary>
        /// <param name="command"></param>
        internal static void Protocol(object help1)//(string command, string sender)
        {
            try
            {
                //////CommItem commItem = (CommItem)help1;
                ////////通讯本地记录

                //////Log.SaveLog(LogType.Comm, "接收到：" + commItem.cmd);

                //////bool b = false;     //用于指示协议中是否包含此指令
                //////for (int i = 0; i < Project.Instance.configuration.L_communicationItemList.Count; i++)
                //////{
                //////    string receiveStr = Project.Instance.configuration.L_communicationItemList[i].ReceivedCommand;
                //////    if (commItem.commType == 1)
                //////        commItem.cmd = commItem.cmd.Substring(0, commItem.cmd.Length - 2);
                //////    string jobName = Project.Instance.configuration.L_communicationItemList[i].JobName;
                //////    string temp = commItem.cmd;
                //////    //////temp = help.str1.Substring(0, help.str1.Length - 2);
                //////    if (temp == receiveStr)
                //////    {
                //////        b = true;
                //////        //执行相应的流程
                //////        string outputItem = Project.Instance.configuration.L_communicationItemList[i].OutputItem;
                //////        Job job = Job.FindJobByName(jobName);
                //////        if (job == null)
                //////        {
                //////            Frm_MessageBox.Instance.MessageBoxShow("未找到名为\"" + jobName + "\"的流程，请检查");
                //////            return;
                //////        }

                //////        //寻找OutputBoxTool
                //////        ToolInfo outputBox = new ToolInfo();
                //////        for (int j = 0; j < job.L_toolList.Count; j++)
                //////        {
                //////            if (job.L_toolList[j].toolType == ToolType.Output)
                //////                outputBox = (ToolInfo)job.L_toolList[j];
                //////        }

                //////        job.Run();
                //////        //////string result = outputBox.GetInput("<--" + outputItem).value.ToString();
                //////        string result = string.Empty;
                //////        if (job.jobRunStatu == JobRunStatu.Succeed)
                //////        {

                //////            //if (temp == "One")
                //////            //    result = ((EyeHandCalibTool)Job.FindJobByName("定位计算").FindToolByName("手眼标定")).outputXYU.ToFormatStr();
                //////            //else
                //////            result = outputBox.GetInput("<--" + outputItem).value.ToString();
                //////        }
                //////        else
                //////        {
                //////            result = Project.Instance.configuration.L_communicationItemList[i].NGRespond;
                //////        }




                //////        string addStr = Project.Instance.configuration.L_communicationItemList[i].PrefixStr;
                //////        string suffixStr = Project.Instance.configuration.L_communicationItemList[i].suffixStr;
                //////        result = addStr + result;
                //////        result += suffixStr;
                //////        if (Project.Instance.configuration.communicationType == CommunicationType.Internet_Client)
                //////        {
                //////            //////Frm_TCPClient.Instance.Send(result + "\r\n");
                //////        }
                //////        else if (Project.Instance.configuration.communicationType == CommunicationType.Internet_Sever)
                //////        {
                //////            //////Frm_TCPServer.Instance.Send(result + "\r\n", commItem.RemoteEndPoint);
                //////        }
                //////        else if (Project.Instance.configuration.communicationType == CommunicationType.SerialPort)
                //////        {
                //////            Frm_SerialPort.Instance.serialPort.ReadExisting();
                //////            Frm_SerialPort.Instance.serialPort.Write(result);
                //////        }
                //////        Frm_UserForm.Instance.OutputMsg("已发送：" + result);

                //////        Log.SaveLog(LogType.Comm, "已发送：" + result);

                //////        Application.DoEvents();
                //////    }
                //////}
                //////if (!b)
                //////{
                //////    Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Receiving the non-negotiated agreement content sent by the remote terminal：" : "接收到远程端发来的未商议协议内容：" + commItem.cmd);
                //////}
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        #endregion

        #region  窗体缩放
        private const int WM_NCHITTEST = 0x0084; //鼠标在窗体客户区（除标题栏和边框以外的部分）时发送的信息
        const int HTLEFT = 10;  //左变
        const int HTRIGHT = 11;  //右边
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;  //左上
        const int HTTOPRIGHT = 14; //右上
        const int HTBOTTOM = 15;  //下
        const int HTBOTTOMLEFT = 0x10;  //左下
        const int HTBOTTOMRIGHT = 17;  //右下
        System.Drawing.Point vPoint = System.Drawing.Point.Empty;
        //自定义边框拉伸
        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
                switch (m.Msg)
                {
                    case WM_NCHITTEST:
                        vPoint = new System.Drawing.Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                        vPoint = PointToClient(vPoint);
                        if (vPoint.X <= 5)
                            if (vPoint.Y <= 5)
                                m.Result = (IntPtr)HTTOPLEFT;  //左上
                            else if (vPoint.Y >= this.ClientSize.Height - 5)
                                m.Result = (IntPtr)HTBOTTOMLEFT; //左下
                            else
                                m.Result = (IntPtr)HTLEFT;  //左边
                        else if (vPoint.X >= this.ClientSize.Width - 5)
                            if (vPoint.Y <= 5)
                                m.Result = (IntPtr)HTTOPRIGHT;  //右上
                            else if (vPoint.Y >= this.ClientSize.Height - 5)
                                m.Result = (IntPtr)HTBOTTOMRIGHT;  //右下
                            else
                                m.Result = (IntPtr)HTRIGHT;  //右
                        else if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOP;  //上
                        else if (vPoint.Y >= this.ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOM; //下

                        else
                        {
                            base.WndProc(ref m);//如果去掉这一行代码,窗体将失去MouseMove..等事件
                            System.Drawing.Point lpint = new System.Drawing.Point((int)m.LParam);//可以得到鼠标坐标,这样就可以决定怎么处理这个消息了,是移动窗体,还是缩放,以及向哪向的缩放

                            m.Result = (IntPtr)0x2;//托动HTCAPTION=2 <0x2>
                        }
                        break;
                }
            }
            catch { }
        }
        #endregion

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
        private void setForm_MouseLeave(object sender, EventArgs e)
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

        private void buttonItem73_Click_1(object sender, EventArgs e)
        {
            //////if (!Permission.CheckPermission(PermissionLevel.Operator))
            //////    return;

            if (Machine.machineRunStatu == MachineRunStatu.Homing)
            {
                Frm_Main.Instance.OutputMsg("设备复位中，请复位完成后开始", Color.Red);
                return;
            }
            else if (Machine.machineRunStatu == MachineRunStatu.WaitReset)
            {
                Frm_Main.Instance.OutputMsg("设备未复位，请复位成后开始", Color.Red);
                return;
            }

            //////if (!Frm_LoginMes.Instance.confirmSucceed)
            //////{
            //////    Frm_LoginMes.Instance.ShowDialog();
            //////}

            Machine.StartRun();

        }

        private void buttonItem78_Click(object sender, EventArgs e)
        {
            //////if (!Permission.CheckPermission(PermissionLevel.Operator))
            //////    return;
            Machine.StopRun();
        }

        private void buttonItem79_Click(object sender, EventArgs e)
        {
            //////if (!Permission.CheckPermission(PermissionLevel.Operator))
            //////    return;
            Machine.Home();
        }

        private void buttonItem80_Click(object sender, EventArgs e)
        {
            Frm_Login.Instance.ShowDialog();
        }

        private void buttonItem82_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonItem776_Click_1(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
            return;
            if (!Permission.CheckPermission(PermissionLevel.Developer))
                return;
            if (Machine.productionMode)
            {
                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Switch to debug page" : "切换到调试页面");
                Frm_Login.Instance.ShowDialog();
            }
            else
            {
                //////Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Switch to production page" : "切换到生产页面");
                //////Frm_Main.Instance.buttonItem776.Image = Properties.Resources.Debug;
                //////Frm_Main.Instance.buttonItem776.Text = "调试页面";
                //////Machine.SwitchToProductForm();

                ////////生产模式不让收缩菜单栏
                //////buttonItem2.Text = "^";
                //////ribbonControl1.Height = 145;
                //////tableLayoutPanel1.Height = 145;
            }
        }


        private void buttonItem61_Click_1(object sender, EventArgs e)
        {
            try
            {


                if (!Directory.Exists(imageSavePath))
                {
                    Directory.CreateDirectory(imageSavePath);
                }
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                int index;
                for (index = 1; index < 100; index++)
                {
                    if (!File.Exists(imageSavePath + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "_" + index + ".bmp"))
                        break;
                }
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd") + "_" + index;
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择图像保存路径";
                dig_saveImage.Filter = "Image File(*.bmp)|*.bmp|图像文件(*.jpg)|*.jpg|Image File|*.tif|Image File(*.png)|*.txt|Image File(*.*)|*.*";
                dig_saveImage.InitialDirectory = imageSavePath;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dig_saveImage.FileName;
                    HObject image;

                    IDockContent temp = dockPanel.ActiveDocument;
                    Frm_ImageWindow ff = temp as Frm_ImageWindow;
                    if (ff != null)
                    {

                        HOperatorSet.DumpWindowImage(out image, ff.WindowHandle);
                        HOperatorSet.WriteImage(image, "bmp", 0, dig_saveImage.FileName);
                        Frm_Main.Instance.OutputMsg("Image saved successfully", Color.Black);
                    }
                    else
                    {
                        Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void tim_recordTime_Tick(object sender, EventArgs e)
        {
            ProcessUiRefreshTick();
        }

        private void buttonItem75_Click_1(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Frm_EngineManager.Instance.Show();
            Frm_EngineManager.Instance.WindowState = FormWindowState.Normal;

        }

        private void buttonItem81_Click(object sender, EventArgs e)
        {
            Frm_ProductData.Instance.Show(dockPanel, DockState.DockRight);
        }

        private void buttonItem83_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!Permission.CheckPermission(PermissionLevel.Admin))
                //    return;
                //////Frm_Lock frm_lock = new Frm_Lock();
                //////buttonItem83.Checked = true;
                //////frm_lock.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void buttonItem45_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow("\r\n已停用");
            return;
        }

        private void buttonItem84_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow("\r\n已停用");
            return;
        }

        private void buttonItem93_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.ExportScheme();
        }

        private void buttonItem92_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                if (Frm_Job.Instance.tbc_jobs.TabPages.Count < 1)
                    return;
                Frm_ConfirmBox.Instance.lbl_info.Text = Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : "确定要删除当前项目吗？";
                Frm_ConfirmBox.Instance.ShowDialog();
                if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Cancel)
                {
                    return;
                }
                foreach (TabPage tabPage in Frm_Job.Instance.tbc_jobs.TabPages)
                {
                    string jobName = tabPage.Text;
                    Job.RemoveJobByName(jobName);
                    for (int j = 0; j < Frm_Job.Instance.tbc_jobs.TabPages.Count; j++)
                    {
                        if (Frm_Job.Instance.tbc_jobs.TabPages[j].Text == jobName)
                        {
                            Frm_Job.Instance.tbc_jobs.TabPages.RemoveAt(j);
                        }
                    }
                    if (File.Exists(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + jobName + ".job"))
                        File.Delete(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + jobName + ".job");
                }
                Frm_Main.Instance.OutputMsg("项目删除成功", Color.Black);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void buttonItem35_Click_1(object sender, EventArgs e)
        {
            Project.SaveProject();
            // SaveAll();
            Project.Instance.configuration.Save();
        }


        private void buttonItem94_Click_1(object sender, EventArgs e)
        {
            try
            {
            Again:
                Frm_InputMessage.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "Please input job's name" : "请输入新方案名");
                Frm_InputMessage.Instance.btn_confirm.Text = (Project.Instance.configuration.language == Language.English ? "Confirm" : "确定");
                Frm_InputMessage.Instance.passwordChar = false;
                Frm_InputMessage.Instance.txt_input.TextStr = string.Empty;
                Frm_InputMessage.Instance.ShowDialog();
                string jobName = Frm_InputMessage.input;
                if (jobName == "")
                    return;

                //检查此名称的流程是否已存在
                //////if (Job.Job_Exist(jobName))
                //////{
                //////    Frm_MessageBox.Instance.MessageBoxShow((Project .Instance .configuration .language == Language.English ? "\r\nA process with this name already exists. The process name cannot be repeated. Please enter again" : "\r\n已存在此名称的流程，流程名不可重复，请重新输入"));
                //////    goto Again;
                //////}
                ////////检查此名称是否含有特殊字符\
                //////if (jobName.Contains(@"\"))
                //////{
                //////    Frm_MessageBox.Instance.MessageBoxShow((Project .Instance .configuration .language == Language.English ? "\r\nA process with this name already exists. The process name cannot be repeated. Please enter again" : "\r\n流程名中不能含有 \\ 等特殊字符 ，请重新输入"));
                //////    goto Again;
                //////}
                Log.SaveLog(LogType.Operate, (Project.Instance.configuration.language == Language.English ? "A new process named:" + jobName : "创建了新流程，流程名为：" + jobName));

                Scheme engine = new Scheme();
                engine.schemeName = jobName;
                Project.Instance.L_engineList.Add(engine);

                //////comboBox1.Items.Add(jobName);
                //////if (comboBox1.Items.Count > 0 && comboBox1.Text == string.Empty)
                //////    comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void buttonItem70_Click_1(object sender, EventArgs e)
        {
            SaveAll();
        }


        private void buttonItem67_Click_1(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.InportScheme();
        }

        private void buttonItem65_Click_1(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.ExportScheme();
        }

        private void buttonItem38_Click_1(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.CloneScheme();
        }

        private void buttonItem37_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                if (Project.Instance.L_engineList.Count > 0)
                {
                    Frm_ConfirmBox.Instance.lbl_info.Text = Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : "确定要删除当前方案吗？";
                    Frm_ConfirmBox.Instance.ShowDialog();
                    if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Cancel)
                    {
                        return;
                    }
                    Project.Instance.L_engineList.Remove(Project.Instance.curEngine);
                    if (Project.Instance.L_engineList.Count > 0)
                        Project.Instance.curEngine = Project.Instance.L_engineList[0];
                    else
                        Project.Instance.curEngine = null;


                    //////foreach (TabPage tabPage in Frm_Job.Instance.tbc_jobs.TabPages)
                    //////{
                    //////    string jobName = tabPage.Text;
                    //////    Job.RemoveJobByName(jobName);
                    //////    for (int j = 0; j < Frm_Job.Instance.tbc_jobs.TabPages.Count; j++)
                    //////    {
                    //////        if (Frm_Job.Instance.tbc_jobs.TabPages[j].Text == jobName)
                    //////        {
                    //////            Frm_Job.Instance.tbc_jobs.TabPages.RemoveAt(j);
                    //////        }
                    //////    }
                    //////    if (File.Exists(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + jobName + ".job"))
                    //////        File.Delete(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + jobName + ".job");
                    //////}
                    Frm_Main.Instance.OutputMsg("项目删除成功", Color.Black);
                }
                else
                {
                    Frm_Output.Instance.OutputMsg("当前项目中未添加任何方案", Color.Red);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void buttonItem87_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                if (Frm_Job.Instance.tbc_jobs.TabPages.Count < 1)
                {
                    Frm_Output.Instance.OutputMsg("当前项目中未添加任何方案", Color.Red);
                    return;
                }
                Frm_ConfirmBox.Instance.lbl_info.Text = Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : "确定要删除当前流程吗？";
                Frm_ConfirmBox.Instance.ShowDialog();
                if (Frm_ConfirmBox.Instance.Result != ConfirmBoxResult.Yes)
                {
                    return;
                }
                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                Job.RemoveJobByName(jobName);
                for (int i = 0; i < Frm_Job.Instance.tbc_jobs.TabPages.Count; i++)
                {
                    if (Frm_Job.Instance.tbc_jobs.TabPages[i].Text == jobName)
                    {
                        //Frm_Job.Instance.tbc_jobs.TabPages.RemoveByKey(jobName );
                        Frm_Job.Instance.tbc_jobs.TabPages.RemoveAt(i);
                    }
                }
                if (File.Exists(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + jobName + ".job"))
                    File.Delete(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + jobName + ".job");
                Frm_Main.Instance.OutputMsg("流程删除成功", Color.Black);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void buttonItem86_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Job.CloneJob();
        }

        private void buttonItem85_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                if (Project.Instance.L_engineList.Count == 0)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n未创建方案，请先创建方案");
                    Frm_EngineManager.Instance.Show();
                    return;
                }

                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = "";
                dig_openImage.Title = (Project.Instance.configuration.language == Language.English ? "Please select a job file" : "请选择流程文件");
                dig_openImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openImage.Filter = (Project.Instance.configuration.language == Language.English ? "job file(*.job)|*.job" : "流程文件(*.job)|*.job");
                dig_openImage.ShowDialog();
                if (dig_openImage.FileName == string.Empty)
                {
                    return;
                }
                try
                {
                    Job job = VM.LoadJob(dig_openImage.FileName);
                    File.Copy(dig_openImage.FileName, Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + job.jobName + ".job");
                }
                catch { }
                Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "A new process was imported" : "导入了新流程");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void buttonItem84_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                Job.ExportJob();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void buttonItem49_Click_1(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Job.CreateJob();
        }

        private void buttonItem45_Click_1(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void buttonItem7_Click_1(object sender, EventArgs e)
        {
            Save();
        }

        private void Frm_Main_SizeChanged(object sender, EventArgs e)
        {
            UpdateMotionRefreshActivity();
        }

        private void buttonItem6_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = "";
                dig_openImage.Title = (Project.Instance.configuration.language == Language.English ? "Please select a job file" : "请选择文件");
                dig_openImage.InitialDirectory = Application.StartupPath + "\\Config\\Resources\\Sample";
                dig_openImage.Filter = (Project.Instance.configuration.language == Language.English ? "job file(*.job)|*.job" : "项目文件(*.pjt)|*.pjt|方案文件(*.eng)|*.eng|流程文件(*.job)|*.job");
                if (dig_openImage.ShowDialog() == DialogResult.OK)
                {
                    if (Project.Instance.configuration.L_recentlyOpendFile.Contains(dig_openImage.FileName))
                        Project.Instance.configuration.L_recentlyOpendFile.Remove(dig_openImage.FileName);
                    Project.Instance.configuration.L_recentlyOpendFile.Insert(0, dig_openImage.FileName);
                    if (Project.Instance.configuration.L_recentlyOpendFile.Count >= 5)
                        Project.Instance.configuration.L_recentlyOpendFile.RemoveRange(5, Project.Instance.configuration.L_recentlyOpendFile.Count - 5);

                    string temp = Path.GetExtension(dig_openImage.FileName);
                    if (Path.GetExtension(dig_openImage.FileName) == ".pjt")
                    {
                        Project project = Project.LoadProject(dig_openImage.FileName);
                        //Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                        //Application.DoEvents();
                        // Frm_ImageWindow.Instance.Update_Last_Run_Result_Image_List();

                        //  job .Draw_Line(null ,null );   //此处不能画线，否则就会出现添加第二个示例流程时报错的问题
                        Scheme engine = Project.Instance.L_engineList[0];
                        for (int i = 0; i < engine.L_jobList.Count; i++)
                        {
                            Job job = engine.L_jobList[i];
                            switch (job.jobName)
                            {
                                case "尺寸测量":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "斑点分析":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "机械手下视觉定位":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "机械手下视觉抓取定点放置":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "模板匹配":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "条码读取":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "位置跟随":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "焊点检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "OCR":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "记号检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "圆度检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                            }
                        }
                    }
                    else if (Path.GetExtension(dig_openImage.FileName) == ".eng")
                    {
                        Scheme engine = Scheme.LoadScheme(dig_openImage.FileName);
                        Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                        Application.DoEvents();
                        // Frm_ImageWindow.Instance.Update_Last_Run_Result_Image_List();

                        //  job .Draw_Line(null ,null );   //此处不能画线，否则就会出现添加第二个示例流程时报错的问题
                        for (int i = 0; i < engine.L_jobList.Count; i++)
                        {
                            Job job = engine.L_jobList[i];
                            switch (job.jobName)
                            {
                                case "尺寸测量":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "斑点分析":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "机械手下视觉定位":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "机械手下视觉抓取定点放置":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "模板匹配":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "条码读取":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "位置跟随":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "焊点检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "OCR":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "记号检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "圆度检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                            }
                        }

                    }
                    else if (Path.GetExtension(dig_openImage.FileName) == ".job")
                    {
                        Job job = Job.LoadJob(dig_openImage.FileName);
                        Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                        Application.DoEvents();

                        //  job .Draw_Line(null ,null );   //此处不能画线，否则就会出现添加第二个示例流程时报错的问题
                        switch (job.jobName)
                        {
                            case "尺寸测量":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "斑点分析":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "机械手下视觉定位":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "机械手下视觉抓取定点放置":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "模板匹配":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "条码读取":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "位置跟随":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "焊点检测":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "OCR":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "记号检测":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "圆度检测":
                                ClearSampleAcqImageDirectory(job);
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.configuration.allowResizeForm)
            {
                button2.Enabled = false;
                return;
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                UpdateMaximizedBoundsForCurrentScreen();
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            UpdateMainMaximizeButtonGlyph();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                UpdateMaximizedBoundsForCurrentScreen();
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            UpdateMainMaximizeButtonGlyph();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            UpdateMainMaximizeButtonGlyph();
        }

        internal void toolStripButton9_Click(object sender, EventArgs e)
        {
            Machine.SwitchFrom(FormMode.MainForm);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Machine.SwitchFrom(FormMode.VisionForm);
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Machine.SwitchFrom(FormMode.MotionForm);

        }

        private void toolStripButton5_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton5.Image = Resources.图像00;
        }

        private void toolStripButton5_MouseLeave(object sender, EventArgs e)
        {
            if (Machine.curFormMode != FormMode.VisionForm)
                toolStripButton5.Image = Resources.图像__7_;
        }

        private void toolStripButton9_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton9.Image = Resources.主页__3_;
        }

        private void toolStripButton9_MouseLeave(object sender, EventArgs e)
        {
            if (Machine.curFormMode != FormMode.MainForm)
                toolStripButton9.Image = Resources.主页;
        }





        private void toolStripButton3_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton3.Image = Resources.停止1;
        }

        private void toolStripButton3_MouseLeave(object sender, EventArgs e)
        {
            if (Machine.machineRunStatu != MachineRunStatu.Stop)
                toolStripButton3.Image = Resources.停止__1_;
        }
        private void ButtonEnable(bool enable)
        {
            打开流程ToolStripMenuItem.Enabled = enable;
            关闭ToolStripMenuItem.Enabled = enable;
            打开ToolStripMenuItem.Enabled = enable;
            系统重置ToolStripMenuItem.Enabled = enable;
            if (Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).jobRunMode == JobRunMode.LoopRunAfterStart)
            {
                toolStripButton11.Enabled = enable;
                toolStripButton12.Enabled = enable;
                toolStripButton35.Enabled = enable;
                toolStripButton16.Enabled = enable;
            }
            最近的项目ToolStripMenuItem.Enabled = enable;
            删除ToolStripMenuItem.Enabled = enable;
            toolStripButton17.Enabled = enable;
            toolStripButton18.Enabled = enable;
            toolStripButton22.Enabled = enable;
            toolStripButton14.Enabled = enable;
            toolStripButton15.Enabled = enable;
            toolStripButton19.Enabled = enable;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //////if (!Permission.CheckPermission(PermissionLevel.Operator))
            //////    return;
            Machine.StopRun();

            Machine.machineRunStatu = MachineRunStatu.Stop;
            toolStripButton3.Image = Resources.停止1;
            toolStripButton4.Image = Resources.启动;
            toolStripButton8.Image = Resources.复位;
            toolStripButton36.Image = Resources.停止__11111_;
            toolStripButton3.Font = new System.Drawing.Font(toolStripButton3.Font.FontFamily, toolStripButton3.Font.Size, FontStyle.Bold);
            toolStripButton4.Font = new System.Drawing.Font(toolStripButton4.Font.FontFamily, toolStripButton4.Font.Size, FontStyle.Regular);
            toolStripButton8.Font = new System.Drawing.Font(toolStripButton8.Font.FontFamily, toolStripButton8.Font.Size, FontStyle.Regular);
            toolStripButton36.Font = new System.Drawing.Font(toolStripButton36.Font.FontFamily, toolStripButton36.Font.Size, FontStyle.Regular);

            toolStripButton3.Text = "已停止";
            toolStripButton4.Text = "启动";
            toolStripButton36.Text = "暂停";
            toolStripButton8.Enabled = true;
            新建流程ToolStripMenuItem.Enabled = true;
            toolStripButton36.Enabled = false;
            ButtonEnable(true);

            Thread th = new Thread(() =>
            {
                Thread.Sleep(1000);      //保证所有的流程最后一次运行都执行完毕
                Configuration.SpeedMode = false;
            });
            th.IsBackground = true;
            th.Start();
        }

        private void toolStripButton8_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton8.Image = Resources.复位__1_;
        }

        private void toolStripButton8_MouseLeave(object sender, EventArgs e)
        {
            if (Machine.machineRunStatu != MachineRunStatu.WaitRun)
                toolStripButton8.Image = Resources.复位;
        }

        private void toolStripButton4_MouseEnter_1(object sender, EventArgs e)
        {
            toolStripButton4.Image = Resources.启动__1_;
        }

        private void toolStripButton4_MouseLeave(object sender, EventArgs e)
        {
            if (Machine.machineRunStatu != MachineRunStatu.Running)
                toolStripButton4.Image = Resources.启动;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

            //////if (!Permission.CheckPermission(PermissionLevel.Operator))
            //////    return;
            toolStripButton36.Enabled = true;
            Machine.machineRunStatu = MachineRunStatu.Running;
            toolStripButton4.Image = Resources.启动__1_;
            toolStripButton8.Image = Resources.复位;
            toolStripButton3.Image = Resources.停止__1_;
            toolStripButton36.Image = Resources.停止__11111_;
            toolStripButton4.Font = new System.Drawing.Font(toolStripButton4.Font.FontFamily, toolStripButton4.Font.Size, FontStyle.Bold);
            toolStripButton8.Font = new System.Drawing.Font(toolStripButton8.Font.FontFamily, toolStripButton8.Font.Size, FontStyle.Regular);
            toolStripButton3.Font = new System.Drawing.Font(toolStripButton3.Font.FontFamily, toolStripButton3.Font.Size, FontStyle.Regular);
            toolStripButton36.Font = new System.Drawing.Font(toolStripButton36.Font.FontFamily, toolStripButton36.Font.Size, FontStyle.Regular);


            if (Machine.machineRunStatu == MachineRunStatu.Homing)
            {
                Frm_Main.Instance.OutputMsg("设备复位中，请复位完成后开始", Color.Red);
                return;
            }
            else if (Machine.machineRunStatu == MachineRunStatu.WaitReset)
            {
                Frm_Main.Instance.OutputMsg("设备未复位，请复位成后开始", Color.Red);
                return;
            }

            //////if (!Frm_LoginMes.Instance.confirmSucceed)
            //////{
            //////    Frm_LoginMes.Instance.ShowDialog();
            //////}

            Machine.StartRun();

            toolStripButton4.Text = "已启动";
            toolStripButton3.Text = "停止";
            toolStripButton8.Text = "复位";
            toolStripButton36.Text = "暂停";
            toolStripButton8.Enabled = false;
            toolStripButton3.Enabled = true;
            Configuration.SpeedMode = true;
            新建流程ToolStripMenuItem.Enabled = false;
            ButtonEnable(false);

            //Job.GetJobTree(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).selec
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            //Frm_UserForm.Instance.AlarmClear();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 新建流程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.CreateScheme();
        }

        private void 打开流程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.OpenScheme();
        }
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Project.InportProject();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.SaveProject();
            // SaveAll();
            Project.Instance.configuration.Save();
        }

        private void 退出ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Frm_Login.Instance.ShowDialog();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Open the Settings page" : "打开设置页面");
            Frm_Setting.Instance.ShowDialog();
        }

        private void 菜单栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



        private void 流程编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Frm_Job.Instance.DockState == DockState.Hidden || Frm_Job.Instance.DockState == DockState.Unknown)
                Frm_Job.Instance.Show(Frm_Main.Instance.dockPanel, DockState.DockRight);
        }

        private void 图像窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                CreateNewImageWindow();

                //需要重新保存一下布局
                SaveDockLayout(true);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }



        private void 工具箱ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowToolboxInVisionSidebar();
        }



        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Job.CreateJob();
        }

        private void 导出ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                Job.ExportJob();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 导入ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                if (Project.Instance.L_engineList.Count == 0)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n未创建方案，请先创建方案");
                    Frm_EngineManager.Instance.Show();
                    return;
                }

                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = "";
                dig_openImage.Title = (Project.Instance.configuration.language == Language.English ? "Please select a job file" : "请选择流程文件");
                dig_openImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openImage.Filter = (Project.Instance.configuration.language == Language.English ? "job file(*.job)|*.job" : "流程文件(*.job)|*.job");
                dig_openImage.ShowDialog();
                if (dig_openImage.FileName == string.Empty)
                {
                    return;
                }
                try
                {
                    Job job = Job.LoadJob(dig_openImage.FileName);
                    // File.Copy(dig_openImage.FileName, Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + job.jobName + ".job");
                }
                catch { }
                Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "A new process was imported" : "导入了新流程");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 克隆ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Job.CloneJob();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Job.DeleteJob();
        }

        private void 读取图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = string.Empty;
                dig_openImage.Title = Project.Instance.configuration.language == Language.English ? "Please select image path" : "请选择图像文件";
                dig_openImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openImage.Filter = Project.Instance.configuration.language == Language.English ? "Image File(*.*)|*.*|Image File(*.png)|*.txt|Image File(*.jpg)|*.jpg|Image File(*.bmp)|*.bmp|Image File(*.tif)|*.tif" : "图像文件(*.jpg)|*.jpg|图像文件(*.tif)|*.tif|图像文件(*.png)|*.txt|图像文件(*.bmp)|*.bmp|图像文件(*.*)|*.*";
                if (dig_openImage.ShowDialog() == DialogResult.OK)
                {
                    HObject image;
                    try
                    {
                        HOperatorSet.ReadImage(out image, dig_openImage.FileName);
                    }
                    catch
                    {
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Unable to read specified file" : "图像文件异常，无法读取", Color.Red);
                        return;
                    }


                    IDockContent temp = dockPanel.ActiveContent;
                    Frm_ImageWindow ff = temp as Frm_ImageWindow;
                    if (ff != null)
                    {


                        ff.hwc_imageWindow.HobjectToHimage(image);
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Loading Image successfully" : "读取图像成功", Color.Green);
                    }
                    else
                    {
                        Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 保存窗口原图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(imageSavePath))
                {
                    Directory.CreateDirectory(imageSavePath);
                }
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                int index;
                for (index = 1; index < 100; index++)
                {
                    if (!File.Exists(imageSavePath + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "_" + index + ".bmp"))
                        break;
                }
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd") + "_" + index;
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择图像保存路径";
                dig_saveImage.Filter = "图像文件(*.bmp)|*.bmp|图像文件(*.jpg)|*.jpg|Image File|*.tif|Image File(*.png)|*.txt|Image File(*.*)|*.*";
                dig_saveImage.InitialDirectory = imageSavePath;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {





                    //HOperatorSet.DumpWindowImage(out image, ff.WindowHandle);
                    //HOperatorSet.WriteImage(image, "tiff", 0, dig_saveImage.FileName);
                    //Frm_Main.Instance.OutputMsg("Image saved successfully", Color.Green);







                    string fileName = dig_saveImage.FileName;
                    imageSavePath = Path.GetDirectoryName(dig_saveImage.FileName);


                    IDockContent temp = dockPanel.ActiveContent;
                    Frm_ImageWindow ff = temp as Frm_ImageWindow;
                    if (ff != null)
                    {

                        HOperatorSet.WriteImage(ff.currentImage, "jpg", 0, dig_saveImage.FileName);
                        Frm_Main.Instance.OutputMsg("Image saved successfully", Color.Green);
                    }
                    else
                    {
                        Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 保存窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {


                if (!Directory.Exists(imageSavePath))
                {
                    Directory.CreateDirectory(imageSavePath);
                }
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                int index;
                for (index = 1; index < 100; index++)
                {
                    if (!File.Exists(imageSavePath + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "_" + index + ".bmp"))
                        break;
                }
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd") + "_" + index;
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择图像保存路径";
                dig_saveImage.Filter = "Image File(*.bmp)|*.bmp|图像文件(*.jpg)|*.jpg|Image File|*.tif|Image File(*.png)|*.txt|Image File(*.*)|*.*";
                dig_saveImage.InitialDirectory = imageSavePath;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dig_saveImage.FileName;
                    HObject image;

                    IDockContent temp = dockPanel.ActiveDocument;
                    Frm_ImageWindow ff = temp as Frm_ImageWindow;
                    if (ff != null)
                    {

                        HOperatorSet.DumpWindowImage(out image, ff.WindowHandle);
                        HOperatorSet.WriteImage(image, "bmp", 0, dig_saveImage.FileName);
                        Frm_Main.Instance.OutputMsg("Image saved successfully", Color.Black);
                    }
                    else
                    {
                        Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 两点测距ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.ContextMenuStrip = null;
                HOperatorSet.SetColor(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, new HTuple("green"));
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.Focus();
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.Select();

                HTuple row, column;
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.DrawModel = true;
                HOperatorSet.DrawPoint(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, out row, out column);
                HOperatorSet.SetLineWidth(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, new HTuple(2));
                HOperatorSet.DispCross(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, row, column, 40, 0);
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;

                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.ContextMenuStrip = null;
                HOperatorSet.SetColor(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, new HTuple("green"));
                HTuple row1, column1;
                HOperatorSet.DrawPoint(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, out row1, out column1);
                HOperatorSet.SetLineWidth(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, new HTuple(2));
                HOperatorSet.DispCross(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, row1, column1, 40, 0);
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).hwc_imageWindow.DrawModel = false;

                HOperatorSet.DispArrow(GetImageWindowControl(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).WindowHandle, row, column, row1, column1, new HTuple(10));
                HTuple distance;
                HOperatorSet.DistancePp(row, column, row1, column1, out distance);
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "\r\nPixel distance" : "\r\n距离：" + ((double)distance).ToString("0.000") + " pix");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 虚拟键盘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processKeyBoard = System.Diagnostics.Process.Start("osk.exe");
        }

        private void 截屏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Rectangle bounds = Screen.FromControl(this).Bounds;
                using (Bitmap image = new Bitmap(bounds.Width, bounds.Height))
                using (Graphics graphics = Graphics.FromImage(image))
                using (System.Windows.Forms.SaveFileDialog saveImageDialog = new System.Windows.Forms.SaveFileDialog())
                {
                    graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    saveImageDialog.Title = Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择图像保存路径";
                    saveImageDialog.Filter = "图像文件(*.jpg)|*.jpg|Image File|*.tif|Image File(*.png)|*.png|Image File(*.bmp)|*.bmp";
                    saveImageDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    saveImageDialog.FileName = DateTime.Now.ToString("yyyy_MM_dd");
                    if (saveImageDialog.ShowDialog() == DialogResult.OK)
                        image.Save(saveImageDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }



        private void 系统重置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "Are you sure you want to reset all the setting and config?" : "      确定要将程序恢复到初始状态吗？这将清除所有配置及流程\r\n文件，使程序恢复到安装完毕时的初始状态！");
                Frm_ConfirmBox.Instance.ShowDialog();
                if (Frm_ConfirmBox.Instance.Result != ConfirmBoxResult.Yes)
                {
                    return;
                }

                //for (int i = Frm_ImageWindow.D_imageWindow.Count - 1; i >= 0; i--)
                //{
                //    Frm_ImageWindow.D_imageWindow.Values.ToArray()[i].Close();
                //}

                //Project.Instance.L_engineList.Clear();
                //Project.Instance.configuration = new Configuration();

                // 工厂布局随程序发布且只读。重置用户配置时不删除模板，避免缺少
                // 历史“副本”文件时把标准布局永久移除。

                //删除所有配置文件
                if (Directory.Exists(Application.StartupPath + "\\Config\\Project"))
                    Directory.Delete(Application.StartupPath + "\\Config\\Project", true);
                if (File.Exists(Application.StartupPath + "\\Config\\Configuration.ini"))
                    File.Delete(Application.StartupPath + "\\Config\\Configuration.ini");
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "\r\nReset succeeded! (Effective after restart, the program will shut down automatically)" : "\r\n重置成功!（重启后生效，确定后程序将自动关闭）");
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Project.Instance.configuration.dataPath + "\\Log");
        }



        private void button4_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            System.Drawing.Point p = new System.Drawing.Point();
            p.X = this.Location.X + panel3.Location.X + button.Location.X + 2;
            p.Y = this.Location.Y + panel3.Location.Y + button.Location.Y + 22;
            contextMenuStrip1.Show(p);
        }

        private void 反馈和建议ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Frm_Feedback.Instance.ShowDialog();
        }

        private void 激活ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n开发版无需激活，默认已激活！");
        }

        private void 关于ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Frm_About.Instance.ShowDialog();
        }

        private void toolStripButton32_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!Permission.CheckPermission(PermissionLevel.Admin))
                //    return;
                toolStripButton32.Image = Resources.锁定__1_;
                locked = true;
                Frm_Lock frm_lock = new Frm_Lock();
                frm_lock.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton10_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton10.Image = Resources.Setting;
        }

        private void toolStripButton10_MouseLeave(object sender, EventArgs e)
        {
            toolStripButton10.Image = Resources.Setting1;
        }

        private void toolStripButton2_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton2.Image = Resources.Login;
        }

        private void toolStripButton32_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void toolStripButton32_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton32.Image = Resources.锁定__1_;
        }

        private void toolStripButton7_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton7.Image = Resources.退出777;
        }
        internal static bool locked = false;
        private void toolStripButton2_MouseLeave(object sender, EventArgs e)
        {
            toolStripButton2.Image = Resources.Login1;
        }

        private void toolStripButton32_MouseLeave(object sender, EventArgs e)
        {
            if (!locked)
                toolStripButton32.Image = Resources.锁定;
        }

        private void toolStripButton7_MouseLeave(object sender, EventArgs e)
        {
            toolStripButton7.Image = Resources.退出;
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Project.ExportProject();
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                if (Frm_Job.Instance.tbc_jobs.TabPages.Count == 0)
                {
                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "No jobs to run" : "没有可运行的流程", Color.Green);
                    return;
                }
                Frm_Job.Instance.btn_runOnce.Enabled = false;
                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                Job job = Job.FindJobByName(jobName);
                Job.RunAndWait(job.jobName);
                Frm_Job.Instance.btn_runOnce.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;


                if (Frm_Job.Instance.btn_runLoop.Text == "连续运行" ||
                    Frm_Job.Instance.btn_runLoop.Text == "Run Loop")
                    Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).LoopRun(true);
                else
                    Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).LoopRun(false);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            Project.SaveProject();
            // SaveAll();
            Project.Instance.configuration.Save();

        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Frm_EngineManager.Instance.Show();
            Frm_EngineManager.Instance.WindowState = FormWindowState.Normal;
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.ExportScheme();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.InportScheme();
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                Scheme.CreateScheme();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                if (Project.Instance.L_engineList.Count > 0)
                {
                    Frm_ConfirmBox.Instance.lbl_info.Text = Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : "确定要删除当前方案吗？";
                    Frm_ConfirmBox.Instance.ShowDialog();
                    if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Cancel)
                    {
                        return;
                    }
                    Project.Instance.L_engineList.Remove(Project.Instance.curEngine);
                    if (Project.Instance.L_engineList.Count > 0)
                        Project.Instance.curEngine = Project.Instance.L_engineList[0];
                    else
                        Project.Instance.curEngine = null;


                    //////foreach (TabPage tabPage in Frm_Job.Instance.tbc_jobs.TabPages)
                    //////{
                    //////    string jobName = tabPage.Text;
                    //////    Job.RemoveJobByName(jobName);
                    //////    for (int j = 0; j < Frm_Job.Instance.tbc_jobs.TabPages.Count; j++)
                    //////    {
                    //////        if (Frm_Job.Instance.tbc_jobs.TabPages[j].Text == jobName)
                    //////        {
                    //////            Frm_Job.Instance.tbc_jobs.TabPages.RemoveAt(j);
                    //////        }
                    //////    }
                    //////    if (File.Exists(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + jobName + ".job"))
                    //////        File.Delete(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + jobName + ".job");
                    //////}
                    Frm_Main.Instance.OutputMsg("项目删除成功", Color.Black);
                }
                else
                {
                    Frm_Output.Instance.OutputMsg("当前项目中未添加任何方案", Color.Red);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.CloneScheme();
        }

        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Job.CreateJob();
        }

        private void toolStripButton21_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Job.CloneJob();
        }

        private void toolStripButton22_Click(object sender, EventArgs e)
        {
            Job.DeleteJob();
        }

        private void toolStripButton29_Click(object sender, EventArgs e)
        {
            try
            {
                ShowToolboxInVisionSidebar();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton31_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Frm_LayoutManage.Instance.Show();
        }

        private void toolStripButton23_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = string.Empty;
                dig_openImage.Title = Project.Instance.configuration.language == Language.English ? "Please select image path" : "请选择图像文件";
                dig_openImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openImage.Filter = Project.Instance.configuration.language == Language.English ? "Image File(*.*)|*.*|Image File(*.png)|*.txt|Image File(*.jpg)|*.jpg|Image File(*.bmp)|*.bmp|Image File(*.tif)|*.tif" : "图像文件(*.jpg)|*.jpg|图像文件(*.tif)|*.tif|图像文件(*.png)|*.txt|图像文件(*.bmp)|*.bmp|图像文件(*.*)|*.*";
                if (dig_openImage.ShowDialog() == DialogResult.OK)
                {
                    HObject image;
                    try
                    {
                        HOperatorSet.ReadImage(out image, dig_openImage.FileName);
                    }
                    catch
                    {
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Unable to read specified file" : "图像文件异常，无法读取", Color.Red);
                        return;
                    }


                    IDockContent temp = dockPanel.ActiveContent;
                    Frm_ImageWindow ff = temp as Frm_ImageWindow;
                    if (ff != null)
                    {


                        ff.hwc_imageWindow.HobjectToHimage(image);
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Loading Image successfully" : "读取图像成功", Color.Green);
                    }
                    else
                    {
                        Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton24_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(imageSavePath))
                {
                    Directory.CreateDirectory(imageSavePath);
                }
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                int index;
                for (index = 1; index < 100; index++)
                {
                    if (!File.Exists(imageSavePath + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "_" + index + ".bmp"))
                        break;
                }
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd") + "_" + index;
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择图像保存路径";
                dig_saveImage.Filter = "图像文件(*.bmp)|*.bmp|图像文件(*.jpg)|*.jpg|Image File|*.tif|Image File(*.png)|*.txt|Image File(*.*)|*.*";
                dig_saveImage.InitialDirectory = imageSavePath;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {





                    //HOperatorSet.DumpWindowImage(out image, ff.WindowHandle);
                    //HOperatorSet.WriteImage(image, "tiff", 0, dig_saveImage.FileName);
                    //Frm_Main.Instance.OutputMsg("Image saved successfully", Color.Green);







                    string fileName = dig_saveImage.FileName;
                    imageSavePath = Path.GetDirectoryName(dig_saveImage.FileName);


                    IDockContent temp = dockPanel.ActiveContent;
                    Frm_ImageWindow ff = temp as Frm_ImageWindow;
                    if (ff != null)
                    {

                        HOperatorSet.WriteImage(ff.currentImage, "jpg", 0, dig_saveImage.FileName);
                        Frm_Main.Instance.OutputMsg("图像保存成功", Color.Green);
                    }
                    else
                    {
                        Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton25_Click(object sender, EventArgs e)
        {
            try
            {


                if (!Directory.Exists(imageSavePath))
                {
                    Directory.CreateDirectory(imageSavePath);
                }
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                int index;
                for (index = 1; index < 100; index++)
                {
                    if (!File.Exists(imageSavePath + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "_" + index + ".bmp"))
                        break;
                }
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd") + "_" + index;
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择图像保存路径";
                dig_saveImage.Filter = "Image File(*.bmp)|*.bmp|图像文件(*.jpg)|*.jpg|Image File|*.tif|Image File(*.png)|*.txt|Image File(*.*)|*.*";
                dig_saveImage.InitialDirectory = imageSavePath;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dig_saveImage.FileName;
                    HObject image;

                    IDockContent temp = dockPanel.ActiveDocument;
                    Frm_ImageWindow ff = temp as Frm_ImageWindow;
                    if (ff != null)
                    {

                        HOperatorSet.DumpWindowImage(out image, ff.WindowHandle);
                        HOperatorSet.WriteImage(image, "bmp", 0, dig_saveImage.FileName);
                        Frm_Main.Instance.OutputMsg("Image saved successfully", Color.Black);
                    }
                    else
                    {
                        Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton1_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton1.Image = Resources.电机__6_;
        }

        private void toolStripButton1_MouseLeave(object sender, EventArgs e)
        {
            if (Machine.curFormMode != FormMode.MotionForm)
                toolStripButton1.Image = Resources.电机__8_;
        }

        private void toolStripButton28_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                CreateNewImageWindow();

                //需要重新保存一下布局
                SaveDockLayout(true);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void 布局管理ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Frm_LayoutManage.Instance.Show();
        }

        private void InitializeLayoutMenu()
        {
            try
            {
                // 添加左中右布局菜单项
                ToolStripMenuItem leftCenterRightLayout = new ToolStripMenuItem();
                leftCenterRightLayout.BackColor = System.Drawing.Color.White;
                leftCenterRightLayout.CheckOnClick = true;
                leftCenterRightLayout.Name = "切换到左中右布局ToolStripMenuItem";
                leftCenterRightLayout.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
                leftCenterRightLayout.Size = new System.Drawing.Size(200, 27);
                leftCenterRightLayout.Text = "左中右布局（图像-工具箱-流程）";
                leftCenterRightLayout.Click += 切换到左中右布局_Click;

                // 在"切换到经典布局2"后面插入
                int insertIndex = 布局ToolStripMenuItem.DropDownItems.IndexOf(切换到经典布局2ToolStripMenuItem);
                if (insertIndex >= 0)
                    布局ToolStripMenuItem.DropDownItems.Insert(insertIndex + 1, leftCenterRightLayout);
                else
                    布局ToolStripMenuItem.DropDownItems.Add(leftCenterRightLayout);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 切换到左中右布局_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
                if (menuItem != null && menuItem.Checked)
                {
                    切换到经典布局1ToolStripMenuItem.Checked = false;
                    切换到经典布局2ToolStripMenuItem.Checked = false;
                    Project.Instance.configuration.layoutFilePath = "Config\\Resources\\Layout\\" + "左中右布局.config";
                }
                else
                {
                    Project.Instance.configuration.layoutFilePath = "dockPanel.config";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 切换到经典布局1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (切换到经典布局1ToolStripMenuItem.Checked)
            {
                切换到经典布局2ToolStripMenuItem.Checked = false;
                Project.Instance.configuration.layoutFilePath = "Config\\Resources\\Layout\\" + "经典布局1.config";
            }
            else
                Project.Instance.configuration.layoutFilePath = "dockPanel.config";
            //////试图ToolStripMenuItem.ShowDropDown();
            //////布局ToolStripMenuItem.ShowDropDown();
            //////布局ToolStripMenuItem.Select();
            //////切换到经典布局1ToolStripMenuItem.Select();
            //////Thread.Sleep(500);
            //////试图ToolStripMenuItem.HideDropDown();
        }

        private void 切换到经典布局2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (切换到经典布局2ToolStripMenuItem.Checked)
            {
                切换到经典布局1ToolStripMenuItem.Checked = false;
                Project.Instance.configuration.layoutFilePath = "Config\\Resources\\Layout\\" + "经典布局2.config";
            }
            else
                Project.Instance.configuration.layoutFilePath = "Config\\dockPanel.config";
            //////试图ToolStripMenuItem.ShowDropDown();
            //////布局ToolStripMenuItem.ShowDropDown();
            //////布局ToolStripMenuItem.Select();
            //////切换到经典布局2ToolStripMenuItem.Select();

            //////Thread.Sleep(500);
            //////试图ToolStripMenuItem.HideDropDown();
        }

        private void 解锁ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;

            dockPanel.AllowEndUserDocking = !dockPanel.AllowEndUserDocking;
            //if (dockPanel.AllowEndUserDocking)
            //{
            //    解锁ToolStripMenuItem.Text = "已解锁";
            //}
            //else
            //{
            //    解锁ToolStripMenuItem.Text = "已锁定";

            //}
            //////试图ToolStripMenuItem.ShowDropDown();
            //////布局ToolStripMenuItem.ShowDropDown();
            //////布局ToolStripMenuItem.Select();
            //////解锁ToolStripMenuItem.Select();

            //////Thread.Sleep(500);
            //////试图ToolStripMenuItem.HideDropDown();
            Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Interface lock enabled" : "界面锁定启用");
        }

        private void 登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_Login.Instance.ShowDialog();
        }

        private void 锁定ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                //if (!Permission.CheckPermission(PermissionLevel.Admin))
                //    return;
                toolStripButton32.Image = Resources.锁定__1_;
                locked = true;
                Frm_Lock frm_lock = new Frm_Lock();
                frm_lock.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void lbl_title_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void lbl_title_DragLeave(object sender, EventArgs e)
        {

        }

        private void lbl_title_DragOver(object sender, DragEventArgs e)
        {

        }

        private void 运行一次ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton11.PerformClick();
        }

        private void 连续运行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton12.PerformClick();
        }
        Stopwatch sw = new Stopwatch();
        Thread th;
        private void toolStripButton8_MouseDown(object sender, MouseEventArgs e)
        {
            isBreak = false;
            sw.Start();
            th = new Thread(Test);
            th.IsBackground = true;
            th.Start();
        }
        bool isBreak = false;
        private void Test()
        {
            while (sw.ElapsedMilliseconds < 8000 && !isBreak)
            {
                Thread.Sleep(100);
                toolStripButton8.Text = string.Format("复位({0})", sw.Elapsed.Seconds);
                Application.DoEvents();
            }
            toolStripButton8.Text = "复位";
            sw.Stop();
            sw.Reset();
        }

        private void toolStripButton8_MouseUp(object sender, MouseEventArgs e)
        {
            if (sw.ElapsedMilliseconds > 2000)
            {
                isBreak = true;
                sw.Stop();
                sw.Reset();
                toolStripButton8.Text = "复位";
                //////if (!Permission.CheckPermission(PermissionLevel.Operator))
                //////    return;
                Machine.Home();



                Machine.machineRunStatu = MachineRunStatu.WaitRun;
                toolStripButton8.Image = Resources.复位__1_;
                toolStripButton3.Image = Resources.停止__1_;
                toolStripButton4.Image = Resources.启动;
                toolStripButton36.Image = Resources.停止__11111_;
                toolStripButton8.Font = new System.Drawing.Font(toolStripButton8.Font.FontFamily, toolStripButton8.Font.Size, FontStyle.Bold);
                toolStripButton4.Font = new System.Drawing.Font(toolStripButton4.Font.FontFamily, toolStripButton4.Font.Size, FontStyle.Regular);
                toolStripButton3.Font = new System.Drawing.Font(toolStripButton3.Font.FontFamily, toolStripButton3.Font.Size, FontStyle.Regular);
                toolStripButton36.Font = new System.Drawing.Font(toolStripButton36.Font.FontFamily, toolStripButton36.Font.Size, FontStyle.Regular);

                toolStripButton8.Text = "已复位";
                toolStripButton4.Text = "启动";
                toolStripButton3.Text = "停止";
                toolStripButton36.Text = "暂停";
                toolStripButton4.Enabled = true;
                //////toolStripButton4.Enabled = true;

                //Frm_UserForm.Instance.Home();
            }
            else
            {
                Frm_Main.Instance.OutputMsg("复位请长按超过2秒", Color.Red);
                isBreak = true;
                sw.Stop();
                sw.Reset();
                toolStripButton8.Text = "复位";
            }
        }


        private void toolStripButton26_Click(object sender, EventArgs e)
        {
            if (!CanNavigatePreviousLocalImage())
                return;

            Job selectedJob = Project.Instance.curEngine.FindJobByName(
                Frm_Job.Instance.tbc_jobs.SelectedTab.Text);
            for (int i = 0; i < selectedJob.L_toolList.Count; i++)
            {
                if (selectedJob.L_toolList[i].toolType == ToolType.ImageAcq)
                {
                    AcqImageTool imageAcqTool = selectedJob.L_toolList[i].tool as AcqImageTool;
                    if (imageAcqTool != null && imageAcqTool.imageSourceMode == ImageSourceMode.FromDirectory)
                    {
                        imageAcqTool.currentImageIndex = imageAcqTool.currentImageIndex - 2;
                        Job.RunAndWait(Frm_Job.Instance.tbc_jobs.SelectedTab.Text);
                    }
                }
            }
        }

        private bool CanNavigatePreviousLocalImage()
        {
            if (Project.Instance.curEngine == null ||
                Frm_Job.Instance.tbc_jobs.SelectedTab == null)
                return false;

            Job selectedJob = Project.Instance.curEngine.FindJobByName(
                Frm_Job.Instance.tbc_jobs.SelectedTab.Text);
            if (selectedJob == null || selectedJob.L_toolList == null)
                return false;

            for (int index = 0; index < selectedJob.L_toolList.Count; index++)
            {
                if (selectedJob.L_toolList[index] == null ||
                    selectedJob.L_toolList[index].toolType != ToolType.ImageAcq)
                    continue;

                AcqImageTool imageTool = selectedJob.L_toolList[index].tool as AcqImageTool;
                if (imageTool != null && imageTool.imageSourceMode == ImageSourceMode.FromDirectory)
                    return true;
            }
            return false;
        }




        /// <summary>
        /// 设置显示字体
        /// </summary>
        internal void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font, HTuple hv_Bold, HTuple hv_Slant)
        {
            try
            {
                HTuple hv_OS = null, hv_BufferWindowHandle = new HTuple();
                HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
                HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
                HTuple hv_Scale = new HTuple(), hv_Exception = new HTuple();
                HTuple hv_SubFamily = new HTuple(), hv_Fonts = new HTuple();
                HTuple hv_SystemFonts = new HTuple(), hv_Guess = new HTuple();
                HTuple hv_I = new HTuple(), hv_Index = new HTuple(), hv_AllowedFontSizes = new HTuple();
                HTuple hv_Distances = new HTuple(), hv_Indices = new HTuple();
                HTuple hv_FontSelRegexp = new HTuple(), hv_FontsCourier = new HTuple();
                HTuple hv_Bold_COPY_INP_TMP = hv_Bold.Clone();
                HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
                HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();
                HTuple hv_Slant_COPY_INP_TMP = hv_Slant.Clone();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    try
                    {
                        HOperatorSet.OpenWindow(0, 0, 256, 256, 0, "buffer", "", out hv_BufferWindowHandle);
                        HOperatorSet.SetFont(hv_BufferWindowHandle, "-Consolas-16-*-0-*-*-1-");
                        HOperatorSet.GetStringExtents(hv_BufferWindowHandle, "test_string", out hv_Ascent,
                            out hv_Descent, out hv_Width, out hv_Height);
                        hv_Scale = 110.0 / hv_Width;
                        hv_Size_COPY_INP_TMP = ((hv_Size_COPY_INP_TMP * hv_Scale)).TupleInt();
                        HOperatorSet.CloseWindow(hv_BufferWindowHandle);
                    }
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    }
                    if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                        new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "Courier New";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "Consolas";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "Arial";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "Times New Roman";
                    }
                    if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_Bold_COPY_INP_TMP = 1;
                    }
                    else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
                    {
                        hv_Bold_COPY_INP_TMP = 0;
                    }
                    else
                    {
                        hv_Exception = "Wrong value of control parameter Bold";
                        throw new HalconException(hv_Exception);
                    }
                    if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_Slant_COPY_INP_TMP = 1;
                    }
                    else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
                    {
                        hv_Slant_COPY_INP_TMP = 0;
                    }
                    else
                    {
                        hv_Exception = "Wrong value of control parameter Slant";
                        throw new HalconException(hv_Exception);
                    }
                    try
                    {
                        HOperatorSet.SetFont(hv_WindowHandle, ((((((("-" + hv_Font_COPY_INP_TMP) + "-") + hv_Size_COPY_INP_TMP) + "-*-") + hv_Slant_COPY_INP_TMP) + "-*-*-") + hv_Bold_COPY_INP_TMP) + "-");
                    }
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    }
                }
                else if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Dar"))) != 0)
                {
                    hv_SubFamily = 0;
                    if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_SubFamily = hv_SubFamily.TupleBor(1);
                    }
                    else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
                    {
                        hv_Exception = "Wrong value of control parameter Slant";
                        throw new HalconException(hv_Exception);
                    }
                    if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_SubFamily = hv_SubFamily.TupleBor(2);
                    }
                    else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
                    {
                        hv_Exception = "Wrong value of control parameter Bold";
                        throw new HalconException(hv_Exception);
                    }
                    if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Fonts = new HTuple();
                        hv_Fonts[0] = "Menlo-Regular";
                        hv_Fonts[1] = "Menlo-Italic";
                        hv_Fonts[2] = "Menlo-Bold";
                        hv_Fonts[3] = "Menlo-BoldItalic";
                    }
                    else if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                        new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                    {
                        hv_Fonts = new HTuple();
                        hv_Fonts[0] = "CourierNewPSMT";
                        hv_Fonts[1] = "CourierNewPS-ItalicMT";
                        hv_Fonts[2] = "CourierNewPS-BoldMT";
                        hv_Fonts[3] = "CourierNewPS-BoldItalicMT";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Fonts = new HTuple();
                        hv_Fonts[0] = "ArialMT";
                        hv_Fonts[1] = "Arial-ItalicMT";
                        hv_Fonts[2] = "Arial-BoldMT";
                        hv_Fonts[3] = "Arial-BoldItalicMT";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                    {
                        hv_Fonts = new HTuple();
                        hv_Fonts[0] = "TimesNewRomanPSMT";
                        hv_Fonts[1] = "TimesNewRomanPS-ItalicMT";
                        hv_Fonts[2] = "TimesNewRomanPS-BoldMT";
                        hv_Fonts[3] = "TimesNewRomanPS-BoldItalicMT";
                    }
                    else
                    {
                        HOperatorSet.QueryFont(hv_WindowHandle, out hv_SystemFonts);
                        hv_Fonts = new HTuple();
                        hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Guess = new HTuple();
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Regular");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "MT");
                        for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                        {
                            HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                            if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Fonts == null)
                                    hv_Fonts = new HTuple();
                                hv_Fonts[0] = hv_Guess.TupleSelect(hv_I);
                                break;
                            }
                        }
                        hv_Guess = new HTuple();
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Italic");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-ItalicMT");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Oblique");
                        for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                        {
                            HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                            if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Fonts == null)
                                    hv_Fonts = new HTuple();
                                hv_Fonts[1] = hv_Guess.TupleSelect(hv_I);
                                break;
                            }
                        }
                        hv_Guess = new HTuple();
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Bold");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldMT");
                        for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                        {
                            HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                            if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Fonts == null)
                                    hv_Fonts = new HTuple();
                                hv_Fonts[2] = hv_Guess.TupleSelect(hv_I);
                                break;
                            }
                        }
                        hv_Guess = new HTuple();
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalic");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalicMT");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldOblique");
                        for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                        {
                            HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                            if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Fonts == null)
                                    hv_Fonts = new HTuple();
                                hv_Fonts[3] = hv_Guess.TupleSelect(hv_I);
                                break;
                            }
                        }
                    }
                    hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(hv_SubFamily);
                    try
                    {
                        HOperatorSet.SetFont(hv_WindowHandle, (hv_Font_COPY_INP_TMP + "-") + hv_Size_COPY_INP_TMP);
                    }
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    }
                }
                else
                {
                    hv_Size_COPY_INP_TMP = hv_Size_COPY_INP_TMP * 1.25;
                    hv_AllowedFontSizes = new HTuple();
                    hv_AllowedFontSizes[0] = 11;
                    hv_AllowedFontSizes[1] = 14;
                    hv_AllowedFontSizes[2] = 17;
                    hv_AllowedFontSizes[3] = 20;
                    hv_AllowedFontSizes[4] = 25;
                    hv_AllowedFontSizes[5] = 34;
                    if ((int)(new HTuple(((hv_AllowedFontSizes.TupleFind(hv_Size_COPY_INP_TMP))).TupleEqual(
                        -1))) != 0)
                    {
                        hv_Distances = ((hv_AllowedFontSizes - hv_Size_COPY_INP_TMP)).TupleAbs();
                        HOperatorSet.TupleSortIndex(hv_Distances, out hv_Indices);
                        hv_Size_COPY_INP_TMP = hv_AllowedFontSizes.TupleSelect(hv_Indices.TupleSelect(
                            0));
                    }
                    if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))).TupleOr(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(
                        "Courier")))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "courier";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "helvetica";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "times";
                    }
                    if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_Bold_COPY_INP_TMP = "bold";
                    }
                    else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
                    {
                        hv_Bold_COPY_INP_TMP = "medium";
                    }
                    else
                    {
                        hv_Exception = "Wrong value of control parameter Bold";
                        throw new HalconException(hv_Exception);
                    }
                    if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("times"))) != 0)
                        {
                            hv_Slant_COPY_INP_TMP = "i";
                        }
                        else
                        {
                            hv_Slant_COPY_INP_TMP = "o";
                        }
                    }
                    else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
                    {
                        hv_Slant_COPY_INP_TMP = "r";
                    }
                    else
                    {
                        hv_Exception = "Wrong value of control parameter Slant";
                        throw new HalconException(hv_Exception);
                    }
                    try
                    {
                        HOperatorSet.SetFont(hv_WindowHandle, ((((((("-adobe-" + hv_Font_COPY_INP_TMP) + "-") + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
                    }
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        if ((int)((new HTuple(((hv_OS.TupleSubstr(0, 4))).TupleEqual("Linux"))).TupleAnd(
                            new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                        {
                            HOperatorSet.QueryFont(hv_WindowHandle, out hv_Fonts);
                            hv_FontSelRegexp = (("^-[^-]*-[^-]*[Cc]ourier[^-]*-" + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP;
                            hv_FontsCourier = ((hv_Fonts.TupleRegexpSelect(hv_FontSelRegexp))).TupleRegexpMatch(
                                hv_FontSelRegexp);
                            if ((int)(new HTuple((new HTuple(hv_FontsCourier.TupleLength())).TupleEqual(
                                0))) != 0)
                            {
                                hv_Exception = "Wrong font name";
                            }
                            else
                            {
                                try
                                {
                                    HOperatorSet.SetFont(hv_WindowHandle, (((hv_FontsCourier.TupleSelect(
                                        0)) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
                                }
                                catch (HalconException HDevExpDefaultException2)
                                {
                                    HDevExpDefaultException2.ToHTuple(out hv_Exception);
                                }
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


        private void toolStripButton33_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;

            Frm_DeviceManager.Instance.Show();
        }

        private void 全局变量ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Frm_GlobalVariable.Instance.Show();
            Frm_GlobalVariable.Instance.WindowState = FormWindowState.Normal;
        }




        private void 文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



        private void toolStripButton27_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton30_Click_1(object sender, EventArgs e)
        {



        }

        private void toolStripButton34_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;

            Frm_GlobalVariable.Instance.LoadVariable(1);
            Frm_GlobalVariable.Instance.cbx_variableType.SelectedIndex = 1;
            Frm_GlobalVariable.Instance.Show();
        }

        private void 输出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOutputInVisionBottomPanel();
        }

        private void toolStripButton27_Click_1(object sender, EventArgs e)
        {
            if (Configuration.SpeedMode)
            {
                Configuration.SpeedMode = false;
                toolStripButton27.Image = Resources.急速退款__1_;
            }
            else
            {
                Configuration.SpeedMode = true;
                toolStripButton27.Image = Resources.急速退款;
            }
        }

        private void lbl_curEngine_TextChanged(object sender, EventArgs e)
        {

        }



        private void lbl_curEngine_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.RemoveAll();
            toolTip1.ToolTipTitle = string.Empty;
            toolTip1.Show("点击可切换方案", statusStrip1);
        }







        private void tss_permissionInfo_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.RemoveAll();
            toolTip1.ToolTipTitle = string.Empty;
            toolTip1.Show("点击可登录用户", statusStrip1);
        }

        private void lbl_curEngine_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (lbl_curEngine.Text.Substring(5) != e.ClickedItem.Text)
                {
                    lbl_curEngine.Text = e.ClickedItem.Text;
                    Scheme.SwitchScheme(lbl_curEngine.Text);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 示例项目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = "";
                dig_openImage.Title = (Project.Instance.configuration.language == Language.English ? "Please select a job file" : "请选择相关示例文件");
                dig_openImage.InitialDirectory = Application.StartupPath + "\\Config\\Resources\\Sample";
                dig_openImage.Filter = (Project.Instance.configuration.language == Language.English ? "job file(*.job)|*.job" : "项目文件(*.pjt)|*.pjt|方案文件(*.eng)|*.eng|流程文件(*.job)|*.job");
                if (dig_openImage.ShowDialog() == DialogResult.OK)
                {
                    if (Project.Instance.configuration.L_recentlyOpendFile.Contains(dig_openImage.FileName))
                        Project.Instance.configuration.L_recentlyOpendFile.Remove(dig_openImage.FileName);
                    Project.Instance.configuration.L_recentlyOpendFile.Insert(0, dig_openImage.FileName);
                    if (Project.Instance.configuration.L_recentlyOpendFile.Count >= 5)
                        Project.Instance.configuration.L_recentlyOpendFile.RemoveRange(5, Project.Instance.configuration.L_recentlyOpendFile.Count - 5);

                    string temp = Path.GetExtension(dig_openImage.FileName);
                    if (Path.GetExtension(dig_openImage.FileName) == ".pjt")
                    {
                        Project project = Project.LoadProject(dig_openImage.FileName);
                        //Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                        //Application.DoEvents();
                        // Frm_ImageWindow.Instance.Update_Last_Run_Result_Image_List();

                        //  job .Draw_Line(null ,null );   //此处不能画线，否则就会出现添加第二个示例流程时报错的问题
                        Scheme engine = Project.Instance.L_engineList[0];
                        for (int i = 0; i < engine.L_jobList.Count; i++)
                        {
                            Job job = engine.L_jobList[i];
                            switch (job.jobName)
                            {
                                case "尺寸测量":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "斑点分析":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "机械手下视觉定位":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "机械手下视觉抓取定点放置":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "模板匹配":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "条码读取":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "位置跟随":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "焊点检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "OCR":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "记号检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "圆度检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                            }
                        }
                    }
                    else if (Path.GetExtension(dig_openImage.FileName) == ".eng")
                    {
                        Scheme engine = Scheme.LoadScheme(dig_openImage.FileName);
                        Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                        Application.DoEvents();
                        // Frm_ImageWindow.Instance.Update_Last_Run_Result_Image_List();

                        //  job .Draw_Line(null ,null );   //此处不能画线，否则就会出现添加第二个示例流程时报错的问题
                        for (int i = 0; i < engine.L_jobList.Count; i++)
                        {
                            Job job = engine.L_jobList[i];
                            switch (job.jobName)
                            {
                                case "尺寸测量":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "斑点分析":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "机械手下视觉定位":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "机械手下视觉抓取定点放置":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "模板匹配":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "条码读取":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "位置跟随":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "焊点检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "OCR":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "记号检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                                case "圆度检测":
                                    ClearSampleAcqImageDirectory(job);
                                    break;
                            }
                        }

                    }
                    else if (Path.GetExtension(dig_openImage.FileName) == ".job")
                    {
                        Job job = Job.LoadJob(dig_openImage.FileName);
                        Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                        Application.DoEvents();

                        //  job .Draw_Line(null ,null );   //此处不能画线，否则就会出现添加第二个示例流程时报错的问题
                        switch (job.jobName)
                        {
                            case "尺寸测量":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "斑点分析":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "机械手下视觉定位":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "机械手下视觉抓取定点放置":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "模板匹配":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "条码读取":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "位置跟随":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "焊点检测":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "OCR":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "记号检测":
                                ClearSampleAcqImageDirectory(job);
                                break;
                            case "圆度检测":
                                ClearSampleAcqImageDirectory(job);
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 帮助文档ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\Config\\Resources\\Help.html");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;
            Scheme.CloneScheme();
        }

        private void 打开ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                if (Project.Instance.L_engineList.Count == 0)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n未创建方案，请先创建方案");
                    Frm_EngineManager.Instance.Show();
                    return;
                }

                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = "";
                dig_openImage.Title = (Project.Instance.configuration.language == Language.English ? "Please select a job file" : "请选择流程文件");
                dig_openImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openImage.Filter = (Project.Instance.configuration.language == Language.English ? "job file(*.job)|*.job" : "流程文件(*.job)|*.job");
                dig_openImage.ShowDialog();
                if (dig_openImage.FileName == string.Empty)
                {
                    return;
                }
                try
                {
                    Job job = Job.LoadJob(dig_openImage.FileName);
                    // File.Copy(dig_openImage.FileName, Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + job.jobName + ".job");
                }
                catch { }
                Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "A new process was imported" : "导入了新流程");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton16_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;

                if (toolStripButton16.Tag == null)
                {
                    toolStripButton16.Tag = false;
                }


                if ((bool)toolStripButton16.Tag)
                {
                    for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                    {
                        Project.Instance.curEngine.L_jobList[i].LoopRun(false);
                    }
                    toolStripButton16.Tag = false;
                }
                else
                {
                    for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                    {
                        Project.Instance.curEngine.L_jobList[i].LoopRun(true);
                    }
                    toolStripButton16.Tag = true;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 采集设备ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            AcqImageTool acqImageTool = new AcqImageTool();


            Frm_AcqDevice.Instance.WindowState = FormWindowState.Normal;



            if (acqImageTool.imageSourceMode == ImageSourceMode.FromDevice)
            {
                Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Enabled = true;
                Frm_AcqDevice.Instance.rdo_fromDevice.Checked = true;
                Frm_AcqDevice.Instance.rdo_fromDevice.ForeColor = Color.FromArgb(18, 150, 219);
                Frm_AcqDevice.Instance.radio_FromLocalFile.ForeColor = Color.Black;
                Frm_AcqDevice.Instance.rdo_fromLocalDirectory.ForeColor = Color.Black;
                Frm_AcqDevice.Instance.rdo_fromDevice.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Bold);
                Frm_AcqDevice.Instance.radio_FromLocalFile.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Regular);
                Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Regular);
                Frm_AcqDevice.Instance.pic_fromDevice.Image = Resources.勾选;
                Frm_AcqDevice.Instance.pic_fromLocalFile.Image = Resources.去勾选;
                Frm_AcqDevice.Instance.pic_fromLocalDirectory.Image = Resources.去勾选;
                Frm_AcqDevice.Instance.pnl_formPanel.Controls.Clear();
                Frm_FromDevice1.Instance.TopLevel = false;
                Frm_FromDevice1.Instance.Parent = Frm_AcqDevice.Instance.pnl_formPanel;
                Frm_FromDevice1.Instance.Dock = DockStyle.Top;
                Frm_FromDevice1.Instance.Show();
            }
            else
            {
                if (acqImageTool.imageSourceMode == ImageSourceMode.FromDirectory)
                {
                    Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Checked = true;
                    Frm_FromLocal1.Instance.pnl_multImage.Visible = true;
                    Frm_AcqDevice.Instance.pic_fromLocalFile.Image = Resources.去勾选;
                    Frm_AcqDevice.Instance.pic_fromLocalDirectory.Image = Resources.勾选;
                    Frm_AcqDevice.Instance.pic_fromDevice.Image = Resources.去勾选;
                    Frm_AcqDevice.Instance.rdo_fromDevice.ForeColor = Color.Black;
                    Frm_AcqDevice.Instance.radio_FromLocalFile.ForeColor = Color.Black;
                    Frm_AcqDevice.Instance.rdo_fromLocalDirectory.ForeColor = Color.FromArgb(18, 150, 219);
                    Frm_AcqDevice.Instance.rdo_fromDevice.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Regular);
                    Frm_AcqDevice.Instance.radio_FromLocalFile.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Regular);
                    Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Bold);
                }
                else
                {
                    Frm_AcqDevice.Instance.radio_FromLocalFile.Checked = true;
                    Frm_FromLocal1.Instance.pnl_multImage.Visible = false;
                    Frm_AcqDevice.Instance.pic_fromLocalDirectory.Image = Resources.去勾选;
                    Frm_AcqDevice.Instance.pic_fromLocalFile.Image = Resources.勾选;
                    Frm_AcqDevice.Instance.pic_fromDevice.Image = Resources.去勾选;

                    Frm_AcqDevice.Instance.rdo_fromDevice.ForeColor = Color.Black;
                    Frm_AcqDevice.Instance.radio_FromLocalFile.ForeColor = Color.FromArgb(18, 150, 219);
                    Frm_AcqDevice.Instance.rdo_fromLocalDirectory.ForeColor = Color.Black;
                    Frm_AcqDevice.Instance.rdo_fromDevice.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Regular);
                    Frm_AcqDevice.Instance.radio_FromLocalFile.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Bold);
                    Frm_AcqDevice.Instance.rdo_fromLocalDirectory.Font = new Font(Frm_AcqDevice.Instance.rdo_fromDevice.Font.Name, Frm_AcqDevice.Instance.rdo_fromDevice.Font.Size, FontStyle.Regular);
                }

                Frm_AcqDevice.Instance.相机实时ToolStripMenuItem.Enabled = false;

                Frm_AcqDevice.Instance.pnl_formPanel.Controls.Clear();
                Frm_FromLocal1.Instance.TopLevel = false;
                Frm_FromLocal1.Instance.Parent = Frm_AcqDevice.Instance.pnl_formPanel;
                Frm_FromLocal1.Instance.Dock = DockStyle.Top;
                Frm_FromLocal1.Instance.Show();
            }

            //将对象信息更新到界面
            //Frm_AcqImageTool.Instance.pic_onOff.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
            //if (SDK_hikVisionTool.deviceInfoStr != string.Empty)
            //{
            Frm_FromDevice1.imageAcqTool = acqImageTool;
            Frm_FromLocal1.imageAcqTool = acqImageTool;
            //}

            Application.DoEvents();
            //Frm_AcqImageTool.Instance.ckb_displayAllImageRegion.Checked = acqImageTool.displayAllImageRegion;

            Frm_AcqDevice.Instance.ckb_RGBToGray.Checked = acqImageTool.RGBToGray;
            //Frm_AcqDevice.Instance.ckb_absPath.Checked = acqImageTool.absPath;
            Frm_AcqDevice.Instance.ckb_autoSwitch.Checked = acqImageTool.autoSwitch;

            Frm_AcqDevice.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", acqImageTool.currentImageName, acqImageTool.currentImageIndex + 1 + "/" + acqImageTool.L_images.Count);
            Frm_FromLocal1.Instance.tbx_imagePath.Text = acqImageTool.imagePath;
            Frm_FromLocal1.Instance.tbx_imageDirectoryPath.Text = acqImageTool.imageDirectoryPath;
            Frm_AcqDevice.Instance.lbl_toolTip.ForeColor = Color.Black;
            if (acqImageTool.L_images.Count != 0)
                Frm_AcqDevice.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", acqImageTool.currentImageName, acqImageTool.currentImageIndex + 1 + "/" + acqImageTool.L_images.Count);
            else
                Frm_AcqDevice.Instance.lbl_toolTip.Text = "状态：成功";
            Frm_AcqDevice.Instance.ckb_rotateImage.Checked = acqImageTool.rotateImage;
            Frm_AcqDevice.Instance.nud_rotateAngle.Value = acqImageTool.rotateAngle;
            Frm_AcqDevice.Instance.btn_runTool.Focus();



            Frm_AcqDevice.Instance.ShowDialog();




        }

        private void toolStripButton35_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;

                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    Job.RunAndWait(Project.Instance.curEngine.L_jobList[i].jobName);
                }

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 演示模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }

        private void toolStripButton36_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton36.Image = Resources.停止222;
        }

        private void toolStripButton36_MouseLeave(object sender, EventArgs e)
        {
            if (Machine.machineRunStatu != MachineRunStatu.Pause)
                toolStripButton36.Image = Resources.停止__11111_;
        }

        private void toolStripButton36_Click(object sender, EventArgs e)
        {
            //////if (!Permission.CheckPermission(PermissionLevel.Operator))
            //////    return;
            Machine.StopRun();

            Machine.machineRunStatu = MachineRunStatu.Pause;
            toolStripButton3.Image = Resources.停止__1_;
            toolStripButton4.Image = Resources.启动;
            toolStripButton8.Image = Resources.复位;
            toolStripButton36.Image = Resources.停止222;
            toolStripButton36.Font = new System.Drawing.Font(toolStripButton36.Font.FontFamily, toolStripButton36.Font.Size, FontStyle.Bold);
            toolStripButton4.Font = new System.Drawing.Font(toolStripButton4.Font.FontFamily, toolStripButton4.Font.Size, FontStyle.Regular);
            toolStripButton8.Font = new System.Drawing.Font(toolStripButton8.Font.FontFamily, toolStripButton8.Font.Size, FontStyle.Regular);
            toolStripButton3.Font = new System.Drawing.Font(toolStripButton3.Font.FontFamily, toolStripButton3.Font.Size, FontStyle.Regular);

            toolStripButton36.Text = "已暂停";
            toolStripButton3.Text = "停止";
            toolStripButton4.Text = "启动";
            toolStripButton8.Enabled = true;
            新建流程ToolStripMenuItem.Enabled = true;
            ButtonEnable(true);


        }


    }
}
