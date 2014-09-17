using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// the possible order modes used for doshii integration
    /// </summary>
    public enum OrderModes
    {
        /// <summary>
        /// restaurant mode, orders are made against a consumer and are paid for at the end of the session, this allows consumers to stay checkedIn and continue to order on the same bill. 
        /// </summary>
        RestaurantMode = 1,

        /// <summary>
        /// bistro mode, orders are made and payed for in the same interaction, users are logged out after thay have paid for the order and must CheckIn again to make another order. 
        /// </summary>
        BistroMode = 2
    }
}
