using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csDmc2210;
using System.Threading;
using System.Drawing;
using APS168_W64;
using APS_Define_W32;

namespace VMPro
{
    /// <summary>
    /// 雷赛DMC2210系列运动控制卡控制类
    /// </summary>
    internal class Card_ADLink : CardBase
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
                Int32 ret = 0;
                Int32 boardID_InBits = 0;
                Int32 mode = 0;
                Int32 card_name = 0;
                Int32 StartAxisID = 0;
                Int32 TotalAxisNum = 0;
                try
                {
                    ret = APS168.APS_initial(ref boardID_InBits, mode);
                }
                catch
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n凌华AMP204C运动控制卡初始化失败，可能原因：\r\n① 未安装对应运动控制卡驱动", TipType.Error );
                    initSucceed = false;
                    return;
                }


                if (ret == 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        Int32 temp = (boardID_InBits >> i) & 1;

                        if (temp == 1)
                        {
                            ret = APS168.APS_get_card_name(i, ref card_name);

                            if (card_name == (Int32)APS_Define.DEVICE_NAME_PCI_825458
                                || card_name == (Int32)APS_Define.DEVICE_NAME_AMP_20408C)
                            {
                                ret = APS168.APS_get_first_axisId(i, ref  StartAxisID, ref  TotalAxisNum);

                              
                            }
                        }
                    }

                }
                else
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n凌华AMP204C运动控制卡初始化失败，可能原因：\r\n① 未安装对应运动控制卡驱动",TipType.Error);
                    initSucceed = false;
                    return;
                }
                initSucceed = true;
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
                //lock (obj_card)
                //{
                //    if (Project.Instance.configuration.vitualCard)
                //        return false;

                //    ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                //    bool result = GetBit16(temp, 14);
                //    if (Axis_Config.Instance.原点逻辑电平[axisIndex] == LogicLevel.低电平有效)
                //    {
                //        if (result)
                //            return true;
                //        else
                //            return false;
                //    }
                //    else
                //    {
                //        if (result)
                //            return true;
                //        else
                //            return false;
                //    }
                //}
                return true;
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
                //lock (obj_card)
                //{
                //    if (Project.Instance.configuration.vitualCard)
                //        return true;

                //    ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                //    bool result = GetBit16(temp, 12);
                //    if (result)
                //        return true;
                //    else
                //        return false;
                //}
                return false;
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
                //lock (obj_card)
                //{
                //    if (Project.Instance.configuration.vitualCard)
                //        return true;

                //    ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                //    bool result = GetBit16(temp, 13);
                //    if (result)
                //        return true;
                //    else
                //        return false;
                //}
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
        /// <param name="axisIndex">轴索引号</param>
        /// <returns>轴是否报警</returns>
        internal static bool GetAlarmStatu(ushort axisIndex)
        {
            try
            {
                //lock (obj_card)
                //{
                //    if (Project.Instance.configuration.vitualCard)
                //        return false;

                //    ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                //    bool result = GetBit16(temp, 11);
                //    if (result)
                //        return true;
                //    else
                //        return false;
                //}
                return false;
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
                //if (Project.Instance.configuration.vitualCard)
                //{
                //    return false;
                //}

                //int result = Dmc2210.d2210_read_SEVON_PIN(axisIndex);
                //if (result == 1)
                //    return false;
                //else
                //    return true;
                return false;
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
                const Int32 ON = 1;
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                APS168.APS_set_servo_on(axisIndex, ON);
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
                const Int32 OFF = 0;
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                APS168.APS_set_servo_on(axisIndex, OFF);
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

                    Int32 digital_input_value = 0;
                    Int32 __MAX_DI_CH = (24);
                    APS168.APS_read_d_input(0, 0, ref digital_input_value);
                    Int32[] di_ch = new Int32[__MAX_DI_CH];
                    for (int i = 0; i < __MAX_DI_CH; i++)
                        di_ch[i] = ((digital_input_value >> i) & 1);
                    if (di_ch[  Convert .ToUInt16 ( diIndex)] == 0)
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
                //if (Project.Instance.configuration.vitualCard)        //如果板卡虚拟，则一律返回低电平
                //    return D_outputSingalVitualStatu[doName.ToString()];

                //ushort doIndex = (ushort)GetDoIndexByName(doName.ToString());
                //int value = Dmc2210.d2210_read_inbit(0, doIndex);
                //if (value == 0)
                //    return Level.Low;
                //else
                //    return Level.High;
                return Level.Low;
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

                Int32 __MAX_DO_CH = (24);
                Int32[] do_ch = new Int32[__MAX_DO_CH];
                Int32 digital_output_value = 0;

                do_ch[0] = 1;  // set 0 or 1
                do_ch[2] = 1;  // set 0 or 1
                do_ch[4] = 1;  // set 0 or 1

                digital_output_value = 0;
                for (int i = 0; i < __MAX_DO_CH; i++)
                    digital_output_value |= (do_ch[i] << i);

                APS168.APS_write_d_output(0, 0, digital_output_value);

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


                   //This example shows how home move operation
                   Int32 axis_id = axisIndex;
                   Int32 return_code = 0;

                   // 1. Select home mode and config home parameters 
                   APS168.APS_set_axis_param(axis_id, (Int32)APS_Define.PRA_HOME_MODE, 0); // Set home mode
                   APS168.APS_set_axis_param(axis_id, (Int32)APS_Define.PRA_HOME_DIR, 1); // Set home direction
                   APS168.APS_set_axis_param_f(axis_id, (Int32)APS_Define.PRA_HOME_CURVE, 0); // Set acceleration paten (T-curve)
                   APS168.APS_set_axis_param_f(axis_id, (Int32)APS_Define.PRA_HOME_ACC, 1000000); // Set homing acceleration rate
                   APS168.APS_set_axis_param_f(axis_id, (Int32)APS_Define.PRA_HOME_VM, 100000); // Set homing maximum velocity.
                   APS168.APS_set_axis_param_f(axis_id, (Int32)APS_Define.PRA_HOME_VO, 50000); // Set homing VO speed
                   APS168.APS_set_axis_param(axis_id, (Int32)APS_Define.PRA_HOME_EZA, 0); // Set EZ signal alignment (yes or no)
                   APS168.APS_set_axis_param_f(axis_id, (Int32)APS_Define.PRA_HOME_SHIFT, 0); // Set home position shfit distance. 
                   APS168.APS_set_axis_param_f(axis_id, (Int32)APS_Define.PRA_HOME_POS, 0); // Set final home position.


                   //servo on
                   APS168.APS_set_servo_on(axis_id, 1);
                   Thread.Sleep(500); // Wait stable.


                   // 2. Start home move
                   return_code = APS168.APS_home_move(axis_id); //Start homing 
                   if (return_code != (Int32)APS_Define.ERR_NoError)
                   { /* Error handling */ }

                   WaitMoveDone(axisIndex);
                   if (!InHome(axisIndex))
                   {
                       Frm_MessageBox.Instance.MessageBoxShow("\r\n回零失败，可能原因：\r\n①回退距离不足\r\n①hui lin shi zan ting");
                   }
                   else
                   {
                       Thread.Sleep(1000);
                       //Dmc2210.d2210_set_position(axisIndex, 0);
                       //Dmc2210.d2210_set_encoder(axisIndex, 0);

                       APS168.APS_set_command_f(axis_id, 0);
                       APS168.APS_set_position_f(axis_id, 0);
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
                //if (targetPos < Axis_Config.Instance.负软极限[axisIndex] || targetPos > Axis_Config.Instance.正软极限[axisIndex])
                //{
                //    Frm_Main.Instance.OutputMsg("目标位置超出软极限，运动失败", Color.Red);
                //    return;
                //}
                Int32 ret = 0;
                ASYNCALL p = new ASYNCALL();

                // Config speed profile parameters.
                ret = APS168.APS_set_axis_param_f(axisIndex, (Int32)APS_Define.PRA_SF, 0.5);
                ret = APS168.APS_set_axis_param_f(axisIndex, (Int32)APS_Define.PRA_ACC, vel/3);
                ret = APS168.APS_set_axis_param_f(axisIndex, (Int32)APS_Define.PRA_DEC, vel/3);
                ret = APS168.APS_set_axis_param_f(axisIndex, (Int32)APS_Define.PRA_VM, vel);

                //servo on
                APS168.APS_set_servo_on(axisIndex, 1);
                Thread.Sleep(500); // Wait stable.

                // Start a relative p to p move
                ret = APS168.APS_ptp(axisIndex, (Int32)APS_Define.OPT_ABSOLUTE , targetPos, ref p);

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
        internal static bool MoveRel(object    axisName, double distance, int vel, bool waitDone = true)
        {
            try
            {
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;

                double targetPos = GetCurEncoder(axisName) * Axis_Config.Instance.MMPixelRoute[axisIndex] + distance;
                if (targetPos < Axis_Config.Instance.负软极限[axisIndex] || targetPos > Axis_Config.Instance.正软极限[axisIndex])
                {
                  Frm_MotionControl.Instance .  OutputMsg("目标位置超出软极限，运动失败", Color.Red);
                    return false;
                }
                //Dmc2210.d2210_set_profile(axisIndex, vel / 2, vel, 0.1, 0.1);

                int distance1 = (int)(targetPos / Axis_Config.Instance.MMPixelRoute[axisIndex]);
                //Dmc2210.d2210_t_pmove(axisIndex, distance1, 0);
                MoveAbs(axisName, distance1, vel, true);
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

                ////Dmc2210.d2210_set_profile(axisIndex, vel / 2, vel, 0.2, 0.2);
                ////Dmc2210.d2210_t_vmove(axisIndex, dir);

                APS168.APS_set_axis_param(axisIndex, (Int32)APS_Define.PRA_JG_MODE, 0);  // Set jog mode
                APS168.APS_set_axis_param(axisIndex, (Int32)APS_Define.PRA_JG_DIR, 0);  // Set jog direction

                APS168.APS_set_axis_param_f(axisIndex, (Int32)APS_Define.PRA_JG_CURVE, 0.0);
                APS168.APS_set_axis_param_f(axisIndex, (Int32)APS_Define.PRA_JG_ACC, 1000.0);
                APS168.APS_set_axis_param_f(axisIndex, (Int32)APS_Define.PRA_JG_DEC, 1000.0);
                APS168.APS_set_axis_param_f(axisIndex, (Int32)APS_Define.PRA_JG_VM, 10000.0);

                //servo on
                APS168.APS_set_servo_on(axisIndex, 1);
                Thread.Sleep(500); // Wait stable.

                // Create a rising edge.
                APS168.APS_jog_start(axisIndex, 0);  //Jog Off
                APS168.APS_jog_start(axisIndex, 1);  //Jog ON

                //////判断是否超出软极限
                ////Thread th = new Thread(() =>
                ////{
                ////    while (true)
                ////    {
                ////        Thread.Sleep(100);
                ////        double curPosition = GetCurEncoder(axisIndex);
                ////        curPosition = curPosition * Axis_Config.Instance.MMPixelRoute[axisIndex];
                ////        if (curPosition < Axis_Config.Instance.负软极限[axisIndex] || curPosition > Axis_Config.Instance.正软极限[axisIndex])
                ////        {
                ////            Frm_Main.Instance.OutputMsg("超出软极限，运动失败", Color.Red);
                ////            DecStop(axisIndex);
                ////            break;
                ////        }

                ////        //判断是否已停止
                ////        int statu = Dmc2210.d2210_check_done((ushort)axisIndex);
                ////        if (statu != 0)
                ////            break;
                ////    }
                ////});


                ////if (waitDone)
                ////    WaitMoveDone(axisIndex);
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
                        for (int j = 3; j < Frm_MotionControl.Instance.dgv_pointList.Columns.Count; j++)
                        {
                            string axisName = Frm_MotionControl.Instance.dgv_pointList.Columns[j].HeaderText.ToString();
                            Int32 targetPos = Convert.ToInt32(Frm_MotionControl.Instance.dgv_pointList.Rows[i].Cells[j].Value);
                            Int32 vel = Convert.ToInt32(Frm_MotionControl.Instance.dgv_pointList.Rows[i].Cells[2].Value);
                            if (Frm_MotionControl.Instance.dgv_pointList.Rows[i].Cells[j].Value.ToString() != "NA")
                            {
                                int axisIndex = Card_ADLink.FindAxisByName(axisName).actNo;

                                //Card_ADLink.MoveAbs(dgv_pointList.Columns[i].HeaderText, Convert.ToInt32(dgv_pointList.Rows[selectRow].Cells[i].Value), (int)(Convert.ToDouble(cbo_moveVel.Text.Trim()) / Axis_Config.Instance.MMPixelRoute[axisIndex]), true);

                                Card_ADLink.MoveAbs(axisName, (int)(targetPos ), vel , false );
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

        public static Int32 check_motion_done_example(Int32 Axis_ID, ref Int32 Stop_Code)
        {
            Int32 axis_id = Axis_ID;
            Int32 msts = 0;
            Int32 m_stop_code = 0;

            msts = APS168.APS_motion_status(axis_id); // Get motion status
            msts = (msts >> 5) & 1;                   // Get motion done bit

            // Get stop code.
            APS168.APS_get_stop_code(Axis_ID, ref Stop_Code);

            if (msts == 1)
            {
                // Check move success or not
                msts = APS168.APS_motion_status(axis_id); // Get motion status
                msts = (msts >> 16) & 1;                  // Get abnormal stop bit

                if (msts == 1)
                { // Error handling ...

                    APS168.APS_get_stop_code(axis_id, ref m_stop_code);
                    return -1; //error
                }
                else
                {   // Motion success.
                    return 1;
                }
            }

            return 0; //Now are in motion
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
                Int32 ret = 0;
                Int32 Stop_Code = 0;
                do
                {
                    ret = check_motion_done_example(axisIndex, ref Stop_Code);
                    Thread.Sleep(10);
                } while (Stop_Code != 0);
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
                //////Dmc2210.d2210_decel_stop(axisIndex, 0.5);
                //////WaitMoveDone(axisIndex);

                APS168.APS_set_axis_param_f(axisIndex, (Int32)APS_Define.PRA_STP_DEC, 10000.0);
                APS168.APS_stop_move(axisIndex);
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

                double curPos = 0;
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                APS168.APS_get_command_f(axisIndex, ref curPos);

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
        internal static double GetCurEncoder(object axisName)
        {
            try
            {
                if (Project.Instance.configuration.vitualCard)
                    return 0;

                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                Double tmp = 0;
                APS168.APS_get_position_f(axisIndex, ref tmp);

                return tmp;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return 0;
            }
        }

        internal static void CloseBoard()
        {
            try
            {
                APS168.APS_close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
