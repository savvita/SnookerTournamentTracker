using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class TournamentStatus
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    public virtual ICollection<Tournament> Tournaments { get; } = new List<Tournament>();
}
