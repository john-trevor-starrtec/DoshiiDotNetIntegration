using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// this is data that is included in the doshii socket messages. all the fields are not used by all the messages 
    /// </summary>
    public class SocketMessageData : JsonSerializationBase<SocketMessageData>
    {
        /// <summary>
        /// the event name
        /// </summary>
        [JsonProperty(PropertyName = "event")]
        public string EventName { get; set; }
        
        /// <summary>
        /// the consumerId associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "consumerId")]
        public string ConsumerId { get; set; }

        /// <summary>
        /// the paypalCustomerId associated with the message. 
        /// </summary>
        [JsonProperty(PropertyName = "paypalCustomerId")]
        public string PaypalCustomerId { get; set; }

        /// <summary>
        /// the name associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// the id associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// the status associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// the orderId associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// the order associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "order")]
        public string Order { get; set; }

        /// <summary>
        /// the consumer associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "consumer")]
        public string Consumer { get; set; }

        /// <summary>
        /// the checkinId associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "checkinId")]
        public string CheckinId { get; set; }

        /// <summary>
        /// the url associated with the message. 
        /// </summary>
        [JsonProperty(PropertyName = "uri")]
        public Uri Uri { get; set; }
    }
}
