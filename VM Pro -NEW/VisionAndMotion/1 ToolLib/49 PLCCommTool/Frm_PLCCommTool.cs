using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_PLCCommTool : Frm_FormBase
    {
        // ── 静态字段 ─────────────────────────────────────────────────────────

        /// <summary>当前正在配置的工具对象引用</summary>
        internal static PLCCommTool plcCommTool = new PLCCommTool();

        private static Frm_PLCCommTool _instance;
        public static Frm_PLCCommTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_PLCCommTool();
                return _instance;
            }
        }

        // ── 构造 ─────────────────────────────────────────────────────────────

        internal Frm_PLCCommTool()
        {
            InitializeComponent();
        }

        // ── 加载 ─────────────────────────────────────────────────────────────

        private void Frm_PLCCommTool_Load(object sender, EventArgs e)
        {
            // 初始化设备下拉列表
            LoadDeviceList();

            // 初始化数据类型下拉
            cbo_dataType.Items.Clear();
            cbo_dataType.Items.Add("Bool");
            cbo_dataType.Items.Add("Int16");
            cbo_dataType.Items.Add("Int32");
            cbo_dataType.Items.Add("Float");
            cbo_dataType.Items.Add("Double");
            cbo_dataType.Items.Add("String");

            // 初始化操作模式下拉
            cbo_opMode.Items.Clear();
            cbo_opMode.Items.Add("读取");
            cbo_opMode.Items.Add("写入");
            cbo_opMode.Items.Add("读写");
        }

        // ── 数据加载（外部调用） ────────────────────────────────────────────

        /// <summary>
        /// 外部调用此方法来初始化/刷新表单显示内容
        /// </summary>
        internal void LoadToolData()
        {
            Job.loadForm = true;
            try
            {
                LoadDeviceList();
                plcCommTool.ApplyLastInputIfEmpty();

                // 同步工具对象内容到界面
                lbl_title.Text = toolName;
                cbo_device.Text = plcCommTool.PLCDeviceName;
                txb_address.Text = plcCommTool.Address;
                cbo_dataType.Text = plcCommTool.PLCDataType;
                cbo_opMode.Text = plcCommTool.OpMode;
                txb_writeValue.Text = plcCommTool.toolPar.InputPar.写入值;
                txb_readResult.Text = plcCommTool.toolPar.ResultPar.读取值;
                txb_expectValue.Text = plcCommTool.ExpectValue;
                plcCommTool.ReadValueChanged = OnReadValueChanged;

                UpdateWriteValueVisibility();

                bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
                pic_onOff.Image = enable ? Resources.开 : Resources.关;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
            finally
            {
                Job.loadForm = false;
            }
        }

        // ── 按钮/事件 ─────────────────────────────────────────────────────────

        private void pic_onOff_Click(object sender, EventArgs e)
        {
            if (Job.loadForm) return;
            bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = !enable;
            pic_onOff.Image = !enable ? Resources.开 : Resources.关;
        }

        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = true;
            pic_onOff.Image = Resources.开;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            plcCommTool.SaveLastInput();
            plcCommTool.quitWait = true;
            this.Close();
        }

        private async void btn_runTool_Click(object sender, EventArgs e)
        {
            try
            {
                SyncToolDataFromForm();
                btn_runTool.Enabled = false;
                lbl_toolTip.ForeColor = Color.Black;
                lbl_toolTip.Text = "状态：运行中...";

                Stopwatch sw = new Stopwatch();
                sw.Start();
                await Task.Run(() => plcCommTool.Run(true, true, toolName));
                if (IsDisposed) return;

                long time = sw.ElapsedMilliseconds;
                bool ok = plcCommTool.toolRunStatu == ToolRunStatu.成功 ||
                           plcCommTool.toolRunStatu == ToolRunStatu.Succeed;

                lbl_toolTip.ForeColor = ok ? Color.Black : Color.Red;
                lbl_runTime.Text = ok
                    ? string.Format("耗时：{0}ms", time.ToString())
                    : "耗时：0ms";
                lbl_toolTip.Text = "状态：" + plcCommTool.toolRunStatu.ToString();

                txb_readResult.Text = plcCommTool.toolPar.ResultPar.读取值;
                SyncReadValueToToolOutput();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
            finally
            {
                if (!IsDisposed)
                    btn_runTool.Enabled = true;
            }
        }

        private void SyncToolDataFromForm()
        {
            plcCommTool.PLCDeviceName = cbo_device.Text;
            plcCommTool.Address = txb_address.Text.Trim();
            plcCommTool.PLCDataType = cbo_dataType.Text;
            plcCommTool.OpMode = cbo_opMode.Text;
            plcCommTool.toolPar.InputPar.写入值 = txb_writeValue.Text;
            plcCommTool.ExpectValue = txb_expectValue.Text;
            plcCommTool.SaveLastInput();
        }

        private void SyncReadValueToToolOutput()
        {
            try
            {
                Job job = Job.FindJobByName(jobName);
                if (job == null)
                    return;

                ToolInfo toolInfo = job.FindToolInfoByName(toolName);
                string readValue = plcCommTool.toolPar.ResultPar.读取值;
                for (int i = 0; i < toolInfo.output.Count; i++)
                {
                    string outputItem = toolInfo.output[i].IOName;
                    if (outputItem == "读取值" || outputItem == "ReadValue")
                    {
                        toolInfo.GetOutput(outputItem).value = readValue;
                        TreeNode outputNode = job.GetToolIONodeByNodeText(toolName, "-->" + outputItem);
                        if (outputNode != null)
                            outputNode.ToolTipText = job.FormatShowTip(readValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void btn_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        internal override void btn_baseClose_Click(object sender, EventArgs e)
        {
            try
            {
                plcCommTool.quitWait = true;
                plcCommTool.SaveLastInput();
                base.btn_baseClose_Click(sender, e);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        // ── 控件事件 ─────────────────────────────────────────────────────────

        private void cbo_device_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Job.loadForm) return;
            plcCommTool.PLCDeviceName = cbo_device.Text;
            plcCommTool.SaveLastInput();
        }

        private void txb_address_TextChanged(object sender, EventArgs e)
        {
            if (Job.loadForm) return;
            plcCommTool.Address = txb_address.Text.Trim();
            plcCommTool.SaveLastInput();
        }

        private void cbo_dataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Job.loadForm) return;
            plcCommTool.PLCDataType = cbo_dataType.Text;
            plcCommTool.SaveLastInput();
        }

        private void cbo_opMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Job.loadForm) return;
            plcCommTool.OpMode = cbo_opMode.Text;
            plcCommTool.SaveLastInput();
            UpdateWriteValueVisibility();
        }

        private void txb_writeValue_TextChanged(object sender, EventArgs e)
        {
            if (Job.loadForm) return;
            plcCommTool.toolPar.InputPar.写入值 = txb_writeValue.Text;
            plcCommTool.SaveLastInput();
        }

        private void txb_expectValue_TextChanged(object sender, EventArgs e)
        {
            if (Job.loadForm) return;
            plcCommTool.ExpectValue = txb_expectValue.Text;
            plcCommTool.SaveLastInput();
        }

        private void btn_stopWait_Click(object sender, EventArgs e)
        {
            plcCommTool.quitWait = true;
        }

        // ── 辅助 ─────────────────────────────────────────────────────────────

        private void OnReadValueChanged(string readVal)
        {
            try
            {
                if (IsDisposed)
                    return;

                if (InvokeRequired)
                {
                    if (!IsHandleCreated)
                        return;
                    BeginInvoke(new Action<string>(OnReadValueChanged), readVal);
                    return;
                }

                if (!txb_readResult.Visible)
                    return;

                txb_readResult.Text = readVal;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal void LoadDeviceList()
        {
            string selectedDevice = cbo_device.Text;
            cbo_device.Items.Clear();
            if (Project.Instance.L_PLCDevice == null)
                return;

            for (int i = 0; i < Project.Instance.L_PLCDevice.Count; i++)
            {
                if (Project.Instance.L_PLCDevice[i] != null)
                    cbo_device.Items.Add(Project.Instance.L_PLCDevice[i].Name);
            }

            if (!string.IsNullOrEmpty(selectedDevice) && cbo_device.Items.Contains(selectedDevice))
                cbo_device.Text = selectedDevice;
        }

        private void UpdateWriteValueVisibility()
        {
            string mode = cbo_opMode.Text;
            bool canWrite = mode == "写入" || mode == "读写";
            bool canRead = mode == "读取" || mode == "读写";
            lbl_writeValue.Visible = canWrite;
            txb_writeValue.Visible = canWrite;
            lbl_readResult.Visible = canRead;
            txb_readResult.Visible = canRead;
            lbl_expectValue.Visible = canRead;
            txb_expectValue.Visible = canRead;
            btn_stopWait.Visible = canRead;
        }
    }
}
