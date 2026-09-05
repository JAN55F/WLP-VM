using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using VMPro.Properties;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace VMPro
{
    internal partial class Frm_ToolBox : DockContent
    {
        internal Frm_ToolBox()
        {
            InitializeComponent();
            this.tvw_tools.ImageList = Job.imageList;
        }

        /// <summary>
        /// 用于关闭工具箱窗体后，再次打开时还能出现在原来消失时的位置，而不是每次只能出现在固定的位置
        /// </summary>
        internal static DockState lastDockState = DockState.Float;
        /// <summary>
        /// 正在拖拽的节点
        /// </summary>
        internal static TreeNode DragNode = null;
        /// <summary>
        /// 节点来源
        /// </summary>
        internal static TreeView NodeSource = null;
        /// <summary>
        /// 树形节点移动方向
        /// </summary>
        internal static MoveTreeView MoveTo = MoveTreeView.NoMove;
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_ToolBox _instance;
        internal static Frm_ToolBox Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ToolBox();
                return _instance;
            }
        }

        /// <summary>
        /// 自动连接源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoConnectSource(string jobName, TreeNode inputNode)
        {
            try
            {
                string nodeText = string.Empty;
                if (inputNode.Level == 1)
                {
                    nodeText = inputNode.Parent.Text;
                }
                else
                {
                    nodeText = inputNode.Text;
                }

                //获取最近的相同类型的输出作为源
                TreeNode sourceNode = null;
                Job job = Job.FindJobByName(jobName);
                for (int i = job.L_toolList.Count - 2; i >= 0; i--)
                {
                    for (int j = job.L_toolList[i].output.Count - 1; j >= 0; j--)
                    {
                        if (job.L_toolList[i].output[j].ioType == (DataType)inputNode.Tag)
                        {
                            sourceNode = job.GetToolIONodeByNodeText(job.L_toolList[i].toolName, "-->" + job.L_toolList[i].output[j].IOName);



                            if (sourceNode == null)
                                return;        //无可源项，返回

                            string input = inputNode.Text;
                            inputNode.Text = input + "《- " + sourceNode.Parent.Text + sourceNode.Text.Substring(1);
                            job.FindToolInfoByName(nodeText).GetInput(input.Substring(3)).value = "《- " + sourceNode.Parent.Text + sourceNode.Text.Substring(1);
                            string toolNodeText = Regex.Split("《- " + sourceNode.Parent.Text + sourceNode.Text.Substring(1), "->")[0].Substring(3);
                            string toolIONodeText = "-->" + Regex.Split("《- " + sourceNode.Parent.Text + sourceNode.Text.Substring(1), "->")[1];
                            job.D_itemAndSource.Add(inputNode, job.GetToolIONodeByNodeText(toolNodeText, toolIONodeText));
                            job.DrawLine();

                            return;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 添加工具，当toolInfo1参数为null而toolType不为空时表示添加工具，当toolType为空而toolInfo1不为空时表示粘贴工具
        /// </summary>
        /// <param name="tool">工具类型</param>
        /// <param name="toolInfo1">被复制的工具</param>
        /// <param name="isInsert">插入位置，当为-1时，表示在末尾插入，当不为-1时，表示被插入的工具索引</param>
        internal void AddTool(string toolType, ToolInfo toolInfo1, int insertIdx = -1)
        {
            try
            {
                Job.isDrawing = true;
                string jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
                string toolName = string.Empty;

                //如果当前流程正在运行，则不允许添加工具
                if (Job.FindJobByName(jobName).isRunLoop)
                {
                    Frm_Output.Instance.OutputMsg("当前流程正在运行，不可添加工具", Color.Red);
                    return;
                }

                ToolInfo toolInfo = toolInfo1;
                TreeNode toolNode = new TreeNode();

                switch (toolType)
                {
                    #region 采集图像
                    case "采集图像":
                    case "ImageAcq":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("采集图像");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            AcqImageTool imageAcqTool = new AcqImageTool();
                            toolInfo = new ToolInfo(ToolType.ImageAcq, imageAcqTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 1, 1);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 1, 1);
                        }

                        //添加常用项
                        TreeNode itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->图像", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "图像", "", DataType.Image));
                        }

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->CustomPath" : "-->自定义路径", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "CustomPath" : "自定义路径", "", DataType.String));
                        }
                        break;
                    #endregion

                    #region 预处理
                    case "预处理":
                    case "SDK_HIKViso擦擦擦n":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("预处理");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            ImageProprecessingTool imageProprecessingTool = new ImageProprecessingTool();
                            toolInfo = new ToolInfo(ToolType.ImagePreprocessing, imageProprecessingTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 1, 1);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 1, 1);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->输出图像", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输出图像", "", DataType.Image));
                        }
                        break;
                    #endregion

                    #region 彩图转RGB
                    case "彩图转RGB":
                    case "FindLinex":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("彩图转RGB");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            ColorToRGBTool colorToRGBTool = new ColorToRGBTool();
                            toolInfo = new ToolInfo(ToolType.ColorToRGB, colorToRGBTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 2, 2);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 2, 2);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            // 该节点是输入项，必须加入 input 集合；加入 output 会导致运行时找不到输入图像。
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入图像", "", DataType.Image));

                            itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->Red" : "-->红", 34, 34);
                            itemNode.ForeColor = Color.Blue;
                            itemNode.Tag = DataType.Image;
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "Red" : "红", "", DataType.Image));

                            itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->Green" : "-->绿", 34, 34);
                            itemNode.ForeColor = Color.Blue;
                            itemNode.Tag = DataType.Image;
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "Green" : "绿", "", DataType.Image));

                            itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->Blue" : "-->蓝", 34, 34);
                            itemNode.ForeColor = Color.Blue;
                            itemNode.Tag = DataType.Image;
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "Blue" : "蓝", "", DataType.Image));
                        }
                        break;
                    #endregion

                    #region 存储图像
                    case "存储图像":
                    case "FindLin信息ex":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("存储图像");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            SaveImageTool saveImageTool = new SaveImageTool();
                            toolInfo = new ToolInfo(ToolType.SaveImage, saveImageTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 39, 39);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 39, 39);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 模板匹配
                    case "模板匹配":
                    case "Match":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("模板匹配");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            MatchTool matchTool = new MatchTool();
                            toolInfo = new ToolInfo(ToolType.Match, matchTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 55, 55);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 55, 55);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->位置", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        break;
                    #endregion

                    #region 距离测量
                    case "距离测量":
                    case "Mat的说法ch":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("距离测量");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            MeasurementTool measurementTool = new MeasurementTool();
                            toolInfo = new ToolInfo(ToolType.Measurement, measurementTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 55, 55);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 55, 55);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->距离", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "距离", "", DataType.Pose));
                        }
                        break;
                    #endregion

                    #region 斑点分析
                    case "斑点分析":
                    case "BlobAnalyse":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("斑点分析");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            BlobAnalyseTool blobAnalyseTool = new BlobAnalyseTool();
                            toolInfo = new ToolInfo(ToolType.BlobAnalyse, blobAnalyseTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 10, 10);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 11, 11);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--Pose" : "<--跟随", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "Pose" : "跟随", "", DataType.Pose));
                        }
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 图像相减
                    case "图像相减":
                    case "SubImage":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("图像相减");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            SubImageTool subImageTool = new SubImageTool();
                            toolInfo = new ToolInfo(ToolType.SubImage, subImageTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 12, 12);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 12, 12);
                        }
                        break;
                    #endregion

                    #region 区域特征
                    case "区域特征":
                    case "Barxxxcode":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("区域特征");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            RegionFeatureTool regionFeatureTool = new RegionFeatureTool();
                            toolInfo = new ToolInfo(ToolType.RegionFeature, regionFeatureTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 22, 22);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 22, 22);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入区域", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Region;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入区域", "", DataType.Region));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 条码识别
                    case "条码识别":
                    case "Barcode":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("条码识别");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            BarcodeTool barcodeTool = new BarcodeTool();
                            toolInfo = new ToolInfo(ToolType.Barcode, barcodeTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 24, 24);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 24, 24);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 二维码识别
                    case "二维码识别":
                    case "Barcodexx":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("二维码识别");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            QRCodeTool qRCodeTool = new QRCodeTool();
                            toolInfo = new ToolInfo(ToolType.QRCode, qRCodeTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 25, 25);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 25, 25);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入图像", "", DataType.Image));
                        }
                        break;
                    #endregion

                    #region OCR
                    case "OCR":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("OCR");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            OCRTool oCRTool = new OCRTool();
                            toolInfo = new ToolInfo(ToolType.OCR, oCRTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 23, 23);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 23, 23);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 手眼标定
                    case "手眼标定":
                    case "EyeHandCalibration":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("手眼标定");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            EyeHandCalibTool eyeHandCalibTool = new EyeHandCalibTool();
                            toolInfo = new ToolInfo(ToolType.EyeHandCalib, eyeHandCalibTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 58, 58);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 58, 58);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->位置", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        break;
                    #endregion

                    #region 引用标定
                    case "引用标定":
                    case "EyeHandC嘻嘻嘻alibration":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("引用标定");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            QuoteTransTool quoteTransTool = new QuoteTransTool();
                            toolInfo = new ToolInfo(ToolType.QuoteTrans, quoteTransTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 58, 58);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 58, 58);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->位置", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        break;
                    #endregion

                    #region 一键手眼标定
                    case "一键手眼标定":
                    case "EyeHandCalibra信息tion":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("一键手眼标定");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            OneKeyEyeHandCalibTool oneKeyEyeHandCalibTool = new OneKeyEyeHandCalibTool();
                            toolInfo = new ToolInfo(ToolType.OneKeyEyeHandCalib, oneKeyEyeHandCalibTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 58, 58);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 58, 58);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->输出位置", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输出位置", "", DataType.Pose));
                        }
                        break;
                    #endregion

                    #region 一维标定
                    case "一维标定":
                    case "EyeHandCxalibration":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("一维标定");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            OneDimensionalCalibTool oneDimensionalCalibTool = new OneDimensionalCalibTool();
                            toolInfo = new ToolInfo(ToolType.OneDimensionalCalib, oneDimensionalCalibTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 37, 37);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 37, 37);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入值", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入值", "", DataType.String));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->输出值", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输出值", "", DataType.String));
                        }
                        break;
                    #endregion

                    #region 上相机定位
                    case "上相机定位":
                    case "RobotDownCamAlignb":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("上相机定位");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            UpCamAlignTool upCamAlignTool = new UpCamAlignTool();
                            toolInfo = new ToolInfo(ToolType.UpCamAlign, upCamAlignTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 49, 49);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 49, 49);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->位置", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        break;
                    #endregion

                    #region 下相机定位
                    case "下相机定位":
                    case "RobotDownCamAlign":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("下相机定位");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            DownCamAlignTool downCamAlignTool = new DownCamAlignTool();
                            toolInfo = new ToolInfo(ToolType.DownCamAlign, downCamAlignTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 5, 5);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 5, 5);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 点位引导
                    case "点位引导":
                    case "RobotD信息点位引导":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("点位引导");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            PointAlignTool pointAlignTool = new PointAlignTool();
                            toolInfo = new ToolInfo(ToolType.PointAlign, pointAlignTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 46, 46);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 46, 46);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 对位组装
                    case "对位组装":
                    case "RobotD信对位贴合位引导":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("对位组装");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            AlignFitTool alignFitTool = new AlignFitTool();
                            toolInfo = new ToolInfo(ToolType.AlignFit, alignFitTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 51, 51);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 51, 51);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--基板位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "基板位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--元件位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "元件位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = (toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->组装位置", 34, 34));
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "组装位置", "", DataType.Pose));
                        }
                        break;
                    #endregion

                    #region 旋转平台
                    case "旋转平台":
                    case "RobotD信息ownCamAlign":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("旋转平台");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            RotatePlatformTool rotatePlatformTool = new RotatePlatformTool();
                            toolInfo = new ToolInfo(ToolType.RotatePlatform, rotatePlatformTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 45, 45);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 45, 45);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region XY平台
                    case "XY平台":
                    case "RobotD信息ownCaDDmAlign":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("XY平台");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            XYPlatformTool xyPlatformTool = new XYPlatformTool();
                            toolInfo = new ToolInfo(ToolType.XYPlatform, xyPlatformTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 46, 46);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 46, 46);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 上相机定位
                    case "上相机定位二":
                    case "RobotDownCa大mAlignb":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("上相机定位二");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            AlignWithoutCalibRotateCenterTool alignWithoutCalibRotateCenterTool = new AlignWithoutCalibRotateCenterTool();
                            toolInfo = new ToolInfo(ToolType.AlignWithoutCalibRotateCenter, alignWithoutCalibRotateCenterTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 49, 49);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 49, 49);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->位置", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        break;
                    #endregion

                    #region 查找边
                    case "查找边":
                    case "FindLine":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("查找边");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            FindLineTool findLineTool = new FindLineTool();
                            toolInfo = new ToolInfo(ToolType.FindLine, findLineTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 6, 6);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 6, 6);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--Pose" : "<--跟随", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "Pose" : "跟随", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->线", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Line;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "线", "", DataType.Line));
                        }
                        break;
                    #endregion

                    #region 查找圆
                    case "查找圆":
                    case "FindCircle":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("查找圆");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            FindCircleTool findCircleTool = new FindCircleTool();
                            toolInfo = new ToolInfo(ToolType.FindCircle, findCircleTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 7, 7);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 7, 7);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--Pose" : "<--跟随", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "Pose" : "跟随", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->圆心", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "圆心", "", DataType.XY));
                        }

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->是否找到圆", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "是否找到圆", false, DataType.String));
                        }

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->结果圆", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Circle;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "结果圆", "", DataType.Circle));
                        }

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->圆半径", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "圆半径", 0.0, DataType.String));
                        }
                        break;
                    #endregion

                    #region 创建ROI
                    case "创建ROI":
                    case "FindCirVVVcle":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("创建ROI");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            CreateROITool createROITool = new CreateROITool();
                            toolInfo = new ToolInfo(ToolType.CreateROI, createROITool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 12, 12);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 12, 12);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->ROI", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "ROI", "", DataType.Image));
                        }
                        break;
                    #endregion

                    #region 阵列区域
                    case "阵列区域":
                    case "FindCir下限VVVcle":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("阵列区域");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            ArrayRegionTool arrayRegionTool = new ArrayRegionTool();
                            toolInfo = new ToolInfo(ToolType.ArrayRegion, arrayRegionTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 44, 44);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 44, 44);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入图像", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Image;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入图像", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->输出区域", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Region;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输出区域", "", DataType.Region));
                        }
                        break;
                    #endregion

                    #region 标记点
                    case "标记点":
                    case "FindxxCirVxxxVVcle":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("标记点");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            MarkTool markTool = new MarkTool();
                            toolInfo = new ToolInfo(ToolType.Mark, markTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 15, 15);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 15, 15);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入点", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入点", "", DataType.XY));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 组合位置
                    case "组合位置":
                    case "FindCirVxVVcle":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("组合位置");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            CreatePositionTool createPositionTool = new CreatePositionTool();
                            toolInfo = new ToolInfo(ToolType.CreatePosition, createPositionTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 13, 13);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 13, 13);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--点", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "点", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--方向", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "方向", "", DataType.Image));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->位置", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        break;
                    #endregion

                    #region 组合线段
                    case "组合线段":
                    case "FindCirVxXVVcle":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("组合线段");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            CreateLineTool createLineTool = new CreateLineTool();
                            toolInfo = new ToolInfo(ToolType.CreateLine, createLineTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 43, 43);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 43, 43);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--起点", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "起点", "", DataType.Image));
                        }

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--终点", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "终点", "", DataType.Image));
                        }

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->线", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.Line;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "线", "", DataType.Line));
                        }
                        break;
                    #endregion

                    #region 转文本
                    case "转文本":
                    case "FindxxC信息irVxxxVVcle":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("转文本");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            ToStrTool toStrTool = new ToStrTool();
                            toolInfo = new ToolInfo(ToolType.ToStr, toStrTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 33, 33);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 33, 33);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--位置", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Pose;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "位置", "", DataType.Pose));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->文本", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "文本", "", DataType.String));
                        }
                        break;
                    #endregion

                    #region 显示编辑
                    case "显示编辑":
                    case "Lab信息el":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("显示编辑");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            DisplayEditTool displayEditTool = new DisplayEditTool();
                            toolInfo = new ToolInfo(ToolType.DisplayEdit, displayEditTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 29, 29);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 29, 29);
                        }
                        break;
                    #endregion

                    #region 数据显示
                    case "数据显示":
                    case "Label":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("数据显示");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            LabelTool labelTool = new LabelTool();
                            toolInfo = new ToolInfo(ToolType.Label, labelTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 60, 60);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 60, 60);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入项1", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入项1", "", DataType.String));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入项2", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入项2", "", DataType.String));
                        }

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入项3", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入项3", "", DataType.String));
                        }

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入项4", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入项4", "", DataType.String));
                        }

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入项5", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入项5", "", DataType.String));
                        }
                        break;
                    #endregion

                    #region 点点距离
                    case "点点距离":
                    case "DistancePS":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("点点距离");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            DistancePPTool distancePPTool = new DistancePPTool();
                            toolInfo = new ToolInfo(ToolType.DistancePP, distancePPTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 16, 16);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 16, 16);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--起点", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "起点", "", DataType.XY));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--终点", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "终点", "", DataType.XY));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->距离", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "距离", "", DataType.String));
                        }
                        break;
                    #endregion

                    #region 点线距离
                    case "点线距离":
                    case "Distancec PS":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("点线距离");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            DistancePLTool distancePLTool = new DistancePLTool();
                            toolInfo = new ToolInfo(ToolType.DistancePL, distancePLTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 17, 17);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 17, 17);
                        }
                        break;
                    #endregion

                    #region 线线距离
                    case "线线距离":
                    case "DistanceSS":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("线线距离");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            DistanceLLTool distanceLLTool = new DistanceLLTool();
                            toolInfo = new ToolInfo(ToolType.DistanceSS, distanceLLTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 19, 19);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 19, 19);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--线1", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Line;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "线1", "", DataType.Line));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--线段2", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Line;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "线段2", "", DataType.Line));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->结果距离值", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "结果距离值", "", DataType.String));
                        }
                        break;
                    #endregion

                    #region 线线交点
                    case "线线交点":
                    case "DistanceSSxxx":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("线线交点");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            LLIntersectTool lLIntersectTool = new LLIntersectTool();
                            toolInfo = new ToolInfo(ToolType.LLIntersect, lLIntersectTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 21, 21);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 21, 21);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--线1", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Line;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "线1", "", DataType.Line));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--线2", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Line;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "线2", "", DataType.Line));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->点", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "点", "", DataType.XY));
                        }
                        break;
                    #endregion

                    #region 线线角度
                    case "线线角度":
                    case "Distance想想SSxxx":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("线线角度");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            AngleLLTool angleLLTool = new AngleLLTool();
                            toolInfo = new ToolInfo(ToolType.AngleLL, angleLLTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 21, 21);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 21, 21);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--线1", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Line;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "线1", "", DataType.Line));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--线2", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.Line;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "线2", "", DataType.Line));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->角度", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "角度", "", DataType.String));
                        }
                        break;
                    #endregion

                    #region 两点中点
                    case "两点中点":
                    case "DistancexxSSxxx":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("两点中点");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            CenterOfPP centerOfPP = new CenterOfPP();
                            toolInfo = new ToolInfo(ToolType.CenterOfPP, centerOfPP, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 20, 20);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 20, 20);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--点1", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.XY;
                        toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "点1", "", DataType.XY));

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--点2", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "点2", "", DataType.XY));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "-->中点", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "中点", "", DataType.XY));
                        }
                        break;
                    #endregion

                    #region 数据分析
                    case "数据分析":
                    case "Cod数据分析eEdit":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("数据分析");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            DataAnalyseTool dataAnalyseTool = new DataAnalyseTool();
                            toolInfo = new ToolInfo(ToolType.DataAnalyse, dataAnalyseTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 26, 26);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 26, 26);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--输入项1", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输入项1", "", DataType.String));

                        itemNode = (toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->输出项1", 34, 34));
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "输出项1", "", DataType.String));
                        }
                        break;
                    #endregion

                    #region 脚本编辑
                    case "脚本编辑":
                    case "CodeEdit":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("脚本编辑");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            CodeEditTool codeEditTool = new CodeEditTool();
                            toolInfo = new ToolInfo(ToolType.CodeEdit, codeEditTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 26, 26);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 26, 26);
                        }

                        break;
                    #endregion

                    #region 光源_奥普特
                    case "光源_奥普特":
                    case "Barcoxxdexxxx":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("光源_奥普特");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            Light_OPTTool light_OPTTool = new Light_OPTTool();
                            toolInfo = new ToolInfo(ToolType.Light_OPT, light_OPTTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 27, 27);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 27, 27);
                        }
                        break;
                    #endregion

                    #region 光源控制
                    case "光源控制":
                    case "Barcoxxdexxxxxx":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("光源控制");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            OptLightControlTool optLightControlTool = new OptLightControlTool();
                            toolInfo = new ToolInfo(ToolType.OPTLightControl, optLightControlTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 27, 27);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 27, 27);
                        }
                        break;
                    #endregion

                    #region 基恩士
                    case "扫码器_基恩士":
                    case "Barcodexxxx":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("扫码器_基恩士");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            Scaner_KenyenceTool scaner_KenyenceTool = new Scaner_KenyenceTool();
                            toolInfo.toolType = ToolType.Scaner_Kenyence;
                            toolInfo = new ToolInfo(ToolType.Scaner_Kenyence, scaner_KenyenceTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 28, 28);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 28, 28);
                        }
                        break;
                    #endregion

                    #region 以太网接收
                    case "以太网接收":
                    case "SDK_H嘻嘻嘻IKVison":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("以太网接收");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            EthernetReceiveTool ethernetReceiveTool = new EthernetReceiveTool();
                            toolInfo = new ToolInfo(ToolType.EthernetReceive, ethernetReceiveTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 57, 57);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 57, 57);
                        }
                        break;
                    #endregion

                    #region 以太网发送
                    case "以太网发送":
                    case "SDK_H嘻嘻嘻的IKVison":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("以太网发送");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            EthernetSendTool ethernetSendTool = new EthernetSendTool();
                            toolInfo = new ToolInfo(ToolType.EthernetSend, ethernetSendTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 56, 56);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 56, 56);
                        }

                        //添加常用项
                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "<--消息", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "消息", "", DataType.String));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);
                        break;
                    #endregion

                    #region 点补偿
                    case "点补偿":
                    case "Barcoxxxxdexx":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("点补偿");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            BuChangTool buChangTool = new BuChangTool();
                            toolInfo = new ToolInfo(ToolType.Scaner_Kenyence, buChangTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 33, 33);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 33, 33);
                        }
                        break;
                    #endregion

                    #region 点偏差
                    case "点偏差":
                    case "PointOffset":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("点偏差");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        if (toolInfo1 == null)
                        {
                            PointOffsetTool pointOffsetTool = new PointOffsetTool();
                            toolInfo = new ToolInfo(ToolType.PointOffset, pointOffsetTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 47, 47);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 47, 47);
                        }

                        //添加常用项

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "<--OutputImage" : "<--点", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "点", "", DataType.XY));
                        }
                        //自动链接输入项
                        AutoConnectSource(jobName, itemNode);

                        itemNode = toolNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "-->OutputImage" : "-->点", 34, 34);
                        itemNode.ForeColor = Color.Blue;
                        itemNode.Tag = DataType.XY;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO(Project.Instance.configuration.language == Language.English ? "OutputImage" : "点", "", DataType.XY));
                        }
                        break;
                    #endregion

                    #region 输出项
                    case "输出项":
                    case "Output":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("输出项");
                        if (toolName == "TooMuch")       //此工具添加个数已达到上限100各，不让继续添加
                            return;

                        //输出工具只允许添加一个，所以此处要判断是否已经存在了
                        if (Job.FindJobByName(jobName).ExistOutputTool())
                        {
                            Frm_MessageBox.Instance.MessageBoxShow("\r\n输出工具已存在，此工具最多只能添加一个");
                            return;
                        }
                        if (toolInfo1 == null)
                        {
                            OutputTool outputTool = new OutputTool();
                            toolInfo = new ToolInfo(ToolType.Output, outputTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 59, 59);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 59, 59);
                        }
                        break;
                    #endregion

                    #region PLC通讯
                    case "PLC通讯":
                        toolName = Job.FindJobByName(jobName).GetNewToolName("PLC通讯");
                        if (toolName == "TooMuch")
                            return;

                        if (toolInfo1 == null)
                        {
                            PLCCommTool plcCommTool = new PLCCommTool();
                            toolInfo = new ToolInfo(ToolType.PLCComm, plcCommTool, jobName, toolName);
                        }
                        else
                        {
                            toolInfo1.toolName = toolName;
                            for (int i = 0; i < toolInfo1.input.Count; i++)
                            {
                                toolInfo1.input[i].value = string.Empty;
                            }
                        }

                        if (insertIdx == -1)
                        {
                            Job.FindJobByName(jobName).L_toolList.Add(toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Add("", toolInfo.toolName, 33, 33);
                        }
                        else
                        {
                            Job.FindJobByName(jobName).L_toolList.Insert(insertIdx, toolInfo);
                            toolNode = Job.GetJobTree(jobName).Nodes.Insert(insertIdx, "", toolName, 33, 33);
                        }

                        // 写入值：输入项（紫色=来自上游）
                        itemNode = toolNode.Nodes.Add("", "<--写入值", 34, 34);
                        itemNode.ForeColor = Color.DarkMagenta;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.input.Add(new ToolIO("写入值", "", DataType.String));
                        }

                        // 读取值：输出项（绿色=流向下游）
                        itemNode = toolNode.Nodes.Add("", "-->读取值", 35, 35);
                        itemNode.ForeColor = Color.DarkGreen;
                        itemNode.Tag = DataType.String;
                        if (toolInfo1 == null)
                        {
                            toolInfo.output.Add(new ToolIO("读取值", "", DataType.String));
                        }

                        break;
                    #endregion

                    default:
                        Frm_MessageBox.Instance.MessageBoxShow("\r\n此工具尚未开发！");
                        return;
                }
                tvw_tools.SelectedNode = toolNode;
                toolNode.Expand();
                toolNode.EnsureVisible();
                toolNode.ToolTipText = Project.Instance.configuration.language == Language.English ? "Not_Start_Run" : "未运行";
                Job.isDrawing = false;
                Job.FindJobByName(jobName).DrawLine();
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void Frm_Tools_Load(object sender, EventArgs e)
        {
            try
            {
                //图像相关
                TreeNode ImageNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "AcqDevice" : "图像相关", 0, 0);
                {
                    ImageNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "SDK_HIKVision" : "采集图像", 1, 1);
                    ImageNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "ShapeMatch" : "预处理", 38, 38);
                    ImageNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "ColorToRGB" : "彩图转RGB", 2, 2);
                    ImageNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "GrayMatch" : "存储图像", 39, 39);
                }

                //检测识别
                TreeNode DetectNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Match" : "检测识别", 0, 0);
                {
                    DetectNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "ShapeMatch" : "模板匹配", 55, 55);
                    DetectNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "BlobAnalyse" : "距离测量", 33, 33);
                    DetectNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "BlobAnalyse" : "斑点分析", 10, 10);
                    DetectNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "SubImage" : "图像相减", 11, 11);
                    DetectNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "SubImage" : "区域特征", 22, 22);
                    DetectNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Barcode" : "条码识别", 24, 24);
                    DetectNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "2DUncode" : "二维码识别", 25, 25);
                    DetectNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "OCRTool" : "OCR", 23, 23);
                    DetectNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "OCVTool" : "OCV", 48, 48);
                }

                //标定
                TreeNode CalibNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Calibration" : "坐标变换", 0, 0);
                {
                    CalibNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "EyeHandCalibration" : "手眼标定", 58, 58);
                    CalibNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "EyeHandCalibration" : "引用标定", 58, 58);
                    CalibNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "EyeHandCalibration" : "一键手眼标定", 58, 58);
                    CalibNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "CircleCalibration" : "尺寸标定", 40, 40);
                    CalibNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "RectCalibration" : "一维标定", 37, 37);
                }

                //定位引导
                TreeNode AlignNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "RobotAlign" : "定位引导", 0, 0);
                {
                    AlignNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "RobotUpCameraAlign(EyeHandSeparation)" : "上相机定位", 49, 49);
                    AlignNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "RobotDownCamAlign" : "下相机定位", 50, 50);
                    AlignNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "RobotDownCamAlign" : "点位引导", 46, 46);
                    AlignNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "RobotDownCamAlign" : "对位组装", 51, 51);
                    AlignNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "RobotDownCamAlign" : "上相机定位二", 46, 46);
                    AlignNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindLine" : "点偏差", 47, 47);

                }

                //逻辑控制
                TreeNode LogicNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Detection" : "逻辑控制", 0, 0);
                {
                    LogicNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "SubImage" : "循环", 52, 52);
                    LogicNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "SubImage" : "分支", 53, 53);
                }

                //查找拟合
                TreeNode FindAndFitNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindAndFit" : "查找拟合", 0, 0);
                {
                    FindAndFitNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindLine" : "查找边", 6, 6);
                    FindAndFitNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindCircle" : "查找圆", 7, 7);
                    FindAndFitNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FitLine" : "拟合线", 8, 8);
                    FindAndFitNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FitCircle" : "拟合圆", 9, 9);
                }

                //创建组合
                TreeNode CreateNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindAndFit" : "创建组合", 0, 0);
                {
                    CreateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindLine" : "创建ROI", 12, 12);
                    CreateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindLine" : "阵列区域", 14, 14);
                    CreateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindLine" : "标记点", 15, 15);
                    CreateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindLine" : "组合位置", 13, 13);
                    CreateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindLine" : "组合线段", 43, 43);
                    CreateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindLine" : "转文本", 33, 33);
                    CreateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Label" : "显示编辑", 29, 29);
                    CreateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Label" : "数据显示", 60, 60);
                }

                //几何相关
                TreeNode GeometryNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Measurement" : "几何相关", 0, 0);
                {
                    GeometryNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "DistancePP" : "点点距离", 16, 16);
                    GeometryNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "DistancePL" : "点线距离", 17, 17);
                    GeometryNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "AngleLL" : "线线角度", 18, 18);
                    GeometryNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "DistanceSS" : "线线距离", 19, 19);
                    GeometryNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "DistanceSS" : "线线交点", 21, 21);
                    GeometryNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "DistanceSS" : "两点中点", 20, 20);
                }

                //运算
                TreeNode CalculateNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Operation" : "运算相关", 0, 0);
                {
                    TreeNode ArithmeticNode = CalculateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Arithmetic" : "算术", 33, 33);
                    TreeNode CSharpCodeEdit = CalculateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "CodeEdit" : "脚本编辑", 26, 26);
                    TreeNode DataAnalyse = CalculateNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "CodeEdit" : "数据分析", 26, 26);
                }

                //仪器仪表
                TreeNode LightNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light" : "仪器仪表", 0, 0);
                {
                    LightNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light_OPT" : "光源_奥普特", 27, 27);
                    LightNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light_CST" : "光源_康视达", 27, 27);
                    LightNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light_LOTS" : "光源_乐视", 27, 27);
                    LightNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light_OPT" : "光源控制", 27, 27);
                    LightNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "OCRTool" : "扫码器_基恩士", 54, 54);
                }

                //通讯相关
                TreeNode CommNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light" : "通讯相关", 0, 0);
                {
                    CommNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light_OPT" : "PLC通讯", 33, 33);
                    CommNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light_OPT" : "以太网接收", 57, 57);
                    CommNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light_OPT" : "以太网发送", 56, 56);
                }

                //3D检测
                TreeNode D3Node = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "3D" : "3D  检测", 0, 0);

                //其它
                TreeNode ElseNode = tvw_tools.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Light" : "其它相关", 0, 0);
                {
                    ElseNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "FindLine" : "点补偿", 33, 33);
                    ElseNode.Nodes.Add("", Project.Instance.configuration.language == Language.English ? "Output" : "输出项", 59, 59);
                }

                //默认展开第一个图像相关节点
                this.tvw_tools.Nodes[0].Expand();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tvw_job_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                switch (tvw_tools.SelectedNode.Text)
                {
                    case "图像相关":
                    case "AcqDevice":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for image acquisition" : "说明：此类工具用于图像的获取和相关处理");
                        break;

                    case "采集图像":
                    case "SDK_HIKVision":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the Hikvision Camera SDK Drawing Tool" : "说明：此工具用于对各品牌相机连接和采图");
                        break;

                    case "SDK_Halcon":
                    case "HalconAcqInterface":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool supports two modes of reading images locally and collecting images from devices, which can be switched freely" : "说明：此工具基于Halcon采集接口连接相机并采图");
                        break;

                    case "预处理":
                    case "SDK_HIKVisixxon":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the Hikvision Camera SDK Drawing Tool" : "说明：此工具用于图像的预处理，可改善图像质量");
                        break;

                    case "彩图转RGB":
                    case "ChannelConvert":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool can divide the color map into three color single channel maps" : "说明：此工具用于将彩色图像分解成RGB单通道图像");
                        break;

                    case "存储图像":
                    case "SDK_HIKxxxVisixxon":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the Hikvision Camera SDK Drawing Tool" : "说明：此工具用于将图像存储到本地磁盘");
                        break;

                    case "检测识别":
                    case "Match":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This type of tool is used for template matching to locate" : "说明：此类工具用于对图像中的特征进行检测和识别");
                        break;

                    case "模板匹配":
                    case "ShapeMatch":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for shape matching positioning" : "说明：此工具为模板匹配工具");
                        break;

                    case "斑点分析":
                    case "BlobAnalyse":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for spot analysis" : "说明：此工具用于对图像中的斑点进行分析");
                        break;

                    case "图像相减":
                    case "SubImage":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool performs feature detection or recognition by subtracting two images" : "说明：此工具使用两图像相减的方式进行特征检测或识别");
                        break;

                    case "区域特征":
                    case "Iden显现显现tity":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used to identify" : "说明：此工具用于获取区域的相关特征，如圆度、面积等");
                        break;

                    case "条码识别":
                    case "Barcode":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for bar code identification" : "说明：此工具用于条码识别");
                        break;

                    case "二维码识别":
                    case "2DUncode":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for two-dimensional code identification" : "说明：此工具用于二维码识别");
                        break;

                    case "OCR":
                    case "OCRTool":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for OCR recognition" : "说明：此工具用于字符的识别");
                        break;

                    case "OCV":
                    case "OCVTool":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for OCV identification" : "说明：此工具用于OCV验证");
                        break;

                    case "标定变换":
                    case "CoorTrans":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This type of tool is used for coordinate system transformation" : "说明：此类工具用于标定获取两个坐标系之间的关系及进行关系转化");
                        break;

                    case "手眼标定":
                    case "EyeHandCalibration":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to calibrate the manipulator and camera" : "说明：此工具用于手动的标定图像坐标系和机械坐标系之间的关系");
                        break;

                    case "引用标定":
                    case "EyeHandCalibra香香tion":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to calibrate the manipulator and camera" : "说明：此工具用于引用其它标定工具中的标定关系");
                        break;

                    case "一键手眼标定":
                    case "EyeHandCalibra信息tion":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to calibrate the manipulator and camera" : "说明：此工具用于一键自动的完成图像坐标系和机械坐标系的标定");
                        break;

                    case "尺寸标定":
                    case "Calibration":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Dimension calibration of such tools" : "说明：此工具用于尺寸标定，可通过标准原点或正方形等方式完成标定");
                        break;

                    case "一维标定":
                    case "Calibrati嘻嘻嘻on":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Dimension calibration of such tools" : "说明：此工具用于标定出两条一维坐标系之间的平移和缩放关系");
                        break;

                    case "定位引导":
                    case "RobotAlign":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are related tools in manipulator positioning applications" : "说明：此类工具用于机械手的定位引导");
                        break;

                    case "上相机定位":
                    case "RobotUpCameraAlign":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for positioning, grabbing and placing the camera fixed on the robot hand" : "说明：此工具用于相机固定于产品上方，拍照定位引导机械手抓取的场景");
                        break;

                    case "下相机定位":
                    case "RobotDownCamAlign":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is an application tool for camera positioning under the machine" : "说明：此工具用于相机固定于底部，机械手抓取产品后经此相机拍照定位的场景");
                        break;

                    case "点位引导":
                    case "RobotDoDw信谢谢息nCamAlign":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is an application tool for camera positioning under the machine" : "说明：此工具应用于相机固定于机械手臂上，拍照定位引导机械手运动到指定点的场景");
                        break;

                    case "对位组装":
                    case "RobotDoDw信谢谢息nC谢谢amAlign":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is an application tool for camera positioning under the machine" : "说明：此工具用于上下相机组合使用的对位组装或贴合类场景");
                        break;

                    case "旋转平台":
                    case "RobotDow信息nCamAlign":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is an application tool for camera positioning under the machine" : "说明：此工具应用于相机固定于待定位产品上方，产品放置于旋转平台上，相机定位后，通过旋转平台和XY模组完成取料的场景");
                        break;

                    case "XY平台":
                    case "RobotDoDw信息nCamAlign":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is an application tool for camera positioning under the machine" : "说明：此工具应用于相机固定于待定位产品上方，产品放置于旋转平台上，相机定位后，通过旋转平台和XY模组完成取料的场景");
                        break;

                    case "逻辑控制":
                    case "ThresholdSegmenta嘻嘻嘻tion":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This type of tool processes based on gray values" : "说明：此类工具用于流程中的逻辑控制，如循环、分支等");
                        break;

                    case "循环":
                    case "ThresholdSeg信息menta嘻嘻嘻tion":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This type of tool processes based on gray values" : "说明：此工具用于循环执行");
                        break;

                    case "分支":
                    case "ThresholdS信息eg信息menta嘻嘻嘻tion":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This type of tool processes based on gray values" : "说明：此工具用于分支选择执行");
                        break;

                    case "查找拟合":
                    case "FindAndFit":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This type of tool is used for finding and fitting" : "说明：此类工具用于查找和拟合");
                        break;

                    case "查找边":
                    case "FindLineTool":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to find straight lines" : "说明：此工具用于查找边");
                        break;

                    case "查找圆":
                    case "FindCircle":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to find circles" : "说明：此工具用于查找圆");
                        break;

                    case "拟合线":
                    case "FitLine":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to fit straight lines" : "说明：此工具用于拟合直线");
                        break;

                    case "拟合圆":
                    case "FitCircle":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to fit circles" : "说明：此工具用于拟合圆");
                        break;

                    case "创建组合":
                    case "FitCirc谢谢le":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to fit circles" : "说明：此类工具用于创建或组合一些类型");
                        break;

                    case "创建ROI":
                    case "ImageConveXXXrt":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for image conversion, such as color image conversion to gray image, etc" : "说明：此工具用于创建一个较复杂的ROI区域");
                        break;

                    case "阵列区域":
                    case "ImageConve信息信息信息XXXrt":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for image conversion, such as color image conversion to gray image, etc" : "说明：此工具用于创建阵列网格式区域");
                        break;

                    case "标记点":
                    case "ImageConve信信息息信息信息XXXrt":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for image conversion, such as color image conversion to gray image, etc" : "说明：此工具用于在图像上指定位置显示一个标记点");
                        break;

                    case "组合位置":
                    case "ImageConve信息XXXrt":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for image conversion, such as color image conversion to gray image, etc" : "说明：此工具可由一个点和一个方向组合出一个XYU位置类型");
                        break;

                    case "组合线段":
                    case "ImageConve信息信息XXXrt":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for image conversion, such as color image conversion to gray image, etc" : "说明：此工具用于通过两个点组合出一条线段");
                        break;

                    case "转文本":
                    case "ImageConve信息xx XXXrt":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for image conversion, such as color image conversion to gray image, etc" : "说明：此工具用于将点或位置类型转化成文本类型");
                        break;

                    case "数据显示":
                    case "Label":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to display string content on an image" : "说明：此工具用于在图像窗口上显示文本信息");
                        break;

                    case "几何相关":
                    case "Segm显现ent":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for measurement" : "说明：此类工具为几何类工具");
                        break;

                    case "点点距离":
                    case "DistancePP":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for point-to-point distance measurement" : "说明：此工具用于计算点与点之间的距离");
                        break;

                    case "点线距离":
                    case "DistancePL":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for measuring distance between points and lines" : "说明：此工具用于计算点与线段之间的距离");
                        break;

                    case "线线角度":
                    case "AngleLL":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for line and line angle measurement" : "说明：此工具用于计算线段与线段之间的角度");
                        break;

                    case "线线距离":
                    case "DistanceLL":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for line-to-line distance measurement" : "说明：此工具用于计算线段与线段之间的距离");
                        break;

                    case "线线交点":
                    case "Segment":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for measurement" : "说明：此工具用于计算线段与线段的交点");
                        break;

                    case "两点中点":
                    case "Segm嘻嘻嘻ent":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for measurement" : "说明：此工具用于计算两点的中点");
                        break;

                    case "运算":
                    case "Operation":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This type of tool is used for arithmetic processing" : "说明：此类工具用于运算处理");
                        break;

                    case "算术":
                    case "Arithemtic":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used for mathematical arithmetic operations" : "说明：此工具用于数学算术运算");
                        break;

                    case "脚本编辑":
                    case "CodeEdit":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to edit CSharp scripts" : "说明：此工具用于编辑CSharp脚本");
                        break;

                    case "仪器仪表":
                    case "Iden谢谢显现tity":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used to identify" : "说明：此类工具用于控制市场上常用的第三方仪表，如扫码枪、光源控制器等");
                        break;

                    case "光源_奥普特":
                    case "Light_OPT":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the control tool of OPT Lighting controller" : "说明：此工具用于奥普特光源控制器");
                        break;

                    case "光源_康视达":
                    case "Light_LOTS":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the control tool of Conspicuous Lighting controller" : "说明：此工具用于康视达光源控制器");
                        break;

                    case "光源_乐视":
                    case "Light_CST":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is Letv Lighting controller's control tool" : "说明：此工具用于乐视光源控制器");
                        break;

                    case "光源控制":
                    case "Ligh显现t_LOTS":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the control tool of Conspicuous Lighting controller" : "说明：此工具可以对光源进行控制，如控制光源的开关、亮度等");
                        break;

                    case "扫码器_基恩士":
                    case "Ligh显现得到显现t_LOTS":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the control tool of Conspicuous Lighting controller" : "说明：此工具用于对基恩士扫码器进行控制，如SR700、SR1000等");
                        break;

                    case "通讯相关":
                    case "Ligh显现得到显现t_LO显现TS":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the control tool of Conspicuous Lighting controller" : "说明：此类工具用于本软件和外部设备之间的通讯");
                        break;

                    case "PLC通讯":
                    case "Ligh显现得到显信息现t_LO显现TS":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the control tool of Conspicuous Lighting controller" : "说明：此工具用于和各品牌PLC的通讯，如寄存器读写、Socket收发等");
                        break;

                    case "3D  检测":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Such tools are used for 3D processing" : "说明：此类工具用于3D相关检测或测量");
                        break;

                    case "其它相关":
                    case "Ligh显现得到显显现现t_LO显现TS":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is the control tool of Conspicuous Lighting controller" : "说明：其它相关工具");
                        break;

                    case "输出项":
                    case "Output":
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：This tool is used to add related items to the output collection for external invocation and retrieval" : "说明：此工具用于将相关项添加到输出集合，用于外部调用或获取");
                        break;

                    default:
                        lbl_toolInfo.Text = (Project.Instance.configuration.language == Language.English ? "Notes：Unknown" : "说明：未知");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tvw_job_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Developer))
                    return;
                if (tvw_tools.SelectedNode.SelectedImageIndex == 0)         //如果双击的是文件夹节点，返回
                    return;
                if (Project.Instance.curEngine.L_jobList.Count > 0)        //如果流程存在
                    AddTool(tvw_tools.SelectedNode.Text, null);
                else           //如果当前不存在可用流程，先创建流程，再添加工具
                    Job.CreateJob();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void 折叠所有ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvw_tools.CollapseAll();
        }
        private void 展开所有ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                tvw_tools.ExpandAll();
                tvw_tools.SelectedNode = tvw_tools.Nodes[0].Nodes[0];
                tvw_tools.AutoScrollOffset = new System.Drawing.Point(0, 0);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tvw_tools_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                TreeNode tn = tvw_tools.GetNodeAt(e.X, e.Y);
                if (tn != null)
                    tvw_tools.SelectedNode = tn;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_Tools_DockStateChanged(object sender, EventArgs e)
        {
            if (Frm_ToolBox.Instance.DockState != DockState.Unknown)
                lastDockState = Frm_ToolBox.Instance.DockState;
        }
        private void Frm_Tools_FormClosed(object sender, FormClosedEventArgs e)
        {
            _instance = null;
        }
        private void tvw_tools_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (sender != null && sender is TreeView)
                {
                    TreeView trv = sender as TreeView;
                    if (trv.Tag != null)
                    {
                        MoveTreeView move = (MoveTreeView)Convert.ToInt32(trv.Tag);
                        if (move == MoveTo) { DragNode = null; NodeSource = null; }
                        else
                        {
                            System.Drawing.Point point = trv.PointToClient(new System.Drawing.Point(e.X, e.Y));
                            TreeNode node = trv.GetNodeAt(point);
                            node.Nodes.Add(DragNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tvw_tools_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                if (tvw_tools.SelectedNode.Level == 0)          //工具箱层级不应该被拖动，直接返回
                    return;

                if (e.Item is TreeNode && e.Button == System.Windows.Forms.MouseButtons.Left &&
                  e.Item != null && sender is TreeView)
                {
                    TreeView trv = sender as TreeView;
                    TreeNode node = e.Item as TreeNode;
                    int value = Convert.ToInt32(trv.Tag);
                    MoveTo = (MoveTreeView)value;
                    DragNode = node;
                    NodeSource = trv;
                    trv.DoDragDrop(node, DragDropEffects.Move);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tvw_tools_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

    }
    /// <summary>
    /// 属性节点移动方向
    /// </summary>
    public enum MoveTreeView
    {
        /// <summary>
        /// 未移动
        /// </summary>
        NoMove = -1,
        /// <summary>
        /// 上传（客户端拖拽到服务器端）
        /// </summary>
        ClientToServer = 0,
        /// <summary>
        /// 下载（服务器端拖拽到客户端）
        /// </summary>
        ServerToClient = 1
    }
}
