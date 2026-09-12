using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using VMPro;
using System.Text.RegularExpressions;

namespace VMPro
{
    /// <summary>
    /// 数据分析工具：链接前置工具输出（或全局变量），按"值下限~值上限"判定后输出
    /// "限内结果/限外结果"字符串（不显示到图像）。每行一个数据源，每行一个输出"输出项N"，
    /// 输出值写入 ToolInfo.output 条目，供脚本编辑等下游工具链接（可再存入全局变量）。
    /// 输入条目（输入项N）与输出条目（输出项N）由窗体保存时同步创建/更新。
    /// </summary>
    [Serializable]
    internal class DataAnalyseTool : ToolBase
    {
        /// <summary>
        /// 工具名（打开窗体时由流程编辑器赋值，输入/输出同步用）
        /// </summary>
        internal string toolName = string.Empty;
        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 分析项集合
        /// </summary>
        internal List<sDataType> L_items = new List<sDataType>();
        /// <summary>
        /// 工具参数
        /// </summary>
        internal ToolPar toolPar = new ToolPar();

        public DataAnalyseTool()
        {
            L_items.Add(MakeDefaultItem());
        }

        internal static sDataType MakeDefaultItem()
        {
            sDataType item = new sDataType();
            item.inputItem = string.Empty;      // 链接串"《- 源->输出"，空=未链接
            item.downLimit = double.NaN;        // NaN=该侧不限制
            item.upLimit = double.NaN;
            item.inResult = "OK";
            item.outResult = "NG";
            return item;
        }

        /// <summary>
        /// 行数据载体（窗体网格 ↔ 工具），不序列化。
        /// </summary>
        internal class ItemRowData
        {
            internal string SourceTool = string.Empty;      // 空=未链接
            internal string SourceOutput = string.Empty;
            internal string LinkDisplay = string.Empty;     // "工具名.输出结果名" / "全局变量.变量名"
            internal string Down = string.Empty;
            internal string Up = string.Empty;
            internal string InResult = "OK";
            internal string OutResult = "NG";
        }

        /// <summary>
        /// 工具 → 窗体：从 L_items 与 input 条目导出网格行（LinkDisplay 由链接串推导）。
        /// </summary>
        internal List<ItemRowData> ExportRows()
        {
            List<ItemRowData> rows = new List<ItemRowData>();
            ToolInfo info = FindToolInfoSafe();
            for (int i = 0; i < L_items.Count; i++)
            {
                sDataType item = L_items[i];
                ItemRowData row = new ItemRowData();
                row.Down = double.IsNaN(item.downLimit) ? string.Empty : item.downLimit.ToString();
                row.Up = double.IsNaN(item.upLimit) ? string.Empty : item.upLimit.ToString();
                row.InResult = string.IsNullOrEmpty(item.inResult) ? "OK" : item.inResult;
                row.OutResult = string.IsNullOrEmpty(item.outResult) ? "NG" : item.outResult;

                ToolIO entry = FindInputEntry(info, i);
                if (entry != null)
                    ParseLink(entry.value == null ? string.Empty : entry.value.ToString(), row);
                rows.Add(row);
            }
            return rows;
        }

        /// <summary>
        /// 窗体 → 工具：按网格行重建 L_items，并同步 input 条目、输出条目、
        /// 流程树输入/输出节点与连线（弹窗关闭后流程编辑器自动连线）。
        /// </summary>
        internal void ImportRows(List<ItemRowData> rows)
        {
            L_items.Clear();
            for (int i = 0; i < rows.Count; i++)
            {
                ItemRowData row = rows[i];
                sDataType item = MakeDefaultItem();
                double d;
                if (!string.IsNullOrEmpty(row.Down) && double.TryParse(row.Down, out d))
                    item.downLimit = d;
                if (!string.IsNullOrEmpty(row.Up) && double.TryParse(row.Up, out d))
                    item.upLimit = d;
                item.inResult = string.IsNullOrEmpty(row.InResult) ? "OK" : row.InResult;
                item.outResult = string.IsNullOrEmpty(row.OutResult) ? "NG" : row.OutResult;
                item.inputItem = string.IsNullOrEmpty(row.SourceTool)
                    ? string.Empty
                    : "《- " + row.SourceTool + "->" + row.SourceOutput;
                L_items.Add(item);
            }
            SyncIO(rows);
        }

        /// <summary>
        /// 运行工具：逐行解析链接值 → 范围判定 → 输出"限内结果/限外结果"，
        /// 并把结果写回 ToolInfo.output 条目（下游脚本编辑等工具经 Job.GetValue 可读，可再存入全局变量）。
        /// </summary>
        public override void Run(bool updateImage, bool debugTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);

                    Job job = string.IsNullOrEmpty(jobName) ? null : Job.FindJobByName(jobName);
                    ToolInfo info = job == null ? null : job.FindToolInfoByName(toolName);

