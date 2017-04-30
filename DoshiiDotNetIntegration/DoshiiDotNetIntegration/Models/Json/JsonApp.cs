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
    internal class JsonApp
    {

        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "points")]
        public string Points { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "ref")]
        public string Ref { get; set; }
    }
}
