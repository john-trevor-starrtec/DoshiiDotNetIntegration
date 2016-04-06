using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Models
{
	/// <summary>
	/// This model is a container that represents the order being placed with a table allocation.
	/// </summary>
	public class TableOrder
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public TableOrder()
		{
			Table = new TableAllocation();
			Order = new Order();
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Table.Clear();
			Order.Clear();
		}

		/// <summary>
		/// The table allocation for the order.
		/// </summary>
		public TableAllocation Table
		{
			get;
			set;
		}

		/// <summary>
		/// The details of the order.
		/// </summary>
		public Order Order
		{
			get;
			set;
		}
	}
}
