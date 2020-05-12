using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Notification
    {
        public int id { get; }
        public int itemId { get; }
        public string itemName { get; }
        public DateTime timeStamp { get; }
        public byte textType { get; }
        public Notification(int _id, int _itemId, string _itemName, DateTime _timeStamp, byte _textType)
        {
            this.id = _id;
            this.itemId = _itemId;
            this.itemName = _itemName;
            this.timeStamp = _timeStamp;
            this.textType = _textType;
        }
    }
}
