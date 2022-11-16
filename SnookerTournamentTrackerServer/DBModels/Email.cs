using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class Email
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Email1 { get; set; } = null!;

    public virtual ICollection<Invitation> Invitations { get; } = new List<Invitation>();

    public virtual User User { get; set; } = null!;
}
