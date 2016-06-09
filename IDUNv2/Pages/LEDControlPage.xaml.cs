using System;
using IDUNv2.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using IDUNv2.ViewModels;
using Newtonsoft.Json;
using Windows.Storage;
using Windows.Storage.Streams;

namespace IDUNv2.Pages
{
    public class LedMatrix : IDisposable
    {
        private I2cDevice device;
        public byte[] buffer = new byte[1 + 192];
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        

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
            catch
            {

            }
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

        public unsafe void Grid(int rows, int cols, uint color)
        {
            int dx = (Width - 14) / cols;
            int dy = (Height - 14) / rows;
            int x = dx;
            int y = dy;
            for (int i = 0; i < cols - 1; ++i)
            {
                _vline(x + 0, 0, Height - 1, color);
                _vline(x + 1, 0, Height - 1, color);
                x += dx + 2;
            }
            for (int i = 0; i < rows - 1; ++i)
            {
                _hline(y + 0, 0, Width - 1, color);
                _hline(y + 1, 0, Width - 1, color);
                y += dy + 2;
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
        private LedMatrix ledMatrix;
        private bool[] ledStatus = new bool[8 * 8];
        private byte r5 = 31, g6 = 63, b5 = 31;
        Bitmap ledBitmap;
        private int px, py;
        public string SavedLEDImageName = string.Empty;

        public LEDControlPage()
        {
            this.InitializeComponent();
            this.Loaded += LEDControlPage_Loaded;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ledMatrix = new LedMatrix();
            ledMatrix.Init();
            ledImage.PointerMoved += LedImage_PointerMoved;
            ledImage.PointerPressed += LedImage_PointerPressed;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ledImage.PointerMoved -= LedImage_PointerMoved;
            ledImage.PointerPressed -= LedImage_PointerPressed;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            ledMatrix.Dispose();
        }

        private void LEDControlPage_Loaded(object sender, RoutedEventArgs e)
        {
            ledBitmap = new Bitmap((int)ledImage.Width, (int)ledImage.Height);
            ledImage.Source = ledBitmap.Sink;
        }

        private void SetLed(int px, int py)
        {
            int xpad = (px / 50) * 2;
            int ypad = (py / 50) * 2;
            int x = (px - xpad) / 48;
            int y = (py - ypad) / 48;
            int i = y * 8 + x;

            //Debug.WriteLine("set led: {0}, {1} [{2}]", x, y, i);
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
                for (int x=0; x < 8; x++)
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
            for (int i=0; i < ledMatrix.buffer.Length; i++)
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
        private void LedImage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var pt = e.GetCurrentPoint(sender as UIElement);
            px = (int)pt.Position.X;
            py = (int)pt.Position.Y;
            if (pt.IsInContact)
            {
                SetLed(px, py);
            }
            //Debug.WriteLine("pointer: {0}, {1}", px, py);
        }

        private void LedImage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pt = e.GetCurrentPoint(sender as UIElement);
            SetLed((int)pt.Position.X, (int)pt.Position.Y);
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            ledBitmap.Clear();
            ledBitmap.Grid(8, 8, 0x000000);

            int rx = (px / 50) * 50;
            int ry = (py / 50) * 50;

            ledBitmap.Rect(rx, ry, 48, 48, 0xCCCCCC);

            for (int i = 0; i < 64; ++i)
            {
                int x = (i & 7);
                int y = (i >> 3);
                int px = x * 48;
                int py = y * 48;
                if (ledStatus[i])
                {
                    if (x > 0) px += x * 2;
                    if (y > 0) py += y * 2;
                    ledBitmap.Rect(px, py, 48, 48, 0x006633);
                }
            }

            ledBitmap.Flush();
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

        private void UpdateColorPreview()
        {
            var colorPreview = FindName("colorPreview") as Rectangle;
            if (colorPreview != null)
            {
                byte r = (byte)((r5 * 255) >> 5);
                byte g = (byte)((g6 * 255) >> 6);
                byte b = (byte)((b5 * 255) >> 5);
                colorPreview.Fill = new SolidColorBrush(new Windows.UI.Color { R = r, G = g, B = b, A = 255 });
            }
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

        private void toSpeech_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SpeechSynthesisPage), null);
        }

        private  void SaveCurrent_Click(object sender, RoutedEventArgs e)
        {
            LEDImageNameTB.Text = string.Empty;
            LEDImageDescriptionTB.Text = string.Empty;
            SaveLEDToolTip.Visibility = Visibility.Visible;
        }

        public class SavedLEDImages
        {
            public string Name { get; set; }
            public StorageFile StorageFile { get; set; }
        }
        public class LEDImage
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public byte[] Buffer { get; set; }
        }

        private async void LoadCurrent_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFolder LEDFolder = await localFolder.GetFolderAsync("LEDImages");

            IReadOnlyList<StorageFile> LEDFiles = await LEDFolder.GetFilesAsync();

            List<string> LEDImagesList = new List<string>();

            foreach (StorageFile item in LEDFiles)
            {
                LEDImagesList.Add(item.Name);
            }

            LoadLedList.ItemsSource = LEDImagesList;

            LoadLEDToolTip.Visibility = Visibility.Visible;
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
                
                StreamReader r = new StreamReader(data.AsStream());

                string text = r.ReadToEnd();

                var Image = JsonConvert.DeserializeObject<LEDImage>(text);

                ledMatrix.LoadBuffer(Image.Buffer);

                UpdateLedStatus();

                LoadLEDToolTip.Visibility = Visibility.Collapsed;
            }
            catch
            {

            }
            
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

                var LED = new LEDImage { Name = LEDImageNameTB.Text, Description = LEDImageDescriptionTB.Text, Buffer = TempBuffer };

                string json = JsonConvert.SerializeObject(LED);

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                StorageFolder LEDFolder = await localFolder.CreateFolderAsync("LEDImages", CreationCollisionOption.OpenIfExists);

                var LEDFile = await LEDFolder.CreateFileAsync(LEDImageNameTB.Text, CreationCollisionOption.FailIfExists);

                await Windows.Storage.FileIO.WriteTextAsync(LEDFile, json);

                SaveLEDToolTip.Visibility = Visibility.Collapsed;

                LEDSaveSameNameWarning.Visibility = Visibility.Collapsed;

                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "LED Image Saved", "LED Image " + LED.Name + " saved.\nDescription: "+ LED.Description);

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
        }

        private async void DeleteLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                StorageFolder LEDFolder = await localFolder.GetFolderAsync("LEDImages");

                var file = await LEDFolder.GetFileAsync(SavedLEDImageName);

                await file.DeleteAsync();

                ShellPage.Current.AddNotificatoin(Models.NotificationType.Information, "LED Image Deleted", SavedLEDImageName);

                SavedLEDImageName = string.Empty;

                LoadLEDToolTip.Visibility = Visibility.Collapsed;
            }
            catch
            {

            }
            
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
    }
}
