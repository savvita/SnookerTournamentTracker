using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Prize
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int PlaceId { get; set; }

    [Required(ErrorMessage = "An amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "An amount must be grater than or equal to 0")]
    public decimal Amount { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;
}
