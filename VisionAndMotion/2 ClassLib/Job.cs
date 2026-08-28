using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Windows.Resources;
using VMPro.Properties;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.IO;
using Tool;
using System.Runtime.Serialization.Formatters.Binary;
using ViewROI;
using ViewWindow.Model;
using System.Reflection;

namespace VMPro
{
    [Serializable]
    public class Job
    {
        public Job()
        {


            if (rightClickMenuAtBlank.Items.Count == 0)
            {
                //Frm_Job.Instance.tbc_jobs.SelectedIndexChanged += tbc_jobs_SelectedIndexChanged;


                rightClickMenu.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                rightClickMenuAtBlank.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                ToolStripItem toolStripItem_折叠流程树 = rightClickMenuAtBlank.Items.Add(Project.Instance.configuration.language == Language.English ? "Fold Job Tree" : "折叠流程");
                toolStripItem_折叠流程树.BackColor = Color.White;
                toolStripItem_折叠流程树.Click += toolStripItem_折叠流程树_Click;
                ToolStripItem toolStripItem_保存当前流程 = rightClickMenuAtBlank.Items.Add(Project.Instance.configuration.language == Language.English ? "Save Current Job" : "保存流程");
                toolStripItem_保存当前流程.BackColor = Color.White;
                toolStripItem_保存当前流程.Click += SaveCurrentJob;
                ToolStripItem toolStripItem_展开流程树 = rightClickMenuAtBlank.Items.Add(Project.Instance.configuration.language == Language.English ? "Expand Job Tree" : "展开流程");
                toolStripItem_展开流程树.BackColor = Color.White;
                toolStripItem_展开流程树.Click += toolStripItem_展开流程树_Click;
                ToolStripItem toolStripItem_启用全部 = rightClickMenuAtBlank.Items.Add(Project.Instance.configuration.language == Language.English ? "Delete Job" : "启用全部");
                toolStripItem_启用全部.BackColor = Color.White;
                toolStripItem_启用全部.Click += toolStripItem_启用全部_Click;
                ToolStripItem toolStripItem_忽略全部 = rightClickMenuAtBlank.Items.Add(Project.Instance.configuration.language == Language.English ? "Delete Job" : "禁用全部");
                toolStripItem_忽略全部.BackColor = Color.White;
                toolStripItem_忽略全部.Click += toolStripItem_忽略全部_Click;
                ToolStripItem toolStripItem_粘贴 = rightClickMenuAtBlank.Items.Add(Project.Instance.configuration.language == Language.English ? "Job Info" : "粘贴");
                toolStripItem_粘贴.BackColor = Color.White;
                toolStripItem_粘贴.Click += PasteToolAtLast;
                ToolStripItem toolStripItem_删除当前流程 = rightClickMenuAtBlank.Items.Add(Project.Instance.configuration.language == Language.English ? "Delete Job" : "删除流程");
                toolStripItem_删除当前流程.BackColor = Color.White;
                toolStripItem_删除当前流程.Click += toolStripItem_删除流程_Click;
                toolStripItem_删除当前流程.Image = Resources.Delete1;
                ToolStripItem toolStripItem_流程属性 = rightClickMenuAtBlank.Items.Add(Project.Instance.configuration.language == Language.English ? "Job Info" : "流程属性");
                toolStripItem_流程属性.BackColor = Color.White;
                toolStripItem_流程属性.Click += toolStripItem_流程属性_Click;
            }
        }

        internal void LoopRun(bool StartRun)
        {
            try
            {

                if (Frm_Job.Instance.tbc_jobs.TabPages.Count == 0)
                {
                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "No jobs to run" : "没有可运行的流程", Color.Green);
                    return;
                }
                if (Frm_Job.Instance.tbc_jobs.SelectedTab.Text == jobName)
                {
                    Frm_Job.Instance.btn_runLoop.Enabled = false;
                    Frm_Job.Instance.btn_runOnce.Enabled = false;
                    Application.DoEvents();
                }
                Thread.Sleep(50);
                if (StartRun)
                {
                    lock (this)
                    {
                        if (isRunOnceBusy || activeRunCount > 0 || (loopRunThread != null && loopRunThread.IsAlive))
                        {
                            Frm_Main.Instance.OutputMsg(string.Format("流程 [{0}] 正在运行，请稍候", jobName), Color.DarkOrange);
                            Frm_Job.Instance.btn_runLoop.Enabled = true;
                            Frm_Job.Instance.btn_runOnce.Enabled = true;
                            return;
                        }

                        stopRequested = false;
                        isRunLoop = true;
                    }
                    //if (Machine .machineRunStatu !=MachineRunStatu .Running )
                    //Frm_Job.Instance.btn_runOnce.BackgroundImage = Resources.ButtonDown;
                    Application.DoEvents();
                    Thread th_runJob = new Thread(() =>
                    {
                        try
                        {
                            while (Job.FindJobByName(jobName.ToString()).isRunLoop)
                            {
                                Job.FindJobByName(jobName.ToString()).Run();

                                //流程失败停止循环
                                if (Project.Instance.configuration.failStop)
                                {
                                    if (Job.FindJobByName(jobName.ToString()).jobRunStatu != JobRunStatu.Succeed)
                                    {
                                        isRunLoop = false;
                                        break;
                                    }
                                }

                                //文件夹图像执行一遍后停止循环
                                if (Project.Instance.configuration.endStop)
                                {
                                    for (int i = 0; i < Job.FindJobByName(jobName.ToString()).L_toolList.Count; i++)
                                    {
                                        if (Job.FindJobByName(jobName.ToString()).L_toolList[i].toolType == ToolType.ImageAcq)
                                        {
                                            if (((AcqImageTool)Job.FindJobByName(jobName.ToString()).L_toolList[i].tool).imageSourceMode == ImageSourceMode.FromDirectory)
                                            {
                                                if (((AcqImageTool)Job.FindJobByName(jobName.ToString()).L_toolList[i].tool).currentImageIndex == ((AcqImageTool)Job.FindJobByName(jobName.ToString()).L_toolList[i].tool).L_images.Count - 1)
                                                {
                                                    isRunLoop = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (isRunLoop)
                                    Thread.Sleep(Convert.ToInt16(Project.Instance.configuration.timeBetweenJobRun));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.SaveError(ex);
                        }
                        finally
                        {
                            lock (this)
                            {
                                isRunLoop = false;
                                loopRunThread = null;
                            }
                            SetRunButtonEnabled(true);
                            SetLoopRunStoppedUi();
                        }
                    });
                    th_runJob.IsBackground = true;
                    lock (this)
                    {
                        loopRunThread = th_runJob;
                    }
                    th_runJob.Start();
                    if (Machine.machineRunStatu != MachineRunStatu.Running)
                    {
                        Frm_Job.Instance.btn_runLoop.Text = Project.Instance.configuration.language == Language.English ? "Run Loop" : "停止运行";
                        Frm_Main.Instance.toolStripButton12.Text = "停止运行";
                    }
                    Thread.Sleep(100);
                    //Frm_Job.Instance.btn_runLoop.BackgroundImage = Resources.ButtonUp;
                }
                else
                {
                    //Frm_Job.Instance.btn_runOnce.BackgroundImage = Resources.ButtonUp;
                    Application.DoEvents();

                    RequestStop();
                    isRunLoop = false;
                    Thread.Sleep(20);
                    bool loopIsStopping = loopRunThread != null && loopRunThread.IsAlive;
                    Frm_Job.Instance.btn_runLoop.Text = loopIsStopping
                        ? (Project.Instance.configuration.language == Language.English ? "Stopping..." : "正在停止...")
                        : (Project.Instance.configuration.language == Language.English ? "Run Loop" : "连续运行");
                    Frm_Main.Instance.toolStripButton11.Enabled = true;
                    Frm_Main.Instance.toolStripButton12.Text = loopIsStopping ? "正在停止..." : "连续运行";
                    Frm_Job.Instance.btn_runOnce.Enabled = !loopIsStopping;
                }
                Frm_Job.Instance.btn_runLoop.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        public HTuple www;


        //public static HTuple  FindJobWindow(string jobName)
        //{
        //    try
        //    {
        //        if (!D_jobAndWindow.ContainsKey(FindJobByName(jobName)))
        //        {
        //          HTuple dd;
        //            HOperatorSet.OpenWindow (0,0,1000,1000,0,"visible","",out dd);
        //            D_jobAndWindow.Add(FindJobByName(jobName), dd);
        //        }
        //        return D_jobAndWindow[FindJobByName(jobName)];
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.SaveError(ex);
        //        return null;
        //    }
        //}
        ///当前流程是否处在连续运行状态
        internal volatile bool isRunLoop = false;
        /// <summary>
        /// 当前流程树是否处于折叠状态
        /// </summary>
        private static bool jobTreeFold = true;
        /// <summary>
        /// 当前流程此次运行结果
        /// </summary>
        public JobRunStatu jobRunStatu = JobRunStatu.Fail;
        /// <summary>
        /// 单次运行忙碌标记，防止多个按钮同时重入同一条流程。
        /// </summary>
        [NonSerialized]
        private bool isRunOnceBusy = false;
        /// <summary>
        /// 连续运行线程。停止连续运行后，当前一轮 Run() 可能仍未结束，
        /// 在该线程真正退出前流程仍应视为忙碌。
        /// </summary>
        [NonSerialized]
        private Thread loopRunThread;
        [NonSerialized]
        private int activeRunCount;
        /// <summary>
        /// 协作式停止请求。停止不会强杀工作线程，避免 HALCON/相机/通讯 SDK 在非安全点被中断而崩溃。
        /// </summary>
        [NonSerialized]
        private volatile bool stopRequested;

        internal bool IsExecutionActive
        {
            get
            {
                lock (this)
                {
                    return isRunOnceBusy || isRunLoop || activeRunCount > 0 || (loopRunThread != null && loopRunThread.IsAlive);
                }
            }
        }
        /// <summary>
        /// 工具输入项个数
        /// </summary>
        private int inputItemNum = 0;
        /// <summary>
        /// 工具输出项个数
        /// </summary>
        private int outputItemNum = 0;
        /// <summary>
        /// 指示图像窗口是否为第一次显示窗体，第一次显示时要初始化
        /// </summary>
        internal bool firstDisplayImage = true;
        /// <summary>
        /// 需要连线的节点对，不停的画连线，注意键值对中第一个为连线的结束节点，第二个为起始节点，一个输出可能连接多个输入，而键值对中的键不能重复，所以把源作为值，输入作为键
        /// </summary>
        internal Dictionary<TreeNode, TreeNode> D_itemAndSource = new Dictionary<TreeNode, TreeNode>();
        /// <summary>
        /// 本流程所绑定的生产窗口的句柄
        /// </summary>
        internal HTuple productWindow = new HTuple();
        /// <summary>
        /// 本流程所绑定的生产窗口的名称
        /// </summary>
        internal string imageWindowName = "无";
        /// <summary>
        /// 流程结果图像所绑定的窗体
        /// </summary>
        internal string debugImageWindow = (Project.Instance.configuration.language == Language.English ? "Image" : "图像");
        /// <summary>
        /// 编辑节点前节点文本，用于修改工具名称
        /// </summary>
        private string nodeTextBeforeEdit = string.Empty;
        /// <summary>
        /// 流程编辑时的右击菜单
        /// </summary>
        internal static ContextMenuStrip rightClickMenu = new ContextMenuStrip();
        /// <summary>
        /// 在空白除右击菜单
        /// </summary>
        private static ContextMenuStrip rightClickMenuAtBlank = new ContextMenuStrip();
        /// <summary>
        /// 流程名
        /// </summary>
        internal string jobName = string.Empty;
        internal JobRunMode jobRunMode = JobRunMode.RunAfterCall;
        /// <summary>
        /// 工具对象集合
        /// </summary>
        internal List<ToolInfo> L_toolList = new List<ToolInfo>();
        /// <summary>
        /// 记录本工具执行完的耗时，用于计算各工具耗时
        /// </summary>
        private double recordElapseTime = 0;
        /// <summary>
        /// 标准图像字典，用于存储标准图像路径和图像对象
        /// </summary>
        internal static Dictionary<string, HObject> D_standardImage = new Dictionary<string, HObject>();
        /// <summary>
        /// 工具图标列表
        /// </summary>
        internal static ImageList imageList = new ImageList();
        /// <summary>
        /// 工具编号，此参数的意义在于当流程为机械手定位类流程时，如机械手上安装了多个工具（如吸嘴或夹爪）时，要通过此参数指定工具编号
        /// </summary>
        internal int toolIdx = 1;


        /// <summary>
        /// 初始化图标集合
        /// </summary>
        internal static void InitImageList()
        {
            try
            {
                //工具图标
                imageList.Images.Add(Resources.ToolBox);
                imageList.Images.Add(Resources.ImageAcqTool);
                imageList.Images.Add(Resources.ColorToRGBTool);
                imageList.Images.Add(Resources.Empty);
                imageList.Images.Add(Resources.EyeHandCalibTool);
                imageList.Images.Add(Resources.DownCamAlignTool);
                imageList.Images.Add(Resources.FindLineTool);
                imageList.Images.Add(Resources.FindCircleTool);
                imageList.Images.Add(Resources.FitLineTool);
                imageList.Images.Add(Resources.FitCircleTool);
                imageList.Images.Add(Resources.BlobAnalyseTool);          //10
                imageList.Images.Add(Resources.SubImageTool);
                imageList.Images.Add(Resources.CreateROITool);
                imageList.Images.Add(Resources.CreatePosTool);
                imageList.Images.Add(Resources.RegionArrayTool);
                imageList.Images.Add(Resources.MarkTool);
                imageList.Images.Add(Resources.DistancePPTool);
                imageList.Images.Add(Resources.DistancePLTool);
                imageList.Images.Add(Resources.AngleLLTool);
                imageList.Images.Add(Resources.DistanceLLTool);
                imageList.Images.Add(Resources.TwoPointCenterTool);       //20
                imageList.Images.Add(Resources.LLIntersectionTool);
                imageList.Images.Add(Resources.RegionFeatureTool);
                imageList.Images.Add(Resources.字符);
                imageList.Images.Add(Resources.BarCodeTool);
                imageList.Images.Add(Resources.QRTool);
                imageList.Images.Add(Resources.CSharpScriptTool);
                imageList.Images.Add(Resources.LightTool);
                imageList.Images.Add(Resources.Empty);
                imageList.Images.Add(Resources.LabelTool);
                imageList.Images.Add(Resources.OutputTool);               //30

                //非工具图标  
                imageList.Images.Add(Resources.Image);
                imageList.Images.Add(Resources.MatchTool);
                imageList.Images.Add(Resources.UnknownTool);
                imageList.Images.Add(Resources.Empty);
                imageList.Images.Add(Resources.Empty);
                imageList.Images.Add(Resources.Robot);
                imageList.Images.Add(Resources.LineCalib);
                imageList.Images.Add(Resources.ImageProprocessingTool);
                imageList.Images.Add(Resources.SaveImageTool);
                imageList.Images.Add(Resources.DimensionCalibTool);     //40
                imageList.Images.Add(Resources.LineCalib);
                imageList.Images.Add(Resources.WhileTool);
                imageList.Images.Add(Resources.CreateSegmentTool);
                imageList.Images.Add(Resources.RegionArrayTool);
                imageList.Images.Add(Resources.Rotate);
                imageList.Images.Add(Resources.XY);
                imageList.Images.Add(Resources.偏移);
                imageList.Images.Add(Resources.OCRTool);
                imageList.Images.Add(Resources.顶部对齐);
                imageList.Images.Add(Resources.底部对齐);                //50
                imageList.Images.Add(Resources.组合表格);
                imageList.Images.Add(Resources.循环);
                imageList.Images.Add(Resources.分支);
                imageList.Images.Add(Resources.扫码枪__1_);
                imageList.Images.Add(Resources.MatchTool);
                imageList.Images.Add(Resources.发送区);
                imageList.Images.Add(Resources.接收区);
                imageList.Images.Add(Resources.阵列);
                imageList.Images.Add(Resources.输出管理);
                imageList.Images.Add(Resources.大屏展示);                //60
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 获取当前流程所对应的流程树对象
        /// </summary>
        /// <param name="jobName">流程名</param>
        /// <returns>流程树控件对象</returns>
        internal static TreeView GetJobTree(string jobName)
        {
            try
            {
                for (int i = 0; i < Frm_Job.Instance.tbc_jobs.TabCount; i++)
                {
                    if (Frm_Job.Instance.tbc_jobs.TabPages[i].Text == jobName)
                    {
                        return (TreeView)(Frm_Job.Instance.tbc_jobs.TabPages[i].Controls[0]);
                    }
                }
                return new TreeView();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new TreeView();
            }
        }
        /// <summary>
        /// 获取当前流程所对应的流程树对象
        /// </summary>
        /// <param name="jobName">流程名</param>
        /// <returns>流程树控件对象</returns>
        internal TreeView GetJobTree()
        {
            try
            {
                for (int i = 0; i < Frm_Job.Instance.tbc_jobs.TabCount; i++)
                {
                    if (Frm_Job.Instance.tbc_jobs.TabPages[i].Text == jobName)
                    {
                        return (TreeView)(Frm_Job.Instance.tbc_jobs.TabPages[i].Controls[0]);
                    }
                }
                return new TreeView();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new TreeView();
            }
        }
        /// <summary>
        /// 更新工具树图标
        /// </summary>
        /// <param name="jobName">流程名</param>
        internal static void UpdateJobTreeIcon(string jobName)
        {
            try
            {
                //更新工具树图标
                for (int j = 0; j < GetJobTree(jobName).Nodes.Count; j++)
                {
                    switch (Job.FindJobByName(jobName).FindToolInfoByName(GetJobTree(jobName).Nodes[j].Text).toolType)
                    {
                        case ToolType.SDK_Halcon:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 1;
                            break;

                        case ToolType.ImageAcq:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 1;
                            break;

                        case ToolType.ColorToRGB:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 2;
                            break;

                        case ToolType.SaveImage:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 39;
                            break;

                        case ToolType.Match:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 55;
                            break;

                        case ToolType.EyeHandCalib:
                        case ToolType.OneKeyEyeHandCalib:
                        case ToolType.QuoteTrans:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 58;
                            break;

                        case ToolType.OneDimensionalCalib:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 58;
                            break;

                        case ToolType.UpCamAlign:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 49;
                            break;

                        case ToolType.DownCamAlign:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 5;
                            break;

                        case ToolType.FindLine:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 6;
                            break;

                        case ToolType.FindCircle:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 7;
                            break;

                        case ToolType.BlobAnalyse:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 10;
                            break;

                        case ToolType.SubImage:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 11;
                            break;

                        case ToolType.CreateROI:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 12;
                            break;

                        case ToolType.CreatePosition:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 13;
                            break;

                        case ToolType.CreateLine:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 43;
                            break;

                        case ToolType.ArrayRegion:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 14;
                            break;

                        case ToolType.Mark:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 15;
                            break;

                        case ToolType.DistancePP:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 16;
                            break;

                        case ToolType.DistancePL:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 17;
                            break;

                        case ToolType.DistanceSS:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 19;
                            break;

                        case ToolType.LLIntersect:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 21;
                            break;

                        case ToolType.AngleLL:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 18;
                            break;

                        case ToolType.CenterOfPP:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 20;
                            break;

                        case ToolType.RegionFeature:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 22;
                            break;

                        case ToolType.OCR:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 23;
                            break;

                        case ToolType.Barcode:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 24;
                            break;

                        case ToolType.QRCode:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 25;
                            break;

                        case ToolType.CodeEdit:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 26;
                            break;

                        case ToolType.Light_OPT:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 27;
                            break;

                        case ToolType.OPTLightControl:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 27;
                            break;

                        case ToolType.Scaner_Kenyence:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 28;
                            break;

                        case ToolType.batteryFirstAlign:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 35;
                            break;

                        case ToolType.Label:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 60;
                            break;

                        case ToolType.Output:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 59;
                            break;

                        case ToolType.RotatePlatform:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 45;
                            break;

                        case ToolType.XYPlatform:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 46;
                            break;

                        case ToolType.PointOffset:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 47;
                            break;

                        case ToolType.EthernetSend:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 56;
                            break;

                        case ToolType.EthernetReceive:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 57;
                            break;

                        case ToolType.PLCComm:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 33;
                            break;

                        default:
                            GetJobTree(jobName).Nodes[j].ImageIndex = GetJobTree(jobName).Nodes[j].SelectedImageIndex = 33;
                            break;

                    }
                    for (int k = 0; k < GetJobTree(jobName).Nodes[j].Nodes.Count; k++)
                    {
                        GetJobTree(jobName).Nodes[j].Nodes[k].ImageIndex = GetJobTree(jobName).Nodes[j].Nodes[k].SelectedImageIndex = 34;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 通过流程名获取窗体
        /// </summary>
        /// <param name="jobName">流程名</param>
        /// <returns></returns>
        internal Frm_ImageWindow GetImageWindowControl()
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
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 判断流程是否已经存在输出工具，一个流程只能含有一个输出工具
        /// </summary>
        /// <returns></returns>
        internal bool ExistOutputTool()
        {
            try
            {
                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolType == ToolType.Output)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 判断TreeView是否已经包含某节点
        /// </summary>
        /// <param name="key">节点文本</param>
        /// <returns>是否包含</returns>
        private bool CheckTreeViewContainsKey(string key)
        {
            try
            {
                foreach (TreeNode node in Job.GetJobTree(jobName).Nodes)
                {
                    if (node.Text == key)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 放弃重命名
        /// </summary>
        private void GiveupRename(object obj)
        {
            try
            {
                Thread.Sleep(20);
                Job.GetJobTree(jobName).SelectedNode.Text = nodeTextBeforeEdit;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 通过工具名获取工具信息
        /// </summary>
        /// <param name="toolName">工具名</param>
        /// <returns>工具信息</returns>
        internal ToolInfo FindToolInfoByName(string toolName)
        {
            try
            {
                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolName == toolName)
                    {
                        return L_toolList[i];
                    }
                }
                return new ToolInfo();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new ToolInfo();
            }
        }
        /// <summary>
        /// 通过输出项字符串获取输出项的值
        /// </summary>
        /// <param name="outputItem">输出项字符串</param>
        /// <returns>输出项的值</returns>
        public object GetOutputItemValue(string outputItem)
        {
            try
            {
                //寻找输出工具
                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolType == ToolType.Output)
                    {
                        for (int j = 0; j < L_toolList[i].input.Count; j++)
                        {
                            if (L_toolList[i].input[j].IOName == outputItem)
                            {
                                return L_toolList[i].GetInput(outputItem).value;
                            }
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取指定工具输出项的值
        /// </summary>
        /// <param name="outputItem">输出项字符串</param>
        /// <returns>输出项的值</returns>
        public object GetToolOutputItemValue(string outputItem)
        {
            try
            {
                string toolName = Regex.Split(outputItem, " . -->")[0];
                string itemName = Regex.Split(outputItem, " . -->")[1];
                //寻找输出工具
                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolName == toolName)
                    {
                        for (int j = 0; j < L_toolList[i].output.Count; j++)
                        {
                            if (L_toolList[i].output[j].IOName == itemName)
                            {
                                return L_toolList[i].GetOutput(itemName).value;
                            }
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取工具输入项的个数
        /// </summary>
        private int GetInputItemNum(TreeNode toolNode)
        {
            try
            {
                int num = 0;
                foreach (TreeNode item in toolNode.Nodes)
                {
                    if (item.Text.Substring(0, 3) == "<--")
                    {
                        num++;
                    }
                }
                return num;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return 0;
            }
        }
        /// <summary>
        /// 修改工具说明                
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyTipInfo(object sender, EventArgs e)
        {
            try
            {
                Frm_InputMessage frm_inputMessage = new Frm_InputMessage();
                frm_inputMessage.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "Please input name of standard image" : "请输入工具说明信息");
                frm_inputMessage.btn_confirm.Text = (Project.Instance.configuration.language == Language.English ? "OK" : "确定");
                frm_inputMessage.txt_input.Text = FindToolInfoByName(GetJobTree().SelectedNode.Text).toolTipInfo;
                frm_inputMessage.TopMost = true;
                frm_inputMessage.ShowDialog();
                if (Frm_InputMessage.input != string.Empty)
                {
                    FindToolInfoByName(GetJobTree().SelectedNode.Text).toolTipInfo = Frm_InputMessage.input;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 把节点文本添加到剪切板，用于复制粘贴输出项文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyNodeText(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(GetJobTree().SelectedNode.Text);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 生成XYU集合的提示信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string FormatShowTip(List<XYU> list)
        {
            try
            {
                string result = string.Empty;
                for (int i = 0; i < list.Count; i++)
                {
                    result += string.Format("{0}  {1} | {2} | {3}\r\n", (i + 1), list[i].Point.X.ToString("000.000"), list[i].Point.Y.ToString("000.000"), list[i].U.ToString("000.000"));
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 生成Point集合的提示信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string FormatShowTip(object value)
        {
            try
            {
                if (value == null)
                    return "空";

                string temp = value.ToString();
                string result = string.Empty;
                switch (temp)
                {
                    case "VMPro.XYU":
                        XYU xyu = value as XYU;
                        result = string.Format("{0} {1} {2}", xyu.Point.X, xyu.Point.Y, xyu.U);
                        return result;

                    case "HObject":
                        return string.Empty;
                    case "VMPro.XY":
                        XY point = value as XY;

                        result = string.Format("{0}  {1}", point.X.ToString("0000.000"), point.Y.ToString("0000.000"));
                        return result;
                    case "Double":
                        break;
                    case "HalconDotNet.HObject":
                        result = "图形变量暂不支持显示";
                        return result;
                    case "System.Collections.Generic.List`1[VMPro.XY]":
                        List<XY> L_point = value as List<XY>;

                        for (int i = 0; i < L_point.Count; i++)
                        {
                            result += string.Format("{0} |  {1}  {2}\r\n", (i + 1), L_point[i].X.ToString("0000.000"), L_point[i].Y.ToString("0000.000"));
                        }
                        return result;
                    case "System.Collections.Generic.List`1[VMPro.XYU]":
                        List<XYU> L_xyu = value as List<XYU>;

                        for (int i = 0; i < L_xyu.Count; i++)
                        {
                            result += string.Format("{0} |  {1}  {2}  {3}\r\n", (i + 1), L_xyu[i].Point.X.ToString("0000.000"), L_xyu[i].Point.Y.ToString("0000.000"), L_xyu[i].U.ToString("0000.000"));
                        }
                        return result;
                    case "VMPro.Line":
                        Line line = value as Line;
                        result = string.Format("({0},{1}) | ({2},{3})", line.起点.X, line.起点.Y, line.终点.X, line.终点.Y);
                        return result;
                    default:
                        result = value.ToString();
                        return result;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }


        #region 绘制节点连线

        /// <summary>
        /// Graphics对象
        /// </summary>
        private static Graphics graphics;
        /// <summary>
        /// 正在绘制输入输出指向线
        /// </summary>
        internal static bool isDrawing = false;
        /// <summary>
        /// 流程树中节点的最大长度
        /// </summary>
        private int maxLength = 130;
        /// <summary>
        /// 记录起始节点和此节点的列坐标值
        /// </summary>
        private static Dictionary<TreeNode, Color> startNodeAndColor = new Dictionary<TreeNode, Color>();
        /// <summary>
        /// 记录前面的划线所跨越的列段，
        /// </summary>
        private static Dictionary<int, Dictionary<TreeNode, TreeNode>> list = new Dictionary<int, Dictionary<TreeNode, TreeNode>>();
        /// <summary>
        /// 每一个列坐标值对应一种颜色
        /// </summary>
        private Dictionary<int, Color> colValueAndColor = new Dictionary<int, Color>();
        /// <summary>
        /// 输入输出指向线的颜色数组
        /// </summary>
        private static Color[] color = new Color[] { Color.Blue, Color.Orange, Color.Black, Color.Red, Color.Green, Color.Brown, Color.Blue, Color.Black, Color.Red, Color.Green, Color.Orange, Color.Brown, Color.Blue, Color.Black, Color.Red, Color.Green, Color.Orange, Color.Brown, Color.Blue, Color.Black, Color.Red, Color.Green, Color.Orange, Color.Brown, Color.Blue, Color.Black, Color.Red, Color.Green, Color.Orange, Color.Brown };


        /// <summary>
        /// 绘制输入输出指向线
        /// </summary>
        /// <param name="obj"></param>
        internal void DrawLine()
        {
            try
            {
                if (Project.Instance.configuration.displayLine && !isDrawing && !Configuration.SpeedMode)
                {
                    isDrawing = true;
                    Thread th = new Thread(() =>
                    {
                        Job.GetJobTree(jobName).MouseWheel += new MouseEventHandler(numericUpDown1_MouseWheel);          //划线的时候不能滚动，否则画好了线，结果已经滚到其它地方了
                        maxLength = 150;
                        colValueAndColor.Clear();
                        startNodeAndColor.Clear();
                        list.Clear();
                        TreeView tree = GetJobTree(jobName);
                        graphics = tree.CreateGraphics();
                        tree.CreateGraphics().Dispose();

                        foreach (KeyValuePair<TreeNode, TreeNode> item in D_itemAndSource)
                        {
                            CreateLine(tree, item.Key, item.Value);
                        }
                        Application.DoEvents();
                        Job.GetJobTree(jobName).MouseWheel -= new MouseEventHandler(numericUpDown1_MouseWheel);
                        isDrawing = false;

                    });
                    th.IsBackground = true;
                    th.ApartmentState = ApartmentState.STA;             //此处要加一行，否则画线时会报错
                    th.Start();
                }
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 画Treeview控件两个节点之间的连线
        /// </summary>
        /// <param name="treeview">要画连线的Treeview</param>
        /// <param name="startNode">结束节点</param>
        /// <param name="endNode">开始节点</param>
        private void CreateLine(TreeView treeview, TreeNode endNode, TreeNode startNode)
        {
            try
            {
                //得到起始与结束节点之间所有节点的最大长度  ，保证画线不穿过节点
                int startNodeParantIndex = startNode.Parent.Index;
                int endNodeParantIndex = endNode.Parent.Index;
                int startNodeIndex = startNode.Index;
                int endNodeIndex = endNode.Index;
                int max = 0;

                if (!startNode.Parent.IsExpanded)
                {
                    max = startNode.Parent.Bounds.X + startNode.Parent.Bounds.Width;
                }
                else
                {
                    for (int i = startNodeIndex; i < startNode.Parent.Nodes.Count - 1; i++)
                    {
                        if (max < treeview.Nodes[startNodeParantIndex].Nodes[i].Bounds.X + treeview.Nodes[startNodeParantIndex].Nodes[i].Bounds.Width)
                            max = treeview.Nodes[startNodeParantIndex].Nodes[i].Bounds.X + treeview.Nodes[startNodeParantIndex].Nodes[i].Bounds.Width;
                    }
                }
                for (int i = startNodeParantIndex + 1; i < endNodeParantIndex; i++)
                {
                    if (!treeview.Nodes[i].IsExpanded)
                    {
                        if (max < treeview.Nodes[i].Bounds.X + treeview.Nodes[i].Bounds.Width)
                            max = treeview.Nodes[i].Bounds.X + treeview.Nodes[i].Bounds.Width;
                    }
                    else
                    {
                        for (int j = 0; j < treeview.Nodes[i].Nodes.Count; j++)
                        {
                            if (max < treeview.Nodes[i].Nodes[j].Bounds.X + treeview.Nodes[i].Nodes[j].Bounds.Width)
                                max = treeview.Nodes[i].Nodes[j].Bounds.X + treeview.Nodes[i].Nodes[j].Bounds.Width;
                        }
                    }
                }
                if (!endNode.Parent.IsExpanded)
                {
                    if (max < endNode.Parent.Bounds.X + endNode.Parent.Bounds.Width)
                        max = endNode.Parent.Bounds.X + endNode.Parent.Bounds.Width;
                }
                else
                {
                    for (int i = 0; i < endNode.Index; i++)
                    {
                        if (max < treeview.Nodes[endNodeParantIndex].Nodes[i].Bounds.X + treeview.Nodes[endNodeParantIndex].Nodes[i].Bounds.Width)
                            max = treeview.Nodes[endNodeParantIndex].Nodes[i].Bounds.X + treeview.Nodes[endNodeParantIndex].Nodes[i].Bounds.Width;
                    }
                }
                max += 20;        //箭头不能连着节点，

                if (!startNode.Parent.IsExpanded)
                    startNode = startNode.Parent;
                if (!endNode.Parent.IsExpanded)
                    endNode = endNode.Parent;

                if (endNode.Bounds.X + endNode.Bounds.Width + 20 > max)
                    max = endNode.Bounds.X + endNode.Bounds.Width + 20;
                if (startNode.Bounds.X + startNode.Bounds.Width + 20 > max)
                    max = startNode.Bounds.X + startNode.Bounds.Width + 20;

                //判断是否可以在当前处划线
                foreach (KeyValuePair<int, Dictionary<TreeNode, TreeNode>> item in list)
                {
                    if (Math.Abs(max - item.Key) < 10)
                    {
                        foreach (KeyValuePair<TreeNode, TreeNode> item1 in item.Value)
                        {
                            if (startNode != item1.Value)
                            {
                                if ((item1.Value.Bounds.X < maxLength && item1.Key.Bounds.X < maxLength) || (item1.Value.Bounds.X < maxLength && item1.Key.Bounds.X < maxLength))
                                {
                                    if (item1.Value.Bounds.Y > startNode.Bounds.Y || item1.Key.Bounds.Y > startNode.Bounds.Y)    //20200612加
                                        max += (10 - Math.Abs(max - item.Key));
                                }
                            }
                        }
                    }
                }

                Dictionary<TreeNode, TreeNode> temp = new Dictionary<TreeNode, TreeNode>();
                temp.Add(endNode, startNode);
                if (!list.ContainsKey(max))
                    list.Add(max, temp);
                else
                    list[max].Add(endNode, startNode);

                if (!startNodeAndColor.ContainsKey(startNode))
                    startNodeAndColor.Add(startNode, color[startNodeAndColor.Count]);

                Pen pen = new Pen(startNodeAndColor[startNode], 1);
                Brush brush = new SolidBrush(startNodeAndColor[startNode]);

                graphics.DrawLine(pen, startNode.Bounds.X + startNode.Bounds.Width,
                    startNode.Bounds.Y + startNode.Bounds.Height / 2,
                max,
                  startNode.Bounds.Y + startNode.Bounds.Height / 2);
                graphics.DrawLine(pen, max,
                   startNode.Bounds.Y + startNode.Bounds.Height / 2,
                   max,
                  endNode.Bounds.Y + endNode.Bounds.Height / 2);
                graphics.DrawLine(pen, max,
                   endNode.Bounds.Y + endNode.Bounds.Height / 2,
                   endNode.Bounds.X + endNode.Bounds.Width,
                     endNode.Bounds.Y + endNode.Bounds.Height / 2);
                graphics.DrawString("<", new Font("微软雅黑", 12F), brush, endNode.Bounds.X + endNode.Bounds.Width - 5,
                     endNode.Bounds.Y + endNode.Bounds.Height / 2 - 12);
                Application.DoEvents();
            }
            catch { }
        }
        private void Instance_Paint(object sender, PaintEventArgs e)
        {
            DrawLineWithoutRefresh(null, null);
        }
        /// <summary>
        /// 取消滚轮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                HandledMouseEventArgs h = e as HandledMouseEventArgs;
                if (h != null)
                {
                    h.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal void tvw_job_AfterSelect(object sender, TreeViewEventArgs e)
        {
            nodeTextBeforeEdit = Job.GetJobTree(jobName).SelectedNode.Text;
            Job.GetJobTree(jobName).Update();
            DrawLine();
        }
        internal void Draw_Line(object sender, TreeViewEventArgs e)
        {
            Job.GetJobTree(jobName).Refresh();
            DrawLine();
        }
        internal void tbc_jobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Job.GetJobTree(jobName).Refresh();
            DrawLine();
        }
        internal void DrawLineWithoutRefresh(object sender, MouseEventArgs e)
        {
            Job.GetJobTree(jobName).Update();
            DrawLine();
        }
        /// <summary>
        /// 生成新工具的名称
        /// </summary>
        /// <param name="toolName">工具类型</param>
        /// <returns>工具名称</returns>
        internal string GetNewToolName(string toolType)
        {
            try
            {
                if (!CheckTreeViewContainsKey(toolType))
                {
                    return toolType;
                }
                for (int i = 1; i < 101; i++)
                {
                    if (!CheckTreeViewContainsKey(toolType + "_" + i))
                    {
                        return toolType + "_" + i;
                    }
                }
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "This tool has reached the maximum number of additions and cannot continue to be added (error code: 0002)" : "\r\n此工具已添加个数已达到数量上限，无法继续添加", TipType.Error);
                return "TooMuch";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return "Error";
            }
        }

        #endregion

        #region 空白处右击菜单

        void toolStripItem_展开流程树_Click(object sender, EventArgs e)
        {
            try
            {
                if (Frm_Job.Instance.tbc_jobs.TabCount < 1)
                    return;
                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                Job job = Job.FindJobByName(jobName);
                Job.GetJobTree(jobName).ExpandAll();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        void toolStripItem_折叠流程树_Click(object sender, EventArgs e)
        {
            try
            {
                if (Frm_Job.Instance.tbc_jobs.TabCount < 1)
                    return;
                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                Job job = Job.FindJobByName(jobName);
                Job.GetJobTree(jobName).CollapseAll();
                job.DrawLine();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        void toolStripItem_启用全部_Click(object sender, EventArgs e)
        {
            try
            {
                List<ToolInfo> toolList = FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).L_toolList;
                for (int i = 0; i < toolList.Count; i++)
                {
                    toolList[i].enable = true;
                }

                foreach (TreeNode item in GetJobTree(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).Nodes)
                {
                    item.ForeColor = Color.Black;
                }
                GetJobTree().SelectedNode = null;
                Frm_Output.Instance.OutputMsg("已启用当前流程中的所有工具", Color.Black);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        void toolStripItem_忽略全部_Click(object sender, EventArgs e)
        {
            try
            {
                List<ToolInfo> toolList = FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).L_toolList;
                for (int i = 0; i < toolList.Count; i++)
                {
                    toolList[i].enable = false;
                }

                foreach (TreeNode item in GetJobTree(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).Nodes)
                {
                    item.ForeColor = Color.DarkGray;
                }
                GetJobTree().SelectedNode = null;
                Frm_Output.Instance.OutputMsg("已禁用当前流程中的所有工具", Color.Black);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        void toolStripItem_删除流程_Click(object sender, EventArgs e)
        {
            DeleteJob();
        }
        void toolStripItem_流程属性_Click(object sender, EventArgs e)
        {
            Frm_Job.Instance.tsb_jobInfo_Click(null, null);
        }

        #endregion

        #region 流程操作

        /// <summary>
        /// 兼容旧版流程：统一所有跟随端口与模板匹配位置输出的位姿类型。
        /// </summary>
        internal void EnsureBlobFollowInput()
        {
            for (int i = 0; i < L_toolList.Count; i++)
            {
                ToolInfo toolInfo = L_toolList[i];
                if (toolInfo.toolType == ToolType.Match)
                {
                    for (int j = 0; j < toolInfo.output.Count; j++)
                    {
                        if (toolInfo.output[j].IOName == "位置" || toolInfo.output[j].IOName == "Position")
                            toolInfo.output[j].ioType = DataType.Pose;
                    }
                }

                if (toolInfo.toolType != ToolType.BlobAnalyse &&
                    toolInfo.toolType != ToolType.FindLine &&
                    toolInfo.toolType != ToolType.FindCircle)
                    continue;

                bool hasFollowInput = false;
                for (int j = 0; j < toolInfo.input.Count; j++)
                {
                    if (toolInfo.input[j].IOName == "跟随" || toolInfo.input[j].IOName == "Pose")
                    {
                        hasFollowInput = true;
                        toolInfo.input[j].ioType = DataType.Pose;
                        break;
                    }
                }

                if (!hasFollowInput)
                {
                    string inputName = Project.Instance.configuration.language == Language.English ? "Pose" : "跟随";
                    toolInfo.input.Add(new ToolIO(inputName, "", DataType.Pose));
                }
            }
        }

        internal bool IsStopRequested
        {
            get { return stopRequested; }
        }

        private void RequestStop()
        {
            stopRequested = true;
            // PLC 等待期望值是流程中的可无限等待项；收到停止请求后主动唤醒。
            for (int i = 0; i < L_toolList.Count; i++)
            {
                if (L_toolList[i].toolType == ToolType.PLCComm)
                    ((PLCCommTool)L_toolList[i].tool).quitWait = true;
            }
        }

        /// <summary>
        /// 添加新流程
        /// </summary>
        internal static void CreateJob()
        {
            try
            {
                Job.isDrawing = true;
                if (Project.Instance.L_engineList.Count == 0)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n当前项目未创建任何方案，请先创建方案");
                    Scheme.CreateScheme();
                    return;
                }

            Again:
                Frm_InputMessage.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "Please input job's name" : "请输入新流程名");
                Frm_InputMessage.Instance.btn_confirm.Text = (Project.Instance.configuration.language == Language.English ? "Confirm" : "确定");
                //Frm_InputMessage.Instance.passwordChar = false;
                Frm_InputMessage.Instance.txt_input.DefaultText = "请输入新流程名";
                Frm_InputMessage.Instance.txt_input.TextStr = string.Empty;
                //Frm_InputMessage.input = string.Empty;
                Frm_InputMessage.Instance.ShowDialog();
                string jobName = Frm_InputMessage.input;
                if (jobName == string.Empty)
                    return;

                //检查此名称的流程是否已存在
                if (CheckJobExist(jobName))
                {
                    Frm_MessageBox.Instance.MessageBoxShow((Project.Instance.configuration.language == Language.English ? "\r\nA process with this name already exists. The process name cannot be repeated. Please enter again" : "\r\n已存在此名称的流程，流程名不可重复，请重新输入"));
                    goto Again;
                }
                //检查此名称是否含有特殊字符\
                if (jobName.Contains(@"\"))
                {
                    Frm_MessageBox.Instance.MessageBoxShow((Project.Instance.configuration.language == Language.English ? "\r\nA process with this name already exists. The process name cannot be repeated. Please enter again" : "\r\n流程名中不能含有 \\ 等特殊字符 ，请重新输入"));
                    goto Again;
                }
                Frm_Output.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "A new process named:" + jobName : string.Format("创建了新流程，流程名为：{0}", jobName), Color.Black);

                Job job = new Job();
                HWindowControl dd = new HWindowControl();
                Frm_Main.d.Add(dd);
                job.www = dd.HalconID;
                job.jobName = jobName;
                Project.Instance.curEngine.L_jobList.Add(job);

                TreeView tvw_job = new TreeView();
                tvw_job.Scrollable = true;
                tvw_job.ItemHeight = 26;
                tvw_job.ShowLines = false;
                tvw_job.AllowDrop = true;
                tvw_job.ImageList = Job.imageList;

                tvw_job.AfterSelect += job.tvw_job_AfterSelect;
                tvw_job.AfterLabelEdit += new NodeLabelEditEventHandler(job.EditNodeText);
                tvw_job.MouseClick += new MouseEventHandler(job.TVW_MouseClick);
                tvw_job.MouseDoubleClick += new MouseEventHandler(job.TVW_DoubleClick);

                //节点间拖拽事件
                tvw_job.ItemDrag += new ItemDragEventHandler(job.tvw_job_ItemDrag);
                tvw_job.DragEnter += new DragEventHandler(job.tvw_job_DragEnter);
                tvw_job.DragDrop += new DragEventHandler(job.tvw_job_DragDrop);

                //以下事件为画线事件
                if (Project.Instance.configuration.displayLine)
                {
                    tvw_job.MouseMove += job.DrawLineWithoutRefresh;
                    tvw_job.AfterExpand += job.Draw_Line;
                    tvw_job.AfterCollapse += job.Draw_Line;
                    Frm_Job.Instance.tbc_jobs.SelectedIndexChanged += job.tbc_jobs_SelectedIndexChanged;
                }

                tvw_job.Dock = DockStyle.Fill;
                tvw_job.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                Frm_Job.Instance.tbc_jobs.TabPages.Add(jobName);
                Frm_Job.Instance.tbc_jobs.TabPages[Frm_Job.Instance.tbc_jobs.TabPages.Count - 1].Controls.Add(tvw_job);
                Frm_Job.Instance.tbc_jobs.SelectedIndex = Frm_Job.Instance.tbc_jobs.TabCount - 1;
                Application.DoEvents();

                //默认添加Halcon采集接口工具
                Frm_ToolBox.Instance.AddTool(Project.Instance.configuration.language == Language.English ? "HalconAcqInterface" : "采集图像", null);

                //默认选中第一个工具节点
                tvw_job.SelectedNode = tvw_job.Nodes[0];

                //展开已默认添加的工具的输入输出项
                tvw_job.ExpandAll();

                //添加此流程的系统变量
                Variable variable1 = new Variable(Project.Instance.curEngine.L_jobList.Count * 2, "String", string.Format("流程[{0}].运行状态", jobName));
                variable1.variableType = 0;
                Project.Instance.curEngine.globelVariable.L_variable.Add(variable1);
                Variable variable2 = new Variable(Project.Instance.curEngine.L_jobList.Count * 2, "Double", string.Format("流程[{0}].运行时间", jobName));
                variable2.variableType = 0;
                Project.Instance.curEngine.globelVariable.L_variable.Add(variable2);
                Job.isDrawing = false;

                ////自动创建窗体并绑定到此流程
                //Frm_Main.Instance.CreateNewImageWindowWithoutInput();
                ////需要重新保存一下布局
                //File.Delete(Application.StartupPath + "\\" + Project.Instance.configuration.layoutFilePath);
                //Frm_Main.Instance.dockPanel.SaveAsXml(Project.Instance.configuration.layoutFilePath);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 从本地加载流程到程序中
        /// </summary>
        /// <param name="path">流程文件路径</param>
        public static Job LoadJob(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Frm_Output.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "\r\nThe process file does not exist" : "\r\n流程文件不存在", Color.Red);
                    return null;
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                Job job = (Job)formatter.Deserialize(stream);
                stream.Close();
                job.EnsureBlobFollowInput();

                foreach (TabPage item in Frm_Job.Instance.tbc_jobs.TabPages)
                {
                    if (item.Text == job.jobName)
                    {
                        Frm_Output.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "\r\nA process with the same name already exists. Please do not add it again" : "\r\n已经存在与此流程名相同的流程，请勿重复添加", Color.Red);
                        return new Job();
                    }
                }
                job.isRunLoop = false;
                Project.Instance.curEngine.L_jobList.Add(job);

                TreeView tvw_job = new TreeView();
                tvw_job.Scrollable = true;
                tvw_job.ItemHeight = 26;
                tvw_job.ShowLines = false;
                tvw_job.AllowDrop = true;
                tvw_job.ImageList = Job.imageList;
                tvw_job.TabStop = false;
                tvw_job.ShowNodeToolTips = true;


                tvw_job.AfterSelect += job.tvw_job_AfterSelect;
                tvw_job.AfterLabelEdit += new NodeLabelEditEventHandler(job.EditNodeText);
                tvw_job.MouseClick += new MouseEventHandler(job.TVW_MouseClick);
                tvw_job.MouseDoubleClick += new MouseEventHandler(job.TVW_DoubleClick);
                tvw_job.AfterSelect += new TreeViewEventHandler(job.TVW_AfterSelect);

                //节点间拖拽
                tvw_job.ItemDrag += new ItemDragEventHandler(job.tvw_job_ItemDrag);
                tvw_job.DragEnter += new DragEventHandler(job.tvw_job_DragEnter);
                tvw_job.DragDrop += new DragEventHandler(job.tvw_job_DragDrop);
                tvw_job.MouseDown += new MouseEventHandler(job.tvw_tools_MouseDown);

                //以下事件为画线事件
                if (Project.Instance.configuration.displayLine)
                {
                    Frm_Job.Instance.Paint += job.Instance_Paint;
                    tvw_job.MouseMove += job.DrawLineWithoutRefresh;
                    tvw_job.MouseWheel += job.DrawLineWithoutRefresh;

                    tvw_job.AfterExpand += job.Draw_Line;
                    tvw_job.AfterCollapse += job.Draw_Line;
                    Frm_Job.Instance.tbc_jobs.SelectedIndexChanged += job.tbc_jobs_SelectedIndexChanged;
                }

                Frm_Job.Instance.tbc_jobs.TabPages.Add(job.jobName);
                Frm_Job.Instance.tbc_jobs.TabPages[Frm_Job.Instance.tbc_jobs.TabPages.Count - 1].Controls.Add(tvw_job);
                tvw_job.Dock = DockStyle.Fill;
                tvw_job.ShowNodeToolTips = true;
                tvw_job.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                //反序列化各工具
                job.D_itemAndSource.Clear();
                for (int i = 0; i < job.L_toolList.Count; i++)
                {
                    TreeNode node = GetJobTree(job.jobName).Nodes.Add(job.L_toolList[i].toolName);
                    for (int j = 0; j < job.L_toolList[i].input.Count; j++)
                    {
                        TreeNode treeNode;
                        //因为OutputBox只有源，所以此处特殊处理
                        if (job.L_toolList[i].toolType != ToolType.Output)
                            treeNode = node.Nodes.Add("<--" + job.L_toolList[i].input[j].IOName + job.L_toolList[i].input[j].value);
                        else
                            treeNode = node.Nodes.Add("<--" + job.L_toolList[i].input[j].IOName);

                        treeNode.Tag = job.L_toolList[i].input[j].ioType;
                        treeNode.ForeColor = Color.DarkMagenta;

                        //解析需要连线的节点对
                        if (treeNode.ToString().Contains("《-"))
                        {
                            string toolNodeText = Regex.Split(job.L_toolList[i].input[j].value.ToString(), "->")[0].Substring(3);
                            string toolIONodeText = "-->" + Regex.Split(job.L_toolList[i].input[j].value.ToString(), "->")[1];
                            job.D_itemAndSource.Add(treeNode, job.GetToolIONodeByNodeText(toolNodeText, toolIONodeText));
                        }
                        if (job.L_toolList[i].toolType == ToolType.Output)
                        {
                            string toolNodeText = Regex.Split(treeNode.Text, "->")[0].Substring(3);
                            string toolIONodeText = Regex.Split(treeNode.Text, "->")[1];
                            job.D_itemAndSource.Add(treeNode, job.GetToolIONodeByNodeText(toolNodeText, toolIONodeText));
                        }
                    }
                    for (int k = 0; k < job.L_toolList[i].output.Count; k++)
                    {
                        TreeNode treeNode = node.Nodes.Add("-->" + job.L_toolList[i].output[k].IOName);
                        treeNode.Tag = job.L_toolList[i].output[k].ioType;
                        treeNode.ForeColor = Color.Blue;
                    }
                }

                UpdateJobTreeIcon(job.jobName);

                //默认选中第一个节点
                if (tvw_job.Nodes.Count > 0)
                    tvw_job.SelectedNode = tvw_job.Nodes[0];
                return job;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        public int m_MouseClicks = 0; //记录鼠标在myTreeView控件上按下的次数
        /// <summary>
        /// 加载指定的流程
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public static Job LoadJob(Job job)
        {
            try
            {
                job.EnsureBlobFollowInput();
                foreach (TabPage item in Frm_Job.Instance.tbc_jobs.TabPages)
                {
                    if (item.Text == job.jobName)
                    {
                        Frm_Output.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "\r\nA process with the same name already exists. Please do not add it again" : "\r\n已经存在与此流程名相同的流程，请勿重复添加", Color.Red);
                        return new Job();
                    }
                }
                job.isRunLoop = false;

                TreeView tvw_job = new TreeView();
                tvw_job.Scrollable = true;
                tvw_job.ItemHeight = 26;
                tvw_job.ShowLines = false;
                tvw_job.AllowDrop = true;
                tvw_job.ImageList = Job.imageList;
                tvw_job.TabStop = false;
                tvw_job.ShowNodeToolTips = true;


                tvw_job.AfterSelect += job.tvw_job_AfterSelect;
                tvw_job.AfterLabelEdit += new NodeLabelEditEventHandler(job.EditNodeText);
                tvw_job.MouseClick += new MouseEventHandler(job.TVW_MouseClick);
                tvw_job.MouseDoubleClick += new MouseEventHandler(job.TVW_DoubleClick);
                tvw_job.AfterSelect += new TreeViewEventHandler(job.TVW_AfterSelect);

                tvw_job.MouseDown += job.tvw_job_MouseDown;
                tvw_job.BeforeCollapse += job.tvw_job_BeforeCollapse;
                tvw_job.BeforeExpand += job.tvw_job_BeforeExpand;

                //节点间拖拽
                tvw_job.ItemDrag += new ItemDragEventHandler(job.tvw_job_ItemDrag);
                tvw_job.DragEnter += new DragEventHandler(job.tvw_job_DragEnter);
                tvw_job.DragDrop += new DragEventHandler(job.tvw_job_DragDrop);
                tvw_job.MouseDown += new MouseEventHandler(job.tvw_tools_MouseDown);


                //以下事件为画线事件
                if (Project.Instance.configuration.displayLine)
                {
                    tvw_job.MouseEnter += job.tvw_job_MouseEnter;
                    tvw_job.MouseWheel += job.DrawLineWithoutRefresh;
                    //  tvw_job.MouseUp += job.tvw_job_MouseUp;

                    tvw_job.AfterExpand += job.Draw_Line;
                    tvw_job.AfterCollapse += job.Draw_Line;

                }

                Frm_Job.Instance.tbc_jobs.TabPages.Add(job.jobName);

                Frm_Job.Instance.tbc_jobs.TabPages[Frm_Job.Instance.tbc_jobs.TabPages.Count - 1].Controls.Add(tvw_job);
                tvw_job.Dock = DockStyle.Fill;
                tvw_job.ShowNodeToolTips = true;
                tvw_job.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                //反序列化各工具
                job.D_itemAndSource.Clear();
                for (int i = 0; i < job.L_toolList.Count; i++)
                {
                    TreeNode node = GetJobTree(job.jobName).Nodes.Add(job.L_toolList[i].toolName);
                    for (int j = 0; j < job.L_toolList[i].input.Count; j++)
                    {
                        TreeNode treeNode;
                        //因为OutputBox只有源，所以此处特殊处理
                        if (job.L_toolList[i].toolType != ToolType.Output)
                            treeNode = node.Nodes.Add("<--" + job.L_toolList[i].input[j].IOName + job.L_toolList[i].input[j].value);
                        else
                            treeNode = node.Nodes.Add("<--" + job.L_toolList[i].input[j].IOName);

                        treeNode.Tag = job.L_toolList[i].input[j].ioType;
                        treeNode.ForeColor = Color.DarkMagenta;

                        //解析需要连线的节点对
                        if (treeNode.ToString().Contains("《-"))
                        {
                            string toolNodeText = Regex.Split(job.L_toolList[i].input[j].value.ToString(), "->")[0].Substring(3);
                            string toolIONodeText = "-->" + Regex.Split(job.L_toolList[i].input[j].value.ToString(), "->")[1];
                            job.D_itemAndSource.Add(treeNode, job.GetToolIONodeByNodeText(toolNodeText, toolIONodeText));
                        }
                        if (job.L_toolList[i].toolType == ToolType.Output)
                        {
                            string toolNodeText = Regex.Split(treeNode.Text, "->")[0].Substring(3);
                            string toolIONodeText = Regex.Split(treeNode.Text, "->")[1];
                            job.D_itemAndSource.Add(treeNode, job.GetToolIONodeByNodeText(toolNodeText, "-->" + toolIONodeText));
                        }
                    }
                    for (int k = 0; k < job.L_toolList[i].output.Count; k++)
                    {
                        TreeNode treeNode = node.Nodes.Add("-->" + job.L_toolList[i].output[k].IOName);

                        treeNode.Tag = job.L_toolList[i].output[k].ioType;
                        treeNode.ForeColor = Color.Blue;
                    }
                }

                UpdateJobTreeIcon(job.jobName);

                //默认选中第一个节点
                if (tvw_job.Nodes.Count > 0)
                    tvw_job.SelectedNode = tvw_job.Nodes[0];

                return job;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }

        private void tvw_job_MouseUp(object sender, MouseEventArgs e)
        {
            Thread.Sleep(1000);
            Application.DoEvents();
            Job.GetJobTree(jobName).Update();
            DrawLine();
        }

        private void tvw_job_MouseEnter(object sender, EventArgs e)
        {
            Job.GetJobTree(jobName).Update();
            DrawLine();
        }
        /// <summary>
        /// 加载指定的流程
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public static Job OpenJob(Job job)
        {
            try
            {
                job.EnsureBlobFollowInput();
                foreach (TabPage item in Frm_Job.Instance.tbc_jobs.TabPages)
                {
                    if (item.Text == job.jobName)
                    {
                        Frm_Output.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "\r\nA process with the same name already exists. Please do not add it again" : "\r\n已经存在与此流程名相同的流程，请勿重复添加", Color.Red);
                        return new Job();
                    }
                }
                job.isRunLoop = false;

                TreeView tvw_job = new TreeView();
                tvw_job.Scrollable = true;
                tvw_job.ItemHeight = 26;
                tvw_job.ShowLines = false;
                tvw_job.AllowDrop = true;
                tvw_job.ImageList = Job.imageList;
                tvw_job.TabStop = false;
                tvw_job.ShowNodeToolTips = true;


                tvw_job.AfterSelect += job.tvw_job_AfterSelect;
                tvw_job.AfterLabelEdit += new NodeLabelEditEventHandler(job.EditNodeText);
                tvw_job.MouseClick += new MouseEventHandler(job.TVW_MouseClick);
                tvw_job.MouseDoubleClick += new MouseEventHandler(job.TVW_DoubleClick);
                tvw_job.AfterSelect += new TreeViewEventHandler(job.TVW_AfterSelect);

                tvw_job.MouseDown += job.tvw_job_MouseDown;
                tvw_job.BeforeCollapse += job.tvw_job_BeforeCollapse;
                tvw_job.BeforeExpand += job.tvw_job_BeforeExpand;

                //节点间拖拽
                tvw_job.ItemDrag += new ItemDragEventHandler(job.tvw_job_ItemDrag);
                tvw_job.DragEnter += new DragEventHandler(job.tvw_job_DragEnter);
                tvw_job.DragDrop += new DragEventHandler(job.tvw_job_DragDrop);
                tvw_job.MouseDown += new MouseEventHandler(job.tvw_tools_MouseDown);


                //以下事件为画线事件
                if (Project.Instance.configuration.displayLine)
                {
                    Frm_Job.Instance.Paint += job.Instance_Paint;
                    tvw_job.MouseMove += job.DrawLineWithoutRefresh;
                    tvw_job.MouseWheel += job.DrawLineWithoutRefresh;

                    tvw_job.AfterExpand += job.Draw_Line;
                    tvw_job.AfterCollapse += job.Draw_Line;
                    Frm_Job.Instance.tbc_jobs.SelectedIndexChanged += job.tbc_jobs_SelectedIndexChanged;
                }

                Frm_Job.Instance.tbc_jobs.TabPages.Add(job.jobName);

                Frm_Job.Instance.tbc_jobs.TabPages[Frm_Job.Instance.tbc_jobs.TabPages.Count - 1].Controls.Add(tvw_job);
                tvw_job.Dock = DockStyle.Fill;
                tvw_job.ShowNodeToolTips = true;
                tvw_job.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                //反序列化各工具
                job.D_itemAndSource.Clear();
                for (int i = 0; i < job.L_toolList.Count; i++)
                {
                    TreeNode node = GetJobTree(job.jobName).Nodes.Add(job.L_toolList[i].toolName);
                    for (int j = 0; j < job.L_toolList[i].input.Count; j++)
                    {
                        TreeNode treeNode;
                        //因为OutputBox只有源，所以此处特殊处理
                        if (job.L_toolList[i].toolType != ToolType.Output)
                            treeNode = node.Nodes.Add("<--" + job.L_toolList[i].input[j].IOName + job.L_toolList[i].input[j].value);
                        else
                            treeNode = node.Nodes.Add("<--" + job.L_toolList[i].input[j].IOName);

                        treeNode.Tag = job.L_toolList[i].input[j].ioType;
                        treeNode.ForeColor = Color.DarkMagenta;

                        //解析需要连线的节点对
                        if (treeNode.ToString().Contains("《-"))
                        {
                            string toolNodeText = Regex.Split(job.L_toolList[i].input[j].value.ToString(), "->")[0].Substring(3);
                            string toolIONodeText = "-->" + Regex.Split(job.L_toolList[i].input[j].value.ToString(), "->")[1];
                            job.D_itemAndSource.Add(treeNode, job.GetToolIONodeByNodeText(toolNodeText, toolIONodeText));
                        }
                        if (job.L_toolList[i].toolType == ToolType.Output)
                        {
                            string toolNodeText = Regex.Split(treeNode.Text, "->")[0].Substring(3);
                            string toolIONodeText = Regex.Split(treeNode.Text, "->")[1];
                            job.D_itemAndSource.Add(treeNode, job.GetToolIONodeByNodeText(toolNodeText, "-->" + toolIONodeText));
                        }
                    }
                    for (int k = 0; k < job.L_toolList[i].output.Count; k++)
                    {
                        TreeNode treeNode = node.Nodes.Add("-->" + job.L_toolList[i].output[k].IOName);

                        treeNode.Tag = job.L_toolList[i].output[k].ioType;
                        treeNode.ForeColor = Color.Blue;
                    }
                }

                UpdateJobTreeIcon(job.jobName);

                //默认选中第一个节点
                if (tvw_job.Nodes.Count > 0)
                    tvw_job.SelectedNode = tvw_job.Nodes[0];

                return job;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }

        void tvw_job_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = (this.m_MouseClicks > 1);
        }

        void tvw_job_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = (m_MouseClicks > 1);
        }

        void tvw_job_MouseDown(object sender, MouseEventArgs e)
        {
            m_MouseClicks = e.Clicks;
        }
        /// <summary>
        /// 运行当前流程
        /// </summary>
        internal static void RunCurJob()
        {
            try
            {
                if (Project.Instance.L_engineList.Count == 0)
                {
                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "No jobs to run" : "当前项目中未添加任何方案，请先新建方案", Color.Black);
                    return;
                }

                if (Project.Instance.curEngine.L_jobList.Count == 0)
                {
                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "No jobs to run" : "当前方案中未添加任何流程，请先新建流程", Color.Black);
                    return;
                }
                RunAndWait(Frm_Job.Instance.tbc_jobs.SelectedTab.Text);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 异步运行指定流程。按钮事件里不要直接同步调用 Run()，否则采图/通讯阻塞会卡住 UI。
        /// </summary>
        internal static Thread RunAsync(string jobName, bool drawLine = false)
        {
            try
            {
                Job job = FindJobByName(jobName);
                if (job == null)
                    return null;
                if (!job.BeginSingleRun())
                    return null;
                if (drawLine && Project.Instance.configuration.displayLine && Frm_Job.Instance.tbc_jobs.TabPages.Count > 0)
                    BeginDrawJobLine(jobName, job);

                Thread th = new Thread(() =>
                {
                    try
                    {
                        job.Run();
                        if (drawLine && Project.Instance.configuration.displayLine && Frm_Job.Instance.tbc_jobs.TabPages.Count > 0)
                            BeginDrawJobLine(jobName, job);
                    }
                    catch (Exception ex)
                    {
                        Log.SaveError(ex);
                    }
                    finally
                    {
                        job.EndSingleRun();
                    }
                });
                th.IsBackground = true;
                th.Start();
                return th;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        private static void BeginDrawJobLine(string jobName, Job job)
        {
            try
            {
                TreeView jobTree = GetJobTree(jobName);
                if (jobTree != null && jobTree.IsHandleCreated)
                {
                    jobTree.BeginInvoke(new MethodInvoker(delegate
                    {
                        job.DrawLine();
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 在后台线程运行流程并等待完成；等待期间处理 UI 消息，保留"运行后读取结果"的旧调用语义。
        /// </summary>
        internal static void RunAndWait(string jobName)
        {
            try
            {
                Job job = FindJobByName(jobName);
                if (job == null)
                    return;
                if (Frm_Main.Instance.IsHandleCreated && !Frm_Main.Instance.InvokeRequired)
                {
                    Thread th = RunAsync(jobName);
                    while (th != null && th.IsAlive)
                    {
                        Application.DoEvents();
                        Thread.Sleep(20);
                    }
                }
                else
                {
                    job.Run();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private bool BeginSingleRun()
        {
            lock (this)
            {
                if (isRunOnceBusy || isRunLoop || activeRunCount > 0 || (loopRunThread != null && loopRunThread.IsAlive))
                {
                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The job is running, please wait" : string.Format("流程 [{0}] 正在运行，请稍候", jobName), Color.DarkOrange);
                    return false;
                }
                isRunOnceBusy = true;
                stopRequested = false;
            }
            SetRunButtonEnabled(false);
            return true;
        }
        private void EndSingleRun()
        {
            lock (this)
            {
                isRunOnceBusy = false;
            }
            SetRunButtonEnabled(true);
        }
        private static void SetRunButtonEnabled(bool enabled)
        {
            try
            {
                if (Frm_Job.Instance.btn_runOnce.InvokeRequired)
                {
                    Frm_Job.Instance.btn_runOnce.BeginInvoke(new MethodInvoker(delegate
                    {
                        Frm_Job.Instance.btn_runOnce.Enabled = enabled;
                    }));
                }
                else
                {
                    Frm_Job.Instance.btn_runOnce.Enabled = enabled;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private static void SetLoopRunStoppedUi()
        {
            try
            {
                MethodInvoker update = delegate
                {
                    Frm_Job.Instance.btn_runLoop.Text = Project.Instance.configuration.language == Language.English ? "Run Loop" : "连续运行";
                    Frm_Job.Instance.btn_runLoop.Enabled = true;
                    Frm_Main.Instance.toolStripButton12.Text = "连续运行";
                };

                if (Frm_Job.Instance.btn_runLoop.InvokeRequired)
                    Frm_Job.Instance.btn_runLoop.BeginInvoke(update);
                else
                    update();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 通过流程名获取流程
        /// </summary>
        /// <param name="jobName">流程名</param>
        /// <returns>流程</returns>
        public static Job FindJobByName(string jobName)
        {
            try
            {
                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    if ((Project.Instance.curEngine.L_jobList[i]).jobName == jobName)
                        return Project.Instance.curEngine.L_jobList[i];
                }
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Can not find job named：" + jobName + "（Error code：0001）" : "未找到名为" + jobName + "的流程（错误代码：00001）");
                return null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 判断是否已经存在此名称的流程
        /// </summary>
        /// <param name="jobName">流程名</param>
        /// <returns>是否已存在</returns>
        internal static bool CheckJobExist(string jobName)
        {
            try
            {
                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    if ((Project.Instance.curEngine.L_jobList[i]).jobName == jobName)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return true;
            }
        }
        /// <summary>
        /// 导入流程
        /// </summary>
        /// <param name="job">流程对象</param>
        internal static void InportJob(Job job)
        {
            try
            {
                job.EnsureBlobFollowInput();
                TreeView tvw_job = new TreeView();
                tvw_job.Scrollable = true;
                tvw_job.ItemHeight = 26;
                tvw_job.ShowLines = false;
                tvw_job.AllowDrop = true;
                tvw_job.ImageList = Job.imageList;
                tvw_job.TabStop = false;
                tvw_job.ShowNodeToolTips = true;

                tvw_job.AfterSelect += job.tvw_job_AfterSelect;
                tvw_job.AfterLabelEdit += new NodeLabelEditEventHandler(job.EditNodeText);
                tvw_job.MouseClick += new MouseEventHandler(job.TVW_MouseClick);
                tvw_job.MouseDoubleClick += new MouseEventHandler(job.TVW_DoubleClick);
                tvw_job.AfterSelect += new TreeViewEventHandler(job.TVW_AfterSelect);

                tvw_job.MouseDown += job.tvw_job_MouseDown;
                tvw_job.BeforeCollapse += job.tvw_job_BeforeCollapse;
                tvw_job.BeforeExpand += job.tvw_job_BeforeExpand;

                //节点间拖拽
                tvw_job.ItemDrag += new ItemDragEventHandler(job.tvw_job_ItemDrag);
                tvw_job.DragEnter += new DragEventHandler(job.tvw_job_DragEnter);
                tvw_job.DragDrop += new DragEventHandler(job.tvw_job_DragDrop);
                tvw_job.MouseDown += new MouseEventHandler(job.tvw_tools_MouseDown);

                //以下事件为画线事件
                if (Project.Instance.configuration.displayLine)
                {
                    tvw_job.MouseEnter += job.tvw_job_MouseEnter;
                    // tvw_job.MouseMove += job.DrawLineWithoutRefresh;
                    tvw_job.MouseWheel += job.DrawLineWithoutRefresh;

                    tvw_job.AfterExpand += job.Draw_Line;
                    tvw_job.AfterCollapse += job.Draw_Line;
                }

                Frm_Job.Instance.tbc_jobs.TabPages.Add(job.jobName);
                Frm_Job.Instance.tbc_jobs.TabPages[Frm_Job.Instance.tbc_jobs.TabPages.Count - 1].Controls.Add(tvw_job);
                tvw_job.Dock = DockStyle.Fill;
                tvw_job.ShowNodeToolTips = true;
                tvw_job.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                //反序列化各工具
                job.D_itemAndSource.Clear();
                for (int i = 0; i < job.L_toolList.Count; i++)
                {
                    TreeNode node = GetJobTree(job.jobName).Nodes.Add(job.L_toolList[i].toolName);
                    for (int j = 0; j < job.L_toolList[i].input.Count; j++)
                    {
                        TreeNode treeNode;
                        //因为OutputBox只有源，所以此处特殊处理
                        if (job.L_toolList[i].toolType != ToolType.Output)
                            treeNode = node.Nodes.Add("<--" + job.L_toolList[i].input[j].IOName + job.L_toolList[i].input[j].value);
                        else
                            treeNode = node.Nodes.Add("<--" + job.L_toolList[i].input[j].IOName);

                        treeNode.Tag = job.L_toolList[i].input[j].ioType;
                        treeNode.ForeColor = Color.DarkMagenta;

                        //解析需要连线的节点对

                        if (treeNode.ToString().Contains("《-"))
                        {
                            string toolNodeText = Regex.Split(job.L_toolList[i].input[j].value.ToString(), "->")[0].Substring(3);
                            string toolIONodeText = "-->" + Regex.Split(job.L_toolList[i].input[j].value.ToString(), "->")[1];
                            job.D_itemAndSource.Add(treeNode, job.GetToolIONodeByNodeText(toolNodeText, toolIONodeText));
                        }
                        if (job.L_toolList[i].toolType == ToolType.Output)
                        {
                            string toolNodeText = Regex.Split(treeNode.Text, "->")[0].Substring(3);
                            string toolIONodeText = Regex.Split(treeNode.Text, "->")[1];
                            job.D_itemAndSource.Add(treeNode, job.GetToolIONodeByNodeText(toolNodeText, toolIONodeText));
                        }
                    }
                    for (int k = 0; k < job.L_toolList[i].output.Count; k++)
                    {
                        TreeNode treeNode = node.Nodes.Add("-->" + job.L_toolList[i].output[k].IOName);

                        treeNode.Tag = job.L_toolList[i].output[k].ioType;
                        treeNode.ForeColor = Color.Blue;
                    }

                    UpdateJobTreeIcon(job.jobName);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 导出流程
        /// </summary>
        internal static void ExportJob()
        {
            try
            {
                if (Project.Instance.curEngine.L_jobList.Count > 0)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                    System.Windows.Forms.SaveFileDialog dig_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                    dig_saveFileDialog.FileName = jobName;
                    dig_saveFileDialog.Title = (Project.Instance.configuration.language == Language.English ? "Please select the project file saving path" : "请选择流程路径");
                    dig_saveFileDialog.Filter = "流程文件|*.job";
                    dig_saveFileDialog.InitialDirectory = path;
                    if (dig_saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        Stream stream = new FileStream(dig_saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                        formatter.Serialize(stream, Project.Instance.curEngine.FindJobByName(jobName));
                        stream.Close();

                        //更新结果下拉框
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Project exported successfully" : "流程导出成功", Color.Green);
                    }
                }
                else
                {
                    Frm_Main.Instance.OutputMsg("当前方案尚未添加流程，不可导出", Color.Red);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 克隆当前流程
        /// </summary>
        internal static void CloneJob()
        {
            try
            {
            Again:
                Frm_InputMessage.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "Please input job's name" : "请输入新流程名");
                Frm_InputMessage.Instance.btn_confirm.Text = (Project.Instance.configuration.language == Language.English ? "Confirm" : "确定");
                Frm_InputMessage.Instance.passwordChar = false;
                Frm_InputMessage.Instance.txt_input.TextStr = string.Empty;
                Frm_InputMessage.Instance.ShowDialog();
                string newJobName = Frm_InputMessage.input;
                if (newJobName != string.Empty)
                {
                    //检查此名称的流程是否已存在
                    if (CheckJobExist(newJobName))
                    {
                        Frm_MessageBox.Instance.MessageBoxShow((Project.Instance.configuration.language == Language.English ? "\r\nA process with this name already exists. The process name cannot be repeated. Please enter again" : "\r\n已存在此名称的流程，流程名不可重复，请重新输入"));
                        goto Again;
                    }
                    Frm_Output.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "A new process named:" + newJobName : "克隆了新流程，流程名为：" + newJobName, Color.Black);

                    string sourceJobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                    Job job = ObjectCopier.Clone(Job.FindJobByName(sourceJobName));
                    job.jobName = newJobName;
                    for (int i = 0; i < job.L_toolList.Count; i++)
                    {
                        job.L_toolList[i].tool.jobName = newJobName;
                    }

                    Project.Instance.curEngine.L_jobList.Add(job);
                    LoadJob(job);

                    Frm_Main.Instance.SaveAll();
                    Frm_Output.Instance.OutputMsg("流程克隆成功", Color.Black);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        public static class ObjectCopier
        {
            public static Job Clone<Job>(Job source)
            {
                if (!typeof(Job).IsSerializable)
                {
                    throw new ArgumentException("The type must be serializable.", "source");
                }

                if (Object.ReferenceEquals(source, null))
                {
                    return default(Job);
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (Job)formatter.Deserialize(stream);
                }
            }
        }
        /// <summary>
        /// 通过流程名从流程集合中移除流程
        /// </summary>
        /// <param name="jobName">流程名</param>
        internal static void RemoveJobByName(string jobName)
        {
            try
            {
                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    if (((Job)Project.Instance.curEngine.L_jobList[i]).jobName == jobName)
                    {
                        Project.Instance.curEngine.L_jobList.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        internal static void DeleteJob()
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;



                if (Frm_Job.Instance.tbc_jobs.TabPages.Count < 1)
                    return;

                TabPage jobPage = Frm_Job.Instance.tbc_jobs.SelectedTab;
                string jobName = jobPage.Text;
                Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : string.Format("确定要删除流程 [{0}] 吗？", jobName));
                Frm_ConfirmBox.Instance.ShowDialog();
                if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Yes)
                {
                    int pageIndex = Frm_Job.Instance.tbc_jobs.TabPages.IndexOf(jobPage);
                    if (pageIndex < 0)
                        return;

                    string currentJobName = string.Empty;
                    // Select the preceding job before removing the current page. The selection
                    // event queries the job list, so it must not observe a deleted job.
                    if (Frm_Job.Instance.tbc_jobs.TabPages.Count > 1)
                    {
                        int targetIndex = pageIndex > 0 ? pageIndex - 1 : 1;
                        Frm_Job.Instance.tbc_jobs.SelectedIndex = targetIndex;
                        currentJobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                    }
                    Frm_Job.Instance.tbc_jobs.TabPages.Remove(jobPage);
                    Job.RemoveJobByName(jobName);
                    string jobFilePath = Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + jobName + ".job";
                    if (File.Exists(jobFilePath))
                        File.Delete(jobFilePath);
                    Frm_Main.Instance.OutputMsg(currentJobName == string.Empty
                        ? string.Format("已删除流程 [{0}]，当前无可用流程", jobName)
                        : string.Format("已删除流程 [{0}]，当前流程已切换为 [{1}]", jobName, currentJobName), Color.Black);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        #endregion

        #region 流程树拖拽

        /// <summary>
        /// 拖动工具节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void tvw_job_ItemDrag(object sender, ItemDragEventArgs e)//左键拖动  
        {
            try
            {
                Frm_ToolBox.DragNode = null;
                if (((TreeView)sender).SelectedNode != null)
                {
                    if (((TreeView)sender).SelectedNode.Level == 1)          //输入输出不允许拖动
                    {
                        Job.GetJobTree(jobName).DoDragDrop(e.Item, DragDropEffects.Move);
                    }

                    else if (e.Button == MouseButtons.Left)
                    {
                        Job.GetJobTree(jobName).DoDragDrop(e.Item, DragDropEffects.Move);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 节点拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void tvw_job_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode"))
                {
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 放开被拖动的节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void tvw_job_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                //需要辨别是工具箱拖过来的，还是流程间内部拖拽，此处只要流程间一旦有拖动动作，就给ToolBox里面DragNode变量赋null，所以此处通过判断ToolBox中的DragNode是否为null来判断属于那种拖动
                if (Frm_ToolBox.DragNode != null)
                {
                    if (sender != null && sender is TreeView)
                    {
                        TreeView trv = sender as TreeView;
                        //if (trv.Tag != null)
                        //{
                        MoveTreeView move = (MoveTreeView)Convert.ToInt32(trv.Tag);
                        if (move == Frm_ToolBox.MoveTo) { Frm_ToolBox.DragNode = null; Frm_ToolBox.NodeSource = null; }
                        else
                        {
                            System.Drawing.Point p = trv.PointToClient(new System.Drawing.Point(e.X, e.Y));
                            TreeNode node = trv.GetNodeAt(p);
                            //string path = GetClientPath(DragNode, DragNode.Text);
                            //Frm_ToolBox.NodeSource.Nodes.Remove(Frm_ToolBox.DragNode);
                            //node.Nodes.Add(Frm_ToolBox.DragNode);
                            if (p.Y > ((TreeView)sender).Nodes[((TreeView)sender).Nodes.Count - 1].Bounds.Y)
                                Frm_ToolBox.Instance.AddTool(Frm_ToolBox.DragNode.Text, null, L_toolList.Count);
                            else if (node.Level == 0)
                                Frm_ToolBox.Instance.AddTool(Frm_ToolBox.DragNode.Text, null, node.Index);
                            else
                                Frm_ToolBox.Instance.AddTool(Frm_ToolBox.DragNode.Text, null, node.Parent.Index + 1);
                            return;
                        }
                        //}
                    }
                }


                //获得拖放中的节点  
                TreeNode moveNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                //根据鼠标坐标确定要移动到的目标节点  
                System.Drawing.Point pt;
                TreeNode targeNode;
                pt = ((TreeView)(sender)).PointToClient(new System.Drawing.Point(e.X, e.Y));
                targeNode = Job.GetJobTree(jobName).GetNodeAt(pt);
                //如果目标节点无子节点则添加为同级节点,反之添加到下级节点的未端  

                if (moveNode == targeNode)       //若是把自己拖放到自己，不可，返回
                    return;

                if (targeNode == null)       //目标节点为null，就是把节点拖到了空白区域，则表示要把节点拖到末尾
                {
                    if (moveNode.Level == 0)        //被拖动的是子节点，也就是工具节点
                    {
                        //if (targeNode.Level == 0)
                        {
                            moveNode.Remove();
                            Job.GetJobTree(jobName).Nodes.Insert(L_toolList.Count, moveNode);

                            ToolInfo temp = new ToolInfo();
                            for (int i = 0; i < L_toolList.Count; i++)
                            {
                                if (L_toolList[i].toolName == moveNode.Text)
                                {
                                    temp = L_toolList[i];
                                    L_toolList.RemoveAt(i);
                                    L_toolList.Insert(L_toolList.Count, temp);
                                    break;
                                }
                            }
                        }
                        //else
                        //{
                        //    moveNode.Remove();
                        //    Job.GetJobTree(jobName).Nodes.Insert(targeNode.Parent.Index + 1, moveNode);

                        //    ToolInfo temp = new ToolInfo();
                        //    for (int i = 0; i < L_toolList.Count; i++)
                        //    {
                        //        if (L_toolList[i].toolName == moveNode.Text)
                        //        {
                        //            temp = L_toolList[i];
                        //            L_toolList.RemoveAt(i);
                        //            L_toolList.Insert(targeNode.Parent.Index + 1, temp);
                        //            break;
                        //        }
                        //    }
                        //}
                    }
                    //更新当前拖动的节点选择  
                    Job.GetJobTree(jobName).SelectedNode = moveNode;
                    //展开目标节点,便于显示拖放效果  
                    GetToolNodeByNodeText(moveNode.Text).Expand();
                    return;
                }

                if (moveNode.Level == 1 && targeNode.Level == 1 && moveNode.Parent == targeNode.Parent)          //都是输入输出节点，内部拖动排序
                {
                    moveNode.Remove();
                    targeNode.Parent.Nodes.Insert(targeNode.Index, moveNode);
                    return;
                }

                if (moveNode.Level == 0)        //被拖动的是子节点，也就是工具节点
                {
                    if (targeNode.Level == 0)
                    {
                        moveNode.Remove();
                        Job.GetJobTree(jobName).Nodes.Insert(targeNode.Index, moveNode);

                        ToolInfo temp = new ToolInfo();
                        for (int i = 0; i < L_toolList.Count; i++)
                        {
                            if (L_toolList[i].toolName == moveNode.Text)
                            {
                                temp = L_toolList[i];
                                L_toolList.RemoveAt(i);
                                L_toolList.Insert(targeNode.Index - 1, temp);
                                break;
                            }
                        }
                    }
                    else
                    {
                        moveNode.Remove();
                        Job.GetJobTree(jobName).Nodes.Insert(targeNode.Parent.Index + 1, moveNode);

                        ToolInfo temp = new ToolInfo();
                        for (int i = 0; i < L_toolList.Count; i++)
                        {
                            if (L_toolList[i].toolName == moveNode.Text)
                            {
                                temp = L_toolList[i];
                                L_toolList.RemoveAt(i);
                                L_toolList.Insert(targeNode.Parent.Index + 1, temp);
                                break;
                            }
                        }
                    }
                }
                else        //被拖动的是输入输出节点
                {
                    if (targeNode.Level == 0 && FindToolInfoByName(targeNode.Text).toolType == ToolType.Output)
                    {
                        string result = moveNode.Parent.Text + "->" + moveNode.Text.Substring(3);
                        if (!((DataGridViewComboBoxCell)(Frm_Monitor.Instance.dgv_monitor.Rows[Frm_Monitor.Instance.dgv_monitor.Rows.Count - 1].Cells[0])).Items.Contains(result))
                            ((DataGridViewComboBoxCell)(Frm_Monitor.Instance.dgv_monitor.Rows[Frm_Monitor.Instance.dgv_monitor.Rows.Count - 1].Cells[0])).Items.Add(result);

                        FindToolInfoByName(targeNode.Text).input.Add(new ToolIO(result, "", DataType.String));
                        TreeNode node = targeNode.Nodes.Add("", "<--" + result, 34, 34);
                        node.ForeColor = Color.DarkMagenta;
                        D_itemAndSource.Add(node, moveNode);
                        targeNode.Expand();
                        DrawLine();
                        return;
                    }
                    else if (targeNode.Level == 0)
                        return;

                    //连线前首先要判断被拖动节点是否为输出项，目标节点是否为输入项
                    if (moveNode.Text.Substring(0, 3) != "-->" || targeNode.Text.Substring(0, 3) != "<--")
                    {
                        Frm_Main.Instance.OutputMsg("输入项与输出项数据类型不一致，不可关联", Color.Red);
                        return;
                    }

                    //连线前要判断被拖动节点和目标节点的数据类型是否一致
                    if ((DataType)moveNode.Tag != (DataType)targeNode.Tag)
                    {
                        Frm_Main.Instance.OutputMsg(string.Format("输入项数据类型为{0}，输出项数据类型为{1}，数据类型不一致，不可关联", (DataType)moveNode.Tag, (DataType)targeNode.Tag), Color.Red);
                        return;
                    }

                    string input = targeNode.Text;
                    if (input.Contains("《"))       //表示已经连接了源
                    {
                        input = Regex.Split(targeNode.Text, "《")[0];
                        string oldSource = Regex.Split(targeNode.Text.Substring(3), "《- ")[1];
                        string oldSourceTool = Regex.Split(oldSource, "->")[0];
                        string oldSourceIO = Regex.Split(oldSource, "->")[1];

                        //移除旧的连线，并新增新的连线
                        for (int i = 0; i < D_itemAndSource.Count; i++)
                        {
                            if (((TreeNode)targeNode) == (TreeNode)D_itemAndSource.Keys.ToArray()[i] && ((TreeNode)GetToolIONodeByNodeText(oldSourceTool, "-->" + oldSourceIO)) == (TreeNode)D_itemAndSource[D_itemAndSource.Keys.ToArray()[i]])
                            {
                                D_itemAndSource.Remove(D_itemAndSource.Keys.ToArray()[i]);
                                break;
                            }
                        }

                        //添加新的连线
                        D_itemAndSource.Add(targeNode, moveNode);
                    }
                    else            //第一次连接源就需要添加到输入输出集合
                    {
                        D_itemAndSource.Add(targeNode, moveNode);
                    }
                    FindToolInfoByName(targeNode.Parent.Text).GetInput(input.Substring(3)).value = "《- " + moveNode.Parent.Text + "->" + moveNode.Text.Substring(3);
                    targeNode.Text = input + "《- " + moveNode.Parent.Text + "->" + moveNode.Text.Substring(3);
                    DrawLine();

                    //移除拖放的节点  
                    if (moveNode.Level == 0)
                        moveNode.Remove();
                }
                //更新当前拖动的节点选择  
                Job.GetJobTree(jobName).SelectedNode = moveNode;
                //展开目标节点,便于显示拖放效果  
                targeNode.Expand();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        #endregion

        #region 流程编辑

        /// <summary>
        /// 添加输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void Add_input(object sender, string toolName)
        {
            try
            {
                string result = string.Empty;
                DataType ioType;
                string temp = sender.GetType().ToString();
                if (temp == "System.Windows.Forms.ToolStripMenuItem")
                {
                    result = sender.ToString();
                    ioType = (DataType)((ToolStripItem)sender).Tag;
                }
                else
                {
                    if (((TreeNode)sender).Tag == null)
                        result = ((TreeNode)sender).Text;
                    else if ((DataType)((TreeNode)sender).Tag == DataType.String)
                        result = (((TreeNode)sender).Text.Contains("=") ? Regex.Split(((TreeNode)sender).Text.Substring(10), "=")[0] : ((TreeNode)sender).Text);
                    else if ((DataType)((TreeNode)sender).Tag == DataType.Image || (DataType)((TreeNode)sender).Tag == DataType.XY || (DataType)((TreeNode)sender).Tag == DataType.Pose || (DataType)((TreeNode)sender).Tag == DataType.Region)
                    {
                        int idx = ((TreeNode)sender).Text.IndexOf("  ");
                        result = ((TreeNode)sender).Text.Substring(idx + 2, ((TreeNode)sender).Text.Length - 2 - idx);
                    }

                    ioType = (DataType)((TreeNode)sender).Tag;
                }

                //首先检查是否已经有此输入项,若已添加，则返回
                foreach (var item in GetToolNodeByNodeText(toolName).Nodes)
                {
                    string text;
                    if (((TreeNode)item).Text.Contains("《"))
                    {
                        text = Regex.Split(((TreeNode)item).Text, "《")[0];
                    }
                    else
                    {
                        text = ((TreeNode)item).Text;
                    }
                    if (text == "<--" + result)
                    {
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "This input or output item already exists and cannot be added repeatedly" : "已存在此输入或输出项，不可重复添加", Color.Black);
                        return;
                    }
                }

                int insertPos = GetInputItemNum(GetToolNodeByNodeText(toolName));        //获取插入位置，要保证输入项在前，输出项在后
                TreeNode node = GetToolNodeByNodeText(toolName).Nodes.Insert(insertPos, "", "<--" + result, 34, 34);
                node.ForeColor = Color.DarkMagenta;
                GetToolNodeByNodeText(toolName).ExpandAll();


                node.Tag = ioType;
                node.Name = "<--" + result;
                FindToolInfoByName(toolName).input.Add(new ToolIO(result, "", ioType));

                //如果是给输出工具添加输入，则需要连线
                if (FindToolInfoByName(toolName).toolType == ToolType.Output)
                {
                    if (!((DataGridViewComboBoxCell)(Frm_Monitor.Instance.dgv_monitor.Rows[Frm_Monitor.Instance.dgv_monitor.Rows.Count - 1].Cells[0])).Items.Contains(result))
                        ((DataGridViewComboBoxCell)(Frm_Monitor.Instance.dgv_monitor.Rows[Frm_Monitor.Instance.dgv_monitor.Rows.Count - 1].Cells[0])).Items.Add(result);
                    string toolNodeText = Regex.Split(sender.ToString(), "->")[0];
                    string toolIONodeText = Regex.Split(sender.ToString(), "->")[1];
                    D_itemAndSource.Add(GetToolIONodeByNodeText(toolName, "<--" + sender.ToString()), GetToolIONodeByNodeText(toolNodeText, "-->" + toolIONodeText));

                    Draw_Line(null, null);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 添加输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void Add_output(object sender, string toolName)
        {
            try
            {
                string temp = sender.GetType().ToString();
                string text = string.Empty;
                DataType ioType;
                if (temp != "System.Windows.Forms.TreeNode")
                {
                    List<ToolStripItem> list = new List<ToolStripItem>();
                    ToolStripItem dd = (ToolStripItem)sender;
                    list.Add(dd);


                    while (true)
                    {
                        dd = dd.OwnerItem;
                        if (dd.Name == "添加输出项")
                            break;
                        list.Add(dd);
                    }


                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        text += list[i].Name;
                        if (i != 0)
                            text += " . ";
                    }
                    ioType = (DataType)((ToolStripItem)sender).Tag;
                }
                else
                {
                    List<TreeNode> list = new List<TreeNode>();
                    TreeNode node1 = (TreeNode)sender;
                    list.Add(node1);

                    while (true)
                    {
                        node1 = node1.Parent;
                        if (node1 == null)
                            break;
                        list.Add(node1);
                    }

                    for (int i = list.Count - 2; i >= 0; i--)
                    {
                        if (list[i].Tag == null)
                            text += list[i].Text;
                        else if ((DataType)(list[i].Tag) == DataType.String)
                        //text += (list[i].Text.Contains("=") ? Regex.Split(list[i].Text, "=")[0] : list[i].Text);
                        {
                            int idx = list[i].Text.IndexOf("  ");
                            string temp11 = list[i].Text.Substring(idx + 2, list[i].Text.Length - 2 - idx);
                            text += Regex.Split(temp11, "=")[0];
                        }

                        else if ((DataType)(list[i].Tag) == DataType.Image || (DataType)(list[i].Tag) == DataType.XY || (DataType)(list[i].Tag) == DataType.Pose || (DataType)(list[i].Tag) == DataType.Region)
                        {
                            int idx = list[i].Text.IndexOf("  ");
                            text += list[i].Text.Substring(idx + 2, list[i].Text.Length - 2 - idx);
                        }


                        if (i != 0)
                            text += " . ";
                    }
                    ioType = (DataType)((TreeNode)sender).Tag;
                }




                foreach (var item in GetToolNodeByNodeText(toolName).Nodes)
                {
                    if (((TreeNode)item).Text == "-->" + text)
                    {
                        return;
                    }
                }
                TreeNode node = GetToolNodeByNodeText(toolName).Nodes.Add("", "-->" + text, 34, 34);
                node.ForeColor = Color.Blue;
                GetToolNodeByNodeText(toolName).ExpandAll();


                //指定输出变量的类型
                if (text == (Project.Instance.configuration.language == Language.English ? "OutputImage" : "输出图像"))
                {
                    node.ToolTipText = (Project.Instance.configuration.language == Language.English ? "Graphic variables do not support display" : "图形变量不支持显示");
                }

                node.Tag = ioType;
                node.Name = "-->" + text;
                FindToolInfoByName(GetToolNodeByNodeText(toolName).Text).output.Add(new ToolIO(text, "", ioType));
                node.ToolTipText = (Project.Instance.configuration.language == Language.English ? "NotRun" : "未运行");
                GetJobTree().ShowNodeToolTips = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal void SyncCodeEditIONodes(string toolName)
        {
            try
            {
                ToolInfo toolInfo = FindToolInfoByName(toolName);
                if (toolInfo == null || toolInfo.toolType != ToolType.CodeEdit)
                    return;

                CodeEditTool codeEditTool = toolInfo.tool as CodeEditTool;
                TreeNode toolNode = GetToolNodeByNodeText(toolName);
                if (codeEditTool == null || toolNode == null)
                    return;

                for (int i = D_itemAndSource.Count - 1; i >= 0; i--)
                {
                    TreeNode inputNode = D_itemAndSource.Keys.ToArray()[i];
                    if (inputNode != null && inputNode.Parent == toolNode)
                        D_itemAndSource.Remove(inputNode);
                }
                toolNode.Nodes.Clear();
                toolInfo.input.Clear();
                toolInfo.output.Clear();

                for (int i = 0; i < codeEditTool.L_inputItems.Count; i++)
                {
                    CodeInputItem item = codeEditTool.L_inputItems[i];
                    if (item == null || string.IsNullOrEmpty(item.InputName))
                        continue;
                    string source = item.VariableSource;
                    TreeNode node = toolNode.Nodes.Add("", "<--" + item.InputName + source, 34, 34);
                    node.ForeColor = Color.DarkMagenta;
                    node.Tag = DataType.String;
                    node.Name = "<--" + item.InputName + source;
                    node.ToolTipText = "变量：" + item.VariableSource;
                    toolInfo.input.Add(new ToolIO(item.InputName, source, DataType.String));
                }

                for (int i = 0; i < codeEditTool.L_outputItems.Count; i++)
                {
                    CodeOutputItem item = codeEditTool.L_outputItems[i];
                    if (item == null || string.IsNullOrEmpty(item.OutputName))
                        continue;
                    if (toolInfo.GetOutput(item.OutputName).IOName == item.OutputName)
                        continue;
                    TreeNode node = toolNode.Nodes.Add("", "-->" + item.OutputName, 34, 34);
                    node.ForeColor = Color.Blue;
                    node.Tag = DataType.String;
                    node.Name = "-->" + item.OutputName;
                    node.ToolTipText = Project.Instance.configuration.language == Language.English ? "NotRun" : "未运行";
                    toolInfo.output.Add(new ToolIO(item.OutputName, string.Empty, DataType.String));
                }

                RebuildCodeEditSourceLines(toolNode, toolInfo);
                toolNode.ExpandAll();
                GetJobTree().ShowNodeToolTips = true;
                DrawLine();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void RebuildCodeEditSourceLines(TreeNode toolNode, ToolInfo toolInfo)
        {
            for (int i = 0; i < toolInfo.input.Count; i++)
            {
                string source = toolInfo.input[i].value == null ? string.Empty : toolInfo.input[i].value.ToString();
                if (!source.StartsWith("《- ") || source.StartsWith("《- [") || source.StartsWith("《- 全局变量->"))
                    continue;

                string[] sourceParts = source.Substring(3).Split(new string[] { "->" }, StringSplitOptions.None);
                if (sourceParts.Length != 2)
                    continue;

                TreeNode inputNode = GetToolIONodeByNodeText(toolNode.Text, "<--" + toolInfo.input[i].IOName + source);
                TreeNode sourceNode = GetToolIONodeByNodeText(sourceParts[0], "-->" + sourceParts[1]);
                if (inputNode != null && sourceNode != null)
                    D_itemAndSource[inputNode] = sourceNode;
            }
        }
        /// <summary>
        /// 工具上移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveUp(object sender, EventArgs e)
        {
            try
            {
                if (GetJobTree().SelectedNode.Index == 0)
                    return;

                TreeNode Node = GetJobTree().SelectedNode;
                TreeNode PrevNode = Node.PrevNode;
                if (PrevNode != null)
                {
                    TreeNode NewNode = (TreeNode)Node.Clone();
                    if (Node.Parent == null)
                    {
                        GetJobTree().Nodes.Insert(PrevNode.Index, NewNode);
                    }
                    else
                    {
                        Node.Parent.Nodes.Insert(PrevNode.Index, NewNode);
                    }
                    Node.Remove();
                    GetJobTree().SelectedNode = NewNode;
                }

                ToolInfo temp = new ToolInfo();
                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolName == GetJobTree().SelectedNode.Text)
                    {
                        temp = L_toolList[i];
                        L_toolList[i] = L_toolList[i - 1];
                        L_toolList[i - 1] = temp;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 工具下移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveDown(object sender, EventArgs e)
        {
            try
            {
                if (GetJobTree().SelectedNode.Index == GetJobTree().Nodes.Count - 1)
                    return;
                TreeNode Node = GetJobTree().SelectedNode;
                TreeNode NextNode = Node.NextNode;
                if (NextNode != null)
                {
                    TreeNode NewNode = (TreeNode)Node.Clone();
                    if (Node.Parent == null)
                    {
                        GetJobTree().Nodes.Insert(NextNode.Index + 1, NewNode);
                    }
                    else
                    {
                        Node.Parent.Nodes.Insert(NextNode.Index + 1, NewNode);
                    }
                    Node.Remove();
                    GetJobTree().SelectedNode = NewNode;
                }

                ToolInfo temp = new ToolInfo();
                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolName == GetJobTree().SelectedNode.Text)
                    {
                        temp = L_toolList[i];
                        L_toolList[i] = L_toolList[i + 1];
                        L_toolList[i + 1] = temp;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void DeleteItem(object sender, EventArgs e)
        {
            try
            {
                if (Job.GetJobTree(jobName).SelectedNode == null)
                    return;



                isDrawing = true;
                string nodeText = Job.GetJobTree(jobName).SelectedNode.Text.ToString();
                int level = Job.GetJobTree(jobName).SelectedNode.Level;
                string fatherNodeText = string.Empty;
                if (level == 0)
                {
                    Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : string.Format("确定要删除工具 [{0}] 吗？", nodeText));
                    Frm_ConfirmBox.Instance.ShowDialog();
                    if (Frm_ConfirmBox.Instance.Result != ConfirmBoxResult.Yes)
                        return;
                }

                //如果是子节点
                if (level == 1)
                {
                    fatherNodeText = Job.GetJobTree(jobName).SelectedNode.Parent.Text;
                }
                foreach (TreeNode toolNode in Job.GetJobTree(jobName).Nodes)
                {
                    if (level == 1)
                    {
                        if (toolNode.Text == fatherNodeText)
                        {
                            foreach (var itemNode in ((TreeNode)toolNode).Nodes)
                            {
                                if (itemNode != null)
                                {
                                    if (((TreeNode)itemNode).Text == nodeText)
                                    {
                                        //移除连线集合中的这条连线
                                        for (int i = 0; i < D_itemAndSource.Count; i++)
                                        {
                                            if (((TreeNode)itemNode) == D_itemAndSource.Keys.ToArray()[i] || ((TreeNode)itemNode) == D_itemAndSource[D_itemAndSource.Keys.ToArray()[i]])
                                                D_itemAndSource.Remove(D_itemAndSource.Keys.ToArray()[i]);
                                        }

                                        ((TreeNode)itemNode).Remove();
                                        Job.GetJobTree(jobName).SelectedNode = null;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (((TreeNode)toolNode).Text == nodeText)
                        {
                            ((TreeNode)toolNode).Remove();
                            break;
                        }
                    }
                }

                //如果是父节点
                if (level == 0)
                {
                    for (int i = 0; i < L_toolList.Count; i++)
                    {
                        if (L_toolList[i].toolName == nodeText)
                        {
                            try
                            {
                                //移除连线集合中的这条连线
                                for (int j = D_itemAndSource.Count - 1; j >= 0; j--)
                                {
                                    if (nodeText == D_itemAndSource.Keys.ToArray()[j].Parent.Text || nodeText == D_itemAndSource[D_itemAndSource.Keys.ToArray()[j]].Parent.Text)
                                        D_itemAndSource.Remove(D_itemAndSource.Keys.ToArray()[j]);
                                }
                            }
                            catch { }

                            L_toolList.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < L_toolList.Count; i++)
                    {
                        if (L_toolList[i].toolName == fatherNodeText)
                        {
                            for (int j = 0; j < L_toolList[i].input.Count; j++)
                            {
                                if (L_toolList[i].input[j].value.ToString() == string.Empty)      //未连接源
                                {
                                    if (string.Format("<--{0}", L_toolList[i].input[j].IOName) == nodeText)
                                        L_toolList[i].RemoveInputIO(nodeText);
                                }
                                else    //已连接源
                                {
                                    if (string.Format("<--{0}{1}", L_toolList[i].input[j].IOName, L_toolList[i].input[j].value.ToString()) == nodeText)
                                        L_toolList[i].RemoveInputIO(nodeText);
                                }
                            }
                            for (int j = 0; j < L_toolList[i].output.Count; j++)
                            {
                                if (L_toolList[i].output[j].IOName == nodeText.Substring(3))
                                    L_toolList[i].RemoveOutputIO(nodeText.Substring(3));
                            }
                        }
                    }
                }

                isDrawing = false;
                DrawLine();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 工具重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EditNodeText(object sender, NodeLabelEditEventArgs e)
        {
            try
            {
                string newToolName = e.Label;
                if (newToolName == "" || newToolName == null)       //放弃工具重命名
                {
                    ThreadPool.QueueUserWorkItem(GiveupRename);
                    return;
                }

                //检查是否已经存在此名称的工具
                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolName == newToolName)
                    {
                        ((TreeView)sender).SelectedNode.Text = nodeTextBeforeEdit;
                        Application.DoEvents();
                        Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "\r\nA tool with this name already exists, and the tool name cannot be duplicated" : "\r\n已存在此名称的工具，工具名不可重复");
                        ((TreeView)sender).SelectedNode.BeginEdit();
                        return;
                    }
                }

                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolName == nodeTextBeforeEdit)
                    {
                        L_toolList[i].toolName = newToolName;
                    }
                }

                for (int i = 0; i < L_toolList.Count; i++)
                {
                    //对OutputBox特殊处理
                    if (L_toolList[i].toolType == ToolType.Output)
                    {
                        for (int j = 0; j < L_toolList[i].input.Count; j++)
                        {
                            string sourceFromItem = L_toolList[i].input[j].IOName;
                            string sourceFromToolName = Regex.Split(sourceFromItem.Substring(3), " . ")[0];
                            if (sourceFromToolName == nodeTextBeforeEdit)
                            {
                                string oldKey = L_toolList[i].input[j].IOName;
                                string value = L_toolList[i].input[j].value.ToString();
                                L_toolList[i].RemoveInputIO(oldKey);
                                string newKey = "<--" + newToolName + " . " + Regex.Split(sourceFromItem.Substring(3), " . ")[1];
                                L_toolList[i].input.Add(new ToolIO(newKey, value, DataType.String));
                                //修改节点文本
                                TreeNode toolNode = GetToolNodeByNodeText(L_toolList[i].toolName);
                                string nodeText = oldKey;
                                foreach (TreeNode item in toolNode.Nodes)
                                {
                                    if (((TreeNode)item).Text == nodeText)
                                    {
                                        ((TreeNode)item).Text = L_toolList[i].input[j].IOName;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < L_toolList[i].input.Count; j++)
                        {
                            if (L_toolList[i].input[j].value != string.Empty)
                            {
                                string sourceFromItem = L_toolList[i].input[j].value.ToString();
                                string sourceFromToolName = Regex.Split(sourceFromItem.Substring(3), "->")[0];
                                if (sourceFromToolName == nodeTextBeforeEdit)
                                {
                                    // 绑定格式统一为 《- 工具名->输出项，与运行时解析保持一致
                                    string outItem = Regex.Split(sourceFromItem.Substring(3), "->")[1];
                                    L_toolList[i].input[j].value = "《- " + newToolName + "->" + outItem;
                                    //修改节点文本
                                    TreeNode toolNode = GetToolNodeByNodeText(L_toolList[i].toolName);
                                    string nodeText = L_toolList[i].input[j].value + sourceFromItem;
                                    foreach (TreeNode item in toolNode.Nodes)
                                    {
                                        if (((TreeNode)item).Text == nodeText)
                                        {
                                            ((TreeNode)item).Text = L_toolList[i].input[j].value + "《- " + newToolName + "->" + outItem;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Job.GetJobTree(jobName).Show();
                Job.GetJobTree(jobName).LabelEdit = false;
                DrawLine();
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 连接源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private ToolStripMenuItem AddSourceCategory(ToolStripMenuItem parentItem, string text)
        {
            if (parentItem == null)
                return null;

            ToolStripMenuItem item = new ToolStripMenuItem(text);
            item.BackColor = Color.White;
            parentItem.DropDownItems.Add(item);
            return item;
        }

        private void AddSourceItem(ToolStripMenuItem parentItem, string sourceText)
        {
            if (parentItem == null)
                return;

            ToolStripItem item = parentItem.DropDownItems.Add(GetSourceMenuDisplayText(sourceText));
            item.Name = sourceText;
            item.BackColor = Color.White;
            item.Click += new EventHandler(ConnectSource);
        }

        private string GetSourceMenuDisplayText(string sourceText)
        {
            string displayText = sourceText;
            if (displayText.StartsWith("《- "))
                displayText = displayText.Substring(3);

            if (displayText.StartsWith("全局变量->"))
                return displayText.Substring("全局变量->".Length);

            if (displayText.StartsWith("[") && displayText.Contains("]"))
                displayText = displayText.Substring(displayText.IndexOf("]") + 1);

            return displayText;
        }

        private bool CanConnectSourceType(string targetToolName, string targetType, string sourceType)
        {
            ToolInfo targetTool = FindToolInfoByName(targetToolName);
            if (targetTool != null && targetTool.toolType == ToolType.CodeEdit)
                return sourceType == DataType.String.ToString();
            return sourceType == targetType;
        }

        private bool IsCodeEditOutputNode(TreeNode node)
        {
            if (node == null || node.Level != 1 || node.Parent == null || !node.Text.StartsWith("-->"))
                return false;

            ToolInfo toolInfo = FindToolInfoByName(node.Parent.Text);
            return toolInfo != null && toolInfo.toolType == ToolType.CodeEdit;
        }

        private void ConnectSource(object sender, EventArgs e)
        {
            try
            {
                if (IsCodeEditOutputNode(Job.GetJobTree(jobName).SelectedNode))
                    return;

                string sourceText = sender is ToolStripItem ? ((ToolStripItem)sender).Name : sender.ToString();
                if (string.IsNullOrEmpty(sourceText))
                    sourceText = sender.ToString();

                string nodeText = string.Empty;
                if (Job.GetJobTree(jobName).SelectedNode.Level == 1)
                {
                    nodeText = Job.GetJobTree(jobName).SelectedNode.Parent.Text;
                }
                else
                {
                    nodeText = Job.GetJobTree(jobName).SelectedNode.Text;
                }

                string input = Job.GetJobTree(jobName).SelectedNode.Text;
                string inputName = input.Substring(3);
                int sourceIndex = inputName.IndexOf("《- ");
                if (sourceIndex >= 0)
                    inputName = inputName.Substring(0, sourceIndex);
                SyncCodeEditInputSource(nodeText, inputName, sourceText);
                if (Job.GetJobTree(jobName).SelectedNode.Text.Contains("《"))       //表示已经连接了源
                {
                    string oldSource = Regex.Split(input.Substring(3), "《- ")[1];
                    string oldSourceTool = Regex.Split(oldSource, "->")[0];
                    string oldSourceIO = Regex.Split(oldSource, "->")[1];

                    string newSource = sourceText.Substring(3);
                    string newSourceTool = Regex.Split(newSource, "->")[0];
                    string newSourceIO = Regex.Split(newSource, "->")[1];

                    //移除旧的连线，并新增新的连线
                    for (int i = 0; i < D_itemAndSource.Count; i++)
                    {
                        if (((TreeNode)GetToolIONodeByNodeText(nodeText, input)) == (TreeNode)D_itemAndSource.Keys.ToArray()[i] && ((TreeNode)GetToolIONodeByNodeText(oldSourceTool, "-->" + oldSourceIO)) == (TreeNode)D_itemAndSource[D_itemAndSource.Keys.ToArray()[i]])
                        {
                            D_itemAndSource.Remove(D_itemAndSource.Keys.ToArray()[i]);
                            break;
                        }
                    }

                    FindToolInfoByName(nodeText).GetInput(Regex.Split(input.Substring(3), "《")[0]).value = sourceText;
                    Job.GetJobTree(jobName).SelectedNode.Text = Regex.Split(input, "《")[0] + sourceText;
                    FindToolInfoByName(nodeText).GetInput(Regex.Split(input.Substring(3), "《")[0]).value = sourceText;
                    Application.DoEvents();

                    //添加新的连线
                    D_itemAndSource.Add(((TreeNode)GetToolIONodeByNodeText(nodeText, Regex.Split(input, "《")[0] + sourceText)), ((TreeNode)GetToolIONodeByNodeText(newSourceTool, "-->" + newSourceIO)));
                }
                else
                {
                    // FindToolInfoByName(nodeText).GetInput(input).value = sender.ToString();
                    Job.GetJobTree(jobName).SelectedNode.Text = input + sourceText;
                    FindToolInfoByName(nodeText).GetInput(input.Substring(3)).value = sourceText;

                    string toolNodeText = Regex.Split(sourceText, "->")[0].Substring(3);
                    string toolIONodeText = "-->" + Regex.Split(sourceText, "->")[1];
                    D_itemAndSource.Add(Job.GetJobTree(jobName).SelectedNode, GetToolIONodeByNodeText(toolNodeText, toolIONodeText));
                    DrawLine();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void SyncCodeEditInputSource(string toolName, string inputName, string sourceText)
        {
            ToolInfo toolInfo = FindToolInfoByName(toolName);
            if (toolInfo == null || toolInfo.toolType != ToolType.CodeEdit)
                return;

            CodeEditTool codeEditTool = toolInfo.tool as CodeEditTool;
            if (codeEditTool == null)
                return;

            for (int i = 0; i < codeEditTool.L_inputItems.Count; i++)
            {
                CodeInputItem item = codeEditTool.L_inputItems[i];
                if (item == null || item.InputName != inputName)
                    continue;

                item.SourceType = CodeInputSourceType.变量;
                item.FixedValue = string.Empty;
                item.VariableSource = sourceText;
                item.ValueType = ResolveCodeEditSourceValueType(sourceText);
                Frm_CodeEditTool.SyncInputSourceFromFlow(jobName, toolName, inputName, sourceText);
                break;
            }
        }

        private void SyncCodeEditInputsBeforeRun(ToolInfo toolInfo, CodeEditTool codeEditTool)
        {
            if (toolInfo == null || codeEditTool == null)
                return;

            for (int i = 0; i < toolInfo.input.Count; i++)
            {
                ToolIO flowInput = toolInfo.input[i];
                string source = flowInput.value == null ? string.Empty : flowInput.value.ToString();
                if (string.IsNullOrEmpty(source))
                    continue;

                for (int j = 0; j < codeEditTool.L_inputItems.Count; j++)
                {
                    CodeInputItem scriptInput = codeEditTool.L_inputItems[j];
                    if (scriptInput == null || scriptInput.InputName != flowInput.IOName)
                        continue;

                    scriptInput.SourceType = CodeInputSourceType.变量;
                    scriptInput.FixedValue = string.Empty;
                    scriptInput.VariableSource = source;
                    scriptInput.ValueType = ResolveCodeEditSourceValueType(source);
                    break;
                }
            }
        }

        internal CodeValueType ResolveCodeEditSourceValueType(string sourceText)
        {
            if (string.IsNullOrEmpty(sourceText))
                return CodeValueType.String;

            string[] parts = sourceText.Split(new string[] { "->" }, StringSplitOptions.None);
            if (parts.Length < 2)
                return CodeValueType.String;

            string sourceToolName = parts[0].Trim();
            if (sourceToolName.StartsWith("《-"))
                sourceToolName = sourceToolName.Substring(3).Trim();
            string outputName = parts[1].Trim();

            if (sourceToolName == "全局变量")
            {
                for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
                {
                    Variable variable = Project.Instance.curEngine.globelVariable.L_variable[i];
                    if (variable.name == outputName)
                        return ParseCodeValueType(variable.type, variable.value);
                }
                return CodeValueType.String;
            }

            string sourceJobName = jobName;
            if (sourceToolName.StartsWith("[") && sourceToolName.Contains("]"))
            {
                int endIndex = sourceToolName.IndexOf("]");
                sourceJobName = sourceToolName.Substring(1, endIndex - 1);
                sourceToolName = sourceToolName.Substring(endIndex + 1);
            }

            Job sourceJob = Project.Instance.curEngine.FindJobByName(sourceJobName);
            ToolInfo sourceTool = sourceJob == null ? null : sourceJob.FindToolInfoByName(sourceToolName);
            if (sourceTool == null)
                return CodeValueType.String;

            CodeEditTool sourceCodeTool = sourceTool.tool as CodeEditTool;
            if (sourceCodeTool != null)
            {
                for (int i = 0; i < sourceCodeTool.L_outputItems.Count; i++)
                    if (sourceCodeTool.L_outputItems[i].OutputName == outputName)
                        return sourceCodeTool.L_outputItems[i].ValueType;
            }

            return ParseCodeValueType(string.Empty, sourceTool.GetOutput(outputName).value);
        }

        private CodeValueType ParseCodeValueType(string configuredType, object value)
        {
            if (configuredType == "Int") return CodeValueType.Int;
            if (configuredType == "Double") return CodeValueType.Double;
            if (configuredType == "Bool") return CodeValueType.Bool;
            if (configuredType == "String") return CodeValueType.String;
            if (value is bool) return CodeValueType.Bool;
            if (value is byte || value is sbyte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong)
                return CodeValueType.Int;
            if (value is float || value is double || value is decimal)
                return CodeValueType.Double;
            return CodeValueType.String;
        }

        #endregion

        #region 工具相关

        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunTool(object sender, EventArgs e)
        {
            try
            {
                FindToolByName(jobName, Job.GetJobTree(jobName).SelectedNode.Text).Run(true, true, string.Empty);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 重命名工具                
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameTool(object sender, EventArgs e)
        {
            try
            {
                GetJobTree(jobName).LabelEdit = true;
                GetJobTree(jobName).SelectedNode.BeginEdit();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 被复制的工具
        /// </summary>
        private ToolInfo toolInfoCopied = new ToolInfo();
        public static class ObjectCopierToolInfo
        {
            public static ToolInfo Clone<ToolInfo>(ToolInfo source)
            {
                if (!typeof(ToolInfo).IsSerializable)
                {
                    throw new ArgumentException("The type must be serializable.", "source");
                }

                if (Object.ReferenceEquals(source, null))
                {
                    return default(ToolInfo);
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (ToolInfo)formatter.Deserialize(stream);
                }
            }
        }
        /// <summary>
        /// 复制工具                
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyTool(object sender, EventArgs e)
        {
            try
            {
                toolInfoCopied = ObjectCopierToolInfo.Clone(FindToolInfoByName(GetJobTree(jobName).SelectedNode.Text));               //此处应该是深拷贝
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /* 利用反射实现深拷贝*/
        public static object DeepCopy(object _object)
        {
            Type T = _object.GetType();
            object o = Activator.CreateInstance(T);
            PropertyInfo[] PI = T.GetProperties();
            for (int i = 0; i < PI.Length; i++)
            {
                PropertyInfo P = PI[i];
                P.SetValue(o, P.GetValue(_object, null), null);
            }
            return o;
        }
        /// <summary>
        /// 粘贴工具                
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasteTool(object sender, EventArgs e)
        {
            try
            {
                Frm_ToolBox.Instance.AddTool(toolInfoCopied.toolType.ToString(), toolInfoCopied, GetJobTree(jobName).SelectedNode.Index);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 粘贴工具                
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasteToolAtLast(object sender, EventArgs e)
        {
            try
            {
                Frm_ToolBox.Instance.AddTool(toolInfoCopied.toolType.ToString(), toolInfoCopied, GetJobTree(jobName).Nodes.Count - 1);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 插入工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertTool(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;
                Frm_ToolBox.Instance.AddTool(((ToolStripItem)sender).Text, null, Job.GetJobTree(jobName).SelectedNode.Index);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 启用/忽略工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowIOForm(object sender, EventArgs e)
        {
            Frm_IOConfig.b = false;
            Frm_IOConfig.Instance.comboBox1.Clear();
            for (int i = 0; i < Project.Instance.curEngine.FindJobByName(jobName).L_toolList.Count; i++)
            {
                Frm_IOConfig.Instance.comboBox1.Add(Project.Instance.curEngine.FindJobByName(jobName).L_toolList[i].toolName);
            }
            ShowIOEdit(Job.GetJobTree(jobName).SelectedNode.Text);
            Frm_IOConfig.b = true;
        }
        internal void ShowIOEdit(string toolName)
        {
            try
            {
                ToolInfo toolInfo = Project.Instance.curEngine.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text).FindToolInfoByName(toolName);
                Frm_IOConfig.result1 = GetToolParForIOEdit(toolInfo);
                Frm_IOConfig.Instance.jobName = this.jobName;
                Frm_IOConfig.Instance.Show();

                Frm_IOConfig.Instance.comboBox1.TextStr = toolName;
                Frm_IOConfig.Instance.Load();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private ToolParBase GetToolParForIOEdit(ToolInfo toolInfo)
        {
            if (toolInfo == null || toolInfo.tool == null)
                return new ToolParBase();

            FieldInfo toolParField = toolInfo.tool.GetType().GetField("toolPar", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (toolParField == null)
                return new ToolParBase();

            ToolParBase toolPar = toolParField.GetValue(toolInfo.tool) as ToolParBase;
            return toolPar == null ? new ToolParBase() : toolPar;
        }
        /// <summary>
        /// 启用/忽略工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableOrDisenableTool(object sender, EventArgs e)
        {
            try
            {
                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                FindToolInfoByName(Job.GetJobTree(jobName).SelectedNode.Text).enable = !FindToolInfoByName(Job.GetJobTree(jobName).SelectedNode.Text).enable;
                if (FindToolInfoByName(Job.GetJobTree(jobName).SelectedNode.Text).enable)
                    (Job.GetJobTree(jobName).SelectedNode).ForeColor = Color.Black;
                else
                    (Job.GetJobTree(jobName).SelectedNode).ForeColor = Color.DarkGray;

                GetJobTree().SelectedNode = null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 通过流程名和工具名获取工具
        /// </summary>
        /// <param name="jobName">流程名</param>
        /// <param name="toolName">工具名</param>
        /// <returns></returns>
        internal static ToolBase FindToolByName(string jobName, string toolName)
        {
            try
            {
                Job job = new Job();
                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    if (Project.Instance.curEngine.L_jobList[i].jobName == jobName)
                    {
                        job = Project.Instance.curEngine.L_jobList[i];
                        break;
                    }
                }
                for (int i = 0; i < job.L_toolList.Count; i++)
                {
                    if (job.L_toolList[i].toolName == toolName)
                    {
                        return job.L_toolList[i].tool;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 查找指定工具
        /// </summary>
        /// <param name="outputItem">工具名称</param>
        public ToolBase FindToolByName(string toolName)
        {
            try
            {
                //寻找输出工具
                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolName == toolName)
                    {
                        return L_toolList[i].tool;
                    }
                }
                return new ToolBase();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new ToolBase();
            }
        }
        /// <summary>
        /// 通过TreeNode节点文本获取节点
        /// </summary>
        /// <param name="nodeText">节点文本</param>
        /// <returns>节点对象</returns>
        internal TreeNode GetToolNodeByNodeText(string nodeText)
        {
            try
            {
                foreach (TreeNode toolNode in Job.GetJobTree(jobName).Nodes)
                {
                    if (((TreeNode)toolNode).Text != nodeText)
                    {
                        foreach (TreeNode itemNode in ((TreeNode)toolNode).Nodes)
                        {
                            if (((TreeNode)itemNode).Text.Substring(3) == nodeText)
                            {
                                return itemNode;
                            }
                        }
                    }
                    else
                    {
                        return toolNode;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 通过TreeNode节点文本获取输入输出节点
        /// </summary>
        /// <param name="toolName">工具名称</param>
        /// <returns>IO名称</returns>
        internal TreeNode GetToolIONodeByNodeText(string toolName, string toolIOName)
        {
            try
            {
                foreach (TreeNode toolNode in Job.GetJobTree(jobName).Nodes)
                {
                    if (toolNode.Text == toolName)
                    {
                        foreach (TreeNode itemNode in ((TreeNode)toolNode).Nodes)
                        {
                            if (((TreeNode)itemNode).Text == toolIOName)
                            {
                                return itemNode;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }

        #endregion


        private void ShowIO(ToolStripMenuItem toolStripMenuItem, object t)
        {
            PropertyInfo[] dd = t.GetType().GetProperties();
            foreach (PropertyInfo pi in t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                string temp = Regex.Split(pi.ToString(), " ")[0];
                Type propertyType = pi.PropertyType;
                //if (temp == "HalconDotNet.HObject"
                //    || temp == "VisionAndMotionPro.XYU"
                //    )
                //{
                string name = pi.Name;//获得属性的名字,后面就可以根据名字判断来进行些自己想要的操作

                ToolStripItem toolStripItem = toolStripMenuItem.DropDownItems.Add(pi.Name);
                toolStripItem.Name = pi.Name;
                toolStripItem.Image = Resources.Image;




                if (propertyType == typeof(XYU) || propertyType == typeof(List<XYU>))
                    toolStripItem.Tag = DataType.Pose;
                else if (temp == "VisionAndMotionPro.HObject")
                    toolStripItem.Tag = DataType.Image;
                else if (temp == "VisionAndMotionPro.Point")
                    toolStripItem.Tag = DataType.XY;
                else if (temp == "Double")
                    toolStripItem.Tag = DataType.String;
                else if (temp == "HalconDotNet.HObject")
                    toolStripItem.Tag = DataType.Image;
                else if (temp == "Int32")
                    toolStripItem.Tag = DataType.String;




                //////toolStripItem.Click += new EventHandler(Add_output);

                // List<XYU> 本身就是可连接的位姿结果，不能再递归展开为 List 的内部属性。
                if (propertyType == typeof(List<XYU>))
                    continue;

                object value = pi.GetValue(t, null);

                if (value == null || value.ToString() == "HalconDotNet.HObject")
                    continue;

                if (pi.PropertyType.IsValueType || pi.PropertyType.Name.StartsWith("String"))
                {

                }
                else
                {
                    ShowIO((ToolStripMenuItem)toolStripItem, (value));
                }

                //}
            }
        }


        private void tvw_tools_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isRunLoop)
                {
                    GetJobTree(jobName).ContextMenuStrip = null;
                    return;
                }

                //if (e.Button == MouseButtons.Right)
                //{
                TreeNode tn = GetJobTree(jobName).GetNodeAt(e.X, e.Y);
                if (tn != null)
                {
                    GetJobTree(jobName).SelectedNode = tn;
                }
                //}

                if (e.Y > GetJobTree(jobName).Nodes[GetJobTree(jobName).Nodes.Count - 1].Bounds.Y + 10)
                {
                    GetJobTree(jobName).ContextMenuStrip = rightClickMenuAtBlank;
                }


            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 右键流程节点"保存当前流程"
        /// </summary>
        private void SaveCurrentJob(object sender, EventArgs e)
        {
            SaveCurrentJob();
        }

        /// <summary>
        /// 保存当前选中流程的所有参数（含流程内全部工具的模板参数、显示参数、搜索区域等）
        /// </summary>
        public static void SaveCurrentJob()
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                if (Frm_Job.Instance.tbc_jobs.TabPages.Count == 0)
                {
                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "No jobs to save" : "没有可保存的流程", Color.Red);
                    return;
                }
                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                Job job = FindJobByName(jobName);
                if (job == null)
                    return;

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + job.jobName + ".job", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, job);
                stream.Close();

                Log.SaveLog(LogType.Operate, Project.Instance.configuration.language == Language.English ? "Program saved successfully" : "程序保存成功");
                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Program saved successfully" : "流程保存成功", Color.Green);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        bool doubleClick = false;

        public static void Delay(double t)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Restart();
            while (stopWatch.Elapsed.TotalMilliseconds < t)
            {
                Application.DoEvents();
            }
        }

        //////    PropertyInfo[] props = null;

        //////    string str = "System.String";
        //////    String exp = (String)Activator.CreateInstance(clas);
        //////    //如果你要得到这个类的类型信息可以这么做
        //////    Type expType = Type.GetType(str);

        //////    Type type = typeof(clas);   
        //////    object obj = Activator.CreateInstance(type);
        //////    props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //////    for (int i = 0; i < props.Length; i++)
        //////    {
        //////        ToolStripItem toolStripItem = toolStripMenuItem.DropDownItems.Add(props[i].Name);
        //////        ShowIO(toolStripItem, props[i]);
        //////    }

        //////}

        internal void TVW_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (isRunLoop)
                    return;

                Thread th = new Thread(() =>
                {

                    doubleClick = false;
                    Thread.Sleep(500);
                    if (doubleClick)
                        return;

                    TreeNode node = ((TreeView)sender).SelectedNode;
                    if (node == null)
                        return;

                    string toolName = ((TreeView)sender).SelectedNode.Text;

                    for (int i = 0; i < L_toolList.Count; i++)
                    {
                        if (L_toolList[i].toolName == toolName)
                        {
                            switch (L_toolList[i].toolType)
                            {
                                #region ImageAcq
                                case ToolType.ImageAcq:
                                    AcqImageTool acqImageTool = (AcqImageTool)L_toolList[i].tool;
                                    // 当前有效图优先保留；没有图时再从最后预览缓存恢复。
                                    acqImageTool.EnsurePreviewImage();
                                    acqImageTool.UpdateOutput(toolName);
                                    if (acqImageTool.toolPar.ResultPar.图像 != null)
                                        GetImageWindowControl().hwc_imageWindow.HobjectToHimage(acqImageTool.toolPar.ResultPar.图像);
                                    else
                                        GetImageWindowControl().hwc_imageWindow.ClearWindow();


                                    if (!acqImageTool.displayAllImageRegion)
                                    {
                                        GetImageWindowControl().hwc_imageWindow.DispObj(acqImageTool.L_regions[0].getRegion());
                                    }

                                    // 切换工具时同步固定路径配置，防止面板残留上一个工具的目录、修改写错对象
                                    Frm_FromLocal.imageAcqTool = acqImageTool;
                                    if (Frm_FromLocal.CurrentInstance != null)
                                    {
                                        try
                                        {
                                            Frm_FromLocal.Instance.BeginInvoke(new MethodInvoker(delegate
                                            {
                                                if (acqImageTool.imageSourceMode == ImageSourceMode.FromFile)
                                                    Frm_FromLocal.Instance.tbx_imagePath.Text = acqImageTool.imagePath ?? string.Empty;
                                                else if (acqImageTool.imageSourceMode == ImageSourceMode.FromDirectory)
                                                    Frm_FromLocal.Instance.tbx_imageDirectoryPath.Text = acqImageTool.imageDirectoryPath ?? string.Empty;
                                            }));
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.SaveError(ex);
                                        }
                                    }
                                    break;
                                #endregion

                                #region ImagePreprocessing
                                case ToolType.ImagePreprocessing:

                                    ImageProprecessingTool imageProprecessingTool = (ImageProprecessingTool)L_toolList[i].tool;







                                    if (imageProprecessingTool.inputImage != null)
                                        Frm_ImageProprecessingTool.Instance.hWindow_Final1.HobjectToHimage(imageProprecessingTool.inputImage);
                                    else
                                        Frm_ImageProprecessingTool.Instance.hWindow_Final1.ClearWindow();

                                    Frm_ImageProprecessingTool.Instance.dataGridView1.Rows.Clear();
                                    for (int j = 0; j < imageProprecessingTool.L_item.Count; j++)
                                    {
                                        int idx = Frm_ImageProprecessingTool.Instance.dataGridView1.Rows.Add();
                                        ((DataGridViewCheckBoxCell)Frm_ImageProprecessingTool.Instance.dataGridView1.Rows[idx].Cells[0]).Value = imageProprecessingTool.L_item[j].enable;
                                        Frm_ImageProprecessingTool.Instance.dataGridView1.Rows[idx].Cells[1].Value = imageProprecessingTool.L_item[j].type;
                                        Frm_ImageProprecessingTool.Instance.dataGridView1.Rows[idx].Cells[2].Value = imageProprecessingTool.L_item[j].itemName;
                                    }

                                    if (Frm_ImageProprecessingTool.Instance.dataGridView1.Rows.Count > 0)
                                        Frm_ImageProprecessingTool.Instance.dataGridView1.Rows[0].Selected = true;

                                    //if (imageProprecessingTool.SearchRegion != null)
                                    //{
                                    //    Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayROI(imageProprecessingTool.L_regions);
                                    //    Frm_ShapeMatchTool.Instance.regions = imageProprecessingTool.L_regions;
                                    //}

                                    ////显示模板
                                    //try
                                    //{
                                    //    if (imageProprecessingTool.modelID != -1)
                                    //    {
                                    //        HTuple row, col, row1, col1;
                                    //        HOperatorSet.SmallestRectangle1(imageProprecessingTool.totalRegion, out row, out col, out row1, out col1);
                                    //        HObject outRectangle1;
                                    //        HOperatorSet.GenRectangle1(out outRectangle1, row - 30, col - 30, row1 + 30, col1 + 30);
                                    //        HObject imageReduced;
                                    //        HOperatorSet.ReduceDomain(imageProprecessingTool.standardImage, outRectangle1, out imageReduced);
                                    //        HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, row - 30, col - 30, row1 + 30, col1 + 30);
                                    //        HOperatorSet.DispObj(imageReduced, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                                    //        HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("margin"));
                                    //        HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("green"));
                                    //        HOperatorSet.DispObj(imageProprecessingTool.templateRegion, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);

                                    //        int statu = imageProprecessingTool.CreateTemplate();
                                    //        if (statu != 0)
                                    //            return;
                                    //        HObject contour;
                                    //        HOperatorSet.GetShapeModelContours(out contour, imageProprecessingTool.modelID, (HTuple)1);
                                    //        HTuple area1, row2, column2;
                                    //        HOperatorSet.AreaCenter(imageProprecessingTool.totalRegion, out area1, out row2, out column2);
                                    //        HTuple homMat2D;
                                    //        HOperatorSet.HomMat2dIdentity(out homMat2D);
                                    //        HOperatorSet.HomMat2dTranslate(homMat2D, row2, column2, out homMat2D);
                                    //        HOperatorSet.AffineTransContourXld(contour, out contour, homMat2D);
                                    //        HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("orange"));
                                    //        HOperatorSet.DispObj(contour, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                                    //    }
                                    //    else
                                    //    {
                                    //        Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow.ClearWindow();
                                    //    }
                                    //}
                                    //catch { }

                                    //将对象信息更新到界面
                                    //Frm_ShapeMatchTool.Instance.pictureBox2.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                    //Frm_ShapeMatchTool.Instance.ckb_showCross.Checked = shapeMatchTool.showCross;
                                    //Frm_ShapeMatchTool.Instance.cbx_showTemplate.Checked = shapeMatchTool.showTemplate;
                                    //Frm_ShapeMatchTool.Instance.ckb_showFeature.Checked = shapeMatchTool.showFeature;
                                    //Frm_ShapeMatchTool.Instance.cbx_searchRegionType.Text = (shapeMatchTool.searchRegionType == RegionType.None ? "" : shapeMatchTool.searchRegionType.ToString());
                                    //Frm_ShapeMatchTool.Instance.nud_minScore.Value = Convert.ToDecimal(shapeMatchTool.minScore);
                                    //Frm_ShapeMatchTool.Instance.nud_matchNum.Value = Convert.ToDecimal(shapeMatchTool.matchNum);
                                    //Frm_ShapeMatchTool.Instance.nud_angleStart.Value = Convert.ToDecimal(shapeMatchTool.startAngle);
                                    //Frm_ShapeMatchTool.Instance.nud_angleRange.Value = Convert.ToDecimal(shapeMatchTool.angleRange);
                                    //Frm_ShapeMatchTool.Instance.nud_angleStep.Value = Convert.ToDecimal(shapeMatchTool.angleStep);
                                    //Frm_ShapeMatchTool.Instance.tkb_contrast.Value = Convert.ToInt16(shapeMatchTool.contrast);
                                    //Frm_ShapeMatchTool.Instance.cbx_polarity.Text = shapeMatchTool.polarity;
                                    //Frm_ShapeMatchTool.Instance.comboBox1.SelectedIndex = (int)shapeMatchTool.sortMode;
                                    //Frm_ShapeMatchTool.Instance.checkBox1.Checked = shapeMatchTool.showIndex;
                                    //Frm_ShapeMatchTool.Instance.textBox1.Text = shapeMatchTool.spanPixelNum.ToString();

                                    //Frm_ShapeMatchTool.Instance.pictureBox3.Image = (shapeMatchTool.showTemplate ? Resources.复选框 : Resources.去复选框);
                                    //Frm_ShapeMatchTool.Instance.pictureBox4.Image = (shapeMatchTool.showCross ? Resources.复选框 : Resources.去复选框);
                                    //Frm_ShapeMatchTool.Instance.pictureBox5.Image = (shapeMatchTool.showFeature ? Resources.复选框 : Resources.去复选框);
                                    //Frm_ShapeMatchTool.Instance.pictureBox8.Image = (shapeMatchTool.showIndex ? Resources.复选框 : Resources.去复选框);
                                    //Frm_ShapeMatchTool.Instance.pictureBox9.Image = (shapeMatchTool.showSearchRegion ? Resources.复选框 : Resources.去复选框);

                                    //if (shapeMatchTool.angleStep == 0)
                                    //{
                                    //    Frm_ShapeMatchTool.Instance.nud_angleStep.Enabled = false;
                                    //    Frm_ShapeMatchTool.Instance.ckb_autoStep.Checked = true;
                                    //}
                                    //else
                                    //{
                                    //    Frm_ShapeMatchTool.Instance.ckb_autoStep.Checked = false;
                                    //}

                                    //Frm_ShapeMatchTool.Instance.tbc_shapeMatch.SelectedIndex = 0;


                                    //if (shapeMatchTool.modelID == -1)
                                    //{
                                    //    Frm_ShapeMatchTool.Instance.panel4.Visible = false;
                                    //    Frm_ShapeMatchTool.Instance.panel17.Visible = false;
                                    //    Frm_ShapeMatchTool.Instance.button5.Visible = false;
                                    //    Frm_ShapeMatchTool.Instance.button9.Visible = false;
                                    //}
                                    //else
                                    //{
                                    //    Frm_ShapeMatchTool.Instance.panel4.Visible = true;
                                    //    Frm_ShapeMatchTool.Instance.panel17.Visible = true;
                                    //    Frm_ShapeMatchTool.Instance.button5.Visible = true;
                                    //    Frm_ShapeMatchTool.Instance.button9.Visible = true;
                                    //}
                                    //Frm_ShapeMatchTool.Instance.hWindow_Final1.DispImageFit();




                                    break;
                                #endregion

                                //////#region ColorToRGB
                                //////case ToolType.ColorToRGB:
                                //////    Frm_ColorToRGBTool.Instance.lbl_title.Text = string.Format("彩图转RGB图    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_ColorToRGBTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_ColorToRGBTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ShapeMatchTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_ColorToRGBTool.Instance.TopMost = true;
                                //////    Frm_ColorToRGBTool.Instance.Activate();

                                //////    Frm_ColorToRGBTool.Instance.jobName = this.jobName;
                                //////    Frm_ColorToRGBTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_ColorToRGBTool.Instance.Show();
                                //////    Frm_ColorToRGBTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_ColorToRGBTool.Instance.btn_runColorToRGBTool.Focus();
                                //////    ColorToRGBTool colorToRGBTool = (ColorToRGBTool)(L_toolList[i].tool);
                                //////    Frm_ColorToRGBTool.colorToRGBTool = colorToRGBTool;
                                //////    Application.DoEvents();

                                //////    if (colorToRGBTool.inputImage != null)
                                //////        colorToRGBTool.ShowImage(colorToRGBTool.inputImage);
                                //////    else
                                //////    { }
                                //////    //////colorToRGBTool.ClearWindow(this.jobName);

                                //////    //将对象信息更新到界面
                                //////    break;
                                //////#endregion

                                //////#region SaveImage
                                //////case ToolType.SaveImage:
                                //////    Frm_SaveImageTool.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "SDK_PointGray - " : string.Format("存储图像    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName));
                                //////    //Frm_SaveImageTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_SaveImageTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_SaveImageTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_SaveImageTool.Instance.TopMost = true;
                                //////    Frm_SaveImageTool.Instance.Activate();
                                //////    Frm_SaveImageTool.Instance.jobName = this.jobName;
                                //////    Frm_SaveImageTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_SaveImageTool.saveImageTool = (SaveImageTool)FindToolByName(L_toolList[i].toolName);
                                //////    Frm_SaveImageTool.Instance.jobName = this.jobName;
                                //////    Frm_SaveImageTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_SaveImageTool.saveImageTool = ((SaveImageTool)Job.FindToolByName(this.jobName, L_toolList[i].toolName));
                                //////    Frm_SaveImageTool.Instance.Show();
                                //////    Frm_SaveImageTool.Instance.WindowState = FormWindowState.Normal;
                                //////    ////Frm_SaveImageTool.Instance.btn_runSDKHIKVisionTool.Focus();
                                //////    SaveImageTool saveImageTool = (SaveImageTool)(L_toolList[i].tool);
                                //////    Application.DoEvents();




                                //////    //将对象信息更新到界面
                                //////    Frm_SaveImageTool.Instance.pictureBox2.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                //////    Frm_SaveImageTool.Instance.tbx_imageSavePath.TextStr = saveImageTool.imageSavePath;
                                //////    Frm_SaveImageTool.Instance.comboBox1.TextStr = saveImageTool.imageFormat;
                                //////    Frm_SaveImageTool.Instance.textBox1.Value = saveImageTool.saveDays;
                                //////    Frm_SaveImageTool.Instance.checkBox1.Checked = saveImageTool.expandTime;
                                //////    Frm_SaveImageTool.Instance.checkBox2.Checked = saveImageTool.autoClear;
                                //////    Frm_SaveImageTool.Instance.textBox2.TextStr = saveImageTool.imageName;
                                //////    Frm_SaveImageTool.Instance.checkBox3.Checked = saveImageTool.autoCreateDirectory;
                                //////    Frm_SaveImageTool.Instance.radioButton1.Checked = (saveImageTool.imageSource == ImageSource.InputImage ? true : false);
                                //////    Frm_SaveImageTool.Instance.radioButton2.Checked = (saveImageTool.imageSource == ImageSource.InputImage ? false : true);
                                //////    Frm_SaveImageTool.Instance.pictureBox8.Image = (saveImageTool.imageSource == ImageSource.InputImage ? Resources.勾选 : Resources.去勾选);
                                //////    Frm_SaveImageTool.Instance.pictureBox7.Image = (saveImageTool.imageSource == ImageSource.WindowImage ? Resources.勾选 : Resources.去勾选);
                                //////    break;
                                //////#endregion

                                #region ShapeMatch
                                case ToolType.Match:
                                    MatchTool matchTool = (MatchTool)L_toolList[i].tool;

                                    if (matchTool.toolPar.InputPar.图像 != null)
                                        Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(matchTool.toolPar.InputPar.图像);
                                    else
                                        Frm_ShapeMatchTool.Instance.hWindow_Final1.ClearWindow();

                                    if (matchTool.SearchRegion != null)
                                    {
                                        Frm_ShapeMatchTool.Instance.hWindow_Final1.DispObj(matchTool.L_regions[0].getRegion());
                                    }





                                    Frm_ShapeMatchTool.Instance.hWindow_Final1.DispImageFit();

                                    matchTool.Run(true, false, toolName);


                                    break;
                                #endregion

                                //////#region BlobAnalyse
                                //////case ToolType.BlobAnalyse:
                                //////    Frm_BlobAnalyseTool.Instance.pictureBox1.Image = Resources.BlobAnalyseTool;
                                //////    Frm_BlobAnalyseTool.Instance.lbl_title.Text = string.Format("斑点分析    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_BlobAnalyseTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_BlobAnalyseTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_BlobAnalyseTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_BlobAnalyseTool.Instance.TopMost = true;
                                //////    Frm_BlobAnalyseTool.Instance.Activate();
                                //////    Frm_BlobAnalyseTool.Instance.jobName = this.jobName;
                                //////    Frm_BlobAnalyseTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_BlobAnalyseTool.Instance.Show();
                                //////    Frm_BlobAnalyseTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //////Frm_BlobAnalyseTool.Instance.btn_runTool.Focus();
                                //////    BlobAnalyseTool blobAnalyseTool = (BlobAnalyseTool)(L_toolList[i].tool);
                                //////    Frm_BlobAnalyseTool.blobAnalyseTool = blobAnalyseTool;
                                //////    Frm_ProcessingItem.blobAnalyseTool = blobAnalyseTool;
                                //////    Frm_ProcessingItem1.blobAnalyseTool = blobAnalyseTool;
                                //////    Application.DoEvents();

                                //////    if (blobAnalyseTool.toolPar.InputPar.图像 != null)
                                //////        Frm_BlobAnalyseTool.Instance.hWindow_Final1.HobjectToHimage(blobAnalyseTool.toolPar.InputPar.图像);
                                //////    else
                                //////        Frm_BlobAnalyseTool.Instance.hWindow_Final1.ClearWindow();

                                //////    Frm_BlobAnalyseTool.Instance.hWindow_Final1.viewWindow.displayROI(blobAnalyseTool.L_regions);

                                //////    Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows.Clear();
                                //////    for (int j = 0; j < blobAnalyseTool.L_select.Count; j++)
                                //////    {
                                //////        int index = Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows.Add();
                                //////        Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[index].Cells[0].Value = blobAnalyseTool.L_select[j].SelectType;
                                //////        Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[index].Cells[1].Value = blobAnalyseTool.L_select[j].AreaDownLimit;
                                //////        Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[index].Cells[2].Value = blobAnalyseTool.L_select[j].AreaUpLimit;
                                //////    }

                                //////    //将预处理项更新到窗体
                                //////    Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Clear();
                                //////    for (int j = 0; j < blobAnalyseTool.L_prePorcessing.Count; j++)
                                //////    {
                                //////        int index = Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Add();
                                //////        Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[0].Value = blobAnalyseTool.L_prePorcessing[j].PreProcessingType;
                                //////        ((DataGridViewCheckBoxCell)Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[1]).Value = blobAnalyseTool.L_prePorcessing[j].Enable;
                                //////    }

                                //////    //////Frm_BlobAnalyseTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;
                                //////    Frm_BlobAnalyseTool.Instance.ckb_displaySearchRegion.Checked = blobAnalyseTool.displaySearchRegion;
                                //////    Frm_BlobAnalyseTool.Instance.ckb_displayCross.Checked = blobAnalyseTool.displayCross;
                                //////    Frm_BlobAnalyseTool.Instance.tbx_lineWidth.Text = blobAnalyseTool.lineWidth.ToString();
                                //////    Frm_BlobAnalyseTool.Instance.rdo_outCircleFillMode.Checked = blobAnalyseTool.outCircleDrawMode == FillMode.Fill ? true : false;
                                //////    Frm_BlobAnalyseTool.Instance.rdo_outCircleMarginMode.Checked = blobAnalyseTool.outCircleDrawMode == FillMode.Fill ? false : true;
                                //////    Frm_BlobAnalyseTool.Instance.rdo_regionFillMode.Checked = blobAnalyseTool.regionDrawMode == FillMode.Fill ? true : false;
                                //////    Frm_BlobAnalyseTool.Instance.rdo_regionMarginMode.Checked = blobAnalyseTool.regionDrawMode == FillMode.Fill ? false : true;
                                //////    Frm_BlobAnalyseTool.Instance.ckb_displayRegion.Checked = blobAnalyseTool.displayRegion;
                                //////    Frm_BlobAnalyseTool.Instance.rdo_outCircleFillMode.Checked = blobAnalyseTool.outCircleDrawMode == FillMode.Fill ? true : false;
                                //////    Frm_BlobAnalyseTool.Instance.ckb_DisplayOutCircle.Checked = blobAnalyseTool.displayOutCircle;
                                //////    Frm_BlobAnalyseTool.Instance.comboBox1.SelectedIndex = (int)blobAnalyseTool.sortMode;
                                //////    Frm_BlobAnalyseTool.Instance.textBox1.Value = blobAnalyseTool.spanPixelNum;
                                //////    Frm_BlobAnalyseTool.Instance.cbx_searchRegionType.TextStr = blobAnalyseTool.searchRegionType.ToString();
                                //////    Frm_BlobAnalyseTool.Instance.trackBar1.Value = (blobAnalyseTool.minThreshold);
                                //////    Frm_BlobAnalyseTool.Instance.trackBar2.Value = (blobAnalyseTool.maxThreshold);
                                //////    Frm_BlobAnalyseTool.Instance.numericUpDown1.Value = blobAnalyseTool.minThreshold;
                                //////    Frm_BlobAnalyseTool.Instance.numericUpDown2.Value = blobAnalyseTool.maxThreshold;
                                //////    Frm_BlobAnalyseTool.Instance.rdo_regionFillMode.Checked = blobAnalyseTool.regionDrawMode == FillMode.Fill ? true : false;
                                //////    break;
                                //////#endregion


                                //////#region ApplyTrans
                                //////case ToolType.QuoteTrans:
                                //////    Frm_ApplyTransTool.Instance.lbl_title.Text = string.Format("引用标定    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //////Frm_EyeHandCalibTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //////Frm_EyeHandCalibTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_EyeHandCalibTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_EyeHandCalibTool.Instance.TopMost = true;
                                //////    Frm_ApplyTransTool.Instance.Activate();
                                //////    Frm_ApplyTransTool.Instance.jobName = this.jobName;
                                //////    Frm_ApplyTransTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_ApplyTransTool.Instance.Show();
                                //////    Frm_ApplyTransTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_ApplyTransTool.Instance.Focus();
                                //////    QuoteTransTool applyTransTool = (QuoteTransTool)(L_toolList[i].tool);
                                //////    Frm_ApplyTransTool.applyTransTool = applyTransTool;
                                //////    Application.DoEvents();


                                //////    //将对象信息更新到界面
                                //////    Frm_ApplyTransTool.Instance.pictureBox8.Image = L_toolList[i].enable ? Resources.Enable : Resources.Disable;


                                //////    Frm_ApplyTransTool.Instance.cbx_jobList.Items.Clear();
                                //////    for (int j = 0; j < Project.Instance.curEngine.L_jobList.Count; j++)
                                //////    {
                                //////        for (int k = 0; k < Project.Instance.curEngine.L_jobList[j].L_toolList.Count; k++)
                                //////        {
                                //////            if (Project.Instance.curEngine.L_jobList[j].L_toolList[k].toolType == ToolType.EyeHandCalib)
                                //////            {
                                //////                Frm_ApplyTransTool.Instance.cbx_jobList.Items.Add(Project.Instance.curEngine.L_jobList[j].jobName + " . " + Project.Instance.curEngine.L_jobList[j].L_toolList[k].toolName);
                                //////            }
                                //////        }
                                //////    }
                                //////    Frm_ApplyTransTool.Instance.cbx_jobList.Text = applyTransTool.cliperNum;
                                //////    Frm_ApplyTransTool.Instance.textBox6.Text = applyTransTool.photoPos.ToString();
                                //////    break;
                                //////#endregion

                                //////#region OneKeyEyeHandCalib
                                //////case ToolType.OneKeyEyeHandCalib:
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.lbl_title.Text = string.Format("一键手眼标定    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_OneKeyEyeHandCalibTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_OneKeyEyeHandCalibTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_OneKeyEyeHandCalibTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_OneKeyEyeHandCalibTool.Instance.TopMost = true;
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.Activate();
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.jobName = this.jobName;
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.Show();
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.Focus();
                                //////    OneKeyEyeHandCalibTool oneKeyEyeHandCalibTool = (OneKeyEyeHandCalibTool)(L_toolList[i].tool);
                                //////    Frm_OneKeyEyeHandCalibTool.oneKeyEyeHandCalibTool = oneKeyEyeHandCalibTool;
                                //////    Application.DoEvents();

                                //////    if (oneKeyEyeHandCalibTool.inputImage != null)
                                //////        oneKeyEyeHandCalibTool.ShowImage(oneKeyEyeHandCalibTool.inputImage);
                                //////    else
                                //////        oneKeyEyeHandCalibTool.ClearWindow();

                                //////    //将对象信息更新到界面
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.cbo_calibType.Text = (oneKeyEyeHandCalibTool.calibType == CalibType.Four_Point ? "四点标定" : "九点标定");
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.tbx_translateX.Text = oneKeyEyeHandCalibTool.TranslateX.ToString();
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.tbx_translateY.Text = oneKeyEyeHandCalibTool.TranslateY.ToString();
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.tbx_scaleX.Text = oneKeyEyeHandCalibTool.ScanX.ToString();
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.tbx_scaleY.Text = oneKeyEyeHandCalibTool.ScanY.ToString();
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.tbx_rotation.Text = oneKeyEyeHandCalibTool.Rotation.ToString();
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.tbx_theta.Text = oneKeyEyeHandCalibTool.Theta.ToString();
                                //////    Application.DoEvents();

                                //////    //显示标定数据
                                //////    for (int j = 0; j < oneKeyEyeHandCalibTool.L_calibData.Count; j++)
                                //////    {
                                //////        for (int k = 0; k < 4; k++)
                                //////        {
                                //////            Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[j].Cells[k].Value = oneKeyEyeHandCalibTool.L_calibData[j][k];
                                //////        }
                                //////    }

                                //////    Frm_OneKeyEyeHandCalibTool.Instance.cbx_jobList.Items.Clear();
                                //////    for (int j = 0; j < Project.Instance.curEngine.L_jobList.Count; j++)
                                //////    {
                                //////        Frm_OneKeyEyeHandCalibTool.Instance.cbx_jobList.Items.Add(Project.Instance.curEngine.L_jobList[j].jobName);
                                //////    }
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.cbx_jobList.Text = oneKeyEyeHandCalibTool.calibJobName;
                                //////    Frm_OneKeyEyeHandCalibTool.Instance.cbx_outputItemList.Text = oneKeyEyeHandCalibTool.calibItemName;
                                //////    break;
                                //////#endregion

                                //////#region OneDimensionalCalib
                                //////case ToolType.OneDimensionalCalib:
                                //////    Frm_OneDimensionalCalibTool.Instance.lbl_title.Text = string.Format("一维标定    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_OneDimensionalCalibTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_OneDimensionalCalibTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_OneDimensionalCalibTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_OneDimensionalCalibTool.Instance.TopMost = true;
                                //////    Frm_OneDimensionalCalibTool.Instance.Activate();
                                //////    Frm_OneDimensionalCalibTool.Instance.jobName = this.jobName;
                                //////    Frm_OneDimensionalCalibTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_OneDimensionalCalibTool.Instance.Show();
                                //////    Frm_OneDimensionalCalibTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_OneDimensionalCalibTool.Instance.Focus();
                                //////    OneDimensionalCalibTool oneDimensionalCalibrationTool = (OneDimensionalCalibTool)(L_toolList[i].tool);
                                //////    Frm_OneDimensionalCalibTool.oneDimensionalCalibTool = oneDimensionalCalibrationTool;
                                //////    Application.DoEvents();

                                //////    //将对象信息更新到界面
                                //////    Frm_OneDimensionalCalibTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;
                                //////    Frm_OneDimensionalCalibTool.Instance.tbx_translate.Text = oneDimensionalCalibrationTool.Translate.ToString();
                                //////    Frm_OneDimensionalCalibTool.Instance.tbx_scale.Text = oneDimensionalCalibrationTool.Scan.ToString();
                                //////    Application.DoEvents();

                                //////    //显示标定数据
                                //////    for (int j = 0; j < ((OneDimensionalCalibTool)L_toolList[i].tool).L_calibData.Count; j++)
                                //////    {
                                //////        for (int k = 0; k < 2; k++)
                                //////        {
                                //////            Frm_OneDimensionalCalibTool.Instance.dgv_calibrateData.Rows[j].Cells[k].Value = oneDimensionalCalibrationTool.L_calibData[j][k];
                                //////        }
                                //////    }

                                //////    break;
                                //////#endregion

                                //////#region UpCamAlign
                                //////case ToolType.UpCamAlign:
                                //////    Frm_UpCamAlignTool.Instance.lbl_title.Text = string.Format("上相机定位    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_UpCamAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_UpCamAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_UpCamAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_UpCamAlignTool.Instance.TopMost = true;
                                //////    Frm_UpCamAlignTool.Instance.Activate();
                                //////    Frm_UpCamAlignTool.Instance.jobName = this.jobName;
                                //////    Frm_UpCamAlignTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_UpCamAlignTool.Instance.Show();
                                //////    Frm_UpCamAlignTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_UpCamAlignTool.Instance.btn_runTool.Focus();
                                //////    UpCamAlignTool upCamAlignTool = (UpCamAlignTool)(L_toolList[i].tool);
                                //////    Frm_UpCamAlignTool.upCamAlignTool = upCamAlignTool;
                                //////    Application.DoEvents();

                                //////    Frm_UpCamAlignTool.Instance.cbx_toolEnable.Checked = L_toolList[i].enable;

                                //////    Frm_UpCamAlignTool.Instance.cbx_toolList.Items.Clear();
                                //////    Frm_UpCamAlignTool.Instance.cbx_toolList.Items.AddRange(upCamAlignTool.L_toolName.ToArray());
                                //////    Frm_UpCamAlignTool.Instance.cbx_toolList.SelectedIndex = upCamAlignTool.toolIdx;

                                //////    Frm_UpCamAlignTool.Instance.tbx_inputPosX.Text = upCamAlignTool.toolPar.InputPar.位置.Point.X.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_inputPosY.Text = upCamAlignTool.toolPar.InputPar.位置.Point.Y.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_inputPosU.Text = upCamAlignTool.toolPar.InputPar.位置.U.ToString();

                                //////    Frm_UpCamAlignTool.Instance.tbx_pickPosX.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_pickPosY.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_pickPosU.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].U.ToString();

                                //////    Frm_UpCamAlignTool.Instance.tbx_featureX.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_featureY.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_featureU.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].U.ToString();

                                //////    Frm_UpCamAlignTool.Instance.tbx_resultPosX.Text = upCamAlignTool.toolPar.ResultPar.位置.Point.X.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_resultPosY.Text = upCamAlignTool.toolPar.ResultPar.位置.Point.Y.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_resultPosU.Text = upCamAlignTool.toolPar.ResultPar.位置.U.ToString();

                                //////    Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetX.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetY.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetU.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].U.ToString();

                                //////    Frm_UpCamAlignTool.Instance.tbx_saftyRangeX.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_saftyRangeY.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////    Frm_UpCamAlignTool.Instance.tbx_saftyRangeU.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].U.ToString();

                                //////    break;
                                //////#endregion

                                //////#region DownCamAlign
                                //////case ToolType.DownCamAlign:
                                //////    Frm_DownCamAlignTool.Instance.lbl_title.Text = string.Format("下相机定位    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_DownCamAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_DownCamAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_DownCamAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_DownCamAlignTool.Instance.TopMost = true;
                                //////    Frm_DownCamAlignTool.Instance.Activate();
                                //////    Frm_DownCamAlignTool.Instance.jobName = this.jobName;
                                //////    Frm_DownCamAlignTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_DownCamAlignTool.Instance.Show();
                                //////    Frm_DownCamAlignTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_DownCamAlignTool.Instance.btn_runTool.Focus();
                                //////    DownCamAlignTool robotDownCamAlignTool = (DownCamAlignTool)(L_toolList[i].tool);
                                //////    Frm_DownCamAlignTool.robotDownCamAlignTool = robotDownCamAlignTool;
                                //////    Application.DoEvents();

                                //////    Frm_DownCamAlignTool.Instance.cbx_toolEnable.Checked = L_toolList[i].enable;

                                //////    Frm_DownCamAlignTool.Instance.tbx_inputPosX.Text = robotDownCamAlignTool.inputPos.Point.X.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_inputPosY.Text = robotDownCamAlignTool.inputPos.Point.Y.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_inputPosU.Text = robotDownCamAlignTool.inputPos.U.ToString();

                                //////    Frm_DownCamAlignTool.Instance.tbx_photoPosX.Text = robotDownCamAlignTool.photoPos.Point.X.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_photoPosY.Text = robotDownCamAlignTool.photoPos.Point.Y.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_photoPosU.Text = robotDownCamAlignTool.photoPos.U.ToString();

                                //////    Frm_DownCamAlignTool.Instance.tbx_featurePosX.Text = robotDownCamAlignTool.featurePos.Point.X.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_featurePosY.Text = robotDownCamAlignTool.featurePos.Point.Y.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_featurePosU.Text = robotDownCamAlignTool.featurePos.U.ToString();

                                //////    Frm_DownCamAlignTool.Instance.tbx_resultPosX.Text = robotDownCamAlignTool.resultPos.Point.X.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_resultPosY.Text = robotDownCamAlignTool.resultPos.Point.Y.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_resultPosU.Text = robotDownCamAlignTool.resultPos.U.ToString();

                                //////    Frm_DownCamAlignTool.Instance.tbx_placePosX.Text = robotDownCamAlignTool.placePos.Point.X.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_placePosY.Text = robotDownCamAlignTool.placePos.Point.Y.ToString();
                                //////    Frm_DownCamAlignTool.Instance.tbx_placePosU.Text = robotDownCamAlignTool.placePos.U.ToString();

                                //////    break;
                                //////#endregion

                                //////#region RotatePlatform
                                //////case ToolType.RotatePlatform:
                                //////    Frm_RotatePlatformTool.Instance.lbl_title.Text = string.Format("旋转平台    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_RotatePlatformTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_RotatePlatformTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_RotatePlatformTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_RotatePlatformTool.Instance.TopMost = true;
                                //////    Frm_RotatePlatformTool.Instance.Activate();
                                //////    Frm_RotatePlatformTool.Instance.jobName = this.jobName;
                                //////    Frm_RotatePlatformTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_RotatePlatformTool.Instance.Show();
                                //////    Frm_RotatePlatformTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_RotatePlatformTool.Instance.btn_runTool.Focus();
                                //////    RotatePlatformTool rotatePlatformTool = (RotatePlatformTool)(L_toolList[i].tool);
                                //////    Frm_RotatePlatformTool.rotatePlatformTool = rotatePlatformTool;
                                //////    Application.DoEvents();

                                //////    Frm_RotatePlatformTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;

                                //////    Frm_RotatePlatformTool.Instance.tbx_rotateCenterX.Text = rotatePlatformTool.rotateCenter.Point.X.ToString();
                                //////    Frm_RotatePlatformTool.Instance.tbx_rotateCenterY.Text = rotatePlatformTool.rotateCenter.Point.Y.ToString();

                                //////    Frm_RotatePlatformTool.Instance.tbx_inputPointX.Text = rotatePlatformTool.inputPos.Point.X.ToString();
                                //////    Frm_RotatePlatformTool.Instance.tbx_inputPointY.Text = rotatePlatformTool.inputPos.Point.Y.ToString();
                                //////    Frm_RotatePlatformTool.Instance.tbx_inputPointU.Text = rotatePlatformTool.inputPos.U.ToString();

                                //////    Frm_RotatePlatformTool.Instance.tbx_outputPointX.Text = rotatePlatformTool.outputPos.Point.X.ToString();
                                //////    Frm_RotatePlatformTool.Instance.tbx_outputPointY.Text = rotatePlatformTool.outputPos.Point.Y.ToString();
                                //////    Frm_RotatePlatformTool.Instance.tbx_outputPointU.Text = rotatePlatformTool.outputPos.U.ToString();

                                //////    //显示数据
                                //////    for (int j = 0; j < rotatePlatformTool.L_calibData.Count; j++)
                                //////    {
                                //////        for (int k = 0; k < 3; k++)
                                //////        {
                                //////            Frm_RotatePlatformTool.Instance.dgv_data.Rows[j].Cells[k].Value = rotatePlatformTool.L_calibData[j][k];
                                //////        }
                                //////    }

                                //////    //流程列表
                                //////    Frm_RotatePlatformTool.Instance.cbx_jobList.Items.Clear();
                                //////    for (int j = 0; j < Project.Instance.curEngine.L_jobList.Count; j++)
                                //////    {
                                //////        Frm_RotatePlatformTool.Instance.cbx_jobList.Items.Add(Project.Instance.curEngine.L_jobList[j].jobName);
                                //////    }
                                //////    Frm_RotatePlatformTool.Instance.cbx_jobList.Text = rotatePlatformTool.calibJobName;
                                //////    Frm_RotatePlatformTool.Instance.cbx_outputItemList.Text = rotatePlatformTool.calibItemName;

                                //////    break;
                                //////#endregion

                                //////#region XYPlatform
                                //////case ToolType.XYPlatform:
                                //////    Frm_XYPlatformTool.Instance.lbl_title.Text = string.Format("XY平台    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_XYPlatformTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_XYPlatformTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_XYPlatformTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_XYPlatformTool.Instance.TopMost = true;
                                //////    Frm_XYPlatformTool.Instance.Activate();
                                //////    Frm_XYPlatformTool.Instance.jobName = this.jobName;
                                //////    Frm_XYPlatformTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_XYPlatformTool.Instance.Show();
                                //////    Frm_XYPlatformTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_XYPlatformTool.Instance.btn_runTool.Focus();
                                //////    XYPlatformTool xyPlatformTool = (XYPlatformTool)(L_toolList[i].tool);
                                //////    Frm_XYPlatformTool.xyPlatformTool = xyPlatformTool;
                                //////    Application.DoEvents();

                                //////    Frm_XYPlatformTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;

                                //////    Frm_XYPlatformTool.Instance.tbx_pickPosX.Text = xyPlatformTool.pickPos.Point.X.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_pickPosY.Text = xyPlatformTool.pickPos.Point.Y.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_pickPosU.Text = xyPlatformTool.pickPos.U.ToString();

                                //////    Frm_XYPlatformTool.Instance.tbx_pickPosOffsetX.Text = xyPlatformTool.pickPosOffset.Point.X.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_pickPosOffsetY.Text = xyPlatformTool.pickPosOffset.Point.Y.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_pickPosOffsetU.Text = xyPlatformTool.pickPosOffset.U.ToString();

                                //////    Frm_XYPlatformTool.Instance.tbx_featureX.Text = xyPlatformTool.featurePos.Point.X.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_featureY.Text = xyPlatformTool.featurePos.Point.Y.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_featureU.Text = xyPlatformTool.featurePos.U.ToString();

                                //////    Frm_XYPlatformTool.Instance.tbx_inputPointX.Text = xyPlatformTool.inputPos.Point.X.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_inputPointY.Text = xyPlatformTool.inputPos.Point.Y.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_inputPointU.Text = xyPlatformTool.inputPos.U.ToString();

                                //////    Frm_XYPlatformTool.Instance.tbx_outputPointX.Text = xyPlatformTool.outputPos.Point.X.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_outputPointY.Text = xyPlatformTool.outputPos.Point.Y.ToString();
                                //////    Frm_XYPlatformTool.Instance.tbx_outputPointU.Text = xyPlatformTool.outputPos.U.ToString();

                                //////    break;
                                //////#endregion

                                //////#region 点位引导
                                //////case ToolType.PointAlign:
                                //////    Frm_PointAlignTool.Instance.lbl_title.Text = string.Format("点位引导    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_PointAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_PointAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_PointAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_PointAlignTool.Instance.TopMost = true;
                                //////    Frm_PointAlignTool.Instance.Activate();
                                //////    Frm_PointAlignTool.Instance.jobName = this.jobName;
                                //////    Frm_PointAlignTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_PointAlignTool.Instance.Show();
                                //////    Frm_PointAlignTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_PointAlignTool.Instance.btn_runTool.Focus();
                                //////    PointAlignTool pointAlignTool = (PointAlignTool)(L_toolList[i].tool);
                                //////    Frm_PointAlignTool.pointAlignTool = pointAlignTool;
                                //////    Application.DoEvents();

                                //////    Frm_PointAlignTool.Instance.cbx_toolEnable.Checked = L_toolList[i].enable;

                                //////    Frm_PointAlignTool.Instance.cbx_toolList.Items.Clear();
                                //////    Frm_PointAlignTool.Instance.cbx_toolList.Items.AddRange(pointAlignTool.L_toolName.ToArray());
                                //////    Frm_PointAlignTool.Instance.cbx_toolList.SelectedIndex = pointAlignTool.toolIdx;

                                //////    Frm_PointAlignTool.Instance.tbx_inputPosX.Text = pointAlignTool.inputPos.Point.X.ToString();
                                //////    Frm_PointAlignTool.Instance.tbx_inputPosY.Text = pointAlignTool.inputPos.Point.Y.ToString();
                                //////    Frm_PointAlignTool.Instance.tbx_inputPosU.Text = pointAlignTool.inputPos.U.ToString();

                                //////    Frm_PointAlignTool.Instance.tbx_pickPosX.Text = pointAlignTool.L_workPos[pointAlignTool.toolIdx].Point.X.ToString();
                                //////    Frm_PointAlignTool.Instance.tbx_pickPosY.Text = pointAlignTool.L_workPos[pointAlignTool.toolIdx].Point.Y.ToString();

                                //////    Frm_PointAlignTool.Instance.tbx_featureX.Text = pointAlignTool.L_featurePos[pointAlignTool.toolIdx].Point.X.ToString();
                                //////    Frm_PointAlignTool.Instance.tbx_featureY.Text = pointAlignTool.L_featurePos[pointAlignTool.toolIdx].Point.Y.ToString();
                                //////    Frm_PointAlignTool.Instance.tbx_featureU.Text = pointAlignTool.L_featurePos[pointAlignTool.toolIdx].U.ToString();

                                //////    Frm_PointAlignTool.Instance.tbx_resultPosX.Text = pointAlignTool.resultPos.Point.X.ToString();
                                //////    Frm_PointAlignTool.Instance.tbx_resultPosY.Text = pointAlignTool.resultPos.Point.Y.ToString();

                                //////    Frm_PointAlignTool.Instance.tbx_pickPosOffsetX.Text = pointAlignTool.L_workPosOffset[pointAlignTool.toolIdx].Point.X.ToString();
                                //////    Frm_PointAlignTool.Instance.tbx_pickPosOffsetY.Text = pointAlignTool.L_workPosOffset[pointAlignTool.toolIdx].Point.Y.ToString();

                                //////    Frm_PointAlignTool.Instance.tbx_saftyRangeX.Text = pointAlignTool.L_safetyRange[pointAlignTool.toolIdx].Point.X.ToString();
                                //////    Frm_PointAlignTool.Instance.tbx_saftyRangeY.Text = pointAlignTool.L_safetyRange[pointAlignTool.toolIdx].Point.Y.ToString();

                                //////    break;
                                //////#endregion

                                //////#region AlignFit
                                //////case ToolType.AlignFit:
                                //////    Frm_AlignFitTool.Instance.lbl_title.Text = string.Format("对位组装    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //////Frm_AlignFitTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //////Frm_AlignFitTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_UpCamAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_AlignFitTool.Instance.TopMost = true;
                                //////    Frm_AlignFitTool.Instance.Activate();
                                //////    Frm_AlignFitTool.Instance.jobName = this.jobName;
                                //////    Frm_AlignFitTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_AlignFitTool.Instance.Show();
                                //////    Frm_AlignFitTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_AlignFitTool.Instance.button5.Focus();
                                //////    AlignFitTool alignFitTool = (AlignFitTool)(L_toolList[i].tool);
                                //////    Frm_AlignFitTool.alignFitTool = alignFitTool;
                                //////    Application.DoEvents();

                                //////    Frm_AlignFitTool.Instance.pictureBox2.Image = L_toolList[i].enable ? Resources.开 : Resources.关;



                                //////    Frm_AlignFitTool.Instance.tbx_inputPosX.Text = alignFitTool.inputPos.Point.X.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_inputPosY.Text = alignFitTool.inputPos.Point.Y.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_inputPosU.Text = alignFitTool.inputPos.U.ToString();

                                //////    Frm_AlignFitTool.Instance.tbx_pickPosX.Text = alignFitTool.TemplateElementPlacePos.Point.X.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_pickPosY.Text = alignFitTool.TemplateElementPlacePos.Point.Y.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_pickPosU.Text = alignFitTool.TemplateElementPlacePos.U.ToString();

                                //////    Frm_AlignFitTool.Instance.tbx_featureX.Text = alignFitTool.TemplateBelowBoardPos.Point.X.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_featureY.Text = alignFitTool.TemplateBelowBoardPos.Point.Y.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_featureU.Text = alignFitTool.TemplateBelowBoardPos.U.ToString();

                                //////    Frm_AlignFitTool.Instance.tbx_resultPosX.Text = alignFitTool.resultPos.Point.X.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_resultPosY.Text = alignFitTool.resultPos.Point.Y.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_resultPosU.Text = alignFitTool.resultPos.U.ToString();

                                //////    Frm_AlignFitTool.Instance.tbx_pickPosOffsetX.Text = alignFitTool.TemplateElementPlacePosOffset.Point.X.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_pickPosOffsetY.Text = alignFitTool.TemplateElementPlacePosOffset.Point.Y.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_pickPosOffsetU.Text = alignFitTool.TemplateElementPlacePosOffset.U.ToString();

                                //////    Frm_AlignFitTool.Instance.tbx_saftyRangeX.Text = alignFitTool.L_safetyRange.Point.X.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_saftyRangeY.Text = alignFitTool.L_safetyRange.Point.Y.ToString();
                                //////    Frm_AlignFitTool.Instance.tbx_saftyRangeU.Text = alignFitTool.L_safetyRange.U.ToString();

                                //////    break;
                                //////#endregion

                                //////#region AlignWithoutCalibRotateCenter
                                //////case ToolType.AlignWithoutCalibRotateCenter:
                                //////    Frm_AlignWithoutCalibRotateCenterTool.Instance.lbl_title.Text = string.Format("上相机定位    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_UpCamAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_UpCamAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_UpCamAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_UpCamAlignTool.Instance.TopMost = true;
                                //////    Frm_AlignWithoutCalibRotateCenterTool.Instance.Activate();
                                //////    Frm_AlignWithoutCalibRotateCenterTool.Instance.jobName = this.jobName;
                                //////    Frm_AlignWithoutCalibRotateCenterTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_AlignWithoutCalibRotateCenterTool.Instance.Show();
                                //////    Frm_AlignWithoutCalibRotateCenterTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_AlignWithoutCalibRotateCenterTool.Instance.btn_runTool.Focus();
                                //////    AlignWithoutCalibRotateCenterTool alignWithoutCalibRotateCenterTool = (AlignWithoutCalibRotateCenterTool)(L_toolList[i].tool);
                                //////    Frm_AlignWithoutCalibRotateCenterTool.alignWithoutCalibRotateCenterTool = alignWithoutCalibRotateCenterTool;
                                //////    Application.DoEvents();

                                //////    Frm_UpCamAlignTool.Instance.cbx_toolEnable.Checked = L_toolList[i].enable;

                                //////    //////Frm_UpCamAlignTool.Instance.cbx_toolList.Items.Clear();
                                //////    //////Frm_UpCamAlignTool.Instance.cbx_toolList.Items.AddRange(upCamAlignTool.L_toolName.ToArray());
                                //////    //////Frm_UpCamAlignTool.Instance.cbx_toolList.SelectedIndex = upCamAlignTool.toolIdx;

                                //////    //////Frm_UpCamAlignTool.Instance.tbx_inputPosX.Text = upCamAlignTool.inputPos.Point.X.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_inputPosY.Text = upCamAlignTool.inputPos.Point.Y.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_inputPosU.Text = upCamAlignTool.inputPos.U.ToString();

                                //////    //////Frm_UpCamAlignTool.Instance.tbx_pickPosX.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_pickPosY.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_pickPosU.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].U.ToString();

                                //////    //////Frm_UpCamAlignTool.Instance.tbx_featureX.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_featureY.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_featureU.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].U.ToString();

                                //////    //////Frm_UpCamAlignTool.Instance.tbx_resultPosX.Text = upCamAlignTool.resultPos.Point.X.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_resultPosY.Text = upCamAlignTool.resultPos.Point.Y.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_resultPosU.Text = upCamAlignTool.resultPos.U.ToString();

                                //////    //////Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetX.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetY.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetU.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].U.ToString();

                                //////    //////Frm_UpCamAlignTool.Instance.tbx_saftyRangeX.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_saftyRangeY.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////    //////Frm_UpCamAlignTool.Instance.tbx_saftyRangeU.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].U.ToString();

                                //////    break;
                                //////#endregion

                                #region FindLine
                                case ToolType.FindLine:




                                    FindLineTool findLineTool = (FindLineTool)L_toolList[i].tool;

                                    if (findLineTool.toolPar.InputPar.图像 != null)
                                        GetImageWindowControl().hwc_imageWindow.HobjectToHimage(findLineTool.toolPar.InputPar.图像);


                                    if (findLineTool.toolPar.InputPar.图像 != null)
                                        GetImageWindowControl().hwc_imageWindow.HobjectToHimage(findLineTool.toolPar.InputPar.图像);
                                    else
                                        GetImageWindowControl().hwc_imageWindow.ClearWindow();


                                    findLineTool.Run(true, false, L_toolList[i].toolName);





                                    break;
                                #endregion

                                #region FindCircle
                                case ToolType.FindCircle:

                                    FindCircleTool findCircleTool = (FindCircleTool)L_toolList[i].tool;

                                    // 流程树/图像窗口中单独运行"查找圆"节点时进入这里。
                                    // runTool=false 表示结果画在主图像窗口；工具参数和结果仍由 FindCircleTool.Run() 处理。
                                    if (findCircleTool.toolPar.InputPar.图像 != null)
                                        GetImageWindowControl().hwc_imageWindow.HobjectToHimage(findCircleTool.toolPar.InputPar.图像);


                                    if (findCircleTool.toolPar.InputPar.图像 != null)
                                        GetImageWindowControl().hwc_imageWindow.HobjectToHimage(findCircleTool.toolPar.InputPar.图像);
                                    else
                                        GetImageWindowControl().hwc_imageWindow.ClearWindow();


                                    findCircleTool.Run(true, false, L_toolList[i].toolName);










                                    break;
                                #endregion

                                //////#region SubImage
                                //////case ToolType.SubImage:
                                //////    Frm_SubImageTool.Instance.pictureBox1.Image = Resources.SubImageTool;
                                //////    Frm_SubImageTool.Instance.lbl_title.Text = string.Format("减图像    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_SubImageTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_SubImageTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_SubImageTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_SubImageTool.Instance.TopMost = true;
                                //////    Frm_SubImageTool.Instance.Activate();
                                //////    Frm_SubImageTool.Instance.jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                                //////    Frm_SubImageTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_SubImageTool.Instance.Show();
                                //////    Frm_SubImageTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_SubImageTool.Instance.btn_runImageSubTool.Focus();
                                //////    SubImageTool subImageTool = (SubImageTool)(L_toolList[i].tool);
                                //////    Frm_SubImageTool.subImageTool = subImageTool;
                                //////    Application.DoEvents();

                                //////    if (subImageTool.inputImage != null)
                                //////        subImageTool.ShowImage(subImageTool.inputImage);
                                //////    else
                                //////        //////subImageTool.ClearWindow(jobName);

                                //////        Frm_SubImageTool.Instance.ckb_subImageToolEnable.Checked = L_toolList[i].enable;
                                //////    Frm_SubImageTool.Instance.cbx_standardImage.Text = subImageTool.standardImageName;
                                //////    break;
                                //////#endregion

                                //////#region CreateROI
                                //////case ToolType.CreateROI:
                                //////    Frm_CreateROITool.Instance.pictureBox1.Image = Resources.CreateROITool;
                                //////    Frm_CreateROITool.Instance.lbl_title.Text = string.Format("创建ROI    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_CreateROITool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_CreateROITool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_CreateROITool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_CreateROITool.Instance.TopMost = true;
                                //////    Frm_CreateROITool.Instance.Activate();
                                //////    Frm_CreateROITool.Instance.jobName = this.jobName;
                                //////    Frm_CreateROITool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_CreateROITool.Instance.Show();
                                //////    Frm_CreateROITool.Instance.WindowState = FormWindowState.Normal;
                                //////    //////Frm_CreateROITool.Instance.btn_runFindCircleTool.Focus();
                                //////    CreateROITool createROITool = (CreateROITool)(L_toolList[i].tool);
                                //////    Frm_CreateROITool.createROITool = createROITool;
                                //////    Application.DoEvents();


                                //////    inputItemNum = (L_toolList[i]).input.Count;
                                //////    for (int j = 0; j < inputItemNum; j++)
                                //////    {
                                //////        string inputItem = L_toolList[i].input[j].IOName;
                                //////        string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString(); if (inputItem == string.Empty)
                                //////        {

                                //////        }

                                //////        //////createROITool.inputPose = null;
                                //////        if (inputItem == "左上点行")
                                //////        {
                                //////            string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                //////            sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                //////            string toolItem = Regex.Split(sourceFrom, "->")[1];
                                //////            createROITool.leftTopRow = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                //////        }
                                //////        else if (inputItem == "左上点列")
                                //////        {
                                //////            string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                //////            sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                //////            string toolItem = Regex.Split(sourceFrom, "->")[1];
                                //////            createROITool.leftTopCol = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                //////        }
                                //////        else if (inputItem == "右下点行" || inputItem == "ExpectCircleCenterX")
                                //////        {
                                //////            string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                //////            sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                //////            string toolItem = Regex.Split(sourceFrom, "->")[1];
                                //////            createROITool.rightDownRow = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                //////        }
                                //////        else if (inputItem == "右下点列" || inputItem == "ExpectCircleCenterY")
                                //////        {
                                //////            string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                //////            sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                //////            string toolItem = Regex.Split(sourceFrom, "->")[1];
                                //////            createROITool.rightDownCol = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                //////        }
                                //////        else if (inputItem == "跟随" || inputItem == "ExpectCircleCenterY")
                                //////        {
                                //////            string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                //////            sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                //////            string toolItem = Regex.Split(sourceFrom, "->")[1];
                                //////            createROITool.inputPose = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                //////        }
                                //////        else if (inputItem == "图像" || inputItem == "ExpectC信息ircleCenterY")
                                //////        {
                                //////            string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                //////            sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                //////            string toolItem = Regex.Split(sourceFrom, "->")[1];
                                //////            createROITool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                //////        }
                                //////    }




                                //////    //显示背景图
                                //////    Frm_CreateROITool.Instance.hWindow_Final1.HobjectToHimage(createROITool.toolPar.InputPar.图像);


                                //////    Frm_CreateROITool.Instance.hWindow_Final1.viewWindow.displayROI(createROITool.regions);
                                //////    Frm_CreateROITool.regions = createROITool.regions;

                                //////    Frm_CreateROITool.binDataGridView(Frm_CreateROITool.Instance.dgv_ROI, createROITool.regions);


                                //////    //将对象信息更新到界面
                                //////    //////Frm_CreateROITool.Instance.ckb_createROIToolEnable.Checked = L_toolList[i].enable;
                                //////    //////Frm_CreateROITool.Instance.tbx_leftTopRow.Text = createROITool.leftTopRow.ToString();
                                //////    //////Frm_CreateROITool.Instance.tbx_leftTopCol.Text = createROITool.leftTopCol.ToString();
                                //////    //////Frm_CreateROITool.Instance.tbx_rightDownRow.Text = createROITool.rightDownRow.ToString();
                                //////    //////Frm_CreateROITool.Instance.tbx_rightDownCol.Text = createROITool.rightDownCol.ToString();
                                //////    break;
                                //////#endregion

                                //////#region ArrayRegion
                                //////case ToolType.ArrayRegion:
                                //////    Frm_ArrayRegionTool.Instance.pictureBox1.Image = Resources.RegionArrayTool;
                                //////    Frm_ArrayRegionTool.Instance.lbl_title.Text = string.Format("阵列区域    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_ArrayRegionTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_ArrayRegionTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ArrayRegionTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_ArrayRegionTool.Instance.TopMost = true;
                                //////    Frm_ArrayRegionTool.Instance.Activate();
                                //////    Frm_ArrayRegionTool.Instance.jobName = this.jobName;
                                //////    Frm_ArrayRegionTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_ArrayRegionTool.Instance.Show();
                                //////    Frm_ArrayRegionTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //////Frm_CreateROITool.Instance.btn_runDistancePLTool.Focus();
                                //////    ArrayRegionTool arrayRegionTool = (ArrayRegionTool)(L_toolList[i].tool);
                                //////    Frm_ArrayRegionTool.arrayRegionTool = arrayRegionTool;
                                //////    Application.DoEvents();

                                //////    //将对象信息更新到界面
                                //////    if (arrayRegionTool.inputImage != null)
                                //////        arrayRegionTool.ShowImage(arrayRegionTool.inputImage);
                                //////    else
                                //////        arrayRegionTool.ClearWindow();

                                //////    if (arrayRegionTool.outputRegion != null)
                                //////    {
                                //////        GetImageWindowControl().hwc_imageWindow.viewWindow.displayROI(arrayRegionTool.regions);
                                //////        GetImageWindowControl().regions = arrayRegionTool.regions;
                                //////    }

                                //////    Frm_ArrayRegionTool.Instance.ckb_shapeMatchToolEnable.Checked = L_toolList[i].enable;
                                //////    Frm_ArrayRegionTool.Instance.textBox1.Text = arrayRegionTool.rowNum.ToString();
                                //////    Frm_ArrayRegionTool.Instance.textBox2.Text = arrayRegionTool.colNum.ToString();
                                //////    Frm_ArrayRegionTool.Instance.textBox3.Text = arrayRegionTool.rowSpan.ToString();
                                //////    Frm_ArrayRegionTool.Instance.textBox4.Text = arrayRegionTool.colSpan.ToString();

                                //////    break;
                                //////#endregion

                                //////#region Mark
                                //////case ToolType.Mark:
                                //////    Frm_MarkTool.Instance.pictureBox1.Image = Resources.MarkTool;
                                //////    Frm_MarkTool.Instance.lbl_title.Text = string.Format("标记点    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_MarkTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_MarkTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_MarkTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_MarkTool.Instance.TopMost = true;
                                //////    Frm_MarkTool.Instance.Activate();
                                //////    Frm_MarkTool.Instance.jobName = this.jobName;
                                //////    Frm_MarkTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_MarkTool.Instance.Show();
                                //////    Frm_MarkTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //////Frm_MarkTool.Instance.btn_runDownCamAlignTool.Focus();
                                //////    MarkTool markTool = (MarkTool)(L_toolList[i].tool);
                                //////    Frm_MarkTool.markTool = markTool;
                                //////    Application.DoEvents();

                                //////    Frm_MarkTool.Instance.ckb_shapeMatchToolEnable.Checked = L_toolList[i].enable;
                                //////    Frm_MarkTool.Instance.tbx_caputurePosX.Text = markTool.inputPoint.X.ToString();
                                //////    Frm_MarkTool.Instance.tbx_caputurePosY.Text = markTool.inputPoint.Y.ToString();

                                //////    break;
                                //////#endregion

                                //////#region PoseToStr
                                //////case ToolType.ToStr:
                                //////    Frm_PoseToStrTool.Instance.pictureBox1.Image = Resources.UnknownTool;
                                //////    Frm_PoseToStrTool.Instance.lbl_title.Text = string.Format("转文本    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //////Frm_PoseToStrTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //////Frm_PoseToStrTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_PoseToStrTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_PoseToStrTool.Instance.TopMost = true;
                                //////    Frm_PoseToStrTool.Instance.Activate();
                                //////    Frm_PoseToStrTool.Instance.jobName = this.jobName;
                                //////    Frm_PoseToStrTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_PoseToStrTool.Instance.Show();
                                //////    Frm_PoseToStrTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //////Frm_MarkTool.Instance.btn_runDownCamAlignTool.Focus();
                                //////    ToStrTool poseToStrTool = (ToStrTool)(L_toolList[i].tool);
                                //////    Frm_PoseToStrTool.poseToStrTool = poseToStrTool;
                                //////    Application.DoEvents();

                                //////    //////Frm_PoseToStrTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;
                                //////    Frm_PoseToStrTool.Instance.textBox1.TextStr = poseToStrTool.splitChar;

                                //////    break;
                                //////#endregion

                                //////#region DistancePL
                                //////case ToolType.DistancePL:
                                //////    Frm_DistancePLTool.Instance.pictureBox1.Image = Resources.DistancePLTool;
                                //////    Frm_DistancePLTool.Instance.lbl_title.Text = string.Format("点线距离    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //////Frm_DistancePLTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //////Frm_DistancePLTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_DistancePLTool.Instance.Width - 2, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_DistancePLTool.Instance.TopMost = true;
                                //////    Frm_DistancePLTool.Instance.Activate();
                                //////    Frm_DistancePLTool.Instance.jobName = this.jobName;
                                //////    Frm_DistancePLTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_DistancePLTool.Instance.Show();
                                //////    Frm_DistancePLTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //  Frm_DistancePointLineTool.Instance.dd.Focus();
                                //////    DistancePLTool distancePLTool = (DistancePLTool)(L_toolList[i].tool);
                                //////    Application.DoEvents();

                                //////    if (((DistancePLTool)(L_toolList[i].tool)).inputImage != null)
                                //////        GetImageWindowControl().Display_Image(((DistancePLTool)(L_toolList[i].tool)).inputImage);
                                //////    else
                                //////        HOperatorSet.ClearWindow(Frm_ImageWindow.Instance.WindowHandle);

                                //////    //将对象信息更新到界面
                                //////    Frm_DistancePLTool.Instance.ckb_distancePLToolEnable.Checked = L_toolList[i].enable;

                                //////    break;
                                //////#endregion

                                //////#region DistanceSS
                                //////case ToolType.DistanceSS:
                                //////    Frm_DistanceLLTool.Instance.pictureBox1.Image = Resources.UnknownTool;
                                //////    Frm_DistanceLLTool.Instance.lbl_title.Text = string.Format("线段与线段距离    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //////Frm_DistanceLLTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //////Frm_DistanceLLTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_DistanceLLTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_DistanceLLTool.Instance.TopMost = true;
                                //////    Frm_DistanceLLTool.Instance.Activate();
                                //////    Frm_DistanceLLTool.Instance.jobName = this.jobName;
                                //////    Frm_DistanceLLTool.Instance.toolName = L_toolList[i].toolName;
                                //////    ////// Frm_DistanceSegmentAndSegmentTool.Instance.Show();
                                //////    Frm_DistanceLLTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //////SharpEdit.Form1.Instance.fctb.Focus();
                                //////    DistanceLLTool distanceSSTool = (DistanceLLTool)(L_toolList[i].tool);
                                //////    Application.DoEvents();

                                //////    //将对象信息更新到界面
                                //////    Frm_MessageBox.Instance.MessageBoxShow("\r\n本工具为无窗体工具！");
                                //////    break;
                                //////#endregion

                                #region LLPoint
                                case ToolType.LLIntersect:
                                    LLIntersectTool llIntersectTool = (LLIntersectTool)L_toolList[i].tool;

                                    GetImageWindowControl().hwc_imageWindow.HobjectToHimage(GetImageWindowControl().currentImage);
                                    llIntersectTool.Run(true, false, L_toolList[i].toolName);

                                    break;
                                #endregion



                                //////#region RegionFeature
                                //////case ToolType.RegionFeature:
                                //////    Frm_RegionFeatureTool.Instance.pictureBox1.Image = Resources.RegionFeatureTool;
                                //////    Frm_RegionFeatureTool.Instance.lbl_title.Text = string.Format("区域特征    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_RegionFeatureTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_RegionFeatureTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_RegionFeatureTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_RegionFeatureTool.Instance.TopMost = true;
                                //////    Frm_RegionFeatureTool.Instance.Activate();
                                //////    Frm_RegionFeatureTool.Instance.jobName = this.jobName;
                                //////    Frm_RegionFeatureTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_RegionFeatureTool.Instance.Show();
                                //////    Frm_RegionFeatureTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //////Frm_RegionFeatureTool.Instance.btn_runFindBarcodeTool.Focus();
                                //////    RegionFeatureTool regionFeatureTool = (RegionFeatureTool)(L_toolList[i].tool);
                                //////    Frm_RegionFeatureTool.regionFeatureTool = regionFeatureTool;
                                //////    Application.DoEvents();

                                //////    break;
                                //////#endregion

                                //////#region OCR
                                //////case ToolType.OCR:
                                //////    Frm_OCRTool.Instance.pictureBox1.Image = Resources.OCRTool;
                                //////    Frm_OCRTool.Instance.lbl_title.Text = string.Format("OCR    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_OCRTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_OCRTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ShapeMatchTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_OCRTool.Instance.TopMost = true;
                                //////    Frm_OCRTool.Instance.Activate();
                                //////    Frm_OCRTool.Instance.jobName = this.jobName;
                                //////    Frm_OCRTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_OCRTool.Instance.Show();
                                //////    Frm_OCRTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_OCRTool.Instance.btn_runOCRTool.Focus();
                                //////    OCRTool ocrTool = (OCRTool)(L_toolList[i].tool);
                                //////    Frm_OCRTool.ocrTool = ocrTool;
                                //////    Application.DoEvents();

                                //////    if (ocrTool.inputImage != null)
                                //////        ocrTool.ShowImage(ocrTool.inputImage);
                                //////    else
                                //////        ////////ocrTool.ClearWindow(this.jobName);

                                //////        if (ocrTool.searchRegion != null)
                                //////        {
                                //////            ocrTool.SetColor(this.jobName, "blue");
                                //////            //////ocrTool.ShowObj(this.jobName, ocrTool.searchRegion);
                                //////        }

                                //////    Frm_OCRTool.Instance.lbl_threshold.Text = ocrTool.threshold.ToString();
                                //////    Frm_OCRTool.Instance.tkb_threshold.Value = ocrTool.threshold;
                                //////    Frm_OCRTool.Instance.ckb_OCRToolEnable.Checked = L_toolList[i].enable;
                                //////    Frm_OCRTool.Instance.cbx_searchRegionType.Text = ocrTool.searchRegionType.ToString();
                                //////    Frm_OCRTool.Instance.cbx_templateRegionType.Text = ocrTool.templateRegionType.ToString();
                                //////    Frm_OCRTool.Instance.tbx_resultStr.Text = ocrTool.outputStr;
                                //////    Frm_OCRTool.Instance.cbx_charType.SelectedIndex = (ocrTool.charType == CharType.BlackChar ? 0 : 1);
                                //////    Frm_OCRTool.Instance.tbx_dilationSize.Text = ocrTool.dilationSize.ToString();
                                //////    Frm_OCRTool.Instance.tbx_standardCharList.Text = ocrTool.standardCharList;

                                //////    break;
                                //////#endregion

                                //////#region Barcode
                                //////case ToolType.Barcode:
                                //////    Frm_BarcodeTool.Instance.pictureBox1.Image = Resources.BarCodeTool;
                                //////    Frm_BarcodeTool.Instance.lbl_title.Text = string.Format("条码    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_BarcodeTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_BarcodeTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_BarcodeTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_BarcodeTool.Instance.TopMost = true;
                                //////    Frm_BarcodeTool.Instance.Activate();
                                //////    Frm_BarcodeTool.Instance.jobName = this.jobName;
                                //////    Frm_BarcodeTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_BarcodeTool.Instance.Show();
                                //////    Frm_BarcodeTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_BarcodeTool.Instance.btn_runFindBarcodeTool.Focus();
                                //////    BarcodeTool barcodeTool = (BarcodeTool)(L_toolList[i].tool);
                                //////    Frm_BarcodeTool.barcodeTool = barcodeTool;
                                //////    Application.DoEvents();

                                //////    break;
                                //////#endregion

                                //////#region CodeEdit
                                //////case ToolType.CodeEdit:
                                //////    //////Frm_CodeEditTool.Instance.Text = "脚本编辑 - " + this.jobName + "." + L_toolList[i].toolName;
                                //////    //////Frm_CodeEditTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //////Frm_CodeEditTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_CodeEditTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //////Frm_CodeEditTool.Instance.TopMost = true;
                                //////    //////Frm_CodeEditTool.Instance.Show();
                                //////    //////Frm_CodeEditTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //////Frm_CodeEditTool.Instance.tbx_code.Focus();
                                //////    //////CodeEditTool codeEditTool = (CodeEditTool)(L_toolList[i].tool);
                                //////    //////Frm_CodeEditTool.codeEditTool = codeEditTool;
                                //////    //////Application.DoEvents();

                                //////    //////Frm_CodeEditTool.Instance.tbx_code.Text = ((CodeEditTool)L_toolList[i].tool).sourceCode;
                                //////    break;
                                //////#endregion

                                //////#region DataAnalyse
                                //////case ToolType.DataAnalyse:
                                //////    Frm_DataAnalyseTool.Instance.pictureBox1.Image = Resources.LabelTool;
                                //////    Frm_DataAnalyseTool.Instance.lbl_title.Text = string.Format("显示文本    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_LabelTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_LabelTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_LabelTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_LabelTool.Instance.TopMost = true;
                                //////    Frm_DataAnalyseTool.Instance.Activate();
                                //////    Frm_DataAnalyseTool.Instance.jobName = this.jobName;
                                //////    Frm_DataAnalyseTool.Instance.toolName = L_toolList[i].toolName; ;
                                //////    Frm_DataAnalyseTool.Instance.Show();
                                //////    Frm_DataAnalyseTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //Frm_LabelTool.Instance.btn_runTool.Focus();
                                //////    DataAnalyseTool dataAnalyseTool = (DataAnalyseTool)(L_toolList[i].tool);
                                //////    Frm_DataAnalyseTool.dataAnalyseTool = dataAnalyseTool;
                                //////    Application.DoEvents();

                                //////    int itemCount = ((DataAnalyseTool)L_toolList[i].tool).L_items.Count;
                                //////    Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows.Clear();
                                //////    for (int j = 0; j < itemCount; j++)
                                //////    {
                                //////        int index = Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows.Add();
                                //////        Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[0].Value = dataAnalyseTool.L_items[j].inputItem;
                                //////        Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[1].Value = dataAnalyseTool.L_items[j].downLimit.ToString().ToString();
                                //////        Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[2].Value = dataAnalyseTool.L_items[j].upLimit.ToString();
                                //////        Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[3].Value = dataAnalyseTool.L_items[j].inResult;
                                //////        Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[4].Value = dataAnalyseTool.L_items[j].outResult;
                                //////    }

                                //////    //Frm_LabelTool.Instance.ckb_toolEnable.Checked = ((ToolInfo)L_toolList[i]).enable;
                                //////    break;
                                //////#endregion

                                //////#region OPTLight
                                //////case ToolType.Light_OPT:
                                //////    Frm_OPTLightTool.Instance.pictureBox1.Image = Resources.LightTool;
                                //////    Frm_OPTLightTool.Instance.lbl_title.Text = string.Format("奥普特光源控制    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_OPTLightTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_OPTLightTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_OPTLightTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_OPTLightTool.Instance.TopMost = true;
                                //////    Frm_OPTLightTool.Instance.Activate();
                                //////    Frm_OPTLightTool.Instance.jobName = this.jobName;
                                //////    Frm_OPTLightTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Light_OPTTool optLightTool = (Light_OPTTool)(L_toolList[i].tool);
                                //////    Frm_OPTLightTool.optLightTool = optLightTool;
                                //////    Frm_OPTLightTool.Instance.Show();
                                //////    Frm_OPTLightTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //Frm_OPTLightTool.Instance.btn_runShapeMatchTool.Focus();
                                //////    Application.DoEvents();
                                //////    break;
                                //////#endregion

                                //////#region OPTLightControl
                                //////case ToolType.OPTLightControl:
                                //////    Frm_OptLightControlTool.Instance.pictureBox1.Image = Resources.LightTool;
                                //////    Frm_OptLightControlTool.Instance.lbl_title.Text = string.Format("光源控制    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_OptLightControlTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_OptLightControlTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_OptLightControlTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_OptLightControlTool.Instance.TopMost = true;
                                //////    Frm_OptLightControlTool.Instance.Activate();
                                //////    Frm_OptLightControlTool.Instance.jobName = this.jobName;
                                //////    Frm_OptLightControlTool.Instance.toolName = L_toolList[i].toolName;
                                //////    OptLightControlTool optLightControlTool = (OptLightControlTool)(L_toolList[i].tool);
                                //////    Frm_OptLightControlTool.optLightControlTool = optLightControlTool;
                                //////    Frm_OptLightControlTool.Instance.Show();
                                //////    Frm_OptLightControlTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //Frm_OPTLightTool.Instance.btn_runShapeMatchTool.Focus();
                                //////    Application.DoEvents();

                                //////    Frm_OptLightControlTool.Instance.comboBox1.SelectedIndex = (optLightControlTool.controlMode ? 0 : 1);
                                //////    break;
                                //////#endregion

                                //////#region BatteryFirstAlign
                                //////case ToolType.batteryFirstAlign:
                                //////    Frm_BatteryFirstAlignTool.Instance.pictureBox1.Image = Resources.OCRTool;
                                //////    Frm_BatteryFirstAlignTool.Instance.lbl_title.Text = string.Format("电池初定位    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_BatteryFirstAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_BatteryFirstAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_BatteryFirstAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_BatteryFirstAlignTool.Instance.TopMost = true;
                                //////    Frm_BatteryFirstAlignTool.Instance.Activate();
                                //////    Frm_BatteryFirstAlignTool.Instance.jobName = this.jobName;
                                //////    Frm_BatteryFirstAlignTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_BatteryFirstAlignTool.Instance.Show();
                                //////    Frm_BatteryFirstAlignTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //////Frm_BatteryFirstAlignTool.Instance.btn_runShapeMatchTool.Focus();
                                //////    BatteryFirstAlignTool batteryFirstAlignTool = (BatteryFirstAlignTool)(L_toolList[i].tool);
                                //////    Frm_BatteryFirstAlignTool.batteryFirstAlignTool = batteryFirstAlignTool;
                                //////    Application.DoEvents();

                                //////    if (batteryFirstAlignTool.inputImage != null)
                                //////        batteryFirstAlignTool.ShowImage(batteryFirstAlignTool.inputImage);
                                //////    else
                                //////        //////batteryFirstAlignTool.ClearWindow(this.jobName);

                                //////        if (batteryFirstAlignTool.SearchRegion != null)
                                //////        {
                                //////            GetImageWindowControl().hwc_imageWindow.viewWindow.displayROI(batteryFirstAlignTool.regions);
                                //////            GetImageWindowControl().regions = batteryFirstAlignTool.regions;

                                //////        }

                                //////    if (batteryFirstAlignTool.regions.Count == 0)
                                //////    {
                                //////        GetImageWindowControl().hwc_imageWindow.viewWindow.genRect1(200.0, 200.0, 600.0, 800.0, ref batteryFirstAlignTool.regions);
                                //////        GetImageWindowControl().regions = batteryFirstAlignTool.regions;
                                //////    }
                                //////    else
                                //////    {
                                //////        GetImageWindowControl().hwc_imageWindow.viewWindow.displayROI(batteryFirstAlignTool.regions);
                                //////        GetImageWindowControl().regions = batteryFirstAlignTool.regions;
                                //////    }

                                //////    //将对象信息更新到界面
                                //////    Frm_BatteryFirstAlignTool.Instance.ckb_findLineToolEnable.Checked = L_toolList[i].enable;
                                //////    Frm_BatteryFirstAlignTool.Instance.nud_minThreshold.Value = batteryFirstAlignTool.minThreshold;
                                //////    Frm_BatteryFirstAlignTool.Instance.nud_maxThreshold.Value = batteryFirstAlignTool.maxThreshold;
                                //////    Frm_BatteryFirstAlignTool.Instance.numericUpDown3.Value = batteryFirstAlignTool.dilationAndErosionSize;
                                //////    Frm_BatteryFirstAlignTool.Instance.numericUpDown1.Value = Convert.ToDecimal(batteryFirstAlignTool.minArea);
                                //////    Frm_BatteryFirstAlignTool.Instance.numericUpDown2.Value = Convert.ToDecimal(batteryFirstAlignTool.maxArea);
                                //////    Frm_BatteryFirstAlignTool.Instance.cbx_edgeSelect.Text = "点" + batteryFirstAlignTool.pointIndex;
                                //////    Frm_BatteryFirstAlignTool.Instance.comboBox1.Text = "边" + batteryFirstAlignTool.lineIndex;

                                //////    //Frm_ShapeMatchTool.Instance.nud_angleStart.Value = Convert.ToDecimal(shapeMatchTool.startAngle);
                                //////    //Frm_ShapeMatchTool.Instance.nud_angleRange.Value = Convert.ToDecimal(shapeMatchTool.angleRange);
                                //////    //Frm_ShapeMatchTool.Instance.nud_angleStep.Value = Convert.ToDecimal(shapeMatchTool.angleStep);
                                //////    //Frm_ShapeMatchTool.Instance.tkb_contrast.Value = Convert.ToInt16(shapeMatchTool.contrast);
                                //////    //Frm_ShapeMatchTool.Instance.cbx_polarity.Text = shapeMatchTool.polarity;
                                //////    //if (shapeMatchTool.angleStep == 0)
                                //////    //{
                                //////    //    Frm_ShapeMatchTool.Instance.nud_angleStep.Enabled = false;
                                //////    //    Frm_ShapeMatchTool.Instance.ckb_angleStep.Checked = true;
                                //////    //}
                                //////    //else
                                //////    //{
                                //////    //    Frm_ShapeMatchTool.Instance.ckb_angleStep.Checked = false;
                                //////    //}
                                //////    break;
                                //////#endregion

                                //////#region BuChang
                                //////case ToolType.BuChang:
                                //////    Frm_BuChangTool.Instance.pictureBox1.Image = Resources.UnknownTool;
                                //////    Frm_BuChangTool.Instance.lbl_title.Text = string.Format("补偿    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_BuChangTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_BuChangTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_BuChangTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_BuChangTool.Instance.TopMost = true;
                                //////    Frm_BuChangTool.Instance.Activate();
                                //////    Frm_BuChangTool.Instance.jobName = this.jobName;
                                //////    Frm_BuChangTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_BuChangTool.Instance.Show();
                                //////    Frm_BuChangTool.Instance.WindowState = FormWindowState.Normal;
                                //////    ////Frm_BuChangTool.Instance.btn_runDownCamAlignTool.Focus();
                                //////    BuChangTool buChangTool = (BuChangTool)(L_toolList[i].tool);
                                //////    Frm_BuChangTool.buChangTool = buChangTool;
                                //////    Application.DoEvents();

                                //////    Frm_BuChangTool.Instance.tbx_caputurePosX.Text = buChangTool.templatePos.Point.X.ToString();
                                //////    Frm_BuChangTool.Instance.tbx_caputurePosY.Text = buChangTool.templatePos.Point.Y.ToString();
                                //////    Frm_BuChangTool.Instance.tbx_caputurePosU.Text = buChangTool.templatePos.U.ToString();

                                //////    Frm_BuChangTool.Instance.tbx_pickPosX.Text = buChangTool.workPos.Point.X.ToString();
                                //////    Frm_BuChangTool.Instance.tbx_pickPosY.Text = buChangTool.workPos.Point.Y.ToString();
                                //////    Frm_BuChangTool.Instance.tbx_pickPosU.Text = buChangTool.workPos.U.ToString();


                                //////    Frm_BuChangTool.Instance.tbx_pickPosOffsetX.Text = buChangTool.buchang.Point.X.ToString();
                                //////    Frm_BuChangTool.Instance.tbx_pickPosOffsetY.Text = buChangTool.buchang.Point.Y.ToString();
                                //////    Frm_BuChangTool.Instance.tbx_pickPosOffsetU.Text = buChangTool.buchang.U.ToString();

                                //////    Frm_BuChangTool.Instance.ckb_distancePLToolEnable.Checked = L_toolList[i].enable;
                                //////    break;
                                //////#endregion

                                //////#region PointOffset
                                //////case ToolType.PointOffset:
                                //////    Frm_PointOffsetTool.Instance.lbl_title.Text = string.Format("补偿    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //Frm_PointOffsetTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //Frm_PointOffsetTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_PointOffsetTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_PointOffsetTool.Instance.TopMost = true;
                                //////    Frm_PointOffsetTool.Instance.Activate();
                                //////    Frm_PointOffsetTool.Instance.jobName = this.jobName;
                                //////    Frm_PointOffsetTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_PointOffsetTool.Instance.Show();
                                //////    Frm_PointOffsetTool.Instance.WindowState = FormWindowState.Normal;
                                //////    ////Frm_BuChangTool.Instance.btn_runDownCamAlignTool.Focus();
                                //////    PointOffsetTool pointOffsetTool = (PointOffsetTool)(L_toolList[i].tool);
                                //////    Frm_PointOffsetTool.pointOffsetTool = pointOffsetTool;
                                //////    Application.DoEvents();

                                //////    Frm_PointOffsetTool.Instance.comboBox1.Clear();

                                //////    if (pointOffsetTool.toolPar.InputPar.点.ToString() == "System.Collections.Generic.List`1[VisionAndMotionPro.Point]")
                                //////    {
                                //////        for (int j = 0; j < ((List<Point>)pointOffsetTool.toolPar.InputPar.点).Count; j++)
                                //////        {
                                //////            Frm_PointOffsetTool.Instance.comboBox1.Add((j + 1).ToString());
                                //////        }
                                //////    }
                                //////    else
                                //////    {

                                //////    }


                                //////    Frm_PointOffsetTool.Instance.comboBox1.TextStr = pointOffsetTool.pointIdx.ToString();

                                //////    Frm_PointOffsetTool.Instance.tbx_caputurePosX.Value = pointOffsetTool.templatePos.X.ToString();
                                //////    Frm_PointOffsetTool.Instance.tbx_caputurePosY.Value = pointOffsetTool.templatePos.Y.ToString();

                                //////    Frm_PointOffsetTool.Instance.tbx_pickPosX.Value = pointOffsetTool.workPos.X.ToString();
                                //////    Frm_PointOffsetTool.Instance.tbx_pickPosY.Value = pointOffsetTool.workPos.Y.ToString();


                                //////    Frm_PointOffsetTool.Instance.tbx_pickPosOffsetX.Value = pointOffsetTool.buchang.X;
                                //////    Frm_PointOffsetTool.Instance.tbx_pickPosOffsetY.Value = pointOffsetTool.buchang.Y;

                                //////    if (pointOffsetTool.toolPar.InputPar.点.ToString() == "")
                                //////    {
                                //////        Frm_PointOffsetTool.Instance.tbx_inputPosX.Text = ((List<Point>)pointOffsetTool.toolPar.InputPar.点)[pointOffsetTool.pointIdx - 1].X.ToString();
                                //////        Frm_PointOffsetTool.Instance.tbx_inputPosY.Text = ((List<Point>)pointOffsetTool.toolPar.InputPar.点)[pointOffsetTool.pointIdx - 1].Y.ToString();
                                //////    }
                                //////    else
                                //////    {

                                //////    }

                                //////    Frm_PointOffsetTool.Instance.tbx_resultPosX.Text = pointOffsetTool.toolPar.ResultPar.点.X.ToString();
                                //////    Frm_PointOffsetTool.Instance.tbx_resultPosY.Text = pointOffsetTool.toolPar.ResultPar.点.Y.ToString();

                                //////    Frm_PointOffsetTool.Instance.pictureBox8.Image = L_toolList[i].enable ? Resources.Enable : Resources.Disable;

                                //////    if (pointOffsetTool.toolPar.InputPar.点.ToString() == "System.Collections.Generic.List`1[VisionAndMotionPro.Point]")
                                //////        Frm_PointOffsetTool.Instance.comboBox1.Visible = true;
                                //////    else
                                //////        Frm_PointOffsetTool.Instance.comboBox1.Visible = false;
                                //////    break;
                                //////#endregion

                                //////#region DisplayEdit
                                //////case ToolType.DisplayEdit:
                                //////    Frm_DisplayEditTool.Instance.lbl_title.Text = string.Format("显示编辑    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////    //////Frm_ShapeMatchTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    //////Frm_ShapeMatchTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ShapeMatchTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    //Frm_ShapeMatchTool.Instance.TopMost = true;
                                //////    Frm_DisplayEditTool.Instance.Activate();
                                //////    Frm_DisplayEditTool.Instance.jobName = this.jobName;
                                //////    Frm_DisplayEditTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_DisplayEditTool.Instance.Show();
                                //////    Frm_DisplayEditTool.Instance.WindowState = FormWindowState.Normal;
                                //////    Frm_DisplayEditTool.Instance.btn_runTool.Focus();
                                //////    DisplayEditTool displayEditTool = (DisplayEditTool)(L_toolList[i].tool);
                                //////    Frm_DisplayEditTool.displayEditTool = displayEditTool;
                                //////    Application.DoEvents();





                                //////    //将对象信息更新到界面

                                //////    break;
                                //////#endregion

                                //////#region EthernetReceive
                                //////case ToolType.EthernetReceive:
                                //////    Frm_EthernetReceiveTool.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "SDK_PointGray" : string.Format("采集图像    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName));
                                //////    ////// Frm_AcqImageTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    ////// Frm_AcqImageTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width -  Frm_AcqImageTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    // Frm_AcqImageTool.Instance.TopMost = true;
                                //////    Frm_EthernetReceiveTool.Instance.Activate();
                                //////    Frm_EthernetReceiveTool.Instance.jobName = this.jobName;
                                //////    Frm_EthernetReceiveTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_EthernetReceiveTool.ethernetReceiveTool = (EthernetReceiveTool)FindToolByName(L_toolList[i].toolName);
                                //////    Frm_EthernetReceiveTool.Instance.Show();
                                //////    Frm_EthernetReceiveTool.Instance.WindowState = FormWindowState.Normal;
                                //////    EthernetReceiveTool ethernetReceiveTool = (EthernetReceiveTool)(L_toolList[i].tool);
                                //////    Application.DoEvents();





                                //////    //将对象信息更新到界面
                                //////    Frm_EthernetReceiveTool.Instance.pic_onOff.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                //////    Frm_EthernetReceiveTool.Instance.comboBox1222.Clear();
                                //////    for (int j = 0; j < Project.Instance.L_TCPClient.Count; j++)
                                //////    {
                                //////        Frm_EthernetReceiveTool.Instance.comboBox1222.Add(Project.Instance.L_TCPClient[j].Name);
                                //////    }
                                //////    for (int j = 0; j < Project.Instance.L_TCPSever.Count; j++)
                                //////    {
                                //////        Frm_EthernetReceiveTool.Instance.comboBox1222.Add(Project.Instance.L_TCPSever[j].Name);
                                //////    }
                                //////    Frm_EthernetReceiveTool.Instance.comboBox1222.TextStr = ethernetReceiveTool.EthernetName;
                                //////    Frm_EthernetReceiveTool.Instance.tbx_imageSavePath.TextStr = ethernetReceiveTool.trigCMD;
                                //////    switch (ethernetReceiveTool.endChar)
                                //////    {
                                //////        case "":
                                //////            Frm_EthernetReceiveTool.Instance.btn_endCharNone.BackColor = Color.Gray;
                                //////            Frm_EthernetReceiveTool.Instance.btn_endCharEnter.BackColor = Color.Gainsboro;
                                //////            break;
                                //////        case "\r\n":
                                //////            Frm_EthernetReceiveTool.Instance.btn_endCharNone.BackColor = Color.Gainsboro;
                                //////            Frm_EthernetReceiveTool.Instance.btn_endCharEnter.BackColor = Color.Gray;
                                //////            break;
                                //////    }
                                //////    break;
                                //////#endregion

                                //////#region EthernetSend
                                //////case ToolType.EthernetSend:
                                //////    Frm_EthernetSendTool.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "SDK_PointGray" : string.Format("采集图像    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName));
                                //////    ////// Frm_AcqImageTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////    ////// Frm_AcqImageTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width -  Frm_AcqImageTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //////    // Frm_AcqImageTool.Instance.TopMost = true;
                                //////    Frm_EthernetSendTool.Instance.Activate();
                                //////    Frm_EthernetSendTool.Instance.jobName = this.jobName;
                                //////    Frm_EthernetSendTool.Instance.toolName = L_toolList[i].toolName;
                                //////    Frm_EthernetSendTool.ethernetReceiveTool = (EthernetSendTool)FindToolByName(L_toolList[i].toolName);
                                //////    Frm_EthernetSendTool.Instance.Show();
                                //////    Frm_EthernetSendTool.Instance.WindowState = FormWindowState.Normal;
                                //////    EthernetSendTool ethernetSendTool = (EthernetSendTool)(L_toolList[i].tool);
                                //////    Application.DoEvents();





                                //////    //将对象信息更新到界面
                                //////    Frm_EthernetSendTool.Instance.pic_onOff.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                //////    Frm_EthernetSendTool.Instance.comboBox1222.Clear();
                                //////    for (int j = 0; j < Project.Instance.L_TCPClient.Count; j++)
                                //////    {
                                //////        Frm_EthernetSendTool.Instance.comboBox1222.Add(Project.Instance.L_TCPClient[j].Name);
                                //////    }
                                //////    for (int j = 0; j < Project.Instance.L_TCPSever.Count; j++)
                                //////    {
                                //////        Frm_EthernetSendTool.Instance.comboBox1222.Add(Project.Instance.L_TCPSever[j].Name);
                                //////    }
                                //////    Frm_EthernetSendTool.Instance.comboBox1222.TextStr = ethernetSendTool.EthernetName;
                                //////    Frm_EthernetSendTool.Instance.tbx_imageSavePath.TextStr = ethernetSendTool.toolPar.InputPar.消息;
                                //////    switch (ethernetSendTool.endChar)
                                //////    {
                                //////        case "":
                                //////            Frm_EthernetSendTool.Instance.btn_endCharNone.BackColor = Color.Gray;
                                //////            Frm_EthernetSendTool.Instance.btn_endCharEnter.BackColor = Color.Gainsboro;
                                //////            break;
                                //////        case "\r\n":
                                //////            Frm_EthernetSendTool.Instance.btn_endCharNone.BackColor = Color.Gainsboro;
                                //////            Frm_EthernetSendTool.Instance.btn_endCharEnter.BackColor = Color.Gray;
                                //////            break;
                                //////    }
                                //////    break;
                                //////#endregion

                                #region Label
                                case ToolType.Label:
                                    LabelTool labelTool = (LabelTool)L_toolList[i].tool;

                                    GetImageWindowControl().hwc_imageWindow.HobjectToHimage(GetImageWindowControl().currentImage);

                                    labelTool.Run(true, false, L_toolList[i].toolName);
                                    break;
                                #endregion

                                //////#region Output
                                //////case ToolType.Output:
                                //////    //Frm_OutputBoxTool.Instance.Text = "输出 - " + this.jobName + "." + L_toolList[i].toolName;
                                //////    //Frm_OutputBoxTool.Instance.TopMost = true;
                                //////    //Frm_OutputBoxTool.Instance.jobName = this.jobName;
                                //////    //Frm_OutputBoxTool.Instance.toolName = L_toolList[i].toolName; ;
                                //////    //Frm_OutputBoxTool.Instance.Show();
                                //////    //Frm_OutputBoxTool.Instance.WindowState = FormWindowState.Normal;
                                //////    //Frm_OutputBoxTool.Instance.b.Focus();
                                //////    //OutputTool outputTool = (OutputTool)(L_toolList[i].tool);
                                //////    //Application.DoEvents();

                                //////    //将对象信息更新到界面
                                //////    //Frm_OutputBoxTool.Instance.ckb_outputBoxToolNotRun.Checked = L_toolList[i].enable;
                                //////    Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "\r\nThis tool is a form-free tool" : "\r\n本工具为无窗体工具！");
                                //////    break;
                                //////#endregion

                                #region Default
                                default:
                                    GetImageWindowControl().hwc_imageWindow.HobjectToHimage(GetImageWindowControl().currentImage);
                                    break;
                                    #endregion
                            }







                        }
                    }
                });
                th.IsBackground = true;
                th.Start();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 流程树右击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void TVW_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;

                if (isRunLoop)
                {
                    GetJobTree(jobName).ContextMenuStrip = null;
                    return;
                }


                if (GetJobTree().SelectedNode == null)
                    return;




                if (e.Button == MouseButtons.Left && GetJobTree().SelectedNode != null)      //如果是鼠标左击，就改工具名
                {

                    TreeView treeView = (TreeView)sender;
                    if (e.Button == MouseButtons.Right)
                    {
                        if (treeView.SelectedNode != null)
                            treeView.SelectedNode.BeginEdit();
                    }
                    return;
                }

                //判断是否在节点单击
                TreeViewHitTestInfo test = GetJobTree().HitTest(e.X, e.Y);
                if (test.Node == null || test.Location != TreeViewHitTestLocations.Label && e.Button == MouseButtons.Right)       //单击空白
                {
                    GetJobTree().ContextMenuStrip = rightClickMenuAtBlank;
                    rightClickMenuAtBlank.Show(e.X, e.Y);
                    return;
                }
                else
                {
                    GetJobTree().ContextMenuStrip = rightClickMenu;
                }

                //右键流程根节点：提供"保存当前流程"选项
                //注意：工具节点本身就是树的顶级节点，节点文本为工具名，不能用 FindJobByName 去查找，
                //否则每次右键工具都会因"未找到名为[工具名]的流程"而弹窗报错
                if (e.Button == MouseButtons.Right && GetJobTree().SelectedNode != null && GetJobTree().SelectedNode.Text == jobName)
                {
                    rightClickMenu.Items.Clear();
                    ToolStripItem saveJobItem = rightClickMenu.Items.Add(
                        Project.Instance.configuration.language == Language.English ? "Save Current Job" : "保存当前流程");
                    saveJobItem.BackColor = Color.White;
                    saveJobItem.Click += new EventHandler(SaveCurrentJob);
                    GetJobTree().ContextMenuStrip = rightClickMenu;
                    rightClickMenu.Show();
                    Application.DoEvents();
                    return;
                }

                if (IsCodeEditOutputNode(GetJobTree().SelectedNode))
                {
                    rightClickMenu.Items.Clear();
                    ToolStripItem deleteOutput = rightClickMenu.Items.Add(
                        Project.Instance.configuration.language == Language.English ? "DeleteItem" : "删除项");
                    deleteOutput.BackColor = Color.White;
                    deleteOutput.Click += new EventHandler(DeleteItem);
                    GetJobTree().ContextMenuStrip = rightClickMenu;
                    rightClickMenu.Show();
                    Application.DoEvents();
                    return;
                }

                rightClickMenu.Items.Clear();
                rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "Add Input" : "编辑终端");
                rightClickMenu.Items[0].Click += new EventHandler(ShowIOForm);


                rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "Enable" : "运行");
                rightClickMenu.Items[1].Click += new EventHandler(RunTool);

                rightClickMenu.Items.Add(FindToolInfoByName(GetJobTree().SelectedNode.Text).enable ? "禁用" : "启用");
                rightClickMenu.Items[2].Click += new EventHandler(EnableOrDisenableTool);



                rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "InsertTool" : "插入工具");

                rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "Rename" : "复制");
                rightClickMenu.Items[4].Click += new EventHandler(CopyTool);
                rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "Rename" : "粘贴");
                rightClickMenu.Items[5].Click += new EventHandler(PasteTool);
                rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "DeleteItem" : "删除");
                rightClickMenu.Items[6].Image = Resources.删_除4;
                rightClickMenu.Items[6].Click += new EventHandler(DeleteItem);
                rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "Rename" : "重命名");
                rightClickMenu.Items[7].Click += new EventHandler(RenameTool);
                rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "Rename" : "编辑说明");
                rightClickMenu.Items[8].Click += new EventHandler(ModifyTipInfo);

                //如果不是第一个则添加上移选项
                if (GetJobTree().SelectedNode == null)
                    return;
                if (GetJobTree().SelectedNode.Index != 0)
                {
                    rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "MoveUp" : "上移");
                    rightClickMenu.Items[9].Click += new EventHandler(MoveUp);
                    rightClickMenu.Items[9].Image = Resources.MoveUp;
                    if (GetJobTree().SelectedNode.Index != GetJobTree().Nodes.Count - 1)
                    {
                        rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "MoveDown" : "下移");
                        rightClickMenu.Items[10].Click += new EventHandler(MoveDown);
                        rightClickMenu.Items[10].Image = Resources.MoveDown;
                    }
                }
                else
                {
                    rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "MoveDown" : "下移");
                    rightClickMenu.Items[9].Click += new EventHandler(MoveDown);
                    rightClickMenu.Items[9].Image = Resources.MoveDown;
                }

                //白色背景好看
                for (int i = 0; i < rightClickMenu.Items.Count; i++)
                {
                    ((ToolStripItem)rightClickMenu.Items[i]).BackColor = Color.White;
                }

                if (e.Button == MouseButtons.Right && e.Clicks == 1)        //如果右击
                {
                    ToolInfo toolInfo = FindToolInfoByName(GetJobTree().SelectedNode.Text);

                    //清空输入，输出下拉选项
                    Application.DoEvents();

                    bool clickToolNode = true;              //操作的是工具节点

                    #region 显示源
                    if (GetJobTree().SelectedNode.Level == 1)
                    {

                        //指定源
                        clickToolNode = false;
                        string nodeText = GetJobTree().SelectedNode.Text;
                        string fatherNodeText = GetJobTree().SelectedNode.Parent.Text;
                        string curNodeType = GetJobTree().SelectedNode.Tag.ToString();
                        ToolStripMenuItem item111 = null;
                        ToolStripMenuItem globalSourceMenu = null;
                        ToolStripMenuItem systemGlobalSourceMenu = null;
                        ToolStripMenuItem customGlobalSourceMenu = null;
                        ToolStripMenuItem currentJobSourceMenu = null;
                        ToolStripMenuItem otherJobSourceMenu = null;
                        //当前流程可源项
                        foreach (TreeNode toolNode in GetJobTree().Nodes)
                        {
                            foreach (TreeNode itemNode in ((TreeNode)toolNode).Nodes)
                            {
                                if (((TreeNode)itemNode).Text == nodeText)
                                {
                                    rightClickMenu.Items.Clear();
                                    ToolStripItem sourceFrom = rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "SourceFrom" : "源于");
                                    sourceFrom.BackColor = Color.White;
                                    ToolStripItem deleteItem = rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "DeleteItem" : "删除项");
                                    deleteItem.BackColor = Color.White;
                                    deleteItem.Click += new EventHandler(DeleteItem);
                                    //////ToolStripItem copyNodeText = rightClickMenu.Items.Add(Project.Instance.configuration.language == Language.English ? "CopyNodeText" : "复制节点文本");
                                    //////copyNodeText.Click += new EventHandler(CopyNodeText);
                                    //////copyNodeText.BackColor = Color.White;

                                    item111 = rightClickMenu.Items[0] as ToolStripMenuItem;
                                    ((ToolStripMenuItem)rightClickMenu.Items[0]).DropDownItems.Clear();
                                    globalSourceMenu = AddSourceCategory(item111, Project.Instance.configuration.language == Language.English ? "Global" : "全局");
                                    systemGlobalSourceMenu = AddSourceCategory(globalSourceMenu, Project.Instance.configuration.language == Language.English ? "System" : "系统变量");
                                    customGlobalSourceMenu = AddSourceCategory(globalSourceMenu, Project.Instance.configuration.language == Language.English ? "Custom" : "自定义变量");
                                    currentJobSourceMenu = AddSourceCategory(item111, Project.Instance.configuration.language == Language.English ? "Current Job" : "当前流程");
                                    otherJobSourceMenu = AddSourceCategory(item111, Project.Instance.configuration.language == Language.English ? "Other Jobs" : "其他流程");

                                    foreach (TreeNode toolNode1 in GetJobTree().Nodes)
                                    {
                                        if (toolNode1.Text == fatherNodeText)        //不能指定自己的输出项为源
                                            continue;
                                        if (((TreeNode)toolNode1).Text != (Project.Instance.configuration.language == Language.English ? "OutputTool" : "输出项"))
                                        {
                                            foreach (TreeNode itemNode1 in ((TreeNode)toolNode1).Nodes)
                                            {
                                                string sourceType = itemNode1.Tag.ToString();
                                                if (CanConnectSourceType(fatherNodeText, curNodeType, sourceType))
                                                {
                                                    if (((TreeNode)itemNode1).Text.Substring(0, 3) != "<--")
                                                    {
                                                        string resultStr = "《- " + toolNode1.Text + "->" + itemNode1.Text.Substring(3);
                                                        AddSourceItem(currentJobSourceMenu, resultStr);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }



                        //其它流程可源项
                        foreach (Job job in Project.Instance.curEngine.L_jobList)
                        {
                            if (job.jobName == jobName)
                                continue;
                            ToolStripMenuItem jobSourceMenu = null;
                            foreach (TreeNode toolNode1 in job.GetJobTree().Nodes)
                            {
                                if (((TreeNode)toolNode1).Text != (Project.Instance.configuration.language == Language.English ? "OutputTool" : "输出项"))
                                {
                                    foreach (TreeNode itemNode1 in ((TreeNode)toolNode1).Nodes)
                                    {
                                        string sourceType = itemNode1.Tag.ToString();
                                        if (CanConnectSourceType(fatherNodeText, curNodeType, sourceType))
                                        {
                                            if (((TreeNode)itemNode1).Text.Substring(0, 3) != "<--")
                                            {
                                                string resultStr = "《- [" + job.jobName + "]" + toolNode1.Text + "->" + itemNode1.Text.Substring(3);
                                                if (jobSourceMenu == null)
                                                    jobSourceMenu = AddSourceCategory(otherJobSourceMenu, job.jobName);
                                                AddSourceItem(jobSourceMenu, resultStr);
                                            }
                                        }
                                    }
                                }
                            }

                        }


                        //全局变量可源项
                        for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
                        {
                            Variable sourceVariable = Project.Instance.curEngine.globelVariable.L_variable[i];
                            ToolInfo sourceTargetTool = FindToolInfoByName(fatherNodeText);
                            bool isCodeEditInput = sourceTargetTool != null && sourceTargetTool.toolType == ToolType.CodeEdit;
                            bool isSupportedCodeEditType = sourceVariable.type == "Int" || sourceVariable.type == "Double" || sourceVariable.type == "String" || sourceVariable.type == "Bool";
                            if ((isCodeEditInput && isSupportedCodeEditType) || (!isCodeEditInput && (sourceVariable.type == "String" || sourceVariable.type == "Double")))
                            {
                                string resultStr = "《- 全局变量->" + sourceVariable.name;
                                if (sourceVariable.variableType == 0)
                                    AddSourceItem(systemGlobalSourceMenu, resultStr);
                                else
                                    AddSourceItem(customGlobalSourceMenu, resultStr);
                            }
                        }
                    }
                    #endregion



                    GetJobTree().ContextMenuStrip = rightClickMenu;


                    rightClickMenu.Show();
                    Application.DoEvents();

                    #region 插入工具
                    if (clickToolNode)
                    {
                        Thread th = new Thread(() =>
                        {
                            ToolStripItem toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "AcqDevice" : "图像相关");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[0]).DropDownItems.Add("采集图像");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.ImageAcqTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[0]).DropDownItems.Add("预处理");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.ImageProprocessingTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[0]).DropDownItems.Add("彩图转RGB图");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.ColorToRGBTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[0]).DropDownItems.Add("存储图像");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.SaveImageTool;
                                toolStripItem1.Click += InsertTool;
                            }

                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "Match" : "检测识别");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[1]).DropDownItems.Add("模板匹配");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.MatchTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[1]).DropDownItems.Add("斑点分析");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.BlobAnalyseTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[1]).DropDownItems.Add("图像相减");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.SubImageTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[1]).DropDownItems.Add("区域特征");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.RegionFeatureTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[1]).DropDownItems.Add("区域运算");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.UnknownTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[1]).DropDownItems.Add("OCR");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.OCRTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[1]).DropDownItems.Add("OCV");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.UnknownTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[1]).DropDownItems.Add("条码");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.BarCodeTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[1]).DropDownItems.Add("二维码");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.QRTool;
                                toolStripItem1.Click += InsertTool;

                            }
                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "CoorTrans" : "坐标变换");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[2]).DropDownItems.Add("手眼标定");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.EyeHandCalibTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[2]).DropDownItems.Add("一键手眼标定");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.EyeHandCalibTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[2]).DropDownItems.Add("尺寸标定");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.DimensionCalibTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[2]).DropDownItems.Add("一维标定");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.DimensionCalibTool;
                                toolStripItem1.Click += InsertTool;
                            }
                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "RobotAlign" : "定位引导");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[3]).DropDownItems.Add("上相机定位");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.UnknownTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[3]).DropDownItems.Add("下相机定位");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.DownCamAlignTool;
                                toolStripItem1.Click += InsertTool;
                            }
                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "Calibration" : "逻辑控制");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[4]).DropDownItems.Add("循环");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.UnknownTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[4]).DropDownItems.Add("分支");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.UnknownTool;
                                toolStripItem1.Click += InsertTool;
                            }

                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "FindAndFit" : "查找拟合");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[5]).DropDownItems.Add("查找边");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.FindLineTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[5]).DropDownItems.Add("查找圆");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.FindCircleTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[5]).DropDownItems.Add("拟合线");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.FitLineTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[5]).DropDownItems.Add("拟合圆");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.FitCircleTool;
                                toolStripItem1.Click += InsertTool;
                            }

                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "Detection" : "创建组合");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[6]).DropDownItems.Add("创建ROI");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.CreateROITool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[6]).DropDownItems.Add("组合位置");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.CreatePosTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[6]).DropDownItems.Add("组合线段");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.CreateSegmentTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[6]).DropDownItems.Add("阵列区域");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.RegionArrayTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[6]).DropDownItems.Add("标记点");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.MarkTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[6]).DropDownItems.Add("数据显示");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.UnknownTool;
                                toolStripItem1.Click += InsertTool;
                            }

                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "Measurement" : "几何相关");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[7]).DropDownItems.Add("点点距离");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.DistancePPTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[7]).DropDownItems.Add("点线距离");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.DistancePLTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[7]).DropDownItems.Add("线线角度");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.AngleLLTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[7]).DropDownItems.Add("线线距离");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.DistanceLLTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[7]).DropDownItems.Add("线线交点");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.AngleLLTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[7]).DropDownItems.Add("两点中线");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.TwoPointCenterTool;
                                toolStripItem1.Click += InsertTool;
                            }

                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "Operation" : "运算相关");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[8]).DropDownItems.Add("算术");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.UnknownTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[8]).DropDownItems.Add("脚本编辑");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.CSharpScriptTool;
                                toolStripItem1.Click += InsertTool;
                            }
                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "Light" : "仪器仪表");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[9]).DropDownItems.Add("奥普特光源控制");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.LightTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[9]).DropDownItems.Add("康视达光源控制");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.LightTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[9]).DropDownItems.Add("乐视光源控制");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.LightTool;
                                toolStripItem1.Click += InsertTool;
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[9]).DropDownItems.Add("光源控制");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.LightTool;
                                toolStripItem1.Click += InsertTool;
                            }
                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "Light" : "3D  检测");
                            toolStripItem1.BackColor = Color.White;
                            {

                            }
                            toolStripItem1 = ((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems.Add(Project.Instance.configuration.language == Language.English ? "Light" : "其它");
                            toolStripItem1.BackColor = Color.White;
                            {
                                toolStripItem1 = ((ToolStripMenuItem)((ToolStripMenuItem)rightClickMenu.Items[3]).DropDownItems[11]).DropDownItems.Add("输出项");
                                toolStripItem1.BackColor = Color.White;
                                toolStripItem1.Image = Resources.LightTool;
                                toolStripItem1.Click += InsertTool;
                            }



                        });
                        th.IsBackground = true;
                        th.Start();
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 流程树的双击事件
        /// </summary>
        internal void TVW_DoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                doubleClick = true;
                //判断是否在节点上双击
                TreeViewHitTestInfo test = GetJobTree().HitTest(e.X, e.Y);
                if (test.Node == null || test.Location != TreeViewHitTestLocations.Label)       //双击节点
                {
                    return;         //未启用
                    if (jobTreeFold)
                    {
                        GetJobTree().ExpandAll();
                        jobTreeFold = false;
                    }
                    else
                    {
                        GetJobTree().CollapseAll();
                        jobTreeFold = true;
                    }
                    return;
                }

                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;

                loadForm = true;
                TreeNode treeNode = GetJobTree().SelectedNode;
                if (treeNode == null)           //如果流程正在运行，可能会没有选中节点
                {
                    Frm_Main.Instance.OutputMsg("流程正在运行，不可编辑,请先停止运行", Color.Black);
                    return;
                }
                string toolName = treeNode.Text;

                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolName == toolName)
                    {
                        switch (L_toolList[i].toolType)
                        {
                            #region ImageAcq
                            case ToolType.ImageAcq:
                                Frm_AcqImageTool.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "SDK_PointGray" : string.Format("采集图像    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName));
                                ////// Frm_AcqImageTool.Instance.StartPosition = FormStartPosition.Manual;
                                ////// Frm_AcqImageTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width -  Frm_AcqImageTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                // Frm_AcqImageTool.Instance.TopMost = true;
                                Frm_AcqImageTool.Instance.Activate();
                                Frm_AcqImageTool.Instance.jobName = this.jobName;
                                Frm_AcqImageTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_AcqImageTool.Instance.acqImageTool = (AcqImageTool)FindToolByName(L_toolList[i].toolName);
                                Frm_FromLocal.Instance.jobName = this.jobName;
                                Frm_FromLocal.Instance.toolName = L_toolList[i].toolName;
                                Frm_FromDevice.Instance.jobName = this.jobName;
                                Frm_FromDevice.Instance.toolName = L_toolList[i].toolName;
                                Frm_FromDevice.imageAcqTool = ((AcqImageTool)Job.FindToolByName(this.jobName, L_toolList[i].toolName));
                                Frm_AcqImageTool.Instance.Show();
                                Frm_AcqImageTool.Instance.WindowState = FormWindowState.Normal;
                                AcqImageTool SDK_hikVisionTool = (AcqImageTool)(L_toolList[i].tool);
                                bool hasImageOutput = false;
                                string imageOutputName = Project.Instance.configuration.language == Language.English ? "OutputImage" : "图像";
                                for (int outputIdx = 0; outputIdx < L_toolList[i].output.Count; outputIdx++)
                                {
                                    if (L_toolList[i].output[outputIdx].IOName == imageOutputName || L_toolList[i].output[outputIdx].IOName == "图像" || L_toolList[i].output[outputIdx].IOName == "输出图像" || L_toolList[i].output[outputIdx].IOName == "OutputImage")
                                    {
                                        hasImageOutput = true;
                                        break;
                                    }
                                }
                                if (!hasImageOutput)
                                {
                                    L_toolList[i].output.Add(new ToolIO(imageOutputName, "", DataType.Image));
                                    TreeNode acqToolNode = GetToolNodeByNodeText(L_toolList[i].toolName);
                                    if (acqToolNode != null)
                                    {
                                        TreeNode imageNode = acqToolNode.Nodes.Add("-->" + imageOutputName);
                                        imageNode.ForeColor = Color.Blue;
                                        imageNode.Tag = DataType.Image;
                                    }
                                }

                                bool hasCustomPathOutput = false;
                                string customPathOutputName = Project.Instance.configuration.language == Language.English ? "CustomPath" : "自定义路径";
                                for (int outputIdx = 0; outputIdx < L_toolList[i].output.Count; outputIdx++)
                                {
                                    if (L_toolList[i].output[outputIdx].IOName == customPathOutputName || L_toolList[i].output[outputIdx].IOName == "自定义路径" || L_toolList[i].output[outputIdx].IOName == "CustomPath")
                                    {
                                        hasCustomPathOutput = true;
                                        break;
                                    }
                                }
                                if (!hasCustomPathOutput)
                                {
                                    L_toolList[i].output.Add(new ToolIO(customPathOutputName, "", DataType.String));
                                    TreeNode acqToolNode = GetToolNodeByNodeText(L_toolList[i].toolName);
                                    if (acqToolNode != null)
                                    {
                                        TreeNode customPathNode = acqToolNode.Nodes.Add("-->" + customPathOutputName);
                                        customPathNode.ForeColor = Color.Blue;
                                        customPathNode.Tag = DataType.String;
                                    }
                                }
                                Frm_FromLocal.imageAcqTool = SDK_hikVisionTool;
                                Application.DoEvents();

                                // 重新进入工具时不要主动清掉用户已选择或已采集的图像。
                                SDK_hikVisionTool.EnsurePreviewImage();
                                SDK_hikVisionTool.UpdateOutput(L_toolList[i].toolName);
                                if (SDK_hikVisionTool.toolPar.ResultPar.图像 != null)
                                    Frm_AcqImageTool.Instance.hWindow_Final1.HobjectToHimage(SDK_hikVisionTool.toolPar.ResultPar.图像);
                                else
                                    Frm_AcqImageTool.Instance.hWindow_Final1.ClearWindow();
                                // 恢复用户保存的图像源模式：相机输出(FromDevice) / 固定路径单张图像(FromFile) / 固定路径文件夹(FromDirectory)
                                SDK_hikVisionTool.SwitchImageSource(SDK_hikVisionTool.imageSourceMode);
                                Frm_AcqImageTool.Instance.ckb_rotateImage.Visible = true;
                                Frm_AcqImageTool.Instance.ckb_RGBToGray.Visible = false;
                                Frm_AcqImageTool.Instance.ckb_displayAllImageRegion.Visible = false;
                                // 恢复本地图像路径显示
                                if (SDK_hikVisionTool.imageSourceMode == ImageSourceMode.FromFile)
                                    Frm_FromLocal.Instance.tbx_imagePath.Text = SDK_hikVisionTool.imagePath;
                                else if (SDK_hikVisionTool.imageSourceMode == ImageSourceMode.FromDirectory)
                                    Frm_FromLocal.Instance.tbx_imageDirectoryPath.Text = SDK_hikVisionTool.imageDirectoryPath;

                                //将对象信息更新到界面
                                Frm_AcqImageTool.Instance.pic_onOff.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                //if (SDK_hikVisionTool.deviceInfoStr != string.Empty)
                                //{
                                Frm_FromDevice.Instance.LoadExposureBrightness(SDK_hikVisionTool.exposure);
                                //}

                                Application.DoEvents();
                                Frm_AcqImageTool.Instance.ckb_displayAllImageRegion.Checked = SDK_hikVisionTool.displayAllImageRegion;

                                Frm_AcqImageTool.Instance.cCheckBox1.Checked = SDK_hikVisionTool.hardTriggerMode;
                                Frm_AcqImageTool.Instance.ckb_RGBToGray.Checked = false;
                                Frm_AcqImageTool.Instance.ckb_absPath.Checked = SDK_hikVisionTool.absPath;
                                Frm_AcqImageTool.Instance.ckb_autoSwitch.Checked = SDK_hikVisionTool.autoSwitch;

                                Frm_FromDevice.Instance.tbx_saveDirectory.Text = SDK_hikVisionTool.customSaveDirectory;
                                Frm_FromDevice.Instance.ckb_useTemplateImage.Checked = SDK_hikVisionTool.useTemplateImageInRun;
                                Frm_FromDevice.Instance.tbx_templateImagePath.Text = SDK_hikVisionTool.templateImagePath;
                                Frm_AcqImageTool.Instance.lbl_toolTip.ForeColor = Color.Black;
                                Frm_AcqImageTool.Instance.lbl_toolTip.Text = "状态：就绪";
                                Frm_AcqImageTool.Instance.ckb_rotateImage.Checked = SDK_hikVisionTool.rotateImage;
                                Frm_AcqImageTool.Instance.nud_rotateAngle.Value = SDK_hikVisionTool.rotateAngle;
                                Frm_AcqImageTool.Instance.btn_runTool.Focus();
                                break;
                            #endregion

                            #region ImagePreprocessing
                            case ToolType.ImagePreprocessing:
                                Frm_ImageProprecessingTool.Instance.lbl_title.Text = string.Format("预处理    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_ShapeMatchTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_ShapeMatchTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ShapeMatchTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_ShapeMatchTool.Instance.TopMost = true;
                                Frm_ImageProprecessingTool.Instance.Activate();
                                Frm_ImageProprecessingTool.Instance.jobName = this.jobName;
                                Frm_ImageProprecessingTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_ImageProprecessingTool.Instance.Show();
                                Frm_ImageProprecessingTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_ImageProprecessingTool.Instance.btn_runTool.Focus();
                                ImageProprecessingTool imageProprecessingTool = (ImageProprecessingTool)(L_toolList[i].tool);
                                Frm_ImageProprecessingTool.imageProprecessingTool = imageProprecessingTool;

                                Frm_BinaryThreshold.imageProprecessingTool = imageProprecessingTool;
                                Frm_BinaryThreshold.jobName = jobName;
                                Application.DoEvents();


                                int inputItemNum = (L_toolList[i]).input.Count;

                                for (int j = 0; j < inputItemNum; j++)
                                {
                                    string inputItemName = L_toolList[i].input[j].IOName;
                                    string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                                    if (sourceFrom == string.Empty)
                                    {
                                        continue;
                                    }
                                    if (inputItemName == "输入图像" || inputItemName == "InputImage")
                                    {
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        imageProprecessingTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                        if (imageProprecessingTool.inputImage == null)
                                        {
                                            continue;
                                        }
                                    }

                                }


                                if (imageProprecessingTool.inputImage != null)
                                    Frm_ImageProprecessingTool.Instance.hWindow_Final1.HobjectToHimage(imageProprecessingTool.inputImage);
                                else
                                    Frm_ImageProprecessingTool.Instance.hWindow_Final1.ClearWindow();

                                Frm_ImageProprecessingTool.Instance.dataGridView1.Rows.Clear();
                                for (int j = 0; j < imageProprecessingTool.L_item.Count; j++)
                                {
                                    int idx = Frm_ImageProprecessingTool.Instance.dataGridView1.Rows.Add();
                                    ((DataGridViewCheckBoxCell)Frm_ImageProprecessingTool.Instance.dataGridView1.Rows[idx].Cells[0]).Value = imageProprecessingTool.L_item[j].enable;
                                    Frm_ImageProprecessingTool.Instance.dataGridView1.Rows[idx].Cells[1].Value = imageProprecessingTool.L_item[j].type;
                                    Frm_ImageProprecessingTool.Instance.dataGridView1.Rows[idx].Cells[2].Value = imageProprecessingTool.L_item[j].itemName;
                                }

                                if (Frm_ImageProprecessingTool.Instance.dataGridView1.Rows.Count > 0)
                                    Frm_ImageProprecessingTool.Instance.dataGridView1.Rows[0].Selected = true;

                                //if (imageProprecessingTool.SearchRegion != null)
                                //{
                                //    Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayROI(imageProprecessingTool.L_regions);
                                //    Frm_ShapeMatchTool.Instance.regions = imageProprecessingTool.L_regions;
                                //}

                                ////显示模板
                                //try
                                //{
                                //    if (imageProprecessingTool.modelID != -1)
                                //    {
                                //        HTuple row, col, row1, col1;
                                //        HOperatorSet.SmallestRectangle1(imageProprecessingTool.totalRegion, out row, out col, out row1, out col1);
                                //        HObject outRectangle1;
                                //        HOperatorSet.GenRectangle1(out outRectangle1, row - 30, col - 30, row1 + 30, col1 + 30);
                                //        HObject imageReduced;
                                //        HOperatorSet.ReduceDomain(imageProprecessingTool.standardImage, outRectangle1, out imageReduced);
                                //        HOperatorSet.SetPart(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, row - 30, col - 30, row1 + 30, col1 + 30);
                                //        HOperatorSet.DispObj(imageReduced, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                                //        HOperatorSet.SetDraw(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("margin"));
                                //        HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("green"));
                                //        HOperatorSet.DispObj(imageProprecessingTool.templateRegion, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);

                                //        int statu = imageProprecessingTool.CreateTemplate();
                                //        if (statu != 0)
                                //            return;
                                //        HObject contour;
                                //        HOperatorSet.GetShapeModelContours(out contour, imageProprecessingTool.modelID, (HTuple)1);
                                //        HTuple area1, row2, column2;
                                //        HOperatorSet.AreaCenter(imageProprecessingTool.totalRegion, out area1, out row2, out column2);
                                //        HTuple homMat2D;
                                //        HOperatorSet.HomMat2dIdentity(out homMat2D);
                                //        HOperatorSet.HomMat2dTranslate(homMat2D, row2, column2, out homMat2D);
                                //        HOperatorSet.AffineTransContourXld(contour, out contour, homMat2D);
                                //        HOperatorSet.SetColor(Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow, new HTuple("orange"));
                                //        HOperatorSet.DispObj(contour, Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow);
                                //    }
                                //    else
                                //    {
                                //        Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow.ClearWindow();
                                //    }
                                //}
                                //catch { }

                                //将对象信息更新到界面
                                //Frm_ShapeMatchTool.Instance.pictureBox2.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                //Frm_ShapeMatchTool.Instance.ckb_showCross.Checked = shapeMatchTool.showCross;
                                //Frm_ShapeMatchTool.Instance.cbx_showTemplate.Checked = shapeMatchTool.showTemplate;
                                //Frm_ShapeMatchTool.Instance.ckb_showFeature.Checked = shapeMatchTool.showFeature;
                                //Frm_ShapeMatchTool.Instance.cbx_searchRegionType.Text = (shapeMatchTool.searchRegionType == RegionType.None ? "" : shapeMatchTool.searchRegionType.ToString());
                                //Frm_ShapeMatchTool.Instance.nud_minScore.Value = Convert.ToDecimal(shapeMatchTool.minScore);
                                //Frm_ShapeMatchTool.Instance.nud_matchNum.Value = Convert.ToDecimal(shapeMatchTool.matchNum);
                                //Frm_ShapeMatchTool.Instance.nud_angleStart.Value = Convert.ToDecimal(shapeMatchTool.startAngle);
                                //Frm_ShapeMatchTool.Instance.nud_angleRange.Value = Convert.ToDecimal(shapeMatchTool.angleRange);
                                //Frm_ShapeMatchTool.Instance.nud_angleStep.Value = Convert.ToDecimal(shapeMatchTool.angleStep);
                                //Frm_ShapeMatchTool.Instance.tkb_contrast.Value = Convert.ToInt16(shapeMatchTool.contrast);
                                //Frm_ShapeMatchTool.Instance.cbx_polarity.Text = shapeMatchTool.polarity;
                                //Frm_ShapeMatchTool.Instance.comboBox1.SelectedIndex = (int)shapeMatchTool.sortMode;
                                //Frm_ShapeMatchTool.Instance.checkBox1.Checked = shapeMatchTool.showIndex;
                                //Frm_ShapeMatchTool.Instance.textBox1.Text = shapeMatchTool.spanPixelNum.ToString();

                                //Frm_ShapeMatchTool.Instance.pictureBox3.Image = (shapeMatchTool.showTemplate ? Resources.复选框 : Resources.去复选框);
                                //Frm_ShapeMatchTool.Instance.pictureBox4.Image = (shapeMatchTool.showCross ? Resources.复选框 : Resources.去复选框);
                                //Frm_ShapeMatchTool.Instance.pictureBox5.Image = (shapeMatchTool.showFeature ? Resources.复选框 : Resources.去复选框);
                                //Frm_ShapeMatchTool.Instance.pictureBox8.Image = (shapeMatchTool.showIndex ? Resources.复选框 : Resources.去复选框);
                                //Frm_ShapeMatchTool.Instance.pictureBox9.Image = (shapeMatchTool.showSearchRegion ? Resources.复选框 : Resources.去复选框);

                                //if (shapeMatchTool.angleStep == 0)
                                //{
                                //    Frm_ShapeMatchTool.Instance.nud_angleStep.Enabled = false;
                                //    Frm_ShapeMatchTool.Instance.ckb_autoStep.Checked = true;
                                //}
                                //else
                                //{
                                //    Frm_ShapeMatchTool.Instance.ckb_autoStep.Checked = false;
                                //}

                                //Frm_ShapeMatchTool.Instance.tbc_shapeMatch.SelectedIndex = 0;


                                //if (shapeMatchTool.modelID == -1)
                                //{
                                //    Frm_ShapeMatchTool.Instance.panel4.Visible = false;
                                //    Frm_ShapeMatchTool.Instance.panel17.Visible = false;
                                //    Frm_ShapeMatchTool.Instance.button5.Visible = false;
                                //    Frm_ShapeMatchTool.Instance.button9.Visible = false;
                                //}
                                //else
                                //{
                                //    Frm_ShapeMatchTool.Instance.panel4.Visible = true;
                                //    Frm_ShapeMatchTool.Instance.panel17.Visible = true;
                                //    Frm_ShapeMatchTool.Instance.button5.Visible = true;
                                //    Frm_ShapeMatchTool.Instance.button9.Visible = true;
                                //}
                                //Frm_ShapeMatchTool.Instance.hWindow_Final1.DispImageFit();




                                break;
                            #endregion

                            #region ColorToRGB
                            case ToolType.ColorToRGB:
                                Frm_ColorToRGBTool.Instance.lbl_title.Text = string.Format("彩图转RGB图    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_ColorToRGBTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_ColorToRGBTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ShapeMatchTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_ColorToRGBTool.Instance.TopMost = true;
                                Frm_ColorToRGBTool.Instance.Activate();

                                Frm_ColorToRGBTool.Instance.jobName = this.jobName;
                                Frm_ColorToRGBTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_ColorToRGBTool.Instance.Show();
                                Frm_ColorToRGBTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_ColorToRGBTool.Instance.btn_runColorToRGBTool.Focus();
                                ColorToRGBTool colorToRGBTool = (ColorToRGBTool)(L_toolList[i].tool);
                                Frm_ColorToRGBTool.colorToRGBTool = colorToRGBTool;
                                Application.DoEvents();

                                if (colorToRGBTool.inputImage != null)
                                    colorToRGBTool.ShowImage(colorToRGBTool.inputImage);
                                else
                                { }
                                //////colorToRGBTool.ClearWindow(this.jobName);

                                //将对象信息更新到界面
                                break;
                            #endregion

                            #region SaveImage
                            case ToolType.SaveImage:
                                Frm_SaveImageTool.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "SDK_PointGray - " : string.Format("存储图像    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName));
                                //Frm_SaveImageTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_SaveImageTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_SaveImageTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_SaveImageTool.Instance.TopMost = true;
                                Frm_SaveImageTool.Instance.Activate();
                                Frm_SaveImageTool.Instance.jobName = this.jobName;
                                Frm_SaveImageTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_SaveImageTool.saveImageTool = (SaveImageTool)FindToolByName(L_toolList[i].toolName);
                                Frm_SaveImageTool.Instance.jobName = this.jobName;
                                Frm_SaveImageTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_SaveImageTool.saveImageTool = ((SaveImageTool)Job.FindToolByName(this.jobName, L_toolList[i].toolName));
                                Frm_SaveImageTool.Instance.Show();
                                Frm_SaveImageTool.Instance.WindowState = FormWindowState.Normal;
                                ////Frm_SaveImageTool.Instance.btn_runSDKHIKVisionTool.Focus();
                                SaveImageTool saveImageTool = (SaveImageTool)(L_toolList[i].tool);
                                Application.DoEvents();




                                //将对象信息更新到界面
                                Frm_SaveImageTool.Instance.pictureBox2.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                Frm_SaveImageTool.Instance.tbx_imageSavePath.TextStr = saveImageTool.imageSavePath;
                                Frm_SaveImageTool.Instance.comboBox1.TextStr = saveImageTool.imageFormat;
                                Frm_SaveImageTool.Instance.textBox1.Value = saveImageTool.saveDays;
                                Frm_SaveImageTool.Instance.checkBox1.Checked = saveImageTool.expandTime;
                                Frm_SaveImageTool.Instance.checkBox2.Checked = saveImageTool.autoClear;
                                Frm_SaveImageTool.Instance.textBox2.TextStr = saveImageTool.imageName;
                                Frm_SaveImageTool.Instance.checkBox3.Checked = saveImageTool.autoCreateDirectory;
                                Frm_SaveImageTool.Instance.radioButton1.Checked = (saveImageTool.imageSource == ImageSource.InputImage ? true : false);
                                Frm_SaveImageTool.Instance.radioButton2.Checked = (saveImageTool.imageSource == ImageSource.InputImage ? false : true);
                                Frm_SaveImageTool.Instance.pictureBox8.Image = (saveImageTool.imageSource == ImageSource.InputImage ? Resources.勾选 : Resources.去勾选);
                                Frm_SaveImageTool.Instance.pictureBox7.Image = (saveImageTool.imageSource == ImageSource.WindowImage ? Resources.勾选 : Resources.去勾选);
                                break;
                            #endregion

                            #region ShapeMatch
                            case ToolType.Match:
                                Frm_ShapeMatchTool.Instance.lbl_title.Text = string.Format("模板匹配    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_ShapeMatchTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_ShapeMatchTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ShapeMatchTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_ShapeMatchTool.Instance.TopMost = true;
                                Frm_ShapeMatchTool.Instance.Activate();
                                Frm_ShapeMatchTool.Instance.jobName = this.jobName;
                                Frm_ShapeMatchTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_ShapeMatchTool.Instance.Show();
                                Frm_ShapeMatchTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_ShapeMatchTool.Instance.btn_runTool.Focus();
                                MatchTool shapeMatchTool = (MatchTool)(L_toolList[i].tool);
                                Frm_ShapeMatchTool.shapeMatchTool = shapeMatchTool;
                                Application.DoEvents();


                                inputItemNum = (L_toolList[i]).input.Count;

                                for (int j = 0; j < inputItemNum; j++)
                                {
                                    string inputItemName = L_toolList[i].input[j].IOName;
                                    string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                                    if (sourceFrom == string.Empty)
                                    {
                                        continue;
                                    }
                                    if (inputItemName == "图像" || inputItemName == "InputImage")
                                    {
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        ToolBase sourceTool = FindToolByName(sourceToolName);
                                        AcqImageTool sourceAcqTool = sourceTool as AcqImageTool;
                                        if (sourceAcqTool != null)
                                        {
                                            sourceAcqTool.ClearInternalSamplePreview();
                                            sourceAcqTool.UpdateOutput(sourceToolName);
                                        }
                                        shapeMatchTool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                        if (shapeMatchTool.toolPar.InputPar.图像 == null)
                                        {
                                            continue;
                                        }
                                    }

                                }


                                if (shapeMatchTool.toolPar.InputPar.图像 != null)
                                    Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(shapeMatchTool.toolPar.InputPar.图像);
                                else
                                    Frm_ShapeMatchTool.Instance.hWindow_Final1.ClearWindow();

                                // 兼容旧工程：AllImage 状态下必须清除序列化残留的 ROI/搜索区域。
                                if (shapeMatchTool.searchRegionType == RegionType.AllImage)
                                    shapeMatchTool.UseAllImageSearchRegion(false);

                                if (shapeMatchTool.searchRegionType != RegionType.AllImage && shapeMatchTool.SearchRegion != null)
                                {
                                    Frm_ShapeMatchTool.Instance.hWindow_Final1.viewWindow.displayROI(shapeMatchTool.L_regions);
                                    Frm_ShapeMatchTool.Instance.regions = shapeMatchTool.L_regions;
                                }

                                //打开时按当前实拍输入图像重绘已学习模板，避免模板预览窗口黑屏。
                                try
                                {
                                    if (shapeMatchTool.modelID != -1 && shapeMatchTool.totalRegion != null)
                                        shapeMatchTool.ShowTemplatePreview();
                                    else
                                        Frm_ShapeMatchTool.Instance.hwc_template.HalconWindow.ClearWindow();
                                }
                                catch { }

                                //将对象信息更新到界面
                                Frm_ShapeMatchTool.Instance.pictureBox2.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                Frm_ShapeMatchTool.Instance.ckb_showCross.Checked = shapeMatchTool.showCross;
                                Frm_ShapeMatchTool.Instance.cbx_showTemplate.Checked = shapeMatchTool.showTemplate;
                                Frm_ShapeMatchTool.Instance.ckb_showFeature.Checked = shapeMatchTool.showFeature;
                                Frm_ShapeMatchTool.Instance.ckb_showMatchBox.Checked = shapeMatchTool.showMatchBox;
                                if (shapeMatchTool.searchRegionType == RegionType.AllImage)
                                {
                                    Frm_ShapeMatchTool.Instance.cbx_searchRegionType.SelectedIndex = 0;
                                    Frm_ShapeMatchTool.Instance.cbx_searchRegionType.TextStr = Project.Instance.configuration.language == Language.English ? "AllImage" : "整幅图像";
                                }
                                else
                                {
                                    Frm_ShapeMatchTool.Instance.cbx_searchRegionType.TextStr = shapeMatchTool.searchRegionType.ToString();
                                }
                                Frm_ShapeMatchTool.Instance.nud_minScore.Value = shapeMatchTool.minScore;
                                Frm_ShapeMatchTool.Instance.numericUpDown1.Value = shapeMatchTool.minScale;
                                Frm_ShapeMatchTool.Instance.numericUpDown2.Value = shapeMatchTool.maxScale;
                                Frm_ShapeMatchTool.Instance.nud_matchNum.Value = shapeMatchTool.matchNum;
                                Frm_ShapeMatchTool.Instance.nud_angleStart.Value = shapeMatchTool.startAngle;
                                Frm_ShapeMatchTool.Instance.nud_angleRange.Value = shapeMatchTool.angleRange;
                                Frm_ShapeMatchTool.Instance.nud_angleStep.Value = shapeMatchTool.angleStep;
                                Frm_ShapeMatchTool.Instance.tkb_contrast.Value = Convert.ToInt16(shapeMatchTool.contrast);
                                Frm_ShapeMatchTool.Instance.cbx_polarity.Text = shapeMatchTool.polarity;
                                Frm_ShapeMatchTool.Instance.comboBox1.SelectedIndex = (int)shapeMatchTool.sortMode;
                                Frm_ShapeMatchTool.Instance.checkBox1.Checked = shapeMatchTool.showIndex;
                                Frm_ShapeMatchTool.Instance.numericUpDown4.Value = shapeMatchTool.spanPixelNum;
                                Frm_ShapeMatchTool.Instance.checkBox2.Checked = shapeMatchTool.showSearchRegion;

                                Frm_ShapeMatchTool.Instance.pictureBox3.Image = (shapeMatchTool.showTemplate ? Resources.复选框 : Resources.去复选框);
                                Frm_ShapeMatchTool.Instance.pictureBox4.Image = (shapeMatchTool.showCross ? Resources.复选框 : Resources.去复选框);
                                Frm_ShapeMatchTool.Instance.pictureBox5.Image = (shapeMatchTool.showFeature ? Resources.复选框 : Resources.去复选框);
                                Frm_ShapeMatchTool.Instance.pictureBox12.Image = (shapeMatchTool.showMatchBox ? Resources.复选框 : Resources.去复选框);
                                Frm_ShapeMatchTool.Instance.pictureBox8.Image = (shapeMatchTool.showIndex ? Resources.复选框 : Resources.去复选框);
                                Frm_ShapeMatchTool.Instance.pictureBox9.Image = (shapeMatchTool.showSearchRegion ? Resources.复选框 : Resources.去复选框);

                                if (shapeMatchTool.angleStep == 0)
                                {
                                    Frm_ShapeMatchTool.Instance.nud_angleStep.Enabled = false;
                                    Frm_ShapeMatchTool.Instance.ckb_autoStep.Checked = true;
                                }
                                else
                                {
                                    Frm_ShapeMatchTool.Instance.ckb_autoStep.Checked = false;
                                }

                                //Frm_ShapeMatchTool.Instance.tbc_shapeMatch.SelectedIndex = 0;


                                if (shapeMatchTool.modelID == -1)
                                {
                                    //Frm_ShapeMatchTool.Instance.panel4.Visible = false;
                                    //Frm_ShapeMatchTool.Instance.panel17.Visible = false;
                                    //Frm_ShapeMatchTool.Instance.button5.Visible = false;
                                    //Frm_ShapeMatchTool.Instance.button9.Visible = false;

                                    Frm_ShapeMatchTool.Instance.radioButton2.Enabled = false;
                                    Frm_ShapeMatchTool.Instance.radioButton3.Enabled = false;
                                }
                                else
                                {
                                    //Frm_ShapeMatchTool.Instance.panel4.Visible = true;
                                    //Frm_ShapeMatchTool.Instance.panel17.Visible = true;
                                    //Frm_ShapeMatchTool.Instance.button5.Visible = true;
                                    //Frm_ShapeMatchTool.Instance.button9.Visible = true;

                                    Frm_ShapeMatchTool.Instance.radioButton2.Enabled = true;
                                    Frm_ShapeMatchTool.Instance.radioButton3.Enabled = true;
                                }
                                Frm_ShapeMatchTool.Instance.hWindow_Final1.DispImageFit();

                                if (shapeMatchTool.modelID != -1)
                                    Frm_ShapeMatchTool.Instance.button7.Text = "重新学习";
                                else
                                    Frm_ShapeMatchTool.Instance.button7.Text = "学习";

                                if (shapeMatchTool.matchMode == MatchMode.BasedShape)
                                {
                                    Frm_ShapeMatchTool.Instance.radioButton6.Checked = true;
                                    Frm_ShapeMatchTool.Instance.pictureBox10.Image = Resources.勾选;
                                    Frm_ShapeMatchTool.Instance.radioButton5.Checked = false;
                                    Frm_ShapeMatchTool.Instance.pictureBox11.Image = Resources.去勾选;
                                }
                                else
                                {
                                    Frm_ShapeMatchTool.Instance.radioButton5.Checked = true;
                                    Frm_ShapeMatchTool.Instance.pictureBox11.Image = Resources.勾选;
                                    Frm_ShapeMatchTool.Instance.radioButton6.Checked = false;
                                    Frm_ShapeMatchTool.Instance.pictureBox10.Image = Resources.去勾选;
                                }


                                break;
                            #endregion

                            #region BlobAnalyse
                            case ToolType.BlobAnalyse:
                                Frm_BlobAnalyseTool.Instance.pictureBox1.Image = Resources.BlobAnalyseTool;
                                Frm_BlobAnalyseTool.Instance.lbl_title.Text = string.Format("斑点分析    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_BlobAnalyseTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_BlobAnalyseTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_BlobAnalyseTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_BlobAnalyseTool.Instance.TopMost = true;
                                Frm_BlobAnalyseTool.Instance.Activate();
                                Frm_BlobAnalyseTool.Instance.jobName = this.jobName;
                                Frm_BlobAnalyseTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_BlobAnalyseTool.Instance.Show();
                                Frm_BlobAnalyseTool.Instance.WindowState = FormWindowState.Normal;
                                //////Frm_BlobAnalyseTool.Instance.btn_runTool.Focus();
                                BlobAnalyseTool blobAnalyseTool = (BlobAnalyseTool)(L_toolList[i].tool);
                                Frm_BlobAnalyseTool.blobAnalyseTool = blobAnalyseTool;
                                Frm_ProcessingItem.blobAnalyseTool = blobAnalyseTool;
                                Frm_ProcessingItem1.blobAnalyseTool = blobAnalyseTool;
                                Application.DoEvents();

                                // 打开工具窗口时也解析图像和跟随输入；否则界面始终显示原始 ROI，
                                // 即使正式流程中已连接模板匹配位置也看不到当前工件位置下的查找框。
                                inputItemNum = L_toolList[i].input.Count;
                                for (int j = 0; j < inputItemNum; j++)
                                {
                                    string inputItemName = L_toolList[i].input[j].IOName;
                                    string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                                    if (sourceFrom == string.Empty)
                                        continue;

                                    string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                    sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                    string toolItem = Regex.Split(sourceFrom, "->")[1];
                                    object sourceValue = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value;

                                    if (inputItemName == "图像" || inputItemName == "InputImage")
                                        blobAnalyseTool.toolPar.InputPar.图像 = sourceValue as HObject;
                                    else if (inputItemName == "跟随" || inputItemName == "Pose")
                                        blobAnalyseTool.toolPar.InputPar.跟随 = sourceValue as List<XYU>;
                                }
                                blobAnalyseTool.EnsureTemplatePoseFromCurrentInput();

                                if (blobAnalyseTool.toolPar.InputPar.图像 != null)
                                {
                                    Frm_BlobAnalyseTool.Instance.hWindow_Final1.HobjectToHimage(blobAnalyseTool.toolPar.InputPar.图像);
                                    HObject runtimeSearchRegion = blobAnalyseTool.GetRuntimeSearchRegion();
                                    if (blobAnalyseTool.searchRegionType != RegionType.AllImage &&
                                        blobAnalyseTool.searchRegionType != RegionType.整幅图像 &&
                                        runtimeSearchRegion != null)
                                        Frm_BlobAnalyseTool.Instance.hWindow_Final1.DispObj(runtimeSearchRegion, "blue");
                                }
                                else
                                    Frm_BlobAnalyseTool.Instance.hWindow_Final1.ClearWindow();

                                // 未启用跟随时保留可编辑的原始 ROI；已启用时上方已绘制实际运行区域。
                                if (blobAnalyseTool.toolPar.InputPar.跟随 == null || blobAnalyseTool.toolPar.InputPar.跟随.Count == 0)
                                    Frm_BlobAnalyseTool.Instance.hWindow_Final1.viewWindow.displayROI(blobAnalyseTool.L_regions);

                                Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows.Clear();
                                for (int j = 0; j < blobAnalyseTool.L_select.Count; j++)
                                {
                                    int index = Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows.Add();
                                    Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[index].Cells[0].Value = blobAnalyseTool.L_select[j].SelectType;
                                    Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[index].Cells[1].Value = blobAnalyseTool.L_select[j].AreaDownLimit;
                                    Frm_BlobAnalyseTool.Instance.dgv_selectItem.Rows[index].Cells[2].Value = blobAnalyseTool.L_select[j].AreaUpLimit;
                                }

                                //将预处理项更新到窗体
                                Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Clear();
                                for (int j = 0; j < blobAnalyseTool.L_prePorcessing.Count; j++)
                                {
                                    int index = Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Add();
                                    Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[0].Value = blobAnalyseTool.L_prePorcessing[j].PreProcessingType;
                                    ((DataGridViewCheckBoxCell)Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[1]).Value = blobAnalyseTool.L_prePorcessing[j].Enable;
                                }

                                //////Frm_BlobAnalyseTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;
                                Frm_BlobAnalyseTool.Instance.ckb_displaySearchRegion.Checked = blobAnalyseTool.displaySearchRegion;
                                Frm_BlobAnalyseTool.Instance.ckb_displayCross.Checked = blobAnalyseTool.displayCross;
                                Frm_BlobAnalyseTool.Instance.tbx_lineWidth.Text = blobAnalyseTool.lineWidth.ToString();
                                Frm_BlobAnalyseTool.Instance.rdo_outCircleFillMode.Checked = blobAnalyseTool.outCircleDrawMode == FillMode.Fill ? true : false;
                                Frm_BlobAnalyseTool.Instance.rdo_outCircleMarginMode.Checked = blobAnalyseTool.outCircleDrawMode == FillMode.Fill ? false : true;
                                Frm_BlobAnalyseTool.Instance.rdo_regionFillMode.Checked = blobAnalyseTool.regionDrawMode == FillMode.Fill ? true : false;
                                Frm_BlobAnalyseTool.Instance.rdo_regionMarginMode.Checked = blobAnalyseTool.regionDrawMode == FillMode.Fill ? false : true;
                                Frm_BlobAnalyseTool.Instance.ckb_displayRegion.Checked = blobAnalyseTool.displayRegion;
                                Frm_BlobAnalyseTool.Instance.rdo_outCircleFillMode.Checked = blobAnalyseTool.outCircleDrawMode == FillMode.Fill ? true : false;
                                Frm_BlobAnalyseTool.Instance.ckb_DisplayOutCircle.Checked = blobAnalyseTool.displayOutCircle;
                                Frm_BlobAnalyseTool.Instance.comboBox1.SelectedIndex = (int)blobAnalyseTool.sortMode;
                                Frm_BlobAnalyseTool.Instance.textBox1.Value = blobAnalyseTool.spanPixelNum;
                                Frm_BlobAnalyseTool.Instance.cbx_searchRegionType.TextStr = blobAnalyseTool.searchRegionType.ToString();
                                Frm_BlobAnalyseTool.Instance.trackBar1.Value = (blobAnalyseTool.minThreshold);
                                Frm_BlobAnalyseTool.Instance.trackBar2.Value = (blobAnalyseTool.maxThreshold);
                                Frm_BlobAnalyseTool.Instance.numericUpDown1.Value = blobAnalyseTool.minThreshold;
                                Frm_BlobAnalyseTool.Instance.numericUpDown2.Value = blobAnalyseTool.maxThreshold;
                                Frm_BlobAnalyseTool.Instance.rdo_regionFillMode.Checked = blobAnalyseTool.regionDrawMode == FillMode.Fill ? true : false;
                                break;
                            #endregion

                            #region EyeHandCalib
                            case ToolType.EyeHandCalib:
                                Frm_EyeHandCalibTool.Instance.lbl_title.Text = string.Format("手眼标定    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_EyeHandCalibTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_EyeHandCalibTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_EyeHandCalibTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_EyeHandCalibTool.Instance.TopMost = true;
                                Frm_EyeHandCalibTool.Instance.Activate();
                                Frm_EyeHandCalibTool.Instance.jobName = this.jobName;
                                Frm_EyeHandCalibTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_EyeHandCalibTool.Instance.Show();
                                Frm_EyeHandCalibTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_EyeHandCalibTool.Instance.Focus();
                                EyeHandCalibTool eyeHandCalibTool = (EyeHandCalibTool)(L_toolList[i].tool);
                                Frm_EyeHandCalibTool.eyeHandCalibTool = eyeHandCalibTool;
                                Application.DoEvents();

                                if (eyeHandCalibTool.toolPar.InputPar.图像 != null)
                                    eyeHandCalibTool.ShowImage(eyeHandCalibTool.toolPar.InputPar.图像);
                                else
                                    eyeHandCalibTool.ClearWindow();

                                //将对象信息更新到界面
                                Frm_EyeHandCalibTool.Instance.pictureBox8.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                Frm_EyeHandCalibTool.Instance.cbx_fixedType.SelectedIndex = (eyeHandCalibTool.fixedType == FixedType.OnHand ? 1 : 0);

                                Frm_EyeHandCalibTool.Instance.checkBox1.Checked = eyeHandCalibTool.multPhoto;
                                Frm_EyeHandCalibTool.Instance.pictureBox6.Image = eyeHandCalibTool.multPhoto ? Resources.复选框 : Resources.去复选框;
                                Frm_EyeHandCalibTool.Instance.checkBox2.Checked = eyeHandCalibTool.calibRotateCenter;
                                Frm_EyeHandCalibTool.Instance.pictureBox2.Image = eyeHandCalibTool.calibRotateCenter ? Resources.复选框 : Resources.去复选框;

                                if (eyeHandCalibTool.multPhoto && !Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Contains(Frm_EyeHandCalibTool.Instance.tabPage7))
                                    Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Add(Frm_EyeHandCalibTool.Instance.tabPage7);
                                else if (!eyeHandCalibTool.multPhoto && Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Contains(Frm_EyeHandCalibTool.Instance.tabPage7))
                                    Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Remove(Frm_EyeHandCalibTool.Instance.tabPage7);

                                if (eyeHandCalibTool.calibRotateCenter && !Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Contains(Frm_EyeHandCalibTool.Instance.tabPage5))
                                {
                                    Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Add(Frm_EyeHandCalibTool.Instance.tabPage5);

                                    Frm_EyeHandCalibTool.Instance.tabPage6.Text = "第四步";
                                    Frm_EyeHandCalibTool.Instance.tabPage8.Text = "第五步";
                                    Frm_EyeHandCalibTool.Instance.tabPage7.Text = "第六步";
                                }
                                else if (!eyeHandCalibTool.calibRotateCenter && Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Contains(Frm_EyeHandCalibTool.Instance.tabPage5))
                                {
                                    Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Remove(Frm_EyeHandCalibTool.Instance.tabPage5);
                                    Frm_EyeHandCalibTool.Instance.tabPage6.Text = "第三步";
                                    Frm_EyeHandCalibTool.Instance.tabPage8.Text = "第四步";
                                    Frm_EyeHandCalibTool.Instance.tabPage7.Text = "第五步";

                                }
                                Application.DoEvents();


                                //显示标定数据

                                Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows.Clear();
                                if (eyeHandCalibTool.L_calibData.Count != 0)
                                    Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows.Add(eyeHandCalibTool.L_calibData.Count);
                                for (int j = 0; j < ((EyeHandCalibTool)L_toolList[i].tool).L_calibData.Count; j++)
                                {
                                    for (int k = 0; k < 5; k++)
                                    {
                                        Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[j].Cells[k].Value = eyeHandCalibTool.L_calibData[j][k];
                                    }
                                }

                                Frm_EyeHandCalibTool.Instance.dataGridView2.Rows.Clear();
                                if (eyeHandCalibTool.L_calibRotateCenterData.Count != 0)
                                    Frm_EyeHandCalibTool.Instance.dataGridView2.Rows.Add(eyeHandCalibTool.L_calibRotateCenterData.Count);
                                for (int j = 0; j < ((EyeHandCalibTool)L_toolList[i].tool).L_calibRotateCenterData.Count; j++)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        Frm_EyeHandCalibTool.Instance.dataGridView2.Rows[j].Cells[k].Value = eyeHandCalibTool.L_calibRotateCenterData[j][k];
                                    }
                                }

                                Frm_EyeHandCalibTool.Instance.dataGridView3.Rows.Clear();
                                if (eyeHandCalibTool.L_calibCheckData.Count != 0)
                                    Frm_EyeHandCalibTool.Instance.dataGridView3.Rows.Add(eyeHandCalibTool.L_calibCheckData.Count);
                                for (int j = 0; j < ((EyeHandCalibTool)L_toolList[i].tool).L_calibCheckData.Count; j++)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        Frm_EyeHandCalibTool.Instance.dataGridView3.Rows[j].Cells[k].Value = eyeHandCalibTool.L_calibCheckData[j][k];
                                    }
                                }

                                Frm_EyeHandCalibTool.Instance.textBox2.Text = eyeHandCalibTool.checkPoint.X.ToString();
                                Frm_EyeHandCalibTool.Instance.textBox1.Text = eyeHandCalibTool.checkPoint.Y.ToString();

                                Frm_EyeHandCalibTool.Instance.textBox4.Text = eyeHandCalibTool.calibOffset.X.ToString();
                                Frm_EyeHandCalibTool.Instance.textBox3.Text = eyeHandCalibTool.calibOffset.Y.ToString();

                                Frm_EyeHandCalibTool.Instance.cbx_jobList.Clear();
                                for (int j = 0; j < Project.Instance.curEngine.L_jobList.Count; j++)
                                {
                                    Frm_EyeHandCalibTool.Instance.cbx_jobList.Add(Project.Instance.curEngine.L_jobList[j].jobName);
                                }
                                Frm_EyeHandCalibTool.Instance.cbx_jobList.TextStr = eyeHandCalibTool.calibJobName;
                                Frm_EyeHandCalibTool.Instance.cbx_outputItemList.TextStr = eyeHandCalibTool.calibItemName;
                                Frm_EyeHandCalibTool.Instance.comboBox2.TextStr = eyeHandCalibTool.dirctionOfU;
                                Frm_EyeHandCalibTool.Instance.comboBox1.Items.Clear();


                                Frm_EyeHandCalibTool.Instance.dataGridView1.Rows.Clear();
                                foreach (KeyValuePair<int, XY> item in eyeHandCalibTool.D_photoPos)
                                {
                                    Frm_EyeHandCalibTool.Instance.comboBox1.Items.Add(item.Key.ToString());
                                    int index = Frm_EyeHandCalibTool.Instance.dataGridView1.Rows.Add();
                                    Frm_EyeHandCalibTool.Instance.dataGridView1.Rows[index].Cells[0].Value = item.Key;
                                    Frm_EyeHandCalibTool.Instance.dataGridView1.Rows[index].Cells[1].Value = item.Value.X;
                                    Frm_EyeHandCalibTool.Instance.dataGridView1.Rows[index].Cells[2].Value = item.Value.Y;
                                }

                                Frm_EyeHandCalibTool.Instance.textBox6.Value = eyeHandCalibTool.rotateCenter.X.ToString();
                                Frm_EyeHandCalibTool.Instance.textBox5.Value = eyeHandCalibTool.rotateCenter.Y.ToString();

                                Frm_EyeHandCalibTool.Instance.comboBox1.SelectedIndex = eyeHandCalibTool.curPhotoPosIndex;


                                Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[0].Selected = true;
                                Frm_EyeHandCalibTool.Instance.dataGridView2.Rows[0].Selected = true;
                                Frm_EyeHandCalibTool.Instance.dataGridView3.Rows[0].Selected = true;

                                if (eyeHandCalibTool.calibResult == "未标定")
                                {
                                    Frm_EyeHandCalibTool.Instance.label28.ForeColor = Color.Black;
                                }
                                Frm_EyeHandCalibTool.Instance.label28.Text = eyeHandCalibTool.calibResult;
                                Frm_EyeHandCalibTool.Instance.tbx_translateX.Text = eyeHandCalibTool.TranslateX.ToString();
                                Frm_EyeHandCalibTool.Instance.tbx_translateY.Text = eyeHandCalibTool.TranslateY.ToString();
                                Frm_EyeHandCalibTool.Instance.tbx_scaleX.Text = eyeHandCalibTool.ScanX.ToString();
                                Frm_EyeHandCalibTool.Instance.tbx_scaleY.Text = eyeHandCalibTool.ScanY.ToString();
                                Frm_EyeHandCalibTool.Instance.tbx_rotation.Text = eyeHandCalibTool.Rotation.ToString();
                                Frm_EyeHandCalibTool.Instance.tbx_theta.Text = eyeHandCalibTool.Theta.ToString();
                                break;
                            #endregion

                            #region ApplyTrans
                            case ToolType.QuoteTrans:
                                Frm_ApplyTransTool.Instance.lbl_title.Text = string.Format("引用标定    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_EyeHandCalibTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_EyeHandCalibTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_EyeHandCalibTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_EyeHandCalibTool.Instance.TopMost = true;
                                Frm_ApplyTransTool.Instance.Activate();
                                Frm_ApplyTransTool.Instance.jobName = this.jobName;
                                Frm_ApplyTransTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_ApplyTransTool.Instance.Show();
                                Frm_ApplyTransTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_ApplyTransTool.Instance.Focus();
                                QuoteTransTool applyTransTool = (QuoteTransTool)(L_toolList[i].tool);
                                Frm_ApplyTransTool.applyTransTool = applyTransTool;
                                Application.DoEvents();


                                //将对象信息更新到界面
                                Frm_ApplyTransTool.Instance.pictureBox8.Image = L_toolList[i].enable ? Resources.Enable : Resources.Disable;


                                Frm_ApplyTransTool.Instance.cbx_jobList.Items.Clear();
                                for (int j = 0; j < Project.Instance.curEngine.L_jobList.Count; j++)
                                {
                                    for (int k = 0; k < Project.Instance.curEngine.L_jobList[j].L_toolList.Count; k++)
                                    {
                                        if (Project.Instance.curEngine.L_jobList[j].L_toolList[k].toolType == ToolType.EyeHandCalib)
                                        {
                                            Frm_ApplyTransTool.Instance.cbx_jobList.Items.Add(Project.Instance.curEngine.L_jobList[j].jobName + " . " + Project.Instance.curEngine.L_jobList[j].L_toolList[k].toolName);
                                        }
                                    }
                                }
                                Frm_ApplyTransTool.Instance.cbx_jobList.Text = applyTransTool.cliperNum;
                                Frm_ApplyTransTool.Instance.textBox6.Text = applyTransTool.photoPos.ToString();
                                break;
                            #endregion

                            #region OneKeyEyeHandCalib
                            case ToolType.OneKeyEyeHandCalib:
                                Frm_OneKeyEyeHandCalibTool.Instance.lbl_title.Text = string.Format("一键手眼标定    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_OneKeyEyeHandCalibTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_OneKeyEyeHandCalibTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_OneKeyEyeHandCalibTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_OneKeyEyeHandCalibTool.Instance.TopMost = true;
                                Frm_OneKeyEyeHandCalibTool.Instance.Activate();
                                Frm_OneKeyEyeHandCalibTool.Instance.jobName = this.jobName;
                                Frm_OneKeyEyeHandCalibTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_OneKeyEyeHandCalibTool.Instance.Show();
                                Frm_OneKeyEyeHandCalibTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_OneKeyEyeHandCalibTool.Instance.Focus();
                                OneKeyEyeHandCalibTool oneKeyEyeHandCalibTool = (OneKeyEyeHandCalibTool)(L_toolList[i].tool);
                                Frm_OneKeyEyeHandCalibTool.oneKeyEyeHandCalibTool = oneKeyEyeHandCalibTool;
                                Application.DoEvents();

                                if (oneKeyEyeHandCalibTool.inputImage != null)
                                    oneKeyEyeHandCalibTool.ShowImage(oneKeyEyeHandCalibTool.inputImage);
                                else
                                    oneKeyEyeHandCalibTool.ClearWindow();

                                //将对象信息更新到界面
                                Frm_OneKeyEyeHandCalibTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;
                                Frm_OneKeyEyeHandCalibTool.Instance.cbo_calibType.Text = (oneKeyEyeHandCalibTool.calibType == CalibType.Four_Point ? "四点标定" : "九点标定");
                                Frm_OneKeyEyeHandCalibTool.Instance.tbx_translateX.Text = oneKeyEyeHandCalibTool.TranslateX.ToString();
                                Frm_OneKeyEyeHandCalibTool.Instance.tbx_translateY.Text = oneKeyEyeHandCalibTool.TranslateY.ToString();
                                Frm_OneKeyEyeHandCalibTool.Instance.tbx_scaleX.Text = oneKeyEyeHandCalibTool.ScanX.ToString();
                                Frm_OneKeyEyeHandCalibTool.Instance.tbx_scaleY.Text = oneKeyEyeHandCalibTool.ScanY.ToString();
                                Frm_OneKeyEyeHandCalibTool.Instance.tbx_rotation.Text = oneKeyEyeHandCalibTool.Rotation.ToString();
                                Frm_OneKeyEyeHandCalibTool.Instance.tbx_theta.Text = oneKeyEyeHandCalibTool.Theta.ToString();
                                Application.DoEvents();

                                //显示标定数据
                                for (int j = 0; j < oneKeyEyeHandCalibTool.L_calibData.Count; j++)
                                {
                                    for (int k = 0; k < 4; k++)
                                    {
                                        Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[j].Cells[k].Value = oneKeyEyeHandCalibTool.L_calibData[j][k];
                                    }
                                }

                                Frm_OneKeyEyeHandCalibTool.Instance.cbx_jobList.Items.Clear();
                                for (int j = 0; j < Project.Instance.curEngine.L_jobList.Count; j++)
                                {
                                    Frm_OneKeyEyeHandCalibTool.Instance.cbx_jobList.Items.Add(Project.Instance.curEngine.L_jobList[j].jobName);
                                }
                                Frm_OneKeyEyeHandCalibTool.Instance.cbx_jobList.Text = oneKeyEyeHandCalibTool.calibJobName;
                                Frm_OneKeyEyeHandCalibTool.Instance.cbx_outputItemList.Text = oneKeyEyeHandCalibTool.calibItemName;
                                break;
                            #endregion

                            #region OneDimensionalCalib
                            case ToolType.OneDimensionalCalib:
                                Frm_OneDimensionalCalibTool.Instance.lbl_title.Text = string.Format("一维标定    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_OneDimensionalCalibTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_OneDimensionalCalibTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_OneDimensionalCalibTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_OneDimensionalCalibTool.Instance.TopMost = true;
                                Frm_OneDimensionalCalibTool.Instance.Activate();
                                Frm_OneDimensionalCalibTool.Instance.jobName = this.jobName;
                                Frm_OneDimensionalCalibTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_OneDimensionalCalibTool.Instance.Show();
                                Frm_OneDimensionalCalibTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_OneDimensionalCalibTool.Instance.Focus();
                                OneDimensionalCalibTool oneDimensionalCalibrationTool = (OneDimensionalCalibTool)(L_toolList[i].tool);
                                Frm_OneDimensionalCalibTool.oneDimensionalCalibTool = oneDimensionalCalibrationTool;
                                Application.DoEvents();

                                //将对象信息更新到界面
                                Frm_OneDimensionalCalibTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;
                                Frm_OneDimensionalCalibTool.Instance.tbx_translate.Text = oneDimensionalCalibrationTool.Translate.ToString();
                                Frm_OneDimensionalCalibTool.Instance.tbx_scale.Text = oneDimensionalCalibrationTool.Scan.ToString();
                                Application.DoEvents();

                                //显示标定数据
                                for (int j = 0; j < ((OneDimensionalCalibTool)L_toolList[i].tool).L_calibData.Count; j++)
                                {
                                    for (int k = 0; k < 2; k++)
                                    {
                                        Frm_OneDimensionalCalibTool.Instance.dgv_calibrateData.Rows[j].Cells[k].Value = oneDimensionalCalibrationTool.L_calibData[j][k];
                                    }
                                }

                                break;
                            #endregion

                            #region UpCamAlign
                            case ToolType.UpCamAlign:
                                Frm_UpCamAlignTool.Instance.lbl_title.Text = string.Format("上相机定位    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_UpCamAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_UpCamAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_UpCamAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_UpCamAlignTool.Instance.TopMost = true;
                                Frm_UpCamAlignTool.Instance.Activate();
                                Frm_UpCamAlignTool.Instance.jobName = this.jobName;
                                Frm_UpCamAlignTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_UpCamAlignTool.Instance.Show();
                                Frm_UpCamAlignTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_UpCamAlignTool.Instance.btn_runTool.Focus();
                                UpCamAlignTool upCamAlignTool = (UpCamAlignTool)(L_toolList[i].tool);
                                Frm_UpCamAlignTool.upCamAlignTool = upCamAlignTool;
                                Application.DoEvents();

                                //Frm_UpCamAlignTool.Instance.cbx_toolEnable.Checked = L_toolList[i].enable;

                                Frm_UpCamAlignTool.Instance.cbx_toolList.Items.Clear();
                                Frm_UpCamAlignTool.Instance.cbx_toolList.Items.AddRange(upCamAlignTool.L_toolName.ToArray());
                                Frm_UpCamAlignTool.Instance.cbx_toolList.SelectedIndex = upCamAlignTool.toolIdx;

                                Frm_UpCamAlignTool.Instance.tbx_inputPosX.Text = upCamAlignTool.toolPar.InputPar.位置.Point.X.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_inputPosY.Text = upCamAlignTool.toolPar.InputPar.位置.Point.Y.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_inputPosU.Text = upCamAlignTool.toolPar.InputPar.位置.U.ToString();

                                Frm_UpCamAlignTool.Instance.tbx_pickPosX.Value = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.X.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_pickPosY.Value = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.Y.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_pickPosU.Value = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].U.ToString();

                                Frm_UpCamAlignTool.Instance.tbx_featureX.Value = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.X.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_featureY.Value = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.Y.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_featureU.Value = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].U.ToString();

                                Frm_UpCamAlignTool.Instance.tbx_resultPosX.Text = upCamAlignTool.toolPar.ResultPar.位置.Point.X.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_resultPosY.Text = upCamAlignTool.toolPar.ResultPar.位置.Point.Y.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_resultPosU.Text = upCamAlignTool.toolPar.ResultPar.位置.U.ToString();

                                Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetX.Value = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.X;
                                Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetY.Value = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.Y;
                                Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetU.Value = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].U;

                                Frm_UpCamAlignTool.Instance.tbx_saftyRangeX.Value = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.X.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_saftyRangeY.Value = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.Y.ToString();
                                Frm_UpCamAlignTool.Instance.tbx_saftyRangeU.Value = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].U.ToString();

                                break;
                            #endregion

                            #region DownCamAlign
                            case ToolType.DownCamAlign:
                                Frm_DownCamAlignTool.Instance.lbl_title.Text = string.Format("下相机定位    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_DownCamAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_DownCamAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_DownCamAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_DownCamAlignTool.Instance.TopMost = true;
                                Frm_DownCamAlignTool.Instance.Activate();
                                Frm_DownCamAlignTool.Instance.jobName = this.jobName;
                                Frm_DownCamAlignTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_DownCamAlignTool.Instance.Show();
                                Frm_DownCamAlignTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_DownCamAlignTool.Instance.btn_runTool.Focus();
                                DownCamAlignTool robotDownCamAlignTool = (DownCamAlignTool)(L_toolList[i].tool);
                                Frm_DownCamAlignTool.robotDownCamAlignTool = robotDownCamAlignTool;
                                Application.DoEvents();

                                Frm_DownCamAlignTool.Instance.cbx_toolEnable.Checked = L_toolList[i].enable;

                                Frm_DownCamAlignTool.Instance.tbx_inputPosX.Text = robotDownCamAlignTool.inputPos.Point.X.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_inputPosY.Text = robotDownCamAlignTool.inputPos.Point.Y.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_inputPosU.Text = robotDownCamAlignTool.inputPos.U.ToString();

                                Frm_DownCamAlignTool.Instance.tbx_photoPosX.Text = robotDownCamAlignTool.photoPos.Point.X.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_photoPosY.Text = robotDownCamAlignTool.photoPos.Point.Y.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_photoPosU.Text = robotDownCamAlignTool.photoPos.U.ToString();

                                Frm_DownCamAlignTool.Instance.tbx_featurePosX.Text = robotDownCamAlignTool.featurePos.Point.X.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_featurePosY.Text = robotDownCamAlignTool.featurePos.Point.Y.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_featurePosU.Text = robotDownCamAlignTool.featurePos.U.ToString();

                                Frm_DownCamAlignTool.Instance.tbx_resultPosX.Text = robotDownCamAlignTool.resultPos.Point.X.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_resultPosY.Text = robotDownCamAlignTool.resultPos.Point.Y.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_resultPosU.Text = robotDownCamAlignTool.resultPos.U.ToString();

                                Frm_DownCamAlignTool.Instance.tbx_placePosX.Text = robotDownCamAlignTool.placePos.Point.X.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_placePosY.Text = robotDownCamAlignTool.placePos.Point.Y.ToString();
                                Frm_DownCamAlignTool.Instance.tbx_placePosU.Text = robotDownCamAlignTool.placePos.U.ToString();

                                break;
                            #endregion

                            #region RotatePlatform
                            case ToolType.RotatePlatform:
                                Frm_RotatePlatformTool.Instance.lbl_title.Text = string.Format("旋转平台    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_RotatePlatformTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_RotatePlatformTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_RotatePlatformTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_RotatePlatformTool.Instance.TopMost = true;
                                Frm_RotatePlatformTool.Instance.Activate();
                                Frm_RotatePlatformTool.Instance.jobName = this.jobName;
                                Frm_RotatePlatformTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_RotatePlatformTool.Instance.Show();
                                Frm_RotatePlatformTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_RotatePlatformTool.Instance.btn_runTool.Focus();
                                RotatePlatformTool rotatePlatformTool = (RotatePlatformTool)(L_toolList[i].tool);
                                Frm_RotatePlatformTool.rotatePlatformTool = rotatePlatformTool;
                                Application.DoEvents();

                                Frm_RotatePlatformTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;

                                Frm_RotatePlatformTool.Instance.tbx_rotateCenterX.Text = rotatePlatformTool.rotateCenter.Point.X.ToString();
                                Frm_RotatePlatformTool.Instance.tbx_rotateCenterY.Text = rotatePlatformTool.rotateCenter.Point.Y.ToString();

                                Frm_RotatePlatformTool.Instance.tbx_inputPointX.Text = rotatePlatformTool.inputPos.Point.X.ToString();
                                Frm_RotatePlatformTool.Instance.tbx_inputPointY.Text = rotatePlatformTool.inputPos.Point.Y.ToString();
                                Frm_RotatePlatformTool.Instance.tbx_inputPointU.Text = rotatePlatformTool.inputPos.U.ToString();

                                Frm_RotatePlatformTool.Instance.tbx_outputPointX.Text = rotatePlatformTool.outputPos.Point.X.ToString();
                                Frm_RotatePlatformTool.Instance.tbx_outputPointY.Text = rotatePlatformTool.outputPos.Point.Y.ToString();
                                Frm_RotatePlatformTool.Instance.tbx_outputPointU.Text = rotatePlatformTool.outputPos.U.ToString();

                                //显示数据
                                for (int j = 0; j < rotatePlatformTool.L_calibData.Count; j++)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        Frm_RotatePlatformTool.Instance.dgv_data.Rows[j].Cells[k].Value = rotatePlatformTool.L_calibData[j][k];
                                    }
                                }

                                //流程列表
                                Frm_RotatePlatformTool.Instance.cbx_jobList.Items.Clear();
                                for (int j = 0; j < Project.Instance.curEngine.L_jobList.Count; j++)
                                {
                                    Frm_RotatePlatformTool.Instance.cbx_jobList.Items.Add(Project.Instance.curEngine.L_jobList[j].jobName);
                                }
                                Frm_RotatePlatformTool.Instance.cbx_jobList.Text = rotatePlatformTool.calibJobName;
                                Frm_RotatePlatformTool.Instance.cbx_outputItemList.Text = rotatePlatformTool.calibItemName;

                                break;
                            #endregion

                            #region XYPlatform
                            case ToolType.XYPlatform:
                                Frm_XYPlatformTool.Instance.lbl_title.Text = string.Format("XY平台    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_XYPlatformTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_XYPlatformTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_XYPlatformTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_XYPlatformTool.Instance.TopMost = true;
                                Frm_XYPlatformTool.Instance.Activate();
                                Frm_XYPlatformTool.Instance.jobName = this.jobName;
                                Frm_XYPlatformTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_XYPlatformTool.Instance.Show();
                                Frm_XYPlatformTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_XYPlatformTool.Instance.btn_runTool.Focus();
                                XYPlatformTool xyPlatformTool = (XYPlatformTool)(L_toolList[i].tool);
                                Frm_XYPlatformTool.xyPlatformTool = xyPlatformTool;
                                Application.DoEvents();

                                Frm_XYPlatformTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;

                                Frm_XYPlatformTool.Instance.tbx_pickPosX.Text = xyPlatformTool.pickPos.Point.X.ToString();
                                Frm_XYPlatformTool.Instance.tbx_pickPosY.Text = xyPlatformTool.pickPos.Point.Y.ToString();
                                Frm_XYPlatformTool.Instance.tbx_pickPosU.Text = xyPlatformTool.pickPos.U.ToString();

                                Frm_XYPlatformTool.Instance.tbx_pickPosOffsetX.Text = xyPlatformTool.pickPosOffset.Point.X.ToString();
                                Frm_XYPlatformTool.Instance.tbx_pickPosOffsetY.Text = xyPlatformTool.pickPosOffset.Point.Y.ToString();
                                Frm_XYPlatformTool.Instance.tbx_pickPosOffsetU.Text = xyPlatformTool.pickPosOffset.U.ToString();

                                Frm_XYPlatformTool.Instance.tbx_featureX.Text = xyPlatformTool.featurePos.Point.X.ToString();
                                Frm_XYPlatformTool.Instance.tbx_featureY.Text = xyPlatformTool.featurePos.Point.Y.ToString();
                                Frm_XYPlatformTool.Instance.tbx_featureU.Text = xyPlatformTool.featurePos.U.ToString();

                                Frm_XYPlatformTool.Instance.tbx_inputPointX.Text = xyPlatformTool.inputPos.Point.X.ToString();
                                Frm_XYPlatformTool.Instance.tbx_inputPointY.Text = xyPlatformTool.inputPos.Point.Y.ToString();
                                Frm_XYPlatformTool.Instance.tbx_inputPointU.Text = xyPlatformTool.inputPos.U.ToString();

                                Frm_XYPlatformTool.Instance.tbx_outputPointX.Text = xyPlatformTool.outputPos.Point.X.ToString();
                                Frm_XYPlatformTool.Instance.tbx_outputPointY.Text = xyPlatformTool.outputPos.Point.Y.ToString();
                                Frm_XYPlatformTool.Instance.tbx_outputPointU.Text = xyPlatformTool.outputPos.U.ToString();

                                break;
                            #endregion

                            #region 点位引导
                            case ToolType.PointAlign:
                                Frm_PointAlignTool.Instance.lbl_title.Text = string.Format("点位引导    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_PointAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_PointAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_PointAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_PointAlignTool.Instance.TopMost = true;
                                Frm_PointAlignTool.Instance.Activate();
                                Frm_PointAlignTool.Instance.jobName = this.jobName;
                                Frm_PointAlignTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_PointAlignTool.Instance.Show();
                                Frm_PointAlignTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_PointAlignTool.Instance.btn_runTool.Focus();
                                PointAlignTool pointAlignTool = (PointAlignTool)(L_toolList[i].tool);
                                Frm_PointAlignTool.pointAlignTool = pointAlignTool;
                                Application.DoEvents();

                                Frm_PointAlignTool.Instance.cbx_toolEnable.Checked = L_toolList[i].enable;

                                Frm_PointAlignTool.Instance.cbx_toolList.Items.Clear();
                                Frm_PointAlignTool.Instance.cbx_toolList.Items.AddRange(pointAlignTool.L_toolName.ToArray());
                                Frm_PointAlignTool.Instance.cbx_toolList.SelectedIndex = pointAlignTool.toolIdx;

                                Frm_PointAlignTool.Instance.tbx_inputPosX.Text = pointAlignTool.inputPos.Point.X.ToString();
                                Frm_PointAlignTool.Instance.tbx_inputPosY.Text = pointAlignTool.inputPos.Point.Y.ToString();
                                Frm_PointAlignTool.Instance.tbx_inputPosU.Text = pointAlignTool.inputPos.U.ToString();

                                Frm_PointAlignTool.Instance.tbx_pickPosX.Text = pointAlignTool.L_workPos[pointAlignTool.toolIdx].Point.X.ToString();
                                Frm_PointAlignTool.Instance.tbx_pickPosY.Text = pointAlignTool.L_workPos[pointAlignTool.toolIdx].Point.Y.ToString();

                                Frm_PointAlignTool.Instance.tbx_featureX.Text = pointAlignTool.L_featurePos[pointAlignTool.toolIdx].Point.X.ToString();
                                Frm_PointAlignTool.Instance.tbx_featureY.Text = pointAlignTool.L_featurePos[pointAlignTool.toolIdx].Point.Y.ToString();
                                Frm_PointAlignTool.Instance.tbx_featureU.Text = pointAlignTool.L_featurePos[pointAlignTool.toolIdx].U.ToString();

                                Frm_PointAlignTool.Instance.tbx_resultPosX.Text = pointAlignTool.resultPos.Point.X.ToString();
                                Frm_PointAlignTool.Instance.tbx_resultPosY.Text = pointAlignTool.resultPos.Point.Y.ToString();

                                Frm_PointAlignTool.Instance.tbx_pickPosOffsetX.Text = pointAlignTool.L_workPosOffset[pointAlignTool.toolIdx].Point.X.ToString();
                                Frm_PointAlignTool.Instance.tbx_pickPosOffsetY.Text = pointAlignTool.L_workPosOffset[pointAlignTool.toolIdx].Point.Y.ToString();

                                Frm_PointAlignTool.Instance.tbx_saftyRangeX.Text = pointAlignTool.L_safetyRange[pointAlignTool.toolIdx].Point.X.ToString();
                                Frm_PointAlignTool.Instance.tbx_saftyRangeY.Text = pointAlignTool.L_safetyRange[pointAlignTool.toolIdx].Point.Y.ToString();

                                break;
                            #endregion

                            #region AlignFit
                            case ToolType.AlignFit:
                                Frm_AlignFitTool.Instance.lbl_title.Text = string.Format("对位组装    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_AlignFitTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_AlignFitTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_UpCamAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_AlignFitTool.Instance.TopMost = true;
                                Frm_AlignFitTool.Instance.Activate();
                                Frm_AlignFitTool.Instance.jobName = this.jobName;
                                Frm_AlignFitTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_AlignFitTool.Instance.Show();
                                Frm_AlignFitTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_AlignFitTool.Instance.button5.Focus();
                                AlignFitTool alignFitTool = (AlignFitTool)(L_toolList[i].tool);
                                Frm_AlignFitTool.alignFitTool = alignFitTool;
                                Application.DoEvents();

                                Frm_AlignFitTool.Instance.pictureBox2.Image = L_toolList[i].enable ? Resources.开 : Resources.关;



                                Frm_AlignFitTool.Instance.tbx_inputPosX.Text = alignFitTool.inputPos.Point.X.ToString();
                                Frm_AlignFitTool.Instance.tbx_inputPosY.Text = alignFitTool.inputPos.Point.Y.ToString();
                                Frm_AlignFitTool.Instance.tbx_inputPosU.Text = alignFitTool.inputPos.U.ToString();

                                Frm_AlignFitTool.Instance.tbx_pickPosX.Text = alignFitTool.TemplateElementPlacePos.Point.X.ToString();
                                Frm_AlignFitTool.Instance.tbx_pickPosY.Text = alignFitTool.TemplateElementPlacePos.Point.Y.ToString();
                                Frm_AlignFitTool.Instance.tbx_pickPosU.Text = alignFitTool.TemplateElementPlacePos.U.ToString();

                                Frm_AlignFitTool.Instance.tbx_featureX.Text = alignFitTool.TemplateBelowBoardPos.Point.X.ToString();
                                Frm_AlignFitTool.Instance.tbx_featureY.Text = alignFitTool.TemplateBelowBoardPos.Point.Y.ToString();
                                Frm_AlignFitTool.Instance.tbx_featureU.Text = alignFitTool.TemplateBelowBoardPos.U.ToString();

                                Frm_AlignFitTool.Instance.tbx_resultPosX.Text = alignFitTool.toolPar.ResultPar.组装位置.Point.X.ToString();
                                Frm_AlignFitTool.Instance.tbx_resultPosY.Text = alignFitTool.toolPar.ResultPar.组装位置.Point.Y.ToString();
                                Frm_AlignFitTool.Instance.tbx_resultPosU.Text = alignFitTool.toolPar.ResultPar.组装位置.U.ToString();

                                Frm_AlignFitTool.Instance.tbx_pickPosOffsetX.Text = alignFitTool.TemplateElementPlacePosOffset.Point.X.ToString();
                                Frm_AlignFitTool.Instance.tbx_pickPosOffsetY.Text = alignFitTool.TemplateElementPlacePosOffset.Point.Y.ToString();
                                Frm_AlignFitTool.Instance.tbx_pickPosOffsetU.Text = alignFitTool.TemplateElementPlacePosOffset.U.ToString();

                                Frm_AlignFitTool.Instance.tbx_saftyRangeX.Text = alignFitTool.L_safetyRange.Point.X.ToString();
                                Frm_AlignFitTool.Instance.tbx_saftyRangeY.Text = alignFitTool.L_safetyRange.Point.Y.ToString();
                                Frm_AlignFitTool.Instance.tbx_saftyRangeU.Text = alignFitTool.L_safetyRange.U.ToString();

                                break;
                            #endregion

                            #region AlignWithoutCalibRotateCenter
                            case ToolType.AlignWithoutCalibRotateCenter:
                                Frm_AlignWithoutCalibRotateCenterTool.Instance.lbl_title.Text = string.Format("上相机定位    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_UpCamAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_UpCamAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_UpCamAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_UpCamAlignTool.Instance.TopMost = true;
                                Frm_AlignWithoutCalibRotateCenterTool.Instance.Activate();
                                Frm_AlignWithoutCalibRotateCenterTool.Instance.jobName = this.jobName;
                                Frm_AlignWithoutCalibRotateCenterTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_AlignWithoutCalibRotateCenterTool.Instance.Show();
                                Frm_AlignWithoutCalibRotateCenterTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_AlignWithoutCalibRotateCenterTool.Instance.btn_runTool.Focus();
                                AlignWithoutCalibRotateCenterTool alignWithoutCalibRotateCenterTool = (AlignWithoutCalibRotateCenterTool)(L_toolList[i].tool);
                                Frm_AlignWithoutCalibRotateCenterTool.alignWithoutCalibRotateCenterTool = alignWithoutCalibRotateCenterTool;
                                Application.DoEvents();

                                //Frm_UpCamAlignTool.Instance.cbx_toolEnable.Checked = L_toolList[i].enable;

                                //////Frm_UpCamAlignTool.Instance.cbx_toolList.Items.Clear();
                                //////Frm_UpCamAlignTool.Instance.cbx_toolList.Items.AddRange(upCamAlignTool.L_toolName.ToArray());
                                //////Frm_UpCamAlignTool.Instance.cbx_toolList.SelectedIndex = upCamAlignTool.toolIdx;

                                //////Frm_UpCamAlignTool.Instance.tbx_inputPosX.Text = upCamAlignTool.inputPos.Point.X.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_inputPosY.Text = upCamAlignTool.inputPos.Point.Y.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_inputPosU.Text = upCamAlignTool.inputPos.U.ToString();

                                //////Frm_UpCamAlignTool.Instance.tbx_pickPosX.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_pickPosY.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_pickPosU.Text = upCamAlignTool.L_pickPos[upCamAlignTool.toolIdx].U.ToString();

                                //////Frm_UpCamAlignTool.Instance.tbx_featureX.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_featureY.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_featureU.Text = upCamAlignTool.L_featurePos[upCamAlignTool.toolIdx].U.ToString();

                                //////Frm_UpCamAlignTool.Instance.tbx_resultPosX.Text = upCamAlignTool.resultPos.Point.X.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_resultPosY.Text = upCamAlignTool.resultPos.Point.Y.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_resultPosU.Text = upCamAlignTool.resultPos.U.ToString();

                                //////Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetX.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetY.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_pickPosOffsetU.Text = upCamAlignTool.L_pickPosOffset[upCamAlignTool.toolIdx].U.ToString();

                                //////Frm_UpCamAlignTool.Instance.tbx_saftyRangeX.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.X.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_saftyRangeY.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].Point.Y.ToString();
                                //////Frm_UpCamAlignTool.Instance.tbx_saftyRangeU.Text = upCamAlignTool.L_safetyRange[upCamAlignTool.toolIdx].U.ToString();

                                break;
                            #endregion

                            #region FindLine
                            case ToolType.FindLine:
                                Frm_FindLineTool.Instance.lbl_title.Text = string.Format("查找线    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_FindLineTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_FindLineTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_FindLineTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_FindLineTool.Instance.TopMost = true;
                                Frm_FindLineTool.Instance.Activate();
                                Frm_FindLineTool.Instance.jobName = this.jobName;
                                Frm_FindLineTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_FindLineTool.Instance.Show();
                                Frm_FindLineTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_FindLineTool.Instance.btn_runTool.Focus();
                                FindLineTool findLineTool = (FindLineTool)(L_toolList[i].tool);
                                Frm_FindLineTool.findLineTool = findLineTool;
                                Application.DoEvents();



                                inputItemNum = (L_toolList[i]).input.Count;

                                for (int j = 0; j < inputItemNum; j++)
                                {
                                    string inputItemName = L_toolList[i].input[j].IOName;
                                    string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                                    if (sourceFrom == string.Empty)
                                    {
                                        continue;
                                    }
                                    if (inputItemName == "图像" || inputItemName == "InputImage")
                                    {
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        findLineTool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                        if (findLineTool.toolPar.InputPar.图像 == null)
                                        {
                                            continue;
                                        }
                                    }
                                    else if (inputItemName == "跟随" || inputItemName == "Pose")
                                    {
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        findLineTool.toolPar.InputPar.跟随 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>;
                                        if (findLineTool.toolPar.InputPar.跟随 == null)
                                        {
                                            continue;
                                        }
                                    }

                                }




                                if (findLineTool.toolPar.InputPar.图像 != null)
                                {
                                    Frm_FindLineTool.Instance.hWindow_Final1.HobjectToHimage(findLineTool.toolPar.InputPar.图像);
                                    if (findLineTool.L_regions == null)
                                        findLineTool.L_regions = new List<ViewWindow.Model.ROI>();
                                    if (findLineTool.L_regions.Count == 0)
                                    {
                                        HTuple width, height;
                                        HOperatorSet.GetImageSize(findLineTool.toolPar.InputPar.图像, out width, out height);
                                        Frm_FindLineTool.Instance.hWindow_Final1.viewWindow.genRect2(height.D / 2.0, width.D / 2.0, 0, Math.Max(10.0, height.D / 20.0), Math.Max(30.0, width.D / 6.0), ref findLineTool.L_regions);
                                    }

                                    // 先将学习时 ROI 刚性变换到当前模板匹配位姿，再作为当前编辑基准。
                                    // 这样配置窗口看到的 ROI、预览卡尺和正式运行使用同一套坐标。
                                    findLineTool.EnsureTemplatePoseFromCurrentInput();
                                    findLineTool.RebaseRoiToCurrentFollowPose();
                                    Frm_FindLineTool.Instance.regions = findLineTool.L_regions;


                                    findLineTool.ShowContour(true, false);








                                }
                                else
                                    findLineTool.ClearWindow();

                                Application.DoEvents();

                                //Frm_FindLineTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;
                                Frm_FindLineTool.Instance.ckb_displayCaliper.Checked = findLineTool.displayCaliper;
                                Frm_FindLineTool.Instance.ckb_displayFeature.Checked = findLineTool.displayFeature;
                                //////Frm_FindLineTool.Instance.tbx_expectLineStartRow.Text = findLineTool.expectLineStartRow.ToString();
                                //////Frm_FindLineTool.Instance.tbx_expectLineStartCol.Text = findLineTool.expectLineStartCol.ToString();
                                //////Frm_FindLineTool.Instance.tbx_expectLineEndRow.Text = findLineTool.expectLineEndRow.ToString();
                                //////Frm_FindLineTool.Instance.tbx_expectLineEndCol.Text = findLineTool.expectLineEndCol.ToString();
                                Frm_FindLineTool.Instance.cbx_edgeSelect.Text = findLineTool.edgeSelect;
                                Frm_FindLineTool.Instance.numericUpDown1.Value = findLineTool.minScore;
                                Frm_FindLineTool.Instance.cbx_polarity.Text = findLineTool.polarity == "positive" ? "从明到暗" : "从暗到明";
                                Frm_FindLineTool.Instance.tbx_caliperNum.Value = findLineTool.cliperNum;
                                Frm_FindLineTool.Instance.tbx_threshold.Value = findLineTool.threshold;
                                Frm_FindLineTool.Instance.numericUpDown3.Value = findLineTool.Length;
                                Frm_FindLineTool.Instance.numericUpDown2.Value = findLineTool.caliperWidth;
                                Frm_FindLineTool.Instance.textBox2.Value = findLineTool.ignoreNum;
                                Frm_FindLineTool.Instance.cCheckBox3.Checked = findLineTool.displayLine;
                                //Frm_FindLineTool.Instance.tbx_threshold.Value = findLineTool.threshold;

                                break;
                            #endregion

                            #region FindCircle
                            case ToolType.FindCircle:
                                // 双击或打开"查找圆"工具时进入这里：
                                // 1. 绑定当前 job/tool 到单例窗体；
                                // 2. 从流程输入连接读取图像和跟随位姿；
                                // 3. 显示输入图、ROI 和卡尺预览；
                                // 4. 把工具参数回填到界面控件。
                                Frm_FindCircleTool.Instance.lbl_title.Text = string.Format("查找圆    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_FindCircleTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_FindCircleTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_FindCircleTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_FindCircleTool.Instance.TopMost = true;
                                Frm_FindCircleTool.Instance.Activate();
                                Frm_FindCircleTool.Instance.jobName = this.jobName;
                                Frm_FindCircleTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_FindCircleTool.Instance.Show();
                                Frm_FindCircleTool.Instance.WindowState = FormWindowState.Normal;
                                //////Frm_FindCircleTool.Instance.btn_runFindCircleTool.Focus();
                                FindCircleTool findCircleTool = (FindCircleTool)(L_toolList[i].tool);
                                Frm_FindCircleTool.findCircleTool = findCircleTool;
                                Application.DoEvents();







                                inputItemNum = (L_toolList[i]).input.Count;

                                for (int j = 0; j < inputItemNum; j++)
                                {
                                    string inputItemName = L_toolList[i].input[j].IOName;
                                    string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                                    if (sourceFrom == string.Empty)
                                    {
                                        continue;
                                    }
                                    if (inputItemName == "图像" || inputItemName == "InputImage")
                                    {
                                        // 输入图像来自上游工具输出，例如"采集图像->图像"或预处理工具输出。
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        findCircleTool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                        if (findCircleTool.toolPar.InputPar.图像 == null)
                                        {
                                            continue;
                                        }
                                    }
                                    else if (inputItemName == "跟随" || inputItemName == "Pose")
                                    {
                                        // 跟随通常来自定位工具输出的 List<XYU>。
                                        // FindCircleTool 会用它把学习时的预期圆移动到当前工件位置。
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        findCircleTool.toolPar.InputPar.跟随 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>;
                                        if (findCircleTool.toolPar.InputPar.跟随 == null)
                                        {
                                            continue;
                                        }
                                    }

                                }





                                HTuple newExpecCircleRow = new HTuple();
                                HTuple newExpectCircleCol = new HTuple();
                                HTuple newExpectCircleRadius = new HTuple();
                                try
                                {
                                    if (findCircleTool.toolPar.InputPar.跟随.Count != 0)
                                    {


                                        HTuple _homMat2D;
                                        HOperatorSet.VectorAngleToRigid(findCircleTool.templatePose[0].Point.X, findCircleTool.templatePose[0].Point.Y, findCircleTool.templatePose[0].U, findCircleTool.toolPar.InputPar.跟随[0].Point.X, findCircleTool.toolPar.InputPar.跟随[0].Point.Y, findCircleTool.toolPar.InputPar.跟随[0].U, out _homMat2D);
                                        // 打开界面预览时先计算当前跟随位姿下的预期圆心。
                                        HTuple tempR, tempC;
                                        HOperatorSet.AffineTransPixel(_homMat2D, (HTuple)findCircleTool.L_regions[0].getModelData()[0], (HTuple)findCircleTool.L_regions[0].getModelData()[1], out tempR, out tempC);
                                        newExpecCircleRow = tempR;
                                        newExpectCircleCol = tempC;
                                        newExpectCircleRadius = findCircleTool.L_regions[0].getModelData()[2].D;

                                    }
                                    else
                                    {
                                        newExpecCircleRow = findCircleTool.L_regions[0].getModelData()[0];
                                        newExpectCircleCol = findCircleTool.L_regions[0].getModelData()[1];
                                        newExpectCircleRadius = findCircleTool.L_regions[0].getModelData()[2];
                                    }
                                }
                                catch { }





                                if (findCircleTool.toolPar.InputPar.图像 != null)
                                {
                                    Frm_FindCircleTool.Instance.hWindow_Final1.HobjectToHimage(findCircleTool.toolPar.InputPar.图像);
                                    if (findCircleTool.L_regions.Count == 0)
                                    {
                                        Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.genCircle(newExpecCircleRow, newExpectCircleCol, newExpectCircleRadius, ref findCircleTool.L_regions);
                                        Frm_FindCircleTool.Instance.regions = findCircleTool.L_regions;
                                    }
                                    else
                                    {
                                        findCircleTool.L_regions.Clear();
                                        Frm_FindCircleTool.Instance.hWindow_Final1.viewWindow.genCircle(newExpecCircleRow, newExpectCircleCol, newExpectCircleRadius, ref findCircleTool.L_regions);
                                        Frm_FindCircleTool.Instance.regions = findCircleTool.L_regions;
                                    }


                                    findCircleTool.ShowContour(true, false);
                                    // 打开界面时显示卡尺预览，并把可编辑 ROI 保持在模板最上层；
                                    // 真正运行并写输出的是 FindCircleTool.Run()/Execute()。
                                    //////    findCircleTool.newExpecCircleRow.Clear();
                                    //////    findCircleTool.newExpectCircleCol.Clear();
                                    //////    findCircleTool.newExpectCircleRadius.Clear();
                                    //////    if (findCircleTool.inputPose != null)
                                    //////    {


                                    //////            HTuple _homMat2D;
                                    //////            HOperatorSet.VectorAngleToRigid(findCircleTool.templatePose[0].Point.X, findCircleTool.templatePose[0].Point.Y, findCircleTool.templatePose[0].U, findCircleTool.inputPose[0].Point.X, findCircleTool.inputPose[0].Point.Y, findCircleTool.inputPose[0].U, out _homMat2D);
                                    //////            //对预期线的起始点做放射变换
                                    //////            HTuple tempR, tempC;
                                    //////            HOperatorSet.AffineTransPixel(_homMat2D, (HTuple)findCircleTool.L_regions[0].getModelData()[0], (HTuple)findCircleTool.L_regions[0].getModelData()[1], out tempR, out tempC);
                                    //////            findCircleTool.newExpecCircleRow.Add(tempR);
                                    //////            findCircleTool.newExpectCircleCol.Add(tempC);
                                    //////            findCircleTool.newExpectCircleRadius.Add(findCircleTool.L_regions[0].getModelData()[2].D);

                                    //////    }
                                    //////    else
                                    //////    {
                                    //////        findCircleTool.newExpecCircleRow.Add(findCircleTool.L_regions[0].getModelData()[0]);
                                    //////        findCircleTool.newExpectCircleCol.Add(findCircleTool.L_regions[0].getModelData()[1]);
                                    //////        findCircleTool.newExpectCircleRadius.Add(findCircleTool.L_regions[0].getModelData()[2]);
                                    //////    }

                                    //////    HTuple handleID;
                                    //////    HOperatorSet.CreateMetrologyModel(out   handleID);
                                    //////    HTuple width, height;
                                    //////    HOperatorSet.GetImageSize(findCircleTool.inputImage, out width, out height);
                                    //////    HOperatorSet.SetMetrologyModelImageSize(handleID, width[0], height[0]);
                                    //////    HTuple index;
                                    //////    HOperatorSet.AddMetrologyObjectCircleMeasure(handleID, findCircleTool.newExpecCircleRow[0], findCircleTool.newExpectCircleCol[0], findCircleTool.newExpectCircleRadius[0], new HTuple(findCircleTool.ringRadiusLength), new HTuple(5), new HTuple(1), new HTuple(30), new HTuple(), new HTuple(), out index);

                                    //////    //参数在这里设置
                                    //////    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_transition"), new HTuple(findCircleTool.polarity));
                                    //////    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("num_measures"), new HTuple(findCircleTool.cliperNum));
                                    //////    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length1"), new HTuple(findCircleTool.ringRadiusLength));
                                    //////    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_length2"), new HTuple(findCircleTool.caliperWidth));
                                    //////    HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_threshold"), new HTuple(findCircleTool.threshold));
                                    //////    //////HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("measure_select"), new HTuple(edgeSelect));
                                    //////    //////HOperatorSet.SetMetrologyObjectParam(handleID, new HTuple("all"), new HTuple("min_score"), new HTuple(minScore));
                                    //////    HOperatorSet.ApplyMetrologyModel(findCircleTool.inputImage, handleID);



                                    //////HObject contours;
                                    //////HTuple row, col;
                                    //////HOperatorSet.GetMetrologyObjectMeasures(out contours, handleID, new HTuple("all"), new HTuple("all"), out row, out col);
                                    //////HOperatorSet.SetColor(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID, new HTuple("cyan"));
                                    //////HOperatorSet.DispObj(contours, Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);

                                }
                                else
                                    HOperatorSet.ClearWindow(Frm_FindCircleTool.Instance.hWindow_Final1.HWindowHalconID);



                                findCircleTool.templatePose.Clear();
                                if (findCircleTool.toolPar.InputPar.跟随 != null && findCircleTool.toolPar.InputPar.跟随.Count > 0)
                                {
                                    XYU temp = new XYU();
                                    temp.Point.X = findCircleTool.toolPar.InputPar.跟随[0].Point.X;
                                    temp.Point.Y = findCircleTool.toolPar.InputPar.跟随[0].Point.Y;
                                    temp.U = findCircleTool.toolPar.InputPar.跟随[0].U;
                                    findCircleTool.templatePose.Add(temp);
                                }

                                //将对象信息更新到界面
                                Frm_FindCircleTool.Instance.pictureBox8.Image = L_toolList[i].enable ? Resources.开 : Resources.关;

                                Frm_FindCircleTool.Instance.pictureBox3.Image = findCircleTool.displayCaliper ? Resources.复选框 : Resources.去复选框;
                                Frm_FindCircleTool.Instance.pictureBox4.Image = findCircleTool.displayFeature ? Resources.复选框 : Resources.去复选框;
                                Frm_FindCircleTool.Instance.pictureBox5.Image = findCircleTool.displayCircle ? Resources.复选框 : Resources.去复选框;
                                Frm_FindCircleTool.Instance.pictureBox2.Image = findCircleTool.displayCircleCenter ? Resources.复选框 : Resources.去复选框;
                                switch (findCircleTool.edgeSelect)
                                {
                                    case "first":
                                        Frm_FindCircleTool.Instance.comboBox1.SelectedIndex = 0;
                                        break;
                                    case "last":
                                        Frm_FindCircleTool.Instance.comboBox1.SelectedIndex = 1;
                                        break;
                                    case "all":
                                        Frm_FindCircleTool.Instance.comboBox1.SelectedIndex = 2;
                                        break;
                                }

                                Frm_FindCircleTool.Instance.numericUpDown1.Value = findCircleTool.minScore;
                                Frm_FindCircleTool.Instance.tbx_ringRadiusLength.Value = findCircleTool.ringRadiusLength;
                                Frm_FindCircleTool.Instance.tbx_threshold.Value = findCircleTool.threshold;
                                Frm_FindCircleTool.Instance.tbx_cliperNum.Value = findCircleTool.cliperNum;
                                Frm_FindCircleTool.Instance.cbx_polarity.SelectedIndex = (findCircleTool.polarity == "negative" ? 0 : 1);
                                Frm_FindCircleTool.Instance.ckb_displayCaliper.Checked = findCircleTool.displayCaliper;
                                Frm_FindCircleTool.Instance.ckb_displayFeature.Checked = findCircleTool.displayFeature;
                                Frm_FindCircleTool.Instance.ckb_displayCircle.Checked = findCircleTool.displayCircle;
                                Frm_FindCircleTool.Instance.checkBox1.Checked = findCircleTool.displayCircleCenter;
                                Frm_FindCircleTool.Instance.textBox1.Value = findCircleTool.caliperWidth;
                                Frm_FindCircleTool.Instance.textBox2.Value = findCircleTool.ignoreNum;

                                break;
                            #endregion

                            #region SubImage
                            case ToolType.SubImage:
                                Frm_SubImageTool.Instance.pictureBox1.Image = Resources.SubImageTool;
                                Frm_SubImageTool.Instance.lbl_title.Text = string.Format("减图像    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_SubImageTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_SubImageTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_SubImageTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_SubImageTool.Instance.TopMost = true;
                                Frm_SubImageTool.Instance.Activate();
                                Frm_SubImageTool.Instance.jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                                Frm_SubImageTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_SubImageTool.Instance.Show();
                                Frm_SubImageTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_SubImageTool.Instance.btn_runImageSubTool.Focus();
                                SubImageTool subImageTool = (SubImageTool)(L_toolList[i].tool);
                                Frm_SubImageTool.subImageTool = subImageTool;
                                Application.DoEvents();

                                if (subImageTool.inputImage != null)
                                    subImageTool.ShowImage(subImageTool.inputImage);
                                else
                                    //////subImageTool.ClearWindow(jobName);

                                    Frm_SubImageTool.Instance.ckb_subImageToolEnable.Checked = L_toolList[i].enable;
                                Frm_SubImageTool.Instance.cbx_standardImage.Text = subImageTool.standardImageName;
                                break;
                            #endregion

                            #region CreateROI
                            case ToolType.CreateROI:
                                Frm_CreateROITool.Instance.pictureBox1.Image = Resources.CreateROITool;
                                Frm_CreateROITool.Instance.lbl_title.Text = string.Format("创建ROI    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_CreateROITool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_CreateROITool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_CreateROITool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_CreateROITool.Instance.TopMost = true;
                                Frm_CreateROITool.Instance.Activate();
                                Frm_CreateROITool.Instance.jobName = this.jobName;
                                Frm_CreateROITool.Instance.toolName = L_toolList[i].toolName;
                                Frm_CreateROITool.Instance.Show();
                                Frm_CreateROITool.Instance.WindowState = FormWindowState.Normal;
                                //////Frm_CreateROITool.Instance.btn_runFindCircleTool.Focus();
                                CreateROITool createROITool = (CreateROITool)(L_toolList[i].tool);
                                Frm_CreateROITool.createROITool = createROITool;
                                Application.DoEvents();


                                inputItemNum = (L_toolList[i]).input.Count;
                                for (int j = 0; j < inputItemNum; j++)
                                {
                                    string inputItem = L_toolList[i].input[j].IOName;
                                    string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString(); if (inputItem == string.Empty)
                                    {
                                        createROITool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                        treeNode.ToolTipText = createROITool.toolRunStatu.ToString();
                                        treeNode.ForeColor = Color.Red;
                                        Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, createROITool.toolRunStatu.ToString()), Color.Red);
                                        return;
                                    }

                                    //////createROITool.inputPose = null;
                                    if (inputItem == "左上点行")
                                    {
                                        string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        createROITool.leftTopRow = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                    }
                                    else if (inputItem == "左上点列")
                                    {
                                        string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        createROITool.leftTopCol = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                    }
                                    else if (inputItem == "右下点行" || inputItem == "ExpectCircleCenterX")
                                    {
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        createROITool.rightDownRow = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                    }
                                    else if (inputItem == "右下点列" || inputItem == "ExpectCircleCenterY")
                                    {
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        createROITool.rightDownCol = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                    }
                                    else if (inputItem == "跟随" || inputItem == "ExpectCircleCenterY")
                                    {
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        createROITool.inputPose = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                    }
                                    else if (inputItem == "图像" || inputItem == "ExpectC信息ircleCenterY")
                                    {
                                        string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                        sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                        string toolItem = Regex.Split(sourceFrom, "->")[1];
                                        createROITool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                    }
                                }




                                //显示背景图
                                Frm_CreateROITool.Instance.hWindow_Final1.HobjectToHimage(createROITool.toolPar.InputPar.图像);


                                Frm_CreateROITool.Instance.hWindow_Final1.viewWindow.displayROI(createROITool.regions);
                                Frm_CreateROITool.regions = createROITool.regions;

                                Frm_CreateROITool.binDataGridView(Frm_CreateROITool.Instance.dgv_ROI, createROITool.regions);


                                //将对象信息更新到界面
                                //////Frm_CreateROITool.Instance.ckb_createROIToolEnable.Checked = L_toolList[i].enable;
                                //////Frm_CreateROITool.Instance.tbx_leftTopRow.Text = createROITool.leftTopRow.ToString();
                                //////Frm_CreateROITool.Instance.tbx_leftTopCol.Text = createROITool.leftTopCol.ToString();
                                //////Frm_CreateROITool.Instance.tbx_rightDownRow.Text = createROITool.rightDownRow.ToString();
                                //////Frm_CreateROITool.Instance.tbx_rightDownCol.Text = createROITool.rightDownCol.ToString();
                                break;
                            #endregion

                            #region ArrayRegion
                            case ToolType.ArrayRegion:
                                Frm_ArrayRegionTool.Instance.pictureBox1.Image = Resources.RegionArrayTool;
                                Frm_ArrayRegionTool.Instance.lbl_title.Text = string.Format("阵列区域    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_ArrayRegionTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_ArrayRegionTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ArrayRegionTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_ArrayRegionTool.Instance.TopMost = true;
                                Frm_ArrayRegionTool.Instance.Activate();
                                Frm_ArrayRegionTool.Instance.jobName = this.jobName;
                                Frm_ArrayRegionTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_ArrayRegionTool.Instance.Show();
                                Frm_ArrayRegionTool.Instance.WindowState = FormWindowState.Normal;
                                //////Frm_CreateROITool.Instance.btn_runDistancePLTool.Focus();
                                ArrayRegionTool arrayRegionTool = (ArrayRegionTool)(L_toolList[i].tool);
                                Frm_ArrayRegionTool.arrayRegionTool = arrayRegionTool;
                                Application.DoEvents();

                                //将对象信息更新到界面
                                if (arrayRegionTool.inputImage != null)
                                    arrayRegionTool.ShowImage(arrayRegionTool.inputImage);
                                else
                                    arrayRegionTool.ClearWindow();

                                if (arrayRegionTool.outputRegion != null)
                                {
                                    GetImageWindowControl().hwc_imageWindow.viewWindow.displayROI(arrayRegionTool.regions);
                                    GetImageWindowControl().regions = arrayRegionTool.regions;
                                }

                                Frm_ArrayRegionTool.Instance.ckb_shapeMatchToolEnable.Checked = L_toolList[i].enable;
                                Frm_ArrayRegionTool.Instance.textBox1.Text = arrayRegionTool.rowNum.ToString();
                                Frm_ArrayRegionTool.Instance.textBox2.Text = arrayRegionTool.colNum.ToString();
                                Frm_ArrayRegionTool.Instance.textBox3.Text = arrayRegionTool.rowSpan.ToString();
                                Frm_ArrayRegionTool.Instance.textBox4.Text = arrayRegionTool.colSpan.ToString();

                                break;
                            #endregion

                            #region Mark
                            case ToolType.Mark:
                                Frm_MarkTool.Instance.pictureBox1.Image = Resources.MarkTool;
                                Frm_MarkTool.Instance.lbl_title.Text = string.Format("标记点    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_MarkTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_MarkTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_MarkTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_MarkTool.Instance.TopMost = true;
                                Frm_MarkTool.Instance.Activate();
                                Frm_MarkTool.Instance.jobName = this.jobName;
                                Frm_MarkTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_MarkTool.Instance.Show();
                                Frm_MarkTool.Instance.WindowState = FormWindowState.Normal;
                                //////Frm_MarkTool.Instance.btn_runDownCamAlignTool.Focus();
                                MarkTool markTool = (MarkTool)(L_toolList[i].tool);
                                Frm_MarkTool.markTool = markTool;
                                Application.DoEvents();

                                Frm_MarkTool.Instance.ckb_shapeMatchToolEnable.Checked = L_toolList[i].enable;
                                Frm_MarkTool.Instance.tbx_caputurePosX.Text = markTool.inputPoint.X.ToString();
                                Frm_MarkTool.Instance.tbx_caputurePosY.Text = markTool.inputPoint.Y.ToString();

                                break;
                            #endregion

                            #region PoseToStr
                            case ToolType.ToStr:
                                Frm_PoseToStrTool.Instance.pictureBox1.Image = Resources.UnknownTool;
                                Frm_PoseToStrTool.Instance.lbl_title.Text = string.Format("转文本    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_PoseToStrTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_PoseToStrTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_PoseToStrTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_PoseToStrTool.Instance.TopMost = true;
                                Frm_PoseToStrTool.Instance.Activate();
                                Frm_PoseToStrTool.Instance.jobName = this.jobName;
                                Frm_PoseToStrTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_PoseToStrTool.Instance.Show();
                                Frm_PoseToStrTool.Instance.WindowState = FormWindowState.Normal;
                                //////Frm_MarkTool.Instance.btn_runDownCamAlignTool.Focus();
                                ToStrTool poseToStrTool = (ToStrTool)(L_toolList[i].tool);
                                Frm_PoseToStrTool.poseToStrTool = poseToStrTool;
                                Application.DoEvents();

                                //////Frm_PoseToStrTool.Instance.ckb_toolEnable.Checked = L_toolList[i].enable;
                                Frm_PoseToStrTool.Instance.textBox1.TextStr = poseToStrTool.splitChar;

                                break;
                            #endregion

                            #region DistancePL
                            case ToolType.DistancePL:
                                Frm_DistancePLTool.Instance.pictureBox1.Image = Resources.DistancePLTool;
                                Frm_DistancePLTool.Instance.lbl_title.Text = string.Format("点线距离    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_DistancePLTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_DistancePLTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_DistancePLTool.Instance.Width - 2, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_DistancePLTool.Instance.TopMost = true;
                                Frm_DistancePLTool.Instance.Activate();
                                Frm_DistancePLTool.Instance.jobName = this.jobName;
                                Frm_DistancePLTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_DistancePLTool.Instance.Show();
                                Frm_DistancePLTool.Instance.WindowState = FormWindowState.Normal;
                                //  Frm_DistancePointLineTool.Instance.dd.Focus();
                                DistancePLTool distancePLTool = (DistancePLTool)(L_toolList[i].tool);
                                Application.DoEvents();

                                if (((DistancePLTool)(L_toolList[i].tool)).inputImage != null)
                                    GetImageWindowControl().Display_Image(((DistancePLTool)(L_toolList[i].tool)).inputImage);
                                else
                                    HOperatorSet.ClearWindow(Frm_ImageWindow.Instance.WindowHandle);

                                //将对象信息更新到界面
                                Frm_DistancePLTool.Instance.ckb_distancePLToolEnable.Checked = L_toolList[i].enable;

                                break;
                            #endregion

                            #region DistanceSS
                            case ToolType.DistanceSS:
                                Frm_DistanceLLTool.Instance.pictureBox1.Image = Resources.UnknownTool;
                                Frm_DistanceLLTool.Instance.lbl_title.Text = string.Format("线段与线段距离    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_DistanceLLTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_DistanceLLTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_DistanceLLTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_DistanceLLTool.Instance.TopMost = true;
                                Frm_DistanceLLTool.Instance.Activate();
                                Frm_DistanceLLTool.Instance.jobName = this.jobName;
                                Frm_DistanceLLTool.Instance.toolName = L_toolList[i].toolName;
                                ////// Frm_DistanceSegmentAndSegmentTool.Instance.Show();
                                Frm_DistanceLLTool.Instance.WindowState = FormWindowState.Normal;
                                //////SharpEdit.Form1.Instance.fctb.Focus();
                                DistanceLLTool distanceSSTool = (DistanceLLTool)(L_toolList[i].tool);
                                Application.DoEvents();

                                //将对象信息更新到界面
                                Frm_MessageBox.Instance.MessageBoxShow("\r\n本工具为无窗体工具！");
                                break;
                            #endregion

                            #region LLPoint
                            case ToolType.LLIntersect:
                                Frm_LLIntersectTool.Instance.pictureBox1.Image = Resources.LLIntersectionTool;
                                Frm_LLIntersectTool.Instance.lbl_title.Text = string.Format("线线交点    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_LLIntersectTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_LLIntersectTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_DistanceLLTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_LLIntersectTool.Instance.TopMost = true;
                                Frm_LLIntersectTool.Instance.Activate();
                                //////Frm_LLPointTool.jobName = this.jobName;
                                //////Frm_LLPointTool.toolName = L_toolList[i].toolName;
                                ////// Frm_DistanceSegmentAndSegmentTool.Instance.Show();
                                Frm_LLIntersectTool.Instance.WindowState = FormWindowState.Normal;
                                //////SharpEdit.Form1.Instance.fctb.Focus();
                                LLIntersectTool llPointTool = (LLIntersectTool)(L_toolList[i].tool);
                                Application.DoEvents();

                                //将对象信息更新到界面
                                Frm_MessageBox.Instance.MessageBoxShow("\r\n本工具为无窗体工具！");
                                break;
                            #endregion

                            #region RegionFeature
                            case ToolType.RegionFeature:
                                Frm_RegionFeatureTool.Instance.pictureBox1.Image = Resources.RegionFeatureTool;
                                Frm_RegionFeatureTool.Instance.lbl_title.Text = string.Format("区域特征    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_RegionFeatureTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_RegionFeatureTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_RegionFeatureTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_RegionFeatureTool.Instance.TopMost = true;
                                Frm_RegionFeatureTool.Instance.Activate();
                                Frm_RegionFeatureTool.Instance.jobName = this.jobName;
                                Frm_RegionFeatureTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_RegionFeatureTool.Instance.Show();
                                Frm_RegionFeatureTool.Instance.WindowState = FormWindowState.Normal;
                                //////Frm_RegionFeatureTool.Instance.btn_runFindBarcodeTool.Focus();
                                RegionFeatureTool regionFeatureTool = (RegionFeatureTool)(L_toolList[i].tool);
                                Frm_RegionFeatureTool.regionFeatureTool = regionFeatureTool;
                                Application.DoEvents();

                                break;
                            #endregion

                            #region OCR
                            case ToolType.OCR:
                                Frm_OCRTool.Instance.pictureBox1.Image = Resources.OCRTool;
                                Frm_OCRTool.Instance.lbl_title.Text = string.Format("OCR    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_OCRTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_OCRTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ShapeMatchTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_OCRTool.Instance.TopMost = true;
                                Frm_OCRTool.Instance.Activate();
                                Frm_OCRTool.Instance.jobName = this.jobName;
                                Frm_OCRTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_OCRTool.Instance.Show();
                                Frm_OCRTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_OCRTool.Instance.btn_runOCRTool.Focus();
                                OCRTool ocrTool = (OCRTool)(L_toolList[i].tool);
                                Frm_OCRTool.ocrTool = ocrTool;
                                Application.DoEvents();

                                if (ocrTool.inputImage != null)
                                    ocrTool.ShowImage(ocrTool.inputImage);
                                else
                                    ////////ocrTool.ClearWindow(this.jobName);

                                    if (ocrTool.searchRegion != null)
                                {
                                    ocrTool.SetColor(this.jobName, "blue");
                                    //////ocrTool.ShowObj(this.jobName, ocrTool.searchRegion);
                                }

                                Frm_OCRTool.Instance.lbl_threshold.Text = ocrTool.threshold.ToString();
                                Frm_OCRTool.Instance.tkb_threshold.Value = ocrTool.threshold;
                                Frm_OCRTool.Instance.ckb_OCRToolEnable.Checked = L_toolList[i].enable;
                                Frm_OCRTool.Instance.cbx_searchRegionType.Text = ocrTool.searchRegionType.ToString();
                                Frm_OCRTool.Instance.cbx_templateRegionType.Text = ocrTool.templateRegionType.ToString();
                                Frm_OCRTool.Instance.tbx_resultStr.Text = ocrTool.outputStr;
                                Frm_OCRTool.Instance.cbx_charType.SelectedIndex = (ocrTool.charType == CharType.BlackChar ? 0 : 1);
                                Frm_OCRTool.Instance.tbx_dilationSize.Text = ocrTool.dilationSize.ToString();
                                Frm_OCRTool.Instance.tbx_standardCharList.Text = ocrTool.standardCharList;

                                break;
                            #endregion

                            #region Barcode
                            case ToolType.Barcode:
                                Frm_BarcodeTool.Instance.pictureBox1.Image = Resources.BarCodeTool;
                                Frm_BarcodeTool.Instance.lbl_title.Text = string.Format("条码    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_BarcodeTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_BarcodeTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_BarcodeTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_BarcodeTool.Instance.TopMost = true;
                                Frm_BarcodeTool.Instance.Activate();
                                Frm_BarcodeTool.Instance.jobName = this.jobName;
                                Frm_BarcodeTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_BarcodeTool.Instance.Show();
                                Frm_BarcodeTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_BarcodeTool.Instance.btn_runFindBarcodeTool.Focus();
                                BarcodeTool barcodeTool = (BarcodeTool)(L_toolList[i].tool);
                                Frm_BarcodeTool.barcodeTool = barcodeTool;
                                Application.DoEvents();

                                break;
                            #endregion

                            #region CodeEdit
                            case ToolType.CodeEdit:
                                Frm_CodeEditTool.Instance.Text = string.Format("脚本编辑    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                Frm_CodeEditTool.Instance.Activate();
                                Frm_CodeEditTool.Instance.jobName = this.jobName;
                                Frm_CodeEditTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_CodeEditTool.codeEditTool = (CodeEditTool)(L_toolList[i].tool);
                                Frm_CodeEditTool.Instance.Show();
                                Frm_CodeEditTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_CodeEditTool.Instance.LoadToolData();
                                Application.DoEvents();
                                break;
                            #endregion

                            #region DataAnalyse
                            case ToolType.DataAnalyse:
                                Frm_DataAnalyseTool.Instance.pictureBox1.Image = Resources.LabelTool;
                                Frm_DataAnalyseTool.Instance.lbl_title.Text = string.Format("数据显示    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_LabelTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_LabelTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_LabelTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_LabelTool.Instance.TopMost = true;
                                Frm_DataAnalyseTool.Instance.Activate();
                                Frm_DataAnalyseTool.Instance.jobName = this.jobName;
                                Frm_DataAnalyseTool.Instance.toolName = L_toolList[i].toolName; ;
                                Frm_DataAnalyseTool.Instance.Show();
                                Frm_DataAnalyseTool.Instance.WindowState = FormWindowState.Normal;
                                //Frm_LabelTool.Instance.btn_runTool.Focus();
                                DataAnalyseTool dataAnalyseTool = (DataAnalyseTool)(L_toolList[i].tool);
                                Frm_DataAnalyseTool.dataAnalyseTool = dataAnalyseTool;
                                Application.DoEvents();

                                int itemCount = ((DataAnalyseTool)L_toolList[i].tool).L_items.Count;
                                Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows.Clear();
                                for (int j = 0; j < itemCount; j++)
                                {
                                    int index = Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows.Add();
                                    Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[0].Value = dataAnalyseTool.L_items[j].inputItem;
                                    Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[1].Value = dataAnalyseTool.L_items[j].downLimit.ToString().ToString();
                                    Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[2].Value = dataAnalyseTool.L_items[j].upLimit.ToString();
                                    Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[3].Value = dataAnalyseTool.L_items[j].inResult;
                                    Frm_DataAnalyseTool.Instance.dgv_outputItem.Rows[index].Cells[4].Value = dataAnalyseTool.L_items[j].outResult;
                                }

                                //Frm_LabelTool.Instance.ckb_toolEnable.Checked = ((ToolInfo)L_toolList[i]).enable;
                                break;
                            #endregion

                            #region OPTLight
                            case ToolType.Light_OPT:
                                Frm_OPTLightTool.Instance.pictureBox1.Image = Resources.LightTool;
                                Frm_OPTLightTool.Instance.lbl_title.Text = string.Format("奥普特光源控制    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_OPTLightTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_OPTLightTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_OPTLightTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_OPTLightTool.Instance.TopMost = true;
                                Frm_OPTLightTool.Instance.Activate();
                                Frm_OPTLightTool.Instance.jobName = this.jobName;
                                Frm_OPTLightTool.Instance.toolName = L_toolList[i].toolName;
                                Light_OPTTool optLightTool = (Light_OPTTool)(L_toolList[i].tool);
                                Frm_OPTLightTool.optLightTool = optLightTool;
                                Frm_OPTLightTool.Instance.Show();
                                Frm_OPTLightTool.Instance.WindowState = FormWindowState.Normal;
                                //Frm_OPTLightTool.Instance.btn_runShapeMatchTool.Focus();
                                Application.DoEvents();
                                break;
                            #endregion

                            #region OPTLightControl
                            case ToolType.OPTLightControl:
                                Frm_OptLightControlTool.Instance.pictureBox1.Image = Resources.LightTool;
                                Frm_OptLightControlTool.Instance.lbl_title.Text = string.Format("光源控制    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_OptLightControlTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_OptLightControlTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_OptLightControlTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_OptLightControlTool.Instance.TopMost = true;
                                Frm_OptLightControlTool.Instance.Activate();
                                Frm_OptLightControlTool.Instance.jobName = this.jobName;
                                Frm_OptLightControlTool.Instance.toolName = L_toolList[i].toolName;
                                OptLightControlTool optLightControlTool = (OptLightControlTool)(L_toolList[i].tool);
                                Frm_OptLightControlTool.optLightControlTool = optLightControlTool;
                                Frm_OptLightControlTool.Instance.Show();
                                Frm_OptLightControlTool.Instance.WindowState = FormWindowState.Normal;
                                //Frm_OPTLightTool.Instance.btn_runShapeMatchTool.Focus();
                                Application.DoEvents();

                                Frm_OptLightControlTool.Instance.comboBox1.SelectedIndex = (optLightControlTool.controlMode ? 0 : 1);
                                break;
                            #endregion

                            #region BatteryFirstAlign
                            case ToolType.batteryFirstAlign:
                                Frm_BatteryFirstAlignTool.Instance.pictureBox1.Image = Resources.OCRTool;
                                Frm_BatteryFirstAlignTool.Instance.lbl_title.Text = string.Format("电池初定位    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_BatteryFirstAlignTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_BatteryFirstAlignTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_BatteryFirstAlignTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_BatteryFirstAlignTool.Instance.TopMost = true;
                                Frm_BatteryFirstAlignTool.Instance.Activate();
                                Frm_BatteryFirstAlignTool.Instance.jobName = this.jobName;
                                Frm_BatteryFirstAlignTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_BatteryFirstAlignTool.Instance.Show();
                                Frm_BatteryFirstAlignTool.Instance.WindowState = FormWindowState.Normal;
                                //////Frm_BatteryFirstAlignTool.Instance.btn_runShapeMatchTool.Focus();
                                BatteryFirstAlignTool batteryFirstAlignTool = (BatteryFirstAlignTool)(L_toolList[i].tool);
                                Frm_BatteryFirstAlignTool.batteryFirstAlignTool = batteryFirstAlignTool;
                                Application.DoEvents();

                                if (batteryFirstAlignTool.inputImage != null)
                                    batteryFirstAlignTool.ShowImage(batteryFirstAlignTool.inputImage);
                                else
                                    //////batteryFirstAlignTool.ClearWindow(this.jobName);

                                    if (batteryFirstAlignTool.SearchRegion != null)
                                {
                                    GetImageWindowControl().hwc_imageWindow.viewWindow.displayROI(batteryFirstAlignTool.regions);
                                    GetImageWindowControl().regions = batteryFirstAlignTool.regions;

                                }

                                if (batteryFirstAlignTool.regions.Count == 0)
                                {
                                    GetImageWindowControl().hwc_imageWindow.viewWindow.genRect1(200.0, 200.0, 600.0, 800.0, ref batteryFirstAlignTool.regions);
                                    GetImageWindowControl().regions = batteryFirstAlignTool.regions;
                                }
                                else
                                {
                                    GetImageWindowControl().hwc_imageWindow.viewWindow.displayROI(batteryFirstAlignTool.regions);
                                    GetImageWindowControl().regions = batteryFirstAlignTool.regions;
                                }

                                //将对象信息更新到界面
                                Frm_BatteryFirstAlignTool.Instance.ckb_findLineToolEnable.Checked = L_toolList[i].enable;
                                Frm_BatteryFirstAlignTool.Instance.nud_minThreshold.Value = batteryFirstAlignTool.minThreshold;
                                Frm_BatteryFirstAlignTool.Instance.nud_maxThreshold.Value = batteryFirstAlignTool.maxThreshold;
                                Frm_BatteryFirstAlignTool.Instance.numericUpDown3.Value = batteryFirstAlignTool.dilationAndErosionSize;
                                Frm_BatteryFirstAlignTool.Instance.numericUpDown1.Value = Convert.ToDecimal(batteryFirstAlignTool.minArea);
                                Frm_BatteryFirstAlignTool.Instance.numericUpDown2.Value = Convert.ToDecimal(batteryFirstAlignTool.maxArea);
                                Frm_BatteryFirstAlignTool.Instance.cbx_edgeSelect.Text = "点" + batteryFirstAlignTool.pointIndex;
                                Frm_BatteryFirstAlignTool.Instance.comboBox1.Text = "边" + batteryFirstAlignTool.lineIndex;

                                //Frm_ShapeMatchTool.Instance.nud_angleStart.Value = Convert.ToDecimal(shapeMatchTool.startAngle);
                                //Frm_ShapeMatchTool.Instance.nud_angleRange.Value = Convert.ToDecimal(shapeMatchTool.angleRange);
                                //Frm_ShapeMatchTool.Instance.nud_angleStep.Value = Convert.ToDecimal(shapeMatchTool.angleStep);
                                //Frm_ShapeMatchTool.Instance.tkb_contrast.Value = Convert.ToInt16(shapeMatchTool.contrast);
                                //Frm_ShapeMatchTool.Instance.cbx_polarity.Text = shapeMatchTool.polarity;
                                //if (shapeMatchTool.angleStep == 0)
                                //{
                                //    Frm_ShapeMatchTool.Instance.nud_angleStep.Enabled = false;
                                //    Frm_ShapeMatchTool.Instance.ckb_angleStep.Checked = true;
                                //}
                                //else
                                //{
                                //    Frm_ShapeMatchTool.Instance.ckb_angleStep.Checked = false;
                                //}
                                break;
                            #endregion

                            #region BuChang
                            case ToolType.BuChang:
                                Frm_BuChangTool.Instance.pictureBox1.Image = Resources.UnknownTool;
                                Frm_BuChangTool.Instance.lbl_title.Text = string.Format("补偿    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_BuChangTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_BuChangTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_BuChangTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_BuChangTool.Instance.TopMost = true;
                                Frm_BuChangTool.Instance.Activate();
                                Frm_BuChangTool.Instance.jobName = this.jobName;
                                Frm_BuChangTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_BuChangTool.Instance.Show();
                                Frm_BuChangTool.Instance.WindowState = FormWindowState.Normal;
                                ////Frm_BuChangTool.Instance.btn_runDownCamAlignTool.Focus();
                                BuChangTool buChangTool = (BuChangTool)(L_toolList[i].tool);
                                Frm_BuChangTool.buChangTool = buChangTool;
                                Application.DoEvents();

                                Frm_BuChangTool.Instance.tbx_caputurePosX.Text = buChangTool.templatePos.Point.X.ToString();
                                Frm_BuChangTool.Instance.tbx_caputurePosY.Text = buChangTool.templatePos.Point.Y.ToString();
                                Frm_BuChangTool.Instance.tbx_caputurePosU.Text = buChangTool.templatePos.U.ToString();

                                Frm_BuChangTool.Instance.tbx_pickPosX.Text = buChangTool.workPos.Point.X.ToString();
                                Frm_BuChangTool.Instance.tbx_pickPosY.Text = buChangTool.workPos.Point.Y.ToString();
                                Frm_BuChangTool.Instance.tbx_pickPosU.Text = buChangTool.workPos.U.ToString();


                                Frm_BuChangTool.Instance.tbx_pickPosOffsetX.Text = buChangTool.buchang.Point.X.ToString();
                                Frm_BuChangTool.Instance.tbx_pickPosOffsetY.Text = buChangTool.buchang.Point.Y.ToString();
                                Frm_BuChangTool.Instance.tbx_pickPosOffsetU.Text = buChangTool.buchang.U.ToString();

                                Frm_BuChangTool.Instance.ckb_distancePLToolEnable.Checked = L_toolList[i].enable;
                                break;
                            #endregion

                            #region PointOffset
                            case ToolType.PointOffset:
                                Frm_PointOffsetTool.Instance.lbl_title.Text = string.Format("补偿    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_PointOffsetTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_PointOffsetTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_PointOffsetTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_PointOffsetTool.Instance.TopMost = true;
                                Frm_PointOffsetTool.Instance.Activate();
                                Frm_PointOffsetTool.Instance.jobName = this.jobName;
                                Frm_PointOffsetTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_PointOffsetTool.Instance.Show();
                                Frm_PointOffsetTool.Instance.WindowState = FormWindowState.Normal;
                                ////Frm_BuChangTool.Instance.btn_runDownCamAlignTool.Focus();
                                PointOffsetTool pointOffsetTool = (PointOffsetTool)(L_toolList[i].tool);
                                Frm_PointOffsetTool.pointOffsetTool = pointOffsetTool;
                                Application.DoEvents();

                                Frm_PointOffsetTool.Instance.comboBox1.Clear();

                                if (pointOffsetTool.toolPar.InputPar.点.ToString() == "System.Collections.Generic.List`1[VisionAndMotionPro.Point]")
                                {
                                    for (int j = 0; j < ((List<XY>)pointOffsetTool.toolPar.InputPar.点).Count; j++)
                                    {
                                        Frm_PointOffsetTool.Instance.comboBox1.Add((j + 1).ToString());
                                    }
                                }
                                else
                                {

                                }


                                Frm_PointOffsetTool.Instance.comboBox1.TextStr = pointOffsetTool.pointIdx.ToString();

                                Frm_PointOffsetTool.Instance.tbx_caputurePosX.Value = pointOffsetTool.templatePos.X.ToString();
                                Frm_PointOffsetTool.Instance.tbx_caputurePosY.Value = pointOffsetTool.templatePos.Y.ToString();

                                Frm_PointOffsetTool.Instance.tbx_pickPosX.Value = pointOffsetTool.workPos.X.ToString();
                                Frm_PointOffsetTool.Instance.tbx_pickPosY.Value = pointOffsetTool.workPos.Y.ToString();


                                Frm_PointOffsetTool.Instance.tbx_pickPosOffsetX.Value = pointOffsetTool.buchang.X;
                                Frm_PointOffsetTool.Instance.tbx_pickPosOffsetY.Value = pointOffsetTool.buchang.Y;

                                if (pointOffsetTool.toolPar.InputPar.点.ToString() == "")
                                {
                                    Frm_PointOffsetTool.Instance.tbx_inputPosX.Text = ((List<XY>)pointOffsetTool.toolPar.InputPar.点)[pointOffsetTool.pointIdx - 1].X.ToString();
                                    Frm_PointOffsetTool.Instance.tbx_inputPosY.Text = ((List<XY>)pointOffsetTool.toolPar.InputPar.点)[pointOffsetTool.pointIdx - 1].Y.ToString();
                                }
                                else
                                {

                                }

                                Frm_PointOffsetTool.Instance.tbx_resultPosX.Text = pointOffsetTool.toolPar.ResultPar.点.X.ToString();
                                Frm_PointOffsetTool.Instance.tbx_resultPosY.Text = pointOffsetTool.toolPar.ResultPar.点.Y.ToString();

                                Frm_PointOffsetTool.Instance.pictureBox8.Image = L_toolList[i].enable ? Resources.Enable : Resources.Disable;

                                if (pointOffsetTool.toolPar.InputPar.点.ToString() == "System.Collections.Generic.List`1[VisionAndMotionPro.Point]")
                                    Frm_PointOffsetTool.Instance.comboBox1.Visible = true;
                                else
                                    Frm_PointOffsetTool.Instance.comboBox1.Visible = false;
                                break;
                            #endregion

                            #region DisplayEdit
                            case ToolType.DisplayEdit:
                                Frm_DisplayEditTool.Instance.lbl_title.Text = string.Format("显示编辑    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //////Frm_ShapeMatchTool.Instance.StartPosition = FormStartPosition.Manual;
                                //////Frm_ShapeMatchTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_ShapeMatchTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_ShapeMatchTool.Instance.TopMost = true;
                                Frm_DisplayEditTool.Instance.Activate();
                                Frm_DisplayEditTool.Instance.jobName = this.jobName;
                                Frm_DisplayEditTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_DisplayEditTool.Instance.Show();
                                Frm_DisplayEditTool.Instance.WindowState = FormWindowState.Normal;
                                Frm_DisplayEditTool.Instance.btn_runTool.Focus();
                                DisplayEditTool displayEditTool = (DisplayEditTool)(L_toolList[i].tool);
                                Frm_DisplayEditTool.displayEditTool = displayEditTool;
                                Application.DoEvents();





                                //将对象信息更新到界面

                                break;
                            #endregion

                            #region EthernetReceive
                            case ToolType.EthernetReceive:
                                Frm_EthernetReceiveTool.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "SDK_PointGray" : string.Format("采集图像    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName));
                                ////// Frm_AcqImageTool.Instance.StartPosition = FormStartPosition.Manual;
                                ////// Frm_AcqImageTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width -  Frm_AcqImageTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                // Frm_AcqImageTool.Instance.TopMost = true;
                                Frm_EthernetReceiveTool.Instance.Activate();
                                Frm_EthernetReceiveTool.Instance.jobName = this.jobName;
                                Frm_EthernetReceiveTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_EthernetReceiveTool.ethernetReceiveTool = (EthernetReceiveTool)FindToolByName(L_toolList[i].toolName);
                                Frm_EthernetReceiveTool.Instance.Show();
                                Frm_EthernetReceiveTool.Instance.WindowState = FormWindowState.Normal;
                                EthernetReceiveTool ethernetReceiveTool = (EthernetReceiveTool)(L_toolList[i].tool);
                                Application.DoEvents();





                                //将对象信息更新到界面
                                Frm_EthernetReceiveTool.Instance.pic_onOff.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                Frm_EthernetReceiveTool.Instance.comboBox1222.Clear();
                                for (int j = 0; j < Project.Instance.L_TCPClient.Count; j++)
                                {
                                    Frm_EthernetReceiveTool.Instance.comboBox1222.Add(Project.Instance.L_TCPClient[j].Name);
                                }
                                for (int j = 0; j < Project.Instance.L_TCPSever.Count; j++)
                                {
                                    Frm_EthernetReceiveTool.Instance.comboBox1222.Add(Project.Instance.L_TCPSever[j].Name);
                                }
                                Frm_EthernetReceiveTool.Instance.comboBox1222.TextStr = ethernetReceiveTool.EthernetName;
                                Frm_EthernetReceiveTool.Instance.tbx_imageSavePath.TextStr = ethernetReceiveTool.trigCMD;
                                switch (ethernetReceiveTool.endChar)
                                {
                                    case "":
                                        Frm_EthernetReceiveTool.Instance.btn_endCharNone.BackColor = Color.Gray;
                                        Frm_EthernetReceiveTool.Instance.btn_endCharEnter.BackColor = Color.Gainsboro;
                                        break;
                                    case "\r\n":
                                        Frm_EthernetReceiveTool.Instance.btn_endCharNone.BackColor = Color.Gainsboro;
                                        Frm_EthernetReceiveTool.Instance.btn_endCharEnter.BackColor = Color.Gray;
                                        break;
                                }
                                break;
                            #endregion

                            #region EthernetSend
                            case ToolType.EthernetSend:
                                Frm_EthernetSendTool.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "SDK_PointGray" : string.Format("采集图像    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName));
                                ////// Frm_AcqImageTool.Instance.StartPosition = FormStartPosition.Manual;
                                ////// Frm_AcqImageTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width -  Frm_AcqImageTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                // Frm_AcqImageTool.Instance.TopMost = true;
                                Frm_EthernetSendTool.Instance.Activate();
                                Frm_EthernetSendTool.Instance.jobName = this.jobName;
                                Frm_EthernetSendTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_EthernetSendTool.ethernetReceiveTool = (EthernetSendTool)FindToolByName(L_toolList[i].toolName);
                                Frm_EthernetSendTool.Instance.Show();
                                Frm_EthernetSendTool.Instance.WindowState = FormWindowState.Normal;
                                EthernetSendTool ethernetSendTool = (EthernetSendTool)(L_toolList[i].tool);
                                Application.DoEvents();





                                //将对象信息更新到界面
                                Frm_EthernetSendTool.Instance.pic_onOff.Image = L_toolList[i].enable ? Resources.开 : Resources.关;
                                Frm_EthernetSendTool.Instance.comboBox1222.Clear();
                                for (int j = 0; j < Project.Instance.L_TCPClient.Count; j++)
                                {
                                    Frm_EthernetSendTool.Instance.comboBox1222.Add(Project.Instance.L_TCPClient[j].Name);
                                }
                                for (int j = 0; j < Project.Instance.L_TCPSever.Count; j++)
                                {
                                    Frm_EthernetSendTool.Instance.comboBox1222.Add(Project.Instance.L_TCPSever[j].Name);
                                }
                                Frm_EthernetSendTool.Instance.comboBox1222.TextStr = ethernetSendTool.EthernetName;
                                Frm_EthernetSendTool.Instance.tbx_imageSavePath.TextStr = ethernetSendTool.toolPar.InputPar.消息;
                                switch (ethernetSendTool.endChar)
                                {
                                    case "":
                                        Frm_EthernetSendTool.Instance.btn_endCharNone.BackColor = Color.Gray;
                                        Frm_EthernetSendTool.Instance.btn_endCharEnter.BackColor = Color.Gainsboro;
                                        break;
                                    case "\r\n":
                                        Frm_EthernetSendTool.Instance.btn_endCharNone.BackColor = Color.Gainsboro;
                                        Frm_EthernetSendTool.Instance.btn_endCharEnter.BackColor = Color.Gray;
                                        break;
                                }
                                break;
                            #endregion

                            #region PLCComm
                            case ToolType.PLCComm:
                                Frm_PLCCommTool.Instance.lbl_title.Text = string.Format("PLC通讯    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                Frm_PLCCommTool.Instance.Activate();
                                Frm_PLCCommTool.Instance.jobName = this.jobName;
                                Frm_PLCCommTool.Instance.toolName = L_toolList[i].toolName;
                                Frm_PLCCommTool.plcCommTool = (PLCCommTool)FindToolByName(L_toolList[i].toolName);
                                Frm_PLCCommTool.Instance.Show();
                                Frm_PLCCommTool.Instance.WindowState = FormWindowState.Normal;
                                Application.DoEvents();
                                Frm_PLCCommTool.Instance.LoadToolData();
                                break;
                            #endregion

                            #region Label
                            case ToolType.Label:
                                Frm_LabelTool.Instance.pictureBox1.Image = Resources.LabelTool;
                                Frm_LabelTool.Instance.lbl_title.Text = string.Format("数据显示    [ {0} . {1} ]", this.jobName, L_toolList[i].toolName);
                                //Frm_LabelTool.Instance.StartPosition = FormStartPosition.Manual;
                                //Frm_LabelTool.Instance.Location = new System.Drawing.Point(System.Windows.Forms.SystemInformation.VirtualScreen.Width - Frm_LabelTool.Instance.Width - 20, 200);        //让其显示在右上方，防止挡住图像窗口
                                //Frm_LabelTool.Instance.TopMost = true;
                                Frm_LabelTool.Instance.Activate();
                                Frm_LabelTool.Instance.jobName = this.jobName;
                                Frm_LabelTool.Instance.toolName = L_toolList[i].toolName; ;
                                Frm_LabelTool.Instance.Show();
                                Frm_LabelTool.Instance.WindowState = FormWindowState.Normal;
                                //Frm_LabelTool.Instance.btn_runTool.Focus();
                                LabelTool labelTool = (LabelTool)(L_toolList[i].tool);
                                Frm_LabelTool.labelTool = labelTool;
                                Application.DoEvents();

                                itemCount = ((LabelTool)L_toolList[i].tool).L_label.Count;
                                Frm_LabelTool.Instance.dgv_outputItem.Rows.Clear();
                                Frm_LabelTool.Instance.dgv_outputItem2.Rows.Clear();
                                for (int j = 0; j < itemCount; j++)
                                {
                                    if (((LabelTool)L_toolList[i].tool).L_label[j].ValueType == "Value")
                                    {
                                        int index = Frm_LabelTool.Instance.dgv_outputItem.Rows.Add();
                                        Frm_LabelTool.Instance.dgv_outputItem.Rows[index].Cells[0].Value = ((LabelTool)L_toolList[i].tool).L_label[j].OutputItem.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem.Rows[index].Cells[1].Value = ((LabelTool)L_toolList[i].tool).L_label[j].PreAddStr.ToString() == "" ? null : ((LabelTool)L_toolList[i].tool).L_label[j].PreAddStr.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem.Rows[index].Cells[2].Value = ((LabelTool)L_toolList[i].tool).L_label[j].Row.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem.Rows[index].Cells[3].Value = ((LabelTool)L_toolList[i].tool).L_label[j].Col.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem.Rows[index].Cells[4].Value = ((LabelTool)L_toolList[i].tool).L_label[j].DownLimit;
                                        Frm_LabelTool.Instance.dgv_outputItem.Rows[index].Cells[5].Value = ((LabelTool)L_toolList[i].tool).L_label[j].UpLimit;
                                        Frm_LabelTool.Instance.dgv_outputItem.Rows[index].Cells[6].Value = ((LabelTool)L_toolList[i].tool).L_label[j].Incolor.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem.Rows[index].Cells[7].Value = ((LabelTool)L_toolList[i].tool).L_label[j].OutColor.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem.Rows[index].Cells[8].Value = ((LabelTool)L_toolList[i].tool).L_label[j].Size.ToString();
                                    }
                                    else
                                    {
                                        int index = Frm_LabelTool.Instance.dgv_outputItem2.Rows.Add();
                                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[index].Cells[0].Value = ((LabelTool)L_toolList[i].tool).L_label[j].OutputItem.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[index].Cells[1].Value = ((LabelTool)L_toolList[i].tool).L_label[j].PreAddStr.ToString() == "" ? null : ((LabelTool)L_toolList[i].tool).L_label[j].PreAddStr.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[index].Cells[2].Value = ((LabelTool)L_toolList[i].tool).L_label[j].Row.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[index].Cells[3].Value = ((LabelTool)L_toolList[i].tool).L_label[j].Col.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[index].Cells[4].Value = ((LabelTool)L_toolList[i].tool).L_label[j].ExpectValue.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[index].Cells[5].Value = ((LabelTool)L_toolList[i].tool).L_label[j].Incolor.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[index].Cells[6].Value = ((LabelTool)L_toolList[i].tool).L_label[j].OutColor.ToString();
                                        Frm_LabelTool.Instance.dgv_outputItem2.Rows[index].Cells[7].Value = ((LabelTool)L_toolList[i].tool).L_label[j].Size.ToString();

                                    }
                                }

                                //Frm_LabelTool.Instance.ckb_toolEnable.Checked = ((ToolInfo)L_toolList[i]).enable;
                                break;
                            #endregion

                            #region Output
                            case ToolType.Output:
                                //Frm_OutputBoxTool.Instance.Text = "输出 - " + this.jobName + "." + L_toolList[i].toolName;
                                //Frm_OutputBoxTool.Instance.TopMost = true;
                                //Frm_OutputBoxTool.Instance.jobName = this.jobName;
                                //Frm_OutputBoxTool.Instance.toolName = L_toolList[i].toolName; ;
                                //Frm_OutputBoxTool.Instance.Show();
                                //Frm_OutputBoxTool.Instance.WindowState = FormWindowState.Normal;
                                //Frm_OutputBoxTool.Instance.b.Focus();
                                //OutputTool outputTool = (OutputTool)(L_toolList[i].tool);
                                //Application.DoEvents();

                                //将对象信息更新到界面
                                //Frm_OutputBoxTool.Instance.ckb_outputBoxToolNotRun.Checked = L_toolList[i].enable;
                                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "\r\nThis tool is a form-free tool" : "\r\n本工具为无窗体工具！");
                                break;
                                #endregion
                        }
                    }
                }
                Job.loadForm = false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        public object GetValue(object obj, string name)
        {
            PropertyInfo[] dd = obj.GetType().GetProperties();
            foreach (PropertyInfo pi in obj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                string temp = Regex.Split(pi.ToString(), " ")[0];
                //if (temp == "HalconDotNet.HObject"
                //    || temp == "VisionAndMotionPro.XYU"
                //    )
                //{
                if (pi.Name == name)
                {
                    return pi.GetValue(obj, null);
                }
            }
            return new object();
        }

        private object GetToolParValue(object obj, string name)
        {
            if (obj == null)
                return string.Empty;
            PropertyInfo pi = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (pi == null)
                return string.Empty;
            return pi.GetValue(obj, null);
        }

        private void SetToolParValue(object obj, string name, object value)
        {
            if (obj == null)
                return;
            PropertyInfo pi = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (pi == null || !pi.CanWrite)
                return;
            pi.SetValue(obj, value == null ? string.Empty : value.ToString(), null);
        }

        private object GetSourceValue(string sourceFrom)
        {
            if (string.IsNullOrEmpty(sourceFrom))
                return string.Empty;

            string[] parts = Regex.Split(sourceFrom, "->");
            if (parts.Length < 2)
                return string.Empty;

            string sourceToolName = parts[0].Trim();
            if (sourceToolName.StartsWith("《-"))
                sourceToolName = sourceToolName.Substring(3).Trim();
            string toolItem = parts[1].Trim();

            if (sourceToolName == "全局变量" || sourceToolName == "Global")
                return Project.Instance.curEngine.globelVariable.GetGlobalVariableValue(toolItem);

            string sourceJobName = jobName;
            Match match = Regex.Match(sourceToolName, @"^\[(?<job>.+?)\](?<tool>.+)$");
            if (match.Success)
            {
                sourceJobName = match.Groups["job"].Value;
                sourceToolName = match.Groups["tool"].Value;
            }

            Job sourceJob = Project.Instance.curEngine.FindJobByName(sourceJobName);
            if (sourceJob == null)
                return string.Empty;
            ToolInfo sourceTool = sourceJob.FindToolInfoByName(sourceToolName);
            if (sourceTool == null)
                return string.Empty;
            return sourceTool.GetOutput(toolItem).value;
        }

        /// <summary>
        /// 是否启用事件，也就是不执行本次触发的事件
        /// </summary>
        internal static bool loadForm = true;
        /// <summary>
        /// 为防止datagridview出现大红叉，此处申明委托使用创建控件的线程调用控件
        /// </summary>
        public delegate void ShowTestData();
        /// <summary>
        /// 运行流程
        /// </summary>
        /// <param name="initRun">若为程序启动时的第一次运行，相关通讯工具不被执行，通常情况下此参数传True值即可</param>
        /// <param name="runToToolIndex">若大于等于0，则只从第一个工具运行到该索引对应的工具为止（用于"运行到当前工具"）</param>
        /// <returns></returns>
        public List<object> Run(bool initRun = false, int runToToolIndex = -1)
        {
            Interlocked.Increment(ref activeRunCount);
            try
            {


                //此处使用委托调用，防止datagridview控件出现大红叉
                ShowTestData showTestData = delegate ()
                {
                    GetJobTree().ShowNodeToolTips = true;
                };
                if (Frm_Job.Instance.tbc_jobs.IsHandleCreated)
                {
                    if (Frm_Job.Instance.tbc_jobs.InvokeRequired)
                        Frm_Job.Instance.tbc_jobs.BeginInvoke(showTestData);
                    else
                        showTestData();
                }






                //GetJobTree().ShowNodeToolTips = true;
                Stopwatch jobElapsedTime = new Stopwatch();
                jobElapsedTime.Restart();
                recordElapseTime = 0;

                //开始逐个执行各工具
                jobRunStatu = JobRunStatu.Succeed;
                List<object> L_result = new List<object>();
                int toolIndex = -1;
                // 同一轮流程的多个斑点工具共享主图像窗口：第一个负责清除旧图层，
                // 后续工具在同一背景上叠加，避免前一个结果被清掉或显示属性相互串扰。
                bool blobMainImagePrepared = false;
                Application.DoEvents();
                for (int i = 0; i < L_toolList.Count && (runToToolIndex < 0 || i <= runToToolIndex); i++)
                {
                    if (IsStopRequested)
                    {
                        jobRunStatu = JobRunStatu.Fail;
                        Frm_Main.Instance.OutputMsg(string.Format("流程 [{0}] 已停止", jobName), Color.DarkOrange);
                        break;
                    }
                    toolIndex++;
                    TreeNode treeNode = GetToolNodeByNodeText(L_toolList[i].toolName);
                    inputItemNum = (L_toolList[i]).input.Count;
                    outputItemNum = (L_toolList[i]).output.Count;
                    bool sourceValueIsEmpty = false;      //此变量判断输入源值是否为空，若为空就终止流程执行

                    if (!L_toolList[i].enable && !Configuration.SpeedMode)
                    {
                        string disabledTip = Project.Instance.configuration.language == Language.English ?
                            string.Format("Tool [{0}] is disabled, skipped", L_toolList[i].toolName) :
                            string.Format("工具 [{0}] 已禁用，已跳过", L_toolList[i].toolName);
                        Frm_Main.Instance.OutputMsg(disabledTip, Color.DarkGray);
                    }

                    #region ImageAcq
                    if (L_toolList[i].toolType == ToolType.ImageAcq)
                    {
                        AcqImageTool SDK_hikVisionTool = (AcqImageTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            SDK_hikVisionTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = SDK_hikVisionTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }

                        SDK_hikVisionTool.Run(true, false, L_toolList[i].toolName);

                        if (SDK_hikVisionTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", SDK_hikVisionTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((AcqImageTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region ImagePreprocessing
                    if (L_toolList[i].toolType == ToolType.ImagePreprocessing)
                    {
                        ImageProprecessingTool imageProprecessingTool = (ImageProprecessingTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            imageProprecessingTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = imageProprecessingTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                imageProprecessingTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                                treeNode.ToolTipText = imageProprecessingTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, imageProprecessingTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "输入图像" || inputItemName == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                imageProprecessingTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItemName + sourceFrom).ToolTipText = FormatShowTip(imageProprecessingTool.inputImage);
                                if (imageProprecessingTool.inputImage == null)
                                {
                                    imageProprecessingTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                    treeNode.ToolTipText = imageProprecessingTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        imageProprecessingTool.Run(true, false, L_toolList[i].toolName);

                        if (imageProprecessingTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", imageProprecessingTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            Result_ImageProcessing result3 = ((ImageProprecessingTool)(L_toolList[i].tool)).result;
                            object value = result3;
                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }

                    }
                    #endregion

                    #region ColorToRGB
                    else if (L_toolList[i].toolType == ToolType.ColorToRGB)
                    {
                        ColorToRGBTool colorToRGBTool = (ColorToRGBTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            colorToRGBTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = colorToRGBTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        colorToRGBTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                colorToRGBTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                                treeNode.ToolTipText = colorToRGBTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, colorToRGBTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "输入图像" || inputItemName == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                colorToRGBTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItemName + sourceFrom).ToolTipText = FormatShowTip(colorToRGBTool.inputImage);
                                if (colorToRGBTool.inputImage == null)
                                {
                                    colorToRGBTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                    treeNode.ToolTipText = colorToRGBTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        colorToRGBTool.Run(false, false, L_toolList[i].toolName);
                        if (colorToRGBTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", colorToRGBTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "红":
                                case "Red":
                                    L_toolList[i].GetOutput(outputItem).value = colorToRGBTool.outputRed;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "图形变量暂不支持显示";
                                    break; ;
                                case "绿":
                                case "Green":
                                    L_toolList[i].GetOutput(outputItem).value = colorToRGBTool.outputGreen;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "图形变量暂不支持显示";
                                    break; ;
                                case "蓝":
                                case "Blue":
                                    L_toolList[i].GetOutput(outputItem).value = colorToRGBTool.outputBlue;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "图形变量暂不支持显示";
                                    break; ;
                            }
                        }


                    }
                    #endregion

                    #region SaveImage
                    else if (L_toolList[i].toolType == ToolType.SaveImage)
                    {
                        SaveImageTool saveImageTool = (SaveImageTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            saveImageTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = saveImageTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        //////saveImageTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                saveImageTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                                treeNode.ToolTipText = saveImageTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, saveImageTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "图像" || inputItemName == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                saveImageTool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItemName + sourceFrom).ToolTipText = FormatShowTip(saveImageTool.toolPar.InputPar.图像);
                                if (saveImageTool.toolPar.InputPar.图像 == null)
                                {
                                    saveImageTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                    treeNode.ToolTipText = saveImageTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        saveImageTool.Run(false, false, L_toolList[i].toolName);
                        if (saveImageTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", saveImageTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                            }
                        }


                    }
                    #endregion

                    #region ShapeMatch
                    else if (L_toolList[i].toolType == ToolType.Match)
                    {
                        MatchTool shapeMatchTool = (MatchTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            shapeMatchTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = shapeMatchTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        shapeMatchTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                shapeMatchTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                                treeNode.ToolTipText = shapeMatchTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, shapeMatchTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "图像" || inputItemName == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                shapeMatchTool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                string temp = FormatShowTip(shapeMatchTool.toolPar.InputPar.图像);
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItemName + sourceFrom).ToolTipText = FormatShowTip(shapeMatchTool.toolPar.InputPar.图像);
                                if (shapeMatchTool.toolPar.InputPar.图像 == null)
                                {
                                    shapeMatchTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                    treeNode.ToolTipText = shapeMatchTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        shapeMatchTool.Run(false, false, L_toolList[i].toolName);


                        if (shapeMatchTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", shapeMatchTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((MatchTool)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XYU]")
                                {
                                    List<XYU> positions = (List<XYU>)value;
                                    if (positions.Count == 0)
                                    {
                                        value = null;
                                        break;
                                    }
                                    value = GetValue(positions[0], strs[k]);
                                }
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }
                    }
                    #endregion

                    #region EyeHandCalibration
                    else if (L_toolList[i].toolType == ToolType.EyeHandCalib)
                    {
                        EyeHandCalibTool eyeHandCalibTool = (EyeHandCalibTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((EyeHandCalibTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = eyeHandCalibTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }

                        //首先把所有的输入都清空
                        eyeHandCalibTool.ClearLastInput();


                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((EyeHandCalibTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                treeNode.ToolTipText = eyeHandCalibTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, eyeHandCalibTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "图像" || inputItem == "InputImage")
                            {





                                string jobName1 = string.Empty;
                                if (sourceFrom.Contains("["))
                                {
                                    jobName1 = sourceFrom.Split(new char[] { '[' })[1];
                                    jobName1 = jobName1.Split(new char[] { ']' })[0];
                                }
                                else
                                {
                                    jobName1 = this.jobName;
                                }
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                eyeHandCalibTool.toolPar.InputPar.图像 = FindJobByName(jobName1).FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;





                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(eyeHandCalibTool.toolPar.InputPar.图像);
                                //string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                //sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                //string toolItem = Regex.Split(sourceFrom, "->")[1];
                                //eyeHandCalibTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                if (eyeHandCalibTool.toolPar.InputPar.图像 == null)
                                {
                                    eyeHandCalibTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_Image : ToolRunStatu.无输入图像);
                                    treeNode.ToolTipText = eyeHandCalibTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItem == "点" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                eyeHandCalibTool.toolPar.InputPar.点 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XY>;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(eyeHandCalibTool.toolPar.InputPar.点);
                                if (eyeHandCalibTool.toolPar.InputPar.点 == null)
                                {
                                    eyeHandCalibTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_String : ToolRunStatu.无输入字符串);
                                    treeNode.ToolTipText = eyeHandCalibTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItem == "位置" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                eyeHandCalibTool.toolPar.InputPar.位置 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(eyeHandCalibTool.toolPar.InputPar.位置);
                                if (eyeHandCalibTool.toolPar.InputPar.位置 == null)
                                {
                                    eyeHandCalibTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_String : ToolRunStatu.无输入字符串);
                                    treeNode.ToolTipText = eyeHandCalibTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;



                        eyeHandCalibTool.Run(true, true, L_toolList[i].toolName);
                        if (eyeHandCalibTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", eyeHandCalibTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        //for (int j = 0; j < this.outputItemNum; j++)
                        //{
                        //    string outputItem = L_toolList[i].output[j].IOName;
                        //    switch (outputItem)
                        //    {
                        //        case "输出图像":
                        //        case "OutputImage":
                        //            L_toolList[i].GetOutput(outputItem).value = eyeHandCalibTool.outputImage;
                        //            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "图形变量不支持显示";
                        //            break;
                        //        case "输出点":
                        //        case "ResultxxxxxStr":
                        //            L_toolList[i].GetOutput(outputItem).value = eyeHandCalibTool.outputPoint;
                        //            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(eyeHandCalibTool.outputPoint);
                        //            break;
                        //        case "输出位置":
                        //        case "ResultxxStr":
                        //            L_toolList[i].GetOutput(outputItem).value = eyeHandCalibTool.outputXYU;
                        //            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(eyeHandCalibTool.outputXYU);
                        //            break;
                        //        case "输出格式位置":
                        //        case "Resultx信息xStr":
                        //            L_toolList[i].GetOutput(outputItem).value = eyeHandCalibTool.outputXYU;
                        //            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(eyeHandCalibTool.outputXYU);
                        //            break;
                        //    }
                        //}


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((EyeHandCalibTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XYU]")
                                {
                                    List<XYU> positions = (List<XYU>)value;
                                    if (positions.Count == 0)
                                    {
                                        value = null;
                                        break;
                                    }
                                    value = GetValue(positions[0], strs[k]);
                                }
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }

                    }
                    #endregion

                    #region ApplyTrans
                    else if (L_toolList[i].toolType == ToolType.QuoteTrans)
                    {
                        QuoteTransTool applyTransTool = (QuoteTransTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((QuoteTransTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = applyTransTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }

                        //首先把所有的输入都清空
                        //eyeHandCalibTool.ClearLastInput();


                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((EyeHandCalibTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                treeNode.ToolTipText = applyTransTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, applyTransTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            //////if (inputItem == "输入图像" || inputItem == "InputImage")
                            //////{





                            //////    string jobName1 = string.Empty;
                            //////    if (sourceFrom.Contains("["))
                            //////    {
                            //////        jobName1 = sourceFrom.Split(new char[] { '[' })[1];
                            //////        jobName1 = jobName1.Split(new char[] { ']' })[0];
                            //////    }
                            //////    else
                            //////    {
                            //////        jobName1 = this.jobName;
                            //////    }
                            //////    sourceFrom = sourceFrom.Split(new char[] { ']' })[1];
                            //////    string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                            //////    string toolItem = Regex.Split(sourceFrom, "->")[1];
                            //////    applyTransTool.inputImage = FindJobByName(jobName1).FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;






                            //////    //string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                            //////    //sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                            //////    //string toolItem = Regex.Split(sourceFrom, "->")[1];
                            //////    //eyeHandCalibTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                            //////    if (applyTransTool.inputImage == null)
                            //////    {
                            //////        applyTransTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_Image : ToolRunStatu.无输入图像);
                            //////        treeNode.ToolTipText = applyTransTool.toolRunStatu.ToString();
                            //////        treeNode.ForeColor = Color.Red;
                            //////        sourceValueIsEmpty = true;
                            //////        break;
                            //////    }
                            //////}
                            if (inputItem == "点" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                applyTransTool.toolPar.InputPar.点 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XY>;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(applyTransTool.toolPar.InputPar.点);
                                if (applyTransTool.toolPar.InputPar.点 == null)
                                {
                                    applyTransTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_String : ToolRunStatu.无输入字符串);
                                    treeNode.ToolTipText = applyTransTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItem == "位置" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                applyTransTool.toolPar.InputPar.位置 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(applyTransTool.toolPar.InputPar.位置);
                                if (applyTransTool.toolPar.InputPar.位置 == null)
                                {
                                    applyTransTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_String : ToolRunStatu.无输入字符串);
                                    treeNode.ToolTipText = applyTransTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;



                        applyTransTool.Run(true, true, L_toolList[i].toolName);
                        if (applyTransTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", applyTransTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }



                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((QuoteTransTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XY]")
                                    value = GetValue(((List<XY>)value)[0], strs[k]);
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }








                    }
                    #endregion

                    #region OneKeyEyeHandCalibration
                    else if (L_toolList[i].toolType == ToolType.OneKeyEyeHandCalib)
                    {
                        OneKeyEyeHandCalibTool oneKeyEyeHandCalibrationTool = (OneKeyEyeHandCalibTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((OneKeyEyeHandCalibTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = oneKeyEyeHandCalibrationTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }

                        //首先把所有的输入都清空
                        oneKeyEyeHandCalibrationTool.inputImage = null;
                        oneKeyEyeHandCalibrationTool.inputPose = null;

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((EyeHandCalibTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                treeNode.ToolTipText = oneKeyEyeHandCalibrationTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, oneKeyEyeHandCalibrationTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "输入图像" || inputItem == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                oneKeyEyeHandCalibrationTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(oneKeyEyeHandCalibrationTool.inputImage);
                                if (oneKeyEyeHandCalibrationTool.inputImage == null)
                                {
                                    oneKeyEyeHandCalibrationTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_Image : ToolRunStatu.无输入图像);
                                    treeNode.ToolTipText = oneKeyEyeHandCalibrationTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItem == "输入点" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                oneKeyEyeHandCalibrationTool.inputPose = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(oneKeyEyeHandCalibrationTool.inputPose);
                                if (oneKeyEyeHandCalibrationTool.inputPose == null)
                                {
                                    oneKeyEyeHandCalibrationTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_String : ToolRunStatu.无输入字符串);
                                    treeNode.ToolTipText = oneKeyEyeHandCalibrationTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItem == "输入位置" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                oneKeyEyeHandCalibrationTool.inputPose = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(oneKeyEyeHandCalibrationTool.inputPose);
                                if (oneKeyEyeHandCalibrationTool.inputPose == null)
                                {
                                    oneKeyEyeHandCalibrationTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_String : ToolRunStatu.无输入字符串);
                                    treeNode.ToolTipText = oneKeyEyeHandCalibrationTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;



                        oneKeyEyeHandCalibrationTool.Run(true, true, L_toolList[i].toolName);

                        if (oneKeyEyeHandCalibrationTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", oneKeyEyeHandCalibrationTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "输出图像":
                                case "OutputImage":
                                    L_toolList[i].GetOutput(outputItem).value = oneKeyEyeHandCalibrationTool.outputImage;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "图形变量不支持显示";
                                    break;
                                case "输出位置":
                                case "ResultxxStr":
                                    L_toolList[i].GetOutput(outputItem).value = oneKeyEyeHandCalibrationTool.outputPose;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = oneKeyEyeHandCalibrationTool.outputPose.ToShowTip();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region OneDimensionalCalib
                    else if (L_toolList[i].toolType == ToolType.OneDimensionalCalib)
                    {
                        OneDimensionalCalibTool oneDimensionalCalibrationTool = (OneDimensionalCalibTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((OneDimensionalCalibTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = oneDimensionalCalibrationTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }

                        //首先把所有的输入都清空

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((EyeHandCalibTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                treeNode.ToolTipText = oneDimensionalCalibrationTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, oneDimensionalCalibrationTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "输入值" || inputItem == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                oneDimensionalCalibrationTool.inputValue = Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value);
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(oneDimensionalCalibrationTool.inputValue);
                                if (oneDimensionalCalibrationTool.inputValue == null)
                                {
                                    oneDimensionalCalibrationTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_Image : ToolRunStatu.无输入图像);
                                    treeNode.ToolTipText = oneDimensionalCalibrationTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;

                        oneDimensionalCalibrationTool.Run(true, true, L_toolList[i].toolName);
                        if (oneDimensionalCalibrationTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", oneDimensionalCalibrationTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "输出值":
                                case "OutputImage":
                                    L_toolList[i].GetOutput(outputItem).value = oneDimensionalCalibrationTool.OutputValue;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = oneDimensionalCalibrationTool.OutputValue.ToString();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region UpCamAlign
                    else if (L_toolList[i].toolType == ToolType.UpCamAlign)
                    {
                        UpCamAlignTool upCamAlignTool = (UpCamAlignTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            upCamAlignTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = upCamAlignTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        upCamAlignTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                upCamAlignTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Pos;
                                treeNode.ToolTipText = upCamAlignTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, upCamAlignTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "位置" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                upCamAlignTool.toolPar.InputPar.位置 = (FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>)[0];
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(upCamAlignTool.toolPar.InputPar.位置);
                            }
                        }

                        //   upCamAlignTool.toolIdx = this.toolIdx;
                        upCamAlignTool.Run(true, true, L_toolList[i].toolName);
                        if (upCamAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", upCamAlignTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((UpCamAlignTool)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XYU]")
                                {
                                    List<XYU> positions = (List<XYU>)value;
                                    if (positions.Count == 0)
                                    {
                                        value = null;
                                        break;
                                    }
                                    value = GetValue(positions[0], strs[k]);
                                }
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region DownCamAlign
                    else if (L_toolList[i].toolType == ToolType.DownCamAlign)
                    {
                        DownCamAlignTool robotDownCamAlignTool = (DownCamAlignTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            robotDownCamAlignTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = robotDownCamAlignTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                robotDownCamAlignTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Pos;
                                treeNode.ToolTipText = robotDownCamAlignTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, robotDownCamAlignTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "输入位置" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                robotDownCamAlignTool.inputPos = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(robotDownCamAlignTool.inputPos);
                                if (robotDownCamAlignTool.inputPos == null)
                                {
                                    robotDownCamAlignTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.No_Input_String : ToolRunStatu.无输入字符串);
                                    treeNode.ToolTipText = robotDownCamAlignTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItem == "输入点" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                robotDownCamAlignTool.inputPos = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(robotDownCamAlignTool.inputPos);
                            }
                        }
                        robotDownCamAlignTool.Run(true, true, L_toolList[i].toolName);
                        if (robotDownCamAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", robotDownCamAlignTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "输出位置":
                                case "OutputAllAddStr":
                                    L_toolList[i].GetOutput(outputItem).value = robotDownCamAlignTool.resultPos.Point.X + "," + robotDownCamAlignTool.resultPos.Point.Y + "," + robotDownCamAlignTool.resultPos.U;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = robotDownCamAlignTool.resultPos.Point.X + "," + robotDownCamAlignTool.resultPos.Point.Y + "," + robotDownCamAlignTool.resultPos.U;
                                    break;
                                case "格式点":
                                case "OutputAllAddxStr":
                                    L_toolList[i].GetOutput(outputItem).value = robotDownCamAlignTool.resultPos.ToFormatStr();
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = robotDownCamAlignTool.resultPos.ToShowTip();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region RotatePlatform
                    else if (L_toolList[i].toolType == ToolType.RotatePlatform)
                    {
                        RotatePlatformTool rotatePlatformTool = (RotatePlatformTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            rotatePlatformTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = rotatePlatformTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        rotatePlatformTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                rotatePlatformTool.toolRunStatu = ToolRunStatu.Not_Asign_Input_Source;
                                treeNode.ToolTipText = rotatePlatformTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "InputImage" || inputItem == "输入位置")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0]; ;
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                rotatePlatformTool.inputPos = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(rotatePlatformTool.inputPos);
                            }
                        }
                        rotatePlatformTool.Run(false, true, L_toolList[i].toolName);
                        if (rotatePlatformTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, rotatePlatformTool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", rotatePlatformTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < outputItemNum; j++)
                        {
                            string outputItemName = L_toolList[i].output[j].IOName;
                            switch (outputItemName)
                            {
                                case "输出位置":
                                case "ResultLineStartX":
                                    L_toolList[i].GetOutput(outputItemName).value = rotatePlatformTool.outputPos;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItemName).ToolTipText = rotatePlatformTool.outputPos.ToShowTip();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region XYPlatform
                    else if (L_toolList[i].toolType == ToolType.XYPlatform)
                    {
                        XYPlatformTool xyPlatformTool = (XYPlatformTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            xyPlatformTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = xyPlatformTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        //////xyPlatformTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                xyPlatformTool.toolRunStatu = ToolRunStatu.Not_Asign_Input_Source;
                                treeNode.ToolTipText = xyPlatformTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "InputImage" || inputItem == "输入位置")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0]; ;
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                xyPlatformTool.inputPos = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(xyPlatformTool.inputPos);
                            }
                        }
                        xyPlatformTool.Run(false, true, L_toolList[i].toolName);
                        if (xyPlatformTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, xyPlatformTool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", xyPlatformTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < outputItemNum; j++)
                        {
                            string outputItemName = L_toolList[i].output[j].IOName;
                            switch (outputItemName)
                            {
                                case "输出位置":
                                case "ResultLineStartX":
                                    L_toolList[i].GetOutput(outputItemName).value = xyPlatformTool.outputPos;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItemName).ToolTipText = xyPlatformTool.outputPos.ToShowTip();
                                    break;
                                case "格式位置":
                                case "OutputAllAddxStr":
                                    L_toolList[i].GetOutput(outputItemName).value = xyPlatformTool.outputPos.ToFormatStr();
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItemName).ToolTipText = xyPlatformTool.outputPos.ToShowTip();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region 点位引导
                    else if (L_toolList[i].toolType == ToolType.PointAlign)
                    {
                        PointAlignTool pointAlignTool = (PointAlignTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            pointAlignTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = pointAlignTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        //////xyPlatformTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                pointAlignTool.toolRunStatu = ToolRunStatu.Not_Asign_Input_Source;
                                treeNode.ToolTipText = pointAlignTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "InputImage" || inputItem == "位置")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0]; ;
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                pointAlignTool.inputPos = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(pointAlignTool.inputPos);
                            }
                        }
                        pointAlignTool.Run(false, true, L_toolList[i].toolName);
                        if (pointAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, pointAlignTool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", pointAlignTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < outputItemNum; j++)
                        {
                            string outputItemName = L_toolList[i].output[j].IOName;
                            switch (outputItemName)
                            {
                                case "位置":
                                case "ResultLineStartX":
                                    L_toolList[i].GetOutput(outputItemName).value = pointAlignTool.resultPos;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItemName).ToolTipText = pointAlignTool.resultPos.ToShowTip();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region AlignFit
                    else if (L_toolList[i].toolType == ToolType.AlignFit)
                    {
                        AlignFitTool alignFitTool = (AlignFitTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            alignFitTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = alignFitTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        alignFitTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                alignFitTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Pos;
                                treeNode.ToolTipText = alignFitTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, alignFitTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "元件位置" || inputItem == "InputStr")
                            {
                                string jobName = string.Empty;
                                if (sourceFrom.Contains("["))
                                {
                                    jobName = sourceFrom.Split(new char[] { '[' })[1];
                                    jobName = jobName.Split(new char[] { ']' })[0];
                                }
                                else
                                {
                                    jobName = this.jobName;
                                }
                                sourceFrom = sourceFrom.Split(new char[] { ']' })[1];
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                alignFitTool.inputPos = (FindJobByName(jobName).FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>)[0];
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(alignFitTool.inputPos);
                            }
                            else if (inputItem == "基板位置" || inputItem == "InputStr")
                            {
                                string jobName = string.Empty;
                                if (sourceFrom.Contains("["))
                                {
                                    jobName = sourceFrom.Split(new char[] { '[' })[1];
                                    jobName = jobName.Split(new char[] { ']' })[0];
                                }
                                else
                                {
                                    jobName = this.jobName;
                                }
                                sourceFrom = sourceFrom.Split(new char[] { ']' })[1];
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                alignFitTool.inputPos = (FindJobByName(jobName).FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>)[0];
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(alignFitTool.inputPos);
                            }
                        }

                        alignFitTool.toolIdx = this.toolIdx;
                        alignFitTool.Run(true, true, L_toolList[i].toolName);
                        if (alignFitTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", alignFitTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((AlignFitTool)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XYU]")
                                {
                                    List<XYU> positions = (List<XYU>)value;
                                    if (positions.Count == 0)
                                    {
                                        value = null;
                                        break;
                                    }
                                    value = GetValue(positions[0], strs[k]);
                                }
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region AlignWithoutCalibRotateCenter
                    else if (L_toolList[i].toolType == ToolType.AlignWithoutCalibRotateCenter)
                    {
                        AlignWithoutCalibRotateCenterTool alignWithoutCalibRotateCenterTool = (AlignWithoutCalibRotateCenterTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            alignWithoutCalibRotateCenterTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = alignWithoutCalibRotateCenterTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        alignWithoutCalibRotateCenterTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                alignWithoutCalibRotateCenterTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Pos;
                                treeNode.ToolTipText = alignWithoutCalibRotateCenterTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, alignWithoutCalibRotateCenterTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "位置" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                alignWithoutCalibRotateCenterTool.inputPos = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(alignWithoutCalibRotateCenterTool.inputPos);
                            }
                        }

                        //////alignWithoutCalibRotateCenterTool.toolIdx = this.toolIdx;
                        alignWithoutCalibRotateCenterTool.Run(true, true, L_toolList[i].toolName);
                        if (alignWithoutCalibRotateCenterTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", alignWithoutCalibRotateCenterTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "位置":
                                case "OutputAllAddStr":
                                    L_toolList[i].GetOutput(outputItem).value = alignWithoutCalibRotateCenterTool.resultPos;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = alignWithoutCalibRotateCenterTool.resultPos.ToShowTip();
                                    break;
                                case "格式位置":
                                case "OutputAllAddxStr":
                                    //////L_toolList[i].GetOutput(outputItem).value = alignWithoutCalibRotateCenterTool.resultPos.ToFormatStr();
                                    //////GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = alignWithoutCalibRotateCenterTool.resultPos.ToShowTip();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region FindLine
                    else if (L_toolList[i].toolType == ToolType.FindLine)
                    {
                        FindLineTool findLineTool = (FindLineTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            findLineTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = findLineTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        findLineTool.ClearLastInput();
                        findLineTool.toolPar.InputPar.跟随 = null;

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                // 跟随可选；未连接时按工具内保存的固定 ROI 查线。
                                if (inputItem == "Pose" || inputItem == "跟随")
                                    continue;
                                findLineTool.toolRunStatu = ToolRunStatu.输入项未链接源;
                                treeNode.ToolTipText = findLineTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "InputImage" || inputItem == "图像")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0]; ;
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                findLineTool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItem + sourceFrom).ToolTipText = FormatShowTip(findLineTool.toolPar.InputPar.图像);


                            }
                            else if (inputItem == "Pose" || inputItem == "跟随")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0]; ;
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                findLineTool.toolPar.InputPar.跟随 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>;
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItem + sourceFrom).ToolTipText = FormatShowTip(findLineTool.toolPar.InputPar.跟随);
                            }
                        }
                        findLineTool.Run(false, false, L_toolList[i].toolName);
                        if (findLineTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, findLineTool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", findLineTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((FindLineTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");

                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region FindCricle
                    else if (L_toolList[i].toolType == ToolType.FindCircle)
                    {
                        FindCircleTool findCircleTool = (FindCircleTool)L_toolList[i].tool;
                        // 流程运行到"查找圆"节点时进入这里。
                        // 这里负责把输入连接解析为 FindCircleTool.toolPar.InputPar，
                        // 然后调用 Execute()，最后把 ResultPar 中的输出写回流程输出节点。
                        if (!L_toolList[i].enable)
                        {
                            findCircleTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = findCircleTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        // 跟随可选；先清除上一轮残留，避免断开连线后仍使用旧位姿。
                        findCircleTool.toolPar.InputPar.跟随 = null;
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                if (inputItemName == "跟随" || inputItemName == "Pose")
                                    continue;
                                findCircleTool.toolRunStatu = ToolRunStatu.输入项未链接源;
                                treeNode.ToolTipText = findCircleTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, findCircleTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "图像")
                            {
                                // 解析图像输入连接，把上游 HObject 写入圆查找输入。
                                //string sourceToolName = Regex.Split(sourceFrom, " , ")[0];
                                //sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, " . ")[0].Length - 3); ;
                                //string toolItem = Regex.Split(sourceFrom, " . ")[1];
                                //shapeMatchTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                //if (shapeMatchTool.inputImage == null)
                                //{
                                //    shapeMatchTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                //    treeNode.ToolTipText = shapeMatchTool.toolRunStatu.ToString();
                                //    treeNode.ForeColor = Color.Red;
                                //    sourceValueIsEmpty = true;
                                //    break;
                                //}



                                string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                findCircleTool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItemName + sourceFrom).ToolTipText = FormatShowTip(findCircleTool.toolPar.InputPar.图像);
                            }
                            else if (inputItemName == "跟随" || inputItemName == "Pose")
                            {
                                // 解析跟随输入连接。没有跟随时工具按固定 ROI 找圆。
                                string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                findCircleTool.toolPar.InputPar.跟随 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>;
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItemName + sourceFrom).ToolTipText = FormatShowTip(findCircleTool.toolPar.InputPar.跟随);
                            }
                            else if (inputItemName == "预期圆中心行" || inputItemName == "ExpectCircleCenterX")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                findCircleTool.expectCircleRow = (HTuple)Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value);
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItemName + sourceFrom).ToolTipText = FormatShowTip(findCircleTool.expectCircleRow);
                            }
                            else if (inputItemName == "预期圆中心列" || inputItemName == "ExpectCircleCenterY")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                findCircleTool.expectCircleCol = (HTuple)Convert.ToDouble((HObject)FindToolInfoByName(sourceToolName).GetOutput(toolItem).value);
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItemName + sourceFrom).ToolTipText = FormatShowTip(findCircleTool.expectCircleCol);
                            }
                            if (inputItemName == "预期圆半径" || inputItemName == "ExpectCircleRadius")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                findCircleTool.expectCircleRadius = (HTuple)Convert.ToDouble((HObject)FindToolInfoByName(sourceToolName).GetOutput(toolItem).value);
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItemName + sourceFrom).ToolTipText = FormatShowTip(findCircleTool.expectCircleRadius);
                            }
                        }

                        ToolRunResult findCircleResult = findCircleTool.Execute(new ToolRunContext
                        {
                            JobName = jobName,
                            ToolName = L_toolList[i].toolName,
                            TimeoutMs = 3000,
                            IsCancellationRequested = null
                        });

                        if (!findCircleResult.Success)
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, findCircleResult.Status.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", findCircleResult.Status.ToString(), findCircleResult.ElapsedMs, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            // 把 FindCircleTool.ToolPar.ResultPar 里的"圆心/是否找到圆/结果圆/圆半径"
                            // 按工具输出节点名称反射取值，写给下游工具使用。
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((FindCircleTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");

                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region BlobAnalyse
                    else if (L_toolList[i].toolType == ToolType.BlobAnalyse)
                    {
                        BlobAnalyseTool blobAnalyseTool = (BlobAnalyseTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((BlobAnalyseTool)(L_toolList[i].tool)).toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = blobAnalyseTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                // 跟随是可选输入；未连接时按固定搜索区域运行。
                                if (inputItem == "跟随" || inputItem == "Pose")
                                {
                                    blobAnalyseTool.toolPar.InputPar.跟随 = null;
                                    continue;
                                }
                                ((BlobAnalyseTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                treeNode.ToolTipText = blobAnalyseTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, blobAnalyseTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "图像" || inputItem == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                blobAnalyseTool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(blobAnalyseTool.toolPar.InputPar.图像);
                                if (blobAnalyseTool.toolPar.InputPar.图像 == null)
                                {
                                    blobAnalyseTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Lack_Of_Input_Image : ToolRunStatu.Lack_Of_Input_Image);
                                    treeNode.ToolTipText = blobAnalyseTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItem == "搜索区域" || inputItem == "SearchRegion")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                blobAnalyseTool.toolPar.InputPar.搜索区域 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(blobAnalyseTool.toolPar.InputPar.搜索区域);
                                if (blobAnalyseTool.toolPar.InputPar.搜索区域 == null)
                                {
                                    blobAnalyseTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Lack_Of_Input_Search_Region : ToolRunStatu.缺少输入搜索区域);
                                    treeNode.ToolTipText = blobAnalyseTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItem == "跟随" || inputItem == "Pose")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                blobAnalyseTool.toolPar.InputPar.跟随 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XYU>;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(blobAnalyseTool.toolPar.InputPar.跟随);
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        blobAnalyseTool.clearMainImageBeforeDraw = !blobMainImagePrepared;
                        blobAnalyseTool.Run(false, false, L_toolList[i].toolName);
                        if (blobAnalyseTool.toolRunStatu == (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                            blobMainImagePrepared = true;
                        if (blobAnalyseTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", blobAnalyseTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((BlobAnalyseTool)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region SubImage
                    else if (L_toolList[i].toolType == ToolType.SubImage)
                    {
                        SubImageTool subImageTool = (SubImageTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((SubImageTool)(L_toolList[i].tool)).toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用;
                            treeNode.ToolTipText = subImageTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((SubImageTool)(L_toolList[i].tool)).toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Image : ToolRunStatu.未指定输入图像;
                                treeNode.ToolTipText = subImageTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, subImageTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "输入图像" || inputItem == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                subImageTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(subImageTool.inputImage);
                                if (subImageTool.inputImage == null)
                                {
                                    subImageTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Image : ToolRunStatu.未指定输入图像);
                                    treeNode.ToolTipText = subImageTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        subImageTool.Run(false, false, L_toolList[i].toolName);
                        if (subImageTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", subImageTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "输出图像":
                                case "OutputImage":
                                    L_toolList[i].GetOutput(outputItem).value = subImageTool.outputImage;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "图形变量不支持显示";
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region CreateROI
                    else if (L_toolList[i].toolType == ToolType.CreateROI)
                    {
                        CreateROITool createROITool = (CreateROITool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            createROITool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = createROITool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString(); if (inputItem == string.Empty)
                            {
                                createROITool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                treeNode.ToolTipText = createROITool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, createROITool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            //////createROITool.inputPose = null;
                            if (inputItem == "左上点行")
                            {
                                string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                createROITool.leftTopRow = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createROITool.leftTopRow);
                            }
                            else if (inputItem == "左上点列")
                            {
                                string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                createROITool.leftTopCol = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createROITool.leftTopCol);
                            }
                            else if (inputItem == "右下点行" || inputItem == "ExpectCircleCenterX")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                createROITool.rightDownRow = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createROITool.rightDownRow);
                            }
                            else if (inputItem == "右下点列" || inputItem == "ExpectCircleCenterY")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                createROITool.rightDownCol = Convert.ToInt16(Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value));
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createROITool.rightDownCol);
                            }
                            else if (inputItem == "跟随" || inputItem == "ExpectCircleCenterY")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                createROITool.inputPose = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createROITool.inputPose);
                            }
                            else if (inputItem == "图像" || inputItem == "ExpectC信息ircleCenterY")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                createROITool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createROITool.toolPar.InputPar.图像);
                            }
                        }

                        createROITool.Run(true, true, L_toolList[i].toolName);
                        if (createROITool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", createROITool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((CreateROITool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region CreatePosition
                    else if (L_toolList[i].toolType == ToolType.CreatePosition)
                    {
                        CreatePositionTool createPositionTool = (CreatePositionTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            createPositionTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = createPositionTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (inputItem == string.Empty)
                            {
                                createPositionTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                treeNode.ToolTipText = createPositionTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, createPositionTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            //////createROITool.inputPose = null;
                            if (inputItem == "点")
                            {
                                string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                object pointValue = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value;
                                XY point = pointValue as XY;
                                createPositionTool.toolPar.InputPar.点 = pointValue as List<XY>;
                                if (createPositionTool.toolPar.InputPar.点 == null && point != null)
                                    createPositionTool.toolPar.InputPar.点 = new List<XY>() { point };
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createPositionTool.toolPar.InputPar.点);
                            }
                            else if (inputItem == "方向")
                            {
                                string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                createPositionTool.toolPar.InputPar.方向 = Convert.ToDouble(FindToolInfoByName(sourceToolName).GetOutput(toolItem).value.ToString());
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createPositionTool.toolPar.InputPar.方向);
                            }
                        }

                        createPositionTool.Run(true, true, L_toolList[i].toolName);
                        if (createPositionTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", createPositionTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((CreatePositionTool)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XYU]")
                                {
                                    List<XYU> positions = (List<XYU>)value;
                                    if (positions.Count == 0)
                                    {
                                        value = null;
                                        break;
                                    }
                                    value = GetValue(positions[0], strs[k]);
                                }
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }
                    }
                    #endregion

                    #region CreateLine
                    else if (L_toolList[i].toolType == ToolType.CreateLine)
                    {
                        CreateLineTool createLineTool = (CreateLineTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            createLineTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = createLineTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (inputItem == string.Empty)
                            {
                                createLineTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                treeNode.ToolTipText = createLineTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, createLineTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            //////createROITool.inputPose = null;
                            if (inputItem == "起点")
                            {

                                string jobName1 = string.Empty;
                                if (sourceFrom.Contains("["))
                                {
                                    jobName1 = sourceFrom.Split(new char[] { '[' })[1];
                                    jobName1 = jobName1.Split(new char[] { ']' })[0];
                                    sourceFrom = sourceFrom.Split(new char[] { ']' })[1];
                                    string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                    string toolItem = Regex.Split(sourceFrom, "->")[1];
                                    createLineTool.toolPar.InputPar.起点 = (FindJobByName(jobName1).FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XY>)[0];
                                    GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createLineTool.toolPar.InputPar.起点);
                                }
                                else
                                {
                                    jobName1 = this.jobName;
                                    string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                    sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                    string toolItem = Regex.Split(sourceFrom, "->")[1];
                                    createLineTool.toolPar.InputPar.起点 = (FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XY>)[0];
                                    GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createLineTool.toolPar.InputPar.起点);
                                }
                            }
                            else if (inputItem == "终点")
                            {
                                string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                createLineTool.toolPar.InputPar.终点 = (FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XY>)[0];
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(createLineTool.toolPar.InputPar.终点);
                            }
                        }

                        createLineTool.Run(true, true, L_toolList[i].toolName);

                        if (createLineTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", createLineTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((CreateLineTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }
                    }
                    #endregion

                    #region ArrayRegion
                    else if (L_toolList[i].toolType == ToolType.ArrayRegion)
                    {
                        ArrayRegionTool arrayRegionTool = (ArrayRegionTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            arrayRegionTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = arrayRegionTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString(); if (inputItem == string.Empty)
                            {
                                arrayRegionTool.toolRunStatu = ToolRunStatu.Not_Input_Image;
                                treeNode.ToolTipText = arrayRegionTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, arrayRegionTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            arrayRegionTool.inputImage = null;
                            if (inputItem == "输入图像")
                            {
                                string sourceToolName = sourceFrom.Split(new char[] { '.' })[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                arrayRegionTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(arrayRegionTool.inputImage);
                            }
                        }

                        arrayRegionTool.Run(true, true, L_toolList[i].toolName);
                        if (arrayRegionTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", arrayRegionTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "输出区域":
                                case "<--Result_CenterX":
                                    L_toolList[i].GetOutput(outputItem).value = arrayRegionTool.outputRegion;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "图形变量不支持显示";
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region Mark
                    else if (L_toolList[i].toolType == ToolType.Mark)
                    {
                        MarkTool markTool = (MarkTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            markTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = markTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                markTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Pos;
                                treeNode.ToolTipText = markTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, markTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "输入点" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                XY point = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XY;
                                markTool.inputPoint.X = point.X;
                                markTool.inputPoint.Y = point.Y;
                            }
                        }

                        markTool.Run(true, true, L_toolList[i].toolName);

                        if (markTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", markTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                    }
                    #endregion

                    #region PoseToStr
                    else if (L_toolList[i].toolType == ToolType.ToStr)
                    {
                        ToStrTool poseToStrTool = (ToStrTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            poseToStrTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = poseToStrTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        poseToStrTool.toolPar.InputPar.点 = null;
                        poseToStrTool.inputPos = null;

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                poseToStrTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Pos;
                                treeNode.ToolTipText = poseToStrTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, poseToStrTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "位置" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                var xyu = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value;
                                string temp = xyu.ToString();
                                if (xyu.ToString() == "VMPro.XYU")
                                {
                                    XYU temp1 = xyu as XYU;
                                    poseToStrTool.inputPos = new XYU();
                                    poseToStrTool.inputPos.Point.X = temp1.Point.X;
                                    poseToStrTool.inputPos.Point.Y = temp1.Point.Y;
                                    poseToStrTool.inputPos.U = temp1.U;
                                }
                                else
                                {
                                    List<XYU> temp1 = xyu as List<XYU>;
                                    poseToStrTool.inputPos = new XYU();
                                    poseToStrTool.inputPos.Point.X = temp1[0].Point.X;
                                    poseToStrTool.inputPos.Point.Y = temp1[0].Point.Y;
                                    poseToStrTool.inputPos.U = temp1[0].U;
                                }
                            }
                            else if (inputItem == "点" || inputItem == "InputStr")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                List<XY> xyu = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XY>;
                                poseToStrTool.toolPar.InputPar.点 = new XY();
                                poseToStrTool.toolPar.InputPar.点.X = xyu[0].X;
                                poseToStrTool.toolPar.InputPar.点.Y = xyu[0].Y;
                            }
                        }

                        poseToStrTool.Run(true, true, L_toolList[i].toolName);

                        if (poseToStrTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", poseToStrTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }




                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((ToStrTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region DistancePP
                    else if (L_toolList[i].toolType == ToolType.DistancePP)
                    {
                        DistancePPTool distancePPTool = (DistancePPTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            distancePPTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = distancePPTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        //////distancePPTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                distancePPTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                                treeNode.ToolTipText = distancePPTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, distancePPTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "起点" || inputItemName == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                distancePPTool.toolPar.InputPar.起点 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value;
                                GetToolNodeByNodeText(inputItemName + sourceFrom).ToolTipText = FormatShowTip(distancePPTool.toolPar.InputPar.起点);
                                if (distancePPTool.toolPar.InputPar.起点 == null)
                                {
                                    distancePPTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                    treeNode.ToolTipText = distancePPTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItemName == "终点" || inputItemName == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                distancePPTool.toolPar.InputPar.终点 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value;
                                GetToolNodeByNodeText(inputItemName + sourceFrom).ToolTipText = FormatShowTip(distancePPTool.toolPar.InputPar.终点);
                                if (distancePPTool.toolPar.InputPar.终点 == null)
                                {
                                    distancePPTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                    treeNode.ToolTipText = distancePPTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        distancePPTool.Run(false, false, L_toolList[i].toolName);

                        if (distancePPTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", distancePPTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((DistancePPTool)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XYU]")
                                {
                                    List<XYU> positions = (List<XYU>)value;
                                    if (positions.Count == 0)
                                    {
                                        value = null;
                                        break;
                                    }
                                    value = GetValue(positions[0], strs[k]);
                                }
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }
                    }
                    #endregion

                    #region DistancePL
                    else if (L_toolList[i].toolType == ToolType.DistancePL)
                    {
                        DistancePLTool distancePLTool = (DistancePLTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            distancePLTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = distancePLTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (inputItem == string.Empty)
                            {
                                ((DistancePLTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.Not_Assign_Input_Source);
                                treeNode.ToolTipText = distancePLTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                return L_result;
                            }

                            if (inputItem == "输入点" || inputItem == "InputSegment1")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                distancePLTool.inputPoint = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XY;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(distancePLTool.inputPoint);
                                if (distancePLTool.inputPoint == null)
                                {
                                    distancePLTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = distancePLTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, distancePLTool.toolRunStatu.ToString()), Color.Red);
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            if (inputItem == "线段" || inputItem == "InputSegment2")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                distancePLTool.inputLine = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as Line;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(distancePLTool.inputLine);
                                if (distancePLTool.inputLine == null)
                                {
                                    distancePLTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = distancePLTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        distancePLTool.Run(true, true, L_toolList[i].toolName);

                        if (distancePLTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", distancePLTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "ResultDistance":
                                case "结果距离":
                                    L_toolList[i].GetOutput(outputItem).value = distancePLTool.outputDistance;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = distancePLTool.outputDistance.ToString();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region DistanceSS
                    else if (L_toolList[i].toolType == ToolType.DistanceSS)
                    {
                        DistanceLLTool distanceSegmentAndSegmentTool = (DistanceLLTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            distanceSegmentAndSegmentTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = distanceSegmentAndSegmentTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (inputItem == string.Empty)
                            {
                                ((DistanceLLTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.Not_Assign_Input_Source);
                                treeNode.ToolTipText = distanceSegmentAndSegmentTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, distanceSegmentAndSegmentTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "线1" || inputItem == "InputSegment1")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                distanceSegmentAndSegmentTool.line1 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as Line;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(distanceSegmentAndSegmentTool.line1);
                                if (distanceSegmentAndSegmentTool.line1 == null)
                                {
                                    distanceSegmentAndSegmentTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = distanceSegmentAndSegmentTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            if (inputItem == "线2" || inputItem == "InputSegment2")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                distanceSegmentAndSegmentTool.line2 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as Line;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(distanceSegmentAndSegmentTool.line2);
                                if (distanceSegmentAndSegmentTool.line2 == null)
                                {
                                    distanceSegmentAndSegmentTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = distanceSegmentAndSegmentTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        distanceSegmentAndSegmentTool.Run(true, true, L_toolList[i].toolName);
                        if (distanceSegmentAndSegmentTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", distanceSegmentAndSegmentTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "ResultDistance":
                                case "结果距离值":
                                    L_toolList[i].GetOutput(outputItem).value = distanceSegmentAndSegmentTool.ResultDistance;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = distanceSegmentAndSegmentTool.ResultDistance.ToString();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region CenterOfPP
                    else if (L_toolList[i].toolType == ToolType.CenterOfPP)
                    {
                        CenterOfPP centerOfTwoPointTool = (CenterOfPP)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            centerOfTwoPointTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = centerOfTwoPointTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (inputItem == string.Empty)
                            {
                                ((DistanceLLTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.Not_Assign_Input_Source);
                                treeNode.ToolTipText = centerOfTwoPointTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, centerOfTwoPointTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "点1" || inputItem == "InputSegment1")
                            {

                                string jobName1 = string.Empty;
                                if (sourceFrom.Contains("["))
                                {
                                    jobName1 = sourceFrom.Split(new char[] { '[' })[1];
                                    jobName1 = jobName1.Split(new char[] { ']' })[0];
                                    sourceFrom = sourceFrom.Split(new char[] { ']' })[1];
                                }
                                else
                                {
                                    jobName1 = this.jobName;
                                }

                                ////string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                ////string toolItem = Regex.Split(sourceFrom, "->")[1];


                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                //////sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                centerOfTwoPointTool.toolPar.InputPar.点1 = (FindJobByName(jobName1).FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XY>)[0];
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(centerOfTwoPointTool.toolPar.InputPar.点1);
                                if (centerOfTwoPointTool.toolPar.InputPar.点1 == null)
                                {
                                    centerOfTwoPointTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = centerOfTwoPointTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            if (inputItem == "点2" || inputItem == "InputSegment2")
                            {

                                string jobName1 = string.Empty;
                                if (sourceFrom.Contains("["))
                                {
                                    jobName1 = sourceFrom.Split(new char[] { '[' })[1];
                                    jobName1 = jobName1.Split(new char[] { ']' })[0];
                                    sourceFrom = sourceFrom.Split(new char[] { ']' })[1];
                                }
                                else
                                {
                                    jobName1 = this.jobName;
                                }
                                //sourceFrom = sourceFrom.Split(new char[] { ']' })[1];
                                //string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                //string toolItem = Regex.Split(sourceFrom, "->")[1];


                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                //sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                centerOfTwoPointTool.toolPar.InputPar.点2 = (FindJobByName(jobName1).FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XY>)[0];
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(centerOfTwoPointTool.toolPar.InputPar.点2);
                                if (centerOfTwoPointTool.toolPar.InputPar.点2 == null)
                                {
                                    centerOfTwoPointTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = centerOfTwoPointTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        centerOfTwoPointTool.Run(true, true, L_toolList[i].toolName);
                        if (centerOfTwoPointTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", centerOfTwoPointTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((CenterOfPP)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XYU]")
                                {
                                    List<XYU> positions = (List<XYU>)value;
                                    if (positions.Count == 0)
                                    {
                                        value = null;
                                        break;
                                    }
                                    value = GetValue(positions[0], strs[k]);
                                }
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region LLPoint
                    else if (L_toolList[i].toolType == ToolType.LLIntersect)
                    {
                        LLIntersectTool llPointTool = (LLIntersectTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            llPointTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = llPointTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (inputItem == string.Empty)
                            {
                                ((LLIntersectTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.Not_Assign_Input_Source);
                                treeNode.ToolTipText = llPointTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, llPointTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "线1" || inputItem == "InputSegment1")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                llPointTool.inputLine1 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as Line;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(llPointTool.inputLine1);
                                if (llPointTool.inputLine1 == null)
                                {
                                    llPointTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = llPointTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            if (inputItem == "线2" || inputItem == "InputSegment2")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                llPointTool.inputLine2 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as Line;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(llPointTool.inputLine2);
                                if (llPointTool.inputLine2 == null)
                                {
                                    llPointTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = llPointTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        llPointTool.Run(true, true, L_toolList[i].toolName);
                        if (llPointTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", llPointTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((LLIntersectTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");

                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region AngleLL
                    else if (L_toolList[i].toolType == ToolType.AngleLL)
                    {
                        AngleLLTool angleLLTool = (AngleLLTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            angleLLTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = angleLLTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (inputItem == string.Empty)
                            {
                                ((AngleLLTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.Not_Assign_Input_Source);
                                treeNode.ToolTipText = angleLLTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, angleLLTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "线1" || inputItem == "InputSegment1")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                angleLLTool.inputLine1 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as Line;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(angleLLTool.inputLine1);
                                if (angleLLTool.inputLine1 == null)
                                {
                                    angleLLTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = angleLLTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            if (inputItem == "线2" || inputItem == "InputSegment2")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                angleLLTool.inputLine2 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as Line;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(angleLLTool.inputLine2);
                                if (angleLLTool.inputLine2 == null)
                                {
                                    angleLLTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                    treeNode.ToolTipText = angleLLTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        angleLLTool.Run(true, true, L_toolList[i].toolName);
                        if (angleLLTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", angleLLTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((AngleLLTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");

                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region RegionFeature
                    else if (L_toolList[i].toolType == ToolType.RegionFeature)
                    {
                        RegionFeatureTool regionFeatureTool = (RegionFeatureTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((RegionFeatureTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = regionFeatureTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((RegionFeatureTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                treeNode.ToolTipText = regionFeatureTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, regionFeatureTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "InputImage" || inputItem == "输入区域")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                regionFeatureTool.inputRegion = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(regionFeatureTool.inputRegion);
                                if (regionFeatureTool.inputRegion == null)
                                {
                                    regionFeatureTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                    treeNode.ToolTipText = regionFeatureTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        regionFeatureTool.Run(true, true, L_toolList[i].toolName);
                        if (regionFeatureTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", regionFeatureTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "圆度":
                                    L_toolList[i].GetOutput(outputItem).value = regionFeatureTool.Roundness;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = regionFeatureTool.Roundness.ToString();
                                    break;
                                case "中心点":
                                    L_toolList[i].GetOutput(outputItem).value = regionFeatureTool.CenterPoint;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = regionFeatureTool.CenterPoint.ToShowTip();
                                    break;
                                case "外接仿矩":
                                    L_toolList[i].GetOutput(outputItem).value = regionFeatureTool.outRectangle2;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "图形变量暂不支持显示";
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region RegionOperation
                    else if (L_toolList[i].toolType == ToolType.RegionOperation)
                    {
                        RegionOperationTool regionOperationTool = (RegionOperationTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((BarcodeTool)(L_toolList[i].tool)).toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = regionOperationTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((BarcodeTool)(L_toolList[i].tool)).toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                treeNode.ToolTipText = regionOperationTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, regionOperationTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "InputImage" || inputItem == "区域1")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                regionOperationTool.inputRegion1 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(regionOperationTool.inputRegion1);
                                if (regionOperationTool.inputRegion1 == null)
                                {
                                    regionOperationTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                    treeNode.ToolTipText = regionOperationTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                            else if (inputItem == "InputImage" || inputItem == "区域2")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                regionOperationTool.inputRegion2 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(regionOperationTool.inputRegion2);
                                if (regionOperationTool.inputRegion2 == null)
                                {
                                    regionOperationTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                    treeNode.ToolTipText = regionOperationTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        regionOperationTool.Run(true, true, L_toolList[i].toolName);

                        if (regionOperationTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", regionOperationTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "结果区域":
                                    L_toolList[i].GetOutput(outputItem).value = regionOperationTool.outputRegion;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = regionOperationTool.Roundness.ToString();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region OCR
                    else if (L_toolList[i].toolType == ToolType.OCR)
                    {
                        OCRTool ocrTool = (OCRTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ocrTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = ocrTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        ocrTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ocrTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                                treeNode.ToolTipText = ocrTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, ocrTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "输入图像" || inputItemName == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                ocrTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItemName + sourceFrom).ToolTipText = FormatShowTip(ocrTool.inputImage);
                                if (ocrTool.inputImage == null)
                                {
                                    ocrTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                    treeNode.ToolTipText = ocrTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        ocrTool.Run(false, false, L_toolList[i].toolName);

                        if (ocrTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", ocrTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "字符串":
                                case "ResultStr":
                                    //////if (ocrTool.L_matchResult.Count == 0)
                                    //////{
                                    //////    L_toolList[i].GetOutput (outputItem ).value = "";
                                    //////    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "空";
                                    //////}
                                    //////else
                                    {

                                        L_toolList[i].GetOutput(outputItem).value = ocrTool.outputStr;
                                        GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = ocrTool.outputStr;
                                    }
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region Barcode
                    else if (L_toolList[i].toolType == ToolType.Barcode)
                    {
                        BarcodeTool barcodeTool = (BarcodeTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((BarcodeTool)(L_toolList[i].tool)).toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = barcodeTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((BarcodeTool)(L_toolList[i].tool)).toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                treeNode.ToolTipText = barcodeTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, barcodeTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "InputImage" || inputItem == "输入图像")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                barcodeTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(barcodeTool.inputImage);
                                if (barcodeTool.inputImage == null)
                                {
                                    barcodeTool.toolRunStatu = ToolRunStatu.Not_Assign_Input_Source;
                                    treeNode.ToolTipText = barcodeTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        barcodeTool.Run(true, true, L_toolList[i].toolName);

                        if (barcodeTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", barcodeTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "字符串":
                                    if (barcodeTool.resultNum == 0)
                                    {
                                        L_toolList[i].GetOutput(outputItem).value = "";
                                        GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "无条码";
                                    }
                                    else
                                    {
                                        L_toolList[i].GetOutput(outputItem).value = barcodeTool.outputStr;
                                        GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = barcodeTool.outputStr;
                                    }
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region CodeEdit
                    else if (L_toolList[i].toolType == ToolType.CodeEdit)
                    {
                        CodeEditTool codeEditTool = (CodeEditTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            codeEditTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = codeEditTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }

                        SyncCodeEditInputsBeforeRun(L_toolList[i], codeEditTool);
                        codeEditTool.Run(true, true, L_toolList[i].toolName);

                        if (codeEditTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                string codeEditMessage = string.IsNullOrEmpty(codeEditTool.compileResult) ? L_toolList[i].toolTipInfo : codeEditTool.compileResult;
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? string.Format("Process [{0}] failed, tool [{1}] {2}: {3}", jobName, L_toolList[i].toolName, codeEditTool.toolRunStatu.ToString(), codeEditMessage) : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}，{3}", jobName, L_toolList[i].toolName, codeEditTool.toolRunStatu.ToString(), codeEditMessage), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", codeEditTool.toolRunStatu.ToString(), 0, codeEditMessage);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            object inputValue = codeEditTool.GetInputValue(inputItem);
                            string inputSource = L_toolList[i].input[j].value == null ? string.Empty : L_toolList[i].input[j].value.ToString();
                            TreeNode inputNode = GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItem + inputSource);
                            if (inputNode != null)
                                inputNode.ToolTipText = FormatShowTip(inputValue);
                        }

                        for (int j = 0; j < outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            object value = codeEditTool.GetOutputValue(outputItem);
                            L_toolList[i].GetOutput(outputItem).value = value;
                            TreeNode outputNode = GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem);
                            if (outputNode != null)
                                outputNode.ToolTipText = FormatShowTip(value);
                        }
                    }
                    #endregion

                    #region OPTLight
                    else if (L_toolList[i].toolType == ToolType.Light_OPT)
                    {
                        Light_OPTTool optLightTool = (Light_OPTTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((Light_OPTTool)(L_toolList[i].tool)).toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = optLightTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }

                        optLightTool.Run(false, false, L_toolList[i].toolName);
                        if (optLightTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", optLightTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);

                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                    }
                    #endregion

                    #region OPTLightControl
                    else if (L_toolList[i].toolType == ToolType.OPTLightControl)
                    {
                        OptLightControlTool optLightControlTool = (OptLightControlTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((OptLightControlTool)(L_toolList[i].tool)).toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = optLightControlTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }

                        optLightControlTool.Run(false, false, L_toolList[i].toolName);
                        if (optLightControlTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", optLightControlTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                    }
                    #endregion

                    #region KeyenceScanner
                    else if (L_toolList[i].toolType == ToolType.Scaner_Kenyence)
                    {
                        Scaner_KenyenceTool kenyenceScanerTool = (Scaner_KenyenceTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((KenyenceScanerTool1)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = kenyenceScanerTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }

                        kenyenceScanerTool.Run(true, true, L_toolList[i].toolName);

                        if (kenyenceScanerTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", kenyenceScanerTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "扫码结果":
                                    L_toolList[i].GetOutput(outputItem).value = kenyenceScanerTool.resultStr;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = kenyenceScanerTool.resultStr;
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region KeyenceScanner1
                    else if (L_toolList[i].toolType == ToolType.KeyenceScanner1)
                    {
                        KenyenceScanerTool1 kenyenceScanerTool = (KenyenceScanerTool1)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((KenyenceScanerTool1)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = kenyenceScanerTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }

                        kenyenceScanerTool.Run(true, true, L_toolList[i].toolName);

                        if (kenyenceScanerTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", kenyenceScanerTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "扫码结果":
                                    L_toolList[i].GetOutput(outputItem).value = kenyenceScanerTool.resultStr;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = kenyenceScanerTool.resultStr;
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region DataAnalyse
                    else if (L_toolList[i].toolType == ToolType.DataAnalyse)
                    {
                        DataAnalyseTool dataAnalyseTool = (DataAnalyseTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((DataAnalyseTool)(L_toolList[i].tool)).toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = dataAnalyseTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((BlobAnalyseTool)(L_toolList[i].tool)).toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                                treeNode.ToolTipText = dataAnalyseTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, dataAnalyseTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "输入项1" || inputItem == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                dataAnalyseTool.toolPar.InputPar.输入项1 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value.ToString();
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(dataAnalyseTool.toolPar.InputPar.输入项1);
                                if (dataAnalyseTool.toolPar.InputPar.输入项1 == null)
                                {
                                    dataAnalyseTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Lack_Of_Input_Image : ToolRunStatu.Lack_Of_Input_Image);
                                    treeNode.ToolTipText = dataAnalyseTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        dataAnalyseTool.Run(false, false, L_toolList[i].toolName);
                        if (dataAnalyseTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", dataAnalyseTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((DataAnalyseTool)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }


                    }
                    #endregion

                    #region batteryFirstAlign
                    else if (L_toolList[i].toolType == ToolType.batteryFirstAlign)
                    {
                        BatteryFirstAlignTool batteryFirstAlignTool = (BatteryFirstAlignTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            batteryFirstAlignTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = batteryFirstAlignTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                batteryFirstAlignTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                                treeNode.ToolTipText = batteryFirstAlignTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, batteryFirstAlignTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "输入图像" || inputItemName == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3); ;
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                batteryFirstAlignTool.inputImage = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                if (batteryFirstAlignTool.inputImage == null)
                                {
                                    batteryFirstAlignTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                    treeNode.ToolTipText = batteryFirstAlignTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        batteryFirstAlignTool.Run(false, false, L_toolList[i].toolName);

                        if (batteryFirstAlignTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", batteryFirstAlignTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            switch (outputItem)
                            {
                                case "位置":
                                case "Pose":
                                    L_toolList[i].GetOutput(outputItem).value = batteryFirstAlignTool.outputPos;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = batteryFirstAlignTool.outputPos.ToShowTip();
                                    break;
                                case "输出图像":
                                case "Posxxxe":
                                    L_toolList[i].GetOutput(outputItem).value = batteryFirstAlignTool.imageWithoutEar;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = "图形变量暂不支持显示";
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region EthernetReceive
                    if (L_toolList[i].toolType == ToolType.EthernetReceive)
                    {
                        if (initRun)        //程序启动时的初始化运行时，通讯类的工具不被执行
                        {
                            treeNode.ForeColor = Color.Green;
                            continue;
                        }

                        EthernetReceiveTool ethernetReceiveTool = (EthernetReceiveTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ethernetReceiveTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = ethernetReceiveTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }

                        if (!Configuration.SpeedMode)
                            treeNode.ForeColor = Color.Orange;


                        ethernetReceiveTool.Run(true, false, L_toolList[i].toolName);


                        if (ethernetReceiveTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", ethernetReceiveTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                    }
                    #endregion

                    #region EthernetSend
                    if (L_toolList[i].toolType == ToolType.EthernetSend)
                    {
                        if (initRun)        //程序启动时的初始化运行时，通讯类的工具不被执行
                        {
                            treeNode.ForeColor = Color.Green;
                            continue;
                        }

                        EthernetSendTool ethernetSendTool = (EthernetSendTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ethernetSendTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = ethernetSendTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }


                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ethernetSendTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                                treeNode.ToolTipText = ethernetSendTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, ethernetSendTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "消息" || inputItemName == "InputImage")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                ethernetSendTool.toolPar.InputPar.消息 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value.ToString();
                                GetToolNodeByNodeText(inputItemName + sourceFrom).ToolTipText = FormatShowTip(ethernetSendTool.toolPar.InputPar.消息);
                                if (ethernetSendTool.toolPar.InputPar.消息 == null)
                                {
                                    ethernetSendTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                    treeNode.ToolTipText = ethernetSendTool.toolRunStatu.ToString();
                                    treeNode.ForeColor = Color.Red;
                                    sourceValueIsEmpty = true;
                                    break;
                                }
                            }
                        }

                        if (!Configuration.SpeedMode)
                            treeNode.ForeColor = Color.Orange;

                        ethernetSendTool.Run(true, false, L_toolList[i].toolName);


                        if (ethernetSendTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", ethernetSendTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }


                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((EthernetSendTool)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XYU]")
                                {
                                    List<XYU> positions = (List<XYU>)value;
                                    if (positions.Count == 0)
                                    {
                                        value = null;
                                        break;
                                    }
                                    value = GetValue(positions[0], strs[k]);
                                }
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }
                    }
                    #endregion

                    #region PLCComm
                    else if (L_toolList[i].toolType == ToolType.PLCComm)
                    {
                        if (initRun)
                        {
                            treeNode.ForeColor = Color.Green;
                            continue;
                        }

                        PLCCommTool plcCommTool = (PLCCommTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            plcCommTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = plcCommTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }

                        // 写入值输入项：从流中读取上游输出
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom != string.Empty)
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                if (inputItemName == "写入值" || inputItemName == "WriteValue")
                                {
                                    object srcVal = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value;
                                    plcCommTool.toolPar.InputPar.写入值 = srcVal != null ? srcVal.ToString() : string.Empty;
                                    GetToolNodeByNodeText(inputItemName + sourceFrom).ToolTipText = FormatShowTip(plcCommTool.toolPar.InputPar.写入值);
                                }
                            }
                        }

                        plcCommTool.Run(true, false, L_toolList[i].toolName);

                        if (plcCommTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", plcCommTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        // 读取值输出项：将读取结果写入流
                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;
                            if (outputItem == "读取值" || outputItem == "ReadValue")
                            {
                                L_toolList[i].GetOutput(outputItem).value = plcCommTool.toolPar.ResultPar.读取值;
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(plcCommTool.toolPar.ResultPar.读取值);
                            }
                        }
                    }
                    #endregion

                    #region BuChang
                    else if (L_toolList[i].toolType == ToolType.BuChang)
                    {
                        BuChangTool buChangTool = (BuChangTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            buChangTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = buChangTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.Goldenrod;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                buChangTool.toolRunStatu = ToolRunStatu.Not_Asign_Input_Source;
                                treeNode.ToolTipText = buChangTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, buChangTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            //buChangTool.inputPose = null;
                            if (inputItem == "InputImage" || inputItem == "补偿")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0]; ;
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                buChangTool.inputPos = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                            }
                            if (inputItem == "InputImage" || inputItem == "点")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0]; ;
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                buChangTool.inputPos = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as XYU;
                            }
                        }
                        buChangTool.Run(false, true, L_toolList[i].toolName);

                        if (buChangTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", buChangTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        for (int j = 0; j < outputItemNum; j++)
                        {
                            string outputItemName = L_toolList[i].output[j].IOName;
                            switch (outputItemName)
                            {
                                case "偏差":
                                case "ResultLineStartX":
                                    L_toolList[i].GetOutput(outputItemName).value = buChangTool.outputPos;
                                    GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItemName).ToolTipText = buChangTool.outputPos.ToShowTip();
                                    break;
                            }
                        }


                    }
                    #endregion

                    #region PointOffset
                    else if (L_toolList[i].toolType == ToolType.PointOffset)
                    {
                        PointOffsetTool pointOffsetTool = (PointOffsetTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            pointOffsetTool.toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = pointOffsetTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                pointOffsetTool.toolRunStatu = ToolRunStatu.Not_Asign_Input_Source;
                                treeNode.ToolTipText = pointOffsetTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, pointOffsetTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            //buChangTool.inputPose = null;
                            if (inputItem == "InputImage" || inputItem == "点")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0]; ;
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                pointOffsetTool.toolPar.InputPar.点 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as List<XY>;
                                GetToolNodeByNodeText(inputItem + sourceFrom).ToolTipText = FormatShowTip(pointOffsetTool.toolPar.InputPar.点);
                            }
                        }
                        pointOffsetTool.Run(false, true, L_toolList[i].toolName);

                        if (pointOffsetTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", pointOffsetTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                        //for (int j = 0; j < outputItemNum; j++)
                        //{
                        //    string outputItemName = L_toolList[i].output[j].IOName;
                        //    switch (outputItemName)
                        //    {
                        //        case "点":
                        //        case "ResultLineStartX":
                        //            L_toolList[i].GetOutput(outputItemName).value = pointOffsetTool.toolPar.ResultPar.输出点.X;
                        //            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItemName).ToolTipText = pointOffsetTool.toolPar.ResultPar.输出点.ToShowTip();
                        //            break;
                        //    }
                        //}



                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result3 = ((PointOffsetTool)(L_toolList[i].tool)).toolPar;
                            object value = result3;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }
                    }
                    #endregion

                    #region DisplayEdit
                    else if (L_toolList[i].toolType == ToolType.DisplayEdit)
                    {
                        DisplayEditTool displayEditTool = (DisplayEditTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            displayEditTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用);
                            treeNode.ToolTipText = displayEditTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        //////shapeMatchTool.ClearLastInput();

                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItemName = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItemName).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                displayEditTool.toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Asign_Input_Image : ToolRunStatu.未指定输入图像);
                                treeNode.ToolTipText = displayEditTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, displayEditTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }
                            if (inputItemName == "输入图像" || inputItemName == "InputImage")
                            {
                                //////string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                //////sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                //////string toolItem = Regex.Split(sourceFrom, "->")[1];
                                //////displayEditTool.toolPar.InputPar.图像 = FindToolInfoByName(sourceToolName).GetOutput(toolItem).value as HObject;
                                //////if (displayEditTool.toolPar.InputPar.图像 == null)
                                //////{
                                //////    displayEditTool.toolRunStatu = ToolRunStatu.未指定输入图像;
                                //////    treeNode.ToolTipText = displayEditTool.toolRunStatu.ToString();
                                //////    treeNode.ForeColor = Color.Red;
                                //////    sourceValueIsEmpty = true;
                                //////    break;
                                //////}
                            }
                        }
                        if (sourceValueIsEmpty)
                            break;
                        displayEditTool.Run(false, false, L_toolList[i].toolName);

                        if (displayEditTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", displayEditTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }

                        for (int j = 0; j < this.outputItemNum; j++)
                        {
                            string outputItem = L_toolList[i].output[j].IOName;

                            string[] strs = Regex.Split(outputItem, " . ");
                            ToolParBase result = ((MatchTool)(L_toolList[i].tool)).toolPar;
                            object value = result;

                            value = GetValue(value, "ResultPar");
                            for (int k = 0; k < strs.Length; k++)
                            {
                                string temp = value.GetType().ToString();
                                if (temp == "System.Collections.Generic.List`1[VMPro.XYU]")
                                {
                                    List<XYU> positions = (List<XYU>)value;
                                    if (positions.Count == 0)
                                    {
                                        value = null;
                                        break;
                                    }
                                    value = GetValue(positions[0], strs[k]);
                                }
                                else
                                    value = GetValue(value, strs[k]);
                            }

                            L_toolList[i].GetOutput(outputItem).value = value;

                            GetToolIONodeByNodeText(L_toolList[i].toolName, "-->" + outputItem).ToolTipText = FormatShowTip(value);
                        }
                    }
                    #endregion

                    #region Label
                    else if (L_toolList[i].toolType == ToolType.Label)
                    {
                        LabelTool labelTool = (LabelTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((LabelTool)(L_toolList[i].tool)).toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Enabled : ToolRunStatu.未启用;
                            treeNode.ToolTipText = labelTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        labelTool.D_inputItemAndVlaue.Clear();
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string inputItem = L_toolList[i].input[j].IOName;
                            string sourceFrom = L_toolList[i].GetInput(inputItem).value.ToString();
                            if (sourceFrom == string.Empty)
                            {
                                ((LabelTool)(L_toolList[i].tool)).toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源;
                                treeNode.ToolTipText = labelTool.toolRunStatu.ToString();
                                treeNode.ForeColor = Color.Red;
                                Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 运行失败，原因： {1}", L_toolList[i].toolName, labelTool.toolRunStatu.ToString()), Color.Red);
                                return L_result;
                            }

                            if (inputItem == "输入项1" || inputItem == "InputItem1")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                if (sourceToolName == "全局变量")
                                {
                                    string toolItem = Regex.Split(sourceFrom, "->")[1];
                                    labelTool.D_inputItemAndVlaue.Add("InputItem1", Project.Instance.curEngine.globelVariable.GetGlobalVariableValue(toolItem).ToString());
                                }
                                else
                                {
                                    string toolItem = Regex.Split(sourceFrom, "->")[1];
                                    labelTool.D_inputItemAndVlaue.Add("InputItem1", FindToolInfoByName(sourceToolName).GetOutput(toolItem).value.ToString());
                                }
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItem + sourceFrom).ToolTipText = FormatShowTip(labelTool.D_inputItemAndVlaue["InputItem1"]);
                            }
                            else if (inputItem == "输入项2" || inputItem == "InputItem2")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                if (sourceToolName == "全局变量")
                                {
                                    string toolItem = Regex.Split(sourceFrom, "->")[1];
                                    labelTool.D_inputItemAndVlaue.Add("InputItem2", Project.Instance.curEngine.globelVariable.GetGlobalVariableValue(toolItem).ToString());
                                }
                                else
                                {
                                    string toolItem = Regex.Split(sourceFrom, "->")[1];
                                    labelTool.D_inputItemAndVlaue.Add("InputItem2", FindToolInfoByName(sourceToolName).GetOutput(toolItem).value.ToString());
                                }
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItem + sourceFrom).ToolTipText = FormatShowTip(labelTool.D_inputItemAndVlaue["InputItem2"]);
                            }
                            else if (inputItem == "输入项3" || inputItem == "InputItem3")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                labelTool.D_inputItemAndVlaue.Add("InputItem3", FindToolInfoByName(sourceToolName).GetOutput(toolItem).value.ToString());
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItem + sourceFrom).ToolTipText = FormatShowTip(labelTool.D_inputItemAndVlaue["InputItem3"]);
                            }
                            else if (inputItem == "输入项4" || inputItem == "InputItem4")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                labelTool.D_inputItemAndVlaue.Add("InputItem4", FindToolInfoByName(sourceToolName).GetOutput(toolItem).value.ToString());
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItem + sourceFrom).ToolTipText = FormatShowTip(labelTool.D_inputItemAndVlaue["InputItem4"]);
                            }
                            else if (inputItem == "输入项5" || inputItem == "InputItem5")
                            {
                                string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                                sourceToolName = sourceToolName.Substring(3, Regex.Split(sourceFrom, "->")[0].Length - 3);
                                string toolItem = Regex.Split(sourceFrom, "->")[1];
                                labelTool.D_inputItemAndVlaue.Add("InputItem5", FindToolInfoByName(sourceToolName).GetOutput(toolItem).value.ToString());
                                GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + inputItem + sourceFrom).ToolTipText = FormatShowTip(labelTool.D_inputItemAndVlaue["InputItem5"]);
                            }
                        }

                        labelTool.Run(true, true, L_toolList[i].toolName);

                        if (labelTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", labelTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo);
                            }
                            treeNode.ForeColor = Color.Red;
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                    }
                    #endregion

                    #region Output
                    else if (L_toolList[i].toolType == ToolType.Output)
                    {
                        OutputTool outputTool = (OutputTool)L_toolList[i].tool;
                        if (!L_toolList[i].enable)
                        {
                            ((OutputTool)(L_toolList[i].tool)).toolRunStatu = ToolRunStatu.Not_Enabled;
                            treeNode.ToolTipText = outputTool.toolRunStatu.ToString();
                            treeNode.ForeColor = Color.DarkGray;
                            continue;
                        }
                        for (int j = 0; j < inputItemNum; j++)
                        {
                            string sourceFrom = L_toolList[i].input[j].IOName;
                            string sourceToolName = Regex.Split(sourceFrom, "->")[0];
                            string sourceToolItem = Regex.Split(sourceFrom, "->")[1];

                            object value = FindToolInfoByName(sourceToolName).GetOutput(sourceToolItem).value;
                            GetToolIONodeByNodeText(L_toolList[i].toolName, "<--" + sourceFrom).ToolTipText = FormatShowTip(value);
                            L_toolList[i].GetInput(sourceFrom).value = value;
                            L_result.Add(value);
                        }
                        outputTool.Run(true, true, L_toolList[i].toolName);

                        if (outputTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                        {
                            if (!Configuration.SpeedMode)
                            {
                                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "The process was successfully run,Elapsed：  ms" : string.Format("流程 [{0}] 运行失败，原因：工具 [{1}] {2}", jobName, L_toolList[i].toolName, L_toolList[i].tool.toolRunStatu.ToString()), Color.Red);
                                treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", outputTool.toolRunStatu.ToString(), 0, L_toolList[i].toolTipInfo); treeNode.ForeColor = Color.Red;
                            }
                            jobRunStatu = JobRunStatu.Fail;
                            break;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Green;
                        }
                    }
                    #endregion

                    if (!Configuration.SpeedMode)
                    {
                        double elapseTime = jobElapsedTime.ElapsedMilliseconds - recordElapseTime;
                        recordElapseTime = jobElapsedTime.ElapsedMilliseconds;
                        treeNode.ToolTipText = string.Format("状态：{0}\r\n耗时：{1}ms\r\n说明：{2}", ((ToolBase)L_toolList[i].tool).toolRunStatu.ToString(), elapseTime, L_toolList[i].toolTipInfo);
                    }
                    Application.DoEvents();
                }
                for (int i = toolIndex + 1; i < L_toolList.Count; i++)
                {
                    GetToolNodeByNodeText(L_toolList[i].toolName).ForeColor = Color.Black;
                }

                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        //////HOperatorSet.DumpWindowImage(out jobResultImage, item.Value.hwc_imageWindow.HalconWindow);
                    }
                }

                //在生产窗口显示图像
                //////foreach (Control item in Frm_UserForm.Instance.Controls)
                //////{
                //////    if (item.GetType().ToString() == "System.Windows.Forms.PictureBox" && item.Name == imageWindowName)
                //////    {
                //////        if (firstDisplayImage)
                //////        {
                //////            firstDisplayImage = false;
                //////            HOperatorSet.OpenWindow(0, 0, item.Size.Width, item.Size.Height, item.Handle, "visible", "", out productWindow);
                //////        }
                //////        //////Frm_ImageWindow.Instance.Display_Image(jobResultImage, imageWindow);
                //////    }
                //////}

                GetJobTree().SelectedNode = null;
                jobElapsedTime.Stop();
                double time = jobElapsedTime.ElapsedMilliseconds;
                //自动运行状态下结果不显示
                if (!initRun && debugImageWindow != "不绑定")
                {
                    bool runSucceeded = jobRunStatu == JobRunStatu.Succeed;
                    PostRunResultToUi(runSucceeded, time);
                    Project.Instance.curEngine.globelVariable.SetGlobalVariableValue(string.Format("流程[{0}].运行状态", jobName), jobRunStatu == JobRunStatu.Succeed ? "运行成功" : "运行失败");
                    Project.Instance.curEngine.globelVariable.SetGlobalVariableValue(string.Format("流程[{0}].运行时间", jobName), time);
                }
                //////if (Machine.machineRunStatu == MachineRunStatu.Running && !Configuration.SpeedMode)
                //////    Frm_Main.Instance.OutputMsg("", Color.Black);
                GC.Collect();
                Application.DoEvents();
                return L_result;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
            finally
            {
                Interlocked.Decrement(ref activeRunCount);
            }
        }

        /// <summary>
        /// 从流程第一个工具开始依次运行（图像从上往下传递），直到运行到指定工具为止
        /// </summary>
        /// <param name="toolName">目标工具名（包含该工具本身）</param>
        /// <param name="initRun">同 Run 方法，通常传 True</param>
        /// <returns></returns>
        public List<object> RunToTool(string toolName, bool initRun = false)
        {
            try
            {
                int index = -1;
                for (int i = 0; i < L_toolList.Count; i++)
                {
                    if (L_toolList[i].toolName == toolName)
                    {
                        index = i;
                        break;
                    }
                }
                if (index < 0)
                    return new List<object>();
                return Run(initRun, index);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }

        /// <summary>
        /// 供各工具窗体"运行流程"按钮调用：从流程第一个工具依次运行（图像从上往下传递），直到运行到当前工具为止，排后面的工具不执行
        /// </summary>
        public static void RunAndWaitToCurrentTool(string jobName, string toolName)
        {
            try
            {
                Job job = FindJobByName(jobName);
                if (job == null)
                    return;
                job.RunToTool(toolName, true);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void PostRunResultToUi(bool succeeded, double elapsedMs)
        {
            try
            {
                MethodInvoker display = delegate
                {
                    HTuple row, col, row1, col1;
                    HOperatorSet.GetPart(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, out row, out col, out row1, out col1);
                    Frm_Main.Instance.set_display_font(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, 15, "sans", "true", "false");
                    Frm_Main.Instance.disp_message(GetImageWindowControl().hwc_imageWindow.HWindowHalconID,
                        succeeded ? "运行成功" : "运行失败",
                        "image",
                        row + (row1 - row) / 31,
                        col + (col1 - col) / 30,
                        succeeded ? "green" : "red",
                        "false");

                    if (succeeded && !Configuration.SpeedMode)
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English
                            ? "The process was successfully run,Elapsed：" + elapsedMs + "ms"
                            : "流程 [" + jobName + "] 运行成功，耗时：" + elapsedMs + "ms", Color.Black);
                };

                if (Frm_Main.Instance.InvokeRequired)
                    Frm_Main.Instance.BeginInvoke(display);
                else
                    display();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}




