using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMPro
{
    public partial class Frm_PLCComm : Form
    {
        public Frm_PLCComm()
        {
            InitializeComponent();
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 2; // 默认 AB
            cbo_inovanceSeries.SelectedIndex = 1; // 默认 H3U
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private static Frm_PLCComm _instance;
        internal static Frm_PLCComm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Frm_PLCComm();
                return _instance;
            }
        }

        internal static bool TryGetExistingInstance(out Frm_PLCComm form)
        {
            form = _instance;
            if (form == null || form.IsDisposed || form.Disposing)
            {
                form = null;
                return false;
            }
            return true;
        }

        private PLCDevice _device;

        /// <summary>
        /// 加载 PLC 设备参数到界面
        /// </summary>
        internal void LoadPar(PLCDevice device)
        {
            _device = device;
            comboBox1.SelectedIndex = device.Brand == PLCBrand.Inovance ? 5 : (device.Brand == PLCBrand.Omron ? 0 : (device.Brand == PLCBrand.Mitsubishi ? 4 : 2));
            SelectInovanceSeries(device.InovanceSeries);
            UpdateInovanceSeriesVisibility();
            textBox4.Text = device.IpAddress;
            textBox5.Text = device.Port.ToString();
            ckb_autoConnectAfterStart.Checked = device.AutoConnectAfterStart;
            ckb_autoDisconnectBeforeClose.Checked = device.AutoDisconnectBeforeClose;
            UpdateStatus();
            UpdateConnectionControlState();
        }

        private PLCBrand GetSelectedBrand()
        {
            if (comboBox1.Text.StartsWith("汇川")) return PLCBrand.Inovance;
            if (comboBox1.Text.StartsWith("欧姆龙")) return PLCBrand.Omron;
            if (comboBox1.Text.StartsWith("三菱")) return PLCBrand.Mitsubishi;
            return PLCBrand.AB;
        }

        private void SelectInovanceSeries(string series)
        {
            int index = cbo_inovanceSeries.FindStringExact(string.IsNullOrEmpty(series) ? "H3U" : series);
            cbo_inovanceSeries.SelectedIndex = index >= 0 ? index : 1;
        }

        private void UpdateInovanceSeriesVisibility()
        {
            bool isInovance = GetSelectedBrand() == PLCBrand.Inovance;
            lbl_inovanceSeries.Visible = isInovance;
            cbo_inovanceSeries.Visible = isInovance;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_device == null)
                return;

            _device.Brand = GetSelectedBrand();
            UpdateInovanceSeriesVisibility();
            if (_device.Brand == PLCBrand.Inovance && (textBox5.Text.Trim() == "44818" || textBox5.Text.Trim() == "5000"))
                textBox5.Text = "502";
            if (_device.Brand == PLCBrand.Mitsubishi && (textBox5.Text.Trim() == "44818" || textBox5.Text.Trim() == "502"))
                textBox5.Text = "5000";
            if (_device.Brand == PLCBrand.Omron && textBox5.Text.Trim() == "9600")
                textBox5.Text = "44818";
            if (_device.Brand == PLCBrand.Inovance)
                _device.InovanceSeries = cbo_inovanceSeries.Text;
            SetPrompt(_device.Brand == PLCBrand.Inovance ? "汇川 " + _device.InovanceSeries + " 使用 Modbus TCP，默认端口 502" : (_device.Brand == PLCBrand.Omron ? "欧姆龙 PLC 使用 EtherNet/IP（CIP），默认端口 44818" : (_device.Brand == PLCBrand.Mitsubishi ? "三菱 PLC 使用 MC Protocol / SLMP TCP，默认端口 5000" : "AB PLC 使用 CIP，默认端口 44818")), Color.Black, false);
        }

        private void cbo_inovanceSeries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_device == null || GetSelectedBrand() != PLCBrand.Inovance)
                return;
            _device.InovanceSeries = cbo_inovanceSeries.Text;
            SetPrompt("已选择汇川 PLC 系列：" + _device.InovanceSeries, Color.Black, false);
        }

        private void UpdateStatus()
        {
            if (_device == null)
            {
                SetPrompt("未选择PLC设备", Color.Red, false);
                return;
            }
            if (_device.connecting)
            {
                SetPrompt("连接中...", Color.Gray, false);
                UpdateConnectionControlState();
                return;
            }
            bool connected = _device.IsConnected;
            SetPrompt(connected ? "已连接" : "未连接", connected ? Color.Green : Color.Red, false);
            UpdateConnectionControlState();
        }

        /// <summary>
        /// 连接建立后锁定通讯参数，避免界面参数与实际连接不一致。
        /// 断开后恢复编辑。
        /// </summary>
        private void UpdateConnectionControlState()
        {
            bool connected = _device != null && _device.IsConnected;
            bool connecting = _device != null && _device.connecting;
            comboBox1.Enabled = !connected && !connecting;
            cbo_inovanceSeries.Enabled = !connected && !connecting;
            textBox4.ReadOnly = connected || connecting;
            textBox5.ReadOnly = connected || connecting;
            btn_connect.Enabled = !connected && !connecting;
            btn_disconnect.Enabled = connected && !connecting;
        }

        private void ApplyConnectionResult(PLCDevice target, bool connected, string errorMessage)
        {
            if (!ReferenceEquals(_device, target))
                return;

            if (connected)
            {
                SetPrompt("连接成功：" + target.IpAddress + ":" + target.Port, Color.Green, true);
                return;
            }

            if (string.IsNullOrEmpty(errorMessage))
                errorMessage = "PLC无响应或通讯参数不正确";
            SetPrompt("连接失败：" + errorMessage, Color.Red, true);
            Frm_MessageBox.Instance.MessageBoxShow("\r\n连接失败：" + errorMessage);
        }

        private void SetPrompt(string msg, Color color, bool writeLog)
        {
            if (lbl_statu.InvokeRequired)
            {
                lbl_statu.BeginInvoke(new Action(() =>
                {
                    lbl_statu.Text = msg;
                    lbl_statu.ForeColor = color;
                    if (writeLog)
                        AppendLog(msg);
                }));
                return;
            }

            lbl_statu.Text = msg;
            lbl_statu.ForeColor = color;
            if (writeLog)
                AppendLog(msg);
        }

        private void AppendLog(string msg)
        {
            string line = DateTime.Now.ToString("HH:mm:ss") + "  " + msg + "\r\n";
            if (textBox1.InvokeRequired)
                textBox1.BeginInvoke(new Action(() => textBox1.AppendText(line)));
            else
                textBox1.AppendText(line);
        }

        private bool TryLoadSelectedDevice()
        {
            try
            {
                var grid = Frm_DeviceManager.Instance.dgv_deviceList;
                string selectedName = string.Empty;

                if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].Tag != null &&
                    grid.SelectedRows[0].Tag.ToString() == "PLCDevice")
                {
                    selectedName = grid.SelectedRows[0].Cells[0].Value.ToString();
                }
                else if (grid.CurrentRow != null && grid.CurrentRow.Tag != null &&
                         grid.CurrentRow.Tag.ToString() == "PLCDevice")
                {
                    selectedName = grid.CurrentRow.Cells[0].Value.ToString();
                }

                if (!string.IsNullOrEmpty(selectedName))
                {
                    for (int i = 0; i < Project.Instance.L_PLCDevice.Count; i++)
                    {
                        if (Project.Instance.L_PLCDevice[i].Name == selectedName)
                        {
                            if (_device == null || _device.Name != selectedName)
                                LoadPar(Project.Instance.L_PLCDevice[i]);
                            return true;
                        }
                    }
                }

                if (_device != null)
                {
                    for (int i = 0; i < Project.Instance.L_PLCDevice.Count; i++)
                    {
                        if (Project.Instance.L_PLCDevice[i].Name == _device.Name)
                            return true;
                    }
                }

                if (!string.IsNullOrEmpty(selectedName))
                {
                    PLCDevice device = new PLCDevice(selectedName);
                    Project.Instance.L_PLCDevice.Add(device);
                    LoadPar(device);
                    SetPrompt("已自动绑定PLC设备：" + selectedName, Color.Black, true);
                    return true;
                }

                if (Project.Instance.L_PLCDevice.Count > 0)
                {
                    LoadPar(Project.Instance.L_PLCDevice[Project.Instance.L_PLCDevice.Count - 1]);
                    SetPrompt("已自动选择PLC设备：" + _device.Name, Color.Black, true);
                    return true;
                }

                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    if (grid.Rows[i].Tag != null && grid.Rows[i].Tag.ToString() == "PLCDevice")
                    {
                        string rowName = grid.Rows[i].Cells[0].Value.ToString();
                        PLCDevice device = new PLCDevice(rowName);
                        Project.Instance.L_PLCDevice.Add(device);
                        LoadPar(device);
                        SetPrompt("已自动绑定PLC设备：" + rowName, Color.Black, true);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }

            return _device != null;
        }

        // -- 连接 --------------------------------------------------

        private void ckb_autoConnectAfterStart_CheckedChanged(object sender, EventArgs e)
        {
            if (_device == null) return;
            _device.AutoConnectAfterStart = ckb_autoConnectAfterStart.Checked;
        }

        private void ckb_autoDisconnectBeforeClose_CheckedChanged(object sender, EventArgs e)
        {
            if (_device == null) return;
            _device.AutoDisconnectBeforeClose = ckb_autoDisconnectBeforeClose.Checked;
        }

        private async void btn_connect_Click(object sender, EventArgs e)
        {
            if (!TryLoadSelectedDevice())
            {
                SetPrompt("未选择PLC设备，请先在左侧选择或新建PLC设备", Color.Red, true);
                return;
            }

            PLCDevice target = _device;

            int port;
            if (!int.TryParse(textBox5.Text.Trim(), out port) || port <= 0 || port > 65535)
            {
                SetPrompt("端口号无效，请输入 1-65535", Color.Red, true);
                Frm_MessageBox.Instance.MessageBoxShow("\r\n端口号无效，请输入 1-65535 之间的端口号");
                return;
            }

            // 更新设备参数
            target.IpAddress = textBox4.Text.Trim();
            target.Port = port;
            target.Brand = GetSelectedBrand();
            if (target.Brand == PLCBrand.Inovance)
                target.InovanceSeries = cbo_inovanceSeries.Text;
            if (string.IsNullOrEmpty(target.IpAddress))
            {
                SetPrompt("IP地址不能为空", Color.Red, true);
                Frm_MessageBox.Instance.MessageBoxShow("\r\nIP地址不能为空");
                return;
            }

            if (target.IsConnected)
            {
                SetPrompt("PLC已连接", Color.Green, true);
                return;
            }

            target.connecting = true;
            UpdateConnectionControlState();
            try
            {
                SetPrompt("连接中...", Color.Gray, true);

                string err = string.Empty;
                bool ok = await Task.Run(() => target.Connect(out err));
                target.connecting = false;
                ApplyConnectionResult(target, ok, err);
            }
            catch (Exception ex)
            {
                if (ReferenceEquals(_device, target))
                {
                    SetPrompt("连接异常：" + ex.Message, Color.Red, true);
                    Frm_MessageBox.Instance.MessageBoxShow("\r\n连接异常：" + ex.Message);
                }
                Log.SaveError(ex);
            }
            finally
            {
                target.connecting = false;
                if (ReferenceEquals(_device, target))
                    UpdateConnectionControlState();
            }
        }

        private void btn_disconnect_Click(object sender, EventArgs e)
        {
            if (!TryLoadSelectedDevice())
            {
                SetPrompt("未选择PLC设备，无法断开", Color.Red, true);
                return;
            }
            if (!_device.IsConnected)
            {
                SetPrompt("PLC未连接，无需断开", Color.Red, true);
                return;
            }
            _device.Disconnect();
            SetPrompt("已断开连接", Color.Red, true);
            UpdateConnectionControlState();
        }

        // -- 读寄存器 ----------------------------------------------

        private void button1_Click(object sender, EventArgs e)
        {
            if (_device == null || !_device.IsConnected)
            {
                Frm_MessageBox.Instance.MessageBoxShow("\r\n尚未连接到PLC，请先连接");
                return;
            }

            string address = textBox2.Text.Trim();
            if (string.IsNullOrEmpty(address))
            {
                Frm_MessageBox.Instance.MessageBoxShow("\r\n请输入寄存器地址");
                return;
            }

            PLCBrand brand = GetSelectedBrand();
            if (brand != PLCBrand.AB && brand != PLCBrand.Inovance &&
                brand != PLCBrand.Omron && brand != PLCBrand.Mitsubishi)
            {
                Frm_MessageBox.Instance.MessageBoxShow(
                    Project.Instance.configuration.language == Language.English
                        ? "Only AB (CIP), Inovance (Modbus TCP), Omron (EtherNet/IP), and Mitsubishi (MC Protocol) PLCs are currently supported."
                        : "\r\n当前支持 AB（CIP）、汇川（Modbus TCP）、欧姆龙（EtherNet/IP）和三菱（MC Protocol）PLC 通讯");
                return;
            }

            try
            {
                string result = ReadFromPlc(address);
                textBox3.Text = result;
                AppendLog("读取 [" + address + "] = " + result);
            }
            catch (Exception ex)
            {
                AppendLog("读取失败：" + ex.Message);
                textBox3.Text = "错误";
            }
        }

        private string ReadFromPlc(string address)
        {
            var comm = _device.Comm;
            switch (comboBox2.Text)
            {
                case "Bit":
                {
                    var r = comm.ReadBool(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content.ToString();
                }
                case "Int16":
                {
                    var r = comm.ReadInt16(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content.ToString();
                }
                case "Int32":
                {
                    var r = comm.ReadInt32(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content.ToString();
                }
                case "Single":
                {
                    var r = comm.ReadFloat(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content.ToString();
                }
                case "Double":
                {
                    var r = comm.ReadDouble(address);
                    if (!r.IsSuccess) throw new Exception(r.Message);
                    return r.Content.ToString();
                }
                default:
                    throw new Exception("未知数据类型");
            }
        }

        // -- 写寄存器 ----------------------------------------------

        private void button2_Click(object sender, EventArgs e)
        {
            if (_device == null || !_device.IsConnected)
            {
                Frm_MessageBox.Instance.MessageBoxShow("\r\n尚未连接到PLC，请先连接");
                return;
            }

            string address = textBox2.Text.Trim();
            if (string.IsNullOrEmpty(address))
            {
                Frm_MessageBox.Instance.MessageBoxShow("\r\n请输入寄存器地址");
                return;
            }

            PLCBrand brand = GetSelectedBrand();
            if (brand != PLCBrand.AB && brand != PLCBrand.Inovance &&
                brand != PLCBrand.Omron && brand != PLCBrand.Mitsubishi)
            {
                Frm_MessageBox.Instance.MessageBoxShow(
                    Project.Instance.configuration.language == Language.English
                        ? "Only AB (CIP), Inovance (Modbus TCP), Omron (EtherNet/IP), and Mitsubishi (MC Protocol) PLCs are currently supported."
                        : "\r\n当前支持 AB（CIP）、汇川（Modbus TCP）、欧姆龙（EtherNet/IP）和三菱（MC Protocol）PLC 通讯");
                return;
            }

            Frm_InputMessage.Instance.lbl_title.Text = "请输入要写入的值";
            Frm_InputMessage.Instance.btn_confirm.Text = "确定";
            Frm_InputMessage.Instance.txt_input.DefaultText = "输入值";
            Frm_InputMessage.Instance.txt_input.TextStr = "";
            Frm_InputMessage.Instance.ShowDialog();
            string valueStr = Frm_InputMessage.input;
            if (string.IsNullOrEmpty(valueStr)) return;

            try
            {
                WriteToPlc(address, valueStr);
                AppendLog("写入 [" + address + "] = " + valueStr + " 成功");
            }
            catch (Exception ex)
            {
                AppendLog("写入失败：" + ex.Message);
                Frm_MessageBox.Instance.MessageBoxShow("\r\n写入失败：" + ex.Message);
            }
        }

        private void WriteToPlc(string address, string valueStr)
        {
            var comm = _device.Comm;
            HslCommunication.OperateResult result;
            switch (comboBox2.Text)
            {
                case "Bit":
                    result = comm.WriteBool(address, valueStr == "1" || valueStr.ToLower() == "true");
                    break;
                case "Int16":
                    result = comm.WriteInt16(address, short.Parse(valueStr));
                    break;
                case "Int32":
                    result = comm.WriteInt32(address, int.Parse(valueStr));
                    break;
                case "Single":
                    result = comm.WriteFloat(address, float.Parse(valueStr));
                    break;
                case "Double":
                    result = comm.WriteDouble(address, double.Parse(valueStr));
                    break;
                default:
                    throw new Exception("未知数据类型");
            }
            if (!result.IsSuccess)
                throw new Exception(result.Message);
        }
    }
}

