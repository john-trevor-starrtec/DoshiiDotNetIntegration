using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// A table allocation object
    /// </summary>
	[DataContract]
	[Serializable]
    internal class JsonTableAllocationForCreate : JsonSerializationBase<JsonTableAllocationForCreate>
	{
		/// <summary>
		/// This is the Name of the table the CheckIn is associated with.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

	}
}