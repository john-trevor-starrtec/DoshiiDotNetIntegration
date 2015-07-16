using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    internal class CheckInEventArgs : EventArgs 
    {
        internal string CheckIn;

        internal string MeerkatCustomerId;

        internal Uri Uri;

        internal Models.Consumer Consumer;
    }
}
