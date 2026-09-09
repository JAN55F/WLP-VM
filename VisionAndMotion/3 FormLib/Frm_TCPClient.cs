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
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_TCPClient();
                return _instance;
            }
        }

        internal static bool TryGetExistingInstance(out Frm_TCPClient form)
        {
            form = _instance;
            if (form == null || form.IsDisposed || form.Disposing)
            {
                form = null;
                return false;
            }
            return true;
        }

        internal static bool TryPost(TCPClient source, Action<Frm_TCPClient> update)
        {
            Frm_TCPClient form;
            if (source == null || update == null || !TryGetExistingInstance(out form) || !form.IsHandleCreated)
                return false;

            MethodInvoker apply = delegate
            {
                try
                {
                    Frm_TCPClient current;
                    if (!TryGetExistingInstance(out current) ||
                        !ReferenceEquals(current, form) ||
                        !ReferenceEquals(tcpClient, source))
                        return;

                    update(current);
                }
                catch (Exception ex)
                {
                    Log.SaveError(ex);
                }
            };

            try
            {
                if (form.InvokeRequired)
                    form.BeginInvoke(apply);
                else
                    apply();
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        internal static void TryAppendLog(TCPClient source, string logLine)
        {
            TryPost(source, form =>
            {
                if (form.Visible && (form.TopLevel || form.Parent != null))
                    form.tbx_log.AppendText(logLine);
            });
        }

        internal static void TryApplyConnectionState(TCPClient source, bool connected)
        {
            TryPost(source, form =>
            {
                form.btn_connect.Enabled = true;
                form.btn_connect.TextStr = connected ? "断开" : "连接";
            });
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
                if (tcpClient.connecting)
                {
                    btn_connect.Enabled = false;
                    btn_connect.TextStr = "连接中...";
                }
                else if (socket != null && socket.Connected)
                {
                    btn_connect.Enabled = true;
                    btn_connect.TextStr = "断开";
                }
                else
                {
                    btn_connect.Enabled = true;
                    btn_connect.TextStr = "连接";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void tbx_clientName_TextStrChanged(string textStr)
        {
            if (tcpClient == null)
                return;

            string requestedName = tbx_clientName.TextStr.Trim();
            if (!tcpClient.Rename(requestedName) && tbx_clientName.TextStr != tcpClient.Name)
                tbx_clientName.TextStr = tcpClient.Name;
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

                TCPClient target = tcpClient;

                Socket socket = target.FindSocketByName();
                if (socket != null && socket.Connected)
                {
                    target.Close();
                    btn_connect.TextStr = "连接";
                    Frm_DeviceManager.TrySetTipForDevice("TCPClient", target.Name, "TCP客户端已断开", Color.FromArgb(52, 64, 84));
                    return;
                }

                btn_connect.TextStr = "连接中...";
                btn_connect.Enabled = false;
                target.connecting = true;
                int generation = target.BeginExplicitConnect();
                try
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        bool connected = false;
                        try
                        {
                            connected = target.ConnectPrepared(2000, true, generation);
                        }
                        finally
                        {
                            target.connecting = false;
                            TryApplyConnectionState(target, connected);
                            Frm_DeviceManager.TrySetTipForDevice("TCPClient", target.Name,
                                connected ? "TCP客户端连接成功" : "TCP客户端连接失败",
                                connected ? Color.Green : Color.Red);
                        }
                    });
                }
                catch
                {
                    target.connecting = false;
                    throw;
                }
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
                Socket socket = tcpClient == null ? null : tcpClient.FindSocketByName();
                if (socket == null || !socket.Connected)
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

