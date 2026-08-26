using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tool;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_TCPServer : Form
    {
        public Frm_TCPServer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 服务器对象
        /// </summary>
        private static TCPSever tcpSever;
        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_TCPServer _instance;
        public static Frm_TCPServer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_TCPServer();
                return _instance;
            }
        }


        /// <summary>
        /// 加载参数到界面
        /// </summary>
        /// <param name="tcpClient_1"></param>
        internal void LoadPar(TCPSever tcpSever_1)
        {
            try
            {
                tcpSever = tcpSever_1;
                tbx_severName.TextStr = tcpSever.Name;
                tbx_severName.Enabled = tcpSever.Name != "服务端1";
                tbx_severIP.TextStr = tcpSever.SeverIP;
                tbx_severPort.TextStr = tcpSever.SeverPort.ToString();
                ckb_autoConnectAfterStart.Checked = tcpSever.AutoListenAfterStart;
                ckb_autoDisconnectBeforeClose.Checked = tcpSever.AutoDisconnectBeforeClose;

                if (tcpSever.listened)
                    btn_listen.TextStr = "停止监听";
                else
                    btn_listen.TextStr = "开始监听";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void tbx_severName_TextStrChanged(string textStr)
        {
            if (tcpSever != null && tcpSever.Name == "服务端1" && tbx_severName.TextStr.Trim() != "服务端1")
            {
                tbx_severName.TextStr = "服务端1";
                return;
            }
            if (tcpSever != null)
                tcpSever.Rename(tbx_severName.TextStr.Trim());
        }
        private void tbx_severIP_TextStrChanged(string textStr)
        {
            tcpSever.SeverIP = tbx_severIP.TextStr.Trim();
        }
        private void tbx_severPort_TextStrChanged(string textStr)
        {
            tcpSever.SeverPort = Convert.ToInt16(tbx_severPort.TextStr.Trim());
        }
        private void btn_listen_Clicked()
        {
            tcpSever.Listen();
        }
        private void ckb_autoConnectAfterStart_CheckChanged(bool Checked)
        {
            tcpSever.AutoListenAfterStart = Checked;
        }
        private void ckb_autoDisconnectBeforeClose_CheckChanged(bool Checked)
        {
            tcpSever.AutoDisconnectBeforeClose = Checked;
        }
        private void 断开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < TCPSever.L_STCPSever.Count; i++)
                {
                    if (TCPSever.L_STCPSever[i].severName == tcpSever.Name)
                    {
                        foreach (KeyValuePair<string, Socket> item in TCPSever.L_STCPSever[i].L_Client)
                        {
                            if (item.Key == lbx_connectedList.SelectedItem.ToString())
                            {
                                item.Value.Disconnect(false);
                                item.Value.Close();
                                TCPSever.L_STCPSever[i].L_Client.Remove(item.Key);

                                lbx_connectedList.Items.RemoveAt(lbx_connectedList.SelectedIndex);
                                //   cbx_connectedList .Items .remove      //移除，待完善
                                if (cbx_connectedList.Items.Length > 0)
                                    cbx_connectedList.SelectedIndex = 0;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void lnk_clearLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tbx_log.Clear();
        }
        private void btn_send_Clicked()
        {
            try
            {
                if (tbx_sendMessage.TextStr.Trim() == string.Empty)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n不可发送空字符串，发送失败 ");
                    return;
                }
                tcpSever.Send(cbx_connectedList.TextStr, tbx_sendMessage.TextStr.Trim());
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Frm_TCPServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
     
    }
}
