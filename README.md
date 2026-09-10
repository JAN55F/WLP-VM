# WLP VM

> WLP VM 是威乐普电子科技有限公司的工业机器视觉 + 运动控制软件平台（C# WinForms）。采用“方案 → 流程 → 工具”的可视化流程式编程方式，面向自动化设备的视觉定位、测量、标定与 PLC/运动控制联机应用。

## 目录

- [功能特性](#功能特性)
- [技术栈](#技术栈)
- [解决方案结构](#解决方案结构)
- [核心架构](#核心架构)
- [界面与工作区](#界面与工作区)
- [工具体系](#工具体系)
- [设备与通讯](#设备与通讯)
- [运动控制](#运动控制)
- [环境与构建](#环境与构建)
- [快速开始](#快速开始)
- [开发指南：新增一个工具](#开发指南新增一个工具)
- [当前状态与已知问题](#当前状态与已知问题)
- [相关文档](#相关文档)

## 功能特性

- **流程式视觉编程**：以可视化方式搭建流程，工具间通过“输入/输出”节点连线传递图像、数值、位姿等数据，支持跨流程引用与全局变量。
- **丰富的视觉工具（50+）**：图像采集、预处理、模板匹配（形状/NCC）、查找边/圆、斑点分析、OCR、条码/二维码、区域特征与操作、手眼标定、一键标定、上下相机定位、对位组装、距离/角度/交点/中点等计算工具。
- **设备与通讯**：PLC（EtherNet/IP）、TCP 服务端/客户端、串口、基恩士扫码枪、奥普特光源，均有设备管理和流程工具。
- **运动控制**：多品牌运动控制卡驱动（固高、研华、雷赛、WMX、YMC3100 等），支持点位表与轴参数配置。
- **脚本扩展**：内置 C# 脚本编辑工具，可自定义输入/输出变量与类型，编译执行并接入流程数据流。
- **多语言界面**：中/英文界面切换、用户权限与登录锁屏、方案/项目文件管理（`.pjt` / `.eng`）。

## 技术栈

- 语言/框架：C#，.NET Framework 3.5 / 4.0 / 4.5 / 4.5.2，WinForms（Visual Studio 2013 解决方案格式；当前 Debug 主工程与启动程序的实际 `PlatformTarget` 均为 AnyCPU，Release 保留 x86 兼容配置）。
- 视觉引擎：HALCON（`halcondotnet`）。
- 相机 SDK：Basler Pylon、海康 MVS（`MvCameraControl.Net`）、PointGrey FlyCapture2、迈德威视 MVSDK。
- 通讯库：`HslCommunication`（PLC/以太网）。
- 序列化：`Newtonsoft.Json` + .NET 二进制序列化。
- 界面：DockPanel 停靠（`WeifenLuo.WinFormsUI.Docking`）、自绘控件库、FastColoredTextBox 代码编辑器。

## 解决方案结构

主解决方案：`VM Pro.sln`。

| 项目 | 路径 | 作用 |
| --- | --- | --- |
| Start | `Start/Start.csproj` | 启动项目，程序入口 `Start/Program.cs` |
| VMPro | `VisionAndMotion/VMPro.csproj` | 主业务项目：流程、工具、设备管理、主界面（绝大多数修改在此） |
| 1 WinFormsUI | `DockForm/WinFormsUI/1 WinFormsUI.csproj` | Docking 停靠窗口库（一般不改） |
| HWindow_Tool | `ImageWindow/HWindow_Tool.csproj` | HALCON 图像窗口 / ROI 显示控件 |
| LightController | `LightController/LightController.csproj` | 光源控制器抽象与实现 |
| Controls | `ControlLib/Controls/Controls.csproj` | 自定义 WinForms 控件 |
| FastColoredTextBox | `CodeEdit/FastColoredTextBox/2 FastColoredTextBox.csproj` | 脚本编辑器控件 |

附属解决方案：`CodeEdit/SharpEdit.sln`（脚本编辑器）、`MyChart/MyChart.sln`（图表控件）、`ImageWindow/HWindow_Tool.sln`（图像窗口独立工程）。

### VMPro 顶层目录

| 目录 | 作用 |
| --- | --- |
| `VisionAndMotion/1 ToolLib/` | 所有流程工具，每个工具通常包含 `xxxTool.cs`（运行类）与 `Frm_xxxTool.cs`（配置窗体） |
| `VisionAndMotion/2 ClassLib/` | 核心业务类：项目、方案、流程、工具基类、设备、通讯、运动控制、日志 |
| `VisionAndMotion/3 FormLib/` | 主界面、设备管理、设置页、流程窗口等公共窗体 |
| `VisionAndMotion/4 EditPart/` | 项目定制逻辑与用户表单（例如插件机 `Task_SmartLineA.cs`） |

## 核心架构

### 启动链路

`Start/Program.cs` 先从 `Config\Config.ini` 预读语言，供单实例/启动失败等最早提示使用；进入 `VM.Init()` 后，在构造欢迎页和主窗体之前由 `Configuration.Read(false)` 读取 `Config\Configuration.ini` 的最终业务配置，再显示欢迎页、在 UI 线程实例化 `Frm_Main`，并由后台线程执行 `Machine.InitAll()` 初始化/连接硬件。`Machine.InitAll()` 不再重复读取 `Configuration.ini`，避免集合项重复和英文配置先生成中文壳层；等待 `Machine.loading == false` 后隐藏欢迎页并显示主窗体。

### 项目生命周期

| 功能 | 核心位置 |
| --- | --- |
| 设备初始化 | `VisionAndMotion/2 ClassLib/Machine.cs`（`InitAll()`） |
| 项目单例与序列化 | `VisionAndMotion/2 ClassLib/Project.cs`（`LoadProject()`/`SaveProjectToPath()`） |
| 方案 | `VisionAndMotion/2 ClassLib/Scheme.cs`（`.eng` 只保存一个方案） |
| 流程 | `VisionAndMotion/2 ClassLib/Job.cs`（流程编辑、工具列表、运行主循环、输入输出流转） |
| 工具信息/基类 | `ToolInfo.cs`、`ToolBase.cs` |
| 公共数据结构/枚举 | `VisionAndMotion/2 ClassLib/DataStrct.cs`（`ToolType` 等） |

- `.pjt` 保存完整项目（全部方案、流程及工具）；`.eng` 只保存一个方案。项目保存先写同目录 `.tmp` 再替换正式 `.pjt`，并保留 `.bak`。
- `Config\LastProject.txt` 记录上次成功打开/保存的项目路径，启动时优先恢复。

### 流程运行（线程模型，重要）

- `Job.Run(bool initRun)` 是单次跑完整条流程的核心函数，内部逐个调用各工具的 `Run()`；采集、PLC、TCP 等工具会做阻塞 IO。
- `Job.Run()` 按“后台工作线程”设计；流程开始/结束状态和公共图像/叠加显示入口会投递回 UI 线程。工具内部仍可能直接调用 HALCON 算子，新增或修改工具时必须逐项确认其线程边界，不能把“后台运行”理解为所有历史工具都已天然线程安全。
- 统一入口：`Job.RunAsync(jobName)` 异步运行；`Job.RunAndWait(jobName)` 放后台线程并等待结果（适合“运行后立刻读输出”的标定/取点）。**不要在 UI 事件里直接同步调用 `Job.Run()`**，否则相机/PLC/TCP 阻塞会卡死界面。
- 连续运行 `LoopRun()`、`Task_SmartLineA` 自动流程等业务工作线程内可直接 `Run()`（依赖同步结果）。

## 界面与工作区

当前主界面采用浅蓝与暖光白的桌面软件结构：蓝色标题带、全局命令栏、标准菜单、工作区内容和底部状态栏。顶层菜单只保留“项目、流程、视觉、设备、系统、帮助”六类；全局命令栏固定为启动/暂停/停止/复位 4 个机器命令和首页/视觉/运动 3 个工作区导航，共 7 个主命令。退出、锁定、登录、选项收入菜单，不再与顶栏重复；标题栏移除与“帮助”菜单重复的更多按钮，只保留最小化、最大化/还原和关闭。普通操作按钮、输入框和内容卡片使用适度圆角，窗口控制区、图像画布和需要精确对齐的数据网格仍保留清晰边界。

三个工作区通过主命令栏稳定切换：

| 工作区 | 用途 | 调度特点 |
| --- | --- | --- |
| 主页 | 只读生产概览：机器状态、当前方案、流程数、运行时长与常用入口 | 首次进入时创建，离开后隐藏并保留状态。 |
| 视觉 | 中央图像、右侧流程/工具箱标签、底部输出/监控标签 | 显示精简视觉快捷栏；加载当前布局或工厂默认布局。 |
| 运动 | 运动控制与 IO 查看 | 首次进入时创建；只有有效可见且非运行状态才轮询 IO。 |

- 工具箱固定为 7 类：图像输入与预处理、检测与识别、标定与定位、几何与 ROI、逻辑与计算、设备与通信、输出与显示。
- 设置导航固定为 6 类：常规、项目、方案、启动、运行、用户与安全。
- 工厂默认 Dock 布局位于 `Start/Config/Resources/Layout/经典布局1.config`：图像占据中央主区，流程与工具箱共用右侧标签 Pane，输出与监控共用底部标签 Pane。经典模板只读，用户修改会保存为 `dockPanel.config`；任意用户自定义布局不会被覆盖。仅对仓库附带、可明确识别的旧演示快照在内存中迁移到专注布局；其他项目可在“视觉 → 布局 → 专注布局（标准）”选择并于重启后生效。
- “项目”菜单覆盖常用方案/项目生命周期：新建、打开、最近、克隆、导出方案、保存项目、项目导入/导出和退出。顶层不保留历史“删除当前方案”快捷入口：旧事件只从模型集合直接移除，缺少完整 UI 刷新和持久化保障；需要删除方案时统一进入“系统 → 选项 → 方案管理”，沿用受控确认、刷新和保存路径。
- 视觉快捷栏只直显“单次运行、连续运行、保存项目、读取图像”4 个高频动作，另保留 1 个“批量运行”下拉。上一张本地图像、暂停目录图自动切换放在“视觉 → 图像”，极速模式放在“视觉 → 辅助工具”；全局变量保留在“视觉”并继续代理原 `toolStripButton34`，以沿用既有刷新逻辑。方案、流程、布局、设备及重复/空实现入口不再占用视觉顶栏。
- 主菜单中迁移出来的代理命令统一显示清晰文字并转发到原按钮事件，不复制旧按钮携带的低分辨率位图，避免高 DPI 下再次模糊，同时不绕过原权限与刷新语义。菜单展开时同步源命令的 `Enabled`、`Available` 和可勾选状态；“系统 → 选项”继续走原管理员权限入口，流程菜单中的 F5/F6 继续触发原单次/连续运行按钮，并与按钮共用禁用门槛，避免机器运行期间从快捷键或菜单绕过状态限制。
- `CTextBox`、`CComboBox`、`CNumeric`、`CNumericUpDown` 四类共享输入控件统一继承 `ModernInputControl`，以 GDI+ 抗锯齿路径绘制暖白底板、浅蓝焦点边框，并移除小尺寸控件原有的二值 `Region` 裁切；下拉箭头、密码可见性及数值加减符号由代码绘制，避免继续拉伸低清位图。`CTextBox.TextStr` 的程序赋值与占位状态原子同步，避免一次赋值产生旧值/空值两次通知；`CNumeric` 允许输入过程中的空文本、负号和小数点中间态，不抛转换异常，只有完整有效数字才提交一次值变化，失焦时恢复/规范化到最后有效值。`CNumericUpDown` 在 50 px 宽时保留纯文本与键盘/滚轮步进，70 px 宽时采用纵向加减按钮，避免数值被按钮遮挡。普通按钮和内容卡片同样改用带透明过渡像素的抗锯齿背景，而非用 `Region` 硬裁圆角。
- 主命令栏和视觉快捷栏的功能图标由 `ModernVectorIconFactory` 按当前显示尺寸绘制，线帽、连接和轮廓启用抗锯齿；ToolStrip 使用精确像素图并关闭二次缩放，减少旧位图放大后的模糊。应用图标改为 `Start/WLPVM.ico`，包含 16/20/24/32/40/48/64/128/256 共 9 个尺寸；`Start.csproj` 将其同时设为 `ApplicationIcon` 和内容资源，两份 manifest 的程序集标识统一为 `WLPVM.app`，欢迎页和主窗体从当前宿主可执行文件提取图标，避免继续显示旧 `vm` 图形。
- 工具配置窗体共享 `ModernUiTheme` 的暖白页面、浅蓝选择态、圆角输入与分层按钮。`存储图像` 已先行改为“图像来源 / 存储规则 / 文件管理”卡片结构；清空保存位置会拦截相对路径、磁盘根目录、用户/系统/程序目录和链接重定向目录，并与后台写图共用原子操作边界和二次确认。写图由跨流程单工作线程串行处理，不再为每张图创建清理线程或固定等待 1 秒；队列最多缓存 64 项，磁盘持续跟不上时对调用方背压，不丢弃请求但也不承诺全程非阻塞。
- 主刷新计时器以 100 ms 为基准，时钟/运行状态限频至 1 s，运动 IO 在满足可见性和运行状态条件时限频至 300 ms；设备管理、运动控制、点表编辑及 TCP/PLC/串口/扫码等配置窗体按需创建，隐藏页面不再接收显示型日志。输出日志在模型队列中收集，UI 每 100 ms 最多批量提交 48 条并只滚动一次，突发报警使用唯一时间键保留最近 1000 条。启动阶段只把硬件初始化/连接留在工作线程，WinForms 创建与状态回写通过 UI 线程派发；配置读取只更新模型字段，不从后台触碰主窗体。这里是 UI 调度优化，不代表真实驱动调用已经完成时延验证。

## 工具体系

完整工具索引见 `PROJECT_GUIDE.md` 第 10 节。界面中的工具箱按以下 7 类组织，叶节点名称及工具创建映射保持兼容：

| 分类 | 工具 |
| --- | --- |
| 图像输入与预处理 | 采集、读取、颜色转换、预处理、存储与图像运算 |
| 检测与识别 | 模板匹配、斑点、OCR、条码/二维码及特征检测 |
| 标定与定位 | 一维/手眼标定、上下相机定位、点位引导、平台与对位工具 |
| 几何与 ROI | ROI 创建、查找/拟合、距离/角度/交点等几何测量 |
| 逻辑与计算 | 流程控制、数学运算、脚本与数据分析 |
| 设备与通信 | 光源、扫码器、PLC 与以太网收发 |
| 输出与显示 | 文本转换、结果显示、输出项等 |

新增/修改工具的标准路径：

1. `DataStrct.cs` 添加/确认 `ToolType` 枚举。
2. `1 ToolLib/<工具目录>/` 添加或修改 `xxxTool.cs` 与 `Frm_xxxTool.cs`。
3. `Frm_ToolBox.cs` 添加工具箱节点、新建工具分支、输入输出项。
4. `Job.cs` 添加工具的“打开窗体”分支和“运行”分支。
5. `VMPro.csproj` 确认新文件已加入 `<Compile Include=...>`。

## 设备与通讯

| 功能 | 数据/运行类 | 配置窗体 | 流程工具 |
| --- | --- | --- | --- |
| PLC | `VisionAndMotion/2 ClassLib/PLCDevice.cs`、`CipCommunication.cs` | `3 FormLib/Frm_PLCComm.cs` | `1 ToolLib/49 PLCCommTool/PLCCommTool.cs` |
| TCP 服务端 | `2 ClassLib/TCPSever.cs` | `Frm_TCPServer.cs` | `1 ToolLib/47 EthernetReceiveTool/` |
| TCP 客户端 | `2 ClassLib/TCPClient.cs` | `Frm_TCPClient.cs` | `1 ToolLib/48 EthernetSendTool/` |
| 串口 | `2 ClassLib/Serial.cs` | `Frm_Serial.cs` | 主要被扫码/设备工具引用 |
| 扫码枪 | `2 ClassLib/Scaner.cs` | `Frm_Scaner.cs` | `1 ToolLib/19 KenyenceScanerTool/` |
| 光源 | `LightController/` | `Frm_LightController.cs` | `1 ToolLib/27 OPTLightTool/`、`33 OptLightControlTool/` |

PLC 关键点：设备列表在 `Project.Instance.L_PLCDevice`；连接对象缓存于 `PLCDevice.L_cipComm`；项目加载后由 `Project.EnsureCommunicationRuntime()` 调 `PLCDevice.ResetRuntimeStore()` / `EnsureRuntime()` 修复运行时；流程工具通过 `PLCCommTool.PLCDeviceName` 绑定设备。

设备运行对象与配置窗体已解耦：连接、接收和状态变化只更新模型，并仅向“已经创建且仍绑定当前设备”的可见页面派发显示；切换设备后，旧连接的迟到结果不会覆盖新页面。TCP 客户端手动断开会抑制自动重连，再次点击连接时才恢复连接意图。

## 运动控制

`VisionAndMotion/2 ClassLib/Motion/`：

- `CardBase.cs`：运动卡抽象基类。
- 驱动实现：固高（`Card_Googol.cs`）、研华（`Card_ADLink.cs`）、雷赛（`Card_LeadShineDMC2210/2410.cs`）、WMX（`Card_WMX.cs`）、YMC3100（`Card_Ymc3100.cs`）、IO 卡（`Card_IOC0640.cs`）等。
- `AxisParameter.cs`：轴参数；`SmartPosTable.cs`：点位表；`ThreeColorLamp.cs`：三色灯。

## 环境与构建

- 需要 Windows + Visual Studio 2022 Build Tools（含对应 .NET Framework targeting pack）。解决方案文件为 VS2013 格式，但当前命令行验证使用 VS2022 的 `MSBuild.exe`，不要用 `dotnet build`。
- 第三方依赖（需先安装/放置，部分 HintPath 指向 `bin\Debug\Dll` 或外部目录）：
  - HALCON 运行时与 `halcondotnet.dll`；
  - 相机 SDK：Basler Pylon、海康 MVS、PointGrey、迈德威视；
  - `HslCommunication`、`Newtonsoft.Json`（`packages/` 下含 NuGet 包）；
  - 运动控制卡 SDK、DevComponents DotNetBar（启动时检测缺失并提示安装）。
- 推荐命令：`MSBuild.exe "VM Pro.sln" /t:Build /p:Configuration=Debug /p:Platform="Mixed Platforms"`。使用 `/t:Build`；历史依赖引用使 `/t:Rebuild` 或清理后的纯净重建不一定可复现。
- `Debug|Mixed Platforms` 下 VMPro 为 AnyCPU；Start 的解决方案配置名映射到 `Debug|x86`，但 `Start.csproj` 的 `PlatformTarget` 是 AnyCPU。`Release|x86` 只是现有发布配置，尚未完成全部原生 SDK 的本轮发布验证。
- 搜索代码时不要优先查 `bin/`、`obj/`（内含旧构建产物与复制文件）。当前目录是 Git 仓库根目录，可直接用 `git status` 和 `git diff` 审查源码改动。

运行目录中三份 INI 不是重复文件：

| 路径 | 职责 |
| --- | --- |
| `Config\Config.ini` | Start 启动最早阶段预读语言，仅供配置完全载入前的单实例/错误提示；`Machine` 确保该基础文件存在。 |
| `Config\Configuration.ini` | `VM.Init()` 在欢迎页/主窗体构造前通过 `Configuration.Read(false)` 读取最终语言、程序、运行、工作区和当前布局等主业务设置；`Machine.InitAll()` 不重复读取。 |
| `Config.ini` | 根目录级注册信息；用户管理窗体仍保留该历史 INI 实例，但当前未见实际读写。 |

## 快速开始

1. 用 Visual Studio 打开 `VM Pro.sln`，将 `Start` 设为启动项目，选择 `Debug|Mixed Platforms`；命令行则使用上面的 VS2022 MSBuild `/t:Build` 命令。
2. 首次运行前确认 `Start\bin\Debug\Config\Configuration.ini` 等配置与部署环境匹配（仓库仍带有旧产线演示数据，见下方已知问题）。运行时产品固定为“威乐普电子科技有限公司 - WLP VM”；空标题和已知旧品牌值归一化为 `WLP VM`，用户自定义项目名仍保留并以“· 项目名”附在主标题后。新建配置的 `dataPath` 默认为 `D:\WLP VM`；已有项目/配置中序列化的 `D:\VM Pro` 不自动迁移，存储图像模块保留针对旧路径的兼容分支，避免升级时擅自移动或改写客户数据。
3. 启动后进入主界面：打开一个 `.pjt` 项目，在“方案/流程”中编辑流程，从工具箱拖入工具并连线，点“运行一次”验证。

## 开发指南：新增一个工具

```text
1. DataStrct.cs        → 添加 ToolType 枚举
2. 1 ToolLib/xxx/      → 新建 xxxTool.cs（继承 ToolBase，实现 Run()）和 Frm_xxxTool.cs
3. Frm_ToolBox.cs      → 注册工具箱节点、新建工具分支、输入输出项
4. Job.cs              → 添加“打开窗体”case 分支 与“运行”分支
5. VMPro.csproj        → 确认文件已编译进项目
```

修改工具时请同时检查 `Job.cs` 中的两个分支（打开窗体、运行），它们均按 `ToolType` 分发。

## 当前状态与已知问题

> 详细清单见 `PROJECT_GUIDE.md` 第 13 节。投产前必须逐条复核。

- **互锁缺陷**：`Frm_Main.toolStripButton4_Click()` 在检查复位状态前已置 `Running`，导致“未复位禁止启动”判断永不命中，不能依赖该软件互锁保证运动安全。
- **插件机主逻辑未启用**：`Machine.InitAll()` 中 `Task_SmartLineA.AutoRun()` 被注释，且其内部入口直接 `return`；当前“开始”只启动配置为 `LoopRunAfterStart` 的通用流程。
- **部署快照不匹配**：`Start/bin/Debug/Config` 仍包含历史产线演示项目与旧绝对路径。软件只迁移品牌显示和已知旧布局，不重命名约 55 MB 的项目文件、不改写其中的工艺数据，因此该快照不能直接作为客户方案上机。
- **手眼标定暂不可生产**：`EyeHandCalibTool.Calibrate()` 的 `OutsideHand` 分支读取列 1/2/3/4（应为像素 X/Y、机械 X/Y 前 4 列并排除最后空行）且失败后仍可能输出“标定成功”；标定精度检查未计算残差。
- **一键标定风险**：TCP `Connect/Receive` 仍缺少统一超时/取消与完整状态机；本轮只收口共享连接表、窗体派发和手动断开语义。未验证运动边界、急停/安全门硬件互锁前不应使用。
- **失败策略**：部署快照 `FailStop=False`，连续流程 NG/异常后不会自动停止循环，需按 PLC 握手与不良处理时序明确策略。
- **编码注意**：`VisionAndMotion/2 ClassLib/PLCDevice.cs` 为旧编码（unknown-8bit）文件，编辑时避免整文件转码，做小范围修改。
- **本轮本机验证**：2026-09-08 UI 基线的 `Controls.csproj` Rebuild 为 0 警告/0 错误、`VMPro.csproj` Rebuild 为 961 警告/0 错误；点击视觉线程修复后的 `VMPro.csproj` Build 为 539 警告/0 错误，推荐的 `Debug|Mixed Platforms` 解决方案 Build 为 4 警告/0 错误。无硬件 `UiShellSmoke` 以退出码 0 通过 1088 项断言，除输入高度/窄宽、6 菜单/7 主命令/视觉 4+1、权限、圆角和图标外，新增 `CheckForIllegalCrossThreadCalls=true` 下两个空流程节点连续选择、首页→视觉切换并等待 900 ms 的回归；EXE 嵌入图标与源 ICO 像素匹配 100%。最终 `CVMPro.dll` SHA-256 为 `11A828BF847E4144525D169F37DC1938648DF1C811706BCD40A635E11325A27A`。
- **本轮验证边界**：为避免 `VM.Init()` 自动进入 `Machine.InitAll()` 并连接现场设备，没有启动完整程序，也没有触发真实相机、PLC 或运动卡。无硬件预览只能验证控件构造、逻辑尺寸及绘制输出；DockPanelSuite 子窗不会完整进入 `DrawToBitmap`，灰色预留区不是像素验收结果。欢迎页内部品牌标记、退出按钮、输入、普通按钮和卡片已使用抗锯齿绘制，但无边框外层 `Form` 仍保留 WinForms `Region` 形状裁切；当前启动清单也没有声明全局/Per-Monitor DPI 感知，因此 100%/125%/150% DPI、多屏切换及外窗圆角边缘必须在目标 Windows 环境验收。直接对 `Start.csproj` 强制 `/p:Platform=x86` 会因被引用的 `VMPro` 没有该项目级映射而失败；当前可复现入口仍是解决方案的 `Debug|Mixed Platforms` 映射，不能把独立 x86 命令写成已支持路径。
- **可见运动页延迟边界**：`Machine.UpdateIO()` 目前仍在 UI 计时器路径同步等待驱动和 `Machine.lock_resources`；低频与可见性门控已减少调用，但慢驱动仍可能冻结界面，必须在隔离设备环境测量后再决定异步快照方案。
- **相机 SDK 独立风险**：海康回调中释放回调 `pData`、迈德威视跨实例静态帧缓冲等资源所有权问题不属于本轮 UI 改动，必须另开批次，依据 SDK 契约并在受控真机环境验证；不得以生产设备反复重试代替验证。

## 相关文档

- `PROJECT_GUIDE.md`：项目导览与代码定位指南（启动、项目生命周期、流程引擎、模板匹配、脚本编辑、设备通讯的详细注意点）。**定位代码时优先阅读本文件**；若与源码不一致，以源码为准并同步更新。
- `更新日志.txt`：近期功能更新记录。
