using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Notification
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }

        public Notification(int id, string name, string date, string type)
        {
            Id = id;
            Name = name;
            Date = date;
            Type = type;
        }
    }
}
