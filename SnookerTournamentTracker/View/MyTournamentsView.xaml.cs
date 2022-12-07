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
    /// Interaction logic for MyTournamentsView.xaml
    /// </summary>
    public partial class MyTournamentsView : Window
    {
        private MyTournamentsViewModel? model;
        private PersonModel? user;
        public MyTournamentsView()
        {
            InitializeComponent();

        }

        public MyTournamentsView(PersonModel user) : this()
        {
            this.user = user;
            model = new MyTournamentsViewModel(user);
            this.DataContext = model;
        }

        private void ViewPlayingTournamentBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO Open this on dbl click
            if(user == null || model == null || model.SelectedPlayingTournament == null)
            {
                return;
            }

            TournamentInfoView view = new TournamentInfoView(user, model.SelectedPlayingTournament);
            view.ShowDialog();
            model?.RefreshAsync();
        }

        private void ViewAdministratingTournamentBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO Open this on dbl click
            if (user == null || model == null || model.SelectedAdministratingTournament == null)
            {
                return;
            }

            TournamentInfoView view = new TournamentInfoView(user, model.SelectedAdministratingTournament);
            view.ShowDialog();
            model?.RefreshAsync();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //ViewAdministratingTournament_Click
    }
}
