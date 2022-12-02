using System;
using System.Collections.Generic;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Administrator
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TournamentId { get; set; }

    public virtual Tournament Tournament { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
