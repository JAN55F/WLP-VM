using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_RotatePlatformTool : Frm_FormBase
    {
        internal Frm_RotatePlatformTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_RotatePlatformTool _instance;
        internal static Frm_RotatePlatformTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_RotatePlatformTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static RotatePlatformTool rotatePlatformTool;


        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            rotatePlatformTool.Run(true, true, toolName);
            if (rotatePlatformTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(rotatePlatformTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(rotatePlatformTool.toolRunStatu.ToString(), Color.Black );
        }
        private void Frm_RotatePlatformTool_Load(object sender, EventArgs e)
        {
            try
            {
                if (dgv_data.Rows.Count != 3)
                    dgv_data.Rows.Add(3);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex );
            }
        }
        private void ckb_toolEnable_CheckedChanged(object sender, EventArgs e)
        {
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = ckb_toolEnable.Checked;
        }
        private void tsb_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }
        private void btn_firstPoint_Click(object sender, EventArgs e)
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
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1].Substring (3);
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                Type type = point.GetType();
                XYU curPos = new XYU();
                if (type.Name == "XYU")
                {
                    curPos.Point.X = ((XYU)point).Point.X;
                    curPos.Point.Y = ((XYU)point).Point.Y;
                }
                else
                {
                    curPos.Point.X = ((XY)point).X;
                    curPos.Point.Y = ((XY)point).Y;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_data.Rows[0].Cells[1].Value = row;
                dgv_data.Rows[0].Cells[2].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_calculate_Click(object sender, EventArgs e)
        {
            rotatePlatformTool.CalculateRotateCenter();
          
        }
        private void cbx_jobList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cbx_outputItemList.Items.Clear();
                for (int j = 0; j < Job.FindJobByName(cbx_jobList.SelectedItem.ToString()).L_toolList.Count; j++)
                {
                    for (int k = 0; k < Job.FindJobByName(cbx_jobList.SelectedItem.ToString()).L_toolList[j].output.Count; k++)
                    {
                        DataType ioType = Job.FindJobByName(cbx_jobList.SelectedItem.ToString()).L_toolList[j].output[k].ioType;
                        if (ioType == DataType.XY || ioType == DataType.Pose)
                            cbx_outputItemList.Items.Add(Job.FindJobByName(cbx_jobList.SelectedItem.ToString()).L_toolList[j].toolName + " . -->" + Job.FindJobByName(cbx_jobList.SelectedItem.ToString()).L_toolList[j].output[k].IOName);
                    }
                }
                rotatePlatformTool.calibJobName = cbx_jobList.Text;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void cbx_outputItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            rotatePlatformTool.calibItemName = cbx_outputItemList.Text;
        }
        private void btn_secondPoint_Click(object sender, EventArgs e)
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
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1].Substring (3);
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                Type type = point.GetType();
                XYU curPos = new XYU();
                if (type.Name == "XYU")
                {
                    curPos.Point.X = ((XYU)point).Point.X;
                    curPos.Point.Y = ((XYU)point).Point.Y;
                }
                else
                {
                    curPos.Point.X = ((XY)point).X;
                    curPos.Point.Y = ((XY)point).Y;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_data.Rows[1].Cells[1].Value = row;
                dgv_data.Rows[1].Cells[2].Value = col;
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
                if (cbx_jobList.Text == string.Empty || cbx_outputItemList.Text == string.Empty)
                {
                    Frm_Output.Instance.OutputMsg("获取失败，请先指定图像特征点", Color.Red);
                    return;
                }
                string jobName = cbx_jobList.Text;
                string toolName = Regex.Split(cbx_outputItemList.Text, " . ")[0];
                string outputItem = Regex.Split(cbx_outputItemList.Text, " . ")[1].Substring (3);
                Job.RunAndWait(jobName);
                object point = Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value;
                Type type = point.GetType();
                XYU curPos = new XYU();
                if (type.Name == "XYU")
                {
                    curPos.Point.X = ((XYU)point).Point.X;
                    curPos.Point.Y = ((XYU)point).Point.Y;
                }
                else
                {
                    curPos.Point.X = ((XY)point).X;
                    curPos.Point.Y = ((XY)point).Y;
                }
                double row = curPos.Point.X; ;
                double col = curPos.Point.Y;
                dgv_data.Rows[2].Cells[1].Value = row;
                dgv_data.Rows[2].Cells[2].Value = col;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
