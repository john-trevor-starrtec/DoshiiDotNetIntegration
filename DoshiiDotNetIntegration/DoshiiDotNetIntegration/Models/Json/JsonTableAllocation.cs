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
	internal class JsonTableAllocation : JsonSerializationBase<JsonTableAllocation>
	{
		/// <summary>
		/// This is the CheckInId associated with the TableAllocation
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }

		/// <summary>
		/// This is the Name of the table the CheckIn is associated with.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		/// <summary>
		/// The allocation status of the table. 
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "status")]
		public string Status { get; set; }

		/// <summary>
		/// An obfuscated string representation of the table version in Doshii.
		/// </summary>
		[DataMember]
		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }
	}
}