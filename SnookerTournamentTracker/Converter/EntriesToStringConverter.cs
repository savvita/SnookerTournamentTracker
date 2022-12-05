using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TournamentLibrary;

namespace SnookerTournamentTracker.Converter
{
    internal class EntriesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is List<MatchUpEntryModel> list)
            {
                if (list.Count == 1)
                {
                    return $"{list[0].Player?.FirstName} {list[0].Player?.LastName} (w/o)";
                }
                if (list.Count == 2)
                {
                    string fisrtPlayer = string.Empty;
                    string secondPlayer = string.Empty;

                    if (list[0].Player == null)
                    {
                        fisrtPlayer = list[0].ParentMatchUp != null ? $"Winner of the match #{list[0].ParentMatchUp!.MatchNumber}" : "Player 1";
                    }
                    else
                    {
                        fisrtPlayer = $"{list[0].Player!.FirstName} {list[0].Player!.LastName}";
                    }

                    if (list[1].Player == null)
                    {
                        secondPlayer = list[1].ParentMatchUp != null ? $"Winner of the match #{list[1].ParentMatchUp!.MatchNumber}" : "Player 2";
                    }
                    else
                    {
                        secondPlayer = $"{list[1].Player!.FirstName} {list[1].Player!.LastName}";
                    }

                    return $"{fisrtPlayer} vs {secondPlayer}";
                }
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
