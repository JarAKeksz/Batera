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

        public Item(int _id, string _name)
        {
            id = _id;
            name = _name;
        }
    }
}
