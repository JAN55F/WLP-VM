using HalconDotNet;
using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace VMPro
{
    /// <summary>
    /// 海康威视相机
    /// </summary>
    [Serializable]
    internal class SDK_HIKVision : SDK_Base
    {
        internal SDK_HIKVision(string cameraInfoStr)
        {
            this.CameraInfoStr = cameraInfoStr;
        }

        /// <summary>
        /// 锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 相机集合   键：信息字符串  值：相机对象
        /// </summary>
        private static Dictionary<string, MyCamera> D_cameras = new Dictionary<string, MyCamera>();
        private static Dictionary<string, MyCamera.MV_CC_DEVICE_INFO> D_cameraInfos = new Dictionary<string, MyCamera.MV_CC_DEVICE_INFO>();

        MyCamera.cbOutputExdelegate cbImage;


        /// <summary>
        /// 枚举相机
        /// </summary>
        /// <returns>是否成功</returns>
        internal static bool EnumCamera()
        {
            SDK_HIKVision instance = new SDK_HIKVision(string.Empty);
            return instance.EnumCamrea();
        }

        internal static bool TryCreateManualIpCamera(string ipAddress, out SDK_Base sdkCamera, out string cameraInfoStr)
        {
            sdkCamera = null;
            cameraInfoStr = string.Empty;
            try
            {
                MyCamera.MV_CC_DEVICE_INFO deviceInfo;
                if (!TryFindGigEDeviceByIp(ipAddress, out deviceInfo, out cameraInfoStr))
                    return false;

                D_cameraInfos[cameraInfoStr] = deviceInfo;
                sdkCamera = new SDK_HIKVision(cameraInfoStr);
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }

        /// <summary>
        /// 枚举相机
        /// </summary>
        /// <returns>是否成功</returns>
        internal bool EnumCamrea()
        {
            try
            {
                Machine.UpdateStep(25, "正在枚举海康威视相机", true);

                cbImage = new MyCamera.cbOutputExdelegate(ImageCallBack);
                int nRet = MyCamera.MV_OK;
                MyCamera.MV_CC_DEVICE_INFO_LIST stDevList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
                nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref stDevList);

                if (MyCamera.MV_OK == nRet)
                {
                    MyCamera.MV_CC_DEVICE_INFO stDevInfo;
                    int camIndex = 0;
                    for (Int32 i = 0; i < stDevList.nDeviceNum; i++)
                    {
                        camIndex++;
                        stDevInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                        if (MyCamera.MV_GIGE_DEVICE == stDevInfo.nTLayerType)
                        {
                            string cameraInfoStr = BuildCameraInfo(stDevInfo);
                            stDevInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                            D_cameraInfos[cameraInfoStr] = stDevInfo;
                            if (!AcqImageTool.L_devices.Any(device => device.CameraInfoStr == cameraInfoStr))
                            {
                                SDK_HIKVision sdk_HIKVision = new SDK_HIKVision(cameraInfoStr);
                                AcqImageTool.L_devices.Add(sdk_HIKVision);
                                Frm_FromDevice.Instance.cbx_deviceList.Add(cameraInfoStr);
                                Frm_FromDevice1.Instance.cbx_deviceList.Add(cameraInfoStr);
                            }

                            Machine.UpdateStep(25 + 10 * camIndex, string.Format("初始化海康威视相机{0}[{1}]", camIndex, cameraInfoStr), true);

                            TryOpenCamera(cameraInfoStr, stDevInfo, cbImage, (IntPtr)i);
                        }
                        else if (MyCamera.MV_USB_DEVICE == stDevInfo.nTLayerType)
                        {
                            MyCamera.MV_USB3_DEVICE_INFO stUsb3DeviceInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                        }
                    }
                }
                else
                {
                    Frm_MessageBox.Instance.MessageBoxShow("枚举相机失败", TipType.Error);
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

        private static bool TryFindGigEDeviceByIp(string ipAddress, out MyCamera.MV_CC_DEVICE_INFO deviceInfo, out string cameraInfoStr)
        {
            deviceInfo = new MyCamera.MV_CC_DEVICE_INFO();
            cameraInfoStr = string.Empty;
            MyCamera.MV_CC_DEVICE_INFO_LIST stDevList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref stDevList);
            if (MyCamera.MV_OK != nRet)
                return false;

            for (Int32 i = 0; i < stDevList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO stDevInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (MyCamera.MV_GIGE_DEVICE != stDevInfo.nTLayerType)
                    continue;

                MyCamera.MV_GIGE_DEVICE_INFO stGigEDeviceInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                string currentIp = GetIpAddress(stGigEDeviceInfo.nCurrentIp);
                if (!string.Equals(currentIp, ipAddress, StringComparison.OrdinalIgnoreCase))
                    continue;

                deviceInfo = stDevInfo;
                cameraInfoStr = BuildCameraInfo(stDevInfo);
                return true;
            }

            return false;
        }

        private static string BuildCameraInfo(MyCamera.MV_CC_DEVICE_INFO stDevInfo)
        {
            MyCamera.MV_GIGE_DEVICE_INFO stGigEDeviceInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
            string sn = stGigEDeviceInfo.chSerialNumber;
            string IPAddress = GetIpAddress(stGigEDeviceInfo.nCurrentIp);
            string macAddress = stGigEDeviceInfo.nCurrentSubNetMask.ToString();
            string vendorName = stGigEDeviceInfo.chModelName;
            string friendlyName = stGigEDeviceInfo.chManufacturerName;
            return string.Format("{0} | {1} | {2} | {3} | {4}", sn, IPAddress, macAddress, vendorName, friendlyName);
        }

        private static string GetIpAddress(uint ip)
        {
            uint nIp1 = ((ip & 0xff000000) >> 24);
            uint nIp2 = ((ip & 0x00ff0000) >> 16);
            uint nIp3 = ((ip & 0x0000ff00) >> 8);
            uint nIp4 = (ip & 0x000000ff);
            return string.Format("{0}.{1}.{2}.{3}", nIp1, nIp2, nIp3, nIp4);
        }

        private static bool TryOpenCamera(string cameraInfoStr, MyCamera.MV_CC_DEVICE_INFO stDevInfo, MyCamera.cbOutputExdelegate callback, IntPtr callbackUser)
        {
            MyCamera myCamera = new MyCamera();
            try
            {
                if (D_cameras.ContainsKey(cameraInfoStr))
                    return true;

                int nRet = myCamera.MV_CC_CreateDevice_NET(ref stDevInfo);
                if (MyCamera.MV_OK != nRet)
                    return false;

                nRet = myCamera.MV_CC_OpenDevice_NET((uint)MyCamera.MV_ACCESS_Control, 0);
                if (MyCamera.MV_OK != nRet)
                    nRet = myCamera.MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    myCamera.MV_CC_DestroyDevice_NET();
                    return false;
                }

                int packetSize = myCamera.MV_CC_GetOptimalPacketSize_NET();
                if (packetSize > 0)
                    myCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)packetSize);

                nRet = myCamera.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    myCamera.MV_CC_CloseDevice_NET();
                    myCamera.MV_CC_DestroyDevice_NET();
                    return false;
                }

                MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                nRet = myCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    myCamera.MV_CC_StopGrabbing_NET();
                    myCamera.MV_CC_CloseDevice_NET();
                    myCamera.MV_CC_DestroyDevice_NET();
                    return false;
                }

                if (callback != null)
                    myCamera.MV_CC_RegisterImageCallBackEx_NET(callback, callbackUser);

                D_cameras[cameraInfoStr] = myCamera;
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                try
                {
                    myCamera.MV_CC_CloseDevice_NET();
                    myCamera.MV_CC_DestroyDevice_NET();
                }
                catch
                {
                }
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
                    foreach (KeyValuePair<string, MyCamera> item in D_cameras)
                    {
                        if (item.Key == CameraInfoStr)
                        {
                            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                            int nRet = item.Value.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                            if (MyCamera.MV_OK != nRet)
                            {
                                Frm_MessageBox.Instance.MessageBoxShow("相机采图异常\r\n可能原因：未正常关闭\r\n解决方案：重启电脑或5分钟后重启程序", TipType.Error);
                                break;
                            }
                            UInt32 nPayloadSize = stParam.nCurValue;
                            IntPtr pBufForDriver = Marshal.AllocHGlobal((int)nPayloadSize);
                            IntPtr pBufForSaveImage = IntPtr.Zero;

                            MyCamera.MV_FRAME_OUT_INFO_EX FrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
                            nRet = item.Value.MV_CC_GetOneFrameTimeout_NET(pBufForDriver, nPayloadSize, ref FrameInfo, 1000);
                            if (MyCamera.MV_OK == nRet)
                            {
                                if (pBufForSaveImage == IntPtr.Zero)
                                {
                                    pBufForSaveImage = Marshal.AllocHGlobal((int)(FrameInfo.nHeight * FrameInfo.nWidth * 3 + 2048));
                                }

                                MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
                                stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
                                stSaveParam.enPixelType = FrameInfo.enPixelType;
                                stSaveParam.pData = pBufForDriver;
                                stSaveParam.nDataLen = FrameInfo.nFrameLen;
                                stSaveParam.nHeight = FrameInfo.nHeight;
                                stSaveParam.nWidth = FrameInfo.nWidth;
                                stSaveParam.pImageBuffer = pBufForSaveImage;
                                stSaveParam.nBufferSize = (uint)(FrameInfo.nHeight * FrameInfo.nWidth * 3 + 2048);
                                stSaveParam.nJpgQuality = 80;
                                nRet = item.Value.MV_CC_SaveImageEx_NET(ref stSaveParam);
                                if (MyCamera.MV_OK != nRet)
                                {
                                    Frm_MessageBox.Instance.MessageBoxShow("采图异常", TipType.Error);
                                }
                                byte[] data = new byte[stSaveParam.nImageLen];
                                Marshal.Copy(pBufForSaveImage, data, 0, (int)stSaveParam.nImageLen);

                                //转化成Halcon对象
                                GCHandle hand = GCHandle.Alloc(data, GCHandleType.Pinned);
                                IntPtr pr = hand.AddrOfPinnedObject();
                                HOperatorSet.GenImage1(out image, new HTuple("byte"), stSaveParam.nWidth, stSaveParam.nHeight, pBufForDriver);
                                if (hand.IsAllocated)
                                    hand.Free();
                                Marshal.FreeHGlobal(pBufForDriver);
                                Marshal.FreeHGlobal(pBufForSaveImage);
                                break;
                            }
                            else
                            {
                                Frm_MessageBox.Instance.MessageBoxShow("采图异常", TipType.Error);
                            }
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
        /// 采图回调函数
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pFrameInfo"></param>
        /// <param name="pUser"></param>
        private void ImageCallBack(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            try
            {
                HObject image = null;
                int nIndex = (int)pUser;

                // ch:抓取的帧数 | en:Aquired Frame Number
                //////++m_nFrames[nIndex];

                //ch:判断是否需要保存图片 | en:Determine whether to save image
                //////if (m_bSaveImg[nIndex])
                //////{
                //////    SaveImage(pData, pFrameInfo, nIndex);
                //////    m_bSaveImg[nIndex] = false;
                //////}


                IntPtr pBufForSaveImage = IntPtr.Zero;




                //MyCamera.MV_SAVE_IMAGE_PARAM_EX stDisplayInfo = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
                ////stDisplayInfo.hWnd = m_hDisplayHandle[nIndex];
                //stDisplayInfo.pData = pData;
                //stDisplayInfo.nDataLen = pFrameInfo.nFrameLen;
                //stDisplayInfo.nWidth = pFrameInfo.nWidth;
                //stDisplayInfo.nHeight = pFrameInfo.nHeight;
                //stDisplayInfo.enPixelType = pFrameInfo.enPixelType;


                //////item .Value .MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);

                if (pBufForSaveImage == IntPtr.Zero)
                {
                    pBufForSaveImage = Marshal.AllocHGlobal((int)(pFrameInfo.nHeight * pFrameInfo.nWidth * 3 + 2048));
                }



                MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
                stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
                stSaveParam.enPixelType = pFrameInfo.enPixelType;
                stSaveParam.pData = pData;
                stSaveParam.nDataLen = pFrameInfo.nFrameLen;
                stSaveParam.nHeight = pFrameInfo.nHeight;
                stSaveParam.nWidth = pFrameInfo.nWidth;
                stSaveParam.pImageBuffer = pBufForSaveImage;
                stSaveParam.nBufferSize = (uint)(pFrameInfo.nHeight * pFrameInfo.nWidth * 3 + 2048);
                stSaveParam.nJpgQuality = 80;


                foreach (KeyValuePair<string, MyCamera> item in D_cameras)
                {
                    if (item.Key == CameraInfoStr)
                    {
                        int nRet = item.Value.MV_CC_SaveImageEx_NET(ref stSaveParam);
                        if (MyCamera.MV_OK != nRet)
                        {
                            Frm_MessageBox.Instance.MessageBoxShow("采图异常", TipType.Error);
                        }
                    }
                }
                byte[] data = new byte[stSaveParam.nImageLen];
                Marshal.Copy(pBufForSaveImage, data, 0, (int)stSaveParam.nImageLen);

                //转化成Halcon对象
                GCHandle hand = GCHandle.Alloc(data, GCHandleType.Pinned);
                IntPtr pr = hand.AddrOfPinnedObject();
                HOperatorSet.GenImage1(out image, new HTuple("byte"), stSaveParam.nWidth, stSaveParam.nHeight, pData);
                if (hand.IsAllocated)
                    hand.Free();
                Marshal.FreeHGlobal(pData);
                Marshal.FreeHGlobal(pBufForSaveImage);

                waitingHardTriggerImage = false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        /// <param name="exposure">曝光时间</param>
        internal override void SetExposure(double exposure)
        {
            // 与 GrabOneImage 的 obj 锁共用：拖动曝光时每个刻度都会触发一次采集 Run，
            // 若 SetExposure（含 200ms 生效等待）与 GrabOneImage 并发进入原生 SDK
            //（海康 MVCAMSDK 非线程安全），会直接在原生层访问冲突崩溃（无托管异常、进程闪退）。
            // 串行化后，超时被遗弃的工作线程也只会在锁上排队，不再叠加并发调用。
            lock (obj)
            {
                try
                {
                    foreach (KeyValuePair<string, MyCamera> item in D_cameras)
                    {
                        if (item.Key == CameraInfoStr)
                        {
                            item.Value.MV_CC_SetEnumValue_NET("ExposureAuto", 0);
                            item.Value.MV_CC_SetFloatValue_NET("ExposureTime", (float)exposure * 1000);
                            Thread.Sleep(200);      //海康威视相机，实测发现设置完曝光后200毫秒以后才能生效，所以如此，当然这会影响CT
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
            foreach (KeyValuePair<string, MyCamera> item in D_cameras)
            {
                if (item.Key == CameraInfoStr)
                    return true;
            }
            return false;
        }
        internal override bool TryOpenCamera()
        {
            try
            {
                if (D_cameras.ContainsKey(CameraInfoStr))
                    return true;

                MyCamera.MV_CC_DEVICE_INFO deviceInfo;
                if (!D_cameraInfos.TryGetValue(CameraInfoStr, out deviceInfo))
                    return false;

                cbImage = new MyCamera.cbOutputExdelegate(ImageCallBack);
                return TryOpenCamera(CameraInfoStr, deviceInfo, cbImage, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 关闭所有相机
        /// </summary>
        internal static void CloseAllCamera()
        {
            try
            {
                foreach (KeyValuePair<string, MyCamera> item in D_cameras)
                {
                    item.Value.MV_CC_StopGrabbing_NET();
                    item.Value.MV_CC_CloseDevice_NET();
                    item.Value.MV_CC_DestroyDevice_NET();
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
