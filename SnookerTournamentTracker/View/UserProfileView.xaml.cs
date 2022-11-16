using SnookerTournamentTracker.LocalValidation;
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
    /// Interaction logic for UserProfileView.xaml
    /// </summary>
    public partial class UserProfileView : Window
    {
        private UserProfileViewModel? model;
        public UserProfileView()
        {
            InitializeComponent();
        }

        public UserProfileView(PersonModel user) : this()
        {
            model = new UserProfileViewModel(user);
            //model.ChangesAccepted += () =>
            //{
            //    this.DialogResult = true;
            //    this.Close();
            //};
            this.DataContext = model;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model == null)
            {
                return;
            }


            //if (model.Validate())
            //{
            //    this.DialogResult = true;
            //    this.Close();
            //}

            if (Validator.IsValid(this.UserProfile))
            {
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

        //private void UserProfile_Error(object sender, ValidationErrorEventArgs e)
        //{
        //    Error.Content = e.Error.ErrorContent;
        //}
    }
}
