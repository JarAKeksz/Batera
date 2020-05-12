using Client.Modells;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        BateraCliensClass helper = new BateraCliensClass();

        public ProfilePage()
        {
            InitializeComponent();
            ProfilPageContent.NavigationService.Navigate(new ProfilPageContent.SavedItemsPage());

            
        }

        private void logoTextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new MainPage());
        }

        private void savedItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ProfilPageContent.NavigationService.Navigate(new ProfilPageContent.SavedItemsPage());
        }

        private void boughtItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ProfilPageContent.NavigationService.Navigate(new ProfilPageContent.BoughtItemsPage());
        }

        private void ProfileSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ProfilPageContent.NavigationService.Navigate(new ProfilPageContent.ProfileSettingsPage());
        }

        private void profileButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProfilePage());
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Pages.ProfilPageContent.AddItemPage());
        }
    }
}
