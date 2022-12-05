using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Round
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A round name is required")]
    [StringLength(30, ErrorMessage = "Maximum length for a round name is 30 chars")]
    public string RoundName { get; set; } = null!;

    public virtual ICollection<TournamentsRound> TournamentsRounds { get; } = new List<TournamentsRound>();
}
