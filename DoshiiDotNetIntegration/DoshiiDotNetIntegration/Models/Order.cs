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
    public class Order
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
			UpdatedAt = DateTime.MinValue;
			OrderUri = String.Empty;
			_items.Clear();
		}

        /// <summary>
        /// Order id
        /// </summary>
        public string Id { get; set; }
        
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
		/// The last time the order was updated on Doshii
		/// </summary>
		public DateTime UpdatedAt { get; set; }

		/// <summary>
		/// The URI of the order
		/// </summary>
		public string OrderUri { get; set; }

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
    }
}
