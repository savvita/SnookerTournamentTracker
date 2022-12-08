using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class PhoneNumber
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Column("PhoneNumber")]
    [Required(ErrorMessage = "A phone number is required")]
    [StringLength(20, ErrorMessage = "Maximum length for a phone number is 20 chars")]
    public string Number { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
