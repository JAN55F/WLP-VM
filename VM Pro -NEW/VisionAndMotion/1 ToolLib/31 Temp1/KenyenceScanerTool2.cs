using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using HalconDotNet;
using System.IO.Ports;
using System.Threading;

namespace VMPro
{
    [Serializable]
    internal class KenyenceScanerTool2 : ToolBase
    {

        /// <summary>
        /// 输入图像
        /// </summary>
        internal string resultStr = string.Empty;
        /// <summary>
        /// 流程名
        /// </summary>
        internal string jobName = string.Empty;


        /// <summary>
        /// 通讯用串口类
        /// </summary>
        internal static SerialPort serialPort = new SerialPort();
        /// <summary>
        /// 串口号
        /// </summary>
        internal string portName = "COM1";
        /// <summary>
        /// 波特率
        /// </summary>
        internal int baudRate = 115200;
        /// <summary>
        /// 数据位
        /// </summary>
        internal int dataBit = 8;
        /// <summary>
        /// 停止位
        /// </summary>
        internal StopBits stopBit = (StopBits)Enum.Parse(typeof(StopBits), "One");
        /// <summary>
        /// 奇偶效验位
        /// </summary>
        internal Parity parity = (Parity)Enum.Parse(typeof(Parity), "Even");


        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n串口未打开，请打开后重试");
                    return;
                }
                Thread.Sleep(100);
                string receiveStr = serialPort.ReadExisting();
                serialPort.DiscardInBuffer();
                if (receiveStr != "")
                {
                    Frm_KenyenceScanerTool2.Instance.tbx_output.Text += DateTime.Now.ToString("HH:mm:ss") + "<-:  " + receiveStr + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        internal void OpenPort()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.NewLine = "\r\n";
                    serialPort.RtsEnable = false;
                    serialPort.PortName = portName;
                    serialPort.BaudRate = baudRate;
                    serialPort.DataBits = dataBit;
                    serialPort.StopBits = stopBit;
                    serialPort.Parity = parity;
                    serialPort.Open();
                    //    serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                    Frm_KenyenceScanerTool2.Instance.tbx_output.Text = "打开串口成功";
                    Frm_KenyenceScanerTool2.Instance.lbl_statu.Text = "已打开";
                    Frm_KenyenceScanerTool2.Instance.lbl_statu.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                Frm_KenyenceScanerTool2.Instance.lbl_statu.Text = "未打开";
                Frm_KenyenceScanerTool2.Instance.lbl_statu.ForeColor = Color.Red;
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        internal void ClosePort()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    Frm_KenyenceScanerTool2.Instance.lbl_statu.Text = "未打开";
                    Frm_KenyenceScanerTool2.Instance.lbl_statu.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal void Send(string cmd)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    OpenPort();
                }
                if (cmd == string.Empty)
                {
                    Frm_MessageBox messageBox = new Frm_MessageBox();
                    messageBox.MessageBoxShow("\r\n不能发送空字符串");
                    return;
                }
                serialPort.ReadExisting();
                serialPort.WriteLine(cmd);
                Frm_KenyenceScanerTool2.Instance.tbx_output.Text += DateTime.Now.ToString("HH:mm:ss") + "->:  " + cmd + Environment.NewLine;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 工具恢复到初始状态
        /// </summary>
        internal void ResetTool()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal string Read()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    Frm_MessageBox messageBox = new Frm_MessageBox();
                    messageBox.MessageBoxShow("\r\n串口未打开，请打开后重试");
                    return string.Empty;
                }
                Thread.Sleep(100);
                string result = serialPort.ReadExisting();
                Frm_KenyenceScanerTool2.Instance.tbx_output.Text += DateTime.Now.ToString("HH:mm:ss") + "->:  " + result + Environment.NewLine;
                return result;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 清空上次运行的所有输入
        /// </summary>
        internal void ClearLastInput()
        {
            try
            {
                resultStr = String.Empty;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否刷新图像</param>
        public override void Run(bool updateImage, bool b, string toolName)
        {
            try
            {
                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                // serialPort.DiscardInBuffer();
                if (!serialPort.IsOpen)
                    OpenPort();
                string sz = "";
                int i = 0;
                serialPort.ReadExisting();
                resultStr = string.Empty;
                serialPort.WriteLine("LON");
                for (i = 0; i < 10 && (string.IsNullOrEmpty(sz) || sz == "\rERROR\r"); i++)
                {
                    sz = serialPort.ReadExisting();
                    if ((!string.IsNullOrEmpty(sz)) && sz[0] == '\r' && sz[sz.Length - 1] == '\r' && sz != "\rERROR\r")
                    {
                        resultStr = sz;
                    }
                    else Thread.Sleep(100);
                }
                if (string.IsNullOrEmpty(sz) || sz == "\rERROR\r")
                {
                    serialPort.Write("LOFF\r");
                    sz = "";
                    resultStr = string.Empty;
                    if (Machine.machineRunStatu != MachineRunStatu.Running)
                        Frm_KenyenceScanerTool.Instance.tbx_log.AppendText("扫描到：" + sz + Environment.NewLine);
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.未知原因);
                    return;
                }
                if (sz.EndsWith("\r"))
                    sz = sz.Substring(0, sz.Length - 1);
                resultStr = sz;
                if (Machine.machineRunStatu != MachineRunStatu.Running)
                    Frm_KenyenceScanerTool2.Instance.tbx_output.AppendText("扫描到：" + sz + Environment.NewLine);
                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
