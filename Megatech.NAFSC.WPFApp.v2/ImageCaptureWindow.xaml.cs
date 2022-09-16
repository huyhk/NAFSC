using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WebEye.Controls.Wpf;

namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for ImageCaptureWindow.xaml
    /// </summary>
    public partial class ImageCaptureWindow : Window
    {
        List<WebCameraId> cameras;
        //MediaCapture mediaCapture = new MediaCapture();
        public ImageCaptureWindow()
        {
            InitializeComponent();
            cameras = new List<WebCameraId>(webCamControl.GetVideoCaptureDevices());

        }
        public ImageCaptureWindow(string number)
        {
            InitializeComponent();
            cameras = new List<WebCameraId>(webCamControl.GetVideoCaptureDevices());
            _number = number;

            this.Title = FindResource("capture_image_invoice").ToString() + " " + _number;
        }
        private string _number;

        private void Capture()
        {

        }
        private bool m_initialized;
        protected override async void OnActivated(System.EventArgs e)
        {
            base.OnActivated(e);
            if (m_initialized)
            {
                return; // Already initialized
            }
            m_initialized = true;

            //var capture = new MediaCapture();
            //await capture.InitializeAsync(new MediaCaptureInitializationSettings
            //{
            //    StreamingCaptureMode = StreamingCaptureMode.Video // No audio
            //});

            //var preview = new CapturePreview(capture);
            //Preview.Source = preview;
            //await preview.StartAsync();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            webCamControl.StartCapture(cameras[0]);

        }
        public string ImagePath { get; set; }
        private string _imagePath = null;
        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (webCamControl.IsCapturing)
            {
                var folder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "images");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                var file = System.IO.Path.Combine(folder,_number+ ".jpg");
                if (File.Exists(file))
                    File.Delete(file);
                using (Bitmap bmp = webCamControl.GetCurrentImage())
                {
                    bmp.Save(file, ImageFormat.Jpeg);
                    webCamControl.StopCapture();
                    //BitmapImage image = new BitmapImage();
                    //image.BeginInit();
                    //image.UriSource = new Uri(file);
                    //image.EndInit();
                    
                    _imagePath = file;
                    viewer.Source = GetBitmapImage(bmp);
                    webCamControl.Visibility = Visibility.Collapsed;
                    viewer.Visibility = Visibility.Visible;
                }
            }
            else
                StartCapture();
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
        private void StartCapture()
        {
            if (cameras.Count > 0)
            {
                webCamControl.Visibility = Visibility.Visible;
                viewer.Visibility = Visibility.Collapsed;
                webCamControl.StartCapture(cameras[0]);
               
                viewer.Source = null;
            }
            else
                MessageBox.Show("No camera found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
