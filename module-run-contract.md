# 模块化流程运行接口规范

本文档用于统一 VM Pro 流程模块的运行接口、输入输出流转、超时取消和 UI 边界，目标是避免流程运行时卡死在某一个工具或外设等待点。

## 1. 目标

模块化运行必须满足以下要求：

- 所有流程运行入口统一进入 `Job.RunAsync()` 或 `Job.RunAndWait()`。
- 所有工具模块对流程调度器表现为同一种运行契约。
- 所有外设等待、通讯等待、采图等待、运动等待必须有超时和取消出口。
- 工具内部只处理业务算法和外设访问，不直接控制流程树和主界面状态。
- 流程调度器统一负责输入准备、工具调用、输出提交、失败处理和日志提示。

## 2. 当前主要风险

当前代码里 `ToolBase.Run(bool updateImage, bool runTool, string toolName)` 返回 `void`，工具运行结果依赖 `toolRunStatu` 字段。这样会产生几个问题：

- 工具成功、失败、超时、取消没有统一返回对象。
- 每个工具自己决定什么时候返回，容易出现无限等待。
- `Job.Run()` 里按 `ToolType` 重复写输入解析、状态判断、输出写回，新增模块时容易遗漏某个步骤。
- 部分工具内部直接访问 UI，后台线程和 UI 线程边界不清晰。

高风险模块包括：

- 采集图像工具：相机同步采图、硬触发等待。
- PLC 通讯工具：等待寄存器值变为期望值。
- 以太网接收工具：等待远程触发命令。
- 串口、扫码枪、运动控制工具：等待设备响应或运动到位。

## 3. 运行入口规范

### 3.1 UI 入口

所有按钮、菜单、右键运行、工具窗体里的“运行流程”按钮，必须使用：

```csharp
Job.RunAndWait(jobName);
```

或：

```csharp
Job.RunAsync(jobName);
```

禁止在 UI 事件中直接调用：

```csharp
Job.FindJobByName(jobName).Run();
```

原因：`Job.Run()` 会执行相机、PLC、TCP 等阻塞式 IO。如果在 UI 线程直接调用，会卡住 WinForms 消息循环。

### 3.2 工作线程入口

以下场景可以直接调用 `job.Run()`：

- 连续运行线程 `Job.LoopRun()`。
- 自动化业务线程，例如 `Task_SmartLineA`。
- 标定工具内部明确处在后台线程并需要同步结果的流程。

前提是调用方本身不在 UI 线程，并且调用链不会等待 UI 线程同步返回。

## 4. 工具运行契约

### 4.1 新增统一上下文

建议新增：

```csharp
public class ToolRunContext
{
    public string JobName;
    public string ToolName;
    public int TimeoutMs;
    public Func<bool> IsCancellationRequested;
}
```

含义：

- `JobName`：当前流程名。
- `ToolName`：当前工具名。
- `TimeoutMs`：本次工具运行最大允许耗时。
- `IsCancellationRequested`：流程停止、用户取消、连续运行停止时返回 `true`。

### 4.2 新增统一返回结果

建议新增：

```csharp
public class ToolRunResult
{
    public bool Success;
    public bool Canceled;
    public bool Timeout;
    public ToolRunStatu Status;
    public string Message;
    public long ElapsedMs;
}
```

工具运行后必须返回明确结果：

- 成功：`Success = true`
- 失败：`Success = false`
- 超时：`Timeout = true`
- 取消：`Canceled = true`
- 状态：`Status = toolRunStatu`
- 文本：`Message` 写清楚失败原因

### 4.3 兼容旧工具

短期不需要一次性重写所有工具。可以先在 `ToolBase` 里增加包装方法：

```csharp
public virtual ToolRunResult Execute(ToolRunContext context)
{
    Stopwatch sw = Stopwatch.StartNew();

    Run(true, false, context.ToolName);

    sw.Stop();

    return new ToolRunResult
    {
        Success = IsSuccessStatus(toolRunStatu),
        Status = toolRunStatu,
        Message = toolRunStatu.ToString(),
        ElapsedMs = sw.ElapsedMilliseconds
    };
}
```

旧工具继续保留 `Run()`，新工具优先实现 `Execute()`。

## 5. 超时和取消规范

### 5.1 所有等待必须有出口

禁止在工具运行路径中出现没有超时的等待：

```csharp
while (true)
{
    ...
}
```

禁止在等待外部条件时只靠人工置位退出：

```csharp
while (!condition)
{
    Thread.Sleep(10);
}
```

必须改为：

