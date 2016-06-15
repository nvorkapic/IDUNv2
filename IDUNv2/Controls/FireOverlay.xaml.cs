using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IDUNv2.Controls
{
    public sealed partial class FireOverlay : UserControl
    {
        private static class ColorHelper
        {
            static public IEnumerable<int> Gradient(int c0, int c1, int steps)
            {
                int r0 = (c0 >> 16) & 0xFF;
                int g0 = (c0 >> 8) & 0xFF;
                int b0 = c0 & 0xFF;
                int r1 = (c1 >> 16) & 0xFF;
                int g1 = (c1 >> 8) & 0xFF;
                int b1 = c1 & 0xFF;
                int stepR = ((r1 - r0) << 16) / steps;
                int stepG = ((g1 - g0) << 16) / steps;
                int stepB = ((b1 - b0) << 16) / steps;
                int r = r0 << 16;
                int g = g0 << 16;
                int b = b0 << 16;

                for (int i = 0; i < steps; ++i)
                {
                    int c = ((r >> 16) << 16) | ((g >> 16) << 8) | (b >> 16);
                    r += stepR;
                    g += stepG;
                    b += stepB;
                    yield return c;
                }
            }
        }

        private class Surface : IDisposable
        {
            private WriteableBitmap wb;
            private Stream stream;

            public int Width { get { return wb.PixelWidth; } }
            public int Height { get { return wb.PixelHeight; } }
            public int Pitch { get; private set; }
            public byte[] Pixels { get; private set; }
            public WriteableBitmap Sink { get { return wb; } }

            public Surface(int width, int height)
            {
                wb = new WriteableBitmap(width, height);
                stream = wb.PixelBuffer.AsStream();
                Pitch = width * 4;
                Pixels = new byte[Pitch * height];
            }

            public void Flush()
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.Write(Pixels, 0, Pixels.Length);
                wb.Invalidate();
            }

            public void Dispose()
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        private class Fire
        {
            private int w;
            private int h;
            private byte[] buf;
            private int[] pal;
            private Random rnd;

            public int palStart = 0;
            public int numPoints;

            public Fire(int w, int h)
            {
                this.w = w;
                this.h = h;
                buf = new byte[w * h];
                pal = new int[256 * 2];
                rnd = new Random();

                int i = 0;

                foreach (var c in ColorHelper.Gradient(0x000000, 0xFF0000, 256 / 4)) pal[i++] = c;
                foreach (var c in ColorHelper.Gradient(0xFF0000, 0xFFFF00, 256 / 4)) pal[i++] = c;
                foreach (var c in ColorHelper.Gradient(0xFFFF00, 0xFFFFFF, 256 / 4)) pal[i++] = c;
                while (i < 256 * 1) pal[i++] = 0xFFFFFF;

                foreach (var c in ColorHelper.Gradient(0x000000, 0x0000FF, 256 / 4)) pal[i++] = c;
                foreach (var c in ColorHelper.Gradient(0x0000FF, 0x00FFFF, 256 / 4)) pal[i++] = c;
                foreach (var c in ColorHelper.Gradient(0x00FFFF, 0xFFFFFF, 256 / 4)) pal[i++] = c;
                while (i < 256 * 2) pal[i++] = 0xFFFFFF;

                numPoints = 10;
            }

            public void Seed()
            {
                int row = (h - 1) * w;
                int end = row + w;
                for (int i = row; i < end; ++i)
                {
                    buf[i] = (byte)rnd.Next(0, 128);
                    //buf[i] = (byte)(buf[i] * 512 / 513);
                }
            }

            public void SeedRect(int x, int y, int w, int h)
            {
                for (int i = 0; i < w; ++i)
                {
                    int py = y + i;
                    if (py < 0 || py >= this.h)
                        continue;
                    for (int j = 0; j < h; ++j)
                    {
                        int px = x + j;
                        if (px > 0 && px < this.w)
                            buf[py * this.w + px] = (byte)rnd.Next(128, 256);
                    }
                }
            }

            public void SeedAt(int x, int y, int min = 128, int max = 256)
            {
                buf[y * this.w + x] = (byte)rnd.Next(min, max);
            }

            //public void SeedPoints(long t)
            //{
            //    double aspect = this.w / (double)this.h;
            //    double a = t * Display.invFreq;

            //    int cx = this.w / 2;
            //    int cy = this.h / 2 + 150;
            //    //int cx = this.w / 2 + (int)(Math.Sin(a) * 50.0*aspect);
            //    //int cy = this.w / 2 + (int)(Math.Sin(a) * 50.0);
            //    //double r = 75.0;
            //    double r = (Math.Abs(Math.Sin(a * 0.5)) + 1.0) * 100.0;
            //    const double pi2Inv = 1.0 / (2.0 * Math.PI);
            //    for (int i = 0; i < numPoints; ++i)
            //    {
            //        double ca = Math.Cos((i + a * 2.0) * pi2Inv) * Math.Sin(i * 0.01 + a * 0.5) * Math.Cos(Math.Sqrt(r * r + i * i) * 0.1 + a);
            //        double sa = Math.Sin((i + a * 2.0) * pi2Inv) * Math.Cos(Math.Cos(a + i * 2) * 0.1 + i * 0.5 + a * 2.0);
            //        double fx = ca * r * aspect;
            //        double fy = sa * r;

            //        int px = cx + (int)fx;
            //        int py = cy + (int)fy;
            //        SeedRect(px - 2, py - 2, 4, 4);
            //    }

            //}

            static private byte Sample(int a, int b, int c, int d)
            {
                int v = ((a + b + c + d) * 128 - 5) >> 9;
                if (v < 0)
                    return 0;
                else
                    return (byte)v;
            }

            public unsafe void Burn()
            {
                fixed (byte* p0 = &buf[0])
                {
                    byte* p = p0;
                    int wlast = w - 1;
                    for (int y = 0; y < h - 1; ++y)
                    {
                        byte* p1 = p + w;
                        byte* p2 = (y == h - 2) ? p0 : p1 + w;

                        p[0] = Sample(p1[wlast], p1[0], p1[1], p2[0]);

                        for (int x = 1; x < w - 1; ++x)
                            p[x] = Sample(p1[x - 1], p1[x], p1[x + 1], p2[x]);

                        p[wlast] = Sample(p1[wlast - 1], p1[wlast], p1[0], p2[wlast]);

                        p += w;
                    }
                }
            }

            public unsafe void Draw(Surface s)
            {
                int sz = w * h;
                fixed (byte* p8 = &s.Pixels[0])
                {
                    int w = s.Width;
                    int* p = (int*)p8;
                    for (int i = 0; i < sz; ++i)
                    {
                        int c = pal[palStart + buf[i]];
                        if (c > 0)
                            p[i] = c;
                    }
                }
            }
        }

        private Fire fire;
        private Surface imageSurface;

        public FireOverlay()
        {
            this.InitializeComponent();
            int w = (int)this.Width;
            int h = (int)this.Height;

            imageSurface = new Surface(w, h);
            image.Source = imageSurface.Sink;
            fire = new Fire(w, h);
        }

        public void Draw()
        {
            fire.Seed();
            fire.Burn();
            fire.Draw(imageSurface);
            imageSurface.Flush();
        }
    }
}
