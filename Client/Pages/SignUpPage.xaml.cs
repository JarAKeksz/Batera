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
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new LoginPage());
        }

        private async void signupButton_Click(object sender, RoutedEventArgs e)
        {
            String email = emailTextBox.Text.Trim(' ');
            String password = passwordPasswordBox.Password.Trim(' ');
            String passwordAgain = passwordAgainPasswordBox.Password;
            String name = nameTextBox.Text;

            if(password == passwordAgain)
            {
                if (email != null && password != null && name != null)
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
                                writer.WriteString("name", name);
                                writer.WriteString("username", name);
                                writer.WriteEndObject();
                            }

                            content = new StringContent(Encoding.UTF8.GetString(stream.ToArray()), Encoding.UTF8, "application/json");
                        }
                        var result = await client.PostAsync("/sign_up", content);
                        string resultContent = await result.Content.ReadAsStringAsync();

                        using (JsonDocument document = JsonDocument.Parse(resultContent))
                        {
                            string success = document.RootElement.GetProperty("token").GetString();
                            if (success != null)
                            {
                                //LOGIN SUCCESS
                                string token = document.RootElement.GetProperty("token").GetString();
                                User.Instance.setToken(token);
                                this.NavigationService.Navigate(new MainPage());
                            }
                            else
                            {
                                //LOGIN FAIL
                                Console.WriteLine("Sign up failed.");
                            }
                        }
                    }
                }
            }

            this.NavigationService.Navigate(new SignUpPage());
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainPage());
        }
    }
}
