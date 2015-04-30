using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// DO NOT USE FROM POS
    /// This model is used when either completing a PUT or a POST to update an order
    /// This model should not instanciated by the Pos and should only be used internaly. 
    /// </summary>
    [DataContract]
    [Serializable]
    public  class OrderToPut : JsonSerializationBase<Checkin>
    {
        /// <summary>
        /// The order status
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public  string Status { get; set; }
        
        /// <summary>
        /// The last time the order was updated. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public  string UpdatedAt { get; set; }
        
        /// <summary>
        /// All the items included in the order. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "items")]
        public  List<Product> Items { get; set; }

        /// <summary>
        /// A list of all surcounts applied at and order level
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "surcounts")]
        public  List<Surcount> Surcounts { get; set; }

        /// <summary>
        /// A list of all payments applied at and order level
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "payments")]
        public  List<Payment> Payments { get; set; }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
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
