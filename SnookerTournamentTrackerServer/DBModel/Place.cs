using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Place
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string PlaceName { get; set; } = null!;

    public virtual ICollection<Prize> Prizes { get; } = new List<Prize>();
}
