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
    internal  partial class Frm_CircleCalibrationTool : Frm_FindCircleTool 
    {
        public Frm_CircleCalibrationTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_CircleCalibrationTool _instance;
        internal static Frm_CircleCalibrationTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_CircleCalibrationTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static CircleCalibrationTool circleCalibrationTool = new CircleCalibrationTool();


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //circleCalibrationTool.MMPixelRoute = (Convert.ToDouble(textBox1.Text.Trim()) / circleCalibrationTool .ResultCircleRadius );
            //tbx_mmPixelRoute.Text = circleCalibrationTool.MMPixelRoute.ToString();
        }

        private void tbx_mmPixelRoute_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
