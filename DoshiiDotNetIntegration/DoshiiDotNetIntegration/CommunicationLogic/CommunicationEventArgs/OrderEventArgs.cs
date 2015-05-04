﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    public class OrderEventArgs : EventArgs 
    {
        public string OrderId;

        public string Status;

        public Models.Order Order;
    }
}
