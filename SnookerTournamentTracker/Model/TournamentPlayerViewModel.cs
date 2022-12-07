using System.ComponentModel;
using System.Runtime.CompilerServices;
using TournamentLibrary;

namespace SnookerTournamentTracker
{
    internal class TournamentPlayerViewModel : INotifyPropertyChanged
    {
        public PersonModel? Player { get; set; }

        private string? result;
        public string? Result
        {
            get => result;
            set
            {
                result = value;
                OnPropertyChanged(nameof(Result));
            }
        }


        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
