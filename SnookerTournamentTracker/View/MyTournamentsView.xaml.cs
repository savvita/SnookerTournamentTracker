using SnookerTournamentTracker.ViewModel;
using System.Windows;
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
            this.Loaded += async (obj, e) => await model.RefreshAsync();
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
    }
}
