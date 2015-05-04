using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// The reasons a table allocation from doshii was rejected / deleted. 
    /// The Doshii app will show a different message to the consumer depending on the rejection reason that is provided
    /// </summary>
    public enum TableAllocationRejectionReasons
    {
        /// <summary>
        /// The table does not exist on the pos
        /// </summary>
        TableDoesNotExist = 1,
        
        /// <summary>
        /// The table is occupied on the pos and the user cannot be seated at it. 
        /// </summary>
        TableIsOccupied = 2,

        /// <summary>
        /// The Pos has deallocation the checkin from the table allocation,
        /// This might happen when the checkin is moved to a different table on the pos
        /// </summary>
        CheckinWasDeallocatedByPos = 3,

        /// <summary>
        /// There was a concurrency Issue with the Checkin Allocation on the pos
        /// eg the checkin was moved on the pos at the same time the checkin was moved on doshii and the allocation message for doshii couldn't be processed because the checkin was no longer at the original table. 
        /// </summary>
        ConcurrencyIssueWithPos = 4,

        /// <summary>
        /// There is no Tab / Order / check associated with the checkin on the pos. 
        /// </summary>
        tableDoesNotHaveATab = 5,

        /// <summary>
        /// The table has been paid on the Pos, this will be sent up automatically when the pos provides an order to be updated with a status of Paid. 
        /// </summary>
        tableHasBeenPaid = 6,

        /// <summary>
        /// There was an unknown error linking the checkin and the table on the Pos
        /// </summary>
        unknownError = 7
    }
}
