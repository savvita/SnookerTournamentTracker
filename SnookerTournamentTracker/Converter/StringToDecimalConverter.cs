using System;
using System.Globalization;
using System.Windows.Data;

namespace SnookerTournamentTracker.Converter
{
    internal class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) 
            { 
                return string.Empty; 
            }

            return value.ToString()!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            decimal.TryParse(value.ToString(), out decimal result);

            return result;
        }
    }
}
