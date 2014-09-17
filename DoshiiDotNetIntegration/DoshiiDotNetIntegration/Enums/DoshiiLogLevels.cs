using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Enums
{
    /// <summary>
    /// the possible levels for logging messages implemented by doshii
    /// </summary>
    public enum DoshiiLogLevels
    {
        /// <summary>
        /// a message prymarily used for debugging, these messages should only be logged while debugging
        /// </summary>
        Debug = 1,
        
        /// <summary>
        /// a message used to detail further infomraton about the operation of the DoshiiDotNetIntegration.dll
        /// </summary>
        Info = 2,
        
        /// <summary>
        /// warning messages that may indicate there is an issue with the use of the DoshiiDotNetIntegration.dll
        /// </summary>
        Warning = 3,

        /// <summary>
        /// and error has occured in the DoshiiDotNetIntegration.dll, these messages should always be logged. 
        /// </summary>
        Error = 4
    }
}
