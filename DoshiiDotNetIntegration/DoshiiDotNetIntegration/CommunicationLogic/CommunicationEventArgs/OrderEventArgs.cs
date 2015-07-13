using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    internal class OrderEventArgs : EventArgs 
    {
        internal string OrderId;

        internal string Status;

        internal Models.Order Order;
    }
}
