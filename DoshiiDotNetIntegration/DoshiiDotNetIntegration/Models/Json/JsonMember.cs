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
    public class JsonMember
    {
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "address")]
        public Address Address { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "points")]
        public decimal Points { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }
}
