using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Category
    {
        public int id { get; }
        public string name { get; }
        public string description { get; }

        public Category(int _id, string _name, string _description)
        {
            id = _id;
            name = _name;
            description = _description;
        }
    }
}
