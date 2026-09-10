using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net;

namespace VMPro
{
    [Serializable]
    internal class OneKeyEyeHandCalibTool : ToolBase
    {

        /// <summary>
        /// IP地址
        /// </summary>
        internal string ip = "192.168.0.1";
        /// <summary>
        /// 端口号
        /// </summary>
        internal int port = 2000;
        /// <summary>
        /// 输入图像
        /// </summary>
        internal HObject inputImage;
        /// <summary>
        /// 输出图像
        /// </summary>
        internal HObject outputImage;
        /// <summary>
        /// 输入位置
        /// </summary>
        internal XYU inputPose = new XYU();
        /// <summary>
        /// 输出位置
        /// </summary>
        internal XYU outputPose = new XYU();
        /// <summary>
        /// 标定用的流程名称
        /// </summary>
        internal string calibJobName = string.Empty;
        /// <summary>
        /// 标定用的输出项的名称
        /// </summary>
        internal string calibItemName = string.Empty;
        /// <summary>
        /// 放射变换矩阵
        /// </summary>
        internal HTuple homMat2D = (HTuple)(new HHomMat2D());
        /// <summary>
        /// 标定类型 四点标定|九点标定
        /// </summary>
        internal CalibType calibType = CalibType.Four_Point;
        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 标定数据
        /// </summary>
        internal List<List<double>> L_calibData = new List<List<double>>();
        /// <summary>
        /// X平移
        /// </summary>
        private HTuple _translateX = 0;
        internal HTuple TranslateX
        {
            get
            {
                _translateX = Math.Round((double)_translateX, 5);
                return _translateX;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _translateX = value;
            }
        }
        /// <summary>
        /// Y平移
        /// </summary>
        private HTuple _translateY = 0;
        internal HTuple TranslateY
        {
            get
            {
                _translateY = Math.Round((double)_translateY, 5);
                return _translateY;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _translateY = value;
            }
        }
        /// <summary>
        /// X缩放
        /// </summary>
        private HTuple _scanX = 1;
        internal HTuple ScanX
        {
            get
            {
                _scanX = Math.Round((double)_scanX, 5);
                return _scanX;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _scanX = value;
            }
        }
        /// <summary>
        /// Y缩放
        /// </summary>
        private HTuple _scanY = 1;
        internal HTuple ScanY
        {
            get
            {
                _scanY = Math.Round((double)_scanY, 5);
                return _scanY;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _scanY = value;
            }
        }
        /// <summary>
        /// 角度旋转
        /// </summary>
        private HTuple _rotation = 0;
        internal HTuple Rotation
        {
            get
            {
                _rotation = Math.Round((double)_rotation, 5);
                return _rotation;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _rotation = value;
            }
        }
        /// <summary>
        /// 轴斜切
        /// </summary>
        private HTuple _theta = 0;
        internal HTuple Theta
        {
            get
            {
                _theta = Math.Round((double)_theta, 5);
                return _theta;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _theta = value;
            }
        }


