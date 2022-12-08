using SnookerTournamentTracker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TournamentLibrary;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for CreateRoundsView.xaml
    /// </summary>
    public partial class CreateRoundsView : Window
    {
        private CreateRoundsViewModel? model;
        public List<RoundModel>? Rounds { get; set; }
        public CreateRoundsView()
        {
            InitializeComponent();
        }

        public CreateRoundsView(List<RoundModel>? rounds) : this()
        {
            model = new CreateRoundsViewModel();
            this.Loaded += async (obj, e) => await model.LoadData(rounds);
            this.DataContext = model;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OnlyNumbersTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(ch => !Char.IsDigit(ch)))
            {
                e.Handled = true;
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if(model == null)
            {
                this.DialogResult = false;
                this.Close();
                return;
            }

            this.Rounds = model.GetRounds();
            this.DialogResult = true;
            this.Close();
        }
    }
}