```csharp
Stopwatch sw = Stopwatch.StartNew();

while (!condition)
{
    if (context.IsCancellationRequested != null && context.IsCancellationRequested())
    {
        return new ToolRunResult
        {
            Success = false,
            Canceled = true,
            Status = ToolRunStatu.未知原因,
            Message = "用户取消运行"
        };
    }

    if (sw.ElapsedMilliseconds > context.TimeoutMs)
    {
        return new ToolRunResult
        {
            Success = false,
            Timeout = true,
            Status = ToolRunStatu.未知原因,
            Message = "工具运行超时"
        };
    }

    Thread.Sleep(10);
}
```

### 5.2 推荐默认超时

建议先统一以下默认值：

- 普通视觉算法：3000 ms
- 相机单帧采图：5000 ms
- 硬触发等待：5000 ms，可配置
- PLC 读写：3000 ms
- PLC 等待期望值：5000 ms，可配置
- TCP/串口接收：5000 ms
- 运动到位：10000 ms，可按轴或动作配置

超时值应放在配置里，不能硬编码散落在各工具中。

### 5.3 外设 SDK 调用

如果 SDK 自身支持超时参数，必须优先使用 SDK 超时。

如果 SDK 不支持中断，至少要在调用前后记录日志，并避免在 UI 线程调用。

相机、PLC、TCP 这类模块必须在工具配置界面显示当前超时参数。

## 6. 输入输出规范

### 6.1 输入准备

工具运行前，由流程调度器统一完成输入准备：

```csharp
PrepareInputs(toolInfo);
```

职责：

- 检查输入项是否连接。
- 解析 `工具名->输出项` 来源。
- 从上游工具输出取值。
- 做基础类型检查。
- 写入工具的 `InputPar` 或等价输入结构。

工具内部不要再重复解析流程树字符串。

### 6.2 输出提交

工具运行成功后，由流程调度器统一提交输出：

```csharp
CommitOutputs(toolInfo);
```

职责：

- 从工具 `ResultPar` 或等价结果结构取值。
- 写入 `ToolInfo.output`。
- 更新输出节点 tooltip。

工具内部不要直接写流程树节点状态。

### 6.3 输入缺失处理

输入源未连接、上游输出为空、类型不匹配时，流程调度器应统一返回失败：

```csharp
ToolRunStatu.输入项未链接源
ToolRunStatu.无输入图像
ToolRunStatu.无输入字符串
ToolRunStatu.未知原因
```

不要让工具在空输入下继续运行。

### 6.4 运行前状态预热

如果工具运行依赖某个配置态资源已经准备好，必须在配置变更时或反序列化后完成预热，不能把这一步留到流程首次运行时临时执行。

典型需要预热的资源包括：

- 模板图片文件。
- 标定文件。
- 设备句柄或通道映射缓存。
- 运行时要直接消费的大对象缓存。

禁止把下面这类动作放在流程运行主路径中作为首次初始化：

- 第一次运行时再读模板文件。
- 第一次运行时再把配置路径转换成内存对象。
- 第一次运行时再建立运行必需的对象绑定。

推荐模式：

```csharp
internal bool PrimeRuntimeCache(bool reportError)
{
    if (cache != null && cacheKey == currentKey)
        return true;

    cache = LoadFromConfig(currentKey);
    cacheKey = currentKey;
    return true;
}
```

然后在以下时机调用：

- 配置值变更后。
- 工具对象反序列化完成后。
- 切换当前编辑工具实例后。

流程运行时只消费已经准备好的缓存对象；如果缓存缺失，应立即失败并返回明确状态，不要在运行线程里临时做重初始化。

## 7. UI 边界规范

### 7.1 工具运行层

工具运行代码可以做：

- 算法计算。
- 外设读写。
- 读写自己的参数对象。
- 写运行状态和结果。

工具运行代码不应做：

- 直接操作流程树节点。
- 直接启用或禁用主界面按钮。
- 直接 `ShowDialog()`。
- 在后台线程里直接改 WinForms 控件。

### 7.2 UI 更新

需要更新 UI 时必须：

- 判断窗体实例是否已存在。
- 使用 `BeginInvoke()` 投递回 UI 线程。
- 不因为流程运行而强制创建工具编辑窗体。

推荐模式：

```csharp
if (form != null && form.IsHandleCreated)
{
    form.BeginInvoke(new MethodInvoker(delegate
    {
        // 更新控件
    }));
}
```

### 7.3 工具实例绑定

共享编辑窗体中的 UI 操作，必须始终作用于当前流程、当前工具真正绑定的工具实例，不能长期依赖某个静态字段缓存的旧实例。

高风险写法：

```csharp
static AcqImageTool imageAcqTool = new AcqImageTool();
imageAcqTool.useTemplateImageInRun = ckb_useTemplateImage.Checked;
```

问题：

