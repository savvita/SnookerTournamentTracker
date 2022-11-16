using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class RegistrationStatus
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<TournamentsPlayer> TournamentsPlayers { get; } = new List<TournamentsPlayer>();
}
