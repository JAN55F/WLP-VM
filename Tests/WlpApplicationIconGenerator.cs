using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

internal static class WlpApplicationIconGenerator
{
    private static readonly int[] Sizes = { 16, 20, 24, 32, 40, 48, 64, 128, 256 };

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: WlpApplicationIconGenerator <output.ico>");
            return 2;
        }

        List<byte[]> images = new List<byte[]>();
        foreach (int size in Sizes)
            images.Add(RenderPng(size));

        using (FileStream stream = File.Create(args[0]))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)images.Count);

            int offset = 6 + (16 * images.Count);
            for (int index = 0; index < images.Count; index++)
            {
                int size = Sizes[index];
                writer.Write((byte)(size == 256 ? 0 : size));
                writer.Write((byte)(size == 256 ? 0 : size));
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((ushort)1);
                writer.Write((ushort)32);
                writer.Write(images[index].Length);
                writer.Write(offset);
                offset += images[index].Length;
            }

            foreach (byte[] image in images)
                writer.Write(image);
        }

        return 0;
    }

    private static byte[] RenderPng(int size)
    {
        using (Bitmap bitmap = new Bitmap(size, size, PixelFormat.Format32bppPArgb))
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            float inset = Math.Max(0.75F, size * 0.025F);
            RectangleF bounds = new RectangleF(inset, inset, size - (2F * inset), size - (2F * inset));
            float radius = size * 0.22F;
            using (GraphicsPath path = RoundedRectangle(bounds, radius))
            using (LinearGradientBrush fill = new LinearGradientBrush(
                bounds, Color.FromArgb(91, 171, 229), Color.FromArgb(44, 120, 184), 90F))
            using (Pen highlight = new Pen(Color.FromArgb(105, 255, 255, 253), Math.Max(0.7F, size * 0.012F)))
            {
                graphics.FillPath(fill, path);
                graphics.DrawPath(highlight, path);
            }

            string text = size < 24 ? "W" : "WLP";
            float emSize = size < 24 ? size * 0.55F : size * 0.285F;
            using (Font font = new Font("Segoe UI", emSize, FontStyle.Bold, GraphicsUnit.Pixel))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 253)))
            using (StringFormat format = new StringFormat(StringFormat.GenericTypographic))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.FormatFlags |= StringFormatFlags.NoWrap;
                RectangleF textBounds = new RectangleF(0F, -size * 0.025F, size, size);
                graphics.DrawString(text, font, brush, textBounds, format);
            }

            using (MemoryStream png = new MemoryStream())
            {
                bitmap.Save(png, ImageFormat.Png);
                return png.ToArray();
            }
        }
    }

    private static GraphicsPath RoundedRectangle(RectangleF bounds, float radius)
    {
        GraphicsPath path = new GraphicsPath();
        float diameter = Math.Min(radius * 2F, Math.Min(bounds.Width, bounds.Height));
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
