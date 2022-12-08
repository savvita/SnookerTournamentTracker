using GalaSoft.MvvmLight.CommandWpf;
using SnookerTournamentTracker.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class TournamentViewViewModel : INotifyPropertyChanged
    {
        private TournamentModel? tournament;
        public TournamentModel Tournament
        {
            get => tournament;
            set
            {
                tournament = value;
                OnPropertyChanged(nameof(Tournament));
            }
        }
        public PersonModel User { get; }

        public bool IsEditable { get; private set; }

        private bool isMatchesEnabled;
        public bool IsMatchesEnabled
        {
            get => isMatchesEnabled;
            private set
            {
                isMatchesEnabled = value;
                OnPropertyChanged(nameof(IsMatchesEnabled));
            }
        }

        public bool IsConfirmPossible { get; }

        private bool isRegistered;
        public bool IsRegistered
        {
            get => isRegistered;
            set
            {
                isRegistered = value;
                OnPropertyChanged(nameof(IsRegistered));
            }
        }

        public bool IsPaid { get; }

        private bool isRegistrationOpened;
        public bool IsRegistrationOpened
        {
            get => isRegistrationOpened;
            set
            {
                isRegistrationOpened = value;
                OnPropertyChanged(nameof(IsRegistrationOpened));
            }
        }

        private bool isClosingRegistrationPossible;
        public bool IsClosingRegistrationPossible
        {
            get => isClosingRegistrationPossible;
            set
            {
                isClosingRegistrationPossible = value;
                OnPropertyChanged(nameof(IsClosingRegistrationPossible));
            }
        }

        private string? status;
        public string? Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private string? tournamentStatus;
        public string? TournamentStatus
        {
            get => tournamentStatus;
            set
            {
                tournamentStatus = value;
                OnPropertyChanged(nameof(TournamentStatus));
            }
        }

        public event Action<string>? RegisteringCompleted;
        public event Action<string>? UnregisteringCompleted;
        public event Action? RegistrationClosed;
        public event Action<string>? NeedPayback;
        public event PropertyChangedEventHandler? PropertyChanged;



        private ICollectionView playersView;
        public ObservableCollection<TournamentPlayerViewModel> Players { get; set; } = new ObservableCollection<TournamentPlayerViewModel>();

        public ObservableCollection<TournamentPlayerViewModel> WaitingPlayers { get; set; } = new ObservableCollection<TournamentPlayerViewModel>();

        public ObservableCollection<PrizeModel> Prizes { get; set; } = new ObservableCollection<PrizeModel>();

        public ObservableCollection<RoundModel> RoundModel { get; set; } = new ObservableCollection<RoundModel>();

        public TournamentViewViewModel(PersonModel user, TournamentModel tournament)
        {
            this.User = user;
            Tournament = tournament;

            IsEditable = false;

            LoadData();

            IsMatchesEnabled = tournament.Status != null ? !tournament.Status.Equals("Registration") : false;
            IsClosingRegistrationPossible = IsEditable && !IsMatchesEnabled;

            IsPaid = Tournament.PaymentInfo != null;


            playersView = CollectionViewSource.GetDefaultView(Players);

            playersView.SortDescriptions.Add(new SortDescription("Result", ListSortDirection.Ascending));

            IsConfirmPossible = IsPaid && IsEditable;
        }
        public async Task RefreshPlayersAsync()
        {
            var players = await ServerConnection.GetPlayersByTournamentIdAsync(Tournament.Id);

            Players.Clear();
            Tournament.Players.Clear();

            if (players == null)
            {
                return;
            }

            foreach (var player in players)
            {
                if (player.Player == null)
                {
                    continue;
                }

                Tournament.Players.Add(new PersonModel()
                {
                    Id = player.Player.Id,
                    FirstName = player.Player.FirstName,
                    SecondName = player.Player.SecondName,
                    LastName = player.Player.LastName,
                });
            }

            foreach (TournamentPlayer player in players.Where(pl => pl.Status.Equals("Registered")))
            {
                TournamentPlayerViewModel p = new TournamentPlayerViewModel()
                {
                    Player = player.Player
                };

                if (player.IsFinished)
                {
                    if (player.Payment != null)
                    {
                        p.Result = $"{player.Payment.ToString()} $";
                    }
                    else
                    {
                        p.Result = "Finished";
                    }
                }

                Players.Add(p);
            }

            WaitingPlayers.Clear();

            foreach (TournamentPlayer player in players.Where(pl => pl.Status.Equals("Waiting for paying")))
            {
                TournamentPlayerViewModel p = new TournamentPlayerViewModel()
                {
                    Player = player.Player
                };

                WaitingPlayers.Add(p);
            }

            var pl = players.FirstOrDefault(p => p.Player != null ? p.Player.Id == User.Id : false);

            if (pl != null)
            {
                Status = pl.Status;
            }

            if (players != null)
            {
                IsRegistered = players.Any(pl =>
                {
                    if (pl.Player == null)
                    {
                        return false;
                    }
                    return pl.Player.Id == User.Id;
                });
            }
        }

        public async Task LoadData()
        {
            //var rounds = await ServerConnection.GetRoundsByTournamentIdAsync(Tournament.Id);

            //if (rounds != null)
            //{
            //    Tournament.RoundModel = rounds;
            //}
            await RefreshRoundModel();
            await RefreshPrizes();

            if (Tournament.Status != null)
            {
                TournamentStatus = Tournament.Status;
                IsEditable = await ServerConnection.IsTournamentAdministratorAsync(User.Id, Tournament.Id) && !Tournament.Status!.Equals("Finished");
                IsRegistrationOpened = Tournament.Status.Equals("Registration");
            }
            else
            {
                IsEditable = false;
                IsRegistrationOpened = false;
            }

            await RefreshPlayersAsync();
        }

        private async Task RefreshPrizes()
        {
            Tournament.Prizes = await ServerConnection.GetPrizesByTournamentIdAsync(Tournament.Id);

            Prizes.Clear();

            if (Tournament.Prizes != null)
            {
                foreach(var prize in Tournament.Prizes)
                {
                    Prizes.Add(prize);
                }
            }
        }

        private async Task RefreshRoundModel()
        {
            var rounds = await ServerConnection.GetRoundsByTournamentIdAsync(Tournament.Id);

            if (rounds == null)
            {
                return;
            }

            Tournament.RoundModel = rounds;

            RoundModel.Clear();

            if (Tournament.RoundModel != null)
            {
                foreach (var round in Tournament.RoundModel)
                {
                    RoundModel.Add(round);
                }
            }
        }

        public async Task<bool> UpdateTournamentAsync(TournamentModel tournament)
        {
            if (await ServerConnection.UpdateTournamentAsync((int)User.Id!, tournament))
            {
                Tournament = tournament;
                await LoadData();
                return true;
            }

            return false;
        }

        #region Commands
        private RelayCommand<object>? confirmCmd;
        public RelayCommand<object> ConfirmCmd
        {
            get => confirmCmd ?? new RelayCommand<object>(async (obj) =>
            {
                if (obj is TournamentPlayerViewModel player)
                {
                    if (player.Player != null)
                    {
                        if (await ServerConnection.ConfirmPlayerRegistrationAsync((int)User.Id!, new TournamentPlayer()
                        {
                            Id = (int)player.Player.Id!,
                            TournamentId = (int)Tournament.Id!
                        }))
                        {
                            await RefreshPlayersAsync();
                        }
                    }
                }
            });
        }

        private RelayCommand? registerCmd;

        public RelayCommand RegisterCmd
        {
            get => registerCmd ?? new RelayCommand(async () => await Register());
        }

        private RelayCommand? unregisterCmd;

        public RelayCommand UnregisterCmd
        {
            get => unregisterCmd ?? new RelayCommand(async () => await Unregister());
        }

        private RelayCommand? closeRegistrationCmd;
        public RelayCommand CloseRegistrationCmd
        {
            get => closeRegistrationCmd ?? new RelayCommand(async () => await CloseRegistrationAsync());
        }

        private RelayCommand? cancelTournamentCmd;
        public RelayCommand CancelTournamentCmd
        {
            get => cancelTournamentCmd ?? new RelayCommand(async () =>
            {
                if (await ServerConnection.CancelTournamentAsync((int)User.Id!, Tournament.Id))
                {
                    TournamentStatus = "Cancelled";
                    IsMatchesEnabled = false;
                    IsClosingRegistrationPossible = false;

                    if(Tournament.BuyIn != null)
                    {
                        NeedPayback?.Invoke("Don't forget return buy-ins!");
                    }
                }
            });
        }
        #endregion

        private async Task Register()
        {
            if(await ServerConnection.RegisterAtTournamentAsync(User.Id, Tournament.Id))
            {

                await RefreshPlayersAsync();

                RegisteringCompleted?.Invoke("You are register");
            }
            else
            {
                RegisteringCompleted?.Invoke(ServerConnection.LastError);
            }
        }

        private async Task Unregister()
        {
            if (await ServerConnection.UnregisterFromTournamentAsync(User.Id, Tournament.Id))
            {

                await RefreshPlayersAsync();

                UnregisteringCompleted?.Invoke("You are unregister");
            }
            else
            {
                UnregisteringCompleted?.Invoke(ServerConnection.LastError ?? "Something happened. Try again later");
            }
        }

        private async Task CloseRegistrationAsync()
        {
            if (await TournamentBuilder.CloseTournamentRegistrationAsync((int)User.Id!, Tournament))
            {
                Tournament.Rounds = await ServerConnection.GetMatchesByTournamentIdAsync(Tournament.Id) ?? new List<List<MatchUpModel>>();
                await HandleByesAsync();
                await RefreshPlayersAsync();
                IsMatchesEnabled = true;
                IsClosingRegistrationPossible = false;
                IsRegistrationOpened = false;
                RegistrationClosed?.Invoke();
            }
        }

        private async Task HandleByesAsync()
        {
            if (Tournament.Rounds.Count == 0)
            {
                return;
            }

            foreach (var match in Tournament.Rounds[0].Where(m => m.Entries.Count == 1))
            {
                if(match.Entries[0].Player == null)
                {
                    continue;
                }

                match.Winner = match.Entries[0].Player;
                FrameModel frame = new FrameModel()
                {
                    MatchId = match.Id,
                    WinnerId = match.Entries[0].Player!.Id
                };

                frame.Entries.Add(new FrameEntryModel()
                {
                    PlayerId = match.Entries[0].Player!.Id
                });

                await ServerConnection.SaveFrameResultAsync((int)User.Id!, frame);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
