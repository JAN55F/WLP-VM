using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    public partial class Frm_ConnectPara : Form
    {
        internal  Frm_ConnectPara(OneKeyEyeHandCalibTool oneKeyEyeHandCalibTool)
        {
            InitializeComponent();
            this.oneKeyEyeHandCalibTool = oneKeyEyeHandCalibTool;
        }

        /// <summary>
        /// 工具对象
        /// </summary>
        private OneKeyEyeHandCalibTool oneKeyEyeHandCalibTool;


        private void Frm_ConnectPara_Load(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = true;
                this.tbx_ip.Text = oneKeyEyeHandCalibTool.ip;
                this.tbx_port.Text = oneKeyEyeHandCalibTool.port.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_apply_Click(object sender, EventArgs e)
        {
            try
            {
                oneKeyEyeHandCalibTool.ip = this.tbx_ip .Text;
                oneKeyEyeHandCalibTool.port = Convert.ToInt16(tbx_port.Text.Trim());
                this.Close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
