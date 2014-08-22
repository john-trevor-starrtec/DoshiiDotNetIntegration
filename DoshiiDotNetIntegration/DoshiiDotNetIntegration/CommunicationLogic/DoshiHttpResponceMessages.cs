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

        internal string statusDescription { get; set; }

        internal string data { get; set; }

        internal string errorMessage { get; set; }

        internal string Message { get; set; }

    }
}
