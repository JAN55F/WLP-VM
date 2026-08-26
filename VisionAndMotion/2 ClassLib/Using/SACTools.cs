using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Threading; 
using Microsoft.Win32;

/***************************************************************************
 * Copyright(c)  : CSS SAC Team
 * CreateDate    : 2015-Dec-03
 * Creater       : Wood Chen
 * LastChangeDate: 2015-Dec-03 19:33
 * LastChanger   : Wood Chen
 * Version Info  : 1.0.0
 * Author        : SAC Team
 * Mail          : woche@celestica.com
 * Description   : For SAC team software development.
 * 
 *                 Anyone who is SAC software team member can update 
 *                 this code, yet he/she need to add the update content 
 *                 in this comment area.
 *                 
 *                 Any of the new updated function needs to be verified OK.
 *                 
 * Remark        : This CS file might include many of bugs. Any of SAC 
 *                 software team member please try to find them, solve them  
 *                 and record those down for our learning reference.
 *                 -Your hard work will be appreciated. Thanks a lot!
 * *************************************************************************/

namespace SACTools
{
    internal class IniReadWrite
    {
        #region 外部接口
        internal string FilePath;
        internal void IniFile(string INIPath)
        {
            FilePath = INIPath;
        }
        #endregion
        #region 调用Windows API
        /// 
        /// 写入INI文件
        /// 
        ///节点名称[如[TypeName]] 
        ///键 
        ///值 
        ///文件路径 
        /// 
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        /// 
        /// 读取INI文件
        /// 
        ///节点名称 
        ///键 
        ///值 
        ///stringbulider对象 
        ///字节大小 
        ///文件路径 
        /// 
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpReturnedString, uint nSize, string lpFileName);

