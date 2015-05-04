using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// The possible order modes available for the Doshii integration
    /// </summary>
    public enum OrderModes
    {
        /// <summary>
        /// Restaurant mode, 
        /// Orders are made against a consumer and are paid for at the end of the session, this allows consumers to stay checkedIn and continue to order on the same bill. 
        /// </summary>
        RestaurantMode = 1,

        /// <summary>
        /// Bistro mode, 
        /// Orders are made and paid for in the same interaction, users are forced to pay at the time they order and are logged out of the app
        /// Users must CheckIn again to make another order. 
        /// </summary>
        BistroMode = 2
    }
}
