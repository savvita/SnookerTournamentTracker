using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SnookerTournamentTracker.LocalValidation
{
    internal class PositiveOnlyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo ci)
        {
            if(value == null)
            {
                return new ValidationResult(true, null);
            }

            if(decimal.TryParse(value.ToString(), out decimal n))
            {
                return new ValidationResult(n >= 0, null);
            }

            return new ValidationResult(true, null);
        }
    }
}
