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
        public string userName { get; set; }
        public string logInToken { get; set; }
        public string name { get; set; }
        public string birthDate { get; set; }
        public string email { get; set; }
        public string phone { get; set; }

        public User(int _id, string _userName)
        {
            id = _id;
            userName = _userName;
        }

        public User(int _id, string _userName, string _logInToken, string _name, string _birthDate, string _email)
        {
            id = _id;
            userName = _userName;
            logInToken = _logInToken;
            name = _name;
            birthDate = _birthDate;
            email = _email;
        }
    }
}
