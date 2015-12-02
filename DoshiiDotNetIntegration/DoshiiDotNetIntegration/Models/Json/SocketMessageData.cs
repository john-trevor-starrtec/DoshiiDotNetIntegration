using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
    /// This is data that is included in the doshii socket messages. all the fields are not used by all the messages 
    /// </summary>
    internal class SocketMessageData : JsonSerializationBase<SocketMessageData>
    {
        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The event name
        /// </summary>
        [JsonProperty(PropertyName = "event")]
        public string EventName { get; set; }
        
        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The ConsumerId associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "consumerId")]
        public string ConsumerId { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The MeerkatCustomerId associated with the message. 
        /// </summary>
        [JsonProperty(PropertyName = "meerkatConsumerId")]
        public string meerkatConsumerId { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The Name associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The Id associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The Status associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The OrderId associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The Order associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "order")]
        public string Order { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The Consumer associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "consumer")]
        public string Consumer { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The CheckInId associated with the message
        /// </summary>
        [JsonProperty(PropertyName = "checkinId")]
        public string CheckinId { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// The URL associated with the message. 
        /// </summary>
        [JsonProperty(PropertyName = "uri")]
        public Uri Uri { get; set; }

        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }


    }
}
