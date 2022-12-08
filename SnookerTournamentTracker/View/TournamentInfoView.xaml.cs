using SnookerTournamentTracker.ViewModel;
using System.Windows;
using TournamentLibrary;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for TournamentInfoView.xaml
    /// </summary>
    public partial class TournamentInfoView : Window
    {
        // TODO register/unregister - show only one of them
        // TODO tournament name in xaml - add wrapping text
        public TournamentInfoView()
        {
            InitializeComponent();
        }

        private TournamentViewViewModel? model;
        private PersonModel? user; 

        public TournamentInfoView(PersonModel user, TournamentModel tournament) : this()
        {
            this.user = user;
            model = new TournamentViewViewModel(user, tournament);
            this.Title = tournament.Name;
            model.RegisteringCompleted += (msg) => MessageBox.Show(msg, "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
            model.UnregisteringCompleted += (msg) => MessageBox.Show(msg, "Unregistration", MessageBoxButton.OK, MessageBoxImage.Information);
            model.NeedPayback += (msg) => MessageBox.Show(msg, "Cancel tournament", MessageBoxButton.OK, MessageBoxImage.Information);

            model.RegistrationClosed += () =>
            {
                new MatchesView(user, tournament).ShowDialog();
                model?.RefreshPlayersAsync();
            };

            this.DataContext = model;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ViewMatchesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (user != null && model != null) 
            {
                MatchesView view = new MatchesView(user, model.Tournament);
                view.ShowDialog();
                model?.RefreshPlayersAsync();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model != null && model.Tournament != null)
            {
                EditTournamentView view = new EditTournamentView(model.Tournament);
                view.ShowDialog();

                if (view.DialogResult == true && view.Tournament != null)
                {
                    model.Tournament = view.Tournament;
                    model?.UpdateTournamentAsync(model.Tournament);
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
    }
}
