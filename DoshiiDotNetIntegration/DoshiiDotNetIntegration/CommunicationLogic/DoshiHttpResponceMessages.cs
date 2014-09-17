using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    internal class DoshiHttpResponceMessages
    {
        internal HttpStatusCode Status { get; set; }

        internal string StatusDescription { get; set; }

        internal string Data { get; set; }

        internal string ErrorMessage { get; set; }

        internal string Message { get; set; }

    }
}
