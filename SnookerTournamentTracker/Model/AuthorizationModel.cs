using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.Model
{
    internal class AuthorizationModel
    {

        public bool SignIn(PersonModel user, out string error)
        {
            return ConnectionClientModel.SignIn(user, out error);
        }

        public bool SignUp(PersonModel user, out string error)
        {
            return ConnectionClientModel.SignUp(user, out error);
        }
    }
}
