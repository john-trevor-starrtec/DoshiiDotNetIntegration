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
    internal class JsonMemberToUpdate : JsonSerializationBase<JsonMemberToUpdate>
    {
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "address")]
        public JsonAddress Address { get; set; }

        public bool ShouldSerializePhone()
        {
            return (!string.IsNullOrEmpty(Phone));
        }
    }
}
