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
using WinFormsLabel = System.Windows.Forms.Label;

namespace VMPro
{
    internal partial class Frm_SaveImageTool : Frm_FormBase
    {
        internal Frm_SaveImageTool()
        {
            InitializeComponent();
            textBox1.ValueChanged += textBox1_valueChanged;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            BuildModernLayout();
        }

        private Button enableToggleButton;

        /// <summary>
        /// 将旧版平铺式工具页整理为“来源 / 存储规则 / 文件管理”三块卡片。
        /// 仅重新组织现有控件，原绑定、工具参数与运行入口保持不变。
        /// </summary>
        private void BuildModernLayout()
        {
            SuspendLayout();
            panel3.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();

            Text = "存储图像";
            lbl_title.Text = "存储图像";
            ClientSize = new Size(760, 610);
            MinimumSize = new Size(640, 540);
            panel3.BackColor = ModernUiTheme.Page;
            panel3.Location = new Point(2, 26);
            panel3.Size = new Size(ClientSize.Width - 4, ClientSize.Height - 28);

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.Margin = Padding.Empty;
            tableLayoutPanel1.Padding = Padding.Empty;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 82F));

            pnl_formBox.Controls.Clear();
            pnl_formBox.Dock = DockStyle.Fill;
            pnl_formBox.Margin = Padding.Empty;
            pnl_formBox.Padding = new Padding(18, 16, 18, 8);
            pnl_formBox.BackColor = ModernUiTheme.Page;
            pnl_formBox.AutoScroll = true;

            TableLayoutPanel sections = new TableLayoutPanel();
            sections.Name = "saveImageSections";
            sections.Dock = DockStyle.Top;
            sections.AutoSize = true;
            sections.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            sections.BackColor = ModernUiTheme.Page;
            sections.ColumnCount = 1;
            sections.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            sections.RowCount = 3;
            sections.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            sections.RowStyles.Add(new RowStyle(SizeType.Absolute, 216F));
            sections.RowStyles.Add(new RowStyle(SizeType.Absolute, 170F));

            Panel sourceCard = BuildSourceCard();
            Panel storageCard = BuildStorageCard();
            Panel fileCard = BuildFileCard();
            sourceCard.TabIndex = 0;
            storageCard.TabIndex = 1;
            fileCard.TabIndex = 2;
            sections.Controls.Add(sourceCard, 0, 0);
            sections.Controls.Add(storageCard, 0, 1);
            sections.Controls.Add(fileCard, 0, 2);
            pnl_formBox.Controls.Add(sections);

            BuildFooter();
            tableLayoutPanel1.Controls.Add(pnl_formBox, 0, 0);
            tableLayoutPanel1.Controls.Add(panel6, 0, 1);

            toolStrip2.Visible = false;
            pictureBox2.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
            RefreshEnableState(true);

