using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
	/// <summary>
	/// This class is use internally within the SDK to communicate data related to <see cref="Models.Booking"/>
	/// </summary>
	internal class BookingEventArgs
	{
		internal String BookingId { get; set; }
		internal Booking Booking { get; set; }
	}
}
