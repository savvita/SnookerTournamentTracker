using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class EditTournamentViewModel : INotifyPropertyChanged
    {

        public TournamentModel? Tournament { get; set; }

        private string error = String.Empty;

        public string Error
        {
            get => error;
            set
            {
                error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public EditTournamentViewModel(TournamentModel tournament)
        {
            Tournament = new TournamentModel()
            {
                Id = tournament.Id,
                Name = tournament.Name,
                StartDate = tournament.StartDate,
                EndDate = tournament.EndDate,
                BuyIn = tournament.BuyIn,
                Garantee = tournament.Garantee,
                PaymentInfo = tournament.PaymentInfo,
                Status = tournament.Status
            };
        }

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
