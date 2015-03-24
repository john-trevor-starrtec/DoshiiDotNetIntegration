using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration
{
    public class RestfulApiErrorResponseException : Exception
    {
        public RestfulApiErrorResponseException() : base() { }
        public RestfulApiErrorResponseException(string message) : base(message) { }
        public RestfulApiErrorResponseException(string message, Exception ex) : base(message, ex) { }
    }
}
