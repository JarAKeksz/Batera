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
    /// Interaction logic for BoughtItemsPage.xaml
    /// </summary>
    public partial class BoughtItemsPage : Page
    {

        BateraCliensClass helper = new BateraCliensClass();
        public BoughtItemsPage()
        {
            InitializeComponent();
            var notificationList = helper.GetNotafication(User.Instance.getToken());
            notificationList.Add(new Notification(1, "valami", "2020/05/13", "tipus"));
            NotificationList.ItemsSource = notificationList;

        }


    }
}
