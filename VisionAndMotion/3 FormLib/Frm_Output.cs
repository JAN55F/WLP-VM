using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace VMPro
{
    public partial class Frm_Output : DockContent
    {
        public Frm_Output()
        {
            InitializeComponent();
            Init_Language();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_Output _instance;
        public static Frm_Output Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_Output();
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
                    this.Text = "Output";
                    listView1.Columns[0].Text = "Time";
                    listView1.Columns[1].Text = "Info";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        List<OutputItem> L_outputItem = new List<OutputItem>();
        private int numGreen = 0;
        private int numYellow = 0;
        private int numRed = 0;
        object obj = new object();
        internal void ClearLog()
        {
            numGreen = 0;
            numRed = 0;
            numYellow = 0;
            this.listView1.Items.Clear();
            L_outputItem.Clear();
            tsb_tip.Text = string.Format("提示({0})", numGreen);
            tsb_warn.Text = string.Format("警告({0})", numYellow);
            tsb_error.Text = string.Format("错误({0})", numRed);
        }
        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="color">颜色显示</param>
        public void OutputMsg(string msg, Color color)
        {
            try
            {
                lock (obj)
                {
                    if (msg == string.Empty)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = string.Empty;
                        item.SubItems.Add(msg);
                        item.ForeColor = color;
                        listView1.Items.Add(item);
                    }
                    else
                    {

                        if (color == Color.Yellow)
                            numYellow++;
                        else if (color == Color.Red)
                        {
                            numRed++;
                            //保存到报警记录集合
                            Project.Instance.curEngine.D_historyAlarm.Add(DateTime.Now, msg);
                            if (Project.Instance.curEngine.D_historyAlarm.Count > 1000)
                                Project.Instance.curEngine.D_historyAlarm.Remove(Project.Instance.curEngine.D_historyAlarm.Keys.ToArray()[Project.Instance.curEngine.D_historyAlarm.Count - 1]);
                        }
                        else
                            numGreen++;

                        string time = DateTime.Now.ToString("HH:mm:ss");

                        OutputItem outputItem = new OutputItem();
                        outputItem.msg = msg;
                        outputItem.color = color;
                        outputItem.time = time;

                        L_outputItem.Add(outputItem);
                        // listView1.Columns[1].Width = listView1.Width - listView1.Columns[0].Width - 10;
                        if (!toolStripButton1.Checked)
                        {
                            ListViewItem item = new ListViewItem();
                            item.Text = time;
                            item.SubItems.Add(msg);
                            item.ForeColor = color;
                            listView1.Items.Add(item);
                            listView1.EnsureVisible(listView1.Items.Count - 1);
                        }
                        if (L_outputItem.Count > 1000)
                        {
                            if (L_outputItem[0].color == Color.Yellow)
                                numYellow--;
                            else if (L_outputItem[0].color == Color.Red)
                                numRed--;
                            else
                                numGreen--;
                            if (!toolStripButton1.Checked)
                                listView1.Items.RemoveAt(0);
                            L_outputItem.RemoveAt(0);
                        }
                        UpdateCount();
                    }
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void UpdateCount()
        {
            tsb_tip.Text = string.Format("提示({0})", numGreen);
            tsb_warn.Text = string.Format("警告({0})", numYellow);
            tsb_error.Text = string.Format("错误({0})", numRed);
            toolStripButton1.Text = string.Format("报警({0})", Project.Instance.curEngine.D_historyAlarm.Count);
        }


        private void Frm_Output_FormClosed(object sender, FormClosedEventArgs e)
        {
            _instance = null;
        }
        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolStripButton1.Checked)
                {
                    listView1.Items.Clear();
                    Project.Instance.curEngine.D_historyAlarm.Clear();
                }
                else
                {
                    L_outputItem.Clear();
                    listView1.Items.Clear();
                    numGreen = 0;
                    numRed = 0;
                    numYellow = 0;
                }
                UpdateCount();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tsb_tip_Click(object sender, EventArgs e)
        {
            if (tsb_tip.Checked)
            {
                tsb_warn.Checked = false;
                tsb_error.Checked = false;
                toolStripButton1.Checked = false;

                listView1.Items.Clear();
                for (int i = 0; i < L_outputItem.Count; i++)
                {
                    if (L_outputItem[i].color == Color.Black )
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = L_outputItem[i].time;
                        item.SubItems.Add(L_outputItem[i].msg);
                        item.ForeColor = L_outputItem[i].color;
                        listView1.Items.Add(item);
                        listView1.EnsureVisible(listView1.Items.Count - 1);
                    }
                }
            }
            else
            {
                listView1.Items.Clear();
                for (int i = 0; i < L_outputItem.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = L_outputItem[i].time;
                    item.SubItems.Add(L_outputItem[i].msg);
                    item.ForeColor = L_outputItem[i].color;
                    listView1.Items.Add(item);
                    listView1.EnsureVisible(listView1.Items.Count - 1);

                }
            }

        }
        private void tsb_warn_Click(object sender, EventArgs e)
        {
            if (tsb_warn.Checked)
            {
                tsb_tip.Checked = false;
                tsb_error.Checked = false;
                toolStripButton1.Checked = false;

                listView1.Items.Clear();
                for (int i = 0; i < L_outputItem.Count; i++)
                {
                    if (L_outputItem[i].color == Color.Yellow)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = L_outputItem[i].time;
                        item.SubItems.Add(L_outputItem[i].msg);
                        item.ForeColor = L_outputItem[i].color;
                        listView1.Items.Add(item);
                        listView1.EnsureVisible(listView1.Items.Count - 1);
                    }
                }
            }
            else
            {
                listView1.Items.Clear();
                for (int i = 0; i < L_outputItem.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = L_outputItem[i].time;
                    item.SubItems.Add(L_outputItem[i].msg);
                    item.ForeColor = L_outputItem[i].color;
                    listView1.Items.Add(item);
                    listView1.EnsureVisible(listView1.Items.Count - 1);

                }
            }
        }
        private void tsb_error_Click(object sender, EventArgs e)
        {
            if (tsb_error.Checked)
            {
                tsb_tip.Checked = false;
                tsb_warn.Checked = false;
                toolStripButton1.Checked = false;

                listView1.Items.Clear();
                for (int i = 0; i < L_outputItem.Count; i++)
                {
                    if (L_outputItem[i].color == Color.Red)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = L_outputItem[i].time;
                        item.SubItems.Add(L_outputItem[i].msg);
                        item.ForeColor = L_outputItem[i].color;
                        listView1.Items.Add(item);
                        listView1.EnsureVisible(listView1.Items.Count - 1);
                    }
                }
            }
            else
            {
                listView1.Items.Clear();
                for (int i = 0; i < L_outputItem.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = L_outputItem[i].time;
                    item.SubItems.Add(L_outputItem[i].msg);
                    item.ForeColor = L_outputItem[i].color;
                    listView1.Items.Add(item);
                    listView1.EnsureVisible(listView1.Items.Count - 1);

                }
            }
        }
        private void Frm_Output_SizeChanged(object sender, EventArgs e)
        {
            listView1.Columns[1].Width = listView1.Width - listView1.Columns[0].Width - 24;

        }
        private void 历史日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\Config\\Log");
        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripButton1.Checked)
            {
                tsb_tip.Checked = false;
                tsb_warn.Checked = false;
                tsb_error.Checked = false;

                columnHeader1.Width = 140;
                tsb_tip.Checked = false;
                tsb_error.Checked = false;
                listView1.Items.Clear();
                foreach (KeyValuePair<DateTime, string> element in Project.Instance.curEngine.D_historyAlarm)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = element.Key.ToString("yyyy_MM_dd HH:mm:ss");
                    item.SubItems.Add(element.Value);
                    item.ForeColor = Color.Red;
                    listView1.Items.Add(item);
                    listView1.EnsureVisible(listView1.Items.Count - 1);
                }
            }
            else
            {
                columnHeader1.Width = 60;
                listView1.Items.Clear();
                for (int i = 0; i < L_outputItem.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = L_outputItem[i].time;
                    item.SubItems.Add(L_outputItem[i].msg);
                    item.ForeColor = L_outputItem[i].color;
                    listView1.Items.Add(item);
                    listView1.EnsureVisible(listView1.Items.Count - 1);

                }
            }
        }

        private void 停止刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }

    }
    public struct OutputItem
    {
        public string msg;
        public string time;
        public Color color;
    }
}
