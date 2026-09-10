using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WinFormsLabel = System.Windows.Forms.Label;

namespace VMPro
{
    internal partial class Frm_Welcome : Form
    {
        private static readonly Color WarmWhite = Color.FromArgb(252, 250, 246);
        private static readonly Color SoftBlue = Color.FromArgb(84, 166, 232);
        private static readonly Color Ink = Color.FromArgb(38, 55, 72);
        private static readonly Color MutedInk = Color.FromArgb(106, 123, 139);

        private WinFormsLabel brandMarkLabel;
        private WinFormsLabel productTitleLabel;
        private WinFormsLabel productCaptionLabel;
        private WelcomeIllustration illustration;
        private WelcomeProgressIndicator progressIndicator;
        private System.Windows.Forms.Timer progressVisualTimer;

        internal Frm_Welcome()
        {
            InitializeComponent();
            ApplyModernWelcomeLayout();
        }

        private void ApplyModernWelcomeLayout()
        {
            SuspendLayout();
            // InitializeComponent 的设计基准是 6x12 字体单位；构造后切成 Dpi
            // 会让 WinForms 以错误基准再次缩放。沿用 Font 缩放，待整套历史绝对
            // 布局完成 Per-Monitor DPI 回归后再统一迁移。
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(680, 380);
            Text = Configuration.ProductDisplayName;
            ApplyExecutableIcon();
            BackColor = WarmWhite;
            ForeColor = Ink;
            DoubleBuffered = true;

            // 旧资源中的白色 Logo 和高饱和动态图只适合纯蓝背景。新版启动页使用
            // 同一套浅蓝矢量图形，避免换色以后出现割裂的贴图块。
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            bar_step.Visible = false;

            brandMarkLabel = new WelcomeBrandMark(SoftBlue);
            brandMarkLabel.Name = "welcomeBrandMark";
            brandMarkLabel.AutoSize = false;
            brandMarkLabel.Location = new Point(34, 26);
            brandMarkLabel.Size = new Size(52, 42);
            brandMarkLabel.BackColor = Color.Transparent;
            brandMarkLabel.ForeColor = Color.White;
            brandMarkLabel.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point);
            brandMarkLabel.Text = "WLP";
            brandMarkLabel.TextAlign = ContentAlignment.MiddleCenter;

            label1.AutoSize = false;
            label1.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Ink;
            label1.Location = new Point(98, 26);
            label1.Size = new Size(160, 26);
            label1.Text = Configuration.ProductName;

            lbl_version.AutoSize = false;
            lbl_version.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_version.ForeColor = MutedInk;
            lbl_version.Location = new Point(99, 51);
            lbl_version.Size = new Size(220, 20);
            lbl_version.Text = GetVersionLabelText();

            lbl_companyName.AutoSize = false;
            lbl_companyName.Font = ModernUiTheme.UiFont;
            lbl_companyName.ForeColor = MutedInk;
            lbl_companyName.Location = new Point(36, 105);
            lbl_companyName.Size = new Size(370, 24);
            lbl_companyName.Text = Configuration.DefaultCompanyName;
            lbl_companyName.TextAlign = ContentAlignment.MiddleLeft;

            productTitleLabel = new WinFormsLabel();
            productTitleLabel.Name = "welcomeProductTitle";
            productTitleLabel.AutoSize = false;
            productTitleLabel.Font = ModernUiTheme.WelcomeTitleFont;
            productTitleLabel.ForeColor = Ink;
            productTitleLabel.Location = new Point(32, 135);
            productTitleLabel.Size = new Size(380, 52);
            productTitleLabel.Text = Configuration.ProductDisplayName;
            productTitleLabel.TextAlign = ContentAlignment.MiddleLeft;

            productCaptionLabel = new WinFormsLabel();
            productCaptionLabel.Name = "welcomeProductCaption";
            productCaptionLabel.AutoSize = false;
            productCaptionLabel.Font = ModernUiTheme.UiFont;
            productCaptionLabel.ForeColor = MutedInk;
            productCaptionLabel.Location = new Point(36, 191);
            productCaptionLabel.Size = new Size(370, 50);
            productCaptionLabel.Text = "视觉流程编排 · 检测识别 · 标定定位 · 设备协同";

