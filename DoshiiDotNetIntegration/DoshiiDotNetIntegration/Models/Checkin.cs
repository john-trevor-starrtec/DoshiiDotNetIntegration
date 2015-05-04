using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    [DataContract]
    [Serializable]
    public class Checkin : JsonSerializationBase<Checkin>
    {
        /// <summary>
        /// The CheckIn Id
        /// </summary>
        [DataMember]
        [JsonProperty (PropertyName = "id")]
        public string Id { get; set; }
        
        /// <summary>
        /// The Doshii customerId
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "consumerId")]
        public string ConsumerId { get; set; }
        
        /// <summary>
        /// The venue specific Id supplied by Doshii
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "locationId")]
        public string LocationId { get; set; }
        
        /// <summary>
        /// This value is controlled by Doshii,
        /// The value can be ignored by the Pos
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        /// <summary>
        /// The ExpirationDate of the CheckIn
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "ExpirationDate")]
        public DateTime ExpirationDate {get;set;}
        
        /// <summary>
        /// The Gratuity for the CheckIn
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "Gratuity")]
        public string Gratuity {get;set;}
        
        /// <summary>
        /// The last time the CheckIn was updated
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime UpdatedAt {get;set;}
        
        /// <summary>
        /// The PaypalConsumerId used to identify the consumer for all interactions with doshii
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "meerkatConsumerId")]
        public string MeerkatConsumerId { get; set; }

        public Checkin()
        {
        }
    }
}
