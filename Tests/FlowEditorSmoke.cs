using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

internal static class FlowEditorSmoke
{
    private static int assertionCount;

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Usage: FlowEditorSmoke <CVMPro.dll> <preview.png>");
            return 2;
        }

        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Control.CheckForIllegalCrossThreadCalls = true;

            Assembly assembly = Assembly.LoadFrom(args[0]);
            VerifyEmptyHalconImageGuard(assembly);
            VerifyWorkflowWindow(assembly, args[1]);
            VerifyLoadedWorkflowRebuild(assembly);
            VerifyAndRenderEditor(assembly, args[1]);
            VerifyTitleChrome(assembly, args[1]);
            Console.WriteLine("Flow editor smoke checks passed: {0} assertions.", assertionCount);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void VerifyEmptyHalconImageGuard(Assembly assembly)
    {
        Type toolBaseType = assembly.GetType("VMPro.ToolBase", true);
        MethodInfo guard = toolBaseType.GetMethod("TryGetHalconImageSize",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo postControlAction = toolBaseType.GetMethod("TryPostControlAction",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Type blobToolType = assembly.GetType("VMPro.BlobAnalyseTool", true);
        MethodInfo queueBlobDisplay = blobToolType.GetMethod("QueueRuntimeDisplay",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Type blobFormType = assembly.GetType("VMPro.Frm_BlobAnalyseTool", true);
        PropertyInfo currentBlobForm = blobFormType.GetProperty("CurrentInstance",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(guard != null, "HALCON image-size calls do not have a shared empty-image guard.");
        Assert(postControlAction != null,
            "Background tool callbacks do not have a shared protected UI dispatcher.");
        Assert(queueBlobDisplay != null && currentBlobForm != null,
            "Blob runtime display still depends on direct or implicitly-created UI access.");

        using (Control dispatcher = new Control())
        {
            dispatcher.CreateControl();
            bool callbackReached = false;
            Action failingCallback = delegate
            {
                callbackReached = true;
                throw new InvalidOperationException("expected smoke callback failure");
            };
            bool posted = (bool)postControlAction.Invoke(null,
                new object[] { dispatcher, failingCallback });
            Assert(posted && callbackReached,
                "Protected UI dispatcher did not execute and absorb a callback failure.");
        }

        Type hObjectType = guard.GetParameters()[0].ParameterType;
        object emptyImage = Activator.CreateInstance(hObjectType);
        try
        {
            object[] parameters = { emptyImage, null, null };
            bool valid = (bool)guard.Invoke(null, parameters);
            Assert(!valid, "A non-null HObject with object ID 0 was accepted as a valid image.");
        }
        finally
        {
            IDisposable disposable = emptyImage as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        Type findCircleType = assembly.GetType("VMPro.FindCircleTool", true);
        ConstructorInfo safeFallbackConstructor = findCircleType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, new[] { typeof(bool) }, null);
        Assert(safeFallbackConstructor != null,
            "Find-circle form fallback still requires the normal image-reading constructor.");
    }

    private static void VerifyLoadedWorkflowRebuild(Assembly assembly)
    {
        Type jobType = assembly.GetType("VMPro.Job", true);
        Type toolInfoType = assembly.GetType("VMPro.ToolInfo", true);
        Type toolIoType = assembly.GetType("VMPro.ToolIO", true);
        Type dataType = assembly.GetType("VMPro.DataType", true);
        MethodInfo rebuild = jobType.GetMethod("RebuildLoadedWorkflowTree",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo legacyExpandBlocker = jobType.GetMethod("tvw_job_BeforeExpand",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        PropertyInfo executionActive = jobType.GetProperty("IsExecutionActive",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(rebuild != null && executionActive != null && legacyExpandBlocker == null,
            "Loaded workflows still depend on the legacy expand blocker or lack a rebuild entry point.");

        object job = Activator.CreateInstance(jobType);
        SetField(jobType, job, "jobName", "旧流程兼容测试");
        SetField(jobType, job, "isRunLoop", true);
        SetField(jobType, job, "m_MouseClicks", 2);

        FieldInfo toolListField = jobType.GetField("L_toolList",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo toolNameField = toolInfoType.GetField("toolName",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo toolInputField = toolInfoType.GetField("input",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo toolOutputField = toolInfoType.GetField("output",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        ConstructorInfo ioConstructor = toolIoType.GetConstructor(
            new[] { typeof(string), typeof(object), dataType });
        object imageType = Enum.Parse(dataType, "Image");
        IList tools = (IList)toolListField.GetValue(job);

        object target = Activator.CreateInstance(toolInfoType);
        toolNameField.SetValue(target, "前置目标");
        ((IList)toolInputField.GetValue(target)).Add(ioConstructor.Invoke(
            new object[] { "图像", "《- 后置源->输出图像", imageType }));
        ((IList)toolInputField.GetValue(target)).Add(ioConstructor.Invoke(
            new object[] { "坏连接", "《- 缺少箭头", imageType }));
        tools.Add(target);

        object source = Activator.CreateInstance(toolInfoType);
        toolNameField.SetValue(source, "后置源");
        ((IList)toolOutputField.GetValue(source)).Add(ioConstructor.Invoke(
            new object[] { "输出图像", string.Empty, imageType }));
        tools.Add(source);

        using (Form host = new Form())
        using (TreeView tree = new TreeView())
        {
            host.Controls.Add(tree);
            int unresolved = (int)rebuild.Invoke(job, new object[] { tree });
            FieldInfo connectionsField = jobType.GetField("D_itemAndSource",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            IDictionary connections = (IDictionary)connectionsField.GetValue(job);
            Assert(tree.Nodes.Count == 2 && connections.Count == 1 && unresolved == 1,
                "Two-pass workflow rebuild did not restore a later source or isolate malformed input.");

            DictionaryEntry connection = connections.Cast<DictionaryEntry>().Single();
            TreeNode targetNode = (TreeNode)connection.Key;
            TreeNode sourceNode = (TreeNode)connection.Value;
            Assert(targetNode.Parent.Text == "前置目标" && sourceNode.Parent.Text == "后置源" &&
                   sourceNode.Text == "-->输出图像",
                "Restored connection points to the wrong module or port.");
            Assert(tree.Nodes[0].Nodes[1].ToolTipText.Contains("无法识别"),
                "Malformed old connection was not surfaced on its own input node.");

            tree.Dock = DockStyle.Fill;
            host.Show();
            tree.Nodes[0].Expand();
            tree.Nodes[1].Expand();
            Application.DoEvents();
            Assert(tree.Nodes[0].IsExpanded && tree.Nodes[1].IsExpanded,
                "A rebuilt default workflow cannot expand after first display.");
            host.Hide();
        }

        Assert(!(bool)executionActive.GetValue(job, null) &&
               !(bool)GetFieldValue(jobType, job, "isRunLoop") &&
               (int)GetFieldValue(jobType, job, "m_MouseClicks") == 0,
            "Serialized continuous-run or mouse state survived workflow reconstruction.");
    }

    private static void VerifyWorkflowWindow(Assembly assembly, string previewPath)
    {
        Type formType = assembly.GetType("VMPro.Frm_Job", true);
        using (Form form = (Form)Activator.CreateInstance(formType, true))
        {
            form.ClientSize = new Size(460, 600);
            form.CreateControl();
            form.PerformLayout();

            Control commandBar = form.Controls.Find("modernEditorCommandBar", true).FirstOrDefault();
            TableLayoutPanel commandLayout = commandBar as TableLayoutPanel;
            Assert(commandLayout != null && commandBar.Dock == DockStyle.Fill &&
                   commandLayout.ColumnCount == 1 && commandLayout.RowCount == 1,
                "Workflow command bar must use the compact management-only layout.");

            TabControl tabs = GetField<TabControl>(formType, form, "tbc_jobs");
            TableLayoutPanel selectorHost = form.Controls.Find("modernWorkflowSelectorHost", true)
                .OfType<TableLayoutPanel>().FirstOrDefault();
            ComboBox workflowSelector = form.Controls.Find("modernWorkflowSelector", true)
                .OfType<ComboBox>().FirstOrDefault();
            Assert(selectorHost != null && workflowSelector != null &&
                   selectorHost.Dock == DockStyle.Top && selectorHost.Height <= 48,
                "Workflow selector is missing or consumes too much editor space.");
            Assert(tabs.Appearance == TabAppearance.FlatButtons && tabs.ItemSize.Height <= 2 && !tabs.Multiline,
                "Legacy multiline workflow tabs are still visible.");

            ToolStrip strip = GetField<ToolStrip>(formType, form, "toolStrip1");
            Assert(strip.Items.Count == 6 && strip.Items.Cast<ToolStripItem>().All(item =>
                item.Image != null && ImageHasVisibleInk(item.Image)),
                "Workflow commands must use six visible runtime vector icons.");
            ToolStripButton showAllConnections = strip.Items
                .OfType<ToolStripButton>()
                .FirstOrDefault(item => item.Name == "modernShowAllConnectionsButton");
            Assert(showAllConnections != null && showAllConnections.CheckOnClick &&
                   showAllConnections.ToolTipText.Contains("输入/输出端口"),
                "Workflow editor is missing the all-connections/focused-connections toggle.");

            Button runOnce = GetField<Button>(formType, form, "btn_runOnce");
            Button runLoop = GetField<Button>(formType, form, "btn_runLoop");
            Assert(!runOnce.Visible && !runLoop.Visible &&
                   !commandLayout.Controls.Contains(runOnce) && !commandLayout.Controls.Contains(runLoop),
                "Single-run and continuous-run actions must not appear in the workflow editor.");

            Type editorType = assembly.GetType("VMPro.FlowEditorTreeView", true);
            TreeView previewEditor = (TreeView)Activator.CreateInstance(editorType, true);
            previewEditor.Dock = DockStyle.Fill;
            TreeNode camera = previewEditor.Nodes.Add("采集图像");
            camera.ImageIndex = 1;
            camera.Nodes.Add("-->输出图像");
            TreeNode match = previewEditor.Nodes.Add("定位检测");
            match.ImageIndex = 10;
            match.Nodes.Add("<--输入图像《- 采集图像->输出图像");
            previewEditor.ExpandAll();
            tabs.TabPages.Add("定位检测流程");
            tabs.TabPages[0].Controls.Add(previewEditor);
            tabs.SelectedIndex = 0;
            form.Show();
            Application.DoEvents();
            ToolStripItem expandWorkflow = strip.Items
                .Cast<ToolStripItem>().First(item => item.Name == "tsb_expandJob");
            ToolStripItem deleteWorkflow = strip.Items
                .Cast<ToolStripItem>().First(item => item.Name == "tsb_deleteJob");
            Assert(expandWorkflow.Enabled && deleteWorkflow.Enabled && workflowSelector.Enabled &&
                   tabs.SelectedTab != null && tabs.SelectedIndex == 0,
                "The initial workflow did not activate its expand/delete commands before switching workflows.");
            tabs.TabPages.Add("复检流程");
            Application.DoEvents();
            Assert(workflowSelector.Enabled && workflowSelector.Items.Count == 2 &&
                   workflowSelector.SelectedIndex == tabs.SelectedIndex,
                "Compact workflow selector did not synchronize added workflows.");
            workflowSelector.SelectedIndex = 1;
            Application.DoEvents();
            Assert(tabs.SelectedIndex == 1,
                "Compact workflow selector did not switch the workflow content.");
            workflowSelector.SelectedIndex = 0;
            tabs.TabPages[1].Text = "复检流程（更新）";
            Application.DoEvents();
            Assert(workflowSelector.Items[1].ToString() == "复检流程（更新）",
                "Compact workflow selector did not synchronize a renamed workflow.");
            tabs.TabPages.RemoveAt(1);
            Application.DoEvents();
            workflowSelector.DroppedDown = true;
            Application.DoEvents();
            workflowSelector.DroppedDown = false;
            Assert(workflowSelector.Items.Count == 1 && workflowSelector.SelectedIndex == 0,
                "Compact workflow selector did not synchronize a removed workflow.");
            MethodInfo syncSelector = formType.GetMethod("SyncWorkflowSelector",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            int unchangedSelectionEvents = 0;
            EventHandler unchangedSelectionHandler = delegate { unchangedSelectionEvents++; };
            workflowSelector.SelectedIndexChanged += unchangedSelectionHandler;
            syncSelector.Invoke(form, null);
            workflowSelector.SelectedIndexChanged -= unchangedSelectionHandler;
            Assert(unchangedSelectionEvents == 0,
                "Opening Vision rebuilt an unchanged workflow selector and queued avoidable UI work.");

            MethodInfo visibleChanged = formType.GetMethod("FrmJob_VisibleChanged",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            visibleChanged.Invoke(form, new object[] { form, EventArgs.Empty });
            Assert(!(bool)GetFieldValue(formType, form, "workflowUiRefreshPending"),
                "Opening Vision still schedules a duplicate full workflow refresh.");
            foreach (ToolStripItem item in strip.Items)
                item.Enabled = true;
            Panel commandPanel = GetField<Panel>(formType, form, "panel1");
            Assert(tabs.Top >= selectorHost.Bottom - 2 &&
                   tabs.Bottom <= commandPanel.Top &&
                   tabs.Height >= form.ClientSize.Height - commandPanel.Height - selectorHost.Height - 4,
                "Workflow content did not receive the reclaimed tab-header space: form=" +
                form.ClientSize + ", selector=" + selectorHost.Bounds + ", tabs=" + tabs.Bounds +
                ", commandPanel=" + commandPanel.Bounds + ".");
            string directory = Path.GetDirectoryName(previewPath);
            if (!String.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            string windowPreview = Path.Combine(directory ?? String.Empty, "flow-editor-window-preview.png");
            using (Bitmap bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
                bitmap.Save(windowPreview, ImageFormat.Png);
            }
            form.Hide();
        }

        Type jobType = assembly.GetType("VMPro.Job", true);
        MethodInfo moveToolNode = jobType.GetMethod("TryMoveToolNode",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo resolveModuleNode = jobType.GetMethod("GetModuleNodeAt",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo toggleToolEnabled = jobType.GetMethod("TryToggleToolEnabled",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo removeWorkflowNode = jobType.GetMethod("RemoveWorkflowNode",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(moveToolNode != null && resolveModuleNode != null &&
               toggleToolEnabled != null && removeWorkflowNode != null,
            "Workflow module interaction entry points are incomplete.");
        object job = Activator.CreateInstance(jobType);
        FieldInfo toolListField = jobType.GetField("L_toolList",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Type toolInfoType = assembly.GetType("VMPro.ToolInfo", true);
        FieldInfo toolNameField = toolInfoType.GetField("toolName",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo toolEnabledField = toolInfoType.GetField("enable",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo toolInputField = toolInfoType.GetField("input",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo toolOutputField = toolInfoType.GetField("output",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        IList toolList = (IList)toolListField.GetValue(job);
        foreach (string toolName in new[] { "第一模块", "第二模块", "第三模块" })
        {
            object toolInfo = Activator.CreateInstance(toolInfoType);
            toolNameField.SetValue(toolInfo, toolName);
            toolList.Add(toolInfo);
        }
        using (TreeView reorderTree = new TreeView())
        {
            TreeNode first = reorderTree.Nodes.Add("第一模块");
            first.Nodes.Add("<--输入");
            TreeNode second = reorderTree.Nodes.Add("第二模块");
            TreeNode secondPort = second.Nodes.Add("-->输出");
            TreeNode third = reorderTree.Nodes.Add("第三模块");

            bool movedToFirst = (bool)moveToolNode.Invoke(job,
                new object[] { reorderTree, third, first });
            Assert(movedToFirst && reorderTree.Nodes.Cast<TreeNode>().Select(node => node.Text)
                       .SequenceEqual(new[] { "第三模块", "第一模块", "第二模块" }) &&
                   toolList.Cast<object>().Select(item => (string)toolNameField.GetValue(item))
                       .SequenceEqual(new[] { "第三模块", "第一模块", "第二模块" }),
                "Dragging a module to the first position produced an invalid order.");

            bool movedAfterTool = (bool)moveToolNode.Invoke(job,
                new object[] { reorderTree, third, secondPort });
            Assert(movedAfterTool && reorderTree.Nodes.Cast<TreeNode>().Select(node => node.Text)
                       .SequenceEqual(new[] { "第一模块", "第二模块", "第三模块" }) &&
                   toolList.Cast<object>().Select(item => (string)toolNameField.GetValue(item))
                       .SequenceEqual(new[] { "第一模块", "第二模块", "第三模块" }),
                "Dropping on a tool port did not place the module safely after its owner.");

            bool movedToEnd = (bool)moveToolNode.Invoke(job,
                new object[] { reorderTree, first, null });
            Assert(movedToEnd && reorderTree.Nodes.Cast<TreeNode>().Select(node => node.Text)
                       .SequenceEqual(new[] { "第二模块", "第三模块", "第一模块" }) &&
                   toolList.Cast<object>().Select(item => (string)toolNameField.GetValue(item))
                       .SequenceEqual(new[] { "第二模块", "第三模块", "第一模块" }),
                "Dragging a module to blank space did not append it safely.");

            using (Form interactionHost = new Form())
            {
                interactionHost.ClientSize = new Size(360, 300);
                reorderTree.Dock = DockStyle.Fill;
                interactionHost.Controls.Add(reorderTree);
                reorderTree.ExpandAll();
                interactionHost.Show();
                Application.DoEvents();

                Point outputPoint = new Point(
                    Math.Max(1, secondPort.Bounds.Left + 4),
                    secondPort.Bounds.Top + Math.Max(1, secondPort.Bounds.Height / 2));
                TreeNode resolved = resolveModuleNode.Invoke(job,
                    new object[] { reorderTree, outputPoint.X, outputPoint.Y }) as TreeNode;
                Assert(resolved == second,
                    "Double-clicking a module port did not resolve to its owning module.");

                object[] toggleArguments = { second, null };
                bool toggled = (bool)toggleToolEnabled.Invoke(job, toggleArguments);
                object secondToolInfo = toolList.Cast<object>()
                    .First(item => (string)toolNameField.GetValue(item) == "第二模块");
                Assert(toggled && !(bool)toggleArguments[1] &&
                       !(bool)toolEnabledField.GetValue(secondToolInfo) &&
                       second.ForeColor == Color.DarkGray && reorderTree.SelectedNode == second,
                    "Disabling one module did not update both model and selected workflow card.");

                object[] enableArguments = { second, null };
                toggleToolEnabled.Invoke(job, enableArguments);
                Assert((bool)enableArguments[1] && (bool)toolEnabledField.GetValue(secondToolInfo),
                    "A disabled module could not be enabled again.");

                Type toolIoType = assembly.GetType("VMPro.ToolIO", true);
                Type dataType = assembly.GetType("VMPro.DataType", true);
                object imageType = Enum.Parse(dataType, "Image");
                ConstructorInfo ioConstructor = toolIoType.GetConstructor(
                    new[] { typeof(string), typeof(object), dataType });
                object thirdToolInfo = toolList.Cast<object>()
                    .First(item => (string)toolNameField.GetValue(item) == "第三模块");
                IList outputs = (IList)toolOutputField.GetValue(secondToolInfo);
                IList inputs = (IList)toolInputField.GetValue(thirdToolInfo);
                outputs.Add(ioConstructor.Invoke(new[] { "输出", string.Empty, imageType }));
                object connectedInput = ioConstructor.Invoke(
                    new object[] { "输入", "《- 第二模块->输出", imageType });
                inputs.Add(connectedInput);
                TreeNode thirdInput = third.Nodes.Add("<--输入《- 第二模块->输出");
                FieldInfo connectionsField = jobType.GetField("D_itemAndSource",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                IDictionary connections = (IDictionary)connectionsField.GetValue(job);
                connections.Add(thirdInput, secondPort);

                bool removed = (bool)removeWorkflowNode.Invoke(job, new object[] { second });
                FieldInfo ioValueField = toolIoType.GetField("value",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Assert(removed && !reorderTree.Nodes.Contains(second) &&
                       !toolList.Cast<object>().Any(item =>
                           (string)toolNameField.GetValue(item) == "第二模块") &&
                       connections.Count == 0 &&
                       string.Empty.Equals(ioValueField.GetValue(connectedInput)) &&
                       thirdInput.Text == "<--输入",
                    "Deleting one module did not remove its model, connection, and downstream source.");

                interactionHost.Hide();
            }
        }
    }

    private static void VerifyTitleChrome(Assembly assembly, string previewPath)
    {
        Type baseFormType = assembly.GetType("VMPro.Frm_FormBase", true);
        using (Form form = (Form)Activator.CreateInstance(baseFormType, true))
        {
            form.ClientSize = new Size(520, 320);
            form.Show();
            Application.DoEvents();

            Button pin = GetField<Button>(baseFormType, form, "button100");
            Button minimize = GetField<Button>(baseFormType, form, "button1");
            Button maximize = GetField<Button>(baseFormType, form, "button2");
            Button close = GetField<Button>(baseFormType, form, "btn_baseClose");
            Button[] buttons = { pin, minimize, maximize, close };
            Assert(buttons.All(button => button.Width >= 34 &&
                   string.IsNullOrEmpty(button.Text) && button.Image != null &&
                   ImageHasVisibleInk(button.Image)),
                "Tool-window title buttons are not using the enlarged vector glyphs.");
            Assert(close.FlatAppearance.MouseOverBackColor != minimize.FlatAppearance.MouseOverBackColor,
                "Close button is missing its distinct danger hover state.");

            Image maximizeImage = maximize.Image;
            maximize.PerformClick();
            Application.DoEvents();
            Assert(form.WindowState == FormWindowState.Maximized && maximize.Image != maximizeImage,
                "Maximize button did not switch to the restore glyph.");
            maximize.PerformClick();
            Application.DoEvents();

            string directory = Path.GetDirectoryName(previewPath) ?? string.Empty;
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            string chromePreview = Path.Combine(directory, "tool-window-title-preview.png");
            using (Bitmap bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
                bitmap.Save(chromePreview, ImageFormat.Png);
            }
            form.Hide();
        }
    }

    private static void VerifyAndRenderEditor(Assembly assembly, string previewPath)
    {
        Type editorType = assembly.GetType("VMPro.FlowEditorTreeView", true);
        MethodInfo setProvider = editorType.GetMethod("SetConnectionProvider",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo renderConnections = editorType.GetMethod("RenderConnections",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, new[] { typeof(Graphics) }, null);
        MethodInfo requestRefresh = editorType.GetMethod("RequestConnectionRefresh",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo resolveVisibleEndpoint = editorType.GetMethod("ResolveVisibleConnectionEndpoint",
            BindingFlags.Static | BindingFlags.NonPublic);
        PropertyInfo renderedCount = editorType.GetProperty("LastRenderedConnectionCount",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        PropertyInfo gutterWidth = editorType.GetProperty("ConnectionGutterWidth",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        PropertyInfo portCenterX = editorType.GetProperty("ConnectionPortCenterX",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        PropertyInfo showAllConnections = editorType.GetProperty("ShowAllConnections",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert(setProvider != null && renderConnections != null && requestRefresh != null &&
               resolveVisibleEndpoint != null &&
               renderedCount != null && gutterWidth != null && portCenterX != null &&
               showAllConnections != null,
            "Flow editor rendering entry points are incomplete.");

        using (Form host = new Form())
        using (TreeView editor = (TreeView)Activator.CreateInstance(editorType, true))
        {
            host.ClientSize = new Size(430, 510);
            editor.Dock = DockStyle.Fill;
            host.Controls.Add(editor);

            TreeNode acquire = editor.Nodes.Add("采集图像");
            acquire.ImageIndex = 1;
            acquire.ForeColor = Color.Green;
            acquire.Nodes.Add("<--触发模式");
            TreeNode imageOutput = acquire.Nodes.Add("-->输出图像");
            TreeNode qualityOutput = acquire.Nodes.Add("-->质量标记");

            TreeNode locate = editor.Nodes.Add("形状匹配");
            locate.ImageIndex = 10;
            TreeNode locateInput = locate.Nodes.Add("<--输入图像《- 采集图像->输出图像");
            TreeNode qualityInput = locate.Nodes.Add("<--质量标记《- 采集图像->质量标记");
            TreeNode poseOutput = locate.Nodes.Add("-->输出位置");

            TreeNode send = editor.Nodes.Add("结果输出");
            send.ImageIndex = 30;
            TreeNode sendInput = send.Nodes.Add("<--输入位置《- 形状匹配->输出位置");

            KeyValuePair<TreeNode, TreeNode>[] connections =
            {
                new KeyValuePair<TreeNode, TreeNode>(locateInput, imageOutput),
                new KeyValuePair<TreeNode, TreeNode>(qualityInput, qualityOutput),
                new KeyValuePair<TreeNode, TreeNode>(sendInput, poseOutput)
            };
            Func<KeyValuePair<TreeNode, TreeNode>[]> provider = delegate { return connections; };
            Func<bool> visible = delegate { return true; };
            setProvider.Invoke(editor, new object[] { provider, visible });

            editor.ExpandAll();
            editor.SelectedNode = locate;
            host.Show();
            Application.DoEvents();

            Assert(editor.DrawMode == TreeViewDrawMode.OwnerDrawAll && editor.ItemHeight >= 34 &&
                   editor.FullRowSelect && !editor.ShowLines && !editor.ShowPlusMinus && !editor.HotTracking,
                "Flow nodes are not using the card-and-port renderer.");
            Control connectionLayer = editor.Controls.Find("flowConnectionLayer", true).FirstOrDefault();
            Assert(connectionLayer != null && connectionLayer.Width >= 64 && connectionLayer.Height == editor.ClientSize.Height,
                "Flow connections must render on a dedicated double-buffered layer.");
            Assert((int)gutterWidth.GetValue(editor, null) >= 64,
                "Flow editor did not reserve a stable connection gutter.");
            Assert((int)portCenterX.GetValue(editor, null) + 4 < connectionLayer.Left,
                "The right-side connection point is still covered by the connection layer.");

            using (Bitmap bitmap = new Bitmap(editor.Width, editor.Height, PixelFormat.Format32bppPArgb))
            {
                editor.DrawToBitmap(bitmap, editor.ClientRectangle);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                    renderConnections.Invoke(editor, new object[] { graphics });

                Assert((int)renderedCount.GetValue(editor, null) == 3,
                    "Expanded workflow did not render all port-level connections.");
                Assert(CountVisiblePixels(bitmap) > bitmap.Width * bitmap.Height / 30,
                    "Rendered workflow preview is unexpectedly empty.");

                Directory.CreateDirectory(Path.GetDirectoryName(previewPath) ?? String.Empty);
                bitmap.Save(previewPath, ImageFormat.Png);
            }

            Func<bool> focusedOnly = delegate { return false; };
            setProvider.Invoke(editor, new object[] { provider, focusedOnly });
            editor.SelectedNode = acquire;
            using (Bitmap focused = new Bitmap(editor.Width, editor.Height))
            using (Graphics graphics = Graphics.FromImage(focused))
            {
                renderConnections.Invoke(editor, new object[] { graphics });
                Assert(!(bool)showAllConnections.GetValue(editor, null) &&
                       (int)renderedCount.GetValue(editor, null) == 0,
                    "Focused mode must not render connections for a selected module title.");
            }

            editor.SelectedNode = locateInput;
            using (Bitmap focusedInput = new Bitmap(editor.Width, editor.Height))
            using (Graphics graphics = Graphics.FromImage(focusedInput))
            {
                renderConnections.Invoke(editor, new object[] { graphics });
                Assert((int)renderedCount.GetValue(editor, null) == 1,
                    "Focused mode did not isolate the selected input-port connection.");
            }

            editor.SelectedNode = poseOutput;
            using (Bitmap focusedOutput = new Bitmap(editor.Width, editor.Height))
            using (Graphics graphics = Graphics.FromImage(focusedOutput))
            {
                renderConnections.Invoke(editor, new object[] { graphics });
                Assert((int)renderedCount.GetValue(editor, null) == 1,
                    "Focused mode did not isolate the selected output-port connection.");
            }

            editor.SelectedNode = null;
            using (Bitmap unselected = new Bitmap(editor.Width, editor.Height))
            using (Graphics graphics = Graphics.FromImage(unselected))
            {
                renderConnections.Invoke(editor, new object[] { graphics });
                Assert((int)renderedCount.GetValue(editor, null) == 0,
                    "Focused mode must hide connections when no input/output port is selected.");
            }

            setProvider.Invoke(editor, new object[] { provider, visible });
            editor.SelectedNode = locate;

            locate.Collapse();
            Application.DoEvents();
            Assert(ReferenceEquals(resolveVisibleEndpoint.Invoke(null, new object[] { imageOutput }), imageOutput) &&
                   ReferenceEquals(resolveVisibleEndpoint.Invoke(null, new object[] { locateInput }), locate) &&
                   ReferenceEquals(resolveVisibleEndpoint.Invoke(null, new object[] { poseOutput }), locate) &&
                   ReferenceEquals(resolveVisibleEndpoint.Invoke(null, new object[] { sendInput }), sendInput),
                "A mixed expanded/collapsed connection did not preserve the concrete endpoint on its expanded side.");
            using (Bitmap collapsed = new Bitmap(editor.Width, editor.Height))
            using (Graphics graphics = Graphics.FromImage(collapsed))
            {
                renderConnections.Invoke(editor, new object[] { graphics });
                Assert((int)renderedCount.GetValue(editor, null) == 3,
                    "A mixed expanded/collapsed workflow lost distinct ports on the expanded side.");
            }

            locate.Expand();
            Application.DoEvents();
            using (Bitmap expandedAgain = new Bitmap(editor.Width, editor.Height))
            using (Graphics graphics = Graphics.FromImage(expandedAgain))
            {
                renderConnections.Invoke(editor, new object[] { graphics });
                Assert((int)renderedCount.GetValue(editor, null) == 3,
                    "Expanding the workflow did not restore port-level connection details.");
            }

            editor.CollapseAll();
            Application.DoEvents();
            using (Bitmap fullyCollapsed = new Bitmap(editor.Width, editor.Height))
            using (Graphics graphics = Graphics.FromImage(fullyCollapsed))
            {
                renderConnections.Invoke(editor, new object[] { graphics });
                Assert((int)renderedCount.GetValue(editor, null) == 2,
                    "A fully collapsed workflow must render exactly one line per module relationship.");
            }

            TreeNode detached = new TreeNode("<--已删除端口");
            connections = connections.Concat(new[]
            {
                new KeyValuePair<TreeNode, TreeNode>(detached, imageOutput),
                new KeyValuePair<TreeNode, TreeNode>(null, null)
            }).ToArray();
            using (Bitmap invalid = new Bitmap(editor.Width, editor.Height))
            using (Graphics graphics = Graphics.FromImage(invalid))
            {
                renderConnections.Invoke(editor, new object[] { graphics });
                Assert((int)renderedCount.GetValue(editor, null) == 2,
                    "Detached connection endpoints were not filtered safely.");
            }

            Exception refreshFailure = null;
            Thread worker = new Thread(new ThreadStart(delegate
            {
                try
                {
                    for (int i = 0; i < 500; i++)
                        requestRefresh.Invoke(editor, null);
                }
                catch (Exception ex)
                {
                    refreshFailure = ex;
                }
            }));
            worker.IsBackground = true;
            worker.Start();
            worker.Join(3000);
            Application.DoEvents();
            Assert(refreshFailure == null && !worker.IsAlive,
                "High-frequency worker refresh did not remain bounded and UI-thread safe.");

            Func<KeyValuePair<TreeNode, TreeNode>[]> failingProvider = delegate
            {
                throw new InvalidOperationException("synthetic snapshot failure");
            };
            setProvider.Invoke(editor, new object[] { failingProvider, visible });
            using (Bitmap failure = new Bitmap(editor.Width, editor.Height))
            using (Graphics graphics = Graphics.FromImage(failure))
                renderConnections.Invoke(editor, new object[] { graphics });
            Assert((int)renderedCount.GetValue(editor, null) == 0,
                "A failed connection snapshot should skip one frame without crashing.");

            host.Hide();
        }
    }

    private static int CountVisiblePixels(Bitmap bitmap)
    {
        int count = 0;
        for (int y = 0; y < bitmap.Height; y += 2)
        {
            for (int x = 0; x < bitmap.Width; x += 2)
            {
                Color color = bitmap.GetPixel(x, y);
                if (color.A > 0 && color.ToArgb() != Color.White.ToArgb() &&
                    color.ToArgb() != Color.FromArgb(255, 255, 254, 250).ToArgb())
                    count++;
            }
        }
        return count;
    }

    private static bool ImageHasVisibleInk(Image image)
    {
        using (Bitmap bitmap = new Bitmap(image))
        {
            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                    if (bitmap.GetPixel(x, y).A > 20)
                        return true;
        }
        return false;
    }

    private static void SetField(Type type, object instance, string name, object value)
    {
        FieldInfo field = type.GetField(name,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(field != null, "Missing field: " + name);
        field.SetValue(instance, value);
    }

    private static object GetFieldValue(Type type, object instance, string name)
    {
        FieldInfo field = type.GetField(name,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(field != null, "Missing field: " + name);
        return field.GetValue(instance);
    }

    private static T GetField<T>(Type type, object instance, string name) where T : class
    {
        FieldInfo field = type.GetField(name,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(field != null, "Missing field: " + name);
        T value = field.GetValue(instance) as T;
        Assert(value != null, "Unexpected field value: " + name);
        return value;
    }

    private static void Assert(bool condition, string message)
    {
        assertionCount++;
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
