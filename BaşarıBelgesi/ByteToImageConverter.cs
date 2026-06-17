using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BaşarıBelgesi
{
    public class ByteToImageConverter : IValueConverter
    {
        private static readonly bool IsDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsDesignMode)
            {
                return null;
            }

            if (value is not string base64 || string.IsNullOrWhiteSpace(base64))
            {
                return null;
            }

            try
            {
                byte[] buffer = System.Convert.FromBase64String(base64);
                BitmapImage image = new();
                using (MemoryStream stream = new(buffer))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}