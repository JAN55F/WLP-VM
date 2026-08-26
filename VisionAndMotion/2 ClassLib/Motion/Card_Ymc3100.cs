using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csDmc2210;
using System.Threading;
using System.Drawing;
using MotionAPI;
using System.Windows;

namespace VMPro
{
    /// <summary>
    /// 雷赛DMC2210系列运动控制卡控制类
    /// </summary>
    internal class Card_Ymc3100 : CardBase
    {
        private static UInt32 g_hController;
        private static UInt32[] g_hDevice = new UInt32[11];  // Device handle
        private static UInt32[] g_hAxis = new UInt32[11];   // Axis handle

        private static UInt32 controller_LoadPCB;
        private static UInt32[] deviceLoadPCB = new UInt32[11];  // Device handle
        private static UInt32[] axisLoadPCB = new UInt32[11];   // Axis handle

        /// <summary>
        /// 运动前检查回原是否完成，若未回原，不准走位
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        internal static bool CheckHomeOK(Axis axis)
        {
            try
            {
                switch (axis)
                {
                    case Axis.X:
                        double value = Card_Ymc3100.ReadData(MemArea.MB, 100);
                        return value == 1 ? true : false;
                    case Axis.Y:
                        value = Card_Ymc3100.ReadData(MemArea.GB, 203);
                        return value == 1 ? true : false;
                    case Axis.Z:
                        value = Card_Ymc3100.ReadData(MemArea.MB, 130);
                        return value == 1 ? true : false;
                    case Axis.R:
                        value = Card_Ymc3100.ReadData(MemArea.MB, 140);
                        return value == 1 ? true : false;
                    case Axis.TR :
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 初始化板卡 主流程
        /// </summary>
        internal static void Init(bool b = true)
        {
            try
            {
                //虚拟相关
                if (b)
                {
                    foreach (Di item in Enum.GetValues(typeof(Di)))
                    {
                        D_inputSignalVitualStatu.Add(item.ToString(), "01");
                    }
                    foreach (Do item in Enum.GetValues(typeof(Do)))
                    {
                        D_outputSingalVitualStatu.Add(item.ToString(), Level.Low);
                    }
                    if (Project.Instance.configuration.vitualCard)
                    {
                        return;
                    }
                }

                //初始化板卡
                UInt32 rc;
                CMotionAPI.COM_DEVICE ComDevice;

                ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCI_MODE;
                ComDevice.PortNumber = 1;
                ComDevice.CpuNumber = (UInt16)1;	//cpuno;
                ComDevice.NetworkNumber = 0;
                ComDevice.StationNumber = 0;
                ComDevice.UnitNumber = 0;
                ComDevice.IPAddress = "";
                ComDevice.Timeout = 10000;

                rc = CMotionAPI.ymcOpenController(ref ComDevice, ref g_hController);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("板卡初始化出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }

                rc = CMotionAPI.ymcSetAPITimeoutValue(40000);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("设置API超时时间出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }

                rc = CMotionAPI.ymcClearAllAxes();
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("清除轴报警时出错，请机台先断电再通电，然后重启电脑,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }

                //创建各轴句柄
                for (int i = 0; i < (short)11; i++)
                {
                    string AxisName = "Axis-" + (i + 1);
                    rc = CMotionAPI.ymcDeclareAxis(1, 0, 3, (UInt16)(i + 1), (UInt16)(i + 1), (UInt16)CMotionAPI.ApiDefs.REAL_AXIS, AxisName, ref g_hAxis[i]);
                    if (rc != CMotionAPI.MP_SUCCESS)
                    {
                        Frm_MessageBox.Instance.MessageBoxShow("定义轴时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                        return;
                    }
                }


                //////if (b)
                //////{

                UInt32[] lhAxis = new UInt32[1];  // Axis handle
                //创建设备句柄
                for (int i = 0; i < (ushort)11; i++)
                {
                    lhAxis[0] = g_hAxis[i];
                    rc = CMotionAPI.ymcDeclareDevice((UInt16)1, lhAxis, ref g_hDevice[i]);
                    if (rc != CMotionAPI.MP_SUCCESS)
                    {
                        Frm_MessageBox.Instance.MessageBoxShow("定义设备时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                        return;
                    }
                }
                //////}




                //============================================================================ To Contents of Processing
                // Clears all the Machine Controller alarms. 
                //============================================================================
                rc = CMotionAPI.ymcClearAlarm(0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("定义设备时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }





                //////UInt16 k;
                //////UInt32[] hAxis = new UInt32[4];
                //============================================================================ To Contents of Processing
                // Gets the axis handle.
                //============================================================================
                //////for (k = 0; k < (UInt16)4; k++)
                //////{
                //////    rc = CMotionAPI.ymcGetAxisHandle((UInt16)CMotionAPI.ApiDefs.PHYSICALAXIS, 1, 0, 3, (UInt16)(k + 1), 0, "", ref hAxis[k]);
                //////    if (rc != CMotionAPI.MP_SUCCESS)
                //////    {
                //////        Frm_MessageBox.Instance.MessageBoxShow("定义设备时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                //////        return;
                //////    }
                //////}
                ////////============================================================================ To Contents of Processing
                //////// Clears the servo alarm. 
                ////////============================================================================
                //////for (k = 0; k < (short)4; k++)
                //////{
                //////    rc = CMotionAPI.ymcClearServoAlarm(hAxis[k]);
                //////    if (rc != CMotionAPI.MP_SUCCESS)
                //////    {
                //////        Frm_MessageBox.Instance.MessageBoxShow("定义设备时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                //////        return;
                //////    }
                //////}





                if (b)
                {
                    MotorOn(Axis.X);
                    MotorOn(Axis.YL);
                    MotorOn(Axis.YR);
                    MotorOn(Axis.Z);
                    MotorOn(Axis.R);
                    MotorOn(Axis.右侧轨道);
                    MotorOn(Axis.中间轨道);
                    MotorOn(Axis.左侧轨道);
                    MotorOn(Axis.左侧调宽);
                    MotorOn(Axis.右侧调宽);
                    MotorOn(Axis.TR);
                }
                //MotorOff(Axis .X );

                initSucceed = true;
            }
            catch (Exception ex)
            {
                initSucceed = false;
                //Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 初始化板卡 PCB送板
        /// </summary>
        internal static void InitPCB(bool b = true)
        {
            try
            {
                //虚拟相关
                if (b)
                {
                    foreach (Di item in Enum.GetValues(typeof(Di)))
                    {
                        D_inputSignalVitualStatu.Add(item.ToString(), "01");
                    }
                    foreach (Do item in Enum.GetValues(typeof(Do)))
                    {
                        D_outputSingalVitualStatu.Add(item.ToString(), Level.Low);
                    }
                    if (Project.Instance.configuration.vitualCard)
                    {
                        return;
                    }
                }

                //初始化板卡
                UInt32 rc;
                CMotionAPI.COM_DEVICE ComDevice;

                ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCI_MODE;
                ComDevice.PortNumber = 1;
                ComDevice.CpuNumber = (UInt16)1;	//cpuno;
                ComDevice.NetworkNumber = 0;
                ComDevice.StationNumber = 0;
                ComDevice.UnitNumber = 0;
                ComDevice.IPAddress = "";
                ComDevice.Timeout = 10000;

                rc = CMotionAPI.ymcOpenController(ref ComDevice, ref controller_LoadPCB);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("板卡初始化出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }

                rc = CMotionAPI.ymcSetAPITimeoutValue(40000);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("设置API超时时间出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }

                rc = CMotionAPI.ymcClearAllAxes();
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("设置API超时时间出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }

                //创建各轴句柄
                for (int i = 0; i < (short)10; i++)
                {
                    string AxisName = "Axis-" + (i + 1);
                    rc = CMotionAPI.ymcDeclareAxis(1, 0, 3, (UInt16)(i + 1), (UInt16)(i + 1), (UInt16)CMotionAPI.ApiDefs.REAL_AXIS, AxisName, ref axisLoadPCB[i]);
                    if (rc != CMotionAPI.MP_SUCCESS)
                    {
                        Frm_MessageBox.Instance.MessageBoxShow("定义轴时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                        return;
                    }
                }


                //////if (b)
                //////{

                UInt32[] lhAxis = new UInt32[1];  // Axis handle
                //创建设备句柄
                for (int i = 0; i < (ushort)10; i++)
                {
                    lhAxis[0] = axisLoadPCB[i];
                    rc = CMotionAPI.ymcDeclareDevice((UInt16)1, lhAxis, ref deviceLoadPCB[i]);
                    if (rc != CMotionAPI.MP_SUCCESS)
                    {
                        Frm_MessageBox.Instance.MessageBoxShow("定义设备时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                        return;
                    }
                }
                //////}




                //============================================================================ To Contents of Processing
                // Clears all the Machine Controller alarms. 
                //============================================================================
                rc = CMotionAPI.ymcClearAlarm(0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("定义设备时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }





                //////UInt16 k;
                //////UInt32[] hAxis = new UInt32[4];
                //============================================================================ To Contents of Processing
                // Gets the axis handle.
                //============================================================================
                //////for (k = 0; k < (UInt16)4; k++)
                //////{
                //////    rc = CMotionAPI.ymcGetAxisHandle((UInt16)CMotionAPI.ApiDefs.PHYSICALAXIS, 1, 0, 3, (UInt16)(k + 1), 0, "", ref hAxis[k]);
                //////    if (rc != CMotionAPI.MP_SUCCESS)
                //////    {
                //////        Frm_MessageBox.Instance.MessageBoxShow("定义设备时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                //////        return;
                //////    }
                //////}
                ////////============================================================================ To Contents of Processing
                //////// Clears the servo alarm. 
                ////////============================================================================
                //////for (k = 0; k < (short)4; k++)
                //////{
                //////    rc = CMotionAPI.ymcClearServoAlarm(hAxis[k]);
                //////    if (rc != CMotionAPI.MP_SUCCESS)
                //////    {
                //////        Frm_MessageBox.Instance.MessageBoxShow("定义设备时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                //////        return;
                //////    }
                //////}





                if (b)
                {
                    MotorOn(Axis.X);
                    MotorOn(Axis.YL);
                    MotorOn(Axis.YR);
                    MotorOn(Axis.Z);
                    MotorOn(Axis.R);
                    MotorOn(Axis.右侧轨道);
                    MotorOn(Axis.中间轨道);
                    MotorOn(Axis.左侧轨道);
                    MotorOn(Axis.左侧调宽);
                    MotorOn(Axis.右侧调宽);
                }
                //MotorOff(Axis .X );

                initSucceed = true;
            }
            catch (Exception ex)
            {
                initSucceed = false;
                //Log.SaveError(ex);
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
                    //////if (Project.Instance.configuration.vitualCard)
                    //////    return false;

                    //////ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                    //////bool result = GetBit16(temp, 14);
                    //////if (Axis_Config.Instance.原点逻辑电平[axisIndex] == LogicLevel.低电平有效)
                    //////{
                    //////    if (result)
                    //////        return true;
                    //////    else
                    //////        return false;
                    //////}
                    //////else
                    //////{
                    //////    if (result)
                    //////        return true;
                    //////    else
                    //////        return false;
                    //////}
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        internal static bool WriteData(MemArea memArea, int add, double value)
        {
            try
            {
                // Init(false);

                // Definition of motion API variables
                ulong hRegister_ML;                   // Register data handle for ML register
                UInt32 hRegister_MB;                   // Register data handle for MB register
                UInt32 hRegister_OW;                   // Register data handle for OW register
                UInt32 hRegister_OB;                   // Register data handle for OB register
                String cRegisterName_ML;               // ML register name storage variable
                String cRegisterName_MB;               // MB register name storage variable
                String cRegisterName_OW;               // OW register name storage variable
                String cRegisterName_OB;               // OB register name storage variable
                UInt32 RegisterDataNumber;             // Number of read-in registers
                UInt16[] Reg_ShortData = new UInt16[3];  // W or B size register data storage variable
                UInt32[] Reg_LongData = new UInt32[3];   // L size register data storage variable
                UInt32 rc;                             // Motion API return value

                hRegister_ML = 0x00000000;
                hRegister_MB = 0x00000000;
                hRegister_OW = 0x00000000;
                hRegister_OB = 0x00000000;

                //============================================================================
                // Gets the register name.
                //============================================================================
                // ML Register
                cRegisterName_ML = memArea.ToString() + add;

                //============================================================================ To Contents of Processing
                // Gets the register data handle.	
                // The obtained register number can be used in other threads.
                //============================================================================
                // ML Register
                if (memArea == MemArea.GB)
                {
                    rc = CMotionAPI.ymcGetRegisterDataHandleEx(cRegisterName_ML, ref hRegister_ML);
                }
                else
                {
                    rc = CMotionAPI.ymcGetRegisterDataHandle(cRegisterName_ML, ref hRegister_MB);
                }
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcGetRegisterDataHandle ML \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }


                //============================================================================ To Contents of Processing
                // Writes the set data into the set register.
                //============================================================================

                // MB Register
                Reg_ShortData[0] = (ushort)value;
                RegisterDataNumber = 1;
                if (memArea == MemArea.GB)
                {
                    rc = CMotionAPI.ymcSetRegisterDataEx(hRegister_ML, RegisterDataNumber, Reg_ShortData);
                }
                else
                {
                    rc = CMotionAPI.ymcSetRegisterData(hRegister_MB, RegisterDataNumber, Reg_ShortData);
                }
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcSetRegisterData MB \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }

                //写入检查
                double result = ReadData(memArea, add);
                if (result != value)
                {
                    MessageBox.Show("参数写入失败");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        internal static bool WriteData(MemArea memArea, string add, double value)
        {
            try
            {
                // Init(false);

                // Definition of motion API variables
                ulong hRegister_ML;                   // Register data handle for ML register
                UInt32 hRegister_MB;                   // Register data handle for MB register
                UInt32 hRegister_OW;                   // Register data handle for OW register
                UInt32 hRegister_OB;                   // Register data handle for OB register
                String cRegisterName_ML;               // ML register name storage variable
                String cRegisterName_MB;               // MB register name storage variable
                String cRegisterName_OW;               // OW register name storage variable
                String cRegisterName_OB;               // OB register name storage variable
                UInt32 RegisterDataNumber;             // Number of read-in registers
                UInt16[] Reg_ShortData = new UInt16[3];  // W or B size register data storage variable
                UInt32[] Reg_LongData = new UInt32[3];   // L size register data storage variable
                UInt32 rc;                             // Motion API return value

                hRegister_ML = 0x00000000;
                hRegister_MB = 0x00000000;
                hRegister_OW = 0x00000000;
                hRegister_OB = 0x00000000;

                //============================================================================
                // Gets the register name.
                //============================================================================
                // ML Register
                cRegisterName_ML = memArea.ToString() + add;

                //============================================================================ To Contents of Processing
                // Gets the register data handle.	
                // The obtained register number can be used in other threads.
                //============================================================================
                // ML Register
                if (memArea == MemArea.GB)
                {
                    rc = CMotionAPI.ymcGetRegisterDataHandleEx(cRegisterName_ML, ref hRegister_ML);
                }
                else
                {
                    rc = CMotionAPI.ymcGetRegisterDataHandle(cRegisterName_ML, ref hRegister_MB);
                }
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcGetRegisterDataHandle ML \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }


                //============================================================================ To Contents of Processing
                // Writes the set data into the set register.
                //============================================================================

                // MB Register
                Reg_ShortData[0] = (ushort)value;
                RegisterDataNumber = 1;
                if (memArea == MemArea.GB)
                {
                    rc = CMotionAPI.ymcSetRegisterDataEx(hRegister_ML, RegisterDataNumber, Reg_ShortData);
                }
                else
                {
                    rc = CMotionAPI.ymcSetRegisterData(hRegister_MB, RegisterDataNumber, Reg_ShortData);
                }
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcSetRegisterData MB \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }

                //写入检查
                double result = ReadData(memArea, add);
                if (result != value)
                {
                    MessageBox.Show("参数写入失败");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        internal static double ReadData(MemArea memArea, string add)
        {
            try
            {
                // Init(false);

                // Definition of motion API variables
                ulong hRegister_ML;                   // Register data handle for ML register
                UInt32 hRegister_MB;                   // Register data handle for MB register
                UInt32 hRegister_OW;                   // Register data handle for OW register
                UInt32 hRegister_OB;                   // Register data handle for OB register
                String cRegisterName_ML;               // ML register name storage variable
                String cRegisterName_MB;               // MB register name storage variable
                String cRegisterName_OW;               // OW register name storage variable
                String cRegisterName_OB;               // OB register name storage variable
                UInt32 RegisterDataNumber;             // Number of read-in registers
                uint ReadDataNumber;                 // Number of obtained registers
                UInt16[] Reg_ShortData = new UInt16[3];  // W or B size register data storage variable
                UInt32[] Reg_LongData = new UInt32[3];   // L size register data storage variable
                UInt32 rc;                             // Motion API return value

                hRegister_ML = 0x00000000;
                hRegister_MB = 0x00000000;
                hRegister_OW = 0x00000000;
                hRegister_OB = 0x00000000;
                ReadDataNumber = 00000000;

                //============================================================================
                // Gets the register name.
                //============================================================================
                // ML Register
                cRegisterName_ML = memArea.ToString() + add;


                //============================================================================ To Contents of Processing
                // Gets the register data handle.
                // The obtained register number can be used in other threads.
                //============================================================================
                // ML Register
                if (memArea == MemArea.GB)
                    rc = CMotionAPI.ymcGetRegisterDataHandleEx(cRegisterName_ML, ref hRegister_ML);
                else
                    rc = CMotionAPI.ymcGetRegisterDataHandle(cRegisterName_ML, ref hRegister_MB);

                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcGetRegisterDataHandle ML \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return 0;
                }


                // MB Register
                RegisterDataNumber = 1;
                if (memArea == MemArea.GB)
                    rc = CMotionAPI.ymcGetRegisterDataEx(hRegister_ML, RegisterDataNumber, Reg_ShortData, ref ReadDataNumber);
                else
                    rc = CMotionAPI.ymcGetRegisterData(hRegister_MB, RegisterDataNumber, Reg_ShortData, ref ReadDataNumber);

                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcGetRegisterData MB \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return 0;
                }
                return Reg_ShortData[0];
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return 0;
            }
        }
        internal static double ReadData(MemArea memArea, int add)
        {
            try
            {
                // Init(false);

                // Definition of motion API variables
                ulong hRegister_ML;                   // Register data handle for ML register
                UInt32 hRegister_MB;                   // Register data handle for MB register
                UInt32 hRegister_OW;                   // Register data handle for OW register
                UInt32 hRegister_OB;                   // Register data handle for OB register
                String cRegisterName_ML;               // ML register name storage variable
                String cRegisterName_MB;               // MB register name storage variable
                String cRegisterName_OW;               // OW register name storage variable
                String cRegisterName_OB;               // OB register name storage variable
                UInt32 RegisterDataNumber;             // Number of read-in registers
                uint ReadDataNumber;                 // Number of obtained registers
                Int32[] Reg_ShortData = new Int32[3];  // W or B size register data storage variable
                Int32[] Reg_LongData = new Int32[3];   // L size register data storage variable
                UInt32 rc;                             // Motion API return value

                hRegister_ML = 0x00000000;
                hRegister_MB = 0x00000000;
                hRegister_OW = 0x00000000;
                hRegister_OB = 0x00000000;
                ReadDataNumber = 00000000;

                //============================================================================
                // Gets the register name.
                //============================================================================
                // ML Register
                cRegisterName_ML = memArea.ToString() + add;


                //============================================================================ To Contents of Processing
                // Gets the register data handle.
                // The obtained register number can be used in other threads.
                //============================================================================
                // ML Register
                if (memArea == MemArea.GB)
                    rc = CMotionAPI.ymcGetRegisterDataHandleEx(cRegisterName_ML, ref hRegister_ML);
                else
                    rc = CMotionAPI.ymcGetRegisterDataHandle(cRegisterName_ML, ref hRegister_MB);

                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcGetRegisterDataHandle ML \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return 0;
                }


                // MB Register
                RegisterDataNumber = 1;
                if (memArea == MemArea.GB)
                    rc = CMotionAPI.ymcGetRegisterDataEx(hRegister_ML, RegisterDataNumber, Reg_ShortData, ref ReadDataNumber);
                else
                    rc = CMotionAPI.ymcGetRegisterData(hRegister_MB, RegisterDataNumber, Reg_ShortData, ref ReadDataNumber);

                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcGetRegisterData MB \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return 0;
                }
                return Reg_ShortData[0];
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return 0;
            }
        }
        internal static void YMoveRel(double distance, int vel)
        {
            MoveInterpolateRel(new Axis[] { Axis.YL, Axis.YR }, new int[] { (int)(distance * 1000), (int)(distance * 1000) }, (int)(vel * 1000 * 1.5));
        }
        internal static void PCBWidthMoveRel(double distance, int vel)
        {
            MoveInterpolateRel(new Axis[] { Axis.左侧调宽, Axis.右侧调宽 }, new int[] { (int)(distance * 1000), (int)(distance * 1000) }, (int)(vel * 1000 * 1.5));
        }
        internal static void YMoveAbs(double distance, int vel)
        {
            if (!FindAxisByName(Axis.YR).homeOK || !CheckHomeOK(Axis.Y))
            {
                Frm_MessageBox.Instance.MessageBoxShow("轴运行失败，轴未回零，请回零后尝试");
                return;
            }

            distance = distance * 1000;
            vel = (int)(vel * 1000 * 1.5);
            MoveInterpolateAbs(new Axis[] { Axis.YL, Axis.YR }, new double[] { distance, distance }, vel);
        }
        internal static bool MoveInterpolateRel(Axis[] axis, int[] L_distance, int vel)
        {
            try
            {

                // Init(false );

                UInt32[] g_hAxis = new UInt32[10];   // Axis handle
                String AxisName;   // Axis name

                CMotionAPI.COM_DEVICE ComDevice;                              // The ymcOpenController setting structure
                UInt32 hController;                            // Controller handle
                UInt32 hDevice111;                                // Device handle
                CMotionAPI.MOTION_DATA MotionData;                             // MOTION_DATA structure
                CMotionAPI.POSITION_DATA[] Pos = new CMotionAPI.POSITION_DATA[5];  // POSITION_DATA structure
                UInt16 WaitForCompletion;                      // Completion attribute storage variable
                UInt32 rc;                                     // Motion API return value
                Int16 i;                                      // Index of number of axes
                Int32[] PosData = new Int32[5];                 // Target position storage variable

                //============================================================================ To Contents of Processing
                // Establishes the communications with the Machine Controller.
                // Must be called for each thread.
                //============================================================================
                hController = new UInt32();
                ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCI_MODE;
                ComDevice.PortNumber = 1;
                ComDevice.CpuNumber = (UInt16)1;	//cpuno;
                ComDevice.NetworkNumber = 0;
                ComDevice.StationNumber = 0;
                ComDevice.UnitNumber = 0;
                ComDevice.IPAddress = "";
                ComDevice.Timeout = 10000;

                rc = CMotionAPI.ymcOpenController(ref ComDevice, ref hController);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcOpenController Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }

                //============================================================================ To Contents of Processing
                // Sets the motion API timeout.			
                // Prevents the host application from freezing due to Machine Controller fault.
                // Must be called for each thread.
                //============================================================================
                rc = CMotionAPI.ymcSetAPITimeoutValue(30000);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error SetAPITimeoutValue \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }
                //============================================================================ To Contents of Processing
                // Creates a device handle using the axis handle that has been defined as global data.
                // The following sets the number of connected axes as one device.
                // The device handle is needed for each thread.
                //============================================================================

                ushort axisIndex1 = (ushort)FindAxisByName(axis[0].ToString()).actNo;
                ushort axisIndex2 = (ushort)FindAxisByName(axis[1].ToString()).actNo;

                rc = CMotionAPI.ymcClearAllAxes();
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("设置API超时时间出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return false;
                }

                for (i = 0; i < (Int16)10; i++)
                {
                    AxisName = "Axis-" + (i + 1);
                    rc = CMotionAPI.ymcDeclareAxis(1, 0, 3, (UInt16)(i + 1), (UInt16)(i + 1), (UInt16)CMotionAPI.ApiDefs.REAL_AXIS, AxisName, ref g_hAxis[i]);
                    if (rc != CMotionAPI.MP_SUCCESS)
                    {
                        MessageBox.Show(String.Format("Error ymcDeclareAxis \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                        return false;
                    }
                }


                UInt32[] g_hAxis111 = new UInt32[2];   // Axis handle
                g_hAxis111[0] = g_hAxis[axisIndex1];
                g_hAxis111[1] = g_hAxis[axisIndex2];

                hDevice111 = new UInt32();
                rc = CMotionAPI.ymcDeclareDevice((UInt16)2, g_hAxis111, ref hDevice111);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcDeclareDevice \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }
                //============================================================================ To Contents of Processing
                // Sets the interpolation parameter.
                // Sets the speed, target position, acceleration and deceleration set in the window.
                //============================================================================

                MotionData = new CMotionAPI.MOTION_DATA();
                MotionData.CoordinateSystem = (Int16)CMotionAPI.ApiDefs.WORK_SYSTEM;     // Work coordinate system
                MotionData.MoveType = (Int16)CMotionAPI.ApiDefs.MTYPE_RELATIVE;  // Incremental value specified
                MotionData.VelocityType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;  // Speed [reference unit/s]
                MotionData.AccDecType = (Int16)CMotionAPI.ApiDefs.ATYPE_TIME;      // Time constant specified [ms]
                MotionData.FilterType = (Int16)CMotionAPI.ApiDefs.FTYPE_S_CURVE;   // Moving average filter (simplified S-curve)
                MotionData.DataType = 0;                                         // All parameters directly specified
                MotionData.MaxVelocity = 100000;		    // Max. feeding speed [reference unit/s]
                MotionData.Acceleration = Int32.Parse("100");			    // Acceleration time constant [ms] 
                MotionData.Deceleration = Int32.Parse("100");			    // Deceleration time constant [ms]
                MotionData.FilterTime = 10;                                        // Filter time [0.1 ms]
                MotionData.Velocity = vel;			        // Speed [reference unit/s]
                /* Not Use MotionData.ApproachVelocity = NULL; */
                /* Not Use MotionData.CreepVelocity    = NULL; */

                i = 0;
                PosData[i] = L_distance[0];
                i = 1;
                PosData[i] = L_distance[1];
                i = 2;
                PosData[i] = Int32.Parse("1000");

                // Position data setting
                for (i = 0; i < (Int16)2; i++)
                {
                    Pos[i].DataType = (Int16)CMotionAPI.ApiDefs.DATATYPE_IMMEDIATE;
                    Pos[i].PositionData = PosData[i];
                }
                // Completion attribute
                WaitForCompletion = (UInt16)CMotionAPI.ApiDefs.POSITIONING_COMPLETED;

                rc = CMotionAPI.ymcMoveLinear(hDevice111, ref MotionData, Pos, 0, "Start", WaitForCompletion, 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcMoveLinear \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }

                //============================================================================ To Contents of Processing
                // Deletes the device handle created in this thread.
                //============================================================================


                //============================================================================ To Contents of Processing
                // Interrupts the communications with the motion API.
                //============================================================================

                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        internal static bool MoveInterpolateAbs(Axis[] axis, double[] L_distance, int vel)
        {
            try
            {
                if (!FindAxisByName(Axis.YR).homeOK || !CheckHomeOK(Axis.Y))
                {
                    Frm_MessageBox.Instance.MessageBoxShow("轴运行失败，轴未回零，请回零后尝试");
                    return false;
                }
                // Init(false );

                UInt32[] g_hAxis = new UInt32[5];   // Axis handle
                String AxisName;   // Axis name

                CMotionAPI.COM_DEVICE ComDevice;                              // The ymcOpenController setting structure
                UInt32 hController;                            // Controller handle
                UInt32 hDevice111 = 0;                                // Device handle
                CMotionAPI.MOTION_DATA MotionData;                             // MOTION_DATA structure
                CMotionAPI.POSITION_DATA[] Pos = new CMotionAPI.POSITION_DATA[5];  // POSITION_DATA structure
                UInt16 WaitForCompletion;                      // Completion attribute storage variable
                UInt32 rc;                                     // Motion API return value
                Int16 i;                                      // Index of number of axes
                Int32[] PosData = new Int32[5];                 // Target position storage variable

                //============================================================================ To Contents of Processing
                // Establishes the communications with the Machine Controller.
                // Must be called for each thread.
                //============================================================================
                //////hController = new UInt32();
                //////ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCI_MODE;
                //////ComDevice.PortNumber = 1;
                //////ComDevice.CpuNumber = (UInt16)1;	//cpuno;
                //////ComDevice.NetworkNumber = 0;
                //////ComDevice.StationNumber = 0;
                //////ComDevice.UnitNumber = 0;
                //////ComDevice.IPAddress = "";
                //////ComDevice.Timeout = 10000;

                //////rc = CMotionAPI.ymcOpenController(ref ComDevice, ref hController);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcOpenController Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return false;
                //////}

                //============================================================================ To Contents of Processing
                // Sets the motion API timeout.			
                // Prevents the host application from freezing due to Machine Controller fault.
                // Must be called for each thread.
                //============================================================================
                //////rc = CMotionAPI.ymcSetAPITimeoutValue(30000);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error SetAPITimeoutValue \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return false;
                //////}
                //============================================================================ To Contents of Processing
                // Creates a device handle using the axis handle that has been defined as global data.
                // The following sets the number of connected axes as one device.
                // The device handle is needed for each thread.
                //============================================================================

                ushort axisIndex1 = (ushort)FindAxisByName(axis[0].ToString()).actNo;
                ushort axisIndex2 = (ushort)FindAxisByName(axis[1].ToString()).actNo;

                rc = CMotionAPI.ymcClearAllAxes();
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("设置API超时时间出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return false;
                }

                for (i = 0; i < (Int16)5; i++)
                {
                    AxisName = "Axis-" + (i + 1);
                    rc = CMotionAPI.ymcDeclareAxis(1, 0, 3, (UInt16)(i + 1), (UInt16)(i + 1), (UInt16)CMotionAPI.ApiDefs.REAL_AXIS, AxisName, ref g_hAxis[i]);
                    if (rc != CMotionAPI.MP_SUCCESS)
                    {
                        MessageBox.Show(String.Format("Error ymcDeclareAxis \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                        return false;
                    }
                }


                UInt32[] g_hAxis111 = new UInt32[2];   // Axis handle
                g_hAxis111[0] = g_hAxis[axisIndex1];
                g_hAxis111[1] = g_hAxis[axisIndex2];



                hDevice111 = new UInt32();
                rc = CMotionAPI.ymcDeclareDevice((UInt16)2, g_hAxis111, ref hDevice111);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcDeclareDevice \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }
                //============================================================================ To Contents of Processing
                // Sets the interpolation parameter.
                // Sets the speed, target position, acceleration and deceleration set in the window.
                //============================================================================

                MotionData = new CMotionAPI.MOTION_DATA();
                MotionData.CoordinateSystem = (Int16)CMotionAPI.ApiDefs.WORK_SYSTEM;     // Work coordinate system
                MotionData.MoveType = (Int16)CMotionAPI.ApiDefs.MTYPE_ABSOLUTE;  // Incremental value specified
                MotionData.VelocityType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;  // Speed [reference unit/s]
                MotionData.AccDecType = (Int16)CMotionAPI.ApiDefs.ATYPE_TIME;      // Time constant specified [ms]
                MotionData.FilterType = (Int16)CMotionAPI.ApiDefs.FTYPE_S_CURVE;   // Moving average filter (simplified S-curve)
                MotionData.DataType = 0;                                         // All parameters directly specified
                MotionData.MaxVelocity = 100000;		    // Max. feeding speed [reference unit/s]
                MotionData.Acceleration = Int32.Parse("100");			    // Acceleration time constant [ms] 
                MotionData.Deceleration = Int32.Parse("100");			    // Deceleration time constant [ms]
                MotionData.FilterTime = 10;                                        // Filter time [0.1 ms]
                MotionData.Velocity = vel;			        // Speed [reference unit/s]
                /* Not Use MotionData.ApproachVelocity = NULL; */
                /* Not Use MotionData.CreepVelocity    = NULL; */

                i = 0;
                PosData[i] = (int)L_distance[0];
                i = 1;
                PosData[i] = (int)L_distance[1];
                i = 2;
                PosData[i] = Int32.Parse("1000");

                // Position data setting
                for (i = 0; i < (Int16)2; i++)
                {
                    Pos[i].DataType = (Int16)CMotionAPI.ApiDefs.DATATYPE_IMMEDIATE;
                    Pos[i].PositionData = PosData[i];
                }
                // Completion attribute
                WaitForCompletion = (UInt16)CMotionAPI.ApiDefs.POSITIONING_COMPLETED;

                rc = CMotionAPI.ymcMoveLinear(hDevice111, ref MotionData, Pos, 0, "Start", WaitForCompletion, 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcMoveLinear \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }

                rc = CMotionAPI.ymcClearDevice(hDevice111);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcDeclareDevice \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }

                //============================================================================ To Contents of Processing
                // Deletes the device handle created in this thread.
                //============================================================================


                //============================================================================ To Contents of Processing
                // Interrupts the communications with the motion API.
                //============================================================================

                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        internal static bool ReadDataWait(MemArea memArea, int add, double value)
        {
            try
            {
                double result = 0;
                do
                {
                    result = ReadData(memArea, add);
                }
                while (result != value);

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
                lock (obj_card)
                {
                    //////if (Project.Instance.configuration.vitualCard)
                    //////    return true;

                    //////ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                    //////bool result = GetBit16(temp, 12);
                    //////if (result)
                    //////    return true;
                    //////else
                    //////    return false;
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
                    //////if (Project.Instance.configuration.vitualCard)
                    //////    return true;

                    //////ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                    //////bool result = GetBit16(temp, 13);
                    //////if (result)
                    //////    return true;
                    //////else
                    //////    return false;
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
                    //////if (Project.Instance.configuration.vitualCard)
                    //////    return false;

                    //////ushort temp = Dmc2210.d2210_axis_io_status(axisIndex);
                    //////bool result = GetBit16(temp, 11);
                    //////if (result)
                    //////    return true;
                    //////else
                    //////    return false;
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
                //////if (Project.Instance.configuration.vitualCard)
                //////{
                //////    return false;
                //////}

                //////int result = Dmc2210.d2210_read_SEVON_PIN(axisIndex);
                //////if (result == 1)
                //////    return false;
                //////else
                //////    return true;
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
                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                UInt32 rc = CMotionAPI.ymcServoControl(g_hDevice[axisIndex], (UInt16)CMotionAPI.ApiDefs.SERVO_ON, 5000);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("伺服使能时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }
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
                UInt32 rc = CMotionAPI.ymcServoControl(g_hDevice[axisIndex], (UInt16)CMotionAPI.ApiDefs.SERVO_OFF, 5000);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("伺服使能时出错,错误编号：" + rc.ToString("X"), TipType.Error);
                    return;
                }


                //Dmc2210.d2210_write_SEVON_PIN(axisIndex, 1);
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
        internal static Level GetDi(Di di)
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
                string axisIndex = GetDiIndexByName(di);
                double value = ReadData(MemArea.IB, axisIndex);
                if (value == 1)
                    return Level.High;
                else
                    return Level.Low;
                return Level.Low;
                //////}
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
        internal static Level GetDo(Do doo)
        {
            try
            {
                ////if (Project.Instance.configuration.vitualCard)        //如果板卡虚拟，则一律返回低电平
                ////    return D_outputSingalVitualStatu[doName.ToString()];

                //////ushort doIndex = (ushort)GetDoIndexByName(doName.ToString());
                //////int value = Dmc2210.d2210_read_inbit(0, doIndex);
                //////if (value == 0)
                //////    return Level.Low;
                //////else
                //////    return Level.High;


                string axisIndex = GetDoIndexByName(doo);
                double value = ReadData(MemArea.OB, axisIndex);
                if (value == 1)
                    return Level.High;
                else
                    return Level.Low;
                return Level.Low;
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
        internal static void SetDo(Do doo, Level level)
        {
            try
            {
                if (Project.Instance.configuration.vitualCard)
                {
                    D_outputSingalVitualStatu[doo.ToString()] = level;
                    return;
                }

                string doIndex = Card_Googol.GetDoIndexByName(doo.ToString());
                if (level == Level.High)
                    WriteData(MemArea.OB, doIndex, 1);
                else
                    WriteData(MemArea.OB, doIndex, 0);
                //////if (level == Level.High)
                //////    Dmc2210.d2210_write_outbit(0, doIndex, 1);
                //////else
                //////    Dmc2210.d2210_write_outbit(0, doIndex, 0);
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
                //////do
                //////{
                //////    statu = GetDiSts(diName);
                //////    Thread.Sleep(10);
                //////}
                //////while (statu != level);
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
        internal static void Home(Axis axis, double homeVel, HomeDir homeDir, Int32 backLength)
        {
            try
            {
                //////Thread th = new Thread(() =>
                //////{
                ushort doIndex = Card_Googol.FindAxisByName(axis.ToString()).actNo;
                homing = true;
                HomePosition((short)doIndex, (UInt16)CMotionAPI.ApiDefs.HMETHOD_C, homeDir == HomeDir.P_正方向 ? (UInt16)CMotionAPI.ApiDefs.DIRECTION_POSITIVE : (UInt16)CMotionAPI.ApiDefs.DIRECTION_NEGATIVE, 10000, 10000, 10000, 10000);
                homing = false;
                //////});
                //////th.IsBackground = true;
                //////th.Start();
            }
            catch (Exception ex)
            {
                homing = false;
                Log.SaveError(ex);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////
        //	HomePosition
        //		HomePosition Proc.
        //////////////////////////////////////////////////////////////////////////////////////////////////
        internal static void HomePosition(Int16 arg_AxisNo, UInt16 arg_HomeMethod, UInt16 arg_Direction, Int32 arg_Velocity, Int32 arg_Approach, Int32 arg_Creep, Int32 arg_Position)
        {
            //Init(false);

            CMotionAPI.MOTION_DATA[] MotionData = new CMotionAPI.MOTION_DATA[1]; // MOTION_DATA structure
            CMotionAPI.POSITION_DATA[] Pos = new CMotionAPI.POSITION_DATA[1];      // POSITION_DATA structure
            UInt16[] HomeMethod = new UInt16[1];     // Zero point return method
            UInt16[] Direction = new UInt16[1];     // Moving direction
            UInt16[] WaitForCompletion = new UInt16[1];    // Completion attribute storage variable
            UInt32 rc;                   // Motion API return value

            //============================================================================ To Contents of Processing
            // Executes servo ON.
            //============================================================================
            //rc = CMotionAPI.ymcServoControl(g_hDevice[arg_AxisNo - 1], (UInt16)CMotionAPI.ApiDefs.SERVO_ON, 5000);
            //if (rc != CMotionAPI.MP_SUCCESS)
            //{
            //    MessageBox.Show(String.Format("Error ymcServoControl \nErrorCode [ 0x{0} ]", rc.ToString("X")));
            //    return;
            //}

            //============================================================================ To Contents of Processing
            // Performs zero point return.
            //============================================================================
            // Zero point return method == Phase C pulse method
            arg_HomeMethod = (UInt16)CMotionAPI.ApiDefs.HMETHOD_NOT_C;
            if (arg_HomeMethod == (UInt16)CMotionAPI.ApiDefs.HMETHOD_C)
            {
                //Motion data setting at phase C pulse method
                /* Not Use MotionData[0].CoordinateSystem = NULL; */
                /* Not Use MotionData[0].MoveType         = NULL; */
                MotionData[0].VelocityType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;    // Speed [reference unit/s]
                MotionData[0].AccDecType = (Int16)CMotionAPI.ApiDefs.ATYPE_TIME;        // Time constant specified [ms]
                /* Not Use MotionData[0].FilterType       = NULL; */
                MotionData[0].DataType = 0;                 // All parameters directly specified
                /* MotionData[0].MaxVelocity              = NULL; */
                MotionData[0].Acceleration = 100;               // Acceleration time constant [ms] 
                MotionData[0].Deceleration = 100;               // Deceleration time constant [ms]
                /* Not Use MotionData[0].FilterTime       = NULL; */
                /* Not Use MotionData[0].Velocity         = NULL; */
                MotionData[0].ApproachVelocity = arg_Approach;     // Approach speed [reference unit/s]
                MotionData[0].CreepVelocity = arg_Creep;        // Creep speed [reference unit/s]
            }
            // Zero point return method == INPUT & phase C pulse method 
            else if (arg_HomeMethod == (UInt16)CMotionAPI.ApiDefs.HMETHOD_NOT_C)
            {
                // Motion data setting at INPUT & phase C pulse method
                /* Not Use MotionData[0].CoordinateSystem = NULL; */
                //MotionData[0].MoveType = (Int16)CMotionAPI.ApiDefs.MTYPE_R_NEGATIVE; 
                MotionData[0].VelocityType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;    // Speed [reference unit/s]
                MotionData[0].AccDecType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;        // Time constant specified [ms]
                /* Not Use MotionData[0].FilterType       = NULL; */
                MotionData[0].DataType = 0;                 // All parameters directly specified
                // MotionData[0].MaxVelocity              = 1000; 
                MotionData[0].Acceleration = 10000;               // Acceleration time constant [ms] 
                MotionData[0].Deceleration = 10000;               // Deceleration time constant [ms]
                /* Not Use MotionData[0].FilterTime       = NULL; */
                MotionData[0].Velocity = arg_Velocity;      // Speed [reference unit/s]
                MotionData[0].ApproachVelocity = -arg_Approach;      // Approach speed [reference unit/s]
                MotionData[0].CreepVelocity = arg_Creep;         // Creep speed [reference unit/s]
            }
            // Position data setting
            Pos[0].DataType = (UInt16)CMotionAPI.ApiDefs.DATATYPE_IMMEDIATE;
            Pos[0].PositionData = arg_Position;

            HomeMethod[0] = arg_HomeMethod;     // Zero point return method
            Direction[0] = arg_Direction;     // Moving direction

            // By setting the completion attribute to "COMMAND_STARTED (starting the command),"
            // the control returns to the application immediately after positioning command execution. 
            WaitForCompletion[0] = (UInt16)CMotionAPI.ApiDefs.COMMAND_STARTED;

            rc = CMotionAPI.ymcMoveHomePosition(g_hDevice[arg_AxisNo], MotionData, Pos, HomeMethod, Direction, 0, "Start", WaitForCompletion, 0);
            if (rc != CMotionAPI.MP_SUCCESS)
            {
                MessageBox.Show(String.Format("Error ymcMoveHomePositioning \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                return;
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
        internal static void MoveAbs(object axisName, double targetPos, int vel, bool waitDone)
        {
            try
            {
                if (!FindAxisByName((Axis)Enum.Parse(typeof(Axis), axisName.ToString())).homeOK)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("轴运行失败，轴未回零，请回零后尝试");
                    return;
                }
                if (!CheckHomeOK((Axis)Enum.Parse(typeof(Axis), axisName.ToString())))
                {
                    Frm_MessageBox.Instance.MessageBoxShow("轴运行失败，轴未回零，请回零后尝试");
                    return;
                }

                targetPos = targetPos * 1000;
                vel = vel * 1000;
                // Init(false);

                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                // Definition of Motion API Variables
                CMotionAPI.COM_DEVICE ComDevice;                                  // The ymcOpenController setting structure
                UInt32[] hAxis = new UInt32[3];                      // Axis handle (for 3 axes)
                CMotionAPI.MOTION_DATA[] MotionData = new CMotionAPI.MOTION_DATA[3]; // MOTION_DATA structure (for 3 axes)
                CMotionAPI.POSITION_DATA[] Pos = new CMotionAPI.POSITION_DATA[3];      // POSITION_DATA structure (for 3 axes)
                UInt16[] WaitForCompletion = new UInt16[3];          // Completion attribute storage variable (for 3 axes)
                UInt32 rc;                                         // Motion API return value
                Int16 i;                                          // Index of number of axes
                String AxisName;                                   // Axis name
                Int32[] VelData = new Int32[3];                     // Speed storage variable (for 3 axes)
                Int32[] PosData = new Int32[3];                     // Target position storage variable (for 3 axes)
                Int32[] AccData = new Int32[3];                     // Acceleration storage variable (for 3 axes)
                Int32[] DecData = new Int32[3];                     // Deceleration storage variable (for 3 axes)

                //============================================================================ To Contents of Processing
                // Establishes the communications with the Machine Controller.
                // Must be called for each thread.
                //============================================================================
                //////ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCI_MODE;
                //////ComDevice.PortNumber = 1;
                //////ComDevice.CpuNumber = (UInt16)spn_CpuNo.Value;	//cpuno;
                //////ComDevice.NetworkNumber = 0;
                //////ComDevice.StationNumber = 0;
                //////ComDevice.UnitNumber = 0;
                //////ComDevice.IPAddress = "";
                //////ComDevice.Timeout = 10000;

                //////rc = CMotionAPI.ymcOpenController(ref ComDevice, ref g_hController);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcOpenController Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}

                //============================================================================ To Contents of Processing
                // Sets the motion API timeout.			
                // Prevents the host application from freezing due to Machine Controller fault.
                // Must be called for each thread.
                //============================================================================
                //////rc = CMotionAPI.ymcSetAPITimeoutValue(30000);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error SetAPITimeoutValue \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}
                //============================================================================ To Contents of Processing
                // Deletes the axis handle that is held by the Machine Controller.
                //============================================================================
                //////rc = CMotionAPI.ymcClearAllAxes();
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ClearAllAxes \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}
                //============================================================================ To Contents of Processing
                // Creates an axis handle.
                // The obtained axis handle can be used as global data by processing.
                // The following calls the ymcDeclareAxis as many times as the connected axes set in the window and creates an axis handle.
                //============================================================================
                //////for (i = 0; i < (Int16)spn_Axis.Value; i++)
                //////{
                //////    AxisName = "Axis-" + (i + 1);
                //////    rc = CMotionAPI.ymcDeclareAxis(1, 0, 3, (UInt16)(i + 1), (UInt16)(i + 1), (UInt16)CMotionAPI.ApiDefs.REAL_AXIS, AxisName, ref hAxis[i]);
                //////    if (rc != CMotionAPI.MP_SUCCESS)
                //////    {
                //////        MessageBox.Show(String.Format("Error ymcDeclareAxis \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////        return;
                //////    }
                //////}
                //============================================================================ To Contents of Processing
                // Creates a device handle using the axis handle obtained by the ymcDeclareAxis.
                // The following sets the number of connected axes as one device.
                // The device handle is needed for each thread.
                // Therefore, the device handle created here cannot be used for any other thread.
                //============================================================================
                //////rc = CMotionAPI.ymcDeclareDevice((UInt16)spn_Axis.Value, hAxis, ref g_hDevice);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcDeclareDevice \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}
                //============================================================================ To Contents of Processing
                // Executes servo ON.							
                // Servo ON is executed using the created device handle.
                // All the set axes connected are servo ON.
                //============================================================================
                //////rc = CMotionAPI.ymcServoControl(g_hDevice, (UInt16)CMotionAPI.ApiDefs.SERVO_ON, 5000);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcServoControl ON \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}
                //============================================================================
                // Sets the positioning parameter.
                // Speed, target position, and acceleration/deceleration set in the window are copied to the buffer.
                //============================================================================
                i = 0;
                VelData[i] = vel;
                AccData[i] = Int32.Parse("1000");
                DecData[i] = Int32.Parse("1000");
                PosData[i] = (int)targetPos;



                //============================================================================ To Contents of Processing
                // Loops as many times as the number of the connected axes in the window and sets as many parameters as the number of axes.
                // Performs positioning (ymcMoveDriverPositioning) after setting the data.
                //============================================================================
                for (i = 0; i < (short)1; i++)
                {
                    // Motion data setting
                    MotionData[i].CoordinateSystem = (Int16)CMotionAPI.ApiDefs.WORK_SYSTEM;
                    MotionData[i].MoveType = (Int16)CMotionAPI.ApiDefs.MTYPE_ABSOLUTE;
                    MotionData[i].VelocityType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;
                    MotionData[i].AccDecType = (Int16)CMotionAPI.ApiDefs.ATYPE_TIME;
                    MotionData[i].FilterType = (Int16)CMotionAPI.ApiDefs.FTYPE_S_CURVE;
                    MotionData[i].DataType = 0;
                    /* Not Use MotionData[i].MaxVelocity      = NULL; */
                    MotionData[i].Acceleration = AccData[i];
                    MotionData[i].Deceleration = DecData[i];
                    MotionData[i].FilterTime = 10;
                    MotionData[i].Velocity = VelData[i];
                    /* Not Use MotionData[i].ApproachVelocity = NULL; */
                    /* Not Use MotionData[i].CreepVelocity    = NULL; */

                    // Position data setting
                    Pos[i].DataType = (UInt16)CMotionAPI.ApiDefs.DATATYPE_IMMEDIATE;
                    Pos[i].PositionData = PosData[i];

                    // By setting the completion attribute to "COMMAND_STARTED (starting the command)," 
                    // the control returns to the application immediately after positioning command execution.
                    WaitForCompletion[i] = (UInt16)CMotionAPI.ApiDefs.POSITIONING_COMPLETED;
                }
                rc = CMotionAPI.ymcMoveDriverPositioning(g_hDevice[axisIndex], MotionData, Pos, 0, "Start", WaitForCompletion, 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcMoveDriverPositioning \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return;
                }

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
        /// <param name="targetPos">目标位置</param>
        /// <param name="vel">移动速度</param>
        /// <param name="waitDone">是否等待</param>
        /// <returns></returns>
        internal static void MoveRel(object axisName, double position, int vel, bool waitDone)
        {
            try
            {

                //Init(false );
                position = position * 1000;
                vel = vel * 1000;

                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
                // Definition of Motion API Variables
                CMotionAPI.COM_DEVICE ComDevice;                                  // The ymcOpenController setting structure
                UInt32[] hAxis = new UInt32[3];                      // Axis handle (for 3 axes)
                CMotionAPI.MOTION_DATA[] MotionData = new CMotionAPI.MOTION_DATA[3]; // MOTION_DATA structure (for 3 axes)
                CMotionAPI.POSITION_DATA[] Pos = new CMotionAPI.POSITION_DATA[3];      // POSITION_DATA structure (for 3 axes)
                UInt16[] WaitForCompletion = new UInt16[3];          // Completion attribute storage variable (for 3 axes)
                UInt32 rc;                                         // Motion API return value
                Int16 i;                                          // Index of number of axes
                String AxisName;                                   // Axis name
                Int32[] VelData = new Int32[3];                     // Speed storage variable (for 3 axes)
                Int32[] PosData = new Int32[3];                     // Target position storage variable (for 3 axes)
                Int32[] AccData = new Int32[3];                     // Acceleration storage variable (for 3 axes)
                Int32[] DecData = new Int32[3];                     // Deceleration storage variable (for 3 axes)

                //============================================================================ To Contents of Processing
                // Establishes the communications with the Machine Controller.
                // Must be called for each thread.
                //============================================================================
                //////ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCI_MODE;
                //////ComDevice.PortNumber = 1;
                //////ComDevice.CpuNumber = (UInt16)spn_CpuNo.Value;	//cpuno;
                //////ComDevice.NetworkNumber = 0;
                //////ComDevice.StationNumber = 0;
                //////ComDevice.UnitNumber = 0;
                //////ComDevice.IPAddress = "";
                //////ComDevice.Timeout = 10000;

                //////rc = CMotionAPI.ymcOpenController(ref ComDevice, ref g_hController);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcOpenController Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}

                //============================================================================ To Contents of Processing
                // Sets the motion API timeout.			
                // Prevents the host application from freezing due to Machine Controller fault.
                // Must be called for each thread.
                //============================================================================
                //////rc = CMotionAPI.ymcSetAPITimeoutValue(30000);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error SetAPITimeoutValue \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}
                //============================================================================ To Contents of Processing
                // Deletes the axis handle that is held by the Machine Controller.
                //============================================================================
                //////rc = CMotionAPI.ymcClearAllAxes();
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ClearAllAxes \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}
                //============================================================================ To Contents of Processing
                // Creates an axis handle.
                // The obtained axis handle can be used as global data by processing.
                // The following calls the ymcDeclareAxis as many times as the connected axes set in the window and creates an axis handle.
                //============================================================================
                //////for (i = 0; i < (Int16)spn_Axis.Value; i++)
                //////{
                //////    AxisName = "Axis-" + (i + 1);
                //////    rc = CMotionAPI.ymcDeclareAxis(1, 0, 3, (UInt16)(i + 1), (UInt16)(i + 1), (UInt16)CMotionAPI.ApiDefs.REAL_AXIS, AxisName, ref hAxis[i]);
                //////    if (rc != CMotionAPI.MP_SUCCESS)
                //////    {
                //////        MessageBox.Show(String.Format("Error ymcDeclareAxis \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////        return;
                //////    }
                //////}
                //============================================================================ To Contents of Processing
                // Creates a device handle using the axis handle obtained by the ymcDeclareAxis.
                // The following sets the number of connected axes as one device.
                // The device handle is needed for each thread.
                // Therefore, the device handle created here cannot be used for any other thread.
                //============================================================================
                //////rc = CMotionAPI.ymcDeclareDevice((UInt16)spn_Axis.Value, hAxis, ref g_hDevice);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcDeclareDevice \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}
                //============================================================================ To Contents of Processing
                // Executes servo ON.							
                // Servo ON is executed using the created device handle.
                // All the set axes connected are servo ON.
                //============================================================================
                //////rc = CMotionAPI.ymcServoControl(g_hDevice, (UInt16)CMotionAPI.ApiDefs.SERVO_ON, 5000);
                //////if (rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcServoControl ON \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //////    return;
                //////}
                //============================================================================
                // Sets the positioning parameter.
                // Speed, target position, and acceleration/deceleration set in the window are copied to the buffer.
                //============================================================================
                i = 0;
                VelData[i] = vel;
                AccData[i] = Int32.Parse("1000");
                DecData[i] = Int32.Parse("1000");
                PosData[i] = (int)position;



                //============================================================================ To Contents of Processing
                // Loops as many times as the number of the connected axes in the window and sets as many parameters as the number of axes.
                // Performs positioning (ymcMoveDriverPositioning) after setting the data.
                //============================================================================
                for (i = 0; i < (short)1; i++)
                {
                    // Motion data setting
                    MotionData[i].CoordinateSystem = (Int16)CMotionAPI.ApiDefs.WORK_SYSTEM;
                    MotionData[i].MoveType = (Int16)CMotionAPI.ApiDefs.MTYPE_RELATIVE;
                    MotionData[i].VelocityType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;
                    MotionData[i].AccDecType = (Int16)CMotionAPI.ApiDefs.ATYPE_TIME;
                    MotionData[i].FilterType = (Int16)CMotionAPI.ApiDefs.FTYPE_S_CURVE;
                    MotionData[i].DataType = 0;
                    /* Not Use MotionData[i].MaxVelocity      = NULL; */
                    MotionData[i].Acceleration = AccData[i];
                    MotionData[i].Deceleration = DecData[i];
                    MotionData[i].FilterTime = 10;
                    MotionData[i].Velocity = VelData[i];
                    /* Not Use MotionData[i].ApproachVelocity = NULL; */
                    /* Not Use MotionData[i].CreepVelocity    = NULL; */

                    // Position data setting
                    Pos[i].DataType = (UInt16)CMotionAPI.ApiDefs.DATATYPE_IMMEDIATE;
                    Pos[i].PositionData = PosData[i];

                    // By setting the completion attribute to "COMMAND_STARTED (starting the command)," 
                    // the control returns to the application immediately after positioning command execution.
                    WaitForCompletion[i] = (UInt16)CMotionAPI.ApiDefs.COMMAND_STARTED;
                }
                rc = CMotionAPI.ymcMoveDriverPositioning(g_hDevice[axisIndex], MotionData, Pos, 0, "Start", WaitForCompletion, 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcMoveDriverPositioning \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return;
                }
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
        internal static bool JogStart(object axisName, HomeDir homeDir, int vel, bool waitDone = true)
        {
            try
            {
                vel = vel * 1000;


                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
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



                //============================================================================
                // Sets the JOG parameter.								
                //============================================================================
                // Definition of Motion API Variables
                CMotionAPI.COM_DEVICE ComDevice;                                  // The ymcOpenController setting structure 
                CMotionAPI.MOTION_DATA[] MotionData = new CMotionAPI.MOTION_DATA[5]; // MOTION_DATA structure (for 3 axes)
                Int16[] Direction = new Int16[5];                   // JOG direction specified (for 3 axes)
                UInt16[] Timeout = new UInt16[5];                    // Timeout time (for 3 axes)
                UInt32 rc;                                         // Motion API return value
                Int16 i;                                          // Index of number of axes
                String AxisName;                                   // Axis name
                Int32[] VelData = new Int32[5];                     // Speed storage variable (for 3 axes)
                Int32[] PosData = new Int32[5];                     // Target position storage variable (for 3 axes)
                Int32[] AccData = new Int32[5];                     // Acceleration storage variable (for 3 axes)
                Int32[] DecData = new Int32[5];                     // Deceleration storage variable (for 3 axes)



                int j = 0;
                i = 0;
                VelData[i] = vel;
                AccData[i] = Int32.Parse("1000");
                DecData[i] = Int32.Parse("1000");
                PosData[i] = Int32.Parse("10000");
                i = 1;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");
                i = 2;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");
                i = 3;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");
                i = 4;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");

                //============================================================================ To Contents of Processing
                // Sets the target board to 1.						
                //============================================================================
                //rc = CMotionAPI.ymcSetController(g_hController_1);
                //if (rc != CMotionAPI.MP_SUCCESS)
                //{
                //    MessageBox.Show(String.Format("Error ymcSetController Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //    return;
                //}
                //============================================================================
                // Executes JOG operation.										
                //============================================================================
                for (j = 0; j < (Int16)5; j++)
                {
                    // Motion data setting
                    MotionData[j].CoordinateSystem = (Int16)CMotionAPI.ApiDefs.WORK_SYSTEM;	// Work coordinate system
                    MotionData[j].MoveType = (Int16)CMotionAPI.ApiDefs.MTYPE_RELATIVE;	// Incremental value specified
                    MotionData[j].VelocityType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;	// Speed [reference unit/s]
                    MotionData[j].AccDecType = (Int16)CMotionAPI.ApiDefs.ATYPE_TIME;		// Time constant specified [ms]
                    MotionData[j].FilterType = (Int16)CMotionAPI.ApiDefs.FTYPE_S_CURVE;	// Moving average filter (simplified S-curve)
                    MotionData[j].DataType = 0;										// All parameters directly specified
                    /* Not Use MotionData[i].MaxVelocity      = NULL; */
                    MotionData[j].Acceleration = AccData[j];								// Acceleration time constant [ms] 
                    MotionData[j].Deceleration = DecData[j];								// Deceleration time constant [ms]
                    MotionData[j].FilterTime = 10;                                       // Filter time [0.1 ms]
                    MotionData[j].Velocity = VelData[j];					            // Speed [reference unit/s]					
                    /* Not Use MotionData[i].ApproachVelocity = NULL; */
                    /* Not Use MotionData[i].CreepVelocity    = NULL; */
                    Direction[j] = (homeDir == HomeDir.N_负方向 ? (Int16)CMotionAPI.ApiDefs.DIRECTION_NEGATIVE : (Int16)CMotionAPI.ApiDefs.DIRECTION_POSITIVE);
                    Timeout[j] = 0;
                }
                rc = CMotionAPI.ymcMoveJOG(g_hDevice[axisIndex], MotionData, Direction, Timeout, 0, "Start", 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcMoveJOG Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }



                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        internal static bool JogStop(object axisName)
        {
            try
            {

                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;



                UInt16[] WaitForCompletion = new UInt16[3];  // Completion attribute storage variable (for 3 axes)
                UInt32 rc;                                 // Motion API return value
                Int16 i;                                  // Index of number of axes

                //============================================================================ To Contents of Processing
                // Sets the target board to 1.
                //============================================================================
                //////rc = CMotionAPI.ymcSetController(g_hController_1);
                //////if(rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcSetController Board 1 \nErrorCode [ 0x{0} ]",rc.ToString("X")));
                //////    return;
                //////}

                //============================================================================
                // Stops the axis motion.
                //============================================================================
                //////for (i = 0; i < (Int16)spn_Axis_1.Value; i++)
                //////{
                //////    WaitForCompletion[i] = (UInt16)CMotionAPI.ApiDefs.POSITIONING_COMPLETED;
                //////}
                rc = CMotionAPI.ymcStopJOG(g_hDevice[axisIndex], 0, "Stop", WaitForCompletion, 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcStopJOG Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                }


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
        /// <param name="distance">移动距离</param>
        /// <param name="vel">移动速度</param>
        /// <param name="waitDone">是否等待</param>
        /// <returns></returns>
        internal static bool KeepMoveStart(object axisName, HomeDir homeDir, int vel)
        {
            try
            {
                vel = vel * 1000;


                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
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



                //============================================================================
                // Sets the JOG parameter.								
                //============================================================================
                // Definition of Motion API Variables
                CMotionAPI.COM_DEVICE ComDevice;                                  // The ymcOpenController setting structure 
                CMotionAPI.MOTION_DATA[] MotionData = new CMotionAPI.MOTION_DATA[5]; // MOTION_DATA structure (for 3 axes)
                Int16[] Direction = new Int16[5];                   // JOG direction specified (for 3 axes)
                UInt16[] Timeout = new UInt16[5];                    // Timeout time (for 3 axes)
                UInt32 rc;                                         // Motion API return value
                Int16 i;                                          // Index of number of axes
                String AxisName;                                   // Axis name
                Int32[] VelData = new Int32[5];                     // Speed storage variable (for 3 axes)
                Int32[] PosData = new Int32[5];                     // Target position storage variable (for 3 axes)
                Int32[] AccData = new Int32[5];                     // Acceleration storage variable (for 3 axes)
                Int32[] DecData = new Int32[5];                     // Deceleration storage variable (for 3 axes)



                int j = 0;
                i = 0;
                VelData[i] = vel;
                AccData[i] = Int32.Parse("1000");
                DecData[i] = Int32.Parse("1000");
                PosData[i] = Int32.Parse("10000");
                i = 1;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");
                i = 2;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");
                i = 3;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");
                i = 4;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");

                //============================================================================ To Contents of Processing
                // Sets the target board to 1.						
                //============================================================================
                //rc = CMotionAPI.ymcSetController(g_hController_1);
                //if (rc != CMotionAPI.MP_SUCCESS)
                //{
                //    MessageBox.Show(String.Format("Error ymcSetController Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //    return;
                //}
                //============================================================================
                // Executes JOG operation.										
                //============================================================================
                for (j = 0; j < (Int16)5; j++)
                {
                    // Motion data setting
                    MotionData[j].CoordinateSystem = (Int16)CMotionAPI.ApiDefs.WORK_SYSTEM;	// Work coordinate system
                    MotionData[j].MoveType = (Int16)CMotionAPI.ApiDefs.MTYPE_RELATIVE;	// Incremental value specified
                    MotionData[j].VelocityType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;	// Speed [reference unit/s]
                    MotionData[j].AccDecType = (Int16)CMotionAPI.ApiDefs.ATYPE_TIME;		// Time constant specified [ms]
                    MotionData[j].FilterType = (Int16)CMotionAPI.ApiDefs.FTYPE_S_CURVE;	// Moving average filter (simplified S-curve)
                    MotionData[j].DataType = 0;										// All parameters directly specified
                    /* Not Use MotionData[i].MaxVelocity      = NULL; */
                    MotionData[j].Acceleration = AccData[j];								// Acceleration time constant [ms] 
                    MotionData[j].Deceleration = DecData[j];								// Deceleration time constant [ms]
                    MotionData[j].FilterTime = 10;                                       // Filter time [0.1 ms]
                    MotionData[j].Velocity = VelData[j];					            // Speed [reference unit/s]					
                    /* Not Use MotionData[i].ApproachVelocity = NULL; */
                    /* Not Use MotionData[i].CreepVelocity    = NULL; */
                    Direction[j] = (homeDir == HomeDir.N_负方向 ? (Int16)CMotionAPI.ApiDefs.DIRECTION_NEGATIVE : (Int16)CMotionAPI.ApiDefs.DIRECTION_POSITIVE);
                    Timeout[j] = 0;
                }
                rc = CMotionAPI.ymcMoveJOG(g_hDevice[axisIndex], MotionData, Direction, Timeout, 0, "Start", 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcMoveJOG Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }



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
        /// <param name="distance">移动距离</param>
        /// <param name="vel">移动速度</param>
        /// <param name="waitDone">是否等待</param>
        /// <returns></returns>
        internal static bool KeepMoveStartPCB(object axisName, HomeDir homeDir, int vel)
        {
            try
            {
                vel = vel * 1000;


                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;
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



                //============================================================================
                // Sets the JOG parameter.								
                //============================================================================
                // Definition of Motion API Variables
                CMotionAPI.COM_DEVICE ComDevice;                                  // The ymcOpenController setting structure 
                CMotionAPI.MOTION_DATA[] MotionData = new CMotionAPI.MOTION_DATA[5]; // MOTION_DATA structure (for 3 axes)
                Int16[] Direction = new Int16[5];                   // JOG direction specified (for 3 axes)
                UInt16[] Timeout = new UInt16[5];                    // Timeout time (for 3 axes)
                UInt32 rc;                                         // Motion API return value
                Int16 i;                                          // Index of number of axes
                String AxisName;                                   // Axis name
                Int32[] VelData = new Int32[5];                     // Speed storage variable (for 3 axes)
                Int32[] PosData = new Int32[5];                     // Target position storage variable (for 3 axes)
                Int32[] AccData = new Int32[5];                     // Acceleration storage variable (for 3 axes)
                Int32[] DecData = new Int32[5];                     // Deceleration storage variable (for 3 axes)



                int j = 0;
                i = 0;
                VelData[i] = vel;
                AccData[i] = Int32.Parse("1000");
                DecData[i] = Int32.Parse("1000");
                PosData[i] = Int32.Parse("10000");
                i = 1;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");
                i = 2;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");
                i = 3;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");
                i = 4;
                VelData[i] = Int32.Parse("1000");
                AccData[i] = Int32.Parse("100");
                DecData[i] = Int32.Parse("100");
                PosData[i] = Int32.Parse("1000");

                //============================================================================ To Contents of Processing
                // Sets the target board to 1.						
                //============================================================================
                //rc = CMotionAPI.ymcSetController(g_hController_1);
                //if (rc != CMotionAPI.MP_SUCCESS)
                //{
                //    MessageBox.Show(String.Format("Error ymcSetController Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                //    return;
                //}
                //============================================================================
                // Executes JOG operation.										
                //============================================================================
                for (j = 0; j < (Int16)5; j++)
                {
                    // Motion data setting
                    MotionData[j].CoordinateSystem = (Int16)CMotionAPI.ApiDefs.WORK_SYSTEM;	// Work coordinate system
                    MotionData[j].MoveType = (Int16)CMotionAPI.ApiDefs.MTYPE_RELATIVE;	// Incremental value specified
                    MotionData[j].VelocityType = (Int16)CMotionAPI.ApiDefs.VTYPE_UNIT_PAR;	// Speed [reference unit/s]
                    MotionData[j].AccDecType = (Int16)CMotionAPI.ApiDefs.ATYPE_TIME;		// Time constant specified [ms]
                    MotionData[j].FilterType = (Int16)CMotionAPI.ApiDefs.FTYPE_S_CURVE;	// Moving average filter (simplified S-curve)
                    MotionData[j].DataType = 0;										// All parameters directly specified
                    /* Not Use MotionData[i].MaxVelocity      = NULL; */
                    MotionData[j].Acceleration = AccData[j];								// Acceleration time constant [ms] 
                    MotionData[j].Deceleration = DecData[j];								// Deceleration time constant [ms]
                    MotionData[j].FilterTime = 10;                                       // Filter time [0.1 ms]
                    MotionData[j].Velocity = VelData[j];					            // Speed [reference unit/s]					
                    /* Not Use MotionData[i].ApproachVelocity = NULL; */
                    /* Not Use MotionData[i].CreepVelocity    = NULL; */
                    Direction[j] = (homeDir == HomeDir.N_负方向 ? (Int16)CMotionAPI.ApiDefs.DIRECTION_NEGATIVE : (Int16)CMotionAPI.ApiDefs.DIRECTION_POSITIVE);
                    Timeout[j] = 0;
                }
                rc = CMotionAPI.ymcMoveJOG(deviceLoadPCB[axisIndex], MotionData, Direction, Timeout, 0, "Start", 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcMoveJOG Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                    return false;
                }



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
        /// <param name="distance">移动距离</param>
        /// <param name="vel">移动速度</param>
        /// <param name="waitDone">是否等待</param>
        /// <returns></returns>
        internal static bool KeepMoveStop(object axisName)
        {
            try
            {
                // vel = vel * 1000;


                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;



                UInt16[] WaitForCompletion = new UInt16[3];  // Completion attribute storage variable (for 3 axes)
                UInt32 rc;                                 // Motion API return value
                Int16 i;                                  // Index of number of axes

                //============================================================================ To Contents of Processing
                // Sets the target board to 1.
                //============================================================================
                //////rc = CMotionAPI.ymcSetController(g_hController_1);
                //////if(rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcSetController Board 1 \nErrorCode [ 0x{0} ]",rc.ToString("X")));
                //////    return;
                //////}

                //============================================================================
                // Stops the axis motion.
                //============================================================================
                //////for (i = 0; i < (Int16)spn_Axis_1.Value; i++)
                //////{
                //////    WaitForCompletion[i] = (UInt16)CMotionAPI.ApiDefs.POSITIONING_COMPLETED;
                //////}
                rc = CMotionAPI.ymcStopJOG(g_hDevice[axisIndex], 0, "Stop", WaitForCompletion, 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcStopJOG Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                }


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
        /// <param name="distance">移动距离</param>
        /// <param name="vel">移动速度</param>
        /// <param name="waitDone">是否等待</param>
        /// <returns></returns>
        internal static bool KeepMoveStopPCB(object axisName)
        {
            try
            {
                // vel = vel * 1000;


                ushort axisIndex = (ushort)FindAxisByName(axisName.ToString()).actNo;



                UInt16[] WaitForCompletion = new UInt16[3];  // Completion attribute storage variable (for 3 axes)
                UInt32 rc;                                 // Motion API return value
                Int16 i;                                  // Index of number of axes

                //============================================================================ To Contents of Processing
                // Sets the target board to 1.
                //============================================================================
                //////rc = CMotionAPI.ymcSetController(g_hController_1);
                //////if(rc != CMotionAPI.MP_SUCCESS)
                //////{
                //////    MessageBox.Show(String.Format("Error ymcSetController Board 1 \nErrorCode [ 0x{0} ]",rc.ToString("X")));
                //////    return;
                //////}

                //============================================================================
                // Stops the axis motion.
                //============================================================================
                //////for (i = 0; i < (Int16)spn_Axis_1.Value; i++)
                //////{
                //////    WaitForCompletion[i] = (UInt16)CMotionAPI.ApiDefs.POSITIONING_COMPLETED;
                //////}
                rc = CMotionAPI.ymcStopJOG(deviceLoadPCB[axisIndex], 0, "Stop", WaitForCompletion, 0);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    MessageBox.Show(String.Format("Error ymcStopJOG Board 1 \nErrorCode [ 0x{0} ]", rc.ToString("X")));
                }


                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
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
                                //////Card_LeadShineDMC2210.MoveAbs(axisName, (int)(targetPos * Axis_Config.Instance.MMPixelRoute[axisIndex]), (int)(Project.Instance.configuration.autoRunVel * (Project.Instance.configuration.autoRunVelRoute / 100) / Axis_Config.Instance.MMPixelRoute[axisIndex]), true);
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
                //////Thread.Sleep(100);
                //////int statu;
                //////do
                //////{
                //////    statu = Dmc2210.d2210_check_done((ushort)axisIndex);
                //////    Thread.Sleep(10);
                //////} while (statu == 0);
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

                //////double curPos;
                //////ushort axisIndex = (ushort)GetAxisIndexByName(axisName.ToString());
                //////curPos = Dmc2210.d2210_get_position(axisIndex);
                //////return curPos;
                return 0;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return 0;
            }
        }
        /// <summary>
        /// 获取各轴编码器反馈值
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static double GetEncPos(Axis axis)
        {
            try
            {
                double value = 0;
                switch (axis)
                {
                    case Axis.X:
                        value = ReadData(MemArea.IL, 8016);
                        break;
                    case Axis.Y:
                        double value1 = 0;
                        value = ReadData(MemArea.IL, 8096);
                        value1 = ReadData(MemArea.IL, 8116);
                        double offset = Math.Abs(Convert.ToInt64(value) - Convert.ToInt64(value1));
                        if (offset > 100)
                        { }
                        //////MessageBox.Show(string.Format("龙门轴左右编码器反馈值相差{0}mm，可能存在轴憋住现象，请检查", offset));
                        break;
                    case Axis.YL:
                        value = ReadData(MemArea.IL, 8096);
                        break;
                    case Axis.YR:
                        value = ReadData(MemArea.IL, 8116);
                        break;
                    case Axis.Z:
                        value = ReadData(MemArea.IL, 8196);
                        break;
                    case Axis.R:
                        value = ReadData(MemArea.IL, 8216);
                        break;
                    case Axis.TR:
                        value = ReadData(MemArea.IL, 8516);
                        break;
                }
                return Convert.ToDouble(value) / 1000;
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
                //////ushort axisIndex = (ushort)GetAxisIndexByName(axisName.ToString());
                //////uint temp = Dmc2210.d2210_get_encoder(axisIndex);
                //////return temp;
                return 0;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return 0;
            }
        }
        /// <summary>
        /// 释放板卡
        /// </summary>
        internal static void CloseBoard()
        {
            try
            {
                UInt32 rc;
                rc = CMotionAPI.ymcCloseController(g_hController);
                if (rc != CMotionAPI.MP_SUCCESS)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("释放板卡时出错，错误编号：" + rc.ToString("X"));
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
    public enum MemArea
    {
        IL,
        ML,
        MB,
        OW,
        OB,
        GB,
        MF,
        IB,
    }
}
