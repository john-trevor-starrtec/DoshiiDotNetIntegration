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
    internal class JsonPointsRedeem : JsonSerializationBase<JsonPointsRedeem>
    {
        [DataMember]
        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "points")]
        public string Points { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "appId")]
        public string AppId { get; set; }
    }
}
