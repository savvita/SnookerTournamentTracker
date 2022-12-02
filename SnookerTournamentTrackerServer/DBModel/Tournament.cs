﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class Tournament
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    public int TournamentStatusId { get; set; }

    public bool? IsPrivate { get; set; }

    public byte? PrizeMode { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? EntreeFee { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Garantee { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<Administrator> Administrators { get; } = new List<Administrator>();

    public virtual ICollection<Invitation> Invitations { get; } = new List<Invitation>();

    public virtual ICollection<Match> Matches { get; } = new List<Match>();

    public virtual ICollection<Prize> Prizes { get; } = new List<Prize>();

    public virtual TournamentStatus TournamentStatus { get; set; } = null!;

    public virtual ICollection<TournamentsPlayer> TournamentsPlayers { get; } = new List<TournamentsPlayer>();

    public virtual ICollection<TournamentsRound> TournamentsRounds { get; } = new List<TournamentsRound>();
}
