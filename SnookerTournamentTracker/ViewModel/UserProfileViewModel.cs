using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class UserProfileViewModel : INotifyPropertyChanged
    {
        public PersonModel? User { get; set; }

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

        public UserProfileViewModel(PersonModel person)
        {
            User = new PersonModel()
            {
                Id = person.Id,
                FirstName = person.FirstName,
                SecondName = person.SecondName,
                LastName = person.LastName,
                EmailAddress = person.EmailAddress,
                PhoneNumber = person.PhoneNumber
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
