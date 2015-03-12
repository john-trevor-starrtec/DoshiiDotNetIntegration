using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// surcharges and discounts that are applied at an order level.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Surcount
    {
        /// <summary>
        /// the name of the surcount
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }


        /// <summary>
        /// the price of the surcount in cents. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "price")]
        public string Price { get; set; }
    }
}
