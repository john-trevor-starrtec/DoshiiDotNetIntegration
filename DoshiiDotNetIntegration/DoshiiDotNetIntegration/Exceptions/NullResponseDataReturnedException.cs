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
    internal class NullResponseDataReturnedException : Exception
    {
        internal NullResponseDataReturnedException() : base() { }
        internal NullResponseDataReturnedException(string message) : base(message) { }
        internal NullResponseDataReturnedException(string message, Exception ex) : base(message, ex) { }
    }
}
