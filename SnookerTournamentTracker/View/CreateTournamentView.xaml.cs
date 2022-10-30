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

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for CreateTournamentView.xaml
    /// </summary>
    public partial class CreateTournamentView : Window
    {
        public CreateTournamentView()
        {
            InitializeComponent();
            this.DataContext = new CreateTournamentViewModel();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnlyNumbersTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(ch => !Char.IsDigit(ch) && ch != '.'))
            {
                e.Handled = true;
            }
        }
    }
}
