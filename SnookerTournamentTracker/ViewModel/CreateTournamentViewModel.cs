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
        public string? Name { get; set; }
        private CreateTournamentModel model = new CreateTournamentModel();

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? EntreeFee { get; set; }

        public string? Garantee { get; set; }

        public PersonModel? SelectedPlayer { get; set; }

        public PersonModel? SelectedInvitedPlayer { get; set; }

        public ObservableCollection<PersonModel>? Players { get; set; }

        public ObservableCollection<PersonModel>? InvitedPlayers { get; set; }

        public CreateTournamentViewModel()
        {
            Players = new ObservableCollection<PersonModel>(model.GetAllPlayers());
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

        private RelayCommand? createTournamentCommand;
        public RelayCommand CreateTournamentCommand
        {
            get => createTournamentCommand ?? new RelayCommand(() =>
            {
                CreateTournament();
            });
        }

        private bool CreateTournament()
        {
            // TODO add validation
            // TODO show errors
            if (Validate())
            {
                return model.CreateTournament(new TournamentModel()
                {
                    Name = Name,
                    EntryFee = Decimal.TryParse(EntreeFee, out decimal amount) ? amount : null,
                    Garantee = Decimal.TryParse(EntreeFee, out decimal garantee) ? garantee : null
                });
            }

            return false;
        }

        private bool Validate()
        {
            //TODO add validation
            if(Name == null || Name.Length == 0)
            {
                return false;
            }

            if(StartDate != null && EndDate != null && StartDate > EndDate)
            {
                return false;
            }

            if(Garantee != null && !decimal.TryParse(Garantee, out decimal _))
            {
                return false;
            }

            if (EntreeFee != null && !decimal.TryParse(EntreeFee, out decimal _))
            {
                return false;
            }

            return true;
        }
    }
}
