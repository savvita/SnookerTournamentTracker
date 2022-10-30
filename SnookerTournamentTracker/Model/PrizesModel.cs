using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.Model
{
    internal class PrizesModel
    {
        TournamentModel tournament;

        public PrizesModel(TournamentModel tournament)
        {
            this.tournament = tournament;
        }

        public List<string> GetAllPlaces()
        {
            return ConnectionClientModel.GetAllPlaces();
        }

        public decimal? GetGarantee()
        {
            return tournament.Garantee;
        }
    }
}
