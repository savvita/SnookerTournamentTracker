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
    /// Interaction logic for CreateTournamentView.xaml
    /// </summary>
    public partial class CreateTournamentView : Window
    {
        private CreateTournamentViewModel model;

        public CreateTournamentView()
        {
            InitializeComponent();
            model = new CreateTournamentViewModel();
            this.DataContext = model;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OnlyNumbersTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(ch => !Char.IsDigit(ch) && ch != '.'))
            {
                e.Handled = true;
            }
        }

        private void CreatePrizesBtn_Click(object sender, RoutedEventArgs e)
        {
            PrizesView view = new PrizesView(model.Tournament, model.Prizes);
            view.ShowDialog();

            if (view.DialogResult == true)
            {
                model.Prizes = view.Prizes;
                model.Tournament.PrizeMode = view.Mode;
            }
        }

        private void CreateTournamentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model.CreateTournament())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CreateRoundsBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateRoundsView view = new CreateRoundsView(model.Rounds);
            view.ShowDialog();

            if (view.DialogResult == true)
            {
                model.Rounds = view.Rounds;
            }
        }
    }
}
