using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// The possible actions for communication with the doshii restful API over HTTP
    /// </summary>
    internal enum EndPointPurposes
    {
		/// <summary>
        /// Requests related to orders
        /// </summary>
        Order = 1,

        /// <summary>
        /// Requests for getting a table allocation
        /// </summary>
        GetTableAllocations = 2,

        /// <summary>
        /// Requests for deleting table allocations with a checkInId
        /// </summary>
        DeleteAllocationFromCheckin = 3,

        /// <summary>
        /// Request related to transactions
        /// </summary>
        Transaction = 4,

        /// <summary>
        /// Requests for transactions where the order related to the transactions is currently unlinked to the pos. 
        /// </summary>
        TransactionFromDoshiiOrderId = 5,

        /// <summary>
        /// Requests for orders where the order is currently unlinked to the pos.
        /// </summary>
        UnlinkedOrders = 6,

        /// <summary>
        /// Request for a consumer with a checkinId.
        /// </summary>
        ConsumerFromCheckinId = 7,

        /// <summary>
        /// Requests for a menu.
        /// </summary>
        Menu = 8,

        /// <summary>
        /// Request for menu products.
        /// </summary>
        Products = 9,

        /// <summary>
        /// Request for menu surcounts.
        /// </summary>
        Surcounts = 10,

        /// <summary>
        /// requests for location data
        /// </summary>
        Location = 11,

        /// <summary>
        /// requests for member data
        /// </summary>
        Members = 12,

        /// <summary>
        /// requests for member reqard data
        /// </summary>
        MemberRewards = 13,

        /// <summary>
        /// requests for member reqard data
        /// </summary>
        MemberRewardsRedeem = 14,

        /// <summary>
        /// requests for member reqard data
        /// </summary>
        MemberRewardsRedeemConfirm = 15,

        /// <summary>
        /// requests for member reqard data
        /// </summary>
        MemberRewardsRedeemCancel = 16,

        MemberPointsRedeem = 17,

        MemberPointsRedeemConfirm = 18,

        MemberPointsRedeemCancel = 19
        
    }
}
