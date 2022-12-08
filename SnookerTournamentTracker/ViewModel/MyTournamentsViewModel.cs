using SnookerTournamentTracker.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class MyTournamentsViewModel : INotifyPropertyChanged
    {
        private PersonModel user;
        private List<TournamentModel>? playingTournaments;
        public List<TournamentModel>? PlayingTournaments
        {
            get => playingTournaments;
            set
            {
                playingTournaments = value;
                OnPropertyChanged(nameof(PlayingTournaments));
            }
        }

        public TournamentModel? SelectedPlayingTournament { get; set; }

        private List<TournamentModel>? administratingTournaments;
        public List<TournamentModel>? AdministratingTournaments
        {
            get => administratingTournaments;
            set
            {
                administratingTournaments = value;
                OnPropertyChanged(nameof(AdministratingTournaments));
            }
        }

        public TournamentModel? SelectedAdministratingTournament { get; set; }

        public MyTournamentsViewModel(PersonModel user)
        {
            this.user = user;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async Task RefreshAsync()
        {
            AdministratingTournaments = await ServerConnection.GetTournamentsByAdministratorIdAsync(user.Id);
            PlayingTournaments = await ServerConnection.GetTournamentsByPlayerIdAsync(user.Id);
        }
    }
}
