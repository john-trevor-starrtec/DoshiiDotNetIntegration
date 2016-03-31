using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This exception should be thrown by the pos when doshii requests action on a transaction that does not exist on the pos. 
    /// </summary>
    public class TransactionDoesNotExistOnPosException: Exception
    {
        public TransactionDoesNotExistOnPosException() : base() { }
        public TransactionDoesNotExistOnPosException(string message) : base(message) { }
        public TransactionDoesNotExistOnPosException(string message, Exception ex) : base(message, ex) { }
    }
    
}