            tableLayoutPanel1.ResumeLayout(true);
            panel3.ResumeLayout(true);
            ResumeLayout(true);
        }

        private Panel CreateCard(string name)
        {
            Panel card = new Panel();
            card.Name = name;
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0, 0, 0, 12);
            card.Padding = new Padding(18, 14, 18, 12);
            ModernUiTheme.StyleCard(card);
            return card;
        }

        private WinFormsLabel CreateSectionTitle(string text)
        {
            WinFormsLabel title = new WinFormsLabel();
            title.AutoSize = true;
            title.Text = text;
            title.Font = ModernUiTheme.TitleFont;
            title.ForeColor = ModernUiTheme.PrimaryText;
            title.BackColor = Color.Transparent;
            title.Margin = new Padding(0, 0, 12, 0);
            return title;
        }

        private WinFormsLabel CreateHint(string text)
        {
            WinFormsLabel hint = new WinFormsLabel();
            hint.AutoSize = true;
            hint.Text = text;
            hint.Font = ModernUiTheme.UiFont;
            hint.ForeColor = ModernUiTheme.SecondaryText;
            hint.BackColor = Color.Transparent;
            hint.Margin = new Padding(0, 2, 0, 0);
            return hint;
        }

        private Panel BuildSourceCard()
        {
            Panel card = CreateCard("saveImageSourceCard");
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.BackColor = Color.Transparent;
            layout.ColumnCount = 4;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 128F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 128F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowCount = 2;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            layout.Controls.Add(CreateSectionTitle("图像来源"), 0, 0);
            radioButton1.Font = ModernUiTheme.UiFont;
            radioButton2.Font = ModernUiTheme.UiFont;
            radioButton1.TabIndex = 0;
            radioButton2.TabIndex = 1;
            radioButton1.Margin = new Padding(0, 3, 12, 0);
            radioButton2.Margin = new Padding(0, 3, 12, 0);
            layout.Controls.Add(radioButton1, 1, 0);
            layout.Controls.Add(radioButton2, 2, 0);
            WinFormsLabel hint = CreateHint("输入图像适合保存采集原图；窗口图像会保留当前显示与叠加结果。");
            layout.Controls.Add(hint, 0, 1);
            layout.SetColumnSpan(hint, 4);
            card.Controls.Add(layout);
            return card;
        }

        private Panel BuildStorageCard()
        {
            Panel card = CreateCard("saveImageStorageCard");
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.BackColor = Color.Transparent;
            layout.ColumnCount = 4;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));

            WinFormsLabel title = CreateSectionTitle("存储规则");
            WinFormsLabel hint = CreateHint("设置文件位置、命名和保留周期");
            layout.Controls.Add(title, 0, 0);
            layout.Controls.Add(hint, 1, 0);
            layout.SetColumnSpan(hint, 3);

            ConfigureFieldLabel(label3, "存储路径");
            ConfigureFieldLabel(label8, "图像名称");
            ConfigureFieldLabel(label4, "图像格式");
            ConfigureFieldLabel(label5, "保留天数");

            tbx_imageSavePath.MaximumSize = new Size(0, 30);
            tbx_imageSavePath.MinimumSize = new Size(80, 30);
            tbx_imageSavePath.Height = 30;
            tbx_imageSavePath.Dock = DockStyle.None;
            tbx_imageSavePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbx_imageSavePath.Margin = new Padding(0, 5, 8, 5);
            tbx_imageSavePath.TabIndex = 0;
            btn_drawTemplateRegionRectangle1.Text = "选择文件夹";
            btn_drawTemplateRegionRectangle1.Font = ModernUiTheme.UiFont;
            btn_drawTemplateRegionRectangle1.Dock = DockStyle.Fill;
            btn_drawTemplateRegionRectangle1.Margin = new Padding(0, 5, 0, 5);
            btn_drawTemplateRegionRectangle1.TabIndex = 1;

            textBox2.MaximumSize = new Size(0, 30);
            textBox2.MinimumSize = new Size(80, 30);
            textBox2.Height = 30;
            textBox2.Dock = DockStyle.None;
            textBox2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox2.Margin = new Padding(0, 5, 18, 5);
            textBox2.TabIndex = 2;
            comboBox1.Dock = DockStyle.None;
            comboBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.MinimumSize = new Size(60, 30);
            comboBox1.MaximumSize = new Size(0, 30);
            comboBox1.Margin = new Padding(0, 5, 0, 5);
            comboBox1.TabIndex = 3;
            textBox1.Dock = DockStyle.Left;
            textBox1.Size = new Size(128, 28);
            textBox1.Margin = new Padding(0, 5, 8, 5);
            textBox1.TabIndex = 4;

            Button resetButton = new Button();
            resetButton.Name = "btnResetSaveImage";
            resetButton.Text = "恢复默认";
            resetButton.Tag = "secondary";
            resetButton.AutoSize = true;
            resetButton.MinimumSize = new Size(88, 32);
            resetButton.Margin = new Padding(0, 5, 0, 5);
            resetButton.TabIndex = 5;
            resetButton.Click += delegate { toolStripButton2.PerformClick(); };

            layout.Controls.Add(label3, 0, 1);
            layout.Controls.Add(tbx_imageSavePath, 1, 1);
            layout.SetColumnSpan(tbx_imageSavePath, 2);
            layout.Controls.Add(btn_drawTemplateRegionRectangle1, 3, 1);

            layout.Controls.Add(label8, 0, 2);
            layout.Controls.Add(textBox2, 1, 2);
            layout.Controls.Add(label4, 2, 2);
            layout.Controls.Add(comboBox1, 3, 2);

            layout.Controls.Add(label5, 0, 3);
            layout.Controls.Add(textBox1, 1, 3);
            WinFormsLabel retentionHint = CreateHint("0 天表示不按期限清理");
            retentionHint.Anchor = AnchorStyles.Left;
            layout.Controls.Add(retentionHint, 2, 3);
            layout.Controls.Add(resetButton, 3, 3);
            card.Controls.Add(layout);
            return card;
        }

        private void ConfigureFieldLabel(WinFormsLabel label, string text)
        {
            label.Text = text;
            label.Font = ModernUiTheme.UiFont;
            label.ForeColor = ModernUiTheme.SecondaryText;
            label.BackColor = Color.Transparent;
            label.AutoSize = true;
            label.Anchor = AnchorStyles.Left;
            label.Margin = Padding.Empty;
        }

        private Panel BuildFileCard()
        {
            Panel card = CreateCard("saveImageFileCard");
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.BackColor = Color.Transparent;
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            layout.RowCount = 2;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            layout.Controls.Add(CreateSectionTitle("文件管理"), 0, 0);
            WinFormsLabel hint = CreateHint("自动规则只在工具执行时生效");
            hint.Anchor = AnchorStyles.Right;
            layout.Controls.Add(hint, 1, 0);

            FlowLayoutPanel options = new FlowLayoutPanel();
            options.Dock = DockStyle.Fill;
            options.BackColor = Color.Transparent;
            options.FlowDirection = FlowDirection.TopDown;
            options.WrapContents = false;
            options.Padding = new Padding(0, 2, 0, 0);
            ConfigureOption(checkBox1);
            ConfigureOption(checkBox2);
            ConfigureOption(checkBox3);
            checkBox1.TabIndex = 0;
            checkBox2.TabIndex = 1;
            checkBox3.TabIndex = 2;
            options.Controls.Add(checkBox1);
            options.Controls.Add(checkBox2);
            options.Controls.Add(checkBox3);

            FlowLayoutPanel actions = new FlowLayoutPanel();
            actions.Dock = DockStyle.Fill;
            actions.BackColor = Color.Transparent;
            actions.FlowDirection = FlowDirection.TopDown;
            actions.WrapContents = false;
            actions.Padding = new Padding(12, 2, 0, 0);
            ResetLegacyButtonSkin(button5);
            ResetLegacyButtonSkin(button4);
            button5.Text = "打开保存位置";
            button4.Text = "清空保存位置";
            button5.Tag = "secondary";
            button5.Size = new Size(126, 34);
            button4.Size = new Size(126, 34);
            button5.TabIndex = 3;
            button4.TabIndex = 4;
            button5.Margin = new Padding(0, 0, 0, 8);
            button4.Margin = Padding.Empty;
            actions.Controls.Add(button5);
            actions.Controls.Add(button4);

            layout.Controls.Add(options, 0, 1);
            layout.Controls.Add(actions, 1, 1);
            card.Controls.Add(layout);
            return card;
        }

        private void ConfigureOption(CheckBox checkBox)
        {
            checkBox.AutoSize = true;
            checkBox.Font = ModernUiTheme.UiFont;
            checkBox.Margin = new Padding(0, 0, 0, 9);
            checkBox.Padding = new Padding(2, 0, 0, 0);
        }

        private void BuildFooter()
        {
            panel6.Controls.Clear();
            panel6.Dock = DockStyle.Fill;
            panel6.Margin = Padding.Empty;
            panel6.Padding = new Padding(18, 11, 18, 11);
            panel6.BackColor = ModernUiTheme.Surface;

            TableLayoutPanel footer = new TableLayoutPanel();
            footer.Dock = DockStyle.Fill;
            footer.BackColor = Color.Transparent;
            footer.ColumnCount = 3;
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 128F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            enableToggleButton = new Button();
            enableToggleButton.Name = "btnSaveImageEnable";
            enableToggleButton.Text = "工具已启用";
            enableToggleButton.Size = new Size(112, 38);
            enableToggleButton.Margin = Padding.Empty;
            enableToggleButton.TabIndex = 0;
            enableToggleButton.Click += EnableToggleButton_Click;

            FlowLayoutPanel status = new FlowLayoutPanel();
            status.Dock = DockStyle.Fill;
            status.BackColor = Color.Transparent;
            status.FlowDirection = FlowDirection.TopDown;
            status.WrapContents = false;
            status.Padding = new Padding(8, 0, 0, 0);
            label2.Font = ModernUiTheme.UiFont;
            label1.Font = ModernUiTheme.UiFont;
            label2.ForeColor = ModernUiTheme.SecondaryText;
            label1.ForeColor = ModernUiTheme.SecondaryText;
            label2.Margin = Padding.Empty;
            label1.Margin = new Padding(0, 4, 0, 0);
            status.Controls.Add(label2);
            status.Controls.Add(label1);

            FlowLayoutPanel actions = new FlowLayoutPanel();
            actions.AutoSize = true;
            actions.Dock = DockStyle.Fill;
            actions.BackColor = Color.Transparent;
            actions.FlowDirection = FlowDirection.LeftToRight;
            actions.WrapContents = false;
            actions.Padding = Padding.Empty;
            ResetLegacyButtonSkin(btn_runTool);
            ResetLegacyButtonSkin(btn_confirm);
            ResetLegacyButtonSkin(btn_cancel);
            ConfigureFooterAction(btn_runTool, "运行工具", 92);
            ConfigureFooterAction(btn_confirm, "运行到此处", 104);
            ConfigureFooterAction(btn_cancel, "关闭", 80);
            btn_runTool.TabIndex = 1;
            btn_confirm.TabIndex = 2;
            btn_cancel.TabIndex = 3;
            btn_confirm.Tag = "secondary";
            actions.Controls.Add(btn_runTool);
            actions.Controls.Add(btn_confirm);
            actions.Controls.Add(btn_cancel);

            footer.Controls.Add(enableToggleButton, 0, 0);
            footer.Controls.Add(status, 1, 0);
            footer.Controls.Add(actions, 2, 0);
            panel6.Controls.Add(footer);
            ModernUiTheme.StyleCard(panel6);
        }

        private void ConfigureFooterAction(Button button, string text, int width)
        {
            button.Text = text;
            button.Size = new Size(width, 38);
            button.Anchor = AnchorStyles.None;
            button.Margin = new Padding(8, 0, 0, 0);
        }

        private void ResetLegacyButtonSkin(Button button)
        {
            button.BackgroundImage = null;
            button.UseVisualStyleBackColor = false;
            button.MouseDown -= Btn_MouseDown;
            button.MouseUp -= Btn_MouseUp;
            button.MouseEnter -= Btn_MouseEnter;
            button.MouseLeave -= Btn_MouseLeave;
        }

        private void EnableToggleButton_Click(object sender, EventArgs e)
        {
            if (Job.loadForm)
                return;

            pictureBox2_Click(pictureBox2, EventArgs.Empty);
            Job job = Job.FindJobByName(jobName);
            if (job != null)
                RefreshEnableState(job.FindToolInfoByName(toolName).enable);
        }

        internal void RefreshEnableState(bool enabled)
        {
            if (enableToggleButton == null)
                return;

            enableToggleButton.Text = enabled ? "工具已启用" : "工具已停用";
            enableToggleButton.ForeColor = enabled ? ModernUiTheme.AccentPressed : ModernUiTheme.SecondaryText;
            ModernUiTheme.RefreshRoundedButtonPalette(
                enableToggleButton,
                enabled ? ModernUiTheme.AccentSoft : ModernUiTheme.SurfaceRaised,
                enabled ? ModernUiTheme.Selection : ModernUiTheme.Surface,
                ModernUiTheme.AccentSoft,
                enabled ? ModernUiTheme.Accent : ModernUiTheme.Border);
        }

        void textBox1_valueChanged(double value)
        {
            saveImageTool.saveDays = (int)textBox1.Value;
        }

        private void comboBox1_SelectedIndexChanged()
        {
            saveImageTool.imageFormat = comboBox1.TextStr;
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
            try
            {
                string configuredPath = (saveImageTool.imageSavePath ?? string.Empty).Trim();
                if (configuredPath.Length == 0)
                {
                    MessageBox.Show("请先设置有效的保存位置。", "无法清空", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!IsFullyQualifiedWindowsPath(configuredPath))
                {
                    MessageBox.Show("请使用完整的绝对路径，例如 D:\\WLP VM 图像。", "无法清空",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string targetPath = Path.GetFullPath(configuredPath);
                string rootPath = Path.GetPathRoot(targetPath);
                if (string.Equals(targetPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                                  (rootPath ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                                  StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("不能清空磁盘根目录，请选择具体的图像保存文件夹。", "已阻止危险操作",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string protectionReason;
                if (IsProtectedClearTarget(targetPath, out protectionReason))
                {
                    MessageBox.Show(protectionReason, "已阻止危险操作",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!Directory.Exists(targetPath))
                {
                    MessageBox.Show("保存位置尚不存在，无需清空。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (ContainsReparsePoint(targetPath))
                {
                    MessageBox.Show("保存路径经过链接或重定向目录，无法确认实际删除范围。请改用普通文件夹。", "已阻止危险操作",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "将永久删除以下位置中的全部文件和子文件夹：\r\n\r\n" + targetPath + "\r\n\r\n此操作不可撤销，是否继续？",
                    "确认清空保存位置", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (result != DialogResult.Yes)
                    return;

                bool cleared = SaveImageTool.TryExecuteDirectoryOperationWhenIdle(targetPath, delegate
                {
                    DeleteDirectoryContentsWithoutFollowingLinks(targetPath);
                });
                if (!cleared)
                {
                    MessageBox.Show("仍有待保存图像或后台文件操作，请稍后重试。", "暂时无法清空",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MessageBox.Show("保存位置已清空。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                MessageBox.Show("清空失败，请检查目录权限或文件占用。", "操作失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool IsFullyQualifiedWindowsPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            string value = path.Trim();
            if (value.StartsWith(@"\\", StringComparison.Ordinal))
                return true;

            return value.Length >= 3 && char.IsLetter(value[0]) && value[1] == ':' &&
                   (value[2] == Path.DirectorySeparatorChar || value[2] == Path.AltDirectorySeparatorChar);
        }

        private static bool IsProtectedClearTarget(string targetPath, out string reason)
        {
            reason = string.Empty;
            string[] userContainers =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            foreach (string protectedPath in userContainers)
            {
                if (IsSamePathOrParent(targetPath, protectedPath))
                {
                    reason = "不能清空用户目录、桌面、文档目录或它们的上级目录。请指定专用的图像保存子文件夹。";
                    return true;
                }
            }

            string[] strictPaths =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                Environment.SystemDirectory,
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Application.StartupPath
            };

            foreach (string protectedPath in strictPaths)
            {
                if (IsSamePathOrParent(targetPath, protectedPath) || IsSamePathOrParent(protectedPath, targetPath))
                {
                    reason = "该位置属于系统、程序安装目录或当前软件目录，不能执行清空。请指定专用的图像保存文件夹。";
                    return true;
                }
            }

            return false;
        }

        private static bool IsSamePathOrParent(string candidateParent, string candidateChild)
        {
            if (string.IsNullOrWhiteSpace(candidateParent) || string.IsNullOrWhiteSpace(candidateChild))
                return false;

            string parent = Path.GetFullPath(candidateParent)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string child = Path.GetFullPath(candidateChild)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (string.Equals(parent, child, StringComparison.OrdinalIgnoreCase))
                return true;

            return child.StartsWith(parent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        private static bool ContainsReparsePoint(string targetPath)
        {
            DirectoryInfo current = new DirectoryInfo(targetPath);
            while (current != null)
            {
                if (current.Exists && (current.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    return true;
                current = current.Parent;
            }
            return false;
        }

        private static void DeleteDirectoryContentsWithoutFollowingLinks(string targetPath)
        {
            DirectoryInfo root = new DirectoryInfo(targetPath);
            foreach (FileInfo file in root.GetFiles())
                file.Delete();

            foreach (DirectoryInfo directory in root.GetDirectories())
            {
                bool isLink = (directory.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
                directory.Delete(!isLink);
            }
        }

    }
}
