using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    class TransactionDoesNotExistOnPosException: Exception
    {
        public TransactionDoesNotExistOnPosException() : base() { }
        public TransactionDoesNotExistOnPosException(string message) : base(message) { }
        public TransactionDoesNotExistOnPosException(string message, Exception ex) : base(message, ex) { }
    }
    
}
