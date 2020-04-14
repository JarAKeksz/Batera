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
        public string image { get; }
        public Item(int _id, string _name, string _image)
        {
            id = _id;
            name = _name;
            image = _image;
        }
    }
}
