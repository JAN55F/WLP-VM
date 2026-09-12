using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HalconDotNet;
using System.Windows.Forms;
using System.Drawing;

namespace VMPro
{
    /// <summary>
    /// 数据显示工具：把流程中前置工具的输出（或全局变量）以"行/列/颜色/字号"标注到图像窗口。
    /// 每一行可链接一个数据源，并按行选择条件判定方式：
    /// 无（字符串，恒 OK）/ 范围（数值上下限，允许留空=单边不限）/ 布尔（true=OK、false=NG）。
    /// 行链接在窗体保存时写入 ToolInfo.input（IOName="输入项N"，value="《- 源->输出"），
    /// 与流程编辑器的连线（D_itemAndSource/DrawLine）联动。
    /// </summary>
    [Serializable]
    internal class LabelTool : ToolBase
    {
        public LabelTool()
        {
            L_label.Add(MakeDefaultLabel());
        }

        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 工具名（打开窗体时由流程编辑器赋值，保存/链接同步用）
        /// </summary>
        internal string toolName = string.Empty;
        /// <summary>
        /// 标签项集合
        /// </summary>
        internal List<Label> L_label = new List<Label>();
        /// <summary>
        /// 输入项和值（Job.Run 的 Label 分支在运行期填充）
        /// </summary>
        internal Dictionary<string, string> D_inputItemAndVlaue = new Dictionary<string, string>();

        /// <summary>
        /// 行数据载体（窗体网格 ↔ 工具），不序列化。
        /// </summary>
        internal class LabelRowData
        {
            internal string SourceTool = string.Empty;      // 空=未链接
            internal string SourceOutput = string.Empty;
            internal string LinkDisplay = string.Empty;     // "工具名.输出结果名" / "全局变量.变量名"
            internal string Row = "100";
            internal string Col = "100";
            internal string OkColor = "green";
            internal string NgColor = "red";
            internal string FontSize = "10";
            internal string JudgeMode = "None";             // None / Range / Bool
            internal string Down = string.Empty;
            internal string Up = string.Empty;
        }

        internal static Label MakeDefaultLabel()
        {
            Label label = new Label();
            label.Row = "100";
            label.Col = "100";
            label.ExpectValue = string.Empty;
            label.Incolor = "green";
            label.OutColor = "red";
            label.OutputItem = "InputItem1";
            label.PreAddStr = string.Empty;
            label.Size = "10";
            label.DownLimit = string.Empty;
            label.UpLimit = string.Empty;
            label.ValueType = "Str";
            label.JudgeMode = "None";
            return label;
        }

        /// <summary>
        /// 条件判定（纯函数）：返回该行是否 OK。
        /// None：恒 OK；Bool：true=OK、false=NG（解析失败按 OK 显示原值）；
        /// Range：数值比较，下限/上限留空=该侧不限制，非数值内容不判定。
        /// </summary>
        internal static bool Judge(string judgeMode, string value, string downLimit, string upLimit)
        {
            string mode = string.IsNullOrEmpty(judgeMode) ? "None" : judgeMode;
            if (mode == "Bool")
            {
                bool parsed;
                if (bool.TryParse(value, out parsed))
                    return parsed;
                if (value == "1") return true;
                if (value == "0") return false;
                return true;
            }
            if (mode == "Range")
            {
                double v;
                if (!double.TryParse(value, out v))
                    return true;
                double bound;
                if (!string.IsNullOrEmpty(downLimit) && double.TryParse(downLimit, out bound) && v < bound)
                    return false;
                if (!string.IsNullOrEmpty(upLimit) && double.TryParse(upLimit, out bound) && v > bound)
                    return false;
                return true;
            }
            return true;
        }

        /// <summary>
        /// 旧数据迁移：JudgeMode 为空时按旧 ValueType 推导（Value→Range、Str→None）。
        /// </summary>
        private static string EffectiveJudgeMode(Label label)
        {
            if (!string.IsNullOrEmpty(label.JudgeMode))
                return label.JudgeMode;
            return label.ValueType == "Value" ? "Range" : "None";
        }

