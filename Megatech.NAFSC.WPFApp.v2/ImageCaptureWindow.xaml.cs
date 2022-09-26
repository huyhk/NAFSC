using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for ImageCaptureWindow.xaml
    /// </summary>
    public partial class ImageCaptureWindow : Window
    {
        MediaCapture _mediaCapture = new MediaCapture();
        CaptureElement _captureElement;
        public ImageCaptureWindow()
        {
            InitializeComponent();
            _captureElement = new CaptureElement
            {
                Stretch = Windows.UI.Xaml.Media.Stretch.Uniform
            };
            _captureElement.Loaded += _captureElement_Loaded;
            _captureElement.Unloaded += _captureElement_Unloaded;

            preview.Child = _captureElement;
        }

        private async void _captureElement_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await _mediaCapture.StopPreviewAsync();

        }

        private  void _captureElement_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StartPreview();
        }

        private async void StartPreview()
        {
            if (!_initialized)
            {
                var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                // Fall back to the local app storage if the Pictures Library is not available
                _captureFolder = picturesLibrary.SaveFolder ?? ApplicationData.Current.LocalFolder;

                // Get available devices for capturing pictures
                var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

               

                if (allVideoDevices.Count > 0)
                {
                    // try to find back camera
                    DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);

                    // If there is no device mounted on the back panel, return the first device found
                    var device = desiredDevice ?? allVideoDevices.FirstOrDefault();
                    
                    await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings() { VideoDeviceId = device.Id });
                    var streamProps = _mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo);
                    int maxWidth = 0;
                    int selected = 0;
                    for (int i = 0; i < streamProps.Count; i++)
                    {

                        var item = streamProps[i];
                        if ((item as VideoEncodingProperties).Width > maxWidth && (item as VideoEncodingProperties).Width < 1024)
                        {
                            maxWidth = (int)(item as VideoEncodingProperties).Width;
                            selected = i;
                        }
                    }
                    await _mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, streamProps[selected]);

                    _captureElement.Source = _mediaCapture;



                    _initialized = true;
                }
            }

            if (_initialized)
            {
                previewing = true;
                await _mediaCapture.StartPreviewAsync();
                preview.Visibility = Visibility.Visible;
                viewer.Visibility = Visibility.Collapsed;
            }
        }
        private async void StopPreview()
        {
          

            if (_initialized && previewing)
            {
                previewing = false;
                await _mediaCapture.StopPreviewAsync();
            }
        }
        public ImageCaptureWindow(string number) : this()
        {
            _number = number;

            Title = FindResource("capture_image_invoice").ToString() + " " + _number;
        }
        private string _number;

        public string ImagePath { get; set; }
        private string _imagePath = null;
        private bool _initialized;
        private StorageFolder _captureFolder;
        private bool previewing;

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (previewing)
            {
                StopPreview();
                using (var stream = new InMemoryRandomAccessStream())
                {

                    await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

                    try
                    {
                        var file = await _captureFolder.CreateFileAsync(_number + ".jpg", CreationCollisionOption.GenerateUniqueName);


                        using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await RandomAccessStream.CopyAndCloseAsync(stream.GetInputStreamAt(0), outputStream.GetOutputStreamAt(0));

                        }

                        viewer.Source = GetBitmapImageFromFile(file.Path);

                        _imagePath = file.Path;
                        viewer.Visibility = Visibility.Visible;
                        preview.Visibility = Visibility.Collapsed;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else StartPreview();
        }

        private BitmapImage GetBitmapImageFromFile(string filePath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(filePath);

            image.EndInit();
            return image;
        }
        private BitmapImage GetBitmapImage(Bitmap bmp)
        {

            var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;

        }




        private void btnCapture_Copy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ImagePath = _imagePath;
            Close();
        }
    }
}
