using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMPro
{
    [Serializable]
    public class SmartPosTable
    {
        public double velPer = 1;
        public List<Table> L_Table = new List<Table>();
        internal Table FindTable(string tableName)
        {
            for (int i = 0; i < L_Table.Count; i++)
            {
                if (L_Table[i].tableName == tableName)
                    return L_Table[i];
            }
            return new Table("");
        }

        internal bool TableExist(string tableName)
        {
            for (int i = 0; i < L_Table.Count; i++)
            {
                if (L_Table[i].tableName == tableName)
                    return true;
            }
            return false;
        }
        internal static void GoPos(string tableName, string posName)
        {
            for (int i = 0; i < Project.Instance.curEngine.smartPosTable.L_Table.Count; i++)
            {
                if (Project.Instance.curEngine.smartPosTable.L_Table[i].tableName == tableName)
                {
                    for (int j = 0; j < Project.Instance.curEngine.smartPosTable.L_Table[i].L_pos.Count; j++)
                    {
                        if (Project.Instance.curEngine.smartPosTable.L_Table[i].L_pos[j].posName == posName)
                        {
                            switch (Project.Instance.configuration.cardType)
                            {
                                case CardType.安川_MP3100:
                                    for (int k = 0; k < Project.Instance.curEngine.smartPosTable.L_Table[i].L_pos[j].L_point.Count; k++)
                                    {
                                        if (Project.Instance.curEngine.smartPosTable.L_Table[i].L_axis[k] == "Y")
                                            Card_Ymc3100.YMoveAbs(Project.Instance.curEngine.smartPosTable.L_Table[i].L_pos[j].L_point[k], (int)(Project.Instance.curEngine.smartPosTable.L_Table[i].L_pos[j].vel * Project.Instance.curEngine.smartPosTable.velPer));
                                        else
                                            Card_Ymc3100.MoveAbs(Project.Instance.curEngine.smartPosTable.L_Table[i].L_axis[k], Project.Instance.curEngine.smartPosTable.L_Table[i].L_pos[j].L_point[k], (int)(Project.Instance.curEngine.smartPosTable.L_Table[i].L_pos[j].vel * Project.Instance.curEngine.smartPosTable.velPer), true);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

        }
        internal static void GoPos(int tableIndex, int posIndex)
        {

            switch (Project.Instance.configuration.cardType)
            {
                case CardType.安川_MP3100:
                    for (int i = 0; i < Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[posIndex].L_point.Count; i++)
                    {
                        if (Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_axis[i] == "Y")
                            Card_Ymc3100.YMoveAbs(Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[posIndex].L_point[i], (int)(Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[posIndex].vel * Project.Instance.curEngine.smartPosTable.velPer));
                        else
                            Card_Ymc3100.MoveAbs(Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_axis[i], Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[posIndex].L_point[i], (int)(Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[posIndex].vel * Project.Instance.curEngine.smartPosTable.velPer), true);
                    }
                    break;
            }

        }
        internal void LoadData(DataGridView dgv_pointList, string tableName)
        {
            Frm_MotionControl.bInit = true;
            dgv_pointList.Rows.Clear();
            dgv_pointList.Columns.Clear();
            int index = dgv_pointList.Columns.Add("", "编号");
            dgv_pointList.Columns[index].Width = 55;
            index = dgv_pointList.Columns.Add("", "名称");
            dgv_pointList.Columns[index].Width = 100;
            index = dgv_pointList.Columns.Add("", "速度");
            dgv_pointList.Columns[index].Width = 60;
            for (int i = 0; i < Project.Instance.curEngine.smartPosTable.FindTable(tableName).L_axis.Count; i++)
            {
                int temp = dgv_pointList.Columns.Add("", Project.Instance.curEngine.smartPosTable.FindTable(tableName).L_axis[i]);
                dgv_pointList.Columns[temp].Width = 60;
            }
            index = dgv_pointList.Columns.Add("", "描述");
            dgv_pointList.Columns[index].Width = 200;

            for (int i = 0; i < Project.Instance.curEngine.smartPosTable.FindTable(tableName).L_pos.Count; i++)
            {
                int idx = dgv_pointList.Rows.Add();
                dgv_pointList.Rows[idx].Cells[0].Value = Project.Instance.curEngine.smartPosTable.FindTable(tableName).L_pos[i].idx;
                dgv_pointList.Rows[idx].Cells[1].Value = Project.Instance.curEngine.smartPosTable.FindTable(tableName).L_pos[i].posName;
                dgv_pointList.Rows[idx].Cells[2].Value = Project.Instance.curEngine.smartPosTable.FindTable(tableName).L_pos[i].vel;
                for (int j = 0; j < Project.Instance.curEngine.smartPosTable.FindTable(tableName).L_pos[i].L_point.Count; j++)
                {
                    dgv_pointList.Rows[idx].Cells[3 + j].Value = Project.Instance.curEngine.smartPosTable.FindTable(tableName).L_pos[i].L_point[j];
                }
                dgv_pointList.Rows[idx].Cells[index].Value = Project.Instance.curEngine.smartPosTable.FindTable(tableName).L_pos[i].info;
            }
            Frm_MotionControl.bInit = false ;
        }
    }
    [Serializable]
    public class Table
    {
        internal Table(string tableName)
        {
            this.tableName = tableName;
        }
        internal string tableName = string.Empty;
        internal List<string> L_axis = new List<string>();
        internal List<Pos> L_pos = new List<Pos>();
    }
    [Serializable]
    public class Pos
    {
        internal Pos(int idx, string posName, int vel, List<double> point, string info)
        {
            this.idx = idx;
            this.posName = posName;
            this.vel = vel;
            this.L_point = point;
            this.info = info;
        }
        internal int idx;
        internal string posName;
        internal int vel;
        internal List<double> L_point;
        internal string info;
    }
}
