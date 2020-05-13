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

namespace Client.Pages
{
    /// <summary>
    /// Interaction logic for DetailedItemPage.xaml
    /// </summary>
    public partial class DetailedItemPage : Page
    {
        BateraCliensClass helper = new BateraCliensClass();
        DetailedItem itemx;
        public DetailedItemPage(DetailedItem item)
        {
            itemx = item;
            InitializeComponent();

            Console.WriteLine("megkaptam" + item.Seller);

            itemTitle.Text = item.Name + "("+item.Category+")";
            itemImage.Source = item.Image;
            itemPrice.Text = "Price: " + item.Price.ToString();
            itemDescription.Text = "Description: \n" + item.Description;
            itemSeller.Text = "Seller: " + item.Seller;
            itemEndDate.Text = "End date: " + item.EndDate;

        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void addFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (User.Instance.getToken() != null)
            {
                helper.AddFavoriteItem(User.Instance.getToken(), itemx.Id);
            }
            else
            {
                Console.WriteLine("Need to sign in");
                this.NavigationService.Navigate(new LoginPage());
            }
        }
    }
}
