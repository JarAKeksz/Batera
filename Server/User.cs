using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class User
    {
        public int id { get; }
        public string userName { get; }

        public User(int _id, string _userName)
        {
            id = _id;
            userName = _userName;
        }
    }
}
