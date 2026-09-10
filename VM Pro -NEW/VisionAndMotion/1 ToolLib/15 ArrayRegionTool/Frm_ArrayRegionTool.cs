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

namespace VMPro
{
    internal partial class Frm_ArrayRegionTool : Frm_FormBase
    {
        internal Frm_ArrayRegionTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_ArrayRegionTool _instance;
        public static Frm_ArrayRegionTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ArrayRegionTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static ArrayRegionTool arrayRegionTool = new ArrayRegionTool();





        private void ckb_shapeMatchToolNotRun_CheckedChanged(object sender, EventArgs e)
        {
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_shapeMatchToolEnable.Checked;
        }

     

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            arrayRegionTool.Draw_Template_Rectangle1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            arrayRegionTool.UpdateRegion();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try 
            {
                arrayRegionTool.rowNum = Convert.ToInt16(textBox1.Text.Trim());
            }
            catch {}
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                arrayRegionTool.colNum = Convert.ToInt16(textBox2.Text.Trim());
            }
            catch { }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                arrayRegionTool.rowSpan = Convert.ToInt16(textBox4.Text.Trim());
            }
            catch { }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                arrayRegionTool.colSpan = Convert.ToInt16(textBox3.Text.Trim());
            }
            catch { }
        }

    }
}
