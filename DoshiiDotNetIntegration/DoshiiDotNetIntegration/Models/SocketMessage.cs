using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    internal class SocketMessage : JsonSerializationBase<SocketMessage>
    {
        [JsonProperty(PropertyName = "emit")]
        public List<object> Emit { get; set; } 
    }
}
