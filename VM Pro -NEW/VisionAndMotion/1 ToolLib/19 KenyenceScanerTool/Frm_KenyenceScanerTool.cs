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
using System.IO.Ports;

namespace VMPro
{
    internal partial class Frm_KenyenceScanerTool : Frm_FormBase
    {
        internal Frm_KenyenceScanerTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_KenyenceScanerTool _instance;
        public static Frm_KenyenceScanerTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_KenyenceScanerTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static Scaner_KenyenceTool kenyenceScanerTool = new Scaner_KenyenceTool();


        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            kenyenceScanerTool.ResetTool();
        }
        private void Frm_KenyenceScanerTool_Load(object sender, EventArgs e)
        {
            try
            {
                cbx_portName.Items.Clear();
                cbx_portName.Items.AddRange(SerialPort.GetPortNames());

                cbx_parityBit.Items.Clear();
                foreach (var item in Enum.GetValues(typeof(Parity)))
                {
                    cbx_parityBit.Items.Add(item.ToString());
                }

                cbx_stopBit.Items.Clear();
                foreach (var item in Enum.GetValues(typeof(StopBits)))
                {
                    cbx_stopBit.Items.Add(item.ToString());
                }

                if (cbx_portName.Items.Count > 0)
                    cbx_portName.SelectedIndex = 0;
                if (cbx_parityBit.Items.Count > 0)
                    cbx_parityBit.SelectedIndex = 1;
                if (cbx_stopBit.Items.Count > 1)
                    cbx_stopBit.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_openPort_Click(object sender, EventArgs e)
        {
            kenyenceScanerTool.OpenPort();
        }
        private void btn_closePort_Click(object sender, EventArgs e)
        {
            kenyenceScanerTool.ClosePort();
        }
        private void btn_send_Click(object sender, EventArgs e)
        {
            kenyenceScanerTool.Send(tbx_sendMsg.Text.Trim());
        }
        private void cbx_portName_SelectedIndexChanged(object sender, EventArgs e)
        {
            kenyenceScanerTool.portName = cbx_portName.Text.Trim();
        }
        private void cbx_baudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                kenyenceScanerTool.baudRate = Convert.ToInt32(cbx_baudRate.Text.Trim());
            }
            catch { }
        }
        private void tbx_dataBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                kenyenceScanerTool.dataBit = Convert.ToInt16(cbx_dataBit.Text.Trim());
            }
            catch { }
        }
        private void cbx_stopBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            kenyenceScanerTool.stopBit = (StopBits)Enum.Parse(typeof(StopBits), cbx_stopBit.Text);
        }
        private void cbx_parityBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            kenyenceScanerTool.parity = (Parity)Enum.Parse(typeof(Parity), cbx_parityBit.Text);
        }
        private void lnk_clear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tbx_log.Clear();
        }
        private void btn_startScan_Click(object sender, EventArgs e)
        {
            kenyenceScanerTool.ScanOnce();
        }
        private void btn_endScan_Click(object sender, EventArgs e)
        {
            kenyenceScanerTool.StopScan();
        }
        private void ckb_toolEnable_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_toolEnable.Checked;
        }
        private void tsb_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            kenyenceScanerTool.Run(true, true, toolName);
            if (kenyenceScanerTool.toolRunStatu  != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(kenyenceScanerTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(kenyenceScanerTool.toolRunStatu.ToString(), Color.Green);
            btn_runTool.Enabled = true;
        }
        private void btn_runTool_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            kenyenceScanerTool.Run(true, true, toolName);
            if (kenyenceScanerTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(kenyenceScanerTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(kenyenceScanerTool.toolRunStatu.ToString(), Color.Black);
            btn_runTool.Enabled = true;
        }

    }
}
