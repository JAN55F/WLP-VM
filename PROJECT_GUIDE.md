# VM Pro Project Guide

> 最后更新时间：2026-07-15（Asia/Shanghai）
>
> 后续定位代码时先读本文件。它是快速导览，不替代源码；如果导览和源码不一致，以源码为准，并同步更新本文件。

## 1. 解决方案和项目

主解决方案：`VM Pro.sln`，Visual Studio 2013 格式。

| 项目 | 路径 | 作用 |
| --- | --- | --- |
| Start | `Start/Start.csproj` | WinForms 启动项目，程序入口在 `Start/Program.cs`。 |
| VMPro | `VisionAndMotion/VMPro.csproj` | 主业务项目：流程、工具、设备管理、主界面。绝大多数修改在这里。 |
| 1 WinFormsUI | `DockForm/WinFormsUI/1 WinFormsUI.csproj` | DockPanel 停靠窗口库。通常不改。 |
| HWindow_Tool | `ImageWindow/HWindow_Tool.csproj` | Halcon 图像窗口/ROI 显示控件。 |
| LightController | `LightController/LightController.csproj` | 光源控制器抽象和实现。 |
| Controls | `ControlLib/Controls/Controls.csproj` | 自定义 WinForms 控件。 |

常见附属解决方案：

- `CodeEdit/SharpEdit.sln`：脚本编辑器相关。
- `MyChart/MyChart.sln`：图表控件/工具。
- `ImageWindow/HWindow_Tool.sln`：图像窗口单独工程。

构建注意：

- 当前项目依赖 .NET Framework 3.5/4.0/4.5/4.5.2、Halcon、相机 SDK、运动控制 SDK、HslCommunication 等 Windows/VS 环境组件。
- 在非 Windows 或缺少 Developer Pack 的环境下，`dotnet build` 会因为缺少 .NET Framework targeting pack 失败。

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
| `packages/` | NuGet 包，当前含 HslCommunication、Newtonsoft.Json 等。 |

## 3. 程序启动和项目生命周期

| 功能 | 入口文件 | 核心函数/位置 |
| --- | --- | --- |
| 程序启动 | `Start/Program.cs` | `Main()`：读取配置、单实例提示、启动主窗体。 |
| 主程序初始化 | `VisionAndMotion/2 ClassLib/VM.cs` | `Init()`：显示欢迎页，实例化主窗体，后台启动 `Machine.InitAll()`，等待 `Machine.loading == false` 后先隐藏欢迎页，再显示主窗体。 |
| 设备初始化 | `VisionAndMotion/2 ClassLib/Machine.cs` | `InitAll()`：启动时初始化硬件、自动连接设备。 |
| 项目单例和序列化 | `VisionAndMotion/2 ClassLib/Project.cs` | `Project.Instance`、`LoadProject()`、`InportProject()`、`ExportProject()`、`EnsureCommunicationRuntime()`。 |
| 配置项 | `VisionAndMotion/2 ClassLib/Configuration.cs` | 程序标题、语言、运行参数等。 |
| 方案 | `VisionAndMotion/2 ClassLib/Scheme.cs` | 方案下挂多个 Job。 |

定位提示：

- 加载 `.pjt` 项目文件的问题，先看 `Project.LoadProject()` 和 `Project.InportProject()`。
- 项目保存统一由 `Project.SaveProjectToPath()` 写入：先序列化到同目录 `.tmp`，再替换正式 `.pjt`，并保留一个 `.bak` 上次版本；不要再用 `OpenOrCreate` 直接覆盖，也不要删除 `Config\Project\Vision` 中的其他项目文件。
- `Config\LastProject.txt` 保存最后一次成功打开或保存的 `.pjt` 路径。启动时 `Machine.InitAll()` 调用 `Project.LoadStartupProject()` 优先恢复该文件；记录失效时回退到 `Config\Project\Vision` 中最近修改的项目，主文件加载失败时再尝试 `.bak`。
- `.pjt` 保存完整项目（全部方案、流程及工具），`.eng` 只保存一个方案。`Scheme.OpenScheme()`/`LoadScheme()` 必须把 `.eng` 反序列化为 `Scheme` 并写回当前项目，不能把 `.eng` 当成 `Project`，也不能再用 `Frm_Main.openProject` 阻止后续项目保存。
- 启动后设备没有恢复/连接，先看 `Project.EnsureCommunicationRuntime()` 和 `Machine.InitAll()`。
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
- 工具运行主循环：搜索 `Run(` 或具体 `else if (L_toolList[i].toolType == ToolType.xxx)`，各工具运行和上下游数据读写都集中在这里。
- 通讯类工具启动初始化时跳过：搜索 `initRun`。

