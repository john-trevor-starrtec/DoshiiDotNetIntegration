using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    public class SocketMessageData : JsonSerializationBase<SocketMessageData>
    {
        [JsonProperty(PropertyName = "event")]
        public string EventName { get; set; }
        [JsonProperty(PropertyName = "consumerId")]
        public string ConsumerId { get; set; }
        [JsonProperty(PropertyName = "paypalCustomerId")]
        public string PaypalCustomerId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }
        [JsonProperty(PropertyName = "order")]
        public string Order { get; set; }
        [JsonProperty(PropertyName = "consumer")]
        public string Consumer { get; set; }
        [JsonProperty(PropertyName = "checkinId")]
        public string CheckinId { get; set; }
        [JsonProperty(PropertyName = "uri")]
        public Uri Uri { get; set; }
    }
}
