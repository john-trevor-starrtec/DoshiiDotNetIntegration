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
        /// Unique identifier for the invoice once the order is paid for.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId{ get; set; }
        
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
