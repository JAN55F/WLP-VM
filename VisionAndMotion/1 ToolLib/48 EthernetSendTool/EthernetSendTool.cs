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
    class EthernetSendTool : ToolBase
    {

        internal string endChar = string.Empty;
        /// <summary>
        /// 通讯端口名称
        /// </summary>
        internal string EthernetName = string.Empty;
        /// <summary>
        /// 工具参数
        /// </summary>
        internal ToolPar toolPar = new ToolPar();


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
                            Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 通讯端未绑定，已自动绑定到通讯端 [{1}]", toolName, EthernetName), Color.Orange);
                        }
                        else
                        {
                            toolRunStatu = ToolRunStatu.未指定以太网通讯端;
                            return;
                        }
                    }
                    if (toolPar.InputPar.消息 == string.Empty)
                    {
                        toolRunStatu = ToolRunStatu.未指定以太网触发命令;
                        return;
                    }

                    //先按TCP客户端设备匹配（原逻辑）
                    bool matched = false;
                    for (int i = 0; i < Project.Instance.L_TCPClient.Count; i++)
                    {
                        if (Project.Instance.L_TCPClient[i].Name == EthernetName)
                        {
                            matched = true;
                            if (!Project.Instance.L_TCPClient[i].FindSocketByName().Connected)
                            {
                                toolRunStatu = ToolRunStatu.未建立通讯连接;
                                return;
                            }
                            Project.Instance.L_TCPClient[i].Send(toolPar.InputPar.消息 + endChar);
                            Frm_Main.Instance.OutputMsg(string.Format("[{0}] 已发送远程命令：{1}", EthernetName, toolPar.InputPar.消息), Color.Black);
                            break;
                        }
                    }

                    //再按TCP服务端设备匹配：向服务端当前第一个已连接的客户端发送
                    if (!matched)
                    {
                        for (int i = 0; i < Project.Instance.L_TCPSever.Count; i++)
                        {
                            if (Project.Instance.L_TCPSever[i].Name == EthernetName)
                            {
                                matched = true;
                                if (!Project.Instance.L_TCPSever[i].SendToFirstClient(toolPar.InputPar.消息 + endChar))
                                {
                                    //服务端没有监听或没有客户端接入
                                    toolRunStatu = ToolRunStatu.未建立通讯连接;
                                    Frm_Main.Instance.OutputMsg(string.Format("TCP服务端 [{0}] 当前没有已连接的客户端，发送失败", EthernetName), Color.Red);
                                    return;
                                }
                                Frm_Main.Instance.OutputMsg(string.Format("[{0}] 已发送远程命令：{1}", EthernetName, toolPar.InputPar.消息), Color.Black);
                                break;
                            }
                        }
                    }

                    //两边都匹配不到：报错而不是静默假成功
                    if (!matched)
                    {
                        toolRunStatu = ToolRunStatu.未指定以太网通讯端;
                        Frm_Main.Instance.OutputMsg(string.Format("工具 [{0}] 未找到通讯端 [{1}]，发送失败", toolName, EthernetName), Color.Red);
                        return;
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
        public class InputPar
        {
            private string _消息 = string.Empty;
            public string 消息
            {
                get { return _消息; }
                set { _消息 = value; }
            }
        }
        [Serializable]
        public class RunPar { }
        [Serializable]
        internal class ResultPar { }
        #endregion

    }
}
