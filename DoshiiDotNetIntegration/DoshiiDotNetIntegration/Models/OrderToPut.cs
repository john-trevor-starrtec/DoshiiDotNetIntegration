using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// DO NOT USE FROM POS
    /// This model is used when either completing a PUT or a POST to update an order
    /// This model should not instantiated by the Pos and should only be used internally. 
    /// </summary>
	public class OrderToPut
	{
		/// <summary>
		/// Private placeholder for the items in the order.
		/// </summary>
		private List<Product> mItems;

		/// <summary>
		/// Private placeholder for the surcounts in the order.
		/// </summary>
		private List<Surcount> mSurcounts;

		/// <summary>
		/// Private placeholder for the payments in the order.
		/// </summary>
		private List<Transaction> mPayments;

		/// <summary>
		/// Constructor.
		/// </summary>
		public OrderToPut()
		{
			mItems = new List<Product>();
			mSurcounts = new List<Surcount>();
			mPayments = new List<Transaction>();
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Status = String.Empty;
			Version = String.Empty;
			mItems.Clear();
			mSurcounts.Clear();
			mPayments.Clear();
		}

		/// <summary>
		/// The order status
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// The obfuscated string version number for the order in Doshii.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// All the items included in the order. 
		/// </summary>
		public IEnumerable<Product> Items
		{
			get
			{
				if (mItems == null)
					mItems = new List<Product>();
				return mItems;
			}
			set
			{
				mItems = value.ToList<Product>();
			}
		}

		/// <summary>
		/// A list of all surcounts applied at the order level
		/// </summary>
		public IEnumerable<Surcount> Surcounts
		{
			get
			{
				if (mSurcounts == null)
					mSurcounts = new List<Surcount>();
				return mSurcounts;
			}
			set
			{
				mSurcounts = value.ToList<Surcount>();
			}
		}

		/// <summary>
		/// A list of all payments applied at the order level
		/// </summary>
		public IEnumerable<Transaction> Payments
		{
			get
			{
				if (mPayments == null)
					mPayments = new List<Transaction>();
				return mPayments;
			}
			set
			{
				mPayments = value.ToList<Transaction>();
			}
		}
	}
}
