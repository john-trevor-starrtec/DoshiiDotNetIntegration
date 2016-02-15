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
        /// Requests about orders
        /// </summary>
        Order = 1,

        /// <summary>
        /// Getting a table allocation
        /// </summary>
        GetTableAllocations = 2,

        /// <summary>
        /// Delete the table allocation with the checkInId
        /// </summary>
        DeleteAllocationFromOrder = 3,

        /// <summary>
        /// request to get a transaction
        /// </summary>
        Transaction = 4
    }
}
