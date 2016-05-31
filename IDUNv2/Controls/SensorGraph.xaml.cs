using System;
using System.Collections.Concurrent;
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

namespace IDUNv2.Controls
{
    public sealed partial class SensorGraph : UserControl
    {
        private const int N = 64;

        private float emitAt;

        private WriteableBitmap wb;
        private Stream wbStream;
        private byte[] wbPixels;

        //private int[] dataPoints = new int[N];
        //private int dataReadIdx;
        //private int dataWriteIdx;

        private List<float> dataPoints = new List<float>(N);

        private double leftPos;

        private bool firstRender = true;
        private double lastRenderTime;

        public SensorGraph()
        {
            this.InitializeComponent();

            leftPos = Width;
            emitAt = (float)Width;
            int w = (int)Width;
            int h = (int)Height;
            wb = new WriteableBitmap(w, h);
            wbStream = wb.PixelBuffer.AsStream();
            wbPixels = new byte[w * h * 4];
            image.Source = wb;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public void AddDataPoint(float y, double dx)
        {
            if (dataPoints.Count == N)
            {
                dataPoints.RemoveAt(0);
            }
            dataPoints.Add(y);
            leftPos -= dx;
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            var re = e as RenderingEventArgs;
            var curRenderTime = re.RenderingTime.TotalSeconds;

            if (firstRender)
            {
                firstRender = false;
                lastRenderTime = curRenderTime;
            }

            double dt = curRenderTime - lastRenderTime;
            lastRenderTime = curRenderTime;

            Clear();

            double xstep = 4.0;
            double x = 0.0;
            float halhf = wb.PixelHeight * 0.5f;
            float ystep = halhf / 140.0f; // -40 to +100
            float fh = wb.PixelHeight;
            float rfh = 1.0f / wb.PixelHeight;
            for (int i = 1; i < dataPoints.Count; ++i)
            {
                float y0 = dataPoints[i-1];
                float y1 = dataPoints[i];

                int py0 = (int)(halhf - y0 * ystep);
                int py1 = (int)(halhf - y1 * ystep);

                int x0 = (int)x;
                int x1 = (int)(x + xstep);
                DrawLine(x0, py0, x1, py1, 0xFF000000);
                x += xstep;
            }

            wbStream.Seek(0, SeekOrigin.Begin);
            wbStream.Write(wbPixels, 0, wbPixels.Length);
            wb.Invalidate();

            //leftPos -= 30.0 * dt;
        }

        #region Drawing
        private unsafe void Clear()
        {
            fixed (byte* pixels = &wbPixels[0])
            {
                int n = wbPixels.Length / 4;
                uint* p = (uint*)pixels;
                for (int i = 0; i < n; ++i)
                {
                    p[i] = 0;
                }
            }
        }

        private static int Clamp(int v, int min, int max)
        {
            return Math.Min(Math.Max(v, min), max);
        }

        private unsafe void DrawLine(int x0, int y0, int x1, int y1, uint color)
        {
            int width = wb.PixelWidth;
            int height = wb.PixelHeight;

            x0 = Clamp(x0, 0, width - 1);
            x1 = Clamp(x1, 0, width - 1);
            y0 = Clamp(y0, 0, height - 1);
            y1 = Clamp(y1, 0, height - 1);

            int dx = x1 - x0;
            int dy = y1 - y0;
            int e0 = dx > 0 ? 1 : -1;
            int e1 = e0;
            int step0 = dy > 0 ? width : -width;
            int step1 = 0;
            int i = dx > 0 ? dx : -dx;
            int j = dy > 0 ? dy : -dy;
            int d, n;

            if (j >= i)
            {
                e1 = 0;
                step1 = step0;
                d = i;
                i = j;
                j = d;
            }
            d = i / 2;
            step0 += e0;
            step1 += e1;
            n = i;

            fixed (byte* pixels = &wbPixels[0])
            {
                uint* p = (uint*)pixels + y0 * width + x0;
                do
                {
                    *p = color;
                    d += j;
                    if (d >= i)
                    {
                        d -= i;
                        p += step0;
                    }
                    else
                    {
                        p += step1;
                    }
                } while (n-- > 0);
            }
        }
        #endregion
    }
}
