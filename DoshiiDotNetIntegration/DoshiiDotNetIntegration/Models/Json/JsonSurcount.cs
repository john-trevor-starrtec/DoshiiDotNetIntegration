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
    internal class JsonSurcount : JsonSerializationBase<JsonSurcount>
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public JsonSurcount()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Name = String.Empty;
			Price = String.Empty;
		}

        /// <summary>
        /// The Name of the surcount
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The Price / value of the surcount in cents. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "price")]
        public string Price { get; set; }
    }
}
