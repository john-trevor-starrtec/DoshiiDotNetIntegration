using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{

    /// <summary>
    /// Payments that are made on the Doshii check
    /// </summary>
    public class Payment
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public Payment()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			PaymentType = String.Empty;
			PaymentAmount = 0.0M;
		}

        /// <summary>
        /// A string describing the payment method eg.. "Cash", "EFTPOS"
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// The amount that has been paid in cents. 
        /// </summary>
        public decimal PaymentAmount { get; set; }
    }
}
