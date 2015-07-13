using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    internal class CheckOutEventArgs : EventArgs
    {
        internal string MeerkatConsumerId { get; set; }
    }
}
