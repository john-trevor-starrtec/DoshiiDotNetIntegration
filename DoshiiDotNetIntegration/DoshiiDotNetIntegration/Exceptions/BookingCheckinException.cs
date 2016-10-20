using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Exceptions
{
    public class BookingCheckinException : Exception
    {
        public BookingCheckinException() : base()
        {
        }

        public BookingCheckinException(String message) : base(message)
        {
        }

        public BookingCheckinException(String message, Exception ex) : base(message, ex)
        {
        }
    }
}
