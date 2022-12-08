using SnookerTournamentTrackerServer.ViewModel;
using System.Windows;

namespace SnookerTournamentTrackerServer.View
{
    /// <summary>
    /// Interaction logic for ServerView.xaml
    /// </summary>
    public partial class ServerView : Window
    {
        private ServerViewModel model;
        public ServerView()
        {
            InitializeComponent();
            model = new ServerViewModel();
            this.DataContext = model;
            this.Loaded += (async (obj, e) => await model.LoadData());
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
