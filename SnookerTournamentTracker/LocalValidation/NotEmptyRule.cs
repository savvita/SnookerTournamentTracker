using System.Security;
using System.Windows.Controls;

namespace SnookerTournamentTracker.LocalValidation
{
    internal class NotEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo ci)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Fill all the required fields");
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
                return new ValidationResult(false, "Fill all the required fields");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
