using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Brake
{
    public int Id { get; set; }

    public int FrameId { get; set; }

    public int PlayerId { get; set; }

    [Required(ErrorMessage = "A score is required")]
    [Range(0, int.MaxValue, ErrorMessage = "A score must be grater than or equal to 0")]
    public short Score { get; set; }

    public virtual Frame Frame { get; set; } = null!;

    public virtual User Player { get; set; } = null!;
}
