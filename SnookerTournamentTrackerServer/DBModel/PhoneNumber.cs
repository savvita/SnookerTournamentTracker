using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class PhoneNumber
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Column("PhoneNumber")]
    public string Number { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
