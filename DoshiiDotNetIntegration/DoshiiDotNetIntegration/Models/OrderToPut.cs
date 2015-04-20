using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// this item is specifically used when communicating with doshii for order updates.
    /// </summary>
    [DataContract]
    [Serializable]
    public  class OrderToPut : JsonSerializationBase<Checkin>
    {
        /// <summary>
        /// the order status
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public  string Status { get; set; }
        
        /// <summary>
        /// the last time the order was updated. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public  string UpdatedAt { get; set; }
        
        /// <summary>
        /// all the items included in the order. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "items")]
        public  List<Product> Items { get; set; }

        /// <summary>
        /// a list of all surcounts applied at and order level
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "surcounts")]
        public  List<Surcount> Surcounts { get; set; }

        /// <summary>
        /// a list of all payments applied at and order level
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "payments")]
        public  List<Payment> Payments { get; set; }

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
