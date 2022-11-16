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
    /// Interaction logic for CreateRoundsView.xaml
    /// </summary>
    public partial class CreateRoundsView : Window
    {
        private CreateRoundsViewModel model;
        public List<RoundModel>? Rounds { get; set; }
        public CreateRoundsView()
        {
            InitializeComponent();
        }

        public CreateRoundsView(List<RoundModel>? rounds) : this()
        {
            model = new CreateRoundsViewModel(rounds);
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

            if(model.Validate())
            {
                this.Rounds = model.Rounds;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
