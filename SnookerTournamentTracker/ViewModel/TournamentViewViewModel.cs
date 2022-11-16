using SnookerTournamentTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class TournamentViewViewModel
    {
        public TournamentModel Tournament { get; set; }
        public List<RoundModel>? Rounds { get; set; }
        public List<PrizeModel>? Prizes { get; set; }

        public TournamentViewViewModel(TournamentModel tournament)
        {
            Tournament = tournament;
            Rounds = ConnectionClientModel.GetRounds(tournament.Id);
            Prizes = ConnectionClientModel.GetPrizes(tournament.Id);
        }
    }
}
