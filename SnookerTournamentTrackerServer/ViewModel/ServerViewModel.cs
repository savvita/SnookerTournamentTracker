using GalaSoft.MvvmLight.Command;
using Microsoft.EntityFrameworkCore;
using SnookerTournamentTrackerServer.DBAccess;
using SnookerTournamentTrackerServer.DbModel;
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


        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();
        public ObservableCollection<Tournament> Tournaments { get; } = new ObservableCollection<Tournament>();
        public ServerViewModel()
        {
            server = new Server(new DBAccesEntity());
            ActiveOnly = true;
            //LoadData();
        }

        public async Task LoadData()
        {
            Tournaments.Clear();
            Users.Clear();

            //DbSnookerTournamentTrackerContext db = new DbSnookerTournamentTrackerContext();
            DbSnookerTournamentTrackerSmarterContext db = new DbSnookerTournamentTrackerSmarterContext();
            
            await db.Tournaments.Include(t => t.TournamentStatus).LoadAsync();

            foreach (var tourney in db.Tournaments)
            {
                Tournaments.Add(tourney);
            }

            tournamentsView = CollectionViewSource.GetDefaultView(Tournaments);
            tournamentsView.Filter = (obj) =>
            {
                if (obj is Tournament tournament)
                {
                    if (tournament.TournamentStatus == null)
                    {
                        return false;
                    }

                    return !ActiveOnly || !(tournament.TournamentStatus.Status.Equals("Finished") || tournament.TournamentStatus.Status.Equals("Cancelled"));
                }

                return false;
            };

            await db.Users.LoadAsync();

            foreach (var user in db.Users)
            {
                Users.Add(user);
            }
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
