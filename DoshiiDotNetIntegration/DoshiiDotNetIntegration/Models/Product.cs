using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// The doshii representation of a product 
    /// A product is the highest level line item on orders.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The Doshii Id of the product.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the product.
        /// This name will be displayed to Doshii users on the mobile app.
        /// </summary>
        public string Name { get; set; }

		/// <summary>
		/// A description of the product that will be displayed on the mobile app
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The price the product will be sold for through the mobile app. 
		/// </summary>
		public decimal Price { get; set; }

        private List<string> _Tags;
        
        /// <summary>
        /// A list of the groups the product should be displayed under in the doshii mobile app
        /// This field is optional,
        /// Products can be added manually to groups in the doshii dashboard,
        /// If this list is populated the product will be automatically added to the groups included, this will reduce setup time. 
        /// </summary>
        public IEnumerable<string> Tags 
        { 
            get
            { 
                if (_Tags == null)
                {
                    _Tags = new List<string>();
                }
                return _Tags;
            }
            set
            {
                _Tags = value.ToList<string>();
            }
        }

        private List<ProductOptions> _ProductOptions;
        
        /// <summary>
        /// A list of ProductOptions the customer can choose from to modify the product they are ordering.
        /// </summary>
        public IEnumerable<ProductOptions> ProductOptions {
            get
            {
                if (_ProductOptions == null)
                {
                    _ProductOptions = new List<ProductOptions>();
                }
                return _ProductOptions;
            } 
            set
            {
                _ProductOptions = value.ToList<ProductOptions>();
            }
        }

		/// <summary>
		/// The date and time of the last update on the product.
		/// </summary>
		public DateTime UpdatedAt { get; set; }

		/// <summary>
		/// The POS Id of the product
		/// </summary>
		public string PosId { get; set; }

		/// <summary>
		/// The image associated with the product.
		/// </summary>
		public string Image { get; set; }

        /// <summary>
        /// The status of the item that is being ordered. 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Product()
		{
			_Tags = new List<string>();
			_ProductOptions = new List<ProductOptions>();
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Id = String.Empty;
			Name = String.Empty;
			Description = String.Empty;
			Price = 0.0M;
			_Tags.Clear();
			_ProductOptions.Clear();
			UpdatedAt = DateTime.MinValue;
			PosId = String.Empty;
			Image = String.Empty;
			Status = String.Empty;
		}
    }
}
