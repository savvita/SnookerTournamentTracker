using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class RegistrationStatus
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A status is required")]
    [StringLength(30, ErrorMessage = "Maximum length for a status is 30 chars")]
    public string Status { get; set; } = null!;

    public virtual ICollection<TournamentsPlayer> TournamentsPlayers { get; } = new List<TournamentsPlayer>();
}
