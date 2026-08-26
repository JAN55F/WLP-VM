using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tool;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_Lock : Form
    {
        public Frm_Lock()
        {
            InitializeComponent();
        }

        #region 窗体拖动
        private static bool IsDrag = false;
        private int enterX;
        private int enterY;
        private void setForm_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                IsDrag = true;
                enterX = e.Location.X;
                enterY = e.Location.Y;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void setForm_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                IsDrag = false;
                enterX = 0;
                enterY = 0;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void setForm_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (IsDrag)
                {
                    Left += e.Location.X - enterX;
                    Top += e.Location.Y - enterY;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        #endregion

        /// <summary>
        /// X向自动增加
        /// </summary>
        private bool Xadd = true;
        /// <summary>
        /// Y向自动增加
        /// </summary>
        private bool Yadd = true;
        /// <summary>
        /// 窗体是否移动
        /// </summary>
        private bool isMove = true;
        /// <summary>
        /// 一段时间不操作则自动恢复飘动
        /// </summary>
        private int waitTime = 0;
        /// <summary>
        /// 是否停止刷新
        /// </summary>
        private bool stopUpdata = false;


        private void Frm_Lock_Click(object sender, EventArgs e)
        {
            try
            {
                //if (isMove)
                //{
                    isMove = false;
                    this.Opacity = 1;
                    this.tbx_password.Focus();
                //}
                //else
                //{
                //    isMove = true;
                //    this.Opacity = 0.4;
                //}
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_Lock_Load(object sender, EventArgs e)
        {
            try
            {
                tbx_password.Select();
                //this.TopMost = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void UpdataStatu(object o)
        {
            try
            {
                while (!stopUpdata)
                {
                    tbx_password.Focus();
                    Application.DoEvents();
                    if (!isMove)
                    {
                        waitTime++;
                        if (waitTime > 5000000)
                        {
                            isMove = true;
                            waitTime = 0;
                        }
                        continue;
                    }

                    if (Xadd)
                        this.Location = new System.Drawing.Point(this.Location.X + 1, this.Location.Y);
                    if (Yadd)
                        this.Location = new System.Drawing.Point(this.Location.X, this.Location.Y + 1);
                    if (!Xadd)
                        this.Location = new System.Drawing.Point(this.Location.X - 1, this.Location.Y);
                    if (!Yadd)
                        this.Location = new System.Drawing.Point(this.Location.X, this.Location.Y - 1);

                    if (this.Location.X >= Frm_Main.Instance.Location.X + Frm_Main.Instance.Width - this.Width - 10)
                        Xadd = false;
                    if (this.Location.Y >= Frm_Main.Instance.Location.Y + Frm_Main.Instance.Height - this.Height - 10)
                        Yadd = false;
                    if (this.Location.X <= Frm_Main.Instance.Location.X + 10)
                        Xadd = true;
                    if (this.Location.Y <= Frm_Main.Instance.Location.Y)
                        Yadd = true;

                    Application.DoEvents();
                    Thread.Sleep(15);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tbx_password_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (isMove)
                {
                    isMove = false;
                    this.Opacity = 1;
                    this.tbx_password.Focus();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_unlock_Click(object sender, EventArgs e)
        {
           
        }
        private void tbx_password_Click(object sender, EventArgs e)
        {
            try
            {
                if (isMove)
                {
                    isMove = false;
                    this.Opacity = 1;
                    this.tbx_password.Focus();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void Frm_Lock_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopUpdata = true;
        }

        private void Frm_Lock_Shown(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(UpdataStatu);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Method.GetMD5(tbx_password.TextStr.Trim()) == Project.Instance.configuration.adminPassword || Method.GetMD5(tbx_password.Text.Trim()) == Project.Instance.configuration.developerPassword)
                {
                    Frm_Main.locked = false;
                    Frm_Main.Instance.toolStripButton32.Image = Resources.锁定;
                    this.Close();
                    //////GetImageWindowControl().hwc_imageWindow.ContextMenuStrip = Frm_ImageWindow.Instance.cnt_rightClickMenu;
                    //////Frm_Main.Instance.buttonItem83.Checked = false;

                }
                else
                {
                    tbx_password.TextStr = string.Empty;
                    tbx_password.DefaultText  = "密码错误，请重新输入";
                    tbx_password.TextStr = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tbx_password_TextStrChanged(string textStr)
        {
            isMove = false;
            this.Opacity = 1;
        }

    }
}
