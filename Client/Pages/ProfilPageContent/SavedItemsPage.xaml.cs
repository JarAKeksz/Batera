using Client.Modells;
using Client.Pages;
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

namespace Client.ProfilPageContent
{
    /// <summary>
    /// Interaction logic for SavedItemsPage.xaml
    /// </summary>
    public partial class SavedItemsPage : Page
    {
        BateraCliensClass helper = new BateraCliensClass();

        public SavedItemsPage()
        {
            InitializeComponent();

            favoriteItemsList.ItemsSource = helper.GetFavoriteItem(User.Instance.getToken());
        }
        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (favoriteItemsList.SelectedItem != null)
            {
                Item i = favoriteItemsList.SelectedItem as Item;
                Console.WriteLine(i.Id);

                if (helper.GetDetailedItem(i.Id) != null)
                {
                    this.NavigationService.Navigate(new DetailedItemPage(helper.GetDetailedItem(i.Id)));
                }
            }
        }
    }
}
