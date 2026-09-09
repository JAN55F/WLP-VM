using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csIOC0640;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using VMPro.Properties;

namespace VMPro
{
    /// <summary>
    /// 雷赛IOC0640系列IO卡
    /// </summary>
    internal class Card_IOC0640 : CardBase
    {

        /// <summary>
        /// 初始化板卡
        /// </summary>
        internal static void Init()
        {
            try
            {
                int cardCount = 0;
                cardCount = IOC0640.ioc_board_init();
                if (cardCount <= 0)
                {
                    Machine.ShowMessageOnMainUiThread("未识别到雷赛IOC0640运动控制卡");
                    initSucceed = false;
                }
                else
                {
                    initSucceed = true;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 获取通用输入状态
        /// </summary>
        /// <param name="diName">输入枚举</param>
        /// <returns>输入状态</returns>
        internal static Level GetDiSts(object diName)
        {
            try
            {
                if (D_inputSignalVitualStatu[diName.ToString()].Substring(0, 1) == "1")       //表示该输入信号当前处于虚拟状态
                {
                    if (D_inputSignalVitualStatu[diName.ToString()].Substring(1, 1) == "1")        //表示虚拟为高电平
                        return Level.High;
                    else
                        return Level.Low;
                }
                else if (Project .Instance .configuration .vitualCard)        //如果板卡虚拟，则一律返回低电平
                {
                    return Level.Low;
                }
                else
                {
                    string  diIndex = GetDiIndexByName(diName.ToString());
                    int value = IOC0640.ioc_read_inbit((ushort)0, Convert .ToUInt16 ( diIndex));
                    if (value == 1)
                        return Level.Low;
                    else
                        return Level.High;
                }
            }
            catch
            {
                return Level.Low;
            }
        }
        /// <summary>
        /// 获取通用输出状态
        /// </summary>
        /// <param name="diName">输入枚举</param>
        /// <returns>输入的状态</returns>
        internal static Level GetDoSts(object doName)
        {
            try
            {
                if (Project.Instance.configuration.vitualCard)        //如果板卡虚拟，则一律返回低电平
                    return D_outputSingalVitualStatu[doName.ToString()];

                string  doIndex = GetDoIndexByName(doName.ToString());
                int value = IOC0640.ioc_read_outbit((ushort)0, Convert .ToUInt16 ( doIndex));
                if (value == 1)
                    return Level.Low;
                else
                    return Level.High;
            }
            catch
            {
                return Level.Low;
            }
        }
        /// <summary>
        /// 通用输出操作
        /// </summary>
        /// <param name="doName">输出枚举</param>
        /// <param name="level">高低电平</param>
        internal static void SetDo(object doName, Level level)
        {
            try
            {
                if (Project.Instance.configuration.vitualCard)
                {
                    D_outputSingalVitualStatu[doName.ToString()] = level;
                    return;
                }

                string  doIndex = GetDoIndexByName(doName.ToString());
                if (level == Level.High)
                    IOC0640.ioc_write_outbit(0, Convert .ToUInt16 ( doIndex), 0);
                else
                    IOC0640.ioc_write_outbit(0, Convert .ToUInt16 ( doIndex), 1);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 等待信号
        /// </summary>
        /// <param name="diName">输入枚举</param>
        /// <param name="level">高低电平</param>
        internal static void WaitDi(object diName, Level level)
        {
            try
            {
                Level statu;
                do
                {
                    statu = GetDiSts(diName);
                    Thread.Sleep(10);
                }
                while (statu != level);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
