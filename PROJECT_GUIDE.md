# WLP VM Project Guide

> 最后更新时间：2026-09-10（Asia/Shanghai）
>
> 后续定位代码时先读本文件。它是快速导览，不替代源码；如果导览和源码不一致，以源码为准，并同步更新本文件。

## 1. 解决方案和项目

主解决方案：`VM Pro.sln`，Visual Studio 2013 格式。

| 项目 | 路径 | 作用 |
| --- | --- | --- |
| Start | `Start/Start.csproj` | WinForms 启动项目，程序入口在 `Start/Program.cs`；输出 `WLP VM.exe`，应用图标为 `Start/WLPVM.ico`。 |
| VMPro | `VisionAndMotion/VMPro.csproj` | 主业务项目：流程、工具、设备管理、主界面。绝大多数修改在这里。 |
| 1 WinFormsUI | `DockForm/WinFormsUI/1 WinFormsUI.csproj` | DockPanel 停靠窗口库。通常不改。 |
| HWindow_Tool | `ImageWindow/HWindow_Tool.csproj` | Halcon 图像窗口/ROI 显示控件。 |
| LightController | `LightController/LightController.csproj` | 光源控制器抽象和实现。 |
| Controls | `ControlLib/Controls/Controls.csproj` | 自定义 WinForms 控件。 |
| FastColoredTextBox | `CodeEdit/FastColoredTextBox/2 FastColoredTextBox.csproj` | 脚本编辑器控件，已纳入主解决方案。 |

常见附属解决方案：

- `CodeEdit/SharpEdit.sln`：脚本编辑器相关。
- `MyChart/MyChart.sln`：图表控件/工具。
- `ImageWindow/HWindow_Tool.sln`：图像窗口单独工程。

构建注意：

- 使用 Visual Studio 2022 Build Tools 的完整 MSBuild 构建旧式 .NET Framework 解决方案，不使用 `dotnet build`。当前可复现入口为 `MSBuild.exe "VM Pro.sln" /t:Build /p:Configuration=Debug /p:Platform="Mixed Platforms"`。
- `Debug|Mixed Platforms` 下，VMPro 为 AnyCPU；Start 在解决方案中的配置名映射为 `Debug|x86`，但 `Start.csproj` 的该 PropertyGroup 明确设置 `PlatformTarget=AnyCPU`。`Release|x86` 是现有兼容配置，不代表本轮已完成所有原生 SDK 的发布验证。
- `VM Pro.sln` 原先在 `Debug|Any CPU` 和 `Debug|x64` 下只给 Start 设置 `ActiveCfg`，遗漏 `Build.0`，解决方案生成会跳过启动项目，导致业务 DLL 更新而 EXE 及其 HALCON 内容复制仍未更新。本轮补齐这两项，继续映射到既有 `Start Debug|x86`（实际 AnyCPU），不改项目位数或 Release 映射。使用 MSBuild `SolutionFile.Parse()` 对照修复前后，确认四种 Debug 配置均包含 Start 构建，其他映射保持原样。
- 使用 `/t:Build`，不要把 `/t:Rebuild` 或单独 `/t:Compile` 当作正常验证路径：仓库包含依赖现有输出目录/外部 SDK 的历史引用，清理后不一定能从纯净环境完整重建。
- 不要在解决方案或项目构建命令中全局传入 `/p:TargetFrameworkVersion=v4.7.2`。该属性会传播到项目引用，把原本分别面向 4.0/4.5/4.5.2 的 `WeifenLuo.WinFormsUI.Docking.dll`、`FastColoredTextBox.dll`、`Controls.dll`、`HWindow_Tool.dll` 和 `CVMPro.dll` 一并写成 4.7.2；随后按 4.5.2 构建 `Start` 时会拒绝解析 `CVMPro.dll`，`VMPro` 命名空间缺失只是连锁错误。若共享 `VisionAndMotion/bin/Debug` 已被这样污染，保留源码中的原始 `TargetFrameworkVersion`，执行上一条解决方案级 `Debug|Mixed Platforms` `/t:Build` 即可恢复；不要先清空 `bin/obj`，因为历史 SDK 引用依赖现有输出目录。恢复后应核对 `CVMPro.dll` 为 4.5.2、DockPanel/FastColoredTextBox 为 4.0、Controls 为 4.5，并重新加载 Visual Studio 解决方案以清除旧诊断。
- 不要把全局 `/p:OutDir=...` 直接传给 `VMPro.csproj` 的整棵项目引用执行 `/t:Rebuild`：该属性会传播到 `HWindow_Tool`，改变历史 HALCON 程序集的解析位置。本机无硬件复核中由此产生 127 个引用错误。需要隔离壳层验证时，先在正常配置下验证项目引用，再用 `Start.csproj /p:BuildProjectReferences=false /p:OutDir=...` 生成隔离启动目录，并核对复制进去的 `CVMPro.dll`、`HWindow_Tool.dll` 时间戳或哈希。
- 当前项目依赖 .NET Framework 3.5/4.0/4.5/4.5.2、Halcon、相机 SDK、运动控制 SDK、HslCommunication 等 Windows/VS 环境组件。
- 本目录现在是 Git 仓库根目录，可直接使用 `git status`、`git diff --check` 检查改动；不要把 `bin/`、`obj/` 的历史产物当成源码证据。
- 2026-09-10 核对的 GitHub 上游为 `github-new/master`（`https://github.com/JAN55F/WLP-VM.git`），当前本地分支为 `master`；`origin` 指向历史仓库 `supermoonper/VMPro`，推送前应核对远程地址，不能仅凭 `origin` 名称选择目标。

### 1.1 HALCON 原生库加载与路径排查（2026-09-10）

