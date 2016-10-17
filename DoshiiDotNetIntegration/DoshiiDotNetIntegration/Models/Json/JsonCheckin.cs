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
    internal class JsonCheckin : JsonSerializationBase<JsonCheckin>
    {
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [DataMember]
        [JsonProperty(PropertyName = "ref")]
        public string Ref { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "tableNames")]
        public List<string> TableNames { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "covers")]
        public string Covers { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "consumer")]
        public JsonConsumer Consumer { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "completedAt")]
        public DateTime? CompletedAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }


        #region serializeMembers

        public bool ShouldSerializeUri()
        {
            return false;
        }

        public bool ShouldSerializeCreatedAt()
        {
            return false;
        }

        public bool ShouldSerializeUpdatedAt()
        {
            return false;
        }

        public bool ShouldSerializeCompletedAt()
        {
            return false;
        }

        public bool ShouldSerializeCovers()
        {
            return (Covers != "0");
        }

        public bool ShouldSerializeConsumer()
        {
            return (!(Consumer == null));
        }

        public bool ShouldSerializeTableNames()
        {
            return (!(TableNames.Count == 0));
        }

        public bool ShouldSerializeRef()
        {
            return (!string.IsNullOrEmpty(Ref));
        }

        public bool ShouldSerializeId()
        {
            return false;
        }
        
        #endregion
    }
}
