using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Round
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string RoundName { get; set; } = null!;

    public virtual ICollection<TournamentsRound> TournamentsRounds { get; } = new List<TournamentsRound>();
}
