using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// The possible actions for communication with the doshii restful API over HTTP
    /// </summary>
    public enum EndPointPurposes
    {
        /// <summary>
        /// Requests about products
        /// </summary>
        Products = 1,

        /// <summary>
        /// Requests about consumers
        /// </summary>
        Consumer = 2,

        /// <summary>
        /// Requests about orders
        /// </summary>
        Order = 3,

        /// <summary>
        /// Getting a table allocation
        /// </summary>
        GetTableAllocations = 4,

        /// <summary>
        /// Confirming a table allocation
        /// </summary>
        ConfirmTableAllocation = 5,

        /// <summary>
        /// Delete the table allocation with the checkInId
        /// </summary>
        DeleteAllocationWithCheckInId = 6,

        /// <summary>
        /// Add a table Allocation
        /// </summary>
        AddTableAllocation = 7,

        /// <summary>
        /// Set Seating and ordering configuration. 
        /// </summary>
        SetSeatingAndOrderConfiguration = 8

    }
}
