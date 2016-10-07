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
    internal class JsonTable : JsonSerializationBase<JsonTable>
    {
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "maxCovers")]
        public string Covers { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "criteria")]
        public JsonTableCriteria Criteria { get; set; }

    }
}
