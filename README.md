# VM Pro（VisionAndMotionPro）

> 机器视觉 + 运动控制软件平台（C# WinForms）。采用“方案 → 流程 → 工具”的可视化流程式编程方式，面向自动化设备（插件机、组装/检测设备等）的视觉定位、测量、标定与 PLC/运动控制联机应用。

## 目录

- [功能特性](#功能特性)
- [技术栈](#技术栈)
- [解决方案结构](#解决方案结构)
- [核心架构](#核心架构)
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

- 语言/框架：C#，.NET Framework 3.5 / 4.0 / 4.5 / 4.5.2，WinForms（Visual Studio 2013 解决方案格式，x86 为主）。
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

`Start/Program.cs` → `VM.Init()`（`VisionAndMotion/2 ClassLib/VM.cs`：欢迎页 + 后台线程 `Machine.InitAll()` 初始化/连接硬件）→ 等待 `Machine.loading == false` → 显示主窗体 `Frm_Main`。

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
- `Job.Run()` 按“后台工作线程”设计：UI 更新一律通过 `BeginInvoke(...)` 投递，后台线程不能直接操作 WinForms/HALCON 窗口。
- 统一入口：`Job.RunAsync(jobName)` 异步运行；`Job.RunAndWait(jobName)` 放后台线程并等待结果（适合“运行后立刻读输出”的标定/取点）。**不要在 UI 事件里直接同步调用 `Job.Run()`**，否则相机/PLC/TCP 阻塞会卡死界面。
- 连续运行 `LoopRun()`、`Task_SmartLineA` 自动流程等业务工作线程内可直接 `Run()`（依赖同步结果）。

## 工具体系

完整工具索引见 `PROJECT_GUIDE.md` 第 10 节。常用工具分类：

| 分类 | 工具 |
| --- | --- |
| 图像采集 | 采集图像（Halcon/Basler/海康/迈德威视/PointGrey） |
| 图像处理 | 预处理、彩图转 RGB、存储图像、图像相减 |
| 定位识别 | 模板匹配、查找边、查找圆、斑点分析、OCR、条码、二维码、区域特征/操作 |
| 标定对位 | 手眼标定、一键手眼标定、一维标定、上/下相机定位、点位引导、对位组装、旋转平台、XY 平台 |
| 计算工具 | 点点/点线/线线距离、线线交点/角度、两点中点、组合位置/线段、转文本、数据分析 |
| 通讯控制 | PLC 通讯、以太网接收/发送、扫码器_基恩士、光源_奥普特/光源控制 |
| 其他 | 脚本编辑、创建 ROI、阵列区域、标记点、点补偿、点偏差、输出项、显示编辑 |

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

## 运动控制

`VisionAndMotion/2 ClassLib/Motion/`：

- `CardBase.cs`：运动卡抽象基类。
- 驱动实现：固高（`Card_Googol.cs`）、研华（`Card_ADLink.cs`）、雷赛（`Card_LeadShineDMC2210/2410.cs`）、WMX（`Card_WMX.cs`）、YMC3100（`Card_Ymc3100.cs`）、IO 卡（`Card_IOC0640.cs`）等。
- `AxisParameter.cs`：轴参数；`SmartPosTable.cs`：点位表；`ThreeColorLamp.cs`：三色灯。

## 环境与构建

- 需要 Windows + Visual Studio（2013 及以上，含 .NET Framework targeting pack），项目依赖 .NET Framework 3.5/4.0/4.5/4.5.2。
- 第三方依赖（需先安装/放置，部分 HintPath 指向 `bin\Debug\Dll` 或外部目录）：
  - HALCON 运行时与 `halcondotnet.dll`；
  - 相机 SDK：Basler Pylon、海康 MVS、PointGrey、迈德威视；
  - `HslCommunication`、`Newtonsoft.Json`（`packages/` 下含 NuGet 包）；
  - 运动控制卡 SDK、DevComponents DotNetBar（启动时检测缺失并提示安装）。
- 在缺少 .NET Framework targeting pack 的非 Windows 环境，`dotnet build` 会失败，属预期。
- 编译产物相关：搜索代码时不要优先查 `bin/`、`obj/`（内含旧构建产物与复制文件）。

## 快速开始

1. 用 Visual Studio 打开 `VM Pro.sln`，将 `Start` 设为启动项目，选择 x86 构建。
2. 首次运行前确认 `Start\bin\Debug\Config\Configuration.ini` 等配置与部署环境匹配（当前提交中的快照历史项目为“手机组装”，见下方已知问题）。
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
- **部署快照不匹配**：`Start/bin/Debug/Config` 是历史“手机组装”快照（`Configuration.ini` 中 `ProgramTitle=手机组装`），`LastProject.txt` 还指向另一台主机的绝对路径，不能直接作为插件机方案上机。
- **手眼标定暂不可生产**：`EyeHandCalibTool.Calibrate()` 的 `OutsideHand` 分支读取列 1/2/3/4（应为像素 X/Y、机械 X/Y 前 4 列并排除最后空行）且失败后仍可能输出“标定成功”；标定精度检查未计算残差。
- **一键标定风险**：TCP `Connect/Receive` 无超时/取消/完整状态机，后台线程直接操作 WinForms；未验证运动边界、急停/安全门硬件互锁前不应使用。
- **失败策略**：部署快照 `FailStop=False`，连续流程 NG/异常后不会自动停止循环，需按 PLC 握手与不良处理时序明确策略。
- **编码注意**：`VisionAndMotion/2 ClassLib/PLCDevice.cs` 为旧编码（unknown-8bit）文件，编辑时避免整文件转码，做小范围修改。

## 相关文档

- `PROJECT_GUIDE.md`：项目导览与代码定位指南（启动、项目生命周期、流程引擎、模板匹配、脚本编辑、设备通讯的详细注意点）。**定位代码时优先阅读本文件**；若与源码不一致，以源码为准并同步更新。
- `更新日志.txt`：近期功能更新记录。
