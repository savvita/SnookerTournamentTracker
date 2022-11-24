﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class User
{
    // TODO - add min max length
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    public string? SecondName { get; set; }

    [Required]
    public string LastName { get; set; } = null!;

    public int UserRoleId { get; set; }

    public virtual ICollection<Administrator> Administrators { get; } = new List<Administrator>();

    public virtual ICollection<FrameEntity> FrameEntities { get; } = new List<FrameEntity>();

    public virtual ICollection<Frame> Frames { get; } = new List<Frame>();

    public virtual ICollection<Invitation> Invitations { get; } = new List<Invitation>();

    public virtual ICollection<Match> Matches { get; } = new List<Match>();

    public virtual ICollection<PhoneNumber> PhoneNumbers { get; } = new List<PhoneNumber>();

    public virtual ICollection<TournamentsPlayer> TournamentsPlayers { get; } = new List<TournamentsPlayer>();

    public virtual UserRole UserRole { get; set; } = null!;
}