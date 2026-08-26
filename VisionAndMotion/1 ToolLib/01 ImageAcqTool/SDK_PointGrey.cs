using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyCapture2Managed;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using HalconDotNet;
using System.Drawing.Imaging;
using System.Threading;

namespace VMPro
{
    /// <summary>
    /// 灰点相机
    /// </summary>
    [Serializable]
    internal class SDK_PointGrey : SDK_Base
    {
        internal SDK_PointGrey(string cameraInfoStr)
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
        private static Dictionary<string, ManagedCamera> D_cameras = new Dictionary<string, ManagedCamera>();


        /// <summary>
        /// 枚举相机
        /// </summary>
        /// <returns>是否成功</returns>
        internal static bool EnumCamera()
        {
            try
            {
                Machine.UpdateStep(25, "正在枚举灰点相机", true);

                ManagedBusManager busMgr = new ManagedBusManager();
                uint cameraNum = busMgr.GetNumOfCameras();
                int camIndex = 0;

                for (int i = 0; i < cameraNum; i++)
                {
                    ManagedPGRGuid guid = busMgr.GetCameraFromIndex(0);
                    ManagedCamera camera = new ManagedCamera();
                    camera.Connect(guid);
                    EmbeddedImageInfo embeddedInfo = camera.GetEmbeddedImageInfo();
                    if (embeddedInfo.timestamp.available == true)
                        embeddedInfo.timestamp.onOff = true;

                    if (embeddedInfo.exposure.available == true)
                    {
                        embeddedInfo.exposure.onOff = false;
                        embeddedInfo.shutter.onOff = false;
                        embeddedInfo.gain.onOff = false;
                    }
                    camera.SetEmbeddedImageInfo(embeddedInfo);

                    CameraProperty cameraShutter = camera.GetProperty(PropertyType.Shutter);
                    cameraShutter.autoManualMode = false;
                    cameraShutter.absControl = true;

                    CameraProperty cameraGain = camera.GetProperty(PropertyType.Gain);
                    cameraGain.autoManualMode = false;
                    cameraGain.absControl = true;

                    CameraProperty cameraExposure = camera.GetProperty(PropertyType.AutoExposure);
                    cameraExposure.autoManualMode = false;
                    cameraExposure.absControl = true;
                    cameraExposure.absValue = 30;

                    CameraProperty cameraAutoBrightness = camera.GetProperty(PropertyType.Brightness);
                    cameraAutoBrightness.autoManualMode = false;
                    cameraAutoBrightness.absControl = true;

                    //设置采图模式  灰度图或彩色图等
                    VideoMode videoMode = VideoMode.NumberOfVideoModes;
                    FrameRate frameRate = FrameRate.NumberOfFrameRates;
                    camera.GetVideoModeAndFrameRate(ref videoMode, ref frameRate);
                    videoMode = VideoMode.VideoMode1600x1200Y8;

                    camera.SetProperty(cameraShutter);
                    camera.SetProperty(cameraGain);
                    camera.SetProperty(cameraExposure);
                    camera.SetProperty(cameraAutoBrightness);
                    camera.SetVideoModeAndFrameRate(videoMode, frameRate);
                    Thread.Sleep(100);
                    camera.StartCapture();

                    CameraInfo cameraInfo = camera.GetCameraInfo();
                    string sn = cameraInfo.serialNumber.ToString();
                    string IPAddress = cameraInfo.ipAddress.ToString();
                    string macAddress = cameraInfo.macAddress.ToString();
                    string vendorName = cameraInfo.vendorName;
                    string friendlyName = cameraInfo.modelName;         //待更正

                    string cameraInfoStr = string.Format("{0} | {1} | {2} | {3} | {4}", sn, IPAddress, macAddress, vendorName, friendlyName);
                    Machine.UpdateStep(25 + 10 * camIndex, string.Format("初始化灰点相机{0}[{1}]", camIndex, friendlyName), true);
                    SDK_PointGrey sdk_PointGrey = new SDK_PointGrey(cameraInfoStr);

                    D_cameras.Add(cameraInfoStr, camera);
                    AcqImageTool.L_devices.Add(sdk_PointGrey);
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
                    foreach (KeyValuePair<string, ManagedCamera> item in D_cameras)
                    {
                        if (item.Key != CameraInfoStr)
                            continue;

                        ManagedImage rawImage = new ManagedImage();
                        item.Value.RetrieveBuffer(rawImage);
                        ManagedImage convertedImage = new ManagedImage();
                        rawImage.Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, convertedImage);
                        System.Drawing.Bitmap bitmap = convertedImage.bitmap;

                        Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                        BitmapData srcBmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        HOperatorSet.GenImageInterleaved(out image, srcBmpData.Scan0, "bgr", bitmap.Width, bitmap.Height, 0, "byte", 0, 0, 0, 0, -1, 0);
                        bitmap.UnlockBits(srcBmpData);
                        break;
                    }
                    return image;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new HObject();
            }
        }
        /// <summary>
        /// 设置相机曝光时间，exposure 单位为 ms。
        /// </summary>
        internal override void SetExposure(double exposure)
        {
            try
            {
                foreach (KeyValuePair<string, ManagedCamera> item in D_cameras)
                {
                    if (item.Key != CameraInfoStr)
                        continue;

                    CameraProperty cameraShutter = item.Value.GetProperty(PropertyType.Shutter);
                    cameraShutter.autoManualMode = false;
                    cameraShutter.absControl = true;
                    cameraShutter.absValue = (float)exposure;
                    item.Value.SetProperty(cameraShutter);
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        internal override bool CheckCamExist()
        {
            foreach (KeyValuePair<string, ManagedCamera> item in D_cameras)
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
                foreach (KeyValuePair<string, ManagedCamera> item in D_cameras)
                {
                    item.Value.StopCapture();
                    item.Value.Disconnect();
                    item.Value.Dispose();
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
