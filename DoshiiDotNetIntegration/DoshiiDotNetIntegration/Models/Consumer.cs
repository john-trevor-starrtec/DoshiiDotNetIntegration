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
        [JsonProperty(PropertyName = "paypalCustomerId")]
        public string PaypalCustomerId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "PhotoUrl")]
        public Uri PhotoUrl { get; set; }
        [JsonProperty(PropertyName = "checkInId")]
        public string CheckInId { get; set; }


    }
}
