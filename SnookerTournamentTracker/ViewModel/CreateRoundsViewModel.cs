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
    internal class CreateRoundsViewModel : INotifyPropertyChanged
    {
        //public ObservableCollection<string>? Rounds { get; set; }

        public List<RoundModel>? Rounds { get; set; }

        private string error = String.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Error
        {
            get => error;
            set
            {
                error = value;
                OnPropertyChanged(nameof(Error));
            }
        }


        public string? SelectedRound { get; set; }

        public CreateRoundsViewModel(List<RoundModel>? rounds)
        {
            //Rounds = new ObservableCollection<string>(model.GetAllRounds());


            if (rounds != null)
            {
                Rounds = rounds;
            }
            else
            {
                Rounds = new List<RoundModel>();

                foreach (string round in ConnectionClientModel.GetAllRounds())
                {
                    Rounds.Add(new RoundModel() { Round = round });
                }
            }
        }



        public bool Validate()
        {
            Error = String.Empty;

            if(Rounds == null)
            {
                return false;
            }

            bool result = Rounds.All(round => round.Frames == null || round.Frames == 0 || round.Frames % 2 > 0);

            if (!result)
            {
                Error = "Match must consist of odd number of frames";
            }

            return result;
        }

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
