using GalaSoft.MvvmLight.Command;
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
    internal class CreateTournamentViewModel : INotifyPropertyChanged
    {
        //public string? Name { get; set; }

        //public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
        //public string? EntreeFee { get; set; }

        //public decimal? Garantee { get; set; }

        private TournamentModel tournament;
        public TournamentModel Tournament
        {
            get => tournament;
            set
            {
                tournament = value;
                OnPropertyChanged(nameof(Tournament));
            }
        }

        public PersonModel? SelectedPlayer { get; set; }

        public PersonModel? SelectedInvitedPlayer { get; set; }

        public ObservableCollection<PersonModel>? Players { get; set; }

        public ObservableCollection<PersonModel>? InvitedPlayers { get; set; }

        public List<PrizeModel>? Prizes { get; set; }
        public List<RoundModel>? Rounds { get; set; }

        private PersonModel? user;

        public CreateTournamentViewModel(PersonModel user)
        {
            this.user = user;
            tournament = new TournamentModel();

            var players = ServerConnection.GetAllPlayers((int)user.Id);

            if (players != null)
            {
                Players = new ObservableCollection<PersonModel>(players);
            }

            InvitedPlayers = new ObservableCollection<PersonModel>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private RelayCommand? addToInvitedCommand;
        public RelayCommand AddToInvitedCommand
        {
            get => addToInvitedCommand ?? new RelayCommand(() =>
            {
                if (SelectedPlayer != null)
                {
                    InvitedPlayers?.Add(SelectedPlayer);
                    Players?.Remove(SelectedPlayer);
                }
            });
        }

        private RelayCommand? removeFromInvitedCommand;
        public RelayCommand RemoveFromInvitedCommand
        {
            get => removeFromInvitedCommand ?? new RelayCommand(() =>
            {
                if (SelectedInvitedPlayer != null)
                {
                    Players?.Add(SelectedInvitedPlayer);
                    InvitedPlayers?.Remove(SelectedInvitedPlayer);
                }
            });
        }

        //private RelayCommand? createTournamentCommand;
        //public RelayCommand CreateTournamentCommand
        //{
        //    get => createTournamentCommand ?? new RelayCommand(() =>
        //    {
        //        CreateTournament();
        //    });
        //}

        public event Action? TournamentCreated;

        public bool CreateTournament()
        {
            if (Rounds == null)
            {
                Error = "Rounds are required";
                return false;
            }

            try
            {
                if (Validate())
                {
                    if (Prizes != null)
                    {
                        Tournament.Prizes = Prizes.Where(prize => prize.PrizeAmount != null).ToList();
                    }
                    Tournament.RoundModel = Rounds.Where(round => round.Frames != null).ToList();
                    if (ServerConnection.CreateTournament((int)user.Id, Tournament))
                    {
                        TournamentCreated?.Invoke();
                        return true;
                    }
                    else
                    {
                        Error = ServerConnection.LastError;
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                Error = "Cannot connect to the server";
                return false;
            }
        }

        private string? error = String.Empty;
        public string? Error
        {
            get => error;
            set
            {
                error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        private bool Validate()
        {
            //TODO add validation
            //if(Name == null || Name.Length == 0)
            //{
            //    return false;
            //}

            //if(StartDate != null && EndDate != null && StartDate > EndDate)
            //{
            //    return false;
            //}

            Error = String.Empty;

            if (Tournament.Name == null || Tournament.Name.Length == 0)
            {
                Error = "Name is required";
                return false;
            }

            if (Tournament.StartDate != null && Tournament.EndDate != null && Tournament.StartDate > Tournament.EndDate)
            {
                Error = "Start date cannot be after the end date";
                return false;
            }

            if(Tournament.Garantee != null && Prizes == null)
            {
                Error = "Total prizes cannot be less than garantee";
                return false;
            }

            if (Rounds == null || Rounds.Where(round => round.Frames == null || round.Frames == 0).Count() == Rounds.Count())
            {
                Error = "Rounds are required";
                return false;
            }


            //if (Tournament.Garantee != null && !decimal.TryParse(Tournament.Garantee, out decimal _))
            //{
            //    return false;
            //}

            //if (Tournament.EntreeFee != null && !decimal.TryParse(Tournament.EntreeFee, out decimal _))
            //{
            //    return false;
            //}

            //if(Garantee != null && !decimal.TryParse(Garantee, out decimal _))
            //{
            //    return false;
            //}

            //if (EntreeFee != null && !decimal.TryParse(EntreeFee, out decimal _))
            //{
            //    return false;
            //}

            return true;
        }
    }
}
