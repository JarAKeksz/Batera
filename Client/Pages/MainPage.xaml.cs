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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        BateraCliensClass helper = new BateraCliensClass();
        public MainPage()
        {
            InitializeComponent();
            setDefault();


            

        }

        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemsList.SelectedItem != null)
            {
                Item i = itemsList.SelectedItem as Item;
                Console.WriteLine(i.Id);

                if (helper.GetDetailedItem(i.Id) != null)
                {
                    this.NavigationService.Navigate(new DetailedItemPage(helper.GetDetailedItem(i.Id)));
                    itemsList.SelectedIndex = -1;
                }
            }
        }


        private void profileButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.Instance.getToken() != null)
            {
                this.NavigationService.Navigate(new ProfilePage());
            }
            else
            {
                this.NavigationService.Navigate(new LoginPage());
            }

        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {

            List<Item> list = new List<Item>();
            int selectedCategory = -1;
            if (categoriesComboBox.SelectedItem != null)
            {
                selectedCategory = categoriesComboBox.SelectedIndex;
                Console.WriteLine(selectedCategory.ToString() + "  category selected");
            }
            
            string selectedCondition = GetCondition().ToString();
            string selectedBuyingFormat = SelectedBuyingFormat().ToString();


            int minPrice = priceFromTextBox.Text.ToString() != "" ? int.Parse(priceFromTextBox.Text) : -1;
            int maxPrice = priceToTextBox.Text.ToString() != "" ? int.Parse(priceToTextBox.Text) : -1;

            Console.WriteLine(minPrice.ToString()+ " " + maxPrice.ToString());

            list = helper.SearchedItem(searchBarTextBox.Text, selectedCategory, selectedBuyingFormat, selectedCondition, minPrice, maxPrice, selectedCategory);


            itemsList.ItemsSource = list;

        }

        private enum ConditionValue
        {
            All,
            Used,
            New
        }
        private ConditionValue GetCondition()
        {
            ConditionValue selectedCondition = ConditionValue.All;
            if(newConditionCheckBox.IsChecked.Value && !usedConditionCheckBox.IsChecked.Value) { selectedCondition = ConditionValue.New; }
            else if(usedConditionCheckBox.IsChecked.Value && !newConditionCheckBox.IsChecked.Value) { selectedCondition = ConditionValue.Used; }
            return selectedCondition;
        }

        private enum BuyingFormat
        {
            All,
            Bid,
            Buy
        }
        private BuyingFormat SelectedBuyingFormat()
        {
            BuyingFormat format = BuyingFormat.All;
            if(bidBuyingFormatCheckBox.IsChecked.Value && !buyBuyingFormatCheckBox.IsChecked.Value) { format = BuyingFormat.Bid; }
            else if (!bidBuyingFormatCheckBox.IsChecked.Value && buyBuyingFormatCheckBox.IsChecked.Value) { format = BuyingFormat.Buy; }
            return format;
        }

        private void logoTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setDefault();
        }

        private void setDefault()
        {
            List<Item> list = new List<Item>();
            list = helper.AllItem();
            itemsList.ItemsSource = list;

            searchBarTextBox.Text = "";
            List<string> categories = helper.GetCategories();
            categoriesComboBox.ItemsSource = categories;
            categoriesComboBox.SelectedItem = null;
            categoriesComboBox.Text = "--select--";
        }
    }
}
