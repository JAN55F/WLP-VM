using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;
using Tool;

namespace VMPro
{
    public partial class Frm_Regiest : Frm_FormBase 
    {
        public Frm_Regiest()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本机的机器码
        /// </summary>
        private string machineCode;
        /// <summary>
        /// 读取本地存储的注册码
        /// </summary>
        private Ini ini = new Ini(Application.StartupPath + "\\Config.ini");
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_Regiest _instance;
        public static Frm_Regiest Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_Regiest();
                return _instance;
            }
        }
      

        private void lnl_author_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Frm_InputMessage inputMessage = new Frm_InputMessage();
            inputMessage.lbl_title .Text = "请输入密码";
            inputMessage.passwordChar = true;
            inputMessage.ShowDialog(this);
            if (inputMessage.txt_input.TextStr == "likang")
            {
                Ini ini = new Ini(Application.StartupPath + "\\Config.ini");
                string machineCode =Regiest. Get_MNum();
                string RegiestCode =Regiest. Get_RNum(machineCode);
                ini.IniWriteValue("Regiest", "RegiestCode", RegiestCode);
                this.Close();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Frm_Regiest_Load(object sender, EventArgs e)
        {
            machineCode =Regiest . Get_MNum();
            txt_machineCode.Text = machineCode;
        }
        private void btn_regiest_Click(object sender, EventArgs e)
        {

            string machineCode =Regiest . Get_MNum();
            string RegiestCode =Regiest. Get_RNum(machineCode);
            if (txt_regiestCode.Text == RegiestCode)
            {
                ini.IniWriteValue("Regiest", "RegiestCode", txt_regiestCode.Text);
            }
            else
            {
                txt_regiestCode.Clear();
                Frm_MessageBox messageBox = new Frm_MessageBox();
                messageBox.MessageBoxShow("The registration code is incorrect. Please enter it again");
                txt_regiestCode.Focus();
            }
        }
        
    }
}
