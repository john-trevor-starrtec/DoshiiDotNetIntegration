using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Exceptions
{
    public class BookingUpdateException : Exception
    {
        public BookingUpdateException() : base()
        {
        }

        public BookingUpdateException(String message) : base(message)
        {
        }

        public BookingUpdateException(String message, Exception ex) : base(message, ex)
        {
        }

    }
}
