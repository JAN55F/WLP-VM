using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Tool;
using System.Diagnostics;

namespace VMPro
{
    internal partial class Frm_TCPClient : Form
    {
        internal Frm_TCPClient()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 客户端对象
        /// </summary>
        private static TCPClient tcpClient;
        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_TCPClient _instance;
        internal static Frm_TCPClient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_TCPClient();
                return _instance;
            }
        }


        /// <summary>
        /// 加载参数到界面
        /// </summary>
        /// <param name="tcpClient_1"></param>
        internal void LoadPar(TCPClient tcpClient_1)
        {
            try
            {
                tcpClient = tcpClient_1;
                tbx_clientName.TextStr = tcpClient.Name;
                tbx_severIP.TextStr = tcpClient.severIP;
                tbx_severPort.TextStr = tcpClient.severPort.ToString();
                ckb_autoConnectAfterStart.Checked = tcpClient.AutoConnectAfterStart;
                ckb_autoDisconnectBeforeClose.Checked = tcpClient.AutoDisconnectBeforeClose;

                Socket socket = tcpClient.FindSocketByName();
                if (socket != null && socket.Connected)
                    btn_connect.TextStr = "断开";
                else
                    btn_connect.TextStr = "连接";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void tbx_clientName_TextStrChanged(string textStr)
        {
            if (tcpClient != null)
                tcpClient.Rename(tbx_clientName.TextStr.Trim());
        }
        private void ckb_autoConnectAfterStart_CheckChanged(bool Checked)
        {
            if (tcpClient != null)
                tcpClient.AutoConnectAfterStart = Checked;
        }
        private void ckb_autoDisconnectBeforeClose_CheckChanged(bool Checked)
        {
            if (tcpClient != null)
                tcpClient.AutoDisconnectBeforeClose = Checked;
        }
        private void tbx_severIP_TextStrChanged(string textStr)
        {
            tcpClient.severIP = tbx_severIP.TextStr.Trim();
        }
        private void tbx_port_TextStrChanged(string textStr)
        {
            tcpClient.severPort = Convert.ToInt32(tbx_severPort.TextStr.Trim());
        }
        private void btn_connect_Clicked()
        {
            try
            {
                if (tcpClient == null)
                    return;

                Socket socket = tcpClient.FindSocketByName();
                if (socket != null && socket.Connected)
                {
                    tcpClient.Close();
                    btn_connect.TextStr = "连接";
                    Frm_DeviceManager.Instance.lbl_tip.Text = "TCP客户端已断开";
                    return;
                }

                btn_connect.TextStr = "连接中...";
                btn_connect.Enabled = false;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    bool connected = tcpClient.Connect(2000, true);
                    try
                    {
                        BeginInvoke(new Action(() =>
                        {
                            btn_connect.Enabled = true;
                            btn_connect.TextStr = connected ? "断开" : "连接";
                            Frm_DeviceManager.Instance.lbl_tip.Text = connected ? "TCP客户端连接成功" : "TCP客户端连接失败";
                        }));
                    }
                    catch { }
                });
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                btn_connect.Enabled = true;
                btn_connect.TextStr = "连接";
            }
        }
        private void btn_send_Clicked()
        {
            try
            {
                if (!tcpClient.FindSocketByName().Connected)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n未连接到服务端，发送失败");
                    return;
                }
                if (tbx_sendMessage.TextStr.Trim() == string.Empty)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n不可发送空字符串，发送失败 ");
                    return;
                }
                tcpClient.Send(tbx_sendMessage.TextStr.Trim());
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
        private void Frm_TCPClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

    }
}

