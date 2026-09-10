using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Start
{
    internal static class HalconRuntime
    {
        // 保留到进程退出，后续 halcondotnet 的 P/Invoke 复用同一个原生模块。
        private static IntPtr module;

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryExW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string fileName, IntPtr file, uint flags);

        internal static void EnsureLoaded()
        {
            if (module != IntPtr.Zero)
                return;

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string managedPath = Path.Combine(appDirectory, "halcondotnet.dll");
            if (!File.Exists(managedPath))
                throw new FileNotFoundException("启动目录缺少 HALCON .NET 程序集：\r\n" + managedPath);

            Version managedVersion = AssemblyName.GetAssemblyName(managedPath).Version;
            bool is64Bit = Environment.Is64BitProcess;
            string halconRoot = Environment.GetEnvironmentVariable("HALCONROOT");
            string oldPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            List<string> directories = GetCandidateDirectories(appDirectory, halconRoot, oldPath, is64Bit);
            List<string> failures = new List<string>();

            foreach (string directory in directories)
            {
                string nativePath = Path.Combine(directory, "halcon.dll");
                try
                {
                    ValidateNativeLibrary(nativePath, managedVersion, is64Bit);
                    // 从绝对路径加载，并让该 DLL 的同目录依赖参与本次查找。
                    // 不调用 SetDllDirectory，避免覆盖相机 MVision 适配器的目录。
                    IntPtr loaded = LoadLibraryEx(nativePath, IntPtr.Zero, 0x00000008);
                    if (loaded == IntPtr.Zero)
                    {
                        int error = Marshal.GetLastWin32Error();
                        failures.Add(nativePath + "\r\n  Win32 " + error + ": " + new Win32Exception(error).Message);
                        continue;
                    }

                    module = loaded;
                    // 只更新本进程，供 HALCON 后续按需加载其他运行库使用。
                    Environment.SetEnvironmentVariable("PATH", directory + ";" + oldPath);
                    Trace.WriteLine("HALCON runtime loaded: " + nativePath);
                    return;
                }
                catch (Exception ex)
                {
                    failures.Add(nativePath + "\r\n  " + ex.Message);
                }
            }

            throw new DllNotFoundException(
                "无法加载 HALCON 原生运行库。\r\n" +
                "程序目录：" + appDirectory + "\r\n" +
                "当前进程：" + (is64Bit ? "64 位" : "32 位") + "\r\n" +
                "halcondotnet 版本：" + managedVersion + "\r\n" +
                "HALCONROOT：" + (halconRoot ?? "未设置") + "\r\n\r\n" +
                string.Join("\r\n", failures.ToArray()) + "\r\n\r\n" +
                "请配置同版本、同位数的完整 HALCON 运行环境。Win32 126 也可能表示依赖 DLL 缺失；" +
                "修改系统环境变量后，需要重新启动 Visual Studio 和程序。");
        }

        internal static List<string> GetCandidateDirectories(string appDirectory, string halconRoot, string path, bool is64Bit)
        {
            List<string> directories = new List<string>();
            AddDirectory(directories, appDirectory);
            string root = NormalizeDirectory(halconRoot);
            if (root != null)
            {
                AddDirectory(directories, Path.Combine(root, "bin", is64Bit ? "x64-win64" : "x86sse2-win32"));
            }

            foreach (string entry in (path ?? string.Empty).Split(';'))
            {
                string directory = NormalizeDirectory(entry);
                // 普通系统 PATH 项不进入错误清单，只加入实际含 HALCON 的目录。
                if (directory != null && File.Exists(Path.Combine(directory, "halcon.dll")))
                    AddDirectory(directories, directory);
            }
            return directories;
        }

        private static void AddDirectory(List<string> directories, string directory)
        {
            string fullPath = NormalizeDirectory(directory);
            if (fullPath != null && !directories.Exists(value => string.Equals(value, fullPath, StringComparison.OrdinalIgnoreCase)))
                directories.Add(fullPath);
        }

        private static string NormalizeDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
                return null;
            try
            {
                string expanded = Environment.ExpandEnvironmentVariables(directory.Trim().Trim('"'));
                if (!Path.IsPathRooted(expanded))
                    return null;
                string fullPath = Path.GetFullPath(expanded);
                string root = Path.GetPathRoot(fullPath);
                return fullPath.Length > root.Length
                    ? fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    : fullPath;
            }
            catch (ArgumentException) { return null; }
            catch (NotSupportedException) { return null; }
            catch (PathTooLongException) { return null; }
        }

        internal static void ValidateNativeLibrary(string path, Version managedVersion, bool is64Bit)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("文件不存在。");

            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                if (reader.BaseStream.Length < 64 || reader.ReadUInt16() != 0x5a4d)
                    throw new BadImageFormatException("不是有效的 Windows DLL。");
                reader.BaseStream.Position = 0x3c;
                int peOffset = reader.ReadInt32();
                if (peOffset < 0 || peOffset > reader.BaseStream.Length - 6)
                    throw new BadImageFormatException("DLL 的 PE 头无效。");
                reader.BaseStream.Position = peOffset;
                if (reader.ReadUInt32() != 0x00004550)
                    throw new BadImageFormatException("DLL 的 PE 签名无效。");
                ushort machine = reader.ReadUInt16();
                if (machine != (is64Bit ? 0x8664 : 0x014c))
                    throw new BadImageFormatException("DLL 位数与当前 " + (is64Bit ? "64" : "32") + " 位进程不匹配。");
            }

            FileVersionInfo nativeVersion = FileVersionInfo.GetVersionInfo(path);
            if (nativeVersion.FileMajorPart != managedVersion.Major || nativeVersion.FileMinorPart != managedVersion.Minor)
                throw new BadImageFormatException("原生版本 " + nativeVersion.FileVersion +
                    " 与 halcondotnet " + managedVersion + " 不属于同一 HALCON 版本系列。");
        }
    }
}
