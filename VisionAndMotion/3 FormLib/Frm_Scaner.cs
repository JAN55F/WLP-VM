using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_Scaner : Form
    {
        internal Frm_Scaner()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 扫码枪对象
        /// </summary>
        private static Scaner scaner;
        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_Scaner _instance;
        public static Frm_Scaner Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_Scaner();
                return _instance;
            }
        }

        internal static bool TryGetExistingInstance(out Frm_Scaner form)
        {
            form = _instance;
            if (form == null || form.IsDisposed || form.Disposing)
            {
                form = null;
                return false;
            }
            return true;
        }

        internal static bool TryAppendOutput(Scaner source, string line)
        {
            Frm_Scaner form;
            if (source == null || !TryGetExistingInstance(out form) || !form.IsHandleCreated)
                return false;

            bool appended = false;
            MethodInvoker apply = delegate
            {
                try
                {
                    Frm_Scaner current;
                    if (!TryGetExistingInstance(out current) ||
                        !ReferenceEquals(current, form) ||
                        !ReferenceEquals(scaner, source) ||
                        !current.Visible ||
                        (!current.TopLevel && current.Parent == null))
                        return;

                    current.tbx_output.AppendText(line);
                    appended = true;
                }
                catch (Exception ex)
                {
                    Log.SaveError(ex);
                }
            };

            try
            {
                if (form.InvokeRequired)
                {
                    form.BeginInvoke(apply);
                    return false;
                }
                else
                    apply();
                return appended;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }


        /// <summary>
        /// 加载参数到界面
        /// </summary>
        /// <param name="scaner_1"></param>
        internal void LoadPar(Scaner scaner_1)
        {
            try
            {
                scaner = scaner_1;
                tbx_clientName.TextStr = scaner.Name;
                bool contains = false;
                for (int i = 0; i < cbx_portName.Items.Length; i++)
                {
                    if (cbx_portName.Items[i] == scaner.portName)
                    {
                        contains = true;
                        break;
                    }
                }
                if (!contains)
                    cbx_portName.Add(scaner.portName);
                cbx_portName.TextStr = scaner.portName;
                cbx_baudRate.TextStr = scaner.baudRate.ToString();
                tbx_dataBit.TextStr = scaner.dataBit.ToString();
                cbx_stopBit.SelectedIndex = (int)scaner.stopBit;
                cbx_parityBit.SelectedIndex = (int)scaner.parity;

                tbx_trigCmd.TextStr = scaner.TrigCmd;
                tbx_scanNum.TextStr = scaner.failNum.ToString();
                if (scaner.endChar == string.Empty)
                {
                    btn_endCharNone.BackColor = Color.Gray;
                    btn_endCharEnter.BackColor = Color.Gainsboro;
                }
                else
                {
                    btn_endCharNone.BackColor = Color.Gainsboro;
                    btn_endCharEnter.BackColor = Color.Gray;
                }

                //////if (scaner.FindSerialPortByName ().IsOpen )
                //////    btn_connect.TextStr = "断开";
                //////else
                //////    btn_connect.TextStr = "连接";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void Frm_Serial_Load(object sender, EventArgs e)
        {
            try
            {
                cbx_portName.Clear();
                string[] strs = SerialPort.GetPortNames();
                for (int i = 0; i < strs.Length; i++)
                {
                    cbx_portName.Add(strs[i]);
                }

                cbx_parityBit.Clear();
                foreach (var item in Enum.GetValues(typeof(Parity)))
                {
                    cbx_parityBit.Add(item.ToString());
                }

                cbx_stopBit.Clear();
                foreach (var item in Enum.GetValues(typeof(StopBits)))
                {
                    cbx_stopBit.Add(item.ToString());
                }

                //if (cbx_portName.Items.Length > 0)
                //    cbx_portName.SelectedIndex = 0;
                //if (cbx_parityBit.Items.Length > 0)
                //    cbx_parityBit.SelectedIndex = 0;
                //if (cbx_stopBit.Items.Length > 0)
                //    cbx_stopBit.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void cbx_portName_SelectedIndexChanged()
        {
            if (Frm_DeviceManager.cancel)
                return;

            scaner.portName = cbx_portName.TextStr.Trim();
        }
        private void cbx_baudRate_SelectedIndexChanged()
        {
            try
            {
                if (Frm_DeviceManager.cancel)
                    return;

                scaner.baudRate = Convert.ToInt32(cbx_baudRate.TextStr.Trim());
            }
            catch { }
        }
        private void tbx_dataBit_SelectedIndexChanged()
        {
            try
            {
                if (Frm_DeviceManager.cancel)
                    return;

                scaner.dataBit = Convert.ToInt32(tbx_dataBit.TextStr.Trim());
            }
            catch { }
        }
        private void cbx_stopBit_SelectedIndexChanged()
        {
            if (Frm_DeviceManager.cancel)
                return;

            scaner.stopBit = (StopBits)Enum.Parse(typeof(StopBits), cbx_stopBit.TextStr);
        }
        private void cbx_parityBit_SelectedIndexChanged()
        {
            if (Frm_DeviceManager.cancel)
                return;

            scaner.parity = (Parity)Enum.Parse(typeof(Parity), cbx_parityBit.TextStr);
        }
        private void lnk_clear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tbx_output.Clear();
        }
        private void btn_connect_Clicked()
        {
            scaner.Init();
        }
        private void btn_close_Clicked()
        {
            scaner.Close();
        }
        private void btn_send_Clicked()
        {
            try
            {
                if (tbx_sendMsg.TextStr.Trim() == string.Empty)
                {
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n不能发送空字符串");
                    return;
                }
                scaner.Send(tbx_sendMsg.TextStr.Trim());
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void tbx_trigCmd_TextStrChanged(string textStr)
        {
            scaner.TrigCmd = tbx_trigCmd.TextStr.Trim();
        }
        private void tbx_scanNum_TextStrChanged(string textStr)
        {
            scaner.failNum = Convert.ToInt16(tbx_scanNum.TextStr.Trim());
        }
        private void cButton2_Clicked()
        {
            scaner.Scan();
        }
        private void btn_endCharNone_Click(object sender, EventArgs e)
        {
            scaner.endChar = string.Empty;
            btn_endCharNone.BackColor = Color.Gray;
            btn_endCharEnter.BackColor = Color.Gainsboro;
        }
        private void btn_endCharEnter_Click(object sender, EventArgs e)
        {
            scaner.endChar = "\r\n";
            btn_endCharNone.BackColor = Color.Gainsboro;
            btn_endCharEnter.BackColor = Color.Gray;
        }

     

    }
}
