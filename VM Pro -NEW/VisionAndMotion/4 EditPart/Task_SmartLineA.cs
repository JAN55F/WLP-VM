using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gts;
using csIOC0640;
using System.Windows;
using System.IO;
using Tool;
using System.Drawing;
using ShareMemNet;
using HalconDotNet;

namespace VMPro
{
    /// <summary>
    /// 示例主逻辑类，用户按照此类编写自己具体项目的主逻辑类
    /// </summary>
    public static class Task_SmartLineA
    {

        internal static void AutoRun()
        {
            return;
            int ret = ShareMemProH.PH_Init();
            if (0 != ret)
            {
                Frm_MessageBox.Instance.MessageBoxShow("\r\nPLC连接失败", TipType.Error);
            }
            Thread th = new Thread(Work);
            th.IsBackground = true;
            th.Start();
        }
        private static string ReadData(int addr)
        {
            string value = string.Empty;
            for (int i = 0; i < 12; i++)
            {
                short[] data = new short[ShareMemProH.RW_MAX_NYUM];
                int ret = ShareMemProH.PH_Read((ushort)CmdRead.ELEM_VD, (ushort)addr, 1, data);
                value = data[0].ToString();
            }
            return value;
        }
        private static void WriteData(int addr, double value)
        {
            short[] data2 = new short[ShareMemProH.RW_MAX_NYUM];
            ushort dw_value2;
            dw_value2 = (ushort)value;
            data2[0] = (short)dw_value2;
            int ret = ShareMemProH.PH_Write((ushort)CmdWrite.ELEM_VD, (ushort)addr, 1, data2);
            if (ret != 0)
                MessageBox.Show("写入数据时出错");
        }
        static Int64 num = 0;
        private static void Work()
        {
            try
            {

                while (true)
                {
                    if (Machine.machineRunStatu == MachineRunStatu.Running)
                    {
                        //////int result = Convert.ToInt16(ReadData(180));
                        //////if (result != 0)
                        //////{
                        //////    Thread.Sleep(Project .Instance .configuration .timeBetweenJobRun );
                        //////    WriteData(180, 0);
                        //////    //continue;
                        //////    Job job = Job.FindJobByName("轴走位精度测试");
                        //////    string  curTime1 = DateTime.Now.ToString("HH:ss:mm:ms");
                        //////    job.Run();
                        //////    if (job.jobRunStatu == JobRunStatu.Succeed)
                        //////    {
                        //////        List<Point> point = job.GetOutputItemValue("手眼标定->输出点") as List<Point>;


                        //////        DateTime curTime = DateTime.Now;
                        //////        string fileName = curTime.ToString("yyyy_MM_dd") + ".csv";
                        //////        string filePath = Project.Instance.configuration.dataPath;
                        //////        if (!Directory.Exists(filePath))
                        //////            Directory.CreateDirectory(filePath);
                        //////        if (!File.Exists(filePath + "\\" + fileName))
                        //////        {
                        //////            File.Create(filePath + "\\" + fileName).Close();
                        //////            File.AppendAllText(filePath + "\\" + fileName, "时间,编号,目标X,目标Y\r\n");
                        //////        }

                        //////        File.AppendAllText(filePath + "\\" + fileName, string.Format("{0},{1},{2},{3}\r\n", curTime1, num.ToString(),  (point[0].X).ToString("0.000"), (point[0].Y ).ToString("0.000")));
                        //////        num++;
                        //////    }

                        //////    //////HObject image;
                        //////    //////HOperatorSet.DumpWindowImage(out image, Job.FindJobByName("轴走位精度测试").GetImageWindowControl().hwc_imageWindow.HWindowHalconID);
                        //////    //////HOperatorSet.WriteImage(image, "bmp", 0, Project.Instance.configuration.dataPath + "\\Image\\" + num.ToString());

                        //////    WriteData(190, 1);
                        //////}








                        int result = Convert.ToInt16(ReadData(180));
                        if (result != 0)
                        {
                            //确认打螺丝还是拆螺丝
                            int mode = Convert.ToInt16(ReadData(186));

                            if (mode == 0)
                            {
                                WriteData(180, 0);

                                ((EyeHandCalibTool)(Job.FindJobByName("打螺丝").FindToolByName("手眼标定"))).curPhotoPosIndex = result - 1;
                                Job job = Job.FindJobByName("打螺丝");
                                job.Run();


                                if (job.jobRunStatu == JobRunStatu.Succeed)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        XY point = job.GetOutputItemValue(string.Format("点偏差_{0}->点", i + 1)) as XY;
                                        int addX = 200 + (result - 1) * 4 + i;
                                        int addY = 400 + (result - 1) * 4 + i;
                                        int valueX = Convert.ToInt16(point.X * 100);
                                        int valueY = Convert.ToInt16(point.Y * 100);
                                        WriteData(addX, valueX);
                                        WriteData(addY, valueY);

                                        if (!File.Exists(@"C:\Users\Administrator\Desktop\新建文本文档.txt"))
                                            File.Create(@"C:\Users\Administrator\Desktop\新建文本文档.txt").Close();
                                        File.AppendAllText(@"C:\Users\Administrator\Desktop\新建文本文档.txt", result + "  " + valueX.ToString() + " " + valueY.ToString() + Environment.NewLine);


                                    }
                                    WriteData(190, 1);
                                }
                                else
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        WriteData(200 + (result - 1) * 4 + i, 0);
                                        WriteData(400 + (result - 1) * 4 + i, 0);
                                    }
                                    WriteData(190, 2);
                                }
                            }
                            else if (mode == 1)
                            {
                                WriteData(180, 0);

                                ((EyeHandCalibTool)(Job.FindJobByName("拆螺丝").FindToolByName("手眼标定"))).curPhotoPosIndex = result - 1;
                                Job job = Job.FindJobByName("拆螺丝");
                                job.Run();


                                if (job.jobRunStatu == JobRunStatu.Succeed)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        XY point = job.GetOutputItemValue(string.Format("点偏差_{0}->点", i + 1)) as XY;
                                        int addX = 200 + (result - 1) * 4 + i;
                                        int addY = 400 + (result - 1) * 4 + i;
                                        int valueX = Convert.ToInt16(point.X * 100);
                                        int valueY = Convert.ToInt16(point.Y * 100);
                                        WriteData(addX, valueX);
                                        WriteData(addY, valueY);

                                        if (!File.Exists(@"C:\Users\Administrator\Desktop\新建文本文档.txt"))
                                            File.Create(@"C:\Users\Administrator\Desktop\新建文本文档.txt").Close();
                                        File.AppendAllText(@"C:\Users\Administrator\Desktop\新建文本文档.txt", result + "  " + valueX.ToString() + " " + valueY.ToString() + Environment.NewLine);


                                    }
                                    WriteData(190, 1);
                                }
                                else
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        WriteData(200 + (result - 1) * 4 + i, 0);
                                        WriteData(400 + (result - 1) * 4 + i, 0);
                                    }
                                    WriteData(190, 2);
                                }
                            }




                        }
                    }

                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
    public enum Di
    {
        启动,
        暂停,
        停止,
        复位,
        急停,
        安全门,
        备用1,
        定位升降下,
        定位升降上,
        上压接模到位接近开关,
        PCB板流入光电检测,
        PCB板到位光电检测,
        PCB板流出光电检测,
        PCB板阻挡伸出位,
        PCB板阻挡缩回位,
        PCB板侧边定位1伸出位,

        PCB板侧边定位2伸出位,
        PCB板侧边定位3伸出位,
        PCB板压紧1压紧位,
        PCB板压紧2压紧位,
        PCB板压紧3压紧位,
        PCB板压紧4压紧位,
        换模伸缩气缸缩回位,
        换模伸缩气缸伸出位,
        换模夹抓气缸夹紧位,
        换模气缸感应,
        一号模感应,
        二号模感应,
        三号模感应,
        四号模感应,
        五号模感应,
        六号模感应,
    }
    public enum Do
    {
        三色灯_红,
        三色灯_绿,
        三色灯_黄,
        三色灯_蜂鸣,
        备用1,
        备用2,
        备用3,
        备用4,
        备用5,
        备用6,
        PCB板阻挡气缸,
        PCB板侧边定位气缸,
        PCB板压紧气缸,
        PCB板顶升气缸,
        备用7,
        上压接模吹气锁紧,

        换模伸缩气缸,
        换模夹爪气缸,
        备用8,
        备用9,
        备用10,
        备用11,
        备用12,
        备用13,
        备用14,
        备用15,
        备用16,
        备用17,
        备用18,
        备用19,
        备用20,
        备用21,
        备用22,
        备用23,
    }
}
