using HalconDotNet;
using MotionAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tool;
using WeifenLuo.WinFormsUI.Docking;

namespace VMPro
{
    public partial class Frm_UserForm : Form
    {
        public Frm_UserForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_UserForm _instance;
        public static Frm_UserForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_UserForm();
                return _instance;
            }
        }


    }
    /// <summary>
    /// 轴
    /// </summary>
    public enum Axis
    {
        X,
        Y,
        YL,
        YR,
        Z,
        R,
        右侧轨道,
        中间轨道,
        左侧轨道,
        左侧调宽,
        右侧调宽,
        TR,
    }
}
