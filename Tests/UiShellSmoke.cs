using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

internal static class UiShellSmoke
{
    private sealed class IcoDirectoryEntry
    {
        internal int Width;
        internal int Height;
        internal int Planes;
        internal int BitsPerPixel;
        internal int DataLength;
        internal int DataOffset;
        internal byte[] PngData;
    }

    private static int capturedTextChangeCount;
    private static string capturedTextChangeValue;
    private static int capturedNumericChangeCount;
    private static double capturedNumericChangeValue;
    private static int assertionCount;

    private sealed class LayoutStub : DockContent
    {
        private readonly string persistString;

        internal LayoutStub(string value)
        {
            persistString = value;
            HideOnClose = true;
        }

        internal string PersistString
        {
            get { return persistString; }
        }

        protected override string GetPersistString()
        {
            return persistString;
        }
    }

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.Error.WriteLine("Usage: UiShellSmoke <CVMPro.dll> <layout.config> <preview.png>");
            return 2;
        }

        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            VerifyProductIdentityArtifacts(args[0], args[1], Path.GetDirectoryName(args[2]) ?? string.Empty);
            VerifyLayoutTemplate(args[1]);
            VerifyAndRenderShell(args[0], args[1], args[2]);
            VerifyModuleOrganization(args[0]);
            Console.WriteLine("UI shell and Dock layout smoke checks passed: {0} assertions.", assertionCount);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void VerifyProductIdentityArtifacts(
        string assemblyPath,
        string layoutPath,
        string outputDirectory)
    {
        string workspaceRoot = FindWorkspaceRoot(layoutPath, assemblyPath);
        string startDirectory = Path.Combine(workspaceRoot, "Start");
        string projectPath = Path.Combine(startDirectory, "Start.csproj");
        string iconPath = Path.Combine(startDirectory, "WLPVM.ico");
        string activeManifestPath = Path.Combine(startDirectory, "app.manifest");
        string designerManifestPath = Path.Combine(startDirectory, "Properties", "app.manifest");
        string assemblyInfoPath = Path.Combine(startDirectory, "Properties", "AssemblyInfo.cs");

        Assert(File.Exists(projectPath) && File.Exists(iconPath) &&
               File.Exists(activeManifestPath) && File.Exists(designerManifestPath) &&
               File.Exists(assemblyInfoPath),
            "The WLP VM Start project identity artifacts are incomplete.");

        string projectText = File.ReadAllText(projectPath);
        Assert(projectText.Contains("<AssemblyName>WLP VM</AssemblyName>") &&
               projectText.Contains("<ApplicationIcon>WLPVM.ico</ApplicationIcon>") &&
               projectText.Contains("<ApplicationManifest>app.manifest</ApplicationManifest>") &&
               projectText.Contains("<Content Include=\"WLPVM.ico\"") &&
               !projectText.Contains("<ApplicationIcon>Logo.ico</ApplicationIcon>"),
            "Start.csproj must use the WLP VM assembly name, manifest, and WLPVM.ico content/application icon.");

        foreach (string manifestPath in new[] { activeManifestPath, designerManifestPath })
        {
            string manifestText = File.ReadAllText(manifestPath);
            Assert(manifestText.Contains("name=\"WLPVM.app\"") &&
                   manifestText.Contains("version=\"1.0.0.0\"") &&
                   manifestText.Contains("level=\"requireAdministrator\""),
                "Manifest identity or execution level is inconsistent: " + manifestPath);
        }

        string assemblyInfo = File.ReadAllText(assemblyInfoPath);
        Assert(assemblyInfo.Contains("AssemblyTitle(\"WLP VM\")") &&
               assemblyInfo.Contains("AssemblyProduct(\"WLP VM\")") &&
               assemblyInfo.Contains("AssemblyCompany(\"威乐普电子科技有限公司\")") &&
               assemblyInfo.Contains("AssemblyVersion(\"1.0.0.0\")") &&
               assemblyInfo.Contains("AssemblyInformationalVersion(\"1.0.0\")") &&
               assemblyInfo.Contains("通用视觉软件"),
            "The executable assembly metadata must retain the WLP VM generic-vision identity.");

        IList<IcoDirectoryEntry> iconEntries = ReadWlpIconDirectory(iconPath);
        int[] expectedSizes = { 16, 20, 24, 32, 40, 48, 64, 128, 256 };
        Assert(iconEntries.Count == expectedSizes.Length &&
               iconEntries.Select(entry => entry.Width).SequenceEqual(expectedSizes) &&
               iconEntries.All(entry => entry.Width == entry.Height &&
                                        entry.Planes == 1 &&
                                        entry.BitsPerPixel == 32),
            "WLPVM.ico must contain the nine square 32-bit sizes: " +
            string.Join(",", iconEntries.Select(entry => entry.Width.ToString()).ToArray()));

        string executablePath = Path.Combine(startDirectory, "bin", "Debug", "WLP VM.exe");
        Assert(File.Exists(executablePath),
            "The rebuilt WLP VM.exe is missing; build Start before the isolated smoke run.");
        Assert(AssemblyName.GetAssemblyName(executablePath).Name == "WLP VM",
            "The rebuilt executable assembly identity is not WLP VM.");
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(executablePath);
        Assert(versionInfo.FileDescription == "WLP VM" &&
               versionInfo.ProductName == "WLP VM" &&
               versionInfo.CompanyName == "威乐普电子科技有限公司" &&
               versionInfo.FileVersion.StartsWith("1.0.0", StringComparison.Ordinal) &&
               versionInfo.ProductVersion.StartsWith("1.0.0", StringComparison.Ordinal),
            "The rebuilt executable version resource has stale product/company branding.");

        IcoDirectoryEntry source32Entry = iconEntries.Single(entry => entry.Width == 32);
        using (Icon embeddedIcon = Icon.ExtractAssociatedIcon(executablePath))
        using (MemoryStream sourceStream = new MemoryStream(source32Entry.PngData, false))
        using (Image sourceImage = Image.FromStream(sourceStream))
        {
            Assert(embeddedIcon != null, "WLP VM.exe does not expose an embedded application icon.");
            using (Bitmap embeddedBitmap = embeddedIcon.ToBitmap())
            using (Bitmap sourceBitmap = new Bitmap(sourceImage))
            {
                if (!string.IsNullOrEmpty(outputDirectory))
                {
                    embeddedBitmap.Save(Path.Combine(outputDirectory, "wlpvm-exe-embedded-icon.png"), ImageFormat.Png);
                    sourceBitmap.Save(Path.Combine(outputDirectory, "wlpvm-source-icon-32.png"), ImageFormat.Png);
                }
                double iconMatchRatio = CalculateImageMatchRatio(sourceBitmap, embeddedBitmap, 32, 8);
                Console.WriteLine("embedded_icon_match={0:P2}", iconMatchRatio);
                Assert(ImageHasVisibleInk(embeddedBitmap) && iconMatchRatio >= 0.80D,
                    "WLP VM.exe does not embed the current WLPVM.ico artwork.");
            }
        }

        RenderWlpIconAtlas(iconEntries, outputDirectory);
    }

    private static string FindWorkspaceRoot(string layoutPath, string assemblyPath)
    {
        foreach (string startingPath in new[]
        {
            Environment.CurrentDirectory,
            Path.GetDirectoryName(Path.GetFullPath(layoutPath)),
            Path.GetDirectoryName(Path.GetFullPath(assemblyPath))
        })
        {
            DirectoryInfo directory = string.IsNullOrEmpty(startingPath)
                ? null
                : new DirectoryInfo(startingPath);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Start", "Start.csproj")))
                    return directory.FullName;
                directory = directory.Parent;
            }
        }

        throw new InvalidOperationException("Unable to locate the WLP VM workspace root.");
    }

    private static IList<IcoDirectoryEntry> ReadWlpIconDirectory(string iconPath)
    {
        byte[] fileBytes = File.ReadAllBytes(iconPath);
        List<IcoDirectoryEntry> entries = new List<IcoDirectoryEntry>();
        using (MemoryStream stream = new MemoryStream(fileBytes, false))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            Assert(reader.ReadUInt16() == 0 && reader.ReadUInt16() == 1,
                "WLPVM.ico has an invalid ICO header.");
            int count = reader.ReadUInt16();
            Assert(count == 9 && fileBytes.Length >= 6 + count * 16,
                "WLPVM.ico must expose exactly nine directory entries.");

            for (int index = 0; index < count; index++)
            {
                IcoDirectoryEntry entry = new IcoDirectoryEntry();
                entry.Width = reader.ReadByte();
                entry.Height = reader.ReadByte();
                if (entry.Width == 0)
                    entry.Width = 256;
                if (entry.Height == 0)
                    entry.Height = 256;
                reader.ReadByte();
                reader.ReadByte();
                entry.Planes = reader.ReadUInt16();
                entry.BitsPerPixel = reader.ReadUInt16();
                entry.DataLength = checked((int)reader.ReadUInt32());
                entry.DataOffset = checked((int)reader.ReadUInt32());
                Assert(entry.DataOffset >= 6 + count * 16 && entry.DataLength > 24 &&
                       entry.DataOffset <= fileBytes.Length - entry.DataLength,
                    "WLPVM.ico contains an out-of-range image payload.");
                entry.PngData = new byte[entry.DataLength];
                Buffer.BlockCopy(fileBytes, entry.DataOffset, entry.PngData, 0, entry.DataLength);
                Assert(IsPngIconPayload(entry),
                    "Every WLPVM.ico entry must be an 8-bit RGBA PNG matching its directory size.");
                entries.Add(entry);
            }
        }
        return entries;
    }

    private static bool IsPngIconPayload(IcoDirectoryEntry entry)
    {
        byte[] data = entry.PngData;
        byte[] pngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };
        if (data == null || data.Length < 26 || !pngSignature.SequenceEqual(data.Take(8)))
            return false;
        if (data[12] != (byte)'I' || data[13] != (byte)'H' ||
            data[14] != (byte)'D' || data[15] != (byte)'R')
            return false;

        int width = ReadBigEndianInt32(data, 16);
        int height = ReadBigEndianInt32(data, 20);
        return width == entry.Width && height == entry.Height && data[24] == 8 && data[25] == 6;
    }

    private static int ReadBigEndianInt32(byte[] data, int offset)
    {
        return (data[offset] << 24) |
               (data[offset + 1] << 16) |
               (data[offset + 2] << 8) |
               data[offset + 3];
    }

    private static void RenderWlpIconAtlas(IList<IcoDirectoryEntry> entries, string outputDirectory)
    {
        if (string.IsNullOrEmpty(outputDirectory))
            return;

        Directory.CreateDirectory(outputDirectory);
        using (Bitmap atlas = new Bitmap(760, 360))
        using (Graphics graphics = Graphics.FromImage(atlas))
        using (SolidBrush titleBrush = new SolidBrush(Color.FromArgb(38, 55, 72)))
        using (Pen borderPen = new Pen(Color.FromArgb(207, 228, 244)))
        {
            graphics.Clear(Color.FromArgb(252, 250, 246));
            graphics.DrawString("WLP VM 应用图标 · 9 尺寸", SystemFonts.MessageBoxFont, titleBrush, 18F, 14F);
            int x = 20;
            int y = 58;
            foreach (IcoDirectoryEntry entry in entries)
            {
                int displaySize = Math.Min(entry.Width, 96);
                if (x + displaySize + 70 > atlas.Width)
                {
                    x = 20;
                    y += 135;
                }
                Rectangle tile = new Rectangle(x, y, Math.Max(72, displaySize + 12), 116);
                graphics.FillRectangle(Brushes.White, tile);
                graphics.DrawRectangle(borderPen, tile);
                using (MemoryStream imageStream = new MemoryStream(entry.PngData, false))
                using (Image iconImage = Image.FromStream(imageStream))
                {
                    int imageX = tile.X + (tile.Width - displaySize) / 2;
                    graphics.DrawImage(iconImage, new Rectangle(imageX, tile.Y + 8, displaySize, displaySize));
                }
                graphics.DrawString(entry.Width + " px", SystemFonts.MessageBoxFont, titleBrush,
                    tile.X + 7, tile.Bottom - 23);
                x = tile.Right + 12;
            }
            atlas.Save(Path.Combine(outputDirectory, "wlpvm-icon-9-sizes.png"), ImageFormat.Png);
        }
    }

    private static double CalculateImageMatchRatio(
        Image expected,
        Image actual,
        int comparisonSize,
        int tolerance)
    {
        using (Bitmap left = new Bitmap(comparisonSize, comparisonSize))
        using (Bitmap right = new Bitmap(comparisonSize, comparisonSize))
        {
            using (Graphics graphics = Graphics.FromImage(left))
                graphics.DrawImage(expected, new Rectangle(0, 0, comparisonSize, comparisonSize));
            using (Graphics graphics = Graphics.FromImage(right))
                graphics.DrawImage(actual, new Rectangle(0, 0, comparisonSize, comparisonSize));

            int matchingPixels = 0;
            int totalPixels = comparisonSize * comparisonSize;
            for (int y = 0; y < comparisonSize; y++)
            {
                for (int x = 0; x < comparisonSize; x++)
                {
                    Color first = left.GetPixel(x, y);
                    Color second = right.GetPixel(x, y);
                    if ((first.A <= tolerance && second.A <= tolerance) ||
                        (Math.Abs(first.A - second.A) <= tolerance &&
                         ColorsNear(first, second, tolerance)))
                        matchingPixels++;
                }
            }
            return matchingPixels / (double)totalPixels;
        }
    }

    private static void VerifyLayoutTemplate(string layoutPath)
    {
        using (Form host = new Form())
        using (DockPanel dockPanel = new DockPanel())
        {
            host.IsMdiContainer = true;
            host.ClientSize = new Size(1200, 700);
            host.StartPosition = FormStartPosition.Manual;
            host.Location = new Point(-32000, -32000);
            host.ShowInTaskbar = false;
            dockPanel.Dock = DockStyle.Fill;
            host.Controls.Add(dockPanel);
            host.Show();

            DeserializeDockContent deserialize = delegate(string persistString)
            {
                return new LayoutStub(persistString);
            };
            dockPanel.LoadFromXml(layoutPath, deserialize);
            Application.DoEvents();

            Assert(dockPanel.Contents.Count == 5, "Standard layout must restore five Dock contents.");
            Assert(Math.Abs(dockPanel.DockLeftPortion - 0.24D) < 0.001D, "Unexpected left Dock portion.");
            Assert(Math.Abs(dockPanel.DockRightPortion - 0.50D) < 0.001D, "Unexpected right Dock portion.");
            Assert(Math.Abs(dockPanel.DockBottomPortion - 0.22D) < 0.001D, "Unexpected bottom Dock portion.");

            LayoutStub job = FindLayoutStub(dockPanel, "VMPro.Frm_Job");
            LayoutStub toolBox = FindLayoutStub(dockPanel, "VMPro.Frm_ToolBox");
            LayoutStub output = FindLayoutStub(dockPanel, "VMPro.Frm_Output");
            LayoutStub monitor = FindLayoutStub(dockPanel, "VMPro.Frm_Monitor");
            LayoutStub image = FindLayoutStub(dockPanel, "VMPro.Frm_ImageWindow");
            AssertEditorColumns(dockPanel, image, job, toolBox);
            Assert(ReferenceEquals(output.Pane, monitor.Pane) && output.DockState == DockState.DockBottom,
                "Output and monitor must share one bottom tab pane.");
            Assert(image.DockState == DockState.Document && image.Pane != null,
                "The image window must remain the primary Document content.");
            host.PerformLayout();
            Rectangle jobBounds = dockPanel.RectangleToClient(
                job.Pane.RectangleToScreen(job.Pane.ClientRectangle));
            Rectangle outputBounds = dockPanel.RectangleToClient(
                output.Pane.RectangleToScreen(output.Pane.ClientRectangle));
            Assert(jobBounds.Bottom >= dockPanel.ClientRectangle.Bottom - 2,
                "The workflow/toolbox pane must extend to the bottom of the vision workspace. pane=" +
                jobBounds + ", workspace=" + dockPanel.ClientRectangle);
            Assert(outputBounds.Right <= jobBounds.Left + 2,
                "The output/monitor pane must stop before the workflow/toolbox column. output=" +
                outputBounds + ", editor=" + jobBounds);
            Assert(dockPanel.Panes.Count == 4,
                "The layout needs document, workflow, toolbox, and bottom-tab panes.");
        }
    }

    private static void AssertEditorColumns(DockPanel dock, DockContent image,
        DockContent job, DockContent toolbox)
    {
        Assert(job.DockState == DockState.DockRight && toolbox.DockState == DockState.DockRight &&
               job.Pane != null && toolbox.Pane != null && job.Pane != toolbox.Pane,
            "Workflow and toolbox must have separate visible right-side panes.");
        dock.PerformLayout();
        Rectangle jobBounds = dock.RectangleToClient(job.Pane.RectangleToScreen(job.Pane.ClientRectangle));
        Rectangle toolboxBounds = dock.RectangleToClient(toolbox.Pane.RectangleToScreen(toolbox.Pane.ClientRectangle));
        Rectangle imageBounds = dock.RectangleToClient(image.Pane.RectangleToScreen(image.Pane.ClientRectangle));
        Assert(imageBounds.Right <= jobBounds.Left + 2 && jobBounds.Right <= toolboxBounds.Left + 2,
            "Columns must be ordered image, workflow, toolbox from left to right.");
        Assert(Math.Abs(jobBounds.Top - toolboxBounds.Top) <= 2 &&
               Math.Abs(jobBounds.Bottom - toolboxBounds.Bottom) <= 2 &&
               jobBounds.Width > 100 && toolboxBounds.Width > 100,
            "Both editors must remain visible side by side at full height.");
    }

    private static void VerifyAndRenderShell(string assemblyPath, string layoutPath, string previewPath)
    {
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        Type mainType = assembly.GetType("VMPro.Frm_Main", true);
        MethodInfo resolveDockLayoutPath = mainType.GetMethod(
            "ResolveDockLayoutPath",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo isFactoryDockLayout = mainType.GetMethod(
            "IsFactoryDockLayout",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(resolveDockLayoutPath != null && isFactoryDockLayout != null,
            "Dock layout path guards are missing.");
        string defaultDockLayoutPath = (string)resolveDockLayoutPath.Invoke(null, new object[] { string.Empty });
        Assert((bool)isFactoryDockLayout.Invoke(null, new object[] { defaultDockLayoutPath }),
            "An empty layout configuration must resolve to a protected factory layout.");
        bool originalCrossThreadCheck = Control.CheckForIllegalCrossThreadCalls;
        Control.CheckForIllegalCrossThreadCalls = true;
        try
        {
            using (Form form = (Form)Activator.CreateInstance(mainType, true))
            {
                Assert(Control.CheckForIllegalCrossThreadCalls,
                    "Main-form construction must not globally disable WinForms cross-thread checks.");
                Control.CheckForIllegalCrossThreadCalls = originalCrossThreadCheck;
                form.ClientSize = new Size(1280, 800);
                form.CreateControl();

            Type formModeType = assembly.GetType("VMPro.FormMode", true);
            object visionMode = Enum.Parse(formModeType, "VisionForm");
            MethodInfo applyWorkspace = mainType.GetMethod("ApplyWorkspaceMode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert(applyWorkspace != null, "Missing ApplyWorkspaceMode method.");
            applyWorkspace.Invoke(form, new[] { visionMode });
            SetLocalVisible(form, true);
            form.PerformLayout();

            Panel header = GetField<Panel>(mainType, form, "panel1");
            MenuStrip menu = GetField<MenuStrip>(mainType, form, "menuStrip1");
            ToolStrip contextBar = GetField<ToolStrip>(mainType, form, "toolStrip2");
            StatusStrip status = GetField<StatusStrip>(mainType, form, "statusStrip1");
            DockPanel dock = GetField<DockPanel>(mainType, form, "dockPanel");
            ToolStrip primaryBar = GetField<ToolStrip>(mainType, form, "toolStrip1");
            Label mainTitle = GetField<Label>(mainType, form, "lbl_title");
            Label mainProductMark = FindControl<Label>(form, "mainProductMark");
            Assert(mainTitle.Text == "威乐普电子科技有限公司 - WLP VM v1.0.0" &&
                    !mainTitle.Text.Contains("手机组装") && !mainTitle.Text.Contains("学习版"),
                "Main shell must expose the WLP VM product brand and release suffix before configuration loading.");
            Assert(mainProductMark.Text == "WLP" &&
                   !GetField<PictureBox>(mainType, form, "pictureBox1").Visible,
                "Main title band must replace the legacy vm bitmap with the WLP brand mark.");

            Type configurationType = assembly.GetType("VMPro.Configuration", true);
            MethodInfo normalizeProgramTitle = configurationType.GetMethod(
                "NormalizeProgramTitle",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo buildApplicationTitle = configurationType.GetMethod(
                "BuildApplicationTitle",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Assert(normalizeProgramTitle != null && buildApplicationTitle != null,
                "Brand title normalization helpers are missing.");
            FieldInfo companyBrand = configurationType.GetField(
                "DefaultCompanyName",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo productBrand = configurationType.GetField(
                "ProductName",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo productVersion = configurationType.GetField(
                "ProductVersion",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo productDisplayName = configurationType.GetField(
                "ProductDisplayName",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Assert(companyBrand != null && productBrand != null && productVersion != null &&
                   productDisplayName != null &&
                   (string)companyBrand.GetRawConstantValue() == "威乐普电子科技有限公司" &&
                   (string)productBrand.GetRawConstantValue() == "WLP VM" &&
                   (string)productVersion.GetRawConstantValue() == "1.0.0" &&
                   (string)productDisplayName.GetRawConstantValue() == "WLP VM v1.0.0",
                "Fixed company/product/version constants are missing or inconsistent.");
            string[] legacyProductTitles = { "", "未命名", "手机组装", "通用视觉软件", "VM Pro", "VM Pro 通用视觉软件", "WLP VM v1.0.0" };
            foreach (string legacyTitle in legacyProductTitles)
                Assert((string)normalizeProgramTitle.Invoke(null, new object[] { legacyTitle }) == "WLP VM",
                    "Legacy product title was not normalized: " + legacyTitle);
            Assert((string)normalizeProgramTitle.Invoke(null, new object[] { "A01 点胶线" }) == "A01 点胶线",
                "A user-defined project title must remain unchanged.");
            const string fixedTitlePrefix = "威乐普电子科技有限公司 - WLP VM v1.0.0";
            Assert((string)buildApplicationTitle.Invoke(null, new object[] { null }) == fixedTitlePrefix &&
                   (string)buildApplicationTitle.Invoke(null, new object[] { "WLP VM" }) == fixedTitlePrefix,
                "Default title composition must always retain the fixed WLP VM product identity.");
            foreach (string customProjectTitle in new[] { "A01 点胶线", "{客户A} 定位项目" })
            {
                string applicationTitle = (string)buildApplicationTitle.Invoke(
                    null,
                    new object[] { customProjectTitle });
                Assert(applicationTitle == fixedTitlePrefix + " · " + customProjectTitle &&
                       applicationTitle.StartsWith(fixedTitlePrefix, StringComparison.Ordinal),
                    "A custom project title must remain a suffix and never replace WLP VM: " + applicationTitle);
            }

            MethodInfo getPersistedContent = mainType.GetMethod(
                "GetContentFromPersistString",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert(getPersistedContent != null, "Missing Dock content deserializer.");
            DeserializeDockContent deserialize = delegate(string persistString)
            {
                return (IDockContent)getPersistedContent.Invoke(form, new object[] { persistString });
            };
            dock.LoadFromXml(layoutPath, deserialize);
            form.PerformLayout();

            Assert(form.MinimumSize.Width >= 1024 && form.MinimumSize.Height >= 640, "Main minimum size is too small.");
            Assert(header.Dock == DockStyle.Top && header.Height >= 88, "Compact header layout was not applied.");
            Assert(menu.Parent == header && menu.Dock == DockStyle.None &&
                   menu.Top >= 36 && menu.Bottom == header.ClientSize.Height,
                "Main menus must share the machine-command row instead of consuming a second row.");
            Assert(IsLocallyVisible(contextBar) && contextBar.Dock == DockStyle.Top, "Vision command bar should be visible in Vision mode.");
            Assert(status.Dock == DockStyle.Bottom && status.RightToLeft == RightToLeft.No, "Status bar direction or docking is invalid.");
            Assert(IsLocallyVisible(dock) && dock.Dock == DockStyle.Fill, "Vision Dock workspace should fill the content area.");
            Assert(dock.Contents.Count == 5, "Vision workspace must restore the five standard Dock modules.");
            Assert(dock.Top >= contextBar.Bottom, "Fill workspace overlaps the title/menu/command bars.");
            Assert(dock.Bottom <= status.Top, "Fill workspace overlaps the status bar.");

            DockContent jobContent = FindDockContent(dock, "VMPro.Frm_Job");
            DockContent toolBoxContent = FindDockContent(dock, "VMPro.Frm_ToolBox");
            DockContent outputContent = FindDockContent(dock, "VMPro.Frm_Output");
            DockContent monitorContent = FindDockContent(dock, "VMPro.Frm_Monitor");
            DockContent imageContent = FindDockContent(dock, "VMPro.Frm_ImageWindow");
            AssertEditorColumns(dock, imageContent, jobContent, toolBoxContent);
            MethodInfo ensureColumns = mainType.GetMethod("EnsureVisionEditorColumns",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert(ensureColumns != null, "Missing legacy-layout migration.");
            // 模拟旧项目把两个编辑器放在同一页签，再验证拆列及重复调用保留用户列宽。
            toolBoxContent.Show(jobContent.Pane, null);
            ensureColumns.Invoke(form, null);
            form.PerformLayout();
            AssertEditorColumns(dock, imageContent, jobContent, toolBoxContent);
            DockPane toolboxPane = toolBoxContent.Pane;
            dock.DockRightPortion = 0.55D;
            ensureColumns.Invoke(form, null);
            Assert(ReferenceEquals(toolboxPane, toolBoxContent.Pane) &&
                   Math.Abs(dock.DockRightPortion - 0.55D) < 0.001D,
                "Reopening the toolbox must preserve panes and resized column widths.");
            dock.DockRightPortion = 0.50D;
            Assert(ReferenceEquals(outputContent.Pane, monitorContent.Pane) && outputContent.DockState == DockState.DockBottom,
                "Live output and monitor must restore into one bottom tab pane.");
            Assert(imageContent.DockState == DockState.Document,
                "The live image workspace must restore as the central document.");
            Rectangle liveJobBounds = dock.RectangleToClient(
                jobContent.Pane.RectangleToScreen(jobContent.Pane.ClientRectangle));
            Rectangle liveOutputBounds = dock.RectangleToClient(
                outputContent.Pane.RectangleToScreen(outputContent.Pane.ClientRectangle));
            Assert(liveJobBounds.Bottom >= dock.ClientRectangle.Bottom - 2,
                "Live workflow/toolbox pane must use the full workspace height.");
            Assert(liveOutputBounds.Right <= liveJobBounds.Left + 2,
                "Live bottom logs must not consume space below the workflow/toolbox pane.");

            ToolStripItem[] visibleTopMenus = menu.Items.Cast<ToolStripItem>()
                .Where(item => item.Available)
                .ToArray();
            string[] menuLabels = visibleTopMenus.Select(item => item.Text).ToArray();
            string[] expected = { "项目", "流程", "视觉", "设备", "系统", "帮助" };
            Assert(menu.Items.Count == 6 && visibleTopMenus.Length == 6 && menuLabels.SequenceEqual(expected),
                "Top-level menus must be the six non-overlapping modules: " +
                string.Join(",", menuLabels));

            ToolStripItem[] menuProxies = EnumerateToolStripItems(menu.Items)
                .Where(item => item.Tag is ToolStripItem)
                .ToArray();
            Assert(menuProxies.Length >= 8,
                "The consolidated menus must retain their original command proxies.");
            foreach (ToolStripItem proxy in menuProxies)
            {
                ToolStripItem source = proxy.Tag as ToolStripItem;
                Assert(source != null && HasClickHandler(proxy) && HasClickHandler(source),
                    "Menu proxy lost its source or executable Click chain: " + proxy.Name);
            }
            VerifyProxyEnablementGate(mainType);

            ToolStripMenuItem projectMenu = visibleTopMenus
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == "项目");
            ToolStripMenuItem systemMenu = visibleTopMenus
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == "系统");
            Assert(projectMenu != null && systemMenu != null,
                "Project/System menu modules are missing.");
            ToolStripItem[] allShellMenuItems = EnumerateToolStripItems(menu.Items).ToArray();
            Assert(!allShellMenuItems.Any(item => item.Text == "删除当前方案" || item.Text == "删除方案" ||
                                                 ((item.Tag as ToolStripItem) != null &&
                                                  ((ToolStripItem)item.Tag).Name == "toolStripButton18")),
                "The incomplete top-level delete-solution shortcut must not be exposed in the shell.");
            ToolStripMenuItem optionsProxy = EnumerateToolStripItems(systemMenu.DropDownItems)
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == "选项...");
            Assert(optionsProxy != null && optionsProxy.Tag is ToolStripItem &&
                   ((ToolStripItem)optionsProxy.Tag).Name == "toolStripButton10" &&
                   HasClickHandler(optionsProxy) && HasClickHandler((ToolStripItem)optionsProxy.Tag),
                "System > Options must retain its executable permission-aware source command.");

            ToolStripMenuItem visionMenu = visibleTopMenus
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == "视觉");
            Assert(visionMenu != null, "The consolidated Vision menu is missing.");
            ToolStripItem[] visionMenuItems = EnumerateToolStripItems(visionMenu.DropDownItems).ToArray();
            ToolStripMenuItem previousLocalImage = visionMenuItems
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == "上一张本地图像");
            ToolStripMenuItem pauseDirectoryAdvance = visionMenuItems
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == "暂停目录图自动切换");
            ToolStripMenuItem speedModeProxy = visionMenuItems
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == "极速模式");
            ToolStripMenuItem globalVariableProxy = visionMenuItems
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text.StartsWith("全局变量", StringComparison.Ordinal));
            Assert(previousLocalImage != null && previousLocalImage.Tag is ToolStripItem &&
                   ((ToolStripItem)previousLocalImage.Tag).Name == "toolStripButton26" &&
                   HasClickHandler(previousLocalImage) && HasClickHandler((ToolStripItem)previousLocalImage.Tag),
                "Vision > Image must expose the working previous-local-image command proxy.");
            MethodInfo canNavigatePrevious = mainType.GetMethod(
                "CanNavigatePreviousLocalImage",
                BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo previousClickHandler = mainType.GetMethod(
                "toolStripButton26_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert(canNavigatePrevious != null && previousClickHandler != null &&
                   MethodBodyContainsDirectCall(previousClickHandler, canNavigatePrevious) &&
                   !(bool)canNavigatePrevious.Invoke(form, null),
                "Previous-local-image execution must short-circuit when no directory-image job is selected.");
            ToolStripDropDownItem imageMenu = previousLocalImage.OwnerItem as ToolStripDropDownItem;
            Assert(imageMenu != null, "Previous-local-image command lost its Image menu owner.");
            RaiseDropDownOpening(imageMenu);
            Assert(!previousLocalImage.Enabled,
                "Previous-local-image command must be disabled when its runtime guard is false.");
            Assert(globalVariableProxy != null && globalVariableProxy.Tag is ToolStripItem &&
                   ((ToolStripItem)globalVariableProxy.Tag).Name == "toolStripButton34" &&
                   HasClickHandler(globalVariableProxy) && HasClickHandler((ToolStripItem)globalVariableProxy.Tag),
                "Vision must reach the permission-aware global-variable command through toolStripButton34.");
            Assert(!visionMenuItems.Any(item => item.Text.StartsWith("下一张", StringComparison.Ordinal) ||
                                                (item.Tag as ToolStripItem) != null &&
                                                ((ToolStripItem)item.Tag).Name == "toolStripButton30"),
                "The no-op next-image shell command must remain inside the acquisition module, not the main menu.");
            Assert(!visionMenuItems.Any(item => item.Text == "演示模式"),
                "The removed no-op demo-mode command must not remain reachable.");

            Assert(pauseDirectoryAdvance != null && pauseDirectoryAdvance.Tag is ToolStripButton,
                "Vision > Image must expose the directory-auto-advance state proxy.");
            ToolStripButton pauseDirectorySource = (ToolStripButton)pauseDirectoryAdvance.Tag;
            Assert(pauseDirectorySource.Name == "tsb_stopSwtich" && pauseDirectorySource.CheckOnClick &&
                   HasClickHandler(pauseDirectoryAdvance) && HasClickHandler(pauseDirectorySource),
                "Directory-auto-advance proxy must retain the source CheckOnClick state contract.");
            bool originalPauseState = pauseDirectorySource.Checked;
            try
            {
                RaiseDropDownOpening(visionMenu);
                Assert(pauseDirectoryAdvance.Checked == pauseDirectorySource.Checked,
                    "Opening Vision must synchronize the directory-auto-advance Checked state.");
                pauseDirectoryAdvance.PerformClick();
                Assert(pauseDirectorySource.Checked != originalPauseState,
                    "The directory-auto-advance proxy must toggle its source CheckOnClick state.");
                RaiseDropDownOpening(visionMenu);
                Assert(pauseDirectoryAdvance.Checked == pauseDirectorySource.Checked,
                    "The toggled directory-auto-advance state was not reflected back into its proxy.");
                pauseDirectoryAdvance.PerformClick();
                Assert(pauseDirectorySource.Checked == originalPauseState,
                    "The directory-auto-advance state did not return to its original value.");
            }
            finally
            {
                pauseDirectorySource.Checked = originalPauseState;
                RaiseDropDownOpening(visionMenu);
            }

            Assert(speedModeProxy != null && speedModeProxy.Tag is ToolStripButton &&
                   ((ToolStripButton)speedModeProxy.Tag).Name == "toolStripButton27" &&
                   !speedModeProxy.CheckOnClick && HasClickHandler(speedModeProxy) &&
                   HasClickHandler((ToolStripItem)speedModeProxy.Tag),
                "Vision > Utilities must expose the executable high-speed-mode proxy without local state drift.");
            ToolStripMenuItem utilityMenu = visionMenu.DropDownItems
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(item => item.Text == "辅助工具");
            PropertyInfo speedMode = configurationType.GetProperty(
                "SpeedMode",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Assert(utilityMenu != null && speedMode != null,
                "High-speed-mode synchronization metadata is missing.");
            bool originalSpeedMode = (bool)speedMode.GetValue(null, null);
            try
            {
                speedMode.SetValue(null, !originalSpeedMode, null);
                RaiseDropDownOpening(utilityMenu);
                Assert(speedModeProxy.Checked == (bool)speedMode.GetValue(null, null),
                    "Opening Vision > Utilities must synchronize the high-speed-mode Checked state.");
            }
            finally
            {
                speedMode.SetValue(null, originalSpeedMode, null);
                RaiseDropDownOpening(utilityMenu);
            }

            ToolStripButton startButton = GetField<ToolStripButton>(mainType, form, "toolStripButton4");
            Assert(startButton.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText, "Primary commands must expose icon and text.");
            Assert(primaryBar.ShowItemToolTips, "Primary command tooltips must be enabled.");
            string[] expectedPrimaryNames =
            {
                "toolStripButton4", "toolStripButton36", "toolStripButton3", "toolStripButton8",
                "toolStripButton1", "toolStripButton5", "toolStripButton9"
            };
            ToolStripButton[] primaryButtons = primaryBar.Items.OfType<ToolStripButton>()
                .Where(item => item.Available)
                .ToArray();
            string[] primaryNames = primaryButtons.Select(item => item.Name).ToArray();
            Assert(primaryNames.SequenceEqual(expectedPrimaryNames),
                "Global command bar must contain only machine commands and workspace navigation: " +
                string.Join(",", primaryNames));
            Assert(primaryButtons.Length == 7 &&
                   primaryButtons.Select(item => item.Text).SequenceEqual(
                       new[] { "启动", "暂停", "停止", "复位", "运动", "视觉", "首页" }),
                "Global command bar must retain exactly seven visible business buttons.");
            string[] renderedPrimaryText = primaryButtons
                .OrderBy(item => item.Bounds.Left)
                .Select(item => item.Text)
                .ToArray();
            Assert(renderedPrimaryText.SequenceEqual(
                       new[] { "启动", "暂停", "停止", "复位", "首页", "视觉", "运动" }),
                "Rendered primary command order is invalid: " + string.Join(",", renderedPrimaryText));
            Assert(primaryButtons.All(HasClickHandler),
                "Every visible primary business button must retain an executable Click handler.");
            ToolStripButton resetButton = GetField<ToolStripButton>(mainType, form, "toolStripButton8");
            int firstWorkspaceLeft = primaryButtons.Where(item => item.Alignment == ToolStripItemAlignment.Right)
                .Min(item => item.Bounds.Left);
            Assert(menu.Left >= resetButton.Bounds.Right && menu.Right <= firstWorkspaceLeft,
                "Integrated menus overlap the machine commands or workspace navigation.");

            VerifyRunShortcutStateGates(mainType, form);

            ToolStripButton[] visionPrimaryButtons = contextBar.Items.OfType<ToolStripButton>()
                .Where(item => item.Available)
                .ToArray();
            string[] expectedVisionPrimaryNames =
            {
                "toolStripButton11", "toolStripButton12", "toolStripButton13", "toolStripButton23"
            };
            string[] expectedVisionPrimaryText = { "单次运行", "连续运行", "保存项目", "读取图像" };
            Assert(visionPrimaryButtons.Length == 4,
                "Vision command bar must expose exactly four high-frequency actions directly.");
            Assert(visionPrimaryButtons.Select(item => item.Name).SequenceEqual(expectedVisionPrimaryNames),
                "Unexpected high-frequency vision command order: " +
                string.Join(",", visionPrimaryButtons.Select(item => item.Name).ToArray()));
            Assert(visionPrimaryButtons.Select(item => item.Text).SequenceEqual(expectedVisionPrimaryText),
                "Unexpected high-frequency vision command labels: " +
                string.Join(",", visionPrimaryButtons.Select(item => item.Text).ToArray()));
            Assert(visionPrimaryButtons.All(item => item.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText &&
                                                    item.Image != null &&
                                                    !string.IsNullOrWhiteSpace(item.ToolTipText)),
                "Each high-frequency vision command must keep an icon, visible text, and tooltip.");

            ToolStripDropDownButton[] visionGroups = contextBar.Items.OfType<ToolStripDropDownButton>()
                .Where(item => item.Available)
                .ToArray();
            string[] expectedVisionGroupNames = { "visionBatchGroup" };
            string[] expectedVisionGroupText = { "批量运行" };
            Assert(visionGroups.Length == 1,
                "Vision command bar must expose only one contextual secondary menu.");
            Assert(visionGroups.Select(item => item.Name).SequenceEqual(expectedVisionGroupNames),
                "Unexpected vision secondary menu: " +
                string.Join(",", visionGroups.Select(item => item.Name).ToArray()));
            Assert(visionGroups.Select(item => item.Text).SequenceEqual(expectedVisionGroupText),
                "Unexpected vision secondary menu label: " +
                string.Join(",", visionGroups.Select(item => item.Text).ToArray()));
            Assert(visionGroups.All(group => group.DropDownItems.OfType<ToolStripMenuItem>().Any()),
                "The batch-run menu must contain executable actions.");
            ToolStripMenuItem[] batchActions = visionGroups[0].DropDownItems
                .OfType<ToolStripMenuItem>()
                .ToArray();
            Assert(batchActions.Length == 2 && batchActions.All(action => action.Tag is ToolStripItem),
                "Batch-run actions must retain both original command sources.");
            Assert(
                   batchActions.Select(action => ((ToolStripItem)action.Tag).Name)
                       .SequenceEqual(new[] { "toolStripButton35", "toolStripButton16" }),
                "Batch-run actions must retain both original command sources.");
            Assert(batchActions.All(action => action.Tag is ToolStripItem &&
                                              HasClickHandler(action) &&
                                              HasClickHandler((ToolStripItem)action.Tag)),
                "Every batch-run proxy must retain an executable Click/source chain.");

            VerifyAndRenderVectorIcons(
                assembly,
                primaryBar,
                contextBar,
                primaryButtons,
                visionPrimaryButtons,
                visionGroups[0],
                Path.GetDirectoryName(previewPath) ?? string.Empty);

            string directory = Path.GetDirectoryName(previewPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            using (Bitmap bitmap = new Bitmap(form.ClientSize.Width, form.ClientSize.Height))
            {
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                RenderControl(bitmap, status);
                RenderControl(bitmap, contextBar);
                RenderControl(bitmap, header);
                RenderControl(bitmap, menu);
                bitmap.Save(previewPath, ImageFormat.Png);
            }

            object homeMode = Enum.Parse(formModeType, "MainForm");
            applyWorkspace.Invoke(form, new[] { homeMode });
            form.PerformLayout();
            Panel homePanel = GetField<Panel>(mainType, form, "panel2");
            Assert(IsLocallyVisible(homePanel) && !IsLocallyVisible(dock), "Home workspace visibility is invalid.");
            Assert(homePanel.Controls.Find("modernDashboard", true).Length == 1, "Home dashboard was not embedded.");

            string homePreviewPath = Path.Combine(
                Path.GetDirectoryName(previewPath) ?? string.Empty,
                "main-home-1280x800.png");
            using (Bitmap bitmap = new Bitmap(form.ClientSize.Width, form.ClientSize.Height))
            {
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                RenderControl(bitmap, homePanel);
                RenderControl(bitmap, status);
                RenderControl(bitmap, header);
                RenderControl(bitmap, menu);
                bitmap.Save(homePreviewPath, ImageFormat.Png);
            }

            applyWorkspace.Invoke(form, new[] { visionMode });
            form.PerformLayout();

            VerifyPendingJobSelectionSurvivesVisionNavigation(
                assembly,
                mainType,
                form,
                applyWorkspace,
                homeMode,
                visionMode);

            form.Size = form.MinimumSize;
            form.PerformLayout();
            Assert(dock.Width > 0 && dock.Height > 0, "Content area collapses at the minimum window size.");
            Assert(dock.Top >= contextBar.Bottom && dock.Bottom <= status.Top, "Minimum-size content overlaps the shell bars.");

                Console.WriteLine("header={0}; menu={1}; context={2}; content={3}; status={4}",
                    header.Bounds,
                    menu.Bounds,
                    contextBar.Bounds,
                    dock.Bounds,
                    status.Bounds);
            }
        }
        finally
        {
            Control.CheckForIllegalCrossThreadCalls = originalCrossThreadCheck;
        }

        Type welcomeType = assembly.GetType("VMPro.Frm_Welcome", true);
        using (Form welcome = (Form)Activator.CreateInstance(welcomeType, true))
        {
            welcome.CreateControl();
            SetLocalVisible(welcome, true);
            Label company = GetField<Label>(welcomeType, welcome, "lbl_companyName");
            Label version = GetField<Label>(welcomeType, welcome, "lbl_version");
            Label step = GetField<Label>(welcomeType, welcome, "lbl_step");
            Label productTitle = FindControl<Label>(welcome, "welcomeProductTitle");
            Label brandMark = FindControl<Label>(welcome, "welcomeBrandMark");
            Control illustration = FindControl<Control>(welcome, "welcomeIllustration");
            Control progress = FindControl<Control>(welcome, "welcomeProgress");
            step.Text = "正在加载界面资源...";
            Assert(company.Text == "威乐普电子科技有限公司",
                "Welcome screen must show the normalized company name.");
            Assert(productTitle.Text == "WLP VM v1.0.0" &&
                   version.Text == "版本 1.0.0" && !version.Text.Contains("2026"),
                "Welcome screen must expose release 1.0.0 instead of the dated build identifier.");
            Assert(welcome.BackColor == Color.FromArgb(252, 250, 246),
                "Welcome screen must use the warm-white background.");
            double brandBlueCoverage = RenderAndMeasureControlColor(
                brandMark,
                Color.FromArgb(84, 166, 232),
                8,
                Path.Combine(Path.GetDirectoryName(previewPath) ?? string.Empty, "welcome-brandmark-52x42.png"),
                "welcome_brandmark");
            Assert(brandMark.Text == "WLP" && brandMark.Width >= 52 && brandMark.Region == null &&
                   brandMark.BackColor == Color.Transparent && brandMark.ForeColor == Color.White &&
                   brandBlueCoverage >= 0.45D,
                "Welcome brand mark must use the matching soft-blue accent.");
            Assert(illustration.Visible && progress.Visible,
                "The code-rendered welcome illustration and progress indicator must remain visible.");
            Assert(!GetField<PictureBox>(welcomeType, welcome, "pictureBox1").Visible &&
                   !GetField<PictureBox>(welcomeType, welcome, "pictureBox2").Visible &&
                   !GetField<ProgressBar>(welcomeType, welcome, "bar_step").Visible,
                "Legacy blue-background welcome assets must remain hidden.");
            Assert(welcome.ClientSize.Width >= 680 && welcome.ClientSize.Height >= 380 && welcome.Region != null,
                "Welcome screen must keep its modern minimum canvas and rounded silhouette.");
            Assert(welcome.AutoScaleMode == AutoScaleMode.Font,
                "Welcome must retain the designer's Font scaling baseline until the legacy absolute layout is DPI-migrated.");
            Assert(company.Font.FontFamily.Name == "Microsoft YaHei UI", "Welcome title font is not normalized.");
            Assert(productTitle.Font.FontFamily.Name == "Microsoft YaHei UI",
                "Welcome product title font is not normalized.");

            string welcomePreviewPath = Path.Combine(
                Path.GetDirectoryName(previewPath) ?? string.Empty,
                "welcome-warm-blue.png");
            using (Bitmap bitmap = new Bitmap(welcome.ClientSize.Width, welcome.ClientSize.Height))
            {
                welcome.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                bitmap.Save(welcomePreviewPath, ImageFormat.Png);
            }
        }

        VerifyAndRenderSaveImageModule(assembly, Path.GetDirectoryName(previewPath) ?? string.Empty);
        VerifyAndRenderModernInputGeometry(assembly, Path.GetDirectoryName(previewPath) ?? string.Empty);
    }

    private static void VerifyPendingJobSelectionSurvivesVisionNavigation(
        Assembly assembly,
        Type mainType,
        Form form,
        MethodInfo applyWorkspace,
        object homeMode,
        object visionMode)
    {
        Type jobType = assembly.GetType("VMPro.Job", true);
        MethodInfo afterSelect = jobType.GetMethod(
            "TVW_AfterSelect",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo delayOnUiThread = jobType.GetMethod(
            "DelayOnUiThread",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo threadStart = typeof(Thread).GetMethod("Start", Type.EmptyTypes);
        FieldInfo delayTimerField = jobType.GetField(
            "afterSelectDelayTimer",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert(afterSelect != null && delayOnUiThread != null,
            "Job selection must expose its UI-thread debounce path.");
        Assert(threadStart != null && !MethodBodyContainsDirectCall(afterSelect, threadStart),
            "Job.TVW_AfterSelect must not start a worker that touches WinForms controls.");
        Assert(delayTimerField != null &&
               delayTimerField.FieldType == typeof(System.Windows.Forms.Timer),
            "Job selection debounce must remain rooted in a WinForms UI timer.");

        bool originalCrossThreadCheck = Control.CheckForIllegalCrossThreadCalls;
        Exception uiThreadException = null;
        ThreadExceptionEventHandler threadExceptionHandler = delegate(object sender, ThreadExceptionEventArgs e)
        {
            uiThreadException = e.Exception;
        };

        Control.CheckForIllegalCrossThreadCalls = true;
        Application.ThreadException += threadExceptionHandler;
        try
        {
            object job = Activator.CreateInstance(jobType, true);
            using (TreeView tree = new TreeView())
            {
                tree.Size = new Size(240, 180);
                IntPtr treeHandle = tree.Handle;
                TreeNode first = tree.Nodes.Add("smoke-selection-a");
                TreeNode second = tree.Nodes.Add("smoke-selection-b");

                tree.SelectedNode = first;
                afterSelect.Invoke(job, new object[] { tree, new TreeViewEventArgs(first) });
                applyWorkspace.Invoke(form, new[] { homeMode });

                tree.SelectedNode = second;
                afterSelect.Invoke(job, new object[] { tree, new TreeViewEventArgs(second) });
                applyWorkspace.Invoke(form, new[] { visionMode });

                DateTime deadline = DateTime.UtcNow.AddMilliseconds(900);
                while (DateTime.UtcNow < deadline)
                {
                    Application.DoEvents();
                    Thread.Sleep(5);
                }

                ToolStrip contextBar = GetField<ToolStrip>(mainType, form, "toolStrip2");
                DockPanel dock = GetField<DockPanel>(mainType, form, "dockPanel");
                ToolStripButton visionButton = GetField<ToolStripButton>(mainType, form, "toolStripButton5");
                Assert(uiThreadException == null,
                    "Pending workflow-tree selection raised an exception while entering Vision.");
                Assert(tree.SelectedNode == second,
                    "A stale delayed workflow-tree callback replaced the latest selection.");
                Assert(visionButton.Checked && IsLocallyVisible(contextBar) && IsLocallyVisible(dock),
                    "Vision navigation did not survive a pending workflow-tree selection callback.");
            }
        }
        finally
        {
            Application.ThreadException -= threadExceptionHandler;
            Control.CheckForIllegalCrossThreadCalls = originalCrossThreadCheck;
        }
    }

    private static void VerifyAndRenderSaveImageModule(Assembly assembly, string outputDirectory)
    {
        Type jobType = assembly.GetType("VMPro.Frm_Job", true);
        PropertyInfo jobInstanceProperty = jobType.GetProperty(
            "Instance",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(jobInstanceProperty != null, "Workflow editor singleton is missing.");
        object jobInstance = jobInstanceProperty.GetValue(null, null);
        TabControl jobTabs = GetField<TabControl>(jobType, jobInstance, "tbc_jobs");
        int originalSelectedIndex = jobTabs.SelectedIndex;
        TabPage smokeTab = null;
        if (jobTabs.TabPages.Count == 0)
        {
            smokeTab = new TabPage("UI-SMOKE-ONLY");
            jobTabs.TabPages.Add(smokeTab);
        }
        if (jobTabs.SelectedTab == null)
            jobTabs.SelectedIndex = 0;

        Type saveImageType = assembly.GetType("VMPro.Frm_SaveImageTool", true);
        VerifySaveImageSafetyContracts(assembly, saveImageType);
        try
        {
            using (Form form = (Form)Activator.CreateInstance(saveImageType, true))
            {
                form.ClientSize = new Size(760, 610);
                form.CreateControl();

                // Only invoke the shared form-theme load path. This does not bind a job, run a tool,
                // browse/delete files, or initialize any acquisition/motion hardware.
                MethodInfo onLoad = saveImageType.GetMethod(
                    "OnLoad",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Assert(onLoad != null, "Save-image form theme load path is missing.");
                onLoad.Invoke(form, new object[] { EventArgs.Empty });
                MethodInfo refreshEnableState = saveImageType.GetMethod(
                    "RefreshEnableState",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Assert(refreshEnableState != null, "Save-image enable-state presenter is missing.");
                refreshEnableState.Invoke(form, new object[] { true });

                SetLocalVisible(form, true);
                form.PerformLayout();
                Application.DoEvents();

                string[] cardNames =
                {
                    "saveImageSourceCard", "saveImageStorageCard", "saveImageFileCard"
                };
                TableLayoutPanel sectionLayout = FindControl<TableLayoutPanel>(form, "saveImageSections");
                Control[] cards = cardNames.Select(name => FindControl<Control>(form, name)).ToArray();
                Assert(sectionLayout.Controls.OfType<Panel>().Count(card => cardNames.Contains(card.Name)) == 3 &&
                       cards.All(card => ReferenceEquals(card.Parent, sectionLayout)) &&
                       Enumerable.Range(0, cards.Length).All(index => sectionLayout.GetRow(cards[index]) == index),
                    "Save-image module must contain the three ordered source/storage/file cards.");
                foreach (Control card in cards)
                    AssertRoundedPaintedSurface(card, Color.FromArgb(255, 254, 250),
                        "Save-image card " + card.Name);

                string[] legacyPictureProxyNames =
                {
                    "pictureBox2", "pictureBox4", "pictureBox5", "pictureBox6", "pictureBox7", "pictureBox8"
                };
                PictureBox[] legacyPictureProxies = legacyPictureProxyNames
                    .Select(name => GetField<PictureBox>(saveImageType, form, name))
                    .ToArray();
                Assert(legacyPictureProxies.All(proxy => !proxy.Visible && proxy.Parent == null),
                    "Legacy picture-based toggles must stay hidden behind native controls.");

                Panel page = GetField<Panel>(saveImageType, form, "pnl_formBox");
                Label moduleTitle = GetField<Label>(saveImageType, form, "lbl_title");
                Button enableToggle = FindControl<Button>(form, "btnSaveImageEnable");
                Button runTool = GetField<Button>(saveImageType, form, "btn_runTool");
                Button runToHere = GetField<Button>(saveImageType, form, "btn_confirm");
                Button close = GetField<Button>(saveImageType, form, "btn_cancel");
                Button clear = GetField<Button>(saveImageType, form, "button4");
                Button browse = GetField<Button>(saveImageType, form, "btn_drawTemplateRegionRectangle1");
                Button reset = FindControl<Button>(form, "btnResetSaveImage");
                Control pathInput = GetField<Control>(saveImageType, form, "tbx_imageSavePath");
                Control nameInput = GetField<Control>(saveImageType, form, "textBox2");
                Control formatInput = GetField<Control>(saveImageType, form, "comboBox1");
                Control retentionInput = GetField<Control>(saveImageType, form, "textBox1");
                Control[] customInputs =
                {
                    pathInput, nameInput, formatInput, retentionInput
                };
                Color warmPage = Color.FromArgb(248, 247, 243);
                Color softBlue = Color.FromArgb(232, 243, 252);
                Color accent = Color.FromArgb(76, 148, 210);
                Color dangerSoft = Color.FromArgb(252, 239, 237);

                Assert(form.MinimumSize.Width >= 640 && form.MinimumSize.Height >= 540,
                    "Save-image module minimum size is too small for its three-card layout.");
                Assert(moduleTitle.Text == "存储图像" && form.Text == "存储图像",
                    "Save-image module must not expose the base-form title placeholder.");
                Assert(form.ClientSize == new Size(760, 610) && page.BackColor == warmPage,
                    "Save-image module must keep the 760x610 warm-white validation canvas.");
                Assert(form.Region != null,
                    "The large save-image shell must retain its rounded window silhouette.");
                AssertRoundedPaintedSurface(runTool, accent, "Save-image run action");
                AssertRoundedPaintedSurface(runToHere, Color.FromArgb(255, 254, 250), "Save-image run-to-here action");
                AssertRoundedPaintedSurface(close, Color.FromArgb(255, 254, 250), "Save-image close action");
                AssertRoundedPaintedSurface(clear, dangerSoft, "Save-image clear action");
                AssertRoundedPaintedSurface(enableToggle, softBlue, "Save-image enabled-state action");
                Assert(customInputs.All(input => input.Region == null &&
                                                 input.Controls.Find("lbl_line", true).Cast<Control>().All(line => !line.Visible)),
                    "Save-image custom inputs must paint anti-aliased rounded borders without Region clipping or the legacy underline.");
                Assert(pathInput.Height >= 30 && nameInput.Height >= 30 && formatInput.Height >= 30,
                    "Save-image text and format inputs must retain a usable 30-pixel height.");
                int[] storageTabOrder =
                {
                    pathInput.TabIndex, browse.TabIndex, nameInput.TabIndex,
                    formatInput.TabIndex, retentionInput.TabIndex, reset.TabIndex
                };
                Assert(storageTabOrder.SequenceEqual(new[] { 0, 1, 2, 3, 4, 5 }),
                    "Save-image storage controls must follow the visual keyboard order: " +
                    string.Join(",", storageTabOrder.Select(index => index.ToString()).ToArray()));

                ComboBox nativeFormatSelector = GetField<ComboBox>(formatInput.GetType(), formatInput, "cbx_item");
                Assert(nativeFormatSelector.Items.Count >= 2,
                    "Save-image format selector must expose multiple formats.");
                int originalFormatIndex = nativeFormatSelector.SelectedIndex;
                int smokeFormatIndex = originalFormatIndex == 1 ? 2 : 1;
                if (smokeFormatIndex >= nativeFormatSelector.Items.Count)
                    smokeFormatIndex = 0;
                nativeFormatSelector.SelectedIndex = smokeFormatIndex;
                Application.DoEvents();
                FieldInfo saveImageModelField = saveImageType.GetField(
                    "saveImageTool",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                Assert(saveImageModelField != null, "Save-image presenter model field is missing.");
                object saveImageModel = saveImageModelField.GetValue(null);
                FieldInfo imageFormatField = saveImageModel.GetType().GetField(
                    "imageFormat",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Assert(imageFormatField != null &&
                       string.Equals((string)imageFormatField.GetValue(saveImageModel),
                           nativeFormatSelector.Items[smokeFormatIndex].ToString(), StringComparison.Ordinal),
                    "Changing the visible image format must update SaveImageTool.imageFormat.");
                nativeFormatSelector.SelectedIndex = originalFormatIndex;
                Application.DoEvents();
                Assert(ImageCenterMatches(runTool.BackgroundImage, accent, 4) &&
                       ImageCenterMatches(runToHere.BackgroundImage, Color.FromArgb(255, 254, 250), 4),
                    "Save-image footer must distinguish the primary run action from the secondary run-to-here action.");
                Assert(ImageCenterMatches(clear.BackgroundImage, dangerSoft, 4) &&
                       ImageCenterMatches(close.BackgroundImage, Color.FromArgb(255, 254, 250), 4),
                    "Destructive and close actions must retain distinct soft semantic colors.");

                if (!string.IsNullOrEmpty(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);
                string previewPath = Path.Combine(outputDirectory, "save-image-module-760x610.png");
                using (Bitmap bitmap = new Bitmap(760, 610))
                {
                    form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                    bitmap.Save(previewPath, ImageFormat.Png);
                }
            }
        }
        finally
        {
            if (smokeTab != null)
            {
                jobTabs.TabPages.Remove(smokeTab);
                smokeTab.Dispose();
            }
            if (originalSelectedIndex >= 0 && originalSelectedIndex < jobTabs.TabPages.Count)
                jobTabs.SelectedIndex = originalSelectedIndex;
            else
                jobTabs.SelectedIndex = -1;
        }
    }

    private static void VerifySaveImageSafetyContracts(Assembly assembly, Type saveImageFormType)
    {
        MethodInfo fullyQualified = saveImageFormType.GetMethod(
            "IsFullyQualifiedWindowsPath",
            BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo protectedTarget = saveImageFormType.GetMethod(
            "IsProtectedClearTarget",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert(fullyQualified != null && protectedTarget != null,
            "Save-image clear-path safety helpers are missing.");
        Assert(!(bool)fullyQualified.Invoke(null, new object[] { @".\images" }) &&
               (bool)fullyQualified.Invoke(null, new object[] { @"D:\images" }),
            "Save-image clear action must reject relative paths and accept a fully-qualified drive path.");

        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        object[] desktopArgs = { desktop, string.Empty };
        Assert((bool)protectedTarget.Invoke(null, desktopArgs) &&
               !string.IsNullOrWhiteSpace(desktopArgs[1] as string),
            "Save-image clear action must reject the Desktop root with an explicit reason.");
        object[] dedicatedChildArgs = { Path.Combine(desktop, "WLP VM 图像", "UI-SMOKE-ONLY"), string.Empty };
        Assert(!(bool)protectedTarget.Invoke(null, dedicatedChildArgs),
            "The dedicated WLP VM image subfolder must remain a valid clear target.");
        object[] applicationArgs = { Application.StartupPath, string.Empty };
        Assert((bool)protectedTarget.Invoke(null, applicationArgs),
            "Save-image clear action must reject the current application directory.");

        Type saveImageModelType = assembly.GetType("VMPro.SaveImageTool", true);
        MethodInfo normalizeFormat = saveImageModelType.GetMethod(
            "NormalizeImageFormat",
            BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo executeWhenIdle = saveImageModelType.GetMethod(
            "TryExecuteDirectoryOperationWhenIdle",
            BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo hasPending = saveImageModelType.GetMethod(
            "HasPendingSaveUnder",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert(normalizeFormat != null && executeWhenIdle != null && hasPending != null,
            "Save-image scheduler safety contracts are missing.");
        Assert((string)normalizeFormat.Invoke(null, new object[] { "TIFF" }) == "tif" &&
               (string)normalizeFormat.Invoke(null, new object[] { "PNG" }) == "png" &&
               (string)normalizeFormat.Invoke(null, new object[] { @"..\bad" }) == "tif",
            "Save-image format normalization must whitelist tif/bmp/jpg/png and fall back to tif.");

        string idlePath = Path.Combine(desktop, "WLP VM 图像", "UI-SMOKE-ONLY");
        bool operationInvoked = false;
        Action operation = delegate { operationInvoked = true; };
        bool executed = (bool)executeWhenIdle.Invoke(null, new object[] { idlePath, operation });
        Assert(executed && operationInvoked && !(bool)hasPending.Invoke(null, new object[] { idlePath }),
            "Save-image idle directory operation must execute atomically when no save is pending.");
    }

    private static void VerifyAndRenderModernInputGeometry(Assembly vmAssembly, string outputDirectory)
    {
        string vmDirectory = Path.GetDirectoryName(vmAssembly.Location) ?? string.Empty;
        string controlsPath = Path.Combine(vmDirectory, "Controls.dll");
        Assert(File.Exists(controlsPath), "The isolated Controls.dll dependency is missing.");

        Assembly controlsAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(candidate => string.Equals(
                candidate.GetName().Name,
                "Controls",
                StringComparison.OrdinalIgnoreCase));
        if (controlsAssembly == null)
            controlsAssembly = Assembly.LoadFrom(controlsPath);

        Type modernInputType = controlsAssembly.GetType("Controls.ModernInputControl", true);
        Dictionary<string, Type> inputTypes = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "CTextBox", controlsAssembly.GetType("Controls.CTextBox", true) },
            { "CComboBox", controlsAssembly.GetType("Controls.CComboBox", true) },
            { "CNumeric", controlsAssembly.GetType("Controls.CNumeric", true) },
            { "CNumericUpDown", controlsAssembly.GetType("Controls.CNumericUpDown", true) }
        };
        Assert(inputTypes.Values.All(type => modernInputType.IsAssignableFrom(type)),
            "All four custom inputs must share the anti-aliased ModernInputControl base.");
        VerifyComboBoxSelectionSynchronization(inputTypes["CComboBox"]);
        VerifyTextBoxAtomicEvents(inputTypes["CTextBox"]);
        VerifyNumericAtomicEvents(inputTypes["CNumeric"]);
        VerifyModernInputNarrowGeometry(inputTypes);
        VerifyModernInputHeightMatrix(inputTypes, outputDirectory);

        if (!string.IsNullOrEmpty(outputDirectory))
            Directory.CreateDirectory(outputDirectory);
        RenderModernInputGeometryScale(inputTypes, modernInputType, 1F, outputDirectory, "modern-input-geometry-100.png");
        RenderModernInputGeometryScale(inputTypes, modernInputType, 1.5F, outputDirectory, "modern-input-geometry-150.png");
    }

    private static void VerifyComboBoxSelectionSynchronization(Type comboBoxType)
    {
        using (Control combo = (Control)Activator.CreateInstance(comboBoxType, true))
        {
            PropertyInfo items = comboBoxType.GetProperty("Items", BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo selectedIndex = comboBoxType.GetProperty("SelectedIndex", BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo text = comboBoxType.GetProperty("TextStr", BindingFlags.Instance | BindingFlags.Public);
            ComboBox nativeCombo = GetField<ComboBox>(comboBoxType, combo, "cbx_item");
            Assert(items != null && selectedIndex != null && text != null,
                "CComboBox selection properties are incomplete.");

            string[] values = { "从明到暗", "从暗到明", "所有点" };
            items.SetValue(combo, values, null);
            selectedIndex.SetValue(combo, 1, null);
            Assert((int)selectedIndex.GetValue(combo, null) == 1 &&
                   (string)text.GetValue(combo, null) == values[1] &&
                   nativeCombo.SelectedIndex == 1,
                "CComboBox SelectedIndex must immediately synchronize TextStr and its hidden native model.");

            text.SetValue(combo, values[2], null);
            Assert((int)selectedIndex.GetValue(combo, null) == 2 &&
                   (string)text.GetValue(combo, null) == values[2] &&
                   nativeCombo.SelectedIndex == 2,
                "CComboBox TextStr must immediately synchronize SelectedIndex and its hidden native model.");

            nativeCombo.SelectedIndex = 0;
            Assert((int)selectedIndex.GetValue(combo, null) == 0 &&
                   (string)text.GetValue(combo, null) == values[0],
                "A native selection update must synchronize the public CComboBox properties.");
        }
    }

    private static void VerifyTextBoxAtomicEvents(Type textBoxType)
    {
        using (Control input = (Control)Activator.CreateInstance(textBoxType, true))
        {
            PropertyInfo text = textBoxType.GetProperty("TextStr", BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo defaultText = textBoxType.GetProperty("DefaultText", BindingFlags.Instance | BindingFlags.Public);
            EventInfo changed = textBoxType.GetEvent("TextStrChanged", BindingFlags.Instance | BindingFlags.Public);
            TextBox nativeEditor = GetField<TextBox>(textBoxType, input, "tbx_text");
            Assert(text != null && defaultText != null && changed != null,
                "CTextBox atomic text contract metadata is incomplete.");

            Delegate listener = Delegate.CreateDelegate(
                changed.EventHandlerType,
                typeof(UiShellSmoke).GetMethod("CaptureTextChange", BindingFlags.Static | BindingFlags.NonPublic));
            changed.AddEventHandler(input, listener);
            try
            {
                defaultText.SetValue(input, "请输入变量名", null);
                capturedTextChangeCount = 0;
                capturedTextChangeValue = null;
                text.SetValue(input, "Line_Result", null);
                Assert(capturedTextChangeCount == 1 && capturedTextChangeValue == "Line_Result" &&
                       (string)text.GetValue(input, null) == "Line_Result" && nativeEditor.Text == "Line_Result",
                    "CTextBox programmatic assignment must publish one atomic logical-value event.");

                capturedTextChangeCount = 0;
                text.SetValue(input, "Line_Result", null);
                Assert(capturedTextChangeCount == 0,
                    "CTextBox must not publish an event when its logical value is unchanged.");

                capturedTextChangeCount = 0;
                capturedTextChangeValue = "sentinel";
                text.SetValue(input, string.Empty, null);
                Assert(capturedTextChangeCount == 1 && capturedTextChangeValue == string.Empty &&
                       (string)text.GetValue(input, null) == string.Empty && nativeEditor.Text == "请输入变量名",
                    "CTextBox clear must publish only the empty logical value while rendering its placeholder.");

                capturedTextChangeCount = 0;
                defaultText.SetValue(input, "变量名称", null);
                Assert(capturedTextChangeCount == 0 && nativeEditor.Text == "变量名称",
                    "Changing an empty CTextBox placeholder must not masquerade as a value change.");

                capturedTextChangeCount = 0;
                nativeEditor.Text = "Circle_Result";
                Assert(capturedTextChangeCount == 1 && capturedTextChangeValue == "Circle_Result" &&
                       (string)text.GetValue(input, null) == "Circle_Result",
                    "CTextBox user editing must publish one synchronized logical-value event.");

                capturedTextChangeCount = 0;
                nativeEditor.Text = string.Empty;
                Assert(capturedTextChangeCount == 1 && capturedTextChangeValue == string.Empty &&
                       nativeEditor.Text == "变量名称",
                    "Deleting CTextBox text must publish one empty event, not an old-value/empty pair.");
            }
            finally
            {
                changed.RemoveEventHandler(input, listener);
            }
        }
    }

    private static void VerifyNumericAtomicEvents(Type numericType)
    {
        using (Control input = (Control)Activator.CreateInstance(numericType, true))
        {
            PropertyInfo value = numericType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
            EventInfo changed = numericType.GetEvent("ValueChanged", BindingFlags.Instance | BindingFlags.Public);
            TextBox nativeEditor = GetField<TextBox>(numericType, input, "tbx_value");
            Assert(value != null && changed != null,
                "CNumeric atomic numeric contract metadata is incomplete.");

            Delegate listener = Delegate.CreateDelegate(
                changed.EventHandlerType,
                typeof(UiShellSmoke).GetMethod("CaptureNumericChange", BindingFlags.Static | BindingFlags.NonPublic));
            changed.AddEventHandler(input, listener);
            try
            {
                capturedNumericChangeCount = 0;
                value.SetValue(input, "12.50", null);
                Assert(capturedNumericChangeCount == 1 && Math.Abs(capturedNumericChangeValue - 12.5D) < 0.000001D &&
                       (string)value.GetValue(input, null) == "12.50" && nativeEditor.Text == "12.50",
                    "CNumeric programmatic assignment must publish one valid numeric event.");

                capturedNumericChangeCount = 0;
                value.SetValue(input, "12.500", null);
                Assert(capturedNumericChangeCount == 0 && nativeEditor.Text == "12.500",
                    "CNumeric equivalent formatting must not publish a duplicate numeric event.");

                capturedNumericChangeCount = 0;
                value.SetValue(input, "-", null);
                Assert(capturedNumericChangeCount == 0 && (string)value.GetValue(input, null) == "-" && nativeEditor.Text == "-",
                    "CNumeric must accept the minus-sign edit state without conversion or notification.");

                capturedNumericChangeCount = 0;
                value.SetValue(input, string.Empty, null);
                Assert(capturedNumericChangeCount == 0 && (string)value.GetValue(input, null) == string.Empty,
                    "CNumeric must accept an empty edit state without conversion or notification.");

                capturedNumericChangeCount = 0;
                nativeEditor.Text = "-";
                Assert(capturedNumericChangeCount == 0,
                    "CNumeric user minus-sign intermediate state must not publish an invalid numeric event.");
                nativeEditor.Text = "-3.25";
                Assert(capturedNumericChangeCount == 1 && Math.Abs(capturedNumericChangeValue + 3.25D) < 0.000001D &&
                       (string)value.GetValue(input, null) == "-3.25",
                    "CNumeric must commit one event when an intermediate edit becomes a complete number.");

                capturedNumericChangeCount = 0;
                nativeEditor.Text = string.Empty;
                RaiseControlLeave(input);
                Assert(capturedNumericChangeCount == 0 && nativeEditor.Text == "-3.25" &&
                       (string)value.GetValue(input, null) == "-3.25",
                    "CNumeric leaving an incomplete edit must restore its last valid value without a duplicate event.");

                using (Control freshInput = (Control)Activator.CreateInstance(numericType, true))
                {
                    TextBox freshEditor = GetField<TextBox>(numericType, freshInput, "tbx_value");
                    Delegate freshListener = Delegate.CreateDelegate(
                        changed.EventHandlerType,
                        typeof(UiShellSmoke).GetMethod("CaptureNumericChange", BindingFlags.Static | BindingFlags.NonPublic));
                    changed.AddEventHandler(freshInput, freshListener);
                    try
                    {
                        capturedNumericChangeCount = 0;
                        freshEditor.Text = "-";
                        RaiseControlLeave(freshInput);
                        Assert(capturedNumericChangeCount == 1 && Math.Abs(capturedNumericChangeValue) < 0.000001D &&
                               freshEditor.Text == "0" && (string)value.GetValue(freshInput, null) == "0",
                            "A fresh CNumeric must normalize an incomplete value to one safe zero event on leave.");
                    }
                    finally
                    {
                        changed.RemoveEventHandler(freshInput, freshListener);
                    }
                }
            }
            finally
            {
                changed.RemoveEventHandler(input, listener);
            }
        }
    }

    private static void VerifyModernInputNarrowGeometry(IDictionary<string, Type> inputTypes)
    {
        using (Form host = new Form())
        {
            host.FormBorderStyle = FormBorderStyle.None;
            host.AutoScaleMode = AutoScaleMode.None;
            host.ClientSize = new Size(420, 80);
            host.BackColor = Color.FromArgb(248, 247, 243);
            string[] names = { "CTextBox", "CComboBox", "CNumeric", "CNumericUpDown" };
            int[] heights = { 22, 24, 26, 26 };
            List<Control> probes = new List<Control>();
            for (int index = 0; index < names.Length; index++)
            {
                Control input = (Control)Activator.CreateInstance(inputTypes[names[index]], true);
                input.Name = "narrow" + names[index];
                input.MaximumSize = Size.Empty;
                input.MinimumSize = Size.Empty;
                input.Location = new Point(10 + index * 100, 20);
                input.Size = new Size(70, heights[index]);
                SetModernInputDisplayValue(input, names[index] == "CComboBox" ? "所有点" : "100");
                host.Controls.Add(input);
                probes.Add(input);
            }

            host.CreateControl();
            SetLocalVisible(host, true);
            host.PerformLayout();
            foreach (Control input in probes)
            {
                input.CreateControl();
                input.PerformLayout();
                Assert(input.Width == 70,
                    input.GetType().Name + " did not retain the 70-pixel parameter-panel width.");
                ValidateModernInputGeometry(input);
            }
        }
    }

    private static void VerifyModernInputHeightMatrix(
        IDictionary<string, Type> inputTypes,
        string outputDirectory)
    {
        string[] names = { "CTextBox", "CComboBox", "CNumeric", "CNumericUpDown" };
        int[] heights = { 22, 24, 26, 30 };
        using (Form host = new Form())
        {
            host.Name = "modernInputHeightMatrix";
            host.FormBorderStyle = FormBorderStyle.None;
            host.AutoScaleMode = AutoScaleMode.None;
            host.ClientSize = new Size(760, 316);
            host.BackColor = Color.FromArgb(252, 250, 246);
            host.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Label title = new Label();
            title.AutoSize = false;
            title.Location = new Point(18, 12);
            title.Size = new Size(710, 28);
            title.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            title.ForeColor = Color.FromArgb(39, 56, 72);
            title.Text = "WLP VM 输入控件 · 22 / 24 / 26 / 30 px 高度矩阵";
            host.Controls.Add(title);

            List<Control> probes = new List<Control>();
            for (int row = 0; row < names.Length; row++)
            {
                Label rowLabel = new Label();
                rowLabel.AutoSize = false;
                rowLabel.Location = new Point(18, 57 + row * 61);
                rowLabel.Size = new Size(128, 24);
                rowLabel.ForeColor = Color.FromArgb(68, 86, 102);
                rowLabel.Text = names[row];
                host.Controls.Add(rowLabel);

                for (int column = 0; column < heights.Length; column++)
                {
                    int height = heights[column];
                    Label heightLabel = new Label();
                    heightLabel.AutoSize = false;
                    heightLabel.Location = new Point(154 + column * 145, 45 + row * 61);
                    heightLabel.Size = new Size(118, 16);
                    heightLabel.Font = new Font("Microsoft YaHei UI", 7.5F, FontStyle.Regular, GraphicsUnit.Point);
                    heightLabel.ForeColor = Color.FromArgb(112, 126, 138);
                    heightLabel.Text = height + " px";
                    host.Controls.Add(heightLabel);

                    Control input = (Control)Activator.CreateInstance(inputTypes[names[row]], true);
                    input.Name = "height" + height + names[row];
                    input.MaximumSize = Size.Empty;
                    input.MinimumSize = Size.Empty;
                    input.Font = host.Font;
                    input.Location = new Point(154 + column * 145, 62 + row * 61);
                    input.Size = new Size(118, height);
                    SetModernInputDisplayValue(
                        input,
                        names[row] == "CComboBox" ? "所有点" :
                        names[row] == "CTextBox" ? "Line_Result" : "-0.50");
                    host.Controls.Add(input);
                    probes.Add(input);
                }
            }

            host.CreateControl();
            SetLocalVisible(host, true);
            host.PerformLayout();
            foreach (Control input in probes)
            {
                input.CreateControl();
                input.PerformLayout();
                int requestedHeight = Convert.ToInt32(
                    input.Name.Substring("height".Length, 2),
                    System.Globalization.CultureInfo.InvariantCulture);
                Assert(input.Height == requestedHeight,
                    input.GetType().Name + " did not retain the requested " + requestedHeight + " px height.");
                ValidateModernInputGeometry(input);
                AssertTextBaselineFits(input);
            }

            if (!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
                using (Bitmap bitmap = new Bitmap(host.ClientSize.Width, host.ClientSize.Height))
                {
                    host.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                    bitmap.Save(Path.Combine(outputDirectory, "modern-input-height-22-24-26-30.png"), ImageFormat.Png);
                }
            }
        }
    }

    private static void AssertTextBaselineFits(Control input)
    {
        TextBox[] visibleEditors = input.Controls
            .OfType<TextBox>()
            .Where(IsLocallyVisible)
            .ToArray();
        foreach (TextBox editor in visibleEditors)
        {
            Assert(editor.Top >= 0 && editor.Bottom <= input.ClientSize.Height,
                input.GetType().Name + " text editor exceeds its owner at " + input.Height + " px: " + editor.Bounds + ".");
            Assert(editor.ClientSize.Height >= editor.PreferredHeight,
                input.GetType().Name + " text editor is shorter than its WinForms PreferredHeight at " +
                input.Height + " px: editor=" + editor.ClientSize.Height + ", preferred=" +
                editor.PreferredHeight + ".");
            AssertEditorTextHasVerticalMargin(input, editor);
        }

        if (input.GetType().Name == "CComboBox" && visibleEditors.Length == 0)
        {
            int glyphHeight = TextRenderer.MeasureText(
                "Ag国0",
                input.Font,
                Size.Empty,
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Height;
            Assert(input.ClientSize.Height - 4 >= glyphHeight,
                "CComboBox painted text baseline is clipped at " + input.Height + " px.");
        }
    }

    private static void AssertEditorTextHasVerticalMargin(Control owner, TextBox editor)
    {
        using (Bitmap bitmap = new Bitmap(editor.Width, editor.Height))
        {
            editor.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
            int firstInkRow = bitmap.Height;
            int lastInkRow = -1;
            int inkPixels = 0;
            Color background = editor.BackColor;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    int difference = Math.Abs(pixel.R - background.R) +
                                     Math.Abs(pixel.G - background.G) +
                                     Math.Abs(pixel.B - background.B);
                    if (difference < 36)
                        continue;
                    inkPixels++;
                    firstInkRow = Math.Min(firstInkRow, y);
                    lastInkRow = Math.Max(lastInkRow, y);
                }
            }
            // Borderless WinForms TextBox uses its PreferredHeight as the native single-line
            // baseline. Glyphs such as '_' can legitimately occupy the last raster row, so
            // clipping is guarded by PreferredHeight + owner bounds; raster QA additionally
            // requires real ink and a clear top margin.
            Assert(inkPixels > 0 && firstInkRow > 0 && lastInkRow < bitmap.Height,
                owner.GetType().Name + " rendered text touches a vertical edge at " + owner.Height +
                " px: inkRows=" + firstInkRow + ".." + lastInkRow + ", editorHeight=" + bitmap.Height + ".");
        }
    }

    private static void RenderModernInputGeometryScale(
        IDictionary<string, Type> inputTypes,
        Type modernInputType,
        float scale,
        string outputDirectory,
        string fileName)
    {
        Color canvasColor = Color.FromArgb(248, 247, 243);
        Color surfaceColor = Color.FromArgb(255, 255, 253);
        Color borderColor = Color.FromArgb(217, 227, 234);
        Color accentColor = Color.FromArgb(76, 148, 210);
        Color textColor = Color.FromArgb(39, 56, 72);
        int canvasWidth = ScalePixel(760, scale);
        int canvasHeight = ScalePixel(330, scale);
        int cardWidth = ScalePixel(350, scale);
        int cardHeight = ScalePixel(244, scale);
        int cardTop = ScalePixel(62, scale);

        using (Font bodyFont = new Font("Microsoft YaHei UI", 9F * scale, FontStyle.Regular, GraphicsUnit.Point))
        using (Font titleFont = new Font("Microsoft YaHei UI", 14F * scale, FontStyle.Bold, GraphicsUnit.Point))
        using (Font cardTitleFont = new Font("Microsoft YaHei UI", 10F * scale, FontStyle.Bold, GraphicsUnit.Point))
        using (Form canvas = new Form())
        {
            canvas.Name = "modernInputGeometryCanvas";
            canvas.FormBorderStyle = FormBorderStyle.None;
            canvas.AutoScaleMode = AutoScaleMode.None;
            canvas.ClientSize = new Size(canvasWidth, canvasHeight);
            canvas.BackColor = canvasColor;
            canvas.Font = bodyFont;
            canvas.ShowInTaskbar = false;

            Label title = new Label();
            title.AutoSize = false;
            title.Location = new Point(ScalePixel(20, scale), ScalePixel(14, scale));
            title.Size = new Size(ScalePixel(700, scale), ScalePixel(34, scale));
            title.Font = titleFont;
            title.ForeColor = textColor;
            title.Text = "WLP VM 输入控件几何 · " + (scale == 1F ? "100%" : "150% 模拟");
            canvas.Controls.Add(title);

            Panel lineCard = CreateInputPreviewCard(
                "findLineInputCard",
                "查找直线参数",
                new Point(ScalePixel(20, scale), cardTop),
                new Size(cardWidth, cardHeight),
                cardTitleFont,
                scale,
                surfaceColor,
                borderColor,
                textColor);
            Panel circleCard = CreateInputPreviewCard(
                "findCircleInputCard",
                "查找圆参数",
                new Point(ScalePixel(390, scale), cardTop),
                new Size(cardWidth, cardHeight),
                cardTitleFont,
                scale,
                surfaceColor,
                borderColor,
                textColor);
            canvas.Controls.Add(lineCard);
            canvas.Controls.Add(circleCard);

            List<Control> inputs = new List<Control>();
            inputs.Add(AddInputPreviewRow(lineCard, "阈值", inputTypes["CNumericUpDown"], "30", 0, scale, bodyFont));
            inputs.Add(AddInputPreviewRow(lineCard, "极性", inputTypes["CComboBox"], "从明到暗", 1, scale, bodyFont));
            inputs.Add(AddInputPreviewRow(lineCard, "起点行", inputTypes["CNumeric"], "125.40", 2, scale, bodyFont));
            inputs.Add(AddInputPreviewRow(lineCard, "输出变量", inputTypes["CTextBox"], "Line_Result", 3, scale, bodyFont));
            inputs.Add(AddInputPreviewRow(circleCard, "卡尺数量", inputTypes["CNumericUpDown"], "32", 0, scale, bodyFont));
            inputs.Add(AddInputPreviewRow(circleCard, "边缘选择", inputTypes["CComboBox"], "所有点", 1, scale, bodyFont));
            inputs.Add(AddInputPreviewRow(circleCard, "预期半径", inputTypes["CNumeric"], "48.00", 2, scale, bodyFont));
            inputs.Add(AddInputPreviewRow(circleCard, "结果半径", inputTypes["CTextBox"], "47.82", 3, scale, bodyFont));

            MethodInfo applyPalette = modernInputType.GetMethod(
                "ApplyModernPalette",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo surfaceField = modernInputType.GetField("surfaceColor", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo borderField = modernInputType.GetField("borderColor", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo focusField = modernInputType.GetField("focusBorderColor", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert(applyPalette != null && surfaceField != null && borderField != null && focusField != null,
                "Modern input palette hooks are missing.");
            foreach (Control input in inputs)
            {
                applyPalette.Invoke(input, new object[] { surfaceColor, borderColor, accentColor, textColor });
                Assert((Color)surfaceField.GetValue(input) == surfaceColor &&
                       (Color)borderField.GetValue(input) == borderColor &&
                       (Color)focusField.GetValue(input) == accentColor,
                    "Custom input did not retain the warm-white/light-blue palette: " + input.GetType().Name);
            }

            canvas.CreateControl();
            SetLocalVisible(canvas, true);
            canvas.PerformLayout();
            foreach (Control input in inputs)
            {
                input.CreateControl();
                input.PerformLayout();
                ValidateModernInputGeometry(input);
            }
            Application.DoEvents();

            Assert(inputs.Select(input => input.GetType().Name).Distinct().Count() == 4,
                "Input geometry preview must include CTextBox, CComboBox, CNumeric, and CNumericUpDown.");
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                using (Bitmap bitmap = new Bitmap(canvasWidth, canvasHeight))
                {
                    bitmap.SetResolution(96F * scale, 96F * scale);
                    canvas.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                    bitmap.Save(Path.Combine(outputDirectory, fileName), ImageFormat.Png);
                }
            }
        }
    }

    private static Panel CreateInputPreviewCard(
        string name,
        string title,
        Point location,
        Size size,
        Font titleFont,
        float scale,
        Color backColor,
        Color borderColor,
        Color textColor)
    {
        Panel card = new Panel();
        card.Name = name;
        card.Location = location;
        card.Size = size;
        card.BackColor = backColor;
        card.Paint += delegate(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(borderColor, Math.Max(1F, scale)))
                e.Graphics.DrawRectangle(pen, 0, 0, Math.Max(0, card.Width - 1), Math.Max(0, card.Height - 1));
        };

        Label heading = new Label();
        heading.AutoSize = false;
        heading.Location = new Point(ScalePixel(18, scale), ScalePixel(12, scale));
        heading.Size = new Size(ScalePixel(300, scale), ScalePixel(28, scale));
        heading.Font = titleFont;
        heading.ForeColor = textColor;
        heading.Text = title;
        card.Controls.Add(heading);
        return card;
    }

    private static Control AddInputPreviewRow(
        Panel card,
        string labelText,
        Type inputType,
        string displayValue,
        int row,
        float scale,
        Font bodyFont)
    {
        int rowTop = ScalePixel(50 + row * 46, scale);
        Label label = new Label();
        label.AutoSize = false;
        label.Location = new Point(ScalePixel(18, scale), rowTop + ScalePixel(5, scale));
        label.Size = new Size(ScalePixel(90, scale), ScalePixel(24, scale));
        label.Font = bodyFont;
        label.ForeColor = Color.FromArgb(68, 86, 102);
        label.Text = labelText;
        card.Controls.Add(label);

        Control input = (Control)Activator.CreateInstance(inputType, true);
        input.Name = "smoke" + inputType.Name + row;
        input.Font = bodyFont;
        input.MaximumSize = new Size(ScalePixel(500, scale), ScalePixel(30, scale));
        input.MinimumSize = new Size(ScalePixel(80, scale), ScalePixel(30, scale));
        input.Location = new Point(ScalePixel(116, scale), rowTop);
        input.Size = new Size(ScalePixel(214, scale), ScalePixel(30, scale));
        SetModernInputDisplayValue(input, displayValue);
        card.Controls.Add(input);
        input.BringToFront();
        return input;
    }

    private static void SetModernInputDisplayValue(Control input, string displayValue)
    {
        Type type = input.GetType();
        if (type.Name == "CComboBox")
        {
            PropertyInfo items = type.GetProperty("Items", BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo text = type.GetProperty("TextStr", BindingFlags.Instance | BindingFlags.Public);
            Assert(items != null && text != null, "CComboBox display properties are missing.");
            items.SetValue(input, new[] { displayValue, "从暗到明", "所有点" }, null);
            text.SetValue(input, displayValue, null);
            return;
        }

        string propertyName = type.Name == "CNumeric" ? "Value" :
                              type.Name == "CNumericUpDown" ? "Value" : "TextStr";
        PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert(property != null, "Custom input display property is missing: " + type.Name + "." + propertyName);
        object value = type.Name == "CNumericUpDown"
            ? (object)Convert.ToDouble(displayValue, System.Globalization.CultureInfo.InvariantCulture)
            : displayValue;
        property.SetValue(input, value, null);
    }

    private static void ValidateModernInputGeometry(Control input)
    {
        Assert(input.Region == null,
            input.GetType().Name + " must use anti-aliased painting instead of a binary Region clip.");
        Control[] legacyUnderlines = input.Controls.Find("lbl_line", true);
        Assert(legacyUnderlines.Length == 1 && legacyUnderlines.All(line => !IsLocallyVisible(line)),
            input.GetType().Name + " must hide its legacy underline at every scale.");

        foreach (Control child in input.Controls.Cast<Control>().Where(IsLocallyVisible))
        {
            Assert(child.Left >= 0 && child.Top >= 0 &&
                   child.Right <= input.ClientRectangle.Right &&
                   child.Bottom <= input.ClientRectangle.Bottom,
                input.GetType().Name + " child escaped its bounds: " + child.Name + "=" + child.Bounds +
                ", owner=" + input.ClientRectangle + ".");
        }

        if (input.GetType().Name == "CComboBox")
        {
            Button dropDown = GetField<Button>(input.GetType(), input, "btn_showItem");
            Control nativeCombo = GetField<Control>(input.GetType(), input, "cbx_item");
            AssertRuntimeVectorButton(dropDown, "CComboBox drop-down");
            Assert(!IsLocallyVisible(nativeCombo) && HasPaintHandler(dropDown),
                "CComboBox must hide the square native selector and paint its drop-down chevron as vector geometry.");
        }
        else if (input.GetType().Name == "CNumericUpDown")
        {
            Button subtract = GetField<Button>(input.GetType(), input, "btn_sub");
            Button add = GetField<Button>(input.GetType(), input, "btn_add");
            Control nativeNumeric = GetField<Control>(input.GetType(), input, "nud_value");
            AssertRuntimeVectorButton(subtract, "CNumericUpDown subtract");
            AssertRuntimeVectorButton(add, "CNumericUpDown add");
            Assert(!IsLocallyVisible(nativeNumeric) && HasPaintHandler(subtract) && HasPaintHandler(add),
                "CNumericUpDown must hide the square native editor and paint both step glyphs as vector geometry.");
        }
        else if (input.GetType().Name == "CTextBox")
        {
            Button eye = GetField<Button>(input.GetType(), input, "btn_eye");
            Assert(eye.Image == null && eye.BackgroundImage == null && HasPaintHandler(eye),
                "CTextBox must not retain the legacy password-eye bitmap.");
        }
    }

    private static void AssertRuntimeVectorButton(Button button, string description)
    {
        Assert(button.Image == null && button.BackgroundImage == null && string.IsNullOrEmpty(button.Text),
            description + " must not retain a legacy bitmap or text glyph.");
    }

    private static int ScalePixel(int value, float scale)
    {
        return Math.Max(1, (int)Math.Round(value * scale));
    }

    private static void VerifyModuleOrganization(string assemblyPath)
    {
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        VerifyStartupUiLaziness(assembly);
        VerifyRuntimeDisplayUiDispatch(assembly);

        Type toolBoxType = assembly.GetType("VMPro.Frm_ToolBox", true);
        using (Form toolBox = (Form)Activator.CreateInstance(toolBoxType, true))
        {
            MethodInfo load = toolBoxType.GetMethod("Frm_Tools_Load", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert(load != null, "Missing toolbox load handler.");
            load.Invoke(toolBox, new object[] { toolBox, EventArgs.Empty });
            TreeView tools = GetField<TreeView>(toolBoxType, toolBox, "tvw_tools");
            Assert(tools.Nodes.Count == 7, "Toolbox must expose seven user-facing categories.");
            Assert(!tools.Nodes.Cast<TreeNode>().Any(node => node.Text.Contains("3D")), "Empty 3D category should be hidden.");
            string toolCategoryNames = string.Join(",", tools.Nodes.Cast<TreeNode>().Select(node => node.Text).ToArray());
            Assert(toolCategoryNames == "图像输入与预处理,检测与识别,标定与定位,几何与 ROI,逻辑与计算,设备与通信,输出与显示",
                "Unexpected toolbox category order: " + toolCategoryNames);
            Assert(FindToolCategory(tools, "图像输入与预处理").Nodes.Cast<TreeNode>().Any(node => node.Text == "图像相减"),
                "Image subtraction must be grouped with image preprocessing.");
            Assert(FindToolCategory(tools, "标定与定位").Nodes.Cast<TreeNode>().Any(node => node.Text == "点补偿"),
                "Point compensation must be grouped with calibration and positioning.");
            Assert(!FindToolCategory(tools, "输出与显示").Nodes.Cast<TreeNode>().Any(node => node.Text == "点补偿"),
                "Output and display must not contain point compensation.");
        }

        Type settingType = assembly.GetType("VMPro.Frm_Setting", true);
        using (Form setting = (Form)Activator.CreateInstance(settingType, true))
        {
            TreeView navigation = GetField<TreeView>(settingType, setting, "tvw_setting");
            string settingNames = string.Join(",", navigation.Nodes.Cast<TreeNode>().Select(node => node.Text).ToArray());
            Assert(navigation.Nodes.Count == 6, "Settings navigation should contain six non-duplicated modules: " + settingNames);
            Assert(!navigation.Nodes.Cast<TreeNode>().Any(node => node.Text == "安全"), "Duplicate security settings entry still exists.");
            Assert(settingNames == "常规,项目,方案,启动,运行,用户与安全", "Unexpected settings navigation order: " + settingNames);
        }

        Type projectType = assembly.GetType("VMPro.Project", true);
        PropertyInfo projectInstance = projectType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
        object project = projectInstance.GetValue(null, null);
        object currentEngine = projectType.GetField("curEngine", BindingFlags.Instance | BindingFlags.Public).GetValue(project);
        FieldInfo smartPositionTableField = currentEngine.GetType().GetField(
            "smartPosTable",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        object smartPositionTable = smartPositionTableField.GetValue(currentEngine);
        FieldInfo modelTablesField = smartPositionTable.GetType().GetField(
            "L_Table",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        IList modelTables = (IList)modelTablesField.GetValue(smartPositionTable);
        object[] originalTables = modelTables.Cast<object>().ToArray();

        try
        {
            modelTables.Clear();
            Type motionType = assembly.GetType("VMPro.Frm_MotionControl", true);
            using (Form motion = (Form)Activator.CreateInstance(motionType, true))
            {
                FieldInfo worker = motionType.GetField("axisRefreshThread", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert(worker != null && worker.GetValue(motion) == null, "Hidden motion workspace must not start its polling thread.");

                MethodInfo refreshTables = motionType.GetMethod(
                    "RefreshSmartPositionTables",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Assert(refreshTables != null, "Motion workspace point-table refresh entry point is missing.");
                FieldInfo tableSelectorField = motionType.GetField(
                    "comboBox1",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Assert(tableSelectorField != null, "Motion workspace point-table selector is missing.");
                object tableSelector = tableSelectorField.GetValue(motion);
                PropertyInfo tableItemsProperty = tableSelector.GetType().GetProperty("Items", BindingFlags.Instance | BindingFlags.Public);
                PropertyInfo selectedIndexProperty = tableSelector.GetType().GetProperty("SelectedIndex", BindingFlags.Instance | BindingFlags.Public);
                PropertyInfo textProperty = tableSelector.GetType().GetProperty("TextStr", BindingFlags.Instance | BindingFlags.Public);
                Assert(tableItemsProperty != null && selectedIndexProperty != null && textProperty != null,
                    "Motion workspace point-table selector properties are missing.");

                refreshTables.Invoke(motion, null);
                Assert((int)selectedIndexProperty.GetValue(tableSelector, null) == -1 &&
                       string.IsNullOrEmpty((string)textProperty.GetValue(tableSelector, null)),
                    "An empty point-table model must leave the selector explicitly unselected.");
                string[] pointActionButtons = { "button5", "button6", "button10", "button13", "button14" };
                foreach (string buttonName in pointActionButtons)
                    Assert(!GetField<Button>(motionType, motion, buttonName).Enabled,
                        "Point-table action must be disabled for an empty model: " + buttonName);

                Type tableType = assembly.GetType("VMPro.Table", true);
                ConstructorInfo tableConstructor = tableType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(string) },
                    null);
                FieldInfo tableAxesField = tableType.GetField("L_axis", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Assert(tableConstructor != null && tableAxesField != null, "Point-table test model metadata is missing.");

                object emptyNamedTable = tableConstructor.Invoke(new object[] { string.Empty });
                object populatedTable = tableConstructor.Invoke(new object[] { "SmokeTable" });
                object duplicateTable = tableConstructor.Invoke(new object[] { "SmokeTable" });
                IList populatedAxes = (IList)tableAxesField.GetValue(populatedTable);
                populatedAxes.Add("X");
                populatedAxes.Add("Y");
                modelTables.Add(emptyNamedTable);
                modelTables.Add(populatedTable);
                modelTables.Add(duplicateTable);

                refreshTables.Invoke(motion, null);
                string[] tableNames = (string[])tableItemsProperty.GetValue(tableSelector, null);
                Assert(tableNames.Length == 1 && tableNames[0] == "SmokeTable",
                    "Empty or duplicate legacy table names must not create selector/index drift.");
                Assert((int)selectedIndexProperty.GetValue(tableSelector, null) == 0 &&
                       (string)textProperty.GetValue(tableSelector, null) == "SmokeTable",
                    "The first valid point table was not selected.");
                IList modelIndexMap = (IList)motionType.GetField(
                    "smartPositionTableModelIndexes",
                    BindingFlags.Instance | BindingFlags.NonPublic).GetValue(motion);
                Assert(modelIndexMap.Count == 1 && (int)modelIndexMap[0] == 1,
                    "Point-table selector must map its item back to the actual model index.");
                DataGridView pointList = GetField<DataGridView>(motionType, motion, "dgv_pointList");
                Assert(pointList.Columns.Count == 6,
                    "A point table with two axes must build the fixed columns plus both axis columns.");
                foreach (string buttonName in pointActionButtons)
                    Assert(GetField<Button>(motionType, motion, buttonName).Enabled,
                        "Point-table action must be enabled for a valid model: " + buttonName);

                Type positionTableEditorType = assembly.GetType("VMPro.Frm_PosTableEdit", true);
                using (Form positionTableEditor = (Form)Activator.CreateInstance(positionTableEditorType, true))
                {
                    MethodInfo refreshTableList = positionTableEditorType.GetMethod(
                        "RefreshSmartPositionTableList",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    MethodInfo visibleChanged = positionTableEditorType.GetMethod(
                        "OnVisibleChanged",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    Assert(refreshTableList != null && visibleChanged != null,
                        "Point-table editor display refresh entry points are missing.");
                    Assert(MethodBodyContainsDirectCall(visibleChanged, refreshTableList),
                        "Point-table editor must refresh its table list when it becomes visible.");

                    refreshTableList.Invoke(positionTableEditor, null);
                    DataGridView tableList = GetField<DataGridView>(positionTableEditorType, positionTableEditor, "dataGridView1");
                    Assert(tableList.Rows.Count == 3, "Point-table editor must expose all legacy model rows for repair.");
                    tableList.ClearSelection();
                    tableList.Rows[1].Selected = true;
                    tableList.CurrentCell = tableList.Rows[1].Cells[1];
                    refreshTableList.Invoke(positionTableEditor, null);
                    DataGridView axisList = GetField<DataGridView>(positionTableEditorType, positionTableEditor, "dataGridView2");
                    Assert(axisList.Rows.Count == 2 && axisList.Rows[0].Cells[1].Value.ToString() == "X" &&
                           axisList.Rows[1].Cells[1].Value.ToString() == "Y",
                        "Point-table editor must refresh the selected table's non-empty axis list.");
                }

                Assert(worker.GetValue(motion) == null,
                    "Point-table UI refresh must not start the motion polling thread.");
            }
        }
        finally
        {
            modelTables.Clear();
            foreach (object table in originalTables)
                modelTables.Add(table);
        }

        Type homeType = assembly.GetType("VMPro.Frm_UserForm", true);
        using (Form home = (Form)Activator.CreateInstance(homeType, true))
        {
            Control[] dashboards = home.Controls.Find("modernDashboard", true);
            Assert(dashboards.Length == 1, "Home workspace must expose the production overview dashboard.");
            Assert(!home.Controls.Cast<Control>().Any(control => control.Visible && control.Text.Contains("自行布局")),
                "Home workspace still exposes the legacy placeholder.");
        }

        VerifyOutputBatching(assembly);
    }

    private static void VerifyStartupUiLaziness(Assembly assembly)
    {
        Type machineType = assembly.GetType("VMPro.Machine", true);
        MethodInfo initializeAll = machineType.GetMethod(
            "InitAll",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(initializeAll != null, "Machine initialization entry point is missing.");

        Type configurationType = assembly.GetType("VMPro.Configuration", true);
        MethodInfo readConfiguration = configurationType.GetMethod(
            "Read",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo companyNameSetter = configurationType.GetProperty(
            "CompanyName",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetSetMethod(true);
        MethodInfo programTitleSetter = configurationType.GetProperty(
            "ProgramTitle",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetSetMethod(true);
        Assert(readConfiguration != null && companyNameSetter != null && programTitleSetter != null,
            "Configuration metadata needed for the startup UI-thread guard is missing.");
        Assert(!MethodBodyContainsDirectCall(readConfiguration, companyNameSetter) &&
               !MethodBodyContainsDirectCall(readConfiguration, programTitleSetter),
            "Configuration.Read must update model fields only; title repaint belongs on the main UI thread.");

        object freshConfiguration = Activator.CreateInstance(configurationType, true);
        FieldInfo dataPathField = configurationType.GetField(
            "dataPath",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(dataPathField != null &&
               string.Equals((string)dataPathField.GetValue(freshConfiguration), @"D:\WLP VM", StringComparison.Ordinal),
            "A new configuration must default its data root to D:\\WLP VM.");

        Type vmType = assembly.GetType("VMPro.VM", true);
        Type welcomeType = assembly.GetType("VMPro.Frm_Welcome", true);
        Type mainShellType = assembly.GetType("VMPro.Frm_Main", true);
        MethodInfo vmInitialize = vmType.GetMethod(
            "Init",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo welcomeGetter = welcomeType.GetProperty(
            "Instance",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true);
        MethodInfo mainGetter = mainShellType.GetProperty(
            "Instance",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true);
        int readOffset = FindFirstDirectCallOffset(vmInitialize, readConfiguration);
        int welcomeOffset = FindFirstDirectCallOffset(vmInitialize, welcomeGetter);
        int mainOffset = FindFirstDirectCallOffset(vmInitialize, mainGetter);
        Assert(readOffset >= 0 && welcomeOffset >= 0 && mainOffset >= 0 &&
               readOffset < welcomeOffset && readOffset < mainOffset,
            "VM.Init must load the final configuration before constructing Welcome or Main UI.");

        MethodInfo[] initializationBodies = EnumerateTypeTree(machineType)
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(method => method == initializeAll || method.Name.IndexOf("<InitAll>", StringComparison.Ordinal) >= 0)
            .ToArray();
        Assert(!initializationBodies.Any(method => MethodBodyContainsDirectCall(method, readConfiguration)),
            "Machine.InitAll must not reload configuration after the shell has been constructed.");

        string[] deferredFormNames =
        {
            "VMPro.Frm_DeviceManager",
            "VMPro.Frm_MotionControl",
            "VMPro.Frm_PosTableEdit"
        };

        foreach (string formName in deferredFormNames)
        {
            Type formType = assembly.GetType(formName, true);
            PropertyInfo instanceProperty = formType.GetProperty(
                "Instance",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Assert(instanceProperty != null, "Deferred form singleton is missing: " + formName);
            MethodInfo instanceGetter = instanceProperty.GetGetMethod(true);
            Assert(instanceGetter != null, "Deferred form singleton getter is missing: " + formName);
            Assert(!initializationBodies.Any(method => MethodBodyContainsDirectCall(method, instanceGetter)),
                "Machine.InitAll must not create a deferred UI form: " + formName);
        }

        Type mainType = assembly.GetType("VMPro.Frm_Main", true);
        Type motionType = assembly.GetType("VMPro.Frm_MotionControl", true);
        MethodInfo applyWorkspaceMode = mainType.GetMethod(
            "ApplyWorkspaceMode",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo refreshTables = motionType.GetMethod(
            "RefreshSmartPositionTables",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(applyWorkspaceMode != null && refreshTables != null && MethodBodyContainsDirectCall(applyWorkspaceMode, refreshTables),
            "Entering the motion workspace must refresh its point-table selector.");

        string[] optionalCommunicationForms =
        {
            "VMPro.Frm_DeviceManager",
            "VMPro.Frm_TCPServer",
            "VMPro.Frm_TCPClient",
            "VMPro.Frm_Serial",
            "VMPro.Frm_Scaner",
            "VMPro.Frm_PLCComm"
        };
        foreach (string formName in optionalCommunicationForms)
            VerifyTryGetDoesNotCreate(assembly.GetType(formName, true));

        string[] communicationTypes =
        {
            "VMPro.TCPSever",
            "VMPro.TCPClient",
            "VMPro.Serial",
            "VMPro.Scaner",
            "VMPro.PLCComm",
            "VMPro.PLCDevice",
            "VMPro.CipCommunication"
        };
        string[] forbiddenSingletonForms = optionalCommunicationForms
            .Concat(new[] { "VMPro.Frm_MessageBox" })
            .ToArray();
        MethodInfo[] optionalFormGetters = forbiddenSingletonForms
            .Select(formName => assembly.GetType(formName, true))
            .Select(formType =>
            {
                PropertyInfo instanceProperty = formType.GetProperty(
                    "Instance",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                Assert(instanceProperty != null, "Optional form singleton is missing: " + formType.FullName);
                return instanceProperty.GetGetMethod(true);
            })
            .ToArray();

        foreach (string communicationTypeName in communicationTypes)
        {
            Type communicationType = assembly.GetType(communicationTypeName, true);
            MethodInfo[] communicationBodies = EnumerateTypeTree(communicationType)
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .ToArray();
            foreach (MethodInfo getter in optionalFormGetters)
            {
                Assert(getter != null && !communicationBodies.Any(method => MethodBodyContainsDirectCall(method, getter)),
                    communicationTypeName + " must not create an optional UI form from a communication path: " +
                    (getter == null ? "unknown" : getter.DeclaringType.FullName));
            }
        }

        VerifyTcpClientUiDispatch(assembly);
        VerifyPlcUiDispatch(assembly);
    }

    private static void VerifyRuntimeDisplayUiDispatch(Assembly assembly)
    {
        Type imageWindowType = assembly.GetType("VMPro.Frm_ImageWindow", true);
        Type toolBaseType = assembly.GetType("VMPro.ToolBase", true);
        MethodInfo tryPost = imageWindowType.GetMethod(
            "TryPostRuntimeDisplay",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo postToImageWindow = toolBaseType.GetMethod(
            "PostToImageWindow",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert(tryPost != null && postToImageWindow != null,
            "Runtime image display UI dispatcher is missing.");

        foreach (string methodName in new[] { "ShowImage", "ShowObj", "Show_Text", "SetDraw", "SetLineWidth", "SetColor" })
        {
            MethodInfo displayMethod = toolBaseType.GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert(displayMethod != null && MethodBodyContainsDirectCall(displayMethod, postToImageWindow),
                methodName + " must route image-window work through the UI dispatcher.");
        }
        Assert(MethodBodyContainsDirectCall(postToImageWindow, tryPost),
            "ToolBase image-window dispatcher must post through an existing image window.");

        bool originalCrossThreadCheck = Control.CheckForIllegalCrossThreadCalls;
        Form imageWindow = null;
        try
        {
            Control.CheckForIllegalCrossThreadCalls = true;
            imageWindow = (Form)Activator.CreateInstance(imageWindowType, true);
            IntPtr handle = imageWindow.Handle;
            int uiThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = 0;
            bool posted = false;
            Exception workerFailure = null;
            Action callback = delegate
            {
                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                imageWindow.Text = "UI-dispatch-verified";
            };

            Thread worker = new Thread(new ThreadStart(delegate
            {
                try
                {
                    posted = (bool)tryPost.Invoke(null, new object[] { callback, null });
                }
                catch (Exception ex)
                {
                    workerFailure = ex;
                }
            }));
            worker.Start();
            worker.Join();
            Application.DoEvents();

            Assert(workerFailure == null && posted && callbackThreadId == uiThreadId &&
                   imageWindow.Text == "UI-dispatch-verified",
                "Background runtime display work was not marshalled to the image-window UI thread.");
        }
        finally
        {
            if (imageWindow != null)
                imageWindow.Dispose();
            Control.CheckForIllegalCrossThreadCalls = originalCrossThreadCheck;
        }
    }

    private static IEnumerable<Type> EnumerateTypeTree(Type root)
    {
        yield return root;
        foreach (Type nestedType in root.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
        {
            foreach (Type descendant in EnumerateTypeTree(nestedType))
                yield return descendant;
        }
    }

    private static void VerifyTryGetDoesNotCreate(Type formType)
    {
        FieldInfo singletonField = formType.GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo tryGet = formType.GetMethod(
            "TryGetExistingInstance",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(singletonField != null && tryGet != null,
            "Optional form must expose a non-creating lookup: " + formType.FullName);

        object before = singletonField.GetValue(null);
        try
        {
            singletonField.SetValue(null, null);
            int openFormCount = Application.OpenForms.Count;
            object[] arguments = { null };
            bool found = (bool)tryGet.Invoke(null, arguments);
            Assert(!found && arguments[0] == null && singletonField.GetValue(null) == null,
                "TryGetExistingInstance must return a null result without constructing a form: " + formType.FullName);
            Assert(Application.OpenForms.Count == openFormCount,
                "TryGetExistingInstance changed the open-form set: " + formType.FullName);
        }
        finally
        {
            singletonField.SetValue(null, before);
        }
    }

    private static void VerifyTcpClientUiDispatch(Assembly assembly)
    {
        Type modelType = assembly.GetType("VMPro.TCPClient", true);
        Type formType = assembly.GetType("VMPro.Frm_TCPClient", true);
        ConstructorInfo modelConstructor = modelType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(string) },
            null);
        MethodInfo loadParameters = formType.GetMethod(
            "LoadPar",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo applyConnectionState = formType.GetMethod(
            "TryApplyConnectionState",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo appendLog = formType.GetMethod(
            "TryAppendLog",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo singletonField = formType.GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
        FieldInfo connectingField = modelType.GetField("connecting", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo socketMapField = modelType.GetField("L_socket", BindingFlags.Static | BindingFlags.NonPublic);
        FieldInfo socketMapSyncField = modelType.GetField("SocketMapSyncRoot", BindingFlags.Static | BindingFlags.NonPublic);
        FieldInfo runtimeNameOwnersField = modelType.GetField("RuntimeNameOwners", BindingFlags.Static | BindingFlags.NonPublic);
        FieldInfo nameField = modelType.GetField("Name", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo manualDisconnectField = modelType.GetField("manualDisconnect", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo connectionGenerationField = modelType.GetField("connectionGeneration", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo findSocket = modelType.GetMethod("FindSocketByName", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo close = modelType.GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo tryInstallRuntimeSocket = modelType.GetMethod("TryInstallCurrentRuntimeSocket", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo beginExplicitConnect = modelType.GetMethod("BeginExplicitConnect", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo connectPrepared = modelType.GetMethod("ConnectPrepared", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo shouldAutoReconnect = modelType.GetMethod("ShouldAutoReconnect", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo connectCore = modelType.GetMethod("ConnectCore", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo receiveLoop = modelType.GetMethod("Recieve", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo rename = modelType.GetMethod("Rename", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo getSocketSnapshot = modelType.GetMethod("GetRuntimeSocketSnapshot", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo resetRuntimeStore = modelType.GetMethod("ResetRuntimeStore", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo removeRuntimeSocketIfSame = modelType.GetMethod(
            "RemoveRuntimeSocket",
            BindingFlags.Static | BindingFlags.NonPublic,
            null,
            new[] { typeof(string), typeof(Socket) },
            null);
        MethodInfo removeRuntimeSocket = modelType.GetMethod(
            "RemoveRuntimeSocket",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(string) },
            null);
        MethodInfo unregisterRuntime = modelType.GetMethod("UnregisterRuntime", BindingFlags.Static | BindingFlags.NonPublic);
        Type projectType = assembly.GetType("VMPro.Project", true);
        MethodInfo ensureCommunicationRuntime = projectType.GetMethod(
            "EnsureCommunicationRuntime",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert(modelConstructor != null && loadParameters != null && applyConnectionState != null && appendLog != null &&
               singletonField != null && connectingField != null && socketMapField != null && socketMapSyncField != null &&
               runtimeNameOwnersField != null && nameField != null &&
               manualDisconnectField != null && connectionGenerationField != null && findSocket != null && close != null &&
               tryInstallRuntimeSocket != null && beginExplicitConnect != null && connectPrepared != null && shouldAutoReconnect != null &&
               connectCore != null && receiveLoop != null && rename != null &&
               getSocketSnapshot != null && resetRuntimeStore != null && removeRuntimeSocketIfSame != null &&
               removeRuntimeSocket != null && unregisterRuntime != null && ensureCommunicationRuntime != null,
            "TCP client lazy UI dispatch metadata is incomplete.");
        Assert(connectingField.IsNotSerialized,
            "Transient TCP connection progress must not be persisted with the project.");
        Assert(manualDisconnectField.IsNotSerialized,
            "Manual TCP disconnect state must not be persisted with the project.");
        Assert(connectionGenerationField.IsNotSerialized,
            "TCP connection generations must not be persisted with the project.");
        Assert(socketMapField.IsPrivate && socketMapField.IsInitOnly && socketMapSyncField.IsPrivate && socketMapSyncField.IsInitOnly &&
               runtimeNameOwnersField.IsPrivate && runtimeNameOwnersField.IsInitOnly,
            "The TCP runtime socket map and its lock must be private, stable synchronization roots.");
        VerifyTcpSocketMapEncapsulation(modelType, socketMapField);
        Assert(MethodBodyContainsDirectCall(connectCore, tryInstallRuntimeSocket) &&
               !MethodBodyContainsDirectCall(receiveLoop, findSocket),
            "TCP connect/receive paths must install atomically and keep each receive loop bound to its owned socket.");
        Assert(MethodBodyContainsDirectCall(ensureCommunicationRuntime, resetRuntimeStore),
            "Project activation must reset TCP runtime ownership before registering the imported models.");

        string suffix = Guid.NewGuid().ToString("N");
        string firstName = "SmokeTCP-A-" + suffix;
        string secondName = "SmokeTCP-B-" + suffix;
        object firstModel = null;
        object secondModel = null;
        object resetOldModel = null;
        object resetNewModel = null;
        object fallbackNewModel = null;
        string resetName = "SmokeTCP-Reset-" + suffix;
        Form form = null;
        object originalSingleton = singletonField.GetValue(null);
        bool originalCrossThreadCheck = Control.CheckForIllegalCrossThreadCalls;
        try
        {
            Control.CheckForIllegalCrossThreadCalls = true;
            firstModel = modelConstructor.Invoke(new object[] { firstName });
            secondModel = modelConstructor.Invoke(new object[] { secondName });

            Socket originalSocket = (Socket)findSocket.Invoke(firstModel, null);
            Socket replacementSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            object[] installArguments = { replacementSocket, 0, false, null };
            bool installed = (bool)tryInstallRuntimeSocket.Invoke(firstModel, installArguments);
            Assert(installed && string.Equals((string)installArguments[3], firstName, StringComparison.Ordinal) &&
                   ReferenceEquals(findSocket.Invoke(firstModel, null), replacementSocket) && IsSocketClosed(originalSocket),
                "Replacing a TCP runtime socket must publish the replacement and close the superseded handle.");
            object staleRemoval = removeRuntimeSocketIfSame.Invoke(null, new object[] { firstName, originalSocket });
            Assert(staleRemoval == null && ReferenceEquals(findSocket.Invoke(firstModel, null), replacementSocket),
                "A stale TCP cleanup must not remove a newer socket for the same client.");
            close.Invoke(firstModel, null);
            int closedGeneration = (int)connectionGenerationField.GetValue(firstModel);
            Assert((bool)manualDisconnectField.GetValue(firstModel) &&
                   !SocketSnapshotContainsName((IEnumerable)getSocketSnapshot.Invoke(null, null), firstName) &&
                   IsSocketClosed(replacementSocket),
                "Manual TCP disconnect must suppress reconnect, remove the map entry, and close its socket.");
            int resumedGeneration = (int)beginExplicitConnect.Invoke(firstModel, null);
            Assert(!(bool)manualDisconnectField.GetValue(firstModel) && resumedGeneration > closedGeneration &&
                   !(bool)shouldAutoReconnect.Invoke(firstModel, new object[] { closedGeneration }) &&
                   (bool)shouldAutoReconnect.Invoke(firstModel, new object[] { resumedGeneration }),
                "A later explicit TCP connection attempt must invalidate stale work and re-enable reconnect behavior.");

            findSocket.Invoke(firstModel, null);
            close.Invoke(firstModel, null);
            bool stalePreparedConnection = (bool)connectPrepared.Invoke(firstModel, new object[] { 1, false, resumedGeneration });
            Assert(!stalePreparedConnection && (bool)manualDisconnectField.GetValue(firstModel) &&
                   !SocketSnapshotContainsName((IEnumerable)getSocketSnapshot.Invoke(null, null), firstName),
                "A queued TCP connect token must not revive a client after a later manual close.");
            beginExplicitConnect.Invoke(firstModel, null);

            Socket firstSocketBeforeCollision = (Socket)findSocket.Invoke(firstModel, null);
            close.Invoke(secondModel, null);
            Assert(!SocketSnapshotContainsName((IEnumerable)getSocketSnapshot.Invoke(null, null), secondName),
                "The disconnected TCP client should exercise name ownership independently of the socket map.");
            string firstNameBeforeCollision = (string)nameField.GetValue(firstModel);
            bool renamedIntoCollision = (bool)rename.Invoke(firstModel, new object[] { secondName });
            Socket secondSocketAfterCollision = (Socket)findSocket.Invoke(secondModel, null);
            Assert(!renamedIntoCollision &&
                   string.Equals((string)nameField.GetValue(firstModel), firstNameBeforeCollision, StringComparison.Ordinal) &&
                   ReferenceEquals(findSocket.Invoke(firstModel, null), firstSocketBeforeCollision) &&
                   secondSocketAfterCollision != null && !ReferenceEquals(secondSocketAfterCollision, firstSocketBeforeCollision),
                "Renaming onto a disconnected TCP client must be rejected without rebinding either model.");

            form = (Form)Activator.CreateInstance(formType, true);
            singletonField.SetValue(null, form);
            IntPtr handle = form.Handle;
            SetLocalVisible(form, true);

            loadParameters.Invoke(form, new[] { firstModel });
            Control connectButton = GetField<Control>(formType, form, "btn_connect");
            PropertyInfo buttonText = connectButton.GetType().GetProperty("TextStr", BindingFlags.Instance | BindingFlags.Public);
            Assert(buttonText != null, "TCP client connect button text property is missing.");

            Exception workerFailure = null;
            Thread worker = new Thread(delegate()
            {
                try
                {
                    applyConnectionState.Invoke(null, new[] { firstModel, (object)true });
                    appendLog.Invoke(null, new[] { firstModel, "visible-log" });
                }
                catch (Exception ex)
                {
                    workerFailure = ex;
                }
            });
            worker.Start();
            worker.Join();
            Application.DoEvents();
            TextBox log = GetField<TextBox>(formType, form, "tbx_log");
            Assert(workerFailure == null && (string)buttonText.GetValue(connectButton, null) == "断开" && connectButton.Enabled &&
                   log.Text.Contains("visible-log"),
                "TCP client state was not marshalled to the existing bound form.");

            SetLocalVisible(form, false);
            worker = new Thread(delegate()
            {
                try
                {
                    applyConnectionState.Invoke(null, new[] { firstModel, (object)false });
                    appendLog.Invoke(null, new[] { firstModel, "hidden-log" });
                }
                catch (Exception ex)
                {
                    workerFailure = ex;
                }
            });
            worker.Start();
            worker.Join();
            Application.DoEvents();
            Assert((string)buttonText.GetValue(connectButton, null) == "连接" && !log.Text.Contains("hidden-log"),
                "Hidden TCP client forms must accept state but drop display-only logs.");

            SetLocalVisible(form, true);
            loadParameters.Invoke(form, new[] { firstModel });
            workerFailure = null;
            worker = new Thread(delegate()
            {
                try
                {
                    applyConnectionState.Invoke(null, new[] { firstModel, (object)true });
                }
                catch (Exception ex)
                {
                    workerFailure = ex;
                }
            });
            worker.Start();
            worker.Join();

            connectButton.Enabled = false;
            buttonText.SetValue(connectButton, "连接中...", null);
            connectingField.SetValue(secondModel, false);
            loadParameters.Invoke(form, new[] { secondModel });
            Application.DoEvents();
            Assert(workerFailure == null && connectButton.Enabled &&
                   (string)buttonText.GetValue(connectButton, null) == "连接",
                "A stale TCP completion must not overwrite the newly selected client state.");

            connectingField.SetValue(secondModel, true);
            loadParameters.Invoke(form, new[] { secondModel });
            Assert(!connectButton.Enabled && (string)buttonText.GetValue(connectButton, null) == "连接中...",
                "A selected TCP client with an in-flight connection must remain disabled.");
            connectingField.SetValue(secondModel, false);

            form.Dispose();
            object disposedSingleton = singletonField.GetValue(null);
            applyConnectionState.Invoke(null, new[] { secondModel, (object)true });
            Assert(ReferenceEquals(disposedSingleton, singletonField.GetValue(null)),
                "A disposed TCP client form must not be recreated by a status callback.");

            resetOldModel = modelConstructor.Invoke(new object[] { resetName });
            Socket resetOldSocket = (Socket)findSocket.Invoke(resetOldModel, null);
            int resetOldToken = (int)beginExplicitConnect.Invoke(resetOldModel, null);
            resetRuntimeStore.Invoke(null, null);
            Assert(IsSocketClosed(resetOldSocket) &&
                   !(bool)shouldAutoReconnect.Invoke(resetOldModel, new object[] { resetOldToken }) &&
                   !(bool)connectPrepared.Invoke(resetOldModel, new object[] { 1, false, resetOldToken }),
                "Resetting TCP runtime state must close old sockets and invalidate queued work from the previous project.");

            resetNewModel = modelConstructor.Invoke(new object[] { resetName });
            Socket resetNewSocket = (Socket)findSocket.Invoke(resetNewModel, null);
            Assert(resetNewSocket != null && !ReferenceEquals(resetNewSocket, resetOldSocket),
                "A new project must be able to register the same TCP client name after the runtime reset.");

            removeRuntimeSocket.Invoke(null, new object[] { resetName });
            fallbackNewModel = modelConstructor.Invoke(new object[] { resetName });
            Socket fallbackNewSocket = (Socket)findSocket.Invoke(fallbackNewModel, null);
            Assert(IsSocketClosed(resetNewSocket) && fallbackNewSocket != null &&
                   !ReferenceEquals(fallbackNewSocket, resetNewSocket),
                "Fallback TCP removal must release both the socket and its name ownership.");
        }
        finally
        {
            Control.CheckForIllegalCrossThreadCalls = originalCrossThreadCheck;
            if (form != null && !form.IsDisposed)
                form.Dispose();
            singletonField.SetValue(null, originalSingleton);

            foreach (string name in new[] { firstName, secondName })
                removeRuntimeSocket.Invoke(null, new object[] { name });
            if (firstModel != null)
                unregisterRuntime.Invoke(null, new[] { firstModel });
            if (secondModel != null)
                unregisterRuntime.Invoke(null, new[] { secondModel });
            if (resetOldModel != null)
                unregisterRuntime.Invoke(null, new[] { resetOldModel });
            if (resetNewModel != null)
                unregisterRuntime.Invoke(null, new[] { resetNewModel });
            if (fallbackNewModel != null)
                unregisterRuntime.Invoke(null, new[] { fallbackNewModel });
        }
    }

    private static void VerifyPlcUiDispatch(Assembly assembly)
    {
        Type modelType = assembly.GetType("VMPro.PLCDevice", true);
        Type formType = assembly.GetType("VMPro.Frm_PLCComm", true);
        ConstructorInfo modelConstructor = modelType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(string) },
            null);
        MethodInfo loadParameters = formType.GetMethod(
            "LoadPar",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo applyConnectionResult = formType.GetMethod(
            "ApplyConnectionResult",
            BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo connectingField = modelType.GetField("connecting", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert(modelConstructor != null && loadParameters != null && applyConnectionResult != null && connectingField != null,
            "PLC lazy UI dispatch metadata is incomplete.");
        Assert(connectingField.IsNotSerialized,
            "Transient PLC connection progress must not be persisted with the project.");

        object firstModel = modelConstructor.Invoke(new object[] { "SmokePLC-A" });
        object secondModel = modelConstructor.Invoke(new object[] { "SmokePLC-B" });
        try
        {
            using (Form form = (Form)Activator.CreateInstance(formType, true))
            {
                Button connectButton = GetField<Button>(formType, form, "btn_connect");
                Label status = GetField<Label>(formType, form, "lbl_statu");

                connectingField.SetValue(firstModel, true);
                loadParameters.Invoke(form, new[] { firstModel });
                Assert(!connectButton.Enabled && status.Text == "连接中...",
                    "A PLC connection in progress must remain disabled when its page is revisited.");

                connectingField.SetValue(firstModel, false);
                loadParameters.Invoke(form, new[] { secondModel });
                string secondStatus = status.Text;
                applyConnectionResult.Invoke(form, new[] { firstModel, (object)true, string.Empty });
                Assert(connectButton.Enabled && status.Text == secondStatus,
                    "A stale PLC completion must not overwrite the newly selected device state.");

                connectingField.SetValue(secondModel, true);
                loadParameters.Invoke(form, new[] { secondModel });
                Assert(!connectButton.Enabled && status.Text == "连接中...",
                    "The selected PLC must expose its non-persisted connection progress.");
            }
        }
        finally
        {
            connectingField.SetValue(firstModel, false);
            connectingField.SetValue(secondModel, false);
        }
    }

    private static bool MethodBodyContainsDirectCall(MethodInfo caller, MethodInfo target)
    {
        return FindFirstDirectCallOffset(caller, target) >= 0;
    }

    private static int FindFirstDirectCallOffset(MethodInfo caller, MethodInfo target)
    {
        if (caller == null || target == null)
            return -1;
        MethodBody body = caller.GetMethodBody();
        if (body == null)
            return -1;

        byte[] instructions = body.GetILAsByteArray();
        int targetToken = target.MetadataToken;
        for (int index = 0; index <= instructions.Length - 5; index++)
        {
            // call (0x28) and callvirt (0x6f) are followed by a four-byte metadata token.
            if ((instructions[index] == 0x28 || instructions[index] == 0x6f) &&
                BitConverter.ToInt32(instructions, index + 1) == targetToken)
                return index;
        }

        return -1;
    }

    private static void VerifyTcpSocketMapEncapsulation(Type modelType, FieldInfo socketMapField)
    {
        HashSet<string> allowedMethods = new HashSet<string>(StringComparer.Ordinal)
        {
            "EnsureRuntimeSocket",
            "GetRuntimeSocket",
            "SwapRuntimeSocket",
            "TryInstallCurrentRuntimeSocket",
            "RemoveRuntimeSocket",
            "IsCurrentRuntimeSocket",
            "GetRuntimeSocketSnapshot",
            "TryRenameRuntimeSocket",
            "InvalidateAndDetachRuntimeSocket",
            "UnregisterRuntime",
            "ResetRuntimeStore"
        };

        foreach (MethodInfo method in modelType.GetMethods(
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
        {
            if (MethodBodyContainsFieldAccess(method, socketMapField))
                Assert(allowedMethods.Contains(method.Name),
                    "TCP socket map access escaped its synchronized helper boundary: " + method.Name);
        }
    }

    private static bool MethodBodyContainsFieldAccess(MethodInfo method, FieldInfo field)
    {
        MethodBody body = method.GetMethodBody();
        if (body == null)
            return false;

        byte[] instructions = body.GetILAsByteArray();
        int fieldToken = field.MetadataToken;
        for (int index = 0; index <= instructions.Length - 5; index++)
        {
            // ldsfld (0x7e), ldsflda (0x7f), and stsfld (0x80) carry a four-byte field token.
            if ((instructions[index] == 0x7e || instructions[index] == 0x7f || instructions[index] == 0x80) &&
                BitConverter.ToInt32(instructions, index + 1) == fieldToken)
                return true;
        }

        return false;
    }

    private static bool SocketSnapshotContainsName(IEnumerable snapshot, string name)
    {
        foreach (object item in snapshot)
        {
            PropertyInfo keyProperty = item.GetType().GetProperty("Key", BindingFlags.Instance | BindingFlags.Public);
            if (keyProperty != null && string.Equals((string)keyProperty.GetValue(item, null), name, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private static bool IsSocketClosed(Socket socket)
    {
        if (socket == null)
            return true;

        try
        {
            return socket.Available < 0;
        }
        catch (ObjectDisposedException)
        {
            return true;
        }
        catch (SocketException ex)
        {
            return ex.SocketErrorCode == SocketError.NotSocket;
        }
    }

    private static void VerifyOutputBatching(Assembly assembly)
    {
        Type outputType = assembly.GetType("VMPro.Frm_Output", true);
        bool originalCrossThreadCheck = Control.CheckForIllegalCrossThreadCalls;
        try
        {
            using (Form output = (Form)Activator.CreateInstance(outputType, true))
            {
            MethodInfo outputMessage = outputType.GetMethod("OutputMsg", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo flush = outputType.GetMethod("FlushPendingOutputItems", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo clear = outputType.GetMethod("ClearLog", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert(outputMessage != null && flush != null && clear != null, "Output batching entry points are missing.");

            Control.CheckForIllegalCrossThreadCalls = true;
            Exception workerFailure = null;
            Thread worker = new Thread(delegate()
            {
                try
                {
                    for (int index = 0; index < 1050; index++)
                        outputMessage.Invoke(output, new object[] { "batch-" + index, Color.Black });
                }
                catch (Exception ex)
                {
                    workerFailure = ex;
                }
            });
            worker.Start();
            worker.Join();
            Assert(workerFailure == null, "Background logging failed: " + workerFailure);

            flush.Invoke(output, null);
            ListView list = GetField<ListView>(outputType, output, "listView1");
            ToolStripButton informationCounter = GetField<ToolStripButton>(outputType, output, "tsb_tip");
            Assert(list.Items.Count == 1000, "Output view must retain the newest 1000 queued messages.");
            Assert(informationCounter.Text == "提示(1000)", "Output counters were not synchronized after a batch rebuild.");

            clear.Invoke(output, null);
            flush.Invoke(output, null);
            Assert(list.Items.Count == 0 && informationCounter.Text == "提示(0)", "Output clear did not reset the model and UI snapshot.");

            Type projectType = assembly.GetType("VMPro.Project", true);
            PropertyInfo projectInstance = projectType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            Assert(projectInstance != null, "Project singleton is missing.");
            object project = projectInstance.GetValue(null, null);
            FieldInfo currentEngineField = projectType.GetField("curEngine", BindingFlags.Instance | BindingFlags.Public);
            Assert(currentEngineField != null, "Current engine field is missing.");
            object currentEngine = currentEngineField.GetValue(project);
            FieldInfo alarmHistoryField = currentEngine.GetType().GetField(
                "D_historyAlarm",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert(alarmHistoryField != null, "Alarm history field is missing.");
            IDictionary alarmHistory = alarmHistoryField.GetValue(currentEngine) as IDictionary;
            Assert(alarmHistory != null, "Alarm history collection is unavailable.");
            alarmHistory.Clear();

            workerFailure = null;
            Thread redWorker = new Thread(delegate()
            {
                try
                {
                    for (int index = 0; index < 1100; index++)
                        outputMessage.Invoke(output, new object[] { "alarm-" + index, Color.Red });
                }
                catch (Exception ex)
                {
                    workerFailure = ex;
                }
            });
            redWorker.Start();
            redWorker.Join();
            Assert(workerFailure == null, "High-frequency alarm logging failed: " + workerFailure);

            flush.Invoke(output, null);
            ToolStripButton errorCounter = GetField<ToolStripButton>(outputType, output, "tsb_error");
            ToolStripButton alarmCounter = GetField<ToolStripButton>(outputType, output, "toolStripButton1");
            string[] retainedAlarmMessages = alarmHistory.Values.Cast<object>().Select(value => value.ToString()).ToArray();
            Assert(list.Items.Count == 1000, "Alarm output view must retain the newest 1000 messages.");
            Assert(errorCounter.Text == "错误(1000)", "Alarm output counter drifted during high-frequency writes.");
            Assert(alarmCounter.Text == "报警(1000)" && alarmHistory.Count == 1000,
                "Alarm history must retain 1000 uniquely timestamped records.");
            Assert(!retainedAlarmMessages.Contains("alarm-99") && retainedAlarmMessages.Contains("alarm-100") && retainedAlarmMessages.Contains("alarm-1099"),
                "Alarm history must evict the oldest records and retain the newest records.");

            alarmHistory.Clear();
            clear.Invoke(output, null);
            flush.Invoke(output, null);
            Assert(list.Items.Count == 0 && errorCounter.Text == "错误(0)" && alarmCounter.Text == "报警(0)",
                "Alarm output cleanup did not reset the model and UI snapshot.");
            }
        }
        finally
        {
            Control.CheckForIllegalCrossThreadCalls = originalCrossThreadCheck;
        }
    }

    private static void RenderControl(Bitmap target, Control control)
    {
        if (control.Width <= 0 || control.Height <= 0)
            return;

        using (Bitmap part = new Bitmap(control.Width, control.Height))
        using (Graphics graphics = Graphics.FromImage(target))
        {
            control.DrawToBitmap(part, new Rectangle(Point.Empty, part.Size));
            Form form = control.FindForm();
            Point location = form == null
                ? control.Location
                : form.PointToClient(control.PointToScreen(Point.Empty));
            graphics.DrawImageUnscaled(part, location.X, location.Y);
        }
    }

    private static void VerifyAndRenderVectorIcons(
        Assembly assembly,
        ToolStrip primaryBar,
        ToolStrip visionBar,
        ToolStripButton[] primaryButtons,
        ToolStripButton[] visionButtons,
        ToolStripDropDownButton batchButton,
        string outputDirectory)
    {
        Type factoryType = assembly.GetType("VMPro.ModernVectorIconFactory", true);
        Type glyphType = factoryType.GetNestedType("Glyph", BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo getIcon = factoryType.GetMethod(
            "Get",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { glyphType, typeof(int), typeof(Color) },
            null);
        MethodInfo getPixelSize = factoryType.GetMethod(
            "GetPixelSize",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(Control), typeof(int) },
            null);
        Assert(glyphType != null && getIcon != null && getPixelSize != null,
            "Modern vector icon factory metadata is incomplete.");

        Array glyphs = Enum.GetValues(glyphType);
        int[] requestedSizes = { 20, 30, 40 };
        Color iconColor = Color.FromArgb(76, 148, 210);
        foreach (int requestedSize in requestedSizes)
        {
            foreach (object glyph in glyphs)
            {
                Image icon = (Image)getIcon.Invoke(null, new[] { glyph, (object)requestedSize, iconColor });
                Assert(icon != null && icon.Width == requestedSize && icon.Height == requestedSize,
                    "Vector glyph did not render at " + requestedSize + " px: " + glyph);
                Assert(ImageHasVisibleInk(icon),
                    "Vector glyph rendered as a transparent bitmap: " + glyph + " at " + requestedSize + " px.");
            }
        }

        int primaryPixelSize = ClampIconSize((int)getPixelSize.Invoke(null, new object[] { primaryBar, 22 }));
        int visionPixelSize = ClampIconSize((int)getPixelSize.Invoke(null, new object[] { visionBar, 20 }));
        int menuPixelSize = ClampIconSize((int)getPixelSize.Invoke(null, new object[] { visionBar, 18 }));
        Assert(primaryButtons.All(button => button.Image != null &&
                                            button.Image.Width == primaryPixelSize &&
                                            button.Image.Height == primaryPixelSize &&
                                            button.ImageScaling == ToolStripItemImageScaling.None),
            "Primary ToolStrip icons must keep their DPI-specific pixels without a second scaling pass.");
        Assert(visionButtons.All(button => button.Image != null &&
                                           button.Image.Width == visionPixelSize &&
                                           button.Image.Height == visionPixelSize &&
                                           button.ImageScaling == ToolStripItemImageScaling.None),
            "Vision ToolStrip icons must keep their DPI-specific pixels without a second scaling pass.");
        Assert(batchButton.Image != null && batchButton.Image.Width == menuPixelSize &&
               batchButton.Image.Height == menuPixelSize &&
               batchButton.ImageScaling == ToolStripItemImageScaling.None,
            "Batch-run icon must keep its DPI-specific pixels without a second scaling pass.");
        Assert(batchButton.DropDownItems.Cast<ToolStripItem>().All(item =>
                   item.Image != null && item.Image.Width == menuPixelSize &&
                   item.Image.Height == menuPixelSize &&
                   item.ImageScaling == ToolStripItemImageScaling.None),
            "Batch-run menu action icons must not be rescaled by ToolStrip.");

        if (string.IsNullOrEmpty(outputDirectory))
            return;

        Directory.CreateDirectory(outputDirectory);
        const int labelWidth = 74;
        const int cellWidth = 50;
        const int rowHeight = 62;
        int atlasWidth = labelWidth + glyphs.Length * cellWidth + 16;
        int atlasHeight = 34 + requestedSizes.Length * rowHeight + 12;
        using (Bitmap atlas = new Bitmap(atlasWidth, atlasHeight))
        using (Graphics graphics = Graphics.FromImage(atlas))
        using (Brush titleBrush = new SolidBrush(Color.FromArgb(40, 65, 88)))
        using (Pen divider = new Pen(Color.FromArgb(218, 231, 242)))
        {
            graphics.Clear(Color.FromArgb(255, 254, 250));
            graphics.DrawString("WLP VM 矢量图标 · 20 / 30 / 40 px", SystemFonts.MessageBoxFont, titleBrush, 12F, 9F);
            for (int row = 0; row < requestedSizes.Length; row++)
            {
                int requestedSize = requestedSizes[row];
                int top = 34 + row * rowHeight;
                graphics.DrawString(requestedSize + " px", SystemFonts.MessageBoxFont, titleBrush, 12F, top + 20F);
                graphics.DrawLine(divider, labelWidth, top + rowHeight - 1, atlasWidth - 12, top + rowHeight - 1);
                for (int column = 0; column < glyphs.Length; column++)
                {
                    Image icon = (Image)getIcon.Invoke(
                        null,
                        new[] { glyphs.GetValue(column), (object)requestedSize, iconColor });
                    int left = labelWidth + column * cellWidth + (cellWidth - icon.Width) / 2;
                    int iconTop = top + (rowHeight - icon.Height) / 2;
                    graphics.DrawImageUnscaled(icon, left, iconTop);
                }
            }

            atlas.Save(Path.Combine(outputDirectory, "vector-icons-20-30-40.png"), ImageFormat.Png);
        }
    }

    private static int ClampIconSize(int pixelSize)
    {
        return Math.Max(16, Math.Min(64, pixelSize));
    }

    private static void VerifyProxyEnablementGate(Type mainType)
    {
        MethodInfo createProxy = mainType.GetMethod(
            "CreateProxyMenuItem",
            BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo synchronize = mainType.GetMethod(
            "SynchronizeProxyMenuItems",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert(createProxy != null && synchronize != null,
            "Consolidated menu proxy helpers are missing.");

        using (ToolStripButton source = new ToolStripButton("source"))
        using (ToolStripMenuItem owner = new ToolStripMenuItem("owner"))
        {
            ToolStripMenuItem proxy = (ToolStripMenuItem)createProxy.Invoke(
                null,
                new object[] { "proxy", source });
            owner.DropDownItems.Add(proxy);
            int clickCount = 0;
            EventHandler observer = delegate { clickCount++; };
            source.Click += observer;
            try
            {
                source.Enabled = false;
                source.Available = true;
                synchronize.Invoke(null, new object[] { owner.DropDownItems });
                Assert(!proxy.Enabled,
                    "A menu proxy must disable itself when its source is disabled.");
                proxy.Enabled = true;
                proxy.PerformClick();
                Assert(clickCount == 0,
                    "A manually re-enabled proxy must not bypass a disabled source command.");

                source.Enabled = true;
                source.Available = false;
                synchronize.Invoke(null, new object[] { owner.DropDownItems });
                Assert(!proxy.Enabled,
                    "A menu proxy must disable itself when its source is unavailable.");
                proxy.Enabled = true;
                proxy.PerformClick();
                Assert(clickCount == 0,
                    "A manually re-enabled proxy must not bypass an unavailable source command.");

                source.Enabled = true;
                source.Available = true;
                synchronize.Invoke(null, new object[] { owner.DropDownItems });
                Assert(proxy.Enabled, "A proxy must enable when its source is enabled and available.");
                proxy.PerformClick();
                Assert(clickCount == 1,
                    "An enabled/available proxy must execute its source exactly once.");
            }
            finally
            {
                source.Click -= observer;
                owner.DropDownItems.Clear();
                proxy.Dispose();
            }
        }
    }

    private static void VerifyRunShortcutStateGates(Type mainType, Form form)
    {
        ToolStripMenuItem runOnce = GetField<ToolStripMenuItem>(
            mainType,
            form,
            "运行一次ToolStripMenuItem");
        ToolStripMenuItem runContinuous = GetField<ToolStripMenuItem>(
            mainType,
            form,
            "连续运行ToolStripMenuItem");
        Assert(runOnce.ShortcutKeys == Keys.F5 && runContinuous.ShortcutKeys == Keys.F6,
            "Run-once/run-continuous menu commands must retain F5/F6.");
        Assert(runOnce.Tag is ToolStripItem && runContinuous.Tag is ToolStripItem &&
               ((ToolStripItem)runOnce.Tag).Name == "toolStripButton11" &&
               ((ToolStripItem)runContinuous.Tag).Name == "toolStripButton12",
            "F5/F6 must delegate to the same source commands as the Vision command bar.");

        Type permissionType = mainType.Assembly.GetType("VMPro.Permission", true);
        MethodInfo permissionCheck = permissionType.GetMethod(
            "CheckPermission",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo stopContinuous = mainType.GetMethod(
            "StopContinuousRunMenuItem_Click",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert(permissionCheck != null && stopContinuous != null &&
               MethodBodyContainsDirectCall(stopContinuous, permissionCheck),
            "The explicit stop-continuous menu command must retain the Admin permission gate.");

        VerifyRunShortcutStateGate(runOnce, (ToolStripItem)runOnce.Tag, "F5");
        VerifyRunShortcutStateGate(runContinuous, (ToolStripItem)runContinuous.Tag, "F6");
    }

    private static void VerifyRunShortcutStateGate(
        ToolStripMenuItem shortcut,
        ToolStripItem source,
        string description)
    {
        bool originalSourceEnabled = source.Enabled;
        bool originalSourceAvailable = source.Available;
        bool originalShortcutEnabled = shortcut.Enabled;
        Delegate originalSourceHandlers = GetComponentEventHandler(
            source,
            typeof(ToolStripItem),
            "eventclick",
            "sclickevent");
        Assert(originalSourceHandlers != null,
            description + " source command has no production Click handler.");

        foreach (Delegate handler in originalSourceHandlers.GetInvocationList())
            source.Click -= (EventHandler)handler;
        int clickCount = 0;
        EventHandler observer = delegate { clickCount++; };
        source.Click += observer;
        try
        {
            shortcut.Enabled = true;
            source.Enabled = false;
            source.Available = true;
            shortcut.PerformClick();
            Assert(clickCount == 0,
                description + " must not execute while its command source is disabled.");

            shortcut.Enabled = true;
            source.Enabled = true;
            source.Available = false;
            shortcut.PerformClick();
            Assert(clickCount == 0,
                description + " must not execute while its command source is unavailable.");

            shortcut.Enabled = true;
            source.Enabled = true;
            source.Available = true;
            shortcut.PerformClick();
            Assert(clickCount == 1,
                description + " must execute its enabled/available source exactly once.");
        }
        finally
        {
            source.Click -= observer;
            foreach (Delegate handler in originalSourceHandlers.GetInvocationList())
                source.Click += (EventHandler)handler;
            source.Enabled = originalSourceEnabled;
            source.Available = originalSourceAvailable;
            shortcut.Enabled = originalShortcutEnabled;
        }
    }

    private static bool ImageHasVisibleInk(Image image)
    {
        Bitmap bitmap = image as Bitmap;
        if (bitmap != null)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y).A != 0)
                        return true;
                }
            }
            return false;
        }

        using (Bitmap copy = new Bitmap(image.Width, image.Height))
        {
            using (Graphics graphics = Graphics.FromImage(copy))
                graphics.DrawImageUnscaled(image, Point.Empty);
            return ImageHasVisibleInk(copy);
        }
    }

    private static double RenderAndMeasureControlColor(
        Control control,
        Color expected,
        int tolerance,
        string outputPath,
        string metricName)
    {
        using (Bitmap bitmap = new Bitmap(control.Width, control.Height))
        {
            control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
            if (!string.IsNullOrEmpty(outputPath))
            {
                string directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);
                bitmap.Save(outputPath, ImageFormat.Png);
            }
            int matches = 0;
            int pixels = bitmap.Width * bitmap.Height;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (ColorsNear(bitmap.GetPixel(x, y), expected, tolerance))
                        matches++;
                }
            }
            double coverage = pixels == 0 ? 0D : matches / (double)pixels;
            Color center = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);
            Color corner = bitmap.GetPixel(0, 0);
            Color inner = bitmap.GetPixel(
                Math.Min(bitmap.Width - 1, 8),
                Math.Min(bitmap.Height - 1, 8));
            Console.WriteLine(
                "{0}: center=#{1:X8}; corner=#{2:X8}; inner8=#{3:X8}; soft_blue_coverage={4:P2}",
                metricName,
                center.ToArgb(),
                corner.ToArgb(),
                inner.ToArgb(),
                coverage);
            return coverage;
        }
    }

    private static void AssertRoundedPaintedSurface(Control control, Color expectedFill, string description)
    {
        Assert(control.Region == null,
            description + " must avoid binary Region clipping for its small rounded geometry.");
        Assert(control.BackgroundImage != null &&
               ImageCenterMatches(control.BackgroundImage, expectedFill, 4) &&
               ImageCornerHasTransparency(control.BackgroundImage),
            description + " must use an anti-aliased warm/light-blue painted surface.");
    }

    private static bool ImageCenterMatches(Image image, Color expected, int tolerance)
    {
        if (image == null || image.Width <= 0 || image.Height <= 0)
            return false;

        Bitmap bitmap = image as Bitmap;
        if (bitmap != null)
            return ColorsNear(bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2), expected, tolerance);

        using (Bitmap copy = new Bitmap(image.Width, image.Height))
        {
            using (Graphics graphics = Graphics.FromImage(copy))
                graphics.DrawImageUnscaled(image, Point.Empty);
            return ColorsNear(copy.GetPixel(copy.Width / 2, copy.Height / 2), expected, tolerance);
        }
    }

    private static bool ImageCornerHasTransparency(Image image)
    {
        if (image == null || image.Width <= 0 || image.Height <= 0)
            return false;

        Bitmap bitmap = image as Bitmap;
        if (bitmap != null)
            return bitmap.GetPixel(0, 0).A < 255;

        using (Bitmap copy = new Bitmap(image.Width, image.Height))
        {
            using (Graphics graphics = Graphics.FromImage(copy))
                graphics.DrawImageUnscaled(image, Point.Empty);
            return copy.GetPixel(0, 0).A < 255;
        }
    }

    private static bool ColorsNear(Color actual, Color expected, int tolerance)
    {
        return Math.Abs(actual.R - expected.R) <= tolerance &&
               Math.Abs(actual.G - expected.G) <= tolerance &&
               Math.Abs(actual.B - expected.B) <= tolerance;
    }

    private static IEnumerable<ToolStripItem> EnumerateToolStripItems(ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            yield return item;
            ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;
            if (dropDownItem == null)
                continue;

            foreach (ToolStripItem child in EnumerateToolStripItems(dropDownItem.DropDownItems))
                yield return child;
        }
    }

    private static void RaiseDropDownOpening(ToolStripDropDownItem item)
    {
        MethodInfo onDropDownOpening = typeof(ToolStripDropDownItem).GetMethod(
            "OnDropDownOpening",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (onDropDownOpening == null)
        {
            // .NET Framework 4.x raises DropDownOpening from the protected
            // OnDropDownShow hook; newer WinForms builds expose the clearer name.
            onDropDownOpening = typeof(ToolStripDropDownItem).GetMethod(
                "OnDropDownShow",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }
        Assert(item != null && onDropDownOpening != null,
            "ToolStrip drop-down opening hook is unavailable.");
        onDropDownOpening.Invoke(item, new object[] { EventArgs.Empty });
    }

    private static bool HasClickHandler(ToolStripItem item)
    {
        return HasComponentEventHandler(item, typeof(ToolStripItem), "eventclick", "sclickevent");
    }

    private static bool HasPaintHandler(Control control)
    {
        return HasComponentEventHandler(control, typeof(Control), "eventpaint", "spaintevent");
    }

    private static void CaptureTextChange(string value)
    {
        capturedTextChangeCount++;
        capturedTextChangeValue = value;
    }

    private static void CaptureNumericChange(double value)
    {
        capturedNumericChangeCount++;
        capturedNumericChangeValue = value;
    }

    private static void RaiseControlLeave(Control control)
    {
        MethodInfo onLeave = typeof(Control).GetMethod(
            "OnLeave",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert(onLeave != null, "WinForms Control.OnLeave is unavailable.");
        onLeave.Invoke(control, new object[] { EventArgs.Empty });
    }

    private static bool HasComponentEventHandler(
        Component component,
        Type eventOwner,
        string classicEventKeyName,
        string modernEventKeyName)
    {
        return GetComponentEventHandler(
            component,
            eventOwner,
            classicEventKeyName,
            modernEventKeyName) != null;
    }

    private static Delegate GetComponentEventHandler(
        Component component,
        Type eventOwner,
        string classicEventKeyName,
        string modernEventKeyName)
    {
        if (component == null || eventOwner == null)
            return null;

        PropertyInfo eventsProperty = typeof(Component).GetProperty(
            "Events",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (eventsProperty == null)
            return null;

        FieldInfo eventKeyField = null;
        for (Type type = eventOwner; type != null && eventKeyField == null; type = type.BaseType)
        {
            foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                string normalizedName = field.Name.Replace("_", string.Empty).ToLowerInvariant();
                if (field.FieldType == typeof(object) &&
                    (normalizedName == classicEventKeyName || normalizedName == modernEventKeyName))
                {
                    eventKeyField = field;
                    break;
                }
            }
        }
        if (eventKeyField == null)
            return null;

        EventHandlerList events = eventsProperty.GetValue(component, null) as EventHandlerList;
        object eventKey = eventKeyField.GetValue(null);
        Delegate handler = events == null || eventKey == null ? null : events[eventKey];
        return handler != null && handler.GetInvocationList().Length > 0 ? handler : null;
    }

    private static TreeNode FindToolCategory(TreeView tools, string text)
    {
        TreeNode node = tools.Nodes.Cast<TreeNode>().FirstOrDefault(candidate => candidate.Text == text);
        Assert(node != null, "Missing toolbox category: " + text);
        return node;
    }

    private static LayoutStub FindLayoutStub(DockPanel dockPanel, string persistString)
    {
        LayoutStub content = dockPanel.Contents
            .Cast<IDockContent>()
            .OfType<LayoutStub>()
            .FirstOrDefault(candidate => candidate.PersistString == persistString);
        Assert(content != null, "Missing layout content: " + persistString);
        return content;
    }

    private static DockContent FindDockContent(DockPanel dockPanel, string typeName)
    {
        DockContent content = dockPanel.Contents
            .Cast<IDockContent>()
            .OfType<DockContent>()
            .FirstOrDefault(candidate => candidate.GetType().FullName == typeName);
        Assert(content != null, "Missing live Dock content: " + typeName);
        return content;
    }

    private static T FindControl<T>(Control root, string name) where T : Control
    {
        T control = root.Controls.Find(name, true).OfType<T>().FirstOrDefault();
        Assert(control != null, "Missing child control: " + name);
        return control;
    }

    private static T GetField<T>(Type type, object instance, string name) where T : class
    {
        FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert(field != null, "Missing field: " + name);
        T value = field.GetValue(instance) as T;
        Assert(value != null, "Unexpected field type: " + name);
        return value;
    }

    private static bool IsLocallyVisible(Control control)
    {
        MethodInfo getState = typeof(Control).GetMethod("GetState", BindingFlags.Instance | BindingFlags.NonPublic);
        return (bool)getState.Invoke(control, new object[] { 2 }); // STATE_VISIBLE
    }

    private static void SetLocalVisible(Control control, bool visible)
    {
        MethodInfo setState = typeof(Control).GetMethod("SetState", BindingFlags.Instance | BindingFlags.NonPublic);
        setState.Invoke(control, new object[] { 2, visible }); // STATE_VISIBLE
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
        Interlocked.Increment(ref assertionCount);
    }
}
