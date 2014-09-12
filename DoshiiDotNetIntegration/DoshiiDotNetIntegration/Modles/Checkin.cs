using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class Checkin : JsonSerializationBase<Checkin>
    {
        public string id { get; set; }
        public string paypalTabId { get; set; }
        public string consumerId { get; set; }
        public string locationId { get; set; }
        
        /// <summary>
        /// this is really only here because it is returned as part of the received data, it can be ignored by the pos and these statuses are handled by doshii.
        /// </summary>
        public string status { get; set; }
        public DateTime expirationDate {get;set;}
        public string gratuity {get;set;}
        public DateTime updatedAt {get;set;}

        public string paypalCustomerId {get;set;}

        public Checkin()
        {
        }
    }
}
