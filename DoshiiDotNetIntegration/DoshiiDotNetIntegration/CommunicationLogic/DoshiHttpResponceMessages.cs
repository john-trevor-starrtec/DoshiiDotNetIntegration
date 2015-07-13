using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// DO NOT USE, This class is used internally by the SDK and should not be instantiated by the pos.
    /// the message object that is used to process all requests to the doshii restful API.
    /// </summary>
    internal class DoshiHttpResponceMessages
    {
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The status of the message response.
        /// </summary>
        internal HttpStatusCode Status { get; set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The status description of the message response
        /// </summary>
        internal string StatusDescription { get; set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The data returned in the response if there is no data returned this may be null
        /// </summary>
        internal string Data { get; set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The error message if the response was unsuccessful
        /// </summary>
        internal string ErrorMessage { get; set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// A message that may be used during processing. 
        /// </summary>
        internal string Message { get; set; }

    }
}
