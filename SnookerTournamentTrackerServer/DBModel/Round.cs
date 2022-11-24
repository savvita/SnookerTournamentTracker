using System;
using System.Collections.Generic;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Round
{
    public int Id { get; set; }

    public string RoundName { get; set; } = null!;

    public virtual ICollection<TournamentsRound> TournamentsRounds { get; } = new List<TournamentsRound>();
}