运行流程的触发入口和线程模型（重要）：

- `Job.Run(bool initRun)` 是单次跑完整条流程的核心函数，内部会逐个调用各工具的 `Run()`，其中采集图像、PLC、TCP 等工具会做阻塞式 IO（相机采图、读寄存器、socket 收发）。
- `Job.Run()` 是按“后台工作线程”设计的：函数开头只用 `BeginInvoke(...)` 非阻塞投递 UI 更新，流程结束后的 HALCON 状态文字也通过 `PostRunResultToUi()` 回到 UI 线程显示。后台流程线程不能直接操作 WinForms/HALCON 窗口。
- 统一入口在 `Job.cs`：`Job.RunAsync(jobName)` 异步运行指定流程；`Job.RunAndWait(jobName)` 把实际运行放到后台线程，等待期间继续处理 UI 消息，适合“运行后立刻读取输出结果”的标定/取点按钮。不要在 UI 事件里再直接调用 `Job.FindJobByName(...).Run()`。
- 主界面“运行一次”、图像窗口右键“运行流程”、采集设备页和各工具窗体的“运行流程”入口都应走 `RunAndWait()` 或 `RunAsync()`，避免相机、PLC、TCP 等阻塞 IO 卡死 WinForms 消息循环。
- `Job.LoopRun()` 的连续运行线程、`Task_SmartLineA` 自动流程、`OneKeyEyeHandCalibTool` 内部自动标定链仍保留直接 `job.Run()`，因为它们本身在业务工作线程里顺序执行，并依赖同步结果。排查卡死时先区分入口是在 UI 线程还是已有工作线程。
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

设备管理 UI 调度：

- `Frm_DeviceManager.ShowSelectedDevice()`：根据设备类型显示子页面。
- `Frm_DeviceManager.ShowChildForm()`：把子窗体嵌入右侧区域。
- PLC 页面：`Frm_PLCComm.Instance.LoadPar(PLCDevice)`。
- TCP 服务端：`Frm_TCPServer.Instance.LoadPar(TCPSever)`。
- TCP 客户端：`Frm_TCPClient.Instance.LoadPar(TCPClient)`。
- 光源：`Frm_LightController.Instance.LoadPar(LightController_Base)`。
- 扫码枪：`Frm_Scaner.Instance.LoadPar(Scaner)`。
- 串口：`Frm_Serial.Instance.LoadPar(Serial)`。

窗体标题栏注意：

- 继承 `Frm_FormBase` 的窗口标题栏按钮由 `Frm_FormBase.AlignTitleButtons()` 在运行时统一右对齐，顺序为 `置顶`、`最小化`、`最大化`、`关闭`。
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
- 这个旧 WinForms/.NET Framework 方案依赖 Halcon 和相机 SDK，macOS 下 `dotnet build` 可能长时间无输出或卡住；验证优先交给 Windows/Visual Studio 和实际相机环境。

查找线工具注意：

