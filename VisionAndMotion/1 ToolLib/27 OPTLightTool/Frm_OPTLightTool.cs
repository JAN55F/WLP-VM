using HalconDotNet;
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
    internal partial class Frm_OPTLightTool : Frm_FormBase
    {
        internal Frm_OPTLightTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_OPTLightTool _instance;
        internal static Frm_OPTLightTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_OPTLightTool();
                return _instance;
            }
        }

        public static Light_OPTTool optLightTool = new Light_OPTTool();

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //OPTControl optControl = new OPTControl(ControlMode.ID, textBox1.Text.Trim(), textBox2.Text.Trim());
                //optLightTool.L_controls.Add(textBox1.Text.Trim(), textBox2.Text.Trim());
                //textBox1.Clear();
                //textBox2.Clear();
                //ctl_opt.ReFlush();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void Frm_OPTLightTool_Load(object sender, EventArgs e)
        {
            //////foreach (KeyValuePair<string, string> item in optLightTool.L_controls)
            //////{
            //////    OPTControl optControl = new OPTControl(ControlMode.ID, item.Key, item.Value);
            //////}
            //////Frm_OPTLightTool.Instance.ctl_opt.ReFlush();
        }

    }
}
