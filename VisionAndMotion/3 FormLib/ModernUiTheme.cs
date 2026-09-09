using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace VMPro
{
    /// <summary>
    /// WLP VM 的统一蓝白界面令牌与 WinForms 控件主题（浅色方案）。
    /// 主题只调整视觉属性，不修改业务事件、权限或设备操作语义。
    /// </summary>
    internal static class ModernUiTheme
    {
        internal static readonly Color Accent = Color.FromArgb(76, 148, 210);
        internal static readonly Color AccentHover = Color.FromArgb(57, 128, 190);
        internal static readonly Color AccentPressed = Color.FromArgb(42, 103, 160);
        internal static readonly Color HeaderBlue = Color.FromArgb(52, 126, 184);
        internal static readonly Color AccentSoft = Color.FromArgb(232, 243, 252);
        internal static readonly Color Selection = Color.FromArgb(226, 240, 252);
        internal static readonly Color Page = Color.FromArgb(248, 247, 243);
        internal static readonly Color Surface = Color.FromArgb(255, 254, 250);
        internal static readonly Color SurfaceRaised = Color.FromArgb(255, 255, 253);
        internal static readonly Color Border = Color.FromArgb(217, 227, 234);
        internal static readonly Color PrimaryText = Color.FromArgb(39, 56, 72);
        internal static readonly Color SecondaryText = Color.FromArgb(99, 116, 130);
        internal static readonly Color Danger = Color.FromArgb(190, 74, 74);
        internal static readonly Color DangerSoft = Color.FromArgb(252, 239, 237);

        internal static readonly Font UiFont = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
        internal static readonly Font UiFontBold = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
        internal static readonly Font TitleFont = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 134);
        internal static readonly Font SectionTitleFont = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 134);
        internal static readonly Font MetricFont = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 134);
        internal static readonly Font WelcomeTitleFont = new Font("Microsoft YaHei UI", 22F, FontStyle.Bold, GraphicsUnit.Point, 134);
        internal static readonly ToolStripRenderer ToolStripRenderer = new ModernToolStripRenderer();

        private static readonly ConditionalWeakTable<Control, ThemeMarker> ThemedControls = new ConditionalWeakTable<Control, ThemeMarker>();
        private static readonly ConditionalWeakTable<Control, RoundedMarker> RoundedControls = new ConditionalWeakTable<Control, RoundedMarker>();
        private static readonly ConditionalWeakTable<Control, CardMarker> CardControls = new ConditionalWeakTable<Control, CardMarker>();
        private static readonly ConditionalWeakTable<Button, RoundedButtonMarker> RoundedButtons = new ConditionalWeakTable<Button, RoundedButtonMarker>();
        private static readonly Dictionary<int, Font> FontCache = new Dictionary<int, Font>();
        private static bool installed;
        private static int nextOpenFormScan;

        private sealed class ThemeMarker
        {
        }

        private sealed class RoundedMarker
        {
            internal int Radius;
        }

        private sealed class RoundedButtonMarker
        {
            internal Color NormalFill;
            internal Color HoverFill;
            internal Color PressedFill;
            internal Color BorderColor;
            internal bool Hovered;
            internal bool Pressed;
            internal Image GeneratedBackground;
            internal Image GeneratedIcon;
        }

        private sealed class CardMarker
        {
            internal Image GeneratedBackground;
            internal Size BackgroundSize;
        }

        internal static void Install()
        {
            if (installed)
                return;

            installed = true;
            nextOpenFormScan = Environment.TickCount;
            Application.Idle += Application_Idle;
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            int now = Environment.TickCount;
            if (unchecked(now - nextOpenFormScan) < 0)
                return;

            nextOpenFormScan = unchecked(now + 1000);
            try
            {
                List<Form> forms = new List<Form>();
                foreach (Form form in Application.OpenForms)
                    forms.Add(form);

                foreach (Form form in forms)
                    Apply(form);
            }
            catch
            {
                // 主题应用失败不能影响视觉流程或设备控制。
            }
        }

        internal static void Apply(Control root)
        {
            if (root == null || root.IsDisposed)
                return;

            ApplyControlTree(root);
        }

        private static void ApplyControlTree(Control control)
        {
            if (control == null || control.IsDisposed)
                return;
            if (control.GetType().Namespace != null && control.GetType().Namespace.StartsWith("HalconDotNet", StringComparison.Ordinal))
                return;

            // 欢迎页使用独立的暖白品牌画面，不套用通用窗体递归规则。
            if (control is Frm_Welcome)
            {
                ThemeMarker welcomeMarker;
                if (!ThemedControls.TryGetValue(control, out welcomeMarker))
                    ThemedControls.Add(control, new ThemeMarker());
                return;
            }

            ThemeMarker marker;
            if (ThemedControls.TryGetValue(control, out marker))
                return;

            ThemedControls.Add(control, new ThemeMarker());
            try
            {
                ApplyControl(control);
            }
            catch
            {
                // 第三方控件可能拒绝部分视觉属性，逐控件隔离失败。
            }

            try
            {
                control.ControlAdded += Control_ControlAdded;
            }
            catch
            {
            }

            foreach (Control child in control.Controls)
                ApplyControlTree(child);
        }

        private static void Control_ControlAdded(object sender, ControlEventArgs e)
        {
            try
            {
                ApplyControlTree(e.Control);
            }
            catch
            {
            }
        }

        private static void ApplyControl(Control control)
        {
            bool standardWindowsControl = control.GetType().Assembly == typeof(Control).Assembly;
            if (standardWindowsControl && !(control is DataGridView) && !(control is PropertyGrid))
                control.Font = ResolveUiFont(control.Font);

            if (ThemeCustomInput(control))
                return;

            Form form = control as Form;
            if (form != null)
            {
                form.BackColor = Page;
                form.ForeColor = PrimaryText;
                if (form is Frm_FormBase)
                    ApplyRoundedRegion(form, 10);
                return;
            }

            ToolStrip strip = control as ToolStrip;
            if (strip != null)
            {
                ThemeToolStrip(strip);
                return;
            }

            DataGridView grid = control as DataGridView;
            if (grid != null)
            {
                ThemeDataGridView(grid);
                return;
            }

            Button button = control as Button;
            if (button != null)
            {
                ThemeButton(button);
                return;
            }

            TextBoxBase textBox = control as TextBoxBase;
            if (textBox != null)
            {
                ModernToolboxSearchBox toolboxSearch = textBox.Parent as ModernToolboxSearchBox;
                if (toolboxSearch != null)
                {
                    toolboxSearch.ApplyInputPalette();
                    return;
                }
                textBox.BackColor = IsInsideCustomInput(textBox) ? SurfaceRaised : Surface;
                textBox.ForeColor = PrimaryText;
                textBox.BorderStyle = IsInsideCustomInput(textBox) ? BorderStyle.None : BorderStyle.FixedSingle;
                return;
            }

            ComboBox comboBox = control as ComboBox;
            if (comboBox != null)
            {
                comboBox.BackColor = IsInsideCustomInput(comboBox) ? SurfaceRaised : Surface;
                comboBox.ForeColor = PrimaryText;
                comboBox.FlatStyle = FlatStyle.Flat;
                return;
            }

            CheckBox checkBox = control as CheckBox;
            if (checkBox != null)
            {
                checkBox.FlatStyle = FlatStyle.Flat;
                checkBox.ForeColor = PrimaryText;
                checkBox.Cursor = Cursors.Hand;
                return;
            }

            RadioButton radioButton = control as RadioButton;
            if (radioButton != null)
            {
                radioButton.FlatStyle = FlatStyle.Flat;
                radioButton.ForeColor = PrimaryText;
                radioButton.Cursor = Cursors.Hand;
                return;
            }

            TreeView treeView = control as TreeView;
            if (treeView != null)
            {
                treeView.BackColor = Surface;
                treeView.ForeColor = PrimaryText;
                treeView.BorderStyle = BorderStyle.None;
                treeView.FullRowSelect = true;
                treeView.HideSelection = false;
                treeView.ShowLines = false;
                treeView.ItemHeight = Math.Max(treeView.ItemHeight, 28);
                return;
            }

            ListView listView = control as ListView;
            if (listView != null)
            {
                listView.BackColor = Surface;
                listView.ForeColor = PrimaryText;
                listView.BorderStyle = BorderStyle.FixedSingle;
                listView.FullRowSelect = true;
                return;
            }

            TabControl tabControl = control as TabControl;
            if (tabControl != null)
            {
                tabControl.BackColor = Surface;
                tabControl.ForeColor = PrimaryText;
                return;
            }

            GroupBox groupBox = control as GroupBox;
            if (groupBox != null)
            {
                groupBox.ForeColor = PrimaryText;
                return;
            }

            LinkLabel linkLabel = control as LinkLabel;
            if (linkLabel != null)
            {
                linkLabel.LinkColor = Accent;
                linkLabel.ActiveLinkColor = AccentPressed;
                linkLabel.VisitedLinkColor = Accent;
                return;
            }

            Panel panel = control as Panel;
            if (panel != null && IsLegacyAccent(panel.BackColor))
                panel.BackColor = HeaderBlue;

            System.Windows.Forms.Label label = control as System.Windows.Forms.Label;
            if (label != null && !IsLegacyAccent(label.BackColor) && label.ForeColor == Color.Black)
                label.ForeColor = PrimaryText;
        }

        private static Font ResolveUiFont(Font source)
        {
            if (source == null)
                return UiFont;
            if (string.Equals(source.FontFamily.Name, "Microsoft YaHei UI", StringComparison.OrdinalIgnoreCase))
                return source;

            int sizeKey = (int)Math.Round(source.SizeInPoints * 100F);
            int key = (sizeKey << 8) ^ ((int)source.Style << 3);
            lock (FontCache)
            {
                Font font;
                if (!FontCache.TryGetValue(key, out font))
                {
                    font = new Font("Microsoft YaHei UI", source.SizeInPoints, source.Style, GraphicsUnit.Point, 134);
                    FontCache.Add(key, font);
                }
                return font;
            }
        }

        private static void ThemeToolStrip(ToolStrip strip)
        {
            strip.Renderer = ToolStripRenderer;
            strip.Font = UiFont;
            strip.BackColor = Surface;
            strip.ForeColor = PrimaryText;
            strip.ItemAdded -= ToolStrip_ItemAdded;
            strip.ItemAdded += ToolStrip_ItemAdded;

            StatusStrip statusStrip = strip as StatusStrip;
            if (statusStrip != null)
                statusStrip.BackColor = Color.FromArgb(242, 247, 253);

            foreach (ToolStripItem item in strip.Items)
                ThemeToolStripItem(item);
        }

        private static void ToolStrip_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            ToolStrip strip = sender as ToolStrip;
            if (strip == null || strip.IsDisposed || !strip.IsHandleCreated)
                return;

            if (strip.InvokeRequired)
            {
                try
                {
                    strip.BeginInvoke(new Action<ToolStripItem>(ThemeToolStripItemSafely), e.Item);
                }
                catch
                {
                }
                return;
            }

            ThemeToolStripItemSafely(e.Item);
        }

        private static void ThemeToolStripItemSafely(ToolStripItem item)
        {
            try
            {
                ThemeToolStripItem(item);
            }
            catch
            {
            }
        }

        private static void ThemeToolStripItem(ToolStripItem item)
        {
            item.Font = UiFont;
            if (item.ForeColor == Color.White || item.ForeColor == Color.Black || item.ForeColor == SystemColors.ControlText)
                item.ForeColor = PrimaryText;

            ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;
            if (dropDownItem == null)
                return;

            dropDownItem.DropDown.BackColor = Surface;
            dropDownItem.DropDown.Renderer = ToolStripRenderer;
            dropDownItem.DropDown.ItemAdded -= ToolStrip_ItemAdded;
            dropDownItem.DropDown.ItemAdded += ToolStrip_ItemAdded;
            foreach (ToolStripItem child in dropDownItem.DropDownItems)
                ThemeToolStripItem(child);
        }

        internal static void RefreshToolStripItems(Control root)
        {
            if (root == null || root.IsDisposed)
                return;

            ToolStrip strip = root as ToolStrip;
            if (strip != null)
            {
                foreach (ToolStripItem item in strip.Items)
                    ThemeToolStripItemSafely(item);
            }

            foreach (Control child in root.Controls)
                RefreshToolStripItems(child);
        }

        private static void ThemeDataGridView(DataGridView grid)
        {
            grid.Font = UiFont;
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 245, 252);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryText;
            grid.ColumnHeadersDefaultCellStyle.Font = UiFontBold;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 245, 252);
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = PrimaryText;
            grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = PrimaryText;
            grid.DefaultCellStyle.SelectionBackColor = Selection;
            grid.DefaultCellStyle.SelectionForeColor = PrimaryText;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 251, 254);
        }

        private static void ThemeButton(Button button)
        {
            bool chromeButton = IsWindowChromeButton(button);
            bool textAction = !string.IsNullOrWhiteSpace(button.Text) && button.Width >= 40 && button.Height >= 24;

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = Selection;
            button.FlatAppearance.MouseDownBackColor = AccentSoft;

            if (IsInsideCustomInput(button))
            {
                button.FlatAppearance.BorderSize = 0;
                button.UseVisualStyleBackColor = false;
                button.BackColor = Color.Transparent;
                button.ForeColor = SecondaryText;
                return;
            }

            if (chromeButton)
            {
                button.FlatAppearance.BorderSize = 0;
                if (IsLegacyAccent(button.BackColor))
                {
                    button.BackColor = HeaderBlue;
                    button.ForeColor = Color.White;
                    button.FlatAppearance.MouseOverBackColor = AccentHover;
                    button.FlatAppearance.MouseDownBackColor = AccentPressed;
                }
                return;
            }

            if (textAction)
            {
                if (button.BackgroundImage != null && button.BackgroundImageLayout == ImageLayout.Stretch)
                    button.BackgroundImage = null;
                button.UseVisualStyleBackColor = false;
                button.Padding = new Padding(Math.Max(button.Padding.Left, 8), button.Padding.Top,
                    Math.Max(button.Padding.Right, 8), button.Padding.Bottom);
            }

            if (IsDangerAction(button))
            {
                button.BackColor = DangerSoft;
                button.ForeColor = Danger;
                button.FlatAppearance.BorderColor = Color.FromArgb(235, 190, 186);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(249, 226, 223);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(244, 211, 207);
            }
            else if (IsNeutralAction(button))
            {
                button.BackColor = Surface;
                button.ForeColor = PrimaryText;
                button.FlatAppearance.BorderColor = Border;
                button.FlatAppearance.MouseOverBackColor = Selection;
                button.FlatAppearance.MouseDownBackColor = AccentSoft;
            }
            else if (IsLegacyAccent(button.BackColor) || IsPrimaryAction(button))
            {
                button.BackColor = Accent;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = Accent;
                button.FlatAppearance.MouseOverBackColor = AccentHover;
                button.FlatAppearance.MouseDownBackColor = AccentPressed;
            }
            else if (button.BackColor == SystemColors.Control || button.BackColor == Color.White || button.BackColor == Color.Transparent)
            {
                button.BackColor = Surface;
                button.ForeColor = PrimaryText;
            }

            if (textAction && button.BackgroundImage == null)
                StyleRoundedButton(button);
        }

        private static void StyleRoundedButton(Button button)
        {
            if (button == null || button.IsDisposed)
                return;

            RoundedButtonMarker marker;
            if (!RoundedButtons.TryGetValue(button, out marker))
            {
                marker = new RoundedButtonMarker();
                RoundedButtons.Add(button, marker);
                button.MouseEnter += RoundedButton_MouseEnter;
                button.MouseLeave += RoundedButton_MouseLeave;
                button.MouseDown += RoundedButton_MouseDown;
                button.MouseUp += RoundedButton_MouseUp;
                button.MouseCaptureChanged += RoundedButton_CaptureChanged;
                button.EnabledChanged += RoundedButton_StateChanged;
                button.Resize += RoundedButton_StateChanged;
                button.Disposed += RoundedButton_Disposed;
            }

            marker.NormalFill = button.BackColor;
            marker.HoverFill = button.FlatAppearance.MouseOverBackColor;
            marker.PressedFill = button.FlatAppearance.MouseDownBackColor;
            marker.BorderColor = button.FlatAppearance.BorderColor;
            NormalizeOversizedTextButtonIcon(button, marker);

            Region oldRegion = button.Region;
            button.Region = null;
            if (oldRegion != null)
                oldRegion.Dispose();

            Color cornerColor = ResolveOpaqueParentColor(button);
            button.BackColor = cornerColor;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = cornerColor;
            button.FlatAppearance.MouseDownBackColor = cornerColor;
            button.BackgroundImageLayout = ImageLayout.None;
            UpdateRoundedButtonBackground(button, marker);
        }

        /// <summary>
        /// 更新已经主题化的圆角按钮状态。圆角按钮的可见填充保存在抗锯齿
        /// BackgroundImage 中，业务代码只改 BackColor 不会刷新该缓存。
        /// </summary>
        internal static void RefreshRoundedButtonPalette(
            Button button,
            Color normalFill,
            Color hoverFill,
            Color pressedFill,
            Color borderColor)
        {
            if (button == null || button.IsDisposed)
                return;

            button.BackColor = normalFill;
            button.FlatAppearance.MouseOverBackColor = hoverFill;
            button.FlatAppearance.MouseDownBackColor = pressedFill;
            button.FlatAppearance.BorderColor = borderColor;
            StyleRoundedButton(button);
        }

        private static void RoundedButton_MouseEnter(object sender, EventArgs e)
        {
            Button button = sender as Button;
            RoundedButtonMarker marker;
            if (button != null && RoundedButtons.TryGetValue(button, out marker))
            {
                marker.Hovered = true;
                UpdateRoundedButtonBackground(button, marker);
            }
        }

        private static void RoundedButton_MouseLeave(object sender, EventArgs e)
        {
            Button button = sender as Button;
            RoundedButtonMarker marker;
            if (button != null && RoundedButtons.TryGetValue(button, out marker))
            {
                marker.Hovered = false;
                marker.Pressed = false;
                UpdateRoundedButtonBackground(button, marker);
            }
        }

        private static void RoundedButton_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            RoundedButtonMarker marker;
            if (e.Button == MouseButtons.Left && button != null && RoundedButtons.TryGetValue(button, out marker))
            {
                marker.Pressed = true;
                UpdateRoundedButtonBackground(button, marker);
            }
        }

        private static void RoundedButton_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            RoundedButtonMarker marker;
            if (button != null && RoundedButtons.TryGetValue(button, out marker))
            {
                marker.Pressed = false;
                UpdateRoundedButtonBackground(button, marker);
            }
        }

        private static void RoundedButton_CaptureChanged(object sender, EventArgs e)
        {
            Button button = sender as Button;
            RoundedButtonMarker marker;
            if (button != null && RoundedButtons.TryGetValue(button, out marker) && marker.Pressed)
            {
                marker.Pressed = false;
                UpdateRoundedButtonBackground(button, marker);
            }
        }

        private static void RoundedButton_StateChanged(object sender, EventArgs e)
        {
            Button button = sender as Button;
            RoundedButtonMarker marker;
            if (button != null && RoundedButtons.TryGetValue(button, out marker))
                UpdateRoundedButtonBackground(button, marker);
        }

        private static void RoundedButton_Disposed(object sender, EventArgs e)
        {
            Button button = sender as Button;
            RoundedButtonMarker marker;
            if (button == null || !RoundedButtons.TryGetValue(button, out marker))
                return;
            if (marker.GeneratedBackground != null)
            {
                if (ReferenceEquals(button.BackgroundImage, marker.GeneratedBackground))
                    button.BackgroundImage = null;
                marker.GeneratedBackground.Dispose();
                marker.GeneratedBackground = null;
            }
            if (marker.GeneratedIcon != null)
            {
                if (ReferenceEquals(button.Image, marker.GeneratedIcon))
                    button.Image = null;
                marker.GeneratedIcon.Dispose();
                marker.GeneratedIcon = null;
            }
            RoundedButtons.Remove(button);
        }

        private static void NormalizeOversizedTextButtonIcon(Button button, RoundedButtonMarker marker)
        {
            if (button.Image == null || marker.GeneratedIcon != null)
                return;

            int target = Math.Min(button.Width < 60 ? 12 : 16, Math.Max(8, button.Height - 10));
            if (button.Image.Width <= target * 2 && button.Image.Height <= target * 2)
                return;

            Image source = button.Image;
            float scale = Math.Min(target / (float)source.Width, target / (float)source.Height);
            int width = Math.Max(1, (int)Math.Round(source.Width * scale));
            int height = Math.Max(1, (int)Math.Round(source.Height * scale));
            Bitmap icon = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            using (Graphics graphics = Graphics.FromImage(icon))
            using (ImageAttributes attributes = new ImageAttributes())
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                Color color = button.ForeColor;
                ColorMatrix tint = new ColorMatrix(new[]
                {
                    new[] { 0F, 0F, 0F, 0F, 0F },
                    new[] { 0F, 0F, 0F, 0F, 0F },
                    new[] { 0F, 0F, 0F, 0F, 0F },
                    new[] { 0F, 0F, 0F, 1F, 0F },
                    new[] { color.R / 255F, color.G / 255F, color.B / 255F, 0F, 1F }
                });
                attributes.SetColorMatrix(tint, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                attributes.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(source, new Rectangle(0, 0, width, height),
                    0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
            }

            marker.GeneratedIcon = icon;
            button.Image = icon;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.ImageAlign = ContentAlignment.MiddleCenter;
            button.TextAlign = ContentAlignment.MiddleCenter;
            if (button.Width < 60)
                button.Padding = new Padding(3, button.Padding.Top, 3, button.Padding.Bottom);
        }

        private static void UpdateRoundedButtonBackground(Button button, RoundedButtonMarker marker)
        {
            if (button == null || button.IsDisposed || button.Width <= 1 || button.Height <= 1)
                return;

            Color fill = marker.Pressed ? marker.PressedFill : marker.Hovered ? marker.HoverFill : marker.NormalFill;
            if (fill == Color.Empty || fill == Color.Transparent)
                fill = Surface;
            if (!button.Enabled)
                fill = BlendColors(fill, ResolveOpaqueParentColor(button), 0.45F);

            Bitmap bitmap = new Bitmap(button.Width, button.Height, PixelFormat.Format32bppPArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.Clear(Color.Transparent);
                float radius = Math.Min(10F, Math.Max(6F, button.Height / 3F));
                using (GraphicsPath path = CreateRoundedPath(
                    new RectangleF(0.5F, 0.5F, Math.Max(0F, button.Width - 1.5F), Math.Max(0F, button.Height - 1.5F)), radius))
                using (SolidBrush brush = new SolidBrush(fill))
                using (Pen pen = new Pen(marker.BorderColor == Color.Empty ? Border : marker.BorderColor, 1F))
                {
                    pen.Alignment = PenAlignment.Inset;
                    graphics.FillPath(brush, path);
                    graphics.DrawPath(pen, path);
                }
            }

            Image oldImage = marker.GeneratedBackground;
            marker.GeneratedBackground = bitmap;
            button.BackgroundImage = bitmap;
            if (oldImage != null)
                oldImage.Dispose();
            button.Invalidate();
        }

        private static Color ResolveOpaqueParentColor(Control control)
        {
            Control current = control == null ? null : control.Parent;
            while (current != null)
            {
                if (current.BackColor != Color.Transparent && current.BackColor.A == 255)
                    return current.BackColor;
                current = current.Parent;
            }
            return Page;
        }

        private static Color BlendColors(Color first, Color second, float secondAmount)
        {
            float amount = Math.Max(0F, Math.Min(1F, secondAmount));
            return Color.FromArgb(
                (int)(first.R + ((second.R - first.R) * amount)),
                (int)(first.G + ((second.G - first.G) * amount)),
                (int)(first.B + ((second.B - first.B) * amount)));
        }

        private static bool ThemeCustomInput(Control control)
        {
            string typeName = control.GetType().FullName;
            if (typeName != "Controls.CTextBox" &&
                typeName != "Controls.CComboBox" &&
                typeName != "Controls.CNumeric" &&
                typeName != "Controls.CNumericUpDown")
                return false;

            // 新版输入控件自行绘制抗锯齿圆角。Region 是二值裁切，在 24~26px
            // 高的小控件及高 DPI 下必然出现阶梯边缘，因此这里明确移除旧 Region，
            // 同时避免主题 Paint 与控件自身边框重复绘制。
            Region oldRegion = control.Region;
            control.Region = null;
            if (oldRegion != null)
                oldRegion.Dispose();
            control.Paint -= SoftInput_Paint;
            Controls.ModernInputControl modernInput = control as Controls.ModernInputControl;
            if (modernInput != null)
                modernInput.ApplyModernPalette(SurfaceRaised, Border, Accent, PrimaryText);
            else
            {
                control.BackColor = SurfaceRaised;
                control.ForeColor = PrimaryText;
            }

            foreach (Control child in control.Controls)
            {
                TextBoxBase textBox = child as TextBoxBase;
                if (textBox != null)
                {
                    textBox.BackColor = SurfaceRaised;
                    textBox.ForeColor = PrimaryText;
                    textBox.BorderStyle = BorderStyle.None;
                    continue;
                }

                ComboBox comboBox = child as ComboBox;
                if (comboBox != null)
                {
                    comboBox.BackColor = SurfaceRaised;
                    comboBox.ForeColor = PrimaryText;
                    comboBox.FlatStyle = FlatStyle.Flat;
                    continue;
                }

                NumericUpDown numeric = child as NumericUpDown;
                if (numeric != null)
                {
                    numeric.BackColor = SurfaceRaised;
                    numeric.ForeColor = PrimaryText;
                    numeric.BorderStyle = BorderStyle.None;
                    continue;
                }

                System.Windows.Forms.Label underline = child as System.Windows.Forms.Label;
                if (underline != null &&
                    (string.Equals(underline.Name, "lbl_line", StringComparison.OrdinalIgnoreCase) || underline.Height <= 3))
                {
                    // 自定义输入旧版依靠会随高度拉伸的下划线。圆角外框启用后继续
                    // 保留它会在高 DPI 下变成一条灰色粗带，因此直接隐藏。
                    underline.Visible = false;
                }
            }
            return true;
        }

        private static bool IsInsideCustomInput(Control control)
        {
            Control parent = control == null ? null : control.Parent;
            if (parent == null)
                return false;

            string typeName = parent.GetType().FullName;
            return typeName == "Controls.CTextBox" ||
                   typeName == "Controls.CComboBox" ||
                   typeName == "Controls.CNumeric" ||
                   typeName == "Controls.CNumericUpDown" ||
                   typeName == "VMPro.ModernToolboxSearchBox";
        }

        internal static void StyleCard(Control control)
        {
            if (control == null || control.IsDisposed)
                return;

            Region oldRegion = control.Region;
            control.Region = null;
            if (oldRegion != null)
                oldRegion.Dispose();

            control.BackColor = ResolveOpaqueParentColor(control);
            control.BackgroundImageLayout = ImageLayout.None;
            control.Paint -= Card_Paint;
            CardMarker marker;
            if (!CardControls.TryGetValue(control, out marker))
            {
                marker = new CardMarker();
                CardControls.Add(control, marker);
                control.Resize += Card_StateChanged;
                control.ParentChanged += Card_StateChanged;
                control.Disposed += Card_Disposed;
            }
            UpdateCardBackground(control, marker);
        }

        private static void Card_StateChanged(object sender, EventArgs e)
        {
            Control control = sender as Control;
            CardMarker marker;
            if (control != null && CardControls.TryGetValue(control, out marker))
            {
                control.BackColor = ResolveOpaqueParentColor(control);
                UpdateCardBackground(control, marker);
            }
        }

        private static void Card_Disposed(object sender, EventArgs e)
        {
            Control control = sender as Control;
            CardMarker marker;
            if (control == null || !CardControls.TryGetValue(control, out marker))
                return;
            if (ReferenceEquals(control.BackgroundImage, marker.GeneratedBackground))
                control.BackgroundImage = null;
            if (marker.GeneratedBackground != null)
            {
                marker.GeneratedBackground.Dispose();
                marker.GeneratedBackground = null;
            }
            CardControls.Remove(control);
        }

        private static void UpdateCardBackground(Control control, CardMarker marker)
        {
            if (control == null || control.IsDisposed || control.Width <= 1 || control.Height <= 1)
                return;
            if (marker.GeneratedBackground != null && marker.BackgroundSize == control.ClientSize)
                return;

            Bitmap bitmap = new Bitmap(control.Width, control.Height, PixelFormat.Format32bppPArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.Clear(Color.Transparent);
                using (GraphicsPath path = CreateRoundedPath(
                    new RectangleF(0.5F, 0.5F, Math.Max(0F, control.Width - 1.5F), Math.Max(0F, control.Height - 1.5F)), 12F))
                using (SolidBrush brush = new SolidBrush(Surface))
                using (Pen pen = new Pen(Border, 1F))
                {
                    pen.Alignment = PenAlignment.Inset;
                    graphics.FillPath(brush, path);
                    graphics.DrawPath(pen, path);
                }
            }

            Image oldImage = marker.GeneratedBackground;
            marker.GeneratedBackground = bitmap;
            marker.BackgroundSize = control.ClientSize;
            control.BackgroundImage = bitmap;
            if (oldImage != null)
                oldImage.Dispose();
            control.Invalidate();
        }

        internal static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null || control.IsDisposed)
                return;

            RoundedMarker marker;
            if (!RoundedControls.TryGetValue(control, out marker))
            {
                marker = new RoundedMarker();
                RoundedControls.Add(control, marker);
                control.Resize += RoundedControl_Resize;
            }
            marker.Radius = Math.Max(2, radius);
            UpdateRoundedRegion(control, marker.Radius);
        }

        private static void RoundedControl_Resize(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control == null || control.IsDisposed)
                return;

            RoundedMarker marker;
            if (RoundedControls.TryGetValue(control, out marker))
                UpdateRoundedRegion(control, marker.Radius);
        }

        private static void UpdateRoundedRegion(Control control, int radius)
        {
            if (control.Width <= 1 || control.Height <= 1)
                return;

            Form form = control as Form;
            if (form != null && form.WindowState == FormWindowState.Maximized)
            {
                Region oldMaximizedRegion = control.Region;
                control.Region = null;
                if (oldMaximizedRegion != null)
                    oldMaximizedRegion.Dispose();
                return;
            }

            using (GraphicsPath path = CreateRoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius))
            {
                Region oldRegion = control.Region;
                control.Region = new Region(path);
                if (oldRegion != null)
                    oldRegion.Dispose();
            }
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Min(Math.Min(radius * 2, bounds.Width), bounds.Height);
            if (diameter <= 2)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            Rectangle arc = new Rectangle(bounds.Left, bounds.Top, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static GraphicsPath CreateRoundedPath(RectangleF bounds, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float safeRadius = Math.Max(1F, Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2F));
            float diameter = safeRadius * 2F;
            if (diameter <= 2F)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            RectangleF arc = new RectangleF(bounds.Left, bounds.Top, diameter, diameter);
            path.AddArc(arc, 180F, 90F);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270F, 90F);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0F, 90F);
            arc.X = bounds.Left;
            path.AddArc(arc, 90F, 90F);
            path.CloseFigure();
            return path;
        }

        private static void SoftInput_Paint(object sender, PaintEventArgs e)
        {
            Control control = sender as Control;
            if (control == null || control.Width <= 1 || control.Height <= 1)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedPath(new Rectangle(0, 0, control.Width - 1, control.Height - 1), 6))
            using (Pen pen = new Pen(Border))
                e.Graphics.DrawPath(pen, path);
        }

        private static void Card_Paint(object sender, PaintEventArgs e)
        {
            Control control = sender as Control;
            if (control == null || control.Width <= 1 || control.Height <= 1)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedPath(new Rectangle(0, 0, control.Width - 1, control.Height - 1), 12))
            using (Pen pen = new Pen(Border))
                e.Graphics.DrawPath(pen, path);
        }

        private static bool IsWindowChromeButton(Button button)
        {
            if (button == null)
                return false;

            string name = button.Name ?? string.Empty;
            if (name == "button100" || name == "btn_baseClose")
                return true;

            Form owner = button.FindForm();
            if (owner is Frm_FormBase && button.Parent != null && button.Parent.Name == "panel1")
                return name == "button1" || name == "button2";

            if (owner is Frm_Main && button.Parent != null && button.Parent.Name == "panel3")
                return name == "button1" || name == "button2" || name == "button3" || name == "button4";

            return false;
        }

        private static bool IsPrimaryAction(Button button)
        {
            string value = ((button.Name ?? string.Empty) + " " + (button.Text ?? string.Empty)).ToLowerInvariant();
            string[] tokens = { "run", "confirm", "save", "connect", "apply", "运行", "确定", "保存", "连接", "应用", "开始", "执行", "登录", "完成" };
            foreach (string token in tokens)
                if (value.Contains(token))
                    return true;
            return false;
        }

        private static bool IsNeutralAction(Button button)
        {
            string role = button.Tag as string;
            if (string.Equals(role, "secondary", StringComparison.OrdinalIgnoreCase))
                return true;

            string value = ((button.Name ?? string.Empty) + " " + (button.Text ?? string.Empty)).ToLowerInvariant();
            return value.Contains("cancel") || value.Contains("close") || value.Contains("back") ||
                   value.Contains("取消") || value.Contains("关闭") || value.Contains("返回");
        }

        private static bool IsDangerAction(Button button)
        {
            string value = ((button.Name ?? string.Empty) + " " + (button.Text ?? string.Empty)).ToLowerInvariant();
            return value.Contains("delete") || value.Contains("clear") || value.Contains("disconnect") ||
                   value.Contains("删除") || value.Contains("清空") || value.Contains("断开");
        }

        private static bool IsLegacyAccent(Color color)
        {
            return color == Color.FromArgb(46, 141, 230) ||
                   color == Color.FromArgb(30, 144, 255) ||
                   color == Color.FromArgb(26, 106, 175) ||
                   color == Color.FromArgb(18, 150, 219) ||
                   color == Color.DodgerBlue;
        }

        private sealed class ModernColorTable : ProfessionalColorTable
        {
            public override Color ToolStripBorder { get { return Border; } }
            public override Color ToolStripGradientBegin { get { return Surface; } }
            public override Color ToolStripGradientMiddle { get { return Surface; } }
            public override Color ToolStripGradientEnd { get { return Surface; } }
            public override Color MenuStripGradientBegin { get { return Surface; } }
            public override Color MenuStripGradientEnd { get { return Surface; } }
            public override Color StatusStripGradientBegin { get { return Color.FromArgb(242, 247, 253); } }
            public override Color StatusStripGradientEnd { get { return Color.FromArgb(242, 247, 253); } }
            public override Color MenuItemSelected { get { return Selection; } }
            public override Color MenuItemBorder { get { return Color.FromArgb(174, 210, 245); } }
            public override Color MenuItemSelectedGradientBegin { get { return Selection; } }
            public override Color MenuItemSelectedGradientEnd { get { return Selection; } }
            public override Color MenuItemPressedGradientBegin { get { return Selection; } }
            public override Color MenuItemPressedGradientEnd { get { return Selection; } }
            public override Color ButtonSelectedBorder { get { return Color.FromArgb(174, 210, 245); } }
            public override Color ButtonSelectedGradientBegin { get { return Selection; } }
            public override Color ButtonSelectedGradientEnd { get { return Selection; } }
            public override Color ButtonPressedBorder { get { return Accent; } }
            public override Color ButtonPressedGradientBegin { get { return Color.FromArgb(219, 236, 253); } }
            public override Color ButtonPressedGradientEnd { get { return Color.FromArgb(219, 236, 253); } }
            public override Color SeparatorDark { get { return Border; } }
            public override Color SeparatorLight { get { return Surface; } }
        }

        private sealed class ModernToolStripRenderer : ToolStripProfessionalRenderer
        {
            internal ModernToolStripRenderer()
                : base(new ModernColorTable())
            {
                RoundedEdges = true;
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                ToolStripButton button = e.Item as ToolStripButton;
                if (button != null && button.Checked)
                {
                    Rectangle bounds = new Rectangle(2, 2, Math.Max(1, e.Item.Width - 4), Math.Max(1, e.Item.Height - 4));
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (GraphicsPath path = CreateRoundedPath(bounds, 7))
                    using (SolidBrush brush = new SolidBrush(Selection))
                        e.Graphics.FillPath(brush, path);
                    using (Pen pen = new Pen(Accent, 2F))
                        e.Graphics.DrawLine(pen, bounds.Left + 4, bounds.Bottom - 2, bounds.Right - 4, bounds.Bottom - 2);
                    return;
                }

                base.OnRenderButtonBackground(e);
            }
        }
    }
}
