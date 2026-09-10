using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace VMPro
{
    internal partial class Frm_Main
    {
        private const int TitleBandHeight = 38;
        private const int MainCommandBarHeight = 50;
        private const int VisionCommandBarHeight = 40;
        private const int IntegratedMenuLeft = 300;
        private const int WorkspaceCommandReserve = 214;

        private ToolStripStatusLabel statusSpring;
        private System.Windows.Forms.Label productMarkLabel;
        private ToolStripMenuItem menuImage;
        private ToolStripMenuItem menuDevice;
        private ToolStripMenuItem menuHelp;
        private bool homeWorkspaceLoaded;
        private bool motionWorkspaceLoaded;
        private bool refreshTickBusy;
        private int nextClockRefreshTick;
        private int nextIoRefreshTick;

        internal void ApplyModernMainLayout()
        {
            int nowTick = Environment.TickCount;
            nextClockRefreshTick = nowTick;
            nextIoRefreshTick = nowTick;
            SuspendLayout();
            panel1.SuspendLayout();
            try
            {
                Text = Configuration.ProductDisplayName;
                ApplyExecutableIcon();
                BackColor = ModernUiTheme.Page;
                ForeColor = ModernUiTheme.PrimaryText;
                Font = ModernUiTheme.UiFont;
                FormBorderStyle = FormBorderStyle.None;
                MinimumSize = new Size(1024, 640);
                Padding = new Padding(1);
                UpdateMaximizedBoundsForCurrentScreen();
                LocationChanged += Frm_Main_ModernLocationChanged;
                VisibleChanged += Frm_Main_ModernVisibleChanged;

                ConfigureTitleBand();
                ConfigurePrimaryCommandBar();
                ConfigureMenuBar();
                ConfigureVisionCommandBar();
                ConfigureStatusBar();
                ConfigureContentArea();
                ConfigureDockPanelSkin();

                tim_recordTime.Interval = 100;
                ModernUiTheme.Apply(this);
            }
            finally
            {
                panel1.ResumeLayout(true);
                ResumeLayout(true);
            }
        }

        private void ApplyExecutableIcon()
        {
            try
            {
                using (Icon executableIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath))
                {
                    if (executableIcon != null)
                        Icon = (Icon)executableIcon.Clone();
                }
            }
            catch
            {
                // 非标准宿主或设计器没有关联图标时保留 Designer 回退图标。
            }
        }

        private void ConfigureTitleBand()
        {
            panel1.BackColor = ModernUiTheme.Accent;
            panel1.Dock = DockStyle.Top;
            panel1.Height = TitleBandHeight + MainCommandBarHeight;
            panel1.Padding = Padding.Empty;

            // 旧位图 Logo 中固化了“vm”字样，改用清晰文字品牌标，
            // 避免软件改名后主标题左侧仍出现旧标识。
            pictureBox1.Visible = false;
            productMarkLabel = new System.Windows.Forms.Label();
            productMarkLabel.Name = "mainProductMark";
            productMarkLabel.BackColor = Color.Transparent;
            productMarkLabel.ForeColor = Color.White;
            productMarkLabel.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point);
            productMarkLabel.Location = new Point(10, 0);
            productMarkLabel.Size = new Size(42, TitleBandHeight);
            productMarkLabel.Text = "WLP";
            productMarkLabel.TextAlign = ContentAlignment.MiddleCenter;
            productMarkLabel.MouseDown += setForm_MouseDown;
            productMarkLabel.MouseMove += setForm_MouseMove;
            productMarkLabel.MouseUp += setForm_MouseUp;
            panel1.Controls.Add(productMarkLabel);
            productMarkLabel.BringToFront();

            lbl_title.BackColor = Color.Transparent;
            lbl_title.ForeColor = Color.White;
            lbl_title.Font = ModernUiTheme.TitleFont;
            lbl_title.Text = Configuration.BuildApplicationTitle(
                Configuration.DefaultProgramTitle);
            lbl_title.Location = new Point(58, 0);
            lbl_title.Height = TitleBandHeight;
            lbl_title.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panel3.BackColor = ModernUiTheme.Accent;
            // 主菜单已包含完整“帮助”分类，旧的标题栏更多按钮只会重复同一组
            // 命令。标题栏仅保留常见的最小化、最大化/还原和关闭。
            button4.Visible = false;
            panel3.Size = new Size(120, TitleBandHeight);
            panel3.Location = new Point(panel1.Width - panel3.Width, 0);
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            Button[] titleButtons = { button1, button2, button3 };
            string[] accessibleNames = { "最小化", "最大化或还原", "关闭" };
            for (int index = 0; index < titleButtons.Length; index++)
            {
                Button button = titleButtons[index];
                button.Location = new Point(index * 40, 0);
                button.Size = new Size(40, TitleBandHeight);
                button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                button.BackColor = ModernUiTheme.Accent;
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                bool isCloseButton = button == button3;
                button.FlatAppearance.MouseOverBackColor = isCloseButton ? ModernUiTheme.Danger : ModernUiTheme.AccentHover;
                button.FlatAppearance.MouseDownBackColor = isCloseButton ? Color.FromArgb(183, 45, 45) : ModernUiTheme.AccentPressed;
                button.AccessibleName = accessibleNames[index];
                toolTip1.SetToolTip(button, accessibleNames[index]);
            }

            ConfigureMainTitleButtonIcons();
        }

        private void ConfigureMainTitleButtonIcons()
        {
            int iconSize = ModernVectorIconFactory.GetPixelSize(panel3, 18);
            Button[] titleButtons = { button1, button2, button3 };
            foreach (Button button in titleButtons)
            {
                button.Text = string.Empty;
                button.BackgroundImage = null;
                button.BackgroundImageLayout = ImageLayout.Center;
                button.ImageAlign = ContentAlignment.MiddleCenter;
            }

            button1.Image = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Minimize, iconSize, Color.White);
            button3.Image = ModernVectorIconFactory.Get(
                ModernVectorIconFactory.Glyph.Close, iconSize, Color.White);
            UpdateMainMaximizeButtonGlyph();
        }

        private void UpdateMainMaximizeButtonGlyph()
        {
            if (button2 == null || button2.IsDisposed)
                return;
            int iconSize = ModernVectorIconFactory.GetPixelSize(panel3, 18);
            ModernVectorIconFactory.Glyph glyph = WindowState == FormWindowState.Maximized
                ? ModernVectorIconFactory.Glyph.Restore
                : ModernVectorIconFactory.Glyph.Maximize;
            button2.Image = ModernVectorIconFactory.Get(glyph, iconSize, Color.White);
        }

        private void ConfigurePrimaryCommandBar()
        {
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.Height = MainCommandBarHeight;
            toolStrip1.AutoSize = false;
            toolStrip1.BackColor = ModernUiTheme.Surface;
            toolStrip1.ForeColor = ModernUiTheme.PrimaryText;
            toolStrip1.Font = ModernUiTheme.UiFont;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Padding = new Padding(8, 1, 8, 1);
            toolStrip1.ShowItemToolTips = true;
            toolStrip1.CanOverflow = true;
            toolStrip1.Renderer = ModernUiTheme.ToolStripRenderer;

            toolStripButton6.Visible = false;

            ToolStripButton[] machineCommands = { toolStripButton4, toolStripButton36, toolStripButton3, toolStripButton8 };
            foreach (ToolStripButton button in machineCommands)
                ConfigurePrimaryButton(button, 64, ToolStripItemAlignment.Left);

            // 只把全局机器命令和工作区导航放在第一层。退出、登录、
            // 锁定和选项收入“系统”菜单，避免与菜单栏重复。原按钮对象保留
            // 作为事件代理，不绕过既有权限与退出确认。
            ToolStripButton[] workspaceCommands =
            {
                toolStripButton1,
                toolStripButton5,
                toolStripButton9
            };
            foreach (ToolStripButton button in workspaceCommands)
                ConfigurePrimaryButton(button, 60, ToolStripItemAlignment.Right);

            toolStrip1.Items.Clear();
            toolStrip1.Items.AddRange(new ToolStripItem[]
            {
                toolStripButton4,
                toolStripButton36,
                toolStripButton3,
                toolStripButton8,
                toolStripSeparator1,
                toolStripButton1,
                toolStripButton5,
                toolStripButton9
            });

            toolStripSeparator1.Margin = new Padding(4, 5, 4, 5);
            toolStripSeparator1.Size = new Size(6, 42);
            ConfigureModernMainCommandIcons();
        }

        private static void ConfigurePrimaryButton(ToolStripButton button, int width, ToolStripItemAlignment alignment)
        {
            button.Alignment = alignment;
            button.AutoSize = false;
            button.Size = new Size(width, 44);
            button.Margin = new Padding(2, 1, 2, 1);
            button.Padding = Padding.Empty;
            button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            button.ImageAlign = ContentAlignment.TopCenter;
            button.TextAlign = ContentAlignment.BottomCenter;
            button.TextImageRelation = TextImageRelation.ImageAboveText;
            button.ImageScaling = ToolStripItemImageScaling.SizeToFit;
            button.ForeColor = ModernUiTheme.PrimaryText;
            button.Font = ModernUiTheme.UiFont;
            button.AccessibleName = button.Text.Trim();
            if (string.IsNullOrEmpty(button.ToolTipText))
                button.ToolTipText = button.Text.Trim();
        }

        private void ConfigureMenuBar()
        {
            bool english = Project.Instance.configuration.language == Language.English;

            menuImage = new ToolStripMenuItem(english ? "Image" : "图像");
            menuDevice = new ToolStripMenuItem(english ? "Device" : "设备");
            menuHelp = new ToolStripMenuItem(english ? "Help" : "帮助");

            // 项目：稳定的文件生命周期入口。
            文件ToolStripMenuItem.Text = english ? "Project" : "项目";
            新建流程ToolStripMenuItem.Text = english ? "New solution" : "新建方案";
            最近的项目ToolStripMenuItem.Text = english ? "Recent solutions" : "最近方案";
            打开流程ToolStripMenuItem.Text = english ? "Open solution..." : "打开方案...";
            toolStripMenuItem1.Text = english ? "Clone solution" : "克隆方案";
            保存ToolStripMenuItem.Text = english ? "Save project" : "保存项目";
            打开ToolStripMenuItem.Text = english ? "Import project..." : "导入项目...";
            关闭ToolStripMenuItem.Text = english ? "Export project..." : "导出项目...";
            退出ToolStripMenuItem1.Text = english ? "Exit" : "退出";
            文件ToolStripMenuItem.DropDownItems.Clear();
            文件ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                新建流程ToolStripMenuItem,
                打开流程ToolStripMenuItem,
                最近的项目ToolStripMenuItem,
                toolStripMenuItem1,
                CreateProxyMenuItem(english ? "Export solution..." : "导出方案...", toolStripButton14),
                new ToolStripSeparator(),
                保存ToolStripMenuItem,
                打开ToolStripMenuItem,
                关闭ToolStripMenuItem,
                new ToolStripSeparator(),
                退出ToolStripMenuItem1
            });

            // 流程：编辑操作与运行命令同属一个业务模块，运行作为子菜单。
            流程ToolStripMenuItem.Text = english ? "Workflow" : "流程";
            新建ToolStripMenuItem.Text = english ? "New workflow" : "新建流程";
            导入ToolStripMenuItem1.Text = english ? "Import workflow..." : "导入流程...";
            导出ToolStripMenuItem1.Text = english ? "Export workflow..." : "导出流程...";
            克隆ToolStripMenuItem1.Text = english ? "Clone workflow" : "克隆流程";
            删除ToolStripMenuItem.Text = english ? "Delete workflow" : "删除流程";
            辅助ToolStripMenuItem.Text = english ? "Run" : "运行";
            辅助ToolStripMenuItem.Visible = true;
            运行一次ToolStripMenuItem.Text = english ? "Run selected once" : "单次运行当前流程";
            连续运行ToolStripMenuItem.Text = english ? "Run selected continuously" : "连续运行当前流程";
            停止连续ToolStripMenuItem.Text = english ? "Stop selected continuous run" : "停止当前流程";
            // 保留设计器中的 F5/F6 快捷键，但让菜单状态和真正的工具栏命令
            // 使用同一 Enabled/Available 门槛，避免机器运行期间从菜单绕过禁用态。
            运行一次ToolStripMenuItem.Tag = toolStripButton11;
            连续运行ToolStripMenuItem.Tag = toolStripButton12;
            辅助ToolStripMenuItem.DropDownItems.Clear();
            辅助ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                运行一次ToolStripMenuItem,
                连续运行ToolStripMenuItem,
                停止连续ToolStripMenuItem,
                new ToolStripSeparator(),
                CreateProxyMenuItem(english ? "Run all once" : "全部单次运行", toolStripButton35),
                CreateProxyMenuItem(english ? "Start/stop all continuous runs" : "全部连续运行/停止", toolStripButton16)
            });
            辅助ToolStripMenuItem.DropDownOpening += delegate
            {
                SynchronizeProxyMenuItems(辅助ToolStripMenuItem.DropDownItems);
                停止连续ToolStripMenuItem.Enabled = IsSelectedContinuousRunActive();
            };
            流程ToolStripMenuItem.DropDownItems.Clear();
            流程ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                新建ToolStripMenuItem,
                导入ToolStripMenuItem1,
                导出ToolStripMenuItem1,
                克隆ToolStripMenuItem1,
                删除ToolStripMenuItem,
                new ToolStripSeparator(),
                辅助ToolStripMenuItem
            });

            // 视觉：图像 I/O、工作区面板、布局和辅助视觉工具。
            读取图像ToolStripMenuItem.Text = english ? "Open image..." : "读取图像...";
            保存窗口原图ToolStripMenuItem.Text = english ? "Save original image..." : "保存原图...";
            保存窗口ToolStripMenuItem.Text = english ? "Save viewport..." : "保存窗口...";
            两点测距ToolStripMenuItem.Text = english ? "Measure distance" : "两点测距";
            ToolStripMenuItem previousLocalImageMenuItem = CreateProxyMenuItem(
                english ? "Previous local image" : "上一张本地图像",
                toolStripButton26);
            menuImage.DropDownOpening += delegate
            {
                previousLocalImageMenuItem.Enabled = CanNavigatePreviousLocalImage();
            };
            menuImage.DropDownItems.AddRange(new ToolStripItem[]
            {
                读取图像ToolStripMenuItem,
                previousLocalImageMenuItem,
                CreateProxyMenuItem(
                    english ? "Pause directory-image auto advance" : "暂停目录图自动切换",
                    tsb_stopSwtich),
                保存窗口原图ToolStripMenuItem,
                保存窗口ToolStripMenuItem,
                new ToolStripSeparator(),
                两点测距ToolStripMenuItem
            });

            ToolStripMenuItem workspacePanels = new ToolStripMenuItem(english ? "Workspace panels" : "工作区面板");
            流程编辑ToolStripMenuItem.Text = english ? "Workflow editor" : "流程编辑器";
            图像窗口ToolStripMenuItem.Text = english ? "New image window..." : "新增图像窗口...";
            工具箱ToolStripMenuItem.Text = english ? "Toolbox" : "工具箱";
            输出ToolStripMenuItem.Text = english ? "Output and log" : "输出与日志";
            workspacePanels.DropDownItems.AddRange(new ToolStripItem[]
            {
                流程编辑ToolStripMenuItem,
                图像窗口ToolStripMenuItem,
                工具箱ToolStripMenuItem,
                输出ToolStripMenuItem
            });

            工具ToolStripMenuItem.Text = english ? "Utilities" : "辅助工具";
            工具ToolStripMenuItem.DropDownItems.Clear();
            ToolStripMenuItem speedModeMenuItem = CreateProxyMenuItem(
                english ? "High-speed mode" : "极速模式",
                toolStripButton27);
            speedModeMenuItem.CheckOnClick = false;
            speedModeMenuItem.Checked = Configuration.SpeedMode;
            speedModeMenuItem.ToolTipText = english
                ? "Improves run speed by disabling selected visual effects"
                : "通过关闭部分视觉效果提升运行速度";
            speedModeMenuItem.Click += delegate
            {
                speedModeMenuItem.Checked = Configuration.SpeedMode;
            };
            工具ToolStripMenuItem.DropDownOpening += delegate
            {
                speedModeMenuItem.Checked = Configuration.SpeedMode;
            };
            工具ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                speedModeMenuItem
            });

            试图ToolStripMenuItem.Text = english ? "Vision" : "视觉";
            布局ToolStripMenuItem.Text = english ? "Layout" : "布局";
            布局管理ToolStripMenuItem1.Text = english ? "Manage layout..." : "布局管理...";
            解锁ToolStripMenuItem.Text = english ? "Lock layout" : "锁定布局";
            试图ToolStripMenuItem.DropDownItems.Clear();
            试图ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuImage,
                workspacePanels,
                布局ToolStripMenuItem,
                CreateProxyMenuItem(english ? "Global variables..." : "全局变量...", toolStripButton34),
                工具ToolStripMenuItem
            });

            // 设备：所有硬件与通信入口归于一处。
            采集设备ToolStripMenuItem.Text = english ? "Image acquisition devices..." : "图像采集设备...";
            menuDevice.DropDownItems.AddRange(new ToolStripItem[]
            {
                采集设备ToolStripMenuItem,
                CreateProxyMenuItem(english ? "Device and communication..." : "设备与通信...", toolStripButton33),
                new ToolStripSeparator(),
                CreateProxyMenuItem(english ? "Motion control workspace" : "运动控制工作区", toolStripButton1)
            });

            // 系统：设置、会话、权限、日志与维护入口。
            系统ToolStripMenuItem.Text = english ? "System" : "系统";
            登录ToolStripMenuItem.Text = english ? "Sign in..." : "登录...";
            锁定ToolStripMenuItem.Text = english ? "Lock" : "锁定";
            数据ToolStripMenuItem.Text = english ? "Open log folder" : "打开日志目录";
            系统重置ToolStripMenuItem.Text = english ? "Reset system..." : "重置系统...";
            虚拟键盘ToolStripMenuItem.Text = english ? "On-screen keyboard" : "屏幕键盘";
            截屏ToolStripMenuItem.Text = english ? "Screenshot" : "截图";
            系统ToolStripMenuItem.DropDownItems.Clear();
            系统ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateProxyMenuItem(english ? "Options..." : "选项...", toolStripButton10),
                登录ToolStripMenuItem,
                锁定ToolStripMenuItem,
                new ToolStripSeparator(),
                虚拟键盘ToolStripMenuItem,
                截屏ToolStripMenuItem,
                数据ToolStripMenuItem,
                new ToolStripSeparator(),
                系统重置ToolStripMenuItem
            });

            menuHelp.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateProxyMenuItem(english ? "Help documentation" : "帮助文档", 帮助文档ToolStripMenuItem),
                CreateProxyMenuItem(english ? "Sample project" : "示例项目", 示例项目ToolStripMenuItem),
                CreateProxyMenuItem(english ? "Feedback" : "反馈与建议", 反馈和建议ToolStripMenuItem1),
                new ToolStripSeparator(),
                CreateProxyMenuItem(english ? "Activation" : "激活", 激活ToolStripMenuItem1),
                CreateProxyMenuItem(english ? "About WLP VM" : "关于 WLP VM", 关于ToolStripMenuItem2)
            });

            切换到经典布局1ToolStripMenuItem.Text = english ? "Three-column layout (standard)" : "三列布局（标准）";
            切换到经典布局1ToolStripMenuItem.ToolTipText = english ? "Applied after restart" : "重启软件后生效";
            切换到经典布局2ToolStripMenuItem.Text = english ? "Legacy layout 2" : "兼容布局 2";
            切换到经典布局2ToolStripMenuItem.Visible = File.Exists(ResolveDockLayoutPath("Config\\Resources\\Layout\\经典布局2.config"));

            停止连续ToolStripMenuItem.Click += StopContinuousRunMenuItem_Click;

            tToolStripMenuItem.Visible = false;
            tToolStripMenuItem1.Visible = false;
            tToolStripMenuItem2.Visible = false;
            tToolStripMenuItem3.Visible = false;
            tToolStripMenuItem4.Visible = false;
            tToolStripMenuItem5.Visible = false;
            tToolStripMenuItem6.Visible = false;

            menuStrip1.Items.Clear();
            menuStrip1.Items.AddRange(new ToolStripItem[]
            {
                文件ToolStripMenuItem,
                流程ToolStripMenuItem,
                试图ToolStripMenuItem,
                menuDevice,
                系统ToolStripMenuItem,
                menuHelp
            });

            ToolStripMenuItem[] topMenus =
            {
                文件ToolStripMenuItem,
                流程ToolStripMenuItem,
                试图ToolStripMenuItem,
                menuDevice,
                系统ToolStripMenuItem,
                menuHelp
            };
            foreach (ToolStripMenuItem topMenu in topMenus)
            {
                topMenu.AutoSize = true;
                topMenu.Margin = new Padding(2, 0, 2, 0);
                topMenu.DropDownOpening += delegate(object sender, EventArgs args)
                {
                    ToolStripMenuItem openingMenu = sender as ToolStripMenuItem;
                    if (openingMenu != null)
                        SynchronizeProxyMenuItems(openingMenu.DropDownItems);
                };
            }

            // 六类主菜单与启动/暂停/停止/复位共用同一行。保留原 MenuStrip
            // 和原菜单对象，确保快捷键、代理命令、权限及 DropDownOpening 链不变。
            if (menuStrip1.Parent != panel1)
            {
                if (menuStrip1.Parent != null)
                    menuStrip1.Parent.Controls.Remove(menuStrip1);
                panel1.Controls.Add(menuStrip1);
            }
            menuStrip1.Dock = DockStyle.None;
            menuStrip1.AutoSize = false;
            menuStrip1.Padding = new Padding(6, 10, 6, 10);
            menuStrip1.BackColor = ModernUiTheme.Surface;
            menuStrip1.ForeColor = ModernUiTheme.PrimaryText;
            menuStrip1.Font = ModernUiTheme.UiFont;
            menuStrip1.Renderer = ModernUiTheme.ToolStripRenderer;
            panel1.Resize -= Panel1_AlignIntegratedMenu;
            panel1.Resize += Panel1_AlignIntegratedMenu;
            AlignIntegratedMenuBar();
            menuStrip1.BringToFront();
        }

        private void Panel1_AlignIntegratedMenu(object sender, System.EventArgs e)
        {
            AlignIntegratedMenuBar();
        }

        private void AlignIntegratedMenuBar()
        {
            if (menuStrip1 == null || menuStrip1.Parent != panel1)
                return;

            int availableWidth = panel1.ClientSize.Width - IntegratedMenuLeft - WorkspaceCommandReserve;
            menuStrip1.SetBounds(
                IntegratedMenuLeft,
                TitleBandHeight,
                Math.Max(280, availableWidth),
                MainCommandBarHeight);
            menuStrip1.PerformLayout();
            menuStrip1.Invalidate();
        }

        private static ToolStripMenuItem CreateProxyMenuItem(string text, ToolStripItem source)
        {
            ToolStripMenuItem proxy = new ToolStripMenuItem(text);
            proxy.Name = source == null ? string.Empty : source.Name + "ProxyMenuItem";
            proxy.Tag = source;
            // 主菜单统一使用清晰文字入口。部分被迁移的旧 ToolStripButton 仍携带
            // 低分辨率位图；继续复制会在高 DPI 下产生模糊和风格混杂。
            proxy.Image = null;
            proxy.Click += delegate
            {
                if (source != null && source.Enabled && source.Available)
                    source.PerformClick();
            };
            return proxy;
        }

        private static void SynchronizeProxyMenuItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                ToolStripItem source = item.Tag as ToolStripItem;
                if (source != null)
                {
                    item.Enabled = source.Enabled && source.Available;
                    ToolStripButton sourceButton = source as ToolStripButton;
                    ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                    if (sourceButton != null && menuItem != null && sourceButton.CheckOnClick)
                        menuItem.Checked = sourceButton.Checked;
                }

                ToolStripMenuItem childMenu = item as ToolStripMenuItem;
                if (childMenu != null && childMenu.DropDownItems.Count > 0)
                    SynchronizeProxyMenuItems(childMenu.DropDownItems);
            }
        }

        private void StopContinuousRunMenuItem_Click(object sender, EventArgs e)
        {
            // 与原“连续运行/停止”按钮保持同一权限门槛；菜单只是更明确的
            // 停止入口，不能成为绕过既有管理员权限的旁路。
            if (!Permission.CheckPermission(PermissionLevel.Admin))
                return;

            if (Frm_Job.Instance.tbc_jobs.SelectedTab == null)
                return;

            Job job = Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text);
            if (job != null && job.isRunLoop)
                job.LoopRun(false);
        }

        private static bool IsSelectedContinuousRunActive()
        {
            try
            {
                if (Frm_Job.Instance.tbc_jobs.SelectedTab == null)
                    return false;

                Job job = Job.FindJobByName(Frm_Job.Instance.tbc_jobs.SelectedTab.Text);
                return job != null && job.isRunLoop;
            }
            catch
            {
                return false;
            }
        }

        private void ConfigureVisionCommandBar()
        {
            bool english = Project.Instance.configuration.language == Language.English;
            toolStrip2.Dock = DockStyle.Top;
            toolStrip2.AutoSize = false;
            toolStrip2.Height = VisionCommandBarHeight;
            toolStrip2.Padding = new Padding(4, 4, 4, 4);
            toolStrip2.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip2.CanOverflow = true;
            toolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip2.BackColor = Color.FromArgb(248, 251, 255);
            toolStrip2.ForeColor = ModernUiTheme.PrimaryText;
            toolStrip2.Renderer = ModernUiTheme.ToolStripRenderer;
            toolStrip2.ShowItemToolTips = true;

            // 只直接呈现当前流程的四个高频动作。方案、流程、布局、设备等
            // 已经在六类主菜单中有稳定入口，不再于此重复为多个平级分组。
            ConfigureVisionPrimaryButton(toolStripButton11, english ? "Run once" : "单次运行", 82);
            ConfigureVisionPrimaryButton(toolStripButton12, english ? "Continuous" : "连续运行", 86);
            ConfigureVisionPrimaryButton(toolStripButton13, english ? "Save" : "保存项目", 82);
            ConfigureVisionPrimaryButton(toolStripButton23, english ? "Open image" : "读取图像", 82);
            ConfigureModernVisionCommandIcons();

            ToolStripDropDownButton batchGroup = CreateVisionGroup(
                "visionBatchGroup",
                english ? "Run" : "批量运行",
                CreateVisionAction(english ? "Run all once" : "全部单次运行", toolStripButton35),
                CreateVisionAction(english ? "Start/stop all continuous runs" : "全部连续运行/停止", toolStripButton16));
            ConfigureModernBatchGroupIcon(batchGroup, toolStrip2);

            toolStrip2.Items.Clear();
            toolStrip2.Items.AddRange(new ToolStripItem[]
            {
                toolStripButton11,
                toolStripButton12,
                toolStripButton13,
                new ToolStripSeparator(),
                toolStripButton23,
                new ToolStripSeparator(),
                batchGroup
            });
        }

        private static void ConfigureVisionPrimaryButton(ToolStripButton button, string text, int width)
        {
            button.AutoSize = false;
            button.AutoToolTip = false;
            button.Size = new Size(width, 30);
            button.Margin = new Padding(2, 1, 2, 1);
            button.Padding = new Padding(5, 0, 5, 0);
            button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.ImageScaling = ToolStripItemImageScaling.SizeToFit;
            button.ForeColor = ModernUiTheme.PrimaryText;
            button.Font = ModernUiTheme.UiFont;
            button.Text = text;
            button.ToolTipText = text;
            button.AccessibleName = text;
            button.Overflow = ToolStripItemOverflow.Never;
        }

        private static ToolStripMenuItem CreateVisionAction(string text, ToolStripItem source)
        {
            ToolStripMenuItem action = new ToolStripMenuItem(text);
            action.Name = source == null ? string.Empty : source.Name + "MenuAction";
            action.Tag = source;
            action.Image = source == null ? null : source.Image;
            action.ImageScaling = ToolStripItemImageScaling.None;
            action.ToolTipText = source == null ? string.Empty : source.ToolTipText;
            action.Click += delegate
            {
                if (source != null && source.Enabled)
                {
                    source.PerformClick();
                    ToolStripButton sourceButton = source as ToolStripButton;
                    if (sourceButton != null && sourceButton.CheckOnClick)
                        action.Checked = sourceButton.Checked;
                }
            };
            return action;
        }

        private static ToolStripDropDownButton CreateVisionGroup(string name, string text, params ToolStripItem[] actions)
        {
            ToolStripDropDownButton group = new ToolStripDropDownButton(text);
            group.Name = name;
            group.AutoSize = true;
            group.AutoToolTip = false;
            group.DisplayStyle = ToolStripItemDisplayStyle.Text;
            group.Font = ModernUiTheme.UiFont;
            group.ForeColor = ModernUiTheme.PrimaryText;
            group.Margin = new Padding(2, 1, 2, 1);
            group.Padding = new Padding(5, 0, 5, 0);
            group.ToolTipText = text;
            group.AccessibleName = text;
            group.DropDownItems.AddRange(actions);
            group.DropDownOpening += delegate
            {
                foreach (ToolStripItem item in group.DropDownItems)
                {
                    ToolStripMenuItem action = item as ToolStripMenuItem;
                    ToolStripItem source = action == null ? null : action.Tag as ToolStripItem;
                    if (source == null)
                        continue;

                    action.Enabled = source.Enabled;
                    ToolStripButton sourceButton = source as ToolStripButton;
                    action.Checked = sourceButton != null && sourceButton.CheckOnClick && sourceButton.Checked;
                }
            };
            return group;
        }

        private void ConfigureStatusBar()
        {
            statusSpring = new ToolStripStatusLabel();
            statusSpring.Spring = true;
            statusSpring.Text = string.Empty;

            statusStrip1.Items.Clear();
            statusStrip1.Items.AddRange(new ToolStripItem[]
            {
                lbl_curEngine,
                statusSpring,
                tss_permissionInfo,
                toolStripStatusLabel1,
                tss_curTime
            });

            statusStrip1.Dock = DockStyle.Bottom;
            statusStrip1.AutoSize = false;
            statusStrip1.Height = 28;
            statusStrip1.SizingGrip = false;
            statusStrip1.RightToLeft = RightToLeft.No;
            statusStrip1.Padding = new Padding(8, 0, 8, 0);
            statusStrip1.BackColor = Color.FromArgb(242, 247, 253);
            statusStrip1.ForeColor = ModernUiTheme.SecondaryText;
            statusStrip1.Renderer = ModernUiTheme.ToolStripRenderer;

            ToolStripItem[] statusItems = { lbl_curEngine, tss_permissionInfo, toolStripStatusLabel1, tss_curTime };
            foreach (ToolStripItem item in statusItems)
            {
                item.BackColor = Color.Transparent;
                item.ForeColor = ModernUiTheme.SecondaryText;
                item.Font = ModernUiTheme.UiFont;
                item.Margin = new Padding(4, 0, 4, 0);
            }

            lbl_curEngine.ForeColor = ModernUiTheme.Accent;
            lbl_curEngine.Font = ModernUiTheme.UiFontBold;
            tss_curTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void ConfigureContentArea()
        {
            panel2.Dock = DockStyle.Fill;
            panel2.BackColor = ModernUiTheme.Page;
            panel4.Dock = DockStyle.Fill;
            panel4.BackColor = ModernUiTheme.Page;
            dockPanel.Dock = DockStyle.Fill;
            dockPanel.BackColor = ModernUiTheme.Page;

            // WinForms 从较高的 child index 开始分配 Dock 空间；Fill 必须位于
            // 较低索引，否则会占满客户区并被标题栏覆盖。
            Controls.SetChildIndex(dockPanel, 0);
            Controls.SetChildIndex(panel4, 1);
            Controls.SetChildIndex(panel2, 2);
            Controls.SetChildIndex(statusStrip1, 3);
            Controls.SetChildIndex(toolStrip2, 4);
            Controls.SetChildIndex(panel1, 5);
        }

        private void ConfigureDockPanelSkin()
        {
            DockPanelSkin skin = dockPanel.Skin;
            skin.AutoHideStripSkin.DockStripGradient.StartColor = ModernUiTheme.Page;
            skin.AutoHideStripSkin.DockStripGradient.EndColor = ModernUiTheme.Page;
            skin.AutoHideStripSkin.TabGradient.StartColor = ModernUiTheme.Surface;
            skin.AutoHideStripSkin.TabGradient.EndColor = ModernUiTheme.Selection;
            skin.AutoHideStripSkin.TabGradient.TextColor = ModernUiTheme.PrimaryText;
            skin.AutoHideStripSkin.TextFont = ModernUiTheme.UiFont;

            skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.StartColor = ModernUiTheme.Page;
            skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.EndColor = ModernUiTheme.Page;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.StartColor = ModernUiTheme.Surface;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.EndColor = ModernUiTheme.Surface;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.TextColor = ModernUiTheme.Accent;
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.StartColor = Color.FromArgb(238, 245, 252);
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.EndColor = Color.FromArgb(238, 245, 252);
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.TextColor = ModernUiTheme.SecondaryText;
            skin.DockPaneStripSkin.TextFont = ModernUiTheme.UiFont;

            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.StartColor = ModernUiTheme.Accent;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.EndColor = ModernUiTheme.AccentHover;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.TextColor = Color.White;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.StartColor = Color.FromArgb(226, 237, 248);
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.EndColor = Color.FromArgb(226, 237, 248);
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.TextColor = ModernUiTheme.PrimaryText;
            dockPanel.Skin = skin;

            dockPanel.DockLeftPortion = 0.24D;
            dockPanel.DockRightPortion = 0.50D;
            dockPanel.DockBottomPortion = 0.22D;
            dockPanel.DefaultFloatWindowSize = new Size(320, 520);
        }

        internal void ShowToolboxInVisionSidebar()
        {
            EnsureVisionEditorColumns();
            Frm_ToolBox.Instance.Activate();
        }

        internal void EnsureVisionEditorColumns()
        {
            Frm_Job job = Frm_Job.Instance;
            Frm_ToolBox toolbox = Frm_ToolBox.Instance;
            // 已经并排时保留用户拖动后的列宽，反复打开工具箱不重复创建 Pane。
            if (job.DockState == DockState.DockRight && toolbox.DockState == DockState.DockRight &&
                job.Pane != null && toolbox.Pane != null && job.Pane != toolbox.Pane &&
                toolbox.Pane.NestedDockingStatus.PreviousPane == job.Pane &&
                toolbox.Pane.NestedDockingStatus.Alignment == DockAlignment.Right)
                return;

            dockPanel.SuspendLayout();
            try
            {
                job.Show(dockPanel, DockState.DockRight);
                toolbox.Show(job.Pane, DockAlignment.Right, 0.38D);
                dockPanel.DockRightPortion = 0.50D;
                dockPanel.UpdateDockWindowZOrder(DockStyle.Right, true);
            }
            finally { dockPanel.ResumeLayout(true); }
        }

        internal void ShowOutputInVisionBottomPanel()
        {
            Frm_Output output = Frm_Output.Instance;
            if (output.DockState != DockState.Hidden && output.DockState != DockState.Unknown)
            {
                output.Activate();
                return;
            }

            if (IsFactoryDockLayout(ResolveDockLayoutPath(Project.Instance.configuration.layoutFilePath)) &&
                Frm_Monitor.Instance.Pane != null)
            {
                output.Show(Frm_Monitor.Instance.Pane, null);
            }
            else
            {
                output.Show(dockPanel, DockState.DockBottom);
            }

            output.Activate();
        }

        internal void ApplyWorkspaceMode(FormMode formMode)
        {
            if (formMode == FormMode.None)
                formMode = FormMode.VisionForm;

            SuspendLayout();
            try
            {
                EnsureEmbeddedWorkspace(formMode);
                Machine.curFormMode = formMode;

                bool showHome = formMode == FormMode.MainForm;
                bool showVision = formMode == FormMode.VisionForm;
                bool showMotion = formMode == FormMode.MotionForm;

                if (showHome)
                    Frm_UserForm.Instance.RefreshDashboard();
                if (showMotion)
                    Frm_MotionControl.Instance.RefreshSmartPositionTables();

                // 标准菜单作为所有工作区的稳定导航入口，避免旧配置把唯一入口永久隐藏。
                menuStrip1.Visible = true;
                toolStrip2.Visible = showVision;
                panel2.Visible = showHome;
                dockPanel.Visible = showVision;
                panel4.Visible = showMotion;

                panel2.Dock = DockStyle.Fill;
                dockPanel.Dock = DockStyle.Fill;
                panel4.Dock = DockStyle.Fill;

                toolStripButton9.Checked = showHome;
                toolStripButton5.Checked = showVision;
                toolStripButton1.Checked = showMotion;

                RefreshModernWorkspaceIcons();

                UpdateMotionRefreshActivity();

            }
            finally
            {
                ResumeLayout(true);
            }
        }

        internal void EnsureEmbeddedWorkspace(FormMode formMode)
        {
            if (formMode == FormMode.MainForm &&
                (!homeWorkspaceLoaded || Frm_UserForm.Instance.Parent != panel2))
            {
                panel2.Controls.Clear();
                Frm_UserForm.Instance.TopLevel = false;
                Frm_UserForm.Instance.Parent = panel2;
                Frm_UserForm.Instance.Dock = DockStyle.Fill;
                Frm_UserForm.Instance.Show();
                homeWorkspaceLoaded = true;
            }

            if (formMode == FormMode.MotionForm &&
                (!motionWorkspaceLoaded || Frm_MotionControl.Instance.Parent != panel4))
            {
                panel4.Controls.Clear();
                Frm_MotionControl.Instance.TopLevel = false;
                Frm_MotionControl.Instance.Parent = panel4;
                Frm_MotionControl.Instance.Dock = DockStyle.Fill;
                Frm_MotionControl.Instance.Show();
                motionWorkspaceLoaded = true;
            }
        }

        internal void ProcessUiRefreshTick()
        {
            if (Machine.willExit || refreshTickBusy)
                return;

            refreshTickBusy = true;
            try
            {
                int nowTick = Environment.TickCount;
                DateTime now = DateTime.Now;

                if (Machine.machineRunStatu == MachineRunStatu.Running)
                {
                    Machine.runTime += now - Machine.lastTime;
                    Machine.lastTime = now;
                }
                else
                {
                    // 避免下一次启动把暂停/停机时长累计到运行时长。
                    Machine.lastTime = now;
                }

                if (unchecked(nowTick - nextClockRefreshTick) >= 0)
                {
                    nextClockRefreshTick = unchecked(nowTick + 1000);
                    tss_curTime.Text = now.ToString("yyyy/MM/dd HH:mm:ss");
                    toolStripStatusLabel1.Text = string.Format("运行时长：{0:0.00} H", Machine.runTime.TotalHours);
                    Machine.UpdateAll();
                    if (homeWorkspaceLoaded && Machine.curFormMode == FormMode.MainForm)
                        Frm_UserForm.Instance.RefreshDashboard();
                }

                bool motionVisible = motionWorkspaceLoaded && Frm_MotionControl.Instance.IsRefreshSurfaceVisible;
                if (motionVisible &&
                    Machine.machineRunStatu != MachineRunStatu.Running &&
                    unchecked(nowTick - nextIoRefreshTick) >= 0)
                {
                    nextIoRefreshTick = unchecked(nowTick + 300);
                    Machine.UpdateIO();
                }
            }
            finally
            {
                refreshTickBusy = false;
            }
        }

        internal void UpdateMaximizedBoundsForCurrentScreen()
        {
            Screen screen = Screen.FromControl(this);
            if (screen != null)
                MaximizedBounds = screen.WorkingArea;
        }

        internal void UpdateMotionRefreshActivity()
        {
            if (!motionWorkspaceLoaded)
                return;

            Frm_MotionControl motionControl = Frm_MotionControl.Instance;
            bool active = motionControl.IsRefreshSurfaceVisible &&
                          WindowState != FormWindowState.Minimized &&
                          (Machine.curFormMode == FormMode.MotionForm || motionControl.TopLevel);
            motionControl.SetRefreshActive(active);
        }

        private void Frm_Main_ModernLocationChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                UpdateMaximizedBoundsForCurrentScreen();
        }

        private void Frm_Main_ModernVisibleChanged(object sender, EventArgs e)
        {
            UpdateMotionRefreshActivity();
        }

        internal static bool IsFactoryDockLayout(string layoutPath)
        {
            string name = Path.GetFileNameWithoutExtension(layoutPath ?? string.Empty);
            return string.Equals(name, "经典布局1", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(name, "经典布局2", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(name, "左中右布局", StringComparison.OrdinalIgnoreCase);
        }

        internal static string ResolveDockLayoutPath(string layoutPath)
        {
            string value = string.IsNullOrWhiteSpace(layoutPath)
                ? "Config\\Resources\\Layout\\经典布局1.config"
                : layoutPath;
            return Path.IsPathRooted(value) ? value : Path.Combine(Application.StartupPath, value);
        }

        /// <summary>
        /// 早期随仓库附带的“手机组装”演示快照固定指向自定义布局1，布局中会同时
        /// 展开流程、工具箱和日志。只迁移带有该演示项目路径标记的已知快照；其他
        /// 同名或任意用户布局均不改写、不删除。
        /// </summary>
        internal void MigrateLegacySampleLayoutIfNeeded()
        {
            Configuration configuration = Project.Instance.configuration;
            string configuredLayout = configuration.layoutFilePath ?? string.Empty;
            if (!string.Equals(
                    Path.GetFileNameWithoutExtension(configuredLayout),
                    "自定义布局1",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            bool legacySample = false;
            foreach (string recentProject in configuration.L_recentlyOpendFile)
            {
                if (!string.IsNullOrEmpty(recentProject) &&
                    recentProject.IndexOf("手机组装", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    legacySample = true;
                    break;
                }
            }

            const string focusLayout = "Config\\Resources\\Layout\\经典布局1.config";
            if (legacySample && File.Exists(ResolveDockLayoutPath(focusLayout)))
                configuration.layoutFilePath = focusLayout;
        }

        /// <summary>
        /// 保存用户布局。系统标准/兼容模板只读；结构变化需要持久化时，
        /// 自动转存到 dockPanel.config，避免覆盖随程序发布的模板。
        /// </summary>
        internal void SaveDockLayout(bool createUserCopyForFactory)
        {
            string configuredPath = Project.Instance.configuration.layoutFilePath;
            string resolvedConfiguredPath = ResolveDockLayoutPath(configuredPath);
            if (IsFactoryDockLayout(resolvedConfiguredPath))
            {
                if (!createUserCopyForFactory)
                    return;

                configuredPath = "dockPanel.config";
                Project.Instance.configuration.layoutFilePath = configuredPath;
                切换到经典布局1ToolStripMenuItem.Checked = false;
                切换到经典布局2ToolStripMenuItem.Checked = false;
            }

            string fullPath = ResolveDockLayoutPath(configuredPath);
            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            dockPanel.SaveAsXml(fullPath);
        }
    }
}
