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
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class TournamentViewViewModel : INotifyPropertyChanged
    {
        public TournamentModel Tournament { get; set; }
        private PersonModel user;

        private List<PersonModel>? players;
        public List<PersonModel>? Players
        {
            get => players;
            set
            {
                players = value;
                OnPropertyChanged(nameof(Players));
            }
        }

        public bool IsEditable { get; }

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

        public event Action<string>? RegisteringCompleted;
        public event Action<string>? UnregisteringCompleted;
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public TournamentViewViewModel(PersonModel user, TournamentModel tournament)
        {
            this.user = user;
            Tournament = tournament;

            var rounds = ServerConnection.GetRoundsByTournamentId(tournament.Id);

            if (rounds != null)
            {
                Tournament.RoundModel = rounds;
            }

            Tournament.Prizes = ServerConnection.GetPrizesByTournamentId(tournament.Id);

            var players = ServerConnection.GetPlayersByTournamentId(tournament.Id);

            if (players != null)
            {
                Tournament.Players = players;
            }

            Players = Tournament.Players;
            IsEditable = ServerConnection.IsTournamentAdministrator(user.Id, tournament.Id);

            IsRegistrationOpened = tournament.Status.Equals("Registration");
            IsMatchesEnabled = tournament.Status != null ? !tournament.Status.Equals("Registration") : false;
            IsClosingRegistrationPossible = IsEditable && !IsMatchesEnabled;
        }

        private RelayCommand? registerCmd;
        
        public RelayCommand RegisterCmd
        {
            get => registerCmd ?? new RelayCommand(Register);
        }

        private void Register()
        {
            //TODO finish this - subscribe to event at the view and show msgbox
            //TODO do this normal
            if(Tournament.Players.FindIndex(pl => pl.Id == user.Id) == -1 && ServerConnection.RegisterAtTournament(user, Tournament))
            {
                var players = ServerConnection.GetPlayersByTournamentId(Tournament.Id);

                if (players != null)
                {
                    Tournament.Players = players;
                    Players = players;
                }

                RegisteringCompleted?.Invoke("You are register");
            }
            else
            {
                //TODO add error msg for server error
                RegisteringCompleted?.Invoke("You are already registered at this tournament");
            }
        }

        private RelayCommand? unregisterCmd;

        public RelayCommand UnregisterCmd
        {
            get => unregisterCmd ?? new RelayCommand(Unregister);
        }

        private void Unregister()
        {
            //TODO finish this - subscribe to event at the view and show msgbox
            //TODO do this normal
            if (ServerConnection.UnregisterFromTournament(user, Tournament))
            {
                var players = ServerConnection.GetPlayersByTournamentId(Tournament.Id);

                if (players != null)
                {
                    Tournament.Players = players;
                    Players = players;
                }

                UnregisteringCompleted?.Invoke("You are unregister");
            }
            else
            {
                //TODO add error msg for server error
                UnregisteringCompleted?.Invoke(ServerConnection.LastError ?? "Something happened. Try again later");
            }
        }

        private RelayCommand? closeRegistrationCmd;
        public RelayCommand CloseRegistrationCmd
        {
            get => closeRegistrationCmd ?? new RelayCommand(CloseRegistration);
        }

        public event Action? RegistrationClosed;

        private void CloseRegistration()
        {
            ConnectionClientModel.CloseTournamentRegistration((int)user.Id, Tournament);
            IsMatchesEnabled = true;
            IsClosingRegistrationPossible = false;
            IsRegistrationOpened = false;
            RegistrationClosed?.Invoke();
        }
    }
}
