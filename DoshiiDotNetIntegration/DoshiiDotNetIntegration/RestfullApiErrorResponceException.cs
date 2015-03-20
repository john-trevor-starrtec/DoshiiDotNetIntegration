using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration
{
    public class RestfullApiErrorResponceException : Exception
    {
        public RestfullApiErrorResponceException() : base() { }
        public RestfullApiErrorResponceException(string message) : base(message) { }
        public RestfullApiErrorResponceException(string message, Exception ex) : base(message, ex) { }
    }
}
