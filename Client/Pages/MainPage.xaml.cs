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
            List<Item> list = new List<Item>();
            list = helper.AllItem();

            itemsList.ItemsSource = list;

            DataContext = new ComboBoxViewModel();

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

            string selectedcategory = categoriesComboBox.SelectedItem.ToString();
            ConditionValue selectedCondition = GetCondition();
            BuyingFormat selectedBuyingFormat = SelectedBuyingFormat();
            int minPrice = Int32.Parse(priceFromTextBox.Text);
            int maxPrice = Int32.Parse(priceToTextBox.Text);

            list = helper.SearchedItem(searchBarTextBox.Text, selectedcategory);



            itemsList.ItemsSource = list;
        }

        private class ComboBoxViewModel{
            public List<String> categoriesCollection { get; set; }

            public ComboBoxViewModel()
            {
                categoriesCollection = new List<string>()
                {
                    "game",
                    "nature",
                    "anime"
                };
            }
            
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


    }
}
