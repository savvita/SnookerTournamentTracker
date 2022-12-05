using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class TournamentStatus
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A tournament status is required")]
    [StringLength(20, ErrorMessage = "Maximum length for a tournament status is 20 chars")]
    public string Status { get; set; } = null!;

    public virtual ICollection<Tournament> Tournaments { get; } = new List<Tournament>();
}
