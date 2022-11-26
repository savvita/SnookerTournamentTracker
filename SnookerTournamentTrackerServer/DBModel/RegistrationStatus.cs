﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class RegistrationStatus
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = null!;

    public virtual ICollection<TournamentsPlayer> TournamentsPlayers { get; } = new List<TournamentsPlayer>();
}
