using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class product_options : JsonSerializationBase<product_options>
    {
        /// <summary>
        /// the name of this product options - or list of vairents
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// the minimum amount of varients that must be chosen from this set of varients
        /// </summary>
        public int min { get; set; }

        /// <summary>
        /// the maximum amoutn of varients that can be chosen form this set of varients. 
        /// </summary>
        public int max { get; set; }

        /// <summary>
        /// the internal pos identifier for this product list. 
        /// </summary>
        public string pos_id { get; set; }

        public List<variants> variants = new List<variants>();

        public List<variants> selected = new List<variants>();

        #region serialization methods 

        private bool serializeselected = false;

        public bool ShouldSerializeselected()
        {
            if (serializeselected)
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
            serializeselected = true;
            return base.ToJsonString();
        }

        public void SetSerializeForOrder(bool value)
        {
            serializeselected = value;
        }

        #endregion
    }
}
