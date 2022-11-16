using GalaSoft.MvvmLight.Command;
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
    internal class UserProfileViewModel : INotifyPropertyChanged
    {
        //public string? Login { get; private set; }
        //public string? FirstName { get; set; }
        //public string? SecondName { get; set; }
        //public string? LastName { get; set; }
        //public string? EmailAddress { get; set; }
        //public string? PhoneNumber { get; set; }

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
            User = person;
            //Login = person.Login;
            //FirstName = person.FirstName;
            //SecondName = person.SecondName;
            //LastName = person.LastName;
            //EmailAddress = person.EmailAddress;
            //PhoneNumber = person.PhoneNumber;
        }

        //public event Action? ChangesAccepted;

        //private bool Validate()
        //{
        //    if(User == null)
        //    {
        //        return false;
        //    }

        //    Error = String.Empty;

        //    if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(EmailAddress))
        //    {
        //        Error = "Fill all the required fields";
        //        return false;
        //    }

        //    return true;
        //}

        //private RelayCommand? acceptChangesCommand;

        //public RelayCommand AcceptChangesCommand
        //{
        //    get => acceptChangesCommand ?? new RelayCommand(AcceptChanges);
        //}

        //private void AcceptChanges()
        //{
        //    if(Validate() && User != null)
        //    {
        //        User.FirstName = FirstName;
        //        User.SecondName = SecondName;
        //        User.LastName = LastName;
        //        User.PhoneNumber = PhoneNumber;
        //        User.EmailAddress = EmailAddress;
        //        ChangesAccepted?.Invoke();
        //    }
        //}


        public event PropertyChangedEventHandler? PropertyChanged;



        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
