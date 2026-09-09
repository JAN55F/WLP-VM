using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

internal static class LayoutTemplateGenerator
{
    private sealed class LayoutStub : DockContent
    {
        private readonly string persistString;

        internal LayoutStub(string value)
        {
            persistString = value;
            HideOnClose = true;
        }

        protected override string GetPersistString()
        {
            return persistString;
        }
    }

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 1)
            return 2;

        try
        {
            using (Form form = new Form())
            using (DockPanel dockPanel = new DockPanel())
            using (LayoutStub toolBox = new LayoutStub("VMPro.Frm_ToolBox"))
            using (LayoutStub job = new LayoutStub("VMPro.Frm_Job"))
            using (LayoutStub output = new LayoutStub("VMPro.Frm_Output"))
            using (LayoutStub monitor = new LayoutStub("VMPro.Frm_Monitor"))
            using (LayoutStub image = new LayoutStub("VMPro.Frm_ImageWindow"))
            {
                form.ClientSize = new Size(1366, 768);
                form.IsMdiContainer = true;
                dockPanel.Dock = DockStyle.Fill;
                dockPanel.DockLeftPortion = 0.24D;
                dockPanel.DockRightPortion = 0.30D;
                dockPanel.DockBottomPortion = 0.22D;
                form.Controls.Add(dockPanel);
                form.CreateControl();

                job.Show(dockPanel, DockState.DockRight);
                toolBox.Show(job.Pane, null);
                output.Show(dockPanel, DockState.DockBottom);
                monitor.Show(output.Pane, null);
                image.Show(dockPanel, DockState.Document);

                // 右侧流程/工具箱应贯穿整个视觉工作区高度；底部输出/监控
                // 只占中央图像列，不能横向延伸到右侧编辑列下面。
                dockPanel.UpdateDockWindowZOrder(DockStyle.Right, true);

                // 工具箱/流程以及输出/监控分别共用一个标签式面板；默认显示
                // 流程和输出，中央图像与日志上下分区，右侧完整留给编辑。
                job.Activate();
                output.Activate();
                image.Activate();
                dockPanel.SaveAsXml(args[0]);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.GetType().FullName);
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                Console.Error.WriteLine(ex.InnerException.GetType().FullName);
                Console.Error.WriteLine(ex.InnerException.Message);
            }
            return 1;
        }

        return 0;
    }
}
