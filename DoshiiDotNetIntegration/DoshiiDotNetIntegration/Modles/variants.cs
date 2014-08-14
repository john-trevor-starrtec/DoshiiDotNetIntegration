using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    /// <summary>
    /// Varients are available to modify products on the Dohsii app,
    /// Examples of varients may include;
    /// Steak cooking methods eg 'rare, medium, or well done',
    /// sauces eg 'Tomato, bbq, sour cream'
    /// or sides eg 'chips, veg, salad'
    /// each varient can have a price attahed to it.
    /// </summary>
    public class variants : JsonSerializationBase<variants>
    {
        /// <summary>
        /// The name of the varient that will be displayed on the mobile app
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The price of the varient in cents
        /// </summary>
        public int price { get; set; }

        /// <summary>
        /// the internal Id of the product.
        /// </summary>
        public string pos_Id { get; set; }
    }
}
