using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This exception will be thrown when a method is called on <see cref="DoshiiController"/> and <see cref="DoshiiController.Initialize"/> has not been successfully called.
    /// </summary>
    public class DoshiiManagerNotInitializedException : Exception
    {
        public DoshiiManagerNotInitializedException() : base() { }
        public DoshiiManagerNotInitializedException(string message) : base(message) { }
        public DoshiiManagerNotInitializedException(string message, Exception ex) : base(message, ex) { }
    }
}
