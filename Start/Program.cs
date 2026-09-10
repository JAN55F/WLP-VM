using Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using VMPro;

namespace Start
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                HalconRuntime.EnsureLoaded();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "WLP VM - HALCON 启动检查", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RunApplication();
        }

        // 在原生运行库就绪后才进入引用 VMPro/图像窗体的启动方法。
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void RunApplication()
        {
            try
            {
                //此处首先读取一次配置，因为程序启动时就需要知道当前语言选择，用于下面的提示信息的语言类型
                Ini ini = new Ini(Application.StartupPath + @"\Config\Config.ini");
                string language = ini.IniReadConfig("Language");
                if (language != string.Empty)
                    Project.Instance.configuration.language = (Language)System.Enum.Parse(typeof(Language), language);

                System.Threading.Mutex mutex = new System.Threading.Mutex(false, "ThisShouldOnlyRunOnce");
                bool running = true;
                try
                {
                    running = !mutex.WaitOne(0, false);            //这一句有时候会报错，管球它的，先Try起来再说
                }
                catch { }
                if (running)
                {
                    Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "An instance has already been run (or the old instance has not been completely closed). Do you want to open another instance?" : "      已经运行了一个实例（或旧实例尚未完全关闭），是否再开启\r\n一个实例？");
                    Frm_ConfirmBox.Instance.ShowDialog();
                    if (Frm_ConfirmBox.Instance.Result == ConfirmBoxResult.Yes)
                    {
                        VM.Init();
                    }
                }
                else
                {
                    VM.Init();
                }
            }
            catch
            {
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Startup failed" : "启动失败（错误代码：00001）", TipType.Error);
            }
        }

    }
}
