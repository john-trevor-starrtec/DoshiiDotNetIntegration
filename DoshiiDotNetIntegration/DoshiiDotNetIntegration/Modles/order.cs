using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class order : JsonSerializationBase<order>
    {
        public int id { get; set; } 
        public Enums.OrderStates status { get; set; }
        public string invoiceId{ get; set; }
        public string transactionId { get; set; } 
        public string checkinId { get; set; }
        public int tip { get; set; }
        public int paySplits { get; set; }
        public int splitWays { get; set; }
        public int payTotal { get; set; }
        public int notPayingTotal { get; set; }
        public List<product> items { get; set; }
    }
}
