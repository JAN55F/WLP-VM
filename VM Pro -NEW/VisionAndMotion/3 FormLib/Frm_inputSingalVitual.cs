using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    public partial class Frm_inputSingalVitual : Frm_FormBase 
    {
        public Frm_inputSingalVitual()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_inputSingalVitual _instance;
        public static Frm_inputSingalVitual Instance
        {
            get
            {
                _instance = new Frm_inputSingalVitual();
                return _instance;
            }
        }


        /// <summary>
        /// 延迟界面不卡死函数，慎用，会大量消耗CPU
        /// </summary>
        /// <param name="pinterval">时长</param>
        private void Delay(double interval)
        {
            try
            {
                DateTime time = DateTime.Now;
                double span = interval * 10000;
                while ((DateTime.Now.Ticks - time.Ticks) < span)
                {
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void Frm_inputSingalVitual_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (Di item in Enum.GetValues(typeof(Di)))
                {
                    cbx_inputSignal.Items.Add(item.ToString());
                }

                if (cbx_inputSignal.Items.Count > 0)
                    cbx_inputSignal.SelectedIndex = 0;
                cbx_vitualType.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_startVitual_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_startVitual.Text == "开始虚拟")
                {
                    btn_startVitual.Text = "虚拟中......";
                    cbx_inputSignal.Enabled = false;
                    cbx_vitualType.Enabled = false;
                    string vitualType = string.Empty;
                    switch (cbx_vitualType.SelectedIndex)
                    {
                        case 0:
                        case 1:
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "1" + cbx_vitualType.SelectedIndex;
                            break;
                        case 2:
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "11";
                            Delay(Convert.ToInt32(tbx_vitualTime.Text.Trim()));
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "00";
                            btn_startVitual.Text = "开始虚拟";
                            cbx_inputSignal.Enabled = true;
                            cbx_vitualType.Enabled = true;
                            break;
                        case 3:
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "01";
                            Delay(Convert.ToInt32(tbx_vitualTime.Text.Trim()));
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "00";
                            btn_startVitual.Text = "开始虚拟";
                            cbx_inputSignal.Enabled = true;
                            cbx_vitualType.Enabled = true;
                            break;
                        case 4:
                            Level level = Card_Googol.GetDiSts(cbx_inputSignal.SelectedItem.ToString());
                            if (level == Level.High)
                                Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "10";
                            else
                                Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "11";
                            break;
                    }
                }
                else
                {
                    Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "00";
                    btn_startVitual.Text = "开始虚拟";
                    cbx_inputSignal.Enabled = true;
                    cbx_vitualType.Enabled = true;
                }
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void cbx_vitualType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbx_vitualType.SelectedIndex == 2 || cbx_vitualType.SelectedIndex == 3)
            {
                tbx_vitualTime.Visible = true;
                lbl_vitualTime.Visible = true;
            }
            else
            {
                tbx_vitualTime.Visible = false;
                lbl_vitualTime.Visible = false;
            }
        }
        private void Frm_inputSingalVitual_FormClosing(object sender, FormClosingEventArgs e)
        {
            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "00";
        }

        private void btn_runOnce_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_startVitual.Text == "开始虚拟")
                {
                    btn_startVitual.Text = "虚拟中......";
                    cbx_inputSignal.Enabled = false;
                    cbx_vitualType.Enabled = false;
                    string vitualType = string.Empty;
                    switch (cbx_vitualType.SelectedIndex)
                    {
                        case 0:
                        case 1:
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "1" + cbx_vitualType.SelectedIndex;
                            break;
                        case 2:
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "11";
                            Delay(Convert.ToInt32(tbx_vitualTime.Text.Trim()));
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "00";
                            btn_startVitual.Text = "开始虚拟";
                            cbx_inputSignal.Enabled = true;
                            cbx_vitualType.Enabled = true;
                            break;
                        case 3:
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "01";
                            Delay(Convert.ToInt32(tbx_vitualTime.Text.Trim()));
                            Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "00";
                            btn_startVitual.Text = "开始虚拟";
                            cbx_inputSignal.Enabled = true;
                            cbx_vitualType.Enabled = true;
                            break;
                        case 4:
                            Level level = Card_Googol.GetDiSts(cbx_inputSignal.SelectedItem.ToString());
                            if (level == Level.High)
                                Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "10";
                            else
                                Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "11";
                            break;
                    }
                }
                else
                {
                    Card_Googol.D_inputSignalVitualStatu[cbx_inputSignal.SelectedItem.ToString()] = "00";
                    btn_startVitual.Text = "开始虚拟";
                    cbx_inputSignal.Enabled = true;
                    cbx_vitualType.Enabled = true;
                }
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonDown;
            Application.DoEvents();
        }

        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }

    }
}
