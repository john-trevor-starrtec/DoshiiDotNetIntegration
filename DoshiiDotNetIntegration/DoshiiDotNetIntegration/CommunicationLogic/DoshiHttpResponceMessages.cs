﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// DO NOT USE, This class is used internally by the SDK and should not be instanciated by the pos.
    /// the message objec that is used to process all requests to the doshii restful api.
    /// </summary>
    public class DoshiHttpResponceMessages
    {
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The status of the message response.
        /// </summary>
        public HttpStatusCode Status { get; set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The status description of the messae response
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The data returned in the response if there is no data returned this may be null
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// The error message if the response was unsuccessfull
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// A message that may be used during prcessing. 
        /// </summary>
        public string Message { get; set; }

    }
}
