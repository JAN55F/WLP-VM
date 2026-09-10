using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using HalconDotNet;
using ViewWindow.Model;

// Windows + HALCON 17.12. Compile with Start/HalconRuntime.cs and place beside WLP VM.exe.
// Uses generated images and the tool editor only; never calls VM.Init or Machine.InitAll.
internal static class FindLineDisplaySmoke
{
    private const BindingFlags Members = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.Static;
    private static int checks;

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: FindLineDisplaySmoke <CVMPro.dll>");
            return 2;
        }
        try
        {
            Start.HalconRuntime.EnsureLoaded();
            RunChecks(args[0]);
            Console.WriteLine("FindLine display checks passed: " + checks);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RunChecks(string assemblyPath)
    {
        Application.EnableVisualStyles();
        Control.CheckForIllegalCrossThreadCalls = true;
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        Type jobType = assembly.GetType("VMPro.Job", true);
        // Disable the debounce timer; each preview below is invoked explicitly.
        jobType.GetField("loadForm", Members).SetValue(null, true);
        object tool = Activator.CreateInstance(assembly.GetType("VMPro.FindLineTool", true), true);
        Type editorType = assembly.GetType("VMPro.Frm_FindLineTool", true);
        using (Form editor = (Form)editorType.GetProperty("Instance", Members).GetValue(null, null))
        {
            HObject blank = null, leftHalf = null, whiteToBlack = null, blackToWhite = null;
            try
            {
                HOperatorSet.GenImageConst(out blank, "byte", 640, 480);
                HOperatorSet.GenRectangle1(out leftHalf, 0, 0, 479, 319);
                HOperatorSet.PaintRegion(leftHalf, blank, out whiteToBlack, 255, "fill");
                HOperatorSet.InvertImage(whiteToBlack, out blackToWhite);
                object input = Get(Get(tool, "toolPar"), "InputPar");
                Set(input, "图像", blank);
                IList rois = (IList)Get(tool, "L_regions");
                rois.Clear();
                rois.Add(new ROIRectangle2(240, 320, 0, 60, 140));
                Set(tool, "cliperNum", 12);
                Set(tool, "caliperWidth", 5);
                Set(tool, "displayFeature", false);
                Set(tool, "displayLine", false);
                Invoke(editor, "BindTool", tool, true);
                editor.StartPosition = FormStartPosition.Manual;
                editor.Location = new Point(-32000, -32000);
                editor.ShowInTaskbar = false;
                editor.Show();
                Application.DoEvents();

                object combo = Get(editor, "cbx_polarity");
                Assert(((string[])Get(combo, "Items")).SequenceEqual(
                    new[] { "由白到黑", "由黑到白", "任意极性" }), "Polarity labels/order mismatch.");
                foreach (string value in new[] { "positive", "negative", "all" })
                {
                    Set(tool, "polarity", value);
                    Invoke(editor, "BindTool", tool, true);
                    Assert((int)Get(combo, "SelectedIndex") ==
                        (value == "positive" ? 0 : value == "negative" ? 1 : 2),
                        "Stored polarity did not bind correctly: " + value);
                    Assert((string)Get(tool, "polarity") == value, "Binding changed stored polarity.");
                }

                Invoke(tool, "ShowContour", true, true, false);
                Assert(CaliperCount(editor) > 1, "No-edge preview lost the calipers.");
                Invoke(tool, "Run", true, true, "直线查找测试");
                Assert(Get(tool, "toolRunStatu").ToString() == "未找到线", "Blank image produced a line.");
                Assert(CaliperCount(editor) > 1, "No-edge run lost the calipers.");

                double before = CaliperCenterCol(editor);
                rois[0] = new ROIRectangle2(240, 340, 0, 60, 140);
                Invoke(tool, "ShowDraggingPreview");
                Assert(CaliperCount(editor) > 1, "Dragging lost the calipers.");
                Assert(Math.Abs(CaliperCenterCol(editor) - before - 20) < 1,
                    "Calipers did not follow the moved ROI.");
                object checkbox = Get(editor, "ckb_displayCaliper");
                Set(checkbox, "Checked", false);
                Invoke(tool, "ShowDraggingPreview");
                Assert(!(bool)Get(tool, "displayCaliper") && CaliperCount(editor) == 0,
                    "Disabling calipers did not clear their overlays.");
                Set(checkbox, "Checked", true);
                Invoke(tool, "ShowDraggingPreview");
                Assert(CaliperCount(editor) > 1, "Re-enabling calipers did not restore their overlays.");
                Invoke(tool, "EditCaliper");
                Assert(CaliperCount(editor) > 1, "Entering ROI edit lost the calipers.");

                rois[0] = new ROIRectangle2(240, 320, 0, 60, 140);
                foreach (bool reversed in new[] { false, true })
                {
                    Set(input, "图像", reversed ? blackToWhite : whiteToBlack);
                    for (int index = 0; index < 3; index++)
                    {
                        Invoke(combo, "SelectUserItem", index);
                        string expected = index == 0 ? "positive" : index == 1 ? "negative" : "all";
                        Assert((string)Get(tool, "polarity") == expected, "UI did not update the metrology parameter.");
                        Invoke(tool, "Run", true, true, "直线查找测试");
                        bool shouldFind = index == 2 || index == (reversed ? 1 : 0);
                        Assert((Get(tool, "toolRunStatu").ToString() == "成功") == shouldFind,
                            "Polarity disagrees with the right-pointing ROI arrow: " + expected);
                        Assert(CaliperCount(editor) > 1, "A polarity result cleared the calipers.");
                        if (shouldFind)
                        {
                            object line = Get(Get(Get(tool, "toolPar"), "ResultPar"), "线");
                            Assert(Math.Abs((double)Get(Get(line, "起点"), "Y") - 319.5) < 2 &&
                                   Math.Abs((double)Get(Get(line, "终点"), "Y") - 319.5) < 2,
                                "Successful detection did not publish the measured line.");
                        }
                    }
                }
                double saved = ((HTuple)Get(tool, "ResultLineStartCol")).D;
                Set(input, "图像", blank);
                Invoke(tool, "ShowContour", true, true, false);
                Assert(((HTuple)Get(tool, "ResultLineStartCol")).D == saved,
                    "Preview overwrote the last formal result.");
            }
            finally
            {
                jobType.GetField("loadForm", Members).SetValue(null, false);
                editor.Close();
                foreach (HObject obj in new[] { blank, leftHalf, whiteToBlack, blackToWhite })
                    if (obj != null) obj.Dispose();
            }
        }
    }

    private static HObject Calipers(Form editor)
    {
        object view = Get(Get(editor, "hWindow_Final1"), "viewWindow");
        IEnumerable overlays = (IEnumerable)Get(Get(view, "_hWndControl"), "hObjectList");
        foreach (object overlay in overlays)
            if ((string)Get(overlay, "Color") == "#4c94d2")
                return (HObject)Get(overlay, "HObject");
        return null;
    }

    private static int CaliperCount(Form editor)
    {
        HObject contours = Calipers(editor);
        if (contours == null) return 0;
        HTuple count;
        HOperatorSet.CountObj(contours, out count);
        return count.I;
    }

    private static double CaliperCenterCol(Form editor)
    {
        HTuple r1, c1, r2, c2;
        HOperatorSet.SmallestRectangle1Xld(Calipers(editor), out r1, out c1, out r2, out c2);
        return (c1.TupleMin().D + c2.TupleMax().D) / 2;
    }

    private static object Get(object target, string name)
    {
        PropertyInfo property = target.GetType().GetProperty(name, Members);
        return property != null ? property.GetValue(target, null) :
            target.GetType().GetField(name, Members).GetValue(target);
    }

    private static void Set(object target, string name, object value)
    {
        PropertyInfo property = target.GetType().GetProperty(name, Members);
        if (property != null) property.SetValue(target, value, null);
        else target.GetType().GetField(name, Members).SetValue(target, value);
    }

    private static void Invoke(object target, string name, params object[] args)
    {
        target.GetType().GetMethods(Members).Single(m => m.Name == name &&
            m.GetParameters().Length == args.Length).Invoke(target, args);
    }

    private static void Assert(bool condition, string message)
    {
        checks++;
        if (!condition) throw new InvalidOperationException(message);
    }
}
