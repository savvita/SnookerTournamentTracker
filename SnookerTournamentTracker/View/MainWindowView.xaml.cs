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
            this.DataContext = model;
        }

        private void CreateTournamentBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateTournamentView view = new CreateTournamentView();
            view.ShowDialog();

            if(view.DialogResult == true)
            {
                model?.Refresh();
            }
            //(new CreateTournamentView()).ShowDialog();
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (user != null)
            {
                UserProfileView view = new UserProfileView(user);
                view.ShowDialog();

                if(view.DialogResult == true)
                {
                    model?.UpdateProfile(user);
                }
            }
        }

        private void ViewTournamentBtn_Click(object sender, RoutedEventArgs e)
        {
            if(model == null || model.SelectedTournament == null)
            {
                return;
            }

            TournamentInfoView view = new TournamentInfoView(model.SelectedTournament);
            view.ShowDialog();
        }
    }
}
