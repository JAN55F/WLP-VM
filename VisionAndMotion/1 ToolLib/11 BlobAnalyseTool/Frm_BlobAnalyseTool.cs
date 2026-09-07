using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_BlobAnalyseTool : Frm_FormBase
    {
        internal Frm_BlobAnalyseTool()
        {
            // 初始化子控件时可能触发事件并再次访问 Instance；必须先登记当前实例，
            // 否则会在构造尚未返回前递归创建窗体，最终造成 StackOverflowException。
            if (_instance == null)
                _instance = this;

            InitializeComponent();
            hWindow_Final1.hWindowControl.MouseUp += Hwindow_MouseUp;
            numericUpDown1.ValueChanged  += numericUpDown1_valueChanged;
            numericUpDown2.ValueChanged += numericUpDown2_valueChanged;
        }

        void numericUpDown2_valueChanged(double value)
        {
            blobAnalyseTool.maxThreshold = (int)numericUpDown2.Value;
            trackBar2.Value = blobAnalyseTool.maxThreshold;
            blobAnalyseTool.Run( true, true,toolName );
        }

        void numericUpDown1_valueChanged(double value)
        {
            blobAnalyseTool.minThreshold = (int)numericUpDown1.Value;
            trackBar1.Value = blobAnalyseTool.minThreshold;
            blobAnalyseTool.Run(true, true, toolName);
        }
        /// <summary>
        /// 注册haclon窗体的鼠标弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hwindow_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int index;

                List<double> data;
                ViewWindow.Model.ROI roi = hWindow_Final1.viewWindow.smallestActiveROI(out data, out index);

                if (index > -1)
                {
                    string name = roi.GetType().Name;
                    this.regions[index] = roi;
                    blobAnalyseTool.CaptureTemplatePoseFromCurrentInput();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_BlobAnalyseTool _instance;
        internal static Frm_BlobAnalyseTool Instance
        {
            get
            {
                if (_instance == null)
                    new Frm_BlobAnalyseTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static BlobAnalyseTool blobAnalyseTool = new BlobAnalyseTool();


        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            blobAnalyseTool.ResetTool();
        }
        private void ckb_blobAnalyseToolEnable_CheckedChanged(object sender, EventArgs e)
        {

        }
        public void cbo_blobAnalyseSearchRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            blobAnalyseTool.Draw_Search_Region(jobName);
        }
        private void btn_SearchRegionDelete_Click(object sender, EventArgs e)
        {
        }
        private void ckb_displaySearchRegion_CheckedChanged(object sender, EventArgs e)
        {
            blobAnalyseTool.displaySearchRegion = ckb_displaySearchRegion.Checked;
        }
        private void ckb_displayCross_CheckedChanged(object sender, EventArgs e)
        {
            blobAnalyseTool.displayCross = ckb_displayCross.Checked;
        }
        private void tbx_lineWidth_TextChanged(object sender, EventArgs e)
        {
            try
            {
                blobAnalyseTool.lineWidth = Convert.ToInt16(tbx_lineWidth.Text.Trim());
            }
            catch
            {
                Frm_Main.Instance.OutputMsg("输入了非法字符，已自动替换为默认值：1", Color.Red);
                tbx_lineWidth.Text = "1";
            }
        }
        private void ckb_showResultRegion_CheckedChanged(object sender, EventArgs e)
        {
            blobAnalyseTool.displayRegion = ckb_displayRegion.Checked;
        }
        private void ckb_showOutCircle_CheckedChanged(object sender, EventArgs e)
        {
            blobAnalyseTool.displayOutCircle = ckb_DisplayOutCircle.Checked;
        }
        private void rdo_resultRegionFillMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rdo_regionFillMode.Checked)
                blobAnalyseTool.regionDrawMode = FillMode.Fill;
            else
                blobAnalyseTool.regionDrawMode = FillMode.Margin;
        }
        private void rdo_outCircleFillMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rdo_outCircleFillMode.Checked)
                blobAnalyseTool.outCircleDrawMode = FillMode.Fill;
            else
                blobAnalyseTool.outCircleDrawMode = FillMode.Margin;
        }
        private void dgv_blobAnalyseResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            blobAnalyseTool.Click_Result_List(dgv_result, e);
        }
        private void lbx_preProcessingItem_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgv_processingItem.SelectedRows == null)
                    return;
                if (dgv_processingItem.SelectedRows[0].Cells[0].Value.ToString() == "开运算" || dgv_processingItem.SelectedRows[0].Cells[0].Value.ToString() == "闭运算")
                {
                    Frm_ProcessingItem1.Instance.cbx_elementType.Text = blobAnalyseTool.L_prePorcessing[dgv_processingItem.SelectedRows[0].Index].ElementType;
                    Frm_ProcessingItem1.Instance.tbx_elementSize.Text = blobAnalyseTool.L_prePorcessing[dgv_processingItem.SelectedRows[0].Index].ElementSize.ToString();
                    Frm_ProcessingItem1.Instance.ShowDialog();
                }
                else if (dgv_processingItem.SelectedRows[0].Cells[0].Value.ToString() == "腐蚀" || dgv_processingItem.SelectedRows[0].Cells[0].Value.ToString() == "膨胀")
                {
                    Frm_ProcessingItem.Instance.tbx_minArea.Text = blobAnalyseTool.L_prePorcessing[dgv_processingItem.SelectedRows[0].Index].MinArea.ToString();
                    Frm_ProcessingItem.Instance.tbx_maxArea.Text = blobAnalyseTool.L_prePorcessing[dgv_processingItem.SelectedRows[0].Index].MaxArea.ToString();
                    Frm_ProcessingItem.Instance.tbx_elementSize.Text = blobAnalyseTool.L_prePorcessing[dgv_processingItem.SelectedRows[0].Index].ElementSize.ToString();
                    Frm_ProcessingItem.Instance.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void dgv_preProcessingItem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex != -1)
                {
                    Boolean flag = Convert.ToBoolean(((DataGridViewCheckBoxCell)this.dgv_processingItem.Rows[e.RowIndex].Cells[1]).Value);
                    if (flag == false)
                    {
                        ((DataGridViewCheckBoxCell)this.dgv_processingItem.Rows[e.RowIndex].Cells[1]).Value = true;
                        blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].Enable = true;
                    }
                    else
                    {
                        ((DataGridViewCheckBoxCell)this.dgv_processingItem.Rows[e.RowIndex].Cells[1]).Value = false;
                        blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].Enable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void nud_minThreshold_ValueChanged(object sender, EventArgs e)
        {
            blobAnalyseTool.minThreshold = trackBar1.Value;
        }
        private void nud_maxThreshold_ValueChanged(object sender, EventArgs e)
        {
            blobAnalyseTool.maxThreshold = trackBar2.Value;
        }
        private void cbx_blobAnalyseSearchRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            blobAnalyseTool.Draw_Search_Region(jobName);
        }
        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();
        private void dgv_select_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (Job.loadForm)
                return;
            blobAnalyseTool.SaveSelectItem();
        }
        private void tvw_preProcessingItem_DoubleClick(object sender, EventArgs e)
        {
            blobAnalyseTool.AddProcessingItem();
        }
        private void tsm_deletePreprocessingItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_processingItem.SelectedRows[0].Index != -1)
                {
                    blobAnalyseTool.L_prePorcessing.RemoveAt(dgv_processingItem.SelectedRows[0].Index);
                    dgv_processingItem.Rows.RemoveAt(dgv_processingItem.SelectedRows[0].Index);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_runBlobAnalyseTool_Click(object sender, EventArgs e)
        {

        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {

            btn_runTool.Enabled = false;
            blobAnalyseTool.Run(true, true, toolName);
            if (blobAnalyseTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(blobAnalyseTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(blobAnalyseTool.toolRunStatu.ToString(), Color.Black);
            btn_runTool.Enabled = true;
        }

        private void btn_saveResultRegion_Click(object sender, EventArgs e)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd");
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the region saving path" : "请选择区域文件保存路径";
                dig_saveImage.Filter = "Region File|*.hobj";
                dig_saveImage.InitialDirectory = path;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dig_saveImage.FileName;
                    HOperatorSet.WriteRegion(blobAnalyseTool.outputRegion, new HTuple(dig_saveImage.FileName));
                    Frm_Main.Instance.OutputMsg("Region saved successfully", Color.Black);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void dgv_selectItem_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            blobAnalyseTool.SaveSelectItem();
        }

        /// <summary>
        /// 右键命中的筛选行索引（右键菜单删除用）
        /// </summary>
        private int selectItemRightClickRowIndex = -1;

        private void dgv_selectItem_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                selectItemRightClickRowIndex = -1;
                if (e.Button != MouseButtons.Right || e.RowIndex < 0)
                    return;
                if (dgv_selectItem.Rows[e.RowIndex].IsNewRow)      //新增占位行不可删除
                    return;
                dgv_selectItem.ClearSelection();
                dgv_selectItem.Rows[e.RowIndex].Selected = true;
                selectItemRightClickRowIndex = e.RowIndex;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void cms_deleteSelectItem_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (selectItemRightClickRowIndex < 0 ||
                    selectItemRightClickRowIndex >= dgv_selectItem.Rows.Count ||
                    dgv_selectItem.Rows[selectItemRightClickRowIndex].IsNewRow)
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void tsm_deleteSelectItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectItemRightClickRowIndex < 0 ||
                    selectItemRightClickRowIndex >= dgv_selectItem.Rows.Count ||
                    dgv_selectItem.Rows[selectItemRightClickRowIndex].IsNewRow)
                    return;
                int index = selectItemRightClickRowIndex;
                selectItemRightClickRowIndex = -1;
                dgv_selectItem.EndEdit();
                dgv_selectItem.Rows.RemoveAt(index);
                blobAnalyseTool.SaveSelectItem();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void tsb_runJob_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            blobAnalyseTool.Run(true, true, toolName);
            if (blobAnalyseTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(blobAnalyseTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(blobAnalyseTool.toolRunStatu.ToString(), Color.Green);
            btn_runTool.Enabled = true;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value > trackBar2.Value)
            {
                Frm_Output.Instance.OutputMsg("阈值下限值不可以大于阈值上限值", Color.Red);
                trackBar1.Value = (int)numericUpDown1.Value;
            }
            numericUpDown1.Value = trackBar1.Value;
        }



        private void label3_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value > trackBar2.Value)
            {
                Frm_Output.Instance.OutputMsg("阈值下限值不可以大于阈值上限值", Color.Red);
                trackBar2.Value = (int)numericUpDown2.Value;
            }
            numericUpDown2.Value = trackBar2.Value;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    blobAnalyseTool.sortMode = SortMode.从上至下且从左至右;
                    break;
                case 1:
                    blobAnalyseTool.sortMode = SortMode.从左至右且从上至下;
                    break;
                case 2:
                    blobAnalyseTool.sortMode = SortMode.从上至下且从右至左;
                    break;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            blobAnalyseTool.spanPixelNum = Convert.ToInt16(textBox1.Text.Trim());
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void 适应图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.DispImageFit();
        }

        private void 显示信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.barVisible_strip_CheckedChanged(sender, e);
        }

        private void 全屏显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (全屏显示ToolStripMenuItem.Checked)
            {
                全屏显示ToolStripMenuItem.Text = "退出全屏";
                hWindow_Final1.Parent = this;
                panel3.Visible = false;
                hWindow_Final1.Dock = DockStyle.Fill;
                hWindow_Final1.m_CtrlHStatusLabelCtrl.BackColor = Color.White;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                全屏显示ToolStripMenuItem.Text = "全屏";
                panel3.Visible = true;
                hWindow_Final1.Parent = tableLayoutPanel2;
                hWindow_Final1.Dock = DockStyle.Fill;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void 图像另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //SDK_hikVisionTool.SaveImage();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;
            System.Drawing.Point p = new System.Drawing.Point();
            p.X = this.Location.X + hWindow_Final1.Width + 43;
            p.Y = this.Location.Y + 105;
            contextMenuStrip1.Show(p);
        }

        private void 填充ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreProcessing prePorcessing = new PreProcessing();
            prePorcessing.PreProcessingType = "填充";
            prePorcessing.ElementType = "";
            prePorcessing.ElementSize = 0;
            prePorcessing.Enable = true;
            blobAnalyseTool.L_prePorcessing.Add(prePorcessing);
            int index = Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Add();
            Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[0].Value = "填充";
            ((DataGridViewCheckBoxCell)Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[1]).Value = true;
            Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Count - 1].Selected = true;
        }

        private void 膨胀ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreProcessing prePorcessing = new PreProcessing();
            prePorcessing.PreProcessingType = "膨胀";
            prePorcessing.ElementType = "";
            prePorcessing.Enable = true;
            prePorcessing.ElementSize = 3;
            prePorcessing.MinArea = 100;
            prePorcessing.MaxArea = 10000;
            blobAnalyseTool.L_prePorcessing.Add(prePorcessing);
            int index = Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Add();
            Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[0].Value = "膨胀";
            ((DataGridViewCheckBoxCell)Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[1]).Value = true;
            Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Count - 1].Selected = true;
        }

        private void 腐蚀ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreProcessing prePorcessing = new PreProcessing();
            prePorcessing.PreProcessingType = "腐蚀";
            prePorcessing.ElementType = "";
            prePorcessing.ElementSize = 3;
            prePorcessing.Enable = true;
            prePorcessing.MinArea = 100;
            prePorcessing.MaxArea = 10000;
            blobAnalyseTool.L_prePorcessing.Add(prePorcessing);
            int index = Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Add();
            Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[0].Value = "腐蚀";
            ((DataGridViewCheckBoxCell)Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[index].Cells[1]).Value = true;
            Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows[Frm_BlobAnalyseTool.Instance.dgv_processingItem.Rows.Count - 1].Selected = true;

        }

        private void textBox1_Leave(object sender, EventArgs e)
        {

        }

        private void textBox1_Leave_1(object sender, EventArgs e)
        {
            blobAnalyseTool.spanPixelNum = (int)textBox1.Value;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
        }

        private void numericUpDown1_Load(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            blobAnalyseTool.ClearSearchRegion();
        }

        private void cbx_searchRegionType_SelectedIndexChanged()
        {
            if (Job.loadForm )
                return;
            blobAnalyseTool.Draw_Search_Region(jobName);
        }

    }
}
