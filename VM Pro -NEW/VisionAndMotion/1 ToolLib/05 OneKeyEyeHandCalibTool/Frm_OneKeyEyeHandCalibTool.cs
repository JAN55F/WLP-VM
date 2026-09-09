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
using System.Threading;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_OneKeyEyeHandCalibTool : Frm_FormBase
    {
        internal Frm_OneKeyEyeHandCalibTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_OneKeyEyeHandCalibTool _instance;
        internal static Frm_OneKeyEyeHandCalibTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_OneKeyEyeHandCalibTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static OneKeyEyeHandCalibTool oneKeyEyeHandCalibTool = new OneKeyEyeHandCalibTool();


        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            oneKeyEyeHandCalibTool.ResetTool();
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
                oneKeyEyeHandCalibTool.calibJobName = cbx_jobList.Text;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void cbx_outputItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            oneKeyEyeHandCalibTool.calibItemName = cbx_outputItemList.Text;
        }
        private void cbo_calibType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbo_calibType.Text == "四点标定")
                {
                    if (dgv_calibData.Rows.Count == 9)
                    {
                        for (int i = dgv_calibData.Rows.Count-1; i > 3; i--)
                        {
                            dgv_calibData.Rows.RemoveAt(i);
                        }
                    }
                    oneKeyEyeHandCalibTool.calibType = CalibType.Four_Point;
                }
                else if (cbo_calibType.Text == "九点标定")
                {
                    dgv_calibData.Rows.Add(5);
                    oneKeyEyeHandCalibTool.calibType = CalibType.Nine_Point;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            oneKeyEyeHandCalibTool.Run(true, true, toolName);
        }
        private void btn_readCalibData_Click(object sender, EventArgs e)
        {
            oneKeyEyeHandCalibTool.ReadCalibData();
        }
        private void btn_writeCalibData_Click(object sender, EventArgs e)
        {
            oneKeyEyeHandCalibTool.WriteCalibData();
        }
        private void ckb_toolEnable_CheckedChanged(object sender, EventArgs e)
        {
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = ckb_toolEnable.Checked;
        }
        private void btn_calibrate_Click(object sender, EventArgs e)
        {
            oneKeyEyeHandCalibTool.Calibrate(true);
        }
        private void btn_connectPara_Click(object sender, EventArgs e)
        {
            Frm_ConnectPara frm_connectPara = new Frm_ConnectPara(oneKeyEyeHandCalibTool);
            frm_connectPara.ShowDialog();
        }
        private void btn_oneKeyCalibrate_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(o =>
            {
                oneKeyEyeHandCalibTool.OneKeyCalibrate(1);
            }));
        }

        private void Frm_OneKeyEyeHandCalibTool_Load(object sender, EventArgs e)
        {
            dgv_calibData.Rows.Add(4);
        }

    }
}
