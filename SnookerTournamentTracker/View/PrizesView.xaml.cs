using SnookerTournamentTracker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TournamentLibrary;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for PrizesView.xaml
    /// </summary>
    public partial class PrizesView : Window
    {
        private CreatePrizesViewModel? model;

        public List<PrizeModel>? Prizes { get; private set; }
        public PrizesModeEnum Mode { get; set; }
        public PrizesView()
        {
            InitializeComponent();
        }

        public PrizesView(TournamentModel tournament, List<PrizeModel>? prizes) : this()
        {
            model = new CreatePrizesViewModel(tournament, prizes);
            this.DataContext = model;
        }


        private void PrizeAmountTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(e.Text.Any(ch => !Char.IsDigit(ch) && ch != '.'))
            {
                e.Handled = true;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void PrizeAmountTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            model?.Refresh();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if(model != null && model.Validate())
            {
                Prizes = model.Prizes;
                Mode = model.Mode;
                this.DialogResult = true;
            }
        }
    }
}
