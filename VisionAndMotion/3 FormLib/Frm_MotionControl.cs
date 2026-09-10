using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using VMPro.Properties;

namespace VMPro
{
    public partial class Frm_MotionControl : Form
    {
        public Frm_MotionControl()
        {
            InitializeComponent();
            Init_Language();
            Disposed += delegate
            {
                axisRefreshStop = true;
                axisRefreshActive = false;
            };
        }



        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_MotionControl _instance;
        public static Frm_MotionControl Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_MotionControl();
                return _instance;
            }
        }

        private sealed class AxisRefreshRequest
        {
            internal int RowIndex;
            internal string AxisName;
            internal double UnitScale;
        }

        private sealed class AxisRefreshResult
        {
            internal int RowIndex;
            internal double CommandPosition;
            internal double CommandPositionInUnit;
            internal double EncoderPosition;
            internal double EncoderPositionInUnit;
        }

        private readonly object axisRefreshThreadSync = new object();
        private Thread axisRefreshThread;
        private volatile bool axisRefreshActive;
        private volatile bool axisRefreshStop;
        private int axisUiUpdatePending;
        private DateTime lastAxisRefreshError = DateTime.MinValue;
        private readonly List<int> smartPositionTableModelIndexes = new List<int>();


        /// <summary>
        /// 初始化语言
        /// </summary>
        private void Init_Language()
        {
            try
            {
                if (Project.Instance.configuration.language == Language.English)
                {
                    this.Text = "Axis Control";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 点位运行
        /// </summary>
        /// <param name="obj"></param>
        private void Movement()
        {
            try
            {
                int selectRow = dgv_pointList.SelectedRows[0].Index;
                for (int i = 2; i < dgv_pointList.Columns.Count; i++)
                {
                    string axisName = dgv_pointList.Columns[i].HeaderText.ToString();
                    Int32 targetPos = Convert.ToInt32(dgv_pointList.Rows[selectRow].Cells[i].Value);
                    if (dgv_pointList.Rows[selectRow].Cells[i].Value.ToString() != "NA")
                    {
                        switch (Project.Instance.configuration.cardType)
                        {
                            case CardType.固高_GTS:
                                targetPos = (int)(Convert.ToDouble(dgv_pointList.Rows[selectRow].Cells[i].Value));
                                int axisIndex = Card_Googol.FindAxisByName(axisName).actNo;
                                Card_Googol.MoveAbs(axisName, targetPos, (short)(Convert.ToDouble(20) / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
                                break;
                            case CardType.雷塞_DMC2410:
                                axisIndex = Card_LeadShine_DMC2410.FindAxisByName(axisName).actNo;
                                Card_LeadShine_DMC2410.MoveAbs(axisName, (int)(targetPos * Axis_Config.Instance.MMPixelRoute[axisIndex]), (int)(Convert.ToDouble(20) / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
                                break;
                            case CardType.雷塞_DMC2210:
                                axisIndex = Card_LeadShineDMC2210.FindAxisByName(axisName).actNo;
                                Card_LeadShineDMC2210.MoveAbs(axisName, (int)(targetPos * Axis_Config.Instance.MMPixelRoute[axisIndex]), (int)(Convert.ToDouble(20) / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 显示各轴信息
        /// </summary>
        internal void SetRefreshActive(bool active)
        {
            axisRefreshActive = active;
            if (active)
                EnsureAxisRefreshThread();
        }

        internal bool IsRefreshSurfaceVisible
        {
            get
            {
                return !IsDisposed && Visible && WindowState != FormWindowState.Minimized;
            }
        }

        /// <summary>
        /// 在运动工作区实际进入时同步智能点表，避免启动阶段为隐藏页面创建窗体。
        /// </summary>
        internal void RefreshSmartPositionTables()
        {
            try
            {
                SmartPosTable smartPositionTable = Project.Instance.curEngine.smartPosTable;
                string selectedTableName = comboBox1.TextStr;

                comboBox1.Clear();
                smartPositionTableModelIndexes.Clear();
                List<string> selectableTableNames = new List<string>();
                HashSet<string> addedTableNames = new HashSet<string>(StringComparer.Ordinal);
                for (int modelIndex = 0; modelIndex < smartPositionTable.L_Table.Count; modelIndex++)
                {
                    string tableName = smartPositionTable.L_Table[modelIndex].tableName;
                    if (string.IsNullOrWhiteSpace(tableName) || !addedTableNames.Add(tableName))
                        continue;

                    comboBox1.Add(tableName);
                    selectableTableNames.Add(tableName);
                    smartPositionTableModelIndexes.Add(modelIndex);
                }

                int selectedIndex = -1;
                if (!string.IsNullOrEmpty(selectedTableName))
                {
                    for (int i = 0; i < selectableTableNames.Count; i++)
                    {
                        if (string.Equals(selectableTableNames[i], selectedTableName, StringComparison.Ordinal))
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                }

                if (selectedIndex < 0 && selectableTableNames.Count > 0)
                    selectedIndex = 0;

                if (selectedIndex >= 0)
                {
                    selectedTableName = selectableTableNames[selectedIndex];
                    // CComboBox 的程序化设置不会触发 SelectedIndexChanged，需同步索引、文本并显式加载。
                    comboBox1.SelectedIndex = selectedIndex;
                    comboBox1.TextStr = selectedTableName;
                }
                else
                {
                    selectedTableName = string.Empty;
                    comboBox1.SelectedIndex = -1;
                    comboBox1.TextStr = string.Empty;
                }

                smartPositionTable.LoadData(dgv_pointList, selectedTableName);
                comboBox2.TextStr = smartPositionTable.velPer * 100 + "%";
                SetPointTableActionsEnabled(selectedIndex >= 0);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private int GetSelectedSmartPositionTableModelIndex()
        {
            int selectorIndex = comboBox1.SelectedIndex;
            if (selectorIndex >= 0 && selectorIndex < smartPositionTableModelIndexes.Count)
            {
                int modelIndex = smartPositionTableModelIndexes[selectorIndex];
                if (modelIndex >= 0 && modelIndex < Project.Instance.curEngine.smartPosTable.L_Table.Count &&
                    string.Equals(Project.Instance.curEngine.smartPosTable.L_Table[modelIndex].tableName, comboBox1.TextStr, StringComparison.Ordinal))
                    return modelIndex;
            }

            for (int modelIndex = 0; modelIndex < Project.Instance.curEngine.smartPosTable.L_Table.Count; modelIndex++)
            {
                if (string.Equals(Project.Instance.curEngine.smartPosTable.L_Table[modelIndex].tableName, comboBox1.TextStr, StringComparison.Ordinal))
                    return modelIndex;
            }

            return -1;
        }

        private void SetPointTableActionsEnabled(bool enabled)
        {
            dgv_pointList.Enabled = enabled;
            button5.Enabled = enabled;
            button6.Enabled = enabled;
            button10.Enabled = enabled;
            button13.Enabled = enabled;
            button14.Enabled = enabled;
        }

        private void EnsureAxisRefreshThread()
        {
            lock (axisRefreshThreadSync)
            {
                if (axisRefreshThread != null && axisRefreshThread.IsAlive)
                    return;

                axisRefreshStop = false;
                axisRefreshThread = new Thread(ShowAxisInfo);
                axisRefreshThread.IsBackground = true;
                axisRefreshThread.Name = "VMPro.MotionStatus";
                axisRefreshThread.Start();
            }
        }

        private void ShowAxisInfo()
        {
            while (!axisRefreshStop && !Machine.willExit)
            {
                if (!axisRefreshActive)
                {
                    Thread.Sleep(500);
                    continue;
                }

                try
                {
                    List<AxisRefreshRequest> requests = CaptureAxisRefreshRequests();
                    if (requests.Count > 0)
                    {
                        List<AxisRefreshResult> results;
                        lock (Machine.lock_resources)
                            results = ReadAxisRefreshResults(requests);
                        QueueAxisRefreshResults(results);
                    }
                }
                catch (Exception ex)
                {
                    if ((DateTime.Now - lastAxisRefreshError).TotalSeconds >= 5)
                    {
                        lastAxisRefreshError = DateTime.Now;
                        Log.SaveError(ex);
                    }
                }

                Thread.Sleep(250);
            }
        }

        private List<AxisRefreshRequest> CaptureAxisRefreshRequests()
        {
            if (IsDisposed || !IsHandleCreated)
                return new List<AxisRefreshRequest>();

            if (InvokeRequired)
                return (List<AxisRefreshRequest>)Invoke(new Func<List<AxisRefreshRequest>>(CaptureAxisRefreshRequests));

            List<AxisRefreshRequest> requests = new List<AxisRefreshRequest>();
            CardType cardType = Project.Instance.configuration.cardType;
            if (cardType != CardType.雷塞_DMC2210 && cardType != CardType.雷塞_DMC2410)
                return requests;

            for (int rowIndex = 0; rowIndex < dgv_axisInfo.Rows.Count; rowIndex++)
            {
                object value = dgv_axisInfo.Rows[rowIndex].Cells[1].Value;
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    continue;

                string axisName = value.ToString();
                double scale = 1.0D;
                if (cardType == CardType.雷塞_DMC2210)
                {
                    ushort axisIndex = (ushort)Card_LeadShineDMC2210.FindAxisByName(axisName).actNo;
                    scale = Axis_Config.Instance.MMPixelRoute[axisIndex];
                }

                requests.Add(new AxisRefreshRequest
                {
                    RowIndex = rowIndex,
                    AxisName = axisName,
                    UnitScale = scale
                });
            }

            return requests;
        }

        private static List<AxisRefreshResult> ReadAxisRefreshResults(List<AxisRefreshRequest> requests)
        {
            List<AxisRefreshResult> results = new List<AxisRefreshResult>(requests.Count);
            CardType cardType = Project.Instance.configuration.cardType;

            foreach (AxisRefreshRequest request in requests)
            {
                double commandPosition;
                double encoderPosition;
                if (cardType == CardType.雷塞_DMC2210)
                {
                    commandPosition = Card_LeadShineDMC2210.GetCurPosition(request.AxisName);
                    encoderPosition = Card_LeadShineDMC2210.GetCurEncoder(request.AxisName);
                }
                else if (cardType == CardType.雷塞_DMC2410)
                {
                    commandPosition = Card_LeadShine_DMC2410.GetCurPosition(request.AxisName);
                    encoderPosition = Card_LeadShine_DMC2410.GetCurEncoder(request.AxisName);
                }
                else
                {
                    continue;
                }

                results.Add(new AxisRefreshResult
                {
                    RowIndex = request.RowIndex,
                    CommandPosition = commandPosition,
                    CommandPositionInUnit = commandPosition * request.UnitScale,
                    EncoderPosition = encoderPosition,
                    EncoderPositionInUnit = encoderPosition * request.UnitScale
                });
            }

            return results;
        }

        private void QueueAxisRefreshResults(List<AxisRefreshResult> results)
        {
            if (results == null || results.Count == 0 || !axisRefreshActive || IsDisposed || !IsHandleCreated)
                return;
            if (Interlocked.CompareExchange(ref axisUiUpdatePending, 1, 0) != 0)
                return;

            try
            {
                BeginInvoke(new Action<List<AxisRefreshResult>>(ApplyAxisRefreshResults), results);
            }
            catch
            {
                Interlocked.Exchange(ref axisUiUpdatePending, 0);
            }
        }

        private void ApplyAxisRefreshResults(List<AxisRefreshResult> results)
        {
            try
            {
                if (!axisRefreshActive || IsDisposed)
                    return;

                foreach (AxisRefreshResult result in results)
                {
                    if (result.RowIndex < 0 || result.RowIndex >= dgv_axisInfo.Rows.Count)
                        continue;

                    DataGridViewRow row = dgv_axisInfo.Rows[result.RowIndex];
                    row.Cells[2].Value = result.CommandPosition;
                    row.Cells[3].Value = result.CommandPositionInUnit;
                    row.Cells[4].Value = result.EncoderPosition;
                    row.Cells[5].Value = result.EncoderPositionInUnit;
                }
            }
            finally
            {
                Interlocked.Exchange(ref axisUiUpdatePending, 0);
            }
        }
        internal void OutputMsg(string msg, Color color)
        {
            this.lbl_tip.ForeColor = color;
            this.lbl_tip.Text = "提示：" + msg;
        }



        private void btn_touch_Click(object sender, EventArgs e)
        {

        }
        private void btn_appear_Click(object sender, EventArgs e)
        {

        }
        private void btn_save_Click(object sender, EventArgs e)
        {

        }
        private void btn_delete_Click(object sender, EventArgs e)
        {

        }

        private void Frm_Axis_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetRefreshActive(false);
            this.Hide();
            e.Cancel = true;
        }
        private void Frm_AxisControl_Load(object sender, EventArgs e)
        {
            dgv_pointList.Rows[0].Cells[0].Value = 1;
            this.TopMost = true;
        }
        private void btn_axisSetting_Click(object sender, EventArgs e)
        {
            Frm_AxisSetting.Instance.ShowDialog();
        }
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (this.Visible)
            {
                dgv_pointList.Rows[dgv_pointList.Rows.Count - 1].Cells[0].Value = dgv_pointList.Rows.Count;
            }
        }
        private void btn_stopMove_Click(object sender, EventArgs e)
        {
            //////try
            //////{
            //////    switch (Project.Instance.configuration.cardType)
            //////    {
            //////        case CardType.固高_GTS:
            //////            break;

            //////        case CardType.雷塞_DMC2210:
            //////            ushort axisIndex = (ushort)Card_LeadShineDMC2210.FindAxisByName(cbx_axisName.Text).actNo;
            //////            Card_LeadShineDMC2210.DecStop(axisIndex);
            //////            break;
            //////        case CardType.雷塞_DMC2410:
            //////            axisIndex = (ushort)Card_LeadShine_DMC2410.FindAxisByName(cbx_axisName.Text).actNo;
            //////            Card_LeadShine_DMC2410.DecStop(axisIndex);
            //////            break;
            //////        case CardType.凌华_AMP204C:
            //////            axisIndex = (ushort)Card_ADLink.FindAxisByName(cbx_axisName.Text).actNo;
            //////            Card_ADLink.DecStop(axisIndex);
            //////            break;
            //////    }
            //////}
            //////catch (Exception ex)
            //////{
            //////    Log.SaveError(ex);
            //////}
        }
        private void lnk_motorOnOrOff_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                switch (Project.Instance.configuration.cardType)
                {
                    case CardType.固高_GTS: if (!Card_Googol.initSucceed)
                        {
                            Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
                            return;
                        }
                        //////if (pic_motorOnOrOff.Image == Resources.On)
                        //////{
                        //////    Card_Googol.MotorOff(cbx_axisName.Text);
                        //////    pic_motorOnOrOff.Image = Resources.Off;
                        //////}
                        //////else
                        //////{
                        //////    Card_Googol.MotorOn(cbx_axisName.Text);
                        //////    pic_motorOnOrOff.Image = Resources.On;
                        //////}
                        break;

                    case CardType.雷塞_DMC2210:
                        if (!Card_LeadShineDMC2210.initSucceed)
                        {
                            Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
                            return;
                        }
                        //////if (pic_motorOnOrOff.Tag.ToString() == "On")
                        //////{
                        //////    pic_motorOnOrOff.Tag = "Off";
                        //////    Card_LeadShineDMC2210.MotorOff(cbx_axisName.Text);
                        //////    pic_motorOnOrOff.Image = Resources.Off;
                        //////}
                        //////else
                        //////{
                        //////    pic_motorOnOrOff.Tag = "On";
                        //////    Card_LeadShineDMC2210.MotorOn(cbx_axisName.Text);
                        //////    pic_motorOnOrOff.Image = Resources.On;
                        //////}
                        break;
                    case CardType.雷塞_DMC2410:
                        if (!Card_LeadShine_DMC2410.initSucceed)
                        {
                            Frm_MessageBox messageBox = new Frm_MessageBox();
                            messageBox.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
                            return;
                        }
                        //////if (pic_motorOnOrOff.Image == Resources.On)
                        //////{
                        //////    Card_LeadShine_DMC2410.MotorOff(cbx_axisName.Text);
                        //////    pic_motorOnOrOff.Image = Resources.Off;
                        //////}
                        //////else
                        //////{
                        //////    Card_LeadShine_DMC2410.MotorOn(cbx_axisName.Text);
                        //////    pic_motorOnOrOff.Image = Resources.On;
                        //////}
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
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

        private void button4_Click(object sender, EventArgs e)
        {

        }
        internal XYU RotateAt(XYU curPos, XYU rotateCenter, double rotateAngle)
        {
            try
            {
                double rad = rotateAngle * Math.PI / 180;
                var res = new XYU();
                res.Point.X = rotateCenter.Point.X + (curPos.Point.X - rotateCenter.Point.X) * Math.Cos(rad) - (curPos.Point.Y - rotateCenter.Point.Y) * Math.Sin(rad);
                res.Point.Y = rotateCenter.Point.Y + (curPos.Point.X - rotateCenter.Point.X) * Math.Sin(rad) + (curPos.Point.Y - rotateCenter.Point.Y) * Math.Cos(rad);
                res.U = curPos.U + rotateAngle;

                if (res.U < -180)
                {
                    res.U += 360;
                }
                else if (res.U >= 180)
                {
                    res.U -= 360;
                }
                return res;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new XYU();
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {

        }
        #region 窗体拖动
        private static bool IsDrag = false;
        private int enterX;
        private int enterY;
        private void setForm_MouseDown(object sender, MouseEventArgs e)
        {
            IsDrag = true;
            enterX = e.Location.X;
            enterY = e.Location.Y;
        }
        private void setForm_MouseUp(object sender, MouseEventArgs e)
        {
            IsDrag = false;
            enterX = 0;
            enterY = 0;
        }
        private void setForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrag)
            {
                Left += e.Location.X - enterX;
                Top += e.Location.Y - enterY;
            }
        }
        #endregion

        private void cbx_axisName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        ////#region  窗体缩放
        ////private const int WM_NCHITTEST = 0x0084; //鼠标在窗体客户区（除标题栏和边框以外的部分）时发送的信息
        ////const int HTLEFT = 10;  //左变
        ////const int HTRIGHT = 11;  //右边
        ////const int HTTOP = 12;
        ////const int HTTOPLEFT = 13;  //左上
        ////const int HTTOPRIGHT = 14; //右上
        ////const int HTBOTTOM = 15;  //下
        ////const int HTBOTTOMLEFT = 0x10;  //左下
        ////const int HTBOTTOMRIGHT = 17;  //右下
        ////System.Drawing.Point vPoint = System.Drawing.Point.Empty;
        //////自定义边框拉伸
        ////protected override void WndProc(ref Message m)
        ////{
        ////    try
        ////    {
        ////        base.WndProc(ref m);
        ////        switch (m.Msg)
        ////        {
        ////            case WM_NCHITTEST:
        ////                vPoint = new System.Drawing.Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
        ////                vPoint = PointToClient(vPoint);
        ////                if (vPoint.X <= 5)
        ////                    if (vPoint.Y <= 5)
        ////                        m.Result = (IntPtr)HTTOPLEFT;  //左上
        ////                    else if (vPoint.Y >= this.ClientSize.Height - 5)
        ////                        m.Result = (IntPtr)HTBOTTOMLEFT; //左下
        ////                    else
        ////                        m.Result = (IntPtr)HTLEFT;  //左边
        ////                else if (vPoint.X >= this.ClientSize.Width - 5)
        ////                    if (vPoint.Y <= 5)
        ////                        m.Result = (IntPtr)HTTOPRIGHT;  //右上
        ////                    else if (vPoint.Y >= this.ClientSize.Height - 5)
        ////                        m.Result = (IntPtr)HTBOTTOMRIGHT;  //右下
        ////                    else
        ////                        m.Result = (IntPtr)HTRIGHT;  //右
        ////                else if (vPoint.Y <= 5)
        ////                    m.Result = (IntPtr)HTTOP;  //上
        ////                else if (vPoint.Y >= this.ClientSize.Height - 5)
        ////                    m.Result = (IntPtr)HTBOTTOM; //下

        ////                else
        ////                {
        ////                    base.WndProc(ref m);//如果去掉这一行代码,窗体将失去MouseMove..等事件
        ////                    System.Drawing.Point lpint = new System.Drawing.Point((int)m.LParam);//可以得到鼠标坐标,这样就可以决定怎么处理这个消息了,是移动窗体,还是缩放,以及向哪向的缩放

        ////                    m.Result = (IntPtr)0x2;//托动HTCAPTION=2 <0x2>
        ////                }
        ////                break;
        ////        }
        ////    }
        ////    catch { }
        ////}
        ////#endregion

        //private void Btn_MouseDown(object sender, MouseEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    button.BackgroundImage = Resources.ButtonDown;
        //    Application.DoEvents();
        //}

        //private void Btn_MouseUp(object sender, MouseEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    button.BackgroundImage = Resources.ButtonUp;
        //    Application.DoEvents();
        //}
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

        private void dgv_axisInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //////try
            //////{
            //////    ushort axisIndex = Card_ADLink.FindAxisByName(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value).actNo;
            //////    switch (e.ColumnIndex)
            //////    {
            //////        case 9:
            //////            if (dgv_axisInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "开")
            //////            {
            //////                dgv_axisInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "关";
            //////                ((DataGridViewButtonCell)(dgv_axisInfo.Rows[e.RowIndex].Cells[e.ColumnIndex])).Style.BackColor = Color.Green;
            //////                switch (Project.Instance.configuration.cardType)
            //////                {
            //////                    case CardType.凌华_AMP204C:
            //////                        Card_ADLink.MotorOn(axisIndex);
            //////                        break;
            //////                    case CardType.安川_MP3100:
            //////                        Card_Ymc3100.MotorOn(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value);
            //////                        break;
            //////                }
            //////            }
            //////            else
            //////            {
            //////                dgv_axisInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "开";
            //////                ((DataGridViewButtonCell)(dgv_axisInfo.Rows[e.RowIndex].Cells[e.ColumnIndex])).Style.BackColor = Color.Gray;

            //////                switch (Project.Instance.configuration.cardType)
            //////                {
            //////                    case CardType.凌华_AMP204C:
            //////                        Card_ADLink.MotorOn(axisIndex);
            //////                        break;
            //////                    case CardType.安川_MP3100:
            //////                        Card_Ymc3100.MotorOff(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value);
            //////                        break;
            //////                }
            //////            }
            //////            break;
            //////        case 10:



            //////            if (Project.Instance.configuration.cardType == CardType.无)
            //////            {
            //////                OutputMsg("当前项目被认定为无板卡项目，若确实存在板卡，请在[系统]菜单下的[设置]页面中选择对应板卡型号", Color.Red);
            //////            }
            //////            switch (Project.Instance.configuration.cardType)
            //////            {
            //////                case CardType.固高_GTS:
            //////                    if (!Card_Googol.initSucceed)
            //////                    {
            //////                        Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可回零");
            //////                        return;
            //////                    }
            //////                    if (cbx_axisName.Text == string.Empty)
            //////                    {
            //////                        OutputMsg(Project.Instance.configuration.language == Language.English ? "Please select axis" : "请先选择轴", Color.Red);
            //////                        return;
            //////                    }
            //////                    if (Card_Googol.homing)
            //////                    {
            //////                        OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////                        return;
            //////                    }
            //////                    OutputMsg("提示：" + cbx_axisName.Text + "轴正在回零中......", Color.Black);
            //////                    string axisName = cbx_axisName.Text;
            //////                    Card_Googol.Home(axisName,
            //////                                     Axis_Config.Instance.回零搜索长度[cbx_axisName.SelectedIndex],
            //////                                     100,
            //////                                     tkb_speed.Value,
            //////                                     Axis_Config.Instance.回零方向[cbx_axisName.SelectedIndex],
            //////                                     Axis_Config.Instance.回退长度[cbx_axisName.SelectedIndex],
            //////                                     1);
            //////                    OutputMsg(Project.Instance.configuration.language == Language.English ? "Tip :Home succeed" : "轴回零成功", Color.Black);
            //////                    break;
            //////                case CardType.雷塞_DMC2210:
            //////                    if (!Card_LeadShineDMC2210.initSucceed)
            //////                    {
            //////                        Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可回零");
            //////                        return;
            //////                    }
            //////                    if (Card_LeadShineDMC2210.homing)
            //////                    {
            //////                        OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////                        return;
            //////                    }
            //////                    if (cbx_axisName.Text == string.Empty)
            //////                    {
            //////                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Please select axis" : "请先选择轴", Color.Red);
            //////                        return;
            //////                    }
            //////                    OutputMsg("提示：" + cbx_axisName.Text + "轴正在回零中......", Color.Black);
            //////                    Card_LeadShineDMC2210.Home(axisIndex,
            //////                                    Axis_Config.Instance.回零速度[axisIndex] / Axis_Config.Instance.MMPixelRoute[axisIndex],
            //////                                    Axis_Config.Instance.回零方向[axisIndex],
            //////                                  (int)(Axis_Config.Instance.回退长度[axisIndex] / Axis_Config.Instance.MMPixelRoute[axisIndex])
            //////                                   );
            //////                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Tip :Home succeed" : "轴回零成功", Color.Black);
            //////                    break;
            //////                case CardType.雷塞_DMC2410:
            //////                    if (!Card_LeadShine_DMC2410.initSucceed)
            //////                    {
            //////                        Frm_MessageBox messageBox = new Frm_MessageBox();
            //////                        messageBox.MessageBoxShow("\r\n未识别到相应运动控制卡，不可回零");
            //////                        return;
            //////                    }
            //////                    if (Card_LeadShine_DMC2410.homing)
            //////                    {
            //////                        OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////                        return;
            //////                    }
            //////                    if (cbx_axisName.Text == string.Empty)
            //////                    {
            //////                        OutputMsg(Project.Instance.configuration.language == Language.English ? "Please select axis" : "请先选择轴", Color.Red);
            //////                        return;
            //////                    }
            //////                    OutputMsg("提示：" + cbx_axisName.Text + "轴正在回零中......", Color.Black);
            //////                    Card_LeadShine_DMC2410.Home(axisIndex,
            //////                                  Axis_Config.Instance.回零速度[axisIndex] / Axis_Config.Instance.MMPixelRoute[axisIndex],
            //////                                    Axis_Config.Instance.回零方向[axisIndex],
            //////                                  (int)(Axis_Config.Instance.回退长度[axisIndex] / Axis_Config.Instance.MMPixelRoute[axisIndex]),
            //////                                    1);
            //////                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Tip :Home succeed" : "轴回零成功", Color.Black);
            //////                    break;
            //////                case CardType.凌华_AMP204C:
            //////                    if (!Card_ADLink.initSucceed)
            //////                    {
            //////                        Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可回零");
            //////                        return;
            //////                    }
            //////                    if (Card_ADLink.homing)
            //////                    {
            //////                        OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////                        return;
            //////                    }
            //////                    if (cbx_axisName.Text == string.Empty)
            //////                    {
            //////                        OutputMsg(Project.Instance.configuration.language == Language.English ? "Please select axis" : "请先选择轴", Color.Red);
            //////                        return;
            //////                    }
            //////                    OutputMsg("提示：" + cbx_axisName.Text + "轴正在回零中......", Color.Black);
            //////                    Card_ADLink.Home(axisIndex,
            //////                                    Axis_Config.Instance.回零速度[axisIndex] / Axis_Config.Instance.MMPixelRoute[axisIndex],
            //////                                    Axis_Config.Instance.回零方向[axisIndex],
            //////                                  (int)(Axis_Config.Instance.回退长度[axisIndex] / Axis_Config.Instance.MMPixelRoute[axisIndex])
            //////                                   );
            //////                    OutputMsg(Project.Instance.configuration.language == Language.English ? "Tip :Home succeed" : "轴回零成功", Color.Black);
            //////                    break;
            //////            }
            //////            break;
            //////        //////case 11:
            //////        //////    if (Project.Instance.configuration.cardType == CardType.无)
            //////        //////    {
            //////        //////        OutputMsg("当前项目被认定为无板卡项目，若确实存在板卡，请在[系统]菜单下的[设置]页面中选择对应板卡型号", Color.Red);
            //////        //////    }
            //////        //////    switch (Project.Instance.configuration.cardType)
            //////        //////    {
            //////        //////        case CardType.固高_GTS:
            //////        //////            axisIndex = Card_Googol.GetAxisIndexByName(cbx_axisName);
            //////        //////            if (!Card_Googol.initSucceed)
            //////        //////            {
            //////        //////                Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
            //////        //////                return;
            //////        //////            }
            //////        //////            if (Card_Googol.homing)
            //////        //////            {
            //////        //////                OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (cbx_axisName.Text == string.Empty)
            //////        //////            {
            //////        //////                Frm_Main.Instance.OutputMsg("Tip:Please select axis", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            double curPos = Card_Googol.GetCurPosition(cbx_axisName.Text);
            //////        //////            double offset = Convert.ToDouble(cbo_moveDistance.Text) * 1000;
            //////        //////            double targetPos = curPos - offset;
            //////        //////            Card_Googol.MoveAbs(cbx_axisName.Text, Convert.ToInt32(targetPos), (short)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]));
            //////        //////            break;

            //////        //////        case CardType.雷塞_DMC2210:
            //////        //////            axisIndex = Card_LeadShineDMC2210.GetAxisIndexByName(cbx_axisName);
            //////        //////            if (!Card_LeadShineDMC2210.initSucceed)
            //////        //////            {
            //////        //////                Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
            //////        //////                return;
            //////        //////            }
            //////        //////            if (Card_LeadShineDMC2210.homing)
            //////        //////            {
            //////        //////                OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (cbx_axisName.Text == string.Empty)
            //////        //////            {
            //////        //////                Frm_Main.Instance.OutputMsg("Tip:Please select axis", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (rdo_jog.Checked)         //点动
            //////        //////            {
            //////        //////                double distance = Convert.ToDouble(cbo_moveDistance.Text.Trim());
            //////        //////                Card_LeadShineDMC2210.MoveRel(cbx_axisName.Text, -(int)distance, (int)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            else
            //////        //////            {
            //////        //////                Card_LeadShineDMC2210.KeepMove(cbx_axisName.Text, 0, -(int)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            break;
            //////        //////        case CardType.雷塞_DMC2410:
            //////        //////            axisIndex = Card_LeadShine_DMC2410.GetAxisIndexByName(cbx_axisName);
            //////        //////            if (!Card_LeadShine_DMC2410.initSucceed)
            //////        //////            {
            //////        //////                Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
            //////        //////                return;
            //////        //////            }
            //////        //////            if (Card_LeadShine_DMC2410.homing)
            //////        //////            {
            //////        //////                Frm_Main.Instance.OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (cbx_axisName.Text == string.Empty)
            //////        //////            {
            //////        //////                OutputMsg("Tip:Please select axis", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (rdo_jog.Checked)         //点动
            //////        //////            {
            //////        //////                double distance = Convert.ToDouble(cbo_moveDistance.Text.Trim());
            //////        //////                Card_LeadShine_DMC2410.MoveRel(cbx_axisName.Text, -(int)(distance / Axis_Config.Instance.MMPixelRoute[axisIndex]), (int)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            else
            //////        //////            {
            //////        //////                Card_LeadShineDMC2210.KeepMove(cbx_axisName.Text, 0, -(int)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            break;
            //////        //////        case CardType.凌华_AMP204C:
            //////        //////            axisIndex = Card_ADLink.GetAxisIndexByName(cbx_axisName);
            //////        //////            if (!Card_ADLink.initSucceed)
            //////        //////            {
            //////        //////                Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
            //////        //////                return;
            //////        //////            }
            //////        //////            if (Card_ADLink.homing)
            //////        //////            {
            //////        //////                OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (cbx_axisName.Text == string.Empty)
            //////        //////            {
            //////        //////                OutputMsg("Tip:Please select axis", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (rdo_jog.Checked)         //点动
            //////        //////            {
            //////        //////                double distance = Convert.ToDouble(cbo_moveDistance.Text.Trim());
            //////        //////                Card_ADLink.MoveRel(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value, -(int)distance, (int)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            else
            //////        //////            {
            //////        //////                Card_ADLink.KeepMove(cbx_axisName.Text, 0, -(int)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            break;
            //////        //////    }
            //////        //////    break;
            //////        //////case 12:
            //////        //////    if (Project.Instance.configuration.cardType == CardType.无)
            //////        //////    {
            //////        //////        OutputMsg("当前项目被认定为无板卡项目，若确实存在板卡，请在[系统]菜单下的[设置]页面中选择对应板卡型号", Color.Red);
            //////        //////    }
            //////        //////    switch (Project.Instance.configuration.cardType)
            //////        //////    {
            //////        //////        case CardType.固高_GTS:
            //////        //////            if (!Card_Googol.initSucceed)
            //////        //////            {
            //////        //////                Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
            //////        //////                return;
            //////        //////            }
            //////        //////            if (Card_Googol.homing)
            //////        //////            {
            //////        //////                OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (cbx_axisName.Text == string.Empty)
            //////        //////            {
            //////        //////                OutputMsg("Tip:Please select axis", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (rdo_jog.Checked)         //点动
            //////        //////            {
            //////        //////                double curPos = Card_Googol.GetCurPosition(cbx_axisName.Text);
            //////        //////                double offset = Convert.ToDouble(cbo_moveDistance.Text) * 1000;
            //////        //////                double targetPos = curPos + offset;
            //////        //////                Card_Googol.MoveAbs(cbx_axisName.Text, Convert.ToInt32(targetPos), (short)tkb_speed.Value);
            //////        //////            }
            //////        //////            else        //连续
            //////        //////            {

            //////        //////            }
            //////        //////            break;

            //////        //////        case CardType.雷塞_DMC2210:
            //////        //////            axisIndex = Card_LeadShineDMC2210.GetAxisIndexByName(cbx_axisName);

            //////        //////            if (!Card_LeadShineDMC2210.initSucceed)
            //////        //////            {
            //////        //////                Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
            //////        //////                return;
            //////        //////            }
            //////        //////            if (Card_LeadShineDMC2210.homing)
            //////        //////            {
            //////        //////                OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (cbx_axisName.Text == string.Empty)
            //////        //////            {
            //////        //////                Frm_Main.Instance.OutputMsg("Tip:Please select axis", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (rdo_jog.Checked)         //点动
            //////        //////            {
            //////        //////                double distance = Convert.ToDouble(cbo_moveDistance.Text.Trim());
            //////        //////                Card_LeadShineDMC2210.MoveRel(cbx_axisName.Text, (int)distance, (int)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            else        //连续
            //////        //////            {
            //////        //////                Card_LeadShineDMC2210.KeepMove(cbx_axisName.Text, 1, (short)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            break;
            //////        //////        case CardType.雷塞_DMC2410:
            //////        //////            axisIndex = Card_LeadShine_DMC2410.GetAxisIndexByName(cbx_axisName);
            //////        //////            if (!Card_LeadShine_DMC2410.initSucceed)
            //////        //////            {
            //////        //////                Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
            //////        //////                return;
            //////        //////            }
            //////        //////            if (Card_LeadShine_DMC2410.homing)
            //////        //////            {
            //////        //////                OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (cbx_axisName.Text == string.Empty)
            //////        //////            {
            //////        //////                Frm_Main.Instance.OutputMsg("Tip:Please select axis", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (rdo_jog.Checked)         //点动
            //////        //////            {
            //////        //////                double distance = Convert.ToDouble(cbo_moveDistance.Text.Trim());
            //////        //////                Card_LeadShine_DMC2410.MoveRel(cbx_axisName.Text, (int)(distance / Axis_Config.Instance.MMPixelRoute[axisIndex]), (short)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            else        //连续
            //////        //////            {
            //////        //////                Card_LeadShineDMC2210.KeepMove(cbx_axisName.Text, 1, (short)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            break;
            //////        //////        case CardType.凌华_AMP204C:
            //////        //////            axisIndex = Card_ADLink.GetAxisIndexByName(cbx_axisName);

            //////        //////            if (!Card_ADLink.initSucceed)
            //////        //////            {
            //////        //////                Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可操作");
            //////        //////                return;
            //////        //////            }
            //////        //////            if (Card_ADLink.homing)
            //////        //////            {
            //////        //////                OutputMsg("当前轴正在回零，请回零完成后操作", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (cbx_axisName.Text == string.Empty)
            //////        //////            {
            //////        //////                Frm_Main.Instance.OutputMsg("Tip:Please select axis", Color.Red);
            //////        //////                return;
            //////        //////            }
            //////        //////            if (rdo_jog.Checked)         //点动
            //////        //////            {
            //////        //////                double distance = Convert.ToDouble(cbo_moveDistance.Text.Trim());
            //////        //////                Card_ADLink.MoveRel(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value, (int)distance, (int)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            else        //连续
            //////        //////            {
            //////        //////                Card_ADLink.KeepMove(cbx_axisName.Text, 1, (short)(tkb_speed.Value / Axis_Config.Instance.MMPixelRoute[axisIndex]), false);
            //////        //////            }
            //////        //////            break;
            //////        //////}
            //////        //////break;
            //////    }
            //////}
            //////catch (Exception ex)
            //////{
            //////    Log.SaveError(ex);
            //////}
        }

        private void dgv_doList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (!Permission.CheckPermission(PermissionLevel.Admin))
                    return;
                if (dgv_doList.SelectedRows[0].Index != -1)
                {
                    if (Project.Instance.configuration.cardType == CardType.固高_GTS)
                    {
                        string doName = dgv_doList.SelectedRows[0].Cells[4].Value.ToString();
                        if (Card_Googol.GetDoSts(doName) == Level.Low)
                            Card_Googol.SetDo(doName, Level.High);
                        else
                            Card_Googol.SetDo(doName, Level.Low);
                    }
                    else if (Project.Instance.configuration.cardType == CardType.雷赛_IOC0640)
                    {
                        string doName = dgv_doList.SelectedRows[0].Cells[4].Value.ToString();
                        if (Card_Googol.GetDoSts(doName) == Level.Low)
                            Card_Googol.SetDo(doName, Level.High);
                        else
                            Card_Googol.SetDo(doName, Level.Low);
                    }
                    else if (Project.Instance.configuration.cardType == CardType.雷塞_DMC2210)
                    {
                        string doName = dgv_doList.SelectedRows[0].Cells[4].Value.ToString();
                        if (Card_LeadShineDMC2210.GetDoSts(doName) == Level.Low)
                            Card_LeadShineDMC2210.SetDo(doName, Level.High);
                        else
                            Card_LeadShineDMC2210.SetDo(doName, Level.Low);
                    }
                    else if (Project.Instance.configuration.cardType == CardType.雷塞_DMC2410)
                    {
                        string doName = dgv_doList.SelectedRows[0].Cells[4].Value.ToString();
                        if (Card_LeadShine_DMC2410.GetDoSts(doName) == Level.Low)
                            Card_LeadShine_DMC2410.SetDo(doName, Level.High);
                        else
                            Card_LeadShine_DMC2410.SetDo(doName, Level.Low);
                    }
                    else if (Project.Instance.configuration.cardType == CardType.安川_MP3100)
                    {
                        string doName = dgv_doList.SelectedRows[0].Cells[4].Value.ToString();
                        if (Card_Ymc3100.GetDo((Do)Enum.Parse(typeof(Do), doName)) == Level.Low)
                            Card_Ymc3100.SetDo((Do)Enum.Parse(typeof(Do), doName), Level.High);
                        else
                            Card_Ymc3100.SetDo((Do)Enum.Parse(typeof(Do), doName), Level.Low);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void dgv_axisInfo_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {



            ushort axisIndex = Card_ADLink.FindAxisByName(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value).actNo;
            switch (e.ColumnIndex)
            {
                case 11:
                    switch (Project.Instance.configuration.cardType)
                    {
                        case CardType.凌华_AMP204C:
                            Card_ADLink.MotorOn(axisIndex);
                            break;
                        case CardType.安川_MP3100:
                            Card_Ymc3100.JogStart(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value, HomeDir.N_负方向, 10, true);
                            break;
                    }
                    break;
                case 12:
                    switch (Project.Instance.configuration.cardType)
                    {
                        case CardType.凌华_AMP204C:
                            Card_ADLink.MotorOn(axisIndex);
                            break;
                        case CardType.安川_MP3100:
                            Card_Ymc3100.JogStart(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value, HomeDir.P_正方向, 10, true);
                            break;
                    }
                    break;
            }



        }

        private void dgv_axisInfo_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {

            ushort axisIndex = Card_ADLink.FindAxisByName(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value).actNo;
            switch (e.ColumnIndex)
            {
                case 11:
                case 12:
                    switch (Project.Instance.configuration.cardType)
                    {
                        case CardType.凌华_AMP204C:
                            Card_ADLink.MotorOn(axisIndex);
                            break;
                        case CardType.安川_MP3100:
                            Card_Ymc3100.JogStop(dgv_axisInfo.Rows[e.RowIndex].Cells[1].Value);
                            break;
                    }
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click_1(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void btn_drawTemplateRegionRectangle1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            btn_drawTemplateRegionRectangle1.BackColor = Color.FromArgb(18, 150, 219);
            button12.BackColor = Color.LightGray;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            button12.BackColor = Color.FromArgb(18, 150, 219);
            btn_drawTemplateRegionRectangle1.BackColor = Color.LightGray;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Project.Instance.curEngine.smartPosTable.LoadData(dgv_pointList, comboBox1.TextStr);

        }

        private void button9_Click_1(object sender, EventArgs e)
        {

        }

        private void button9_Click_2(object sender, EventArgs e)
        {

        }

        private void button10_Click_2(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_pointList.SelectedRows.Count == 0)
                {
                    OutputMsg("请先选中点表中的具体行后删除", Color.Red);
                    return;
                }

                int selectRow = dgv_pointList.SelectedRows[0].Index;

                bool isEmpty = false;
                for (int j = 3; j < dgv_pointList.Columns.Count; j++)
                {
                    if (dgv_pointList.Rows[selectRow].Cells[j].Value == null || dgv_pointList.Rows[selectRow].Cells[j].Value.ToString() == string.Empty || dgv_pointList.Rows[selectRow].Cells[j].Value.ToString() == "0")
                    {
                        isEmpty = true;
                        break;
                    }
                }
                if (!isEmpty)
                {
                    Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : "\r\n此点位位置信息已存在，确定要覆盖吗？");
                    Frm_ConfirmBox.Instance.ShowDialog();
                    if (Frm_ConfirmBox.Instance.Result != ConfirmBoxResult.Yes)
                    {
                        return;
                    }
                }

                switch (Project.Instance.configuration.cardType)
                {
                    case CardType.固高_GTS:
                        if (!Card_Googol.initSucceed && !Project.Instance.configuration.vitualCard)
                        {
                            Frm_MessageBox messageBox = new Frm_MessageBox();
                            messageBox.MessageBoxShow("\r\n未识别到相应运动控制卡，不可示教");
                            return;
                        }
                        break;

                    case CardType.雷塞_DMC2210:
                        if (!Card_LeadShineDMC2210.initSucceed && !Project.Instance.configuration.vitualCard)
                        {
                            Frm_MessageBox messageBox = new Frm_MessageBox();
                            messageBox.MessageBoxShow("\r\n未识别到相应运动控制卡，不可示教");
                            return;
                        }
                        Frm_ConfirmBox.Instance.lbl_info.Text = Project.Instance.configuration.language == Language.English ? "Are you sure you want to overwrite the point data?" : "确定要覆盖此点？";
                        Frm_ConfirmBox.Instance.ShowDialog();
                        if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Cancel)
                        {
                            return;
                        }

                        for (int i = 2; i < dgv_pointList.Columns.Count; i++)
                        {
                            double curPos = Card_LeadShineDMC2210.GetCurPosition(dgv_pointList.Columns[i].HeaderText);
                            dgv_pointList.Rows[selectRow].Cells[i].Value = curPos;
                        }
                        break;
                    case CardType.雷塞_DMC2410:
                        if (!Card_LeadShine_DMC2410.initSucceed && !Project.Instance.configuration.vitualCard)
                        {
                            Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到相应运动控制卡，不可示教");
                            return;
                        }
                        Frm_ConfirmBox.Instance.lbl_info.Text = Project.Instance.configuration.language == Language.English ? "Are you sure you want to overwrite the point data?" : "确定要覆盖此点？";
                        Frm_ConfirmBox.Instance.ShowDialog();
                        if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Cancel)
                        {
                            return;
                        }
                        selectRow = dgv_pointList.SelectedRows[0].Index;
                        for (int i = 2; i < dgv_pointList.Columns.Count; i++)
                        {
                            double curPos = Card_LeadShine_DMC2410.GetCurPosition(dgv_pointList.Columns[i].HeaderText);
                            dgv_pointList.Rows[selectRow].Cells[i].Value = curPos;
                        }
                        break;
                    case CardType.凌华_AMP204C:
                        if (!Card_ADLink.initSucceed && !Project.Instance.configuration.vitualCard)
                        {
                            Frm_MessageBox messageBox = new Frm_MessageBox();
                            messageBox.MessageBoxShow("\r\n未识别到相应运动控制卡，不可示教");
                            return;
                        }
                        Frm_ConfirmBox.Instance.lbl_info.Text = Project.Instance.configuration.language == Language.English ? "Are you sure you want to overwrite the point data?" : "确定要覆盖此点？";
                        Frm_ConfirmBox.Instance.ShowDialog();
                        if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Cancel)
                        {
                            return;
                        }
                        selectRow = dgv_pointList.SelectedRows[0].Index;
                        for (int i = 3; i < dgv_pointList.Columns.Count; i++)
                        {
                            double curPos = Card_ADLink.GetCurPosition(dgv_pointList.Columns[i].HeaderText);
                            dgv_pointList.Rows[selectRow].Cells[i].Value = curPos;
                        }
                        break;
                    case CardType.安川_MP3100:
                        if (!Card_Ymc3100.initSucceed && !Project.Instance.configuration.vitualCard)
                        {
                            Frm_MessageBox messageBox = new Frm_MessageBox();
                            messageBox.MessageBoxShow("\r\n未识别到相应运动控制卡，不可示教");
                            return;
                        }
                        //Frm_ConfirmBox.Instance.lbl_info.Text = Project.Instance.configuration.language == Language.English ? "Are you sure you want to overwrite the point data?" : "确定要覆盖此点？";
                        //Frm_ConfirmBox.Instance.ShowDialog();
                        //if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Cancel)
                        //{
                        //    return;
                        //}
                        selectRow = dgv_pointList.SelectedRows[0].Index;
                        for (int i = 3; i < dgv_pointList.Columns.Count - 1; i++)
                        {
                            double curPos = Card_Ymc3100.GetEncPos((Axis)Enum.Parse(typeof(Axis), dgv_pointList.Columns[i].HeaderText));
                            dgv_pointList.Rows[selectRow].Cells[i].Value = curPos;
                        }
                        break;
                    default:
                        Frm_MessageBox.Instance.MessageBoxShow("未指定板卡类型，操作失败");
                        break;
                }
                Save();
                Project.SaveProject();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button10_Click_3(object sender, EventArgs e)
        {
            try
            {
                if (dgv_pointList.SelectedRows.Count == 0)
                {
                    OutputMsg("请先选中点表中的具体行后删除", Color.Red);
                    return;
                }
                int tableIndex = GetSelectedSmartPositionTableModelIndex();
                if (tableIndex < 0)
                {
                    OutputMsg("当前没有可用点表", Color.Red);
                    return;
                }
                SmartPosTable.GoPos(tableIndex, dgv_pointList.SelectedRows[0].Index);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            try
            {
                //if (File.Exists(Application.StartupPath + "\\Config\\Project\\Motion\\Point.xml"))
                //{
                //    File.Delete(Application.StartupPath + "\\Config\\Project\\Motion\\Point.xml");
                //}
                //XmlDocument xmlDoc = new XmlDocument();
                //XmlDeclaration xmlSM = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                //xmlDoc.AppendChild(xmlSM);
                //XmlElement root = xmlDoc.CreateElement("", "PointList", "");
                //xmlDoc.AppendChild(root);
                //for (int i = 0; i < dgv_pointList.Rows.Count - 1; i++)
                //{
                //    string pointName = dgv_pointList.Rows[i].Cells[1].Value.ToString();
                //    XmlElement pointElement = xmlDoc.CreateElement(pointName);
                //    XmlNode node = root.AppendChild(pointElement);
                //    for (int j = 2; j < dgv_pointList.Columns.Count; j++)
                //    {
                //        string axisName = dgv_pointList.Columns[j].HeaderText;
                //        node = pointElement.AppendChild(xmlDoc.CreateElement("", axisName, ""));
                //        if (dgv_pointList.Rows[i].Cells[j].Value == null)
                //            dgv_pointList.Rows[i].Cells[j].Value = string.Empty;
                //        node.InnerText = dgv_pointList.Rows[i].Cells[j].Value.ToString();
                //    }
                //}
                //Application.DoEvents();
                //xmlDoc.Save(Application.StartupPath + "\\Config\\Project\\Motion\\Point.xml");

                Save();
                Project.SaveProject();
                Frm_MessageBox.Instance.MessageBoxShow("\r\n保存成功");
            }
            catch (Exception ex)
            {
                Frm_MessageBox.Instance.MessageBoxShow("\r\n表中存在非法数据，请更正后保存", TipType.Error);
            }
        }
        private void Save()
        {
            try
            {
                Project.Instance.curEngine.smartPosTable.FindTable(comboBox1.TextStr).L_pos.Clear();
                for (int i = 0; i < dgv_pointList.Rows.Count - 1; i++)
                {
                    List<double> point = new List<double>();
                    for (int j = 3; j < dgv_pointList.Columns.Count - 1; j++)
                    {
                        point.Add(Convert.ToDouble(dgv_pointList.Rows[i].Cells[j].Value));
                    }

                    int index = (dgv_pointList.Rows[i].Cells[0].Value == null ? 0 : Convert.ToInt16(dgv_pointList.Rows[i].Cells[0].Value));
                    string posName = (dgv_pointList.Rows[i].Cells[1].Value == null ? "" : dgv_pointList.Rows[i].Cells[1].Value.ToString());
                    int vel = (dgv_pointList.Rows[i].Cells[2].Value == null ? 10 : Convert.ToInt16(dgv_pointList.Rows[i].Cells[2].Value));
                    string info = (dgv_pointList.Rows[i].Cells[dgv_pointList.Columns.Count - 1].Value == null ? "" : dgv_pointList.Rows[i].Cells[dgv_pointList.Columns.Count - 1].Value.ToString());
                    Pos pos = new Pos(index, posName, vel, point, info);
                    Project.Instance.curEngine.smartPosTable.FindTable(comboBox1.TextStr).L_pos.Add(pos);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void button13_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dgv_pointList.SelectedRows.Count == 0)
                {
                    OutputMsg("请先选中点表中的具体行后删除", Color.Red);
                    return;
                }
                int selectRow = dgv_pointList.SelectedRows[0].Index;
                dgv_pointList.Rows.RemoveAt(selectRow);

                Save();
                ////////自动保存一下
                //////if (!File.Exists(Application.StartupPath + "\\Config\\Project\\Motion\\Point.xml"))
                //////{
                //////    File.Create(Application.StartupPath + "\\Config\\Project\\Motion\\Point.xml").Close();
                //////}
                //////XmlDocument xmlDoc = new XmlDocument();
                //////XmlDeclaration xmlSM = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                //////xmlDoc.AppendChild(xmlSM);
                //////XmlElement root = xmlDoc.CreateElement("", "Root", "");
                //////xmlDoc.AppendChild(root);
                //////for (int i = 0; i < dgv_pointList.Rows.Count - 1; i++)
                //////{
                //////    string pointName = dgv_pointList.Rows[i].Cells[1].Value.ToString();
                //////    XmlElement pointElement = xmlDoc.CreateElement(pointName);
                //////    root.AppendChild(pointElement);
                //////    for (int j = 2; j < dgv_pointList.Columns.Count; j++)
                //////    {
                //////        string axisName = dgv_pointList.Rows[i].Cells[j].Value.ToString();
                //////        XmlNode node = pointElement.AppendChild(xmlDoc.CreateElement("", axisName, ""));
                //////        node.InnerText = dgv_pointList.Rows[i].Cells[j].Value.ToString();
                //////    }
                //////}
                //////Application.DoEvents();
                //////xmlDoc.Save(Application.StartupPath + "\\Config\\Project\\Motion\\Point.xml");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            Save();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Project.Instance.curEngine.smartPosTable.velPer = Convert.ToDouble(comboBox2.TextStr.Substring(0, comboBox2.TextStr.Length - 1)) / 100;
        }
        internal static bool bInit = true;
        private void dgv_pointList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (bInit)
                    return;

                Save();
                Project.SaveProject();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void Frm_MotionControl_Shown(object sender, EventArgs e)
        {
            bInit = false;
        }

        private void cCheckBox1_Load(object sender, EventArgs e)
        {

        }

        private void cCheckBox1_CheckChanged(bool Checked)
        {
            try
            {
                if (Checked)
                {
                    this.panel1.Dock = DockStyle.Top;
                    this.panel1.Height = 23;
                    this.Parent = null;
                    this.TopLevel = true;
                    this.WindowState = FormWindowState.Normal;
                    this.panel2.Location = new System.Drawing.Point(2, 23);
                    this.BackColor = Color.FromArgb(46, 141, 230);
                    this.panel2.Height = this.Height - 25;
                    //this.panel2.Width = this.panel2.Width  - 5;
                    this.Size = new Size(this.Size.Width, this.Size.Height + 23);
                    this.Show();
                    SetRefreshActive(true);
                }
                else
                {
                    this.panel2.Location = new System.Drawing.Point(2, 2);
                    this.panel2.Height = this.Height - 2;
                    this.BackColor = Color.White;
                    this.Size = new Size(this.Size.Width, this.Size.Height - 21);
                    this.panel1.Dock = DockStyle.None;
                    this.panel1.Height = 0;
                    Frm_MotionControl.Instance.TopLevel = false;
                    Frm_MotionControl.Instance.Parent = Frm_Main.Instance.panel4;
                    Frm_MotionControl.Instance.Dock = DockStyle.Fill;
                    Frm_MotionControl.Instance.Show();
                    Frm_Main.Instance.UpdateMotionRefreshActivity();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void comboBox1_SelectedIndexChanged()
        {
            Project.Instance.curEngine.smartPosTable.LoadData(dgv_pointList, comboBox1.TextStr);
        }

        private void comboBox2_SelectedIndexChanged()
        {
            Project.Instance.curEngine.smartPosTable.velPer = Convert.ToDouble(comboBox2.TextStr.Substring(0, comboBox2.TextStr.Length - 1)) / 100;
        }

        private void btn_move_Click(object sender, EventArgs e)
        {
            btn_jog.BackColor = Color.Silver;
            btn_move.BackColor = Color.Green;
            btn_YForward.Enabled = true;
            btn_YBackward.Enabled = true;
            btn_ZMoveDown.Enabled = true;
            btn_ZMoveUp.Enabled = true;
            btn_RRotateFormward.Enabled = true;
            btn_RRotateBackward.Enabled = true;
            btn_widthPCBBackward.Enabled = true;
            btn_widthPCBForeward.Enabled = true;
        }

        private void btn_jog_Click(object sender, EventArgs e)
        {
            btn_jog.BackColor = Color.Green;
            btn_move.BackColor = Color.Silver;
            btn_YForward.Enabled = false;
            btn_YBackward.Enabled = false;
            btn_ZMoveDown.Enabled = false;
            btn_ZMoveUp.Enabled = false;
            btn_RRotateFormward.Enabled = false;
            btn_RRotateBackward.Enabled = false;
            btn_widthPCBBackward.Enabled = false;
            btn_widthPCBForeward.Enabled = false;
        }

        private void btn_YForward_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_YForward.Enabled = false;
                Card_Ymc3100.YMoveRel(Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()));
                btn_YForward.Enabled = true;
            }
        }

        private void btn_XMoveLeft_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_XMoveLeft.Enabled = false;
                Card_Ymc3100.MoveRel(Axis.X, Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
                btn_XMoveLeft.Enabled = true;
            }
        }

        private void btn_XMoveRight_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_XMoveRight.Enabled = false;
                Card_Ymc3100.MoveRel(Axis.X, -Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
                btn_XMoveRight.Enabled = true;
            }
        }

        private void btn_YBackward_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_YBackward.Enabled = false;
                Card_Ymc3100.YMoveRel(-Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()));
                btn_YBackward.Enabled = true;
            }
        }

        private void btn_ZMoveUp_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_ZMoveUp.Enabled = false;
                Card_Ymc3100.MoveRel(Axis.Z, -Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
                btn_ZMoveUp.Enabled = true;
            }
        }

        private void btn_ZMoveDown_Click(object sender, EventArgs e)
        {
            //////try
            //////{
            //////    if (btn_move.BackColor == Color.Green)
            //////    {
            //////        btn_ZMoveDown.Enabled = false;

            //////        //添加安全防呆保护
            //////        double pressValue = Card_Ymc3100.ReadData(MemArea.ML, 5000);
            //////        if (pressValue > 3000)                    //压力防呆检查
            //////        {
            //////            btn_ZMoveDown.Enabled = true;
            //////            MessageBox.Show("当前压力已超过3.5T，不可继续下压");
            //////            return;
            //////        }
            //////        double ZPos = Card_Ymc3100.GetEncPos(Axis.Z);
            //////        if (ZPos + (Convert.ToDouble(cbx_distance.TextStr.Trim())) > Frm_UserForm.Instance.parameter.maxSafetyPos)    //超过安全位置提示执行
            //////        {
            //////            if (MessageBox.Show(string.Format("超出安全位置{0}mm，确定要执行吗？", Math.Round(ZPos + (Convert.ToDouble(cbx_distance.TextStr .Trim())) - Frm_UserForm.Instance.parameter.maxSafetyPos)), "安全确认", MessageBoxButtons.YesNo) != DialogResult.Yes)
            //////            {
            //////                btn_ZMoveDown.Enabled = true;
            //////                return;
            //////            }
            //////        }

            //////        if ((ZPos + (Convert.ToDouble(cbx_distance.TextStr.Trim())) > Frm_UserForm.Instance.parameter.maxSafetyPos) && Convert.ToDouble(cbx_distance.TextStr .Trim()) > 0.1)    //超过安全位置点动距离限制
            //////        {
            //////            MessageBox.Show("目标位置超过安全位置时，点动距离不得超过0.1，已放弃执行");
            //////            btn_ZMoveDown.Enabled = true;
            //////            return;
            //////        }

            //////        if (Convert.ToDouble(cbx_distance.TextStr.Trim()) > 5)    //距离限制
            //////        {
            //////            MessageBox.Show("Z轴点动距离不可大于5mm，已放弃执行");
            //////            btn_ZMoveDown.Enabled = true;
            //////            return;
            //////        }

            //////        Card_Ymc3100.MoveRel(Axis.Z, Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
            //////        btn_ZMoveDown.Enabled = true;
            //////    }
            //////}
            //////catch (Exception ex)
            //////{
            //////    btn_ZMoveDown.Enabled = true;
            //////    Log.SaveError(ex);
            //////}
        }

        private void btn_RRotateBackward_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                //R轴旋转过多可能会把线拉住，所以此处添加防呆检查
                double RPos = Card_Ymc3100.GetEncPos(Axis.R);
                if (RPos - Convert.ToDouble(cbx_distance.TextStr.Trim()) < -60)
                {
                    MessageBox.Show("R轴目标位置小于-60度，可能会拉住感应器线，已放弃执行");
                    return;
                }

                btn_RRotateBackward.Enabled = false;
                Card_Ymc3100.MoveRel(Axis.R, -Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
                btn_RRotateBackward.Enabled = true;
            }
        }

        private void btn_RRotateFormward_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                //R轴旋转过多可能会把线拉住，所以此处添加防呆检查
                double RPos = Card_Ymc3100.GetEncPos(Axis.R);
                if (RPos + Convert.ToDouble(cbx_distance.TextStr.Trim()) > 60)
                {
                    MessageBox.Show("R轴目标位置超过60度，可能会拉住感应器线，已放弃执行");
                    return;
                }

                btn_RRotateFormward.Enabled = false;
                Card_Ymc3100.MoveRel(Axis.R, Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
                btn_RRotateFormward.Enabled = true;
            }
        }

        private void btn_HomeX_Click(object sender, EventArgs e)
        {
            try
            {
                lbl_tip.Text = "X轴开始回零";
                lbl_tip.BackColor = Color.White;
                btn_HomeX.Enabled = false;
                Application.DoEvents();
                Card_Ymc3100.WriteData(MemArea.GB, 600, 0);
                Thread.Sleep(100);
                Card_Ymc3100.WriteData(MemArea.GB, 600, 1);
                Thread.Sleep(100);
                Card_Ymc3100.ReadDataWait(MemArea.MB, 100, 1);
                Card_Ymc3100.FindAxisByName(Axis.X).homeOK = true;
                btn_HomeX.Enabled = true;
                lbl_tip.Text = "X轴回零完成";
                lbl_tip.BackColor = Color.Green;
            }
            catch (Exception ex)
            {
                btn_HomeX.Enabled = true;
                Log.SaveError(ex);
            }
        }

        private void btn_homeY_Click(object sender, EventArgs e)
        {
            try
            {
                lbl_tip.Text = "Y轴开始回零";
                lbl_tip.BackColor = Color.White;
                btn_homeY.Enabled = false;
                Application.DoEvents();
                Card_Ymc3100.WriteData(MemArea.GB, 200, 0);
                Thread.Sleep(100);
                Card_Ymc3100.WriteData(MemArea.GB, 200, 1);
                Thread.Sleep(100);
                Card_Ymc3100.ReadDataWait(MemArea.GB, 203, 1);
                Card_Ymc3100.FindAxisByName(Axis.YR).homeOK = true;
                btn_homeY.Enabled = true;
                lbl_tip.Text = "Y轴回零完成";
                lbl_tip.BackColor = Color.Green;
            }
            catch (Exception ex)
            {
                btn_homeY.Enabled = true;
                Log.SaveError(ex);
            }
        }

        private void btn_homeZ_Click(object sender, EventArgs e)
        {
            try
            {
                lbl_tip.Text = "Z轴开始回零";
                lbl_tip.BackColor = Color.White;
                btn_homeZ.Enabled = false;
                Application.DoEvents();
                Card_Ymc3100.WriteData(MemArea.GB, 620, 0);
                Thread.Sleep(100);
                Card_Ymc3100.WriteData(MemArea.GB, 620, 1);
                Thread.Sleep(100);
                Card_Ymc3100.ReadDataWait(MemArea.MB, 130, 1);
                Card_Ymc3100.FindAxisByName(Axis.Z).homeOK = true;
                btn_homeZ.Enabled = true;
                lbl_tip.Text = "Z轴回零完成";
                lbl_tip.BackColor = Color.Green;
            }
            catch (Exception ex)
            {
                btn_homeZ.Enabled = true;
                Log.SaveError(ex);
            }
        }

        private void btn_homeR_Click(object sender, EventArgs e)
        {
            try
            {
                lbl_tip.Text = "R轴开始回零";
                lbl_tip.BackColor = Color.White;
                btn_homeR.Enabled = false;
                Application.DoEvents();
                Card_Ymc3100.WriteData(MemArea.GB, 610, 0);
                Thread.Sleep(100);
                Card_Ymc3100.WriteData(MemArea.GB, 610, 1);
                Thread.Sleep(100);
                Card_Ymc3100.ReadDataWait(MemArea.MB, 140, 1);
                Card_Ymc3100.FindAxisByName(Axis.R).homeOK = true;
                btn_homeR.Enabled = true;
                lbl_tip.Text = "R轴回零完成";
                lbl_tip.BackColor = Color.Green;
            }
            catch (Exception ex)
            {
                btn_homeR.Enabled = true;
                Log.SaveError(ex);
            }
        }

        private void btn_enableX_Click(object sender, EventArgs e)
        {
            if (btn_enableX.BackColor == Color.Green)
            {
                Card_Ymc3100.MotorOff(Axis.X);
                btn_enableX.BackColor = Color.White;
            }
            else
            {
                Card_Ymc3100.MotorOn(Axis.X);
                btn_enableX.BackColor = Color.Green;
            }
        }

        private void btn_enableY_Click(object sender, EventArgs e)
        {
            if (btn_enableY.BackColor == Color.Green)
            {
                Card_Ymc3100.MotorOff(Axis.YL);
                Card_Ymc3100.MotorOff(Axis.YR);
                btn_enableY.BackColor = Color.White;
            }
            else
            {
                Card_Ymc3100.MotorOn(Axis.YL);
                Card_Ymc3100.MotorOn(Axis.YR);
                btn_enableY.BackColor = Color.Green;
            }
        }

        private void btn_enableZ_Click(object sender, EventArgs e)
        {
            if (btn_enableZ.BackColor == Color.Green)
            {
                Card_Ymc3100.MotorOff(Axis.Z);
                btn_enableZ.BackColor = Color.White;
            }
            else
            {
                Card_Ymc3100.MotorOn(Axis.Z);
                btn_enableZ.BackColor = Color.Green;
            }
        }

        private void btn_enableR_Click(object sender, EventArgs e)
        {
            if (btn_enableR.BackColor == Color.Green)
            {
                Card_Ymc3100.MotorOff(Axis.R);
                btn_enableR.BackColor = Color.White;
            }
            else
            {
                Card_Ymc3100.MotorOn(Axis.R);
                btn_enableR.BackColor = Color.Green;
            }
        }

        private void btn_leftLoadPCB_Click(object sender, EventArgs e)
        {
            //////if (!Frm_UserForm.Instance.m左侧轨道运动中)
            //////{
            //////    Frm_UserForm.Instance.m左侧轨道运动中 = true;
            //////    btn_leftLoadPCB.BackColor = Color.Green;
            //////    Application.DoEvents();
            //////    Card_Ymc3100.KeepMoveStart(Axis.左侧轨道, HomeDir.P_正方向, Convert.ToInt16(cbx_vel.TextStr.Trim()));
            //////}
            //////else
            //////{
            //////    Frm_UserForm.Instance.m左侧轨道运动中 = false;
            //////    btn_leftLoadPCB.BackColor = Color.Gainsboro;
            //////    Application.DoEvents();
            //////    Card_Ymc3100.KeepMoveStop(Axis.左侧轨道);
            //////}
        }

        private void btn_middleLoadPCB_Click(object sender, EventArgs e)
        {
            //////if (!Frm_UserForm.Instance.m中间轨道运动中)
            //////{
            //////    Frm_UserForm.Instance.m中间轨道运动中 = true;
            //////    btn_middleLoadPCB.BackColor = Color.Green;
            //////    Application.DoEvents();
            //////    Card_Ymc3100.KeepMoveStart(Axis.中间轨道, HomeDir.P_正方向, Convert.ToInt16(cbx_vel.TextStr.Trim()));
            //////}
            //////else
            //////{
            //////    Frm_UserForm.Instance.m中间轨道运动中 = false;
            //////    btn_middleLoadPCB.BackColor = Color.Gainsboro;
            //////    Application.DoEvents();
            //////    Card_Ymc3100.KeepMoveStop(Axis.中间轨道);
            //////}
        }

        private void btn_rightLoadPCB_Click(object sender, EventArgs e)
        {
            //////if (!Frm_UserForm.Instance.m右侧轨道运动中)
            //////{
            //////    Frm_UserForm.Instance.m右侧轨道运动中 = true;
            //////    btn_rightLoadPCB.BackColor = Color.Green;
            //////    Application.DoEvents();
            //////    Card_Ymc3100.KeepMoveStart(Axis.右侧轨道, HomeDir.P_正方向, Convert.ToInt16(cbx_vel.TextStr.Trim()));
            //////}
            //////else
            //////{
            //////    Frm_UserForm.Instance.m右侧轨道运动中 = false;
            //////    btn_rightLoadPCB.BackColor = Color.Gainsboro;
            //////    Application.DoEvents();
            //////    Card_Ymc3100.KeepMoveStop(Axis.右侧轨道);
            //////}
        }

        private void btn_widthPCBForeward_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_widthPCBForeward.Enabled = false;
                Card_Ymc3100.PCBWidthMoveRel(-Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()));
                btn_widthPCBForeward.Enabled = true;
            }
        }

        private void btn_widthPCBBackward_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_widthPCBBackward.Enabled = false;
                Card_Ymc3100.PCBWidthMoveRel(Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()));
                btn_widthPCBBackward.Enabled = true;
            }
        }

        private void btn_go_Click(object sender, EventArgs e)
        {
            btn_go.Enabled = false;
            if (cbx_axisList.TextStr == "Z" && Convert.ToInt16(cbx_vel.TextStr.Trim()) > 10)
            {
                MessageBox.Show("Z轴运行速度不得大于10mm/s，已放弃执行");
                btn_go.Enabled = true;
                return;
            }
            if (cbx_axisList.TextStr == "Y")
                Card_Ymc3100.YMoveAbs(Convert.ToDouble(tbx_position.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()));
            else
                Card_Ymc3100.MoveAbs((Axis)Enum.Parse(typeof(Axis), cbx_axisList.TextStr), Convert.ToDouble(tbx_position.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
            btn_go.Enabled = true;
        }

        private void btn_XMoveLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (btn_jog.BackColor == Color.Green)
                Card_Ymc3100.JogStart(Axis.X, HomeDir.P_正方向, Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
        }

        private void btn_XMoveLeft_MouseUp(object sender, MouseEventArgs e)
        {
            if (btn_jog.BackColor == Color.Green)
                Card_Ymc3100.JogStop(Axis.X);
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            Frm_PosTableEdit.Instance.Show();
        }

        private void btn_homeTR_Click(object sender, EventArgs e)
        {
            try
            {
                lbl_tip.Text = "TR轴开始回零";
                lbl_tip.BackColor = Color.White;
                btn_homeTR.Enabled = false;
                Application.DoEvents();
                Card_Ymc3100.Home(Axis.TR, 10, HomeDir.N_负方向, 100);
                Card_Ymc3100.FindAxisByName(Axis.TR).homeOK = true;
                btn_homeTR.Enabled = true;
                lbl_tip.Text = "TR轴回零完成";
                lbl_tip.BackColor = Color.Green;
            }
            catch (Exception ex)
            {
                btn_homeTR.Enabled = true;
                Log.SaveError(ex);
            }
        }

        private void btn_TRMoveLeft_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_TRMoveLeft.Enabled = false;
                Card_Ymc3100.MoveRel(Axis.TR, Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
                btn_TRMoveLeft.Enabled = true;
            }
        }

        private void btn_XMoveRight_Click_1(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_XMoveRight.Enabled = false;
                Card_Ymc3100.MoveRel(Axis.X, -Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
                btn_XMoveRight.Enabled = true;
            }
        }

        private void btn_TRMoveRight_Click(object sender, EventArgs e)
        {
            if (btn_move.BackColor == Color.Green)
            {
                btn_TRMoveRight.Enabled = false;
                Card_Ymc3100.MoveRel(Axis.TR, -Convert.ToDouble(cbx_distance.TextStr.Trim()), Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
                btn_TRMoveRight.Enabled = true;
            }
        }

        private void btn_enableTR_Click(object sender, EventArgs e)
        {
            if (btn_enableTR.BackColor == Color.Green)
            {
                Card_Ymc3100.MotorOff(Axis.TR);
                btn_enableTR.BackColor = Color.White;
            }
            else
            {
                Card_Ymc3100.MotorOn(Axis.TR);
                btn_enableTR.BackColor = Color.Green;
            }
        }

        private void btn_TRMoveLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (btn_jog.BackColor == Color.Green)
                Card_Ymc3100.JogStart(Axis.TR, HomeDir.P_正方向, Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
        }

        private void btn_TRMoveLeft_MouseUp(object sender, MouseEventArgs e)
        {
            if (btn_jog.BackColor == Color.Green)
                Card_Ymc3100.JogStop(Axis.TR);
        }

        private void btn_TRMoveRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (btn_jog.BackColor == Color.Green)
                Card_Ymc3100.JogStart(Axis.TR, HomeDir.N_负方向, Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
        }

        private void btn_TRMoveRight_MouseUp(object sender, MouseEventArgs e)
        {
            if (btn_jog.BackColor == Color.Green)
                Card_Ymc3100.JogStop(Axis.TR);
        }

        private void btn_XMoveRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (btn_jog.BackColor == Color.Green)
                Card_Ymc3100.JogStart(Axis.X, HomeDir.N_负方向, Convert.ToInt16(cbx_vel.TextStr.Trim()), true);
        }

        private void btn_XMoveRight_MouseUp(object sender, MouseEventArgs e)
        {
            if (btn_jog.BackColor == Color.Green)
                Card_Ymc3100.JogStop(Axis.X);
        }

        private void toolStripButton33_Click(object sender, EventArgs e)
        {
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;

            Frm_DeviceManager.Instance.Show();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            try
            {
                int index = dgv_pointList.SelectedRows[0].Index;
                if (index == 0)
                    return;

                int tableIndex = GetSelectedSmartPositionTableModelIndex();
                if (tableIndex < 0)
                    return;

                Pos temp = Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[index - 1];
                Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[index - 1] = Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[index];
                Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[index] = temp;

                for (int i = 0; i < dgv_pointList.Columns.Count; i++)
                {
                    object obj = dgv_pointList.Rows[dgv_pointList.SelectedRows[0].Index - 1].Cells[i].Value.ToString();
                    dgv_pointList.Rows[dgv_pointList.SelectedRows[0].Index - 1].Cells[i].Value = dgv_pointList.SelectedRows[0].Cells[i].Value;
                    dgv_pointList.SelectedRows[0].Cells[i].Value = obj;
                }

                dgv_pointList.Rows[index - 1].Selected = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button5_Click_2(object sender, EventArgs e)
        {
            try
            {
                int index = dgv_pointList.SelectedRows[0].Index;
                int tableIndex = GetSelectedSmartPositionTableModelIndex();
                if (tableIndex < 0)
                    return;

                if (index == Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos.Count - 1)
                    return;

                Pos temp = Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[index + 1];
                Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[index + 1] = Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[index];
                Project.Instance.curEngine.smartPosTable.L_Table[tableIndex].L_pos[index] = temp;

                for (int i = 0; i < dgv_pointList.Columns.Count; i++)
                {
                    string obj = dgv_pointList.Rows[dgv_pointList.SelectedRows[0].Index + 1].Cells[i].Value.ToString();
                    dgv_pointList.Rows[dgv_pointList.SelectedRows[0].Index + 1].Cells[i].Value = dgv_pointList.SelectedRows[0].Cells[i].Value;
                    dgv_pointList.SelectedRows[0].Cells[i].Value = obj;
                }


                dgv_pointList.Rows[index + 1].Selected = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void dgv_axisInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



    }
}