- 核心文件是 `VisionAndMotion/1 ToolLib/09 FindLineTool/FindLineTool.cs`，界面文件是 `VisionAndMotion/1 ToolLib/09 FindLineTool/Frm_FindLineTool.cs`。优先阅读顺序：构造函数默认 ROI -> `ShowContour()` 预览 -> `Run()` 正式运行 -> `SyncDisplayedRoi()` 与窗体鼠标事件。
- 线查找的搜索区域不是普通矩形，而是 `ROIRectangle2` 表示的“可旋转卡尺区域”。`FindLineTool.GetBaseLine()` 直接从该 ROI 的顶点数据中取第 7/9 号端点作为预期线起止点，因此改 ROI 数据结构或端点顺序时会直接影响找线结果。
- `FindLineTool.EnableLineRoiEditing()` 会给 `ROIRectangle2.EndpointRotationEnabled` 赋值，使内部拖动表示整体平移、两端拖动同时改变长度和角度。这是找线专用交互；如果预览恢复成普通旋转矩形，先检查这里是否在显示 ROI 后被重新调用。
- `FindLineTool.ShowContour(bool showROI, bool trans, bool preserveInteractiveRoi)` 是调参预览主入口：它只显示卡尺、边缘点和拟合线，不写 `ResultPar`。`preserveInteractiveRoi=true` 时会保留当前交互中的 ROI，只清叠加层重绘卡尺和结果线；结果对象画完后必须再完整 `repaint()`，按“背景图 -> 结果叠加层 -> ROI”的顺序保证 ROI 位于最上层。
- `Frm_FindLineTool.RoiControllerChanged()` 监听 `ROIController.EVENT_MOVING_ROI`，在控制器真正更新坐标后立即通过 `FindLineTool.SyncDisplayedRoi()` 回写 ROI，并调用 `ShowDraggingPreview()` 只画最新期望线和 ROI。阈值等参数变化以及鼠标松开统一进入 150ms `previewRefreshTimer` 合并刷新；如果 `Job.IsExecutionActive` 仍为 true，只保存参数并继续等待，绝不能与流程线程并发运行 `ShowContour()`。
- 找线窗口通过 `ViewWindow.displayInteractiveROI()` 直接把 `FindLineTool.L_regions` 中的对象挂入 `ROIController`，业务层和显示层共用同一个 ROI 实例，不能再由 `displayROI()` 创建副本。每轮 `mouseDownAction()` 会复位 `currX/currY`，`HWndCtrl.mouseUp()` 会用松开事件坐标再执行一次 `mouseMoveAction()`，保证被系统合并掉的最后一个移动坐标也能提交。
- `ROIRectangle2` 内部整体拖动使用 `activeHandleIdx=-1`；`displayActive()` 必须把该状态映射到中心手柄后再访问 `rows/cols`。所有 ROI 工具的 MouseMove 最终都会进入 `HWndCtrl.repaint()`；该函数必须在静态 `GraphicRenderLock` 内把 `flush_graphic=false`、背景/叠加/ROI 整帧绘制、`flush_graphic=true` 和最终提交作为一个不可交错的事务。这样既看不到 `ClearWindow()` 产生的背景闪烁，也不会让多个 HALCON 窗口并发切换进程级刷新状态。
- `FindLineTool.Run(runTool=false)` 是流程运行模式：不能写找线窗体文本框或直接操作 `Frm_FindLineTool`，但在 `displayLine=true` 时必须像查找圆一样通过工具基类 `ShowObj(finalLine, "green")` 把最终线送到主预览窗口；`runTool=true` 则使用 `Frm_FindLineTool.Instance.hWindow_Final1.DispObj()` 显示在工具窗口。不要再把最终线绘制整体限制为 `displayLine && runTool`，否则流程运行只计算结果却看不到主界面叠加线。
- `VisionAndMotion/2 ClassLib/Job.cs` 的 `ToolType.FindLine` 打开分支会在显示窗体后立刻完成 4 件事：绑定 `Frm_FindLineTool.jobName/toolName` 和 `Frm_FindLineTool.findLineTool`、按流程输入连接解析 `图像/跟随`、在没有 ROI 时按当前图像尺寸补建默认 `Rect2`、最后调用 `findLineTool.ShowContour(true, false)` 做一次“显示 ROI 但不按跟随位姿平移”的预览。排查“打开窗体后看见的线与运行结果不一致”时，先看这条 `trans=false` 的预览链。
- 同一个 `Job.cs` 打开分支末尾还会回填界面控件，包括 `edgeSelect`、`minScore`、`polarity`、`cliperNum`、`threshold`、`Length`、`caliperWidth`、`ignoreNum` 和 `displayLine`。如果修改了 `FindLineTool` 字段却发现窗体打开后又被旧值覆盖，优先检查这里的回填逻辑。
- 跟随输入通过 `templatePose + InputPar.跟随` 做刚性变换。`FindLineTool.BuildExpectedLines()` 会把学习时的预期线变换到每个当前位姿，因此一个工具运行可能对多个跟随位姿各测一条线。
- `templatePose` 是 ROI 编辑时的匹配位姿基准，不得每次打开找线窗口都覆盖。打开窗口只通过 `EnsureTemplatePoseFromCurrentInput()` 补齐新工具的空基准；用户实际拖动或缩放 ROI 后，`SyncDisplayedRoi()` 才调用 `CaptureTemplatePoseFromCurrentInput()` 按当前匹配位置重新标定。
- 图片位姿变化后重新打开找线窗口，先调用 `RebaseRoiToCurrentFollowPose()`，用 `templatePose -> 当前跟随位姿` 的刚性变换同步更新 `ROIRectangle2` 的中心和角度，再把当前位姿设为新基准。随后 `ShowContour(true, false)` 显示的 ROI、卡尺和正式运行坐标保持一致，不允许只变换测量线而仍显示原始 ROI。
- 流程运行解析找线输入时，`ClearLastInput()` 只允许在遍历输入连接之前执行一次，不能在每个输入项循环内再次把 `InputPar.跟随` 清空；否则当“跟随”排在“图像”之前时，模板匹配位姿会被后一个输入覆盖为空，找线退回固定 ROI。
- HALCON 参数统一收口在 `FindLineTool.ApplyMetrologyParams()`：`measure_transition=polarity`、`num_measures=cliperNum`、`measure_length1=Length`、`measure_length2=caliperWidth`、`measure_threshold=threshold`、`measure_select=edgeSelect`、`min_score=minScore`。调找线稳定性优先改这里，不要散改 `AddMetrologyObjectLineMeasure()` 的常量。
- `FindLineTool.Run()` 在 `ignoreNum == 0` 时直接取 HALCON 返回的线结果；`ignoreNum > 0` 时会先按“点到初拟合线的距离”排序，剔除最远的若干点，再用 `FitLineAfterReject()` 重拟合。以后若要增强抗毛刺能力，优先从这段离群点剔除逻辑入手。
- 对外输出在 `FindLineTool.ToolPar.ResultPar`，当前只写一条 `线`；界面上的起点/终点文本框只是同步显示。若要支持多条线结果，不能只改 UI，需要同时扩展结果结构和下游连接逻辑。

