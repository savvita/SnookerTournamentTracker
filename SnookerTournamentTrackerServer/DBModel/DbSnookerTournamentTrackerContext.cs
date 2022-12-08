using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SnookerTournamentTrackerServer.DbModel;

public partial class DbSnookerTournamentTrackerContext : DbContext
{
    public DbSnookerTournamentTrackerContext()
    {
    }

    public DbSnookerTournamentTrackerContext(DbContextOptions<DbSnookerTournamentTrackerContext> options)
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
        //=> optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
        => optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["SmarterConnection"].ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Administ__3214EC07C1B30E60");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Administrators)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Administr__Tourn__44FF419A");

            entity.HasOne(d => d.User).WithMany(p => p.Administrators)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Administr__UserI__440B1D61");
        });

        modelBuilder.Entity<Brake>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brakes__3214EC07F7612AEA");

            entity.HasOne(d => d.Frame).WithMany(p => p.Brakes)
                .HasForeignKey(d => d.FrameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Brakes__FrameId__5AEE82B9");

            entity.HasOne(d => d.Player).WithMany(p => p.Brakes)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Brakes__PlayerId__14270015");
        });

        modelBuilder.Entity<Frame>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Frames__3214EC0768CFA24D");

            entity.HasOne(d => d.Match).WithMany(p => p.Frames)
                .HasForeignKey(d => d.MatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Frames__MatchId__571DF1D5");

            entity.HasOne(d => d.Winner).WithMany(p => p.Frames)
                .HasForeignKey(d => d.WinnerId)
                .HasConstraintName("FK__Frames__WinnerId__5812160E");
        });

        modelBuilder.Entity<FrameEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FrameEnt__3214EC0765F98F89");

            entity.HasOne(d => d.Frame).WithMany(p => p.FrameEntities)
                .HasForeignKey(d => d.FrameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FrameEnti__Frame__5DCAEF64");

            entity.HasOne(d => d.Player).WithMany(p => p.FrameEntities)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FrameEnti__Playe__5EBF139D");
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invitati__3214EC071F548459");

            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.Tournamentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invitatio__Tourn__656C112C");

            entity.HasOne(d => d.User).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invitatio__UserI__6477ECF3");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Matches__3214EC07E4CCBE7D");

            entity.HasOne(d => d.Round).WithMany(p => p.Matches)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Matches__RoundId__4E88ABD4");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Matches)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Matches__Tournam__4D94879B");

            entity.HasOne(d => d.Winner).WithMany(p => p.Matches)
                .HasForeignKey(d => d.WinnerId)
                .HasConstraintName("FK__Matches__WinnerI__4F7CD00D");
        });

        modelBuilder.Entity<MatchUpEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MatchUpE__3214EC0704CC7198");

            entity.HasOne(d => d.User).WithMany(p => p.MatchUpEntries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MatchUpEn__UserI__02FC7413");

            entity.HasOne(d => d.Match).WithMany(p => p.MatchUpEntries)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK__MatchUpEn__Match__03F0984C");

            entity.HasOne(d => d.ParentMatch).WithMany(p => p.ParentMatchUpEntries)
                .HasForeignKey(d => d.ParentMatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MatchUpEn__Paren__04E4BC85");
        });

        modelBuilder.Entity<PhoneNumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhoneNum__3214EC079CAD4DA0");

            entity.Property(e => e.Number)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PhoneNumber");

            entity.HasOne(d => d.User).WithMany(p => p.PhoneNumbers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhoneNumb__UserI__619B8048");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Places__3214EC0748161CB2");

            entity.Property(e => e.PlaceName).HasMaxLength(20);

            entity.HasOne(d => d.Round).WithMany(p => p.Places)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Places__RoundId__2BFE89A6");
        });

        modelBuilder.Entity<Prize>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prizes__3214EC0739F68832");

            entity.Property(e => e.Amount).HasColumnType("money");

            entity.HasOne(d => d.Place).WithMany(p => p.Prizes)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prizes__PlaceId__5441852A");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Prizes)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prizes__Tourname__534D60F1");
        });

        modelBuilder.Entity<RegistrationStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registra__3214EC072441627C");

            entity.ToTable("RegistrationStatus");

            entity.Property(e => e.Status).HasMaxLength(30);
        });

        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rounds__3214EC07EFAA043E");

            entity.Property(e => e.RoundName).HasMaxLength(30);
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC070CC0F32D");

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
                .HasConstraintName("FK__Tournamen__Tourn__3F466844");

            entity.HasOne(d => d.PaymentInfo).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.PaymentInfoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Payme__3F115E1A");
        });

        modelBuilder.Entity<TournamentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC07AA80D7B5");

            entity.ToTable("TournamentStatus");

            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<TournamentsPlayer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC0720BBD064");

            entity.HasOne(d => d.RegistrationStatus).WithMany(p => p.TournamentsPlayers)
                .HasForeignKey(d => d.RegistrationStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Regis__6D0D32F4");

            entity.HasOne(d => d.Tournament).WithMany(p => p.TournamentsPlayers)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Tourn__6B24EA82");

            entity.HasOne(d => d.User).WithMany(p => p.TournamentsPlayers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__UserI__6C190EBB");
        });

        modelBuilder.Entity<TournamentsRound>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC0769B45C6C");

            entity.HasOne(d => d.Round).WithMany(p => p.TournamentsRounds)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Round__4AB81AF0");

            entity.HasOne(d => d.Tournament).WithMany(p => p.TournamentsRounds)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Tourn__49C3F6B7");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("money");

            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC073EF0FF23");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Tourna__2B0A656D");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__UserId__2A164134");
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.Property(e => e.CardNumber).HasMaxLength(16);

            entity.HasKey(e => e.Id).HasName("PK__Cards__3214EC077AFDA19F");

            entity.HasOne(d => d.User).WithMany(p => p.Cards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cards__UserId__40058253");
        });

        modelBuilder.Entity<PaymentInfo>(entity =>
        {
            entity.ToTable("PaymentInfo");

            entity.Property(e => e.Sum).HasColumnType("money");

            entity.HasKey(e => e.Id).HasName("PK__PaymentI__3214EC07BC47471F");

            entity.HasOne(d => d.Card).WithMany(p => p.PaymentInfos)
                .HasForeignKey(d => d.CardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentIn__CardI__3E1D39E1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07618A56AF");

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
                .HasConstraintName("FK__Users__UserRoleI__3C69FB99");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC076CD9BC59");

            entity.ToTable("UserRole");

            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("UserRole");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
