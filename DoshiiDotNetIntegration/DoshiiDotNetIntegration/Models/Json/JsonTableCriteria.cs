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
    internal class JsonTableCriteria : JsonSerializationBase<JsonTableCriteria>
    {
        [DataMember]
        [JsonProperty(PropertyName = "isCommunal")]
        public bool IsCommunal { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "canMerge")]
        public bool CanMerge { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "isSmoking")]
        public bool IsSmoking { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "isOutdoor")]
        public bool IsOutdoor { get; set; }

        //public bool ShouldSerializeCanMerge()
        //{
        //    return false;
        //}

        //public bool ShouldSerializeIsSmoking()
        //{
        //    return false;
        //}

    }
}
