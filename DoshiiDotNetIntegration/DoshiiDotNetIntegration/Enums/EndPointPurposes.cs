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
        DeleteAllocationFromCheckin = 3,

        /// <summary>
        /// request to get a transaction
        /// </summary>
        Transaction = 4,

        /// <summary>
        /// request to get a list of transactions from the doshii order id
        /// </summary>
        TransactionFromDoshiiOrderId = 5,

        /// <summary>
        /// Requests about orders from the doshii order id
        /// </summary>
        UnlinkedOrders = 6,

        /// <summary>
        /// Request about the consumer from the checkinId
        /// </summary>
        ConsumerFromCheckinId = 7
    }
}
