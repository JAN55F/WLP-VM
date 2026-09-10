using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    public partial class Frm_LayoutManage : Frm_FormBase
    {
        public Frm_LayoutManage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_LayoutManage _instance;
        public static Frm_LayoutManage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_LayoutManage();
                return _instance;
            }
        }


        /// <summary>
        /// 初始化语言
        /// </summary>
        private void Init_Language()
        {
            try
            {
                if (Project.Instance.configuration.language == Language.English)
                {
                    this.Text = "Layout Manage";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void btn_createNewLayout_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 1; i < 11; i++)
                {
                    if (!File.Exists(Application.StartupPath + "\\Config\\Resources\\Layout\\自定义布局" + i + ".config"))
                    {
                        Frm_Main.Instance.dockPanel.SaveAsXml(Application.StartupPath + "\\Config\\Resources\\Layout\\自定义布局" + i + ".config");
                        int index = dataGridView1.Rows.Add();
                        dataGridView1.Rows[index].Cells[0].Value = index + 1;
                        dataGridView1.Rows[index].Cells[1].Value = "自定义布局" + i;
                        cbx_layoutList.Items.Add("自定义布局" + i);
                        cbx_layoutList.SelectedIndex = cbx_layoutList.Items.Count - 1;
                        return;
                    }
                }
                Frm_MessageBox messageBox = new Frm_MessageBox();
                messageBox.MessageBoxShow("自定义布局最多只能添加10个，请删除后再添加");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_LayoutManage_Load(object sender, EventArgs e)
        {
            try
            {
                string[] files = Directory.GetFiles(Application.StartupPath + "\\Config\\Resources\\Layout");
                dataGridView1.Rows.Clear();
                cbx_layoutList.Items.Clear();
                cbx_layoutList.Items.Add("上次退出时布局");
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = 1;
                dataGridView1.Rows[index].Cells[1].Value = "上次退出时布局";
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = Path.GetFileNameWithoutExtension(files[i]);
                    index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = index + 1;
                    dataGridView1.Rows[index].Cells[1].Value = fileName;
                    cbx_layoutList.Items.Add(fileName);
                }
                string selectedLayout = Path.GetFileNameWithoutExtension(Project.Instance.configuration.layoutFilePath);
                cbx_layoutList.Text = selectedLayout;

                this.TopMost = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_deleteLayout_Click(object sender, EventArgs e)
        {
            try
            {
                string layoutName = dataGridView1.SelectedRows[0].Cells[1].Value.ToString() + ".config";
                string layoutPath = "Config\\Resources\\Layout\\" + layoutName;
                if (layoutName != "经典布局1.config" && layoutName != "经典布局2.config" && layoutName != "左中右布局.config" && layoutName != "上次退出时布局.config")
                {
                    if (File.Exists(Application.StartupPath + "\\" + layoutPath))
                        File.Delete(Application.StartupPath + "\\" + layoutPath);
                    cbx_layoutList.Items.Remove(dataGridView1.SelectedRows[0].Cells[1].Value.ToString());
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                }
                else
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n此布局文件为系统经典布局，不可删除");
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbx_layoutList.Text == "上次退出时布局")
                    Project.Instance.configuration.layoutFilePath = cbx_layoutList.Text + ".config";
                else
                    Project.Instance.configuration.layoutFilePath = "Config\\Resources\\Layout\\" + cbx_layoutList.Text + ".config";

                if (cbx_layoutList.Text == "经典布局1")
                    Frm_Main.Instance.切换到经典布局1ToolStripMenuItem.Checked = true;
                else if (cbx_layoutList.Text == "经典布局2")
                    Frm_Main.Instance.切换到经典布局2ToolStripMenuItem.Checked = true;
                else
                {
                    Frm_Main.Instance.切换到经典布局1ToolStripMenuItem.Checked = false;
                    Frm_Main.Instance.切换到经典布局2ToolStripMenuItem.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_LayoutManage_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }


        private void btn_runOnce_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 1; i < 11; i++)
                {
                    if (!File.Exists(Application.StartupPath + "\\Config\\Resources\\Layout\\自定义布局" + i + ".config"))
                    {
                        Frm_Main.Instance.dockPanel.SaveAsXml(Application.StartupPath + "\\Config\\Resources\\Layout\\自定义布局" + i + ".config");
                        int index = dataGridView1.Rows.Add();
                        dataGridView1.Rows[index].Cells[0].Value = index + 1;
                        dataGridView1.Rows[index].Cells[1].Value = "自定义布局" + i;
                        cbx_layoutList.Items.Add("自定义布局" + i);
                        cbx_layoutList.SelectedIndex = cbx_layoutList.Items.Count - 1;
                        return;
                    }
                }
                Frm_MessageBox messageBox = new Frm_MessageBox();
                messageBox.MessageBoxShow("自定义布局最多只能添加10个，请删除后再添加");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string layoutName = dataGridView1.SelectedRows[0].Cells[1].Value.ToString() + ".config";
                string layoutPath = "Config\\Resources\\Layout\\" + layoutName;
                if (layoutName != "经典布局1.config" && layoutName != "经典布局2.config" && layoutName != "左中右布局.config" && layoutName != "上次退出时布局.config")
                {
                    if (File.Exists(Application.StartupPath + "\\" + layoutPath))
                        File.Delete(Application.StartupPath + "\\" + layoutPath);
                    cbx_layoutList.Items.Remove(dataGridView1.SelectedRows[0].Cells[1].Value.ToString());
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                }
                else
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n此布局文件为系统经典布局，不可删除");
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Project.SaveProject();
            Project.Instance.configuration.Save();
            Frm_Main.Instance.SaveDockLayout(false);
            this.Close();
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

    }
}
