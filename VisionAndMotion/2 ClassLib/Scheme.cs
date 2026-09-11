using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;
using LightController;
using HalconDotNet;

namespace VMPro
{
    [Serializable]
    public class Scheme : ICloneable
    {

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        internal Dictionary<DateTime, string> D_historyAlarm = new Dictionary<DateTime, string>();


        internal GlobelVariable globelVariable = new GlobelVariable();
        internal SmartPosTable smartPosTable = new SmartPosTable();

        internal string GetNewName(string type)
        {
            int i;
            for (i = 0; i < 1000; i++)
            {
                bool exist = false;
                for (int j = 0; j < globelVariable.L_variable.Count; j++)
                {
                    if (globelVariable.L_variable[j].name == type + i)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    return type + i;
                }
            }

            return "";
        }

        internal static void OpenScheme()
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dig_openFileDialog = new System.Windows.Forms.OpenFileDialog();
                dig_openFileDialog.Title = (Project.Instance.configuration.language == Language.English ? "Please select scheme file" : "请选择方案文件");
                dig_openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openFileDialog.Filter = "方案文件(*.eng)|*.eng";
                if (dig_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Scheme loadedScheme = LoadScheme(dig_openFileDialog.FileName);
                    if (loadedScheme != null)
                    {
                        Project.RememberRecentFile(dig_openFileDialog.FileName);
                        Project.SaveProject();
                        Project.Instance.configuration.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                Frm_Main.Instance.OutputMsg("方案打开失败：" + ex.Message, Color.Red);
            }
        }
        /// <summary>
        /// 方案名称
        /// </summary>
        internal string schemeName = "未创建";
        /// <summary>
        /// 流程集合
        /// </summary>
        internal List<Job> L_jobList = new List<Job>();
        private static Scheme temp;

