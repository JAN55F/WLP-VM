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

namespace VMPro
{
    internal partial class Frm_PointOffsetTool : Frm_FormBase
    {
        internal Frm_PointOffsetTool()
        {
            InitializeComponent();
            tbx_caputurePosX.ValueChanged += tbx_caputurePosX_valueChanged;
            tbx_caputurePosY.ValueChanged += tbx_caputurePosY_valueChanged;
        }

        void tbx_caputurePosY_valueChanged(double  value)
        {
            try
            {
                pointOffsetTool.templatePos.Y = value;
            }
            catch { }
        }

        void tbx_caputurePosX_valueChanged(double  value)
        {
            try
            {
                pointOffsetTool.templatePos.X = value;
            }
            catch { }
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_PointOffsetTool _instance;
        public static Frm_PointOffsetTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_PointOffsetTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static PointOffsetTool pointOffsetTool = new PointOffsetTool();




        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.ResetTool();
        }

        private void btn_runShapeMatchTool_Click(object sender, EventArgs e)
        {

        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            //////btn_runDistancePLTool.Enabled = false;
            //////shapeMatchTool.Run(true, jobName);
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runDistancePLTool.Enabled = true;
        }

        private void tbx_caputurePosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointOffsetTool.templatePos.X = Convert.ToDouble(tbx_caputurePosX.Value);
            }
            catch { }
        }

        private void tbx_caputurePosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointOffsetTool.templatePos.Y = Convert.ToDouble(tbx_caputurePosY.Value);
            }
            catch { }
        }



        private void btn_autoGet_Click(object sender, EventArgs e)
        {

        }

        private void tbx_pickPosOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointOffsetTool.buchang.X = Convert.ToDouble(tbx_pickPosOffsetX.Text.Trim());
            }
            catch { }
        }

        private void tbx_pickPosOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointOffsetTool.buchang.Y = Convert.ToDouble(tbx_pickPosOffsetY.Text.Trim());
            }
            catch { }
        }


        private void tbx_pickPosX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointOffsetTool.workPos.X = Convert.ToDouble(tbx_pickPosX.Text.Trim());
            }
            catch
            {

            }
        }

        private void tbx_pickPosY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pointOffsetTool.workPos.Y = Convert.ToDouble(tbx_pickPosY.Text.Trim());
            }
            catch
            {

            }
        }
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            if (!comboBox1.Items.Contains(comboBox1.Text))
            {
                comboBox1.Text = "1";
            }
            pointOffsetTool.pointIdx = Convert.ToInt16(comboBox1.Text.Trim()) - 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            pointOffsetTool.pointIdx = Convert.ToInt16(comboBox1.Text.Trim());
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
                pictureBox8.Image = Resources.Enable;
            else
                pictureBox8.Image = Resources.Disable;
        }

        private void btn_runTool_Click(object sender, EventArgs e)
        {
            //////btn_runDistancePLTool.Enabled = false;
            //////shapeMatchTool.Run(true, jobName);
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runDistancePLTool.Enabled = true;
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

        private void button5_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
            tbx_caputurePosX.Value = ((List<XY>)pointOffsetTool.toolPar.InputPar.点)[pointOffsetTool.pointIdx - 1].X.ToString();
            tbx_caputurePosY.Value = ((List<XY>)pointOffsetTool.toolPar.InputPar.点)[pointOffsetTool.pointIdx - 1].Y.ToString();
        }

        private void btn_setToPickPos_Click(object sender, EventArgs e)
        {

        }

        private void tbx_pickPosOffsetX_Leave(object sender, EventArgs e)
        {
            pointOffsetTool.buchang.X = tbx_pickPosOffsetX.Value;
        }

        private void tbx_pickPosOffsetY_Leave(object sender, EventArgs e)
        {
            pointOffsetTool.buchang.Y = tbx_pickPosOffsetY.Value;
        }

        private void tbx_caputurePosX_Leave(object sender, EventArgs e)
        {
            pointOffsetTool.templatePos.X = Convert.ToDouble(tbx_caputurePosX.Value);

        }

        private void tbx_caputurePosY_Leave(object sender, EventArgs e)
        {
            try
            {
                pointOffsetTool.templatePos.Y = Convert.ToDouble(tbx_caputurePosY.Value);
            }
            catch { }
        }

        private void tbx_pickPosX_Leave(object sender, EventArgs e)
        {
            try
            {
                pointOffsetTool.workPos.X = Convert.ToDouble(tbx_pickPosX.Value);
            }
            catch
            {

            }
        }

        private void tbx_pickPosY_Load(object sender, EventArgs e)
        {

        }

        private void tbx_pickPosY_Leave(object sender, EventArgs e)
        {
            try
            {
                pointOffsetTool.workPos.Y = Convert.ToDouble(tbx_pickPosY.Value);
            }
            catch
            {

            }
        }

    }
}
