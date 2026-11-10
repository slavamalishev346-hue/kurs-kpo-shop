using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Shop.Core
{
    public class ArticleToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string basePath = AppDomain.CurrentDomain.BaseDirectory;

         
            if (value is string article && !string.IsNullOrEmpty(article))
            {
                string filePath = Path.Combine(basePath, "Images", "Products", $"{article}.jpg");
                if (File.Exists(filePath))
                    return new BitmapImage(new Uri(filePath, UriKind.Absolute));
            }

            string noImagePath = Path.Combine(basePath, "Images", "Products", "no_image.png");
            if (File.Exists(noImagePath))
                return new BitmapImage(new Uri(noImagePath, UriKind.Absolute));

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}