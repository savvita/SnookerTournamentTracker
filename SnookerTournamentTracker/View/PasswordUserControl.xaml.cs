using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace SnookerTournamentTracker.View
{
    /// <summary>
    /// Interaction logic for PasswordUserControl.xaml
    /// </summary>
    public partial class PasswordUserControl : UserControl
    {
        public SecureString Password
        {
            get =>  (SecureString)GetValue(PasswordProperty);

            set 
            { 
                SetValue(PasswordProperty, value); 
            }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(SecureString), typeof(PasswordUserControl),
                new PropertyMetadata(OnSourcePropertyChanged));


        public PasswordUserControl()
        {
            InitializeComponent();

            PasswordBox.PasswordChanged += (sender, args) => {
                Password = ((PasswordBox)sender).SecurePassword;
            };

        }

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                if(d is PasswordUserControl control)
                {
                    control.PasswordBox.Clear();
                }
            }
        }
    }
}
