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
    internal partial class Frm_CreateROITool : Frm_FormBase
    {
        internal Frm_CreateROITool()
        {
            InitializeComponent();


            regions = new List<ViewWindow.Model.ROI>();

            ho_ModelImage = new HObject();
            //注册窗口鼠标事件
            hWindow_Final1.hWindowControl.MouseUp += Hwindow_MouseUp;

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
                    this.dgv_ROI.Rows[index].Cells[0].Selected = true;

                    regions[index] = roi;
                }
            }
            catch { }
        }
        internal static void binDataGridView(DataGridView dgv, List<ViewWindow.Model.ROI> config)
        {
            try
            {

                dgv.DataSource = null;

                DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
                column1.DataPropertyName = "Type";
                column1.HeaderText = "类型";
                column1.Name = "Type";
                column1.Width = 90;
                column1.ReadOnly = true;

                dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { column1 });
                dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.AllowUserToDeleteRows = true;
                dgv.AllowUserToAddRows = false;
                dgv.MultiSelect = false;
                dgv.AllowUserToAddRows = false;//禁止添加行
                dgv.AllowUserToDeleteRows = true;//禁止删除行
                //dgv.ContextMenuStrip = contextMenuStrip;
                dgv.DataSource = config;
                dgv.Refresh();
                if (config.Count > 0)
                {
                    dgv.Rows[config.Count - 1].Cells[0].Selected = true;
                }

            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 将roi和DataGridView关联显示
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="config"></param>
        private void dgv_ROI_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index;
            string name = "";
            List<double> data;

            hWindow_Final1.viewWindow.selectROI(e.RowIndex);
            ViewWindow.Model.ROI roi = hWindow_Final1.viewWindow.smallestActiveROI(out data, out index);

            if (index > -1)
            {
                name = roi.GetType().Name;
            }

        }
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_CreateROITool _instance;
        public static Frm_CreateROITool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_CreateROITool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static CreateROITool createROITool = new CreateROITool();
        internal static List<ViewWindow.Model.ROI> regions;//roi集合
        HObject ho_ModelImage;

        internal void btn_drawShapeMatchSearchRegion_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Search_Region();
        }
        private void btn_deleteShapeMatchSearchRegion_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Clear_Search_Region();
        }
        private void dgv_matchResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //////shapeMatchTool.Click_Result_Dgv(e);
        }
        private void tkb_contrast_Scroll(object sender, EventArgs e)
        {
            //////shapeMatchTool.Contrast_Changed();
        }

        private void cbo_shapeMatchSearchRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Search_Region();
        }
        private void btn_displayStandardImage_Click(object sender, EventArgs e)
        {
            //////HOperatorSet.DispObj(shapeMatchTool.standardImage, Frm_ImageWindow.Instance.WindowHandle);
        }
        private void btn_displayTemplateContour_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.ShowTemplate();
        }
        private void ckb_shapeMatchToolNotRun_CheckedChanged(object sender, EventArgs e)
        {
            //////Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_createROIToolEnable.Checked;
        }
        private void btn_drawTemplateRegionRectangle1_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Template_Rectangle1();
        }
        private void btn_drawTemplateRegionRectangle2_Click(object sender, EventArgs e)
        {
            //////////shapeMatchTool.Draw_Template_Rectangle2();
        }
        private void btn_drawTemplateRegionCircle_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Template_Circle();
        }
        private void btn_drawTemplateRegionEllipse_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Template_Ellipse();
        }
        private void btn_drawTemplateRegionAny_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.Draw_Template_Any();
        }

        private void tsb_resetTool_Click(object sender, EventArgs e)
        {
            //////shapeMatchTool.ResetTool();
        }

        private void btn_runShapeMatchTool_Click(object sender, EventArgs e)
        {
            //////btn_runDistancePLTool.Enabled = false;
            //////shapeMatchTool.Run(true, jobName);
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runDistancePLTool.Enabled = true;
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            //////btn_runDistancePLTool.Enabled = false;
            //////shapeMatchTool.Run(true, jobName);
            //////if (shapeMatchTool.runStatu != (Project .Instance .configuration .language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Red);
            //////else
            //////    Frm_Main.Instance.OutputMsg(shapeMatchTool.runStatu.ToString(), Color.Green);
            //////btn_runDistancePLTool.Enabled = true;
        }

        private void ckb_leftTopRow_CheckedChanged(object sender, EventArgs e)
        {
            //////if (ckb_leftTopRow.Checked)
            //////{
            //////    createROITool.LeftTopRowUseConst = true;
            //////    createROITool.leftTopRowConstValue = Convert.ToInt16(tbx_leftTopRow.Text);

            //////}
            //////else
            //////{
            //////    createROITool.LeftTopRowUseConst = false ;
            //////}
        }

        private void ckb_leftTopCol_CheckedChanged(object sender, EventArgs e)
        {
            //////if (ckb_leftTopCol.Checked)
            //////{
            //////    createROITool.LeftTopColUseConst = true;
            //////    createROITool.leftTopColConstValue  = Convert.ToInt16(tbx_leftTopCol.Text);

            //////}
            //////else
            //////{
            //////    createROITool.LeftTopColUseConst = false ;
            //////}
        }

        private void ckb_rightDownRow_CheckedChanged(object sender, EventArgs e)
        {
            ////if (ckb_rightDownRow.Checked)
            ////{
            ////    createROITool.RightDownRowUseConst = true;
            ////    createROITool.rightDownRowConstValue  = Convert.ToInt16(tbx_rightDownRow.Text);

            ////}
            ////else
            ////{
            ////    createROITool.RightDownRowUseConst = false ;
            ////}
        }

        private void ckb_rightDownCol_CheckedChanged(object sender, EventArgs e)
        {
            //////if (ckb_rightDownCol.Checked)
            //////{
            //////    createROITool.RightDownColUseConst = true;
            //////    createROITool.rightDownColConstValue = Convert.ToInt16(tbx_rightDownCol.Text);

            //////}
            //////else
            //////{
            //////    createROITool.RightDownColUseConst = false ;
            //////}
        }

        private void tbx_leftTopRow_TextChanged(object sender, EventArgs e)
        {
            ////createROITool.leftTopRow = Convert.ToInt16(tbx_leftTopRow .Text .Trim ());
        }

        private void tbx_leftTopCol_TextChanged(object sender, EventArgs e)
        {
            //createROITool.leftTopCol = Convert.ToInt16(tbx_leftTopCol .Text.Trim ());
        }

        private void tbx_rightDownRow_TextChanged(object sender, EventArgs e)
        {
            //createROITool.rightDownRow = Convert.ToInt16(tbx_rightDownRow .Text .Trim ());

        }

        private void tbx_rightDownCol_TextChanged(object sender, EventArgs e)
        {
            //createROITool.rightDownCol = Convert.ToInt16(tbx_rightDownCol .Text .Trim ());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    FileName = string.Empty,
                    Title = (Project.Instance.configuration.language == Language.English) ? "Please select image path" : "请选择区域文件",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    Filter = (Project.Instance.configuration.language == Language.English) ? "Image File(*.*)|*.*|Image Fie(*.bmp)|*.bmp|Image File(*.tif)|*.tif" : "图像文件(*.hobi)|*.hobj"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    HObject obj2 = new HObject();
                    try
                    {
                        HObject obj3;
                        HOperatorSet.ReadRegion(out obj3, dialog.FileName);
                        createROITool.localRegion = obj3;
                    }
                    catch
                    {
                        Frm_Main.Instance.OutputMsg((Project.Instance.configuration.language == Language.English) ? "Unable to read specified file" : "区域文件异常，无法读取", Color.Red);
                        return;
                    }
                    Frm_Main.Instance.OutputMsg((Project.Instance.configuration.language == Language.English) ? "Loading Image successfully" : "读取区域成功", Color.Black);
                }
            }
            catch (Exception exception)
            {
                Log.SaveError(exception);
            }
        }

        private void Rect1Button_Click(object sender, EventArgs e)
        {

        }

        private void Rect2Button_Click(object sender, EventArgs e)
        {

        }

        private void CircleButton_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void btn_drawTemplateRegionCircle_Click_1(object sender, EventArgs e)
        {
            hWindow_Final1.viewWindow.genInitRect1(ref regions);
            //  hWindow_Fit1.viewWindow.genRect1(110.0, 110.0, 210.0, 210.0, ref this.regions);
            regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, regions);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            hWindow_Final1.viewWindow.genInitRect2(ref regions);
            //  hWindow_Fit1.viewWindow.genRect2(200.0, 200.0, 30.0/180.0*Math.PI, 60.0, 30.0, ref this.regions);
            //设置roi的颜色
            regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, regions);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            hWindow_Final1.viewWindow.genCircle(200.0, 200.0, 60.0, ref regions);
            regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, regions);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            HObject ho_ContOut1;
            HTuple hv_Rows, hv_Cols, hv_Weights;
            HOperatorSet.DrawNurbs(out ho_ContOut1, hWindow_Final1.HWindowHalconID, "true", "true",
               "true", "true", 3, out hv_Rows, out hv_Cols, out hv_Weights);
            // hWindow_Fit1.viewWindow.genInitRect1(ref this.regions);

            hWindow_Final1.viewWindow.genNurbs(hv_Rows, hv_Cols, ref regions);
            regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, regions);
            HOperatorSet.GenRegionPolygonFilled(out ho_ContOut1, hv_Rows, hv_Cols);
            hWindow_Final1.DispObj(ho_ContOut1, "red");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            hWindow_Final1.viewWindow.genCircularArc(100.0, 100.0, 40.0, 0, 5, "positive", ref regions);
            regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, regions);
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void btn_runTool_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            createROITool.Run( true, true,toolName );
            long elapsedTime = sw.ElapsedMilliseconds;


            if (createROITool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            {
                label6.ForeColor = Color.Red;
                label7.Text = string.Format("耗时：0ms");
            }
            else
            {
                label6.ForeColor = Color.Black;
                label7.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());
            }
            label6.Text = "状态：" + createROITool.toolRunStatu.ToString();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            Job.RunAndWaitToCurrentTool(jobName, toolName);
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

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                hWindow_Final1.viewWindow.removeActiveROI(ref regions);
                binDataGridView(this.dgv_ROI, regions);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            hWindow_Final1.DispImageFit();
        }

        private void 显示信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.barVisible_strip_CheckedChanged(sender, e);
        }

    }
}