        #endregion
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="Section">节点</param>
        /// <param name="Key">键</param>
        /// <returns>值</returns>
        internal string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, FilePath);
            return temp.ToString();
        }
        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="Section">节点</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        internal void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, FilePath);
        }
        /// <summary>
        /// 读取某个节点下的相应键值
        /// </summary>
        /// <param name="Section">节点</param>
        /// <param name="StartNumber">起始位</param>
        /// <returns>所有键值</returns>
        internal string[] ReadIniAllKeys(string Section, int StartNumber)
        {
            UInt32 MAX_BUFFER = 32767;
            string[] items = new string[0];
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));
            UInt32 bytesReturned = GetPrivateProfileSection(Section, pReturnedString, MAX_BUFFER, FilePath);

            if (!(bytesReturned == MAX_BUFFER - 2) || (bytesReturned == 0))
            {
                string returnedString = Marshal.PtrToStringAuto(pReturnedString, (int)bytesReturned);
                items = returnedString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            Marshal.FreeCoTaskMem(pReturnedString);

            string[] AllKeys = new string[items.Length - StartNumber];

            int j = 0;
            for (int i = StartNumber; i < items.Length; i++)
            {
                //items[i].IndexOf("=");
                AllKeys[j] = items[i].Substring(0, items[i].IndexOf("=")).Trim();
                j++;
            }
            return AllKeys;
        }
        /// <summary>
        /// 判断是否为数字字符串
        /// </summary>
        /// <param name="Section">输入字符</param>
        /// <returns>布尔值</returns>
        internal bool IsNumber(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                   objNumberPattern.IsMatch(strNumber);
        }
    }
    internal class TxtReadWrite
    {
        #region 外部接口
        internal string sFileName { get; set; }
        internal string sContent  { get; set; }
        #endregion
        /// <summary>
        /// 保存txt文件
        /// </summary>
        /// <param name="sFileMode">保存模式</param>
        internal void SaveTxt(FileMode sFileMode)
        {
            try
            {
                if (!System.IO.File.Exists(sFileName))
                {
                    FileStream fs;
                    fs = File.Create(sFileName);
                    fs.Close();
                }
                FileStream fsTxtWrite = new FileStream(sFileName, sFileMode, FileAccess.Write);
                StreamWriter srWrite = new StreamWriter(fsTxtWrite, System.Text.Encoding.UTF8);
                //StreamReader rd = new StreamReader(fsTxtWrite);

                //string a = rd.ReadToEnd();
                srWrite.Write(sContent);

                //srWrite.WriteLine(sContent);

                srWrite.Close();
                srWrite.Dispose();
                fsTxtWrite.Dispose();
                //=====================================================
                //string alllog = "";
                //alllog = rd.ReadToEnd();
                //=====================================================

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                #region ### error info
                //try
                //{
                //    string sFolder = Directory.GetParent(sFileName).ToString();
                //    string sErrFullName = sFolder + "_err_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                //    if (!System.IO.File.Exists(sErrFullName))
                //    {
                //        FileStream fs;
                //        fs = File.Create(sErrFullName);

                //        StreamWriter srWrite = new StreamWriter(fs, System.Text.Encoding.UTF8);

                //        srWrite.Write(sFileName);

                //        srWrite.Close();
                //        srWrite.Dispose();

                //        fs.Close();
                //    }
                //}
                //catch (Exception)
                //{ }
                #endregion ###error
            }
        }
        /// <summary>
        /// 读取txt文件
        /// </summary>
        /// <returns>文件内容</returns>
        internal string ReadTxt()
        {
            if (!System.IO.File.Exists(sFileName))
            {
                return "File Not Exist!";
            }
            else
            {
                return File.ReadAllText(sFileName);
            }
        }
    }
    internal class TcpIpCommunicate
    {
        internal class TCPServer
        {
            #region 外部接口
            internal string ServerIP { get; set; } 
            internal Int32 port { get; set; }
            internal List<User> ClientList 
            {
                get
                {
                    return _ClientList;
                }
            }
            internal string receiveString 
            {
                get { return _Rcvmsg; }
                private set
                {
                    _Rcvmsg = value;
                    this.OnMsgReceived(new EventArgs());
                }
            }
            internal string StatusMessage 
            {
                get { return _StatusMessage; }
                private set
                {
                    _StatusMessage = value;
                    this.OnStatusChanged(new EventArgs());
                }
            }
            
            #endregion
            #region 内部变量
            private TcpListener myListener;
            private bool isNormalExit = false;
            private string _Rcvmsg = "";
            private string _StatusMessage;
            private List<User> _ClientList = new List<User>();
            #endregion
            #region 事件
            internal event EventHandler MsgReceived;
            internal event EventHandler StatusChanged;
            #endregion
            internal void StartListen()
            { 
                myListener = new TcpListener(IPAddress.Parse(ServerIP), port);
                myListener.Start();
                StatusMessage = string.Format("开始在{0}:{1}监听客户连接", ServerIP, port);
                //创建一个线程监客户端连接请求
                Thread myThread = new Thread(_Listen);
                myThread.Start();
            }
            private void _Listen()
            {
                TcpClient newClient = null;
                while (true)
                {
                    try
                    {
                        newClient = myListener.AcceptTcpClient();
                    }
                    catch
                    {
                        //当单击‘停止监听’或者退出此窗体时 AcceptTcpClient() 会产生异常
                        //因此可以利用此异常退出循环
                        break;
                    }
                    //每接收一个客户端连接，就创建一个对应的线程循环接收该客户端发来的信息；
                    User user = new User(newClient);
                    Thread threadReceive = new Thread(ReceiveData);
                    threadReceive.Start(user);
                    _ClientList.Add(user);
                    StatusMessage = string.Format(string.Format("[{0}]进入", newClient.Client.RemoteEndPoint)) + Environment.NewLine;
                    StatusMessage = string.Format("当前连接用户数：{0}", _ClientList.Count) + Environment.NewLine;
                }
            }
            /// <summary>
            /// 处理接收的客户端信息
            /// </summary>
            /// <param name="userState">客户端信息</param>
            private void ReceiveData(object userState)
            {
                User user = (User)userState;
                TcpClient client = user.client;
                while (isNormalExit == false)
                {
                    receiveString = null;
                    try
                    {
                        //从网络流中读出字符串，此方法会自动判断字符串长度前缀
                        receiveString = user.br.ReadString();
                        
                    }
                    catch (Exception)
                    {
                        if (isNormalExit == false)
                        {
                            StatusMessage = string.Format("与[{0}]失去联系，已终止接收该用户信息", client.Client.RemoteEndPoint) + Environment.NewLine;
                            RemoveUser(user);
                        }
                        break;
                    }
                    StatusMessage = string.Format("来自[{0}]：{1}", user.client.Client.RemoteEndPoint, receiveString) + Environment.NewLine;
                    #region 命令处理
                    //string[] splitString = receiveString.Split(',');
                    //switch (splitString[0])
                    //{
                    //    case "Login":
                    //        user.userName = splitString[1];
                    //        SendToAllClient(user, receiveString);
                    //        break;
                    //    case "Logout":
                    //        SendToAllClient(user, receiveString);
                    //        RemoveUser(user);
                    //        return;
                    //    case "Talk":
                    //        string talkString = receiveString.Substring(splitString[0].Length + splitString[1].Length + 2);
                    //        StatusMessage = string.Format("{0}对{1}说：{2}", user.userName, splitString[1], talkString) + Environment.NewLine;
                    //        SendToClient(user, "talk," + user.userName + "," + talkString);
                    //        foreach (User target in _ClientList)
                    //        {
                    //            if (target.userName == splitString[1] && user.userName != splitString[1])
                    //            {
                    //                SendToClient(target, "talk," + user.userName + "," + talkString);
                    //                break;
                    //            }
                    //        }
                    //        break;
                    //    default:
                    //        StatusMessage = string.Format("未知指令：" + receiveString) + Environment.NewLine;
                    //        break;
                    //}
                    #endregion
                }
            }
            /// <summary>
            /// 移除用户
            /// </summary>
            /// <param name="user">指定要移除的用户</param>
            private void RemoveUser(User user)
            {
                _ClientList.Remove(user);
                user.Close();
                //AddItemToListBox(string.Format("当前连接用户数：{0}", userList.Count));
            }
            /// <summary>
            /// 发送消息给所有客户
            /// </summary>
            /// <param name="user">指定发给哪个用户</param>
            /// <param name="message">信息内容</param>
            internal void SendToAllClient(User user, string message)
            {
                string command = message.Split(',')[0].ToLower();
                if (command == "login")
                {
                    //获取所有客户端在线信息到当前登录用户
                    for (int i = 0; i < _ClientList.Count; i++)
                    {
                        SendToClient(user, "login," + _ClientList[i].userName);
                    }
                    //把自己上线，发送给所有客户端
                    for (int i = 0; i < _ClientList.Count; i++)
                    {
                        if (user.userName != _ClientList[i].userName)
                        {
                            SendToClient(_ClientList[i], "login," + user.userName);
                        }
                    }
                }
                else if (command == "logout")
                {
                    for (int i = 0; i < _ClientList.Count; i++)
                    {
                        if (_ClientList[i].userName != user.userName)
                        {
                            SendToClient(_ClientList[i], message);
                        }
                    }
                }
            }
            /// <summary>
            /// 发送 message 给 user
            /// </summary>
            /// <param name="user">指定发给哪个用户</param>
            /// <param name="message">信息内容</param>
            internal void SendToClient(User user, string message)
            {
                try
                {
                    //将字符串写入网络流，此方法会自动附加字符串长度前缀
                    user.bw.Write(message);
                    user.bw.Flush();
                    //AddItemToListBox(string.Format("向[{0}]发送：{1}", user.userName, message));
                }
                catch
                {
                    //AddItemToListBox(string.Format("向[{0}]发送信息失败", user.userName));
                }
            }

            private void OnMsgReceived(EventArgs eventArgs)
            {
                if (this.MsgReceived != null)
                {
                    this.MsgReceived(this,eventArgs);
                }
            }
            private void OnStatusChanged(EventArgs eventArgs)
            {
                if (this.StatusChanged != null)
                {
                    this.StatusChanged(this,eventArgs);
                }
            }

            internal void CloseServer()
            {
                StatusMessage = "开始停止服务，并依次使用户退出！";
                isNormalExit = true;
                for (int i = ClientList.Count - 1; i >= 0; i--)
                {
                    RemoveUser(ClientList[i]);
                }
                //通过停止监听让 myListener.AcceptTcpClient() 产生异常退出监听线程
                myListener.Stop();
            }
        }
        internal class TCPClient
        {
            #region 外部接口
            internal string userName { get; set; }
            internal string ServerIP { get; set; }
            internal Int32 port { get; set; }
            internal string StatusMessage 
            {
                get { return _statusmsg; }
                private set
                {
                    _statusmsg = value;
                    this.OnStatusChanged(new EventArgs());
                }
            }
            internal List<string> OtherOnlineUsers { get; private set; }
            internal string receiveString 
            {
                get { return _RcvStr; }
                private set
                {
                    _RcvStr = value;
                    this.OnMsgReceived(new EventArgs());
                }
            }
            internal bool ConnectStatus 
            {
                get { return _connectStatus; }
                private set
                {
                    _connectStatus = value;
                    this.OnConnectStatusChanged(new EventArgs());
                }
            }
            #endregion
            #region 事件
            internal event EventHandler MsgReceived;
            internal event EventHandler StatusChanged;
            internal event EventHandler ConnectStatusChanged;
            #endregion
            #region 内部变量
            private bool isExit = false;
            private TcpClient client;
            private BinaryReader br;
            private BinaryWriter bw;
            private string _statusmsg;
            private string _RcvStr;
            private bool _connectStatus;
            #endregion
            
            /// <summary>
            /// 关闭本客户端
            /// </summary>
            internal void Close()
            {
                br.Close();
                bw.Close();
                client.Close();
            }
            /// <summary>
            /// 连接到服务端
            /// </summary>
            internal void ConnectToServer()
            {
                try
                {
                    //此处为方便演示，实际使用时要将Dns.GetHostName()改为服务器域名
                    //IPAddress ipAd = IPAddress.Parse("182.150.193.7");
                    client = new TcpClient();
                    client.Connect(IPAddress.Parse(ServerIP), port);
                    StatusMessage = "连接成功" + Environment.NewLine;
                    ConnectStatus = true;
                }
                catch (Exception ex)
                {
                    StatusMessage = "连接失败，原因：" + ex.Message + Environment.NewLine;
                    ConnectStatus = false;
                    return;
                }
                //获取网络流
                NetworkStream networkStream = client.GetStream();
                //将网络流作为二进制读写对象
                br = new BinaryReader(networkStream);
                bw = new BinaryWriter(networkStream);
                OtherOnlineUsers = new List<string>();
                SendMessage("Login," + userName);
                Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
                threadReceive.IsBackground = true;
                threadReceive.Start();
            }
            /// <summary>
            /// 向服务端发送消息
            /// </summary>
            /// <param name="message"></param>
            internal void SendMessage(string message)
            {
                try
                {
                    //将字符串写入网络流，此方法会自动附加字符串长度前缀
                    bw.Write(message);
                    bw.Flush();
                }
                catch
                {
                    StatusMessage = "发送 " + message + " 至服务器失败" + Environment.NewLine;
                    ConnectStatus = false;
                }
            }
            /// <summary>
            /// 接收服务器信息
            /// </summary>
            internal void ReceiveData()
            {
                receiveString = null;
                while (isExit == false)
                {
                    try
                    {
                        //从网络流中读出字符串
                        //此方法会自动判断字符串长度前缀，并根据长度前缀读出字符串
                        receiveString = br.ReadString();
                    }
                    catch
                    {
                        if (isExit == false)
                        {
                            StatusMessage = "与服务器失去连接" + Environment.NewLine;
                            ConnectStatus = false;
                        }
                        break;
                    }
                    #region 命令处理-暂不用
                    //string[] splitString = receiveString.Split(',');
                    //string command = splitString[0].ToLower();
                    //switch (command)
                    //{
                    //    case "login":   //格式： login,用户名
                    //        OtherOnlineUsers.Add(splitString[1]);
                    //        break;
                    //    case "logout":  //格式： logout,用户名
                    //        OtherOnlineUsers.Remove(splitString[1]);
                    //        break;
                    //    case "talk":    //格式： talk,用户名,对话信息
                    //        StatusMessage = splitString[1] + "：\r\n" + Environment.NewLine;
                    //        StatusMessage = receiveString.Substring(splitString[0].Length + splitString[1].Length + 2);
                    //        break;
                    //    default:
                    //        StatusMessage = "未知信息：" + receiveString;
                    //        break;
                    //}
                    #endregion
                }
                return;
            }

            private void OnStatusChanged(EventArgs eventArgs)
            {
                if (this.StatusChanged != null)
                {
                    this.StatusChanged(this,eventArgs);
                }
            }
            private void OnMsgReceived(EventArgs eventArgs)
            {
                if (this.MsgReceived != null)
                {
                    this.MsgReceived(this,eventArgs);
                }
            }
            private void OnConnectStatusChanged(EventArgs eventArgs)
            {
                if (this.ConnectStatusChanged != null)
                {
                    this.ConnectStatusChanged(this,eventArgs);
                }
            }
        }
        internal class User
        {
            internal TcpClient client { get; private set; }
            internal BinaryReader br { get; private set; }
            internal BinaryWriter bw { get; private set; }
            internal string userName { get; set; }

            internal User(TcpClient client)
            {
                this.client = client;
                NetworkStream networkStream = client.GetStream();
                br = new BinaryReader(networkStream);
                bw = new BinaryWriter(networkStream);
            }

            internal void Close()
            {
                br.Close();
                bw.Close();
                client.Close();
            }

        }
    }

    //未完成
    internal class FileTransfer
    {
        internal string FilePath;
        internal string DestinationPath;
    }

    internal class XMLReadWrite
    {
        /// <summary>
        /// 创建XML
        /// </summary>
        internal void CreatXML()
        {
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径　　
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "NotesAccess.exe.config";
            doc.Load(strFileName);
            //找出名称为“add”的所有元素　　
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性　　　　
                XmlAttribute att = nodes[i].Attributes["key"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素　　　　
                if (att.Value == "time")
                {
                    //对目标元素中的第二个属性赋值　　　　　
                    att = nodes[i].Attributes["value"];
                    att.Value = DateTime.Now.ToString(); ;
                    break;
                }
            }
            //保存上面的修改　　
            doc.Save(strFileName);
        }
        /// <summary>
        /// 读取XML文件
        /// </summary>
        /// <param name="FilePath">XML文件路径</param>
        /// <param name="NodeName">节点名</param>
        /// <returns>值</returns>
        internal string ReadXML(string FilePath, string NodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FilePath);
            XmlElement rootElem = doc.DocumentElement;
            XmlNodeList personNodes = rootElem.GetElementsByTagName(NodeName);

            foreach (XmlNode node in personNodes)
            { 
                
            }



            return "";
        }
    }
    internal class RegeditReadWrite
    {
        /// <summary>
        /// 读取的注册表中HKEY\HKEYSubKey目录下的SubKey目录中名称为name的注册表值
        /// </summary>
        /// <param name="HKEY">HKEY</param>
        /// <param name="HKEYSubKey">目录</param>
        /// <param name="SubKey">子键</param>
        /// <param name="name">名称</param>
        /// <returns>注册表值</returns>
        private string GetRegistData(RegistryKey HKEY, string HKEYSubKey,string SubKey,string name)
        {
            string registData;
            RegistryKey hkml = HKEY;
            RegistryKey software = hkml.OpenSubKey(HKEYSubKey, true);
            RegistryKey aimdir = software.OpenSubKey(SubKey, true);
            registData = aimdir.GetValue(name).ToString();
            return registData;
        }
        /// <summary>
        /// 在注册表中HKEY\HKEYSubKey目录下新建SubKey目录并在此目录下创建名称为name,值为tovalue的注册表项
        /// </summary>
        /// <param name="HKEY">HKEY</param>
        /// <param name="HKEYSubKey">目录</param>
        /// <param name="SubKey">子键</param>
        /// <param name="name">名称</param>
        /// <param name="tovalue">写入值</param>
        private void WriteRegedit(RegistryKey HKEY, string HKEYSubKey, string SubKey, string name, string tovalue)
        {
            RegistryKey hklm = HKEY;
            RegistryKey software = hklm.OpenSubKey(HKEYSubKey, true);
            RegistryKey aimdir = software.CreateSubKey(SubKey);
            aimdir.SetValue(name, tovalue);
        }
        /// <summary>
        /// 在注册表中HKEY\HKEYSubKey目录下SubKey目录中删除名称为name注册表项
        /// </summary>
        /// <param name="HKEY">HKEY</param>
        /// <param name="HKEYSubKey">目录</param>
        /// <param name="SubKey">子键</param>
        /// <param name="name">名称</param>
        private void DeleteRegist(RegistryKey HKEY, string HKEYSubKey, string SubKey, string name)
        {
            string[] aimnames;
            RegistryKey hkml = HKEY;
            RegistryKey software = hkml.OpenSubKey(HKEYSubKey, true);
            RegistryKey aimdir = software.OpenSubKey(SubKey, true);
            aimnames = aimdir.GetSubKeyNames();
            foreach (string aimKey in aimnames)
            {
                if (aimKey == name)
                    aimdir.DeleteSubKeyTree(name);
            }
        }
        /// <summary>
        /// 判断在注册表中HKEY\HKEYSubKey目录下SubKey目录中是否存在名称为name注册表项
        /// </summary>
        /// <param name="HKEY">HKEY</param>
        /// <param name="HKEYSubKey">目录</param>
        /// <param name="SubKey">子键</param>
        /// <param name="name">名称</param>
        private bool IsRegeditExit(RegistryKey HKEY, string HKEYSubKey, string SubKey, string name)
        {
            bool _exsit = false;
            string[] subkeyNames;
            RegistryKey hkml = HKEY;
            RegistryKey software = hkml.OpenSubKey(HKEYSubKey, true);
            RegistryKey aimdir = software.OpenSubKey(SubKey, true);
            subkeyNames = aimdir.GetSubKeyNames();
            foreach (string keyName in subkeyNames)
            {
                if (keyName == name)
                {
                    _exsit = true;
                    return _exsit;
                }
            }
            return _exsit;
        }
    }
}
