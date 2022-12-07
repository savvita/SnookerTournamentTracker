using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "An email is required")]
    [EmailAddress(ErrorMessage = "The email is not a valid e-mail address")]
    [StringLength(255, ErrorMessage = "Maximum length for an email is 255 chars")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "A password is required")]
    [StringLength(100, ErrorMessage = "Maximum length for a password is 100 chars")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "A first name is required")]
    [StringLength(100, ErrorMessage = "Maximum length for a first name is 100 chars")]
    public string FirstName { get; set; } = null!;

    [StringLength(100, ErrorMessage = "Maximum length for a second name is 100 chars")]
    public string? SecondName { get; set; }

    [Required(ErrorMessage = "A last name is required")]
    [StringLength(100, ErrorMessage = "Maximum length for a last name is 100 chars")]
    public string LastName { get; set; } = null!;

    public int UserRoleId { get; set; }

    public virtual ICollection<Administrator> Administrators { get; } = new List<Administrator>();

    public virtual ICollection<FrameEntity> FrameEntities { get; } = new List<FrameEntity>();

    public virtual ICollection<Frame> Frames { get; } = new List<Frame>();

    public virtual ICollection<Invitation> Invitations { get; } = new List<Invitation>();

    public virtual ICollection<Match> Matches { get; } = new List<Match>();

    public virtual ICollection<MatchUpEntry> MatchUpEntries { get; } = new List<MatchUpEntry>();

    public virtual ICollection<Brake> Brakes { get; } = new List<Brake>();

    public virtual ICollection<PhoneNumber> PhoneNumbers { get; } = new List<PhoneNumber>();

    public virtual ICollection<TournamentsPlayer> TournamentsPlayers { get; } = new List<TournamentsPlayer>();

    public virtual ICollection<Payment> Payments { get; } = new List<Payment>();

    public virtual ICollection<Card> Cards { get; } = new List<Card>();

    public virtual UserRole UserRole { get; set; } = null!;
}
