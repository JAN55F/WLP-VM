using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using Basler.Pylon;
using VMPro.Properties;
using Ookii.Dialogs.WinForms;
using VMPro;
using System.Text.RegularExpressions;

namespace VMPro
{
    [Serializable]
    class EthernetReceiveTool : ToolBase
    {

        /// <summary>
        /// 通讯端口名称
        /// </summary>
        internal string EthernetName = string.Empty;
        internal string trigCMD = "T";
        internal string endChar = string.Empty;
        /// <summary>
        /// 工具参数
        /// </summary>
        internal ToolPar toolPar = new ToolPar();
        internal volatile bool quitReceive = false;


        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否更新图像</param>
        /// <param name="debugTool">调试工具模式</param>
        public override void Run(bool updateImage, bool debugTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    if (EthernetName == string.Empty)
                    {
                        //自动绑定通讯端
                        if (Project.Instance.L_TCPClient.Count > 0)
                        {
                            EthernetName = Project.Instance.L_TCPClient[0].Name;
                            Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 未指定通讯端，已自动绑定到通讯端 [{1}]", toolName, EthernetName), Color.Black);
                        }
                        else
                        {
                            toolRunStatu = ToolRunStatu.未指定以太网通讯端;
                            return;
                        }
                    }
                    if (trigCMD == string.Empty)
                    {
                        toolRunStatu = ToolRunStatu.未指定以太网触发命令;
                        return;
                    }

                    for (int i = 0; i < Project.Instance.L_TCPClient.Count; i++)
                    {
                        if (Project.Instance.L_TCPClient[i].Name == EthernetName)
                        {
                            if (!Project.Instance.L_TCPClient[i].FindSocketByName().Connected)
                            {
                                toolRunStatu = ToolRunStatu.未建立通讯连接;
                                return;
                            }
                            Frm_Main.Instance.OutputMsg(string.Format("[{0}] 等待远程命令中......", EthernetName), Color.Black);
                            while (Project.Instance.L_TCPClient[i].receivedStr != trigCMD + endChar)
                            {
                                Thread.Sleep(10);
                                if (quitReceive)        //放弃继续等待接收消息
                                {
                                    quitReceive = false;
                                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                                    return;
                                }
                            }
                            Frm_Main.Instance.OutputMsg(string.Format("[{0}] 接收到远程命令：{1}", EthernetName, trigCMD), Color.Black);
                            Project.Instance.L_TCPClient[i].receivedStr = string.Empty;
                            break;
                        }
                    }
                    for (int i = 0; i < Project.Instance.L_TCPSever.Count; i++)
                    {
                        if (Project.Instance.L_TCPSever[i].Name == EthernetName)
                        {
                            //////if (!Project.Instance.L_TCPSever[i].listened)
                            //////{
                            //////    toolRunStatu = ToolRunStatu.未建立连接;
                            //////    return;
                            //////}
                            Frm_Main.Instance.OutputMsg(string.Format("[{0}] 等待远程命令中......", EthernetName), Color.Black);
                            while (Project.Instance.L_TCPSever[i].receivedStr != trigCMD + endChar)
                            {
                                Thread.Sleep(10);
                                if (quitReceive)        //放弃继续等待接收消息
                                {
                                    quitReceive = false;
                                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                                    return;
                                }
                            }
                            Frm_Main.Instance.OutputMsg(string.Format("[{0}] 接收到远程命令：{1}", EthernetName, trigCMD), Color.Black);
                            Project.Instance.L_TCPSever[i].receivedStr = string.Empty;
                            break;
                        }
                    }
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        #region 参数
        [Serializable]
        public class ToolPar : ToolParBase
        {
            private InputPar _inputPar = new InputPar();
            public InputPar InputPar
            {
                get { return _inputPar; }
                set { _inputPar = value; }
            }

            private RunPar _runPar = new RunPar();
            public RunPar RunPar
            {
                get { return _runPar; }
                set { _runPar = value; }
            }

            private ResultPar _resultPar = new ResultPar();
            public ResultPar ResultPar
            {
                get { return _resultPar; }
                set { _resultPar = value; }
            }
        }
        [Serializable]
        public class InputPar { }
        [Serializable]
        public class RunPar { }
        [Serializable]
        internal class ResultPar { }
        #endregion

    }
}
