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

namespace VMPro
{
    internal partial class Frm_CreateLineTool : Frm_FormBase
    {
        internal Frm_CreateLineTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_CreateLineTool _instance;
        public static Frm_CreateLineTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_CreateLineTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static CreateROITool createROITool = new CreateROITool();


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
           Job.FindJobByName (jobName ).FindToolInfoByName (toolName ).enable   = ckb_createROIToolEnable.Checked;
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
            if (ckb_leftTopRow.Checked)
            {
                createROITool.LeftTopRowUseConst = true;
                createROITool.leftTopRowConstValue = Convert.ToInt16(tbx_leftTopRow.Text);

            }
            else
            {
                createROITool.LeftTopRowUseConst = false ;
            }
        }

        private void ckb_leftTopCol_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_leftTopCol.Checked)
            {
                createROITool.LeftTopColUseConst = true;
                createROITool.leftTopColConstValue  = Convert.ToInt16(tbx_leftTopCol.Text);

            }
            else
            {
                createROITool.LeftTopColUseConst = false ;
            }
        }

        private void ckb_rightDownRow_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_rightDownRow.Checked)
            {
                createROITool.RightDownRowUseConst = true;
                createROITool.rightDownRowConstValue  = Convert.ToInt16(tbx_rightDownRow.Text);

            }
            else
            {
                createROITool.RightDownRowUseConst = false ;
            }
        }

        private void ckb_rightDownCol_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_rightDownCol.Checked)
            {
                createROITool.RightDownColUseConst = true;
                createROITool.rightDownColConstValue = Convert.ToInt16(tbx_rightDownCol.Text);

            }
            else
            {
                createROITool.RightDownColUseConst = false ;
            }
        }

        private void tbx_leftTopRow_TextChanged(object sender, EventArgs e)
        {
            createROITool.leftTopRow = Convert.ToInt16(tbx_leftTopRow .Text .Trim ());
        }

        private void tbx_leftTopCol_TextChanged(object sender, EventArgs e)
        {
            createROITool.leftTopCol = Convert.ToInt16(tbx_leftTopCol .Text.Trim ());
        }

        private void tbx_rightDownRow_TextChanged(object sender, EventArgs e)
        {
            createROITool.rightDownRow = Convert.ToInt16(tbx_rightDownRow .Text .Trim ());

        }

        private void tbx_rightDownCol_TextChanged(object sender, EventArgs e)
        {
            createROITool.rightDownCol = Convert.ToInt16(tbx_rightDownCol .Text .Trim ());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    FileName = string.Empty,
                    Title = (Project .Instance .configuration .language == Language.English) ? "Please select image path" : "请选择区域文件",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    Filter = (Project .Instance .configuration .language == Language.English) ? "Image File(*.*)|*.*|Image Fie(*.bmp)|*.bmp|Image File(*.tif)|*.tif" : "图像文件(*.hobi)|*.hobj"
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
                        Frm_Main.Instance.OutputMsg((Project .Instance .configuration .language == Language.English) ? "Unable to read specified file" : "区域文件异常，无法读取", Color.Red);
                        return;
                    }
                    Frm_Main.Instance.OutputMsg((Project .Instance .configuration .language == Language.English) ? "Loading Image successfully" : "读取区域成功", Color.Green);
                }
            }
            catch (Exception exception)
            {
                Log.SaveError(exception);
            }
        }

    }
}