- 当前修改对象是 `/Volumes/LiJian/WellPull/JAN-正在开发/VM Pro -NEW` 根目录的 `VM Pro.sln`；仓库中还有嵌套的 `VM Pro -NEW/` 副本，本轮未同步修改该副本。
- `Start/Program.cs` 先调用 `Start/HalconRuntime.cs` 的 `EnsureLoaded()`，成功后才进入禁止内联的 `RunApplication()`，读取项目配置并创建主界面。按 `EXE目录/Halcon/<进程架构>`、实际 EXE 目录、`HALCONROOT/bin/<进程架构>`、含 `halcon.dll` 的 `PATH` 项依次探测；架构根据 `Environment.Is64BitProcess` 选择 `x64-win64` 或 `x86sse2-win32`，不能仅凭解决方案配置名判断。
- 每个候选 DLL 先校验 PE 位数及与 `halcondotnet.dll` 相同的主/次版本系列，再使用绝对路径 `LoadLibraryExW(..., LOAD_WITH_ALTERED_SEARCH_PATH)` 加载；保留模块句柄，并只将成功目录加入本进程 PATH，不修改系统环境变量、当前工作目录或相机适配器的 `SetDllDirectory`。失败时显示实际程序目录、进程位数、托管版本、HALCONROOT 和逐个候选的失败原因/Win32 错误码。
- 用户后续 Windows 截图确认：64 位进程使用 `halcondotnet 17.12.0.0`，EXE 目录中的 `halcon.dll` 不存在，`HALCONROOT` 未设置，PATH 也未发现候选。此前 Git 只跟踪了托管封装 `Lib/halcondotnet.dll`，原生库仅位于被 `.gitignore` 排除的 `bin/` 中，因此拉取上一轮启动检查代码仍不能补齐缺文件。
- 原生库补齐后用户再次发来的截图仍只列 `Debug/halcon.dll`，没有 `Debug/Halcon/x64-win64/halcon.dll`，说明实际运行的启动检查尚未包含 `67accf2` 的候选路径逻辑；新代码即使文件不存在，也会显示该候选。先区分源码已拉取与 EXE 已重新生成：重新加载根解决方案，设 Start 为启动项目，右键 Start 执行“生成”，再运行。当前错误窗口标题由 `HalconRuntime.DiagnosticTitle` 标为 `WLP VM - HALCON 启动检查 v3`；若仍出现无 v3 的旧标题，应核对正在运行的 EXE 路径、生成错误及配置管理器中 Start 的“生成”勾选，而不是继续修改 DLL 搜索路径。
- 本轮将当前工程已有的 x64、x86 `halcon.dll` 原样保存到受版本管理的 `Lib/Halcon/x64-win64/`、`Lib/Halcon/x86sse2-win32/`，原生版本均为 `17.12.0.1`，托管封装为 `17.12.0.0`；来源副本分别为 `Start/bin/Debug` 和 `VisionAndMotion/bin/Debug`。`Lib/Halcon/README.md` 记录两份 SHA-256。`Start.csproj` 使用显式 Content + Link + PreserveNewest，构建时复制到 `输出目录/Halcon/<架构>/halcon.dll`；同时保留两种架构，运行时选择，不依赖构建机位数。复制程序时必须带上整个 Halcon 子目录；不要合并两种架构或只复制 EXE。
- `HWindow_Final.ClearWindow()` 报 `DllNotFoundException / 0x8007007E` 时，先确认目标 Windows 上实际 EXE 的完整路径与加载检查结果。`halcondotnet.dll` 是托管封装，不能替代原生 `halcon.dll`；126 也可能是后者的依赖缺失，仅凭截图不能确定是路径还是依赖。不要以吞掉清屏异常作为修复。修改系统环境变量后重新启动 Visual Studio，使新进程继承环境。加载规则参考 [Microsoft DLL 搜索顺序](https://learn.microsoft.com/en-us/windows/win32/dlls/dynamic-link-library-search-order) 与 [LoadLibraryExW](https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibraryexw)。
- `Tests/HalconRuntimeSmoke.cs` 与 `Start/HalconRuntime.cs` 一起编译，入口参数为仓库根目录；位数/版本样本改读受版本管理的 `Lib/Halcon`，不再依赖被忽略的 bin 文件。macOS 已通过 11 项路径/拒绝错误 DLL 检查，新增覆盖 HALCONROOT/PATH 均为空且根目录缺 DLL 时仍能发现随程序复制的核心库；原有移动目录、中文/空格/引号路径、重复/非法 PATH、工作目录隔离和错误 DLL 检查仍通过。实际执行 `Start.csproj` 的 AssignTargetPaths 并将两个 Content 项复制到临时目录，确认目标路径和 SHA-256 一致；这仅验证 MSBuild 内容复制。原生 Windows PE 版本资源的 3 项检查因 Unix `FileVersionInfo` 限制跳过；原生加载、授权、窗口绘制和相机行为仍须 Windows 验收。启动源码通过 C# 5 + .NET Framework 4.8 参考程序集的隔离编译，不代表原项目 4.5.2 的 Windows 完整构建已通过。
- 本轮进一步直接执行 `Start.csproj` 的标准 `_CopySourceItemsToOutputDirectory` 目标（关闭项目引用构建，输出/中间目录隔离到临时目录），两种原生 DLL 都出现在预期子目录且 SHA-256 一致。这验证了项目自带的内容复制链路，无须再增加另一套复制脚本；仍不替代 Windows 完整生成与原生加载验证。

## 2. 顶层源码结构

| 目录 | 作用 |
| --- | --- |
| `Start/` | 程序入口和启动前检查。 |
| `VisionAndMotion/1 ToolLib/` | 所有流程工具，每个工具通常包含 `xxxTool.cs` 和 `Frm_xxxTool.cs`。 |
| `VisionAndMotion/2 ClassLib/` | 核心业务类：项目、流程、工具基类、设备、通讯、运动控制、日志。 |
| `VisionAndMotion/3 FormLib/` | 主界面、设备管理、设置页、流程窗口等公共窗体。 |
| `VisionAndMotion/4 EditPart/` | 项目定制逻辑和用户表单，例如 `Task_SmartLineA.cs`。 |
| `VisionAndMotion/Resources/` | 图标和界面资源。 |
| `ImageWindow/` | Halcon 视图、ROI、序列化辅助。 |
| `LightController/` | 光源控制库。 |
| `ControlLib/Controls/` | 自定义控件库。 |
| `CodeEdit/FastColoredTextBox/` | 主解决方案使用的脚本编辑控件源码。 |
| `Tests/` | 不启动完整程序的布局生成器和 UI 壳层烟测。 |
| `packages/` | NuGet 包，当前含 HslCommunication、Newtonsoft.Json 等。 |

## 3. 程序启动和项目生命周期

| 功能 | 入口文件 | 核心函数/位置 |
| --- | --- | --- |
| 程序启动 | `Start/Program.cs`、`Start/HalconRuntime.cs` | `Main()`：先加载匹配的 HALCON 原生库；`RunApplication()` 从 `Config\Config.ini` 预读语言，处理单实例提示，再进入主程序初始化。 |
| 主程序初始化 | `VisionAndMotion/2 ClassLib/VM.cs` | `Init()`：先执行 `Configuration.Read(false)`，再显示欢迎页并实例化主窗体；后台启动 `Machine.InitAll()`，等待 `Machine.loading == false` 后先隐藏欢迎页，再显示主窗体。 |
| 设备初始化 | `VisionAndMotion/2 ClassLib/Machine.cs` | `InitAll()`：启动时初始化硬件、自动连接设备。 |
| 项目单例和序列化 | `VisionAndMotion/2 ClassLib/Project.cs` | `Project.Instance`、`LoadProject()`、`InportProject()`、`ExportProject()`、`EnsureCommunicationRuntime()`。 |
| 配置项 | `VisionAndMotion/2 ClassLib/Configuration.cs` | 程序标题、语言、运行参数等。 |
| 方案 | `VisionAndMotion/2 ClassLib/Scheme.cs` | 方案下挂多个 Job。 |

运行目录中有三份名称相近但职责不同的 INI，不能合并，也不要新增第四份同名配置：

| 运行时路径 | 主要读写者 | 职责 |
| --- | --- | --- |
| `Config\Config.ini` | `Start/Program.cs`、`Machine.EnsureFolder()` | 启动最早阶段预读语言，供主配置载入前的单实例/错误提示使用，并作为 `Config` 目录下的启动基础配置文件。 |
| `Config\Configuration.ini` | `Configuration.cs`、`VM.Init()` | 最终语言、程序标题、运行参数、工作区开关、当前 Dock 布局等主业务配置；在欢迎页/主窗体构造前由 `Configuration.Read(false)` 读取。 |
| `Config.ini` | `Frm_Main.cs`、`Frm_Regiest.cs`、`Frm_UserManager.cs` | 程序根目录级注册信息；用户管理窗体仍保留该历史 INI 实例，但当前未见实际读写。 |

定位提示：

- 加载 `.pjt` 项目文件的问题，先看 `Project.LoadProject()` 和 `Project.InportProject()`。
- 项目保存统一由 `Project.SaveProjectToPath()` 写入：先序列化到同目录 `.tmp`，再替换正式 `.pjt`，并保留一个 `.bak` 上次版本；不要再用 `OpenOrCreate` 直接覆盖，也不要删除 `Config\Project\Vision` 中的其他项目文件。
- `Config\LastProject.txt` 保存最后一次成功打开或保存的 `.pjt` 路径。启动时 `Machine.InitAll()` 调用 `Project.LoadStartupProject()` 优先恢复该文件；记录失效时回退到 `Config\Project\Vision` 中最近修改的项目，主文件加载失败时再尝试 `.bak`。
- `.pjt` 保存完整项目（全部方案、流程及工具），`.eng` 只保存一个方案。`Scheme.OpenScheme()`/`LoadScheme()` 必须把 `.eng` 反序列化为 `Scheme` 并写回当前项目，不能把 `.eng` 当成 `Project`，也不能再用 `Frm_Main.openProject` 阻止后续项目保存。
- 启动后设备没有恢复/连接，先看 `Project.EnsureCommunicationRuntime()` 和 `Machine.InitAll()`。
- 菜单、快捷栏和欢迎页会在构造时读取语言，因此 `VM.Init()` 必须在创建这些窗体前执行 `Configuration.Read(false)`。`Start/Program.cs` 对 `Config\Config.ini` 的读取只是最早提示所需的语言预读，最终值来自 `Config\Configuration.ini`；`Machine.InitAll()` 不得再次调用 `Configuration.Read()`，否则列表型配置可能重复，且首次壳层语言会与最终配置不一致。
- 卡在“启动成功”欢迎页不进入主界面时，先看 `Machine.InitAll()` 末尾和 `VM.Init()` 的等待循环：`VM.Init()` 只有等 `Machine.loading == false` 才会进入主窗体。欢迎页文字设为“启动成功”以后，应立即 `loading = false`，并由 `VM.Init()` 在主线程调用 `Frm_Welcome.Instance.Hide()` 后再 `frm.ShowDialog()`。
- 启动阶段不要预跑流程。旧逻辑曾在 `Machine.InitAll()` 末尾为每个 Job 创建临时 `HWindowControl`，把 `Project.Instance.curEngine.L_jobList[i].www` 指到该窗口句柄后调用 `Run(true)`；如果任意流程预跑访问相机、模板、Halcon 窗口或外设时卡住，会导致欢迎页或主窗体切换死锁。需要预热图像或流程时，应放到主窗体完全显示后的显式操作中，不要阻塞启动链路。

## 4. 流程和工具运行主线

| 概念 | 文件 | 说明 |
| --- | --- | --- |
| 流程 Job | `VisionAndMotion/2 ClassLib/Job.cs` | 流程编辑、工具列表、工具运行、输入输出流转的核心大文件。 |
| 工具信息 | `VisionAndMotion/2 ClassLib/ToolInfo.cs` | 保存工具类型、名称、启用状态、输入输出项和工具对象。 |
| 工具基类 | `VisionAndMotion/2 ClassLib/ToolBase.cs` | `Run()` 虚函数、工具状态 `ToolRunStatu`、公共辅助。 |
| 工具类型枚举 | `VisionAndMotion/2 ClassLib/DataStrct.cs` | `ToolType` 等结构和枚举。 |
| 工具箱/新增工具 | `VisionAndMotion/3 FormLib/Frm_ToolBox.cs` | 按工具名称创建 `ToolInfo`、添加输入输出节点、构建工具箱树。 |
| 流程界面 | `VisionAndMotion/3 FormLib/Frm_Job.cs` | 流程 UI、运行按钮、流程树承载。 |
| 输入输出配置 | `VisionAndMotion/3 FormLib/Frm_IOConfig.cs` | 工具输入输出连接配置。 |

`Job.cs` 常用定位点：

- `L_toolList`：当前流程工具列表。
- 工具界面打开分支：搜索 `case ToolType.xxx`，约在 `Job.cs` 中部，负责打开对应 `Frm_xxxTool`。
- 流程模块右键必须以本次 `HitTest` 命中节点为准：顶层模块提供运行、禁用/启用和删除；删除模块或输出端口时要同步清理 `L_toolList`、`D_itemAndSource`、本流程及其他流程中的下游 `ToolIO.value`，不得留下指向已删模块的字符串源。
- 模块双击入口为 `TVW_DoubleClick()`/`GetModuleNodeAt()`：双击端口行也解析到所属模块，不再依赖可能滞后的 `SelectedNode` 或只接受文字区域。入口用 `moduleEditorOpenBusy` 防快速连点/消息泵重入，并必须在 `finally` 中恢复 `Job.loadForm`，避免某个窗体打开分支异常后把后续编辑一直锁住。
- `Frm_Job.ModernEditor.cs` 的首次激活不能依赖隐藏页签的 `TabControl.RowCount` 或等待用户切换流程。第一个 `TabPage` 加入及流程窗体首次显示时必须调用 `ActivateCurrentWorkflowEditor()`，同步下拉选择、启用展开/折叠/删除/属性按钮，并刷新当前连线层。窗体 `VisibleChanged` 只做一次激活，不能再紧接着 `BeginInvoke` 第二次完整刷新；流程下拉内容未变化时只同步选中索引，不能 `Items.Clear()` 后重建。
- 连线的输入端和输出端分别按所属模块的展开状态投影：展开的一端连到具体输入/输出端口，折叠的一端才收口到模块标题。只展开一端时不得把已展开端也降级为模块；只有两端都折叠时，同一有向模块对才归并为一条概要线。
- “显示全部连线”关闭时进入端口聚焦模式：选择顶层模块标题或空白处不画线；只有选择已展开模块中的具体输入端口或输出端口，才显示直接连接到该端口的来源线或去向线。不要再按整个模块汇总显示相关连线。
- 工具运行主循环：搜索 `Run(` 或具体 `else if (L_toolList[i].toolType == ToolType.xxx)`，各工具运行和上下游数据读写都集中在这里。
- 通讯类工具启动初始化时跳过：搜索 `initRun`。

运行流程的触发入口和线程模型（重要）：

- `Job.Run(bool initRun)` 是单次跑完整条流程的核心函数，内部会逐个调用各工具的 `Run()`，其中采集图像、PLC、TCP 等工具会做阻塞式 IO（相机采图、读寄存器、socket 收发）。
- `Job.Run()` 是按“后台工作线程”设计的：函数开头用 `BeginInvoke(...)` 非阻塞投递状态更新，结束状态通过 `PostRunResultToUi()` 回到 UI 线程；公共 `ToolBase.ShowImage()` / `ShowObj()` 入口也必须只在目标图像窗体的 UI 线程读取 Dock/控件状态并显示。工具内部仍有大量直接 HALCON 算子调用，不能据此宣称所有历史工具都已线程安全，新增或修改工具时要继续审查。
- 统一入口在 `Job.cs`：`Job.RunAsync(jobName)` 异步运行指定流程；`Job.RunAndWait(jobName)` 把实际运行放到后台线程，等待期间继续处理 UI 消息，适合“运行后立刻读取输出结果”的标定/取点按钮。不要在 UI 事件里再直接调用 `Job.FindJobByName(...).Run()`。
- 主界面“运行一次”、图像窗口右键“运行流程”、采集设备页和各工具窗体的“运行流程”入口都应走 `RunAndWait()` 或 `RunAsync()`，避免相机、PLC、TCP 等阻塞 IO 卡死 WinForms 消息循环。
- `Job.LoopRun()` 的连续运行线程、`Task_SmartLineA` 自动流程、`OneKeyEyeHandCalibTool` 内部自动标定链仍保留直接 `job.Run()`，因为它们本身在业务工作线程里顺序执行，并依赖同步结果。连续运行每轮不得强制 `GC.Collect()`；`Run()` 返回 null 表示内部未恢复异常，循环必须停止并指出错误日志，不能继续形成异常风暴。排查卡死时先区分入口是在 UI 线程还是已有工作线程。
- 连续运行位于首页或运动页时，工具仍执行并更新流程输出，但 `ToolBase.ShowImage()/ShowObj()` 不向隐藏的 HALCON 图像窗口排队，`PostRunResultToUi()` 也不逐轮追加成功日志；返回视觉页后的下一轮恢复最新图像和叠加显示。否则隐藏绘制队列和上千条输出项会把“首页 -> 视觉”的首次显示拖慢。
- `Job.BeginSingleRun()`/`EndSingleRun()` 维护单次运行忙碌标记；`activeRunCount` 覆盖所有直接 `Job.Run()` 入口；`loopRunThread` 跟踪停止连续运行后尚未结束的最后一轮。三者共同组成 `IsExecutionActive`，线程真正退出前不能启动第二次运行或执行工具预览。

新增/修改工具的标准路径：

1. `DataStrct.cs` 添加/确认 `ToolType`。
2. `1 ToolLib/<工具目录>/` 添加或修改 `xxxTool.cs` 和 `Frm_xxxTool.cs`。
3. `Frm_ToolBox.cs` 添加工具箱节点、新建工具分支、输入输出项。
4. `Job.cs` 添加工具界面打开分支和运行分支。
5. `VMPro.csproj` 确认新文件已 `<Compile Include=...>`。

## 5. 主窗体和公共窗体

| 窗体 | 文件 | 作用 |
| --- | --- | --- |
| 主窗体 | `VisionAndMotion/3 FormLib/Frm_Main.cs` | 菜单、布局、dock 面板、项目命令、全局入口。 |
| 公共窗体基类 | `VisionAndMotion/3 FormLib/Frm_FormBase.cs` | 自定义标题栏、拖动/缩放、置顶/最小化/最大化/关闭按钮。 |
| 流程窗口 | `VisionAndMotion/3 FormLib/Frm_Job.cs` | Job 标签页、流程运行按钮。 |
| 工具箱 | `VisionAndMotion/3 FormLib/Frm_ToolBox.cs` | 工具树和拖放/添加工具。 |
| 设备管理 | `VisionAndMotion/3 FormLib/Frm_DeviceManager.cs` | TCP、PLC、串口、扫码枪、光源等设备列表和子页面调度。 |
| 输出日志 | `VisionAndMotion/3 FormLib/Frm_Output.cs` | 程序输出信息窗口。 |
| 设置主窗体 | `VisionAndMotion/3 FormLib/Frm_Setting.cs` | 设置页容器。 |
| 方案管理 | `VisionAndMotion/3 FormLib/SettingPages/Frm_EngineManager.cs` | 新建、删除、切换、导入导出方案。 |
| 通用设置 | `VisionAndMotion/3 FormLib/SettingPages/Frm_GeneralSettings.cs` | 常规配置。 |
| 项目设置 | `VisionAndMotion/3 FormLib/SettingPages/Frm_ProjetSettings.cs` | 项目信息配置。 |
| 运行设置 | `VisionAndMotion/3 FormLib/SettingPages/Frm_RunSettings.cs` | 运行参数配置。 |
| 启动设置 | `VisionAndMotion/3 FormLib/SettingPages/Frm_StartSettings.cs` | 启动行为配置。 |
| 用户管理 | `VisionAndMotion/3 FormLib/SettingPages/Frm_UserManager.cs` | 用户权限相关配置。 |

### 5.1 浅蓝与暖光白主界面（2026-09-09 当前）

- `ModernUiTheme.cs` 统一暖光白页面、浅蓝强调色、微软雅黑字体、控件前景/背景与 ToolStrip 渲染；`VM.Init()` 安装全局主题，`Frm_FormBase.OnLoad()` 为后续打开的公共窗体补应用。文本操作按钮、输入控件与内容卡片适度圆角；窗口控制区、HALCON 图像控件和数据密集型网格不做无差别圆角。HALCON 控件不做全局字体替换。
- `Frm_Main.ModernLayout.cs` 将主壳层固定为“蓝色标题带 → 全局命令/标准菜单合并栏 → 工作区内容 → 状态栏”的桌面软件结构，窗口最小尺寸为 `1024 × 640`，最大化范围使用当前屏幕工作区。
- 顶层菜单归并为 6 个不重叠入口：项目、流程、视觉、设备、系统、帮助，并嵌入启动/暂停/停止/复位右侧的同一条 50 px 主命令栏，不再单独占一行；菜单对象仍是原 `MenuStrip/ToolStripMenuItem`，因此快捷键、代理命令、权限和展开同步链不变。全局命令栏固定为启动/暂停/停止/复位 4 个机器命令和首页/视觉/运动 3 个工作区导航，共 7 个主命令；退出位于“项目”，锁定/登录/选项位于“系统”，保留原事件和权限判断。“系统 → 选项”仍代理原 `toolStripButton10`，因此管理员权限检查不会被绕过。标题栏隐藏与“帮助”重复的旧更多按钮，只保留最小化、最大化/还原和关闭。
- 三个工作区由 `ApplyWorkspaceMode()` 统一切换：主页（只读生产概览、当前方案/流程数/运行时长和常用入口）、视觉（DockPanel 编辑区）、运动（运动控制）。主页和运动页首次进入时才创建，离开后隐藏并保留状态；视觉快捷栏仅在视觉工作区显示。
- “首页 -> 视觉”的实际入口链为 `toolStripButton5_Click()`/首页按钮 -> `Machine.SwitchFrom(VisionForm)` -> `Frm_Main.ApplyWorkspaceMode()` -> `dockPanel.Visible = true` -> 当前 DockContent 的 `VisibleChanged`。这条 UI 线程链只允许布局、当前流程命令状态和一次重绘；连续运行产生的图像/日志不能在这里补处理历史积压，工作区图标也应复用缓存位图，不能每次切换重新查询 DPI 和重复赋值。
- 工具箱由 `Frm_ToolBox.ModernCategories.cs` 仅重排既有节点，不改变叶节点文字、图标或创建分支，固定为 7 类：图像输入与预处理、检测与识别、标定与定位、几何与 ROI、逻辑与计算、设备与通信、输出与显示；空的历史“3D”根节点移除。
- `Frm_Job.ModernEditor.cs` 不再把所有流程页签按两列、多行长期铺在流程树上方；旧 `tbc_jobs` 继续承载流程内容并保留创建、删除、运行和 `Job.GetJobTree()` 兼容链，但页签头压缩隐藏，顶部改为固定 44 px 的“当前流程”下拉切换区。下拉会同步流程新增、删除、重命名和外部页签切换，流程数量增加不再挤占编辑树高度。编辑器底部不再展示“单次运行/连续运行”，命令区由 98 px 压缩为 54 px，保留五个流程管理命令并新增一个“显示全部连线”切换；关闭该切换时，无选择或选择模块标题都不画线，只有选择已展开模块内的具体输入/输出端口才显示该端口的直接连线。连线两端分别按自己所属模块的展开状态投影：展开端连到具体端口，折叠端连到模块标题；两端都折叠时，同一对模块的多个端口连接只画一条关系线。两个旧运行按钮对象仍隐藏保留，供历史状态链写入 `Enabled/Text`，实际运行继续从视觉快捷栏、菜单或 F5/F6 进入。流程模块拖拽由 `Job.TryMoveToolNode()` 在移除节点前统一计算并校正插入位置，同时同步流程树与 `L_toolList`；拖到首项、子端口或空白末尾均不得产生负索引或半更新状态。`FlowEditorTreeView` 不再在原生 TreeView HWND 上用 `Graphics.FromHwnd` 补画连线，连线改由不接管鼠标的独立双缓冲层绘制，原生 HotTracking 关闭，避免鼠标移动局部擦除造成闪烁或拖拽期间的窗口绘制竞争；端口圆点中心固定在覆盖层左侧并用短线接入连线区，不能让覆盖层遮掉圆点右半边。`Frm_ToolBox.ModernUi.cs` 采用“紧凑搜索/命令区 + 工具树”两行布局：搜索输入逻辑高度 26 px，底部工具说明卡片及其选择更新事件移除，释放的空间全部交给工具树；工具搜索、清空、上下键/回车和分类展开折叠语义不变。
- 旧 `.pjt/.eng/.job` 流程加载统一走 `Job.PrepareLoadedWorkflowData()`、`AttachLoadedWorkflow()` 和 `RebuildLoadedWorkflowTree()`：先清除序列化残留的连续运行、单次忙碌、停止请求、双击次数、旧 TreeNode 连线及工具锁，再初始化空工具/IO集合；编辑树必须先创建全部模块和输入输出端口，第二遍才按名称恢复本流程连线，因此来源模块排在目标模块之后也可恢复。跨流程与全局变量来源保留给运行时解析，不画成当前流程内连线；格式错误或已不存在的本地来源只在对应端口显示提示，不得让整个流程页加载中断。`LoadJob(string)`、`LoadJob(Job)`、`OpenJob(Job)`、`InportJob(Job)` 不得各自复制恢复代码，也不得重新注册旧 `BeforeExpand/BeforeCollapse + m_MouseClicks` 双击拦截。`D_itemAndSource` 是纯 UI 运行期索引，不再序列化；旧项目的非空工具对象、模板、ROI、标定和参数不得为“兼容”而替换。
- 设置导航固定为 6 个顶层分类：常规、项目、方案、启动、运行、用户与安全；历史“功能”改名为“启动”，“用户管理/安全”归并到“用户与安全”。
- “项目”菜单覆盖常用方案与项目生命周期：新建/打开/最近/克隆方案、导出方案、保存项目、导入/导出项目及退出。不要恢复顶层“删除当前方案”代理：历史 `toolStripButton18` 路径只直接修改模型集合，缺少完整 UI 刷新和持久化保障；删除方案统一走“系统 → 选项 → 方案管理”（`Frm_EngineManager`）的受控确认、刷新和保存路径。
- 工厂默认布局是 `Start/Config/Resources/Layout/经典布局1.config`：右侧 30% 由流程和工具箱共用标签 Pane，并以全高度贯穿工作区；中央列上方承载图像文档，输出和监控共用的底部 22% 标签 Pane 只延伸到右侧编辑列左边，不再占用流程编辑器下方空间。视觉快捷栏只直显单次运行、连续运行、保存项目、读取图像 4 个动作，另保留单一“批量运行”下拉；上一张本地图像、暂停目录图自动切换保留在“视觉 → 图像”，极速模式保留在“视觉 → 辅助工具”。全局变量保留在“视觉”，并通过原 `toolStripButton34` 代理执行以保留刷新行为。方案、流程、图像管理、布局、设备及重复/空实现入口不再与主菜单平级占用顶栏。经典模板只读；用户调整需要保存时自动转存 `dockPanel.config`，避免升级覆盖工厂模板。只有可明确识别的仓库旧演示快照会在内存中迁移到新版专注布局，任意其他自定义布局不改写；手动入口为“视觉 → 布局 → 专注布局（标准）”，重启后生效。旧项目的多图像窗口与复杂自定义 Dock 布局仍需真实项目回归。
- `CreateProxyMenuItem()` 为迁移到六类菜单的原按钮统一生成文字入口，`Tag` 保存源 `ToolStripItem`，点击继续调用源项 `PerformClick()`；代理菜单不复制旧低分辨率 `Image`，避免高 DPI 模糊，也不能改成直接调用业务函数而绕过原权限、确认或刷新路径。主菜单展开时递归同步源项的 `Enabled`、`Available` 和 `CheckOnClick/Checked` 状态；设计器原有 F5/F6 仍落到“单次运行当前流程/连续运行当前流程”，并分别绑定 `toolStripButton11/12` 的可用状态，不能让快捷键从禁用菜单绕过机器运行门槛。
- 当前显示版本由 `Configuration.ProductVersion` 定义（2026-09-10 源码为 `1.5.2`），更新日期由 `ProductUpdateTime` 定义；标题格式由 `BuildApplicationTitle()` 统一生成。`Start/Properties/AssemblyInfo.cs` 为 `1.5.2.0`/`1.5.2`，而 `VisionAndMotion/Properties/AssemblyInfo.cs` 仍为 `1.0.0.0`/`1.0.0`，发布时需独立核对，不能把标题版本当作两个程序集版本已统一的证据。`Frm_Welcome.cs` 使用独立的暖白启动画面、浅蓝矢量视觉图、柔和进度条，显示配置中的产品版本，不再读取日期式 `AssemblyConfiguration`；内部 WLP 品牌标记由 `WelcomeBrandMark` 抗锯齿绘制，退出按钮使用透明过渡圆角底图，加载进度仍由原 `bar_step/lbl_step` 驱动，视觉刷新计时器仅在窗口可见时运行。欢迎页作为无边框顶层 `Form` 仍保留 `Region` 外形裁切，这个边界不能用内部控件的抗锯齿结果替代。
- `ControlLib/Controls/ModernInputControl.cs` 是输入底板基类；`CTextBox`、`CComboBox`、`CNumeric`、`CNumericUpDown` 四类共享输入控件统一继承它。底板用 GDI+ `AntiAlias`、半像素内缩和暖白/浅蓝调色绘制，不再给 24–26 px 小输入控件使用二值 `Region` 裁切；下拉箭头、密码可见性、数值加减等符号用代码绘制。`CTextBox.TextStr` 程序赋值时同时更新内部值、编辑器文本和占位状态，一次实际变化最多发出一次 `TextStrChanged`；`CNumeric` 把空文本、单独负号/小数点等视为编辑中间态，只有完整数字才提交 `ValueChanged`，失焦时回到最后有效值，避免 `Convert.ToDouble` 异常和重复通知。`CNumericUpDown` 小宽度布局分档：50 px 只显示可编辑数值（键盘/滚轮仍可步进），70 px 采用纵向加减按钮，90 px 及以上再使用横向按钮，负数和两位小数不得被遮挡。
- `ModernUiTheme.StyleRoundedButton()` 与卡片主题使用带透明过渡像素的 PArgb 背景图绘制圆角，并主动清除普通按钮/卡片旧 `Region`；已有超大功能位图会以高质量插值缩小并按前景色着色，不能把业务图标当成按钮皮肤覆盖。无边框顶层 `Form`（包括欢迎页外轮廓）仍有 WinForms `Region` 路径，这一层的边缘平滑度必须在目标系统和 DPI 下另验，不能由内部控件预览代替。
- `ModernVectorIconFactory.cs` 为主命令栏、视觉快捷栏生成代码矢量图形的精确像素位图，按控件当前 `DpiX` 选择尺寸并缓存；笔画使用圆角线帽/连接及抗锯齿，ToolStrip 关闭二次图像缩放。`Start/WLPVM.ico` 提供 16/20/24/32/40/48/64/128/256 共 9 个尺寸，`Start.csproj` 同时用作 `ApplicationIcon` 和内容资源；`Start/app.manifest` 与 `Start/Properties/app.manifest` 的程序集标识统一为 `WLPVM.app`，欢迎页/主窗体从当前宿主可执行文件提取同一图标。启动清单目前未声明全局或 Per-Monitor DPI 感知，Windows 对整个旧式 WinForms 进程的虚拟化仍可能再次缩放，启用清单前必须回归全部历史绝对布局。
- 工具配置窗体继续通过 `Frm_FormBase` 继承统一主题。`VisionAndMotion/1 ToolLib/37 SaveImageTool/Frm_SaveImageTool.cs` 是模块内重排样板：来源、存储规则、文件管理三张卡片复用原参数控件与运行事件；旧图片式勾选/按钮皮肤隐藏。清空保存位置属于不可撤销操作，必须做受保护路径检查、待写入检查和二次确认。

### 5.2 UI 资源调度边界

- 主 UI 计时器基准为 100 ms，但时钟、运行时长及通用状态更新限频到 1 s；运动 IO 仅在运动工作区可见、窗口未最小化且机器不处于运行状态时按 300 ms 更新，避免所有工作区持续轮询。
- `Frm_MotionControl` 的刷新线程改为首次需要时才创建；仅在运动界面有效可见时活跃，隐藏后降频，并将控件快照/界面回写放回 UI 线程。设备管理、运动控制与点表编辑窗体不再由 `Machine.InitAll()` 无条件实例化；点表视图在进入对应模块时从当前项目刷新。
- `Frm_Output` 将日志模型更新与 WinForms 控件更新分离：后台调用仅更新受锁保护的模型并入有界队列，UI Timer 每 100 ms 最多提交 48 条、一次滚动；筛选切换、清空和队列积压使用完整快照重建，窗口隐藏时暂停绘制。报警历史使用唯一 `DateTime` 键并按最早时间淘汰，避免同一时刻突发日志丢失或计数漂移。
- 存储图像模块的后台写图队列是跨流程实例共享的线程安全单工作线程；自动清理只在启用且保留天数大于 0 时限频调度，不应每保存一张图就新建清理线程。写图、过期目录清理和手动清空共享文件操作边界，避免目录删除与写文件并发。窗口截图的 `HObject` 所有权会移交给队列并在写入后释放，输入图像仍是上游借用对象；队列上限为 64 项，满载时会对调用方背压以保证不丢请求和内存有界，因此不能宣称保存入口始终非阻塞。
- `Machine.UpdateAll()` 只负责运行状态更新；已移除主刷新路径里的同步 UI `Invoke`、全局资源锁和人为 `Sleep`。`HWndCtrl` 的背景图、叠加对象、图形上下文与释放路径由同一窗口锁保护；涉及 HALCON 进程级 `flush_graphic` 时固定使用 `GraphicRenderLock → objectStackLock`，禁止反向取锁。
- `Machine.InitAll()` 的板卡、通讯和外设初始化仍在后台线程；欢迎页、主窗体、项目 UI 重建、状态栏/菜单和板卡初始化失败提示通过主窗体句柄派发到 UI 线程。`VM.Init()` 先在 UI 线程读取最终配置，再构造欢迎页/主窗体及其句柄，最后启动初始化线程；`Machine.InitAll()` 不重复读配置。`Configuration.Read(false)` 只更新配置模型字段，主窗体构造也不得全局关闭 WinForms 跨线程检查。
- 流程树 `Job.TVW_AfterSelect()` 必须在控件所属线程执行完整工具预览分支。单击/双击区分使用每个 `Job` 持有的 WinForms UI Timer 延迟 500 ms；新选择以请求代次取消旧回调，双击显式取消待执行 Timer。禁止恢复 `new Thread + Sleep` 后直接读取 `TreeView.SelectedNode` 的旧写法，也不能只对节点读取局部 `Invoke`，因为后续各 `Frm_*`、DataGridView 和 HALCON 窗口同样属于 UI 对象。
- 这次调度优化不等于硬件 IO 已完全异步化：可见运动页的 `Machine.UpdateIO()` 仍可能等待驱动/资源锁，真实设备的最坏延迟必须现场测量。
- 没有启动完整程序，因为 `VM.Init()` 会进入 `Machine.InitAll()` 并自动初始化/连接设备；当前只允许做无硬件构造、Dock XML 加载和 `DrawToBitmap` 壳层烟测。没有真实相机、PLC、运动卡或现场 DPI/多屏验证。
- 2026-09-08 最终验证：UI 基线的 `Controls.csproj /t:Rebuild` 为 0 警告/0 错误、`VMPro.csproj /t:Rebuild` 为 961 警告/0 错误；点击视觉线程修复后的 `VMPro.csproj /t:Build` 为 539 警告/0 错误，`VM Pro.sln /t:Build /p:Configuration=Debug /p:Platform="Mixed Platforms"` 为 4 警告/0 错误。隔离 `UiShellSmoke` 退出码 0，通过 1088 项断言；新增严格跨线程检查下两个空流程节点快速选择、首页/视觉切换及 900 ms 延迟窗口回归，不连接相机、PLC 或运动卡。WLP EXE 嵌入图标与源 ICO 匹配 100%，欢迎页品牌块浅蓝绘制覆盖 80.13%。最终 `CVMPro.dll` 为 8,532,480 bytes，SHA-256 `11A828BF847E4144525D169F37DC1938648DF1C811706BCD40A635E11325A27A`；`WLP VM.exe` 为 31,232 bytes，SHA-256 `AE425B788A48B8746CEB6A19422EE7CF87E99DBCFCF27CA2DFD97CF2E0BC642B`。直接构建 `Start.csproj /p:Platform=x86` 会因 `VMPro.csproj` 没有该项目级平台输出映射而失败；使用解决方案 `Debug|Mixed Platforms` 映射。2026-09-07 的旧 DLL 哈希不再作为当前证据。
- 2026-09-09 流程/工具箱紧凑布局验证：解决方案 `Debug|Mixed Platforms` 构建为 0 错误；`FlowEditorSmoke` 通过 39 项断言，除流程下拉、隐藏运行入口、拖拽同步和独立连线层外，新增覆盖连接点不被覆盖层遮挡、显示全部、选中首/中间模块聚焦及无选择隐藏。`ToolboxUiSmoke` 通过 13 项断言。Dock 模板与主窗体加载阶段确认右侧流程/工具箱贯穿到底、底部日志在右侧编辑列之前截止；主壳层阶段确认六类菜单嵌入机器命令同一行且不与左右命令重叠，视觉区因此增加 30 px 高度。版本资源已验证：`WLP VM.exe` 与 `CVMPro.dll` 均为 FileVersion `1.0.0.0`、ProductVersion `1.0.0`；主标题和欢迎页版本阶段通过。完整 `UiShellSmoke` 随后仍停在与本轮无关的“存储图像窗体圆角外形”断言，因此不把新增总断言数标为完整通过。预览位于 `Tests/artifacts/compact-workflow-toolbox/`。没有启动会进入 `Machine.InitAll()` 的完整程序，也没有连接相机、PLC 或运动卡。

设备管理 UI 调度：

- PLC、TCP 客户端/服务端、串口和扫码器的运行对象不得通过 `Frm_*.Instance` 隐式创建配置页；只向已存在、未释放且仍绑定当前设备的页面派发状态，页面重新显示时从模型恢复状态。隐藏页面丢弃显示型日志，避免后台通信持续堆积 UI 工作。
- TCP/PLC 的连接中状态是非序列化运行时字段；切换 A/B 设备后，A 的迟到完成不能覆盖 B 的页面。TCP 客户端共享 Socket 表必须通过专用锁交换/查询，网络调用不得在表锁内执行；手动断开必须阻止接收线程立即自动重连。

- `Frm_DeviceManager.ShowSelectedDevice()`：根据设备类型显示子页面。
- `Frm_DeviceManager.ShowChildForm()`：把子窗体嵌入右侧区域。
- PLC 页面：`Frm_PLCComm.Instance.LoadPar(PLCDevice)`。
- TCP 服务端：`Frm_TCPServer.Instance.LoadPar(TCPSever)`。
- TCP 客户端：`Frm_TCPClient.Instance.LoadPar(TCPClient)`。
- 光源：`Frm_LightController.Instance.LoadPar(LightController_Base)`。
- 扫码枪：`Frm_Scaner.Instance.LoadPar(Scaner)`。
- 串口：`Frm_Serial.Instance.LoadPar(Serial)`。

窗体标题栏注意：

- 继承 `Frm_FormBase` 的窗口标题栏按钮由 `Frm_FormBase.AlignTitleButtons()` 在运行时统一右对齐，顺序为 `置顶`、`最小化`、`最大化/还原`、`关闭`。四个按钮的最小点击宽度为 34 px，图形由 `ModernVectorIconFactory` 按 DPI 重绘，关闭使用独立红色悬停态；窗口状态或置顶状态改变后调用 `RefreshTitleButtonVisuals()`，不要在子窗体重新覆盖旧位图。
- 不要优先在各子窗体 Designer 里手调 `button100.Location`；旧 Designer 里可能有历史坐标，但运行时应由基类统一覆盖。
- 继承 `Frm_FormBase` 的模态弹窗统一通过 `Frm_FormBase.ShowDialog()` 创建临时顶层 owner，保证提示、错误、确认、输入类窗口不被其他置顶窗口盖住。

## 6. 设备和通讯

| 功能 | 数据类 | 配置窗体 | 流程工具 |
| --- | --- | --- | --- |
| PLC | `VisionAndMotion/2 ClassLib/PLCDevice.cs`、`CipCommunication.cs`、旧 `PLCComm.cs` | `VisionAndMotion/3 FormLib/Frm_PLCComm.cs` | `VisionAndMotion/1 ToolLib/49 PLCCommTool/PLCCommTool.cs`、`Frm_PLCCommTool.cs` |
| TCP 服务端 | `VisionAndMotion/2 ClassLib/TCPSever.cs` | `VisionAndMotion/3 FormLib/Frm_TCPServer.cs` | `VisionAndMotion/1 ToolLib/47 EthernetReceiveTool/` |
| TCP 客户端 | `VisionAndMotion/2 ClassLib/TCPClient.cs` | `VisionAndMotion/3 FormLib/Frm_TCPClient.cs` | `VisionAndMotion/1 ToolLib/48 EthernetSendTool/` |
| 串口 | `VisionAndMotion/2 ClassLib/Serial.cs` | `VisionAndMotion/3 FormLib/Frm_Serial.cs` | 主要被扫码/设备工具引用。 |
| 扫码枪 | `VisionAndMotion/2 ClassLib/Scaner.cs` | `VisionAndMotion/3 FormLib/Frm_Scaner.cs` | `VisionAndMotion/1 ToolLib/19 KenyenceScanerTool/` |
| 光源 | `LightController/LightController_Base.cs`、`LightController_CST.cs` | `VisionAndMotion/3 FormLib/Frm_LightController.cs` | `VisionAndMotion/1 ToolLib/27 OPTLightTool/`、`33 OptLightControlTool/` |

PLC 当前重点：

- 设备列表：`Project.Instance.L_PLCDevice`。
- 连接对象缓存：`PLCDevice.L_cipComm`，按设备名保存 `CipCommunication`。
- 连接/断开：`PLCDevice.Connect()`、`Disconnect()`、`Close()`。
- 项目加载后运行时修复：`Project.EnsureCommunicationRuntime()` 调用 `PLCDevice.ResetRuntimeStore()` 和 `PLCDevice.EnsureRuntime()`。
- PLC 流程工具绑定设备名：`PLCCommTool.PLCDeviceName`。
- PLC 工具读写：`PLCCommTool.Run()`、`ReadFromPlc()`、`WriteToPlc()`。
- PLC 工具界面：`Frm_PLCCommTool.LoadToolData()`、`LoadDeviceList()`。

## 7. 图像和视觉工具

图像窗口/ROI：

- `ImageWindow/HWindow_Final.cs`：图像显示控件主实现。
- `ImageWindow/Model/ROI*.cs`：ROI 形状和控制。
- `ImageWindow/Config/SerializeHelper.cs`：图形/ROI 序列化辅助。
- `VisionAndMotion/3 FormLib/Frm_ImageWindow.cs`：主程序中的图像窗口页面。

常见图像工具：

| 工具 | 目录 | 运行类 |
| --- | --- | --- |
| 采集图像 | `VisionAndMotion/1 ToolLib/01 ImageAcqTool/` | `AcqImageTool.cs`，SDK 文件含 `SDK_Halcon.cs`、`SDK_Basler.cs`、`SDK_HIKVision.cs`、`SDK_MindVision.cs`、`SDK_PointGrey.cs`。 |
| 预处理 | `VisionAndMotion/1 ToolLib/42 ImageProprecessingTool/` | `ImageProprecessingTool.cs`，附 `Binary.cs`、`FillUp.cs`。 |
| 模板匹配 | `VisionAndMotion/1 ToolLib/03 ShapeMatchTool/` | `ShapeMatchTool.cs`。 |
| 查找边 | `VisionAndMotion/1 ToolLib/09 FindLineTool/` | `FindLineTool.cs`。 |
| 查找圆 | `VisionAndMotion/1 ToolLib/10 FindCircleTool/` | `FindCircleTool.cs`。 |
| 斑点分析 | `VisionAndMotion/1 ToolLib/11 BlobAnalyseTool/` | `BlobAnalyseTool.cs`。 |
| OCR | `VisionAndMotion/1 ToolLib/21 OCRTool/` | `OCRTool.cs`。 |
| 条码/二维码 | `VisionAndMotion/1 ToolLib/22 BarcodeTool/`、`21 QRCodeTool/` | `BarcodeTool.cs`、`QRCodeTool.cs`。 |
| 区域特征/区域操作 | `VisionAndMotion/1 ToolLib/20 RegionFeatureTool/`、`22 RegionOperationTool/` | 对应 `Tool.cs`。 |

采集图像工具注意：

- `Frm_FromDevice` 属于采集图像工具窗口，实时按钮必须使用 `Frm_AcqImageTool.CurrentInstance`，不能通过 `Frm_AcqImageTool.Instance` 隐式创建或操作错误窗体；主菜单的“采集设备”窗口必须使用 `Frm_FromDevice1/Frm_FromLocal1` 和 `Frm_AcqDevice` 自己的图像窗口，避免两个入口共用子窗体导致样图、实时状态和关闭行为串线。

- 设备列表和曝光亮度 UI 在 `VisionAndMotion/1 ToolLib/01 ImageAcqTool/Frm_FromDevice.cs`、`Frm_FromDevice.designer.cs`；设备枚举由各 `SDK_*.cs` 调用 `Frm_FromDevice.Instance.cbx_deviceList.Add(...)` 填充。
- `AcqImageTool.ResetTool()` 是采集工具“重置”按钮最终走到的核心复位函数。当前逻辑会切回 `ImageSourceMode.FromDevice`，再调用 `RefreshDeviceList()` 重新检索相机，然后清空本地图像路径、清窗口、恢复自动切图/转灰度/显示整图等 UI 状态。
- `AcqImageTool.RefreshDeviceList()` 专门处理“程序启动后再接入相机”的设备列表刷新：如果正在实时采集，先通过 `PlayImage(false, Frm_AcqImageTool.Instance.hWindow_Final1)` 停止实时；清空 `SDK_Camera`、`Frm_FromDevice.Instance.cbx_deviceList`、`Frm_FromDevice1.Instance.cbx_deviceList` 和 `AcqImageTool.L_devices`；调用各 `SDK_*.CloseAllCamera()` 释放旧句柄；最后重新调用 `SDK_Halcon.EnumCamera()`、`SDK_HIKVision.EnumCamera()` 和 `SDK_PointGrey.EnumCamera()` 重新枚举。不要只恢复启动枚举而漏掉这里，否则“重置/重新检索设备”后海康相机可能仍然不回到设备列表。
- `SDK_HIKVision.EnumCamera()` 是海康原生 SDK 的静态枚举入口，内部兼容旧的实例方法 `EnumCamrea()`。刷新设备列表时必须通过这个静态入口调用，避免不同枚举入口分叉。
- `SDK_Halcon.EnsureAcquisitionInterfacePath()` 负责兼容不同 MVS 版本的 Halcon/MVision 适配器路径。候选根目录顺序包括 `MVS_HALCON_ADAPTER_DIR`、程序运行目录、`MVS_HOME`/`MVS_ROOT`、注册表卸载项里的 `MVS` 安装路径，以及常见 `Program Files` 路径；像 `E:\Work-SoftWare\MVS` 这种非默认安装目录要靠注册表发现。多版本并存时不要硬编码单一 MVS 版本或单一路径，优先让探测逻辑逐个尝试含 `hAcqMVision.dll` 的适配器目录。
- `Frm_FromDevice` 支持手动 IP 添加相机。输入相机 IP 后会按 `MVision | <ip>` 创建 `SDK_Halcon` 设备、加入下拉框、切换当前设备，并立即调用 `SDK_Camera.TryOpenCamera()` 验证是否真正打开；不能只把 IP 加到列表里，否则“相机实时”会出现列表有设备但没有句柄可用的问题。手动 IP 控件放在设备列表和曝光控件之间，调整布局时要避免压住曝光输入、滑块、实时按钮和保存目录区域。
- 手动 IP 必须严格校验为四段 IPv4，例如 `192.168.1.11`；不要只用 `IPAddress.TryParse()`，因为 .NET 会接受 `192.168.11` 这类缩写地址，容易把用户输入解析成非预期 IP。
- 手动 IP 添加时优先调用 `SDK_HIKVision.TryCreateManualIpCamera()`：通过海康 SDK 枚举结果按 IP 精确匹配设备，匹配到就用海康原生 `CreateDevice/OpenDevice/StartGrabbing` 建立句柄；只有海康枚举不到该 IP 时才回退到 `SDK_Halcon.BuildManualIpCameraInfo()` 的 `MVision | <ip>`。如果 MVS 能看到并使用相机，而程序手动 IP 打不开，优先确认 MVS 已关闭该相机连接，否则海康控制权会被 MVS 占用并返回访问权限错误。
- 手动 IP 成功打开后会写入 `AcqImageTool.knownManualIpAddresses` 并随项目保存；`RefreshDeviceList()` 常规 SDK/Halcon 枚举完成后会调用 `AddKnownManualIpCameras()`，自动按这些 IP 再尝试检索并加入设备列表。刷新时不要清掉该列表，也不要在打开失败前记住新 IP，避免错误 IP 被长期保存。
- `SDK_Base.TryOpenCamera()` 是手动 IP 和实时采集前的统一打开验证入口。`SDK_Halcon.TryOpenCamera()` 会复用当前 `CameraInfoStr` 调 `OpenFramegrabber()` 建立句柄；`AcqImageTool.PlayImage()` 启动实时前必须先调用该入口，实时线程中首帧失败时要恢复按钮/菜单/下拉框状态并输出明确错误。
- `Job.Run()` 本身会在流程中遇到 `ToolType.ImageAcq` 时执行 `AcqImageTool.Run(false, false, toolName)`；不要在各个“运行流程”按钮里额外手写采集逻辑。为避免“先运行采集工具正常、直接运行流程卡死”，`AcqImageTool.Run()` 在流程模式也必须先用 `SDK_Camera.TryOpenCamera()` 打开/预热相机，并且对打开、曝光、`GrabOneImage()` 都使用 `AcquisitionTimeoutMs` 超时保护。不能把采图改回裸调用，否则相机 SDK 首帧或断线阻塞会卡住整条流程。
- `AcqImageTool.lastPreviewImagePath` 仅用于本次会话中记录预览来源；项目反序列化时会清空该路径和 `ResultPar.图像`。`EnsurePreviewImage()` 只保留本次启动后新采集/新选择的内存图，不再自动从磁盘恢复上次预览；没有新输入时，主图像窗口、采集窗口、模板匹配窗口和模板预览小窗口均应保持黑屏。
- `MatchTool.OnDeserialized()` 只清空上次运行的 `InputPar.图像` 和派生预处理图，保留已学习模型、模板区域和搜索区参数；这样重启后四个图像区域为黑屏，同时不丢失模板配置。
- 旧示例流程或序列化工程可能残留 `Config\Resources\Image` 内置样图路径或结果图。`AcqImageTool.ClearInternalSamplePreview()` 是统一过滤口：打开采集窗体、点击流程树采集节点、模板匹配从采集工具刷新输入前都要先调用；`SaveLastPreviewImage()` 和 `TryLoadLastPreviewImage()` 也必须跳过内置样图路径，避免样图被写成运行预览缓存。
- 设备采集模式 `ImageSourceMode.FromDevice` 中，本次会话已采集的图像可在工具窗口之间保留；程序重启或重新加载项目后必须清空，不能将上次运行的最后帧当作当前输入。
- 海康 `MvCameraControl.Net.dll` 在不同 MVS 版本间可能强签名状态和程序集版本都不同，例如 2.4 是无强签名而 3.x/4.x 可能是强签名，不能指望 .NET bindingRedirect 自动互换。跨版本兼容优先走 Halcon/MVision 适配器探测；原生 HIK SDK 枚举失败应被捕获，不能阻断整体设备刷新。
- 各 `SDK_*.CloseAllCamera()` 关闭硬件句柄后必须清理自己的静态相机字典，例如 `SDK_Halcon.D_cameras.Clear()`、`SDK_PointGrey.D_cameras.Clear()` 等。否则重置后旧 key 还留在缓存里，`EnumCamera()` 会认为设备已存在，导致新接入或重连的相机不能重新加入 UI 列表。
- `AcqImageTool.SwitchDevice(string cameraInfoStr)` 根据下拉框文本从 `AcqImageTool.L_devices` 中选择当前 `SDK_Camera`，并调用 `ApplyExposureBrightness()` 把当前曝光下发到新相机。设备列表刷新后不要直接手改 `SDK_Camera`，优先让下拉框选择触发这条链路。
- 曝光亮度最终仍保存到 `AcqImageTool.exposure`，即时下发入口为 `AcqImageTool.ApplyExposureBrightness()`，流程采图时仍由 `Run()` 在采图前调用 `SDK_Camera.SetExposure(exposure)`。
- 曝光亮度 UI 范围为 `0.1ms - 100ms`，滑条按 `Frm_FromDevice.ExposureSliderScale = 10` 映射到 0.1ms 细调。
- 曝光即时下发依赖各 `SDK_*.cs` 同时正确实现 `SetExposure()` 和 `CheckCamExist()`；Halcon/MVision 走 `SDK_Halcon.SetExposure()` 的多参数名尝试。
- 相机实时提示在 `AcqImageTool.PlayImage()` 中刷新图像后绘制，避免 `HobjectToHimage()` 或 `ShowImage()` 把“实时中...”文字擦掉造成闪烁。
- 进入采集图像编辑界面的入口在 `Job.cs` 的 `ToolType.ImageAcq` 分支。打开采集图像窗体时不要强制把 `imageSourceMode` 改回 `FromDevice`；调用 `EnsurePreviewImage()` 只显示本次会话已有的新图像，空时直接清窗。运行工具时仍按当前 `imageSourceMode` 实际采集/读取。
- 采集图像启用 `rotateImage` 后仍由 HALCON `RotateImage()` 处理像素；90°旋转后若圆形 ROI 显示成椭圆，根因通常不是旋转算法，而是显示窗口把图像坐标范围强行铺满了宽高比不同的控件。统一等比显示入口在 `ImageWindow/Model/HWndCtrl.cs` 的 `FitImagePartToWindow()`：`resetWindow()`/`resetAll()` 必须通过它按窗口宽高比扩展 `ImagePart` 并居中留白，不能重新直接设置为 `(0,0,imageWidth,imageHeight)`，否则横竖图切换会再次产生非等比拉伸。
- 自动运行流程时不要用 `Frm_AcqImageTool.Instance` 做可见性判断；它会在未打开编辑窗体时创建窗体实例，把流程运行和编辑窗口初始化绑在一起。需要刷新采集编辑窗体时先通过 `Frm_AcqImageTool.CurrentInstance` 判断已有窗体，并用 `BeginInvoke` 投递到 UI 线程。
- `AcqImageTool.Run()` 是采集输出替换的核心：先更新 `toolPar.ResultPar.图像`，流程再把输出写入下游输入。排查“拍照后卡死/旧图替换异常”时先区分卡在相机 SDK 同步采图，还是卡在 Halcon 窗口刷新；不要把显示窗口刷新当作输出替换的必要步骤。
- 硬触发等待必须有超时保护，避免 `SDK_Camera.waitingHardTriggerImage` 没被回调清掉时无限等待。Halcon 相机的 `GrabImage` 也应尽量设置 `grab_timeout`，保持和 Basler/HIK/MindVision 这类已有超时的 SDK 行为一致。
- 采集图像编辑窗体“运行流程”卡死的根因：`Frm_AcqImageTool.btn_runJob_Click` 旧实现直接在 UI 线程同步调用 `Job.FindJobByName(jobName).Run()`。`Run()` 会走到 `AcqImageTool.Run()` 的阻塞采图（`SDK_Camera.GrabOneImage()` -> `HOperatorSet.GrabImage`），在 UI 线程执行会卡住整个消息循环，界面就“卡死”。程序刚启动时相机首帧还没就绪（采集接口若没真正生效的 `grab_timeout`，首次 `GrabImage` 会一直等），所以“刚启动运行流程一定卡死”；之后偶发丢帧/采图延迟时表现为“偶尔卡死”。当前运行入口已收口到 `Job.RunAndWait()`/`RunAsync()`，运行期间由统一忙碌标记防重入；排查同类卡死时，先确认新增入口有没有绕开统一入口，再看相机 SDK 的 `GrabImage` 有没有超时。
- 这个旧 WinForms/.NET Framework 方案依赖 HALCON 和相机 SDK；不要在 macOS/非 Windows 环境使用 `dotnet build` 作为构建结论，验证应使用 Windows + VS2022 Build Tools MSBuild，硬件行为再由受控真机环境确认。

斑点分析工具注意：

- `BlobAnalyseTool.Run(false, false, ...)` 由流程工作线程执行，不得直接调用 `GetImageWindowControl().hwc_imageWindow`。图像窗口可能尚未创建、正在切换或已关闭，`GetImageWindowControl()` 返回 null 时不能让显示失败中断斑点计算和输出。
- 流程显示统一由 `QueueRuntimeDisplay()` 在工作线程复制 HALCON 对象，再作为一个 UI 回调按“背景 -> 搜索区 -> 结果区 -> 外接圆 -> 中心十字”绘制。第一个成功斑点工具可重绘背景，后续斑点工具只叠加图层；外接圆和十字必须合并后批量投递，不得按斑点数堆积 UI 消息。
- 斑点运行前用 `ToolBase.TryGetHalconImageSize()` 拦截 null 及 object ID 0 图像。结果表只通过 `Frm_BlobAnalyseTool.CurrentInstance` 取已存在窗体并安全投递，后台流程不得因刷新表格而隐式创建斑点编辑窗口。

查找线工具注意：

- 2026-09-09 已完整重制：窗口固定为“左侧大图 + 右侧基本参数/运行参数/结果显示 + 底部预览/运行工具/运行流程”，使用 `ModernUiTheme` 暖白、浅蓝风格。不要恢复旧的绝对坐标参数面板。
- 窗体只通过 `Frm_FindLineTool.BindTool()` 绑定真实工具并回填参数；`Job.cs` 只解析 `图像/跟随`、必要时重定位 ROI，不再直接逐个操作窗体控件。`BindTool()` 不得调用会清空运行时图像的 `EnsureLoadedState()`。
- 参数改变、ROI 移动和鼠标松开统一走 150ms 防抖预览；流程正在运行时只保存参数，不得与流程并发调用 HALCON。边缘极性、边缘选择、阈值、卡尺数/宽度、最低得分和剔除点数都在运行前归一化。
- `ShowContour()` 是预览入口，`Run()` 是正式运行入口，两者共用 `NormalizeParameters()`、`BuildExpectedLines()`、`ApplyMetrologyParams()` 和拟合/剔除规则。正式运行对每个跟随位姿单独捕获测量失败，一个位姿异常不再把整个界面或流程崩掉。
- 临时 `HObject` 和 Metrology handle 必须走 `finally`/`Dispose()` 释放；投递到 UI 线程的图形必须先做副本，防止工作线程释放后 UI 再访问失效 HALCON 对象。
- 对外仍只输出第一条成功的 `线`；多线输出需要同时扩展 `ResultPar`、连线类型和下游工具，不是单纯 UI 功能。

- 核心文件是 `VisionAndMotion/1 ToolLib/09 FindLineTool/FindLineTool.cs`，界面文件是 `VisionAndMotion/1 ToolLib/09 FindLineTool/Frm_FindLineTool.cs`。优先阅读顺序：构造函数默认 ROI -> `ShowContour()` 预览 -> `Run()` 正式运行 -> `SyncDisplayedRoi()` 与窗体鼠标事件。
- 线查找的搜索区域不是普通矩形，而是 `ROIRectangle2` 表示的“可旋转卡尺区域”。`FindLineTool.GetBaseLine()` 直接从该 ROI 的顶点数据中取第 7/9 号端点作为预期线起止点，因此改 ROI 数据结构或端点顺序时会直接影响找线结果。
- `FindLineTool.EnableLineRoiEditing()` 会给 `ROIRectangle2.EndpointRotationEnabled` 赋值，使内部拖动表示整体平移、两端拖动同时改变长度和角度。这是找线专用交互；如果预览恢复成普通旋转矩形，先检查这里是否在显示 ROI 后被重新调用。
- `FindLineTool.ShowContour(bool showROI, bool trans, bool preserveInteractiveRoi)` 是调参预览主入口：它只显示卡尺、边缘点和拟合线，不写 `ResultPar`。`preserveInteractiveRoi=true` 时会保留当前交互中的 ROI，只清叠加层重绘卡尺和结果线；结果对象画完后必须再完整 `repaint()`，按“背景图 -> 结果叠加层 -> ROI”的顺序保证 ROI 位于最上层。
- `Frm_FindLineTool.RoiControllerChanged()` 监听 `ROIController.EVENT_MOVING_ROI`，在控制器真正更新坐标后立即通过 `FindLineTool.SyncDisplayedRoi()` 回写 ROI，并调用 `ShowDraggingPreview()` 只画最新期望线和 ROI。阈值等参数变化以及鼠标松开统一进入 150ms `previewRefreshTimer` 合并刷新；如果 `Job.IsExecutionActive` 仍为 true，只保存参数并继续等待，绝不能与流程线程并发运行 `ShowContour()`。
- 找线窗口通过 `ViewWindow.displayInteractiveROI()` 直接把 `FindLineTool.L_regions` 中的对象挂入 `ROIController`，业务层和显示层共用同一个 ROI 实例，不能再由 `displayROI()` 创建副本。每轮 `mouseDownAction()` 会复位 `currX/currY`，`HWndCtrl.mouseUp()` 会用松开事件坐标再执行一次 `mouseMoveAction()`，保证被系统合并掉的最后一个移动坐标也能提交。
- `ROIRectangle2` 内部整体拖动使用 `activeHandleIdx=-1`；`displayActive()` 必须把该状态映射到中心手柄后再访问 `rows/cols`。所有 ROI 工具的 MouseMove 最终都会进入 `HWndCtrl.repaint()`；该函数必须在静态 `GraphicRenderLock` 内把 `flush_graphic=false`、背景/叠加/ROI 整帧绘制、`flush_graphic=true` 和最终提交作为一个不可交错的事务。这样既看不到 `ClearWindow()` 产生的背景闪烁，也不会让多个 HALCON 窗口并发切换进程级刷新状态。
- `FindLineTool.Run(runTool=false)` 是流程运行模式：不能写找线窗体文本框或直接操作 `Frm_FindLineTool`，但在 `displayLine=true` 时必须像查找圆一样通过工具基类 `ShowObj(finalLine, "green")` 把最终线送到主预览窗口；`runTool=true` 则使用 `Frm_FindLineTool.Instance.hWindow_Final1.DispObj()` 显示在工具窗口。不要再把最终线绘制整体限制为 `displayLine && runTool`，否则流程运行只计算结果却看不到主界面叠加线。
- `VisionAndMotion/2 ClassLib/Job.cs` 的 `ToolType.FindLine` 打开分支先解析 `图像/跟随`，在没有 ROI 时按当前图像尺寸补建默认 `Rect2`，重定位后再交给 `BindTool()` 统一绑定和回填。排查打开窗体后预览与运行不一致时，检查“输入解析 -> ROI 重定位 -> `BindTool()` -> `ShowContour(true, false)`”整条链。
- 界面参数回填已从 `Job.cs` 收口到 `Frm_FindLineTool.BindTool()`。如果字段在打开后显示不一致，优先检查 `BindTool()` 和 `bindingUi` 防重入，不要再在 `Job.cs` 新增控件赋值。
- 跟随输入通过 `templatePose + InputPar.跟随` 做刚性变换。`FindLineTool.BuildExpectedLines()` 会把学习时的预期线变换到每个当前位姿，因此一个工具运行可能对多个跟随位姿各测一条线。
- `templatePose` 是 ROI 编辑时的匹配位姿基准，不得每次打开找线窗口都覆盖。打开窗口只通过 `EnsureTemplatePoseFromCurrentInput()` 补齐新工具的空基准；用户实际拖动或缩放 ROI 后，`SyncDisplayedRoi()` 才调用 `CaptureTemplatePoseFromCurrentInput()` 按当前匹配位置重新标定。
- 图片位姿变化后重新打开找线窗口，先调用 `RebaseRoiToCurrentFollowPose()`，用 `templatePose -> 当前跟随位姿` 的刚性变换同步更新 `ROIRectangle2` 的中心和角度，再把当前位姿设为新基准。随后 `ShowContour(true, false)` 显示的 ROI、卡尺和正式运行坐标保持一致，不允许只变换测量线而仍显示原始 ROI。
- 流程运行解析找线输入时，`ClearLastInput()` 只允许在遍历输入连接之前执行一次，不能在每个输入项循环内再次把 `InputPar.跟随` 清空；否则当“跟随”排在“图像”之前时，模板匹配位姿会被后一个输入覆盖为空，找线退回固定 ROI。
- HALCON 参数统一收口在 `FindLineTool.ApplyMetrologyParams()`：`measure_transition=polarity`、`num_measures=cliperNum`、`measure_length1=Length`、`measure_length2=caliperWidth`、`measure_threshold=threshold`、`measure_select=edgeSelect`、`min_score=minScore`。调找线稳定性优先改这里，不要散改 `AddMetrologyObjectLineMeasure()` 的常量。
- `FindLineTool.Run()` 在 `ignoreNum == 0` 时直接取 HALCON 返回的线结果；`ignoreNum > 0` 时会先按“点到初拟合线的距离”排序，剔除最远的若干点，再用 `FitLineAfterReject()` 重拟合。以后若要增强抗毛刺能力，优先从这段离群点剔除逻辑入手。
- 对外输出在 `FindLineTool.ToolPar.ResultPar`，当前只写一条 `线`；界面上的起点/终点文本框只是同步显示。若要支持多条线结果，不能只改 UI，需要同时扩展结果结构和下游连接逻辑。
- `HObject != null` 不等于存在有效 HALCON 图像：尚未采图或旧项目恢复后对象 ID 可能仍为 0。构造默认 ROI、打开窗体、预览和运行中的图像尺寸读取必须先走 `ToolBase.TryGetHalconImageSize()`；无有效图像时保留兜底 ROI 并正常打开窗口，不能直接调用 `GetImageSize()`。

查找圆工具注意：

- 2026-09-09 已完整重制：查找圆与查找线使用同一结构和视觉层级，左侧编辑 `ROICircle`，右侧分页调整边缘、卡尺、质量和显示参数，结果页统一显示圆心、半径、耗时和状态。
- 核心测量只保留一条 `MeasureCircle()` 链路：预览和正式运行共用相同的 Metrology 建模、参数、最低得分、离群点剔除和图层生成逻辑，不再保留两套可漂移的预览/运行实现。
- 构造函数只创建数据和默认 ROI，不得创建窗体或调用 HALCON 测量。`Frm_FindCircleTool` 通过 `BindTool()` 持有当前流程的真实工具，切换多个查找圆节点时不得使用隐式新建工具作为运行对象。
- `Job.cs` 打开查找圆时依次执行：`EnsureLoadedState()` -> 清理上次输入 -> 按连线解析 `图像/跟随` -> 必要时按当前位姿重定位 ROI -> `BindTool()` -> 预览。不再每次清空重建 ROI，也不在 `Job.cs` 逐个回填 UI 控件。
- ROI 拖动/缩放、参数变化和显示开关统一走 150ms 防抖预览；鼠标松开后回写 `L_regions` 并更新当前 `templatePose`，流程运行中禁止并发预览。
- 屏蔽区数据为兼容旧工程仍保留，运行时会在测量前排除有效 `final_region`；旧的阻塞式涂抹 `Work()` 交互已移除，新 UI 只提供可恢复的“清除屏蔽区”。未来若恢复画笔，必须使用独立编辑状态和确认/取消，不得在 UI 线程循环阻塞。
- `Run()` 在入口先归一化参数、清空旧结果，再对每个跟随位姿独立测量和捕获异常；结果为空时返回失败状态，不向界面抛出。所有 HALCON 对象和 handle 在成功、失败、异常三条路径上都必须释放。
- 对外兼容字段仍用 `List<XY> _圆心` 保存，公开 `圆心` 输出第一个结果，`圆心列表` 供多位姿结果使用，`是否找到圆/圆半径/结果圆` 与该列表同步。现有坐标约定仍是 `XY.X=row`、`XY.Y=column`。
- `FindCircleTool.Execute()` 仍通过 `Run()` 接入执行包装；`TimeoutMs` 目前只能在运行结束后判断超时，不能中断已进入的 HALCON 调用。如需强制取消，必须增加独立工作进程或 HALCON 可取消包装。


模板匹配工具注意：

- 采集图像到模板匹配的运行链路是：`AcqImageTool.Run()` 更新 `toolPar.ResultPar.图像` -> 流程输入节点把采集输出连接到模板匹配 `MatchTool.toolPar.InputPar.图像` -> `MatchTool.Run()` 在当前输入图像上执行匹配。不要把实时预览图当作工具输入；实时预览只负责显示，工具运行依赖流程输出值。
- 配置窗体入口在 `VisionAndMotion/1 ToolLib/03 ShapeMatchTool/Frm_ShapeMatchTool.cs`。关键操作前应通过 `Frm_ShapeMatchTool.RefreshInputImageFromFlow(bool displayImage)` 从流程连接刷新当前输入图像，避免学习、运行、绘制模板区域继续使用打开窗体时的旧图或空图。这个函数会解析工具输入节点里类似 `工具名->输出项` 的来源，找到上游工具输出并写入 `shapeMatchTool.toolPar.InputPar.图像`。
- `Frm_ShapeMatchTool.button7_Click()` 是主窗体“重新学习”入口：先 `RefreshInputImageFromFlow(false)`，再检查模板区域，最后调用 `shapeMatchTool.CreateAndShowTemplate()` 和 `ShowTemplate()`。如果以后改学习流程，要先确认这里有没有刷新流程输入。
- `Frm_ShapeMatchTool.button6_Click()` 是模板“编辑”入口。现在不再打开 `Frm_MoreEdit` 弹窗，而是切到主窗体的模板页 `tabPage5`，调用 `shapeMatchTool.RefreshEditableTemplateDisplay()` 在同一个 `hWindow_Final1` 里编辑模板。这样编辑视图和外部绘制视图共用同一个窗口尺寸、缩放、平移和工具栏布局，避免弹窗窗口尺寸不同导致模板位置视觉不一致。
- `Frm_MoreEdit` 旧编辑窗体仍保留在目录里，但当前主流程不再从 `Frm_ShapeMatchTool.button6_Click()` 调用它。以后若清理旧窗体，要先确认没有其他入口直接使用 `Frm_MoreEdit.Instance.ShowDialog()`；如果恢复弹窗，也必须通过 `Frm_MoreEdit.BindTool(MatchTool tool)` 绑定主工具对象，不能新建独立 `MatchTool`。
- 已删除“样板图”概念：`ShapeMatchTool`（模板匹配）和 `MeasurementTool`（测量，结构是 `MatchTool` 的副本）原来都有 `standardImage`（做模板时的标准图像）字段，现已整体移除，全部统一使用当前流程实拍输入图 `toolPar.InputPar.图像`。`CreateTemplate()`、`CreateAndShowTemplate()`、`Work()`、`ShowTemplate()`、`ShowStandardImage()`、`ResetTool()` 以及 `Frm_MoreEdit`、`Frm_GlobalVariable`、`Frm_MeasurementTool` 里所有对 `standardImage` 的显示/建模引用都改成了 `toolPar.InputPar.图像`。以后不要再新增 `standardImage` 字段或用“保存的样板图”做回退显示。
- `ShapeMatchTool.CreateTemplate()` 是真正创建 Halcon 模型的函数，现在直接用 `toolPar.InputPar.图像` 配合 `templateRegion/final_region/final_region111/totalRegion` 得到训练区域，并调用 `CreateScaledShapeModel()` 或 `CreateNccModel()`。如果出现重新学习后仍用旧图，优先检查 `RefreshInputImageFromFlow()` 是否拿到了新图、`CreateAndShowTemplate()` 是否被绕过，以及 `MeasurementTool.CreateTemplate()` 是否被改回旧的样板图逻辑。
- `templateRegion` 是基础模板区域；`templateROIs` 是主窗口可编辑模板 ROI 列表，保存矩形、仿射矩形、圆、椭圆转多点 ROI 和任意形状 ROI 对象，并用 `ROI.POSITIVE_FLAG/ROI.NEGATIVE_FLAG` 区分添加区域和扣除区域；`final_region` 是后续擦除掉的区域；`final_region111` 是后续补画回来的区域；`totalRegion` 是最终传给 Halcon 建模的区域。改区域逻辑时要同时检查 `CreateTemplate()`、`ShowTemplate()`、`Work()`、`WorkCreateModel()`。
- 模板预览窗口 `hwc_template` 的统一重绘入口是 `ShapeMatchTool.ShowTemplatePreview()`：先用当前流程输入图和 `totalRegion` 得到预览图，再绘制最终学习区域和模型轮廓。`totalRegion` 才是真正参与建模的区域，预览和重新打开工具都不要再依赖 `templateRegion`，否则补画/扣除后的模板会显示成旧样板区域或黑屏。
- 模板匹配成功后的右上预览走 `ShapeMatchTool.ShowMatchedImagePreview()`：按本次输入图和第一个匹配位姿裁取实际目标，并叠加与主显示一致的结果标记；它和学习阶段的 `ShowTemplatePreview()` 是两种不同语义，运行成功后不能继续保留训练模板预览。
- 模板匹配运行结果统一由 `DisplaySearchRegion()` 和 `DisplayMatchResult()` 绘制：蓝色为搜索区域（整幅图搜索也画全图边界）、绿色为匹配到的模板区域、橙色为模型特征轮廓、黄色为匹配外接框、青色为匹配中心和序号。界面的“显示匹配到的模板 / 显示特征点 / 显示匹配框 / 显示搜索区域”等开关应只控制各自图层，不要再在 `Run()` 中分叉重复绘图。
- `ShapeMatchTool.SyncTemplateRegionFromEditableRois()` 是模板 ROI 到 Halcon 区域的同步函数：遍历 `templateROIs`，正区域做 `Union2`，负区域做 `Difference`，最后写回 `templateRegion`。`CreateAndShowTemplate()` 和 `CreateTemplate()` 会先调用它，保证拖动、缩放或新增 ROI 后再学习时使用的是最新区域。
- `ShapeMatchTool.RebuildTemplateRoisInWindow()` 解决 `HobjectToHimage()` 清空 ROI 控制器的问题：每次刷新模板编辑图像后，用 `ROI.getModelData()` 重新生成 `genRect1/genRect2/genCircle/genNurbs`，再恢复每个 ROI 的正负操作标记和颜色。凡是刷新编辑窗口后还需要继续拖动模板区域，都要经过这个函数。
- `Frm_ShapeMatchTool.Hwindow_MouseUp()` 是 ROI 拖动/缩放后的同步点。它通过 `hWindow_Final1.viewWindow.smallestActiveROI()` 取当前活动 ROI，更新搜索区域列表中对应对象，并调用 `shapeMatchTool.SyncTemplateRegionFromEditableRois()`，所以模板区域拖动后不需要重新画，也能进入后续学习。
- ROI 选中和拖动在 `ImageWindow/Model/ROIController.cs` 及各 `ROI` 子类里：鼠标按下先按原来的手柄距离命中边角/边界缩放手柄；如果没命中手柄，再调用 `ROI.selectMoveHandleIfInside()` 判断是否在 ROI 内部。普通矩形、仿射矩形、圆形、多点 ROI 的内部点击会进入整体平移状态，移动过程中用鼠标增量平移整个 ROI；边界手柄仍负责调整大小/旋转。以后改 ROI 交互时要保持“手柄优先、内部拖动次之”的顺序。
- `ShapeMatchTool.GetTemplateDisplayImage()` 是模板绘制前选择显示图像的统一入口，现在直接返回当前流程实拍输入图 `toolPar.InputPar.图像`（没有输入图就返回 null，由调用方做空图保护），不再有任何样板图回退。矩形、仿射矩形、圆、椭圆、任意形状绘制都应通过它显示图像。
- `ShapeMatchTool.FinishTemplateRegionDraw(Button activeButton)` 是模板区域绘制后的统一 UI 恢复入口，会恢复按钮颜色、`tbc_shapeMatch.Enabled`、`toolStrip1.Enabled`、`hWindow_Final1.DrawModel` 和右键菜单。`DrawTemplateRectangle1()`、`DrawTemplateRectangle2()`、`DrawTemplateCircle()`、`DrawTemplateEllipse()`、`DrawTemplateAny()` 的空图返回和异常捕获都应调用它，避免 Halcon 绘制取消或异常后按钮/页签失效。
- `DrawTemplateRectangle1()`、`DrawTemplateRectangle2()`、`DrawTemplateCircle()`、`DrawTemplateEllipse()`、`DrawTemplateAny()` 仍使用 Halcon 的阻塞式绘制函数取初始区域，但绘制完成后会马上调用 `viewWindow.genRect1/genRect2/genCircle/genNurbs` 生成可编辑 ROI，放入 `templateROIs`，再调用 `SyncTemplateRegionFromEditableRois()` 写回 `templateRegion`。椭圆当前会通过 `GenEllipseContourXld()` + `GetContourXld()` 转成多点 ROI，因此也能在主窗口里移动和调整；如果以后要做更标准的椭圆手柄体验，再在 `ViewWindow.Model` 增加专用椭圆 ROI 类。
- 打开模板匹配工具时不再绘制“样板图和样板模板”：`Job.cs` 的 `case ToolType.Match` 打开分支只在已有 `modelID` 和 `totalRegion` 时调用 `ShowTemplatePreview()`，用当前流程输入图重绘已学习模板；没有有效模板才清空预览窗。
- 搜索区域类型下拉 `cbx_searchRegionType_SelectedIndexChanged` 不应该直接调用 `DrawSearchRegion()`。绘制搜索区域必须由明确的绘制按钮触发，避免打开配置或切换选项时自动进入 Halcon 阻塞绘制并生成默认图形。
- 搜索区域选择“整幅图像”是例外：下拉事件必须立即调用 `UseAllImageSearchRegion()`，同步清空 `L_regions/searchRegionPointData/_searchRegion/reducedImage` 并设置 `searchRegionType=AllImage`。打开旧工程时如果类型已经是 `AllImage`，也要先清理序列化残留 ROI，避免界面仍画旧搜索区域。矩形、仿射矩形、圆和多点区域仍由明确的绘制按钮触发。
- 模板匹配的两个角度输入框语义是“起始角度、结束角度”，而 HALCON 查找算子接收“起始角度、角度跨度”。运行查找必须使用 `GetMatchAngleExtentRad()` 计算 `结束角度 - 起始角度`；例如 `-180/180` 对应 `360°`，不能把第二个输入值 `180°` 直接当跨度，否则搜索只覆盖 `-180°..0°`。
- Shape/NCC 模型学习统一由 `GetModelAngleStartRad()/GetModelAngleExtentRad()` 建立 `-180° + 360°` 的完整旋转模型；运行查找仍使用界面起始/结束角度限制实际搜索。这样扩大搜索范围时不会出现模型只学习了顺时针或逆时针一侧。启用该逻辑前保存的旧模型仍需重新学习一次。
- HALCON 匹配结果角度和下游 `XYU.U` 位姿内部使用弧度，`dgv_matchResult` 的“角度(°)”列必须转换成度显示；点击表格结果重绘模板时再把显示值转换回弧度。不要把内部结果直接改成度，否则直线查找的 `VectorAngleToRigid()` 跟随计算会发生单位错误。
- `ShapeMatchTool.ResetTool()` 会清空模型、模板区域、搜索区域、结果表和 UI 状态；`ShowTemplate()`、`ShowStandardImage()`、`CreateAndShowTemplate()` 都要做空图/未建模板保护，避免 Halcon 窗口或模型接口拿到空对象后卡死闪退。
- 新建 `ShapeMatchTool` 不应生成默认圆形、矩形或组合模板区域；构造函数必须用空 Halcon 对象初始化 `brush_region/final_region/brush_region111/final_region111`。`CreateTemplate()` 遇到空 `templateRegion` 必须返回未创建模板，`Run()` 捕获未建模板异常时不能自动调用 `CreateTemplate()`，否则会出现“首次运行只要采集有图就通过”的假通过。
- 模板匹配学习按钮在判断“是否已创建模板”前必须调用 `ShapeMatchTool.HasValidTemplateRegion()`，该方法会先 `SyncTemplateRegionFromEditableRois()`，再用面积判断模板区域是否有效。不要只判断 `templateRegion == null`，否则用户刚绘制的 ROI 可能还没同步就被误判为未创建模板。
- 模板匹配左上角工具栏的撤销按钮是 `Frm_ShapeMatchTool.tsb_undoTemplateRegion`，放在“训练图像/显示模板/重置”三个按钮旁边；点击后调用 `ShapeMatchTool.UndoLastTemplateRegion()` 删除 `templateROIs` 最后一个 ROI，重新同步 `templateRegion/totalRegion` 并刷新编辑窗口。
- 模板匹配工具栏“重置”入口在 `Frm_ShapeMatchTool.tsb_resetTool_Click()`。按钮不要直接在 UI 线程同步执行完整 `ResetTool()`；当前实现会先禁用按钮并在后台线程调用 `ShapeMatchTool.ResetRuntimeData()` 清理模型句柄、模板区域、搜索区域和结果数据，再用 `BeginInvoke` 回到 UI 线程调用 `ShapeMatchTool.ResetToolUI()` 清窗口、恢复控件默认值和启用状态。后续改重置逻辑时继续保持“Halcon 模型/数据清理”和“WinForms/Halcon 窗口刷新”分离，避免 `ClearShapeModel`、大图重绘或窗口清理卡死界面。

## 8. 运动和定位

运动底层：

- `VisionAndMotion/2 ClassLib/Motion/CardBase.cs`：运动卡基类。
- `Card_ADLink.cs`、`Card_LeadShineDMC2210.cs`、`Card_LeadShineDMC2410.cs`、`Card_WMX.cs`、`Card_Ymc3100.cs`、`Card_Googol.cs`、`Card_IOC0640.cs`：各运动/IO 卡适配。
- `AxisParameter.cs`：轴参数。
- `SmartPosTable.cs`：位置表。
- `ThreeColorLamp.cs`：三色灯。
- `VisionAndMotion/3 FormLib/Frm_MotionControl.cs`：运动控制界面。
- `VisionAndMotion/3 FormLib/Frm_AxisSetting.cs`：轴设置界面。
- `VisionAndMotion/3 FormLib/Frm_PosTableEdit.cs`：位置表编辑。

定位/标定工具：

| 工具 | 目录 |
| --- | --- |
| 手眼标定 | `VisionAndMotion/1 ToolLib/04 EyeHandCalibTool/` |
| 一键手眼标定 | `VisionAndMotion/1 ToolLib/05 OneKeyEyeHandCalibTool/` |
| 一维标定 | `VisionAndMotion/1 ToolLib/06 OneDimensionalCalibTool/` |
| 引用标定/坐标变换 | `VisionAndMotion/1 ToolLib/43 ApplyTransTool/` |

手眼标定工具注意：

- 核心算法文件是 `VisionAndMotion/1 ToolLib/04 EyeHandCalibTool/EyeHandCalibTool.cs`，配置界面是同目录的 `Frm_EyeHandCalibTool.cs`，表格列定义在 `Frm_EyeHandCalibTool.Designer.cs`。阅读顺序建议为：界面 `GetPixelXY()` 获取视觉点 -> `EyeHandCalibTool.Calibrate()` 求标定矩阵 -> `Run()` 转换流程输入。
- 该模块是工业视觉常说的二维平面手眼/九点标定，不是六轴机器人 `AX=XB` 型三维手眼标定。核心关系是 `(pixelRow, pixelColumn) -> (mechanicalX, mechanicalY)`，通过 HALCON `VectorToHomMat2d()` 求一般二维仿射矩阵 `homMat2D`。
- 项目沿用历史坐标约定：`XY.X` 通常存 HALCON Row，`XY.Y` 通常存 Column。界面虽然显示“像素X/像素Y”，算法变量实际叫 `pixelRow/pixelCol`；修改命名或交换顺序时必须同步检查取点工具、表格列、`AffineTransPoint2d()` 和所有下游连接。
- 标定表列定义为“组、像素X、像素Y、机械X、机械Y”。读取单元格时必须避开第 0 列组号；尤其检查 `FixedType.OutsideHand` 分支，不能把组号送入 `pixelRow`，也不能把像素Y误送入机械X。表格带自动追加空白行时，遍历有效数据应排除最后的新行。
- `FixedType.OutsideHand` 表示相机固定，可直接使用像素点与机械点的同名点对应关系；`FixedType.OnHand` 表示相机随轴移动，相机位移与“固定相机下工件的等效位移”方向相反，所以代码以第一组机械位置为基准对相对位移取反。
- `HomMat2dToAffinePar()` 分解出的 `ScanX/ScanY/Rotation/Theta/TranslateX/TranslateY` 只用于界面展示和诊断，正式运行始终使用完整 `homMat2D`。`ScanX/ScanY` 差异大表示非等比缩放，`Theta` 异常表示明显斜切，应优先检查点位质量、列顺序和 Row/Column 是否颠倒。
- `EyeHandCalibTool.Run()` 可转换三类输入：图像使用 `AffineTransImage()` 重采样；普通点和 XYU 的 Point 使用 `AffineTransPoint2d()` 转成机械坐标。XYU 当前只转换 Point，角度 `U` 原样透传，不会自动叠加仿射矩阵旋转量。
- 将像素到机械单位的 `homMat2D` 直接用于整幅图像会自然引入机械单位比例、非等比缩放或斜切；下游若只需要定位坐标，应优先连接“点/位置”输出，不要把变换后的整幅图像当作保持原像素几何的图像继续测量。
- 多拍照位补偿以 `D_photoPos[0]` 为基准，在普通点完成仿射转换后叠加当前拍照位与第 0 位的机械平移差。该补偿只处理 X/Y 平移，不处理不同拍照位之间的旋转。
- `ClearLastInput()` 用于每轮重新解析流程连接前清除旧图像、点和位置引用；`ResetTool()` 只复位界面参数和数据表，不会重新求解标定矩阵。新增运行入口时不要混淆“清输入”“复位界面”和“重新标定”。
| 上相机定位 | `VisionAndMotion/1 ToolLib/07 UpCamAlignTool/` |
| 下相机定位 | `VisionAndMotion/1 ToolLib/08 DownCamAlignTool/` |
| 点位引导 | `VisionAndMotion/1 ToolLib/39 PointAlignTool/` |
| 对位组装 | `VisionAndMotion/1 ToolLib/41 AlignFitTool/` |
| 旋转平台 | `VisionAndMotion/1 ToolLib/09 RotatePlatformTool/` |
| XY 平台 | `VisionAndMotion/1 ToolLib/10 XYPlatformTool/` |

## 9. 全局变量和输入输出连接

| 功能 | 入口文件 | 关键位置 |
| --- | --- | --- |
| 全局变量窗口 | `VisionAndMotion/3 FormLib/Frm_GlobalVariable.cs` | `LoadVariable()` 加载表格；`AddCustomVariable()` 添加自定义变量；`ReindexCustomVariables()` 删除/加载后重排自定义变量编号。 |
| 全局变量数据结构 | `VisionAndMotion/2 ClassLib/DataStrct.cs` | `GlobelVariable.L_variable` 保存变量；`Variable.index/type/name/value/info/variableType` 保存单项。`variableType=0` 为系统变量，`1` 为自定义变量。 |
| 编辑终端窗口 | `VisionAndMotion/3 FormLib/Frm_IOConfig.cs` | `Load()` 根据 `Frm_IOConfig.result1` 刷新树；空参数对象应显示为空树并隐藏添加按钮。 |
| 打开编辑终端 | `VisionAndMotion/2 ClassLib/Job.cs` | `ShowIOForm()` 填充工具下拉；`ShowIOEdit()` 根据选中工具刷新 `Frm_IOConfig.result1`。 |
| 源项右键菜单 | `VisionAndMotion/2 ClassLib/Job.cs` | `ConnectSource()` 写入输入项连接；`AddSourceCategory()`/`AddSourceItem()` 构建“源于”多级菜单。 |

输入输出连接注意：

- “源于”菜单在流程树输入项右键时生成，分类为 `全局 -> 系统变量/自定义变量`、`当前流程`、`其他流程 -> 流程名`。
- 菜单叶子显示文本可以短，但 `ToolStripItem.Name` 要保留完整连接字符串，例如 `《- 工具->输出项`、`《- [流程名]工具->输出项`、`《- 全局变量->变量名`，否则 `ConnectSource()` 解析和保存会出错。
- 当前流程源项要排除当前工具自身输出；其他流程不要按同名工具过滤，因为不同流程里可能有同名工具。
- `ShowIOEdit()` 需要在工具没有可显示参数时刷新为空，不能沿用上一次 `Frm_IOConfig.result1`。
- 自定义全局变量删除后要重排 `Variable.index`，添加时从当前自定义变量数量继续编号。

## 10. 工具目录快速索引

| UI 名称 | ToolType | 目录 |
| --- | --- | --- |
| 采集图像 | `ImageAcq` | `VisionAndMotion/1 ToolLib/01 ImageAcqTool/` |
| 预处理 | `ImagePreprocessing` | `VisionAndMotion/1 ToolLib/42 ImageProprecessingTool/` |
| 彩图转RGB | `ColorToRGB` | `VisionAndMotion/1 ToolLib/02 ColorToRGBTool/` |
| 存储图像 | `SaveImage` | `VisionAndMotion/1 ToolLib/37 SaveImageTool/` |
| 模板匹配 | `Match` | `VisionAndMotion/1 ToolLib/03 ShapeMatchTool/` |
| 距离测量 | `Measurement` | `VisionAndMotion/1 ToolLib/50 MeasurementTool/` |
| 斑点分析 | `BlobAnalyse` | `VisionAndMotion/1 ToolLib/11 BlobAnalyseTool/` |
| 图像相减 | `SubImage` | `VisionAndMotion/1 ToolLib/12 SubImageTool/` |
| 区域特征 | `RegionFeature` | `VisionAndMotion/1 ToolLib/20 RegionFeatureTool/` |
| 区域操作 | `RegionOperation` | `VisionAndMotion/1 ToolLib/22 RegionOperationTool/` |
| 条码识别 | `Barcode` | `VisionAndMotion/1 ToolLib/22 BarcodeTool/` |
| 二维码识别 | `QRCode` | `VisionAndMotion/1 ToolLib/21 QRCodeTool/` |
| OCR | `OCR` | `VisionAndMotion/1 ToolLib/21 OCRTool/` |
| 手眼标定 | `EyeHandCalib` | `VisionAndMotion/1 ToolLib/04 EyeHandCalibTool/` |
| 引用标定 | `QuoteTrans` | `VisionAndMotion/1 ToolLib/43 ApplyTransTool/` |
| 一键手眼标定 | `OneKeyEyeHandCalib` | `VisionAndMotion/1 ToolLib/05 OneKeyEyeHandCalibTool/` |
| 一维标定 | `OneDimensionalCalib` | `VisionAndMotion/1 ToolLib/06 OneDimensionalCalibTool/` |
| 上相机定位 | `UpCamAlign` | `VisionAndMotion/1 ToolLib/07 UpCamAlignTool/` |
| 下相机定位 | `DownCamAlign` | `VisionAndMotion/1 ToolLib/08 DownCamAlignTool/` |
| 点位引导 | `PointAlign` | `VisionAndMotion/1 ToolLib/39 PointAlignTool/` |
| 对位组装 | `AlignFit` | `VisionAndMotion/1 ToolLib/41 AlignFitTool/` |
| 上相机定位二 | `AlignWithoutCalibRotateCenter` | `VisionAndMotion/1 ToolLib/44 AlignWithoutCalibRotateCenterTool/` |
| 旋转平台 | `RotatePlatform` | `VisionAndMotion/1 ToolLib/09 RotatePlatformTool/` |
| XY平台 | `XYPlatform` | `VisionAndMotion/1 ToolLib/10 XYPlatformTool/` |
| 查找边 | `FindLine` | `VisionAndMotion/1 ToolLib/09 FindLineTool/` |
| 查找圆 | `FindCircle` | `VisionAndMotion/1 ToolLib/10 FindCircleTool/` |
| 创建ROI | `CreateROI` | `VisionAndMotion/1 ToolLib/13 CreateROITool/` |
| 阵列区域 | `ArrayRegion` | `VisionAndMotion/1 ToolLib/15 ArrayRegionTool/` |
| 标记点 | `Mark` | `VisionAndMotion/1 ToolLib/17 MarkTool/` |
| 组合位置 | `CreatePosition` | `VisionAndMotion/1 ToolLib/14 CreatePositionTool/` |
| 组合线段 | `CreateLine` | `VisionAndMotion/1 ToolLib/24 CreateLineTool/` |
| 转文本 | `ToStr` | `VisionAndMotion/1 ToolLib/38 PoseToStrTool/` |
| 显示编辑 | `DisplayEdit` | `VisionAndMotion/1 ToolLib/45 DisplayEditTool/` |
| 数据显示 | `Label` | `VisionAndMotion/1 ToolLib/30 LabelTool/` |
| 点点距离 | `DistancePP` | `VisionAndMotion/1 ToolLib/46 DistancePPTool/` |
| 点线距离 | `DistancePL` | `VisionAndMotion/1 ToolLib/17 DistancePLTool/` |
| 线线距离 | `DistanceSS` | `VisionAndMotion/1 ToolLib/18 DistanceLLTool/` |
| 线线交点 | `LLIntersect` | `VisionAndMotion/1 ToolLib/19 LLIntersectTool/` |
| 线线角度 | `AngleLL` | `VisionAndMotion/1 ToolLib/51 AngleSSTool/` |
| 两点中点 | `CenterOfPP` | `VisionAndMotion/1 ToolLib/26 CenterOfTwoPointTool/` |
| 数据分析 | `DataAnalyse` | `VisionAndMotion/1 ToolLib/49 DataAnalyseTool/` |
| 脚本编辑 | `CodeEdit` | `VisionAndMotion/1 ToolLib/30 CodeEditTool/` |
| 光源_奥普特 | `Light_OPT` | `VisionAndMotion/1 ToolLib/27 OPTLightTool/` |
| 光源控制 | `OPTLightControl` | `VisionAndMotion/1 ToolLib/33 OptLightControlTool/` |
| 扫码器_基恩士 | `Scaner_Kenyence` | `VisionAndMotion/1 ToolLib/19 KenyenceScanerTool/` |
| 以太网接收 | `EthernetReceive` | `VisionAndMotion/1 ToolLib/47 EthernetReceiveTool/` |
| 以太网发送 | `EthernetSend` | `VisionAndMotion/1 ToolLib/48 EthernetSendTool/` |
| 点补偿 | `BuChang` | `VisionAndMotion/1 ToolLib/35 BuChangTool/` |
| 点偏差 | `PointOffset` | `VisionAndMotion/1 ToolLib/40 PointOffsetTool/` |
| 输出项 | `Output` | `VisionAndMotion/1 ToolLib/18 OutputTool/` |
| PLC通讯 | `PLCComm` | `VisionAndMotion/1 ToolLib/49 PLCCommTool/` |

脚本编辑当前实现：

- 核心类在 `VisionAndMotion/1 ToolLib/30 CodeEditTool/CodeEditTool.cs`，通过 `L_inputItems`、`L_outputItems` 保存动态输入输出定义，并通过动态参数字典向流程暴露自定义名称的结果。旧 `L_calcItems` 字段仅为项目数据兼容保留，不参与运行。
- 配置窗体在 `VisionAndMotion/1 ToolLib/30 CodeEditTool/Frm_CodeEditTool.cs`，双击脚本编辑工具由 `Job.cs` 的 `ToolType.CodeEdit` 分支打开。界面为左右分栏：左侧包含输入、输出页签，右侧使用 `CodeEdit/FastColoredTextBox/` 的代码编辑器显示自定义代码。
- 新增脚本编辑工具时流程树不默认挂输入输出节点；在配置窗体保存后调用 `Job.SyncCodeEditIONodes()`，按当前配置同步显示自定义输入输出节点，并把变量来源同步到流程输入节点和当前流程连线。
- 输入项只包含序号、名称、类型和变量链接，不再支持固定值。链接菜单按 `全局变量`、`当前流程`、`其他流程` 组织，只提供标量字符串类流程输出以及 `Int/Double/String/Bool` 全局变量。流程树通过“源于”修改脚本输入时，`Job.ConnectSource()` 会同步更新 `CodeEditTool.L_inputItems` 及已打开的脚本窗体；脚本窗体保存后反向同步流程树。
- 流程运行分支在 `VisionAndMotion/2 ClassLib/Job.cs` 的 `ToolType.CodeEdit` 区域，输入取值由 `CodeEditTool.L_inputItems` 解析，结果通过动态名称直接回写流程树输出。
- 输入和输出均可自定义变量名称及 `Int`、`Double`、`String`、`Bool` 类型，不再限制为固定的 `imput1..10/output1..10`。
- 输入和输出表格均显示自动递增序号，新建项生成不重复的 `inputN`、`outputN` 默认名称，名称仍可自定义。
- `sourceCode` 保存完整 C# 类源码，不再由系统包裹成方法体。固定入口为 `VMPro.CodeEditRuntime.UserScript`，执行入口为 `public void Execute(Dictionary<string, object> inputs, Dictionary<string, object> outputs)`。输入按名称从 `inputs` 读取，结果按名称写入 `outputs`；用户可自由添加局部临时变量、成员变量、辅助方法和辅助类，不要求输入输出对应类字段。
- 新建脚本工具默认不创建输入输出实例，用户自行添加。用户源码模板只保留 `#region InputInitialization` 和 `#region FunctionImplementation` 两个内部折叠标记，对应的中文分区标题独立显示；`Read<T>` 类型转换由编译时注入的隐藏 `ScriptBase` 提供，不再显示在编辑区。旧的系统生成模板打开时会自动迁移折叠标记、移除辅助方法区域并继承 `ScriptBase`。
- 代码区提供“生成初始化”按钮：点击后同时重写 `#region InputInitialization` 内的输入初始化内容，并在 `#region FunctionImplementation` 里维护输出初始化和输出回写块。输入按左侧名称和类型生成 `Read<T>(inputs, "name")` 代码，并把链接来源写入相邻注释；输出按左侧定义生成同名局部变量，例如 `double result = default(double);`，用户功能代码可直接给 `result` 赋值，区域末尾再自动生成 `outputs["result"] = result;` 回写到流程输出。初始化注释通过 `CodeEditTool.FormatCommentText()` 仅在相邻中文字符之间加入空格，实际 `VariableSource` 链接值保持不变。代码区还提供“编译”按钮并检查 `UserScript` 和字典式 `Execute` 入口，保存时也会强制编译；运行后若未写入左侧定义的某个输出名称，会报告缺失输出，并在流程运行失败提示中显示脚本错误信息。
- 选择输入链接时会自动识别类型：全局变量读取其配置类型，脚本编辑输出读取其输出配置类型，其他流程输出根据当前值推断 `Int/Double/Bool/String`，无法确定时使用 `String`。流程树“源于”连接和脚本窗体选择链接均执行同样的类型同步。
- 脚本运行前，`Job.SyncCodeEditInputsBeforeRun()` 会再次以流程 `ToolInfo.input` 的连接为准同步 `VariableSource` 和类型；`CodeEditTool.ApplyInputItems()` 随后解析链接实际值、按配置类型转换并写入传给脚本的 `inputs` 字典，确保流程连接值进入对应输入名称。
- 脚本输出的来源只能是脚本内对 `outputs` 字典的写入，不允许在流程树再使用“源于”覆盖。`Job.IsCodeEditOutputNode()` 同时用于右键菜单过滤和 `ConnectSource()` 入口保护；脚本输出节点右键只保留“删除项”。
- 初始化区域和功能实现区域使用 `////////////////////// 输 入 初 始 化 区 域 //////////////////////` 样式，不再在长斜线前另加 `//`；系统中文标题和说明的汉字之间加入空格，两个功能区域内部的上下空白行也已加大。代码编辑器启用 C# 语法高亮、行号、自动缩进、4 空格 Tab、代码折叠和横向滚动，关闭视觉自动折行；字体使用 `NSimSun/新宋体`，中文恢复字体原始显示大小，不再缩小字形，也不统一加宽英文代码字符。`FastColoredTextBox.Font` 对该中文代码字体不回退到 `Courier New`。加载和保存时统一为 Windows `CRLF` 换行。`VMPro.csproj` 通过项目引用依赖 `CodeEdit/FastColoredTextBox/2 FastColoredTextBox.csproj`，该项目也已加入 `VM Pro.sln`。
- 输出项仍可勾选“写入全局变量”；写入目标直接使用输出项“名称”，修改名称后保存即可同步修改写入目标。脚本编辑窗口会检测未保存更改，关闭前提示保存。

- 主界面导入 `.pjt/.eng/.job` 示例流程时不要再把 `SDK_Halcon` 采集工具自动指向 `Config\Resources\Image` 的内置样图目录；`Frm_Main.ClearSampleAcqImageDirectory(Job job)` 会清空示例流程里的采集图片路径、目录、缓存和当前图像，防止流程预览继续显示样板图。

## 11. 常见修改定位

| 修改目标 | 先看 |
| --- | --- |
| 新增流程工具 | `Frm_ToolBox.cs`、`DataStrct.cs`、`Job.cs`、目标 `1 ToolLib` 目录。 |
| 修改某工具算法 | 对应 `1 ToolLib/<工具>/xxxTool.cs` 的 `Run()`。 |
| 修改某工具配置界面 | 对应 `1 ToolLib/<工具>/Frm_xxxTool.cs` 和 `.Designer.cs`。 |
| 修改工具输入输出连接 | `Frm_ToolBox.cs` 添加节点处、`Job.cs` 的 `ConnectSource()`/源项菜单生成/运行分支、`Frm_IOConfig.cs`。 |
| 修改全局变量 | `Frm_GlobalVariable.cs`、`DataStrct.cs` 的 `GlobelVariable`/`Variable`。 |
| 修改流程运行顺序/失败处理 | `Job.cs`。 |
| 修改项目保存/加载 | `Project.cs`。 |
| 修改设备添加/删除/选择 | `Frm_DeviceManager.cs`。 |
| 修改 PLC 设备配置页 | `Frm_PLCComm.cs`。 |
| 修改 PLC 流程读写 | `PLCCommTool.cs`、`Frm_PLCCommTool.cs`、`PLCDevice.cs`、`CipCommunication.cs`。 |
| 修改 TCP 收发 | `TCPSever.cs`、`TCPClient.cs`、`EthernetReceiveTool.cs`、`EthernetSendTool.cs`、对应配置窗体。 |
| 修改相机采集 | `AcqImageTool.cs` 和对应 `SDK_*.cs`。 |
| 修改模板匹配输入刷新/训练/编辑 | `ShapeMatchTool.cs`、`Frm_ShapeMatchTool.cs`、`Frm_MoreEdit.cs`；先确认流程输入 `toolPar.InputPar.图像` 是否来自采集工具输出。 |
| 修改 Halcon 显示/ROI | `ImageWindow/HWindow_Final.cs`、`ImageWindow/Model/ROI*.cs`。 |
| 修改运动控制 | `Motion/Card*.cs`、`Frm_MotionControl.cs`、`Frm_AxisSetting.cs`。 |
| 修改主菜单/布局 | `Frm_Main.cs`、`Frm_Main.ModernLayout.cs`、`ModernUiTheme.cs`、`Frm_LayoutManage.cs`。 |
| 修改共享输入/圆角/命令图标 | `ControlLib/Controls/ModernInputControl.cs`、`CTextBox.cs`、`CComboBox.cs`、`CNumeric.cs`、`CNumericUpDown.cs`，以及 `ModernUiTheme.cs`、`ModernVectorIconFactory.cs`。 |
| 修改工具箱分类 | `Frm_ToolBox.cs`、`Frm_ToolBox.ModernCategories.cs`；保留叶节点文字与创建分支兼容性。 |
| 修改公共标题栏按钮 | `Frm_FormBase.cs` 的 `AlignTitleButtons()`，不要逐窗体改 `button100.Location`。 |
| 修改语言/程序配置 | `Configuration.cs` 和设置页。 |

公司名称固定为“威乐普电子科技有限公司”，软件产品名固定为 `WLP VM`。`Configuration.Read()` 忽略旧 `Configuration.ini` 中的公司示例名；空标题、`未命名`、历史 `VM Pro`/通用视觉产品名和已知旧演示标题都归一化为 `WLP VM`。其他用户自定义项目名保持不变，主标题格式为“威乐普电子科技有限公司 - WLP VM v<版本> [Debug: <更新日期>] · 项目名”，因此加载自定义项目不会覆盖软件品牌。最近项目菜单只替换已知旧品牌/演示文件的显示名，实际路径和项目文件不重命名。

`Start.csproj` 的可执行程序输出名已改为 `WLP VM.exe`，应用图标为包含 9 个尺寸的 `Start/WLPVM.ico`，manifest 标识为 `WLPVM.app`。`VM Pro.sln`、`VMPro` 命名空间、`CVMPro.dll` 和第三方 `HintPath` 是源码/部署兼容名，本轮不重命名。新安装/新建配置的 `Configuration.dataPath` 默认为 `D:\WLP VM`；已有 `Configuration.ini` 或项目中序列化的 `D:\VM Pro` 不自动迁移、不移动数据，`Frm_SaveImageTool` 保留旧 `D:\VM Pro` 保存路径的切换兼容分支。新建“存储图像”工具仍使用桌面 `WLP VM 图像` 专用目录。

## 12. 快速搜索建议

优先使用 `rg`：

```bash
rg -n "ToolType.PLCComm|PLCCommTool|Frm_PLCCommTool|PLCDevice" VisionAndMotion -S -g "*.cs"
rg -n "case \"PLC通讯\"|new ToolInfo\\(ToolType.PLCComm" "VisionAndMotion/3 FormLib/Frm_ToolBox.cs"
rg -n "else if \\(L_toolList\\[i\\]\\.toolType == ToolType.PLCComm\\)" "VisionAndMotion/2 ClassLib/Job.cs"
rg -n "internal static Project LoadProject|EnsureCommunicationRuntime|L_PLCDevice" "VisionAndMotion/2 ClassLib/Project.cs"
rg -n "ConnectSource|AddSourceCategory|ShowIOEdit|Frm_IOConfig.result1" "VisionAndMotion/2 ClassLib/Job.cs"
rg -n "LoadVariable|ReindexCustomVariables|GlobelVariable|Variable" "VisionAndMotion/3 FormLib/Frm_GlobalVariable.cs" "VisionAndMotion/2 ClassLib/DataStrct.cs"
```

不要优先搜索 `bin/`、`obj/`，里面有旧构建产物和复制文件，容易误判。

## 13. 当前已知注意点

- 插件机投产前阻断项（2026-07-15 源码审查）：`Frm_Main.toolStripButton4_Click()` 在检查 `Homing/WaitReset` 之前先把 `Machine.machineRunStatu` 改成 `Running`，导致“未复位禁止启动”判断永远不会命中。修复前不能依赖该软件互锁保证运动安全。
- 插件机定制主逻辑尚未启用：`Machine.InitAll()` 中 `Task_SmartLineA.AutoRun()` 被注释，且 `Task_SmartLineA.AutoRun()` 自身入口立即 `return`。当前“开始”只会启动配置为 `LoopRunAfterStart` 的通用视觉流程，不等于已实现插件机的 PLC/运动时序。
- 当前 `Start/bin/Debug/Config` 仍是历史产线部署快照：配置标题、约 55 MB 项目文件、窗口/流程内容和 `LastProject.txt` 的旧绝对路径都不是通用出厂配置。运行时仅迁移品牌显示和已知旧布局，不会改写或重命名项目数据；因此这套配置不能作为客户方案直接上机。
- 当前部署快照 `FailStop=False`，连续流程中某轮 NG/异常后不会因此自动停止循环；插件机上线前必须根据 PLC 握手和不良处理时序明确失败策略，不能沿用该默认值。
- 手眼标定暂不可用于生产：`EyeHandCalibTool.Calibrate()` 的 `OutsideHand` 分支应读取表格第 1/2/3/4 列（像素X、像素Y、机械X、机械Y）并排除最后空行，当前却读取 0/1/2/3 列并遍历全部行；求矩阵失败后仍会继续分解旧矩阵，最后可能输出“标定成功”。
- 标定精度检查当前没有计算残差：`Frm_EyeHandCalibTool.button11_Click()` 保存数据后直接写入“标定完成，精度较高”。投产前必须增加独立验证点、像素到机械反投影残差、最大/平均误差阈值，并用实机走位确认 Row/Column 与 X/Y 方向。
- 一键手眼标定的 TCP `Connect/Receive` 仍没有统一取消与完整状态机；本轮仅补了 2 s 连接超时、共享 Socket 生命周期、手动断开和配置页 UI 派发。未加入运动边界、急停/安全门硬件互锁验证前，不应在插件机上使用“一键标定”自动走位。
- `VisionAndMotion/2 ClassLib/PLCDevice.cs` 是旧编码/unknown-8bit 文件，编辑时避免整文件转码；尽量做小范围修改。
- 当前目录是 Git 仓库根目录；提交前用 `git status --short` 核对范围，并用 `git diff --check` 检查空白错误。
- 相机 SDK 仍有必须独立处理的高风险资源所有权问题：`SDK_HIKVision.ImageCallBack()` 对回调传入的 `pData` 调用 `Marshal.FreeHGlobal()`，需先依据海康 SDK 的缓冲区所有权契约确认并在真机受控测试；`SDK_MindVision` 使用跨实例静态 `frameBuffer`，多相机并发、重复枚举和释放边界尚未验证。不要把这些问题混入纯 UI 发布，也不要用生产设备重试来证明安全。
- 部分工具箱 case 中存在历史乱码/占位英文 case，修改工具名称时要谨慎，不要误删仍被旧项目引用的名称。
- `Job.cs` 很大，多个阶段都按 `ToolType` 分支处理。改某个工具时，要同时检查“打开工具窗体”和“运行工具”两个分支。
- `Job.Run()` 设计为后台线程执行。任何“运行流程/运行一次”入口都必须放后台线程，不能在按钮事件里同步调用，否则相机采图等阻塞 IO 会卡死 UI；公共状态和图像显示入口负责异步派发，但各具体工具仍需检查是否绕过公共入口直接访问 WinForms/HALCON。仍以 UI 线程同步调用 `Run()` 的历史入口要逐个迁移，不能用 `Application.DoEvents()` 掩盖阻塞。
## Recent Notes

- 2026-09-10: 后续截图证明实际 EXE 仍使用旧的 HALCON 候选路径逻辑。发现并修复解决方案 Debug Any CPU/x64 配置遗漏 Start 的 Build.0，避免“解决方案生成了业务项目但跳过启动 EXE 和内容复制”；启动错误标题加 v3 以便辨认新旧程序。MSBuild 解析验证仅改变这两个构建勾选，标准内容复制目标和启动源码隔离编译通过；用户 Windows 原生启动仍待验收。
- 2026-09-10: 根据 Windows 启动检查截图补齐原生 HALCON 漏发：此前原生 DLL 只在被 Git 忽略的 bin 中，Git 拉取后仍缺文件。现将现有 `17.12.0.1` x64/x86 核心库纳入 `Lib/Halcon`，构建复制到 EXE 旁按架构分开的 Halcon 子目录，启动优先从该目录加载；不再要求用户手动从旧输出目录复制 DLL。11 项路径检查、启动源码隔离编译和两份内容复制/哈希检查通过，Windows 原生运行仍待验收。
- 2026-09-10: 修正 `Configuration.BuildApplicationTitle()` 两处字符串边界的中文弯引号为 ASCII 双引号；Roslyn 对实际文件的语法诊断由 29 项降为 0，保留字符串中的中点及原标题内容。另补启动阶段 HALCON 绝对路径探测、位数/版本过滤和失败诊断，验证范围见 1.1；尚未复现或验证用户 Windows 上的原生加载异常。标题中的 `[Debug: 日期]` 当前没有条件编译，在 Release 中同样显示，本轮未改变这一显示约定。
- 2026-09-09: 查找边（直线）和圆查找完整重制为当前暖白/浅蓝界面：左侧 HALCON 交互图、右侧基本/运行/结果分页、底部预览/运行工具/运行流程，参数层级参考海康 VisionMaster 的 ROI + 卡尺边缘测量工作流。两工具收口真实对象绑定、输入解析、ROI 跟随/回写、150ms 防抖预览、参数归一化、多位姿测量、结果显示和 HALCON 资源释放；圆查找的预览/正式运行统一到 `MeasureCircle()`，查找线预览也与正式运行共用同一参数和离群点规则。无图、无 ROI、单位姿测量失败和无结果改为状态返回并记录日志，不向窗体抛异常。macOS 使用 .NET Framework 4.8 参考包完成 `VMPro.csproj` 编译，产出 `CVMPro.dll` 且 0 编译错误；本结论不代表 Windows 实际界面、HALCON 授权、相机图像或现场节拍已通过。
- 2026-09-09: 修复斑点分析连续运行的 `NullReferenceException`：原路径在工作线程直接调用 `GetImageWindowControl().hwc_imageWindow.HobjectToHimage(...)`，图像窗口未就绪或切页时可返回 null。斑点背景、搜索区、结果、外接圆和十字改为快照后单次 UI 批量投递，无可用窗口时只跳过显示，工具计算与输出不中断；结果表也不再从后台隐式创建编辑窗口。输入同步增加 object ID 0 检查。当前实际工程目录为 `VM Pro -1.0.4`；macOS 只做源码编译，Windows 连续运行和 HALCON 显示由目标环境验收。
- 2026-09-09: 修正流程连线的混合展开状态：输入端和输出端各自判断所属模块是否展开，已展开的一端始终落到具体步骤端口，另一端折叠时只收口该端。仅在两端都折叠时按模块对归并；`FlowEditorSmoke` 增加两个方向的混合端点回归。Windows 界面效果由目标环境验收。
- 2026-09-09: 收口连续运行异常和首页进入视觉卡顿路径：`LoopRun()` 复用当前 Job，不再每轮重复查找；`Run()` 内部异常返回 null 时立即停止连续运行，错误日志记录流程、工具、异常类型和真实异常堆栈；移除每轮 `GC.Collect()`。新增统一受保护 UI 投递，采集、模板匹配、查找线、查找圆及流程结束回调的异步异常不再逃逸到 WinForms；查找圆流程运行不再从工作线程直接读取工具窗体句柄或写结果控件。连续运行停留首页/运动页时跳过隐藏 HALCON 绘制及逐轮成功日志，进入视觉后下一轮恢复最新显示；流程窗体首次显示取消重复延迟刷新，下拉内容不变时不重建，主命令图标复用 DPI/位图缓存。macOS 上 `VMPro.csproj /t:Compile` 通过且 `FlowEditorSmoke` 源码编译通过；Windows HALCON、相机及实际切页耗时仍由目标环境验证。
- 2026-09-09: 修复打开圆查找时报 `HalconDotNet.HOperatorException #4056 get_image_size object ID is NULL (0)`：新增统一 `TryGetHalconImageSize()`，不再把非 null 的空 `HObject` 当有效图像；圆查找备用对象使用不读取当前流程图像的构造路径，属性 getter 不再递归创建窗体，打开/预览/运行/屏蔽入口均先验证图像。查找线的同类构造、打开、预览和运行入口同步加固。无有效输入图像时窗口仍可打开并保留兜底 ROI，实际查找需先运行上游采集。`FlowEditorSmoke` 增加对象 ID 为 0 的反射回归；本轮仅完成 macOS 上 .NET Framework 4.8 目标编译，Windows HALCON 运行待目标环境验证。
- 2026-09-09: 隐藏“显示全部连线”后的聚焦粒度由模块改为具体端口：选中模块标题不显示连线，选中输入端口只显示该输入的来源，选中输出端口显示该输出的去向；折叠模块因无可选端口不显示聚焦线。同步更新按钮提示和 `FlowEditorSmoke` 的模块/输入端口/输出端口回归。Windows 实际交互待目标环境验收。
- 2026-09-09: 修复首次进入视觉页必须先切换流程才能使用展开/折叠/删除的问题：现代流程工具栏在第一个默认流程加入及窗体首次显示时主动激活当前流程，不再使用尚未生成的隐藏页签 `RowCount` 判断。完全折叠状态继续按有向模块对归并连线，同一对模块只画一条概要线；`FlowEditorSmoke` 增加首次流程未切换即启用命令和完全折叠归并回归。当前工程目录已由 `VM Pro -NEW` 改名为 `VM Pro -1.0.1`；Windows 实际界面运行仍由目标环境验收。
- 2026-09-09: 旧项目/方案/流程的恢复路径统一重建：全部流程运行状态先归零，流程树改为“先建齐模块端口、再恢复连线”，移除旧双击次数对展开/折叠的拦截；坏连接只标记单端口，不再中断整个默认流程。查找线/查找圆补齐旧文件缺失集合、运行锁、输入参数和位姿基准，打开和运行改用统一来源解析，且打开查找圆不再清空已有学习基准。保留现有 `.pjt/.job` 二进制、非空工具对象及模板/ROI/标定参数；本轮 macOS 仅完成 .NET Framework 4.8 目标的 MSBuild 编译和烟测源编译，Windows 打开默认项目、展开连线、圆查找及连续运行仍由目标环境验收。
- 2026-09-09: 流程模块右键改为精确命中当前模块，补齐单模块禁用/启用和删除后的模型、端口、连线与跨流程下游清理；双击改为命中模块或其端口均可打开，补充运行中阻止、重入门闩和 `finally` 状态恢复。连线后续已改为两端独立判断：展开端保留具体端口，折叠端才收口到模块，两端都折叠时才归并为唯一模块关系线。主窗口与 `Frm_FormBase` 标题按钮改用 DPI 矢量图形和更大点击区，并为最大化/还原、置顶状态提供即时视觉反馈。`FlowEditorSmoke` 已增加命中、禁用/启用、删除清理、折叠连线归并和标题按钮回归；本轮仅完成 macOS 上的项目源文件 Roslyn 语义编译，Windows MSBuild、烟测执行和实际打开各工具窗口由目标 Windows 环境继续验收。
- 2026-09-08: 软件产品名更新为 `WLP VM`，公司名固定为“威乐普电子科技有限公司”；主菜单从 9 类收敛为项目/流程/视觉/设备/系统/帮助 6 类，主命令栏固定为 7 个机器/工作区命令，视觉快捷栏收敛为 4 个直达 + 1 个批量运行菜单。项目菜单保留导出方案但不暴露不完整的顶层删除代理；删除统一走系统 → 选项 → 方案管理。低频但有效的上一张本地图像、暂停目录图自动切换和极速模式归入视觉菜单，全局变量继续代理原刷新入口。选项及 F5/F6 运行菜单同步源命令状态，标题栏移除重复更多按钮，菜单代理不复制旧低清位图，重复或空实现入口不占顶栏。
- 2026-09-08: 四类共享输入控件改用统一抗锯齿底板；`CTextBox`/`CNumeric` 收口原子事件与数字编辑中间态，`CNumericUpDown` 补齐 50/70 px 响应式布局。普通圆角按钮/卡片及欢迎页内部品牌/退出控件移除二值 `Region`，主命令图标按当前像素尺寸代码绘制；`WLPVM.ico` 提供 9 个尺寸并统一 Start/manifest/窗体宿主图标。新配置默认 `D:\WLP VM`，旧序列化 `D:\VM Pro` 不迁移。外层窗体 `Region` 与尚未声明的全局 DPI 感知仍是现场显示验证边界。
- 2026-09-07: 第二轮统一为浅蓝暖白视觉；视觉工作区改为中央图像、右侧流程/工具箱标签、底部输出/监控标签；启动页重绘为矢量图；存储图像模块采用三卡片布局并补齐安全清空和写图/清理调度边界。无硬件渲染与烟测覆盖这些界面，完整程序、相机/PLC/运动卡和现场 DPI 仍待验证。
- 2026-09-07: 主界面首轮完成蓝白简约重排：主页生产概览、三工作区、首轮 9 个菜单入口、7 类工具箱、6 类设置导航、只读工厂 Dock 模板，以及按可见性限频的 UI/运动/日志刷新；通信配置页改为按需创建并按绑定目标派发，配置后台读取不再触碰主窗体，公共图像显示回到 UI 线程。当前菜单结构以 2026-09-08 的 6 类收敛结果为准。
- 2026-07-15: ImageAcq -> Match 只传递本次会话新采集/新选择的图像。项目加载后 `AcqImageTool.OnDeserialized()` 清空 `lastPreviewImagePath` 和 `ResultPar.图像`；打开采集或模板匹配窗口不再自动调用 `TryLoadLastPreviewImage()`。
- 2026-06-27: Flow-run image display for ImageAcq depends on `Job.Run()` calling `AcqImageTool.Run(true, false, toolName)`. Passing `updateImage=false` still updates outputs, but the main image window will not show the newly acquired image. If `useTemplateImageInRun` is enabled, `AcqImageTool.Run()` must also call `ShowImage(...)` in that branch.
