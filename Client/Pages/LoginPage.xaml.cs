using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            String email = emailTextBox.Text;
            String password = passwordPasswordBox.Password;

            if(email != null && password != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8000");
                    
                    HttpContent content;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        JsonWriterOptions JW_OPTS = new JsonWriterOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                        using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                        {
                            writer.WriteStartObject();
                            writer.WriteString("email", email);
                            writer.WriteString("password", password);
                            writer.WriteEndObject();
                        }

                        content = new StringContent(Encoding.UTF8.GetString(stream.ToArray()), Encoding.UTF8, "application/json");
                    }
                    var result = await client.PostAsync("/login", content);
                    string resultContent = await result.Content.ReadAsStringAsync();

                    using (JsonDocument document = JsonDocument.Parse(resultContent))
                    {
                        bool success = document.RootElement.GetProperty("success").GetBoolean();
                        if (success)
                        {
                            //LOGIN SUCCESS
                            string token = document.RootElement.GetProperty("token").GetString();
                            string name = document.RootElement.GetProperty("username").GetString();
                            User.Instance.setToken(token);
                            User.Instance.setName(name);
                            this.NavigationService.Navigate(new MainPage());
                        }
                        else
                        {
                            //LOGIN FAIL
                            Console.WriteLine("Login failed.");
                            MessageBox.Show("Login failed.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("email and password is mandatory");
                
                MessageBox.Show("email and password is mandatory");
                
            }
        }

        private void signupButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SignUpPage());
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
            this.NavigationService.Navigate(new MainPage());
        }

        
    }
}
