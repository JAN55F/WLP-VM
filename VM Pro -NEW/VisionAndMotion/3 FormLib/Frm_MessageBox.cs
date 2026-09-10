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
    public partial class Frm_MessageBox : Frm_FormBase
    {
        internal Frm_MessageBox()
        {
            InitializeComponent();
            Init_Language();
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

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_MessageBox _instance;
        public static Frm_MessageBox Instance
        {
            get
            {
                _instance = new Frm_MessageBox();
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
                if (Project .Instance .configuration .language == Language.English)
                {
                    btn_confim.Text = "Confirm";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 弹框
        /// </summary>
        /// <param name="msg">要显示的信息</param>
        public  void MessageBoxShow(string msg, TipType tipType = TipType .Tip)
        {
            this.lbl_info.Text = msg;
            this.TopMost = true;
            switch (tipType)
            {
                case TipType.Tip:
                    pictureBox1.Image = Resources.Tip;
                    lbl_title.Text = "提示";
                    break;
                case TipType.Warn:
                    pictureBox1.Image = Resources.Warn;
                    lbl_title.Text = "警告";
                    break;
                case TipType.Error:
                    pictureBox1.Image = Resources.Error;
                    lbl_title.Text = "错误";
                    break;
            }
            Application.DoEvents();
            this.ShowDialog();
        }


        private void btn_confim_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void Frm_MessageBox_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.TopLevel = true;
        }

        private void pic_tipType_Click(object sender, EventArgs e)
        {

        }

        private void newButton2_Click(object sender, EventArgs e)
        {

        }

        private void btn_confim_MouseDown(object sender, MouseEventArgs e)
        {
            btn_confim.BackgroundImage = Resources.ButtonDown;
            Application.DoEvents();
        }

        private void btn_confim_MouseUp(object sender, MouseEventArgs e)
        {
            btn_confim.BackgroundImage = Resources.ButtonUp ;
            Application.DoEvents();
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

    }
}
