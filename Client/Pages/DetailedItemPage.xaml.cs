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

            Console.WriteLine("*******" + item.SoldTo + "******");

            if (item.SoldTo == "")
            {
                setItemData();
            }
            else
            {
                itemSold.Text = "SOLD";
                setItemData();
                disableItemActions();

            }



        }

        private void setItemData()
        {
            itemTitle.Text = itemx.Name + "(" + itemx.Category + ")";
            itemImage.Source = itemx.Image;
            itemDescription.Text = "Description: \n" + itemx.Description;
            itemSeller.Text = "Seller: " + itemx.Seller;
            itemEndDate.Text = "End date: " + itemx.EndDate;
            minBidTextBlock.Text = "Minimum bid:" + itemx.MinBid;

            if (itemx.New == true)
            {
                itemCondition.Text = "condition: New";
            }
            else
            {
                itemCondition.Text = "condition: Used";
            }


            if (itemx.QuickBuy == true)
            {
                itemPricebuy.Text = "Price (for buy now): " + itemx.PriceBuy.ToString();
                buyNowButton.IsEnabled = true;
            }
            else
            {
                buyNowButton.IsEnabled = false;
            }

            int e = -1;
            if (itemx.Price == e)
            {
                itemPrice.Text = "Price: " + (itemx.Price + 1).ToString();
            }
        }
        private void disableItemActions()
        {
            addFavorite.Visibility = Visibility.Hidden;
            placeBid.Visibility = Visibility.Hidden;
            minBidTextBlock.Visibility = Visibility.Hidden;
            placeBidButton.Visibility = Visibility.Hidden;
            autobid.Visibility = Visibility.Hidden;
            itemAutoBid.Visibility = Visibility.Hidden;
            placeAutoBidButton.Visibility = Visibility.Hidden;
            removeAutoBidButton.Visibility = Visibility.Hidden;
            buyNowButton.Visibility = Visibility.Hidden;
            itemBid.Visibility = Visibility.Hidden;
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

        private void placeBidButton_Click(object sender, RoutedEventArgs e)
        {
            int bid = int.Parse(itemBid.Text);
            bool bidOk = Int32.TryParse(itemBid.Text, out bid);

            if (User.Instance.getToken() != null && itemBid.Text != "" && bidOk)
            {
                if (itemx.Price < bid)
                {
                    helper.MakeBid(User.Instance.getToken(), itemx.Id, bid);
                    itemPrice.Text = "Price: " + itemx.Price.ToString();
                }
                else
                {
                    Console.WriteLine("Bid more!");
                }

            }
            else
            {
                if (User.Instance.getToken() == null)
                {
                    Console.WriteLine(" need to log in");
                    this.NavigationService.Navigate(new LoginPage());
                }
                if (itemBid.Text == "" && !bidOk)
                {
                    Console.WriteLine("make a valid bid");
                }
            }
        }

        private void placeAutoBidButton_Click(object sender, RoutedEventArgs e)
        {
            int bid = int.Parse(itemAutoBid.Text);
            bool bidOk = Int32.TryParse(itemAutoBid.Text, out bid);
            if (User.Instance.getToken() != null && itemAutoBid.Text != "" && bidOk)
            {
                if (itemx.Price < bid)
                {
                    helper.MakeAutoBid(User.Instance.getToken(), itemx.Id, bid, true);
                }
                else
                {
                    Console.WriteLine("Bid more!");
                }

            }
            else
            {
                if (User.Instance.getToken() == null)
                {
                    Console.WriteLine(" need to log in");
                    this.NavigationService.Navigate(new LoginPage());
                }
                if (itemAutoBid.Text == "" && !bidOk)
                {
                    Console.WriteLine("make a valid auto bid");
                }
            }
        }

        private void removeAutoBidButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.Instance.getToken() != null)
            {
                Console.WriteLine("autobid removed");
            }
            else
            {
                Console.WriteLine(" need to log in");
                this.NavigationService.Navigate(new LoginPage());

            }
        }

        private void buyNowButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.Instance.getToken() != null)
            {
                helper.MakeBuy(User.Instance.getToken(), itemx.Id);

            }
            else
            {
                Console.WriteLine(" need to log in");
                this.NavigationService.Navigate(new LoginPage());
            }
        }
    }
}
