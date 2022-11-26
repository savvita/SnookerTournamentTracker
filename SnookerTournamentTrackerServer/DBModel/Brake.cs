using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Brake
{
    public int Id { get; set; }

    public int FrameId { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public short Score { get; set; }

    public virtual Frame Frame { get; set; } = null!;
}
