using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    
    public class Consumer : JsonSerializationBase<Consumer>
    {
        /// <summary>
        /// the doshii id for the cusomter
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        
        /// <summary>
        /// the paypalConsumerId used to identify the consumer for all interactions with doshii
        /// </summary>
        [JsonProperty(PropertyName = "paypalCustomerId")]
        public string PaypalCustomerId { get; set; }
        
        /// <summary>
        /// the name of the customer
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        /// <summary>
        /// the url of the customers photos
        /// </summary>
        [JsonProperty(PropertyName = "PhotoUrl")]
        public Uri PhotoUrl { get; set; }

        /// <summary>
        /// the checkinId associated with the customer. 
        /// </summary>
        [JsonProperty(PropertyName = "checkInId")]
        public string CheckInId { get; set; }


    }
}
