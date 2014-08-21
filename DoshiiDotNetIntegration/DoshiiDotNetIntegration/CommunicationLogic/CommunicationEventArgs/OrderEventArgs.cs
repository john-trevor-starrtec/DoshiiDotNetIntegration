using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    public class OrderEventArgs : EventArgs 
    {
        public int OrderId;

        public Enums.OrderStates status;

        public Modles.order order;
    }
}
