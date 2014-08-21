using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Modles
{
    public class table_allocation : JsonSerializationBase<table_allocation>
    {
        /// <summary>
        /// this is the checkin Id associated with the table
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// this is the name of the table the checkin is associated with.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// this is the paypal customerId the table is associated with.
        /// </summary>
        public string customerId { get; set; }

        /// <summary>
        /// the allocaiton state of the table. 
        /// </summary>
        public Enums.AllocationStates status { get; set; }
    }
}
