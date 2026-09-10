using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMPro
{
    /// <summary>
    /// 三色灯控制类
    /// </summary>
    internal class ThreeColorLamp
    {

        /// <summary>
        /// 三色灯亮红色
        /// </summary>
        internal static void SetRed()
        {
            try
            {
                switch (Project .Instance .configuration .cardType)
                {
                    case CardType.固高_GTS:
                        Card_Googol.SetDo(Do.三色灯_红, Level.High);
                        Card_Googol.SetDo(Do.三色灯_蜂鸣, Level.High);
                        Card_Googol.SetDo(Do.三色灯_黄, Level.Low);
                        Card_Googol.SetDo(Do.三色灯_绿, Level.Low);
                        break;
                    case CardType.雷赛_IOC0640:
                        Card_IOC0640.SetDo(Do.三色灯_红, Level.High);
                        Card_IOC0640.SetDo(Do.三色灯_蜂鸣, Level.High);
                        Card_IOC0640.SetDo(Do.三色灯_黄, Level.Low);
                        Card_IOC0640.SetDo(Do.三色灯_绿, Level.Low);
                        break;
                }
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 三色灯亮黄色
        /// </summary>
        internal static void SetYellow()
        {
            try
            {
                switch (Project .Instance .configuration .cardType)
                {
                    case CardType.固高_GTS:
                        Card_Googol.SetDo(Do.三色灯_黄, Level.High);
                        Card_Googol.SetDo(Do.三色灯_蜂鸣, Level.Low);
                        Card_Googol.SetDo(Do.三色灯_红, Level.Low);
                        Card_Googol.SetDo(Do.三色灯_绿, Level.Low);
                        break;
                    case CardType.雷赛_IOC0640:
                        Card_IOC0640.SetDo(Do.三色灯_黄, Level.High);
                        Card_IOC0640.SetDo(Do.三色灯_蜂鸣, Level.Low);
                        Card_IOC0640.SetDo(Do.三色灯_红, Level.Low);
                        Card_IOC0640.SetDo(Do.三色灯_绿, Level.Low);
                        break;
                }
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 三色灯亮绿色
        /// </summary>
        internal static void SetGreen()
        {
            try
            {
                switch (Project .Instance .configuration .cardType)
                {
                    case CardType.固高_GTS:
                        Card_Googol.SetDo(Do.三色灯_绿, Level.High);
                        Card_Googol.SetDo(Do.三色灯_蜂鸣, Level.Low);
                        Card_Googol.SetDo(Do.三色灯_红, Level.Low);
                        Card_Googol.SetDo(Do.三色灯_黄, Level.Low);
                        break;
                    case CardType.雷赛_IOC0640:
                        Card_IOC0640.SetDo(Do.三色灯_绿, Level.High);
                        Card_IOC0640.SetDo(Do.三色灯_蜂鸣, Level.Low);
                        Card_IOC0640.SetDo(Do.三色灯_红, Level.Low);
                        Card_IOC0640.SetDo(Do.三色灯_黄, Level.Low);
                        break;
                }
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }

    }
}
