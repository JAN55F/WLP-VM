using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WMX3ApiCLR;

namespace VMPro
{
    internal class Card_WMX : CardBase 
    {

        internal static bool initSucceed = false;
        internal static object obj_card = new object();
        static WMX3Api Device = new WMX3Api();
     static  CoreMotion Wmx3Lib_cm;
    static  CoreMotionStatus CmStatus = new CoreMotionStatus();
                 
        /// <summary>
        /// 初始化板卡
        /// </summary>
        internal static  void Init()
        {
            try
            {
                //初始化板卡
            
              
                // When all the devices are done, the WMX3 engine will also terminate.	
                 Device = new WMX3Api();


                // Get DevicesInfo to determine the type of device currently created
                DevicesInfo devInfo = new DevicesInfo();

                // Create device.
                Device.CreateDevice("C:\\Program Files\\SoftServo\\WMX3\\",
                    DeviceType.DeviceTypeNormal,
                    0xFFFFFFFF);

                // Set Device Name.
                Device.SetDeviceName("device");

                // Get created device state.
                Device.GetAllDevices(ref devInfo);

                Device.StartCommunication(0xFFFFFFFF);

                Wmx3Lib_cm = new CoreMotion(Device); 

                initSucceed = true;
            }
            catch (Exception ex)
            {
                initSucceed = false;
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
                Wmx3Lib_cm.GetStatus(ref CmStatus);
                 CoreMotionAxisStatus cmAxis = CmStatus.AxesStatus[0];
                 if (cmAxis.ServoOn)
                 {
                     return true;
                 }
                 else 
                     return false; 
            }
            catch (Exception ex)
            {
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
                Wmx3Lib_cm.GetStatus(ref CmStatus);
                CoreMotionAxisStatus cmAxis = CmStatus.AxesStatus[0];
                if (cmAxis.PositiveLS )
                {
                    return true;
                }
                else
                    return false; 
            }
            catch (Exception ex)
            {
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
                Wmx3Lib_cm.GetStatus(ref CmStatus);
                CoreMotionAxisStatus cmAxis = CmStatus.AxesStatus[0];
                if (cmAxis.NegativeLS )
                {
                    return true;
                }
                else
                    return false; 
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                Wmx3Lib_cm.GetStatus(ref CmStatus);
                CoreMotionAxisStatus cmAxis = CmStatus.AxesStatus[0];
                if (cmAxis.AmpAlarm )
                {
                    return true;
                }
                else
                    return false; 
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                Wmx3Lib_cm.GetStatus(ref CmStatus);
                if (CmStatus.AxesStatus[0].ServoOn)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                Wmx3Lib_cm.AxisControl.SetServoOn(axisIndex, 1);
                Stopwatch wait = Stopwatch.StartNew();
                while (wait.ElapsedMilliseconds < 5000)
                {
                    Wmx3Lib_cm.GetStatus(ref CmStatus);
                    if (CmStatus.AxesStatus[axisIndex].ServoOn)
                    {
                        return;
                    }

                    System.Threading.Thread.Sleep(100);
                }
                Log.SaveError(new TimeoutException("WMX 轴上电超时：" + axisName));
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                Wmx3Lib_cm.AxisControl.SetServoOn(axisIndex, 0);
                Stopwatch wait = Stopwatch.StartNew();
                while (wait.ElapsedMilliseconds < 5000)
                {
                    Wmx3Lib_cm.GetStatus(ref CmStatus);
                    if (!CmStatus.AxesStatus[axisIndex].ServoOn)
                    {
                        return;
                    }

                    System.Threading.Thread.Sleep(100);
                }
                Log.SaveError(new TimeoutException("WMX 轴下电超时：" + axisName));
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                //////if (D_inputSignalVitualStatu[diName.ToString()].Substring(0, 1) == "1")       //表示该输入信号当前处于虚拟状态
                //////{
                //////    if (D_inputSignalVitualStatu[diName.ToString()].Substring(1, 1) == "1")        //表示虚拟为高电平
                //////        return Level.High;
                //////    else
                //////        return Level.Low;
                //////}
                //////else if (Project.Instance.configuration.vitualCard)        //如果板卡虚拟，则一律返回低电平
                //////{
                //////    return Level.Low;
                //////}
                //////else
                //////{

                //////    ushort diIndex = (ushort)GetDiIndexByName(diName.ToString());
                //////    int value = Dmc2210.d2210_read_inbit(0, diIndex);
                //////    if (value == 0)
                //////        return Level.High;
                //////    else
                //////        return Level.Low;
                //////}
                return Level.Low;
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                //////if (Project.Instance.configuration.vitualCard)        //如果板卡虚拟，则一律返回低电平
                //////    return D_outputSingalVitualStatu[doName.ToString()];

                //////ushort doIndex = (ushort)GetDoIndexByName(doName.ToString());
                //////int value = Dmc2210.d2210_read_inbit(0, doIndex);
                //////if (value == 0)
                //////    return Level.Low;
                //////else
                //////    return Level.High;
                return Level.Low;
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                //////if (Project.Instance.configuration.vitualCard)
                //////{
                //////    D_outputSingalVitualStatu[doName.ToString()] = level;
                //////    return;
                //////}

                //////ushort doIndex = (ushort)Card_Googol.GetDoIndexByName(doName.ToString());
                //////if (level == Level.High)
                //////    Dmc2210.d2210_write_outbit(0, doIndex, 1);
                //////else
                //////    Dmc2210.d2210_write_outbit(0, doIndex, 0);
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                //////Level statu;
                //////do
                //////{
                //////    statu = GetDiSts(diName);
                //////    Thread.Sleep(10);
                //////}
                //////while (statu != level);
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 轴回零
        /// </summary>
        /// <param name="axisIndex">轴索引号</param>
        /// <param name="homeVel">回零速度</param>
        /// <param name="homeDir">回零方向</param>
        /// <param name="backLength">回退距离</param>
        internal static void Home(ushort axisIndex, double homeVel, HomeDir homeDir, Int32 backLength)
        {
            try
            {
                Thread th = new Thread(() =>
                {
                    Config.HomeParam homeParam = new Config.HomeParam();
                    homeParam.HomeType = Config.HomeType.CurrentPos;
                    Wmx3Lib_cm.Config.SetHomeParam(0, homeParam);
                    Wmx3Lib_cm.Home.StartHome(0);
                    Wmx3Lib_cm.Motion.Wait(0);
                });
                th.IsBackground = true;
                th.Start();
            }
            catch (Exception ex)
            {
                //////homing = false;
                //////Log.SaveError(ex);
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


                Motion.PosCommand posCommand = new Motion.PosCommand();
                posCommand.Profile.Type = WMX3ApiCLR.ProfileType.Trapezoidal;
                posCommand.Axis = 0;
                posCommand.Target = 1000000;
                posCommand.Profile.Velocity = 100000;
                posCommand.Profile.StartingVelocity = 0;
                posCommand.Profile.EndVelocity = 0;
                posCommand.Profile.Acc = 1000000;
                posCommand.Profile.Dec = 1000000;
                posCommand.Profile.JerkAcc = 1000;
                posCommand.Profile.JerkAccRatio = 0.5;
                posCommand.Profile.JerkDec = 1000;
                posCommand.Profile.JerkDecRatio = 0.5;

                //-----------------------------------------------------------------
                // Execute command to move from current position to 
                // specified position.
                //-----------------------------------------------------------------
                Wmx3Lib_cm.Motion.StartMov(posCommand);


             
             if (waitDone)
                Wmx3Lib_cm.Motion.Wait(0);
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                //////ushort axisIndex = (ushort)GetAxisIndexByName(axisName.ToString());
                //////double targetPos = GetCurPosition(axisIndex) * Axis_Config.Instance.MMPixelRoute[axisIndex] + distance;
                //////if (targetPos < Axis_Config.Instance.负软极限[axisIndex] || targetPos > Axis_Config.Instance.正软极限[axisIndex])
                //////{
                //////    Frm_Main.Instance.OutputMsg("目标位置超出软极限，运动失败", Color.Red);
                //////    return false;
                //////}
                //////Dmc2210.d2210_set_profile(axisIndex, vel / 2, vel, 0.1, 0.1);

                //////int distance1 = (int)(targetPos / Axis_Config.Instance.MMPixelRoute[axisIndex]);
                //////Dmc2210.d2210_t_pmove(axisIndex, distance1, 0);
                //////if (waitDone)
                //////    WaitMoveDone(axisIndex);
                return true;
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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

                //////ushort axisIndex = (ushort)GetAxisIndexByName(axisName.ToString());

                //////Dmc2210.d2210_set_profile(axisIndex, vel / 2, vel, 0.2, 0.2);
                //////Dmc2210.d2210_t_vmove(axisIndex, dir);

                ////////判断是否超出软极限
                //////Thread th = new Thread(() =>
                //////{
                //////    while (true)
                //////    {
                //////        Thread.Sleep(100);
                //////        double curPosition = GetCurEncoder(axisIndex);
                //////        curPosition = curPosition * Axis_Config.Instance.MMPixelRoute[axisIndex];
                //////        if (curPosition < Axis_Config.Instance.负软极限[axisIndex] || curPosition > Axis_Config.Instance.正软极限[axisIndex])
                //////        {
                //////            Frm_Main.Instance.OutputMsg("超出软极限，运动失败", Color.Red);
                //////            DecStop(axisIndex);
                //////            break;
                //////        }

                //////        //判断是否已停止
                //////        int statu = Dmc2210.d2210_check_done((ushort)axisIndex);
                //////        if (statu != 0)
                //////            break;
                //////    }
                //////});

                //////if (waitDone)
                //////    WaitMoveDone(axisIndex);
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                //////Log.SaveError(ex);
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
                Wmx3Lib_cm.Motion.Wait(0);
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                Wmx3Lib_cm.Motion.Stop(0, 1000000); 
                Wmx3Lib_cm.Motion.Wait(0);
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
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
                Wmx3Lib_cm.GetStatus(ref CmStatus);
                CoreMotionAxisStatus cmAxis = CmStatus.AxesStatus[0];
                return cmAxis .PosCmd ;
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

                double curPos;
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                Wmx3Lib_cm.GetStatus(ref CmStatus);
                CoreMotionAxisStatus cmAxis = CmStatus.AxesStatus[0];
                return cmAxis.ActualPos ;
            }
            catch (Exception ex)
            {
                //////Log.SaveError(ex);
                return 0;
            }
        }
        /// <summary>
        ///      关闭板卡
        /// </summary>
        internal static void CloseBoard()
        {

            Device.StopCommunication(0xFFFFFFFF);

            //Quit device.
            Device.CloseDevice();
        }
    }
}
