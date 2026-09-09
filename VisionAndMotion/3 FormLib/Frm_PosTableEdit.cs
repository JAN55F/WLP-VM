using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMPro
{
    public partial class Frm_PosTableEdit : Frm_FormBase
    {
        private bool tableListRefreshedForVisibleCycle;

        public Frm_PosTableEdit()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_PosTableEdit _instance;
        public static Frm_PosTableEdit Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_PosTableEdit();
                return _instance;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void Frm_PosTableEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!Visible)
            {
                tableListRefreshedForVisibleCycle = false;
                return;
            }

            if (!tableListRefreshedForVisibleCycle)
            {
                RefreshSmartPositionTableList();
                tableListRefreshedForVisibleCycle = true;
            }
        }

        /// <summary>
        /// 在点表编辑器显示时从当前方案重建列表，避免启动阶段创建隐藏窗体。
        /// </summary>
        internal void RefreshSmartPositionTableList()
        {
            try
            {
                string selectedTableName = string.Empty;
                if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Cells[1].Value != null)
                    selectedTableName = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();

                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
                SmartPosTable smartPositionTable = Project.Instance.curEngine.smartPosTable;
                for (int i = 0; i < smartPositionTable.L_Table.Count; i++)
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells[0].Value = i + 1;
                    dataGridView1.Rows[rowIndex].Cells[1].Value = smartPositionTable.L_Table[i].tableName;
                }

                if (dataGridView1.Rows.Count == 0)
                    return;

                int selectedIndex = 0;
                for (int i = 0; i < smartPositionTable.L_Table.Count; i++)
                {
                    if (string.Equals(smartPositionTable.L_Table[i].tableName, selectedTableName, StringComparison.Ordinal))
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                dataGridView1.ClearSelection();
                dataGridView1.Rows[selectedIndex].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[selectedIndex].Cells[1];
                Table selectedTable = smartPositionTable.L_Table[selectedIndex];
                for (int i = 0; i < selectedTable.L_axis.Count; i++)
                {
                    int rowIndex = dataGridView2.Rows.Add();
                    dataGridView2.Rows[rowIndex].Cells[0].Value = i + 1;
                    dataGridView2.Rows[rowIndex].Cells[1].Value = selectedTable.L_axis[i];
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Frm_InputMessage.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "Please input job's name" : "请输入新表名");
                Frm_InputMessage.Instance.btn_confirm.Text = (Project.Instance.configuration.language == Language.English ? "Confirm" : "确定");
                Frm_InputMessage.Instance.passwordChar = false;
                Frm_InputMessage.Instance.txt_input.TextStr = string.Empty;
                Frm_InputMessage.Instance.ShowDialog();
                string pointFormName = Frm_InputMessage.input;
                if (pointFormName == "")
                    return;

                //查重
                if (Project.Instance.curEngine.smartPosTable.TableExist(pointFormName))
                {
                    Frm_MessageBox.Instance.MessageBoxShow(string.Format("\r\n点表中已存在名为{0}的表，请勿重复添加", pointFormName));
                    return;
                }

                Frm_MotionControl.Instance.comboBox1.Add(pointFormName);
                Project.Instance.curEngine.smartPosTable.L_Table.Add(new Table(pointFormName));


                if (Frm_MotionControl.Instance.comboBox1.TextStr == string.Empty)
                    Frm_MotionControl.Instance.comboBox1.SelectedIndex = 0;

                int idx = dataGridView1.Rows.Add();
                dataGridView1.Rows[idx].Cells[0].Value = dataGridView1.Rows.Count + 1;
                dataGridView1.Rows[idx].Cells[1].Value = pointFormName;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                //查重
                if (Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis.Contains(comboBox2.Text))
                {
                    Frm_MessageBox.Instance.MessageBoxShow(string.Format("\r\n此表中已存在{0}轴，请勿重复添加", comboBox2.Text));
                    return;
                }

                if (Frm_MotionControl.Instance.comboBox1.TextStr == dataGridView1.SelectedRows[0].Cells[1].Value.ToString())
                {
                    int idx = Frm_MotionControl.Instance.dgv_pointList.Columns.Add("", comboBox2.Text);
                    Frm_MotionControl.Instance.dgv_pointList.Columns[idx].Width = 70;
                }
                Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis.Add(comboBox2.Text);

                int idx1 = dataGridView2.Rows.Add();
                dataGridView2.Rows[idx1].Cells[0].Value = dataGridView1.Rows.Count + 1;
                dataGridView2.Rows[idx1].Cells[1].Value = comboBox2.Text;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex );
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView2.Rows.Clear();
                for (int i = 0; i < Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis.Count; i++)
                {
                    int idx = dataGridView2.Rows.Add();
                    dataGridView2.Rows[idx].Cells[0].Value = i + 1;
                    dataGridView2.Rows[idx].Cells[1].Value = Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis[i];
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                int idx = dataGridView1.SelectedRows[0].Index;
                if (idx < 0)
                    return;

                if (idx == 0)
                    return;

                Table temp = Project.Instance.curEngine.smartPosTable.L_Table[idx - 1];
                Project.Instance.curEngine.smartPosTable.L_Table[idx - 1] = Project.Instance.curEngine.smartPosTable.L_Table[idx];
                Project.Instance.curEngine.smartPosTable.L_Table[idx] = temp;

                string obj = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index - 1].Cells[1].Value.ToString();
                dataGridView1.Rows[dataGridView1.SelectedRows[0].Index - 1].Cells[1].Value = dataGridView1.SelectedRows[0].Cells[1].Value;
                dataGridView1.SelectedRows[0].Cells[1].Value = obj;

                dataGridView1.Rows[idx - 1].Selected = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                int idx = dataGridView1.SelectedRows[0].Index;
                if (idx < 0)
                    return;

                if (idx == dataGridView1.Rows.Count - 1)
                    return;

                Table temp = Project.Instance.curEngine.smartPosTable.L_Table[idx + 1];
                Project.Instance.curEngine.smartPosTable.L_Table[idx + 1] = Project.Instance.curEngine.smartPosTable.L_Table[idx];
                Project.Instance.curEngine.smartPosTable.L_Table[idx] = temp;

                string obj = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index + 1].Cells[1].Value.ToString();
                dataGridView1.Rows[dataGridView1.SelectedRows[0].Index + 1].Cells[1].Value = dataGridView1.SelectedRows[0].Cells[1].Value;
                dataGridView1.SelectedRows[0].Cells[1].Value = obj;

                dataGridView1.Rows[idx + 1].Selected = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                int idx = dataGridView1.SelectedRows[0].Index;
                dataGridView1.Rows.RemoveAt(idx);
                Project.Instance.curEngine.smartPosTable.L_Table.RemoveAt(idx);
                //////Frm_MotionControl.Instance.comboBox1.RemoveAt(idx);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            int idx = dataGridView2.SelectedRows[0].Index;
            if (idx < 0)
                return;

            if (idx == 0)
                return;

            string temp = Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis[idx - 1];
            Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis[idx - 1] = Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis[idx];
            Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis[idx] = temp;

            string obj = dataGridView2.Rows[dataGridView2.SelectedRows[0].Index - 1].Cells[1].Value.ToString();
            dataGridView2.Rows[dataGridView2.SelectedRows[0].Index - 1].Cells[1].Value = dataGridView2.SelectedRows[0].Cells[1].Value;
            dataGridView2.SelectedRows[0].Cells[1].Value = obj;

            Project.Instance.curEngine.smartPosTable.LoadData(Frm_MotionControl.Instance.dgv_pointList, Frm_MotionControl.Instance.comboBox1.TextStr);

            dataGridView2.Rows[idx - 1].Selected = true;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                int idx = dataGridView2.SelectedRows[0].Index;
                if (idx < 0)
                    return;

                if (idx == dataGridView2.Rows.Count - 1)
                    return;

                string temp = Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis[idx + 1];
                Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis[idx + 1] = Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis[idx];
                Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis[idx] = temp;

                string obj = dataGridView2.Rows[dataGridView2.SelectedRows[0].Index + 1].Cells[1].Value.ToString();
                dataGridView2.Rows[dataGridView2.SelectedRows[0].Index + 1].Cells[1].Value = dataGridView2.SelectedRows[0].Cells[1].Value;
                dataGridView2.SelectedRows[0].Cells[1].Value = obj;

                Project.Instance.curEngine.smartPosTable.LoadData(Frm_MotionControl.Instance.dgv_pointList, Frm_MotionControl.Instance.comboBox1.TextStr);

                dataGridView2.Rows[idx + 1].Selected = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Project.Instance.curEngine.smartPosTable.FindTable(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()).L_axis.RemoveAt(dataGridView2.SelectedRows[0].Index);
            dataGridView2.Rows.RemoveAt(dataGridView2.SelectedRows[0].Index);
            Frm_MotionControl.Instance.dgv_pointList.Columns.RemoveAt(dataGridView2.SelectedRows[0].Index + 3);
        }

        private void Frm_PosTableEdit_Load(object sender, EventArgs e)
        {
            if (!tableListRefreshedForVisibleCycle)
            {
                RefreshSmartPositionTableList();
                tableListRefreshedForVisibleCycle = true;
            }
        }
    }
}