- 用户在窗体里修改的是静态缓存对象。
- 流程运行时执行的可能是 `Job` 中另一个反序列化出来的真实工具对象。
- 表现为“必须先手动运行一次工具，流程实例才同步正确”，或者“界面上改了参数但流程里没生效”。

推荐模式：

```csharp
private ToolType GetBoundTool()
{
    return (ToolType)Job.FindToolByName(jobName, toolName);
}
```

然后所有按钮、复选框、参数输入事件，都通过 `GetBoundTool()` 获取当前真实工具实例后再修改。

结论：

- UI 只负责编辑当前绑定实例。
- 流程运行只消费当前绑定实例。
- 不允许依靠“先手动运行一次工具”来补齐实例同步。

## 8. Job.Run 调度规范

`Job.Run()` 的目标结构应逐步收口为：

```csharp
foreach (ToolInfo toolInfo in L_toolList)
{
    if (!PrepareTool(toolInfo))
        break;

    if (!PrepareInputs(toolInfo))
        break;

    ToolRunResult result = ExecuteTool(toolInfo);

    if (!HandleToolResult(toolInfo, result))
        break;

    CommitOutputs(toolInfo);
}
```

职责划分：

- `PrepareTool()`：检查启用、初始化状态、设置节点颜色。
- `PrepareInputs()`：统一输入解析。
- `ExecuteTool()`：调用工具运行契约。
- `HandleToolResult()`：统一处理成功、失败、取消、超时。
- `CommitOutputs()`：统一输出写回。

## 9. 日志规范

每个工具运行至少记录：

- 流程名。
- 工具名。
- 工具类型。
- 开始时间。
- 结束时间。
- 耗时。
- 结果状态。
- 失败原因。
- 超时值。

推荐输出格式：

```text
[JobRun] Job=流程1 Tool=采集图像1 Type=ImageAcq Result=Fail Status=采集图像时出错 Elapsed=5021ms Timeout=5000ms Message=GrabOneImage timeout
```

卡死排查时，必须能从日志看到最后进入了哪个工具、停在什么等待点。

## 10. 迁移优先级

### 第一阶段：止血

优先修改会无限等待的工具：

1. `PLCCommTool`
2. `EthernetReceiveTool`
3. `AcqImageTool`
4. 串口和扫码枪工具
5. 运动控制相关工具

要求：

- 每个等待循环加超时。
- 每个等待循环支持取消。
- 超时后返回失败，不继续卡住流程。

### 第二阶段：接口收口

修改 `ToolBase`：

- 增加 `ToolRunContext`。
- 增加 `ToolRunResult`。
- 增加 `Execute()` 包装旧 `Run()`。

修改 `Job.Run()`：

- 先通过 `ExecuteTool()` 调用工具。
- 统一判断 `ToolRunResult`。

### 第三阶段：输入输出收口

逐步把 `Job.Run()` 里每个 `ToolType` 分支中的输入解析、输出写回抽成公共方法。

优先处理结构稳定的工具：

- 图像预处理。
- 色彩转换。
- 保存图像。
- 模板匹配。
- PLC 通讯。
- 以太网收发。

### 第四阶段：新工具强制规范

新增工具必须满足：

- 不直接新增裸 `Run()` 调用入口。
- 必须实现或继承 `Execute()`。
- 必须定义输入项、输出项和类型。
- 所有等待必须有超时。
- 不在运行代码里直接操作 UI。

## 11. 新增工具检查清单

新增或改造工具前，逐项确认：

- 是否通过 `Job.RunAsync()` 或 `Job.RunAndWait()` 触发流程。
- 是否有统一运行结果。
- 是否有最大运行时间。
- 是否支持用户取消或流程停止。
- 输入为空时是否立即失败。
- 输出是否只在成功后提交。
- 运行前依赖的缓存或资源是否已预热，而不是首次运行时临时初始化。
- 共享窗体里的参数修改是否一定作用于当前真实工具实例，而不是静态旧实例。
- 是否避免在运行代码里直接改 UI。
- 是否写入足够日志。
- 是否不会在启动阶段预跑流程。

## 12. 推荐落地顺序

建议实际执行顺序：

1. 给 `ToolRunStatu` 增加 `运行超时`、`用户取消` 两个状态。
2. 增加全局默认工具超时配置。
3. 先改 `PLCCommTool` 和 `EthernetReceiveTool` 的无限等待。
4. 确认 `AcqImageTool` 普通采图也有可靠超时。
5. 在 `ToolBase` 增加 `Execute()` 兼容层。
6. 在 `Job.Run()` 里增加统一 `ExecuteTool()` 包装点。
7. 逐步抽离输入准备和输出提交。

这套顺序可以避免一次性大改导致旧项目文件不兼容，同时能先解决最影响现场运行的卡死问题。