查找圆工具注意：

- 核心文件是 `VisionAndMotion/1 ToolLib/10 FindCircleTool/FindCircleTool.cs`，界面文件是 `VisionAndMotion/1 ToolLib/10 FindCircleTool/Frm_FindCircleTool.cs`。源码里已经按“构造函数 -> `DrawExpectCircle()` -> `ShowContour()` -> `Run()` -> `Work()`”写了阅读顺序，后续定位优先沿这条链走。
- `Frm_FindCircleTool` 是单例窗体，但流程里可以存在多个查找圆工具。当前 `findCircleTool` 属性不会直接缓存唯一实例，而是优先用 `jobName/toolName` 通过 `GetBoundTool()` 反查当前绑定的工具对象；如果这个绑定逻辑被破坏，多开多个查找圆工具时很容易串参数。
- `FindCircleTool.L_regions[0]` 保存用户可编辑的预期圆 ROI，真正运行时会读取 `getModelData()` 得到 row、column、radius。`DrawExpectCircle()` 负责把该 ROI 放回窗口，并在有跟随输入时记录当前 `templatePose` 作为后续刚性变换基准。
- 查找圆和查找线一样支持跟随位姿，但这里只对圆心做刚性变换，半径保持不变。`ShowContour()` 与 `Run()` 都会根据 `templatePose -> InputPar.跟随` 生成 `newExpecCircleRow/newExpectCircleCol/newExpectCircleRadius`，因此一个工具可以对多个跟随位姿分别找圆。
- `Frm_FindCircleTool.Hwindow_MouseUp()` 仍是预期圆编辑后的同步点：用户拖动或缩放 ROI 后，要把 `smallestActiveROI()` 取回的当前圆写回 `regions/L_regions`。当前没有像查找线那样在 `MouseMove` 里实时同步，所以圆查找的预览刷新节奏仍以松手后为主。
- `VisionAndMotion/2 ClassLib/Job.cs` 的 `ToolType.FindCircle` 打开分支除了绑定 `Frm_FindCircleTool.jobName/toolName` 和 `Frm_FindCircleTool.findCircleTool` 之外，还会按流程输入连接重新解析 `图像/跟随`，然后先根据当前 `templatePose + 跟随` 算出“打开窗体时的预期圆心”，再把 `L_regions` 清空并用 `genCircle()` 重新生成一个当前位姿下的可编辑 ROI，最后调用 `findCircleTool.ShowContour(true, false)` 画预览。也就是说，查找圆在打开窗体时会主动重建 ROI，而不是直接复用内存里的旧 ROI。
- 同一打开分支在预览后会把当前跟随位姿重新写回 `templatePose`，再回填 UI 控件和图标状态：`displayCaliper/displayFeature/displayCircle/displayCircleCenter`、`edgeSelect`、`minScore`、`ringRadiusLength`、`threshold`、`cliperNum`、`polarity`、`caliperWidth`、`ignoreNum`。如果以后出现“打开窗体后圆位置跳了”或“切流程后参数串到别的查找圆工具”，先看这个分支有没有被改坏。
- `FindCircleTool.ShowContour()` 只做调参预览，不写流程输出。它会重建 Metrology 圆测量对象，显示蓝色卡尺、橙色特征点和绿色拟合圆，并根据 `trans` 决定是否应用跟随位姿。打开工具或只想查看原始 ROI 时，避免误用会改显示基准的调用路径。
- HALCON 核心调参在 `FindCircleTool.Run()` 和 `ShowContour()` 中通过 `SetMetrologyObjectParam()` 统一设置：`measure_transition=polarity`、`num_measures=cliperNum`、`measure_length1=ringRadiusLength`、`measure_length2=caliperWidth`、`measure_threshold=threshold`、`measure_select=edgeSelect`。`Run()` 额外启用了 `min_score=minScore`，但 `ShowContour()` 里目前保留了 `min_score` 注释未启用，调试预览和正式运行可能因此略有差异。
- `FindCircleTool.Work()` 是当前重写后的屏蔽区域主入口。它使用 `brush_region` 作为画笔、`final_region` 作为最终屏蔽区域，`radioButton2` 做 `Union2` 添加屏蔽，`radioButton3` 做 `Difference` 擦除屏蔽。后续如果看到“涂抹后需要重新学习”的提示或显示异常，优先从这里查，而不是先怀疑 `Run()`。
- `FindCircleTool.Run()` 运行时如果 `final_region` 有效，会先生成整幅图区域再做 `Difference(rec1, final_region)`，只在剩余区域内找边。也就是说屏蔽区不是后处理删除结果点，而是在 Metrology 前就缩小了参与测量的图像域。
- `ignoreNum == 0` 时，圆结果直接使用 HALCON 返回的 `all_param`；`ignoreNum > 0` 时，会先按点到初拟合圆轮廓的距离排序，剔除最远的若干点，再调用 `LeastSquaresFit()` 重拟合，并把保留点画成橙色、剔除点画成红色。这是当前抗离群点的主要策略。
- `FindCircleTool.Execute(ToolRunContext context)` 已经接入新的工具执行包装：`Job.Run()` 通过 `Execute()` 获取 `ToolRunResult`，统一上报成功、取消、超时和耗时。当前 `TimeoutMs` 还没有真正中断内部 `Run()`；如果以后要做可取消或超时退出，需要把 `Run()` 继续拆成可中断流程。
- `FindCircleTool.ResultPar` 现已明确包含 `圆心` 列表、`是否找到圆`、`结果圆`、`圆半径`。虽然内部支持对多个跟随位姿分别找到多个圆心，但对外主结果只回填第一个 `foundCircles[0]`。如果要支持多圆输出，必须扩展 `ResultPar` 和流程连线定义，不能只改 UI 文本框。
- 坐标含义沿用旧项目约定：`ResultPar.圆心` 的 `X` 实际存 row，`Y` 实际存 column。查找圆、下游定位和显示控件都建立在这个约定上，任何“纠正坐标名”的修改都要连带检查所有使用点。

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
| 修改主菜单/布局 | `Frm_Main.cs`、`Frm_LayoutManage.cs`。 |
| 修改公共标题栏按钮 | `Frm_FormBase.cs` 的 `AlignTitleButtons()`，不要逐窗体改 `button100.Location`。 |
| 修改语言/程序配置 | `Configuration.cs` 和设置页。 |

