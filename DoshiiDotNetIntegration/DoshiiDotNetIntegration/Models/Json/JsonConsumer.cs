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
        /// the url for the consumers photo
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "photoURL")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// is this an anonymous user. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "anonymous")]
        public bool Anonymous { get; set; }
        
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
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "address")]
        public Address Address { get; set; }

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
