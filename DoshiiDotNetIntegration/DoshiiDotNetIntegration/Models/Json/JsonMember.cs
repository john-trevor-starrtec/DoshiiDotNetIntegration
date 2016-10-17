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
    internal class JsonMember : JsonSerializationBase<JsonMember>
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
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "address")]
        public JsonAddress Address { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "ref")]
        public string Ref { get; set; }

        private List<App> _Apps;

        [DataMember]
        [JsonProperty(PropertyName = "apps")]
        public List<App> Apps
        {
            get
            {
                if (_Apps == null)
                {
                    _Apps = new List<App>();
                }
                return _Apps;
            }
            set { _Apps = value; }
        }
    }
}
