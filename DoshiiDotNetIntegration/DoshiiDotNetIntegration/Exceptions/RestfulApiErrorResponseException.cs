using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DoshiiDotNetIntegration.Exceptions
{
    public class RestfulApiErrorResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        
        public RestfulApiErrorResponseException() : base() { }
        public RestfulApiErrorResponseException(HttpStatusCode code) : base() { StatusCode = code; }
        public RestfulApiErrorResponseException(string message) : base(message) { }
        public RestfulApiErrorResponseException(string message, Exception ex) : base(message, ex) { }
    }
}
