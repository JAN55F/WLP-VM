using System;
using System.IO;
using System.Reflection;
using Start;

// 与 Start/HalconRuntime.cs 一起编译运行；参数为仓库根目录。
// 仅验证路径发现和 DLL 元数据，不调用 Windows/HALCON 原生函数或硬件。
internal static class HalconRuntimeSmoke
{
    private static int checks;

    private static void Main(string[] args)
    {
        string repository = Path.GetFullPath(args[0]);
        Version version = AssemblyName.GetAssemblyName(Path.Combine(repository, "Lib", "halcondotnet.dll")).Version;
        string x64 = Path.Combine(repository, "Start", "bin", "Debug", "halcon.dll");
        string x86 = Path.Combine(repository, "VisionAndMotion", "bin", "Debug", "halcon.dll");
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            HalconRuntime.ValidateNativeLibrary(x64, version, true);
            Check(true, "The existing x64 runtime matches the managed version");
            HalconRuntime.ValidateNativeLibrary(x86, version, false);
            Check(true, "The existing x86 runtime matches the managed version");
            Reject<BadImageFormatException>(() => HalconRuntime.ValidateNativeLibrary(x64, new Version(99, 99), true), "Reject unrelated HALCON version");
        }
        else
        {
            // Unix 的 FileVersionInfo 不提供原生 Windows PE 版本资源。
            Console.WriteLine("SKIP: 3 native FileVersionInfo checks require Windows.");
        }
        Reject<BadImageFormatException>(() => HalconRuntime.ValidateNativeLibrary(x86, version, true), "Reject x86 in x64 process");
        Reject<BadImageFormatException>(() => HalconRuntime.ValidateNativeLibrary(x64, version, false), "Reject x64 in x86 process");

        string temporary = Path.Combine(Path.GetTempPath(), "WLP HALCON 路径 " + Guid.NewGuid().ToString("N"));
        string originalDirectory = Directory.GetCurrentDirectory();
        try
        {
            string app = Directory.CreateDirectory(Path.Combine(temporary, "移动后的程序")).FullName;
            string root = Directory.CreateDirectory(Path.Combine(temporary, "HALCON 安装")).FullName;
            string native = Directory.CreateDirectory(Path.Combine(root, "bin", "x64-win64")).FullName;
            string fallback = Directory.CreateDirectory(Path.Combine(temporary, "PATH runtime")).FullName;
            string unrelated = Directory.CreateDirectory(Path.Combine(temporary, "current directory")).FullName;
            File.WriteAllText(Path.Combine(native, "halcon.dll"), "candidate only");
            File.WriteAllText(Path.Combine(fallback, "halcon.dll"), "candidate only");
            File.WriteAllText(Path.Combine(unrelated, "halcon.dll"), "candidate only");
            Directory.SetCurrentDirectory(unrelated);

            var directories = HalconRuntime.GetCandidateDirectories(app, "\"" + root + "\"",
                "\"" + native + "\";" + fallback + ";" + fallback + Path.DirectorySeparatorChar + ";.;\0;", true);
            Check(directories.Count == 3, "Ignore duplicates, relative paths and invalid PATH entries");
            Check(directories[0] == app && directories[1] == native && directories[2] == fallback,
                "Use EXE directory, matching HALCONROOT architecture, then PATH after relocation");
            Check(!directories.Contains(unrelated), "Current directory does not replace EXE directory");

            var withoutRoot = HalconRuntime.GetCandidateDirectories(app, null, fallback, true);
            Check(withoutRoot.Count == 2 && withoutRoot[1] == fallback, "Discover PATH runtime when HALCONROOT is unset");
            var forX86 = HalconRuntime.GetCandidateDirectories(app, root, string.Empty, false);
            Check(forX86[1] == Path.Combine(root, "bin", "x86sse2-win32"), "Select directory from process bitness");
            var driveRoot = HalconRuntime.GetCandidateDirectories(Path.GetPathRoot(app), null, string.Empty, true);
            Check(driveRoot[0] == Path.GetPathRoot(app), "Preserve rooted drive paths");
            Reject<FileNotFoundException>(() => HalconRuntime.ValidateNativeLibrary(Path.Combine(app, "halcon.dll"), version, true), "Report missing native library");
            Reject<BadImageFormatException>(() => HalconRuntime.ValidateNativeLibrary(Path.Combine(native, "halcon.dll"), version, true), "Reject corrupt native library");
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory);
            if (Directory.Exists(temporary))
                Directory.Delete(temporary, true);
        }
        Console.WriteLine("PASS: " + checks + " HALCON path/metadata checks; native loading was not executed.");
    }

    private static void Check(bool condition, string description)
    {
        if (!condition)
            throw new InvalidOperationException(description);
        checks++;
    }

    private static void Reject<T>(Action action, string description) where T : Exception
    {
        try { action(); }
        catch (T) { checks++; return; }
        throw new InvalidOperationException(description);
    }
}
