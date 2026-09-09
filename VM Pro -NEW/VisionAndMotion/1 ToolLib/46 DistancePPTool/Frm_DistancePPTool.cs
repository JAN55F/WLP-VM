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
    public partial class Frm_DistancePPTool : Frm_FormBase
    {
        public Frm_DistancePPTool()
        {
            InitializeComponent();
        }

    
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_DistancePPTool _instance;
        public static Frm_DistancePPTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_DistancePPTool();
                return _instance;
            }
        }
        
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static DistancePPTool distancePPTool = new DistancePPTool();


       
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            distancePPTool.Run(true, true, toolName);
            long elapsedTime = sw.ElapsedMilliseconds;


            if (distancePPTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            {
                label1.ForeColor = Color.Red;
                label2.Text = string.Format("耗时：0ms");
            }
            else
            {
                label1.ForeColor = Color.Black;
                label2.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());
            }
            label1.Text = "状态：" + distancePPTool.toolRunStatu.ToString();

            if ( distancePPTool.imageSourceMode == ImageSourceMode.FromDirectory )
            {
                 Frm_AcqImageTool.Instance.lbl_toolTip.Text = string.Format("状态：成功，当前图像：{0} ({1})", distancePPTool.currentImageName, distancePPTool.currentImageIndex + 1 + "/" + distancePPTool.L_imageFiles.Count);
            }

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

       

        private void 实时显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");

        }

     

     




        private void Frm_SDK_HIKVisionTool_Load_1(object sender, EventArgs e)
        {
            //hWindow_Final1.showStatusBar();
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

     

       
    }
}
