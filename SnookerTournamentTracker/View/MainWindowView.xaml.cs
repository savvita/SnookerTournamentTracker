using SnookerTournamentTracker.ViewModel;
using System.Windows;
using TournamentLibrary;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private PersonModel? user;
        private MainWindowViewModel? model;
        public MainWindowView()
        {
            InitializeComponent();
        }

        public MainWindowView(PersonModel user) : this()
        {
            this.user = user;
            model = new MainWindowViewModel(user);
            model.UpdateFailed += (msg) => MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            this.Loaded += async (obj, e) => await model.LoadData();

            this.DataContext = model;
        }

        private void CreateTournamentBtn_Click(object sender, RoutedEventArgs e)
        {
            if(user == null)
            {
                return;
            }

            CreateTournamentView view = new CreateTournamentView(user);
            view.ShowDialog();

            if(view.DialogResult == true)
            {
                model?.Refresh();
            }
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (user != null)
            {
                UserProfileView view = new UserProfileView(user);
                view.ShowDialog();

                if (view.DialogResult == true && view.User != null)
                {
                    user = view.User;
                    model?.UpdateProfileAsync(user);
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void ViewTournamentBtn_Click(object sender, RoutedEventArgs e)
        {
            //TODO open this on dbl click
            if(user == null || model == null || model.SelectedTournament == null)
            {
                return;
            }

            TournamentInfoView view = new TournamentInfoView(user, model.SelectedTournament);
            view.ShowDialog();
            model?.Refresh();
        }

        private void MyTournamentsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (user == null)
            {
                return;
            }

            MyTournamentsView view = new MyTournamentsView(user);
            view.ShowDialog();
            model?.Refresh();
        }
    }
}
