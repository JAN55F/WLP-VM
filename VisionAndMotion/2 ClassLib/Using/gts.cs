using System.Runtime.InteropServices;

namespace gts
{
    internal class mc
    {
        internal const short DLL_VERSION_0 = 2;
        internal const short DLL_VERSION_1 = 1;
        internal const short DLL_VERSION_2 = 0;

        internal const short DLL_VERSION_3 = 1;
        internal const short DLL_VERSION_4 = 5;
        internal const short DLL_VERSION_5 = 0;
        internal const short DLL_VERSION_6 = 6;
        internal const short DLL_VERSION_7 = 0;
        internal const short DLL_VERSION_8 = 7;

        internal const short MC_NONE = -1;

        internal const short MC_LIMIT_POSITIVE = 0;
        internal const short MC_LIMIT_NEGATIVE = 1;
        internal const short MC_ALARM = 2;
        internal const short MC_HOME = 3;
        internal const short MC_GPI = 4;
        internal const short MC_ARRIVE = 5;
        internal const short MC_MPG = 6;

        internal const short MC_ENABLE = 10;
        internal const short MC_CLEAR = 11;
        internal const short MC_GPO = 12;

        internal const short MC_DAC = 20;
        internal const short MC_STEP = 21;
        internal const short MC_PULSE = 22;
        internal const short MC_ENCODER = 23;
        internal const short MC_ADC = 24;

        internal const short MC_AXIS = 30;
        internal const short MC_PROFILE = 31;
        internal const short MC_CONTROL = 32;

        internal const short CAPTURE_HOME = 1;
        internal const short CAPTURE_INDEX = 2;
        internal const short CAPTURE_PROBE = 3;
        internal const short CAPTURE_HSIO0 = 6;
        internal const short CAPTURE_HSIO1 = 7;
        internal const short CAPTURE_HOME_GPI = 8;

        internal const short PT_MODE_STATIC = 0;
        internal const short PT_MODE_DYNAMIC = 1;

        internal const short PT_SEGMENT_NORMAL = 0;
        internal const short PT_SEGMENT_EVEN = 1;
        internal const short PT_SEGMENT_STOP = 2;

        internal const short GEAR_MASTER_ENCODER = 1;
        internal const short GEAR_MASTER_PROFILE = 2;
        internal const short GEAR_MASTER_AXIS = 3;

        internal const short FOLLOW_MASTER_ENCODER = 1;
        internal const short FOLLOW_MASTER_PROFILE = 2;
        internal const short FOLLOW_MASTER_AXIS = 3;

        internal const short FOLLOW_EVENT_START = 1;
        internal const short FOLLOW_EVENT_PASS = 2;

        internal const short GEAR_EVENT_START = 1;
        internal const short GEAR_EVENT_PASS = 2;
        internal const short GEAR_EVENT_AREA = 5;

        internal const short FOLLOW_SEGMENT_NORMAL = 0;
        internal const short FOLLOW_SEGMENT_EVEN = 1;
        internal const short FOLLOW_SEGMENT_STOP = 2;
        internal const short FOLLOW_SEGMENT_CONTINUE = 3;

        internal const short INTERPOLATION_AXIS_MAX = 4;
        internal const short CRD_FIFO_MAX = 4096;
        internal const short FIFO_MAX = 2;
        internal const short CRD_MAX = 2;
        internal const short CRD_OPERATION_DATA_EXT_MAX = 2;

        internal const short CRD_OPERATION_TYPE_NONE = 0;
        internal const short CRD_OPERATION_TYPE_BUF_IO_DELAY = 1;
        internal const short CRD_OPERATION_TYPE_LASER_ON = 2;
        internal const short CRD_OPERATION_TYPE_LASER_OFF = 3;
        internal const short CRD_OPERATION_TYPE_BUF_DA = 4;
        internal const short CRD_OPERATION_TYPE_LASER_CMD = 5;
        internal const short CRD_OPERATION_TYPE_LASER_FOLLOW = 6;
        internal const short CRD_OPERATION_TYPE_LMTS_ON = 7;
        internal const short CRD_OPERATION_TYPE_LMTS_OFF = 8;
        internal const short CRD_OPERATION_TYPE_SET_STOP_IO = 9;
        internal const short CRD_OPERATION_TYPE_BUF_MOVE = 10;
        internal const short CRD_OPERATION_TYPE_BUF_GEAR = 11;
        internal const short CRD_OPERATION_TYPE_SET_SEG_NUM = 12;
        internal const short CRD_OPERATION_TYPE_STOP_MOTION = 13;
        internal const short CRD_OPERATION_TYPE_SET_VAR_VALUE = 14;
        internal const short CRD_OPERATION_TYPE_JUMP_NEXT_SEG = 15;
        internal const short CRD_OPERATION_TYPE_SYNCH_PRF_POS = 16;
        internal const short CRD_OPERATION_TYPE_VIRTUAL_TO_ACTUAL = 17;
        internal const short CRD_OPERATION_TYPE_SET_USER_VAR = 18;
        internal const short CRD_OPERATION_TYPE_SET_DO_BIT_PULSE = 19;
        internal const short CRD_OPERATION_TYPE_BUF_COMPAREPULSE = 20;
        internal const short CRD_OPERATION_TYPE_LASER_ON_EX = 21;
        internal const short CRD_OPERATION_TYPE_LASER_OFF_EX = 22;
        internal const short CRD_OPERATION_TYPE_LASER_CMD_EX = 23;
        internal const short CRD_OPERATION_TYPE_LASER_FOLLOW_RATIO_EX = 24;
        internal const short CRD_OPERATION_TYPE_LASER_FOLLOW_MODE = 25;
        internal const short CRD_OPERATION_TYPE_LASER_FOLLOW_OFF = 26;
        internal const short CRD_OPERATION_TYPE_LASER_FOLLOW_OFF_EX = 27;
        internal const short CRD_OPERATION_TYPE_LASER_FOLLOW_SPLINE = 28;
        internal const short CRD_OPERATION_TYPE_MOTION_DATA = 29;

        internal const short INTERPOLATION_MOTION_TYPE_LINE = 0;
        internal const short INTERPOLATION_MOTION_TYPE_CIRCLE = 1;
        internal const short INTERPOLATION_MOTION_TYPE_HELIX = 2;
        internal const short INTERPOLATION_MOTION_TYPE_CIRCLE_3D = 3;

        internal const short INTERPOLATION_CIRCLE_PLAT_XY = 0;
        internal const short INTERPOLATION_CIRCLE_PLAT_YZ = 1;
        internal const short INTERPOLATION_CIRCLE_PLAT_ZX = 2;

