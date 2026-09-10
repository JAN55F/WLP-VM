using LightController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    [Serializable]
    public class Project
    {

        internal List<LightController_Base> L_lightController = new List<LightController_Base>();
        internal List<TCPClient> L_TCPClient = new List<TCPClient>();
        internal List<TCPSever> L_TCPSever = new List<TCPSever>();
        internal List<Scaner> L_Scaner = new List<Scaner>();
        internal List<Serial> L_Serial = new List<Serial>();
        internal List<PLCDevice> L_PLCDevice = new List<PLCDevice>();
        /// <summary>
        /// 方案集合
        /// </summary>
        internal List<Scheme> L_engineList = new List<Scheme>();
        /// <summary>
        /// 当前正在使用的方案
        /// </summary>
        public Scheme curEngine = new Scheme();
        /// <summary>
        /// 项目配置
        /// </summary>
        public Configuration configuration = new Configuration();
        /// <summary>
        /// 项目实例
        /// </summary>
        private static Project _instance;
        private static string _currentProjectFilePath = string.Empty;

        private static string ProjectDirectory
        {
            get { return Path.Combine(Application.StartupPath, "Config", "Project", "Vision"); }
        }

        private static string LastProjectMarkerPath
        {
            get { return Path.Combine(Application.StartupPath, "Config", "LastProject.txt"); }
        }
        public static Project Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Project();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }



        /// <summary>
        /// 根据方案名称查找方案
        /// </summary>
        /// <param name="engineName"></param>
        /// <returns></returns>
        internal Scheme FindEngineByName(string engineName)
        {
            try
            {
                for (int i = 0; i < L_engineList.Count; i++)
                {
                    if (L_engineList[i].schemeName == engineName)
                        return L_engineList[i];
                }
                return new Scheme();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new Scheme();
            }
        }
        /// <summary>
        /// 加载项目
        /// </summary>
        internal static Project LoadProject(string path)
        {
            Project previousProject = Project.Instance;
            try
            {
                Job.isDrawing = true;
                Job temp = new Job();       //new一下，就会在构造函数中初始化空白处右击菜单，否则流程编辑器空白处右击将没有菜单

                IFormatter formatter = new BinaryFormatter();
                Project loadedProject;
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    loadedProject = (Project)formatter.Deserialize(stream);
                }

                NormalizeLoadedProject(loadedProject);
                Project.Instance = loadedProject;
                // 项目文件中可能序列化了历史公司名，加载后统一覆盖为当前品牌。
                Project.Instance.configuration.CompanyName = Configuration.DefaultCompanyName;
                // 仅迁移空标题和已知的旧演示标题；客户自行命名的项目标题保持不变。
                Project.Instance.configuration.ProgramTitle = Configuration.NormalizeProgramTitle(
                    Project.Instance.configuration.ProgramTitle);
                EnsureCommunicationRuntime();

                RememberProjectPath(path);

                Frm_Job.Instance.tbc_jobs.TabPages.Clear();
                if (Project.Instance.L_engineList.Count > 0)
                {

                    //Project.Instance.curEngine = Project.Instance.L_engineList[0];
                    Frm_Main.Instance.lbl_title.Text = Configuration.BuildApplicationTitle(
                        Project.Instance.configuration.ProgramTitle);
                    Frm_Main.Instance.lbl_curEngine.Text = string.Format("当前方案：{0}", Project.Instance.curEngine.schemeName);

                    for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                    {
                        Job.LoadJob(Project.Instance.curEngine.L_jobList[i]);

                        //if (Project.Instance.curEngine.L_jobList[i].jobName == "其它设备")                      //临时
                        //    Project.Instance.curEngine.L_jobList[i].Run();
                    }
                }
                else
                {
                    Frm_Main.Instance.lbl_curEngine.Text = "当前方案：未创建";
                }
                Frm_EngineManager.Instance.cbx_engineList.Clear();
                Frm_Main.Instance.lbl_curEngine.DropDownItems.Clear();
                for (int i = 0; i < Project.Instance.L_engineList.Count; i++)
                {
                    Frm_EngineManager.Instance.cbx_engineList.Add(Project.Instance.L_engineList[i].schemeName);
                    Frm_Main.Instance.lbl_curEngine.DropDownItems.Add(Project.Instance.L_engineList[i].schemeName);
                }
                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Project inported successfully" : "项目加载成功", Color.Black);
                Job.isDrawing = false;
                return Project.Instance;
            }
            catch (Exception ex)
            {
                Project.Instance = previousProject;
                Log.SaveError(ex);
                try
                {
                    Frm_Main.Instance.OutputMsg("项目加载失败，已保留当前项目：" + ex.Message, Color.Red);
                }
                catch { }
                return null;
            }
            finally
            {
                Job.isDrawing = false;
            }
        }

        private static void NormalizeLoadedProject(Project project)
        {
            if (project == null)
                throw new SerializationException("项目文件内容为空");

            if (project.configuration == null)
                project.configuration = new Configuration();
            if (project.configuration.L_recentlyOpendFile == null)
                project.configuration.L_recentlyOpendFile = new List<string>();
            if (project.L_engineList == null)
                project.L_engineList = new List<Scheme>();
            project.L_engineList.RemoveAll(item => item == null);
            if (project.L_lightController == null)
                project.L_lightController = new List<LightController_Base>();
            if (project.L_TCPClient == null)
                project.L_TCPClient = new List<TCPClient>();
            if (project.L_TCPSever == null)
                project.L_TCPSever = new List<TCPSever>();
            if (project.L_Scaner == null)
                project.L_Scaner = new List<Scaner>();
            if (project.L_Serial == null)
                project.L_Serial = new List<Serial>();
            if (project.L_PLCDevice == null)
                project.L_PLCDevice = new List<PLCDevice>();

            if (project.curEngine == null && project.L_engineList.Count > 0)
                project.curEngine = project.L_engineList[0];
            if (project.curEngine == null)
                project.curEngine = new Scheme();

            for (int i = 0; i < project.L_engineList.Count; i++)
            {
                if (project.L_engineList[i] != null && project.L_engineList[i].L_jobList == null)
                    project.L_engineList[i].L_jobList = new List<Job>();
            }
            if (project.curEngine.L_jobList == null)
                project.curEngine.L_jobList = new List<Job>();

            // 某些旧文件把当前方案保存成了列表外的另一个对象，按名称恢复为列表中的正式对象。
            bool currentSchemeFound = false;
            for (int i = 0; i < project.L_engineList.Count; i++)
            {
                if (project.L_engineList[i] != null &&
                    project.L_engineList[i].schemeName == project.curEngine.schemeName)
                {
                    project.curEngine = project.L_engineList[i];
                    currentSchemeFound = true;
                    break;
                }
            }
            if (!currentSchemeFound && project.L_engineList.Count > 0)
                project.curEngine = project.L_engineList[0];

            // 所有旧方案中的流程都先恢复为干净的运行时状态，而不仅是当前显示的方案。
            // 后续切换方案时，InportJob 会再按同一规则重建编辑树和连线。
            HashSet<Job> preparedJobs = new HashSet<Job>();
            for (int i = 0; i < project.L_engineList.Count; i++)
            {
                Scheme scheme = project.L_engineList[i];
                if (scheme == null)
                    continue;
                if (scheme.L_jobList == null)
                    scheme.L_jobList = new List<Job>();
                scheme.L_jobList.RemoveAll(item => item == null);
                for (int j = 0; j < scheme.L_jobList.Count; j++)
                {
                    if (preparedJobs.Add(scheme.L_jobList[j]))
                        scheme.L_jobList[j].PrepareLoadedWorkflowData();
                }
            }

            if (project.curEngine != null && project.curEngine.L_jobList != null)
            {
                project.curEngine.L_jobList.RemoveAll(item => item == null);
                for (int i = 0; i < project.curEngine.L_jobList.Count; i++)
                {
                    if (preparedJobs.Add(project.curEngine.L_jobList[i]))
                        project.curEngine.L_jobList[i].PrepareLoadedWorkflowData();
                }
            }
        }

        internal static string FindStartupProjectPath()
        {
            try
            {
                if (File.Exists(LastProjectMarkerPath))
                {
                    string rememberedPath = File.ReadAllText(LastProjectMarkerPath).Trim();
                    if (!string.IsNullOrEmpty(rememberedPath) && File.Exists(rememberedPath))
                        return rememberedPath;
                }

                if (!Directory.Exists(ProjectDirectory))
                    return string.Empty;

                string[] files = Directory.GetFiles(ProjectDirectory, "*.pjt");
                if (files.Length == 0)
                    return string.Empty;

                return files.OrderByDescending(file => File.GetLastWriteTimeUtc(file)).First();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }

        internal static bool LoadStartupProject()
        {
            string path = FindStartupProjectPath();
            if (string.IsNullOrEmpty(path))
                return false;
            if (LoadProject(path) != null)
                return true;

            string backupPath = path + ".bak";
            if (File.Exists(backupPath) && LoadProject(backupPath) != null)
            {
                RememberProjectPath(path);
                Frm_Main.Instance.OutputMsg("主项目文件损坏，已从上一次备份恢复", Color.Red);
                return true;
            }
            return false;
        }

        internal static void RememberRecentFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            path = Path.GetFullPath(path);
            if (Project.Instance.configuration.L_recentlyOpendFile == null)
                Project.Instance.configuration.L_recentlyOpendFile = new List<string>();
            Project.Instance.configuration.L_recentlyOpendFile.Remove(path);
            Project.Instance.configuration.L_recentlyOpendFile.Insert(0, path);
            if (Project.Instance.configuration.L_recentlyOpendFile.Count > 5)
                Project.Instance.configuration.L_recentlyOpendFile.RemoveRange(5, Project.Instance.configuration.L_recentlyOpendFile.Count - 5);
        }

        private static void RememberProjectPath(string path)
        {
            _currentProjectFilePath = Path.GetFullPath(path);
            string markerDirectory = Path.GetDirectoryName(LastProjectMarkerPath);
            if (!Directory.Exists(markerDirectory))
                Directory.CreateDirectory(markerDirectory);
            File.WriteAllText(LastProjectMarkerPath, _currentProjectFilePath, Encoding.UTF8);
            RememberRecentFile(_currentProjectFilePath);
        }

        private static void EnsureCommunicationRuntime()
        {
            try
            {
                if (Project.Instance.L_TCPSever == null)
                    Project.Instance.L_TCPSever = new List<TCPSever>();
                if (Project.Instance.L_TCPClient == null)
                    Project.Instance.L_TCPClient = new List<TCPClient>();
                if (Project.Instance.L_PLCDevice == null)
                    Project.Instance.L_PLCDevice = new List<PLCDevice>();

                TCPClient.ResetRuntimeStore();
                PLCDevice.ResetRuntimeStore();

                for (int i = 0; i < Project.Instance.L_TCPSever.Count; i++)
                {
                    if (Project.Instance.L_TCPSever[i] != null)
                        Project.Instance.L_TCPSever[i].EnsureRuntime();
                }
                for (int i = 0; i < Project.Instance.L_TCPClient.Count; i++)
                {
                    if (Project.Instance.L_TCPClient[i] != null)
                        Project.Instance.L_TCPClient[i].EnsureRuntime();
                }
                for (int i = 0; i < Project.Instance.L_PLCDevice.Count; i++)
                {
                    if (Project.Instance.L_PLCDevice[i] != null)
                        Project.Instance.L_PLCDevice[i].EnsureRuntime();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 导入项目
        /// </summary>
        internal static void InportProject()
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dig_openFileDialog = new System.Windows.Forms.OpenFileDialog();
                dig_openFileDialog.Title = (Project.Instance.configuration.language == Language.English ? "Please select project file" : "请指定项目文件");
                dig_openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openFileDialog.Filter = "项目文件(*.pjt)|*.pjt";
                if (dig_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (LoadProject(dig_openFileDialog.FileName) != null)
                        Project.Instance.configuration.Save();
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 导出项目
        /// </summary>
        internal static void ExportProject()
        {
            try
            {
                if (Project.Instance.L_engineList.Count > 0)
                {
                    System.Windows.Forms.SaveFileDialog dig_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                    dig_saveFileDialog.FileName = Project.Instance.configuration.ProgramTitle;
                    dig_saveFileDialog.Title = (Project.Instance.configuration.language == Language.English ? "Please select the project file saving path" : "请指定项目导出路径");
                    dig_saveFileDialog.Filter = "项目文件|*.pjt";
                    dig_saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    if (dig_saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveProjectToPath(dig_saveFileDialog.FileName, true);

                        //更新结果下拉框
                        //Frm_ImageWindow.Instance.Update_Last_Run_Result_Image_List();
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Project exported successfully" : "项目导出成功", Color.Black);
                    }
                }
                else
                {
                    Frm_Main.Instance.OutputMsg("当前项目尚未添加任何方案，不可导出", Color.Red);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 保存项目
        /// </summary>
        internal static void SaveProject()
        {
            try
            {
                string path = _currentProjectFilePath;
                if (string.IsNullOrEmpty(path))
                {
                    string fileName = MakeSafeFileName(Project.Instance.configuration.ProgramTitle);
                    path = Path.Combine(ProjectDirectory, fileName + ".pjt");
                }

                SaveProjectToPath(path, true);

                //更新结果下拉框
                //Frm_ImageWindow.Instance.Update_Last_Run_Result_Image_List();
                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Project exported successfully" : "保存项目成功", Color.Black);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                try
                {
                    Frm_Main.Instance.OutputMsg("保存项目失败：" + ex.Message, Color.Red);
                }
                catch { }
            }
        }

        private static string MakeSafeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "未命名项目";

            char[] invalidChars = Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalidChars.Length; i++)
                fileName = fileName.Replace(invalidChars[i], '_');
            return fileName;
        }

        private static void SaveProjectToPath(string path, bool rememberPath)
        {
            string fullPath = Path.GetFullPath(path);
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string tempPath = fullPath + ".tmp";
            string backupPath = fullPath + ".bak";

            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, Project.Instance);
                    stream.Flush(true);
                }

                if (File.Exists(fullPath))
                {
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                    try
                    {
                        File.Replace(tempPath, fullPath, backupPath);
                    }
                    catch (IOException)
                    {
                        // FAT/部分网络盘不支持 File.Replace，退回普通覆盖。
                        File.Copy(fullPath, backupPath, true);
                        File.Copy(tempPath, fullPath, true);
                        File.Delete(tempPath);
                    }
                }
                else
                {
                    File.Move(tempPath, fullPath);
                }

                if (rememberPath)
                    RememberProjectPath(fullPath);
            }
            catch
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                throw;
            }
        }

    }
}
