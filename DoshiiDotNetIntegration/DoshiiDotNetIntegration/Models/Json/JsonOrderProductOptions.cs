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
    internal class JsonOrderProductOptions : JsonSerializationBase<JsonOrderProductOptions>
    {
        /// <summary>
        /// The name of this product options / or list of variants
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The POS identifier for this set of variants. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "posId")]
        public string PosId { get; set; }

		private List<JsonOrderVariants> _Variants;

        /// <summary>
        /// A List of Variants available to be selected from this list. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "variants")]
		public List<JsonOrderVariants> Variants 
        {
            get
            {
                if (_Variants == null)
                {
					_Variants = new List<JsonOrderVariants>();
                }
                return _Variants;
            }
            set
            {
				_Variants = value;
            }
        }

        public bool ShouldSerializePosId()
        {
            return (!string.IsNullOrEmpty(PosId));
        }
    }
}
