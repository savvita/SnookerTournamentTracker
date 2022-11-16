using SnookerTournamentTracker.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ICollectionView? tournamentsView;
        public string Name { get; set; }

        private bool activeOnly;
        public bool ActiveOnly
        {
            get => activeOnly;
            set
            {
                activeOnly = value;
                tournamentsView?.Refresh();
                OnPropertyChanged(nameof(ActiveOnly));
            }
        }

        public TournamentModel? SelectedTournament { get; set; }


        private ObservableCollection<TournamentModel>? tournaments;

        public ObservableCollection<TournamentModel>? Tournaments
        {
            get => tournaments;
            private set
            {
                tournaments = value;
                OnPropertyChanged(nameof(Tournaments));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindowViewModel(PersonModel person)
        {
            ActiveOnly = true;
            Name = $"{person.FirstName ?? ""} {person.SecondName ?? ""} {person.LastName ?? ""}";
            Refresh();
            //Tournaments = new ObservableCollection<TournamentModel> (ConnectionClientModel.GetAllTournaments());
            //tournamentsView = CollectionViewSource.GetDefaultView(Tournaments);
            //tournamentsView.Filter = (obj) =>
            //{
            //    if(obj is TournamentModel tournament)
            //    {
            //        return !ActiveOnly || tournament.IsActive;
            //    }

            //    return false;
            //};
        }

        internal bool UpdateProfile(PersonModel user)
        {           
            if(ConnectionClientModel.UpdateProfile(user))
            {
                return true;
            }

            MessageBox.Show("Cannot update the profile", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            return false;
        }

        public void Refresh()
        {
            Tournaments = new ObservableCollection<TournamentModel>(ConnectionClientModel.GetAllTournaments());
            tournamentsView = CollectionViewSource.GetDefaultView(Tournaments);
            tournamentsView.Filter = (obj) =>
            {
                if (obj is TournamentModel tournament)
                {
                    return !ActiveOnly || tournament.IsActive;
                }

                return false;
            };
        }

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
