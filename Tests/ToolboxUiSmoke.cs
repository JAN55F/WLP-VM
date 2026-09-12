using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

internal static class ToolboxUiSmoke
{
    private static int assertionCount;

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Usage: ToolboxUiSmoke <CVMPro.dll> <preview.png>");
            return 2;
        }

        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Assembly assembly = Assembly.LoadFrom(args[0]);
            InitializeToolImages(assembly);
            VerifyWorkflowEditorMinimumSize(assembly);
            VerifyImageWindowMinimumSize(assembly);
            VerifyAndRenderToolbox(assembly, args[1]);
            Console.WriteLine("Toolbox UI smoke checks passed: {0} assertions.", assertionCount);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void InitializeToolImages(Assembly assembly)
    {
        Type jobType = assembly.GetType("VMPro.Job", true);
        MethodInfo initialize = jobType.GetMethod(
            "InitImageList",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(initialize != null, "Missing Job.InitImageList.");
        initialize.Invoke(null, null);
    }

        private static void VerifyAndRenderToolbox(Assembly assembly, string previewPath)
    {
        Type toolboxType = assembly.GetType("VMPro.Frm_ToolBox", true);
        using (Form toolbox = (Form)Activator.CreateInstance(toolboxType, true))
        {
            toolbox.ClientSize = new Size(310, 620);
            toolbox.Show();
            Application.DoEvents();
            toolbox.PerformLayout();

            TableLayoutPanel layout = FindControl<TableLayoutPanel>(toolbox, "modernToolboxLayout");
            Assert(layout.RowCount == 2 && layout.Dock == DockStyle.Fill,
                "Toolbox must use a compact header/tree layout.");
            Assert(toolbox.MinimumSize.Width == 286 && toolbox.MinimumSize.Height == 180,
                "Toolbox minimum size must protect its three-column layout without crowding other modules.");

            Control searchBox = FindControl<Control>(toolbox, "modernToolSearch");
            TextBox searchInput = FindControl<TextBox>(toolbox, "modernToolSearchInput");
            Button clearSearch = FindControl<Button>(toolbox, "modernToolSearchClear");
            Control toolboxHeader = FindControl<Control>(toolbox, "modernToolboxHeader");
            Assert(searchBox.Height >= searchInput.PreferredHeight + 3 &&
                   searchBox.Height <= toolboxHeader.Height / 2 &&
                   searchInput.BorderStyle == BorderStyle.None,
                "Toolbox search field is missing or still oversized: search=" + searchBox.Bounds +
                ", header=" + toolboxHeader.Bounds + ".");
            Assert(clearSearch.Image != null,
                "Search clear action must use a vector icon.");

            TreeView tree = GetField<TreeView>(toolboxType, toolbox, "tvw_tools");
            Assert(tree.GetType().Name == "ModernToolboxTreeView" &&
                   tree.DrawMode == TreeViewDrawMode.OwnerDrawAll &&
                   tree.ItemHeight >= 38 && !tree.ShowLines && !tree.ShowPlusMinus,
                "Toolbox catalogue is not using the modern owner-drawn tree.");
            Control grid = FindControl<Control>(toolbox, "modernToolGrid");
            Assert(!tree.Visible && tree.Parent != null && grid.Dock == DockStyle.Fill && ((ScrollableControl)grid).AutoScroll,
                "Tool shortcuts must be rendered in the scrollable grid while retaining the tree drag source.");
            FieldInfo cardHeight = grid.GetType().GetField("CardHeight", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo cardWidth = grid.GetType().GetField("CardWidth", BindingFlags.Static | BindingFlags.NonPublic);
            Assert(cardHeight != null && cardWidth != null &&
                   (int)cardHeight.GetRawConstantValue() == 76 &&
                   (int)cardWidth.GetRawConstantValue() == 78,
                "Tool shortcut cards must use the compact proportional dimensions.");

            string categories = string.Join(",", tree.Nodes.Cast<TreeNode>().Select(node => node.Text).ToArray());
            Assert(tree.Nodes.Count == 7 &&
                   categories == "图像输入与预处理,检测与识别,标定与定位,几何与 ROI,逻辑与计算,设备与通信,输出与显示",
                "Toolbox category structure changed unexpectedly: " + categories);
            Assert(CountLeafTools(tree) == 58,
                "Toolbox lost tools while building the modern catalogue.");

            Label toolCount = FindControl<Label>(toolbox, "modernToolCount");
            Button expand = FindControl<Button>(toolbox, "modernExpandAll");
            Button collapse = FindControl<Button>(toolbox, "modernCollapseAll");
            Assert(toolCount.Text.Contains("58") && expand.Image != null && collapse.Image != null,
                "Tool count or expand/collapse vector actions are missing.");
            collapse.PerformClick();
            Application.DoEvents();
            Assert(tree.Nodes.Cast<TreeNode>().All(node => !node.IsExpanded),
                "Collapse all must keep every category collapsed in the grid.");
            expand.PerformClick();
            Application.DoEvents();
            Assert(tree.Nodes.Cast<TreeNode>().All(node => node.IsExpanded),
                "Expand all must expand every category in the grid.");
            collapse.PerformClick();
            Application.DoEvents();
            MethodInfo setCategoryExpanded = toolboxType.GetMethod("SetCategoryExpanded", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo applyFilter = toolboxType.GetMethod("ApplyModernToolFilter", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert(setCategoryExpanded != null && applyFilter != null, "Missing toolbox category-state refresh methods.");
            setCategoryExpanded.Invoke(toolbox, new object[] { tree.Nodes[0], true });
            applyFilter.Invoke(toolbox, null);
            Assert(tree.Nodes[0].IsExpanded && tree.Nodes.Cast<TreeNode>().Skip(1).All(node => !node.IsExpanded),
                "A category must retain its individual expanded state after a grid refresh.");

            Label infoText = GetField<Label>(toolboxType, toolbox, "lbl_toolInfo");
            Assert(toolbox.Controls.Find("modernToolboxInfoCard", true).Length == 0 &&
                   infoText.Parent == null,
                "Tool description area should not consume toolbox space.");

            searchInput.Focus();
            searchInput.Text = "图像";
            Application.DoEvents();
            int matchingTools = CountLeafTools(tree);
            Assert(matchingTools > 0 && matchingTools < 58 && toolCount.Text.Contains("/ 58"),
                "Incremental toolbox filtering did not reduce the visible catalogue.");

            searchInput.Text = "不存在的工具-XYZ";
            Application.DoEvents();
            Label emptyState = FindControl<Label>(toolbox, "modernToolboxEmptyState");
            Assert(tree.Nodes.Count == 0 && emptyState.Visible,
                "No-result search state is not visible.");

            clearSearch.PerformClick();
            Application.DoEvents();
            Assert(CountLeafTools(tree) == 58 && tree.Nodes[0].IsExpanded,
                "Clearing search did not restore the complete toolbox.");

            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(previewPath)));
            using (Bitmap bitmap = new Bitmap(toolbox.Width, toolbox.Height))
            {
                toolbox.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                bitmap.Save(previewPath, ImageFormat.Png);
            }

            toolbox.Hide();
        }
    }

        private static void VerifyWorkflowEditorMinimumSize(Assembly assembly)
        {
            Type workflowEditorType = assembly.GetType("VMPro.Frm_Job", true);
            FieldInfo minimumWidth = workflowEditorType.GetField(
                "ModernWorkflowEditorMinimumWidth", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo minimumHeight = workflowEditorType.GetField(
                "ModernWorkflowEditorMinimumHeight", BindingFlags.Static | BindingFlags.NonPublic);
            Assert(minimumWidth != null && minimumHeight != null &&
                   (int)minimumWidth.GetRawConstantValue() == 300 &&
                   (int)minimumHeight.GetRawConstantValue() == 240,
                "Workflow editor must expose a usable minimum size beside the toolbox.");
        }

        private static void VerifyImageWindowMinimumSize(Assembly assembly)
        {
            Type imageWindowType = assembly.GetType("VMPro.Frm_ImageWindow", true);
            FieldInfo minimumWidth = imageWindowType.GetField(
                "ImageWindowMinimumWidth", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo minimumHeight = imageWindowType.GetField(
                "ImageWindowMinimumHeight", BindingFlags.Static | BindingFlags.NonPublic);
            Assert(minimumWidth != null && minimumHeight != null &&
                   (int)minimumWidth.GetRawConstantValue() == 300 &&
                   (int)minimumHeight.GetRawConstantValue() == 220,
                "Image window must expose a usable minimum size beside the workflow editor.");
        }

    private static int CountLeafTools(TreeView tree)
    {
        int count = 0;
        foreach (TreeNode root in tree.Nodes)
            count += root.Nodes.Count;
        return count;
    }

    private static T FindControl<T>(Control root, string name) where T : Control
    {
        T control = root.Controls.Find(name, true).OfType<T>().FirstOrDefault();
        if (control == null)
            throw new InvalidOperationException("Missing control: " + name);
        return control;
    }

    private static T GetField<T>(Type type, object instance, string name) where T : class
    {
        FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field == null)
            throw new MissingFieldException(type.FullName, name);
        T value = field.GetValue(instance) as T;
        if (value == null)
            throw new InvalidOperationException("Field is null or has unexpected type: " + name);
        return value;
    }

    private static void Assert(bool condition, string message)
    {
        assertionCount++;
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
