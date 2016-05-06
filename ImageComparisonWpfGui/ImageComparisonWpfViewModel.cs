using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using XnaFan.ImageComparison;

namespace ImageComparisonWpfGui
{
    public class ImageComparisonWpfViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddImage1 { get; set; }
        public ICommand AddImage2 { get; set; }

        private BitmapImage _image1, _image2, _differenceImage;
        public BitmapImage Image1
        {
            get { return _image1; }
            set
            {
                if (_image1 != value)
                {
                    _image1 = value;
                    OnPropertyChanged("Image1");
                }
            }
        }

        public BitmapImage Image2
        {
            get { return _image2; }
            set
            {
                if (_image2 != value)
                {
                    _image2 = value;
                    OnPropertyChanged("Image2");
                }
            }
        }

        public BitmapImage DifferenceImage
        {
            get { return _differenceImage; }
            private set
            {
                if (_differenceImage != value)
                {
                    _differenceImage = value;
                    OnPropertyChanged("DifferenceImage");
                }
            }
        }



        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public ImageComparisonWpfViewModel()
        {
            this.AddImage1 = new AddImageCommand(this, 1);
            this.AddImage2 = new AddImageCommand(this, 2);

            PropertyChanged += new PropertyChangedEventHandler(ImageComparisonWpfViewModelPropertyChanged);
        }

        void ImageComparisonWpfViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Image1" || e.PropertyName == "Image2")
            {
                if (Image1 != null && Image2 != null)
                {
                    Bitmap bmp = BitmapImage2Bitmap(Image1).GetDifferenceImage(BitmapImage2Bitmap(Image2), true);
                    DifferenceImage = (BitmapImage) Bitmap2BitmapImage(bmp);
                }
            }
        }


        internal void AddAnImage(BitmapImage image, int imageNumber)
        {

            switch (imageNumber)
            {
                case 1:
                    this.Image1 = image;
                    break;
                case 2:
                    this.Image2 = image;
                    break;
                default: throw new ArgumentException("invalid image number - only 1 and 2 allowed - you passed: " + imageNumber);
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            MemoryStream ms = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(ms);
            return new Bitmap(ms);
        }

        //http://stackoverflow.com/questions/6484357/converting-bitmapimage-to-bitmap-and-vice-versa
        BitmapSource Bitmap2BitmapImage(Bitmap bmp)
        {
            BitmapImage bmimg = null;
             using (MemoryStream ms = new MemoryStream())
             {
                 Byte[] imageData = null;
                 bmp.Save(ms, ImageFormat.Png);
                 imageData = ms.ToArray();
                 bmimg = GetBitmapImageFromBytes(imageData);
             }
            return bmimg;
        }


        //http://stackoverflow.com/questions/6484357/converting-bitmapimage-to-bitmap-and-vice-versa
        private BitmapImage GetBitmapImageFromBytes(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BitmapImage btm = new BitmapImage();
                btm.BeginInit();
                btm.StreamSource = ms;
                // Below code for caching is crucial.
                btm.CacheOption = BitmapCacheOption.OnLoad;
                btm.EndInit();
                return btm;
            }
        }
    }
}
