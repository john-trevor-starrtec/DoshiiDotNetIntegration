using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    public class TableAllocation : JsonSerializationBase<TableAllocation>
    {
        /// <summary>
        /// this is the checkin Id associated with the table
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// this is the name of the table the checkin is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// this is the paypal customerId the table is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// this is the property that should be used to identify the client. 
        /// </summary>
        [JsonProperty(PropertyName = "paypalCustomerId")]
        public string PaypalCustomerId { get; set; }

        /// <summary>
        /// the allocaiton state of the table. 
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// the reason the allocation was rejected if the allocation was rejected. 
        /// </summary>
        [JsonProperty(PropertyName = "ReasonCode")]
        public string ReasonCode { get; set; }

        /// <summary>
        /// the checkin associated with the table allocation.
        /// </summary>
        [JsonProperty(PropertyName = "checkin")]
        public Checkin Checkin { get; set; }

        public TableAllocation()
        {
        }


    }
}
