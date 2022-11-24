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
    /// Interaction logic for TournamentInfoView.xaml
    /// </summary>
    public partial class TournamentInfoView : Window
    {
        public TournamentInfoView()
        {
            InitializeComponent();
        }

        public TournamentInfoView(PersonModel user, TournamentModel tournament) : this()
        {
            TournamentViewViewModel model = new TournamentViewViewModel(user, tournament);
            model.RegisteringCompleted += (msg) => MessageBox.Show(msg, "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
            model.UnregisteringCompleted += (msg) => MessageBox.Show(msg, "Unregistration", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DataContext = model;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
