using SnookerTournamentTracker.ViewModel;
using SnookerTournamentTracker.LocalValidation;
using System.Windows;
using System.Windows.Controls;
using TournamentLibrary;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for EditTournamentView.xaml
    /// </summary>
    public partial class EditTournamentView : Window
    {
        public TournamentModel? Tournament;
        private EditTournamentViewModel? model;
        public EditTournamentView()
        {
            InitializeComponent();
        }

        public EditTournamentView(TournamentModel tournament) : this()
        {
            this.Tournament = tournament;
            this.model = new EditTournamentViewModel(tournament);
            this.Title = $"{tournament.Name}. Edit mode";
            this.DataContext = model;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Tournament = null;
            this.DialogResult = false;
            this.Close();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model == null)
            {
                return;
            }

            if (Validator.IsValid(this.TourneyPanel))
            {
                Tournament = model.Tournament;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                foreach (var element in this.TourneyPanel.Children)
                {
                    if (element is TextBox txt && Validation.GetHasError(txt))
                    {
                        Error.Content = Validation.GetErrors(txt)[0].ErrorContent.ToString();
                        break;
                    }
                }
            }

        }
    }
}
