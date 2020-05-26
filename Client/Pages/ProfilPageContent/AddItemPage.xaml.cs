using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Drawing;
using System.Net.Http;
using System.Text.Json;

namespace Client.Pages.ProfilPageContent
{
    /// <summary>
    /// Interaction logic for AddItemPage.xaml
    /// </summary>
    public partial class AddItemPage : Page
    {
        BateraCliensClass helper = new BateraCliensClass();
        string base64String;
        public AddItemPage()
        {
            InitializeComponent();

            List<string> categories = helper.GetCategories();
            categoriesComboBox.ItemsSource = categories;
            categoriesComboBox.SelectedItem = null;
            categoriesComboBox.Text = "--select--";
            startingPirceTextbox.Text = "-1";
            priceTextbox.Text = "-1";
            categoriesComboBox.SelectedIndex = 0;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
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


                using (System.Drawing.Image image = System.Drawing.Image.FromFile(op.FileName))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                }
            }

        }

        private void addItemButton_Click(object sender, RoutedEventArgs e)
        {
            int price = -1;
            int startingPirce = -1;
            bool startingPirceOk = Int32.TryParse(startingPirceTextbox.Text, out startingPirce);
            bool pirceOk = Int32.TryParse(startingPirceTextbox.Text, out price);

            if (titleTextbox.Text != "" && descriptionTextbox.Text != "" && itemImage.Source != null && startingPirceOk && pirceOk)
            {
                string title = titleTextbox.Text;
                string description = descriptionTextbox.Text;
                if(priceTextbox.Text == "-1" && startingPirceTextbox.Text != "-1")
                {
                    //bid only
                     price = int.Parse(priceTextbox.Text);
                    startingPirce = int.Parse(startingPirceTextbox.Text);
                }
                else if (priceTextbox.Text != "-1" && startingPirceTextbox.Text != "-1")
                {
                    //bid and buy
                    price = int.Parse(priceTextbox.Text);
                    startingPirce = int.Parse(startingPirceTextbox.Text);
                }
                else if (priceTextbox.Text != "-1" && startingPirceTextbox.Text == "-1")
                {
                    //buy only
                    price = int.Parse(priceTextbox.Text);
                    startingPirce = int.Parse(startingPirceTextbox.Text);
                }

                int categorie = categoriesComboBox.SelectedIndex;

                helper.AddItem(User.Instance.getToken(), title, description, base64String,categorie, startingPirce, price);
                this.NavigationService.GoBack();

            }
        }

       


    }
}
