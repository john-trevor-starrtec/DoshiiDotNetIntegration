using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class SocketMessageData : JsonSerializationBase<SocketMessageData>
    {
        public string EventName { get; set; }
        public string consumerId { get; set; }
        public string paypalCustomerId { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public string orderId { get; set; }
        public string order { get; set; }
        public string consumer { get; set; }
        public string checkinId { get; set; }
        public Uri uri { get; set; }
    }
}
