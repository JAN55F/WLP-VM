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
using csDmc2410;

namespace VMPro
{
    /// <summary>
    /// 固高GTS系列板卡
    /// </summary>
    internal class Card_LeadShine_DMC2410 : CardBase
    {

        /// <summary>
        /// 初始化板卡
        /// </summary>
        internal static void Init()
        {
            try
            {
                //虚拟相关
                foreach (Di item in Enum.GetValues(typeof(Di)))
                {
                    D_inputSignalVitualStatu.Add(item.ToString(), "01");
                }
                foreach (Do item in Enum.GetValues(typeof(Do)))
                {
                    D_outputSingalVitualStatu.Add(item.ToString(), Level .Low );
                }
                if (Project .Instance .configuration .vitualCard)
                {

                    return;
                }

                //初始化板卡
                int count = 0;
                try
                {
                    count = Dmc2410.d2410_board_init();
                }
                catch
                {
                    Frm_MessageBox messageBox = new Frm_MessageBox();
                    messageBox.MessageBoxShow("\r\n雷赛DMC2410运动控制卡初始化失败，可能原因：\r\n① 未安装对应运动控制卡驱动");
                    return;
                }
                if (count == 0)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n未识别到雷赛DMC2410运动控制卡");
                }
                else
                {
                    Dmc2410.d2410_write_SEVON_PIN(0, 1);
                    Dmc2410.d2410_write_SEVON_PIN(1, 1);
                    Dmc2410.d2410_write_SEVON_PIN(2, 1);
                    Dmc2410.d2410_write_SEVON_PIN(3, 1);

                    Thread.Sleep(100);
                    Dmc2410.d2410_write_SEVON_PIN(0, 0);
                    Dmc2410.d2410_write_SEVON_PIN(1, 0);
                    Dmc2410.d2410_write_SEVON_PIN(2, 0);
                    Dmc2410.d2410_write_SEVON_PIN(3, 0);
                    //设置原点、正负限位的逻辑电平
                    for (ushort i = 0; i < 4; i++)
                    {
                        Dmc2410.d2410_set_HOME_pin_logic(i, (ushort)(Axis_Config.Instance.原点逻辑电平[i] == LogicLevel.低电平有效 ? 0 : 1), 1);
                    }
                    initSucceed = true;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 是否在原点
        /// </summary>
        /// <returns></returns>
        internal static bool InHome(ushort axisIndex)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)
                    return true;

                ushort tt = Dmc2410.d2410_axis_io_status(axisIndex);
                bool result = GetBit16(tt, 14);
                if (Axis_Config.Instance.原点逻辑电平[axisIndex] == LogicLevel.低电平有效)
                {
                    if (result)
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (result)
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 减速停止
        /// </summary>
        /// <param name="axisIndex"></param>
        internal static void DecStop(ushort axisIndex)
        {
            try
            {
                Dmc2410.d2410_decel_stop(axisIndex, 0.5);
                WaitMoveDone(axisIndex);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 是否在正限位
        /// </summary>
        /// <returns></returns>
        internal static bool InPEL(ushort axisIndex)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)
                    return true;

                ushort tt = Dmc2410.d2410_axis_io_status(axisIndex);
                bool result = GetBit16(tt, 12);
                if (result)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return true;
            }
        }
        /// <summary>
        /// 是否在负限位
        /// </summary>
        /// <returns></returns>
        internal static bool InNEL(ushort axisIndex)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)
                    return true;

