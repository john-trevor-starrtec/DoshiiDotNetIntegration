using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DoshiiDotNetIntegration.Models.Json
{
	/// <summary>
	/// The data transfer object used in internal communication between the SDK and the RESTful API
	/// for an order containing a table allocation.
	/// </summary>
	[DataContract]
	[Serializable]
	internal class JsonTableOrder : JsonSerializationBase<JsonTableOrder>
	{
		/// <summary>
		/// The table allocation for the order.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "table")]
        public JsonTableAllocationForCreate Table
		{
			get;
			set;
		}

		/// <summary>
		/// The order details.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "order")]
		public JsonOrderToPut Order
		{
			get;
			set;
		}
		
	}
}
