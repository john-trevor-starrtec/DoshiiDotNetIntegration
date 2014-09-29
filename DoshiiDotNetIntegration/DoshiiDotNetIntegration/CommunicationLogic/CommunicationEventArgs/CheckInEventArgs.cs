using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    public class CheckInEventArgs : EventArgs 
    {
        public string CheckIn;

        public string PaypalCustomerId;

        public Uri Uri;

        public Models.Consumer Consumer;
    }
}
