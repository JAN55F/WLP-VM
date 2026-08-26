using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csDmc2210;
using System.Threading;
using System.Drawing;

namespace VMPro
{
    /// <summary>
    /// 雷赛DMC2210系列运动控制卡控制类
    /// </summary>
    internal class Card_LeadShineDMC2210 : CardBase
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
                if (Project.Instance.configuration.vitualCard)
                {
                    return;
                }

                //初始化板卡
                int count = 0;
                try
                {
                    count = Dmc2210.d2210_board_init();
                }
                catch
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n雷赛DMC2210运动控制卡初始化失败，可能原因：\r\n① 未安装对应运动控制卡驱动");
                    return;
                }

                if (count == 0)
                {
                    Frm_MessageBox messageBox = new Frm_MessageBox();
                    messageBox.MessageBoxShow("\r\n未识别到雷赛DMC2210运动控制卡");
                }
                else
                {
                    Dmc2210.d2210_write_SEVON_PIN(0, 1);
                    Dmc2210.d2210_write_SEVON_PIN(1, 1);

                    //各轴使能
                    Thread.Sleep(100);
                    Dmc2210.d2210_write_SEVON_PIN(0, 0);
                    Dmc2210.d2210_write_SEVON_PIN(1, 0);

                    //设置限位感应器的有效电平和制动方式
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2210.d2210_config_EL_MODE(i, (ushort)(Axis_Config.Instance.限位逻辑电平[i]));
                    }

                    //设置脉冲输出模式
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2210.d2210_set_pulse_outmode(i, (ushort)(Axis_Config.Instance.脉冲输出模式[i]));
                    }

                    //设置原点感应器的逻辑电平
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2210.d2210_set_HOME_pin_logic(i, (ushort)(Axis_Config.Instance.原点逻辑电平[i] == LogicLevel.低电平有效 ? 0 : 1), 1);
                    }

                    //设置编码器计数方式
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2210.d2210_counter_config(i, (ushort)(Axis_Config.Instance.编码器计数方式[i]));
                    }
                    initSucceed = true;
                }
            }
            catch (Exception ex)
            {
                initSucceed = false;
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 当前是否在原点
        /// </summary>
        /// <param name="axisIndex">轴索引号</param>
        /// <returns>是否在原点</returns>
        internal static bool InHome(ushort axisIndex)
        {
            try
            {
                lock (obj_card)
                {
                    if (Project.Instance.configuration.vitualCard)
                        return false;

                    ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                    bool result = GetBit16(temp, 14);
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
                            return true;
                        else
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 当前是否在正限位
        /// </summary>
        /// <param name="axisIndex">轴索引号</param>
        /// <returns>是否在正限位</returns>
        internal static bool InPEL(ushort axisIndex)
        {
            try
            {
                lock (obj_card)
                {
                    if (Project.Instance.configuration.vitualCard)
                        return true;

                    ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                    bool result = GetBit16(temp, 12);
                    if (result)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return true;
            }
        }
        /// <summary>
        /// 当前是否在负限位
        /// </summary>
        /// <param name="axisIndex">轴索引号</param>
        /// <returns>是否在限位</returns>
        internal static bool InNEL(ushort axisIndex)
        {
            try
            {
                lock (obj_card)
                {
                    if (Project.Instance.configuration.vitualCard)
                        return true;

                    ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                    bool result = GetBit16(temp, 13);
                    if (result)
                        return true;
                    else
                        return false;
                }
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
        /// <param name="axisIndex">轴索引号</param>
        /// <returns>轴是否报警</returns>
        internal static bool GetAlarmStatu(ushort axisIndex)
        {
            try
            {
                lock (obj_card)
                {
                    if (Project.Instance.configuration.vitualCard)
                        return false;

                    ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                    bool result = GetBit16(temp, 11);
                    if (result)
                        return true;
                    else
                        return false;
                }
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
                if (Project.Instance.configuration.vitualCard)
                {
                    return false;
                }

                int result = Dmc2210.d2210_read_SEVON_PIN(axisIndex);
                if (result == 1)
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
        /// 轴上电
        /// </summary>
        /// <param name="axisName">轴名称</param>
        internal static void MotorOn(object axisName)
        {
            try
            {
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                Dmc2210.d2210_write_SEVON_PIN(axisIndex, 0);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 轴下电
        /// </summary>
        /// <param name="axisName">轴名称</param>
        internal static void MotorOff(object axisName)
        {
            try
            {
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                Dmc2210.d2210_write_SEVON_PIN(axisIndex, 1);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 获取通用输入状态
        /// </summary>
        /// <param name="diName">输入点名称</param>
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
                else if (Project.Instance.configuration.vitualCard)        //如果板卡虚拟，则一律返回低电平
                {
                    return Level.Low;
                }
                else
                {

                    string  diIndex = GetDiIndexByName(diName.ToString());
                    int value = Dmc2210.d2210_read_inbit(0,Convert .ToUInt16 ( diIndex));
                    if (value == 0)
                        return Level.High;
                    else
                        return Level.Low;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return Level.Low;
            }
        }
        /// <summary>
        /// 获取通用输出状态
        /// </summary>
        /// <param name="diName">输入点名称</param>
        /// <returns>输入状态</returns>
        internal static Level GetDoSts(object doName)
        {
            try
            {
                if (Project.Instance.configuration.vitualCard)        //如果板卡虚拟，则一律返回低电平
                    return D_outputSingalVitualStatu[doName.ToString()];

                string  doIndex = GetDoIndexByName(doName.ToString());
                int value = Dmc2210.d2210_read_inbit(0, Convert .ToUInt16 ( doIndex));
                if (value == 0)
                    return Level.Low;
                else
                    return Level.High;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return Level.Low;
            }
        }
        /// <summary>
        /// 通用输出操作
        /// </summary>
        /// <param name="doName">输出点名称</param>
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

                string  doIndex = Card_Googol.GetDoIndexByName(doName.ToString());
                if (level == Level.High)
                    Dmc2210.d2210_write_outbit(0,Convert .ToUInt16 (  doIndex), 1);
                else
                    Dmc2210.d2210_write_outbit(0,Convert .ToUInt16 (  doIndex), 0);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 等待信号到达
        /// </summary>
        /// <param name="diName">输入点名称</param>
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
        /// 轴回零
        /// </summary>
        /// <param name="axisIndex">轴索引号</param>
        /// <param name="homeVel">回零速度</param>
        /// <param name="homeDir">回零方向</param>
        /// <param name="backLength">回退距离</param>
        internal static void Home(ushort   axisIndex, double homeVel, HomeDir homeDir, Int32 backLength)
        {
            try
            {
                Thread th = new Thread(() =>
               {
                   homing = true;
                   Dmc2210.d2210_set_profile(axisIndex, homeVel / 3, homeVel, 0.2, 0.2);
                   Dmc2210.d2210_config_home_mode(axisIndex, 0, 0);
                   if (homeDir == HomeDir.N_负方向)       //向负方向回零
                   {
                       if (!InHome(axisIndex))       //如果不在原点
                       {
                           if (InNEL(axisIndex))       //如果在负极限
                           {
                               Dmc2210.d2210_t_pmove(axisIndex, backLength, 0);         //回退一定距离，使其推到原点以外
                               WaitMoveDone(axisIndex);
                               Dmc2210.d2210_home_move(axisIndex, 2, 0);
                               WaitMoveDone(axisIndex);
                           }
                           else         //如果不在负极限
                           {
                               Dmc2210.d2210_home_move(axisIndex, 2, 0);
                               WaitMoveDone(axisIndex);
                               if (InNEL(axisIndex))        //如果在负限位
                               {
                                   Dmc2210.d2210_t_pmove(axisIndex, backLength, 0);
                                   WaitMoveDone(axisIndex);
                                   Dmc2210.d2210_home_move(axisIndex, 2, 0);
                                   WaitMoveDone(axisIndex);
                               }
                               else
                               {
                                   //如果当前处在原点，那么回零完成
                               }
                           }
                       }
                       else        //如果在原点
                       {
                           Dmc2210.d2210_t_pmove(axisIndex, backLength, 0);
                           WaitMoveDone(axisIndex);
                           Dmc2210.d2210_home_move(axisIndex, 2, 0);
                           WaitMoveDone(axisIndex);
                       }
                   }
                   else        //向正方向回零
                   {
                       if (!InHome(axisIndex))        //如果不在原点
                       {
                           if (InPEL(axisIndex))         //如果在正极限
                           {
                               Dmc2210.d2210_t_pmove(axisIndex, -backLength, 0);
                               WaitMoveDone(axisIndex);
                               Dmc2210.d2210_home_move(axisIndex, 1, 0);
                               WaitMoveDone(axisIndex);
                           }
                           else        //如果不在正极限
                           {
                               Dmc2210.d2210_home_move(axisIndex, 1, 0);
                               WaitMoveDone(axisIndex);
                               if (InPEL(axisIndex))        //如果在正极限
                               {
                                   Dmc2210.d2210_t_pmove(axisIndex, -backLength, 0);
                                   WaitMoveDone(axisIndex);
                                   Dmc2210.d2210_home_move(axisIndex, 1, 0);
                                   WaitMoveDone(axisIndex);
                               }
                               else
                               {
                                   //如果当前处在原点，那么回零完成
                               }
                           }
                       }
                       else        //如果在原点
                       {
                           Dmc2210.d2210_t_pmove(axisIndex, -backLength, 0);
                           WaitMoveDone(axisIndex);
                           Dmc2210.d2210_home_move(axisIndex, 1, 0);
                           WaitMoveDone(axisIndex);
                       }
                   }
                   if (!InHome(axisIndex))
                   {
                       Frm_MessageBox.Instance.MessageBoxShow("\r\n回零失败，可能原因：\r\n①回退距离不足\r\n①hui lin shi zan ting");
                   }
                   else
                   {
                       Thread.Sleep(100);
                       Dmc2210.d2210_set_position(axisIndex, 0);
                       Dmc2210.d2210_set_encoder(axisIndex, 0);
                   }
                   homing = false;
               });
                th.IsBackground = true;
                th.Start();
            }
            catch (Exception ex)
            {
                homing = false;
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绝对位置移动
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <param name="targetPos">目标位置</param>
        /// <param name="vel">移动速度</param>
        /// <param name="waitDone">是否等待</param>
        /// <returns></returns>
        internal static void MoveAbs(object axisName, Int32 targetPos, int vel, bool waitDone)
        {
            try
            {
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                if (targetPos < Axis_Config.Instance.负软极限[axisIndex] || targetPos > Axis_Config.Instance.正软极限[axisIndex])
                {
                    Frm_Main.Instance.OutputMsg("目标位置超出软极限，运动失败", Color.Red);
                    return;
                }
                Dmc2210.d2210_set_profile(axisIndex, vel / 3, vel, 0.2, 0.2);

                Dmc2210.d2210_t_pmove(axisIndex, (int)(targetPos / Axis_Config.Instance.MMPixelRoute[axisIndex]), 1);
                if (waitDone)
                    WaitMoveDone(axisIndex);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 相对位置移动
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <param name="distance">移动距离</param>
        /// <param name="vel">移动速度</param>
        /// <param name="waitDone">是否等待</param>
        /// <returns></returns>
        internal static bool MoveRel(object axisName, double distance, int vel, bool waitDone = true)
        {
            try
            {
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                double targetPos = GetCurPosition(axisIndex) * Axis_Config.Instance.MMPixelRoute[axisIndex] + distance;
                if (targetPos < Axis_Config.Instance.负软极限[axisIndex] || targetPos > Axis_Config.Instance.正软极限[axisIndex])
                {
                    Frm_Main.Instance.OutputMsg("目标位置超出软极限，运动失败", Color.Red);
                    return false;
                }
                Dmc2210.d2210_set_profile(axisIndex, vel / 2, vel, 0.1, 0.1);

                int distance1 = (int)(targetPos / Axis_Config.Instance.MMPixelRoute[axisIndex]);
                Dmc2210.d2210_t_pmove(axisIndex, distance1, 0);
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
        internal static void KeepMove(object axisName, ushort dir, int vel, bool waitDone)
        {
            try
            {

                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;

                Dmc2210.d2210_set_profile(axisIndex, vel / 2, vel, 0.2, 0.2);
                Dmc2210.d2210_t_vmove(axisIndex, dir);

                //判断是否超出软极限
                Thread th = new Thread(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(100);
                        double curPosition = GetCurEncoder(axisIndex);
                        curPosition = curPosition * Axis_Config.Instance.MMPixelRoute[axisIndex];
                        if (curPosition < Axis_Config.Instance.负软极限[axisIndex] || curPosition > Axis_Config.Instance.正软极限[axisIndex])
                        {
                            Frm_Main.Instance.OutputMsg("超出软极限，运动失败", Color.Red);
                            DecStop(axisIndex);
                            break;
                        }

                        //判断是否已停止
                        int statu = Dmc2210.d2210_check_done((ushort)axisIndex);
                        if (statu != 0)
                            break;
                    }
                });

                if (waitDone)
                    WaitMoveDone(axisIndex);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 移动到电表中的某个点
        /// </summary>
        /// <param name="pointName">点名称</param>
        internal static void MoveToPoint(string pointName)
        {
            try
            {
                for (int i = 0; i < Frm_MotionControl.Instance.dgv_pointList.Rows.Count - 1; i++)
                {
                    if (Frm_MotionControl.Instance.dgv_pointList.Rows[i].Cells[1].Value.ToString() == pointName)
                    {
                        for (int j = 2; j < Frm_MotionControl.Instance.dgv_pointList.Columns.Count; j++)
                        {
                            string axisName = Frm_MotionControl.Instance.dgv_pointList.Columns[j].HeaderText.ToString();
                            Int32 targetPos = Convert.ToInt32(Frm_MotionControl.Instance.dgv_pointList.Rows[i].Cells[j].Value);
                            if (Frm_MotionControl.Instance.dgv_pointList.Rows[i].Cells[j].Value.ToString() != "NA")
                            {
                                int axisIndex = Card_LeadShineDMC2210.FindAxisByName(axisName).actNo;
                                Card_LeadShineDMC2210.MoveAbs(axisName, (int)(targetPos * Axis_Config.Instance.MMPixelRoute[axisIndex]), (int)(Project.Instance.configuration.autoRunVel * (Project.Instance.configuration.autoRunVelRoute / 100) / Axis_Config.Instance.MMPixelRoute[axisIndex]), true);
                            }
                        }
                        return;
                    }
                }
                Frm_MessageBox.Instance.MessageBoxShow("要移动的点位不存在，请检查程序");
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 等待运动完成
        /// </summary>
        /// <param name="axisIndex">轴索引号</param>
        internal static void WaitMoveDone(ushort axisIndex)
        {
            try
            {
                Thread.Sleep(100);
                int statu;
                do
                {
                    statu = Dmc2210.d2210_check_done((ushort)axisIndex);
                    Thread.Sleep(10);
                } while (statu == 0);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 减速停止
        /// </summary>
        /// <param name="axisIndex">轴索引号</param>
        internal static void DecStop(ushort axisIndex)
        {
            try
            {
                Dmc2210.d2210_decel_stop(axisIndex, 0.5);
                WaitMoveDone(axisIndex);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
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
                if (Project.Instance.configuration.vitualCard)
                    return 0;

                double curPos;
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                curPos = Dmc2210.d2210_get_position(axisIndex);
                return curPos;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return 0;
            }
        }
        /// <summary>
        /// 获取当前编码器位置
        /// </summary>
        /// <param name="axisName">轴名称 </param>
        /// <returns>当前编码器位置</returns>
        internal static uint GetCurEncoder(object axisName)
        {
            try
            {
                if (Project.Instance.configuration.vitualCard)
                    return 0;

                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                uint temp = Dmc2210.d2210_get_encoder(axisIndex);
                return temp;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return 0;
            }
        }

    }
}
