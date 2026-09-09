using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace VMPro
{
    /// <summary>
    /// 轴参数配置类
    /// </summary>
    [Serializable]
    public class Axis_Config
    {

        /// <summary>
        /// 类实例对象
        /// </summary>
        private static Axis_Config _instance;
        public static Axis_Config Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Axis_Config();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// 脉冲输出模式
        /// </summary>
        private int[] _脉冲输出模式 = new int[20] { 2, 2, 2, 2, 2, 2, 2, 2,2, 2, 2, 2, 2, 2, 2, 2,2, 2, 2, 2 };
        [Category("Axis参数设置"), DescriptionAttribute("功能：设置各轴的脉冲输出模式,0-5分别代表6种输出模式")]
        public int[] 脉冲输出模式
        {
            get { return _脉冲输出模式; }
            set { _脉冲输出模式 = value; }
        }

        /// <summary>
        /// 编码器计数方式
        /// </summary>
        private int[] _编码器计数方式 = new int[20] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        [Category("Axis参数设置"), DescriptionAttribute("功能：设置各轴的编码器计数方式,0-4分别代表4种输出模式")]
        public int[] 编码器计数方式
        {
            get { return _编码器计数方式; }
            set { _编码器计数方式 = value; }
        }

        /// <summary>
        /// 轴回零速度
        /// </summary>
        private int[] _回零速度 = new int[20] { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 };
        [Category("Axis参数设置"), DescriptionAttribute("功能：设置各轴的回零速度，单位：mm/s")]
        public int[] 回零速度
        {
            get { return _回零速度; }
            set { _回零速度 = value; }
        }

        /// <summary>
        /// 轴回零方向
        /// </summary>
        private HomeDir[] _回零方向 = new HomeDir[20] { HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向, HomeDir.N_负方向 };
        [Category("Axis参数设置"), DescriptionAttribute("功能：设置各轴的回零方向\nN:负方向\nP:正方向")]
        public HomeDir[] 回零方向
        {
            get { return _回零方向; }
            set { _回零方向 = value; }
        }

        /// <summary>
        /// 回零搜索长度
        /// </summary>
        private int[] _回零搜索长度 = new int[20] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
        [Category("Axis参数设置"), DescriptionAttribute("功能：设置各轴的回零搜索长度，单位：mm")]
        public int[] 回零搜索长度
        {
            get { return _回零搜索长度; }
            set { _回零搜索长度 = value; }
        }

        /// <summary>
        /// 丝杆螺距
        /// </summary>
        private double[] _丝杆螺距 = new double[20] { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 };
        [Category("Axis参数设置"), DescriptionAttribute("功能：各轴丝杆的螺距(若为非丝杆类，可大概输入一个值)，单位：mm")]
        public double[] 丝杆螺距
        {
            get { return _丝杆螺距; }
            set { _丝杆螺距 = value; }
        }

        /// <summary>
        /// 编码器分辨率
        /// </summary>
        private int[] _编码器分辨率 = new int[20] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 ,10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000,10000, 10000, 10000, 10000};
        [Category("Axis参数设置"), DescriptionAttribute("功能：各轴编码器的分辨率")]
        public int[] 编码器分辨率
        {
            get { return _编码器分辨率; }
            set { _编码器分辨率 = value; }
        }

        /// <summary>
        /// 回零回退长度
        /// </summary>
        private int[] _回退长度 = new int[20] { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 };
        [Category("Axis参数设置"), DescriptionAttribute("功能：设置各轴的回零回退长度，单位：mm")]
        public int[] 回退长度
        {
            get { return _回退长度; }
            set { _回退长度 = value; }
        }

        /// <summary>
        /// 负软极限
        /// </summary>
        private int[] _负软极限 = new int[20] { -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100 };
        [Category("软限位"), DescriptionAttribute("功能：设置各轴的正软极限，单位：mm")]
        public int[] 负软极限
        {
            get { return _负软极限; }
            set { _负软极限 = value; }
        }

        /// <summary>
        /// 正软极限
        /// </summary>
        private int[] _正软极限 = new int[20] { 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500 };
        [Category("软限位"), DescriptionAttribute("功能：设置各轴的正软极限，单位：mm")]
        public int[] 正软极限
        {
            get { return _正软极限; }
            set { _正软极限 = value; }
        }

        /// <summary>
        /// 原点逻辑电平
        /// </summary>
        private LogicLevel[] _原点逻辑电平 = new LogicLevel[20] { LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效};
        [Category("逻辑电平"), DescriptionAttribute("功能：设置各轴原点信号的逻辑电平")]
        public LogicLevel[] 原点逻辑电平
        {
            get { return _原点逻辑电平; }
            set { _原点逻辑电平 = value; }
        }

        /// <summary>
        /// 负限位逻辑电平
        /// </summary>
        private LogicLevel[] _限位逻辑电平 = new LogicLevel[20] { LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效, LogicLevel.低电平有效 };
        [Category("逻辑电平"), DescriptionAttribute("功能：设置各轴负限位信号的逻辑电平")]
        public LogicLevel[] 限位逻辑电平
        {
            get { return _限位逻辑电平; }
            set { _限位逻辑电平 = value; }
        }

        /// <summary>
        /// 各轴的毫米脉冲比
        /// </summary>
        public double[] MMPixelRoute = new double[20] { 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0.005 };

    }
    public enum HomeDir
    {
        P_正方向,
        N_负方向,
    }
    public enum LogicLevel
    {
        高电平有效,
        低电平有效,
    }
}
