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

        internal Frm_FormBase()
        {
            InitializeComponent();
            this.MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            this.TopMost = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AlignTitleButtons();
            titleButtonsAligned = true;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (titleButtonsAligned)
                AlignTitleButtons();
        }

        public new DialogResult ShowDialog()
        {
            return ShowTopMostDialog();
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            return ShowTopMostDialog();
        }

        private DialogResult ShowTopMostDialog()
        {
            bool oldTopMost = this.TopMost;
            bool oldShowInTaskbar = this.ShowInTaskbar;
            this.TopMost = true;
            this.ShowInTaskbar = false;

            using (Form topMostOwner = CreateTopMostOwner())
            {
                try
                {
                    topMostOwner.Show();
                    topMostOwner.BringToFront();
                    this.BringToFront();
                    this.Activate();

                    return base.ShowDialog(topMostOwner);
                }
                finally
                {
                    this.TopMost = oldTopMost;
                    this.ShowInTaskbar = oldShowInTaskbar;
                }
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
            const int buttonSize = 25;

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

            button100.Size = new Size(buttonSize, buttonSize);
            button1.Size = new Size(buttonSize, buttonSize);
            button2.Size = new Size(buttonSize, buttonSize);
            btn_baseClose.Size = new Size(buttonSize, buttonSize);

            btn_baseClose.Location = new Point(panel1.Width - buttonSize, 0);
            button2.Location = new Point(panel1.Width - buttonSize * 2, 0);
            button1.Location = new Point(panel1.Width - buttonSize * 3, 0);
            button100.Location = new Point(panel1.Width - buttonSize * 4, 0);
            button100.BringToFront();
            button1.BringToFront();
            button2.BringToFront();
            btn_baseClose.BringToFront();
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
                button2.BackgroundImage = Resources.Min;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                button2.BackgroundImage = Resources.Max;
            }
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            if (this.MinimumSize == this.MaximumSize && this.MinimumSize != new Size(0, 0))
                return;

            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                button2.BackgroundImage = Resources.Min;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                button2.BackgroundImage = Resources.Max;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {


        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (this.TopMost)
            {
                this.TopMost = false;
                button100.Image = Resources.unTopLevel;
            }
            else
            {
                this.TopMost = true;
                button100.Image = Resources.钉;
            }
            lbl_title.Focus();
        }

        internal virtual void btn_baseClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
