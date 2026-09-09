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
    internal class Scaner_KenyenceTool : ToolBase
    {

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
        /// <summary>
        /// 解码字符串
        /// </summary>
        internal string resultStr = string.Empty;

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
                    Frm_KenyenceScanerTool.Instance.lbl_statu.Text = "已打开";
                    Frm_KenyenceScanerTool.Instance.lbl_statu.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                Frm_KenyenceScanerTool.Instance.lbl_statu.Text = "未打开";
                Frm_KenyenceScanerTool.Instance.lbl_statu.ForeColor = Color.Red;
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
                    Frm_KenyenceScanerTool.Instance.lbl_statu.Text = "未打开";
                    Frm_KenyenceScanerTool.Instance.lbl_statu.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 扫描一次
        /// </summary>
        internal void ScanOnce()
        {
            Send("LON");
            Read();
        }
        /// <summary>
        /// 停止扫描
        /// </summary>
        internal void StopScan()
        {
            Send("LOFF");
            Read();
        }
        /// <summary>
        /// 向串口发送数据
        /// </summary>
        /// <param name="cmd"></param>
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
                    Frm_Output.Instance.OutputMsg("不能发送空字符串", Color.Red);
                    return;
                }
                serialPort.ReadExisting();
                serialPort.WriteLine(cmd);
                Frm_KenyenceScanerTool.Instance.tbx_log.Text += DateTime.Now.ToString("HH:mm:ss") + "->:  " + cmd + Environment.NewLine;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 从串口接收数据
        /// </summary>
        /// <returns></returns>
        internal string Read()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    Frm_Output.Instance.OutputMsg("串口未打开，请打开后重试", Color.Red);
                    return string.Empty;
                }
                Thread.Sleep(100);
                string result = serialPort.ReadExisting();
                Frm_KenyenceScanerTool.Instance.tbx_log.Text += DateTime.Now.ToString("HH:mm:ss") + "->:  " + result + Environment.NewLine;
                return result;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否刷新图像</param>
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            try
            {
                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                if (!serialPort.IsOpen)
                    OpenPort();
                serialPort.ReadExisting();
                resultStr = string.Empty;
                serialPort.WriteLine("LON");
                for (int i = 0; i < 3 && (string.IsNullOrEmpty(resultStr) || resultStr == "\rERROR\r"); i++)
                {
                    resultStr = serialPort.ReadExisting();
                    if ((!string.IsNullOrEmpty(resultStr)) && resultStr[0] == '\r' && resultStr[resultStr.Length - 1] == '\r' && resultStr != "\rERROR\r")
                        break;
                    else
                        Thread.Sleep(80);
                }
                if (string.IsNullOrEmpty(resultStr) || resultStr == "\rERROR\r")
                {
                    serialPort.Write("LOFF\r");
                    resultStr = string.Empty;
                    if (Frm_KenyenceScanerTool.Instance.Visible)
                        Frm_KenyenceScanerTool.Instance.tbx_log.AppendText("未扫描到条码");
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.未扫描到条码);
                    return;
                }
                if (resultStr.EndsWith("\r"))
                    resultStr = resultStr.Substring(0, resultStr.Length - 1);
                if (Frm_KenyenceScanerTool.Instance.Visible)
                    Frm_KenyenceScanerTool.Instance.tbx_log.AppendText("扫描到：" + resultStr + Environment.NewLine);
                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
