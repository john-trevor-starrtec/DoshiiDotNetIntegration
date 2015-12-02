using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    internal class TransactionEventArgs
    {
        internal string TransactionId { get; set; }

        internal string Status { get; set; }

        internal Transaction Transaction { get; set; }
    }
}
