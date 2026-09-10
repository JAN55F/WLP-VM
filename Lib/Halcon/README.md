# HALCON 原生核心库

这里保存工程现有部署目录中的 HALCON `17.12.0.1` 原生核心库，与 `Lib/halcondotnet.dll`（`17.12.0.0`）配套。文件从当前工程已有副本复制，未修改二进制内容。

| 目录 | PE 架构 | SHA-256 |
| --- | --- | --- |
| `x64-win64/halcon.dll` | x64 | `5a739b41eef4d7d88abc9b8bbd9a1ec0ce002a1de088c1cd46e2f49588b6015b` |
| `x86sse2-win32/halcon.dll` | x86 | `18ff1fe5e2f1ca3e3ca2aaef341e582a68d7cd22d17eeff76481921cb344c0c8` |

`Start.csproj` 把这两个文件按原架构目录复制到输出目录的 `Halcon/` 下。`Start/HalconRuntime.cs` 根据实际进程位数选择目录，不依赖解决方案配置显示名，也不需要为了查找这个核心库而设置 HALCONROOT。

发布或复制程序时保留完整的 `Halcon/` 目录。不要把两种位数的 DLL 合并到一个目录，也不要只复制 EXE。此处仅补齐核心库，HALCON 授权、相机驱动和采集接口仍由目标运行环境提供。
