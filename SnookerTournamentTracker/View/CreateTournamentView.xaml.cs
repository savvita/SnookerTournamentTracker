using SnookerTournamentTracker.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TournamentLibrary;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for CreateTournamentView.xaml
    /// </summary>
    public partial class CreateTournamentView : Window
    {
        private CreateTournamentViewModel? model;

        public CreateTournamentView()
        {
            InitializeComponent();
        }

        public CreateTournamentView(PersonModel user) : this()
        {
            if(user.Id != null)
            {
                model = new CreateTournamentViewModel(user);
                this.Loaded += async (obj, e) => await model.LoadData();
                this.DataContext = model;
            }

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

        private void OnlyDigitsTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(ch => !Char.IsDigit(ch)))
            {
                e.Handled = true;
            }
        }

        private void CreatePrizesBtn_Click(object sender, RoutedEventArgs e)
        {
            if(model == null)
            {
                return;
            }

            PrizesView view = new PrizesView(model.Tournament, model.Prizes);
            view.ShowDialog();

            if (view.DialogResult == true)
            {
                model.Prizes = view.Prizes;
                model.Tournament.PrizeMode = view.Mode;
            }
        }

        private async void CreateTournamentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model == null)
            {
                return;
            }

            if (await model.CreateTournamentAsync())
            {
                this.DialogResult = true;
                this.Close();
            }
        }


        private void CreateRoundsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model == null)
            {
                return;
            }

            CreateRoundsView view = new CreateRoundsView(model.Rounds);
            view.ShowDialog();

            if (view.DialogResult == true)
            {
                model.Rounds = view.Rounds;
            }
        }
    }
}
