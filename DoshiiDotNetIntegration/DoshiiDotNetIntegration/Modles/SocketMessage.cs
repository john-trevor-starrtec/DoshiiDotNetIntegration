using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Modles
{
    internal class SocketMessage : JsonSerializationBase<SocketMessage>
    {
        [JsonProperty(PropertyName = "event")]
        internal string EventName { get; set; }

        internal string consumerId { get; set; }

        internal string paypalCustomerId { get; set; }

        internal string name { get; set; }

        internal string id { get; set; }

        internal string status { get; set; }

        internal int orderId { get; set; }

        internal string order { get; set; }

        internal string consumer { get; set; }

        internal string checkin { get; set; }

        internal Uri uri { get; set; }
    }
}
