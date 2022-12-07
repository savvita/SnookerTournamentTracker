using SnookerTournamentTracker.LocalValidation;
using SnookerTournamentTracker.ViewModel;
using System.Windows;
using System.Windows.Controls;
using TournamentLibrary;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for UserProfileView.xaml
    /// </summary>
    public partial class UserProfileView : Window
    {
        private UserProfileViewModel? model;

        public PersonModel? User { get; private set; }

        public UserProfileView()
        {
            InitializeComponent();
        }

        public UserProfileView(PersonModel user) : this()
        {
            model = new UserProfileViewModel(user);
            this.DataContext = model;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            User = null;
            this.DialogResult = false;
            this.Close();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model == null)
            {
                return;
            }

            if (Validator.IsValid(this.UserProfile))
            {
                User = model.User;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                foreach(var element in this.UserProfile.Children)
                {
                    if(element is TextBox txt && Validation.GetHasError(txt))
                    {
                        Error.Content = Validation.GetErrors(txt)[0].ErrorContent.ToString();
                        break;
                    }
                }
            }
             
        }
    }
}
