using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Models
{
	public class TableOrder
	{

		public TableOrder()
		{
			Table = new TableAllocation();
			Order = new Order();
			Clear();
		}


		public void Clear()
		{
			Table.Clear();
			Order.Clear();
		}


		public TableAllocation Table
		{
			get;
			private set;
		}


		public Order Order
		{
			get;
			private set;
		}
	}
}
