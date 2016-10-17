using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This exception should be thrown by the pos when doshii requests action on a member that does not exist on the pos. 
    /// </summary>
    public class MemberIncompleteException : Exception
    {
        public MemberIncompleteException() : base() { }
        public MemberIncompleteException(string message) : base(message) { }
        public MemberIncompleteException(string message, Exception ex) : base(message, ex) { }
    }
}
