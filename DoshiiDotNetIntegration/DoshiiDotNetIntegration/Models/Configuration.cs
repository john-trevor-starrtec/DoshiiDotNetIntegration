using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Models
{
	/// <summary>
	/// The model class for configuration within the Doshii API.
	/// </summary>
	public class Configuration
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public Configuration()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			CheckoutOnPaid = false;
			DeallocateTableOnPaid = false;
		}

		/// <summary>
		/// True indicates that the consumer is checked out once they have paid for their order.
		/// </summary>
		public bool CheckoutOnPaid
		{
			get;
			set;
		}

		/// <summary>
		/// True indicates that the table associated with an order is deallocated once it has been paid.
		/// </summary>
		public bool DeallocateTableOnPaid
		{
			get;
			set;
		}
	}
}
