using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// A Doshii order
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonOrder : JsonSerializationBase<JsonOrder>
    {
        /// <summary>
        /// Order id
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        /// <summary>
        /// Order status
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        /// <summary>
        /// Unique identifier for the invoice once the order is paid for.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId{ get; set; }
        
        /// <summary>
        /// Unique transaction identifier for the order.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }
        
        /// <summary>
        /// The CheckinId the order is associated with
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "checkinId")]
        public string CheckinId { get; set; }

		/// <summary>
		/// The Id of the location that the order was created in.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "locationId")]
		public string LocationId { get; set; }

		private List<JsonTransaction> _payments;

		/// <summary>
		/// A list of all payments applied from the pos at an order level. 
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "payments")]
		public List<JsonTransaction> Payments
		{
			get
			{
				if (_payments == null)
				{
					_payments = new List<JsonTransaction>();
				}
				return _payments;
			}
			set { _payments = value; }
		}

		private List<JsonSurcount> _surcounts;

		/// <summary>
		/// A list of all surcounts applied at and order level
		/// Surcounts are discounts and surcharges / discounts should have a negative value. 
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "surcounts")]
		public List<JsonSurcount> Surcounts
		{
			get
			{
				if (_surcounts == null)
				{
					_surcounts = new List<JsonSurcount>();
				}
				return _surcounts;
			}
			set { _surcounts = value; }
		}
        
        /// <summary>
        /// Total tip amount in cents associated with the order.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "tip")]
        public string Tip { get; set; }

        /// <summary>
        /// This is used by Doshii when splitting the bill - should not be changed on the pos.
        /// </summary>
        [JsonProperty(PropertyName = "paySplits")]
        public int PaySplits { get; set; }

        /// <summary>
        /// This is used by Doshii when splitting the bill - should not be changed on the pos.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "splitWays")]
        public int SplitWays { get; set; }

        /// <summary>
        /// The amount that is being paid in cents when a payment is made from Doshii.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "payTotal")]
        public string PayTotal { get; set; }

        /// <summary>
        /// The amount that has not been paid (in cents).
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "notPayingTotal")]
        public string NotPayingTotal { get; set; }

		/// <summary>
		/// The last time the order was updated on Doshii
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "updatedAt")]
		public string UpdatedAt { get; set; }

		/// <summary>
		/// The URI of the order
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "uri")]
		public string OrderUri { get; set; }

        private List<JsonProduct> _items;
        
        /// <summary>
        /// A list of all the items included in the order. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "items")]
		public List<JsonProduct> Items
		{
            get
            {
                if (_items == null)
                {
					_items = new List<JsonProduct>();
                }
                return _items;
            }
			set { _items = value; } 
        }
    }
}
