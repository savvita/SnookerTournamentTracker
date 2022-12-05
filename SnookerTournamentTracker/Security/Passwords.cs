using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTracker.Security
{
    internal static class Passwords
    {
        public static string? SecureStringToString(SecureString? value)
        {
            if (value == null)
            {
                return null;
            }

            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static bool ComparePasswords(SecureString password, SecureString passwordConfirm)
        {
            string? pass = SecureStringToString(password);
            string? passConf = SecureStringToString(passwordConfirm);

            if(pass == null)
            {
                return false;
            }

            bool result = pass.Equals(passConf);

            int gen = GC.GetGeneration(pass);
            GC.Collect(gen);

            return result;
        }

    }
}
