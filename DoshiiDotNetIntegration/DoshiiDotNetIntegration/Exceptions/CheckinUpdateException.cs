using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This is the general exception that is thrown from UpdateOrder when the order update was unsuccessful.
    /// </summary>
    public class CheckinUpdateException: Exception
    {
        public CheckinUpdateException() : base() { }
        public CheckinUpdateException(string message) : base(message) { }
        public CheckinUpdateException(string message, Exception ex) : base(message, ex) { }
    }
}