        /// <summary>
        /// 复位工具
        /// </summary>
        internal void ResetTool()
        {
            try
            {
                TranslateX = 0;
                TranslateY = 0;
                ScanX = 1;
                ScanY = 1;
                Rotation = 0;
                Theta = 0;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows.Clear();
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows.Add(4);
                Frm_OneKeyEyeHandCalibTool.Instance.tbx_translateX.Text = "0";
                Frm_OneKeyEyeHandCalibTool.Instance.tbx_translateY.Text = "0";
                Frm_OneKeyEyeHandCalibTool.Instance.tbx_scaleX.Text = "1";
                Frm_OneKeyEyeHandCalibTool.Instance.tbx_scaleY.Text = "1";
                Frm_OneKeyEyeHandCalibTool.Instance.tbx_rotation.Text = "0";
                Frm_OneKeyEyeHandCalibTool.Instance.tbx_theta.Text = "0";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 导出标定数据
        /// </summary>
        internal void WriteCalibData()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                dig_saveImage.FileName = "Data " + DateTime.Now.ToString("yyyy_MM_dd");
                dig_saveImage.Title = (Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择导出路径");
                dig_saveImage.Filter = "CSV文件(*.csv)|*.csv|XLS文件(*.xls)|*.xls";
                dig_saveImage.InitialDirectory = path;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {
                    File.Create(dig_saveImage.FileName).Close();
                    string data = string.Empty;
                    for (int i = 0; i < Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows.Count; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            data += Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[j].Value;
                            if (j < 3)
                                data += ",";
                        }
                        data += Environment.NewLine;
                    }
                    File.AppendAllText(dig_saveImage.FileName, data, Encoding.Default);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 导入标定数据
        /// </summary>
        internal void ReadCalibData()
        {
            System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
            dig_openImage.FileName = string.Empty;
            dig_openImage.Title = (Project.Instance.configuration.language == Language.English ? "Please select a form file" : "请选择表格文件");
            dig_openImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            dig_openImage.Filter = (Project.Instance.configuration.language == Language.English ? "Image File(*.*)|*.*|Image File(*.png)|*.txt|Image File(*.jpg)|*.jpg|Image File(*.bmp)|*.bmp|Image File(*.tif)|*.tif" : "标定文件(*.*)|*.*|CSV文件(*.csv)|*.csv|XLS文件(*.xls)|*.xls");
            if (dig_openImage.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(dig_openImage.FileName, Encoding.Default);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] data = Regex.Split(lines[i], ",");
                    for (int j = 0; j < data.Length; j++)
                    {
                        Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[j].Value = data[j];
                        if (j == 3)            //只导入前四列
                            break;
                    }
                    if (i == (Frm_OneKeyEyeHandCalibTool.Instance.cbo_calibType.SelectedIndex == 0 ? 3 : 8))           //若标定类型为四点标定，则导入前四行，若为九点标定，则导入前九行
                        break;
                }
                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Loading Image successfully" : "标定文件导入成功", Color.Green);
            }
        }
        /// <summary>
        /// 多点拟合圆
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static PointF FitCenter(List<PointF> pts, double epsilon = 0.1)
        {
            try
            {
                double totalX = 0, totalY = 0;
                int setCount = 0;

                for (int i = 0; i < pts.Count; i++)
                {
                    for (int j = 1; j < pts.Count; j++)
                    {
                        for (int k = 2; k < pts.Count; k++)
                        {
                            double delta = (pts[k].X - pts[j].X) * (pts[j].Y - pts[i].Y) - (pts[j].X - pts[i].X) * (pts[k].Y - pts[j].Y);

                            if (Math.Abs(delta) > epsilon)
                            {
                                double ii = Math.Pow(pts[i].X, 2) + Math.Pow(pts[i].Y, 2);
                                double jj = Math.Pow(pts[j].X, 2) + Math.Pow(pts[j].Y, 2);
                                double kk = Math.Pow(pts[k].X, 2) + Math.Pow(pts[k].Y, 2);

                                double cx = ((pts[k].Y - pts[j].Y) * ii + (pts[i].Y - pts[k].Y) * jj + (pts[j].Y - pts[i].Y) * kk) / (2 * delta);
                                double cy = -((pts[k].X - pts[j].X) * ii + (pts[i].X - pts[k].X) * jj + (pts[j].X - pts[i].X) * kk) / (2 * delta);

                                totalX += cx;
                                totalY += cy;

                                setCount++;
                            }
                        }
                    }
                }

                if (setCount == 0)
                {
                    return PointF.Empty;
                }

                return new PointF((float)totalX / setCount, (float)totalY / setCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return PointF.Empty;
            }
        }
        /// <summary>
        /// 标定
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public HTuple Calibrate(bool showAndSave = false)
        {
            try
            {
                List<double> L_pixelRow = new List<double>();
                List<double> L_pixelCol = new List<double>();
                List<double> L_MechanicalX = new List<double>();
                List<double> L_MechanicalY = new List<double>();
                try
                {
                    for (int i = 0; i < Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows.Count; i++)
                    {
                        L_pixelRow.Add(Convert.ToDouble(Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[0].Value));
                        L_pixelCol.Add(Convert.ToDouble(Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[1].Value));
                        L_MechanicalX.Add(Convert.ToDouble(Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[2].Value));
                        L_MechanicalY.Add(Convert.ToDouble(Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[3].Value));
                    }
                }
                catch
                {
                    Frm_Output.Instance.OutputMsg("标定失败，标定数据异常（错误代码：12901）", Color.Red);
                    return new HTuple();
                }

                try
                {
                    HTuple pixelRow = new HTuple(L_pixelRow.ToArray());
                    HTuple pixelCol = new HTuple(L_pixelCol.ToArray());
                    HTuple mechanicalX = new HTuple(L_MechanicalX.ToArray());
                    HTuple mechanicalY = new HTuple(L_MechanicalY.ToArray());
                    HOperatorSet.VectorToHomMat2d(pixelRow, pixelCol, mechanicalX, mechanicalY, out homMat2D);
                }
                catch
                {
                    Frm_Output.Instance.OutputMsg("标定失败，标定数据异常，无法确定仿射变换关系（错误代码：12902）", Color.Red);
                }
                HOperatorSet.HomMat2dToAffinePar(homMat2D, out _scanX, out _scanY, out _rotation, out _theta, out _translateX, out _translateY);

                if (showAndSave)
                {
                    Frm_OneKeyEyeHandCalibTool.Instance.tbx_scaleX.Text = (Convert.ToDouble(ScanX.ToString())).ToString("0.00000");
                    Frm_OneKeyEyeHandCalibTool.Instance.tbx_scaleY.Text = (Convert.ToDouble(ScanY.ToString())).ToString("0.00000");
                    Frm_OneKeyEyeHandCalibTool.Instance.tbx_rotation.Text = (Convert.ToDouble(Rotation.ToString())).ToString("0.00000");
                    Frm_OneKeyEyeHandCalibTool.Instance.tbx_theta.Text = (Convert.ToDouble(Theta.ToString())).ToString("0.00000");
                    Frm_OneKeyEyeHandCalibTool.Instance.tbx_translateX.Text = (Convert.ToDouble(TranslateX.ToString())).ToString("0.00000");
                    Frm_OneKeyEyeHandCalibTool.Instance.tbx_translateY.Text = (Convert.ToDouble(TranslateY.ToString())).ToString("0.00000");

                    //保存标定数据
                    L_calibData.Clear();
                    for (int i = 0; i < Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows.Count; i++)
                    {
                        List<double> list = new List<double>();
                        for (int j = 0; j < Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells.Count; j++)
                        {
                            list.Add(Math.Round(Convert.ToDouble(Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[j].Value), 3));
                        }
                        L_calibData.Add(list);
                    }
                }
                return homMat2D;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return new HTuple();
            }
        }
        private bool Connect(ref  Socket socket)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress IP;
                try
                {
                    IP = IPAddress.Parse(ip);
                }
                catch
                {
                    return false;
                }
                IPEndPoint point = new IPEndPoint(IP, port);
                try
                {
                    socket.Connect(point);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex );
                return false;
            }
        }
        /// <summary>
        /// 接收一次消息
        /// </summary>
        internal string RecieveOnce(Socket socket)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int length = 0;
                try
                {
                    length = socket.Receive(buffer);
                }
                catch { }
                string result = Encoding.Default.GetString(buffer, 0, length);
                if (length > 0)
                {
                    return result;
                }
                else
                {
                    Frm_Output.Instance.OutputMsg("连接已断开", Color.Red);
                    return string.Empty;
                }

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg">要发送的信息</param>
        internal void Send(Socket socket, string msg)
        {
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(msg);
                socket.Send(buffer);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        public void DisConnect(Socket socket)
        {
            if (socket.Connected)
            {
                socket.Disconnect(false);
                socket.Close();
            }
        }
        /// <summary>
        /// 一键标定
        /// </summary>
        /// <param name="camIndex">相机编号，如果有多个相机都要和一个机器人做标定，这个时候就需要传入相机编号以区分</param>
        /// <returns></returns>
        internal bool OneKeyCalibrate(int camIndex)
        {
            Socket socket = null;
            try
            {
                Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = false;
                Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Green;
                Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "标定中......";
                Frm_Output.Instance.OutputMsg("开始自动标定", Color.Green);

                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows.Clear();
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows.Add(4);

                //首先连接机器人
                if (!Connect( ref  socket))
                {
                    Frm_Output.Instance.OutputMsg("机器人连接失败，标定失败，可能原因：1 机器人未运行标定程序", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    return false;
                }
                Frm_Output.Instance.OutputMsg("机器人连接成功", Color.Green);

                //等待机器人到达安全位，然后人工放置标定片
                RecieveOnce(socket );
                Frm_MessageBox.Instance.MessageBoxShow("请先确保机械手标定的可能动作范围内无干扰物，然后将标定板按指定要求吸在吸嘴的指定位置上，然后点击确定，确定后将开始自动标定");

                Dictionary<PointF, string> D_point = new Dictionary<PointF, string>();

                //控制机器人前往标定点1，并返回标定点1坐标位置
                Frm_Output.Instance.OutputMsg("等待机器人前往标定点1......", Color.Green);
                Send(socket ,"Cam" + camIndex + "GoC1\r\n");
                string value = RecieveOnce(socket );
                if (value == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，机器人返回的位置坐标为空，可能是机器人链接已断开", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket ,"Quit\r\n");
                    DisConnect(socket );
                    return false;
                }
                string mechanicalPosX = Regex.Split(value, ";")[0];
                string mechanicalPosY = Regex.Split(value, ";")[1];

                //获取像素坐标
                XYU curPos = new XYU();
                Job job = Job.FindJobByName(calibJobName);
                job.Run();
                if (job.jobRunStatu != JobRunStatu.Succeed)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，特征点未识别到，请检查", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket ,"Quit\r\n");
                    DisConnect(socket );
                    return false;
                }
                Object value1 = Job.FindJobByName(calibJobName).GetToolOutputItemValue(calibItemName);
                Type type = value1.GetType();
                if (type.Name == "XYU")
                {
                    curPos.Point.X = (float)((XYU)value1).Point.X;
                    curPos.Point.Y = (float)((XYU)value1).Point.Y;
                }
                else
                {
                    curPos.Point.X = (float)((XY)value1).X;
                    curPos.Point.Y = (float)((XY)value1).Y;
                }

                //添加到点集合，并显示点
                D_point.Add(new PointF((float)curPos.Point.X, (float)curPos.Point.Y), "C1");
                foreach (KeyValuePair<PointF, string> item in D_point)
                {
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("green"));
                    HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple(item.Key.X), new HTuple(item.Key.Y), new HTuple(30), new HTuple(0));
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("blue"));
                    Show_Text( item.Value, item.Key.X + 10, item.Key.Y + 10);
                }

                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[0].Cells[0].Value = curPos.Point.X;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[0].Cells[1].Value = curPos.Point.Y;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[0].Cells[2].Value = mechanicalPosX;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[0].Cells[3].Value = mechanicalPosY;
                Frm_Output.Instance.OutputMsg("机器人到达标定点1，当前像素坐标为：" + curPos.Point.X + ";" + curPos.Point.Y + "机械坐标为:" + mechanicalPosX + ";" + mechanicalPosY, Color.Green);

