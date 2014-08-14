using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class product_options : JsonSerializationBase<product_options>
    {
        /// <summary>
        /// a list of variants available in this product options set. 
        /// </summary>
        public List<variants> variants { get; set; }

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

    }
}
