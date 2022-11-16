using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class Invitation
{
    public int Id { get; set; }

    public int EmailId { get; set; }

    public int Tournamentid { get; set; }

    public DateTime? Date { get; set; }

    public virtual Email Email { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;
}
