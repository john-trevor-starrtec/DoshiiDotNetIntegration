using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DoshiiDotNetIntegration.Models.Json
{

	[DataContract]
	[Serializable]
	internal class JsonTableOrder : JsonSerializationBase<JsonTableOrder>
	{

		[DataMember]
		[JsonProperty(PropertyName = "table")]
		public JsonTableAllocation Table
		{
			get;
			set;
		}


		[DataMember]
		[JsonProperty(PropertyName = "order")]
		public JsonOrder Order
		{
			get;
			set;
		}
		
	}
}
