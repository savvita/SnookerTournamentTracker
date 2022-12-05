using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class UserRole
{
    public int Id { get; set; }

    [Column("UserRole")]
    [Required(ErrorMessage = "A user role is required")]
    [StringLength(50, ErrorMessage = "Maximum length for an user role is 50 chars")]
    public string Role { get; set; } = null!;

    public virtual ICollection<User> Users { get; } = new List<User>();
}
