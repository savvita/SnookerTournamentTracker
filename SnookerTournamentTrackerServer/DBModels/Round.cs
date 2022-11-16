using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class Round
{
    public int Id { get; set; }

    public string Round1 { get; set; } = null!;

    public virtual ICollection<TournamentsRound> TournamentsRounds { get; } = new List<TournamentsRound>();
}
