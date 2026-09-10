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
    internal partial class Frm_KenyenceScanerTool1 : Frm_FormBase
    {
        internal Frm_KenyenceScanerTool1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_KenyenceScanerTool1 _instance;
        public static Frm_KenyenceScanerTool1 Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_KenyenceScanerTool1();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static KenyenceScanerTool1 kenyenceScanerTool1 = new KenyenceScanerTool1();


        internal void btn_drawShapeMatchSearchRegion_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.Draw_Search_Region();
        }
        private void btn_deleteShapeMatchSearchRegion_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.Clear_Search_Region();
        }
        private void dgv_matchResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //shapeMatchTool.Click_Result_Dgv(e);
        }
        private void tkb_contrast_Scroll(object sender, EventArgs e)
        {
            //shapeMatchTool.Contrast_Changed();
        }

        private void cbo_shapeMatchSearchRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            //shapeMatchTool.Draw_Search_Region();
        }
        private void btn_displayStandardImage_Click(object sender, EventArgs e)
        {
            //HOperatorSet.DispObj(shapeMatchTool.standardImage, Frm_ImageWindow.Instance.WindowHandle);
        }
        private void btn_displayTemplateContour_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.ShowTemplate();
        }
        private void ckb_shapeMatchToolNotRun_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_shapeMatchToolEnable.Checked;
        }
        private void btn_drawTemplateRegionRectangle1_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.Draw_Template_Rectangle1();
        }
        private void btn_drawTemplateRegionRectangle2_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.Draw_Template_Rectangle2();
        }
        private void btn_drawTemplateRegionCircle_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.Draw_Template_Circle();
        }
        private void btn_drawTemplateRegionEllipse_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.Draw_Template_Ellipse();
        }
        private void btn_drawTemplateRegionAny_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.Draw_Template_Any();
        }
    
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.ResetTool();
        }
        private void nud_minScore_ValueChanged(object sender, EventArgs e)
        {
            //shapeMatchTool.minScore = Convert.ToDouble(nud_minScore.Value);
        }
        private void nud_findResultNum_ValueChanged(object sender, EventArgs e)
        {
            //shapeMatchTool.expectMatchNum = Convert.ToInt16(nud_matchNum.Value);
        }
        private void ckb_showCross_CheckedChanged(object sender, EventArgs e)
        {
            //shapeMatchTool.showCross = ckb_showCross.Checked;
        }
        private void ckb_showFeature_CheckedChanged(object sender, EventArgs e)
        {
            //shapeMatchTool.showFeature = ckb_showFeature.Checked;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Job.RunAndWait(jobName);
        }
        private void btn_runShapeMatchTool_Click(object sender, EventArgs e)
        {
            //////btn_runShapeMatchTool.Enabled = false;
            //////shapeMatchTool.Run(true, jobName);
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runShapeMatchTool.Enabled = true;
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            //////btn_runShapeMatchTool.Enabled = false;
            //////shapeMatchTool.Run(true, jobName);
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runShapeMatchTool.Enabled = true;
        }

        private void btn_runKenyenceScannerTool_Click(object sender, EventArgs e)
        {
            btn_runKenyenceScannerTool.Enabled = false;
            kenyenceScanerTool1.Run(true, true, toolName);
            if (kenyenceScanerTool1.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(kenyenceScanerTool1.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(kenyenceScanerTool1.toolRunStatu.ToString(), Color.Black);
            btn_runKenyenceScannerTool.Enabled = true;
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
            (kenyenceScanerTool1).OpenPort();
        }

        private void btn_closePort_Click(object sender, EventArgs e)
        {
            (kenyenceScanerTool1).ClosePort();
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            (kenyenceScanerTool1).Send(tbx_sendMsg.Text.Trim());
        }

        private void cbx_portName_SelectedIndexChanged(object sender, EventArgs e)
        {
            (kenyenceScanerTool1).portName = cbx_portName.Text.Trim();
        }

        private void cbx_baudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                (kenyenceScanerTool1).baudRate = Convert.ToInt32(cbx_baudRate.Text.Trim());
            }
            catch { }
        }

        private void tbx_dataBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                (kenyenceScanerTool1).dataBit = Convert.ToInt16(cbx_dataBit.Text.Trim());
            }
            catch { }
        }

        private void cbx_stopBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            kenyenceScanerTool1.stopBit = (StopBits)Enum.Parse(typeof(StopBits), cbx_stopBit.Text);
        }
        private void cbx_parityBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            kenyenceScanerTool1.parity = (Parity)Enum.Parse(typeof(Parity), cbx_parityBit.Text);
        }
        private void lnk_clear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tbx_output.Clear();
        }
        private void btn_startScan_Click(object sender, EventArgs e)
        {
            kenyenceScanerTool1.Send("LON");
            kenyenceScanerTool1.Read();
        }
        private void btn_endScan_Click(object sender, EventArgs e)
        {
            kenyenceScanerTool1.Send("LOFF");
            kenyenceScanerTool1.Read();
        }

    }
}
