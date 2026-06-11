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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return null;
            }
            if (value is string base64 && !string.IsNullOrWhiteSpace(base64))
            {
                BitmapImage bi = new();
                bi.BeginInit();
                byte[] buffer = System.Convert.FromBase64String(base64);
                bi.StreamSource = new MemoryStream(buffer);
                bi.CacheOption = BitmapCacheOption.None;
                bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.DelayCreation;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}