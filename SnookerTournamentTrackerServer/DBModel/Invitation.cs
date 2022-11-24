using System;
using System.Collections.Generic;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Invitation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Tournamentid { get; set; }

    public DateTime? Date { get; set; }

    public virtual Tournament Tournament { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
