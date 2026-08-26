using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using ViewWindow;

namespace VMPro
{
    public partial class Frm_DrawSearchROI : Form
    {
        public Frm_DrawSearchROI(HObject inputImage)
        {
            InitializeComponent();
            hWindow_Fit1.HobjectToHimage(inputImage);
        }

        List<ViewWindow.Model.ROI> regions;//roi集合
        HObject ho_ModelImage;

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {

            //////this.regions = new List<ViewWindow.Model.ROI>();

            ho_ModelImage = new HObject();
            HOperatorSet.ReadImage(out this.ho_ModelImage, "测试.bmp");

            //显示背景图
            hWindow_Fit1.HobjectToHimage(ho_ModelImage);

            ////////注册窗口鼠标事件
            //////hWindow_Fit1.hWindowControl.MouseUp += Hwindow_MouseUp;
        }



        void binDataGridView(DataGridView dgv, List<ViewWindow.Model.ROI> config)
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
        /// 加载图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_LoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnDlg = new OpenFileDialog();
            opnDlg.Filter = "所有图像文件 | *.bmp; *.pcx; *.png; *.jpg; *.gif;" +
                "*.tif; *.ico; *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf";
            opnDlg.Title = "打开图像文件";
            opnDlg.ShowHelp = true;
            opnDlg.Multiselect = false;
            //opnDlg.InitialDirectory = startImagePath;

            if (opnDlg.ShowDialog() == DialogResult.OK)
            {
                string filename = opnDlg.FileName;

                HOperatorSet.GenEmptyObj(out this.ho_ModelImage);
                this.ho_ModelImage.Dispose();
                HOperatorSet.ReadImage(out this.ho_ModelImage, filename);

                //this._windowControl.displayImage(this.ho_ModelImage);
                hWindow_Fit1.viewWindow.displayROI(this.regions);


                HObject reG = new HObject();
                HOperatorSet.GenCrossContourXld(out reG, 500, 500, 1000, 1);
                
                HObject im = new HObject();
               // HOperatorSet.ReadImage(out im, "C:\\Users\\ThinkPad\\Desktop\\temp\\1.png");
                

                HObject rec = new HObject();
                HOperatorSet.GenRectangle1(out rec, 1100, 0, 1600, 200);

                hWindow_Fit1.DispObj(im);
                hWindow_Fit1.DispObj(reG);
                hWindow_Fit1.DispObj(rec);

                reG.Dispose();
                rec.Dispose();

                
            }


        }

#region 绘制roi
       
        private void Rect1Button_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.viewWindow.genRect1(110.0, 110.0, 210.0, 210.0, ref this.regions);
            this.regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, this.regions);
        }

        private void Rect2Button_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.viewWindow.genRect2(200.0, 200.0, 30.0/180.0*Math.PI, 60.0, 30.0, ref this.regions);
            //设置roi的颜色
            this.regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, this.regions);
        }

        private void CircleButton_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.viewWindow.genCircle(200.0, 200.0, 60.0, ref this.regions);
            this.regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, this.regions);
        }

        private void LineButton_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.viewWindow.genLine(100.0, 100.0, 100.0, 200.0, ref this.regions);
            this.regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, this.regions);
        }

        private void DelActROIButton_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.viewWindow.removeActiveROI(ref this.regions);
            binDataGridView(this.dgv_ROI, this.regions);
        }

#endregion

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

            hWindow_Fit1.viewWindow.selectROI(e.RowIndex);
            ViewWindow.Model.ROI roi = hWindow_Fit1.viewWindow.smallestActiveROI(out data, out index);

            if (index > -1)
            {
                name = roi.GetType().Name;
            }

        }


        /// <summary>
        /// 注册haclon窗体的鼠标弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hwindow_MouseUp(object sender, MouseEventArgs e)
        {
            //////int index;

            //////List<double> data;
            //////ViewWindow.Model.ROI roi = hWindow_Fit1.viewWindow.smallestActiveROI(out data, out index);

            //////if (index > -1)
            //////{
            //////    string name = roi.GetType().Name;
            //////    this.dgv_ROI.Rows[index].Cells[0].Selected = true;

            //////    this.regions[index] = roi;
            //////}
        }


        /// <summary>
        /// 禁止缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            hWindow_Fit1.DrawModel = true;
        }

        /// <summary>
        /// 允许缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.DrawModel = false;
        }

        /// <summary>
        /// 显示region
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            HObject region1 = new HObject();
            HOperatorSet.GenRectangle1(out region1,20,20,600,600);
            //默认显示红色的region
            hWindow_Fit1.DispObj(region1);
            region1.Dispose();

            HObject region2 = new HObject();
            HOperatorSet.GenRectangle1(out region2, 600, 600, 900, 900);
            //显示成黄色
            hWindow_Fit1.DispObj(region2,"yellow");
            region2.Dispose();
        }

        /// <summary>
        /// 显示xld
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            HObject cross1 = new HObject();
            HOperatorSet.GenCrossContourXld(out cross1,200,200,300,0);
            //默认显示红色的xld
            hWindow_Fit1.DispObj(cross1);
            cross1.Dispose();


            HObject cross2 = new HObject();
            HOperatorSet.GenCrossContourXld(out cross2, 900, 900, 300, 4);
            //显示成蓝色
            hWindow_Fit1.DispObj(cross2,"blue");
            cross2.Dispose();
        }

        /// <summary>
        /// 显示截图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {

            HObject region1 = new HObject();
            HObject reduce_image = new HObject();

            HOperatorSet.GenRectangle1(out region1, 30, 30, 700, 700);
            HOperatorSet.ReduceDomain(ho_ModelImage, region1, out reduce_image);

            hWindow_Fit1.DispObj(reduce_image);
            region1.Dispose();
            reduce_image.Dispose();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string str = regions[0].Type;
            
           HTuple x= regions[0].getModelData();
           MessageBox.Show(str);
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.EditModel = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.EditModel = false;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.viewWindow.genCircularArc(100.0, 100.0, 40.0, 0, 5, "negative",ref this.regions);
            this.regions.Last().Color = "blue";
            binDataGridView(this.dgv_ROI, this.regions);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            hWindow_Fit1.HobjectToHimage(ho_ModelImage);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }



    }
}
