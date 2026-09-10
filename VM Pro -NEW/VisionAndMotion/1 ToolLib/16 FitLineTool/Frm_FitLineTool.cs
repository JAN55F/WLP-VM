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
    public partial class Frm_FitLineTool : Form
    {
        public Frm_FitLineTool()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_FitLineTool _instance;
        public static Frm_FitLineTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_FitLineTool();
                return _instance;
            }
        }

    }
}
