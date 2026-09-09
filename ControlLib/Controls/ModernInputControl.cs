using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Controls
{
    /// <summary>
    /// 自定义输入控件的统一圆角底板。
    /// 不使用 WinForms Region 裁切圆角，避免小尺寸和高 DPI 下产生阶梯锯齿。
    /// </summary>
    public abstract class ModernInputControl : UserControl
    {
        private Color surfaceColor = Color.FromArgb(255, 255, 253);
        private Color borderColor = Color.FromArgb(217, 227, 234);
        private Color focusBorderColor = Color.FromArgb(76, 148, 210);
        private Color textColor = Color.FromArgb(39, 56, 72);

        protected ModernInputControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);
            UpdateStyles();
            BackColor = Color.Transparent;
            ForeColor = textColor;
            TabStop = false;
        }

        protected Color InputSurfaceColor
        {
            get { return surfaceColor; }
        }

        protected Color InputTextColor
        {
            get { return textColor; }
        }

        protected Color InputFocusColor
        {
            get { return focusBorderColor; }
        }

        /// <summary>
        /// 由应用主题注入颜色；控件自身独立使用时也保留一致的默认配色。
        /// </summary>
        public void ApplyModernPalette(Color surface, Color border, Color focusBorder, Color foreground)
        {
            surfaceColor = surface;
            borderColor = border;
            focusBorderColor = focusBorder;
            textColor = foreground;
            ForeColor = foreground;
            BackColor = Color.Transparent;
            OnModernPaletteChanged();
            Invalidate();
        }

        protected virtual void OnModernPaletteChanged()
        {
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (Width <= 1 || Height <= 1)
                return;

            ConfigureQuality(e.Graphics);
            using (GraphicsPath path = CreateRoundedPath(GetPaintBounds(), GetCornerRadius()))
            using (SolidBrush brush = new SolidBrush(Enabled ? surfaceColor : Blend(surfaceColor, SystemColors.Control, 0.45F)))
                e.Graphics.FillPath(brush, path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width <= 1 || Height <= 1)
                return;

            ConfigureQuality(e.Graphics);
            Color outline = ContainsFocus && Enabled ? focusBorderColor : borderColor;
            using (GraphicsPath path = CreateRoundedPath(GetPaintBounds(), GetCornerRadius()))
            using (Pen pen = new Pen(outline, ContainsFocus && Enabled ? 1.35F : 1F))
            {
                pen.Alignment = PenAlignment.Inset;
                e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutInputChildren();
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            LayoutInputChildren();
            Invalidate();
        }

        protected abstract void LayoutInputChildren();

        protected static void ConfigureQuality(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
        }

        protected static Color Blend(Color first, Color second, float secondAmount)
        {
            float amount = Math.Max(0F, Math.Min(1F, secondAmount));
            return Color.FromArgb(
                (int)(first.R + ((second.R - first.R) * amount)),
                (int)(first.G + ((second.G - first.G) * amount)),
                (int)(first.B + ((second.B - first.B) * amount)));
        }

        private RectangleF GetPaintBounds()
        {
            // 半像素内缩使 1px 描边落在像素中心，同时保留抗锯齿过渡像素。
            return new RectangleF(0.5F, 0.5F, Math.Max(0F, Width - 1.5F), Math.Max(0F, Height - 1.5F));
        }

        private float GetCornerRadius()
        {
            return Math.Max(4F, Math.Min(7F, (Height - 2F) / 2F));
        }

        private static GraphicsPath CreateRoundedPath(RectangleF bounds, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float safeRadius = Math.Max(1F, Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2F));
            float diameter = safeRadius * 2F;
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
    }
}
