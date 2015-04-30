using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// Varients are available to modify products on the Dohsii app,
    /// Examples of varients may include;
    /// Steak cooking methods eg 'rare, medium, or well done',
    /// sauces eg 'Tomato, bbq, sour cream'
    /// or sides eg 'chips, veg, salad'
    /// each varient can have a price attahed to it.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Variants : JsonSerializationBase<Variants>
    {
        /// <summary>
        /// The name of the varient that will be displayed on the mobile app
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The price of the varient in cents
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "price")]
        public string Price { get; set; }

        /// <summary>
        /// The POS Id of the varient.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "pos_id")]
        public string PosId { get; set; }
    }
}
