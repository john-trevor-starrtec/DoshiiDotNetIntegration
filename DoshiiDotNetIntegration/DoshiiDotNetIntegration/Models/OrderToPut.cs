using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// this item is specifically used when communicating with doshii for order updates.
    /// </summary>
    internal class OrderToPut : JsonSerializationBase<Checkin>
    {
        /// <summary>
        /// the order status
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        internal string Status { get; set; }
        
        /// <summary>
        /// the last time the order was updated. 
        /// </summary>
        [JsonProperty(PropertyName = "updatedAt")]
        internal DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// all the items included in the order. 
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        internal List<Product> Items { get; set; }

        public string ToJsonStringForOrder()
        {
            foreach (Product pro in Items)
            {
                pro.SetSerializeSettingsForOrder();
            }
            return base.ToJsonString();
        }
    }
}
