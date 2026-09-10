using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Globalization;
using HalconDotNet;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading;

namespace VMPro
{
    [Serializable]
    internal class SaveImageTool : ToolBase
    {
        internal SaveImageTool()
        {
            string jobName = "未命名流程";
            try
            {
                if (Frm_Job.Instance.tbc_jobs.SelectedTab != null)
                    jobName = Frm_Job.Instance.tbc_jobs.SelectedTab.Text;
            }
            catch
            {
                // 没有活动流程时仍使用安全的专用默认目录。
            }

            string storageRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "WLP VM 图像");
            imageSavePath = Path.Combine(storageRoot, NormalizePathSegment(jobName, "未命名流程"), "原始图像");
        }

        // 队列满时对调用方施加背压：不丢弃任何一次保存，同时避免窗口截图无限占用本机内存。
        private const int MaxQueuedSaveCount = 64;
        private static readonly TimeSpan CleanupThrottle = TimeSpan.FromMinutes(10);
        private static readonly object SchedulerSync = new object();
        private static readonly object DirectoryOperationGate = new object();
        private static readonly object FileOperationSync = new object();
        private static readonly Queue<Imagee> SaveQueue = new Queue<Imagee>();
        private static readonly Queue<CleanupRequest> CleanupQueue = new Queue<CleanupRequest>();
        private static readonly HashSet<string> PendingSaveFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> PendingCleanupPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, DateTime> LastCleanupUtc = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        private static readonly AutoResetEvent SchedulerSignal = new AutoResetEvent(false);
        private static Thread schedulerThread;

