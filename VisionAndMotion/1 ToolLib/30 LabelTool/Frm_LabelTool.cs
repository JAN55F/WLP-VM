using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    /// <summary>
    /// 数据显示工具窗体：单表编辑显示项。
    /// 每行 = 一个链接的数据源 + 图像上的显示位置/颜色/字号 + 按行选择的条件判定（无/范围/布尔）。
    /// "链接"按钮打开 Frm_LinkPicker（全局变量 + 当前流程中此工具之前的工具输出），
    /// 选择后写回工具的 input 条目并触发流程编辑器自动连线。
    /// 行支持右键"删除此行"。
    /// </summary>
    internal partial class Frm_LabelTool : Frm_FormBase
    {
        public Frm_LabelTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_LabelTool _instance;
        public static Frm_LabelTool Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_LabelTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象（打开窗体时绑定到实际工具实例）
        /// </summary>
        internal static LabelTool labelTool = new LabelTool();

        /// <summary>
        /// 打开窗体时由流程编辑器调用：设置流程/工具名，绑定工具实例并回填表格。
        /// </summary>
        internal void LoadPar(string jobName, string toolName, LabelTool toolInstance)
        {
            this.jobName = jobName;
            this.toolName = toolName;
            labelTool = toolInstance ?? new LabelTool();
            labelTool.jobName = jobName;
            labelTool.toolName = toolName;
            FillGridFromTool();
        }

        /// <summary>
        /// 工具 → 网格。
        /// </summary>
        private void FillGridFromTool()
        {
            dgv_items.Rows.Clear();
            List<LabelTool.LabelRowData> rows = labelTool.ExportRows();
            foreach (LabelTool.LabelRowData row in rows)
            {
                int index = dgv_items.Rows.Add(row.LinkDisplay, string.Empty, row.Row, row.Col, row.OkColor, row.NgColor, row.FontSize, ToJudgeModeText(row.JudgeMode), row.Down, row.Up);
                // 关键：行 Tag 携带链接来源（SourceTool/SourceOutput），后续编辑保存时依赖它恢复链接；
                // 缺失会导致改行列时把已链接的行当成“未链接”写回，链接失效、流程运行到本工具被终止。
                dgv_items.Rows[index].Tag = row;
            }
        }

        private static string ToJudgeModeText(string judgeMode)
        {
            if (judgeMode == "Range") return "范围";
            if (judgeMode == "Bool") return "布尔";
            return "无";
        }

        private static string ToJudgeModeValue(string text)
        {
            if (text == "范围") return "Range";
            if (text == "布尔") return "Bool";
            return "None";
        }

        /// <summary>
        /// 网格 → 工具（L_label + input 条目 + 流程树/连线刷新）。
        /// </summary>
        private void SaveGridToTool()
        {
            if (Job.loadForm)
                return;

            List<LabelTool.LabelRowData> rows = new List<LabelTool.LabelRowData>();
            for (int i = 0; i < dgv_items.Rows.Count; i++)
            {
                DataGridViewRow gridRow = dgv_items.Rows[i];
                if (gridRow.IsNewRow)
                    continue;

                LabelTool.LabelRowData row = new LabelTool.LabelRowData();
                // 来源双保险：行 Tag 优先，缺失时从工具现有链接推导，避免既有链接被误清
                LabelTool.LabelRowData previous = gridRow.Tag as LabelTool.LabelRowData;
                if (previous == null)
                {
                    List<LabelTool.LabelRowData> existing = labelTool.ExportRows();
                    if (i < existing.Count)
                        previous = existing[i];
                }
                if (previous != null)
                {
                    row.SourceTool = previous.SourceTool;
                    row.SourceOutput = previous.SourceOutput;
                    row.LinkDisplay = previous.LinkDisplay;
                }

                row.Row = CellText(gridRow, "colRow", "100");
                row.Col = CellText(gridRow, "colCol", "100");
                row.OkColor = CellText(gridRow, "colOkColor", "green");
                row.NgColor = CellText(gridRow, "colNgColor", "red");
                row.FontSize = CellText(gridRow, "colFontSize", "10");
                row.JudgeMode = ToJudgeModeValue(CellText(gridRow, "colJudgeMode", "无"));
                row.Down = CellText(gridRow, "colDown", "");
                row.Up = CellText(gridRow, "colUp", "");
                rows.Add(row);
            }
            labelTool.ImportRows(rows);

            // 把工具导出的规范行写回（含 LinkDisplay 等推导值），并保持每行 Tag 供后续保存
            List<LabelTool.LabelRowData> exported = labelTool.ExportRows();
            for (int i = 0; i < exported.Count && i < dgv_items.Rows.Count - (dgv_items.AllowUserToAddRows ? 1 : 0); i++)
                dgv_items.Rows[i].Tag = exported[i];
        }

        private static string CellText(DataGridViewRow row, string columnName, string fallback)
        {
            object value = row.Cells[columnName].Value;
            return value == null ? fallback : value.ToString();
        }

        /// <summary>
        /// 增加输入项：追加一行默认值。
        /// </summary>
        private void tsb_addInput_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dgv_items.Rows.Add(string.Empty, string.Empty, "100", "100", "green", "red", "10", "无", string.Empty, string.Empty);
                dgv_items.CurrentCell = dgv_items.Rows[index].Cells["colRow"];
                SaveGridToTool();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据显示工具增加输入项");
            }
        }

        /// <summary>
        /// 链接按钮：打开选择器（全局变量 + 此工具之前的工具输出）。
        /// </summary>
        private void dgv_items_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgv_items.Columns[e.ColumnIndex].Name != "colLink")
                    return;

                using (Frm_LinkPicker picker = new Frm_LinkPicker(jobName, toolName))
                {
                    if (picker.ShowDialog(this) != DialogResult.OK || string.IsNullOrEmpty(picker.SelectedSource))
                        return;

                    ApplyRowLink(e.RowIndex, picker.SelectedSourceTool, picker.SelectedSourceOutput, picker.SelectedDisplay);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据显示工具链接数据源");
            }
        }

        /// <summary>
        /// 把某行链接到指定数据源：回填显示文本、写回工具（L_label + input 条目 + 连线刷新）。
        /// </summary>
        internal void ApplyRowLink(int rowIndex, string sourceTool, string sourceOutput, string linkDisplay)
        {
            if (rowIndex < 0 || rowIndex >= dgv_items.Rows.Count || dgv_items.Rows[rowIndex].IsNewRow)
                return;

            LabelTool.LabelRowData row = dgv_items.Rows[rowIndex].Tag as LabelTool.LabelRowData;
            if (row == null)
            {
                row = new LabelTool.LabelRowData();
                dgv_items.Rows[rowIndex].Tag = row;
            }
            row.SourceTool = sourceTool;
            row.SourceOutput = sourceOutput;
            row.LinkDisplay = linkDisplay;

            dgv_items.Rows[rowIndex].Cells["colLinkDisplay"].Value = row.LinkDisplay;
            SaveGridToTool();
        }

        /// <summary>
        /// 右键行 → 删除此行（同时同步 input 条目与连线）。
        /// </summary>
        private void menuDeleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (cms_row.Tag == null)
                    return;
                int rowIndex = (int)cms_row.Tag;
                if (rowIndex < 0 || rowIndex >= dgv_items.Rows.Count || dgv_items.Rows[rowIndex].IsNewRow)
                    return;

                dgv_items.Rows.RemoveAt(rowIndex);
                SaveGridToTool();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据显示工具删除输入项");
            }
        }

        private void dgv_items_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                // 判定方式切换时维护下限/上限的可用性（非"范围"行灰显）
                if (dgv_items.Columns[e.ColumnIndex].Name == "colJudgeMode")
                {
                    string mode = Convert.ToString(dgv_items.Rows[e.RowIndex].Cells["colJudgeMode"].Value);
                    bool rangeEnabled = mode == "范围";
                    dgv_items.Rows[e.RowIndex].Cells["colDown"].ReadOnly = !rangeEnabled;
                    dgv_items.Rows[e.RowIndex].Cells["colUp"].ReadOnly = !rangeEnabled;
                    if (!rangeEnabled)
                    {
                        dgv_items.Rows[e.RowIndex].Cells["colDown"].Value = string.Empty;
                        dgv_items.Rows[e.RowIndex].Cells["colUp"].Value = string.Empty;
                    }
                }
                SaveGridToTool();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据显示工具编辑输入项");
            }
        }

        private void dgv_items_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // 下拉/按钮类单元格点选后立即提交，保证 CellValueChanged 即时触发
            if (dgv_items.IsCurrentCellDirty)
                dgv_items.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgv_items_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 右键行时记住行号，供"删除此行"使用；同时选中该行
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && !dgv_items.Rows[e.RowIndex].IsNewRow)
            {
                cms_row.Tag = e.RowIndex;
                dgv_items.CurrentCell = dgv_items.Rows[e.RowIndex].Cells[Math.Max(0, Math.Min(e.ColumnIndex, dgv_items.Columns.Count - 1))];
            }
            else if (e.Button == MouseButtons.Right)
            {
                cms_row.Tag = null;
            }
        }

        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }

        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            labelTool.Run(true, true, toolName);
        }
        private void btn_runTool_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            labelTool.Run(true, true, toolName);
            label4.Text = string.Format("耗时：{0}ms", sw.ElapsedMilliseconds.ToString());
            if (labelTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                label9.ForeColor = Color.Red;
            else
                label9.ForeColor = Color.Black;
            label9.Text = "状态：" + labelTool.toolRunStatu.ToString();
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            RefreshTitleButtonVisuals();
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
