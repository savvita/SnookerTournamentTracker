using SnookerTournamentTracker.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class PrizesViewModel : INotifyPropertyChanged
    {
        private PrizesModel model;

        private ObservableCollection<string>? places;
        public ObservableCollection<string>? Places
        {
            get => places;
            set
            {
                places = value;
                OnPropertyChanged(nameof(Places));
            }
        }

        public PrizesViewModel(TournamentModel tournament)
        {
            Places = new ObservableCollection<string>(model.GetAllPlaces());
            model = new PrizesModel(tournament);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