                //控制机器人前往标定点2，并返回标定点2坐标位置
                Frm_Output.Instance.OutputMsg("等待机器人前往标定点2......", Color.Green);
                Send(socket,"Cam" + camIndex + "GoC2\r\n");
                value = RecieveOnce(socket);
                if (value == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，机器人返回的位置坐标为空，可能是机器人链接已断开", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                mechanicalPosX = Regex.Split(value, ";")[0];
                mechanicalPosY = Regex.Split(value, ";")[1];

                curPos = new XYU();
                job = Job.FindJobByName(calibJobName);
                job.Run();
                if (job.jobRunStatu != JobRunStatu.Succeed)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，特征点未识别到，请检查", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                value1 = Job.FindJobByName(calibJobName).GetToolOutputItemValue(calibItemName);
                type = value1.GetType();
                if (type.Name == "XYU")
                {
                    curPos.Point.X = (float)((XYU)value1).Point.X;
                    curPos.Point.Y = (float)((XYU)value1).Point.Y;
                }
                else
                {
                    curPos.Point.X = (float)((XY)value1).X;
                    curPos.Point.Y = (float)((XY)value1).Y;
                }

                D_point.Add(new PointF((float)curPos.Point.X, (float)curPos.Point.Y), "C2");
                foreach (KeyValuePair<PointF, string> item in D_point)
                {
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("green"));
                    HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple(item.Key.X), new HTuple(item.Key.Y), new HTuple(30), new HTuple(0));
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("blue"));
                    Show_Text( item.Value, item.Key.X + 10, item.Key.Y + 10);
                }

                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[1].Cells[0].Value = curPos.Point.X;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[1].Cells[1].Value = curPos.Point.Y;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[1].Cells[2].Value = mechanicalPosX;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[1].Cells[3].Value = mechanicalPosY;
                Frm_Output.Instance.OutputMsg("机器人到达2号点，当前像素坐标为：" + curPos.Point.X + ";" + curPos.Point.Y + "机械坐标为:" + mechanicalPosX + ";" + mechanicalPosY, Color.Green);

                //控制机器人前往标定点3，并返回标定点3坐标位置
                Frm_Output.Instance.OutputMsg("等待机器人前往标定点3......", Color.Green);
                Send(socket,"Cam" + camIndex + "GoC3\r\n");
                value = RecieveOnce(socket);
                if (value == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，机器人返回的位置坐标为空，可能是机器人链接已断开", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                mechanicalPosX = Regex.Split(value, ";")[0];
                mechanicalPosY = Regex.Split(value, ";")[1];

                curPos = new XYU();
                job = Job.FindJobByName(calibJobName);
                job.Run();
                if (job.jobRunStatu != JobRunStatu.Succeed)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，特征点未识别到，请检查", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                value1 = Job.FindJobByName(calibJobName).GetToolOutputItemValue(calibItemName);
                type = value1.GetType();
                if (type.Name == "XYU")
                {
                    curPos.Point.X = (float)((XYU)value1).Point.X;
                    curPos.Point.Y = (float)((XYU)value1).Point.Y;
                }
                else
                {
                    curPos.Point.X = (float)((XY)value1).X;
                    curPos.Point.Y = (float)((XY)value1).Y;
                }
                D_point.Add(new PointF((float)curPos.Point.X, (float)curPos.Point.Y), "C3");
                foreach (KeyValuePair<PointF, string> item in D_point)
                {
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("green"));
                    HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple(item.Key.X), new HTuple(item.Key.Y), new HTuple(30), new HTuple(0));
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("blue"));
                    Show_Text( item.Value, item.Key.X + 10, item.Key.Y + 10);
                }

                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[2].Cells[0].Value = curPos.Point.X;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[2].Cells[1].Value = curPos.Point.Y;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[2].Cells[2].Value = mechanicalPosX;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[2].Cells[3].Value = mechanicalPosY;
                Frm_Output.Instance.OutputMsg("机器人到达标定点3，当前像素坐标为：" + curPos.Point.Y + ";" + curPos.Point.Y + "机械坐标为:" + mechanicalPosX + ";" + mechanicalPosY, Color.Green);

                //控制机器人前往标定点4，并返回标定点4坐标位置
                Frm_Output.Instance.OutputMsg("等待机器人前往标定点4......", Color.Green);
                Send(socket,"Cam" + camIndex + "GoC4\r\n");
                value = RecieveOnce(socket);
                if (value == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，机器人返回的位置坐标为空，可能是机器人链接已断开", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                mechanicalPosX = Regex.Split(value, ";")[0];
                mechanicalPosY = Regex.Split(value, ";")[1];

                curPos = new XYU();
                job = Job.FindJobByName(calibJobName);
                job.Run();
                if (job.jobRunStatu != JobRunStatu.Succeed)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，特征点未识别到，请检查", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                value1 = Job.FindJobByName(calibJobName).GetToolOutputItemValue(calibItemName);
                type = value1.GetType();
                if (type.Name == "XYU")
                {
                    curPos.Point.X = (float)((XYU)value1).Point.X;
                    curPos.Point.Y = (float)((XYU)value1).Point.Y;
                }
                else
                {
                    curPos.Point.X = (float)((XY)value1).X;
                    curPos.Point.Y = (float)((XY)value1).Y;
                }

                D_point.Add(new PointF((float)curPos.Point.X, (float)curPos.Point.Y), "C4");
                foreach (KeyValuePair<PointF, string> item in D_point)
                {
                    HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple(item.Key.X), new HTuple(item.Key.Y), new HTuple(30), new HTuple(0));
                    Show_Text( item.Value, item.Key.X + 10, item.Key.Y + 10);
                }

                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[3].Cells[0].Value = curPos.Point.X;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[3].Cells[1].Value = curPos.Point.Y;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[3].Cells[2].Value = mechanicalPosX;
                Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[3].Cells[3].Value = mechanicalPosY;
                Frm_Output.Instance.OutputMsg("机器人到达标定点4，当前像素坐标为：" + curPos.Point.X + ";" + curPos.Point.Y + "机械坐标为:" + mechanicalPosX + ";" + mechanicalPosY, Color.Green);

                //先在Tool0下标定一下，此时是以Tool0的工具中心标定的
                Frm_Output.Instance.OutputMsg("开始Tool0下的标定......", Color.Green);
                Calibrate();
                Application.DoEvents();

                //三点定圆，先前往到第二个点
                Frm_Output.Instance.OutputMsg("Tool0下标定完成，开始创建工具坐标", Color.Green);
                Frm_Output.Instance.OutputMsg("等待机器人前往创建工具2号点......", Color.Green);
                Send(socket,"Cam" + camIndex + "GoCT2\r\n");
                value = RecieveOnce(socket);

                XYU pos2 = new XYU();
                Job.FindJobByName(calibJobName).Run();
                value1 = Job.FindJobByName(calibJobName).GetToolOutputItemValue(calibItemName);
                type = value1.GetType();
                if (type.Name == "XYU")
                {
                    pos2.Point.X = (float)((XYU)value1).Point.X;
                    pos2.Point.Y = (float)((XYU)value1).Point.Y;
                }
                else
                {
                    pos2.Point.X = (float)((XY)value1).X;
                    pos2.Point.Y = (float)((XY)value1).Y;
                }

                D_point.Add(new PointF((float)pos2.Point.X, (float)pos2.Point.Y), "CT2");
                foreach (KeyValuePair<PointF, string> item in D_point)
                {
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("green"));
                    HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple(item.Key.X), new HTuple(item.Key.Y), new HTuple(30), new HTuple(0));
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("blue"));
                    Show_Text( item.Value, item.Key.X + 10, item.Key.Y + 10);
                }

                Frm_Output.Instance.OutputMsg("机器人到达创建工具2号点，当前像素坐标为：" + pos2.Point.X + ";" + pos2.Point.Y, Color.Green);

                //前往第三个点
                Frm_Output.Instance.OutputMsg("等待机器人前往创建工具3号点......", Color.Green);
                Send(socket,"Cam" + camIndex + "GoCT3\r\n");
                value = RecieveOnce(socket);

                XYU pos3 = new XYU();
                job = Job.FindJobByName(calibJobName);
                job.Run();
                if (job.jobRunStatu != JobRunStatu.Succeed)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，特征点未识别到，请检查", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                value1 = Job.FindJobByName(calibJobName).GetToolOutputItemValue(calibItemName);
                type = value1.GetType();
                if (type.Name == "XYU")
                {
                    pos3.Point.X = (float)((XYU)value1).Point.X;
                    pos3.Point.Y = (float)((XYU)value1).Point.Y;
                }
                else
                {
                    pos3.Point.X = (float)((XY)value1).X;
                    pos3.Point.Y = (float)((XY)value1).Y;
                }

                D_point.Add(new PointF((float)pos3.Point.X, (float)pos3.Point.Y), "CT3");
                foreach (KeyValuePair<PointF, string> item in D_point)
                {
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("green"));
                    HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple(item.Key.X), new HTuple(item.Key.Y), new HTuple(30), new HTuple(0));
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("blue"));
                    Show_Text( item.Value, item.Key.X + 10, item.Key.Y + 10);
                }

                Frm_Output.Instance.OutputMsg("机器人到达创建工具3号点，当前像素坐标为：" + pos3.Point.X + ";" + pos3.Point.Y, Color.Green);

                //前往第一个点，此时需要返回机器人的当前坐标
                Frm_Output.Instance.OutputMsg("等待机器人前往创建工具1号点......", Color.Green);
                Send(socket,"Cam" + camIndex + "GoCT1\r\n");
                value = RecieveOnce(socket);
                if (value == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，机器人返回的位置坐标为空，可能是机器人链接已断开", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }

                XYU pos1 = new XYU();
                job = Job.FindJobByName(calibJobName);
                job.Run();
                if (job.jobRunStatu != JobRunStatu.Succeed)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，特征点未识别到，请检查", Color.Red);
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                value1 = Job.FindJobByName(calibJobName).GetToolOutputItemValue(calibItemName);
                type = value1.GetType();
                if (type.Name == "XYU")
                {
                    pos1.Point.X = (float)((XYU)value1).Point.X;
                    pos1.Point.Y = (float)((XYU)value1).Point.Y;
                }
                else
                {
                    pos1.Point.X = (float)((XY)value1).X;
                    pos1.Point.Y = (float)((XY)value1).Y;
                }

                D_point.Add(new PointF((float)pos1.Point.X, (float)pos1.Point.Y), "CT1");
                foreach (KeyValuePair<PointF, string> item in D_point)
                {
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("green"));
                    HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple(item.Key.X), new HTuple(item.Key.Y), new HTuple(30), new HTuple(0));
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("blue"));
                    Show_Text( item.Value, item.Key.X + 10, item.Key.Y + 10);
                }

                mechanicalPosX = Regex.Split(value, ";")[0];
                mechanicalPosY = Regex.Split(value, ";")[1];
                Frm_Output.Instance.OutputMsg("机器人到达标创建工具1号点，当前像素坐标为：" + pos1.Point.X + ";" + pos1.Point.Y + "机械坐标为:" + mechanicalPosX + ";" + mechanicalPosY, Color.Green);

                //三点求圆心，并将圆心经Tool0下的标定关系转化到机器人坐标系中
                List<PointF> list = new List<PointF>();
                PointF p1 = new PointF((float)pos1.Point.X, (float)pos1.Point.Y);
                PointF p2 = new PointF((float)pos2.Point.X, (float)pos2.Point.Y);
                PointF p3 = new PointF((float)pos3.Point.X, (float)pos3.Point.Y);
                list.Add(p1);
                list.Add(p2);
                list.Add(p3);
                PointF center = FitCenter(list);
                HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, center.X, center.Y, 20, 0);
                HTuple row1AfterTrans, col1AfterTrans;
                HOperatorSet.AffineTransPoint2d(homMat2D, (HTuple)center.X, (HTuple)center.Y, out row1AfterTrans, out col1AfterTrans);
                Frm_Output.Instance.OutputMsg("Tool0工具中心经标定关系转化后的坐标为：" + row1AfterTrans + ";" + col1AfterTrans, Color.Green);

                //计算工具1和工具0的毫米偏差，并把最初标定时的Tool0下的四个点加上这个毫米偏差，使其变为Tool1下的坐标，Tool1的工具中心就是我们选取的特征点那个地方
                double spanX = Convert.ToDouble(mechanicalPosX) - row1AfterTrans.D;
                double spanY = Convert.ToDouble(mechanicalPosY) - col1AfterTrans.D;
                for (int i = 0; i < 4; i++)
                {
                    Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[2].Value = Math.Round(Convert.ToDouble(Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[2].Value) + spanX, 3);
                    Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[3].Value = Math.Round(Convert.ToDouble(Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[3].Value) + spanY, 3);
                }

                //现在在Tool1下重新标定就OK啦
                Calibrate();

                //触发机器人创建工具1
                HOperatorSet.AffineTransPoint2d(homMat2D, (HTuple)pos1.Point.X, (HTuple)pos1.Point.Y, out row1AfterTrans, out col1AfterTrans);
                Send(socket,"CreateTool" + ";" + row1AfterTrans + ";" + col1AfterTrans + "\r\n");
                RecieveOnce(socket);
                Frm_Output.Instance.OutputMsg("机器人工具1创建完成", Color.Green);

                //做精度测试
                Send(socket,"Cam" + camIndex + "Test\r\n");
                value = RecieveOnce(socket);
                if (value == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，机器人返回的位置坐标为空，可能是机器人链接已断开", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                mechanicalPosX = Regex.Split(value, ";")[0];
                mechanicalPosY = Regex.Split(value, ";")[1];

                curPos = new XYU();
                job = Job.FindJobByName(calibJobName);
                job.Run();
                if (job.jobRunStatu != JobRunStatu.Succeed)
                {
                    Frm_Output.Instance.OutputMsg("标定失败，特征点未识别到，请检查", Color.Red);
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                    Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                    Send(socket,"Quit\r\n");
                    DisConnect(socket);
                    return false;
                }
                value1 = Job.FindJobByName(calibJobName).GetToolOutputItemValue(calibItemName);
                type = value1.GetType();
                if (type.Name == "XYU")
                {
                    curPos.Point.X = (float)((XYU)value1).Point.X;
                    curPos.Point.Y = (float)((XYU)value1).Point.Y;
                }
                else
                {
                    curPos.Point.X = (float)((XY)value1).X;
                    curPos.Point.Y = (float)((XY)value1).Y;
                }

                D_point.Add(new PointF((float)curPos.Point.X, (float)curPos.Point.Y), "TP");
                foreach (KeyValuePair<PointF, string> item in D_point)
                {
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("green"));
                    HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple(item.Key.X), new HTuple(item.Key.Y), new HTuple(30), new HTuple(0));
                    HOperatorSet.SetColor(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, new HTuple("blue"));
                    Show_Text( item.Value, item.Key.X + 10, item.Key.Y + 10);
                }

                HOperatorSet.AffineTransPoint2d(homMat2D, (HTuple)curPos.Point.X, (HTuple)curPos.Point.Y, out row1AfterTrans, out col1AfterTrans);
                spanX = Math.Abs(row1AfterTrans.D - Convert.ToDouble(mechanicalPosX));
                spanY = Math.Abs(col1AfterTrans.D - Convert.ToDouble(mechanicalPosY));
                Frm_Output.Instance.OutputMsg("标定精度测试结果：X偏差：" + Math.Round(spanX, 3) + "mm,Y偏差：" + Math.Round(spanY, 3) + "mm", Color.Green);
                if (Math.Abs(spanX) < 0.1 && Math.Abs(spanY) < 0.1)
                    Frm_Output.Instance.OutputMsg("标定精度较好", Color.Green);
                else
                    Frm_Output.Instance.OutputMsg("标定精度较差", Color.Red);

                //保存标定数据
                L_calibData.Clear();
                for (int i = 0; i < Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows.Count; i++)
                {
                    List<double> list1 = new List<double>();
                    for (int j = 0; j < Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells.Count; j++)
                    {
                        list1.Add(Convert.ToDouble(Frm_OneKeyEyeHandCalibTool.Instance.dgv_calibData.Rows[i].Cells[j].Value));
                    }
                    L_calibData.Add(list1);
                }
                DisConnect(socket );
                Frm_Output.Instance.OutputMsg("自动标定完成", Color.Green);
                Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Enabled = true;
                Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.ForeColor = Color.Black;
                Frm_OneKeyEyeHandCalibTool.Instance.btn_oneKeyCalibrate.Text = "一键标定";
                Send(socket,"Quit\r\n");
                DisConnect(socket);
                return false;
            }
        }
        /// <summary>
        /// 工具运行
        /// </summary>
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
                    if (inputImage == null && inputPose == null)
                    {
                        toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Assign_Input_Source : ToolRunStatu.输入项未链接源);
                        return;
                    }

                    //对图像进行仿射变换
                    if (inputImage != null)
                    {
                        HOperatorSet.AffineTransImage(inputImage, out outputImage, homMat2D, (HTuple)"nearest_neighbor", (HTuple)"false");
                        if (updateImage)
                            HOperatorSet.DispObj(outputImage, GetImageWindowControl().hwc_imageWindow.HWindowHalconID);
                    }

                    //对输入位置进行放射变换
                    if (inputPose != null)
                    {
                        HTuple rowAfterTrans;
                        HTuple colAfterTrans;
                        HTuple angleAfterTrans;
                        HOperatorSet.AffineTransPoint2d(homMat2D, inputPose.Point.X, inputPose.Point.Y, out rowAfterTrans, out colAfterTrans);
                        angleAfterTrans = inputPose.U;
                        outputPose.Point.X = rowAfterTrans;
                        outputPose.Point.Y = colAfterTrans;
                        outputPose.U = angleAfterTrans;
                    }
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
