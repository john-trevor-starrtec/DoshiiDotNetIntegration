using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json
{
    /// <summary>
    /// A doshii consumer
    /// </summary>
    [DataContract]
    [Serializable]
    internal class JsonConsumer : JsonSerializationBase<JsonConsumer>
    {
        /// <summary>
        /// The consumers name
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// the consumers phone number
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// the consumers address line 1
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "address_line1")]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// the consumers address line 1
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "address_line2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// the consumers address city
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// the consumers address state
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// the consumers address postal code
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "postalCode")]
        public string PostalCode { get; set; }

        /// <summary>
        /// the consumers address country
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        /// <summary>
        /// Notes specific to this order, 
        /// this may include:
        /// Notes about delivery location,
        /// Notes about allergies,
        /// Notes about a booking that has been made,
        /// Notes about special requests for the delivery. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
    }
}
