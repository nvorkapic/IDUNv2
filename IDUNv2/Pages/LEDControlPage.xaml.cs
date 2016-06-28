using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.Storage;
using IDUNv2.Models;
using IDUNv2.DataAccess;

namespace IDUNv2.Pages
{
    public class LedMatrix : IDisposable
    {
        #region Fields

        private I2cDevice device;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public byte[] buffer = new byte[1 + 192];

        #endregion

        private async Task<I2cDevice> GetDeviceAsync()
        {
            var aqs = I2cDevice.GetDeviceSelector();
            var infos = await DeviceInformation.FindAllAsync(aqs);
            var settings = new I2cConnectionSettings(0x46)
            {
                BusSpeed = I2cBusSpeed.StandardMode
            };
            return await I2cDevice.FromIdAsync(infos[0].Id, settings);
        }

        public void Init()
        {
            try
            {
                Task.Run(async () =>
                {
                    device = await GetDeviceAsync().ConfigureAwait(false);
                }).Wait(5000);
            }
            catch (Exception)
            {

            }
            var data = new byte[1 + 192];
            device?.Write(data);
        }

        public void LoadBuffer(byte[] buff)
        {
            try
            {
                buffer = buff;
                Flush();
            }
            catch { } // ignore
        }

        public void SaveBuffer()
        {
            localSettings.Values["LEDBuffer"] = buffer;
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            int i = 1 + y * 24 + x;
            buffer[i + 0] = r;
            buffer[i + 8] = g;
            buffer[i + 16] = b;
        }

        public void Flush()
        {
            device?.Write(buffer);
        }

        public void Dispose()
        {
            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }
    }

    #region CustomDrawing

    public class Bitmap : IDisposable
    {
        private WriteableBitmap wb;
        private Stream stream;

        public int Width { get { return wb.PixelWidth; } }
        public int Height { get { return wb.PixelHeight; } }
        public int Pitch { get; private set; }
        public byte[] Pixels { get; private set; }
        public WriteableBitmap Sink { get { return wb; } }

        public Bitmap(int width, int height)
        {
            wb = new WriteableBitmap(width, height);
            stream = wb.PixelBuffer.AsStream();
            Pitch = width * 4;
            Pixels = new byte[Pitch * height];
        }

        public void Clear()
        {
            int n = Pixels.Length;
            for (int i = 0; i < n; ++i)
                Pixels[i] = 0x80;
        }

        public unsafe void Rect(int x, int y, int w, int h, uint color)
        {
            int xx = x + w;
            int yy = y + h;

            x = Math.Min(Math.Max(x, 0), Width);
            y = Math.Min(Math.Max(y, 0), Height);
            xx = Math.Min(Math.Max(xx, 0), Width);
            yy = Math.Min(Math.Max(yy, 0), Height);

            w = xx - x;
            h = yy - y;

            if (w <= 0 || h <= 0)
                return;

            int offs = y * Pitch + x * 4;
            for (int i = 0; i < h; ++i, offs += Pitch)
            {
                fixed (byte* p8 = &Pixels[offs])
                {
                    uint* p = (uint*)p8;
                    for (int j = 0; j < w; ++j)
                    {
                        p[j] = color;
                    }
                }
            }
        }

        private unsafe void _vline(int x, int y0, int y1, uint color)
        {
            fixed (byte* p8 = &Pixels[y0 * Pitch + x * 4])
            {
                uint* p = (uint*)p8;
                for (int y = y0; y <= y1; ++y)
                {
                    *p = color;
                    p += Width;
                }
            }
        }

        private unsafe void _hline(int y, int x0, int x1, uint color)
        {
            fixed (byte* p8 = &Pixels[y * Pitch + x0 * 4])
            {
                uint* p = (uint*)p8;
                for (int x = x0; x <= x1; ++x)
                {
                    p[x] = color;
                }
            }
        }

