using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// these are options that are available with a product, they are not available for sale separatly but are available to be sold with products, productOptions can have a price but it is not necessary. 
    /// </summary>
    [DataContract]
    [Serializable]
    public class ProductOptions : JsonSerializationBase<ProductOptions>
    {
        /// <summary>
        /// the name of this product options - or list of vairents
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// the minimum amount of varients that must be chosen from this set of varients
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "min")]
        public int Min { get; set; }

        /// <summary>
        /// the maximum amoutn of varients that can be chosen form this set of varients. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "max")]
        public int Max { get; set; }

        /// <summary>
        /// the internal pos identifier for this product list. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "pos_id")]
        public string PosId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "variants")]
        public List<Variants> Variants = new List<Variants>();

        [DataMember]
        [JsonProperty(PropertyName = "selected")]
        public List<Variants> Selected = new List<Variants>();

        #region serialization methods 

        private bool SerializeSelected = false;

        public bool ShouldSerializeSelected()
        {
            if (SerializeSelected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ToJsonStringForOrder()
        {
            SerializeSelected = true;
            return base.ToJsonString();
        }

        public void SetSerializeForOrder(bool value)
        {
            SerializeSelected = value;
        }

        public string ToJsonStringForProductUpdate()
        {
            SerializeSelected = true;
            return base.ToJsonString();
        }

        #endregion
    }
}
