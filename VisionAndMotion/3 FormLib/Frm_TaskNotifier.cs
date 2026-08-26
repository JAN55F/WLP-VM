using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMPro._3_FormLib
{
    public partial class Frm_TaskNotifier : Frm_FormBase   
    {
        public Frm_TaskNotifier()
        {
            InitializeComponent();
        }
       // private TaskState m_TaskState = TaskState.Hidden;
        private System.Drawing.Rectangle m_maxBound;
        private System.Windows.Forms.Timer timer1;

        private bool isMouseEnter = false;
        private bool isMouseMove = false;
    }
}
