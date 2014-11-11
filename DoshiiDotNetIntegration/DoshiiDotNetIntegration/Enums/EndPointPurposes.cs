using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// the possible actions for communication with the doshii restful api
    /// </summary>
    internal enum EndPointPurposes
    {
        /// <summary>
        /// requests about products
        /// </summary>
        Products = 1,

        /// <summary>
        /// requests about consumers
        /// </summary>
        Consumer = 2,

        /// <summary>
        /// requests about orders
        /// </summary>
        Order = 3,

        /// <summary>
        /// getting a table allocation
        /// </summary>
        GetTableAllocations = 4,

        /// <summary>
        /// confirming a table allocation
        /// </summary>
        ConfirmTableAllocation = 5,

        /// <summary>
        /// delete the table allocation with the checkInId
        /// </summary>
        DeleteAllocationWithCheckInId = 6,

        AddTableAllocation = 7,

        SetSeatingAndOrderConfiguration = 8

    }
}
