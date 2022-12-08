using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Tournament
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A tournament name is required")]
    [StringLength(500, ErrorMessage = "Maximum length for a tournament name is 500 chars")]
    public string Name { get; set; } = null!;

    public int TournamentStatusId { get; set; }

    public int? PaymentInfoId { get; set; }

    public bool? IsPrivate { get; set; }

    public byte? PrizeMode { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Entree fee must be grater than or equal to 0")]
    public decimal? EntreeFee { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Garantee must be grater than or equal to 0")]
    public decimal? Garantee { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<Administrator> Administrators { get; } = new List<Administrator>();

    public virtual ICollection<Invitation> Invitations { get; } = new List<Invitation>();

    public virtual ICollection<Match> Matches { get; } = new List<Match>();

    public virtual ICollection<Prize> Prizes { get; } = new List<Prize>();

    public virtual TournamentStatus TournamentStatus { get; set; } = null!;

    public virtual PaymentInfo? PaymentInfo { get; set; }

    public virtual ICollection<TournamentsPlayer> TournamentsPlayers { get; } = new List<TournamentsPlayer>();

    public virtual ICollection<TournamentsRound> TournamentsRounds { get; } = new List<TournamentsRound>();

    public virtual ICollection<Payment> Payments { get; } = new List<Payment>();
}
