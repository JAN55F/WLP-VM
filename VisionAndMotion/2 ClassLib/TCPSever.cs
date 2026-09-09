using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VMPro
{
    [Serializable]
    internal class TCPSever
    {
        internal TCPSever(string severName)
        {
            this.Name = severName;
            EnsureRuntimeItem(severName);
        }

        private static STCPSever CreateRuntimeItem(string severName)
        {
            STCPSever stcpSever = new STCPSever();
            stcpSever.severName = severName;
            stcpSever.SeverObj = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            stcpSever.L_Client = new Dictionary<string, Socket>();
            return stcpSever;
        }

        /// <summary>
        /// 是否已监听
        /// </summary>
        internal bool listened = false;
        /// <summary>
        /// 接收到的消息
        /// </summary>
        internal volatile string receivedStr = string.Empty;
        /// <summary>
        /// 服务器名称
        /// </summary>
        internal string Name = string.Empty;
        /// <summary>
        /// 服务器IP地址（0.0.0.0 表示监听本机所有网卡）
        /// </summary>
        internal string SeverIP = "0.0.0.0";
        /// <summary>
        /// 服务器端口号
        /// </summary>
        internal Int32 SeverPort = 10004;
        /// <summary>  
        /// Socket集合  因Socket类不能被序列化，所以声明一个静态的集合来存储    键：通讯设备名   值：Socket对象
        /// </summary>
        internal static List<STCPSever> L_STCPSever = new List<STCPSever>();
        /// <summary>
        /// 程序开启后自动监听
        /// </summary>
        public bool AutoListenAfterStart = true;
        /// <summary>
        /// 程序关闭前自动断开服务器
        /// </summary>
        public bool AutoDisconnectBeforeClose = true;

        internal void EnsureRuntime()
        {
            EnsureRuntimeItem(Name);
        }

        private static void EnsureRuntimeItem(string severName)
        {
            if (string.IsNullOrEmpty(severName))
                return;

            for (int i = 0; i < L_STCPSever.Count; i++)
            {
                if (L_STCPSever[i].severName == severName)
                    return;
            }
            L_STCPSever.Add(CreateRuntimeItem(severName));
        }

        internal void Rename(string newName)
        {
            try
            {
                if (string.IsNullOrEmpty(newName) || Name == newName)
                    return;

                string oldName = Name;
                for (int i = 0; i < L_STCPSever.Count; i++)
                {
                    if (L_STCPSever[i].severName == oldName)
                    {
                        STCPSever stcpSever = L_STCPSever[i];
                        stcpSever.severName = newName;
                        L_STCPSever[i] = stcpSever;
                        Name = newName;
                        return;
                    }
                }

                Name = newName;
                EnsureRuntime();
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
                for (int i = 0; i < L_STCPSever.Count; i++)
                {
                    if (L_STCPSever[i].severName == Name)
                        return L_STCPSever[i].SeverObj;
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
        /// 监听
        /// </summary>
        internal void Listen()
        {
            Listen(true);
        }

        internal void Listen(bool showFailureMessage)
        {
            try
            {
                EnsureRuntime();
                if (listened)
                {
                    Close();
                    return;
                }

                Thread th = new Thread(() =>
                {
                    for (int i = 0; i < L_STCPSever.Count; i++)
                    {
                        if (L_STCPSever[i].severName == Name)
                        {
                            try
                            {
                                IPAddress ip = IPAddress.Parse(SeverIP);
                                IPEndPoint point = new IPEndPoint(ip, SeverPort);

                                L_STCPSever[i].SeverObj.Bind(point);
                                L_STCPSever[i].SeverObj.Listen(10);
                                listened = true;
                                Frm_Main.Instance.BeginInvoke(new Action(() =>
                                {
                                    Frm_TCPServer.Instance.btn_listen.TextStr = "停止监听";
                                    Frm_DeviceManager.Instance.lbl_tip.Text = "TCP服务端已开始监听";
                                }));
                            }
                            catch (Exception ex)
                            {
                                listened = false;
                                Log.SaveError(ex);
                                if (showFailureMessage)
                                {
                                    Frm_Main.Instance.BeginInvoke(new Action(() =>
                                    {
                                        Frm_MessageBox.Instance.MessageBoxShow("\r\n服务端监听失败：" + ex.Message + "\r\n\r\n请检查IP地址和端口是否被占用");
                                        Frm_TCPServer.Instance.btn_listen.TextStr = "开始监听";
                                    }));
                                }
                                else
                                {
                                    try
                                    {
                                        Frm_Main.Instance.OutputMsg(string.Format("TCP服务端 [{0}] 启动监听失败：{1}，请进入界面后手动监听", Name, ex.Message), Color.Red);
                                    }
                                    catch { }
                                }
                                return;
                            }

                            while (true)
                            {
                                Socket socket;
                                try
                                {
                                    socket = L_STCPSever[i].SeverObj.Accept();
                                }
                                catch
                                {
                                    // 服务器Socket已关闭，退出监听循环
                                    break;
                                }
                                Thread th_receive = new Thread(Recieve);
                                th_receive.IsBackground = true;
                                th_receive.Start(socket);

                                string remoteEndPoint = socket.RemoteEndPoint.ToString();
                                L_STCPSever[i].L_Client.Add(remoteEndPoint, socket);
                                Frm_Main.Instance.OutputMsg(string.Format("客户端已连接，信息: {0}", remoteEndPoint), Color.Green);
                                Frm_Main.Instance.BeginInvoke(new Action(() =>
                                {
                                    Frm_TCPServer.Instance.lbx_connectedList.Items.Add(remoteEndPoint);
                                    Frm_TCPServer.Instance.cbx_connectedList.Add(remoteEndPoint);
                                    if (Frm_TCPServer.Instance.cbx_connectedList.Items.Length > 0)
                                        Frm_TCPServer.Instance.cbx_connectedList.SelectedIndex = 0;
                                }));
                            }
                            listened = false;
                            Frm_Main.Instance.BeginInvoke(new Action(() =>
                            {
                                Frm_TCPServer.Instance.btn_listen.TextStr = "开始监听";
                            }));
                        }
                    }
                });
                th.IsBackground = true;
                th.Start();
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
        internal void Send(string clientStr, string msg)
        {
            try
            {
                if (Frm_TCPServer.Instance.Visible)
                {
                    string curTime = DateTime.Now.ToString("HH:mm:ss");
                    Frm_TCPServer.Instance.tbx_log.AppendText(curTime + "<-  : " + msg + "\r\n");
                }
                byte[] buffer = Encoding.Default.GetBytes(msg);
                for (int i = 0; i < L_STCPSever.Count; i++)
                {
                    if (L_STCPSever[i].severName == Name)
                    {
                        foreach (KeyValuePair<string, Socket> item in L_STCPSever[i].L_Client)
                        {
                            if (item.Key == clientStr)
                                item.Value.Send(buffer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 向当前第一个已连接的客户端发送消息（供流程工具使用）。返回是否发送成功。
        /// </summary>
        /// <param name="msg">消息内容</param>
        internal bool SendToFirstClient(string msg)
        {
            try
            {
                if (Frm_TCPServer.Instance.Visible)
                {
                    string curTime = DateTime.Now.ToString("HH:mm:ss");
                    Frm_TCPServer.Instance.tbx_log.AppendText(curTime + "<-  : " + msg + "\r\n");
                }
                for (int i = 0; i < L_STCPSever.Count; i++)
                {
                    if (L_STCPSever[i].severName != Name)
                        continue;
                    foreach (KeyValuePair<string, Socket> item in L_STCPSever[i].L_Client)
                    {
                        if (item.Value != null && item.Value.Connected)
                        {
                            byte[] buffer = Encoding.Default.GetBytes(msg);
                            item.Value.Send(buffer);
                            return true;
                        }
                    }
                    return false;       //没有已连接的客户端
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 接收一次消息
        /// </summary>
        internal string RecieveOnce(object obj)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int length = 0;
                try
                {
                    length = ((Socket)obj).Receive(buffer);
                }
                catch { }
                string result = Encoding.Default.GetString(buffer, 0, length);
                if (length > 0)
                {
                    if (Frm_TCPServer.Instance.Visible)
                    {
                        string curTime = DateTime.Now.ToString("HH:mm:ss");
                        Frm_TCPServer.Instance.tbx_log.AppendText(curTime + "->  : " + result + "\r\n");
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
        private void Recieve(object obj)
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int length = 0;
                    try
                    {
                        length = ((Socket)obj).Receive(buffer);
                    }
                    catch { }
                    string result = Encoding.Default.GetString(buffer, 0, length);
                    if (length > 0)
                    {
                        string logLine = DateTime.Now.ToString("HH:mm:ss") + "->  : " + result + "\r\n";
                        Frm_Main.Instance.BeginInvoke(new Action(() =>
                        {
                            if (Frm_TCPServer.Instance.Visible)
                                Frm_TCPServer.Instance.tbx_log.AppendText(logLine);
                        }));
                        receivedStr = result;
                    }
                    else
                    {
                        // 关闭客户端Socket（而非服务器监听Socket）
                        Socket clientSocket = (Socket)obj;
                        try
                        {
                            if (clientSocket.Connected)
                                clientSocket.Disconnect(false);
                            clientSocket.Close();
                        }
                        catch { }

                        // 从客户端列表中移除已断开的客户端
                        for (int j = 0; j < L_STCPSever.Count; j++)
                        {
                            if (L_STCPSever[j].severName == Name)
                            {
                                string clientKey = null;
                                foreach (var kv in L_STCPSever[j].L_Client)
                                {
                                    if (kv.Value == clientSocket) { clientKey = kv.Key; break; }
                                }
                                if (clientKey != null)
                                    L_STCPSever[j].L_Client.Remove(clientKey);
                                break;
                            }
                        }

                        string localName = Name;
                        Frm_Main.Instance.BeginInvoke(new Action(() =>
                        {
                            if (Frm_TCPServer.Instance.Visible)
                            {
                                if (Frm_DeviceManager.Instance.dgv_deviceList.SelectedRows.Count > 0 &&
                                    Frm_DeviceManager.Instance.dgv_deviceList.SelectedRows[0].Cells[0].Value.ToString() == localName)
                                {
                                    Frm_TCPServer.Instance.btn_listen.TextStr = "连接";
                                    Frm_DeviceManager.Instance.lbl_tip.Text = "连接已断开";
                                }
                            }
                        }));
                        Frm_Main.Instance.OutputMsg("客户端连接已断开", Color.Red);
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
                for (int i = 0; i < L_STCPSever.Count; i++)
                {
                    if (L_STCPSever[i].severName != Name)
                        continue;

                    //断开当前服务端的所有客户端
                    foreach (KeyValuePair<string, Socket> item in L_STCPSever[i].L_Client.ToList())
                    {
                        if (item.Value.Connected)
                            item.Value.Disconnect(false);
                        item.Value.Close();
                    }

                    try
                    {
                        if (L_STCPSever[i].SeverObj.Connected)
                            L_STCPSever[i].SeverObj.Disconnect(false);
                        L_STCPSever[i].SeverObj.Close();
                    }
                    catch { }

                    STCPSever stcpSever = L_STCPSever[i];
                    stcpSever.SeverObj = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    stcpSever.L_Client = new Dictionary<string, Socket>();
                    L_STCPSever[i] = stcpSever;
                    listened = false;
                    Frm_Main.Instance.BeginInvoke(new Action(() =>
                    {
                        if (Frm_TCPServer.Instance.Visible)
                        {
                            Frm_TCPServer.Instance.btn_listen.TextStr = "开始监听";
                            Frm_TCPServer.Instance.lbx_connectedList.Items.Clear();
                            Frm_TCPServer.Instance.cbx_connectedList.Clear();
                        }
                        Frm_DeviceManager.Instance.lbl_tip.Text = "TCP服务端已停止监听";
                    }));
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
    }
    /// <summary>
    /// 服务器类型
    /// </summary>
    internal struct STCPSever
    {
        /// <summary>
        /// 服务器名称
        /// </summary>
        internal string severName;
        /// <summary>
        /// Socket对象
        /// </summary>
        internal Socket SeverObj;
        /// <summary>
        /// 连接到此服务器的客户端集合
        /// </summary>
        internal Dictionary<string, Socket> L_Client;
    }
}
