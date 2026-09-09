using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace VMPro
{
    /// <summary>
    /// 以当前显示器的实际像素尺寸绘制命令图标，避免旧位图在 DPI 缩放时被二次拉伸。
    /// 这里生成的是透明背景的运行时位图；业务按钮及其 Click 事件保持不变。
    /// </summary>
    internal static class ModernVectorIconFactory
    {
        internal enum Glyph
        {
            Play,
            Pause,
            Stop,
            Reset,
            Home,
            Vision,
            Motion,
            Repeat,
            Save,
            OpenImage,
            SaveImage,
            BatchRun,
            NewWindow,
            Toolbox,
            Layout,
            Add,
            Expand,
            Collapse,
            Delete,
            Info,
            Search,
            Minimize,
            Maximize,
            Restore,
            Pin,
            Close,
            Connection,
            More
        }

        private static readonly object CacheSync = new object();
        private static readonly Dictionary<string, Image> Cache = new Dictionary<string, Image>();

        internal static int GetPixelSize(Control owner, int logicalSize)
        {
            float dpi = 96F;
            try
            {
                if (owner != null && !owner.IsDisposed)
                {
                    using (Graphics graphics = owner.CreateGraphics())
                        dpi = graphics.DpiX;
                }
            }
            catch
            {
                // 尚未创建句柄时按标准 DPI 绘制，界面显示不应因图标失败而中断。
            }

            return Math.Max(logicalSize, (int)Math.Round(logicalSize * dpi / 96F));
        }

        internal static Image Get(Glyph glyph, int pixelSize, Color color)
        {
            pixelSize = Math.Max(16, Math.Min(64, pixelSize));
            string cacheKey = ((int)glyph).ToString() + ":" + pixelSize.ToString() + ":" + color.ToArgb().ToString();

            lock (CacheSync)
            {
                Image cached;
                if (Cache.TryGetValue(cacheKey, out cached))
                    return cached;

                cached = Draw(glyph, pixelSize, color);
                Cache.Add(cacheKey, cached);
                return cached;
            }
        }

        private static Bitmap Draw(Glyph glyph, int pixelSize, Color color)
        {
            Bitmap bitmap = new Bitmap(pixelSize, pixelSize, PixelFormat.Format32bppPArgb);
            bitmap.SetResolution(96F, 96F);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Pen pen = CreatePen(color, 1.75F))
            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.Clear(Color.Transparent);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.ScaleTransform(pixelSize / 24F, pixelSize / 24F);

                switch (glyph)
                {
                    case Glyph.Play:
                        DrawPlay(graphics, pen, brush);
                        break;
                    case Glyph.Pause:
                        DrawPause(graphics, pen, brush);
                        break;
                    case Glyph.Stop:
                        DrawStop(graphics, pen, brush);
                        break;
                    case Glyph.Reset:
                        DrawReset(graphics, pen, brush);
                        break;
                    case Glyph.Home:
                        DrawHome(graphics, pen);
                        break;
                    case Glyph.Vision:
                        DrawVision(graphics, pen, brush);
                        break;
                    case Glyph.Motion:
                        DrawMotion(graphics, pen, brush);
                        break;
                    case Glyph.Repeat:
                        DrawRepeat(graphics, pen, brush);
                        break;
                    case Glyph.Save:
                        DrawSave(graphics, pen);
                        break;
                    case Glyph.OpenImage:
                        DrawOpenImage(graphics, pen, brush);
                        break;
                    case Glyph.SaveImage:
                        DrawSaveImage(graphics, pen, brush);
                        break;
                    case Glyph.BatchRun:
                        DrawBatchRun(graphics, pen, brush);
                        break;
                    case Glyph.NewWindow:
                        DrawNewWindow(graphics, pen);
                        break;
                    case Glyph.Toolbox:
                        DrawToolbox(graphics, pen);
                        break;
                    case Glyph.Layout:
                        DrawLayout(graphics, pen);
                        break;
                    case Glyph.Add:
                        DrawAdd(graphics, pen);
                        break;
                    case Glyph.Expand:
                        DrawExpand(graphics, pen);
                        break;
                    case Glyph.Collapse:
                        DrawCollapse(graphics, pen);
                        break;
                    case Glyph.Delete:
                        DrawDelete(graphics, pen);
                        break;
                    case Glyph.Info:
                        DrawInfo(graphics, pen, brush);
                        break;
                    case Glyph.Search:
                        DrawSearch(graphics, pen);
                        break;
                    case Glyph.Minimize:
                        DrawMinimize(graphics, pen);
                        break;
                    case Glyph.Maximize:
                        DrawMaximize(graphics, pen);
                        break;
                    case Glyph.Restore:
                        DrawRestore(graphics, pen);
                        break;
                    case Glyph.Pin:
                        DrawPin(graphics, pen);
                        break;
                    case Glyph.Close:
                        DrawClose(graphics, pen);
                        break;
                    case Glyph.Connection:
                        DrawConnection(graphics, pen, brush);
                        break;
                    default:
                        DrawMore(graphics, brush);
                        break;
                }
            }

            return bitmap;
        }

        private static Pen CreatePen(Color color, float width)
        {
            Pen pen = new Pen(color, width);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            pen.LineJoin = LineJoin.Round;
            return pen;
        }

        private static GraphicsPath RoundedRectangle(float x, float y, float width, float height, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float diameter = radius * 2F;
            path.AddArc(x, y, diameter, diameter, 180F, 90F);
            path.AddArc(x + width - diameter, y, diameter, diameter, 270F, 90F);
            path.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0F, 90F);
            path.AddArc(x, y + height - diameter, diameter, diameter, 90F, 90F);
            path.CloseFigure();
            return path;
        }

        private static void DrawPlay(Graphics graphics, Pen pen, Brush brush)
        {
            graphics.DrawEllipse(pen, 3.75F, 3.75F, 16.5F, 16.5F);
            graphics.FillPolygon(brush, new[]
            {
                new PointF(10F, 8F), new PointF(16.25F, 12F), new PointF(10F, 16F)
            });
        }

        private static void DrawPause(Graphics graphics, Pen pen, Brush brush)
        {
            graphics.DrawEllipse(pen, 3.75F, 3.75F, 16.5F, 16.5F);
            using (GraphicsPath left = RoundedRectangle(8.2F, 7.8F, 2.5F, 8.4F, 1.1F))
            using (GraphicsPath right = RoundedRectangle(13.3F, 7.8F, 2.5F, 8.4F, 1.1F))
            {
                graphics.FillPath(brush, left);
                graphics.FillPath(brush, right);
            }
        }

        private static void DrawStop(Graphics graphics, Pen pen, Brush brush)
        {
            graphics.DrawEllipse(pen, 3.75F, 3.75F, 16.5F, 16.5F);
            using (GraphicsPath square = RoundedRectangle(8F, 8F, 8F, 8F, 1.4F))
                graphics.FillPath(brush, square);
        }

        private static void DrawReset(Graphics graphics, Pen pen, Brush brush)
        {
            graphics.DrawArc(pen, 4.25F, 4.25F, 15.5F, 15.5F, 44F, 286F);
            graphics.FillPolygon(brush, new[]
            {
                new PointF(3.65F, 5.25F), new PointF(9.2F, 5.15F), new PointF(6.15F, 9.45F)
            });
            graphics.DrawLine(pen, 12F, 8F, 12F, 12F);
            graphics.DrawLine(pen, 12F, 12F, 15.1F, 14F);
        }

        private static void DrawHome(Graphics graphics, Pen pen)
        {
            graphics.DrawLines(pen, new[]
            {
                new PointF(3.8F, 11F), new PointF(12F, 4.6F), new PointF(20.2F, 11F)
            });
            graphics.DrawLines(pen, new[]
            {
                new PointF(6.2F, 10F), new PointF(6.2F, 19.2F), new PointF(17.8F, 19.2F),
                new PointF(17.8F, 10F)
            });
            graphics.DrawLines(pen, new[]
            {
                new PointF(10F, 19.2F), new PointF(10F, 14F), new PointF(14F, 14F), new PointF(14F, 19.2F)
            });
        }

        private static void DrawVision(Graphics graphics, Pen pen, Brush brush)
        {
            using (GraphicsPath camera = RoundedRectangle(3.5F, 7F, 17F, 12.5F, 2.2F))
                graphics.DrawPath(pen, camera);
            graphics.DrawLines(pen, new[]
            {
                new PointF(8F, 7F), new PointF(9.5F, 4.7F), new PointF(14.5F, 4.7F), new PointF(16F, 7F)
            });
            graphics.DrawEllipse(pen, 8.25F, 9.25F, 7.5F, 7.5F);
            graphics.FillEllipse(brush, 17.2F, 9.1F, 1.6F, 1.6F);
        }

        private static void DrawMotion(Graphics graphics, Pen pen, Brush brush)
        {
            graphics.DrawLine(pen, 5F, 18.5F, 19.2F, 18.5F);
            graphics.DrawLine(pen, 5F, 18.5F, 5F, 4.4F);
            graphics.FillPolygon(brush, new[]
            {
                new PointF(19.8F, 18.5F), new PointF(16.3F, 16.4F), new PointF(16.3F, 20.6F)
            });
            graphics.FillPolygon(brush, new[]
            {
                new PointF(5F, 3.7F), new PointF(2.9F, 7.2F), new PointF(7.1F, 7.2F)
            });
            graphics.DrawEllipse(pen, 9F, 8F, 7F, 7F);
            graphics.FillEllipse(brush, 11.1F, 10.1F, 2.8F, 2.8F);
        }

        private static void DrawRepeat(Graphics graphics, Pen pen, Brush brush)
        {
            graphics.DrawArc(pen, 4F, 5F, 16F, 11F, 197F, 250F);
            graphics.DrawArc(pen, 4F, 8F, 16F, 11F, 17F, 250F);
            graphics.FillPolygon(brush, new[]
            {
                new PointF(19.7F, 6.1F), new PointF(15.2F, 5.3F), new PointF(17.2F, 9.4F)
            });
            graphics.FillPolygon(brush, new[]
            {
                new PointF(4.3F, 17.9F), new PointF(8.8F, 18.7F), new PointF(6.8F, 14.6F)
            });
        }

        private static void DrawSave(Graphics graphics, Pen pen)
        {
            using (GraphicsPath body = RoundedRectangle(4F, 3.5F, 16F, 17F, 1.8F))
                graphics.DrawPath(pen, body);
            graphics.DrawRectangle(pen, 7.2F, 4.1F, 8.2F, 5.2F);
            using (GraphicsPath label = RoundedRectangle(7.2F, 13F, 9.6F, 7F, 1.3F))
                graphics.DrawPath(pen, label);
            graphics.DrawLine(pen, 14F, 5.3F, 14F, 8F);
        }

        private static void DrawImageFrame(Graphics graphics, Pen pen, Brush brush)
        {
            using (GraphicsPath frame = RoundedRectangle(3.5F, 4.5F, 17F, 14.5F, 2F))
                graphics.DrawPath(pen, frame);
            graphics.FillEllipse(brush, 7F, 7.5F, 2.3F, 2.3F);
            graphics.DrawLines(pen, new[]
            {
                new PointF(5.7F, 16.8F), new PointF(10F, 12.3F), new PointF(13F, 15F),
                new PointF(15.3F, 12.8F), new PointF(18.6F, 16.4F)
            });
        }

        private static void DrawOpenImage(Graphics graphics, Pen pen, Brush brush)
        {
            DrawImageFrame(graphics, pen, brush);
            graphics.DrawLine(pen, 12F, 2.9F, 12F, 8.6F);
            graphics.FillPolygon(brush, new[]
            {
                new PointF(12F, 2.5F), new PointF(9.4F, 5.6F), new PointF(14.6F, 5.6F)
            });
        }

        private static void DrawSaveImage(Graphics graphics, Pen pen, Brush brush)
        {
            DrawImageFrame(graphics, pen, brush);
            graphics.DrawLine(pen, 12F, 11.6F, 12F, 21.1F);
            graphics.FillPolygon(brush, new[]
            {
                new PointF(12F, 21.5F), new PointF(9.2F, 18.1F), new PointF(14.8F, 18.1F)
            });
        }

        private static void DrawBatchRun(Graphics graphics, Pen pen, Brush brush)
        {
            using (GraphicsPath back = RoundedRectangle(3.7F, 5F, 12.5F, 13.5F, 2F))
                graphics.DrawPath(pen, back);
            using (GraphicsPath front = RoundedRectangle(7.8F, 3.5F, 12.5F, 13.5F, 2F))
                graphics.DrawPath(pen, front);
            graphics.FillPolygon(brush, new[]
            {
                new PointF(12.3F, 7.4F), new PointF(17F, 10.3F), new PointF(12.3F, 13.2F)
            });
        }

        private static void DrawNewWindow(Graphics graphics, Pen pen)
        {
            using (GraphicsPath window = RoundedRectangle(3.5F, 4.5F, 17F, 15F, 2F))
                graphics.DrawPath(pen, window);
            graphics.DrawLine(pen, 3.9F, 8.2F, 20.1F, 8.2F);
            graphics.DrawLine(pen, 12F, 10.7F, 12F, 17.2F);
            graphics.DrawLine(pen, 8.75F, 14F, 15.25F, 14F);
        }

        private static void DrawToolbox(Graphics graphics, Pen pen)
        {
            using (GraphicsPath box = RoundedRectangle(3.5F, 7F, 17F, 12.5F, 2F))
                graphics.DrawPath(pen, box);
            using (GraphicsPath handle = RoundedRectangle(8F, 3.8F, 8F, 5F, 1.5F))
                graphics.DrawPath(pen, handle);
            graphics.DrawLine(pen, 3.8F, 11.5F, 20.2F, 11.5F);
            graphics.DrawLine(pen, 10.4F, 10.2F, 10.4F, 12.8F);
            graphics.DrawLine(pen, 13.6F, 10.2F, 13.6F, 12.8F);
        }

        private static void DrawLayout(Graphics graphics, Pen pen)
        {
            using (GraphicsPath frame = RoundedRectangle(3.5F, 4F, 17F, 16F, 2F))
                graphics.DrawPath(pen, frame);
            graphics.DrawLine(pen, 10F, 4.4F, 10F, 19.6F);
            graphics.DrawLine(pen, 10.4F, 11F, 20.1F, 11F);
        }

        private static void DrawAdd(Graphics graphics, Pen pen)
        {
            using (GraphicsPath card = RoundedRectangle(4F, 4F, 16F, 16F, 3F))
                graphics.DrawPath(pen, card);
            graphics.DrawLine(pen, 12F, 8F, 12F, 16F);
            graphics.DrawLine(pen, 8F, 12F, 16F, 12F);
        }

        private static void DrawExpand(Graphics graphics, Pen pen)
        {
            using (GraphicsPath top = RoundedRectangle(4F, 4F, 16F, 6F, 1.5F))
            using (GraphicsPath bottom = RoundedRectangle(4F, 14F, 16F, 6F, 1.5F))
            {
                graphics.DrawPath(pen, top);
                graphics.DrawPath(pen, bottom);
            }
            graphics.DrawLines(pen, new[]
            {
                new PointF(9F, 10.8F), new PointF(12F, 13.2F), new PointF(15F, 10.8F)
            });
        }

        private static void DrawCollapse(Graphics graphics, Pen pen)
        {
            using (GraphicsPath top = RoundedRectangle(4F, 4F, 16F, 6F, 1.5F))
            using (GraphicsPath bottom = RoundedRectangle(4F, 14F, 16F, 6F, 1.5F))
            {
                graphics.DrawPath(pen, top);
                graphics.DrawPath(pen, bottom);
            }
            graphics.DrawLines(pen, new[]
            {
                new PointF(9F, 13.2F), new PointF(12F, 10.8F), new PointF(15F, 13.2F)
            });
        }

        private static void DrawDelete(Graphics graphics, Pen pen)
        {
            using (GraphicsPath bin = RoundedRectangle(6F, 7F, 12F, 13F, 2F))
                graphics.DrawPath(pen, bin);
            graphics.DrawLine(pen, 4.5F, 7F, 19.5F, 7F);
            graphics.DrawLine(pen, 9F, 4F, 15F, 4F);
            graphics.DrawLine(pen, 10F, 10F, 10F, 17F);
            graphics.DrawLine(pen, 14F, 10F, 14F, 17F);
        }

        private static void DrawInfo(Graphics graphics, Pen pen, Brush brush)
        {
            graphics.DrawEllipse(pen, 4F, 4F, 16F, 16F);
            graphics.FillEllipse(brush, 10.6F, 7F, 2.8F, 2.8F);
            graphics.DrawLine(pen, 12F, 11.5F, 12F, 17F);
        }

        private static void DrawSearch(Graphics graphics, Pen pen)
        {
            graphics.DrawEllipse(pen, 4.5F, 4.5F, 11F, 11F);
            graphics.DrawLine(pen, 14.2F, 14.2F, 20F, 20F);
        }

        private static void DrawMinimize(Graphics graphics, Pen pen)
        {
            graphics.DrawLine(pen, 6F, 16.5F, 18F, 16.5F);
        }

        private static void DrawMaximize(Graphics graphics, Pen pen)
        {
            graphics.DrawRectangle(pen, 6.25F, 6.25F, 11.5F, 11.5F);
        }

        private static void DrawRestore(Graphics graphics, Pen pen)
        {
            graphics.DrawRectangle(pen, 5.75F, 8.25F, 10F, 10F);
            graphics.DrawLines(pen, new[]
            {
                new PointF(8.25F, 8.25F), new PointF(8.25F, 5.75F),
                new PointF(18.25F, 5.75F), new PointF(18.25F, 15.75F),
                new PointF(15.75F, 15.75F)
            });
        }

        private static void DrawPin(Graphics graphics, Pen pen)
        {
            graphics.DrawLine(pen, 8F, 5.5F, 16F, 5.5F);
            graphics.DrawLine(pen, 9.25F, 5.5F, 10.5F, 11F);
            graphics.DrawLine(pen, 14.75F, 5.5F, 13.5F, 11F);
            graphics.DrawLine(pen, 8.5F, 11F, 15.5F, 11F);
            graphics.DrawLine(pen, 12F, 11F, 12F, 19F);
        }

        private static void DrawClose(Graphics graphics, Pen pen)
        {
            graphics.DrawLine(pen, 6F, 6F, 18F, 18F);
            graphics.DrawLine(pen, 18F, 6F, 6F, 18F);
        }

        private static void DrawConnection(Graphics graphics, Pen pen, Brush brush)
        {
            graphics.FillEllipse(brush, 3.5F, 5F, 4F, 4F);
            graphics.FillEllipse(brush, 16.5F, 15F, 4F, 4F);
            graphics.DrawLines(pen, new[]
            {
                new PointF(7.5F, 7F), new PointF(12F, 7F),
                new PointF(12F, 17F), new PointF(16.5F, 17F)
            });
        }

        private static void DrawMore(Graphics graphics, Brush brush)
        {
            graphics.FillEllipse(brush, 4.4F, 10.4F, 3.2F, 3.2F);
            graphics.FillEllipse(brush, 10.4F, 10.4F, 3.2F, 3.2F);
            graphics.FillEllipse(brush, 16.4F, 10.4F, 3.2F, 3.2F);
        }
    }

    internal partial class Frm_Main
    {
        private bool modernMainIconEventsBound;
        private int modernMainCommandIconSize;

        private void ConfigureModernMainCommandIcons()
        {
            if (modernMainCommandIconSize <= 0)
                modernMainCommandIconSize = ModernVectorIconFactory.GetPixelSize(toolStrip1, 22);
            int iconSize = modernMainCommandIconSize;

            SetModernCommandIcon(toolStripButton4, ModernVectorIconFactory.Glyph.Play, iconSize, ModernUiTheme.Accent);
            SetModernCommandIcon(toolStripButton36, ModernVectorIconFactory.Glyph.Pause, iconSize, ModernUiTheme.Accent);
            SetModernCommandIcon(toolStripButton3, ModernVectorIconFactory.Glyph.Stop, iconSize, ModernUiTheme.Danger);
            SetModernCommandIcon(toolStripButton8, ModernVectorIconFactory.Glyph.Reset, iconSize, ModernUiTheme.Accent);
            RefreshModernWorkspaceIcons(iconSize);

            if (modernMainIconEventsBound)
                return;

            modernMainIconEventsBound = true;
            ToolStripButton[] commandButtons =
            {
                toolStripButton4, toolStripButton36, toolStripButton3, toolStripButton8,
                toolStripButton9, toolStripButton5, toolStripButton1
            };
            foreach (ToolStripButton button in commandButtons)
            {
                button.MouseEnter += RestoreModernMainCommandIcons;
                button.MouseLeave += RestoreModernMainCommandIcons;
                button.Click += RestoreModernMainCommandIcons;
            }
        }

        private void RestoreModernMainCommandIcons(object sender, EventArgs e)
        {
            ConfigureModernMainCommandIcons();
        }

        private void RefreshModernWorkspaceIcons()
        {
            if (modernMainCommandIconSize <= 0)
                modernMainCommandIconSize = ModernVectorIconFactory.GetPixelSize(toolStrip1, 22);
            RefreshModernWorkspaceIcons(modernMainCommandIconSize);
        }

        private void RefreshModernWorkspaceIcons(int iconSize)
        {
            SetModernCommandIcon(toolStripButton9, ModernVectorIconFactory.Glyph.Home, iconSize,
                toolStripButton9.Checked ? ModernUiTheme.Accent : ModernUiTheme.SecondaryText);
            SetModernCommandIcon(toolStripButton5, ModernVectorIconFactory.Glyph.Vision, iconSize,
                toolStripButton5.Checked ? ModernUiTheme.Accent : ModernUiTheme.SecondaryText);
            SetModernCommandIcon(toolStripButton1, ModernVectorIconFactory.Glyph.Motion, iconSize,
                toolStripButton1.Checked ? ModernUiTheme.Accent : ModernUiTheme.SecondaryText);
        }

        private void ConfigureModernVisionCommandIcons()
        {
            int primarySize = ModernVectorIconFactory.GetPixelSize(toolStrip2, 20);
            int menuSize = ModernVectorIconFactory.GetPixelSize(toolStrip2, 18);

            SetModernCommandIcon(toolStripButton11, ModernVectorIconFactory.Glyph.Play, primarySize, ModernUiTheme.Accent);
            SetModernCommandIcon(toolStripButton12, ModernVectorIconFactory.Glyph.Repeat, primarySize, ModernUiTheme.Accent);
            SetModernCommandIcon(toolStripButton13, ModernVectorIconFactory.Glyph.Save, primarySize, ModernUiTheme.Accent);
            SetModernCommandIcon(toolStripButton23, ModernVectorIconFactory.Glyph.OpenImage, primarySize, ModernUiTheme.Accent);

            // 批量运行菜单复用这些原按钮的业务事件，先替换源图标后再创建菜单代理，
            // 菜单项即可直接继承相同的高清图形。
            SetModernCommandIcon(toolStripButton35, ModernVectorIconFactory.Glyph.BatchRun, menuSize, ModernUiTheme.Accent);
            SetModernCommandIcon(toolStripButton16, ModernVectorIconFactory.Glyph.Repeat, menuSize, ModernUiTheme.Accent);
        }

        private static void ConfigureModernBatchGroupIcon(ToolStripDropDownButton batchButton, Control owner)
        {
            if (batchButton == null)
                return;

            int iconSize = ModernVectorIconFactory.GetPixelSize(owner, 18);
            SetModernCommandIcon(batchButton, ModernVectorIconFactory.Glyph.BatchRun, iconSize, ModernUiTheme.Accent);
            batchButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            batchButton.TextImageRelation = TextImageRelation.ImageBeforeText;
        }

        private static void SetModernCommandIcon(ToolStripItem item, ModernVectorIconFactory.Glyph glyph, int size, Color color)
        {
            if (item == null)
                return;

            Image image = ModernVectorIconFactory.Get(glyph, size, color);
            if (!ReferenceEquals(item.Image, image))
                item.Image = image;
            item.ImageScaling = ToolStripItemImageScaling.None;
            item.ImageTransparentColor = Color.Transparent;
        }
    }
}
