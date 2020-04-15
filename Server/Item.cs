using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Item
    {
        public int id { get; }
        public string name { get; }
        public string category { get; }
        public int price { get; }
        public string image { get; }

        public Item(int id, string name, string category, int price, string image)
        {
            this.id = id;
            this.name = name;
            this.category = category;
            this.price = price;
            this.image = image;
        }
    }
}
