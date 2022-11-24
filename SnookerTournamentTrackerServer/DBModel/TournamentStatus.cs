﻿using System;
using System.Collections.Generic;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class TournamentStatus
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Tournament> Tournaments { get; } = new List<Tournament>();
}