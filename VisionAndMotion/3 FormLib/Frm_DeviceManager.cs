using LightController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_DeviceManager : Frm_FormBase
    {
        private static CurForm curForm = CurForm.None;
        internal static bool cancel = false;
        private const string DefaultTcpServerName = "服务端1";
        private const string DefaultTcpServerType = "DefaultTCPSever";
        private ListBox lst_deviceListSimple;
        private ComboBox cbo_deviceTypeSimple;
        private System.Windows.Forms.Label lbl_tipSimple;

        private class SimpleDeviceItem
        {
            internal string DeviceType;
            internal string DeviceName;

            internal SimpleDeviceItem(string deviceType, string deviceName)
            {
                DeviceType = deviceType;
                DeviceName = deviceName;
            }

            public override string ToString()
            {
                return GetDeviceNameDisplayName(DeviceName) + "    [" + GetDeviceTypeDisplayName(DeviceType) + "]";
            }
        }

        private static Frm_DeviceManager _instance;
        internal static Frm_DeviceManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_DeviceManager();
                return _instance;
            }
        }

        internal Frm_DeviceManager()
        {
            InitializeComponent();
            // 此窗口需要支持查看较宽的 PLC 参数和通讯日志，不能沿用设计器中的固定最大尺寸。
            MaximumSize = Size.Empty;
            MinimumSize = new Size(1050, 650);
            BuildSimpleDeviceManagerUi();
            RefreshDeviceList();
        }

        private void BuildSimpleDeviceManagerUi()
        {
            panel3.Controls.Clear();
            panel3.BackColor = Color.FromArgb(248, 250, 252);
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Panel leftPanel = new Panel();
            leftPanel.Location = new Point(10, 12);
            leftPanel.Size = new Size(210, panel3.ClientSize.Height - 24);
            leftPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            leftPanel.BorderStyle = BorderStyle.FixedSingle;
            leftPanel.BackColor = Color.White;
            panel3.Controls.Add(leftPanel);

            System.Windows.Forms.Label title = new System.Windows.Forms.Label();
            title.Text = "  通讯及设备列表";
            title.Location = new Point(0, 0);
            title.Size = new Size(208, 32);
            title.BackColor = Color.FromArgb(26, 106, 175);
            title.ForeColor = Color.White;
            title.Font = new Font("微软雅黑", 9F, FontStyle.Bold);
            title.TextAlign = ContentAlignment.MiddleLeft;
            leftPanel.Controls.Add(title);

            lst_deviceListSimple = new ListBox();
            lst_deviceListSimple.Location = new Point(8, 40);
            lst_deviceListSimple.Size = new Size(192, leftPanel.ClientSize.Height - 121);
            lst_deviceListSimple.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lst_deviceListSimple.IntegralHeight = false;
            lst_deviceListSimple.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            lst_deviceListSimple.BorderStyle = BorderStyle.FixedSingle;
            lst_deviceListSimple.SelectedIndexChanged += lst_deviceListSimple_SelectedIndexChanged;
            leftPanel.Controls.Add(lst_deviceListSimple);

            cbo_deviceTypeSimple = new ComboBox();
            cbo_deviceTypeSimple.DropDownStyle = ComboBoxStyle.DropDownList;
            cbo_deviceTypeSimple.Location = new Point(8, leftPanel.ClientSize.Height - 69);
            cbo_deviceTypeSimple.Size = new Size(118, 25);
            cbo_deviceTypeSimple.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbo_deviceTypeSimple.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            cbo_deviceTypeSimple.Items.Add("PLC");
            cbo_deviceTypeSimple.Items.Add("TCP服务端");
            cbo_deviceTypeSimple.Items.Add("TCP客户端");
            cbo_deviceTypeSimple.Items.Add("串口通讯");
            cbo_deviceTypeSimple.Items.Add("扫码枪");
            cbo_deviceTypeSimple.Items.Add("光源控制器");
            cbo_deviceTypeSimple.SelectedIndex = 0;
            leftPanel.Controls.Add(cbo_deviceTypeSimple);

            Button addButton = new Button();
            addButton.Text = "添加";
            addButton.Location = new Point(132, leftPanel.ClientSize.Height - 70);
            addButton.Size = new Size(68, 28);
            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            addButton.BackColor = Color.FromArgb(18, 150, 219);
            addButton.ForeColor = Color.White;
            addButton.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            addButton.FlatStyle = FlatStyle.Flat;
            addButton.FlatAppearance.BorderSize = 0;
            addButton.TextAlign = ContentAlignment.MiddleCenter;
            addButton.Click += btn_simpleAdd_Click;
            leftPanel.Controls.Add(addButton);

            Button deleteButton = new Button();
            deleteButton.Text = "删除";
            deleteButton.Location = new Point(132, leftPanel.ClientSize.Height - 38);
            deleteButton.Size = new Size(68, 28);
            deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            deleteButton.BackColor = Color.FromArgb(102, 112, 133);
            deleteButton.ForeColor = Color.White;
            deleteButton.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            deleteButton.FlatStyle = FlatStyle.Flat;
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.TextAlign = ContentAlignment.MiddleCenter;
            deleteButton.Click += btn_simpleDelete_Click;
            leftPanel.Controls.Add(deleteButton);

            pnl_formPnl = new Panel();
            pnl_formPnl.Location = new Point(230, 12);
            pnl_formPnl.Size = new Size(panel3.ClientSize.Width - 240, panel3.ClientSize.Height - 70);
            pnl_formPnl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnl_formPnl.BorderStyle = BorderStyle.FixedSingle;
            pnl_formPnl.BackColor = Color.White;
            panel3.Controls.Add(pnl_formPnl);

            lbl_tipSimple = new System.Windows.Forms.Label();
            lbl_tipSimple.AutoSize = true;
            lbl_tipSimple.Location = new Point(230, panel3.ClientSize.Height - 30);
            lbl_tipSimple.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_tipSimple.ForeColor = Color.FromArgb(52, 64, 84);
            lbl_tipSimple.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            lbl_tipSimple.Text = "提示：请选择或添加设备";
            panel3.Controls.Add(lbl_tipSimple);
            lbl_tip = lbl_tipSimple;

            Button closeButton = new Button();
            closeButton.Text = "关闭";
            closeButton.Location = new Point(panel3.ClientSize.Width - 94, panel3.ClientSize.Height - 40);
            closeButton.Size = new Size(94, 31);
            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeButton.BackColor = Color.FromArgb(18, 150, 219);
            closeButton.ForeColor = Color.White;
            closeButton.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.TextAlign = ContentAlignment.MiddleCenter;
            closeButton.UseCompatibleTextRendering = true;
            closeButton.Click += btn_close_Click;
            panel3.Controls.Add(closeButton);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RefreshDeviceList();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
                RefreshDeviceList();
        }

        private void ConfigureDeviceList()
        {
            dgv_deviceList.AutoGenerateColumns = false;
            dgv_deviceList.MultiSelect = false;
            dgv_deviceList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_deviceList.ReadOnly = true;
            dgv_deviceList.AllowUserToAddRows = false;
            dgv_deviceList.AllowUserToDeleteRows = false;
            dgv_deviceList.RowHeadersVisible = false;
            dgv_deviceList.ColumnHeadersVisible = true;
            dgv_deviceList.Columns[0].HeaderText = "名称";
            dgv_deviceList.Columns[1].HeaderText = "类型";
            dgv_deviceList.Columns[2].HeaderText = "状态";
        }

        private void ConfigureDeviceTypeSelector()
        {
            if (cbo_deviceType == null)
                return;

            cbo_deviceType.Items.Clear();
            cbo_deviceType.Items.Add("PLC");
            cbo_deviceType.Items.Add("TCP服务端");
            cbo_deviceType.Items.Add("TCP客户端");
            cbo_deviceType.Items.Add("串口通讯");
            cbo_deviceType.Items.Add("扫码枪");
            cbo_deviceType.Items.Add("光源控制器");
            cbo_deviceType.SelectedIndex = 0;
        }

        private static string GetDeviceTypeDisplayName(string deviceType)
        {
            switch (deviceType)
            {
                case "PLCDevice":
                case "PLC":
                    return "PLC设备";
                case "TCPSever":
                case "TCP Server":
                case "TCP服务端":
                    return "TCP服务端";
                case DefaultTcpServerType:
                    return "默认服务端";
                case "TCPClient":
                case "TCP Client":
                case "TCP客户端":
                    return "TCP客户端";
                case "LightController":
                case "Light":
                case "光源控制器":
                    return "光源控制器";
                case "Scaner":
                case "Scanner":
                case "扫码枪":
                    return "扫码枪";
                case "Serial":
                case "串口通讯":
                    return "串口通讯";
                default:
                    return deviceType;
            }
        }

        private static string GetDeviceNameDisplayName(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
                return deviceName;

            if (deviceName == DefaultTcpServerName)
                return "默认服务端";
            if (deviceName.StartsWith("TCPClient"))
                return "客户端" + deviceName.Substring("TCPClient".Length);
            if (deviceName.StartsWith("TCPServer"))
                return "服务端" + deviceName.Substring("TCPServer".Length);
            if (deviceName.StartsWith("Serial"))
                return "串口" + deviceName.Substring("Serial".Length);
            if (deviceName.StartsWith("Scanner"))
                return "扫码枪" + deviceName.Substring("Scanner".Length);
            if (deviceName.StartsWith("Light"))
                return "光源" + deviceName.Substring("Light".Length);

            return deviceName;
        }

        private static bool IsDefaultTcpServer(TCPSever tcpSever)
        {
            return tcpSever != null && tcpSever.Name == DefaultTcpServerName;
        }

        private void EnsureDefaultTcpServer()
        {
            EnsureDeviceCollections();
            if (FindTcpServer(DefaultTcpServerName) == null)
                Project.Instance.L_TCPSever.Insert(0, new TCPSever(DefaultTcpServerName));
        }

        internal void RefreshDeviceList()
        {
            cancel = true;
            try
            {
                EnsureDeviceCollections();
                EnsureDefaultTcpServer();
                string selectedName = GetSelectedDeviceName();
                string selectedType = GetSelectedDeviceType();

                if (dgv_deviceList != null)
                    dgv_deviceList.Rows.Clear();
                if (lst_deviceListSimple != null)
                    lst_deviceListSimple.Items.Clear();

                AddDeviceRow(DefaultTcpServerType, DefaultTcpServerName);
                for (int i = 0; i < Project.Instance.L_PLCDevice.Count; i++)
                    AddDeviceRow("PLCDevice", Project.Instance.L_PLCDevice[i].Name);
                for (int i = 0; i < Project.Instance.L_TCPSever.Count; i++)
                {
                    if (IsDefaultTcpServer(Project.Instance.L_TCPSever[i]))
                        continue;
                    AddDeviceRow("TCPSever", Project.Instance.L_TCPSever[i].Name);
                }
                for (int i = 0; i < Project.Instance.L_TCPClient.Count; i++)
                    AddDeviceRow("TCPClient", Project.Instance.L_TCPClient[i].Name);
                for (int i = 0; i < Project.Instance.L_lightController.Count; i++)
                    AddDeviceRow("LightController", Project.Instance.L_lightController[i].Name);
                for (int i = 0; i < Project.Instance.L_Scaner.Count; i++)
                    AddDeviceRow("Scaner", Project.Instance.L_Scaner[i].Name);
                for (int i = 0; i < Project.Instance.L_Serial.Count; i++)
                    AddDeviceRow("Serial", Project.Instance.L_Serial[i].Name);

                cancel = false;
                if (!SelectDevice(selectedType, selectedName) && dgv_deviceList != null && dgv_deviceList.Rows.Count > 0)
                    SelectRow(0);
                SetTip(string.Format("设备列表已刷新：默认服务端 1，PLC {0}，TCP服务端 {1}，TCP客户端 {2}，串口 {3}，扫码枪 {4}，光源 {5}",
                    Project.Instance.L_PLCDevice.Count,
                    Math.Max(0, Project.Instance.L_TCPSever.Count - 1),
                    Project.Instance.L_TCPClient.Count,
                    Project.Instance.L_Serial.Count,
                    Project.Instance.L_Scaner.Count,
                    Project.Instance.L_lightController.Count), Color.Green);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                if (lbl_tip != null)
                {
                    lbl_tip.Text = "提示：刷新设备列表失败 - " + ex.Message;
                    lbl_tip.ForeColor = Color.Red;
                }
            }
            finally
            {
                cancel = false;
            }
        }

        private void EnsureDeviceCollections()
        {
            if (Project.Instance.L_PLCDevice == null)
                Project.Instance.L_PLCDevice = new List<PLCDevice>();
            if (Project.Instance.L_TCPSever == null)
                Project.Instance.L_TCPSever = new List<TCPSever>();
            if (Project.Instance.L_TCPClient == null)
                Project.Instance.L_TCPClient = new List<TCPClient>();
            if (Project.Instance.L_lightController == null)
                Project.Instance.L_lightController = new List<LightController_Base>();
            if (Project.Instance.L_Scaner == null)
                Project.Instance.L_Scaner = new List<Scaner>();
            if (Project.Instance.L_Serial == null)
                Project.Instance.L_Serial = new List<Serial>();
        }

        private int GetTotalDeviceCount()
        {
            EnsureDeviceCollections();
            return Project.Instance.L_PLCDevice.Count +
                Project.Instance.L_TCPSever.Count(t => !IsDefaultTcpServer(t)) +
                Project.Instance.L_TCPClient.Count +
                Project.Instance.L_lightController.Count +
                Project.Instance.L_Scaner.Count +
                Project.Instance.L_Serial.Count;
        }

        private int AddDeviceRow(string type, string name)
        {
            if (lst_deviceListSimple != null)
                lst_deviceListSimple.Items.Add(new SimpleDeviceItem(type, name));

            if (dgv_deviceList == null)
                return -1;

            try
            {
                int idx = dgv_deviceList.Rows.Add();
                dgv_deviceList.Rows[idx].Tag = type;
                dgv_deviceList.Rows[idx].Height = 30;
                dgv_deviceList.Rows[idx].Cells[0].Value = name;
                dgv_deviceList.Rows[idx].Cells[1].Value = null;
                dgv_deviceList.Rows[idx].Cells[2].Value = null;
                return idx;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return -1;
            }
        }

        private bool SelectDevice(string type, string name)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name))
                return false;

            if (lst_deviceListSimple != null)
            {
                for (int i = 0; i < lst_deviceListSimple.Items.Count; i++)
                {
                    SimpleDeviceItem item = lst_deviceListSimple.Items[i] as SimpleDeviceItem;
                    if (item != null && item.DeviceType == type && item.DeviceName == name)
                    {
                        lst_deviceListSimple.SelectedIndex = i;
                        ShowSelectedDevice(type, name);
                        return true;
                    }
                }
            }

            for (int i = 0; i < dgv_deviceList.Rows.Count; i++)
            {
                if (dgv_deviceList.Rows[i].Tag != null &&
                    dgv_deviceList.Rows[i].Tag.ToString() == type &&
                    Convert.ToString(dgv_deviceList.Rows[i].Cells[0].Value) == name)
                {
                    SelectRow(i);
                    return true;
                }
            }
            return false;
        }

        private void SelectRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgv_deviceList.Rows.Count)
                return;

            cancel = true;
            dgv_deviceList.ClearSelection();
            dgv_deviceList.CurrentCell = dgv_deviceList.Rows[rowIndex].Cells[0];
            dgv_deviceList.Rows[rowIndex].Selected = true;
            cancel = false;
            ShowSelectedDevice();
        }

        private string GetSelectedDeviceName()
        {
            if (dgv_deviceList.CurrentRow != null)
                return Convert.ToString(dgv_deviceList.CurrentRow.Cells[0].Value);
            if (dgv_deviceList.SelectedRows.Count > 0)
                return Convert.ToString(dgv_deviceList.SelectedRows[0].Cells[0].Value);
            return string.Empty;
        }

        private string GetSelectedDeviceType()
        {
            if (dgv_deviceList.CurrentRow != null && dgv_deviceList.CurrentRow.Tag != null)
                return dgv_deviceList.CurrentRow.Tag.ToString();
            if (dgv_deviceList.SelectedRows.Count > 0 && dgv_deviceList.SelectedRows[0].Tag != null)
                return dgv_deviceList.SelectedRows[0].Tag.ToString();
            return string.Empty;
        }

        private void SetTip(string message, Color color)
        {
            if (lbl_tip == null)
                return;
            lbl_tip.Text = "提示：" + message;
            lbl_tip.ForeColor = color;
        }

        private void ShowSelectedDevice()
        {
            if (cancel)
                return;

            try
            {
                if (lst_deviceListSimple != null && lst_deviceListSimple.SelectedItem is SimpleDeviceItem)
                {
                    SimpleDeviceItem item = (SimpleDeviceItem)lst_deviceListSimple.SelectedItem;
                    ShowSelectedDevice(item.DeviceType, item.DeviceName);
                    return;
                }

                if (dgv_deviceList.CurrentRow == null || dgv_deviceList.CurrentRow.Tag == null)
                {
                    pnl_formPnl.Controls.Clear();
                    curForm = CurForm.None;
                    SetTip("请选择设备", Color.Red);
                    return;
                }

                string deviceType = dgv_deviceList.CurrentRow.Tag.ToString();
                string deviceName = Convert.ToString(dgv_deviceList.CurrentRow.Cells[0].Value);
                ShowSelectedDevice(deviceType, deviceName);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                SetTip("设备加载失败：" + ex.Message, Color.Red);
            }
        }

        private void ShowSelectedDevice(string deviceType, string deviceName)
        {
            switch (deviceType)
            {
                case "PLCDevice":
                    PLCDevice plcDevice = FindPlcDevice(deviceName);
                    if (plcDevice == null)
                    {
                        plcDevice = new PLCDevice(deviceName);
                        Project.Instance.L_PLCDevice.Add(plcDevice);
                    }
                    ShowChildForm(Frm_PLCComm.Instance, CurForm.PLCDevice, DockStyle.Fill);
                    Frm_PLCComm.Instance.LoadPar(plcDevice);
                    SetTip("已选择PLC设备：" + deviceName, Color.Green);
                    break;
                case DefaultTcpServerType:
                    EnsureDefaultTcpServer();
                    TCPSever defaultTcpSever = FindTcpServer(DefaultTcpServerName);
                    if (defaultTcpSever == null) return;
                    ShowChildForm(Frm_TCPServer.Instance, CurForm.TCPSever, DockStyle.Top);
                    Frm_TCPServer.Instance.LoadPar(defaultTcpSever);
                    SetTip("已选择默认TCP服务端：" + DefaultTcpServerName, Color.Green);
                    break;
                case "TCPSever":
                    TCPSever tcpSever = FindTcpServer(deviceName);
                    if (tcpSever == null) return;
                    ShowChildForm(Frm_TCPServer.Instance, CurForm.TCPSever, DockStyle.Top);
                    Frm_TCPServer.Instance.LoadPar(tcpSever);
                    SetTip("已选择TCP服务端：" + deviceName, Color.Green);
                    break;
                case "TCPClient":
                    TCPClient tcpClient = FindTcpClient(deviceName);
                    if (tcpClient == null) return;
                    ShowChildForm(Frm_TCPClient.Instance, CurForm.TCPClient, DockStyle.Top);
                    Frm_TCPClient.Instance.LoadPar(tcpClient);
                    SetTip("已选择TCP客户端：" + deviceName, Color.Green);
                    break;
                case "LightController":
                    LightController_Base lightController = FindLightController(deviceName);
                    if (lightController == null) return;
                    ShowChildForm(Frm_LightController.Instance, CurForm.LightController, DockStyle.Top);
                    Frm_LightController.Instance.LoadPar(lightController);
                    SetTip("已选择光源控制器：" + deviceName, Color.Green);
                    break;
                case "Scaner":
                    Scaner scaner = FindScaner(deviceName);
                    if (scaner == null) return;
                    ShowChildForm(Frm_Scaner.Instance, CurForm.Scaner, DockStyle.Top);
                    Frm_Scaner.Instance.LoadPar(scaner);
                    SetTip("已选择扫码枪：" + deviceName, Color.Green);
                    break;
                case "Serial":
                    Serial serial = FindSerial(deviceName);
                    if (serial == null) return;
                    ShowChildForm(Frm_Serial.Instance, CurForm.Serial, DockStyle.Top);
                    Frm_Serial.Instance.LoadPar(serial);
                    SetTip("已选择串口通讯：" + deviceName, Color.Green);
                    break;
            }
        }

        private void ShowChildForm(Form form, CurForm formType, DockStyle dockStyle)
        {
            if (curForm != formType || !pnl_formPnl.Controls.Contains(form))
            {
                curForm = formType;
                pnl_formPnl.Controls.Clear();
                form.TopLevel = false;
                form.Parent = pnl_formPnl;
                form.Dock = dockStyle;
                form.Show();
            }
            else
            {
                form.Visible = true;
                form.BringToFront();
            }
        }

        private PLCDevice FindPlcDevice(string name)
        {
            for (int i = 0; i < Project.Instance.L_PLCDevice.Count; i++)
                if (Project.Instance.L_PLCDevice[i].Name == name)
                    return Project.Instance.L_PLCDevice[i];
            return null;
        }

        private TCPSever FindTcpServer(string name)
        {
            for (int i = 0; i < Project.Instance.L_TCPSever.Count; i++)
                if (Project.Instance.L_TCPSever[i].Name == name)
                    return Project.Instance.L_TCPSever[i];
            return null;
        }

        private TCPClient FindTcpClient(string name)
        {
            for (int i = 0; i < Project.Instance.L_TCPClient.Count; i++)
                if (Project.Instance.L_TCPClient[i].Name == name)
                    return Project.Instance.L_TCPClient[i];
            return null;
        }

        private LightController_Base FindLightController(string name)
        {
            for (int i = 0; i < Project.Instance.L_lightController.Count; i++)
                if (Project.Instance.L_lightController[i].Name == name)
                    return Project.Instance.L_lightController[i];
            return null;
        }

        private Scaner FindScaner(string name)
        {
            for (int i = 0; i < Project.Instance.L_Scaner.Count; i++)
                if (Project.Instance.L_Scaner[i].Name == name)
                    return Project.Instance.L_Scaner[i];
            return null;
        }

        private Serial FindSerial(string name)
        {
            for (int i = 0; i < Project.Instance.L_Serial.Count; i++)
                if (Project.Instance.L_Serial[i].Name == name)
                    return Project.Instance.L_Serial[i];
            return null;
        }

        private string GetUniqueName(string baseName, Func<string, bool> exists)
        {
            string prefix = baseName.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            string numberText = baseName.Substring(prefix.Length);
            int index;
            if (!int.TryParse(numberText, out index))
                index = 1;

            string name = prefix + index.ToString();
            while (exists(name))
            {
                index++;
                name = prefix + index.ToString();
            }
            return name;
        }

        private int GetAvailableTcpServerPort(int startPort)
        {
            int port = startPort;
            while (Project.Instance.L_TCPSever.Any(s => s != null && s.SeverPort == port))
                port++;
            return port;
        }

        private string InputDeviceName(string title, string defaultText)
        {
            Frm_InputMessage.Instance.lbl_title.Text = title;
            Frm_InputMessage.Instance.btn_confirm.Text = "确定";
            Frm_InputMessage.Instance.txt_input.DefaultText = "请输入自定义的设备名称";
            Frm_InputMessage.Instance.txt_input.TextStr = defaultText;
            Frm_InputMessage.Instance.ShowDialog();
            return Frm_InputMessage.input;
        }

        private void btn_addDevice_MouseEnter(object sender, EventArgs e)
        {
        }

        private void btn_addDevice_Click(object sender, EventArgs e)
        {
            AddSelectedDevice();
        }

        private void btn_simpleAdd_Click(object sender, EventArgs e)
        {
            AddSelectedDevice();
        }

        private void btn_simpleDelete_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleDeviceItem item = lst_deviceListSimple == null ? null : lst_deviceListSimple.SelectedItem as SimpleDeviceItem;
                if (item == null)
                {
                    SetTip("请先选择设备", Color.Red);
                    return;
                }

                DeleteDevice(item.DeviceType, item.DeviceName);
                RefreshDeviceList();
                SetTip("已删除设备：" + item.DeviceName, Color.Green);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                SetTip("删除失败：" + ex.Message, Color.Red);
            }
        }

        private void lst_deviceListSimple_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedDevice();
        }

        private void ShowAddMenu()
        {
            try
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                SetTip("显示添加菜单失败：" + ex.Message, Color.Red);
            }
        }

        private void AddPlcDeviceDirect()
        {
            try
            {
                EnsureDeviceCollections();
                string name = GetUniqueName("PLC1", n => FindPlcDevice(n) != null);
                PLCDevice plcDevice = new PLCDevice(name);
                Project.Instance.L_PLCDevice.Add(plcDevice);
                RefreshDeviceList();
                SelectDevice("PLCDevice", name);
                try
                {
                    Frm_PLCCommTool.Instance.LoadDeviceList();
                }
                catch (Exception ex)
                {
                    Log.SaveError(ex);
                    SetTip("PLC已添加，但刷新PLC工具列表失败：" + ex.Message, Color.OrangeRed);
                    return;
                }
                SetTip("已添加PLC设备：" + name, Color.Green);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                SetTip("PLC添加失败：" + ex.Message, Color.Red);
            }
        }

        private void AddSelectedDevice()
        {
            try
            {
                string type = "PLC";
                if (cbo_deviceTypeSimple != null && cbo_deviceTypeSimple.SelectedItem != null)
                    type = Convert.ToString(cbo_deviceTypeSimple.SelectedItem);
                else if (cbo_deviceType != null && cbo_deviceType.SelectedItem != null)
                    type = Convert.ToString(cbo_deviceType.SelectedItem);

                if (string.IsNullOrEmpty(type))
                    type = "PLC";

                switch (type)
                {
                    case "TCP Server":
                    case "TCP服务端":
                        AddTcpServerDirect();
                        break;
                    case "TCP Client":
                    case "TCP客户端":
                        AddTcpClientDirect();
                        break;
                    case "Serial":
                    case "串口通讯":
                        AddSerialDirect();
                        break;
                    case "Scanner":
                    case "扫码枪":
                        AddScannerDirect();
                        break;
                    case "Light":
                    case "光源控制器":
                        AddLightControllerDirect();
                        break;
                    default:
                        AddPlcDeviceDirect();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                SetTip("添加失败：" + ex.Message, Color.Red);
            }
        }

        private void AddTcpServerDirect()
        {
            EnsureDeviceCollections();
            EnsureDefaultTcpServer();
            string name = GetUniqueName("服务端2", n => FindTcpServer(n) != null);
            TCPSever tcpSever = new TCPSever(name);
            tcpSever.SeverPort = GetAvailableTcpServerPort(10004);
            Project.Instance.L_TCPSever.Add(tcpSever);
            RefreshDeviceList();
            SelectDevice("TCPSever", name);
            SetTip("已添加TCP服务端：" + name, Color.Green);
        }

        private void AddTcpClientDirect()
        {
            EnsureDeviceCollections();
            string name = GetUniqueName("客户端1", n => FindTcpClient(n) != null);
            TCPClient tcpClient = new TCPClient(name);
            Project.Instance.L_TCPClient.Add(tcpClient);
            RefreshDeviceList();
            SelectDevice("TCPClient", name);
            SetTip("已添加TCP客户端：" + name, Color.Green);
        }

        private void AddSerialDirect()
        {
            EnsureDeviceCollections();
            string name = GetUniqueName("串口1", n => FindSerial(n) != null);
            Serial serial = new Serial(name);
            Project.Instance.L_Serial.Add(serial);
            RefreshDeviceList();
            SelectDevice("Serial", name);
            SetTip("已添加串口通讯：" + name, Color.Green);
        }

        private void AddScannerDirect()
        {
            EnsureDeviceCollections();
            string name = GetUniqueName("扫码枪1", n => FindScaner(n) != null);
            Scaner scaner = new Scaner(name);
            Project.Instance.L_Scaner.Add(scaner);
            RefreshDeviceList();
            SelectDevice("Scaner", name);
            SetTip("已添加扫码枪：" + name, Color.Green);
        }

        private void AddLightControllerDirect()
        {
            EnsureDeviceCollections();
            string name = GetUniqueName("光源1", n => FindLightController(n) != null);
            LightController_CST lightController = new LightController_CST();
            lightController.Name = name;
            Project.Instance.L_lightController.Add(lightController);
            RefreshDeviceList();
            SelectDevice("LightController", name);
            SetTip("已添加光源控制器：" + name, Color.Green);
        }

        private void TCPIP服务端ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureDefaultTcpServer();
                string name = InputDeviceName("请输入TCP/IP服务端名称", GetUniqueName("服务端2", n => FindTcpServer(n) != null));
                if (string.IsNullOrEmpty(name)) return;

                TCPSever tcpSever = new TCPSever(name);
                tcpSever.SeverPort = GetAvailableTcpServerPort(10004);
                Project.Instance.L_TCPSever.Add(tcpSever);
                RefreshDeviceList();
                SelectDevice("TCPSever", name);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void TCPIP客户端ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string name = InputDeviceName("请输入TCP/IP客户端名称", GetUniqueName("客户端1", n => FindTcpClient(n) != null));
                if (string.IsNullOrEmpty(name)) return;

                TCPClient tcpClient = new TCPClient(name);
                Project.Instance.L_TCPClient.Add(tcpClient);
                ToolStripItem tsb = new ToolStripStatusLabel("", Resources.客户端1);
                tsb.AutoSize = false;
                tsb.Width = 20;
                tsb.Name = name;
                tsb.ToolTipText = string.Format("名称：{0}\r\n状态：{1}\r\nIP    : {2}\r\nPort : {3}", name, "未连接", "192.168.0.1", 10004);
                Frm_Main.Instance.statusStrip1.Items.Insert(1, tsb);
                Frm_Main.Instance.tss_curTime.BorderSides = ToolStripStatusLabelBorderSides.Left;
                RefreshDeviceList();
                SelectDevice("TCPClient", name);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 奥普特光源控制器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string name = InputDeviceName("请输入光源控制器名称", GetUniqueName("光源控制器1", n => FindLightController(n) != null));
                if (string.IsNullOrEmpty(name)) return;

                LightController_CST lightController = new LightController_CST();
                lightController.Name = name;
                lightController.OpenController();
                Project.Instance.L_lightController.Add(lightController);
                RefreshDeviceList();
                SelectDevice("LightController", name);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 删除toolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_deviceList.CurrentRow == null || dgv_deviceList.CurrentRow.Tag == null)
                    return;

                string deviceType = dgv_deviceList.CurrentRow.Tag.ToString();
                string deviceName = Convert.ToString(dgv_deviceList.CurrentRow.Cells[0].Value);
                DeleteDevice(deviceType, deviceName);
                RefreshDeviceList();
                if (dgv_deviceList.Rows.Count == 0)
                {
                    pnl_formPnl.Controls.Clear();
                    curForm = CurForm.None;
                    SetTip("设备列表为空", Color.Red);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void DeleteDevice(string deviceType, string deviceName)
        {
            if (deviceType == DefaultTcpServerType || deviceName == DefaultTcpServerName)
            {
                SetTip("默认服务端不可删除，可在默认服务端页面中修改IP、端口和自动监听配置", Color.OrangeRed);
                return;
            }

            switch (deviceType)
            {
                case "PLCDevice":
                    PLCDevice plcDevice = FindPlcDevice(deviceName);
                    if (plcDevice != null)
                    {
                        plcDevice.Close();
                        Project.Instance.L_PLCDevice.Remove(plcDevice);
                    }
                    break;
                case "TCPSever":
                    TCPSever tcpSever = FindTcpServer(deviceName);
                    if (tcpSever != null)
                    {
                        tcpSever.Close();
                        Project.Instance.L_TCPSever.Remove(tcpSever);
                    }
                    for (int i = TCPSever.L_STCPSever.Count - 1; i >= 0; i--)
                        if (TCPSever.L_STCPSever[i].severName == deviceName)
                            TCPSever.L_STCPSever.RemoveAt(i);
                    break;
                case "TCPClient":
                    TCPClient tcpClient = FindTcpClient(deviceName);
                    if (tcpClient != null)
                    {
                        tcpClient.Close();
                        Project.Instance.L_TCPClient.Remove(tcpClient);
                    }
                    if (TCPClient.L_socket.ContainsKey(deviceName))
                        TCPClient.L_socket.Remove(deviceName);
                    break;
                case "LightController":
                    LightController_Base lightController = FindLightController(deviceName);
                    if (lightController != null)
                        Project.Instance.L_lightController.Remove(lightController);
                    break;
                case "Scaner":
                    Scaner scaner = FindScaner(deviceName);
                    if (scaner != null)
                        Project.Instance.L_Scaner.Remove(scaner);
                    break;
                case "Serial":
                    Serial serial = FindSerial(deviceName);
                    if (serial != null)
                        Project.Instance.L_Serial.Remove(serial);
                    break;
            }
        }

        private void dgv_deviceList_SelectionChanged(object sender, EventArgs e)
        {
            ShowSelectedDevice();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            curForm = CurForm.None;
            Hide();
        }

        private void dgv_deviceList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            SelectRow(e.RowIndex);
        }

        private void 扫码枪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string name = InputDeviceName("请输入扫码枪名称", GetUniqueName("扫码枪1", n => FindScaner(n) != null));
                if (string.IsNullOrEmpty(name)) return;

                Scaner scaner = new Scaner(name);
                Project.Instance.L_Scaner.Add(scaner);
                RefreshDeviceList();
                SelectDevice("Scaner", name);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 全屏显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string name = InputDeviceName("请输入串口通讯名称", GetUniqueName("串口通讯1", n => FindSerial(n) != null));
                if (string.IsNullOrEmpty(name)) return;

                Serial serial = new Serial(name);
                Project.Instance.L_Serial.Add(serial);
                RefreshDeviceList();
                SelectDevice("Serial", name);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        public enum CurForm
        {
            None,
            Scaner,
            TCPSever,
            TCPClient,
            LightController,
            Serial,
            PLCDevice,
        }

        private void pLC通讯ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string name = InputDeviceName("请输入PLC设备名称", GetUniqueName("PLC1", n => FindPlcDevice(n) != null));
                if (string.IsNullOrEmpty(name)) return;

                PLCDevice plcDevice = new PLCDevice(name);
                Project.Instance.L_PLCDevice.Add(plcDevice);
                try
                {
                    Frm_PLCCommTool.Instance.LoadDeviceList();
                }
                catch (Exception ex)
                {
                    Log.SaveError(ex);
                }
                RefreshDeviceList();
                SelectDevice("PLCDevice", name);
                SetTip("已添加PLC设备：" + name, Color.Green);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                SetTip("PLC添加失败：" + ex.Message, Color.Red);
            }
        }

        private void 实时ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
