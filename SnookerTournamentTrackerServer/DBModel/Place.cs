using System;
using System.Collections.Generic;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Place
{
    public int Id { get; set; }

    public string PlaceName { get; set; } = null!;

    public virtual ICollection<Prize> Prizes { get; } = new List<Prize>();
}
