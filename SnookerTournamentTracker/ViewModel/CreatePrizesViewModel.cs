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
    internal class CreatePrizesViewModel : INotifyPropertyChanged
    {

        private PrizesModeEnum mode;
        public PrizesModeEnum Mode
        {
            get => mode;
            set
            {
                mode = value;
                Refresh();
                OnPropertyChanged(nameof(Mode));
            }
        }

        private double total;
        public double Total
        {
            get => total;
            set
            {
                total = value;
                OnPropertyChanged(nameof(Total));
            }
        }

        private double rest;
        public double Rest
        {
            get => rest;
            set
            {
                rest = value;
                OnPropertyChanged(nameof(Rest));
            }
        }

        private List<PrizeModel>? prizes;
        public List<PrizeModel>? Prizes
        {
            get => prizes;
            set
            {
                prizes = value;
                OnPropertyChanged(nameof(Prizes));
            }
        }

        private PrizeModel? selectedPrize;
        public PrizeModel? SelectedPrize
        {
            get => selectedPrize;
            set
            {
                selectedPrize = value;
                Refresh();
                OnPropertyChanged(nameof(SelectedPrize));
            }
        }

        private decimal? garantee;

        public CreatePrizesViewModel(TournamentModel tournament, List<PrizeModel>? prizes)
        {
            this.garantee = tournament.Garantee;

            if (prizes != null)
            {
                Prizes = prizes;
                Mode = tournament.PrizeMode;
            }
            else
            {
                Prizes = ConnectionClientModel.GetAllPlaces();
                Mode = PrizesModeEnum.Absolute;
            }


            Refresh();
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

        public void Refresh()
        {
            GetTotal();
            GetRest();
        }

        private void GetTotal()
        {
            if(Prizes != null)
            {
                Total = 0;
                foreach(PrizeModel prize in Prizes)
                {
                    if (prize.PrizeAmount != null)
                    {
                        Total += (double)prize.PrizeAmount;
                    }
                }

            }
        }

        private void GetRest()
        {
            if (Prizes != null)
            {
                if (Mode == PrizesModeEnum.Percentage)
                {
                    Rest = 100 - (double)Total;
                }


                else if (garantee != null)
                {
                    double diff = (double)garantee - total;
                    Rest = diff > 0 ? diff : 0;
                }

            }
        }

        public bool Validate()
        {
            Error = String.Empty;

            if (Mode == PrizesModeEnum.Absolute && garantee != null && Rest > 0)
            {
                Error = "Total prizes cannot be less than garantee";
                return false;
            }

            else if(Mode == PrizesModeEnum.Percentage && Total > 100)
            {
                Error = "Total prizes cannot be more than 100%";
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
