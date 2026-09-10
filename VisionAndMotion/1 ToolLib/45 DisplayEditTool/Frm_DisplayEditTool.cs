using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    public partial class Frm_DisplayEditTool : Frm_FormBase
    {
        public Frm_DisplayEditTool()
        {
            InitializeComponent();
            hWindow_Final1.hWindowControl.MouseUp += Hwindow_MouseUp;
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_DisplayEditTool _instance;
        public static Frm_DisplayEditTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_DisplayEditTool();
                return _instance;
            }
        }

        /// <summary>
        /// 工具对象
        /// </summary>
        internal static DisplayEditTool displayEditTool = new DisplayEditTool();



        private void tsb_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }


        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            displayEditTool.Run(true, true, toolName);
            long elapsedTime = sw.ElapsedMilliseconds;


            if (displayEditTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            {
                label1.ForeColor = Color.Red;
                label2.Text = string.Format("耗时：0ms");
            }
            else
            {
                label1.ForeColor = Color.Black;
                label2.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());
            }
            label1.Text = "状态：" + displayEditTool.toolRunStatu.ToString();


        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = !enable;
            if (!enable)
                pictureBox2.Image = Resources.开;
            else
                pictureBox2.Image = Resources.关;
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

        private void btn_runTool_Click(object sender, EventArgs e)
        {

        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();


            List<ToolIO> io = Job.FindJobByName(jobName).FindToolInfoByName(toolName).output;
            for (int j = 0; j < io.Count; j++)
            {
                string outputItem = io[j].IOName;

                string[] strs = Regex.Split(outputItem, " . ");
                ToolParBase result3 = displayEditTool.toolPar;
                object value = result3;
                for (int k = 0; k < strs.Length; k++)
                {
                    value = GetValue(value, strs[k]);
                }
                Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value = value;
                Job.FindJobByName(jobName).GetToolIONodeByNodeText(toolName, "-->" + outputItem).ToolTipText = Job.FindJobByName(jobName).FormatShowTip(value);
            }
        }
        private object GetValue(object obj, string name)
        {
            PropertyInfo[] dd = obj.GetType().GetProperties();
            foreach (PropertyInfo pi in obj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                string temp = Regex.Split(pi.ToString(), " ")[0];
                //if (temp == "HalconDotNet.HObject"
                //    || temp == "VisionAndMotionPro.XYU"
                //    )
                //{
                if (pi.Name == name)
                {


                    return pi.GetValue(obj, null);
                }
            }
            return new object();
        }






        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

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

        private void 适应图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.DispImageFit();
        }


        private void 显示信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.barVisible_strip_CheckedChanged(sender, e);
        }




        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();



        /// <summary>
        /// 注册haclon窗体的鼠标弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hwindow_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int index;

                List<double> data;
                ViewWindow.Model.ROI roi = hWindow_Final1.viewWindow.smallestActiveROI(out data, out index);

                if (index > -1)
                {
                    string name = roi.GetType().Name;
                    this.regions[index] = roi;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void Frm_SDK_HIKVisionTool_Load_1(object sender, EventArgs e)
        {
            //hWindow_Final1.showStatusBar();
        }


        private void 全屏显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (全屏显示ToolStripMenuItem.Checked)
            {
                全屏显示ToolStripMenuItem.Text = "退出全屏";
                hWindow_Final1.Parent = this;
                panel3.Visible = false;
                hWindow_Final1.Dock = DockStyle.Fill;
                hWindow_Final1.m_CtrlHStatusLabelCtrl.BackColor = Color.White;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                全屏显示ToolStripMenuItem.Text = "全屏";
                panel3.Visible = true;
                hWindow_Final1.Parent = tableLayoutPanel2;
                hWindow_Final1.Dock = DockStyle.Fill;
                this.WindowState = FormWindowState.Normal;
            }
        }

        public static void Delay(double t)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Restart();
            while (stopWatch.Elapsed.TotalMilliseconds < t)
            {
                Application.DoEvents();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Frm_SDKInfo.Instance.Show();
        }



        private void button4_Click(object sender, EventArgs e)
        {
            Frm_IOConfig.result1 = displayEditTool.toolPar;
            Frm_IOConfig.Instance.toolInfoForEdit = null;
            Frm_IOConfig.Instance.jobName = this.jobName;
            Frm_IOConfig.Instance.ShowDialog();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayEditTool.color = comboBox1.Text;
        }

        private void numericUpDown2_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            displayEditTool.lineWidth = (int)numericUpDown2.Value;
        }
    }
}
