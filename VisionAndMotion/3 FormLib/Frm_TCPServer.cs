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
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_TCPServer();
                return _instance;
            }
        }

        internal static bool TryGetExistingInstance(out Frm_TCPServer form)
        {
            form = _instance;
            if (form == null || form.IsDisposed || form.Disposing)
            {
                form = null;
                return false;
            }
            return true;
        }

        internal static bool TryPost(TCPSever source, Action<Frm_TCPServer> update)
        {
            Frm_TCPServer form;
            if (source == null || update == null || !TryGetExistingInstance(out form) || !form.IsHandleCreated)
                return false;

            MethodInvoker apply = delegate
            {
                try
                {
                    Frm_TCPServer current;
                    if (!TryGetExistingInstance(out current) ||
                        !ReferenceEquals(current, form) ||
                        !ReferenceEquals(tcpSever, source))
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

        internal static void TryAppendLog(TCPSever source, string logLine)
        {
            TryPost(source, form =>
            {
                if (form.Visible && (form.TopLevel || form.Parent != null))
                    form.tbx_log.AppendText(logLine);
            });
        }

        internal static void TryApplyListeningState(TCPSever source, bool isListening)
        {
            TryPost(source, form =>
            {
                form.btn_listen.TextStr = isListening ? "停止监听" : "开始监听";
                if (!isListening)
                {
                    form.lbx_connectedList.Items.Clear();
                    form.cbx_connectedList.Clear();
                }
            });
        }

        internal static void TryRefreshConnectedClients(TCPSever source)
        {
            TryPost(source, form => form.RefreshConnectedClients(source));
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

                RefreshConnectedClients(tcpSever);
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

        private void RefreshConnectedClients(TCPSever source)
        {
            lbx_connectedList.Items.Clear();
            cbx_connectedList.Clear();

            if (source == null)
                return;

            string[] clientNames = source.GetConnectedClientNamesSnapshot();
            for (int i = 0; i < clientNames.Length; i++)
            {
                lbx_connectedList.Items.Add(clientNames[i]);
                cbx_connectedList.Add(clientNames[i]);
            }

            if (cbx_connectedList.Items.Length > 0)
                cbx_connectedList.SelectedIndex = 0;
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
                if (tcpSever == null || lbx_connectedList.SelectedItem == null)
                    return;

                if (tcpSever.TryDisconnectClient(lbx_connectedList.SelectedItem.ToString()))
                    RefreshConnectedClients(tcpSever);
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
