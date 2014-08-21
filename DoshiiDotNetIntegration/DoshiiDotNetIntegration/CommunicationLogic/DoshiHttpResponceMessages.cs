using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    public class DoshiHttpResponceMessages
    {
        public HttpStatusCode Status { get; set; }

        public string statusDescription { get; set; } 

        public string data { get; set; }

        public string errorMessage { get; set; }

        public string Message { get; set; }

    }
}
