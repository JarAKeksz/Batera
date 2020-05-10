using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class DetailedItem : Item
    {
        public string seller { get; }
        public string description { get; }
        public string date { get; }
        public string endDate { get; }
        public bool isItNew { get; }
        public bool buyWithoutBid { get; }
        public int bidStart { get; }
        public int bidIncrement { get; }
        public List<Bid> bidList { get; }
        //public List<string> images { get; }
        /*
         * this.images = _images;
         * 
         * List<string> _images
         */

        public DetailedItem(int _id, string _name, string _category, int _price, int _current, string _image,
            string _seller, string _description, string _date, string _endDate, bool _isItNew, bool _buyWithoutBid, int _bidStart, int _bidIncrement, List<Bid> _bidList)
            : base(_id, _name, _category, _price, _current, _image)
        {
            this.seller = _seller;
            this.description = _description;
            this.date = _date;
            this.endDate = _endDate;
            this.isItNew = _isItNew;
            this.buyWithoutBid = _buyWithoutBid;
            this.bidStart = _bidStart;
            this.bidIncrement = _bidIncrement;
            this.bidList = _bidList;
        }
    }
}
