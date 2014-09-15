using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    public enum OrderStates
    {
        rejected = 1, 
        accepted = 2, 
        waitingforpayment = 3,
        paid = 4,
        pending = 5,
        readytopay = 6,
        cancelled = 7,
        New = 8
    }
}
