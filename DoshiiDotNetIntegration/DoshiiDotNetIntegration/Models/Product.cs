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
    public class Product : ICloneable
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
		/// The total price of the line item before surcounts are included qty * unit price. 
		/// </summary>
		public decimal TotalBeforeSurcounts { get; set; }

        /// <summary>
        /// The total price of the line item after surcounts are included qty * unit price + surcount. 
        /// </summary>
        public decimal TotalAfterSurcounts { get; set; }

        /// <summary>
        /// the unit price of the item 
        /// </summary>
        public decimal UnitPrice { get; set; }

        private List<string> _Tags;
        
        /// <summary>
        /// A list of the groups the product should be displayed under in the doshii mobile app
        /// This field is optional,
        /// Products can be added manually to groups in the doshii dashboard,
        /// If this list is populated the product will be automatically added to the groups included, this will reduce setup time. 
        /// </summary>
        public List<string> Tags 
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

        private List<Surcount> _ProductSurcounts;

        /// <summary>
        /// A list of Surcharges available / selected for this product on the product.
        /// </summary>
        public IEnumerable<Surcount> ProductSurcounts
        {
            get
            {
                if (_ProductSurcounts == null)
                {
                    _ProductSurcounts = new List<Surcount>();
                }
                return _ProductSurcounts;
            }
            set
            {
                _ProductSurcounts = value.ToList<Surcount>();
            }
        }

		/// <summary>
		/// The obfuscated string representation of the version for the product.
		/// </summary>
		public string Version { get; set; }

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
        public decimal Quantity { get; set; }

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
			TotalBeforeSurcounts = 0.0M;
		    TotalAfterSurcounts = 0.0M;
		    UnitPrice = 0.0M;
			_Tags.Clear();
			_ProductOptions.Clear();
			Version = String.Empty;
			PosId = String.Empty;
			Image = String.Empty;
			Quantity = 0.0M;
		}

		#region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the instance.</returns>
		public object Clone()
		{
			var product = (Product)this.MemberwiseClone();

			var tags = new List<string>();
			foreach (string tag in this.Tags)
				tags.Add(tag);
			product.Tags = tags;

			var options = new List<ProductOptions>();
			foreach (var option in this.ProductOptions)
				options.Add((ProductOptions)option.Clone());
			product.ProductOptions = options;

			return product;
		}

		#endregion
	}
}
