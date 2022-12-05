using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Place
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A place name is required")]
    [StringLength(20, ErrorMessage = "Maximum length for a place name is 20 chars")]
    public string PlaceName { get; set; } = null!;

    public virtual ICollection<Prize> Prizes { get; } = new List<Prize>();
}
