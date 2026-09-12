using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    /// <summary>
    /// 数据显示工具的链接选择器。
    /// 树形分组：顶层第一组"全局变量"（子节点=各全局变量，链接"全局变量->变量名"）；
    /// 其下为当前流程中位于该工具之前的每个工具（子节点=该工具各输出，链接"工具名->输出名"）。
    /// 顶部搜索框按关键字过滤；双击叶子或点"确定"返回选择。
    /// </summary>
    internal class Frm_LinkPicker : Form
    {
        /// <summary>
        /// 选中链接串："工具名->输出名" 或 "全局变量->变量名"
        /// </summary>
        internal string SelectedSource = string.Empty;
        /// <summary>
        /// 选中源工具名（全局变量时为“全局变量”）
        /// </summary>
        internal string SelectedSourceTool = string.Empty;
        /// <summary>
        /// 选中源输出名（全局变量时为变量名）
        /// </summary>
        internal string SelectedSourceOutput = string.Empty;
        /// <summary>
        /// 选中显示文本："工具名.输出结果名" 或 "全局变量.变量名"
        /// </summary>
        internal string SelectedDisplay = string.Empty;

        private readonly TextBox txt_search;
        private readonly TreeView tvw_items;
        private readonly Button btn_ok;

        internal Frm_LinkPicker(string jobName, string toolName)
        {
            Text = "选择数据源";
            Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 480);

            txt_search = new TextBox();
            txt_search.SetBounds(12, 12, ClientSize.Width - 24, 25);
            txt_search.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txt_search.TextChanged += delegate { ApplyFilter(); };

            tvw_items = new TreeView();
            tvw_items.SetBounds(12, 45, ClientSize.Width - 24, ClientSize.Height - 45 - 45);
            tvw_items.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tvw_items.HideSelection = false;
            tvw_items.FullRowSelect = true;
            tvw_items.ShowLines = true;
            tvw_items.ItemHeight = 24;
            tvw_items.NodeMouseDoubleClick += delegate (object sender, TreeNodeMouseClickEventArgs e)
            {
                if (e.Node != null && e.Node.Tag != null)
                    ConfirmSelection(e.Node);
            };

            btn_ok = new Button();
            btn_ok.Text = "确定";
            btn_ok.SetBounds(ClientSize.Width - 152, ClientSize.Height - 36, 65, 28);
            btn_ok.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_ok.Click += delegate
            {
                TreeNode selected = tvw_items.SelectedNode;
                if (selected == null || selected.Tag == null)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n请先选择一个数据源（叶子节点）");
                    return;
                }
                ConfirmSelection(selected);
            };

            Button btn_cancel = new Button();
            btn_cancel.Text = "取消";
            btn_cancel.SetBounds(ClientSize.Width - 77, ClientSize.Height - 36, 65, 28);
            btn_cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_cancel.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(txt_search);
            Controls.Add(tvw_items);
            Controls.Add(btn_ok);
            Controls.Add(btn_cancel);
            AcceptButton = btn_ok;
            CancelButton = btn_cancel;

            BuildTree(jobName, toolName);
        }

        /// <summary>
        /// 组树：全局变量在最上，然后是此工具之前的各工具及其输出。
        /// 叶子节点 Tag = 链接串；叶子 Text = 输出结果名/变量名；父节点 Text = 工具名/全局变量。
        /// </summary>
        private void BuildTree(string jobName, string toolName)
        {
            try
            {
                Job job = Job.FindJobByName(jobName);
                if (job == null)
                    return;

                TreeNode globalNode = tvw_items.Nodes.Add("全局变量");
                globalNode.ForeColor = Color.FromArgb(120, 120, 120);
                List<Variable> variables = Project.Instance.curEngine.globelVariable.L_variable;
                for (int i = 0; i < variables.Count; i++)
                {
                    TreeNode child = globalNode.Nodes.Add(variables[i].name);
                    child.Tag = "全局变量->" + variables[i].name;
                }

                // 局部变量分组：只能在当前流程内使用，供本工具输入引用
                TreeNode localNode = tvw_items.Nodes.Add("局部变量");
                localNode.ForeColor = Color.FromArgb(120, 120, 120);
                lock (Job.LocalVariableSync)
                {
                    for (int i = 0; i < job.localVariables.Count; i++)
                    {
                        LocalVariableItem variable = job.localVariables[i];
                        if (variable == null || string.IsNullOrEmpty(variable.name))
                            continue;
                        TreeNode localChild = localNode.Nodes.Add(variable.name);
                        localChild.Tag = "局部变量->" + variable.name;
                    }
                }

                bool pastTool = string.IsNullOrEmpty(toolName);
                for (int i = 0; i < job.L_toolList.Count; i++)
                {
                    ToolInfo toolInfo = job.L_toolList[i];
                    if (!pastTool)
                    {
                        // 只列此工具之前的工具（含边界保护：找不到目标工具时全部列出）
                        if (toolInfo.toolName == toolName)
                            pastTool = true;
                        if (!pastTool)
                        {
                            AddToolNode(toolInfo);
                            continue;
                        }
                        continue;
                    }
                    AddToolNode(toolInfo);
                }

                globalNode.Expand();
                localNode.Expand();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据显示工具链接选择器组树");
            }
        }

        private void AddToolNode(ToolInfo toolInfo)
        {
            if (toolInfo.output == null || toolInfo.output.Count == 0)
                return;
            TreeNode toolNode = tvw_items.Nodes.Add(toolInfo.toolName);
            for (int j = 0; j < toolInfo.output.Count; j++)
            {
                string outputName = toolInfo.output[j].IOName;
                if (string.IsNullOrEmpty(outputName))
                    continue;
                TreeNode child = toolNode.Nodes.Add(outputName);
                child.Tag = toolInfo.toolName + "->" + outputName;
            }
        }

        private void ApplyFilter()
        {
            string keyword = txt_search.Text == null ? string.Empty : txt_search.Text.Trim();
            // TreeNode 没有 Visible：先把所有叶子收回到暂存表，再把不匹配的临时移除
            foreach (KeyValuePair<TreeNode, TreeNode> hidden in hiddenLeaves)
                hidden.Key.Nodes.Add(hidden.Value);
            hiddenLeaves.Clear();
            if (keyword.Length == 0)
            {
                foreach (TreeNode group in tvw_items.Nodes)
                    group.Expand();
                return;
            }
            foreach (TreeNode group in tvw_items.Nodes)
            {
                bool groupVisible = group.Text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
                for (int i = group.Nodes.Count - 1; i >= 0; i--)
                {
                    TreeNode leaf = group.Nodes[i];
                    bool leafVisible = leaf.Text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!leafVisible && !groupVisible)
                    {
                        hiddenLeaves.Add(new KeyValuePair<TreeNode, TreeNode>(group, leaf));
                        leaf.Remove();
                    }
                }
                if (group.Nodes.Count > 0 || groupVisible)
                    group.Expand();
            }
        }

        private readonly List<KeyValuePair<TreeNode, TreeNode>> hiddenLeaves = new List<KeyValuePair<TreeNode, TreeNode>>();

        private void ConfirmSelection(TreeNode node)
        {
            string source = node.Tag as string;
            if (string.IsNullOrEmpty(source))
                return;
            SelectedSource = source;
            SelectedSourceTool = node.Parent.Text;
            SelectedSourceOutput = node.Text;
            SelectedDisplay = node.Parent.Text + "." + node.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
