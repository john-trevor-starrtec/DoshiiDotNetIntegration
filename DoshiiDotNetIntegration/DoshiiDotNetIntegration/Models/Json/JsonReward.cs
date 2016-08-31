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
    internal class JsonReward : JsonSerializationBase<JsonReward>
    {
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "appName")]
        public string AppName { get; set; }


        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "uri")]
        public Uri Uri { get; set; }

    }
}
