using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
	/// <summary>
	/// An object representing a doshii booking
	/// </summary>
    public class Booking
	{
		/// <summary>
		/// the Id of the booking
		/// </summary>
        public String Id { get; set; }

        /// <summary>
        /// the table names associated with the booking. 
        /// </summary>
		public List<String> TableNames { get; set; }

        /// <summary>
        /// the dateTime of the booking. 
        /// </summary>
		public DateTime Date { get; set; }

        /// <summary>
        /// the amount of covers associated with the booking. 
        /// </summary>
		public int Covers { get; set; }

        /// <summary>
        /// the <see cref="Consumer"/> associated with the booking. 
        /// </summary>
		public Consumer Consumer { get; set; }
        public String CheckinId { get; set; }

        /// <summary>
        /// the <see cref="App"/> associated with the booking. 
        /// </summary>
		public String App { get; set; }

        /// <summary>
        /// the last time the booking was updated
        /// </summary>
		public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// the time the booking as created.
        /// </summary>
		public DateTime CreatedAt { get; set; }

        /// <summary>
        /// the Uri to retreive the booking. 
        /// </summary>
		public Uri Uri { get; set; }
	}
}
