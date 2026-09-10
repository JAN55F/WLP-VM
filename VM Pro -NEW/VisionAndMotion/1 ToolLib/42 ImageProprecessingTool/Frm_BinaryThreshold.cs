using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMPro
{
    public partial class Frm_BinaryThreshold : Frm_FormBase 
    {
        public Frm_BinaryThreshold()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_BinaryThreshold _instance;
        internal static Frm_BinaryThreshold Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_BinaryThreshold();
                return _instance;
            }
        }

        /// <summary>
        /// 工具对象
        /// </summary>
        internal static ImageProprecessingTool imageProprecessingTool = new ImageProprecessingTool();
        internal static Binary binary = new Binary();
        internal static string jobName = string.Empty;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
          
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
          
        }

        private void label4_TextChanged(object sender, EventArgs e)
        {
            binary.lowThreshold = Convert.ToInt16(label4.Text);
            imageProprecessingTool.Run(true, true, toolName);
        }

        private void label3_TextChanged(object sender, EventArgs e)
        {
            binary.highThreshold = Convert.ToInt16(label3.Text);
            imageProprecessingTool.Run(true, true, toolName);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label4.Text = trackBar1.Value.ToString();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            label3.Text = trackBar2.Value.ToString();
        }

    }
}
