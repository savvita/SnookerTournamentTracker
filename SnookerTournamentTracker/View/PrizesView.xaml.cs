using SnookerTournamentTracker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TournamentLibrary;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for PrizesView.xaml
    /// </summary>
    public partial class PrizesView : Window
    {
        public PrizesView()
        {
            InitializeComponent();
        }

        public PrizesView(TournamentModel tournament) : this()
        {
            this.DataContext = new PrizesViewModel(tournament);
        }

        private void PrizeAmountTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(e.Text.Any(ch => !Char.IsDigit(ch) && ch != '.'))
            {
                e.Handled = true;
            }
        }
    }
}
