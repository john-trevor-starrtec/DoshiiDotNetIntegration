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
    internal class JsonBooking : JsonSerializationBase<JsonBooking>
    {
        [DataMember]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "tableNames")]
        public List<string> TableNames { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "covers")]
        public string Covers { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "consumer")]
        public JsonConsumer Consumer { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "checkinId")]
        public String checkinId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "app")]
        public String App { get; set; }

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

        public bool ShouldSerializeApp()
        {
            return false;
        }

        #endregion serializeMembers
    }
}
