using HalconDotNet;
using Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMPro.Properties;
using HalconPaint;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace VMPro
{
    internal partial class Frm_AlignWithoutCalibRotateCenterTool : Frm_FormBase
    {
        internal Frm_AlignWithoutCalibRotateCenterTool()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_AlignWithoutCalibRotateCenterTool _instance;
        internal static Frm_AlignWithoutCalibRotateCenterTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_AlignWithoutCalibRotateCenterTool();
                return _instance;
            }
        }

        /// <summary>
        /// 工具对象
        /// </summary>
        internal static AlignWithoutCalibRotateCenterTool alignWithoutCalibRotateCenterTool = new AlignWithoutCalibRotateCenterTool();




        private void btn_displayStandardImage_Click(object sender, EventArgs e)
        {

        }

        private void tsb_runTool_Click(object sender, EventArgs e)
        {

            btn_runTool.Enabled = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            alignWithoutCalibRotateCenterTool.Run(true, true, toolName);
            long elapsedTime = sw.ElapsedMilliseconds;
            if (alignWithoutCalibRotateCenterTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            {
                Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                label3.Text = string.Format("耗时：0ms");
            }
            else
            {
                Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Black;
                label3.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());
            }
            label4.Text = "状态：" + alignWithoutCalibRotateCenterTool.toolRunStatu.ToString();
            btn_runTool.Enabled = true;
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

        private void button2_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            alignWithoutCalibRotateCenterTool.Run(true, true, toolName);
            long elapsedTime = sw.ElapsedMilliseconds;
            label3.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());

            if (alignWithoutCalibRotateCenterTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                label4.ForeColor = Color.Red;
            else
                label4.ForeColor = Color.Black;
            label4.Text = "状态：" + alignWithoutCalibRotateCenterTool.toolRunStatu.ToString();
            btn_runTool.Enabled = true;
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
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

        private void button5_Click(object sender, EventArgs e)
        {
            alignWithoutCalibRotateCenterTool.featurePosBeforeRotate.X = Convert.ToDouble(textBox9.Text.Trim());
            alignWithoutCalibRotateCenterTool.featurePosBeforeRotate.Y = Convert.ToDouble(textBox10.Text.Trim());
            //////alignWithoutCalibRotateCenterTool.featurePosBeforeRotate.X = alignWithoutCalibRotateCenterTool.inputPos[0].Point.X;
            //////alignWithoutCalibRotateCenterTool.featurePosBeforeRotate.Y = alignWithoutCalibRotateCenterTool.inputPos[0].Point.Y;
            //////textBox9.Text = alignWithoutCalibRotateCenterTool.featurePosBeforeRotate.X.ToString();
            //////textBox10.Text = alignWithoutCalibRotateCenterTool.featurePosBeforeRotate.Y.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            alignWithoutCalibRotateCenterTool.featurePosAfterRotate.X = Convert.ToDouble(textBox1.Text.Trim());
            alignWithoutCalibRotateCenterTool.featurePosAfterRotate.Y = Convert.ToDouble(textBox2.Text.Trim());
            //////alignWithoutCalibRotateCenterTool.featurePosAfterRotate.X = alignWithoutCalibRotateCenterTool.inputPos[0].Point.X;
            //////alignWithoutCalibRotateCenterTool.featurePosAfterRotate.Y = alignWithoutCalibRotateCenterTool.inputPos[0].Point.Y;
            //////textBox1.Text = alignWithoutCalibRotateCenterTool.featurePosAfterRotate.X.ToString();
            //////textBox2.Text = alignWithoutCalibRotateCenterTool.featurePosAfterRotate.Y.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            alignWithoutCalibRotateCenterTool.RotateAngle = Convert.ToDouble(textBox3.Text.Trim());

            //开始计算固定偏差
            double a = alignWithoutCalibRotateCenterTool.RotateAngle;
            XY temp = alignWithoutCalibRotateCenterTool.featurePosAfterRotate - alignWithoutCalibRotateCenterTool.featurePosBeforeRotate;
            double Mdx = temp.X;
            double Mdy = temp.Y;
            alignWithoutCalibRotateCenterTool.StDx = -0.5 * (Mdx * (Math.Cos(a) - 1) + Mdy * Math.Sin(a)) / (1 - Math.Cos(a));
            alignWithoutCalibRotateCenterTool.StDy = 0.5 * (Mdx * Math.Sin(a) - Mdy * (Math.Cos(a) - 1)) / (1 - Math.Cos(a));

            textBox4.Text = alignWithoutCalibRotateCenterTool.StDx.ToString();
            textBox5.Text = alignWithoutCalibRotateCenterTool.StDy.ToString();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            alignWithoutCalibRotateCenterTool.templateFeaturePos.Point.X = alignWithoutCalibRotateCenterTool.inputPos[0].Point.X;
            alignWithoutCalibRotateCenterTool.templateFeaturePos.Point.Y = alignWithoutCalibRotateCenterTool.inputPos[0].Point.Y;
            alignWithoutCalibRotateCenterTool.templateFeaturePos.U = alignWithoutCalibRotateCenterTool.inputPos[0].U;
            textBox18.Text = alignWithoutCalibRotateCenterTool.templateFeaturePos.Point.X.ToString();
            textBox17.Text = alignWithoutCalibRotateCenterTool.templateFeaturePos.Point.Y.ToString();
            textBox16.Text = alignWithoutCalibRotateCenterTool.templateFeaturePos.U.ToString();
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            alignWithoutCalibRotateCenterTool.templatePickPos.Point.X = Convert.ToDouble(textBox12.Text.Trim());
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            alignWithoutCalibRotateCenterTool.templatePickPos.Point.Y = Convert.ToDouble(textBox13.Text.Trim());
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            alignWithoutCalibRotateCenterTool.templatePickPos.U = Convert.ToDouble(textBox19.Text.Trim());
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }



    }
}
