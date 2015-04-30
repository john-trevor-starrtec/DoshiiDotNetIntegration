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
        /// Pos users allocate a table on the Pos once consumers are checked in
        /// </summary>
        PosAllocation = 1,

        /// <summary>
        /// Doshii customers select a table on the mobile app and this is confirmed by the pos. 
        /// The confirmation can be set to be automatic by the pos or manual depending on the implementation. 
        /// </summary>
        DoshiiAllocation = 2
    }
}
