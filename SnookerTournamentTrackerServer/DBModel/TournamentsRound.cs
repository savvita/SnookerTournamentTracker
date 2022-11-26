using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class TournamentsRound
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int RoundId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int FrameCount { get; set; }

    public virtual ICollection<Match> Matches { get; } = new List<Match>();

    public virtual Round Round { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;
}
