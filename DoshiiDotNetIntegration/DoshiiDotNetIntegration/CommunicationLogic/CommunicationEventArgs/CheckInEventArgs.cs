using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.CommunicationLogic.CommunicationEventArgs
{
    public class CheckInEventArgs : EventArgs 
    {
        public string consumer;

        public string checkin;

        public string paypalCustomerId;

        public Uri uri;

        public Modles.Consumer consumerObject;
    }
}
