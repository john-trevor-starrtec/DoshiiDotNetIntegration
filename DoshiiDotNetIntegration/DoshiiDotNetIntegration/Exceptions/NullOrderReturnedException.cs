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
    internal class NullOrderReturnedException : Exception
    {
        internal NullOrderReturnedException() : base() { }
        internal NullOrderReturnedException(string message) : base(message) { }
        internal NullOrderReturnedException(string message, Exception ex) : base(message, ex) { }
    }
}
