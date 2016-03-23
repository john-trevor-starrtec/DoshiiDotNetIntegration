using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// The Log Levels for log entries made by Doshii.
    /// </summary>
    public enum DoshiiLogLevels
    {
		/// <summary>
        /// A message primarily used for debugging.
        /// These messages should only be logged to the POS logging mechanism while debugging.
        /// </summary>
        Debug = 1,
        
        /// <summary>
        /// A message used to detail further information about the operation of the DoshiiDotNetIntegration.dll
        /// </summary>
        Info = 2,
        
        /// <summary>
        /// Warning messages that may indicate there is an issue with the use of the DoshiiDotNetIntegration.dll
        /// </summary>
        Warning = 3,

        /// <summary>
        /// An error has occurred in the DoshiiDotNetIntegration.dll. These messages should always be logged in the POS logging mechanism.
        /// </summary>
        Error = 4,

		/// <summary>
		/// A fatal error has occurred, causing the SDK to shut down. These messages should always be logged in the POS logging mechanism.
		/// </summary>
		Fatal = 5
    }
}