        internal const short INTERPOLATION_HELIX_CIRCLE_XY_LINE_Z = 0;
        internal const short INTERPOLATION_HELIX_CIRCLE_YZ_LINE_X = 1;
        internal const short INTERPOLATION_HELIX_CIRCLE_ZX_LINE_Y = 2;

        internal const short INTERPOLATION_CIRCLE_DIR_CW = 0;
        internal const short INTERPOLATION_CIRCLE_DIR_CCW = 1;

        internal const short COMPARE_PORT_HSIO = 0;
        internal const short COMPARE_PORT_GPO = 1;

        internal const short COMPARE2D_MODE_2D = 1;
        internal const short COMPARE2D_MODE_1D = 0;

        internal const short INTERFACEBOARD20 = 2;
        internal const short INTERFACEBOARD30 = 3;

        internal const short AXIS_LASER = 7;
        internal const short AXIS_LASER_EX = 8;

        internal const short LASER_CTRL_MODE_PWM1 = 0;
        internal const short LASER_CTRL_FREQUENCY = 1;
        internal const short LASER_CTRL_VOLTAGE = 2;
        internal const short LASER_CTRL_MODE_PWM2 = 3;

        internal struct TTrapPrm
        {
            internal double acc;
            internal double dec;
            internal double velStart;
            internal short  smoothTime;
        }

        internal struct TJogPrm
        {
            internal double acc;
            internal double dec;
            internal double smooth;
        }

        internal struct TPid
        {
            internal double kp;
            internal double ki;
            internal double kd;
            internal double kvff;
            internal double kaff;

            internal int integralLimit;
            internal int derivativeLimit;
            internal short  limit;
        }

        internal struct TThreadSts
        {
            internal short run;
            internal short error;
            internal double result;
            internal short line;
        }

        internal struct TVarInfo
        {
            internal short id;
            internal short dataType;
            internal double dumb0;
            internal double dumb1;
            internal double dumb2;
            internal double dumb3;
        }
        internal struct TCompileInfo
        {
            internal string pFileName;
            internal short pLineNo1;
            internal short pLineNo2;
            internal string pMessage;
        }
        internal struct TCrdPrm
        {
            internal short dimension;
            internal short profile1;
            internal short profile2;
            internal short profile3;
            internal short profile4;
            internal short profile5;
            internal short profile6;
            internal short profile7;
            internal short profile8;

            internal double synVelMax;
            internal double synAccMax;
            internal short evenTime;
            internal short setOriginFlag;
            internal int originPos1;
            internal int originPos2;
            internal int originPos3;
            internal int originPos4;
            internal int originPos5;
            internal int originPos6;
            internal int originPos7;
            internal int originPos8;
        }

        internal struct TCrdBufOperation
        {
            internal short flag;
            internal ushort delay;
            internal short doType;
            internal ushort doMask;
            internal ushort doValue;
            internal ushort dataExt1;
            internal ushort dataExt2;
        }

        internal struct TCrdData
        {
            internal short motionType;
            internal short circlePlat;
            internal int posX;
            internal int posY;
            internal int posZ;
            internal int posA;
            internal double radius;
            internal short circleDir;
            internal double lCenterX;
            internal double lCenterY;
            internal double lCenterZ;
            internal double vel;
            internal double acc;
            internal short velEndZero;
            internal TCrdBufOperation operation;

            internal double cosX;
            internal double cosY;
            internal double cosZ;
            internal double cosA;
            internal double velEnd;
            internal double velEndAdjust;
            internal double r;
        }

        internal struct TTrigger
        {
            internal short encoder;
            internal short probeType;
            internal short probeIndex;
            internal short offset;
            internal short windowOnly;
            internal int firstPosition;
            internal int lastPosition;
        }

        internal struct TTriggerStatus
        {
            internal short execute;
            internal short done;
            internal int position;
        }

        internal struct T2DCompareData
        {
            internal int px;
            internal int py;
        }

        internal struct T2DComparePrm
        {
            internal short encx;
            internal short ency;
            internal short source;
            internal short outputType;
            internal short startLevel;
            internal short time;
            internal short maxerr;
            internal short threshold; 
        }
        [DllImport("gts.dll")]
        internal static extern short GT_GetDllVersion(out string pDllVersion);
        [DllImport("gts.dll")]
        internal static extern short GT_SetCardNo(short index);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCardNo(out short index);

        [DllImport("gts.dll")]
        internal static extern short GT_GetVersion(out string pVersion);
        [DllImport("gts.dll")]
        internal static extern short GT_GetInterfaceBoardSts(out short pStatus);
        [DllImport("gts.dll")]
        internal static extern short GT_SetInterfaceBoardSts(short type);

        [DllImport("gts.dll")]
        internal static extern short GT_Open(short channel,short param);
        [DllImport("gts.dll")]
        internal static extern short GT_Close();

        [DllImport("gts.dll")]
        internal static extern short GT_LoadConfig(string pFile);

        [DllImport("gts.dll")]
        internal static extern short GT_AlarmOff(short axis);
        [DllImport("gts.dll")]
        internal static extern short GT_AlarmOn(short axis);
        [DllImport("gts.dll")]
        internal static extern short GT_LmtsOn(short axis, short limitType);
        [DllImport("gts.dll")]
        internal static extern short GT_LmtsOff(short axis, short limitType);
        [DllImport("gts.dll")]
        internal static extern short GT_ProfileScale(short axis, short alpha, short beta);
        [DllImport("gts.dll")]
        internal static extern short GT_EncScale(short axis, short alpha, short beta);
        [DllImport("gts.dll")]
        internal static extern short GT_StepDir(short step);
        [DllImport("gts.dll")]
        internal static extern short GT_StepPulse(short step);
        [DllImport("gts.dll")]
        internal static extern short GT_SetMtrBias(short dac, short bias);
        [DllImport("gts.dll")]
        internal static extern short GT_GetMtrBias(short dac, out short pBias);
        [DllImport("gts.dll")]
        internal static extern short GT_SetMtrLmt(short dac, short limit);
        [DllImport("gts.dll")]
        internal static extern short GT_GetMtrLmt(short dac, out short pLimit);
        [DllImport("gts.dll")]
        internal static extern short GT_EncSns(ushort sense);
        [DllImport("gts.dll")]
        internal static extern short GT_EncOn(short encoder);
        [DllImport("gts.dll")]
        internal static extern short GT_EncOff(short encoder);
        [DllImport("gts.dll")]
        internal static extern short GT_SetPosErr(short control, int error);
        [DllImport("gts.dll")]
        internal static extern short GT_GetPosErr(short control, out int pError);
        [DllImport("gts.dll")]
        internal static extern short GT_SetStopDec(short profile, double decSmoothStop, double decAbruptStop);
        [DllImport("gts.dll")]
        internal static extern short GT_GetStopDec(short profile, out double pDecSmoothStop, out double pDecAbruptStop);
        [DllImport("gts.dll")]
        internal static extern short GT_LmtSns(ushort sense);
        [DllImport("gts.dll")]
        internal static extern short GT_CtrlMode(short axis, short mode);
        [DllImport("gts.dll")]
        internal static extern short GT_SetStopIo(short axis, short stopType, short inputType, short inputIndex);
        [DllImport("gts.dll")]
        internal static extern short GT_GpiSns(ushort sense);
        [DllImport("gts.dll")]
        internal static extern short GT_SetAdcFilter(short adc,short filterTime);
        [DllImport("gts.dll")]
        internal static extern short GT_SetAxisPrfVelFilter(short axis,short filterNumExp);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisPrfVelFilter(short axis,out short pFilterNumExp);
        [DllImport("gts.dll")]
        internal static extern short GT_SetAxisEncVelFilter(short axis,short filterNumExp);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisEncVelFilter(short axis,out short pFilterNumExp);
        [DllImport("gts.dll")]
        internal static extern short GT_SetAxisInputShaping(short axis, short enable, short count, double k);

