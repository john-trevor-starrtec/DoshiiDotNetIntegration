using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    public class TransactionRequestNotProcessedException: Exception
    {
        public TransactionRequestNotProcessedException() : base() { }
        public TransactionRequestNotProcessedException(string message) : base(message) { }
        public TransactionRequestNotProcessedException(string message, Exception ex) : base(message, ex) { }
    }
}
