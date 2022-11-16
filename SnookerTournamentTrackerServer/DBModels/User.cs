using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class User
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? SecondName { get; set; }

    public string LastName { get; set; } = null!;

    public int UserRoleId { get; set; }

    public virtual ICollection<Administrator> Administrators { get; } = new List<Administrator>();

    public virtual ICollection<Email> Emails { get; } = new List<Email>();

    public virtual ICollection<FrameEntity> FrameEntities { get; } = new List<FrameEntity>();

    public virtual ICollection<Frame> Frames { get; } = new List<Frame>();

    public virtual ICollection<Match> Matches { get; } = new List<Match>();

    public virtual ICollection<PhoneNumber> PhoneNumbers { get; } = new List<PhoneNumber>();

    public virtual ICollection<TournamentsPlayer> TournamentsPlayers { get; } = new List<TournamentsPlayer>();

    public virtual UserRole UserRole { get; set; } = null!;
}
