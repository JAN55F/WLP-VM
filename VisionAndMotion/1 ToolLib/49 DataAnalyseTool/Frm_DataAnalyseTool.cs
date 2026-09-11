using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    /// <summary>
    /// 数据分析工具窗体：单表编辑分析项（与数据显示同交互）。
    /// 每行 = 一个链接的数据源 + 范围判定（值下限~值上限，留空=单边不限）+ 限内/限外结果文本。
    /// 运行后每行的判定结果写入该工具的"输出项N"，供下游工具/脚本链接（可再存入全局变量）。
    /// </summary>
    internal partial class Frm_DataAnalyseTool : Frm_FormBase
    {
        internal Frm_DataAnalyseTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 工具对象
        /// </summary>
        internal static DataAnalyseTool dataAnalyseTool = new DataAnalyseTool();
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_DataAnalyseTool _instance;
        public static Frm_DataAnalyseTool Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_DataAnalyseTool();
                return _instance;
            }
        }

        /// <summary>
        /// 打开窗体时由流程编辑器调用：设置流程/工具名，绑定工具实例并回填表格。
        /// </summary>
        internal void LoadPar(string jobName, string toolName, DataAnalyseTool toolInstance)
        {
            this.jobName = jobName;
            this.toolName = toolName;
            dataAnalyseTool = toolInstance ?? new DataAnalyseTool();
            dataAnalyseTool.jobName = jobName;
            dataAnalyseTool.toolName = toolName;
            FillGridFromTool();
        }

        private void FillGridFromTool()
        {
            dgv_outputItem.Rows.Clear();
            List<DataAnalyseTool.ItemRowData> rows = dataAnalyseTool.ExportRows();
            foreach (DataAnalyseTool.ItemRowData row in rows)
            {
                int index = dgv_outputItem.Rows.Add(row.LinkDisplay, string.Empty, row.Down, row.Up, row.InResult, row.OutResult);
                // 行 Tag 携带链接来源，后续编辑保存时依赖它恢复链接
                dgv_outputItem.Rows[index].Tag = row;
            }
        }

        /// <summary>
        /// 网格 → 工具（L_items + input/output 条目 + 流程树/连线刷新）。
        /// </summary>
        private void SaveGridToTool()
        {
            if (Job.loadForm)
                return;

            List<DataAnalyseTool.ItemRowData> rows = new List<DataAnalyseTool.ItemRowData>();
            for (int i = 0; i < dgv_outputItem.Rows.Count; i++)
            {
                DataGridViewRow gridRow = dgv_outputItem.Rows[i];

                DataAnalyseTool.ItemRowData row = new DataAnalyseTool.ItemRowData();
                // 来源双保险：行 Tag 优先，缺失时从工具现有链接推导
                DataAnalyseTool.ItemRowData previous = gridRow.Tag as DataAnalyseTool.ItemRowData;
                if (previous == null)
                {
                    List<DataAnalyseTool.ItemRowData> existing = dataAnalyseTool.ExportRows();
                    if (i < existing.Count)
                        previous = existing[i];
                }
                if (previous != null)
                {
                    row.SourceTool = previous.SourceTool;
                    row.SourceOutput = previous.SourceOutput;
                    row.LinkDisplay = previous.LinkDisplay;
                }

                row.Down = CellText(gridRow, "colDown", "");
                row.Up = CellText(gridRow, "colUp", "");
                row.InResult = CellText(gridRow, "colInResult", "OK");
                row.OutResult = CellText(gridRow, "colOutResult", "NG");
                rows.Add(row);
            }
            dataAnalyseTool.ImportRows(rows);

            // 把工具导出的规范行写回（含 LinkDisplay 等推导值），并保持每行 Tag 供后续保存
            List<DataAnalyseTool.ItemRowData> exported = dataAnalyseTool.ExportRows();
            for (int i = 0; i < exported.Count && i < dgv_outputItem.Rows.Count; i++)
                dgv_outputItem.Rows[i].Tag = exported[i];
        }

        private static string CellText(DataGridViewRow row, string columnName, string fallback)
        {
            object value = row.Cells[columnName].Value;
            return value == null ? fallback : value.ToString();
        }

        /// <summary>
        /// 链接按钮：打开选择器（全局变量 + 此工具之前的工具输出）。
        /// </summary>
        private void dgv_outputItem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgv_outputItem.Columns[e.ColumnIndex].Name != "colLink")
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
                Log.SaveError(ex, "数据分析工具链接数据源");
            }
        }

        /// <summary>
        /// 把某行链接到指定数据源：回填显示文本、写回工具（L_items + input/output 条目 + 连线刷新）。
        /// </summary>
        internal void ApplyRowLink(int rowIndex, string sourceTool, string sourceOutput, string linkDisplay)
        {
            if (rowIndex < 0 || rowIndex >= dgv_outputItem.Rows.Count || dgv_outputItem.Rows[rowIndex].IsNewRow)
                return;

            DataAnalyseTool.ItemRowData row = dgv_outputItem.Rows[rowIndex].Tag as DataAnalyseTool.ItemRowData;
            if (row == null)
            {
                row = new DataAnalyseTool.ItemRowData();
                dgv_outputItem.Rows[rowIndex].Tag = row;
            }
            row.SourceTool = sourceTool;
            row.SourceOutput = sourceOutput;
            row.LinkDisplay = linkDisplay;

            dgv_outputItem.Rows[rowIndex].Cells["colLinkDisplay"].Value = row.LinkDisplay;
            SaveGridToTool();
        }

        /// <summary>
        /// 增加输入项：追加一行默认值。
        /// </summary>
        private void tsb_addInput_Click(object sender, EventArgs e)
        {
            try
            {
                dgv_outputItem.Rows.Add(string.Empty, string.Empty, string.Empty, string.Empty, "OK", "NG");
                SaveGridToTool();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据分析工具增加输入项");
            }
        }

        /// <summary>
        /// 右键行 → 删除此行（同时同步 input 条目与连线；输出条目只增不减，避免破坏下游链接）。
        /// </summary>
        private void menuDeleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (cms_row.Tag == null)
                    return;
                int rowIndex = (int)cms_row.Tag;
                if (rowIndex < 0 || rowIndex >= dgv_outputItem.Rows.Count)
                    return;

                dgv_outputItem.Rows.RemoveAt(rowIndex);
                SaveGridToTool();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据分析工具删除输入项");
            }
        }

        private void dgv_outputItem_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                cms_row.Tag = e.RowIndex;
                dgv_outputItem.CurrentCell = dgv_outputItem.Rows[e.RowIndex].Cells[Math.Max(0, Math.Min(e.ColumnIndex, dgv_outputItem.Columns.Count - 1))];
            }
            else if (e.Button == MouseButtons.Right)
            {
                cms_row.Tag = null;
            }
        }

        private void dgv_outputItem_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                SaveGridToTool();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex, "数据分析工具编辑输入项");
            }
        }

        private void dgv_outputItem_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgv_outputItem.IsCurrentCellDirty)
                dgv_outputItem.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        internal override void btn_baseClose_Click(object sender, EventArgs e)
        {
            try
            {
                base.btn_baseClose_Click(sender, e);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            try
            {
                Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = true;
                this.pic_onOff.Image = Resources.开;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pic_onOff_Click(object sender, EventArgs e)
        {
            try
            {
                if (Job.loadForm)
                    return;

                bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
                Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = !enable;
                if (!enable)
                    pic_onOff.Image = Resources.开;
                else
                    pic_onOff.Image = Resources.关;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                dataAnalyseTool.Run(true, true, toolName);
                long time = sw.ElapsedMilliseconds;

                if (dataAnalyseTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                {
                    lbl_toolTip.ForeColor = Color.Red;
                    lbl_runTime.Text = string.Format("耗时：0ms");
                }
                else
                {
                    lbl_toolTip.ForeColor = Color.Black;
                    lbl_runTime.Text = string.Format("耗时：{0}ms", time.ToString());
                }
                lbl_toolTip.Text = "状态：" + dataAnalyseTool.toolRunStatu.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void btn_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }
    }
}
