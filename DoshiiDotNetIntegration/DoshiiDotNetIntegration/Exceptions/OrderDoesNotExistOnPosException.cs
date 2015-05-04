using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This exception should be thrown by the pos when doshii requests action on an order that does not exist on the pos. 
    /// </summary>
    public class OrderDoesNotExistOnPosException : Exception
    {
        public OrderDoesNotExistOnPosException() : base() { }
        public OrderDoesNotExistOnPosException(string message) : base(message) { }
        public OrderDoesNotExistOnPosException(string message, Exception ex) : base(message, ex) { }
    }
}
