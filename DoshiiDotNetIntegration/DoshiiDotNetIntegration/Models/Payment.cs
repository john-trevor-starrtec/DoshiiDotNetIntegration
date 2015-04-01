using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{

    /// <summary>
    /// payments that can be made aginst the the doshii orders
    /// </summary>
    [DataContract]
    [Serializable]
    public class Payment
    {
        [DataMember]
        [JsonProperty(PropertyName = "type")]
        public string PaymentType { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "amount")]
        public string PaymentAmount { get; set; }
    }
}
