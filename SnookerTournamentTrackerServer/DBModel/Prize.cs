using System;
using System.Collections.Generic;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Prize
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int PlaceId { get; set; }

    public decimal Amount { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;
}
