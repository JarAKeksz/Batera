﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.ProfilPageContent
{
    /// <summary>
    /// Interaction logic for ProfileSettingsPage.xaml
    /// </summary>
    public partial class ProfileSettingsPage : Page
    {
        public ProfileSettingsPage()
        {
            InitializeComponent();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            User.Instance.setToken(null);
            this.NavigationService.GoBack();
            this.NavigationService.GoBack();
            this.NavigationService.GoBack();
            this.NavigationService.GoBack();
            this.NavigationService.GoBack();
            this.NavigationService.GoBack();
            this.NavigationService.Navigate(new MainPage());
        }
    }
}
