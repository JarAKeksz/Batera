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
        public int current { get; }
        public string image { get; }

        public Item(int _id, string _name, string _category, int _price, int _current, string _image)
        {
            this.id = _id;
            this.name = _name;
            this.category = _category;
            this.price = _price;
            this.current = _current;
            this.image = _image;
        }
    }
}
