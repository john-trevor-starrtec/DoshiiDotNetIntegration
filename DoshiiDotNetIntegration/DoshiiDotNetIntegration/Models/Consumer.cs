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
    public class Consumer : JsonSerializationBase<Consumer>
    {
        /// <summary>
        /// The Doshii id for the consumer.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        
        /// <summary>
        /// The meerkatConsumerId used to identify the consumer for all interactions with Doshii
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "meerkatConsumerId")]
        public string PaypalCustomerId { get; set; }
        
        /// <summary>
        /// The name of the consumer
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The url of the consumer photos
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "PhotoUrl")]
        public Uri PhotoUrl { get; set; }

        /// <summary>
        /// The CheckinId associated with the consumer. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "checkInId")]
        public string CheckInId { get; set; }

        public Consumer()
        {

        }


    }
}
