using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{

    /// <summary>
    /// Transactions that are linked to orders in Doshii
    /// </summary>
    public class Transaction : ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public Transaction()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Id = String.Empty;
            OrderId = String.Empty;
            Reference = String.Empty;
            Invoice = String.Empty;
		    PaymentAmount = 0.0M;
		    AcceptLess = false;
		    PartnerInitiated = false; 
            Partner = String.Empty;
		    Status = "pending";
            Version = String.Empty;
            Uri = String.Empty;
		}

        /// <summary>
        /// Unique number identifying this resource
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// identify the order this transaction relates to
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// info about the payment
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// partner identifier for the transaction
        /// </summary>
        public string Invoice { get; set; }

        /// <summary>
        /// The amount that has been paid in cents. 
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// flag indicating if the pos will accept less than the total amount as a payment from the partner
        /// </summary>
        public bool AcceptLess { get; set; }

        /// <summary>
        /// flag indicating if the transaction was initiated by the partner
        /// </summary>
        public bool PartnerInitiated { get; set; }

        /// <summary>
        /// identifier for the partner that completed the transaction
        /// </summary>
        public string Partner { get; set; }

        /// <summary>
        /// The current transaction status, pending, waiting, complete
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// An obfuscated string representation of the version for the order in Doshii.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The URI of the order
        /// </summary>
        public string Uri { get; set; }


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
