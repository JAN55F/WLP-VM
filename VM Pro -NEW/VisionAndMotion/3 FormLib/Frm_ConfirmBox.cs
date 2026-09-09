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
    public partial class Frm_ConfirmBox : Frm_FormBase
    {
        public Frm_ConfirmBox()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_ConfirmBox _instance;
        public static Frm_ConfirmBox Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ConfirmBox();
                return _instance;
            }
        }
        /// <summary>
        /// 选择结果
        /// </summary>
        public ConfirmBoxResult Result = ConfirmBoxResult.Cancel;



        private void btn_cancel_Click(object sender, EventArgs e)
        {
            Result = ConfirmBoxResult.Cancel;
            this.Hide();
        }
        private void btn_confirm_Click(object sender, EventArgs e)
        {
            Result = ConfirmBoxResult.Yes;
            this.Hide();
        }
        private void Frm_ConfirmBox_Load(object sender, EventArgs e)
        {
            Result = ConfirmBoxResult.Cancel;
            Frm_ConfirmBox.Instance.TopMost = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result = ConfirmBoxResult.No;
            this.Hide();
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
    public enum ConfirmBoxResult
    {
        Cancel,
        Yes,
        No,
    }
}
