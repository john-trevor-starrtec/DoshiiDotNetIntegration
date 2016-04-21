using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This exception will be thrown when there is a conflict while attempting to update an order in doshii.
    /// <para/>This exception is handled by the Doshii SDK and does not need to be handled by the pos. 
    /// </summary>
    internal class ConflictWithOrderUpdateException : Exception
    {
        internal ConflictWithOrderUpdateException() : base() { }
        internal ConflictWithOrderUpdateException(string message) : base(message) { }
        internal ConflictWithOrderUpdateException(string message, Exception ex) : base(message, ex) { }
    }
}
