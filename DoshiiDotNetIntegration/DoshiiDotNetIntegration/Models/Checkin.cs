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
        /// the checkIn Id
        /// </summary>
        [DataMember]
        [JsonProperty (PropertyName = "id")]
        public string Id { get; set; }
        
        /// <summary>
        /// the paypalTabId related to the checkin
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "paypalTabId")]
        public string PaypalTabId { get; set; }
        
        /// <summary>
        /// The doshii customerId
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "consumerId")]
        public string ConsumerId { get; set; }
        
        /// <summary>
        /// the venue specific id supplied by doshii
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "locationId")]
        public string LocationId { get; set; }
        
        /// <summary>
        /// this is really only here because it is returned as part of the received data, it can be ignored by the pos and these statuses are handled by doshii.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        /// <summary>
        /// the expirationDate of the checkin
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "ExpirationDate")]
        public DateTime ExpirationDate {get;set;}
        
        /// <summary>
        /// the Gratuity for the checkin
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "Gratuity")]
        public string Gratuity {get;set;}
        
        /// <summary>
        /// the last time the checkin was updated
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime UpdatedAt {get;set;}
        
        /// <summary>
        /// the paypalConsumerId used to identify the consumer for all interactions with doshii
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "paypalCustomerId")]
        public string PaypalCustomerId {get;set;}

        public Checkin()
        {
        }
    }
}
