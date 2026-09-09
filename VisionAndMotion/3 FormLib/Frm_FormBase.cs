using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    public partial class Frm_FormBase : Form
    {
        private bool titleButtonsAligned = false;
        private ToolTip titleButtonToolTip;

        internal Frm_FormBase()
        {
            InitializeComponent();
            this.MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            this.TopMost = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ModernUiTheme.Apply(this);
            AlignTitleButtons();
            ConfigureTitleButtons();
            titleButtonsAligned = true;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (titleButtonsAligned)
            {
                AlignTitleButtons();
                UpdateMaximizeButtonGlyph();
            }
        }

        #region 任务栏入口：让模态弹窗拥有独立的任务栏按钮
        // 弹窗模态显示时 owned 于（不可见的）dummy owner，而按 Windows 规则，
        // owned 窗口默认不会出现在任务栏上——这正是“弹窗打开后任务栏没有任何入口、
        // 最小化后再也找不回来”的根因。这里用 WS_EX_APPWINDOW + ShowInTaskbar=true
        // 双保险：无论 owner 是谁，弹窗在打开期间都在任务栏上拥有属于自己的按钮（侧体），
        // 最小化后随时可以点击它还原。
        private bool forceTaskbarButton = false;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (forceTaskbarButton)
                    cp.ExStyle |= 0x00040000; // WS_EX_APPWINDOW：强制窗口出现在任务栏
                return cp;
            }
        }

        /// <summary>
        /// 弹窗模态显示前调用：为其获得独立的任务栏按钮。
        /// </summary>
        private void GainTaskbarEntry()
        {
            forceTaskbarButton = true;      // CreateParams 建句柄时会据此添加 WS_EX_APPWINDOW
            this.ShowInTaskbar = true;      // 去掉 WS_EX_TOOLWINDOW（它会把窗口从任务栏排除）
            EnsureTaskbarText();            // 任务栏按钮显示正确的标题
            if (this.IsHandleCreated)
                this.RecreateHandle();      // 单例复用时句柄已存在，必须重建才能使新样式生效
        }

        /// <summary>
        /// 弹窗模态结束后调用：还原为模态前的任务栏属性。
        /// </summary>
        private void ReleaseTaskbarEntry(bool oldShowInTaskbar)
        {
            // 先摘标志再恢复属性：这样恢复 ShowInTaskbar 触发的句柄重建不会带上 WS_EX_APPWINDOW
            forceTaskbarButton = false;
            this.ShowInTaskbar = oldShowInTaskbar;
        }

        /// <summary>
        /// 任务栏按钮的标题取自窗口文本；个别弹窗 Text 还是设计器默认值时，用标题栏文本兜底。
        /// </summary>
        private void EnsureTaskbarText()
        {
            try
            {
                string titleText = lbl_title != null ? lbl_title.Text : null;
                bool textIsBad = string.IsNullOrEmpty(this.Text) || this.Text == "Frm_ToolBase" || this.Text == this.Name;
                bool labelIsGood = !string.IsNullOrEmpty(titleText) && titleText != "label1";
                if (textIsBad && labelIsGood)
                    this.Text = titleText;
            }
            catch
            {
                // 兑底标题失败不影响弹窗显示
            }
        }
        #endregion

        public new DialogResult ShowDialog()
        {
            return ShowTopMostDialog();
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            return ShowTopMostDialog(owner);
        }

        private DialogResult ShowTopMostDialog()
        {
            return ShowTopMostDialog(null);
        }

        private DialogResult ShowTopMostDialog(IWin32Window requestedOwner)
        {
            // 先记录当前活动窗口（通常是主窗体；弹窗套弹窗时是父弹窗）。
            // 弹窗被“×/最小化”隐藏后，Windows 会把激活焦点交给不可见的 dummy owner，
            // 主窗体不会自动回到前台，看起来就像“所有窗口一起消失/程序卡死”，
            // 因此模态结束后必须手动把焦点还给原来的窗口。
            Form returnFocus = Form.ActiveForm;

            // 有明确 owner 时保持 Windows 原有的窗口层级；不设置 TopMost，也不强制激活。
            // 这适用于流程的新建、克隆、删除等普通编辑操作，避免窗口跳动和闪烁。
            bool oldShowInTaskbar = this.ShowInTaskbar;

            Form owner = requestedOwner as Form;
            if (owner != null && !owner.IsDisposed)
            {
                GainTaskbarEntry();
                DialogResult dr;
                try
                {
                    dr = base.ShowDialog(owner);
                }
                finally
                {
                    ReleaseTaskbarEntry(oldShowInTaskbar);
                }
                RestoreFocusAfterModal(returnFocus);
                return dr;
            }

            bool oldTopMost = this.TopMost;
            this.TopMost = true;

            GainTaskbarEntry();

            using (Form topMostOwner = CreateTopMostOwner())
            {
                try
                {
                    topMostOwner.Show();
                    topMostOwner.BringToFront();
                    this.BringToFront();
                    this.Activate();

                    DialogResult dr;
                    try
                    {
                        dr = base.ShowDialog(topMostOwner);
                    }
                    finally
                    {
                        ReleaseTaskbarEntry(oldShowInTaskbar);
                    }
                    RestoreFocusAfterModal(returnFocus);
                    return dr;
                }
                finally
                {
                    this.TopMost = oldTopMost;
                }
            }
        }

        /// <summary>
        /// 模态弹窗结束后，把激活焦点交还给弹出弹窗之前的窗口（通常是主窗体）。
        /// 此时主窗体已被重新启用，Activate() 能把它带回前台，避免程序“看起来全没了”。
        /// </summary>
        private static void RestoreFocusAfterModal(Form returnFocus)
        {
            try
            {
                if (returnFocus == null || returnFocus.IsDisposed || !returnFocus.Visible)
                    return;
                if (returnFocus == Form.ActiveForm)
                    return;
                if (returnFocus.WindowState == FormWindowState.Minimized)
                    returnFocus.WindowState = FormWindowState.Normal;
                returnFocus.Activate();
            }
            catch
            {
                // 焦点还原因任何原因失败都不应影响弹窗本身的返回值
            }
        }

        private static Form CreateTopMostOwner()
        {
            Form owner = new Form();
            owner.FormBorderStyle = FormBorderStyle.None;
            owner.ShowInTaskbar = false;
            owner.StartPosition = FormStartPosition.Manual;
            owner.Size = new Size(1, 1);
            owner.Location = new Point(-32000, -32000);
            owner.TopMost = true;
            return owner;
        }

        private void AlignTitleButtons()
        {
            const int buttonWidth = 34;
            int buttonHeight = Math.Max(25, panel1.Height);

            if (button100.Parent != panel1)
            {
                if (button100.Parent != null)
                    button100.Parent.Controls.Remove(button100);
                panel1.Controls.Add(button100);
            }

            button100.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_baseClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            button100.Size = new Size(buttonWidth, buttonHeight);
            button1.Size = new Size(buttonWidth, buttonHeight);
            button2.Size = new Size(buttonWidth, buttonHeight);
            btn_baseClose.Size = new Size(buttonWidth, buttonHeight);

            btn_baseClose.Location = new Point(panel1.Width - buttonWidth, 0);
            button2.Location = new Point(panel1.Width - buttonWidth * 2, 0);
            button1.Location = new Point(panel1.Width - buttonWidth * 3, 0);
            button100.Location = new Point(panel1.Width - buttonWidth * 4, 0);
            button100.BringToFront();
            button1.BringToFront();
            button2.BringToFront();
            btn_baseClose.BringToFront();
        }

        private void ConfigureTitleButtons()
        {
            int iconSize = ModernVectorIconFactory.GetPixelSize(panel1, 16);
            Button[] buttons = { button100, button1, button2, btn_baseClose };
            string[] names = { "置顶", "最小化", "最大化或还原", "关闭" };

            if (titleButtonToolTip == null)
            {
                titleButtonToolTip = new ToolTip();
                titleButtonToolTip.ShowAlways = true;
            }

            for (int index = 0; index < buttons.Length; index++)
            {
                Button button = buttons[index];
                button.Text = string.Empty;
                button.BackgroundImage = null;
                button.BackgroundImageLayout = ImageLayout.Center;
                button.ImageAlign = ContentAlignment.MiddleCenter;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor = button == btn_baseClose
                    ? ModernUiTheme.Danger
                    : ModernUiTheme.AccentHover;
                button.FlatAppearance.MouseDownBackColor = button == btn_baseClose
                    ? Color.FromArgb(183, 45, 45)
                    : ModernUiTheme.AccentPressed;
                button.AccessibleName = names[index];
                titleButtonToolTip.SetToolTip(button, names[index]);
            }

            button100.Image = ModernVectorIconFactory.Get(ModernVectorIconFactory.Glyph.Pin, iconSize, Color.White);
            button1.Image = ModernVectorIconFactory.Get(ModernVectorIconFactory.Glyph.Minimize, iconSize, Color.White);
            btn_baseClose.Image = ModernVectorIconFactory.Get(ModernVectorIconFactory.Glyph.Close, iconSize, Color.White);
            UpdateMaximizeButtonGlyph();
            UpdatePinButtonVisual();
        }

        internal void RefreshTitleButtonVisuals()
        {
            if (IsDisposed)
                return;
            ConfigureTitleButtons();
        }

        private void UpdateMaximizeButtonGlyph()
        {
            if (button2 == null || button2.IsDisposed)
                return;
            int iconSize = ModernVectorIconFactory.GetPixelSize(panel1, 16);
            ModernVectorIconFactory.Glyph glyph = WindowState == FormWindowState.Maximized
                ? ModernVectorIconFactory.Glyph.Restore
                : ModernVectorIconFactory.Glyph.Maximize;
            button2.Image = ModernVectorIconFactory.Get(glyph, iconSize, Color.White);
        }

        private void UpdatePinButtonVisual()
        {
            if (button100 == null || button100.IsDisposed)
                return;
            button100.BackColor = TopMost ? ModernUiTheme.AccentPressed : ModernUiTheme.HeaderBlue;
            button100.AccessibleDescription = TopMost ? "当前窗口已置顶" : "当前窗口未置顶";
        }

        /// <summary>
        /// 将工具窗体嵌入其它面板时隐藏基类自定义标题栏，避免标题按钮覆盖宿主布局。
        /// </summary>
        internal void SetEmbeddedMode(bool embedded)
        {
            panel1.Visible = !embedded;
            button100.Visible = !embedded;
            if (embedded)
            {
                FormBorderStyle = FormBorderStyle.None;
                Padding = Padding.Empty;
            }
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
        #region  窗体缩放
        private const int WM_NCHITTEST = 0x0084; //鼠标在窗体客户区（除标题栏和边框以外的部分）时发送的信息
        const int HTLEFT = 10;  //左变
        const int HTRIGHT = 11;  //右边
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;  //左上
        const int HTTOPRIGHT = 14; //右上
        const int HTBOTTOM = 15;  //下
        const int HTBOTTOMLEFT = 0x10;  //左下
        const int HTBOTTOMRIGHT = 17;  //右下
        System.Drawing.Point vPoint = System.Drawing.Point.Empty;
        //自定义边框拉伸
        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
                switch (m.Msg)
                {
                    case WM_NCHITTEST:
                        vPoint = new System.Drawing.Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                        vPoint = PointToClient(vPoint);
                        if (vPoint.X <= 5)
                            if (vPoint.Y <= 5)
                                m.Result = (IntPtr)HTTOPLEFT;  //左上
                            else if (vPoint.Y >= this.ClientSize.Height - 5)
                                m.Result = (IntPtr)HTBOTTOMLEFT; //左下
                            else
                                m.Result = (IntPtr)HTLEFT;  //左边
                        else if (vPoint.X >= this.ClientSize.Width - 5)
                            if (vPoint.Y <= 5)
                                m.Result = (IntPtr)HTTOPRIGHT;  //右上
                            else if (vPoint.Y >= this.ClientSize.Height - 5)
                                m.Result = (IntPtr)HTBOTTOMRIGHT;  //右下
                            else
                                m.Result = (IntPtr)HTRIGHT;  //右
                        else if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOP;  //上
                        else if (vPoint.Y >= this.ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOM; //下

                        else
                        {
                            base.WndProc(ref m);//如果去掉这一行代码,窗体将失去MouseMove..等事件
                            System.Drawing.Point lpint = new System.Drawing.Point((int)m.LParam);//可以得到鼠标坐标,这样就可以决定怎么处理这个消息了,是移动窗体,还是缩放,以及向哪向的缩放

                            m.Result = (IntPtr)0x2;//托动HTCAPTION=2 <0x2>
                        }
                        break;
                }
            }
            catch { }
        }
        #endregion
        /// <summary>
        /// 当前工具所属的流程
        /// </summary>
        internal string jobName = string.Empty;
        /// <summary>
        /// 当前工具名
        /// </summary>
        internal string toolName = string.Empty;


        private void Frm_ToolBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 弹窗（单例）永远不真正关闭：“×”/Alt+F4 只是隐藏，由 ShowDialog 返回后
            // 由调用处决定后续逻辑。e.Cancel 同时阻断“owned 窗体关闭→owner”的连锁，
            // 保证关闭动作只影响弹窗本身、绝不会连带关闭主窗体或退出进程。
            // 隐藏后主窗体回到前台的逻辑在 ShowTopMostDialog 的 RestoreFocusAfterModal 中。
            this.Hide();
            e.Cancel = true;
        }
        /// <summary>
        /// 通过流程名获取窗体句柄
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        internal Frm_ImageWindow GetImageWindowControl(string jobName)
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        if (Machine.machineRunStatu != MachineRunStatu.Running)
                        {
                            item.Value.Show();              //切换到当前图像窗体
                        }
                        return item.Value;
                    }
                }
                //////Frm_Main.Instance.OutputMsg(Project .Instance .configuration .language == Language.English ? "The process was successfully run,Elapsed：" : "此流程所绑定的窗体不存在，已自动更换为默认图像窗体", Color.Red);
                //////Job.GetJobByName(jobName).debugImageWindow = Frm_ImageWindow.Instance.Text;
                return Frm_ImageWindow.Instance;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            // 真最小化：小窗口收进任务栏、保持最小化状态，等用户点击任务栏上它的按钮再还原。
            // ShowTopMostDialog 已保证模态期间弹窗在任务栏上有独立入口（WS_EX_APPWINDOW），
            // 所以这里可以安全地最小化——不会再出现“弹窗消失无入口、主窗体又被模态锁死”的假死态。
            // 注意：最小化不会结束模态循环，弹窗被真正关闭（×）之前主窗体始终不可点击。
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.MinimumSize == this.MaximumSize && this.MinimumSize != new Size(0, 0))
            {
                button2.Enabled = false;
                return;
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            UpdateMaximizeButtonGlyph();
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            if (this.MinimumSize == this.MaximumSize && this.MinimumSize != new Size(0, 0))
                return;

            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            UpdateMaximizeButtonGlyph();
        }

        private void button4_Click(object sender, EventArgs e)
        {


        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (this.TopMost)
            {
                this.TopMost = false;
            }
            else
            {
                this.TopMost = true;
            }
            UpdatePinButtonVisual();
            lbl_title.Focus();
        }

        internal virtual void btn_baseClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
