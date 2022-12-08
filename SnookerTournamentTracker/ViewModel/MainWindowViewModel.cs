using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private PersonModel user;
        private ICollectionView? tournamentsView;

        private string? name;
        public string? Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

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

        private TournamentModel? selectedTournament;
        public TournamentModel? SelectedTournament
        {
            get => selectedTournament;
            set
            {
                selectedTournament = value;
                OnPropertyChanged(nameof(SelectedTournament));
            }
        }

        public ObservableCollection<TournamentModel> Tournaments { get; } = new ObservableCollection<TournamentModel>();

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<string>? UpdateFailed;

        public MainWindowViewModel(PersonModel person)
        {
            this.user = person;
            ActiveOnly = true;
            Name = $"{person.FirstName ?? ""} {person.SecondName ?? ""} {person.LastName ?? ""}";
        }

        public async Task<bool> UpdateProfileAsync(PersonModel person)
        {
            person.OpenPassword = "1";
            if(await ServerConnection.UpdateProfileAsync((int)user.Id!, person))
            {
                Name = $"{person.FirstName ?? ""} {person.SecondName ?? ""} {person.LastName ?? ""}";
                return true;
            }

            OnUpdateFailed(ServerConnection.LastError);

            return false;
        }

        public async Task LoadData()
        {
            await Refresh();

            tournamentsView = CollectionViewSource.GetDefaultView(Tournaments);
            tournamentsView.Filter = (obj) =>
            {
                if (obj is TournamentModel tournament)
                {
                    if (tournament.Status == null)
                    {
                        return false;
                    }

                    return !ActiveOnly || !(tournament.Status.Equals("Finished") || tournament.Status.Equals("Cancelled"));
                }

                return false;
            };
        }

        public async Task Refresh()
        {
            List<TournamentModel>? tournaments = await ServerConnection.GetAllTournamentsAsync();

            if (tournaments != null)
            {
                Tournaments.Clear();

                foreach (var tournament in tournaments)
                {
                    Tournaments.Add(tournament);
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OnUpdateFailed(string message)
        {
            UpdateFailed?.Invoke(message);
        }
    }
}
