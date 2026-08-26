using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using gts;
using System.Diagnostics;
using System.Threading;
using VMPro.Properties;

namespace VMPro
{
    /// <summary>
    /// 固高GTS系列板卡
    /// </summary>
    internal class Card_Googol : CardBase
    {

        /// <summary>
        /// 初始化板卡
        /// </summary>
        internal static void Init()
        {
            try
            {
                //虚拟相关
                if (Project .Instance .configuration .vitualCard)
                {
                    foreach (Di item in Enum.GetValues(typeof(Di)))
                    {
                        D_inputSignalVitualStatu.Add(item.ToString(), "01");
                    }
                    foreach (Do item in Enum.GetValues(typeof(Do)))
                    {
                        D_outputSingalVitualStatu.Add(item.ToString(), Level .Low );
                    }
                    return;
                }

                //初始化板卡
                short statu = 0;
                statu += mc.GT_Open(0, 1);
                statu += mc.GT_Reset();
                statu += mc.GT_LoadConfig(Application.StartupPath + "\\GTS400.cfg");
                statu += mc.GT_ClrSts(1, 4);
                statu += mc.GT_HomeInit();
                statu += mc.GT_AxisOn(1);
                statu += mc.GT_AxisOn(2);
                statu += mc.GT_AxisOn(3);
                statu += mc.GT_AxisOn(4);
                statu = mc.GT_SetCaptureMode(1, 1);
                statu = mc.GT_SetCaptureMode(1, 1);
                statu = mc.GT_SetCaptureMode(1, 1);
                statu = mc.GT_SetCaptureMode(1, 1);
                if (statu != 0)
                    Frm_MessageBox.Instance.MessageBoxShow("固高GTS运动控制卡初始化失败");
                else
                    initSucceed = true;
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 轴回零
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <param name="searchDistance">搜索长度</param>
        /// <param name="offset">偏移量</param>
        /// <param name="vel">回零速度</param>
        /// <param name="homeDir">回零方向</param>
        /// <param name="backDistance">回退距离</param>
        /// <param name="acc">加速度</param>
        internal static void Home(object axisName, Int32 searchLength, int offset, int vel, HomeDir homeDir, Int32 backLength, short acc)
        {
            try
            {
                Thread th = new Thread(() =>
                {
                    short axisIndex = (short)FindAxisByName(axisName.ToString()).actNo;
                    if (homeDir == HomeDir.N_负方向)       //向负方向回零
                    {
                        //判断各种回零情况
                        int value;
                        uint clock;
                        int statu;
                        mc.GT_ClrSts(1, 4);
                        mc.GT_PrfTrap((short)axisIndex);

                        mc.GT_HomeInit();
                        mc.GT_SetVel((short)axisIndex, vel);
                        mc.GT_GetDi(mc.MC_HOME, out value);
                        if ((value & (1 << (short)(axisIndex - 1))) != 0)       //如果不在原点
                        {
                            mc.GT_GetDi(mc.MC_LIMIT_NEGATIVE, out value);
                            if ((value & (1 << (short)axisIndex - 1)) != 0)       //如果在负极限
                            {
                                mc.TTrapPrm trap;
                                trap.acc = acc;
                                trap.dec = 1;
                                trap.smoothTime = 2;
                                trap.velStart = 0.5;
                                mc.GT_SetTrapPrm((short)axisIndex, ref trap);
                                double curPos;
                                mc.GT_GetPrfPos((short)axisIndex, out curPos, 1, out clock);
                                mc.GT_SetPos((short)axisIndex, Convert.ToInt32(curPos + backLength));
                                mc.GT_Update(1 << (axisIndex - 1));
                                Thread.Sleep(200);
                                do
                                {
                                    gts.mc.GT_GetSts(axisIndex, out statu, 1, out clock);
                                } while ((statu & (1 << 10)) != 0);
                                mc.GT_Home(axisIndex, searchLength, vel, acc, offset);
                            }
                            else         //如果不在负极限 ,在中间区域
                            {
                                mc.GT_Home(axisIndex, searchLength, vel, acc, offset);
                                Thread.Sleep(200);
                                do
                                {
                                    gts.mc.GT_GetSts(axisIndex, out statu, 1, out clock);
                                } while ((statu & (1 << 10)) != 0);
                                mc.GT_GetDi(mc.MC_LIMIT_NEGATIVE, out value);
                                //判断是否在负极限
                                if ((value & (1 << (short)axisIndex - 1)) != 0)        //如果在负限位
                                {
                                    mc.TTrapPrm trap;
                                    trap.acc = acc;
                                    trap.dec = 1;
                                    trap.smoothTime = 2;
                                    trap.velStart = 0.5;
                                    mc.GT_SetTrapPrm(axisIndex, ref trap);
                                    double curPos;
                                    mc.GT_GetPrfPos(axisIndex, out curPos, 1, out clock);
                                    mc.GT_SetPos(axisIndex, Convert.ToInt32(curPos + backLength));
                                    mc.GT_Update(1 << (axisIndex - 1));
                                    Thread.Sleep(200);
                                    do
                                    {
                                        gts.mc.GT_GetSts(axisIndex, out statu, 1, out clock);
                                    } while ((statu & (1 << 10)) != 0);
                                    mc.GT_Home(axisIndex, searchLength, vel, acc, offset);
                                }
                                else
                                {
                                    //如果当前处在原点，回零完成
                                }

                            }
                        }
                        else        //如果在原点
                        {
                            mc.TTrapPrm trap;
                            trap.acc = acc;
                            trap.dec = 1;
                            trap.smoothTime = 2;
                            trap.velStart = 0.5;
                            mc.GT_SetTrapPrm(axisIndex, ref trap);
                            double curPos;
                            mc.GT_GetPrfPos(axisIndex, out curPos, 1, out clock);
                            gts.mc.GT_SetPos(axisIndex, Convert.ToInt32(curPos + 10000));
                            gts.mc.GT_Update(1 << (axisIndex - 1));
                            Thread.Sleep(200);
                            do
                            {
                                mc.GT_GetSts(axisIndex, out statu, 1, out clock);
                            } while ((statu & (1 << 10)) != 0);
                            mc.GT_Home(axisIndex, searchLength, vel, acc, offset);
                        }
                    }
                    else        //向正方向回零
                    {
                        mc.GT_PrfTrap(axisIndex);
                        //判断各种回零情况
                        int value;
                        uint clock;
                        int statu;
                        mc.GT_ClrSts(1, 4);
                        statu = gts.mc.GT_PrfTrap(axisIndex);
                        mc.GT_GetDi(mc.MC_HOME, out value);
                        statu = gts.mc.GT_SetVel(axisIndex, vel);
                        if ((value & (1 << (short)(axisIndex - 1))) != 0)        //如果不在原点
                        {
                            mc.GT_GetDi(mc.MC_LIMIT_POSITIVE, out value);
                            if ((value & (1 << (short)axisIndex - 1)) != 0)         //如果在正极限
                            {
                                mc.TTrapPrm trap;
                                trap.acc = vel;
                                trap.dec = 0.125;
                                trap.smoothTime = 25;
                                trap.velStart = 0.5;
                                statu = gts.mc.GT_SetTrapPrm(axisIndex, ref trap);
                                double curPos;
                                mc.GT_GetPrfPos(axisIndex, out curPos, 1, out clock);
                                mc.GT_SetPos(axisIndex, Convert.ToInt32(curPos - backLength));
                                mc.GT_Update(1 << (axisIndex - 1));
                                Thread.Sleep(200);
                                do
                                {
                                    gts.mc.GT_GetSts(axisIndex, out statu, 1, out clock);
                                } while ((statu & (1 << 10)) != 0);
                                mc.GT_Home(axisIndex, searchLength, vel, acc, offset);
                            }
                            else        //如果不在负极限
                            {
                                //正常回零
                                mc.GT_Home(axisIndex, searchLength, vel, acc, offset);
                                Thread.Sleep(200);
                                do
                                {
                                    gts.mc.GT_GetSts(axisIndex, out statu, 1, out clock);
                                } while ((statu & (1 << 10)) != 0);

                                //判断是否在正极限
                                mc.GT_GetDi(mc.MC_LIMIT_POSITIVE, out value);
                                if ((value & (1 << (short)axisIndex - 1)) != 0)        //如果在正极限
                                {
                                    mc.TTrapPrm trap;
                                    trap.acc = acc;
                                    trap.dec = 1;
                                    trap.smoothTime = 2;
                                    trap.velStart = 0.5;
                                    mc.GT_SetTrapPrm(axisIndex, ref trap);
                                    double curPos;
                                    mc.GT_GetPrfPos(axisIndex, out curPos, 1, out clock);
                                    mc.GT_SetPos(axisIndex, Convert.ToInt32(curPos - backLength));
                                    mc.GT_Update(1 << (axisIndex - 1));
                                    Thread.Sleep(200);
                                    do
                                    {
                                        gts.mc.GT_GetSts(axisIndex, out statu, 1, out clock);
                                    } while ((statu & (1 << 10)) != 0);
                                    mc.GT_Home(axisIndex, searchLength, vel, acc, offset);
                                }
                            }
                        }
                        else        //如果在原点
                        {
                            mc.TTrapPrm trap;
                            trap.acc = acc;
                            trap.dec = 1;
                            trap.smoothTime = 2;
                            trap.velStart = 0.5;
                            mc.GT_SetTrapPrm(axisIndex, ref trap);
                            double curPos;
                            mc.GT_GetPrfPos(axisIndex, out curPos, 1, out clock);
                            gts.mc.GT_SetPos(axisIndex, Convert.ToInt32(curPos - 8000));
                            gts.mc.GT_Update(1 << (axisIndex - 1));
                            Thread.Sleep(200);
                            do
                            {
                                mc.GT_GetSts(axisIndex, out statu, 1, out clock);
                            } while ((statu & (1 << 10)) != 0);
                            mc.GT_Home(axisIndex, searchLength, vel, 1, offset);
                        }
                    }
                    Thread.Sleep(500);
                    mc.GT_ZeroPos(axisIndex, 1);
                });
                th.IsBackground = true;
                th.Start();
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
                    int value;
                    string  diIndex = GetDiIndexByName(diName.ToString());
                    mc.GT_GetDi(mc.MC_GPI, out value);
                    if ((value & (1 << Convert .ToUInt16 ( diIndex) - 1)) != 0)
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
        /// 轴下电
        /// </summary>
        /// <param name="axisName">轴枚举</param>
        internal static void MotorOff(object axisName)
        {
            try
            {
                short axisIndex = (short)FindAxisByName(axisName.ToString()).actNo;
                mc.GT_AxisOff(axisIndex);
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 轴上电
        /// </summary>
        /// <param name="axisName">轴枚举</param>
        internal static void MotorOn(object axisName)
        {
            try
            {
                short axisIndex = (short)FindAxisByName(axisName.ToString()).actNo;
                mc.GT_AxisOn(axisIndex);
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绝对位置移动
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <param name="pos">脉冲数</param>
        internal static void MoveAbs(object axisName, Int32 pos, short vel, bool waitDone = true)
        {
            try
            {
                short axisIndex = (short)FindAxisByName(axisName.ToString()).actNo;
                mc.GT_SetVel(axisIndex, vel);
                mc.GT_SetPos(axisIndex, pos);     //传入的是毫米，此处需要的是脉冲，待更正
                mc.GT_Update(1 << (axisIndex - 1));
                Thread.Sleep(100);
                if (waitDone)
                    WaitMoveDone(axisIndex);
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 等待信号到达
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
        /// <summary>
        /// 获取当前编码器位置
        /// </summary>
        /// <param name="axisName">轴枚举</param>
        /// <returns>当前位置</returns>
        internal static double GetCurEncoder(object axisName)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)
                    return 0;

                double curPos;
                uint clock;
                short axisIndex = (short)FindAxisByName(axisName.ToString()).actNo;
                mc.GT_GetPrfPos(axisIndex, out curPos, 1, out clock);
                return curPos;
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
                return 0;
            }
        }
        /// <summary>
        /// 获取当前命令位置
        /// </summary>
        /// <param name="axisName">轴枚举</param>
        /// <returns>当前位置</returns>
        internal static double GetCurPosition(object axisName)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)
                    return 0;

                double curPos;
                uint clock;
                short axisIndex = (short)FindAxisByName(axisName.ToString()).actNo;
                mc.GT_GetPrfPos(axisIndex, out curPos, 1, out clock);       //此处有误，待修复
                return 0;
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
                return 0;
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
                if (Project .Instance .configuration .vitualCard)
                {
                    D_outputSingalVitualStatu[doName.ToString()] = level;
                    return;
                }

                string  doIndex = Card_Googol.GetDoIndexByName(doName.ToString());
                if (level == Level.High)
                    mc.GT_SetDoBit(mc.MC_GPO,Convert .ToInt16  (  doIndex), 1);
                else
                    mc.GT_SetDoBit(mc.MC_GPO, Convert.ToInt16(doIndex), 0);
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 获取通用输出状态
        /// </summary>
        /// <param name="diName">输入枚举</param>
        /// <returns>输入状态</returns>
        internal static Level GetDoSts(object doName)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)        //如果板卡虚拟，则一律返回低电平
                    return D_outputSingalVitualStatu[doName.ToString()];

                int value;
                string  doIndex = GetDoIndexByName(doName.ToString());
                mc.GT_GetDi(mc.MC_GPI, out value);
                if ((value & (1 << Convert .ToUInt16 ( doIndex))) == 0)
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
        /// 等待运动完成
        /// </summary>
        /// <param name="axisIndex">轴编号</param>
        internal static void WaitMoveDone(short axisIndex)
        {
            try
            {
                int statu;
                uint clock;
                do
                {
                    gts.mc.GT_GetSts(axisIndex, out statu, 1, out clock);
                } while ((statu & (1 << 10)) != 0);
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }

    }
}
