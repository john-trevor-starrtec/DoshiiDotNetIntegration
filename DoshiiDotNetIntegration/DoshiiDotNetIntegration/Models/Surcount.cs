using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// Surcharges and Discounts that are applied at an order level.
    /// This model should not be used to record Product level discounts - discounts applied at a product level should be applied directly to the price attached to the product itself. 
    /// Surcharges should have a positive price.
    /// Discounts should have a negative price. 
    /// </summary>
    public class Surcount : ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public Surcount()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Name = String.Empty;
		    Type = String.Empty;
		    Id = String.Empty;
		    Value = 0.0M;
			Amount = 0.0M;
		    RewardId = string.Empty;
		}

        /// <summary>
        /// The Name of the surcount
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Amount / value of the surcount in cents. 
        /// </summary>
        public decimal Amount { get; set; }

        public decimal Value { get; set; }

        /// <summary>
        /// The type of the surcount ('absolute' or 'percentage')
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The posId for the product
        /// </summary>
        public string Id { get; set; }

        public string RewardId { get; set; }

		#region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the instance.</returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}
}
