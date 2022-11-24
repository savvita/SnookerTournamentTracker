using System;
using System.Collections.Generic;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class TournamentsPlayer
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int UserId { get; set; }

    public int RegistrationStatusId { get; set; }

    public virtual RegistrationStatus RegistrationStatus { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
