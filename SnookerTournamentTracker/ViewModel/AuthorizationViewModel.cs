using GalaSoft.MvvmLight.Command;
using SnookerTournamentTracker.Security;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class AuthorizationViewModel : INotifyPropertyChanged
    {
        private SecureString? password;
        public SecureString? Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private SecureString? passwordConfirm;
        public SecureString? PasswordConfirm
        {
            get => passwordConfirm;
            set
            {
                passwordConfirm = value;
                OnPropertyChanged(nameof(PasswordConfirm));
            }
        }

        private string? firstName;
        public string? FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string? secondName;
        public string? SecondName
        {
            get => secondName;
            set
            {
                secondName = value;
                OnPropertyChanged(nameof(SecondName));
            }
        }

        private string? lastName;
        public string? LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        private string? email;
        public string? Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string? phoneNumber;
        public string? PhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        private string? error = String.Empty;
        public string? Error
        {
            get => error;
            set
            {
                error = value;
                OnPropertyChanged(nameof(Error));
            }
        }


        public event Action<PersonModel>? Authorizated;
        public event PropertyChangedEventHandler? PropertyChanged;


        #region Commands

        private RelayCommand? signInCommand;

        public RelayCommand SignInCommand
        {
            get => signInCommand ?? new RelayCommand(async () => await SignInAsync());
        }

        private RelayCommand? signUpCommand;

        public RelayCommand SignUpCommand
        {
            get => signUpCommand ?? new RelayCommand(async () => await SignUpAsync());
        }

        private RelayCommand? clearCommand;

        public RelayCommand ClearCommand
        {
            get => clearCommand ?? new RelayCommand(Clear);
        } 
        #endregion

        private async Task SignInAsync()
        {
            try
            {
                if (Validate())
                {
                    PersonModel person = new PersonModel() 
                    { 
                        EmailAddress = Email, 
                        OpenPassword = Passwords.SecureStringToString(Password) 
                    };

                    if (await ServerConnection.SignInAsync(person))
                    {
                        OnAuthorizated(person);
                    }
                    else
                    {
                        Error = ServerConnection.LastError;
                    }

                    if (person.OpenPassword != null)
                    {
                        int gen = GC.GetGeneration(person.OpenPassword);
                        person.OpenPassword = null;
                        GC.Collect(gen);
                    }
                }
            }
            catch
            {
                Error = "Cannot connect to the server";
            }
        }

        private async Task SignUpAsync()
        {
            try
            {
                if (Validate(false))
                {
                    PersonModel person = new PersonModel()
                    {
                        FirstName = FirstName,
                        SecondName = SecondName,
                        LastName = LastName,
                        EmailAddress = Email,
                        PhoneNumber = PhoneNumber,
                        OpenPassword = Passwords.SecureStringToString(Password)
                    };

                    if (await ServerConnection.SignUpAsync(person))
                    {
                        Password?.Dispose();
                        PasswordConfirm?.Dispose();

                        if (person.OpenPassword != null)
                        {
                            int gen = GC.GetGeneration(person.OpenPassword);
                            person.OpenPassword = null;
                            GC.Collect(gen);
                        }

                        OnAuthorizated(person);
                    }
                    else
                    {
                        Error = ServerConnection.LastError;
                    }
                }
            }
            catch
            {
                Error = "Cannot connect to the server";
            }
        }

        private bool Validate(bool signIn = true)
        {
            Error = String.Empty;

            if (string.IsNullOrEmpty(Email) || Password == null || Password.Length == 0)
            {
                Error = "Fill all the required fields";
                return false;
            }

            if(!signIn)
            {
                if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) 
                    || string.IsNullOrEmpty(Email) || Password == null || PasswordConfirm == null)
                {
                    Error = "Fill all the required fields";
                    return false;
                }

                if(!Passwords.ComparePasswords(Password, PasswordConfirm))
                {
                    Error = "Passwords do not match";
                    return false;
                }
            }

            return true;
        }

        private void Clear()
        {
            Password = null;
            PasswordConfirm = null;
            FirstName = null;
            SecondName = null;
            LastName = null;
            Email = null;
            PhoneNumber = null;
            Error = String.Empty;
        }


        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OnAuthorizated(PersonModel model)
        {
            Authorizated?.Invoke(model);
        }
    }
}
