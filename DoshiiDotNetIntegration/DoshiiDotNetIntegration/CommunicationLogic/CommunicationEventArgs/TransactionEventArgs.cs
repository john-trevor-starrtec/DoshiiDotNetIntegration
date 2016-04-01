using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    /// <summary>
    /// This class is use internally within the SDK to communicate data related to <see cref="Models.Transaction"/>
    /// </summary>
    internal class TransactionEventArgs
    {
        internal string TransactionId { get; set; }

        internal string Status { get; set; }

        internal Transaction Transaction { get; set; }
    }
}
