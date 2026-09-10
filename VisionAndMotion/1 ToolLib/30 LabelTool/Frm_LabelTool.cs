using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_LabelTool : Frm_FormBase
    {
        public Frm_LabelTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_LabelTool _instance;
        public static Frm_LabelTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_LabelTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static LabelTool labelTool = new LabelTool();


        private void dgv_outputItem2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            labelTool.SaveData2();
        }
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void dgv_outputItem2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                if (Job.loadForm)
                    return;

                Label label = new Label();
                label.Row = "100";
                label.Col = "100";
                label.ExpectValue = "OK";
                label.Incolor = "blue";
                label.OutColor = "blue";
                label.OutputItem = "InputItem1";
                label.Size = "10";
                labelTool.L_label.Add(label);

                dgv_outputItem2.Rows[dgv_outputItem2.Rows.Count - 2].Cells[1].Value = string.Empty;
                dgv_outputItem2.Rows[dgv_outputItem2.Rows.Count - 2].Cells[2].Value = "100";
                dgv_outputItem2.Rows[dgv_outputItem2.Rows.Count - 2].Cells[3].Value = "100";
                dgv_outputItem2.Rows[dgv_outputItem2.Rows.Count - 2].Cells[4].Value = "OK";
                dgv_outputItem2.Rows[dgv_outputItem2.Rows.Count - 2].Cells[5].Value = "blue";
                dgv_outputItem2.Rows[dgv_outputItem2.Rows.Count - 2].Cells[6].Value = "blue";
                dgv_outputItem2.Rows[dgv_outputItem2.Rows.Count - 2].Cells[7].Value = "10";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void ckb_toolEnable_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            labelTool.Run(true, true, toolName);
        }
        private void btn_runTool_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            labelTool.Run(true, true, toolName);
            long elapsedTime = sw.ElapsedMilliseconds;
            label2.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());

            if (labelTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                label1.ForeColor = Color.Red;
            else
                label1.ForeColor = Color.Black;
            label9.Text = "状态：" + labelTool.toolRunStatu.ToString();
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            RefreshTitleButtonVisuals();
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
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
        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.按钮__2_;
            Application.DoEvents();
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonUp;
            Application.DoEvents();
        }

        private void dgv_outputItem_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            labelTool.SaveData2();
        }


    }
}