            illustration = new WelcomeIllustration();
            illustration.Name = "welcomeIllustration";
            illustration.Location = new Point(438, 92);
            illustration.Size = new Size(198, 174);
            illustration.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            lbl_step.AutoSize = false;
            lbl_step.Font = ModernUiTheme.UiFont;
            lbl_step.ForeColor = MutedInk;
            lbl_step.Location = new Point(36, 307);
            lbl_step.Size = new Size(608, 24);
            lbl_step.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            lbl_step.TextAlign = ContentAlignment.MiddleLeft;

            progressIndicator = new WelcomeProgressIndicator();
            progressIndicator.Name = "welcomeProgress";
            progressIndicator.Location = new Point(36, 338);
            progressIndicator.Size = new Size(608, 9);
            progressIndicator.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            btn_exit.Location = new Point(ClientSize.Width - 50, 18);
            btn_exit.Size = new Size(32, 32);
            btn_exit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_exit.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btn_exit.BackColor = WarmWhite;
            btn_exit.ForeColor = MutedInk;
            btn_exit.FlatStyle = FlatStyle.Flat;
            btn_exit.FlatAppearance.BorderSize = 0;
            btn_exit.FlatAppearance.MouseOverBackColor = Color.FromArgb(250, 232, 229);
            btn_exit.FlatAppearance.MouseDownBackColor = Color.FromArgb(245, 217, 214);
            btn_exit.AccessibleName = Project.Instance.configuration.language == Language.English ? "Exit" : "退出";
            ModernUiTheme.RefreshRoundedButtonPalette(
                btn_exit,
                WarmWhite,
                Color.FromArgb(250, 232, 229),
                Color.FromArgb(245, 217, 214),
                // FlatAppearance.BorderColor 不接受透明色；使用与常态填充相同的
                // 暖白色，视觉上保持无边，同时避免欢迎页构造时抛异常。
                WarmWhite);

            Controls.Add(brandMarkLabel);
            Controls.Add(productTitleLabel);
            Controls.Add(productCaptionLabel);
            Controls.Add(illustration);
            Controls.Add(progressIndicator);
            btn_exit.BringToFront();

            EnableWindowDrag(brandMarkLabel);
            EnableWindowDrag(label1);
            EnableWindowDrag(lbl_version);
            EnableWindowDrag(lbl_companyName);
            EnableWindowDrag(productTitleLabel);
            EnableWindowDrag(productCaptionLabel);
            EnableWindowDrag(illustration);

            Paint += Frm_Welcome_Paint;
            Resize += delegate { UpdateRoundedRegion(); };
            UpdateRoundedRegion();

            progressVisualTimer = new System.Windows.Forms.Timer();
            progressVisualTimer.Interval = 80;
            progressVisualTimer.Tick += delegate
            {
                if (progressIndicator != null && !progressIndicator.IsDisposed)
                    progressIndicator.Value = bar_step.Value;
            };
            VisibleChanged += delegate
            {
                if (progressVisualTimer != null)
                    progressVisualTimer.Enabled = Visible;
            };
            Disposed += delegate
            {
                if (progressVisualTimer != null)
                {
                    progressVisualTimer.Stop();
                    progressVisualTimer.Dispose();
                    progressVisualTimer = null;
                }
            };
            ResumeLayout(false);
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

        private void EnableWindowDrag(Control control)
        {
            control.MouseDown += setForm_MouseDown;
            control.MouseMove += setForm_MouseMove;
            control.MouseUp += setForm_MouseUp;
        }

        private void UpdateRoundedRegion()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            using (GraphicsPath path = CreateRoundedRectangle(
                new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), 16))
            {
                Region oldRegion = Region;
                Region = new Region(path);
                if (oldRegion != null)
                    oldRegion.Dispose();
            }

