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
    public partial class Frm_DistanceLLTool : Frm_FormBase 
    {
        public Frm_DistanceLLTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_DistanceLLTool _instance;
        public static Frm_DistanceLLTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_DistanceLLTool();
                return _instance;
            }
        }

    }
}