        /// <summary>
        /// 工具 → 窗体：从 L_label 与 input 条目导出网格行（LinkDisplay 由链接串推导）。
        /// </summary>
        internal List<LabelRowData> ExportRows()
        {
            List<LabelRowData> rows = new List<LabelRowData>();
            ToolInfo info = FindToolInfoSafe();
            for (int i = 0; i < L_label.Count; i++)
            {
                Label label = L_label[i];
                LabelRowData row = new LabelRowData();
                row.Row = label.Row;
                row.Col = label.Col;
                row.OkColor = string.IsNullOrEmpty(label.Incolor) ? "green" : label.Incolor;
                row.NgColor = string.IsNullOrEmpty(label.OutColor) ? "red" : label.OutColor;
                row.FontSize = string.IsNullOrEmpty(label.Size) ? "10" : label.Size;
                row.JudgeMode = EffectiveJudgeMode(label);
                row.Down = label.DownLimit ?? string.Empty;
                row.Up = label.UpLimit ?? string.Empty;

                // 由 OutputItem("InputItemN") 找到对应 input 条目，推导链接
                ToolIO entry = FindInputEntry(info, i);
                if (entry != null)
                    ParseLink(entry.value == null ? string.Empty : entry.value.ToString(), row);
                rows.Add(row);
            }
            return rows;
        }

        /// <summary>
        /// 窗体 → 工具：按网格行重建 L_label，并同步 ToolInfo.input 条目、
        /// 流程树输入节点与连线（D_itemAndSource + DrawLine）——弹窗关闭后流程编辑器自动连线。
        /// </summary>
        internal void ImportRows(List<LabelRowData> rows)
        {
            L_label.Clear();
            for (int i = 0; i < rows.Count; i++)
            {
                LabelRowData row = rows[i];
                Label label = new Label();
                label.OutputItem = "InputItem" + (i + 1);
                label.Row = row.Row;
                label.Col = row.Col;
                label.Incolor = row.OkColor;
                label.OutColor = row.NgColor;
                label.Size = row.FontSize;
                label.JudgeMode = string.IsNullOrEmpty(row.JudgeMode) ? "None" : row.JudgeMode;
                label.DownLimit = label.JudgeMode == "Range" ? (row.Down ?? string.Empty) : string.Empty;
                label.UpLimit = label.JudgeMode == "Range" ? (row.Up ?? string.Empty) : string.Empty;
                label.ValueType = label.JudgeMode == "None" ? "Str" : "Value";
                label.ExpectValue = string.Empty;
                label.PreAddStr = string.Empty;
                L_label.Add(label);
            }
            SyncInputs(rows);
        }

        /// <summary>
        /// 运行工具：按每行判定方式着色，把值显示在图像的行/列位置。
        /// </summary>
        public override void Run(bool updateImage, bool debugTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    for (int i = 0; i < L_label.Count; i++)
                    {
                        Label label = L_label[i];
                        Frm_ImageWindow window = GetImageWindowControl();
                        if (window == null)
                            continue;

                        string raw;
                        if (!D_inputItemAndVlaue.TryGetValue(label.OutputItem, out raw))
                            raw = string.Empty;

                        // 行/列未填或非数字时跳过该行显示，不中断整个工具
                        int displayRow, displayCol;
                        if (!int.TryParse(label.Row, out displayRow) || !int.TryParse(label.Col, out displayCol))
                            continue;

                        bool isOk = Judge(EffectiveJudgeMode(label), raw, label.DownLimit, label.UpLimit);
                        string color = isOk ? label.Incolor : label.OutColor;

                        window.set_display_font(Convert.ToInt16(label.Size), "nomo", "true", "false");
                        Frm_Main.Instance.disp_message(window.hwc_imageWindow.HWindowHalconID,
                            raw,
                            new HTuple("image"),
                            new HTuple(displayRow),
                            new HTuple(displayCol),
                            new HTuple(color),
                            new HTuple("false"));
                    }
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据显示工具运行");
            }
        }

        #region 输入链接同步