公司名称当前固定为“威乐普科技有限公司”：`Configuration.Read()` 忽略旧 `Configuration.ini` 中的示例名，`Project.LoadProject()` 在反序列化 `.pjt` 后也会再次覆盖，避免项目内嵌的历史名称恢复到标题栏。

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
- 当前 `Start/bin/Debug/Config` 仍是历史“手机组装”部署快照：`Configuration.ini` 的 `ProgramTitle=手机组装`，唯一项目文件为 `Config/Project/Vision/手机组装.pjt`，窗口名也是 Cover/HSG/螺丝流程。`LastProject.txt` 还指向另一台 Windows 主机的绝对路径；虽然加载逻辑会回退到最近 `.pjt`，但这套配置不能作为插件机方案直接上机。
- 当前部署快照 `FailStop=False`，连续流程中某轮 NG/异常后不会因此自动停止循环；插件机上线前必须根据 PLC 握手和不良处理时序明确失败策略，不能沿用该默认值。
- 手眼标定暂不可用于生产：`EyeHandCalibTool.Calibrate()` 的 `OutsideHand` 分支应读取表格第 1/2/3/4 列（像素X、像素Y、机械X、机械Y）并排除最后空行，当前却读取 0/1/2/3 列并遍历全部行；求矩阵失败后仍会继续分解旧矩阵，最后可能输出“标定成功”。
- 标定精度检查当前没有计算残差：`Frm_EyeHandCalibTool.button11_Click()` 保存数据后直接写入“标定完成，精度较高”。投产前必须增加独立验证点、像素到机械反投影残差、最大/平均误差阈值，并用实机走位确认 Row/Column 与 X/Y 方向。
- 一键手眼标定的 TCP `Connect/Receive` 没有超时、取消和完整状态机，且后台线程直接操作 WinForms 控件。未加入运动边界、急停/安全门硬件互锁验证前，不应在插件机上使用“一键标定”自动走位。
- `VisionAndMotion/2 ClassLib/PLCDevice.cs` 是旧编码/unknown-8bit 文件，编辑时避免整文件转码；尽量做小范围修改。
- 项目不是 Git 仓库根目录，当前工作区下 `git status` 不可用。
- 部分工具箱 case 中存在历史乱码/占位英文 case，修改工具名称时要谨慎，不要误删仍被旧项目引用的名称。
- `Job.cs` 很大，多个阶段都按 `ToolType` 分支处理。改某个工具时，要同时检查“打开工具窗体”和“运行工具”两个分支。
- `Job.Run()` 设计为后台线程执行（内部用 `Invoke`/`DoEvents` 回写 UI）。任何“运行流程/运行一次”入口都必须放后台线程，不能在按钮事件里同步调用，否则相机采图等阻塞 IO 会卡死 UI。仍以 UI 线程同步调用 `Run()` 的历史入口有 `Frm_OneDimensionalCalibTool.cs`、`Frm_EthernetReceiveTool.cs`、`Frm_AcqDevice.cs`，后续若复现卡死可一并改成后台线程。
## Recent Notes

- 2026-07-15: ImageAcq -> Match 只传递本次会话新采集/新选择的图像。项目加载后 `AcqImageTool.OnDeserialized()` 清空 `lastPreviewImagePath` 和 `ResultPar.图像`；打开采集或模板匹配窗口不再自动调用 `TryLoadLastPreviewImage()`。
- 2026-06-27: Flow-run image display for ImageAcq depends on `Job.Run()` calling `AcqImageTool.Run(true, false, toolName)`. Passing `updateImage=false` still updates outputs, but the main image window will not show the newly acquired image. If `useTemplateImageInRun` is enabled, `AcqImageTool.Run()` must also call `ShowImage(...)` in that branch.
