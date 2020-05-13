using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Client.Modells
{
    public class Item
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Current { get; set; }
        public string Category { get; set; }
        public BitmapImage Image { get; set; }

        public Item(int id, string name, int price, int current, string category, string image)
        {
            Id = id;
            Name = name;
            Price = price;
            Current = current;
            Category = category;
            convertImage(image);
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
