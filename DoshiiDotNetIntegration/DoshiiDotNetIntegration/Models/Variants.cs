using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

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
    public class Variants : JsonSerializationBase<Variants>
    {
        /// <summary>
        /// The name of the varient that will be displayed on the mobile app
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The price of the varient in cents
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public string Price { get; set; }

        /// <summary>
        /// the internal Id of the product.
        /// </summary>
        [JsonProperty(PropertyName = "pos_id")]
        public string PosId { get; set; }
    }
}
