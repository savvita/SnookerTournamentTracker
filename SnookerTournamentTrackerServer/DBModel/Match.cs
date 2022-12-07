using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Match
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A match number is required")]
    public int MatchNumber { get; set; }

    public int TournamentId { get; set; }

    public int RoundId { get; set; }

    public int? WinnerId { get; set; }

    public bool IsCompleted { get; set; } = false;

    public virtual ICollection<Frame> Frames { get; } = new List<Frame>();

    public virtual ICollection<MatchUpEntry> MatchUpEntries { get; } = new List<MatchUpEntry>();

    public virtual ICollection<MatchUpEntry> ParentMatchUpEntries { get; } = new List<MatchUpEntry>();

    public virtual TournamentsRound Round { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;

    public virtual User? Winner { get; set; }
}
