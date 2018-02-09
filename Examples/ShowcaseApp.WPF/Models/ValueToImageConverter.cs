using System;
using System.Globalization;
using System.Windows.Data;
using TrafficManagement.WPF.Models;
using System.Windows.Media;

namespace TrafficManagement.WPF
{
    
    public sealed class ValueToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return null;
            return ImageLoader.GetImageById((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Image to Id conversion is not supported!");
        }

        #endregion
    }

    public sealed class ValueToPersonImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return null;
            return ThemedDataStorage.GetImageById((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Image to Id conversion is not supported!");
        }

        #endregion

        
    }

    public sealed class ValueToEditorImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return null;
            return ThemedDataStorage.GetEditorImageById((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Image to Id conversion is not supported!");
        }

        #endregion
    }

    [ValueConversion(typeof(string), typeof(Brush))]
    public class StringToBrushConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string _source = value as string;
                if (_source == "Green") return new SolidColorBrush(Colors.Green);
                if (_source == "LightGreen") return new SolidColorBrush(Colors.LightGreen);
                if (_source == "Red") return new SolidColorBrush(Colors.Red);
                if (_source == "Gold") return new SolidColorBrush(Colors.Gold);
                if (_source == "Black") return new SolidColorBrush(Colors.Black);
                if (_source == "LightBlue") return new SolidColorBrush(Colors.LightBlue);
                if (_source == "Gray") return new SolidColorBrush(Colors.LightGray);
                if (_source == "Violet") return new SolidColorBrush(Colors.Violet);
                if (_source == "Orange") return new SolidColorBrush(Colors.Orange);
                if (_source == "Yellow") return new SolidColorBrush(Colors.Yellow);
                if (_source == "DarkRed") return new SolidColorBrush(Colors.DarkRed);
                /*  switch (_sourse)
                  {
                      case "Green":
                          return new SolidColorBrush(Colors.Green);                        
                      case "Red":
                          return new SolidColorBrush(Colors.Red);
                      case "Gold":
                          return new SolidColorBrush(Colors.Gold);
                      default:
                          return new SolidColorBrush(Colors.Black);
                  } */
            }

            // Default color
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // To keep it simple, no need to convert back 
            return null;
        }
        #endregion
    }
}
