using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMPro
{
    [Serializable]
    internal class TCPClient
    {
        internal TCPClient(string name)
        {
            this.Name = name;
            EnsureRuntimeSocket(name);
        }

        /// <summary>
        /// 接收到的消息
        /// </summary>
        internal volatile string receivedStr = string.Empty;
        /// <summary>
        /// 客户端名称
        /// </summary>
        internal string Name = string.Empty;
        /// <summary>
        /// 服务器IP地址
        /// </summary>
        internal string severIP = "192.168.0.1";
        /// <summary>
        /// 服务器端口号
        /// </summary>
        internal Int32 severPort = 10004;
        /// <summary>  
        /// Socket集合  因Socket类不能被序列化，所以声明一个静态的集合来存储    键：通讯设备名   值：Socket对象
        /// </summary>
        internal static Dictionary<string, Socket> L_socket = new Dictionary<string, Socket>();
        /// <summary>
        /// 程序开启后自动连接服务器
        /// </summary>
        public bool AutoConnectAfterStart = true;
        /// <summary>
        /// 程序关闭前自动断开服务器
        /// </summary>
        public bool AutoDisconnectBeforeClose = true;
        /// <summary>
        /// 断开时是否自动连接
        /// </summary>
        public bool AutoConnect = true;

        internal void EnsureRuntime()
        {
            EnsureRuntimeSocket(Name);
        }

        private static void EnsureRuntimeSocket(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (!L_socket.ContainsKey(name))
                L_socket.Add(name, new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
        }

        internal void Rename(string newName)
        {
            try
            {
                if (string.IsNullOrEmpty(newName) || Name == newName)
                    return;

                Socket socket = null;
                if (L_socket.ContainsKey(Name))
                {
                    socket = L_socket[Name];
                    L_socket.Remove(Name);
                }

                Name = newName;
                if (!L_socket.ContainsKey(Name))
                    L_socket.Add(Name, socket ?? new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 通过通讯设备名来查找对应的Socket对象
        /// </summary>
        /// <param name="name">通讯设备名</param>
        /// <returns></returns>
        internal Socket FindSocketByName()
        {
            try
            {
                EnsureRuntime();
                foreach (KeyValuePair<string, Socket> item in L_socket)
                {
                    if (item.Key == Name)
                        return item.Value;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 连接服务端
        /// </summary>
        internal void Connect()
        {
            Connect(2000, true);
        }

        /// <summary>
        /// 连接服务端，使用超时避免界面卡死
        /// </summary>
        internal bool Connect(int timeoutMs, bool showTip)
        {
            try
            {
                EnsureRuntime();
                for (int i = 0; i < L_socket.Count; i++)
                {
                    if (L_socket.Keys.ToArray()[i] == Name)
                        L_socket[L_socket.Keys.ToArray()[i]] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                IPAddress ip;
                try
                {
                    ip = IPAddress.Parse(severIP);
                }
                catch
                {
                    ShowConnectMessage("\r\nIP地址有误或IP不存在，连接失败，请检查");
                    return false;
                }
                IPEndPoint point = new IPEndPoint(ip, severPort);
                try
                {
                    Socket socket = FindSocketByName();
                    if (socket == null)
                        return false;

                    IAsyncResult result = socket.BeginConnect(point, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(timeoutMs, true);
                    if (!success)
                    {
                        try { socket.Close(); } catch { }
                        if (showTip)
                            ShowConnectMessage(string.Format("\r\n客户端 [{0}] 连接失败：服务端未监听或网络超时", Name));
                        return false;
                    }
                    socket.EndConnect(result);
                }
                catch (Exception ex)
                {
                    if (showTip)
                        ShowConnectMessage(string.Format("\r\n客户端 [{0}] 连接失败：{1}", Name, ex.Message));
                    return false;
                }
                if (FindSocketByName().Connected)
                {
                    Thread th_recieve = new Thread(Recieve);
                    th_recieve.IsBackground = true;
                    th_recieve.Start();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }

        private void ShowConnectMessage(string message)
        {
            try
            {
                if (Frm_Main.Instance.InvokeRequired)
                    Frm_Main.Instance.BeginInvoke(new Action(() => Frm_MessageBox.Instance.MessageBoxShow(message)));
                else
                    Frm_MessageBox.Instance.MessageBoxShow(message);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息</param>
        internal void Send(string msg)
        {
            try
            {
                if (Frm_TCPClient.Instance.Visible)
                {
                    string curTime = DateTime.Now.ToString("HH:mm:ss");
                    Frm_TCPClient.Instance.tbx_log.AppendText(curTime + "<-  : " + msg + "\r\n");
                }
                byte[] buffer = Encoding.Default.GetBytes(msg);
                FindSocketByName().Send(buffer);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 接收一次消息
        /// </summary>
        internal string RecieveOnce()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int length = 0;
                try
                {
                    length = FindSocketByName().Receive(buffer);
                }
                catch { }
                string result = Encoding.Default.GetString(buffer, 0, length);
                if (length > 0)
                {
                    if (Frm_TCPClient.Instance.Visible)
                    {
                        string curTime = DateTime.Now.ToString("HH:mm:ss");
                        Frm_TCPClient.Instance.tbx_log.AppendText(curTime + "->  : " + result + "\r\n");
                    }
                    return result;
                }
                else
                {
                    //连接断开
                    return string.Empty;
                }

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        private void Recieve()
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int length = 0;
                    try
                    {
                        length = FindSocketByName().Receive(buffer);
                    }
                    catch { }
                    string result = Encoding.Default.GetString(buffer, 0, length);
                    if (length > 0)
                    {
                        string logLine = DateTime.Now.ToString("HH:mm:ss") + "->  : " + result + "\r\n";
                        Frm_Main.Instance.BeginInvoke(new Action(() =>
                        {
                            if (Frm_TCPClient.Instance.Visible)
                                Frm_TCPClient.Instance.tbx_log.AppendText(logLine);
                        }));
                        receivedStr = result;
                    }
                    else
                    {
                        if (FindSocketByName() != null)
                        {
                            try
                            {
                                FindSocketByName().Disconnect(false);
                                FindSocketByName().Close();

                            }
                            catch { }
                        }

                        string localName1 = Name;
                        Frm_Main.Instance.BeginInvoke(new Action(() =>
                        {
                            if (Frm_TCPClient.Instance.Visible)
                            {
                                if (Frm_DeviceManager.Instance.dgv_deviceList.SelectedRows.Count > 0 &&
                                    Frm_DeviceManager.Instance.dgv_deviceList.SelectedRows[0].Cells[0].Value.ToString() == localName1)
                                {
                                    Frm_TCPClient.Instance.btn_connect.TextStr = "连接";
                                    Frm_DeviceManager.Instance.lbl_tip.Text = "连接已断开";
                                }
                            }
                        }));
                        Frm_Main.Instance.OutputMsg("服务器连接已中断，已启动自动重连...", Color.Red);

                        if (!AutoConnect)
                            return;

                        while (FindSocketByName() == null || !FindSocketByName().Connected)
                        {
                            if (Connect(2000, false))
                                break;
                            Thread.Sleep(1000);
                        }

                        string localName2 = Name;
                        Frm_Main.Instance.BeginInvoke(new Action(() =>
                        {
                            if (Frm_TCPClient.Instance.Visible)
                            {
                                if (Frm_DeviceManager.Instance.dgv_deviceList.SelectedRows.Count > 0 &&
                                    Frm_DeviceManager.Instance.dgv_deviceList.SelectedRows[0].Cells[0].Value.ToString() == localName2)
                                {
                                    Frm_TCPClient.Instance.btn_connect.TextStr = "断开";
                                    Frm_DeviceManager.Instance.lbl_tip.Text = "连接成功";
                                }
                            }
                        }));
                        //////});
                        //////th.IsBackground = true;
                        //////th.Start();

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        internal void Close()
        {
            try
            {
                if (FindSocketByName().Connected)
                    FindSocketByName().Disconnect(false);
                FindSocketByName().Close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
