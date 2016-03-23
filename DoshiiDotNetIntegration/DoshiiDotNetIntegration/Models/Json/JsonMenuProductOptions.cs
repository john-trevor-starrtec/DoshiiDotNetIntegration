using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// Product options are lists of product variants that can be selected from to modify products.
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonMenuProductOptions : JsonSerializationBase<JsonMenuProductOptions>
    {
        /// <summary>
        /// The name of this product options / or list of variants
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The minimum amount of variants that must be chosen from this set of variants
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "min")]
        public int Min { get; set; }

        /// <summary>
        /// The maximum amount of variants that can be chosen form this set of variants. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "max")]
        public int Max { get; set; }

        /// <summary>
        /// The POS identifier for this set of variants. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "pos_id")]
        public string PosId { get; set; }

		private List<JsonVariants> _Variants;

        /// <summary>
        /// A List of Variants available to be selected from this list. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "variants")]
		public List<JsonVariants> Variants 
        {
            get
            {
                if (_Variants == null)
                {
					_Variants = new List<JsonVariants>();
                }
                return _Variants;
            }
            set
            {
				_Variants = value;
            }
        } 
    }
}
