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
    [DataContract(Name = "order")]
    [Serializable]
    internal class JsonOrder : JsonSerializationBase<JsonOrder>
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Order id
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "doshiiId")]
        public string DoshiiId { get; set; }
        

        /// <summary>
        /// Order status
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Order Type 'delivery' or 'pickup'
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        
        /// <summary>
        /// Unique identifier for the invoice once the order is paid for.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId{ get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "memberId")]
        public string MemberId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "phase")]
        public string Phase { get; set; }

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

		private List<JsonOrderSurcount> _surcounts;

		/// <summary>
		/// A list of all surcounts applied at and order level
		/// Surcounts are discounts and surcharges / discounts should have a negative value. 
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "surcounts")]
		public List<JsonOrderSurcount> Surcounts
		{
			get
			{
				if (_surcounts == null)
				{
					_surcounts = new List<JsonOrderSurcount>();
				}
				return _surcounts;
			}
			set { _surcounts = value; }
		}
        
        /// <summary>
		/// An obfuscated string representation of the version of the order in Doshii.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }

		/// <summary>
		/// The URI of the order
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "uri")]
		public string Uri { get; set; }

        /// <summary>
        /// the dateTime the order is Required
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "requiredAt")]
        public DateTime? RequiredAt { get; set; }

        private List<JsonOrderProduct> _items;
        
        /// <summary>
        /// A list of all the items included in the order. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "items")]
		public List<JsonOrderProduct> Items
		{
            get
            {
                if (_items == null)
                {
					_items = new List<JsonOrderProduct>();
                }
                return _items;
            }
			set { _items = value; } 
        }
    }
}
