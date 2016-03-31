using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// Variants are available to modify products on the Doshii app,
    /// Examples of variants may include;
    /// Steak cooking methods eg 'rare, medium, or well done',
    /// sauces eg 'Tomato, bbq, sour cream'
    /// or sides eg 'chips, veg, salad'
    /// each variant can have a price attached to it.
    /// </summary>
    [DataContract]
    [Serializable]
	internal class JsonOrderVariants : JsonSerializationBase<JsonOrderVariants>
    {
        /// <summary>
        /// The name of the variant that will be displayed on the mobile app
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The POS Id of the variant.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "posId")]
        public string PosId { get; set; }

		/// <summary>
		/// This field will be true if the variant has been selected.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "selectedOptionalVariant")]
		public bool SelectedOptionalVariant { get; set; }

        /// <summary>
        /// The price of the variant in cents
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "price")]
        public string Price { get; set; }
    }
}
