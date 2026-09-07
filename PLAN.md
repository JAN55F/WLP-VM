# 斑点分析“筛选类型”支持删除

## 问题与根因
视觉工具箱→斑点分析→主页面页签的筛选表 `dgv_selectItem`（筛选类型/下限/上限）：
- **增加正常**：`AllowUserToAddRows` 默认 true，在末行空白行选择筛选类型即自动追加新行，`CellValueChanged → SaveSelectItem()` 同步进 `L_select`。
- **无法删除**：该网格行头隐藏（`RowHeadersVisible=false`）、无 ContextMenuStrip、无删除按钮、CellSelect 模式选不中整行——删除入口完全缺失。
- 同窗体“区域处理”页签的 `dgv_processingItem` 已有成熟删除范式：右键菜单“删除”→ 删行 → 同步列表。本方案完全复用该范式。

## 改动内容（仅 2 个文件，均在 `VisionAndMotion/1 ToolLib/11 BlobAnalyseTool/`）

### 1. `Frm_BlobAnalyseTool.designer.cs`
- 新增字段：`ContextMenuStrip cms_deleteSelectItem` + `ToolStripMenuItem tsm_deleteSelectItem`（Text=“删除”，与区域处理菜单文案一致）。
- `InitializeComponent` 中（仿 `contextMenuStrip2` 写法）：实例化（挂 `this.components`）、`Items.AddRange`、`SuspendLayout/ResumeLayout`；给 `dgv_selectItem` 挂 `ContextMenuStrip = cms_deleteSelectItem`，并新增事件绑定：`tsm_deleteSelectItem.Click`、`dgv_selectItem.CellMouseDown`、`cms_deleteSelectItem.Opening`。

### 2. `Frm_BlobAnalyseTool.cs`（新 handler）
- 字段 `private int selectItemRightClickRowIndex = -1;` 记录右键命中的数据行。
- `dgv_selectItem_CellMouseDown`：右键且命中有效数据行（RowIndex≥0 且非新增占位行）→ 清除选择、选中该行、记录索引；否则索引复位 -1。
- `cms_deleteSelectItem_Opening`：索引无效或指向占位行 → `e.Cancel = true`（右键空白处不弹菜单）。
- `tsm_deleteSelectItem_Click`：索引守卫 → `dgv_selectItem.EndEdit()` → `Rows.RemoveAt(index)` → `blobAnalyseTool.SaveSelectItem()`（从网格全量重建 `L_select`，天然保持同步）→ 复位索引；try/catch `Log.SaveError`，风格随现有代码。

## 行为规格
- 右键筛选行→“删除”→该行从网格消失且 `L_select` 移除该项；关闭/重开工具窗体不复活（重开时从 `L_select` 重新加载）。
- 右键空白处或新增占位行不弹菜单；删除后占位行仍在，可继续自增添加。
- 允许删到 0 行（等价于无特征筛选，`Run()` 的 SelectShape 循环自然跳过）；构造器默认的 area 行同样可删。
- 删除后**不自动重跑**工具（点“运行工具”生效）——与区域处理删除行为一致。
- 新增自增、上下限编辑、区域处理删除等现有行为一概不动。

## 验证方案
1. **构建**：`E:/Visual Studio2022/MSBuild/Current/Bin/MSBuild.exe` 编译整个解决方案（VM Pro.exe 需未运行；遇 devenv 锁文件则重试）。
2. **驱动冒烟（Driver5.cs，Start/bin/Debug，csc 编译，沿用 Driver4 骨架）**：反射独立实例化 `Frm_BlobAnalyseTool`（不跑 VM.Init、不弹窗），置 `Job.loadForm=false`，向 `dgv_selectItem` 添加 2 行有效数据 → 反射设 `selectItemRightClickRowIndex=0` → 调用 `tsm_deleteSelectItem_Click` → 断言：网格剩 1 行、`L_select.Count==1` 且剩余项类型正确；再用占位行索引调用 → 断言无变化（no-op）；输出 PASS/FAIL 汇总。
3. **人工验收（你操作）**：打开斑点分析 → 右键筛选行删除 → 重开窗体确认不复活 → “运行工具”结果符合剩余筛选条件。
4. 测试后清理 Driver5 产物（本次驱动不启动完整程序，不写配置，无需备份还原）。
5. **提交**：olxolx 分支本地提交，消息：“斑点分析筛选类型支持右键删除”。

## 假设与默认
- 删除交互=右键菜单（你已确认）；不自动重跑（你已确认）。
- 菜单文案“删除”中文写死，与区域处理页签一致（该网格列头本就无多语言处理）。
- 允许删除全部筛选行，不做“至少保留一行”限制。
