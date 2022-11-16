using GalaSoft.MvvmLight.Command;
using SnookerTournamentTracker.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class AuthorizationViewModel : INotifyPropertyChanged
    {
        private string? login;
        public string? Login
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged(nameof(Login));
            }
        }

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

        private RelayCommand? signInCommand;

        public RelayCommand SignInCommand
        {
            get => signInCommand ?? new RelayCommand(SignIn);
        }

        private RelayCommand? clearCommand;

        public RelayCommand ClearCommand
        {
            get => clearCommand ?? new RelayCommand(Clear);
        }

        private void Clear()
        {
            Login = null;
            Password = null;
            PasswordConfirm = null;
            FirstName = null;
            SecondName = null;
            LastName = null;
            Email = null;
            PhoneNumber = null;
            Error = String.Empty;
        }

        public event Action<PersonModel>? Authorizated;

        private void OnAuthorizated(PersonModel model)
        {
            Authorizated?.Invoke(model);
        }

        private async void SignIn()
        {
            //TODO - Add error handling
            //TODO - Show server responce when failed
            if (Validate())
            {
                PersonModel person = new PersonModel() { Login = Login, Password = Password };
                //if (ConnectionClientModel.SignIn(person, out error))
                if (await ConnectionClientModel.SignIn(person))
                {
                    //TODO - Add going to the next window
                    OnAuthorizated(person);

                }
                else
                {
                    OnPropertyChanged(nameof(Error));
                }
            }
        }

        private RelayCommand? signUpCommand;

        public RelayCommand SignUpCommand
        {
            get => signUpCommand ?? new RelayCommand(SignUp);
        }

        private void SignUp()
        {
            //TODO - Add error handling
            //TODO - Show server responce when failed
            if (Validate(false))
            {
                PersonModel person = new PersonModel()
                {
                    FirstName = FirstName,
                    SecondName = SecondName,
                    LastName = LastName,
                    EmailAddress = Email,
                    PhoneNumber = PhoneNumber,
                    Login = Login,
                    Password = Password
                };

                if (ConnectionClientModel.SignUp(person, out error))
                {
                    //TODO - Add going to the next window
                    Password?.Dispose();
                    PasswordConfirm?.Dispose();
                    OnAuthorizated(person);

                }
                else
                {
                    OnPropertyChanged(nameof(Error));
                }
            }
        }

        private bool Validate(bool signIn = true)
        {
            Error = String.Empty;

            if (string.IsNullOrEmpty(Login) || Password == null || Password.Length == 0)
            {
                Error = "Fill all the required fields";
                return false;
            }

            if(!signIn)
            {
                if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Email))
                {
                    Error = "Fill all the required fields";
                    return false;
                }

                string? pass = SecureStringToString(Password);
                string? passConf = SecureStringToString(PasswordConfirm);

                if (pass != passConf)
                {
                    Error += "Passwords do not match";
                    return false;
                }

                if (pass != null)
                {
                    GC.Collect(GC.GetGeneration(pass));
                }
            }

            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string? SecureStringToString(SecureString? value)
        {
            if (value == null)
            {
                return null;
            }

            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

    }
}
