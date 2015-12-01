using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// A table allocation object
    /// </summary>
	public class TableAllocation
	{
		/// <summary>
		/// This is the CheckInId associated with the TableAllocation
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// This is the Name of the table the CheckIn is associated with.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The allocation status of the table. 
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// Last date/time that the table allocation was updated.
		/// </summary>
		public DateTime UpdatedAt { get; set; }

		/// <summary>
		/// Constructor. 
		/// </summary>
		public TableAllocation()
		{
			Clear();
		}

		/// <summary>
		/// Resets all property values to default settings.
		/// </summary>
		public void Clear()
		{
			Id = String.Empty;
			Name = String.Empty;
			Status = String.Empty;
			UpdatedAt = DateTime.MinValue;
		}
	}
}