using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_GeneralSettings : Form
    {
        internal Frm_GeneralSettings()
        {
            InitializeComponent();
            Init_Language();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_GeneralSettings _instance;
        public static Frm_GeneralSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_GeneralSettings();
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
                    //////cbo_lanuage.Item.Clear();
                    //////cbo_lanuage.Item.Add("Simplified Chinese");
                    //////cbo_lanuage.Item.Add("English");
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void btn_selectPath_Click(object sender, EventArgs e)
        {
            try
            {



                VistaFolderBrowserDialog folderBrowseDialog = new VistaFolderBrowserDialog();
                if (Directory.Exists(Project.Instance.configuration.dataPath))
                    folderBrowseDialog.SelectedPath = Project.Instance.configuration.dataPath;

                folderBrowseDialog.Description = Project.Instance.configuration.language == Language.English ? "Please select image folder" : "请选择数据存储路径";
                if (folderBrowseDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Project.Instance.configuration.dataPath = folderBrowseDialog.SelectedPath;
                    tbx_dataPath.TextStr = folderBrowseDialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private void btn_drawTemplateRegionRectangle1_Click(object sender, EventArgs e)
        {
            try
            {



                VistaFolderBrowserDialog folderBrowseDialog = new VistaFolderBrowserDialog();
                if (Directory.Exists(Project.Instance.configuration.dataPath))
                    folderBrowseDialog.SelectedPath = Project.Instance.configuration.dataPath;

                folderBrowseDialog.Description = Project.Instance.configuration.language == Language.English ? "Please select image folder" : "请选择数据存储路径";
                if (folderBrowseDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Project.Instance.configuration.dataPath = folderBrowseDialog.SelectedPath;
                    tbx_dataPath.TextStr = folderBrowseDialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
