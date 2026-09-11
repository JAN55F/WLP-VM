using HalconDotNet;
using MVSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using CameraHandle = System.Int32;

namespace VMPro
{
    /// <summary>
    /// 迈德威视相机
    /// </summary>
    internal class SDK_MindVision : SDK_Base
    {
        internal SDK_MindVision(string cameraInfoStr)
        {
            this.CameraInfoStr = cameraInfoStr;
        }

        /// <summary>
        /// 锁
        /// </summary>
        private object obj = new object();
        /// <summary>pir 
        /// 相机集合   键：信息字符串  值：相机句柄
        /// </summary>
        private static Dictionary<string, CameraHandle> D_cameras = new Dictionary<string, CameraHandle>();

        static IntPtr frameBuffer;

        /// <summary>
        /// 枚举相机
        /// </summary>
        /// <returns>是否成功</returns>
        internal static bool EnumCamera()
        {
            try
            {
                Machine.UpdateStep(25, "正在枚举迈德威视相机", true);

                tSdkCameraDevInfo[] CameraList = new tSdkCameraDevInfo[16];
                MvApi.CameraEnumerateDevice(out  CameraList);
                if (CameraList == null || CameraList.Length < 0)
                    return false;

                int camIndex = 0;
                for (int i = 0; i < CameraList.Length; i++)
                {
                    camIndex++;
                    //获取相机SN
                    byte[] buffer = CameraList[i].acSn;
                    string sn = Encoding.Default.GetString(buffer);
                    sn = Regex.Split(sn, "\0")[0];

                    //获取相机IP        //待完善
                    //MvApi .CameraGigeGetIp ();

                    //获取相机型号
                    buffer = CameraList[i].acFriendlyName;
                    string friendlyName = Encoding.Default.GetString(buffer);
                    friendlyName = Regex.Split(friendlyName, "\0")[0];

                    //获取网卡名
                    string networdName = string.Empty;
                    string cameraMac, networdCardMac;
                    MvApi.CameraGigeGetMac(ref CameraList[i], out cameraMac, out networdCardMac);
                    networdCardMac = networdCardMac.Replace("-", "");
                    NetworkInterface[] networkInterface = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (var item in networkInterface)
                    {
                        string localMac = item.GetPhysicalAddress().ToString();
                        if (localMac == networdCardMac)
                        {
                            networdName = item.Name;
                            break;
                        }
                    }

                    CameraHandle cameraHandle = 0;
                    MvApi.CameraInit(ref  CameraList[i], -1, -1, ref cameraHandle);

                    tSdkCameraCapbility CameraInfo;
                    MvApi.CameraGetCapability(cameraHandle, out CameraInfo);
                    uint value = CameraInfo.sIspCapacity.bMonoSensor;
                    bool isMonoCamera = false;
                    if (value == 1)
                    {
                        isMonoCamera = true;
                        MvApi.CameraSetIspOutFormat(cameraHandle, (int)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
                    }
                    MvApi.CameraSetTriggerMode(cameraHandle, 1);
                    MvApi.CameraSetAeState(cameraHandle, 0);

                    MvApi.CameraPlay(cameraHandle);
                    int FrameBufferSize = CameraInfo.sResolutionRange.iWidthMax * CameraInfo.sResolutionRange.iWidthMax * (isMonoCamera ? 1 : 3);
                    frameBuffer = (IntPtr)MvApi.CameraAlignMalloc(FrameBufferSize, 16);

                    string cameraInfoStr = string.Format("{0} | {1} | {2}", sn, networdName, friendlyName);
                    Machine.UpdateStep(25 + 10 * camIndex, string.Format("初始化迈德威视相机{0}[{1}]", camIndex, cameraInfoStr), true);
                    SDK_MindVision sdk_MindVision = new SDK_MindVision(cameraInfoStr);

                    D_cameras.Add(cameraInfoStr, cameraHandle);
                    AcqImageTool.L_devices.Add(sdk_MindVision);
                    Frm_FromDevice.Instance.cbx_deviceList.Add(cameraInfoStr);
                    Frm_FromDevice1.Instance.cbx_deviceList.Add(cameraInfoStr);
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
        /// 采图
        /// </summary>
        /// <returns>采集到的图像</returns>
        internal override HObject GrabOneImage()
        {
            try
            {
                lock (obj)
                {
                    HObject image = null;
                    foreach (KeyValuePair<string, CameraHandle> item in D_cameras)
                    {
                        if (item.Key != CameraInfoStr)
                            continue;

                        IntPtr pRawData;
                        MvApi.CameraSoftTrigger(item.Value);
                        tSdkFrameHead frameHead;
                        CameraSdkStatus statu = MvApi.CameraGetImageBuffer(item.Value, out  frameHead, out  pRawData, 2000);
                        if (statu == MVSDK.CameraSdkStatus.CAMERA_STATUS_SUCCESS)
                        {
                            MvApi.CameraImageProcess(item.Value, pRawData, frameBuffer, ref frameHead);
                            MvApi.CameraReleaseImageBuffer(item.Value, pRawData);
                            image = Trans_Himage_To_Hobject(frameBuffer, ref frameHead);
                            break;
                        }
                        else
                        {
                            Frm_MessageBox.Instance.MessageBoxShow("采图异常", TipType.Error);
                        }
                    }
                    return image;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 将Himage类型转化成Hobject类型
        /// </summary>
        /// <param name="pFrameBuffer">Himage类型待转图像</param>
        /// <param name="pFrameHead">待转图像的帧头</param>
        /// <returns>输出已转化的Hobject类型图像</returns>
        private HObject Trans_Himage_To_Hobject(IntPtr pFrameBuffer, ref tSdkFrameHead pFrameHead)
        {
            try
            {
                int w = pFrameHead.iWidth;
                int h = pFrameHead.iHeight;
                HObject Image = null;

                if (pFrameHead.uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8)
                    HOperatorSet.GenImage1(out Image, "byte", w, h, pFrameBuffer);
                else if (pFrameHead.uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_BGR8)
                    HOperatorSet.GenImageInterleaved(out Image,
                                                         pFrameBuffer,
                                                         "bgr",
                                                         w,
                                                         h,
                                                         -1,
                                                         "byte",
                                                         w,
                                                         h,
                                                         0,
                                                         0,
                                                         -1,
                                                         0);
                else
                    Frm_MessageBox.Instance.MessageBoxShow("图像格式不支持转换", TipType.Error);

                HObject ImageRaw = Image;
                HOperatorSet.MirrorImage(ImageRaw, out Image, "row");
                ImageRaw.Dispose();
                HOperatorSet.ImageToChannels(Image, out Image);
                return Image;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        /// <param name="exposure">曝光时间(ms)</param>
        internal override void SetExposure(double exposure)
        {
            // 与 GrabOneImage 的 obj 锁共用，避免拖动曝光时 SetExposure 与采图并发进入原生 SDK 崩溃
            lock (obj)
            {
                try
                {
                    foreach (KeyValuePair<string, CameraHandle> item in D_cameras)
                    {
                        if (item.Key == CameraInfoStr)
                        {
                            MvApi.CameraSetExposureTime(item.Value, exposure * 1000);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveError(ex);
                }
            }
        }
        internal override bool CheckCamExist()
        {
            foreach (KeyValuePair<string, CameraHandle> item in D_cameras)
            {
                if (item.Key == CameraInfoStr)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 关闭所有相机
        /// </summary>
        internal static void CloseAllCamera()
        {
            try
            {
                foreach (KeyValuePair<string, CameraHandle> item in D_cameras)
                {
                    MvApi.CameraUnInit(item.Value);
                    MvApi.CameraAlignFree(frameBuffer);
                }
                D_cameras.Clear();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


    }
}
