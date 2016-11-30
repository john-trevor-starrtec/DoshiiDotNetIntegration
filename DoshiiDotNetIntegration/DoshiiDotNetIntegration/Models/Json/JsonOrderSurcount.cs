using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// Surcharges and Discounts that are applied at an order level.
    /// This model should not be used to record Product level discounts - discounts applied at a product level should be applied directly to the price attached to the product itself. 
    /// Surcharges should have a positive price.
    /// Discounts should have a negative price. 
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonOrderSurcount : JsonSerializationBase<JsonOrderSurcount>
    {
		/// <summary>
        /// The Name of the surcount
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The Amount / value of the surcount in cents. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// The Amount / value of the surcount in cents. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// The type of the surcount ('absolute' or 'percentage')
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// The posId for the product
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "posId")]
        public string Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "rewardId")]
        public string RewardId { get; set; }

        public bool ShouldSerializeId()
        {
            return (!string.IsNullOrEmpty(Id));
        }
    }
}