                    for (int i = 0; i < L_items.Count; i++)
                    {
                        sDataType item = L_items[i];
                        string ioKey = "输入项" + (i + 1);
                        string outKey = "输出项" + (i + 1);

                        // 解析本行链接值（工具输出 / 全局变量）
                        string raw = string.Empty;
                        ToolIO entry = FindInputEntry(info, i);
                        string link = entry == null ? string.Empty : (entry.value == null ? string.Empty : entry.value.ToString());
                        if (!string.IsNullOrEmpty(link))
                        {
                            string body = link.StartsWith("《- ") ? link.Substring(3) : link;
                            string[] parts = Regex.Split(body, "->");
                            if (parts.Length == 2)
                            {
                                if (parts[0] == "全局变量")
                                    raw = Convert.ToString(Project.Instance.curEngine.globelVariable.GetGlobalVariableValue(parts[1]));
                                else if (parts[0] == "局部变量")
                                    raw = job == null ? string.Empty : Convert.ToString(job.GetLocalVariableValue(parts[1]));
                                else
                                    raw = Convert.ToString(job.FindToolInfoByName(parts[0]).GetOutput(parts[1]).value);
                            }
                        }

                        // 范围判定（留空=该侧不限制；非数值按限外处理）
                        bool inRange;
                        double v;
                        if (!double.TryParse(raw, out v))
                            inRange = false;
                        else
                        {
                            inRange = true;
                            if (!double.IsNaN(item.downLimit) && v < item.downLimit)
                                inRange = false;
                            if (!double.IsNaN(item.upLimit) && v > item.upLimit)
                                inRange = false;
                        }
                        string result = inRange ? item.inResult : item.outResult;

                        // 输出写入 ToolInfo.output 条目（下游经 Job.GetValue 读取），并保留旧 toolPar 兼容
                        if (info != null && info.output != null)
                        {
                            for (int k = 0; k < info.output.Count; k++)
                            {
                                if (info.output[k].IOName == outKey)
                                {
                                    info.output[k].value = result;
                                    break;
                                }
                            }
                        }
                        if (i == 0)
                            toolPar.ResultPar.输出项1 = result;
                    }
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据分析工具运行");
            }
        }

        #region 输入/输出同步

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

        private static void ParseLink(string entryValue, ItemRowData row)
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
        /// 按"输入项N/输出项N"同步 input/output 条目、流程树节点与连线映射，最后触发连线重绘。
        /// 输出条目/节点只增不减（避免破坏下游已建的链接）。
        /// </summary>
        private void SyncIO(List<ItemRowData> rows)
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
                if (info.output == null)
                    info.output = new List<ToolIO>();

                TreeNode toolNode = FindToolNode(job);

                // —— 输入条目全量重建（同数据显示）——
                List<ToolIO> staleInputEntries = new List<ToolIO>();
                foreach (ToolIO io in info.input)
                    if (io.IOName != null && Regex.IsMatch(io.IOName, "^输入项\\d+$"))
                        staleInputEntries.Add(io);

                List<TreeNode> staleNodes = new List<TreeNode>();
                if (toolNode != null)
                {
                    foreach (TreeNode node in toolNode.Nodes)
                    {
                        // 旧节点 Name 为空串，按文本前缀清理（含输入与输出节点分开匹配）
                        if (node.Text != null && node.Text.StartsWith("<--输入项"))
                            staleNodes.Add(node);
                    }
                    foreach (TreeNode node in staleNodes)
                    {
                        job.D_itemAndSource.Remove(node);
                        node.Remove();
                    }
                }
                foreach (ToolIO io in staleInputEntries)
                    info.input.Remove(io);

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

                    if (!string.IsNullOrEmpty(linkValue) && rows[i].SourceTool != "全局变量")
                    {
                        TreeNode srcNode = job.GetToolIONodeByNodeText(rows[i].SourceTool, "-->" + rows[i].SourceOutput);
                        if (srcNode != null)
                            job.D_itemAndSource[ioNode] = srcNode;
                    }
                }

                // —— 输出条目/节点只增不减 ——
                for (int i = 0; i < rows.Count; i++)
                {
                    string outKey = "输出项" + (i + 1);
                    bool hasEntry = false;
                    foreach (ToolIO io in info.output)
                        if (io.IOName == outKey)
                        {
                            hasEntry = true;
                            break;
                        }
                    if (!hasEntry)
                        info.output.Add(new ToolIO(outKey, string.Empty, DataType.String));

                    if (toolNode == null)
                        continue;
                    bool hasNode = false;
                    foreach (TreeNode node in toolNode.Nodes)
                        if (node.Text == "-->" + outKey)
                        {
                            hasNode = true;
                            break;
                        }
                    if (!hasNode)
                    {
                        TreeNode outNode = toolNode.Nodes.Add("-->" + outKey, "-->" + outKey, 34, 34);
                        outNode.ForeColor = Color.Blue;
                        outNode.Tag = DataType.String;
                    }
                }

                job.DrawLine();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据分析工具同步输入/输出");
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
            private string _输入项1 = string.Empty;
            public string 输入项1
            {
                get { return _输入项1; }
                set { _输入项1 = value; }
            }
        }
        [Serializable]
        public class RunPar { }
        [Serializable]
        internal class ResultPar
        {
            private string _输出项1 = string.Empty;
            public string 输出项1
            {
                get { return _输出项1; }
                set { _输出项1 = value; }
            }
        }
        #endregion

    }
    [Serializable]
    internal struct sDataType
    {
        /// <summary>链接串"《- 源->输出"（空=未链接）</summary>
        internal string inputItem;
        internal double downLimit;      // NaN=该侧不限制
        internal double upLimit;        // NaN=该侧不限制
        internal string inResult;
        internal string outResult;
    }
}
