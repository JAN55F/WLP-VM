using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace VMPro
{
    /// <summary>
    /// Log记录辅助类
    /// </summary>
    internal class Log
    {

        /// <summary>
        /// 通讯文件锁，防止文件冲突
        /// </summary>
        private static object objComm = new object();
        /// <summary>
        /// 异常文件锁，防止文件冲突
        /// </summary>
        private static object objError = new object();
        /// <summary>
        /// 数据信息锁，防止文件冲突
        /// </summary>
        private static object objData = new object();
        /// <summary>
        /// 操作信息锁，防止文件冲突
        /// </summary>
        private static object objOperate = new object();


        /// <summary>
        /// Log保存函数
        /// </summary>
        /// <param name="logType">Log信息类型</param>
        /// <param name="message">信息内容</param>
        internal static void SaveLog(LogType logType, string message)
        {
            try
            {
                switch (logType)
                {
                    case LogType.Comm:
                        lock (objComm)
                        {
                            DateTime now = DateTime.Now;
                            string filePath = Path.Combine(
                                Project.Instance.configuration.dataPath,
                                "Log",
                                "Comm",
                                now.ToString("yyyyMMdd"));
                            string fileName = now.ToString("HH时") + ".txt";         //每小时创建一个txt，防止通讯数据交换频率高，数据量大，文本文件过大
                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);
                            string fullPath = Path.Combine(filePath, fileName);
                            if (!File.Exists(fullPath))
                                File.Create(fullPath).Close();
                            File.AppendAllText(fullPath, DateTime.Now.ToString("yyyy/MM/dd mm:HH:ss    ") + message + Environment.NewLine);
                        }
                        break;
                    case LogType.Operate :
                        lock (objOperate)
                        {
                            DateTime now1 = DateTime.Now;
                            string filePath1 = string.Format("{0}\\Log\\Operate\\", Project.Instance .configuration .dataPath );
                            string fileName1 = now1.ToString("yy_MM_dd") + ".txt";
                            if (!Directory.Exists(filePath1))
                                Directory.CreateDirectory(filePath1);
                            if (!File.Exists(filePath1 + fileName1))
                                File.Create(filePath1 + fileName1).Close();
                            File.AppendAllText(filePath1 + fileName1, DateTime.Now.ToString("yyyy/MM/dd mm:HH:ss    ") + message + Environment.NewLine);
                    
                        }
                        break;
                    case LogType.Data:
                        lock (objData)
                        {
                            //此处待添加
                        }
                        break;
                }
            }
            catch (Exception es)
            {
                //////System.Windows.Forms.MessageBox.Show("存储日志文件异常\r\n" + es.ToString(), "提示：");
            }
        }
        /// <summary>
        /// Log保存函数
        /// </summary>
        /// <param name="ex">异常对象</param>
        internal static void SaveError(Exception ex)
        {
            SaveError(ex, null);
        }

        /// <summary>
        /// 保存带业务上下文的异常。定位信息必须来自异常自己的 StackTrace，不能记录
        /// SaveError 调用点，否则连续运行日志只能看到日志函数本身，无法定位具体工具。
        /// </summary>
        internal static void SaveError(Exception ex, string context)
        {
            try
            {
                lock (objError)
                {
                    if (ex == null)
                        return;

                    StackTrace trace = new StackTrace(ex, true);
                    StackFrame tmpSF = trace.GetFrames() == null
                        ? null
                        : trace.GetFrames().FirstOrDefault(frame => !string.IsNullOrEmpty(frame.GetFileName()));
                    if (tmpSF == null)
                        tmpSF = trace.GetFrame(0);
                    string fileName = tmpSF == null || string.IsNullOrEmpty(tmpSF.GetFileName())
                        ? "未知"
                        : tmpSF.GetFileName();
                    string methodName = ex.TargetSite == null ? "未知" : ex.TargetSite.Name;
                    int lineNumber = tmpSF == null ? 0 : tmpSF.GetFileLineNumber();
                    int columnNumber = tmpSF == null ? 0 : tmpSF.GetFileColumnNumber();
                    string data = "----------     " + DateTime.Now.ToString() + "     ----------" + Environment.NewLine +
                                  "业务上下文：" + (string.IsNullOrEmpty(context) ? "未提供" : context) + Environment.NewLine +
                                  "异常类型：" + ex.GetType().FullName + Environment.NewLine +
                                  "出错文件：" + fileName + Environment.NewLine +
                                  "出错函数：" + methodName + Environment.NewLine +
                                  "出错行号：" + (lineNumber > 0 ? lineNumber.ToString() : "未知") + Environment.NewLine +
                                  "出错列号：" + (columnNumber > 0 ? columnNumber.ToString() : "未知") + Environment.NewLine +
                                  "完整异常：" + ex.ToString() + Environment.NewLine;
                    data += Environment.NewLine;
                    string ErrorPath = Project.Instance.configuration.dataPath + "\\Config\\Log\\Error";
                    if (!Directory.Exists(ErrorPath))
                    {
                        Directory.CreateDirectory(ErrorPath);
                    }
                    string path = ErrorPath + "\\" + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
                    if (!File.Exists(path))
                    {
                        File.Create(path).Close();
                    }
                    File.AppendAllText(path, data);
                }
            }
            catch (Exception es)
            {
                //////System.Windows.Forms.MessageBox.Show("存储日志文件异常\r\n" + es.ToString(), "提示：");
            }
        }

        /// <summary>
        /// 保存错误并显示用户友好的错误消息，防止异常被吞掉导致应用状态异常
        /// </summary>
        internal static void SaveErrorAndShow(Exception ex, string userMessage = null, string context = null)
        {
            try
            {
                SaveError(ex, context);
                if (userMessage == null)
                    userMessage = Project.Instance.configuration.language == Language.English 
                        ? "An error occurred, please check the log file for details." 
                        : "发生错误，请查看日志文件了解详情。";
                if (Frm_Main.Instance != null && !Frm_Main.Instance.IsDisposed && Frm_Main.Instance.Visible)
                    Frm_Main.Instance.OutputMsg(userMessage, System.Drawing.Color.Red);
                else
                    Frm_MessageBox.Instance.MessageBoxShow(userMessage);
            }
            catch
            {
                // 防止显示错误消息时再次抛异常
                System.Windows.Forms.MessageBox.Show(userMessage ?? "An error occurred");
            }
        }

    }
    /// <summary>
    /// Log信息类型：通讯信息|异常信息|生产数据
    /// </summary>
    internal enum LogType
    {
        Comm,
        Error,
        Data,
        Operate,
    }
}
