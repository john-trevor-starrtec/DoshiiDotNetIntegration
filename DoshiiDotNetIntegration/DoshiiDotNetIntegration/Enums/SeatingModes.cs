using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// the possible seating modes that can be implemented in the doshii integation. 
    /// </summary>
    public enum SeatingModes
    {
        /// <summary>
        /// pos users allocate a table once consumers are checked in
        /// </summary>
        PosAllocation = 1,

        /// <summary>
        /// doshii customers select a table on the mobile app and this is confirmed by the pos. 
        /// </summary>
        DoshiiAllocation = 2
    }
}
