using System;
using System.Globalization;
using System.Windows.Data;
using TournamentLibrary;

namespace SnookerTournamentTracker.Converter
{
    internal class PersonToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is PersonModel person)
            {
                return $"{person.FirstName} {person.LastName}";
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
