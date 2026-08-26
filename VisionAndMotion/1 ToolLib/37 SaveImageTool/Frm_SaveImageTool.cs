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
using System.IO;
using VMPro.Properties;
using Ookii.Dialogs.WinForms;

namespace VMPro
{
    internal partial class Frm_SaveImageTool : Frm_FormBase
    {
        internal Frm_SaveImageTool()
        {
            InitializeComponent();
            textBox1.ValueChanged += textBox1_valueChanged;
          
        }

        void textBox1_valueChanged(double value)
        {
            saveImageTool.saveDays = (int)textBox1.Value;
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_SaveImageTool _instance;
        public static Frm_SaveImageTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_SaveImageTool();
                return _instance;
            }
        }
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        internal static SaveImageTool saveImageTool = new SaveImageTool();



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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void ckb_distancePLToolEnable_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
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
        private void Btn_MouseDown3(object sender, MouseEventArgs e)
        {
            PictureBox button = (PictureBox)sender;
            button.Image = Resources.查找__1_;
            Application.DoEvents();
        }

        private void Btn_MouseEnter1(object sender, EventArgs e)
        {
            PictureBox button = (PictureBox)sender;
            button.Image = Resources.查找3;
            Application.DoEvents();
        }

    

        private void pictureBox3_Click(object sender, EventArgs e)
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

        private void btn_runTool_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            saveImageTool.Run(true, true, toolName);
            long elapsedTime = sw.ElapsedMilliseconds;


            if (saveImageTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
            {
                label1.ForeColor = Color.Red;
                label2.Text = string.Format("耗时：0ms");
            }
            else
            {
                label1.ForeColor = Color.Black;
                label2.Text = string.Format("耗时：{0}ms", elapsedTime.ToString());
            }
            label1.Text = "状态：" + saveImageTool.toolRunStatu.ToString();
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



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                saveImageTool.saveDays = Convert.ToInt16(textBox1.Text.Trim());
            }
            catch { }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            saveImageTool.expandTime = checkBox1.Checked;
            pictureBox6.Image = saveImageTool.expandTime ? Resources.复选框 : Resources.去复选框;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            saveImageTool.autoClear = checkBox2.Checked;
            pictureBox4.Image = saveImageTool.autoClear ? Resources.复选框 : Resources.去复选框;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            saveImageTool.imageName = textBox2.Text.Trim();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            saveImageTool.autoCreateDirectory = checkBox3.Checked;
            pictureBox5.Image = saveImageTool.autoCreateDirectory ? Resources.复选框 : Resources.去复选框;
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                Frm_SaveImageTool.Instance.TopMost = false;
                Process.Start(saveImageTool.imageSavePath);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            saveImageTool.ResetTool();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = !checkBox1.Checked;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = !checkBox2.Checked;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            checkBox3.Checked = !checkBox3.Checked;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton1.Checked;

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                saveImageTool.imageSource = (radioButton1.Checked ? ImageSource.InputImage : ImageSource.WindowImage);
                pictureBox8.Image = radioButton1.Checked ? Resources.勾选 : Resources.去勾选;
                pictureBox7.Image = radioButton2.Checked ? Resources.勾选 : Resources.去勾选;
                if (saveImageTool.imageSavePath == "D:\\VM Pro")
                {
                    saveImageTool.imageSavePath += "\\原始图像";
                    tbx_imageSavePath.TextStr = saveImageTool.imageSavePath;
                }
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton2.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                saveImageTool.imageSource = (radioButton2.Checked ? ImageSource.WindowImage : ImageSource.InputImage);
                pictureBox7.Image = radioButton2.Checked ? Resources.勾选 : Resources.去勾选;
                pictureBox8.Image = radioButton1.Checked ? Resources.勾选 : Resources.去勾选;
                if (saveImageTool.imageSavePath ==string .Format ( "D:\\VM Pro\\Image\\{0}\\原始图像",jobName ))
                {
                    saveImageTool.imageSavePath = string.Format("D:\\VM Pro\\Image\\{0}\\结果图像", jobName);
                    tbx_imageSavePath.TextStr = saveImageTool.imageSavePath;
                }
            }
        }

        private void btn_drawTemplateRegionRectangle1_Click(object sender, EventArgs e)
        {
            try
            {
                VistaFolderBrowserDialog _sampleVistaFolderBrowserDialog = new VistaFolderBrowserDialog();
                if (Directory.Exists(saveImageTool.imageSavePath))
                    _sampleVistaFolderBrowserDialog.SelectedPath = saveImageTool.imageSavePath;
                _sampleVistaFolderBrowserDialog.Description = Project.Instance.configuration.language == Language.English ? "Please select image folder" : "请选择图像文件夹路径";
                if (_sampleVistaFolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    saveImageTool.imageSavePath = _sampleVistaFolderBrowserDialog.SelectedPath;
                    tbx_imageSavePath.TextStr = _sampleVistaFolderBrowserDialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void tbx_imageSavePath_Leave(object sender, EventArgs e)
        {
            saveImageTool.imageSavePath = tbx_imageSavePath.TextStr;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            saveImageTool.imageName = textBox2.TextStr;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Directory.Delete(saveImageTool.imageSavePath, true);
        }

    }
}
