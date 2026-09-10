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
using HalconPaint;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace VMPro
{
    internal partial class Frm_MeasurementTool : Frm_FormBase
    {
        internal Frm_MeasurementTool()
        {
            InitializeComponent();
            hWindow_Final1.hWindowControl.MouseUp += Hwindow_MouseUp;
        }
        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_MeasurementTool _instance;
        internal static Frm_MeasurementTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_MeasurementTool();
                return _instance;
            }
        }
        internal override void btn_baseClose_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            shapeMatchTool.drawMode = false;
            base.btn_baseClose_Click(sender, e);

            List<ToolIO> io = Job.FindJobByName(jobName).FindToolInfoByName(toolName).output;
            for (int j = 0; j < io.Count; j++)
            {
                string outputItem = io[j].IOName;

                string[] strs = Regex.Split(outputItem, " . ");
                ToolParBase result3 = shapeMatchTool.toolPar;
                object value = result3;
                for (int k = 0; k < strs.Length; k++)
                {
                    value = Job.FindJobByName(jobName).GetValue(value, strs[k]);
                }
                if (value != null)
                {
                    Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value = value;
                    Job.FindJobByName(jobName).GetToolIONodeByNodeText(toolName, "-->" + outputItem).ToolTipText = Job.FindJobByName(jobName).FormatShowTip(value);
                }
            }




        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static MatchTool shapeMatchTool = new MatchTool();

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
            shapeMatchTool.ClickResultDgv(e);
        }
        private void tkb_contrast_Scroll(object sender, EventArgs e)
        {
            shapeMatchTool.ContrastChanged();
        }
        private void ckb_autoContrast_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void btn_displayStandardImage_Click(object sender, EventArgs e)
        {

        }
        private void btn_displayTemplateContour_Click(object sender, EventArgs e)
        {

        }
        private void btn_drawTemplateRegionRectangle1_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateRectangle1();
            Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
            Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
        }
        private void btn_drawTemplateRegionRectangle2_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateRectangle2();
            Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
            Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
        }
        private void btn_drawTemplateRegionCircle_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateCircle();
            Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
            Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
        }
        private void btn_drawTemplateRegionEllipse_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateEllipse();
            Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
            Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
        }
        private void btn_drawTemplateRegionAny_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawTemplateAny();
            Frm_ShapeMatchTool.Instance.hWindow_Final1.DrawModel = false;
            Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
        }
        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            shapeMatchTool.ResetTool();
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = true;
            this.pictureBox2.Image = Resources.开;
        }
        private void nud_minScore_ValueChanged(object sender, EventArgs e)
        {
            shapeMatchTool.minScore = Convert.ToDouble(nud_minScore.Value);
        }
        private void nud_findResultNum_ValueChanged(object sender, EventArgs e)
        {
            shapeMatchTool.matchNum = Convert.ToInt16(nud_matchNum.Value);
        }
        private void ckb_showCross_CheckedChanged(object sender, EventArgs e)
        {
            shapeMatchTool.showCross = ckb_showCross.Checked;
            pictureBox4.Image = (ckb_showCross.Checked ? Resources.复选框 : Resources.去复选框);
            shapeMatchTool.Run(true, true, toolName);
        }
        private void ckb_showFeature_CheckedChanged(object sender, EventArgs e)
        {
            shapeMatchTool.showFeature = ckb_showFeature.Checked;
            pictureBox5.Image = (ckb_showFeature.Checked ? Resources.复选框 : Resources.去复选框);
            shapeMatchTool.Run(true, true, toolName);
        }
        private void ckb_angleStep_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_autoStep.Checked)
                nud_angleStep.Enabled = false;
            else
                nud_angleStep.Enabled = true;
        }
        private void btn_drawSearchRegion_Click(object sender, EventArgs e)
        {

        }
        private void cbx_searchRegionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            shapeMatchTool.DrawSearchRegion();
        }
        private void btn_deleteSearchRegion_Click(object sender, EventArgs e)
        {
            shapeMatchTool.ClearSearchRegion();
        }

        private void tsb_runJob_Click(object sender, EventArgs e)
        {

        }
        private void tsb_runTool_Click(object sender, EventArgs e)
        {

            btn_runTool.Enabled = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            shapeMatchTool.Run(true, true, toolName);
            long elapsedTime = sw.ElapsedMilliseconds;
            if (shapeMatchTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            {
                Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Red;
                label3.Text = string.Format("耗时：0ms");
            }
            else
            {
                Frm_ShapeMatchTool.Instance.label4.ForeColor = Color.Black;
                label3.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());
            }
            label4.Text = "状态：" + shapeMatchTool.toolRunStatu.ToString();
            btn_runTool.Enabled = true;
        }
        private void btn_runTool_Click(object sender, EventArgs e)
        {

        }
        private void cbx_showTemplate_CheckedChanged(object sender, EventArgs e)
        {
            shapeMatchTool.showTemplate = cbx_showTemplate.Checked;
            pictureBox3.Image = (cbx_showTemplate.Checked ? Resources.复选框 : Resources.去复选框);
            shapeMatchTool.Run(true, true, toolName);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    shapeMatchTool.sortMode = SortMode.从上至下且从左至右;
                    break;
            }
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
            shapeMatchTool.Run(true, true, toolName);
            long elapsedTime = sw.ElapsedMilliseconds;
            label3.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());

            if (shapeMatchTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                label4.ForeColor = Color.Red;
            else
                label4.ForeColor = Color.Black;
            label4.Text = "状态：" + shapeMatchTool.toolRunStatu.ToString();
            btn_runTool.Enabled = true;
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            button100.Image = Resources.钉;
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            shapeMatchTool.drawMode = false;
            this.Close();

            List<ToolIO> io = Job.FindJobByName(jobName).FindToolInfoByName(toolName).output;
            for (int j = 0; j < io.Count; j++)
            {
                string outputItem = io[j].IOName;

                string[] strs = Regex.Split(outputItem, " . ");
                ToolParBase result3 = shapeMatchTool.toolPar;
                object value = result3;
                for (int k = 0; k < strs.Length; k++)
                {
                    value = Job.FindJobByName(jobName).GetValue(value, strs[k]);
                }
                if (value != null)
                {
                    Job.FindJobByName(jobName).FindToolInfoByName(toolName).GetOutput(outputItem).value = value;
                    Job.FindJobByName(jobName).GetToolIONodeByNodeText(toolName, "-->" + outputItem).ToolTipText = Job.FindJobByName(jobName).FormatShowTip(value);
                }
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (shapeMatchTool.showTemplate)
            {
                shapeMatchTool.showTemplate = false;
                pictureBox3.Image = (shapeMatchTool.showTemplate ? Resources.复选框 : Resources.去复选框);
            }
            else
            {
                shapeMatchTool.showTemplate = true;
                pictureBox3.Image = (shapeMatchTool.showTemplate ? Resources.复选框 : Resources.去复选框);
            }
            shapeMatchTool.Run(true, true, toolName);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (shapeMatchTool.showCross)
            {
                shapeMatchTool.showCross = false;
                pictureBox4.Image = (shapeMatchTool.showCross ? Resources.复选框 : Resources.去复选框);
            }
            else
            {
                shapeMatchTool.showCross = true;
                pictureBox4.Image = (shapeMatchTool.showCross ? Resources.复选框 : Resources.去复选框);
            }
            shapeMatchTool.Run(true, true, toolName);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (shapeMatchTool.showFeature)
            {
                shapeMatchTool.showFeature = false;
                pictureBox5.Image = (shapeMatchTool.showFeature ? Resources.复选框 : Resources.去复选框);
            }
            else
            {
                shapeMatchTool.showFeature = true;
                pictureBox5.Image = (shapeMatchTool.showFeature ? Resources.复选框 : Resources.去复选框);
            }
            shapeMatchTool.Run(true, true, toolName);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            shapeMatchTool.ShowTemplate();
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(shapeMatchTool.toolPar.InputPar.图像);
        }

        private void rdo_templateRegionAdd_CheckedChanged(object sender, EventArgs e)
        {
            if (rdo_templateRegionAdd.Checked)
            {
                pictureBox7.Image = (rdo_templateRegionAdd.Checked ? Resources.勾选 : Resources.去勾选);
                pictureBox6.Image = (rdo_templateRegionSub.Checked ? Resources.勾选 : Resources.去勾选);
                rdo_templateRegionAdd.ForeColor = Color.FromArgb(18, 150, 219);
                rdo_templateRegionSub.ForeColor = Color.Black;
                rdo_templateRegionAdd.Font = new Font(rdo_templateRegionAdd.Font.Name, rdo_templateRegionAdd.Font.Size, FontStyle.Bold);
                rdo_templateRegionSub.Font = new Font(rdo_templateRegionSub.Font.Name, rdo_templateRegionSub.Font.Size, FontStyle.Regular);
            }
        }

        private void rdo_templateRegionSub_CheckedChanged(object sender, EventArgs e)
        {
            if (rdo_templateRegionSub.Checked)
            {
                pictureBox6.Image = (rdo_templateRegionSub.Checked ? Resources.勾选 : Resources.去勾选);
                pictureBox7.Image = (rdo_templateRegionAdd.Checked ? Resources.勾选 : Resources.去勾选);
                rdo_templateRegionAdd.ForeColor = Color.Black;
                rdo_templateRegionSub.ForeColor = Color.FromArgb(18, 150, 219);
                rdo_templateRegionAdd.Font = new Font(rdo_templateRegionSub.Font.Name, rdo_templateRegionSub.Font.Size, FontStyle.Regular);
                rdo_templateRegionSub.Font = new Font(rdo_templateRegionAdd.Font.Name, rdo_templateRegionAdd.Font.Size, FontStyle.Bold);
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            rdo_templateRegionAdd.Checked = true;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            rdo_templateRegionSub.Checked = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (shapeMatchTool.templateRegion == null && shapeMatchTool.final_region111 == null)
            {
                label4.ForeColor = Color.Red;
                label4.Text = "状态：未创建模板";
                return;
            }
            Frm_ShapeMatchTool.Instance.button7.Text = "学习中";
            Thread.Sleep(20);
            Application.DoEvents();
            shapeMatchTool.CreateAndShowTemplate();
            shapeMatchTool.ShowTemplate();
            //Frm_ShapeMatchTool.Instance.panel4.Visible = true;
            //Frm_ShapeMatchTool.Instance.panel17.Visible = true;
            //Frm_ShapeMatchTool.Instance.button5.Visible = true;
            //Frm_ShapeMatchTool.Instance.button9.Visible = true;
            radioButton2.Enabled = true;
            radioButton3.Enabled = true;


            Frm_ShapeMatchTool.Instance.button7.Text = "重新学习";

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Frm_MoreEdit.Instance.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawSearchRegion();
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
            shapeMatchTool.spanPixelNum = Convert.ToInt16(numericUpDown4.Value);
        }

        private void tbx_timeout_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            shapeMatchTool.showIndex = checkBox1.Checked;
            pictureBox8.Image = (checkBox1.Checked ? Resources.复选框 : Resources.去复选框);
            shapeMatchTool.Run(true, true, toolName);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (shapeMatchTool.showIndex)
            {
                shapeMatchTool.showIndex = false;
                pictureBox8.Image = (shapeMatchTool.showIndex ? Resources.复选框 : Resources.去复选框);
            }
            else
            {
                shapeMatchTool.showIndex = true;
                pictureBox8.Image = (shapeMatchTool.showIndex ? Resources.复选框 : Resources.去复选框);
            }
            shapeMatchTool.Run(true, true,toolName );
        }


        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            shapeMatchTool.DrawPen(sender);
        }

        private void button4_Click_2(object sender, EventArgs e)
        {
            shapeMatchTool.DrawPen(sender);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void nud_angleStart_ValueChanged(object sender, EventArgs e)
        {
            shapeMatchTool.startAngle = Convert.ToInt16(nud_angleStart.Value);
        }

        private void nud_angleRange_ValueChanged(object sender, EventArgs e)
        {
            shapeMatchTool.angleRange = Convert.ToInt16(nud_angleRange.Value);
        }

        private void 显示信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.barVisible_strip_CheckedChanged(sender, e);

        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            shapeMatchTool.ShowStandardImage();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            shapeMatchTool.ClearSearchRegion();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            try
            {
                radioButton2.ForeColor = Color.FromArgb(18, 150, 219);
                radioButton1.ForeColor = Color.Black;
                radioButton4.ForeColor = Color.Black;
                radioButton3.ForeColor = Color.Black;
                radioButton2.Font = new Font(radioButton2.Font.Name, radioButton2.Font.Size, FontStyle.Bold);
                radioButton1.Font = new Font(radioButton1.Font.Name, radioButton1.Font.Size, FontStyle.Regular);
                radioButton4.Font = new Font(radioButton4.Font.Name, radioButton4.Font.Size, FontStyle.Regular);
                radioButton3.Font = new Font(radioButton3.Font.Name, radioButton3.Font.Size, FontStyle.Regular);

                shapeMatchTool.drawMode = true;
                shapeMatchTool.Work(sender);
                // button7_Click(null, null);

                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(shapeMatchTool.toolPar.InputPar.图像);
                shapeMatchTool.ShowTemplate();
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            try
            {
                radioButton3.ForeColor = Color.FromArgb(18, 150, 219);
                radioButton1.ForeColor = Color.Black;
                radioButton2.ForeColor = Color.Black;
                radioButton2.ForeColor = Color.Black;
                radioButton3.Font = new Font(radioButton3.Font.Name, radioButton3.Font.Size, FontStyle.Bold);
                radioButton1.Font = new Font(radioButton1.Font.Name, radioButton1.Font.Size, FontStyle.Regular);
                radioButton2.Font = new Font(radioButton2.Font.Name, radioButton2.Font.Size, FontStyle.Regular);
                radioButton2.Font = new Font(radioButton2.Font.Name, radioButton2.Font.Size, FontStyle.Regular);

                shapeMatchTool.drawMode = true;
                shapeMatchTool.Work(sender);

                button7_Click(null, null);

                Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(shapeMatchTool.toolPar.InputPar.图像);
                shapeMatchTool.ShowTemplate();
                Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            radioButton1.ForeColor = Color.FromArgb(18, 150, 219);
            radioButton2.ForeColor = Color.Black;
            radioButton4.ForeColor = Color.Black;
            radioButton3.ForeColor = Color.Black;
            radioButton1.Font = new Font(radioButton1.Font.Name, radioButton1.Font.Size, FontStyle.Bold);
            radioButton2.Font = new Font(radioButton2.Font.Name, radioButton2.Font.Size, FontStyle.Regular);
            radioButton3.Font = new Font(radioButton3.Font.Name, radioButton3.Font.Size, FontStyle.Regular);
            radioButton4.Font = new Font(radioButton4.Font.Name, radioButton4.Font.Size, FontStyle.Regular);

            shapeMatchTool.drawMode = false;
            //hWindow_Final1.DispImageFit();
        }

        private void 适应图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.DispImageFit();
        }

        private void Frm_ShapeMatchTool_Load(object sender, EventArgs e)
        {
            //hWindow_Final1.showStatusBar();
        }

        private void 全屏显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (全屏显示ToolStripMenuItem.Checked)
            {
                全屏显示ToolStripMenuItem.Text = "退出全屏（ESC退出全屏）";

                hWindow_Final1.Parent = this;
                panel2.Visible = false;
                hWindow_Final1.Dock = DockStyle.Fill;
                hWindow_Final1.m_CtrlHStatusLabelCtrl.BackColor = Color.White;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                全屏显示ToolStripMenuItem.Text = "全屏（ESC退出全屏）";
                panel2.Visible = true;
                hWindow_Final1.Parent = tableLayoutPanel2;
                hWindow_Final1.Dock = DockStyle.Fill;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void 图像另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.SaveFileDialog dig_saveImage = new System.Windows.Forms.SaveFileDialog();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                dig_saveImage.FileName = DateTime.Now.ToString("yyyy_MM_dd");
                dig_saveImage.Title = Project.Instance.configuration.language == Language.English ? "Please select the image path" : "请选择图像保存路径";
                dig_saveImage.Filter = Project.Instance.configuration.language == Language.English ? "Image File(*.tif)|*.tif|Image File(*.png)|*.png|Image File(*.jpg)|*.jpg|Image File(*.*)|*.*" : "图像文件(*.tif)|*.tif|图像文件(*.png)|*.png|图像文件(*.jpg)|*.jpg|图像文件(*.*)|*.*";
                dig_saveImage.InitialDirectory = path;
                if (dig_saveImage.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dig_saveImage.FileName;
                    try
                    {
                        HOperatorSet.WriteImage(shapeMatchTool.toolPar.InputPar.图像, "tiff", 0, dig_saveImage.FileName);
                    }
                    catch
                    {
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "There's a problem with the file or the path is invalid(ErrorCode:1201)" : "图像文件异常或路径不合法（错误代码：0102）", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            shapeMatchTool.showSearchRegion = checkBox2.Checked;
            pictureBox9.Image = (checkBox2.Checked ? Resources.复选框 : Resources.去复选框);
            shapeMatchTool.Run(true, true, toolName);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = !checkBox2.Checked;
            pictureBox9.Image = (checkBox2.Checked ? Resources.复选框 : Resources.去复选框);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            shapeMatchTool.minScale = Convert.ToDouble(numericUpDown1.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            shapeMatchTool.maxScale = Convert.ToDouble(numericUpDown2.Value);
        }

        private void nud_minScore_Leave(object sender, EventArgs e)
        {
            shapeMatchTool.minScore = (double)nud_minScore.Value;
        }

        private void nud_matchNum_Leave(object sender, EventArgs e)
        {
            shapeMatchTool.matchNum = (int)nud_matchNum.Value;
        }

        private void nud_angleStart_Leave(object sender, EventArgs e)
        {
            shapeMatchTool.startAngle = (int)nud_angleStart.Value;
        }

        private void nud_angleRange_Leave(object sender, EventArgs e)
        {
            shapeMatchTool.angleRange = (int)nud_angleRange.Value;
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            shapeMatchTool.minScale = (double)numericUpDown1.Value;
        }

        private void numericUpDown2_Leave(object sender, EventArgs e)
        {
            shapeMatchTool.maxScale = (double)numericUpDown2.Value;
        }

        private void nud_angleStep_Leave(object sender, EventArgs e)
        {
            shapeMatchTool.angleStep = (int)nud_angleStep.Value;
        }

        private void tbx_timeout_Leave(object sender, EventArgs e)
        {

        }

        private void nud_minScore_Leave_1(object sender, EventArgs e)
        {
            shapeMatchTool.minScore = (double)nud_minScore.Value;
        }

        private void nud_matchNum_Leave_1(object sender, EventArgs e)
        {
        }

        private void button4_Click_3(object sender, EventArgs e)
        {
            Frm_IOConfig.result1 = shapeMatchTool.toolPar;
            Frm_IOConfig.Instance.jobName = this.jobName;
            Frm_IOConfig.Instance.ShowDialog();
        }

        private void numericUpDown5_Leave(object sender, EventArgs e)
        {
            shapeMatchTool.startAngle = (int)nud_angleStart.Value;
        }

        private void numericUpDown5_Leave_1(object sender, EventArgs e)
        {
            shapeMatchTool.angleRange = (int)nud_angleRange.Value;

        }

        private void numericUpDown6_Leave(object sender, EventArgs e)
        {
            shapeMatchTool.minScale = (double)numericUpDown1.Value;
        }

        private void nud_minScore_ValueChanged(double value)
        {
            shapeMatchTool.minScore = value;
        }

        private void nud_matchNum_ValueChanged(double value)
        {
            shapeMatchTool.matchNum = (int)value;
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            radioButton4.ForeColor = Color.FromArgb(18, 150, 219);
            radioButton1.ForeColor = Color.Black;
            radioButton3.ForeColor = Color.Black;
            radioButton2.ForeColor = Color.Black;
            radioButton4.Font = new Font(radioButton2.Font.Name, radioButton2.Font.Size, FontStyle.Bold);
            radioButton1.Font = new Font(radioButton1.Font.Name, radioButton1.Font.Size, FontStyle.Regular);
            radioButton2.Font = new Font(radioButton3.Font.Name, radioButton3.Font.Size, FontStyle.Regular);
            radioButton3.Font = new Font(radioButton3.Font.Name, radioButton3.Font.Size, FontStyle.Regular);

            shapeMatchTool.drawMode = true;
            shapeMatchTool.WorkCreateModel(sender);
            // button7_Click(null, null);

            Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(shapeMatchTool.toolPar.InputPar.图像);
            shapeMatchTool.ShowTemplate();
            Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
        }

        private void Frm_ShapeMatchTool_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (全屏显示ToolStripMenuItem.Checked)
                    {
                        if (全屏显示ToolStripMenuItem.Checked)
                        {
                            全屏显示ToolStripMenuItem.Checked = false;
                            全屏显示ToolStripMenuItem.Text = "全屏（ESC退出全屏）";
                            panel2.Visible = true;
                            hWindow_Final1.Parent = tableLayoutPanel2;
                            hWindow_Final1.Dock = DockStyle.Fill;
                            this.WindowState = FormWindowState.Normal;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void 保存窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            HObject circle;
            HOperatorSet.GenCircle(out circle, 50, 50, 6);
            shapeMatchTool.brush_region = circle;
            shapeMatchTool.brush_region111 = circle;

            button3.BackgroundImage  = Resources.正方形__1_;
            button4.BackgroundImage = Resources.正方形__2_;
            button10.BackgroundImage = Resources.正方形__4_;
            button8.BackgroundImage = Resources.正方形__6_;
        }

        private void button4_Click_4(object sender, EventArgs e)
        {
            HObject circle;
            HOperatorSet.GenCircle(out circle, 50, 50, 16);
            shapeMatchTool.brush_region = circle;
            shapeMatchTool.brush_region111 = circle;

            button3.BackgroundImage = Resources.正方形;
            button4.BackgroundImage = Resources.正方形__3_;
            button10.BackgroundImage = Resources.正方形__4_;
            button8.BackgroundImage = Resources.正方形__6_;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            HObject circle;
            HOperatorSet.GenCircle(out circle, 50, 50, 26);
            shapeMatchTool.brush_region = circle;
            shapeMatchTool.brush_region111 = circle;

            button3.BackgroundImage = Resources.正方形;
            button4.BackgroundImage = Resources.正方形__2_;
            button10.BackgroundImage = Resources.正方形__5_;
            button8.BackgroundImage = Resources.正方形__6_;
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            HObject circle;
            HOperatorSet.GenCircle(out circle, 50, 50, 36);
            shapeMatchTool.brush_region = circle;
            shapeMatchTool.brush_region111 = circle;

            button3.BackgroundImage = Resources.正方形;
            button4.BackgroundImage = Resources.正方形__2_;
            button10.BackgroundImage = Resources.正方形__4_;
            button8.BackgroundImage = Resources.正方形__7_;
        }

        private void cCheckBox1_CheckChanged(bool Checked)
        {
            if (Checked)
            {
                lbl_contastValue.Text = string.Empty;
                tkb_contrast.Enabled = false;
            }
            else
            {
                lbl_contastValue.Text = tkb_contrast.Value.ToString();
                tkb_contrast.Enabled = true;
            }
            shapeMatchTool.ContrastChanged();
        }

        private void cbx_searchRegionType_SelectedIndexChanged()
        {
            shapeMatchTool.DrawSearchRegion();
        }



    }
}
