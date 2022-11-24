using System;
using System.Collections.Generic;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Brake
{
    public int Id { get; set; }

    public int FrameId { get; set; }

    public short Score { get; set; }

    public virtual Frame Frame { get; set; } = null!;
}