        public unsafe void Grid(int rows, int cols, int pad, uint color)
        {
            int dx = (Width - (cols - 1) * pad) / cols;
            int dy = (Height - (rows - 1) * pad) / rows;
            int x = dx;
            int y = dy;
            dx += pad;
            dy += pad;
            for (int i = 0; i < cols - 1; ++i)
            {
                _vline(x + 0, 0, Height - 1, color);
                _vline(x + 1, 0, Height - 1, color);
                x += dx;
            }
            for (int i = 0; i < rows - 1; ++i)
            {
                _hline(y + 0, 0, Width - 1, color);
                _hline(y + 1, 0, Width - 1, color);
                y += dy;
            }
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

    #endregion

    public sealed partial class LEDControlPage : Page
    {
        private class SavedLEDImages
        {
            public string Name { get; set; }
            public StorageFile StorageFile { get; set; }
        }

        private class LEDImage
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public byte[] Buffer { get; set; }
        }

        const int LedSize = 40;
        const int LedPad = 2;
        const int LedSizeNoPad = LedSize - LedPad;

        #region Fields

        private LedMatrix ledMatrix;
        private bool[] ledStatus = new bool[8 * 8];
        private byte r5 = 31, g6 = 63, b5 = 31;
        private Bitmap ledBitmap;
        private int px, py;
        public string SavedLEDImageName = "";

        #endregion

        #region CmdBar Actions

        private void ClearLED(object param)
        {
            EmptyBuffer();
            ClearLEDStatus();
        }

        private async void LoadLED(object param)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder LEDFolder = await localFolder.CreateFolderAsync("LEDImages", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFile> LEDFiles = await LEDFolder.GetFilesAsync();
            var LEDImagesList = new List<string>();

            foreach (StorageFile item in LEDFiles)
            {
                LEDImagesList.Add(item.Name);
            }

            LoadLedList.ItemsSource = LEDImagesList;
            LoadLEDToolTip.Visibility = Visibility.Visible;
        }

        private void SaveLED(object param)
        {
            LEDImageNameTB.Text = string.Empty;
            LEDImageDescriptionTB.Text = string.Empty;
            SaveLEDToolTip.Visibility = Visibility.Visible;
        }

        private void NavigateToSpeech(object param)
        {
            Frame.Navigate(typeof(SpeechSynthesisPage), null);
        }

        #endregion

        #region Constructors

        public LEDControlPage()
        {
            this.InitializeComponent();
            this.Loaded += LEDControlPage_Loaded;
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ledMatrix = new LedMatrix();
            ledMatrix.Init();

            ledImage.PointerMoved += LedImage_PointerMoved;
            ledImage.PointerPressed += LedImage_PointerPressed;
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            DAL.SetCmdBarItems(new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Save, "Save", SaveLED),
                new CmdBarItem(Symbol.Clear, "Clear", ClearLED),
                new CmdBarItem(Symbol.OpenFile, "Load", LoadLED),
                new CmdBarItem(Symbol.Microphone, "Speech",NavigateToSpeech),
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ledImage.PointerMoved -= LedImage_PointerMoved;
            ledImage.PointerPressed -= LedImage_PointerPressed;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            ledMatrix.Dispose();
        }

        #region LED Manipulation

        private void SetLed(int px, int py)
        {
            int xpad = (px / LedSize) * LedPad;
            int ypad = (py / LedSize) * LedPad;
            int x = (px - xpad) / (LedSize - LedPad);
            int y = (py - ypad) / (LedSize - LedPad);
            int i = y * 8 + x;

            ledStatus[i] = fillToggle.IsChecked.Value;
            if (ledStatus[i])
            {
                ledMatrix.SetPixel(x, y, r5, g6, b5);
            }
            else
            {
                ledMatrix.SetPixel(x, y, 0, 0, 0);
            }
            ledMatrix.Flush();
        }

        private void UpdateLedStatus()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int i = 1 + y * 24 + x;
                    var q1 = ledMatrix.buffer[i + 0];
                    var q2 = ledMatrix.buffer[i + 8];
                    var q3 = ledMatrix.buffer[i + 16];

                    if (q1 != 0 || q2 != 0 || q3 != 0)
                    {
                        ledStatus[y * 8 + x] = true;
                    }
                }
            }
        }

        private void EmptyBuffer()
        {
            for (int i = 0; i < ledMatrix.buffer.Length; i++)
            {
                ledMatrix.buffer[i] = 0;
            }
        }

        private void ClearLEDStatus()
        {
            int x = 0;
            foreach (var i in ledStatus)
            {
                ledStatus[x] = false;
                x++;
            }
        }

        private unsafe void Draw()
        {
            int w = ledBitmap.Width;
            int h = ledBitmap.Height;
            int pitch = ledBitmap.Pitch;
            for (int y = 0; y < h; ++y)
            {
                fixed (byte* bp = &ledBitmap.Pixels[y * pitch])
                {
                    uint* p = (uint*)bp;
                    for (int x = 0; x < w; ++x)
                    {
                        uint c = (uint)((x * 255) / w);
                        p[x] = c;
                    }
                }
            }

            ledBitmap.Flush();
        }

        #endregion

        private void UpdateColorPreview()
        {
            if (colorPreview != null)
            {
                byte r = (byte)((r5 * 255) >> 5);
                byte g = (byte)((g6 * 255) >> 6);
                byte b = (byte)((b5 * 255) >> 5);
                colorPreview.Fill = new SolidColorBrush(new Windows.UI.Color { R = r, G = g, B = b, A = 255 });
            }
        }

        #region Event Handlers

        private void LEDControlPage_Loaded(object sender, RoutedEventArgs e)
        {
            ledBitmap = new Bitmap((int)ledImage.Width, (int)ledImage.Height);
            ledImage.Source = ledBitmap.Sink;
        }

        private void LedImage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var pt = e.GetCurrentPoint(sender as UIElement);
            px = (int)pt.Position.X;
            py = (int)pt.Position.Y;
            if (pt.IsInContact)
            {
                SetLed(px, py);
            }
        }

        private void LedImage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pt = e.GetCurrentPoint(sender as UIElement);
            SetLed((int)pt.Position.X, (int)pt.Position.Y);
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            ledBitmap.Clear();
            ledBitmap.Grid(8, 8, LedPad, 0x000000);

            int rx = (px / LedSize) * LedSize;
            int ry = (py / LedSize) * LedSize;

            ledBitmap.Rect(rx, ry, LedSizeNoPad, LedSizeNoPad, 0xCCCCCC);

            for (int i = 0; i < 64; ++i)
            {
                int x = (i & 7);
                int y = (i >> 3);
                int px = x * LedSizeNoPad;
                int py = y * LedSizeNoPad;
                if (ledStatus[i])
                {
                    if (x > 0) px += x * LedPad;
                    if (y > 0) py += y * LedPad;
                    ledBitmap.Rect(px, py, LedSizeNoPad, LedSizeNoPad, 0x006633);
                }
            }

            ledBitmap.Flush();
        }

        private void unCheckfill(object sender, RoutedEventArgs e)
        {
            fillToggle.Background = new SolidColorBrush(new Windows.UI.Color { R = 104, G = 33, B = 122, A = 255 });
            fillToggle.Content = "Remove Fill";
        }

        private void fillToggle_Checked(object sender, RoutedEventArgs e)
        {
            fillToggle.Background = new SolidColorBrush(new Windows.UI.Color { R = 0, G = 0, B = 0, A = 255 });
            fillToggle.Content = "Fill";
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            fillToggle.Background = new SolidColorBrush(new Windows.UI.Color { R = 0, G = 0, B = 0, A = 255 });
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmptyBuffer();
                ClearLEDStatus();

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder LEDFolder = await localFolder.GetFolderAsync("LEDImages");
                var file = await LEDFolder.GetFileAsync(SavedLEDImageName);
                var data = await file.OpenReadAsync();
                using (var r = new StreamReader(data.AsStream()))
                {
                    string text = r.ReadToEnd();
                    var Image = JsonConvert.DeserializeObject<LEDImage>(text);
                    ledMatrix.LoadBuffer(Image.Buffer);
                    UpdateLedStatus();
                    LoadLEDToolTip.Visibility = Visibility.Collapsed;
                }
            }
            catch { } // ignore
        }

        private void LoadCancel_Click(object sender, RoutedEventArgs e)
        {
            LoadLEDToolTip.Visibility = Visibility.Collapsed;
        }

        private void LEDImageNameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            osk.SetTarget(tb);
            osk.Visibility = Visibility.Visible;
        }

        private void LEDImageNameTB_LostFocus(object sender, RoutedEventArgs e)
        {
            osk.Visibility = Visibility.Collapsed;
        }

        private void LEDImageDescriptionTB_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            osk.SetTarget(tb);
            osk.Visibility = Visibility.Visible;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] TempBuffer = new byte[ledMatrix.buffer.Length];
                Array.Copy(ledMatrix.buffer, TempBuffer, ledMatrix.buffer.Length);
                var LED = new LEDImage
                {
                    Name = LEDImageNameTB.Text,
                    Description = LEDImageDescriptionTB.Text,
                    Buffer = TempBuffer
                };
                string json = JsonConvert.SerializeObject(LED);
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder LEDFolder = await localFolder.CreateFolderAsync("LEDImages", CreationCollisionOption.OpenIfExists);
                var LEDFile = await LEDFolder.CreateFileAsync(LEDImageNameTB.Text, CreationCollisionOption.FailIfExists);
                await FileIO.WriteTextAsync(LEDFile, json);
                SaveLEDToolTip.Visibility = Visibility.Collapsed;
                LEDSaveSameNameWarning.Visibility = Visibility.Collapsed;
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Information,
                    "LED Image Saved",
                    "LED Image " +
                    LED.Name +
                    "saved.\nDescription: " +
                    LED.Description);
                ClearLEDStatus();
                EmptyBuffer();
            }
            catch
            {
                LEDSaveSameNameWarning.Visibility = Visibility.Visible;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            LEDImageNameTB.Text = string.Empty;
            LEDImageDescriptionTB.Text = string.Empty;
            SaveLEDToolTip.Visibility = Visibility.Collapsed;
            LEDSaveSameNameWarning.Visibility = Visibility.Collapsed;
        }

        private void LoadLedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ((ListView)sender).SelectedItem;
            SavedLEDImageName = (string)item;
        }

        private void ClearLED_Click(object sender, RoutedEventArgs e)
        {
            EmptyBuffer();
            ClearLEDStatus();
            ledBitmap.Flush();
        }

        private async void DeleteLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder LEDFolder = await localFolder.GetFolderAsync("LEDImages");
                var file = await LEDFolder.GetFileAsync(SavedLEDImageName);
                await file.DeleteAsync();
                ShellPage.Current.AddNotificatoin(
                    NotificationType.Information,
                    "LED Image Deleted",
                    SavedLEDImageName);
                SavedLEDImageName = "";
                LoadLEDToolTip.Visibility = Visibility.Collapsed;
            }
            catch { } // ignore
        }

        private void sliderRed_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            r5 = (byte)e.NewValue;
            UpdateColorPreview();
        }

        private void sliderGreen_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            g6 = (byte)e.NewValue;
            UpdateColorPreview();
        }

        private void sliderBlue_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            b5 = (byte)e.NewValue;
            UpdateColorPreview();
        }

        #endregion
    }
}
