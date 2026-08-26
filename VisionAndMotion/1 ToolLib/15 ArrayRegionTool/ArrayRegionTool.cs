using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using HalconDotNet;
using ViewWindow.Model;
using System.Reflection;
using System.ComponentModel;

namespace VMPro
{
    [Serializable]
    internal class ArrayRegionTool : ToolBase
    {
        internal ArrayRegionTool()
        {
            //HOperatorSet.GenEmptyRegion(out _searchRegion);
        }

        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();
        internal HObject outputRegion;
        internal HObject inputImage;
        internal int rowNum = 2;
        internal int colNum = 2;
        internal int rowSpan = 10;
        public int colSpan = 10;
        internal void UpdateRegion()
        {
            try
            {
                GetImageWindowControl().hwc_imageWindow.HobjectToHimage (inputImage);
                double rowStart, colStart, rowEnd, colEnd;
                rowStart = ((ROIRectangle1)this.regions[0]).Row1;
                colStart = ((ROIRectangle1)this.regions[0]).Column1;
                rowEnd = ((ROIRectangle1)this.regions[0]).Row2;
                colEnd = ((ROIRectangle1)this.regions[0]).Column2;
                double dRow = rowSpan * (rowNum - 1);
                double dCol = colSpan * (colNum - 1);
                double dr = rowEnd - rowStart - dRow;
                double dc = colEnd - colStart - dCol;
                double ddr = dr / rowNum;
                double ddc = dc / colNum;
                HObject region = new HObject();
                HOperatorSet.GenEmptyObj(out region);
                for (int i = 0; i < rowNum; i++)
                {
                    for (int j = 0; j < colNum; j++)
                    {
                        HObject temp = new HObject();
                        HOperatorSet.GenEmptyObj(out temp);
                        HOperatorSet.GenRectangle1(out temp, rowStart + ddr * (i) + rowSpan * (i), colStart + ddc * j + colSpan * (j), rowStart + ddr * (i) + rowSpan * (i) + ddr, colStart + ddc * j + colSpan * (j) + ddc);
                        HOperatorSet.Union2(region, temp, out region);
                    }
                }
                outputRegion = region;
                GetImageWindowControl().hwc_imageWindow.DispObj(region);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绘制矩形模板
        /// </summary>
        internal void Draw_Template_Rectangle1()
        {
            try
            {
                //////Frm_Main.Instance.tsb_dragMode.CheckState = CheckState.Checked;
                GetImageWindowControl().hwc_imageWindow.HobjectToHimage (inputImage );
                Frm_ArrayRegionTool.Instance.button1.BackColor = Color.LightGreen;
                GetImageWindowControl().hwc_imageWindow.Focus();
                HOperatorSet.SetDraw(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, "margin");
                if (regions.Count == 0)
                {
                    this.regions.Clear();
                    GetImageWindowControl().hwc_imageWindow.viewWindow.genRect1(200.0, 200.0, 600.0, 800.0, ref this.regions);
                    GetImageWindowControl().regions = this.regions;
                }
                else
                {
                    GetImageWindowControl().hwc_imageWindow.viewWindow.displayROI(regions);
                    GetImageWindowControl().regions = this.regions;
                }

                Frm_ShapeMatchTool.Instance.btn_drawTemplateRegionRectangle1.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private object obj = new object();
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否刷新图像</param>
        public override void Run(bool updateImage, bool b, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;


                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