        /// <summary>
        /// 通过流程名获取流程
        /// </summary>
        /// <param name="jobName">流程名</param>
        /// <returns>流程</returns>
        internal Job FindJobByName(string jobName)
        {
            try
            {
                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    if ((Project.Instance.curEngine.L_jobList[i]).jobName == jobName)
                        return Project.Instance.curEngine.L_jobList[i];
                }
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Can not find job named：" + jobName + "（Error code：0001）" : "未找到名为" + jobName + "的流程（错误代码：00001）");
                return null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 加载方案
        /// </summary>
        internal static Scheme LoadScheme(string path)
        {
            try
            {
                IFormatter formatter = HalconSerializationGuard.CreateFormatter();
                Scheme loadedScheme;
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    loadedScheme = (Scheme)formatter.Deserialize(stream);
                }

                if (loadedScheme == null)
                    throw new SerializationException("方案文件内容为空");

                int oldIndex = Project.Instance.L_engineList.FindIndex(item => item != null && item.schemeName == loadedScheme.schemeName);
                if (oldIndex >= 0)
                    Project.Instance.L_engineList[oldIndex] = loadedScheme;
                else
                    Project.Instance.L_engineList.Add(loadedScheme);
                Project.Instance.curEngine = loadedScheme;

                Frm_Job.Instance.tbc_jobs.TabPages.Clear();
                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    Job.InportJob(Project.Instance.curEngine.L_jobList[i]);
                }
                Frm_EngineManager.Instance.cbx_engineList.Clear();
                Frm_Main.Instance.lbl_curEngine.DropDownItems.Clear();
                for (int i = 0; i < Project.Instance.L_engineList.Count; i++)
                {
                    Frm_EngineManager.Instance.cbx_engineList.Add(Project.Instance.L_engineList[i].schemeName);
                    Frm_Main.Instance.lbl_curEngine.DropDownItems.Add(Project.Instance.L_engineList[i].schemeName);
                }
                Frm_Main.Instance.lbl_curEngine.Text = "当前方案：" + loadedScheme.schemeName;
                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Project inported successfully" : "方案加载成功", Color.Black);
                return Project.Instance.curEngine;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                Frm_Main.Instance.OutputMsg("方案加载失败：" + ex.Message, Color.Red);
                return null;
            }
        }
        /// <summary>
        /// 加载方案
        /// </summary>
        internal static Scheme LoadScheme(Scheme engine)
        {
            try
            {
                Frm_Main.Instance.lbl_title.Text = Configuration.BuildApplicationTitle(
                    Project.Instance.configuration.ProgramTitle);
                Frm_Main.Instance.lbl_curEngine.Text = string.Format("当前方案：{0}", engine.schemeName);
                Frm_Job.Instance.tbc_jobs.TabPages.Clear();
                for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
                {
                    Job.InportJob(Project.Instance.curEngine.L_jobList[i]);
                }
                //////Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Project inported successfully" : "方案加载成功", Color.Green);
                return Project.Instance.curEngine;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new Scheme();
            }
        }
        /// <summary>
        /// 导入方案
        /// </summary>
        internal static void InportScheme()
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dig_openFileDialog = new System.Windows.Forms.OpenFileDialog();
                dig_openFileDialog.Title = Project.Instance.configuration.language == Language.English ? "Please select project file" : "请选择方案文件";
                dig_openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dig_openFileDialog.Filter = "方案文件(*.eng)|*.eng";
                if (dig_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (LoadScheme(dig_openFileDialog.FileName) != null)
                    {
                        Project.RememberRecentFile(dig_openFileDialog.FileName);
                        Project.SaveProject();
                        Project.Instance.configuration.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /* 利用反射实现深拷贝*/
        public static object DeepCopy(object _object)
        {
            Type T = _object.GetType();
            object o = Activator.CreateInstance(T);
            PropertyInfo[] PI = T.GetProperties();
            for (int i = 0; i < PI.Length; i++)
            {
                PropertyInfo P = PI[i];
                P.SetValue(o, P.GetValue(_object, null), null);
            }
            return o;
        }
        internal static void DeleteScheme()
        {
            try
            {
                Frm_ConfirmBox.Instance.lbl_info.Text = (Project.Instance.configuration.language == Language.English ? "Are you sure you want to delete current job?" : "\r\n确定要删除当前方案吗？");
                Frm_ConfirmBox.Instance.ShowDialog();
                if (Frm_ConfirmBox.Instance.Result != ConfirmBoxResult.Yes)
                {
                    return;
                }

                Job.isDrawing = true;
                if (Project.Instance.curEngine.schemeName == Frm_EngineManager.Instance.cbx_engineList.Text)
                {
                    Frm_Job.Instance.tbc_jobs.TabPages.Clear();
                }
                Project.Instance.L_engineList.Remove(Project.Instance.curEngine);
                //////Frm_EngineManager.Instance.cbx_engineList.Remove(Project.Instance.curEngine.schemeName);
                Frm_Main.Instance.lbl_curEngine.DropDownItems.RemoveByKey(Project.Instance.curEngine.schemeName);

                if (Project.Instance.L_engineList.Count > 0)
                {
                    Project.Instance.curEngine = Project.Instance.L_engineList[0];
                    Frm_EngineManager.Instance.cbx_engineList.Text = Project.Instance.curEngine.schemeName;
                    Frm_Main.Instance.lbl_curEngine.Text = Project.Instance.curEngine.schemeName;
                }
                else
                {
                    Project.Instance.curEngine = new Scheme();
                    Frm_Main.Instance.lbl_curEngine.Text = "当前方案：未创建";
                }
                Job.isDrawing = false;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 导出方案
        /// </summary>
        internal static void ExportScheme()
        {
            try
            {
                if (Project.Instance.curEngine.L_jobList.Count > 0)
                {
                    System.Windows.Forms.SaveFileDialog dig_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                    dig_saveFileDialog.FileName = Project.Instance.curEngine.schemeName;
                    dig_saveFileDialog.Title = Project.Instance.configuration.language == Language.English ? "Please select the project file saving path" : "请选择方案保存路径";
                    dig_saveFileDialog.Filter = "方案文件(*.eng)|*.eng";
                    dig_saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    if (dig_saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        IFormatter formatter = HalconSerializationGuard.CreateFormatter();
                        Stream stream = new FileStream(dig_saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                        formatter.Serialize(stream, Project.Instance.curEngine);
                        stream.Close();
                        Project.RememberRecentFile(dig_saveFileDialog.FileName);
                        Project.Instance.configuration.Save();

                        //更新结果下拉框
                        Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "Project exported successfully" : "方案导出成功", Color.Green);
                    }
                }
                else
                {
                    Frm_Main.Instance.OutputMsg("当前项目尚未添加方案，不可导出", Color.Red);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        public static class ObjectCopier
        {
            public static Engine Clone<Engine>(Engine source)
            {
                if (!typeof(Engine).IsSerializable)
                {
                    throw new ArgumentException("The type must be serializable.", "source");
                }

                if (Object.ReferenceEquals(source, null))
                {
                    return default(Engine);
                }

                IFormatter formatter = HalconSerializationGuard.CreateFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (Engine)formatter.Deserialize(stream);
                }
            }
        }
        internal static void SwitchScheme(string name)
        {
            Job.isDrawing = true;



            Project.Instance.curEngine = Project.Instance.FindEngineByName(name);
            Frm_EngineManager.Instance.dgv_engineInfo.Rows.Clear();
            for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
            {
                int index = Frm_EngineManager.Instance.dgv_engineInfo.Rows.Add();
                Frm_EngineManager.Instance.dgv_engineInfo.Rows[index].Cells[0].Value = index + 1;
                Frm_EngineManager.Instance.dgv_engineInfo.Rows[index].Cells[1].Value = Project.Instance.curEngine.L_jobList[i].jobName;
            }
            Frm_EngineManager.Instance.tbx_engineName.Text = Project.Instance.curEngine.schemeName;

            if (Project.Instance.curEngine.schemeName == name && Project.Instance.curEngine.schemeName != "未创建")
            {
                Frm_Main.Instance.OutputMsg(string.Format("已由方案 [{0}] 成功切换到方案 [{1}] ", Project.Instance.curEngine.schemeName, name), Color.Black);
                Job.isDrawing = false;
                return;
            }
            //切换当前方案
            Scheme.LoadScheme(Scheme.FindSchemeByName(name));
            Frm_Main.Instance.dockPanel.Visible = true;

            //初始化图像窗口
            for (int i = 0; i < Project.Instance.curEngine.L_jobList.Count; i++)
            {
                HWindowControl dd = new HWindowControl();       //此处必须声明一个全部变量把它存起来，否则会被GC给清理掉
                Frm_Main.d.Add(dd);
                Project.Instance.curEngine.L_jobList[i].www = dd.HalconID;

                Project.Instance.curEngine.L_jobList[i].Run(true);
            }

            Job.isDrawing = false;
        }
        ///////// <summary>
        ///////// 判断是否已经存在此名称的流程
        ///////// </summary>
        ///////// <param name="jobName">流程名</param>
        ///////// <returns>是否已存在</returns>
        //////internal static bool CheckEngineExist(string jobName)
        //////{
        //////    try
        //////    {
        //////        for (int i = 0; i < Project.Instance.L_engineList .Count ; i++)
        //////        {
        //////            if (Project.Instance.L_engineList [i] .engineName == jobName)
        //////            {
        //////                return true;
        //////            }
        //////        }
        //////        return false;
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        Log.SaveError(ex);
        //////        return true;
        //////    }
        //////}
        internal static void CreateScheme()
        {
            try
            {
            Again:
                Frm_InputMessage.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "Please input job's name" : "请输入新方案名");
                Frm_InputMessage.Instance.btn_confirm.Text = (Project.Instance.configuration.language == Language.English ? "Confirm" : "确定");
                //////Frm_InputMessage.Instance.passwordChar = false;
                Frm_InputMessage.Instance.txt_input.DefaultText = "请输入新方案名";
                Frm_InputMessage.Instance.txt_input.TextStr = "示例方案";
                Frm_InputMessage.Instance.ShowDialog();
                string jobName = Frm_InputMessage.input;
                if (jobName == "")
                    return;

                //检查此名称的流程是否已存在
                if (CheckSchemeExist(jobName))
                {
                    Frm_MessageBox.Instance.MessageBoxShow((Project.Instance.configuration.language == Language.English ? "\r\nA process with this name already exists. The process name cannot be repeated. Please enter again" : "\r\n已存在此名称的方案，方案名不可重复，请重新输入"));
                    goto Again;
                }
                //检查此名称是否含有特殊字符\
                if (jobName.Contains(@"\"))
                {
                    Frm_MessageBox.Instance.MessageBoxShow((Project.Instance.configuration.language == Language.English ? "\r\nA process with this name already exists. The process name cannot be repeated. Please enter again" : "\r\n方案名中不能含有 \\ 等特殊字符 ，请重新输入"));
                    goto Again;
                }


                Frm_Main.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "A new process named:" + jobName : "创建了新方案，方案名为：" + jobName, Color.Black);
                Scheme engine = new Scheme();
                engine.schemeName = jobName;
                Project.Instance.L_engineList.Add(engine);

                Frm_EngineManager.Instance.cbx_engineList.Add(jobName);
                Frm_Main.Instance.lbl_curEngine.DropDownItems.Add(jobName);
                if (Frm_EngineManager.Instance.cbx_engineList.Items.Length > 0)
                {
                    Frm_EngineManager.Instance.cbx_engineList.Text = jobName;
                    Frm_Main.Instance.lbl_curEngine.DropDownItems[Frm_Main.Instance.lbl_curEngine.DropDownItems.Count - 1].Select();
                    Frm_Main.Instance.lbl_curEngine.Text = "当前方案：" + jobName;
                }

                Scheme.SwitchScheme(jobName);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 克隆当前方案
        /// </summary>
        internal static void CloneScheme()
        {
            try
            {
            Again:
                Frm_InputMessage.Instance.lbl_title.Text = (Project.Instance.configuration.language == Language.English ? "Please input job's name" : "请输入新方案名");
                Frm_InputMessage.Instance.btn_confirm.Text = (Project.Instance.configuration.language == Language.English ? "Confirm" : "确定");
                Frm_InputMessage.Instance.passwordChar = false;
                Frm_InputMessage.Instance.txt_input.TextStr = string.Empty;
                Frm_InputMessage.Instance.ShowDialog();
                string newEngineName = Frm_InputMessage.input;
                if (newEngineName != string.Empty)
                {
                    //检查此名称的流程是否已存在
                    if (CheckSchemeExist(newEngineName))
                    {
                        Frm_MessageBox.Instance.MessageBoxShow((Project.Instance.configuration.language == Language.English ? "\r\nA process with this name already exists. The process name cannot be repeated. Please enter again" : "\r\n已存在此名称的方案，方案名不可重复，请重新输入"));
                        goto Again;
                    }
                    Frm_Output.Instance.OutputMsg(Project.Instance.configuration.language == Language.English ? "A new process named:" + newEngineName : "克隆了新方案，方案名为：" + newEngineName, Color.Green);

                    string sourceEngineName = Project.Instance.curEngine.schemeName;
                    Scheme engine = ObjectCopier.Clone(Project.Instance.curEngine);
                    engine.schemeName = newEngineName;

                    Project.Instance.L_engineList.Add(engine);
                    //////Job.LoadJob(job);
                    //////LoadJob(Application.StartupPath + "\\Config\\Project\\Vision\\Job\\" + newEngineName + ".job");

                    Frm_Main.Instance.SaveAll();
                    Frm_EngineManager.Instance.cbx_engineList.Add(newEngineName);
                    Frm_Main.Instance.lbl_curEngine.DropDownItems.Add(newEngineName);
                    Frm_Output.Instance.OutputMsg("方案克隆成功", Color.Green);

                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 通过方案名获取方案
        /// </summary>
        /// <param name="jobName">方案名</param>
        /// <returns>方案</returns>
        public static Scheme FindSchemeByName(string engineName)
        {
            try
            {
                for (int i = 0; i < Project.Instance.L_engineList.Count; i++)
                {
                    if (Project.Instance.L_engineList[i].schemeName == engineName)
                        return Project.Instance.L_engineList[i];
                }
                Frm_MessageBox.Instance.MessageBoxShow(Project.Instance.configuration.language == Language.English ? "Can not find job named：" + engineName + "（Error code：0001）" : "未找到名为" + engineName + "的方案（错误代码：00001）");
                return null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 判断是否已经存在此名称的方案
        /// </summary>
        /// <param name="engineName">方案名</param>
        /// <returns>是否已存在</returns>
        private static bool CheckSchemeExist(string engineName)
        {
            try
            {
                try
                {
                    for (int i = 0; i < Project.Instance.L_engineList.Count; i++)
                    {
                        if ((Project.Instance.L_engineList[i]).schemeName == engineName)
                            return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Log.SaveError(ex);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }

    }
}
