using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Client.Modells
{
    public class DetailedItem : Item
    {
        
        public string Description { get; set; }
        public string  EndDate { get; set; }
        public string   Seller { get; set; }

        public int PriceBuy { get; set; }
        public bool New { get; set; }
        public bool QuickBuy { get; set; }
        public int MinBid { get; set; }

        public string SoldTo { get; set; }

        public DetailedItem(int id, string name, int price, int priceBuy, string category, string image, string description, string endDate, string seller, int minBid, bool quickBuy, bool New, string soldTo): base(id, name, priceBuy, price, category, image)
        {
            Id = id;
            Name = name;
            Price = price;
            Current = Current;
            Category = category;
            convertImage(image);
            Description = description;
            EndDate = endDate;
            Seller = seller;
            PriceBuy = priceBuy;
            this.New = New;
            QuickBuy = quickBuy;
            MinBid = minBid;
            SoldTo = soldTo;
        }


        private void convertImage(string b64)
        {
            try
            {
                byte[] binaryData = Convert.FromBase64String(b64);

                Image = new BitmapImage();
                Image.BeginInit();
                Image.StreamSource = new MemoryStream(binaryData);
                Image.EndInit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
