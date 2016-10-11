using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// This object is used to deserialize json orders that have a price element that cannot be converted to decimal, 
    /// This is only used so the order can be automatically rejected by the sdk. 
    /// </summary>
    internal class OrderWithNoPriceProperties: ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public OrderWithNoPriceProperties()
		{
			Clear();
		}


        public string Phase { get; set; }

        public string MemberId { get; set; }

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Id = String.Empty;
		    DoshiiId = String.Empty;
			Status = String.Empty;
			InvoiceId = String.Empty;
			CheckinId = String.Empty;
			LocationId = String.Empty;
			Version = String.Empty;
			Uri = String.Empty;
			RequiredAt = null;
		    Phase = string.Empty;
		}

        /// <summary>
        /// Order id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Doshii Id for the order, this is used to get unlinked orders
        /// </summary>
        public string DoshiiId { get; set; }

        /// <summary>
        /// Order status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// type of order 'delivery' or 'pickup'
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Unique identifier for the invoice once the order is paid for.
        /// </summary>
        public string InvoiceId{ get; set; }
        
        /// <summary>
        /// The CheckinId the order is associated with, the doshii system uses this checkinId to relate tables to orders, to delete a table allocation you must have the
        /// order checkIn Id. 
        /// </summary>
        public string CheckinId { get; set; }

		/// <summary>
		/// The Id of the location that the order was created in.
		/// </summary>
		public string LocationId { get; set; }

		/// <summary>
		/// An obfuscated string representation of the version for the order in Doshii.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// The URI of the order
		/// </summary>
		public string Uri { get; set; }

        /// <summary>
        /// the time the order is required if it is required in the future, 
        /// string will be empty is it is required now. 
        /// </summary>
        public DateTime? RequiredAt { get; set; }
        
        #region ICloneable Members

		/// <summary>
		/// Returns a deep copy of the instance.
		/// </summary>
		/// <returns>A clone of the calling instance.</returns>
		public object Clone()
		{
			var order = (Order)this.MemberwiseClone();

			// Memberwise clone doesn't handle recursive cloning of internal properties such as lists
			// here I am overwriting the list with cloned copies of the list items
			return order;
		}

		#endregion
	}
}

