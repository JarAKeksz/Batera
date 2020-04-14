using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{

    public sealed class User
    {
        private static User instance = null;
        private string id;
        private string name;
        private string token;

        private User()
        {
        }

        public static User Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new User();
                }
                return instance;
            }
        }
        public string getToken()
        {
            return token;
        }
        public void setToken(string token)
        {
            this.token = token;
        }

        public string getName()
        {
            return name;
        }
        public void setName(String name)
        {
            this.name = name;
        }

        public string getId()
        {
            return id;
        }
        public void setId(string id)
        {
            this.id = id;
        }
    }
}
