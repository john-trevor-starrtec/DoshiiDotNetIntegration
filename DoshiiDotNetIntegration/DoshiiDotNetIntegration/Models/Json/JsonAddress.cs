using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models.Json
{
    [DataContract]
    [Serializable]
    internal class JsonAddress : JsonSerializationBase<JsonAddress>
    {
        /// <summary>
        /// Line 1 of the address
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "line1")]
        public string Line1 { get; set; }

        /// <summary>
        /// Line 2 of the address
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "line2")]
        public string Line2 { get; set; }

        /// <summary>
        /// the Address city
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// the address state
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// the address post/zip code
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "postalCode")]
        public string PostalCode { get; set; }

        /// <summary>
        /// the address country
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

    }
}
