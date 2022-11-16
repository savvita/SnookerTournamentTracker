using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class Match
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int? ParentMatchUpId { get; set; }

    public int RoundId { get; set; }

    public int? WinnerId { get; set; }

    public virtual ICollection<Frame> Frames { get; } = new List<Frame>();

    public virtual ICollection<Match> InverseParentMatchUp { get; } = new List<Match>();

    public virtual Match? ParentMatchUp { get; set; }

    public virtual TournamentsRound Round { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;

    public virtual User? Winner { get; set; }
}
