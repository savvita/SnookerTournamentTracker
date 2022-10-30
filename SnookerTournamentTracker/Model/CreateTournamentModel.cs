using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.Model
{
    internal class CreateTournamentModel
    {
        public List<PersonModel> GetAllPlayers()
        {
            return ConnectionClientModel.GetAllPlayers();
        }

        public bool CreateTournament(TournamentModel tournament)
        {
            return ConnectionClientModel.CreateTournament(tournament);
        }
    }
}
