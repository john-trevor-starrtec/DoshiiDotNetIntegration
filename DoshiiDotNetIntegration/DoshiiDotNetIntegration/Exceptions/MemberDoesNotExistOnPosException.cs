using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Exceptions
{
    /// <summary>
    /// This exception should be thrown by the pos when doshii requests action on a member that does not exist on the pos. 
    /// </summary>
    public class MemberDoesNotExistOnPosException : Exception
    {
        public MemberDoesNotExistOnPosException() : base() { }
        public MemberDoesNotExistOnPosException(string message) : base(message) { }
        public MemberDoesNotExistOnPosException(string message, Exception ex) : base(message, ex) { }
    }
}
