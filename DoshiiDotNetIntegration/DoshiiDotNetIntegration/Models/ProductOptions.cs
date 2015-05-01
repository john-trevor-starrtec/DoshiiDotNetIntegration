using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// Product options are lists of product variants that can be selected from to modify products.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ProductOptions : JsonSerializationBase<ProductOptions>
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

        private List<Variants> _Variants;

        /// <summary>
        /// A List of Variants available to be selected from this list. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "variants")]
        public List<Variants> Variants 
        {
            get
            {
                if (_Variants == null)
                {
                    _Variants = new List<Variants>();
                }
                return _Variants;
            }
            set
            {
                _Variants = value;
            }
        } 
        
        
        private List<Variants> _Selected;
                
        /// <summary>
        /// A list of Variants that have been selected from the list. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "selected")]
        public List<Variants> Selected
        {
            get
            {
                if (_Selected == null)
                {
                    _Selected = new List<Variants>();
                }
                return _Selected;
            }
            set
            {
                _Selected = value;
            }

        }

        #region serialization methods 

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        public bool SerializeSelected = false;

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS. 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
        public string ToJsonStringForOrder()
        {
            SerializeSelected = true;
            return base.ToJsonString();
        }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <param name="value"></param>
        public void SetSerializeForOrder(bool value)
        {
            SerializeSelected = value;
        }

        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// </summary>
        /// <returns></returns>
        public string ToJsonStringForProductUpdate()
        {
            SerializeSelected = true;
            return base.ToJsonString();
        }

        #endregion
    }
}
