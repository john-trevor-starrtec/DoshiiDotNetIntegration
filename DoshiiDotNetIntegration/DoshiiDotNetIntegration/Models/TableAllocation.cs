using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// A table allocation object
    /// </summary>
    [DataContract]
    [Serializable]
    public class TableAllocation : JsonSerializationBase<TableAllocation>
    {
        /// <summary>
        /// This is the CheckInId associated with the TableAllocation
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// This is the Name of the table the CheckIn is associated with.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// This is the PayPal CustomerId the table is associated with.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// This is the property that should be used to identify the client. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "meerkatConsumerId")]
        public string MeerkatConsumerId { get; set; }

        /// <summary>
        /// The allocation status of the table. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// The CheckIn associated with the TableAllocation.
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "checkin")]
        public Checkin Checkin { get; set; }

        /// <summary>
        /// The reason the TableAllocation was rejected if it was rejected. 
        /// </summary>
        public Enums.TableAllocationRejectionReasons rejectionReason;

        /// <summary>
        /// Constructor. 
        /// </summary>
        public TableAllocation()
        {
        }
    }
}
