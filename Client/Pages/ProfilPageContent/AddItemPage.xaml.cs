using Microsoft.Win32;
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

namespace Client.Pages.ProfilPageContent
{
    /// <summary>
    /// Interaction logic for AddItemPage.xaml
    /// </summary>
    public partial class AddItemPage : Page
    {
        BateraCliensClass helper = new BateraCliensClass();
        public AddItemPage()
        {
            InitializeComponent();

            List<string> categories = helper.GetCategories();
            categoriesComboBox.ItemsSource = categories;
            categoriesComboBox.SelectedItem = null;
            categoriesComboBox.Text = "--select--";
            startingPirceTextbox.Text = "-1";
            startingPirceTextbox.Text = "-1";
            categoriesComboBox.SelectedIndex = 0;
        }

        private void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                itemImage.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void addItemButton_Click(object sender, RoutedEventArgs e)
        {
            if(titleTextbox.Text != "" && descriptionTextbox.Text != "" && itemImage.Source != null)
            {
                string title = titleTextbox.Text;
                string description = descriptionTextbox.Text;
                if(priceTextbox.Text == "-1" && startingPirceTextbox.Text != "-1")
                {
                    //bid only
                    int price = int.Parse(priceTextbox.Text);
                    int startingPirce = int.Parse(startingPirceTextbox.Text);
                }
                else if (priceTextbox.Text != "-1" && startingPirceTextbox.Text != "-1")
                {
                    //bid and buy
                    int price = int.Parse(priceTextbox.Text);
                    int startingPirce = int.Parse(startingPirceTextbox.Text);
                }
                else if (priceTextbox.Text != "-1" && startingPirceTextbox.Text == "-1")
                {
                    //buy only
                    int price = int.Parse(priceTextbox.Text);
                    int startingPirce = int.Parse(startingPirceTextbox.Text);
                }

                int categories = categoriesComboBox.SelectedIndex;
            }

        }



    }
}
