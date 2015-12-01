using DoshiiDotNetIntegration.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    internal class OrderEventArgs : EventArgs 
    {
        internal string OrderId { get; set; }

		internal string Status { get; set; }

		internal JsonOrder Order { get; set; }
    }
}
