using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    public class NullOrderReturnedException : Exception
    {
        public NullOrderReturnedException() : base() { }
        public NullOrderReturnedException(string message) : base(message) { }
        public NullOrderReturnedException(string message, Exception ex) : base(message, ex) { }
    }
}
