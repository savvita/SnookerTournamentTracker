using SnookerTournamentTracker.LocalValidation;
using SnookerTournamentTracker.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for AuthorizationView.xaml
    /// </summary>
    public partial class AuthorizationView : Window
    {
        public AuthorizationView()
        {
            InitializeComponent();
            AuthorizationViewModel model = new AuthorizationViewModel();
            model.Authorizated += ((person) => { 
                new MainWindowView(person).Show();
                this.Close();
            });
            this.DataContext = model;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SignInBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Validator.IsValid(this.SignIn);

        }

        private void SignUpBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Validator.IsValid(this.SignUp);
        }
    }
}
