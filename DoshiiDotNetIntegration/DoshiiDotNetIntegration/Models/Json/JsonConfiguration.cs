using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DoshiiDotNetIntegration.Models.Json
{
	/// <summary>
	/// The data transfer object representing the configuration of the Doshii integration.
	/// This class is only for use internally within the SDK and should not be accessed directly
	/// by the POS implementation. It is used to communicate with the RESTful API only.
	/// </summary>
	[DataContract]
	[Serializable]
	internal class JsonConfiguration : JsonSerializationBase<JsonConfiguration>
	{
		/// <summary>
		/// True indicates that the consumer is checked out once they have paid.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName="checkoutOnPaid")]
		public bool CheckoutOnPaid
		{
			get;
			set;
		}

		/// <summary>
		/// True indicates that a table is deallocated once its corresponding order has been paid.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName="deallocateTableOnPaid")]
		public bool DeallocateTableOnPaid
		{
			get;
			set;
		}
	}
}
