using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// object used to process socket messages received from doshii. 
    /// </summary>
    internal class SocketMessage : JsonSerializationBase<SocketMessage>
    {
        /// <summary>
        /// a list that holds the message type and the message data. 
        /// </summary>
        [JsonProperty(PropertyName = "emit")]
        internal List<object> Emit { get; set; } 
    }
}
