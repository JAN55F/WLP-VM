using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VersionMethods;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_ApplyTransTool : Frm_FormBase 
    {
        public Frm_ApplyTransTool()
        {
            InitializeComponent();    
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_ApplyTransTool _instance;
        internal static Frm_ApplyTransTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ApplyTransTool();
                return _instance;
            }
        }
        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static QuoteTransTool applyTransTool = new QuoteTransTool();

     

        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
     
        }
        private void tsb_help_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project .Instance .configuration .language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void btn_moveCliperRegion_Click(object sender, EventArgs e)
        {
            //////findCircleTool.DrawExpectCircle(jobName);
        }
        private void btn_subExpectRingRadiusLength_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_ringRadiusLengthSpan.Text.Trim()); 
            //findCircleTool.ringRadiusLength  -= value;
            //tbx_ringRadiusLength.Text = findCircleTool.ringRadiusLength.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
        private void btn_addExpectCircleRingRadiusLength_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_ringRadiusLengthSpan.Text.Trim());
            //findCircleTool.ringRadiusLength += value;
            //tbx_ringRadiusLength.Text = findCircleTool.ringRadiusLength.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
        private void btn_subExpectCircleStartAngle_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_startAngleSpan.Text.Trim());
            //findCircleTool.startAngle  -= value;
            //tbx_startAngle.Text = findCircleTool.startAngle.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
        private void btn_addExpectCircleStartAngle_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_startAngleSpan.Text.Trim());
            //findCircleTool.startAngle += value;
            //tbx_startAngle.Text = findCircleTool.startAngle.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
        private void btn_subExpectCircleEndAngle_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_endAngleSpan.Text.Trim());
            //findCircleTool.endAngle  -= value;
            //tbx_endAngle.Text = findCircleTool.endAngle.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
        private void btn_addExpectCircleEndAngle_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_endAngleSpan.Text.Trim());
            //findCircleTool.endAngle += value;
            //tbx_endAngle.Text = findCircleTool.endAngle.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
        private void btn_subCliperNum_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_cliperNumSpan.Text.Trim());
            //findCircleTool.cliperNum  -= value;
            //tbx_cliperNum.Text = findCircleTool.cliperNum.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
        private void btn_addCliperNum_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_cliperNumSpan.Text.Trim());
            //findCircleTool.cliperNum += value;
            //tbx_cliperNum.Text = findCircleTool.cliperNum.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
        private void btn_switchPolarity_Click(object sender, EventArgs e)
        {
            //if (cbx_polarity.SelectedIndex == 0)
            //    cbx_polarity.SelectedIndex = 1;
            //else cbx_polarity.SelectedIndex = 0;
        }
        private void cbx_polarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            //findCircleTool.polarity = cbx_polarity.SelectedIndex == 0 ? "negative" : "positive";
        }
        private void btn_subThreshold_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_thresholdSpan.Text.Trim());
            //findCircleTool.threshold  -= value;
            //tbx_threshold.Text = findCircleTool.threshold.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
        private void btn_addThreshold_Click(object sender, EventArgs e)
        {
            //int value = Convert.ToInt16(tbx_thresholdSpan.Text.Trim());
            //findCircleTool.threshold += value;
            //tbx_threshold.Text = findCircleTool.threshold.ToString();
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
        }
      
        private void tbx_startAngle_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    findCircleTool.startAngle = Convert.ToDouble(tbx_startAngle.Text.Trim());
            //}
            //catch
            //{
            //    Frm_Main.Instance.OutputMsg("输入了非法字符，已自动替换为默认值：10", Color.Red);
            //    tbx_startAngle.Text = "10";
            //}
        }
        private void tbx_endAngle_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    findCircleTool.endAngle = Convert.ToDouble(tbx_endAngle.Text.Trim());
            //}
            //catch
            //{
            //    Frm_Main.Instance.OutputMsg("输入了非法字符，已自动替换为默认值：360", Color.Red);
            //    tbx_endAngle.Text = "360";
            //}
        }
        private void tbx_threshold_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    findCircleTool.threshold = Convert.ToInt16 (tbx_threshold.Text.Trim());
            //}
            //catch
            //{
            //    Frm_Main.Instance.OutputMsg("输入了非法字符，已自动替换为默认值：30", Color.Red);
            //    tbx_threshold.Text = "30";
            //}
        }
        private void tbx_ringRadiusLength_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    findCircleTool.ringRadiusLength = Convert.ToInt16(tbx_ringRadiusLength.Text.Trim());
            //}
            //catch
            //{
            //    Frm_Main.Instance.OutputMsg("输入了非法字符，已自动替换为默认值：80", Color.Red);
            //    tbx_ringRadiusLength.Text = "80";
            //}
        }
        private void tbx_cliperNum_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    findCircleTool.cliperNum = Convert.ToInt16(tbx_cliperNum.Text.Trim());
            //}
            //catch
            //{
            //    Frm_Main.Instance.OutputMsg("输入了非法字符，已自动替换为默认值：20", Color.Red);
            //    tbx_cliperNum.Text = "20";
            //}
        }
      
        internal void btn_runFindCircleTool_Click(object sender, EventArgs e)
        {
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
            //if (findCircleTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //    Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Red);
            //else
            //    Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Black);
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            //findCircleTool.UpdateImage(jobName);
            //findCircleTool.Run(jobName, true, true);
            //if (findCircleTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //    Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Red);
            //else
            //    Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Black);
        }
        private void ckb_displayCaliper_CheckedChanged(object sender, EventArgs e)
        {
            //findCircleTool.displayCaliper = ckb_displayCaliper.Checked;
            //pictureBox3.Image = (ckb_displayCaliper.Checked ? Resources.复选框 : Resources.去复选框);
        }
        private void ckb_displayFeature_CheckedChanged(object sender, EventArgs e)
        {
            //findCircleTool.displayFeature = ckb_displayFeature.Checked;
            //pictureBox4.Image = (ckb_displayCaliper.Checked ? Resources.复选框 : Resources.去复选框);
        }

        private void ckb_displayCircle_CheckedChanged(object sender, EventArgs e)
        {
            //findCircleTool.displayCircle = ckb_displayCircle.Checked;
            //pictureBox5.Image = (ckb_displayCaliper.Checked ? Resources.复选框 : Resources.去复选框);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //findCircleTool.displayCircleCenter = checkBox1.Checked;
            //pictureBox2.Image = (ckb_displayCaliper.Checked ? Resources.复选框 : Resources.去复选框);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
          
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
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

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void btn_runTool_Click(object sender, EventArgs e)
        {
            //btn_runTool.Enabled = false;
            //findCircleTool.Run(jobName, true, true);
            //if (findCircleTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //    Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Red);
            //else
            //    Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Green);
            //btn_runTool.Enabled = true;
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            //if (findCircleTool.displayCaliper )
            //{
            //    findCircleTool.displayCaliper = false;
            //    pictureBox3.Image = (findCircleTool.displayCaliper ? Resources.复选框 : Resources.去复选框);
            //}
            //else
            //{
            //    findCircleTool.displayCaliper = true;
            //    pictureBox3.Image = (findCircleTool.displayCaliper ? Resources.复选框 : Resources.去复选框);
            //}
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //if (findCircleTool.displayFeature )
            //{
            //    findCircleTool.displayFeature = false;
            //    pictureBox4.Image = (findCircleTool.displayFeature ? Resources.复选框 : Resources.去复选框);
            //}
            //else
            //{
            //    findCircleTool.displayFeature = true;
            //    pictureBox4.Image = (findCircleTool.displayFeature ? Resources.复选框 : Resources.去复选框);
            //}
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            //if (findCircleTool.displayCircle)
            //{
            //    findCircleTool.displayCircle = false;
            //    pictureBox5.Image = (findCircleTool.displayCircle ? Resources.复选框 : Resources.去复选框);
            //}
            //else
            //{
            //    findCircleTool.displayCircle = true;
            //    pictureBox5.Image = (findCircleTool.displayCircle ? Resources.复选框 : Resources.去复选框);
            //}
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            //if (findCircleTool.displayCircleCenter)
            //{
            //    findCircleTool.displayCircleCenter = false;
            //    pictureBox2.Image = (findCircleTool.displayCircleCenter ? Resources.复选框 : Resources.去复选框);
            //}
            //else
            //{
            //    findCircleTool.displayCircleCenter = true;
            //    pictureBox2.Image = (findCircleTool.displayCircleCenter ? Resources.复选框 : Resources.去复选框);
            //}
        }

        private void cbx_jobList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            applyTransTool.cliperNum = cbx_jobList.Text;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            applyTransTool.photoPos = Convert.ToInt16(textBox6 .Text .Trim ());
        }

    }
}
