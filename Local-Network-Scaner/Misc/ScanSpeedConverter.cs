using Local_Network_Scanner.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Local_Network_Scanner.Misc
{
    public class ScanSpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ScanSpeedPreset speed)
            {
                // We construct the key dynamically: "Speed_" + "Slow" -> "Speed_Slow"
                string resourceKey = $"Speed_{speed}";

                // Try to get the string from resources
                return Application.Current.Resources[resourceKey] ?? speed.ToString();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
