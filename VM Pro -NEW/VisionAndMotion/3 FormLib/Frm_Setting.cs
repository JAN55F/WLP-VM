using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tool;
using VMPro.Properties;

namespace VMPro
{
    internal partial class Frm_Setting : Frm_FormBase
    {
        internal Frm_Setting()
        {
            InitializeComponent();
            Init_Language();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_Setting _instance;
        public static Frm_Setting Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_Setting();
                return _instance;
            }
        }


        /// <summary>
        /// 初始化语言
        /// </summary>
        private void Init_Language()
        {
            try
            {
                if (Project.Instance.configuration.language == Language.English)
                {
                    this.Text = "Axis Control";
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void Frm_Setting_Load(object sender, EventArgs e)
        {
            try
            {
                tvw_setting.SelectedNode = tvw_setting.Nodes[0];

                Frm_GeneralSettings.Instance.tbx_companyName.TextStr = Project.Instance.configuration.CompanyName;
                Frm_GeneralSettings.Instance.cbo_lanuage.TextStr = Project.Instance.configuration.language == Language.English ? "English" : "简体中文";
                Frm_GeneralSettings.Instance.tbx_dataPath.TextStr = Project.Instance.configuration.dataPath;

                Frm_ProjetSettings.Instance.tbx_programTitle.TextStr = Project.Instance.configuration.ProgramTitle;
                Frm_ProjetSettings.Instance.cbx_cardType.TextStr = Project.Instance.configuration.cardType.ToString();
                Frm_ProjetSettings.Instance.ckb_vitualCard.Checked = Project.Instance.configuration.vitualCard;
                switch (Project.Instance.configuration.defaultForm)
                {
                    case FormMode.MainForm:
                        Frm_ProjetSettings.Instance.radioButton1.Checked = true;
                        break;
                    case FormMode.VisionForm:
                        Frm_ProjetSettings.Instance.radioButton2.Checked = true;
                        break;
                    case FormMode.MotionForm:
                        Frm_ProjetSettings.Instance.radioButton3.Checked = true;
                        break;
                }

                //////Frm_RunSettings.Instance.tbx_autoRunVel.Text = Project.Instance.configuration.autoRunVel.ToString();
                Frm_RunSettings.Instance.tbx_jobsRunPouseTime.Value = Project.Instance.configuration.timeBetweenJobRun;
                Frm_StartSetting.Instance.cCheckBox1.Checked = Project.Instance.configuration.autoRunAfterStart;
                Frm_StartSetting.Instance.ckb_autoConnect.Checked = Project.Instance.configuration.autoConnectAfterStart;
                Frm_StartSetting.Instance.ckb_switchedToAutoRunMode.Checked = Project.Instance.configuration.switchedToAutoMode;
                Frm_StartSetting.Instance.ckb_autoLock.Checked = Project.Instance.configuration.autoLockAfterStart;
                Frm_StartSetting.Instance.ckb_maxSizeAfterStart.Checked = Project.Instance.configuration.maxSizeAfterStart;
                Frm_StartSetting.Instance.ckb_displayLine.Checked = Project.Instance.configuration.displayLine;
                Frm_RunSettings.Instance.ckb_displayLine.Checked = Project.Instance.configuration.failStop;
                Frm_RunSettings.Instance.cCheckBox1.Checked = Project.Instance.configuration.endStop;
                Frm_StartSetting.Instance.ckb_allowResizeFormSize.Checked = Project.Instance.configuration.allowResizeForm;
                Frm_StartSetting.Instance.ckb_saveWhileExit.Checked = Project.Instance.configuration.saveWhenExit;
                Frm_StartSetting.Instance.ckb_autoStartAfterStartup.Checked = Project.Instance.configuration.autoStartAfterStartup;

                Frm_StartSetting.Instance.ckb_enablePermissionControl.Checked = Project.Instance.configuration.enablePermissionControl;

                Frm_ProjetSettings.Instance.checkBox1.Checked = Project.Instance.configuration.EnableMainForm;
                Frm_ProjetSettings.Instance.checkBox2.Checked = Project.Instance.configuration.EnableVisionForm;
                Frm_ProjetSettings.Instance.checkBox3.Checked = Project.Instance.configuration.EnableMotionForm;
                Frm_GeneralSettings.Instance.ckb_dataSaveDays.Value = Project.Instance.configuration.dataSaveDays;

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_saveSetting_Click(object sender, EventArgs e)
        {

        }
        static int windowType = 0;
        private void tvw_setting_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNode node = tvw_setting.SelectedNode;
                switch (node.Text)
                {
                    case "功能":
                        pnl_window.Controls.Clear();
                        Frm_StartSetting.Instance.TopLevel = false;
                        Frm_StartSetting.Instance.Parent = pnl_window;
                        Frm_StartSetting.Instance.Show();
                        windowType = 3;
                        break;
                    case "项目":
                        pnl_window.Controls.Clear();
                        Frm_ProjetSettings.Instance.TopLevel = false;
                        Frm_ProjetSettings.Instance.Parent = pnl_window;
                        Frm_ProjetSettings.Instance.Show();
                        windowType = 1;
                        break;
                    case "方案":
                        pnl_window.Controls.Clear();
                        Frm_EngineManager.Instance.TopLevel = false;
                        Frm_EngineManager.Instance.Parent = pnl_window;
                        Frm_EngineManager.Instance.Show();
                        windowType = 2;
                        break;
                    case "运行":
                        pnl_window.Controls.Clear();
                        Frm_RunSettings.Instance.TopLevel = false;
                        Frm_RunSettings.Instance.Parent = pnl_window;
                        Frm_RunSettings.Instance.Show();
                        windowType = 5;
                        break;
                    case "常规":
                        pnl_window.Controls.Clear();
                        Frm_GeneralSettings.Instance.TopLevel = false;
                        Frm_GeneralSettings.Instance.Parent = pnl_window;
                        Frm_GeneralSettings.Instance.Show();
                        windowType = 0;
                        break;
                    case "安全":
                    case "用户管理":
                        if (windowType != 4)
                        {
                            pnl_window.Controls.Clear();
                            Frm_UserManager.Instance.TopLevel = false;
                            Frm_UserManager.Instance.Parent = pnl_window;
                            Frm_UserManager.Instance.Show();
                            windowType = 4;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.BackgroundImage = Resources.ButtonDown;
            Application.DoEvents();
        }

        private void MouseUp(object sender, MouseEventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Project.Instance.configuration.autoConnectAfterStart = Frm_StartSetting.Instance.ckb_autoConnect.Checked;
                //////Project.Instance.configuration.autoRunVel = (short)Convert.ToInt32(Frm_RunSettings.Instance.tbx_autoRunVel.Text);
                Project.Instance.configuration.ProgramTitle = Frm_ProjetSettings.Instance.tbx_programTitle.TextStr.Trim();
                Project.Instance.configuration.EnableMainForm = Frm_ProjetSettings.Instance.checkBox1.Checked;
                Project.Instance.configuration.EnableVisionForm = Frm_ProjetSettings.Instance.checkBox2.Checked;
                Project.Instance.configuration.EnableMotionForm = Frm_ProjetSettings.Instance.checkBox3.Checked;
                Project.Instance.configuration.timeBetweenJobRun = Convert.ToInt32(Frm_RunSettings.Instance.tbx_jobsRunPouseTime.Value);
                Project.Instance.configuration.switchedToAutoMode = Frm_StartSetting.Instance.ckb_switchedToAutoRunMode.Checked;
                Project.Instance.configuration.dataPath = Frm_GeneralSettings.Instance.tbx_dataPath.TextStr.Trim();
                Project.Instance.configuration.language = Frm_GeneralSettings.Instance.cbo_lanuage.SelectedIndex == 0 ? Language.Chinese : Language.English;
                Project.Instance.configuration.cardType = (Frm_ProjetSettings.Instance.cbx_cardType.TextStr == string.Empty ? CardType.无 : (CardType)Enum.Parse(typeof(CardType), Frm_ProjetSettings.Instance.cbx_cardType.TextStr));
                Project.Instance.configuration.autoConnectAfterStart = Frm_StartSetting.Instance.ckb_autoConnect.Checked;
                Project.Instance.configuration.CompanyName = Frm_GeneralSettings.Instance.tbx_companyName.TextStr.Trim();
                Project.Instance.configuration.displayLine = Frm_StartSetting.Instance.ckb_displayLine.Checked;
                Project.Instance.configuration.autoRunAfterStart = Frm_StartSetting.Instance.cCheckBox1.Checked;

                Project.Instance.configuration.endStop = Frm_RunSettings.Instance.cCheckBox1.Checked;
                Project.Instance.configuration.failStop = Frm_RunSettings.Instance.ckb_displayLine.Checked;
                Project.Instance.configuration.dataSaveDays = Convert.ToInt16(Frm_GeneralSettings.Instance.ckb_dataSaveDays.Value);
                Project.Instance.configuration.autoLockAfterStart = Frm_StartSetting.Instance.ckb_autoLock.Checked;
                Project.Instance.configuration.maxSizeAfterStart = Frm_StartSetting.Instance.ckb_maxSizeAfterStart.Checked;
                Project.Instance.configuration.enablePermissionControl = Frm_StartSetting.Instance.ckb_enablePermissionControl.Checked;
                if (Frm_StartSetting.Instance.ckb_autoStartAfterStartup.Checked != Project.Instance.configuration.autoStartAfterStartup)
                {
                    if (Frm_StartSetting.Instance.ckb_autoStartAfterStartup.Checked)
                        Frm_Main.Auto_Start(!Project.Instance.configuration.autoStartAfterStartup);
                    else
                        Frm_Main.Auto_Start(!Project.Instance.configuration.autoStartAfterStartup);
                }
                Project.Instance.configuration.autoStartAfterStartup = Frm_StartSetting.Instance.ckb_autoStartAfterStartup.Checked;
                Project.Instance.configuration.allowResizeForm = Frm_StartSetting.Instance.ckb_allowResizeFormSize.Checked;
                Project.Instance.configuration.vitualCard = Frm_ProjetSettings.Instance.ckb_vitualCard.Checked;
                Project.Instance.configuration.saveWhenExit = Frm_StartSetting.Instance.ckb_saveWhileExit.Checked;
                if (Frm_ProjetSettings.Instance.radioButton1.Checked)
                    Project.Instance.configuration.defaultForm = FormMode.MainForm;
                else if (Frm_ProjetSettings.Instance.radioButton2.Checked)
                    Project.Instance.configuration.defaultForm = FormMode.VisionForm;
                else
                    Project.Instance.configuration.defaultForm = FormMode.MotionForm;

                Project.SaveProject();
                // SaveAll();
                Project.Instance.configuration.Save();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
