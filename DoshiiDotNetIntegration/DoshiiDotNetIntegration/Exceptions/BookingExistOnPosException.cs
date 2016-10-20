using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Exceptions
{
	/// <summary>
	/// This exception should be thrown by the pos when doshii requests action on a booking that already exists on the pos. 
	/// </summary>
	public class BookingExistOnPosException : Exception
	{
		public BookingExistOnPosException() : base()
		{
		}

		public BookingExistOnPosException(String message) : base(message)
		{
		}

		public BookingExistOnPosException(String message, Exception ex) : base(message, ex)
		{
		}

	}
}
