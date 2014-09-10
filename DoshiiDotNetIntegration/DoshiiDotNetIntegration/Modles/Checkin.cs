using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class Checkin : JsonSerializationBase<Checkin>
    {
        string id { get; set; }
        string paypalTabId { get; set; }
        string consumerId { get; set; }
        string locationId { get; set; }
        
        /// <summary>
        /// this is really only here because it is returned as part of the received data, it can be ignored by the pos and these statuses are handled by doshii.
        /// </summary>
        string status { get; set; }
        DateTime expirationDate {get;set;}
        string gratuity {get;set;}
        DateTime updatedAt {get;set;}

        string paypalCustomerId {get;set;}
    }
}
