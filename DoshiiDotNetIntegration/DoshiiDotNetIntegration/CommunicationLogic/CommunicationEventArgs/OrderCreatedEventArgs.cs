using DoshiiDotNetIntegration.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    /// <summary>
    /// This class is used internally within the SDK to communicate data related to <see cref="Models.Order"/>
    /// </summary>
    internal class OrderCreatedEventArgs : EventArgs 
    {
        internal string OrderId { get; set; }

		internal string Status { get; set; }

		internal OrderWithConsumer Order { get; set; }

        internal IEnumerable<Transaction> TransactionList { get; set; }
    }
}
