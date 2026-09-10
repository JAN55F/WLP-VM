using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_EyeHandCalibTool : Frm_FormBase
    {
        internal Frm_EyeHandCalibTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_EyeHandCalibTool _instance;
        internal static Frm_EyeHandCalibTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_EyeHandCalibTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static EyeHandCalibTool eyeHandCalibTool = new EyeHandCalibTool();



        private void btn_calibrate_Click(object sender, EventArgs e)
        {

        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            eyeHandCalibTool.Run(true, true, toolName);
        }
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {

        }
        private void cbx_jobList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tsb_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }
        private void btn_readCaliData_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dig_openImage = new System.Windows.Forms.OpenFileDialog();
                dig_openImage.FileName = string.Empty;
                dig_openImage.Title = (Project.Instance.configuration.language == Language.English ? "Please select a form file" : "请选择标定文件");
                dig_openImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openImage.Filter = (Project.Instance.configuration.language == Language.English ? "Image File(*.*)|*.*|Image File(*.png)|*.txt|Image File(*.jpg)|*.jpg|Image File(*.bmp)|*.bmp|Image File(*.tif)|*.tif" : "标定文件(*.*)|*.*|CSV文件(*.csv)|*.csv|XLS文件(*.xls)|*.xls");
                if (dig_openImage.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = File.ReadAllLines(dig_openImage.FileName, Encoding.Default);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] data = Regex.Split(lines[i], ",");
                        for (int j = 0; j < 4; j++)
                        {
                            dgv_calibrateData.Rows[i].Cells[j].Value = data[j];
                        }
                        //////if (i == (cbx_calibType.SelectedIndex == 0 ? 3 : 8))           //若标定类型为四点标定，则导入前四行，若为九点标定，则导入前九行
                        //////    break;
                    }
                    Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Loading Image successfully" : "标定文件导入成功", Color.Green);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_writeCalibData_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                dig_saveImage.FileName = "标定数据 " + DateTime.Now.ToString("yyyy_MM_dd");
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image saving path" : "请选择导出路径";
                dig_saveImage.Filter = "CSV文件(*.csv)|*.csv|XLS文件(*.xls)|*.xls";
                dig_saveImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {
                    File.Create(dig_saveImage.FileName).Close();
                    string data = string.Empty;
                    for (int i = 0; i < dgv_calibrateData.Rows.Count; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            data += dgv_calibrateData.Rows[i].Cells[j].Value;
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
        private void ckb_toolEnable_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void btn_firstPoint_Click(object sender, EventArgs e)
        {

        }
        private void GetPixelXY1(int index)
        {
            try
            {
                if (cbx_jobList.TextStr == string.Empty || cbx_outputItemList.TextStr == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.TextStr;
                string toolName = Regex.Split(cbx_outputItemList.TextStr, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.TextStr, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VMPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dataGridView2.Rows[index].Cells[0].Value = index + 1;
                dataGridView2.Rows[index].Cells[1].Value = row;
                dataGridView2.Rows[index].Cells[2].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void GetPixelXY2(int index)
        {
            try
            {
                if (cbx_jobList.TextStr == string.Empty || cbx_outputItemList.TextStr == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.TextStr;
                string toolName = Regex.Split(cbx_outputItemList.TextStr, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.TextStr, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VMPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dataGridView3.Rows[index].Cells[0].Value = index + 1;
                dataGridView3.Rows[index].Cells[1].Value = row;
                dataGridView3.Rows[index].Cells[2].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_secondPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbx_jobList.TextStr == string.Empty || cbx_outputItemList.Text == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.TextStr;
                string toolName = Regex.Split(cbx_outputItemList.Text, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_calibrateData.Rows[1].Cells[0].Value = row;
                dgv_calibrateData.Rows[1].Cells[1].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_thirdPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbx_jobList.TextStr == string.Empty || cbx_outputItemList.Text == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.TextStr;
                string toolName = Regex.Split(cbx_outputItemList.Text, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_calibrateData.Rows[2].Cells[0].Value = row;
                dgv_calibrateData.Rows[2].Cells[1].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_fourthPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbx_jobList.TextStr == string.Empty || cbx_outputItemList.Text == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.TextStr;
                string toolName = Regex.Split(cbx_outputItemList.Text, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_calibrateData.Rows[3].Cells[0].Value = row;
                dgv_calibrateData.Rows[3].Cells[1].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_fifthPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbx_jobList.Text == string.Empty || cbx_outputItemList.Text == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.Text;
                string toolName = Regex.Split(cbx_outputItemList.Text, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_calibrateData.Rows[4].Cells[0].Value = row;
                dgv_calibrateData.Rows[4].Cells[1].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_sixthPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbx_jobList.Text == string.Empty || cbx_outputItemList.Text == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.Text;
                string toolName = Regex.Split(cbx_outputItemList.Text, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_calibrateData.Rows[5].Cells[0].Value = row;
                dgv_calibrateData.Rows[5].Cells[1].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_seventhPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbx_jobList.Text == string.Empty || cbx_outputItemList.Text == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.Text;
                string toolName = Regex.Split(cbx_outputItemList.Text, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_calibrateData.Rows[6].Cells[0].Value = row;
                dgv_calibrateData.Rows[6].Cells[1].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_eighthPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbx_jobList.Text == string.Empty || cbx_outputItemList.Text == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.Text;
                string toolName = Regex.Split(cbx_outputItemList.Text, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_calibrateData.Rows[7].Cells[0].Value = row;
                dgv_calibrateData.Rows[7].Cells[1].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_ninethPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbx_jobList.Text == string.Empty || cbx_outputItemList.Text == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.Text;
                string toolName = Regex.Split(cbx_outputItemList.Text, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1];
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_calibrateData.Rows[8].Cells[0].Value = row;
                dgv_calibrateData.Rows[8].Cells[1].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void cbx_outputItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            eyeHandCalibTool.calibItemName = cbx_outputItemList.Text;
        }
        private void Frm_EyeHandCalibTool_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.SelectedRows[0].Index;
            dataGridView1.Rows.RemoveAt(index);
            eyeHandCalibTool.D_photoPos.Remove(index);
            comboBox1.Items.Remove(index.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            eyeHandCalibTool.curPhotoPosIndex = comboBox1.SelectedIndex;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (Job.loadForm)
                    return;
                eyeHandCalibTool.D_photoPos.Clear();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    eyeHandCalibTool.D_photoPos.Add(Convert.ToInt16(dataGridView1.Rows[i].Cells[0].Value), new XY(Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value), Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value)));
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = !enable;
            if (!enable)
                pictureBox8.Image = Resources.开;
            else
                pictureBox8.Image = Resources.关;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            eyeHandCalibTool.ResetTool();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void btn_runTool_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            eyeHandCalibTool.Run(true, true,toolName );
            long elapsedTime = sw.ElapsedMilliseconds;
            label3.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());

            if (eyeHandCalibTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                label21.ForeColor = Color.Red;
            else
                label21.ForeColor = Color.Black;
            label21.Text = "状态：" + eyeHandCalibTool.toolRunStatu.ToString();
        }
        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonDown;
            Application.DoEvents();
        }

        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }
        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.按钮__2_;
            Application.DoEvents();
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            RefreshTitleButtonVisuals();

            if (dgv_calibrateData.SelectedRows.Count != 0)
                GetPixelXY(dgv_calibrateData.SelectedRows[0].Index);
            else
            {
                label21.ForeColor = Color.Red;
                label21.Text = "状态：失败，原因：获取像素坐标前请先选中行";
            }
        }


        private void GetPixelXY(int index)
        {
            try
            {
                if (cbx_jobList.TextStr == string.Empty || cbx_outputItemList.TextStr == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.TextStr;
                string toolName = Regex.Split(cbx_outputItemList.TextStr, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.TextStr, " . ")[1];
                Job.RunAndWait(jobName);

                //////HObject image;
                //////HOperatorSet.DumpWindowImage(out image, Job.FindJobByName(jobName).www);
                //////hWindow_Final1.HobjectToHimage(image);

                HObject image;
                HOperatorSet.DumpWindowImage(out image, GetImageWindowControl(jobName).hwc_imageWindow.HWindowHalconID);
                hWindow_Final1.HobjectToHimage(image);

                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                string type = point.ToString();
                XYU curPos = new XYU();
                switch (type)
                {
                    case "XYU":
                        curPos.Point.X = ((XYU)point).Point.X;
                        curPos.Point.Y = ((XYU)point).Point.Y;
                        break;
                    case "System.Collections.Generic.List`1[VMPro.Point]":
                        List<XY> L_xyu = point as List<XY>;
                        curPos.Point.X = L_xyu[0].X;
                        curPos.Point.Y = L_xyu[0].Y;
                        break;
                    default:
                        curPos.Point.X = ((XY)point).X;
                        curPos.Point.Y = ((XY)point).Y;
                        break;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_calibrateData.Rows[index].Cells[0].Value = index + 1;
                dgv_calibrateData.Rows[index].Cells[1].Value = row;
                dgv_calibrateData.Rows[index].Cells[2].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

            try
            {
                this.TopMost = true;
                RefreshTitleButtonVisuals();


                if (dataGridView2.SelectedRows.Count != 0)
                {
                    if (dataGridView2.SelectedRows[0].Index == dataGridView2.Rows.Count - 1)
                    {
                        int index = dataGridView2.Rows.Add();
                        dataGridView2.Rows[index].Selected = true;

                    }
                    Application.DoEvents();

                    GetPixelXY1(dataGridView2.SelectedRows[0].Index);
                    Application.DoEvents();

                    dataGridView2.Rows[dataGridView2.SelectedRows[0].Index + 1].Selected = true;


                }
                else
                {
                    label21.ForeColor = Color.Red;
                    label21.Text = "状态：失败，原因：获取像素坐标前请先选中行";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            dgv_calibrateData.Rows.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            eyeHandCalibTool.Calibrate();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                eyeHandCalibTool.L_calibCheckData.Clear();
                for (int i = 0; i < dataGridView3.Rows.Count - 1; i++)
                {
                    List<double> list = new List<double>();
                    for (int j = 0; j < 3; j++)
                    {
                        list.Add(Math.Round(Convert.ToDouble(dataGridView3.Rows[i].Cells[j].Value), 3));
                    }
                    eyeHandCalibTool.L_calibCheckData.Add(list);
                }

                eyeHandCalibTool.checkPoint.X = Convert.ToDouble(textBox2.Text.Trim());
                eyeHandCalibTool.checkPoint.Y = Convert.ToDouble(textBox1.Text.Trim());
                eyeHandCalibTool.calibResult = "标定完成，精度较高";
                label28.Text = eyeHandCalibTool.calibResult;
                label28.ForeColor = Color.Green;

                eyeHandCalibTool.calibOffset.X = Convert.ToDouble(textBox4.Text.Trim());
                eyeHandCalibTool.calibOffset.Y = Convert.ToDouble(textBox3.Text.Trim());
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            RefreshTitleButtonVisuals();

            if (dataGridView3.SelectedRows.Count != 0)
            {
                if (dataGridView3.SelectedRows[0].Index == dataGridView3.Rows.Count - 1)
                {
                    int index = dataGridView3.Rows.Add();
                    dataGridView3.Rows[index].Selected = true;

                }
                Application.DoEvents();

                GetPixelXY2(dataGridView3.SelectedRows[0].Index);

                dataGridView3.Rows[dataGridView3.SelectedRows[0].Index + 1].Selected = true;



            }
            else
            {
                label21.ForeColor = Color.Red;
                label21.Text = "状态：失败，原因：获取像素坐标前请先选中行";
            }
        }

        private void 显示信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.barVisible_strip_CheckedChanged(sender, e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            eyeHandCalibTool.multPhoto = this.checkBox1.Checked;
            pictureBox6.Image = (checkBox1.Checked ? Resources.复选框 : Resources.去复选框);
            if (eyeHandCalibTool.multPhoto)
                Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Add(Frm_EyeHandCalibTool.Instance.tabPage7);
            else
                Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Remove(Frm_EyeHandCalibTool.Instance.tabPage7);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (eyeHandCalibTool.multPhoto)
            {
                checkBox1.Checked = false;
                eyeHandCalibTool.multPhoto = false;
                pictureBox6.Image = (eyeHandCalibTool.multPhoto ? Resources.复选框 : Resources.去复选框);
            }
            else
            {
                checkBox1.Checked = true;
                eyeHandCalibTool.multPhoto = true;
                pictureBox6.Image = (eyeHandCalibTool.multPhoto ? Resources.复选框 : Resources.去复选框);
            }

            if (eyeHandCalibTool.multPhoto && !Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Contains(Frm_EyeHandCalibTool.Instance.tabPage7))
                Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Add(Frm_EyeHandCalibTool.Instance.tabPage7);
            else if (!eyeHandCalibTool.multPhoto && Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Contains(Frm_EyeHandCalibTool.Instance.tabPage7))
                Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Remove(Frm_EyeHandCalibTool.Instance.tabPage7);
        }

        private void btn_writeCalibData_Click_1(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = index.ToString();
                dataGridView1.Rows[index].Cells[1].Value = "0";
                dataGridView1.Rows[index].Cells[2].Value = "0";
                //eyeHandCalibTool.D_photoPos.Add(index, new Point(0, 0));
                comboBox1.Items.Add(index.ToString());
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            eyeHandCalibTool.calibRotateCenter = this.checkBox2.Checked;
            pictureBox2.Image = (checkBox2.Checked ? Resources.复选框 : Resources.去复选框);
            if (eyeHandCalibTool.calibRotateCenter)
            {
                Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Insert(2, Frm_EyeHandCalibTool.Instance.tabPage5);
                tabPage6.Text = "第四步";
                tabPage8.Text = "第五步";
                tabPage7.Text = "第六步";
            }
            else
            {
                Frm_EyeHandCalibTool.Instance.tabControl2.TabPages.Remove(Frm_EyeHandCalibTool.Instance.tabPage5);
                tabPage6.Text = "第三步";
                tabPage8.Text = "第四步";
                tabPage7.Text = "第五步";
            }
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            checkBox2.Checked = !checkBox2.Checked;
            pictureBox2.Image = (checkBox2.Checked ? Resources.复选框 : Resources.去复选框);

        }

        private void dgv_calibrateData_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex % 2 != 0)
                dgv_calibrateData.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
        }

        private void cbx_fixedType_SelectedIndexChanged(object sender, EventArgs e)
        {
            eyeHandCalibTool.fixedType = (cbx_fixedType.SelectedIndex == 0 ? FixedType.OutsideHand : FixedType.OnHand);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            eyeHandCalibTool.dirctionOfU = comboBox2.Text;
        }

        private void cbx_jobList_SelectedIndexChanged()
        {
            cbx_outputItemList.Clear();
            Job job = Job.FindJobByName(cbx_jobList.TextStr);
            for (int j = 0; j < job.L_toolList.Count; j++)
            {
                for (int k = 0; k < job.L_toolList[j].output.Count; k++)
                {
                    DataType ioType = job.L_toolList[j].output[k].ioType;
                    if (ioType == DataType.XY || ioType == DataType.Pose)
                        cbx_outputItemList.Add(job.L_toolList[j].toolName + " . " + job.L_toolList[j].output[k].IOName);
                }
            }
            eyeHandCalibTool.calibJobName = cbx_jobList.TextStr;
        }

        private void cbx_outputItemList_SelectedIndexChanged()
        {
            eyeHandCalibTool.calibItemName = cbx_outputItemList.TextStr;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Job.FindJobByName(eyeHandCalibTool.calibJobName).L_toolList.Count; i++)
            {
                if (Job.FindJobByName(eyeHandCalibTool.calibJobName).L_toolList[i].toolType == ToolType.ImageAcq)
                {
                    ((AcqImageTool)Job.FindJobByName(eyeHandCalibTool.calibJobName).L_toolList[i].tool).PlayImage(true, hWindow_Final1);
                }
            }
        }

        private void cbx_fixedType_SelectedIndexChanged()
        {
            eyeHandCalibTool.fixedType = (FixedType)cbx_fixedType.SelectedIndex;
        }

        private void textBox6_ValueChanged(double value)
        {
            eyeHandCalibTool.rotateCenter.X = value;
        }

        private void textBox5_ValueChanged(double value)
        {
            eyeHandCalibTool.rotateCenter.Y = value;
        }

    }
}
