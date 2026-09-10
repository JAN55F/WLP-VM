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
            EnsureRuntimeSocket();
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
        private static readonly object SocketMapSyncRoot = new object();
        private static readonly Dictionary<string, Socket> L_socket = new Dictionary<string, Socket>();
        private static readonly Dictionary<string, TCPClient> RuntimeNameOwners = new Dictionary<string, TCPClient>();
        /// <summary>
        /// 程序开启后自动连接服务器（默认不自动连接，由用户在设备界面勾选启用；
        /// 此前默认 true，导致用户未勾选时下次启动也自动连服务器——服务器开着就“处于连接状态”，
        /// 关着就弹“连接失败”，看起来像“关闭前自动断开”没生效）
        /// </summary>
        public bool AutoConnectAfterStart = false;
        /// <summary>
        /// 程序关闭前自动断开服务器
        /// </summary>
        public bool AutoDisconnectBeforeClose = true;
        /// <summary>
        /// 断开时是否自动连接
        /// </summary>
        public bool AutoConnect = true;

        [NonSerialized]
        internal volatile bool connecting;

        [NonSerialized]
        private volatile bool manualDisconnect;

        [NonSerialized]
        private int connectionGeneration;

        internal void EnsureRuntime()
        {
            EnsureRuntimeSocket();
        }

        private static Socket CreateRuntimeSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private static void CloseRuntimeSocket(Socket socket)
        {
            if (socket == null)
                return;

            try
            {
                if (socket.Connected)
                    socket.Disconnect(false);
            }
            catch { }

            try
            {
                socket.Close();
            }
            catch { }
        }

        private bool EnsureRuntimeSocket()
        {
            lock (SocketMapSyncRoot)
            {
                string name = Name;
                if (string.IsNullOrEmpty(name))
                    return false;

                TCPClient owner;
                if (RuntimeNameOwners.TryGetValue(name, out owner) && !ReferenceEquals(owner, this))
                    return false;

                RuntimeNameOwners[name] = this;
                if (!L_socket.ContainsKey(name))
                    L_socket.Add(name, CreateRuntimeSocket());
                return true;
            }
        }

        private Socket GetRuntimeSocket()
        {
            lock (SocketMapSyncRoot)
            {
                string name = Name;
                TCPClient owner;
                if (string.IsNullOrEmpty(name) ||
                    !RuntimeNameOwners.TryGetValue(name, out owner) ||
                    !ReferenceEquals(owner, this))
                    return null;

                Socket socket;
                return L_socket.TryGetValue(name, out socket) ? socket : null;
            }
        }

        private static Socket SwapRuntimeSocket(string name, Socket replacement)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            lock (SocketMapSyncRoot)
            {
                Socket previous;
                L_socket.TryGetValue(name, out previous);
                L_socket[name] = replacement;
                return previous;
            }
        }

        private bool TryInstallCurrentRuntimeSocket(Socket replacement, int generation, bool automaticReconnect, out string socketName)
        {
            Socket previous = null;
            lock (SocketMapSyncRoot)
            {
                socketName = Name;
                TCPClient owner;
                if (string.IsNullOrEmpty(socketName) ||
                    (RuntimeNameOwners.TryGetValue(socketName, out owner) && !ReferenceEquals(owner, this)) ||
                    !IsCurrentGeneration(generation) ||
                    (automaticReconnect && (!AutoConnect || manualDisconnect)))
                    return false;

                RuntimeNameOwners[socketName] = this;
                L_socket.TryGetValue(socketName, out previous);
                L_socket[socketName] = replacement;
            }

            if (!ReferenceEquals(previous, replacement))
                CloseRuntimeSocket(previous);
            return true;
        }

        private static Socket RemoveRuntimeSocket(string name, Socket expected)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            lock (SocketMapSyncRoot)
            {
                Socket current;
                if (!L_socket.TryGetValue(name, out current) ||
                    (expected != null && !ReferenceEquals(current, expected)))
                    return null;

                L_socket.Remove(name);
                return current;
            }
        }

        private static Socket RemoveRuntimeSocket(Socket expected)
        {
            if (expected == null)
                return null;

            lock (SocketMapSyncRoot)
            {
                string matchedName = null;
                foreach (KeyValuePair<string, Socket> item in L_socket)
                {
                    if (ReferenceEquals(item.Value, expected))
                    {
                        matchedName = item.Key;
                        break;
                    }
                }

                if (matchedName == null)
                    return null;

                L_socket.Remove(matchedName);
                return expected;
            }
        }

        private static bool IsCurrentRuntimeSocket(Socket expected)
        {
            if (expected == null)
                return false;

            lock (SocketMapSyncRoot)
            {
                foreach (Socket socket in L_socket.Values)
                    if (ReferenceEquals(socket, expected))
                        return true;
                return false;
            }
        }

        internal static KeyValuePair<string, Socket>[] GetRuntimeSocketSnapshot()
        {
            lock (SocketMapSyncRoot)
                return L_socket.ToArray();
        }

        internal static void ResetRuntimeStore()
        {
            Socket[] sockets;
            lock (SocketMapSyncRoot)
            {
                foreach (TCPClient client in RuntimeNameOwners.Values.Distinct())
                {
                    client.manualDisconnect = true;
                    unchecked { client.connectionGeneration++; }
                }

                sockets = L_socket.Values.Distinct().ToArray();
                L_socket.Clear();
                RuntimeNameOwners.Clear();
            }

            foreach (Socket socket in sockets)
                CloseRuntimeSocket(socket);
        }

        internal static void RemoveRuntimeSocket(string name)
        {
            Socket removed = null;
            lock (SocketMapSyncRoot)
            {
                TCPClient owner;
                if (RuntimeNameOwners.TryGetValue(name, out owner))
                {
                    owner.manualDisconnect = true;
                    unchecked { owner.connectionGeneration++; }
                    RuntimeNameOwners.Remove(name);
                }

                L_socket.TryGetValue(name, out removed);
                L_socket.Remove(name);
            }

            CloseRuntimeSocket(removed);
        }

        internal static void UnregisterRuntime(TCPClient client)
        {
            if (client == null)
                return;

            Socket removed = null;
            lock (SocketMapSyncRoot)
            {
                string name = client.Name;
                TCPClient owner;
                if (!string.IsNullOrEmpty(name) &&
                    RuntimeNameOwners.TryGetValue(name, out owner) &&
                    ReferenceEquals(owner, client))
                {
                    client.manualDisconnect = true;
                    unchecked { client.connectionGeneration++; }
                    RuntimeNameOwners.Remove(name);
                    L_socket.TryGetValue(name, out removed);
                    L_socket.Remove(name);
                }
            }

            CloseRuntimeSocket(removed);
        }

        private bool TryRenameRuntimeSocket(string newName)
        {
            lock (SocketMapSyncRoot)
            {
                string oldName = Name;
                if (string.Equals(oldName, newName, StringComparison.Ordinal))
                    return true;
                TCPClient destinationOwner;
                if (RuntimeNameOwners.TryGetValue(newName, out destinationOwner) &&
                    !ReferenceEquals(destinationOwner, this))
                    return false;

                TCPClient oldOwner;
                bool ownsOldName = !string.IsNullOrEmpty(oldName) &&
                    RuntimeNameOwners.TryGetValue(oldName, out oldOwner) &&
                    ReferenceEquals(oldOwner, this);
                Socket moving = null;
                if (ownsOldName)
                {
                    L_socket.TryGetValue(oldName, out moving);
                    L_socket.Remove(oldName);
                    RuntimeNameOwners.Remove(oldName);
                }

                RuntimeNameOwners[newName] = this;
                L_socket.Add(newName, moving ?? CreateRuntimeSocket());
                Name = newName;
                return true;
            }
        }

        internal bool Rename(string newName)
        {
            try
            {
                if (string.IsNullOrEmpty(newName) || Name == newName)
                    return Name == newName;

                return TryRenameRuntimeSocket(newName);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
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
                if (!EnsureRuntimeSocket())
                    return null;
                return GetRuntimeSocket();
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
            int generation = BeginExplicitConnect();
            return ConnectPrepared(timeoutMs, showTip, generation);
        }

        internal int BeginExplicitConnect()
        {
            lock (SocketMapSyncRoot)
            {
                manualDisconnect = false;
                unchecked { connectionGeneration++; }
                return connectionGeneration;
            }
        }

        internal bool ConnectPrepared(int timeoutMs, bool showTip, int generation)
        {
            return ConnectCore(timeoutMs, showTip, false, generation);
        }

        private bool ConnectCore(int timeoutMs, bool showTip, bool automaticReconnect, int generation)
        {
            Socket socket = null;
            try
            {
                if (!IsCurrentGeneration(generation) || (automaticReconnect && !ShouldAutoReconnect(generation)))
                    return false;

                string serverIp = severIP;
                int serverPort = severPort;
                IPAddress ip;
                try
                {
                    ip = IPAddress.Parse(serverIp);
                }
                catch
                {
                    ShowConnectMessage("\r\nIP地址有误或IP不存在，连接失败，请检查");
                    return false;
                }
                IPEndPoint point = new IPEndPoint(ip, serverPort);
                socket = CreateRuntimeSocket();
                if (!IsCurrentGeneration(generation) || (automaticReconnect && !ShouldAutoReconnect(generation)))
                {
                    CloseRuntimeSocket(socket);
                    return false;
                }

                string socketName;
                if (!TryInstallCurrentRuntimeSocket(socket, generation, automaticReconnect, out socketName))
                {
                    CloseRuntimeSocket(socket);
                    return false;
                }
                if (!IsCurrentGeneration(generation) || (automaticReconnect && !ShouldAutoReconnect(generation)))
                {
                    RemoveRuntimeSocket(socket);
                    CloseRuntimeSocket(socket);
                    return false;
                }
                try
                {
                    IAsyncResult result = socket.BeginConnect(point, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(timeoutMs, true);
                    if (!success)
                    {
                        RemoveRuntimeSocket(socket);
                        CloseRuntimeSocket(socket);
                        if (showTip)
                            ShowConnectMessage(string.Format("\r\n客户端 [{0}] 连接失败：服务端未监听或网络超时", socketName));
                        return false;
                    }
                    socket.EndConnect(result);
                }
                catch (Exception ex)
                {
                    RemoveRuntimeSocket(socket);
                    CloseRuntimeSocket(socket);
                    if (showTip)
                        ShowConnectMessage(string.Format("\r\n客户端 [{0}] 连接失败：{1}", socketName, ex.Message));
                    return false;
                }

                if (socket.Connected && IsCurrentGeneration(generation) && IsCurrentRuntimeSocket(socket))
                {
                    Thread th_recieve = new Thread(new ThreadStart(delegate { Recieve(socket, generation); }));
                    th_recieve.IsBackground = true;
                    th_recieve.Start();
                    return true;
                }

                RemoveRuntimeSocket(socket);
                CloseRuntimeSocket(socket);
                return false;
            }
            catch (Exception ex)
            {
                RemoveRuntimeSocket(socket);
                CloseRuntimeSocket(socket);
                Log.SaveError(ex);
                return false;
            }
        }

        private bool IsCurrentGeneration(int generation)
        {
            return Volatile.Read(ref connectionGeneration) == generation;
        }

        private bool ShouldAutoReconnect(int generation)
        {
            return IsCurrentGeneration(generation) && AutoConnect && !manualDisconnect;
        }

        private Socket InvalidateAndDetachRuntimeSocket()
        {
            lock (SocketMapSyncRoot)
            {
                manualDisconnect = true;
                unchecked { connectionGeneration++; }

                string name = Name;
                TCPClient owner;
                if (string.IsNullOrEmpty(name) ||
                    !RuntimeNameOwners.TryGetValue(name, out owner) ||
                    !ReferenceEquals(owner, this))
                    return null;

                Socket socket;
                L_socket.TryGetValue(name, out socket);
                L_socket.Remove(name);
                return socket;
            }
        }

        private void ShowConnectMessage(string message)
        {
            try
            {
                Machine.ShowMessageOnMainUiThread(message);
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
                string curTime = DateTime.Now.ToString("HH:mm:ss");
                Frm_TCPClient.TryAppendLog(this, curTime + "<-  : " + msg + "\r\n");
                byte[] buffer = Encoding.Default.GetBytes(msg);
                Socket socket = FindSocketByName();
                if (socket == null)
                    return;
                socket.Send(buffer);
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
                    Socket socket = FindSocketByName();
                    if (socket != null)
                        length = socket.Receive(buffer);
                }
                catch { }
                string result = Encoding.Default.GetString(buffer, 0, length);
                if (length > 0)
                {
                    string curTime = DateTime.Now.ToString("HH:mm:ss");
                    Frm_TCPClient.TryAppendLog(this, curTime + "->  : " + result + "\r\n");
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
        private void Recieve(Socket socket, int generation)
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int length = 0;
                    try
                    {
                        length = socket.Receive(buffer);
                    }
                    catch { }
                    string result = Encoding.Default.GetString(buffer, 0, length);
                    if (length > 0)
                    {
                        if (!IsCurrentGeneration(generation) || !IsCurrentRuntimeSocket(socket))
                            return;

                        string logLine = DateTime.Now.ToString("HH:mm:ss") + "->  : " + result + "\r\n";
                        Frm_TCPClient.TryAppendLog(this, logLine);
                        receivedStr = result;
                    }
                    else
                    {
                        bool wasCurrentSocket = RemoveRuntimeSocket(socket) != null;
                        CloseRuntimeSocket(socket);
                        if (!wasCurrentSocket || !IsCurrentGeneration(generation))
                            return;

                        string localName1 = Name;
                        Frm_TCPClient.TryApplyConnectionState(this, false);
                        Frm_DeviceManager.TrySetTipForDevice("TCPClient", localName1,
                            "连接已断开", Color.Red);

                        if (!ShouldAutoReconnect(generation))
                            return;

                        Frm_Main.Instance.OutputMsg("服务器连接已中断，已启动自动重连...", Color.Red);
                        while (ShouldAutoReconnect(generation))
                        {
                            Socket current = GetRuntimeSocket();
                            if (current != null && current.Connected)
                                break;

                            if (ConnectCore(2000, false, true, generation))
                                break;
                            Thread.Sleep(1000);
                        }

                        Socket reconnected = GetRuntimeSocket();
                        if (!ShouldAutoReconnect(generation) || reconnected == null || !reconnected.Connected)
                            return;

                        string localName2 = Name;
                        Frm_TCPClient.TryApplyConnectionState(this, true);
                        Frm_DeviceManager.TrySetTipForDevice("TCPClient", localName2,
                            "连接成功", Color.Green);
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
                Socket socket = InvalidateAndDetachRuntimeSocket();
                CloseRuntimeSocket(socket);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
