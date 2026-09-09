using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    /// <summary>
    /// PLC 通讯工具：支持对 AB PLC（CIP 协议）进行寄存器读写，结果可接入流程数据流。
    /// </summary>
    [Serializable]
    class PLCCommTool : ToolBase
    {
        internal PLCCommTool()
        {
            ApplyLastInputIfEmpty();
        }

        /// <summary>绑定的 PLC 设备名称（来自设备管理器）</summary>
        internal string PLCDeviceName = string.Empty;

        /// <summary>操作地址（Tag 名，例如 "Motor_Speed"）</summary>
        internal string Address = string.Empty;

        /// <summary>数据类型："Bool" "Int16" "Int32" "Float" "Double" "String"</summary>
        internal string PLCDataType = "Float";

        /// <summary>操作模式："读取" "写入" "读写"</summary>
        internal string OpMode = "读取";

        /// <summary>字符串类型读取时的最大长度</summary>
        internal ushort StringLength = 32;

        /// <summary>期望值：读取模式下阻塞等待，直到读到此值才继续（为空时不阻塞，直接读一次）</summary>
        internal string ExpectValue = string.Empty;

        /// <summary>设为 true 可放弃当前阻塞等待，令工具以成功状态返回</summary>
        internal volatile bool quitWait = false;

        /// <summary>工具参数（输入/输出项）</summary>
        internal ToolPar toolPar = new ToolPar();

        /// <summary>读取值变化通知，用于界面在等待期望值时即时刷新显示</summary>
        [NonSerialized]
        internal Action<string> ReadValueChanged;

        // ── Run ──────────────────────────────────────────────────────────────

        public override void Run(bool updateImage, bool debugTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = ToolRunStatu.未知原因;

                    // 1. 找设备。多个 PLCCommTool 可绑定同一个 PLCDeviceName，共享同一设备连接。
                    PLCDevice device = PLCDevice.FindByName(PLCDeviceName);
                    if (device == null)
                    {
                        toolRunStatu = ToolRunStatu.未建立通讯连接;
                        Frm_Main.Instance.OutputMsg(
                            string.Format("[PLCComm] 工具 [{0}] 未找到PLC设备：{1}", toolName, PLCDeviceName), Color.Red);
                        return;
                    }
                    if (!device.IsConnected)
                    {
                        toolRunStatu = ToolRunStatu.未建立通讯连接;
                        Frm_Main.Instance.OutputMsg(
                            string.Format("[PLCComm] 工具 [{0}] PLC设备 [{1}] 未连接", toolName, PLCDeviceName), Color.Red);
                        return;
                    }

                    CipCommunication comm = device.Comm;

                    // 2. 写入
                    if (OpMode == "写入" || OpMode == "读写")
                    {
                        string writeVal = toolPar.InputPar.写入值;
                        if (string.IsNullOrEmpty(writeVal))
                        {
                            toolRunStatu = ToolRunStatu.输入项未链接源;
                            return;
                        }
                        try
                        {
                            lock (device.SyncRoot)
                            {
                                WriteToPlc(comm, Address, PLCDataType, writeVal);
                            }
                        }
                        catch (Exception ex)
                        {
                            toolRunStatu = ToolRunStatu.未知原因;
                            Frm_Main.Instance.OutputMsg(
                                string.Format("[PLCComm] 工具 [{0}] 写入失败：{1}", toolName, ex.Message), Color.Red);
                            return;
                        }
                    }

                    // 3. 读取
                    if (OpMode == "读取" || OpMode == "读写")
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(ExpectValue))
                            {
                                // 阻塞模式：循环读取，直到值等于期望值
                                Frm_Main.Instance.OutputMsg(
                                    string.Format("[PLCComm] 工具 [{0}] 等待 [{1}] 的值变为 [{2}]...", toolName, Address, ExpectValue), Color.Black);
                                while (true)
                                {
                                    if (quitWait)
                                    {
                                        quitWait = false;
                                        toolRunStatu = ToolRunStatu.成功;
                                        return;
                                    }
                                    string readVal;
                                    try
                                    {
                                        lock (device.SyncRoot)
                                        {
                                            readVal = ReadFromPlc(comm, Address, PLCDataType);
                                        }
                                    }
                                    catch { readVal = string.Empty; }
                                    SetReadValue(readVal);
                                    if (readVal == ExpectValue)
                                    {
                                        Frm_Main.Instance.OutputMsg(
                                            string.Format("[PLCComm] 工具 [{0}] 已读到期望值 [{1}]", toolName, readVal), Color.Black);
                                        break;
                                    }
                                    System.Threading.Thread.Sleep(50);
                                }
                            }
                            else
                            {
                                string readVal;
                                lock (device.SyncRoot)
                                {
                                    readVal = ReadFromPlc(comm, Address, PLCDataType);
                                }
                                SetReadValue(readVal);
                                Frm_Main.Instance.OutputMsg(
                                    string.Format("[PLCComm] 工具 [{0}] 读取 [{1}] = {2}", toolName, Address, readVal), Color.Black);
                            }
                        }
                        catch (Exception ex)
                        {
                            toolRunStatu = ToolRunStatu.未知原因;
                            Frm_Main.Instance.OutputMsg(
                                string.Format("[PLCComm] 工具 [{0}] 读取失败：{1}", toolName, ex.Message), Color.Red);
                            return;
                        }
                    }

                    toolRunStatu = ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        // ── 内部辅助 ────────────────────────────────────────────────────────

        private void SetReadValue(string readVal)
        {
            toolPar.ResultPar.读取值 = readVal;
            Action<string> handler = ReadValueChanged;
            if (handler != null)
                handler(readVal);
        }

        internal void ApplyLastInputIfEmpty()
        {
            try
            {
                if (!string.IsNullOrEmpty(PLCDeviceName) ||
                    !string.IsNullOrEmpty(Address) ||
                    !string.IsNullOrEmpty(ExpectValue) ||
                    !string.IsNullOrEmpty(toolPar.InputPar.写入值) ||
                    PLCDataType != "Float" ||
                    OpMode != "读取" ||
                    StringLength != 32)
                    return;

                LoadLastInput();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal void SaveLastInput()
        {
            try
            {
                string path = LastInputPath;
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllLines(path, new string[]
                {
                    "PLCDeviceName=" + Encode(PLCDeviceName),
                    "Address=" + Encode(Address),
                    "PLCDataType=" + Encode(PLCDataType),
                    "OpMode=" + Encode(OpMode),
                    "StringLength=" + StringLength.ToString(),
                    "ExpectValue=" + Encode(ExpectValue),
                    "WriteValue=" + Encode(toolPar.InputPar.写入值)
                }, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void LoadLastInput()
        {
            string path = LastInputPath;
            if (!File.Exists(path))
                return;

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            for (int i = 0; i < lines.Length; i++)
            {
                int index = lines[i].IndexOf('=');
                if (index <= 0)
                    continue;

                string key = lines[i].Substring(0, index);
                string value = lines[i].Substring(index + 1);
                switch (key)
                {
                    case "PLCDeviceName":
                        PLCDeviceName = Decode(value);
                        break;
                    case "Address":
                        Address = Decode(value);
                        break;
                    case "PLCDataType":
                        PLCDataType = Decode(value);
                        break;
                    case "OpMode":
                        OpMode = Decode(value);
                        break;
                    case "StringLength":
                    {
                        ushort length;
                        if (ushort.TryParse(value, out length))
                            StringLength = length;
                        break;
                    }
                    case "ExpectValue":
                        ExpectValue = Decode(value);
                        break;
                    case "WriteValue":
                        toolPar.InputPar.写入值 = Decode(value);
                        break;
                }
            }
        }

        private static string LastInputPath
        {
            get { return Path.Combine(Application.StartupPath, "Config", "PLCCommToolLast.ini"); }
        }

        private static string Encode(string value)
        {
            if (value == null)
                value = string.Empty;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        private static string Decode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
        private string ReadFromPlc(CipCommunication comm, string address, string dataType)
        {
            switch (dataType)
            {
                case "Bool":
                {
                    var r = comm.ReadBool(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content ? "1" : "0";
                }
                case "Int16":
                {
                    var r = comm.ReadInt16(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content.ToString();
                }
                case "Int32":
                {
                    var r = comm.ReadInt32(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content.ToString();
                }
                case "Float":
                {
                    var r = comm.ReadFloat(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content.ToString();
                }
                case "Double":
                {
                    var r = comm.ReadDouble(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content.ToString();
                }
                case "String":
                {
                    var r = comm.ReadString(address, StringLength);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content;
                }
                default:
                    throw new Exception("未知数据类型：" + dataType);
            }
        }

        private void WriteToPlc(CipCommunication comm, string address, string dataType, string value)
        {
            HslCommunication.OperateResult result;
            switch (dataType)
            {
                case "Bool":
                    result = comm.WriteBool(address, value == "1" || value.ToLower() == "true");
                    break;
                case "Int16":
                    result = comm.WriteInt16(address, short.Parse(value));
                    break;
                case "Int32":
                    result = comm.WriteInt32(address, int.Parse(value));
                    break;
                case "Float":
                    result = comm.WriteFloat(address, float.Parse(value));
                    break;
                case "Double":
                    result = comm.WriteDouble(address, double.Parse(value));
                    break;
                case "String":
                    result = comm.WriteString(address, value);
                    break;
                default:
                    throw new Exception("未知数据类型：" + dataType);
            }
            if (!result.IsSuccess)
                throw new Exception(result.Message);
        }

        // ── 参数类 ──────────────────────────────────────────────────────────

        [Serializable]
        public class ToolPar : ToolParBase
        {
            private InputPar _inputPar = new InputPar();
            public InputPar InputPar
            {
                get { return _inputPar; }
                set { _inputPar = value; }
            }

            private ResultPar _resultPar = new ResultPar();
            public ResultPar ResultPar
            {
                get { return _resultPar; }
                set { _resultPar = value; }
            }
        }

        [Serializable]
        public class InputPar
        {
            private string _写入值 = string.Empty;
            public string 写入值
            {
                get { return _写入值; }
                set { _写入值 = value; }
            }
        }

        [Serializable]
        public class ResultPar
        {
            private string _读取值 = string.Empty;
            public string 读取值
            {
                get { return _读取值; }
                set { _读取值 = value; }
            }
        }
    }
}