            // 内部标牌和按钮使用透明背景上的抗锯齿绘制，不再用二值 Region
            // 裁切；这里只保留顶层无边框启动窗体的兼容轮廓。
            if (brandMarkLabel != null && brandMarkLabel.Region != null)
            {
                Region oldRegion = brandMarkLabel.Region;
                brandMarkLabel.Region = null;
                oldRegion.Dispose();
            }
            if (btn_exit != null && btn_exit.Region != null)
            {
                Region oldRegion = btn_exit.Region;
                btn_exit.Region = null;
                oldRegion.Dispose();
            }
        }

        private void Frm_Welcome_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRectangle(
                new Rectangle(1, 1, ClientSize.Width - 3, ClientSize.Height - 3), 15))
            using (Pen pen = new Pen(Color.FromArgb(210, 226, 239)))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Max(2, radius * 2);
            Rectangle arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);
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

        #region 窗体拖动
        private static bool IsDrag = false;
        private int enterX;
        private int enterY;
        private void setForm_MouseDown(object sender, MouseEventArgs e)
        {
            IsDrag = true;
            enterX = e.Location.X;
            enterY = e.Location.Y;
        }
        private void setForm_MouseUp(object sender, MouseEventArgs e)
        {
            IsDrag = false;
            enterX = 0;
            enterY = 0;
        }
        private void setForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrag)
            {
                Left += e.Location.X - enterX;
                Top += e.Location.Y - enterY;
            }
        }
        #endregion
        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_Welcome _instance;
        internal static Frm_Welcome Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_Welcome();
                return _instance;
            }
        }
        private void Frm_Welcome_Load(object sender, EventArgs e)
        {
            lbl_version.Text = GetVersionLabelText();
            bar_step.Maximum = 100;
            lbl_companyName.Text = Configuration.DefaultCompanyName;
            productTitleLabel.Text = Configuration.ProductDisplayName;
            productCaptionLabel.Text = Project.Instance.configuration.language == Language.English
                ? "Workflow · Inspection · Calibration · Device integration"
                : "视觉流程编排 · 检测识别 · 标定定位 · 设备协同";

        }

        private static string GetVersionLabelText()
        {
            return Project.Instance.configuration.language == Language.English
                ? "Version " + Configuration.ProductVersion
                : "版本 " + Configuration.ProductVersion;
        }
        private void btn_exit_Click(object sender, EventArgs e)
        {
            try
            {
                SDK_Basler.CloseAllCamera();
            }
            catch { }
            try
            {
                SDK_HIKVision.CloseAllCamera(); 
            }
            catch { }
            try
            {
                SDK_MindVision.CloseAllCamera(); 
            }
            catch { }
            try
            {
                SDK_PointGrey.CloseAllCamera();
            }
            catch { }
            Process.GetCurrentProcess().Kill();
        }

    }

    /// <summary>
    /// 启动页品牌标牌：透明矩形控件内绘制柔和圆角，边缘保留 Alpha
    /// 过渡，避免小尺寸 Label.Region 的锯齿。
    /// </summary>
    internal sealed class WelcomeBrandMark : WinFormsLabel
    {
        private readonly Color fillColor;

        internal WelcomeBrandMark(Color fill)
        {
            fillColor = fill;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle bounds = new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1));
            using (GraphicsPath path = CreatePath(bounds, 11))
            using (SolidBrush brush = new SolidBrush(fillColor))
                e.Graphics.FillPath(brush, path);
        }

        private static GraphicsPath CreatePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Max(2, Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height)));
            Rectangle arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);
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
    }

    /// <summary>
    /// 启动页专用的轻量矢量插图，不依赖外部图片或相机 SDK。
    /// </summary>
    internal sealed class WelcomeIllustration : Control
    {
        internal WelcomeIllustration()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle cardBounds = new Rectangle(2, 2, Width - 5, Height - 5);
            using (GraphicsPath cardPath = CreatePath(cardBounds, 24))
            using (LinearGradientBrush cardBrush = new LinearGradientBrush(
                cardBounds,
                Color.FromArgb(239, 248, 255),
                Color.FromArgb(251, 247, 239),
                35F))
            using (Pen borderPen = new Pen(Color.FromArgb(207, 228, 244)))
            {
                e.Graphics.FillPath(cardBrush, cardPath);
                e.Graphics.DrawPath(borderPen, cardPath);
            }

            using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(68, 163, 231)))
            {
                e.Graphics.FillEllipse(glowBrush, Width - 74, 20, 34, 34);
            }

            Rectangle lensBounds = new Rectangle(60, 42, 82, 82);
            using (Pen outerPen = new Pen(Color.FromArgb(84, 166, 232), 7F))
            using (Pen innerPen = new Pen(Color.FromArgb(169, 216, 247), 5F))
            using (SolidBrush lensBrush = new SolidBrush(Color.FromArgb(250, 253, 255)))
            {
                e.Graphics.FillEllipse(lensBrush, lensBounds);
                e.Graphics.DrawEllipse(outerPen, lensBounds);
                Rectangle innerBounds = Rectangle.Inflate(lensBounds, -18, -18);
                e.Graphics.DrawEllipse(innerPen, innerBounds);
            }

            using (Pen focusPen = new Pen(Color.FromArgb(47, 137, 207), 3F))
            {
                int left = 42;
                int top = 29;
                int right = Width - 41;
                int bottom = Height - 30;
                int corner = 22;
                e.Graphics.DrawLine(focusPen, left, top + corner, left, top);
                e.Graphics.DrawLine(focusPen, left, top, left + corner, top);
                e.Graphics.DrawLine(focusPen, right - corner, top, right, top);
                e.Graphics.DrawLine(focusPen, right, top, right, top + corner);
                e.Graphics.DrawLine(focusPen, left, bottom - corner, left, bottom);
                e.Graphics.DrawLine(focusPen, left, bottom, left + corner, bottom);
                e.Graphics.DrawLine(focusPen, right - corner, bottom, right, bottom);
                e.Graphics.DrawLine(focusPen, right, bottom, right, bottom - corner);
            }
        }

        private static GraphicsPath CreatePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);
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
    }

    internal sealed class WelcomeProgressIndicator : Control
    {
        private int value;

        internal WelcomeProgressIndicator()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
        }

        internal int Value
        {
            get { return value; }
            set
            {
                int normalized = Math.Max(0, Math.Min(100, value));
                if (this.value == normalized)
                    return;
                this.value = normalized;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle track = new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1));
            using (GraphicsPath trackPath = CreateCapsule(track))
            using (SolidBrush trackBrush = new SolidBrush(Color.FromArgb(221, 234, 243)))
                e.Graphics.FillPath(trackBrush, trackPath);

            int progressWidth = (int)Math.Round(track.Width * value / 100D);
            if (progressWidth <= 0)
                return;

            Rectangle progress = new Rectangle(track.X, track.Y, Math.Max(track.Height, progressWidth), track.Height);
            if (progress.Right > track.Right)
                progress.Width = track.Right - progress.Left;
            using (GraphicsPath progressPath = CreateCapsule(progress))
            using (LinearGradientBrush progressBrush = new LinearGradientBrush(
                progress,
                Color.FromArgb(104, 185, 239),
                Color.FromArgb(62, 144, 218),
                0F))
                e.Graphics.FillPath(progressBrush, progressPath);
        }

        private static GraphicsPath CreateCapsule(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Max(1, Math.Min(bounds.Height, bounds.Width));
            Rectangle left = new Rectangle(bounds.X, bounds.Y, diameter, bounds.Height);
            Rectangle right = new Rectangle(bounds.Right - diameter, bounds.Y, diameter, bounds.Height);
            path.AddArc(left, 90, 180);
            path.AddArc(right, 270, 180);
            path.CloseFigure();
            return path;
        }
    }
}
