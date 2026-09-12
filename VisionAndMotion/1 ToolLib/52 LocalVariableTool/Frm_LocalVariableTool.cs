using System;
using System.Drawing;
using System.Windows.Forms;

namespace VMPro
{
    /// <summary>
    /// 局部变量配置窗体：以表格编辑当前流程的局部变量。
    /// 列：变量名、变量类型（Int/Double/Bool/String）、当前值、备注；
    /// 支持添加变量与右键删除行；编辑即时写回 Job.localVariables，随项目保存持久化。
    /// </summary>
    internal class Frm_LocalVariableTool : Frm_FormBase
    {
        internal string jobName = string.Empty;
        internal string toolName = string.Empty;

        private static Frm_LocalVariableTool _instance;
        internal static Frm_LocalVariableTool Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_LocalVariableTool();
                return _instance;
            }
        }

        private DataGridView dgvVariable = new DataGridView();
        private Button btnAddVariable = new Button();
        private bool loadingData;
        /// <summary>右键待删除的行索引；菜单点击时使用。</summary>
        private int pendingDeleteRowIndex = -1;
        private ContextMenuStrip rowDeleteMenu = new ContextMenuStrip();

        private Frm_LocalVariableTool()
        {
            InitializeComponent();
            ToolStripItem deleteRowItem = rowDeleteMenu.Items.Add("删除该行");
            deleteRowItem.Click += deleteRowMenuItem_Click;
        }

        private void InitializeComponent()
        {
            Text = "局部变量";
            Size = new Size(660, 500);
            MinimumSize = new Size(520, 360);
            StartPosition = FormStartPosition.CenterScreen;

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 1;
            panel.RowCount = 2;
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Controls.Add(panel);

            btnAddVariable.Text = "添加变量";
            btnAddVariable.Dock = DockStyle.Fill;
            btnAddVariable.Click += btnAddVariable_Click;
            panel.Controls.Add(btnAddVariable, 0, 0);

            dgvVariable.Dock = DockStyle.Fill;
            dgvVariable.AllowUserToAddRows = false;
            dgvVariable.AllowUserToResizeRows = false;
            dgvVariable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVariable.RowHeadersVisible = false;
            dgvVariable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVariable.MultiSelect = false;
            dgvVariable.CellValueChanged += dgvVariable_CellValueChanged;
            dgvVariable.CurrentCellDirtyStateChanged += dgvVariable_CurrentCellDirtyStateChanged;
            dgvVariable.CellMouseClick += dgvVariable_CellMouseClick;

            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.Name = "VarName";
            nameColumn.HeaderText = "变量名";
            nameColumn.FillWeight = 24;
            dgvVariable.Columns.Add(nameColumn);

            DataGridViewComboBoxColumn typeColumn = new DataGridViewComboBoxColumn();
            typeColumn.Name = "VarType";
            typeColumn.HeaderText = "变量类型";
            typeColumn.DataSource = new string[] { "Int", "Double", "Bool", "String" };
            typeColumn.FillWeight = 18;
            dgvVariable.Columns.Add(typeColumn);

            DataGridViewTextBoxColumn valueColumn = new DataGridViewTextBoxColumn();
            valueColumn.Name = "VarValue";
            valueColumn.HeaderText = "当前值";
            valueColumn.FillWeight = 26;
            dgvVariable.Columns.Add(valueColumn);

            DataGridViewTextBoxColumn remarkColumn = new DataGridViewTextBoxColumn();
            remarkColumn.Name = "VarRemark";
            remarkColumn.HeaderText = "备注";
            remarkColumn.FillWeight = 32;
            dgvVariable.Columns.Add(remarkColumn);

            panel.Controls.Add(dgvVariable, 0, 1);
        }

        /// <summary>
        /// 从所属流程的局部变量集合刷新表格（行 Tag 直接绑定 LocalVariableItem）。
        /// </summary>
        internal void LoadToolData()
        {
            loadingData = true;
            try
            {
                dgvVariable.Rows.Clear();
                Job job = FindBoundJob();
                if (job == null)
                    return;
                lock (Job.LocalVariableSync)
                {
                    for (int i = 0; i < job.localVariables.Count; i++)
                    {
                        LocalVariableItem item = job.localVariables[i];
                        if (item == null)
                            continue;
                        int index = dgvVariable.Rows.Add();
                        DataGridViewRow row = dgvVariable.Rows[index];
                        row.Tag = item;
                        row.Cells["VarName"].Value = item.name;
                        row.Cells["VarType"].Value = string.IsNullOrEmpty(item.valueType) ? "Int" : item.valueType;
                        row.Cells["VarValue"].Value = item.value == null ? string.Empty : item.value.ToString();
                        row.Cells["VarRemark"].Value = item.remark;
                    }
                }
            }
            finally
            {
                loadingData = false;
            }
        }

        private Job FindBoundJob()
        {
            if (Project.Instance == null || Project.Instance.curEngine == null ||
                Project.Instance.curEngine.L_jobList == null)
                return null;
            for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
            {
                if (Project.Instance.curEngine.L_jobList[i] != null &&
                    Project.Instance.curEngine.L_jobList[i].jobName == jobName)
                    return Project.Instance.curEngine.L_jobList[i];
            }
            return null;
        }

        private void btnAddVariable_Click(object sender, EventArgs e)
        {
            Job job = FindBoundJob();
            if (job == null)
            {
                Frm_MessageBox.Instance.MessageBoxShow("\r\n未找到所属流程，无法添加局部变量");
                return;
            }

            string name = GetUniqueName();
            job.EnsureLocalVariable(name, "Int");
            LoadToolData();
        }

        /// <summary>
        /// 生成本流程内不重复的默认变量名（变量1、变量2……）。
        /// </summary>
        private string GetUniqueName()
        {
            int index = 1;
            while (true)
            {
                string name = "变量" + index.ToString();
                bool exists = false;
                for (int i = 0; i < dgvVariable.Rows.Count; i++)
                {
                    if (GetCellString(dgvVariable.Rows[i], "VarName") == name)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                    return name;
                index++;
            }
        }

        private void dgvVariable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (loadingData || e.RowIndex < 0)
                return;

            LocalVariableItem item = dgvVariable.Rows[e.RowIndex].Tag as LocalVariableItem;
            if (item == null)
                return;

            string columnName = dgvVariable.Columns[e.ColumnIndex].Name;
            if (columnName == "VarName")
            {
                string name = GetCellString(dgvVariable.Rows[e.RowIndex], "VarName");
                if (!IsValidName(name, item))
                {
                    // 名称非法时不写回模型，直接按模型刷新还原显示。
                    LoadToolData();
                    return;
                }
                item.name = name;
            }
            else if (columnName == "VarType")
            {
                item.valueType = GetCellString(dgvVariable.Rows[e.RowIndex], "VarType");
            }
            else if (columnName == "VarValue")
            {
                item.value = GetCellString(dgvVariable.Rows[e.RowIndex], "VarValue");
            }
            else if (columnName == "VarRemark")
            {
                item.remark = GetCellString(dgvVariable.Rows[e.RowIndex], "VarRemark");
            }
        }

        /// <summary>
        /// 变量名要求：非空且本流程内唯一（允许中文，与全局变量一致）。
        /// </summary>
        private bool IsValidName(string name, LocalVariableItem self)
        {
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("变量名不能为空。", "局部变量", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            for (int i = 0; i < dgvVariable.Rows.Count; i++)
            {
                LocalVariableItem other = dgvVariable.Rows[i].Tag as LocalVariableItem;
                if (other == null || other == self)
                    continue;
                if (GetCellString(dgvVariable.Rows[i], "VarName") == name)
                {
                    MessageBox.Show("变量名“" + name + "”已存在，请更换名称。", "局部变量", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        private void dgvVariable_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvVariable.IsCurrentCellDirty)
                dgvVariable.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// <summary>
        /// 右键某一行时选中该行并弹出删除菜单。
        /// </summary>
        private void dgvVariable_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            pendingDeleteRowIndex = -1;
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.RowIndex >= dgvVariable.Rows.Count)
                return;
            pendingDeleteRowIndex = e.RowIndex;
            dgvVariable.ClearSelection();
            dgvVariable.Rows[e.RowIndex].Selected = true;
            rowDeleteMenu.Show(dgvVariable, dgvVariable.PointToClient(Cursor.Position));
        }

        private void deleteRowMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingDeleteRowIndex < 0 || pendingDeleteRowIndex >= dgvVariable.Rows.Count)
                return;

            LocalVariableItem item = dgvVariable.Rows[pendingDeleteRowIndex].Tag as LocalVariableItem;
            Job job = FindBoundJob();
            if (item != null && job != null && !string.IsNullOrEmpty(item.name))
                job.RemoveLocalVariable(item.name);

            dgvVariable.Rows.RemoveAt(pendingDeleteRowIndex);
            pendingDeleteRowIndex = -1;
        }

        private string GetCellString(DataGridViewRow row, string columnName)
        {
            object value = row.Cells[columnName].Value;
            return value == null ? string.Empty : value.ToString();
        }
    }
}
