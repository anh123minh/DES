using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace TrafficManagement.WPF.Models
{
    public static class ImageLoader
    {
        private static readonly List<BitmapImage> Images = new List<BitmapImage>();

        static ImageLoader()
        {
            Images.Add(new BitmapImage(new Uri(@"pack://application:,,,/TrafficManagement.WPF;component/Assets/circle_red.png", UriKind.Absolute)) { CacheOption = BitmapCacheOption.OnLoad });
            Images.Add(new BitmapImage(new Uri(@"pack://application:,,,/TrafficManagement.WPF;component/Assets/circle_blue.png", UriKind.Absolute)) { CacheOption = BitmapCacheOption.OnLoad });
            Images.Add(new BitmapImage(new Uri(@"pack://application:,,,/TrafficManagement.WPF;component/Assets/circle_green.png", UriKind.Absolute)) { CacheOption = BitmapCacheOption.OnLoad });
        }

        public static BitmapImage GetImageById(int id)
        {
            return Images.Count < id ? null : Images[id];
        }
    }
}
