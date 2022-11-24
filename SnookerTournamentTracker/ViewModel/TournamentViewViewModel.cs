using GalaSoft.MvvmLight.CommandWpf;
using SnookerTournamentTracker.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class TournamentViewViewModel
    {
        public TournamentModel Tournament { get; set; }
        private PersonModel user;

        public ObservableCollection<PersonModel> Players { get; set; }

        public event Action<string>? RegisteringCompleted;
        public event Action<string>? UnregisteringCompleted;

        public TournamentViewViewModel(PersonModel user, TournamentModel tournament)
        {
            this.user = user;
            Tournament = tournament;
            Players = new ObservableCollection<PersonModel>(Tournament.Players);
        }

        private RelayCommand? registerCmd;
        
        public RelayCommand RegisterCmd
        {
            get => registerCmd ?? new RelayCommand(Register);
        }

        private void Register()
        {
            //TODO finish this - subscribe to event at the view and show msgbox
            if(Tournament.Players.FindIndex(pl => pl.Id == user.Id) == -1)
            {
                Tournament.Players.Add(user);
                Players.Add(user);
                //TODO add adding to db
                RegisteringCompleted?.Invoke("You are register");
            }
            else
            {
                RegisteringCompleted?.Invoke("You are already registered at this tournament");
            }
        }

        private RelayCommand? unregisterCmd;

        public RelayCommand UnregisterCmd
        {
            get => unregisterCmd ?? new RelayCommand(Unregister);
        }

        private void Unregister()
        {
            int idx = Tournament.Players.FindIndex(pl => pl.Id == user.Id);
            //TODO finish this - subscribe to event at the view and show msgbox
            if (idx != -1)
            {
                Tournament.Players.RemoveAt(idx);
                Players.RemoveAt(idx);
                //TODO add removing from db
                RegisteringCompleted?.Invoke("You are unregister");
            }
            else
            {
                RegisteringCompleted?.Invoke("You are not registered at this tournament");
            }
        }
    }
}
