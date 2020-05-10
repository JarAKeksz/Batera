using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Bid
    {
        public int userId { get; }
        public string userName { get; }
        public int value { get; }
        public Bid(int _userId, string _userName, int _value)
        {
            this.userId = _userId;
            this.userName = _userName;
            this.value = _value;
        }
    }
}
