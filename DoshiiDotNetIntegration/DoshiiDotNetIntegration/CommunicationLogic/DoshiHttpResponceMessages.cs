using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// the message objec that is used to process all requests to the doshii restful api.
    /// </summary>
    internal class DoshiHttpResponceMessages
    {
        /// <summary>
        /// the status of the message responce
        /// </summary>
        internal HttpStatusCode Status { get; set; }

        /// <summary>
        /// the status description of the messae responce
        /// </summary>
        internal string StatusDescription { get; set; }

        /// <summary>
        /// the data returned in the responce if there is no data returned this may be null
        /// </summary>
        internal string Data { get; set; }

        /// <summary>
        /// the error message if the responce was unsuccessfull
        /// </summary>
        internal string ErrorMessage { get; set; }

        /// <summary>
        /// a message that may be used dring prcessing. 
        /// </summary>
        internal string Message { get; set; }

    }
}
