using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace VMPro
{
    internal class Frm_CodeEditTool : Form
    {
        internal static CodeEditTool codeEditTool;
        internal string jobName = string.Empty;
        internal string toolName = string.Empty;

        private static Frm_CodeEditTool _instance;
        public static Frm_CodeEditTool Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_CodeEditTool();
                return _instance;
            }
        }

        private DataGridView dgvInput = new DataGridView();
        private DataGridView dgvOutput = new DataGridView();
        private FastColoredTextBox tbxCode = new FastColoredTextBox();
        private TextBox tbxCompileResult = new TextBox();
        private Button btnAddInput = new Button();
        private Button btnAddOutput = new Button();
        private Button btnGenerateInputs = new Button();
        private Button btnCompile = new Button();
        private Button btnSave = new Button();
        private bool loadingData;
        private bool hasUnsavedChanges;
        /// <summary>右键待删除的表格与行索引；菜单点击时使用。</summary>
        private DataGridView pendingDeleteGrid;
        private int pendingDeleteRowIndex = -1;
        private ContextMenuStrip gridDeleteMenu = new ContextMenuStrip();

        private Frm_CodeEditTool()
        {
            InitializeComponent();
            ToolStripItem deleteRowItem = gridDeleteMenu.Items.Add("删除该行");
            deleteRowItem.Click += deleteRowMenuItem_Click;
            dgvInput.CellMouseClick += dgv_CellMouseClick;
            dgvOutput.CellMouseClick += dgv_CellMouseClick;
        }

        /// <summary>
        /// 输入/输出表格右键某一行时选中该行并弹出删除菜单。
        /// </summary>
        private void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            pendingDeleteGrid = null;
            pendingDeleteRowIndex = -1;
            if (e.Button != MouseButtons.Right || e.RowIndex < 0)
                return;
            DataGridView grid = sender as DataGridView;
            if (grid == null || e.RowIndex >= grid.Rows.Count)
                return;
            pendingDeleteGrid = grid;
            pendingDeleteRowIndex = e.RowIndex;
            grid.ClearSelection();
            grid.Rows[e.RowIndex].Selected = true;
            gridDeleteMenu.Show(grid, grid.PointToClient(Cursor.Position));
        }

        private void deleteRowMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingDeleteGrid == null || pendingDeleteRowIndex < 0 ||
                pendingDeleteRowIndex >= pendingDeleteGrid.Rows.Count)
                return;
            pendingDeleteGrid.Rows.RemoveAt(pendingDeleteRowIndex);
            pendingDeleteGrid = null;
            pendingDeleteRowIndex = -1;
        }

        private void InitializeComponent()
        {
            Text = "脚本编辑";
            Size = new Size(1100, 720);
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += Frm_CodeEditTool_FormClosing;

            SplitContainer split = new SplitContainer();
            split.Size = new Size(1080, 640);
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Vertical;
            split.Panel1MinSize = 320;
            split.Panel2MinSize = 320;
            split.SplitterDistance = 500;
            Controls.Add(split);

            TabControl ioTabs = new TabControl();
            ioTabs.Dock = DockStyle.Fill;
            TabPage inputTab = new TabPage("输入");
            TabPage outputTab = new TabPage("输出");
            ioTabs.TabPages.Add(inputTab);
            ioTabs.TabPages.Add(outputTab);
            split.Panel1.Controls.Add(ioTabs);

            BuildInputPage(inputTab);
            BuildOutputPage(outputTab);
            BuildCodePage(split.Panel2);

            btnSave.Dock = DockStyle.Bottom;
            btnSave.Height = 36;
            btnSave.Text = "保存";
            btnSave.Click += btnSave_Click;
            Controls.Add(btnSave);
            btnSave.BringToFront();
        }

        private void BuildInputPage(Control parent)
        {
            TableLayoutPanel panel = CreateGridPanel();
            parent.Controls.Add(panel);

            btnAddInput.Text = "添加输入";
            btnAddInput.Dock = DockStyle.Fill;
            btnAddInput.Click += btnAddInput_Click;
            panel.Controls.Add(btnAddInput, 0, 0);

            dgvInput.Dock = DockStyle.Fill;
            dgvInput.AllowUserToAddRows = false;
            dgvInput.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInput.CellContentClick += dgvInput_CellContentClick;
            dgvInput.CellValueChanged += delegate(object sender, DataGridViewCellEventArgs e) { MarkDirty(sender, e); };
            dgvInput.RowsAdded += delegate(object sender, DataGridViewRowsAddedEventArgs e) { MarkDirty(sender, e); };
            dgvInput.RowsRemoved += delegate(object sender, DataGridViewRowsRemovedEventArgs e) { RenumberRows(dgvInput); MarkDirty(sender, e); };
            dgvInput.CurrentCellDirtyStateChanged += dgv_CurrentCellDirtyStateChanged;
            DataGridViewTextBoxColumn inputIndex = new DataGridViewTextBoxColumn();
            inputIndex.Name = "Index";
            inputIndex.HeaderText = "序号";
            inputIndex.ReadOnly = true;
            inputIndex.FillWeight = 35;
            dgvInput.Columns.Add(inputIndex);
            dgvInput.Columns.Add("InputName", "名称");

            DataGridViewComboBoxColumn valueType = new DataGridViewComboBoxColumn();
            valueType.Name = "ValueType";
            valueType.HeaderText = "类型";
            valueType.DataSource = Enum.GetNames(typeof(CodeValueType));
            dgvInput.Columns.Add(valueType);

            DataGridViewTextBoxColumn variableSource = new DataGridViewTextBoxColumn();
            variableSource.Name = "VariableSource";
            variableSource.HeaderText = "变量来源";
            variableSource.ReadOnly = true;
            dgvInput.Columns.Add(variableSource);

            DataGridViewButtonColumn chooseSource = new DataGridViewButtonColumn();
            chooseSource.Name = "ChooseSource";
            chooseSource.HeaderText = "链接";
            chooseSource.Text = "选择";
            chooseSource.UseColumnTextForButtonValue = true;
            dgvInput.Columns.Add(chooseSource);
            panel.Controls.Add(dgvInput, 0, 1);
        }

        private void BuildOutputPage(Control parent)
        {
            TableLayoutPanel panel = CreateGridPanel();
            parent.Controls.Add(panel);

            btnAddOutput.Text = "添加输出";
            btnAddOutput.Dock = DockStyle.Fill;
            btnAddOutput.Click += btnAddOutput_Click;
            panel.Controls.Add(btnAddOutput, 0, 0);

            dgvOutput.Dock = DockStyle.Fill;
            dgvOutput.AllowUserToAddRows = false;
            dgvOutput.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOutput.CellValueChanged += delegate(object sender, DataGridViewCellEventArgs e) { MarkDirty(sender, e); };
            dgvOutput.RowsAdded += delegate(object sender, DataGridViewRowsAddedEventArgs e) { MarkDirty(sender, e); };
            dgvOutput.RowsRemoved += delegate(object sender, DataGridViewRowsRemovedEventArgs e) { RenumberRows(dgvOutput); MarkDirty(sender, e); };
            dgvOutput.CurrentCellDirtyStateChanged += dgv_CurrentCellDirtyStateChanged;
            DataGridViewTextBoxColumn outputIndex = new DataGridViewTextBoxColumn();
            outputIndex.Name = "Index";
            outputIndex.HeaderText = "序号";
            outputIndex.ReadOnly = true;
            outputIndex.FillWeight = 35;
            dgvOutput.Columns.Add(outputIndex);
            dgvOutput.Columns.Add("OutputName", "名称");

            DataGridViewComboBoxColumn valueType = new DataGridViewComboBoxColumn();
            valueType.Name = "ValueType";
            valueType.HeaderText = "类型";
            valueType.DataSource = Enum.GetNames(typeof(CodeValueType));
            dgvOutput.Columns.Add(valueType);

            DataGridViewCheckBoxColumn writeGlobal = new DataGridViewCheckBoxColumn();
            writeGlobal.Name = "WriteGlobalVariable";
            writeGlobal.HeaderText = "写入全局变量";
            dgvOutput.Columns.Add(writeGlobal);

            DataGridViewCheckBoxColumn writeLocal = new DataGridViewCheckBoxColumn();
            writeLocal.Name = "WriteLocalVariable";
            writeLocal.HeaderText = "写入局部变量";
            dgvOutput.Columns.Add(writeLocal);
            panel.Controls.Add(dgvOutput, 0, 1);
        }

        private void BuildCodePage(Control parent)
        {
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 1;
            panel.RowCount = 3;
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            parent.Controls.Add(panel);

            Panel header = new Panel();
            header.Dock = DockStyle.Fill;
            System.Windows.Forms.Label title = new System.Windows.Forms.Label();
            title.Text = "自定义代码";
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;
            header.Controls.Add(title);
            btnCompile.Text = "编译";
            btnCompile.Dock = DockStyle.Right;
            btnCompile.Width = 80;
            btnCompile.Click += btnCompile_Click;
            header.Controls.Add(btnCompile);
            btnCompile.BringToFront();
            btnGenerateInputs.Text = "生成初始化";
            btnGenerateInputs.Dock = DockStyle.Right;
            btnGenerateInputs.Width = 100;
            btnGenerateInputs.Click += btnGenerateInitialization_Click;
            header.Controls.Add(btnGenerateInputs);
            btnGenerateInputs.BringToFront();
            panel.Controls.Add(header, 0, 0);

            tbxCompileResult.Dock = DockStyle.Fill;
            tbxCompileResult.Multiline = true;
            tbxCompileResult.ReadOnly = true;
            tbxCompileResult.ScrollBars = ScrollBars.Vertical;
            panel.Controls.Add(tbxCompileResult, 0, 2);

            tbxCode.Dock = DockStyle.Fill;
            tbxCode.Font = new Font("NSimSun", 11F, FontStyle.Regular, GraphicsUnit.Point);
            tbxCode.Language = FastColoredTextBoxNS.Language.CSharp;
            tbxCode.ShowLineNumbers = true;
            tbxCode.WordWrap = false;
            tbxCode.AutoIndent = true;
            tbxCode.AutoIndentExistingLines = true;
            tbxCode.TabLength = 4;
            tbxCode.LineInterval = 3;
            tbxCode.BackColor = Color.White;
            tbxCode.IndentBackColor = Color.FromArgb(245, 245, 245);
            tbxCode.LineNumberColor = Color.DimGray;
            tbxCode.TextChanged += delegate { MarkDirty(tbxCode, EventArgs.Empty); };
            panel.Controls.Add(tbxCode, 0, 1);
        }

        private TableLayoutPanel CreateGridPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 1;
            panel.RowCount = 2;
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            return panel;
        }

        internal void LoadToolData()
        {
            if (codeEditTool == null)
                return;

            loadingData = true;
            try
            {
                codeEditTool.EnsureClassTemplate();
                dgvInput.Rows.Clear();
                for (int i = 0; i < codeEditTool.L_inputItems.Count; i++)
                {
                    CodeInputItem item = codeEditTool.L_inputItems[i];
                    int index = dgvInput.Rows.Add();
                    DataGridViewRow row = dgvInput.Rows[index];
                    row.Cells["Index"].Value = index + 1;
                    row.Cells["InputName"].Value = item.InputName;
                    row.Cells["ValueType"].Value = item.ValueType.ToString();
                    row.Cells["VariableSource"].Value = item.VariableSource;
                }

                dgvOutput.Rows.Clear();
                for (int i = 0; i < codeEditTool.L_outputItems.Count; i++)
                {
                    CodeOutputItem item = codeEditTool.L_outputItems[i];
                    int index = dgvOutput.Rows.Add();
                    DataGridViewRow row = dgvOutput.Rows[index];
                    row.Cells["Index"].Value = index + 1;
                    row.Cells["OutputName"].Value = item.OutputName;
                    row.Cells["ValueType"].Value = item.ValueType.ToString();
                    row.Cells["WriteGlobalVariable"].Value = item.WriteGlobalVariable;
                    row.Cells["WriteLocalVariable"].Value = item.WriteLocalVariable;
                }

                tbxCode.Text = NormalizeLineEndings(codeEditTool.sourceCode);
                tbxCompileResult.Text = codeEditTool.compileResult;
                hasUnsavedChanges = false;
            }
            finally
            {
                loadingData = false;
            }
        }

        private void btnAddInput_Click(object sender, EventArgs e)
        {
            int index = dgvInput.Rows.Add();
            DataGridViewRow row = dgvInput.Rows[index];
            row.Cells["Index"].Value = index + 1;
            row.Cells["InputName"].Value = GetUniqueName("input", dgvInput, "InputName");
            row.Cells["ValueType"].Value = CodeValueType.Double.ToString();
        }

        private void btnAddOutput_Click(object sender, EventArgs e)
        {
            int index = dgvOutput.Rows.Add();
            DataGridViewRow row = dgvOutput.Rows[index];
            row.Cells["Index"].Value = index + 1;
            row.Cells["OutputName"].Value = GetUniqueName("output", dgvOutput, "OutputName");
            row.Cells["ValueType"].Value = CodeValueType.Double.ToString();
            row.Cells["WriteGlobalVariable"].Value = false;
            row.Cells["WriteLocalVariable"].Value = false;
        }

        private string GetUniqueName(string prefix, DataGridView grid, string columnName)
        {
            int index = 1;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                Match match = Regex.Match(GetCellString(grid.Rows[i], columnName), "^" + Regex.Escape(prefix) + "([0-9]+)$");
                int current;
                if (match.Success && int.TryParse(match.Groups[1].Value, out current) && current >= index)
                    index = current + 1;
            }
            while (true)
            {
                string name = prefix + index.ToString();
                bool exists = false;
                for (int i = 0; i < grid.Rows.Count; i++)
                    if (GetCellString(grid.Rows[i], columnName) == name)
                        exists = true;
                if (!exists)
                    return name;
                index++;
            }
        }

        private void RenumberRows(DataGridView grid)
        {
            if (!grid.Columns.Contains("Index"))
                return;
            for (int i = 0; i < grid.Rows.Count; i++)
                grid.Rows[i].Cells["Index"].Value = i + 1;
        }

        private void dgvInput_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvInput.Columns[e.ColumnIndex].Name != "ChooseSource")
                return;

            ContextMenuStrip menu = BuildVariableMenu(e.RowIndex);
            Rectangle rect = dgvInput.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            menu.Show(dgvInput, rect.Left, rect.Bottom);
        }

        private ContextMenuStrip BuildVariableMenu(int rowIndex)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem globalMenu = new ToolStripMenuItem("全局变量");
            ToolStripMenuItem localMenu = new ToolStripMenuItem("局部变量");
            ToolStripMenuItem currentJobMenu = new ToolStripMenuItem("当前流程");
            ToolStripMenuItem otherJobMenu = new ToolStripMenuItem("其他流程");
            menu.Items.Add(globalMenu);
            menu.Items.Add(localMenu);
            menu.Items.Add(currentJobMenu);
            menu.Items.Add(otherJobMenu);
            AddGlobalVariableMenu(globalMenu, rowIndex);
            AddLocalVariableMenu(localMenu, rowIndex);
            AddJobVariableMenus(currentJobMenu, otherJobMenu, rowIndex);
            return menu;
        }

        private void AddLocalVariableMenu(ToolStripMenuItem parent, int rowIndex)
        {
            Job job = Project.Instance.curEngine.FindJobByName(jobName);
            if (job == null)
                return;
            lock (Job.LocalVariableSync)
            {
                for (int i = 0; i < job.localVariables.Count; i++)
                {
                    LocalVariableItem variable = job.localVariables[i];
                    if (variable == null || string.IsNullOrEmpty(variable.name))
                        continue;
                    AddVariableMenuItem(parent, variable.name, "《- 局部变量->" + variable.name, rowIndex);
                }
            }
        }

        private void AddGlobalVariableMenu(ToolStripMenuItem parent, int rowIndex)
        {
            ToolStripMenuItem systemMenu = new ToolStripMenuItem("系统变量");
            ToolStripMenuItem customMenu = new ToolStripMenuItem("自定义变量");
            parent.DropDownItems.Add(systemMenu);
            parent.DropDownItems.Add(customMenu);
            for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
            {
                Variable variable = Project.Instance.curEngine.globelVariable.L_variable[i];
                if (!IsSupportedVariableType(variable.type))
                    continue;
                ToolStripMenuItem target = variable.variableType == 1 ? customMenu : systemMenu;
                AddVariableMenuItem(target, variable.name, "《- 全局变量->" + variable.name, rowIndex);
            }
        }

        private void AddJobVariableMenus(ToolStripMenuItem currentJobMenu, ToolStripMenuItem otherJobMenu, int rowIndex)
        {
            for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
            {
                Job job = Project.Instance.curEngine.L_jobList[i];
                ToolStripMenuItem jobMenu = job.jobName == jobName ? currentJobMenu : new ToolStripMenuItem(job.jobName);
                if (job.jobName != jobName)
                    otherJobMenu.DropDownItems.Add(jobMenu);
                for (int j = 0; j < job.L_toolList.Count; j++)
                {
                    ToolInfo tool = job.L_toolList[j];
                    if (job.jobName == jobName && tool.toolName == toolName)
                        continue;
                    ToolStripMenuItem toolMenu = null;
                    for (int k = 0; k < tool.output.Count; k++)
                    {
                        if (tool.output[k].ioType != DataType.String)
                            continue;
                        if (toolMenu == null)
                        {
                            toolMenu = new ToolStripMenuItem(tool.toolName);
                            jobMenu.DropDownItems.Add(toolMenu);
                        }
                        string source = job.jobName == jobName
                            ? "《- " + tool.toolName + "->" + tool.output[k].IOName
                            : "《- [" + job.jobName + "]" + tool.toolName + "->" + tool.output[k].IOName;
                        AddVariableMenuItem(toolMenu, tool.output[k].IOName, source, rowIndex);
                    }
                }
            }
        }

        private void AddVariableMenuItem(ToolStripMenuItem parent, string text, string source, int rowIndex)
        {
            ToolStripItem item = parent.DropDownItems.Add(text);
            item.Tag = source;
            item.Click += delegate(object sender, EventArgs e) { SelectVariableSource(rowIndex, ((ToolStripItem)sender).Tag.ToString()); };
        }

        private void SelectVariableSource(int rowIndex, string source)
        {
            if (rowIndex < 0 || rowIndex >= dgvInput.Rows.Count)
                return;
            dgvInput.Rows[rowIndex].Cells["VariableSource"].Value = source;
            Job job = Project.Instance.curEngine.FindJobByName(jobName);
            if (job != null)
                dgvInput.Rows[rowIndex].Cells["ValueType"].Value = job.ResolveCodeEditSourceValueType(source).ToString();
        }

        private bool IsSupportedVariableType(string type)
        {
            return type == "Int" || type == "Double" || type == "String" || type == "Bool";
        }

        private void MarkDirty(object sender, EventArgs e)
        {
            if (!loadingData)
                hasUnsavedChanges = true;
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (grid != null && grid.IsCurrentCellDirty)
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void Frm_CodeEditTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!hasUnsavedChanges)
                return;

            DialogResult result = MessageBox.Show("脚本编辑内容已更改，是否保存？", "脚本编辑", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (result == DialogResult.Yes && !TrySaveToolData())
                e.Cancel = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (TrySaveToolData())
                Hide();
        }

        private bool TrySaveToolData()
        {
            dgvInput.EndEdit();
            dgvOutput.EndEdit();
            if (!ValidateItems())
                return false;
            tbxCompileResult.Text = codeEditTool.ValidateCode(CreateInputItems(), CreateOutputItems(), tbxCode.Text);
            if (tbxCompileResult.Text != "编译成功")
                return false;
            SaveToolData();
            SyncJobTree();
            hasUnsavedChanges = false;
            return true;
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            dgvInput.EndEdit();
            dgvOutput.EndEdit();
            if (!ValidateItems())
                return;
            tbxCompileResult.Text = codeEditTool.ValidateCode(CreateInputItems(), CreateOutputItems(), tbxCode.Text);
        }

        private void btnGenerateInitialization_Click(object sender, EventArgs e)
        {
            dgvInput.EndEdit();
            string code = NormalizeLineEndings(tbxCode.Text);
            const string startMarker = "#region InputInitialization";
            const string endMarker = "#endregion";
            int start = code.IndexOf(startMarker, StringComparison.Ordinal);
            if (start < 0)
            {
                MessageBox.Show("代码中未找到“#region 输入初始化”区域。", "脚本编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int contentStart = code.IndexOf(Environment.NewLine, start, StringComparison.Ordinal);
            int end = code.IndexOf(endMarker, contentStart < 0 ? start + startMarker.Length : contentStart, StringComparison.Ordinal);
            if (contentStart < 0 || end < 0)
            {
                MessageBox.Show("输入初始化区域缺少正确的 #endregion。", "脚本编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int endLineStart = code.LastIndexOf(Environment.NewLine, end, StringComparison.Ordinal);
            if (endLineStart < contentStart)
                endLineStart = end;

            System.Text.StringBuilder initialization = new System.Text.StringBuilder();
            initialization.Append(Environment.NewLine).Append(Environment.NewLine);
            for (int i = 0; i < dgvInput.Rows.Count; i++)
            {
                string name = GetCellString(dgvInput.Rows[i], "InputName");
                CodeValueType type = ParseValueType(GetCellString(dgvInput.Rows[i], "ValueType"));
                string source = GetCellString(dgvInput.Rows[i], "VariableSource");
                if (!string.IsNullOrEmpty(source))
                    initialization.Append("            // ")
                        .Append(CodeEditTool.FormatCommentText("链接：" + source))
                        .Append(Environment.NewLine);
                initialization.Append("            ")
                    .Append(GetCSharpType(type)).Append(" ").Append(name)
                    .Append(" = Read<").Append(GetCSharpType(type)).Append(">(inputs, \"")
                    .Append(name).Append("\");").Append(Environment.NewLine);
                if (i < dgvInput.Rows.Count - 1)
                    initialization.Append(Environment.NewLine);
            }
            initialization.Append(Environment.NewLine).Append(Environment.NewLine);

            tbxCode.Text = code.Substring(0, contentStart + Environment.NewLine.Length)
                + initialization.ToString()
                + code.Substring(endLineStart + Environment.NewLine.Length);
            GenerateOutputInitialization();
            hasUnsavedChanges = true;
        }

        private bool GenerateOutputInitialization()
        {
            dgvOutput.EndEdit();
            string code = NormalizeLineEndings(tbxCode.Text);
            const string startMarker = "#region FunctionImplementation";
            const string endMarker = "#endregion";
            string outputStartMarker = "// " + CodeEditTool.FormatCommentText("输出初始化开始");
            string outputEndMarker = "// " + CodeEditTool.FormatCommentText("输出初始化结束");
            string outputWriteStartMarker = "// " + CodeEditTool.FormatCommentText("输出回写开始");
            string outputWriteEndMarker = "// " + CodeEditTool.FormatCommentText("输出回写结束");
            const string legacyOutputStartMarker = "// 输出初始化开始";
            const string legacyOutputEndMarker = "// 输出初始化结束";
            const string legacyOutputWriteStartMarker = "// 输出回写开始";
            const string legacyOutputWriteEndMarker = "// 输出回写结束";
            int start = code.IndexOf(startMarker, StringComparison.Ordinal);
            if (start < 0)
            {
                MessageBox.Show("代码中未找到“#region 功能实现”区域。", "脚本编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            int contentStart = code.IndexOf(Environment.NewLine, start, StringComparison.Ordinal);
            int end = code.IndexOf(endMarker, contentStart < 0 ? start + startMarker.Length : contentStart, StringComparison.Ordinal);
            if (contentStart < 0 || end < 0)
            {
                MessageBox.Show("功能实现区域缺少正确的 #endregion。", "脚本编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            System.Text.StringBuilder initialization = new System.Text.StringBuilder();
            initialization.Append(Environment.NewLine).Append(Environment.NewLine);
            initialization.Append("            ").Append(outputStartMarker).Append(Environment.NewLine);
            for (int i = 0; i < dgvOutput.Rows.Count; i++)
            {
                string name = GetCellString(dgvOutput.Rows[i], "OutputName");
                CodeValueType type = ParseValueType(GetCellString(dgvOutput.Rows[i], "ValueType"));
                initialization.Append("            ").Append(GetCSharpType(type)).Append(" ").Append(name)
                    .Append(" = default(").Append(GetCSharpType(type)).Append(");").Append(Environment.NewLine);
            }
            initialization.Append("            ").Append(outputEndMarker);
            initialization.Append(Environment.NewLine).Append(Environment.NewLine);

            System.Text.StringBuilder writeBack = new System.Text.StringBuilder();
            writeBack.Append(Environment.NewLine).Append(Environment.NewLine);
            writeBack.Append("            ").Append(outputWriteStartMarker).Append(Environment.NewLine);
            for (int i = 0; i < dgvOutput.Rows.Count; i++)
            {
                string name = GetCellString(dgvOutput.Rows[i], "OutputName");
                writeBack.Append("            outputs[\"").Append(name).Append("\"] = ").Append(name).Append(";").Append(Environment.NewLine);
            }
            writeBack.Append("            ").Append(outputWriteEndMarker);
            writeBack.Append(Environment.NewLine).Append(Environment.NewLine);

            int outputStart = code.IndexOf(outputStartMarker, contentStart, StringComparison.Ordinal);
            if (outputStart < 0)
                outputStart = code.IndexOf(legacyOutputStartMarker, contentStart, StringComparison.Ordinal);
            int outputEnd = outputStart >= 0 ? code.IndexOf(outputEndMarker, outputStart, StringComparison.Ordinal) : -1;
            if (outputEnd < 0 && outputStart >= 0)
                outputEnd = code.IndexOf(legacyOutputEndMarker, outputStart, StringComparison.Ordinal);
            if (outputStart >= 0 && outputStart < end && outputEnd >= 0 && outputEnd < end)
            {
                int replaceStart = code.LastIndexOf(Environment.NewLine, outputStart, StringComparison.Ordinal);
                replaceStart = replaceStart < contentStart ? outputStart : replaceStart;
                int replaceEnd = code.IndexOf(Environment.NewLine, outputEnd, StringComparison.Ordinal);
                string matchedEndMarker = code.IndexOf(outputEndMarker, outputEnd, StringComparison.Ordinal) == outputEnd ? outputEndMarker : legacyOutputEndMarker;
                replaceEnd = replaceEnd < 0 || replaceEnd > end ? outputEnd + matchedEndMarker.Length : replaceEnd;
                tbxCode.Text = code.Substring(0, replaceStart)
                    + initialization.ToString()
                    + code.Substring(replaceEnd);
                return GenerateOutputWriteBack(outputWriteStartMarker, outputWriteEndMarker, legacyOutputWriteStartMarker, legacyOutputWriteEndMarker, writeBack.ToString());
            }

            tbxCode.Text = code.Substring(0, contentStart + Environment.NewLine.Length)
                + initialization.ToString()
                + code.Substring(contentStart + Environment.NewLine.Length);
            return GenerateOutputWriteBack(outputWriteStartMarker, outputWriteEndMarker, legacyOutputWriteStartMarker, legacyOutputWriteEndMarker, writeBack.ToString());
        }

        private bool GenerateOutputWriteBack(string outputWriteStartMarker, string outputWriteEndMarker, string legacyOutputWriteStartMarker, string legacyOutputWriteEndMarker, string writeBack)
        {
            string code = NormalizeLineEndings(tbxCode.Text);
            const string startMarker = "#region FunctionImplementation";
            const string endMarker = "#endregion";
            int start = code.IndexOf(startMarker, StringComparison.Ordinal);
            int contentStart = start < 0 ? -1 : code.IndexOf(Environment.NewLine, start, StringComparison.Ordinal);
            int end = contentStart < 0 ? -1 : code.IndexOf(endMarker, contentStart, StringComparison.Ordinal);
            if (contentStart < 0 || end < 0)
                return false;

            int writeStart = code.IndexOf(outputWriteStartMarker, contentStart, StringComparison.Ordinal);
            if (writeStart < 0)
                writeStart = code.IndexOf(legacyOutputWriteStartMarker, contentStart, StringComparison.Ordinal);
            int writeEnd = writeStart >= 0 ? code.IndexOf(outputWriteEndMarker, writeStart, StringComparison.Ordinal) : -1;
            if (writeEnd < 0 && writeStart >= 0)
                writeEnd = code.IndexOf(legacyOutputWriteEndMarker, writeStart, StringComparison.Ordinal);
            if (writeStart >= 0 && writeStart < end && writeEnd >= 0 && writeEnd < end)
            {
                int replaceStart = code.LastIndexOf(Environment.NewLine, writeStart, StringComparison.Ordinal);
                replaceStart = replaceStart < contentStart ? writeStart : replaceStart;
                int replaceEnd = code.IndexOf(Environment.NewLine, writeEnd, StringComparison.Ordinal);
                string matchedEndMarker = code.IndexOf(outputWriteEndMarker, writeEnd, StringComparison.Ordinal) == writeEnd ? outputWriteEndMarker : legacyOutputWriteEndMarker;
                replaceEnd = replaceEnd < 0 || replaceEnd > end ? writeEnd + matchedEndMarker.Length : replaceEnd;
                tbxCode.Text = code.Substring(0, replaceStart)
                    + writeBack
                    + code.Substring(replaceEnd);
                return true;
            }

            int endLineStart = code.LastIndexOf(Environment.NewLine, end, StringComparison.Ordinal);
            if (endLineStart < contentStart)
                endLineStart = end;
            tbxCode.Text = code.Substring(0, endLineStart)
                + writeBack
                + code.Substring(endLineStart);
            return true;
        }

        private string GetCSharpType(CodeValueType type)
        {
            if (type == CodeValueType.Int) return "int";
            if (type == CodeValueType.String) return "string";
            if (type == CodeValueType.Bool) return "bool";
            return "double";
        }

        private bool ValidateItems()
        {
            HashSet<string> names = new HashSet<string>();
            for (int i = 0; i < dgvInput.Rows.Count; i++)
            {
                string name = GetCellString(dgvInput.Rows[i], "InputName");
                if (!ValidateName(name, "输入", names))
                    return false;
            }
            for (int i = 0; i < dgvOutput.Rows.Count; i++)
            {
                string name = GetCellString(dgvOutput.Rows[i], "OutputName");
                if (!ValidateName(name, "输出", names))
                    return false;
            }
            return true;
        }

        private bool ValidateName(string name, string itemType, HashSet<string> names)
        {
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show(itemType + "名称不能为空。", "脚本编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*$") || IsCSharpKeyword(name))
            {
                MessageBox.Show("“" + name + "”不是有效的变量名称，请使用字母、数字和下划线，且不能以数字开头。", "脚本编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (names.Contains(name))
            {
                MessageBox.Show("变量名称“" + name + "”重复。", "脚本编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            names.Add(name);
            return true;
        }

        private bool IsCSharpKeyword(string name)
        {
            const string keywords = " abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while ";
            return keywords.Contains(" " + name + " ");
        }

        private void SaveToolData()
        {
            List<CodeOutputItem> oldOutputItems = codeEditTool.L_outputItems;
            codeEditTool.L_inputItems = CreateInputItems();
            codeEditTool.L_outputItems = CreateOutputItems();
            for (int i = 0; i < codeEditTool.L_outputItems.Count; i++)
            {
                string oldGlobalName = string.Empty;
                string oldLocalName = string.Empty;
                bool oldWriteLocal = false;
                if (oldOutputItems != null && i < oldOutputItems.Count && oldOutputItems[i] != null)
                {
                    oldGlobalName = string.IsNullOrEmpty(oldOutputItems[i].GlobalVariableName) ? oldOutputItems[i].OutputName : oldOutputItems[i].GlobalVariableName;
                    oldLocalName = oldOutputItems[i].OutputName;
                    oldWriteLocal = oldOutputItems[i].WriteLocalVariable;
                }
                EnsureGlobalVariableForOutput(codeEditTool.L_outputItems[i], oldGlobalName);
                EnsureLocalVariableForOutput(codeEditTool.L_outputItems[i], oldLocalName, oldWriteLocal);
            }
            codeEditTool.L_calcItems.Clear();
            codeEditTool.sourceCode = NormalizeLineEndings(tbxCode.Text);
        }

        private List<CodeInputItem> CreateInputItems()
        {
            List<CodeInputItem> items = new List<CodeInputItem>();
            for (int i = 0; i < dgvInput.Rows.Count; i++)
            {
                CodeInputItem item = new CodeInputItem();
                item.InputName = GetCellString(dgvInput.Rows[i], "InputName");
                item.ValueType = ParseValueType(GetCellString(dgvInput.Rows[i], "ValueType"));
                item.SourceType = CodeInputSourceType.变量;
                item.FixedValue = string.Empty;
                item.VariableSource = GetCellString(dgvInput.Rows[i], "VariableSource");
                items.Add(item);
            }
            return items;
        }

        private List<CodeOutputItem> CreateOutputItems()
        {
            List<CodeOutputItem> items = new List<CodeOutputItem>();
            for (int i = 0; i < dgvOutput.Rows.Count; i++)
            {
                CodeOutputItem item = new CodeOutputItem();
                item.OutputName = GetCellString(dgvOutput.Rows[i], "OutputName");
                item.ValueType = ParseValueType(GetCellString(dgvOutput.Rows[i], "ValueType"));
                object writeGlobal = dgvOutput.Rows[i].Cells["WriteGlobalVariable"].Value;
                item.WriteGlobalVariable = writeGlobal != null && Convert.ToBoolean(writeGlobal);
                object writeLocal = dgvOutput.Rows[i].Cells["WriteLocalVariable"].Value;
                item.WriteLocalVariable = writeLocal != null && Convert.ToBoolean(writeLocal);
                item.GlobalVariableName = item.OutputName;
                items.Add(item);
            }
            return items;
        }

        private CodeValueType ParseValueType(string value)
        {
            CodeValueType result;
            return Enum.TryParse<CodeValueType>(value, out result) ? result : CodeValueType.Double;
        }

        private void EnsureGlobalVariableForOutput(CodeOutputItem item, string oldGlobalName)
        {
            if (!item.WriteGlobalVariable || string.IsNullOrEmpty(item.OutputName))
                return;
            Variable oldVariable = null;
            for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
            {
                Variable variable = Project.Instance.curEngine.globelVariable.L_variable[i];
                if (variable.variableType != 1)
                    continue;
                if (variable.name == item.OutputName)
                {
                    variable.type = item.ValueType.ToString();
                    return;
                }
                if (!string.IsNullOrEmpty(oldGlobalName) && variable.name == oldGlobalName)
                    oldVariable = variable;
            }

            if (oldVariable != null)
            {
                oldVariable.name = item.OutputName;
                oldVariable.type = item.ValueType.ToString();
                return;
            }

            Variable newVariable = new Variable(GetCustomVariableCount() + 1, item.ValueType.ToString(), item.OutputName);
            newVariable.variableType = 1;
            Project.Instance.curEngine.globelVariable.L_variable.Add(newVariable);
        }

        private int GetCustomVariableCount()
        {
            int count = 0;
            for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
                if (Project.Instance.curEngine.globelVariable.L_variable[i].variableType == 1)
                    count++;
            return count;
        }

        /// <summary>
        /// 保存时确保勾选“写入局部变量”的输出在所属流程中存在同名局部变量并同步类型；
        /// 输出改名时同步更名旧局部变量。取消勾选不删除已存在的局部变量。
        /// </summary>
        private void EnsureLocalVariableForOutput(CodeOutputItem item, string oldLocalName, bool oldWriteLocal)
        {
            if (!item.WriteLocalVariable || string.IsNullOrEmpty(item.OutputName))
                return;

            Job job = Project.Instance.curEngine.FindJobByName(jobName);
            if (job == null)
                return;

            if (oldWriteLocal && !string.IsNullOrEmpty(oldLocalName) && oldLocalName != item.OutputName)
                job.RenameLocalVariable(oldLocalName, item.OutputName);

            job.EnsureLocalVariable(item.OutputName, item.ValueType.ToString());
        }

        private void SyncJobTree()
        {
            Job job = Project.Instance.curEngine.FindJobByName(jobName);
            if (job != null)
                job.SyncCodeEditIONodes(toolName);
        }

        internal static void SyncInputSourceFromFlow(string sourceJobName, string sourceToolName, string inputName, string sourceText)
        {
            if (_instance == null || _instance.IsDisposed || !_instance.Visible)
                return;
            if (_instance.jobName != sourceJobName || _instance.toolName != sourceToolName)
                return;
            for (int i = 0; i < _instance.dgvInput.Rows.Count; i++)
            {
                DataGridViewRow row = _instance.dgvInput.Rows[i];
                if (_instance.GetCellString(row, "InputName") != inputName)
                    continue;
                row.Cells["VariableSource"].Value = sourceText;
                Job job = Project.Instance.curEngine.FindJobByName(sourceJobName);
                if (job != null)
                    row.Cells["ValueType"].Value = job.ResolveCodeEditSourceValueType(sourceText).ToString();
                break;
            }
        }

        private string GetCellString(DataGridViewRow row, string columnName)
        {
            object value = row.Cells[columnName].Value;
            return value == null ? string.Empty : value.ToString();
        }

        private string NormalizeLineEndings(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        }
    }
}
