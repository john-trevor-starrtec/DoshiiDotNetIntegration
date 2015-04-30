using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DoshiiDotNetIntegration.Models
{

    /// <summary>
    /// Payments that are made on the Doshii check
    /// </summary>
    [DataContract]
    [Serializable]
    public class Payment
    {
        /// <summary>
        /// A string describing the payment method eg "Cash", "EFTPOS"
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "type")]
        public string PaymentType { get; set; }

        /// <summary>
        /// The amount that has been paid in cents. 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "amount")]
        public string PaymentAmount { get; set; }
    }
}
