using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    public class ConflictWithOrderUpdateException: Exception
    {
        public ConflictWithOrderUpdateException() : base() { }
        public ConflictWithOrderUpdateException(string message) : base(message) { }
        public ConflictWithOrderUpdateException(string message, Exception ex) : base(message, ex) { }
    }
}
