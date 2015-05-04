﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This exception will be thrown when there is a conflict during a PUT or a POST of an order to doshii.
    /// This exception is handled by the Doshii SDK and does not need to be handled by the pos. 
    /// </summary>
    public class ConflictWithOrderUpdateException : Exception
    {
        public ConflictWithOrderUpdateException() : base() { }
        public ConflictWithOrderUpdateException(string message) : base(message) { }
        public ConflictWithOrderUpdateException(string message, Exception ex) : base(message, ex) { }
    }
}
