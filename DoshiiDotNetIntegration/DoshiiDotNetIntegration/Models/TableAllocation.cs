using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// a table allocation object
    /// </summary>
    [DataContract]
    [Serializable]
    public class TableAllocation : JsonSerializationBase<TableAllocation>
    {
        /// <summary>
        /// this is the checkin Id associated with the table
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// this is the name of the table the checkin is associated with.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// this is the paypal customerId the table is associated with.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// this is the property that should be used to identify the client. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "paypalCustomerId")]
        public string PaypalCustomerId { get; set; }

        /// <summary>
        /// the allocaiton state of the table. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// the reason the allocation was rejected if the allocation was rejected. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "ReasonCode")]
        public string ReasonCode { get; set; }

        /// <summary>
        /// the checkin associated with the table allocation.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "checkin")]
        public Checkin Checkin { get; set; }

        public TableAllocation()
        {
        }


    }
}
