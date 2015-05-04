using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
    /// Object used to process socket messages received from doshii. 
    /// </summary>
    public  class SocketMessage : JsonSerializationBase<SocketMessage>
    {
        /// <summary>
        /// DO NOT USE, the internal methods will set this value correctly and it should not be changed by the POS.
        /// A list that holds the message type and the message data. 
        /// </summary>
        [JsonProperty(PropertyName = "emit")]
        public  List<object> Emit { get; set; } 
    }
}
