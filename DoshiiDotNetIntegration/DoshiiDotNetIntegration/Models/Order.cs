using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// A Doshii order
    /// </summary>
    public class Order : ICloneable
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public Order()
		{
			_payments = new List<Transaction>();
			_surcounts = new List<Surcount>();
			_items = new List<Product>();
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Id = String.Empty;
		    DoshiiId = String.Empty;
			Status = String.Empty;
			InvoiceId = String.Empty;
			TransactionId = String.Empty;
			CheckinId = String.Empty;
			LocationId = String.Empty;
			_payments.Clear();
			_surcounts.Clear();
			Tip = 0.0M;
			PaySplits = 0;
			SplitWays = 0;
			PayTotal = 0.0M;
			NotPayingTotal = 0.0M;
			Version = String.Empty;
			Uri = String.Empty;
			_items.Clear();
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
        /// Unique identifier for the invoice once the order is paid for.
        /// </summary>
        public string InvoiceId{ get; set; }
        
        /// <summary>
        /// Unique transaction identifier for the order.
        /// </summary>
        public string TransactionId { get; set; }
        
        /// <summary>
        /// The CheckinId the order is associated with
        /// </summary>
        public string CheckinId { get; set; }

		/// <summary>
		/// The Id of the location that the order was created in.
		/// </summary>
		public string LocationId { get; set; }

		private List<Transaction> _payments;

		/// <summary>
		/// A list of all payments applied from the pos at an order level. 
		/// </summary>
		public IEnumerable<Transaction> Payments
		{
			get
			{
				if (_payments == null)
				{
					_payments = new List<Transaction>();
				}
				return _payments;
			}
			set { _payments = value.ToList<Transaction>(); }
		}

		private List<Surcount> _surcounts;

		/// <summary>
		/// A list of all surcounts applied at and order level
		/// Surcounts are discounts and surcharges / discounts should have a negative value. 
		/// </summary>
		public IEnumerable<Surcount> Surcounts
		{
			get
			{
				if (_surcounts == null)
				{
					_surcounts = new List<Surcount>();
				}
				return _surcounts;
			}
			set { _surcounts = value.ToList<Surcount>(); }
		}
        
        /// <summary>
        /// Total tip amount in cents associated with the order.
        /// </summary>
        public decimal Tip { get; set; }

        /// <summary>
        /// This is used by Doshii when splitting the bill - should not be changed on the pos.
        /// </summary>
        public int PaySplits { get; set; }

        /// <summary>
        /// This is used by Doshii when splitting the bill - should not be changed on the pos.
        /// </summary>
        public int SplitWays { get; set; }

        /// <summary>
        /// The amount that is being paid in cents when a payment is made from Doshii.
        /// </summary>
        public decimal PayTotal { get; set; }

        /// <summary>
        /// The amount that has not been paid (in cents).
        /// </summary>
        public decimal NotPayingTotal { get; set; }

		/// <summary>
		/// An obfuscated string representation of the version for the order in Doshii.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// The URI of the order
		/// </summary>
		public string Uri { get; set; }

        private List<Product> _items;
        
        /// <summary>
        /// A list of all the items included in the order. 
        /// </summary>
        public IEnumerable<Product> Items {
            get
            {
                if (_items == null)
                {
                    _items = new List<Product>();
                }
                return _items;
            }
            set { _items = value.ToList<Product>(); } 
        }

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
			var payments = new List<Transaction>();
			foreach (var payment in this.Payments)
			{
				payments.Add((Transaction)payment.Clone());
			}
			order.Payments = payments;

			var surcounts = new List<Surcount>();
			foreach (var surcount in this.Surcounts)
			{
				surcounts.Add((Surcount)surcount.Clone());
			}
			order.Surcounts = surcounts;



			return order;
		}

		#endregion
	}
}
