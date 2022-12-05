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
            this.DataContext = model;
            this.Title = $"{tournament.Name} Matches";
        }

    }
}
