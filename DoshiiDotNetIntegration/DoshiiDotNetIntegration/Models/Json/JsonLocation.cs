using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models.Json
{
    [DataContract]
    [Serializable]
    internal class JsonLocation : JsonSerializationBase<JsonLocation>
    {
        /// <summary>
        /// the DoshiiId for the venue - give this value to partners to allow them to send orders and payments to your venue. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "doshiiId")]
        string DoshiiId { get; set; }

        /// <summary>
        /// the name of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        string Name { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "addressLine1")]
        string AddressLine1 { get; set; }

        /// <summary>
        /// address of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "addressLine2")]
        string AddressLine2 { get; set; }

        /// <summary>
        /// the city element of the venue address
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "city")]
        string City { get; set; }

        /// <summary>
        /// the state element of the venue address
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "state")]
        string State { get; set; }

        /// <summary>
        /// the postal code of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "country")]
        string PostalCode { get; set; }

        /// <summary>
        /// the country element of the venue address
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "postalCode")]
        string Country { get; set; }

        /// <summary>
        /// the phone number of the venue
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "phoneNumber")]
        string PhoneNumber { get; set; }

        /// <summary>
        /// the last time the venue was disconnected - will be null if the venue is connected. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "disconnectedDate")]
        DateTime? DisconnectedDate { get; set; }
    }
}
