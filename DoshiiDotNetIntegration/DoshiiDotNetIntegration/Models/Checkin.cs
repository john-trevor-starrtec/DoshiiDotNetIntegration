using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    public class Checkin : JsonSerializationBase<Checkin>
    {
        [JsonProperty (PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "paypalTabId")]
        public string PaypalTabId { get; set; }
        [JsonProperty(PropertyName = "consumerId")]
        public string ConsumerId { get; set; }
        [JsonProperty(PropertyName = "locationId")]
        public string LocationId { get; set; }
        
        /// <summary>
        /// this is really only here because it is returned as part of the received data, it can be ignored by the pos and these statuses are handled by doshii.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "ExpirationDate")]
        public DateTime ExpirationDate {get;set;}
        [JsonProperty(PropertyName = "Gratuity")]
        public string Gratuity {get;set;}
        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime UpdatedAt {get;set;}
        [JsonProperty(PropertyName = "paypalCustomerId")]
        public string PaypalCustomerId {get;set;}

        public Checkin()
        {
        }
    }
}
