﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This exception will be thrown when a product is not created as expected. 
    /// </summary>
    public class ProductNotCreatedException : Exception
    {
        public ProductNotCreatedException() : base() { }
        public ProductNotCreatedException(string message) : base(message) { }
        public ProductNotCreatedException(string message, Exception ex) : base(message, ex) { }
    }
}
