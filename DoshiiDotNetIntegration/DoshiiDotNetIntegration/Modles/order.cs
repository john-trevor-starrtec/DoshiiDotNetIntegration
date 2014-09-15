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
        public string tip { get; set; }
        public string paySplits { get; set; }
        public string splitWays { get; set; }
        public string payTotal { get; set; }
        public string notPayingTotal { get; set; }
        public List<product> items { get; set; }
    }
}
