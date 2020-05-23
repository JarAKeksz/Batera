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
        public DateTime endDate { get; }
        public bool isItNew { get; }
        public bool buyWithoutBid { get; }
        public string soldTo { get; }
        public int bidStart { get; }
        public int bidIncrement { get; }
        public List<Bid> bidList { get; }
        public DetailedItem(int _id, string _name, string _category, int _price, int _current, string _image,
            string _seller, string _description, DateTime _endDate, bool _isItNew, bool _buyWithoutBid, string _soldTo, int _bidStart, int _bidIncrement, List<Bid> _bidList)
            : base(_id, _name, _category, _price, _current, _image)
        {
            this.seller = _seller;
            this.description = _description;
            this.endDate = _endDate;
            this.isItNew = _isItNew;
            this.buyWithoutBid = _buyWithoutBid;
            this.soldTo = _soldTo;
            this.bidStart = _bidStart;
            this.bidIncrement = _bidIncrement;
            this.bidList = _bidList;
        }
    }
}
