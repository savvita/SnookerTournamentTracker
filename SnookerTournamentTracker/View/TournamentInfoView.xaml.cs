﻿using SnookerTournamentTracker.ViewModel;
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
    /// Interaction logic for TournamentInfoView.xaml
    /// </summary>
    public partial class TournamentInfoView : Window
    {
        // TODO register/unregister - show only one of them
        // TODO tournament name in xaml - add wrapping text
        public TournamentInfoView()
        {
            InitializeComponent();
        }

        private TournamentViewViewModel? model;
        private PersonModel? user; 

        public TournamentInfoView(PersonModel user, TournamentModel tournament) : this()
        {
            this.user = user;
            model = new TournamentViewViewModel(user, tournament);
            this.Title = tournament.Name;
            model.RegisteringCompleted += (msg) => MessageBox.Show(msg, "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
            model.UnregisteringCompleted += (msg) => MessageBox.Show(msg, "Unregistration", MessageBoxButton.OK, MessageBoxImage.Information);
            model.RegistrationClosed += () =>
            {
                new MatchesView(user, tournament).ShowDialog();
            };

            this.DataContext = model;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ViewMatchesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (user != null && model != null) 
            {
                new MatchesView(user, model.Tournament).ShowDialog();
            }
        }
    }
}
