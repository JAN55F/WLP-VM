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

            outputFlushTimer = new System.Windows.Forms.Timer(components);
            outputFlushTimer.Interval = OutputFlushIntervalMilliseconds;
            outputFlushTimer.Tick += outputFlushTimer_Tick;
            outputFlushTimer.Enabled = Visible;
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

        internal static bool TryGetExistingInstance(out Frm_Output output)
        {
            output = _instance;
            return output != null && !output.IsDisposed && !output.Disposing;
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
        private const int MaximumOutputItems = 1000;
        private const int OutputFlushIntervalMilliseconds = 100;
        private const int MaximumItemsPerFlush = 48;

        private readonly List<OutputItem> L_outputItem = new List<OutputItem>();
        private readonly Queue<PendingOutputItem> pendingOutputItems = new Queue<PendingOutputItem>();
        private readonly object obj = new object();
        private System.Windows.Forms.Timer outputFlushTimer;
        private OutputViewMode outputViewMode = OutputViewMode.All;
        private bool countsDirty;
        private bool fullRebuildPending;
        private int numGreen = 0;
        private int numYellow = 0;
        private int numRed = 0;

        internal void ClearLog()
        {
            lock (obj)
            {
                numGreen = 0;
                numRed = 0;
                numYellow = 0;
                L_outputItem.Clear();
                pendingOutputItems.Clear();
                countsDirty = true;
                fullRebuildPending = true;
            }
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
                        EnqueueUiMutation(new PendingOutputItem
                        {
                            AddItem = true,
                            Item = new OutputItem { msg = msg, time = string.Empty, color = color }
                        });
                    }
                    else
                    {
                        DateTime messageTime = DateTime.Now;
                        if (color == Color.Yellow)
                            numYellow++;
                        else if (color == Color.Red)
                        {
                            //保存到报警记录集合
                            Dictionary<DateTime, string> historyAlarm = Project.Instance.curEngine.D_historyAlarm;
                            DateTime alarmTime = messageTime;
                            while (historyAlarm.ContainsKey(alarmTime))
                                alarmTime = alarmTime.AddTicks(1);

                            historyAlarm.Add(alarmTime, msg);
                            if (historyAlarm.Count > MaximumOutputItems)
                                historyAlarm.Remove(historyAlarm.Keys.Min());

                            //历史报警写入成功后再更新计数，避免键冲突造成计数漂移。
                            numRed++;
                        }
                        else
                            numGreen++;

                        string time = messageTime.ToString("HH:mm:ss");

                        OutputItem outputItem = new OutputItem();
                        outputItem.msg = msg;
                        outputItem.color = color;
                        outputItem.time = time;

                        L_outputItem.Add(outputItem);
                        bool addItem = IsVisibleInCurrentView(outputItem);
                        bool removeFirstVisibleItem = false;
                        if (L_outputItem.Count > MaximumOutputItems)
                        {
                            OutputItem removedItem = L_outputItem[0];
                            if (removedItem.color == Color.Yellow)
                                numYellow--;
                            else if (removedItem.color == Color.Red)
                                numRed--;
                            else
                                numGreen--;

                            removeFirstVisibleItem = IsVisibleInCurrentView(removedItem);
                            L_outputItem.RemoveAt(0);
                        }

                        countsDirty = true;
                        if (addItem || removeFirstVisibleItem)
                        {
                            EnqueueUiMutation(new PendingOutputItem
                            {
                                AddItem = addItem,
                                Item = outputItem,
                                RemoveFirstVisibleItem = removeFirstVisibleItem
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void EnqueueUiMutation(PendingOutputItem pendingItem)
        {
            if (fullRebuildPending)
                return;

            if (pendingOutputItems.Count >= MaximumOutputItems)
            {
                pendingOutputItems.Clear();
                fullRebuildPending = true;
                return;
            }

            pendingOutputItems.Enqueue(pendingItem);
        }

        private bool IsVisibleInCurrentView(OutputItem outputItem)
        {
            switch (outputViewMode)
            {
                case OutputViewMode.Information:
                    return outputItem.color == Color.Black;
                case OutputViewMode.Warning:
                    return outputItem.color == Color.Yellow;
                case OutputViewMode.Error:
                    return outputItem.color == Color.Red;
                case OutputViewMode.AlarmHistory:
                    return false;
                default:
                    return true;
            }
        }

        private void outputFlushTimer_Tick(object sender, EventArgs e)
        {
            if (IsDisposed || Disposing || !Visible)
                return;

            SafeFlushPendingOutputItems();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (outputFlushTimer == null || IsDisposed || Disposing)
                return;

            outputFlushTimer.Enabled = Visible;
            if (Visible)
                SafeFlushPendingOutputItems();
        }

        private void SafeFlushPendingOutputItems()
        {
            try
            {
                FlushPendingOutputItems();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void FlushPendingOutputItems()
        {
            bool rebuild;
            bool updateCounts;
            List<PendingOutputItem> batch = new List<PendingOutputItem>(MaximumItemsPerFlush);

            lock (obj)
            {
                rebuild = fullRebuildPending;
                if (rebuild)
                {
                    fullRebuildPending = false;
                    pendingOutputItems.Clear();
                }
                else
                {
                    while (pendingOutputItems.Count > 0 && batch.Count < MaximumItemsPerFlush)
                        batch.Add(pendingOutputItems.Dequeue());
                }

                updateCounts = countsDirty;
                countsDirty = false;
            }

            if (rebuild)
            {
                RebuildVisibleItems();
                return;
            }

            if (batch.Count > 0)
            {
                bool addedItem = false;
                listView1.BeginUpdate();
                try
                {
                    foreach (PendingOutputItem pendingItem in batch)
                    {
                        if (pendingItem.RemoveFirstVisibleItem && listView1.Items.Count > 0)
                            listView1.Items.RemoveAt(0);

                        if (pendingItem.AddItem)
                        {
                            listView1.Items.Add(CreateListViewItem(pendingItem.Item));
                            addedItem = true;
                        }
                    }
                }
                finally
                {
                    listView1.EndUpdate();
                }

                if (addedItem && listView1.Items.Count > 0)
                    listView1.EnsureVisible(listView1.Items.Count - 1);
            }

            if (updateCounts)
                UpdateCount();
        }

        private void UpdateCount()
        {
            int green;
            int yellow;
            int red;
            int historyAlarmCount;

            lock (obj)
            {
                green = numGreen;
                yellow = numYellow;
                red = numRed;
                historyAlarmCount = Project.Instance.curEngine.D_historyAlarm.Count;
            }

            tsb_tip.Text = string.Format("提示({0})", green);
            tsb_warn.Text = string.Format("警告({0})", yellow);
            tsb_error.Text = string.Format("错误({0})", red);
            toolStripButton1.Text = string.Format("报警({0})", historyAlarmCount);
        }

        private static ListViewItem CreateListViewItem(OutputItem outputItem)
        {
            ListViewItem item = new ListViewItem();
            item.Text = outputItem.time;
            item.SubItems.Add(outputItem.msg);
            item.ForeColor = outputItem.color;
            return item;
        }

        private void RebuildVisibleItems()
        {
            List<OutputItem> items = new List<OutputItem>();
            lock (obj)
            {
                pendingOutputItems.Clear();
                fullRebuildPending = false;

                if (outputViewMode == OutputViewMode.AlarmHistory)
                {
                    foreach (KeyValuePair<DateTime, string> element in Project.Instance.curEngine.D_historyAlarm)
                    {
                        items.Add(new OutputItem
                        {
                            time = element.Key.ToString("yyyy_MM_dd HH:mm:ss"),
                            msg = element.Value,
                            color = Color.Red
                        });
                    }
                }
                else
                {
                    items.AddRange(L_outputItem.Where(IsVisibleInCurrentView));
                }

                countsDirty = false;
            }

            listView1.BeginUpdate();
            try
            {
                listView1.Items.Clear();
                foreach (OutputItem item in items)
                    listView1.Items.Add(CreateListViewItem(item));
            }
            finally
            {
                listView1.EndUpdate();
            }

            if (listView1.Items.Count > 0)
                listView1.EnsureVisible(listView1.Items.Count - 1);

            UpdateCount();
        }

        private void Frm_Output_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (outputFlushTimer != null)
                outputFlushTimer.Stop();
            _instance = null;
        }
        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                lock (obj)
                {
                    if (outputViewMode == OutputViewMode.AlarmHistory)
                    {
                        Project.Instance.curEngine.D_historyAlarm.Clear();
                    }
                    else
                    {
                        L_outputItem.Clear();
                        numGreen = 0;
                        numRed = 0;
                        numYellow = 0;
                    }

                    pendingOutputItems.Clear();
                    fullRebuildPending = false;
                    countsDirty = false;
                }

                listView1.Items.Clear();
                UpdateCount();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tsb_tip_Click(object sender, EventArgs e)
        {
            SetOutputViewMode(tsb_tip.Checked ? OutputViewMode.Information : OutputViewMode.All);
        }
        private void tsb_warn_Click(object sender, EventArgs e)
        {
            SetOutputViewMode(tsb_warn.Checked ? OutputViewMode.Warning : OutputViewMode.All);
        }
        private void tsb_error_Click(object sender, EventArgs e)
        {
            SetOutputViewMode(tsb_error.Checked ? OutputViewMode.Error : OutputViewMode.All);
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
                columnHeader1.Width = 140;
                SetOutputViewMode(OutputViewMode.AlarmHistory);
            }
            else
            {
                columnHeader1.Width = 60;
                SetOutputViewMode(OutputViewMode.All);
            }
        }

        private void SetOutputViewMode(OutputViewMode viewMode)
        {
            lock (obj)
            {
                outputViewMode = viewMode;
                pendingOutputItems.Clear();
                fullRebuildPending = false;
            }

            tsb_tip.Checked = viewMode == OutputViewMode.Information;
            tsb_warn.Checked = viewMode == OutputViewMode.Warning;
            tsb_error.Checked = viewMode == OutputViewMode.Error;
            toolStripButton1.Checked = viewMode == OutputViewMode.AlarmHistory;
            columnHeader1.Width = viewMode == OutputViewMode.AlarmHistory ? 140 : 60;
            RebuildVisibleItems();
        }

        private void 停止刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }

        private enum OutputViewMode
        {
            All,
            Information,
            Warning,
            Error,
            AlarmHistory
        }

        private struct PendingOutputItem
        {
            public bool AddItem;
            public bool RemoveFirstVisibleItem;
            public OutputItem Item;
        }

    }
    public struct OutputItem
    {
        public string msg;
        public string time;
        public Color color;
    }
}
