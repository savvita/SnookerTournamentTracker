using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
