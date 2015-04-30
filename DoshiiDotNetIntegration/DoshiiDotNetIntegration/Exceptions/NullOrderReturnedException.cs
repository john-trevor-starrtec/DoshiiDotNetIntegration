using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This exception will be thrown when the order returned by Doshii is Null
    /// This exception is handled by the Doshii SDK and does not need to be handled by the pos. 
    /// </summary>
    public class NullOrderReturnedException : Exception
    {
        public NullOrderReturnedException() : base() { }
        public NullOrderReturnedException(string message) : base(message) { }
        public NullOrderReturnedException(string message, Exception ex) : base(message, ex) { }
    }
}
