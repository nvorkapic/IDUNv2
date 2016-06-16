using IDUNv2.Common;
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
        private class ViewModel : NotifyBase
        {
            public class ScaleLabel
            {
                public string Text { get; set; }
                public float Ty { get; set; }
            }

            private List<ScaleLabel> _labels;

            public List<ScaleLabel> Labels
            {
                get { return _labels; }
                set { _labels = value; Notify(); }
            }

            public List<int> LabelYs { get; private set; }

            public void SetLabels(float min, float max, float height, float fontSize, string format = "F0")
            {
                float stepSize = 10.0f;
                float range = max - min;
                float rangeStep = range / stepSize;
                float yStep = (height / stepSize);
                float yAdjust = (fontSize + 5.0f) * 0.5f;
                var labels = new List<ScaleLabel>((int)stepSize);
                LabelYs = new List<int>((int)stepSize);
                for (float v = max, y = -yAdjust; v >= min; v -= rangeStep, y += yStep)
                {
                    string text = v.ToString(format);
                    float top = y;
                    labels.Add(new ScaleLabel { Text = text, Ty = top });
                    LabelYs.Add((int)(y + yAdjust));
                }

                Labels = labels;
            }
        }

        #region Fields

        private WriteableBitmap wb;
        private Stream wbStream;
        private byte[] wbPixels;

        private int dataPointsCap;
        private List<float> dataPoints;

        private float rangeMin;
        private float rangeMax;
        private float dangerLo;
        private float dangerHi;
        private float range;
        private float rangeStep;
        private int centerY;
        private int dangerLoY;
        private int dangerHiY;

        private ViewModel viewModel = new ViewModel();

        #endregion

        #region Properties

        public uint ColorDangerLo { get; set; }
        public uint ColorDangerHi { get; set; }
        public uint ColorScaleLines { get; set; }
        public uint ColorDataLines { get; set; }

        #endregion

        public SensorGraph()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;

            int w = (int)Width;
            int h = (int)Height;
            wb = new WriteableBitmap(w, h);
            wbStream = wb.PixelBuffer.AsStream();
            wbPixels = new byte[w * h * 4];
            image.Source = wb;

            centerY = h >> 1;

            dataPointsCap = (int)(Width * 0.45);
            dataPoints = new List<float>(dataPointsCap);

            ColorDangerLo = 0xFFFF0000;
            ColorDangerHi = 0xFFFF0000;
            ColorScaleLines = 0xFF808080;
            ColorDataLines = 0xFFFFFF00;
        }

        #region Clamp

        private static int Clamp(int v, int min, int max)
        {
            return Math.Min(Math.Max(v, min), max);
        }

        private static float Clamp(float v, float min, float max)
        {
            return Math.Min(Math.Max(v, min), max);
        }

        #endregion

        public void SetRange(float min, float max)
        {
            if (min >= max)
            {
                throw new Exception("min must be < max");
            }

            rangeMin = min;
            rangeMax = max;
            range = max - min;
            rangeStep = wb.PixelHeight / range;
            viewModel.SetLabels(min, max, wb.PixelHeight, (float)FontSize);
        }

        public void SetDanger(float lo, float hi)
        {
            lo = Clamp(lo, rangeMin, rangeMax);
            hi = Clamp(hi, rangeMin, rangeMax);
            dangerLo = lo;
            dangerHi = hi;
            dangerLoY = wb.PixelHeight - (int)((dangerLo - rangeMin) * rangeStep);
            dangerHiY = (int)((rangeMax - dangerHi) * rangeStep);
        }

        public void AddDataPoint(float y)
        {
            if (dataPoints.Count == dataPointsCap)
            {
                dataPoints.RemoveAt(0);
            }
            dataPoints.Add(y);
        }

        private void DrawScaleLines()
        {
            int xmax = wb.PixelWidth - 1;
            int lh = wb.PixelHeight - dangerLoY - 1;
            int hh = wb.PixelHeight - dangerHiY - 1;
            FillRectFast(0, dangerLoY + 1, wb.PixelWidth - 1, lh, (ColorDangerLo & 0x303030));
            FillRectFast(0, 0, wb.PixelWidth - 1, dangerHiY, (ColorDangerHi & 0x303030));

            uint scaleColor = ColorScaleLines;
            foreach (var y in viewModel.LabelYs)
            {
                hLine(y, 0, xmax, scaleColor);
            }

            hLine(dangerLoY - 1, 0, xmax, ColorDangerLo);
            hLine(dangerLoY, 0, xmax, ColorDangerLo);
            hLine(dangerLoY + 1, 0, xmax, ColorDangerLo);

            hLine(dangerHiY - 1, 0, xmax, ColorDangerHi);
            hLine(dangerHiY, 0, xmax, ColorDangerHi);
            hLine(dangerHiY + 1, 0, xmax, ColorDangerHi);
        }

        private void DrawDataLines()
        {
            int centerY = wb.PixelHeight >> 1;
            int x = 0;
            int dx = 2;
            int h = wb.PixelHeight;
            uint color = ColorDataLines;
            for (int i = 1; i < dataPoints.Count; ++i)
            {
                float y0 = dataPoints[i - 1];
                float y1 = dataPoints[i];
                int py0 = h - (int)((y0 - rangeMin) * rangeStep);
                int py1 = h - (int)((y1 - rangeMin) * rangeStep);
                DrawFatLine(x, py0, x + dx, py1, color);
                x += dx;
            }
        }

        public void Render()
        {
            Clear();

            DrawScaleLines();
            DrawDataLines();

            wbStream.Seek(0, SeekOrigin.Begin);
            wbStream.Write(wbPixels, 0, wbPixels.Length);
            wb.Invalidate();
        }

        #region Custom Drawing

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

        private unsafe void FillRectFast(int x, int y, int w, int h, uint color)
        {
            fixed (byte* p8 = &wbPixels[0])
            {
                uint* p = (uint*)p8 + y * w + x;
                while (h-- > 0)
                {
                    for (int i = 0; i < w; ++i)
                        p[i] = color;
                    p += w;
                }
            }
        }

        private unsafe void DrawFatLine(int x0, int y0, int x1, int y1, uint color)
        {
            int width = wb.PixelWidth;
            int height = wb.PixelHeight;

            x0 = Clamp(x0, 3, width - 4);
            x1 = Clamp(x1, 3, width - 4);
            y0 = Clamp(y0, 3, height - 4);
            y1 = Clamp(y1, 3, height - 4);

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

            int step0_2x = 2 * step0;
            int step1_2x = 2 * step1;

            fixed (byte* pixels = &wbPixels[0])
            {
                uint* p = (uint*)pixels + y0 * width + x0;
                do
                {
                    //*p = color;
                    p[-1] = color;
                    p[0] = color;
                    p[1] = color;
                    p[step0] = color;
                    p[step1] = color;
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

        private unsafe void hLine(int y, int x0, int x1, uint color)
        {
            y = Clamp(y, 0, wb.PixelHeight - 1);
            x0 = Clamp(x0, 0, wb.PixelWidth - 1);
            x1 = Clamp(x1, 0, wb.PixelWidth - 1);

            int n = x1 - x0 + 1;
            fixed (byte* p8 = &wbPixels[0])
            {
                uint* p = (uint*)p8 + y * wb.PixelWidth + x0;
                for (int i = 0; i < n; ++i)
                {
                    p[i] = color;
                }
            }
        }

        #endregion
    }
}
