using csDmc2210;
using csDmc2410;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_AxisSetting : Form
    {
        internal Frm_AxisSetting()
        {
            InitializeComponent();
            Init_Language();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_AxisSetting _instance;
        public static Frm_AxisSetting Instance
        {
            get
            {
                _instance = new Frm_AxisSetting();
                return _instance;
            }
        }


        /// <summary>
        /// 初始化语言
        /// </summary>
        private void Init_Language()
        {
            try
            {
                if (Project .Instance .configuration .language == Language.English)
                {
                    this.Text = "Axis Control";
                }
            }
            catch (Exception ex)
            {
               Log.SaveError(ex);
            }
        }


        private void Frm_AxisSetting_Load(object sender, EventArgs e)
        {
            this.ppg_axisProperty.SelectedObject = Axis_Config.Instance;
            this.TopMost = true;
        }
        private void Frm_AxisSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < Axis_Config.Instance.MMPixelRoute.Length; i++)
            {
                Axis_Config.Instance.MMPixelRoute[i] = (Axis_Config.Instance.丝杆螺距[i] / Axis_Config.Instance.编码器分辨率[i]);
            }

            switch (Project .Instance .configuration .cardType)
            {
                case CardType.雷塞_DMC2210:
                    //设置限位感应器的有效电平和制动方式
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2210.d2210_config_EL_MODE(i, (ushort)(Axis_Config.Instance.限位逻辑电平[i]));
                    }

                    //设置脉冲输出模式
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2210.d2210_set_pulse_outmode(i, (ushort)(Axis_Config.Instance.脉冲输出模式[i]));
                    }

                    //设置原点感应器的逻辑电平
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2210.d2210_set_HOME_pin_logic(i, (ushort)(Axis_Config.Instance.原点逻辑电平[i] == LogicLevel.低电平有效 ? 0 : 1), 1);
                    }

                    //设置编码器计数方式
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2210.d2210_counter_config(i, (ushort)(Axis_Config.Instance.编码器计数方式[i]));
                    }
                    break;
                case CardType.雷塞_DMC2410:
                    //设置限位感应器的有效电平和制动方式
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2410.d2410_config_EL_MODE(i, (ushort)(Axis_Config.Instance.限位逻辑电平[i]));
                    }

                    //设置脉冲输出模式
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2410.d2410_set_pulse_outmode(i, (ushort)(Axis_Config.Instance.脉冲输出模式[i]));
                    }

                    //设置原点感应器的逻辑电平
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2410.d2410_set_HOME_pin_logic(i, (ushort)(Axis_Config.Instance.原点逻辑电平[i] == LogicLevel.低电平有效 ? 0 : 1), 1);
                    }

                    //设置编码器计数方式
                    for (ushort i = 0; i < 2; i++)
                    {
                        Dmc2410.d2410_counter_config(i, (ushort)(Axis_Config.Instance.编码器计数方式[i]));
                    }
                    break;
            }

            IFormatter formatter = HalconSerializationGuard.CreateFormatter();
            Stream stream = new FileStream(Application.StartupPath + "\\Config\\Project\\Motion\\AxisPar.cfg", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, Axis_Config.Instance);
            stream.Close();
        }

    }
}
