using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// the reasons a table allocation from doshii was rejected. 
    /// </summary>
    public enum TableAllocationRejectionReasons
    {
        TableDoesNotExist = 1,
 
        TableIsOccupied = 2,

        CheckinWasDeallocatedByPos = 3,

        ConcurrencyIssueWithPos = 4
    }
}
