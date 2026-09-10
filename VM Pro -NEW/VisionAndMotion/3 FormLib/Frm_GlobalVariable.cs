using HalconDotNet;
using Tool;
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
using HalconPaint;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace VMPro
{
    internal partial class Frm_GlobalVariable : Frm_FormBase
    {
        internal Frm_GlobalVariable()
        {
            InitializeComponent();
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
        }
        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();
        bool enable = false;
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_GlobalVariable _instance;
        internal static Frm_GlobalVariable Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_GlobalVariable();
                return _instance;
            }
        }

        /// <summary>
        /// 工具对象
        /// </summary>
        internal static MatchTool shapeMatchTool = new MatchTool();





        private void btn_cancel_Click(object sender, EventArgs e)
        {

        }
        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonDown;
            Application.DoEvents();
        }

        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }





        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            shapeMatchTool.ShowTemplate();
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(shapeMatchTool.toolPar.InputPar.图像);
        }







        private void button6_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawSearchRegion();
        }

        private void btn_drawTemplateRegionRectangle2_MouseUp(object sender, MouseEventArgs e)
        {

        }


        private void tbx_timeout_TextChanged(object sender, EventArgs e)
        {

        }











        private void 图像另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd");
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image path" : "请选择图像保存路径";
                dig_saveImage.Filter = Project.Instance.configuration.language == Language.English ? "Image File(*.tif)|*.tif|Image File(*.png)|*.png|Image File(*.jpg)|*.jpg|Image File(*.*)|*.*" : "图像文件(*.tif)|*.tif|图像文件(*.png)|*.png|图像文件(*.jpg)|*.jpg|图像文件(*.*)|*.*";
                dig_saveImage.InitialDirectory = path;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dig_saveImage.FileName;
                    try
                    {
                        HOperatorSet.WriteImage(shapeMatchTool.toolPar.InputPar.图像, "tiff", 0, dig_saveImage.FileName);
                    }
                    catch
                    {
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1201)" : "图像文件异常或路径不合法（错误代码：0102）", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        internal void LoadVariable(int variableType = 1)
        {
            try
            {
                enable = false;
                dataGridView1.Rows.Clear();
                if (variableType == 1)
                    ReindexCustomVariables();
                for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
                {
                    if (Project.Instance.curEngine.globelVariable.L_variable[i].variableType == variableType)
                    {
                        int idx = dataGridView1.Rows.Add();
                        dataGridView1.Rows[idx].Tag = Project.Instance.curEngine.globelVariable.L_variable[i];
                        dataGridView1.Rows[idx].Cells[0].Value = false;
                        dataGridView1.Rows[idx].Cells[1].Value = Project.Instance.curEngine.globelVariable.L_variable[i].index;
                        dataGridView1.Rows[idx].Cells[2].Value = Project.Instance.curEngine.globelVariable.L_variable[i].type;
                        dataGridView1.Rows[idx].Cells[3].Value = Project.Instance.curEngine.globelVariable.L_variable[i].name;
                        dataGridView1.Rows[idx].Cells[4].Value = Project.Instance.curEngine.globelVariable.L_variable[i].value;
                        dataGridView1.Rows[idx].Cells[5].Value = Project.Instance.curEngine.globelVariable.L_variable[i].info;
                    }
                }
                enable = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AddCustomVariable("Int");
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            AddCustomVariable("Double");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AddCustomVariable("String");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            AddCustomVariable("Bool");
        }

        private void AddCustomVariable(string type)
        {
            ReindexCustomVariables();
            Variable variable = new Variable(GetCustomVariableCount() + 1, type, Project.Instance.curEngine.GetNewName(type));
            variable.variableType = 1;
            Project.Instance.curEngine.globelVariable.L_variable.Add(variable);
            LoadVariable(1);
        }

        private int GetCustomVariableCount()
        {
            int count = 0;
            for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
            {
                if (Project.Instance.curEngine.globelVariable.L_variable[i].variableType == 1)
                    count++;
            }
            return count;
        }

        private void ReindexCustomVariables()
        {
            int index = 1;
            for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
            {
                if (Project.Instance.curEngine.globelVariable.L_variable[i].variableType == 1)
                    Project.Instance.curEngine.globelVariable.L_variable[i].index = index++;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.EndEdit();

                List<Variable> selectedVariable = new List<Variable>();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    object selected = dataGridView1.Rows[i].Cells[0].Value;
                    Variable variable = dataGridView1.Rows[i].Tag as Variable;
                    if (selected != null && Convert.ToBoolean(selected) && variable != null && variable.variableType == 1)
                        selectedVariable.Add(variable);
                }

                if (selectedVariable.Count == 0)
                {
                    Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Please select the custom variable to delete" : "\r\n请勾选要删除的自定义变量");
                    return;
                }

                for (int i = 0; i < selectedVariable.Count; i++)
                {
                    Project.Instance.curEngine.globelVariable.L_variable.Remove(selectedVariable[i]);
                }

                ReindexCustomVariables();
                LoadVariable(cbx_variableType.SelectedIndex);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                //for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
                //{
                //    if (Project.Instance.curEngine.globelVariable.L_variable[i].variableType == 1)
                //        Project.Instance.curEngine.globelVariable.L_variable.RemoveAt(i);
                //}
                //for (int i = 0; i < dataGridView1.Rows.Count; i++)
                //{
                //    Variable variable = new Variable(Convert.ToInt16(dataGridView1.Rows[i].Cells[0].Value), dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView1.Rows[i].Cells[2].Value.ToString());
                //    variable.value = dataGridView1.Rows[i].Cells[3].Value;
                //    variable.info = dataGridView1.Rows[i].Cells[4].Value.ToString();
                //    Project.Instance.curEngine.globelVariable.L_variable.Add(variable);
                //}
                this.Close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }




        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.按钮__2_;
            Application.DoEvents();
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }

     
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (enable == false)
                return;
            if (e.RowIndex < 0 || e.ColumnIndex <= 0)
                return;

            Variable variable = dataGridView1.Rows[e.RowIndex].Tag as Variable;
            if (variable == null)
                return;

            object cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            string value = cellValue == null ? string.Empty : cellValue.ToString();
            if (e.ColumnIndex == 1)
                variable.index = Convert.ToInt32(value);
            else if (e.ColumnIndex == 2)
                variable.type = value;
            else if (e.ColumnIndex == 3)
                variable.name = value;
            else if (e.ColumnIndex == 4)
                variable.value = value;
            else if (e.ColumnIndex == 5)
                variable.info = value;
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty && dataGridView1.CurrentCell is DataGridViewCheckBoxCell)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void Frm_GlobalVariable_Shown(object sender, EventArgs e)
        {
            cbx_variableType.SelectedIndex = 1;
        }

        private void cbx_variableType_SelectedIndexChanged()
        {
            LoadVariable(cbx_variableType.SelectedIndex);
            if (cbx_variableType.SelectedIndex == 0)
            {
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                button8.Visible = false;
                button9.Visible = false;
                dataGridView1.ReadOnly = true;
            }
            else
            {
                button5.Visible = true;
                button6.Visible = true;
                button7.Visible = true;
                button8.Visible = true;
                button9.Visible = true;
                dataGridView1.ReadOnly = false;
            }
        }

      


    }
}
