using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Exceptions
{
	/// <summary>
	/// This exception should be thrown by the pos when doshii requests action on a booking that does not exist on the pos. 
	/// </summary>
	public class BookingDoesNotExistOnPosException : Exception
	{
		public BookingDoesNotExistOnPosException() : base()
		{
		}

		public BookingDoesNotExistOnPosException(String message) : base(message)
		{
		}

		public BookingDoesNotExistOnPosException(String message, Exception ex) : base(message, ex)
		{
		}
	}
}
