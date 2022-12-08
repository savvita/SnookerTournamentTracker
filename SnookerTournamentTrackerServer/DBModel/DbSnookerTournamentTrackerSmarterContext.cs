using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class DbSnookerTournamentTrackerSmarterContext : DbContext
{
    public DbSnookerTournamentTrackerSmarterContext()
    {
    }

    public DbSnookerTournamentTrackerSmarterContext(DbContextOptions<DbSnookerTournamentTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Brake> Brakes { get; set; }

    public virtual DbSet<Frame> Frames { get; set; }

    public virtual DbSet<FrameEntity> FrameEntities { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<MatchUpEntry> MatchUpEntries { get; set; }

    public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Prize> Prizes { get; set; }

    public virtual DbSet<RegistrationStatus> RegistrationStatuses { get; set; }

    public virtual DbSet<Round> Rounds { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<TournamentStatus> TournamentStatuses { get; set; }

    public virtual DbSet<TournamentsPlayer> TournamentsPlayers { get; set; }

    public virtual DbSet<TournamentsRound> TournamentsRounds { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<PaymentInfo> PaymentInfos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["SmarterConnection"].ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Administ__3214EC0708F4F2B4");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Administrators)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Administr__Tourn__73BA3083");

            entity.HasOne(d => d.User).WithMany(p => p.Administrators)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Administr__UserI__72C60C4A");
        });

        modelBuilder.Entity<Brake>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brakes__3214EC076BF6DF3A");

            entity.HasOne(d => d.Frame).WithMany(p => p.Brakes)
                .HasForeignKey(d => d.FrameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Brakes__FrameId__123EB7A3");

            entity.HasOne(d => d.Player).WithMany(p => p.Brakes)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Brakes__PlayerId__114A936A");
        });

        modelBuilder.Entity<Frame>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Frames__3214EC0702261FFF");

            entity.HasOne(d => d.Match).WithMany(p => p.Frames)
                .HasForeignKey(d => d.MatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Frames__MatchId__0D7A0286");

            entity.HasOne(d => d.Winner).WithMany(p => p.Frames)
                .HasForeignKey(d => d.WinnerId)
                .HasConstraintName("FK__Frames__WinnerId__0E6E26BF");
        });

        modelBuilder.Entity<FrameEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FrameEnt__3214EC07C521D7DD");

            entity.HasOne(d => d.Frame).WithMany(p => p.FrameEntities)
                .HasForeignKey(d => d.FrameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FrameEnti__Frame__151B244E");

            entity.HasOne(d => d.Player).WithMany(p => p.FrameEntities)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FrameEnti__Playe__160F4887");
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invitati__3214EC07E37D7974");

            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.Tournamentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invitatio__Tourn__1CBC4616");

            entity.HasOne(d => d.User).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invitatio__UserI__1BC821DD");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Matches__3214EC07CA26DD87");

            entity.HasOne(d => d.Round).WithMany(p => p.Matches)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Matches__RoundId__00200768");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Matches)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Matches__Tournam__7F2BE32F");

            entity.HasOne(d => d.Winner).WithMany(p => p.Matches)
                .HasForeignKey(d => d.WinnerId)
                .HasConstraintName("FK__Matches__WinnerI__01142BA1");
        });

        modelBuilder.Entity<MatchUpEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MatchUpE__3214EC071D82D1D1");

            entity.HasOne(d => d.User).WithMany(p => p.MatchUpEntries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MatchUpEn__UserI__04E4BC85");

            entity.HasOne(d => d.Match).WithMany(p => p.MatchUpEntries)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK__MatchUpEn__Match__05D8E0BE");

            entity.HasOne(d => d.ParentMatch).WithMany(p => p.ParentMatchUpEntries)
                .HasForeignKey(d => d.ParentMatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MatchUpEn__Paren__06CD04F7");
        });

        modelBuilder.Entity<PhoneNumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhoneNum__3214EC07EC04E830");

            entity.Property(e => e.Number)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PhoneNumber");

            entity.HasOne(d => d.User).WithMany(p => p.PhoneNumbers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhoneNumb__UserI__18EBB532");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Places__3214EC077C1C5461");

            entity.Property(e => e.PlaceName).HasMaxLength(20);

            entity.HasOne(d => d.Round).WithMany(p => p.Places)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Places__RoundId__787EE5A0");
        });

        modelBuilder.Entity<Prize>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prizes__3214EC07B5E9A884");

            entity.Property(e => e.Amount).HasColumnType("money");

            entity.HasOne(d => d.Place).WithMany(p => p.Prizes)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prizes__PlaceId__0A9D95DB");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Prizes)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prizes__Tourname__09A971A2");
        });

        modelBuilder.Entity<RegistrationStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registra__3214EC07F8A3A00E");

            entity.ToTable("RegistrationStatus");

            entity.Property(e => e.Status).HasMaxLength(30);
        });

        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rounds__3214EC0770CB4DD2");

            entity.Property(e => e.RoundName).HasMaxLength(30);
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC0705F4E454");

            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.EntreeFee).HasColumnType("money");
            entity.Property(e => e.Garantee).HasColumnType("money");
            entity.Property(e => e.IsPrivate).HasDefaultValueSql("((0))");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PrizeMode).HasDefaultValueSql("((0))");
            entity.Property(e => e.StartDate).HasColumnType("date");

            entity.HasOne(d => d.TournamentStatus).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.TournamentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Tourn__693CA210");

            entity.HasOne(d => d.PaymentInfo).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.PaymentInfoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Payme__6C190EBB");
        });

        modelBuilder.Entity<TournamentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC07BEF27268");

            entity.ToTable("TournamentStatus");

            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<TournamentsPlayer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC07E665BD55");

            entity.HasOne(d => d.RegistrationStatus).WithMany(p => p.TournamentsPlayers)
                .HasForeignKey(d => d.RegistrationStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Regis__245D67DE");

            entity.HasOne(d => d.Tournament).WithMany(p => p.TournamentsPlayers)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Tourn__22751F6C");

            entity.HasOne(d => d.User).WithMany(p => p.TournamentsPlayers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__UserI__236943A5");
        });

        modelBuilder.Entity<TournamentsRound>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC07DF1AFE44");

            entity.HasOne(d => d.Round).WithMany(p => p.TournamentsRounds)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Round__7C4F7684");

            entity.HasOne(d => d.Tournament).WithMany(p => p.TournamentsRounds)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Tourn__7B5B524B");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("money");

            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC07EEBA3A91");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Tourna__6FE99F9F");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__UserId__6EF57B66");
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.Property(e => e.CardNumber).HasMaxLength(16);

            entity.HasKey(e => e.Id).HasName("PK__Cards__3214EC07F4501A23");

            entity.HasOne(d => d.User).WithMany(p => p.Cards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cards__UserId__6383C8BA");
        });

        modelBuilder.Entity<PaymentInfo>(entity =>
        {
            entity.ToTable("PaymentInfo");

            entity.Property(e => e.Sum).HasColumnType("money");

            entity.HasKey(e => e.Id).HasName("PK__PaymentI__3214EC07AD3E4E2E");

            entity.HasOne(d => d.Card).WithMany(p => p.PaymentInfos)
                .HasForeignKey(d => d.CardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentIn__CardI__66603565");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC079948DC8A");

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.SecondName).HasMaxLength(100);

            entity.HasOne(d => d.UserRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__UserRoleI__60A75C0F");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC070ADEC1B4");

            entity.ToTable("UserRole");

            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("UserRole");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
