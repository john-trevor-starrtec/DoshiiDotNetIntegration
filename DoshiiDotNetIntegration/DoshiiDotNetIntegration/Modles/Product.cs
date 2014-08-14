using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class Product : JsonSerializationBase<Product>
    {
        /// <summary>
        /// The Doshii Id of the prduct this will be provided by Doshii
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// The internal Id of the product
        /// </summary>
        public string pos_Id { get; set; }

        /// <summary>
        /// The name of the product that will be displayed to the user on the Doshii application
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// A list of the tags the product should be displayed under in the mobile menu
        /// </summary>
        List<string> tags { get; set; }

        /// <summary>
        /// The price the product will be sold for through the mobile app, 
        /// This price is to be represented in cents. 
        /// </summary>
        public int price { get; set; }

        /// <summary>
        /// A description of the product that will be displayed on the mobile app
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// A list of product options available for the product. 
        /// </summary>
        public List<product_options> product_options { get; set; }



    }
}