        [DllImport("gts.dll")]
        internal static extern short GT_SetDo(short doType,int value);
        [DllImport("gts.dll")]
        internal static extern short GT_SetDoBit(short doType,short doIndex,short value);
        [DllImport("gts.dll")]
        internal static extern short GT_GetDo(short doType,out int pValue);
        [DllImport("gts.dll")]
        internal static extern short GT_SetDoBitReverse(short doType,short doIndex,short value,short reverseTime);
        [DllImport("gts.dll")]
        internal static extern short GT_SetDoMask(short doType,ushort doMask,int value);
        [DllImport("gts.dll")]
        internal static extern short GT_EnableDoBitPulse(short doType,short doIndex,ushort highLevelTime,ushort lowLevelTime,int pulseNum,short firstLevel);
        [DllImport("gts.dll")]
        internal static extern short GT_DisableDoBitPulse(short doType, short doIndex);

        [DllImport("gts.dll")]
        internal static extern short GT_GetDi(short diType,out int pValue);
        [DllImport("gts.dll")]
        internal static extern short GT_GetDiReverseCount(short diType,short diIndex,out uint reverseCount,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_SetDiReverseCount(short diType,short diIndex,ref uint reverseCount,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_GetDiRaw(short diType,out int pValue);

        [DllImport("gts.dll")]
        internal static extern short GT_SetDac(short dac,ref short value,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_GetDac(short dac,out short value,short count,out uint pClock);

        [DllImport("gts.dll")]
        internal static extern short GT_GetAdc(short adc,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAdcValue(short adc,out short pValue,short count,out uint pClock);

        [DllImport("gts.dll")]
        internal static extern short GT_SetEncPos(short encoder,int encPos);
        [DllImport("gts.dll")]
        internal static extern short GT_GetEncPos(short encoder,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetEncPosPre(short encoder,out double pValue,short count,uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetEncVel(short encoder,out double pValue,short count,out uint pClock);

        [DllImport("gts.dll")]
        internal static extern short GT_SetCaptureMode(short encoder,short mode);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCaptureMode(short encoder,out short pMode,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCaptureStatus(short encoder,out short pStatus,out int pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_SetCaptureSense(short encoder, short mode,short sense);
        [DllImport("gts.dll")]
        internal static extern short GT_ClearCaptureStatus(short encoder);
        [DllImport("gts.dll")]
        internal static extern short GT_SetCaptureRepeat(short encoder,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCaptureRepeatStatus(short encoder,out short pCount);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCaptureRepeatPos(short encoder, out int pValue, short startNum, short count);
        [DllImport("gts.dll")]
        internal static extern short GT_SetCaptureEncoder(short trigger,short encoder);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCaptureWidth(short trigger,out short pWidth,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCaptureHomeGpi(short trigger,out short pHomeSts,out short pHomePos,out short pGpiSts,out short pGpiPos,short count);

        [DllImport("gts.dll")]
        internal static extern short GT_Reset();
        [DllImport("gts.dll")]
        internal static extern short GT_GetClock(out uint pClock,out uint pLoop);
        [DllImport("gts.dll")]
        internal static extern short GT_GetClockHighPrecision(out uint pClock);

        [DllImport("gts.dll")]
        internal static extern short GT_GetSts(short axis,out int pSts,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_ClrSts(short axis,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_AxisOn(short axis);
        [DllImport("gts.dll")]
        internal static extern short GT_AxisOff(short axis);
        [DllImport("gts.dll")]
        internal static extern short GT_Stop(int mask,int option);
        [DllImport("gts.dll")]
        internal static extern short GT_SetPrfPos(short profile,int prfPos);
        [DllImport("gts.dll")]
        internal static extern short GT_SynchAxisPos(int mask);
        [DllImport("gts.dll")]
        internal static extern short GT_ZeroPos(short axis,short count);

        [DllImport("gts.dll")]
        internal static extern short GT_SetSoftLimit(short axis,int positive,int negative);
        [DllImport("gts.dll")]
        internal static extern short GT_GetSoftLimit(short axis,out int pPositive,out int pNegative);
        [DllImport("gts.dll")]
        internal static extern short GT_SetAxisBand(short axis,int band,int time);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisBand(short axis,out int pBand,out int pTime);
        [DllImport("gts.dll")]
        internal static extern short GT_SetBacklash(short axis,int compValue,double compChangeValue,int compDir);
        [DllImport("gts.dll")]
        internal static extern short GT_GetBacklash(short axis,out int pCompValue,out double pCompChangeValue,out int pCompDir);
        [DllImport("gts.dll")]
        internal static extern short GT_SetLeadScrewComp(short axis,short n,int startPos,int lenPos,out int pCompPos,out int pCompNeg);
        [DllImport("gts.dll")]
        internal static extern short GT_EnableLeadScrewComp(short axis,short mode);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCompensate(short axis, out double pPitchError, out double pCrossError, out double pBacklashError, out double pEncPos, out double pPrfPos);
        
        [DllImport("gts.dll")]
        internal static extern short GT_EnableGantry(short gantryMaster,short gantrySlave,double masterKp,double slaveKp);
        [DllImport("gts.dll")]
        internal static extern short GT_DisableGantry();
        [DllImport("gts.dll")]
        internal static extern short GT_SetGantryErrLmt(int gantryErrLmt);
        [DllImport("gts.dll")]
        internal static extern short GT_GetGantryErrLmt(out int pGantryErrLmt);

        [DllImport("gts.dll")]
        internal static extern short GT_GetPrfPos(short profile,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetPrfVel(short profile,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetPrfAcc(short profile,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetPrfMode(short profile,out int pValue,short count,out uint pClock);

        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisPrfPos(short axis,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisPrfVel(short axis,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisPrfAcc(short axis,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisEncPos(short axis,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisEncVel(short axis,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisEncAcc(short axis,out double pValue,short count,out uint pClock);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisError(short axis,out double pValue,short count,out uint pClock);

        [DllImport("gts.dll")]
        internal static extern short GT_SetLongVar(short index,int value);
        [DllImport("gts.dll")]
        internal static extern short GT_GetLongVar(short index,out int pValue);
        [DllImport("gts.dll")]
        internal static extern short GT_SetDoubleVar(short index,double pValue);
        [DllImport("gts.dll")]
        internal static extern short GT_GetDoubleVar(short index, out double pValue);

        [DllImport("gts.dll")]
        internal static extern short GT_SetControlFilter(short control,short index);
        [DllImport("gts.dll")]
        internal static extern short GT_GetControlFilter(short control,out short pIndex);

        [DllImport("gts.dll")]
        internal static extern short GT_SetPid(short control,short index,ref TPid pPid);
        [DllImport("gts.dll")]
        internal static extern short GT_GetPid(short control,short index,out TPid pPid);

        [DllImport("gts.dll")]
        internal static extern short GT_SetKvffFilter(short control,short index,short kvffFilterExp,double accMax);
        [DllImport("gts.dll")]
        internal static extern short GT_GetKvffFilter(short control, short index, out short pKvffFilterExp, out double pAccMax);

        [DllImport("gts.dll")]
        internal static extern short GT_Update(int mask);
        [DllImport("gts.dll")]
        internal static extern short GT_SetPos(short profile,int pos);
        [DllImport("gts.dll")]
        internal static extern short GT_GetPos(short profile,out int pPos);
        [DllImport("gts.dll")]
        internal static extern short GT_SetVel(short profile,double vel);
        [DllImport("gts.dll")]
        internal static extern short GT_GetVel(short profile,out double pVel);

        [DllImport("gts.dll")]
        internal static extern short GT_PrfTrap(short profile);
        [DllImport("gts.dll")]
        internal static extern short GT_SetTrapPrm(short profile,ref TTrapPrm pPrm);
        [DllImport("gts.dll")]
        internal static extern short GT_GetTrapPrm(short profile,out TTrapPrm pPrm);

        [DllImport("gts.dll")]
        internal static extern short GT_PrfJog(short profile);
        [DllImport("gts.dll")]
        internal static extern short GT_SetJogPrm(short profile,ref TJogPrm pPrm);
        [DllImport("gts.dll")]
        internal static extern short GT_GetJogPrm(short profile,out TJogPrm pPrm);

        [DllImport("gts.dll")]
        internal static extern short GT_PrfPt(short profile,short mode);
        [DllImport("gts.dll")]
        internal static extern short GT_SetPtLoop(short profile,int loop);
        [DllImport("gts.dll")]
        internal static extern short GT_GetPtLoop(short profile,out int pLoop);
        [DllImport("gts.dll")]
        internal static extern short GT_PtSpace(short profile,out short pSpace,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_PtData(short profile,double pos,int time,short type,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_PtDataWN(short profile,double pos,int time,short type,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_PtClear(short profile,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_PtStart(int mask,int option);
        [DllImport("gts.dll")]
        internal static extern short GT_SetPtMemory(short profile,short memory);
        [DllImport("gts.dll")]
        internal static extern short GT_GetPtMemory(short profile,out short pMemory);
        [DllImport("gts.dll")]
        internal static extern short GT_PtGetSegNum(short profile, out int pSegNum);

        [DllImport("gts.dll")]
        internal static extern short GT_PrfGear(short profile,short dir);
        [DllImport("gts.dll")]
        internal static extern short GT_SetGearMaster(short profile,short masterIndex,short masterType,short masterItem);
        [DllImport("gts.dll")]
        internal static extern short GT_GetGearMaster(short profile,out short pMasterIndex,out short pMasterType,out short pMasterItem);
        [DllImport("gts.dll")]
        internal static extern short GT_SetGearRatio(short profile,int masterEven,int slaveEven,int masterSlope);
        [DllImport("gts.dll")]
        internal static extern short GT_GetGearRatio(short profile,out int pMasterEven,out int pSlaveEven,out int pMasterSlope);
        [DllImport("gts.dll")]
        internal static extern short GT_GearStart(int mask);
        [DllImport("gts.dll")]
        internal static extern short GT_SetGearEvent(short profile,short gearEvent,int startPara0,int startPara1);
        [DllImport("gts.dll")]
        internal static extern short GT_GetGearEvent(short profile, out short pEvent,out int pStartPara0, out int pStartPara1);

        [DllImport("gts.dll")]
        internal static extern short GT_PrfFollow(short profile,short dir);
        [DllImport("gts.dll")]
        internal static extern short GT_SetFollowMaster(short profile,short masterIndex,short masterType,short masterItem);
        [DllImport("gts.dll")]
        internal static extern short GT_GetFollowMaster(short profile,out short pMasterIndex,out short pMasterType,out short pMasterItem);
        [DllImport("gts.dll")]
        internal static extern short GT_SetFollowLoop(short profile,int loop);
        [DllImport("gts.dll")]
        internal static extern short GT_GetFollowLoop(short profile,out int pLoop);
        [DllImport("gts.dll")]
        internal static extern short GT_SetFollowEvent(short profile,short followEvent,short masterDir,int pos);
        [DllImport("gts.dll")]
        internal static extern short GT_GetFollowEvent(short profile,out short pFollowEvent,out short pMasterDir,out int pPos);
        [DllImport("gts.dll")]
        internal static extern short GT_FollowSpace(short profile,out short pSpace,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_FollowData(short profile,int masterSegment,double slaveSegment,short type,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_FollowClear(short profile,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_FollowStart(int mask,int option);
        [DllImport("gts.dll")]
        internal static extern short GT_FollowSwitch(int mask);
        [DllImport("gts.dll")]
        internal static extern short GT_SetFollowMemory(short profile,short memory);
        [DllImport("gts.dll")]
        internal static extern short GT_GetFollowMemory(short profile,out short memory);
        [DllImport("gts.dll")]
        internal static extern short GT_GetFollowStatus(short profile, out short pFifoNum, out short pSwitchStatus);
        [DllImport("gts.dll")]
        internal static extern short GT_SetFollowPhasing(short profile, short profilePhasing);
        [DllImport("gts.dll")]
        internal static extern short GT_GetFollowPhasing(short profile, out short pProfilePhasing);

        [DllImport("gts.dll")]
        internal static extern short GT_Compile(string pFileName, out TCompileInfo pWrongInfo);
        [DllImport("gts.dll")]
        internal static extern short GT_Download(string pFileName);

        [DllImport("gts.dll")]
        internal static extern short GT_GetFunId(string pFunName,out short pFunId);
        [DllImport("gts.dll")]
        internal static extern short GT_Bind(short thread,short funId, short page);

        [DllImport("gts.dll")]
        internal static extern short GT_RunThread(short thread);
        [DllImport("gts.dll")]
        internal static extern short GT_StopThread(short thread);
        [DllImport("gts.dll")]
        internal static extern short GT_PauseThread(short thread);

        [DllImport("gts.dll")]
        internal static extern short GT_GetThreadSts(short thread,out TThreadSts pThreadSts);

        [DllImport("gts.dll")]
        internal static extern short GT_GetVarId(string pFunName,string pVarName,out TVarInfo pVarInfo);
        [DllImport("gts.dll")]
        internal static extern short GT_SetVarValue(short page,ref TVarInfo pVarInfo,ref double pValue,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_GetVarValue(short page,ref TVarInfo pVarInfo,out double pValue,short count);

        [DllImport("gts.dll")]
        internal static extern short GT_SetCrdPrm(short crd, ref TCrdPrm pCrdPrm);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCrdPrm(short crd,out TCrdPrm pCrdPrm);
        [DllImport("gts.dll")]
        internal static extern short GT_CrdSpace(short crd,out int pSpace,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_CrdData(short crd,System.IntPtr pCrdData,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_CrdDataCircle(short crd, ref TCrdData pCrdData, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXY(short crd, int x, int y, double synVel, double synAcc, double velEnd, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZ(short crd, int x, int y, int z, double synVel, double synAcc, double velEnd, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZA(short crd, int x, int y, int z, int a, double synVel, double synAcc, double velEnd, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYG0(short crd, int x, int y, double synVel, double synAcc, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZG0(short crd, int x, int y, int z, double synVel, double synAcc, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZAG0(short crd, int x, int y, int z, int a, double synVel, double synAcc, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYR(short crd, int x, int y, double radius, short circleDir, double synVel, double synAcc, double velEnd, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYC(short crd, int x, int y, double xCenter, double yCenter, short circleDir, double synVel, double synAcc, double velEnd, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcYZR(short crd, int y, int z, double radius, short circleDir, double synVel, double synAcc, double velEnd, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcYZC(short crd, int y, int z, double yCenter, double zCenter, short circleDir, double synVel, double synAcc, double velEnd, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcZXR(short crd, int z, int x, double radius, short circleDir, double synVel, double synAcc, double velEnd, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcZXC(short crd, int z, int x, double zCenter, double xCenter, short circleDir, double synVel, double synAcc, double velEnd, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYZ(short crd,int x,int y,int z,double interX,double interY,double interZ,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYOverride2(short crd,int x,int y,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZOverride2(short crd,int x,int y,int z,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZAOverride2(short crd,int x,int y,int z,int a,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYG0Override2(short crd,int x,int y,double synVel,double synAcc,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZG0Override2(short crd,int x,int y,int z,double synVel,double synAcc,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZAG0Override2(short crd,int x,int y,int z,int a,double synVel,double synAcc,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYROverride2(short crd,int x,int y,double radius,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYCOverride2(short crd,int x,int y,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcYZROverride2(short crd,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcYZCOverride2(short crd,int y,int z,double yCenter,double zCenter,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcZXROverride2(short crd,int z,int x,double radius,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcZXCOverride2(short crd,int z,int x,double zCenter,double xCenter,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixXYRZ(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixXYCZ(short crd,int x,int y,int z,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixYZRX(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixYZCX(short crd,int x,int y,int z,double yCenter,double zCenter,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixZXRY(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixZXCY(short crd,int x,int y,int z,double zCenter,double xCenter,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixXYRZOverride2(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixXYCZOverride2(short crd,int x,int y,int z,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixYZRXOverride2(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixYZCXOverride2(short crd,int x,int y,int z,double yCenter,double zCenter,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixZXRYOverride2(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixZXCYOverride2(short crd,int x,int y,int z,double zCenter,double xCenter,short circleDir,double synVel,double synAcc,double velEnd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYWN(short crd,int x,int y,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZWN(short crd,int x,int y,int z,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZAWN(short crd,int x,int y,int z,int a,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYG0WN(short crd,int x,int y,double synVel,double synAcc,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZG0WN(short crd,int x,int y,int z,double synVel,double synAcc,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZAG0WN(short crd,int x,int y,int z,int a,double synVel,double synAcc,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYRWN(short crd,int x,int y,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYCWN(short crd,int x,int y,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcYZRWN(short crd,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcYZCWN(short crd,int y,int z,double yCenter,double zCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcZXRWN(short crd,int z,int x,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcZXCWN(short crd,int z,int x,double zCenter,double xCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYZWN(short crd,int x,int y,int z,double interX,double interY,double interZ,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYOverride2WN(short crd,int x,int y,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZOverride2WN(short crd,int x,int y,int z,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZAOverride2WN(short crd,int x,int y,int z,int a,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYG0Override2WN(short crd,int x,int y,double synVel,double synAcc,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZG0Override2WN(short crd,int x,int y,int z,double synVel,double synAcc,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_LnXYZAG0Override2WN(short crd,int x,int y,int z,int a,double synVel,double synAcc,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYROverride2WN(short crd,int x,int y,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcXYCOverride2WN(short crd,int x,int y,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcYZROverride2WN(short crd,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcYZCOverride2WN(short crd,int y,int z,double yCenter,double zCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcZXROverride2WN(short crd,int z,int x,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_ArcZXCOverride2WN(short crd,int z,int x,double zCenter,double xCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixXYRZWN(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixXYCZWN(short crd,int x,int y,int z,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixYZRXWN(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixYZCXWN(short crd,int x,int y,int z,double yCenter,double zCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixZXRYWN(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixZXCYWN(short crd,int x,int y,int z,double zCenter,double xCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixXYRZOverride2WN(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixXYCZOverride2WN(short crd,int x,int y,int z,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixYZRXOverride2WN(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixYZCXOverride2WN(short crd,int x,int y,int z,double yCenter,double zCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixZXRYOverride2WN(short crd,int x,int y,int z,double radius,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_HelixZXCYOverride2WN(short crd,int x,int y,int z,double zCenter,double xCenter,short circleDir,double synVel,double synAcc,double velEnd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufIO(short crd, ushort doType, ushort doMask, ushort doValue, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufEnableDoBitPulse(short crd,short doType,short doIndex,ushort highLevelTime,ushort lowLevelTime,int pulseNum,short firstLevel,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufDisableDoBitPulse(short crd,short doType,short doIndex,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufDelay(short crd, ushort delayTime, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufComparePulse(short crd,short level,short outputType,short time,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufDA(short crd, short chn, short daValue, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufLmtsOn(short crd, short axis, short limitType, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufLmtsOff(short crd, short axis, short limitType, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufSetStopIo(short crd, short axis, short stopType, short inputType, short inputIndex, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufMove(short crd, short moveAxis, int pos, double vel, double acc, short modal, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufGear(short crd, short gearAxis, int pos, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufGearPercent(short crd,short gearAxis,int pos,short accPercent,short decPercent,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufStopMotion(short crd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufSetVarValue(short crd,short pageId,out TVarInfo pVarInfo,double value,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufJumpNextSeg(short crd,short axis,short limitType,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufSynchPrfPos(short crd,short encoder,short profile,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufVirtualToActual(short crd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufSetLongVar(short crd,short index,int value,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufSetDoubleVar(short crd,short index,double value,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_CrdStart(short mask,short option);
        [DllImport("gts.dll")]
        internal static extern short GT_CrdStartStep(short mask, short option);
        [DllImport("gts.dll")]
        internal static extern short GT_CrdStepMode(short mask, short option);
        [DllImport("gts.dll")]
        internal static extern short GT_SetOverride(short crd,double synVelRatio);
        [DllImport("gts.dll")]
        internal static extern short GT_SetOverride2(short crd, double synVelRatio);
        [DllImport("gts.dll")]
        internal static extern short GT_InitLookAhead(short crd,short fifo,double T,double accMax,short n,ref TCrdData pLookAheadBuf);
        [DllImport("gts.dll")]
        internal static extern short GT_GetLookAheadSpace(short crd,out int pSpace,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_GetLookAheadSegCount(short crd,out int pSegCount,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_CrdClear(short crd,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_CrdStatus(short crd,out short pRun,out int pSegment,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_SetUserSegNum(short crd,int segNum,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_GetUserSegNum(short crd,out int pSegment,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_GetUserSegNumWN(short crd,out int pSegment,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_GetRemainderSegNum(short crd,out int pSegment,short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_SetCrdStopDec(short crd,double decSmoothStop,double decAbruptStop);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCrdStopDec(short crd,out double pDecSmoothStop,out double pDecAbruptStop);
        [DllImport("gts.dll")]
        internal static extern short GT_SetCrdLmtStopMode(short crd,short lmtStopMode);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCrdLmtStopMode(short crd,out short pLmtStopMode);
        [DllImport("gts.dll")]
        internal static extern short GT_GetUserTargetVel(short crd,out double pTargetVel);
        [DllImport("gts.dll")]
        internal static extern short GT_GetSegTargetPos(short crd,out int pTargetPos);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCrdPos(short crd,out double pPos);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCrdVel(short crd,out double pSynVel);
        [DllImport("gts.dll")]
        internal static extern short GT_SetCrdSingleMaxVel(short crd,ref double pMaxVel);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCrdSingleMaxVel(short crd,out double pMaxVel);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCmdCount(short crd, out short pResult, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_BufLaserOn(short crd,short fifo,short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_BufLaserOff(short crd,short fifo,short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_BufLaserPrfCmd(short crd,double laserPower,short fifo,short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_BufLaserFollowRatio(short crd,double ratio,double minPower,double maxPower,short fifo,short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_BufLaserFollowMode(short crd,short source ,short fifo,short channel,double startPower );
        [DllImport("gts.dll")]
        internal static extern short GT_BufLaserFollowOff(short crd,short fifo,short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_BufLaserFollowSpline(short crd,short tableId,double minPower,double maxPower,short fifo,short channel);

        [DllImport("gts.dll")]
        internal static extern short GT_PrfPvt(short profile);
        [DllImport("gts.dll")]
        internal static extern short GT_SetPvtLoop(short profile,int loop);
        [DllImport("gts.dll")]
        internal static extern short GT_GetPvtLoop(short profile,out int pLoopCount,out int pLoop);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtStatus(short profile,out short pTableId,out double pTime,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtStart(int mask);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtTableSelect(short profile,short tableId);

        [DllImport("gts.dll")]
        internal static extern short GT_PvtTable(short tableId,int count,ref double pTime,ref double pPos,ref double pVel);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtTableEx(short tableId,int count,ref double pTime,ref double pPos,ref double pVelBegin,ref double pVelEnd);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtTableComplete(short tableId,int count,ref double pTime,ref double pPos,ref double pA,ref double pB,ref double pC,double velBegin,double velEnd);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtTablePercent(short tableId,int count,ref double pTime,ref double pPos,ref double pPercent,double velBegin);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtPercentCalculate(int n,ref double pTime,ref double pPos,ref double pPercent,double velBegin,ref double pVel);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtTableContinuous(short tableId,int count,ref double pPos,ref double pVel,ref double pPercent,ref double pVelMax,ref double pAcc,ref double pDec,double timeBegin);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtContinuousCalculate(int n,ref double pPos,ref double pVel,ref double pPercent,ref double pVelMax,ref double pAcc,ref double pDec,ref double pTime);
        [DllImport("gts.dll")]
        internal static extern short GT_PvtTableMovePercent(short tableId, long distance, double vm, double acc, double pa1, double pa2, double dec, double pd1, double pd2, out double pVel, out double pAcc, out double pDec, out double pTime);

        [DllImport("gts.dll")]
        internal static extern short GT_HomeInit();
        [DllImport("gts.dll")]
        internal static extern short GT_Home(short axis,int pos,double vel,double acc,int offset);
        [DllImport("gts.dll")]
        internal static extern short GT_Index(short axis,int pos,int offset);
        [DllImport("gts.dll")]
        internal static extern short GT_HomeStop(short axis,int pos,double vel,double acc);
        [DllImport("gts.dll")]
        internal static extern short GT_HomeSts(short axis,out ushort pStatus);

        [DllImport("gts.dll")]
        internal static extern short GT_HandwheelInit();
        [DllImport("gts.dll")]
        internal static extern short GT_SetHandwheelStopDec(short slave,double decSmoothStop,double decAbruptStop);
        [DllImport("gts.dll")]
        internal static extern short GT_StartHandwheel(short slave,short master,short masterEven,short slaveEven,short intervalTime,double acc,double dec,double vel,short stopWaitTime);
        [DllImport("gts.dll")]
        internal static extern short GT_EndHandwheel(short slave);

        [DllImport("gts.dll")]
        internal static extern short GT_SetTrigger(short i,ref TTrigger pTrigger);
        [DllImport("gts.dll")]
        internal static extern short GT_GetTrigger(short i,out TTrigger pTrigger);
        [DllImport("gts.dll")]
        internal static extern short GT_GetTriggerStatus(short i,out TTriggerStatus pTriggerStatus,short count);
        [DllImport("gts.dll")]
        internal static extern short GT_ClearTriggerStatus(short i);

        [DllImport("gts.dll")]
        internal static extern short GT_SetComparePort(short channel,short hsio0,short hsio1);

        [DllImport("gts.dll")]
        internal static extern short GT_ComparePulse(short level,short outputType,short time);
        [DllImport("gts.dll")]
        internal static extern short GT_CompareStop();
        [DllImport("gts.dll")]
        internal static extern short GT_CompareStatus(out short pStatus,out int pCount);
        [DllImport("gts.dll")]
        internal static extern short GT_CompareData(short encoder,short source,short pulseType,short startLevel,short time,ref int pBuf1,short count1,ref int pBuf2,short count2);
        [DllImport("gts.dll")]
        internal static extern short GT_CompareLinear(short encoder,short channel,int startPos,int repeatTimes,int interval,short time,short source);
        [DllImport("gts.dll")]
        internal static extern short GT_CompareContinuePulseMode(short mode, short count, short standTime);

        [DllImport("gts.dll")]
        internal static extern short GT_SetEncResponseCheck(short control, short dacThreshold, double minEncVel, int time);
        [DllImport("gts.dll")]
        internal static extern short GT_GetEncResponseCheck(short control, out short pDacThreshold, out double pMinEncVel, out int pTime);
        [DllImport("gts.dll")]
        internal static extern short GT_EnableEncResponseCheck(short control);
        [DllImport("gts.dll")]
        internal static extern short GT_DisableEncResponseCheck(short control);

        [DllImport("gts.dll")]
        internal static extern short GT_2DCompareMode(short chn, short mode);
        [DllImport("gts.dll")]
        internal static extern short GT_2DComparePulse(short chn, short level, short outputType, short time);
        [DllImport("gts.dll")]
        internal static extern short GT_2DCompareStop(short chn);
        [DllImport("gts.dll")]
        internal static extern short GT_2DCompareClear(short chn);
        [DllImport("gts.dll")]
        internal static extern short GT_2DCompareStatus(short chn, out short pStatus, out int pCount, out short pFifo, out short pFifoCount, out short pBufCount);
        [DllImport("gts.dll")]
        internal static extern short GT_2DCompareSetPrm(short chn, ref T2DComparePrm pPrm);
        [DllImport("gts.dll")]
        internal static extern short GT_2DCompareData(short chn, short count, ref T2DCompareData pBuf, short fifo);
        [DllImport("gts.dll")]
        internal static extern short GT_2DCompareStart(short chn);

        [DllImport("gts.dll")]
        internal static extern short GT_SetAxisMode(short axis, short mode);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAxisMode(short axis, out short pMode);
        [DllImport("gts.dll")]
        internal static extern short GT_SetHSIOOpt(ushort value, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_GetHSIOOpt(out ushort pValue, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_LaserPowerMode(short laserPowerMode, double maxValue, double minValue, short channel, short delaymode);
        [DllImport("gts.dll")]
        internal static extern short GT_LaserPrfCmd(double outputCmd, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_LaserOutFrq(double outFrq, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_SetPulseWidth(uint width, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_SetWaitPulse(ushort mode, double waitPulseFrq, double waitPulseDuty, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_SetPreVltg(ushort mode, double voltageValue, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_SetLevelDelay(ushort offDelay, ushort onDelay, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_EnaFPK(ushort time1, ushort time2, ushort laserOffDelay, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_DisFPK(short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_SetLaserDisMode(short mode, short source, ref int pPos, ref double pScale, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_SetLaserDisRatio(ref double pRatio, double minPower, double maxPower, short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_SetWaitPulseEx(ushort mode, double waitPulseFrq, double waitPulseDuty);
        [DllImport("gts.dll")]
        internal static extern short GT_SetLaserMode(short mode);
        [DllImport("gts.dll")]
        internal static extern short GT_SetLaserFollowSpline(short tableId,long n,ref double pX,ref double pY,double beginValue,double endValue,short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_GetLaserFollowSpline(short tableId,long n,out double pX,out double pY,out double pA,out double pB,out double pC,out long pCount,short channel);
        
        //ExtMdl
        [DllImport("gts.dll")]
        internal static extern short GT_OpenExtMdl(string pDllName);
        [DllImport("gts.dll")]
        internal static extern short GT_CloseExtMdl();
        [DllImport("gts.dll")]
        internal static extern short GT_SwitchtoCardNoExtMdl(short card);
        [DllImport("gts.dll")]
        internal static extern short GT_ResetExtMdl();
        [DllImport("gts.dll")]
        internal static extern short GT_LoadExtConfig(string pFileName);
        [DllImport("gts.dll")]
        internal static extern short GT_SetExtIoValue(short mdl,ushort value);
        [DllImport("gts.dll")]
        internal static extern short GT_GetExtIoValue(short mdl,out ushort pValue);
        [DllImport("gts.dll")]
        internal static extern short GT_SetExtIoBit(short mdl,short index,ushort value);
        [DllImport("gts.dll")]
        internal static extern short GT_GetExtIoBit(short mdl,short index,out ushort pValue);
        [DllImport("gts.dll")]
        internal static extern short GT_GetExtAdValue(short mdl,short chn,out ushort pValue);
        [DllImport("gts.dll")]
        internal static extern short GT_GetExtAdVoltage(short mdl,short chn,out double pValue);
        [DllImport("gts.dll")]
        internal static extern short GT_SetExtDaValue(short mdl,short chn,ushort value);
        [DllImport("gts.dll")]
        internal static extern short GT_SetExtDaVoltage(short mdl,short chn,double value);
        [DllImport("gts.dll")]
        internal static extern short GT_GetStsExtMdl(short mdl,short chn,out ushort pStatus);
        [DllImport("gts.dll")]
        internal static extern short GT_GetExtDoValue(short mdl,out ushort pValue);
        [DllImport("gts.dll")]
        internal static extern short GT_GetExtMdlMode(out short pMode);
        [DllImport("gts.dll")]
        internal static extern short GT_SetExtMdlMode(short mode);
        [DllImport("gts.dll")]
        internal static extern short GT_UploadConfig();
        [DllImport("gts.dll")]
        internal static extern short GT_DownloadConfig();
        [DllImport("gts.dll")]
        internal static extern short GT_GetUuid(out char pCode,short count);

        //2D Compensate
        internal struct TCompensate2DTable
        {      
            internal short count1;
            internal short count2;
            internal int posBegin1;
            internal int posBegin2;   
            internal int step1;
            internal int step2;
        } 
        internal struct TCompensate2D 
        {
	        internal short enable;                                 
	        internal short tableIndex;                              
	        internal short axisType1;	
            internal short axisType2;              
	        internal short axisIndex1;
            internal short axisIndex2;              
        } 
        [DllImport("gts.dll")]
        internal static extern short GT_SetCompensate2DTable(short tableIndex,ref TCompensate2DTable pTable,ref int pData,short externComp);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCompensate2DTable(short tableIndex,out TCompensate2DTable pTable,out short pExternComp);
        [DllImport("gts.dll")]
        internal static extern short GT_SetCompensate2D(short axis, ref TCompensate2D pComp2d);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCompensate2D(short axis, out TCompensate2D pComp2d);
        [DllImport("gts.dll")]
        internal static extern short GT_GetCompensate2DValue(short axis, out double pValue);

        //Smart Home
        internal const short HOME_STAGE_IDLE=0;
        internal const short HOME_STAGE_START=1;
        internal const short HOME_STAGE_SEARCH_LIMIT=10;
        internal const short HOME_STAGE_SEARCH_LIMIT_STOP=11;
        internal const short HOME_STAGE_SEARCH_LIMIT_ESCAPE = 13;
        internal const short HOME_STAGE_SEARCH_LIMIT_RETURN=15;
        internal const short HOME_STAGE_SEARCH_LIMIT_RETURN_STOP=16;
        internal const short HOME_STAGE_SEARCH_HOME=20;
        internal const short HOME_STAGE_SEARCH_HOME_RETURN=25;
        internal const short HOME_STAGE_SEARCH_INDEX=30;
        internal const short HOME_STAGE_SEARCH_GPI=40;
        internal const short HOME_STAGE_SEARCH_GPI_RETURN=45;
        internal const short HOME_STAGE_GO_HOME=80;
        internal const short HOME_STAGE_END=100;
        internal const short HOME_ERROR_NONE=0;
        internal const short HOME_ERROR_NOT_TRAP_MODE=1;
        internal const short HOME_ERROR_DISABLE=2;
        internal const short HOME_ERROR_ALARM=3;
        internal const short HOME_ERROR_STOP=4;
        internal const short HOME_ERROR_STAGE=5;
        internal const short HOME_ERROR_HOME_MODE=6;
        internal const short HOME_ERROR_SET_CAPTURE_HOME=7;
        internal const short HOME_ERROR_NO_HOME=8;
        internal const short HOME_ERROR_SET_CAPTURE_INDEX=9;
        internal const short HOME_ERROR_NO_INDEX=10;
        internal const short HOME_MODE_LIMIT=10;
        internal const short HOME_MODE_LIMIT_HOME=11;
        internal const short HOME_MODE_LIMIT_INDEX=12;
        internal const short HOME_MODE_LIMIT_HOME_INDEX=13;
        internal const short HOME_MODE_HOME=20;
        internal const short HOME_MODE_HOME_INDEX=22;
        internal const short HOME_MODE_INDEX = 30;
        internal struct THomePrm
        {
	        internal short mode;						
	        internal short moveDir;					
	        internal short indexDir;					
	        internal short edge;					
	        internal short triggerIndex;			
			internal short pad1_1;
	        internal short pad1_2;
            internal short pad1_3;
	        internal double velHigh;				
	        internal double velLow;				
	        internal double acc;					
	        internal double dec;
	        internal short smoothTime;
			internal short pad2_1;
		    internal short pad2_2;
            internal short pad2_3;
	        internal int homeOffset;				
	        internal int searchHomeDistance;	
	        internal int searchIndexDistance;	
	        internal int escapeStep;			
            internal int pad3_1;
            internal int pad3_2;
            internal int pad3_3;
        } 
        internal struct THomeStatus
        {
	        internal short run;
	        internal short stage;
            internal short error;
            internal short pad1;
	        internal int capturePos;
	        internal int targetPos;
        }
        [DllImport("gts.dll")]
        internal static extern short GT_GoHome(short axis, ref THomePrm pHomePrm);
        [DllImport("gts.dll")]
        internal static extern short GT_GetHomePrm(short axis, out THomePrm pHomePrm);
        [DllImport("gts.dll")]
        internal static extern short GT_GetHomeStatus(short axis, out THomeStatus pHomeStatus);

        //Extern Control
        internal struct TControlConfigEx
        {
	        internal short refType;
            internal short refIndex;
            internal short feedbackType;
            internal short feedbackIndex;
            internal int errorLimit;
            internal short feedbackSmooth;
            internal short controlSmooth;	
        }
        [DllImport("gts.dll")]
        internal static extern short GT_SetControlConfigEx(short control, ref TControlConfigEx pControl);
        [DllImport("gts.dll")]
        internal static extern short GT_GetControlConfigEx(short control, out TControlConfigEx pControl);

        //Adc filter
        internal struct TAdcConfig
        {
	        internal short active;
            internal short reverse;
            internal double a;
            internal double b;
            internal short filterMode;	
        }
        [DllImport("gts.dll")]
        internal static extern short GT_SetAdcConfig(short adc, ref TAdcConfig pAdc);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAdcConfig(short adc, out TAdcConfig pAdc);
        [DllImport("gts.dll")]
        internal static extern short GT_SetAdcFilterPrm(short adc, double k);
        [DllImport("gts.dll")]
        internal static extern short GT_GetAdcFilterPrm(short adc, out double pk);

        //Superimposed
        [DllImport("gts.dll")]
        internal static extern short GT_SetControlSuperimposed(short control, short superimposedType, short superimposedIndex);
        [DllImport("gts.dll")]
        internal static extern short GT_GetControlSuperimposed(short control, out short pSuperimposedType, out short pSuperimposedIndex);

        ////////////////////
        [DllImport("gts.dll")]
        internal static extern short GT_ZeroLaserOnTime(short channel);
        [DllImport("gts.dll")]
        internal static extern short GT_GetLaserOnTime(short channel,out uint pTime);
    }
}
