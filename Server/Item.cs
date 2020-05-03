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
        //public List<string> images { get; }
        /*
         * this.images = _images;
         * 
         * List<string> _images
         */

        public string seller { get; }
        public string description { get; }
        public string date { get; }
        public string endDate { get; }
        public bool isItNew { get; }
        public bool buyWithoutBid { get; }
        public int bidStart { get; }

        public Item(int _id, string _name, string _category, int _price, int _current, string _image)
        {
            this.id = _id;
            this.name = _name;
            this.category = _category;
            this.price = _price;
            this.current = _current;
            this.image = _image;
        }

        public Item(int _id, string _name, string _category, int _price, int _current, string _image,
            string _seller, string _description, string _date, string _endDate, bool _isItNew, bool _buyWithoutBid, int _bidStart)
        {
            this.id = _id;
            this.name = _name;
            this.category = _category;
            this.price = _price;
            this.current = _current;
            this.image = _image;
            this.seller = _seller;
            this.description = _description;
            this.date = _date;
            this.endDate = _endDate;
            this.isItNew = _isItNew;
            this.buyWithoutBid = _buyWithoutBid;
            this.bidStart = _bidStart;
        }
    }
}
