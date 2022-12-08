using SnookerTournamentTracker.ViewModel;
using System.Windows;
using TournamentLibrary;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for MatchesView.xaml
    /// </summary>
    public partial class MatchesView : Window
    {
        private MatchesViewModel? model;
        public MatchesView()
        {
            InitializeComponent();
        }

        public MatchesView(PersonModel user, TournamentModel tournament) : this()
        {
            model = new MatchesViewModel(user, tournament);
            this.Loaded += async (obj, e) => await model.LoadData();
            this.DataContext = model;

            this.Title = $"{tournament.Name} Matches";
        }

    }
}