        private ToolInfo FindToolInfoSafe()
        {
            try
            {
                if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(toolName))
                    return null;
                Job job = Job.FindJobByName(jobName);
                if (job == null)
                    return null;
                return job.FindToolInfoByName(toolName);
            }
            catch
            {
                return null;
            }
        }

        private ToolIO FindInputEntry(ToolInfo info, int rowIndex)
        {
            if (info == null || info.input == null)
                return null;
            string key = "输入项" + (rowIndex + 1);
            for (int i = 0; i < info.input.Count; i++)
                if (info.input[i].IOName == key)
                    return info.input[i];
            return null;
        }

        private static void ParseLink(string entryValue, LabelRowData row)
        {
            row.SourceTool = string.Empty;
            row.SourceOutput = string.Empty;
            row.LinkDisplay = string.Empty;
            if (string.IsNullOrEmpty(entryValue))
                return;
            string s = entryValue;
            if (s.StartsWith("《- "))
                s = s.Substring(3);
            string[] parts = Regex.Split(s, "->");
            if (parts.Length == 2)
            {
                row.SourceTool = parts[0];
                row.SourceOutput = parts[1];
                row.LinkDisplay = parts[0] + "." + parts[1];
            }
            else
            {
                row.LinkDisplay = s;
            }
        }

        /// <summary>
        /// 按"输入项N"重建 input 条目、流程树输入节点与连线映射，最后触发连线重绘。
        /// 无流程树环境（驱动/流程未展开）时各节点操作自动降级为空操作。
        /// </summary>
        private void SyncInputs(List<LabelRowData> rows)
        {
            try
            {
                if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(toolName))
                    return;
                Job job = Job.FindJobByName(jobName);
                if (job == null)
                    return;
                ToolInfo info = job.FindToolInfoByName(toolName);
                if (info == null)
                    return;
                if (info.input == null)
                    info.input = new List<ToolIO>();

                // 清掉旧的"输入项N"条目与对应树节点/连线映射（行数与链接都可能变化，全量重建）
                List<ToolIO> staleEntries = new List<ToolIO>();
                foreach (ToolIO io in info.input)
                    if (io.IOName != null && Regex.IsMatch(io.IOName, "^输入项\\d+$"))
                        staleEntries.Add(io);

                TreeNode toolNode = FindToolNode(job);
                List<TreeNode> staleNodes = new List<TreeNode>();
                if (toolNode != null)
                {
                    foreach (TreeNode node in toolNode.Nodes)
                    {
                        // 旧版拖入时预建的输入项节点 Name 为空串、文本为“<--输入项N”，按文本前缀清理才能扫到残留
                        if (node.Text != null && node.Text.StartsWith("<--输入项"))
                            staleNodes.Add(node);
                    }
                    foreach (TreeNode node in staleNodes)
                    {
                        job.D_itemAndSource.Remove(node);
                        node.Remove();
                    }
                }
                foreach (ToolIO io in staleEntries)
                    info.input.Remove(io);

                // 逐行重建
                for (int i = 0; i < rows.Count; i++)
                {
                    string ioKey = "输入项" + (i + 1);
                    string linkValue = string.IsNullOrEmpty(rows[i].SourceTool)
                        ? string.Empty
                        : "《- " + rows[i].SourceTool + "->" + rows[i].SourceOutput;
                    info.input.Add(new ToolIO(ioKey, linkValue, DataType.String));

                    if (toolNode == null)
                        continue;
                    int insertPos = Math.Min(i, toolNode.Nodes.Count);
                    TreeNode ioNode = toolNode.Nodes.Insert(insertPos, "<--" + ioKey, "<--" + ioKey + linkValue, 34, 34);
                    ioNode.ForeColor = Color.DarkMagenta;
                    ioNode.Tag = DataType.String;

                    // 工具→工具的链接才建连线映射；全局变量没有节点可连
                    if (!string.IsNullOrEmpty(linkValue) && rows[i].SourceTool != "全局变量")
                    {
                        TreeNode srcNode = job.GetToolIONodeByNodeText(rows[i].SourceTool, "-->" + rows[i].SourceOutput);
                        if (srcNode != null)
                            job.D_itemAndSource[ioNode] = srcNode;
                    }
                }

                job.DrawLine();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据显示工具同步输入链接");
            }
        }

        private TreeNode FindToolNode(Job job)
        {
            TreeView tree = Job.GetJobTree(jobName);
            if (tree == null)
                return null;
            foreach (TreeNode node in tree.Nodes)
                if (node.Text == toolName)
                    return node;
            return null;
        }

        #endregion

        #region 参数
        [Serializable]
        public class ToolPar : ToolParBase
        {
            private InputPar _inputPar = new InputPar();
            public InputPar InputPar
            {
                get { return _inputPar; }
                set { _inputPar = value; }
            }
            private RunPar _runPar = new RunPar();
            public RunPar RunPar
            {
                get { return _runPar; }
                set { _runPar = value; }
            }
            private ResultPar _resultPar = new ResultPar();
            public ResultPar ResultPar
            {
                get { return _resultPar; }
                set { _resultPar = value; }
            }
        }
        [Serializable]
        public class InputPar
        {
            private string _文本1 = string.Empty;
            public string 文本1
            {
                get { return _文本1; }
                set { _文本1 = value; }
            }
        }
        [Serializable]
        public class RunPar { }
        [Serializable]
        internal class ResultPar
        {
            private HObject _输出图像;
            public HObject 输出图像
            {
                get { return _输出图像; }
                set { _输出图像 = value; }
            }
        }
        #endregion
    }
}
