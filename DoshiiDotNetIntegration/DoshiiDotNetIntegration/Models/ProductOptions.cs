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
    public class ProductOptions : ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public ProductOptions()
		{
			_Variants = new List<Variants>();
			_Selected = new List<Variants>();
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Name = String.Empty;
			Min = 0;
			Max = 0;
			PosId = String.Empty;
			_Variants.Clear();
			_Selected.Clear();
		}

        /// <summary>
        /// The name of this product options / or list of variants
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The minimum amount of variants that must be chosen from this set of variants
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// The maximum amount of variants that can be chosen form this set of variants. 
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// The POS identifier for this set of variants. 
        /// </summary>
        public string PosId { get; set; }

        private List<Variants> _Variants;

        /// <summary>
        /// A List of Variants available to be selected from this list. 
        /// </summary>
        public IEnumerable<Variants> Variants 
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
                _Variants = value.ToList<Variants>();
            }
        } 
        
        private List<Variants> _Selected;
                
        /// <summary>
        /// A list of Variants that have been selected from the list. 
        /// </summary>
        public IEnumerable<Variants> Selected
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
                _Selected = value.ToList<Variants>();
            }

        }

		#region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the instance.</returns>
		public object Clone()
		{
			var options = (ProductOptions)this.MemberwiseClone();

			var variants = new List<Variants>();
			foreach (var variant in this.Variants)
			{
				variants.Add((Variants)variant.Clone());
			}
			options.Variants = variants;

			var selectedVariants = new List<Variants>();
			foreach (var variant in this.Selected)
			{
				selectedVariants.Add((Variants)variant.Clone());
			}
			options.Selected = selectedVariants;

			return options;
		}

		#endregion
	}
}
