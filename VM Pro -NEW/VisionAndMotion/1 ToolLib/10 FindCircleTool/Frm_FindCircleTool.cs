using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VersionMethods;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_FindCircleTool : Frm_FormBase
    {
        /*
         * 圆查找工具界面说明
         *
         * 这个窗体不直接实现找圆算法，它负责三件事：
         * 1. 把 UI 控件的值写入 FindCircleTool 字段，例如 threshold、cliperNum、ringRadiusLength。
         * 2. 在用户拖动预期圆 ROI 后，把最新 ROI 同步回 FindCircleTool.L_regions。
         * 3. 触发 findCircleTool.ShowContour() 做调参预览，或 findCircleTool.Run() 做真实运行。
         *
         * 如果要改算法，优先去 FindCircleTool.cs；如果要改交互、默认显示、按钮行为，再改这里。
         */
        public Frm_FindCircleTool()
        {
            InitializeComponent();
        }

        void textBox1_valueChanged(double value)
        {
            try
            {
                // caliperWidth -> Halcon measure_length2：卡尺沿圆切线方向的宽度。
                // 宽度越大越能平均噪声，但也可能吃到附近其他边。
                findCircleTool.caliperWidth = (int)textBox1.Value;
                trackBar3.Value = findCircleTool.caliperWidth;
                if (!Job.loadForm)
                    findCircleTool.ShowContour(true);
            }
            catch { }
        }

        void tbx_ringRadiusLength_valueChanged(double value)
        {
            try
            {
                // ringRadiusLength -> Halcon measure_length1：卡尺沿半径方向搜索的半长。
                // 预期圆位置不准时加大它；背景复杂时过大容易误抓边。
                findCircleTool.ringRadiusLength = (int)tbx_ringRadiusLength.Value;
                trackBar2.Value = findCircleTool.ringRadiusLength;
                if (!Job.loadForm)
                    findCircleTool.ShowContour(true);
            }
            catch { }
        }

        void tbx_cliperNum_valueChanged(double value)
        {
            try
            {
                // cliperNum -> Halcon num_measures：沿圆周放置多少个卡尺。
                // 数量越多结果越稳定但耗时更高，圆边缺损时也更容易保留足够点。
                findCircleTool.cliperNum = (int)tbx_cliperNum.Value;
                trackBar1.Value = findCircleTool.cliperNum;
                if (!Job.loadForm)
                    findCircleTool.ShowContour(true);
            }
            catch { }
        }

        void numericUpDown2_valueChanged(double value)
        {
            try
            {
                // threshold -> Halcon measure_threshold：边缘强度阈值。
                // 漏检边缘时降低；噪声点太多时升高。
                findCircleTool.threshold = (int)tbx_threshold.Value;
                tkb_exposure.Value = findCircleTool.threshold;
                if (!Job.loadForm)
                    findCircleTool.ShowContour(true);
            }
            catch { }
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_FindCircleTool _instance;
        internal static Frm_FindCircleTool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_FindCircleTool();
                return _instance;
            }
        }
        /// <summary>
        /// 窗体是否已打开（避免在流程运行线程访问 Instance 时误创建隐藏窗体）。
        /// </summary>
        internal static bool IsOpen
        {
            get { return _instance != null && !_instance.IsDisposed && _instance.Visible; }
        }
        internal List<ViewWindow.Model.ROI> regions = new List<ViewWindow.Model.ROI>();
        /// <summary>
        /// 当前工具所对应的工具对象
        /// </summary>
        private static FindCircleTool _cachedFindCircleTool = new FindCircleTool();
        internal static FindCircleTool findCircleTool
        {
            get
            {
                // 工具窗体是单例，但流程里可以有多个"查找圆"工具。
                // 因此每次取 findCircleTool 都优先按 jobName/toolName 找当前绑定的工具对象。
                FindCircleTool bound = Instance.GetBoundTool();
                return bound ?? _cachedFindCircleTool;
            }
            set
            {
                _cachedFindCircleTool = value;
            }
        }

        private FindCircleTool GetBoundTool()
        {
            try
            {
                if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(toolName))
                {
                    return Job.FindToolByName(jobName, toolName) as FindCircleTool;
                }
            }
            catch
            {
            }

            return _cachedFindCircleTool;
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
                    // 用户拖动/缩放 ROI 后，ROI 控制器里的是最新对象。
                    // 这里同步回 regions，后续 findCircleTool.ShowContour/Run 才会使用新圆。
                    string name = roi.GetType().Name;
                    this.regions[index] = roi;
                }
                if (Frm_FindCircleTool.Instance.radioButton1.Checked)
                    // 普通查看模式下，松开鼠标立即刷新卡尺和拟合结果，形成实时调参反馈。
                    findCircleTool.ShowContour(true);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void tsb_resetTool_Click(object sender, EventArgs e)
        {

        }
        private void tsb_help_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }
        private void btn_moveCliperRegion_Click(object sender, EventArgs e)
        {

        }



        private void btn_switchPolarity_Click(object sender, EventArgs e)
        {

        }
        private void cbx_polarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            // polarity 对应 Halcon measure_transition。
            // negative 表示从亮到暗找边，positive 表示从暗到亮找边。
            findCircleTool.polarity = cbx_polarity.SelectedIndex == 0 ? "negative" : "positive";
        }



        private void tbx_threshold_TextChanged(object sender, EventArgs e)
        {

        }
        private void tbx_ringRadiusLength_TextChanged(object sender, EventArgs e)
        {

        }
        private void tbx_cliperNum_TextChanged(object sender, EventArgs e)
        {
            try
            {
                findCircleTool.cliperNum = Convert.ToInt16(tbx_cliperNum.Text.Trim());
            }
            catch
            {
                Frm_Main.Instance.OutputMsg("输入了非法字符，已自动替换为默认值：20", Color.Red);
                tbx_cliperNum.Text = "20";
            }
        }

        internal void btn_runFindCircleTool_Click(object sender, EventArgs e)
        {
            // 单独运行工具：先刷新显示图，再 Run(true, true, toolName)。
            // 第二个参数 runTool=true 表示结果画在本工具窗口，而不是流程主图像窗口。
            findCircleTool.UpdateImage(jobName);
            findCircleTool.Run(true, true, toolName);
            if (findCircleTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Black);
        }
        private void tsb_runOnce_Click(object sender, EventArgs e)
        {
            findCircleTool.UpdateImage(jobName);
            findCircleTool.Run(true, true, toolName);
            if (findCircleTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Black);
        }
        private void ckb_displayCaliper_CheckedChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            // 只影响显示，不影响实际找圆结果。改完立即运行一次，让窗口显示同步。
            findCircleTool.displayCaliper = ckb_displayCaliper.Checked;
            pictureBox3.Image = (ckb_displayCaliper.Checked ? Resources.复选框 : Resources.去复选框);
            findCircleTool.Run(true, true, toolName);
        }
        private void ckb_displayFeature_CheckedChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            findCircleTool.displayFeature = ckb_displayFeature.Checked;
            pictureBox4.Image = (ckb_displayFeature.Checked ? Resources.复选框 : Resources.去复选框);
            findCircleTool.Run(true, true, toolName);
        }

        private void ckb_displayCircle_CheckedChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            findCircleTool.displayCircle = ckb_displayCircle.Checked;
            pictureBox5.Image = (ckb_displayCircle.Checked ? Resources.复选框 : Resources.去复选框);
            findCircleTool.Run(true, true, toolName);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            findCircleTool.displayCircleCenter = checkBox1.Checked;
            pictureBox2.Image = (checkBox1.Checked ? Resources.复选框 : Resources.去复选框);
            findCircleTool.Run(true, true, toolName);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Frm_MessageBox messageBox = new Frm_MessageBox();
            messageBox.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Not yet developed, please wait!" : "\r\n尚未开发，敬请期待！");
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            bool enable = Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable;
            Job.FindJobByName(jobName).FindToolInfoByName(toolName).enable = !enable;
            if (!enable)
                pictureBox8.Image = Resources.开;
            else
                pictureBox8.Image = Resources.关;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            button100.Image = Resources.钉;
            // "运行流程"从第一个工具运行到当前圆查找工具为止，排后面的工具不执行，
            // 这样可以从上游采图/定位工具重新刷新输入，再得到最终圆结果。
            Job.RunAndWaitToCurrentTool(jobName, toolName);
        }

        private void btn_runTool_Click(object sender, EventArgs e)
        {
            // 只运行当前圆查找工具，不触发上游工具。
            // 如果输入图像来自上游且已经变化，应先运行流程或使用 btn_confirm_Click。
            btn_runTool.Enabled = false;
            findCircleTool.Run(true, true, toolName);
            if (findCircleTool.toolRunStatu != (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功))
                Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Red);
            else
                Frm_Main.Instance.OutputMsg(findCircleTool.toolRunStatu.ToString(), Color.Green);
            btn_runTool.Enabled = true;
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            if (findCircleTool.displayCaliper)
            {
                ckb_displayCaliper.Checked = false;
                findCircleTool.displayCaliper = false;
                pictureBox3.Image = (findCircleTool.displayCaliper ? Resources.复选框 : Resources.去复选框);
            }
            else
            {
                ckb_displayCaliper.Checked = true;
                findCircleTool.displayCaliper = true;
                pictureBox3.Image = (findCircleTool.displayCaliper ? Resources.复选框 : Resources.去复选框);
            }
            findCircleTool.Run(true, true, toolName);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            if (findCircleTool.displayFeature)
            {
                ckb_displayFeature.Checked = false;
                findCircleTool.displayFeature = false;
                pictureBox4.Image = (findCircleTool.displayFeature ? Resources.复选框 : Resources.去复选框);
            }
            else
            {
                ckb_displayFeature.Checked = true;
                findCircleTool.displayFeature = true;
                pictureBox4.Image = (findCircleTool.displayFeature ? Resources.复选框 : Resources.去复选框);
            }
            findCircleTool.Run(true, true, toolName);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            if (findCircleTool.displayCircle)
            {
                ckb_displayCircle.Checked = false;
                findCircleTool.displayCircle = false;
                pictureBox5.Image = (findCircleTool.displayCircle ? Resources.复选框 : Resources.去复选框);
            }
            else
            {
                ckb_displayCircle.Checked = true;
                findCircleTool.displayCircle = true;
                pictureBox5.Image = (findCircleTool.displayCircle ? Resources.复选框 : Resources.去复选框);
            }
            findCircleTool.Run(true, true, toolName);
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;
            if (findCircleTool.displayCircleCenter)
            {
                checkBox1.Checked = false;
                findCircleTool.displayCircleCenter = false;
                pictureBox2.Image = (findCircleTool.displayCircleCenter ? Resources.复选框 : Resources.去复选框);
            }
            else
            {
                checkBox1.Checked = true;
                findCircleTool.displayCircleCenter = true;
                pictureBox2.Image = (findCircleTool.displayCircleCenter ? Resources.复选框 : Resources.去复选框);
            }
            findCircleTool.Run(true, true, toolName);
        }

        private void 显示信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.barVisible_strip_CheckedChanged(sender, e);
        }

        private void bx_polarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            findCircleTool.polarity = cbx_polarity.SelectedIndex == 0 ? "negative" : "positive";

        }

        private void Frm_FindCircleTool_Load(object sender, EventArgs e)
        {
            // ROI 拖动完成后的同步入口。注意 MouseMove 中不更新，避免频繁触发 Halcon 重绘。
            hWindow_Final1.hWindowControl.MouseUp += Hwindow_MouseUp;
            hWindow_Final1.EnableImagePan = false;
        }

        private void 适应图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWindow_Final1.DispImageFit();
            findCircleTool.ShowContour(true, true);
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
                        HOperatorSet.WriteImage(findCircleTool.toolPar.InputPar.图像, "tiff", 0, dig_saveImage.FileName);
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

        private void 全屏显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!全屏显示ToolStripMenuItem.Checked)
            {
                全屏显示ToolStripMenuItem.Text = "全屏（ESC退出全屏）";
                panel2.Visible = true;
                hWindow_Final1.Parent = tableLayoutPanel2;
                hWindow_Final1.Dock = DockStyle.Fill;
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                全屏显示ToolStripMenuItem.Text = "退出全屏（ESC退出全屏）";
                hWindow_Final1.Parent = this;
                panel2.Visible = false;
                hWindow_Final1.Dock = DockStyle.Fill;
                hWindow_Final1.m_CtrlHStatusLabelCtrl.BackColor = Color.White;
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void tkb_exposure_Scroll(object sender, EventArgs e)
        {
            tbx_threshold.Value = tkb_exposure.Value;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            tbx_cliperNum.Value = trackBar1.Value;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            tbx_ringRadiusLength.Value = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            textBox1.Value = trackBar3.Value;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // 绘制/编辑预期圆。最终会更新 FindCircleTool.L_regions[0]。
            findCircleTool.DrawExpectCircle(jobName);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // 快捷切换边缘极性。适用于圆边黑白方向选错，卡尺抓不到边的情况。
            if (cbx_polarity.SelectedIndex == 0)
                cbx_polarity.SelectedIndex = 1;
            else
                cbx_polarity.SelectedIndex = 0;

            findCircleTool.polarity = (cbx_polarity.SelectedIndex == 1 ? "positive" : "negative");
            findCircleTool.ShowContour(true);
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            radioButton2.ForeColor = Color.FromArgb(18, 150, 219);
            radioButton1.ForeColor = Color.Black;
            radioButton3.ForeColor = Color.Black;
            radioButton2.Font = new Font(radioButton2.Font.Name, radioButton2.Font.Size, FontStyle.Bold);
            radioButton1.Font = new Font(radioButton1.Font.Name, radioButton1.Font.Size, FontStyle.Regular);
            radioButton3.Font = new Font(radioButton3.Font.Name, radioButton3.Font.Size, FontStyle.Regular);

            findCircleTool.drawMode = true;
            // 进入添加屏蔽区域模式。FindCircleTool.Work() 会阻塞循环读取鼠标，
            // 左键涂抹，直到 drawMode 被 radioButton1_Click 置为 false。
            findCircleTool.Work(sender);
            // button7_Click(null, null);

            //Frm_ShapeMatchTool.Instance.hWindow_Final1.HobjectToHimage(shapeMatchTool.standardImage);
            //findCircleTool.ShowTemplate();
            Frm_ShapeMatchTool.Instance.hWindow_Final1.ContextMenuStrip = Frm_ShapeMatchTool.Instance.cnt_rightClickMenu;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            radioButton1.ForeColor = Color.FromArgb(18, 150, 219);
            radioButton2.ForeColor = Color.Black;
            radioButton3.ForeColor = Color.Black;
            radioButton1.Font = new Font(radioButton1.Font.Name, radioButton1.Font.Size, FontStyle.Bold);
            radioButton2.Font = new Font(radioButton2.Font.Name, radioButton2.Font.Size, FontStyle.Regular);
            radioButton3.Font = new Font(radioButton3.Font.Name, radioButton3.Font.Size, FontStyle.Regular);

            // 退出涂抹模式，Work() 中的 while(drawMode) 会结束。
            findCircleTool.drawMode = false;
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            findCircleTool.ignoreNum = Convert.ToInt16(textBox2.Text.Trim());
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // edgeSelect 对应 Halcon measure_select：
            // first/last/all 决定一个卡尺扫到多条边时选哪条边。
            findCircleTool.edgeSelect = comboBox1.Text;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            findCircleTool.ignoreNum = (int)textBox2.Value;
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            
        }

        private void cbx_polarity_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {

        }

        private void cbx_polarity_Leave(object sender, EventArgs e)
        {
            findCircleTool.polarity = (cbx_polarity.SelectedIndex == 1 ? "positive" : "negative");
        }

        private void tbx_threshold_ValueChanged(double value)
        {
            findCircleTool.threshold = (int)value;
            tkb_exposure.Value = (int)value;
            findCircleTool.ShowContour(true, true);
        }

        private void tbx_cliperNum_ValueChanged_1(double value)
        {
            findCircleTool.cliperNum = (int)value;
            trackBar1.Value = (int)value;
            findCircleTool.ShowContour(true, true);
        }

        private void tbx_ringRadiusLength_ValueChanged_1(double value)
        {
            findCircleTool.ringRadiusLength = (int)value;
            trackBar2.Value = (int)value;
            findCircleTool.ShowContour(true, true);
        }

        private void textBox1_ValueChanged_1(double value)
        {
            findCircleTool.caliperWidth = (int)value;
            trackBar3.Value = (int)value;
            findCircleTool.ShowContour(true, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    comboBox1.SelectedIndex = 1;
                    break;
                case 1:
                    comboBox1.SelectedIndex = 2;
                    break;
                case 2:
                    comboBox1.SelectedIndex = 0;
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    findCircleTool.edgeSelect = "first";
                    break;
                case 1:
                    findCircleTool.edgeSelect = "last";
                    break;
                case 2:
                    findCircleTool.edgeSelect = "all";
                    break;
            }
            findCircleTool.ShowContour(true, true);
        }

        private void textBox2_ValueChanged(double value)
        {
            findCircleTool.ignoreNum = (int)value;
            findCircleTool.ShowContour(true, true);
        }
        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    try
        //    {
        //        if ((keyData & Keys.Escape) == Keys.Escape)
        //        {
        //            if (全屏显示ToolStripMenuItem.Checked)
        //            {
        //                if (全屏显示ToolStripMenuItem.Checked)
        //                {
        //                    全屏显示ToolStripMenuItem.Checked = false;
        //                    全屏显示ToolStripMenuItem.Text = "全屏（ESC退出全屏）";
        //                    panel2.Visible = true;
        //                    hWindow_Final1.Parent = tableLayoutPanel2;
        //                    hWindow_Final1.Dock = DockStyle.Fill;
        //                    this.WindowState = FormWindowState.Normal;
        //                }
        //            }
                 
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.SaveError(ex);
        //        return false;
        //    }
        //}
        private void Frm_FindCircleTool_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void Frm_FindCircleTool_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e .KeyCode  == Keys.Escape)
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

        private void numericUpDown1_ValueChanged(double value)
        {
            findCircleTool.minScore = numericUpDown1.Value;
            findCircleTool.ShowContour(true, true);
        }



    }
}