                ushort tt = Dmc2410.d2410_axis_io_status(axisIndex);
                bool result = GetBit16(tt, 3);
                if (result)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return true;
            }
        }
        /// <summary>
        /// 获取指定轴报警状态
        /// </summary>
        /// <param name="axisIndex">轴枚举</param>
        /// <returns>轴是否报警</returns>
        internal static bool GetAlarmStatu(ushort axisIndex)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)
                    return false;

                ushort tt = Dmc2410.d2410_axis_io_status(axisIndex);
                bool result = GetBit16(tt, 11);
                if (!result)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 获取指定轴上电状态
        /// </summary>
        /// <param name="axisIndex">轴枚举</param>
        /// <returns>轴是否已上电</returns>
        internal static bool GetMotorStatu(ushort axisIndex)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)
                {
                    return false;
                }

                int value = Dmc2410.d2410_read_SEVON_PIN(axisIndex);
                if (value == 1)
                    return false;
                else
                    return true;


            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 轴回零
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <param name="searchDistance">搜索长度</param>
        /// <param name="vel">回零速度</param>
        /// <param name="homeDir">回零方向</param>
        /// <param name="backDistance">回退距离</param>
        /// <param name="acc">加速度</param>
        internal static void Home(ushort axisIndex, double homeVel, HomeDir homeDir, Int32 backLength, short acc)
        {
            try
            {
                int value;
                if (homeDir == HomeDir.N_负方向)       //向负方向回零
                {
                    //判断各种回零情况       
                    if (!InHome(axisIndex))       //如果不在原点
                    {
                        if (InNEL(axisIndex))       //如果在负极限
                        {
                            Dmc2410.d2410_t_pmove(axisIndex, backLength, 0);
                            Thread.Sleep(100);
                            WaitMoveDone(axisIndex);
                            Dmc2410.d2410_home_move(axisIndex, 0, 0);
                        }
                        else         //如果不在负极限 ,在中间区域
                        {
                            Dmc2410.d2410_home_move(axisIndex, 0, 0);
                            WaitMoveDone(axisIndex);
                            //判断是否在负极限
                            if (InNEL(axisIndex))        //如果在负限位
                            {
                                Dmc2410.d2410_t_pmove(axisIndex, backLength, 0);
                                WaitMoveDone(axisIndex);
                                Dmc2410.d2410_home_move(axisIndex, 0, 0);
                            }
                            else
                            {
                                //如果当前处在原点，回零完成
                            }
                        }
                    }
                    else        //如果在原点
                    {
                        Dmc2410.d2410_t_pmove(axisIndex, 10000, 0);
                        WaitMoveDone(axisIndex);
                        Dmc2410.d2410_home_move(axisIndex, 0, 0);
                    }
                }
                else        //向正方向回零
                {
                    //判断各种回零情况
                    if (InHome(axisIndex))        //如果不在原点
                    {
                        if (InPEL(axisIndex))         //如果在正极限
                        {
                            Dmc2410.d2410_t_pmove(axisIndex, -backLength, 0);
                            WaitMoveDone(axisIndex);
                            Dmc2410.d2410_home_move(axisIndex, 1, 0);
                        }
                        else        //如果不在负极限
                        {
                            //正常回零
                            Dmc2410.d2410_home_move(axisIndex, 1, 0);
                            WaitMoveDone(axisIndex);
                            //判断是否在正极限
                            if (InPEL(axisIndex))        //如果在正极限
                            {
                                Dmc2410.d2410_t_pmove(axisIndex, -backLength, 0);
                                WaitMoveDone(axisIndex);
                                Dmc2410.d2410_home_move(axisIndex, 1, 0);
                            }
                            else
                            {
                                //如果当前处在原点，回零完成
                            }
                        }
                    }
                    else        //如果在原点
                    {
                        Dmc2410.d2410_t_pmove(axisIndex, -20000, 0);
                        WaitMoveDone(axisIndex);
                        Dmc2410.d2410_home_move(axisIndex, 1, 0);
                    }
                }
                if (!InHome(axisIndex))
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n回零失败，可能原因：\r\n回退距离不足");
                }
                Thread.Sleep(100);
                Dmc2410.d2410_set_position(axisIndex, 0);
                Dmc2410.d2410_set_encoder(axisIndex, 0);
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
                    value = Dmc2410.d2410_read_inbit(0,Convert .ToUInt16 (  diIndex));
                    if (value != 0)
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
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                Dmc2410.d2410_write_SEVON_PIN(axisIndex, 1);
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
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                Dmc2410.d2410_write_SEVON_PIN(axisIndex, 0);
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
        internal static bool MoveAbs(object axisName, Int32 targetPos, int vel, bool waitDone = true)
        {
            try
            {

                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                if (targetPos <= Axis_Config.Instance.负软极限[axisIndex] || targetPos >= Axis_Config.Instance.正软极限[axisIndex])
                {
                    Frm_Main.Instance.OutputMsg("超出软极限，运动失败", Color.Red);
                    return false;
                }
                Dmc2410.d2410_set_profile(axisIndex, vel / 2, vel, 0.2, 0.2);
                Dmc2410.d2410_t_pmove(axisIndex, targetPos, 1);
                if (waitDone)
                    WaitMoveDone(axisIndex);
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 连续运动
        /// </summary>
        /// <param name="axisName">轴名称</param>
        internal static bool KeepMove(object axisName, ushort dir, short vel, bool waitDone)
        {
            try
            {

                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                //if (targetPos <= Axis_Config.Instance.负软极限[axisIndex] || targetPos >= Axis_Config.Instance.正软极限[axisIndex])
                //{
                //    Frm_Main.Instance.OutputMsg("超出软极限，运动失败", Color.Red);
                //    return false;
                //}
                Dmc2410.d2410_set_profile(axisIndex, vel / 2, vel, 0.2, 0.2);
                Dmc2410.d2410_t_vmove(axisIndex, dir);
                if (waitDone)
                    WaitMoveDone(axisIndex);
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 相对位置移动
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <param name="pos">脉冲数</param>
        internal static bool MoveRel(object axisName, Int32 distance, int vel, bool waitDone = true)
        {
            try
            {

                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                double targetPos = GetCurPosition(axisIndex) + distance;
                if (targetPos <= Axis_Config.Instance.负软极限[axisIndex] || targetPos >= Axis_Config.Instance.正软极限[axisIndex])
                {
                    Frm_Main.Instance.OutputMsg("超出软极限，运动失败", Color.Red);
                    return false;
                }
                Dmc2410.d2410_set_profile(axisIndex, vel / 2, vel, 0.2, 0.2);
                Dmc2410.d2410_t_pmove(axisIndex, distance, 0);
                if (waitDone)
                    WaitMoveDone(axisIndex);
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
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
        /// <returns>当前编码器位置</returns>
        internal static double GetCurEncoder(object axisName)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)
                    return 0;

                double curPos;
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo ;
                curPos = Dmc2410.d2410_get_encoder(axisIndex);
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
        /// <returns>当前命令位置</returns>
        internal static double GetCurPosition(object axisName)
        {
            try
            {
                if (Project .Instance .configuration .vitualCard)
                    return 0;

                double curPos;
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                curPos = Dmc2410.d2410_get_position(axisIndex);
                return curPos;
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
                    Dmc2410.d2410_write_outbit(0, Convert .ToUInt16 ( doIndex), 1);
                else
                    Dmc2410.d2410_write_outbit(0,Convert .ToUInt16 (  doIndex), 0);
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

                value = Dmc2410.d2410_read_inbit(0, Convert .ToUInt16 ( doIndex));
                if (value == 0)
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
        internal static void WaitMoveDone(ushort axisIndex)
        {
            try
            {
                Thread.Sleep(100);
                int statu;
                do
                {
                    statu = Dmc2410.d2410_check_done((ushort)axisIndex);
                    Thread.Sleep(10);
                } while (statu == 1);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
