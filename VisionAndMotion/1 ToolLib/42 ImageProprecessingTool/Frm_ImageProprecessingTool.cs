using HalconDotNet;
using Tool;
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
    internal partial class Frm_ImageProprecessingTool : Frm_FormBase
    {
        internal Frm_ImageProprecessingTool()
        {
            InitializeComponent();
            hWindow_Final1.hWindowControl.MouseUp += Hwindow_MouseUp;
        }
        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_ImageProprecessingTool _instance;
        internal static Frm_ImageProprecessingTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ImageProprecessingTool();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static ImageProprecessingTool imageProprecessingTool = new ImageProprecessingTool();
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
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void dgv_matchResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //shapeMatchTool.ClickResultDgv(e);
        }
        private void tkb_contrast_Scroll(object sender, EventArgs e)
        {
            //shapeMatchTool.ContrastChanged();
        }

        private void btn_displayStandardImage_Click(object sender, EventArgs e)
        {

        }
        private void btn_displayTemplateContour_Click(object sender, EventArgs e)
        {

        }
        private void btn_drawTemplateRegionRectangle1_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.DrawTemplateRectangle1();
        }
        private void btn_drawTemplateRegionRectangle2_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.DrawTemplateRectangle2();
        }
        private void btn_drawTemplateRegionCircle_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.DrawTemplateCircle();
        }
        private void btn_drawTemplateRegionEllipse_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.DrawTemplateEllipse();
        }
        private void btn_drawTemplateRegionAny_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.DrawTemplateAny();
        }
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.ResetTool();
        }
        private void nud_minScore_ValueChanged(object sender, EventArgs e)
        {
            //shapeMatchTool.minScore = Convert.ToDouble(nud_minScore.Value);
        }
        private void nud_findResultNum_ValueChanged(object sender, EventArgs e)
        {
            //shapeMatchTool.matchNum = Convert.ToInt16(nud_matchNum.Value);
        }


        private void btn_drawSearchRegion_Click(object sender, EventArgs e)
        {

        }
        private void cbx_searchRegionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //shapeMatchTool.DrawSearchRegion();
        }
        private void btn_deleteSearchRegion_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.ClearSearchRegion();
        }

        private void tsb_runJob_Click(object sender, EventArgs e)
        {

        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {
            //btn_runTool.Enabled = false;
            //shapeMatchTool.Run(jobName, true, true);
            //if (shapeMatchTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //    Frm_Main.Instance.OutputMsg(shapeMatchTool.toolRunStatu.ToString(), Color.Red);
            //else
            //    Frm_Main.Instance.OutputMsg(shapeMatchTool.toolRunStatu.ToString(), Color.Green);
            //btn_runTool.Enabled = true;
        }



        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = !enable;
            if (!enable)
                pictureBox2.Image = Resources.开;
            else
                pictureBox2.Image = Resources.关;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            btn_runTool.Enabled = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            imageProprecessingTool.Run(true, true, toolName);
            long elapsedTime = sw.ElapsedMilliseconds;
            label3.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());

            if (imageProprecessingTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                label4.ForeColor = Color.Red;
            else
                label4.ForeColor = Color.Black;
            label4.Text = "状态：" + imageProprecessingTool.toolRunStatu.ToString();
            btn_runTool.Enabled = true;
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            //if (shapeMatchTool.showTemplate)
            //{
            //    shapeMatchTool.showTemplate = false;
            //    pictureBox3.Image = (shapeMatchTool.showTemplate ? Resources.复选框 : Resources.去复选框);
            //}
            //else
            //{
            //    shapeMatchTool.showTemplate = true;
            //    pictureBox3.Image = (shapeMatchTool.showTemplate ? Resources.复选框 : Resources.去复选框);
            //}
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //if (shapeMatchTool.showCross)
            //{
            //    shapeMatchTool.showCross = false;
            //    pictureBox4.Image = (shapeMatchTool.showCross ? Resources.复选框 : Resources.去复选框);
            //}
            //else
            //{
            //    shapeMatchTool.showCross = true;
            //    pictureBox4.Image = (shapeMatchTool.showCross ? Resources.复选框 : Resources.去复选框);
            //}
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            //if (shapeMatchTool.showFeature)
            //{
            //    shapeMatchTool.showFeature = false;
            //    pictureBox5.Image = (shapeMatchTool.showFeature ? Resources.复选框 : Resources.去复选框);
            //}
            //else
            //{
            //    shapeMatchTool.showFeature = true;
            //    pictureBox5.Image = (shapeMatchTool.showFeature ? Resources.复选框 : Resources.去复选框);
            //}
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.ShowTemplate();
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(shapeMatchTool.standardImage);
        }






        private void button7_Click(object sender, EventArgs e)
        {
            //Frm_ShapeMatchTool.Instance.button7.Text = "学习中";
            //Application.DoEvents();
            //shapeMatchTool.CreateAndShowTemplate();
            //Frm_ShapeMatchTool.Instance.button7.Text = "学习";

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //shapeMatchTool.DrawSearchRegion();
        }

        private void btn_drawTemplateRegionRectangle2_MouseUp(object sender, MouseEventArgs e)
        {

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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //shapeMatchTool.spanPixelNum  = Convert.ToInt16(textBox1 .Text .Trim ());
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;
            System.Drawing.Point p = new System.Drawing.Point();
            p.X = this.Location.X + hWindow_Final1.Width + 35;
            p.Y = this.Location.Y + 75;
            contextMenuStrip1.Show(p);
        }

        private void 二值化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ItemType itemType = new ItemType("二值化", "二值化1", new Binary());
                imageProprecessingTool.L_item.Add(itemType);

                int idx = dataGridView1.Rows.Add();
                ((DataGridViewCheckBoxCell)dataGridView1.Rows[idx].Cells[0]).Value = true;
                dataGridView1.Rows[idx].Cells[1].Value = "二值化";
                dataGridView1.Rows[idx].Cells[2].Value = "二值化1";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string itemType = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string itemName = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                switch (itemType)
                {
                    case "二值化":
                        Frm_BinaryThreshold.Instance.trackBar1.Value = ((Binary)(imageProprecessingTool.FindItemByName(itemName).item)).lowThreshold;
                        Frm_BinaryThreshold.Instance.trackBar2.Value = ((Binary)(imageProprecessingTool.FindItemByName(itemName).item)).highThreshold;
                        panel4.Controls.Clear();
                        Frm_BinaryThreshold.Instance.TopLevel = false;
                        Frm_BinaryThreshold.Instance.Parent = panel4;
                        Frm_BinaryThreshold.Instance.SetEmbeddedMode(true);
                        // 参数窗体嵌入右下角面板时填满可用区域，避免仍按 264px 固定宽度绘制导致右侧错位。
                        Frm_BinaryThreshold.Instance.Dock = DockStyle.Fill;
                        Frm_BinaryThreshold.Instance.Show();
                        Frm_BinaryThreshold.binary = (Binary)imageProprecessingTool.FindItemByName(itemName).item;
                        Frm_BinaryThreshold.Instance.trackBar1.Value = Frm_BinaryThreshold.binary.lowThreshold;
                        Frm_BinaryThreshold.Instance.trackBar2.Value = Frm_BinaryThreshold.binary.highThreshold;
                        break;
                }

                if (e != null && e.ColumnIndex == 0)
                {
                    string temp = ((DataGridViewCheckBoxCell)Frm_ImageProprecessingTool.Instance.dataGridView1.Rows[e.RowIndex].Cells[0]).Value.ToString();
                    imageProprecessingTool.FindItemByName(itemName).enable = Convert.ToBoolean(temp);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton1_MouseDown(object sender, MouseEventArgs e)
        {
            hWindow_Final1.ClearWindow();
            hWindow_Final1.HobjectToHimage(imageProprecessingTool.inputImage);
        }

        private void toolStripButton1_MouseUp(object sender, MouseEventArgs e)
        {
            hWindow_Final1.ClearWindow();
            hWindow_Final1.HobjectToHimage(imageProprecessingTool.outputImage);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows[0] != null)
                {
                    int idx = dataGridView1.SelectedRows[0].Index;
                    dataGridView1.Rows.RemoveAt(idx);
                    imageProprecessingTool.L_item.RemoveAt(idx);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.SelectedRows[0].Index;
                if (index == 0)
                    return;

                ItemType temp = imageProprecessingTool.L_item[index - 1];
                imageProprecessingTool.L_item[index - 1] = imageProprecessingTool.L_item[index];
                imageProprecessingTool.L_item[index] = temp;

                DataGridViewRow row = dataGridView1.Rows[index];
                dataGridView1.Rows.Remove(row);
                dataGridView1.Rows.Insert(index - 1);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.SelectedRows[0].Index;
                if (index == 0)
                    return;

                ItemType temp = imageProprecessingTool.L_item[index - 1];
                imageProprecessingTool.L_item[index - 1] = imageProprecessingTool.L_item[index];
                imageProprecessingTool.L_item[index] = temp;

                DataGridViewRow row = dataGridView1.Rows[index];
                dataGridView1.Rows.Remove(row);
                dataGridView1.Rows.Insert(index - 1);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 填充ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ItemType itemType = new ItemType("填充", "填充", new Binary());
                imageProprecessingTool.L_item.Add(itemType);

                int idx = dataGridView1.Rows.Add();
                ((DataGridViewCheckBoxCell)dataGridView1.Rows[idx].Cells[0]).Value = true;
                dataGridView1.Rows[idx].Cells[1].Value = "填充";
                dataGridView1.Rows[idx].Cells[2].Value = "填充1";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 膨胀ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }





    }
}
