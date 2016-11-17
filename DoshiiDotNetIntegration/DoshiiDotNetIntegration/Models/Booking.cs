using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
	public class Booking
	{
		public String Id { get; set; }
		public List<String> TableNames { get; set; }
		public DateTime Date { get; set; }
		public int Covers { get; set; }
		public Consumer Consumer { get; set; }
        public String CheckinId { get; set; }
		public String App { get; set; }
		public DateTime UpdatedAt { get; set; }
		public DateTime CreatedAt { get; set; }
		public Uri Uri { get; set; }
	}
}
