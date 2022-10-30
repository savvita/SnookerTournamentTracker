using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SnookerTournamentTracker.LocalValidation
{
    internal class NotEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo ci)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Required field");
            }
            int length = 0;

            if (value is string str)
            {
                length = str.Length;
            }
            else if (value is SecureString secStr)
            {
                length = secStr.Length;
            }

            if (length == 0)
            {
                return new ValidationResult(false, "Required field");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