        internal string imageSavePath = string.Empty;
        private new object obj = new object();
        internal string imageFormat = "tif";
        internal int saveDays = 7;
        internal bool expandTime = true;
        internal bool autoClear = true;
        internal bool autoCreateDirectory = true;
        internal string imageName = "图像";
        internal ImageSource imageSource = ImageSource.InputImage;
        internal void ResetTool()
        {
            try
            {
                imageSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "WLP VM 图像");
                imageFormat = "tif";
                saveDays = 7;
                expandTime = true;
                autoClear = true;
                autoCreateDirectory = true;
                imageName = "图像";
                Frm_SaveImageTool.Instance.tbx_imageSavePath.TextStr = imageSavePath;
                Frm_SaveImageTool.Instance.textBox2.TextStr = imageName;
                Frm_SaveImageTool.Instance.comboBox1.TextStr = imageFormat;
                Frm_SaveImageTool.Instance.textBox1.Value = saveDays;
                Frm_SaveImageTool.Instance.checkBox1.Checked = expandTime;
                Frm_SaveImageTool.Instance.checkBox2.Checked = autoClear;
                Frm_SaveImageTool.Instance.checkBox3.Checked = autoCreateDirectory;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private sealed class CleanupRequest
        {
            internal string RootPath;
            internal int SaveDays;
        }

        /// <summary>
        /// 判断指定目录下是否仍有排队或正在写入的图像。
        /// 该方法不改变队列状态，供删除入口在执行前进行只读防护。
        /// </summary>
        internal static bool HasPendingSaveUnder(string path)
        {
            string normalizedPath;
            if (!TryNormalizeDirectoryPath(path, out normalizedPath))
                return false;

            lock (SchedulerSync)
            {
                return HasPendingSaveUnderLocked(normalizedPath);
            }
        }

        /// <summary>
        /// 在没有待写图且磁盘工作线程空闲时，原子执行目录操作。
        /// 已有写图/清理时立即返回 false，不阻塞 UI 等待后台任务。
        /// </summary>
        internal static bool TryExecuteDirectoryOperationWhenIdle(string path, Action operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            string normalizedPath;
            if (!TryNormalizeDirectoryPath(path, out normalizedPath))
                return false;

            // 先阻止新的保存预约，再确认当前没有正在执行的磁盘操作；两步都采用快速失败。
            if (!Monitor.TryEnter(DirectoryOperationGate))
                return false;

            try
            {
                if (!Monitor.TryEnter(FileOperationSync))
                    return false;

                try
                {
                    lock (SchedulerSync)
                    {
                        if (HasPendingSaveUnderLocked(normalizedPath))
                            return false;
                    }

                    operation();
                    return true;
                }
                finally
                {
                    Monitor.Exit(FileOperationSync);
                }
            }
            finally
            {
                Monitor.Exit(DirectoryOperationGate);
            }
        }

        private static bool HasPendingSaveUnderLocked(string normalizedPath)
        {
            foreach (string fileName in PendingSaveFiles)
            {
                if (IsFileUnderDirectory(fileName, normalizedPath))
                    return true;
            }
            return false;
        }

        private static bool TryNormalizeDirectoryPath(string path, out string normalizedPath)
        {
            normalizedPath = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return false;

                string fullPath = Path.GetFullPath(path.Trim());
                string rootPath = Path.GetPathRoot(fullPath);
                string trimmedPath = fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string trimmedRoot = (rootPath ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                normalizedPath = string.Equals(trimmedPath, trimmedRoot, StringComparison.OrdinalIgnoreCase)
                    ? rootPath
                    : trimmedPath;
                return !string.IsNullOrEmpty(normalizedPath);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsFileUnderDirectory(string fileName, string normalizedDirectory)
        {
            try
            {
                string fullFileName = Path.GetFullPath(fileName);
                string prefix = normalizedDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                return fullFileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string NormalizeImageName(string configuredName)
        {
            return NormalizePathSegment(configuredName, "图像");
        }

        private static string NormalizeImageFormat(string configuredFormat)
        {
            string format = (configuredFormat ?? string.Empty).Trim().TrimStart('.').ToLowerInvariant();
            if (format == "tiff")
                return "tif";

            if (format == "tif" || format == "bmp" || format == "jpg" || format == "png")
                return format;

            return "tif";
        }

        private static string GetHalconImageFormat(string normalizedFormat)
        {
            return normalizedFormat == "tif" ? "tiff" : normalizedFormat;
        }

        private static string NormalizePathSegment(string configuredValue, string fallback)
        {
            string value = (configuredValue ?? string.Empty).Trim();
            if (value.Length == 0)
                value = fallback;

            char[] invalidChars = Path.GetInvalidFileNameChars();
            StringBuilder builder = new StringBuilder(value.Length);
            foreach (char character in value)
                builder.Append(Array.IndexOf(invalidChars, character) >= 0 ? '_' : character);

            value = builder.ToString().Trim().TrimEnd('.');
            return value.Length == 0 || value == "." || value == ".." ? fallback : value;
        }

        private static string MakeSuffixedFileName(string originalFileName, int suffix)
        {
            string directory = Path.GetDirectoryName(originalFileName);
            string extension = Path.GetExtension(originalFileName);
            string baseName = Path.GetFileNameWithoutExtension(originalFileName);
            return Path.Combine(directory, string.Format("{0}_{1}{2}", baseName, suffix, extension));
        }

        private static void EnqueueSave(Imagee image)
        {
            lock (DirectoryOperationGate)
            {
                string originalFileName = Path.GetFullPath(image.fileName);
                string candidate = originalFileName;
                int suffix = 2;
                while (true)
                {
                    // UNC/网络目录上的 File.Exists 可能较慢，不在调度锁内执行。
                    if (File.Exists(candidate))
                    {
                        candidate = MakeSuffixedFileName(originalFileName, suffix++);
                        continue;
                    }

                    lock (SchedulerSync)
                    {
                        EnsureSchedulerLocked();
                        while (SaveQueue.Count >= MaxQueuedSaveCount)
                        {
                            // 不丢图；磁盘持续慢于生产速度时，最多缓存固定数量，随后让生产方等待空位。
                            Monitor.Wait(SchedulerSync);
                            EnsureSchedulerLocked();
                        }

                        if (PendingSaveFiles.Contains(candidate))
                        {
                            candidate = MakeSuffixedFileName(originalFileName, suffix++);
                            continue;
                        }

                        image.fileName = candidate;
                        SaveQueue.Enqueue(image);
                        PendingSaveFiles.Add(image.fileName);
                        break;
                    }
                }
            }
            SchedulerSignal.Set();
        }

        private static void ScheduleCleanup(string path, int days)
        {
            if (days <= 0)
                return;

            string normalizedPath;
            if (!TryNormalizeDirectoryPath(path, out normalizedPath))
                return;

            string rootPath = Path.GetPathRoot(normalizedPath);
            if (string.Equals(normalizedPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                              (rootPath ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                              StringComparison.OrdinalIgnoreCase))
                return;

            lock (SchedulerSync)
            {
                DateTime now = DateTime.UtcNow;
                DateTime lastCleanup;
                if (PendingCleanupPaths.Contains(normalizedPath) ||
                    (LastCleanupUtc.TryGetValue(normalizedPath, out lastCleanup) && now - lastCleanup < CleanupThrottle))
                    return;

                CleanupQueue.Enqueue(new CleanupRequest { RootPath = normalizedPath, SaveDays = days });
                PendingCleanupPaths.Add(normalizedPath);
                LastCleanupUtc[normalizedPath] = now;
                EnsureSchedulerLocked();
            }
            SchedulerSignal.Set();
        }

        private static void EnsureSchedulerLocked()
        {
            if (schedulerThread != null && schedulerThread.IsAlive)
                return;

            schedulerThread = new Thread(ProcessScheduledWork);
            schedulerThread.IsBackground = true;
            schedulerThread.Name = "WLP VM image storage";
            schedulerThread.Priority = ThreadPriority.BelowNormal;
            schedulerThread.Start();
        }

        private static void ProcessScheduledWork()
        {
            while (true)
            {
                Imagee image = new Imagee();
                CleanupRequest cleanup = null;
                bool hasImage = false;

                lock (SchedulerSync)
                {
                    // 写图优先，避免磁盘清理阻塞生产图像队列。
                    if (SaveQueue.Count > 0)
                    {
                        image = SaveQueue.Dequeue();
                        hasImage = true;
                        Monitor.PulseAll(SchedulerSync);
                    }
                    else if (CleanupQueue.Count > 0)
                    {
                        cleanup = CleanupQueue.Dequeue();
                    }
                }

                if (hasImage)
                {
                    WriteQueuedImage(image);
                    continue;
                }

                if (cleanup != null)
                {
                    DeleteExpiredDirectories(cleanup);
                    continue;
                }

                SchedulerSignal.WaitOne();
            }
        }

        private static void WriteQueuedImage(Imagee image)
        {
            try
            {
                lock (FileOperationSync)
                {
                    string directory = Path.GetDirectoryName(image.fileName);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    HOperatorSet.WriteImage(image.image, image.format, 0, image.fileName);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
            finally
            {
                if (image.disposeAfterWrite)
                    DisposeImageSafely(image.image);

                lock (SchedulerSync)
                    PendingSaveFiles.Remove(image.fileName);
            }
        }

        private static void DisposeImageSafely(HObject image)
        {
            if (object.ReferenceEquals(image, null))
                return;

            try
            {
                image.Dispose();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        private static void DeleteExpiredDirectories(CleanupRequest request)
        {
            bool rescheduled = false;
            try
            {
                lock (FileOperationSync)
                {
                    if (!Directory.Exists(request.RootPath) || request.SaveDays <= 0)
                        return;

                    DateTime expirationDate = DateTime.Today.AddDays(-request.SaveDays);
                    foreach (string directoryPath in Directory.GetDirectories(request.RootPath))
                    {
                        try
                        {
                            lock (SchedulerSync)
                            {
                                if (SaveQueue.Count > 0)
                                {
                                    // 清理只利用写图空档；生产图像到达时把本次清理放回队尾。
                                    CleanupQueue.Enqueue(request);
                                    rescheduled = true;
                                    return;
                                }
                            }

                            DirectoryInfo directory = new DirectoryInfo(directoryPath);
                            DateTime directoryDate;
                            if (!DateTime.TryParseExact(directory.Name, "yyyy_MM_dd", CultureInfo.InvariantCulture,
                                                        DateTimeStyles.None, out directoryDate) ||
                                directoryDate.Date >= expirationDate)
                                continue;

                            // 不递归进入联接、符号链接等重解析目录，避免越过配置的保存根目录。
                            if ((directory.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                                continue;

                            lock (SchedulerSync)
                            {
                                if (HasPendingSaveUnderLocked(directory.FullName))
                                    continue;
                            }

                            Directory.Delete(directory.FullName, true);
                        }
                        catch (Exception ex)
                        {
                            Log.SaveError(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
            finally
            {
                if (!rescheduled)
                {
                    lock (SchedulerSync)
                        PendingCleanupPaths.Remove(request.RootPath);
                }
            }
        }
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
                    string storagePath;
                    if (!TryNormalizeDirectoryPath(imageSavePath, out storagePath))
                        throw new InvalidOperationException("图像保存路径为空或无效。");

                    string targetDirectory = storagePath;
                    if (autoCreateDirectory)
                        targetDirectory = Path.Combine(storagePath, DateTime.Now.ToString("yyyy_MM_dd"));

                    string fileName = string.Empty;
                    string normalizedImageName = NormalizeImageName(imageName);
                    string normalizedImageFormat = NormalizeImageFormat(imageFormat);
                    string halconImageFormat = GetHalconImageFormat(normalizedImageFormat);
                    if (expandTime)
                    {
                        fileName = Path.Combine(targetDirectory,
                            normalizedImageName + " " + DateTime.Now.ToString("HH_mm_ss ffff") + "." + normalizedImageFormat);
                    }
                    else
                    {
                        int index;
                        for (index = 1; index < 10000; index++)
                        {
                            fileName = Path.Combine(targetDirectory, normalizedImageName + index + "." + normalizedImageFormat);
                            if (!File.Exists(fileName))
                                break;
                        }
                    }

                    HObject image = null;

                    if (imageSource == ImageSource.WindowImage)
                    {
                        Frm_ImageWindow ff = GetImageWindowControl();

                        if (ff != null)
                        {
                            try
                            {
                                HOperatorSet.DumpWindowImage(out image, ff.WindowHandle);

                                Imagee iii = new Imagee();
                                iii.image = image;
                                iii.format = halconImageFormat;
                                iii.fileName = fileName;
                                iii.disposeAfterWrite = true;

                                EnqueueSave(iii);
                                image = null; // 所有权已移交给保存工作线程。
                            }
                            finally
                            {
                                // 入队前发生异常时由当前调用释放；成功入队后由工作线程释放。
                                DisposeImageSafely(image);
                            }
                        }
                        else
                        {
                            Frm_Output.Instance.OutputMsg("请先选中图像窗口", Color.Red);
                        }
                    }
                    else
                    {
                        // 输入图像归上游所有，保存队列仅借用引用，写入完成后不得释放。
                        Imagee iii = new Imagee();
                        iii.image = toolPar.InputPar.图像;
                        iii.format = halconImageFormat;
                        iii.fileName = fileName;
                        iii.disposeAfterWrite = false;

                        EnqueueSave(iii);
                    }

                    if (autoClear && saveDays > 0)
                        ScheduleCleanup(storagePath, saveDays);


                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        [Serializable]
        public struct Imagee
        {
            public HObject image;
            public string format;
            public string fileName;
            public bool disposeAfterWrite;
        }
        internal ToolPar toolPar = new ToolPar();

        [Serializable]
        public class ToolPar : ToolParBase
        {
            private InputPar _inputPar = new InputPar();

            public InputPar InputPar
            {
                get { return _inputPar; }
                set { _inputPar = value; }
            }
            private RunPar _runPar = new RunPar();

            public RunPar RunPar
            {
                get { return _runPar; }
                set { _runPar = value; }
            }
            private ResultPar _resultPar = new ResultPar();

            public ResultPar ResultPar
            {
                get { return _resultPar; }
                set { _resultPar = value; }
            }
        }
        [Serializable]
        public class InputPar
        {
            private HObject _图像;
            public HObject 图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {
        }



    }
    internal enum ImageSource
    {
        InputImage,
        WindowImage,
    }
}
