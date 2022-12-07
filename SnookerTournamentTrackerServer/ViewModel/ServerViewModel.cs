using GalaSoft.MvvmLight.Command;
using SnookerTournamentTrackerServer.DBAccess;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using TournamentLibrary;

namespace SnookerTournamentTrackerServer.ViewModel
{
    internal class ServerViewModel : INotifyPropertyChanged
    {
        private Server server;
        private ICollectionView? tournamentsView;

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


        public ObservableCollection<PersonModel> Users { get; } = new ObservableCollection<PersonModel>();
        public ObservableCollection<TournamentModel> Tournaments { get; } = new ObservableCollection<TournamentModel>();
        public ServerViewModel()
        {
            server = new Server(new DBAccesEntity());
            ActiveOnly = true;
            LoadData();
        }

        private async Task LoadData()
        {
            Tournaments.Clear();
            Users.Clear();

            var tournaments = await ServerConnection.GetAllTournamentsAsync();

            foreach(var tournament in tournaments)
            {
                Tournaments.Add(tournament);
            }

            tournamentsView = CollectionViewSource.GetDefaultView(Tournaments);
            tournamentsView.Filter = (obj) =>
            {
                if (obj is TournamentModel tournament)
                {
                    if (tournament.Status == null)
                    {
                        return false;
                    }

                    return !ActiveOnly || !tournament.Status.Equals("Finished");
                }

                return false;
            };

            var users = await ServerConnection.GetAllPlayersAsync();

            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        public void Close()
        {
            //server.Dispose();
        }

        private RelayCommand? refreshCmd;
        public RelayCommand RefreshCmd
        {
            get => refreshCmd ?? new RelayCommand(async () => await LoadData());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
