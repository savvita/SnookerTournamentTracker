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
                LoadData();
            }


            Refresh();
        }

        private async Task LoadData()
        {
            Prizes = await ServerConnection.GetAllPlacesAsync();
            Mode = PrizesModeEnum.Absolute;
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

                for (int i = 0; i < Prizes.Count; i++)
                {
                    if (Prizes[i].PrizeAmount != null)
                    {
                        Total += (double)Prizes[i].PrizeAmount! * Math.Pow(2, Math.Max(i - 1, 0));
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
