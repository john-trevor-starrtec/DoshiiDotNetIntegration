using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    public class CheckOutEventArgs : EventArgs
    {
        public string ConsumerId { get; set; }
    }
}
